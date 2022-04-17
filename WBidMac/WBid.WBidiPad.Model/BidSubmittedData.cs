using System;
namespace WBid.WBidiPad.Model
{
    public class BidSubmittedData
    {
        public BidSubmittedData()
        {
        }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Round { get; set; }
        public string Domicile { get; set; }
        public string Position { get; set; }
        public int EmpNum { get; set; }
        public string SubmittedResult { get; set; }
        public string SubmitBy { get; set; }
        public string SubmitFor { get; set; }
        public string SubmitDTG { get; set; }
        public int FromApp { get; set; }
        public string RawData { get; set; }

    }
}

