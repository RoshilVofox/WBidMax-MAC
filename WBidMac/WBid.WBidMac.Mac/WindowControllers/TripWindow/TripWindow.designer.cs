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
	[Register ("TripWindowController")]
	partial class TripWindowController
	{
		[Outlet]
		AppKit.NSButton btnExportToOutlook { get; set; }

		[Outlet]
		AppKit.NSButton btnFFDO { get; set; }

		[Outlet]
		AppKit.NSButton btnPrint { get; set; }

		[Outlet]
		AppKit.NSTableView tblTrip { get; set; }

		[Action ("ExportToOutLookFunctionality:")]
		partial void ExportToOutLookFunctionality (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnExportToOutlook != null) {
				btnExportToOutlook.Dispose ();
				btnExportToOutlook = null;
			}

			if (btnFFDO != null) {
				btnFFDO.Dispose ();
				btnFFDO = null;
			}

			if (btnPrint != null) {
				btnPrint.Dispose ();
				btnPrint = null;
			}

			if (tblTrip != null) {
				tblTrip.Dispose ();
				tblTrip = null;
			}
		}
	}

	[Register ("TripWindow")]
	partial class TripWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
