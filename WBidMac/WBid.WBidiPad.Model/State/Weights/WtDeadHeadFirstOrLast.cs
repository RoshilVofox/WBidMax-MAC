using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     [Serializable]
    public class WtDeadHeadFirstOrLast
    {


        /// <summary>
        /// PURPOSe :Apply
        /// </summary>
        [XmlAttribute("First")]
        public decimal First { get; set; }


        /// <summary>
        /// PURPOSe :Time
        /// </summary>
        [XmlAttribute("Last")]
        public decimal Last { get; set; }



        public WtDeadHeadFirstOrLast()
        {

        }

        public WtDeadHeadFirstOrLast(WtDeadHeadFirstOrLast wtDeadHeadFirstOrLast)
        {
            First = wtDeadHeadFirstOrLast.First;
            Last = wtDeadHeadFirstOrLast.Last;

        }


    }
}
