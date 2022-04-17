using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
    public partial class LoginWindowController : AppKit.NSWindowController
    {
        public LoginWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public LoginWindowController(NSCoder coder) : base(coder)
        {
        }

        public LoginWindowController() : base("LoginWindow")
        {
        }
        //strongly typed window accessor
        public new LoginWindow Window
        {
            get
            {
                return (LoginWindow)base.Window;
            }
        }
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.ShouldCascadeWindows = false;
            
        }
        }
}
