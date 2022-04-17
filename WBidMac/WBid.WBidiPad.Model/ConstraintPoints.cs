#region NameSpace
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class ConstraintPoints
    {
        [ProtoMember(1)]
        public bool AcftChanges { get; set; }

        [ProtoMember(2)]
        public bool AmPmNte { get; set; }

        [ProtoMember(3)]
        public bool BlkDaysOff { get; set; }
      
        [ProtoMember(4)]
        public bool CmtDhds { get; set; }
        
        [ProtoMember(5)]
        public bool CmtLines { get; set; }

        [ProtoMember(6)]
        public bool DaysOfMonthOff { get; set; }

        [ProtoMember(7)]
        public bool DaysOfWeekOff { get; set; }

        [ProtoMember(8)]
        public bool DutyPeriod { get; set; }

        [ProtoMember(9)]
        public bool EquipType { get; set; }

        [ProtoMember(10)]
        public bool BlockTime { get; set; }

        [ProtoMember(11)]
        public bool GrndTime { get; set; }

        [ProtoMember(12)]
        public bool LegsPerDutPd { get; set; }

        [ProtoMember(13)]
        public bool LegsPerTrip { get; set; }

        [ProtoMember(14)]
        public bool NumDaysOff { get; set; }

        [ProtoMember(15)]
        public bool OvernightCities { get; set; }

        [ProtoMember(16)]
        public bool PartialDaysOff { get; set; }

        [ProtoMember(17)]
        public bool StartDow { get; set; }

        [ProtoMember(18)]
        public bool Rest { get; set; }

        [ProtoMember(19)]
        public bool TripLength { get; set; }

        [ProtoMember(20)]
        public bool WorkBlklength { get; set; }

        [ProtoMember(21)]
        public bool TimeAwayFromBase { get; set; }

        [ProtoMember(22)]
        public bool MinimumPay { get; set; }

        [ProtoMember(23)]
        public bool NoOverLap { get; set; }

        [ProtoMember(24)]
        public bool No3On3off { get; set; }

        [ProtoMember(25)]
        public bool LrgBlkDayOf { get; set; }

        [ProtoMember(26)]
        public bool Positions { get; set; }

        [ProtoMember(27)]
        public bool Deadhead { get; set; }

        [ProtoMember(28)]
        public bool HardConstraint { get; set; }

        [ProtoMember(29)]
        public bool BlankConstraint { get; set; }

        [ProtoMember(30)]
        public bool ReserveConstraint { get; set; }

        [ProtoMember(31)]
        public bool ReadyReserveConstraint { get; set; }

        [ProtoMember(32)]
        public bool InternationalConstraint { get; set; }

        [ProtoMember(33)]
        public bool NonConusConstraint { get; set; }

        [ProtoMember(34)]
        public bool WeekDayConstraint { get; set; }

        [ProtoMember(35)]
        public bool WorkDay { get; set; }

         [ProtoMember(36)]
        public bool InterNonConus { get; set; }


         [ProtoMember(37)]
         public bool OvernightCityBulk { get; set; }

         [ProtoMember(38)]
         public bool TripLengthHard { get; set; }

		[ProtoMember(39)]
		public bool CmtLinesAuto { get; set; }

		[ProtoMember(40)]
		public bool CitiesLegs { get; set; }

		[ProtoMember(41)]
		public bool Commute { get; set; }

        [ProtoMember(42)]
        public bool ETOPS { get; set; }

        [ProtoMember(43)]
        public bool AdavanceETOPSConstraint { get; set; }

        [ProtoMember(44)]
        public bool ReserveETOPSConstraint { get; set; }

        [ProtoMember(45)]
        public bool StartDay { get; set; }

        [ProtoMember(46)]
        public bool ReportRelease { get; set; }

        [ProtoMember(47)]
        public bool MixedHardandReserve { get; set; }
        public bool IsConstraint()
        {
			return AcftChanges || AmPmNte || BlkDaysOff || DaysOfMonthOff || CmtDhds || CmtLines || LegsPerDutPd || LegsPerTrip || NumDaysOff
			|| DaysOfWeekOff || DutyPeriod || EquipType || BlockTime || GrndTime || TimeAwayFromBase || TripLength || MinimumPay || OvernightCities || NoOverLap || Rest || StartDow
			|| PartialDaysOff || No3On3off || LrgBlkDayOf || Positions || Deadhead
			|| HardConstraint || BlankConstraint || ReserveConstraint || ReadyReserveConstraint
                || InternationalConstraint || NonConusConstraint || WorkBlklength || WeekDayConstraint || WorkDay || InterNonConus || OvernightCityBulk || TripLengthHard || CmtLinesAuto||CitiesLegs||Commute|| ETOPS || AdavanceETOPSConstraint || ReserveETOPSConstraint || StartDay || ReportRelease
                || MixedHardandReserve;
        }

        public void Reset()
        {

            AcftChanges = false;

            AmPmNte = false;

            BlkDaysOff = false;

            CmtDhds = false;

            CmtLines = false;

			CmtLinesAuto = false;

            DaysOfMonthOff = false;

            DaysOfWeekOff = false;

            DutyPeriod = false;

            EquipType = false;

            BlockTime = false;

            GrndTime = false;

            LegsPerDutPd = false;

            LegsPerTrip = false;

            NumDaysOff = false;

            OvernightCities = false;

            PartialDaysOff = false;

            StartDow = false;

            Rest = false;

            TripLength = false;

            WorkBlklength = false;

            TimeAwayFromBase = false;

            MinimumPay = false;

            NoOverLap = false;

            Positions = false;

            HardConstraint = false;
            BlankConstraint = false;
            ReserveConstraint = false;
            ReadyReserveConstraint = false;
            InternationalConstraint = false;
            NonConusConstraint = false;
            WeekDayConstraint = false;
            WorkDay = false;
            InterNonConus = false;
            OvernightCityBulk = false;
            TripLengthHard = false;
			CitiesLegs = false;
			Commute = false;
            ETOPS = false;
            AdavanceETOPSConstraint = false;
            ReserveETOPSConstraint = false;
            StartDay = false;
            ReportRelease = false;
            MixedHardandReserve = false;
        }
    }
}
