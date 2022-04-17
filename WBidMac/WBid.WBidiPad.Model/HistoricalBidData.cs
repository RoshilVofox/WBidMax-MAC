using System;
using ProtoBuf;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{

//	[ProtoContract]
//	public class HistoricalBidDataList
//	{
//
//		[ProtoMember(1)]
//		public List<HistoricalBidData> HistoricalDataCollection { get; set; }
//
//	}



	[ProtoContract]
	public class HistoricalBidData
	{
		

		[ProtoMember(1)]
		public int Year { get; set; }

		[ProtoMember(2)]
		public int Month { get; set; }

		[ProtoMember(3)]
		public int Round { get; set; }

		[ProtoMember(4)]
		public string Domicile { get; set; }

		[ProtoMember(5)]
		public string Position { get; set; }
	}
}

