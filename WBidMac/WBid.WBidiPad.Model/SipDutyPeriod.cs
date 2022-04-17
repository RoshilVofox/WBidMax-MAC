using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class SipDutyPeriod
    {
        private List<Flight> flights = new List<Flight>();
        [ProtoMember(1)]
        public List<Flight> Flights
        {
            get { return flights; }
            set { flights = value; }
        }
        [ProtoMember(2)]
        public int SdpSeqNum { get; set; }
        [ProtoMember(3)]
        public int FlightHours { get; set; }
    }
}
