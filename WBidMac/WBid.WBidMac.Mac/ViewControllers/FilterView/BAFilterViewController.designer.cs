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
	[Register ("BAFilterViewController")]
	partial class BAFilterViewController
	{
		[Outlet]
		AppKit.NSButton btnClear { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnPopUp { get; set; }

		[Outlet]
		AppKit.NSScrollView scrolView { get; set; }

		[Outlet]
		AppKit.NSTableView tblFilters { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnClear != null) {
				btnClear.Dispose ();
				btnClear = null;
			}

			if (btnPopUp != null) {
				btnPopUp.Dispose ();
				btnPopUp = null;
			}

			if (scrolView != null) {
				scrolView.Dispose ();
				scrolView = null;
			}

			if (tblFilters != null) {
				tblFilters.Dispose ();
				tblFilters = null;
			}
		}
	}
}
