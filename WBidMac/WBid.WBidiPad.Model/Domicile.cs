using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    public class Domicile
    {
        public Domicile()
        {
        }

        /// <summary>
        /// DomicileId
        /// </summary>
        [XmlAttribute("DomicileId")]
        public int DomicileId { get; set; }

        /// <summary>
        /// DomicileName
        /// </summary>
        [XmlAttribute("DomicileName")]
        public string DomicileName { get; set; }

        /// <summary>
        /// Number
        /// </summary>
        [XmlAttribute("Number")]
        public int Number { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        [XmlAttribute("Code")]
        public string Code { get; set; }

    }
}
