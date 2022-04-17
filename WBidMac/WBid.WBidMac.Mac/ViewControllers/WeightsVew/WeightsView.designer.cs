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
	[Register ("WeightsViewController")]
	partial class WeightsViewController
	{
		[Outlet]
		AppKit.NSPopUpButton btnAddWeights { get; set; }

		[Outlet]
		AppKit.NSButton btnClear { get; set; }

		[Outlet]
		AppKit.NSButton btnHelpWt { get; set; }

		[Outlet]
		public AppKit.NSTableView tblWeights { get; private set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAddWeights != null) {
				btnAddWeights.Dispose ();
				btnAddWeights = null;
			}

			if (btnClear != null) {
				btnClear.Dispose ();
				btnClear = null;
			}

			if (tblWeights != null) {
				tblWeights.Dispose ();
				tblWeights = null;
			}

			if (btnHelpWt != null) {
				btnHelpWt.Dispose ();
				btnHelpWt = null;
			}
		}
	}

	[Register ("WeightsView")]
	partial class WeightsView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
