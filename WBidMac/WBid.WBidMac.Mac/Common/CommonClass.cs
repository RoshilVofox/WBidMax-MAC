using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.Text;
using System.IO;
using CoreGraphics;
//using CoreGraphics;

using WBid.WBidiPad.iOS.Utility;
using WBid.WBidMac.Mac.WindowControllers;
using WBid.WBidMac.Mac.WindowControllers.SynchView;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Font;
using iText.Layout.Element;
using iText.IO.Font.Constants;

namespace WBid.WBidMac.Mac
{
	public class ConnectTimeHelper
	{
		public string Day { get; set; }

		public string Arrival { get; set; }

		public string Departure { get; set; }

		public bool IsEnabled { get; set; }

		public DateTime Date { get; set; }


	}
	public class CommonClass
	{
		public CommonClass ()
		{
			// now 64bit!
		}

		public static int VisibleRow = 0;
		public static string Platform = "Mac";
		public static string OperatingSystem = "Mac OS";
		public static string AppVersion = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
		//NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"].ToString ();
		public static ObservableCollection<ConnectTimeHelper> ListCommuteTime;
		public static int ArrivalPageIndex = 0;
		public static AppDelegate AppDelegate { set; get; }

		public static HomeWindowController HomeController { set; get; }

		public static MainWindowController MainController { set; get; }

		public static DownloadBidWindowController DownloadController { set; get; }

		public static CalendarWindowController CalendarController { set; get; }

		public static CSWWindowController CSWController { set; get; }

		public static SynchViewWindowController synchVController { set; get; }

		public static SecretDataDownloadController SecretDataController { set; get; }

        public static BAWindowController BAController { set; get; }

		public static ConfigurationWindowController ConfigureController { set; get; }

		public static SummaryViewController SummaryController { set; get; }

		public static BidLineViewController BidLineController { set; get; }

		public static ModernViewController ModernController { set; get; }
        public static BASortsViewController BASortController { set; get; }
		public static ConstraintsViewController ConstraintsController { set; get; }
		public static BAFilterViewController FilterController { set; get; }
		public static SortsViewController SortController { set; get; }

		public static WeightsViewController WeightsController { set; get; }

		public static HelpWindowController HelpController { set; get; }

		public static MILConfigViewController MILController { set; get; }

		public static NSPanel Panel { set; get; }

		public static string UserName { set; get; }

		public static string Password { set; get; }

		public static int columnID { get; set; }

		public static bool columnAscend { get; set; }

		public static bool isHomeWindow { get; set; }

		public static bool isTKeyPressed { get; set; }

		public static bool isBKeyPressed { get; set; }

		public static bool ViewChanged { get; set; }

		public static bool isShowDataTab { get; set; }

		public static List <int> selectedRows { get; set; }

		public static int selectedLine { get; set; }

		public static Line importLine { get; set; }

		public static string selectedTrip { get; set; }

		public static bool isLastTrip { get; set; }

		public static ObservableCollection <CalendarData> calData = new ObservableCollection <CalendarData> ();
		public static ObservableCollection <TripData> tripData = new ObservableCollection <TripData> ();

		public static List <string> bidLineProperties { get; set; }

		public static List <string> modernProperties { get; set; }

		public static List<Absense> MILList = new List<Absense> ();

		public static List<string> listFilters = new List<string> {
			"Add Filters",
			"Am - Pm",
			//			"Blank-Reserve",
			"Commutable Lines",
			"Days of the Month",
			"Days of the Week - All",
			"Days of the Week - Some",
			"DH First-Last",
			"Equipment",
			"Line Type",
			"Overnight Cities",
			"Rest",
			"Start Day Of Week",
			"Trip-Work-Block Length"
		} ;

