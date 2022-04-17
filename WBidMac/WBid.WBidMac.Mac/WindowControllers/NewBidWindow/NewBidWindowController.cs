
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Model;
using System.Windows;
using System.Text.RegularExpressions;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using System.IO;
using System.ServiceModel;

namespace WBid.WBidMac.Mac
{
	public partial class NewBidWindowController : AppKit.NSWindowController
	{
		List<Domicile> listDomicile;
		List<Position> listPosition;
		List<BidRound> listBidRound;

		List<HistoricalBidData> lstHistoricalData = null;

		BidDetails bidDetail = new BidDetails ();
		NSObject notif;
		List<MonthYear> _monthYearList;
		WBidDataDwonloadAuthServiceClient client;

		public List<Day> LeadOutDays { get; set; }

		public string LastLegArrTime { get; set; }

		#region Constructors

		// Called when created from unmanaged code
		public NewBidWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public NewBidWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public NewBidWindowController () : base ("NewBidWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			listDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy (x => x.DomicileName).ToList ();
			listPosition = WBidCollection.GetPositions ();
			listBidRound = WBidCollection.GetBidRounds ();
		}

		#endregion

		//strongly typed window accessor
		public new NewBidWindow Window {
			get {
				return (NewBidWindow)base.Window;
			}
		}

		static NSButton closeButton;

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib();
				btnRefresh.Hidden = true;
				btnHelp.Hidden = true;

				this.ShouldCascadeWindows = false;
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (senderr, ee) => {
					CloseNewBidWindow ();
				};
				this.Window.ContentView = vwNewBid;
				btnNewBidContinue.Activated += HandleBtnNewBidContinue;
				btnOverlapBack.Activated += HandleBtnOverlapBack;
				btnOverlapContinue.Activated += HandleBtnOverlapContinue;
				btnBlockTimeBack.Activated += HandleBtnBlockTimeBack;
				btnBlockTimeContinue.Activated += HandleBtnBlockTimeContinue;
				btnOverlapSkip.Activated += HandleBtnOverlapContinue;
				btnRefresh.Activated += HandleRefreshButton;
				btnOverlapSkip.Enabled=true;
				btnOverlapContinue.Enabled=false;
				
				foreach (var item in listDomicile) {
					var index = listDomicile.IndexOf (item);
					btnDomicle.CellWithTag (index).Title = item.DomicileName;
                    var button = (NSButtonCell)btnDomicle.CellWithTag(index);
                   
				}
				foreach (var item in listPosition) {
					var index = listPosition.IndexOf (item);
					btnPosition.CellWithTag (index).Title = item.LongStr;
				}
				foreach (var item in listBidRound) {
					var index = listBidRound.IndexOf (item);
					btnRound.CellWithTag (index).Title = item.Round;
				}


				
				//			if (DateTime.Now.Month == 12)
				//				txtYear.StringValue = DateTime.Now.AddYears (1).ToString ();
				//			else
				//				txtYear.StringValue = DateTime.Now.Year.ToString ();
				
				if (GlobalSettings.WbidUserContent != null) {
					btnDomicle.SelectCellWithTag (listDomicile.IndexOf (listDomicile.FirstOrDefault (x => x.DomicileName == GlobalSettings.WbidUserContent.UserInformation.Domicile)));
					btnPosition.SelectCellWithTag (listPosition.IndexOf (listPosition.FirstOrDefault (x => x.LongStr == GlobalSettings.WbidUserContent.UserInformation.Position)));
				}
				
				btnMonth.SelectCellWithTag (DateTime.Now.AddMonths (1).Month);
				bidDetail.Month = (int)btnMonth.SelectedTag;
				SetBidYear ();
				
				if (CommonClass.importLine != null && GlobalSettings.Lines != null) {
					ckImportLine.Title = "Import Line " + CommonClass.importLine.LineNum + "  times from last month";
				}
				
				ckMonthlyOverlap.Activated += (sender, e) => {
					GlobalSettings.IsOverlapCorrection = (ckMonthlyOverlap.State == NSCellStateValue.On);
					if (CommonClass.importLine != null && GlobalSettings.Lines != null) {
						ckImportLine.Enabled = (ckMonthlyOverlap.State == NSCellStateValue.On);
						if (!ckImportLine.Enabled)
						{
							ckImportLine.State = NSCellStateValue.Off;
						}

					}
					btnOverlapSkip.Enabled=false;
					btnOverlapContinue.Enabled=false;
					if (ckMonthlyOverlap.State == NSCellStateValue.On) btnOverlapContinue.Enabled=true;
					else btnOverlapSkip.Enabled=true;
				};
				
