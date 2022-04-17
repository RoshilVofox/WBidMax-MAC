using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
   
	[Serializable]
    public class SeniorityListItem
    {
        [XmlAttribute("SeniorityNumber")]
        public int SeniorityNumber { get; set; }

        [XmlAttribute("TotalCount")]
        public int TotalCount { get; set; }

        [XmlAttribute("EBGType")]
        public string EBgType { get; set; }

    }
}
