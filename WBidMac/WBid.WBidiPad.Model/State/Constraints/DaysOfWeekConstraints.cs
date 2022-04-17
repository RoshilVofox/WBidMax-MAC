using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     [Serializable]
    public class DaysOfWeekConstraints
    {

         public DaysOfWeekConstraints()
         {
         }
         public DaysOfWeekConstraints(DaysOfWeekConstraints daysOfWeekConstraints)
         {
             SUN = daysOfWeekConstraints.SUN;
             MON = daysOfWeekConstraints.MON;
             TUE = daysOfWeekConstraints.TUE;
             WED = daysOfWeekConstraints.WED;
             THU = daysOfWeekConstraints.THU;
             FRI = daysOfWeekConstraints.FRI;
             SAT = daysOfWeekConstraints.SAT;
         }
        [XmlAttribute("SUN")]
        public bool SUN { get; set; }
        [XmlAttribute("MON")]
        public bool MON { get; set; }
        [XmlAttribute("TUE")]
        public bool TUE { get; set; }
        [XmlAttribute("WED")]
        public bool WED { get; set; }
        [XmlAttribute("THU")]
        public bool THU { get; set; }
        [XmlAttribute("FRI")]
        public bool FRI { get; set; }
        [XmlAttribute("SAT")]
        public bool SAT { get; set; }
    }
}
