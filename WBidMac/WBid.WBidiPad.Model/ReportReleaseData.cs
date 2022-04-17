using System;
namespace WBid.WBidiPad.Model
{
    public class ReportReleaseData
    {
        public ReportReleaseData()
        {
        }
        public DateTime Date { get; set; }
        public int Report { get; set; }
        public int Release { get; set; }

        /// <summary>
        /// 1 for Start day,2 for normal day and 3 for last day
        /// </summary>
        public int DaysStatus { get; set; }
    }
}
