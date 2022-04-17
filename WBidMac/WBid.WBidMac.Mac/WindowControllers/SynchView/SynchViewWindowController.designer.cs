// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.SynchView
{
	[Register ("SynchViewWindowController")]
	partial class SynchViewWindowController
	{
		[Outlet]
		AppKit.NSButton bothOption { get; set; }

		[Outlet]
		AppKit.NSButton quickOption { get; set; }

		[Outlet]
		AppKit.NSButton stateOption { get; set; }

		[Action ("bothSelectionAction:")]
		partial void bothSelectionAction (Foundation.NSObject sender);

		[Action ("CancelAction:")]
		partial void CancelAction (Foundation.NSObject sender);

		[Action ("OkAction:")]
		partial void OkAction (Foundation.NSObject sender);

		[Action ("quicksetSelectAction:")]
		partial void quicksetSelectAction (Foundation.NSObject sender);

		[Action ("StateSelectionAction:")]
		partial void StateSelectionAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (stateOption != null) {
				stateOption.Dispose ();
				stateOption = null;
			}

			if (quickOption != null) {
				quickOption.Dispose ();
				quickOption = null;
			}

			if (bothOption != null) {
				bothOption.Dispose ();
				bothOption = null;
			}
		}
	}
}
