using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBid.WBidiPad.SharedLibrary.Model
{
    [Serializable]
    public class SeniorityListMember
    {

        public string EmpNum { get; set; }

        public int DomicileSeniority { get; set; }

        public string Position { get; set; }

        public List<Absense> Absences { get; set; }

    }
    [Serializable]
    public class Absense
    {
        public string AbsenceString { get; set; }

        public string AbsenceType { get; set; }

        public DateTime StartAbsenceDate { get; set; }

        public DateTime EndAbsenceDate { get; set; }
    }
}
