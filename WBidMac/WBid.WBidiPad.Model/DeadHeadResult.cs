using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class DeadHeadResult
    {
        [ProtoMember(1)]
        public string Route { get; set; }
        [ProtoMember(2)]
        public string RtDep { get; set; }
        [ProtoMember(3)]
        public string RtArr { get; set; }
        [ProtoMember(4)]
        public string RtTime { get; set; }
    }
}
