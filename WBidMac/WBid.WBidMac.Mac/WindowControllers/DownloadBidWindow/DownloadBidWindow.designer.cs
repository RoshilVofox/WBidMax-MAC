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
	[Register ("DownloadBidWindowController")]
	partial class DownloadBidWindowController
	{
		[Outlet]
		AppKit.NSButton btnDone { get; set; }

		[Outlet]
		AppKit.NSButton btnVacLater { get; set; }

		[Outlet]
		AppKit.NSMatrix ckDownloadSteps { get; set; }

		[Outlet]
		AppKit.NSTextField lblMessage1 { get; set; }

		[Outlet]
		AppKit.NSTextField lblMessage2 { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator progDownload { get; set; }

		[Outlet]
		AppKit.NSTextField txtVANumber { get; set; }

		[Outlet]
		AppKit.NSView vwVacOverlap { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ckDownloadSteps != null) {
				ckDownloadSteps.Dispose ();
				ckDownloadSteps = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (progDownload != null) {
				progDownload.Dispose ();
				progDownload = null;
			}

			if (vwVacOverlap != null) {
				vwVacOverlap.Dispose ();
				vwVacOverlap = null;
			}

			if (lblMessage1 != null) {
				lblMessage1.Dispose ();
				lblMessage1 = null;
			}

			if (lblMessage2 != null) {
				lblMessage2.Dispose ();
				lblMessage2 = null;
			}

			if (txtVANumber != null) {
				txtVANumber.Dispose ();
				txtVANumber = null;
			}

			if (btnDone != null) {
				btnDone.Dispose ();
				btnDone = null;
			}

			if (btnVacLater != null) {
				btnVacLater.Dispose ();
				btnVacLater = null;
			}
		}
	}

	[Register ("DownloadBidWindow")]
	partial class DownloadBidWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
