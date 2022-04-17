
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using PdfKit;
using WebKit;
using WBid.WBidiPad.iOS.Utility;
using System.IO;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class FileWindowController : AppKit.NSWindowController
	{
		#region Constructors
		public bool isCourier;

		// Called when created from unmanaged code
		public FileWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public FileWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public FileWindowController () : base ("FileWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new FileWindow Window {
			get {
				return (FileWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
				this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void LoadPDF (string file)
		{
			try {
				var filePath = WBidHelper.GetAppDataPath () + "/" + file;
				var fileUrl = NSUrl.FromFilename (filePath);
				var pdfDoc = new PdfDocument (fileUrl);
				this.Window.ContentView = pdfFileView;	
				pdfFileView.Document = pdfDoc;
				this.Window.MakeKeyAndOrderFront (this);

			} catch (Exception ex) {


//				CommonClass.AppDelegate.ErrorLog (ex);
				var alert = new NSAlert ();
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Error";
				alert.InformativeText = "File not available . Please try again later";
				alert.AddButton ("OK");
				alert.Buttons [0].Activated += delegate {
					
					alert.Window.Close ();
					this.Window.Close();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();

				};
				alert.RunModal ();
			//CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);

			}
		}

		public void LoadTXT (string file)
		{
			try {
				var filePath = WBidHelper.GetAppDataPath () + "/" + file;
				this.Window.ContentView = txtFileView.EnclosingScrollView;		
				txtFileView.Value = File.ReadAllText (filePath);
				//if (isCourier)
				txtFileView.Font = NSFont.FromFontName ("Courier", 13);
				this.Window.MakeFirstResponder(txtFileView);
				this.Window.MakeKeyAndOrderFront (this);
				
			} catch (Exception ex) {
//				CommonClass.AppDelegate.ErrorLog (ex);
//				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);

				var alert = new NSAlert ();
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Error";
				alert.InformativeText = "File not available . Please try again later";
				alert.AddButton ("OK");
				alert.Buttons [0].Activated += delegate {

					alert.Window.Close ();
					this.Window.Close();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();

				};
				alert.RunModal ();
			}
		}

	}
}

