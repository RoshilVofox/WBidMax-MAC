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
	[Register ("ArrivalDepartCell")]
	partial class ArrivalDepartCell
	{
		[Outlet]
		AppKit.NSTextField lblArrival { get; set; }

		[Outlet]
		AppKit.NSTextField lblDay { get; set; }

		[Outlet]
		AppKit.NSTextField lblDepart { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblArrival != null) {
				lblArrival.Dispose ();
				lblArrival = null;
			}

			if (lblDepart != null) {
				lblDepart.Dispose ();
				lblDepart = null;
			}

			if (lblDay != null) {
				lblDay.Dispose ();
				lblDay = null;
			}
		}
	}
}
