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
	[Register ("BidEditorPilotWindowController")]
	partial class BidEditorPilotWindowController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnCell { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeAvoidance { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeEmployee { get; set; }

		[Outlet]
		AppKit.NSButton btnClear { get; set; }

		[Outlet]
		AppKit.NSButton btnDelete { get; set; }

		[Outlet]
		AppKit.NSButton btnSubmit { get; set; }

		[Outlet]
		AppKit.NSCollectionView cwAvailableLines { get; set; }

		[Outlet]
		AppKit.NSTextField lblBids { get; set; }

		[Outlet]
		AppKit.NSTextField lblCellBack { get; set; }

		[Outlet]
		AppKit.NSTableView tblSelectedLines { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblCellBack != null) {
				lblCellBack.Dispose ();
				lblCellBack = null;
			}

			if (btnCell != null) {
				btnCell.Dispose ();
				btnCell = null;
			}

			if (cwAvailableLines != null) {
				cwAvailableLines.Dispose ();
				cwAvailableLines = null;
			}

			if (tblSelectedLines != null) {
				tblSelectedLines.Dispose ();
				tblSelectedLines = null;
			}

			if (btnDelete != null) {
				btnDelete.Dispose ();
				btnDelete = null;
			}

			if (btnClear != null) {
				btnClear.Dispose ();
				btnClear = null;
			}

			if (lblBids != null) {
				lblBids.Dispose ();
				lblBids = null;
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

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}
		}
	}

	[Register ("BidEditorPilotWindow")]
	partial class BidEditorPilotWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
