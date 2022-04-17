using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class DaysOfMonthController : AppKit.NSViewController
	{
		#region Constructors
		List<CalendarButton> _buttons;
		int _numberItemPerRow = 7;
		int _numberRows = 6;
		nfloat _itemSize = 47;
		public DaysOfMonthCx objData;
		bool isAdd;
		WBidState wBIdStateContent;
		// Called when created from unmanaged code
		public DaysOfMonthController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public DaysOfMonthController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public DaysOfMonthController () : base ("DaysOfMonth", NSBundle.MainBundle)
		{
			Initialize ();
		}

		public override void AwakeFromNib ()
		{
			if (objData != null)
				isAdd = false;
			else isAdd=true;
			LoadMonth ();
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            if (interfaceStyle == "Dark") {
                lblWeekDays.TextColor = NSColor.Black;
                btnThirdTouch.BackgroundColor = NSColor.White;
            }
		}
		void LoadMonth()
		{
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			ViewCalender.WantsLayer = true;
			ViewCalender.Layer.BorderWidth = 1;
			ViewCalender.Layer.BorderColor = NSColor.DarkGray.CGColor;
			ViewCalender.Layer.BorderWidth = (nfloat)0.5;
			ViewCalender.NeedsLayout = true;
			List<WBid.WBidiPad.Model.DaysOfMonth> dayOfMonthList = WBidCollection.GetDaysOfMonthList();
			_buttons = new List<CalendarButton> ();
			int index = 0;
			for (int row = 0; row < _numberRows; row++) {
				for (int col = 0; col < _numberItemPerRow; col++) {
					CalendarButton btn = new CalendarButton (dayOfMonthList[index].Day?? string.Empty,dayOfMonthList[index].Id);
					//btn.BezelStyle = NSBezelStyle.TexturedSquare;

					if (btn.Title.Length == 0)
					{
						btn.Enabled = false;
					}
					else
					{
						btn.Enabled = true;
					}

					btn.Frame = new CoreGraphics.CGRect (col * (ViewCalender.Frame.Width/7),(ViewCalender.Frame.Height-50) - row *( _itemSize), ViewCalender.Frame.Width/7 , _itemSize);
					//btn.Layer.BackgroundColor = NSColor.White.CGColor; // default color;
//					btn.Layer.BorderColor = NSColor.DarkGray.CGColor;
//					btn.Layer.BorderWidth = 0.6f;
//					btn.Layer.CornerRadius =2f;
					btn.Tag = 0;
					btn.Font = NSFont.BoldSystemFontOfSize(13);
					ButtonLayout (btn, 0);
					//btn.Font.col (UIColor.Black, UIControlState.Normal);
					btn.Activated += delegate {
						
						btn.Tag++;
						setButtonColorsAndState(btn);
					};
					_buttons.Add (btn);
					ViewCalender.AddSubview (btn);
						index++;
				}
			}

			DoneButtonStatus ();
		}


		public void ClearAll()
		{

            foreach (CalendarButton calBtn in _buttons)
//			{
//				if (calBtn.Tag%3 == 1  || calBtn.Tag%3 == 2)
//				{
//					calBtn.State = 0;
//					if (calBtn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1) || calBtn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2) || calBtn.Date == GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3))
//					{
//						calBtn.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
//					}
//					else
//					{
//						calBtn.Layer.BackgroundColor = NSColor.White.CGColor;
//					}
//
//					calBtn.Tag = 0;
//				}
//
//			}
                if (_buttons != null)
                {
                    if(objData !=null && objData.OFFDays !=null){
                        objData.OFFDays.Clear();
                    }
                    if(objData !=null && objData.WorkDays !=null){
                        objData.WorkDays.Clear();
                    }     
                }
            LoadMonth();
		}

		void ButtonLayout(CalendarButton button,int state)
		{
			button.WantsLayer = true;//NSColor.FromRgba(124/256,206/256,38/256,1).CGColor;
			var dom=objData;

			if (dom != null) 
			{
				
				if (objData == null)
					objData = new DaysOfMonthCx ();


				var bidautoobject =objData;

				if (bidautoobject.WorkDays!=null && bidautoobject.WorkDays.Contains (button.ID)) 
				{
					
					state = 2;
					if (objData.WorkDays == null)
						objData.WorkDays = new List<int> ();
					//objData.WorkDays.Add (button.ID);

				} else if (bidautoobject.OFFDays!=null && bidautoobject.OFFDays.Contains (button.ID)) {
					state = 1;
					if (objData.OFFDays == null)
						objData.OFFDays = new List<int> ();
					//objData.OFFDays.Add (button.ID);
				}
				button.State = state;
			}
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            switch (state) {
			case 0:
				button.Layer.BackgroundColor = NSColor.White.CGColor;
                    if (interfaceStyle == "Dark")
                    {
                        button.AttributedTitle = new NSAttributedString(button.Title, ApplyAttribure(NSColor.Black));
                    }

                    break;
			case 1:
				button.Layer.BackgroundColor = NSColor.Red.CGColor;
                   if (interfaceStyle == "Dark"){
                        button.AttributedTitle = new NSAttributedString(button.Title, ApplyAttribure(NSColor.Black));
                    }

                    break;
			case 2:
				
				button.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
				break;

			}
//			if (state) button.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
//			else button.Layer.BackgroundColor = NSColor.Orange.CGColor;//NSColor.FromCalibratedRgba(.91f, .51f, .21f, 1).CGColor;

			button.Layer.CornerRadius = (nfloat)2;
			button.Layer.BorderColor = NSColor.DarkGray.CGColor;
			button.Layer.BorderWidth = (nfloat)0.5;
			button.NeedsLayout = true;

		}
		void setButtonColorsAndState(CalendarButton button)
		{
			button.NeedsLayout = true;
			if (objData == null) {
				objData = new DaysOfMonthCx();
			}
			var _city = button.ID;
			if (button.Tag%3 == 0) {
				// remove
				button.Layer.BackgroundColor = NSColor.White.CGColor;
				button.AttributedTitle = new NSAttributedString (button.Title, ApplyAttribure (NSColor.Black));

				if (objData != null && objData.OFFDays != null && objData.OFFDays.Contains(_city)) {
					objData.OFFDays.Remove(_city);
				}

				if (objData != null && objData.WorkDays != null && objData.WorkDays.Contains(_city)) {
					objData.WorkDays.Remove(_city);
				}
			}
			if (button.Tag%3 == 2) {
				button.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
				button.AttributedTitle = new NSAttributedString (button.Title, ApplyAttribure (NSColor.Black));

				if (objData.WorkDays!= null && !objData.WorkDays.Contains(_city)) {
					objData.WorkDays.Add(_city);
				}
				if (objData.WorkDays == null) {
					objData.WorkDays = new List<int>();
					objData.WorkDays.Add(_city);
				}
				if (objData.OFFDays != null && objData.OFFDays.Contains(_city)) {
					objData.OFFDays.Remove(_city);
				}
			}
			if (button.Tag %3  == 1) {
				// no
				button.Layer.BackgroundColor = NSColor.Red.CGColor;
				button.AttributedTitle = new NSAttributedString (button.Title, ApplyAttribure (NSColor.White));

				if (objData.WorkDays!= null && objData.WorkDays.Contains(_city)) {
					objData.WorkDays.Remove(_city);
				}
				if (objData.OFFDays != null && !objData.OFFDays.Contains(_city)) {
					objData.OFFDays.Add(_city);
				}
				if (objData.OFFDays == null) {
					objData.OFFDays = new List<int>();
					objData.OFFDays.Add(_city);
				}
			}
			DoneButtonStatus ();
		}
		NSStringAttributes ApplyAttribure(NSColor color)
		{
			NSMutableParagraphStyle objAlignment = new NSMutableParagraphStyle ();
			objAlignment.Alignment = NSTextAlignment.Center;
			var Applied = new NSStringAttributes {
				ForegroundColor =color,
				Font=NSFont.BoldSystemFontOfSize((nint)13),
				ParagraphStyle= objAlignment
			}  ;
			return Applied;

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
		partial void funClearAction (NSObject sender)
		{
			ClearAll();
		}

		partial void funDoneAction (NSObject sender)
		{
			

			//


			if (!isAdd)
			{

//				objData.WorkDays=objData.WorkDays;
//				objData.OFFDays=objData.OFFDays;

					NSNotificationCenter.DefaultCenter.PostNotificationName ("ClickdaysOfMonthNotification", null);
			}
			else
			{
				BidAutoItem bidAutoItem = new BidAutoItem ();
				WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				bidAutoItem.Name = "DOM";
				bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
				bidAutoItem.IsApplied=false;
				bidAutoItem.BidAutoObject = new DaysOfMonthCx()
				{
					WorkDays=objData.WorkDays,
					OFFDays=objData.OFFDays
				};

				wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);

				NSNotificationCenter.DefaultCenter.PostNotificationName ("daysOfMonthNotification", null);


					
			}

			CommonClass.MainController.UpdateSaveButton (true);



//			if(wBIdStateContent.BidAuto.BAFilter.Any(x=>x.Name=="DOM"))
//			{
//				wBIdStateContent.BidAuto.BAFilter.FirstOrDefault(x=>x.Name=="DOM").BidAutoObject=new DaysOfMonthCx()
//				{
//					WorkDays=objData.WorkDays,
//					OFFDays=objData.OFFDays
//				};
//			}
//			else
//			{
//				if(objData!=null)
//				{
//					bidAutoItem.BidAutoObject = new DaysOfMonthCx()
//					{
//						WorkDays=objData.WorkDays,
//						OFFDays=objData.OFFDays
//					};
//					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
//				}
//			}

			this.View.Window.Close();
			this.View.Window.OrderOut(this);
		}

		public void DoneButtonStatus()
		{
			btnDone.Enabled = false;
			if ((objData != null) && (((objData.WorkDays != null)&&(objData.WorkDays.Count >0))|| ((objData.OFFDays != null)&&(objData.OFFDays.Count>0)))) {
				btnDone.Enabled = true;
			}
		}
		#endregion

		//strongly typed view accessor
		public new DaysOfMonth View {
			get {
				return (DaysOfMonth)base.View;
			}
		}
	}
}
