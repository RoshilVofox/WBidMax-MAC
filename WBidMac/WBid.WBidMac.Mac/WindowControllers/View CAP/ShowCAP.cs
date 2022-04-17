using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class ShowCAP : NSWindow
	{
		public ShowCAP (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ShowCAP (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
}
