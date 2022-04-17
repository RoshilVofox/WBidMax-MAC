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
	[Register ("RatioViewControllerController")]
	partial class RatioViewControllerController
	{
		[Outlet]
		AppKit.NSButton btnCancelAction { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnDenominator { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnNumerator { get; set; }

		[Outlet]
		AppKit.NSButton btnOkAction { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDenominator != null) {
				btnDenominator.Dispose ();
				btnDenominator = null;
			}

			if (btnNumerator != null) {
				btnNumerator.Dispose ();
				btnNumerator = null;
			}

			if (btnOkAction != null) {
				btnOkAction.Dispose ();
				btnOkAction = null;
			}

			if (btnCancelAction != null) {
				btnCancelAction.Dispose ();
				btnCancelAction = null;
			}
		}
	}
}
