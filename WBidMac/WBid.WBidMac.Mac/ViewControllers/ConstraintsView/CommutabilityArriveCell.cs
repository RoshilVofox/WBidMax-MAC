using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
    public partial class CommutabilityArriveCell : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public CommutabilityArriveCell (IntPtr handle) : base (handle)
        {
            Initialize ();
        }

        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public CommutabilityArriveCell (NSCoder coder) : base (coder)
        {
            Initialize ();
        }

        // Shared initialization code
        void Initialize ()
        {
        }

        #endregion
    }
}
