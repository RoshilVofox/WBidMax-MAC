using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{ 
	[Serializable]
	public class FtCommutableLine
	{
		public FtCommutableLine ()
		{
			
		}
		public FtCommutableLine (FtCommutableLine ftCommutableLine)
		{
			if (ftCommutableLine != null) {
				ConnectTime = ftCommutableLine.ConnectTime;
				CommuteCity = ftCommutableLine.CommuteCity;
				City = ftCommutableLine.City;
				CheckInTime = ftCommutableLine.CheckInTime;
				BaseTime = ftCommutableLine.BaseTime;
				NoNights = ftCommutableLine.NoNights;
				ToWork = ftCommutableLine.ToWork;
				ToHome = ftCommutableLine.ToHome;
				IsNonStopOnly = ftCommutableLine.IsNonStopOnly;
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

		[XmlAttribute("IsNonStopOnly")]
		public bool IsNonStopOnly { get; set; }
	}
}

