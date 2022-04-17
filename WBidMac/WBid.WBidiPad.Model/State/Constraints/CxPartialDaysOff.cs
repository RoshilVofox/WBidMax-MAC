using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     [DataContract]
    [Serializable]
    public class CxPDOFS : List<CxPDOF>
    {
    }
    [DataContract]
    [Serializable]
    public class CxPDOF
    {

        [DataMember]
        [XmlAttribute("Date")]
        public int Date { get; set; }


        [DataMember]
        [XmlAttribute("City")]
        public int City { get; set; }


        [DataMember]
        [XmlAttribute("Time")]
        public int Time { get; set; }


        [DataMember]
        [XmlAttribute("BA")]
        public bool IsBefore { get; set; }
    }
}
