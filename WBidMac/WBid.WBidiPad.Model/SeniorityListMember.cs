using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class SeniorityListMember
    {
        [ProtoMember(1)]
        public string EmpNum { get; set; }

        [ProtoMember(2)]
        public int DomicileSeniority { get; set; }

        [ProtoMember(3)]
        public string Position { get; set; }

        private List<Absense> _absences;
        [ProtoMember(4)]
        public List<Absense> Absences
        {
            get { return _absences; }
            set { _absences = value; }
        }
        [ProtoMember(5)]
        public string EBG { get; set; }

        [ProtoMember(6)]
        public string BidType { get; set; }


    }
     
    [ProtoContract]
    public class Absense
    {
		public Absense( )
		{
		}

		public Absense(Absense Absense)
		{
			AbsenceString = Absense.AbsenceString;
			AbsenceType = Absense.AbsenceType;
			StartAbsenceDate = Absense.StartAbsenceDate;
			EndAbsenceDate = Absense.EndAbsenceDate;
		}


        [ProtoMember(1)]
        public string AbsenceString { get; set; }

         [ProtoMember(2)]
        public string AbsenceType { get; set; }

         [ProtoMember(3)]
         public DateTime StartAbsenceDate { get; set; }

         [ProtoMember(4)]
        public DateTime EndAbsenceDate { get; set; }
    }
}
