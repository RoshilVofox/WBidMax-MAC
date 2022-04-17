
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

namespace WBid.WBidMac.Mac
{
    public partial class WeightsViewController : AppKit.NSViewController
    {
        #region Constructors
        public List<string> appliedWeights = new List<string>();
        NSObject CLOLoadNotification1;
        NSObject CmtbltyWeightLoadNotification;
        WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
        WeightCalculation weightCalc = new WeightCalculation();
        NSObject CLOWeightNotification;
        NSObject CmtbltyNotification;
        // Called when created from unmanaged code
        public WeightsViewController(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public WeightsViewController(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public WeightsViewController() : base("WeightsView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new WeightsView View
        {
            get
            {
                return (WeightsView)base.View;
            }
        }

        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();

        }

        public void RemoveNotifications()
        {
            if (CLOLoadNotification1 != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(CLOLoadNotification1);
            if (CLOWeightNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(CLOWeightNotification);
            if (CmtbltyWeightLoadNotification != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(CmtbltyWeightLoadNotification);
                CmtbltyWeightLoadNotification = null;
            }
        }
        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
            CLOLoadNotification1 = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CLOLoadNotification1", ShowLine);
            if (CmtbltyWeightLoadNotification == null)
            {
                CmtbltyWeightLoadNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CmtbltyWeightLoadNotification", ShowCommutabilityDetails);
            }
        }
        public override void AwakeFromNib()
        {
            try
            {
                base.AwakeFromNib();

                WeightsApplied.MainList = WeightsApplied.MainList.OrderBy(x => x).ToList();
                btnAddWeights.RemoveAllItems();
                btnAddWeights.AddItem("Add Weights");
                btnAddWeights.AddItems(WeightsApplied.MainList.ToArray());
                btnAddWeights.Activated += btnAddWeightsClicked;
                tblWeights.Source = new WeightsTableSource(this);
                btnClear.Activated += ClearTapped;
                LoadWeights();
                btnHelpWt.Activated += (object sender, EventArgs e) =>
                {
                    if (CommonClass.HelpController == null)
                    {
                        var help = new HelpWindowController();
                        CommonClass.HelpController = help;
                    }
                    CommonClass.HelpController.Window.MakeKeyAndOrderFront(this);
                    CommonClass.HelpController.tblDocument.SelectRow(2, false);
                    CommonClass.HelpController.pdfDocView.GoToPage(CommonClass.HelpController.pdfDocView.Document.GetPage(0));
                };

            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        public void ShowCommutabilityDetails(NSNotification n)
        {
            ShowCommutabilityLine();
        }
        void LoadWeights()
        {
            if (wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt)
            {
                appliedWeights.Add("Largest Block of Days Off");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Largest Block of Days Off").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.NormalizeDays.Wt)
            {
                appliedWeights.Add("Normalize Days Off");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Normalize Days Off").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.AMPM.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.AM_PM.lstParameters)
                {
                    appliedWeights.Add("AM/PM");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "AM/PM").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.CitiesLegs.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.CitiesLegs.lstParameters)
                {
                    appliedWeights.Add("Cities-Legs");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Cities-Legs").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.DHDFoL.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.DHDFoL.lstParameters)
                {
                    appliedWeights.Add("DH - first - last");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "DH - first - last").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.InterConus.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.InterConus.lstParameters)
                {
                    appliedWeights.Add("Intl – NonConus");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Intl – NonConus").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.NODO.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.NODO.lstParameters)
                {
                    appliedWeights.Add("Number of Days Off");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Number of Days Off").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.RON.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.RON.lstParameters)
                {
                    appliedWeights.Add("Overnight Cities");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Overnight Cities").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.Position.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.POS.lstParameters)
                {
                    appliedWeights.Add("Position");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Position").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.SDOW.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.SDOW.lstParameters)
                {
                    appliedWeights.Add("Start Day of Week");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Start Day of Week").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.PerDiem.Wt)
            {
                appliedWeights.Add("Time-Away-From-Base");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Time-Away-From-Base").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.TL.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.TL.lstParameters)
                {
                    appliedWeights.Add("Trip Length");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Trip Length").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.WB.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.WB.lstParameters)
                {
                    appliedWeights.Add("Work Blk Length");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Work Blk Length").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.ACChg.Wt)
            {
                appliedWeights.Add("Aircraft Changes");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Aircraft Changes").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.BDO.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.BDO.lstParameters)
                {
                    appliedWeights.Add("Blocks of Days Off");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Blocks of Days Off").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.DHD.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.DHD.lstParameters)
                {
                    appliedWeights.Add("Cmut DHs");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Cmut DHs").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.DP.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.DP.lstParameters)
                {
                    appliedWeights.Add("Duty period");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Duty period").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.EQUIP.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.EQUIP.lstParameters)
                {
                    appliedWeights.Add("Equipment Type");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Equipment Type").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.ETOPS.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.ETOPS.lstParameters)
                {
                    appliedWeights.Add("ETOPS");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "ETOPS").State = NSCellStateValue.On;
            }

            if (wBIdStateContent.CxWtState.ETOPSRes.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.ETOPSRes.lstParameters)
                {
                    appliedWeights.Add("ETOPS-Res");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "ETOPS-Res").State = NSCellStateValue.On;
            }

            if (wBIdStateContent.CxWtState.FLTMIN.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.FLTMIN.lstParameters)
                {
                    appliedWeights.Add("Flight Time");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Flight Time").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.GRD.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.GRD.lstParameters)
                {
                    appliedWeights.Add("Ground Time");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Ground Time").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.LEGS.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.LEGS.lstParameters)
                {
                    appliedWeights.Add("Legs Per Duty Period");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Legs Per Duty Period").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.LegsPerPairing.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.WtLegsPerPairing.lstParameters)
                {
                    appliedWeights.Add("Legs Per Pairing");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Legs Per Pairing").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.WorkDay.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.WorkDays.lstParameters)
                {
                    appliedWeights.Add("Work Days");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Work Days").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.PDAfter.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.PDAfter.lstParameters)
                {
                    appliedWeights.Add("PDO-after");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "PDO-after").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.PDBefore.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.PDBefore.lstParameters)
                {
                    appliedWeights.Add("PDO-before");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "PDO-before").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.Rest.Wt)
            {
                foreach (var item in wBIdStateContent.Weights.WtRest.lstParameters)
                {
                    appliedWeights.Add("Rest");
                }
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Rest").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.DOW.Wt)
            {
                appliedWeights.Add("Days of the Week");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Days of the Week").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.SDO.Wt)
            {
                appliedWeights.Add("Days of the Month");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Days of the Month").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.BulkOC.Wt)
            {
                appliedWeights.Add("Overnight Cities - Bulk");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Overnight Cities - Bulk").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.CL.Wt)
            {
                appliedWeights.Add("Commutable Lines - Manual");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Commutable Lines - Manual").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.Commute.Wt)
            {
                appliedWeights.Add("Commutability");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Commutability").State = NSCellStateValue.On;
            }

            if (wBIdStateContent.CxWtState.CLAuto.Wt)
            {
                appliedWeights.Add("Commutable Lines - Auto");
                btnAddWeights.Items().FirstOrDefault(x => x.Title == "Commutable Lines - Auto").State = NSCellStateValue.On;
            }
            appliedWeights = appliedWeights.OrderBy(x => x).ToList();
            tblWeights.ReloadData();
        }

        public void ShowCommutableLine()
        {

            CLOWeightNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CLWeightNotification", AddCLOToWeight);
            CommutableViewController ObjCommute = new CommutableViewController();
            ObjCommute.Objtype = CommutableAutoFrom.Weight;
            this.PresentViewControllerAsSheet(ObjCommute);


        }
        public void ShowCommutabilityLine()
        {
            CmtbltyNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CmtbltyWeightNotification", AddCmtbltyToWeights);
            CommutabilityViewController ObjCommute = new CommutabilityViewController();
            ObjCommute.Objtype = CommutabilityEnum.CommutabilityWeight;
            this.PresentViewControllerAsSheet(ObjCommute);


        }

        public void ShowCommutabileAutoView()
        {
            //CmtbltyNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CLOLoadNotification1", ShowLine);
            //modifed by Roshil january 15-2021
            CLOWeightNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CLWeightNotification", AddCLOToWeight);
            CommutableViewController ObjCommute = new CommutableViewController();
            ObjCommute.Objtype = CommutableAutoFrom.Weight;
            this.PresentViewControllerAsSheet(ObjCommute);


        }

        /// <summary>
        /// Add commutability cell and apply block sort logic when the user click done button from the commutability parameter view
        /// </summary>
        /// <param name="n">N.</param>
        private void AddCmtbltyToWeights(NSNotification n)
        {
            //FtCommutableLine ftCom=	(FtCommutableLine)n.Object as FtCommutableLine;


            if (!appliedWeights.Contains("Commutability"))
                appliedWeights.Add("Commutability");
            //ApplyAndReloadWeights("Commutability");
            tblWeights.ReloadData();
            //tblConstraints.ReloadData ();
            NSNotificationCenter.DefaultCenter.RemoveObserver(CmtbltyNotification);


        }


        /// <summary>
        /// Executes when The commutability is applied in either constraint or sort and commutability drpdown in sort is clicked.
        /// </summary>
        private void AddCmtbltyToWeightsCell()
        {
            //FtCommutableLine ftCom=	(FtCommutableLine)n.Object as FtCommutableLine;


            if (!appliedWeights.Contains("Commutability"))
                appliedWeights.Add("Commutability");
            ApplyAndReloadWeights("Commutability");
            //tblConstraints.ReloadData ();



        }
        /// <summary>
        /// Executes when The Commutble line auto weight is applied in either constraint or sort and commutble line auto drpdown in sort is clicked.
        /// </summary>
        private void AddCmtbleAutoToWeightsCell()
        {
            if (!appliedWeights.Contains("Commutable Lines - Auto"))
                appliedWeights.Add("Commutable Lines - Auto");
            ApplyAndReloadWeights("Commutable Lines - Auto");
            tblWeights.ReloadData();
        }
        public void ShowLine(NSNotification n)
        {
            ShowCommutableLine();
        }

        public void AddCmtbltyToContraints(NSNotification n)
        {
            ShowCommutableLine();
        }
        private void AddCLOToWeight(NSNotification n)
        {

            if (!appliedWeights.Contains("Commutable Lines - Auto"))
                appliedWeights.Add("Commutable Lines - Auto");
            ApplyAndReloadWeights("Commutable Lines - Auto");

            //			ApplyAndReloadWeights ("Commutable Lines - Auto");
            tblWeights.ReloadData();
            NSNotificationCenter.DefaultCenter.RemoveObserver(CLOWeightNotification);
        }
        void ClearTapped(object sender, EventArgs e)
        {

            try
            {
                var alert = new NSAlert();
                alert.AlertStyle = NSAlertStyle.Informational;
                alert.MessageText = "WBidMax";
                alert.InformativeText = "Are you sure you want to clear Weights?";
                alert.AddButton("YES");
                alert.AddButton("No");
                alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
                {
                    alert.Window.Close();
                    NSApplication.SharedApplication.StopModal();
                    WBidHelper.PushToUndoStack();
                    weightCalc.ClearWeights();

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

        bool isMultiple(string str)
        {
            switch (str)
            {
                case "AM/PM":
                    return true;
                case "Blocks of Days Off":
                    return true;
                case "Cities-Legs":
                    return true;
                case "Cmut DHs":
                    return true;
                case "DH - first - last":
                    return true;
                case "Duty period":
                    return true;
                case "Equipment Type":
                    return true;
                case "ETOPS":
                    return true;
                case "ETOPS-Res":
                    return true;
                case "Flight Time":
                    return true;
                case "Ground Time":
                    return true;
                case "Intl – NonConus":
                    return true;
                case "Legs Per Duty Period":
                    return true;
                case "Legs Per Pairing":
                    return true;
                case "Number of Days Off":
                    return true;
                case "Overnight Cities":
                    return true;
                case "PDO-after":
                    return true;
                case "PDO-before":
                    return true;
                case "Position":
                    return true;
                case "Rest":
                    return true;
                case "Start Day of Week":
                    return true;
                case "Trip Length":
                    return true;
                case "Work Blk Length":
                    return true;
                case "Work Days":
                    return true;
            }
            return false;
        }

        bool isSpecial(string str)
        {
            switch (str)
            {
                case "Commutable Lines - Manual":
                    return true;
                case "Days of the Month":
                    return true;
                case "Days of the Week":
                    return true;
                case "Overnight Cities - Bulk":
                    return true;
            }
            return false;
        }
        void ShowMessage(string text)
        {
            var alert = new NSAlert();
            alert.AlertStyle = NSAlertStyle.Warning;
            alert.MessageText = "Information";
            alert.InformativeText = text;
            alert.RunModal();
        }
        void btnAddWeightsClicked(object sender, EventArgs e)
        {
            try
            {
                WBidHelper.PushToUndoStack();
                var btn = (NSPopUpButton)sender;
                if (!appliedWeights.Contains(btn.SelectedItem.Title) && !isMultiple(btn.SelectedItem.Title) && !isSpecial(btn.SelectedItem.Title))
                {
                    btnAddWeights.SelectedItem.State = NSCellStateValue.On;

                    if (btn.SelectedItem.Title == "Commutable Lines - Auto")
                    {
                        if (wBIdStateContent.CxWtState.CL.Cx || wBIdStateContent.CxWtState.CL.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("36") || wBIdStateContent.SortDetails.BlokSort.Contains("37") || wBIdStateContent.SortDetails.BlokSort.Contains("38")))
                        {
                            ShowMessage("You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.");
                            btnAddWeights.SelectedItem.State = NSCellStateValue.Off;
                            return;
                        }
                        if (wBIdStateContent.CxWtState.CLAuto.Wt)
                        {
                            return;
                        }
                        if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35"))
                        {
                            AddCmtbleAutoToWeightsCell();
                        }
                        else
                            ShowCommutabileAutoView();

                        return;
                    }
                    else if (btn.SelectedItem.Title == "Commutability")
                    {

                        //if (wBIdStateContent.CxWtState.Commute.Wt)
                        if (wBIdStateContent.CxWtState.Commute.Wt)
                        {
                            return;
                        }
                        if (wBIdStateContent.CxWtState.Commute.Cx || wBIdStateContent.SortDetails.BlokSort.Contains("30") || wBIdStateContent.SortDetails.BlokSort.Contains("31") || wBIdStateContent.SortDetails.BlokSort.Contains("32"))
                        {
                            AddCmtbltyToWeightsCell();
                        }
                        else
                            ShowCommutabilityLine();


                        return;
                    }

                    appliedWeights.Add(btn.SelectedItem.Title);
                    appliedWeights = appliedWeights.OrderBy(x => x).ToList();
                    ApplyAndReloadWeights(btn.SelectedItem.Title);
                    tblWeights.ReloadData();
                }
                else if (isMultiple(btn.SelectedItem.Title))
                {
                    btnAddWeights.SelectedItem.State = NSCellStateValue.On;
                    appliedWeights.Add(btn.SelectedItem.Title);
                    appliedWeights = appliedWeights.OrderBy(x => x).ToList();
                    if (btn.SelectedItem.Title == "AM/PM")
                    {
                        int third = wBIdStateContent.Weights.AM_PM.Type;
                        decimal weight = wBIdStateContent.Weights.AM_PM.Weight;
                        if (wBIdStateContent.Weights.AM_PM.lstParameters == null)
                            wBIdStateContent.Weights.AM_PM.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.AM_PM.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyAMPMWeight(wBIdStateContent.Weights.AM_PM.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Cities-Legs")
                    {
                        if (wBIdStateContent.Weights.CitiesLegs == null)
                        {
                            wBIdStateContent.Weights.CitiesLegs = new Wt2Parameters
                            {
                                Type = 1,
                                Weight = 0,
                                lstParameters = new List<Wt2Parameter>()
                            };
                        }

                        int third = wBIdStateContent.Weights.CitiesLegs.Type;
                        decimal weight = wBIdStateContent.Weights.CitiesLegs.Weight;
                        if (wBIdStateContent.Weights.CitiesLegs.lstParameters == null)
                            wBIdStateContent.Weights.CitiesLegs.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.CitiesLegs.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyCitiesLegsWeight(wBIdStateContent.Weights.CitiesLegs.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "DH - first - last")
                    {
                        int third = wBIdStateContent.Weights.DHDFoL.Type;
                        decimal weight = wBIdStateContent.Weights.DHDFoL.Weight;
                        if (wBIdStateContent.Weights.DHDFoL.lstParameters == null)
                            wBIdStateContent.Weights.DHDFoL.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.DHDFoL.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyDeadheadFisrtLastWeight(wBIdStateContent.Weights.DHDFoL.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Intl – NonConus")
                    {
                        int third = wBIdStateContent.Weights.InterConus.Type;
                        decimal weight = wBIdStateContent.Weights.InterConus.Weight;
                        if (wBIdStateContent.Weights.InterConus.lstParameters == null)
                            wBIdStateContent.Weights.InterConus.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.InterConus.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyInternationalNonConusWeight(wBIdStateContent.Weights.InterConus.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Number of Days Off")
                    {
                        int third = wBIdStateContent.Weights.NODO.Type;
                        decimal weight = wBIdStateContent.Weights.NODO.Weight;
                        if (wBIdStateContent.Weights.NODO.lstParameters == null)
                            wBIdStateContent.Weights.NODO.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.NODO.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyNumberOfDaysOfWeight(wBIdStateContent.Weights.NODO.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Overnight Cities")
                    {
                        int third = wBIdStateContent.Weights.RON.Type;
                        decimal weight = wBIdStateContent.Weights.RON.Weight;
                        if (wBIdStateContent.Weights.RON.lstParameters == null)
                            wBIdStateContent.Weights.RON.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.RON.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyOverNightCitiesWeight(wBIdStateContent.Weights.RON.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Position")
                    {
                        int third = wBIdStateContent.Weights.POS.Type;
                        decimal weight = wBIdStateContent.Weights.POS.Weight;
                        if (wBIdStateContent.Weights.POS.lstParameters == null)
                            wBIdStateContent.Weights.POS.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.POS.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyPositionWeight(wBIdStateContent.Weights.POS.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Start Day of Week")
                    {
                        int third = wBIdStateContent.Weights.SDOW.Type;
                        decimal weight = wBIdStateContent.Weights.SDOW.Weight;
                        if (wBIdStateContent.Weights.SDOW.lstParameters == null)
                            wBIdStateContent.Weights.SDOW.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.SDOW.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyStartDayOfWeekWeight(wBIdStateContent.Weights.SDOW.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Trip Length")
                    {
                        int third = wBIdStateContent.Weights.TL.Type;
                        decimal weight = wBIdStateContent.Weights.TL.Weight;
                        if (wBIdStateContent.Weights.TL.lstParameters == null)
                            wBIdStateContent.Weights.TL.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.TL.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyTripLengthWeight(wBIdStateContent.Weights.TL.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Work Blk Length")
                    {
                        int third = wBIdStateContent.Weights.WB.Type;
                        decimal weight = wBIdStateContent.Weights.WB.Weight;
                        if (wBIdStateContent.Weights.WB.lstParameters == null)
                            wBIdStateContent.Weights.WB.lstParameters = new List<Wt2Parameter>();
                        wBIdStateContent.Weights.WB.lstParameters.Add(new Wt2Parameter
                        {
                            Type = third,
                            Weight = weight
                        });
                        weightCalc.ApplyWorkBlockLengthWeight(wBIdStateContent.Weights.WB.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Aircraft Changes")
                    {
                        int second = wBIdStateContent.Weights.AirCraftChanges.SecondlValue;
                        int third = wBIdStateContent.Weights.AirCraftChanges.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.AirCraftChanges.Weight;
                        weightCalc.ApplyAirCraftChangeWeight(wBIdStateContent.Weights.AirCraftChanges);
                    }
                    else if (btn.SelectedItem.Title == "Blocks of Days Off")
                    {
                        int second = wBIdStateContent.Weights.BDO.SecondlValue;
                        int third = wBIdStateContent.Weights.BDO.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.BDO.Weight;
                        if (wBIdStateContent.Weights.BDO.lstParameters == null)
                            wBIdStateContent.Weights.BDO.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.BDO.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyBlockOFFDaysOfWeight(wBIdStateContent.Weights.BDO.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Cmut DHs")
                    {
                        int second = wBIdStateContent.Weights.DHD.SecondlValue;
                        int third = wBIdStateContent.Weights.DHD.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.DHD.Weight;
                        if (wBIdStateContent.Weights.DHD.lstParameters == null)
                            wBIdStateContent.Weights.DHD.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.DHD.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyCommutableDeadhead(wBIdStateContent.Weights.DHD.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Duty period")
                    {
                        int second = wBIdStateContent.Weights.DP.SecondlValue;
                        int third = wBIdStateContent.Weights.DP.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.DP.Weight;
                        if (wBIdStateContent.Weights.DP.lstParameters == null)
                            wBIdStateContent.Weights.DP.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.DP.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyDutyPeriodWeight(wBIdStateContent.Weights.DP.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Equipment Type")
                    {
                        int second = wBIdStateContent.Weights.EQUIP.SecondlValue;
                        int third = wBIdStateContent.Weights.EQUIP.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.EQUIP.Weight;
                        if (wBIdStateContent.Weights.EQUIP.lstParameters == null)
                            wBIdStateContent.Weights.EQUIP.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.EQUIP.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyEquipmentTypeWeights(wBIdStateContent.Weights.EQUIP.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "ETOPS")
                    {
                        
                        decimal weight = wBIdStateContent.Weights.ETOPS.Weight;
                        if (wBIdStateContent.Weights.ETOPS.lstParameters == null)
                            wBIdStateContent.Weights.ETOPS.lstParameters = new List<Wt1Parameter>();
                        wBIdStateContent.Weights.ETOPS.lstParameters.Add(new Wt1Parameter
                        {
                            
                            Weight = weight
                        });
                        weightCalc.ApplyETOPSWeights(wBIdStateContent.Weights.ETOPS.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "ETOPS-Res")
                    {

                        decimal weight = wBIdStateContent.Weights.ETOPSRes.Weight;
                        if (wBIdStateContent.Weights.ETOPSRes.lstParameters == null)
                            wBIdStateContent.Weights.ETOPSRes.lstParameters = new List<Wt1Parameter>();
                        wBIdStateContent.Weights.ETOPSRes.lstParameters.Add(new Wt1Parameter
                        {

                            Weight = weight
                        });
                        weightCalc.ApplyETOPSResWeights(wBIdStateContent.Weights.ETOPSRes.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Flight Time")
                    {
                        int second = wBIdStateContent.Weights.FLTMIN.SecondlValue;
                        int third = wBIdStateContent.Weights.FLTMIN.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.FLTMIN.Weight;
                        if (wBIdStateContent.Weights.FLTMIN.lstParameters == null)
                            wBIdStateContent.Weights.FLTMIN.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.FLTMIN.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyFlightTimeWeights(wBIdStateContent.Weights.FLTMIN.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Ground Time")
                    {
                        int second = wBIdStateContent.Weights.GRD.SecondlValue;
                        int third = wBIdStateContent.Weights.GRD.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.GRD.Weight;
                        if (wBIdStateContent.Weights.GRD.lstParameters == null)
                            wBIdStateContent.Weights.GRD.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.GRD.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyGroundTimeWeight(wBIdStateContent.Weights.GRD.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Legs Per Duty Period")
                    {
                        int second = wBIdStateContent.Weights.LEGS.SecondlValue;
                        int third = wBIdStateContent.Weights.LEGS.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.LEGS.Weight;
                        if (wBIdStateContent.Weights.LEGS.lstParameters == null)
                            wBIdStateContent.Weights.LEGS.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.LEGS.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyLegsPerDutyPeriodWeight(wBIdStateContent.Weights.LEGS.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Legs Per Pairing")
                    {
                        int second = wBIdStateContent.Weights.WtLegsPerPairing.SecondlValue;
                        int third = wBIdStateContent.Weights.WtLegsPerPairing.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.WtLegsPerPairing.Weight;
                        if (wBIdStateContent.Weights.WtLegsPerPairing.lstParameters == null)
                            wBIdStateContent.Weights.WtLegsPerPairing.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.WtLegsPerPairing.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyLegsPerPairingWeight(wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Work Days")
                    {
                        int second = wBIdStateContent.Weights.WorkDays.SecondlValue;
                        int third = wBIdStateContent.Weights.WorkDays.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.WorkDays.Weight;
                        if (wBIdStateContent.Weights.WorkDays.lstParameters == null)
                            wBIdStateContent.Weights.WorkDays.lstParameters = new List<Wt3Parameter>();
                        wBIdStateContent.Weights.WorkDays.lstParameters.Add(new Wt3Parameter
                        {
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyWorkDaysWeight(wBIdStateContent.Weights.WorkDays.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "PDO-after")
                    {
                        int first = wBIdStateContent.Weights.PDAfter.FirstValue;
                        int second = wBIdStateContent.Weights.PDAfter.SecondlValue;
                        int third = wBIdStateContent.Weights.PDAfter.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.PDAfter.Weight;
                        if (wBIdStateContent.Weights.PDAfter.lstParameters == null)
                            wBIdStateContent.Weights.PDAfter.lstParameters = new List<Wt4Parameter>();
                        wBIdStateContent.Weights.PDAfter.lstParameters.Add(new Wt4Parameter
                        {
                            FirstValue = first,
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyPartialDaysAfterWeight(wBIdStateContent.Weights.PDAfter.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "PDO-before")
                    {
                        int first = wBIdStateContent.Weights.PDBefore.FirstValue;
                        int second = wBIdStateContent.Weights.PDBefore.SecondlValue;
                        int third = wBIdStateContent.Weights.PDBefore.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.PDBefore.Weight;
                        if (wBIdStateContent.Weights.PDBefore.lstParameters == null)
                            wBIdStateContent.Weights.PDBefore.lstParameters = new List<Wt4Parameter>();
                        wBIdStateContent.Weights.PDBefore.lstParameters.Add(new Wt4Parameter
                        {
                            FirstValue = first,
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyPartialDaysBeforeWeight(wBIdStateContent.Weights.PDBefore.lstParameters);
                    }
                    else if (btn.SelectedItem.Title == "Rest")
                    {
                        int first = wBIdStateContent.Weights.WtRest.FirstValue;
                        int second = wBIdStateContent.Weights.WtRest.SecondlValue;
                        int third = wBIdStateContent.Weights.WtRest.ThrirdCellValue;
                        decimal weight = wBIdStateContent.Weights.WtRest.Weight;
                        if (wBIdStateContent.Weights.WtRest.lstParameters == null)
                            wBIdStateContent.Weights.WtRest.lstParameters = new List<Wt4Parameter>();
                        wBIdStateContent.Weights.WtRest.lstParameters.Add(new Wt4Parameter
                        {
                            FirstValue = first,
                            SecondlValue = second,
                            ThrirdCellValue = third,
                            Weight = weight
                        });
                        weightCalc.ApplyRestWeight(wBIdStateContent.Weights.WtRest.lstParameters);



                    }
                    ApplyAndReloadWeights(btn.SelectedItem.Title);
                    tblWeights.ReloadData();
                }
                else if (isSpecial(btn.SelectedItem.Title))
                {
                    if (!appliedWeights.Contains(btn.SelectedItem.Title))
                    {
                        if (btn.SelectedItem.Title == "Commutable Lines - Manual")
                        {
                            if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
                            {
                                ShowMessage("You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.");
                                return;
                            }
                        }
                        btnAddWeights.SelectedItem.State = NSCellStateValue.On;
                        appliedWeights.Add(btn.SelectedItem.Title);
                        appliedWeights = appliedWeights.OrderBy(x => x).ToList();
                        ApplyAndReloadWeights(btn.SelectedItem.Title);
                        tblWeights.ReloadData();
                    }
                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        public void ApplyAndReloadWeights(string weight)
        {
            try
            {

                if (weight == "Largest Block of Days Off")
                {
                    wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt = true;
                    weightCalc.ApplyLargestBlockOfDaysoffWeight(wBIdStateContent.Weights.LrgBlkDayOff);
                }
                if (weight == "Normalize Days Off")
                {
                    wBIdStateContent.CxWtState.NormalizeDays.Wt = true;
                    weightCalc.ApplyNormalizeDaysOffWeight(wBIdStateContent.Weights.NormalizeDaysOff);
                }
                if (weight == "AM/PM")
                {
                    wBIdStateContent.CxWtState.AMPM.Wt = true;
                    weightCalc.ApplyAMPMWeight(wBIdStateContent.Weights.AM_PM.lstParameters);
                }
                if (weight == "Cities-Legs")
                {
                    wBIdStateContent.CxWtState.CitiesLegs.Wt = true;
                    weightCalc.ApplyCitiesLegsWeight(wBIdStateContent.Weights.CitiesLegs.lstParameters);
                }
                if (weight == "DH - first - last")
                {
                    wBIdStateContent.CxWtState.DHDFoL.Wt = true;
                    weightCalc.ApplyDeadheadFisrtLastWeight(wBIdStateContent.Weights.DHDFoL.lstParameters);
                }
                if (weight == "Intl – NonConus")
                {
                    wBIdStateContent.CxWtState.InterConus.Wt = true;
                    weightCalc.ApplyInternationalNonConusWeight(wBIdStateContent.Weights.InterConus.lstParameters);
                }
                if (weight == "Number of Days Off")
                {
                    wBIdStateContent.CxWtState.NODO.Wt = true;
                    weightCalc.ApplyNumberOfDaysOfWeight(wBIdStateContent.Weights.NODO.lstParameters);
                }
                if (weight == "Overnight Cities")
                {
                    wBIdStateContent.CxWtState.RON.Wt = true;
                    weightCalc.ApplyOverNightCitiesWeight(wBIdStateContent.Weights.RON.lstParameters);
                }
                if (weight == "Position")
                {
                    wBIdStateContent.CxWtState.Position.Wt = true;
                    weightCalc.ApplyPositionWeight(wBIdStateContent.Weights.POS.lstParameters);
                }
                if (weight == "Start Day of Week")
                {
                    wBIdStateContent.CxWtState.SDOW.Wt = true;
                    weightCalc.ApplyStartDayOfWeekWeight(wBIdStateContent.Weights.SDOW.lstParameters);
                }
                if (weight == "Time-Away-From-Base")
                {
                    wBIdStateContent.CxWtState.PerDiem.Wt = true;
                    weightCalc.ApplyTimeAwayFromBaseWeight(wBIdStateContent.Weights.PerDiem);
                }
                if (weight == "Trip Length")
                {
                    wBIdStateContent.CxWtState.TL.Wt = true;
                    weightCalc.ApplyTripLengthWeight(wBIdStateContent.Weights.TL.lstParameters);
                }
                if (weight == "Work Blk Length")
                {
                    wBIdStateContent.CxWtState.WB.Wt = true;
                    weightCalc.ApplyWorkBlockLengthWeight(wBIdStateContent.Weights.WB.lstParameters);
                }
                if (weight == "Aircraft Changes")
                {
                    wBIdStateContent.CxWtState.ACChg.Wt = true;
                    weightCalc.ApplyAirCraftChangeWeight(wBIdStateContent.Weights.AirCraftChanges);
                }
                if (weight == "Blocks of Days Off")
                {
                    wBIdStateContent.CxWtState.BDO.Wt = true;
                    weightCalc.ApplyBlockOFFDaysOfWeight(wBIdStateContent.Weights.BDO.lstParameters);
                }
                if (weight == "Cmut DHs")
                {
                    wBIdStateContent.CxWtState.DHD.Wt = true;
                    weightCalc.ApplyCommutableDeadhead(wBIdStateContent.Weights.DHD.lstParameters);
                }
                if (weight == "Duty period")
                {
                    wBIdStateContent.CxWtState.DP.Wt = true;
                    weightCalc.ApplyDutyPeriodWeight(wBIdStateContent.Weights.DP.lstParameters);
                }
                if (weight == "Equipment Type")
                {
                    wBIdStateContent.CxWtState.EQUIP.Wt = true;
                    weightCalc.ApplyEquipmentTypeWeights(wBIdStateContent.Weights.EQUIP.lstParameters);
                }
                if (weight == "ETOPS")
                {
                    wBIdStateContent.CxWtState.ETOPS.Wt = true;
                    weightCalc.ApplyETOPSWeights(wBIdStateContent.Weights.ETOPS.lstParameters);
                }
                if (weight == "ETOPS-Res")
                {
                    wBIdStateContent.CxWtState.ETOPSRes.Wt = true;
                    weightCalc.ApplyETOPSResWeights(wBIdStateContent.Weights.ETOPSRes.lstParameters);
                }
                if (weight == "Flight Time")
                {
                    wBIdStateContent.CxWtState.FLTMIN.Wt = true;
                    weightCalc.ApplyFlightTimeWeights(wBIdStateContent.Weights.FLTMIN.lstParameters);
                }
                if (weight == "Ground Time")
                {
                    wBIdStateContent.CxWtState.GRD.Wt = true;
                    weightCalc.ApplyGroundTimeWeight(wBIdStateContent.Weights.GRD.lstParameters);
                }
                if (weight == "Legs Per Duty Period")
                {
                    wBIdStateContent.CxWtState.LEGS.Wt = true;
                    weightCalc.ApplyLegsPerDutyPeriodWeight(wBIdStateContent.Weights.LEGS.lstParameters);
                }
                if (weight == "Legs Per Pairing")
                {
                    wBIdStateContent.CxWtState.LegsPerPairing.Wt = true;
                    weightCalc.ApplyLegsPerPairingWeight(wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
                }
                if (weight == "Work Days")
                {
                    wBIdStateContent.CxWtState.WorkDay.Wt = true;
                    weightCalc.ApplyWorkDaysWeight(wBIdStateContent.Weights.WorkDays.lstParameters);
                }
                if (weight == "PDO-after")
                {
                    wBIdStateContent.CxWtState.PDAfter.Wt = true;
                    weightCalc.ApplyPartialDaysAfterWeight(wBIdStateContent.Weights.PDAfter.lstParameters);
                }
                if (weight == "PDO-before")
                {
                    wBIdStateContent.CxWtState.PDBefore.Wt = true;
                    weightCalc.ApplyPartialDaysBeforeWeight(wBIdStateContent.Weights.PDBefore.lstParameters);
                }
                if (weight == "Rest")
                {
                    wBIdStateContent.CxWtState.Rest.Wt = true;
                    weightCalc.ApplyRestWeight(wBIdStateContent.Weights.WtRest.lstParameters);
                }
                if (weight == "Days of the Week")
                {
                    wBIdStateContent.CxWtState.DOW.Wt = true;
                    weightCalc.ApplyDaysOfWeekWeight(wBIdStateContent.Weights.DOW);
                    tblWeights.ReloadData();
                }
                if (weight == "Days of the Month")
                {
                    wBIdStateContent.CxWtState.SDO.Wt = true;
                    weightCalc.ApplyDaysOfMonthWeight(wBIdStateContent.Weights.SDO);
                    tblWeights.ReloadData();
                }
                if (weight == "Overnight Cities - Bulk")
                {
                    wBIdStateContent.CxWtState.BulkOC.Wt = true;
                    weightCalc.ApplyOvernightCityBulkWeight(wBIdStateContent.Weights.OvernightCitybulk);
                    tblWeights.ReloadData();
                }
                if (weight == "Commutable Lines - Manual")
                {
                    wBIdStateContent.CxWtState.CL.Wt = true;
                    weightCalc.ApplyCommutableLine(wBIdStateContent.Weights.CL);
                    //tblWeights.ReloadData ();
                }
                if (weight == "Commutable Lines - Auto")
                {
                    wBIdStateContent.CxWtState.CLAuto.Wt = true;
                    weightCalc.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                    if (tblWeights != null)
                    {
                        tblWeights.ReloadData();
                    }
                }
                if (weight == "Commutability")
                {
                    wBIdStateContent.CxWtState.Commute.Wt = true;
                    weightCalc.ApplyCommuttabilityWeight(wBIdStateContent.Weights.Commute);
                    if (tblWeights != null)
                    {
                        tblWeights.ReloadData();
                    }
                }
                if (weight == "ETOPS")
                {
                    wBIdStateContent.CxWtState.ETOPS.Wt = true;
                    weightCalc.ApplyETOPSWeights(wBIdStateContent.Weights.ETOPS.lstParameters);
                    if (tblWeights != null)
                    {
                        tblWeights.ReloadData();
                    }
                }
                if (weight == "ETOPS-Res")
                {
                    wBIdStateContent.CxWtState.ETOPSRes.Wt = true;
                    weightCalc.ApplyETOPSResWeights(wBIdStateContent.Weights.ETOPSRes.lstParameters);
                    if (tblWeights != null)
                    {
                        tblWeights.ReloadData();
                    }
                }
                CommonClass.MainController.ReloadAllContent();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }


        public void ApplyAndReloadWeightsFromCommutability(string weight)
        {
            try
            {


                if (weight == "Commutable Lines - Manual")
                {
                    wBIdStateContent.CxWtState.CL.Wt = true;
                    weightCalc.ApplyCommutableLine(wBIdStateContent.Weights.CL);
                    //tblWeights.ReloadData ();
                }
                if (weight == "Commutable Lines - Auto")
                {
                    wBIdStateContent.CxWtState.CLAuto.Wt = true;
                    weightCalc.ApplyCommutableLineAuto(wBIdStateContent.Weights.CLAuto);
                }
                if (weight == "Commutability")
                {
                    wBIdStateContent.CxWtState.Commute.Wt = true;
                    weightCalc.ApplyCommuttabilityWeight(wBIdStateContent.Weights.Commute);
                    //	tblWeights.ReloadData();
                }
                CommonClass.MainController.ReloadAllContent();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        public void RemoveAndReloadWeights(string weight, int order)
        {
            try
            {
                WBidHelper.PushToUndoStack();
                if (weight == "Largest Block of Days Off")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.LrgBlkDaysOff.Wt = false;
                    weightCalc.RemoveLargestBlockDaysWeight();
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Normalize Days Off")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.NormalizeDays.Wt = false;
                    weightCalc.RemoveNormalizeDaysOffWeight();
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "AM/PM")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.AMPM.Wt = false;
                    }
                    wBIdStateContent.Weights.AM_PM.lstParameters.RemoveAt(order);
                    weightCalc.ApplyAMPMWeight(wBIdStateContent.Weights.AM_PM.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Cities-Legs")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.CitiesLegs.Wt = false;
                    }
                    wBIdStateContent.Weights.CitiesLegs.lstParameters.RemoveAt(order);
                    weightCalc.ApplyCitiesLegsWeight(wBIdStateContent.Weights.CitiesLegs.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "DH - first - last")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.DHDFoL.Wt = false;
                    }
                    wBIdStateContent.Weights.DHDFoL.lstParameters.RemoveAt(order);
                    weightCalc.ApplyDeadheadFisrtLastWeight(wBIdStateContent.Weights.DHDFoL.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Intl – NonConus")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.InterConus.Wt = false;
                    }
                    wBIdStateContent.Weights.InterConus.lstParameters.RemoveAt(order);
                    weightCalc.ApplyInternationalNonConusWeight(wBIdStateContent.Weights.InterConus.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Number of Days Off")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.NODO.Wt = false;
                    }
                    wBIdStateContent.Weights.NODO.lstParameters.RemoveAt(order);
                    weightCalc.ApplyNumberOfDaysOfWeight(wBIdStateContent.Weights.NODO.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Overnight Cities")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.RON.Wt = false;
                    }
                    wBIdStateContent.Weights.RON.lstParameters.RemoveAt(order);
                    weightCalc.ApplyOverNightCitiesWeight(wBIdStateContent.Weights.RON.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Position")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.Position.Wt = false;
                    }
                    wBIdStateContent.Weights.POS.lstParameters.RemoveAt(order);
                    weightCalc.ApplyPositionWeight(wBIdStateContent.Weights.POS.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Start Day of Week")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.SDOW.Wt = false;
                    }
                    wBIdStateContent.Weights.SDOW.lstParameters.RemoveAt(order);
                    weightCalc.ApplyStartDayOfWeekWeight(wBIdStateContent.Weights.SDOW.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Time-Away-From-Base")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.PerDiem.Wt = false;
                    weightCalc.RemoveTimeAwayFromBaseWeight();
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Trip Length")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.TL.Wt = false;
                    }
                    wBIdStateContent.Weights.TL.lstParameters.RemoveAt(order);
                    weightCalc.ApplyTripLengthWeight(wBIdStateContent.Weights.TL.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Work Blk Length")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.WB.Wt = false;
                    }
                    wBIdStateContent.Weights.WB.lstParameters.RemoveAt(order);
                    weightCalc.ApplyWorkBlockLengthWeight(wBIdStateContent.Weights.WB.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Aircraft Changes")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.ACChg.Wt = false;
                    weightCalc.RemoveAirCraftChangesWeight();
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Blocks of Days Off")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.BDO.Wt = false;
                    }
                    wBIdStateContent.Weights.BDO.lstParameters.RemoveAt(order);
                    weightCalc.ApplyBlockOFFDaysOfWeight(wBIdStateContent.Weights.BDO.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Cmut DHs")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.DHD.Wt = false;
                    }
                    wBIdStateContent.Weights.DHD.lstParameters.RemoveAt(order);
                    weightCalc.ApplyCommutableDeadhead(wBIdStateContent.Weights.DHD.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Duty period")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.DP.Wt = false;
                    }
                    wBIdStateContent.Weights.DP.lstParameters.RemoveAt(order);
                    weightCalc.ApplyDutyPeriodWeight(wBIdStateContent.Weights.DP.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Equipment Type")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.EQUIP.Wt = false;
                    }
                    wBIdStateContent.Weights.EQUIP.lstParameters.RemoveAt(order);
                    weightCalc.ApplyEquipmentTypeWeights(wBIdStateContent.Weights.EQUIP.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "ETOPS")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.ETOPS.Wt = false;
                    }
                    wBIdStateContent.Weights.ETOPS.lstParameters.RemoveAt(order);
                    weightCalc.ApplyETOPSWeights(wBIdStateContent.Weights.ETOPS.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "ETOPS-Res")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.ETOPSRes.Wt = false;
                    }
                    wBIdStateContent.Weights.ETOPSRes.lstParameters.RemoveAt(order);
                    weightCalc.ApplyETOPSResWeights(wBIdStateContent.Weights.ETOPSRes.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Flight Time")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.FLTMIN.Wt = false;
                    }
                    wBIdStateContent.Weights.FLTMIN.lstParameters.RemoveAt(order);
                    weightCalc.ApplyFlightTimeWeights(wBIdStateContent.Weights.FLTMIN.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Ground Time")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.GRD.Wt = false;
                    }
                    wBIdStateContent.Weights.GRD.lstParameters.RemoveAt(order);
                    weightCalc.ApplyGroundTimeWeight(wBIdStateContent.Weights.GRD.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Legs Per Duty Period")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.LEGS.Wt = false;
                    }
                    wBIdStateContent.Weights.LEGS.lstParameters.RemoveAt(order);
                    weightCalc.ApplyLegsPerDutyPeriodWeight(wBIdStateContent.Weights.LEGS.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Legs Per Pairing")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.LegsPerPairing.Wt = false;
                    }
                    wBIdStateContent.Weights.WtLegsPerPairing.lstParameters.RemoveAt(order);
                    weightCalc.ApplyLegsPerPairingWeight(wBIdStateContent.Weights.WtLegsPerPairing.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Work Days")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.WorkDay.Wt = false;
                    }
                    wBIdStateContent.Weights.WorkDays.lstParameters.RemoveAt(order);
                    weightCalc.ApplyWorkDaysWeight(wBIdStateContent.Weights.WorkDays.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "PDO-after")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.PDAfter.Wt = false;
                    }
                    wBIdStateContent.Weights.PDAfter.lstParameters.RemoveAt(order);
                    weightCalc.ApplyPartialDaysAfterWeight(wBIdStateContent.Weights.PDAfter.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "PDO-before")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.PDBefore.Wt = false;
                    }
                    wBIdStateContent.Weights.PDBefore.lstParameters.RemoveAt(order);
                    weightCalc.ApplyPartialDaysBeforeWeight(wBIdStateContent.Weights.PDBefore.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Rest")
                {
                    if (appliedWeights.Count(x => x == weight) == 1)
                    {
                        btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.Rest.Wt = false;
                    }
                    wBIdStateContent.Weights.WtRest.lstParameters.RemoveAt(order);
                    weightCalc.ApplyRestWeight(wBIdStateContent.Weights.WtRest.lstParameters);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Days of the Week")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.DOW.Wt = false;
                    wBIdStateContent.Weights.DOW.lstWeight.Clear();
                    weightCalc.ApplyDaysOfWeekWeight(wBIdStateContent.Weights.DOW);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Days of the Month")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.SDO.Wt = false;
                    wBIdStateContent.Weights.SDO.Weights.Clear();
                    weightCalc.ApplyDaysOfMonthWeight(wBIdStateContent.Weights.SDO);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Overnight Cities - Bulk")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.BulkOC.Wt = false;
                    wBIdStateContent.Weights.OvernightCitybulk.Clear();
                    weightCalc.ApplyOvernightCityBulkWeight(wBIdStateContent.Weights.OvernightCitybulk);
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Commutable Lines - Manual")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.CL.Wt = false;
                    weightCalc.RemoveCommutableLineWeight();
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Commutable Lines - Auto")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.CLAuto.Wt = false;
                    weightCalc.RemoveCommutableLineAutoWeight();
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }
                else if (weight == "Commutability")
                {
                    btnAddWeights.Items().FirstOrDefault(x => x.Title == weight).State = NSCellStateValue.Off;
                    wBIdStateContent.CxWtState.Commute.Wt = false;
                    weightCalc.RemoveCommutabilityWeight();
                    appliedWeights.Remove(weight);
                    tblWeights.ReloadData();
                }

                CommonClass.MainController.ReloadAllContent();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

    }

    public partial class WeightsTableSource : NSTableViewSource
    {
        WeightsViewController parentVC;
        public WeightsTableSource(WeightsViewController parent)
        {
            parentVC = parent;
        }

        public override nint GetRowCount(NSTableView tableView)
        {
            return parentVC.appliedWeights.Count;
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            var vw = (WeightsCell)tableView.MakeView(WeightsApplied.ViewTypes[parentVC.appliedWeights[(int)row]], this);
            vw.BindData(parentVC.appliedWeights[(int)row], (int)row);
            return vw;
        }
        public override nfloat GetRowHeight(NSTableView tableView, nint row)
        {
            if (WeightsApplied.ViewTypes[parentVC.appliedWeights[(int)row]] == "DOW")
                return 90;
            else if (WeightsApplied.ViewTypes[parentVC.appliedWeights[(int)row]] == "DOM")
                return 320;
            else if (WeightsApplied.ViewTypes[parentVC.appliedWeights[(int)row]] == "OBLK")
                return 415;
            else if (WeightsApplied.ViewTypes[parentVC.appliedWeights[(int)row]] == "CL")
                return 250;
            else
                return 30;
        }
        public override bool SelectionShouldChange(NSTableView tableView)
        {
            return false;
        }

    }

}

