using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class BidDetails
    {
        /// <summary>
        /// Purpose:Domicile
        /// </summary>
        public string Domicile { get; set; }
        /// <summary>
        /// Purpose:Postion
        /// </summary>
        public string Postion { get; set; }

        /// <summary>
        /// Purpose:Round
        /// </summary>
        public string Round { get; set; }
        /// <summary>
        /// Purpose:Month
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// Purpose:Year
        /// </summary>
        public int Year { get; set; }
        /// <summary>
        /// Purpose:Equipment
        /// </summary>
        public int Equipment { get; set; }
        /// <summary>
        /// Purpose:BidPeriodStartDate
        /// Pilots: this bid period always starts on the first day of the month
        /// Flight Attendants: Feb bid period starts on 31 Jan, Mar bid period starts on 2 Mar, all other months start on the 1st
        /// </summary>
        public DateTime BidPeriodStartDate { get; set; }
        /// <summary>
        /// Purpose:BidPeriodEndDate
        /// Pilots: the bid period always finished on the last day of the month
        /// Flight Attendants:  Jan bid period ends on 30 Jan, Feb bid period ends on 1 Mar, all other bid periods end on the last day of the month
        /// </summary>
        public DateTime BidPeriodEndDate { get; set; }
    }
}
