
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class InvisibleWindowController : AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public InvisibleWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public InvisibleWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public InvisibleWindowController () : base ("InvisibleWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new InvisibleWindow Window {
			get {
				return (InvisibleWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.Window.AlphaValue = 0.0f;
			this.Window.IgnoresMouseEvents = true;
		}
	}
}

