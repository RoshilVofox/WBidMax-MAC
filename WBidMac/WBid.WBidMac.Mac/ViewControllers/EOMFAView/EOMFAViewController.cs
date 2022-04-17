
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class EOMFAViewController : AppKit.NSViewController
	{
		#region Constructors
		public string [] options;

		// Called when created from unmanaged code
		public EOMFAViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public EOMFAViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public EOMFAViewController () : base ("EOMFAView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new EOMFAView View {
			get {
				return (EOMFAView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				btnVacOptions.CellWithTag (0).Title = options [0];
				btnVacOptions.CellWithTag (1).Title = options [1];
				btnVacOptions.CellWithTag (2).Title = options [2];
				btnVacOptions.CellWithTag (3).Title = "None of the above";
				
				btnOK.Activated += (object sender, EventArgs e) => {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("EOMFAVacation", new NSNumber (btnVacOptions.SelectedTag));
				};
				btnCancel.Activated += (object sender, EventArgs e) => {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("EOMFAVacation", new NSNumber (3));
				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
	}
}

