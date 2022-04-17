using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class VacationStateDutyPeriod
    {
        [ProtoMember(1)]
        public int DutyperidSeqNo { get; set; }

        [ProtoMember(2)]
        public string DutyPeriodType { get; set; }

        [ProtoMember(3)]
        public List<VacationStateFlight> VacationFlights { get; set; }
    }
}
