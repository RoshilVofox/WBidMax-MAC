using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using System.Net;
using System.IO.Compression;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.Core.Enum;
using System.Collections.ObjectModel;

namespace WBid.WBidMac.Mac
{
    public partial class CommutabilityViewController : AppKit.NSViewController
    {
        
        string [] _arrCities;
        string [] _arrConnectTime;
        string [] _arrCheckInTime;
        string [] _arrBacktoBaseTime;
        ObservableCollection<City> _lstCommuteCities;
		private List<FlightRouteDetails> _flightRouteDetails;
		private int _depTime;
        private int _arrTime;
		WBidState WBidStateContent;
        public CommutabilityEnum Objtype;
		
		//public	WBid.WBidMac.Mac.ArrivalAndDepartViewController.CommutableAutoFrom Objtype;
		#region Constructors

		// Called when created from unmanaged code
		public CommutabilityViewController (IntPtr handle) : base (handle)
        {
            Initialize ();
        }

        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public CommutabilityViewController (NSCoder coder) : base (coder)
        {
            Initialize ();
        }

        // Call to load from the XIB/NIB file
        public CommutabilityViewController () : base ("CommutabilityView", NSBundle.MainBundle)
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
            GenerateCommuteCities ();
            GenerateConnectTime ();

            GenerateCheckInTime ();

            GenerateBackToBaseTime ();




            WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            bool isApplied = false;
            Commutability ftCommutableLine;
            Commutability wtCommutableLineAuto;
            switch (Objtype) {
                case CommutabilityEnum.CommutabilityConstraint:

					//if (!WBidStateContent.CxWtState.Commute.Wt)
                    if (!(WBidStateContent.CxWtState.Commute.Cx || WBidStateContent.CxWtState.Commute.Wt || WBidStateContent.SortDetails.BlokSort.Contains ("30") || WBidStateContent.SortDetails.BlokSort.Contains ("31") || WBidStateContent.SortDetails.BlokSort.Contains ("32"))) 
					{
						ftCommutableLine = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
						WBidStateContent.Weights.Commute = ftCommutableLine;

					}

                if (WBidStateContent.CxWtState.Commute.Cx) {

                    ftCommutableLine = WBidStateContent.Constraints.Commute;
                    popUpCommuteCity.Title = ftCommutableLine.City;
                    popUpConnectTime.Title = ConvertMinuteToHHMM (ftCommutableLine.ConnectTime);
                    popUpCheckInTime.Title = ConvertMinuteToHHMM (ftCommutableLine.CheckInTime);
                    popUpBackTobase.Title = ConvertMinuteToHHMM (ftCommutableLine.BaseTime);
                    isApplied = true;

                }
                break;
                case CommutabilityEnum.CommutabilityWeight:

					//if (!WBidStateContent.CxWtState.Commute.Cx)
                    if (!(WBidStateContent.CxWtState.Commute.Cx || WBidStateContent.CxWtState.Commute.Wt || WBidStateContent.SortDetails.BlokSort.Contains ("30") || WBidStateContent.SortDetails.BlokSort.Contains ("31") || WBidStateContent.SortDetails.BlokSort.Contains ("32"))) 
					{
						ftCommutableLine = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
						WBidStateContent.Constraints.Commute = ftCommutableLine;

					}

					if (WBidStateContent.CxWtState.Commute.Wt) {
                    wtCommutableLineAuto = WBidStateContent.Constraints.Commute;
                    popUpCommuteCity.Title = wtCommutableLineAuto.City;
                    popUpConnectTime.Title = ConvertMinuteToHHMM (wtCommutableLineAuto.ConnectTime);
                    popUpCheckInTime.Title = ConvertMinuteToHHMM (wtCommutableLineAuto.CheckInTime);
                    popUpBackTobase.Title = ConvertMinuteToHHMM (wtCommutableLineAuto.BaseTime);
                    isApplied = true;

                }
                break;
                case CommutabilityEnum.CommutabilitySort:
                if (!(WBidStateContent.CxWtState.Commute.Cx || WBidStateContent.CxWtState.Commute.Wt ||  WBidStateContent.SortDetails.BlokSort.Contains("30")|| WBidStateContent.SortDetails.BlokSort.Contains ("31") || WBidStateContent.SortDetails.BlokSort.Contains ("32")))
                {
						ftCommutableLine = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
                    WBidStateContent.Constraints.Commute = ftCommutableLine;

                }

                if (WBidStateContent.SortDetails.BlokSort.Contains ("33") || WBidStateContent.SortDetails.BlokSort.Contains ("34") || WBidStateContent.SortDetails.BlokSort.Contains ("35")) {

                    ftCommutableLine = WBidStateContent.Constraints.Commute;
                    popUpCommuteCity.Title = ftCommutableLine.City;
                    popUpConnectTime.Title = ConvertMinuteToHHMM (ftCommutableLine.ConnectTime);
                    popUpCheckInTime.Title = ConvertMinuteToHHMM (ftCommutableLine.CheckInTime);
                    popUpBackTobase.Title = ConvertMinuteToHHMM (ftCommutableLine.BaseTime);
                    isApplied = true;

                }
                break;
            
            }



