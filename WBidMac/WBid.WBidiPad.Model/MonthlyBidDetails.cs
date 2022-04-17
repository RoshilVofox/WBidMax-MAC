using System;
namespace WBid.WBidiPad.Model
{
    public class MonthlyBidDetails
    {
        public MonthlyBidDetails()
        {
        }
        public int Year { get; set; }

        public int Month { get; set; }

        public int Round { get; set; }

        public string Domicile { get; set; }

        public string Position { get; set; }

    }
}
