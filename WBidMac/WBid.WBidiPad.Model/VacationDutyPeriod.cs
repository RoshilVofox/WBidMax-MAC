using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VacationDutyPeriod
    {
        [ProtoMember(1)]
        public VacationDetails VacationDetails { get; set; }
        [ProtoMember(2)]
        public List<VacationFlight> FlightDetails { get; set; }
        [ProtoMember(3)]
        public VacationRig VacationRig { get; set; }
        [ProtoMember(4)]
        public int RefDutPdSeqNum { get; set; }

        // frank refactor test
        [ProtoMember(5)]
        public string VacationType { get; set; }
        [ProtoMember(6)]
        public bool isSplit { get; set; }
        [ProtoMember(7)]
        public bool hasSip { get; set; }
        [ProtoMember(8)]
        public int SplitLeg { get; set; }
        [ProtoMember(9)]
        public decimal RigDpMin { get; set; }
        [ProtoMember(10)]
        public decimal RigDhr { get; set; }

        // when the trip has a split duty period or a VD duty period, the Tafb and ADG rig will
        // be distributed to the duty periods and NOT the trip.

        // frank add: when dp is split, the dp may get Tafb or ADG rig
        [ProtoMember(11)]
        public decimal RigTafb { get; set; }
        [ProtoMember(12)]
        public decimal RigAdg { get; set; }
        [ProtoMember(13)]
        public decimal RigDpMin_VA { get; set; }
        [ProtoMember(14)]
        public decimal RigDpMin_VO { get; set; }
        [ProtoMember(15)]
        public decimal RigDpMin_VD { get; set; }
        [ProtoMember(16)]
        public decimal RigDhr_VA { get; set; }
        [ProtoMember(17)]
        public decimal RigDhr_VO { get; set; }
        [ProtoMember(18)]
        public decimal RigDhr_VD { get; set; }

        // frank add: when dp is split, the dp may get Tafb or ADG rig
        [ProtoMember(19)]
        public decimal RigTafb_VA { get; set; }
        [ProtoMember(20)]
        public decimal RigTafb_VO { get; set; }
        [ProtoMember(21)]
        public decimal RigTafb_VD { get; set; }

        // frank add: when dp is split, the dp may get Tafb or ADG rig
        [ProtoMember(22)]
        public decimal RigAdg_VA { get; set; }
        [ProtoMember(23)]
        public decimal RigAdg_VO { get; set; }
        [ProtoMember(24)]
        public decimal RigAdg_VD { get; set; }
        [ProtoMember(25)]
        public int Tafb_VA { get; set; }
        [ProtoMember(26)]
        public int Tafb_VO { get; set; }
        [ProtoMember(27)]
        public int Tafb_VD { get; set; }
        [ProtoMember(28)]

        public bool isInBp { get; set; }
    }
}
