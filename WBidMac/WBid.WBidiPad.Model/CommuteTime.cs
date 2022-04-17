using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[Serializable]
	public class CommuteTime
	{
		public CommuteTime ()
		{
		}
		[XmlAttribute("Day")]
		public DateTime BidDay { get; set; }

		[XmlAttribute("Arv")]
		public DateTime EarliestArrivel { get; set; }

		[XmlAttribute("Dpt")]
		public DateTime LatestDeparture { get; set; }

	}
}

