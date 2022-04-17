using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class MILConfigCell : AppKit.NSTableCellView
	{
		public MILConfigCell ()
		{
		}

		// Called when created from unmanaged code
		public MILConfigCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MILConfigCell (NSCoder coder) : base (coder)
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
			dpStartDate.TimeZone = NSTimeZone.LocalTimeZone;
			dpEndDate.TimeZone = NSTimeZone.LocalTimeZone;
			dpStartDate.Calendar = NSCalendar.CurrentCalendar;
			dpEndDate.Calendar = NSCalendar.CurrentCalendar;
//			dpStartDate.Locale = new NSLocale ("NL");
//			dpEndDate.Locale = new NSLocale ("NL");

			btnClose.Activated += (object sender, EventArgs e) => {
				var btn = (NSButton)sender;
				CommonClass.MILList.RemoveAt ((int)btn.Tag);
				CommonClass.MILController.ReloadView ();
			};

			dpStartDate.Activated += (object sender, EventArgs e) => {
				var dp = (NSDatePicker)sender;
				//CommonClass.MILList [(int)dp.Tag].StartAbsenceDate = dp.DateValue.NSDateToDateTime();
				CommonClass.MILList [(int)dp.Tag].StartAbsenceDate = NSDateToDateTime(dp.DateValue);
			};
			dpEndDate.Activated += (object sender, EventArgs e) => {
				var dp = (NSDatePicker)sender;
				CommonClass.MILList [(int)dp.Tag].EndAbsenceDate = NSDateToDateTime(dp.DateValue);
			};
			btnStartDrop.Activated += (object sender, EventArgs e) => {
				var btn = (NSButton)sender;
				CommonClass.MILController.isStartPop = true;
				CommonClass.MILController.curDate = CommonClass.MILList[(int)btn.Tag].StartAbsenceDate;
				CommonClass.MILController.ShowCalendarPopover (sender);
			};
			btnEndDrop.Activated += (object sender, EventArgs e) => {
				var btn = (NSButton)sender;
				CommonClass.MILController.isStartPop = false;
				CommonClass.MILController.curDate = CommonClass.MILList[(int)btn.Tag].EndAbsenceDate;
				CommonClass.MILController.ShowCalendarPopover (sender);
			};
		}
		public DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime( 
				new DateTime(2001, 1, 1, 0, 0, 0) );
			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		}

		public void BindData (Absense abs, int row)
		{
			if (CommonClass.MILList.Count == 1)
				btnClose.Hidden = true;
			else
				btnClose.Hidden = false;

			btnClose.Tag = row;
			btnStartDrop.Tag = row;
			btnEndDrop.Tag = row;
			dpStartDate.Tag = row;
			dpEndDate.Tag = row;

			dpStartDate.DateValue = abs.StartAbsenceDate.DateTimeToNSDate();
			dpEndDate.DateValue = abs.EndAbsenceDate.DateTimeToNSDate();
		}

	}
}

