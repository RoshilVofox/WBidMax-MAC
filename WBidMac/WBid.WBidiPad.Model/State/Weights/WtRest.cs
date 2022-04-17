using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace WBid.WBidiPad.Model
{
    [DataContract]
    [Serializable]
    public class WtRest
    {
        [DataMember]
        [XmlAttribute("Time")]
        public int Time { get; set; }

        [DataMember]
        [XmlAttribute("Wt")]
        public decimal Wt { get; set; }

        [DataMember]
        [XmlAttribute("Apply")]
        public int Apply { get; set; }

        [DataMember]
        [XmlAttribute("Type")]
        public int Type { get; set; }


        public WtRest()
        {

        }

        public WtRest(WtRest wtRest)
        {
            Time = wtRest.Time;
            Wt = wtRest.Wt;
            Apply = wtRest.Apply;
            Type = wtRest.Type;


        }
    }
}
