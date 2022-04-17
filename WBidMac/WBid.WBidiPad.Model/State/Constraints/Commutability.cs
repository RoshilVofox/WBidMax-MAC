using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	public class Commutability
	{
		public Commutability ()
		{
		}
		public Commutability(Commutability commutability)
		{
			if (commutability != null) {
				ConnectTime = commutability.ConnectTime;
				CommuteCity = commutability.CommuteCity;
				City = commutability.City;
				CheckInTime = commutability.CheckInTime;
				BaseTime = commutability.BaseTime;
				SecondcellValue = commutability.SecondcellValue;
				ThirdcellValue = commutability.ThirdcellValue;
				Type = commutability.Type;
				Value = commutability.Value;
				Weight = commutability.Weight;
			}
		}

		[XmlAttribute("Connect")]
		public int ConnectTime { get; set; }

		[XmlAttribute("Commute")]
		public int CommuteCity { get; set; }

		[XmlAttribute("City")]
		public string City { get; set; }

		[XmlAttribute("CheckIn")]
		public int CheckInTime { get; set; }

		[XmlAttribute("Base")]
		public int BaseTime { get; set; }

		[XmlAttribute("SecondCell")]
		public int SecondcellValue { get; set; }

		[XmlAttribute("ThirdCell")]
		public int ThirdcellValue { get; set; }

		[XmlAttribute("Type")]
		public int Type { get; set; }

		[XmlAttribute("Value")]
		public int Value { get; set; }

		[XmlAttribute("Weight")]
		public decimal Weight { get; set; }
	}
}

