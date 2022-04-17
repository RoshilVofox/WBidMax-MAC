using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using System.Drawing;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Globalization;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidMac.Mac
{
//	public class ConnectTimeHelper
//	{
//		public string Day { get; set; }
//
//		public string Arrival { get; set; }
//
//		public string Departure { get; set; }
//
//		public bool IsEnabled { get; set; }
//
//		public DateTime Date { get; set; }
//
//
//	}
	public partial class ArrivalAndDepartViewController : AppKit.NSViewController
	{
		#region Constructors
		NSObject[]	titles ;
		public ObservableCollection<ConnectTimeHelper> ListCommuteTime;
		WBidState	wBIdStateContent;

		public string City;
		public string Base;
		public bool IsNonStop;
		//public enum CommutableAutoFrom
		//{
		//	Constraints = 1,
		//	Weight,Filters,CommutabilityConstraint,CommutabilityWeight
		//}
		public	CommutableAutoFrom ObjArrivaltype;
		// Called when created from unmanaged code
		public ArrivalAndDepartViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
			NSTextField lbl = new NSTextField ();

		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ArrivalAndDepartViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public ArrivalAndDepartViewController () : base ("ArrivalAndDepartView", NSBundle.MainBundle)
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
			CommonClass.ArrivalPageIndex = 0;
			CommonClass.ListCommuteTime= new ObservableCollection<ConnectTimeHelper>();
			GenerateCommuteTimes ();
			string monthandDateTitle= new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM", CultureInfo.InvariantCulture) + " " + GlobalSettings.CurrentBidDetails.Year; 
			string title = IsNonStop ? monthandDateTitle + " (Non Stop)" : monthandDateTitle;
			lblBase.StringValue = GlobalSettings.CurrentBidDetails.Domicile;

			lblDate.StringValue = title;
			lblCommuteCity.StringValue = City;


		}

		public void LoadColelctionView()
		{
			MyCollectionViewItem2 CollectionItem = new MyCollectionViewItem2 (ListCommuteTime);
			CollectionItem.ObjCollectiontype = (MyCollectionViewItem2.CommutableAutoFrom)ObjArrivaltype;


			CommonClass.ListCommuteTime = ListCommuteTime;

			ArrivalAndDepartedCollectionView.ItemPrototype = CollectionItem;
//			NSObject[] titles= new NSObject[ListCommuteTime.Count];
//
//
//			for (int cnt = 0; cnt < ListCommuteTime.Count; cnt++) {
//
//
//				
//				titles [cnt] = (NSString)cnt.ToString();
//
//			}

			//NSArray arrList = ListCommuteTime.ToArray ();



			ArrivalAndDepartedCollectionView.Content = new NSObject[42];
			ArrivalAndDepartedCollectionView.NeedsDisplay = true;

			//ArrivalAndDepartedCollectionView
		}


		public void GenerateCommuteTimes()
		{


			try
			{
				ListCommuteTime = new ObservableCollection<ConnectTimeHelper>();
				DateTime calendarDate;
				DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;

				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (GlobalSettings.CurrentBidDetails.Month == 3 || GlobalSettings.CurrentBidDetails.Month == 2))
				{
					startDate = startDate.AddDays(-1);
				}

				int iterationCount = (int)startDate.DayOfWeek;

				int id = 1;
				calendarDate = startDate;
				for (int count = 0; count < iterationCount; count++)
				{
					ListCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = false, Day = null });
					calendarDate = calendarDate.AddDays(1);
					id++;
				}

				iterationCount = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Subtract(startDate).Days + 1;
				calendarDate = startDate;
				for (int count = 1; count <= iterationCount; count++)
				{
					ListCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = true, Day = calendarDate.Day.ToString(), Date = calendarDate.Date });
					calendarDate = calendarDate.AddDays(1);
					id++;
				}


				bool status = true;
				for (int count = 1; count <= 35 - iterationCount; count++)
				{
					ListCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = status, Day = calendarDate.Day.ToString(), Date = calendarDate.Date });
					calendarDate = calendarDate.AddDays(1);
					if (status && calendarDate.Month != GlobalSettings.CurrentBidDetails.Month)
						status = false;
					id++;
				}

				for (int count = id; count <= 42; count++)
				{
					ListCommuteTime.Add(new ConnectTimeHelper() { IsEnabled = false, Day = calendarDate.Day.ToString(), Date = calendarDate.Date });
					calendarDate = calendarDate.AddDays(1);
					id++;

				}



				foreach (var item in ListCommuteTime)
				{
					CommuteTime obj=null;
					wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					switch (ObjArrivaltype) {
					case CommutableAutoFrom.Constraints:
                    case CommutableAutoFrom.Weight:
                    case CommutableAutoFrom.Sort:
                            obj = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						break;
					//case CommutableAutoFrom.Weight:
      //                      obj =wBIdStateContent.Weights.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						//break;
					case CommutableAutoFrom.Filters:
						obj = wBIdStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						break;
					case CommutableAutoFrom.CommutabilityConstraint:
					case CommutableAutoFrom.CommutabilityWeight:
						obj = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == item.Date.Date);
						break;
					}
					
					if (obj != null)
					{ 
						item.Arrival = (obj.EarliestArrivel==DateTime.MinValue)?string.Empty: obj.EarliestArrivel.ToString("HH:mm").Replace(":", "");
						item.Departure = (obj.LatestDeparture==DateTime.MinValue)?string.Empty:  obj.LatestDeparture.ToString("HH:mm").Replace(":", "");
					}




				}
				ListCommuteTime.Insert (0, new ConnectTimeHelper {Day="",Arrival="",Departure="",IsEnabled=false});
				LoadColelctionView();

			}

			catch (Exception ex)
			{
//				WBidHelper.WriteLog(ex);
//				WBidHelper.ShowMessage(WBidErrorMessages.CommonError);
			}
		}

		partial void btnOk (NSObject sender)
		{
			this.View.Window.Close();
			this.View.Window.OrderOut(this);
		}
		#endregion

		//strongly typed view accessor
		public new ArrivalAndDepartView View {
			get {
				return (ArrivalAndDepartView)base.View;
			}
		}
	}



	public class MyCollectionViewItem2 : NSCollectionViewItem
	{
		private static readonly NSString EMPTY_NSSTRING = new NSString(string.Empty);
		private MyView2 view;
		int Index=0;

		public ObservableCollection<ConnectTimeHelper> ListCommuteTime;
		public enum CommutableAutoFrom
		{
			Constraints = 1,
			Weight,Filters
		}
		public	CommutableAutoFrom ObjCollectiontype;




		public MyCollectionViewItem2(ObservableCollection<ConnectTimeHelper> Lists) : base()
		{
			ListCommuteTime = Lists;
		}

		public MyCollectionViewItem2(IntPtr ptr) : base(ptr)
		{

		}

		public override void LoadView ()
		{
			
			view = new MyView2();

			//ArrivalCell = view;
			View = view;
		//	View.Layer.BackgroundColor=NSColor.Red.CGColor;
		}
//		public new ArrivalDepartCell baseView {
//			get {
//				return (ArrivalDepartCell)base.View;
//			}
//		}
	
		public override NSObject RepresentedObject 
		{
			get { return base.RepresentedObject; }

			set 
			{
				//var data= (ConnectTimeHelper)value;

				if (value == null)
				{
					
					// Need to do this because setting RepresentedObject in base to null 
					// throws an exception because of the MonoMac generated wrappers,
					// and even though we don't have any null values, this will get 
					// called during initialization of the content with a null value.
					base.RepresentedObject = EMPTY_NSSTRING;
					//view.Button.Title = string.Empty;
				}
				else
				{
					
					base.RepresentedObject = value;
				

				}
			}
		}

		public void BindData (string day,string arrival,string depart)
		{
			view.lblday.StringValue = day;
			view.lblArival.StringValue = arrival;
			view.lblDeparture.StringValue = depart;
		}
	}

	public class MyView2 : NSView
	{
		public NSButton button;
		public NSTextField lblday;
		public NSTextField lblBackground;
		public NSTextField lblArival;
		public NSTextField lblDeparture;
		public MyView2() : base(new CoreGraphics.CGRect(0,0,72,86))
		{

			lblBackground = new NSTextField(new  CoreGraphics.CGRect(0,0,74,86));
			lblBackground.DrawsBackground = true;
			lblBackground.BackgroundColor = NSColor.White;
		
			lblBackground.Alignment = NSTextAlignment.Center;


			lblday = new NSTextField(new  CoreGraphics.CGRect(0,61,74,25));
			lblday.DrawsBackground = true;

			if (!CommonClass.ListCommuteTime [CommonClass.ArrivalPageIndex].IsEnabled)
				lblday.BackgroundColor = NSColor.LightGray;
			else lblday.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1);



            if (CommonClass.ArrivalPageIndex <= 42)
				lblday.StringValue = CommonClass.ListCommuteTime [CommonClass.ArrivalPageIndex].Day ?? string.Empty;
			else
				lblday.StringValue = string.Empty;
			lblday.Alignment = NSTextAlignment.Center;

			lblArival = new NSTextField(new  CoreGraphics.CGRect(0,31,73,25));
			lblArival.DrawsBackground = true;
			lblArival.BackgroundColor = NSColor.White;
			if (CommonClass.ArrivalPageIndex <=42)

			lblArival.StringValue=CommonClass.ListCommuteTime[CommonClass.ArrivalPageIndex].Arrival??string.Empty;
			else
				lblArival.StringValue=string.Empty;
			
			
			lblArival.Alignment = NSTextAlignment.Left;

			lblDeparture = new NSTextField(new  CoreGraphics.CGRect(2,5,69,25));
						lblArival.DrawsBackground = true;
			lblArival.BackgroundColor = NSColor.White;
			if (CommonClass.ArrivalPageIndex <= 42)
			lblDeparture.StringValue=CommonClass.ListCommuteTime[CommonClass.ArrivalPageIndex].Departure??string.Empty;
			else
				lblDeparture.StringValue=string.Empty;
			lblDeparture.Alignment = NSTextAlignment.Right;
			lblArival.Editable = false;
			lblday.Editable = false;
			lblDeparture.Editable = false;
			lblBackground.Editable = false;
		


			//lblArival.Editable=false
			AddSubview(lblBackground);
			AddSubview(lblday);
			AddSubview(lblArival);
			AddSubview(lblDeparture);
			CommonClass.ArrivalPageIndex++;
		}

		public NSButton Button
		{
			get { return button; }	
		}		
	}

}







