using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers.SynchView
{


    // mwindows delegate

    public class synchviewWindowDelegate : AppKit.NSWindowDelegate
    {
        public override bool WindowShouldClose(NSObject sender)
        {

            NSNotificationCenter.DefaultCenter.PostNotificationName("closeSynch", null);
            return true;
        }

        public override void WillClose(NSNotification notification)
        {
           
        }

        
    }


    public partial class SynchViewWindow : NSWindow
    {


        
        public SynchViewWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SynchViewWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.Delegate = new synchviewWindowDelegate();
        }
    }
}
