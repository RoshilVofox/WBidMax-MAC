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
	[Register ("CalendarWindowController")]
	partial class CalendarWindowController
	{
		[Outlet]
		AppKit.NSArrayController arrCalendarList { get; set; }

		[Outlet]
		AppKit.NSButton btnBottomLock { get; set; }

		[Outlet]
		AppKit.NSButton btnCalendar { get; set; }

		[Outlet]
		AppKit.NSButton btnExportOutlook { get; set; }

		[Outlet]
		AppKit.NSButton btnFFDO { get; set; }

		[Outlet]
		AppKit.NSButton btnMoveDown { get; set; }

		[Outlet]
		AppKit.NSButton btnMoveUp { get; set; }

		[Outlet]
		AppKit.NSToolbarItem btnPrint { get; set; }

		[Outlet]
		AppKit.NSButton btnTopLock { get; set; }

		[Outlet]
		AppKit.NSBox bxCal { get; set; }

		[Outlet]
		AppKit.NSBox bxLineLayer { get; set; }

		[Outlet]
		AppKit.NSTextField lblPrintTitle { get; set; }

		[Outlet]
		AppKit.NSCollectionView vwCalendar { get; set; }

		[Action ("btnExportOutlookClicked:")]
		partial void btnExportOutlookClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnExportOutlook != null) {
				btnExportOutlook.Dispose ();
				btnExportOutlook = null;
			}

			if (arrCalendarList != null) {
				arrCalendarList.Dispose ();
				arrCalendarList = null;
			}

			if (btnBottomLock != null) {
				btnBottomLock.Dispose ();
				btnBottomLock = null;
			}

			if (btnCalendar != null) {
				btnCalendar.Dispose ();
				btnCalendar = null;
			}

			if (btnFFDO != null) {
				btnFFDO.Dispose ();
				btnFFDO = null;
			}

			if (btnMoveDown != null) {
				btnMoveDown.Dispose ();
				btnMoveDown = null;
			}

			if (btnMoveUp != null) {
				btnMoveUp.Dispose ();
				btnMoveUp = null;
			}

			if (btnPrint != null) {
				btnPrint.Dispose ();
				btnPrint = null;
			}

			if (btnTopLock != null) {
				btnTopLock.Dispose ();
				btnTopLock = null;
			}

			if (bxCal != null) {
				bxCal.Dispose ();
				bxCal = null;
			}

			if (bxLineLayer != null) {
				bxLineLayer.Dispose ();
				bxLineLayer = null;
			}

			if (lblPrintTitle != null) {
				lblPrintTitle.Dispose ();
				lblPrintTitle = null;
			}

			if (vwCalendar != null) {
				vwCalendar.Dispose ();
				vwCalendar = null;
			}
		}
	}

	[Register ("CalendarWindow")]
	partial class CalendarWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