            if (!isApplied) {

                popUpConnectTime.Title = "00:30";
                popUpCheckInTime.Title = "02:00";
                popUpBackTobase.Title = "00:10";
            }

            if (popUpCommuteCity.Title == "Not set") {

                btnDone.Enabled = false;
                btnViewArrival.Enabled = false;
            } else {
                btnDone.Enabled = true;
                btnViewArrival.Enabled = true;
            }
			popUpCheckInTime.Activated += PopUpCheckInTime_Activated; 
            popUpCommuteCity.Activated+=PopUpCommuteCity_Activated1;


        }


        void PopUpCommuteCity_Activated1 (object sender, EventArgs e)
        {
            if (popUpCommuteCity.Title != "Not set") {
                CalculateDailyCommutableTimes ();

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
	}
		}
        partial void funCancelAction (NSObject sender)
        {

            //this.View.Window.Close ();
            //this.View.Window.OrderOut (this);
			this.DismissViewController(this);

        }
        private void GenerateCommuteCities ()
        {
            _lstCommuteCities = new ObservableCollection<City> ();
            foreach (var city in GlobalSettings.WBidINIContent.Cities) {
                _lstCommuteCities.Add (new City { Id = city.Id, Name = city.Name });
            }
            _lstCommuteCities.Insert (0, new City { Id = 0, Name = "Not set" });

            _arrCities = _lstCommuteCities.Select (x => x.Name).ToArray ();

            popUpCommuteCity.RemoveAllItems ();
            popUpCommuteCity.AddItems (_arrCities);
            //foreach( var city in GlobalSettings.
        }

        private void GenerateConnectTime ()
        {
            List<string> lstConnectTime = new List<string> ();
            for (int cnt = 5; cnt < 60; cnt = cnt + 5) {
                lstConnectTime.Add ("00:" + cnt.ToString ().PadLeft (2, '0'));
            }
            lstConnectTime.Add ("01:00");
            _arrConnectTime = lstConnectTime.ToArray ();

            popUpConnectTime.RemoveAllItems ();
            popUpConnectTime.AddItems (_arrConnectTime);
        }


        private void GenerateCheckInTime ()
        {
            List<string> lstCheckTime = new List<string> ();
            for (int cnt = 5; cnt <= 180; cnt = cnt + 5) {
                lstCheckTime.Add (ConvertMinuteToHHMM (cnt));
            }

            _arrCheckInTime = lstCheckTime.ToArray ();

            popUpCheckInTime.RemoveAllItems ();
            popUpCheckInTime.AddItems (_arrCheckInTime);

        }

        private void GenerateBackToBaseTime ()
        {
            List<string> lstCheckTime = new List<string> ();
            for (int cnt = 5; cnt < 60; cnt = cnt + 5) {
                lstCheckTime.Add ("00:" + cnt.ToString ().PadLeft (2, '0'));
            }
            lstCheckTime.Add ("01:00");

            _arrBacktoBaseTime = lstCheckTime.ToArray ();

            popUpBackTobase.RemoveAllItems ();
            popUpBackTobase.AddItems (_arrBacktoBaseTime);

        }
        private string ConvertMinuteToHHMM (int minute)
        {
            string result = string.Empty;
            result = Convert.ToString (minute / 60).PadLeft (2, '0');
            result += ":";
            result += Convert.ToString (minute % 60).PadLeft (2, '0');
            return result;


        }

