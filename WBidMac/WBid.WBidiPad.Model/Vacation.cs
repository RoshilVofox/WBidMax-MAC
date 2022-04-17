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
    public class Vacation
    {
        /// <summary>
        /// vacatio  strart Date
        /// </summary>

        [DataMember]
        [XmlAttribute("StartDate")]
        public string StartDate { get; set; }

        /// <summary>
        /// Vacation End Date
        /// </summary>
        [DataMember]
        [XmlAttribute("EndDate")]
        public string EndDate { get; set; }
    }
}
