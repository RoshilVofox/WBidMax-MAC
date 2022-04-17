using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers.initialLogin
{
    public partial class InitialLoginWindow : AppKit.NSWindow
    {
        public InitialLoginWindow(IntPtr handle) : base(handle)
        {
           
        }

        [Export("initWithCoder:")]
        public InitialLoginWindow(NSCoder coder) : base(coder)
        {
          
        }
       
        
    }
}