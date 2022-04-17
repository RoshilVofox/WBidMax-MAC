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
	[Register ("BidLineCell")]
	partial class BidLineCell
	{
		[Outlet]
		public AppKit.NSBox bxCalData { get; private set; }

		[Outlet]
		public AppKit.NSBox bxProperty { get; private set; }

		[Outlet]
		AppKit.NSImageView imgConstraint { get; set; }

		[Outlet]
		AppKit.NSImageView imgLock { get; set; }

		[Outlet]
		AppKit.NSImageView imgOverlap { get; set; }

		[Outlet]
		AppKit.NSTextField lblBack { get; set; }

		[Outlet]
		AppKit.NSTextField lblLine { get; set; }

		[Outlet]
		AppKit.NSTextField lblOrder { get; set; }

		[Outlet]
		AppKit.NSTextField lblPairingDesc { get; set; }

		[Outlet]
		AppKit.NSTextField lblPosition { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (bxCalData != null) {
				bxCalData.Dispose ();
				bxCalData = null;
			}

			if (bxProperty != null) {
				bxProperty.Dispose ();
				bxProperty = null;
			}

			if (imgConstraint != null) {
				imgConstraint.Dispose ();
				imgConstraint = null;
			}

			if (imgLock != null) {
				imgLock.Dispose ();
				imgLock = null;
			}

			if (imgOverlap != null) {
				imgOverlap.Dispose ();
				imgOverlap = null;
			}

			if (lblBack != null) {
				lblBack.Dispose ();
				lblBack = null;
			}

			if (lblLine != null) {
				lblLine.Dispose ();
				lblLine = null;
			}

			if (lblOrder != null) {
				lblOrder.Dispose ();
				lblOrder = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

			if (lblPairingDesc != null) {
				lblPairingDesc.Dispose ();
				lblPairingDesc = null;
			}
		}
	}
}
