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
	[Register ("SecretDataDownloadController")]
	partial class SecretDataDownloadController
	{
		[Outlet]
		AppKit.NSButton btnAllFlts { get; set; }

		[Outlet]
		AppKit.NSButton btnAllPilots { get; set; }

		[Outlet]
		AppKit.NSButton btnDownload { get; set; }

		[Outlet]
		AppKit.NSMatrix btnPositions { get; set; }

		[Outlet]
		AppKit.NSMatrix btnRound { get; set; }

		[Outlet]
		AppKit.NSPopUpButton dropDownMonth { get; set; }

		[Outlet]
		AppKit.NSTableView tblDomiciles { get; set; }

		[Outlet]
		AppKit.NSSecureTextField txtPassword { get; set; }

		[Outlet]
		AppKit.NSTextField txtUserID { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAllFlts != null) {
				btnAllFlts.Dispose ();
				btnAllFlts = null;
			}

			if (btnAllPilots != null) {
				btnAllPilots.Dispose ();
				btnAllPilots = null;
			}

			if (btnDownload != null) {
				btnDownload.Dispose ();
				btnDownload = null;
			}

			if (btnPositions != null) {
				btnPositions.Dispose ();
				btnPositions = null;
			}

			if (btnRound != null) {
				btnRound.Dispose ();
				btnRound = null;
			}

			if (dropDownMonth != null) {
				dropDownMonth.Dispose ();
				dropDownMonth = null;
			}

			if (tblDomiciles != null) {
				tblDomiciles.Dispose ();
				tblDomiciles = null;
			}

			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}

			if (txtUserID != null) {
				txtUserID.Dispose ();
				txtUserID = null;
			}
		}
	}
}
