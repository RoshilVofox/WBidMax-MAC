#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization; 
#endregion

namespace WBid.WBidiPad.Model
{
    public class WBidINI
    {
        #region Properties
        public WBidINI()
        {
        }

        public string Version { get; set; }


        /// <summary>
        /// PURPOSE : Updates
        /// </summary>
        public INIUpdates Updates { get; set; }


        /// <summary>
        /// PURPOSE :Domicile List
        /// </summary>
        public List<Domicile> Domiciles { get; set; }

        /// <summary>
        /// PURPOSE : ColumnDefinitions
        /// </summary>
        public List<DataColumn> DataColumns { get; set; }

        /// <summary>
        /// PURPOSE : Am PM Define
        /// </summary>
        public AmPmConfigure AmPmConfigure { get; set; }


        /// <summary>
        /// PURPOSE : Equipments
        /// </summary>
        public List<Equipment> Equipments { get; set; }

        /// <summary>
        /// PURPOSE : EquipmentTypes
        /// </summary>
        public List<EquipType> EquipTypes { get; set; }

        /// <summary>
        /// PURPOSE : Cities
        /// </summary>
        public List<City> Cities { get; set; }


        /// <summary>
        /// PURPOSE : Hotels
        /// </summary>
        public Hotels Hotels { get; set; }


        public string Effective { get; set; }


        /// <summary>
        /// OUROISE:Holds the Week tab configuration details
        /// </summary>
        public Week Week { get; set; }

        /// <summary>
        /// PURPOSE: Holds the Avoidance bids
        /// </summary>
        public AvoidanceBids AvoidanceBids { get; set; }

        /// <summary>
        /// PURPOSE: Holds the BuddyBids 
        /// </summary>
        public BuddyBids BuddyBids { get; set; }

        /// <summary>
        /// Hold the Company server data or Mock Data. or such like properties.
        /// </summary>
        public Data Data { get; set; }

        /// <summary>
        /// 1 is for Line Summary : 2is ofr Classic Bid Line view & 3 is for Modern Bid line view
        /// </summary>
        public int ViewType { get; set; }

        /// <summary>
        /// PURPOSE:Holds the Miscellanious tab values
        /// </summary>
        public MiscellaneousTab MiscellaneousTab { get; set; }


        /// <summary>
        /// PURPOSE:Holds the pairingExport tab configuration details
        /// </summary>
        public PairingExport PairingExport { get; set; }

        /// <summary>
        /// OUROISE:Holds the Hotel tab configuration details
        /// </summary>
        public SourceHotel SourceHotel { get; set; }

        public User User { get; set; }

		public List<int> ModernNormalColumns { get; set; }
		public List<int> ModernVacationColumns { get; set; }
		public List<int> BidLineNormalColumns { get; set; }
		public List<int> BidLineVacationColumns { get; set; }
		public List<DataColumn> SummaryVacationColumns { get; set; }

		/// <summary>
		/// PURPOSE:Holds the Main Window Size
		/// </summary>
		public MainWindowSize MainWindowSize { get; set; }

		/// <summary>
		/// PURPOSE:Holds the Main Window Size
		/// </summary>
		public MainWindowSize CSWViewSize { get; set; }

		public bool IsConnectedVofox { get; set; }

		public Ratio RatioValues { get; set; }

        public List<SenListFormat> SenioritylistFormat { get; set; }

        public SplitPointCities SplitPointCities { get; set; }

        public string LocalFlightDataVersion { get; set; }


        #endregion

    }
}
