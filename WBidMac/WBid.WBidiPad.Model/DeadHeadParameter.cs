using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class DeadHeadParameter
    {

        public DateTime Date { get; set; }

        public string DepSta { get; set; }

        public string ArrSta { get; set; }

        public int ConnectTime { get; set; }

        public int DepTimeHhMm { get; set; }

        public int ArrTimeHhMm { get; set; }

        public Flight RefFlt { get; set; }

    }
}
