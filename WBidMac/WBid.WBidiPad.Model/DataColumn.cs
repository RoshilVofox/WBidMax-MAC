using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    //public class DataColumns:List<DataColumn>
    //{

    //}
    public class DataColumn
    {

        /// <summary>
        /// Column Id
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Column Code
        /// </summary>
        [XmlAttribute("Order")]
        public int Order { get; set; }

        /// <summary>
        /// Column Wdth
        /// </summary>
        [XmlAttribute("Width")]
        public int Width { get; set; }


    }
}
