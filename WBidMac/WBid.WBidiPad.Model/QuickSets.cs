using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.Model
{
    public class QuickSets
    {
        public List<QuickSetCSW> QuickSetCSW { get; set; }

        public List<QuickSetColumn> QuickSetColumn { get; set; }

        public string Version { get; set; }
        public string SyncQuickSetVersion { get; set; }
        public DateTime QuickSetUpdatedTime { get; set; }
        public bool IsModified { get; set; }

    }

    public class QuickSetCSW
    {
        
        public string QuickSetCSWName { get; set; }

        //public DateTime DateCreated { get; set; }

        public SortDetails SortDetails { get; set; }

        public Constraints Constraints { get; set; }

        public Weights Weights { get; set; }

        public CxWtState CxWtState { get; set; }

       
    }
     
    [Serializable]
    public class QuickSetColumn
    {
        public string QuickSetColumnName { get; set; }

        //public DateTime DateCreated { get; set; }

        public List<int> ModernNormalColumns { get; set; }

        public List<int> ModernVacationColumns { get; set; }

        public List<int> BidLineNormalColumns { get; set; }

        public List<int> BidLineVacationColumns { get; set; }


		public List<DataColumn> SummaryVacationColumns { get; set; }

		public List<DataColumn> SummaryNormalColumns { get; set; }
    }
}
