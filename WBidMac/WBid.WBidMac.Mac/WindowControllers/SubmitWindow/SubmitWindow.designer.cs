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
	[Register ("SubmitWindowController")]
	partial class SubmitWindowController
	{
		[Outlet]
		AppKit.NSMatrix btnAvoidance { get; set; }

		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeAvoidance { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeEmp { get; set; }

		[Outlet]
		AppKit.NSButton btnSubmit { get; set; }

		[Outlet]
		AppKit.NSMatrix btnSubmitType { get; set; }

		[Outlet]
		AppKit.NSTextField lblAvoidanceBid { get; set; }

		[Outlet]
		AppKit.NSForm txtSeniorityNo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAvoidance != null) {
				btnAvoidance.Dispose ();
				btnAvoidance = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnChangeAvoidance != null) {
				btnChangeAvoidance.Dispose ();
				btnChangeAvoidance = null;
			}

			if (btnChangeEmp != null) {
				btnChangeEmp.Dispose ();
				btnChangeEmp = null;
			}

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}

			if (btnSubmitType != null) {
				btnSubmitType.Dispose ();
				btnSubmitType = null;
			}

			if (lblAvoidanceBid != null) {
				lblAvoidanceBid.Dispose ();
				lblAvoidanceBid = null;
			}

			if (txtSeniorityNo != null) {
				txtSeniorityNo.Dispose ();
				txtSeniorityNo = null;
			}
		}
	}

	[Register ("SubmitWindow")]
	partial class SubmitWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
