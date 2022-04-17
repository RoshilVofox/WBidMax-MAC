using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
    public partial class CommutabilityArriveViewController : AppKit.NSViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public CommutabilityArriveViewController (IntPtr handle) : base (handle)
        {
            Initialize ();
        }

        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public CommutabilityArriveViewController (NSCoder coder) : base (coder)
        {
            Initialize ();
        }

        // Call to load from the XIB/NIB file
        public CommutabilityArriveViewController () : base ("CommutabilityArriveView", NSBundle.MainBundle)
        {
            Initialize ();
        }

        // Shared initialization code
        void Initialize ()
        {
        }

        #endregion

        //strongly typed view accessor
        public new CommutabilityArriveView View {
            get {
                return (CommutabilityArriveView)base.View;
            }
        }
    }
}