				btnMonth.Activated += (object sender, EventArgs e) => {
					SetBidYear ();
					btnNewBidContinue.Enabled = btnMonth.CellWithTag (btnMonth.SelectedTag).Enabled;
				};

				if (GlobalSettings.isHistorical) {
					btnRefresh.Hidden = false;
					btnRefresh.ToolTip = "Update Historical bid data details";
					btnHelp.Hidden = true;
					this.Window.Title = "Retrieve Historical Bid Period";
					txtYear.Hidden = true;
					btnYear.Hidden = false;
					btnYear.CellWithTag (0).Title = (DateTime.Now.Year - 2).ToString ();
					btnYear.CellWithTag (1).Title = (DateTime.Now.Year - 1).ToString ();
					btnYear.CellWithTag (2).Title = DateTime.Now.Year.ToString ();
					btnYear.SelectCellWithTag (2);

					btnYear.Activated += HandleBtnYearClick;
					btnDomicle.Activated += HandleBtnDomicileClick;
					btnRound.Activated += HandleBtnRoundClick;
					btnPosition.Activated += HandleBtnPositionClick;

					if (System.IO.File.Exists (WBidHelper.HistoricalFilesInfoPath)) {
						using (FileStream fileStream = File.OpenRead (WBidHelper.HistoricalFilesInfoPath)) {
							List<HistoricalBidData> historicalBidDataList = new List<HistoricalBidData> ();
							lstHistoricalData = ProtoSerailizer.DeSerializeObject (WBidHelper.HistoricalFilesInfoPath, historicalBidDataList, fileStream);

						}

//						btnYear.Activated+= HandleBtnYearClick;
//						btnDomicle.Activated+=HandleBtnDomicileClick;
//						btnRound.Activated+=HandleBtnRoundClick;
//						btnPosition.Activated+=HandleBtnPositionClick;
						SetUiBasedOnHistoricalList ();
					} else {
                        if (Reachability.CheckVPSAvailable ()) {
							BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
							client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
							client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 30);
							client.GetAvailableHistoricalListCompleted += Client_GetAvailableHistoricalListCompleted;
							;
							client.GetAvailableHistoricalListAsync ();
						}
						
					}



				} else {
					this.Window.Title = "Retrieve New Bid Period";
					txtYear.Hidden = false;
					btnYear.Hidden = true;
				}

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void Client_GetAvailableHistoricalListCompleted (object sender, GetAvailableHistoricalListCompletedEventArgs e)
		{
			try {
				if (e.Result != null) {

					List<WBidDataDownloadAuthorizationService.Model.BidData> lstBid = e.Result.ToList ();
					lstHistoricalData = new List<HistoricalBidData> ();
					foreach (var item in lstBid) {
						lstHistoricalData.Add (new HistoricalBidData {Domicile = item.Domicile, Month = item.Month, Position = item.Position, Round = item.Round
							, Year = item.Year
						});
					}

					var previousfile = Directory.EnumerateFiles(WBidHelper.GetAppDataPath(), "*.HST", SearchOption.AllDirectories).Select(Path.GetFileName);
					if (previousfile != null && previousfile.Count()>0)
					{
						foreach (var item in previousfile)
						{
							File.Delete(WBidHelper.GetAppDataPath() + "/" + item);
						}
					}

					var stream = File.Create (WBidHelper.HistoricalFilesInfoPath);
					ProtoSerailizer.SerializeObject (WBidHelper.HistoricalFilesInfoPath, lstHistoricalData, stream);
					stream.Dispose ();
					stream.Close ();

					BeginInvokeOnMainThread (() => {
						SetUiBasedOnHistoricalList ();
					});

				}
			} catch (Exception ex) {
			}
		}
		
		void HandleBtnYearClick (object sender, EventArgs e)
		{
			SetUiBasedOnHistoricalList ();
		}

		void HandleBtnDomicileClick (object sender, EventArgs e)
		{
			SetUiBasedOnHistoricalList ();
		}

