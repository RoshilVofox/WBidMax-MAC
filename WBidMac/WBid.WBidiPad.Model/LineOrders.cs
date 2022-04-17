using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [DataContract ]
  

    public class LineOrders
    {
        [DataMember]
        [XmlAttribute("Lines")]
        public int Lines { get; set; }


        [DataMember]
        public List<LineOrder> Orders { get; set; }

        public LineOrders()
        {
            Orders = new List<LineOrder>();

        }


        public LineOrders(LineOrders lineOrders)
        {
            Lines = lineOrders.Lines;

            if (lineOrders.Orders != null)
            {
                Orders = new List<LineOrder>();

                foreach (LineOrder lineObj in lineOrders.Orders)
                {
                    Orders.Add(new LineOrder(lineObj));
                }

            }



        }
    }

    [DataContract]
    public class LineOrder
    {

        /// <summary>
        /// PURPOSe : Order Id
        /// </summary>
        [DataMember]
        [XmlAttribute("OId")]
        public int OId { get; set; }

        /// <summary>
        /// PURPOSE : Line Id
        /// </summary>
        [DataMember]
        [XmlAttribute("LId")]
        public int LId { get; set; }

        public LineOrder()
        {
 
        }


        public LineOrder(LineOrder lineOrder)
        {
            OId = lineOrder.OId;
            LId = lineOrder.LId;
        }    


    }
}
