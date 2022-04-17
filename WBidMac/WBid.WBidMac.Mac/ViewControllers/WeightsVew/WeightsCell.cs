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
    public partial class WeightsCell : AppKit.NSTableCellView
    {
        public WeightsCell()
        {
        }

        int order;
        #region Constructors
        public static WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        static WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

        // Weights Param Popover
        string[] arrSecondParam1 = { "less", "more", "equal", "not equal" };
        string[] arrSecondParam2 = { "relative", "longer", "shorter" };
        string[] arrSecondParam3 = { "less", "more" };
        string[] arrSecondParam4 = { "all", "more", "less" };
        string[] arrSecondParam5 = { "shorter", "+ & -", "longer" };
        string[] arrSecondParam6 = { "all", "away", "inDom" };
        string[] arrSecondParam7 = { "less", "more", "equal" };

        string[] arrThirdParam1 = { "am", "pm", "nte" };
        string[] arrThirdParam2 = { "first", "last" };
        string[] arrThirdParam3 = { "A", "B", "C", "D" };
        string[] arrThirdParam4 = { "Turn", "2 day", "3 day", "4 day" };
        string[] arrDays = { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };
        string[] arrCmut = { "begin", "end", "both" };
        string[] arrEQType = { "700", "800", "7Max", "8Max" };

        List<int> arrAirCraftThirdValue = Enumerable.Range(1, 30 - 1 + 1).ToList();
        List<int> arrBDOThirdValue = Enumerable.Range(1, 31 - 1 + 1).ToList();
        List<string> lstDPTimes = WeightBL.GetTimeList(5, 16, 5);
        List<int> arrFltTimeValue = Enumerable.Range(20, 140 - 20 + 1).ToList();
        List<string> lstGrdTimes = WeightBL.GetTimeList(0, 6, 5);
        List<int> arrGrdValue = Enumerable.Range(1, 25 - 1 + 1).ToList();
        List<int> arrLegsDPValue = Enumerable.Range(0, 9 - 0 + 1).ToList();
        List<int> arrNODOThirdValue = Enumerable.Range(9, 31 - 9 + 1).ToList();
        List<int> arrTimeAwayThirdValue = Enumerable.Range(100, 300 - 100 + 1).ToList();
        List<int> arrWorkDaysThirdValue = Enumerable.Range(0, 20 - 0 + 1).ToList();
        List<string> lstPDOTimeSecValue = WeightBL.GetTimeList(3, 27, 15);
        public List<IntlNonConusCity> intlNonConusCities = WeightBL.GetIntlNonConusCities();
        List<string> lstRestTimeSecValue = WeightBL.GetTimeList(8, 24, 60);
        List<DateHelper> lstPDODays = ConstraintBL.GetPartialDayList();

        public static List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();
        public static List<Wt> lstDOMWeights = wBIdStateContent.Weights.SDO.Weights;
        public static List<City> lstOvernightCities = GlobalSettings.WBidINIContent.Cities;
        public static List<Wt2Parameter> lstBulkWeights = wBIdStateContent.Weights.OvernightCitybulk;
        string[] arrCommutabilityFirstCellValue = { "No Middle", "Ok Middle" };
        string[] arrCommutabilitySecondCellValue = { "Front", "Back", "Overall" };
        string[] arrCommutabilityThirdCellValue = { "<=", ">=" };
        List<int> arrCommutabilityValue = Enumerable.Range(0, 100).ToList();
        List<DateHelper> newPdoDateList = ConstraintBL.GetPartialDayList();
        List<string> newPdoDateStringList = new List<string>();
        private List<string> getComutabilityValues()
        {
            List<string> lstvalue = new List<string>();
            for (int i = 0; i <= 100; i = i + 5)
            {
                lstvalue.Add(i + "%");
            }
            return lstvalue;
        }
        // Called when created from unmanaged code
        public WeightsCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public WeightsCell(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

            if (newPdoDateStringList.Count > 0)
            {
            }
            else
            {
                newPdoDateStringList.Insert(0, "Any Date");
                for (int date = 0; date < newPdoDateList.Count; date++)
                {
                    DateTime date1 = newPdoDateList[date].Date;
                    string str = date1.ToString("dd - MMM");
                    newPdoDateStringList.Insert(date + 1, str);
                }
            }
            lstDOMWeights = wBIdStateContent.Weights.SDO.Weights;
            lstBulkWeights = wBIdStateContent.Weights.OvernightCitybulk;

            switch (this.Identifier)
            {
                //			case "Commutable Lines - Auto":
                //				{
                //					NSButton btnNight = (NSButton)this.ViewWithTag (101);
                //					btnNight.Activated += (object sender, EventArgs e) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt)
                //						{
                //						wBIdStateContent.Weights.CLAuto.NoNights=!wBIdStateContent.Weights.CLAuto.NoNights;
                //						ButtonLayout(btnNight,wBIdStateContent.Weights.CLAuto.NoNights);
                //						CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //					};
                //					NSButton btnClose = (NSButton)this.ViewWithTag (104);
                //					btnClose.Activated += (object sender1, EventArgs e1) => {
                //
                //						CommonClass.WeightsController.RemoveAndReloadWeights ("Commutable Lines - Auto", order);
                //
                //					};
                //
                //					NSButton btnTowork = (NSButton)this.ViewWithTag (102);
                //					btnTowork.Activated += (object sender2, EventArgs e2) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt)
                //						{
                //						wBIdStateContent.Weights.CLAuto.ToWork=!wBIdStateContent.Weights.CLAuto.ToWork;
                //						ButtonLayout(btnTowork,wBIdStateContent.Weights.CLAuto.ToWork);
                //						CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //					};
                //					NSButton btnToHome = (NSButton)this.ViewWithTag (103);
                //					btnToHome.Activated += (object sender3, EventArgs e3) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt)
                //						{
                //						wBIdStateContent.Weights.CLAuto.ToHome=!wBIdStateContent.Weights.CLAuto.ToHome;
                //						ButtonLayout(btnToHome,wBIdStateContent.Weights.CLAuto.ToHome);
                //						CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //
                //					};
                //					NSButton btnHeadername = (NSButton)this.ViewWithTag (106);
                //					if(wBIdStateContent.CxWtState.CLAuto.Wt)
                //					{
                //						btnHeadername.Title = "Cmt Line" + " [" + wBIdStateContent.Weights.CLAuto.City + "]";
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
                //			case "Commutable Lines - Auto":
                //				{
                //					NSButton btnNight = (NSButton)this.ViewWithTag (101);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.NoNights != null) {
                //
                //						ButtonLayout(btnNight,wBIdStateContent.Weights.CLAuto.NoNights);
                //					}
                //					btnNight.Activated += (object sender, EventArgs e) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.NoNights!=null)
                //						{
                //							wBIdStateContent.Weights.CLAuto.NoNights=!wBIdStateContent.Weights.CLAuto.NoNights;
                //							ButtonLayout(btnNight,wBIdStateContent.Weights.CLAuto.NoNights);
                //							CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //					};
                //					NSButton btnClose = (NSButton)this.ViewWithTag (104);
                //					btnClose.Activated += (object sender1, EventArgs e1) => {
                //
                //						CommonClass.WeightsController.RemoveAndReloadWeights ("Commutable Lines - Auto", order);
                //
                //					};
                //
                //					NSButton btnTowork = (NSButton)this.ViewWithTag (102);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null) {
                //						ButtonLayout(btnTowork,wBIdStateContent.Weights.CLAuto.ToWork);
                //					}
                //					btnTowork.Activated += (object sender2, EventArgs e2) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight!=null)
                //						{
                //							wBIdStateContent.Weights.CLAuto.ToWork=!wBIdStateContent.Weights.CLAuto.ToWork;
                //							ButtonLayout(btnTowork,wBIdStateContent.Weights.CLAuto.ToWork);
                //							CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //					};
                //					NSButton btnToHome = (NSButton)this.ViewWithTag (103);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null) 
                //					{
                //						ButtonLayout(btnToHome,wBIdStateContent.Weights.CLAuto.ToHome);
                //					}
                //					btnToHome.Activated += (object sender3, EventArgs e3) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight!=null)
                //						{
                //							wBIdStateContent.Weights.CLAuto.ToHome=!wBIdStateContent.Weights.CLAuto.ToHome;
                //							ButtonLayout(btnToHome,wBIdStateContent.Weights.CLAuto.ToHome);
                //							CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //
                //					};
                //					NSButton btnHeadername = (NSButton)this.ViewWithTag (106);
                //					if(wBIdStateContent.CxWtState.CLAuto.Wt)
                //					{
                //						btnHeadername.Title = "Cmt Line" + " [" + wBIdStateContent.Weights.CLAuto.City + "]";
                //					}  else
                //						btnHeadername.Title = "Cmt Line";
                //					btnHeadername.Activated += (object sender3, EventArgs e3) => {
                //
                //						NSNotificationCenter.DefaultCenter.PostNotificationName ("CLOLoadNotification", null);
                //
                //					} ;
                //
                //					NSTextField text = (NSTextField)this.ViewWithTag (107);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight!=null) {
                //						text.StringValue = wBIdStateContent.Weights.CLAuto.Weight.ToString ();
                //					}
                //					text.Activated += txtCLOWtChanged;
                //
                //
                //				}
                //				break;
                case "View1":
                    {
                        txtWeight1.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (txtWeight1.StringValue == string.Empty)
                                txtWeight1.StringValue = "0";
                            if (this.lblTitle1.StringValue == "Largest Block of Days Off")
                            {
                                wBIdStateContent.Weights.LrgBlkDayOff.Weight = Decimal.Parse(txtWeight1.StringValue);
                            }
                            else if (this.lblTitle1.StringValue == "Normalize Days Off")
                            {
                                wBIdStateContent.Weights.NormalizeDaysOff.Weight = Decimal.Parse(txtWeight1.StringValue);
                            }
                            else if (this.lblTitle1.StringValue == "ETOPS")
                            {
                                wBIdStateContent.Weights.ETOPS.lstParameters[order].Weight = Decimal.Parse(txtWeight1.StringValue);
                            }
                            else if (this.lblTitle1.StringValue == "ETOPS-Res")
                            {
                                wBIdStateContent.Weights.ETOPSRes.lstParameters[order].Weight = Decimal.Parse(txtWeight1.StringValue);
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle1.StringValue);
                        };
                        btnClose1.Activated += (object sender, EventArgs e) =>
                        {
                            txtWeight1.AbortEditing();

                            CommonClass.WeightsController.RemoveAndReloadWeights(lblTitle1.StringValue, order);

                        };
                        btnHelp1.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo[lblTitle1.StringValue] - 1));
                        };

                    }
                    break;
                case "View2":
                    {
                        btnThird2.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (this.lblTitle2.StringValue == "AM/PM")
                            {
                                wBIdStateContent.Weights.AM_PM.lstParameters[order].Type = (int)btnThird2.SelectedTag;
                            }
                            else if (this.lblTitle2.StringValue == "DH - first - last")
                            {
                                wBIdStateContent.Weights.DHDFoL.lstParameters[order].Type = (int)btnThird2.SelectedTag;
                            }
                            else if (this.lblTitle2.StringValue == "Intl – NonConus")
                            {
                                var cityId = intlNonConusCities.FirstOrDefault(x => x.City == btnThird2.SelectedItem.Title).CityId;
                                wBIdStateContent.Weights.InterConus.lstParameters[order].Type = cityId;
                            }
                            else if (this.lblTitle2.StringValue == "Number of Days Off")
                            {
                                wBIdStateContent.Weights.NODO.lstParameters[order].Type = int.Parse(btnThird2.SelectedItem.Title.Replace(" off", ""));
                            }
                            else if (this.lblTitle2.StringValue == "Overnight Cities")
                            {
                                var cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == btnThird2.SelectedItem.Title).Id;
                                //var cityId = GlobalSettings.OverNightCitiesInBid.FirstOrDefault (x => x.Name == btnThird2.SelectedItem.Title).Id;
                                wBIdStateContent.Weights.RON.lstParameters[order].Type = cityId;
                            }
                            else if (this.lblTitle2.StringValue == "Cities-Legs")
                            {
                                var cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == btnThird2.SelectedItem.Title).Id;
                                wBIdStateContent.Weights.CitiesLegs.lstParameters[order].Type = cityId;
                            }
                            else if (this.lblTitle2.StringValue == "Position")
                            {
                                wBIdStateContent.Weights.POS.lstParameters[order].Type = (int)btnThird2.SelectedTag;
                            }
                            else if (this.lblTitle2.StringValue == "Start Day of Week")
                            {
                                wBIdStateContent.Weights.SDOW.lstParameters[order].Type = (int)btnThird2.SelectedTag;
                            }
                            else if (this.lblTitle2.StringValue == "Time-Away-From-Base")
                            {
                                wBIdStateContent.Weights.PerDiem.Type = int.Parse(btnThird2.SelectedItem.Title);
                            }
                            else if (this.lblTitle2.StringValue == "Trip Length")
                            {
                                wBIdStateContent.Weights.TL.lstParameters[order].Type = (int)btnThird2.SelectedTag;
                            }
                            else if (this.lblTitle2.StringValue == "Work Blk Length")
                            {
                                wBIdStateContent.Weights.WB.lstParameters[order].Type = (int)btnThird2.SelectedTag;
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle2.StringValue);
                        };

                        txtWeight2.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (txtWeight2.StringValue == string.Empty)
                                txtWeight2.StringValue = "0";

                            if (this.lblTitle2.StringValue == "AM/PM")
                            {
                                wBIdStateContent.Weights.AM_PM.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);

                            }
                            else if (this.lblTitle2.StringValue == "DH - first - last")
                            {
                                wBIdStateContent.Weights.DHDFoL.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Intl – NonConus")
                            {
                                wBIdStateContent.Weights.InterConus.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Number of Days Off")
                            {
                                wBIdStateContent.Weights.NODO.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Overnight Cities")
                            {
                                wBIdStateContent.Weights.RON.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Cities-Legs")
                            {
                                wBIdStateContent.Weights.CitiesLegs.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Position")
                            {
                                wBIdStateContent.Weights.POS.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Start Day of Week")
                            {
                                wBIdStateContent.Weights.SDOW.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Time-Away-From-Base")
                            {
                                wBIdStateContent.Weights.PerDiem.Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Trip Length")
                            {
                                wBIdStateContent.Weights.TL.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            else if (this.lblTitle2.StringValue == "Work Blk Length")
                            {
                                wBIdStateContent.Weights.WB.lstParameters[order].Weight = Decimal.Parse(txtWeight2.StringValue);
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle2.StringValue);
                        };
                        btnClose2.Activated += (object sender, EventArgs e) =>
                        {
                            txtWeight2.AbortEditing();

                            CommonClass.WeightsController.RemoveAndReloadWeights(lblTitle2.StringValue, order);
                        };
                        btnHelp2.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo[lblTitle2.StringValue] - 1));
                        };

                    }
                    break;
                case "View3":
                    {
                        btnSecond3.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (this.lblTitle3.StringValue == "Aircraft Changes")
                            {
                                wBIdStateContent.Weights.AirCraftChanges.SecondlValue = (int)btnSecond3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Blocks of Days Off")
                            {
                                wBIdStateContent.Weights.BDO.lstParameters[order].SecondlValue = (int)btnSecond3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Cmut DHs")
                            {
                                var cityId = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == btnSecond3.SelectedItem.Title).Id;
                                wBIdStateContent.Weights.DHD.lstParameters[order].SecondlValue = cityId;
                            }
                            else if (this.lblTitle3.StringValue == "Duty period")
                            {
                                wBIdStateContent.Weights.DP.lstParameters[order].SecondlValue = (int)btnSecond3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Equipment Type")
                            {
                                int secondcell;
                                if (btnSecond3.SelectedItem.Title == "8Max")
                                    secondcell = 600;
                                else if (btnSecond3.SelectedItem.Title == "7Max")
                                    secondcell = 200;
                                else
                                    secondcell = int.Parse(btnSecond3.SelectedItem.Title);
                                //int secondcell = (btnSecond3.SelectedItem.Title == "MAX") ? 600 : int.Parse(btnSecond3.SelectedItem.Title);
                                wBIdStateContent.Weights.EQUIP.lstParameters[order].SecondlValue = secondcell;
                            }
                            else if (this.lblTitle3.StringValue == "Flight Time")
                            {
                                wBIdStateContent.Weights.FLTMIN.lstParameters[order].SecondlValue = (int)btnSecond3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Ground Time")
                            {
                                wBIdStateContent.Weights.GRD.lstParameters[order].SecondlValue = Helper.ConvertHHMMtoMinute(btnSecond3.SelectedItem.Title);
                            }
                            else if (this.lblTitle3.StringValue == "Legs Per Duty Period")
                            {
                                wBIdStateContent.Weights.LEGS.lstParameters[order].SecondlValue = (int)btnSecond3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Legs Per Pairing")
                            {
                                wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[order].SecondlValue = (int)btnSecond3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Work Days")
                            {
                                wBIdStateContent.Weights.WorkDays.lstParameters[order].SecondlValue = (int)btnSecond3.SelectedTag;
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle3.StringValue);
                        };
                        btnThird3.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (this.lblTitle3.StringValue == "Aircraft Changes")
                            {
                                wBIdStateContent.Weights.AirCraftChanges.ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title.Replace(" chg", ""));
                            }
                            else if (this.lblTitle3.StringValue == "Blocks of Days Off")
                            {
                                wBIdStateContent.Weights.BDO.lstParameters[order].ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title.Replace(" blk", ""));
                            }
                            else if (this.lblTitle3.StringValue == "Cmut DHs")
                            {
                                wBIdStateContent.Weights.DHD.lstParameters[order].ThrirdCellValue = (int)btnThird3.SelectedTag;
                            }
                            else if (this.lblTitle3.StringValue == "Duty period")
                            {
                                wBIdStateContent.Weights.DP.lstParameters[order].ThrirdCellValue = Helper.ConvertHHMMtoMinute(btnThird3.SelectedItem.Title);
                            }
                            else if (this.lblTitle3.StringValue == "Equipment Type")
                            {
                                wBIdStateContent.Weights.EQUIP.lstParameters[order].ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title.Replace(" legs", ""));
                            }
                            else if (this.lblTitle3.StringValue == "Flight Time")
                            {
                                wBIdStateContent.Weights.FLTMIN.lstParameters[order].ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title);
                            }
                            else if (this.lblTitle3.StringValue == "Ground Time")
                            {
                                wBIdStateContent.Weights.GRD.lstParameters[order].ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title.Replace(" occurs", ""));
                            }
                            else if (this.lblTitle3.StringValue == "Legs Per Duty Period")
                            {
                                wBIdStateContent.Weights.LEGS.lstParameters[order].ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title.Replace(" legs", ""));
                            }
                            else if (this.lblTitle3.StringValue == "Legs Per Pairing")
                            {
                                wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[order].ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title.Replace(" legs", ""));
                            }
                            else if (this.lblTitle3.StringValue == "Work Days")
                            {
                                wBIdStateContent.Weights.WorkDays.lstParameters[order].ThrirdCellValue = int.Parse(btnThird3.SelectedItem.Title.Replace(" wk days", ""));
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle3.StringValue);
                        };
                        txtWeight3.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (txtWeight3.StringValue == string.Empty)
                                txtWeight3.StringValue = "0";

                            if (this.lblTitle3.StringValue == "Aircraft Changes")
                            {
                                wBIdStateContent.Weights.AirCraftChanges.Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Blocks of Days Off")
                            {
                                wBIdStateContent.Weights.BDO.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Cmut DHs")
                            {
                                wBIdStateContent.Weights.DHD.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Duty period")
                            {
                                wBIdStateContent.Weights.DP.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Equipment Type")
                            {
                                wBIdStateContent.Weights.EQUIP.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Flight Time")
                            {
                                wBIdStateContent.Weights.FLTMIN.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Ground Time")
                            {
                                wBIdStateContent.Weights.GRD.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Legs Per Duty Period")
                            {
                                wBIdStateContent.Weights.LEGS.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Legs Per Pairing")
                            {
                                wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            else if (this.lblTitle3.StringValue == "Work Days")
                            {
                                wBIdStateContent.Weights.WorkDays.lstParameters[order].Weight = Decimal.Parse(txtWeight3.StringValue);
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle3.StringValue);
                        };
                        btnClose3.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            txtWeight3.AbortEditing();
                            CommonClass.WeightsController.RemoveAndReloadWeights(lblTitle3.StringValue, order);
                        };
                        btnHelp3.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo[lblTitle3.StringValue] - 1));
                        };

                    }
                    break;
                case "View4":
                    {
                        btnFirst4.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (this.lblTitle4.StringValue == "PDO-after")
                            {
                                if (btnFirst4.SelectedItem.Title == "Any Date")
                                {
                                    wBIdStateContent.Weights.PDAfter.lstParameters[order].FirstValue = 300;
                                }
                                else
                                {
                                    var lst = lstPDODays.ConvertAll(x => x.Date.ToString("dd - MMM")).ToList();
                                    var ind = lst.IndexOf(btnFirst4.SelectedItem.Title);

                                    wBIdStateContent.Weights.PDAfter.lstParameters[order].FirstValue = lstPDODays[ind].DateId;
                                }
                            }
                            else if (this.lblTitle4.StringValue == "PDO-before")
                            {
                                if (btnFirst4.SelectedItem.Title == "Any Date")
                                {
                                    wBIdStateContent.Weights.PDBefore.lstParameters[order].FirstValue = 300;
                                }
                                else
                                {
                                    var lst = lstPDODays.ConvertAll(x => x.Date.ToString("dd - MMM")).ToList();
                                    var ind = lst.IndexOf(btnFirst4.SelectedItem.Title);
                                    wBIdStateContent.Weights.PDBefore.lstParameters[order].FirstValue = lstPDODays[ind].DateId;
                                }
                            }
                            else if (this.lblTitle4.StringValue == "Rest")
                            {
                                wBIdStateContent.Weights.WtRest.lstParameters[order].FirstValue = (int)btnFirst4.SelectedTag;
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle4.StringValue);
                        };
                        btnSecond4.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (this.lblTitle4.StringValue == "PDO-after")
                            {
                                wBIdStateContent.Weights.PDAfter.lstParameters[order].SecondlValue = Helper.ConvertHHMMtoMinute(btnSecond4.SelectedItem.Title);
                            }
                            else if (this.lblTitle4.StringValue == "PDO-before")
                            {
                                wBIdStateContent.Weights.PDBefore.lstParameters[order].SecondlValue = Helper.ConvertHHMMtoMinute(btnSecond4.SelectedItem.Title);
                            }
                            else if (this.lblTitle4.StringValue == "Rest")
                            {
                                wBIdStateContent.Weights.WtRest.lstParameters[order].SecondlValue = Helper.ConvertHHMMtoMinute(btnSecond4.SelectedItem.Title);
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle4.StringValue);
                        };
                        btnThird4.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (this.lblTitle4.StringValue == "PDO-after")
                            {
                                if (btnThird4.SelectedItem.Title == "Any City")
                                {
                                    wBIdStateContent.Weights.PDAfter.lstParameters[order].ThrirdCellValue = 400;
                                }
                                else
                                {
                                    var id = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == btnThird4.SelectedItem.Title).Id;
                                    wBIdStateContent.Weights.PDAfter.lstParameters[order].ThrirdCellValue = id;
                                }

                            }
                            else if (this.lblTitle4.StringValue == "PDO-before")
                            {
                                if (btnThird4.SelectedItem.Title == "Any City")
                                {
                                    wBIdStateContent.Weights.PDBefore.lstParameters[order].ThrirdCellValue = 400;
                                }
                                else
                                {
                                    var id = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == btnThird4.SelectedItem.Title).Id;
                                    wBIdStateContent.Weights.PDBefore.lstParameters[order].ThrirdCellValue = id;
                                }
                            }
                            else if (this.lblTitle4.StringValue == "Rest")
                            {
                                wBIdStateContent.Weights.WtRest.lstParameters[order].ThrirdCellValue = (int)btnThird4.SelectedTag;
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle4.StringValue);
                        };
                        txtWeight4.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (txtWeight4.StringValue == string.Empty)
                                txtWeight4.StringValue = "0";

                            if (this.lblTitle4.StringValue == "PDO-after")
                            {
                                wBIdStateContent.Weights.PDAfter.lstParameters[order].Weight = Decimal.Parse(txtWeight4.StringValue);
                            }
                            else if (this.lblTitle4.StringValue == "PDO-before")
                            {
                                wBIdStateContent.Weights.PDBefore.lstParameters[order].Weight = Decimal.Parse(txtWeight4.StringValue);
                            }
                            else if (this.lblTitle4.StringValue == "Rest")
                            {
                                wBIdStateContent.Weights.WtRest.lstParameters[order].Weight = Decimal.Parse(txtWeight4.StringValue);
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights(lblTitle4.StringValue);
                        };
                        btnClose4.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            txtWeight4.AbortEditing();
                            CommonClass.WeightsController.RemoveAndReloadWeights(lblTitle4.StringValue, order);
                        };
                        btnHelp4.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo[lblTitle4.StringValue] - 1));
                        };

                    }
                    break;
                case "DOW":
                    {
                        btnHelpDOW.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo["Days of the Week"] - 1));
                        };

                        btnWorkOff.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (btnWorkOff.SelectedTag == 0)
                            {
                                wBIdStateContent.Weights.DOW.IsWork = true;
                                wBIdStateContent.Weights.DOW.IsOff = false;
                            }
                            else
                            {
                                wBIdStateContent.Weights.DOW.IsWork = false;
                                wBIdStateContent.Weights.DOW.IsOff = true;
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights("Days of the Week");
                        };
                        txtMon.Activated += txtDOWWtChanged;
                        txtTue.Activated += txtDOWWtChanged;
                        txtWed.Activated += txtDOWWtChanged;
                        txtThu.Activated += txtDOWWtChanged;
                        txtFri.Activated += txtDOWWtChanged;
                        txtSat.Activated += txtDOWWtChanged;
                        txtSun.Activated += txtDOWWtChanged;
                        btnCloseDOW.Activated += (object sender, EventArgs e) =>
                        {

                            txtMon.AbortEditing();
                            txtTue.AbortEditing();
                            txtWed.AbortEditing();
                            txtThu.AbortEditing();
                            txtFri.AbortEditing();
                            txtSat.AbortEditing();
                            txtSun.AbortEditing();

                            CommonClass.WeightsController.RemoveAndReloadWeights("Days of the Week", order);
                        };
                    }
                    break;
                case "DOM":
                    {
                        btnHelpDOM.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo["Days of the Month"] - 1));
                        };

                        btnWorkOffDOM.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (btnWorkOffDOM.SelectedTag == 0)
                            {
                                wBIdStateContent.Weights.SDO.isWork = true;
                            }
                            else
                            {
                                wBIdStateContent.Weights.SDO.isWork = false;
                            }
                            CommonClass.WeightsController.ApplyAndReloadWeights("Days of the Month");
                        };
                        lblDOM.RotateByAngle(-90);
                        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                        string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
                        lblDOM.StringValue = strMonthName + "\nWork/Off Days";
                        btnCloseDOM.Activated += (object sender, EventArgs e) =>
                        {
                            CommonClass.WeightsController.RemoveAndReloadWeights("Days of the Month", order);
                        };
                        vwDOM.ItemPrototype = new DOMWeightsItem();
                        vwDOM.Content = lstDaysOfMonth.Select(x => new NSString(x.Id.ToString())).ToArray();
                    }
                    break;
                case "OBLK":
                    {
                        btnHelpBLK.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo["Overnight Cities - Bulk"] - 1));
                        };

                        btnCloseBulk.Activated += (object sender, EventArgs e) =>
                        {
                            CommonClass.WeightsController.RemoveAndReloadWeights("Overnight Cities - Bulk", order);
                        };
                        vwBulk.ItemPrototype = new BulkWeightsItem();
                        vwBulk.Content = lstOvernightCities.Select(x => new NSString(x.Id.ToString())).ToArray();
                    }
                    break;
                case "CL":
                    {

                        btnHelpCL.Activated += (object sender, EventArgs e) =>
                        {
                            if (CommonClass.HelpController == null)
                            {
                                var help = new HelpWindowController();
                                CommonClass.HelpController = help;
                            }
                            CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                            CommonClass.HelpController.tblDocument.SelectRow(2, false);
                            CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(WeightsApplied.HelpPageNo["Commutable Lines - Manual"] - 1));
                        };

                        btnCloseCL.Activated += (object sender, EventArgs e) =>
                        {
                            dpMonThuCheckIn.AbortEditing();
                            dpFriCheckIn.AbortEditing();
                            dpSatCheckIn.AbortEditing();
                            dpSunCheckIn.AbortEditing();
                            dpMonThuToBase.AbortEditing();
                            dpFriToBase.AbortEditing();
                            dpSatToBase.AbortEditing();
                            dpSunToBase.AbortEditing();

                            CommonClass.WeightsController.RemoveAndReloadWeights("Commutable Lines - Manual", order);
                        };
                        btnCommuteWork.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (btnCommuteWork.State == NSCellStateValue.Off && btnCommuteHome.State == NSCellStateValue.Off)
                            {
                                btnCommuteHome.State = NSCellStateValue.On;
                                wBIdStateContent.Weights.CL.Type = 2;
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
                            wBIdStateContent.Constraints.CL.CommuteToWork = (btnCommuteWork.State == NSCellStateValue.On);
                            wBIdStateContent.Constraints.CL.CommuteToHome = (btnCommuteHome.State == NSCellStateValue.On);

                            EnableDisableTimeFields();
                            CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Manual");
                        };
                        btnCommuteHome.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
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
                            wBIdStateContent.Constraints.CL.CommuteToWork = (btnCommuteWork.State == NSCellStateValue.On);
                            wBIdStateContent.Constraints.CL.CommuteToHome = (btnCommuteHome.State == NSCellStateValue.On);
                            EnableDisableTimeFields();
                            CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Manual");
                        };
                        txtInDomWt.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (txtInDomWt.StringValue == string.Empty)
                                txtInDomWt.StringValue = "0";
                            wBIdStateContent.Weights.CL.InDomicile = decimal.Parse(txtInDomWt.StringValue);
                            CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Manual");
                        };
                        txtBothEndWt.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            if (txtBothEndWt.StringValue == string.Empty)
                                txtBothEndWt.StringValue = "0";
                            wBIdStateContent.Weights.CL.BothEnds = decimal.Parse(txtBothEndWt.StringValue);
                            CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Manual");
                        };
                        btnLoadDefaults.Activated += (object sender, EventArgs e) =>
                        {
                            wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                            WBidHelper.PushToUndoStack();
                            wBIdStateContent.Weights.CL.TimesList[0].Checkin = wbidintialState.Weights.CL.DefaultTimes[0].Checkin;
                            wBIdStateContent.Weights.CL.TimesList[1].Checkin = wbidintialState.Weights.CL.DefaultTimes[1].Checkin;
                            wBIdStateContent.Weights.CL.TimesList[2].Checkin = wbidintialState.Weights.CL.DefaultTimes[2].Checkin;
                            wBIdStateContent.Weights.CL.TimesList[3].Checkin = wbidintialState.Weights.CL.DefaultTimes[3].Checkin;
                            wBIdStateContent.Weights.CL.TimesList[0].BackToBase = wbidintialState.Weights.CL.DefaultTimes[0].BackToBase;
                            wBIdStateContent.Weights.CL.TimesList[1].BackToBase = wbidintialState.Weights.CL.DefaultTimes[1].BackToBase;
                            wBIdStateContent.Weights.CL.TimesList[2].BackToBase = wbidintialState.Weights.CL.DefaultTimes[2].BackToBase;
                            wBIdStateContent.Weights.CL.TimesList[3].BackToBase = wbidintialState.Weights.CL.DefaultTimes[3].BackToBase;
                            CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Manual");


                            dpMonThuCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].Checkin)).DateTimeToNSDate();
                            dpFriCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].Checkin)).DateTimeToNSDate();
                            dpSatCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].Checkin)).DateTimeToNSDate();
                            dpSunCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].Checkin)).DateTimeToNSDate();

                            dpMonThuToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].BackToBase)).DateTimeToNSDate();
                            dpFriToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].BackToBase)).DateTimeToNSDate();
                            dpSatToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].BackToBase)).DateTimeToNSDate();
                            dpSunToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].BackToBase)).DateTimeToNSDate();

                            wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());
                            wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());
                            wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());
                            wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());
                            wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());
                            wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());
                            wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());
                            wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());

                            bool enableDefBtns = checkChangesInDefaultsValue();
                            btnLoadDefaults.Enabled = enableDefBtns;
                            btnSaveDefaults.Enabled = enableDefBtns;
                            CommonClass.WeightsController.tblWeights.ReloadData();
                        };
                        btnSaveDefaults.Activated += (object sender, EventArgs e) =>
                        {
                            WBidHelper.PushToUndoStack();
                            wbidintialState.Weights.CL.DefaultTimes[0].Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());
                            wbidintialState.Weights.CL.DefaultTimes[1].Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());
                            wbidintialState.Weights.CL.DefaultTimes[2].Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());
                            wbidintialState.Weights.CL.DefaultTimes[3].Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());
                            wbidintialState.Weights.CL.DefaultTimes[0].BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());
                            wbidintialState.Weights.CL.DefaultTimes[1].BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());
                            wbidintialState.Weights.CL.DefaultTimes[2].BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());
                            wbidintialState.Weights.CL.DefaultTimes[3].BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());


                            XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
                            wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

                            bool enableDefBtns = checkChangesInDefaultsValue();
                            btnLoadDefaults.Enabled = enableDefBtns;
                            btnSaveDefaults.Enabled = enableDefBtns;
                        };

                        dpMonThuCheckIn.Locale = new NSLocale("NL");
                        dpFriCheckIn.Locale = new NSLocale("NL");
                        dpSatCheckIn.Locale = new NSLocale("NL");
                        dpSunCheckIn.Locale = new NSLocale("NL");
                        dpMonThuToBase.Locale = new NSLocale("NL");
                        dpFriToBase.Locale = new NSLocale("NL");
                        dpSatToBase.Locale = new NSLocale("NL");
                        dpSunToBase.Locale = new NSLocale("NL");

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
        void ShowMessage(string text)
        {
            var alert = new NSAlert();
            alert.AlertStyle = NSAlertStyle.Warning;
            alert.MessageText = "Information";
            alert.InformativeText = text;
            alert.RunModal();
        }
        void ButtonLayout(NSButton button, bool state)
        {
            button.WantsLayer = true;//NSColor.FromRgba(124/256,206/256,38/256,1).CGColor;
            if (state) button.Layer.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1).CGColor;
            else button.Layer.BackgroundColor = NSColor.Orange.CGColor;//NSColor.FromCalibratedRgba(.91f, .51f, .21f, 1).CGColor;

            button.Layer.CornerRadius = (nfloat)2;
            button.Layer.BorderColor = NSColor.DarkGray.CGColor;
            button.Layer.BorderWidth = (nfloat)0.5;
            button.NeedsLayout = true;

        }
        void TimeValuesChanged(object sender, EventArgs e)
        {
            WBidHelper.PushToUndoStack();
            wBIdStateContent.Weights.CL.TimesList[0].Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[1].Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[2].Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[3].Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[0].BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[1].BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[2].BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[3].BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());
            CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Manual");

            wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());

            bool enableDefBtns = checkChangesInDefaultsValue();
            btnLoadDefaults.Enabled = enableDefBtns;
            btnSaveDefaults.Enabled = enableDefBtns;
        }

        void txtDOWWtChanged(object sender, EventArgs e)
        {
            var txt = (NSTextField)sender;
            if (txt.StringValue == string.Empty)
                txt.StringValue = "0";

            List<Wt> lstWeight = wBIdStateContent.Weights.DOW.lstWeight;
            if (lstWeight.Any(x => x.Key == txt.Tag))
            {
                if (txt.StringValue != "0")
                    lstWeight.FirstOrDefault(x => x.Key == txt.Tag).Value = Decimal.Parse(txt.StringValue);
                else
                    lstWeight.RemoveAll(x => x.Key == txt.Tag);
            }
            else
            {
                if (txt.StringValue != "0")
                    lstWeight.Add(new Wt { Key = (int)txt.Tag, Value = Decimal.Parse(txt.StringValue) });
            }
            CommonClass.WeightsController.ApplyAndReloadWeights("Days of the Week");
        }

        void txtCLOWtChanged(object sender, EventArgs e)
        {
            var txt = (NSTextField)sender;
            if (txt.StringValue == string.Empty)
                txt.StringValue = "0";
            decimal Values;
            bool isDecimal = decimal.TryParse(txt.StringValue, out Values);
            if (isDecimal)
            {
                wBIdStateContent.Weights.CLAuto.Weight = Values;
                CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Auto");
            }
        }

        void txtCmtWtChanged(object sender, EventArgs e)
        {
            var txt = (NSTextField)sender;
            if (txt.StringValue == string.Empty)
                txt.StringValue = "0";
            decimal Values;
            bool isDecimal = decimal.TryParse(txt.StringValue, out Values);
            if (isDecimal)
            {
                wBIdStateContent.Weights.Commute.Weight = Values;
                CommonClass.WeightsController.ApplyAndReloadWeights("Commutability");
            }
        }

        public void BindData(string weight, int index)
        {
            var lst = CommonClass.WeightsController.appliedWeights;
            order = index - lst.IndexOf(lst.FirstOrDefault(x => x == weight));

            switch (this.Identifier)
            {

                case "Commutability":
                    {

                        NSButton btnTitle = (NSButton)ViewWithTag(206);
                        btnTitle.Title = "Comut%(" + wBIdStateContent.Constraints.Commute.City + ")";
                        btnTitle.Activated += (object sender, EventArgs e) =>
                        {

                            NSNotificationCenter.DefaultCenter.PostNotificationName("CmtbltyWeightLoadNotification", null);
                        };

                        NSButton btnClose = (NSButton)this.ViewWithTag(104);
                        btnClose.Activated += (object sender1, EventArgs e1) =>
                        {

                            CommonClass.WeightsController.RemoveAndReloadWeights("Commutability", order);

                        };


                        string NoMiddleOrOkMiddle = arrCommutabilityFirstCellValue[wBIdStateContent.Weights.Commute.SecondcellValue - 1];
                        string NoFronOrBack = arrCommutabilitySecondCellValue[wBIdStateContent.Weights.Commute.ThirdcellValue - 1];


                        string leethanOrGreaterThan = "";
                        switch (wBIdStateContent.Weights.Commute.Type)
                        {
                            case (int)ConstraintType.LessThan:
                                leethanOrGreaterThan = arrCommutabilityThirdCellValue[0];

                                break;
                            case (int)ConstraintType.MoreThan:
                                leethanOrGreaterThan = arrCommutabilityThirdCellValue[1];
                                break;

                        }



                        string Value = wBIdStateContent.Weights.Commute.Value.ToString();

                        cmtbltybtnValue1.RemoveAllItems();
                        cmtbltybtnValue1.AddItems(arrCommutabilityFirstCellValue);
                        cmtbltybtnValue1.SelectItem(NoMiddleOrOkMiddle);
                        cmtbltybtnValue1.Activated += (object sender, EventArgs e) =>
                        {
                            if (wBIdStateContent.CxWtState.Commute.Wt && wBIdStateContent.Weights.Commute != null)
                            {

                                wBIdStateContent.Weights.Commute.SecondcellValue = (Int32)cmtbltybtnValue1.IndexOfSelectedItem + 1;

                                CommonClass.WeightsController.ApplyAndReloadWeights("Commutability");
                            }
                        };




                        cmtbltybtnValue2.RemoveAllItems();
                        cmtbltybtnValue2.AddItems(arrCommutabilitySecondCellValue);
                        cmtbltybtnValue2.SelectItem(NoFronOrBack);

                        cmtbltybtnValue2.Activated += (object sender, EventArgs e) =>
                        {

                            if (wBIdStateContent.CxWtState.Commute.Wt && wBIdStateContent.Weights.Commute != null)
                            {

                                wBIdStateContent.Weights.Commute.ThirdcellValue = (Int32)cmtbltybtnValue2.IndexOfSelectedItem + 1;

                                CommonClass.WeightsController.ApplyAndReloadWeights("Commutability");
                            }
                        };

                        cmtbltybtnValue3.RemoveAllItems();
                        cmtbltybtnValue3.AddItems(arrCommutabilityThirdCellValue);
                        cmtbltybtnValue3.SelectItem(leethanOrGreaterThan);

                        cmtbltybtnValue3.Activated += (object sender, EventArgs e) =>
                        {
                            if (wBIdStateContent.CxWtState.Commute.Wt && wBIdStateContent.Weights.Commute != null)
                            {

                                switch (cmtbltybtnValue3.IndexOfSelectedItem)
                                {
                                    case 0:
                                        wBIdStateContent.Weights.Commute.Type = (int)ConstraintType.LessThan;

                                        break;
                                    case 1:
                                        wBIdStateContent.Weights.Commute.Type = (int)ConstraintType.MoreThan;
                                        break;

                                }

                                CommonClass.WeightsController.ApplyAndReloadWeights("Commutability");
                            }
                        };

                        cmtbltybtnValue4.RemoveAllItems();
                        string[] arrWeight = getComutabilityValues().ToArray();
                        cmtbltybtnValue4.AddItems(arrWeight);
                        cmtbltybtnValue4.SelectItem(Value + "%");

                        cmtbltybtnValue4.Activated += (object sender, EventArgs e) =>
                        {
                            if (wBIdStateContent.CxWtState.Commute.Wt && wBIdStateContent.Weights.Commute != null)
                            {

                                string WeightValue = arrWeight[(Int32)cmtbltybtnValue4.IndexOfSelectedItem];

                                wBIdStateContent.Weights.Commute.Value = Int32.Parse(Regex.Match(WeightValue, @"\d+").Value);

                                CommonClass.WeightsController.ApplyAndReloadWeights("Commutability");
                            }
                        };

                        NSTextField text = (NSTextField)this.ViewWithTag(107);
                        if (wBIdStateContent.CxWtState.Commute.Wt && wBIdStateContent.Weights.Commute.Weight != null)
                        {
                            text.StringValue = wBIdStateContent.Weights.Commute.Weight.ToString();
                        }
                        text.Activated += txtCmtWtChanged;

                        break;
                    }



                case "Commutable Lines - Auto":
                    {
                        NSButton btnNight = (NSButton)this.ViewWithTag(101);
                        if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.NoNights != null)
                        {

                            ButtonLayout(btnNight, wBIdStateContent.Weights.CLAuto.NoNights);
                        }
                        btnNight.Activated += (object sender, EventArgs e) =>
                        {
                            if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.NoNights != null)
                            {
                                wBIdStateContent.Weights.CLAuto.NoNights = !wBIdStateContent.Weights.CLAuto.NoNights;
                                ButtonLayout(btnNight, wBIdStateContent.Weights.CLAuto.NoNights);
                                CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Auto");
                            }
                        };
                        NSButton btnClose = (NSButton)this.ViewWithTag(104);
                        btnClose.Activated += (object sender1, EventArgs e1) =>
                        {

                            CommonClass.WeightsController.RemoveAndReloadWeights("Commutable Lines - Auto", order);

                        };

                        NSButton btnTowork = (NSButton)this.ViewWithTag(102);
                        if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null)
                        {
                            ButtonLayout(btnTowork, wBIdStateContent.Weights.CLAuto.ToWork);
                        }
                        btnTowork.Activated += (object sender2, EventArgs e2) =>
                        {
                            if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null)
                            {
                                wBIdStateContent.Weights.CLAuto.ToWork = !wBIdStateContent.Weights.CLAuto.ToWork;
                                ButtonLayout(btnTowork, wBIdStateContent.Weights.CLAuto.ToWork);
                                CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Auto");
                            }
                        };
                        NSButton btnToHome = (NSButton)this.ViewWithTag(103);
                        if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null)
                        {
                            ButtonLayout(btnToHome, wBIdStateContent.Weights.CLAuto.ToHome);
                        }
                        btnToHome.Activated += (object sender3, EventArgs e3) =>
                        {
                            if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null)
                            {
                                wBIdStateContent.Weights.CLAuto.ToHome = !wBIdStateContent.Weights.CLAuto.ToHome;
                                ButtonLayout(btnToHome, wBIdStateContent.Weights.CLAuto.ToHome);
                                CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Auto");
                            }

                        };
                        NSButton btnHeadername = (NSButton)this.ViewWithTag(106);
                        if (wBIdStateContent.CxWtState.CLAuto.Wt)
                        {
                            btnHeadername.Title = "Cmt Line" + " [" + wBIdStateContent.Weights.CLAuto.City + "]";
                        }
                        else
                            btnHeadername.Title = "Cmt Line";
                        btnHeadername.Activated += (object sender3, EventArgs e3) =>
                        {

                            NSNotificationCenter.DefaultCenter.PostNotificationName("CLOLoadNotification1", null);

                        };

                        NSTextField text = (NSTextField)this.ViewWithTag(107);
                        if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null)
                        {
                            text.StringValue = wBIdStateContent.Weights.CLAuto.Weight.ToString();
                        }
                        text.Activated += txtCLOWtChanged;


                    }
                    break;
                //			case "Commutable Lines - Auto":
                //				{
                //					NSButton btnNight = (NSButton)this.ViewWithTag (101);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.NoNights != null) {
                //						
                //						ButtonLayout(btnNight,wBIdStateContent.Weights.CLAuto.NoNights);
                //					}
                //					btnNight.Activated += (object sender, EventArgs e) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.NoNights!=null)
                //						{
                //							wBIdStateContent.Weights.CLAuto.NoNights=!wBIdStateContent.Weights.CLAuto.NoNights;
                //							ButtonLayout(btnNight,wBIdStateContent.Weights.CLAuto.NoNights);
                //							CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //					};
                //					NSButton btnClose = (NSButton)this.ViewWithTag (104);
                //					btnClose.Activated += (object sender1, EventArgs e1) => {
                //
                //						CommonClass.WeightsController.RemoveAndReloadWeights ("Commutable Lines - Auto", order);
                //
                //					};
                //
                //					NSButton btnTowork = (NSButton)this.ViewWithTag (102);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null) {
                //						ButtonLayout(btnTowork,wBIdStateContent.Weights.CLAuto.ToWork);
                //					}
                //					btnTowork.Activated += (object sender2, EventArgs e2) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight!=null)
                //						{
                //							wBIdStateContent.Weights.CLAuto.ToWork=!wBIdStateContent.Weights.CLAuto.ToWork;
                //							ButtonLayout(btnTowork,wBIdStateContent.Weights.CLAuto.ToWork);
                //							CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //					};
                //					NSButton btnToHome = (NSButton)this.ViewWithTag (103);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight != null) 
                //					{
                //						ButtonLayout(btnToHome,wBIdStateContent.Weights.CLAuto.ToHome);
                //					}
                //					btnToHome.Activated += (object sender3, EventArgs e3) => {
                //						if(wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight!=null)
                //						{
                //							wBIdStateContent.Weights.CLAuto.ToHome=!wBIdStateContent.Weights.CLAuto.ToHome;
                //							ButtonLayout(btnToHome,wBIdStateContent.Weights.CLAuto.ToHome);
                //							CommonClass.WeightsController.ApplyAndReloadWeights ("Commutable Lines - Auto");
                //						}
                //
                //					};
                //					NSButton btnHeadername = (NSButton)this.ViewWithTag (106);
                //					if(wBIdStateContent.CxWtState.CLAuto.Wt)
                //					{
                //						btnHeadername.Title = "Cmt Line" + " [" + wBIdStateContent.Weights.CLAuto.City + "]";
                //					}  else
                //						btnHeadername.Title = "Cmt Line";
                //					btnHeadername.Activated += (object sender3, EventArgs e3) => {
                //
                //						NSNotificationCenter.DefaultCenter.PostNotificationName ("CLOLoadNotification", null);
                //
                //					} ;
                //
                //					NSTextField text = (NSTextField)this.ViewWithTag (107);
                //					if (wBIdStateContent.CxWtState.CLAuto.Wt && wBIdStateContent.Weights.CLAuto.Weight!=null) {
                //						text.StringValue = wBIdStateContent.Weights.CLAuto.Weight.ToString ();
                //					}
                //					text.Activated += txtCLOWtChanged;
                //				
                //
                //				}
                //				break;
                case "View1":
                    {
                        lblTitle1.StringValue = weight;
                        if (this.lblTitle1.StringValue == "Largest Block of Days Off")
                        {
                            decimal wgt = wBIdStateContent.Weights.LrgBlkDayOff.Weight;
                            txtWeight1.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle1.StringValue == "Normalize Days Off")
                        {
                            decimal wgt = wBIdStateContent.Weights.NormalizeDaysOff.Weight;
                            txtWeight1.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle1.StringValue == "ETOPS")
                        {
                            decimal wgt = wBIdStateContent.Weights.ETOPS.lstParameters[order].Weight;
                            txtWeight1.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle1.StringValue == "ETOPS-Res")
                        {
                            decimal wgt = wBIdStateContent.Weights.ETOPSRes.lstParameters[order].Weight;
                            txtWeight1.StringValue = wgt.ToString();
                        }
                    }
                    break;
                case "View2":
                    {
                        lblTitle2.StringValue = weight;
                        if (this.lblTitle2.StringValue == "AM/PM")
                        {
                            int third = wBIdStateContent.Weights.AM_PM.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.AM_PM.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrThirdParam1);
                            btnThird2.Items()[0].Tag = (int)AMPMType.AM;
                            btnThird2.Items()[1].Tag = (int)AMPMType.PM;
                            btnThird2.Items()[2].Tag = (int)AMPMType.NTE;
                            btnThird2.SelectItemWithTag(third);

                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "DH - first - last")
                        {
                            int third = wBIdStateContent.Weights.DHDFoL.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.DHDFoL.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrThirdParam2);
                            btnThird2.Items()[0].Tag = (int)DeadheadType.First;
                            btnThird2.Items()[1].Tag = (int)DeadheadType.Last;
                            btnThird2.SelectItemWithTag(third);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Intl – NonConus")
                        {
                            int third = wBIdStateContent.Weights.InterConus.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.InterConus.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            var cities = intlNonConusCities.Select(x => x.City).ToList();
                            var city = intlNonConusCities.FirstOrDefault(x => x.CityId == third).City;
                            btnThird2.AddItems(cities.ToArray());
                            btnThird2.SelectItem(city);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Number of Days Off")
                        {
                            int third = wBIdStateContent.Weights.NODO.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.NODO.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrNODOThirdValue.ConvertAll(x => x.ToString() + " off").ToArray());
                            btnThird2.SelectItem(third.ToString() + " off");
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Overnight Cities")
                        {
                            int third = wBIdStateContent.Weights.RON.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.RON.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            //var cities = GlobalSettings.OverNightCitiesInBid.Select (x => x.Name).ToList ();
                            var cities = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                            //var city = GlobalSettings.OverNightCitiesInBid.FirstOrDefault (x => x.Id == third).Name;
                            var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
                            btnThird2.AddItems(cities.ToArray());
                            btnThird2.SelectItem(city);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Cities-Legs")
                        {
                            int third = wBIdStateContent.Weights.CitiesLegs.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.CitiesLegs.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            var cities = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                            var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
                            btnThird2.AddItems(cities.ToArray());
                            btnThird2.SelectItem(city);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Position")
                        {
                            int third = wBIdStateContent.Weights.POS.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.POS.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrThirdParam3);
                            btnThird2.Items()[0].Tag = (int)FAPositon.A;
                            btnThird2.Items()[1].Tag = (int)FAPositon.B;
                            btnThird2.Items()[2].Tag = (int)FAPositon.C;
                            btnThird2.Items()[3].Tag = (int)FAPositon.D;
                            btnThird2.SelectItemWithTag(third);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Start Day of Week")
                        {
                            int third = wBIdStateContent.Weights.SDOW.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.SDOW.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrDays);
                            int i = 0;
                            foreach (var item in btnThird2.Items())
                            {
                                item.Tag = i;
                                i++;
                            }
                            btnThird2.SelectItemWithTag(third);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Time-Away-From-Base")
                        {
                            int third = wBIdStateContent.Weights.PerDiem.Type;
                            decimal wgt = wBIdStateContent.Weights.PerDiem.Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrTimeAwayThirdValue.ConvertAll(x => x.ToString()).ToArray());
                            btnThird2.SelectItem(third.ToString());
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Trip Length")
                        {
                            int third = wBIdStateContent.Weights.TL.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.TL.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrThirdParam4);
                            int i = 1;
                            foreach (var item in btnThird2.Items())
                            {
                                item.Tag = i;
                                i++;
                            }
                            btnThird2.SelectItemWithTag(third);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle2.StringValue == "Work Blk Length")
                        {
                            int third = wBIdStateContent.Weights.WB.lstParameters[order].Type;
                            decimal wgt = wBIdStateContent.Weights.WB.lstParameters[order].Weight;
                            btnThird2.RemoveAllItems();
                            btnThird2.AddItems(arrThirdParam4);
                            int i = 1;
                            foreach (var item in btnThird2.Items())
                            {
                                item.Tag = i;
                                i++;
                            }
                            btnThird2.SelectItemWithTag(third);
                            txtWeight2.StringValue = wgt.ToString();
                        }
                    }
                    break;
                case "View3":
                    {
                        lblTitle3.StringValue = weight;
                        if (this.lblTitle3.StringValue == "Aircraft Changes")
                        {
                            int second = wBIdStateContent.Weights.AirCraftChanges.SecondlValue;
                            int third = wBIdStateContent.Weights.AirCraftChanges.ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.AirCraftChanges.Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrSecondParam1);
                            btnSecond3.Items()[0].Tag = (int)WeightType.Less;
                            btnSecond3.Items()[1].Tag = (int)WeightType.More;
                            btnSecond3.Items()[2].Tag = (int)WeightType.Equal;
                            btnSecond3.Items()[3].Tag = (int)WeightType.NotEqual;
                            btnSecond3.SelectItemWithTag(second);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrAirCraftThirdValue.ConvertAll(x => x.ToString() + " chg").ToArray());
                            btnThird3.SelectItem(third.ToString() + " chg");
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Blocks of Days Off")
                        {
                            int second = wBIdStateContent.Weights.BDO.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.BDO.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.BDO.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrSecondParam1);
                            btnSecond3.Items()[0].Tag = (int)WeightType.Less;
                            btnSecond3.Items()[1].Tag = (int)WeightType.More;
                            btnSecond3.Items()[2].Tag = (int)WeightType.Equal;
                            btnSecond3.Items()[3].Tag = (int)WeightType.NotEqual;
                            btnSecond3.SelectItemWithTag(second);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrBDOThirdValue.ConvertAll(x => x.ToString() + " blk").ToArray());
                            btnThird3.SelectItem(third.ToString() + " blk");
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Cmut DHs")
                        {
                            int second = wBIdStateContent.Weights.DHD.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.DHD.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.DHD.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            var cities = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                            btnSecond3.AddItems(cities.ToArray());
                            var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == second).Name;
                            btnSecond3.SelectItem(city);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrCmut);
                            btnThird3.Items()[0].Tag = (int)DeadheadType.First;
                            btnThird3.Items()[1].Tag = (int)DeadheadType.Last;
                            btnThird3.Items()[2].Tag = (int)DeadheadType.Both;
                            btnThird3.SelectItemWithTag(third);
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Duty period")
                        {
                            int second = wBIdStateContent.Weights.DP.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.DP.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.DP.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrSecondParam2);
                            btnSecond3.Items()[0].Tag = (int)DutyPeriodType.Relative;
                            btnSecond3.Items()[1].Tag = (int)DutyPeriodType.Longer;
                            btnSecond3.Items()[2].Tag = (int)DutyPeriodType.Shorter;
                            btnSecond3.SelectItemWithTag(second);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(lstDPTimes.ToArray());
                            btnThird3.SelectItem(Helper.ConvertMinuteToHHMM(third));
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Equipment Type")
                        {
                            if (wBIdStateContent.Weights.EQUIP.lstParameters[order].SecondlValue == 500)
                                wBIdStateContent.Weights.EQUIP.lstParameters[order].SecondlValue = 300;
                            int second = wBIdStateContent.Weights.EQUIP.lstParameters[order].SecondlValue;
                            string secondcell = "";
                            if (second == 600)
                                secondcell = "8Max";
                            else if (second == 200)
                                secondcell = "7Max";
                            else
                                secondcell = second.ToString();
                            int third = wBIdStateContent.Weights.EQUIP.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.EQUIP.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrEQType);
                            btnSecond3.SelectItem(secondcell);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrBDOThirdValue.ConvertAll(x => x.ToString() + " legs").ToArray());
                            btnThird3.SelectItem(third.ToString() + " legs");
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Flight Time")
                        {
                            int second = wBIdStateContent.Weights.FLTMIN.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.FLTMIN.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.FLTMIN.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrSecondParam3);
                            btnSecond3.Items()[0].Tag = (int)WeightType.Less;
                            btnSecond3.Items()[1].Tag = (int)WeightType.More;
                            btnSecond3.SelectItemWithTag(second);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrFltTimeValue.ConvertAll(x => x.ToString()).ToArray());
                            btnThird3.SelectItem(third.ToString());
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Ground Time")
                        {
                            int second = wBIdStateContent.Weights.GRD.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.GRD.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.GRD.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(lstGrdTimes.ToArray());
                            btnSecond3.SelectItem(Helper.ConvertMinuteToHHMM(second));
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrGrdValue.ConvertAll(x => x.ToString() + " occurs").ToArray());
                            btnThird3.SelectItem(third.ToString() + " occurs");
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Legs Per Duty Period")
                        {
                            int second = wBIdStateContent.Weights.LEGS.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.LEGS.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.LEGS.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrSecondParam7);
                            btnSecond3.Items()[0].Tag = (int)WeightType.Less;
                            btnSecond3.Items()[1].Tag = (int)WeightType.More;
                            btnSecond3.Items()[2].Tag = (int)WeightType.Equal;
                            btnSecond3.SelectItemWithTag(second);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrLegsDPValue.ConvertAll(x => x.ToString() + " legs").ToArray());
                            btnThird3.SelectItem(third.ToString() + " legs");
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Legs Per Pairing")
                        {
                            int second = wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.WtLegsPerPairing.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrSecondParam4);
                            btnSecond3.Items()[0].Tag = (int)LegsPerPairingType.All;
                            btnSecond3.Items()[1].Tag = (int)LegsPerPairingType.More;
                            btnSecond3.Items()[2].Tag = (int)LegsPerPairingType.Less;
                            btnSecond3.SelectItemWithTag(second);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrAirCraftThirdValue.ConvertAll(x => x.ToString() + " legs").ToArray());
                            btnThird3.SelectItem(third.ToString() + " legs");
                            txtWeight3.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle3.StringValue == "Work Days")
                        {
                            int second = wBIdStateContent.Weights.WorkDays.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.WorkDays.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.WorkDays.lstParameters[order].Weight;
                            btnSecond3.RemoveAllItems();
                            btnSecond3.AddItems(arrSecondParam7);
                            btnSecond3.Items()[0].Tag = (int)WeightType.Less;
                            btnSecond3.Items()[1].Tag = (int)WeightType.More;
                            btnSecond3.Items()[2].Tag = (int)WeightType.Equal;
                            btnSecond3.SelectItemWithTag(second);
                            btnThird3.RemoveAllItems();
                            btnThird3.AddItems(arrWorkDaysThirdValue.ConvertAll(x => x.ToString() + " wk days").ToArray());
                            btnThird3.SelectItem(third.ToString() + " wk days");
                            txtWeight3.StringValue = wgt.ToString();
                        }
                    }
                    break;
                case "View4":
                    {
                        lblTitle4.StringValue = weight;
                        if (this.lblTitle4.StringValue == "PDO-after")
                        {
                            int first = wBIdStateContent.Weights.PDAfter.lstParameters[order].FirstValue;
                            int second = wBIdStateContent.Weights.PDAfter.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.PDAfter.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.PDAfter.lstParameters[order].Weight;
                            btnFirst4.RemoveAllItems();
                            //btnFirst4.AddItems (lstPDODays.ConvertAll (x => x.Date.ToString ("dd - MMM")).ToArray ());
                            btnFirst4.AddItems(newPdoDateStringList.ToArray());
                            if (first == 300)
                            {
                                btnFirst4.SelectItem("Any Date");
                            }
                            else
                            {
                                var date = lstPDODays.FirstOrDefault(x => x.DateId == first).Date;
                                btnFirst4.SelectItem(date.ToString("dd - MMM"));
                            }
                            btnSecond4.RemoveAllItems();
                            btnSecond4.AddItems(lstPDOTimeSecValue.ToArray());
                            btnSecond4.SelectItem(lstPDOTimeSecValue.IndexOf(Helper.ConvertMinuteToHHMM(second)));
                            btnThird4.RemoveAllItems();

                            var lstname = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                            lstname.Insert(0, "Any City");
                            btnThird4.AddItems(lstname.ToArray());
                            if (third == 400)
                            {
                                btnThird4.SelectItem("Any City");
                            }
                            else
                            {
                                var name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
                                btnThird4.SelectItem(name);
                            }
                            txtWeight4.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle4.StringValue == "PDO-before")
                        {
                            int first = wBIdStateContent.Weights.PDBefore.lstParameters[order].FirstValue;
                            int second = wBIdStateContent.Weights.PDBefore.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.PDBefore.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.PDBefore.lstParameters[order].Weight;
                            btnFirst4.RemoveAllItems();
                            //btnFirst4.AddItems (lstPDODays.ConvertAll (x => x.Date.ToString ("dd - MMM")).ToArray ());
                            btnFirst4.AddItems(newPdoDateStringList.ToArray());
                            if (first == 300)
                            {
                                btnFirst4.SelectItem("Any Date");
                            }
                            else
                            {
                                var date = lstPDODays.FirstOrDefault(x => x.DateId == first).Date;
                                btnFirst4.SelectItem(date.ToString("dd - MMM"));
                            }
                            btnSecond4.RemoveAllItems();
                            btnSecond4.AddItems(lstPDOTimeSecValue.ToArray());
                            btnSecond4.SelectItem(lstPDOTimeSecValue.IndexOf(Helper.ConvertMinuteToHHMM(second)));
                            btnThird4.RemoveAllItems();
                            var lstname = GlobalSettings.WBidINIContent.Cities.Select(x => x.Name).ToList();
                            lstname.Insert(0, "Any City");
                            btnThird4.AddItems(lstname.ToArray());
                            if (third == 400)
                            {
                                btnThird4.SelectItem("Any City");
                            }
                            else
                            {
                                var name = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == third).Name;
                                btnThird4.SelectItem(name);
                            }
                            txtWeight4.StringValue = wgt.ToString();
                        }
                        else if (this.lblTitle4.StringValue == "Rest")
                        {
                            int first = wBIdStateContent.Weights.WtRest.lstParameters[order].FirstValue;
                            int second = wBIdStateContent.Weights.WtRest.lstParameters[order].SecondlValue;
                            int third = wBIdStateContent.Weights.WtRest.lstParameters[order].ThrirdCellValue;
                            decimal wgt = wBIdStateContent.Weights.WtRest.lstParameters[order].Weight;
                            btnFirst4.RemoveAllItems();
                            btnFirst4.AddItems(arrSecondParam5);
                            btnFirst4.Items()[0].Tag = (int)RestOptions.Shorter;
                            btnFirst4.Items()[1].Tag = (int)RestOptions.Both;
                            btnFirst4.Items()[2].Tag = (int)RestOptions.Longer;
                            btnFirst4.SelectItemWithTag(first);
                            btnSecond4.RemoveAllItems();
                            btnSecond4.AddItems(lstRestTimeSecValue.ToArray());
                            btnSecond4.SelectItem(lstRestTimeSecValue.IndexOf(Helper.ConvertMinuteToHHMM(second)));
                            btnThird4.RemoveAllItems();
                            btnThird4.AddItems(arrSecondParam6);
                            btnThird4.Items()[0].Tag = (int)RestType.All;
                            btnThird4.Items()[1].Tag = (int)RestType.AwayDomicile;
                            btnThird4.Items()[2].Tag = (int)RestType.InDomicile;
                            btnThird4.SelectItemWithTag(second);
                            txtWeight4.StringValue = wgt.ToString();
                        }
                    }
                    break;
                case "DOW":
                    {
                        if (wBIdStateContent.Weights.DOW.IsOff)
                            btnWorkOff.SelectCellWithTag(1);
                        else
                            btnWorkOff.SelectCellWithTag(0);

                        List<Wt> lstWeight = wBIdStateContent.Weights.DOW.lstWeight;
                        if (lstWeight.Any(x => x.Key == 0))
                            txtMon.StringValue = lstWeight.FirstOrDefault(x => x.Key == 0).Value.ToString();
                        else
                            txtMon.StringValue = "0";
                        if (lstWeight.Any(x => x.Key == 1))
                            txtTue.StringValue = lstWeight.FirstOrDefault(x => x.Key == 1).Value.ToString();
                        else
                            txtTue.StringValue = "0";
                        if (lstWeight.Any(x => x.Key == 2))
                            txtWed.StringValue = lstWeight.FirstOrDefault(x => x.Key == 2).Value.ToString();
                        else
                            txtWed.StringValue = "0";
                        if (lstWeight.Any(x => x.Key == 3))
                            txtThu.StringValue = lstWeight.FirstOrDefault(x => x.Key == 3).Value.ToString();
                        else
                            txtThu.StringValue = "0";
                        if (lstWeight.Any(x => x.Key == 4))
                            txtFri.StringValue = lstWeight.FirstOrDefault(x => x.Key == 4).Value.ToString();
                        else
                            txtFri.StringValue = "0";
                        if (lstWeight.Any(x => x.Key == 5))
                            txtSat.StringValue = lstWeight.FirstOrDefault(x => x.Key == 5).Value.ToString();
                        else
                            txtSat.StringValue = "0";
                        if (lstWeight.Any(x => x.Key == 6))
                            txtSun.StringValue = lstWeight.FirstOrDefault(x => x.Key == 6).Value.ToString();
                        else
                            txtSun.StringValue = "0";

                        if (txtMon.IntValue < 0)
                            txtMon.BackgroundColor = NSColor.Red;
                        else if (txtMon.IntValue > 0)
                            txtMon.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                        else
                            txtMon.BackgroundColor = NSColor.White;

                        if (txtTue.IntValue < 0)
                            txtTue.BackgroundColor = NSColor.Red;
                        else if (txtTue.IntValue > 0)
                            txtTue.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                        else
                            txtTue.BackgroundColor = NSColor.White;

                        if (txtWed.IntValue < 0)
                            txtWed.BackgroundColor = NSColor.Red;
                        else if (txtWed.IntValue > 0)
                            txtWed.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                        else
                            txtWed.BackgroundColor = NSColor.White;

                        if (txtThu.IntValue < 0)
                            txtThu.BackgroundColor = NSColor.Red;
                        else if (txtThu.IntValue > 0)
                            txtThu.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                        else
                            txtThu.BackgroundColor = NSColor.White;

                        if (txtFri.IntValue < 0)
                            txtFri.BackgroundColor = NSColor.Red;
                        else if (txtFri.IntValue > 0)
                            txtFri.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                        else
                            txtFri.BackgroundColor = NSColor.White;

                        if (txtSat.IntValue < 0)
                            txtSat.BackgroundColor = NSColor.Red;
                        else if (txtSat.IntValue > 0)
                            txtSat.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                        else
                            txtSat.BackgroundColor = NSColor.White;

                        if (txtSun.IntValue < 0)
                            txtSun.BackgroundColor = NSColor.Red;
                        else if (txtSun.IntValue > 0)
                            txtSun.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                        else
                            txtSun.BackgroundColor = NSColor.White;

                    }
                    break;
                case "DOM":
                    {
                        if (wBIdStateContent.Weights.SDO.isWork)
                            btnWorkOffDOM.SelectCellWithTag(0);
                        else
                            btnWorkOffDOM.SelectCellWithTag(1);

                        vwDOM.ItemPrototype = new DOMWeightsItem();
                    }
                    break;
                case "OBLK":
                    {
                        vwBulk.ItemPrototype = new BulkWeightsItem();
                    }
                    break;
                case "CL":
                    {
                        if (wBIdStateContent.Weights.CL.Type == 1)
                        {
                            btnCommuteWork.State = NSCellStateValue.On;
                            btnCommuteHome.State = NSCellStateValue.On;
                        }
                        else if (wBIdStateContent.Weights.CL.Type == 2)
                        {
                            btnCommuteWork.State = NSCellStateValue.Off;
                            btnCommuteHome.State = NSCellStateValue.On;
                        }
                        else if (wBIdStateContent.Weights.CL.Type == 3)
                        {
                            btnCommuteWork.State = NSCellStateValue.On;
                            btnCommuteHome.State = NSCellStateValue.Off;
                        }

                        EnableDisableTimeFields();

                        dpMonThuCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].Checkin)).DateTimeToNSDate();
                        dpFriCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].Checkin)).DateTimeToNSDate();
                        dpSatCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].Checkin)).DateTimeToNSDate();
                        dpSunCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].Checkin)).DateTimeToNSDate();

                        dpMonThuToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[0].BackToBase)).DateTimeToNSDate();
                        dpFriToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[1].BackToBase)).DateTimeToNSDate();
                        dpSatToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[2].BackToBase)).DateTimeToNSDate();
                        dpSunToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Weights.CL.TimesList[3].BackToBase)).DateTimeToNSDate();

                        bool enableDefBtns = checkChangesInDefaultsValue();
                        btnLoadDefaults.Enabled = enableDefBtns;
                        btnSaveDefaults.Enabled = enableDefBtns;

                        txtInDomWt.StringValue = wBIdStateContent.Weights.CL.InDomicile.ToString();
                        txtBothEndWt.StringValue = wBIdStateContent.Weights.CL.BothEnds.ToString();
                    }
                    break;
            }
        }
        void BtnClose_Activated(object sender, EventArgs e)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName("CmtbltyLoadNotification", null);
        }

        void EnableDisableTimeFields()
        {
            dpMonThuCheckIn.Enabled = false;
            dpFriCheckIn.Enabled = false;
            dpSatCheckIn.Enabled = false;
            dpSunCheckIn.Enabled = false;
            dpMonThuToBase.Enabled = false;
            dpFriToBase.Enabled = false;
            dpSatToBase.Enabled = false;
            dpSunToBase.Enabled = false;
            txtInDomWt.Enabled = false;
            txtBothEndWt.Enabled = false;
            if (btnCommuteWork.State == NSCellStateValue.On)
            {
                dpMonThuCheckIn.Enabled = true;
                dpFriCheckIn.Enabled = true;
                dpSatCheckIn.Enabled = true;
                dpSunCheckIn.Enabled = true;
                txtInDomWt.Enabled = true;
            }
            if (btnCommuteHome.State == NSCellStateValue.On)
            {
                dpMonThuToBase.Enabled = true;
                dpFriToBase.Enabled = true;
                dpSatToBase.Enabled = true;
                dpSunToBase.Enabled = true;
                txtBothEndWt.Enabled = true;
            }
        }

        /// <summary>
        /// retuurn true if any value chnages from the dafault state.
        /// </summary>
        /// <returns></returns>
        private bool checkChangesInDefaultsValue()
        {
            if ((wbidintialState.Weights.CL.DefaultTimes[0].Checkin != Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue())) || (wbidintialState.Weights.CL.DefaultTimes[0].BackToBase != Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue())))
            {
                return true;
            }
            if ((wbidintialState.Weights.CL.DefaultTimes[1].Checkin != Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue())) || (wbidintialState.Weights.CL.DefaultTimes[1].BackToBase != Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue())))
            {
                return true;
            }
            if ((wbidintialState.Weights.CL.DefaultTimes[2].Checkin != Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue())) || (wbidintialState.Weights.CL.DefaultTimes[2].BackToBase != Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue())))
            {
                return true;
            }
            if ((wbidintialState.Weights.CL.DefaultTimes[3].Checkin != Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue())) || (wbidintialState.Weights.CL.DefaultTimes[3].BackToBase != Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue())))
            {
                return true;
            }

            return false;

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
    public class DOMWeightsItem : NSCollectionViewItem
    {
        private static readonly NSString EMPTY_NSSTRING = new NSString(string.Empty);
        private DOMWtCell view;

        public DOMWeightsItem() : base()
        {

        }

        public DOMWeightsItem(IntPtr ptr) : base(ptr)
        {

        }

        public override void LoadView()
        {
            view = new DOMWtCell();
            View = view;
        }

        public override NSObject RepresentedObject
        {
            get { return base.RepresentedObject; }

            set
            {
                if (value == null)
                {
                    // Need to do this because setting RepresentedObject in base to null 
                    // throws an exception because of the MonoMac generated wrappers,
                    // and even though we don't have any null values, this will get 
                    // called during initialization of the content with a null value.
                    base.RepresentedObject = EMPTY_NSSTRING;
                    view.TxtWt.StringValue = string.Empty;
                }
                else
                {
                    base.RepresentedObject = value;
                    var item = WeightsCell.lstDaysOfMonth.FirstOrDefault(x => x.Id.ToString() == value.ToString());
                    if (item.Day == null)
                        view.TxtWt.Hidden = true;
                    else
                        view.TxtWt.Hidden = false;

                    view.TxtWt.Tag = item.Id;
                    view.TxtDay.StringValue = item.Day.ToString();
                    if (WeightsCell.lstDOMWeights.Any(x => x.Key == item.Id))
                        view.TxtWt.StringValue = WeightsCell.lstDOMWeights.FirstOrDefault(x => x.Key == item.Id).Value.ToString();
                    else
                        view.TxtWt.StringValue = "0";

                    if (view.TxtWt.IntValue < 0)
                        view.TxtWt.BackgroundColor = NSColor.Red;
                    else if (view.TxtWt.IntValue > 0)
                        view.TxtWt.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                    else
                        view.TxtWt.BackgroundColor = NSColor.White;

                    view.TxtWt.Activated += (object sender, EventArgs e) =>
                    {
                        WBidHelper.PushToUndoStack();
                        if (view.TxtWt.StringValue == string.Empty)
                            view.TxtWt.StringValue = "0";

                        if (WeightsCell.lstDOMWeights.Any(x => x.Key == view.TxtWt.Tag))
                        {
                            if (view.TxtWt.StringValue != "0")
                                WeightsCell.lstDOMWeights.FirstOrDefault(x => x.Key == view.TxtWt.Tag).Value = Decimal.Parse(view.TxtWt.StringValue);
                            else
                                WeightsCell.lstDOMWeights.RemoveAll(x => x.Key == view.TxtWt.Tag);
                        }
                        else
                        {
                            if (view.TxtWt.StringValue != "0")
                                WeightsCell.lstDOMWeights.Add(new Wt { Key = (int)view.TxtWt.Tag, Value = Decimal.Parse(view.TxtWt.StringValue) });
                        }
                        CommonClass.WeightsController.ApplyAndReloadWeights("Days of the Month");
                    };
                }
            }
        }
    }

    public class DOMWtCell : NSView
    {
        private NSTextField txtWt;
        private NSTextField txtDay;

        public DOMWtCell() : base(new CGRect(0, 0, 51, 40))
        {
            txtDay = new NSTextField(new CGRect(5, 20, 21, 21));
            txtDay.Bordered = false;
            txtDay.Editable = false;
            txtDay.BackgroundColor = NSColor.Clear;
            txtDay.TextColor = NSColor.Red;
            AddSubview(txtDay);
            txtWt = new NSTextField(new CGRect(3, 5, 44, 21));
            txtWt.Alignment = NSTextAlignment.Center;
            var format = new NSNumberFormatter();
            format.NumberStyle = NSNumberFormatterStyle.Decimal;
            format.Minimum = NSNumber.FromDouble(-999.99);
            format.Maximum = NSNumber.FromDouble(999.99);
            txtWt.Formatter = format;
            txtWt.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
            txtWt.Cell.SetSendsActionOnEndEditing(true);
            AddSubview(txtWt);
        }

        public NSTextField TxtWt
        {
            get { return txtWt; }
        }

        public NSTextField TxtDay
        {
            get { return txtDay; }
        }

    }
    #endregion

    #region BulkCollection
    public class BulkWeightsItem : NSCollectionViewItem
    {
        private static readonly NSString EMPTY_NSSTRING = new NSString(string.Empty);
        private BulkWtCell view;

        public BulkWeightsItem() : base()
        {

        }

        public BulkWeightsItem(IntPtr ptr) : base(ptr)
        {

        }

        public override void LoadView()
        {
            view = new BulkWtCell();
            View = view;
        }

        public override NSObject RepresentedObject
        {
            get { return base.RepresentedObject; }

            set
            {
                if (value == null)
                {
                    // Need to do this because setting RepresentedObject in base to null 
                    // throws an exception because of the MonoMac generated wrappers,
                    // and even though we don't have any null values, this will get 
                    // called during initialization of the content with a null value.
                    base.RepresentedObject = EMPTY_NSSTRING;
                    view.TxtWt.StringValue = string.Empty;
                }
                else
                {
                    base.RepresentedObject = value;
                    var item = WeightsCell.lstOvernightCities.FirstOrDefault(x => x.Id.ToString() == value.ToString());
                    view.TxtWt.Tag = item.Id;
                    view.TxtCity.StringValue = item.Name;

                    if (GlobalSettings.OverNightCitiesInBid.Any(x => x.Name == item.Name))
                    {
                        view.TxtCity.BackgroundColor = NSColor.White;
                        view.TxtCity.TextColor = NSColor.Black;
                    }
                    else
                    {
                        view.TxtCity.BackgroundColor = NSColor.Black;
                        view.TxtCity.TextColor = NSColor.White;
                    }

                    if (WeightsCell.lstBulkWeights.Any(x => x.Type == item.Id))
                        view.TxtWt.StringValue = WeightsCell.lstBulkWeights.FirstOrDefault(x => x.Type == item.Id).Weight.ToString();
                    else
                        view.TxtWt.StringValue = "0";

                    if (view.TxtWt.IntValue < 0)
                        view.TxtWt.BackgroundColor = NSColor.Red;
                    else if (view.TxtWt.IntValue > 0)
                        view.TxtWt.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);
                    else
                        view.TxtWt.BackgroundColor = NSColor.White;

                    view.TxtWt.Activated += (object sender, EventArgs e) =>
                    {
                        WBidHelper.PushToUndoStack();
                        if (view.TxtWt.StringValue == string.Empty)
                            view.TxtWt.StringValue = "0";

                        if (WeightsCell.lstBulkWeights.Any(x => x.Type == view.TxtWt.Tag))
                        {
                            if (view.TxtWt.StringValue != "0")
                                WeightsCell.lstBulkWeights.FirstOrDefault(x => x.Type == view.TxtWt.Tag).Weight = Decimal.Parse(view.TxtWt.StringValue);
                            else
                                WeightsCell.lstBulkWeights.RemoveAll(x => x.Type == view.TxtWt.Tag);
                        }
                        else
                        {
                            if (view.TxtWt.StringValue != "0")
                                WeightsCell.lstBulkWeights.Add(new Wt2Parameter { Type = (int)view.TxtWt.Tag, Weight = Decimal.Parse(view.TxtWt.StringValue) });
                        }
                        CommonClass.WeightsController.ApplyAndReloadWeights("Overnight Cities - Bulk");
                    };
                }
            }
        }
    }

    public class BulkWtCell : NSView
    {
        private NSTextField txtWt;
        private NSTextField txtCity;

        public BulkWtCell() : base(new CGRect(0, 0, 51, 40))
        {
            txtCity = new NSTextField(new CGRect(5, 20, 40, 21));
            txtCity.Bordered = false;
            txtCity.Editable = false;
            txtCity.BackgroundColor = NSColor.Clear;
            txtCity.Alignment = NSTextAlignment.Center;
            AddSubview(txtCity);
            txtWt = new NSTextField(new CGRect(3, 5, 44, 21));
            txtWt.Alignment = NSTextAlignment.Center;
            var format = new NSNumberFormatter();
            format.NumberStyle = NSNumberFormatterStyle.Decimal;
            format.Minimum = NSNumber.FromDouble(-999.99);
            format.Maximum = NSNumber.FromDouble(999.99);
            txtWt.Formatter = format;
            txtWt.Font = NSFont.SystemFontOfSize(NSFont.SmallSystemFontSize);
            txtWt.Cell.SetSendsActionOnEndEditing(true);
            AddSubview(txtWt);
        }

        public NSTextField TxtWt
        {
            get { return txtWt; }
        }

        public NSTextField TxtCity
        {
            get { return txtCity; }
        }

    }
    #endregion

}

