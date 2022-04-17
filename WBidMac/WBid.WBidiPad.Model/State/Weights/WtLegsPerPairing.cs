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
    public class WtLegsPerPairing
    {

        [DataMember]
        [XmlAttribute("Legs")]
        public int Legs { get; set; }

        [DataMember]
        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }

        [DataMember]
        [XmlAttribute("Type")]
        public int Type { get; set; }


        public WtLegsPerPairing()
        {

        }

        public WtLegsPerPairing(WtLegsPerPairing wtLegsPerPairing)
        {
           Legs= wtLegsPerPairing.Legs;
           Weight = wtLegsPerPairing.Weight;
           Type = wtLegsPerPairing.Type;

        }
    }
}
