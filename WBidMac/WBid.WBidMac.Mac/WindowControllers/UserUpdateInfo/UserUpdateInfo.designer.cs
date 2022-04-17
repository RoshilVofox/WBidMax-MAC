// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo
{
	[Register ("UserUpdateInfoController")]
	partial class UserUpdateInfoController
	{
		[Outlet]
		AppKit.NSButton AcceptCheckButton { get; set; }

		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		AppKit.NSTextField CellPhoneTextField { get; set; }

		[Outlet]
		AppKit.NSButton ChangeEmplyeeNumButton { get; set; }

		[Outlet]
		AppKit.NSTextField EmailTextField { get; set; }

		[Outlet]
		AppKit.NSTextField EmployeeNumberTextField { get; set; }

		[Outlet]
		AppKit.NSTextField FirstNameTextField { get; set; }

		[Outlet]
		AppKit.NSTextField LastNameTextField { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl PositionSegmentControl { get; set; }

		[Outlet]
		AppKit.NSButton UpdateButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AcceptCheckButton != null) {
				AcceptCheckButton.Dispose ();
				AcceptCheckButton = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (CellPhoneTextField != null) {
				CellPhoneTextField.Dispose ();
				CellPhoneTextField = null;
			}

			if (ChangeEmplyeeNumButton != null) {
				ChangeEmplyeeNumButton.Dispose ();
				ChangeEmplyeeNumButton = null;
			}

			if (EmailTextField != null) {
				EmailTextField.Dispose ();
				EmailTextField = null;
			}

			if (EmployeeNumberTextField != null) {
				EmployeeNumberTextField.Dispose ();
				EmployeeNumberTextField = null;
			}

			if (FirstNameTextField != null) {
				FirstNameTextField.Dispose ();
				FirstNameTextField = null;
			}

			if (LastNameTextField != null) {
				LastNameTextField.Dispose ();
				LastNameTextField = null;
			}

			if (PositionSegmentControl != null) {
				PositionSegmentControl.Dispose ();
				PositionSegmentControl = null;
			}

			if (UpdateButton != null) {
				UpdateButton.Dispose ();
				UpdateButton = null;
			}
		}
	}

	[Register ("UserUpdateInfo")]
	partial class UserUpdateInfo
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
