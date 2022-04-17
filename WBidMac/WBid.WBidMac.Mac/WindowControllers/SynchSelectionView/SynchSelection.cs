using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers.SynchSelectionView
{


    // mwindows delegate

    public class synchSelectionWindowDelegate : AppKit.NSWindowDelegate
    {
        public override bool WindowShouldClose(NSObject sender)
        {
           
            return true;
        }

        public override void WillClose(NSNotification notification)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName("closeSynchSelection", null);
        }
    }


    public partial class SynchSelection : NSWindow
    {
        public SynchSelection(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SynchSelection(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.Delegate = new synchSelectionWindowDelegate();



        }
    }
}
