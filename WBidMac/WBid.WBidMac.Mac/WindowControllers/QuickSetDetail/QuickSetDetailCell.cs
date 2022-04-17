using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;

namespace WBid.WBidMac.Mac
{
	public partial class QuickSetDetailCell : AppKit.NSTableCellView
	{
		public QuickSetDetailCell ()
		{
		}

		// Called when created from unmanaged code
		public QuickSetDetailCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QuickSetDetailCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public void BindData (QSDetailData data, int index)
		{
			if (data.Type == 0) {
				lblTitle.DrawsBackground = true;
				lblTitle.BackgroundColor = NSColor.LightGray;
				lblTitle.Font = NSFont.BoldSystemFontOfSize (13);
				lblTitle.StringValue = data.Title;
				lblData.StringValue = "";
			} else {
				lblTitle.DrawsBackground = false;
				lblTitle.BackgroundColor = NSColor.White;
				lblTitle.Font = NSFont.SystemFontOfSize (12);
				lblTitle.StringValue = data.Title;
				lblData.Font = NSFont.SystemFontOfSize (12);
				lblData.StringValue = data.DataValue;
			}
		}
	}
}

