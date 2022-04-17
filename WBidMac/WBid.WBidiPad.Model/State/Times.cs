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
    public class Times
    {
        public Times()
        { }
        public Times(Times times)
        {
            if (times != null)
            {
                Checkin = times.Checkin;
                BackToBase = times.BackToBase;
            }
        }

        [DataMember]
        [XmlAttribute("Checkin")]
        public int Checkin { get; set; }

        [DataMember]
        [XmlAttribute("BackToBase")]
        public int BackToBase { get; set; }
    }
}
