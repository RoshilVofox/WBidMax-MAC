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
	[Register ("OverlayViewController")]
	partial class OverlayViewController
	{
		[Outlet]
		AppKit.NSTextField lblOverlay { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator pgrsOverlay { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblOverlay != null) {
				lblOverlay.Dispose ();
				lblOverlay = null;
			}

			if (pgrsOverlay != null) {
				pgrsOverlay.Dispose ();
				pgrsOverlay = null;
			}
		}
	}

	[Register ("OverlayView")]
	partial class OverlayView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
