using System;
using Foundation;
using AppKit;

using System.Drawing;

namespace WBid.WBidMac.Mac
{
	public class CalendarButton: NSButton
	{

		public int ID;
		public int State;
		public DateTime Date;
		public CalendarButton (string text, int id)
//			: base (UIButtonType.Custom)
		{
			ID = id;
			this.Title = text;
			this.SetButtonType (NSButtonType.OnOff);
			this.Bordered = false;
			this.BezelStyle = NSBezelStyle.TexturedSquare;
			//SetTitleWithMnemonic (text);
		}
		public CalendarButton ():base(){
			State = 0;
		}
		public CalendarButton(IntPtr handle) : base(handle) { }
	}
}

