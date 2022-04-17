using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	[DataContract]
	[Serializable]
	public class MainWindowSize
	{
		/// <summary>
		/// PURPOSE:Width
		/// </summary>
		/// 
		[DataMember]
		[XmlAttribute("Width")]
		public int Width { get; set; }
		/// <summary>
		/// PURPOSE:HEight
		/// </summary>
		[DataMember]
		[XmlAttribute("Height")]
		public int Height { get; set; }
		/// <summary>
		/// PURPOSE:Left
		/// </summary>
		[DataMember]
		[XmlAttribute("Left")]
		public int Left { get; set; }
		/// <summary>
		/// PURPOSE:Top
		/// </summary>
		[DataMember]
		[XmlAttribute("Top")]
		public int Top { get; set; }

		/// <summary>
		/// PURPOSE:IsMaximised
		/// </summary>
		[DataMember]
		[XmlAttribute("IsMaximised")]
		public bool IsMaximised { get; set; }
	}
}

