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
	[Register ("GetAwardsWindowController")]
	partial class GetAwardsWindowController
	{
		[Outlet]
		AppKit.NSMatrix btnAwards { get; set; }

		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnDomicile { get; set; }

		[Outlet]
		AppKit.NSMatrix btnPosition { get; set; }

		[Outlet]
		AppKit.NSButton btnRetrieve { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnPosition != null) {
				btnPosition.Dispose ();
				btnPosition = null;
			}

			if (btnAwards != null) {
				btnAwards.Dispose ();
				btnAwards = null;
			}

			if (btnDomicile != null) {
				btnDomicile.Dispose ();
				btnDomicile = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnRetrieve != null) {
				btnRetrieve.Dispose ();
				btnRetrieve = null;
			}
		}
	}

	[Register ("GetAwardsWindow")]
	partial class GetAwardsWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
