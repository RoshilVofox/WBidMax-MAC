// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSButton btnBA { get; set; }

		[Outlet]
		AppKit.NSButton btnBlueShade { get; set; }

		[Outlet]
		AppKit.NSButton btnBottomLock { get; set; }

		[Outlet]
		AppKit.NSButton btnCSW { get; set; }

		[Outlet]
		AppKit.NSButton btnDrop { get; set; }

		[Outlet]
		AppKit.NSButton btnEOM { get; set; }

		[Outlet]
		AppKit.NSButton btnHome { get; set; }

		[Outlet]
		AppKit.NSMatrix btnLinesPrint { get; set; }

		[Outlet]
		public AppKit.NSButton btnMIL { get; private set; }

		[Outlet]
		AppKit.NSButton btnOverlap { get; set; }

		[Outlet]
		AppKit.NSButton btnPairings { get; set; }

		[Outlet]
		AppKit.NSButton btnPrintCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnPrintOK { get; set; }

		[Outlet]
		AppKit.NSButton btnQuickSet { get; set; }

		[Outlet]
		AppKit.NSButton btnRedo { get; set; }

		[Outlet]
		AppKit.NSButton btnRemBottomLock { get; set; }

		[Outlet]
		AppKit.NSButton btnRemTopLock { get; set; }

		[Outlet]
		AppKit.NSButton btnReparse { get; set; }

		[Outlet]
		AppKit.NSButton btnReset { get; set; }

		[Outlet]
		AppKit.NSButton btnSave { get; set; }

		[Outlet]
		AppKit.NSButton btnSecretDownload { get; set; }

		[Outlet]
		public AppKit.NSButton btnSynch { get; private set; }

		[Outlet]
		AppKit.NSButton btnTest { get; set; }

		[Outlet]
		AppKit.NSButton btnTopLock { get; set; }

		[Outlet]
		AppKit.NSButton btnUndo { get; set; }

		[Outlet]
		AppKit.NSButton btnVacation { get; set; }

		[Outlet]
		AppKit.NSButton btnVacDiff { get; set; }

		[Outlet]
		public AppKit.NSSegmentedControl sgViewSelect { get; private set; }

		[Outlet]
		AppKit.NSButton swSeniority { get; set; }

		[Outlet]
		AppKit.NSButton swUser { get; set; }

		[Outlet]
		AppKit.NSTextField txtGoToLine { get; set; }

		[Outlet]
		AppKit.NSTextField txtLineNoPrint { get; set; }

		[Outlet]
		AppKit.NSTextField txtUser { get; set; }

		[Outlet]
		public AppKit.NSView vwAdmin { get; private set; }

		[Outlet]
		AppKit.NSView vwMain { get; set; }

		[Outlet]
		public AppKit.NSView vwPrintOption { get; private set; }

		[Outlet]
		AppKit.NSView vwToolBar { get; set; }

		[Action ("funBlueShadeFunctionality:")]
		partial void funBlueShadeFunctionality (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBA != null) {
				btnBA.Dispose ();
				btnBA = null;
			}

			if (btnBlueShade != null) {
				btnBlueShade.Dispose ();
				btnBlueShade = null;
			}

			if (btnBottomLock != null) {
				btnBottomLock.Dispose ();
				btnBottomLock = null;
			}

			if (btnCSW != null) {
				btnCSW.Dispose ();
				btnCSW = null;
			}

			if (btnDrop != null) {
				btnDrop.Dispose ();
				btnDrop = null;
			}

			if (btnEOM != null) {
				btnEOM.Dispose ();
				btnEOM = null;
			}

			if (btnHome != null) {
				btnHome.Dispose ();
				btnHome = null;
			}

			if (btnLinesPrint != null) {
				btnLinesPrint.Dispose ();
				btnLinesPrint = null;
			}

			if (btnMIL != null) {
				btnMIL.Dispose ();
				btnMIL = null;
			}

			if (btnOverlap != null) {
				btnOverlap.Dispose ();
				btnOverlap = null;
			}

			if (btnPairings != null) {
				btnPairings.Dispose ();
				btnPairings = null;
			}

			if (btnVacDiff != null) {
				btnVacDiff.Dispose ();
				btnVacDiff = null;
			}

			if (btnPrintCancel != null) {
				btnPrintCancel.Dispose ();
				btnPrintCancel = null;
			}

			if (btnPrintOK != null) {
				btnPrintOK.Dispose ();
				btnPrintOK = null;
			}

			if (btnQuickSet != null) {
				btnQuickSet.Dispose ();
				btnQuickSet = null;
			}

			if (btnRedo != null) {
				btnRedo.Dispose ();
				btnRedo = null;
			}

			if (btnRemBottomLock != null) {
				btnRemBottomLock.Dispose ();
				btnRemBottomLock = null;
			}

			if (btnRemTopLock != null) {
				btnRemTopLock.Dispose ();
				btnRemTopLock = null;
			}

			if (btnReparse != null) {
				btnReparse.Dispose ();
				btnReparse = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}

			if (btnSave != null) {
				btnSave.Dispose ();
				btnSave = null;
			}

			if (btnSecretDownload != null) {
				btnSecretDownload.Dispose ();
				btnSecretDownload = null;
			}

			if (btnSynch != null) {
				btnSynch.Dispose ();
				btnSynch = null;
			}

			if (btnTest != null) {
				btnTest.Dispose ();
				btnTest = null;
			}

			if (btnTopLock != null) {
				btnTopLock.Dispose ();
				btnTopLock = null;
			}

			if (btnUndo != null) {
				btnUndo.Dispose ();
				btnUndo = null;
			}

			if (btnVacation != null) {
				btnVacation.Dispose ();
				btnVacation = null;
			}

			if (sgViewSelect != null) {
				sgViewSelect.Dispose ();
				sgViewSelect = null;
			}

			if (swSeniority != null) {
				swSeniority.Dispose ();
				swSeniority = null;
			}

			if (swUser != null) {
				swUser.Dispose ();
				swUser = null;
			}

			if (txtGoToLine != null) {
				txtGoToLine.Dispose ();
				txtGoToLine = null;
			}

			if (txtLineNoPrint != null) {
				txtLineNoPrint.Dispose ();
				txtLineNoPrint = null;
			}

			if (txtUser != null) {
				txtUser.Dispose ();
				txtUser = null;
			}

			if (vwAdmin != null) {
				vwAdmin.Dispose ();
				vwAdmin = null;
			}

			if (vwMain != null) {
				vwMain.Dispose ();
				vwMain = null;
			}

			if (vwPrintOption != null) {
				vwPrintOption.Dispose ();
				vwPrintOption = null;
			}

			if (vwToolBar != null) {
				vwToolBar.Dispose ();
				vwToolBar = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
