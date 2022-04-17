
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
//using System.Collections.Generic;
using WBid.WBidiPad.Model;
//using System.Linq;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;

namespace WBid.WBidMac.Mac
{
	public partial class BidEditorPrepWindowController : AppKit.NSWindowController
	{
		#region Constructors
		string[] domicileArray = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();
		public ObservableCollection<BidPeriod> BidPeriods;
		public BidPrep BidPrepDetails { get; set; }
//		private bool _isStartwithCurrentLineChecked;
//		private bool _isAvodenceBidChecked;

		// Called when created from unmanaged code
		public BidEditorPrepWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidEditorPrepWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BidEditorPrepWindowController () : base ("BidEditorPrepWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new BidEditorPrepWindow Window {
			get {
				return (BidEditorPrepWindow)base.Window;
			}
		}

		static NSButton closeButton;
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				
				this.Window.Title = "Bid Editor Preparation";
				GlobalSettings.TemporaryEmployeeNumber = (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) ? GlobalSettings.WbidUserContent.UserInformation.EmpNo : string.Empty;
				
				btnChangeAvoidance.Activated += (object sender, EventArgs e) => {
					if (btnPosition.SelectedTag == 1) {
						var panel = new NSPanel ();
						var changeAvoid = new ChangeAvoidanceViewController ();
						CommonClass.Panel = panel;
						panel.SetContentSize (new CoreGraphics.CGSize (400, 270));
						panel.ContentView = changeAvoid.View;
						NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					} else if (btnPosition.SelectedTag == 2) {
						var panel = new NSPanel ();
						var changeBud = new ChangeBuddyViewController ();
						CommonClass.Panel = panel;
						panel.SetContentSize (new CoreGraphics.CGSize (400, 190));
						panel.ContentView = changeBud.View;
						NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					}
				};
				btnChangeEmployee.Activated += (object sender, EventArgs e) => {
					var panel = new NSPanel ();
					var changeEmp = new ChangeEmployeeViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (400, 180));
					panel.ContentView = changeEmp.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
				};
				SetupViews ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		void SetupViews ()
		{
			btnDomicile.AddItems (domicileArray);
			btnDomicile.SelectItem (GlobalSettings.CurrentBidDetails.Domicile);
			List<int> lstMonth = new List<int> ();
			int currentMonth = GlobalSettings.CurrentBidDetails.Month;
			lstMonth.Add ((currentMonth - 1 == 0) ? 12 : (currentMonth - 1));
			lstMonth.Add (currentMonth);
			lstMonth.Add ((currentMonth + 1 == 13) ? 1 : (currentMonth + 1));
			BidPeriods = new ObservableCollection<BidPeriod> ();
			foreach (int monthId in lstMonth) {
				BidPeriod bidPerid = WBidCollection.GetBidPeriods ().FirstOrDefault (x => x.BidPeriodId == monthId);
				if (bidPerid != null) {
					BidPeriods.Add (bidPerid);
				}
			}
			string[] arr =  {
				BidPeriods [0].Period,
				BidPeriods [1].Period,
				BidPeriods [2].Period
			};
			for (int i = 0; i < 3; i++) {
				btnPeriod.Cells [i].Title = arr [i];
			}
			btnPeriod.SelectCellWithTag (1);

			string pos = GlobalSettings.CurrentBidDetails.Postion;
			if (pos == "CP")
				btnPosition.SelectCellWithTag (0);
			else if (pos == "FO")
				btnPosition.SelectCellWithTag (1);
			else
				btnPosition.SelectCellWithTag (2);

			if (pos != "FA")
				btnPosition.Cells [2].Enabled = false;

			string round = GlobalSettings.CurrentBidDetails.Round;
			if (round == "M")
				btnRound.SelectCellWithTag (0);
			else
				btnRound.SelectCellWithTag (1);

			UpdateUI ();

			btnDomicile.Activated += (object sender, EventArgs e) => {
				UpdateUI ();
			};
			btnPeriod.Activated += (object sender, EventArgs e) => {
				UpdateUI ();
			};
			btnPosition.Activated += (object sender, EventArgs e) => {
				UpdateUI ();
			};
			btnRound.Activated += (object sender, EventArgs e) => {
				UpdateUI ();
			};

			btnCancel.Activated += (object sender, EventArgs e) => {
				this.Window.Close ();
				this.Window.OrderOut (this);
				NSApplication.SharedApplication.StopModal ();
			};

			btnOK.Activated += (object sender, EventArgs e) => {

				BidPrepDetails = new BidPrep ();
				BidPrepDetails.BidYear = GlobalSettings.CurrentBidDetails.Year;
				BidPrepDetails.BidPeriod = BidPeriods [(int)btnPeriod.SelectedTag].BidPeriodId;

				if (btnPosition.SelectedTag == 0)
					BidPrepDetails.Position = "CP";
				else if (btnPosition.SelectedTag == 1)
					BidPrepDetails.Position = "FO";
				else
					BidPrepDetails.Position = "FA";

				BidPrepDetails.BidRound = (btnRound.SelectedTag == 0) ? "D" : "B";
				BidPrepDetails.Domicile = btnDomicile.SelectedItem.Title;

				if (btnAvoidanceBid.State == NSCellStateValue.On)
					BidPrepDetails.IsChkAvoidanceBid = true;
				else
					BidPrepDetails.IsChkAvoidanceBid = false;

				if (btnStartCurrentOrder.State == NSCellStateValue.On)
					BidPrepDetails.IsOnStartWithCurrentLine = true;
				else
					BidPrepDetails.IsOnStartWithCurrentLine = false;

				BidPrepDetails.LineFrom = int.Parse (txtLine1.StringValue.Trim ());
				BidPrepDetails.LineTo = int.Parse (txtLine2.StringValue.Trim ());
				GlobalSettings.BidPrepDetails = BidPrepDetails;

				if (BidPrepDetails.Position == "CP" || BidPrepDetails.Position == "FO") {
					var bidEditPilot = new BidEditorPilotWindowController ();
					this.Window.AddChildWindow (bidEditPilot.Window, NSWindowOrderingMode.Above);
					NSApplication.SharedApplication.RunModalForWindow (bidEditPilot.Window);
				} else if (BidPrepDetails.Position == "FA" && GlobalSettings.CurrentBidDetails.Postion == "FA") {
					if (GlobalSettings.CurrentBidDetails.Round == "M") {
						var bidEditFA = new BidEditorFAWindowController ();
						this.Window.AddChildWindow (bidEditFA.Window, NSWindowOrderingMode.Above);
						NSApplication.SharedApplication.RunModalForWindow (bidEditFA.Window);
					} else {
						var bidEditPilot = new BidEditorPilotWindowController ();
						this.Window.AddChildWindow (bidEditPilot.Window, NSWindowOrderingMode.Above);
						NSApplication.SharedApplication.RunModalForWindow (bidEditPilot.Window);
					}
				}
			};

		}

