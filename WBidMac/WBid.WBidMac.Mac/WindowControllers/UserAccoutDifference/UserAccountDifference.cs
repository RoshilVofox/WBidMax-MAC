using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers.UserAccoutDifference
{
    public partial class UserAccountDifference : NSWindow
    {
        public UserAccountDifference(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public UserAccountDifference(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        
    }
}
