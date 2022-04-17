
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using CoreGraphics;
using System.IO;
using System.Text;
using PdfKit;

namespace WBid.WBidMac.Mac
{
	public partial class BidReceiptViewController : AppKit.NSViewController
	{
		public List<string> fileNames;
		public string selectedFile;
		public bool toPrint;

		#region Constructors

		// Called when created from unmanaged code
		public BidReceiptViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidReceiptViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BidReceiptViewController () : base ("BidReceiptView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new BidReceiptView View {
			get {
				return (BidReceiptView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				if (toPrint)
					btnOK.Title = "Print";

				tblBidReceipt.Source = new ReceiptTableSource (this);


				btnCancel.Activated += (object sender, EventArgs e) => {
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
				};
				btnOK.Activated += (object sender, EventArgs e) => {
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
					if (!toPrint) {
						


						var fileViewer = new FileWindowController ();
						fileViewer.Window.Title = "Bid Receipt";
						if(Path.GetExtension(selectedFile).ToLower()==".rct")
						{
							fileViewer.LoadTXT (selectedFile);
						}
						else
						{
						fileViewer.LoadPDF (selectedFile);
						}
						//CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
						//fileViewer.Window.MakeKeyAndOrderFront (this);
					} else {
					

						string filePath = WBidHelper.GetAppDataPath () + "/" + selectedFile;
						var inv = new InvisibleWindowController ();
						CommonClass.MainController.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);

						if (Path.GetExtension (selectedFile).ToLower () == ".pdf") 
						{
							var fileUrl = NSUrl.FromFilename (filePath);
							PdfDocument aPdfDocument = new PdfDocument(fileUrl);
							PdfView aPDFView = new PdfView();
							aPDFView.Document = aPdfDocument;
							inv.Window.ContentView.AddSubview (aPDFView);
							//var pr = NSPrintInfo.SharedPrintInfo;
							//pr.VerticallyCentered = false;
							//pr.TopMargin = 2.0f;
							//pr.BottomMargin = 2.0f;
							//pr.LeftMargin = 1.0f;
							//pr.RightMargin = 1.0f;
							//aPDFView.Print(this);

							NSPrintInfo pr = NSPrintInfo.SharedPrintInfo;
							pr.VerticallyCentered = false;
							pr.TopMargin = 2.0f;
							pr.BottomMargin = 2.0f;
							pr.LeftMargin = 1.0f;
							pr.RightMargin = 1.0f;
							aPDFView.Print(pr, true);

						}
						else
						{


							var txt = new NSTextView (new CGRect (0, 0, 550, 550));
							txt.Font = NSFont.FromFontName ("Courier", 8);
							inv.Window.ContentView.AddSubview (txt);
							txt.Value = CommonClass.FormatBidReceipt (filePath);
							var pr = NSPrintInfo.SharedPrintInfo;
							pr.VerticallyCentered = false;
							pr.TopMargin = 2.0f;
							pr.BottomMargin = 2.0f;
							pr.LeftMargin = 1.0f;
							pr.RightMargin = 1.0f;
							txt.Print (this);
							inv.Close ();
						}








//						var filePath = WBidHelper.GetAppDataPath () + "/" + selectedFile;
//						var inv = new InvisibleWindowController ();
//						CommonClass.MainController.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
//						var txt = new NSTextView (new RectangleF (0, 0, 550, 550));
//						txt.Font = NSFont.FromFontName ("Courier New", 8);
//						inv.Window.ContentView.AddSubview (txt);
//						txt.Value = CommonClass.FormatBidReceipt (filePath);
//						var pr = NSPrintInfo.SharedPrintInfo;
//						pr.VerticallyCentered = false;
//						pr.TopMargin = 4.0f;
//						pr.BottomMargin = 2.0f;
//						pr.LeftMargin = 1.0f;
//						pr.RightMargin = 1.0f;
//						txt.Print (this);
//						inv.Close ();
					}
				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

	}

	public partial class ReceiptTableSource : NSTableViewSource
	{
		BidReceiptViewController parentVC;

		public ReceiptTableSource (BidReceiptViewController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.fileNames.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return new NSString (parentVC.fileNames [(int)row]);
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			parentVC.selectedFile = parentVC.fileNames [(int)table.SelectedRow];
		}
	}
}

