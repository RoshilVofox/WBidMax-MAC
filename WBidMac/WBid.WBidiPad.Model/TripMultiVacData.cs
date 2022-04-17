using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
   public  class TripMultiVacData
    {
         [ProtoMember(1)]
        public VacationTrip VaData { get; set; }
         [ProtoMember(2)]
        public VacationTrip VofData { get; set; }

         [ProtoMember(3)]
        public VacationTrip VobData { get; set; }
    }
}
