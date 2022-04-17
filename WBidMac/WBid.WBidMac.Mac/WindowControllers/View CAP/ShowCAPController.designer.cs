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
	[Register ("ShowCAPController")]
	partial class ShowCAPController
	{
		[Outlet]
		AppKit.NSTextField lblCurrMonth { get; set; }

		[Outlet]
		AppKit.NSTextField lblPrevMonth { get; set; }

		[Outlet]
		AppKit.NSTableView tblShowCAP { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblCurrMonth != null) {
				lblCurrMonth.Dispose ();
				lblCurrMonth = null;
			}

			if (lblPrevMonth != null) {
				lblPrevMonth.Dispose ();
				lblPrevMonth = null;
			}

			if (tblShowCAP != null) {
				tblShowCAP.Dispose ();
				tblShowCAP = null;
			}
		}
	}
}
