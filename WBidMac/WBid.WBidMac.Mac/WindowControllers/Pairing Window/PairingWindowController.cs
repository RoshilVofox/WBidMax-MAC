
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using System.Collections.Generic;
using WBid.WBidiPad.Core;

//using System.Linq;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary;

//using MonoTouch.EventKit;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidMac.Mac
{
	public partial class PairingWindowController : AppKit.NSWindowController
	{
		#region Constructors

		public List <string> lstTrips;
		public ObservableCollection <DateTime> lstDates = new ObservableCollection<DateTime> ();
		ObservableCollection<TripData> tripData = new ObservableCollection<TripData> ();

		public string SelectedTripName;
		public DateTime SelectedDate;

		// Called when created from unmanaged code
		public PairingWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PairingWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PairingWindowController () : base ("PairingWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new PairingWindow Window {
			get {
				return (PairingWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.ShouldCascadeWindows = false;
			btnFFDO.Hidden = !(GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO");
			this.Window.WillClose += delegate {
				this.Window.OrderOut (this);
				NSApplication.SharedApplication.StopModal ();
			};

			lstTrips = GlobalSettings.Trip.Select (x => x.TripNum).ToList ();

			tblTrips.Source = new TripNameTableSource (this);
			tblDays.Source = new DaysTableSource (this);

			txtPairing.Font = NSFont.FromFontName ("Courier", 12);
			btnPrint.Activated += btnPrintClicked;
			btnFFDO.Activated += btnFFDOClicked;
		}

		void btnFFDOClicked (object sender, EventArgs e)
		{
			try {
				string ffdoData = GetFlightDataforFFDB (lstTrips[(int)tblTrips.SelectedRow], lstDates[(int)tblDays.SelectedRow].Date);
				NSPasteboard clipBoard = NSPasteboard.GeneralPasteboard;
				clipBoard.DeclareTypes (new string[]{ "NSStringPboardType" }, null);
				clipBoard.SetStringForType (ffdoData, "NSStringPboardType");
				var alert = new NSAlert ();
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Trips has been copied to ClipBoard!";

				alert.RunModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		/// <summary>
		/// PURPOSE : Get Flight data for FFDB
		/// </summary>
		/// <param name="trip"></param>
		/// <param name="tripName"></param>
		private string GetFlightDataforFFDB(string tripNum,DateTime tripDate)
		{

			Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum.Substring(0,4));
			trip = trip ?? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripNum);
			string result = string.Empty;

			// var tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(SelectedDay.Replace(" ", "")));
			//DateTime dutPeriodDate = WBidCollection.SetDate(int.Parse(tripNum.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);
			DateTime dutPeriodDate = tripDate;

			foreach (var dp in trip.DutyPeriods)
			{
				string datestring = dutPeriodDate.ToString("MM'/'dd'/'yyyy");
				if (trip.ReserveTrip)
				{

					result += datestring + "  RSRV " + trip.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveOut % 1440)).Replace(":", "") + " " + trip.RetSta + " " + Helper.CalcTimeFromMinutesFromMidnight(Convert.ToString(dp.ReserveIn % 1440)).Replace(":", "") + " \n";
				}
				else
				{
					foreach (var flt in dp.Flights)
					{
						result += datestring + " " + flt.FltNum.ToString().PadLeft(4, '0') + " " + flt.DepSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.DepTime.ToString()).Replace(":", "") + " " + flt.ArrSta + " " + Helper.CalcTimeFromMinutesFromMidnight(flt.ArrTime.ToString()).Replace(":", "") + " \n";
					}
				}
				dutPeriodDate = dutPeriodDate.AddDays(1);
			}
			return result;
		}

		void btnPrintClicked (object sender, EventArgs e)
		{
			try {
				var content = txtPairing.Value;
				var inv = new InvisibleWindowController ();
				this.Window.AddChildWindow (inv.Window, NSWindowOrderingMode.Below);
				var txt = new NSTextView (new CGRect (0, 0, 550, 550));
				txt.Font = NSFont.FromFontName ("Courier", 10);
				inv.Window.ContentView.AddSubview (txt);
				txt.Value = content;
				var pr = NSPrintInfo.SharedPrintInfo;
				pr.VerticallyCentered = false;
				pr.TopMargin = 2.0f;
				pr.BottomMargin = 2.0f;
				pr.LeftMargin = 1.0f;
				pr.RightMargin = 1.0f;
				txt.Print (this);
				inv.Close ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void GeneratePairingDetails ()
		{

			// TitleDetails = "Pairing " + SelectedTripName;
			// MainViewModel mainView = ((MainViewModel)ServiceLocator.Current.GetInstance<MainViewModel>());
			//  ObservableCollection<Line> lines = mainView.Lines;

			var SelectedTrip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == SelectedTripName.Substring (0, 4));

			if (SelectedTrip == null) {
				SelectedTrip = GlobalSettings.Trip.FirstOrDefault (x => x.TripNum == SelectedTripName);

			}
			lstDates = new ObservableCollection<DateTime> ();
			if (SelectedTrip != null) {
				if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
					GenerateDates (SelectedTrip.GtripOpVector);
				} else {

					List<string> tempList = GlobalSettings.Lines.SelectMany (x => x.Pairings).Where (x => x.StartsWith (SelectedTripName)).OrderBy (x => x.ToString ()).ToList ();
					for (int count = 0; count < tempList.Count; count++) {
						lstDates.Add (new DateTime (GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse (tempList [count].Substring (4, 2))));
					}
				}


			}

			if (lstDates.Count == 0) {


				DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
				while (startDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate) {
					lstDates.Add (startDate);
					startDate = startDate.AddDays (1);
				}

			}
			CorrectionParams correctionParams = new WBid.WBidiPad.Model.CorrectionParams ();
			//correctionParams.selectedLineNum = CommonClass.selectedLine;
			tripData = TripViewBL.GenerateTripDetails (SelectedTripName + SelectedDate.Day.ToString ().PadLeft (2, '0'), correctionParams, false);

			var content = string.Empty;
			foreach (TripData data in tripData) {
				content += "\n";
				content += data.Content;
			}

			txtPairing.Value = content;


		}

		private void GenerateDates (string gtripOpVector)
		{

			//if (gtripOpVector.Trim() == string.Empty)
			if (string.IsNullOrEmpty (gtripOpVector)) {
				return;
			}
			DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
			foreach (var item in gtripOpVector) {
				if (startDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
					break;
				if (item == '1') {
					lstDates.Add (startDate);
				}
				startDate = startDate.AddDays (1);
			}
		}


	}

	public partial class TripNameTableSource : NSTableViewSource
	{
		PairingWindowController parentVC;

		public TripNameTableSource (PairingWindowController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.lstTrips.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return new NSString (parentVC.lstTrips [(int)row]);		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				parentVC.SelectedTripName = parentVC.lstTrips [(int)table.SelectedRow];
				parentVC.GeneratePairingDetails ();
				parentVC.tblDays.ReloadData ();
			}
		}
	}

	public partial class DaysTableSource : NSTableViewSource
	{
		PairingWindowController parentVC;

		public DaysTableSource (PairingWindowController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.lstDates.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return new NSString (parentVC.lstDates [(int)row].Day.ToString ());		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				parentVC.SelectedDate = parentVC.lstDates [(int)table.SelectedRow];
				parentVC.GeneratePairingDetails ();
			}
		}
	}

}

