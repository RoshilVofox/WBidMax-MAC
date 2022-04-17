// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers.SynchSelectionView
{
	[Register ("SynchSelectionController")]
	partial class SynchSelectionController
	{
		[Outlet]
		AppKit.NSTextField lblQuickLocalDate { get; set; }

		[Outlet]
		AppKit.NSTextField lblQuickLocalTime { get; set; }

		[Outlet]
		AppKit.NSTextField lblQuickServerDate { get; set; }

		[Outlet]
		AppKit.NSTextField lblQuickServerTime { get; set; }

		[Outlet]
		AppKit.NSTextField lblStateLocalDate { get; set; }

		[Outlet]
		AppKit.NSTextField lblStateLocalTime { get; set; }

		[Outlet]
		AppKit.NSTextField lblStateServerDate { get; set; }

		[Outlet]
		AppKit.NSTextField lblStateServerTime { get; set; }

		[Outlet]
		AppKit.NSLayoutConstraint QuickHieghtConstraint { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl QuickSegment { get; set; }

		[Outlet]
		AppKit.NSView QuickView { get; set; }

		[Outlet]
		AppKit.NSLayoutConstraint StateHieghtConstraint { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl StateSegment { get; set; }

		[Outlet]
		AppKit.NSView StateView { get; set; }

		[Action ("SynchAction:")]
		partial void SynchAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblQuickLocalDate != null) {
				lblQuickLocalDate.Dispose ();
				lblQuickLocalDate = null;
			}

			if (lblQuickLocalTime != null) {
				lblQuickLocalTime.Dispose ();
				lblQuickLocalTime = null;
			}

			if (lblQuickServerDate != null) {
				lblQuickServerDate.Dispose ();
				lblQuickServerDate = null;
			}

			if (lblQuickServerTime != null) {
				lblQuickServerTime.Dispose ();
				lblQuickServerTime = null;
			}

			if (lblStateLocalDate != null) {
				lblStateLocalDate.Dispose ();
				lblStateLocalDate = null;
			}

			if (lblStateLocalTime != null) {
				lblStateLocalTime.Dispose ();
				lblStateLocalTime = null;
			}

			if (lblStateServerDate != null) {
				lblStateServerDate.Dispose ();
				lblStateServerDate = null;
			}

			if (lblStateServerTime != null) {
				lblStateServerTime.Dispose ();
				lblStateServerTime = null;
			}

			if (QuickHieghtConstraint != null) {
				QuickHieghtConstraint.Dispose ();
				QuickHieghtConstraint = null;
			}

			if (QuickSegment != null) {
				QuickSegment.Dispose ();
				QuickSegment = null;
			}

			if (QuickView != null) {
				QuickView.Dispose ();
				QuickView = null;
			}

			if (StateHieghtConstraint != null) {
				StateHieghtConstraint.Dispose ();
				StateHieghtConstraint = null;
			}

			if (StateSegment != null) {
				StateSegment.Dispose ();
				StateSegment = null;
			}

			if (StateView != null) {
				StateView.Dispose ();
				StateView = null;
			}
		}
	}
}
