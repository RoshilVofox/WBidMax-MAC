
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary;
using System.IO;
using CoreGraphics;

namespace WBid.WBidMac.Mac
{
	public partial class MILConfigViewController : AppKit.NSViewController
	{
		#region Constructors

		MILData milData;
		NSPopover pop;
		public bool isStartPop;
		public DateTime curDate;
		private WBidState wBIdStateContent;
		// Called when created from unmanaged code
		public MILConfigViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MILConfigViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public MILConfigViewController () : base ("MILConfigView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new MILConfigView View {
			get {
				return (MILConfigView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			pop = new NSPopover ();
			pop.ContentViewController = new NSViewController ();
			pop.ContentViewController.View = vwDatePopover;
			pop.ContentSize = new CGSize (139, 170);
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            if (File.Exists (WBidHelper.MILFilePath)) {
				LineInfo lineInfo = null;
				using (FileStream milStream = File.OpenRead (WBidHelper.MILFilePath)) {
					MILData milDataobject = new MILData ();
					milData = ProtoSerailizer.DeSerializeObject (WBidHelper.MILFilePath, milDataobject, milStream);
				}
				GlobalSettings.MILDates = wBIdStateContent.MILDateList;
				if (GlobalSettings.MILDates != null && GlobalSettings.MILDates.Count > 0) {
					lblStartDates.StringValue = string.Empty;
					lblEndDates.StringValue = string.Empty;
					if (GlobalSettings.MILDates != null) {
						for (int i = 0; i < GlobalSettings.MILDates.Count; i++) {
							lblStartDates.StringValue += GlobalSettings.MILDates [i].StartAbsenceDate.ToString ("MM/dd/yyyy HH:mm") + "\n\n";
							lblEndDates.StringValue += GlobalSettings.MILDates [i].EndAbsenceDate.ToString ("MM/dd/yyyy HH:mm") + "\n\n";
						}
					}
					CommonClass.MILList = GlobalSettings.MILDates;
				} else {
					this.View.AddSubview (vwDateSelect);
                    if (interfaceStyle == "Dark")
                    {
                        objLabelStart.Hidden = true;
                        objLabelEnd.Hidden = true;
                        objLabelTitle.Hidden = true;
                    }
                    CommonClass.MILList = new System.Collections.Generic.List<Absense> ();
					CommonClass.MILList.Add (new Absense () {
						StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
						EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
					});
				}

			} else {
				this.View.AddSubview (vwDateSelect);
                if (interfaceStyle == "Dark")
                {
                    objLabelStart.Hidden = true;
                    objLabelEnd.Hidden = true;
                    objLabelTitle.Hidden = true;
                }
                CommonClass.MILList = new System.Collections.Generic.List<Absense> ();
				CommonClass.MILList.Add (new Absense () {
					StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
					EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
				});
			}

//			if (GlobalSettings.MILDates != null) {
//				CommonClass.MILList = GlobalSettings.MILDates;
//			} else {
//				CommonClass.MILList = new System.Collections.Generic.List<Absense> ();
//				CommonClass.MILList.Add (new Absense () {
//					StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
//					EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
//				});
//			}

			tblMILDates.Source = new MILDatesTableSource (this);
			ReloadView ();
			btnAdd.Activated += (object sender, EventArgs e) => {
				CommonClass.MILList.Add (new Absense () {
					StartAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate,
					EndAbsenceDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate
				});
				ReloadView ();
			};
			btnCancel.Activated += (object sender, EventArgs e) => {
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
				CommonClass.MainController.btnMIL.State = NSCellStateValue.Off;
			};
			btnCancel2.Activated += (object sender, EventArgs e) => {
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
				CommonClass.MainController.btnMIL.State = NSCellStateValue.Off;
			};
			btnApply.Activated += btnApplyClicked;


            btnCalulateNew.Activated += (object sender, EventArgs e) => {
                if (interfaceStyle == "Dark")
                {
                    objLabelStart.Hidden = true;
                    objLabelEnd.Hidden = true;
                    objLabelTitle.Hidden = true;
                }

                this.View.AddSubview (vwDateSelect);
			};
			btnCalculate2.Activated += btnCalculateClicked;
			btnPopOK.Activated += (object sender, EventArgs e) => {
				if (isStartPop)
					CommonClass.MILList [(int)dpPop.Tag].StartAbsenceDate = dpPop.DateValue.NSDateToDateTime ();
				else
					CommonClass.MILList [(int)dpPop.Tag].EndAbsenceDate = dpPop.DateValue.NSDateToDateTime ();
				tblMILDates.ReloadData ();
				pop.Close ();
			};
			btnPopCancel.Activated += (object sender, EventArgs e) => {
				pop.Close ();
			};
		}

		public void ShowCalendarPopover (object sender)
		{
			var btn = (NSButton)sender;
			var view = tblMILDates.GetView (0, btn.Tag, false);
			dpPop.Tag = btn.Tag;
			dpPop.DateValue = curDate.DateTimeToNSDate ();
			pop.Show (btn.Frame, view, NSRectEdge.MaxXEdge);
		}

		void btnCalculateClicked (object sender, EventArgs e)
		{
			var milList = CommonClass.MILList.OrderBy (x => x.StartAbsenceDate).ToList ();
			bool isValid = ValidateDates ();
			if (isValid) {
				WBidHelper.PushToUndoStack ();
				CommonClass.MainController.UpdateUndoRedoButtons ();
//				var loadingOverlay = new LoadingOverlay (View.Bounds, "Calculating MIL data. Please wait..");
//				View.Add (loadingOverlay);
				var overlayPanel = new NSPanel ();
				overlayPanel.SetContentSize (new CGSize (400, 120));
				var overlay = new OverlayViewController ();
				overlay.OverlayText = "Calculating and Applying MIL Data..";
				overlayPanel.ContentView = overlay.View;
				NSApplication.SharedApplication.BeginSheet (overlayPanel, CommonClass.Panel);

				BeginInvokeOnMainThread (() => {
					GlobalSettings.MILDates = GenarateOrderedMILDates (milList);
					CalculateMIL calculateMIL = new CalculateMIL ();
					MILParams milParams = new MILParams ();

					NetworkData networkData = new NetworkData ();
					if (System.IO.File.Exists (WBidHelper.GetAppDataPath () + "/FlightData.NDA"))
						networkData.ReadFlightRoutes ();
					else
						networkData.GetFlightRoutes ();

					//calculate MIL value and create MIL File
					//==============================================
					WBidCollection.GenerateSplitPointCities ();
					wBIdStateContent.MILDateList = milList;
					milParams.Lines = GlobalSettings.Lines.ToList ();
					Dictionary<string, TripMultiMILData> milvalue = calculateMIL.CalculateMILValues (milParams);

					MILData milData = new MILData ();

					milData.Version = GlobalSettings.MILFileVersion;
					milData.MILValue = milvalue;

					var stream = File.Create (WBidHelper.MILFilePath);
					ProtoSerailizer.SerializeObject (WBidHelper.MILFilePath, milData, stream);
					stream.Dispose ();
					stream.Close ();

					//==============================================

					//Apply MIL values (calculate property values including Modern bid line properties
					//==============================================


					GlobalSettings.MILData = milvalue;
					GlobalSettings.MenuBarButtonStatus.IsMIL = true;

					RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties ();
					recalcalculateLineProperties.CalcalculateLineProperties ();
					StateManagement statemanagement = new StateManagement ();
					WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					statemanagement.ApplyCSW(wBidStateCont);

					InvokeOnMainThread (() => {
						NSApplication.SharedApplication.EndSheet (overlayPanel);
						overlayPanel.OrderOut (this);
						NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
						CommonClass.Panel.OrderOut (this);
						//CommonClass.MainController.UpdateSaveButton(true);
						CommonClass.MainController.SetVacButtonStates ();
						CommonClass.MainController.ReloadAllContent ();
					});
				});

			} else {
				var alert = new NSAlert ();
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Please enter valid dates";
				alert.RunModal ();
			}

		}

		private bool ValidateDates ()
		{
			var valid = true;
			var mildates = CommonClass.MILList.ToList ();

			mildates.ForEach (item => {
				mildates.Where (y => y != item).ToList ().ForEach (z => {
					if (item.StartAbsenceDate > item.EndAbsenceDate
					    || item.StartAbsenceDate < z.StartAbsenceDate && z.StartAbsenceDate < item.EndAbsenceDate
					    || item.StartAbsenceDate < z.EndAbsenceDate && z.EndAbsenceDate < item.EndAbsenceDate) {
						valid = false;
						return;
					}

				});
			});

			return valid;
		}

		private List<Absense> GenarateOrderedMILDates (List<Absense> milList)
		{
			List<Absense> absence = new List<Absense> ();
			if (milList.Count > 0) {
				absence.Add (new Absense {
					StartAbsenceDate = milList.FirstOrDefault ().StartAbsenceDate,
					EndAbsenceDate = milList.FirstOrDefault ().EndAbsenceDate,
					AbsenceType = "VA"
				});

				for (int count = 0; count < milList.Count - 1; count++) {
					if ((milList [count + 1].StartAbsenceDate - milList [count].EndAbsenceDate).Days == 1) {
						absence [absence.Count - 1].EndAbsenceDate = milList [count + 1].EndAbsenceDate;
					} else {
						absence.Add (new Absense {
							StartAbsenceDate = milList [count + 1].StartAbsenceDate,
							EndAbsenceDate = milList [count + 1].EndAbsenceDate,
							AbsenceType = "VA"
						});
					}
				}
			}
			return absence;
		}

		void btnApplyClicked (object sender, EventArgs e)
		{
//			var loadingOverlay = new LoadingOverlay (View.Bounds, "Applying MIL. Please wait..");
//			View.Add (loadingOverlay);
			WBidHelper.PushToUndoStack ();
			CommonClass.MainController.UpdateUndoRedoButtons ();
			var overlayPanel = new NSPanel ();
			overlayPanel.SetContentSize (new CGSize (400, 120));
			var overlay = new OverlayViewController ();
			overlay.OverlayText = "Applying MIL Data..";
			overlayPanel.ContentView = overlay.View;
			NSApplication.SharedApplication.BeginSheet (overlayPanel, CommonClass.Panel);

			BeginInvokeOnMainThread (() => {
				GlobalSettings.MILDates = GenarateOrderedMILDates (wBIdStateContent.MILDateList);
				//Apply MIL values (calculate property values including Modern bid line properties
				//==============================================



				var milList = CommonClass.MILList.OrderBy(x => x.StartAbsenceDate).ToList();
				GlobalSettings.MILDates = GenarateOrderedMILDates(milList);
				CalculateMIL calculateMIL = new CalculateMIL();
				MILParams milParams = new MILParams();

				NetworkData networkData = new NetworkData();
				if (System.IO.File.Exists(WBidHelper.GetAppDataPath() + "/FlightData.NDA"))
					networkData.ReadFlightRoutes();
				else
					networkData.GetFlightRoutes();

				//calculate MIL value and create MIL File
				//==============================================
				WBidCollection.GenerateSplitPointCities();
				wBIdStateContent.MILDateList = milList;
				milParams.Lines = GlobalSettings.Lines.ToList();
				Dictionary<string, TripMultiMILData> milvalue = calculateMIL.CalculateMILValues(milParams);

				MILData milData1 = new MILData();

				milData1.Version = GlobalSettings.MILFileVersion;
				milData1.MILValue = milvalue;





				GlobalSettings.MILData = milData1.MILValue;
				GlobalSettings.MenuBarButtonStatus.IsMIL = true;

				RecalcalculateLineProperties recalcalculateLineProperties = new RecalcalculateLineProperties ();
				recalcalculateLineProperties.CalcalculateLineProperties ();

				StateManagement statemanagement = new StateManagement ();
				WBidState wBidStateCont = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				statemanagement.ApplyCSW(wBidStateCont);
				InvokeOnMainThread (() => {
					NSApplication.SharedApplication.EndSheet (overlayPanel);
					overlayPanel.OrderOut (this);
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
					//CommonClass.MainController.UpdateSaveButton(true);
					CommonClass.MainController.SetVacButtonStates ();
					CommonClass.MainController.ReloadAllContent ();
				});
			});
		}

		public void ReloadView ()
		{
			if (CommonClass.MILList.Count == 4)
				btnAdd.Enabled = false;
			else
				btnAdd.Enabled = true;
			tblMILDates.ReloadData ();
		}

	}

	public partial class MILDatesTableSource : NSTableViewSource
	{
		MILConfigViewController parentVC;

		public MILDatesTableSource (MILConfigViewController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return CommonClass.MILList.Count;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var vw = (MILConfigCell)tableView.MakeView ("MILCell", this);
			try {
				vw.BindData (CommonClass.MILList [(int)row], (int)row);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
			return vw;
		}


	}

}

