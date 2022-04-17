using System;

using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.iOS.Utility;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.Generic;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class ShowCAPController : NSWindowController
	{
		List<CAPOutputParameter> lstCapValues;
		public ShowCAPController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ShowCAPController (NSCoder coder) : base (coder)
		{
		}

		public ShowCAPController () : base ("ShowCAP")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.Window.WillClose += delegate {
				this.Window.OrderOut (this);
				NSApplication.SharedApplication.StopModal ();
			};
			GetData();
			tblShowCAP.Source = new ShowCapTableSource (this);
			lblPrevMonth.StringValue = MonthValues((DateTime.Now.Month));
			lblCurrMonth.StringValue = MonthValues((DateTime.Now.Month+1));
		}

		string MonthValues(int month)
		{
			string objMonth="";
			switch (month)
			{
			case 1:case 13:
				objMonth="Jan";
				break;
			case 2:
				objMonth="Feb";
				break;
			case 3:
				objMonth="Mar";
				break;
			case 4:
				objMonth="Apr";
				break;
			case 5:
				objMonth = "May";
				break;
			case 6:
				objMonth = "Jun";
				break;
			case 7:
				objMonth = "Jul";
				break;
			case 8:
				objMonth = "Aug";
				break;
			case 9:
				objMonth = "Sep";
				break;
			case 10:
				objMonth = "Oct";
				break;
			case 11:
				objMonth = "Nov";
				break;
			case 12:case 0:
				objMonth = "Dec";
				break;

			}

			return objMonth;
		}
		public partial class ShowCapTableSource : NSTableViewSource
		{
			ShowCAPController showCapVC;

			public ShowCapTableSource (ShowCAPController show)
			{
				showCapVC = show;
			}

			public override nint GetRowCount (NSTableView tableView)
			{
				return showCapVC.lstCapValues.Count;
			}

			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				if (tableColumn.Identifier == "base") {
					return  (NSString)showCapVC.lstCapValues [(int)row].Domicile;
						
				}
				else if(tableColumn.Identifier == "seat") {
					return  (NSString)showCapVC.lstCapValues [(int)row].Position;

				}
				else if(tableColumn.Identifier == "prevMonth") {
					return  (NSString)showCapVC.lstCapValues [(int)row].PreviousMonthCap.ToString ();

				}
				else if(tableColumn.Identifier == "currMonth") {
					return  (NSString)showCapVC.lstCapValues [(int)row].CurrentMonthCap.ToString ();

				}

				return new NSString ();	
			}

			public override void SelectionDidChange (NSNotification notification)
			{
				
//				var table = (NSTableView)notification.Object;
//				if (table.SelectedRowCount > 0) {
//					parentVC.SelectedTripName = parentVC.lstTrips [(int)table.SelectedRow];
//					parentVC.GeneratePairingDetails ();
//					parentVC.tblDays.ReloadData ();
//				}
			}
		}


		public new ShowCAP Window {
			get { return (ShowCAP)base.Window; }
		}
		private void GetData()
		{
			// MessageBoxResult msgResult;
            bool isConnectionAvailable = Reachability.CheckVPSAvailable ();

			if (isConnectionAvailable) {
				CAPInputParameter capInput = new CAPInputParameter ();
				capInput.Month = DateTime.Now.AddMonths (1).Month;
				capInput.Year = DateTime.Now.AddMonths (1).Year;
				var jsonData = ServiceUtils.JsonSerializer (capInput);
				StreamReader dr = ServiceUtils.GetRestData ("GetCAPData", jsonData);
				lstCapValues = WBidCollection.ConvertJSonStringToObject<List<CAPOutputParameter>> (dr.ReadToEnd ());
			} else 
			{
                string alertmessage = GlobalSettings.VPSDownAlert;
                if (Reachability.IsSouthWestWifiOr2wire())
                {
                    alertmessage = GlobalSettings.SouthWestConnectionAlert;
                }
                ShowMessageBox ("WBidMax", alertmessage);
			}
		}
		private void ShowMessageBox (string title, string content)
		{
			var alert = new NSAlert ();
			alert.MessageText = title;
			alert.InformativeText = content;
			alert.RunModal ();
		}
	}
}
