using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class BidPeriod
    {
        public int BidPeriodId { get; set; }

        /// <summary>
        /// Bid period (January,February etc
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// PURPOSE : Hexa value of month( 1,2...9,A,B,C)
        /// </summary>
        public string HexaValue { get; set; }
    }
}
