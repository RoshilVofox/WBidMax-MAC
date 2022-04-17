using Foundation;

namespace WBid.WBidMac.Mac
{
	[Register("LoginWindowController")]
	partial class LoginWindowController
	{
		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		AppKit.NSButton LoginButton { get; set; }

		[Outlet]
		AppKit.NSSecureTextField PasswordTextField { get; set; }

		[Outlet]
		AppKit.NSTextField UserIdTextField { get; set; }

		[Action("CancelBtnTapped:")]
		partial void CancelBtnTapped(Foundation.NSObject sender);

		[Action("LoginBtnTapped:")]
		partial void LoginBtnTapped(Foundation.NSObject sender);

		void ReleaseDesignerOutlets()
		{
			if (CancelButton != null)
			{
				CancelButton.Dispose();
				CancelButton = null;
			}

			if (LoginButton != null)
			{
				LoginButton.Dispose();
				LoginButton = null;
			}

			if (PasswordTextField != null)
			{
				PasswordTextField.Dispose();
				PasswordTextField = null;
			}

			if (UserIdTextField != null)
			{
				UserIdTextField.Dispose();
				UserIdTextField = null;
			}
		}
	}
	// Should subclass MonoMac.AppKit.NSWindow
	[Register("LoginWindow")]
    public partial class LoginWindow
    {
    }
}
