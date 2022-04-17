using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{

    public class TripLengthConstraints
    {
        public TripLengthConstraints()
        {
        }
        public TripLengthConstraints(TripLengthConstraints tripLengthConstraints)
        {
            Turns = tripLengthConstraints.Turns;
            Twoday = tripLengthConstraints.Twoday;
            ThreeDay = tripLengthConstraints.ThreeDay;
            FourDay = tripLengthConstraints.FourDay;
        }

        [XmlAttribute("Turns")]
        public bool Turns { get; set; }
        [XmlAttribute("Twoday")]
        public bool Twoday { get; set; }
        [XmlAttribute("ThreeDay")]
        public bool ThreeDay { get; set; }
        [XmlAttribute("FourDay")]
        public bool FourDay { get; set; }

    }
}
