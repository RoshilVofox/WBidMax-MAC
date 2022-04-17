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
	[Register ("HelpWindowController")]
	partial class HelpWindowController
	{
		[Outlet]
		public PdfKit.PdfView pdfDocView { get; set; }

		[Outlet]
		public AppKit.NSTableView tblDocument { get; set; }

		[Outlet]
		AppKit.NSTableView tblVideo { get; set; }

		[Outlet]
		AppKit.NSTabView tbView { get; set; }

		[Outlet]
		public WebKit.WebView webVideo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (pdfDocView != null) {
				pdfDocView.Dispose ();
				pdfDocView = null;
			}

			if (tblDocument != null) {
				tblDocument.Dispose ();
				tblDocument = null;
			}

			if (tblVideo != null) {
				tblVideo.Dispose ();
				tblVideo = null;
			}

			if (tbView != null) {
				tbView.Dispose ();
				tbView = null;
			}

			if (webVideo != null) {
				webVideo.Dispose ();
				webVideo = null;
			}
		}
	}

	[Register ("HelpWindow")]
	partial class HelpWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
