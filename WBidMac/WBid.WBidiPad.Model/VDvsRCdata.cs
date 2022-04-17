using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VDvsRCdata
    {
        [ProtoMember(1)]
        public decimal TotVDCost { get; set; }
        [ProtoMember(2)]
        public decimal TotRCCost { get; set; }
        [ProtoMember(3)]
        public int RCcalcDutyPeriod { get; set; }
        [ProtoMember(4)]
        public int RCcalcFlightSeqNum { get; set; }
        [ProtoMember(5)]
        public bool isSplit { get; set; }
        [ProtoMember(6)]
        public bool RCisDayPriorOrAfter { get; set; }
        [ProtoMember(7)]
        public List<DeadHeadResult> pltDh { get; set; }
        [ProtoMember(8)]
        public List<RouteDomain> resDh { get; set; }
    }

    [ProtoContract]
    public class RouteDomain
    {
        [ProtoMember(1)]
        public DateTime Date { get; set; }
        [ProtoMember(2)]
        public string Route { get; set; }
        [ProtoMember(3)]
        public Int64 RtDep { get; set; }
        [ProtoMember(4)]
        public Int64 RtArr { get; set; }
        [ProtoMember(5)]
        public Int64 RtTime { get; set; }
        [ProtoMember(6)]
        public Int64 Rt1 { get; set; }
        [ProtoMember(7)]
        public Int64 Rt2 { get; set; }
        [ProtoMember(8)]
        public Int64 Rt3 { get; set; }
        [ProtoMember(9)]
        public Int64 Rt1Dep { get; set; }
        [ProtoMember(10)]
        public Int64 Rt2Dep { get; set; }
        [ProtoMember(11)]
        public Int64 Rt3Dep { get; set; }
        [ProtoMember(12)]
        public Int64 Rt1Arr { get; set; }
        [ProtoMember(13)]
        public Int64 Rt2Arr { get; set; }
        [ProtoMember(14)]
        public Int64 Rt3Arr { get; set; }

        // added by frank to get tfp by dist
        [ProtoMember(15)]
        public string Rt1Orig { get; set; }
        [ProtoMember(16)]
        public string Rt2Orig { get; set; }
        [ProtoMember(17)]
        public string Rt3Orig { get; set; }
        [ProtoMember(18)]
        // added by frank to get tfp by dist
        public string Rt1Dest { get; set; }
        [ProtoMember(19)]
        public string Rt2Dest { get; set; }
        [ProtoMember(20)]
        public string Rt3Dest { get; set; }

        // added by frank for display purposes
        [ProtoMember(21)]
        public int Rt1FltNum { get; set; }
        [ProtoMember(22)]
        public int Rt2FltNum { get; set; }
        [ProtoMember(23)]
        public int Rt3FltNum { get; set; }
        [ProtoMember(24)]
        public Int64 Con1 { get; set; }
        [ProtoMember(25)]
        public Int64 Con2 { get; set; }

    }


    [Serializable]
    public class TfpDomain
    {
        public decimal Flight1Tfp { get; set; }

        public decimal Flight2Tfp { get; set; }

        public decimal Flight3Tfp { get; set; }

        public decimal TotTfp { get; set; }

    }
}
