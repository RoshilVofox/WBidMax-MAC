using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[Serializable]
	public class CxDays
	{
		
		public CxDays ()
		{
		}
		[XmlAttribute("IsSun")]
		public bool  IsSun { get; set; }

		[XmlAttribute("IsMon")]
		public bool IsMon { get; set; }

		[XmlAttribute("IsTue")]
		public bool IsTue { get; set; }

		[XmlAttribute("IsWed")]
		public bool IsWed { get; set; }

		[XmlAttribute("IsThu")]
		public bool IsThu { get; set; }

		[XmlAttribute("IsFri")]
		public bool IsFri { get; set; }

		[XmlAttribute("IsSat")]
		public bool IsSat { get; set; }
	}
}