		void HandleBtnPositionClick (object sender, EventArgs e)
		{
			SetUiBasedOnHistoricalList ();
		}

		void HandleBtnRoundClick (object sender, EventArgs e)
		{
			SetUiBasedOnHistoricalList ();
		}


		void SetUiBasedOnHistoricalList ()
		{
			if (GlobalSettings.isHistorical) {
				int year = int.Parse (btnYear.SelectedCell.Title);
				string domicile = btnDomicle.SelectedCell.Title;
				string position = btnPosition.SelectedCell.Title;
				int round = (btnRound.SelectedCell.Title.ToLower () == "monthly") ? 1 : 2;

				if (lstHistoricalData != null) {
					var tempList = lstHistoricalData.Where (x => x.Domicile == domicile && x.Position == position && x.Round == round && x.Year == year).Select (y => y.Month).ToList ();
					if (tempList == null) {
						tempList = new List<int> ();
					}
					for (int count = 1; count <= 12; count++) {
						var month = tempList.FirstOrDefault (x => x == count);
						btnMonth.CellWithTag (count).Enabled = !(month == 0);
						var cell = btnMonth.CellWithTag (count) as NSButtonCell;
						//if (cell.Enabled)
						//	cell.BezelStyle = NSBezelStyle.TexturedRounded;
						//else
						//	cell.BezelStyle = NSBezelStyle.SmallSquare;
					}
					
				}

				btnNewBidContinue.Enabled = btnMonth.CellWithTag (btnMonth.SelectedTag).Enabled;
			}
		}

		void CloseNewBidWindow ()
		{
			this.Window.ContentView = vwNewBid;
			this.Window.Close ();
			this.Window.OrderOut (this);
			NSApplication.SharedApplication.StopModal ();
		}

		void HandleBtnBlockTimeContinue (object sender, EventArgs e)
		{

			
			try {
				foreach (var vw in ((NSView)bxBlockTime.ContentView).Subviews) {
					var txt = (vw.Identifier == "label") ? (NSTextField)vw : null;
					var dp = (vw.Identifier == "date") ? (NSDatePicker)vw : null;
					if (dp != null) {
						LeadOutDays [(int)dp.Tag].FlightTimeHour = dp.DateValue.LocalValue ();
					}
				}
				LeadOutDays.ToList ().ForEach (x => x.FlightTime = Helper.ConvertHhmmToMinutes (x.FlightTimeHour));
				GlobalSettings.LeadOutDays = LeadOutDays.ToList ();
				GlobalSettings.LastLegArrivalTime = Helper.ConvertHhmmToMinutes (LastLegArrTime);
				GlobalSettings.IsOverlapCorrection = (btnOverlapChoice.SelectedTag == 1) ? true : false;
				
				LoginViewController login = new LoginViewController ();
				var panel = new NSPanel ();
				CommonClass.Panel = panel;
				panel.SetContentSize (new CoreGraphics.CGSize (450, 250));
				panel.ContentView = login.View;
				NSApplication.SharedApplication.BeginSheet (panel, this.Window);
				notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"LoginSuccess", HandleLoginSuccess);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleBtnBlockTimeBack (object sender, EventArgs e)
		{
			this.Window.ContentView = vwOverlap;
		}
		void HandleRefreshButton(object sender, EventArgs e)
		{
			if (Reachability.CheckVPSAvailable())
			{
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
				client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
				client.GetAvailableHistoricalListCompleted += Client_GetAvailableHistoricalListCompleted;
				;
				client.GetAvailableHistoricalListAsync();
			}
		}
		void HandleBtnOverlapContinue (object sender, EventArgs e)
		{
			try {
				if (GlobalSettings.DownloadBidDetails.Round == "B" && (GlobalSettings.DownloadBidDetails.Postion == "CP" || GlobalSettings.DownloadBidDetails.Postion == "FO")) {
					GlobalSettings.IsOverlapCorrection = false;
					ckMonthlyOverlap.State = NSCellStateValue.Off;
					LoginViewController login = new LoginViewController ();
					var panel = new NSPanel ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (450, 250));
					panel.ContentView = login.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"LoginSuccess", HandleLoginSuccess);
				} else if (ckMonthlyOverlap.State == NSCellStateValue.On) {
					this.Window.ContentView = viewOverLapAlert;
				} else {
					GlobalSettings.IsOverlapCorrection = false;
					LoginViewController login = new LoginViewController ();
					var panel = new NSPanel ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (450, 250));
					panel.ContentView = login.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"LoginSuccess", HandleLoginSuccess);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		partial void btnOverlapAlertTapped(NSObject sender)
		{
			GlobalSettings.IsOverlapCorrection = true;
                    LoadBlockView();
					this.Window.ContentView = vwBlockTime;
		} 
		void LoadBlockView ()
		{
			GetDates ();

			if (ckImportLine.State == NSCellStateValue.On) {
				//get the selected line
				if (CommonClass.importLine != null) {
					GetBlockTime (CommonClass.importLine);
					SetlastLegArrivalTime (CommonClass.importLine);
				}
			} 

			foreach (var vw in ((NSView)bxBlockTime.ContentView).Subviews) {
				var txt = (vw.Identifier == "label") ? (NSTextField)vw : null;
				var dp = (vw.Identifier == "date") ? (NSDatePicker)vw : null;
				if (txt != null) {
					txt.StringValue = LeadOutDays [(int)txt.Tag].Date.ToString ("MMM dd :");
				}
				if (dp != null) {
					dp.Locale = new NSLocale ("NL");
					dp.TimeZone = NSTimeZone.LocalTimeZone;
					dp.Calendar = NSCalendar.CurrentCalendar;
					dp.DateValue = DateTime.Parse (LeadOutDays [(int)dp.Tag].FlightTimeHour).DateTimeToNSDate();
				}
			}
			dpMinRest.Locale = new NSLocale ("NL");
			dpMinRest.TimeZone = NSTimeZone.LocalTimeZone;
			dpMinRest.Calendar = NSCalendar.CurrentCalendar;
			if (LastLegArrTime == null)
				LastLegArrTime = "00:00";
			dpMinRest.DateValue = DateTime.Parse (LastLegArrTime).DateTimeToNSDate();
			dpMinRest.Activated += delegate {
				LastLegArrTime = dpMinRest.DateValue.LocalValue ();
			};

		}

