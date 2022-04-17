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
	[Register ("ContractorEmpScrapController")]
	partial class ContractorEmpScrapController
	{
		[Outlet]
		WebKit.WebView WebBrowser1 { get; set; }

		[Outlet]
		WebKit.WebView WebBrowser2 { get; set; }

		[Outlet]
		WebKit.WebView WebBrowser3 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (WebBrowser1 != null) {
				WebBrowser1.Dispose ();
				WebBrowser1 = null;
			}

			if (WebBrowser2 != null) {
				WebBrowser2.Dispose ();
				WebBrowser2 = null;
			}

			if (WebBrowser3 != null) {
				WebBrowser3.Dispose ();
				WebBrowser3 = null;
			}
		}
	}
}
