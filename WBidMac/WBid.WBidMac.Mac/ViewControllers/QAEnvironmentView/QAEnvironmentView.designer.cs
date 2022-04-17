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
	[Register ("QAEnvironmentViewController")]
	partial class QAEnvironmentViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSMatrix btnQAOptions { get; set; }

		[Outlet]
		AppKit.NSMatrix btnServerOptions { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (btnQAOptions != null) {
				btnQAOptions.Dispose ();
				btnQAOptions = null;
			}

			if (btnServerOptions != null) {
				btnServerOptions.Dispose ();
				btnServerOptions = null;
			}
		}
	}

	[Register ("QAEnvironmentView")]
	partial class QAEnvironmentView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
