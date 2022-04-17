using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{

    public class Week
    {

        #region Properties
        [XmlAttribute("IsMaxWeekend")]
        public bool IsMaxWeekend { get; set; }

        [XmlAttribute("MaxNumber")]
        public string MaxNumber { get; set; }

        [XmlAttribute("MaxPercentage")]
        public string MaxPercentage { get; set; }


        //--Default values
        //------------------------------
        [XmlAttribute("IsMaxWeekendD")]
        public bool IsMaxWeekendDefault { get; set; }

        [XmlAttribute("MaxNumberD")]
        public string MaxNumberDefault { get; set; }

        [XmlAttribute("MaxPercentageD")]
        public string MaxPercentageDefault { get; set; }
        //------------------------------

        [XmlAttribute("StartDOW")]
        public string StartDOW { get; set; }
        #endregion

    }
}
