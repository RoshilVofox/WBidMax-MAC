using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.Core
{
    public class GlobalSettings
    {

		public static string IniFileVersion ="1.7";
        public const string LineVersion = "1.7";
        public const string TripVersion = "2.0";
        public const string DwcVersion = "2.9";
        public const string StateFileVersion = "2.5";
        public const int requiredRest = 600;
        public static string IsCoverletterShowFileName=string.Empty;
		public static string MILFileVersion ="1.0";

		public static bool IsNeedToDownloadSeniority = false;

		public static bool IsNeedToDownloadSeniorityUser=false;

        public static bool IsNewsShow= false;

        public static string ServerUrl = "108.60.201.50";
        //To test the mock data we need to donwlaod the data from this location
        public static string MockUrl = "http://www.wbidMax.com/MockData/";

        public static string WBidDownloadFileUrl = "http://www.wbidmax.com/downloads/swa/";

		public static string DataDownloadAuthenticationUrl="http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc/";

        public const int ReserveAmPmClassification = 510;  // 510 in minutes is 08:30

		public static string installerDownloadUrl = "http://www.wbidmax.com/downloads/";

        public static WBidINI WBidINIContent { get; set; }
        /// <summary>
        /// Store Current Bid Information
        /// </summary>
        public static BidDetails DownloadBidDetails { get; set; }
        /// <summary>
        /// Gets or sets the columndefenition.
        /// </summary>
        /// <value>The columndefenition.</value>
        public static List<ColumnDefinition> columndefinition { get; set; }

        public static List<AdditionalColumns> AdditionalColumns { get; set; }
		public static List<AdditionalColumns> AdditionalvacationColumns { get; set; }

        public static List<AdditionalColumns> BidlineAdditionalColumns { get; set; }

		public static List<AdditionalColumns> ModernAdditionalColumns { get; set; }

		public static List<AdditionalColumns> BidlineAdditionalvacationColumns { get; set; }

		public static List<AdditionalColumns> ModernAdditionalvacationColumns { get; set; }


        /// <summary>
        /// Store Current Bid Information
        /// </summary>
        public static BidDetails CurrentBidDetails { get; set; }


        public static WBidStateCollection WBidStateCollection { get; set; }

		public static string CompanyVA { get; set; }
        //private static WBidStateCollection _wBidStateCollection;
        //public static WBidStateCollection WBidStateCollection
        //{
        //    get { return _wBidStateCollection; }
        //    set
        //    {
        //        _wBidStateCollection = value;
               
        //    }
        //}


        /// <summary>
        /// second Sunday in March.  Used to calculate herb time for FA takeoff and land times from raw data
        /// </summary>
        public static DateTime FirstDayOfDST { get; set; }

        /// <summary>
        /// first Sunday in November.  Used to calculate herb time for FA takeoff and land times from raw data
        /// </summary>
        public static DateTime LastDayOfDST { get; set; }

	
        public const int show1stDay = 60;
        public const int showAfter1stDay = 30;
        public const int debrief = 30;
        public const int ReserveBriefTime=0;
        public const int ReserveDebrief = 0;

        public const decimal dutyHrRig = 60m / .74m;
        public const decimal adgRig = 6.5m;
        public const decimal tafbRig = 180m;
        public const decimal dailyMinRig = 5.0m;
        public const decimal fltMinRig = 1.0m;
        public const decimal lineGuar31day = 89.0m;
        public const decimal lineGuar30day = 87.0m;
        public const decimal lineGuar2829Day = 85.0m;

        public const decimal PltDpMinFactor = 5.0m;
        public const decimal PltDhrFactor = 0.74m;
        public const decimal PltAdgFactor = 6.5m;
        public const decimal PltTafbFactor = 0.333m;
        public const decimal FaDpMinFactor = 4.0m;
        public const decimal FaDhrFactor = 0.74m;
        public const decimal FaAdgFactor = 6.5m;
        public const decimal FaTafbFactor = 0.333m;

        public const int FAshow1stDutyPeriod = 60;
        public const int FAshow = 30;
        public const int FArelease = 30;
        public const int FAreleaseLastDutyPeriod = 30;
        public const decimal FAReserveDayPay = 6.0m;

        public const decimal ReserveDailyGuarantee = 6.0m;

        private static ObservableCollection<Trip> _trip;
        public static ObservableCollection<Trip> Trip
        {
            get
            {
                return _trip ?? (_trip = new ObservableCollection<Trip>());
            }
            set
            {
                _trip = value;
            }
        }
        private static ObservableCollection<Line> _lines;
        public static ObservableCollection<Line> Lines
        {
            get
            {
                return _lines ?? (_lines = new ObservableCollection<Line>());
            }
            set
            {
                _lines = value;
            }
        }
		public static ObservableCollection<Line> tempLinesForReparse { get; set; }

       

        public static List<LineData> LineData { get; set; }

        /// <summary>
        /// All the overnight cities in this bid data.
        /// </summary>
        public static List<City> OverNightCitiesInBid { get; set; }

        /// <summary>
        /// All the cities in this bid month.
        /// </summary>
        public static List<string> AllCitiesInBid { get; set; }

        public static bool buddyBidTest = false;

        public static WbidUser WbidUserContent { get; set; }

        public static List<string> DownloadFiles { get; set; }

        /// <summary>
        /// Store the employee number temporarily while submiting the bid or Bid editor
        /// </summary>
        public static string TemporaryEmployeeNumber { get; set; }

        /// <summary>
        /// IsVacationCorrection
        /// </summary>
        public static bool IsVacationCorrection { get; set; }




        public static BidPrep BidPrepDetails { get; set; }

        public static bool ClearBuddyBid { get; set; }

        public static SubmitBid SubmitBid { get; set; }

        public static bool IsDifferentUser { get; set; }

        public static string ModifiedEmployeeNumber { get; set; }

        public static SeniorityListMember SeniorityListMember { get; set; }


      

        public static List<Absense> OrderedVacationDays { get; set; }


        public static List<Absense> TempOrderedVacationDays { get; set; }

        public static Dictionary<string, Trip> parsedDict;

        public static bool IsScrapStart { get; set; }

        public static decimal DailyVacPay = 3.75m;

        public static List<FlightRouteDetails> FlightRouteDetails { get; set; }

        public static MenuBarButtonStatus MenuBarButtonStatus { get; set; }

        public static Dictionary<string,TripMultiVacData> VacationData { get; set; }






        public const int connectTime = 40;

        public const int EarliestTakeOffMinutes = 180;
        public const int DutyDayMinutes = 720;
        public const int RcConnect = 30;

        public const decimal HotelCostCP = 0.4m;
        public const decimal HotelCostFO = 0.6m;
        public const decimal VDvsRCtfpFactor = 1.0m;
        //public const int LastLandingMinus1440 = 275;        // 4:35 am herb, earliest takeoff in database is 280 which is 4:40 am herb
        public const int LastLandingMinus1440 = 179;        // 2:59 is the end of SWA flight day

        // show between 0200 - 0359
        public const int DutyDayMinutes0200_0359 = 600;     // 10 hours
        public const int DutyDayMinutes0400_0559 = 720;     // 12 hours
        public const int DutyDayMinutes0600_1059 = 780;     // 13 hours
        public const int DutyDayMinutes1100_1459 = 720;     // 12 hours
        public const int DutyDayMinutes1500_1959 = 660;     // 11 hours
        public const int DutyDayMinutes2000_2559 = 540;     // 9 hours




        public static string SouthWestConnectionAlert = "You cannot get needed access via SouthwestWifi or 2Wire.  \n\nTry again later when you are safely on the ground and have another internet access.";
        //public static string VPSDownAlert = "We are not able to connect to the WBidMax server.  We need to do so to download important files.  Please try again later.  If this persists, you can try to change internet connections (like use a hotspot).  Finally, if this persists for more than 4 hours, let us know.";
        public static string VPSDownAlert = "We are not able to connect to the WBidMax server.Please check your Internet connection and  try again later.  \n\nIf this persists, you can try to change internet connections (like use a hotspot). \n\nFinally, if this persists for more than 4 hours, let us know.";

        public const int Far117buffer = 90;



        //public const int LastLandingMinutes = 1620;         // 3:00 am
        //Frank Change 2/8/2022 to reflect end of flight day at 2:59 Am
        public const int LastLandingMinutes = 1619;         // 3:00 am



        private static List<CityPair> _TtpCityPairs;
        public static List<CityPair> TtpCityPairs
        {
            get
            {
                return _TtpCityPairs ?? (_TtpCityPairs = new List<CityPair>());
            }
            set
            {
                _TtpCityPairs = value;
            }
        }




        public static DateTime FAEOMStartDate { get; set; }


        /// <summary>
        /// This property set when user selects the Overlap correction  while downloading the datas.
        /// </summary>
        public static bool IsOverlapCorrection { get; set; }

        /// <summary>
        /// Store Overlap correction dayas and flight time from the overlap corrrection Dialogue
        /// </summary>
        public static List<Day> LeadOutDays { get; set; }


        /// <summary>
        /// Store the last leg arrival time of the previous bid period to test rest  legality condition(atleat 9 hours between lead out days and lead in days )
        /// </summary>
        public static int LastLegArrivalTime { get; set; }

        /// <summary>
        /// Selected line values.
        /// </summary>
        public static Line SelectedLine { get; set; }

        public static WBidIntialState wbidintialState { get; set; }

        //public static string FdpLegColumn(int legs)
        //{
        //    switch (legs)
        //    {
        //        case 0:
        //        case 1: return "_1leg";
        //        case 2: return "_2leg";
        //        case 3: return "_3leg";
        //        case 4: return "_4leg";
        //        case 5: return "_5leg";
        //        case 6: return "_6leg";
        //        case 7: return "_7leg";
        //        default: return "_7leg";
        //    }
        //}

		public static bool isModified { get; set; }

		public static bool SynchEnable { get; set; }

		public static string synchServiceUrl ="http://108.60.201.50:8006/SynchronizationService.svc/";

		// public static Stack<WBidState> UndoStack { get; set; }
		public static List<WBidState> UndoStack { get; set; }

		public static List<WBidState> RedoStack { get; set; }

		public static bool isUndo { get; set; }
		public static bool isRedo { get; set; }

		public static MaintananceData NewVersionData { get; set; }

		public static QuickSets QuickSets { get; set; }

		public static Dictionary<string, TripMultiMILData> MILData { get; set; }

		public static List<Absense> MILDates { get; set; }

		public static SplitPointCities SplitPointCities { get; set; }

		public static bool isHistorical { get; set; }

        public static List<Absense> FVVacation{ get; set; }

        public static bool IsFVVacation{ get; set; }

        public static bool isVFXServer { get; set; }

        public static bool IsNeedToEnableVacDiffButton { get; set; }
    }
}
