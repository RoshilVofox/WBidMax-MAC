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
	[Register ("AMPMView")]
	partial class AMPMView
	{
		[Outlet]
		AppKit.NSButton btnAM { get; set; }

		[Outlet]
		AppKit.NSButton btnClose { get; set; }

		[Outlet]
		AppKit.NSButton btnMix { get; set; }

		[Outlet]
		AppKit.NSButton btnPM { get; set; }

		[Action ("funSelection:")]
		partial void funSelection (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAM != null) {
				btnAM.Dispose ();
				btnAM = null;
			}

			if (btnPM != null) {
				btnPM.Dispose ();
				btnPM = null;
			}

			if (btnMix != null) {
				btnMix.Dispose ();
				btnMix = null;
			}

			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}
		}
	}
}
