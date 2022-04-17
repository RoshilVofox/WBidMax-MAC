using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class CommutableViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public CommutableViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public CommutableViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public CommutableViewController () : base ("CommutableView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		partial void funCancelAction (NSObject sender)
		{
			
			this.View.Window.Close();
			this.View.Window.OrderOut(this);

		}
		partial void funDoneAction (NSObject sender)
		{
			
			//CLONotification
		}
		partial void funViewArrivalAnddepartTimeAction (NSObject sender)
		{
			
		}


		#endregion

		//strongly typed view accessor
		public new CommutableView View {
			get {
				return (CommutableView)base.View;
			}
		}
	}
}
