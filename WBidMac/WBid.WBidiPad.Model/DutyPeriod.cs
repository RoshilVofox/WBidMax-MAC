using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class DutyPeriod
    {
        #region Constructor
        // constructor
        public DutyPeriod()
        {

        }
        #endregion

        #region Properties

        private List<Flight> flights = new List<Flight>();
        [ProtoMember(1)]
        public string TripNum { get; set; }
        [ProtoMember(2)]
        public int DutPerSeqNum { get; set; }
        [ProtoMember(3)]
        public string ArrStaLastLeg { get; set; }
        [ProtoMember(4)]
        public int ShowTime { get; set; }
        [ProtoMember(5)]
        public int ReleaseTime { get; set; }
        [ProtoMember(6)]
        public decimal Tfp { get; set; }
        [ProtoMember(7)]
        public decimal TfpByTime { get; set; }
        [ProtoMember(8)]
        public decimal TfpByDistance { get; set; }
        [ProtoMember(9)]
        public decimal TfpByDutyHrs { get; set; }
        [ProtoMember(10)]
        public decimal RigDailyMin { get; set; }
        [ProtoMember(11)]
        public decimal RigDhr { get; set; }
        [ProtoMember(12)]
        public decimal RigFlt { get; set; }
        [ProtoMember(13)]
        public decimal RigTotal { get; set; }
        [ProtoMember(14)]
        public int Block { get; set; }
        [ProtoMember(15)]
        public int TotFlights { get; set; }
        [ProtoMember(16)]
        public List<Flight> Flights
        {
            get { return flights; }
            set { flights = value; }
        }
        [ProtoMember(17)]
        public int DutyTime { get; set; }
        [ProtoMember(18)]
        public int DutyBreak { get; set; }
        [ProtoMember(19)]
        public int DepTimeFirstLeg { get; set; }
        [ProtoMember(20)]
        public int LandTimeLastLeg { get; set; }

        [ProtoMember(21)]
        public int ReserveOut { get; set; }     // stored as minutes
        [ProtoMember(22)]
        public int ReserveIn { get; set; }        // stored as minutes
        [ProtoMember(23)]
        public int FDP { get; set; }
        [ProtoMember(24)]
        public bool ArrivesAfterMidnightDayBeforeVac { get; set; }


        #endregion
    }
}