		void HandleBtnOverlapBack (object sender, EventArgs e)
		{
			this.Window.ContentView = vwNewBid;
		}

		void HandleLoginSuccess (NSNotification obj)
		{
			try {
				NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
				if (obj.Object.ToString () == "Login") {
					//GlobalSettings.DownloadBidDetails = bidDetail;
					CloseNewBidWindow ();
					NSNotificationCenter.DefaultCenter.PostNotificationName ("StartBidDownload", new NSString ("Start"));
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleBtnNewBidContinue (object sender, EventArgs e)
		{
			try {


				bidDetail.Domicile = listDomicile.ElementAt ((int)btnDomicle.SelectedTag).DomicileName;
				bidDetail.Postion = listPosition.ElementAt ((int)btnPosition.SelectedTag).LongStr;
				if (btnRound.SelectedTag == 0)
					bidDetail.Round = "D";
				else
					bidDetail.Round = "B";
				bidDetail.Month = (int)btnMonth.SelectedTag;
				//bidDetail.Year = DateTime.Now.Month == 12 ? DateTime.Now.AddYears(1).Year : DateTime.Now.Year;
				SetBidYear ();
				GlobalSettings.DownloadBidDetails = bidDetail;
				if (!GlobalSettings.isHistorical) 
				{
					this.Window.ContentView = vwOverlap;
					if (GlobalSettings.DownloadBidDetails.Round == "B" && (GlobalSettings.DownloadBidDetails.Postion == "CP" || GlobalSettings.DownloadBidDetails.Postion == "FO")) {
						bxFirstRnd.Hidden = true;
						bxSecondRnd.Hidden = false;
					} else {
						bxFirstRnd.Hidden = false;
						bxSecondRnd.Hidden = true;
					}
					ShowEarlyBidWarning();
//				if(GlobalSettings.DownloadBidDetails.Round=="D"){
//					bxFirstRnd.Hidden = false;
//					bxSecondRnd.Hidden = true;
//				} else {
//					bxFirstRnd.Hidden = true;
//					bxSecondRnd.Hidden = false;
//				}
				} else {
					GlobalSettings.IsOverlapCorrection = false;
					ckMonthlyOverlap.State = NSCellStateValue.Off;
					LoginViewController login = new LoginViewController ();
					var panel = new NSPanel ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (450, 250));
					panel.ContentView = login.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"LoginSuccess", HandleLoginSuccess);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		private void ShowEarlyBidWarning()
		{
			DateTime utctime= TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.Local);
			string date = string.Empty;
			if (GlobalSettings.DownloadBidDetails.Postion == "CP" || GlobalSettings.DownloadBidDetails.Postion == "FO````````````````````````````````````````")
			{ 
				if(GlobalSettings.DownloadBidDetails.Round=="D")
				{
					if(utctime < new DateTime(utctime.Year,utctime.Month,4,12,0,0))
						date = " 4th ";
				}
				else
				{
					if (utctime < new DateTime(utctime.Year, utctime.Month, 17, 12, 0, 0))
						date = " 17th ";
				}
			}
			else if (GlobalSettings.DownloadBidDetails.Postion == "FA")
			{
				if (GlobalSettings.DownloadBidDetails.Round == "D")
				{
					if (utctime < new DateTime(utctime.Year, utctime.Month, 2, 12, 0, 0))
						date = " 2nd ";
				}
				else
				{
					if (utctime < new DateTime(utctime.Year, utctime.Month, 12, 12, 0, 0))
						date = " 12th ";
				}
			}
			if (date != string.Empty)
			{
				var message="SWA guarantees that the lines will be realeased by noon Central Time on the" +date+".\n\nSometimes SWA releases the lines earlier. If SWA has not released the lines early,then attempting to downlaod them now will result in a BID INFO UNAVAILABLE error.\n\nSo If you receive this error,try again later ";
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.MessageText = "Early Bid Warning";
				alert.InformativeText = message;
				alert.AddButton ("OK");
//				((NSButton)alert.Buttons [0]).Activated += (sender, e) => {
//					DismissCurrentView ();
//				};
				alert.BeginSheet (this.Window);


//				alert.AlertStyle = NSAlertStyle.Warning;
//				alert.Window.Title = "WBidMax";
//				alert.MessageText = "Early Bid Warning";
//				alert.InformativeText = message;
//				alert.AddButton ("OK");
//				alert.RunModal ();
//				NSApplication.SharedApplication.EndSheet (overlayPanel);
//				overlayPanel.OrderOut (this);
				//Xceed.Wpf.Toolkit.MessageBox.Show(message, "Early Bid Warning", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}
		void DismissCurrentView ()
		{
			//CommonClass.Window.Close ();
			//CommonClass.DownloadController.Window.OrderOut (this);

			NSApplication.SharedApplication.StopModal ();
		}
		private void GenerateYaerOfEachMonth ()
		{
			//month list is hold the year  of each month 
			try {
				_monthYearList = new List<MonthYear> ();
				MonthYear monthYear = null;
				
				for (int month = 1; month <= 12; month++) {
					_monthYearList.Add (new MonthYear () { Month = month });
				
				}
				
				
				if (DateTime.Now.Month < 6) {
					int previousMonth = (DateTime.Now.Month == 1) ? 12 : DateTime.Now.Month - 1;
					for (int count = 1; count <= 5; count++) {
						monthYear = _monthYearList.FirstOrDefault (x => x.Month == previousMonth);
						monthYear.Year = (previousMonth > DateTime.Now.Month) ? DateTime.Now.Year - 1 : DateTime.Now.Year;
						previousMonth = (previousMonth == 1) ? 12 : previousMonth - 1;
				
					}
				} else if (DateTime.Now.Month > 6) {
					int nextMonth = (DateTime.Now.Month == 12) ? 1 : DateTime.Now.Month + 1;
					for (int count = 1; count <= 6; count++) {
						monthYear = _monthYearList.FirstOrDefault (x => x.Month == nextMonth);
						monthYear.Year = (nextMonth < DateTime.Now.Month) ? DateTime.Now.Year + 1 : DateTime.Now.Year;
						nextMonth = (nextMonth == 12) ? 1 : nextMonth + 1;
				
					}
				}
				
				for (int month = 1; month <= 12; month++) {
					monthYear = _monthYearList.FirstOrDefault (x => x.Month == month);
					if (monthYear.Year == 0) {
						monthYear.Year = DateTime.Now.Year;
				
					}
				
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


		private void SetBidYear ()
		{

			if (GlobalSettings.isHistorical) {
				bidDetail.Year = int.Parse (btnYear.SelectedCell.Title);
			} else {
				if (_monthYearList == null) {
					GenerateYaerOfEachMonth ();
				}
				bidDetail.Year = _monthYearList.FirstOrDefault (x => x.Month == bidDetail.Month).Year;
				txtYear.StringValue = bidDetail.Year.ToString ();
			}

		}


		public class MonthYear
		{
			public int Month { get; set; }

			public int Year { get; set; }
		}

		#region Private Methods

		/// <summary>
		/// Get the dates for the overlap (add last 6 days from the current month and first 6 days  from the next month to the list)
		/// </summary>
		private void GetDates ()
		{
			//Get the end of the current bid month.
			DateTime date = new DateTime (GlobalSettings.DownloadBidDetails.Year, GlobalSettings.DownloadBidDetails.Month, 1).AddDays (-1);
			//  DateTime date = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).AddMonths(1).AddDays(-1);

			LeadOutDays = new List<Day> ();

			//add last 6 days from the current month and first 6 days  from the next month to the list
			Day day;
			date = date.AddDays (-5);
			for (int count = 0; count <= 11; count++) {
				day = new Day ();
				day.Date = date;
				day.FlightTimeHour = "00:00";
				date = date.AddDays (1);
				LeadOutDays.Add (day);

			}
		}

		/// <summary>
		/// set the arrival time for the last leg in the duty period
		/// </summary>
		/// <param name="selectedline"></param>
		private void SetlastLegArrivalTime (Line selectedline)
		{
			if (selectedline != null && selectedline.Pairings.Count > 0) {
				Trip trip = GlobalSettings.Trip.Where (x => x.TripNum == selectedline.Pairings [selectedline.Pairings.Count - 1].Substring (0, 4)).FirstOrDefault ();
				if (trip == null) {
					trip = GlobalSettings.Trip.Where (x => x.TripNum == selectedline.Pairings [selectedline.Pairings.Count - 1]).FirstOrDefault ();
				}
				if (trip != null && trip.DutyPeriods.Count > 0) {
					var lastpairingdutyperiods = trip.DutyPeriods;

					LastLegArrTime = Helper.ConvertMinutesToFormattedHour (lastpairingdutyperiods [lastpairingdutyperiods.Count - 1].LandTimeLastLeg - ((lastpairingdutyperiods.Count - 1) * 1440));
				}
			}
		}

		/// <summary>
		/// get the block time for the days
		/// </summary>
		/// <param name="selectedline"></param>
		private void GetBlockTime (Line selectedline)
		{
			bool isLastTrip = false;
			int paringCount = 0;
			///iterate through all the pairings in the selected line
			foreach (var pairing in selectedline.Pairings) {

				//start date of the trip
				int pairingstartdate = Convert.ToInt32 (pairing.Substring (4, 2));

				//get the duty period for the pairings
				List<DutyPeriod> dutyperiod = GlobalSettings.Trip.Where (x => x.TripNum.StartsWith (pairing.Substring (0, 4))).ToList ().FirstOrDefault ().DutyPeriods;
				int midnightimeconvert = 0;
				for (int count = 0; count < dutyperiod.Count; count++) {
					isLastTrip = ((selectedline.Pairings.Count - 1) == paringCount);
					paringCount++;
					DateTime newdate = WBidCollection.SetDate (pairingstartdate, isLastTrip);
					newdate = newdate.AddDays (count);

					if (LeadOutDays.Any (x => x.Date == newdate)) {
						Day objday = LeadOutDays.Where (x => x.Date == newdate).FirstOrDefault ();
						objday.FlightTime = dutyperiod [count].Block;
						objday.FlightTimeHour = Helper.ConvertMinutesToFormattedHour (dutyperiod [count].Block);
						objday.DepartutreTime = dutyperiod [count].DepTimeFirstLeg - midnightimeconvert;
						objday.ArrivalTime = dutyperiod [count].LandTimeLastLeg - midnightimeconvert;
					}
					midnightimeconvert += 1440;
				}

			}
		}

		#endregion


	}


}

