using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
   public  class MiscellaneousTab
    {

       [XmlAttribute("DataUpdate")]
       public bool DataUpdate { get; set; }

        [XmlAttribute("Coverletter")]
        public bool Coverletter { get; set; }


        [XmlAttribute("GatherData")]
        public bool GatherData { get; set; }

        [XmlAttribute("RetrieveMissingData")]
        public bool IsRetrieveMissingData { get; set; }
    }
}
