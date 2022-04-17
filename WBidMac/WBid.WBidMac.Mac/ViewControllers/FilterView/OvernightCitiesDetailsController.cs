using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidMac.Mac
{
	public partial class OvernightCitiesDetailsController : AppKit.NSViewController
	{
		#region Constructors
		List<CalendarButton> _buttons;
		OvernightCitiesCx _data;
		public BulkOvernightCityCx _BulkOverNightCity;
		int _numberItemPerRow = 12;
		int _numberRows = 60/12;
		nfloat _itemSize = 50;
		WBidState wBIdStateContent;
		// Called when created from unmanaged code
		public OvernightCitiesDetailsController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public OvernightCitiesDetailsController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public OvernightCitiesDetailsController () : base ("OvernightCitiesDetails", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		public override void AwakeFromNib ()
		{
			//LoadMonth ();

			LoadMonthInScrollView ();
		}

		void LoadMonthInScrollView()
		{
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			ScrollViewOvernightCities.WantsLayer = true;
			ScrollViewOvernightCities.Layer.BorderWidth = 1;
			ScrollViewOvernightCities.Layer.BorderColor = NSColor.DarkGray.CGColor;
			ScrollViewOvernightCities.Layer.BorderWidth = (nfloat)0.5;
			ScrollViewOvernightCities.NeedsLayout = true;
			var lstname = GlobalSettings.WBidINIContent.Cities.Select (x => x.Name).ToList ();
			var cities = GlobalSettings.WBidINIContent.Cities;

//			ViewContent.Frame = new CoreGraphics.CGRect (0, 0, 500, 600);
//			ScrollViewOvernightCities.Frame= new CoreGraphics.CGRect (0, 0, 200, 200);
		
			_buttons = new List<CalendarButton> ();
			//
			int Index=0;
			for (int row = 0; row < lstname.Count/12; row++) {
				int leftpadding = 20;
				for (int col = 0; col < _numberItemPerRow; col++) {
					//CalendarButton btn = new CalendarButton (lstname[Index],2);
					CalendarButton btn = new CalendarButton (cities[Index].Name,cities[Index].Id);
					//btn.BezelStyle = NSBezelStyle.TexturedSquare;
					if (GlobalSettings.OverNightCitiesInBid.FirstOrDefault (x => x.Name == lstname [Index]) == null) {
						btn.Frame = new CoreGraphics.CGRect (col * (ViewOvernightCities.Frame.Width / 12)+ leftpadding, (ViewOvernightCities.Frame.Height -100) - (row * _itemSize), ViewOvernightCities.Frame.Width / 12, _itemSize);

						btn.Tag = 0;
						btn.Font = NSFont.BoldSystemFontOfSize (13);
						ButtonLayout (btn, 3);

					} else {

						btn.Frame = new CoreGraphics.CGRect (col * (ViewOvernightCities.Frame.Width / 12)+leftpadding, (ViewOvernightCities.Frame.Height -100) - (row * _itemSize), ViewOvernightCities.Frame.Width / 12, _itemSize);

						btn.Tag = 0;
						btn.Font = NSFont.BoldSystemFontOfSize (13);
						ButtonLayout (btn, 2);
					}
					btn.Activated += delegate {
						btn.Tag++;
						setButtonColorsAndState(btn);

					};
					_buttons.Add (btn);

					ViewContent.AddSubview (btn);
					//leftpadding = 0;
					Index++;
				}
			}
			DoneButtonStatus ();
			ScrollViewOvernightCities.DocumentView = ViewOvernightCities;

			ScrollViewOvernightCities.ContentView.ScrollPoint (new CoreGraphics.CGPoint (0, 150));
		}
		void LoadMonth()
		{
			 wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			ViewOvernightCities.WantsLayer = true;
			ViewOvernightCities.Layer.BorderWidth = 1;
			ViewOvernightCities.Layer.BorderColor = NSColor.DarkGray.CGColor;
			ViewOvernightCities.Layer.BorderWidth = (nfloat)0.5;
			ViewOvernightCities.NeedsLayout = true;
			var lstname = GlobalSettings.WBidINIContent.Cities.Select (x => x.Name).ToList ();
			var cities = GlobalSettings.WBidINIContent.Cities;




			_buttons = new List<CalendarButton> ();
			//
			int Index=0;
			for (int row = 0; row < lstname.Count/12; row++) {
				for (int col = 0; col < _numberItemPerRow; col++) {
					//CalendarButton btn = new CalendarButton (lstname[Index],2);
					CalendarButton btn = new CalendarButton (cities[Index].Name,cities[Index].Id);
					//btn.BezelStyle = NSBezelStyle.TexturedSquare;
					if (GlobalSettings.OverNightCitiesInBid.FirstOrDefault (x => x.Name == lstname [Index]) == null) {
						btn.Frame = new CoreGraphics.CGRect (col * (ViewOvernightCities.Frame.Width / 12), (ViewOvernightCities.Frame.Height - 50) - (row * _itemSize), ViewOvernightCities.Frame.Width / 12, _itemSize);

						btn.Tag = 0;
						btn.Font = NSFont.BoldSystemFontOfSize (13);
						ButtonLayout (btn, 3);

					} else {

						btn.Frame = new CoreGraphics.CGRect (col * (ViewOvernightCities.Frame.Width / 12), (ViewOvernightCities.Frame.Height - 50) - (row * _itemSize), ViewOvernightCities.Frame.Width / 12, _itemSize);
				
						btn.Tag = 0;
						btn.Font = NSFont.BoldSystemFontOfSize (13);
						ButtonLayout (btn, 2);
					}
					btn.Activated += delegate {
						btn.Tag++;
						setButtonColorsAndState(btn);

					};
					_buttons.Add (btn);
					ViewOvernightCities.AddSubview (btn);
					Index++;
				}
			}


		}
		void setButtonColorsAndState(NSButton button)
		{
			button.NeedsLayout = true;
			if (_data == null) {
				_data = new OvernightCitiesCx();
			}
			var _city = button.Title;
			if (button.Tag%3 == 0) {
				// remove
				button.Layer.BackgroundColor = NSColor.White.CGColor;
				button.AttributedTitle = new NSAttributedString (button.Title, ApplyAttribure (NSColor.Black));
				button.Alignment = NSTextAlignment.Center;
				if (_data != null && _data.No != null && _data.No.Contains(_city)) {
					_data.No.Remove(_city);
				}

				if (_data != null && _data.Yes != null && _data.Yes.Contains(_city)) {
					_data.Yes.Remove(_city);
				}
			}
			if (button.Tag%3 == 1) {
				button.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
				button.AttributedTitle = new NSAttributedString (button.Title, ApplyAttribure (NSColor.Black));
				button.Alignment = NSTextAlignment.Center;
				if (_data.Yes!= null && !_data.Yes.Contains(_city)) {
					_data.Yes.Add(_city);
				}
				if (_data.Yes == null) {
					_data.Yes = new List<string>();
					_data.Yes.Add(_city);
				}
				if (_data.No != null && _data.No.Contains(_city)) {
					_data.No.Remove(_city);
				}
			}
			if (button.Tag %3  == 2) {
				// no
				button.Layer.BackgroundColor = NSColor.Red.CGColor;
				button.AttributedTitle = new NSAttributedString (button.Title, ApplyAttribure (NSColor.White));
				button.Alignment = NSTextAlignment.Center;
				if (_data.Yes!= null && _data.Yes.Contains(_city)) {
					_data.Yes.Remove(_city);
				}
				if (_data.No != null && !_data.No.Contains(_city)) {
					_data.No.Add(_city);
				}
				if (_data.No == null) {
					_data.No = new List<string>();
					_data.No.Add(_city);
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
			} ;
			return Applied;

		}

		void ButtonLayout(CalendarButton button,int state)
		{
			
			
			button.WantsLayer = true;//NSColor.FromRgba(124/256,206/256,38/256,1).CGColor;
			//var city=wBIdStateContent.BidAuto.BAFilter.FirstOrDefault(x=>x.Name=="OC");
			var city=_BulkOverNightCity;
			if (city != null) 
			{
				if (_data == null)
					_data = new OvernightCitiesCx ();
				
				
				var bidautoobject = city;

				if (bidautoobject.OverNightYes.Contains (button.ID)) 
				{
					
					state = 0;
					if (_data.Yes == null)
						_data.Yes = new List<string> ();
					_data.Yes.Add (button.Title);
					
				} else if (bidautoobject.OverNightNo.Contains (button.ID)) {
					state = 1;
					if (_data.No == null)
						_data.No = new List<string> ();
					_data.No.Add (button.Title);
				}
			}

            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");

            switch (state) {
			case 0:
				button.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
				break;
			case 1:
				button.Layer.BackgroundColor = NSColor.Red.CGColor;
                  
                    break;
			case 2:
                    button.Layer.BackgroundColor = NSColor.White.CGColor;
                    if (interfaceStyle == "Dark") {
                        button.AttributedTitle = new NSAttributedString(button.Title, ApplyAttribure(NSColor.Black));
                    }
                  
                    break;
			case 3:
				button.Layer.BackgroundColor = NSColor.Black.CGColor;
                    if (interfaceStyle == "Dark")
                    {
                        button.AttributedTitle = new NSAttributedString(button.Title, ApplyAttribure(NSColor.White));
                    }
                    else {
                        button.AttributedTitle = new NSAttributedString(button.Title, ApplyAttribure(NSColor.White));
                    }
                    // button.Enabled = false;
                    break;

			}
            //			if (state) button.Layer.BackgroundColor = NSColor.FromCalibratedRgba (.48f, .80f, .14f, 1).CGColor;
            //			else button.Layer.BackgroundColor = NSColor.Orange.CGColor;//NSColor.FromCalibratedRgba(.91f, .51f, .21f, 1).CGColor;
          
            button.Layer.CornerRadius = (nfloat)2;
			button.Layer.BorderColor = NSColor.DarkGray.CGColor;
			button.Layer.BorderWidth = (nfloat)0.5;


		}

		partial void funClearAction (NSObject sender)
		{
			if (_data != null) {
				if (_data.No != null) {
					_data.No.Clear();
				}
				if (_data.Yes != null) {
					_data.Yes.Clear();
				}
			}
            if (_BulkOverNightCity != null) {
                if (_BulkOverNightCity.OverNightNo != null) {
                    _BulkOverNightCity.OverNightNo.Clear();
                }
                if (_BulkOverNightCity.OverNightYes != null) {
                    _BulkOverNightCity.OverNightYes.Clear();
                }
            }
           
			LoadMonthInScrollView();

		}
		partial void FunCancelAction (NSObject sender)
		{
			this.View.Window.Close();
			this.View.Window.OrderOut(this);
		}
		partial void funDoneActions (NSObject sender)
		{
			
			BidAutoItem bidAutoItem = new BidAutoItem ();

			bidAutoItem.Name = "OC";

			if (_BulkOverNightCity!= null)
			{
				_BulkOverNightCity.OverNightYes=GetOvernightCityIdListFromName(_data.Yes);
				_BulkOverNightCity.OverNightNo=GetOvernightCityIdListFromName(_data.No);
				NSNotificationCenter.DefaultCenter.PostNotificationName ("UpdateOverNightCitiesNotification", null);
			}
			else
			{
				if(_data!=null)
				{
					bidAutoItem.BidAutoObject = new BulkOvernightCityCx()
					{
						OverNightYes=GetOvernightCityIdListFromName(_data.Yes ),
						OverNightNo=GetOvernightCityIdListFromName(_data.No)
					};
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					bidAutoItem.IsApplied=false;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
				}


				NSNotificationCenter.DefaultCenter.PostNotificationName ("OverNightCitiesNotification", null);
			}
			CommonClass.MainController.UpdateSaveButton (true);
//			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
//			if(wBIdStateContent.BidAuto.BAFilter.Any(x=>x.Name=="OC"))
//			{
//				wBIdStateContent.BidAuto.BAFilter.FirstOrDefault(x=>x.Name=="OC").BidAutoObject=new BulkOvernightCityCx()
//				{
//					OverNightYes=GetOvernightCityIdListFromName(_data.Yes),
//					OverNightNo=GetOvernightCityIdListFromName(_data.No)
//				};
//			}
//			else
//			{
//				if(_data!=null)
//				{
//				bidAutoItem.BidAutoObject = new BulkOvernightCityCx()
//				{
//					OverNightYes=GetOvernightCityIdListFromName(_data.Yes ),
//					OverNightNo=GetOvernightCityIdListFromName(_data.No)
//				};
//				wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
//				}
			//			}UpdateOverNightCitiesNotification

				this.View.Window.Close();
				this.View.Window.OrderOut(this);

		}


		void DoneButtonStatus()
		{
				
			btnDone.Enabled = false;

			if ((_data != null) && (((_data.No != null)&&(_data.No.Count >0))|| ((_data.Yes != null)&&(_data.Yes.Count>0)))) {
				btnDone.Enabled = true;
						}
		}
		public static List<int> GetOvernightCityIdListFromName(List<string> cities)
		{
			List<int> lstCity = new List<int>();
			if (cities != null && cities.Count > 0)
			{
				foreach (string city in cities)
				{
					var objCity = GlobalSettings.OverNightCitiesInBid.FirstOrDefault(x => x.Name == city);
					if (objCity != null)
					{
						lstCity.Add(objCity.Id);

					}
				}
			}

			return lstCity;
		}
		#endregion

		//strongly typed view accessor
		public new OvernightCitiesDetails View {
			get {
				return (OvernightCitiesDetails)base.View;
			}
		}
	}
}
