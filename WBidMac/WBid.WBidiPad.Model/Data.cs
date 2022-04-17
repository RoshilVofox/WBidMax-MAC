using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class Data
    {
        [XmlAttribute("IsCompanyData")]
        public bool IsCompanyData { get; set; }

    }
}
