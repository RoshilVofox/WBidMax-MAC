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
	[Register ("BAgroupWindowController")]
	partial class BAgroupWindowController
	{
		[Outlet]
		AppKit.NSButton btnFFDO { get; set; }

		[Outlet]
		AppKit.NSButton btnPrint { get; set; }

		[Outlet]
		AppKit.NSBox GroupDetailViewBox { get; set; }

		[Outlet]
		AppKit.NSTableView tblGroupDetails { get; set; }

		[Outlet]
		AppKit.NSTableView tblGroups { get; set; }

		[Outlet]
		public AppKit.NSTextView txtPairing { get; private set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnFFDO != null) {
				btnFFDO.Dispose ();
				btnFFDO = null;
			}

			if (btnPrint != null) {
				btnPrint.Dispose ();
				btnPrint = null;
			}

			if (GroupDetailViewBox != null) {
				GroupDetailViewBox.Dispose ();
				GroupDetailViewBox = null;
			}

			if (tblGroups != null) {
				tblGroups.Dispose ();
				tblGroups = null;
			}

			if (txtPairing != null) {
				txtPairing.Dispose ();
				txtPairing = null;
			}

			if (tblGroupDetails != null) {
				tblGroupDetails.Dispose ();
				tblGroupDetails = null;
			}
		}
	}

	[Register ("BAgroupWindow")]
	partial class BAgroupWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
