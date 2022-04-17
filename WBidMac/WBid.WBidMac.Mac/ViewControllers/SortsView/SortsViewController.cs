
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using MonoTouch.CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Core.Enum;
using System.Linq;
using static WBid.WBidMac.Mac.ArrivalAndDepartViewController;
using System.IO;

namespace WBid.WBidMac.Mac
{
    public partial class SortsViewController : AppKit.NSViewController
    {
        #region Constructors

        public List<string> blockSort = new List<string>();
        WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListDataCSW();
        NSObject CmtbltyNotification;
        NSObject CommutableManualNotification;
        NSObject RatioNotification;
        NSObject ShowCommuteInfoViewNotification;
        NSObject ApplyBlockSortNotification;
        NSObject ShowCommutableManualNotification;
        //NSObject RelaodBlockSortNotification; 

        // Called when created from unmanaged code
        public SortsViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public SortsViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public SortsViewController() : base("SortsView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new SortsView View
        {
            get
            {
                return (SortsView)base.View;
            }
        }

        public override void AwakeFromNib()
        {
            try
            {
                base.AwakeFromNib();
                btnPayPerFDP.Enabled = false;
                btnPayPerFDP.BezelStyle = NSBezelStyle.SmallSquare;
                if (wBidStateContent.SortDetails.BlokSort == null)
                    wBidStateContent.SortDetails.BlokSort = new List<string>();
                tblBlockSort.Source = new BlockSortSource(this);

                btnBlankToBottom.Activated += btnBlankToBottomClicked;
                btnReserveToBottom.Activated += btnReserveToBottomClicked;
                btnLineNumber.Activated += HandleSortButtons;
                btnLinePay.Activated += HandleSortButtons;
                btnPayPerDay.Activated += HandleSortButtons;
                btnPayPerDH.Activated += HandleSortButtons;
                btnPayPerFH.Activated += HandleSortButtons;
                btnPayPerFDP.Activated += ShowMessage;
                btnPayPerTimeAway.Activated += HandleSortButtons;
                btnBlockSort.Activated += HandleSortButtons;
                btnBlockDrop.Activated += btnBlockDropClicked;
                btnSortByAward.Activated += HandleSortButtons;
                btnSortBySubmitted.Activated += HandleSortButtons;
                btnClear.Activated += ClearTapped;

                setValuesToFixedSorts();
                setViews();
                LoadBlockSort();
                btnHelpSort.Activated += (object sender, EventArgs e) =>
                {
                    if (CommonClass.HelpController == null)
                    {
                        var help = new HelpWindowController();
                        CommonClass.HelpController = help;
                    }
                    CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                    CommonClass.HelpController.tblDocument.SelectRow(3, false);
                    CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(0));
                };

                Initial_ButtonLayOutSetUp();
                //CmtbltyNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"CmtbltySortNotification", AddCmtbltyToSorts);
                //ShowCommuteInfoViewNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"ShowCommuteInfoView", ShowCommuteInfoView);
                // ApplyBlockSortNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"ApplyBlockSort", ApplyBlockSortNotif);


            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }

        }
        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            if (CmtbltyNotification == null)
            {
                CmtbltyNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CmtbltyAutoSortNotification", AddCmtbltyAutoToSorts);
            }

            if (CommutableManualNotification == null)
            {
                CommutableManualNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CommutableManualSortNotification", AddCommutableManualToSorts);
            }

