using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class SourceHotel
    {

        //Id=1,Name="Use Only the Company Hotel List"
        //Id=2,Name="Try Company List. If not found,use WBid List"
        //Id=3,Name="Use only the WBid Hotel List"

        [XmlAttribute("SourceType")]
        public int SourceType { get; set; }
    }
}
