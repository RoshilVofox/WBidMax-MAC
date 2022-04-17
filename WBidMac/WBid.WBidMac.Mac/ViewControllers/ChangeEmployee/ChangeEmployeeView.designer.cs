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
	[Register ("ChangeEmployeeViewController")]
	partial class ChangeEmployeeViewController
	{
		[Outlet]
		AppKit.NSButton btnChange { get; set; }

		[Outlet]
		AppKit.NSButton btnKeepOld { get; set; }

		[Outlet]
		AppKit.NSTextField txtEmployeeNo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtEmployeeNo != null) {
				txtEmployeeNo.Dispose ();
				txtEmployeeNo = null;
			}

			if (btnKeepOld != null) {
				btnKeepOld.Dispose ();
				btnKeepOld = null;
			}

			if (btnChange != null) {
				btnChange.Dispose ();
				btnChange = null;
			}
		}
	}

	[Register ("ChangeEmployeeView")]
	partial class ChangeEmployeeView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
