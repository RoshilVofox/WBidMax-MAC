using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class ArrivalAndDepartView : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public ArrivalAndDepartView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ArrivalAndDepartView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		
		}


		#endregion
	}
}
