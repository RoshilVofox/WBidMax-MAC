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
    public class WtPerDiem
    {
        [DataMember]
        [XmlAttribute("PerDiemm")]
        public string BreakPoint { get; set; }

        [DataMember]
        [XmlAttribute("Wt")]
        public decimal Wt { get; set; }


        public WtPerDiem()
        {

        }


        public WtPerDiem(WtPerDiem wtPerDiem)
        {
            BreakPoint = wtPerDiem.BreakPoint;
            Wt = wtPerDiem.Wt;

        }
    }
}
