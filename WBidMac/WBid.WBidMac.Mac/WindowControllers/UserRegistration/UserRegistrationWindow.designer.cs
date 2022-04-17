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
	[Register ("UserRegistrationWindowController")]
	partial class UserRegistrationWindowController
	{
		[Outlet]
		AppKit.NSButton btnAccept { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnDomicle { get; set; }

		[Outlet]
		AppKit.NSMatrix btnGender { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnPosition { get; set; }

		[Outlet]
		AppKit.NSButton btnRePass { get; set; }

		[Outlet]
		AppKit.NSButton btnSubmit { get; set; }

		[Outlet]
		AppKit.NSButton ckAutoSave { get; set; }

		[Outlet]
		AppKit.NSButton ckBidReceipt { get; set; }

		[Outlet]
		AppKit.NSButton ckCrashReports { get; set; }

		[Outlet]
		AppKit.NSButton ckSmartSync { get; set; }

		[Outlet]
		AppKit.NSPopUpButton popSaveTime { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmail { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmployee { get; set; }

		[Outlet]
		AppKit.NSTextField txtFirstName { get; set; }

		[Outlet]
		AppKit.NSTextField txtLastName { get; set; }

		[Outlet]
		AppKit.NSTextField txtRePass { get; set; }

		[Outlet]
		AppKit.NSView vwRePass { get; set; }

		[Outlet]
		AppKit.NSView vwUserManagement { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (vwRePass != null) {
				vwRePass.Dispose ();
				vwRePass = null;
			}

			if (txtRePass != null) {
				txtRePass.Dispose ();
				txtRePass = null;
			}

			if (btnRePass != null) {
				btnRePass.Dispose ();
				btnRePass = null;
			}

			if (btnAccept != null) {
				btnAccept.Dispose ();
				btnAccept = null;
			}

			if (btnDomicle != null) {
				btnDomicle.Dispose ();
				btnDomicle = null;
			}

			if (btnGender != null) {
				btnGender.Dispose ();
				btnGender = null;
			}

			if (btnPosition != null) {
				btnPosition.Dispose ();
				btnPosition = null;
			}

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}

			if (ckAutoSave != null) {
				ckAutoSave.Dispose ();
				ckAutoSave = null;
			}

			if (ckBidReceipt != null) {
				ckBidReceipt.Dispose ();
				ckBidReceipt = null;
			}

			if (ckCrashReports != null) {
				ckCrashReports.Dispose ();
				ckCrashReports = null;
			}

			if (ckSmartSync != null) {
				ckSmartSync.Dispose ();
				ckSmartSync = null;
			}

			if (popSaveTime != null) {
				popSaveTime.Dispose ();
				popSaveTime = null;
			}

			if (txtEmail != null) {
				txtEmail.Dispose ();
				txtEmail = null;
			}

			if (txtEmployee != null) {
				txtEmployee.Dispose ();
				txtEmployee = null;
			}

			if (txtFirstName != null) {
				txtFirstName.Dispose ();
				txtFirstName = null;
			}

			if (txtLastName != null) {
				txtLastName.Dispose ();
				txtLastName = null;
			}

			if (vwUserManagement != null) {
				vwUserManagement.Dispose ();
				vwUserManagement = null;
			}
		}
	}

	[Register ("UserRegistrationWindow")]
	partial class UserRegistrationWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
