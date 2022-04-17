using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class WeightPoints
    {

        [ProtoMember(1)]
        public decimal AcftChanges { get; set; }

        [ProtoMember(2)]
        public decimal AmPmNte { get; set; }

        [ProtoMember(3)]
        public decimal BlkDaysOff { get; set; }

        [ProtoMember(4)]
        public decimal CmtDhds { get; set; }

        [ProtoMember(5)]
        public decimal CmtLines { get; set; }

        [ProtoMember(6)]
        public decimal DaysOfMonthOff { get; set; }

        [ProtoMember(7)]
        public decimal DaysOfWeekOff { get; set; }

        [ProtoMember(8)]
        public decimal DutyPeriod { get; set; }

        [ProtoMember(9)]
        public decimal EquipType { get; set; }

        [ProtoMember(10)]
        public decimal BlockTime { get; set; }

        [ProtoMember(11)]
        public decimal GrndTime { get; set; }

        [ProtoMember(12)]
        public decimal LegsPerDutPd { get; set; }

        [ProtoMember(13)]
        public decimal LegsPerTrip { get; set; }

        [ProtoMember(14)]
        public decimal NumDaysOff { get; set; }

        [ProtoMember(15)]
        public decimal OvernightCities { get; set; }

        [ProtoMember(16)]
        public decimal PartialDaysOff { get; set; }

        [ProtoMember(17)]
        public decimal StartDow { get; set; }

        [ProtoMember(18)]
        public decimal Rest { get; set; }

        [ProtoMember(19)]
        public decimal TripLength { get; set; }

        [ProtoMember(20)]
        public decimal WorkBlklength { get; set; }

        [ProtoMember(21)]
        public decimal TimeAwayFromBase { get; set; }

        [ProtoMember(22)]
        public decimal LrgBlkdaysOff { get; set; }

        [ProtoMember(23)]
        public decimal Position { get; set; }

        [ProtoMember(24)]
        public decimal DeadHeadFoL { get; set; }

        [ProtoMember(25)]
        public decimal InternationalConus { get; set; }

        [ProtoMember(26)]
        public decimal WorkDays { get; set; }

        [ProtoMember(27)]
        public decimal PartialDaysAfter { get; set; }

        [ProtoMember(28)]
        public decimal PartialDaysBefore { get; set; }

        [ProtoMember(29)]
        public decimal NormalizeDaysOff { get; set; }

        [ProtoMember(30)]
        public decimal OvernightCityBulk { get; set; }
		[ProtoMember(31)]
		public decimal CmtLinesAuto { get; set; }

		[ProtoMember(32)]
		public decimal CitiesLegs { get; set; }


		[ProtoMember(33)]
		public decimal Commute { get; set; }

        [ProtoMember(34)]
        public decimal ETOPS { get; set; }

        [ProtoMember(35)]
        public decimal ETOPSRes { get; set; }

        public decimal Total()
        {
            // \\ frank look here. Added LegsPerTrip property
          return  AcftChanges + AmPmNte + BlkDaysOff + DaysOfMonthOff + CmtDhds + CmtLines + LegsPerDutPd + NumDaysOff + LegsPerTrip
                  + DaysOfWeekOff + DutyPeriod + EquipType + BlockTime + GrndTime + TimeAwayFromBase + StartDow + OvernightCities + TripLength + Rest + PartialDaysOff + WorkBlklength
				+ LrgBlkdaysOff + Position + DeadHeadFoL + InternationalConus + WorkDays + PartialDaysAfter + PartialDaysBefore + NormalizeDaysOff + OvernightCityBulk+CmtLinesAuto+CitiesLegs+Commute+ ETOPS + ETOPSRes;
        }

        public void Reset()
        {

            AcftChanges = 0; ;

            AmPmNte = 0;

            BlkDaysOff = 0;

            CmtDhds = 0;

            CmtLines = 0;

            DaysOfMonthOff = 0;

            DaysOfWeekOff = 0;

            DutyPeriod = 0;

            EquipType = 0;

            ETOPS = 0;

            ETOPSRes = 0;

            BlockTime = 0;

            GrndTime = 0;

            LegsPerDutPd = 0;

            LegsPerTrip = 0;

            NumDaysOff = 0;

            OvernightCities = 0;

            PartialDaysOff = 0;

            StartDow = 0;

            Rest = 0;

            TripLength = 0;

            WorkBlklength = 0;

            TimeAwayFromBase = 0;

            LrgBlkdaysOff = 0;

            Position = 0;

            DeadHeadFoL = 0;

            InternationalConus = 0;
           

            WorkDays = 0;
            PartialDaysAfter = 0;
            PartialDaysBefore = 0;
            NormalizeDaysOff = 0;
            OvernightCityBulk = 0;
			CmtLinesAuto = 0;
			CitiesLegs = 0;
			Commute = 0;
        }
    }
}
