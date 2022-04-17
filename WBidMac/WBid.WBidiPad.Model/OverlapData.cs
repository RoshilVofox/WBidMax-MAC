using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
       [ProtoContract]
    public class OverlapData
    {
        /// <summary>
        /// Store the last leg arrival time of the previous bid period to test rest  legality condition(atleat 9 hours between lead out days and lead in days )
        /// </summary>
        //[XmlAttribute("LastLegArrivalTime")]
        [ProtoMember(1)]
        public string LastLegArrivalTime { get; set; }

        /// <summary>
        /// Store Overlap correction dayas and flight time from the overlap corrrection Dialogue
        /// </summary>
        [ProtoMember(2)]
        public List<Day> LeadOutDays { get; set; }
    }

  
    //public class OverlapDay
    //{

    //    [XmlAttribute("Date")]
    //    public DateTime Date { get; set; }

    //    [XmlAttribute("FlightTimeHour")]
    //    public string FlightTimeHour { get; set; }

    //    [XmlAttribute("FlightTime")]
    //    public int FlightTime { get; set; }


    //    [XmlAttribute("DepartutreTime")]
    //    public int DepartutreTime { get; set; }


    //    [XmlAttribute("ArrivalTime")]
    //    public int ArrivalTime { get; set; }

    //    [XmlAttribute("OffDuty")]
    //    public bool OffDuty { get; set; }

    //    [XmlAttribute("ArrivalCity")]
    //    public string ArrivalCity { get; set; }

    //    [XmlAttribute("DepartutreCity")]
    //    public string DepartutreCity { get; set; }




    //}
}
