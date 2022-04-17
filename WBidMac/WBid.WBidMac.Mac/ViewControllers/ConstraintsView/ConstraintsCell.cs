using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;

//using System.Linq;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

//using System.Collections.Generic;
using WBid.WBidiPad.Core.Enum;
using System.Text.RegularExpressions;

namespace WBid.WBidMac.Mac
{
	public partial class ConstraintsCell : AppKit.NSTableCellView
	{
		public ConstraintsCell ()
		{

		}

		int order;

		#region Constructors

		public static WBidState wBIdStateContent;
		// = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		static WBidIntialState wbidintialState;
		// = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

		// Constraints Param Popover
		string[] arrCompString1 = { "less than", "more than" };
		string[] arrCompString2 = { "less than", "more than", "equal to" };
		string[] arrCompString3 = { "at+after", "at+before" };
		string[] arrStartday = {"Block","Trip" };

		string[] arrCompString4 = { "Intl", "NonConus" };
		string[] arrDays = { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };
		string[] arrFirstLast = { "first", "last", "both" };
		string[] arrEQType = {"700", "800","7Max","8Max" };
		string[] arrRest = { "All", "InDom", "AwayDom" };
        string[] arrStartDayType = { "Start On", "Does Not Start" };

		string[] arrTripLenth = { "1Day", "2Day", "3Day", "4Day" };
		//		string[] arrCmut = {"begin","end","both"};
		string[] arrCmut2 = { "begin", "end", "either", "both" };
		string[] arrCompString5 = { "NO 3-on-30ff", "Only 3-on-3-off" };
        string [] arrCmtbltyThrdcell = { "<=", ">=" };
        string[] arrCmtbltyFirstcell = { "No Middle", "Ok Middle" };
        string [] arrCmtbltyScndcell = { "Front", "Back","Overall" };

		List<int> arrAirCraft = Enumerable.Range (0, 20 - 0 + 1).ToList ();
		List<int> arrBDO = Enumerable.Range (3, 31 - 3 + 1).ToList ();
		string[] arrDutyPeriodValue = {
			"08:00",
			"08:15",
			"08:30",
			"08:45",
			"09:00",
			"09:15",
			"09:30",
			"09:45",
			"10:00",
			"10:15",
			"10:30",
			"10:45",
			"11:00",
			"11:15",
			"11:30",
			"11:45",
			"12:00",
			"12:15",
			"12:30",
			"12:45",
			"13:00",
			"13:15",
			"13:30",
			"13:45",
			"14:00",
			"14:15",
			"14:30",
			"14:45",
			"15:00",
			"15:15",
			"15:30",
			"15:45",
			"16:00"
		};
		List<int> arrFlightTimeValue = Enumerable.Range (50, 120 - 50 + 1).ToList ();
		string[] arrGrndTimeValue = {
			"00:30",
			"00:45",
			"01:00",
			"01:15",
			"01:30",
			"01:45",
			"02:00",
			"02:15",
			"02:30",
			"02:45",
			"03:00",
			"03:15",
			"03:30",
			"03:45",
			"04:00"
		};
		List<int> arrlegsPerPeriodValue = Enumerable.Range (1, 8 - 1 + 1).ToList ();
		List<int> arrlegsPerPairingValue = Enumerable.Range (1, 32 - 1 + 1).ToList ();
		List<int> arrNumOfDaysOffValue = Enumerable.Range (10, 31 - 10 + 1).ToList ();
		List<int> arrTimeAwayFrmBaseValue = Enumerable.Range (200, 400 - 200 + 1).ToList ();
		List<int> arrMinPayValue = Enumerable.Range (60, 130 - 60 + 1).ToList ();
		//		int[] arrNo3on3offValue = Enumerable.Range (1, 7-1+1).ToArray ();
		//		int[] arrNoOverlapValue = Enumerable.Range (0, 4-0+1).ToArray ();
		List<int> arrDOWValue = Enumerable.Range (1, 6 - 1 + 1).ToList ();
		List<int> arrDHFOLValue = Enumerable.Range (0, 5 - 0 + 1).ToList ();
		List<int> arrEQTypeValue = Enumerable.Range (0, 20 - 0 + 1).ToList ();
		List<int> arrStartDayValue = Enumerable.Range (0, 6 - 0 + 1).ToList ();
		List<int> arrRestValue = Enumerable.Range (8, 48 - 8 + 1).ToList ();
        List<int> startDayValue = Enumerable.Range(1, 31).ToList();
		List<DateHelper> lstPDODays = ConstraintBL.GetPartialDayList ();
		List<string> lstPDOTimes = ConstraintBL.GetPartialTimeList ();
		List<int> arrGrndTimeValue2 = Enumerable.Range (1, 25 - 1 + 1).ToList ();
        string [] arrCommutabilityFirstCellValue = { "No Middle", "Ok Middle" };
        string [] arrCommutabilitySecondCellValue = { "Front", "Back", "Overall" };
        string [] arrCommutabilityThirdCellValue = { "<=", ">=" };
        List<int> arrCommutabilityValue = Enumerable.Range (0, 100).ToList ();
		public static List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList ();
		//public static List<City> lstOvernightCities = GlobalSettings.OverNightCitiesInBid;
		public static List<City> lstOvernightCities = GlobalSettings.WBidINIContent.Cities;
		List<DateHelper> newPdoDateList = ConstraintBL.GetPartialDayList();
		List<string> newPdoDateStringList = new List<string>();

		// Called when created from unmanaged code
		public ConstraintsCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ConstraintsCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		void BtnClose_Activated(object sender, EventArgs e)
		{
			  NSNotificationCenter.DefaultCenter.PostNotificationName ("CmtbltyLoadNotification", null);
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState> (WBidHelper.GetWBidDWCFilePath ());

			if (newPdoDateStringList.Count > 0)
			{
			} 
			else {
				newPdoDateStringList.Insert (0, "Any Date");
				for (int date = 0; date <newPdoDateList.Count ; date++) {
					DateTime date1 = newPdoDateList [date].Date;
					string str = date1.ToString ("dd - MMM");
					newPdoDateStringList.Insert (date + 1, str);
				}
			}

			switch (this.Identifier) 
			{
				case "View0":
					{
						btnClose1.Activated += (object sender, EventArgs e) => {
							CommonClass.ConstraintsController.RemoveAndReloadConstraints(lblTitle1.StringValue, order);
						};
						btnHelp1.Activated += (object sender, EventArgs e) => {
							if (CommonClass.HelpController == null)
							{
								var help = new HelpWindowController();
								CommonClass.HelpController = help;
							}
							CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
							CommonClass.HelpController.tblDocument.SelectRow(1, false);
							CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(ConstraintsApplied.HelpPageNo[lblTitle1.StringValue] - 1));
						};
					}
					break;
			case "View1":
				{
						
							btnValue1.Activated += (object sender, EventArgs e) =>
							{
								WBidHelper.PushToUndoStack();
								wBIdStateContent.Constraints.No3On3Off.Type = (int)btnValue1.SelectedTag;
								CommonClass.ConstraintsController.ApplyAndReloadConstraints(lblTitle1.StringValue);
							};
						
						
					btnClose1.Activated += (object sender, EventArgs e) => {
						CommonClass.ConstraintsController.RemoveAndReloadConstraints (lblTitle1.StringValue, order);
					};
					btnHelp1.Activated += (object sender, EventArgs e) => {
						if (CommonClass.HelpController == null) {
							var help = new HelpWindowController ();
							CommonClass.HelpController = help;
						}
						CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
						CommonClass.HelpController.tblDocument.SelectRow (1, false);
						CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (ConstraintsApplied.HelpPageNo [lblTitle1.StringValue] - 1));
					};
				}
				break;
//			case "Commutable Lines - Auto":
//				{
//					NSButton btnNight = (NSButton)this.ViewWithTag (101);
//					if (wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null) {
//						ButtonLayout (btnNight, wBIdStateContent.Constraints.CLAuto.NoNights);
//					}
//
//
//					btnNight.Activated += (object sender, EventArgs e) => {
//						if(wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null)
//						{
//							
//						wBIdStateContent.Constraints.CLAuto.NoNights=!wBIdStateContent.Constraints.CLAuto.NoNights;
//						ButtonLayout(btnNight, wBIdStateContent.Constraints.CLAuto.NoNights);
//						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
//						}
//					};
//					NSButton btnClose = (NSButton)this.ViewWithTag (104);
//					btnClose.Activated += (object sender1, EventArgs e1) => {
//
//						CommonClass.ConstraintsController.RemoveAndReloadConstraints ("Commutable Lines - Auto", order);
//
//					};
//
//					NSButton btnTowork = (NSButton)this.ViewWithTag (102);
//					if (wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null) {
//						
//						ButtonLayout (btnTowork, wBIdStateContent.Constraints.CLAuto.ToWork);
//					}
//					btnTowork.Activated += (object sender2, EventArgs e2) => {
//						if(wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null)
//						{
//							
//						wBIdStateContent.Constraints.CLAuto.ToWork=!wBIdStateContent.Constraints.CLAuto.ToWork;
//						ButtonLayout(btnTowork, wBIdStateContent.Constraints.CLAuto.ToWork);
//						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
//						}
//					};
//					NSButton btnToHome = (NSButton)this.ViewWithTag (103);
//					if (wBIdStateContent.CxWtState.CLAuto.Cx) {
//						
//						ButtonLayout (btnToHome, wBIdStateContent.Constraints.CLAuto.ToHome);
//					}
//					btnToHome.Activated += (object sender3, EventArgs e3) => {
//						if(wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null)
//						{
//							
//						wBIdStateContent.Constraints.CLAuto.ToHome=!wBIdStateContent.Constraints.CLAuto.ToHome;
//						ButtonLayout(btnToHome, wBIdStateContent.Constraints.CLAuto.ToHome);
//						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
//						}
//					};
//					NSButton btnHeadername = (NSButton)this.ViewWithTag (106);
//					if (wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null) {
//						btnHeadername.StringValue = "Cmt Line" + " [" + wBIdStateContent.Constraints.CLAuto.City + "]";
//					}  else
//						btnHeadername.StringValue = "Cmt Line";
//					btnHeadername.Activated += (object sender3, EventArgs e3) => {
//
//						NSNotificationCenter.DefaultCenter.PostNotificationName ("CLOLoadNotification", null);
//
//					} ;
//
//
//
//				}
//				break;
			case "View2":
				{
					btnComp2.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle2.StringValue == "Aircraft Changes") {
							wBIdStateContent.Constraints.AircraftChanges.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Blocks of Days Off") {
							wBIdStateContent.Constraints.BlockOfDaysOff.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Duty period") {
							wBIdStateContent.Constraints.DutyPeriod.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Flight Time") {
							wBIdStateContent.Constraints.FlightMin.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Legs Per Duty Period") {
							wBIdStateContent.Constraints.LegsPerDutyPeriod.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Legs Per Pairing") {
							wBIdStateContent.Constraints.LegsPerPairing.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Min Pay") {
							wBIdStateContent.Constraints.MinimumPay.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Number of Days Off") {
							wBIdStateContent.Constraints.NumberOfDaysOff.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Time-Away-From-Base") {
							wBIdStateContent.Constraints.PerDiem.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Work Days") {
							wBIdStateContent.Constraints.WorkDay.Type = (int)btnComp2.SelectedTag;
						} else if (this.lblTitle2.StringValue == "Intl – NonConus") {
							wBIdStateContent.Constraints.InterConus.lstParameters [order].Type = (int)btnComp2.SelectedTag;
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle2.StringValue);
					};

