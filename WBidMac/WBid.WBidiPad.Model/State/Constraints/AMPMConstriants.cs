using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{

    public class AMPMConstriants
    {
        public AMPMConstriants()
        {
        }
        public AMPMConstriants(AMPMConstriants aMPMConstriants)
        {
            AM = aMPMConstriants.AM;
            PM = aMPMConstriants.PM;
            MIX = aMPMConstriants.MIX;
        }
        [XmlAttribute("AM")]
        public bool AM { get; set; }
        [XmlAttribute("PM")]
        public bool PM { get; set; }
        [XmlAttribute("MIX")]
        public bool MIX { get; set; }
    }
}