		public static Dictionary<string,string> DicFilters = new Dictionary<string,string> {
			{"Add Filters","Add Filters"},
			{"Am - Pm","AP"},
			{"Commutable Lines - Auto","CL"},
			{"Days of the Month","DOM"},
			{"Days of the Week - All","DOWA"},
			{"Days of the Week - Some","DOWS"},
			{"DH First-Last","DHFL"},
			{"Equipment","ET"},
			{"Line Type","LT"},
			{"Overnight Cities","OC"},
			{"Rest","RT"},
			{"Start Day Of Week","SDOW"},
			{"Trip-Work-Block Length","TBL"}
		} ;


		#region Print Methods

		public static string PrintSummaryLines ()
		{
			var content = string.Empty;
			var space = "\t ";
			content += "Ord" + space + "Line" + space;
			foreach (var colID in GlobalSettings.WBidINIContent.DataColumns) {
				var col = GlobalSettings.columndefinition.FirstOrDefault (x => x.Id == colID.Id);
				if (!col.IsRequied) {
					content += col.DisplayName + space;
				}
			}
			content += "\n\n";
			int ord = 1;
			foreach (var line in GlobalSettings.Lines) {
				content += ord.ToString () + space + line.LineNum.ToString () + space;
				foreach (var colID in GlobalSettings.WBidINIContent.DataColumns) {
					var col = GlobalSettings.columndefinition.FirstOrDefault (x => x.Id == colID.Id);
					if (!col.IsRequied) {
						content += GetLineProperty (col.DisplayName, line) + space;
					}
				}
				content += "\n";
				ord++;
			}
			return content;
		}

		public static string PrintBidLines (int lineNum)
		{
			var content = string.Empty;
			var dotLine = "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
			content += dotLine + "\n";
			var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == lineNum);

