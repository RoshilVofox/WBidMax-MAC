using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace WBid.WBidiPad.Model
{
	[ProtoContract]
	public class MILData
	{
		[ProtoMember(1)]
		public string Version { get; set; }
		[ProtoMember(2)]
		public Dictionary<string,TripMultiMILData> MILValue { get; set; }
//		[ProtoMember(3)]
//		public List<Absense> MILDateList { get; set; }
	}
}
