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
    public partial class CommutableLineManualController : AppKit.NSViewController
    {
        #region Constructors
        int order;
        public static WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

        static WBidIntialState wbidintialState;
        // Called when created from unmanaged code
        public CommutableLineManualController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public CommutableLineManualController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public CommutableLineManualController() : base("CommutableLineManual", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }


        public override void AwakeFromNib()
        {
            try
            {
                base.AwakeFromNib();

                wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
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

                //EnableDisableTimeFields();

                dpMonThuCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.Checkin)).DateTimeToNSDate();
                dpFriCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.Checkin)).DateTimeToNSDate();
                dpSatCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.Checkin)).DateTimeToNSDate();
                dpSunCheckIn.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.Checkin)).DateTimeToNSDate();

                dpMonThuToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.MondayThu.BackToBase)).DateTimeToNSDate();
                dpFriToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Friday.BackToBase)).DateTimeToNSDate();
                dpSatToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Saturday.BackToBase)).DateTimeToNSDate();
                dpSunToBase.DateValue = DateTime.Parse(Helper.ConvertMinuteToHHMM(wBIdStateContent.Constraints.CL.Sunday.BackToBase)).DateTimeToNSDate();

                //bool enableDeffBtns = checkChangesInDefaultsValue();
                //btnLoadDefaults.Enabled = enableDeffBtns;
                //btnSaveDefaults.Enabled = enableDeffBtns;


                btnHelp.Activated += (object sender, EventArgs e) =>
                {
                    if (CommonClass.HelpController == null)
                    {
                        var help = new HelpWindowController();
                        CommonClass.HelpController = help;
                    }
                    CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                    CommonClass.HelpController.tblDocument.SelectRow(1, false);
                    CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(ConstraintsApplied.HelpPageNo["Commutable Lines - Manual"] - 1));
                };

                btnCloseCL.Activated += (object sender, EventArgs e) =>
                {
                    CommonClass.SortController.RemoveBlockSort(("Commutable Line- Manual"));
                   

                };
                btnCommuteWork.Activated += (object sender, EventArgs e) =>
                {

                    if (btnCommuteWork.State == NSCellStateValue.Off && btnCommuteHome.State == NSCellStateValue.Off)
                    {
                        btnCommuteHome.State = NSCellStateValue.On;
                        wBIdStateContent.Constraints.CL.CommuteToWork = false;
                        wBIdStateContent.Constraints.CL.CommuteToHome = true;
                    }
                    else
                    {
                        wBIdStateContent.Constraints.CL.CommuteToWork = (btnCommuteWork.State == NSCellStateValue.On);
                    }
                    EnableDisableTimeFields();
                    //CommonClass.ConstraintsController.ApplyAndReloadConstraints("Commutable Lines - Manual");
                };
                btnCommuteHome.Activated += (object sender, EventArgs e) =>
                {
                    WBidHelper.PushToUndoStack();
                    if (btnCommuteWork.State == NSCellStateValue.Off && btnCommuteHome.State == NSCellStateValue.Off)
                    {
                        btnCommuteWork.State = NSCellStateValue.On;
                        wBIdStateContent.Constraints.CL.CommuteToWork = true;
                        wBIdStateContent.Constraints.CL.CommuteToHome = false;
                    }
                    else
                    {
                        wBIdStateContent.Constraints.CL.CommuteToHome = (btnCommuteHome.State == NSCellStateValue.On);
                    }
                    EnableDisableTimeFields();

                };
                btnInDom.Activated += (object sender, EventArgs e) =>
                {
                    WBidHelper.PushToUndoStack();
                    btnInDom.State = NSCellStateValue.On;
                    btnBothEnds.State = NSCellStateValue.Off;
                    wBIdStateContent.Constraints.CL.AnyNight = true;
                    wBIdStateContent.Constraints.CL.RunBoth = false;

                };
                btnBothEnds.Activated += (object sender, EventArgs e) =>
                {
                    WBidHelper.PushToUndoStack();
                    btnInDom.State = NSCellStateValue.Off;
                    btnBothEnds.State = NSCellStateValue.On;
                    wBIdStateContent.Constraints.CL.AnyNight = false;
                    wBIdStateContent.Constraints.CL.RunBoth = true;
                   
                };
                btnLoadDefaults.Activated += (object sender, EventArgs e) =>
                {
                    wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    WBidHelper.PushToUndoStack();
                    wBIdStateContent.Constraints.CL.MondayThu.Checkin = wbidintialState.Weights.CL.DefaultTimes[0].Checkin;
                    wBIdStateContent.Constraints.CL.Friday.Checkin = wbidintialState.Weights.CL.DefaultTimes[1].Checkin;
                    wBIdStateContent.Constraints.CL.Saturday.Checkin = wbidintialState.Weights.CL.DefaultTimes[2].Checkin;
                    wBIdStateContent.Constraints.CL.Sunday.Checkin = wbidintialState.Weights.CL.DefaultTimes[3].Checkin;
                    wBIdStateContent.Constraints.CL.MondayThu.BackToBase = wbidintialState.Weights.CL.DefaultTimes[0].BackToBase;
                    wBIdStateContent.Constraints.CL.Friday.BackToBase = wbidintialState.Weights.CL.DefaultTimes[1].BackToBase;
                    wBIdStateContent.Constraints.CL.Saturday.BackToBase = wbidintialState.Weights.CL.DefaultTimes[2].BackToBase;
                    wBIdStateContent.Constraints.CL.Sunday.BackToBase = wbidintialState.Weights.CL.DefaultTimes[3].BackToBase;

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


                btnDoneButton.Activated += (object sender, EventArgs e) =>
                {
                    DoneButtonEvent();
                    this.DismissViewController(this);
                };



                dpMonThuCheckIn.Activated += TimeValuesChanged;
                dpFriCheckIn.Activated += TimeValuesChanged;
                dpSatCheckIn.Activated += TimeValuesChanged;
                dpSunCheckIn.Activated += TimeValuesChanged;
                dpMonThuToBase.Activated += TimeValuesChanged;
                dpFriToBase.Activated += TimeValuesChanged;
                dpSatToBase.Activated += TimeValuesChanged;
                dpSunToBase.Activated += TimeValuesChanged;
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                // CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }

        }
        private void DoneButtonEvent()
        {
            wBIdStateContent.Constraints.CL.CommuteToHome = (btnCommuteHome.State == NSCellStateValue.On);
            wBIdStateContent.Constraints.CL.CommuteToWork = (btnCommuteWork.State == NSCellStateValue.On);
            wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());             wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());             wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());             wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());             wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());             wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());             wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());             wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());


            wBIdStateContent.Weights.CL.TimesList[0].Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[1].Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[2].Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[3].Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[0].BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[1].BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[2].BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());
            wBIdStateContent.Weights.CL.TimesList[3].BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());

            if (btnCommuteWork.State == NSCellStateValue.Off && btnCommuteHome.State == NSCellStateValue.Off)
            {
                btnCommuteHome.State = NSCellStateValue.On;
                wBIdStateContent.Weights.CL.Type = 2;
            }
            else if (btnCommuteWork.State == NSCellStateValue.On && btnCommuteHome.State == NSCellStateValue.On)
            {
                wBIdStateContent.Weights.CL.Type = 1;
            }
            else
            {
                wBIdStateContent.Weights.CL.Type = 3;
            }
            wBIdStateContent.Constraints.CL.CommuteToWork = (btnCommuteWork.State == NSCellStateValue.On);
            wBIdStateContent.Constraints.CL.CommuteToHome = (btnCommuteHome.State == NSCellStateValue.On);

            bool enableDefBtns = checkChangesInDefaultsValue();
            btnLoadDefaults.Enabled = enableDefBtns;
            btnSaveDefaults.Enabled = enableDefBtns; 

            //we dont need to appy block sort logic becuase it will calulates in the notifications
            if (wBIdStateContent.CxWtState.CLAuto.Cx)
                CommonClass.ConstraintsController.ApplyAndReloadConstraints("Commutable Lines - Manual");
            if (wBIdStateContent.CxWtState.CLAuto.Wt)
                CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Manual");
            //call this like notification to apply block sort
            NSNotificationCenter.DefaultCenter.PostNotificationName("CommutableManualSortNotification", null);

           
        }

        void TimeValuesChanged(object sender, EventArgs e)
        {
            WBidHelper.PushToUndoStack();
            wBIdStateContent.Constraints.CL.MondayThu.Checkin = Helper.ConvertHHMMtoMinute(dpMonThuCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Friday.Checkin = Helper.ConvertHHMMtoMinute(dpFriCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Saturday.Checkin = Helper.ConvertHHMMtoMinute(dpSatCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Sunday.Checkin = Helper.ConvertHHMMtoMinute(dpSunCheckIn.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.MondayThu.BackToBase = Helper.ConvertHHMMtoMinute(dpMonThuToBase.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Friday.BackToBase = Helper.ConvertHHMMtoMinute(dpFriToBase.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Saturday.BackToBase = Helper.ConvertHHMMtoMinute(dpSatToBase.DateValue.LocalValue());
            wBIdStateContent.Constraints.CL.Sunday.BackToBase = Helper.ConvertHHMMtoMinute(dpSunToBase.DateValue.LocalValue());
            CommonClass.ConstraintsController.ApplyAndReloadConstraints("Commutable Lines - Manual");

            bool enableDefBtns = checkChangesInDefaultsValue();
            btnLoadDefaults.Enabled = enableDefBtns;
            btnSaveDefaults.Enabled = enableDefBtns;
        }

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
            if (wBIdStateContent.Constraints.CL.CommuteToWork)
            {
                dpMonThuCheckIn.Enabled = true;
                dpFriCheckIn.Enabled = true;
                dpSatCheckIn.Enabled = true;
                dpSunCheckIn.Enabled = true;
            }
            if (wBIdStateContent.Constraints.CL.CommuteToHome)
            {
                dpMonThuToBase.Enabled = true;
                dpFriToBase.Enabled = true;
                dpSatToBase.Enabled = true;
                dpSunToBase.Enabled = true;
            }
        }
        #endregion

        //strongly typed view accessor
        public new CommutableLineManual View
        {
            get
            {
                return (CommutableLineManual)base.View;
            }
        }
    }
}
