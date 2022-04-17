
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using PdfKit;
using WebKit;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidMac.Mac
{
	public partial class HelpWindowController : AppKit.NSWindowController
	{
		#region Constructors
		public List <string> lstDocuments;
		public List <string> lstVideos;
		public List <string> lstVideoIDs;

		// Called when created from unmanaged code
		public HelpWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public HelpWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public HelpWindowController () : base ("HelpWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new HelpWindow Window {
			get {
				return (HelpWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.ShouldCascadeWindows = false;
			this.Window.WillClose += (object sender, EventArgs e) => {
				webVideo.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));
				this.Window.OrderOut (this);
				NSApplication.SharedApplication.StopModal ();
			};
			lstDocuments = new List<string> (){ 
				"Getting Started",
				"Constraints",
				"Weights",
				"Sorts",
				"Column Headings",
				"EOM - End of Month",
				"OVR – MTM Overlap",
				"VAC - Vacation Pilots",
				"VAC – Vacation Flt Att",
				"Smart Sync",
				"Quicksets",
				"MIL Button",
			};
			lstVideos = new List<string> (){ 
				"Download Bid Data",
				"Four Views of WBid",
				"Simple Pilot Bid with Constraints",
				"Submit Flt Att Bid 1st Round"
			};
			lstVideoIDs = new List<string> (){ 
				"uRvOJvefjfQ",
				"Ux6ryrXpBOk",
				"xJCJNqSsrgY",
				"U37T4SSCSN8"
			};

			tblDocument.Source = new DocumentTableSource (this);
			tblVideo.Source = new VideoTableSource (this);
		}

	}

	public partial class DocumentTableSource : NSTableViewSource 
	{
		HelpWindowController parentVC;
		public DocumentTableSource (HelpWindowController parent)
		{
			parentVC = parent;
		}
		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.lstDocuments.Count;
		}
		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return new NSString (parentVC.lstDocuments [(int)row]);		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				var filePath = NSBundle.MainBundle.ResourcePath + "/Help Files/" + parentVC.lstDocuments[(int)table.SelectedRow]+".pdf";
				var fileUrl = NSUrl.FromFilename (filePath);
				var pdfDoc = new PdfKit.PdfDocument (fileUrl);
				parentVC.pdfDocView.Document = pdfDoc;
			}
		}
	}

	public partial class VideoTableSource : NSTableViewSource 
	{
		HelpWindowController parentVC;
		public VideoTableSource (HelpWindowController parent)
		{
			parentVC = parent;
		}
		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.lstVideos.Count;
		}
		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return new NSString (parentVC.lstVideos [(int)row]);		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				if (Reachability.IsHostReachable ("www.youtube.com")) {
					var videoID = parentVC.lstVideoIDs [(int)table.SelectedRow];
					string loadStr = "<iframe width=\"475\" height=\"360\" src=\"http://www.youtube.com/embed/" + videoID + "?rel=0&amp;showinfo=0\" frameborder=\"0\" allowfullscreen></iframe>";
					parentVC.webVideo.MainFrame.LoadHtmlString (loadStr,null);
				} else {
					parentVC.webVideo.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));
				}
			} else {
				parentVC.webVideo.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));
			}
		}
	}

}

