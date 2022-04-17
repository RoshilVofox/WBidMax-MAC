using System;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[Serializable]
	public class CxLine
	{
		public CxLine ()
		{
		}
		[XmlElement("Hard")]
		public bool Hard { get; set; }


		/// <summary>
		/// PURPOSE : Blank
		/// </summary>
		[XmlElement("Blank")]
		public bool Blank { get; set; }


		/// <summary>
		/// PURPOSE : Reserve
		/// </summary>
		[XmlElement("Reserve")]
		public bool Reserve { get; set; }

		/// <summary>
		/// PURPOSE : Ready
		/// </summary>
		[XmlElement("Ready")]
		public bool Ready { get; set; }

		/// <summary>
		/// PURPOSE : International
		/// </summary>
		[XmlElement("International")]
		public bool International { get; set; }

		/// <summary>
		/// PURPOSE : NonConus
		/// </summary>
		[XmlElement("NonConus")]
		public bool NonConus { get; set; }
	}
}

