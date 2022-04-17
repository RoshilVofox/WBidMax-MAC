using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class FlightPlan
    {
        [ProtoMember(1)]
        public List<FlightDetails> FlightDetails { get; set; }

        [ProtoMember(2)]
        public List<FlightRoute> FlightRoutes { get; set; }
    }

    [ProtoContract]
    public class FlightDetails
    {
        #region Properties
        [ProtoMember(1)]
        public Int32 FlightId { get; set; }
        [ProtoMember(2)]
        public Int32 Flight { get; set; }
        [ProtoMember(3)]
        public string Orig { get; set; }
        [ProtoMember(4)]
        public string Dest { get; set; }
        [ProtoMember(5)]
        public int Cdep { get; set; }
        [ProtoMember(6)]
        public int Carr { get; set; }
        [ProtoMember(7)]
        public int Ldep { get; set; }
        [ProtoMember(8)]
        public int Larr { get; set; }


        #endregion


    }
    [ProtoContract]
    public class FlightRoute
    {
        [ProtoMember(1)]
        public Int32 FlightId { get; set; }
        [ProtoMember(2)]
        public DateTime FlightDate { get; set; }
        [ProtoMember(3)]
        public int RouteNum { get; set; }
    }
}
