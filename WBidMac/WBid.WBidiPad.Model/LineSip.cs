using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class LineSip
    {
        [ProtoMember(1)]
        public Sip Sip { get; set; }
        [ProtoMember(2)]
        public bool Dropped { get; set; }
        [ProtoMember(3)]
        public int SipFltHrs { get; set; }
        [ProtoMember(4)]
        public decimal SipTfp { get; set; }
        [ProtoMember(5)]
        public DateTime SipStartDate { get; set; }
    }
}
