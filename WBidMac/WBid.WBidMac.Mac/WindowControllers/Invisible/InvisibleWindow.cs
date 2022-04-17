
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class InvisibleWindow : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public InvisibleWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public InvisibleWindow (NSCoder coder) : base (coder)
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

