using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.SharedLibrary.Parser;
using WBid.WBidiPad.SharedLibrary;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Serialization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Net;

//using MiniZip.ZipArchive;
using System.ServiceModel;

//using WBidPushService.Model;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBidDataDownloadAuthorizationService;
using WBidDataDownloadAuthorizationService.Model;
using System.Text;
using PdfKit;
using WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo;

namespace WBid.WBidMac.Mac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		//MainWindowController mainWindowController;
		HomeWindowController homeWindowController;
		SubmitWindowController submitController;
		bool isManualUpdate;
		NSPanel overlayPanel;
		OverlayViewController overlay;

		WBidDataDwonloadAuthServiceClient client;

		public AppDelegate ()
		{
			BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
			client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
			client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 30);
			client.GetLatestVersionByUserRoleForMultyPlatformCompleted += client_GetLatestVersionByUserRoleForMultyPlatformCompleted;
		}

		void client_GetLatestVersionByUserRoleForMultyPlatformCompleted (object sender, GetLatestVersionByUserRoleForMultyPlatformCompletedEventArgs e)
		{
			try {
				var version = e.Result.VersionNumber;
				var file = e.Result.FileName;
				if (!string.IsNullOrEmpty (version) && version != CommonClass.AppVersion) {
					GlobalSettings.NewVersionData = new MaintananceData () {
						VersionNumber = version,
						FileName = file
					};
					InvokeOnMainThread (() => {
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "App Update Available!";
						alert.InformativeText = "A New Version " + version + " of WBidMax exists for Mac.";
						alert.AddButton ("Download Now");
						alert.AddButton ("Later");
						alert.Buttons [0].Activated += (object senderr, EventArgs ee) => {
							alert.Window.Close ();
//							menuCheckUpdate.Enabled = false;
							NSApplication.SharedApplication.StopModal ();
							overlayPanel = new NSPanel ();
							overlayPanel.SetContentSize (new CGSize (400, 120));
							overlay = new OverlayViewController ();
							overlay.OverlayText = "Downloading WBidMax dmg file \n Please wait..";
							overlayPanel.ContentView = overlay.View;
							if (CommonClass.HomeController != null && CommonClass.HomeController.Window.IsVisible)
								NSApplication.SharedApplication.BeginSheet (overlayPanel, CommonClass.HomeController.Window);
							else
								NSApplication.SharedApplication.BeginSheet (overlayPanel, CommonClass.MainController.Window);
							//if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {
                            if (Reachability.CheckVPSAvailable())
                            {
								DownloadNewINstallerFromServer (file, version);
							} else {
                                string alertmessage = GlobalSettings.VPSDownAlert;
                                if(Reachability.IsSouthWestWifiOr2wire())
                                {
                                    alertmessage = GlobalSettings.SouthWestConnectionAlert;
                                }
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.Window.Title = "WBidMax";
								alert.MessageText = "WBid MAC App Download";
                                alert.InformativeText = alertmessage;
								alert.AddButton ("OK");
								alert.RunModal ();
								NSApplication.SharedApplication.EndSheet (overlayPanel);
								overlayPanel.OrderOut (this);
							}
						};
						alert.RunModal ();
					});
				} else if (isManualUpdate) {
					isManualUpdate = false;
					InvokeOnMainThread (() => {
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "No Update Available!";
						alert.InformativeText = "You're using the Latest Verion of WBidMax";
						alert.RunModal ();
					});
				}
			} catch (Exception ex) {

			}
		}

		public void PerformVersionDownload ()
		{
//			menuCheckUpdate.Enabled = false;
			overlayPanel = new NSPanel ();
			overlayPanel.SetContentSize (new CGSize (400, 120));
			overlay = new OverlayViewController ();
			overlay.OverlayText = "Downloading WBidMax dmg file \n Please wait..";
			overlayPanel.ContentView = overlay.View;
			if (CommonClass.HomeController != null && CommonClass.HomeController.Window.IsVisible)
				NSApplication.SharedApplication.BeginSheet (overlayPanel, CommonClass.HomeController.Window);
			else
				NSApplication.SharedApplication.BeginSheet (overlayPanel, CommonClass.MainController.Window);
			if (Reachability.CheckVPSAvailable() && GlobalSettings.NewVersionData != null) {
				DownloadNewINstallerFromServer (GlobalSettings.NewVersionData.FileName, GlobalSettings.NewVersionData.VersionNumber);
			} else {
                string alertmessage = GlobalSettings.VPSDownAlert;
                if (Reachability.IsSouthWestWifiOr2wire())
                {
                    alertmessage = GlobalSettings.SouthWestConnectionAlert;
                }
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.Window.Title = "WBidMax";
				alert.MessageText = "WBid MAC App Download";
                alert.InformativeText = alertmessage;
				alert.RunModal ();
				NSApplication.SharedApplication.EndSheet (overlayPanel);
				overlayPanel.OrderOut (this);
			}

		}

		private void DownloadNewINstallerFromServer (string file, string version)
		{
			string url = GlobalSettings.installerDownloadUrl + file;
			WebClient webclient = new WebClient ();
			string downloadPath = NSSearchPath.GetDirectories (NSSearchPathDirectory.DownloadsDirectory, NSSearchPathDomain.All, true).First ();
			webclient.DownloadFileAsync (new Uri (url), downloadPath + "/WBidMax_" + version + ".dmg");
			webclient.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {

				if (e.Error == null) {

					InvokeOnMainThread (() => {
//						menuCheckUpdate.Enabled = true;
						NSApplication.SharedApplication.EndSheet (overlayPanel);
						overlayPanel.OrderOut (this);
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "Download Completed!";
						alert.InformativeText = "The new version is downloaded to your Downloads folder as a .dmg file. Please Quit WBidMax to install from .dmg file";
						alert.RunModal ();
					});

					ClientRequestModel clientRequestModel = new ClientRequestModel ();
					clientRequestModel.OperatingSystem = CommonClass.OperatingSystem;
					clientRequestModel.Platform = CommonClass.Platform;
					clientRequestModel.Version = CommonClass.AppVersion;
					clientRequestModel.EmployeeNumber = Convert.ToInt32 (GlobalSettings.WbidUserContent.UserInformation.EmpNo);

					client.LogDownloadUpdatesAsync (clientRequestModel);


				}
			};
		}
		//		private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		//		{
		//			if (e.Error == null)
		//			{
		//				/*IsBusy = false;
		//				DownloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
		//
		//				ClientRequestModel clientRequestModel = new ClientRequestModel();
		//				clientRequestModel.OperatingSystem = GetOSFriendlyName();
		//				clientRequestModel.Platform = "PC";
		//				clientRequestModel.Version = MaintananceData.VersionNumber;
		//				clientRequestModel.EmployeeNumber = Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
		//				var result = DownloadAuthServiceClient.LogDownloadUpdates(clientRequestModel);
		//
		//
		//
		//				//((MainViewModel)ServiceLocator.Current.GetInstance<MainViewModel>()).IsStateModified = false;
		//				InstallDownloadedSetup();
		//				*/
		//			}
		//			else
		//			{
		//				//IsBusy = false;
		//				//Xceed.Wpf.Toolkit.MessageBox.Show("Update failed..Please try again later..!", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
		//			}
		//		}
		public override void DidFinishLaunching (NSNotification notification)
		{


			NSApplication.SharedApplication.Delegate = this;
			CommonClass.AppDelegate = this;
			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            Reachability.CheckVPSAvailable();
			LoadINIFileData ();
			//GlobalSettings.WBidINIContent.User.IsModernViewShade = true;
			if (File.Exists (WBidHelper.WBidUserFilePath)) {
				GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
			}
			if (File.Exists (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log") && GlobalSettings.WBidINIContent.User.IsNeedCrashMail && GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {
				//Check internet wavailable
                if (Reachability.CheckVPSAvailable()) {
					string content = System.IO.File.ReadAllText (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log");
					WBidMail wbidMail = new WBidMail ();
					wbidMail.SendCrashMail (content);
					File.Delete (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log");
				}
			}

			//copyBundledFileToAppdata ("ColumnDefinitions.xml");
			LoadColumnDefenitionData ();

			if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {
                if (Reachability.CheckVPSAvailable()) {
					client.GetLatestVersionByUserRoleForMultyPlatformAsync (new WBidDataDownloadAuthorizationService.Model.EmployeeVersionDetails {
						EmpNum = int.Parse (GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToLower ().Replace ("e", "").Replace ("x", "")),
						Platform = CommonClass.Platform,
						Version = CommonClass.AppVersion
					});
				}
			}
          
			homeWindowController = new HomeWindowController ();
			CommonClass.HomeController = homeWindowController;
			homeWindowController.Window.MakeKeyAndOrderFront (this);
			CommonClass.isHomeWindow = true;
			setupMenu ();
			ReloadMenu ();
			GlobalSettings.WBidINIContent.Data.IsCompanyData = true;
		}

		void HandleUnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine (e.ExceptionObject.ToString ());
			var exception = (Exception)e.ExceptionObject;

			var submitResult =	GenerateErrorMessageFromException (exception);


			if (exception != null) {

				if (!Directory.Exists (WBidHelper.GetAppDataPath () + "/" + "Crash")) {
					Directory.CreateDirectory (WBidHelper.GetAppDataPath () + "/" + "Crash");
				}

				System.IO.File.AppendAllText (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log", submitResult);
			}

		}


		public void ShowErrorMessage (string message)
		{
			var alert = new NSAlert ();
			alert.AlertStyle = NSAlertStyle.Critical;
			alert.Window.Title = "WBidMax";
			alert.MessageText = "Error";
			alert.InformativeText = message;
			alert.AddButton ("OK");
			alert.RunModal ();

		}


		public void ErrorLog (Exception exception)
		{

			// string submitResult = "\r\n\r\n\r\n Crash Report : \r\n\r\n\r\n" + "\r\n Date: " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + "\r\n\r\n Device: " + UIDevice.CurrentDevice.LocalizedModel + "\r\n\r\n Crash Details: " + ex + "\r\n\r\n Data: " + currentBid + "\r\n\r\n" + " ******************************* \r\n";

			var submitResult =	GenerateErrorMessageFromException (exception);
            if (Reachability.CheckVPSAvailable()) {
				WBidMail wbidMail = new WBidMail ();
				wbidMail.SendCrashMail (submitResult);
			} else {

				if (!Directory.Exists (WBidHelper.GetAppDataPath () + "/" + "Crash")) {
					Directory.CreateDirectory (WBidHelper.GetAppDataPath () + "/" + "Crash");
				}

				System.IO.File.AppendAllText (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log", submitResult);
			}

		}


		private string GenerateErrorMessageFromException (Exception exception)
		{
			string error = string.Empty;
			Console.WriteLine (exception.ToString ());

			string currentBid = FileOperations.ReadCurrentBidDetails (WBidHelper.GetAppDataPath () + "/CurrentDetails.txt");
			try {
				if (exception != null) {
					Exception InnerException = exception.InnerException;
					string message = exception.Message;
					string where = exception.StackTrace.Split (new string[] { " at " }, 2, StringSplitOptions.None) [1];
					string source = exception.Source;

					if (InnerException != null) {
						if (InnerException.Message != null) {
							message = InnerException.Message;
						}

						if (InnerException.StackTrace != null) {
							where = InnerException.StackTrace.Split (new string[] { " at " }, 2, StringSplitOptions.None) [1];
						}

						source = InnerException.Source;

						if (InnerException.InnerException != null) {
							if (InnerException.InnerException.Message != null) {
								message += " -> " + InnerException.InnerException.Message;
							}

							if (InnerException.InnerException.StackTrace != null) {
								where += "\r\n\r\n -> " + InnerException.InnerException.StackTrace.Split (new string[] { " at " },
									2, StringSplitOptions.None) [1];
							}

							if (InnerException.InnerException.Source != null) {
								source += " -> " + InnerException.InnerException.Source;
							}
						}
					}

					if (where.Length > 1024) {
						where = where.Substring (0, 1024);
					}


					var submitResult = "\r\n WBidMax Error Details.\r\n\r\n Error  :  " + message + "\r\n\r\n Where  :  " + where + "\r\n\r\n Source   :  " + source + "\r\n\r\n Version : " + System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString () + "\r\n\r\n Date  :" + DateTime.Now;
					submitResult += "\r\n\r\n Data :" + currentBid + "\r\n\r\n Device :" + "Mac" + "\r\n\r\n";
					error = submitResult;
				}
			} catch (Exception) {
				error = string.Empty;
				var submitResult = "\r\n WBidMax Error Details.\r\n\r\n Error  :  " + exception.ToString () + "\r\n\r\n Version : " + System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString () + "\r\n\r\n Date  :" + DateTime.Now;
				submitResult += "\r\n\r\n Data :" + currentBid + "\r\n\r\n Device :" + "Mac" + "\r\n\r\n";
				error = submitResult;
			}

			return error;

		}


		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}


		public void ReloadMenu ()
		{
			if (!CommonClass.isHomeWindow) {
				menuBidStuff.Hidden = false;
				menuView.Hidden = false;
				menuCSW.Hidden = false;
				menuResetAll.Enabled = true;
				menuClearConstraints.Enabled = true;
				menuBuddyBid.Hidden = false;
				if (GlobalSettings.CurrentBidDetails.Postion == "FA")
					menuBuddyBid.Title = "Buddy Bid";
				else
					menuBuddyBid.Title = "Avoidance Bid";
				menuClearTags.Enabled = true;
			} else {
				menuBidStuff.Hidden = true;
				menuView.Hidden = true;
				menuCSW.Hidden = true;
				menuResetAll.Enabled = false;
				menuClearConstraints.Enabled = false;
				menuRemoveToplock.Enabled = false;
				menuRemoveBottomLock.Enabled = false;
				menuBuddyBid.Hidden = true;
				menuClearTags.Enabled = false;
			}

			if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData") {
				menuSubmit.Enabled = false;
				menuBidEditor.Enabled = false;
			} else {
				menuSubmit.Enabled = true;
				menuBidEditor.Enabled = true;
			}

		}

		/// <summary>
		/// Copies the bundled XMLs to appdata.
		/// </summary>
		private static void copyBundledFileToAppdata (string fileName)
		{
			var sourcePath = Path.Combine (NSBundle.MainBundle.ResourcePath, fileName);
			var destinationPath = WBidHelper.GetAppDataPath () + "/" + fileName;
			try {
				//---copy only if file does not exist---
				if (File.Exists (destinationPath)) {
					File.Delete (destinationPath);
				}
				if (!File.Exists (destinationPath)) {
					File.Copy (sourcePath, destinationPath);

					if (File.Exists (destinationPath)) {
						LoadColumnDefenitionData ();
					}

				} else {
					LoadColumnDefenitionData ();
				}
			} catch (Exception e) {
				Console.WriteLine (e.Message);
				throw e;
			}

		}

		private static void LoadColumnDefenitionData ()
		{

			GlobalSettings.columndefinition = (List<ColumnDefinition>)XmlHelper.DeserializeFromXml<ColumnDefinitions> (WBidHelper.GetWBidColumnDefinitionFilePath ());
		}



		public void HandleViewCap ()
		{
			if (GlobalSettings.CurrentBidDetails.Postion == "FA")
				menuCAP.Hidden = true;
			else
				menuCAP.Hidden = false;
		}

//		menuCAP.Activated += HandleCAP;
		/// <summary>
		/// Load/Read the INI file data from the app data folder.or Create the INI file if the INI file is not present in the app data folder.
		/// </summary>
		/// 
		private static void LoadINIFileData ()
		{
			if (!Directory.Exists (WBidHelper.GetAppDataPath ())) {
				//create app data folder
				WBidHelper.CreateAppDataDirectory ();
			}
			//cheCk the INI file is ceated or not.If not,create it.

			if (!File.Exists (WBidHelper.GetWBidINIFilePath ())) {
				WBidINI wbidINI = WBidCollection.CreateINIFile ();
				XmlHelper.SerializeToXml (wbidINI, WBidHelper.GetWBidINIFilePath ());
			}
			WBidIntialState WBidIntialState;
			if (!File.Exists (WBidHelper.GetWBidDWCFilePath ())) 
			{
				WBidIntialState = WBidCollection.CreateDWCFile (GlobalSettings.DwcVersion);
				XmlHelper.SerializeToXml (WBidIntialState, WBidHelper.GetWBidDWCFilePath ());
			}
			else
			{
				
				WBidIntialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
				if (Convert.ToDecimal(WBidIntialState.Version) < Convert.ToDecimal(GlobalSettings.DwcVersion)) 
				{
					WBidIntialState = WBidCollection.CreateDWCFile (GlobalSettings.DwcVersion);
					XmlHelper.SerializeToXml (WBidIntialState, WBidHelper.GetWBidDWCFilePath ());
				}
			}


			//read the values of the INI file.
			GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI> (WBidHelper.GetWBidINIFilePath ());
			GlobalSettings.WBidINIContent.Cities = GlobalSettings.WBidINIContent.Cities.OrderBy(x => x.Name).ToList();

//			if ((decimal.Parse(GlobalSettings.WBidINIContent.Version??"0.0")) < decimal.Parse( GlobalSettings.IniFileVersion))
			if (GlobalSettings.WBidINIContent.BidLineVacationColumns == null || GlobalSettings.WBidINIContent.BidLineVacationColumns.Count == 0 || GlobalSettings.WBidINIContent.ModernVacationColumns == null || GlobalSettings.WBidINIContent.ModernVacationColumns.Count == 0 || GlobalSettings.WBidINIContent.SummaryVacationColumns == null || GlobalSettings.WBidINIContent.SummaryVacationColumns.Count == 0) {
				GlobalSettings.WBidINIContent.BidLineNormalColumns = new List<int> () { 36, 37, 27, 34, 12 };
				GlobalSettings.WBidINIContent.BidLineVacationColumns = new List<int> () { 36, 53, 200, 34, 12 };

				GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int> () { 36, 37, 27, 34, 12 };
				GlobalSettings.WBidINIContent.ModernVacationColumns = new List<int> () { 36, 53, 200, 34, 12 };

				GlobalSettings.WBidINIContent.SummaryVacationColumns = WBidCollection.GenerateDefaultVacationColumns ();
				GlobalSettings.WBidINIContent.Version = GlobalSettings.IniFileVersion;
				XmlHelper.SerializeToXml (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());


			}
			if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count>3 &&  GlobalSettings.WBidINIContent.SummaryVacationColumns[3].Id==67)
			{
				GlobalSettings.WBidINIContent.SummaryVacationColumns [3] = new DataColumn{ Order = 4, Id = 3, Width = 50 };
				GlobalSettings.WBidINIContent.SummaryVacationColumns [4] = new DataColumn{ Order = 5, Id = 67, Width = 50 };
			}
			//if (decimal.Parse(GlobalSettings.WBidINIContent.Version ?? "0.0") < 1.2m)
			//{
				if (!GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 67))
				{
					if (GlobalSettings.WBidINIContent.DataColumns.Count > 4)
					{
						for (int count = 4; count < GlobalSettings.WBidINIContent.DataColumns.Count; count++)
						{
							GlobalSettings.WBidINIContent.DataColumns[count].Order = GlobalSettings.WBidINIContent.DataColumns[count].Order + 1;
						}

						GlobalSettings.WBidINIContent.DataColumns.Insert(4, new DataColumn() { Id = 67, Order = 5, Width = 60 });
					}
				}
				if (!GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 67))
				{
					if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count > 4)
					{
						for (int count = 4; count < GlobalSettings.WBidINIContent.SummaryVacationColumns.Count; count++)
						{
							GlobalSettings.WBidINIContent.SummaryVacationColumns[count].Order = GlobalSettings.WBidINIContent.SummaryVacationColumns[count].Order + 1;
						}

						GlobalSettings.WBidINIContent.SummaryVacationColumns.Insert(4, new DataColumn() { Id = 67, Order = 5, Width = 60 });
					}
				}
			//}
			if (GlobalSettings.WBidINIContent.MainWindowSize == null)
			{
				GlobalSettings.WBidINIContent.MainWindowSize = new MainWindowSize (){
				Height = 500,
				Width = 720,
				Left = 358,
				Top = 103
				};
			}	
			if (GlobalSettings.WBidINIContent.CSWViewSize == null)
			{
				GlobalSettings.WBidINIContent.CSWViewSize = new MainWindowSize (){
					Height = 600,
					Width = 450,
					Left = 358,
					Top = 50
				};
			}	
			if (GlobalSettings.WBidINIContent.AmPmConfigure == null) {

				GlobalSettings.WBidINIContent.AmPmConfigure = new AmPmConfigure () {
					HowCalcAmPm = 1,
					AmPush = TimeSpan.FromHours (4),
					AmLand = TimeSpan.FromHours (19),
					PmPush = TimeSpan.FromHours (11),
					PmLand = TimeSpan.FromHours (2),
					NitePush = TimeSpan.FromHours (22),
					NiteLand = TimeSpan.FromHours (7),
					NumberOrPercentageCalc = 1,
					NumOpposites = 3,
					PctOpposities = 20

				};
			}
			if (GlobalSettings.WBidINIContent.Version == null) {
				GlobalSettings.WBidINIContent.Version = "1.0";
				GlobalSettings.WBidINIContent.Updates = new INIUpdates {
					Trips = 0,
					News = 0,
					Cities = 0,
					Hotels = 0,
					Domiciles = 0,
					Equipment = 0,
					EquipTypes = 0
				};
				XmlHelper.SerializeToXml (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
			}
			if (GlobalSettings.WBidINIContent.Data == null) {
				GlobalSettings.WBidINIContent.Data = new Data ();
				GlobalSettings.WBidINIContent.Data.IsCompanyData = true;
			}
			if (GlobalSettings.WBidINIContent.User == null) {
				GlobalSettings.WBidINIContent.User = new User () {
					IsNeedBidReceipt = true,
					SmartSynch = false,
					AutoSave = false,
					IsNeedCrashMail = true
				};
			}
			if (decimal.Parse(GlobalSettings.WBidINIContent.Version) < 1.3m)
                {
                    //remove the legs in 500 and legs in 300 columns
                    GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
                    GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

                    GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
                    GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

                    GlobalSettings.WBidINIContent.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);
                    GlobalSettings.WBidINIContent.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);

                    GlobalSettings.WBidINIContent.BidLineNormalColumns.RemoveAll(x => x == 58 || x == 59);
                    GlobalSettings.WBidINIContent.BidLineVacationColumns.RemoveAll(x => x == 58 || x == 59);
                    
                }
			if (decimal.Parse(GlobalSettings.WBidINIContent.Version) < 1.4m)
			{
				if (!GlobalSettings.WBidINIContent.Domiciles.Any(X => X.DomicileName == "AUS"))
					GlobalSettings.WBidINIContent.Domiciles.Add(new Domicile{DomicileName="AUS",DomicileId=11,Code="T",Number=11});
				if (!GlobalSettings.WBidINIContent.Domiciles.Any(X => X.DomicileName == "FLL"))
					GlobalSettings.WBidINIContent.Domiciles.Add(new Domicile{DomicileName="FLL",DomicileId=12,Code="T",Number=12});

			}
            if (decimal.Parse(GlobalSettings.WBidINIContent.Version) < 1.6m)
            {
                if (!GlobalSettings.WBidINIContent.Domiciles.Any(X => X.DomicileName == "LAX"))
                    GlobalSettings.WBidINIContent.Domiciles.Add(new Domicile { DomicileName = "LAX", DomicileId = 13, Code = "X", Number = 13 });

            }
			if (decimal.Parse(GlobalSettings.WBidINIContent.Version) < 1.5m)
			{
				GlobalSettings.WBidINIContent.User.IsModernViewShade = true;
			}
			if (File.Exists (WBidHelper.WBidUserFilePath)) {
				GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
			}
            if (GlobalSettings.WBidINIContent.SenioritylistFormat.Count == 0)
            {
                GlobalSettings.WBidINIContent.SenioritylistFormat = WBidCollection.getDefaultSenlistFormatValue();
            }
		}

		#region MenuItems

		private void setupMenu ()
		{
			menuNewBid.Activated += HandleNewBid;
			menuSubmit.Activated += HandleSubmit;
			menuBidEditor.Activated += HandleBidEditor;
			menuGetAwards.Activated += HandleGetAwards;
			menuBidReceipt.Activated += HandleBidReceipt;
			menuConfigure.Activated += HandleConfigure;
			menuSummaryView.Activated += HandleViewChange;
			menuBidLineView.Activated += HandleViewChange;
			menuModernView.Activated += HandleViewChange;
			menuCSW.Activated += HandleCSW;
			menuCAP.Activated += HandleCAP;
			menuLatestNews.Activated += HandleLatestNews;
			menuChangeUserInfo.Activated += HandleChangeUserInfo;
			menuResetAll.Activated += HandleResetAll;
			menuClearConstraints.Activated += HandleClearConstrints; 
			menuRemoveToplock.Activated += HandleRemoveTopLock;
			menuRemoveBottomLock.Activated += HandleRemoveBottomLock;
			menuExportToCalendar.Enabled = false;
			menuCoverLetter.Activated += HandleViewFile;
			menuSeniorityList.Activated += HandleViewFile;
			menuViewAwards.Activated += HandleViewFile;
			menuBuddyBid.Activated += HandleBuddyBid;
			menuCheckUpdate.Activated += HandleCheckUpdate;
			menuContact.Activated += HandleContactUs;
			menuNewHelp.Activated += HandleHelp;
			menuCurrentView.Activated += HandleCurrentViewPrint;
			menuAllPairings.Activated += HandleAllPairingsPrint;
			menuFormattedReceipt.Activated += HandleFormattedReceiptPrint;
			menuWBidHotels.Activated += HandleWBidHotelsPrint;
			menuPairings.Activated += HandlePairings;
			menuClearTags.Activated += HandleClearTags;
		}

		void HandleClearTags (object sender, EventArgs e)
		{
			foreach (var line in GlobalSettings.Lines) {
				line.Tag = string.Empty;
			}
			CommonClass.MainController.ReloadAllContent ();
		}

		void HandlePairings (object sender, EventArgs e)
		{
			var pairing = new PairingWindowController ();
			CommonClass.MainController.Window.AddChildWindow (pairing.Window, NSWindowOrderingMode.Above);
			NSApplication.SharedApplication.RunModalForWindow (pairing.Window);
		}

		void HandleWBidHotelsPrint (object sender, EventArgs e)
		{
			try {
				if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Hotels != null) {
					string heading = string.Empty;
					heading = "WBid Hotel List \n" + DateTime.Now.ToString ("D") + "\n";
					StringBuilder hotelList = new StringBuilder ();
					GlobalSettings.WBidINIContent.Hotels.HotelList.ForEach (x => hotelList.Append (x.City + " - " + x.Hotels + "\r\n"));
				
					var str = "\n" + heading + "\n";
					str += hotelList.ToString ();
				
					var inv = new InvisibleWindowController ();
					CommonClass.MainController.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
					var txt = new NSTextView (new CGRect (0, 0, 550, 550));
					txt.Font = NSFont.FromFontName ("Courier", 9);
					inv.Window.ContentView.AddSubview (txt);
					txt.Value = str;
					var pr = NSPrintInfo.SharedPrintInfo;
					pr.VerticallyCentered = false;
					pr.TopMargin = 2.0f;
					pr.BottomMargin = 2.0f;
					pr.LeftMargin = 1.0f;
					pr.RightMargin = 1.0f;
					txt.Print (this);
					inv.Close ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleFormattedReceiptPrint (object sender, EventArgs e)
		{
			try {
				string path = WBidHelper.GetAppDataPath ();
				List<string> filenames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
					.Where(s => s.ToLower().EndsWith(".rct") || s.ToLower().EndsWith(".pdf")).ToList();
				filenames.Remove ("news.pdf");
				if (filenames.Count > 1) {
					//show separate controller
					var panel = new NSPanel ();
					var receiptView = new BidReceiptViewController ();
					receiptView.fileNames = filenames;
					receiptView.toPrint = true;
					CommonClass.Panel = panel;
					panel.SetContentSize (new CGSize (300, 350));
					panel.ContentView = receiptView.View;
					NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
				
				} else if (filenames.Count == 1) {
					string filePath = WBidHelper.GetAppDataPath () + "/" + filenames [0];
					var inv = new InvisibleWindowController ();
					CommonClass.MainController.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);

					if (Path.GetExtension (filenames [0]).ToLower () == ".pdf") 
					{
					var fileUrl = NSUrl.FromFilename (filePath);
					PdfDocument aPdfDocument = new PdfDocument(fileUrl);
					PdfView aPDFView = new PdfView();
					aPDFView.Document = aPdfDocument;
					inv.Window.ContentView.AddSubview (aPDFView);
						var pr = NSPrintInfo.SharedPrintInfo;
						pr.VerticallyCentered = false;
						pr.TopMargin = 2.0f;
						pr.BottomMargin = 2.0f;
						pr.LeftMargin = 1.0f;
						pr.RightMargin = 1.0f;
						aPDFView.Print(this);


					}
					else
					{

					
					var txt = new NSTextView (new CGRect (0, 0, 550, 550));
					txt.Font = NSFont.FromFontName ("Courier", 8);
					inv.Window.ContentView.AddSubview (txt);
					txt.Value = CommonClass.FormatBidReceipt (filePath);
					var pr = NSPrintInfo.SharedPrintInfo;
					pr.VerticallyCentered = false;
					pr.TopMargin = 2.0f;
					pr.BottomMargin = 2.0f;
					pr.LeftMargin = 1.0f;
					pr.RightMargin = 1.0f;
					txt.Print (this);
					inv.Close ();
					}
				} else {
					var alert = new NSAlert ();
					alert.AlertStyle = NSAlertStyle.Informational;
					alert.Window.Title = "WBidMax";
					alert.MessageText = "Bid Receipt";
					alert.InformativeText = "There is no bid reciept available..!";
					alert.RunModal ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleAllPairingsPrint (object sender, EventArgs e)
		{
			try {
				if (CommonClass.selectedLine != 0) {
					var str = CommonClass.PrintAllPairings ();
					var inv = new InvisibleWindowController ();
					CommonClass.MainController.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
					var txt = new NSTextView (new CGRect (0, 0, 550, 550));
					txt.Font = NSFont.FromFontName ("Courier", 6);
					inv.Window.ContentView.AddSubview (txt);
					txt.Value = str;
					var pr = NSPrintInfo.SharedPrintInfo;
					pr.VerticallyCentered = false;
					pr.TopMargin = 2.0f;
					pr.BottomMargin = 2.0f;
					pr.LeftMargin = 1.0f;
					pr.RightMargin = 1.0f;
					txt.Print (this);
					inv.Close ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleCurrentViewPrint (object sender, EventArgs e)
		{
			try {
				if (GlobalSettings.WBidINIContent.ViewType == 1) {
					// Summary view
					var printContent = CommonClass.PrintSummaryLines ();
					var inv = new InvisibleWindowController ();
					CommonClass.MainController.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
					var txt = new NSTextView (new CGRect (0, 0, 550, 550));
					txt.Font = NSFont.FromFontName ("Courier", 5);
					inv.Window.ContentView.AddSubview (txt);
					txt.Value = printContent;
					var pr = NSPrintInfo.SharedPrintInfo;
					pr.VerticallyCentered = false;
					pr.TopMargin = 2.0f;
					pr.BottomMargin = 2.0f;
					pr.LeftMargin = 1.0f;
					pr.RightMargin = 1.0f;
					txt.Print (this);
					inv.Close ();
				
				} else {
					// Bidline or Modern view
					var panel = new NSPanel ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CGSize (350, 180));
					panel.ContentView = CommonClass.MainController.vwPrintOption;
					NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleHelp (object sender, EventArgs e)
		{
			if (CommonClass.HelpController == null) {
				var help = new HelpWindowController ();
				CommonClass.HelpController = help;
			}
			CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
			//NSApplication.SharedApplication.RunModalForWindow (help.Window);
		}

		void HandleContactUs (object sender, EventArgs e)
		{
			var contact = new ContactUsWindowController ();
			contact.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (contact.Window);
		}

		void HandleCheckUpdate (object sender, EventArgs e)
		{
			isManualUpdate = true;
            if (Reachability.CheckVPSAvailable()) {
				client.GetLatestVersionByUserRoleForMultyPlatformAsync (new WBidDataDownloadAuthorizationService.Model.EmployeeVersionDetails {
					EmpNum = int.Parse (GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToLower ().Replace ("e", "").Replace ("x", "")),
					Platform = CommonClass.Platform,
					Version = CommonClass.AppVersion
				});
			}
		}

		void HandleBuddyBid (object sender, EventArgs e)
		{
			if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
				var panel = new NSPanel ();
				var changeBud = new ChangeBuddyViewController ();
				CommonClass.Panel = panel;
				panel.SetContentSize (new CGSize (400, 190));
				panel.ContentView = changeBud.View;
				NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
			} else {
				var panel = new NSPanel ();
				var changeAvoid = new ChangeAvoidanceViewController ();
				CommonClass.Panel = panel;
				panel.SetContentSize (new CGSize (400, 270));
				panel.ContentView = changeAvoid.View;
				NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
			}
		}



		void HandleClearConstrints (object sender, EventArgs e)
		{
			CommonClass.MainController.ClearConstraintsAndweights ();
		}

		void HandleChangeUserInfo (object sender, EventArgs e)
		{
			//			var userReg = new UserRegistrationWindowController ();
			//			userReg.IsEditMode = true;
			////			CommonClass.MainController.Window.AddChildWindow (userReg.Window,NSWindowOrderingMode.Above);
			//			userReg.Window.MakeKeyAndOrderFront (this);
			//			NSApplication.SharedApplication.RunModalForWindow (userReg.Window);

			var userUpdate = new UserUpdateInfoController();
			userUpdate.title = "User Update Info";
			userUpdate.buttonName = "Update";
			userUpdate.isRegister = false;
			userUpdate.authenticatedUserId = GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToLower().Replace("e", "").Replace("x", "");
			userUpdate.Window.StandardWindowButton(NSWindowButton.CloseButton).Enabled = false;
			//CommonClass.MainController.Window.AddChildWindow(userUpdate.Window, NSWindowOrderingMode.Above);
			userUpdate.Window.MakeKeyAndOrderFront(this);
			NSApplication.SharedApplication.RunModalForWindow(userUpdate.Window);

		}

		void HandleResetAll (object sender, EventArgs e)
		{
			CommonClass.MainController.ResetAll ();
		}

		void HandleRemoveTopLock (object sender, EventArgs e)
		{
			CommonClass.MainController.RemoveTopLock ();
		}

		void HandleRemoveBottomLock (object sender, EventArgs e)
		{
			CommonClass.MainController.RemoveBottomLock ();

		}

		void HandleLatestNews (object sender, EventArgs e)
		{
			var newsPath = WBidHelper.GetAppDataPath () + "/news.pdf";
			if (File.Exists (newsPath)) {
				var fileViewer = new FileWindowController ();
				fileViewer.Window.Title = "Latest News";
				fileViewer.LoadPDF ("news.pdf");

				CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
				fileViewer.Window.MakeKeyAndOrderFront (this);
			} else {
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Latest News";
				alert.InformativeText = "There are no Latest News available..!";
				alert.AddButton ("OK");
				alert.RunModal ();
			}
		}

		void HandleCSW (object sender, EventArgs e)
		{
			CommonClass.MainController.ShowCSW ();
		}

		void HandleCAP (object sender, EventArgs e)
		{
			var showCap = new ShowCAPController ();
			CommonClass.MainController.Window.AddChildWindow (showCap.Window, NSWindowOrderingMode.Above);
			NSApplication.SharedApplication.RunModalForWindow (showCap.Window);
		}


		void HandleViewChange (object sender, EventArgs e)
		{
			var m = (NSMenuItem)sender;
			CommonClass.MainController.ToggleView ((int)m.Tag);
		}

		void HandleConfigure (object sender, EventArgs e)
		{
			var config = new ConfigurationWindowController ();
			CommonClass.ConfigureController = config;
			config.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (config.Window);
		}

		void HandleBidReceipt (object sender, EventArgs e)
		{
			string path = WBidHelper.GetAppDataPath ();
			List<string> filenames = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).Select(Path.GetFileName)
				.Where(s => s.ToLower().EndsWith(".rct") || s.ToLower().EndsWith(".pdf")).ToList();
			filenames.Remove ("news.pdf");
			//List<string> filenames = Directory.EnumerateFiles (path, "*.RCT", SearchOption.AllDirectories).Select (Path.GetFileName).ToList ();
			if (filenames.Count > 1) {
				//show separate controller
				var panel = new NSPanel ();
				var receiptView = new BidReceiptViewController ();
				receiptView.fileNames = filenames;
				CommonClass.Panel = panel;
				panel.SetContentSize (new CGSize (300, 350));
				panel.ContentView = receiptView.View;
				NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);

			} else if (filenames.Count == 1) {
				InvokeOnMainThread (() => {
					var fileViewer = new FileWindowController ();
					fileViewer.Window.Title = "Bid Receipt";
					if(Path.GetExtension(filenames [0]).ToLower()==".rct")
					{
					fileViewer.LoadTXT (filenames [0]);
					}
					else
					{
						fileViewer.LoadPDF (filenames [0]);
					}
					CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
					fileViewer.Window.MakeKeyAndOrderFront (this);
				});
			} else {
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Bid Receipt";
				alert.InformativeText = "There is no bid reciept available..!";
				alert.AddButton ("OK");
				alert.RunModal ();
			}

		}

		void HandleGetAwards (object sender, EventArgs e)
		{
			var getAwards = new GetAwardsWindowController ();
			//CommonClass.MainController.Window.AddChildWindow (getAwards.Window, NSWindowOrderingMode.Above);
			getAwards.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (getAwards.Window);
		}

		void HandleViewFile (object sender, EventArgs e)
		{


			var viewFile = new ViewFileWindowController ();
			viewFile.ViewType = (int)((NSMenuItem)sender).Tag;
			//CommonClass.MainController.Window.AddChildWindow (viewFile.Window, NSWindowOrderingMode.Above);
			//viewFile.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (viewFile.Window);
		}

		void HandleBidEditor (object sender, EventArgs e)
		{
			//check blank lines are in low to high order. alert if it not satisfy the condtion
			List<int> sortedblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).OrderBy(z=>z).ToList();
			List<int> currentblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).ToList();
			bool isBlankLinesCorrectOrder = true;
			for (int i = 0; i < sortedblanklines.Count; i++) {
				if (sortedblanklines [i] != currentblanklines [i]) {
					isBlankLinesCorrectOrder = false;
					break;
				}

			}
			if (!isBlankLinesCorrectOrder) {

				//
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Your blank lines are not in order of lowest to highest. Click Cancel to go back and fix this issue.";
				alert.AddButton ("OK");
				alert.AddButton ("Cancel");
				alert.Buttons [0].Activated += (object sender1, EventArgs ex) => {

					alert.Window.Close ();
					NSApplication.SharedApplication.EndSheet (alert.Window);

					var bidEdit = new BidEditorPrepWindowController ();
					CommonClass.MainController.Window.AddChildWindow (bidEdit.Window, NSWindowOrderingMode.Above);
					bidEdit.Window.MakeKeyAndOrderFront (this);
					NSApplication.SharedApplication.RunModalForWindow (bidEdit.Window);

				};
				alert.RunSheetModal (CommonClass.MainController.Window);

			}
			else
			{
			var bidEdit = new BidEditorPrepWindowController ();
			CommonClass.MainController.Window.AddChildWindow (bidEdit.Window, NSWindowOrderingMode.Above);
			bidEdit.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (bidEdit.Window);
			}
		}

		void HandleSubmit (object sender, EventArgs e)
		{

			//check blank lines are in low to high order. alert if it not satisfy the condtion
				List<int> sortedblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).OrderBy(z=>z).ToList();
			List<int> currentblanklines=GlobalSettings.Lines.Where(x=>x.BlankLine).Select(y=>y.LineNum).ToList();
			bool isBlankLinesCorrectOrder = true;
			for (int i = 0; i < sortedblanklines.Count; i++) {
				if (sortedblanklines [i] != currentblanklines [i]) {
					isBlankLinesCorrectOrder = false;
					break;
				}

			}
			if (!isBlankLinesCorrectOrder) {
				
				//
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Your blank lines are not in order of lowest to highest. Click Cancel to go back and fix this issue";
				alert.AddButton ("OK");
				alert.AddButton ("Cancel");
				alert.Buttons [0].Activated += delegate {
					
				
					alert.Window.Close ();
					NSApplication.SharedApplication.EndSheet (alert.Window);



					if (submitController == null)
						submitController = new SubmitWindowController ();
					else
						submitController.SetUpView ();

					submitController.Window.Level = NSWindowLevel.Floating;
					NSApplication.SharedApplication.RunModalForWindow(submitController.Window);

				//	CommonClass.MainController.Window.AddChildWindow (submitController.Window, NSWindowOrderingMode.Above);
				//submitController.Window.MakeKeyAndOrderFront (this);
				//	submitController.Window.Level = NSWindowLevel.Floating;
				//	NSApplication.SharedApplication.RunModalForWindow (submitController.Window);

				};
			    alert.RunSheetModal (CommonClass.MainController.Window);

			} else {
				if (submitController == null)
					submitController = new SubmitWindowController ();
				else
					submitController.SetUpView ();
				//CommonClass.MainController.Window.AddChildWindow (submitController.Window, NSWindowOrderingMode.Above);
				//submitController.Window.MakeKeyAndOrderFront (this);
				submitController.Window.Level = NSWindowLevel.Floating;
				//submitController.ShowWindow(this);

				NSApplication.SharedApplication.RunModalForWindow (submitController.Window);
			}
		}

		void HandleNewBid (object sender, EventArgs e)
		{

		}

		#endregion
	}

	public class PrintBidReceipt
	{
		public int LineOrder { get; set; }

		public string LineNum { get; set; }

	}

}

