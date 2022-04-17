#region nameSpace
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class TripInfo
    {
         [ProtoMember(1)]
        public string TripVersion { get; set; }

         [ProtoMember(2)]
        public Dictionary<string, Trip> Trips { get; set; }

    }
}
