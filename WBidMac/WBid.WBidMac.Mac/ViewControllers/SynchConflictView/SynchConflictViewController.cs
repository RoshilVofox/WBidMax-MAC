
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class SynchConflictViewController : AppKit.NSViewController
	{
		#region Constructors
		public DateTime serverSynchTime;
		public bool noServer;

		// Called when created from unmanaged code
		public SynchConflictViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SynchConflictViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public SynchConflictViewController () : base ("SynchConflictView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SynchConflictView View {
			get {
				return (SynchConflictView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			btnCancel.Activated += delegate {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("SyncConflict", new NSString (""), null);
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
				NSApplication.SharedApplication.StopModal();
			};
			btnServer.Activated += delegate {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("SyncConflict", new NSString ("server"), null);
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
				NSApplication.SharedApplication.StopModal();
			};
			btnLocal.Activated += delegate {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("SyncConflict", new NSString ("local"), null);
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
				NSApplication.SharedApplication.StopModal();
			};

			if (serverSynchTime != DateTime.MinValue)
			{
				var serverTimeCST = TimeZoneInfo.ConvertTimeFromUtc (serverSynchTime, TimeZoneInfo.FindSystemTimeZoneById ("America/Chicago"));

				if (noServer)
					btnServer.Enabled = false;
				else {
					btnServer.Enabled = true;
					lblServerDate.StringValue = serverTimeCST.ToShortDateString ();
					lblServerTime.StringValue = serverTimeCST.ToString ("hh:mm:ss tt");
				}
			}
			if (GlobalSettings.WBidStateCollection.StateUpdatedTime != DateTime.MinValue)
			{
				var localTimeCST = DateTime.MinValue;
				try {
					localTimeCST = TimeZoneInfo.ConvertTimeFromUtc (GlobalSettings.WBidStateCollection.StateUpdatedTime, TimeZoneInfo.FindSystemTimeZoneById ("America/Chicago"));
				} catch {
					localTimeCST = TimeZoneInfo.ConvertTimeFromUtc (GlobalSettings.WBidStateCollection.StateUpdatedTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById ("America/Chicago"));
				}

				lblLocalDate.StringValue = localTimeCST.ToShortDateString();
				lblLocalTime.StringValue = localTimeCST.ToString ("hh:mm:ss tt");
			}

		}
	}
}

