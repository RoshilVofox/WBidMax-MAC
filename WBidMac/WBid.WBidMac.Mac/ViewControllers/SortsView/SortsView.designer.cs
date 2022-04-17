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
	[Register ("SortsViewController")]
	partial class SortsViewController
	{
		[Outlet]
		AppKit.NSButton btnBlankToBottom { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnBlockDrop { get; set; }

		[Outlet]
		AppKit.NSButton btnBlockSort { get; set; }

		[Outlet]
		AppKit.NSButton btnClear { get; set; }

		[Outlet]
		AppKit.NSButton btnHelpSort { get; set; }

		[Outlet]
		AppKit.NSButton btnLineNumber { get; set; }

		[Outlet]
		AppKit.NSButton btnLinePay { get; set; }

		[Outlet]
		AppKit.NSButton btnPayPerDay { get; set; }

		[Outlet]
		AppKit.NSButton btnPayPerDH { get; set; }

		[Outlet]
		AppKit.NSButton btnPayPerFDP { get; set; }

		[Outlet]
		AppKit.NSButton btnPayPerFH { get; set; }

		[Outlet]
		AppKit.NSButton btnPayPerTimeAway { get; set; }

		[Outlet]
		AppKit.NSButton btnReserveToBottom { get; set; }

		[Outlet]
		AppKit.NSButton btnSortByAward { get; set; }

		[Outlet]
		AppKit.NSButton btnSortBySubmitted { get; set; }

		[Outlet]
		AppKit.NSTextField lblManual { get; set; }

		[Outlet]
		AppKit.NSTextField lblSelected { get; set; }

		[Outlet]
		public AppKit.NSTableView tblBlockSort { get; private set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBlankToBottom != null) {
				btnBlankToBottom.Dispose ();
				btnBlankToBottom = null;
			}

			if (btnBlockDrop != null) {
				btnBlockDrop.Dispose ();
				btnBlockDrop = null;
			}

			if (btnBlockSort != null) {
				btnBlockSort.Dispose ();
				btnBlockSort = null;
			}

			if (btnClear != null) {
				btnClear.Dispose ();
				btnClear = null;
			}

			if (btnHelpSort != null) {
				btnHelpSort.Dispose ();
				btnHelpSort = null;
			}

			if (btnLineNumber != null) {
				btnLineNumber.Dispose ();
				btnLineNumber = null;
			}

			if (btnLinePay != null) {
				btnLinePay.Dispose ();
				btnLinePay = null;
			}

			if (btnPayPerDay != null) {
				btnPayPerDay.Dispose ();
				btnPayPerDay = null;
			}

			if (btnPayPerDH != null) {
				btnPayPerDH.Dispose ();
				btnPayPerDH = null;
			}

			if (btnPayPerFDP != null) {
				btnPayPerFDP.Dispose ();
				btnPayPerFDP = null;
			}

			if (btnPayPerFH != null) {
				btnPayPerFH.Dispose ();
				btnPayPerFH = null;
			}

			if (btnPayPerTimeAway != null) {
				btnPayPerTimeAway.Dispose ();
				btnPayPerTimeAway = null;
			}

			if (btnReserveToBottom != null) {
				btnReserveToBottom.Dispose ();
				btnReserveToBottom = null;
			}

			if (lblManual != null) {
				lblManual.Dispose ();
				lblManual = null;
			}

			if (lblSelected != null) {
				lblSelected.Dispose ();
				lblSelected = null;
			}

			if (tblBlockSort != null) {
				tblBlockSort.Dispose ();
				tblBlockSort = null;
			}

			if (btnSortBySubmitted != null) {
				btnSortBySubmitted.Dispose ();
				btnSortBySubmitted = null;
			}

			if (btnSortByAward != null) {
				btnSortByAward.Dispose ();
				btnSortByAward = null;
			}
		}
	}

	[Register ("SortsView")]
	partial class SortsView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
