using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[Serializable]
	public class CxTripBlockLength
	{
		public CxTripBlockLength ()
		{
		} 
		[XmlAttribute("T")]
		public bool Turns { get; set; }
		[XmlAttribute("twod")]
		public bool Twoday { get; set; }
		[XmlAttribute("threed")]
		public bool ThreeDay { get; set; }
		[XmlAttribute("fourd")]
		public bool FourDay { get; set; }
		[XmlAttribute("IsBlock")]
		public bool IsBlock { get; set; }

	}
}

