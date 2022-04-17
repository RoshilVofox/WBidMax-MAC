using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VacationRig
    {
        [ProtoMember(1)]
        public decimal VacRigDpMinInLineVD { get; set; }
        [ProtoMember(2)]
        public decimal VacRigDpMinInLineVOVA { get; set; }
        [ProtoMember(3)]
        public decimal VacRigDpMinInBpVD { get; set; }
        [ProtoMember(4)]
        public decimal VacRigDpMinInBpVOVA { get; set; }
        [ProtoMember(5)]
        public decimal VacRigDhrInLineVD { get; set; }
        [ProtoMember(6)]
        public decimal VacRigDhrInLineVOVA { get; set; }
        [ProtoMember(7)]
        public decimal VacRigDhrInBpVD { get; set; }
        [ProtoMember(8)]
        public decimal VacRigDhrInBpVOVA { get; set; }
        [ProtoMember(9)]
        public decimal VacRigAdgInLineVD { get; set; }
        [ProtoMember(10)]
        public decimal VacRigAdgInBpVD { get; set; }
        [ProtoMember(11)]
        public decimal VacRigTafbInLineVD { get; set; }
        [ProtoMember(12)]
        public decimal VacRigTafbInBpVD { get; set; }
        [ProtoMember(13)]
        public decimal VacRigAdgInLineVOVA { get; set; }
        [ProtoMember(14)]
        public decimal VacRigAdgInBpVOVA { get; set; }
        [ProtoMember(15)]
        public decimal VacRigTafbInLineVOVA { get; set; }
        [ProtoMember(16)]
        public decimal VacRigTafbInBpVOVA { get; set; }


    }
}
