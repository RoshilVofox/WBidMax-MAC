using System;
using ProtoBuf;
using System.Collections.Generic;
namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class FVVacationClass
    {
        public FVVacationClass()
        {
        }

    }
    [ProtoContract]
    public class FVVacationData
    {
        [ProtoMember(1)]
        public DateTime StartDate { get; set; }
        [ProtoMember(2)]
        public DateTime EndDate { get; set; }
        [ProtoMember(3)]
        public List<FVVacationTripData> FVVacationTripDatas { get; set; }
    }
    [ProtoContract]
    public class FVVacationTripData
    {
        [ProtoMember(1)]
        public string TripName { get; set; }
        [ProtoMember(2)]
        public decimal TripTfpInLine { get; set; }
        // public decimal TripTfpOutSideBp { get; set; }
        [ProtoMember(3)]
        public string Type { get; set; }
        [ProtoMember(4)]
        public DateTime TripStartDate { get; set; }
        [ProtoMember(5)]
        public DateTime TripEndDate { get; set; }
        [ProtoMember(6)]
        public int TripLegs { get; set; }

        //this property is used to store the drop tfp of a vacation if left and right side vacation comes
        //If a Floating vacation is in the middle of two regular vacations, the FV will not float.  The FV will be worth 3.75 x vac days (7) = 26.25 or trips dropped in that week.  Also, the first week will not have any VOB or VDB,  the third week will not have any VOF or VDF.

        //The first week can have VOF or VDF and the last(3rd) week can have VOB or VDB

        [ProtoMember(7)]
        public decimal VOFrontTfpDrop { get; set; }
        [ProtoMember(8)]
        public decimal VDFrontTfpDrop { get; set; }
        [ProtoMember(9)]
        public decimal VOBackTfpDrop { get; set; }
        [ProtoMember(10)]
        public decimal VDBackTfpDrop { get; set; }
        [ProtoMember(11)]
        public int VaVacationDropDays { get; set; }
    }
    [ProtoContract]
    public class FVvacationLineData
    {
        [ProtoMember(1)]
        public DateTime FVStartDate { get; set; }
        [ProtoMember(2)]
        public DateTime FVEndDate { get; set; }
        [ProtoMember(3)]
        public List<FVVacationTripData> FVVacationTripDatas { get; set; }
        [ProtoMember(4)]
        public DateTime FVVAPStarManetDate { get; set; }
        [ProtoMember(5)]
        public DateTime FVVAPEndDate { get; set; }
        [ProtoMember(6)]
        public decimal FVVacTfp { get; set; }
        [ProtoMember(7)]
        public decimal FVVap { get; set; }

        //this property is used to store the drop tfp of a vacation if left and right side vacation comes
        //If a Floating vacation is in the middle of two regular vacations, the FV will not float.  The FV will be worth 3.75 x vac days (7) = 26.25 or trips dropped in that week.  Also, the first week will not have any VOB or VDB,  the third week will not have any VOF or VDF.

        //The first week can have VOF or VDF and the last(3rd) week can have VOB or VDB
        [ProtoMember(8)]
        public decimal VOTfpDrop { get; set; }
        [ProtoMember(9)]
        public decimal VDTfpDrop { get; set; }

        [ProtoMember(10)]
        public int VaVacationDropDays { get; set; }

    }
    [ProtoContract]
    public class FVVAP
    {
        [ProtoMember(1)]
        public DateTime StartDate { get; set; }
        [ProtoMember(2)]
        public DateTime EndDate { get; set; }
        [ProtoMember(3)]
        public decimal Vap { get; set; }
    }
}