		void UpdateUI ()
		{
			string position = string.Empty;
			string round = string.Empty;

			//Captain
			if (btnPosition.SelectedTag == 0) {
				btnAvoidanceBid.Title = "Avoidance Bids not allowed for Captains";
				btnAvoidanceBid.Enabled = false;
				btnChangeAvoidance.Title = "Change Avoidance Bids";
				btnChangeAvoidance.Enabled = false;
				position = "CP";
			}
			//First Officer
			else if (btnPosition.SelectedTag == 1) {
				btnAvoidanceBid.Title = "Clear Avoidance Bids for this bid";
				btnAvoidanceBid.Enabled = true;
				btnChangeAvoidance.Title = "Change Avoidance Bids";
				btnChangeAvoidance.Enabled = true;
				position = "FO";
			}
			//Flight Attendant
			else {
				btnAvoidanceBid.Title = "Clear Buddy Bids for this bid";
				btnAvoidanceBid.Enabled = true;
				btnChangeAvoidance.Title = "Change Buddy Bids";
				btnChangeAvoidance.Enabled = true;
				position = "FA";
				if (GlobalSettings.CurrentBidDetails.Round == "S") {
					btnAvoidanceBid.Enabled = false;
					btnChangeAvoidance.Enabled = false;
				}
			}

			round = (btnRound.SelectedTag == 0) ? "M" : "S";

			if (position == GlobalSettings.CurrentBidDetails.Postion && round == GlobalSettings.CurrentBidDetails.Round && btnDomicile.SelectedItem.Title == GlobalSettings.CurrentBidDetails.Domicile && BidPeriods [(int)btnPeriod.SelectedTag].BidPeriodId == GlobalSettings.CurrentBidDetails.Month) {
				var startline = GlobalSettings.Lines.Min (x => x.LineNum);
				txtLine1.StringValue = startline.ToString ();
				txtLine2.StringValue = (GlobalSettings.Lines.Count).ToString ();
				txtLine1.Enabled = false;
				txtLine2.Enabled = false;
				btnStartCurrentOrder.Enabled = true;

			} else {
				txtLine1.StringValue = "1";
				txtLine2.StringValue = "750";
				txtLine1.Enabled = true;
				txtLine2.Enabled = true;
				btnStartCurrentOrder.Enabled = false;

			}
		}
	}
}