        private int ConvertHHMMToMinute (string hhmm)
        {

            int result = 0;

            if (hhmm == string.Empty || hhmm == null) return result;

            string [] split = hhmm.Split (':');
            result = int.Parse (split [0]) * 60 + int.Parse (split [1]);
            return result;

        }
        #endregion

       
		partial void funDoneAction (NSObject sender)
		{
		 WBidStateContent =GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		 Commutability ftCommutableLine;

			if ((int)CommutabilityEnum.CommutabilityConstraint == (int)Objtype)
			{

				if (WBidStateContent.CxWtState.Commute.Cx)
				{
					ftCommutableLine = WBidStateContent.Constraints.Commute;
				}
				else
				{
					ftCommutableLine = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
					WBidStateContent.CxWtState.Commute.Cx = true;
				}

				if (popUpCommuteCity.Title != "Not set")
				{
					ftCommutableLine.ConnectTime = ConvertHHMMToMinute(popUpConnectTime.Title);
					ftCommutableLine.City = popUpCommuteCity.Title;
					//ftCommutableLine.CommuteCity=_lstCommuteCities.FirstOrDefault(x=>x.Name==popUpCommuteCity.Title).Id;
					ftCommutableLine.CheckInTime = ConvertHHMMToMinute(popUpCheckInTime.Title);
					ftCommutableLine.BaseTime = ConvertHHMMToMinute(popUpBackTobase.Title);
					
				}
				WBidStateContent.CxWtState.Commute.Cx = true;
				WBidStateContent.Constraints.Commute = ftCommutableLine;

				//Common

				WBidStateContent.Weights.Commute.ConnectTime = ftCommutableLine.ConnectTime;
				WBidStateContent.Weights.Commute.CheckInTime = ftCommutableLine.CheckInTime;

				WBidStateContent.Weights.Commute.BaseTime = ftCommutableLine.BaseTime;


				CalculateLineProperties lineproprty = new CalculateLineProperties();
				lineproprty.CalculateCommuteLineProperties(WBidStateContent);

                Recalculatevalues();
				NSNotificationCenter.DefaultCenter.PostNotificationName("CmtbltyNotification", null);

			}
			else if ((int)CommutabilityEnum.CommutabilityWeight == (int)Objtype)
			{

				if (WBidStateContent.CxWtState.Commute.Wt)
				{
					ftCommutableLine = WBidStateContent.Weights.Commute;
					ftCommutableLine.City = WBidStateContent.Constraints.Commute.City;
					WBidStateContent.Constraints.Commute.City = popUpCommuteCity.Title;



				}
				else
				{
					ftCommutableLine = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };

				}

				if (popUpCommuteCity.Title != "Not set")
				{
					ftCommutableLine.ConnectTime = ConvertHHMMToMinute(popUpConnectTime.Title);
					ftCommutableLine.City = popUpCommuteCity.Title;
					//ftCommutableLine.CommuteCity=_lstCommuteCities.FirstOrDefault(x=>x.Name==popUpCommuteCity.Title).Id;
					ftCommutableLine.CheckInTime = ConvertHHMMToMinute(popUpCheckInTime.Title);
					ftCommutableLine.BaseTime = ConvertHHMMToMinute(popUpBackTobase.Title);
				}

				WBidStateContent.CxWtState.Commute.Wt = true;
				WBidStateContent.Weights.Commute = ftCommutableLine;

				//Common

				WBidStateContent.Constraints.Commute.ConnectTime = ftCommutableLine.ConnectTime;
				WBidStateContent.Constraints.Commute.CheckInTime = ftCommutableLine.CheckInTime;
				WBidStateContent.Constraints.Commute.City = popUpCommuteCity.Title;
				WBidStateContent.Constraints.Commute.BaseTime = ftCommutableLine.BaseTime;

				CalculateLineProperties line = new CalculateLineProperties();
				line.CalculateCommuteLineProperties(WBidStateContent);
                Recalculatevalues();
				NSNotificationCenter.DefaultCenter.PostNotificationName("CmtbltyWeightNotification", null);
			}

			else if ((int)CommutabilityEnum.CommutabilitySort == (int)Objtype)
			{
				if (WBidStateContent.SortDetails.BlokSort.Contains("33")|| WBidStateContent.SortDetails.BlokSort.Contains("34")|| WBidStateContent.SortDetails.BlokSort.Contains("35"))
				{
					ftCommutableLine =WBidStateContent.Constraints.Commute;
					ftCommutableLine.City = WBidStateContent.Constraints.Commute.City;
					WBidStateContent.Constraints.Commute.City = popUpCommuteCity.Title;
				}
				else
				{
					ftCommutableLine = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };

				}

				if (popUpCommuteCity.Title != "Not set")
				{
					ftCommutableLine.ConnectTime = ConvertHHMMToMinute(popUpConnectTime.Title);
					ftCommutableLine.City = popUpCommuteCity.Title;
					//ftCommutableLine.CommuteCity=_lstCommuteCities.FirstOrDefault(x=>x.Name==popUpCommuteCity.Title).Id;
					ftCommutableLine.CheckInTime = ConvertHHMMToMinute(popUpCheckInTime.Title);
					ftCommutableLine.BaseTime = ConvertHHMMToMinute(popUpBackTobase.Title);
				}


