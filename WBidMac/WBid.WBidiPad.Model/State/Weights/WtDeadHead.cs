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
     [Serializable]
    public class WtDeadHead
    {

        /// <summary>
        /// PURPOSE : Type id  like botth ends,begining only etc
        /// </summary>
        [XmlAttribute("TypeId")]
        public int TypeId { get; set; }

        /// <summary>
        /// PURPOSE : Select
        /// </summary>
        [XmlAttribute("CityId")]
        public int CityId { get; set; }

        /// <summary>
        /// PURPOSE : Weight
        /// </summary>
          [XmlAttribute("Weight")]
        public decimal Weight { get; set; }

          public WtDeadHead()
          {

          }

          public WtDeadHead(WtDeadHead wtDeadHead)
          {
              TypeId = wtDeadHead.TypeId;
              CityId = wtDeadHead.CityId;
              Weight = wtDeadHead.Weight;
          }


    }
}
