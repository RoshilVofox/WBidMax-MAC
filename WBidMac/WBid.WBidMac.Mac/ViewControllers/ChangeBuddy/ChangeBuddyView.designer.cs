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
	[Register ("ChangeBuddyViewController")]
	partial class ChangeBuddyViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddy1 { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddy2 { get; set; }

		[Outlet]
		AppKit.NSTextField txtBuddy1 { get; set; }

		[Outlet]
		AppKit.NSTextField txtBuddy2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtBuddy1 != null) {
				txtBuddy1.Dispose ();
				txtBuddy1 = null;
			}

			if (txtBuddy2 != null) {
				txtBuddy2.Dispose ();
				txtBuddy2 = null;
			}

			if (lblBuddy1 != null) {
				lblBuddy1.Dispose ();
				lblBuddy1 = null;
			}

			if (lblBuddy2 != null) {
				lblBuddy2.Dispose ();
				lblBuddy2 = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}
		}
	}

	[Register ("ChangeBuddyView")]
	partial class ChangeBuddyView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
