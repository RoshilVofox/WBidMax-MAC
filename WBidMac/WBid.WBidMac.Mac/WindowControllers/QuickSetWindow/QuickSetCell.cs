using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;

namespace WBid.WBidMac.Mac
{
	public partial class QuickSetCell : AppKit.NSTableCellView
	{
		public QuickSetCell ()
		{
		}

		// Called when created from unmanaged code
		public QuickSetCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QuickSetCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			if (btnCSWHelp != null) {
				btnCSWHelp.Activated += (object sender, EventArgs e) => {
					var btn = (NSButton)sender;
					var qsDetail = new QuickSetDetailWindowController ();
					qsDetail.QSCSWName = GlobalSettings.QuickSets.QuickSetCSW[(int)btn.Tag].QuickSetCSWName;
					this.Window.AddChildWindow (qsDetail.Window, NSWindowOrderingMode.Above);
					NSApplication.SharedApplication.RunModalForWindow (qsDetail.Window);
				};
			}

			if (btnColumnsHelp != null) {
				btnColumnsHelp.Activated += (object sender, EventArgs e) => {
					var btn = (NSButton)sender;
					var qsDetail = new QuickSetDetailWindowController ();
					qsDetail.QSColumnsName = GlobalSettings.QuickSets.QuickSetColumn[(int)btn.Tag].QuickSetColumnName;
					this.Window.AddChildWindow (qsDetail.Window, NSWindowOrderingMode.Above);
					NSApplication.SharedApplication.RunModalForWindow (qsDetail.Window);
				};
			}
		}

		public void BindData (int type, string name, int index)
		{
			if (type == 0) {
				btnCSWHelp.Tag = index;
				lblCSWName.StringValue = name;
			} else {
				btnColumnsHelp.Tag = index;
				lblColumnsName.StringValue = name;
			}
		}
	}
}

