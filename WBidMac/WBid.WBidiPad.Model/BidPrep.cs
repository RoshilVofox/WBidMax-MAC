using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class BidPrep
    {
        public int BidYear { get; set; }

        public int BidPeriod { get; set; }

        public string Domicile { get; set; }

        public string Position { get; set; }

        public string BidRound { get; set; }

        public int LineFrom { get; set; }

        public int LineTo { get; set; }

        public bool IsOnStartWithCurrentLine { get; set; }

        public bool IsChkAvoidanceBid { get; set; }



    }
}
