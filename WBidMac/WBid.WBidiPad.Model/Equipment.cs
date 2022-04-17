using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    public class Equipment
    {
        /// <summary>
        /// EquipmentId
        /// </summary>
        [XmlAttribute("EquipmentId")]
        public int EquipmentId { get; set; }

        /// <summary>
        /// EquipmenNumber
        /// </summary>
        [XmlAttribute("EquipmentNumber")]
        public int EquipmentNumber { get; set; }

        /// <summary>
        /// EquipmentName
        /// </summary>
        [XmlAttribute("EquipmentName")]
        public string EquipmentName { get; set; }
    }
}
