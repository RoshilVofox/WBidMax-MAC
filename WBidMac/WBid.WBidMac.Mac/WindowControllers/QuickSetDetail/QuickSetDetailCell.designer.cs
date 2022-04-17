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
	[Register ("QuickSetDetailCell")]
	partial class QuickSetDetailCell
	{
		[Outlet]
		AppKit.NSTextField lblData { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblData != null) {
				lblData.Dispose ();
				lblData = null;
			}
		}
	}
}
