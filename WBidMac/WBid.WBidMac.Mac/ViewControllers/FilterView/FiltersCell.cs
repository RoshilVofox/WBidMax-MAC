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
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidMac.Mac
{
	public partial class FiltersCell : AppKit.NSTableCellView
	{
		public FiltersCell ()
		{

		}
		public BAFilterViewController _parent;
		int order;

		#region Constructors

		public static WBidState wBIdStateContent;
		// = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		static WBidIntialState wbidintialState;
		// = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

		// Constraints Param Popover
		string[] arrCompString1 = { "Less than", "More than" };
		string[] arrCompString2 = { "Less than", "More than", "Equal to" };
		string[] arrCompString3 = { "At+after", "At+before" };
		string[] arrCompString4 = { "Intl", "NonConus" };
		string[] arrCompString6 = { "Less than", "Equal" ,"More than"};
		string[] arrDays = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
		string[] arrDaysOfWeek = { "sun","mon", "tue", "wed", "thu", "fri", "sat"  };
		string[] arrFirstLast = { "First", "Last", "Both" };
		string[] arrEQType = { "700", "800","7Max","8Max" };
		//string[] arrEQType = { "300", "500", "300 & 500", "700", "800" };
		string[] arrRest = { "All", "InDom", "AwayDom" };
		string[] arrTripLenth = { "1Day", "2Day", "3Day", "4Day" };
		//		string[] arrCmut = {"begin","end","both"};
		string[] arrCmut2 = { "begin", "end", "either", "both" };
		string[] arrCompString5 = { "NO 3-on-30ff", "Only 3-on-3-off" };

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
		List<int> arrDHFOLValue = Enumerable.Range (0, 25 - 0 + 1).ToList ();
		List<int> arrEQTypeValue = Enumerable.Range (0, 20 - 0 + 1).ToList ();
		List<int> arrDowSomeValue = Enumerable.Range (0, 20 - 0 + 1).ToList ();
		List<int> arrStartDayValue = Enumerable.Range (0, 6 - 0 + 1).ToList ();
		List<int> arrRestValue = Enumerable.Range (8, 48 - 8 + 1).ToList ();
		List<DateHelper> lstPDODays = ConstraintBL.GetPartialDayList ();
		List<string> lstPDOTimes = ConstraintBL.GetPartialTimeList ();
		List<int> arrGrndTimeValue2 = Enumerable.Range (1, 25 - 1 + 1).ToList ();

		public static List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList ();
		public static List<City> lstOvernightCities = GlobalSettings.OverNightCitiesInBid;

		// Called when created from unmanaged code
		public FiltersCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public FiltersCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.WantsLayer = true;
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState> (WBidHelper.GetWBidDWCFilePath ());
//
//			switch (this.Identifier) {
//			}
		}
		public void FunIscalculated(bool IsCalcluated)
		{
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            this.WantsLayer = true;

			if (!IsCalcluated) {
				this.Layer.BackgroundColor = NSColor.Red.CGColor;
			} else {
                this.Layer.BackgroundColor = NSColor.ControlBackground.CGColor;
			}
			NSTextField txtlabel;
			NSButton btnlabel;
			switch (this.Identifier) {
			case "DOWA":
				txtlabel = (NSTextField)ViewWithTag ((nint)18);
				txtlabel.AttributedStringValue = new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;
			case "AP":
				txtlabel = (NSTextField)ViewWithTag ((nint)4);
				txtlabel.AttributedStringValue = new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;
			case "DOWS":
				txtlabel = (NSTextField)ViewWithTag ((nint)34);
				txtlabel.AttributedStringValue =new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				
				break;
			case "DHFL":
				txtlabel = (NSTextField)ViewWithTag ((nint)44);
				txtlabel.AttributedStringValue = new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;

			case "ET":
				txtlabel = (NSTextField)ViewWithTag ((nint)54);
				txtlabel.AttributedStringValue =new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;
			case "RT":
				txtlabel = (NSTextField)ViewWithTag ((nint)64);
				txtlabel.AttributedStringValue =new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;
			case "LT":
				txtlabel = (NSTextField)ViewWithTag ((nint)26);
				txtlabel.AttributedStringValue =new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;
			case "TBL":
				txtlabel = (NSTextField)ViewWithTag ((nint)87);
				txtlabel.AttributedStringValue= new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;
			case "SDOW":
				txtlabel = (NSTextField)ViewWithTag ((nint)78);
				txtlabel.AttributedStringValue =new NSAttributedString(txtlabel.StringValue, ApplyAttribure (IsCalcluated));
				break;

			case "DOM":
				btnlabel = (NSButton)ViewWithTag ((nint)111);
				btnlabel.AttributedTitle = new NSAttributedString (btnlabel.Title, ApplyAttribure (IsCalcluated));
				NSTextField txtlabel1 = (NSTextField)ViewWithTag ((nint)112);
                    if (!IsCalcluated)
                    {
                        txtlabel1.TextColor = NSColor.White;
                    }
                    else {
                        if (interfaceStyle == "Dark") {
                            txtlabel1.TextColor = NSColor.White;
                        }
                        else {
                            txtlabel1.TextColor = NSColor.Black;
                        }

                    }
					
				break;
			case "OC":
				btnlabel = (NSButton)ViewWithTag ((nint)121);
				btnlabel.AttributedTitle =new NSAttributedString(btnlabel.Title, ApplyAttribure (IsCalcluated));
				NSTextField txtlabel2 = (NSTextField)ViewWithTag ((nint)122);
                    if (!IsCalcluated)
                    {
                        txtlabel2.TextColor = NSColor.White;
                    }
                    else
                    {
                        if (interfaceStyle == "Dark")
                        {
                            txtlabel2.TextColor = NSColor.White;
                        }
                        else
                        {
                            txtlabel2.TextColor = NSColor.Black;
                        }

                    }
                    break;
			case "CL":
				btnlabel = (NSButton)ViewWithTag ((nint)131);
				btnlabel.AttributedTitle =new NSAttributedString(btnlabel.Title, ApplyAttribure (IsCalcluated));

				break;

			}




		}
		NSStringAttributes ApplyAttribure(bool isapplied)
		{
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            var Applied = new NSStringAttributes {
				ForegroundColor = NSColor.Black

			};
			if (!isapplied) {
				Applied = new NSStringAttributes {
					ForegroundColor = NSColor.White

				};
				return Applied;
			} else {
                if (interfaceStyle == "Dark") {
                    Applied = new NSStringAttributes
                    {
                        ForegroundColor = NSColor.White

                    };
                }
				return Applied;
			}
		

		}

		/// <summary>
		/// retuurn true if any value chnages from the dafault state.
		/// </summary>
		/// <returns></returns>
	
	
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

		private void SetAmPmFilter(AMPMConstriants AmPm)
		{
			ButtonLayout (btnAM, AmPm.AM);
			ButtonLayout (btnPM, AmPm.PM);
			ButtonLayout (btnMix, AmPm.MIX);

		}
		private void SetDaysofWeekSome(Cx3Parameter cx3parameter)
		{
			for (int index = 31; index <= 33; index++) {
				NSPopUpButton btnTemp = (NSPopUpButton)this.ViewWithTag (index);
				btnTemp.RemoveAllItems ();
				switch (index) {
				case 31:
					btnTemp.AddItems (arrDays);
					if (cx3parameter.ThirdcellValue == null) {
						cx3parameter.ThirdcellValue = ((int)DayofTheWeek.Monday).ToString ();
						cx3parameter.Type = (int)ConstraintType.LessThan;
						cx3parameter.Value = arrDowSomeValue.FirstOrDefault ();
					}

					if (cx3parameter.ThirdcellValue != null)
					{	
						switch (int.Parse (cx3parameter.ThirdcellValue)) {
						case (int)DayofTheWeek.Monday:
							btnTemp.Title = "Mon";
							break;
						case (int)DayofTheWeek.Tuesday:
							btnTemp.Title = "Tue";
							break;
						case (int)DayofTheWeek.Wednesday:
							btnTemp.Title = "Wed";
							break;
						case (int)DayofTheWeek.Thursday:
							btnTemp.Title = "Thu";
							break;
						case (int)DayofTheWeek.Friday:
							btnTemp.Title = "Fri";
							break;
						case (int)DayofTheWeek.Saturday:
							btnTemp.Title = "Sat";
							break;
						case (int)DayofTheWeek.Sunday:
							btnTemp.Title = "Sun";
							break;
						
						}
					}
						break;
				case 32:
					btnTemp.AddItems (arrCompString6);
					switch (cx3parameter.Type)
					{
					case (int)ConstraintType.LessThan:
						btnTemp.Title = "Less than";
						break;
					case (int)ConstraintType.EqualTo:
						btnTemp.Title = "Equal";
						break;

					case (int)ConstraintType.MoreThan:
						btnTemp.Title = "More than";
						break;

					}

					break;

				case 33:
					btnTemp.AddItems (arrDowSomeValue.ConvertAll (x => x.ToString ()).ToArray ());
					btnTemp.Title=cx3parameter.Value.ToString();
					break;
				}
			}
			
		}
		private void SetDHFirstLast(Cx3Parameter cx3parameter)
		{
			for (int index = 41; index <= 43; index++) {

				NSPopUpButton btnTemp = (NSPopUpButton)this.ViewWithTag (index);
				btnTemp.RemoveAllItems ();
				switch (index) {
				case 41:
					btnTemp.AddItems (arrFirstLast);
					if (cx3parameter.ThirdcellValue == null)
					{
						cx3parameter.ThirdcellValue = ((int)DeadheadType.First).ToString ();
						cx3parameter.Type = (int)ConstraintType.LessThan;
						cx3parameter.Value = arrDHFOLValue.FirstOrDefault ();
					}
					if (cx3parameter.ThirdcellValue != null) {
						switch (int.Parse(cx3parameter.ThirdcellValue))
						{
						case (int)DeadheadType.First:
							btnTemp.Title = "First";
							break;
						case (int)DeadheadType.Last:
							btnTemp.Title = "Last";
							break;
						case (int)DeadheadType.Both:
							btnTemp.Title = "Both";
							break;

						}
					}

					break;
				case 42:
					btnTemp.AddItems (arrCompString6);
					switch (cx3parameter.Type)
					{
					case (int)ConstraintType.LessThan:
						btnTemp.Title = "Less than";
						break;
					case (int)ConstraintType.EqualTo:
						btnTemp.Title = "Equal";
						break;

					case (int)ConstraintType.MoreThan:
						btnTemp.Title = "More than";
						break;

					}

					break;

				case 43:
					btnTemp.AddItems (arrDHFOLValue.ConvertAll (x => x.ToString ()).ToArray ());
					btnTemp.Title=cx3parameter.Value.ToString();
					break;
				}
			}

		}
		private void SetEquipmentTypeBA(Cx3Parameter cx3parameter)
		{
				for (int index = 51; index <= 53; index++) {

				NSPopUpButton btnTemp = (NSPopUpButton)this.ViewWithTag (index);
				btnTemp.RemoveAllItems ();
				switch (index) {
				case 51:
					btnTemp.AddItems (arrEQType);
					if (cx3parameter.ThirdcellValue==null)
					{
						cx3parameter.ThirdcellValue = "700";
						cx3parameter.Type = (int)ConstraintType.LessThan;
						cx3parameter.Value = arrAirCraft.FirstOrDefault ();
					}
					if (cx3parameter.ThirdcellValue != null)
					{
							if (cx3parameter.ThirdcellValue == "35")
								btnTemp.Title = "700";
							else if (cx3parameter.ThirdcellValue == "600")
								btnTemp.Title = "8Max";
							else if (cx3parameter.ThirdcellValue == "200")
								btnTemp.Title = "7Max";
							//btnTemp.Title = "300 & 500";
							else
								btnTemp.Title = cx3parameter.ThirdcellValue;
					}
					break;
				case 52:
					btnTemp.AddItems (arrCompString6);
					switch (cx3parameter.Type)
					{
					case (int)ConstraintType.LessThan:
						btnTemp.Title = "Less than";
						break;
					case (int)ConstraintType.EqualTo:
						btnTemp.Title = "Equal";
						break;

					case (int)ConstraintType.MoreThan:
						btnTemp.Title = "More than";
						break;

					}

					break;

				case 53:
					btnTemp.AddItems (arrAirCraft.ConvertAll (x => x.ToString ()).ToArray ());
					btnTemp.Title=cx3parameter.Value.ToString();
					break;
				}
			}

		}
		private void SetLineTypeParameter(CxLine objCxLine)
		{
			bool status=false;
			for (int index = 21; index <= 25; index++) {
				NSButton btnTemp = (NSButton)this.ViewWithTag (index);

				switch(index)
				{
				case 21: 
					status=objCxLine.Hard;
					break;
				case 22:
					status=objCxLine.Reserve;
					break;
				case 23:
					if (GlobalSettings.CurrentBidDetails.Postion=="FA") {
						btnTemp.Title = "Ready";
						status = objCxLine.Ready;
					} else {
						btnTemp.Title = "Blank";
						status = objCxLine.Blank;
					}

					break;
				case 24:
					status=objCxLine.International;
					break;
				case 25:
					status=objCxLine.NonConus;
					break;


				}

				ButtonLayout (btnTemp, status);
			}
		}
		private void SetTripBlockLengthParameter(CxTripBlockLength objCxTBL)
		{
			bool status=false;
			for (int index = 81; index <= 86; index++) {
				NSButton btnTemp = (NSButton)this.ViewWithTag (index);

				switch(index)
				{
				case 81: 
					status=objCxTBL.Turns;
					break;
				case 82:
					status=objCxTBL.Twoday;
					break;
				case 83:
					status=objCxTBL.ThreeDay;
					break;
				case 84:
					status=objCxTBL.FourDay;
					break;
				case 85:
					status=!objCxTBL.IsBlock;
					break;
				case 86:
					status=objCxTBL.IsBlock;
					break;

				}

				ButtonLayout (btnTemp, status);
			}
		}
		private void SetRestBA(Cx3Parameter cx3parameter)
		{
			for (int index = 61; index <= 63; index++) {

				NSPopUpButton btnTemp = (NSPopUpButton)this.ViewWithTag (index);
				btnTemp.RemoveAllItems ();
				switch (index) {
				case 61:
					//string[] arrRest = { "All", "InDom", "AwayDom" };
					btnTemp.AddItems (arrRest);
					if (cx3parameter.ThirdcellValue == null) {
						cx3parameter.ThirdcellValue = "1";
						cx3parameter.Type = (int)ConstraintType.LessThan;
						cx3parameter.Value = arrRestValue.FirstOrDefault ();
					}
					if (cx3parameter.Value == 0)
						cx3parameter.Value = 8;

						switch (int.Parse(cx3parameter.ThirdcellValue)) {
						case (int)RestType.All:
							btnTemp.Title = "All";
							break;

						case (int)RestType.InDomicile:
							btnTemp.Title = "InDom";
							break;
						case (int)RestType.AwayDomicile:
							btnTemp.Title = "AwayDom";
							break;

						}
				
					break;
				case 62:
					btnTemp.AddItems (arrCompString1);

					switch (cx3parameter.Type)
					{
					case (int)ConstraintType.LessThan:
						btnTemp.Title = "Less than";
						break;
					
					case (int)ConstraintType.MoreThan:
						btnTemp.Title = "More than";
						break;

					}

					break;

				case 63:
					btnTemp.AddItems (arrRestValue.ConvertAll (x => x.ToString ()).ToArray ());
					if(cx3parameter.Value.ToString() != "0")btnTemp.Title=cx3parameter.Value.ToString();
					else btnTemp.Title="8";
					break;
				}
			}

		}
		private void SetDaysOfWeekAllParameter(CxDays objCxDays)
		{
			//
		
			bool status=false;
			for (int index = 11; index <= 17; index++) {
					NSButton btnTemp = (NSButton)this.ViewWithTag (index);

					switch(index)
				{
				case 11: 
					status=objCxDays.IsSun;
					break;
				case 12:
					status=objCxDays.IsMon;
					break;
				case 13:
					
					status=objCxDays.IsTue;
					break;
				case 14:
					status=objCxDays.IsWed;
					break;
				case 15:
					status=objCxDays.IsThu;
					break;
				case 16:
					status=objCxDays.IsFri;
					break;
				case 17:
					status=objCxDays.IsSat;
					break;

				}

				ButtonLayout (btnTemp, status);
			}
		}


		private void SetDaysOfMonth(DaysOfMonthCx ObjCxDays)
		{
			NSTextField button = ((NSTextField)this.ViewWithTag (112));
		//	BidAutoHelper biduatohelper = new BidAutoHelper ();
			//var dom = ObjCxDays;
			var domdata = ObjCxDays;
			List<WBid.WBidiPad.Model.DaysOfMonth> dayOfMonthList = WBidCollection.GetDaysOfMonthList();

			if (domdata == null) {
				button.StringValue ="Work[] OFF[]";
				return;
			}
			string yesStr = "Work[";
			if (domdata.WorkDays == null || domdata.WorkDays.Count==0) {
				yesStr = "Work[]";
			} else {
				for (int i = 0; i < domdata.WorkDays.Count; i++) {
					if (i == domdata.WorkDays.Count - 1) {
						// last element
						yesStr =yesStr + GetCaledervalue(dayOfMonthList,  domdata.WorkDays [i])+ "]";
					} else {
						yesStr = yesStr + GetCaledervalue(dayOfMonthList,domdata.WorkDays [i])+ ",";
					}
				}
			}

			string noStr = "OFF[";
			if (domdata.OFFDays == null || domdata.OFFDays.Count==0) {
				noStr = "OFF[]";
			} else {
				for (int i = 0; i < domdata.OFFDays.Count; i++) {
					if (i == domdata.OFFDays.Count - 1) {
						// last element
						noStr = noStr + GetCaledervalue(dayOfMonthList,domdata.OFFDays [i])+ "]";
					} else {
						noStr = noStr + GetCaledervalue(dayOfMonthList,domdata.OFFDays [i])+ ",";
					}
				}
			}
			button.StringValue = string.Format ("{0} {1}", yesStr, noStr);

		}

		private void SetStartDaysOfWeekAllParameter(CxDays objCxDays)
		{
			//
			bool status=false;
			for (int index = 71; index <= 77; index++) {
				NSButton btnTemp = (NSButton)this.ViewWithTag (index);

				switch(index)
				{
				case 71: 
					status=objCxDays.IsSun;
					break;
				case 72:
					status=objCxDays.IsMon;
					break;
				case 73:

					status=objCxDays.IsTue;
					break;
				case 74:
					status=objCxDays.IsWed;
					break;
				case 75:
					status=objCxDays.IsThu;
					break;
				case 76:
					status=objCxDays.IsFri;
					break;
				case 77:
					status=objCxDays.IsSat;
					break;

				}

				ButtonLayout (btnTemp, status);
			}
		}
		partial void funEquimentTypetag51To53 (NSObject sender)
		{
			//51 to 53
			NSPopUpButton btnTemp=(NSPopUpButton)sender;
			Cx3Parameter objEqType=(Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			bool status=false;
			switch(btnTemp.Tag)
			{
			case 51:
				if(btnTemp.Title== "300 & 500")
					objEqType.ThirdcellValue="35";
					if(btnTemp.Title== "8Max")
					objEqType.ThirdcellValue="600";
					if (btnTemp.Title == "7Max")
						objEqType.ThirdcellValue = "200";
					else
				objEqType.ThirdcellValue=btnTemp.Title;
				break;
			case 52:
				
				switch(btnTemp.Title.ToLower())
				{
				case "less than":
					objEqType.Type=(int)ConstraintType.LessThan;
					break;
				case "equal":
					objEqType.Type=(int)ConstraintType.EqualTo;
					break;
				case "more than":
					objEqType.Type=(int)ConstraintType.MoreThan;
					break;

				}

				break;
			case 53:
				objEqType.Value=Convert.ToInt32(btnTemp.Title);
				break;

			}
			CommonClass.MainController.UpdateSaveButton (true);
		}
		partial void funDaysOfWeekAll (NSObject sender)
		{
			NSButton btnTemp=(NSButton)sender;
			CxDays objCxDays=(CxDays)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			bool status=false;
			switch(btnTemp.Tag)
			{
			case 11: 
				objCxDays.IsSun=!objCxDays.IsSun;
				status=objCxDays.IsSun;
				break;
			case 12:
				objCxDays.IsMon=!objCxDays.IsMon;
				status=objCxDays.IsMon;
				break;
			case 13:
				objCxDays.IsTue=!objCxDays.IsTue;
				status=objCxDays.IsTue;
				break;
			case 14:
				objCxDays.IsWed=!objCxDays.IsWed;
				status=objCxDays.IsWed;
				break;
			case 15:
				objCxDays.IsThu=!objCxDays.IsThu;
				status=objCxDays.IsThu;
				break;
			case 16:
				objCxDays.IsFri=!objCxDays.IsFri;
				status=objCxDays.IsFri;
				break;
			case 17:
				objCxDays.IsSat=!objCxDays.IsSat;
				status=objCxDays.IsSat;
				break;

			}
			ButtonLayout (btnTemp, status);


			CommonClass.MainController.UpdateSaveButton (true);
			// 11 to 17 tags

		}
		partial void funLinetype (NSObject sender)
		{
			// 21 to 25

			NSButton btnTemp=(NSButton)sender;
			CxLine objCxLine=(CxLine)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;

			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);

			bool status=false;
			switch(btnTemp.Tag)
			{
			case 21: 
				objCxLine.Hard=!objCxLine.Hard;
				status=objCxLine.Hard;
				break;

			case 22: 
				objCxLine.Reserve=!objCxLine.Reserve;
				status=objCxLine.Reserve;
				break;

			case 23: 
				if(btnTemp.Title=="Blank")
				{
					objCxLine.Blank=!objCxLine.Blank;
					status=objCxLine.Blank;
				}
				else
				{
					objCxLine.Ready=!objCxLine.Ready;
					status=objCxLine.Ready;
				}

				break;

			case 24: 
				objCxLine.International=!objCxLine.International;
				status=objCxLine.International;
				break;

			case 25: 
				objCxLine.NonConus=!objCxLine.NonConus;
				status=objCxLine.NonConus;
				break;
			}

			ButtonLayout (btnTemp, status);
			CommonClass.MainController.UpdateSaveButton (true);
			// 21 to 25

		}
		partial void funDowSome (NSObject sender)
		{
			//31 to 33
			NSPopUpButton btnTemp=(NSPopUpButton)sender;
			Cx3Parameter objdowSum=(Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			bool status=false;
			switch(btnTemp.Tag)
			{
			case 31:
				objdowSum.ThirdcellValue=btnTemp.IndexOfSelectedItem.ToString();
				switch(btnTemp.Title.ToLower())
				{
				case "mon":
					objdowSum.ThirdcellValue=((int)DayofTheWeek.Monday).ToString();
					break;
				case "tue":
					objdowSum.ThirdcellValue=((int)DayofTheWeek.Tuesday).ToString();
					break;
				case "wed":
					objdowSum.ThirdcellValue=((int)DayofTheWeek.Wednesday).ToString();
					break;
				case "thu":
					objdowSum.ThirdcellValue=((int)DayofTheWeek.Thursday).ToString();
					break;
				case "fri":
					objdowSum.ThirdcellValue=((int)DayofTheWeek.Friday).ToString();
					break;
				case "sat":
					objdowSum.ThirdcellValue=((int)DayofTheWeek.Saturday).ToString();
					break;
				case "sun":
					objdowSum.ThirdcellValue=((int)DayofTheWeek.Sunday).ToString();
					break;

				}
				break;
			case 32:
				switch(btnTemp.Title.ToLower())
				{
				case "less than":
					objdowSum.Type=(int)ConstraintType.LessThan;
					break;
				case "equal":
					objdowSum.Type=(int)ConstraintType.EqualTo;
					break;
					case "more than":
					objdowSum.Type=(int)ConstraintType.MoreThan;
					break;
					
				}

				break;
			case 33:
				objdowSum.Value=Convert.ToInt32(btnTemp.Title);
				break;
				
			}
			CommonClass.MainController.UpdateSaveButton (true);
		}

		partial void funDHFirstLast (NSObject sender)
		{
			//41 to 43
			NSPopUpButton btnTemp=(NSPopUpButton)sender;
			Cx3Parameter objDHFirstLastSum=(Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			bool status=false;
			switch(btnTemp.Tag)
			{
			case 41:
				switch(btnTemp.Title.ToLower())
				{
				case "first":
					objDHFirstLastSum.ThirdcellValue=((int)DeadheadType.First).ToString();
					break;
				case "last":
					objDHFirstLastSum.ThirdcellValue=((int)DeadheadType.Last).ToString();
					break;
				case "both":
					objDHFirstLastSum.ThirdcellValue=((int)DeadheadType.Both).ToString();
					break;

					}

				break;
			case 42:
				switch(btnTemp.Title.ToLower())
				{
				case "less than":
					objDHFirstLastSum.Type=(int)ConstraintType.LessThan;
					break;

				case "equal":
					objDHFirstLastSum.Type=(int)ConstraintType.EqualTo;
					break;

				case "more than":
					objDHFirstLastSum.Type=(int)ConstraintType.MoreThan;
					break;
				}

				break;
			case 43:
				objDHFirstLastSum.Value=Convert.ToInt32(btnTemp.Title);
				break;

			}
			CommonClass.MainController.UpdateSaveButton (true);
		}
		partial void funDaysOfMonth111to112 (NSObject sender)
		{
			NSButton btnDaysOfMonth= (NSButton)sender;
			if(btnDaysOfMonth.Tag == 111)
			{
				_parent.ShowDaysOfMonthCellClick((DaysOfMonthCx)wBIdStateContent.BidAuto.BAFilter [(int)order].BidAutoObject);
			}
			CommonClass.MainController.UpdateSaveButton (true);

		}
		partial void FunDeleteFilter (NSObject sender)
		{
			
			_parent.RemoveAndReloadConstraints (wBIdStateContent.BidAuto.BAFilter [(int)order].Name, (int)order);
			CommonClass.MainController.UpdateSaveButton (true);
		}

		public void BindData (string constraint, int index,BAFilterViewController parent)
		{
			_parent = parent;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [index].IsApplied);
//			var lst = CommonClass.ConstraintsController.appliedConstraints;
//			order = index - lst.IndexOf (lst.FirstOrDefault (x => x == constraint));
			order = index;
			switch (this.Identifier) 
			{

			case "DOWA":
				
				SetDaysOfWeekAllParameter ((CxDays)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "AP":
				SetAmPmFilter ((AMPMConstriants)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				//btnClose.Tag = index;
				break;
			case "DOWS":
				SetDaysofWeekSome ((Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "DHFL":
				SetDHFirstLast ((Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;

			case "ET":
				SetEquipmentTypeBA ((Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "RT":
				SetRestBA((Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "LT":
				SetLineTypeParameter((CxLine)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "TBL":
				SetTripBlockLengthParameter((CxTripBlockLength)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "SDOW":
				SetStartDaysOfWeekAllParameter ((CxDays)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;

			case "DOM":
				SetDaysOfMonth ((DaysOfMonthCx)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "OC":
				SetOverNightCities ((BulkOvernightCityCx)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;
			case "CL":
				SetCommutableLineAuto ((FtCommutableLine)wBIdStateContent.BidAuto.BAFilter [index].BidAutoObject);
				break;


			}




		}
		void SetCommutableLineAuto(FtCommutableLine CommutableLine)
		{
			NSButton btnName = (NSButton)ViewWithTag ((nint)131);
			btnName.Title= "Cmt Line ("+CommutableLine.City+")";


			NSButton btnNoNightInMiddle = (NSButton)ViewWithTag ((nint)132);
			ButtonLayout (btnNoNightInMiddle, CommutableLine.NoNights);

			NSButton btnToWork = (NSButton)ViewWithTag ((nint)133);
			ButtonLayout (btnToWork, CommutableLine.ToWork);

			NSButton btnToHome = (NSButton)ViewWithTag ((nint)134);
			ButtonLayout (btnToHome, CommutableLine.ToHome);
		}
		partial void funCommutableLineAuto131to134 (NSObject sender)
		{
			NSButton temp= (NSButton)sender;
			FtCommutableLine commutableLine=(FtCommutableLine)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			switch(temp.Tag)
			{
			case 131:_parent.ShowCommutableLine();
				break;
			case 132: 
				commutableLine.NoNights=!commutableLine.NoNights;
			
				break;
				
			case 134 : 
				if (commutableLine.ToWork == true) commutableLine.ToHome =! commutableLine.ToHome ;
				 
				break;
			case 133:
				if (commutableLine.ToHome == true) commutableLine.ToWork =! commutableLine.ToWork ;
				break;	
			}

			SetCommutableLineAuto ((FtCommutableLine)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject);
			CommonClass.MainController.UpdateSaveButton (true);
		} 
		void SetOverNightCities(BulkOvernightCityCx City)
		{
			NSTextField button = ((NSTextField)this.ViewWithTag (122));
			//BidAutoHelper biduatohelper = new BidAutoHelper ();
			//var overnightcity = City;
			var overnightcitydata =City;

			if (overnightcitydata == null) {
				button.StringValue ="yes[] no[]";
				return;
			}
			string yesStr = "yes[";
			if (overnightcitydata.OverNightYes == null || overnightcitydata.OverNightYes.Count==0) {
				yesStr = "yes[]";
			} else {
				for (int i = 0; i < overnightcitydata.OverNightYes.Count; i++) {
					if (i == overnightcitydata.OverNightYes.Count - 1) {
						// last element
						yesStr = yesStr + getOvernightCitynameFromId( overnightcitydata.OverNightYes [i])+ "]";
					} else {
						yesStr = yesStr + getOvernightCitynameFromId(overnightcitydata.OverNightYes [i])+ ",";
					}
				}
			}

			string noStr = "no[";
			if (overnightcitydata.OverNightNo == null || overnightcitydata.OverNightNo.Count==0) {
				noStr = "no[]";
			} else {
				for (int i = 0; i < overnightcitydata.OverNightNo.Count; i++) {
					if (i == overnightcitydata.OverNightNo.Count - 1) {
						// last element
						noStr = noStr + getOvernightCitynameFromId(overnightcitydata.OverNightNo [i])+ "]";
					} else {
						noStr = noStr + getOvernightCitynameFromId(overnightcitydata.OverNightNo [i])+ ",";
					}
				}
			}
			button.StringValue = string.Format ("{0} {1}", yesStr, noStr);
		}
		partial void funOverNightCities121to122 (NSObject sender)
		{
			BulkOvernightCityCx objOverNightCity=(BulkOvernightCityCx)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			_parent.ShowOvernightCitiesCellClick(objOverNightCity);
			CommonClass.MainController.UpdateSaveButton (true);
//			NSButton btn=(NSButton)sender;
//			switch(btn.Tag)
//			{
//			case 121:_parent.ShowOvernightCities();
//				
//				break;
//				case 122 :
//				break;
//			}
				
		}
		/// <summary>
		/// Updates the overnight label from the overnight city pop over
		/// </summary>
	
		/// <summary>
		/// Updates the overnight label from the overnight city pop over
		/// </summary>
		private void UpdateDaysofMonthLabel ()
		{
					}
		private string GetCaledervalue(List<WBid.WBidiPad.Model.DaysOfMonth> dayOfMonthList ,int cityId)
		{
			string day=	(dayOfMonthList.FirstOrDefault (x => x.Id == cityId).Day);
			return day;
		}
		private string getOvernightCitynameFromId(int id)
		{
			
			string city = string.Empty;
			var citydata = (GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == id));
			if (citydata != null) {
				city = citydata.Name;
			}
			return city;
		}
		partial void funCommuteAutotype101to107 (NSObject sender)
		{
			
		}
		partial void funRestTypetag61To63 (NSObject sender)
		{
			//61 to 63
			NSPopUpButton btnTemp=(NSPopUpButton)sender;
			Cx3Parameter objRestBA=(Cx3Parameter)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			bool status=false;
			switch(btnTemp.Tag)
			{
			case 61:
				switch(btnTemp.Title.ToLower())
				{
			case "all":
					objRestBA.ThirdcellValue=((int)RestType.All).ToString();
				break;
				case "indom":
					objRestBA.ThirdcellValue=((int)RestType.InDomicile).ToString();
					break;
				case "awaydom":
					objRestBA.ThirdcellValue=((int)RestType.AwayDomicile).ToString();
					break;
				}

				break;
			case 62:
				switch(btnTemp.Title.ToLower())
				{
				case "more than":
					objRestBA.Type=(int)ConstraintType.MoreThan;
					break;
				case "less than":
					objRestBA.Type=(int)ConstraintType.LessThan;
					break;
				}

				break;
			case 63:
				objRestBA.Value=Convert.ToInt32(btnTemp.Title);
				break;

			}
			CommonClass.MainController.UpdateSaveButton (true);

		}
		partial void funSDOWtype71to77 (NSObject sender)
		{
			NSButton btnTemp=(NSButton)sender;
			CxDays objCxDays=(CxDays)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			bool status=false;
			switch(btnTemp.Tag)
			{
			case 71: 
				objCxDays.IsSun=!objCxDays.IsSun;

				status=objCxDays.IsSun;
                    if(objCxDays.IsSun)
                    {
                        objCxDays.IsMon=false;
                        objCxDays.IsTue=false;
                        objCxDays.IsWed=false;
                        objCxDays.IsThu=false;
                        objCxDays.IsFri=false;
                        objCxDays.IsSat=false;
                     }
				break;
			case 72:
				objCxDays.IsMon=!objCxDays.IsMon;
				status=objCxDays.IsMon;
                    if(objCxDays.IsMon)
                    {
                        objCxDays.IsSun=false;
                        objCxDays.IsTue=false;
                        objCxDays.IsWed=false;
                        objCxDays.IsThu=false;
                        objCxDays.IsFri=false;
                        objCxDays.IsSat=false;
                     }
				break;
			case 73:
				objCxDays.IsTue=!objCxDays.IsTue;
				status=objCxDays.IsTue;
                    if(objCxDays.IsTue)
                    {
                        objCxDays.IsMon=false;
                        objCxDays.IsWed=false;
                        objCxDays.IsThu=false;
                        objCxDays.IsFri=false;
                        objCxDays.IsSat=false;
                        objCxDays.IsSun=false;
                    }
				break;
			case 74:
				objCxDays.IsWed=!objCxDays.IsWed;
				status=objCxDays.IsWed;
                    if(objCxDays.IsWed)
                    {
                        objCxDays.IsMon=false;
                        objCxDays.IsTue=false;
                        objCxDays.IsThu=false;
                        objCxDays.IsFri=false;
                        objCxDays.IsSat=false;
                        objCxDays.IsSun=false;
                    }
				break;
			case 75:
				objCxDays.IsThu=!objCxDays.IsThu;
				status=objCxDays.IsThu;
                    if(objCxDays.IsThu)
                    {
                        objCxDays.IsMon=false;
                        objCxDays.IsTue=false;
                        objCxDays.IsWed=false;
                        objCxDays.IsFri=false;
                        objCxDays.IsSat=false;
                        objCxDays.IsSun=false;
                    }
				break;
			case 76:
				objCxDays.IsFri=!objCxDays.IsFri;
				status=objCxDays.IsFri;
                    if(objCxDays.IsFri)
                    {
                        objCxDays.IsMon=false;
                        objCxDays.IsTue=false;
                        objCxDays.IsWed=false;
                        objCxDays.IsThu=false;
                        objCxDays.IsSat=false;
                        objCxDays.IsSun=false;
                    }
				break;
			case 77:
				objCxDays.IsSat=!objCxDays.IsSat;
				status=objCxDays.IsSat;
                    if(objCxDays.IsSat)
                    {
                        objCxDays.IsMon=false;
                        objCxDays.IsTue=false;
                        objCxDays.IsWed=false;
                        objCxDays.IsThu=false;
                        objCxDays.IsFri=false;
                        objCxDays.IsSun=false;
                    }
				break;

			}
			ButtonLayout (btnTemp, status);
            SetStartDaysOfWeekAllParameter(objCxDays);
			CommonClass.MainController.UpdateSaveButton (true);
		}
		partial void funTrpBlkLen81t087 (NSObject sender)
		{
			NSButton btnTemp=(NSButton)sender;
			CxTripBlockLength objTbl=(CxTripBlockLength)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject;
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);
			bool status=false;
			switch(btnTemp.Tag)
			{
			case 81: 
				objTbl.Turns=!objTbl.Turns;
				status=objTbl.Turns;
				ButtonLayout (btnTemp, status);
				break;
			case 82:
				objTbl.Twoday=!objTbl.Twoday;
				status=objTbl.Twoday;
				ButtonLayout (btnTemp, status);
				break;
			case 83:
				objTbl.ThreeDay=!objTbl.ThreeDay;
				status=objTbl.ThreeDay;
				ButtonLayout (btnTemp, status);
				break;
			case 84:
				objTbl.FourDay=!objTbl.FourDay;
				status=objTbl.FourDay;
				ButtonLayout (btnTemp, status);
				break;
			case 85:
				if(objTbl.IsBlock)
				{
				objTbl.IsBlock=false;
				status=objTbl.IsBlock;
				ButtonLayout (btnTemp, !status);
				ButtonLayout(((NSButton)this.ViewWithTag (86)),status);
				}
				break;
			case 86:
				if(objTbl.IsBlock==false)
				{
					objTbl.IsBlock=true;
				status=objTbl.IsBlock;
				ButtonLayout (btnTemp, status);
				ButtonLayout(((NSButton)this.ViewWithTag (85)),!status);
				}
				break;
			

			}
			CommonClass.MainController.UpdateSaveButton (true);
		}		


		partial void funAPButtonSelection (NSObject sender)
		{
			NSButton btnAMPM= (NSButton)sender;
			AMPMConstriants ampmitem= ((AMPMConstriants)wBIdStateContent.BidAuto.BAFilter [order].BidAutoObject);
			wBIdStateContent.BidAuto.BAFilter [order].IsApplied=false;
			FunIscalculated (wBIdStateContent.BidAuto.BAFilter [order].IsApplied);

			switch (btnAMPM.Tag)
			{
			case 1:
				ampmitem.AM=!ampmitem.AM;

				ButtonLayout(btnAMPM,ampmitem.AM);

				break;
				case 2:
				ampmitem.PM=!ampmitem.PM;
				ButtonLayout(btnAMPM,ampmitem.PM);
				break;
			case 3:
				ampmitem.MIX=!ampmitem.MIX;
				ButtonLayout(btnAMPM,ampmitem.MIX);
				break;
			}
			CommonClass.MainController.UpdateSaveButton (true);
		}
		void BtnClose_Activated (object sender, EventArgs e)
		{
			NSButton btnCloses = (NSButton)sender;

			_parent.RemoveAndReloadConstraints (wBIdStateContent.BidAuto.BAFilter [(int)btnCloses.Tag].Name, (int)btnCloses.Tag);

		}

		void EnableDisableTimeFields ()
		{
			
		}
	}









}

