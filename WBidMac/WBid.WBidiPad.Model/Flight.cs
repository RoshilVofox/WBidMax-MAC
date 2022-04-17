using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class Flight
    {
        #region properties
        [ProtoMember(1)]
        public int FltNum { get; set; }
        //  public string ReserveFltNum { get; set; }  // this is used for reserve flights as FltNum is an int and B1A is a string
        [ProtoMember(2)]
        public string DepType { get; set; }
        [ProtoMember(3)]
        public bool DeadHead { get; set; }
        [ProtoMember(4)]
        public bool DutyBreak { get; set; }
        [ProtoMember(5)]
        public string DepSta { get; set; }
        [ProtoMember(6)]
        public int DepTime { get; set; }
        [ProtoMember(7)]
        public string ArrSta { get; set; }
        [ProtoMember(8)]
        public int ArrTime { get; set; }
        [ProtoMember(9)]
        public string Equip { get; set; }
        [ProtoMember(10)]
        public bool AcftChange { get; set; }
        [ProtoMember(11)]
        public string ReserveTripType { get; set; }
        [ProtoMember(12)]
        public decimal RigFlt { get; set; }
        [ProtoMember(13)]
        public decimal Tfp { get; set; }
        [ProtoMember(14)]
        public decimal TfpByDistance { get; set; }
        [ProtoMember(15)]
        public decimal TfpByTime { get; set; }
        [ProtoMember(16)]
        public int Block { get; set; }                  // stored in minutes
        [ProtoMember(17)]
        public int TurnTime { get; set; }
        [ProtoMember(18)]
        public int FlightSeqNum { get; set; }
        [ProtoMember(19)]
        public string Reg { get; set; }

        [ProtoMember(20)]
        public bool ETOPS { get; set; }
        //  public int SipFltSeqNum { get; set; }
        #endregion




    }
}
