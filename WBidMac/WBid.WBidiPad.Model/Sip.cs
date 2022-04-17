using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class Sip
    {
        private List<SipDutyPeriod> sipDutyPeriods = new List<SipDutyPeriod>();
        [ProtoMember(1)]
        public List<SipDutyPeriod> SipDutyPeriods
        {
            get { return sipDutyPeriods; }
            set { sipDutyPeriods = value; }
        }
        [ProtoMember(2)]
        public string SipName { get; set; }
        /// <summary>
        /// This hold the sip start day from the trip start day.(For ex: if the trip starts 3rd april and Second sip comes on 2nd day ,it will hold 2 in SipStartDay)
        /// </summary>
        [ProtoMember(3)]
        public int SipStartDay { get; set; }
        [ProtoMember(4)]
        public int SipFltHrs { get; set; }
        [ProtoMember(5)]
        public decimal SipTfp { get; set; }
        [ProtoMember(6)]
        public int SipTAFB { get; set; }
    }
}
