using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{  // [Register("ArrivalDepartCell")]
	public partial class ArrivalDepartCell : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
//		public ArrivalDepartCell (IntPtr handle) : base (handle)
//		{
//			Initialize ();
//		}

//		public ArrivalDepartCell ()
//		{
//			Initialize ();
//		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ArrivalDepartCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

//		public ArrivalDepartCell () : base ("ArrivalDepartCell")
//		{
//			Initialize ();
//		}
//		public ArrivalDepartCell() : base(new CoreGraphics.CGRect(0,0,100,40))
//		{
//			
//		}
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.NeedsDisplay = true;

		}
		public void BindData(string Day,string Arrival,string Depart)

		{
			lblArrival.StringValue = Arrival;
			lblDay.StringValue = Day;
			lblDepart.StringValue = Depart;

		}
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
	}
}
