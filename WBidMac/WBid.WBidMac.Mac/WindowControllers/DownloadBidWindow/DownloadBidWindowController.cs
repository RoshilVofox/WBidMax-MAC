
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using MonoMac.Foundation;
//using MonoMac.AppKit;
using WBid.WBidiPad.SharedLibrary;

#region NameSpace
using System;
using CoreGraphics;
using Foundation;
using AppKit;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.iOS.Utility;
using System.ServiceModel;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WBid.WBidiPad.Model.SWA;

//using iOSPasswordStorage;
using Security;
using System.Linq;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary.Parser;
using System.IO;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary.Parser;
using WBid.WBidiPad.Core;
using System.Collections.ObjectModel;
using WBid.WBidiPad.SharedLibrary.Serialization;
using WBid.WBidiPad.PortableLibrary;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using WBid.WBidiPad.SharedLibrary.Utility;
using VacationCorrection;

//using MiniZip.ZipArchive;
//using System.IO.Packaging;
using System.IO.Compression;
using WBid.WBidiPad.Core.Enum;
using System.Globalization;
using WBid.WBidMac.Mac.WindowControllers;
using WBid.WBidMac.Mac.WindowControllers.CustomAlert;
using WBid.WBidMac.Mac.ViewControllers.CustomAlertView;

#endregion

namespace WBid.WBidMac.Mac
{
	public partial class DownloadBidWindowController : AppKit.NSWindowController
	{
		#region Properties & Variables

		public Dictionary<string, TripMultiVacData> VacationData { get; set; }


		private bool _waitCompanyVADialog;
		private static ObservableCollection<Trip> _trip;

		public static ObservableCollection<Trip> Trip {
			get {
				return _trip ?? (_trip = new ObservableCollection<Trip> ());
			}
			set {
				_trip = value;
			}
		}

		WBidDataDwonloadAuthServiceClient client;

		private Guid token;
		private bool _isCompanyServerData;

		private string _sessionCredentials = string.Empty;

		private DownloadInfo _downloadFileDetails;

		private DownloadBid _downloadBidObject = new DownloadBid ();

		//Hold the totla number of seniority list item and domcile seniority
		private SeniorityListItem _seniorityListItem;

		private List<SeniorityListMember> seniorityListMembers;

		List<NSObject> arrObserver = new List<NSObject> ();
		NSAlert alertVW;

		Dictionary<string, Trip> trips = null;
		Dictionary<string, Line> lines = null;


		/// <summary>
		/// create single instance of TripTtpParser class
		/// </summary>
		private TripTtpParser _tripTtpParser;

		public TripTtpParser TripTtpParser {
			get {
				return _tripTtpParser ?? (_tripTtpParser = new TripTtpParser ());
			}
		}

		private CalculateTripProperties _calculateTripProperties;

		public CalculateTripProperties CalculateTripProperties {
			get {
				return _calculateTripProperties ?? (_calculateTripProperties = new CalculateTripProperties ());
			}
		}


		private CalculateLineProperties _calculateLineProperties;

		public CalculateLineProperties CalculateLineProperties {
			get {
				return _calculateLineProperties ?? (_calculateLineProperties = new CalculateLineProperties ());
			}
		}

		#endregion

		public bool IsMissingTripFailed { get; set; }

		#region Constructors

		// Called when created from unmanaged code
		public DownloadBidWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public DownloadBidWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public DownloadBidWindowController () : base ("DownloadBidWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new DownloadBidWindow Window {
			get {
				return (DownloadBidWindow)base.Window;
			}
		}

		private bool running = true;

		public bool Running {
			get { return running; } 
			private set { running = value; }
		}

		// Call this method to close the modal window when you're done
		private void CloseModal (NSObject sender)
		{
			this.Window.OrderOut (sender);
			this.Window.Close ();
			this.Running = false;
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.Window.WillClose += (object sender, EventArgs e) => {
					foreach (NSObject obj in arrObserver) {
						NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
					}
					if (alertVW != null) {
						alertVW.Window.Close ();
						alertVW = null;
					}
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				this.ShouldCascadeWindows = false;
				if (GlobalSettings.isHistorical) {
					this.Window.Title = "Downloading Historical Bid Data";
				} else
				{
					this.Window.Title = "Downloading New Bid Data";
				}
				lblTitle.StringValue = SetTitle ();
				GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus ();
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
				GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
				GlobalSettings.MenuBarButtonStatus.IsEOM = false;
				GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
				GlobalSettings.MenuBarButtonStatus.IsMIL = false;
				IsMissingTripFailed = false;
				//			
				
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
				client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 30);
				client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
				//client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
				client.LogTimeOutDetailsCompleted += client_LogTimeOutDetailsCompleted;
				// Perform any additional setup after loading the view, typically from a nib.
				//this.prgrsVw.SetProgress(0.0f, false);
				this.observeNotifications ();
				
				_isCompanyServerData = GlobalSettings.WBidINIContent.Data.IsCompanyData;
				
				//startProgress ();
				new System.Threading.Thread (new System.Threading.ThreadStart (() => {
					this.initiateDownloadProcess ();
				})).Start ();
				
				//			BeginInvokeOnMainThread (() => {
				//				this.initiateDownloadProcess ();
				//			});
				InvokeOnMainThread (() => {
					progDownload.StartAnimation (null);
				});
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				InvokeOnMainThread (() => {
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				} );
			}
		}

		/// <summary>
		/// set the title of this page.
		/// </summary>
		private string SetTitle ()
		{
			string domicile = GlobalSettings.DownloadBidDetails.Domicile;
			string position = GlobalSettings.DownloadBidDetails.Postion;
			System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo ();
			string strMonthName = mfi.GetMonthName (GlobalSettings.DownloadBidDetails.Month).ToString ();
			string round = GlobalSettings.DownloadBidDetails.Round == "D" ? "1st Round" : "2nd Round";
			string downloadTitle = domicile + " - " + position + " - " + round + " - " + strMonthName + " - " + GlobalSettings.DownloadBidDetails.Year;
			return downloadTitle;
		}

