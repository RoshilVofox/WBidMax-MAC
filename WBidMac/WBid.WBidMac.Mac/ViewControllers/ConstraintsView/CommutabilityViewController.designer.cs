// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
	[Register ("CommutabilityViewController")]
	partial class CommutabilityViewController
	{
		[Outlet]
		AppKit.NSButton btnDone { get; set; }

		[Outlet]
		AppKit.NSButton btnNonStop { get; set; }

		[Outlet]
		AppKit.NSButton btnViewArrival { get; set; }

		[Outlet]
		AppKit.NSPopUpButton popUpBackTobase { get; set; }

		[Outlet]
		AppKit.NSPopUpButton popUpCheckInTime { get; set; }

		[Outlet]
		AppKit.NSPopUpButton popUpCommuteCity { get; set; }

		[Outlet]
		AppKit.NSPopUpButton popUpConnectTime { get; set; }

		[Action ("btnNonStopClicked:")]
		partial void btnNonStopClicked (AppKit.NSButton sender);

		[Action ("funCancelAction:")]
		partial void funCancelAction (Foundation.NSObject sender);

		[Action ("funDoneAction:")]
		partial void funDoneAction (Foundation.NSObject sender);

		[Action ("funViewArrivalAnddepartTimeAction:")]
		partial void funViewArrivalAnddepartTimeAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnDone != null) {
				btnDone.Dispose ();
				btnDone = null;
			}

			if (btnViewArrival != null) {
				btnViewArrival.Dispose ();
				btnViewArrival = null;
			}

			if (popUpBackTobase != null) {
				popUpBackTobase.Dispose ();
				popUpBackTobase = null;
			}

			if (popUpCheckInTime != null) {
				popUpCheckInTime.Dispose ();
				popUpCheckInTime = null;
			}

			if (popUpCommuteCity != null) {
				popUpCommuteCity.Dispose ();
				popUpCommuteCity = null;
			}

			if (btnNonStop != null) {
				btnNonStop.Dispose ();
				btnNonStop = null;
			}

			if (popUpConnectTime != null) {
				popUpConnectTime.Dispose ();
				popUpConnectTime = null;
			}
		}
	}
}
