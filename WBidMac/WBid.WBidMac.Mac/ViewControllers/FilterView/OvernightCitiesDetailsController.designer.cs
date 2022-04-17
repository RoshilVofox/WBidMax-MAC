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
	[Register ("OvernightCitiesDetailsController")]
	partial class OvernightCitiesDetailsController
	{
		[Outlet]
		AppKit.NSButton btnClear { get; set; }

		[Outlet]
		AppKit.NSButton btnDone { get; set; }

		[Outlet]
		AppKit.NSScrollView ScrollViewOvernightCities { get; set; }

		[Outlet]
		AppKit.NSClipView ViewContent { get; set; }

		[Outlet]
		AppKit.NSView ViewOvernightCities { get; set; }

		[Action ("FunCancelAction:")]
		partial void FunCancelAction (Foundation.NSObject sender);

		[Action ("funClearAction:")]
		partial void funClearAction (Foundation.NSObject sender);

		[Action ("funDoneActions:")]
		partial void funDoneActions (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnClear != null) {
				btnClear.Dispose ();
				btnClear = null;
			}

			if (btnDone != null) {
				btnDone.Dispose ();
				btnDone = null;
			}

			if (ScrollViewOvernightCities != null) {
				ScrollViewOvernightCities.Dispose ();
				ScrollViewOvernightCities = null;
			}

			if (ViewContent != null) {
				ViewContent.Dispose ();
				ViewContent = null;
			}

			if (ViewOvernightCities != null) {
				ViewOvernightCities.Dispose ();
				ViewOvernightCities = null;
			}
		}
	}
}
