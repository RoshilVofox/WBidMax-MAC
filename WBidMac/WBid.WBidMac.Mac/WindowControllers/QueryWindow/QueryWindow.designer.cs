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
	[Register ("QueryWindowController")]
	partial class QueryWindowController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnShowBidChoices { get; set; }

		[Outlet]
		AppKit.NSButton btnSubmit { get; set; }

		[Outlet]
		AppKit.NSTextField lblAvoidanceInfo { get; set; }

		[Outlet]
		AppKit.NSTextField lblChoices { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		AppKit.NSTextField txtChoices { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmployee { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnShowBidChoices != null) {
				btnShowBidChoices.Dispose ();
				btnShowBidChoices = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}

			if (lblAvoidanceInfo != null) {
				lblAvoidanceInfo.Dispose ();
				lblAvoidanceInfo = null;
			}

			if (lblChoices != null) {
				lblChoices.Dispose ();
				lblChoices = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (txtChoices != null) {
				txtChoices.Dispose ();
				txtChoices = null;
			}

			if (txtEmployee != null) {
				txtEmployee.Dispose ();
				txtEmployee = null;
			}
		}
	}

	[Register ("QueryWindow")]
	partial class QueryWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
