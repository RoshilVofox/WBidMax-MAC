using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VacationFlight
    {
        [ProtoMember(1)]
        public string FlightNumber { get; set; }
        [ProtoMember(2)]
        public VacationDetails VacationDetail { get; set; }
        [ProtoMember(3)]
        public int RefFltSeqNum { get; set; }

        // frank refactor test
        [ProtoMember(4)]
        public decimal Tfp { get; set; }
        [ProtoMember(5)]
        public int Block { get; set; }
        [ProtoMember(6)]
        public string VacationType { get; set; }
    }
}
