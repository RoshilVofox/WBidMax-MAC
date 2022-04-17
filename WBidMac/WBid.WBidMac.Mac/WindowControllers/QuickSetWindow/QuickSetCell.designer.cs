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
	[Register ("QuickSetCell")]
	partial class QuickSetCell
	{
		[Outlet]
		AppKit.NSButton btnColumnsHelp { get; set; }

		[Outlet]
		AppKit.NSButton btnCSWHelp { get; set; }

		[Outlet]
		AppKit.NSTextField lblColumnsName { get; set; }

		[Outlet]
		AppKit.NSTextField lblCSWName { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblCSWName != null) {
				lblCSWName.Dispose ();
				lblCSWName = null;
			}

			if (btnCSWHelp != null) {
				btnCSWHelp.Dispose ();
				btnCSWHelp = null;
			}

			if (lblColumnsName != null) {
				lblColumnsName.Dispose ();
				lblColumnsName = null;
			}

			if (btnColumnsHelp != null) {
				btnColumnsHelp.Dispose ();
				btnColumnsHelp = null;
			}
		}
	}
}
