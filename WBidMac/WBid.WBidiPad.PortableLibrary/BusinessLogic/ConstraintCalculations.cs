#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
//using WBid.WBidiPad.Model.State.Constraints;

#endregion
namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class ConstraintCalculations
    {


        #region Public Methods

        public void ApplyAllConstraints()
        {
            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            ClearAllConstraintCalclculation();


            if (wBidStateContent.Constraints.Hard)
            {
                HardConstraintCalclculation(true);
            }
            if (wBidStateContent.Constraints.Ready)
            {
                ReadyReserveConstraintCalculation(true);
            }
            if (wBidStateContent.Constraints.Reserve)
            {
                ReserveConstraintCalculation(true);
            }
            if (wBidStateContent.Constraints.Blank)
            {
                BlankConstraintCalculation(true);
            }
            if (wBidStateContent.Constraints.International)
            {
                InternationalConstraintCalculation(true);
            }
            if (wBidStateContent.Constraints.NonConus)
            {
                Non_ConusConstraintClaculation(true);
            }
            if (wBidStateContent.Constraints.ETOPS)
            {
                ETOPSConstraintClaculation(true);
            }
            if (wBidStateContent.Constraints.ReserveETOPS)
            {
                ETOPSResConstraintClaculation(true);
            }

            if (wBidStateContent.CxWtState.AMPMMIX.AM)
            {
                AMPMMixConstraint("AM", true);
            }

            if (wBidStateContent.CxWtState.AMPMMIX.PM)
            {
                AMPMMixConstraint(" PM", true);
            }
            if (wBidStateContent.CxWtState.AMPMMIX.MIX)
            {
                AMPMMixConstraint("Mix", true);
            }

            //			if (wBidStateContent.CxWtState.FaPosition.A)
            //			{
            //				ABCDPositionsConstraint("A", true);
            //			}
            //			if (wBidStateContent.CxWtState.FaPosition.B)
            //			{
            //				ABCDPositionsConstraint("B", true);
            //			}
            //			if (wBidStateContent.CxWtState.FaPosition.C)
            //			{
            //				ABCDPositionsConstraint("C", true);
            //			}
            //			if (wBidStateContent.CxWtState.FaPosition.D)
            //			{
            //				ABCDPositionsConstraint("D", true);
            //			}

            var faposition = wBidStateContent.CxWtState.FaPosition;
            ABCDPositionsConstraint(faposition.A, faposition.B, faposition.C, faposition.D);

            string tripLength = string.Empty;
            tripLength = (wBidStateContent.CxWtState.TripLength.Turns) ? "1," : "";
            tripLength += (wBidStateContent.CxWtState.TripLength.Twoday) ? "2," : "";
            tripLength += (wBidStateContent.CxWtState.TripLength.ThreeDay) ? "3," : "";
            tripLength += (wBidStateContent.CxWtState.TripLength.FourDay) ? "4," : "";

            if (tripLength != string.Empty)
            {
                tripLength = tripLength.Substring(0, tripLength.Length - 1);
                ApplyTripLengthConstraint(tripLength);
            }


            List<int> weekDays = new List<int>();
            if (wBidStateContent.CxWtState.DaysOfWeek.MON)
                weekDays.Add(0);
            if (wBidStateContent.CxWtState.DaysOfWeek.TUE)
                weekDays.Add(1);
            if (wBidStateContent.CxWtState.DaysOfWeek.WED)
                weekDays.Add(2);
            if (wBidStateContent.CxWtState.DaysOfWeek.THU)
                weekDays.Add(3);
            if (wBidStateContent.CxWtState.DaysOfWeek.FRI)
                weekDays.Add(4);
            if (wBidStateContent.CxWtState.DaysOfWeek.SAT)
                weekDays.Add(5);
            if (wBidStateContent.CxWtState.DaysOfWeek.SUN)
                weekDays.Add(6);

            if (weekDays.Count > 0)
            {
                ApplyWeekDayConstraint(weekDays);
            }

            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);

            //Days of Month
            //--------------------------------------------------------------------
            DaysOfMonthCx daysOfMonthCx = wBidStateContent.Constraints.DaysOfMonth;
            List<DateTime> workDays = null;
            List<DateTime> offDays = null;
            if (daysOfMonthCx != null)
            {
                List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();

                DayOfMonth result = null;
                //  if (daysOfMonthCx.Work)
                //  {
                workDays = new List<DateTime>();
                if (daysOfMonthCx.WorkDays != null)
                {
                    foreach (int id in daysOfMonthCx.WorkDays)
                    {
                        result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
                        if (result != null)
                            workDays.Add(result.Date);

                    }
                }

                //}

                //  if (daysOfMonthCx.Off)
                // {
                offDays = new List<DateTime>();
                if (daysOfMonthCx.OFFDays != null)
                {
                    foreach (int id in daysOfMonthCx.OFFDays)
                    {
                        result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
                        if (result != null)
                            offDays.Add(result.Date);

                    }
                }
            }
            else
            {
                workDays = new List<DateTime>();
                offDays = new List<DateTime>();
            }
            // }

            //-------------------------------------------------------------------------------



            //Partial Days off --Generate calendar
            //-------------------------------------------------------------------------------
            //List<DateHelper> lstDateHelper = new List<DateHelper>();
            //int dayCount = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
            //DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            //for (int count = 1; count <= dayCount; count++)
            //{
            //    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
            //    startDate = startDate.AddDays(1);
            //}
            //for (int count = dayCount + 1, i = 1; count <= 34; count++, i++)
            //{
            //    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
            //    startDate = startDate.AddDays(1);
            //}

            List<DateHelper> lstDateHelper = ConstraintBL.GetPartialDayList();
            //-----------------------------------------------------------------------------------

            if (wBidStateContent.CxWtState.Commute == null)
                wBidStateContent.CxWtState.Commute = new StateStatus { Cx = false, Wt = false };
           

            if (wBidStateContent.CxWtState.CLAuto == null)
                wBidStateContent.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
            if (wBidStateContent.CxWtState.CLAuto.Cx || wBidStateContent.CxWtState.CLAuto.Wt || wBidStateContent.CxWtState.Commute.Cx || wBidStateContent.CxWtState.Commute.Wt || (wBidStateContent.SortDetails.BlokSort.Contains("33") || wBidStateContent.SortDetails.BlokSort.Contains("34") || wBidStateContent.SortDetails.BlokSort.Contains("35")))
            {
                CalculateLineProperties lineproprty = new CalculateLineProperties();
                lineproprty.CalculateCommuteLineProperties(wBidStateContent);
            }
            foreach (Line line in requiredLines)
            {
                //---------------------------------------------------------------------------------------------
                //Aircraft Changes
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.AcftChanges = (wBidStateContent.CxWtState.ACChg.Cx) ? AirCraftChangesConstraint(wBidStateContent.Constraints.AircraftChanges, line) : false;


                //---------------------------------------------------------------------------------------------
                //BlockOfDaysOffConstraint DaysOff
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.BlkDaysOff = (wBidStateContent.CxWtState.BDO.Cx) ? BlockOfDaysOffConstraint(wBidStateContent.Constraints.BlockOfDaysOff, line) : false;

                //---------------------------------------------------------------------------------------------
                //  Cmut DHs
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.CmtDhds = (wBidStateContent.CxWtState.DHD.Cx) ? CommutableDeadHeadConstraint(wBidStateContent.Constraints.DeadHeads.LstParameter, line) : false;

                //---------------------------------------------------------------------------------------------
                //  Commutable Lines
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.CmtLines = (wBidStateContent.CxWtState.CL.Cx) ? CommutableLineConstraint(wBidStateContent.Constraints.CL, line) : false;

                //Commutable Line Auto Constraint
                //------------------------------------------------
                if (wBidStateContent.CxWtState.CLAuto == null)
                    wBidStateContent.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
                if (wBidStateContent.Constraints.CLAuto != null)
                    line.ConstraintPoints.CmtLinesAuto = (wBidStateContent.CxWtState.CLAuto.Cx) ? CalculateCommutableLineAutoConstraint(wBidStateContent.Constraints.CLAuto, line) : false;
                else
                    line.ConstraintPoints.CmtLinesAuto = false;


                line.ConstraintPoints.Commute = (wBidStateContent.CxWtState.Commute.Cx) ? CalculateCommuttabilityConstraint(wBidStateContent.Constraints.Commute, line) : false;

                //---------------------------------------------------------------------------------------------
                //Daysof week
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.DaysOfWeekOff = (wBidStateContent.CxWtState.DOW.Cx) ? DaysofWeekConstraint(wBidStateContent.Constraints.DaysOfWeek.lstParameters, line) : false;


                //---------------------------------------------------------------------------------------------
                //  Days of the Month
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.DaysOfMonthOff = (wBidStateContent.CxWtState.SDO.Cx) ? DaysOfMonthConstraint(workDays, offDays, line) : false;

                //---------------------------------------------------------------------------------------------
                //  DH - first - last
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.Deadhead = (wBidStateContent.CxWtState.DHDFoL.Cx) ? DeadHeadConstraint(wBidStateContent.Constraints.DeadHeadFoL.lstParameters, line) : false;

                //---------------------------------------------------------------------------------------------
                //Dutyperiod  DaysOff
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.DutyPeriod = (wBidStateContent.CxWtState.DP.Cx) ? DutyPeriodConstraint(wBidStateContent.Constraints.DutyPeriod, line) : false;



                //---------------------------------------------------------------------------------------------
                //  Equipment
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.EquipType = (wBidStateContent.CxWtState.EQUIP.Cx) ? EquipmentTypeConstraints(wBidStateContent.Constraints.EQUIP.lstParameters, line) : false;


                //---------------------------------------------------------------------------------------------
                // Flight Time
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.BlockTime = (wBidStateContent.CxWtState.FLTMIN.Cx) ? FlightTimeConstraint(wBidStateContent.Constraints.FlightMin, line) : false;

                //---------------------------------------------------------------------------------------------
                // GroundTime
                //---------------------------------------------------------------------------------------------
                if (!line.ReserveLine)
                {
                    line.ConstraintPoints.GrndTime = (wBidStateContent.CxWtState.GRD.Cx) ? GrounTimeConstraint(wBidStateContent.Constraints.GroundTime, line) : false;
                }

                //---------------------------------------------------------------------------------------------
                //International-Nonconus
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.InterNonConus = (wBidStateContent.CxWtState.InterConus.Cx) ? InternationalNonConusConstraint(wBidStateContent.Constraints.InterConus.lstParameters, line) : false;

                //---------------------------------------------------------------------------------------------
                // Legs per dutyperiod
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.LegsPerDutPd = (wBidStateContent.CxWtState.LEGS.Cx) ? LegsPerDutyPeriodConstraint(wBidStateContent.Constraints.LegsPerDutyPeriod, line) : false;



                //---------------------------------------------------------------------------------------------
                // Legs per pairing
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.LegsPerTrip = (wBidStateContent.CxWtState.LegsPerPairing.Cx) ? LegsPerDutyPeriodConstraint(wBidStateContent.Constraints.LegsPerPairing, line) : false;


                //---------------------------------------------------------------------------------------------
                // Number of days Off
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.NumDaysOff = (wBidStateContent.CxWtState.NODO.Cx) ? NumberofDaysOffConstraint(wBidStateContent.Constraints.NumberOfDaysOff, line) : false;


                //---------------------------------------------------------------------------------------------
                // Overnight Cities
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.OvernightCities = (wBidStateContent.CxWtState.RON.Cx) ? OverNightCitiesConstraints(wBidStateContent.Constraints.OverNightCities.lstParameters, line) : false;
                //---------------------------------------------------------------------------------------------
                // CititesLegs
                //---------------------------------------------------------------------------------------------
                if (wBidStateContent.CxWtState.CitiesLegs == null)
                    wBidStateContent.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
                line.ConstraintPoints.CitiesLegs = (wBidStateContent.CxWtState.CitiesLegs.Cx) ? CitiesLegsConstraints(wBidStateContent.Constraints.CitiesLegs.lstParameters, line) : false;

                //---------------------------------------------------------------------------------------------
                // Partial days off
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.PartialDaysOff = (wBidStateContent.CxWtState.WtPDOFS.Cx) ? PartialDaysOffConstraint(wBidStateContent.Constraints.PDOFS.LstParameter, line, lstDateHelper) : false;

                //---------------------------------------------------------------------------------------------
                // Start day of week
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.StartDow = (wBidStateContent.CxWtState.SDOW.Cx) ? StartDayOfWeekConstraint(wBidStateContent.Constraints.StartDayOftheWeek.lstParameters, line) : false;
                //---------------------------------------------------------------------------------------------
                // Rest
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.Rest = (wBidStateContent.CxWtState.Rest.Cx) ? RestConstraint(wBidStateContent.Constraints.Rest.lstParameters, line) : false;

                //---------------------------------------------------------------------------------------------
                // Timeaway from base
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.TimeAwayFromBase = (wBidStateContent.CxWtState.PerDiem.Cx) ? TimeAwayFromBaseConstraint(wBidStateContent.Constraints.PerDiem, line) : false;


                //---------------------------------------------------------------------------------------------
                // Trip Length
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.TripLength = (wBidStateContent.CxWtState.TL.Cx) ? TripLengthConstraint(wBidStateContent.Constraints.TripLength.lstParameters, line) : false;


                //---------------------------------------------------------------------------------------------
                // Work Block length
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.WorkBlklength = (wBidStateContent.CxWtState.WB.Cx) ? WorkBlockLengthConstraint(wBidStateContent.Constraints.WorkBlockLength.lstParameters, line) : false;


                //---------------------------------------------------------------------------------------------
                // Work Days
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.WorkDay = (wBidStateContent.CxWtState.WorkDay.Cx) ? WorkDaysConstraint(wBidStateContent.Constraints.WorkDay, line) : false;



                //---------------------------------------------------------------------------------------------
                // Min Pay
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.MinimumPay = (wBidStateContent.CxWtState.MP.Cx) ? MinimumPayConstraint(wBidStateContent.Constraints.MinimumPay, line) : false;


                //---------------------------------------------------------------------------------------------
                // 3 on 3 off
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.No3On3off = (wBidStateContent.CxWtState.No3on3off.Cx) ? ThreeOn3offConstraint(wBidStateContent.Constraints.No3On3Off, line) : false;



                //---------------------------------------------------------------------------------------------
                // Start Day
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.StartDay = (wBidStateContent.CxWtState.StartDay.Cx) ? CalculateStartDayConstraint(wBidStateContent.Constraints.StartDay.lstParameters, line) : false;

                //---------------------------------------------------------------------------------------------
                // Report release
                //---------------------------------------------------------------------------------------------
                line.ConstraintPoints.ReportRelease = (wBidStateContent.CxWtState.ReportRelease.Cx) ? CalculateReportReleaseConstraint(wBidStateContent.Constraints.ReportRelease.lstParameters, line) : false;

                //---------------------------------------------------------------------------------------------
                // Overnight City
                //---------------------------------------------------------------------------------------------

                line.ConstraintPoints.OvernightCityBulk = (wBidStateContent.CxWtState.BulkOC.Cx) ? OvernightCitiesBulkCalculation(wBidStateContent.Constraints.BulkOvernightCity, line) : false;

                if (wBidStateContent.CxWtState.MixedHardReserveTrip == null)
                    wBidStateContent.CxWtState.MixedHardReserveTrip = new StateStatus { Cx = false, Wt = false };
               
                    line.ConstraintPoints.MixedHardandReserve = (wBidStateContent.CxWtState.MixedHardReserveTrip.Cx) ? CalculateMixedHardandReserveConstraint(line) : false;
                

                line.Constrained = line.ConstraintPoints.IsConstraint();




            }

        }



        public void ClearAllConstraintCalclculation()
        {

            foreach (Line line in GlobalSettings.Lines)
            {
                // line.ConstraintPoints.HardConstraint = isEnable;
                line.ConstraintPoints.Reset();
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        private void ETopsAdvancedCalulation(bool hard, bool reserve, bool international, bool nonconus, bool blank)
        {

            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                foreach (var line in GlobalSettings.Lines)
                {
                    line.ConstraintPoints.AdavanceETOPSConstraint = false;
                    line.Constrained = line.ConstraintPoints.IsConstraint();
                }
                if (hard && blank && international && nonconus && !reserve)
                {
                    var lines = GlobalSettings.Lines.Where(x => !x.ReserveLine && x.ETOPS);
                    foreach (var line in lines)
                    {
                        line.ConstraintPoints.AdavanceETOPSConstraint = true;
                        line.Constrained = line.ConstraintPoints.IsConstraint();
                    }
                }
                else if (hard && blank && international && nonconus && reserve)
                {
                    var lines = GlobalSettings.Lines.Where(x => x.ReserveLine && !x.ETOPS);
                    foreach (var line in lines)
                    {
                        line.ConstraintPoints.AdavanceETOPSConstraint = true;
                        line.Constrained = line.ConstraintPoints.IsConstraint();
                    }
                }
            }
        }

        #region Hard Constraints
        /// <summary>
        /// Hard constraint calculation::Hard: any line that is not a “Reserve” or “Blank” or “Ready” is a hard line.  If this button is selected then all “Hard” lines will be constrained.
        /// </summary>
        public void HardConstraintCalclculation(bool isEnable)
        {
            var constraint = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints;
            ETopsAdvancedCalulation(isEnable, constraint.Reserve, constraint.International, constraint.NonConus, constraint.Blank);
            //var hardlines = GlobalSettings.Lines.Where(x => !x.ReserveLine && !x.BlankLine && !x.Pairings.Any(y => y.Substring(1, 1) == "R"));
            var hardlines = GlobalSettings.Lines.Where(x => !x.ReserveLine && !x.BlankLine && !x.Pairings.Any(y => y.Substring(1, 1) == "R" || y.Substring(1, 1) == "W" || y.Substring(1, 1) == "Y") && !x.International && !x.NonConus && !x.ETOPS);
            foreach (Line line in hardlines)
            {
                line.ConstraintPoints.HardConstraint = isEnable;
                line.Constrained = line.ConstraintPoints.IsConstraint();
            }

        }

        /// <summary>
        /// all Blank lines will be constrained.
        /// </summary>
        public void BlankConstraintCalculation(bool isEnable)
        {
            var constraint = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints;
            ETopsAdvancedCalulation(constraint.Hard, constraint.Reserve, constraint.International, constraint.NonConus, isEnable);
            var blanklines = GlobalSettings.Lines.Where(x => x.BlankLine);
            foreach (Line line in blanklines)
            {
                line.ConstraintPoints.BlankConstraint = isEnable;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        /// <summary>
        /// all reserve lines will be constrained
        /// </summary>
        public void ReserveConstraintCalculation(bool isEnable)
        {
            var constraint = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints;
            ETopsAdvancedCalulation(constraint.Hard, isEnable, constraint.International, constraint.NonConus, constraint.Blank);
            //var reservelines = GlobalSettings.Lines.Where(x => x.ReserveLine && !x.Pairings.Any(y => y.Substring(1, 1) == "R"));
            var reservelines = GlobalSettings.Lines.Where(x => x.ReserveLine && !x.BlankLine && !x.Pairings.Any(y => y.Substring(1, 1) == "R") && !x.International && !x.NonConus && !x.ETOPS);
            foreach (Line line in reservelines)
            {
                line.ConstraintPoints.ReserveConstraint = isEnable;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }


        /// all ready reserve lines will be constrained
        /// </summary>
        public void ReadyReserveConstraintCalculation(bool isEnable)
        {
            var readyReservelines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.Pairings.Any(y => y.Substring(1, 1) == "R"));
            foreach (Line line in readyReservelines)
            {
                line.ConstraintPoints.ReadyReserveConstraint = isEnable;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        /// <summary>
        /// all the international lines will be constrained
        /// </summary>
        public void InternationalConstraintCalculation(bool isEnable)
        {

            //			var internationallines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.International);
            //			foreach (Line line in internationallines)
            //			{
            //				line.ConstraintPoints.InternationalConstraint = isEnable;
            //				line.Constrained = line.ConstraintPoints.IsConstraint();
            //
            //			}
            var constraint = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints;
            ETopsAdvancedCalulation(constraint.Hard, constraint.Reserve, isEnable, constraint.NonConus, constraint.Blank);
            //var internationallines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            var internationallines = GlobalSettings.Lines.Where(x => !x.BlankLine && !x.ETOPS);
            foreach (Line line in internationallines)
            {

                line.ConstraintPoints.InternationalConstraint = (line.International && isEnable);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        /// <summary>
        /// All the non-conus lines will be constrained
        /// </summary>
        public void Non_ConusConstraintClaculation(bool isEnable)
        {

            var constraint = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints;
            ETopsAdvancedCalulation(constraint.Hard, constraint.Reserve, constraint.International, isEnable, constraint.Blank);
            //			var nonConuslines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.NonConus);
            //			foreach (Line line in nonConuslines)
            //			{
            //				line.ConstraintPoints.NonConusConstraint = isEnable;
            //				line.Constrained = line.ConstraintPoints.IsConstraint();
            //
            //			}
            //var nonConuslines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            var nonConuslines = GlobalSettings.Lines.Where(x => !x.BlankLine && !x.ETOPS);

            foreach (Line line in nonConuslines)
            {

                line.ConstraintPoints.NonConusConstraint = (line.NonConus && isEnable);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        /// <summary>
        /// All the ETOPS line will be constrained
        /// </summary>
        public void ETOPSConstraintClaculation(bool isEnable)
        {
            var constraint = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints;
            ETopsAdvancedCalulation(constraint.Hard, constraint.Reserve, constraint.International, constraint.NonConus, constraint.Blank);
            var etops = GlobalSettings.Lines.Where(x => !x.BlankLine && !x.ReserveLine);
            foreach (Line line in etops)
            {

                line.ConstraintPoints.ETOPS = (line.ETOPS && isEnable);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        public void ETOPSResConstraintClaculation(bool isEnable)
        {


            var constraint = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints;
            ETopsAdvancedCalulation(constraint.Hard, constraint.Reserve, constraint.International, constraint.NonConus, constraint.Blank);
            var etops = GlobalSettings.Lines.Where(x => x.ReserveLine);
            foreach (Line line in etops)
            {

                line.ConstraintPoints.ReserveETOPSConstraint = (line.ETOPS && isEnable);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        /// <summary>
        /// Am Pm Mix constraints
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isEnable"></param>
        public void AMPMMixConstraint(string type, bool isEnable)
        {

            var aMPMMixLines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.AMPM == type);
            foreach (Line line in aMPMMixLines)
            {
                line.ConstraintPoints.AmPmNte = isEnable;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }



        }

        /// <summary>
        /// AbCD position constraints
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isEnable"></param>
        public void ABCDPositionsConstraint(string type, bool isEnable)
        {

            var aBCDLines = GlobalSettings.Lines.Where(x => !x.BlankLine && x.FAPositions.Any(y => y == type));
            foreach (Line line in aBCDLines)
            {
                line.ConstraintPoints.Positions = isEnable;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }



        }
        /// <summary>
        /// AbCD position constraints
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isEnable"></param>
        public void ABCDPositionsConstraint(bool posA, bool posB, bool posC, bool posD)
        {
            List<string> positions = new List<string>();
            List<string> Linepositions = new List<string>();
            if (posA)
                positions.Add("A");
            if (posB)
                positions.Add("B");
            if (posC)
                positions.Add("C");
            if (posD)
                positions.Add("D");
            var requiredlines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredlines)
            {
                if (GlobalSettings.CurrentBidDetails.Round == "M")
                    Linepositions = line.FAPositions;
                else
                    Linepositions = line.FASecondRoundPositions.Values.ToList().Select(y => y.Substring(y.Length - 1, 1)).ToList();

                bool isPositionExist = Linepositions.Any(x => positions.Any(y => y == x));
                if (isPositionExist)
                {
                    line.ConstraintPoints.Positions = true;
                    line.Constrained = line.ConstraintPoints.IsConstraint();
                }
                else
                {
                    line.ConstraintPoints.Positions = false;
                    line.Constrained = line.ConstraintPoints.IsConstraint();
                }

            }

        }
        /// <summary>
        /// Trip Length constraint
        /// </summary>
        /// <param name="tripDays"></param>
        public void ApplyTripLengthConstraint(string tripDays)
        {

            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {

                TripLengthConstraint(tripDays, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();
            }



        }

        /// <summary>
        /// Calculatae Sunday,Monday,Tuesday,..Saturday button constraint--Placed in the top 4 line of constraint section
        /// Monday 0,tuesday 1...Sunday 6
        /// </summary>
        /// <param name="days"></param>
        public void ApplyWeekDayConstraint(List<int> days)
        {


            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                WeekDayConstraint(days, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        #endregion

        #region AirCraft Changes

        /// <summary>
        /// Aircraft Changes constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyAirCraftChangesConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.AcftChanges = AirCraftChangesConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }



        }

        /// <summary>
        /// Remove Aircraft changes constraints
        /// </summary>
        public void RemoveAirCraftChangesConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.AcftChanges);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.AcftChanges = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }



        }
        /// <summary>
        /// Remove Commutable line constraints
        /// </summary>
        public void RemoveCommutableLinesConstraint()
        {
            WBidState currentState =
              GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
                  x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.CmtLines);
            foreach (var line in GlobalSettings.Lines)
            {
                if (requiredLines.Contains(line))
                {
                    line.ConstraintPoints.CmtLines = false;
                    line.Constrained = line.ConstraintPoints.IsConstraint();

                }
                if (currentState.CxWtState.CL.Wt == false && !(currentState.SortDetails.BlokSort.Contains("36") || currentState.SortDetails.BlokSort.Contains("37") || currentState.SortDetails.BlokSort.Contains("38")))
                {
                    line.CommutableBacks = 0;
                    line.commutableFronts = 0;
                    line.CommutabilityFront = 0;
                    line.CommutabilityBack = 0;
                    line.CommutabilityOverall = 0;

                }
            }



        }
        #endregion

        #region Block Of Days Off

        /// <summary>
        /// Block of days off constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyBlockOfDaysOffConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.BlkDaysOff = BlockOfDaysOffConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        /// <summary>
        /// Remove Block Of DaysOff constraints
        /// </summary>
        public void RemoveBlockOfDaysOffConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.BlkDaysOff);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.BlkDaysOff = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Commutable deadHead

        /// <summary>
        /// Commuttable dead head
        /// </summary>
        /// <param name="lstCx4Parameter"></param>
        public void ApplyCommutableDeadHeadConstraint(List<Cx4Parameter> lstCx4Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.CmtDhds = CommutableDeadHeadConstraint(lstCx4Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        #endregion

        #region Commutable lines manual

        /// <summary>
        /// Commutable lines
        /// </summary>
        /// <param name="CxCommutableLine"></param>
        public void ApplyCommutableLinesConstraint(CxCommutableLine CxCommutableLine)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.CmtLines = CommutableLineConstraint(CxCommutableLine, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        #endregion

        #region Commutable lines auto
        /// <summary>
        /// Commutable lines
        /// </summary>
        /// <param name="CxCommutableLine"></param>
        public void ApplyCommutableLinesAutoConstraint(FtCommutableLine ftCommutableLine)
        {

            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.CmtLinesAuto = CalculateCommutableLineAutoConstraint(ftCommutableLine, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }
        // Remove Commutable line auto constraints
        /// </summary>
        public void RemoveCommutableLinesAutoConstraint()
        {

            WBidState currentState =
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
                    x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.CmtLinesAuto);
            foreach (var line in GlobalSettings.Lines)
            {
                if (requiredLines.Contains(line))
                {
                    line.ConstraintPoints.CmtLinesAuto = false;
                    line.Constrained = line.ConstraintPoints.IsConstraint();
                }

                if (currentState.CxWtState.CLAuto.Wt == false && !(currentState.SortDetails.BlokSort.Contains("33") || currentState.SortDetails.BlokSort.Contains("34") || currentState.SortDetails.BlokSort.Contains("35")))
                {
                    line.CommutableBacks = 0;
                    line.commutableFronts = 0;
                    line.CommutabilityFront = 0;
                    line.CommutabilityBack = 0;
                    line.CommutabilityOverall = 0;

                }
            }

        }
        #endregion
        #region Commuttability
        /// <summary>
        /// Commutablility
        /// </summary>
        /// <param name="CxCommutableLine"></param>
        public void ApplyCommuttabilityConstraint(Commutability ftCommutableLine)
        {

            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.Commute = CalculateCommuttabilityConstraint(ftCommutableLine, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        /// <summary>
        /// Remove Comutability constraints
        /// </summary>
        public void RemoveCommutabilityConstraint()
        {
            WBidState currentState =
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
                    x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.Commute);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.Commute = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();
                if (currentState.CxWtState.Commute.Wt == false)
                {
                    line.CommutableBacks = 0;
                    line.commutableFronts = 0;
                    line.CommutabilityFront = 0;
                    line.CommutabilityBack = 0;
                    line.CommutabilityOverall = 0;
                }
            }



        }
        #endregion

        public void ApplyStartDayConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.StartDay = CalculateStartDayConstraint(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        public void ApplyReportReleaseConstraint(List<ReportRelease> lstReportRelease)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.ReportRelease = CalculateReportReleaseConstraint(lstReportRelease, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #region Days of Week
        public void ApplyDaysofWeekConstraint(List<Cx3Parameter> lstcx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.DaysOfWeekOff = DaysofWeekConstraint(lstcx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();
            }
        }

        public void RemoveDaysofWeekConstraint()
        {

            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.DaysOfWeekOff);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.DaysOfWeekOff = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Days Of Month

        /// <summary>
        /// Days Of Month Constraint
        /// </summary>
        /// <param name="daysOfMonthCx"></param>
        public void ApplyDaysOfMonthConstraint(DaysOfMonthCx daysOfMonthCx)
        {

            List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();
            List<DateTime> workDays = null;
            List<DateTime> offDays = null;
            DayOfMonth result = null;
            //  if (daysOfMonthCx.Work)
            // {
            workDays = new List<DateTime>();
            foreach (int id in daysOfMonthCx.WorkDays)
            {
                result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
                if (result != null)
                    workDays.Add(result.Date);

            }
            // }

            // if (daysOfMonthCx.Off)
            //{
            offDays = new List<DateTime>();
            foreach (int id in daysOfMonthCx.OFFDays)
            {
                result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
                if (result != null)
                    offDays.Add(result.Date);

            }
            //}

            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.DaysOfMonthOff = DaysOfMonthConstraint(workDays, offDays, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveDaysOfMonthConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.DaysOfMonthOff);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.DaysOfMonthOff = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }
        #endregion

        #region DeadHead
        public void ApplyDeadHeadConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.Deadhead = DeadHeadConstraint(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Duty Period
        /// <summary>
        /// Block of days off constraint
        /// </summary>
        /// <param name="constraintParams"></param>
        public void ApplyDutyPeriodConstraint(Cx2Parameter Cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.DutyPeriod = DutyPeriodConstraint(Cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveDutyPeriodConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.DutyPeriod);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.DutyPeriod = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }
        #endregion

        #region Equip Type
        public void ApplyEquipmentTypeConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.EquipType = EquipmentTypeConstraints(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveEquipmentTypeConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.EquipType);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.EquipType = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }
        #endregion

        #region Flight Time

        /// <summary>
        /// Flight Time constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyFlightTimeConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.BlockTime = FlightTimeConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        public void RemoveFlightTimeConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.BlockTime);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.BlockTime = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        #endregion

        #region Ground Time
        public void ApplyGroundTimeConstraint(Cx3Parameter cx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine && !x.ReserveLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.GrndTime = GrounTimeConstraint(cx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        public void RemoveGroundTimeConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.GrndTime);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.GrndTime = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region International NonConus

        public void ApplyInternationalonConusConstraint(List<Cx2Parameter> lstCx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.InterNonConus = InternationalNonConusConstraint(lstCx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Legs Per DutyPeriod

        /// <summary>
        /// Legs per Duty period constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyLegsPerDutyPeriodConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.LegsPerDutPd = LegsPerDutyPeriodConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }


        }

        public void RemoveLegsPerDutyPeriodConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.LegsPerDutPd);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.LegsPerDutPd = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Legs Per Pairing

        /// <summary>
        /// Legs per Pairing constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyLegsPerPairingConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.LegsPerTrip = LegsPerPairingConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }


        }

        public void RemoveLegsPerPairingConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.LegsPerTrip);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.LegsPerTrip = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Number of Days Off
        /// <summary>
        /// Number of Days off Constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyNumberofDaysOffConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.NumDaysOff = NumberofDaysOffConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveNumberofDaysOffConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.NumDaysOff);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.NumDaysOff = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        #endregion

        #region Overnight Cities

        public void ApplyOvernightCitiesConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.OvernightCities = OverNightCitiesConstraints(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveOvernightCitiesConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.OvernightCities);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.OvernightCities = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        #endregion

        #region CitiesLegs

        public void ApplyCitiesLegsConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.CitiesLegs = CitiesLegsConstraints(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveCityLegsConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.CitiesLegs);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.CitiesLegs = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Partial days Off

        public void ApplyPartialdaysOffConstraint(List<Cx4Parameter> lstCx4Parameter)
        {

            //Generate calendar
            //----------------------------------------
            //List<DateHelper> lstDateHelper = new List<DateHelper>();
            //int dayCount = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
            //DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            //for (int count = 1; count <= dayCount; count++)
            //{
            //    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
            //    startDate = startDate.AddDays(1);
            //}
            //for (int count = dayCount + 1, i = 1; count <= 34; count++, i++)
            //{
            //    lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
            //    startDate = startDate.AddDays(1);
            //}

            List<DateHelper> lstDateHelper = ConstraintBL.GetPartialDayList();
            //----------------------------------------
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.PartialDaysOff = PartialDaysOffConstraint(lstCx4Parameter, line, lstDateHelper);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemovePartialdaysOffConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.PartialDaysOff);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.PartialDaysOff = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        #endregion

        #region Start day of week

        public void ApplyStartDayOfWeekConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.StartDow = StartDayOfWeekConstraint(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveStartDayOfWeekConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.StartDow);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.StartDow = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Rest

        public void ApplyRestConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.Rest = RestConstraint(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveRestConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.Rest);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.Rest = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region TimeAwayFromBaseConstraint

        /// <summary>
        /// Time Away From Base
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyTimeAwayFromBaseConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.TimeAwayFromBase = TimeAwayFromBaseConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveTimeAwayFromBaseConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.TimeAwayFromBase);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.TimeAwayFromBase = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region TripLength

        /// <summary>
        /// Trip Length
        /// </summary>
        /// <param name="lstCx3Parameter"></param>
        public void ApplyTripLengthConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.TripLength = TripLengthConstraint(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveTripLengthConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.TripLength);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.TripLength = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Work Block Length
        /// <summary>
        /// Work Block length constraint
        /// </summary>
        /// <param name="lstCx3Parameter"></param>
        public void ApplyWorkBlockLengthConstraint(List<Cx3Parameter> lstCx3Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.WorkBlklength = WorkBlockLengthConstraint(lstCx3Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        public void RemoveWorkBlockLengthConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.WorkBlklength);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.WorkBlklength = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region WorkDay Constraint
        /// <summary>
        /// Work Days Constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyWorkDaysConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.WorkDay = WorkDaysConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }



        }

        /// <summary>
        /// Remove Work Day constraint
        /// </summary>
        public void RemoveWorkDayConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.WorkDay);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.WorkDay = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region MinimumPay

        /// <summary>
        /// Miniimum pay  Constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyMinimumPayConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.MinimumPay = MinimumPayConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        /// <summary>
        /// Miniimum pay  Constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        public void ApplyOvernightBulkConstraint(BulkOvernightCityCx BulkOvernightCityCx)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            //if (BulkOvernightCityCx.OverNightYes.Count != 0 || BulkOvernightCityCx.OverNightNo.Count != 0)
            //{
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.OvernightCityBulk = OvernightCitiesBulkCalculation(BulkOvernightCityCx, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
            // }




        }
        public void RemoveMinimumPayConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.MinimumPay);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.MinimumPay = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }
        #endregion

        #region Three On 3 off
        public void ApplyThreeOn3offConstraint(Cx2Parameter cx2Parameter)
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.No3On3off = ThreeOn3offConstraint(cx2Parameter, line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }


        public void RemoveThreeOn3offConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.No3On3off);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.No3On3off = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }

        }

        #endregion

        #region Mixed Hard/Reserve Trip

        /// <summary>
        /// Mixed Hard/Reserve Trip constraint
        /// </summary>

        public void ApplyMixedHardandReserveConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.MixedHardandReserve = CalculateMixedHardandReserveConstraint(line);
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }
        }

        /// <summary>
        /// Remove Aircraft changes constraints
        /// </summary>
        public void RemoveMixedHardandReserveConstraint()
        {
            var requiredLines = GlobalSettings.Lines.Where(x => x.ConstraintPoints.MixedHardandReserve);
            foreach (Line line in requiredLines)
            {
                line.ConstraintPoints.MixedHardandReserve = false;
                line.Constrained = line.ConstraintPoints.IsConstraint();

            }



        }
        #endregion

        #region Display line details

        public string LinesNotConstrained()
        {
            return GlobalSettings.Lines.Where(x => !x.Constrained).Count().ToString() + " of " + GlobalSettings.Lines.Count();
        }
        #endregion

        #region Clear Constraints
        /// <summary>
        /// PURPOSE Clear Constraints
        /// </summary>
        public void ClearConstraints()
        {
            List<int> lstOff = new List<int>() { };

            List<int> lstWork = new List<int>() { };
            //var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
            foreach (Line line in GlobalSettings.Lines)
            {
                line.ConstraintPoints.Reset();
                line.Constrained = false;

            }

            WBidState currentState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            CxWtState states = currentState.CxWtState;

            currentState.Constraints.Hard = false;
            currentState.Constraints.Ready = false;
            currentState.Constraints.Reserve = false;
            currentState.Constraints.Blank = false;
            currentState.Constraints.International = false;
            currentState.Constraints.NonConus = false;
            currentState.Constraints.ETOPS = false;

            currentState.CxWtState.AMPMMIX.AM = false;
            currentState.CxWtState.AMPMMIX.PM = false;
            currentState.CxWtState.AMPMMIX.MIX = false;

            currentState.CxWtState.FaPosition.A = false;
            currentState.CxWtState.FaPosition.B = false;
            currentState.CxWtState.FaPosition.C = false;
            currentState.CxWtState.FaPosition.D = false;

            currentState.CxWtState.TripLength.Turns = false;
            currentState.CxWtState.TripLength.Twoday = false;
            currentState.CxWtState.TripLength.ThreeDay = false;
            currentState.CxWtState.TripLength.FourDay = false;

            currentState.CxWtState.DaysOfWeek.MON = false;
            currentState.CxWtState.DaysOfWeek.TUE = false;
            currentState.CxWtState.DaysOfWeek.WED = false;
            currentState.CxWtState.DaysOfWeek.THU = false;
            currentState.CxWtState.DaysOfWeek.FRI = false;
            currentState.CxWtState.DaysOfWeek.SAT = false;
            currentState.CxWtState.DaysOfWeek.SUN = false;



            states.ACChg.Cx = false;
            //states.AMPM.Cx = false;
            states.BDO.Cx = false;
            states.CL.Cx = false;
            states.CLAuto.Cx = false;
            states.DHD.Cx = false;
            states.DHDFoL.Cx = false;
            states.DOW.Cx = false;
            states.DP.Cx = false;
            states.EQUIP.Cx = false;
            states.FLTMIN.Cx = false;
            states.GRD.Cx = false;
            states.InterConus.Cx = false;
            states.LEGS.Cx = false;
            states.LegsPerPairing.Cx = false;
            states.MP.Cx = false;
            states.No3on3off.Cx = false;
            states.NODO.Cx = false;
            states.NOL.Cx = false;
            states.PerDiem.Cx = false;
            states.Rest.Cx = false;
            states.RON.Cx = false;
            states.SDO.Cx = false;
            states.SDOW.Cx = false;
            states.TL.Cx = false;
            states.WB.Cx = false;
            states.WtPDOFS.Cx = false;
            states.LrgBlkDaysOff.Cx = false;
            states.Position.Cx = false;
            states.WorkDay.Cx = false;
            states.BulkOC.Cx = false;
            states.CitiesLegs.Cx = false;
            states.Commute.Cx = false;
            states.StartDay.Cx = false;
            states.ReportRelease.Cx = false;
            states.MixedHardReserveTrip.Cx = false;
            // states.InterCon.Cx = false;
            //Update the state object
            Commutability commutecx;
            if (currentState.CxWtState.Commute.Wt || currentState.SortDetails.BlokSort.Contains("30") || currentState.SortDetails.BlokSort.Contains("31") || currentState.SortDetails.BlokSort.Contains("32"))
                commutecx = new Commutability
                {
                    City = currentState.Constraints.Commute.City,
                    BaseTime = currentState.Constraints.Commute.BaseTime,
                    ConnectTime = currentState.Constraints.Commute.ConnectTime,
                    CheckInTime = currentState.Constraints.Commute.CheckInTime,
                    SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
                    ThirdcellValue = (int)CommutabilityThirdCell.Overall,
                    Type = (int)ConstraintType.MoreThan,
                    Value = 100
                };
            else
                commutecx = new Commutability
                {
                    BaseTime = 10,
                    ConnectTime = 30,
                    CheckInTime = 60,
                    SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
                    ThirdcellValue = (int)CommutabilityThirdCell.Overall,
                    Type = (int)ConstraintType.MoreThan,
                    Value = 100
                };
            FtCommutableLine clauto;
            if (currentState.CxWtState.CLAuto.Wt || currentState.SortDetails.BlokSort.Contains("33") || currentState.SortDetails.BlokSort.Contains("34") || currentState.SortDetails.BlokSort.Contains("35"))
            {
                clauto = new FtCommutableLine()
                {
                    City = currentState.Constraints.CLAuto.City,
                    ToHome = currentState.Constraints.CLAuto.ToHome,
                    ToWork = currentState.Constraints.CLAuto.ToWork,
                    NoNights = currentState.Constraints.CLAuto.NoNights,
                    BaseTime = currentState.Constraints.CLAuto.BaseTime,
                    ConnectTime = currentState.Constraints.CLAuto.ConnectTime,
                    CheckInTime = currentState.Constraints.CLAuto.CheckInTime,
                    IsNonStopOnly = currentState.Constraints.CLAuto.IsNonStopOnly
                };
            }
            else
            {
                clauto = new FtCommutableLine()
                {
                    
                    ToHome = true,
                    ToWork = false,
                    NoNights = false,
                    BaseTime = 10,
                    ConnectTime = 30,
                    CheckInTime = 120
                };
            }
            Constraints constraint = new Constraints()
            {
                Hard = false,
                Ready = false,
                Reserve = false,
                International = false,
                NonConus = false,
                ETOPS = false,
                // AM_PM = new AMPMConstriants{AM=false,PM=false,MIX=false},
                LrgBlkDayOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 10 },
                AircraftChanges = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
                BlockOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },
                DeadHeads = new Cx4Parameters { SecondcellValue = "1", ThirdcellValue = ((int)DeadheadType.First).ToString(), Type = (int)ConstraintType.LessThan, Value = 1, LstParameter = new List<Cx4Parameter>() },
                CL = new CxCommutableLine()
                {
                    AnyNight = true,
                    RunBoth = false,
                    CommuteToHome = true,
                    CommuteToWork = true,
                    MondayThu = new Times() { Checkin = 0, BackToBase = 0 },
                    MondayThuDefault = new Times() { Checkin = 0, BackToBase = 0 },
                    Friday = new Times() { Checkin = 0, BackToBase = 0 },
                    FridayDefault = new Times() { Checkin = 0, BackToBase = 0 },
                    Saturday = new Times() { Checkin = 0, BackToBase = 0 },
                    SaturdayDefault = new Times() { Checkin = 0, BackToBase = 0 },
                    Sunday = new Times() { Checkin = 0, BackToBase = 0 },
                    SundayDefault = new Times() { Checkin = 0, BackToBase = 0 },
                    TimesList = new List<Times>()

                },
                Commute = commutecx,
                CLAuto = clauto,
                DaysOfMonth = new DaysOfMonthCx() { OFFDays = lstOff, WorkDays = lstWork },
                DaysOfWeek = new Cx3Parameters() { ThirdcellValue = ((int)Dow.Tue).ToString(), Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
                DeadHeadFoL = new Cx3Parameters { ThirdcellValue = ((int)DeadheadType.First).ToString(), Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
                DutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 600 },

                EQUIP = new Cx3Parameters { ThirdcellValue = "700", Type = (int)ConstraintType.MoreThan, Value = 0, lstParameters = new List<Cx3Parameter>() },
                FlightMin = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 7200 },
                GroundTime = new Cx3Parameter { Type = (int)ConstraintType.MoreThan, Value = 1, ThirdcellValue = "30" },
                InterConus = new Cx2Parameters() { Type = (int)CityType.International, Value = 1, lstParameters = new List<Cx2Parameter>() },
                LegsPerDutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
                LegsPerPairing = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18 },
                NumberOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },

                OverNightCities = new Cx3Parameters { ThirdcellValue = "6", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
                CitiesLegs = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
                BulkOvernightCity = new BulkOvernightCityCx { OverNightNo = new List<int>(), OverNightYes = new List<int>() },
                PDOFS = new Cx4Parameters { SecondcellValue = "1", ThirdcellValue = "1", Type = (int)ConstraintType.atafter, Value = 915, LstParameter = new List<Cx4Parameter>() },
                Position = new Cx3Parameters { Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
                StartDayOftheWeek = new Cx3Parameters { SecondcellValue = "1", ThirdcellValue = "6", Type = (int)ConstraintType.MoreThan, Value = 3, lstParameters = new List<Cx3Parameter>() },
                Rest = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 8, lstParameters = new List<Cx3Parameter>() },
                PerDiem = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18000 },
                TripLength = new Cx3Parameters { ThirdcellValue = "4", Type = (int)ConstraintType.MoreThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
                WorkBlockLength = new Cx3Parameters { ThirdcellValue = "4", Type = (int)ConstraintType.LessThan, Value = 2, lstParameters = new List<Cx3Parameter>() },
                MinimumPay = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 90 },
                No3On3Off = new Cx2Parameter { Type = (int)ThreeOnThreeOff.ThreeOnThreeOff, Value = 10 },
                WorkDay = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 11 },
                StartDay = new Cx3Parameters { ThirdcellValue = "1", Type = 1, Value = 1 },
                ReportRelease = new ReportReleases { AllDays = true, First = false, Last = false, NoMid = false, Report = 0, Release = 0 },
                MixedHardReserveTrip = false




            };
            if (constraint.Commute.City != null)
            {
                constraint.DailyCommuteTimesCmmutability = currentState.Constraints.DailyCommuteTimesCmmutability;
            }
            currentState.Constraints = constraint;

        }

        #endregion

        #endregion

        #region Private Methods

        #region Hard Constraints

        /// <summary>
        /// Trip length constraint
        /// </summary>
        /// <param name="tripDays"></param>
        /// <param name="line"></param>
        private void TripLengthConstraint(string tripDays, Line line)
        {
            bool status = false;


            foreach (string str in tripDays.Split(','))
            {
                switch (str)
                {
                    case "1":
                        if (line.Trips1Day > 0)
                        {
                            status = true;
                        }
                        break;

                    case "2":
                        if (line.Trips2Day > 0)
                        {
                            status = true;
                        }
                        break;

                    case "3":
                        if (line.Trips3Day > 0)
                        {
                            status = true;
                        }
                        break;
                    case "4":
                        if (line.Trips4Day > 0)
                        {
                            status = true;
                        }
                        break;



                }
                if (status)
                    break;
            }

            line.ConstraintPoints.TripLengthHard = status;


        }

        /// <summary>
        /// Single Week day  constraint
        /// </summary>
        /// <param name="days"></param>
        /// <param name="line"></param>
        private void WeekDayConstraint(List<int> days, Line line)
        {
            bool status = false;


            foreach (int day in days)
            {
                if (line.DaysOfWeekWork[day] != 0)
                {
                    status = true;
                    break;
                }
            }

            line.ConstraintPoints.WeekDayConstraint = status;


        }

        #endregion


        /// <summary>
        /// Single line Aircraft changes
        /// </summary>
        /// <param name="cx2Parameter"></param>
        /// <param name="line"></param>
        private bool AirCraftChangesConstraint(Cx2Parameter cx2Parameter, Line line)
        {
            bool status = false;
            if (cx2Parameter.Type == (int)ConstraintType.LessThan)
            {
                status = (line.AcftChanges < cx2Parameter.Value);

            }
            if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
            {
                status = (line.AcftChanges == cx2Parameter.Value);

            }
            else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
            {
                status = (line.AcftChanges > cx2Parameter.Value);
            }



            return status;

        }

        /// <summary>
        /// Block Of Days Off Constraint
        /// </summary>
        /// <param name="cx2Parameter"></param>
        /// <param name="line"></param>
        private bool BlockOfDaysOffConstraint(Cx2Parameter cx2Parameter, Line line)
        {
            //if (line.LineNum == 3)
            //{
            //}
            //Type--Less than,equal to,More Than
            //Value= Number of days off
            bool status = false;
            if (cx2Parameter.Type == (int)ConstraintType.LessThan)
            {

                if ((line.BlkOfDaysOff.Take(cx2Parameter.Value).Where(x => x != 0).Count() > 0) && (line.BlkOfDaysOff.Skip(cx2Parameter.Value).Where(x => x != 0).Count() == 0))
                {
                    status = true;
                }

            }
            else if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
            {

                //if (line.BlkOfDaysOff[cx2Parameter.Value] != 0 && line.BlkOfDaysOff.Where(x => x != 0).Count() == 0)
                if (line.BlkOfDaysOff[cx2Parameter.Value] != 0)
                {
                    status = true;

                }
            }
            else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
            {
                //if ((line.BlkOfDaysOff.Take(cx2Parameter.Value+1).Where(x => x != 0).Count() == 0) && (line.BlkOfDaysOff.Skip(cx2Parameter.Value).Where(x => x != 0).Count() > 0))
                //{
                //    status = true;
                //}
                for (int count = cx2Parameter.Value + 1; count < line.BlkOfDaysOff.Count(); count++)
                {
                    if (line.BlkOfDaysOff[count] != 0)
                    {
                        status = true;
                        break;

                    }
                }
            }

            return status;

        }

        /// <summary>
        /// Commutable Deadhead constraint
        /// </summary>
        /// <param name="lstCx4parameter"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool CommutableDeadHeadConstraint(List<Cx4Parameter> lstCx4parameter, Line line)
        {   //SecondcellValue- CityId
            //ThirdcellValue -- Both,Begin,End
            //value          --Passes 

            bool status = false;

            if (lstCx4parameter.Count == 0)
                return status;

            foreach (Cx4Parameter cx4Parameter in lstCx4parameter)
            {

                //Both Ends
                if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.Both))
                {
                    if (cx4Parameter.Type == (int)ConstraintType.LessThan)
                    {
                        status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom < cx4Parameter.Value) && (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo < cx4Parameter.Value));
                    }
                    else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
                    {
                        status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom > cx4Parameter.Value) && (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo > cx4Parameter.Value));

                    }
                }
                //Either
                else if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.Either))
                {
                    if (cx4Parameter.Type == (int)ConstraintType.LessThan)
                    {
                        status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom < cx4Parameter.Value) || (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo < cx4Parameter.Value));
                    }
                    else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
                    {
                        status = line.CmtDhds.Any(x => (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom > cx4Parameter.Value) || (x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo > cx4Parameter.Value));

                    }
                }
                //Begining Only
                else if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.First))
                {

                    if (cx4Parameter.Type == (int)ConstraintType.LessThan)
                    {
                        status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo < cx4Parameter.Value);
                    }
                    else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
                    {
                        status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountTo > cx4Parameter.Value);
                    }

                }
                //end Only
                else if (cx4Parameter.ThirdcellValue == Convert.ToString((int)DeadheadType.Last))
                {
                    if (cx4Parameter.Type == (int)ConstraintType.LessThan)
                    {
                        status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom < cx4Parameter.Value);
                    }
                    else if (cx4Parameter.Type == (int)ConstraintType.MoreThan)
                    {
                        status = line.CmtDhds.Any(x => x.CityId == int.Parse(cx4Parameter.SecondcellValue) && x.CountFrom > cx4Parameter.Value);
                    }
                }

                if (!status)
                    break;
            }


            return !status;


        }

        /// <summary>
        /// Commutable Line Constraint
        /// </summary>
        /// <param name="cxCommutableLine"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool CommutableLineConstraint(CxCommutableLine cxCommutableLine, Line line)
        {
            bool status = false;
            // CxCommutableLine cxDeadHead = GlobalSettings.WBidStateContent.Constraints.CL;

            bool isCommuteFrontEndConstain = false;
            bool isCommuteBackEndConstrain = false;

            line.CommutableBacks = 0;
            line.commutableFronts = 0;
            line.CommutabilityFront = 0;
            line.CommutabilityBack = 0;
            line.CommutabilityOverall = 0;
            line.TotalCommutes = 0;


            //bool isCommuteFrontEndConstain = false;
            //bool isCommuteBackEndConstrain = false;
            bool CommuteFrontEnd = false;
            bool CommuteBackEnd = false;

            foreach (WorkBlockDetails workBlock in line.WorkBlockList)
            {

                int checkIntime = 0;
                int backToBaseTime = 0;

                switch (workBlock.StartDay)
                {
                    //Monday--Thurs
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        checkIntime = cxCommutableLine.MondayThu.Checkin;
                        backToBaseTime = cxCommutableLine.MondayThu.BackToBase;
                        backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                        break;
                    //Friday
                    case 5:
                        checkIntime = cxCommutableLine.Friday.Checkin;
                        backToBaseTime = cxCommutableLine.Friday.BackToBase;
                        backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                        break;
                    // saturday
                    case 6:
                        checkIntime = cxCommutableLine.Saturday.Checkin;
                        backToBaseTime = cxCommutableLine.Saturday.BackToBase;
                        backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                        break;
                    //sunday
                    case 0:
                        checkIntime = cxCommutableLine.Sunday.Checkin;
                        backToBaseTime = cxCommutableLine.Sunday.BackToBase;
                        backToBaseTime += (checkIntime > backToBaseTime) ? 1440 : 0;
                        break;

                }


                isCommuteFrontEndConstain = checkIntime > workBlock.StartTime;

                CommuteFrontEnd = checkIntime <= workBlock.StartTime;
                if (CommuteFrontEnd)
                {
                    line.commutableFronts++;
                }

                switch (workBlock.EndDay)
                {
                    //Monday--Thurs
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        checkIntime = cxCommutableLine.MondayThu.Checkin;
                        backToBaseTime = cxCommutableLine.MondayThu.BackToBase;
                        break;
                    //Friday
                    case 5:
                        checkIntime = cxCommutableLine.Friday.Checkin;
                        backToBaseTime = cxCommutableLine.Friday.BackToBase;
                        break;
                    // saturday
                    case 6:
                        checkIntime = cxCommutableLine.Saturday.Checkin;
                        backToBaseTime = cxCommutableLine.Saturday.BackToBase;
                        break;
                    //sunday
                    case 0:
                        checkIntime = cxCommutableLine.Sunday.Checkin;
                        backToBaseTime = cxCommutableLine.Sunday.BackToBase;
                        break;

                }

                isCommuteBackEndConstrain = backToBaseTime < workBlock.EndTime;

                CommuteBackEnd = backToBaseTime >= workBlock.EndTime;
                if (CommuteBackEnd)
                {
                    line.CommutableBacks++;
                }

                if (cxCommutableLine.RunBoth)
                {

                    if (isCommuteFrontEndConstain || isCommuteBackEndConstrain)
                    {
                        status = true;
                        break;
                    }
                }
                else if (cxCommutableLine.AnyNight)
                {

                    if (cxCommutableLine.CommuteToHome && cxCommutableLine.CommuteToWork)
                    {
                        if (isCommuteFrontEndConstain || isCommuteBackEndConstrain || workBlock.BackToBackCount > 0)
                        {
                            status = true;
                            break;
                        }


                    }
                    else if (cxCommutableLine.CommuteToHome && (isCommuteBackEndConstrain || workBlock.BackToBackCount > 0))
                    {
                        status = true;
                        break;
                    }

                    else if (cxCommutableLine.CommuteToWork && (isCommuteFrontEndConstain || workBlock.BackToBackCount > 0))
                    {
                        status = true;
                        break;
                    }


                }

            }
            line.TotalCommutes = line.WorkBlockList.Count;
            if (line.TotalCommutes > 0)
            {
                line.CommutabilityFront = Math.Round((line.commutableFronts / line.TotalCommutes) * 100, 2);
                line.CommutabilityBack = Math.Round((line.CommutableBacks / line.TotalCommutes) * 100, 2);
                line.CommutabilityOverall = Math.Round((line.commutableFronts + line.CommutableBacks) / (2 * line.TotalCommutes) * 100, 2);
            }
            return status;
            //line.ConstraintPoints.CmtLines = status;

        }

        public static bool CalculateCommutableLineAutoConstraint(FtCommutableLine ftCommutableLine, Line line)
        {
            bool status = false;
            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //Sate object for Commutable line filter
            //  var ftCommutableLine = (FtCommutableLine)item.BidAutoObject;

            //Here we are keeping all commute times for the bid
            if (wBidStateContent.Constraints.DailyCommuteTimesCmmutability != null)
            {


                bool isCommuteFrontEnd = false;
                bool isCommuteBackEnd = false;

                if (line.WorkBlockList != null)
                {
                    isCommuteFrontEnd = false;
                    isCommuteBackEnd = false;

                    foreach (WorkBlockDetails workBlock in line.WorkBlockList)
                    {
                        //Checking the  corresponding Commute based on Workblock Start time
                        var commutTimes = wBidStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.StartDateTime.Date);

                        if (commutTimes != null && ftCommutableLine.ToWork)
                        {
                            if (commutTimes.EarliestArrivel != DateTime.MinValue)
                            {
                                isCommuteFrontEnd = (commutTimes.EarliestArrivel.AddMinutes(ftCommutableLine.CheckInTime)) <= workBlock.StartDateTime.AddMinutes(workBlock.BriefTime);
                                if (!isCommuteFrontEnd)
                                {
                                    status = true;
                                    break;
                                }
                            }
                        }


                        //Checking the  corresponding Commute based on Workblock End time
                        //commutTimes = GlobalSettings.WBidStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDateTime.Date);
                        // using EndDate to account for irregular datetimes in company time keeping method.
                        commutTimes = wBidStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDate.Date);

                        if (commutTimes != null && ftCommutableLine.ToHome)
                        {
                            if (commutTimes.LatestDeparture != DateTime.MinValue)
                            {
                                isCommuteBackEnd = commutTimes.LatestDeparture.AddMinutes(-ftCommutableLine.BaseTime) >= workBlock.EndDateTime;
                                if (!isCommuteBackEnd)
                                {
                                    status = true;
                                    break;
                                }
                            }
                        }


                        //-----------------------------------------------------------------------------------------------
                        if (ftCommutableLine.NoNights)
                        {
                            //if ((ftCommutableLine.ToWork && isCommuteFrontEnd) || (ftCommutableLine.ToHome && isCommuteBackEnd))
                            if (workBlock.BackToBackCount > 0)  // if BackToBackCount is > 0 then there are nights in the middle
                            {
                                status = true;
                                break;
                            }


                        }
                        //-----------------------------------------------------------------------------------------------

                    }

                }

                //if (line.BAFilters == null)
                //{
                //    line.BAFilters = new List<bool>();
                //}
                //line.BAFilters.Add(status);

            }
            return status;
        }
        public static bool CalculateCommuttabilityConstraint(Commutability commutability, Line line)
        {
            bool status = false;

            if (commutability.SecondcellValue == ((int)CommutabilitySecondCell.NoMiddle))
            {
                if (line.NightsInMid > 0)
                {
                    status = true;
                }
                else
                {
                    if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Back))
                    {
                        if (commutability.Type == ((int)ConstraintType.LessThan))
                        {
                            if (line.CommutabilityBack > commutability.Value)
                                status = true;
                        }
                        else if (commutability.Type == ((int)ConstraintType.MoreThan))
                        {
                            if (line.CommutabilityBack < commutability.Value)
                                status = true;
                        }
                    }
                    else if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Front))
                    {
                        if (commutability.Type == ((int)ConstraintType.LessThan))
                        {
                            if (line.CommutabilityFront > commutability.Value)
                                status = true;
                        }
                        else if (commutability.Type == ((int)ConstraintType.MoreThan))
                        {
                            if (line.CommutabilityFront < commutability.Value)
                                status = true;
                        }

                    }
                    if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Overall))
                    {
                        if (commutability.Type == ((int)ConstraintType.LessThan))
                        {
                            if (line.CommutabilityOverall > commutability.Value)
                                status = true;
                        }
                        else if (commutability.Type == ((int)ConstraintType.MoreThan))
                        {
                            if (line.CommutabilityOverall < commutability.Value)
                                status = true;
                        }

                    }
                }

            }
            else
            {
                //OK Middle
                if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Back))
                {
                    if (commutability.Type == ((int)ConstraintType.LessThan))
                    {
                        if (line.CommutabilityBack > commutability.Value)
                            status = true;
                    }
                    else if (commutability.Type == ((int)ConstraintType.MoreThan))
                    {
                        if (line.CommutabilityBack < commutability.Value)
                            status = true;
                    }
                }
                else if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Front))
                {
                    if (commutability.Type == ((int)ConstraintType.LessThan))
                    {
                        if (line.CommutabilityFront > commutability.Value)
                            status = true;
                    }
                    else if (commutability.Type == ((int)ConstraintType.MoreThan))
                    {
                        if (line.CommutabilityFront < commutability.Value)
                            status = true;
                    }

                }
                if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Overall))
                {
                    if (commutability.Type == ((int)ConstraintType.LessThan))
                    {
                        if (line.CommutabilityOverall > commutability.Value)
                            status = true;
                    }
                    else if (commutability.Type == ((int)ConstraintType.MoreThan))
                    {
                        if (line.CommutabilityOverall < commutability.Value)
                            status = true;
                    }

                }
            }
            return status;
        }
        /// <summary>
        /// days of week constraint calculation
        /// </summary>
        /// <param name="lstcx3Parameter"></param>
        /// <param name="line"></param>
        private bool DaysofWeekConstraint(List<Cx3Parameter> lstcx3Parameter, Line line)
        {
            bool status = false;
            foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
            {
                if (cx3Parameter.Type == (int)ConstraintType.LessThan)
                {
                    if (line.DaysOfWeekWork[Convert.ToInt32(cx3Parameter.ThirdcellValue)] < cx3Parameter.Value)
                        status = true;
                }
                else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
                {
                    if (line.DaysOfWeekWork[Convert.ToInt32(cx3Parameter.ThirdcellValue)] > cx3Parameter.Value)
                        status = true;
                }
                if (status)
                    break;
            }

            return status;

        }
        private bool CalculateReportReleaseConstraint(List<ReportRelease> lstreportRelease, Line line)
        {
            bool status = false;
            Trip trip;
            foreach (var reportRelease in lstreportRelease)
            {
                if (reportRelease.AllDays)
                {

                    var lstReportReleaseData = CalculateReportReleaseTimes(line);
                    //If a user select “All days” and set Report Net 0830 (HHMM) and Release NLT 2230, we need to constraints all lines if the ANY of the dutyperiods in that line have less than 0830 report time OR greater than 2230 release NLT.
                    if (lstReportReleaseData.Any(x => x.Report < reportRelease.Report || x.Release > reportRelease.Release))
                    {
                        return true;
                    }
                }
                else
                {
                    //Report - Release: First just checks the First report time for the Work Block if NO Mid is checked. If No Mid is unchecked then First checks the first report time of all trips.
                    //Last just checks the last release time of the work block if NO Mid is checked. If NO MID is unchecked then Last checks the last release time of all trips.
                    // if the user selects Trip / Work Block and First and Last and No Mid then it is an AND condition for the work block. If NO MID is unchecked it is a first and last AND condition for all trips.
                    if (reportRelease.NoMid)
                    {
                        //line.WorkBlockList =0 condition will comes when all trips cancelled due to vacation
                        //check report and release time for work block
                        if (reportRelease.First && reportRelease.Last)
                        {
                            if (line.WorkBlockList.Any(x => x.StartTime < reportRelease.Report || x.EndTime > reportRelease.Release - GlobalSettings.debrief) || line.WorkBlockList.Count == 0)
                            {
                                return true;
                            }
                        }
                        else if (reportRelease.First)
                        {
                            if ((line.WorkBlockList.Any(x => x.StartTime < reportRelease.Report)) || line.WorkBlockList.Count == 0)
                            {
                                return true;
                            }
                        }
                        else if (reportRelease.Last)
                        {
                            if (line.WorkBlockList.Any(x => x.EndTime > reportRelease.Release - GlobalSettings.debrief) || line.WorkBlockList.Count == 0)
                            {
                                return true;
                            }
                        }

                    }
                    else
                    {

                        var lstReportReleaseData = CalculateReportReleaseTimes(line);
                        //check report and release time for trip
                        if (reportRelease.First && reportRelease.Last)
                        {
                            //get first and last day
                            var daysToCheck = lstReportReleaseData.Where(x => x.DaysStatus == (int)ReportReleaseDayType.StartDay || x.DaysStatus == (int)ReportReleaseDayType.LastDay || x.DaysStatus == (int)ReportReleaseDayType.OneDayTrip).ToList();

                            //we need to convert this day details into the trip formatted dates. It means we need to get the start and end time of the trip from this days
                            List<ReportReleaseData> Tripdata = new List<ReportReleaseData>();
                            for (int count = 0; count < daysToCheck.Count; count++)
                            {
                                if (daysToCheck[count].DaysStatus == (int)ReportReleaseDayType.OneDayTrip)
                                    Tripdata.Add(daysToCheck[count]);
                                else if (daysToCheck[count].DaysStatus == (int)ReportReleaseDayType.StartDay)
                                {
                                    ReportReleaseData objdata = new ReportReleaseData();
                                    objdata.Date = daysToCheck[count].Date;
                                    objdata.Report = daysToCheck[count].Report;
                                    if (daysToCheck.Count > (count + 1) && daysToCheck[count + 1].DaysStatus == (int)ReportReleaseDayType.LastDay)
                                        objdata.Release = daysToCheck[count + 1].Release;
                                    else
                                    {
                                    }
                                    Tripdata.Add(objdata);
                                    count++;
                                }

                            }
                            if (Tripdata.Any(x => x.Report < reportRelease.Report || x.Release > reportRelease.Release) || lstReportReleaseData.Count == 0)
                            {
                                return true;
                            }

                        }
                        else if (reportRelease.First)
                        {
                            var daysToCheck = lstReportReleaseData.Where(x => x.DaysStatus == (int)ReportReleaseDayType.StartDay || x.DaysStatus == (int)ReportReleaseDayType.OneDayTrip);
                            if (daysToCheck.Any(x => x.Report < reportRelease.Report) || lstReportReleaseData.Count == 0)
                            {
                                return true;
                            }

                        }
                        else if (reportRelease.Last)
                        {
                            var daysToCheck = lstReportReleaseData.Where(x => x.DaysStatus == (int)ReportReleaseDayType.LastDay || x.DaysStatus == (int)ReportReleaseDayType.OneDayTrip);
                            if (daysToCheck.Any(x => x.Release > reportRelease.Release) || lstReportReleaseData.Count == 0)
                            {
                                return true;
                            }

                        }
                    }

                }
            }
            return status;
        } /// <summary>
          /// PURPOSE:Calculate CalculateStartDayConstraint
          /// </summary>
          /// <param name="line"></param>
        public static bool CalculateStartDayConstraint(List<Cx3Parameter> lstCx3Parameter, Line line)
        {
            bool status = false;
            List<ReportReleaseData> StartTripDays = new List<ReportReleaseData>();
            if (lstCx3Parameter.Any(x => Convert.ToInt32(x.ThirdcellValue) == (int)StartDayType.Trip))
            {
                var tripDays = CalculateReportReleaseTimes(line);
                StartTripDays = tripDays.Where(x => x.DaysStatus == (int)ReportReleaseDayType.StartDay || x.DaysStatus == (int)ReportReleaseDayType.OneDayTrip).ToList();
            }

            foreach (Cx3Parameter cx3Parameter in lstCx3Parameter)
            {
                if (Convert.ToInt32(cx3Parameter.ThirdcellValue) == (int)StartDayType.Trip)
                {

                    if (cx3Parameter.Type == (int)StartDay.DoesnotStart)
                    {
                        if (!StartTripDays.Any(x => x.Date.Day == cx3Parameter.Value))
                        {
                            status = true;
                            break;
                        }
                    }
                    else
                    {
                        //line.StartDaysListPerTrip
                        if (StartTripDays.Any(x => x.Date.Day == cx3Parameter.Value))
                        {
                            status = true;
                            break;
                        }
                    }

                }
                else
                {
                    if (cx3Parameter.Type == (int)StartDay.DoesnotStart)
                    {
                        if (!line.WorkBlockList.Any(x => x.StartDateTime.Day == cx3Parameter.Value))
                        {
                            status = true;
                            break;
                        }
                    }
                    else
                    {
                        if (line.WorkBlockList.Any(x => x.StartDateTime.Day == cx3Parameter.Value))
                        {
                            status = true;
                            break;
                        }
                    }
                }

            }
            return status;
        }
        private static List<ReportReleaseData> CalculateReportReleaseTimes(Line line)
        {
          
            List<ReportReleaseData> lstReportRelease = new List<ReportReleaseData>();
            Trip trip;
            DateTime tripStartDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripStartDay = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                VacationStateTrip vacTrip = null;


                if (line.VacationStateLine != null && line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection == false || vacTrip == null)
                {
                    ReportReleaseData objData;

                    foreach (var dp in trip.DutyPeriods)
                    {
                        objData = new ReportReleaseData();
                        int dpcount = dp.DutPerSeqNum - 1;
                        objData.Date = tripStartDay.AddDays(dp.DutPerSeqNum - 1);
                        if (trip.DutyPeriods.Count == 1)
                            objData.DaysStatus = (int)ReportReleaseDayType.OneDayTrip;
                        else if (dp.DutPerSeqNum == 1)
                            objData.DaysStatus = (int)ReportReleaseDayType.StartDay;
                        else if (dp.DutPerSeqNum == trip.DutyPeriods.Count)
                            objData.DaysStatus = (int)ReportReleaseDayType.LastDay;
                        else
                            objData.DaysStatus = (int)ReportReleaseDayType.NormalDay;
                        SetReportRelease(lstReportRelease, trip, objData, dp, dpcount);
                    }
                }
                else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
                {
                    ReportReleaseData objData;
                    foreach (var vacDp in vacTrip.VacationDutyPeriods)
                    {
                        if (vacDp.DutyPeriodType == "VA" || vacDp.DutyPeriodType == "VO" || vacDp.DutyPeriodType == "VD" || vacDp.DutyPeriodType == "Split")
                        {
                         

                            continue;
                        }
                        else
                        {
                            objData = new ReportReleaseData();
                            var dp = trip.DutyPeriods.FirstOrDefault(x => x.DutPerSeqNum == vacDp.DutyperidSeqNo);
                            int dpcount = dp.DutPerSeqNum - 1;
                            objData.Date = tripStartDay.AddDays(dp.DutPerSeqNum - 1);
                            if (dp.DutPerSeqNum == 1)
                                objData.DaysStatus = (int)ReportReleaseDayType.StartDay;
                            else if (dp.DutPerSeqNum == trip.DutyPeriods.Count)
                                objData.DaysStatus = (int)ReportReleaseDayType.LastDay;
                            else
                                objData.DaysStatus = (int)ReportReleaseDayType.NormalDay;
                            SetReportRelease(lstReportRelease, trip, objData, dp, dpcount);
                        }
                    }
                }
                else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop == false)
                {
                    ReportReleaseData objData;
                    foreach (var vacDp in vacTrip.VacationDutyPeriods)
                    {
                        if (vacDp.DutyPeriodType == "VA" || vacDp.DutyPeriodType == "VO")
                        {
                            var currentdpdate = tripStartDay.AddDays(vacDp.DutyperidSeqNo - 1);
                            var previousDp = lstReportRelease.FirstOrDefault(x => x.Date == currentdpdate.AddDays(-1));
                            //if the vacation starts from the first dutyperiod, we can set the previous dp as a single day trip
                            if (previousDp != null)
                            {
                                if (vacDp.DutyperidSeqNo == 2)
                                    previousDp.DaysStatus = (int)ReportReleaseDayType.OneDayTrip;
                                else if (vacDp.DutyperidSeqNo > 2)
                                    previousDp.DaysStatus = (int)ReportReleaseDayType.LastDay;
                            }

                            continue;
                        }
                        else if (vacDp.DutyPeriodType == "Split")
                        {
                            objData = new ReportReleaseData();

                            var vdflights = vacDp.VacationFlights.Where(x => x.FlightType == "VD").ToList();
                            var lastvdFlight = vdflights[vdflights.Count() - 1];
                            var dp = trip.DutyPeriods.FirstOrDefault(x => x.DutPerSeqNum == vacDp.DutyperidSeqNo);
                            int dpcount = dp.DutPerSeqNum - 1;
                            if (vacTrip.TripType == "VOF")
                            {
                                if (trip.ReserveTrip)
                                {
                                    objData.Report = dp.ReserveOut - (dpcount * 1440);
                                    objData.Release = (dp.Flights.FirstOrDefault(x => x.FlightSeqNum == lastvdFlight.FlightSeqNo).ArrTime - (dpcount * 1440)) + GlobalSettings.ReserveDebrief;
                                }
                                else
                                {
                                    objData.Report = dp.ShowTime - (dpcount * 1440);
                                    objData.Release = (dp.Flights.FirstOrDefault(x => x.FlightSeqNum == lastvdFlight.FlightSeqNo).ArrTime - (dpcount * 1440)) + GlobalSettings.debrief;
                                }
                                objData.Release = (dp.Flights.FirstOrDefault(x => x.FlightSeqNum == lastvdFlight.FlightSeqNo).ArrTime - (dpcount * 1440)) + GlobalSettings.debrief;
                                objData.DaysStatus = (int)ReportReleaseDayType.LastDay;
                            }
                            else if (vacTrip.TripType == "VOB")
                            {
                                if (trip.ReserveTrip)
                                {

                                    objData.Report = (dp.Flights.FirstOrDefault(x => x.FlightSeqNum == vdflights[0].FlightSeqNo).DepTime - (dpcount * 1440)) - GlobalSettings.ReserveBriefTime;
                                    objData.Release = dp.ReserveIn - (dpcount * 1440);
                                }
                                else
                                {
                                    //objData.Report = (dp.Flights[0].DepTime - (dpcount * 1440)) + GlobalSettings.show1stDay;
                                    objData.Report = (dp.Flights.FirstOrDefault(x => x.FlightSeqNum == vdflights[0].FlightSeqNo).DepTime - (dpcount * 1440)) - GlobalSettings.show1stDay;
                                    objData.Release = dp.ReleaseTime - (dpcount * 1440);
                                }

                                if (vacTrip.VacationDutyPeriods.Count == dp.DutPerSeqNum)
                                    objData.DaysStatus = (int)ReportReleaseDayType.OneDayTrip;
                                else
                                    objData.DaysStatus = (int)ReportReleaseDayType.StartDay;

                            }
                            objData.Date = tripStartDay.AddDays(dp.DutPerSeqNum - 1);
                            lstReportRelease.Add(objData);
                        }
                        else
                        {
                            objData = new ReportReleaseData();
                            var dp = trip.DutyPeriods.FirstOrDefault(x => x.DutPerSeqNum == vacDp.DutyperidSeqNo);
                            int dpcount = dp.DutPerSeqNum - 1;
                            objData.Date = tripStartDay.AddDays(dp.DutPerSeqNum - 1);
                            // if the previous dutyperiod is Vacation effected dp, then we need to set this dutyperiod as the first Dp(start day)
                            bool isPreviousVacDp = false;
                            if (dp.DutPerSeqNum > 1)
                            {
                                var previousDp = vacTrip.VacationDutyPeriods.FirstOrDefault(x => x.DutyperidSeqNo == vacDp.DutyperidSeqNo - 1);
                                if (previousDp.DutyPeriodType == "VA" || previousDp.DutyPeriodType == "VO")
                                    isPreviousVacDp = true;

                            }
                            if (dp.DutPerSeqNum == 1 || isPreviousVacDp)
                                objData.DaysStatus = (int)ReportReleaseDayType.StartDay;
                            else if (dp.DutPerSeqNum == trip.DutyPeriods.Count)
                                objData.DaysStatus = (int)ReportReleaseDayType.LastDay;
                            else
                                objData.DaysStatus = (int)ReportReleaseDayType.NormalDay;
                            if (objData.DaysStatus == (int)ReportReleaseDayType.StartDay && vacTrip.VacationDutyPeriods.Count == dp.DutPerSeqNum)
                                objData.DaysStatus = (int)ReportReleaseDayType.OneDayTrip;
                            SetReportRelease(lstReportRelease, trip, objData, dp, dpcount);
                        }
                    }
                }
            }
            return lstReportRelease;
        }
        private static void SetReportRelease(List<ReportReleaseData> lstReportRelease, Trip trip, ReportReleaseData objData, DutyPeriod dp, int dpcount)
        {
            if (trip.ReserveTrip)
            {

                objData.Report = dp.ReserveOut - (dpcount * 1440);
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {

                    objData.Release = (dp.ReserveIn - (dpcount * 1440)) + GlobalSettings.FArelease;
                    if (objData.Report > objData.Release)
                        objData.Release = (dp.ReserveIn + (dpcount + 1 * 1440) - (dpcount * 1440)) + GlobalSettings.FArelease;
                }
                else
                {
                    objData.Release = (dp.ReserveIn - (dpcount * 1440));
                    //need to check the flight lands next day.
                    if (objData.Report > objData.Release)
                        objData.Release = (dp.ReserveIn + (dpcount + 1 * 1440) - (dpcount * 1440));
                }

            }
            else
            {
                objData.Report = dp.ShowTime - (dpcount * 1440);
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    objData.Release = (dp.ReleaseTime - (dpcount * 1440)) + GlobalSettings.FArelease;
                    if (objData.Report > objData.Release)
                        objData.Release = (dp.ReleaseTime + (dpcount + 1 * 1440) - ((dpcount + 1) * 1440)) + GlobalSettings.FArelease;
                }
                else
                {

                    objData.Release = dp.ReleaseTime - (dpcount * 1440);
                    if (objData.Report > objData.Release)
                        objData.Release = dp.ReleaseTime + (dpcount + 1 * 1440) - ((dpcount + 1) * 1440);
                }
            }
            lstReportRelease.Add(objData);
        }
		/// <summary>
		/// Days of month 
		/// </summary>
		/// <param name="workingDays"></param>
		/// <param name="offDays"></param>
		/// <param name="line"></param>
		private bool DaysOfMonthConstraint(List<DateTime> workingDays, List<DateTime> offDays, Line line)
		{

			bool status = false;



			if (workingDays.Count > 0)
			{
				foreach (DateTime dateTime in workingDays)
				{
					var dayObject = line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == dateTime && x.Working);
					status = (dayObject == null);

					if (status)
						break;
				}
			}

			if (!status && offDays.Count > 0)
			{
				foreach (DateTime dateTime in offDays)
				{
					var dayObject = line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == dateTime);
					status = (dayObject != null);

					if (status)
						break;
				}

			}

			return status;



		}


		/// <summary>
		/// PURPOSE:calculate deadhead constraint.
		/// </summary>
		/// <param name="line"></param>
		private bool DeadHeadConstraint(List<Cx3Parameter> lstcx3Parameter, Line line)
		{      //ThirdcellValue --Both,First,Last
			//Type= Less than,More than


			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{
				if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.First).ToString())
				{
					//if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					//{
					//    if (line.DhFirstTotal > cx3Parameter.Value)
					//        status = true;
					//}

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhFirstTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhFirstTotal > cx3Parameter.Value)
							status = true;
					}
				}
				else if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.Last).ToString())
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhLastTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhLastTotal > cx3Parameter.Value)
							status = true;
					}
				}
				else if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.Both).ToString())
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhLastTotal < cx3Parameter.Value || line.DhFirstTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhLastTotal > cx3Parameter.Value || line.DhFirstTotal > cx3Parameter.Value)
							status = true;
					}
				}

				if (status)
					break;
			}

			return status;

		}


		/// <summary>
		/// Dutyperiod Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool DutyPeriodConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			//retrive the current constrain values from global settings
			// Cx2Parameter cxDutyPeriod = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName).Constraints.DutyPeriod;
			bool status = false;

			//only want duty period longer than length
			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = line.DutyPeriodHours.Any(x => x < cx2Parameter.Value);
			}

			else if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
			{
				status = line.DutyPeriodHours.Any(x => x == cx2Parameter.Value);
			}
			//only want duty period shorter than length
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = line.DutyPeriodHours.Any(x => x > cx2Parameter.Value);
			}
			return status;
		}


		/// <summary>
		/// Equipment Type constraint Calcaultion
		/// </summary>
		/// <param name="lstcx3Parameter"></param>
		/// <param name="line"></param>
		private bool EquipmentTypeConstraints(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{

				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					//if (cx3Parameter.ThirdcellValue == "300")
					//{
					//	if (line.LegsIn300 < cx3Parameter.Value)
					//		status = true;
					//}
					//else if (cx3Parameter.ThirdcellValue == "500")
					//{
					//	if (line.LegsIn500 < cx3Parameter.Value)
					//		status = true;
					//}
					if (cx3Parameter.ThirdcellValue == "700")
					{
						if (line.LegsIn700 < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "800")
					{
						if (line.LegsIn800 < cx3Parameter.Value)
							status = true;

					}
					else if (cx3Parameter.ThirdcellValue == "600")
					{
						if (line.LegsIn600 < cx3Parameter.Value)
							status = true;

					}
                    else if (cx3Parameter.ThirdcellValue == "200")
                    {
                        if (line.LegsIn200 < cx3Parameter.Value)
                            status = true;

                    }
                    if (status)
						break;
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					//if (cx3Parameter.ThirdcellValue == "300")
					//{
					//	if (line.LegsIn300 > cx3Parameter.Value)
					//		status = true;
					//}
					//else if (cx3Parameter.ThirdcellValue == "500")
					//{
					//	if (line.LegsIn500 > cx3Parameter.Value)
					//		status = true;
					//}
					 if (cx3Parameter.ThirdcellValue == "700")
					{
						if (line.LegsIn700 > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "800")
					{
						if (line.LegsIn800 > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "600")
					{
						if (line.LegsIn600 > cx3Parameter.Value)
							status = true;
					}
                    else if (cx3Parameter.ThirdcellValue == "200")
                    {
                        if (line.LegsIn200 > cx3Parameter.Value)
                            status = true;
                    }
                    if (status)
						break;
				}
			}
			return status;
		}

		/// <summary>
		/// Flight Time Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool FlightTimeConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			bool status = false;

			int blkHrsInBp = Helper.ConvertHHMMtoMinute(line.BlkHrsInBp);

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (blkHrsInBp < cx2Parameter.Value);

			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (blkHrsInBp > cx2Parameter.Value);
			}


			return status;

		}

		private bool GrounTimeConstraint(Cx3Parameter cx3Parameter, Line line)
		{

			bool status;

			int count = 0;

			if (cx3Parameter.Type == (int)ConstraintType.LessThan)
			{
				count = line.GroundTimes.Count(x => x < int.Parse(cx3Parameter.ThirdcellValue));
			}
			else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
			{
				count = line.GroundTimes.Count(x => x > int.Parse(cx3Parameter.ThirdcellValue));
			}
			status = (count >= cx3Parameter.Value);

			return status;


		}


		private bool InternationalNonConusConstraint(List<Cx2Parameter> lstCx2Parameter, Line line)
		{

			bool status = false;
			List<string> internationalCities = null;
			List<string> nonConusCities = null;
			Trip trip = null;
			List<string> arrivalCities = new List<string>();
			foreach (string pairing in line.Pairings)
			{
				trip = GetTrip(pairing);
				if (trip != null)
				{
					arrivalCities.AddRange(trip.DutyPeriods.SelectMany(x => x.Flights.Select(y => y.ArrSta)).ToList());

				}

			}

			foreach (Cx2Parameter cx2Parameter in lstCx2Parameter)
			{
				//International 
				if (cx2Parameter.Type == (int)CityType.International)
				{
					internationalCities = new List<string>();
					//All cities
					if (cx2Parameter.Value == 0)
					{
						internationalCities = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).Select(y => y.Name).ToList();

					}
					else
					{
						var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.International && x.Id == cx2Parameter.Value);
						if (city != null)
							internationalCities.Add(city.Name);


					}
					status = arrivalCities.Intersect(internationalCities).Any();

				}

				//Non-Conus
				else if (cx2Parameter.Type == (int)CityType.NonConus)
				{
					nonConusCities = new List<string>();
					if (cx2Parameter.Value == 0)
					{
						nonConusCities = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).Select(y => y.Name).ToList();

					}
					else
					{
						var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.NonConus && x.Id == cx2Parameter.Value);
						if (city != null)
							nonConusCities.Add(city.Name);


					}
					status = arrivalCities.Intersect(nonConusCities).Any();

				}

				if (status)
					break;

			}


			return status;
			// line.ConstraintPoints.InterNonConus = status;


		}

		/// <summary>
		/// Legs per duty period constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool LegsPerDutyPeriodConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				for (int count = 1; count < cx2Parameter.Value; count++)
				{
					status = (line.LegsPerDutyPeriod[count] != 0);
					if (status)
						break;

				}
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				for (int count = cx2Parameter.Value + 1; count < 10; count++)
				{
					status = (line.LegsPerDutyPeriod[count] != 0);
					if (status)
						break;

				}
			}
			return status;


		}

		/// <summary>
		/// Legs Per Pairing
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool LegsPerPairingConstraint(Cx2Parameter cx2Parameter, Line line)
		{
			bool status = false;


			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (line.MostLegs < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (line.MostLegs > cx2Parameter.Value);
			}

			return status;
		}


		private bool NumberofDaysOffConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (line.DaysOff < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (line.DaysOff > cx2Parameter.Value);
			}


			return status;
		}

		/// <summary>
		///  Overnight Cities constraint calcualtion
		/// </summary>
		/// <param name="lstcx3Parameter"></param>
		/// <param name="line"></param>
		private bool OverNightCitiesConstraints(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{

				var city= GlobalSettings.OverNightCitiesInBid.FirstOrDefault(x => x.Id == int.Parse(cx3Parameter.ThirdcellValue));
				if (city != null)
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						// if (line.OvernightCities.Where(x => x == cx3Parameter.ThirdcellValue).Count() > cx3Parameter.Value)

						if (line.OvernightCities.Where(y => y.ToString() == GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == int.Parse(cx3Parameter.ThirdcellValue)).Name).Count() < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						// if (line.OvernightCities.Where(x => x == cx3Parameter.ThirdcellValue).Count() <= cx3Parameter.Value)
						if (line.OvernightCities.Where(y => y.ToString() == GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == int.Parse(cx3Parameter.ThirdcellValue)).Name).Count() > cx3Parameter.Value)
							status = true;

					}
				}
				if (status)
					break;
			}
			return status;
		}
		private bool CitiesLegsConstraints(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			if (!line.ReserveLine) {
				Trip trip;
				List<string> citiesleges = new List<string> ();
				foreach (var pairing in line.Pairings) {
					trip = GlobalSettings.Trip.Where (x => x.TripNum == pairing.Substring (0, 4)).FirstOrDefault ();
					if (trip == null) {
						trip = GlobalSettings.Trip.Where (x => x.TripNum == pairing).FirstOrDefault ();
					}
					trip = GlobalSettings.Trip.Where (x => x.TripNum == pairing.Substring (0, 4)).FirstOrDefault ();
					trip = trip ?? GlobalSettings.Trip.Where (x => x.TripNum == pairing).FirstOrDefault ();
					//  string tripName = pairing.Substring(0, 4);

					foreach (var dp in trip.DutyPeriods)
					{
						foreach (var flt in dp.Flights)
							if (flt.ArrSta != null)
							{
								citiesleges.Add(flt.ArrSta);
							}
					}
					//remove last flights because we dont need to consider this last flight of a trip
					if (citiesleges.Count >= 1)
					{
						citiesleges.RemoveAt(citiesleges.Count - 1);
					}
				}

				foreach (Cx3Parameter cx3Parameter in lstcx3Parameter) {

					if (cx3Parameter.Type == (int)ConstraintType.LessThan) {


							if (citiesleges.Where(y => y.ToString() == GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == int.Parse(cx3Parameter.ThirdcellValue)).Name).Count() < cx3Parameter.Value)
								status = true;
						
					} else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						
						if (citiesleges.Where (y => y.ToString () == GlobalSettings.WBidINIContent.Cities.FirstOrDefault (x => x.Id == int.Parse (cx3Parameter.ThirdcellValue)).Name).Count () > cx3Parameter.Value)
							status = true;
						


					}
					if (status)
						break;
				}
			}
			return status;
		}
		/// <summary>
		/// Partial DaysOff Constraint
		/// </summary>
		/// <param name="lstCx4Parameter"></param>
		/// <param name="line"></param>
		/// <param name="lstDateHelper"></param>
		/// <returns></returns>
		private bool PartialDaysOffConstraint(List<Cx4Parameter> lstCx4Parameter, Line line, List<DateHelper> lstDateHelper)
		{
			bool status = false;
			List<City> citylist = GlobalSettings.WBidINIContent.Cities;
			DateTime date = new DateTime();



			Day newDay = null;
			foreach (Cx4Parameter cx4Parameter in lstCx4Parameter)
			{
				City city = citylist.FirstOrDefault(x => x.Id == int.Parse(cx4Parameter.ThirdcellValue));
				if (city != null)
				{
					//300 indicates the user selected Any Date from the Date selection view
					if (cx4Parameter.SecondcellValue == "300")
					{
						if (cx4Parameter.Type == (int)ConstraintType.atbefore)
						{
							newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true && x.DepartutreCity == city.Name) || ( x.DepartutreCity == city.Name && (x.DepartutreTime) >= cx4Parameter.Value));

						}
						else
						{
							newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true &&  x.ArrivalCity == city.Name) || ( x.ArrivalCity == city.Name && (x.ArrivalTime) <= cx4Parameter.Value));

						}
					}
					else
					{
						date = lstDateHelper.FirstOrDefault(x => x.DateId == int.Parse(cx4Parameter.SecondcellValue)).Date;

						if (cx4Parameter.Type == (int)ConstraintType.atbefore)
						{
							newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true && x.Date == date && x.DepartutreCity == city.Name) || (x.Date == date && x.DepartutreCity == city.Name && (x.DepartutreTime) >= cx4Parameter.Value));

						}
						else
						{
							newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true && x.Date == date && x.ArrivalCity == city.Name) || (x.Date == date && x.ArrivalCity == city.Name && (x.ArrivalTime) <= cx4Parameter.Value));

						}
					}
					if (newDay == null)
					{
						status = true;
						break;
					}
				} //400 indicates the user selected Any city from the city selection view. In those case we dont need to consider city
				else if (cx4Parameter.ThirdcellValue == "400")
				{
					//300 indicates the user selected Any Date from the Date selection view
					if (cx4Parameter.SecondcellValue == "300")
					{
						if (cx4Parameter.Type == (int)ConstraintType.atbefore)
						{
							// newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true ) || ((x.DepartutreTime) >= cx4Parameter.Value));
							// newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true));
							// if(newDay!=null)
							// newDay = line.DaysInBidPeriod.Any( x => x.DepartutreTime < cx4Parameter.Value && x.OffDuty==false) ? null : new Day();
							// newDay = line.DaysInBidPeriod.All(x => x.DepartutreTime <= cx4Parameter.Value) ? null : new Day();
							newDay = line.DaysInBidPeriod.Any(x => x.DepartutreTime < cx4Parameter.Value && x.OffDuty == false)? null:new Day();


						}
						else
						{
							//  newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true ) || ((x.ArrivalTime) <= cx4Parameter.Value));

							newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true)) ;
							if (newDay != null)
								newDay = line.DaysInBidPeriod.Any(x => x.ArrivalTime > cx4Parameter.Value && x.OffDuty == false) ? null : new Day();
						}
					}
					else
					{
						date = lstDateHelper.FirstOrDefault(x => x.DateId == int.Parse(cx4Parameter.SecondcellValue)).Date;

						if (cx4Parameter.Type == (int)ConstraintType.atbefore)
						{
							newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true && x.Date == date) || (x.Date == date && (x.DepartutreTime) >= cx4Parameter.Value));

						}
						else
						{
							newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty == true && x.Date == date) || (x.Date == date && (x.ArrivalTime) <= cx4Parameter.Value));

						}
					}
					if (newDay == null)
					{
						status = true;
						break;
					}
				}

			}

			return status;


		}

		private bool RestConstraint(List<Cx3Parameter> lstCx3Parameter, Line line)
		{

			bool status = false;


			foreach (Cx3Parameter cx3Parameter in lstCx3Parameter)
			{
				var restinMinutes = cx3Parameter.Value*60;
				if (cx3Parameter.Type == (int)RestType.All)
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Any(x => x.RestMinutes < restinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Any(x => x.RestMinutes > restinMinutes);
					}
				}

				else if (cx3Parameter.Type == (int)RestType.InDomicile)
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Where(x => !x.IsInTrip).Any(x => x.RestMinutes < restinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Where(x => !x.IsInTrip).Any(x => x.RestMinutes > restinMinutes);
					}
				}

				else if (cx3Parameter.Type == (int)RestType.AwayDomicile)
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Where(x => x.IsInTrip).Any(x => x.RestMinutes < restinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Where(x => x.IsInTrip).Any(x => x.RestMinutes > restinMinutes);
					}
				}


			}

			return status;

		}

		/// <summary>
		/// Star day of the Week Constraint  calculation (Mon =0)
		/// </summary>
		/// <param name="lstcx3Parameter"></param>
		/// <param name="line"></param>
		private bool StartDayOfWeekConstraint(List<Cx3Parameter> lstcx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstcx3Parameter)
			{
				if (Convert.ToInt32(cx3Parameter.SecondcellValue) == 1)
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.StartDaysList[Convert.ToInt32(cx3Parameter.ThirdcellValue)] < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.StartDaysList[Convert.ToInt32(cx3Parameter.ThirdcellValue)] > cx3Parameter.Value)
							status = true;
					}
					if (status)
						break;
				}
				else if (Convert.ToInt32(cx3Parameter.SecondcellValue) == 2)
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.StartDaysListPerTrip[Convert.ToInt32(cx3Parameter.ThirdcellValue)] < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.StartDaysListPerTrip[Convert.ToInt32(cx3Parameter.ThirdcellValue)] > cx3Parameter.Value)
							status = true;
					}
					if (status)
						break;
				}
			}
			return status;
		}

		/// <summary>
		/// Time Away From Base Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		private bool TimeAwayFromBaseConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp) < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp) > cx2Parameter.Value);
			}

			return status;

		}

		private bool TripLengthConstraint(List<Cx3Parameter> lstCx3Parameter, Line line)
		{
			bool status = false;
			foreach (Cx3Parameter cx3Parameter in lstCx3Parameter)
			{
				//Type=1Day, 2Day, 3Day, 4Day
				switch (cx3Parameter.ThirdcellValue)
				{
				case "1":

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips1Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips1Day > cx3Parameter.Value);
					}

					break;

				case "2":
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips2Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips2Day > cx3Parameter.Value);
					}

					break;

				case "3":
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips3Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips3Day > cx3Parameter.Value);
					}
					break;
				case "4":
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = (line.Trips4Day < cx3Parameter.Value);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = (line.Trips4Day > cx3Parameter.Value);
					}
					break;



				}


				if (status)
					break;

			}


			return status;


		}

		/// <summary>
		/// WorkBlock Length
		/// </summary>
		/// <param name="lstCx3Parameter"></param>
		/// <param name="line"></param>
		private bool WorkBlockLengthConstraint(List<Cx3Parameter> lstCx3Parameter, Line line)
		{


			bool status = false;

			foreach (Cx3Parameter cx3Parameter in lstCx3Parameter)
			{

				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					status = (line.WorkBlockLengths[int.Parse(cx3Parameter.ThirdcellValue)] < cx3Parameter.Value);
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					status = (line.WorkBlockLengths[int.Parse(cx3Parameter.ThirdcellValue)] > cx3Parameter.Value);
				}


				if (status)
					break;

			}
			return status;


		}

		/// <summary>
		/// Work days Constraint
		/// </summary>
		/// <param name="cx2Parameter"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		private bool WorkDaysConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;
			int workDaysCount = line.DaysOfMonthWorks.Count(x => x.Working);

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (workDaysCount < cx2Parameter.Value);

			}
			if (cx2Parameter.Type == (int)ConstraintType.EqualTo)
			{
				status = (workDaysCount == cx2Parameter.Value);

			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (workDaysCount > cx2Parameter.Value);
			}

			return status;

		}


		private bool MinimumPayConstraint(Cx2Parameter cx2Parameter, Line line)
		{

			bool status = false;

			if (cx2Parameter.Type == (int)ConstraintType.LessThan)
			{
				status = (line.Tfp < cx2Parameter.Value);
			}
			else if (cx2Parameter.Type == (int)ConstraintType.MoreThan)
			{
				status = (line.Tfp > cx2Parameter.Value);
			}

			return status;

		}
		public bool OvernightCitiesBulkCalculation(BulkOvernightCityCx bulkOvernightCityCx, Line line)
		{
			bool status = false;


			if (bulkOvernightCityCx.OverNightYes !=null &&bulkOvernightCityCx.OverNightYes.Count > 0)
			{
				List<string> lstYesCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightYes.Contains(x.Id)).Select(y => y.Name).ToList();
				status = !line.OvernightCities.Intersect(lstYesCityNames).Any();
			}

			if (status == false)
			{
				if (bulkOvernightCityCx.OverNightNo!=null&& bulkOvernightCityCx.OverNightNo.Count > 0)
				{
					List<string> lstNoCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightNo.Contains(x.Id)).Select(y => y.Name).ToList();
					status = line.OvernightCities.Intersect(lstNoCityNames).Any();
				}
			}


			return status;

		}

		/// <summary>
		/// 3 On 3 off Constraint
		/// </summary>
		/// <param name="line"></param>
		private bool ThreeOn3offConstraint(Cx2Parameter cx2Parameter, Line line)
		{

//			return (cx2Parameter.Type == (int)ThreeOnThreeOff.ThreeOnThreeOff) ? line.Is3on3Off : !line.Is3on3Off;
			return (cx2Parameter.Type == (int)ThreeOnThreeOff.ThreeOnThreeOff) ? !line.Is3on3Off : line.Is3on3Off;
			//bool status = false;

			//if (cx2Parameter.Type == (int)ThreeOnThreeOff.ThreeOnThreeOff)
			//{
			//    status= line.Is3on3Off;

			//}
			//else  if   (cx2Parameter.Type == (int)ThreeOnThreeOff.NoThreeOnThreeOff)
			//{
			//    status= !line.Is3on3Off;

			//}

			//return status;


		}
        /// <summary>
        /// PURPOSE : Calculate Mixed HardandReserve Constraint
        /// </summary>
        /// <param name="line"></param>
        public static bool CalculateMixedHardandReserveConstraint(Line line)
        {
            bool status = false;
            Trip trip = null;
            int hardTripCount = 0;
            int reserveTripCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (trip.ReserveTrip)
                    reserveTripCount++;
                else
                    hardTripCount++;

            }
            if (hardTripCount > 0 && reserveTripCount > 0)
                status = true;

            return status;
        }

        private static Trip GetTrip(string pairing)
		{
			Trip trip = null;
			trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
			if (trip == null)
			{
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
			}
			if (trip == null && pairing.Length>6)
			{
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0,6)).FirstOrDefault();
			}

			return trip;

		}



		#endregion





	}
}