            if (RatioNotification == null)
                RatioNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"RatioNotification", AddRatioToSorts);
            if (ShowCommuteInfoViewNotification == null)
            {
                ShowCommuteInfoViewNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"ShowCommuteInfoView", ShowCommutLienAutoInfoView);
            }

            if (ShowCommutableManualNotification == null)             {                 ShowCommutableManualNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"ShowCommutableManualView", ShowCommutabileLineManual);             }

            ApplyBlockSortNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"ApplyBlockSort", ApplyBlockSortNotif);
            //RelaodBlockSortNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"ReloadBlockSort", ReloadBlockSortNotif);


        }
        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();

            //NSNotificationCenter.DefaultCenter.RemoveObserver(RelaodBlockSortNotification);

        }

        public void RemoveNotifications()
        {
            if (CmtbltyNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(CmtbltyNotification);
            if (CommutableManualNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(CommutableManualNotification);
            if (ShowCommuteInfoViewNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(ShowCommuteInfoViewNotification);
            if (ShowCommutableManualNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(ShowCommutableManualNotification);
            if (ApplyBlockSortNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(ApplyBlockSortNotification);

            if (RatioNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(RatioNotification);
        }
        void Initial_ButtonLayOutSetUp()
        {
            if (btnBlankToBottom.State == NSCellStateValue.On) ButtonActiveLayout(btnBlankToBottom);
            else ButtonInActiveLayout(btnBlankToBottom);


            if (btnReserveToBottom.State == NSCellStateValue.On) ButtonActiveLayout(btnReserveToBottom);
            else ButtonInActiveLayout(btnReserveToBottom);

            if (btnLineNumber.State == NSCellStateValue.On) ButtonActiveLayout(btnLineNumber);
            else ButtonInActiveLayout(btnLineNumber);


            if (btnLinePay.State == NSCellStateValue.On) ButtonActiveLayout(btnLinePay);
            else ButtonInActiveLayout(btnLinePay);


            if (btnPayPerDay.State == NSCellStateValue.On) ButtonActiveLayout(btnPayPerDay);
            else ButtonInActiveLayout(btnPayPerDay);

            if (btnPayPerDH.State == NSCellStateValue.On) ButtonActiveLayout(btnPayPerDH);
            else ButtonInActiveLayout(btnPayPerDH);

            if (btnPayPerFH.State == NSCellStateValue.On) ButtonActiveLayout(btnPayPerFH);
            else ButtonInActiveLayout(btnPayPerFH);

            if (btnPayPerFDP.State == NSCellStateValue.On) ButtonActiveLayout(btnPayPerFDP);
            else ButtonInActiveLayout(btnPayPerFDP);

            if (btnPayPerTimeAway.State == NSCellStateValue.On) ButtonActiveLayout(btnPayPerTimeAway);
            else ButtonInActiveLayout(btnPayPerTimeAway);

            if (btnSortByAward.State == NSCellStateValue.On) ButtonActiveLayout(btnSortByAward);
            else ButtonInActiveLayout(btnSortByAward);

            if (btnSortBySubmitted.State == NSCellStateValue.On) ButtonActiveLayout(btnSortBySubmitted);
            else ButtonInActiveLayout(btnSortBySubmitted);

            if (btnBlockSort.State == NSCellStateValue.On) ButtonActiveLayout(btnBlockSort);
            else ButtonInActiveLayout(btnBlockSort);

            if(GlobalSettings.CurrentBidDetails.Postion=="FA" && GlobalSettings.CurrentBidDetails.Round=="M")
            {
                btnSortByAward.Enabled = false;
                btnSortBySubmitted.Enabled = false;
            }
            else
            {
                btnSortByAward.Enabled = true;
                btnSortBySubmitted.Enabled = true;
            }


        }
        void LoadBlockSort()
        {
            if (wBidStateContent.SortDetails.BlokSort.Count != 0)
            {
                foreach (var id in wBidStateContent.SortDetails.BlokSort)
                {
                    string name;
                    //if (id == "30" || id == "31" || id == "32")
                    //{
                    //    //commutability
                    //    name=WBidCollection.GetBlockSortListDataCSW ().FirstOrDefault (x => x.Id == 18).Name;
                    //}
                    if (id == "33" || id == "34" || id == "35")
                    {
                        //commutabille line auto
                        name = WBidCollection.GetBlockSortListDataCSW().FirstOrDefault(x => x.Id == 23).Name;
                    }
                    else if (id == "36" || id == "37" || id == "38")
                    {
                        //commutabille line auto
                        name = WBidCollection.GetBlockSortListDataCSW().FirstOrDefault(x => x.Id == 24).Name;
                    }
                    else
                    {
                        name = lstblockData.FirstOrDefault(x => x.Id == int.Parse(id)).Name;

                    }
                    blockSort.Add(name);
                    btnBlockDrop.Items().FirstOrDefault(x => x.Title == name).State = NSCellStateValue.On;
                }
                tblBlockSort.ReloadData();
            }
        }

        void LoadBlockSortFOrCommutability()
        {
            if (wBidStateContent.SortDetails.BlokSort.Count != 0)
            {
                foreach (var id in wBidStateContent.SortDetails.BlokSort)
                {
                    string name;
                    if (id == "30" || id == "31" || id == "32")
                    {
                        //commutability
                        name = WBidCollection.GetBlockSortListDataCSW().FirstOrDefault(x => x.Id == 18).Name;
                    }
                    else
                    {
                        name = lstblockData.FirstOrDefault(x => x.Id == int.Parse(id)).Name;

                    }
                    blockSort.Add(name);
                    //btnBlockDrop.Items().FirstOrDefault(x => x.Title == name).State = NSCellStateValue.On;
                }
                //tblBlockSort.ReloadData();
            }
        }
        void ClearTapped(object sender, EventArgs e)
        {
            try
            {
                var alert = new NSAlert();
                alert.AlertStyle = NSAlertStyle.Informational;
                alert.MessageText = "WBidMax";
                alert.InformativeText = "Are you sure you want to clear Sorts?";
                alert.AddButton("YES");
                alert.AddButton("No");
                alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
                {
                    //ClearButtons ();
                    alert.Window.Close();
                    NSApplication.SharedApplication.StopModal();
                    ApplySort("Line Number");
                    CommonClass.MainController.ReloadAllContent();
                    CommonClass.CSWController.ReloadAllContent();
                };
                alert.RunModal();

            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        public void setValuesToFixedSorts()
        {

            if (lblSelected == null)
            {
                return;
            }
            // Set label text color for dark mode 
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            if (interfaceStyle == "Dark")
            {
                lblSelected.TextColor = NSColor.Black;
                lblManual.TextColor = NSColor.Black;
            }

            ClearButtons();

            if (wBidStateContent.ForceLine.IsBlankLinetoBottom)
            {
                btnBlankToBottom.State = NSCellStateValue.On;
            }

            if (wBidStateContent.ForceLine.IsReverseLinetoBottom)
            {
                btnReserveToBottom.State = NSCellStateValue.On;
            }
            if (btnBlankToBottom != null)
                btnBlankToBottom.BezelStyle = NSBezelStyle.TexturedSquare;
            if (btnReserveToBottom != null)
                btnReserveToBottom.BezelStyle = NSBezelStyle.TexturedSquare;

            SortDetails stateSortDetails = wBidStateContent.SortDetails;
            if (stateSortDetails.SortColumn == "Line" || stateSortDetails.SortColumn == string.Empty)
            {
                this.btnLineNumber.State = NSCellStateValue.On;
                if (btnBlankToBottom != null)
                {
                    btnBlankToBottom.Enabled = false;
                    btnBlankToBottom.BezelStyle = NSBezelStyle.SmallSquare;
                }
                if (btnReserveToBottom != null)
                {
                    btnReserveToBottom.Enabled = false;
                    btnReserveToBottom.BezelStyle = NSBezelStyle.SmallSquare;
                }
            }
            else if (stateSortDetails.SortColumn == "LinePay")
            {
                if (this.btnLinePay != null)
                    this.btnLinePay.State = NSCellStateValue.On;
            }
            else if (stateSortDetails.SortColumn == "PayPerDay")
            {
                if (this.btnPayPerDay != null)
                    this.btnPayPerDay.State = NSCellStateValue.On;
            }
            else if (stateSortDetails.SortColumn == "PayPerDutyHour")
            {
                if (this.btnPayPerDH != null)
                    this.btnPayPerDH.State = NSCellStateValue.On;
            }
            else if (stateSortDetails.SortColumn == "PayPerFlightHour")
            {
                if (this.btnPayPerFH != null)
                    this.btnPayPerFH.State = NSCellStateValue.On;
            }
            else if (stateSortDetails.SortColumn == "PayPerTimeAway")
            {
                if (this.btnPayPerTimeAway != null)
                    this.btnPayPerTimeAway.State = NSCellStateValue.On;
            }
            else if (stateSortDetails.SortColumn == "Award")
            {
                if (this.btnSortByAward != null)
                    this.btnSortByAward.State = NSCellStateValue.On;
            }
            else if (stateSortDetails.SortColumn == "SubmittedBid")
            {
                if (this.btnSortBySubmitted != null)
                    this.btnSortBySubmitted.State = NSCellStateValue.On;
            }
            else if (stateSortDetails.SortColumn == "BlockSort")
            {
                if (this.btnBlockSort != null)
                    this.btnBlockSort.State = NSCellStateValue.On;
            }

            if (stateSortDetails.SortColumn == "SelectedColumn")
            {

                if (this.lblSelected != null)
                    this.lblSelected.BackgroundColor = NSColor.LightGray;

                //this.lblSelected.TextColor = UIColor.White;
            }

            if (stateSortDetails.SortColumn == "Manual")
            {
                if (this.lblManual != null)
                    this.lblManual.BackgroundColor = NSColor.LightGray;
                //this.lblManual.TextColor = UIColor.White;
            }


        }
        void ButtonActiveLayout(NSButton button)
        {

            button.WantsLayer = true;
            button.Layer.BackgroundColor = NSColor.Orange.CGColor;//NSColor.FromCalibratedRgba(.91f, .51f, .21f, 1).CGColor;
            button.Layer.CornerRadius = (nfloat)2;
            button.Layer.BorderColor = NSColor.DarkGray.CGColor;
            button.Layer.BorderWidth = (nfloat)0.5;
            button.NeedsLayout = true;

        }

        void ButtonInActiveLayout(NSButton button)
        {
            button.WantsLayer = true;//NSColor.FromRgba(124/256,206/256,38/256,1).CGColor;
            button.Layer.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1).CGColor;
            button.Layer.CornerRadius = (nfloat)2;
            button.Layer.BorderColor = NSColor.DarkGray.CGColor;
            button.Layer.BorderWidth = (nfloat)0.5;
            button.NeedsLayout = true;
        }

        public void RightClicked(NSEvent ev)
        {
            try
            {
                //Console.WriteLine (ev.Description);
                var point = this.View.ConvertPointToView(ev.LocationInWindow, tblBlockSort);
                var toRow = tblBlockSort.GetRow(point);
                if (tblBlockSort.SelectedRowCount > 0 && toRow >= 0)
                {
                    WBidHelper.PushToUndoStack();
                    var frmRow = tblBlockSort.SelectedRow;
                    var block = blockSort[(int)frmRow];
                    blockSort.RemoveAt((int)frmRow);
                    blockSort.Insert((int)toRow, block);

                    var blockID = wBidStateContent.SortDetails.BlokSort[(int)frmRow];
                    wBidStateContent.SortDetails.BlokSort.RemoveAt((int)frmRow);
                    wBidStateContent.SortDetails.BlokSort.Insert((int)toRow, blockID);

                    ApplyBlockSort();
                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        public void ShowRatioView()
        {
            if (WBidHelper.IsRatioPropertiesSetFromOtherViews(wBidStateContent))
            {
                WBidHelper.PushToUndoStack();
                btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                blockSort.Add(btnBlockDrop.SelectedItem.Title);
                wBidStateContent.SortDetails.BlokSort.Add(lstblockData.FirstOrDefault(x => x.Name == btnBlockDrop.SelectedItem.Title).Id.ToString());
                ApplyBlockSort();
                ReloadView();

            }
            else
            {
                RatioViewControllerController login = new RatioViewControllerController();
                login.FromScreen = "Sort";
                var panel = new NSPanel();
                login.panel = panel;
                panel.SetContentSize(new CoreGraphics.CGSize(293, 221));
                panel.ContentView = login.View;
                NSApplication.SharedApplication.BeginSheet(panel, this.View.Window);
            }
        }



        void btnBlockDropClicked(object sender, EventArgs e)
        {
            try
            {
                if (btnBlockDrop.SelectedItem.Title == "Ratio")
                {
                    if (!blockSort.Contains(btnBlockDrop.SelectedItem.Title))
                    {
                        ShowRatioView();
                    }
                }
                //else if (btnBlockDrop.SelectedItem.Title == "Commutability")
                //{

                //    //if (wBidStateContent.CxWtState.Commute.Wt && wBidStateContent.CxWtState.Commute.Cx) {
                //    //                   return;
                //    //               }
                //    if (wBidStateContent.SortDetails.BlokSort.Contains("30") || wBidStateContent.SortDetails.BlokSort.Contains("31") || wBidStateContent.SortDetails.BlokSort.Contains("32"))
                //    {
                //        //no need to do anything because, application have already commutbility sort applied
                //    }
                //    else
                //    {
                //        if (wBidStateContent.CxWtState.Commute.Cx || wBidStateContent.CxWtState.Commute.Wt)
                //        {
                //            AddCmtbltyToSortCell();
                //        }
                //        else
                //            ShowCommutabilityLine();
                //    }
                //    ReloadView();
                //    return;
                //}
                else if (btnBlockDrop.SelectedItem.Title == "Commutable Line-Auto")
                {
                    if (wBidStateContent.CxWtState.CL.Cx || wBidStateContent.CxWtState.CL.Wt || (wBidStateContent.SortDetails.BlokSort.Contains("36") || wBidStateContent.SortDetails.BlokSort.Contains("37") || wBidStateContent.SortDetails.BlokSort.Contains("38")))
                    {
                        ShowMessage("You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.");
                        return;
                    }


                    if (wBidStateContent.SortDetails.BlokSort.Contains("33") || wBidStateContent.SortDetails.BlokSort.Contains("34") || wBidStateContent.SortDetails.BlokSort.Contains("35"))
                    {
                        //no need to do anything because, application have already commutbility sort applied
                    }
                    else
                    {
                        if (wBidStateContent.CxWtState.CLAuto.Cx || wBidStateContent.CxWtState.CLAuto.Wt)
                        {
                            AddCmtbleAutoSortCell();
                        }
                        else
                            ShowCommutabilityLineAuto();
                    }
                    ReloadView();
                    return;
                }
                else if (btnBlockDrop.SelectedItem.Title == "Commutable Line- Manual")
                {
                    if (wBidStateContent.CxWtState.CLAuto.Cx || wBidStateContent.CxWtState.CLAuto.Wt || (wBidStateContent.SortDetails.BlokSort.Contains("33") || wBidStateContent.SortDetails.BlokSort.Contains("34") || wBidStateContent.SortDetails.BlokSort.Contains("35")))
                    {
                        ShowMessage("You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.");
                        return;
                    }

                    if (wBidStateContent.SortDetails.BlokSort.Contains("36") || wBidStateContent.SortDetails.BlokSort.Contains("37") || wBidStateContent.SortDetails.BlokSort.Contains("38"))
                    {
                        //no need to do anything because, application have already commutbility sort applied
                    }
                    else
                    {
                        if (wBidStateContent.CxWtState.CL.Cx || wBidStateContent.CxWtState.CL.Wt)
                        {
                            AddCmtbleManualSortCell();
                        }
                        else
                            ShowCommutabileLineManual();
                    }

                    ReloadView();
                    return;
                }

                else if (!blockSort.Contains(btnBlockDrop.SelectedItem.Title))
                {
                    WBidHelper.PushToUndoStack();
                    btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                    blockSort.Add(btnBlockDrop.SelectedItem.Title);
                    wBidStateContent.SortDetails.BlokSort.Add(lstblockData.FirstOrDefault(x => x.Name == btnBlockDrop.SelectedItem.Title).Id.ToString());
                    ApplyBlockSort();
                    ReloadView();
                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        public void ReloadView()
        {
            setValuesToFixedSorts();
            Initial_ButtonLayOutSetUp();


        }
        //public void ShowCommuteInfoView (NSNotification n)
        //{
        //    ShowCommutabilityLine ();
        //}
        public void ShowCommutLienAutoInfoView(NSNotification n)
        {
            ShowCommutabilityLineAuto();
        }
        public void ApplyBlockSortNotif(NSNotification n)
        {
            ApplyBlockSort();
        }
        public void ShowCommutabileLineManual(NSNotification n)         {             ShowCommutabileLineManual();         }


        public void ShowCommutabileLineManual()         {              CommutableLineManualController ObjCommute = new CommutableLineManualController();             this.PresentViewControllerAsSheet(ObjCommute);          }


        //public void ReloadBlockSortNotif(NSNotification n)
        //{
        //	tblBlockSort.ReloadData();
        //}

        //To show the commute info view.
        //      public void ShowCommutabilityLine()
        //{

        //	CommutabilityViewController ObjCommute = new CommutabilityViewController();
        //	ObjCommute.Objtype = CommutabilityEnum.CommutabilitySort; 
        //	this.PresentViewControllerAsSheet(ObjCommute);


        //}
        public void ShowCommutabilityLineAuto()
        {

            CommutableViewController ObjCommute = new CommutableViewController();
            ObjCommute.Objtype = CommutableAutoFrom.Sort;
            this.PresentViewControllerAsSheet(ObjCommute);


        }
        /// <summary>
        /// Add commutability cell and apply block sort logic when the user click done button from the commutability parameter view
        /// </summary>
        /// <param name="n">N.</param>
        private void AddCmtbltyAutoToSorts(NSNotification n)
        {
            //if (!blockSort.Contains(btnBlockDrop.SelectedItem.Title))
            //{
            WBidHelper.PushToUndoStack();

            if (!blockSort.Contains("Commutable Line-Auto"))
            {
                btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                blockSort.Add(btnBlockDrop.SelectedItem.Title);
            }
            if (wBidStateContent.SortDetails.BlokSort.Contains("33") || wBidStateContent.SortDetails.BlokSort.Contains("34") || wBidStateContent.SortDetails.BlokSort.Contains("35"))
            {

            }
            else
            {
                //30 is the value for commutability overall sort and it is the default value
                wBidStateContent.SortDetails.BlokSort.Add("33");
            }
            ApplyBlockSort();
            tblBlockSort.ReloadData();

        }

        private void AddCommutableManualToSorts(NSNotification n)
        {
            try
            {
                WBidHelper.PushToUndoStack();

                if (!blockSort.Contains("Commutable Line- Manual"))
                {
                    if (btnBlockDrop.SelectedItem != null)
                        btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                    blockSort.Add("Commutable Line- Manual");
                }
                if (wBidStateContent.SortDetails.BlokSort.Contains("36") || wBidStateContent.SortDetails.BlokSort.Contains("37") || wBidStateContent.SortDetails.BlokSort.Contains("38"))
                {

                }
                else
                {

                    wBidStateContent.SortDetails.BlokSort.Add("36");
                }
                ApplyBlockSort();
                tblBlockSort.ReloadData();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }

        }



        private void AddRatioToSorts(NSNotification n)
        {

            var objectData = n.Object.ToString();

            if (objectData == "OK")
            {
                WBidHelper.PushToUndoStack();
                btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                blockSort.Add(btnBlockDrop.SelectedItem.Title);
                wBidStateContent.SortDetails.BlokSort.Add(lstblockData.FirstOrDefault(x => x.Name == btnBlockDrop.SelectedItem.Title).Id.ToString());
                ApplyBlockSort();
                ReloadView();
            }
            else
            {

            }

        }
        /// <summary>
        /// Executes when The commutability is applied in either constraint or weight and commutability drpdown in sort is clicked.
        /// </summary>
        private void AddCmtbltyToSortCell()
        {
            if (!blockSort.Contains(btnBlockDrop.SelectedItem.Title))
            {
                WBidHelper.PushToUndoStack();
                btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                blockSort.Add(btnBlockDrop.SelectedItem.Title);
                wBidStateContent.SortDetails.BlokSort.Add("30");
                ApplyBlockSort();
            }

        }
        /// <summary>
        /// Executes when The commuable line auto is applied in either constraint or weight and commutabile line auto drpdown in sort is clicked.
        /// </summary>
        private void AddCmtbleAutoSortCell()
        {
            if (!blockSort.Contains(btnBlockDrop.SelectedItem.Title))
            {
                WBidHelper.PushToUndoStack();
                btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                blockSort.Add(btnBlockDrop.SelectedItem.Title);
                wBidStateContent.SortDetails.BlokSort.Add("33");
                ApplyBlockSort();
            }

        }

        /// <summary>
        /// Executes when The commuable line manual is applied in either constraint or weight and commutabile line manual drpdown in sort is clicked.
        /// </summary>
        private void AddCmtbleManualSortCell()
        {
            if (!blockSort.Contains(btnBlockDrop.SelectedItem.Title))
            {
                WBidHelper.PushToUndoStack();
                btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
                blockSort.Add(btnBlockDrop.SelectedItem.Title);
                wBidStateContent.SortDetails.BlokSort.Add("36");
                ApplyBlockSort();
            }

        }


        public void ApplyBlockSort()
        {
            try
            {

                if (blockSort.Count != 0)
                {

                    wBidStateContent.SortDetails.SortColumn = "BlockSort";
                    ApplySort("BlockSort");
                    setValuesToFixedSorts();
                    setViews();
                }
                else
                {
                    wBidStateContent.SortDetails.SortColumn = "Line";
                    ApplySort("Line Number");
                    setValuesToFixedSorts();
                    setViews();
                }
                CommonClass.columnID = 0;
                //				CommonClass.SummaryController.LoadContent ();
                CommonClass.MainController.ReloadAllContent();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        public void ApplyBlockSortFromCommutability()
        {
            try
            {
                //LoadBlockSortFOrCommutability();

                if (wBidStateContent.SortDetails.BlokSort.Count != 0)
                {

                    wBidStateContent.SortDetails.SortColumn = "BlockSort";
                    ApplySort("BlockSort");
                    //setValuesToFixedSorts();
                    //setViews();
                }
                else
                {
                    wBidStateContent.SortDetails.SortColumn = "Line";
                    ApplySort("Line Number");
                    //setValuesToFixedSorts();
                    //setViews();
                }
                CommonClass.columnID = 0;
                //				CommonClass.SummaryController.LoadContent ();
                CommonClass.MainController.ReloadAllContent();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        public void RemoveBlockSort(string sort)
        {
            blockSort.Remove(sort);
            if (sort == "Commutability")
            {
                wBidStateContent.SortDetails.BlokSort.RemoveAll(x => x == "30" || x == "31" || x == "32");
            }
            else if (sort == "Commutable Line-Auto")
            {
                wBidStateContent.SortDetails.BlokSort.RemoveAll(x => x == "33" || x == "34" || x == "35");
            }
            else if (sort == "Commutable Line- Manual")
            {
                wBidStateContent.SortDetails.BlokSort.RemoveAll(x => x == "36" || x == "37" || x == "38");
            }
            else
            {
                wBidStateContent.SortDetails.BlokSort.Remove(lstblockData.FirstOrDefault(x => x.Name == sort).Id.ToString());
            }
            btnBlockDrop.Items().FirstOrDefault(x => x.Title == sort).State = NSCellStateValue.Off;
            ApplyBlockSort();
            ReloadView();
        }

        void btnBlankToBottomClicked(object sender, EventArgs e)
        {
            try
            {
                WBidHelper.PushToUndoStack();
                if (btnBlankToBottom.State == NSCellStateValue.On)
                {
                    wBidStateContent.ForceLine.IsBlankLinetoBottom = true;
                    LineOperations.ForceBlankLinestoBottom();
                    ButtonActiveLayout(btnBlankToBottom);
                }
                else
                {
                    ButtonInActiveLayout(btnBlankToBottom);
                    wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
                }
                CommonClass.MainController.ReloadAllContent();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        void btnReserveToBottomClicked(object sender, EventArgs e)
        {
            try
            {
                WBidHelper.PushToUndoStack();
                if (btnReserveToBottom.State == NSCellStateValue.On)
                {
                    wBidStateContent.ForceLine.IsReverseLinetoBottom = true;
                    LineOperations.ForceReserveLinestoBottom();
                    ButtonActiveLayout(btnReserveToBottom);
                }
                else
                {
                    ButtonInActiveLayout(btnReserveToBottom);
                    wBidStateContent.ForceLine.IsReverseLinetoBottom = false;
                }
                CommonClass.MainController.ReloadAllContent();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        void ClearButtons()
        {
            try
            {

                if (btnLineNumber != null)
                {
                    btnLineNumber.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnLineNumber);
                }
                if (btnLinePay != null)
                {
                    btnLinePay.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnLinePay);
                }
                if (btnPayPerDay != null)
                {
                    btnPayPerDay.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnPayPerDay);
                }
                if (btnPayPerDH != null)
                {
                    btnPayPerDH.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnPayPerDH);
                }
                if (btnPayPerFH != null)
                {
                    btnPayPerFH.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnPayPerFH);
                }
                if (btnPayPerFDP != null)
                {
                    btnPayPerFDP.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnPayPerFDP);
                }
                if (btnPayPerTimeAway != null)
                {
                    btnPayPerTimeAway.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnPayPerTimeAway);
                }
                if (btnSortByAward != null)
                {
                    btnSortByAward.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnSortByAward);
                }
                if (btnSortBySubmitted != null)
                {
                    btnSortBySubmitted.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnSortBySubmitted);
                }
                if (btnBlockSort != null)
                {
                    btnBlockSort.State = NSCellStateValue.Off;
                    ButtonInActiveLayout(btnBlockSort);
                }

                if (lblSelected != null)
                {
                    lblSelected.BackgroundColor = NSColor.White;
                }
                if (lblManual != null)
                {
                    lblManual.BackgroundColor = NSColor.White;
                }

            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        void ShowMessage(object sender, EventArgs e)
        {
            //
            var alert = new NSAlert();
            alert.AlertStyle = NSAlertStyle.Warning;
            alert.MessageText = "Information";
            alert.InformativeText = "Not yet functional";
            alert.RunModal();
        }
        void ShowMessage(string text)
        {
            var alert = new NSAlert();
            alert.AlertStyle = NSAlertStyle.Warning;
            alert.MessageText = "Information";
            alert.InformativeText = text;
            alert.RunModal();
        }
        void HandleSortButtons(object sender, EventArgs e)
        {

            try
            {

                var btn = (NSButton)sender;

                bool isneedtoSetActiveButtons = false;
                if (btn == btnSortByAward)
                {
                    if (GlobalSettings.WBidStateCollection.BidAwards == null)
                        GlobalSettings.WBidStateCollection.BidAwards = new List<BidAward>();
                    //if (GlobalSettings.WBidStateCollection.BidAwards.Count == 0)
                    //{
                    bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                    if (isConnectionAvailable)
                    {


                        MonthlyBidDetails objbiddetails = new MonthlyBidDetails();
                        objbiddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                        objbiddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                        objbiddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                        objbiddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                        objbiddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                        var jsonData = ServiceUtils.JsonSerializer(objbiddetails);
                        StreamReader dr = ServiceUtils.GetRestData("GetMonthlyAwardData", jsonData);
                        GlobalSettings.WBidStateCollection.BidAwards = (WBidCollection.ConvertJSonStringToObject<BidAwardDetails>(dr.ReadToEnd())).BidAwards;

                        if (GlobalSettings.WBidStateCollection.BidAwards == null)
                            GlobalSettings.WBidStateCollection.BidAwards = new List<BidAward>();

                    }
                    else
                    {
                        string alertmessage = GlobalSettings.VPSDownAlert;
                        if (Reachability.IsSouthWestWifiOr2wire())
                        {
                            alertmessage = GlobalSettings.SouthWestConnectionAlert;
                        }
                        ShowMessageBox("WBidMax", alertmessage);
                        return;
                    }
                    //}
                    if (GlobalSettings.WBidStateCollection.BidAwards.Count == 0)
                    {
                        ShowMessageBox("WBidMax", "The Bid Awards are not available.");
                        return;
                    }
                    else
                    {
                        isneedtoSetActiveButtons = true;
                        btn.State = NSCellStateValue.On;

                    }
                }
                else if (btn == btnSortBySubmitted)
                {
                    if (GlobalSettings.WBidStateCollection.SubmittedResult == null || GlobalSettings.WBidStateCollection.SubmittedResult == string.Empty)
                    {
                        bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                        if (isConnectionAvailable)
                        {
                            BidSubmittedData objbiddetails = new BidSubmittedData();
                            objbiddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                            objbiddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                            objbiddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                            objbiddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                            objbiddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                            objbiddetails.EmpNum = Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                            var jsonData = ServiceUtils.JsonSerializer(objbiddetails);
                            StreamReader dr = ServiceUtils.GetRestData("GetBidSubmittedData", jsonData);
                            GlobalSettings.WBidStateCollection.SubmittedResult = (WBidCollection.ConvertJSonStringToObject<BidSubmittedData>(dr.ReadToEnd())).SubmittedResult;

                        }
                        else
                        {
                            string alertmessage = GlobalSettings.VPSDownAlert;
                            if (Reachability.IsSouthWestWifiOr2wire())
                            {
                                alertmessage = GlobalSettings.SouthWestConnectionAlert;
                            }

                            ShowMessageBox("WBidMax", alertmessage);
                            return;
                        }
                    }
                    if (GlobalSettings.WBidStateCollection.BidAwards.Count == 0)
                    {

                        ShowMessageBox("Sorry", "We have no record of a Submitted Bid.  You can login to SwaLife and go see all of your submitted bids.\n\nPilots: My Work => Flight Ops => Our Business => Bid Info => BidInfo => Search Bids\n\nFlight Attendants: My Work = Inflight => Bidding => BidInfo => BidInfo => Search Bids.");
                        return;
                    }
                    else
                    {
                        isneedtoSetActiveButtons = true;
                        btn.State = NSCellStateValue.On;

                    }
                }
                else
                {
                    btn.State = NSCellStateValue.On;
                }

                    ClearButtons();
                if(isneedtoSetActiveButtons)
                {
                    btn.State = NSCellStateValue.On;
                }
                //btn.State = NSCellStateValue.On;
                setViews();
                ApplySort(btn.Title);

                CommonClass.columnID = 0;
                //				CommonClass.SummaryController.LoadContent ();
                CommonClass.MainController.ReloadAllContent();
                Initial_ButtonLayOutSetUp();
                if (btn == btnLineNumber)
                {
                    ButtonActiveLayout(btnLineNumber);
                    //					ButtonInActiveLayout (btnLinePay);
                    //					ButtonInActiveLayout (btnPayPerDay);
                    //					ButtonInActiveLayout (btnPayPerDH);
                }
                else if (btn == btnLinePay)
                {
                    ButtonActiveLayout(btnLinePay);

                }
                else if (btn == btnPayPerDay)
                {
                    ButtonActiveLayout(btnPayPerDay);

                }

                else if (btn == btnPayPerDH)
                {
                    ButtonActiveLayout(btnPayPerDH);

                }

                else if (btn == btnPayPerFH)
                {
                    ButtonActiveLayout(btnPayPerFH);

                }
                else if (btn == btnSortByAward)
                {
                    if(btn.State == NSCellStateValue.On)
                        ButtonActiveLayout(btnSortByAward);

                }
                else if (btn == btnSortBySubmitted)
                {
                    if (btn.State == NSCellStateValue.On)
                        ButtonActiveLayout(btnSortBySubmitted);

                }


            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        private void ShowMessageBox(string title, string content)
        {
            var alert = new NSAlert();
            alert.MessageText = title;
            alert.InformativeText = content;
            alert.RunModal();
        }
        public void setViews()
        {

            if (btnBlockDrop == null)
            {
                return;
            }

            btnBlockDrop.RemoveAllItems();
            List<string> lstBlockSort = new List<string>();
            foreach (var item in lstblockData)
            {
                lstBlockSort.Add(item.Name);
            }
            btnBlockDrop.AddItem("");
            btnBlockDrop.AddItems(lstBlockSort.ToArray());
            if (blockSort.Count == 0)
            {
                btnBlockSort.Enabled = false;
                btnBlockSort.BezelStyle = NSBezelStyle.SmallSquare;
            }
            else
            {
                btnBlockSort.Enabled = true;
                btnBlockSort.BezelStyle = NSBezelStyle.TexturedSquare;
            }

            foreach (var item in blockSort)
            {
                btnBlockDrop.Items().FirstOrDefault(x => x.Title == item.Trim()).State = NSCellStateValue.On;
            }

            btnBlankToBottom.BezelStyle = NSBezelStyle.TexturedSquare;
            btnReserveToBottom.BezelStyle = NSBezelStyle.TexturedSquare;

            if (GlobalSettings.CurrentBidDetails != null && GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M")
            {

                btnBlankToBottom.Enabled = false;
                btnBlankToBottom.State = NSCellStateValue.Off;
                btnReserveToBottom.Enabled = false;
                btnReserveToBottom.State = NSCellStateValue.Off;
                btnBlankToBottom.BezelStyle = NSBezelStyle.SmallSquare;
                btnReserveToBottom.BezelStyle = NSBezelStyle.SmallSquare;

            }
            else
            {
                if (btnLineNumber.State == NSCellStateValue.On)
                {
                    btnBlankToBottom.Enabled = false;
                    btnBlankToBottom.State = NSCellStateValue.Off;
                    btnReserveToBottom.Enabled = false;
                    btnReserveToBottom.State = NSCellStateValue.Off;
                    btnBlankToBottom.BezelStyle = NSBezelStyle.SmallSquare;
                    btnReserveToBottom.BezelStyle = NSBezelStyle.SmallSquare;

                }
                else
                {
                    btnBlankToBottom.Enabled = true;
                    btnReserveToBottom.Enabled = true;
                }
            }
            tblBlockSort.ReloadData();
        }


        public void ApplySort(string sortParameter)
        {
            string sortKey = string.Empty;
            SortCalculation sort = new SortCalculation();
            switch (sortParameter)
            {
                case "Line Number":
                    sortKey = "Line";
                    break;
                case "Line Pay":
                    sortKey = "LinePay";
                    break;
                case "Pay Per Day":
                    sortKey = "PayPerDay";
                    break;
                case "Pay Per Duty Hour":
                    sortKey = "PayPerDutyHour";
                    break;

                case "Pay Per Flight Hour":
                    sortKey = "PayPerFlightHour";
                    break;
                case "Pay Per Time Away From Base":
                    sortKey = "PayPerTimeAway";
                    break;
                case "Sort By Award":
                    sortKey = "Award";
                    break;
                case "Sort By Submitted Bid":
                    sortKey = "SubmittedBid";
                    break;
                case "Selected Column":
                    sortKey = "SelectedColumn";
                    break;
                case "Block Sort":
                    sortKey = "BlockSort";
                    break;

            }
            if (sortKey != "" && sortKey != "BlockSort")
                WBidHelper.PushToUndoStack();

            if (sortKey != string.Empty)
            {
                sort.SortLines(sortKey);
                wBidStateContent.SortDetails.SortColumn = sortKey;
            }
        }

    }

	public partial class BlockSortSource : NSTableViewSource
	{
		SortsViewController parentVC;

		public BlockSortSource (SortsViewController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.blockSort.Count;
		}

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            SortCell vw;
            //if (parentVC.blockSort[(int)row] == "Commutability")
            //{
            //    vw = (SortCell)tableView.MakeView("CommutabilitySort", this);
            //}
            if (parentVC.blockSort[(int)row] == "Commutable Line-Auto")
            {
                vw = (SortCell)tableView.MakeView("Commutable Line-Auto", this);
            }
            else if (parentVC.blockSort[(int)row] == "Commutable Line- Manual")
            {
                vw = (SortCell)tableView.MakeView("CommutableManual", this);
            }
            else
            {
                vw = (SortCell)tableView.MakeView("Sort", this);
            }
            vw.BindData(parentVC.blockSort[(int)row], (int)row);
            return vw;

        }
		public override bool SelectionShouldChange(NSTableView tableView)
		{
			return false;
		}
	}

}

