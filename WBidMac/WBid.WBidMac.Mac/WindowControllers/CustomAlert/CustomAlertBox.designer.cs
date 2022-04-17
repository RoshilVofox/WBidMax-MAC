// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.CustomAlert
{
	[Register ("CustomAlertBox")]
	partial class CustomAlertBox
	{
		[Outlet]
		AppKit.NSButton btnOk { get; set; }

		[Outlet]
		AppKit.NSTextField lblMessage { get; set; }

		[Action ("btnOkTap:")]
		partial void btnOkTap (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblMessage != null) {
				lblMessage.Dispose ();
				lblMessage = null;
			}

			if (btnOk != null) {
				btnOk.Dispose ();
				btnOk = null;
			}
		}
	}
}
