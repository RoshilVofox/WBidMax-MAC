// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
	[Register ("ContactUsWindowController")]
	partial class ContactUsWindowController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSTextField lblSupport { get; set; }

		[Outlet]
		AppKit.NSTextField lblVersion { get; set; }

		[Outlet]
		AppKit.NSTextView txtDescription { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmail { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmpNum { get; set; }

		[Outlet]
		AppKit.NSTextField txtName { get; set; }

		[Outlet]
		AppKit.NSTextField txtPhoneNo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtEmpNum != null) {
				txtEmpNum.Dispose ();
				txtEmpNum = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (lblSupport != null) {
				lblSupport.Dispose ();
				lblSupport = null;
			}

			if (lblVersion != null) {
				lblVersion.Dispose ();
				lblVersion = null;
			}

			if (txtDescription != null) {
				txtDescription.Dispose ();
				txtDescription = null;
			}

			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}

			if (txtName != null) {
				txtName.Dispose ();
				txtName = null;
			}

			if (txtPhoneNo != null) {
				txtPhoneNo.Dispose ();
				txtPhoneNo = null;
			}
		}
	}

	[Register ("ContactUsWindow")]
	partial class ContactUsWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
