// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.initialLogin
{
	[Register ("InitialLoginWindowController")]
	partial class InitialLoginWindowController
	{
		[Outlet]
		AppKit.NSButton cancelButton { get; set; }

		[Outlet]
		AppKit.NSButton loginButton { get; set; }

		[Outlet]
		AppKit.NSSecureTextField passswordText { get; set; }

		[Outlet]
		AppKit.NSTextField userIdText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (userIdText != null) {
				userIdText.Dispose ();
				userIdText = null;
			}

			if (passswordText != null) {
				passswordText.Dispose ();
				passswordText = null;
			}

			if (loginButton != null) {
				loginButton.Dispose ();
				loginButton = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}
		}
	}
}
