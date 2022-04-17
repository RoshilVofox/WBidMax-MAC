#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization; 
#endregion

namespace WBid.WBidiPad.Model
{
    public class AvoidanceBids
    {


        [XmlAttribute("Avoidance1")]
        public string Avoidance1 { get; set; }

        [XmlAttribute("Avoidance2")]
        public string Avoidance2 { get; set; }

        [XmlAttribute("Avoidance3")]
        public string Avoidance3 { get; set; }
    }
}
