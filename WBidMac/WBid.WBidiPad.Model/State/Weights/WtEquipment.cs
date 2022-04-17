#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
#endregion


namespace WBid.WBidiPad.Model
{
    public class WtEquipment
    {
        /// <summary>
        /// PURPOSe :Days
        /// </summary>
        [XmlAttribute("Legs")]
        public int Legs { get; set; }


        /// <summary>
        /// PURPOSe :Type
        /// </summary>
        [XmlAttribute("Type")]
        public int Type { get; set; }

        /// <summary>
        /// PURPOSe :Weight
        /// </summary>
        [XmlAttribute("WT")]
        public decimal Weight { get; set; }


        public WtEquipment()
        {

        }
        public WtEquipment(WtEquipment wtEquipment)
        {
            Legs = wtEquipment.Legs;
            Type = wtEquipment.Type;
            Weight = wtEquipment.Weight;

        }
    }
}
