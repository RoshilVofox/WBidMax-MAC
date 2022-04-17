
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidMac.Mac
{
	public partial class ChangeEmployeeViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public ChangeEmployeeViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ChangeEmployeeViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ChangeEmployeeViewController () : base ("ChangeEmployeeView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ChangeEmployeeView View {
			get {
				return (ChangeEmployeeView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				
				txtEmployeeNo.StringValue = GlobalSettings.TemporaryEmployeeNumber ?? string.Empty;
				
				btnKeepOld.Activated += (object sender, EventArgs e) => {
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
				};
				btnChange.Activated += (object sender, EventArgs e) => {
				
					//Validate Employee Number!
				
					GlobalSettings.TemporaryEmployeeNumber = txtEmployeeNo.StringValue;
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
				};
			} catch (Exception ex) 
			{
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
	}
}

