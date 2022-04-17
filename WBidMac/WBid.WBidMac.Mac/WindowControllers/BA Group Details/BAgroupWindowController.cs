
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using System.Collections.Generic;
using WBid.WBidiPad.Core;

//using System.Linq;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary;

//using MonoTouch.EventKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidMac.Mac
{
	public partial class BAgroupWindowController : AppKit.NSWindowController
	{

		#region  properties
		private ObservableCollection<AppliedStates> _allAppliedStates;
		public ObservableCollection<AppliedStates> AllAppliedStates
		{
			get
			{
				return _allAppliedStates;
			}
			set
			{
				_allAppliedStates = value;

			}
		}

		#region Constructors
		WBidState wBidStateContent;
		public List <string> lstGroups;
		public List <string> lstDetails;
		public ObservableCollection <DateTime> lstDates = new ObservableCollection<DateTime> ();
		ObservableCollection<TripData> tripData = new ObservableCollection<TripData> ();
		public string SelectedGroup;
		public string SelectedTripName;
		public DateTime SelectedDate;
		private ObservableCollection<string> _groupNameList;
		public ObservableCollection<string> GroupNameList
		{
			get
			{
				return _groupNameList;
			}
			set
			{
				_groupNameList = value;

			}
		}
		private string _selectedGroupName;
		public string SelectedGroupName
		{
			get
			{
				return _selectedGroupName;
			}
			set
			{

				_selectedGroupName = value;
				SetContent();

			}
		}
		private int _totalLines;
		public int TotalLines
		{
			get
			{
				return _totalLines;
			}
			set
			{

				_totalLines = value;

			}
		}
		#endregion
		// Called when created from unmanaged code
		public BAgroupWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BAgroupWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BAgroupWindowController () : base ("BAgroupWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new BAgroupWindow Window {
			get {
				return (BAgroupWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.ShouldCascadeWindows = false;
			wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			btnFFDO.Hidden = !(GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO");
			this.Window.WillClose += delegate {
				this.Window.OrderOut (this);
				NSApplication.SharedApplication.StopModal ();
			};

			lstGroups = GlobalSettings.Trip.Select (x => x.TripNum).ToList ();

		

			//txtPairing.Font = NSFont.FromFontName ("Courier", 12);
			btnPrint.Activated += btnPrintClicked;
			btnFFDO.Activated += btnFFDOClicked;

			// load datas

			AllAppliedStates = new ObservableCollection<AppliedStates>();


			if (wBidStateContent.CalculatedBA!=null && wBidStateContent.CalculatedBA.BAGroup!=null)
			{
				GroupNameList = new ObservableCollection<string>(wBidStateContent.CalculatedBA.BAGroup.Select(x => x.GroupName).Distinct());
				//GroupNameList = new ObservableCollection<string>(ServiceLocator.Current.GetInstance<MainViewModel>().Lines.Select(x => x.BAGroup).Distinct());

				SelectedGroupName = GroupNameList.FirstOrDefault();
			}
			settableContents ();

			tblGroups.Source = new TGroupNameTableSource (this);
			tblGroupDetails.Source = new TGroupDetailsTableSource (this);

			var selectedLine=	GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine);
			if(selectedLine != null)
				tblGroups.SelectRow (GroupNameList.IndexOf (selectedLine.BAGroup), false);
			//tblGroupDetails.SelectRow (NSIndexPath.FromIndex (1), false, nstablevie.None);
			SetGroupName (selectedLine.BAGroup);
		}

		public void settableContents()
		{
			lstDetails= new List<string>();
			if (AllAppliedStates != null) {
				for (int mainitem = 0; mainitem < AllAppliedStates .Count; mainitem++) {
					lstDetails.Add ("Header|" + AllAppliedStates [mainitem].Key);
					for (int item = 0; item < AllAppliedStates [mainitem].AppliedStateTypes.Count; item++) {

						string concat="\n\n\n";
						if(AllAppliedStates [mainitem].AppliedStateTypes [item].Value !=null)
							concat = String.Join("\n", AllAppliedStates [mainitem].AppliedStateTypes [item].Value.ToArray());
						lstDetails.Add (AllAppliedStates [mainitem].AppliedStateTypes [item].Key + "|" + concat);
					}
				}
			}

		}
		public void SetGroupName(string grpName)
		{
			SelectedGroupName = GroupNameList.FirstOrDefault(x=>x==grpName);

			GroupDetailViewBox.Title = "Group : " + SelectedGroupName + "      Total Lines : " + TotalLines.ToString ();
			SetContent ();
			settableContents ();
			tblGroupDetails.ReloadData ();
		}

		void btnFFDOClicked (object sender, EventArgs e)
		{
			try {
//				//string ffdoData = GetFlightDataforFFDB (lstTrips[(int)tblGroups.SelectedRow], lstDates[(int)tblDays.SelectedRow].Date);
//				NSPasteboard clipBoard = NSPasteboard.GeneralPasteboard;
//				clipBoard.DeclareTypes (new string[]{ "NSStringPboardType" }, null);
//				clipBoard.SetStringForType (ffdoData, "NSStringPboardType");
//				var alert = new NSAlert ();
//				alert.MessageText = "WBidMax";
//				alert.InformativeText = "Trips has been copied to ClipBoard!";
//				alert.RunModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		/// <summary>
		/// PURPOSE : Get Flight data for FFDB
		/// </summary>
		/// <param name="trip"></param>
		/// <param name="tripName"></param>
		private string GetFlightDataforFFDB(string tripNum,DateTime tripDate)
		{

			Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum.Substring(0,4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum);
			string result = string.Empty;

			// var tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(SelectedDay.Replace(" ", "")));
			//DateTime dutPeriodDate = WBidCollection.SetDate(int.Parse(tripNum.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
			DateTime dutPeriodDate = tripDate;

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

		void btnPrintClicked (object sender, EventArgs e)
		{
			try {
				var content ="";
				var inv = new InvisibleWindowController ();
				this.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
				var txt = new NSTextView (new CGRect (0, 0, 550, 550));
				txt.Font = NSFont.FromFontName ("Courier", 10);
				inv.Window.ContentView.AddSubview (txt);
				txt.Value = content;
				var pr = NSPrintInfo.SharedPrintInfo;
				pr.VerticallyCentered = false;
				pr.TopMargin = 2.0f;
				pr.BottomMargin = 2.0f;
				pr.LeftMargin = 1.0f;
				pr.RightMargin = 1.0f;
				txt.Print (this);
				inv.Close ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void GenerateDates (string gtripOpVector)
		{

			//if (gtripOpVector.Trim() == string.Empty)
			if (string.IsNullOrEmpty (gtripOpVector)) {
				return;
			}
			DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
			foreach (var item in gtripOpVector) {
				if (startDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
					break;
				if (item == '1') {
					lstDates.Add (startDate);
				}
				startDate = startDate.AddDays (1);
			}
		}
		//Bid automator
		#region Private Methods
		private void SetContent()
		{
			if (SelectedGroupName != string.Empty)
			{
				int totLines = 0;
				//TotalLines = ServiceLocator.Current.GetInstance<MainViewModel>().Lines.Where(x => x.BAGroup == SelectedGroupName).Count();
				var grpInfo = wBidStateContent.CalculatedBA.BAGroup.FirstOrDefault(x => x.GroupName == SelectedGroupName);
				if (grpInfo != null)
				{
					totLines = (grpInfo.Lines == null) ? 0 : grpInfo.Lines.Count();
				}
				TotalLines = totLines;
				AllAppliedStates = new ObservableCollection<AppliedStates>();
				SetSortInformation();
				SetFilterInformation();
			}
		}

		private void SetSortInformation()
		{
			AppliedStates sortState = new AppliedStates();
			sortState.Key = "Sort Options";

			sortState.AppliedStateTypes = new List<AppliedStateType>();
			AppliedStateType appliedStateType = new AppliedStateType();
			if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BASort != null)
			{
				var currentsort = wBidStateContent.CalculatedBA.BASort.SortColumn;

				if (currentsort == "LinePay")
					appliedStateType.Key = "Bottom Line Pay Per Total";
				else if (currentsort == "PayPerDay")
					appliedStateType.Key = "Most Pay Per Day";
				else if (currentsort == "PayPerDutyHour")
					appliedStateType.Key = "Most Pay Duty Hour";
				else if (currentsort == "PayPerFlightHour")
					appliedStateType.Key = "Most Pay Per Flight Hour";
				else if (currentsort == "PayPerTimeAway")
					appliedStateType.Key = "Most Pay Per Time Away From Base";
				else if (currentsort == "BlockSort")
					appliedStateType.Key = "Block Sort";


				if (appliedStateType.Key == "Block Sort")
				{
					//get the Block sort names from the block sort code stored in the global settings value
					List<string> blockSortitems = new List<string>();
					wBidStateContent.CalculatedBA.BASort.BlokSort.ForEach(x =>
						{
							if (x != string.Empty)
								blockSortitems.Add(WBidCollection.GetBlockSortListData().First(y => y.Id.ToString() == x).Name);
						});
					appliedStateType.Value = blockSortitems;

				}


				sortState.AppliedStateTypes.Add(appliedStateType);

				AllAppliedStates.Add(sortState);
			}

		}

		private void SetFilterInformation()
		{
			AppliedStates filterState = new AppliedStates();
			filterState.Key = "Filters";

			filterState.AppliedStateTypes = new List<AppliedStateType>();

			AppliedStateType appliedStateType = null; ;
			if (wBidStateContent.CalculatedBA != null && wBidStateContent.CalculatedBA.BAFilter != null && wBidStateContent.CalculatedBA.BAFilter.Count() > 0)
			{
				int filterCount = wBidStateContent.CalculatedBA.BAFilter.Count();
				string groupNum = SelectedGroupName.Replace("G", "");
				int grpNum = groupNum == string.Empty ? 0 : int.Parse(groupNum);
				int totalCombinations = (int)Math.Pow(2, filterCount);
				string combinationString = Convert.ToString((totalCombinations) - grpNum, 2).PadLeft(filterCount, '0');

				for (int pos = 0; pos < combinationString.Length; pos++)
				{
					if (combinationString[pos] == '1')
					{
						appliedStateType = new AppliedStateType();
						if (wBidStateContent.CalculatedBA.BAFilter.Count > pos)
						{
							appliedStateType.Key = StateFilterNameToAcualName(wBidStateContent.CalculatedBA.BAFilter[pos].Name);
							appliedStateType.Value = SetFilterParameterFromState(wBidStateContent.CalculatedBA.BAFilter[pos]);
							filterState.AppliedStateTypes.Add(appliedStateType);
						}
					}
				}





				AllAppliedStates.Add(filterState);
			}

		}

		private List<string> SetFilterParameterFromState(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			switch (bidAutoItem.Name)
			{


			case "AP":
				tempString = ReadAMPMStateContent(bidAutoItem);
				break;
			case "CL":
				tempString = ReadCommutableLineStateContent(bidAutoItem);
				break;
			case "DOM":
				tempString = ReadDaysOfMonthStateContent(bidAutoItem);
				break;
			case "DOWA":
				tempString = ReadDaysOfWeekAllStateContent(bidAutoItem);
				break;
			case "DOWS":
				tempString = ReadDaysOfWeekSomeStateContent(bidAutoItem);
				break;
			case "DHFL":
				tempString =ReadDeadHeadFirstLastStateContent(bidAutoItem);
				break;
			case "ET":
				tempString=ReadEquipmentTypeStateContent(bidAutoItem);
				break;
			case "LT":
				tempString = ReadLineTypeStateContent(bidAutoItem);
				break;

			case "OC":
				tempString = ReadOvernightCitiesStateContent(bidAutoItem);
				break;
			case "RT":
				tempString=ReadRestTypeStateContent(bidAutoItem);
				break;
			case "SDOW":
				tempString = ReadStartDayOfOfWeekAllStateContent(bidAutoItem);
				break;
			case "TBL":
				tempString = ReadTripBlockLengthStateContent(bidAutoItem);
				break;


			}
			return tempString;

		}

		private List<string> ReadAMPMStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			var ampmitem = (AMPMConstriants)bidAutoItem.BidAutoObject;
			if (ampmitem.AM)
			{
				str = "AM";
			}

			if (ampmitem.PM)
			{
				if (str != string.Empty)
					str += " , ";
				str += "PM";
			}
			if (ampmitem.MIX)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Mix";
			}

			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadDaysOfMonthStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			DaysOfMonthCx daysOfMonthState = (DaysOfMonthCx)bidAutoItem.BidAutoObject;
			List<WBid.WBidiPad.Model.DaysOfMonth> lstDaysOfMonth =WBidCollection.GetDaysOfMonthList().ToList();

			WBid.WBidiPad.Model.DaysOfMonth result;

			//Work days
			if (daysOfMonthState.WorkDays != null && daysOfMonthState.WorkDays.Count > 0)
			{

				foreach (int id in daysOfMonthState.WorkDays)
				{
					result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
					if (result != null)
					{
						if (str != string.Empty)
							str += " , ";
						str += result.Date.ToString("MM/dd/yyyy");
					}

				}
				tempString.Add("Work Days :- ");
				tempString.Add(str);


			}

			//Off Days
			if (daysOfMonthState.OFFDays != null && daysOfMonthState.OFFDays.Count > 0)
			{

				foreach (int id in daysOfMonthState.OFFDays)
				{
					result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
					if (str != string.Empty)
						str += " , ";
					str += result.Date.ToString("MM/dd/yyyy");

				}

				if (tempString.Count > 0)
				{
					tempString.Add("");
				}

				tempString.Add("Off Days :- ");
				tempString.Add(str);
			}



			return tempString;
		}

		private List<string> ReadDaysOfWeekAllStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			CxDays cxDay = (CxDays)bidAutoItem.BidAutoObject;
			if (cxDay.IsSun)
			{
				str = "Sunday";
			}

			if (cxDay.IsMon)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Monday";
			}
			if (cxDay.IsTue)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Tuesday";
			}

			if (cxDay.IsWed)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Wednesday";
			}

			if (cxDay.IsThu)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Thursday";
			}
			if (cxDay.IsFri)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Friday";
			}
			if (cxDay.IsSat)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Saturday";
			}

			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadDaysOfWeekSomeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell=string.Empty;
			string type = string.Empty;
			switch(int.Parse( cx3Parameter.ThirdcellValue))
			{
			case  (int)DayofTheWeek.Monday:
				thirdCell="Monday";
				break;
			case (int)DayofTheWeek.Tuesday:
				thirdCell = "Tuesday";
				break;
			case (int)DayofTheWeek.Wednesday:
				thirdCell = "Wednesday";
				break;
			case (int)DayofTheWeek.Thursday :
				thirdCell = "Thursday";
				break;
			case (int)DayofTheWeek.Friday:
				thirdCell = "Friday";
				break;
			case (int)DayofTheWeek.Saturday:
				thirdCell = "Saturday";
				break;
			case (int)DayofTheWeek.Sunday:
				thirdCell = "Sunday";
				break;


			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type="less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadDeadHeadFirstLastStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell = string.Empty;
			string type = string.Empty;
			switch (int.Parse(cx3Parameter.ThirdcellValue))
			{
			case (int)DeadheadType.First:
				thirdCell = "First";
				break;
			case (int)DeadheadType.Last:
				thirdCell = "Last";
				break;
			case (int)DeadheadType.Both:
				thirdCell = "Both";
				break;



			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type = "less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadEquipmentTypeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell = string.Empty;
			string type = string.Empty;
			switch (int.Parse(cx3Parameter.ThirdcellValue))
			{
				case 300:
					thirdCell = "300";
					break;
				case 500:
					thirdCell = "300";
					break;
				case 35:
					//thirdCell = "300 & 500";
					thirdCell = "300";
					break;
				case 700:
					thirdCell = "700";
					break;
				case 800:
					thirdCell = "800";
					break;
				case 600:
					thirdCell = "8Max";
					break;
				case 200:
					thirdCell = "7Max";
					break;


			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type = "less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadLineTypeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			var lineTypeItem = (CxLine)bidAutoItem.BidAutoObject;
			if (lineTypeItem.Hard)
			{
				str = "Hard";
			}

			if (lineTypeItem.Reserve)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Reserve";
			}

			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && lineTypeItem.Ready)
			{ 
				if (str != string.Empty)
					str += " , ";
				str+= "Ready";

			}
			else if (GlobalSettings.CurrentBidDetails.Postion != "FA" && lineTypeItem.Blank)
			{

				if (str != string.Empty)
					str += " , ";
				str+= "Blank";

			}


			if (lineTypeItem.International)
			{
				if (str != string.Empty)
					str += " , ";
				str += "International";
			}

			if (lineTypeItem.NonConus)
			{
				if (str != string.Empty)
					str += " , ";
				str += "NonConus";
			}



			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadStartDayOfOfWeekAllStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			CxDays cxDay = (CxDays)bidAutoItem.BidAutoObject;
			if (cxDay.IsSun)
			{
				str = "Sunday";
			}

			if (cxDay.IsMon)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Monday";
			}
			if (cxDay.IsTue)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Tuesday";
			}

			if (cxDay.IsWed)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Wednesday";
			}

			if (cxDay.IsThu)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Thursday";
			}
			if (cxDay.IsWed)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Friday";
			}
			if (cxDay.IsSat)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Saturday";
			}

			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadOvernightCitiesStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			BulkOvernightCityCx bulkOvernightCityCx = (BulkOvernightCityCx)bidAutoItem.BidAutoObject;
			if (bulkOvernightCityCx.OverNightYes != null && bulkOvernightCityCx.OverNightYes.Count > 0)
			{
				List<string> lstYesCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightYes.Contains(x.Id)).Select(y => y.Name).ToList();
				if (lstYesCityNames.Count() > 0)
				{
					tempString.Add("YES :-");

					foreach (var item in lstYesCityNames)
					{
						if (str != string.Empty)
							str += " , ";
						str += item;

					}
					tempString.Add(str);

				}

			}


			if (bulkOvernightCityCx.OverNightNo != null && bulkOvernightCityCx.OverNightNo.Count > 0)
			{
				List<string> lstNoCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightNo.Contains(x.Id)).Select(y => y.Name).ToList();
				if (lstNoCityNames.Count() > 0)
				{
					if (tempString.Count() > 0)
					{
						tempString.Add("");
					}

					tempString.Add("No :-");

					foreach (var item in lstNoCityNames)
					{
						if (str != string.Empty)
							str += " , ";
						str += item;

					}
					tempString.Add(str);

				}
			}




			return tempString;
		}

		private List<string> ReadRestTypeStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			Cx3Parameter cx3Parameter = (Cx3Parameter)bidAutoItem.BidAutoObject;
			string thirdCell = string.Empty;
			string type = string.Empty;
			switch (int.Parse(cx3Parameter.ThirdcellValue))
			{
			case (int)RestType.All:
				thirdCell = "All";
				break;
			case (int)RestType.InDomicile:
				thirdCell = "InDomicile";
				break;
			case (int)RestType.AwayDomicile:
				thirdCell = "AwayDomicile";
				break;



			}


			switch (cx3Parameter.Type)
			{
			case (int)ConstraintType.LessThan:
				type = "less than";
				break;
			case (int)ConstraintType.EqualTo:
				type = "equal";
				break;

			case (int)ConstraintType.MoreThan:
				type = "more than";
				break;

			}


			str = thirdCell + " " + type + " " + cx3Parameter.Value;


			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadTripBlockLengthStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;
			var tripBlockLengthItem = (CxTripBlockLength)bidAutoItem.BidAutoObject;
			if (tripBlockLengthItem.IsBlock)
			{
				tempString.Add("Block");
			}
			else
			{
				tempString.Add("Trip");
			}

			if (tripBlockLengthItem.Turns)
			{

				str += "Turns";
			}




			if (tripBlockLengthItem.Twoday)
			{
				if (str != string.Empty)
					str += " , ";
				str += "Twoday";
			}

			if (tripBlockLengthItem.ThreeDay)
			{
				if (str != string.Empty)
					str += " , ";
				str += "ThreeDay";
			}


			if (tripBlockLengthItem.FourDay)
			{
				if (str != string.Empty)
					str += " , ";
				str += "FourDay";
			}



			tempString.Add(str);
			return tempString;
		}

		private List<string> ReadCommutableLineStateContent(BidAutoItem bidAutoItem)
		{
			List<string> tempString = new List<String>();
			string str = string.Empty;

			var ftCommutableLine = (FtCommutableLine)bidAutoItem.BidAutoObject;

			str = "Commute City\t\t"+ ": " + ftCommutableLine.City;
			tempString.Add(str);

			str = "Check-in Pad Time\t" + ": " + ConvertMinuteToHHMM(ftCommutableLine.CheckInTime);
			tempString.Add(str);

			str = "Back-to-Base Pad Time\t" + ": " + ConvertMinuteToHHMM(ftCommutableLine.BaseTime);
			tempString.Add(str);

			str = "Connect Time\t\t" + ": " + ConvertMinuteToHHMM(ftCommutableLine.ConnectTime);
			tempString.Add(str);

			str = "No Nights in middle\t" + ": " + ((ftCommutableLine.NoNights) ? "TRUE" : "FALSE");
			tempString.Add(str);

			str = string.Empty;
			if (ftCommutableLine.ToHome)
			{
				str += "To Home";
			}
			if (ftCommutableLine.ToWork)
			{
				if (str != string.Empty)
					str += " , ";
				str += "To Work";
			}
			tempString.Add(str);
			return tempString;
		}

		private string ConvertMinuteToHHMM(int minute)
		{
			string result = string.Empty;
			result = Convert.ToString(minute / 60).PadLeft(2, '0');
			result += ":";
			result += Convert.ToString(minute % 60).PadLeft(2, '0');
			return result;


		}

		private string StateFilterNameToAcualName(string name)
		{
			string actaulName = string.Empty;

			switch (name)
			{
			case "AP":
				actaulName = "Am - Pm";
				break;
			case "CL":
				actaulName = "Commutable Lines";
				break;
			case "DOM":
				actaulName = "Days of the Month";
				break;
			case "DOWA":
				actaulName = "Days of the Week - All";
				break;
			case "DOWS":
				actaulName = "Days of the Week - Some";
				break;
			case "DHFL":
				actaulName = "DH First - Last";
				break;
			case "ET":
				actaulName = "Equipment Type";
				break;
			case "LT":
				actaulName = "Line Type";
				break;
			case "OC":
				actaulName = "Overnight Cities";
				break;
			case "RT":
				actaulName = "Rest";
				break;

			case "SDOW":
				actaulName = "Start Day of Week";
				break;
			case "TBL":
				actaulName = "Trip-Block Length";
				break;
			}

			return actaulName;

		}

		public void ReloadContent()
		{
			tblGroupDetails.ReloadData ();
		}

	}


	public partial class TGroupNameTableSource : NSTableViewSource
	{
		BAgroupWindowController parentVC;

		public TGroupNameTableSource (BAgroupWindowController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return  parentVC.GroupNameList.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return new NSString ( parentVC.GroupNameList[(int)row]);		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;

			parentVC.SelectedGroupName = parentVC.GroupNameList[(int)table.SelectedRow];
			parentVC.SetGroupName (parentVC.GroupNameList [(int)table.SelectedRow]);

			//parentVC.tblDayView.ReloadData ();
			//parentVC.ReloadContent ();

//			if (table.SelectedRowCount > 0) {
//				parentVC.SelectedTripName = parentVC.lstGroups [(int)table.SelectedRow];
//				parentVC.GeneratePairingDetails ();
//			
//			}
		}



//	public partial class DaysTableSource : NSTableViewSource
//	{
//		PairingWindowController parentVC;
//
//		public DaysTableSource (PairingWindowController parent)
//		{
//			parentVC = parent;
//		}
//
//		public override nint GetRowCount (NSTableView tableView)
//		{
//			return parentVC.lstDates.Count;
//		}
//
//		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
//		{
//			return new NSString (parentVC.lstDates [(int)row].Day.ToString ());		
//		}
//
//		public override void SelectionDidChange (NSNotification notification)
//		{
//			var table = (NSTableView)notification.Object;
//			if (table.SelectedRowCount > 0) {
//				parentVC.SelectedDate = parentVC.lstDates [(int)table.SelectedRow];
//				parentVC.GeneratePairingDetails ();
//			}
//		}
//	}
	#endregion
}


	public partial class TGroupDetailsTableSource : NSTableViewSource
	{
		BAgroupWindowController parentVC;

		public TGroupDetailsTableSource (BAgroupWindowController parent)
		{
			parentVC = parent;
		}

//		public override nint GetRowCount (NSTableView tableView)
//		{
//			int count = 0;
//			for(int items=0; items <parentVC.AllAppliedStates.Count ; items++)
//			{
//				count += parentVC.AllAppliedStates [0].AppliedStateTypes.Count;
//			}
//
//			return count+2;
//		}
		public override nint GetRowCount (NSTableView tableView)
		{
			
			return (nint)parentVC.lstDetails.Count;
		}

//		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
//		{
//			return parentVC.AllAppliedStates[(int)section].AppliedStateTypes.Count;;	
//		}

//		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
//		{
//			
//			var vw = (BaGroupCell)tableView.MakeView ("Cell", this);;
//			int section = 0;
//			int typesRow=0;
//			if (parentVC.AllAppliedStates [0].AppliedStateTypes.Count + 1 <= row)
//				section = 1;
//			
//
//			if (row == 0 || row == parentVC.AllAppliedStates [0].AppliedStateTypes.Count + 1) {
//				vw = (BaGroupCell)tableView.MakeView ("Header", this);
//
//				vw.BindData ("Header", (int)row, parentVC,parentVC.AllAppliedStates [section].AppliedStateTypes ,parentVC.AllAppliedStates [section].Key);
//			} else 
//			{
//				vw = (BaGroupCell)tableView.MakeView ("Cell", this);
//				if (section == 0)
//					typesRow = (int)row - 1;
//				else
//					typesRow =(int) row - parentVC.AllAppliedStates [0].AppliedStateTypes.Count + 2;
//				vw.BindData ("Cell", (int)typesRow, parentVC,parentVC.AllAppliedStates [section].AppliedStateTypes,parentVC.AllAppliedStates [section].Key);
//			}
//			return vw;
//		}
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{

			var vw = (BaGroupCell)tableView.MakeView ("Cell", this);;

			string[] values = parentVC.lstDetails[(int)row].Split('|');

			if (values [0] == "Header") {
				vw = (BaGroupCell)tableView.MakeView ("Header", this);
				vw.BindData ("Header", values [0], values [1]);
			} else {
				vw = (BaGroupCell)tableView.MakeView ("Cell", this);
				vw.BindData ("Cell", values [0], values [1]);
			}


		
			return vw;
		}
		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			string[] values = parentVC.lstDetails[(int)row].Split('|');
			if (values [0] == "Header") {
				return 40;
			} else if (values [0] == "Commutable Lines" || values [0].ToLower() == "block sort") {
				return 100;
			}
			else return 40;
		}
//		public override void SelectionDidChange (NSNotification notification)
//		{
//			var table = (NSTableView)notification.Object;
//			if (table.SelectedRowCount > 0) {
//				parentVC.SelectedTripName = parentVC.lstGroups [(int)table.SelectedRow];
//				parentVC.GeneratePairingDetails ();
//
//			}
//		}
	}
}
