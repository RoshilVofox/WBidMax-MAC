using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class Hotels
    {



        /// <summary>
        /// PURPOSE : Effective Date
        /// </summary>
        [XmlAttribute("Effective")]
        public string Effective { get; set; }


        /// <summary>
        /// PURPOSE : List of Hotels
        /// </summary>

        public List<Hotel> HotelList { get; set; }

    }
    public class Hotel
    {

        /// <summary>
        /// PURPOSE : City Name
        /// </summary>
        [XmlAttribute("City")]
        public string City { get; set; }


        /// <summary>
        /// PURPOSE : Hotel Name
        /// </summary>
        [XmlAttribute("Hotel")]
        public string Hotels { get; set; }

    }
}
