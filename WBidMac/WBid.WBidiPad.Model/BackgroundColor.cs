using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class BackgroundColor
    {

        [XmlAttribute("Dialog")]
        public string DialogColor { get; set; }


        [XmlAttribute("Text")]
        public string TextColor { get; set; }

        [XmlAttribute("AMTrips")]
        public string AMTripsColor { get; set; }

        [XmlAttribute("PMTrips")]
        public string PMTripsColor { get; set; }

        [XmlAttribute("MixedTrips")]
        public string MixedTripsColor { get; set; }

        [XmlAttribute("AMReserve")]
        public string AMReserveColor { get; set; }

        [XmlAttribute("PMReserve")]
        public string PMReserveColor { get; set; }

        [XmlAttribute("ReadyReserve")]
        public string ReadyReserveColor { get; set; }

        [XmlAttribute("VacationOverlap")]
        public string VacationOverlapColor { get; set; }

        [XmlAttribute("VacationDrop")]
        public string VacationDropColor { get; set; }

        [XmlAttribute("Vacation")]
        public string VacationColor { get; set; }
    }
}
