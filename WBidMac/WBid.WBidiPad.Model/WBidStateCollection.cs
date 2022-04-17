using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
   
    public class WBidStateCollection
    {
        [XmlAttribute("DefaultName")]
        public string DefaultName { get; set; }

        [XmlAttribute("DataSource")]
        public string DataSource { get; set; }

        [XmlElement("StateList")]
        public List<WBidState> StateList { get; set; }

        [XmlElement("Vacation")]
        public List<Vacation> Vacation { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlAttribute("SyncVersion")]
        public string SyncVersion { get; set; }

        [XmlAttribute("IsModified")]
        public bool IsModified { get; set; }

        [XmlAttribute("IsQuickSetModified")]
        public bool IsQuickSetModified { get; set; }

        [XmlAttribute("StateUpdatedTime")]
        public DateTime StateUpdatedTime { get; set; }

		[XmlElement("SeniorityListItem")]
        public SeniorityListItem SeniorityListItem { get; set; }

        [XmlAttribute("CompanyVA")]
        public string CompanyVA { get; set; }

        [XmlElement("FVVacation")]
        public List<Absense> FVVacation { get; set; }

        [XmlElement("BidAwards")]
        public List<BidAward> BidAwards { get; set; }

        [XmlElement("SubmittedResult")]
        public string SubmittedResult { get; set; }


    }

   
    public class WBidState
    {
        public WBidState()
        {
            TopLockCount = 0;
            BottomLockCount = 0;
        }


        public WBidState(WBidState wBidState)
        {
            TopLockCount = wBidState.TopLockCount;
            BottomLockCount = wBidState.BottomLockCount;
            StateName = wBidState.StateName;
            LineOrders = new LineOrders(wBidState.LineOrders);
            SortDetails = new SortDetails(wBidState.SortDetails);
           Constraints = new Constraints(wBidState.Constraints);
            Weights = new Weights(wBidState.Weights);
            CxWtState = new CxWtState(wBidState.CxWtState);
            ForceLine = new ForceLine(wBidState.ForceLine);
//            TagDetails = new TagDetails(wBidState.TagDetails);
            Theme = new Theme(wBidState.Theme);
            IsOverlapCorrection = wBidState.IsOverlapCorrection;
            IsVacationOverlapOverlapCorrection = wBidState.IsVacationOverlapOverlapCorrection;
            MenuBarButtonState = new MenuBarButtonStatus(wBidState.MenuBarButtonState);
            FAEOMStartDate = wBidState.FAEOMStartDate;

			if (wBidState.MILDateList != null)
			{
				MILDateList = new List<Absense> ();
				foreach (var item in wBidState.MILDateList)
				{
					MILDateList.Add(new Absense(item));

				}
			}

			

        }




        [XmlElement("StateName")]
        public string StateName { get; set; }


        [XmlElement("TopLockCount")]
        public int TopLockCount { get; set; }

        [XmlElement("BottomLockCount")]
        public int BottomLockCount { get; set; }

        public LineOrders LineOrders { get; set; }

        public SortDetails SortDetails { get; set; }

        public Constraints Constraints { get; set; }

        public Weights Weights { get; set; }

        public CxWtState CxWtState { get; set; }

        /// <summary>
        /// Hold whether the User selects the Blank line to bottom or Reverse line to bottom
        /// </summary>
        public ForceLine ForceLine { get; set; }


        public TagDetails TagDetails { get; set; }


        /// <summary>
        /// PURPOSe :Theme
        /// </summary>
        public Theme Theme { get; set; }

        [XmlElement("IsOverlap")]
        public bool IsOverlapCorrection { get; set; }

        [XmlElement("IsVacation")]
        public bool IsVacationOverlapOverlapCorrection { get; set; }

        /// <summary>
        /// This will hold the check box states for the overlap,vacation and EOM checkbox
        /// </summary>
        //public CheckBoxState CheckBoxState { get; set; }


        /// <summary>
        /// This will hold the menu bar states for the overlap,vacation and EOM checkbox
        /// </summary>
        public MenuBarButtonStatus MenuBarButtonState { get; set; }

        [XmlElement("EOMDate")]
        public DateTime FAEOMStartDate { get; set; }

		[XmlElement("MILDate")]
		public List<Absense> MILDateList { get; set; }

		//For identify BidAuto On or OFF
		[XmlElement("BidAutoOn")]
		public bool BidAutoOn { get; set; }


		//For keeping the BidAutomator settings
		public BidAutomator BidAuto { get; set; }



		//For keeping the calculated BidAutomator state
		public BidAutomator CalculatedBA { get; set; }


		[XmlElement("IsMissingTripFailed")]
		public bool IsMissingTripFailed { get; set; }

        [XmlElement("LineForBlueLine")]
        public int LineForBlueLine { get; set; }

        [XmlElement("LinesForBlueBorder")]
        public List<int> LinesForBlueBorder { get; set; }

    }
}
