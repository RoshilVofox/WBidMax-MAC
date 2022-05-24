using System;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class CommuteFltChangeValues
    {
        public CommuteFltChangeValues()
        {
        }
        
        public int LineNum { get; set; }
        public decimal OldCmtOV { get; set; }
        public decimal OldCmtFr { get; set; }
        public decimal OldCmtBa { get; set; }
        public decimal NewCmtOV { get; set; }
        public decimal NewCmtFr { get; set; }
        public decimal NewCmtBa { get; set; }
    }

    [Serializable]
    public class CommuteFltChange
    {

        public CommuteFltChange()
        {
        }
        public string FlightDataVersion { get; set; }
        public List<CommuteFltChangeValues> LstCommuteFltChangeValues { get; set; }


    }
}
