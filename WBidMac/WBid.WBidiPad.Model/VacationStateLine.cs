using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VacationStateLine
    {
        public VacationStateLine()
        {
            VacationBlkHrs = "00:00";
            VacationDropBlkHrs = "00:00";
            VacationTafb = "00:00";
            VacationDropTafb = "00:00";
        }

        // VA+VO+VAP
        [ProtoMember(1)]
        public decimal VacationTfp { get; set; }
        //VD
        [ProtoMember(2)]
        public decimal VacationDropTfp { get; set; }

        [ProtoMember(3)]
        public string VacationBlkHrs { get; set; }

        [ProtoMember(4)]
        public string VacationDropBlkHrs { get; set; }

        [ProtoMember(5)]
        public string VacationTafb { get; set; }

        [ProtoMember(6)]
        public string VacationDropTafb { get; set; }

        [ProtoMember(7)]
        public int VacationDaysOff { get; set; }

        [ProtoMember(8)]
        public int VacationDropDaysOff { get; set; }

        [ProtoMember(9)]
        public int VacationLegs { get; set; }

        [ProtoMember(10)]
        public int VacationDropLegs { get; set; }

        [ProtoMember(11)]
        public decimal VacationFront { get; set; }

        [ProtoMember(12)]
        public decimal VacationBack { get; set; }

        [ProtoMember(13)]
        public decimal FlyPay { get; set; }

        [ProtoMember(14)]
        public decimal FlyPayInLine { get; set; }

        [ProtoMember(15)]
        public List<VacationStateTrip> VacationTrips { get; set; }

        [ProtoMember(16)]
        public decimal VacationTfpInLine { get; set; }




    }
}

