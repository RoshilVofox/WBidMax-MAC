using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{

    [ProtoContract]
    public class VacationDetails
    {
        [ProtoMember(1)]
        public string VacationType { get; set; }
        [ProtoMember(2)]
        public decimal VDfTfp { get; set; }
        [ProtoMember(3)]
        public decimal VDfBlock { get; set; }
        [ProtoMember(4)]
        public decimal VDfTafb { get; set; }
        [ProtoMember(5)]
        public decimal VOfTfp { get; set; }
        [ProtoMember(6)]
        public decimal VOfBlock { get; set; }
        [ProtoMember(7)]
        public decimal VOfTafb { get; set; }
        [ProtoMember(8)]
        public decimal VATfpInLine { get; set; }
        [ProtoMember(9)]
        public decimal VATfpInBp { get; set; }
        [ProtoMember(10)]
        public decimal VABlockInLIne { get; set; }
        [ProtoMember(11)]
        public decimal VABlockInBp { get; set; }
        [ProtoMember(12)]
        public decimal VATafbInLine { get; set; }
        [ProtoMember(13)]
        public decimal VATafbInBp { get; set; }
        [ProtoMember(14)]

        public decimal VObTfpInLine { get; set; }
        [ProtoMember(15)]
        public decimal VObTfpInBp { get; set; }
        [ProtoMember(16)]
        public decimal VObBlockInLine { get; set; }
        [ProtoMember(17)]
        public decimal VObBlockInBp { get; set; }
        [ProtoMember(18)]
        public decimal VObTafbInLine { get; set; }
        [ProtoMember(19)]
        public decimal VObTafbInBp { get; set; }
        [ProtoMember(20)]
        public decimal VDbTfpInLine { get; set; }
        [ProtoMember(21)]
        public decimal VDbTfpInBp { get; set; }
        [ProtoMember(22)]
        public decimal VDbBlockInLine { get; set; }
        [ProtoMember(23)]
        public decimal VDbBlockInBp { get; set; }
        [ProtoMember(24)]
        public decimal VDbTafbInLine { get; set; }
        [ProtoMember(25)]
        public decimal VDbTafbInBp { get; set; }

    }
}
