
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
	public partial class ConstraintsViewController : AppKit.NSViewController
	{
		#region Constructors

		public List<string> appliedConstraints = new List<string> ();
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		ConstraintCalculations constCalc = new ConstraintCalculations ();
		NSObject CLONotification;
        NSObject CmtbltyNotification;
		NSObject CLOLoadNotification;
        NSObject CmtbltyLoadNotification;

		// Called when created from unmanaged code
		public ConstraintsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ConstraintsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ConstraintsViewController () : base ("ConstraintsView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ConstraintsView View {
			get {
				return (ConstraintsView)base.View;
			}
		}
		public void ShowCommuteDetails(NSNotification n)
		{
			ShowCommutableLine ();
		}
        public void ShowCommutabilityDetails (NSNotification n)
        {
            ShowCommutabilityLine ();
        }
		public void ShowCommutableLine()
		{

			CLONotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"CLONotification", AddCLOToContraints);
			CommutableViewController ObjCommute = new CommutableViewController ();
			ObjCommute.Objtype = CommutableAutoFrom.Constraints;
			this.PresentViewControllerAsSheet (ObjCommute);


		}
        public void ShowCommutabilityLine ()
        {
            //          ArrivalAndDepartViewController ObjArrival= new ArrivalAndDepartViewController();
            //          this.PresentViewControllerAsSheet(ObjArrival);
            //          return;
            CmtbltyNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"CmtbltyNotification", AddCmtbltyToContraints);
            CommutabilityViewController ObjCommute = new CommutabilityViewController ();
            ObjCommute.Objtype = CommutabilityEnum.CommutabilityConstraint;
            this.PresentViewControllerAsSheet (ObjCommute);

        }
		/// <summary>
		/// Add commutability cell and apply block sort logic when the user click done button from the commutability parameter view
		/// </summary>
		/// <param name="n">N.</param>
		/// 
		private void AddCmtbltyToContraints (NSNotification n)
		{
			//FtCommutableLine ftCom=	(FtCommutableLine)n.Object as FtCommutableLine;


			if(!appliedConstraints.Contains("Commutability"))
			appliedConstraints.Add ("Commutability");
			//ApplyAndReloadConstraints ("Commutability");
			tblConstraints.ReloadData ();
			if (CmtbltyNotification != null)
			NSNotificationCenter.DefaultCenter.RemoveObserver (CmtbltyNotification);

			
		}

		 
		/// <summary>
		/// Executes when The commutability is applied in either weight or sort and commutability drpdown in sort is clicked.
		/// </summary>
		private void AddCmtbltyToContraintsCell()
		{
			//FtCommutableLine ftCom=	(FtCommutableLine)n.Object as FtCommutableLine;


			if (!appliedConstraints.Contains("Commutability"))
				appliedConstraints.Add("Commutability");
			ApplyAndReloadConstraints("Commutability");
			//tblConstraints.ReloadData ();



		}
        /// <summary>
        /// Executes when The commutble line auto is applied in either weight or sort and commutabile lien auto drpdown in sort is clicked.
        /// </summary>
        private void AddCLOToContraintsCell()
        {
            if (!appliedConstraints.Contains("Commutable Lines - Auto"))
                appliedConstraints.Add("Commutable Lines - Auto");
            ApplyAndReloadConstraints("Commutable Lines - Auto");
        }
        private void AddCLOToContraints (NSNotification n)
        {
            //FtCommutableLine ftCom=   (FtCommutableLine)n.Object as FtCommutableLine;


            if (!appliedConstraints.Contains ("Commutable Lines - Auto"))
                appliedConstraints.Add ("Commutable Lines - Auto");
            ApplyAndReloadConstraints ("Commutable Lines - Auto");
            //tblConstraints.ReloadData ();
			if (CLONotification != null)
            NSNotificationCenter.DefaultCenter.RemoveObserver (CLONotification);


        }
		public override void ViewDidDisappear ()
		{
			base.ViewDidDisappear ();


		}

		public void RemoveNotifications()
		{
				if (CLOLoadNotification != null)
        NSNotificationCenter.DefaultCenter.RemoveObserver (CLOLoadNotification);
			if (CmtbltyLoadNotification != null)
			{
				
				NSNotificationCenter.DefaultCenter.RemoveObserver(CmtbltyLoadNotification);
				CmtbltyLoadNotification = null;
			}
		}
		public override void ViewDidAppear()
		{
			base.ViewDidAppear();
			CLOLoadNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CLOLoadNotification", ShowCommuteDetails);
			if (CmtbltyLoadNotification == null)
			CmtbltyLoadNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"CmtbltyLoadNotification", ShowCommutabilityDetails);
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();

				ConstraintsApplied.MainList = ConstraintsApplied.MainList.OrderBy (x => x).ToList ();
				if (ConstraintsApplied.MainList [0] == "3-on-3-off") {
					ConstraintsApplied.MainList.Remove ("3-on-3-off");
					ConstraintsApplied.MainList.Add ("3-on-3-off");
				}

				if (GlobalSettings.CurrentBidDetails.Postion != "FA") {
					btnA.Hidden = btnB.Hidden = btnC.Hidden = btnD.Hidden = true;
					btnBlank.Title = "Blank";
				} else {
					btnA.Hidden = btnB.Hidden = btnC.Hidden = btnD.Hidden = false;
					btnBlank.Title = "Ready";

				}

				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M") {
					btnBlank.Enabled = false;
					btnReserve.Enabled = false;
					btnBlank.BezelStyle = NSBezelStyle.SmallSquare;
					btnReserve.BezelStyle = NSBezelStyle.SmallSquare;
				} else {
					btnBlank.Enabled = true;
					btnReserve.Enabled = true;
					btnBlank.BezelStyle = NSBezelStyle.TexturedSquare;
					btnReserve.BezelStyle = NSBezelStyle.TexturedSquare;
				}
               
				setValuesToFixedContraints ();
				SetupButtons ();
				btnAddConstraints.RemoveAllItems ();
				btnAddConstraints.AddItem ("Additional Constraints");
				btnAddConstraints.AddItems (ConstraintsApplied.MainList.ToArray ());
				btnAddConstraints.Activated += btnAddConstraintsClicked;
				tblConstraints.Source = new ConstraintsTableSource (this);
				btnClear.Activated += ClearTapped;


				LoadAdditionalConstraints ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void LoadAdditionalConstraints ()
		{
			if (wBIdStateContent.CxWtState.No3on3off.Cx) {
				appliedConstraints.Add ("3-on-3-off");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "3-on-3-off").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.ACChg.Cx) {
				appliedConstraints.Add ("Aircraft Changes");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Aircraft Changes").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.BDO.Cx) {
				appliedConstraints.Add ("Blocks of Days Off");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Blocks of Days Off").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.DP.Cx) {
				appliedConstraints.Add ("Duty period");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Duty period").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.FLTMIN.Cx) {
				appliedConstraints.Add ("Flight Time");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Flight Time").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.LEGS.Cx) {
				appliedConstraints.Add ("Legs Per Duty Period");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Legs Per Duty Period").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.LegsPerPairing.Cx) {
				appliedConstraints.Add ("Legs Per Pairing");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Legs Per Pairing").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.MP.Cx) {
				appliedConstraints.Add ("Min Pay");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Min Pay").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.NODO.Cx) {
				appliedConstraints.Add ("Number of Days Off");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Number of Days Off").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.PerDiem.Cx) {
				appliedConstraints.Add ("Time-Away-From-Base");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Time-Away-From-Base").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.WorkDay.Cx) {
				appliedConstraints.Add ("Work Days");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Work Days").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.InterConus.Cx) {
				foreach (var item in wBIdStateContent.Constraints.InterConus.lstParameters) {
					appliedConstraints.Add ("Intl – NonConus");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Intl – NonConus").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.DOW.Cx) {
				foreach (var item in wBIdStateContent.Constraints.DaysOfWeek.lstParameters) {
					appliedConstraints.Add ("Days of the Week");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Days of the Week").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.DHDFoL.Cx) {
				foreach (var item in wBIdStateContent.Constraints.DeadHeadFoL.lstParameters) {
					appliedConstraints.Add ("DH - first - last");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "DH - first - last").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.EQUIP.Cx) {
				foreach (var item in wBIdStateContent.Constraints.EQUIP.lstParameters) {
					appliedConstraints.Add ("Equipment Type");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Equipment Type").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.GRD.Cx) {
				appliedConstraints.Add ("Ground Time");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Ground Time").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.RON.Cx) {
				foreach (var item in wBIdStateContent.Constraints.OverNightCities.lstParameters) {
					appliedConstraints.Add ("Overnight Cities");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Overnight Cities").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.CitiesLegs.Cx) {
				foreach (var item in wBIdStateContent.Constraints.CitiesLegs.lstParameters) {
					appliedConstraints.Add ("Cities-Legs");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Cities-Legs").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.Rest.Cx) {
				foreach (var item in wBIdStateContent.Constraints.Rest.lstParameters) {
					appliedConstraints.Add ("Rest");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Rest").State = NSCellStateValue.On;
			}
            if (wBIdStateContent.CxWtState.StartDay.Cx)
            {
                foreach (var item in wBIdStateContent.Constraints.StartDay.lstParameters)
                {
                    appliedConstraints.Add("Start Day");
                }
                btnAddConstraints.Items().FirstOrDefault(x => x.Title == "Start Day").State = NSCellStateValue.On;
            }
            if (wBIdStateContent.CxWtState.ReportRelease.Cx)
            {
                foreach (var item in wBIdStateContent.Constraints.ReportRelease.lstParameters)
                {
                    appliedConstraints.Add("Report-Release");
                }
                btnAddConstraints.Items().FirstOrDefault(x => x.Title == "Report-Release").State = NSCellStateValue.On;
            }
			if (wBIdStateContent.CxWtState.SDOW.Cx) {
				foreach (var item in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters) {
					appliedConstraints.Add ("Start Day of Week");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Start Day of Week").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.TL.Cx) {
				foreach (var item in wBIdStateContent.Constraints.TripLength.lstParameters) {
					appliedConstraints.Add ("Trip Length");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Trip Length").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.WB.Cx) {
				foreach (var item in wBIdStateContent.Constraints.WorkBlockLength.lstParameters) {
					appliedConstraints.Add ("Work Blk Length");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Work Blk Length").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.DHD.Cx) {
				foreach (var item in wBIdStateContent.Constraints.DeadHeads.LstParameter) {
					appliedConstraints.Add ("Cmut DHs");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Cmut DHs").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.WtPDOFS.Cx) {
				foreach (var item in wBIdStateContent.Constraints.PDOFS.LstParameter) {
					appliedConstraints.Add ("PDO");
				}
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "PDO").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.SDO.Cx) {
				appliedConstraints.Add ("Days of the Month");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Days of the Month").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.BulkOC.Cx) {
				appliedConstraints.Add ("Overnight Cities - Bulk");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Overnight Cities - Bulk").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.CL.Cx) {
				appliedConstraints.Add ("Commutable Lines - Manual");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Commutable Lines - Manual").State = NSCellStateValue.On;
			}
			if (wBIdStateContent.CxWtState.CLAuto.Cx) {
				appliedConstraints.Add ("Commutable Lines - Auto");
				btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Commutable Lines - Auto").State = NSCellStateValue.On;
			}
            if (wBIdStateContent.CxWtState.Commute.Cx) {
                appliedConstraints.Add ("Commutability");
                btnAddConstraints.Items ().FirstOrDefault (x => x.Title == "Commutability").State = NSCellStateValue.On;
            }
			appliedConstraints = appliedConstraints.OrderBy (x => x).ToList ();
			if (appliedConstraints.Contains ("3-on-3-off") && appliedConstraints [0] == "3-on-3-off") {
				appliedConstraints.Remove ("3-on-3-off");
				appliedConstraints.Add ("3-on-3-off");
			}
			if (wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx)
			{
				appliedConstraints.Add("Mixed Hard/Reserve");
				btnAddConstraints.Items().FirstOrDefault(x => x.Title == "Mixed Hard/Reserve").State = NSCellStateValue.On;
			}
			tblConstraints.ReloadData ();
		}

		void ClearTapped (object sender, EventArgs e)
		{
			try {
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Are you sure you want to clear Constraints?";
				alert.AddButton ("YES");
				alert.AddButton ("No");
				alert.Buttons [0].Activated += (object sender1, EventArgs ex) => {
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
					WBidHelper.PushToUndoStack ();
					constCalc.ClearConstraints ();
					wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					CommonClass.MainController.ReloadAllContent ();
					CommonClass.CSWController.ReloadAllContent ();
				};
				alert.RunModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		void SetupButtons ()
		{
			btnHelpConst.Activated += (object sender, EventArgs e) => {
				if (CommonClass.HelpController == null) {
					var help = new HelpWindowController ();
					CommonClass.HelpController = help;
				}
				CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
				CommonClass.HelpController.tblDocument.SelectRow (1, false);
				CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (0));
			};


//			btnHard.Cell = new NSButtonCell{ BackgroundColor = NSColor.Green };
//			btnHard.Title ="Hard";

			//btnHard.Cell.BackgroundColor = NSColor.Green;

			btnHard.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.Constraints.Hard = (btnHard.State == NSCellStateValue.On);
				constCalc.HardConstraintCalclculation (wBIdStateContent.Constraints.Hard);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();

				if (wBIdStateContent.Constraints.Hard) ButtonActiveLayout (btnHard);
 				else ButtonInActiveLayout (btnHard);

			};

			btnReserve.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.Constraints.Reserve = (btnReserve.State == NSCellStateValue.On);
				constCalc.ReserveConstraintCalculation (wBIdStateContent.Constraints.Reserve);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
				if (wBIdStateContent.Constraints.Reserve) {
					
					ButtonActiveLayout(btnReserve);
				
				} else {
					ButtonInActiveLayout(btnReserve);
				
				}
			};

			btnBlank.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				if (btnBlank.Title == "Blank") {
					wBIdStateContent.Constraints.Blank = (btnBlank.State == NSCellStateValue.On);
					constCalc.BlankConstraintCalculation (wBIdStateContent.Constraints.Blank);
					if (wBIdStateContent.Constraints.Blank)ButtonActiveLayout (btnBlank);
					else ButtonInActiveLayout (btnBlank);
						

				} else {
					wBIdStateContent.Constraints.Ready = (btnBlank.State == NSCellStateValue.On);
					constCalc.ReadyReserveConstraintCalculation (wBIdStateContent.Constraints.Ready);
					if (wBIdStateContent.Constraints.Ready )ButtonActiveLayout (btnBlank);
					else ButtonInActiveLayout (btnBlank);

				}
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
			};

			btnInternational.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.Constraints.International = (btnInternational.State == NSCellStateValue.On);
				constCalc.InternationalConstraintCalculation (wBIdStateContent.Constraints.International);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();

				if (wBIdStateContent.Constraints.International) 
					ButtonActiveLayout (btnInternational);
				 else 
					ButtonInActiveLayout (btnInternational);

			};

			btnNonConus.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.Constraints.NonConus = (btnNonConus.State == NSCellStateValue.On);
				constCalc.Non_ConusConstraintClaculation (wBIdStateContent.Constraints.NonConus);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
				if (wBIdStateContent.Constraints.NonConus)
					ButtonActiveLayout (btnNonConus);
					else 
					ButtonInActiveLayout (btnNonConus);
					
			};

            btnEtops.Activated += (object sender, EventArgs e) => {

                if (btnEtops.State == NSCellStateValue.Off && GlobalSettings.CurrentBidDetails.Postion != "FA" && ChecktheUserIsInEBGGroup() == false)
                {
                    var alert = new NSAlert()
                    {
                        AlertStyle = NSAlertStyle.Informational,
                        InformativeText = "You are NOT in the ETOPS Bid Group.  You should not bid the ETOPS lines as you will not be awarded any ETOPS line.",
                        MessageText = "Alert !",
                    };
                    alert.AddButton("Leave ON");
                    alert.AddButton("Turn OFF");

                    alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
                    {
                        btnEtops.State = NSCellStateValue.On;
                        alert.Window.Close();
                        NSApplication.SharedApplication.EndSheet(alert.Window);

                    };
                    alert.Buttons[1].Activated += (object sender1, EventArgs ex) =>
                    {
                        ApplyETOPSConstraints();
                        alert.Window.Close();
                        NSApplication.SharedApplication.EndSheet(alert.Window);

                    };
                    var result = alert.RunModal();
                }
                else
                {
                    ApplyETOPSConstraints();
                }
            };
            btnEtopsRes.Activated += (object sender, EventArgs e) => {
                if (btnEtopsRes.State == NSCellStateValue.Off && GlobalSettings.CurrentBidDetails.Postion != "FA" && ChecktheUserIsInEBGGroup() == false)
                {
                    var alert = new NSAlert()
                    {
                        AlertStyle = NSAlertStyle.Informational,
                        InformativeText = "You are NOT in the ETOPS Bid Group.  You should not bid the ETOPS lines as you will not be awarded any ETOPS line.",
                        MessageText = "Alert !",
                    };
                    alert.AddButton("Leave ON");
                    alert.AddButton("Turn OFF");

                    alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
                    {
                        btnEtopsRes.State = NSCellStateValue.On;
                        alert.Window.Close();
                        NSApplication.SharedApplication.EndSheet(alert.Window);

                    };
                    alert.Buttons[1].Activated += (object sender1, EventArgs ex) =>
                    {
                        ApplyETOPSReserveConstrain();
                        alert.Window.Close();
                        NSApplication.SharedApplication.EndSheet(alert.Window);

                    };
                    var result = alert.RunModal(); 
                }
                else
                {
                    ApplyETOPSReserveConstrain();
                }
            };
			btnAMs.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.CxWtState.AMPMMIX.AM = (btnAMs.State == NSCellStateValue.On);
				constCalc.AMPMMixConstraint ("AM", wBIdStateContent.CxWtState.AMPMMIX.AM);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
				if (wBIdStateContent.CxWtState.AMPMMIX.AM) ButtonActiveLayout (btnAMs);
				else ButtonInActiveLayout (btnAMs);
					
			};

			btnPMs.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.CxWtState.AMPMMIX.PM = (btnPMs.State == NSCellStateValue.On);
				constCalc.AMPMMixConstraint (" PM", wBIdStateContent.CxWtState.AMPMMIX.PM);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();

				if (wBIdStateContent.CxWtState.AMPMMIX.PM)ButtonActiveLayout (btnPMs);
				else ButtonInActiveLayout (btnPMs);
				
			};

			btnMix.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.CxWtState.AMPMMIX.MIX = (btnMix.State == NSCellStateValue.On);
				constCalc.AMPMMixConstraint ("Mix", wBIdStateContent.CxWtState.AMPMMIX.MIX);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
				if (wBIdStateContent.CxWtState.AMPMMIX.MIX) ButtonActiveLayout (btnMix);
				else ButtonInActiveLayout (btnMix);

			};

			btnA.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.CxWtState.FaPosition.A = (btnA.State == NSCellStateValue.On);
				var faposition = wBIdStateContent.CxWtState.FaPosition;
				constCalc.ABCDPositionsConstraint (faposition.A, faposition.B, faposition.C, faposition.D);
				//constCalc.ABCDPositionsConstraint ("A", wBIdStateContent.CxWtState.FaPosition.A);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();

				if (wBIdStateContent.CxWtState.FaPosition.A) ButtonActiveLayout (btnA);
				 else ButtonInActiveLayout (btnA);
					
			};

			btnB.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.CxWtState.FaPosition.B = (btnB.State == NSCellStateValue.On);
				var faposition = wBIdStateContent.CxWtState.FaPosition;
				constCalc.ABCDPositionsConstraint (faposition.A, faposition.B, faposition.C, faposition.D);
				//constCalc.ABCDPositionsConstraint ("B", wBIdStateContent.CxWtState.FaPosition.B);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();

				if (wBIdStateContent.CxWtState.FaPosition.B)ButtonActiveLayout (btnB);
				 else ButtonInActiveLayout (btnB);
			};

			btnC.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.CxWtState.FaPosition.C = (btnC.State == NSCellStateValue.On);
				var faposition = wBIdStateContent.CxWtState.FaPosition;
				constCalc.ABCDPositionsConstraint (faposition.A, faposition.B, faposition.C, faposition.D);
				//constCalc.ABCDPositionsConstraint ("C", wBIdStateContent.CxWtState.FaPosition.C);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();

				if (wBIdStateContent.CxWtState.FaPosition.C) ButtonActiveLayout (btnC);
				 else ButtonInActiveLayout (btnC);
					
			};

			btnD.Activated += (object sender, EventArgs e) => {
				WBidHelper.PushToUndoStack ();
				wBIdStateContent.CxWtState.FaPosition.D = (btnD.State == NSCellStateValue.On);
				//constCalc.ABCDPositionsConstraint ("D", wBIdStateContent.CxWtState.FaPosition.D);
				var faposition = wBIdStateContent.CxWtState.FaPosition;
				constCalc.ABCDPositionsConstraint (faposition.A, faposition.B, faposition.C, faposition.D);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
				if (wBIdStateContent.CxWtState.FaPosition.D)ButtonActiveLayout (btnD);
				else ButtonInActiveLayout (btnD);
					

			};

			btnTurns.Activated += HandleTripLengthConstraint;
			btn2Days.Activated += HandleTripLengthConstraint;
			btn3Days.Activated += HandleTripLengthConstraint;
			btn4Days.Activated += HandleTripLengthConstraint;

			btnMon.Activated += HandleWeekdayConstraint;
			btnTue.Activated += HandleWeekdayConstraint;
			btnWed.Activated += HandleWeekdayConstraint;
			btnThu.Activated += HandleWeekdayConstraint;
			btnFri.Activated += HandleWeekdayConstraint;
			btnSat.Activated += HandleWeekdayConstraint;
			btnSun.Activated += HandleWeekdayConstraint;

		}

        private void ApplyETOPSConstraints()
        {
            WBidHelper.PushToUndoStack();
            wBIdStateContent.Constraints.ETOPS = (btnEtops.State == NSCellStateValue.On);
            constCalc.ETOPSConstraintClaculation(wBIdStateContent.Constraints.ETOPS);
            CommonClass.MainController.ReloadAllContent();
            lblLineCount.StringValue = constCalc.LinesNotConstrained();
            if (wBIdStateContent.Constraints.ETOPS)
                ButtonActiveLayout(btnEtops);
            else
                ButtonInActiveLayout(btnEtops);
        }

        private void ApplyETOPSReserveConstrain()
        {
            WBidHelper.PushToUndoStack();
            wBIdStateContent.Constraints.ReserveETOPS = (btnEtopsRes.State == NSCellStateValue.On);
            constCalc.ETOPSResConstraintClaculation(wBIdStateContent.Constraints.ReserveETOPS);
            CommonClass.MainController.ReloadAllContent();
            lblLineCount.StringValue = constCalc.LinesNotConstrained();
            if (wBIdStateContent.Constraints.ReserveETOPS)
                ButtonActiveLayout(btnEtopsRes);
            else
                ButtonInActiveLayout(btnEtopsRes);
        }

        private bool ChecktheUserIsInEBGGroup()
        {
			return (GlobalSettings.WBidStateCollection.SeniorityListItem != null && GlobalSettings.WBidStateCollection.SeniorityListItem.EBgType == "Y") ? true : false;

			//return (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.EBG == "Y") ? true : false;
        }
		void HandleWeekdayConstraint (object sender, EventArgs e)
		{
			try {
				WBidHelper.PushToUndoStack ();
				List <int> days = new List<int> ();
				
				if (btnMon.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnMon);
					wBIdStateContent.CxWtState.DaysOfWeek.MON = true;
					days.Add ((int)btnMon.Tag);
				} else
				{
					ButtonInActiveLayout(btnMon);
					wBIdStateContent.CxWtState.DaysOfWeek.MON = false;
				}
				if (btnTue.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnTue);
					wBIdStateContent.CxWtState.DaysOfWeek.TUE = true;
					days.Add ((int)btnTue.Tag);
				} else
				{
					ButtonInActiveLayout(btnTue);
					wBIdStateContent.CxWtState.DaysOfWeek.TUE = false;
				}
				if (btnWed.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnWed);
					wBIdStateContent.CxWtState.DaysOfWeek.WED = true;
					days.Add ((int)btnWed.Tag);
				} else
				{
					ButtonInActiveLayout(btnWed);
					wBIdStateContent.CxWtState.DaysOfWeek.WED = false;
				}
				if (btnThu.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnThu);
					wBIdStateContent.CxWtState.DaysOfWeek.THU = true;
					days.Add ((int)btnThu.Tag);
				} else
				{
					ButtonInActiveLayout(btnThu);
					wBIdStateContent.CxWtState.DaysOfWeek.THU = false;
				}
				if (btnFri.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnFri);
					wBIdStateContent.CxWtState.DaysOfWeek.FRI = true;
					days.Add ((int)btnFri.Tag);
				} else
				{
					ButtonInActiveLayout(btnFri);
					wBIdStateContent.CxWtState.DaysOfWeek.FRI = false;
				}
				if (btnSat.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnSat);
					wBIdStateContent.CxWtState.DaysOfWeek.SAT = true;
					days.Add ((int)btnSat.Tag);
				} else
				{
					ButtonInActiveLayout(btnSat);
					wBIdStateContent.CxWtState.DaysOfWeek.SAT = false;
				}
				if (btnSun.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnSun);
					wBIdStateContent.CxWtState.DaysOfWeek.SUN = true;
					days.Add ((int)btnSun.Tag);
				} else
				{
					ButtonInActiveLayout(btnSun);
					wBIdStateContent.CxWtState.DaysOfWeek.SUN = false;
				}
				
				constCalc.ApplyWeekDayConstraint (days);
				CommonClass.MainController.ReloadAllContent ();	
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleTripLengthConstraint (object sender, EventArgs e)
		{
			try {
				WBidHelper.PushToUndoStack ();
				string str = string.Empty;
				
				if (btnTurns.State == NSCellStateValue.On) {
					ButtonActiveLayout(btnTurns);
					wBIdStateContent.CxWtState.TripLength.Turns = true;
					str += btnTurns.Tag.ToString ();
				} else
				{
					ButtonInActiveLayout(btnTurns);
					wBIdStateContent.CxWtState.TripLength.Turns = false;
				}
				if (btn2Days.State == NSCellStateValue.On) {
					ButtonActiveLayout(btn2Days);
					wBIdStateContent.CxWtState.TripLength.Twoday = true;
					if (str != string.Empty)
						str += ",";
					str += btn2Days.Tag.ToString ();
				} else
				{
					ButtonInActiveLayout(btn2Days);
					wBIdStateContent.CxWtState.TripLength.Twoday = false;
				}
				if (btn3Days.State == NSCellStateValue.On) {
					ButtonActiveLayout(btn3Days);
					wBIdStateContent.CxWtState.TripLength.ThreeDay = true;
					if (str != string.Empty)
						str += ",";
					str += btn3Days.Tag.ToString ();
				} else
				{
					ButtonInActiveLayout(btn3Days);
					wBIdStateContent.CxWtState.TripLength.ThreeDay = false;
				}
				if (btn4Days.State == NSCellStateValue.On) {
					ButtonActiveLayout(btn4Days);
					wBIdStateContent.CxWtState.TripLength.FourDay = true;
					if (str != string.Empty)
						str += ",";
					str += btn4Days.Tag.ToString ();
				} else
				{
					ButtonInActiveLayout(btn4Days);
					wBIdStateContent.CxWtState.TripLength.FourDay = false;
				}
				
				constCalc.ApplyTripLengthConstraint (str);
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void setValuesToFixedContraints ()
		{
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			if (wBIdStateContent.Constraints.Hard) {
				this.btnHard.State = NSCellStateValue.On;
				ButtonActiveLayout (btnHard);

			} else {
				btnHard.State = NSCellStateValue.Off;
				ButtonInActiveLayout (btnHard);
			}

			if (wBIdStateContent.Constraints.Reserve) {
				ButtonActiveLayout (btnReserve);
				this.btnReserve.State = NSCellStateValue.On;

			} else {
				ButtonInActiveLayout (btnReserve);
				btnReserve.State = NSCellStateValue.Off;

			}

			if (wBIdStateContent.Constraints.Ready || wBIdStateContent.Constraints.Blank) {
				ButtonActiveLayout (btnBlank);
				this.btnBlank.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnBlank);
				btnBlank.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.Constraints.International) {
				ButtonActiveLayout (btnInternational);
				this.btnInternational.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnInternational);
				btnInternational.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.Constraints.NonConus) {
				ButtonActiveLayout (btnNonConus);
				this.btnNonConus.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnNonConus);
				btnNonConus.State = NSCellStateValue.Off;
			}

            if (wBIdStateContent.Constraints.ETOPS)
            {
                ButtonActiveLayout(btnEtops);
                this.btnEtops.State = NSCellStateValue.On;
            }
            else
            {
                ButtonInActiveLayout(btnEtops);
                btnEtops.State = NSCellStateValue.Off;
            }
            if (wBIdStateContent.Constraints.ReserveETOPS)
            {
                ButtonActiveLayout(btnEtopsRes);
                this.btnEtopsRes.State = NSCellStateValue.On;
            }
            else
            {
                ButtonInActiveLayout(btnEtopsRes);
                btnEtopsRes.State = NSCellStateValue.Off;
            }

			if (wBIdStateContent.CxWtState.AMPMMIX.AM) {
				ButtonActiveLayout (btnAMs);
				this.btnAMs.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnAMs);
				btnAMs.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.AMPMMIX.PM) {
				ButtonActiveLayout (btnPMs);
				this.btnPMs.State = NSCellStateValue.On;
			} else {
				btnPMs.State = NSCellStateValue.Off;
				ButtonInActiveLayout (btnPMs);
			}

			if (wBIdStateContent.CxWtState.AMPMMIX.MIX) {
				ButtonActiveLayout (btnMix);
				this.btnMix.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnMix);
				btnMix.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.FaPosition.A) {
				ButtonActiveLayout (btnA);
				this.btnA.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnA);
				this.btnA.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.FaPosition.B) {
				ButtonActiveLayout (btnB);
				this.btnB.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnB);
				this.btnB.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.FaPosition.C) {
				ButtonActiveLayout (btnC);
				this.btnC.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnC);
				this.btnC.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.FaPosition.D) {
				ButtonActiveLayout (btnD);
				this.btnD.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnD);
				this.btnD.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.TripLength.Turns) {
				ButtonActiveLayout (btnTurns);
				this.btnTurns.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnTurns);
				this.btnTurns.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.TripLength.Twoday) {
				ButtonActiveLayout (btn2Days);
				this.btn2Days.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btn2Days);
				this.btn2Days.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.TripLength.ThreeDay) 
			{
				ButtonActiveLayout (btn3Days);
				this.btn3Days.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btn3Days);
				this.btn3Days.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.TripLength.FourDay) {
				ButtonActiveLayout (btn4Days);
				this.btn4Days.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btn4Days);
				this.btn4Days.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.DaysOfWeek.SUN) {
				ButtonActiveLayout (btnSun);
				this.btnSun.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnSun);
				this.btnSun.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.DaysOfWeek.MON) {
				ButtonActiveLayout (btnMon);
				this.btnMon.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnMon);
				this.btnMon.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.DaysOfWeek.TUE) {
				ButtonActiveLayout (btnTue);
				this.btnTue.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnTue);
				this.btnTue.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.DaysOfWeek.WED) {
				ButtonActiveLayout (btnWed);
				this.btnWed.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnWed);
				this.btnWed.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.DaysOfWeek.THU) {
				ButtonActiveLayout (btnThu);
				this.btnThu.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnThu);
				this.btnThu.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.DaysOfWeek.FRI) {
				ButtonActiveLayout (btnFri);
				this.btnFri.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnFri);
				this.btnFri.State = NSCellStateValue.Off;
			}

			if (wBIdStateContent.CxWtState.DaysOfWeek.SAT) {
				ButtonActiveLayout (btnSat);
				this.btnSat.State = NSCellStateValue.On;
			} else {
				ButtonInActiveLayout (btnSat);
				this.btnSat.State = NSCellStateValue.Off;
			}

			lblLineCount.StringValue = constCalc.LinesNotConstrained ();
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
        bool isMultiple(string str)
        {
            switch (str)
            {
                case "Intl – NonConus":
                    return true;
                case "Cities-Legs":
                    return true;
                case "Days of the Week":
                    return true;
                case "DH - first - last":
                    return true;
                case "Equipment Type":
                    return true;
                case "Overnight Cities":
                    return true;
                case "Rest":
                    return true;
                case "Report-Release":
                    return true;
                case "Start Day":
                    return true;
                case "Start Day of Week":
                    return true;
                case "Trip Length":
                    return true;
                case "Work Blk Length":
                    return true;
                case "Cmut DHs":
                    return true;
                case "PDO":
                    return true;
                    //			case "Commutable Lines - Auto":
                    //				return true;
            }
            return false;
        }

		bool isSpecial (string str)
		{
			switch (str) {
			case "Commutable Lines - Manual":
				return true;
			case "Days of the Month":
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
        void btnAddConstraintsClicked(object sender, EventArgs e)
        {
            try
            {
                WBidHelper.PushToUndoStack();
                var btn = (NSPopUpButton)sender;
                if (!appliedConstraints.Contains(btn.SelectedItem.Title) && !isMultiple(btn.SelectedItem.Title) && !isSpecial(btn.SelectedItem.Title))
                {


                    if (btn.SelectedItem.Title == "Commutable Lines - Auto")
                    {
                        if (wBIdStateContent.CxWtState.CL.Cx || wBIdStateContent.CxWtState.CL.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("36") || wBIdStateContent.SortDetails.BlokSort.Contains("37") || wBIdStateContent.SortDetails.BlokSort.Contains("38")))
                        {
                            ShowMessage("You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.");
                            return;
                        } 

                        if (wBIdStateContent.CxWtState.CLAuto.Cx)
                        {
                            return;
                        }
                        if (wBIdStateContent.CxWtState.CLAuto.Wt || wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35"))
                        {
                            AddCLOToContraintsCell();
                        }
                        else
                            ShowCommutableLine();
                        return;
                    }
                    else if (btn.SelectedItem.Title == "Commutability")
                    {

                        //if (wBIdStateContent.CxWtState.Commute.Cx)
                        if (wBIdStateContent.CxWtState.Commute.Cx)
                        {
                            return;
                        }
                        if (wBIdStateContent.CxWtState.Commute.Wt || wBIdStateContent.SortDetails.BlokSort.Contains("30") || wBIdStateContent.SortDetails.BlokSort.Contains("31") || wBIdStateContent.SortDetails.BlokSort.Contains("32"))
                        {
                            AddCmtbltyToContraintsCell();
                        }
                        else
                            ShowCommutabilityLine();

                        return;
                    }
                    btnAddConstraints.SelectedItem.State = NSCellStateValue.On;
                    appliedConstraints.Add(btn.SelectedItem.Title);
                    appliedConstraints = appliedConstraints.OrderBy(x => x).ToList();
                    if (appliedConstraints[0] == "3-on-3-off")
                    {
                        appliedConstraints.Remove("3-on-3-off");
                        appliedConstraints.Add("3-on-3-off");
                    }
                    ApplyAndReloadConstraints(btn.SelectedItem.Title);
                    tblConstraints.ReloadData();
                }
                else if (isMultiple(btn.SelectedItem.Title))
                {
                    if (btn.SelectedItem.Title == "Intl – NonConus")
                    {
                        if (GlobalSettings.WBidINIContent.Cities.Count(x => x.International == true) != 0 || GlobalSettings.WBidINIContent.Cities.Count(x => x.NonConus == true) != 0)
                        {
                            btnAddConstraints.SelectedItem.State = NSCellStateValue.On;
                            appliedConstraints.Add(btn.SelectedItem.Title);
                            appliedConstraints = appliedConstraints.OrderBy(x => x).ToList();
                            if (appliedConstraints[0] == "3-on-3-off")
                            {
                                appliedConstraints.Remove("3-on-3-off");
                                appliedConstraints.Add("3-on-3-off");
                            }
                            if (wBIdStateContent.Constraints.InterConus.lstParameters == null)
                                wBIdStateContent.Constraints.InterConus.lstParameters = new List<Cx2Parameter>();
                            wBIdStateContent.Constraints.InterConus.lstParameters.Add(new Cx2Parameter
                            {
                                Type = wBIdStateContent.Constraints.InterConus.Type,
                                Value = wBIdStateContent.Constraints.InterConus.Value
                            });
                            ApplyAndReloadConstraints(btn.SelectedItem.Title);
                            tblConstraints.ReloadData();
                        }
                    }
                    else
                    {
                        btnAddConstraints.SelectedItem.State = NSCellStateValue.On;
                        appliedConstraints.Add(btn.SelectedItem.Title);
                        appliedConstraints = appliedConstraints.OrderBy(x => x).ToList();
                        if (appliedConstraints[0] == "3-on-3-off")
                        {
                            appliedConstraints.Remove("3-on-3-off");
                            appliedConstraints.Add("3-on-3-off");
                        }
                        if (btn.SelectedItem.Title == "Days of the Week")
                        {
                            if (wBIdStateContent.Constraints.DaysOfWeek.lstParameters == null)
                                wBIdStateContent.Constraints.DaysOfWeek.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.DaysOfWeek.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.DaysOfWeek.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.DaysOfWeek.Type,
                                Value = wBIdStateContent.Constraints.DaysOfWeek.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "DH - first - last")
                        {
                            if (wBIdStateContent.Constraints.DeadHeadFoL.lstParameters == null)
                                wBIdStateContent.Constraints.DeadHeadFoL.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.DeadHeadFoL.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.DeadHeadFoL.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.DeadHeadFoL.Type,
                                Value = wBIdStateContent.Constraints.DeadHeadFoL.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Equipment Type")
                        {
                            if (wBIdStateContent.Constraints.EQUIP.lstParameters == null)
                                wBIdStateContent.Constraints.EQUIP.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.EQUIP.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.EQUIP.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.EQUIP.Type,
                                Value = wBIdStateContent.Constraints.EQUIP.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Overnight Cities")
                        {
                            if (wBIdStateContent.Constraints.OverNightCities.lstParameters == null)
                                wBIdStateContent.Constraints.OverNightCities.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.OverNightCities.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.OverNightCities.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.OverNightCities.Type,
                                Value = wBIdStateContent.Constraints.OverNightCities.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Cities-Legs")
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
                            if (wBIdStateContent.Constraints.CitiesLegs.lstParameters == null)
                                wBIdStateContent.Constraints.CitiesLegs.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.CitiesLegs.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.CitiesLegs.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.CitiesLegs.Type,
                                Value = wBIdStateContent.Constraints.CitiesLegs.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Rest")
                        {
                            if (wBIdStateContent.Constraints.Rest.lstParameters == null)
                                wBIdStateContent.Constraints.Rest.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.Rest.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.Rest.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.Rest.Type,
                                Value = wBIdStateContent.Constraints.Rest.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Start Day")
                        {
                            if (wBIdStateContent.Constraints.StartDay.lstParameters == null)
                                wBIdStateContent.Constraints.StartDay.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.StartDay.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.StartDay.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.StartDay.Type,
                                Value = wBIdStateContent.Constraints.StartDay.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Report-Release")
                        {
                            if (wBIdStateContent.Constraints.ReportRelease.lstParameters == null)
                                wBIdStateContent.Constraints.ReportRelease.lstParameters = new List<ReportRelease>();
                            wBIdStateContent.Constraints.ReportRelease.lstParameters.Add(new ReportRelease
                            {
                                AllDays = wBIdStateContent.Constraints.ReportRelease.AllDays,
                                First = wBIdStateContent.Constraints.ReportRelease.First,
                                Last = wBIdStateContent.Constraints.ReportRelease.Last,
                                NoMid = wBIdStateContent.Constraints.ReportRelease.NoMid,
                                Report = wBIdStateContent.Constraints.ReportRelease.Report,
                                Release = wBIdStateContent.Constraints.ReportRelease.Release,

                            });
                        }
                        else if (btn.SelectedItem.Title == "Start Day of Week")
                        {
                            if (wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters == null)
                                wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters.Add(new Cx3Parameter
                            {
                                SecondcellValue = wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue,
                                ThirdcellValue = wBIdStateContent.Constraints.StartDayOftheWeek.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.StartDayOftheWeek.Type,
                                Value = wBIdStateContent.Constraints.StartDayOftheWeek.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Trip Length")
                        {
                            if (wBIdStateContent.Constraints.TripLength.lstParameters == null)
                                wBIdStateContent.Constraints.TripLength.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.TripLength.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.TripLength.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.TripLength.Type,
                                Value = wBIdStateContent.Constraints.TripLength.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Work Blk Length")
                        {
                            if (wBIdStateContent.Constraints.WorkBlockLength.lstParameters == null)
                                wBIdStateContent.Constraints.WorkBlockLength.lstParameters = new List<Cx3Parameter>();
                            wBIdStateContent.Constraints.WorkBlockLength.lstParameters.Add(new Cx3Parameter
                            {
                                ThirdcellValue = wBIdStateContent.Constraints.WorkBlockLength.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.WorkBlockLength.Type,
                                Value = wBIdStateContent.Constraints.WorkBlockLength.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "Cmut DHs")
                        {
                            if (wBIdStateContent.Constraints.DeadHeads.LstParameter == null)
                                wBIdStateContent.Constraints.DeadHeads.LstParameter = new List<Cx4Parameter>();
                            wBIdStateContent.Constraints.DeadHeads.LstParameter.Add(new Cx4Parameter
                            {
                                SecondcellValue = wBIdStateContent.Constraints.DeadHeads.SecondcellValue,
                                ThirdcellValue = wBIdStateContent.Constraints.DeadHeads.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.DeadHeads.Type,
                                Value = wBIdStateContent.Constraints.DeadHeads.Value
                            });
                        }
                        else if (btn.SelectedItem.Title == "PDO")
                        {
                            if (wBIdStateContent.Constraints.PDOFS.LstParameter == null)
                                wBIdStateContent.Constraints.PDOFS.LstParameter = new List<Cx4Parameter>();
                            wBIdStateContent.Constraints.PDOFS.LstParameter.Add(new Cx4Parameter
                            {
                                SecondcellValue = wBIdStateContent.Constraints.PDOFS.SecondcellValue,
                                ThirdcellValue = wBIdStateContent.Constraints.PDOFS.ThirdcellValue,
                                Type = wBIdStateContent.Constraints.PDOFS.Type,
                                Value = wBIdStateContent.Constraints.PDOFS.Value
                            });
                        }

                        ApplyAndReloadConstraints(btn.SelectedItem.Title);
                        tblConstraints.ReloadData();
                    }
                }
                else if (isSpecial(btn.SelectedItem.Title))
                {
                    if (!appliedConstraints.Contains(btn.SelectedItem.Title))
                    {
                        if (btn.SelectedItem.Title == "Commutable Lines - Manual")
                        {
                            if (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35")))
                            {
                                ShowMessage("You can only add a Commutable Line - Auto or a Commutable Line - Manual constraint or weight or sort, but NOT both.");
                                return;
                            }
                        }
                        btnAddConstraints.SelectedItem.State = NSCellStateValue.On;
                        appliedConstraints.Add(btn.SelectedItem.Title);
                        appliedConstraints = appliedConstraints.OrderBy(x => x).ToList();
                        if (appliedConstraints[0] == "3-on-3-off")
                        {
                            appliedConstraints.Remove("3-on-3-off");
                            appliedConstraints.Add("3-on-3-off");
                        }
                        ApplyAndReloadConstraints(btn.SelectedItem.Title);
                        tblConstraints.ReloadData();
                    }
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

		public void ApplyAndReloadConstraints (string constraint)
		{
			try {

				//wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				//WBidHelper.PushToUndoStack ();
				if (constraint == "3-on-3-off")
				{
					wBIdStateContent.CxWtState.No3on3off.Cx = true;
					constCalc.ApplyThreeOn3offConstraint(wBIdStateContent.Constraints.No3On3Off);
				}
				else if (constraint == "Aircraft Changes")
				{
					wBIdStateContent.CxWtState.ACChg.Cx = true;
					constCalc.ApplyAirCraftChangesConstraint(wBIdStateContent.Constraints.AircraftChanges);
				}
				else if (constraint == "Blocks of Days Off")
				{
					wBIdStateContent.CxWtState.BDO.Cx = true;
					constCalc.ApplyBlockOfDaysOffConstraint(wBIdStateContent.Constraints.BlockOfDaysOff);
				}
				else if (constraint == "Duty period")
				{
					wBIdStateContent.CxWtState.DP.Cx = true;
					constCalc.ApplyDutyPeriodConstraint(wBIdStateContent.Constraints.DutyPeriod);
				}
				else if (constraint == "Flight Time")
				{
					wBIdStateContent.CxWtState.FLTMIN.Cx = true;
					constCalc.ApplyFlightTimeConstraint(wBIdStateContent.Constraints.FlightMin);
				}
				else if (constraint == "Legs Per Duty Period")
				{
					wBIdStateContent.CxWtState.LEGS.Cx = true;
					constCalc.ApplyLegsPerDutyPeriodConstraint(wBIdStateContent.Constraints.LegsPerDutyPeriod);
				}
				else if (constraint == "Legs Per Pairing")
				{
					wBIdStateContent.CxWtState.LegsPerPairing.Cx = true;
					constCalc.ApplyLegsPerPairingConstraint(wBIdStateContent.Constraints.LegsPerPairing);
				}
				else if (constraint == "Min Pay")
				{
					wBIdStateContent.CxWtState.MP.Cx = true;
					constCalc.ApplyMinimumPayConstraint(wBIdStateContent.Constraints.MinimumPay);
				}
				else if (constraint == "Number of Days Off")
				{
					wBIdStateContent.CxWtState.NODO.Cx = true;
					constCalc.ApplyNumberofDaysOffConstraint(wBIdStateContent.Constraints.NumberOfDaysOff);
				}
				else if (constraint == "Time-Away-From-Base")
				{
					wBIdStateContent.CxWtState.PerDiem.Cx = true;
					constCalc.ApplyTimeAwayFromBaseConstraint(wBIdStateContent.Constraints.PerDiem);
				}
				else if (constraint == "Work Days")
				{
					wBIdStateContent.CxWtState.WorkDay.Cx = true;
					constCalc.ApplyWorkDaysConstraint(wBIdStateContent.Constraints.WorkDay);
				}
				else if (constraint == "Intl – NonConus")
				{
					wBIdStateContent.CxWtState.InterConus.Cx = true;
					constCalc.ApplyInternationalonConusConstraint(wBIdStateContent.Constraints.InterConus.lstParameters);
					tblConstraints.ReloadData();
				}
				else if (constraint == "Days of the Week")
				{
					wBIdStateContent.CxWtState.DOW.Cx = true;
					constCalc.ApplyDaysofWeekConstraint(wBIdStateContent.Constraints.DaysOfWeek.lstParameters);
				}
				else if (constraint == "DH - first - last")
				{
					wBIdStateContent.CxWtState.DHDFoL.Cx = true;
					constCalc.ApplyDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeadFoL.lstParameters);
				}
				else if (constraint == "Equipment Type")
				{
					wBIdStateContent.CxWtState.EQUIP.Cx = true;
					constCalc.ApplyEquipmentTypeConstraint(wBIdStateContent.Constraints.EQUIP.lstParameters);
				}
				else if (constraint == "Ground Time")
				{
					wBIdStateContent.CxWtState.GRD.Cx = true;
					constCalc.ApplyGroundTimeConstraint(wBIdStateContent.Constraints.GroundTime);
				}
				else if (constraint == "Overnight Cities")
				{
					wBIdStateContent.CxWtState.RON.Cx = true;
					constCalc.ApplyOvernightCitiesConstraint(wBIdStateContent.Constraints.OverNightCities.lstParameters);
				}
				else if (constraint == "Cities-Legs")
				{
					wBIdStateContent.CxWtState.CitiesLegs.Cx = true;
					constCalc.ApplyCitiesLegsConstraint(wBIdStateContent.Constraints.CitiesLegs.lstParameters);
				}
				else if (constraint == "Rest")
				{
					wBIdStateContent.CxWtState.Rest.Cx = true;
					constCalc.ApplyRestConstraint(wBIdStateContent.Constraints.Rest.lstParameters);
				}
				else if (constraint == "Start Day")
				{
					wBIdStateContent.CxWtState.StartDay.Cx = true;
					constCalc.ApplyStartDayConstraint(wBIdStateContent.Constraints.StartDay.lstParameters);
				}
				else if (constraint == "Report-Release")
				{
					wBIdStateContent.CxWtState.ReportRelease.Cx = true;
					constCalc.ApplyReportReleaseConstraint(wBIdStateContent.Constraints.ReportRelease.lstParameters);
				}
				else if (constraint == "Start Day of Week")
				{
					wBIdStateContent.CxWtState.SDOW.Cx = true;
					constCalc.ApplyStartDayOfWeekConstraint(wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
				}
				else if (constraint == "Trip Length")
				{
					wBIdStateContent.CxWtState.TL.Cx = true;
					constCalc.ApplyTripLengthConstraint(wBIdStateContent.Constraints.TripLength.lstParameters);
				}
				else if (constraint == "Work Blk Length")
				{
					wBIdStateContent.CxWtState.WB.Cx = true;
					constCalc.ApplyWorkBlockLengthConstraint(wBIdStateContent.Constraints.WorkBlockLength.lstParameters);
				}
				else if (constraint == "Cmut DHs")
				{
					wBIdStateContent.CxWtState.DHD.Cx = true;
					constCalc.ApplyCommutableDeadHeadConstraint(wBIdStateContent.Constraints.DeadHeads.LstParameter);
				}
				else if (constraint == "PDO")
				{
					wBIdStateContent.CxWtState.WtPDOFS.Cx = true;
					constCalc.ApplyPartialdaysOffConstraint(wBIdStateContent.Constraints.PDOFS.LstParameter);
				}
				else if (constraint == "Days of the Month")
				{
					wBIdStateContent.CxWtState.SDO.Cx = true;
					constCalc.ApplyDaysOfMonthConstraint(wBIdStateContent.Constraints.DaysOfMonth);
					tblConstraints.ReloadData();
				}
				else if (constraint == "Overnight Cities - Bulk")
				{
					wBIdStateContent.CxWtState.BulkOC.Cx = true;
					constCalc.ApplyOvernightBulkConstraint(wBIdStateContent.Constraints.BulkOvernightCity);
					tblConstraints.ReloadData();
				}
				else if (constraint == "Commutable Lines - Manual")
				{
					wBIdStateContent.CxWtState.CL.Cx = true;
					constCalc.ApplyCommutableLinesConstraint(wBIdStateContent.Constraints.CL);
					//tblConstraints.ReloadData ();
				}
				else if (constraint == "Commutable Lines - Auto")
				{
					wBIdStateContent.CxWtState.CLAuto.Cx = true;
					constCalc.ApplyCommutableLinesAutoConstraint(wBIdStateContent.Constraints.CLAuto);
					tblConstraints.ReloadData();
				}
				else if (constraint == "Commutability")
				{
					wBIdStateContent.CxWtState.Commute.Cx = true;
					constCalc.ApplyCommuttabilityConstraint(wBIdStateContent.Constraints.Commute);
					tblConstraints.ReloadData();
				}
				else if (constraint == "Mixed Hard/Reserve")
				{
					wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx = true;
					constCalc.ApplyMixedHardandReserveConstraint();

				}
					CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


public void ApplyAndReloadConstraintsFromCommutability(string constraint)
{
	try
	{

		
		 if (constraint == "Commutable Lines - Manual")
		{
			wBIdStateContent.CxWtState.CL.Cx = true;
			constCalc.ApplyCommutableLinesConstraint(wBIdStateContent.Constraints.CL);
			//tblConstraints.ReloadData ();
		}
		else if (constraint == "Commutable Lines - Auto")
		{
			wBIdStateContent.CxWtState.CLAuto.Cx = true;
			constCalc.ApplyCommutableLinesAutoConstraint(wBIdStateContent.Constraints.CLAuto);
			//tblConstraints.ReloadData();
		}
		else if (constraint == "Commutability")
		{
			wBIdStateContent.CxWtState.Commute.Cx = true;
			constCalc.ApplyCommuttabilityConstraint(wBIdStateContent.Constraints.Commute);
			//tblConstraints.ReloadData();
		}
		CommonClass.MainController.ReloadAllContent();
		//lblLineCount.StringValue = constCalc.LinesNotConstrained();
	}
	catch (Exception ex)
	{
		CommonClass.AppDelegate.ErrorLog(ex);
		CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
	}
		}

		public void RemoveAndReloadConstraints (string constraint, int order)
		{
			try {
				WBidHelper.PushToUndoStack ();
				if (constraint == "3-on-3-off") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.No3on3off.Cx = false;
					constCalc.RemoveThreeOn3offConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Aircraft Changes") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.ACChg.Cx = false;
					constCalc.RemoveAirCraftChangesConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Blocks of Days Off") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.BDO.Cx = false;
					constCalc.RemoveBlockOfDaysOffConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Duty period") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.DP.Cx = false;
					constCalc.RemoveDutyPeriodConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Flight Time") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.FLTMIN.Cx = false;
					constCalc.RemoveFlightTimeConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Legs Per Duty Period") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.LEGS.Cx = false;
					constCalc.RemoveLegsPerDutyPeriodConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Legs Per Pairing") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.LegsPerPairing.Cx = false;
					constCalc.RemoveLegsPerPairingConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Min Pay") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.MP.Cx = false;
					constCalc.RemoveMinimumPayConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Number of Days Off") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.NODO.Cx = false;
					constCalc.RemoveNumberofDaysOffConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Time-Away-From-Base") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.PerDiem.Cx = false;
					constCalc.RemoveTimeAwayFromBaseConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Work Days") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.WorkDay.Cx = false;
					constCalc.RemoveWorkDayConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Intl – NonConus") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.InterConus.Cx = false;
					}
					wBIdStateContent.Constraints.InterConus.lstParameters.RemoveAt (order);
					constCalc.ApplyInternationalonConusConstraint (wBIdStateContent.Constraints.InterConus.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Days of the Week") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.DOW.Cx = false;
					}
					wBIdStateContent.Constraints.DaysOfWeek.lstParameters.RemoveAt (order);
					constCalc.ApplyDaysofWeekConstraint (wBIdStateContent.Constraints.DaysOfWeek.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "DH - first - last") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.DHDFoL.Cx = false;
					}
					wBIdStateContent.Constraints.DeadHeadFoL.lstParameters.RemoveAt (order);
					constCalc.ApplyDeadHeadConstraint (wBIdStateContent.Constraints.DeadHeadFoL.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Equipment Type") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.EQUIP.Cx = false;
					}
					wBIdStateContent.Constraints.EQUIP.lstParameters.RemoveAt (order);
					constCalc.ApplyEquipmentTypeConstraint (wBIdStateContent.Constraints.EQUIP.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Ground Time") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.GRD.Cx = false;
					constCalc.RemoveGroundTimeConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Overnight Cities") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.RON.Cx = false;
					}

					wBIdStateContent.Constraints.OverNightCities.lstParameters.RemoveAt (order);
					constCalc.ApplyOvernightCitiesConstraint (wBIdStateContent.Constraints.OverNightCities.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Cities-Legs") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.CitiesLegs.Cx = false;
					}

					wBIdStateContent.Constraints.CitiesLegs.lstParameters.RemoveAt (order);
					constCalc.ApplyCitiesLegsConstraint (wBIdStateContent.Constraints.CitiesLegs.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				}
				else if (constraint == "Rest") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.Rest.Cx = false;
					}
					wBIdStateContent.Constraints.Rest.lstParameters.RemoveAt (order);
					constCalc.ApplyRestConstraint (wBIdStateContent.Constraints.Rest.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				}
                else if (constraint == "Report-Release")
                {
                    if (appliedConstraints.Count(x => x == constraint) == 1)
                    {
                        btnAddConstraints.Items().FirstOrDefault(x => x.Title == constraint).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.ReportRelease.Cx = false;
                    }
                    wBIdStateContent.Constraints.ReportRelease.lstParameters.RemoveAt(order);
                    constCalc.ApplyReportReleaseConstraint(wBIdStateContent.Constraints.ReportRelease.lstParameters);
                    appliedConstraints.Remove(constraint);
                    tblConstraints.ReloadData();
                }
                else if (constraint == "Start Day")
                {
                    if (appliedConstraints.Count(x => x == constraint) == 1)
                    {
                        btnAddConstraints.Items().FirstOrDefault(x => x.Title == constraint).State = NSCellStateValue.Off;
                        wBIdStateContent.CxWtState.StartDay.Cx = false;
                    }
                    wBIdStateContent.Constraints.StartDay.lstParameters.RemoveAt(order);
                    constCalc.ApplyStartDayConstraint(wBIdStateContent.Constraints.StartDay.lstParameters);
                    appliedConstraints.Remove(constraint);
                    tblConstraints.ReloadData();
                }
                else if (constraint == "Start Day of Week") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.SDOW.Cx = false;
					}
					wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters.RemoveAt (order);
					constCalc.ApplyStartDayOfWeekConstraint (wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Trip Length") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.TL.Cx = false;
					}
					wBIdStateContent.Constraints.TripLength.lstParameters.RemoveAt (order);
					constCalc.ApplyTripLengthConstraint (wBIdStateContent.Constraints.TripLength.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Work Blk Length") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.WB.Cx = false;
					}
					wBIdStateContent.Constraints.WorkBlockLength.lstParameters.RemoveAt (order);
					constCalc.ApplyWorkBlockLengthConstraint (wBIdStateContent.Constraints.WorkBlockLength.lstParameters);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Cmut DHs") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.DHD.Cx = false;
					}
					wBIdStateContent.Constraints.DeadHeads.LstParameter.RemoveAt (order);
					constCalc.ApplyCommutableDeadHeadConstraint (wBIdStateContent.Constraints.DeadHeads.LstParameter);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "PDO") {
					if (appliedConstraints.Count (x => x == constraint) == 1) {
						btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
						wBIdStateContent.CxWtState.WtPDOFS.Cx = false;
					}
					wBIdStateContent.Constraints.PDOFS.LstParameter.RemoveAt (order);
					constCalc.ApplyPartialdaysOffConstraint (wBIdStateContent.Constraints.PDOFS.LstParameter);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Days of the Month") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.SDO.Cx = false;
					wBIdStateContent.Constraints.DaysOfMonth.OFFDays.Clear ();
					wBIdStateContent.Constraints.DaysOfMonth.WorkDays.Clear ();
					constCalc.RemoveDaysOfMonthConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Overnight Cities - Bulk") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.BulkOC.Cx = false;
					wBIdStateContent.Constraints.BulkOvernightCity.OverNightYes.Clear ();
					wBIdStateContent.Constraints.BulkOvernightCity.OverNightNo.Clear ();
					constCalc.ApplyOvernightBulkConstraint (wBIdStateContent.Constraints.BulkOvernightCity);
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				} else if (constraint == "Commutable Lines - Manual") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.CL.Cx = false;
					constCalc.RemoveCommutableLinesConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				}

				else if (constraint == "Commutable Lines - Auto") {
					btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.CLAuto.Cx = false;
					constCalc.RemoveCommutableLinesAutoConstraint ();
					appliedConstraints.Remove (constraint);
					tblConstraints.ReloadData ();
				}
                else if (constraint == "Commutability") {
                    btnAddConstraints.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.Commute.Cx = false;
					constCalc.RemoveCommutabilityConstraint ();
                    appliedConstraints.Remove (constraint);
                    tblConstraints.ReloadData ();
                }
				else if (constraint == "Mixed Hard/Reserve")
				{
					btnAddConstraints.Items().FirstOrDefault(x => x.Title == constraint).State = NSCellStateValue.Off;
					wBIdStateContent.CxWtState.MixedHardReserveTrip.Cx = false;
					constCalc.RemoveMixedHardandReserveConstraint();
					appliedConstraints.Remove(constraint);
					tblConstraints.ReloadData();
				}
				
				CommonClass.MainController.ReloadAllContent ();
				lblLineCount.StringValue = constCalc.LinesNotConstrained ();
				CommonClass.MainController.SetFlightDataDiffButton();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

	}

	public partial class ConstraintsTableSource : NSTableViewSource
	{
		ConstraintsViewController parentVC;

		public ConstraintsTableSource (ConstraintsViewController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.appliedConstraints.Count;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var vw = (ConstraintsCell)tableView.MakeView (ConstraintsApplied.ViewTypes [parentVC.appliedConstraints [(int)row]], this);
			vw.BindData (parentVC.appliedConstraints [(int)row], (int)row);
			return vw;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			if (ConstraintsApplied.ViewTypes [parentVC.appliedConstraints [(int)row]] == "DOM")
				return 270;
			else if (ConstraintsApplied.ViewTypes [parentVC.appliedConstraints [(int)row]] == "OBLK")
				return 370;
			else if (ConstraintsApplied.ViewTypes [parentVC.appliedConstraints [(int)row]] == "CL")
				return 250;
            else if (ConstraintsApplied.ViewTypes[parentVC.appliedConstraints[(int)row]] == "View6")
                return 121;
            else
				return 30;
		}

		public override bool SelectionShouldChange (NSTableView tableView)
		{
			return false;
		}

	}

}

