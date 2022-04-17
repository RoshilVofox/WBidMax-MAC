using System;
namespace WBid.WBidiPad.Model
{
    public class Weekday
    {
        public Weekday()
        {
        }
        public int Day { get; set; }
        public string Code { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
