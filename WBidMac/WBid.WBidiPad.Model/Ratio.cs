using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[DataContract]
	[Serializable]
	public class Ratio
	{
		public Ratio()
		{
		}

        [XmlAttribute("Numerator")]
		public int Numerator { get; set; }

		[XmlAttribute("Denominator")]
		public int Denominator { get; set; }

	}
}
