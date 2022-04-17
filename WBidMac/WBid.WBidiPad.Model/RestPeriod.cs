using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class RestPeriod
    {
         [ProtoMember(1)]
        public int PeriodId { get; set; }

         [ProtoMember(2)]
        public int RestMinutes { get; set; }

         [ProtoMember(3)]
        public bool IsInTrip { get; set; }
    }
}
