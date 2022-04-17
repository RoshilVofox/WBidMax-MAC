using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
   public class VacationStateTrip
   {
       [ProtoMember(1)]
       public string TripName { get; set; }

       [ProtoMember(2)]
       public string TripType { get; set; }

       [ProtoMember(3)]
       public DateTime TripActualStartDate { get; set; }
       [ProtoMember(4)]
       public DateTime TripVacationStartDate { get; set; }
       [ProtoMember(5)]
       public List<VacationStateDutyPeriod> VacationDutyPeriods { get; set; }



   }
}

