// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.UserAccoutDifference
{
	[Register ("UserAccountDifferenceController")]
	partial class UserAccountDifferenceController
	{
		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		AppKit.NSButton UpdateButton { get; set; }

		[Outlet]
		AppKit.NSTableView updateInfoTableView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (UpdateButton != null) {
				UpdateButton.Dispose ();
				UpdateButton = null;
			}

			if (updateInfoTableView != null) {
				updateInfoTableView.Dispose ();
				updateInfoTableView = null;
			}
		}
	}
}
