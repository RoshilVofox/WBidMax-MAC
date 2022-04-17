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
	[Register ("ModernCell")]
	partial class ModernCell
	{
		[Outlet]
		AppKit.NSColorWell borderView { get; set; }

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
		AppKit.NSTextField lblLine { get; set; }

		[Outlet]
		AppKit.NSTextField lblLineBack { get; set; }

		[Outlet]
		AppKit.NSTextField lblOrder { get; set; }

		[Outlet]
		AppKit.NSTextField lblPosition { get; set; }

		[Outlet]
		AppKit.NSTextField lblPropBack { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (borderView != null) {
				borderView.Dispose ();
				borderView = null;
			}

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

			if (lblLine != null) {
				lblLine.Dispose ();
				lblLine = null;
			}

			if (lblLineBack != null) {
				lblLineBack.Dispose ();
				lblLineBack = null;
			}

			if (lblOrder != null) {
				lblOrder.Dispose ();
				lblOrder = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

			if (lblPropBack != null) {
				lblPropBack.Dispose ();
				lblPropBack = null;
			}
		}
	}
}
