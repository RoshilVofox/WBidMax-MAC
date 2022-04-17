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
	[Register ("FileWindowController")]
	partial class FileWindowController
	{
		[Outlet]
		PdfKit.PdfView pdfFileView { get; set; }

		[Outlet]
		AppKit.NSTextView  txtFileView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (txtFileView != null) {
				txtFileView.Dispose ();
				txtFileView = null;
			}

			if (pdfFileView != null) {
				pdfFileView.Dispose ();
				pdfFileView = null;
			}
		}
	}

	[Register ("FileWindow")]
	partial class FileWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