			content += "Line " + line.LineNum + "\t ";
			content += CommonClass.modernProperties [0] + "\t " + GetLineProperty (CommonClass.modernProperties [0], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (item.Date.Day.ToString ().Length == 1)
					content += item.Date.Day.ToString () + "   ";
				else
					content += item.Date.Day.ToString () + "  ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [1] + "\t " + GetLineProperty (CommonClass.modernProperties [1], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				content += item.Date.DayOfWeek.ToString ().Substring (0, 2).ToUpper () + "  ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [2] + "\t " + GetLineProperty (CommonClass.modernProperties [2], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (string.IsNullOrEmpty (item.TripNum))
					content += "*" + "   ";
				else
					content += item.TripNum + " ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [3] + "\t " + GetLineProperty (CommonClass.modernProperties [3], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (string.IsNullOrEmpty (item.ArrStaLastLeg))
					content += "*" + "   ";
				else
					content += item.ArrStaLastLeg + " ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [4] + "\t " + GetLineProperty (CommonClass.modernProperties [4], line) + "\t";

			content += line.Pairingdesription;
			content += "\n";
			content += dotLine + "\n\n";

			return content;
		}

		public static string PrintAllPairings ()
		{
			var content = string.Empty;
			var dotLine = "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
			content += dotLine + "\n";
			var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine);

			content += "Line " + line.LineNum + "\t ";
			content += CommonClass.modernProperties [0] + "\t " + GetLineProperty (CommonClass.modernProperties [0], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (item.Date.Day.ToString ().Length == 1)
					content += item.Date.Day.ToString () + "   ";
				else
					content += item.Date.Day.ToString () + "  ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [1] + "\t " + GetLineProperty (CommonClass.modernProperties [1], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				content += item.Date.DayOfWeek.ToString ().Substring (0, 2).ToUpper () + "  ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [2] + "\t " + GetLineProperty (CommonClass.modernProperties [2], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (string.IsNullOrEmpty (item.TripNum))
					content += "*" + "   ";
				else
					content += item.TripNum + " ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [3] + "\t " + GetLineProperty (CommonClass.modernProperties [3], line) + "\t";

			foreach (var item in line.BidLineTemplates) {
				if (string.IsNullOrEmpty (item.ArrStaLastLeg))
					content += "*" + "   ";
				else
					content += item.ArrStaLastLeg + " ";
			}
			content += "\n" + "\t ";
			content += CommonClass.modernProperties [4] + "\t " + GetLineProperty (CommonClass.modernProperties [4], line) + "\t";

			content += line.Pairingdesription;
			content += "\n";
			content += dotLine + "\n\n";
			foreach (var item in line.BidLineTemplates) {
				if (!string.IsNullOrEmpty (item.TripNum)) {
					CorrectionParams correctionParams = new WBid.WBidiPad.Model.CorrectionParams ();
					correctionParams.selectedLineNum = CommonClass.selectedLine;
					ObservableCollection<TripData> trip = TripViewBL.GenerateTripDetails (item.TripName, correctionParams, false);
					foreach (var tr in trip) {
						content += tr.Content + "\n";
					}
					content += dotLine + "\n\n";
				}
			}

			return content;
		}

		public static string GetLineProperty (string displayName, Line line)
		{
			if (displayName == "Line")
			{
				return line.LineDisplay;
			}
			else if (displayName == "$/Day")
			{
				return string.Format("{0:0.00}", line.TfpPerDay);
			}
			else if (displayName == "$/DHr")
			{
				return string.Format("{0:0.00}", line.TfpPerDhr);
			}
			else if (displayName == "$/Hr")
			{
				return string.Format("{0:0.00}", line.TfpPerFltHr);
			}
			else if (displayName == "$/TAFB")
			{
				return line.TfpPerTafb.ToString();
			}
			else if (displayName == "+Grd")
			{
				return line.LongestGrndTime.ToString();
			}
			else if (displayName == "+Legs")
			{
				return line.MostLegs.ToString();
			}
			else if (displayName == "+Off")
			{
				return line.LargestBlkOfDaysOff.ToString();
			}
			else if (displayName == "1Dy")
			{
				return line.Trips1Day.ToString();
			}
			else if (displayName == "2Dy")
			{
				return line.Trips2Day.ToString();
			}
			else if (displayName == "3Dy")
			{
				return line.Trips3Day.ToString();
			}
			else if (displayName == "4Dy")
			{
				return line.Trips4Day.ToString();
			}
			else if (displayName == "787m8m")
			{
				return line.Equip8753.ToString();
			}
			else if (displayName == "A/P")
			{
				return line.AMPM.ToString();
			}
			else if (displayName == "ACChg")
			{
				return line.AcftChanges.ToString();
			}
			else if (displayName == "ACDay")
			{
				return line.AcftChgDay.ToString();
			}
			else if (displayName == "CO")
			{
				return line.CarryOverTfp.ToString();
			}
			else if (displayName == "DP")
			{
				return line.TotDutyPds.ToString();
			}
			else if (displayName == "DPinBP")
			{
				return line.TotDutyPdsInBp.ToString();
			}
			else if (displayName == "EDomPush")
			{
				return (line.EDomPush != null) ? line.EDomPush : string.Empty;
			}
			else if (displayName == "EPush")
			{
				return (line.EPush != null) ? line.EPush : string.Empty;
			}
			else if (displayName == "FA Posn")
			{
				return string.Join("", line.FAPositions.ToArray());
			}
			else if (displayName == "Flt")
			{
				return (line.BlkHrsInBp != null) ? line.BlkHrsInBp : string.Empty;
			}
			else if (displayName == "LArr")
			{
				return line.LastArrTime.ToString(@"hh\:mm");
			}
			else if (displayName == "AEDP")
			{
				return line.AvgEarliestDomPush.ToString(@"hh\:mm");
			}
			else if (displayName == "ALDA")
			{
				return line.AvgLatestDomArrivalTime.ToString(@"hh\:mm");
			}
			else if (displayName == "LDomArr")
			{
				return line.LastDomArrTime.ToString(@"hh\:mm");
			}
			else if (displayName == "Legs")
			{
				return line.Legs.ToString();
			}
			else if (displayName == "LgDay")
			{
				return line.LegsPerDay.ToString();
			}
			else if (displayName == "LgPair")
			{
				return line.LegsPerPair.ToString();
			}
			else if (displayName == "ODrop")
			{
				return line.OverlapDrop.ToString();
			}
			else if (displayName == "Off")
			{
				return line.DaysOff.ToString();
			}
			else if (displayName == "Pairs")
			{
				return line.TotPairings.ToString();
			}
			else if (displayName == "Pay" || displayName == "TotPay")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.Tfp, 2));
			}
			else if (displayName == "PDiem")
			{
				return (line.TafbInBp != null) ? line.TafbInBp : string.Empty;
			}
			else if (displayName == "MyValue")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.Points, 2));
			}
			else if (displayName == "SIPs")
			{
				return line.Sips.ToString();
			}
			else if (displayName == "StartDOW")
			{
				return (line.StartDow != null) ? line.StartDow : string.Empty;
			}
			else if (displayName == "T234")
			{
				return (line.T234 != null) ? line.T234 : string.Empty;
			}
			else if (displayName == "VDrop")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VacationDrop, 2));
			}
			else if (displayName == "WkEnd")
			{
				return (line.Weekend != null) ? line.Weekend.ToLower() : string.Empty;
			}
			else if (displayName == "FltRig")
			{
				return line.RigFltInBP.ToString();
			}
			else if (displayName == "MinPayRig")
			{
				return line.RigDailyMinInBp.ToString();
			}
			else if (displayName == "DhrRig")
			{
				return line.RigDhrInBp.ToString();
			}
			else if (displayName == "AdgRig")
			{
				return line.RigAdgInBp.ToString();
			}
			else if (displayName == "TafbRig")
			{
				return line.RigTafbInBp.ToString();
			}
			else if (displayName == "TotRig")
			{
				return line.RigTotalInBp.ToString();
			}
            else if (displayName == "VacPayCu")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VacPayCuBp, 2));
			}
            else if (displayName == "VacPayNe")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayNeBp, 2));
            }
            else if (displayName == "VacPayBo")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPayBothBp, 2));
            }
			else if (displayName == "Vofrnt")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VacationOverlapFront, 2));
			}
			else if (displayName == "Vobk")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VacationOverlapBack, 2));
			}
			else if (displayName == "800legs")
			{
				return line.LegsIn800.ToString();
			}
			else if (displayName == "700legs")
			{
				return line.LegsIn700.ToString();
			}
			else if (displayName == "500legs")
			{
				return line.LegsIn500.ToString();
			}
			else if (displayName == "300legs")
			{
				return line.LegsIn300.ToString();
			}
			else if (displayName == "7Max")
			{
				return line.LegsIn200.ToString();
			}
			else if (displayName == "8Max")
			{
				return line.LegsIn600.ToString();
			}
			else if (displayName == "DhrInBp")
			{
				return (line.DutyHrsInBp != null) ? line.DutyHrsInBp : string.Empty;
			}
			else if (displayName == "DhrInLine")
			{
				return (line.DutyHrsInLine != null) ? line.DutyHrsInLine : string.Empty;
			}
			else if (displayName == "Wts")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.TotWeight, 2));
			}
			else if (displayName == "LineRig")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.LineRig, 2));
			}
			else if (displayName == "FlyPay")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.FlyPay, 2));
			}
			else if (displayName == "Tag")
			{
				return (line.Tag != null) ? line.Tag : string.Empty;
			}
			else if (displayName == "HolRig")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.HolRig, 2));
			}
			else if (displayName == "Grp")
			{
				return (line.BAGroup != null) ? line.BAGroup : string.Empty;
			}
			else if (displayName == "nMid")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.NightsInMid, 2));
			}
			else if (displayName == "cmts")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.TotalCommutes, 2));
			}
			else if (displayName == "cmtFr")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.commutableFronts, 2));
			}
			else if (displayName == "cmtBa")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.CommutableBacks, 2));
			}
			else if (displayName == "cmt%Fr")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.CommutabilityFront, 2));
			}
			else if (displayName == "cmt%Ba")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.CommutabilityBack, 2));
			}
			else if (displayName == "cmt%Ov")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.CommutabilityOverall, 2));
			}
			else if (displayName == "Ratio")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.Ratio, 2));
			}
            else if (displayName == "Vac+LG")
            {
                return string.Format("{0:0.00}", Decimal.Round(line.VacPlusRig, 2));
            }
			else if (displayName == "VAbo")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAbo, 2));
			}
			else if (displayName == "VAne")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAne, 2));
			}
			else if (displayName == "VAbp")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAbp, 2));
			}
			else if (displayName == "VAPbp")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAPbp, 2));
			}
			else if (displayName == "VAPne")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAPne, 2));
			}
			else if (displayName == "VAPbo")
			{
				return string.Format("{0:0.00}", Decimal.Round(line.VAPbo, 2));
			}
			else if (displayName == "Etrips")
			{
				return line.ETOPSTripsCount.ToString();
			}
			else if (displayName == "DHFirst")
			{
				return line.DhFirstTotal.ToString();
			}
			else if (displayName == "DHLast")
			{
				return line.DhLastTotal.ToString();
			}
			else if (displayName == "DHTotal")
			{
				return line.DhTotal.ToString();
			}
			else
			{
				return "";
			}
		}

		public static string FormatBidReceipt (string receipt)
		{
			StringBuilder resultHeadingString = new StringBuilder ();
			List<StringBuilder> content = new List<StringBuilder> ();

			List<PrintBidReceipt> lstPrintBidReceipt = new List<PrintBidReceipt> ();
			string lineString = string.Empty;
			string lineType = "header";

			resultHeadingString.Append ("WBidMax Formatted Bid Receipt" + Environment.NewLine);
			resultHeadingString.Append ("Employee Number :");

			int count = 1;

			using (StreamReader reader = new StreamReader (receipt)) {
				while ((lineString = reader.ReadLine ()) != null) {
					// lineString = reader.ReadLine();
					if (lineType == "header") {
						resultHeadingString.Append (lineString + Environment.NewLine);
						resultHeadingString.Append ("Bid Receipt File :" + receipt + Environment.NewLine);
						resultHeadingString.Append ("Receipt File Dated :" + DateTime.Now.ToLongDateString () + "(Local)" + Environment.NewLine + Environment.NewLine);
						lineType = "body";
					} else if (lineType == "body") {
						int num;
						if (int.TryParse (lineString.Substring (0, 1), out num)) {
							lstPrintBidReceipt.Add (new PrintBidReceipt () { LineOrder = count++, LineNum = lineString });

						} else {
							lineType = "footer";
							resultHeadingString.Append (lineString + Environment.NewLine);

						}

					} else if (lineType == "footer") {
						resultHeadingString.Append (lineString + Environment.NewLine + Environment.NewLine);

					}

				}

			}

			int startvalu = 0;
			int index = 0;
			int itemPercolumn = 67;

			StringBuilder singlePageContent = new StringBuilder ();
			string lineStr = string.Empty;
			int bidReceiptIndex;
			while (index + startvalu < lstPrintBidReceipt.Count) {
				for (int cnt = 0; cnt < 9; cnt++) {
					bidReceiptIndex = startvalu + index + (cnt * itemPercolumn);

					if (bidReceiptIndex < lstPrintBidReceipt.Count) {
						lineStr = lstPrintBidReceipt [bidReceiptIndex].LineOrder.ToString ().PadLeft (3, ' ') + ". " + lstPrintBidReceipt [bidReceiptIndex].LineNum;
						singlePageContent.Append (lineStr.PadRight (13, ' '));
					} else {
						break;
					}
				}
				singlePageContent.Append (Environment.NewLine);

				index++;
				if (index == itemPercolumn) {
					index = 0;
					startvalu = startvalu + index + (9 * itemPercolumn);
					itemPercolumn = 72;
					content.Add (singlePageContent);
					singlePageContent = new StringBuilder ();
				}
			}

			if (singlePageContent.ToString ().Trim () != string.Empty) {
				content.Add (singlePageContent);

			}

			PDfParams pdfParams = new PDfParams
			{
				Author = "WBidMax",
				Creator = "WBidmax",
				FileName = WBidHelper.GetAppDataPath() + "/" + "test.pdf",
				Subject = "Bid Receipt",
				Title = "Bid Receipt"
			};

			var str = "\n\n" + resultHeadingString.ToString () + "\n\n";
			str += string.Join ("", content.ToList ().ConvertAll (x => x.ToString ()));
			return str;
		}
        public static bool SaveFormatBidReceipt(string result)
		{
            try
            {
                //			    string result = string.Empty;
                //			result = "22028\n";
                //
                //			    for (int i = 10; i < 385; i++)
                //			    {
                //			        result += i + "\n";
                //			    }
                string lineString = string.Empty;
                string employeename = result.Substring(0, result.IndexOf("\n"));
                string fileName = employeename + "Rct.pdf";
                string footer = string.Empty;
                var lists = result.Split('*').ToList();
                lists.RemoveAt(0);
                foreach (var item in lists)
                {
                    footer += item;
                }
                StringBuilder resultHeadingString = new StringBuilder();

                List<StringBuilder> content = new List<StringBuilder>();

                List<PrintBidReceipt> lstPrintBidReceipt = new List<PrintBidReceipt>();
                var submit = result.Split('\n').ToList();
                submit.RemoveAt(0);
                int count = 1;
                foreach (string item in submit)
                {
                    if (item.Contains('*'))
                        break;
                    lstPrintBidReceipt.Add(new PrintBidReceipt() { LineOrder = count++, LineNum = item });

                }
                resultHeadingString.Append(lineString + Environment.NewLine + Environment.NewLine);

                int startvalu = 0;
                int index = 0;
                int itemPercolumn = 35;

                StringBuilder singlePageContent = new StringBuilder();
                singlePageContent.Append("WBidMax Formatted Bid Receipt" + Environment.NewLine);
                singlePageContent.Append("Employee Number : " + employeename + Environment.NewLine);
                singlePageContent.Append("Bid Receipt File :" + WBidHelper.GetAppDataPath() + "\\" + fileName + Environment.NewLine);
                singlePageContent.Append("Receipt File Dated :" + DateTime.Now.ToLongDateString() + "(Local)" + Environment.NewLine);
                singlePageContent.Append(footer + Environment.NewLine);
                string lineStr = string.Empty;
                int bidReceiptIndex;
                while (index + startvalu < lstPrintBidReceipt.Count)
                {
                    for (int cnt = 0; cnt < 7; cnt++)
                    {
                        bidReceiptIndex = startvalu + index + (cnt * itemPercolumn);

                        if (bidReceiptIndex < lstPrintBidReceipt.Count)
                        {
                            lineStr = lstPrintBidReceipt[bidReceiptIndex].LineOrder.ToString().PadLeft(3, ' ') + ". " + lstPrintBidReceipt[bidReceiptIndex].LineNum.PadRight(3, ' ');
                            singlePageContent.Append(lineStr.PadRight(19, ' '));
                        }
                        else
                        {
                            break;
                        }
                    }
                    singlePageContent.Append(Environment.NewLine);

                    index++;
                    if (index == itemPercolumn)
                    {
                        index = 0;
                        startvalu = startvalu + index + (7 * itemPercolumn);
						//itemPercolumn = 62;
						itemPercolumn = 42;
						content.Add(singlePageContent);
                        singlePageContent = new StringBuilder();
                    }
                }

                if (singlePageContent.ToString().Trim() != string.Empty)
                {
                    content.Add(singlePageContent);

                }

                PDfParams pdfParams = new PDfParams
                {
                    Author = "WBidMax",
                    Creator = "WBidmax",
                    FileName = WBidHelper.GetAppDataPath() + "/" + employeename + "Rct.pdf",
                    Subject = "Bid Receipt",
                    Title = "Bid Receipt"
                };
                CreatePDF(pdfParams, content);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;

		}

		//public static bool SaveFormattedBidReceiptForPilot(string submitresult)
		//{
		//	try
		//	{
		//		string lineString = string.Empty;
		//		string employeename = submitresult.Substring(0, submitresult.IndexOf("\n"));
		//		string fileName = employeename + "Rct.pdf";
		//		string footer = string.Empty;
		//		var lists = submitresult.Split('*').ToList();
		//		lists.RemoveAt(0);
		//		foreach (var item in lists)
		//		{
		//			footer += item;
		//		}

		//		StringBuilder resultHeadingString = new StringBuilder();

		//		List<StringBuilder> content = new List<StringBuilder>();

		//		List<PrintBidReceipt> lstPrintBidReceipt = new List<PrintBidReceipt>();
		//		var submit = submitresult.Split('\n').ToList();
		//		submit.RemoveAt(0);
		//		int count = 1;
		//		var lines = GlobalSettings.Lines;
		//		foreach (string item in submit)
		//		{
		//			if (item.Contains('*'))
		//				break;
		//			lstPrintBidReceipt.Add(new PrintBidReceipt() { LineOrder = count++, LineNum = item });

		//		}
		//		resultHeadingString.Append(lineString + Environment.NewLine + Environment.NewLine);

		//		List<PDFPage> lstPDfPages = new List<PDFPage>();

		//		PDFPage objPDFPage = new PDFPage();
		//		objPDFPage.Page = new List<SingleLine>();
		//		SingleLine objSingleLine = new SingleLine();
		//		objSingleLine.lstContent = new List<Content>();




		//		int startvalu = 0;
		//		int index = 0;
		//		int itemPercolumn = 53;


		//		StringBuilder singlePageContent = new StringBuilder();

		//		objSingleLine.lstContent.Add(new Content { Data = "WBidMax Formatted Bid Receipt " });
		//		objPDFPage.Page.Add(objSingleLine);

		//		objSingleLine = new SingleLine();
		//		objSingleLine.lstContent = new List<Content>();
		//		objSingleLine.lstContent.Add(new Content { Data = "Employee Number : " + employeename });
		//		objPDFPage.Page.Add(objSingleLine);

		//		objSingleLine = new SingleLine();
		//		objSingleLine.lstContent = new List<Content>();
		//		objSingleLine.lstContent.Add(new Content { Data = "Bid Receipt File :" + WBidHelper.GetAppDataPath() + "\\" + fileName });
		//		objPDFPage.Page.Add(objSingleLine);

		//		objSingleLine = new SingleLine();
		//		objSingleLine.lstContent = new List<Content>();
		//		objSingleLine.lstContent.Add(new Content { Data = "Receipt File Dated :" + DateTime.Now.ToLongDateString() + "(Local)" });
		//		objPDFPage.Page.Add(objSingleLine);

		//		objSingleLine = new SingleLine();
		//		objSingleLine.lstContent = new List<Content>();
		//		objSingleLine.lstContent.Add(new Content { Data = footer });
		//		objPDFPage.Page.Add(objSingleLine);

		//		Content objContent = new Content();

		//		objContent = new Content();

		//		string lineStr = string.Empty;
		//		int bidReceiptIndex;
		//		while (index + startvalu < lstPrintBidReceipt.Count)
		//		{
		//			objSingleLine = new SingleLine();
		//			objSingleLine.lstContent = new List<Content>();
		//			for (int cnt = 0; cnt < 7; cnt++)
		//			{
		//				bidReceiptIndex = startvalu + index + (cnt * itemPercolumn);

		//				if (bidReceiptIndex < lstPrintBidReceipt.Count)
		//				{
		//					lineStr = lstPrintBidReceipt[bidReceiptIndex].LineOrder.ToString().PadLeft(3, ' ') + ". " + lstPrintBidReceipt[bidReceiptIndex].LineNum.PadRight(3, ' ');


		//					//singlePageContent.Append(lineStr.PadRight(15, ' '));
		//					objContent = new Content();
		//					objContent.Data = lineStr.PadRight(15, ' ');
		//					int lineNUm = Convert.ToInt32(lstPrintBidReceipt[bidReceiptIndex].LineNum);
		//					var liendata = lines.FirstOrDefault(x => x.LineNum == lineNUm);

		//					if (liendata != null)
		//					{
		//						objContent.Blank = liendata.BlankLine;
		//						objContent.Reserve = liendata.ReserveLine;

		//					}

		//					objSingleLine.lstContent.Add(objContent);
		//				}
		//				else
		//				{
		//					break;
		//				}
		//			}
		//			objPDFPage.Page.Add(objSingleLine);


		//			index++;
		//			if (index == itemPercolumn)
		//			{
		//				index = 0;
		//				startvalu = startvalu + index + (7 * itemPercolumn);
		//				itemPercolumn = 62;

		//				lstPDfPages.Add(objPDFPage);

		//				objPDFPage = new PDFPage();
		//				objPDFPage.Page = new List<SingleLine>();

		//			}


		//		}

		//		if (singlePageContent.ToString().Trim() != string.Empty)
		//		{
		//			lstPDfPages.Add(objPDFPage);
		//			// content.Add(singlePageContent);

		//		}
				
		//		PDfParams pdfParams = new PDfParams
		//		{
		//			Author = "WBidMax",
		//			Creator = "WBidmax",
		//			FileName = fileName,
		//			Subject = "Bid Receipt",
		//			Title = "Bid Receipt"
		//		};

		//		CreatePDF(pdfParams, lstPDfPages);
		//	}
		//	catch (Exception ex)
		//	{
		//		return false;
		//	}
		//	return true;
		//}

		public static void CreatePDF(PDfParams PDfParams,List<StringBuilder> ls)
		{
			using (PdfWriter pdfWriter = new PdfWriter(PDfParams.FileName))
			using (PdfDocument pdfDocument = new PdfDocument(pdfWriter))
			using (Document document1 = new Document(pdfDocument))
			{
				PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
			
				foreach (var item in ls)
				{
					//document1.Add(new Paragraph(item.ToString()));
					document1.Add(new Paragraph(item.ToString()).SetFont(font));
			    }
				
			}
			//FileStream fileStream = new FileStream(PDfParams.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
			//Document document = new Document();

			//if (!String.IsNullOrEmpty(PDfParams.Title))
			//{
			//	document.AddTitle(PDfParams.Title);
			//}
			//if (!String.IsNullOrEmpty(PDfParams.Subject))
			//{
			//	document.AddSubject(PDfParams.Subject);
			//}
			//if (!String.IsNullOrEmpty(PDfParams.Creator))
			//{
			//	document.AddCreator(PDfParams.Creator);
			//}
			//if (!String.IsNullOrEmpty(PDfParams.Author))
			//{
			//	document.AddAuthor(PDfParams.Author);
			//}
			////document.AddHeader("Nothing", "No Header");
			//PdfWriter writer = PdfWriter.GetInstance(document, fileStream);

			//document.Open();

			//iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER,8f,0);


			//foreach (var item in ls)
			//{
			//	document.Add(new Paragraph(item.ToString(), font));
			//	document.NewPage();
			//}

			//document.Close();

		}

		public void CreatePDF(PDfParams PDfParams, List<PDFPage> lstData)
		{
			try
			{
				
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}
		#endregion


	}
	public class PDfParams
	{

		public string FileName { get; set; }

		public StringBuilder FileContent { get; set; }

		public string Title { get; set; }

		public string Subject { get; set; }

		public string Creator { get; set; }

		public string Author { get; set; }




	}
	public class PDFPage
	{
		public List<SingleLine> Page { get; set; }
	}
	public class SingleLine
	{
		public List<Content> lstContent { get; set; }

	}

	public class Content
	{
		public string Data { get; set; }
		public bool Blank { get; set; }
		public bool Reserve { get; set; }
	}
}

