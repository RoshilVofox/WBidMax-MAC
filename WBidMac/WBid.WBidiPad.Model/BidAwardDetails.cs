using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    public class BidAwardDetails
    {
        public BidAwardDetails()
        {
        }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Round { get; set; }
        public string Domicile { get; set; }
        public string Position { get; set; }
        public string Message { get; set; }
        public bool isError { get; set; }
        public List<BidAward> BidAwards { get; set; }
    }
    public class BidAward
    {
        public BidAward()
        {
        }
        public int LineNum { get; set; }
        public int EmpNum { get; set; }
        public string Position { get; set; }

        public int SeqNumber { get; set; }
    }
}
