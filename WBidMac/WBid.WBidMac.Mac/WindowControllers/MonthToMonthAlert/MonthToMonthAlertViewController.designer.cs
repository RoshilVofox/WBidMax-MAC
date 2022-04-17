// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers
{
	[Register ("MonthToMonthAlertViewController")]
	partial class MonthToMonthAlertViewController
	{
		[Outlet]
		AppKit.NSButton btnKink2 { get; set; }

		[Outlet]
		AppKit.NSButton btnLink1 { get; set; }

		[Outlet]
		AppKit.NSButton btnLink3 { get; set; }

		[Outlet]
		AppKit.NSTextField lblAlert { get; set; }

		[Action ("btnLink1tap:")]
		partial void btnLink1tap (Foundation.NSObject sender);

		[Action ("btnLink2Tap:")]
		partial void btnLink2Tap (Foundation.NSObject sender);

		[Action ("btnOkTapped:")]
		partial void btnOkTapped (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnKink2 != null) {
				btnKink2.Dispose ();
				btnKink2 = null;
			}

			if (btnLink1 != null) {
				btnLink1.Dispose ();
				btnLink1 = null;
			}

			if (btnLink3 != null) {
				btnLink3.Dispose ();
				btnLink3 = null;
			}

			if (lblAlert != null) {
				lblAlert.Dispose ();
				lblAlert = null;
			}
		}
	}
}