		private void observeNotifications ()
		{
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"reachabilityCheckSuccess", reachabilityCheck));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"cwaCheckSuccess", cwaCredentialsCheck));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"authCheckSuccess", authCheckSuccess));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"getDataFilesSuccess", getDataFilesSuccess));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"parseDataSuccess", parseDataSuccess));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"saveDataSuccess", saveDataSuccess));
			arrObserver.Add (NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"calcVACCorrection", calcVACCorrection));
		}

		public void reachabilityCheck (NSNotification n)
		{
			InvokeOnMainThread (() => {
				ckDownloadSteps.SelectCellWithTag (0);
			});
			Console.WriteLine ("reachabilityCheck");
		}

		public void cwaCredentialsCheck (NSNotification n)
		{
			InvokeOnMainThread (() => {
				ckDownloadSteps.SelectCellWithTag (1);
			});
			Console.WriteLine ("cwaCredentialsCheck");
		}

		public void authCheckSuccess (NSNotification n)
		{
			InvokeOnMainThread (() => {
				ckDownloadSteps.SelectCellWithTag (2);
			});
			Console.WriteLine ("authCheckSuccess");
		}

		public void getDataFilesSuccess (NSNotification n)
		{
			InvokeOnMainThread (() => {
				ckDownloadSteps.SelectCellWithTag (3);
			});
			Console.WriteLine ("getDataFilesSuccess");
			//startProgress ();
		}

		public void parseDataSuccess (NSNotification n)
		{
			InvokeOnMainThread (() => {
				ckDownloadSteps.SelectCellWithTag (4);
			});
			Console.WriteLine ("parseDataSuccess");
			//startProgress ();
		}

		public void saveDataSuccess (NSNotification n)
		{
			Console.WriteLine ("saveDataSuccess");
            if (GlobalSettings.IsVacationCorrection || GlobalSettings.IsFVVacation)
				applyVacation ();
			else if (GlobalSettings.IsOverlapCorrection)
				applyOverlapCorrection ();
			InvokeOnMainThread (() => {
				foreach (NSObject obj in arrObserver) {
					NSNotificationCenter.DefaultCenter.RemoveObserver (obj);
				}

				if (alertVW != null) {
					alertVW.Window.Close ();
					alertVW = null;
				}

//					lineViewController lineController = new lineViewController();
//					UINavigationController navController = new UINavigationController(lineController);
//					navController.NavigationBar.BarStyle = UIBarStyle.Black;
//					navController.NavigationBar.Hidden = true;
//					UIApplication.SharedApplication.KeyWindow.RootViewController = navController;
				CommonClass.isHomeWindow = false;
				CommonClass.HomeController.LoadContent ();
//					this.Window.Close ();
//					this.Window.OrderOut (this);
//					NSApplication.SharedApplication.StopModal ();
				this.CloseModal (this);
				var mainWindowController = new MainWindowController ();
				CommonClass.MainController = mainWindowController;
				mainWindowController.Window.MakeKeyAndOrderFront (this);
				CommonClass.HomeController.Window.Close ();
				CommonClass.HomeController.Window.OrderOut (this);

				ShowNewsAndCoverLetter ();
				//Show Month to Month Vacation alert
				ShowMonthTomonthAlert();
			});
		}
		private void ShowMonthTomonthAlert()
		{
			List<Weekday> lstweekdays = GetWeekDays(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);
			List<Vacation> uservacation = GlobalSettings.WBidStateCollection.Vacation;
			List<Weekday> vacationweeks = lstweekdays.Where(x => uservacation.Any(y => DateTime.Parse(y.StartDate) == x.StartDate && DateTime.Parse(y.EndDate) == x.EndDate)).ToList();
			
			bool isneedtoShowAlert = vacationweeks.Any(x => x.Code.Contains("A") || x.Code.Contains("E"));
			if (isneedtoShowAlert == true && GlobalSettings.CurrentBidDetails.Postion != "FA")
			{


				string AlertText="";
				var startDateA = "";
				var endDateA = "";
				var startDateE = "";
				var endDateE = "";
				

				var codeArray = vacationweeks.Select(x => x.Code);

			 if (codeArray.Contains("A") && codeArray.Contains("E"))
				{
					//AE Vacation
					startDateA = vacationweeks.Find(x => x.Code == "A").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "A").StartDate.ToString("MMM");
					endDateA = vacationweeks.Find(x => x.Code == "A").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "A").EndDate.ToString("MMM");

					startDateE = vacationweeks.Find(x => x.Code == "E").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "E").StartDate.ToString("MMM");
					endDateE = vacationweeks.Find(x => x.Code == "E").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "E").EndDate.ToString("MMM");

					AlertText = "You have 'A & E' week vacation: " + startDateA + " - " + endDateA + " and " + startDateE + " - " + endDateE;
					AlertText += "\n\nA weeks generally are the lead out month and E weeks generally are the lead-in month of a month-to-month vacation..";
					AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";
				}
				else if (codeArray.Contains("A"))
				{
					//A Vacation
					startDateA = vacationweeks.Find(x => x.Code == "A").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "A").StartDate.ToString("MMM");
					endDateA = vacationweeks.Find(x => x.Code == "A").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "A").EndDate.ToString("MMM");
					AlertText = "You have 'A' week vacation: " + startDateA + " - " + endDateA;
					AlertText += "\n\nA weeks generally are the lead out month of a month-to - month vacation.";
					AlertText += "\n\nThere are opportunities with Month-To-Month Vacations, but there are ALSO limitations.";

				}
				else if (codeArray.Contains("E"))
				{
					//E Vacation
					startDateE = vacationweeks.Find(x => x.Code == "E").StartDate.Day + " " + vacationweeks.Find(x => x.Code == "E").StartDate.ToString("MMM");
					endDateE = vacationweeks.Find(x => x.Code == "E").EndDate.Day + " " + vacationweeks.Find(x => x.Code == "E").EndDate.ToString("MMM");
					AlertText = "You have 'E' week vacation: " + startDateE + " - " + endDateE;
					AlertText += "\n\nE weeks generally are the lead-in month of a month-to-month vacation..";
				}

				AlertText += "\n\nWe suggest you read the following documents to improve your bidding knowledge";

				var monthView = new MonthToMonthAlertViewController();
				monthView.Alert = AlertText;
				CommonClass.MainController.Window.AddChildWindow(monthView.Window, NSWindowOrderingMode.Above);
				NSApplication.SharedApplication.RunModalForWindow(monthView.Window);

			}
		}
		private List<Weekday> GetWeekDays(int year, int month)
		{

			List<Weekday> dates = new List<Weekday>();
			CultureInfo ci = new CultureInfo("en-US");

			for (int i = 1; i <= ci.Calendar.GetDaysInMonth(year, month); i++)
			{

				if (new DateTime(year, month, i).DayOfWeek == DayOfWeek.Saturday)
				{
					dates.Add(new Weekday { Day = new DateTime(year, month, i).AddDays(-6).Day, StartDate = new DateTime(year, month, i).AddDays(-6).Date, EndDate = new DateTime(year, month, i) });
				}

			}
			//need to add one extra sunday 
			dates.Add(new Weekday { Day = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(7).Day, StartDate = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(7).Date, EndDate = new DateTime(year, month, dates[dates.Count - 1].Day).AddDays(13).Date });
			for (int i = 0; i < dates.Count; i++)
			{
				switch (i)
				{
					case 0:
						dates[0].Code = "A";
						break;
					case 1:
						dates[1].Code = "B";
						break;
					case 2:
						dates[2].Code = "C";
						break;
					case 3:
						dates[3].Code = "D";
						break;
					case 4:
						dates[4].Code = "E";
						break;
					case 5:
						dates[5].Code = "F";
						break;
				}
			}
			return dates;

		}
		private void applyOverlapCorrection()
		{
			ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
			reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
			GlobalSettings.MenuBarButtonStatus.IsOverlap = true;
		}
		private void applyVacation()
		{
            //SET FV Vacation 
            PerformFVVacation();
			WBidCollection.GenarateTempAbsenceList ();
			PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
			RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
			prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
			RecalcalculateLineProperties.CalcalculateLineProperties ();
		}
        private void PerformFVVacation()
        {
            //SET FV Vacation 
            if (GlobalSettings.IsFVVacation)
            {
                //GlobalSettings.WBidStateCollection.FVVacation = GlobalSettings.FVVacation;
                FVVacation objFvVacation = new FVVacation();
                GlobalSettings.Lines = new ObservableCollection<Line>(objFvVacation.SetFVVacationValuesForAllLines(GlobalSettings.Lines.ToList(), VacationData));
            }
        }
		public void calcVACCorrection (NSNotification n)
		{
			ckDownloadSteps.SelectCellWithTag (6);
			Console.WriteLine ("vacationCrrectionSuccess");
		}

		private void initiateDownloadProcess ()
		{

			try {
				_downloadFileDetails = new DownloadInfo ();
				_downloadFileDetails.UserId = CommonClass.UserName;
				_downloadFileDetails.Password = CommonClass.Password;
				
				//checking  the internet connection available
				//==================================================================================================================
                if (Reachability.CheckVPSAvailable()) {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("reachabilityCheckSuccess", null);
				
					//checking CWA credential
					//==================================================================================================================
					//				InvokeOnMainThread (() => {
					//					progDownload.DoubleValue = 0.8;
					//				});
				
					//this.startProgress();
					//				InvokeOnMainThread(() =>
					//					{
					//						var alert = new NSAlert ();
					//						alert.AlertStyle = NSAlertStyle.Warning;
					//						alert.MessageText = "WBidMax";
					//						alert.InformativeText = "Invalid Username or Password";
					//						alert.AddButton("OK");
					//						((NSButton)alert.Buttons[0]).Activated += (sender, e) => {
					//							DismissCurrentView();
					//						};
					//						alert.BeginSheet (this.Window);
					//					});
					//				return;
				
					Authentication authentication = new Authentication ();
					string authResult = authentication.CheckCredential (_downloadFileDetails.UserId, _downloadFileDetails.Password);
				
				
					if (authResult.Contains ("ERROR: ")) {
						CommonClass.Password = string.Empty;
						WBidLogEvent objlogs = new WBidLogEvent();
						objlogs.LogBadPasswordUsage(_downloadFileDetails.UserId,true, authResult);
						InvokeOnMainThread (() => {

                            //var customAlert = new CustomAlertBoxController();
                            //customAlert.AlertType = "InvalidCredential";
                            //customAlert.objDownloadWindow = this;
                            //this.Window.AddChildWindow(customAlert.Window, NSWindowOrderingMode.Above);
                            //NSApplication.SharedApplication.RunModalForWindow(customAlert.Window);

							var panel = new NSPanel();
							var customAlert = new CustomAlertViewController();
							customAlert.AlertType = "InvalidCredential";
							customAlert.objDownloadWindow = this;
							CommonClass.Panel = panel;
							panel.SetContentSize(new CGSize(430, 350));
							panel.ContentView = customAlert.View;
							NSApplication.SharedApplication.BeginSheet(panel, this.Window);

							//CustomAlertWindow customAlert = new CustomAlertWindow();
							//customAlert.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
							//UINavigationController nav = new UINavigationController(customAlert);
							//customAlert.AlertType = "InvalidCredential";
							//nav.NavigationBarHidden = true;
							//nav.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
							//this.PresentViewController(nav, true, null);

							//string err = string.Empty;
							//                     var alert = new NSAlert();
							//                     alert.AlertStyle = NSAlertStyle.Warning;
							//                     alert.MessageText = "Login Failed";
							//                     err = "Bid Package Data\r\n\r\n";
							//                     err += "This Typically Occures due to invalid UserId or Password. Make Sure your UserId and Password are correct. You must use your current crew Portal Login\r\n\r\n";
							//                     err += authResult.Replace("ERROR: ", " ");
							//                     alert.InformativeText = err;
							//                     alert.AddButton("OK");
							//                     ((NSButton)alert.Buttons[0]).Activated += (sender, e) =>
							//                     {
							//                         DismissCurrentView();
							//                     };
							//                     alert.BeginSheet(this.Window);
							//alertVW = new UIAlertView("WBidMax", "Invalid Username or Password", null, "OK", null);
							//alertVW.Clicked += (object sender, UIButtonEventArgs e) =>
							//{
							//    DismissCurrentView();
							//};
							//alertVW.Show();

						});
					} else if (authResult.Contains ("Exception")) {
				
				
						//Need to log submit time out
				
						//WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
						//obgWBidLogEvent.LogTimeoutBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber, authResult);
				
				
						InvokeOnMainThread (() => {
							var alert = new NSAlert ();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.MessageText = "WBidMax";
							alert.InformativeText = "Your attempt to submit a bid or download bid data has failed. Specifically, the Southwest Airlines server did not respond with a certain time, and as a result, you received a Server Timeout.\n\nThis can happen for many reasons.  Our suggestion is to keep trying over the next 10 minutes or so, and if the app still fails to submit a bid or download bid data, we suggest the following:\n\nChange your internet connection.You can also try to use your cell phone as a hotspot for your internet connection \n\nFinally, send us an email if you are continuously having trouble.";

							//alert.InformativeText = "The company server is down. They have been notified.  We don't know how long it could take to bring the server back on line.  Most of the time it is within 10-20 minutes, but we have seen this server down for 6-7 hours also.";
							alert.AddButton ("OK");
							((NSButton)alert.Buttons [0]).Activated += (sender, e) => {
								DismissCurrentView ();
							};
							alert.BeginSheet (this.Window);
				
							//							alertVW = new UIAlertView("Warning", "The company server is down.  They have been notified.  We don�t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.", null, "OK", null);
							//							alertVW.Clicked += (object sender, UIButtonEventArgs e) =>
							//							{
							//								DismissCurrentView();
							//							};
							//							alertVW.Show();
						});
					} else {
						NSNotificationCenter.DefaultCenter.PostNotificationName ("cwaCheckSuccess", null);
				
						//this.startProgress();
				
						_sessionCredentials = authResult;
						//checking authorization
						//==================================================================================================================
				
						ClientRequestModel clientRequestModel = new ClientRequestModel ();
						clientRequestModel.Base = GlobalSettings.DownloadBidDetails.Domicile;
						clientRequestModel.BidRound = (GlobalSettings.DownloadBidDetails.Round == "D") ? 1 : 2;
						clientRequestModel.Month = new DateTime (GlobalSettings.DownloadBidDetails.Year, GlobalSettings.DownloadBidDetails.Month, 1).ToString ("MMM").ToUpper ();
						clientRequestModel.Postion = GlobalSettings.DownloadBidDetails.Postion;
						clientRequestModel.OperatingSystem = CommonClass.OperatingSystem;
						clientRequestModel.Platform = CommonClass.Platform;
						if (GlobalSettings.isHistorical)
						{
							clientRequestModel.RequestType = (int)RequestTypes.DownnloadHostoricalBid;
						}
						else
						{
							clientRequestModel.RequestType = (int)RequestTypes.DownnloadBid;
						}
						token = Guid.NewGuid ();
						clientRequestModel.Token = token;
						clientRequestModel.Version = CommonClass.AppVersion; //"1.3.1.0";//System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
						//clientRequestModel.Version = "4.0.31.2";
						clientRequestModel.EmployeeNumber = Convert.ToInt32 (Regex.Match (_downloadFileDetails.UserId, @"\d+").Value);
				
						// client.GetAuthorizationDetailsAsync(clientRequestModel);
						InvokeOnMainThread (() => {
							client.GetAuthorizationforMultiPlatformAsync (clientRequestModel);
				
						});
					}
				} else {
					InvokeOnMainThread (() => {
                        string alertmessage = GlobalSettings.VPSDownAlert;
                        if (Reachability.IsSouthWestWifiOr2wire())
                        {
                            alertmessage = GlobalSettings.SouthWestConnectionAlert;
                        }
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Warning;
						alert.MessageText = "WBidMax";
                        alert.InformativeText = alertmessage;
						alert.AddButton ("OK");
						((NSButton)alert.Buttons [0]).Activated += (sender, e) => {
							DismissCurrentView ();
						};
						alert.BeginSheet (this.Window);
				
						//						alertVW = new UIAlertView("WBidMax", "Connectivity not available", null, "OK", null);
						//						alertVW.Clicked += (object sender, UIButtonEventArgs e) =>
						//						{
						//							DismissCurrentView();
						//						};
						//						alertVW.Show();
						//						NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
					});
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				InvokeOnMainThread (() => {
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				} );
			}
		}

		void client_LogTimeOutDetailsCompleted (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{

		}

		void client_GetAuthorizationforMultiPlatformCompleted (object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
		{
			try {
				bool timeout = false;
				if (e.Error != null && e.Error.Message != "") {
					client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 60);
					client.LogTimeOutDetailsAsync (token);
					timeout = true;
				}

				if (timeout || e.Result != null) {
					ServiceResponseModel serviceResponseModel = new ServiceResponseModel ();
					if (timeout)
						serviceResponseModel.IsAuthorized = true;
					else
						serviceResponseModel = e.Result;
					

					if (serviceResponseModel.IsAuthorized) 
					{
						GlobalSettings.IsNeedToDownloadSeniorityUser=serviceResponseModel.IsNeedToDownloadSeniorityFromServer;
						NSNotificationCenter.DefaultCenter.PostNotificationName ("authCheckSuccess", null);
						//this.startProgress();
						_downloadFileDetails.SessionCredentials = _sessionCredentials;
						_downloadFileDetails.DownloadList = WBid.WBidiPad.PortableLibrary.WBidCollection.GenarateDownloadFileslist (GlobalSettings.DownloadBidDetails);
						List<DownloadedFileInfo> lstDownloadedFiles = DownloadFiles (_downloadFileDetails);
						if(!GlobalSettings.isHistorical)
						{
						if (lstDownloadedFiles == null) {
							InvokeOnMainThread (() => {
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.MessageText = "WBidMax";
								alert.InformativeText = "Data Transfer Failed";
								alert.AddButton ("OK");
								((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
									DismissCurrentView ();
								};
								alert.BeginSheet (this.Window);

//									alertVW = new UIAlertView("Data Transfer Failed", "Error", null, "OK", null);
//									alertVW.Clicked += (object obj, UIButtonEventArgs f) =>
//									{
//										DismissCurrentView();
//									};
//									alertVW.Show();
							});
							return;
						}

						_downloadBidObject.SaveDownloadedBidFiles (lstDownloadedFiles, WBidHelper.GetAppDataPath ());
						string zipFileName = _downloadFileDetails.DownloadList.Where (x => x.Contains (".737")).FirstOrDefault ();
						DownloadedFileInfo zipFile = lstDownloadedFiles.FirstOrDefault (x => x.FileName == zipFileName);

						if (zipFile.IsError) {

							string errorMessage = string.Empty;
							if (zipFile.Message.Contains ("BIDINFO DATA NOT AVAILABLE")) {

								errorMessage += "The Requested data doesnot exist on  the SWA servers . make sure proper month is selected and you are within the normal timeframe for the request\r\n";
								errorMessage += zipFile.Message.Replace ("ERROR: ", " ");

							} else {
								errorMessage += zipFile.Message.Replace ("ERROR: ", " ");

							}


							InvokeOnMainThread (() => {
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.MessageText = "WBidMax";
								alert.InformativeText = "Data Transfer Failed\r\n\r\n" + errorMessage;
								alert.AddButton ("OK");
								((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
									DismissCurrentView ();
								};
								alert.BeginSheet (this.Window);

//									alertVW = new UIAlertView("Data Transfer Failed", errorMessage, null, "OK", null);
//									alertVW.Clicked += (object obj, UIButtonEventArgs f) =>
//									{
//										DismissCurrentView();
//									};
//									alertVW.Show();
							});
						} else {

							string path = WBidHelper.GetAppDataPath () + "/" + Path.GetFileNameWithoutExtension (zipFileName);

							if (!(File.Exists (path + "/" + "TRIPS") && File.Exists (path + "/" + "PS"))) {

								InvokeOnMainThread (() => {
									var alert = new NSAlert ();
									alert.AlertStyle = NSAlertStyle.Warning;
									alert.MessageText = "WBidMax";
									alert.InformativeText = "There is an error while downloading the data. Please check your internet connection and try again.";
									alert.AddButton ("OK");
									((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
										DismissCurrentView ();
									};
									alert.BeginSheet (this.Window);
								});
								return;
							}
								
							//complted the download process
							WBidHelper.SetCurrentBidInformationfromZipFileName (zipFileName,false);

							//Write to currentBidDetailsfile for Error log
							FileOperations.WriteCurrentBidDetails (WBidHelper.GetAppDataPath () + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData ());

							//Download Wbid files
							DownloadWBidFiles ();

							ReadWbUpdateAndUpdateINIFile ();

							//  Set Cover letter file
							if (GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter) {
								DownloadedFileInfo coverLetter = lstDownloadedFiles.FirstOrDefault (x => x.FileName.Contains ("C.TXT") && !x.IsError);
								if (coverLetter != null) {

									GlobalSettings.IsCoverletterShowFileName = coverLetter.FileName;
									//InvokeOnMainThread(() =>
									//{
									//    webPrint fileViewer = new webPrint();
									//    this.PresentViewController(fileViewer, true, () =>
									//    {
									//        fileViewer.loadFileFromUrl(coverLetter.FileName);
									//    });
									//});

								}
							}



							NSNotificationCenter.DefaultCenter.PostNotificationName ("getDataFilesSuccess", null);

							// string zipFileName = _downloadFileDetails.DownloadList.Where(x => x.Contains(".737")).FirstOrDefault();

							ParseData (zipFileName);
						}
					}

					} 
					else 
					{
						if (serviceResponseModel.Message != null && serviceResponseModel.Type == "Invalid Account") 
						{
							var bodyContent = GetInvalidAccountMessage ();
							WBidMail objMailAgent = new WBidMail ();
							objMailAgent.SendMailtoAdmin (bodyContent.ToString (), GlobalSettings.WbidUserContent.UserInformation.Email, "User has Invalid Account");
						}
						if (serviceResponseModel.Message != null && serviceResponseModel.Type == "Invalid Version") 
						{
							InvokeOnMainThread (() => {
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.MessageText = "WBidMax";
								alert.InformativeText = serviceResponseModel.Message;
								alert.AddButton ("Download");
								alert.AddButton ("Later");
								((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
									DismissCurrentView ();
									CommonClass.AppDelegate.PerformVersionDownload ();
								};
								((NSButton)alert.Buttons [1]).Activated += (senderr, ee) => {
									DismissCurrentView ();
								};
								alert.BeginSheet (this.Window);
							});
						} 
						else 
						{
							InvokeOnMainThread (() => {
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.MessageText = "WBidMax";
								alert.InformativeText = serviceResponseModel.Message;
								//alert.AddButton ("Go to wbidmax.com and Subscribe");
								alert.AddButton ("OK");
								//alert.AddButton ("Quit WBid");
								((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
									DismissCurrentView ();
								};

								alert.BeginSheet (this.Window);
							});
						}
					}
				}
				//ShowNewsAndCoverLetter();
			} catch (Exception ex) {


				InvokeOnMainThread (() => {

					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				});
			}
		}
		void HandleDownloadHistoricalDataCompleted (object sender, DownloadHistoricalDataCompletedEventArgs e)
		{
			if (e.Result!=null) 
			{
				List<DownloadedFileInfo> lstDownloadedFiles = new List<DownloadedFileInfo> ();
				DownloadedFileInfo sWAFileInfo = new DownloadedFileInfo();
				sWAFileInfo.byteArray = e.Result.Data;
				sWAFileInfo.FileName = e.Result.Title;

				sWAFileInfo.IsError = (e.Result.Data == null) ? true : false;

				lstDownloadedFiles.Add(sWAFileInfo);




				if (lstDownloadedFiles == null) {
					InvokeOnMainThread (() => {
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Warning;
						alert.MessageText = "WBidMax";
						alert.InformativeText = "Data Transfer Failed";
						alert.AddButton ("OK");
						((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
							DismissCurrentView ();
						};
						alert.BeginSheet (this.Window);

					});
					return;
				}
				//Download Bid line file.
				if (GlobalSettings.DownloadBidDetails.Round!="D")
				{
					WBidDataDownloadAuthorizationService.Model.BidDetails bidDetails = new WBidDataDownloadAuthorizationService.Model.BidDetails ();
					bidDetails.Month = GlobalSettings.DownloadBidDetails.Month;
					bidDetails.Year = GlobalSettings.DownloadBidDetails.Year;
					bidDetails.Round = GlobalSettings.DownloadBidDetails.Round == "D" ? 1 : 2;
					bidDetails.Domicile = GlobalSettings.DownloadBidDetails.Domicile;
					bidDetails.Position = GlobalSettings.DownloadBidDetails.Postion;

					bidDetails.FileName =bidDetails.Domicile + bidDetails.Position + "N" + ".TXT";
					var jsonData = ServiceUtils.JsonSerializer(bidDetails);
					StreamReader dr = ServiceUtils.GetRestData("DownloadHistoricalBidLineAll", jsonData);
					HistoricalFileInfo historicalFileInfo = WBidCollection.ConvertJSonStringToObject<HistoricalFileInfo>(dr.ReadToEnd());
					sWAFileInfo = new DownloadedFileInfo();
					sWAFileInfo.byteArray =Convert.FromBase64String(historicalFileInfo.DataString);
					sWAFileInfo.FileName = historicalFileInfo.Title;

					//  sWAFileInfo.IsError = (historicalFileInfo.Data == null) ? true : false;
					lstDownloadedFiles.Add(sWAFileInfo);
				}
				_downloadBidObject.SaveDownloadedBidFiles (lstDownloadedFiles, WBidHelper.GetAppDataPath ());
				string zipFileName = _downloadFileDetails.DownloadList.Where (x => x.Contains (".737")).FirstOrDefault ();
				DownloadedFileInfo zipFile = lstDownloadedFiles.FirstOrDefault (x => x.FileName == zipFileName);

				if (zipFile.IsError) {

					string errorMessage = string.Empty;
					if (zipFile.Message.Contains ("BIDINFO DATA NOT AVAILABLE")) {

						errorMessage += "The Requested History data doesnot exist on  the  server . \r\n";
						errorMessage += zipFile.Message.Replace ("ERROR: ", " ");

					} else {
						errorMessage += zipFile.Message.Replace ("ERROR: ", " ");

					}


					InvokeOnMainThread (() => {
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Warning;
						alert.MessageText = "WBidMax";
						alert.InformativeText = "Data Transfer Failed\r\n\r\n" + errorMessage;
						alert.AddButton ("OK");
						((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
							DismissCurrentView ();
						};
						alert.BeginSheet (this.Window);
						});
				} else {

					string path = WBidHelper.GetAppDataPath () + "/" + Path.GetFileNameWithoutExtension (zipFileName);

					if (!(File.Exists (path + "/" + "TRIPS") && File.Exists (path + "/" + "PS"))) {

						InvokeOnMainThread (() => {
							var alert = new NSAlert ();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.MessageText = "WBidMax";
							alert.InformativeText = "There is an error while downloading the data. Please check your internet connection and try again.";
							alert.AddButton ("OK");
							((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
								DismissCurrentView ();
								NSApplication.SharedApplication.StopModal ();
							};
							alert.BeginSheet (this.Window);
						});
						return;
					}

					//complted the download process
					WBidHelper.SetCurrentBidInformationfromZipFileName (zipFileName,true);

					//Write to currentBidDetailsfile for Error log
					FileOperations.WriteCurrentBidDetails (WBidHelper.GetAppDataPath () + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData ());

					//Download Wbid files
					DownloadWBidFiles ();

					ReadWbUpdateAndUpdateINIFile ();

					//  Set Cover letter file
					if (GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter) {
						DownloadedFileInfo coverLetter = lstDownloadedFiles.FirstOrDefault (x => x.FileName.Contains ("C.TXT") && !x.IsError);
						if (coverLetter != null) {

							GlobalSettings.IsCoverletterShowFileName = coverLetter.FileName;

						}
					}



					NSNotificationCenter.DefaultCenter.PostNotificationName ("getDataFilesSuccess", null);
					ParseData (zipFileName);
				}



			}
		}
		private void ShowNewsAndCoverLetter ()
		{ 
			try {
				if (GlobalSettings.IsCoverletterShowFileName != string.Empty) {
				
					string coverLetter = GlobalSettings.IsCoverletterShowFileName;
					InvokeOnMainThread (() => {
						var fileViewer = new FileWindowController ();
						fileViewer.Window.Title = "Cover Letter";
						fileViewer.LoadTXT (coverLetter);
						//fileViewer.ShowWindow(this);
						CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
						//fileViewer.Window.MakeKeyAndOrderFront (this);
					});
					GlobalSettings.IsCoverletterShowFileName = string.Empty;
				}
				if (GlobalSettings.IsNewsShow) {
					GlobalSettings.IsNewsShow = false;
					InvokeOnMainThread (() => {
						var fileViewer = new FileWindowController ();
						fileViewer.Window.Title = "Latest News";
						fileViewer.LoadPDF ("news.pdf");
						CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
						//fileViewer.Window.MakeKeyAndOrderFront (this);
					});
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				InvokeOnMainThread (() => {
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				} );
			}
		

		}

		private StringBuilder GetInvalidAccountMessage ()
		{
			var bodyContent = new StringBuilder ();
			bodyContent.Append ("<table width=\"700\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style='font-family:Arial;font-size:12'>");
			bodyContent.Append ("<tr><td align=\"left\" valign=\"top\" colspan=\"3\"> Hi Admin, </td></tr>");
			bodyContent.Append ("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style='height:15px'> </td></tr>");
			bodyContent.Append ("<tr><td align=\"left\" valign=\"top\" colspan=\"3\"> I am getting \"Invalid Account\" message while Downloading bid(authenticating)  </td></tr>");
			bodyContent.Append ("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style='height:15px'> </td></tr>");

			bodyContent.Append ("<tr><td style='Width:180px'><B>First Name</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.FirstName + " </td><td></td></tr>");
			bodyContent.Append ("<tr><td ><B>Last Name</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.LastName + " </td><td></td></tr>");
			bodyContent.Append ("<tr><td ><B>Email</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.Email + " </td><td></td></tr>");
			try {
				bodyContent.Append ("<tr><td ><B>Employee Number</B></td><td>" + Regex.Match (_downloadFileDetails.UserId, @"\d+").Value + " </td><td></td></tr>");
			} catch (Exception) {


			}

			bodyContent.Append ("<tr><td ><B>Domicile</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.Domicile + " </td><td></td></tr>");
			bodyContent.Append ("<tr><td ><B>Gender</B></td><td>" + (GlobalSettings.WbidUserContent.UserInformation.IsFemale ? "Female" : "Male") + " </td><td></td></tr>");
			bodyContent.Append ("<tr><td ><B>Position</B></td><td>" + GlobalSettings.WbidUserContent.UserInformation.Position + " </td><td></td></tr>");
			bodyContent.Append ("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style='height:15px'> </td></tr>");
			bodyContent.Append ("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=>" + GlobalSettings.WbidUserContent.UserInformation.FirstName + " " + GlobalSettings.WbidUserContent.UserInformation.LastName + " </td></tr>");

			bodyContent.Append ("</table>");
			return bodyContent;
		}

		/// <summary>
		/// Download SWA files
		/// </summary>
		/// <param name="downloadInfo"></param>
		/// <returns></returns>
		private List<DownloadedFileInfo> DownloadFiles (DownloadInfo downloadInfo)
		{
			try {
				// _serverUrl = SWAConstants.SWAUrl;
				List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo> ();

				var tasks = new Task[downloadInfo.DownloadList.Count];

				//Check the SWA authentication 
				//Authentication authentication = new Authentication();
				//downloadInfo.SessionCredentials = authentication.CheckCredential(downloadInfo.UserId, downloadInfo.Password);
				if(GlobalSettings.isHistorical) 
				{
					DownloadHistoryData(downloadInfo);
				}
				else if (_isCompanyServerData) {
					//int count = 0;

					//foreach (string filename in downloadInfo.DownloadList)
					//{
					//    tasks[count] = Task.Factory.StartNew(() => ChekFileLengthAndDownload(downloadInfo, filename));
					//    count++;

					//}
					//Task.WaitAll(tasks);
					//count = 0;
					//foreach (Task task in tasks)
					//{
					//    downloadedFileDetails.Add(((Task<DownloadedFileInfo>)tasks[count]).Result);
					//    count++;
					//}

					DownloadedFileInfo downloadedFileInfo;
					foreach (string filename in downloadInfo.DownloadList) {
						downloadedFileInfo = new DownloadedFileInfo ();

						downloadedFileInfo = ChekFileLengthAndDownload (downloadInfo, filename);

						downloadedFileDetails.Add (downloadedFileInfo);
					}
				}
				//Mock data download.
				else {
					downloadedFileDetails = DownloadMockDetails (downloadInfo);
				}

				return downloadedFileDetails;
				//return null;


			} catch (Exception ex) {
				return null;

			}
		}
		/// <summary>
		/// PURPOSE :Download History details
		/// </summary>
		/// <param name="downloadInfo"></param>
		/// <returns></returns>
		private void DownloadHistoryData(DownloadInfo downloadInfo)
		{
			List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo>();


			WBidDataDownloadAuthorizationService.Model.BidDetails bidDetails = new WBidDataDownloadAuthorizationService.Model.BidDetails ();
			bidDetails.Month = GlobalSettings.DownloadBidDetails.Month;
			bidDetails.Year = GlobalSettings.DownloadBidDetails.Year;
			bidDetails.Round = GlobalSettings.DownloadBidDetails.Round == "D" ? 1 : 2;
			bidDetails.Domicile = GlobalSettings.DownloadBidDetails.Domicile;
			bidDetails.Position = GlobalSettings.DownloadBidDetails.Postion;
			bidDetails.FileName = downloadInfo.DownloadList.FirstOrDefault(x => x.Length == 10 && x.Substring(7, 3) == "737");
			client.DownloadHistoricalDataCompleted+= HandleDownloadHistoricalDataCompleted;
			client.DownloadHistoricalDataAsync(bidDetails);


		}

		/// <summary>
		/// PURPOSE :Download Mock Award details
		/// </summary>
		/// <param name="downloadInfo"></param>
		/// <returns></returns>
		private List<DownloadedFileInfo> DownloadMockDetails (DownloadInfo downloadInfo)
		{
			List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo> ();
			WebClient wcClient = new WebClient ();

			foreach (string filename in downloadInfo.DownloadList) {

				downloadedFileDetails.Add (_downloadBidObject.DownloadMockFile (wcClient, filename,false));
			}
			wcClient.Dispose ();

			return downloadedFileDetails;
		}
		private void DownloadMockSeniorityListFile()
		{
			DownloadedFileInfo objSeniority = new DownloadedFileInfo();
			WebClient wcClient = new WebClient();
			string fileName = string.Empty;
			if (GlobalSettings.DownloadBidDetails.Round == "D")
			{
				//S seniority list
				fileName = GlobalSettings.DownloadBidDetails.Domicile + GlobalSettings.DownloadBidDetails.Postion + "S" + ".TXT";

			}
			else  // second round
			{
				if (GlobalSettings.DownloadBidDetails.Postion == "FA")
				{
					fileName = GlobalSettings.DownloadBidDetails.Domicile + GlobalSettings.DownloadBidDetails.Postion + "SR" + ".TXT";
				}
				else
				{
					// get pilot 2nd round cover letter and seniority list
					fileName = GlobalSettings.DownloadBidDetails.Domicile + GlobalSettings.DownloadBidDetails.Postion + "R" + ".TXT";
				}
			}
			objSeniority = _downloadBidObject.DownloadMockFile(wcClient, fileName, true);
			if (objSeniority.IsError == false)
			{
				//save seniority file

				//setting  the  file name, if the file type is a seniority list
				//if (objSeniority.FileName.Substring(5, 1) == "S")
				//{
				//	_seniorityListFilename = objSeniority.FileName;
				//}

				FileStream fStream = new FileStream(Path.Combine(WBidHelper.GetAppDataPath(), objSeniority.FileName), FileMode.Create);
				fStream.Write(objSeniority.byteArray, 0, objSeniority.byteArray.Length);
				fStream.Dispose();
			}
			else
			{
				//Xceed.Wpf.Toolkit.MessageBox.Show(downloadWindow, "Unable to get seniority file from the server. Please contact administrator", "Mock data seniority download", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
			}
		}
		private DownloadedFileInfo ChekFileLengthAndDownload (DownloadInfo downloadInfo, string filename)
		{
			DownloadedFileInfo downloadedFileDetails = new DownloadedFileInfo ();
			string packetType = string.Empty;
			//if length <10 or > 11 will add an error message  and continue to download next file. 
			if (filename.Length < 10 || filename.Length > 11) {
				downloadedFileDetails = new DownloadedFileInfo () { IsError = true, Message = "", FileName = filename };

			} else {
				//finding packet type
				packetType = (filename.Substring (7, 3) == "737") ? "ZIPPACKET" : "TXTPACKET";
				//Download the selected file and adding  downloaded information to downloadedFileDetails list.
				downloadedFileDetails = _downloadBidObject.DownloadBidFile (downloadInfo, filename.ToUpper (), packetType);
			}
			return downloadedFileDetails;
		}

		/// <summary>
		/// Download WBid Files
		/// </summary>
		/// <param name="downloadInfo"></param>
		/// <returns></returns>
		private bool DownloadWBidFiles ()
		{
			bool status = true;
			int previousnewsverion = 0;
			if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Updates != null) {
				previousnewsverion = GlobalSettings.WBidINIContent.Updates.News;
			}
			try {
				// 

				List<string> lstWBidFiles = new List<string> () { "WBUPDATE.DAT", "news.pdf", "trips.ttp" };
				//, "falistwb4.dat" 
				//  var tasks = new Task[4];

				//  int count = 0;

				//foreach (var bidFile in lstWBidFiles)
				//{
				//    tasks[count] = Task.Factory.StartNew(() => DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), bidFile));
				//    //DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), bidFile);
				//    count++;
				//}

				//Task.WaitAll(tasks);
				foreach (var bidFile in lstWBidFiles) {

					DownloadBid.DownloadWBidFile (WBidHelper.GetAppDataPath (), bidFile);
				}


				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M") {
					DownloadBid.DownloadWBidFile (WBidHelper.GetAppDataPath (), "falistwb4.dat");
				}

			} catch (Exception ex) {
				throw ex;
			}
			return status;
		}

		/// <summary>
		/// Read the WBPUpdate.Dat and Update the INI file.
		/// </summary>
		private void ReadWbUpdateAndUpdateINIFile ()
		{
			try {
				if (!File.Exists (WBidHelper.WBidUpdateFilePath))
					return;
				
				
				int previousnewsverion = GlobalSettings.WBidINIContent.Updates.News;
				WBidUpdate WBidUpdate = WBidHelper.ReadValuesfromWBUpdateFile (WBidHelper.WBidUpdateFilePath);
				
				
				if (WBidUpdate != null) {
					bool isUpdateFound = WBidCollection.UpdateINIFile (WBidUpdate);
					//Save the INI file
					if (isUpdateFound) {
						WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
					}
				
					if (GlobalSettings.WBidINIContent.Updates.News > previousnewsverion) {
						GlobalSettings.IsNewsShow = true;
				
						//InvokeOnMainThread(() =>
						//   {
						//       webPrint fileViewer = new webPrint();
						//       this.PresentViewController(fileViewer, true, () =>
						//       {
						//           fileViewer.loadFileFromUrl("news.rtf");
						//       });
						//   });
				
					}
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				InvokeOnMainThread (() => {
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				} );
			}

		}

		/// <summary>
		/// Parse the Downloaded Data.This will Parse trip file,line file etc.
		/// </summary>
		/// <param name="employeeNumber"></param>
		/// <param name="password"></param>
		/// <param name="zipFilename"></param>
		private void ParseData (string zipFilename)
		{



			try {
				//Prase Trip files
				trips = ParseTripFile (zipFilename);
				if (trips != null) {
				
					if (zipFilename.Substring (0, 1) == "A" && zipFilename.Substring (1, 1) == "B") {
						FASecondRoundParser fASecondRound = new FASecondRoundParser ();
						lines = fASecondRound.ParseFASecondRound (WBidHelper.GetAppDataPath () + "/" + zipFilename.Substring (0, 6).ToString () + "/PS", ref trips, GlobalSettings.FAReserveDayPay, zipFilename.Substring (2, 3));
					} else {
						lines = ParseLineFiles (zipFilename);
					}
					try {
						if (GlobalSettings.CurrentBidDetails.Round == "M") {
						

							CoverLetterParser coverletterparser = new CoverLetterParser ();
							string coverlettername = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "C" + ".TXT";
							CoverLetterData coverletterdata = new CoverLetterData ();
							if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") {
								coverletterdata = coverletterparser.ParseCoverLetteForPilots (WBidHelper.GetAppDataPath () + "/" + coverlettername, GlobalSettings.CurrentBidDetails.Domicile, GlobalSettings.CurrentBidDetails.Postion);
							} else if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
								coverletterdata = coverletterparser.ParseCoverLetteForFlightAttendants (WBidHelper.GetAppDataPath () + "/" + coverlettername, GlobalSettings.CurrentBidDetails.Domicile);
							}
							if (GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Year == coverletterdata.Year && GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Month == coverletterdata.Month) {
								if (coverletterdata.TotalLine != lines.Count) 
								{

									if(coverletterdata.TotalLine==0)
									{
										InvokeOnMainThread (() => {
											var alert = new NSAlert ();
											alert.AlertStyle = NSAlertStyle.Warning;
											alert.Window.Title = "WBidMax";
											alert.MessageText = "We are not able to parse the cover letter, make sure the number of lines in the bid data equals what is stated in the cover letter";
											alert.RunModal ();
										});
									}
									else
									{
									InvokeOnMainThread (() => {
										var alert = new NSAlert ();
										alert.AlertStyle = NSAlertStyle.Warning;
										alert.Window.Title = "WBidMax";
										alert.MessageText = "It appears the bid data is incorrect.  The cover letter says there will be " + coverletterdata.TotalLine + " lines, but the bid data only contains " + lines.Count + " lines.";
										alert.RunModal ();
									});
									}
									//Xceed.Wpf.Toolkit.MessageBox.Show(downloadWindow, "It appears the bid data is incorrect.  The cover letter says there will be " + coverletterdata.TotalLine + " lines, but the bid data only contains " + lines.Count + " lines.", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
								}
							}
						
						} else { 
							if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
								SeniorityListParser parser = new SeniorityListParser ();
								string senioriyListname = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "SR" + ".TXT";
								int totalFACount = parser.ParseSeniorityListForSecondRoundFAForTotalFACount (WBidHelper.GetAppDataPath () + "/" + senioriyListname);
								if (totalFACount != lines.Count) {
									InvokeOnMainThread (() => {
										var alert = new NSAlert ();
										alert.AlertStyle = NSAlertStyle.Warning;
										alert.Window.Title = "WBidMax";
										alert.MessageText = "The seniority list has " + totalFACount + " Flight Attendants, but the bid data contains " + lines.Count + " lines.  Please send up an email to support@wbidmax.com.";
										alert.RunModal ();
									});
									//Xceed.Wpf.Toolkit.MessageBox.Show(downloadWindow, "The seniority list has " + totalFACount + " Flight Attendants, but the bid data contains " + lines.Count + " lines.  Please send up an email to support@wbidmax.com.", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
								}
							}
						
						}
					} catch (Exception ex) {

					}

					trips = ScrapMissingTrip (lines, trips);
				
				
					// Parse trip.ttp file.
					List<CityPair> ListCityPair = TripTtpParser.ParseCity (WBidHelper.GetAppDataPath () + "/trips.ttp");
					GlobalSettings.TtpCityPairs = ListCityPair;
				
					// Additional processing needs to be done to FA trips before CalculateTripPropertyValues
					if (zipFilename.Substring (0, 1) == "A")
						CalculateTripProperties.PreProcessFaTrips (trips, ListCityPair);
				
					CalculateTripProperties.CalculateTripPropertyValues (trips, ListCityPair);
					//WBidHelper.SetCurrentBidInformationfromZipFileName(zipFilename);
				
					CalculateLineProperties.CalculateLinePropertyValues (trips, lines, GlobalSettings.CurrentBidDetails);
				
					//  SaveParsedFiles(trips, lines);
				
					//Send Notif
					NSNotificationCenter.DefaultCenter.PostNotificationName ("parseDataSuccess", null);
					if (!GlobalSettings.isHistorical) {
						ParseSeniorityList ();
				
						CheckAndPerformVacationCorrection ();
					}
				
					SaveParsedFiles (trips, lines);
				
				
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				InvokeOnMainThread (() => {
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				} );

			}

		}

		private Dictionary<string, Trip> ScrapMissingTrip (Dictionary<string, Line> lines, Dictionary<string, Trip> trips)
		{
			//bid round is second round
			//if (GlobalSettings.CurrentBidDetails.Round == "S") {
				//Finding if any missed trip exists
				List<string> allPair = lines.SelectMany (x => x.Value.Pairings).Distinct ().ToList ();
				var pairingwHasNoDetails = allPair.Where (x => !trips.Select (y => y.Key).ToList ().Any (z => (z == x.Substring (0, 4)) || (z == x && x.Substring (1, 1) == "P"))).ToList ();

				//Checking any missed trip  exist
				if (pairingwHasNoDetails.Count > 0) {

					
					try {
                    if (GlobalSettings.WBidINIContent.MiscellaneousTab.IsRetrieveMissingData)
                    {
                        List<string> temppairingwHasNoDetails = new List<string>();
                        bool isscrapRequired = true;
                        MonthlyBidDetails biddetails = new MonthlyBidDetails();
                        biddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                        biddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                        biddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                        biddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                        biddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                        var missedtrips = WBidHelper.GetMissingtripFromVPS(biddetails);
                        if (missedtrips.Count >= pairingwHasNoDetails.Count)
                        {
                            //trips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                            var temptrips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                            temppairingwHasNoDetails = allPair.Where(x => !temptrips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x))).ToList();
                            if (temppairingwHasNoDetails.Count == 0)
                            {
                                trips = trips.Concat(missedtrips).ToDictionary(pair => pair.Key, pair => pair.Value);
                                isscrapRequired = false;
                            }
                        }
                        if (isscrapRequired)
                        {
                            if ((GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(-1).Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(1).Month))
                            {

                                GlobalSettings.parsedDict = new Dictionary<string, WBid.WBidiPad.Model.Trip>();

                                scrap(_downloadFileDetails.UserId, _downloadFileDetails.Password, pairingwHasNoDetails, GlobalSettings.DownloadBidDetails.Month, GlobalSettings.DownloadBidDetails.Year, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay);

                                if (GlobalSettings.parsedDict == null || GlobalSettings.parsedDict.Count == 0)
                                {
                                    InvokeOnMainThread(() =>
                                    {
                                    //										alertVW = new UIAlertView("Missing Data", "Unable to get missing data.  Only partial data will be displayed for split pairings.", null, "OK", null);
                                    //										alertVW.Show();
                                    var alert = new NSAlert();
                                        alert.AlertStyle = NSAlertStyle.Warning;
                                        alert.MessageText = "Missing Data";
                                        alert.InformativeText = "Unable to get missing data.  Only partial data will be displayed for split pairings.";
                                        alert.AddButton("OK");
                                        alert.RunSheetModal(this.Window);

                                        IsMissingTripFailed = true;
                                        string bidFileName = string.Empty;
                                        bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                        BidLineParser bidLineParser = new BidLineParser();
                                        var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                        trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                                    });

                                }
                                else
                                {
                                    //IsMissingTripFailed = true;
                                    trips = trips.Concat(GlobalSettings.parsedDict).ToDictionary(pair => pair.Key, pair => pair.Value);
                                }
                                //							string bidFileName = string.Empty;
                                //							//IsMissingTripFailed = true;
                                //							bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                //							BidLineParser bidLineParser = new BidLineParser();
                                //							var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;
                                //
                                //							trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                            }
                            else
                            {

                                string bidFileName = string.Empty;
                                //IsMissingTripFailed = true;
                                bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                BidLineParser bidLineParser = new BidLineParser();
                                var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                            }
                        }
                    }
                    else
                    {
                        string bidFileName = string.Empty;
                        //IsMissingTripFailed = true;
                        bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                        BidLineParser bidLineParser = new BidLineParser();
                        var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                        trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                    }
					} catch (Exception ex) {
						InvokeOnMainThread (() => {
							// Parse Missing trip details from  Bidline File
							GlobalSettings.IsScrapStart = false;
//								alertVW = new UIAlertView("Missing Data", "Unable to get missing data.  Only partial data will be displayed for split pairings.", null, "OK", null);
//								alertVW.Show();
							var alert = new NSAlert ();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.MessageText = "Missing Data";
							alert.InformativeText = "Unable to get missing data.  Only partial data will be displayed for split pairings.";
							alert.AddButton ("OK");
							alert.RunSheetModal (this.Window);
//								((NSButton)alert.Buttons[0]).Activated += (senderr, ee) => {
//									DismissCurrentView();
//								};

							//IsMissingTripFailed = true;
							string bidFileName = string.Empty;
							bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
							BidLineParser bidLineParser = new BidLineParser ();
							var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault (x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

							trips = trips.Concat (bidLineParser.ParseBidlineFile (WBidHelper.GetAppDataPath () + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where (x => pairingwHasNoDetails.Contains (x.Key))).ToDictionary (pair => pair.Key, pair => pair.Value);
						});

					}


					//}
					//// Parse Missing trip details from  Bidline File
					//else
					//{

					//    string bidFileName = string.Empty;
					//    bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
					//    BidLineParser bidLineParser = new BidLineParser();
					//    var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;
					//    trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "\\" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);

					//}
				}
			//}

			return trips;
		}

		private void scrap (string userName, string password, List<string> pairingwHasNoDetails, int month, int year, int show1stDay, int showAfter1stDay)
		{
			Console.WriteLine ("Scrapping started");
			GlobalSettings.IsScrapStart = true;
			InvisibleWindowController inv = null;
			InvokeOnMainThread (() => {
				//var scrap = new ScrapViewController (userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay);
				//scrap.View.Hidden = true;
				//this.Window.ContentView.AddSubview (scrap.View);

				inv = new InvisibleWindowController ();
				this.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Above);
				if(userName.ToLower()=="x21221")
				{
					var scrap = new ContractorEmpScrapController (userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay,GlobalSettings.CurrentBidDetails.Postion);
					scrap.View.Hidden = true;
					inv.Window.ContentView.AddSubview (scrap.View);
				}
				else
				{
					var scrap = new ScrapViewController (userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion);
					scrap.View.Hidden = true;
					inv.Window.ContentView.AddSubview (scrap.View);
				}



//					webView scrapper = new webView(userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay);
//					this.AddChildViewController(scrapper);
//					scrapper.View.Hidden = true;
//					this.View.AddSubview(scrapper.View);
			});



			while (GlobalSettings.IsScrapStart) {
			}
			;
			InvokeOnMainThread (() => {
				if (inv != null) {
					inv.Window.Close ();
					inv.Window.OrderOut (this);
				}
			});
			Console.WriteLine ("Scrapping done");
		}

        private void SaveParsedFiles(Dictionary<string, Trip> trips, Dictionary<string, Line> lines)
        {

            string fileToSave = string.Empty;

            fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


            TripInfo tripInfo = new TripInfo()
            {
                TripVersion = GlobalSettings.TripVersion,
                Trips = trips

            };
            // save the trip file to app data folder
            //Task triptask = Task.Run(() =>
            //{


            // try
            // {
            var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP");
            ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP", tripInfo, stream);
            stream.Dispose();
            stream.Close();
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            // });
            GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));

            // Additional processing needs to be done to FA 2nd Round lines before CalculateLinePropertyValues
            //if (zipFilename.Substring(0, 1) == "A" && GlobalSettings.CurrentBidDetails.Round == "S")
            //{ 
            //   // CalculateLineProperties.PreProcessFaLines(trips, lines);
            //}

            //CalculateLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);
            //LineProperties properties = new LineProperties()
            //{
            //    CurrentBidDetail = GlobalSettings.CurrentBidDetails,
            //    EOMStartdate = DateTime.MinValue,
            //    IsDownloadProcess = true,
            //    IsDrop = false,
            //    IsEomChecked = false,
            //    IsOverlapCorrection = false,
            //    IsVacationCorrection = false,
            //    LinesData = lines,
            //    TripData = trips
            //};


            //CalculateLineProperties.CalculateLinePropertyValues(properties);

            if (GlobalSettings.IsVacationCorrection && GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0)
            {//set  vacation details  to line object. 

                CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
                calVacationdetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);

                //set the Vacpay,Vdrop,Vofont and VoBack columns in the line summary view 
                ManageVacationColumns managevacationcolumns = new ManageVacationColumns();
                managevacationcolumns.SetVacationColumns();
            }

            LineInfo lineInfo = new LineInfo()
            {
                LineVersion = GlobalSettings.LineVersion,
                Lines = lines

            };



            GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));


            //  int[] arr = GlobalSettings.Lines[0].DaysOfWeekWork;
            //save the line file to app data folder
            // Task linetask = Task.Run(() =>
            // {

            try
            {
                var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
                ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
                linestream.Dispose();
                linestream.Close();
            }
            catch (Exception ex)
            {

                CommonClass.AppDelegate.ErrorLog(ex);
                InvokeOnMainThread(() =>
                {
                    CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                });
            }


            foreach (Line line in GlobalSettings.Lines)
            {
                line.ConstraintPoints = new ConstraintPoints();
                line.WeightPoints = new WeightPoints();
            }

            if (GlobalSettings.IsOverlapCorrection && GlobalSettings.CurrentBidDetails.Round == "M")
            {
                OverlapData overlapData = new OverlapData();
                overlapData.LastLegArrivalTime = GlobalSettings.LastLegArrivalTime.ToString();
                //List<OverlapDay> overlapdays = new List<OverlapDay>();
                //foreach (Day day in GlobalSettings.LeadOutDays)
                //{
                //    overlapdays.Add(new OverlapDay
                //    {
                //        ArrivalCity = day.ArrivalCity,
                //        ArrivalTime = day.ArrivalTime,
                //        Date = day.Date,
                //        DepartutreCity = day.DepartutreCity,
                //        DepartutreTime = day.DepartutreTime,
                //        FlightTime = day.FlightTime,
                //        FlightTimeHour = day.FlightTimeHour,
                //        OffDuty = day.OffDuty
                //    });

                //}
                overlapData.LeadOutDays = GlobalSettings.LeadOutDays;
                var overlapfile = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL");
                ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL", overlapData, overlapfile);
                overlapfile.Dispose();
                overlapfile.Close();
                //WBidHelper.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL", overlapData);
            }

            //Read the intial state file value from DWC file and create state file
            if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS"))
            {
                try
                {


                    WBidIntialState wbidintialState = null;
                    try
                    {
                        wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    }
                    catch (Exception ex)
                    {
                        wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                        XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                        WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0");
                    }

                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
                    if (GlobalSettings.isHistorical)
                    {
                        GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    }
                    else
                        GlobalSettings.WBidStateCollection.DataSource = (_isCompanyServerData) ? "Original" : "Mock";
                    //WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                //Read the state file object and store it to global settings.
                try
                {
                    GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                }
                catch (Exception exx)
                {
                    WBidIntialState wbidintialState = null;
                    try
                    {
                        wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    }
                    catch (Exception exxx)
                    {
                        wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                        XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                        WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0");

                    }

                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", 400, 1, wbidintialState);
                    WBidHelper.SaveStateFile(fileToSave);
                    WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "wbsRecreate", "0", "0");


                }
                //XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                if (GlobalSettings.isHistorical)
                {
                    GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                }
                else if (GlobalSettings.WBidStateCollection.DataSource == "Original" && _isCompanyServerData == false)
                {
                    GlobalSettings.WBidStateCollection.DataSource = "Mock";

                    //WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                }
                else if (GlobalSettings.WBidStateCollection.DataSource == "Mock" && _isCompanyServerData == true)
                {
                    GlobalSettings.WBidStateCollection.DataSource = "Original";
                    // WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
                }



            }
            GlobalSettings.WBidStateCollection.SeniorityListItem = _seniorityListItem;
            //save the vacation to state file
            GlobalSettings.WBidStateCollection.Vacation = new List<Vacation>();
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null && GlobalSettings.IsVacationCorrection)
            {
                var vacation = GlobalSettings.SeniorityListMember.Absences.Where(x => x.AbsenceType == "VA").Select(y => new Vacation
                {
                    StartDate = y.StartAbsenceDate.ToShortDateString(),
                    EndDate = y.EndAbsenceDate.ToShortDateString()
                });

                GlobalSettings.WBidStateCollection.Vacation.AddRange(vacation.ToList());

                wBIdStateContent.IsVacationOverlapOverlapCorrection = true;
            }
            else
                wBIdStateContent.IsVacationOverlapOverlapCorrection = false;
            wBIdStateContent.IsOverlapCorrection = GlobalSettings.IsOverlapCorrection;


            GlobalSettings.WBidStateCollection.FVVacation = new List<Absense>();
            GlobalSettings.WBidStateCollection.FVVacation = GlobalSettings.FVVacation;


            GlobalSettings.WBidStateCollection.CompanyVA = GlobalSettings.CompanyVA;

            if (IsMissingTripFailed)
            {
                wBIdStateContent.IsMissingTripFailed = true;
            }
            else
            {
                wBIdStateContent.IsMissingTripFailed = false;
            }
            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                //After the Seniority LIst is parsed, if the USER is NOT in the EBG, we should filter/ constrain for ETOPS.However, some users may want to look at the ETOPS bid data, so in the Alert we give them the chance to "Turn OFF"
                if (ChecktheUserIsInEBGGroup() == false)
                {
                    wBIdStateContent.Constraints.ETOPS = true;
                    wBIdStateContent.Constraints.ReserveETOPS = true;
                }
            }
            WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

            //if (GlobalSettings.CurrentBidDetails.Round == "M" || (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA"))
            //{
            //    string sFileName = string.Empty;
            //    if (GlobalSettings.CurrentBidDetails.Round == "M")
            //    {
            //        //First round Pilot and FA  SeniorityList
            //        sFileName = WBidHelper.GetAppDataPath() + "\\" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S" + ".SL";
            //    }
            //    else
            //    {   //Second round Pilot SeniorityList
            //        sFileName = WBidHelper.GetAppDataPath() + "\\" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R" + ".SL";
            //    }

            //    //List<SeniorityListMember> seniorityListMembers = null;
            //    //seniorityListMembers = (List<SeniorityListMember>)WBidHelper.DeSerializeObject(sFileName);
            //    //if (seniorityListMembers != null)
            //    //{

            //    //    if (GlobalSettings.IsDifferentUser)
            //    //    {
            //    //        GlobalSettings.SeniorityListMember = seniorityListMembers.FirstOrDefault(x => x.EmpNum == (GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0')));
            //    //    }
            //    //    else
            //    //    {
            //    //        //GlobalSettings.SeniorityListMember = seniorityListMembers.FirstOrDefault(x => x.EmpNum == downloadInfo.UserId.Substring(1, downloadInfo.UserId.Length - 1).PadLeft(6, '0'));
            //    //    }
            //    //}
            //}


            WBidHelper.GenerateDynamicOverNightCitiesList();
            //  GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
            GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList();
            var linePairing = GlobalSettings.Lines.SelectMany(y => y.Pairings);

            // var sabu = GlobalSettings.Trip.Where(x => linePairing.Contains(x.TripNum)).SelectMany(z => z.DutyPeriods.SelectMany(r => r.Flights.Select(t => new { arrival = t.ArrSta, depart = t.DepSta })));
            // int a = 0;
            //GlobalSettings.Trip.Where(x => linePairing.Contains(x.TripNum)).SelectMany(a => a.DutyPeriods).SelectMany(b => b.Flights).SelectMany(c => c.ArrSta && c.DepSta);

            //temporary code..
            // var WBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBIdStateContent.CxWtState.AMPMMIX == null)
                wBIdStateContent.CxWtState.AMPMMIX = new AMPMConstriants();
            if (wBIdStateContent.CxWtState.FaPosition == null)
                wBIdStateContent.CxWtState.FaPosition = new PostionConstraint();
            if (wBIdStateContent.CxWtState.TripLength == null)
                wBIdStateContent.CxWtState.TripLength = new TripLengthConstraints();
            if (wBIdStateContent.CxWtState.DaysOfWeek == null)
                wBIdStateContent.CxWtState.DaysOfWeek = new DaysOfWeekConstraints();
            if (wBIdStateContent.Constraints.DaysOfMonth == null)
                wBIdStateContent.Constraints.DaysOfMonth = new DaysOfMonthCx() { };


            if (wBIdStateContent.Weights.NormalizeDaysOff == null)
            {
                wBIdStateContent.Weights.NormalizeDaysOff = new Wt2Parameter() { Type = 1, Weight = 0 };

            }
            if (wBIdStateContent.CxWtState.NormalizeDays == null)
            {
                wBIdStateContent.CxWtState.NormalizeDays = new StateStatus() { Cx = false, Wt = false };

            }
			if (wBIdStateContent.CxWtState.ETOPS == null)
			{
				wBIdStateContent.CxWtState.ETOPS = new StateStatus() { Cx = false, Wt = false };

			}
			if (wBIdStateContent.CxWtState.ETOPSRes == null)
			{
				wBIdStateContent.CxWtState.ETOPSRes = new StateStatus() { Cx = false, Wt = false };

			}
			if (wBIdStateContent.Weights.ETOPS == null)
			{
				wBIdStateContent.Weights.ETOPS = new Wt1Parameters
				{
					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				};
			}
			if (wBIdStateContent.Weights.ETOPSRes == null)
			{
				wBIdStateContent.Weights.ETOPSRes = new Wt1Parameters
				{
					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				};
			}
			StateManagement statemanagement = new StateManagement();
            statemanagement.ReloadDataFromStateFile();



            if (IsMissingTripFailed)
            {
                IsMissingTripFailed = false;
                InvokeOnMainThread(() =>
                {
                    var alert = new NSAlert();
                    alert.AlertStyle = NSAlertStyle.Warning;
                    alert.MessageText = "WBidMax";
                    alert.InformativeText = "Vacation Corrections are not available at this time – contact admin@wbidmax.com";
                    alert.AddButton("OK");
                    ((NSButton)alert.Buttons[0]).Activated += (sender, e) =>
                    {
                        //DismissCurrentView();
                        NSApplication.SharedApplication.StopModal();
                        NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);
                    };
                    alert.RunSheetModal(this.Window);

                    //						if (alertVW != null)
                    //						{
                    //							alertVW.DismissWithClickedButtonIndex(0, false);
                    //							alertVW = null;
                    //						}
                    //
                    //						alertVW = new UIAlertView("WBidMax", "Vacation Corrections are not available at this time – contact admin@wbidmax.com", null, "OK", null);
                    //						alertVW.Show();
                    //						alertVW.Clicked += alertVW_Clicked;
                });


            }
            else
            {
                NSNotificationCenter.DefaultCenter.PostNotificationName("saveDataSuccess", null);

            }


        }
        private bool ChecktheUserIsInEBGGroup()
        {
			return (GlobalSettings.WBidStateCollection.SeniorityListItem != null && GlobalSettings.WBidStateCollection.SeniorityListItem.EBgType == "Y") ? true : false;

			//if (GlobalSettings.SeniorityListMember == null)// tihs user is not in the seniority list
   //             return false;
   //         return GlobalSettings.SeniorityListMember.EBG == "Y" ? true : false;
        }
        private void CheckAndPerformVacationCorrection()
        {

            GlobalSettings.IsVacationCorrection = false;

            //			seniorityListMembers = new List<SeniorityListMember>();
            //			SeniorityListMember slM = new SeniorityListMember();
            //			slM.EmpNum = "022028";
            //			slM.Absences = new List<Absense>();
            //			Absense ab = new Absense() { AbsenceType = "VA", StartAbsenceDate = new DateTime(2014, 12, 07), EndAbsenceDate = new DateTime(2014, 12, 13) };
            //			slM.Absences.Add(ab);
            //			seniorityListMembers.Add(slM);

            if (seniorityListMembers.Count > 0)
            {

                if (GlobalSettings.IsDifferentUser)
                {
                    if (GlobalSettings.ModifiedEmployeeNumber.Trim() == string.Empty)
                    {

                        GlobalSettings.SeniorityListMember = seniorityListMembers.FirstOrDefault(x => x.EmpNum == _downloadFileDetails.UserId.Substring(1, _downloadFileDetails.UserId.Length - 1).PadLeft(6, '0'));
                    }
                    else
                    {
                        GlobalSettings.SeniorityListMember = seniorityListMembers.FirstOrDefault(x => x.EmpNum == (GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0')));
                    }
                }
                else
                {
                    GlobalSettings.SeniorityListMember = seniorityListMembers.FirstOrDefault(x => x.EmpNum == _downloadFileDetails.UserId.Substring(1, _downloadFileDetails.UserId.Length - 1).PadLeft(6, '0'));
                }


                GlobalSettings.OrderedVacationDays = null;

                if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null)
                {

                    //var CFVabsence1 = new List<Absense>() { new Absense { AbsenceType = "CFV", StartAbsenceDate = new DateTime(2020, 12, 2) },
                    //               new Absense { AbsenceType = "CFV", StartAbsenceDate = new DateTime(2020, 12, 28) }
                    //               };
                    //GlobalSettings.SeniorityListMember.Absences.AddRange(CFVabsence1);

                    var FAvacationabsence = GlobalSettings.SeniorityListMember.Absences.Where(x => x.AbsenceType == "FV" || x.AbsenceType=="CFV");
                    GlobalSettings.FVVacation = new List<Absense>();
                    FAvacationabsence.ToList().ForEach(x => GlobalSettings.FVVacation.Add(new Absense { StartAbsenceDate = x.StartAbsenceDate, EndAbsenceDate = x.EndAbsenceDate,AbsenceType=x.AbsenceType }));

                    if (GlobalSettings.SeniorityListMember.Absences.Any(x => x.StartAbsenceDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && x.EndAbsenceDate >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate && x.AbsenceType == "VA"))
                    {
                        GlobalSettings.IsVacationCorrection = (GlobalSettings.CurrentBidDetails.Round == "M" || (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA"));
                        GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();

                        GlobalSettings.TempOrderedVacationDays = GlobalSettings.OrderedVacationDays;
                    }
                    GlobalSettings.IsFVVacation = (GlobalSettings.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

                }



            }

            if (IsMissingTripFailed)
                GlobalSettings.IsVacationCorrection = false;

            if (GlobalSettings.IsVacationCorrection)
            {


                if (GlobalSettings.CurrentBidDetails.Postion != "FA" &&
                    GlobalSettings.SeniorityListMember.Absences.Any(x => x.AbsenceType == "VA" && x.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate))
                {
                    string dynamicdate = string.Empty;
                    var leadinvacation = GlobalSettings.SeniorityListMember.Absences.FirstOrDefault(x => x.AbsenceType == "VA" && x.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate);
                    if (leadinvacation != null)
                    {
                        GlobalSettings.CompanyVA = (leadinvacation.EndAbsenceDate.Date.Day * GlobalSettings.DailyVacPay).ToString();

                    }
                    //need to implement
                    //					_waitCompanyVADialog = true;
                    //
                    //					ShowVacationOverlapView ();
                    //
                    //					while (_waitCompanyVADialog) {
                    //
                    //					}

                    if (GlobalSettings.IsVacationCorrection)
                    {

                        PerformVacation();
                    }

                }
                else
                {
                    PerformVacation();

                }

            }
            else
            {

                if (Reachability.CheckVPSAvailable())
                {
                    string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
                    string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
                    string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";

                    FlightPlan flightPlan = null;
                    WebClient wcClient = new WebClient();
                    //Downloading networkdat file
                    wcClient.DownloadFile(serverPath, zipLocalFile);

                    //this.setButtonState (this.btnVacationData, true);
                    Console.WriteLine("VacationDataDownloadSuccess");

                    string target = Path.Combine(WBidHelper.GetAppDataPath(), WBidHelper.GetAppDataPath() + "/");

                    if (File.Exists(networkDataPath))
                    {
                        File.Delete(networkDataPath);
                    }

                    if (File.Exists(zipLocalFile))
                    {
                        ZipFile.ExtractToDirectory(zipLocalFile, target);
                    }
                }

            }
        }

		void ShowVacationOverlapView ()
		{
			InvokeOnMainThread (() => {
				if(alertVW!=null)
				{
					NSApplication.SharedApplication.EndSheet(alertVW.Window);
					alertVW.Window.OrderOut(this);
				}
				string dynamicdate = string.Empty;
				var leadinvacation = GlobalSettings.SeniorityListMember.Absences.FirstOrDefault (x => x.AbsenceType == "VA" && x.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate);
				if (leadinvacation != null) {
					if (leadinvacation.EndAbsenceDate == GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
						dynamicdate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.ToString ("MMM") + " - " + GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Day.ToString ();
					else
						dynamicdate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Month.ToString () + "/" + GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Day + " - " + leadinvacation.EndAbsenceDate.Month.ToString () + "/" + leadinvacation.EndAbsenceDate.Day.ToString ();
				}
				string Message1 = "You have a vacation that overlaps the begining of the month.  WBidMax needs to know how much VA the company awarded for  " + dynamicdate;
				string Message2 = "Log into CWA and go to your crewboard.  Hover over the green bar for  " + dynamicdate + "  and copy the VA credit  for those vacation days.";

				var panel = new NSPanel ();
				panel.SetContentSize (new CGSize (400, 250));
				panel.ContentView = vwVacOverlap;
				NSApplication.SharedApplication.BeginSheet (panel, this.Window);

				lblMessage1.StringValue = Message1;
				lblMessage2.StringValue = Message2;

				txtVANumber.Changed += (object sender, EventArgs e) => {
				

				//txtVANumber.StringValue = txtVANumber.StringValue;
				};
				decimal vavalue;
				btnDone.Activated += (object sender, EventArgs e) => {
					txtVANumber.ResignFirstResponder ();
					var value = leadinvacation.EndAbsenceDate.Date.Day * GlobalSettings.DailyVacPay;
					if (!string.IsNullOrEmpty (txtVANumber.StringValue) && decimal.TryParse(txtVANumber.StringValue,out vavalue)) {
						if (decimal.Parse (txtVANumber.StringValue) >= 0 && decimal.Parse (txtVANumber.StringValue) <= value) {
							GlobalSettings.CompanyVA = txtVANumber.StringValue;
							_waitCompanyVADialog = false;
							InvokeOnMainThread (() => {
								NSApplication.SharedApplication.EndSheet (panel);
								panel.OrderOut (this);
							});
						}
						else
						{	InvokeOnMainThread (() => {
							var alert = new NSAlert ();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.Window.Title = "WBidMax";
							alert.MessageText = "Please enter a value below or equal to "+value;
							alert.RunModal ();
						});
						}
					}
				};

				btnVacLater.Activated += (object sender, EventArgs e) => {
					txtVANumber.ResignFirstResponder ();
					GlobalSettings.IsVacationCorrection = false;
					_waitCompanyVADialog = false;
					InvokeOnMainThread (() => {
						NSApplication.SharedApplication.EndSheet (panel);
						panel.OrderOut (this);

						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Warning;
						alert.MessageText = "WBidMax";
						alert.InformativeText = "You can do vacation corrections later, when you have retrieved the company awared VA, by simply re-downloading the bid data.";
						alert.RunSheetModal (this.Window);
					});

				};
			});
		}

		/// <summary>
		/// Perform vacation correction
		/// </summary>
		private void PerformVacation ()
		{
			try {
				VacationCorrectionParams vacationParams = new VacationCorrectionParams ();

				if (GlobalSettings.CurrentBidDetails.Postion != "FA") {

					string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
					string zipLocalFile = Path.Combine (WBidHelper.GetAppDataPath (), "FlightData.zip");
					string networkDataPath = WBidHelper.GetAppDataPath () + "/" + "FlightData.NDA";

					FlightPlan flightPlan = null;
					WebClient wcClient = new WebClient ();
					//Downloading networkdat file
					wcClient.DownloadFile (serverPath, zipLocalFile);
					InvokeOnMainThread (() => {
						ckDownloadSteps.SelectCellWithTag (5);
					});
					//Extracting the zip file
					//var zip = new ZipArchive();
					//zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");

//					Unzip(zipLocalFile,WBidHelper.GetAppDataPath());

					//ZipStorer.

					// Open an existing zip file for reading
					ZipStorer zip = ZipStorer.Open (zipLocalFile, FileAccess.Read);

					// Read the central directory collection
					List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();

					// Look for the desired file
					foreach (ZipStorer.ZipFileEntry entry in dir) {
						zip.ExtractFile (entry, WBidHelper.GetAppDataPath () + "/" + entry);
					}
					zip.Close ();


					//Deserializing data to FlightPlan object
					FlightPlan fp = new FlightPlan ();
					using (FileStream networkDatatream = File.OpenRead (networkDataPath)) {

						FlightPlan objineinfo = new FlightPlan ();
						flightPlan = ProtoSerailizer.DeSerializeObject (networkDataPath, fp, networkDatatream);

					}

					if (File.Exists (zipLocalFile)) {
						File.Delete (zipLocalFile);
					}
					if (File.Exists (networkDataPath)) {
						File.Delete (networkDataPath);
					}




					vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join (flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
						(fr, f) =>
						new FlightRouteDetails {
							Flight = f.FlightId,
							FlightDate = fr.FlightDate,
							Orig = f.Orig,
							Dest = f.Dest,
							Cdep = f.Cdep,
							Carr = f.Carr,
							Ldep = f.Ldep,
							Larr = f.Larr,
							RouteNum = fr.RouteNum,

						}).ToList ();

				}


				vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
				vacationParams.Trips = trips;
				vacationParams.Lines = lines;
				//  VacationData = new Dictionary<string, TripMultiVacData>();


				//Performing vacation correction algoritham
				VacationCorrectionBL vacationBL = new VacationCorrectionBL ();

				if (GlobalSettings.CurrentBidDetails.Postion != "FA") {
					GlobalSettings.VacationData = vacationBL.PerformVacationCorrection (vacationParams);
				} else {
					GlobalSettings.VacationData = vacationBL.PerformFAVacationCorrection (vacationParams);

				}
				// GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);

				if (GlobalSettings.VacationData != null) {

					string fileToSave = string.Empty;
					fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();


					// save the VAC file to app data folder

					var stream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC");
					ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
					stream.Dispose ();
					stream.Close ();


					GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
					GlobalSettings.MenuBarButtonStatus.IsVacationDrop = true;

				} else {
					GlobalSettings.IsVacationCorrection = false;
				}



			} catch (Exception ex) {
				GlobalSettings.IsVacationCorrection = false;
				throw ex;
			}
		}

		//		public static void Unzip(string zipPath, string baseFolder)
		//		{
		//			using (FileStream fileStream = new FileStream(zipPath, FileMode.Open))
		//			{
		//				UnzipFilesFromStream(fileStream, baseFolder);
		//			}
		//		}
		//		private static void UnzipFilesFromStream(Stream source, string baseFolder)
		//		{
		//			if (!Directory.Exists(baseFolder))
		//			{
		//				Directory.CreateDirectory(baseFolder);
		//			}
		//
		//			using (Package package = Package.Open(source, FileMode.Open))
		//			{
		//				foreach (PackagePart zipPart in package.GetParts())
		//				{
		//					// fix for white spaces in file names (by ErrCode)
		//					string path = Path.Combine(baseFolder,
		//						Uri.UnescapeDataString(zipPart.Uri.ToString()).Substring(1));
		//
		//					using (Stream zipStream = zipPart.GetStream())
		//					{
		//						using (FileStream fileStream = new FileStream(path, FileMode.Create))
		//						{
		//							zipStream.CopyTo(fileStream);
		//						}
		//					}
		//				}
		//			}
		//		}
		/// <summary>
		/// Parse Seniority List
		/// </summary>
		private void ParseSeniorityList ()
		{
            GlobalSettings.IsFVVacation = false;
			SeniorityListParser senParser = new SeniorityListParser ();
			string seniorityFileName = string.Empty;
			// List<SeniorityListMember> seniorityListMembers = new List<SeniorityListMember>();
			seniorityListMembers = new List<SeniorityListMember> ();

			var modifiedSeniorityListParser = new ModifiedSeniorityListParser();

            int round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
			//Download wbid seniority file
			if ((GlobalSettings.IsNeedToDownloadSeniority || GlobalSettings.IsNeedToDownloadSeniorityUser))
			{
				DownloadMockSeniorityListFile();
				//string sFile;
				//if (GlobalSettings.CurrentBidDetails.Round == "M")
				//	sFile = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S";
				//else
				//	sFile = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R";
				//DownloadBid.DownloadWBidSeniorityFile(WBidHelper.GetAppDataPath(), (sFile + ".TXT"));

			}

			try {
                SenListFormat senlistformat = null;
                try
                {

                    //string url = GlobalSettings.CurrentBidDetails.Round == "M" ? "GetSeniorityListFormat/1" : "GetSeniorityListFormat/2";
                    //string url = (GlobalSettings.CurrentBidDetails.Round == "M") ? "GetSeniorityListFormatFromDB/1/" + GlobalSettings.CurrentBidDetails.Postion : "GetSeniorityListFormatFromDB/2" + GlobalSettings.CurrentBidDetails.Postion;
                    string url = "GetAllSeniorityListFormatFromDB";
                    StreamReader dr = ServiceUtils.GetRestData(url);
                    List<SenListFormat> allsenlistformat = WBidCollection.ConvertJSonStringToObject<List<SenListFormat>>(dr.ReadToEnd());
                    senlistformat = allsenlistformat.FirstOrDefault(x => x.Round == round && x.Position == GlobalSettings.CurrentBidDetails.Postion);
                    GlobalSettings.WBidINIContent.SenioritylistFormat = allsenlistformat;
                    WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                }
                catch (Exception ex)
                {
                    senlistformat = GlobalSettings.WBidINIContent.SenioritylistFormat.FirstOrDefault(x => x.Round == round && x.Position == GlobalSettings.CurrentBidDetails.Postion);
                    //GlobalSettings.IsVacationCorrection = false;

                    //InvokeOnMainThread(() =>
                    //{
                    //    alertVW = new NSAlert();
                    //    alertVW.AlertStyle = NSAlertStyle.Warning;
                    //    alertVW.MessageText = "WBidMax";
                    //    alertVW.InformativeText = "We are not able to read the seniority list, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view.\"";
                    //    alertVW.AddButton("OK");
                    //    alertVW.BeginSheet(this.Window);

                    //});
                }
				if (GlobalSettings.CurrentBidDetails.Round == "M") 
                {
                    
					seniorityFileName = WBidHelper.GetAppDataPath () + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S";
					if (GlobalSettings.CurrentBidDetails.Postion == "FA") {

						seniorityListMembers = senParser.ParseSeniorityListForFirstRoundFA (seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);

					} 
					else 
					{
						try
						{
                            if (senlistformat != null)
                            {
                                ModifiedSeniorityListParser newsenlist = new ModifiedSeniorityListParser();
                                seniorityListMembers = newsenlist.ParseSeniorityListForPilot(seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, GlobalSettings.CurrentBidDetails.Round, senlistformat);
                            }
                            else
                            {
                                GlobalSettings.IsVacationCorrection = false;

                                InvokeOnMainThread(() =>
                                {
                                    alertVW = new NSAlert();
                                    alertVW.AlertStyle = NSAlertStyle.Warning;
                                    alertVW.MessageText = "WBidMax";
                                    alertVW.InformativeText = "We are not able to read the seniority list, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view.\"";
                                    alertVW.AddButton("OK");
                                    alertVW.BeginSheet(this.Window);

                                });
                            }

						}
						catch(Exception ex)
						{
							//seniorityListMembers = senParser.ParseSeniorityListForFirstRoundPilot (seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);
                            GlobalSettings.IsVacationCorrection = false;

                            InvokeOnMainThread(() =>
                            {
                                alertVW = new NSAlert();
                                alertVW.AlertStyle = NSAlertStyle.Warning;
                                alertVW.MessageText = "WBidMax";
                                alertVW.InformativeText = "We are not able to read the seniority list, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view.\"";
                                alertVW.AddButton("OK");
                                alertVW.BeginSheet(this.Window);

                            });
						}
					}

				} else if (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion != "FA") 
				{
					seniorityFileName = WBidHelper.GetAppDataPath () + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R";
					try
					{
                        if (senlistformat != null)
                        {
                            ModifiedSeniorityListParser newsenlist = new ModifiedSeniorityListParser();
                            seniorityListMembers = newsenlist.ParseSeniorityListForPilot(seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, GlobalSettings.CurrentBidDetails.Round, senlistformat);
                        }
                        else
                        {
                            GlobalSettings.IsVacationCorrection = false;

                            InvokeOnMainThread(() =>
                            {
                                alertVW = new NSAlert();
                                alertVW.AlertStyle = NSAlertStyle.Warning;
                                alertVW.MessageText = "WBidMax";
                                alertVW.InformativeText = "We are not able to read the seniority list, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view.\"";
                                alertVW.AddButton("OK");
                                alertVW.BeginSheet(this.Window);

                            });
                        }

					}
					catch(Exception ex)
					{
						//seniorityListMembers = senParser.ParseSeniorityListForSecondRoundPilot (seniorityFileName + ".TXT", GlobalSettings.CurrentBidDetails.Postion, GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month);
                        GlobalSettings.IsVacationCorrection = false;

                        InvokeOnMainThread(() =>
                        {
                            alertVW = new NSAlert();
                            alertVW.AlertStyle = NSAlertStyle.Warning;
                            alertVW.MessageText = "WBidMax";
                            alertVW.InformativeText = "We are not able to read the seniority list, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view.\"";
                            alertVW.AddButton("OK");
                            alertVW.BeginSheet(this.Window);

                        });
					}

				}
				else if (GlobalSettings.CurrentBidDetails.Round == "S" && GlobalSettings.CurrentBidDetails.Postion == "FA")
				{
					
					seniorityFileName = WBidHelper.GetAppDataPath() + "/" + GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "SR";
				}

				if (seniorityFileName == string.Empty)
					return;

				//Saving seniority content
				//---
				try {
					var stream = File.Create (seniorityFileName + ".SL");
					ProtoSerailizer.SerializeObject (seniorityFileName + ".SL", seniorityListMembers, stream);
					stream.Dispose ();
					stream.Close ();
				} catch (Exception ex) {

					CommonClass.AppDelegate.ErrorLog (ex);
					InvokeOnMainThread (() => {
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					} );
				}
				//--------------
				string message = string.Empty;
				bool iSinSeniorityList = false;
				int intEmpNum = 0;
				if (GlobalSettings.CurrentBidDetails.Postion == "FA" || ((GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") && GlobalSettings.CurrentBidDetails.Round == "S"))
				{
					iSinSeniorityList = CheckEmpNumExistInSeniorityList(seniorityFileName + ".TXT");
					if (iSinSeniorityList)
					{
						message = "WBidMax found you in the Seniority List !! ";
					}
					else
					{
						message = "WBidMax DID NOT find you in the Seniority List." +
					"You may want to check your assigned Domicile for next month." +
					"DO NOT BID THESE LINES!!!!!";
					}

				}
				else
				{
					int papercount = 0;

					SeniorityListMember seniority = new SeniorityListMember();

					string sencheckempnum = (GlobalSettings.IsDifferentUser) ? GlobalSettings.ModifiedEmployeeNumber : _downloadFileDetails.UserId;
					intEmpNum = Convert.ToInt32(Regex.Match(sencheckempnum, @"\d+").Value);

					foreach (var item in seniorityListMembers)
					{
						seniority = item;

						if (Convert.ToInt32(item.EmpNum) == intEmpNum)
						{
							iSinSeniorityList = true;
							break;
						}
						if (item.BidType == "P")
							papercount++;
					}
					if (iSinSeniorityList)
					{

						int actualSeniority = seniority.DomicileSeniority - papercount;


						_seniorityListItem = new SeniorityListItem()
						{
							SeniorityNumber = seniority.DomicileSeniority,
							TotalCount = seniorityListMembers.LastOrDefault().DomicileSeniority,
							EBgType = seniority.EBG

						};

						message = "WBidMax found you in the Seniority List !! . You are number " + seniority.DomicileSeniority + " out of " + seniorityListMembers.LastOrDefault().DomicileSeniority + "\n\n There are " + papercount + " paper bids and ExTO/ETO above you, making you " + actualSeniority + " on the bid list";

					}
					else
					{
						//zero indicates the seniority number is not in domcile
						_seniorityListItem = new SeniorityListItem()
						{
							SeniorityNumber = 0,
							TotalCount = seniorityListMembers.LastOrDefault().DomicileSeniority,
							EBgType = ""

						};
						message = "WBidMax DID NOT find you in the Seniority List." +
						"You may want to check your assigned Domicile for next month." +
						"DO NOT BID THESE LINES!!!!!";
					}

				}

									//message = "WBidMax found you in the Seniority List";
					//InvokeOnMainThread(() =>
					//{
					//    UIAlertView alert = new UIAlertView("WBidMax", "WBidMax found you in the Seniority List", null, "OK", null);
					//    alert.Show();
					//});
					//InvokeOnMainThread (() => {
					//	alertVW = new NSAlert ();
					//	alertVW.AlertStyle = NSAlertStyle.Warning;
					//	alertVW.MessageText = "WBidMax";
					//	alertVW.InformativeText = message;

					//	alertVW.BeginSheet (this.Window);
					//});


				
				
				InvokeOnMainThread(() =>
				{
					var alert = new NSAlert();
					alert.Window.Title = "WBidMax";
					alert.MessageText = message;
					//alert.InformativeText = "There are no Latest News available..!";
					alert.AddButton("Ok");
					alert.AddButton("View In Senioriy List");
					
					//alert.AddButton ("Cancel");
					alert.Buttons[1].Activated += delegate
					{
						alert.Window.Close();
						NSApplication.SharedApplication.StopModal();

						var fileViewer = new FileWindowController();
						fileViewer.Window.Title = "Seniority List";
						var senlistName = CreateSeniorityLetterFileName();
						fileViewer.LoadTXT(senlistName);
						////fileViewer.ShowWindow(this);
						///CommonClass.MainController.Window.AddChildWindow(fileViewer.Window, NSWindowOrderingMode.Above);
						//fileViewer.LoadTXT(seniorityFileName + ".TXT");
						////this.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
						fileViewer.Window.MakeKeyAndOrderFront(this);
						NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
					};
					alert.Buttons[0].Activated += delegate
					{
						alert.Window.Close();
						NSApplication.SharedApplication.StopModal();

					};
					alert.RunModal();
				});

				//				InvokeOnMainThread(() =>
				//					{
				//						alertVW = new UIAlertView("WBidMax", message, null, "OK", null);
				//						alertVW.Show();
				//					});



				//  seniorityListMembers=

			} catch (Exception ex) {


				GlobalSettings.IsVacationCorrection = false;
				InvokeOnMainThread (() => {
					alertVW = new NSAlert ();
					alertVW.AlertStyle = NSAlertStyle.Warning;
					alertVW.MessageText = "WBidMax";
					alertVW.InformativeText = "The Seniority List is improperly formatted, as a result, the vacation corrections for the month are currently not available. Please notify Support in the Contact us view.\"";
					alertVW.AddButton ("OK");
					//						((NSButton)alert.Buttons[0]).Activated += (sender, e) => {
					//							DismissCurrentView();
					//						};
					alertVW.BeginSheet (this.Window);

//						alertVW = new UIAlertView("Error", "The Seniority List is improperly formatted, as a result, the vacation corrections for the month are currently not available.", null, "OK", null);
//						alertVW.Show();
				});



			}
			  

		}
		/// <summary>
		/// Create the seniority filename to open the file
		/// </summary>
		/// <returns></returns>
		private string CreateSeniorityLetterFileName()
		{
			string senioritylistfilename = string.Empty;
			if (GlobalSettings.CurrentBidDetails.Round == "M")
			{
				//first round seniority list
				senioritylistfilename = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "S" + ".TXT";
			}
			else
			{
				if (GlobalSettings.CurrentBidDetails.Postion == "FA")
				{
					// get flight attendant 2nd round cover letter and seniority list
					senioritylistfilename = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "SR" + ".TXT";

				}
				else
				{
					// get pilot 2nd round cover letter and seniority list
					senioritylistfilename = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "R" + ".TXT";
				}
			}
			return senioritylistfilename;
		}
		private bool CheckEmpNumExistInSeniorityList (string seniorityFileName)
		{
			StreamReader reader = new StreamReader (seniorityFileName);
			string employeeNumber = (GlobalSettings.IsDifferentUser) ? GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6,'0') : _downloadFileDetails.UserId;
			
			string allRead = reader.ReadToEnd ();                                       //Reads the whole text file to the end
			reader.Close ();                                                            //Closes the text file after it is fully read.
			string regMatch = employeeNumber.Substring (1, employeeNumber.Length - 1);
			if (seniorityListMembers != null)
			{
				var senlistemployee = employeeNumber.PadLeft(6, '0');
				var seniorityitem = seniorityListMembers.FirstOrDefault(x => x.EmpNum == senlistemployee);
				if (seniorityitem != null)
				{
					_seniorityListItem = new SeniorityListItem() { SeniorityNumber = seniorityitem.DomicileSeniority, TotalCount = seniorityListMembers[seniorityListMembers.Count - 1].DomicileSeniority,EBgType= seniorityitem.EBG };
				}
				else
				{
					if(seniorityListMembers.Count>0)
					//zero indicates the Not in Domicle,
					_seniorityListItem = new SeniorityListItem() { SeniorityNumber = 0, TotalCount = seniorityListMembers[seniorityListMembers.Count - 1].DomicileSeniority };
					else
						_seniorityListItem = new SeniorityListItem() { SeniorityNumber = 0, TotalCount = 0 };
				}
			}
			//string to search for inside of text5 c                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        file. It is case sensitive.
			return Regex.IsMatch (allRead, regMatch);
			//if (Regex.IsMatch(allRead, regMatch))                                        //If the match is found in allRead
			//{
			//    MessageBox.Show(System.Windows.Application.Current.MainWindow, "WBidMax found you in the Seniority List", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
			//}
			//else
			//{
			//    SoundPlayer player = new SoundPlayer();
			//    string fileName = "SIREN.WAV";
			//    string path = Path.Combine(WBidHelper.GetExecutablePath(), fileName);
			//    player.SoundLocation = path;
			//    player.Play();
			//    MessageBox.Show(System.Windows.Application.Current.MainWindow, "WBidMax DID NOT find you in the Seniority List." +
			//                                    "You may want to check your assigned Domicile for next month." +
			//                                    "DO NOT BID THESE LINES!!!!!", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
			//}

		}

		/// <summary>
		/// Parse Trip Files
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private Dictionary<string, Trip> ParseTripFile (string fileName)
		{
			Dictionary<string, Trip> Trips = new Dictionary<string, Trip> ();
			TripParser tripParser = new TripParser ();
			string filePath = WBidHelper.GetAppDataPath () + "/" + fileName.Substring (0, 6).ToString () + "/TRIPS";
			byte[] byteArray = File.ReadAllBytes (filePath);

			DateTime[] dSTProperties = DSTProperties.SetDSTProperties ();
			if (dSTProperties [0] != null && dSTProperties [0] != DateTime.MinValue) {
				GlobalSettings.FirstDayOfDST = dSTProperties [0];
			}
			if (dSTProperties [1] != null && dSTProperties [1] != DateTime.MinValue) {
				GlobalSettings.LastDayOfDST = dSTProperties [1];
			}
			//WBidHelper.SetDSTProperties();
			Trips = tripParser.ParseTrips (fileName, byteArray, GlobalSettings.FirstDayOfDST, GlobalSettings.LastDayOfDST);
			return Trips;
		}

		private Dictionary<string, Line> ParseLineFiles (string fileName)
		{
			Dictionary<string, Line> Lines = new Dictionary<string, Line> ();
			LineParser lineParser = new LineParser ();
			string filePath = WBidHelper.GetAppDataPath () + "/" + fileName.Substring (0, 6).ToString () + "/PS";
			byte[] byteArray = File.ReadAllBytes (filePath);
			Lines = lineParser.ParseLines (fileName, byteArray);
			return Lines;
		}

		public void DismissCurrentView ()
		{
			CommonClass.DownloadController.Window.Close ();
			CommonClass.DownloadController.Window.OrderOut (this);
//			CommonClass.DownloadController.Window.ResignKeyWindow ();
			NSApplication.SharedApplication.StopModal ();
		}

	}

}

