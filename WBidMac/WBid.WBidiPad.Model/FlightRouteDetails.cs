using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class FlightRouteDetails
    {

        #region Properties

        public Int32 FlightId { get; set; }

        public Int32 Flight { get; set; }

        public string Orig { get; set; }

        public string Dest { get; set; }

        public int Cdep { get; set; }

        public int Carr { get; set; }

        public int Ldep { get; set; }

        public int Larr { get; set; }

        public DateTime FlightDate { get; set; }

        public int RouteNum { get; set; }

        #endregion

    }
}
