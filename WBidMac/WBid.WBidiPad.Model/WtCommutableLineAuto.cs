using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[Serializable]
	public class WtCommutableLineAuto
	{
		public WtCommutableLineAuto ()
		{
		}
		public WtCommutableLineAuto (WtCommutableLineAuto wtCommutableLineAuto )
		{
			if (wtCommutableLineAuto != null) {
				ConnectTime = wtCommutableLineAuto.ConnectTime;
				CommuteCity = wtCommutableLineAuto.CommuteCity;
				City = wtCommutableLineAuto.City;
				CheckInTime = wtCommutableLineAuto.CheckInTime;
				BaseTime = wtCommutableLineAuto.BaseTime;
				NoNights = wtCommutableLineAuto.NoNights;
				ToWork = wtCommutableLineAuto.ToWork;
				ToHome = wtCommutableLineAuto.ToHome;
				Weight = wtCommutableLineAuto.Weight;
				IsNonStopOnly = wtCommutableLineAuto.IsNonStopOnly;
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

		[XmlAttribute("NoNights")]
		public bool NoNights { get; set; }

		[XmlAttribute("ToWork")]
		public bool ToWork { get; set; }

		[XmlAttribute("ToHome")]
		public bool ToHome { get; set; }

		[XmlAttribute("WT")]
		public decimal Weight { get; set; }

		[XmlAttribute("IsNonStopOnly")]
        public bool IsNonStopOnly { get; set; }





	}
}

