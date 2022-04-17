using System;
namespace WBid.WBidiPad.Model
{
    public class StateQuickSetSyncDTO
    {
        public StateQuickSetSyncDTO()
        {
        }
        public string EmployeeNumber { get; set; }

        public string StateFileName { get; set; }

        public string QuickSetFileName { get; set; }

        public int Year { get; set; }

        public int VersionNumber { get; set; }

        public string StateContent { get; set; }

        public DateTime LastUpdatedTime { get; set; }

        public int QuickSetVersionNumber { get; set; }

        public string QuickSetStateContent { get; set; }

        public DateTime QuickSetLastUpdatedTime { get; set; }
        public string StateLastUpdatedTimeString { get; set; }

        public string QuickSetLastUpdatedTimeString { get; set; }
    }
}
