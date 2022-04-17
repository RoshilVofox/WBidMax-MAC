
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class OverlayViewController : AppKit.NSViewController
	{
		#region Constructors
		public string OverlayText = string.Empty;

		// Called when created from unmanaged code
		public OverlayViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public OverlayViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public OverlayViewController () : base ("OverlayView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new OverlayView View {
			get {
				return (OverlayView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				pgrsOverlay.StartAnimation (null);
				lblOverlay.StringValue = OverlayText;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void UpdateText (string text)
		{
			lblOverlay.StringValue = text;
		}
	}
}

