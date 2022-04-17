
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using CoreGraphics;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Text.RegularExpressions;

namespace WBid.WBidMac.Mac
{
	public partial class TripWindowController : AppKit.NSWindowController
	{
		public string tripNum;
		#region Constructors

		// Called when created from unmanaged code
		public TripWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public TripWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public TripWindowController() : base("TripWindow")
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}

		#endregion

		//strongly typed window accessor
		public new TripWindow Window
		{
			get
			{
				return (TripWindow)base.Window;
			}
		}
		//		static NSButton closeButton;
		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			this.ShouldCascadeWindows = false;

			btnFFDO.Hidden = !(GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO");
			try
			{
				//				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				//				closeButton.Activated += (sender, e) => {
				//					this.Window.Close ();
				//					this.Window.OrderOut (this);
				//					NSApplication.SharedApplication.StopModal ();
				//					CommonClass.selectedTrip = null;
				//					if (CommonClass.MainController.sgViewSelect.SelectedSegment == 1 || CommonClass.MainController.sgViewSelect.SelectedSegment == 2) {
				//						CommonClass.ViewChanged = true;
				//						CommonClass.MainController.ReloadAllContent ();
				//						CommonClass.ViewChanged = false;
				//					} else if (CommonClass.MainController.sgViewSelect.SelectedSegment == 0 && CommonClass.CalendarController != null)
				//						CommonClass.CalendarController.LoadContent ();
				//				};
				this.Window.WillClose += delegate
				{
					this.Window.OrderOut(this);
					//					NSApplication.SharedApplication.StopModal ();
					CommonClass.selectedTrip = null;
					CommonClass.isLastTrip = false;
					if (CommonClass.MainController.sgViewSelect.SelectedSegment == 1 || CommonClass.MainController.sgViewSelect.SelectedSegment == 2)
					{
						CommonClass.ViewChanged = true;
						CommonClass.MainController.ReloadAllContent();
						CommonClass.ViewChanged = false;
					}
					else if (CommonClass.MainController.sgViewSelect.SelectedSegment == 0 && CommonClass.CalendarController != null)
						CommonClass.CalendarController.LoadContent();
				};
				tblTrip.Source = new TripTableSource();
				btnPrint.Activated += btnPrintClicked;
				btnFFDO.Activated += btnFFDOClicked;

			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			}
		}
		partial void ExportToOutLookFunctionality(NSObject sender)

		{

			List<AppointmentDetails> apointmentslist = new List<AppointmentDetails>();
			if (GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing)
			{
				apointmentslist = ExportToOutlook();
			}
			else
			{
				apointmentslist = GetFlightData();
			}

			foreach (var apointments in apointmentslist)
			{

				string MainScript = @"tell application ""Microsoft Outlook""
				
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


				string replaced = MainScript.Replace(@"vSubject", apointments.Subject).Replace(@"vContent", apointments.Body).Replace(@"vStartDay", apointments.StartDateTime.Day.ToString()).Replace(@"vStartMonth", apointments.StartDateTime.Month.ToString()).Replace(@"vStartYear", apointments.StartDateTime.Year.ToString()).Replace(@"vStartHour", apointments.StartDateTime.Hour.ToString()).Replace(@"vStartMinutes", apointments.StartDateTime.Minute.ToString()).Replace("vStartSeconds", apointments.StartDateTime.Second.ToString()).Replace(@"vEndDay", apointments.EndDateTime.Day.ToString()).Replace(@"vEndMonth", apointments.EndDateTime.Month.ToString()).Replace(@"vEndYear", apointments.EndDateTime.Year.ToString()).Replace(@"vEndHour", apointments.EndDateTime.Hour.ToString()).Replace(@"vEndMinutes", apointments.EndDateTime.Minute.ToString()).Replace(@"vEndSeconds", apointments.EndDateTime.Second.ToString());
				Console.WriteLine(replaced);


				NSAppleScript scr = new NSAppleScript(replaced);
				NSAppleEventDescriptor eventss = new NSAppleEventDescriptor();
				NSDictionary dic = new NSDictionary();
				eventss = scr.ExecuteAndReturnError(out dic);
				if (eventss != null)
				{
					Console.WriteLine("Done");
					var alert = new NSAlert();
					alert.MessageText = "Pairing exported to Outlook";


					alert.InformativeText = "Pairing " + tripNum.Substring(0, 4) + " starting on " + apointments.StartDate + " exported to outlook as an Appointment";
					alert.RunModal();

				}
				else
				{
					Console.WriteLine("failed");
					var alert = new NSAlert();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "Pairing failed to export Outlook "+ dic.Values.ToList()[0].ToString();
					alert.RunModal();
				}
			}

		}
		void btnFFDOClicked(object sender, EventArgs e)
		{
			try
			{
				string ffdoData = GetFlightDataforFFDB(tripNum, CommonClass.isLastTrip);
				NSPasteboard clipBoard = NSPasteboard.GeneralPasteboard;
				clipBoard.DeclareTypes(new string[] { "NSStringPboardType" }, null);
				clipBoard.SetStringForType(ffdoData, "NSStringPboardType");
				var alert = new NSAlert();
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Trips has been copied to ClipBoard!";
				alert.RunModal();
			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			}
		}

