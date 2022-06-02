using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using System.Net;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.IO.Compression;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidMac.Mac
{
	public partial class CommutableViewController : AppKit.NSViewController
	{
		

		//public enum CommutableAutoFrom
		//{
		//	Constraints = 1,
		//	Weight,Filters,Commutability,Sort
		//}
		public	CommutableAutoFrom Objtype;
		ObservableCollection<City> _lstCommuteCities;
		string[] _arrCities;
		string[] _arrConnectTime;
		string[] _arrCheckInTime;
		string[] _arrBacktoBaseTime;
		private List<FlightRouteDetails> _flightRouteDetails;
		private int _depTime;
		private int _arrTime;
		WBidState WBidStateContent;
		
		#region Constructors

		// Called when created from unmanaged code
		public CommutableViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public CommutableViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public CommutableViewController () : base ("CommutableView", NSBundle.MainBundle)
		{
			Initialize ();
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
        partial void funDoneAction(NSObject sender)
        {
            FtCommutableLine ftCommutableLine;
            WtCommutableLineAuto wtCommutableLineAuto;
            CalculateLineProperties lineproprty = new CalculateLineProperties();
			var selectedConnectTime = popUpConnectTime.Title == "--:--" ? "00:00" : popUpConnectTime.Title;
			switch (Objtype)
            {
                case CommutableAutoFrom.Constraints:
                    if (WBidStateContent.CxWtState.CLAuto.Cx)
                    {
                        ftCommutableLine = WBidStateContent.Constraints.CLAuto;
                    }
                    else
                    {
                        ftCommutableLine = new FtCommutableLine();
                        ftCommutableLine.ToHome = true;
                        WBidStateContent.Constraints.CLAuto = ftCommutableLine;
                        WBidStateContent.CxWtState.CLAuto.Cx = true;
                    }

                    if (popUpCommuteCity.Title != "Not set")
                    {
                        ftCommutableLine.ConnectTime = ConvertHHMMToMinute(selectedConnectTime);
                        ftCommutableLine.City = popUpCommuteCity.Title;
                        ftCommutableLine.CommuteCity = _lstCommuteCities.FirstOrDefault(x => x.Name == popUpCommuteCity.Title).Id;
                        ftCommutableLine.CheckInTime = ConvertHHMMToMinute(popUpCheckInTime.Title);
                        ftCommutableLine.BaseTime = ConvertHHMMToMinute(popUpBackTobase.Title);
						ftCommutableLine.IsNonStopOnly= (btnNonStop.State == NSCellStateValue.On);
					}
                    //set the same commtble line common properties to weight also becuase both constraints weights and sorts using the same values
                    if (WBidStateContent.Weights.CLAuto == null)
                        WBidStateContent.Weights.CLAuto = new WtCommutableLineAuto();
                    WBidStateContent.Weights.CLAuto.ConnectTime = ftCommutableLine.ConnectTime;
                    WBidStateContent.Weights.CLAuto.City = ftCommutableLine.City;
                    WBidStateContent.Weights.CLAuto.CommuteCity = ftCommutableLine.CommuteCity;
                    WBidStateContent.Weights.CLAuto.CheckInTime = ftCommutableLine.CheckInTime;
                    WBidStateContent.Weights.CLAuto.BaseTime = ftCommutableLine.BaseTime;
					WBidStateContent.Weights.CLAuto.IsNonStopOnly = ftCommutableLine.IsNonStopOnly;

					if (File.Exists(WBidHelper.WBidCommuteFilePath))
					{
						File.Delete(WBidHelper.WBidCommuteFilePath);

					}
					lineproprty.CalculateCommuteLineProperties(WBidStateContent);

                    if (WBidStateContent.CxWtState.CLAuto.Wt)
                        CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Auto");

                    NSNotificationCenter.DefaultCenter.PostNotificationName("CLONotification", null);
                    break;

                case CommutableAutoFrom.Weight:

                    if (WBidStateContent.CxWtState.CLAuto.Wt)
                    {
                        wtCommutableLineAuto = WBidStateContent.Weights.CLAuto;
                    }
                    else
                    {
                        wtCommutableLineAuto = new WtCommutableLineAuto();
                        wtCommutableLineAuto.ToHome = true;
                        WBidStateContent.Weights.CLAuto = wtCommutableLineAuto;
                        WBidStateContent.CxWtState.CLAuto.Wt = true;
                    }

                    if (popUpCommuteCity.Title != "Not set")
                    {
                        wtCommutableLineAuto.ConnectTime = ConvertHHMMToMinute(selectedConnectTime);
                        wtCommutableLineAuto.City = popUpCommuteCity.Title;
                        wtCommutableLineAuto.CommuteCity = _lstCommuteCities.FirstOrDefault(x => x.Name == popUpCommuteCity.Title).Id;
                        wtCommutableLineAuto.CheckInTime = ConvertHHMMToMinute(popUpCheckInTime.Title);
                        wtCommutableLineAuto.BaseTime = ConvertHHMMToMinute(popUpBackTobase.Title);
						wtCommutableLineAuto.IsNonStopOnly = (btnNonStop.State == NSCellStateValue.On);
					}
                    //set the same commtble line common properties to constraints also
                    if (WBidStateContent.Constraints.CLAuto == null)
                        WBidStateContent.Constraints.CLAuto = new FtCommutableLine();
                    WBidStateContent.Constraints.CLAuto.ConnectTime = wtCommutableLineAuto.ConnectTime;
                    WBidStateContent.Constraints.CLAuto.City = wtCommutableLineAuto.City;
                    WBidStateContent.Constraints.CLAuto.CommuteCity = wtCommutableLineAuto.CommuteCity;
                    WBidStateContent.Constraints.CLAuto.CheckInTime = wtCommutableLineAuto.CheckInTime;
                    WBidStateContent.Constraints.CLAuto.BaseTime = wtCommutableLineAuto.BaseTime;
					WBidStateContent.Constraints.CLAuto.IsNonStopOnly = wtCommutableLineAuto.IsNonStopOnly;
                    lineproprty.CalculateCommuteLineProperties(WBidStateContent);

					if (File.Exists(WBidHelper.WBidCommuteFilePath))
					{
						File.Delete(WBidHelper.WBidCommuteFilePath);

					}
					if (WBidStateContent.CxWtState.CLAuto.Cx)
                        CommonClass.ConstraintsController.ApplyAndReloadConstraints("Commutable Lines - Auto");
					// NSNotificationCenter.DefaultCenter.PostNotificationName("CLWeightNotification", null);
					NSNotificationCenter.DefaultCenter.PostNotificationName("CLWeightNotification", null);
					break;
                case CommutableAutoFrom.Filters:


                    if (popUpCommuteCity.Title != "Not set")
                    {
                        if (WBidStateContent.BidAuto.BAFilter.FirstOrDefault(x => x.Name == "CL") != null)
                        {
                            ftCommutableLine = (FtCommutableLine)WBidStateContent.BidAuto.BAFilter.FirstOrDefault(x => x.Name == "CL").BidAutoObject;

                        }
                        else
                        {
                            ftCommutableLine = new FtCommutableLine();
                            WBidStateContent.BidAuto.BAFilter.Add(new BidAutoItem { Name = "CL", Priority = WBidStateContent.BidAuto.BAFilter.Count, BidAutoObject = ftCommutableLine });
                            ftCommutableLine.ToHome = true;

                        }



                        ftCommutableLine.ConnectTime = ConvertHHMMToMinute(selectedConnectTime);
                        ftCommutableLine.City = popUpCommuteCity.Title;
                        ftCommutableLine.CommuteCity = _lstCommuteCities.FirstOrDefault(x => x.Name == popUpCommuteCity.Title).Id;
                        ftCommutableLine.CheckInTime = ConvertHHMMToMinute(popUpCheckInTime.Title);
                        ftCommutableLine.BaseTime = ConvertHHMMToMinute(popUpBackTobase.Title);
						ftCommutableLine.IsNonStopOnly = (btnNonStop.State == NSCellStateValue.On);
					}
					if (File.Exists(WBidHelper.WBidCommuteFilePath))
					{
						File.Delete(WBidHelper.WBidCommuteFilePath);

					}
					CommonClass.MainController.UpdateSaveButton(true);
                    NSNotificationCenter.DefaultCenter.PostNotificationName("CLONotification", null);
                    break;
                case CommutableAutoFrom.Sort:
                    if (WBidStateContent.SortDetails.BlokSort.Contains("33") || WBidStateContent.SortDetails.BlokSort.Contains("34") || WBidStateContent.SortDetails.BlokSort.Contains("35"))
                    {
                        ftCommutableLine = WBidStateContent.Constraints.CLAuto;
                        ftCommutableLine.City = WBidStateContent.Constraints.CLAuto.City;
                        WBidStateContent.Constraints.CLAuto.City = popUpCommuteCity.Title;
                    }
                    else
                    {
                        ftCommutableLine = new FtCommutableLine { BaseTime = 10, ConnectTime = 30, CheckInTime = 60 };

                    }

                    if (popUpCommuteCity.Title != "Not set")
                    {
                        ftCommutableLine.ConnectTime = ConvertHHMMToMinute(selectedConnectTime);
                        ftCommutableLine.City = popUpCommuteCity.Title;
                        //ftCommutableLine.CommuteCity=_lstCommuteCities.FirstOrDefault(x=>x.Name==popUpCommuteCity.Title).Id;
                        ftCommutableLine.CheckInTime = ConvertHHMMToMinute(popUpCheckInTime.Title);
                        ftCommutableLine.BaseTime = ConvertHHMMToMinute(popUpBackTobase.Title);
						ftCommutableLine.IsNonStopOnly = (btnNonStop.State == NSCellStateValue.On);
					}


                    //Common

                    WBidStateContent.Constraints.CLAuto.ConnectTime = ftCommutableLine.ConnectTime;
                    WBidStateContent.Constraints.CLAuto.CheckInTime = ftCommutableLine.CheckInTime;
                    WBidStateContent.Constraints.CLAuto.City = popUpCommuteCity.Title;
                    WBidStateContent.Constraints.CLAuto.BaseTime = ftCommutableLine.BaseTime;
					WBidStateContent.Constraints.CLAuto.IsNonStopOnly = ftCommutableLine.IsNonStopOnly;

					//set the same commtble line common properties to weight also becuase both constraints weights and sorts using the same values
					if (WBidStateContent.Weights.CLAuto == null)
                        WBidStateContent.Weights.CLAuto = new WtCommutableLineAuto();
                    WBidStateContent.Weights.CLAuto.ConnectTime = ftCommutableLine.ConnectTime;
                    WBidStateContent.Weights.CLAuto.City = ftCommutableLine.City;
                    WBidStateContent.Weights.CLAuto.CommuteCity = ftCommutableLine.CommuteCity;
                    WBidStateContent.Weights.CLAuto.CheckInTime = ftCommutableLine.CheckInTime;
                    WBidStateContent.Weights.CLAuto.BaseTime = ftCommutableLine.BaseTime;
					WBidStateContent.Weights.CLAuto.IsNonStopOnly = ftCommutableLine.IsNonStopOnly;


					lineproprty.CalculateCommuteLineProperties(WBidStateContent);

					if (File.Exists(WBidHelper.WBidCommuteFilePath))
					{
						File.Delete(WBidHelper.WBidCommuteFilePath);

					}
					
					//we dont need to appy block sort logic becuase it will calulates in the notifications
					if (WBidStateContent.CxWtState.CLAuto.Cx)
                        CommonClass.ConstraintsController.ApplyAndReloadConstraints("Commutable Lines - Auto");
                    if (WBidStateContent.CxWtState.CLAuto.Wt)
                        CommonClass.WeightsController.ApplyAndReloadWeights("Commutable Lines - Auto");
                    NSNotificationCenter.DefaultCenter.PostNotificationName("CmtbltyAutoSortNotification", null);
                    break;

            }
			CommonClass.MainController.SetFlightDataDiffButton();
			this.View.Window.Close();
            this.View.Window.OrderOut(this);
           
        }
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			GenerateCommuteCities ();
			GenerateConnectTime ();
		
			GenerateCheckInTime ();

			GenerateBackToBaseTime ();
		
			WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			bool isApplied=false;
			FtCommutableLine ftCommutableLine;
			WtCommutableLineAuto wtCommutableLineAuto;
			switch (Objtype) {
			case CommutableAutoFrom.Constraints:
				if (WBidStateContent.CxWtState.CLAuto.Cx) {
						
						ftCommutableLine = WBidStateContent.Constraints.CLAuto;
					popUpCommuteCity.Title = ftCommutableLine.City;
					popUpConnectTime.Title = ftCommutableLine.ConnectTime==0?"--:--":ConvertMinuteToHHMM(ftCommutableLine.ConnectTime);
					popUpCheckInTime.Title = ConvertMinuteToHHMM (ftCommutableLine.CheckInTime);
					popUpBackTobase.Title = ConvertMinuteToHHMM (ftCommutableLine.BaseTime);
					isApplied = true;
						btnNonStop.State = ftCommutableLine.IsNonStopOnly==true?NSCellStateValue.On: NSCellStateValue.Off;

				}
                    if (WBidStateContent.CxWtState.CLAuto.Wt)
                    {
                        wtCommutableLineAuto = WBidStateContent.Weights.CLAuto;
                        popUpCommuteCity.Title = wtCommutableLineAuto.City;
                        popUpConnectTime.Title = wtCommutableLineAuto.ConnectTime == 0 ? "--:--" : ConvertMinuteToHHMM(wtCommutableLineAuto.ConnectTime);
						popUpCheckInTime.Title = ConvertMinuteToHHMM(wtCommutableLineAuto.CheckInTime);
                        popUpBackTobase.Title = ConvertMinuteToHHMM(wtCommutableLineAuto.BaseTime);
                        isApplied = true;
						btnNonStop.State = wtCommutableLineAuto.IsNonStopOnly == true ? NSCellStateValue.On : NSCellStateValue.Off;

					}
				break;
			case CommutableAutoFrom.Weight:

				if (WBidStateContent.CxWtState.CLAuto.Wt) {
					wtCommutableLineAuto= WBidStateContent.Weights.CLAuto;
					popUpCommuteCity.Title = wtCommutableLineAuto.City;
					popUpConnectTime.Title = wtCommutableLineAuto.ConnectTime == 0 ? "--:--" : ConvertMinuteToHHMM(wtCommutableLineAuto.ConnectTime);
						popUpCheckInTime.Title = ConvertMinuteToHHMM (wtCommutableLineAuto.CheckInTime);
					popUpBackTobase.Title = ConvertMinuteToHHMM (wtCommutableLineAuto.BaseTime);
					isApplied = true;
						btnNonStop.State = wtCommutableLineAuto.IsNonStopOnly == true ? NSCellStateValue.On : NSCellStateValue.Off;

					}
				break;
			case CommutableAutoFrom.Filters:
				
				if (WBidStateContent.BidAuto.BAFilter.FirstOrDefault(x=>x.Name=="CL") != null)
				{
					ftCommutableLine = (FtCommutableLine)WBidStateContent.BidAuto.BAFilter.FirstOrDefault(x=>x.Name=="CL").BidAutoObject;
					popUpCommuteCity.Title = ftCommutableLine.City;
					popUpConnectTime.Title = ftCommutableLine.ConnectTime == 0 ? "--:--" : ConvertMinuteToHHMM(ftCommutableLine.ConnectTime);
					popUpCheckInTime.Title = ConvertMinuteToHHMM (ftCommutableLine.CheckInTime);
					popUpBackTobase.Title = ConvertMinuteToHHMM (ftCommutableLine.BaseTime);
					isApplied = true;
						btnNonStop.State = ftCommutableLine.IsNonStopOnly == true ? NSCellStateValue.On : NSCellStateValue.Off;

					}
                    break;
                case CommutableAutoFrom.Sort:
                    if (!(WBidStateContent.CxWtState.Commute.Cx || WBidStateContent.CxWtState.Commute.Wt || WBidStateContent.SortDetails.BlokSort.Contains("33") || WBidStateContent.SortDetails.BlokSort.Contains("34") || WBidStateContent.SortDetails.BlokSort.Contains("35")))
                    {
                        ftCommutableLine = new FtCommutableLine { BaseTime = 10, ConnectTime = 30, CheckInTime = 60 };
                        WBidStateContent.Constraints.CLAuto = ftCommutableLine;

                    }

                    if (WBidStateContent.SortDetails.BlokSort.Contains("33") || WBidStateContent.SortDetails.BlokSort.Contains("34") || WBidStateContent.SortDetails.BlokSort.Contains("35"))
                    {

                        ftCommutableLine = WBidStateContent.Constraints.CLAuto;
                        if (ftCommutableLine.City != null)
                            popUpCommuteCity.Title = ftCommutableLine.City;
                        popUpConnectTime.Title = ftCommutableLine.ConnectTime == 0 ? "--:--" : ConvertMinuteToHHMM(ftCommutableLine.ConnectTime);
						popUpCheckInTime.Title = ConvertMinuteToHHMM(ftCommutableLine.CheckInTime);
                        popUpBackTobase.Title = ConvertMinuteToHHMM(ftCommutableLine.BaseTime);
                        isApplied = true;
						btnNonStop.State = ftCommutableLine.IsNonStopOnly == true ? NSCellStateValue.On : NSCellStateValue.Off;

					}
				break;
			}



			if(!isApplied) {

				popUpConnectTime.Title = "00:30";
				popUpCheckInTime.Title = "02:00";
				popUpBackTobase.Title = "00:10";
			}


			if (popUpCommuteCity.Title == "Not set") {

				btnDone.Enabled = false;
				btnViewArrival.Enabled = false;
			} 
			else {
				btnDone.Enabled = true;
				btnViewArrival.Enabled = true;
			}
			popUpConnectTime.Activated += PopUpConnectTime_Activated;
		popUpCommuteCity.Activated+= PopUpCommuteCity_Activated;
			popUpCheckInTime.Activated += PopUpCheckInTime_Activated; 
		}
        partial void btnNonStopClicked(NSButton sender)
        {
		
			
			bool IsNonStopOnly = (btnNonStop.State == NSCellStateValue.On);
			popUpConnectTime.Title = IsNonStopOnly ? "--:--" : "00:30";
			if (popUpCommuteCity.Title != "Not set")
			{
				CalculateDailyCommutableTimes();

			}
		}

        void PopUpCheckInTime_Activated(object sender, EventArgs e)
{
	int time = ConvertHHMMToMinute(popUpCheckInTime.Title);

	if (time < 120)
	{
		var alert = new NSAlert();
		alert.AlertStyle = NSAlertStyle.Informational;
		alert.Window.Title = "WBidMax";
		alert.MessageText = "You are setting the take-off pad to less than 2:00, you may not meet the contractual requirement to take a flight that arrives 1 hours before check-in";
		alert.InformativeText = "";
		alert.AddButton("OK");

		alert.RunModal();
	} 		}
		void PopUpCommuteCity_Activated (object sender, EventArgs e)
		{
			if (popUpCommuteCity.Title!="Not set")
			{
				CalculateDailyCommutableTimes();
			
			}

		}
		void PopUpConnectTime_Activated(object sender, EventArgs e)
		{
			bool IsNonStopOnly = (btnNonStop.State == NSCellStateValue.On);
			if (IsNonStopOnly && popUpCommuteCity.Title != "Not set")
			{
				btnNonStop.State = NSCellStateValue.Off;
				
			}
			
				CalculateDailyCommutableTimes();
			

		}

		private async void CalculateDailyCommutableTimes()
		{
			bool IsNonStopOnly = (btnNonStop.State == NSCellStateValue.On);
			if (popUpCommuteCity!=null && popUpCommuteCity.Title != "Not set")
			{
				
				ReadFlightRoutes();

				if (_flightRouteDetails != null)
				{
					int connectTime = popUpConnectTime.Title!= "--:--" ? ConvertHHMMToMinute(popUpConnectTime.Title):0;
					CalculateCommutableTimes(popUpCommuteCity.Title, IsNonStopOnly, connectTime);
					btnDone.Enabled = true;
					btnViewArrival.Enabled = true;
				}
				else
				{

					var alert = new NSAlert();
					alert.AlertStyle = NSAlertStyle.Informational;
					alert.Window.Title = "WBidMax";
					alert.MessageText = "Commutable Filter is NOT available  this time";
					alert.InformativeText = "Commutable line auto";
					alert.AddButton("OK");
					((NSButton)alert.Buttons[0]).Activated += (senderr, ee) =>
					{
						this.View.Window.Close();
						this.View.Window.OrderOut(this);
						NSApplication.SharedApplication.StopModal();

					};
					alert.RunModal();
					//Error alert view 
					//				System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() =>
					//					{
					//						SendNotificationMessage(WBidMessages.CommutableLineBAWindow_Notofication_ShowFilterNotAvilableMessageBox);
					//						SendNotificationMessage(WBidMessages.CommuteLineBAWindow_Notofication_CloseCommuteLineBAWindow);
					//					}));

				}
			}

		}

		public void ReadFlightRoutes()
		{

			_flightRouteDetails = null;

			string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
			string zipLocalFile = Path.Combine (WBidHelper.GetAppDataPath (), "FlightData.zip");
			string networkDataPath = WBidHelper.GetAppDataPath () + "/" + "FlightData.NDA";

			if (!File.Exists (networkDataPath)) {
				WebClient wcClient = new WebClient ();


				//Downloading networkdat file
				wcClient.DownloadFile (serverPath, zipLocalFile);

				string target = Path.Combine (WBidHelper.GetAppDataPath (), WBidHelper.GetAppDataPath () + "/");//+ Path.GetFileNameWithoutExtension(zipLocalFile))+ "/";



				// Open an existing zip file for reading
				ZipStorer zip = ZipStorer.Open (zipLocalFile, FileAccess.Read);

				// Read the central directory collection
				List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();

				// Look for the desired file
				foreach (ZipStorer.ZipFileEntry entry in dir) {
					zip.ExtractFile (entry, target + entry);
				}
				zip.Close ();

				if (File.Exists (zipLocalFile)) {
					File.Delete (zipLocalFile);
				}
			}
			//Reading NDA file
			//----------------------------------------------------------------------------------------
			if (File.Exists(networkDataPath))
			{

				//Deserializing data to FlightPlan object
				FlightPlan fp = new FlightPlan();
				using (FileStream networkDatatream = File.OpenRead(networkDataPath))
				{

					FlightPlan flightPlan = new FlightPlan();
					flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);



					_flightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
						(fr, f) =>
						new FlightRouteDetails
						{
							Flight = f.FlightId,
							FlightDate = fr.FlightDate,
							Orig = f.Orig,
							Dest = f.Dest,
							Cdep = f.Cdep,
							Carr = f.Carr,
							Ldep = f.Ldep,
							Larr = f.Larr,
							RouteNum = fr.RouteNum,

						}).ToList();
				}
			}
			//----------------------------------------------------------------------------------------
		}

		private void CalculateCommutableTimes(string commuteCity,bool IsNonStopOnly, int connectTime)
		{
			string domicile = GlobalSettings.CurrentBidDetails.Domicile;

//			if (WBidStateContent.Constraints.DailyCommuteTimes == null)
//			{
//				WBidStateContent.BidAuto = new BidAutomator();
//			}

			switch (Objtype) {
                case CommutableAutoFrom.Constraints:
                case CommutableAutoFrom.Weight:
                case CommutableAutoFrom.Sort:
                    if (WBidStateContent.Constraints.DailyCommuteTimesCmmutability == null)
                {
                        WBidStateContent.Constraints.DailyCommuteTimesCmmutability = new List<CommuteTime>();
                }
                    WBidStateContent.Constraints.DailyCommuteTimesCmmutability.Clear();
                    break;
                    
			//case CommutableAutoFrom.Constraints:
			//	if (WBidStateContent.Constraints.DailyCommuteTimes == null)
			//	{
			//		WBidStateContent.Constraints.DailyCommuteTimes = new List<CommuteTime>();
			//	}
			//	WBidStateContent.Constraints.DailyCommuteTimes.Clear();
			//	break;
			//case CommutableAutoFrom.Weight:

				//if (WBidStateContent.Weights.DailyCommuteTimes == null)
				//{
				//	WBidStateContent.Weights.DailyCommuteTimes = new List<CommuteTime>();
				//}
				//WBidStateContent.Weights.DailyCommuteTimes.Clear();
				//break;
			case CommutableAutoFrom.Filters:
				if (WBidStateContent.BidAuto.DailyCommuteTimes == null)
				{
					WBidStateContent.BidAuto.DailyCommuteTimes = new List<CommuteTime>();
				}
				WBidStateContent.BidAuto.DailyCommuteTimes.Clear();
				break;
			}



			DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Date;
			DateTime endDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Date;
			endDate = endDate.AddDays(3);
			int depTimeHhMm = 0301;  /* format is hhmm -- min time is 0301 (3:01AM) */
			int arrTimeHhMm = 2700; /* format is hhmm -- max time is 2700 (3:00AM)   */
			_depTime = depTimeHhMm / 100 * 60 + depTimeHhMm % 100;
			_arrTime = arrTimeHhMm / 100 * 60 + arrTimeHhMm % 100;
			int arrivalcount = 0;
			int depaturecount = 0;

			for (var date = startDate; date <= endDate; date = date.AddDays(1))
			{

				CommuteTime commuteTime = new CommuteTime();
				commuteTime.BidDay = date;
				commuteTime.EarliestArrivel = DateTime.MinValue;
				commuteTime.LatestDeparture = DateTime.MinValue;
				//Calculating earliest arrivel time
				//------------------------------------------------------------------------
				var nonConnectFlights = GetNonConnectFlights(commuteCity, domicile, date);

				//var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date);

				//var oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();

				var oneAndZeroConnectFlights = nonConnectFlights;
				if (IsNonStopOnly != true)
				{
					var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date, connectTime);
					oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
				}
				if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
				{
					double earliestArrivelTime = oneAndZeroConnectFlights.OrderBy(x => x.RtArr).FirstOrDefault().RtArr;
					commuteTime.EarliestArrivel = date.Date.AddMinutes(earliestArrivelTime);
				}
				else
				{
					arrivalcount++;
					commuteTime.EarliestArrivel = DateTime.MinValue;
					
				}
				//----------------------------------------------------------------------------

				//Calculating earliest arrivel time
				//------------------------------------------------------------------------
				nonConnectFlights = GetNonConnectFlights(domicile, commuteCity, date);

				 oneAndZeroConnectFlights = nonConnectFlights;
				if (IsNonStopOnly != true)
				{
					var oneConnectFlights = GetOneConnectFlights(domicile,commuteCity, date, connectTime);
					oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
				}



				if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
				{
					double latestDepartureTime = oneAndZeroConnectFlights.OrderByDescending(x => x.RtDep).FirstOrDefault().RtDep;
					commuteTime.LatestDeparture = date.Date.AddMinutes(latestDepartureTime);


				}
				else
				{
					depaturecount++;
					commuteTime.LatestDeparture = DateTime.MinValue;


				}

                //----------------------------------------------------------------------------

                switch (Objtype)
                {
                    case CommutableAutoFrom.Constraints:
                    case CommutableAutoFrom.Weight:
                    case CommutableAutoFrom.Sort:
                        WBidStateContent.Constraints.DailyCommuteTimesCmmutability.Add(commuteTime);
                        break;
                    //case CommutableAutoFrom.Constraints:
                    //	WBidStateContent.Constraints.DailyCommuteTimes.Add(commuteTime);
                    //	break;
                    //case CommutableAutoFrom.Weight:
                    //WBidStateContent.Weights.DailyCommuteTimes.Add(commuteTime);
                    //break;
                    case CommutableAutoFrom.Filters:
                        WBidStateContent.BidAuto.DailyCommuteTimes.Add(commuteTime);
                        break;
                }

			}


			int totaldays = (endDate - startDate).Days+1;
			if (totaldays == depaturecount && totaldays == arrivalcount)
			{
									var alert = new NSAlert ();
									alert.AlertStyle = NSAlertStyle.Informational;
									alert.Window.Title = "WBidMax";
									alert.MessageText = "There are NO possible connections between your commute city and your base";
									alert.InformativeText = "Commutable line auto";
									alert.AddButton ("OK");
									((NSButton)alert.Buttons [0]).Activated += (senderr, ee) => {
										this.View.Window.Close();
										this.View.Window.OrderOut(this);
										NSApplication.SharedApplication.StopModal ();
									};
									alert.RunModal ();

			}


		}

		public override void ViewDidDisappear ()
		{
			base.ViewDidDisappear ();
			switch (Objtype) {
			case CommutableAutoFrom.Constraints:
				
				break;
			case CommutableAutoFrom.Weight:
				break;
			case CommutableAutoFrom.Filters:
				break;
			}
		}
		private List<RouteDomain> GetNonConnectFlights(string depSta, string arrSta, DateTime dateTime)
		{

			List<RouteDomain> nonConnectFlights = null;

			nonConnectFlights = _flightRouteDetails
				.Where(x => x.Orig == depSta && x.Dest == arrSta && x.FlightDate == dateTime && x.Cdep >= _depTime && x.Carr <= _arrTime)

				.Select(y =>
					new RouteDomain
					{
						Date = y.FlightDate,
						Route = y.Orig + '-' + y.Dest,
						RtDep = y.Cdep,
						RtArr = y.Carr,
						RtTime = y.Carr - y.Cdep,
						Rt1 = y.RouteNum,
						Rt2 = 0,
						Rt3 = 0,
						Rt1Dep = y.Cdep,
						Rt2Dep = 0,
						Rt3Dep = 0,
						Rt1Arr = y.Carr,
						Rt2Arr = 0,
						Rt3Arr = 0,
						Con1 = 0,
						Con2 = 0,
						Rt1Orig = y.Orig,
						Rt2Orig = "",
						Rt3Orig = "",
						Rt1Dest = y.Dest,
						Rt2Dest = "",
						Rt3Dest = "",
						Rt1FltNum = y.Flight,
						Rt2FltNum = 0,
						Rt3FltNum = 0


					}).ToList()
				.OrderBy(z => z.Route).ThenBy(z1 => z1.RtTime).ToList();
			//.ThenBy(z1 => z1.RtTime).ToList();
			return nonConnectFlights;
		}

		private List<RouteDomain> GetOneConnectFlights(string depSta, string arrSta, DateTime dateTime, int connectTime)
		{
			//int connectTime = 30;
			List<RouteDomain> oneConnectFlights = null;

			oneConnectFlights = _flightRouteDetails.Where(frd1 => frd1.Orig == depSta && frd1.FlightDate == dateTime).Join(_flightRouteDetails.Where(frd2 => frd2.Dest == arrSta && frd2.FlightDate == dateTime), f1 => f1.Dest, f2 => f2.Orig,
				(f1, f2) => new { ff1 = f1, ff2 = f2 }).ToList()
				.Where(x =>
					x.ff1.Dest != arrSta
					&& x.ff1.Cdep >= _depTime && x.ff2.Carr <= _arrTime
					&& (x.ff1.Carr + connectTime <= x.ff2.Cdep || x.ff1.RouteNum == x.ff2.RouteNum) && x.ff2.Cdep > x.ff1.Cdep
				)
				.Select(y =>
					new RouteDomain
					{
						Date = y.ff1.FlightDate,
						Route = y.ff1.Orig + '-' + y.ff1.Dest + '-' + y.ff2.Dest,
						RtDep = y.ff1.Cdep,
						RtArr = y.ff2.Carr,
						RtTime = y.ff2.Carr - y.ff1.Cdep,
						Rt1 = y.ff1.RouteNum,
						Rt2 = y.ff2.RouteNum,
						Rt3 = 0,
						Rt1Dep = y.ff1.Cdep,
						Rt2Dep = y.ff2.Cdep,
						Rt3Dep = 0,
						Rt1Arr = y.ff1.Carr,
						Rt2Arr = y.ff2.Carr,
						Rt3Arr = 0,
						Con1 = y.ff2.Cdep - y.ff1.Carr,
						Con2 = 0,
						Rt1Orig = y.ff1.Orig,
						Rt2Orig = y.ff2.Orig,
						Rt3Orig = "",
						Rt1Dest = y.ff1.Dest,
						Rt2Dest = y.ff2.Dest,
						Rt3Dest = "",
						Rt1FltNum = y.ff1.Flight,
						Rt2FltNum = y.ff2.Flight,
						Rt3FltNum = 0

					}).ToList()
				.OrderBy(z => z.Route).ThenBy(z1 => z1.RtTime).ToList();
			return oneConnectFlights;
		}
		partial void funViewArrivalAnddepartTimeAction (NSObject sender)
		{
			
						ArrivalAndDepartViewController ObjArrival= new ArrivalAndDepartViewController();
			
			ObjArrival.ObjArrivaltype=(CommutableAutoFrom)Objtype;
			ObjArrival.City=popUpCommuteCity.Title;
			ObjArrival.IsNonStop = (btnNonStop.State == NSCellStateValue.On); 
			this.PresentViewControllerAsSheet(ObjArrival);

		}


	




		#endregion


		private void GenerateCommuteCities()
		{
			_lstCommuteCities = new ObservableCollection<City> ();
			foreach (var city in GlobalSettings.WBidINIContent.Cities) {
				_lstCommuteCities.Add (new City{ Id = city.Id, Name = city.Name });
			}
			_lstCommuteCities.Insert (0, new City{ Id = 0, Name = "Not set" });

			_arrCities = _lstCommuteCities.Select (x => x.Name).ToArray ();

			popUpCommuteCity.RemoveAllItems() ;
			popUpCommuteCity.AddItems( _arrCities);
			//foreach( var city in GlobalSettings.
		}


		private void GenerateConnectTime()
		{
			List<string> lstConnectTime = new List<string> ();
			lstConnectTime.Add("--:--");
			for (int cnt = 5; cnt < 60; cnt = cnt + 5) {
				lstConnectTime.Add("00:"+cnt.ToString().PadLeft(2,'0'));
			}
			lstConnectTime.Add ("01:00");
			_arrConnectTime = lstConnectTime.ToArray ();

			popUpConnectTime.RemoveAllItems() ;
			popUpConnectTime.AddItems( _arrConnectTime);
		}


		private void GenerateCheckInTime()
		{
			List<string> lstCheckTime = new List<string> ();
			for (int cnt = 5; cnt <= 180; cnt = cnt + 5) {
				lstCheckTime.Add(ConvertMinuteToHHMM(cnt));
			}

			_arrCheckInTime = lstCheckTime.ToArray ();

			popUpCheckInTime.RemoveAllItems() ;
			popUpCheckInTime.AddItems( _arrCheckInTime);
			
		}

		private void GenerateBackToBaseTime()
		{
			List<string> lstCheckTime = new List<string> ();
			for (int cnt = 5; cnt < 60; cnt = cnt + 5) {
				lstCheckTime.Add("00:" + cnt.ToString().PadLeft(2, '0'));
			}
			lstCheckTime.Add ("01:00");

			_arrBacktoBaseTime = lstCheckTime.ToArray ();

			popUpBackTobase.RemoveAllItems() ;
			popUpBackTobase.AddItems( _arrBacktoBaseTime);

		}

		private string ConvertMinuteToHHMM(int minute)
		{
			string result = string.Empty;
			result = Convert.ToString(minute / 60).PadLeft(2, '0');
			result += ":";
			result += Convert.ToString(minute % 60).PadLeft(2, '0');
			return result;


		}

		private int ConvertHHMMToMinute(string hhmm)
		{

			int result = 0;

			if (hhmm == string.Empty || hhmm == null) return result;

			string[] split = hhmm.Split(':');
			result = int.Parse(split[0]) * 60 + int.Parse(split[1]);
			return result;

		}

		//strongly typed view accessor
		public new CommutableView View {
			get {
				return (CommutableView)base.View;
			}
		}
	}
}
