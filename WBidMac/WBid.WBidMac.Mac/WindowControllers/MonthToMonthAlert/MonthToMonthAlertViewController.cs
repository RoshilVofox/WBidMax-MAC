using System;

using Foundation;
using AppKit;
using System.Net;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidMac.Mac.WindowControllers
{
    public partial class MonthToMonthAlertViewController : NSWindowController
    {
        public string Alert { get; set; }
        public MonthToMonthAlertViewController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MonthToMonthAlertViewController(NSCoder coder) : base(coder)
        {
        }

        public MonthToMonthAlertViewController() : base("MonthToMonthAlertView")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            lblAlert.StringValue = Alert;
            btnLink1.Title = "The Limitations and Opportunity of a Month-to-Month Vacation";
            btnKink2.Title = "Who Wants 65 tfp for one week of Vacation";
            this.Window.WillClose += delegate {
                this.Window.OrderOut(this);
                NSApplication.SharedApplication.StopModal();
            };
        }

        public new MonthToMonthAlertView Window
        {
            get { return (MonthToMonthAlertView)base.Window; }
        }
        partial void btnLink1tap(NSObject sender)
        {
            

            DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), "The Limitations and Opportunity of a Month-to-Month Vacation.pdf");
            InvokeOnMainThread(() => {
                var fileViewer = new FileWindowController();
                fileViewer.Window.Title = "Alert";
                fileViewer.LoadPDF("The Limitations and Opportunity of a Month-to-Month Vacation.pdf");
                
                CommonClass.MainController.Window.AddChildWindow(fileViewer.Window, NSWindowOrderingMode.Above);
                fileViewer.Window.MakeKeyAndOrderFront(fileViewer.Window);
                NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
                
            });
        }
        partial void btnOkTapped(NSObject sender)
        {
            this.Window.Close();
            this.Window.OrderOut(this);
        }
        partial void btnLink2Tap(NSObject sender)
        {
            DownloadBid.DownloadWBidFile(WBidHelper.GetAppDataPath(), "Who Wants 65 tfp for one week of Vacation.pdf");
            InvokeOnMainThread(() => {
                var fileViewer = new FileWindowController();
                fileViewer.Window.Title = "Alert";
                fileViewer.LoadPDF("Who Wants 65 tfp for one week of Vacation.pdf");
               
                CommonClass.MainController.Window.AddChildWindow(fileViewer.Window, NSWindowOrderingMode.Above);
                fileViewer.Window.MakeKeyAndOrderFront(fileViewer.Window);
                NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
                
            });
         
        }

    }
}
