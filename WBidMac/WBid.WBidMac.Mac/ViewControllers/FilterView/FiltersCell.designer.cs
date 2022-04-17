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
	[Register ("FiltersCell")]
	partial class FiltersCell
	{
		[Outlet]
		AppKit.NSButton btnAM { get; set; }

		[Outlet]
		AppKit.NSButton btnClose { get; set; }

		[Outlet]
		AppKit.NSButton btnMix { get; set; }

		[Outlet]
		AppKit.NSButton btnPM { get; set; }

		[Action ("funAPButtonSelection:")]
		partial void funAPButtonSelection (Foundation.NSObject sender);

		[Action ("funCommutableLineAuto131to134:")]
		partial void funCommutableLineAuto131to134 (Foundation.NSObject sender);

		[Action ("funCommuteAutotype101to107:")]
		partial void funCommuteAutotype101to107 (Foundation.NSObject sender);

		[Action ("funDaysOfMonth111to112:")]
		partial void funDaysOfMonth111to112 (Foundation.NSObject sender);

		[Action ("funDaysOfWeekAll:")]
		partial void funDaysOfWeekAll (Foundation.NSObject sender);

		[Action ("FunDeleteFilter:")]
		partial void FunDeleteFilter (Foundation.NSObject sender);

		[Action ("funDHFirstLast:")]
		partial void funDHFirstLast (Foundation.NSObject sender);

		[Action ("funDowSome:")]
		partial void funDowSome (Foundation.NSObject sender);

		[Action ("funEquimentTypetag51To53:")]
		partial void funEquimentTypetag51To53 (Foundation.NSObject sender);

		[Action ("funLinetype:")]
		partial void funLinetype (Foundation.NSObject sender);

		[Action ("funOverNightCities121to122:")]
		partial void funOverNightCities121to122 (Foundation.NSObject sender);

		[Action ("funRestTypetag61To63:")]
		partial void funRestTypetag61To63 (Foundation.NSObject sender);

		[Action ("funSDOWtype71to77:")]
		partial void funSDOWtype71to77 (Foundation.NSObject sender);

		[Action ("funTrpBlkLen81t087:")]
		partial void funTrpBlkLen81t087 (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAM != null) {
				btnAM.Dispose ();
				btnAM = null;
			}

			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}

			if (btnMix != null) {
				btnMix.Dispose ();
				btnMix = null;
			}

			if (btnPM != null) {
				btnPM.Dispose ();
				btnPM = null;
			}
		}
	}
}
