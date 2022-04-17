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
    public class WtGroundTime
    {

        /// <summary>
        /// PURPOSe :Length
        /// </summary>
        [XmlAttribute("Length")]
        public int Length { get; set; }


        /// <summary>
        /// PURPOSe :Occur
        /// </summary>
        [XmlAttribute("Occur")]
        public int Occur { get; set; }

        /// <summary>
        /// PURPOSe :Weight
        /// </summary>
        [XmlAttribute("WT")]
        public decimal Weight { get; set; }


        public WtGroundTime()
        {

        }
        public WtGroundTime(WtGroundTime wtGroundTime)
        {
            Length = wtGroundTime.Length;
            Occur = wtGroundTime.Occur;
            Weight = wtGroundTime.Weight;

        }
    }
}
