using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	
	public partial class AMPMView : AppKit.NSTableCellView
	{
		#region Constructors

		// Called when created from unmanaged code
		public AMPMView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AMPMView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		public void BindData(string data,int row)
		{
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
