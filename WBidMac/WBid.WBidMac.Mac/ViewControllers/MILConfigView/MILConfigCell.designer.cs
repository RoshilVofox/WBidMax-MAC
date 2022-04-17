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
	[Register ("MILConfigCell")]
	partial class MILConfigCell
	{
		[Outlet]
		AppKit.NSButton btnClose { get; set; }

		[Outlet]
		AppKit.NSButton btnEndDrop { get; set; }

		[Outlet]
		AppKit.NSButton btnStartDrop { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpEndDate { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpStartDate { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (dpStartDate != null) {
				dpStartDate.Dispose ();
				dpStartDate = null;
			}

			if (dpEndDate != null) {
				dpEndDate.Dispose ();
				dpEndDate = null;
			}

			if (btnStartDrop != null) {
				btnStartDrop.Dispose ();
				btnStartDrop = null;
			}

			if (btnEndDrop != null) {
				btnEndDrop.Dispose ();
				btnEndDrop = null;
			}

			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}
		}
	}
}
