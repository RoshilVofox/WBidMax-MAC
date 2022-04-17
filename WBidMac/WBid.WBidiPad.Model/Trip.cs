using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
     [ProtoContract]
    public class Trip
    {
        #region Construcotr

        public Trip()
        {
        }
        #endregion

        #region Properties
         
        private List<DutyPeriod> dutyPeriods = new List<DutyPeriod>();
        private List<Sip> sipsList = new List<Sip>();
        public string Date { get; set; }    // should be generally null since a Trip can be used on many dates

        // properties
        
          [ProtoMember(1)]
        public List<DutyPeriod> DutyPeriods
        {
            get { return dutyPeriods; }
            set { dutyPeriods = value; }
        }
          [ProtoMember(2)]
        public List<Sip> SipsList
        {
            get { return sipsList; }
            set { sipsList = value; }
        }
          [ProtoMember(3)]
        public string FreqCode { get; set; }
          [ProtoMember(4)]
        public string OpDays { get; set; }
          [ProtoMember(5)]
        public int PairLength { get; set; }
          [ProtoMember(6)]
        public string DepSta { get; set; }
          [ProtoMember(7)]
        public string DepTime { get; set; }
          [ProtoMember(8)]
        public string RetSta { get; set; }
          [ProtoMember(9)]
        public string RetTime { get; set; }
           [ProtoMember(10)]
        public string AmPm { get; set; }
           [ProtoMember(11)]
        public int Block { get; set; }
           [ProtoMember(12)]
        public int DutyTime { get; set; }
           [ProtoMember(13)]
        public int TotDutPer { get; set; }
           [ProtoMember(14)]
        public string DepFltNum { get; set; }
           [ProtoMember(15)]
        public string RetFltNum { get; set; }
           [ProtoMember(16)]
        public string PairClassCode { get; set; }
           [ProtoMember(17)]
        public int StartOp { get; set; }
           [ProtoMember(18)]
        public int EndOp { get; set; }
           [ProtoMember(19)]
        public string NonOpDays { get; set; }

           [ProtoMember(20)]
        public int DepTime1stDutPer { get; set; }
           [ProtoMember(21)]
        public int DepTimeLastDutper { get; set; }
           [ProtoMember(22)]
        public int BriefTime { get; set; }
           [ProtoMember(23)]
        public int DebriefTime { get; set; }
           [ProtoMember(24)]
        public decimal Tfp { get; set; }
           [ProtoMember(25)]
        public decimal CarryOverTfp { get; set; }      // carry over is the pay that falls in the next month from a trip that overlaps the end of the bid month to the next month
           [ProtoMember(26)]
        public decimal CarryOverBlock { get; set; }
           [ProtoMember(27)]
        public decimal TfpByTime { get; set; }
           [ProtoMember(28)]
        public decimal TfpByDistance { get; set; }
           [ProtoMember(29)]
        public decimal RigAdg { get; set; }
           [ProtoMember(30)]
        public decimal RigTafb { get; set; }
           [ProtoMember(31)]
        public decimal RigDailyMin { get; set; }
           [ProtoMember(32)]
        public decimal RigDhr { get; set; }
           [ProtoMember(33)]
        public decimal RigFlt { get; set; }
           [ProtoMember(34)]
        public decimal RigTotal { get; set; }
           [ProtoMember(35)]
        public int Tafb { get; set; }
           [ProtoMember(36)]
        public int TotalLegs { get; set; }
           [ProtoMember(37)]
        public int Sips { get; set; }           // Sip is when a trip passes through a domicile in the middle of a trip.
           [ProtoMember(38)]
        public bool ReserveTrip { get; set; }
           [ProtoMember(39)]
        public string TypeFaReserveTrip { get; set; }
        

       // public string OpVector { get; set; }
       // public string PositionString { get; set; }  // used for Flight Attendands -- all iterations of A,B,C,D
            [ProtoMember(40)]
        public string GtripStartDate { get; set; }
            [ProtoMember(41)]
        public string GtripEndDate { get; set; }
            [ProtoMember(42)]
        public string GtripCreateDate { get; set; }
            [ProtoMember(43)]
        public string GtripDomicile { get; set; }
            [ProtoMember(44)]
        public string GtripPositions { get; set; }
            [ProtoMember(45)]
        public string GtripOpVector { get; set; }
            [ProtoMember(46)]
        public List<DateTime> FAresTripOpDay { get; set; }
            [ProtoMember(47)]
        public bool DhFirst { get; set; }
            [ProtoMember(48)]
        public bool DhLast { get; set; }

            [ProtoMember(49)]
        public int FDP { get; set; }

         [ProtoMember(50)]
         public string TripNum { get; set; }

         [ProtoMember(51)]
         public bool International { get; set; }
         [ProtoMember(52)]
         public bool NonConus { get; set; }

         [ProtoMember(53)]
         public bool IsFromBidFile { get; set; }
        #endregion


    }
}
