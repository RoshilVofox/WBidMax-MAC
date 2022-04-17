using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class RCtimes
    {
        [ProtoMember(1)]
        public DateTime date { get; set; }
        [ProtoMember(2)]
        public int depTime { get; set; }
        [ProtoMember(3)]
        public int arrTime { get; set; }
        [ProtoMember(4)]
        public DateTime rc0date { get; set; }

        [ProtoMember(5)]
        public int rc0depTime { get; set; }      
        
        [ProtoMember(6)]
        public int rc0arrTime { get; set; }
    }
}
