// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.UserLogin
{

	[Register ("UserLogin")]
	partial class UserLogin
	{
		[Outlet]
		AppKit.NSButtonCell CancelButton { get; set; }

		[Outlet]
		AppKit.NSButtonCell LoginButton { get; set; }

		[Outlet]
		AppKit.NSSecureTextField PasswordTextField { get; set; }

		[Outlet]
		AppKit.NSTextField UserNameTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (UserNameTextField != null) {
				UserNameTextField.Dispose ();
				UserNameTextField = null;
			}

			if (PasswordTextField != null) {
				PasswordTextField.Dispose ();
				PasswordTextField = null;
			}

			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}
		}
	}
}
