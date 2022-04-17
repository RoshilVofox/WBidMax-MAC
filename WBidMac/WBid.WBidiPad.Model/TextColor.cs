using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class TextColor
    {
        [XmlAttribute("Dialog")]
        public string DialogText { get; set; }


        [XmlAttribute("Text")]
        public string TextBox { get; set; }

        [XmlAttribute("AMTrips")]
        public string AMTripsText { get; set; }

        [XmlAttribute("PMTrips")]
        public string PMTripsText { get; set; }

        [XmlAttribute("MixedTrips")]
        public string MixedTripsText { get; set; }

        [XmlAttribute("AMReserve")]
        public string AMReserveText { get; set; }

        [XmlAttribute("PMReserve")]
        public string PMReserveText { get; set; }

        [XmlAttribute("ReadyReserve")]
        public string ReadyReserveText { get; set; }

        [XmlAttribute("VacationOverlap")]
        public string VacationOverlapText { get; set; }

        [XmlAttribute("VacationDrop")]
        public string VacationDropText { get; set; }

        [XmlAttribute("Vacation")]
        public string VacationText { get; set; }
    }
}
