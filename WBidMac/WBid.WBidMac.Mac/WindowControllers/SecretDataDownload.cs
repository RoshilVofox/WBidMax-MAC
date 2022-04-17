using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers
{
    public partial class SecretDataDownload : NSWindow
    {
        public SecretDataDownload(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SecretDataDownload(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
