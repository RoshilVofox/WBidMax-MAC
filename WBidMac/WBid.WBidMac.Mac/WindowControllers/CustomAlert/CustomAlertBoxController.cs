using System;

using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac.WindowControllers.CustomAlert
{
    public partial class CustomAlertBoxController : NSWindowController
    {//
        public DownloadBidWindowController objDownloadWindow;
        public string AlertType = "";
        public CustomAlertBoxController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public CustomAlertBoxController(NSCoder coder) : base(coder)
        {
        }

        public CustomAlertBoxController() : base("CustomAlertBox")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            if (AlertType == "InvalidCredential")
                ShowLoginErroAlert();
            this.Window.WillClose += delegate {
                this.Window.OrderOut(this);
                NSApplication.SharedApplication.StopModal();
            };

        }

        public new CustomAlertBox Window
        {
            get { return (CustomAlertBox)base.Window; }
        }

        public void ShowLoginErroAlert()
        {
            SetInvalidCredentialAlertMessage();
        }

        public void SetInvalidCredentialAlertMessage()
        {
            string showmessage1 = "\n\nTo LOGIN, you need to use your ";
            string showmessage2 = " password! \n\nMost likely, your ";
            string showmessage3 = " password has expired. \n\nBTW, it is possible your password to LOGIN on ";
            string showmessage4 = " is valid and your ";
            string showmessage5 = " password is expired. \n\n\nTo verify your ";
            string showmessage6 = " password is still good, open a browser and try to login to ";
            string showmessage7 = " (NOT swacrew.com).";
            string Boldsletter1 = "SwaLife"; //Defines the bold field
            string Boldsletter2 = "swacrew.com"; //Defines the bold field
            string Boldsletter3 = "swalife.com"; //Defines the bold field

            var res = new NSMutableAttributedString();
            nfloat size = 15.0f;
            var msg = new NSAttributedString(
    showmessage1,
    font: NSFont.SystemFontOfSize(size)

);
            var msg2 = new NSAttributedString(
    showmessage2,
    font: NSFont.SystemFontOfSize(size)

);
            var msg3 = new NSAttributedString(
    showmessage3,
    font: NSFont.SystemFontOfSize(size)

);
            var msg4 = new NSAttributedString(
    showmessage4,
    font: NSFont.SystemFontOfSize(size)

);
            var msg5 = new NSAttributedString(
    showmessage5,
    font: NSFont.SystemFontOfSize(size)

);
            var msg6 = new NSAttributedString(
    showmessage6,
    font: NSFont.SystemFontOfSize(size)

);
            var msg7 = new NSAttributedString(
    showmessage7,
    font: NSFont.SystemFontOfSize(size)

);
            var swaBold = (new NSAttributedString(
    Boldsletter1,
 font: NSFont.BoldSystemFontOfSize(size)

));
            var swacrew = (new NSAttributedString(
   Boldsletter2,
 font: NSFont.BoldSystemFontOfSize(size)

));
            var swalife = (new NSAttributedString(
   Boldsletter3,
 font: NSFont.BoldSystemFontOfSize(size)

));
            // CustomAttributedMessage.Append(msg);
            res.Append(msg);
            res.Append(swaBold);
            res.Append(msg2);
            res.Append(swaBold);
            res.Append(msg3);
            res.Append(swacrew);
            res.Append(msg4);
            res.Append(swaBold);
            res.Append(msg5);
            res.Append(swaBold);
            res.Append(msg6);
            res.Append(swalife);
            res.Append(msg7);
            lblMessage.AttributedStringValue = res;

        }

        partial void btnOkTapped(NSObject sender)
        {
            
            this.Window.Close();
            this.Window.OrderOut(this);
            NSApplication.SharedApplication.StopModal();
            if (objDownloadWindow != null)
                objDownloadWindow.DismissCurrentView();
        }
    }
}
