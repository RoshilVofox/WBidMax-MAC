using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class Day 
    {
      
        [ProtoMember(1)]
        public DateTime Date { get; set; }
     
        [ProtoMember(2)]
        public string FlightTimeHour { get; set; }
     
        [ProtoMember(3)]
        public int FlightTime { get; set; }
    
    
        [ProtoMember(4)]
        public int DepartutreTime { get; set; }
       
     
        [ProtoMember(7)]
        public int ArrivalTime { get; set; }

        [ProtoMember(8)]
        public bool OffDuty { get; set; }

        [ProtoMember(9)]
        public string ArrivalCity { get; set; }

        [ProtoMember(10)]
        public string DepartutreCity { get; set; }




    }
}
