using System;
namespace WBid.WBidiPad.Model
{
    public class SubmittedDataRaw
    {
        public SubmittedDataRaw()
        {
        }
        public string Domicile { get; set; }
        public string Position { get; set; }
        public int Month { get; set; }
        public int Round { get; set; }
        public int Year { get; set; }
        public string RawData { get; set; }
        public int FromApp { get; set; }
        public string EmployeeNumber { get; set; }
    }
}