					btnValue2.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle2.StringValue == "Aircraft Changes") {
							wBIdStateContent.Constraints.AircraftChanges.Value = int.Parse (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Blocks of Days Off") {
							wBIdStateContent.Constraints.BlockOfDaysOff.Value = int.Parse (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Duty period") {
							wBIdStateContent.Constraints.DutyPeriod.Value = Helper.ConvertHHMMtoMinute (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Flight Time") {
							wBIdStateContent.Constraints.FlightMin.Value = Helper.ConvertHHMMtoMinute (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Legs Per Duty Period") {
							wBIdStateContent.Constraints.LegsPerDutyPeriod.Value = int.Parse (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Legs Per Pairing") {
							wBIdStateContent.Constraints.LegsPerPairing.Value = int.Parse (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Min Pay") {
							wBIdStateContent.Constraints.MinimumPay.Value = int.Parse (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Number of Days Off") {
							wBIdStateContent.Constraints.NumberOfDaysOff.Value = int.Parse (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Time-Away-From-Base") {
							wBIdStateContent.Constraints.PerDiem.Value = Helper.ConvertHHMMtoMinute (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Work Days") {
							wBIdStateContent.Constraints.WorkDay.Value = int.Parse (btnValue2.SelectedItem.Title);
						} else if (this.lblTitle2.StringValue == "Intl – NonConus") {
							int id = 0;
							if (btnValue2.SelectedItem.Title == "All") {
								//id=0 indecates the user selectd "All" 
								id = 0;
							} else
								id = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Name == btnValue2.SelectedItem.Title).Id;
							wBIdStateContent.Constraints.InterConus.lstParameters [order].Value = id;
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle2.StringValue);
					};

					btnClose2.Activated += (object sender, EventArgs e) => {
						CommonClass.ConstraintsController.RemoveAndReloadConstraints (this.lblTitle2.StringValue, order);
					};

					btnHelp2.Activated += (object sender, EventArgs e) => {
						if (CommonClass.HelpController == null) {
							var help = new HelpWindowController ();
							CommonClass.HelpController = help;
						}
						CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
						CommonClass.HelpController.tblDocument.SelectRow (1, false);
						CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (ConstraintsApplied.HelpPageNo [lblTitle2.StringValue] - 1));
					};
				}
				break;
			case "View3":
				{
					btnSecond3.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle3.StringValue == "Days of the Week") {
							wBIdStateContent.Constraints.DaysOfWeek.lstParameters [order].ThirdcellValue = btnSecond3.SelectedTag.ToString ();
						} else if (this.lblTitle3.StringValue == "DH - first - last") {
							wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [order].ThirdcellValue = btnSecond3.SelectedTag.ToString ();
						} else if (this.lblTitle3.StringValue == "Equipment Type") {
							var thirdcellvalue = "";
							if (btnSecond3.SelectedItem.Title == "8Max")
								thirdcellvalue = "600";
							else if(btnSecond3.SelectedItem.Title == "7Max")
								thirdcellvalue = "200";
							else
								thirdcellvalue = btnSecond3.SelectedItem.Title;
							wBIdStateContent.Constraints.EQUIP.lstParameters [order].ThirdcellValue = thirdcellvalue;
						} else if (this.lblTitle3.StringValue == "Ground Time") {
							wBIdStateContent.Constraints.GroundTime.ThirdcellValue = Helper.ConvertHHMMtoMinute (btnSecond3.SelectedItem.Title).ToString ();
						} 
						else if (this.lblTitle3.StringValue == "Overnight Cities") 
						{
							//var id = GlobalSettings.OverNightCitiesInBid.FirstOrDefault (x => x.Name == btnSecond3.SelectedItem.Title).Id;
								var id = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == btnSecond3.SelectedItem.Title).Id;
							wBIdStateContent.Constraints.OverNightCities.lstParameters [order].ThirdcellValue = id.ToString ();
						} 
						else if (this.lblTitle3.StringValue == "Cities-Legs") 
						{
							var id = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Name == btnSecond3.SelectedItem.Title).Id;
							wBIdStateContent.Constraints.CitiesLegs.lstParameters [order].ThirdcellValue = id.ToString ();
						} 
						else if (this.lblTitle3.StringValue == "Rest") {
							wBIdStateContent.Constraints.Rest.lstParameters [order].ThirdcellValue = btnSecond3.SelectedTag.ToString ();
						} else if (this.lblTitle3.StringValue == "Start Day") {
                                wBIdStateContent.Constraints.StartDay.lstParameters [order].ThirdcellValue = btnSecond3.SelectedTag.ToString ();
                        }  
                            else if (this.lblTitle3.StringValue == "Trip Length") {
							wBIdStateContent.Constraints.TripLength.lstParameters [order].ThirdcellValue = btnSecond3.SelectedTag.ToString ();
						} else if (this.lblTitle3.StringValue == "Work Blk Length") {
							wBIdStateContent.Constraints.WorkBlockLength.lstParameters [order].ThirdcellValue = btnSecond3.SelectedTag.ToString ();
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle3.StringValue);
					};
					btnComp3.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle3.StringValue == "Days of the Week") {
							wBIdStateContent.Constraints.DaysOfWeek.lstParameters [order].Type = (int)btnComp3.SelectedTag;
						} else if (this.lblTitle3.StringValue == "DH - first - last") {
							wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [order].Type = (int)btnComp3.SelectedTag;
						} else if (this.lblTitle3.StringValue == "Equipment Type") {
							wBIdStateContent.Constraints.EQUIP.lstParameters [order].Type = (int)btnComp3.SelectedTag;
						} else if (this.lblTitle3.StringValue == "Ground Time") {
							wBIdStateContent.Constraints.GroundTime.Type = (int)btnComp3.SelectedTag;
						} 
						else if (this.lblTitle3.StringValue == "Overnight Cities") {
							wBIdStateContent.Constraints.OverNightCities.lstParameters [order].Type = (int)btnComp3.SelectedTag;
						} 
						else if (this.lblTitle3.StringValue == "Cities-Legs") {
							wBIdStateContent.Constraints.CitiesLegs.lstParameters [order].Type = (int)btnComp3.SelectedTag;
						} 
						else if (this.lblTitle3.StringValue == "Rest") {
							wBIdStateContent.Constraints.Rest.lstParameters [order].Type = (int)btnComp3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Start Day")
                        {
                                wBIdStateContent.Constraints.StartDay.lstParameters[order].Type = (int)btnComp3.SelectedTag;
                        }
                            else if (this.lblTitle3.StringValue == "Trip Length") {
							wBIdStateContent.Constraints.TripLength.lstParameters [order].Type = (int)btnComp3.SelectedTag;
						} else if (this.lblTitle3.StringValue == "Work Blk Length") {
							wBIdStateContent.Constraints.WorkBlockLength.lstParameters [order].Type = (int)btnComp3.SelectedTag;
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle3.StringValue);
					};
					btnValue3.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle3.StringValue == "Days of the Week") {
							wBIdStateContent.Constraints.DaysOfWeek.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
						} else if (this.lblTitle3.StringValue == "DH - first - last") {
							wBIdStateContent.Constraints.DeadHeadFoL.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
						} else if (this.lblTitle3.StringValue == "Equipment Type") {
							wBIdStateContent.Constraints.EQUIP.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
						} else if (this.lblTitle3.StringValue == "Ground Time") {
							wBIdStateContent.Constraints.GroundTime.Value = int.Parse (btnValue3.SelectedItem.Title);
						}
						else if (this.lblTitle3.StringValue == "Overnight Cities") {
							wBIdStateContent.Constraints.OverNightCities.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
						}
						else if (this.lblTitle3.StringValue == "Cities-Legs") {
							wBIdStateContent.Constraints.CitiesLegs.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
						}
						else if (this.lblTitle3.StringValue == "Rest") {
							wBIdStateContent.Constraints.Rest.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
                            } else if (this.lblTitle3.StringValue == "Start Day")
                        {
                                wBIdStateContent.Constraints.StartDay.lstParameters[order].Value = int.Parse(btnValue3.SelectedItem.Title);
                        }
                            else if (this.lblTitle3.StringValue == "Trip Length") {
							wBIdStateContent.Constraints.TripLength.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
						} else if (this.lblTitle3.StringValue == "Work Blk Length") {
							wBIdStateContent.Constraints.WorkBlockLength.lstParameters [order].Value = int.Parse (btnValue3.SelectedItem.Title);
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle3.StringValue);
					};
					btnClose3.Activated += (object sender, EventArgs e) => {
						CommonClass.ConstraintsController.RemoveAndReloadConstraints (this.lblTitle3.StringValue, order);
					};
					btnHelp3.Activated += (object sender, EventArgs e) => {
						if (CommonClass.HelpController == null) {
							var help = new HelpWindowController ();
							CommonClass.HelpController = help;
						}
						CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
						CommonClass.HelpController.tblDocument.SelectRow (1, false);
						CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (ConstraintsApplied.HelpPageNo [lblTitle3.StringValue] - 1));
					};

				}
				break;

			case "View4":
				{
					btnThird4.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle4.StringValue == "Cmut DHs") {
							var id = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Name == btnThird4.SelectedItem.Title).Id;
							wBIdStateContent.Constraints.DeadHeads.LstParameter [order].SecondcellValue = id.ToString ();
						} else if (this.lblTitle4.StringValue == "PDO")
						{
							//var lst = lstPDODays.ConvertAll (x => x.Date.ToString ("dd - MMM")).ToList ();
							//var ind = lst.IndexOf (btnThird4.SelectedItem.Title);
							var ind = newPdoDateStringList.IndexOf (btnThird4.SelectedItem.Title);
							if(btnThird4.SelectedItem.Title=="Any Date")
							{
								wBIdStateContent.Constraints.PDOFS.LstParameter [order].SecondcellValue="300";
							}
							else
							{
							wBIdStateContent.Constraints.PDOFS.LstParameter [order].SecondcellValue = lstPDODays [ind-1].DateId.ToString ();

							}
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle4.StringValue);
					};
					btnSecond4.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle4.StringValue == "Cmut DHs") {
							wBIdStateContent.Constraints.DeadHeads.LstParameter [order].ThirdcellValue = btnSecond4.SelectedTag.ToString ();
						} else if (this.lblTitle4.StringValue == "PDO")
						{
							if(btnSecond4.SelectedItem.Title=="Any City")
							{
								wBIdStateContent.Constraints.PDOFS.LstParameter [order].ThirdcellValue="400";
							}
							else
							{
							var id = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Name == btnSecond4.SelectedItem.Title).Id;
							wBIdStateContent.Constraints.PDOFS.LstParameter [order].ThirdcellValue = id.ToString ();
							}
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle4.StringValue);
					};
					btnComp4.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle4.StringValue == "Cmut DHs") {
							wBIdStateContent.Constraints.DeadHeads.LstParameter [order].Type = (int)btnComp4.SelectedTag;
						} else if (this.lblTitle4.StringValue == "PDO") {
							wBIdStateContent.Constraints.PDOFS.LstParameter [order].Type = (int)btnComp4.SelectedTag;
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle4.StringValue);
					};
					btnValue4.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (this.lblTitle4.StringValue == "Cmut DHs") {
							wBIdStateContent.Constraints.DeadHeads.LstParameter [order].Value = int.Parse (btnValue4.SelectedItem.Title);
						} else if (this.lblTitle4.StringValue == "PDO") {
							wBIdStateContent.Constraints.PDOFS.LstParameter [order].Value = Helper.ConvertHHMMtoMinute (btnValue4.SelectedItem.Title);
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints (this.lblTitle4.StringValue);
					};
					btnClose4.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						CommonClass.ConstraintsController.RemoveAndReloadConstraints (this.lblTitle4.StringValue, order);
					};
					btnHelp4.Activated += (object sender, EventArgs e) => {
						if (CommonClass.HelpController == null) {
							var help = new HelpWindowController ();
							CommonClass.HelpController = help;
						}
						CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
						CommonClass.HelpController.tblDocument.SelectRow (1, false);
						CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (ConstraintsApplied.HelpPageNo [lblTitle4.StringValue] - 1));
					};

				}
				break;

				case "View5":
					{
						btnThird5.Activated += (object sender, EventArgs e) =>
						{
							WBidHelper.PushToUndoStack();
							if (this.lblTitle5.StringValue == "Start Day of Week")
							{
								wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].SecondcellValue = btnThird5.SelectedTag.ToString();
							}
							CommonClass.ConstraintsController.ApplyAndReloadConstraints(this.lblTitle5.StringValue);
						};

						btnSecond5.Activated += (object sender, EventArgs e) =>
						{
							WBidHelper.PushToUndoStack();
							if (this.lblTitle5.StringValue == "Start Day of Week")
							{
								wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].ThirdcellValue = btnSecond5.SelectedTag.ToString();
							}
							CommonClass.ConstraintsController.ApplyAndReloadConstraints(this.lblTitle5.StringValue);
						};

						btnComp5.Activated += (object sender, EventArgs e) =>
						{
							WBidHelper.PushToUndoStack();
							if (this.lblTitle5.StringValue == "Start Day of Week")
							{
								wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].Type = (int)btnComp5.SelectedTag;
							}
							CommonClass.ConstraintsController.ApplyAndReloadConstraints(this.lblTitle5.StringValue);
						};

						btnValue5.Activated += (object sender, EventArgs e) =>
						{
							WBidHelper.PushToUndoStack();
							if (this.lblTitle5.StringValue == "Start Day of Week")
							{
								wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].Value = int.Parse(btnValue5.SelectedItem.Title);
							}
							CommonClass.ConstraintsController.ApplyAndReloadConstraints(this.lblTitle5.StringValue);
						};

						btnClose5.Activated += (object sender, EventArgs e) =>
						{
							CommonClass.ConstraintsController.RemoveAndReloadConstraints(this.lblTitle5.StringValue, order);
						};

						btnHelp5.Activated += (object sender, EventArgs e) =>
						{
							if (CommonClass.HelpController == null)
							{
								var help = new HelpWindowController();
								CommonClass.HelpController = help;
							}
							CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
							CommonClass.HelpController.tblDocument.SelectRow(1, false);
							CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(ConstraintsApplied.HelpPageNo[lblTitle5.StringValue] - 1));
						};

					}

					break;

                case "View6":
                    {
                       
                        btnCloseReportRelease.Activated += (object sender, EventArgs e) =>
                        {
                            CommonClass.ConstraintsController.RemoveAndReloadConstraints("Report-Release", order);
                        };



                        btnCalculate.Activated += (object sender, EventArgs e) =>
                        {
                            ReportRelease objReport=wBIdStateContent.Constraints.ReportRelease.lstParameters[order];
                            objReport.AllDays=(btnType.SelectedTag==0);
                            objReport.First=(btnFirstCheck.State == NSCellStateValue.On);
                            objReport.Last=(btnLastCheck.State==NSCellStateValue.On);
                            objReport.NoMid=(btnNoMidCheck.State == NSCellStateValue.On);
                            objReport.Report = Helper.ConvertHHMMtoMinute(dpReport.DateValue.LocalValue());
                            objReport.Release = Helper.ConvertHHMMtoMinute(dpRelease.DateValue.LocalValue());
                            CommonClass.ConstraintsController.ApplyAndReloadConstraints("Report-Release");
                        };


                        //First Check
                        btnFirstCheck.Activated += (object sender, EventArgs e) => {
                        
                            if (((NSButton)sender).State == NSCellStateValue.On)
                            {
                                

                            }
                            else
                            {
                               

                            }


                        };

                        //Last Check

                        btnLastCheck.Activated += (object sender, EventArgs e) => {

                            if (((NSButton)sender).State == NSCellStateValue.On)
                            {


                            }
                            else
                            {


                            }


                        };

                        //NoMid Check

                        btnNoMidCheck.Activated += (object sender, EventArgs e) => {

                            if (((NSButton)sender).State == NSCellStateValue.On)
                            {


                            }
                            else
                            {


                            }


                        };

                        btnType.Activated += (object sender, EventArgs e) => {
                            setReportReleaseUI();
                            if (btnType.SelectedTag == 0)
                            {
 }
                               
                            else
                            {

                            }
                               
                        };

                        //dpReport.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin)).DateTimeToNSDate();
                        //dpRelease.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin)).DateTimeToNSDate();


                        dpReport.Locale = new NSLocale("NL");
                        dpReport.TimeZone = NSTimeZone.LocalTimeZone;
                        dpReport.Calendar = NSCalendar.CurrentCalendar;
                        dpReport.Activated += ReportReleaseTimeValuesChanged;
                        dpRelease.Locale = new NSLocale("NL");
                        dpRelease.TimeZone = NSTimeZone.LocalTimeZone;
                        dpRelease.Calendar = NSCalendar.CurrentCalendar;
                        dpRelease.Activated += ReportReleaseTimeValuesChanged; 




                    }
                    break;

                case "DOM":
				{
					lblDOM.RotateByAngle (-90);
					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo ();
					string strMonthName = mfi.GetMonthName (GlobalSettings.CurrentBidDetails.Month).ToString ();
					lblDOM.StringValue = strMonthName + "\nWork/Off Days";
					btnCloseDOM.Activated += (object sender, EventArgs e) => {
						ConstraintsCell.lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList ();
						CommonClass.ConstraintsController.RemoveAndReloadConstraints ("Days of the Month", order);
					};
					ConstraintsCell.lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList ();
					List<int> offDays = wBIdStateContent.Constraints.DaysOfMonth.OFFDays;
					if (offDays != null) {
						foreach (var item in offDays) {
							lstDaysOfMonth.FirstOrDefault (x => x.Id == item).Status = 2;
						}
					}
					List<int> workDays = wBIdStateContent.Constraints.DaysOfMonth.WorkDays;
					if (workDays != null) {
						foreach (var item in workDays) {
							lstDaysOfMonth.FirstOrDefault (x => x.Id == item).Status = 1;
						}
					}
					vwDOM.ItemPrototype = new DOMConstraintsItem ();
					vwDOM.Content = lstDaysOfMonth.Select (x => new NSString (x.Id.ToString ())).ToArray ();
					btnHelpDOM.Activated += (object sender, EventArgs e) => {
						if (CommonClass.HelpController == null) {
							var help = new HelpWindowController ();
							CommonClass.HelpController = help;
						}
						CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
						CommonClass.HelpController.tblDocument.SelectRow (1, false);
						CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (ConstraintsApplied.HelpPageNo ["Days of the Month"] - 1));
					};

				}
				break;
			case "OBLK":
				{
					btnCloseBLK.Activated += (object sender, EventArgs e) => {

						CommonClass.ConstraintsController.RemoveAndReloadConstraints ("Overnight Cities - Bulk", order);
						foreach (var item in lstOvernightCities) {
							item.Status = 0;
						}
					};

						foreach (var item in lstOvernightCities)
						{
							if (GlobalSettings.OverNightCitiesInBid.Any(x => x.Name == item.Name))
								item.Status = 0;
							else
								item.Status = 3;
						}
					List<int> noDays = wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo;
					if (noDays != null) {
						foreach (var item in noDays) {
							lstOvernightCities.FirstOrDefault (x => x.Id == item).Status = 2;
						}
					}
					List<int> yesDays = wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes;
					if (yesDays != null) {
						foreach (var item in yesDays) {
							lstOvernightCities.FirstOrDefault (x => x.Id == item).Status = 1;
						}
					}
					vwBLK.ItemPrototype = new BulkConstraintsItem ();
					vwBLK.Content = lstOvernightCities.Select (x => new NSString (x.Id.ToString ())).ToArray ();
					btnHelpBLK.Activated += (object sender, EventArgs e) => {
						if (CommonClass.HelpController == null) {
							var help = new HelpWindowController ();
							CommonClass.HelpController = help;
						}
						CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
						CommonClass.HelpController.tblDocument.SelectRow (1, false);
						CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (ConstraintsApplied.HelpPageNo ["Overnight Cities - Bulk"] - 1));
					};

				}
				break;
			case "CL":
				{
                       
					btnHelpCL.Activated += (object sender, EventArgs e) => {
						if (CommonClass.HelpController == null) {
							var help = new HelpWindowController ();
							CommonClass.HelpController = help;
						}
						CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
						CommonClass.HelpController.tblDocument.SelectRow (1, false);
						CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (ConstraintsApplied.HelpPageNo ["Commutable Lines - Manual"] - 1));
					};

					btnCloseCL.Activated += (object sender, EventArgs e) => {
						CommonClass.ConstraintsController.RemoveAndReloadConstraints ("Commutable Lines - Manual", order);
					};
					btnCommuteWork.Activated += (object sender, EventArgs e) => {

						if (btnCommuteWork.State == NSCellStateValue.Off && btnCommuteHome.State == NSCellStateValue.Off) {
							btnCommuteHome.State = NSCellStateValue.On;
							wBIdStateContent.Constraints.CL.CommuteToWork = false;
							wBIdStateContent.Constraints.CL.CommuteToHome = true;
						} else {
							wBIdStateContent.Constraints.CL.CommuteToWork = (btnCommuteWork.State == NSCellStateValue.On);
						}
                            SetweightButton();
						EnableDisableTimeFields ();
						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Manual");
					};
					btnCommuteHome.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						if (btnCommuteWork.State == NSCellStateValue.Off && btnCommuteHome.State == NSCellStateValue.Off) {
							btnCommuteWork.State = NSCellStateValue.On;
							wBIdStateContent.Constraints.CL.CommuteToWork = true;
							wBIdStateContent.Constraints.CL.CommuteToHome = false;
						} else {
							wBIdStateContent.Constraints.CL.CommuteToHome = (btnCommuteHome.State == NSCellStateValue.On);
						}
                        SetweightButton();
						EnableDisableTimeFields ();
						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Manual");
					};
					btnInDom.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						btnInDom.State = NSCellStateValue.On;
						btnBothEnds.State = NSCellStateValue.Off;
						wBIdStateContent.Constraints.CL.AnyNight = true;
						wBIdStateContent.Constraints.CL.RunBoth = false;
						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Manual");
					};
					btnBothEnds.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						btnInDom.State = NSCellStateValue.Off;
						btnBothEnds.State = NSCellStateValue.On;
						wBIdStateContent.Constraints.CL.AnyNight = false;
						wBIdStateContent.Constraints.CL.RunBoth = true;
						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Manual");
					};
					btnLoadDefaults.Activated += (object sender, EventArgs e) => {
                            wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
						WBidHelper.PushToUndoStack ();
						wBIdStateContent.Constraints.CL.MondayThu.Checkin = wbidintialState.Weights.CL.DefaultTimes [0].Checkin;
						wBIdStateContent.Constraints.CL.Friday.Checkin = wbidintialState.Weights.CL.DefaultTimes [1].Checkin;
						wBIdStateContent.Constraints.CL.Saturday.Checkin = wbidintialState.Weights.CL.DefaultTimes [2].Checkin;
						wBIdStateContent.Constraints.CL.Sunday.Checkin = wbidintialState.Weights.CL.DefaultTimes [3].Checkin;
						wBIdStateContent.Constraints.CL.MondayThu.BackToBase = wbidintialState.Weights.CL.DefaultTimes [0].BackToBase;
						wBIdStateContent.Constraints.CL.Friday.BackToBase = wbidintialState.Weights.CL.DefaultTimes [1].BackToBase;
						wBIdStateContent.Constraints.CL.Saturday.BackToBase = wbidintialState.Weights.CL.DefaultTimes [2].BackToBase;
						wBIdStateContent.Constraints.CL.Sunday.BackToBase = wbidintialState.Weights.CL.DefaultTimes [3].BackToBase;

                            wBIdStateContent.Weights.CL.TimesList[0].Checkin = wBIdStateContent.Constraints.CL.MondayThu.Checkin;
                            wBIdStateContent.Weights.CL.TimesList[1].Checkin = wBIdStateContent.Constraints.CL.Friday.Checkin;
                            wBIdStateContent.Weights.CL.TimesList[2].Checkin = wBIdStateContent.Constraints.CL.Saturday.Checkin;
                            wBIdStateContent.Weights.CL.TimesList[3].Checkin = wBIdStateContent.Constraints.CL.Sunday.Checkin;
                            wBIdStateContent.Weights.CL.TimesList[0].BackToBase = wBIdStateContent.Constraints.CL.MondayThu.BackToBase;
                        wBIdStateContent.Weights.CL.TimesList[1].BackToBase = wBIdStateContent.Constraints.CL.Friday.BackToBase;
                            wBIdStateContent.Weights.CL.TimesList[2].BackToBase = wBIdStateContent.Constraints.CL.Saturday.BackToBase;
                            wBIdStateContent.Weights.CL.TimesList[3].BackToBase = wBIdStateContent.Constraints.CL.Sunday.BackToBase;

						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Manual");

                        dpMonThuCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.Checkin)).DateTimeToNSDate();
                        dpFriCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.Checkin)).DateTimeToNSDate();
                        dpSatCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin)).DateTimeToNSDate();
                        dpSunCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.Checkin)).DateTimeToNSDate();

                        dpMonThuToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.BackToBase)).DateTimeToNSDate();
                        dpFriToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.BackToBase)).DateTimeToNSDate();
                        dpSatToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.BackToBase)).DateTimeToNSDate();
                        dpSunToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.BackToBase)).DateTimeToNSDate();

                        bool enableDefBtns = checkChangesInDefaultsValue();
                        btnLoadDefaults.Enabled = enableDefBtns;
                        btnSaveDefaults.Enabled = enableDefBtns;
						CommonClass.ConstraintsController.tblConstraints.ReloadData ();
					};
					btnSaveDefaults.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						wbidintialState.Weights.CL.DefaultTimes [0].Checkin = Helper.ConvertHHMMtoMinute (dpMonThuCheckIn.DateValue.LocalValue ());
						wbidintialState.Weights.CL.DefaultTimes [1].Checkin = Helper.ConvertHHMMtoMinute (dpFriCheckIn.DateValue.LocalValue ());
						wbidintialState.Weights.CL.DefaultTimes [2].Checkin = Helper.ConvertHHMMtoMinute (dpSatCheckIn.DateValue.LocalValue ());
						wbidintialState.Weights.CL.DefaultTimes [3].Checkin = Helper.ConvertHHMMtoMinute (dpSunCheckIn.DateValue.LocalValue ());
						wbidintialState.Weights.CL.DefaultTimes [0].BackToBase = Helper.ConvertHHMMtoMinute (dpMonThuToBase.DateValue.LocalValue ());
						wbidintialState.Weights.CL.DefaultTimes [1].BackToBase = Helper.ConvertHHMMtoMinute (dpFriToBase.DateValue.LocalValue ());
						wbidintialState.Weights.CL.DefaultTimes [2].BackToBase = Helper.ConvertHHMMtoMinute (dpSatToBase.DateValue.LocalValue ());
						wbidintialState.Weights.CL.DefaultTimes [3].BackToBase = Helper.ConvertHHMMtoMinute (dpSunToBase.DateValue.LocalValue ());

						XmlHelper.SerializeToXml (wbidintialState, WBidHelper.GetWBidDWCFilePath ());
						wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState> (WBidHelper.GetWBidDWCFilePath ());

						bool enableDefBtns = checkChangesInDefaultsValue ();
						btnLoadDefaults.Enabled = enableDefBtns;
						btnSaveDefaults.Enabled = enableDefBtns;
					};

					dpMonThuCheckIn.Locale = new NSLocale ("NL");
					dpFriCheckIn.Locale = new NSLocale ("NL");
					dpSatCheckIn.Locale = new NSLocale ("NL");
					dpSunCheckIn.Locale = new NSLocale ("NL");
					dpMonThuToBase.Locale = new NSLocale ("NL");
					dpFriToBase.Locale = new NSLocale ("NL");
					dpSatToBase.Locale = new NSLocale ("NL");
					dpSunToBase.Locale = new NSLocale ("NL");

					dpMonThuCheckIn.TimeZone = NSTimeZone.LocalTimeZone;
					dpFriCheckIn.TimeZone = NSTimeZone.LocalTimeZone;
					dpSatCheckIn.TimeZone = NSTimeZone.LocalTimeZone;
					dpSunCheckIn.TimeZone = NSTimeZone.LocalTimeZone;
					dpMonThuToBase.TimeZone = NSTimeZone.LocalTimeZone;
					dpFriToBase.TimeZone = NSTimeZone.LocalTimeZone;
					dpSatToBase.TimeZone = NSTimeZone.LocalTimeZone;
					dpSunToBase.TimeZone = NSTimeZone.LocalTimeZone;

					dpMonThuCheckIn.Calendar = NSCalendar.CurrentCalendar;
					dpFriCheckIn.Calendar = NSCalendar.CurrentCalendar;
					dpSatCheckIn.Calendar = NSCalendar.CurrentCalendar;
					dpSunCheckIn.Calendar = NSCalendar.CurrentCalendar;
					dpMonThuToBase.Calendar = NSCalendar.CurrentCalendar;
					dpFriToBase.Calendar = NSCalendar.CurrentCalendar;
					dpSatToBase.Calendar = NSCalendar.CurrentCalendar;
					dpSunToBase.Calendar = NSCalendar.CurrentCalendar;

					dpMonThuCheckIn.Activated += TimeValuesChanged;
					dpFriCheckIn.Activated += TimeValuesChanged;
					dpSatCheckIn.Activated += TimeValuesChanged;
					dpSunCheckIn.Activated += TimeValuesChanged;
					dpMonThuToBase.Activated += TimeValuesChanged;
					dpFriToBase.Activated += TimeValuesChanged;
					dpSatToBase.Activated += TimeValuesChanged;
					dpSunToBase.Activated += TimeValuesChanged;

				}

				break;

				

			}
		}
        private void setReportReleaseUI()
        {
            if(btnType.SelectedTag==0)
            {
                btnFirstCheck.Enabled = false;
                btnLastCheck.Enabled = false;
                btnNoMidCheck.Enabled = false;
            }
            else
            {
                btnFirstCheck.Enabled = true;
                btnLastCheck.Enabled = true;
                btnNoMidCheck.Enabled = true;
            }
        }

        // Report textBox actions
        void HandleReport(object sender, EventArgs e)
        {
        }

        // Release textBox actions
        void HandleRelease(object sender, EventArgs e)
        {
        }

        void ReportReleaseTimeValuesChanged(object sender, EventArgs e)         {
        }


        void ShowMessage(string text)
        {
            var alert = new NSAlert();
            alert.AlertStyle = NSAlertStyle.Warning;
            alert.MessageText = "Information";
            alert.InformativeText = text;
            alert.RunModal();
        }
        private void SetweightButton()
        {
            if (btnCommuteWork.State == NSCellStateValue.Off && btnCommuteHome.State == NSCellStateValue.Off)
            {
                btnCommuteWork.State = NSCellStateValue.On;
                wBIdStateContent.Weights.CL.Type = 3;
            }
            else if (btnCommuteWork.State == NSCellStateValue.On && btnCommuteHome.State == NSCellStateValue.On)
            {
                wBIdStateContent.Weights.CL.Type = 1;
            }
            else if (btnCommuteWork.State == NSCellStateValue.On)
            {
                wBIdStateContent.Weights.CL.Type = 3;
            }
            else if (btnCommuteHome.State == NSCellStateValue.On)
            {
                wBIdStateContent.Weights.CL.Type = 2;
            }
        }
		void ButtonLayout(NSButton button,bool state)
		{
			button.WantsLayer = true;//NSColor.FromRgba(124/256,206/256,38/256,1).CGColor;
			if (state) button.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
			else button.Layer.BackgroundColor = NSColor.Orange.CGColor;//NSColor.FromCalibratedRgba(.91f, .51f, .21f, 1).CGColor;

			button.Layer.CornerRadius = (nfloat)2;
			button.Layer.BorderColor = NSColor.DarkGray.CGColor;
			button.Layer.BorderWidth = (nfloat)0.5;
			button.NeedsLayout = true;

		}
		void TimeValuesChanged (object sender, EventArgs e)
		{
			WBidHelper.PushToUndoStack ();
			wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute (dpMonThuCheckIn.DateValue.LocalValue ());
			wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute (dpFriCheckIn.DateValue.LocalValue ());
			wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute (dpSatCheckIn.DateValue.LocalValue ());
			wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute (dpSunCheckIn.DateValue.LocalValue ());
			wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute (dpMonThuToBase.DateValue.LocalValue ());
			wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute (dpFriToBase.DateValue.LocalValue ());
			wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute (dpSatToBase.DateValue.LocalValue ());
			wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute (dpSunToBase.DateValue.LocalValue ());

            wBIdStateContent.Weights.CL.TimesList[0].Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[1].Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[2].Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[3].Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[0].BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[1].BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[2].BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[3].BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());

            CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Manual");

			bool enableDefBtns = checkChangesInDefaultsValue ();
			btnLoadDefaults.Enabled = enableDefBtns;
			btnSaveDefaults.Enabled = enableDefBtns;
		}

		/// <summary>
		/// retuurn true if any value chnages from the dafault state.
		/// </summary>
		/// <returns></returns>
		private bool checkChangesInDefaultsValue ()
		{
			if ((wbidintialState.Weights.CL.DefaultTimes [0].Checkin != Helper.ConvertHHMMtoMinute (dpMonThuCheckIn.DateValue.LocalValue ())) || (wbidintialState.Weights.CL.DefaultTimes [0].BackToBase != Helper.ConvertHHMMtoMinute (dpMonThuToBase.DateValue.LocalValue ()))) {
				return true;
			}
			if ((wbidintialState.Weights.CL.DefaultTimes [1].Checkin != Helper.ConvertHHMMtoMinute (dpFriCheckIn.DateValue.LocalValue ())) || (wbidintialState.Weights.CL.DefaultTimes [1].BackToBase != Helper.ConvertHHMMtoMinute (dpFriToBase.DateValue.LocalValue ()))) {
				return true;
			}
			if ((wbidintialState.Weights.CL.DefaultTimes [2].Checkin != Helper.ConvertHHMMtoMinute (dpSatCheckIn.DateValue.LocalValue ())) || (wbidintialState.Weights.CL.DefaultTimes [2].BackToBase != Helper.ConvertHHMMtoMinute (dpSatToBase.DateValue.LocalValue ()))) {
				return true;
			}
			if ((wbidintialState.Weights.CL.DefaultTimes [3].Checkin != Helper.ConvertHHMMtoMinute (dpSunCheckIn.DateValue.LocalValue ())) || (wbidintialState.Weights.CL.DefaultTimes [3].BackToBase != Helper.ConvertHHMMtoMinute (dpSunToBase.DateValue.LocalValue ()))) {
				return true;
			}

			return false;

		}

        private List<string> getComutabilityValues ()
        {
            List<string> lstvalue = new List<string> ();
            for (int i = 0; i <= 100; i=i+5) {
                lstvalue.Add (i + "%");
            }
            return lstvalue;
        }
		public void BindData (string constraint, int index)
		{
			var lst = CommonClass.ConstraintsController.appliedConstraints;
			order = index - lst.IndexOf (lst.FirstOrDefault (x => x == constraint));

			switch (this.Identifier) {
            //			case "Commutable Lines - Auto":
            //				{
            //					NSButton btnNight = (NSButton)this.ViewWithTag (101);
            //					btnNight.Activated += (object sender, EventArgs e) => {
            //						if(wBIdStateContent.CxWtState.CLAuto.Cx)
            //						{
            //
            //							wBIdStateContent.Constraints.CLAuto.NoNights=!wBIdStateContent.Constraints.CLAuto.NoNights;
            //							ButtonLayout(btnNight, wBIdStateContent.Constraints.CLAuto.NoNights);
            //							CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
            //						}
            //					};
            //					NSButton btnClose = (NSButton)this.ViewWithTag (104);
            //					btnClose.Activated += (object sender1, EventArgs e1) => {
            //
            //						CommonClass.ConstraintsController.RemoveAndReloadConstraints ("Commutable Lines - Auto", order);
            //
            //					};
            //
            //					NSButton btnTowork = (NSButton)this.ViewWithTag (102);
            //					btnTowork.Activated += (object sender2, EventArgs e2) => {
            //						if(wBIdStateContent.CxWtState.CLAuto.Cx)
            //						{
            //
            //							wBIdStateContent.Constraints.CLAuto.ToWork=!wBIdStateContent.Constraints.CLAuto.ToWork;
            //							ButtonLayout(btnTowork, wBIdStateContent.Constraints.CLAuto.ToWork);
            //							CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
            //						}
            //					};
            //					NSButton btnToHome = (NSButton)this.ViewWithTag (103);
            //					btnToHome.Activated += (object sender3, EventArgs e3) => {
            //						if(wBIdStateContent.CxWtState.CLAuto.Cx)
            //						{
            //
            //							wBIdStateContent.Constraints.CLAuto.ToHome=!wBIdStateContent.Constraints.CLAuto.ToHome;
            //							ButtonLayout(btnToHome, wBIdStateContent.Constraints.CLAuto.ToHome);
            //							CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
            //						}
            //					};
            //					NSButton btnHeadername = (NSButton)this.ViewWithTag (106);
            //					if (wBIdStateContent.CxWtState.CLAuto.Cx) {
            //						btnHeadername.Title = "Cmt Line" + " [" + wBIdStateContent.Constraints.CLAuto.City + "]";
            //					}  else
            //						btnHeadername.Title = "Cmt Line";
            //					btnHeadername.Activated += (object sender3, EventArgs e3) => {
            //
            //						NSNotificationCenter.DefaultCenter.PostNotificationName ("CLOLoadNotification", null);
            //
            //					} ;
            //
            //
            //
            //				}
            //				break;
            case "Commutability": {

                    NSButton btnTitle = (NSButton)ViewWithTag (206);
                    btnTitle.Title = "Comut%(" + wBIdStateContent.Constraints.Commute.City + ")";

						btnTitle.Activated-= BtnClose_Activated;
							btnTitle.Activated+= BtnClose_Activated;

                    //btnTitle.Activated += (object sender, EventArgs e) => {

                    //    NSNotificationCenter.DefaultCenter.PostNotificationName ("CmtbltyLoadNotification", null);
                    //};

                    NSButton btnClose = (NSButton)this.ViewWithTag (104);


					

                    btnClose.Activated += (object sender1, EventArgs e1) => {

                        CommonClass.ConstraintsController.RemoveAndReloadConstraints ("Commutability", order);

                    };


                    string NoMiddleOrOkMiddle =arrCommutabilityFirstCellValue[wBIdStateContent.Constraints.Commute.SecondcellValue -1] ;
                    string NoFronOrBack = arrCommutabilitySecondCellValue [wBIdStateContent.Constraints.Commute.ThirdcellValue-1];
					string leethanOrGreaterThan = string.Empty;
						switch (wBIdStateContent.Constraints.Commute.Type)
						{
							case (int)ConstraintType.LessThan:
								leethanOrGreaterThan = arrCommutabilityThirdCellValue[0];

								break;
							case (int)ConstraintType.MoreThan:
								leethanOrGreaterThan = arrCommutabilityThirdCellValue[1];
								break;

						}

                    string Value = wBIdStateContent.Constraints.Commute.Value.ToString();

                    cmtbltybtnValue1.RemoveAllItems ();
                    cmtbltybtnValue1.AddItems (arrCommutabilityFirstCellValue);
                    cmtbltybtnValue1.SelectItem (NoMiddleOrOkMiddle);
                    cmtbltybtnValue1.Activated += (object sender, EventArgs e) => {
                        if (wBIdStateContent.CxWtState.Commute.Cx && wBIdStateContent.Constraints.Commute != null) {

                            wBIdStateContent.Constraints.Commute.SecondcellValue =(Int32) cmtbltybtnValue1.IndexOfSelectedItem +1;
                           
                            CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutability");
                        }
                    };




                    cmtbltybtnValue2.RemoveAllItems ();
                    cmtbltybtnValue2.AddItems (arrCommutabilitySecondCellValue);
                    cmtbltybtnValue2.SelectItem (NoFronOrBack);

                    cmtbltybtnValue2.Activated += (object sender, EventArgs e) => {

                        if (wBIdStateContent.CxWtState.Commute.Cx && wBIdStateContent.Constraints.Commute != null) {

                            wBIdStateContent.Constraints.Commute.ThirdcellValue = (Int32)cmtbltybtnValue2.IndexOfSelectedItem + 1;

                            CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutability");
                        }
                    };

                    cmtbltybtnValue3.RemoveAllItems ();
                    cmtbltybtnValue3.AddItems (arrCommutabilityThirdCellValue);
                    cmtbltybtnValue3.SelectItem (leethanOrGreaterThan);

                    cmtbltybtnValue3.Activated += (object sender, EventArgs e) => {
                        if (wBIdStateContent.CxWtState.Commute.Cx && wBIdStateContent.Constraints.Commute != null) {

							switch (cmtbltybtnValue3.IndexOfSelectedItem)
							{
									case 0 : wBIdStateContent.Constraints.Commute.Type = (int)ConstraintType.LessThan;
										
									break;
								case 1: wBIdStateContent.Constraints.Commute.Type = (int)ConstraintType.MoreThan;
									break;
										
							}
                            

                            CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutability");
                        }
                    };

                    cmtbltybtnValue4.RemoveAllItems ();

                    cmtbltybtnValue4.AddItems (getComutabilityValues ().ToArray());
                    cmtbltybtnValue4.SelectItem (Value +"%");
                    string [] arrWeight = getComutabilityValues ().ToArray ();
                    cmtbltybtnValue4.Activated += (object sender, EventArgs e) => {
                        if (wBIdStateContent.CxWtState.Commute.Cx && wBIdStateContent.Constraints.Commute != null) {

                            string WeightValue = arrWeight [(Int32)cmtbltybtnValue4.IndexOfSelectedItem];

                            wBIdStateContent.Constraints.Commute.Value = Int32.Parse (Regex.Match (WeightValue, @"\d+").Value);

                            CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutability");
                        }
                    };


                    break;
                }


			case "Commutable Lines - Auto":
				{
					NSButton btnNight = (NSButton)this.ViewWithTag (101);
					if (wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null) {
						ButtonLayout (btnNight, wBIdStateContent.Constraints.CLAuto.NoNights);
					}


					btnNight.Activated += (object sender, EventArgs e) => {
						if(wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null)
						{

							wBIdStateContent.Constraints.CLAuto.NoNights=!wBIdStateContent.Constraints.CLAuto.NoNights;
							ButtonLayout(btnNight, wBIdStateContent.Constraints.CLAuto.NoNights);
							CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
						}
					};
					NSButton btnClose = (NSButton)this.ViewWithTag (104);
					btnClose.Activated += (object sender1, EventArgs e1) => {

						CommonClass.ConstraintsController.RemoveAndReloadConstraints ("Commutable Lines - Auto", order);

					};

					NSButton btnTowork = (NSButton)this.ViewWithTag (102);
					if (wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null) {

						ButtonLayout (btnTowork, wBIdStateContent.Constraints.CLAuto.ToWork);
					}
					btnTowork.Activated += (object sender2, EventArgs e2) => {
						if(wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null)
						{

							wBIdStateContent.Constraints.CLAuto.ToWork=!wBIdStateContent.Constraints.CLAuto.ToWork;
							ButtonLayout(btnTowork, wBIdStateContent.Constraints.CLAuto.ToWork);
							CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
						}
					};
					NSButton btnToHome = (NSButton)this.ViewWithTag (103);
					if (wBIdStateContent.CxWtState.CLAuto.Cx) {

						ButtonLayout (btnToHome, wBIdStateContent.Constraints.CLAuto.ToHome);
					}
					btnToHome.Activated += (object sender3, EventArgs e3) => {
						if(wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null)
						{

							wBIdStateContent.Constraints.CLAuto.ToHome=!wBIdStateContent.Constraints.CLAuto.ToHome;
							ButtonLayout(btnToHome, wBIdStateContent.Constraints.CLAuto.ToHome);
							CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Commutable Lines - Auto");
						}
					};
					NSButton btnHeadername = (NSButton)this.ViewWithTag (106);
					if (wBIdStateContent.CxWtState.CLAuto.Cx && wBIdStateContent.Constraints.CLAuto!=null) {
						btnHeadername.Title = "Cmt Line" + " [" + wBIdStateContent.Constraints.CLAuto.City + "]";
					}  else
						btnHeadername.Title = "Cmt Line";
					btnHeadername.Activated += (object sender3, EventArgs e3) => {

						NSNotificationCenter.DefaultCenter.PostNotificationName ("CLOLoadNotification", null);

					} ;



				}
				break;
				case "View0":
					{
						lblTitle1.StringValue = constraint;


					}
					break;
				case "View1":
				{
					lblTitle1.StringValue = constraint;
						
							int type = wBIdStateContent.Constraints.No3On3Off.Type;
							btnValue1.RemoveAllItems();
							btnValue1.AddItems(arrCompString5);
							btnValue1.Items()[0].Tag = (int)ThreeOnThreeOff.NoThreeOnThreeOff;
							btnValue1.Items()[1].Tag = (int)ThreeOnThreeOff.ThreeOnThreeOff;
						btnValue1.SelectItemWithTag(type);
						
				}
				break;
			case "View2":
				{
					this.lblTitle2.StringValue = constraint;
					if (this.lblTitle2.StringValue == "Aircraft Changes") {
						int type = wBIdStateContent.Constraints.AircraftChanges.Type;
						int value = wBIdStateContent.Constraints.AircraftChanges.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrAirCraft.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue2.SelectItem (arrAirCraft.IndexOf (value));
					} else if (this.lblTitle2.StringValue == "Blocks of Days Off") {
						int type = wBIdStateContent.Constraints.BlockOfDaysOff.Type;
						int value = wBIdStateContent.Constraints.BlockOfDaysOff.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString2);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.Items () [2].Tag = (int)ConstraintType.EqualTo;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrBDO.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue2.SelectItem (arrBDO.IndexOf (value));
					} else if (this.lblTitle2.StringValue == "Duty period") {
						int type = wBIdStateContent.Constraints.DutyPeriod.Type;
						int value = wBIdStateContent.Constraints.DutyPeriod.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrDutyPeriodValue);
						btnValue2.SelectItem (Helper.ConvertMinuteToHHMM (value));
					} else if (this.lblTitle2.StringValue == "Flight Time") {
						int type = wBIdStateContent.Constraints.FlightMin.Type;
						int value = wBIdStateContent.Constraints.FlightMin.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrFlightTimeValue.ConvertAll (x => x.ToString () + ":00").ToArray ());
						btnValue2.SelectItem (Helper.ConvertMinuteToHHMM (value));
					} else if (this.lblTitle2.StringValue == "Legs Per Duty Period") {
						int type = wBIdStateContent.Constraints.LegsPerDutyPeriod.Type;
						int value = wBIdStateContent.Constraints.LegsPerDutyPeriod.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrlegsPerPeriodValue.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue2.SelectItem (arrlegsPerPeriodValue.IndexOf (value));
					} else if (this.lblTitle2.StringValue == "Legs Per Pairing") {
						int type = wBIdStateContent.Constraints.LegsPerPairing.Type;
						int value = wBIdStateContent.Constraints.LegsPerPairing.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrlegsPerPairingValue.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue2.SelectItem (arrlegsPerPairingValue.IndexOf (value));
					} else if (this.lblTitle2.StringValue == "Min Pay") {
						int type = wBIdStateContent.Constraints.MinimumPay.Type;
						int value = wBIdStateContent.Constraints.MinimumPay.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrMinPayValue.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue2.SelectItem (arrMinPayValue.IndexOf (value));
					} else if (this.lblTitle2.StringValue == "Number of Days Off") {
						int type = wBIdStateContent.Constraints.NumberOfDaysOff.Type;
						int value = wBIdStateContent.Constraints.NumberOfDaysOff.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrNumOfDaysOffValue.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue2.SelectItem (arrNumOfDaysOffValue.IndexOf (value));
					} else if (this.lblTitle2.StringValue == "Time-Away-From-Base") {
						int type = wBIdStateContent.Constraints.PerDiem.Type;
						int value = wBIdStateContent.Constraints.PerDiem.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString1);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrTimeAwayFrmBaseValue.ConvertAll (x => x.ToString () + ":00").ToArray ());
						btnValue2.SelectItem (Helper.ConvertMinuteToHHMM (value));
					} else if (this.lblTitle2.StringValue == "Work Days") {
						int type = wBIdStateContent.Constraints.WorkDay.Type;
						int value = wBIdStateContent.Constraints.WorkDay.Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString2);
						btnComp2.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp2.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp2.Items () [2].Tag = (int)ConstraintType.EqualTo;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						btnValue2.AddItems (arrAirCraft.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue2.SelectItem (arrAirCraft.IndexOf (value));
					} else if (this.lblTitle2.StringValue == "Intl – NonConus") {
						int type = wBIdStateContent.Constraints.InterConus.lstParameters [order].Type;
						int value = wBIdStateContent.Constraints.InterConus.lstParameters [order].Value;
						btnComp2.RemoveAllItems ();
						btnComp2.AddItems (arrCompString4);
						btnComp2.Items () [0].Tag = (int)CityType.International;
						btnComp2.Items () [1].Tag = (int)CityType.NonConus;
						btnComp2.SelectItemWithTag (type);
						btnValue2.RemoveAllItems ();
						if (btnComp2.SelectedTag == (int)CityType.International) {
							List<City> city = GlobalSettings.WBidINIContent.Cities.Where (x => x.International).ToList ();
							var lstname = city.Select (x => x.Name).ToList ();
							btnValue2.AddItems (lstname.ToArray ());
							btnValue2.InsertItem ("All", 0);
							string name = string.Empty;
							if (value == 0) {
								name = "All";
							} else {
								var cit=GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == value);
								if (cit != null)
									name = cit.Name;
								
							}
							btnValue2.SelectItem (name);
						} else {
							List<City> city = GlobalSettings.WBidINIContent.Cities.Where (x => x.NonConus).ToList ();
							var lstname = city.Select (x => x.Name).ToList ();
							btnValue2.AddItems (lstname.ToArray ());
							var name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == value).Name;
							btnValue2.SelectItem (name);
						}
					}
				}
				break;
			case "View3":
                    {
                        this.lblTitle3.StringValue = constraint;
                        if (this.lblTitle3.StringValue == "Days of the Week")
                        {
                            string third = wBIdStateContent.Constraints.DaysOfWeek.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.DaysOfWeek.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.DaysOfWeek.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrDays);
                            int i = 0;
                            foreach (var item in btnSecond3.Items())
                            {
                                item.Tag = i;
                                i++;
                            }
                            btnSecond3.SelectItemWithTag(int.Parse(third));
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrDOWValue.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrDOWValue.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "DH - first - last")
                        {
                            string third = wBIdStateContent.Constraints.DeadHeadFoL.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.DeadHeadFoL.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.DeadHeadFoL.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrFirstLast);
                            btnSecond3.Items()[0].Tag = (int)DeadheadType.First;
                            btnSecond3.Items()[1].Tag = (int)DeadheadType.Last;
                            btnSecond3.Items()[2].Tag = (int)DeadheadType.Both;
                            btnSecond3.SelectItemWithTag(int.Parse(third));
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrDHFOLValue.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrDHFOLValue.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "Equipment Type")
                        {
                            if (wBIdStateContent.Constraints.EQUIP.lstParameters[order].ThirdcellValue == "500")
                                wBIdStateContent.Constraints.EQUIP.lstParameters[order].ThirdcellValue = "300";
                            string third = wBIdStateContent.Constraints.EQUIP.lstParameters[order].ThirdcellValue;
							if (third == "600")
								third = "8Max";
							else if (third == "200")
								third = "7Max";
							
                            int type = wBIdStateContent.Constraints.EQUIP.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.EQUIP.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrEQType);
                            btnSecond3.SelectItem(third);
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrEQTypeValue.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrEQTypeValue.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "Ground Time")
                        {
                            string third = wBIdStateContent.Constraints.GroundTime.ThirdcellValue;
                            int type = wBIdStateContent.Constraints.GroundTime.Type;
                            int value = wBIdStateContent.Constraints.GroundTime.Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrGrndTimeValue);
                            btnSecond3.SelectItem(Helper.ConvertMinuteToHHMM(int.Parse(third)));
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrGrndTimeValue2.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrGrndTimeValue2.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "Overnight Cities")
                        {
                            string third = wBIdStateContent.Constraints.OverNightCities.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.OverNightCities.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.OverNightCities.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            //var lstname = GlobalSettings.OverNightCitiesInBid.Select (x => x.Name).ToList ();
                            var lstname = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                            btnSecond3.AddItems(lstname.ToArray());
                            //var city = GlobalSettings.OverNightCitiesInBid.FirstOrDefault(x => x.Id == int.Parse(third));
                            var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == int.Parse(third));
                            if (city != null)
                            {
                                var name = city.Name;
                                btnSecond3.SelectItem(name);
                                btnComp3.RemoveAllItems();
                                btnComp3.AddItems(arrCompString1);
                                btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                                btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                                btnComp3.SelectItemWithTag(type);
                                btnValue3.RemoveAllItems();
                                btnValue3.AddItems(arrAirCraft.ConvertAll(x => x.ToString()).ToArray());
                                btnValue3.SelectItem(arrAirCraft.IndexOf(value));
                            }
                        }
                        else if (this.lblTitle3.StringValue == "Cities-Legs")
                        {

                            if (wBIdStateContent.Constraints.CitiesLegs == null)
                            {
                                wBIdStateContent.Constraints.CitiesLegs = new Cx3Parameters
                                {
                                    ThirdcellValue = "1",
                                    Type = (int)ConstraintType.LessThan,
                                    Value = 1,
                                    lstParameters = new List<Cx3Parameter>()
                                };
                            }
                            string third = wBIdStateContent.Constraints.CitiesLegs.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.CitiesLegs.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.CitiesLegs.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            var lstname = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                            btnSecond3.AddItems(lstname.ToArray());
                            var name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == int.Parse(third)).Name;
                            btnSecond3.SelectItem(name);
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrAirCraft.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrAirCraft.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "Rest")
                        {
                            string third = wBIdStateContent.Constraints.Rest.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.Rest.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.Rest.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrRest);
                            btnSecond3.Items()[0].Tag = (int)RestType.All;
                            btnSecond3.Items()[1].Tag = (int)RestType.InDomicile;
                            btnSecond3.Items()[2].Tag = (int)RestType.AwayDomicile;
                            if (string.IsNullOrEmpty(third))
                                third = "0";
                            btnSecond3.SelectItemWithTag(int.Parse(third));
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrRestValue.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrRestValue.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "Start Day")
                        {
                            string third = wBIdStateContent.Constraints.StartDay.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.StartDay.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.StartDay.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrStartday);
                            btnSecond3.Items()[0].Tag = (int)StartDayType.Block;
                            btnSecond3.Items()[1].Tag = (int)StartDayType.Trip;
                          if (string.IsNullOrEmpty(third))
                                third = "0";
                            btnSecond3.SelectItemWithTag(int.Parse(third));
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrStartDayType);
                            btnComp3.Items()[0].Tag = (int)StartDay.StartOn;
                            btnComp3.Items()[1].Tag = (int)StartDay.DoesnotStart;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(startDayValue.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(startDayValue.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "Trip Length")
                        {
                            string third = wBIdStateContent.Constraints.TripLength.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.TripLength.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.TripLength.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrTripLenth);
                            int i = 1;
                            foreach (var item in btnSecond3.Items())
                            {
                                item.Tag = i;
                                i++;
                            }
                            btnSecond3.SelectItemWithTag(int.Parse(third));
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrAirCraft.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrAirCraft.IndexOf(value));
                        }
                        else if (this.lblTitle3.StringValue == "Work Blk Length")
                        {
                            string third = wBIdStateContent.Constraints.WorkBlockLength.lstParameters[order].ThirdcellValue;
                            int type = wBIdStateContent.Constraints.WorkBlockLength.lstParameters[order].Type;
                            int value = wBIdStateContent.Constraints.WorkBlockLength.lstParameters[order].Value;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrTripLenth);
                            int i = 1;
                            foreach (var item in btnSecond3.Items())
                            {
                                item.Tag = i;
                                i++;
                            }
                            btnSecond3.SelectItemWithTag(int.Parse(third));
                            btnComp3.RemoveAllItems();
                            btnComp3.AddItems(arrCompString1);
                            btnComp3.Items()[0].Tag = (int)ConstraintType.LessThan;
                            btnComp3.Items()[1].Tag = (int)ConstraintType.MoreThan;
                            btnComp3.SelectItemWithTag(type);
                            btnValue3.RemoveAllItems();
                            btnValue3.AddItems(arrAirCraft.ConvertAll(x => x.ToString()).ToArray());
                            btnValue3.SelectItem(arrAirCraft.IndexOf(value));
                        }
                    }
				break;
			case "View4":
				{
					this.lblTitle4.StringValue = constraint;
					if (this.lblTitle4.StringValue == "Cmut DHs") {
						string second = wBIdStateContent.Constraints.DeadHeads.LstParameter [order].SecondcellValue;
						string third = wBIdStateContent.Constraints.DeadHeads.LstParameter [order].ThirdcellValue;
						int type = wBIdStateContent.Constraints.DeadHeads.LstParameter [order].Type;
						int value = wBIdStateContent.Constraints.DeadHeads.LstParameter [order].Value;
						btnThird4.RemoveAllItems ();
						var lstname = GlobalSettings.WBidINIContent.Cities.Select (x => x.Name).ToList ();
						btnThird4.AddItems (lstname.ToArray ());
						var name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == int.Parse (second)).Name;
						btnThird4.SelectItem (name);
						btnSecond4.RemoveAllItems ();
						btnSecond4.AddItems (arrCmut2);
						btnSecond4.Items () [0].Tag = (int)DeadheadType.First;
						btnSecond4.Items () [1].Tag = (int)DeadheadType.Last;
						btnSecond4.Items () [2].Tag = (int)DeadheadType.Either;
						btnSecond4.Items () [3].Tag = (int)DeadheadType.Both;
						btnSecond4.SelectItemWithTag (int.Parse (third));
						btnComp4.RemoveAllItems ();
						btnComp4.AddItems (arrCompString1);
						btnComp4.Items () [0].Tag = (int)ConstraintType.LessThan;
						btnComp4.Items () [1].Tag = (int)ConstraintType.MoreThan;
						btnComp4.SelectItemWithTag (type);
						btnValue4.RemoveAllItems ();
						btnValue4.AddItems (arrStartDayValue.ConvertAll (x => x.ToString ()).ToArray ());
						btnValue4.SelectItem (arrStartDayValue.IndexOf (value));
					} else if (this.lblTitle4.StringValue == "PDO") {
						string second = wBIdStateContent.Constraints.PDOFS.LstParameter [order].SecondcellValue;
						string third = wBIdStateContent.Constraints.PDOFS.LstParameter [order].ThirdcellValue;
						int type = wBIdStateContent.Constraints.PDOFS.LstParameter [order].Type;
						int value = wBIdStateContent.Constraints.PDOFS.LstParameter [order].Value;
						btnThird4.RemoveAllItems ();

						//btnThird4.AddItems (lstPDODays.ConvertAll (x => x.Date.ToString ("dd - MMM")).ToArray ());
						btnThird4.AddItems(newPdoDateStringList.ToArray());
						if (second == "300") 
						{
							btnThird4.SelectItem ("Any Date");
						} else {
							var date = lstPDODays.FirstOrDefault (x => x.DateId == int.Parse (second)).Date;
							btnThird4.SelectItem (date.ToString ("dd - MMM"));
						}
						btnSecond4.RemoveAllItems ();
						var lstname = GlobalSettings.WBidINIContent.Cities.Select (x => x.Name).ToList ();
						lstname.Insert (0, "Any City");
						btnSecond4.AddItems (lstname.ToArray ());
						if (third == "400") {
							btnSecond4.SelectItem ("Any City");
						} else {
							var name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == int.Parse (third)).Name;
							btnSecond4.SelectItem (name);
						}
						btnComp4.RemoveAllItems ();
						btnComp4.AddItems (arrCompString3);
						btnComp4.Items () [0].Tag = (int)ConstraintType.atafter;
						btnComp4.Items () [1].Tag = (int)ConstraintType.atbefore;
						btnComp4.SelectItemWithTag (type);
						btnValue4.RemoveAllItems ();
						btnValue4.AddItems (lstPDOTimes.ToArray ());
						btnValue4.SelectItem (lstPDOTimes.IndexOf (Helper.ConvertMinuteToHHMM (value)));
					}
				}
				break;
				case "View5":
					{
						this.lblTitle5.StringValue = constraint;
					  if (this.lblTitle5.StringValue == "Start Day of Week")
						{
							string second = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].SecondcellValue;
							string third = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].ThirdcellValue;
							int type = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].Type;
							int value = wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters[order].Value;
							btnThird5.RemoveAllItems();
							btnThird5.AddItems(arrStartday);
							btnThird5.Items()[0].Tag = (int)StartDayOfWeekType.Blk;
							btnThird5.Items()[1].Tag = (int)StartDayOfWeekType.Trip;
							btnThird5.SelectItemWithTag(int.Parse(second));
							btnSecond5.RemoveAllItems();
							btnSecond5.AddItems(arrDays);
							int i = 0;
							foreach (var item in btnSecond5.Items())
							{
								item.Tag = i;
								i++;
							}
							btnSecond5.SelectItemWithTag(int.Parse(third));
							btnComp5.RemoveAllItems();
							btnComp5.AddItems(arrCompString1);
							btnComp5.Items()[0].Tag = (int)ConstraintType.LessThan;
							btnComp5.Items()[1].Tag = (int)ConstraintType.MoreThan;
							btnComp5.SelectItemWithTag(type);
							btnValue5.RemoveAllItems();
							btnValue5.AddItems(arrStartDayValue.ConvertAll(x => x.ToString()).ToArray());
							btnValue5.SelectItem(arrStartDayValue.IndexOf(value));
					    }
					}
				break;
            case "View6":
                    {
                        //this.lblTitle5.StringValue = constraint;
                        if (constraint == "Report-Release")
                        {
                            ReportRelease objReport = wBIdStateContent.Constraints.ReportRelease.lstParameters[order];
                           //btnType.SetState = (wBIdStateContent.Constraints.ReportRelease.lstParameters[order].AllDays) ? 0 : 1;
                            if(objReport.AllDays)
                            {
                                btnType.SelectCellWithTag(0);
                            }
                            else
                            {
                                btnType.SelectCellWithTag(1);
                            }
                           
                            btnFirstCheck.State = (objReport.First) ? NSCellStateValue.On : NSCellStateValue.Off;
                            btnLastCheck.State = (objReport.Last) ? NSCellStateValue.On : NSCellStateValue.Off;
                            btnNoMidCheck.State = (objReport.NoMid) ? NSCellStateValue.On : NSCellStateValue.Off;
                            dpReport.DateValue =DateTime.Parse(Helper.ConvertMinuteToHHMM(objReport.Report)).DateTimeToNSDate();
                            dpRelease.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(objReport.Release)).DateTimeToNSDate();
                        }
                        setReportReleaseUI();
                    }
                    break;
            case "DOM":
				{
					vwDOM.ItemPrototype = new DOMConstraintsItem ();
				}
				break;
			case "OBLK":
				{
					vwBLK.ItemPrototype = new BulkConstraintsItem ();
				}
				break;
			case "CL":
				{
					if (wBIdStateContent.Constraints.CL.CommuteToWork)
						btnCommuteWork.State = NSCellStateValue.On;
					else
						btnCommuteWork.State = NSCellStateValue.Off;
					if (wBIdStateContent.Constraints.CL.CommuteToHome)
						btnCommuteHome.State = NSCellStateValue.On;
					else
						btnCommuteHome.State = NSCellStateValue.Off;
					if (wBIdStateContent.Constraints.CL.AnyNight)
						btnInDom.State = NSCellStateValue.On;
					else
						btnInDom.State = NSCellStateValue.Off;
					if (wBIdStateContent.Constraints.CL.RunBoth)
						btnBothEnds.State = NSCellStateValue.On;
					else
						btnBothEnds.State = NSCellStateValue.Off;

					EnableDisableTimeFields ();

					dpMonThuCheckIn.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.MondayThu.Checkin)).DateTimeToNSDate();
					dpFriCheckIn.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.Friday.Checkin)).DateTimeToNSDate();
					dpSatCheckIn.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.Saturday.Checkin)).DateTimeToNSDate();
					dpSunCheckIn.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.Sunday.Checkin)).DateTimeToNSDate();

					dpMonThuToBase.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.MondayThu.BackToBase)).DateTimeToNSDate();
					dpFriToBase.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.Friday.BackToBase)).DateTimeToNSDate();
					dpSatToBase.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.Saturday.BackToBase)).DateTimeToNSDate();
					dpSunToBase.DateValue = DateTime.Parse (Helper.ConvertMinuteToHHMM (wBIdStateContent.Constraints.CL.Sunday.BackToBase)).DateTimeToNSDate();

					bool enableDefBtns = checkChangesInDefaultsValue ();
					btnLoadDefaults.Enabled = enableDefBtns;
					btnSaveDefaults.Enabled = enableDefBtns;

				}
				break;

			}
		}


		void EnableDisableTimeFields ()
		{
			dpMonThuCheckIn.Enabled = false;
			dpFriCheckIn.Enabled = false;
			dpSatCheckIn.Enabled = false;
			dpSunCheckIn.Enabled = false;
			dpMonThuToBase.Enabled = false;
			dpFriToBase.Enabled = false;
			dpSatToBase.Enabled = false;
			dpSunToBase.Enabled = false;
			if (wBIdStateContent.Constraints.CL.CommuteToWork) {
				dpMonThuCheckIn.Enabled = true;
				dpFriCheckIn.Enabled = true;
				dpSatCheckIn.Enabled = true;
				dpSunCheckIn.Enabled = true;
			}
			if (wBIdStateContent.Constraints.CL.CommuteToHome) {
				dpMonThuToBase.Enabled = true;
				dpFriToBase.Enabled = true;
				dpSatToBase.Enabled = true;
				dpSunToBase.Enabled = true;
			}
		}
	}

	//	public static class Extensions {
	//		public static string LocalValue (this NSDate date) {
	//			var d = DateTime.Parse (date.ToString());
	//			var convertedTime = TimeZoneInfo.ConvertTime (d, TimeZoneInfo.Local);
	//			return convertedTime.ToString ("HH:mm");
	//		}
	//	}
   
	#region DOMCollection
	public class DOMConstraintsItem : NSCollectionViewItem
	{
		private static readonly NSString EMPTY_NSSTRING = new NSString (string.Empty);
		private DOMCell view;

		public DOMConstraintsItem () : base ()
		{

		}

		public DOMConstraintsItem (IntPtr ptr) : base (ptr)
		{

		}

		public override void LoadView ()
		{
			view = new DOMCell ();
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
					view.Button.Title = string.Empty;
				} else {
					base.RepresentedObject = value;
					var item = ConstraintsCell.lstDaysOfMonth.FirstOrDefault (x => x.Id.ToString () == value.ToString ());

					if (item.Day == null)
						view.Button.Hidden = true;
					else
						view.Button.Hidden = false;

					view.Button.Tag = item.Id;
					if (item.Status == 1) {
						view.Button.Title = "Work";
						view.WantsLayer = true;
						view.BackView.WantsLayer = true;
                        this.view.BackView.Layer.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1).CGColor;
                        view.NeedsLayout = true;
					} else if (item.Status == 2) {
						view.Button.Title = "Off";
						view.WantsLayer = true;
						view.BackView.WantsLayer = true;
						this.view.BackView.Layer.BackgroundColor = NSColor.Red.CGColor;
						view.NeedsLayout = true;
					} else
						view.Button.Title = item.Day.ToString ();

					view.Button.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						var btn = (NSButton)sender;
						var dom = ConstraintsCell.lstDaysOfMonth.FirstOrDefault (x => x.Id == btn.Tag);
						if (dom.Status == 0) {
							ConstraintsCell.lstDaysOfMonth.FirstOrDefault (x => x.Id == btn.Tag).Status = 2;
							ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Add (dom.Id);
							if (ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Contains (dom.Id))
								ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Remove (dom.Id);
						} else if (dom.Status == 2) {
							ConstraintsCell.lstDaysOfMonth.FirstOrDefault (x => x.Id == btn.Tag).Status = 1;
							ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Add (dom.Id);
							if (ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Contains (dom.Id))
								ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Remove (dom.Id);
						} else {
							ConstraintsCell.lstDaysOfMonth.FirstOrDefault (x => x.Id == btn.Tag).Status = 0;
							ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Remove (dom.Id);
							ConstraintsCell.wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Remove (dom.Id);
						}
						CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Days of the Month");
					};
				}
			}
		}
	}

	public class DOMCell : NSView
	{
		private NSButton button;
		private NSView backView;

		public DOMCell () : base (new CGRect (0, 0, 51, 32))
		{
			backView = new NSView (new CGRect (5, 5, 40, 21));
			AddSubview (backView);
			button = new NSButton (new CGRect (5, 5, 40, 21));
			button.Bordered = false;
			AddSubview (button);
		}

		public NSButton Button {
			get { return button; }	
		}

		public NSView BackView {
			get { return backView; }	
		}

	}
	#endregion

	#region BulkCollection
	public class BulkConstraintsItem : NSCollectionViewItem
	{
		private static readonly NSString EMPTY_NSSTRING = new NSString (string.Empty);
		private BulkCell view;

		public BulkConstraintsItem () : base ()
		{

		}

		public BulkConstraintsItem (IntPtr ptr) : base (ptr)
		{

		}

		public override void LoadView ()
		{
			view = new BulkCell ();
			View = view;
		}

		public override NSObject RepresentedObject {
			get { return base.RepresentedObject; }

			set {
                var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
                if (value == null) {
					// Need to do this because setting RepresentedObject in base to null 
					// throws an exception because of the MonoMac generated wrappers,
					// and even though we don't have any null values, this will get 
					// called during initialization of the content with a null value.
					base.RepresentedObject = EMPTY_NSSTRING;
                    view.Button.Title = string.Empty;

				} else {
					base.RepresentedObject = value;
					var item = ConstraintsCell.lstOvernightCities.FirstOrDefault (x => x.Id.ToString () == value.ToString ());
					view.Button.Tag = item.Id;
					this.view.SelectedBgColor.Color = NSColor.White;
                    view.Button.Title = item.Name.ToString ();

                    if (interfaceStyle == "Dark")
                    {
                        view.Button.AttributedTitle = new NSAttributedString(view.Button.Title, ApplyAttribute(NSColor.Black));

                    }

                    if (item.Status == 1) {


                        this.view.SelectedBgColor.Color = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);


                    } else if (item.Status == 2) {
                       
                        this.view.SelectedBgColor.Color = NSColor.Red;
						
					} 
					else if (item.Status == 3)
					{

					var coloredTitle = new NSMutableAttributedString(item.Name.ToString());
					var titleRange = new NSRange(0, coloredTitle.Length);
					coloredTitle.AddAttribute (NSStringAttributeKey.ForegroundColor, NSColor.White, titleRange);            
	 				var centeredAttribute = new NSMutableParagraphStyle();
					centeredAttribute.Alignment = NSTextAlignment.Center;             
					coloredTitle.AddAttribute (NSStringAttributeKey.ParagraphStyle, centeredAttribute, titleRange);             
					view.Button.AttributedTitle = coloredTitle; 
						//						view.WantsLayer = true;
						//						this.view.BackView.Layer.BackgroundColor = NSColor.Red.CGColor;
						//						view.NeedsLayout = true;
						//this.view.SelectedBgColor.laye = NSColor.Black;
						this.view.SelectedBgColor.Color = NSColor.Black;
					//	view.Button.Layer.BackgroundColor = NSColor.Black.CGColor;
						//view.Button.Layer.BackgroundColor = NSColor.Black.CGColor;
						//view.Button.Enabled = false;
						//this.view.BackView.UpdateLayer ();
					}

	             		



					view.Button.Activated += (object sender, EventArgs e) => {
						WBidHelper.PushToUndoStack ();
						var btn = (NSButton)sender;
						var blk = ConstraintsCell.lstOvernightCities.FirstOrDefault (x => x.Id == btn.Tag);
						if (blk.Status == 0) {
							ConstraintsCell.lstOvernightCities.FirstOrDefault (x => x.Id == btn.Tag).Status = 2;
							ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Add (blk.Id);
							if (ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Contains (blk.Id))
								ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Remove (blk.Id);
								CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Overnight Cities - Bulk");
						} else if (blk.Status == 2) {
							
							ConstraintsCell.lstOvernightCities.FirstOrDefault (x => x.Id == btn.Tag).Status = 1;
							ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Add (blk.Id);
							if (ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Contains (blk.Id))
								ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Remove (blk.Id);

								CommonClass.ConstraintsController.ApplyAndReloadConstraints ("Overnight Cities - Bulk");
						} else {
							if (this.view.SelectedBgColor.Color != NSColor.Black)
							{
								ConstraintsCell.lstOvernightCities.FirstOrDefault(x => x.Id == btn.Tag).Status = 0;
								ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Remove(blk.Id);
								ConstraintsCell.wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Remove(blk.Id);
								CommonClass.ConstraintsController.ApplyAndReloadConstraints("Overnight Cities - Bulk");
							}
						}
					
					};
				}
			}


		}



        NSStringAttributes ApplyAttribute(NSColor color)
        {
            NSMutableParagraphStyle objAlignment = new NSMutableParagraphStyle();
            objAlignment.Alignment = NSTextAlignment.Center;
            var Applied = new NSStringAttributes
            {
                ForegroundColor = color,
                Font = NSFont.BoldSystemFontOfSize((nint)13),
                ParagraphStyle = objAlignment
            };
            return Applied;

        }

    }

  

    public class BulkCell : NSView
	{
		private NSButton button;
		private NSView backView;
		private NSColorWell BgColor;
		public BulkCell () : base (new CGRect (0, 0, 51, 32))
		{
			backView = new NSView (new CGRect (5, 5, 40, 21));
			AddSubview (backView);

			BgColor = new NSColorWell(new CGRect (0, 0, 51, 32));
			BgColor.Enabled = false;
			AddSubview (BgColor);
			button = new NSButton (new CGRect (5, 5, 40, 21));
			button.Bordered = false;
			AddSubview (button);
		}

		public NSButton Button {
			get { return button; }	
		}
		public NSColorWell SelectedBgColor {
			get { return BgColor; }	
		}
		public NSView BackView {
			get { return backView; }	
		}
	

	}
	#endregion

}

