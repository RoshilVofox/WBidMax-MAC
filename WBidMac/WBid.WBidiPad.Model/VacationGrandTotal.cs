using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{

    [ProtoContract]
    public class VacationGrandTotal
    {
        [ProtoMember(1)]
        public decimal VD_TfpInLineTot { get; set; }
        [ProtoMember(2)]
        public decimal VD_TfpInBpTot { get; set; }
        [ProtoMember(3)]
        public decimal VD_BlkInLineTot { get; set; }
        [ProtoMember(4)]
        public decimal VD_BlkInBpTot { get; set; }
        [ProtoMember(5)]
        public decimal VD_TafbInLineTot { get; set; }
        [ProtoMember(6)]
        public decimal VD_TafbInBpTot { get; set; }

        [ProtoMember(7)]
        public decimal VA_TfpInLineTot { get; set; }
        [ProtoMember(8)]
        public decimal VA_TfpInBpTot { get; set; }
        [ProtoMember(9)]
        public decimal VA_BlkInLineTot { get; set; }
        [ProtoMember(10)]
        public decimal VA_BlkInBpTot { get; set; }
        [ProtoMember(11)]
        public decimal VA_TafbInLineTot { get; set; }
        [ProtoMember(12)]
        public decimal VA_TafbInBpTot { get; set; }

        // frank refactor add
        [ProtoMember(13)]
        public decimal VO_TfpInLineTot { get; set; }
        [ProtoMember(14)]
        public decimal VO_TfpInBpTot { get; set; }
        [ProtoMember(15)]
        public decimal VO_BlkInLineTot { get; set; }
        [ProtoMember(16)]
        public decimal VO_BlkInBpTot { get; set; }
        [ProtoMember(17)]
        public decimal VO_TafbInLineTot { get; set; }
        [ProtoMember(18)]
        public decimal VO_TafbInBpTot { get; set; }





    }
}
