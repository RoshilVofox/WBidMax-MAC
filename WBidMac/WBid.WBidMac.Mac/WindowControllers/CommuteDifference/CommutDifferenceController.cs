using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers.CommuteDifference
{
    public partial class CommutDifferenceController : NSWindow
    {
        public CommutDifferenceController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public CommutDifferenceController(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();


        }
    }
}
