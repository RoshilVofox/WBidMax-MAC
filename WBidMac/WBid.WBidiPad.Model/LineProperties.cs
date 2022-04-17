#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model
{
    public class LineProperties
    {
        //ToIdentify whether it is a download process or not
        public bool IsDownloadProcess { get; set; }

        public Dictionary<string, Trip> TripData { get; set; }

        public Dictionary<string, Line> LinesData { get; set; }

        public BidDetails CurrentBidDetail { get; set; }

        public bool IsOverlapCorrection { get; set; }

        public bool IsVacationCorrection{ get; set; }

        public bool IsDrop{ get; set; }

        public bool IsEomChecked{ get; set; }

        public DateTime EOMStartdate{ get; set; }
         
    }
}