		/// <summary>
		/// PURPOSE : Get Flight data for FFDB
		/// </summary>
		/// <param name="trip"></param>
		/// <param name="tripName"></param>
		private string GetFlightDataforFFDB(string tripNum, bool isLastTrip)
		{

			Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum.Substring(0, 4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum);
			string result = string.Empty;

			// var tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(SelectedDay.Replace(" ", "")));
			DateTime dutPeriodDate = WBidCollection.SetDate(int.Parse(tripNum.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
			//DateTime dutPeriodDate = SelectedTripStartDate;

			foreach (var dp in trip.DutyPeriods)
			{
				string datestring = dutPeriodDate.ToString("MM'/'dd'/'yyyy");
				if (trip.ReserveTrip)
				{

					result += datestring + "  RSRV " + trip.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveOut % 1440)).Replace(":", "") + " " + trip.RetSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveIn % 1440)).Replace(":", "") + " \n";
				}
				else
				{
					foreach (var flt in dp.Flights)
					{
						result += datestring + " " + flt.FltNum.ToString().PadLeft(4, '0') + " " + flt.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.DepTime.ToString()).Replace(":", "") + " " + flt.ArrSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.ArrTime.ToString()).Replace(":", "") + " \n";
					}
				}
				dutPeriodDate = dutPeriodDate.AddDays(1);
			}
			return result;
		}

		void btnPrintClicked(object sender, EventArgs e)
		{


			try
			{
				var str = TripPrint();
				var inv = new InvisibleWindowController();
				this.Window.AddChildWindow(inv.Window, NSWindowOrderingMode.Below);
				var txt = new NSTextView(new CGRect(0, 0, 550, 550));
				txt.Font = NSFont.FromFontName("Courier", 10);
				inv.Window.ContentView.AddSubview(txt);
				txt.Value = str;
				var pr = NSPrintInfo.SharedPrintInfo;
				pr.VerticallyCentered = false;
				pr.TopMargin = 2.0f;
				pr.BottomMargin = 2.0f;
				pr.LeftMargin = 1.0f;
				pr.RightMargin = 1.0f;
				txt.Print(this);
				inv.Close();
			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			}
		}

		private string TripPrint()
		{
			var content = string.Empty;
			foreach (TripData data in CommonClass.tripData)
			{
				content += "\n";
				content += data.Content;
			}

			return content;
		}
		private List<AppointmentDetails> ExportToOutlook()
		{
			//input paramets needs to check
			//string tripName, DateTime selectedTripStartDate, string body
			List<AppointmentDetails> appointmentslist = new List<AppointmentDetails>();
			string tripname = tripNum;
			Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum.Substring(0, 4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum);
			DateTime tripStartDate = WBidCollection.SetDate(int.Parse(tripNum.Substring(4, 2)), CommonClass.isLastTrip, GlobalSettings.CurrentBidDetails);

			var content = string.Empty;
			foreach (TripData data in CommonClass.tripData)
			{
				content += "<br />";
				content += data.Content;
			}
			ExportPairingDetails exportdetails = new ExportPairingDetails();
			AppointmentDetails apointments = exportdetails.ExportToOutlook(trip.TripNum, tripStartDate, content);
			appointmentslist.Add(apointments);
			return appointmentslist;
			//you will get all the details in the apointments, like body,subject,start date,start time etc..look into the appointment class for details
			//MessageBox.Show("Pairing " + tripName + " starting on " + appointmentDetails.StartDate + " exported to outlook as an Appointment", "Pairing exported to Outlook", MessageBoxButton.OK, MessageBoxImage.Information);

		}
		public List<AppointmentDetails> GetFlightData()
		{
			string fltData = null;
			int count = 0;
			// int showTime = 0;
			List<AppointmentDetails> appointmentslist = new List<AppointmentDetails>();
			ExportPairingDetails exportPairingDetails = new ExportPairingDetails();
			string tripname = tripNum;
			Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum.Substring(0, 4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum);
			DateTime tripStartDate = WBidCollection.SetDate(int.Parse(tripNum.Substring(4, 2)), CommonClass.isLastTrip, GlobalSettings.CurrentBidDetails);

			DateTime dutPeriodDate = tripStartDate;
			string subject = string.Empty;
			foreach (var dp in trip.DutyPeriods)
			{
				if (trip.ReserveTrip)
				{
					fltData += "<br />";
					fltData += GetReserveSingleLineData(trip, dp, count);

				}
				else
				{
					foreach (var flt in dp.Flights)
					{
						fltData += "<br />";
						fltData += GetSingleFlightData(flt);
					}
					if (!trip.ReserveTrip)
					{
						if (count + 1 < trip.DutyPeriods.Count)
						{
							///add Hotels data to the flight details
							var hotels = GlobalSettings.WBidINIContent.Hotels.HotelList.Where(x => x.City == trip.DutyPeriods[count].ArrStaLastLeg).FirstOrDefault();
							if (hotels != null)
							{
								fltData += "<br /> \n <br />";
								fltData += hotels.Hotels + "\n";
							}

						}
					}

				}

				int day = 0;

				if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
				{
					subject = dp.ArrStaLastLeg + " " + FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) % 1440, out day);
				}
				else
				{

					string domicileTime = FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) % 1440, out day);
					bool isDayBefore;
					string modifiedtime;
					var startingTime = ConvertTimeToDomicile(domicileTime, dutPeriodDate.AddDays(day), out isDayBefore, out modifiedtime);
					if (isDayBefore)
					{
						subject = dp.ArrStaLastLeg + " " + modifiedtime;
					}
					else
					{
						subject = dp.ArrStaLastLeg + " " + startingTime;
					}

				}

				if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected)
				{
					subject = trip + " " + subject;
				}

				// showTime = (count == 0) ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;
				count++;

				string startTime;
				string endTime;


				startTime = FormatTime((dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440)), out day);
				endTime = FormatTime((dp.LandTimeLastLeg + GlobalSettings.debrief) - ((dp.DutPerSeqNum - 1) * 1440), out day);

				AppointmentDetails appointments = ExportDutyWiseDetailsToOutLook(dutPeriodDate, dutPeriodDate.AddDays(day), startTime, endTime, fltData, subject);
				appointmentslist.Add(appointments);
				dutPeriodDate = dutPeriodDate.AddDays(1);
				fltData = string.Empty;

			}
			//System.Windows.MessageBox.Show("Pairing " + SelectedTrip + " starting on " + SelectedTripStartDate + " exported to outlook as an Appointment", "Pairing exported to Outlook", MessageBoxButton.OK, MessageBoxImage.Information);
			return appointmentslist;
		}
		public AppointmentDetails ExportDutyWiseDetailsToOutLook(DateTime startDate, DateTime endDate, string startTime, string EndTime, string body, string subject)
		{
			try
			{
				
				AppointmentDetails appointmentDetails = new AppointmentDetails();
				appointmentDetails.Subject = subject;
				appointmentDetails.StartDate = startDate.ToShortDateString();

				appointmentDetails.EndDate = endDate.ToShortDateString();
				appointmentDetails.StartTime = startTime;
				appointmentDetails.EndTime = EndTime;
				if (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
				{
					appointmentDetails.StartTime = startTime;
					appointmentDetails.EndTime = EndTime;
				}
				else
				{
					bool isDayBefore;
					string modifiedtime;
					appointmentDetails.StartTime = ConvertTimeToDomicile(startTime, startDate, out isDayBefore, out modifiedtime);
					if (isDayBefore)
					{
						appointmentDetails.StartDate = startDate.AddDays(-1).ToShortDateString();
						appointmentDetails.StartTime = modifiedtime;
					}

					appointmentDetails.EndTime = ConvertTimeToDomicile(EndTime, endDate, out isDayBefore, out modifiedtime);
					if (isDayBefore)
					{
						appointmentDetails.EndDate = endDate.AddDays(-1).ToShortDateString();
						appointmentDetails.EndTime = modifiedtime;
						//appointmentDetails.Subject = subject;
					}

				}
				if (appointmentDetails.StartDateTime > appointmentDetails.EndDateTime)
					appointmentDetails.EndDate = endDate.AddDays(1).ToShortDateString();
				body += Environment.NewLine + Environment.NewLine + "<br /> (Note: All times are CST/CDT unless otherwise noted.)";
				appointmentDetails.Body = "REPORT " + startTime + " CST/CDT";
				if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
				{
					appointmentDetails.Body += " (" + appointmentDetails.StartTime + " Domicile Time)";
					body += Environment.NewLine + "<br />(Note: AppointmentDetails times  are in Domicile time).";
				}
				appointmentDetails.Body += Environment.NewLine + Environment.NewLine + "<br />"+body;
				return appointmentDetails;

			}
			catch (Exception ex)
			{

				throw ex;
			}
		}
		private string GetReserveSingleLineData(Trip trip, DutyPeriod dutyPeriod, int index)
		{
			string result = string.Empty;

			result = "RSRV\t" + trip.DepSta + "\t" + trip.RetSta + "\t" + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveOut - (1440*index))) +"\t" + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dutyPeriod.ReserveIn - (1440 * index))) +"\t";
			result += Helper.CalcTimeFromMinutesFromMidnight(dutyPeriod.Block.ToString()) + "\t" + dutyPeriod.Tfp.ToString("0.0") + "\t" + Environment.NewLine;

			return result;
		}

		private string GetSingleFlightData(Flight flt)
		{

			string flight = null;
			string changeEqp = string.Empty;
			string equipment = string.Empty;
			flight = flt.FltNum.ToString().PadLeft(4, '0');
			if (flt.Equip != null)
			{
				if (flt.Equip.Length > 0)
				{
					changeEqp = (flt.Equip.Substring(0, 1) == "*" ? "chg" : "");
					equipment = WBidCollection.GetEquipmentName(flt.Equip);
				}
			}


			flight += "\t" +
						flt.DepSta + "\t" + flt.ArrSta + "\t" +
						Helper.CalcTimeFromMinutesFromMidnight(flt.DepTime.ToString()) + "\t" +
						Helper.CalcTimeFromMinutesFromMidnight(flt.ArrTime.ToString()) + "\t" +
						Helper.CalcTimeFromMinutesFromMidnight((flt.ArrTime - flt.DepTime).ToString()) + "\t" +
						flt.Tfp.ToString("0.0") + "\t" +  // block time
						equipment + " " + (flt.DeadHead ? "dhd" : "") +
						changeEqp + Environment.NewLine;
			return flight;
		}
		private string FormatTime(int time, out int day)
		{
			day = 0;

			string stringTime = string.Empty;
			int hour = 0;
			int minutes = 0;


			hour = time / 60;
			minutes = time % 60;

			if (hour >= 24)
			{
				if (minutes > 0)
				{
					day = 1;
				}

				hour = hour - 24;
				stringTime = hour.ToString().PadLeft(2, '0');
			}
			else
			{
				stringTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
			}
			stringTime += ":" + minutes.ToString("d2");

			stringTime += ((hour >= 12) ? " PM" : " AM");

			return stringTime;
		}
		private string FormatTime(int time)
		{


			string stringTime = string.Empty;
			int hour = 0;
			int minutes = 0;


			hour = time / 60;
			minutes = time % 60;


			if (hour >= 24)
			{
				if (minutes > 0)
				{

				}

				hour = hour - 24;
				stringTime = hour.ToString().PadLeft(2, '0');
			}
			else
			{
				stringTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
			}
			stringTime += ":" + minutes.ToString("d2");

			stringTime += ((hour >= 12) ? " PM" : " AM");

			return stringTime;

		}

		private string ConvertTimeToDomicile(string time, DateTime date, out bool isDayBefore, out string modifiedtime)
		{
			modifiedtime = "0";
			int hours = 0;
			int minutes = 0;
			int result = 0;
			string strTime = string.Empty;

			if (time.Substring(6, 2) == "PM" && int.Parse(time.Substring(0, 2)) != 12)
			{
				hours = 12;
			}

			hours += int.Parse(time.Substring(0, 2));
			minutes = int.Parse(time.Substring(3, 2));

			result = hours * 60 + minutes;

			result = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, result);
			if (result < 0)
			{
				isDayBefore = true;
				modifiedtime = FormatTime(1440 + result);

			}
			else
				isDayBefore = false;
			hours = result / 60;
			minutes = result % 60;

			if (hours == 24)
			{
				hours = 0;
				strTime = "00";
			}
			else
			{
				strTime = (hours > 12) ? (hours - 12).ToString("d2") : hours.ToString("d2");
			}
			strTime += ":" + minutes.ToString("d2");

			strTime += ((hours >= 12) ? " PM" : " AM");

			return strTime;
		}
		public static int DomicileTimeFromHerb(string domicile, DateTime dutPdDate, int herb)
		{
			// TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
			//TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
			//TimeSpan ts = new TimeSpan(12, 0, 0);

			// Insures that time dateTime is not 12:00AM but 12:00pm
			// Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.

			//dutPdDate = dutPdDate.Date + ts;
			bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(dutPdDate);
			//bool isDst = timeZoneInfo.IsDaylightSavingTime(dutPdDate);
			// bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(tstTime);



			switch (domicile)
			{
				case "ATL":
					return herb + 60;       // EST = herb + 60
					case "AUS":
					return herb;            // CST = herb 
				case "BWI":
					return herb + 60;       // EST = herb + 60
				case "DAL":
					return herb;            // CST = herb 
				case "DEN":
					return herb - 60;       // MST = herb - 60
					case "FLL":
					return herb + 60;       // EST = herb + 60
				case "HOU":
					return herb;            // CST = herb 
				case "LAS":
					return herb - 120;      // PST = herb - 120
                case "LAX":
                    return herb - 120;      // PST = herb - 120
				case "MCO":
					return herb + 60;       // EST = herb + 60
				case "MDW":
					return herb;            // CST = herb 
				case "OAK":
					return herb - 120;      // PST = herb - 120
				case "PHX":
					if (isDst)
						return herb - 120;  // PST = herb - 120
					else
						return herb - 60;   // MST = herb - 60
				default:
					return 1440;
			}
		}
		public void LoadContent()
		{
			this.Window.Title = "Pairing " + tripNum.Substring(0, 4);
			if (GlobalSettings.CurrentBidDetails.Postion == "FA")
			{
				CorrectionParams correctionParams = new CorrectionParams();
				correctionParams.selectedLineNum = CommonClass.selectedLine;
				CommonClass.tripData = TripViewBL.GenerateTripDetails(tripNum, correctionParams, CommonClass.isLastTrip);

				var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == correctionParams.selectedLineNum);
				//lines.FirstOrDefault(y => y.Pairings.Any(x => x == tripname));
				if (line != null)
				{
					if (GlobalSettings.CurrentBidDetails.Round == "M")
					{
						this.Window.Title += "  - " + string.Join("", line.FAPositions);// + " Positions";
					}
					else
					{
						this.Window.Title += "  - " + line.FASecondRoundPositions.FirstOrDefault(x => x.Key.Contains(tripNum.Substring(0, 4))).Value;// + " Position";
					}
				}
			}
			tblTrip.ReloadData();
		}
	}

	public partial class TripTableSource : NSTableViewSource 
	{
		public override nint GetRowCount (NSTableView tableView)
		{
			return CommonClass.tripData.Count;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var vw = (NSTableCellView)tableView.MakeView ("Trip", this);
			vw.TextField.StringValue = CommonClass.tripData [(int)row].Content;
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");

            if (CommonClass.tripData[(int)row].BackColor == "VA")
            {
                vw.TextField.BackgroundColor = ColorClass.VacationTripColor;
                if (interfaceStyle == "Dark") {
                    vw.TextField.TextColor = NSColor.White;
                } else {
                    vw.TextField.TextColor = NSColor.Black;
                }

            }
            else if (CommonClass.tripData[(int)row].BackColor == "VD")
            {
                vw.TextField.BackgroundColor = ColorClass.VacationDropTripColor;
                if (interfaceStyle == "Dark")
                {
                    vw.TextField.TextColor = NSColor.White;
                }
                else
                {
                    vw.TextField.TextColor = NSColor.Black;
                }
            }
            else if (CommonClass.tripData[(int)row].BackColor == "VO")
            {
                vw.TextField.BackgroundColor = ColorClass.VacationOverlapTripColor;
                if (interfaceStyle == "Dark")
                {
                    vw.TextField.TextColor = NSColor.White;
                }
                else
                {
                    vw.TextField.TextColor = NSColor.Black;
                }
            }
            else if (CommonClass.tripData[(int)row].BackColor == "Overlap")
            {
                vw.TextField.BackgroundColor = ColorClass.OverlapColor;
                if (interfaceStyle == "Dark")
                {
                    vw.TextField.TextColor = NSColor.White;
                }
                else
                {
                    vw.TextField.TextColor = NSColor.Black;
                }
            }
            else if (CommonClass.tripData[(int)row].BackColor == "MD")
            {
                vw.TextField.BackgroundColor = NSColor.Orange;
                if (interfaceStyle == "Dark")
                {
                    vw.TextField.TextColor = NSColor.White;
                }
                else
                {
                    vw.TextField.TextColor = NSColor.Black;
                }
            }
            else if (CommonClass.tripData[(int)row].BackColor == "FV")
            {
                vw.TextField.BackgroundColor = ColorClass.FVVacationColor;
                if (interfaceStyle == "Dark")
                {
                    vw.TextField.TextColor = NSColor.White;
                }
                else
                {
                    vw.TextField.TextColor = NSColor.Black;
                }
            }
            
            else {
				vw.TextField.BackgroundColor = NSColor.Clear;
                if (interfaceStyle == "Dark")
                {
                    vw.TextField.TextColor = NSColor.White;
                }
                else
                {
                    vw.TextField.TextColor = NSColor.Black;
                }
            }

			if (CommonClass.tripData [(int)row].IsStrike) {
				NSAttributedString attString = new NSAttributedString (
					CommonClass.tripData [(int)row].Content,
					new NSStringAttributes { StrikethroughStyle = (int)NSUnderlineStyle.Single }
				);
				vw.TextField.AttributedStringValue = attString;
			} else {
				NSAttributedString attString = new NSAttributedString (
					CommonClass.tripData [(int)row].Content,
					new NSStringAttributes { StrikethroughStyle = (int)NSUnderlineStyle.None }
				);
				vw.TextField.AttributedStringValue = attString;
			}

			return vw;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
//			if (!string.IsNullOrEmpty(CommonClass.tripData[row].Content)&&string.IsNullOrEmpty (CommonClass.tripData [row].BackColor))
//				return 30;
//			else
				return 15;
		}
	}
}

