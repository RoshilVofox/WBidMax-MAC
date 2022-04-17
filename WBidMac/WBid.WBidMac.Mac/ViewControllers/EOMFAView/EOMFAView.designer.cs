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
	[Register ("EOMFAViewController")]
	partial class EOMFAViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSMatrix btnVacOptions { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnVacOptions != null) {
				btnVacOptions.Dispose ();
				btnVacOptions = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}
		}
	}

	[Register ("EOMFAView")]
	partial class EOMFAView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
