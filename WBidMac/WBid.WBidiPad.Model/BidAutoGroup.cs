using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
	
	public class BidAutoGroup
	{
		[Serializable]
		public BidAutoGroup ()
		{
		}
		[XmlAttribute("GrpName")]
		public string GroupName { get; set; }

        [XmlElement("Lines")]
		public List<int> Lines { get; set; }
	}
}

