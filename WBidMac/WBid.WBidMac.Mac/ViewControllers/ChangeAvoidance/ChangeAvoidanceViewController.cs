
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
//using System.Collections.Generic;
using WBid.WBidiPad.Model;
//using System.Linq;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;

namespace WBid.WBidMac.Mac
{
	public partial class ChangeAvoidanceViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public ChangeAvoidanceViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ChangeAvoidanceViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ChangeAvoidanceViewController () : base ("ChangeAvoidanceView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ChangeAvoidanceView View {
			get {
				return (ChangeAvoidanceView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try{
			base.AwakeFromNib ();
			txtAvoidanceBidChoice.CellWithTag (0).StringValue = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1;
			txtAvoidanceBidChoice.CellWithTag (1).StringValue = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2;
			txtAvoidanceBidChoice.CellWithTag (2).StringValue = GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3;

			btnCancel.Activated += (object sender, EventArgs e) => {
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
			};
			btnOK.Activated += (object sender, EventArgs e) => {

				// Validate Avoidance Bids

				GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance1 = txtAvoidanceBidChoice.CellWithTag (0).StringValue.Trim ();
				GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance2 = txtAvoidanceBidChoice.CellWithTag (1).StringValue.Trim ();
				GlobalSettings.WBidINIContent.AvoidanceBids.Avoidance3 = txtAvoidanceBidChoice.CellWithTag (2).StringValue.Trim ();

				//save the state of the INI File
				WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);

			};
			}
			catch(Exception ex) {

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
	}
}

