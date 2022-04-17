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
   
    public class WtAircraftChange
    {
        [XmlAttribute("WT")]
        public decimal Weight { get; set; }

        [XmlAttribute("Occur")]
        public int Occur { get; set; }

        public WtAircraftChange()
        {
                
        }
        public WtAircraftChange(WtAircraftChange wtAircraftChange)
        {

            Weight = wtAircraftChange.Weight;
            Occur = wtAircraftChange.Occur;
        }

    }
}
