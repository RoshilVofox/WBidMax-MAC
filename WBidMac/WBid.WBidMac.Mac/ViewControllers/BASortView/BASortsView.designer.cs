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
	[Register ("BASortsViewController")]
	partial class BASortsViewController
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
		AppKit.NSSegmentedControl segLinePriority { get; set; }

		[Outlet]
		AppKit.NSTableView tblBlockSort { get; set; }

		[Action ("FunClearAction:")]
		partial void FunClearAction (Foundation.NSObject sender);

		[Action ("segLinePriorityAction:")]
		partial void segLinePriorityAction (Foundation.NSObject sender);
		
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

			if (segLinePriority != null) {
				segLinePriority.Dispose ();
				segLinePriority = null;
			}

			if (tblBlockSort != null) {
				tblBlockSort.Dispose ();
				tblBlockSort = null;
			}
		}
	}

	[Register ("BASortsView")]
	partial class BASortsView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
