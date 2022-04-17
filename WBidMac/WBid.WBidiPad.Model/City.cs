using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    public class City
    {
        /// <summary>
        /// Id
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [XmlAttribute("code")]
        public int Code { get; set; }

        /// <summary>
        /// dst
        /// </summary>
        [XmlAttribute("dst")]
        public string dst { get; set; }

        [XmlAttribute("International")]
        public bool International { get; set; }

        [XmlAttribute("NonConus")]
        public bool NonConus { get; set; }

        /// <summary>
        /// For overnight city bulk calculation.
        /// 0 indicates Non,1 indicates Yes,2 indicates No.
        /// </summary>
        [XmlAttribute("Status")]
        public int Status { get; set; }

        [XmlAttribute("Hawai")]
        public bool Hawai { get; set; }

    }
}
