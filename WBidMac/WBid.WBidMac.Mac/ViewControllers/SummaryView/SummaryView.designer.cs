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
	[Register ("SummaryViewController")]
	partial class SummaryViewController
	{
		[Outlet]
		AppKit.NSTableView tblsummary { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblsummary != null) {
				tblsummary.Dispose ();
				tblsummary = null;
			}
		}
	}

	[Register ("SummaryView")]
	partial class SummaryView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
