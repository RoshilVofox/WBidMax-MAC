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
	[Register ("BAWindowController")]
	partial class BAWindowController
	{
		[Outlet]
		AppKit.NSButton btnCalculateBid { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl sgViewSelect { get; set; }

		[Action ("funCalculateBid:")]
		partial void funCalculateBid (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCalculateBid != null) {
				btnCalculateBid.Dispose ();
				btnCalculateBid = null;
			}

			if (sgViewSelect != null) {
				sgViewSelect.Dispose ();
				sgViewSelect = null;
			}
		}
	}

	[Register ("BAWindow")]
	partial class BAWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
