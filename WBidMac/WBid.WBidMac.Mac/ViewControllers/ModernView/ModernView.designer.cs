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
	[Register ("ModernViewController")]
	partial class ModernViewController
	{
		[Outlet]
		AppKit.NSTableView tblModern { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tblModern != null) {
				tblModern.Dispose ();
				tblModern = null;
			}
		}
	}

	[Register ("ModernView")]
	partial class ModernView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
