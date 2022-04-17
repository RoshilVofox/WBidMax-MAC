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
	[Register ("ChangeAvoidanceViewController")]
	partial class ChangeAvoidanceViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSForm txtAvoidanceBidChoice { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtAvoidanceBidChoice != null) {
				txtAvoidanceBidChoice.Dispose ();
				txtAvoidanceBidChoice = null;
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

	[Register ("ChangeAvoidanceView")]
	partial class ChangeAvoidanceView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
