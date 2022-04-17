using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace WBid.WBidiPad.Model
{
	[ProtoContract]
	public class TripMultiMILData
	{
		[ProtoMember(1)]
		public MILTrip MaData { get; set; }
		[ProtoMember(2)]
		public MILTrip MofData { get; set; }
		[ProtoMember(3)]
		public MILTrip MobData { get; set; }
		[ProtoMember(4)]
		public MILTrip MofbData { get; set; }



	}
	[ProtoContract]
	public class MILTrip
	{

		[ProtoMember(1)]
		public string TripName { get; set; }
		[ProtoMember(2)]
		public string MILType { get; set; }
		[ProtoMember(3)]
		public List<MILDutyPeriod> DutyPeriodsDetails { get; set; }
		[ProtoMember(4)]
		public bool IsDropped { get; set; }
		[ProtoMember(5)]
		public decimal Tfp { get; set; }

	}
	[ProtoContract]
	public class MILDutyPeriod
	{
		[ProtoMember(1)]
		public int DpSeqNum { get; set; }
		[ProtoMember(2)]
		public List<MILFlights> FlightDetails { get; set; }
		[ProtoMember(3)]
		public string Type { get; set; }
		[ProtoMember(4)]
		public bool IsinBidPeriod { get; set; }
		[ProtoMember(5)]
		public bool IsDropped { get; set; }
		[ProtoMember(6)]
		public decimal Tfp { get; set; }

	}

	[ProtoContract]
	public class MILFlights
	{
		[ProtoMember(1)]
		public int FlightNumber { get; set; }
		[ProtoMember(2)]
		public int FltSeqNum { get; set; }
		[ProtoMember(3)]
		public decimal Tfp { get; set; }
		[ProtoMember(4)]
		public string Type { get; set; }
		[ProtoMember(5)]
		public bool IsDropped { get; set; }
	}
}
