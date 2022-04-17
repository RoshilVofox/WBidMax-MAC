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
	[Register ("ViewFileWindowController")]
	partial class ViewFileWindowController
	{
		[Outlet]
		AppKit.NSMatrix btnBlankReserve { get; set; }

		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnDomicile { get; set; }

		[Outlet]
		AppKit.NSMatrix btnMonthly { get; set; }

		[Outlet]
		AppKit.NSMatrix btnPosition { get; set; }

		[Outlet]
		AppKit.NSButton btnViewFile { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDomicile != null) {
				btnDomicile.Dispose ();
				btnDomicile = null;
			}

			if (btnPosition != null) {
				btnPosition.Dispose ();
				btnPosition = null;
			}

			if (btnMonthly != null) {
				btnMonthly.Dispose ();
				btnMonthly = null;
			}

			if (btnBlankReserve != null) {
				btnBlankReserve.Dispose ();
				btnBlankReserve = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnViewFile != null) {
				btnViewFile.Dispose ();
				btnViewFile = null;
			}
		}
	}

	[Register ("ViewFileWindow")]
	partial class ViewFileWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
