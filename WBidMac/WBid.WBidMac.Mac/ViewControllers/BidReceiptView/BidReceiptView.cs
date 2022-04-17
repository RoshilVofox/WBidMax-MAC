
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class BidReceiptView : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public BidReceiptView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidReceiptView (NSCoder coder) : base (coder)
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

