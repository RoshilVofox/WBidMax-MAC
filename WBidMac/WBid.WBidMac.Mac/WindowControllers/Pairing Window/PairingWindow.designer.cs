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
	[Register ("PairingWindowController")]
	partial class PairingWindowController
	{
		[Outlet]
		AppKit.NSButton btnFFDO { get; set; }

		[Outlet]
		AppKit.NSButton btnPrint { get; set; }

		[Outlet]
		public AppKit.NSTableView tblDays { get; private set; }

		[Outlet]
		public AppKit.NSTableView tblTrips { get; private set; }

		[Outlet]
		public AppKit.NSTextView txtPairing { get; private set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnPrint != null) {
				btnPrint.Dispose ();
				btnPrint = null;
			}

			if (tblDays != null) {
				tblDays.Dispose ();
				tblDays = null;
			}

			if (tblTrips != null) {
				tblTrips.Dispose ();
				tblTrips = null;
			}

			if (txtPairing != null) {
				txtPairing.Dispose ();
				txtPairing = null;
			}

			if (btnFFDO != null) {
				btnFFDO.Dispose ();
				btnFFDO = null;
			}
		}
	}

	[Register ("PairingWindow")]
	partial class PairingWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
