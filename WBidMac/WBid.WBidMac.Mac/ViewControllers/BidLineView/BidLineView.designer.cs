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
	[Register ("BidLineViewController")]
	partial class BidLineViewController
	{
		[Outlet]
		AppKit.NSTableView tblBidLine { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblBidLine != null) {
				tblBidLine.Dispose ();
				tblBidLine = null;
			}
		}
	}

	[Register ("BidLineView")]
	partial class BidLineView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}