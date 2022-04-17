using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class BAFilterView : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public BAFilterView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BAFilterView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		public override void RightMouseUp (NSEvent theEvent)
		{
			base.RightMouseUp (theEvent);
			CommonClass.FilterController.RightClicked (theEvent);
		}

		#endregion
	}
}
