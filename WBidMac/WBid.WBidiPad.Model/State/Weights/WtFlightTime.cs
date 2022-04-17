#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
#endregion
namespace WBid.WBidiPad.Model
{
    public class WtFlightTime
    {

        /// <summary>
        /// PURPOSe : Weight
        /// 
        /// </summary>
        [XmlAttribute("WT")]
        public decimal Weight { get; set; }


        /// <summary>
        /// PURPOSe :Flight Time
        /// </summary>
        [XmlAttribute("Flt")]
        public string FlightTime { get; set; }


        public WtFlightTime()
        {

        }


        public WtFlightTime(WtFlightTime wtFlightTime)
        {

            Weight = wtFlightTime.Weight;
            FlightTime = wtFlightTime.FlightTime; 
        }

       
    }
}
