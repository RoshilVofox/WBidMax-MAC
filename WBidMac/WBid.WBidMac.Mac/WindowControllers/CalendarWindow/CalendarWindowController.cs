using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using CoreGraphics;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.PortableLibrary;
using System.Globalization;
using WebKit;

namespace WBid.WBidMac.Mac
{
	public partial class CalendarWindowController : AppKit.NSWindowController
	{
		TripWindowController TripWC;
		//NSAlert printAlert;
		public bool toPrint;

		#region Constructors

		// Called when created from unmanaged code
		public CalendarWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public CalendarWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public CalendarWindowController () : base ("CalendarWindow")
		{
			Initialize ();
		}
		partial void btnExportOutlookClicked (NSObject sender)
		{
			AppointmentDetails apointments=ExportLinesToOutlook();


		}
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new CalendarWindow Window {
			get {
				return (CalendarWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				btnFFDO.Hidden = !(GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO");
				this.Window.WillClose += delegate {
					if (this.Window.ChildWindows != null) {
						foreach (var item in this.Window.ChildWindows) {
							item.Close ();
						}
					}
				};
				btnMoveUp.Activated += (object sender, EventArgs e) => {
					CommonClass.MainController.MoveLineUp ();
				};
				btnMoveDown.Activated += (object sender, EventArgs e) => {
					CommonClass.MainController.MoveLineDown ();
				};
				btnTopLock.Activated += (object sender, EventArgs e) => {
					CommonClass.MainController.TopLock ();
					CommonClass.MainController.MoveLineDown ();
				};
				btnBottomLock.Activated += (object sender, EventArgs e) => {
					CommonClass.MainController.BottomLock ();
					var line = GlobalSettings.Lines.First (x => x.LineNum == CommonClass.selectedLine);
					if (line.BotLock)
						CommonClass.MainController.MoveLineUp ();
					else
						CommonClass.SummaryController.LoadContent ();
				};
				btnPrint.Activated += btnPrintClicked;
				btnFFDO.Activated += btnFFDOClicked;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void btnFFDOClicked (object sender, EventArgs e)
		{
			try {
				string result = GenarateFFDOforLine ();
				NSPasteboard clipBoard = NSPasteboard.GeneralPasteboard;
				clipBoard.DeclareTypes (new string[]{ "NSStringPboardType" }, null);
				clipBoard.SetStringForType (result, "NSStringPboardType");
				var alert = new NSAlert ();
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Trips has been copied to ClipBoard!";
				alert.RunModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		/// <summary>
		/// Genarate FFDO Data for a line
		/// </summary>
		/// <returns></returns>
		private string GenarateFFDOforLine ()
		{
			string result = string.Empty;
			Line selectedline = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine);
			if (selectedline != null) {
				bool isLastTrip = false;
				int paringCount = 0;
				foreach (string pairing in selectedline.Pairings) {
					isLastTrip = ((selectedline.Pairings.Count - 1) == paringCount);
					paringCount++;
					result += GetFlightDataforFFDB (pairing, isLastTrip);
				}
			}
			return result;
		}

		/// <summary>
		/// PURPOSE : Get Flight data for FFDB
		/// </summary>
		/// <param name="trip"></param>
		/// <param name="tripName"></param>
		private string GetFlightDataforFFDB (string tripNum, bool isLastTrip)
		{

			Trip trip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == tripNum.Substring (0, 4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == tripNum);
			string result = string.Empty;

			// var tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(SelectedDay.Replace(" ", "")));
			DateTime dutPeriodDate = WBidCollection.SetDate (int.Parse (tripNum.Substring (4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
			//DateTime dutPeriodDate = SelectedTripStartDate;

			foreach (var dp in trip.DutyPeriods) {
				string datestring = dutPeriodDate.ToString ("MM'/'dd'/'yyyy");
				if (trip.ReserveTrip) {

					result += datestring + "  RSRV " + trip.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight (Convert.ToString (dp.ReserveOut % 1440)).Replace (":", "") + " " + trip.RetSta + " " + Helper.CalcTimeFromMinutesFromMidnight (Convert.ToString (dp.ReserveIn % 1440)).Replace (":", "") + " \n";
				} else {
					foreach (var flt in dp.Flights) {
						result += datestring + " " + flt.FltNum.ToString ().PadLeft (4, '0') + " " + flt.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight (flt.DepTime.ToString ()).Replace (":", "") + " " + flt.ArrSta + " " + Helper.CalcTimeFromMinutesFromMidnight (flt.ArrTime.ToString ()).Replace (":", "") + " \n";
					}
				}
				dutPeriodDate = dutPeriodDate.AddDays (1);
			}
			return result;
		}

		void btnPrintClicked (object sender, EventArgs e)
		{
			try {
//				printAlert = new NSAlert();
//				printAlert.MessageText = "Please choose the options";
//				printAlert.AddButton("Cancel");
//				printAlert.AddButton("Calendar View");
//				printAlert.AddButton("BidLine with All Pairings");
//				printAlert.Buttons[0].Activated += delegate {
//					NSApplication.SharedApplication.EndSheet(printAlert.Window);
//					printAlert.Window.OrderOut(this);
//				};
//				printAlert.Buttons[1].Activated += delegate {
//					bxCal.AddSubview(bxLineLayer);
//					NSApplication.SharedApplication.EndSheet(printAlert.Window);
//					printAlert.Window.OrderOut(this);
//					var op = NSPrintOperation.FromView((NSView)bxCal.ContentView);
//					op.RunOperation();
//					bxLineLayer.RemoveFromSuperview();
//				};
//				printAlert.Buttons[2].Activated += delegate {
//					NSApplication.SharedApplication.EndSheet(printAlert.Window);
//					printAlert.Window.OrderOut(this);
//					var str = CommonClass.PrintAllPairings ();
//					var inv = new InvisibleWindowController ();
//					this.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
//					var txt = new NSTextView (new RectangleF (0, 0, 550, 550));
//					txt.Font = NSFont.FromFontName ("Courier New", 6);
//					inv.Window.ContentView.AddSubview (txt);
//					txt.Value = str;
//					var pr = NSPrintInfo.SharedPrintInfo;
//					pr.VerticallyCentered = false;
//					pr.TopMargin = 2.0f;
//					pr.BottomMargin = 2.0f;
//					pr.LeftMargin = 1.0f;
//					pr.RightMargin = 1.0f;
//					txt.Print (this);
//					inv.Close ();
//				};
//				printAlert.BeginSheet(this.Window);
//				toPrint = true;
//				bxCal.AddSubview (bxLineLayer);
////				NSApplication.SharedApplication.EndSheet(printAlert.Window);
////				printAlert.Window.OrderOut(this);
//				LoadContent ();
//				this.PerformSelector (new MonoMac.ObjCRuntime.Selector ("PrintView"), null, 0.5);
				//PrintView ();
				//bxLineLayer.RemoveFromSuperview();
				//toPrint = false;
				//LoadContent();

				CalFormatPrint();

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void CalFormatPrint ()
		{
			var content = string.Empty;
			var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine);

			string heading = string.Empty;
			//May-2015-BWI CP Line14

			if (GlobalSettings.CurrentBidDetails != null) {
				heading = heading + 
					CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(GlobalSettings.CurrentBidDetails.Month) + " " + GlobalSettings.CurrentBidDetails.Domicile + " " + GlobalSettings.CurrentBidDetails.Postion;
			}
			if (line != null) {
				heading += " Line " + line.LineNum;
			}

			string InitalContent = "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">\r\n<html>\r\n<head>\r\n  <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\r\n  <meta http-equiv=\"Content-Style-Type\" content=\"text/css\">\r\n  <title></title>\r\n  <meta name=\"Generator\" content=\"Cocoa HTML Writer\">\r\n  <meta name=\"CocoaVersion\" content=\"1347.57\">\r\n  <style type=\"text/css\">\r\n    p.p1 {margin: 0.0px 0.0px 1.0px 0.0px; text-align: center; font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n    p.p2 {margin: 0.0px 0.0px 1.0px 0.0px;  font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n  p.p4 {margin: 0.0px 0.0px 1.0px 0.0px;  font: 12.0px Courier; color: #2e3e44; -webkit-text-stroke: #2e3e44}\r\n    p.p3 {margin: 0.0px 0.0px 1.0px 0.0px; text-align: center; font: 12.0px Arial; color: #2e3e44; -webkit-text-stroke: #2e3e44; min-height: 14.0px}\r\n    span.s1 {font-kerning: none; }\r\n    table.t1 {border-style: solid; border-width: 0.5px 0.5px 0.5px 0.5px; border-color: #cbcbcb ; border-collapse: collapse}\r\n    table.t2 {border-style: solid; border-width: 0.5px 0.5px 0.5px 0.5px; border-color: #cbcbcb ; border-collapse: collapse}\r\n    td.td1 {width: 70.0px; border-style: solid; border-width: 1px ; border-color:  #979797; padding:5px; }\r\n    td.td2 {width: 10.0px; border-style: solid; border-width: 0.0px 1.0px 1.0px 1.0px; border-color: #979797;}\r\n\t td.td3 { border:solid 1px #979797; border-right:none; border-top:none; width:20px; text-align:center;}\r\n\t .heading{ font-family:Arial, Helvetica, sans-serif; font-weight:bold; text-align:center;}\r\n  </style>\r\n</head>\r\n<body>\r\n    <table cellspacing=\"0\" cellpadding=\"0\" class=\"t1\" align =\"Center\">\r\n <tr>\r\n      <td valign=\"top\" colspan=\"7\" class=\"td1 heading\">"+heading+"</td>\r\n\t  <tr>\r\n  <tbody>\r\n   \r\n    <tr>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>SUN</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>MON</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>TUE</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>WED</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>THU</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>FRI</b></span></p>\r\n      </td>\r\n      <td valign=\"top\" class=\"td1\">\r\n        <p class=\"p1\"><span class=\"s1\"><b>SAT</b></span></p>\r\n      </td>\r\n    </tr>\r\n    <tr>";


			string DynamicDays = string.Empty;
			string saparator =	" </tr>\n    <tr>";
			for (int i = 0; i < CommonClass.calData.Count; i++) {
				if (i % 7 == 0) {
					DynamicDays += saparator;
				}

				string DepTimeFirstLeg = CommonClass.calData [i].DepTimeFirstLeg;
				string ArrStaLastLeg = CommonClass.calData [i].ArrStaLastLeg;
				string LandTimeLastLeg = CommonClass.calData [i].LandTimeLastLeg;
				if (string.IsNullOrEmpty (DepTimeFirstLeg))
					DepTimeFirstLeg = "    ";

				if (string.IsNullOrEmpty (ArrStaLastLeg))
					ArrStaLastLeg = "    ";


				if (string.IsNullOrEmpty (LandTimeLastLeg))
					LandTimeLastLeg = "    ";

				if (string.IsNullOrEmpty (CommonClass.calData [i].Day)) {
					DynamicDays += "<td valign=\"top\" class=\"td2\">\r\n       \r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n<p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n        <p class=\"p3\">" + DepTimeFirstLeg + "</p>\r\n        <p class=\"p3\">" + ArrStaLastLeg + "</p>\r\n        <p class=\"p3\">" + LandTimeLastLeg + "</p>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n      </td>";
				} else
					DynamicDays += "<td valign=\"top\" class=\"td2\">\r\n        <table cellspacing=\"0\" cellpadding=\"0\" class=\"t2\" align=\"right\">\r\n          <tbody>\r\n            <tr>\r\n              <td valign=\"middle\" class=\"td3\">\r\n                <p class=\"p2\"><span class=\"s1\"><b>" + CommonClass.calData [i].Day + "</b></span></p>\r\n              </td>\r\n            </tr>\r\n          </tbody>\r\n        </table>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n<p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n        <p class=\"p3\">" + DepTimeFirstLeg + "</p>\r\n        <p class=\"p3\">" + ArrStaLastLeg + "</p>\r\n        <p class=\"p3\">" + LandTimeLastLeg + "</p>\r\n        <p class=\"p2\"><span class=\"s1\"><br>\r\n</span></p>\r\n      </td>";
			}

			DynamicDays += " </tr>\n    \r\n  </tbody>";
			string hotels = "<br/>";
			var hotelLst = 	CalendarViewBL.GenerateCalendarAndHotelDetails (line);
			foreach (var hot in hotelLst) {
				hotels += hot+"<br/>";
			}
			string holidayDetails = "<tr>\r\n              <td valign=\"top\" colspan=\"7\" class=\"td4\"> <p class=\"p4\">"+hotels+"</p></td>\r\n              <tr>";
			string FinalContent = "</table>\r\n\r\n</body>\r\n</html>";

			content = InitalContent + DynamicDays + holidayDetails + FinalContent;
			var web = new WebView ();
			var inv = new InvisibleWindowController ();
			this.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
			inv.Window.ContentView.AddSubview (web);
			var pr = NSPrintInfo.SharedPrintInfo;
			pr.VerticallyCentered = false;
			pr.TopMargin = 2.0f;
			pr.BottomMargin = 2.0f;
			pr.LeftMargin = 1.0f;
			pr.RightMargin = 1.0f;
			web.MainFrame.LoadHtmlString (content,null);
			web.OnFinishedLoading += delegate {
				web.MainFrame.FrameView.DocumentView.Print(this);
				inv.Close ();
			};
		}

//		[Export ("PrintView")]
//		void PrintView ()
//		{
//			var frm = bxCal.Frame;
//			bxCal.Frame = new RectangleF (frm.Location, new SizeF (frm.Size.Width, frm.Size.Height + 25));
//			((NSView)bxCal.ContentView).AddSubview (lblPrintTitle);
//			lblPrintTitle.Frame = new RectangleF (new PointF (0, bxCal.Frame.Size.Height - 25), lblPrintTitle.Frame.Size);
//			lblPrintTitle.StringValue = SetPrintTitle ();
//			var op = NSPrintOperation.FromView ((NSView)bxCal.ContentView);
//			op.RunOperation ();
//			bxLineLayer.RemoveFromSuperview ();
//			lblPrintTitle.RemoveFromSuperview ();
//			bxCal.Frame = frm;
//			toPrint = false;
//			LoadContent ();
//		}

		private string SetPrintTitle ()
		{
			var title = string.Empty;
			System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo ();
			string strMonthName = mfi.GetMonthName (GlobalSettings.CurrentBidDetails.Month);
			title += strMonthName + " ";
			title += GlobalSettings.CurrentBidDetails.Year.ToString () + " - ";
			title += GlobalSettings.CurrentBidDetails.Domicile + " ";
			title += GlobalSettings.CurrentBidDetails.Postion + " ";
			title += "Line " + CommonClass.selectedLine.ToString ();
			return title;
		}

		public void btnCalendarClicked (object sender)
		{
			
			try {
				//Console.WriteLine (((NSButton)sender).Title);
				var index = ((NSButton)sender).Tag;
				var trip = CommonClass.calData.FirstOrDefault (x => x.Id == index).TripNumber;
				var isLast = CommonClass.calData.FirstOrDefault (x => x.Id == index).IsLastTrip;
				if (!string.IsNullOrEmpty (trip)) {
					if (TripWC == null)
						TripWC = new TripWindowController ();
					TripWC.tripNum = trip;
					CorrectionParams correctionParams = new WBid.WBidiPad.Model.CorrectionParams ();
					correctionParams.selectedLineNum = CommonClass.selectedLine;
					CommonClass.tripData = TripViewBL.GenerateTripDetails (trip, correctionParams, isLast);
					TripWC.LoadContent ();
					CommonClass.selectedTrip = trip;
					CommonClass.isLastTrip = CommonClass.calData [(int)index].IsLastTrip;
					CommonClass.CalendarController.LoadContent ();
					this.Window.AddChildWindow (TripWC.Window, NSWindowOrderingMode.Above);
					TripWC.Window.MakeKeyAndOrderFront (this);
					//NSApplication.SharedApplication.RunModalForWindow (TripWC.Window);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void LoadContent ()
		{
//			if (printAlert != null && printAlert.Window.IsSheet) {
//				NSApplication.SharedApplication.EndSheet(printAlert.Window);
//				printAlert.Window.OrderOut(this);
//			}
			var line = GlobalSettings.Lines.First (x => x.LineNum == CommonClass.selectedLine);
			if (GlobalSettings.Lines.IndexOf (line) == 0)
				btnMoveUp.Enabled = false;
			else
				btnMoveUp.Enabled = true;
			if (GlobalSettings.Lines.IndexOf (line) == GlobalSettings.Lines.Count - 1)
				btnMoveDown.Enabled = false;
			else
				btnMoveDown.Enabled = true;

			btnTopLock.Enabled = !line.TopLock;
			btnBottomLock.Enabled = !line.BotLock;

			this.Window.Title = "Line " + GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine).LineDisplay;

			vwCalendar.ItemPrototype = new CalendarCellViewItem ();
			var lstCal = CommonClass.calData.ToList ().ConvertAll (x => new NSString (x.Id.ToString ()));
			vwCalendar.Content = lstCal.ToArray ();

//			var objects = arrCalendarList.ArrangedObjects ();
//			arrCalendarList.Remove (objects);
//			int index = 0;
//			foreach (var item in CommonClass.calData) {
//				var data = new CalendarClass ();
//				data.Day = item.Day;
//				data.DepTimeFirst = item.DepTimeFirstLeg;
//				data.ArrLastLeg = item.ArrStaLastLeg;
//				data.ArrTimeLast = item.LandTimeLastLeg;
//				data.ID = item.Id.ToString ();
//				data.Index = index.ToString ();
//				arrCalendarList.AddObject (data);
//				index++;
//			}

		}
		private AppointmentDetails ExportLinesToOutlook()
		{
			var line = GlobalSettings.Lines.First (x => x.LineNum == CommonClass.selectedLine);

			DateTime tripStartDate;
			ExportPairingDetails exportPairingDetails = new ExportPairingDetails();
			int paringCount = 0;
			AppointmentDetails apointments = new AppointmentDetails ();
			foreach (var pairing in line.Pairings)
			{
				
				Trip trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();

				if (trip == null)
				{
					trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
				}

				bool isLastTrip = (line.Pairings.LastOrDefault() == pairing);
				tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
				CorrectionParams correctionParams = new WBid.WBidiPad.Model.CorrectionParams ();
				correctionParams.selectedLineNum = line.LineNum;
				var datas = TripViewBL.GenerateTripDetails (pairing, correctionParams, false);
				var content = string.Empty;
				foreach (TripData data in datas) {
					content += "<br />";
					content += data.Content;
				}

				ExportPairingDetails exportdetails = new ExportPairingDetails ();
				 apointments = exportdetails.ExportToOutlook (trip.TripNum,tripStartDate, content);
				ExportToOutllok (apointments,trip.TripNum.Substring(0,4));
				//here we need to include the export to outlook scripts.\also show message on each pairing exported

				//MessageBox.Show("Pairing " + tripName + " starting on " + appointmentDetails.StartDate + " exported to outlook as an Appointment", "Pairing exported to Outlook", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			return apointments;
		}
		public void ExportToOutllok(AppointmentDetails apointments, string TripName)
		{
			string MainScript=@"tell application ""Microsoft Outlook""
				
				set theSubject to ""vSubject""
				set theContent to ""vContent""
				set StartSate to current date
				set day of StartSate to vStartDay -- important
				set month of StartSate to vStartMonth
				set year of StartSate to vStartYear
				set hours of StartSate to vStartHour
				set minutes of StartSate to vStartMinutes
				set seconds of StartSate to vStartSeconds
				
				
				set EndDate to current date
				set day of EndDate to vEndDay -- important
				set month of EndDate to vEndMonth
				set year of EndDate to vEndYear
				set hours of EndDate to vEndHour
				set minutes of EndDate to vEndMinutes
				set seconds of EndDate to vEndSeconds
				
				
				set newAppointment to make new calendar event with properties {subject:theSubject, content:theContent, start time:StartSate, end time:EndDate}
				
				
				
			end tell";


			string replaced = MainScript.Replace (@"vSubject", apointments.Subject).Replace (@"vContent", apointments.Body).Replace (@"vStartDay",apointments.StartDateTime.Day.ToString()).Replace (@"vStartMonth", apointments.StartDateTime.Month.ToString()).Replace (@"vStartYear", apointments.StartDateTime.Year.ToString()).Replace (@"vStartHour",apointments.StartDateTime.Hour.ToString()).Replace (@"vStartMinutes", apointments.StartDateTime.Minute.ToString()).Replace ("vStartSeconds",  apointments.StartDateTime.Second.ToString()).Replace (@"vEndDay", apointments.EndDateTime.Day.ToString()).Replace (@"vEndMonth", apointments.EndDateTime.Month.ToString()).Replace (@"vEndYear", apointments.EndDateTime.Year.ToString()).Replace (@"vEndHour", apointments.EndDateTime.Hour.ToString()).Replace (@"vEndMinutes",apointments.EndDateTime.Minute.ToString()).Replace (@"vEndSeconds", apointments.EndDateTime.Second.ToString());
			Console.WriteLine (replaced);


			NSAppleScript scr = new NSAppleScript (replaced);
			NSAppleEventDescriptor eventss=new NSAppleEventDescriptor();
			NSDictionary dic = new NSDictionary ();
			eventss = scr.ExecuteAndReturnError (out dic);
			if (eventss != null)
			{
				Console.WriteLine ("Done");
				var alert = new NSAlert ();
				alert.MessageText = "Pairing exported to Outlook";
				alert.InformativeText = "Pairing " + TripName + " starting on " + apointments.StartDate + " exported to outlook as an Appointment";
				alert.RunModal ();

			}
			else
			{
				Console.WriteLine ("failed");
				var alert = new NSAlert ();
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Trips failed to export Outlook";
				alert.RunModal ();
			}
		}
	}


	public class CalendarCellViewItem : NSCollectionViewItem
	{
		private static readonly NSString EMPTY_NSSTRING = new NSString (string.Empty);
		private CalendarView view;

		public CalendarCellViewItem () : base ()
		{

		}

		public CalendarCellViewItem (IntPtr ptr) : base (ptr)
		{

		}

		public override void LoadView ()
		{
			view = new CalendarView ();
			View = view;
		}

		public override NSObject RepresentedObject {
			get { return base.RepresentedObject; }

			set {
				if (value == null) {
					// Need to do this because setting RepresentedObject in base to null 
					// throws an exception because of the MonoMac generated wrappers,
					// and even though we don't have any null values, this will get 
					// called during initialization of the content with a null value.
					base.RepresentedObject = EMPTY_NSSTRING;

				} else {
					base.RepresentedObject = value;
					var Id = (NSString)value;
					view.Button.Tag = int.Parse (Id);
					var calItem = CommonClass.calData.FirstOrDefault (x => x.Id == int.Parse (Id));
					view.DayView.StringValue = calItem.Day;
					view.DepTimeFirstView.StringValue = (calItem.DepTimeFirstLeg != null) ? calItem.DepTimeFirstLeg : "";
					view.ArrLastLegView.StringValue = (calItem.ArrStaLastLeg != null) ? calItem.ArrStaLastLeg : "";
					view.ArrTimeLastView.StringValue = (calItem.LandTimeLastLeg != null) ? calItem.LandTimeLastLeg : "";

					if (!CommonClass.CalendarController.toPrint) {
						if (calItem.DepTimeFirstLegDecoration == "Strikethrough") {
							NSAttributedString attString = new NSAttributedString (
								                               calItem.DepTimeFirstLeg,
								                               new NSStringAttributes { StrikethroughStyle = (int)NSUnderlineStyle.Single }
							                               );
							view.DepTimeFirstView.AttributedStringValue = attString;
						}

						if (calItem.LandTimeLastLegDecoration == "Strikethrough") {
							NSAttributedString attString = new NSAttributedString (
								                               calItem.LandTimeLastLeg,
								                               new NSStringAttributes { StrikethroughStyle = (int)NSUnderlineStyle.Single }
							                               );
							view.ArrTimeLastView.AttributedStringValue = attString;
						}

						view.BackView1.BackgroundColor = NSColor.Clear;
						view.BackView2.BackgroundColor = NSColor.Clear;


						if (CommonClass.selectedTrip != null && CommonClass.selectedTrip == calItem.TripNumber) {

                            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
                            if (interfaceStyle == "Dark") {
                                view.BackView1.BackgroundColor = NSColor.Red;
                                view.BackView2.BackgroundColor = NSColor.Red;
                            }
                            else {
                                view.BackView1.BackgroundColor = NSColor.Yellow;
                                view.BackView2.BackgroundColor = NSColor.Yellow;
                            }
                           
						} else {
							if (calItem.ColorTop != null) {
                                if (calItem.ColorTop == "Red")
                                    view.BackView1.BackgroundColor = ColorClass.OverlapColor;
                                else if (calItem.ColorTop == "Green")
                                    view.BackView1.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                                else if (calItem.ColorTop == CalenderColortype.VA.ToString())
                                    view.BackView1.BackgroundColor = ColorClass.VacationTripColor;
                                else if (calItem.ColorTop == CalenderColortype.VO.ToString())
                                    view.BackView1.BackgroundColor = ColorClass.VacationOverlapTripColor;
                                else if (calItem.ColorTop == CalenderColortype.VD.ToString())
                                    view.BackView1.BackgroundColor = ColorClass.VacationDropTripColor;
                                else if (calItem.ColorTop == CalenderColortype.VD.ToString())
                                    view.BackView1.BackgroundColor = ColorClass.VacationDropTripColor;
                                else if (calItem.ColorTop == CalenderColortype.VD.ToString())
                                    view.BackView1.BackgroundColor = ColorClass.VacationDropTripColor;
                                else if (calItem.ColorTop == CalenderColortype.Transparaent.ToString())
                                    view.BackView1.BackgroundColor = NSColor.Clear;
                                else if (calItem.ColorTop == CalenderColortype.MILVO_No_Work.ToString())
                                    view.BackView1.BackgroundColor = NSColor.Orange;
                                else if (calItem.ColorTop == CalenderColortype.MILBackSplitWork.ToString())
                                    view.BackView1.BackgroundColor = NSColor.Orange;
                                else if (calItem.ColorTop == CalenderColortype.MILFrontSplitWork.ToString())
                                    view.BackView1.BackgroundColor = NSColor.Clear;
                                else if (calItem.ColorTop == CalenderColortype.MILBackSplitWithoutStrike.ToString())
                                    view.BackView1.BackgroundColor = NSColor.Orange;
                                else if (calItem.ColorTop == CalenderColortype.MILFrontSplitWithoutStrike.ToString())
                                    view.BackView1.BackgroundColor = NSColor.Clear;
                                else if (calItem.ColorTop == CalenderColortype.FV.ToString())
                                    view.BackView1.BackgroundColor = ColorClass.FVVacationColor;
                                else
									view.BackView1.BackgroundColor = NSColor.Clear;

							}
							if (calItem.ColorBottom != null) {
                                if (calItem.ColorBottom == "Red")
                                    view.BackView2.BackgroundColor = ColorClass.OverlapColor;
                                else if (calItem.ColorBottom == "Green")
                                    view.BackView2.BackgroundColor = ColorClass.VacationOverlapTripColor;
                                else if (calItem.ColorBottom == CalenderColortype.VA.ToString())
                                    view.BackView2.BackgroundColor = ColorClass.VacationTripColor;
                                else if (calItem.ColorBottom == CalenderColortype.VO.ToString())
                                    view.BackView2.BackgroundColor = ColorClass.VacationOverlapTripColor;
                                else if (calItem.ColorBottom == CalenderColortype.VD.ToString())
                                    view.BackView2.BackgroundColor = ColorClass.VacationDropTripColor;
                                else if (calItem.ColorBottom == CalenderColortype.VD.ToString())
                                    view.BackView2.BackgroundColor = ColorClass.VacationDropTripColor;
                                else if (calItem.ColorBottom == CalenderColortype.VD.ToString())
                                    view.BackView2.BackgroundColor = ColorClass.VacationDropTripColor;
                                else if (calItem.ColorBottom == CalenderColortype.Transparaent.ToString())
                                    view.BackView2.BackgroundColor = NSColor.Clear;
                                else if (calItem.ColorBottom == CalenderColortype.MILVO_No_Work.ToString())
                                    view.BackView2.BackgroundColor = NSColor.Orange;
                                else if (calItem.ColorBottom == CalenderColortype.MILBackSplitWork.ToString())
                                    view.BackView2.BackgroundColor = NSColor.Clear;
                                else if (calItem.ColorBottom == CalenderColortype.MILFrontSplitWork.ToString())
                                    view.BackView2.BackgroundColor = NSColor.Orange;
                                else if (calItem.ColorBottom == CalenderColortype.MILBackSplitWithoutStrike.ToString())
                                    view.BackView2.BackgroundColor = NSColor.Clear;
                                else if (calItem.ColorBottom == CalenderColortype.MILFrontSplitWithoutStrike.ToString())
                                    view.BackView2.BackgroundColor = NSColor.Orange;
                                else if (calItem.ColorBottom == CalenderColortype.FV.ToString())
                                    view.BackView2.BackgroundColor = ColorClass.FVVacationColor;
								else
									view.BackView2.BackgroundColor = NSColor.Clear;
							}
						}
					} else {
						view.BackView1.BackgroundColor = NSColor.Clear;
						view.BackView2.BackgroundColor = NSColor.Clear;
					}

					view.Button.Activated += (object sender, EventArgs e) => {
						CommonClass.CalendarController.btnCalendarClicked (sender);
					};
				}
			}
		}
	}

	public class CalendarView : NSView
	{
		private NSTextField dayView;
		private NSTextField depTimeFirstView;
		private NSTextField arrLastLegView;
		private NSTextField arrTimeLastView;
		private NSTextField backView1;
		private NSTextField backView2;
		private NSButton button;

		public CalendarView () : base (new CGRect (0, 0, 60, 66))
		{
			dayView = new NSTextField (new CGRect (5, 48, 20, 17));
			dayView.Bordered = false;
			dayView.Editable = false;
			dayView.DrawsBackground = false;
			AddSubview (dayView);

			backView1 = new NSTextField (new CGRect (5, 27, 50, 23));
			backView1.Bordered = false;
			backView1.Editable = false;
			backView1.DrawsBackground = true;
			AddSubview (backView1);
			backView2 = new NSTextField (new CGRect (5, 4, 50, 23));
			backView2.Bordered = false;
			backView2.Editable = false;
			backView2.DrawsBackground = true;
			AddSubview (backView2);

			depTimeFirstView = new NSTextField (new CGRect (8, 35, 45, 15));
			depTimeFirstView.Bordered = false;
			depTimeFirstView.Editable = false;
			depTimeFirstView.Alignment = NSTextAlignment.Center;
			depTimeFirstView.DrawsBackground = false;
			AddSubview (depTimeFirstView);
			arrLastLegView = new NSTextField (new CGRect (8, 22, 45, 15));
			arrLastLegView.Bordered = false;
			arrLastLegView.Editable = false;
			arrLastLegView.Alignment = NSTextAlignment.Center;
			arrLastLegView.DrawsBackground = false;
			AddSubview (arrLastLegView);
			arrTimeLastView = new NSTextField (new CGRect (8, 9, 45, 15));
			arrTimeLastView.Bordered = false;
			arrTimeLastView.Editable = false;
			arrTimeLastView.Alignment = NSTextAlignment.Center;
			arrTimeLastView.DrawsBackground = false;
			AddSubview (arrTimeLastView);

			button = new NSButton (new CGRect (0, 0, 60, 66));
			button.Title = "";
			button.Transparent = true;
			button.IgnoresMultiClick = true;
			AddSubview (button);
		}

		public NSTextField DayView {
			get { return dayView; }	
		}

		public NSTextField DepTimeFirstView {
			get { return depTimeFirstView; }	
		}

		public NSTextField ArrLastLegView {
			get { return arrLastLegView; }	
		}

		public NSTextField ArrTimeLastView {
			get { return arrTimeLastView; }	
		}

		public NSButton Button {
			get { return button; }	
		}

		public NSTextField BackView1 {
			get { return backView1; }	
		}

		public NSTextField BackView2 {
			get { return backView2; }	
		}

	}

	public class CalendarClass : NSObject
	{
		[Export ("Day")]
		public string Day { get; set; }

		[Export ("ArrLastLeg")]
		public string ArrLastLeg { get; set; }

		[Export ("ArrTimeLast")]
		public string ArrTimeLast { get; set; }

		[Export ("DepTimeFirst")]
		public string DepTimeFirst { get; set; }

		[Export ("Index")]
		public string Index { get; set; }

		[Export ("ID")]
		public string ID { get; set; }

	}
}

