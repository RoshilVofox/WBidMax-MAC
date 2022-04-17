using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class VacationCorrectionParams
    {
        #region Properties

        /// <summary>
        /// Lines Dictionary
        /// </summary>
        public Dictionary<string, Line> Lines { get; set; }


        /// <summary>
        /// Trips Dictionary
        /// </summary>
        public Dictionary<string, Trip> Trips { get; set; }


        /// <summary>
        /// List of Flight Details
        /// </summary>
        public List<FlightRouteDetails> FlightRouteDetails { get; set; }



        /// <summary>
        /// Current bid Details
        /// </summary>
        public BidDetails CurrentBidDetails { get; set; }




        /// <summary>
        ///Is EOM only
        /// </summary>
        public bool IsEOM { get; set; }
        #endregion

    }
}
