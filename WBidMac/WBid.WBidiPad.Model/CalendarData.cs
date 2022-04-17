#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model
{
    public  class CalendarData
    {


        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string  Day { get; set; }

        public string ArrStaLastLeg { get; set; }

        public string DepTimeFirstLeg { get; set; }

        public string LandTimeLastLeg { get; set; }

        public string TripNumber { get; set; }

        public bool IsEnabled { get; set; }

        public string ColorTop { get; set; }

        public string ColorBottom { get; set; }

        public bool IsTripStart { get; set; }

        public bool IsTripEnd{ get; set; }

		public bool IsLastTrip{ get; set; }



        public string DepTimeFirstLegDecoration { get; set; }

        public string ArrStaLastLegDecoration { get; set; }

        public string LandTimeLastLegDecoration { get; set; }
    }
}
