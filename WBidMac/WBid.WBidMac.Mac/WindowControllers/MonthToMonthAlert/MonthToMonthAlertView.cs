using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers
{
    public partial class MonthToMonthAlertView : NSWindow
    {
        public MonthToMonthAlertView(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MonthToMonthAlertView(NSCoder coder) : base(coder)
        {
        }
       
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
