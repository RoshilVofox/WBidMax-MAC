using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VacationTrip
    {
        [ProtoMember(1)]
        public string TripName { get; set; }

        [ProtoMember(2)]
        public VacationDetails VacationDetails { get; set; }

        [ProtoMember(3)]
        public List<VacationDutyPeriod> DutyPeriodsDetails { get; set; }

        [ProtoMember(4)]
        public VacationRig VacationRig { get; set; }

        [ProtoMember(5)]
        public VacationGrandTotal VacationGrandTotal { get; set; }

        [ProtoMember(6)]
        public string VacationOverlapLength { get; set; }

        // frank refactor test

        [ProtoMember(7)]
        public VDvsRCdata VDvsRCdata { get; set; }

        [ProtoMember(8)]
        public RCtimes RCtimes { get; set; }

        [ProtoMember(9)]
        public bool HasSip { get; set; }

        [ProtoMember(10)]
        public string VacationType { get; set; }

        [ProtoMember(11)]
        public decimal RigAdg { get; set; }
        [ProtoMember(12)]
        public decimal RigTafb { get; set; }

        [ProtoMember(13)]

        public decimal RigVAVO_Adg { get; set; }
        [ProtoMember(14)]
        public decimal RigVD_Adg { get; set; }
        [ProtoMember(15)]

        public decimal RigVAVO_Tafb { get; set; }
        [ProtoMember(16)]
        public decimal RigVD_Tafb { get; set; }

        // frank add for distribution of rig if trip is Split (at least one flight is VD)
        [ProtoMember(17)]
        public bool isSplitTrip { get; set; }
    }
}