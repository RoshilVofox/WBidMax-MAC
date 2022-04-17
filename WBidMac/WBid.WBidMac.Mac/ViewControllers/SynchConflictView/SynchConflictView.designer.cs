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
	[Register ("SynchConflictViewController")]
	partial class SynchConflictViewController
	{
		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnLocal { get; set; }

		[Outlet]
		AppKit.NSButton btnServer { get; set; }

		[Outlet]
		AppKit.NSTextField lblLocalDate { get; set; }

		[Outlet]
		AppKit.NSTextField lblLocalTime { get; set; }

		[Outlet]
		AppKit.NSTextField lblServerDate { get; set; }

		[Outlet]
		AppKit.NSTextField lblServerTime { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblServerDate != null) {
				lblServerDate.Dispose ();
				lblServerDate = null;
			}

			if (lblServerTime != null) {
				lblServerTime.Dispose ();
				lblServerTime = null;
			}

			if (lblLocalDate != null) {
				lblLocalDate.Dispose ();
				lblLocalDate = null;
			}

			if (lblLocalTime != null) {
				lblLocalTime.Dispose ();
				lblLocalTime = null;
			}

			if (btnServer != null) {
				btnServer.Dispose ();
				btnServer = null;
			}

			if (btnLocal != null) {
				btnLocal.Dispose ();
				btnLocal = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}
		}
	}

	[Register ("SynchConflictView")]
	partial class SynchConflictView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
