using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VacationStateFlight
    {
        [ProtoMember(1)]
        public int FlightSeqNo { get; set; }
        [ProtoMember(2)]
        public string FlightType { get; set; }
    }
}
