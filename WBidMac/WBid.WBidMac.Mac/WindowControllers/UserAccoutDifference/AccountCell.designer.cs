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
	[Register ("AccountCell")]
	partial class AccountCell
	{
		[Outlet]
		public AppKit.NSTextFieldCell parameter { get; set; }

		[Outlet]
		public AppKit.NSSegmentedCell parameterSegment { get; set; }

		[Action ("accountSelectionSegment:")]
		partial void accountSelectionSegment (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (parameter != null) {
				parameter.Dispose ();
				parameter = null;
			}

			if (parameterSegment != null) {
				parameterSegment.Dispose ();
				parameterSegment = null;
			}
		}
	}
}
