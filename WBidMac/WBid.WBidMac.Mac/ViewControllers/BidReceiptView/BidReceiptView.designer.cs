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
	[Register ("BidReceiptViewController")]
	partial class BidReceiptViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSTableView tblBidReceipt { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblBidReceipt != null) {
				tblBidReceipt.Dispose ();
				tblBidReceipt = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}
		}
	}

	[Register ("BidReceiptView")]
	partial class BidReceiptView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
