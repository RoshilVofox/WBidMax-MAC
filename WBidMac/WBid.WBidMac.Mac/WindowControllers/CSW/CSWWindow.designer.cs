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
	[Register ("CSWWindowController")]
	partial class CSWWindowController
	{
		[Outlet]
		AppKit.NSSegmentedControl sgViewSelect { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (sgViewSelect != null) {
				sgViewSelect.Dispose ();
				sgViewSelect = null;
			}
		}
	}

	[Register ("CSWWindow")]
	partial class CSWWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
