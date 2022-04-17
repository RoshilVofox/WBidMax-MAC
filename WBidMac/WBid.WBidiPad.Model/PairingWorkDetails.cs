using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class PairingWorkDetails
    {
        [ProtoMember(1)]
        public int StartTime { get; set; }

        [ProtoMember(2)]
        public int EndTime { get; set; }
        [ProtoMember(3)]
        public int StartDay { get; set; }

        [ProtoMember(4)]
        public int EndDay { get; set; }
    }

   
}
