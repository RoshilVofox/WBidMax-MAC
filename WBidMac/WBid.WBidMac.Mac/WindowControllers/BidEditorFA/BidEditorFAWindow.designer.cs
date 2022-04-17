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
	[Register ("BidEditorFAWindowController")]
	partial class BidEditorFAWindowController
	{
		[Outlet]
		AppKit.NSButton btnAdd { get; set; }

		[Outlet]
		AppKit.NSButton btnAddReserve { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnBuddy1 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnBuddy2 { get; set; }

		[Outlet]
		AppKit.NSButton btnCancelClear { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeBuddy { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeEmployee { get; set; }

		[Outlet]
		AppKit.NSButton btnClear { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnFirst { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnFourth { get; set; }

		[Outlet]
		AppKit.NSButton btnInsert { get; set; }

		[Outlet]
		AppKit.NSButton btnRemove { get; set; }

		[Outlet]
		AppKit.NSButton btnRepeatA { get; set; }

		[Outlet]
		AppKit.NSButton btnRepeatB { get; set; }

		[Outlet]
		AppKit.NSButton btnRepeatC { get; set; }

		[Outlet]
		AppKit.NSButton btnSaveClose { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnSecond { get; set; }

		[Outlet]
		AppKit.NSButton btnSubmit { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnThird { get; set; }

		[Outlet]
		AppKit.NSTextField lblBids { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddy1 { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddy2 { get; set; }

		[Outlet]
		AppKit.NSTextField lblLines { get; set; }

		[Outlet]
		AppKit.NSTableView tblAvailableLines { get; set; }

		[Outlet]
		AppKit.NSTableView tblSelectedLines { get; set; }

		[Outlet]
		AppKit.NSTextField txtManual { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblAvailableLines != null) {
				tblAvailableLines.Dispose ();
				tblAvailableLines = null;
			}

			if (tblSelectedLines != null) {
				tblSelectedLines.Dispose ();
				tblSelectedLines = null;
			}

			if (lblLines != null) {
				lblLines.Dispose ();
				lblLines = null;
			}

			if (lblBids != null) {
				lblBids.Dispose ();
				lblBids = null;
			}

			if (txtManual != null) {
				txtManual.Dispose ();
				txtManual = null;
			}

			if (btnAdd != null) {
				btnAdd.Dispose ();
				btnAdd = null;
			}

			if (btnInsert != null) {
				btnInsert.Dispose ();
				btnInsert = null;
			}

			if (btnRemove != null) {
				btnRemove.Dispose ();
				btnRemove = null;
			}

			if (btnClear != null) {
				btnClear.Dispose ();
				btnClear = null;
			}

			if (btnRepeatA != null) {
				btnRepeatA.Dispose ();
				btnRepeatA = null;
			}

			if (btnRepeatB != null) {
				btnRepeatB.Dispose ();
				btnRepeatB = null;
			}

			if (btnRepeatC != null) {
				btnRepeatC.Dispose ();
				btnRepeatC = null;
			}

			if (btnFirst != null) {
				btnFirst.Dispose ();
				btnFirst = null;
			}

			if (btnSecond != null) {
				btnSecond.Dispose ();
				btnSecond = null;
			}

			if (btnThird != null) {
				btnThird.Dispose ();
				btnThird = null;
			}

			if (btnFourth != null) {
				btnFourth.Dispose ();
				btnFourth = null;
			}

			if (btnBuddy1 != null) {
				btnBuddy1.Dispose ();
				btnBuddy1 = null;
			}

			if (btnBuddy2 != null) {
				btnBuddy2.Dispose ();
				btnBuddy2 = null;
			}

			if (lblBuddy1 != null) {
				lblBuddy1.Dispose ();
				lblBuddy1 = null;
			}

			if (lblBuddy2 != null) {
				lblBuddy2.Dispose ();
				lblBuddy2 = null;
			}

			if (btnAddReserve != null) {
				btnAddReserve.Dispose ();
				btnAddReserve = null;
			}

			if (btnChangeBuddy != null) {
				btnChangeBuddy.Dispose ();
				btnChangeBuddy = null;
			}

			if (btnChangeEmployee != null) {
				btnChangeEmployee.Dispose ();
				btnChangeEmployee = null;
			}

			if (btnSaveClose != null) {
				btnSaveClose.Dispose ();
				btnSaveClose = null;
			}

			if (btnCancelClear != null) {
				btnCancelClear.Dispose ();
				btnCancelClear = null;
			}

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}
		}
	}

	[Register ("BidEditorFAWindow")]
	partial class BidEditorFAWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
