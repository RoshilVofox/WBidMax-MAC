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
	[Register ("QuickSetWindowController")]
	partial class QuickSetWindowController
	{
		[Outlet]
		AppKit.NSButton btnColumnDelete { get; set; }

		[Outlet]
		AppKit.NSButton btnColumnNew { get; set; }

		[Outlet]
		AppKit.NSButton btnColumnUse { get; set; }

		[Outlet]
		AppKit.NSButton btnCSWDelete { get; set; }

		[Outlet]
		AppKit.NSButton btnCSWNew { get; set; }

		[Outlet]
		AppKit.NSButton btnCSWUse { get; set; }

		[Outlet]
		AppKit.NSButton btnNewCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnNewOK { get; set; }

		[Outlet]
		AppKit.NSTextField lblNewName { get; set; }

		[Outlet]
		AppKit.NSTableView tblColumnsQuickSets { get; set; }

		[Outlet]
		AppKit.NSTableView tblCSWQuickSets { get; set; }

		[Outlet]
		AppKit.NSTextField txtNewName { get; set; }

		[Outlet]
		AppKit.NSView vwNewSet { get; set; }

		[Outlet]
		AppKit.NSTabView vwTabQuickSets { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (vwTabQuickSets != null) {
				vwTabQuickSets.Dispose ();
				vwTabQuickSets = null;
			}

			if (vwNewSet != null) {
				vwNewSet.Dispose ();
				vwNewSet = null;
			}

			if (txtNewName != null) {
				txtNewName.Dispose ();
				txtNewName = null;
			}

			if (btnNewOK != null) {
				btnNewOK.Dispose ();
				btnNewOK = null;
			}

			if (btnNewCancel != null) {
				btnNewCancel.Dispose ();
				btnNewCancel = null;
			}

			if (btnCSWNew != null) {
				btnCSWNew.Dispose ();
				btnCSWNew = null;
			}

			if (btnCSWUse != null) {
				btnCSWUse.Dispose ();
				btnCSWUse = null;
			}

			if (btnCSWDelete != null) {
				btnCSWDelete.Dispose ();
				btnCSWDelete = null;
			}

			if (tblCSWQuickSets != null) {
				tblCSWQuickSets.Dispose ();
				tblCSWQuickSets = null;
			}

			if (tblColumnsQuickSets != null) {
				tblColumnsQuickSets.Dispose ();
				tblColumnsQuickSets = null;
			}

			if (btnColumnNew != null) {
				btnColumnNew.Dispose ();
				btnColumnNew = null;
			}

			if (btnColumnUse != null) {
				btnColumnUse.Dispose ();
				btnColumnUse = null;
			}

			if (btnColumnDelete != null) {
				btnColumnDelete.Dispose ();
				btnColumnDelete = null;
			}

			if (lblNewName != null) {
				lblNewName.Dispose ();
				lblNewName = null;
			}
		}
	}

	[Register ("QuickSetWindow")]
	partial class QuickSetWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
