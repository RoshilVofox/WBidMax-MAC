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
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnLogin { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmployee { get; set; }

		[Outlet]
		AppKit.NSSecureTextField txtPassword { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtEmployee != null) {
				txtEmployee.Dispose ();
				txtEmployee = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}

			if (btnLogin != null) {
				btnLogin.Dispose ();
				btnLogin = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}
		}
	}

	[Register ("LoginView")]
	partial class LoginView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