				//Common

				WBidStateContent.Constraints.Commute.ConnectTime = ftCommutableLine.ConnectTime;
				WBidStateContent.Constraints.Commute.CheckInTime = ftCommutableLine.CheckInTime;
				WBidStateContent.Constraints.Commute.City = popUpCommuteCity.Title;
				WBidStateContent.Constraints.Commute.BaseTime = ftCommutableLine.BaseTime;

				CalculateLineProperties lineproprty = new CalculateLineProperties();
				lineproprty.CalculateCommuteLineProperties(WBidStateContent);

				//we dont need to appy block sort logic becuase it will calulates in the notifications
				if (WBidStateContent.CxWtState.Commute.Cx)
					CommonClass.ConstraintsController.ApplyAndReloadConstraints("Commutability");
				if (WBidStateContent.CxWtState.Commute.Wt)
					CommonClass.WeightsController.ApplyAndReloadWeights("Commutability");
				NSNotificationCenter.DefaultCenter.PostNotificationName("CmtbltySortNotification", null);

			}


			//this.View.Window.Close();
			//this.View.Window.OrderOut(this);
			this.DismissViewController(this);




			//CLONotification
		}
		/// <summary>
		/// Recalculate the constrain,weight and sort values.
		/// </summary>
		void Recalculatevalues()
		{
			if(WBidStateContent.CxWtState.Commute.Cx)
CommonClass.ConstraintsController.ApplyAndReloadConstraintsFromCommutability("Commutability");
			if (WBidStateContent.CxWtState.Commute.Wt)
CommonClass.WeightsController.ApplyAndReloadWeightsFromCommutability("Commutability");
			if (WBidStateContent.SortDetails.BlokSort.Contains("30")|| WBidStateContent.SortDetails.BlokSort.Contains("31") || WBidStateContent.SortDetails.BlokSort.Contains("32"))
			CommonClass.SortController.ApplyBlockSortFromCommutability();


				CommonClass.MainController.ReloadAllContent();
		}
		
		void PopUpCommuteCity_Activated (object sender, EventArgs e)
		{
			if (popUpCommuteCity.Title!="Not set")
			{
				CalculateDailyCommutableTimes();

			}
			
		}
        partial void btnNonStopClicked(NSButton sender)
        {
			if (popUpCommuteCity.Title != "Not set")
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
				CalculateDailyCommutableTimes();
			}
			if (popUpCommuteCity.Title != "Not set")
			{

				CalculateDailyCommutableTimes();

			}
			else if (popUpCommuteCity.Title != "Not set")
			{
				CalculateDailyCommutableTimes();
			}
			else if (IsNonStopOnly)
			{
				CalculateDailyCommutableTimes();
			}

		}
		private async void CalculateDailyCommutableTimes()
		{

			bool IsNonStopOnly = (btnNonStop.State == NSCellStateValue.On);
			ReadFlightRoutes();

			if (_flightRouteDetails != null)
			{
				int connectTime = ConvertHHMMToMinute(popUpConnectTime.Title);
				CalculateCommutableTimes(popUpCommuteCity.Title, IsNonStopOnly,0);
				btnDone.Enabled = true;
				btnViewArrival.Enabled = true;
			}
			else
			{

				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Commutable Filter is NOT available  this time";
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

		private void CalculateCommutableTimes(string commuteCity,bool IsNonStopOnly,int connectTime)
		{
			string domicile = GlobalSettings.CurrentBidDetails.Domicile;


			WBidStateContent.Constraints.DailyCommuteTimesCmmutability = new List<CommuteTime>();
				
		
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
					var oneConnectFlights = GetOneConnectFlights(domicile, commuteCity, date, connectTime);
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


				WBidStateContent.Constraints.DailyCommuteTimesCmmutability.Add(commuteTime);


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
			//ObjArrival.ObjArrivaltype=(ArrivalAndDepartViewController.CommutableAutoFrom)Objtype;
			ObjArrival.ObjArrivaltype=CommutableAutoFrom.CommutabilityConstraint;
			ObjArrival.City=popUpCommuteCity.Title;
			this.PresentViewControllerAsSheet(ObjArrival);

		}

        //strongly typed view accessor
        public new CommutabilityView View {
            get {
                return (CommutabilityView)base.View;
            }
        }
    }
}
