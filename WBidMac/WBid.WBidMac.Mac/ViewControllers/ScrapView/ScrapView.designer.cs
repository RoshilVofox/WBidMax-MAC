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
	[Register ("ScrapViewController")]
	partial class ScrapViewController
	{
		[Outlet]
		WebKit.WebView scrapWeb1 { get; set; }

		[Outlet]
		WebKit.WebView scrapWeb2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (scrapWeb1 != null) {
				scrapWeb1.Dispose ();
				scrapWeb1 = null;
			}

			if (scrapWeb2 != null) {
				scrapWeb2.Dispose ();
				scrapWeb2 = null;
			}
		}
	}

	[Register ("ScrapView")]
	partial class ScrapView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
