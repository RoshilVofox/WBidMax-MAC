// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
	[Register ("NewBidWindowController")]
	partial class NewBidWindowController
	{
		[Outlet]
		AppKit.NSButton btnBlockTimeBack { get; set; }

		[Outlet]
		AppKit.NSButton btnBlockTimeContinue { get; set; }

		[Outlet]
		AppKit.NSMatrix btnDomicle { get; set; }

		[Outlet]
		AppKit.NSButton btnHelp { get; set; }

		[Outlet]
		AppKit.NSMatrix btnMonth { get; set; }

		[Outlet]
		AppKit.NSButton btnNewBidContinue { get; set; }

		[Outlet]
		AppKit.NSButton btnOverlapBack { get; set; }

		[Outlet]
		AppKit.NSMatrix btnOverlapChoice { get; set; }

		[Outlet]
		AppKit.NSButton btnOverlapContinue { get; set; }

		[Outlet]
		AppKit.NSButton btnOverlapSkip { get; set; }

		[Outlet]
		AppKit.NSMatrix btnPosition { get; set; }

		[Outlet]
		AppKit.NSButton btnRefresh { get; set; }

		[Outlet]
		AppKit.NSMatrix btnRound { get; set; }

		[Outlet]
		AppKit.NSMatrix btnYear { get; set; }

		[Outlet]
		AppKit.NSBox bxBlockTime { get; set; }

		[Outlet]
		AppKit.NSBox bxFirstRnd { get; set; }

		[Outlet]
		AppKit.NSBox bxSecondRnd { get; set; }

		[Outlet]
		AppKit.NSButton ckImportLine { get; set; }

		[Outlet]
		AppKit.NSButton ckMonthlyOverlap { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpMinRest { get; set; }

		[Outlet]
		AppKit.NSTextField txtYear { get; set; }

		[Outlet]
		AppKit.NSView viewOverLapAlert { get; set; }

		[Outlet]
		AppKit.NSView vwBlockTime { get; set; }

		[Outlet]
		AppKit.NSView vwNewBid { get; set; }

		[Outlet]
		AppKit.NSView vwOverlap { get; set; }

		[Action ("btnFOTapped:")]
		partial void btnFOTapped (Foundation.NSObject sender);

		[Action ("btnOverlapAlertTapped:")]
		partial void btnOverlapAlertTapped (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBlockTimeBack != null) {
				btnBlockTimeBack.Dispose ();
				btnBlockTimeBack = null;
			}

			if (btnBlockTimeContinue != null) {
				btnBlockTimeContinue.Dispose ();
				btnBlockTimeContinue = null;
			}

			if (btnDomicle != null) {
				btnDomicle.Dispose ();
				btnDomicle = null;
			}

			if (btnHelp != null) {
				btnHelp.Dispose ();
				btnHelp = null;
			}

			if (btnMonth != null) {
				btnMonth.Dispose ();
				btnMonth = null;
			}

			if (btnNewBidContinue != null) {
				btnNewBidContinue.Dispose ();
				btnNewBidContinue = null;
			}

			if (btnOverlapBack != null) {
				btnOverlapBack.Dispose ();
				btnOverlapBack = null;
			}

			if (btnOverlapChoice != null) {
				btnOverlapChoice.Dispose ();
				btnOverlapChoice = null;
			}

			if (btnOverlapContinue != null) {
				btnOverlapContinue.Dispose ();
				btnOverlapContinue = null;
			}

			if (btnOverlapSkip != null) {
				btnOverlapSkip.Dispose ();
				btnOverlapSkip = null;
			}

			if (btnPosition != null) {
				btnPosition.Dispose ();
				btnPosition = null;
			}

			if (btnRefresh != null) {
				btnRefresh.Dispose ();
				btnRefresh = null;
			}

			if (btnRound != null) {
				btnRound.Dispose ();
				btnRound = null;
			}

			if (btnYear != null) {
				btnYear.Dispose ();
				btnYear = null;
			}

			if (bxBlockTime != null) {
				bxBlockTime.Dispose ();
				bxBlockTime = null;
			}

			if (bxFirstRnd != null) {
				bxFirstRnd.Dispose ();
				bxFirstRnd = null;
			}

			if (bxSecondRnd != null) {
				bxSecondRnd.Dispose ();
				bxSecondRnd = null;
			}

			if (ckImportLine != null) {
				ckImportLine.Dispose ();
				ckImportLine = null;
			}

			if (ckMonthlyOverlap != null) {
				ckMonthlyOverlap.Dispose ();
				ckMonthlyOverlap = null;
			}

			if (dpMinRest != null) {
				dpMinRest.Dispose ();
				dpMinRest = null;
			}

			if (txtYear != null) {
				txtYear.Dispose ();
				txtYear = null;
			}

			if (viewOverLapAlert != null) {
				viewOverLapAlert.Dispose ();
				viewOverLapAlert = null;
			}

			if (vwBlockTime != null) {
				vwBlockTime.Dispose ();
				vwBlockTime = null;
			}

			if (vwNewBid != null) {
				vwNewBid.Dispose ();
				vwNewBid = null;
			}

			if (vwOverlap != null) {
				vwOverlap.Dispose ();
				vwOverlap = null;
			}
		}
	}

	[Register ("NewBidWindow")]
	partial class NewBidWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
