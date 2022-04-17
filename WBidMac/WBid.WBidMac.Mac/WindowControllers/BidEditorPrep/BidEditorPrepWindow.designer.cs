// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
	[Register ("BidEditorPrepWindowController")]
	partial class BidEditorPrepWindowController
	{
		[Outlet]
		AppKit.NSButton btnAvoidanceBid { get; set; }

		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeAvoidance { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeEmployee { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnDomicile { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSMatrix btnPeriod { get; set; }

		[Outlet]
		AppKit.NSMatrix btnPosition { get; set; }

		[Outlet]
		AppKit.NSMatrix btnRound { get; set; }

		[Outlet]
		AppKit.NSButton btnStartCurrentOrder { get; set; }

		[Outlet]
		AppKit.NSTextField txtLine1 { get; set; }

		[Outlet]
		AppKit.NSTextField txtLine2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDomicile != null) {
				btnDomicile.Dispose ();
				btnDomicile = null;
			}

			if (btnPeriod != null) {
				btnPeriod.Dispose ();
				btnPeriod = null;
			}

			if (btnPosition != null) {
				btnPosition.Dispose ();
				btnPosition = null;
			}

			if (btnRound != null) {
				btnRound.Dispose ();
				btnRound = null;
			}

			if (txtLine1 != null) {
				txtLine1.Dispose ();
				txtLine1 = null;
			}

			if (txtLine2 != null) {
				txtLine2.Dispose ();
				txtLine2 = null;
			}

			if (btnStartCurrentOrder != null) {
				btnStartCurrentOrder.Dispose ();
				btnStartCurrentOrder = null;
			}

			if (btnAvoidanceBid != null) {
				btnAvoidanceBid.Dispose ();
				btnAvoidanceBid = null;
			}

			if (btnChangeAvoidance != null) {
				btnChangeAvoidance.Dispose ();
				btnChangeAvoidance = null;
			}

			if (btnChangeEmployee != null) {
				btnChangeEmployee.Dispose ();
				btnChangeEmployee = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}
		}
	}

	[Register ("BidEditorPrepWindow")]
	partial class BidEditorPrepWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
