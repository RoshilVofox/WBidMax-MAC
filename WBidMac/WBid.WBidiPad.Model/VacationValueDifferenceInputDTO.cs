using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    //
    public class VacationValueDifferenceInputDTO
    {
        public VacationValueDifferenceInputDTO()
        {
        }
        public UserBidDetails BidDetails { get; set; }

        public int FromApp { get; set; }

        public bool IsVAC { get; set; }
        public bool IsDrop { get; set; }
        public bool IsEOM { get; set; }
        public int FAEOMStartDate { get; set; }
        public List<VacationInfo> lstVacation { get; set; }
    }
}
