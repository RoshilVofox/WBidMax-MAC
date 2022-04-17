using System;

using Foundation;
using AppKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class BidChoiceWindowController : NSWindowController
	{
		public BidChoiceWindowController(IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public BidChoiceWindowController(NSCoder coder) : base(coder)
		{
		}

		public BidChoiceWindowController() : base("BidChoiceWindow")
		{
		}
		static NSButton closeButton;
		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			//this.Window.OrderFront(null);
			closeButton = this.Window.StandardWindowButton(NSWindowButton.CloseButton);
			closeButton.Activated += (sender, e) =>
			{
				this.Window.Close();
				this.Window.OrderOut(this);
				NSApplication.SharedApplication.StopModal();
			};
			this.Window.Title = "Bid Choices";
			txtBidChoiceView.Editable = false;

			var submitbidchoice =GlobalSettings.SubmitBid.Bid.ToString();

			//txtBidChoiceView.Value = submitbidchoice;
 			//	string submitbidchoice = GlobalSettings.SubmitBid.Bid; 			//submitbidchoice = submitbidchoice.Replace(",", ", "); 			//txtView.Text = submitbidchoice;
 			var textStyle = new NSMutableParagraphStyle(); 			textStyle.LineSpacing = 15;
			//txtBidChoiceView.AttributedString 			var textFontAttributes = new NSStringAttributes() { Font = NSFont.SystemFontOfSize(14.0f), ForegroundColor = NSColor.Black, ParagraphStyle = textStyle };
			var txt = new NSAttributedString(submitbidchoice, textFontAttributes);
			txtBidChoiceView.Value = submitbidchoice.ToString();
			//var txtt== new NSAttributedString(submitbidchoice, textFontAttributes);
			//txtBidChoiceView.Value = new NSAttributedString(submitbidchoice, textFontAttributes);
			//NSMutableAttributedString str = new NSMutableAttributedString(submitbidchoice, textFontAttributes);
			//txtBidChoiceView.Value = str;  
		}

		public new BidChoiceWindow Window
		{
			get { return (BidChoiceWindow)base.Window; }
		}
	}
}
