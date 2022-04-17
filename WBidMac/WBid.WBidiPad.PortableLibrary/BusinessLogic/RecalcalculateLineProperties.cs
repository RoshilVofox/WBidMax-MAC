using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class RecalcalculateLineProperties
    {
        #region Properties
        private string _month;
        private string _year;
        private AmPmConfigure _amPmConfigure;
        private DateTime _bpStartDate;
        private DateTime _bpEndDate;
        private decimal _flyPayInLine;
        //private Dictionary<string, TripMultiVacData> _vacationData;
        private Dictionary<string, string> _mILTripsOfSingleLine;
        private Dictionary<string, TripMultiMILData> _mILData;
        private bool isAdajacentEOMandVacation;
        private DateTime _eomDate;
        #endregion


        public void CalcalculateLineProperties()
        {
            try
            {
                _month = GlobalSettings.CurrentBidDetails.Month.ToString();
                _year = GlobalSettings.CurrentBidDetails.Year.ToString();
                _amPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
                _bpStartDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
                _bpEndDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;

                if (GlobalSettings.CurrentBidDetails.Postion != "FA")
                {
                    _eomDate = WBidCollection.GetnextSunday();
                }
                else
                {
                    _eomDate = GlobalSettings.FAEOMStartDate;
                }
                if (GlobalSettings.MenuBarButtonStatus.IsMIL)
                {
                    _mILData = GlobalSettings.MILData;
                    if (_mILData == null)
                        return;
                }
                if (GlobalSettings.OrderedVacationDays != null)
                {
                    if (GlobalSettings.OrderedVacationDays.Count > 0)
                    {
                        var lastvacation = GlobalSettings.OrderedVacationDays.LastOrDefault();
                        isAdajacentEOMandVacation = (_eomDate == lastvacation.EndAbsenceDate || _eomDate == lastvacation.EndAbsenceDate.AddDays(1));
                    }
                }
                foreach (Line line in GlobalSettings.Lines)
                {
                    try
                    {
                        if (line.BlankLine)
                        {
                            line.TafbInBp = "0:00";
                            line.Equip8753 = string.Empty;
                            continue;
                        }
                        if (GlobalSettings.MenuBarButtonStatus.IsMIL)
                        {
                            _mILTripsOfSingleLine = new Dictionary<string, string>();

                            try
                            {
                                if (line.LineNum == 15)
                                {

                                }
                                SetMILAffetedTrips(line);
                                GenerateMILBidLineTemplate(line);
                                line.DaysOff = CalculateMILDaysOff(line);
                                line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                                line.FlyPay = CalculateMILFlightPay(line);
                                line.Tfp = line.FlyPay;
                              
                                line.BlkHrsInBp = CalculateMILBlockHours(line);
                                line.TafbInBp = line.BlankLine || line.ReserveLine ? "0:00" : CalculateMILTAFB(line);
                                
                                line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffMIL(line);
                                line.DutyHrsInBp = CalcDutyHrsForMIL(line, false);
                                line.DutyHrsInLine = CalcDutyHrsForMIL(line, true);
                                line.AcftChanges = CalcNumAcftChangesMIL(line);
                                line.TfpPerDay = CalculateTfpPerDay(line);
                                line.TotDutyPds = CalcTotDutPdsForMIL(line, true);
                                line.TotDutyPdsInBp = CalcTotDutPdsForMIL(line, false);
                                line.DaysWorkInLine = CalcDaysWorkInLineForMIL(line);
                                line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2);
                                line.AcftChgDay = decimal.Parse(String.Format("{0:0.00}", line.AcftChgDay));
                                line.DaysOfWeekWork = CalcWeekDaysWorkforMIL(line);
                                line.DaysOfMonthWorks = CalcDaysOfMonthOffForMIL(line);
                                line.LastArrTime = CalcLastArrTimeForMIL(line);
                                line.LastDomArrTime = CalcLastDomArrTimeForMIL(line);
                                line.StartDow = CalcStartDowMIL(line);
                                line.StartDaysList = CalcStartDowListForMIL(line);
                                line.StartDaysListPerTrip = CalcStartDowListPerTripForMIL(line);
                                line.LegsPerDutyPeriod = CalcLegsPerDutyPeriodForMIL(line);
                                line.LegsPerDay = CalcLegsPerDay(line);
                                line.TotPairings = CalcTotPairingsForMIL(line);
                                line.LegsPerPair = CalcLegsPerPairForMIL(line);
                                CalculateWorkBlockLengthMIL(line);
                                CalcNumLegsOfEachTypeforMIL(line);
                                CalcTripLengthforMIL(line);
                                line.T234 = CalcT234(line);
                                line.Equip8753 = line.LegsIn700 + "-" + line.LegsIn800 + "-" + line.LegsIn200 + "-" + line.LegsIn600;
                                line.MostLegs = CalcMostlegsForMIL(line);
                                line.ShowOverLap = false;
                                CalculateHolidayRigProperties(line);
                                line.Tfp += line.HolRig;
                                line.VacPlusRig = line.VacPayCuBp + line.LineRig;
                                line.TfpPerFltHr = CalcTfpPerFltHrVacationforMIL(line);
                                line.TfpPerTafb = CalcTafbTime(line);
                                line.VAne = 0;
                                line.VAbo = 0;
                                line.VAbp = 0;
                                line.VAPne = 0;
                                line.VAPbp = 0;
                                line.VAPbo = 0;
                            }
                            catch (Exception ex)
                            {
                            }

                        }

                        else if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            line.VacPayCuBp = 0;
                            line.VacPayNeBp = 0;
                            line.VacPayBothBp = 0;
                            line.VacationDrop = 0;
                            line.VacationOverlapBack = 0;
                            line.VacationOverlapFront = 0;
                            line.VAne = 0;
                            line.VAbo = 0;
                            line.VAbp = 0;
                            line.VAPne = 0;
                            line.VAPbp = 0;
                            line.VAPbo = 0;
                            line.AMPM = CalcAmPmProp(line);
                            line.AMPMSortOrder = CalcAmPmSortOrder(line);
                            line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine));
                            line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;

                            line.BlkHrsInBp = line.TempBlkHrsInBp;
                            line.Tfp = line.TempTfp;
                            line.DaysOff = line.TempDaysOff;
                            line.TafbInBp = line.TempTafbInBp;
                            line.Legs = line.TempLegs;
                            line.DaysWorkInLine = line.TempDaysWorkInLine;
                            line.TfpInLine = line.TempTfpInLine;
                            line.BlkHrsInLine = line.TempBlkHrsInLine;
                            line.TafbInLine = line.BlankLine || line.ReserveLine ? "0:00" : CalcTafb(line, true);
                            line.ShowOverLap = false;


                            
                            line.DutyHrsInBp = CalcDutyHrs(line, false);
                            line.DutyHrsInLine = CalcDutyHrs(line, true);
                            
                            line.LegsPerDay = CalcLegsPerDay(line);
                            //line.LegsPerPair = (line.ReserveLine || line.BlankLine) ? 0 : (line.Pairings.Count == 0) ? 0 : line.Pairings.Count == 0 ? 0 : Math.Round(Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.Pairings.Count), 2, MidpointRounding.AwayFromZero);
                            line.LegsPerPair = (line.ReserveLine || line.BlankLine) ? 0 : (line.Pairings.Count == 0) ? 0 : line.Pairings.Count == 0 ? 0 : Math.Round(Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.Pairings.Count), 2);
                            line.Weekend = CalcWkEndProp(line);
                            line.TotDutyPds = CalcTotDutPds(line, true);
                            line.TotDutyPdsInBp = CalcTotDutPds(line, false);
                            line.LargestBlkOfDaysOff = CalcLargestBlkDaysOff(line);
                            line.LongestGrndTime = CalcLongGrndTime(line);
                            line.MostLegs = CalcMostlegs(line);
                            //Trips1Day,Trips2Day,Trips3Day,Trips4Day
                            CalcTripLength(line);
                            //LegsIn800 ,LegsIn700 ,LegsIn500,LegsIn300 
                            CalcNumLegsOfEachType(line);
                            line.Equip8753 = line.LegsIn700.ToString() + "-" + line.LegsIn800.ToString() + "-" + line.LegsIn200.ToString()+ "-" + line.LegsIn600.ToString();
                            line.AcftChanges = CalcNumAcftChanges(line);
                            line.DaysWorkInLine = CalcDaysWorkInLine(line);
                            //line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2, MidpointRounding.AwayFromZero);
                            line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2);
                            line.AcftChgDay = decimal.Parse(String.Format("{0:0.00}", line.AcftChgDay));
                            line.LastArrTime = CalcLastArrTime(line);
                            line.LastDomArrTime = CalcLastDomArrTime(line);
                            line.StartDow = CalcStartDow(line);
                            line.StartDaysList = CalcStartDowList(line);
                            line.StartDaysListPerTrip = CalcStartDowListPerTrip(line);
                            line.T234 = CalcT234(line);
                            // line.VacationDrop = 0.00m;
                            line.BlkOfDaysOff = CalcBlkOfDaysOff(line);
                            line.LegsPerDutyPeriod = CalcLegsPerDutyPeriod(line);
                            line.DaysOfWeekWork = CalcWeekDaysWork(line);
                            line.DaysOfMonthWorks = CalcDaysOfMonthOff(line);
                            CalcOvernightCities(line);
                            line.RestPeriods = CalcRestPeriods(line);
                            CalculateWorkBlockLength(line);
                            line.EPush = CalcEPush(line).ToString(@"hh\:mm");
                            line.EDomPush = CalcEDomPush(line).ToString(@"hh\:mm");
                            line.TotPairings = line.Pairings.Count();
                            CalculateWorkBlockDetails(line);
                            line.NightsInMid = line.WorkBlockList.Sum(x => x.nightINDomicile);
                            line.TotalCommutes = line.WorkBlockList.Count;
                            CalculateAverageLatestDomicileArrivalAndPush(line);
                            line.ShowOverLap = false;
                            CalculateHolidayRigProperties(line);
                            line.Tfp += line.HolRig;
                            SetInternationAndNoConusLine(line);
                            line.VacPlusRig = line.VacPayCuBp + line.LineRig;
                            CalculateDeadHeadProperties(line);
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                            {
                                line.ETOPS = CalculateETOPSProperties(line);
                            }
                            if (line.ETOPS)
                            {
                                line.ETOPSTripsCount = CalculateETOPSTripsCount(line);
                            }
                            line.TfpPerFltHr = CalcTfpPerFltHr(line);
                            line.TfpPerDay = CalculateTfpPerDay(line);
                            line.TfpPerTafb = CalcTafbTime(line);
                            line.TfpPerDhr = CalcTfpPerDhr(line);
                        }
                        //if vacation  only
                        else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            if (GlobalSettings.OrderedVacationDays == null || GlobalSettings.OrderedVacationDays.Count == 0)
                            {
                                HandleOnlyFvVacation(line);
                                line.Tfp += line.HolRig;
                                line.Tfp += line.CFVPay;
                            }


                            else if (line.VacationStateLine != null)
                            {
                                //===========================
                                int tafbInBpInt = 0;
                                line.BlkHrsInBp = line.TempBlkHrsInBp;
                                line.DaysOff = line.TempDaysOff;
                                line.FlyPay = line.TempTfp;
                                _flyPayInLine = line.TempTfpInLine;
                                line.Legs = line.TempLegs;
                                line.TafbInBp = line.TempTafbInBp;
                                line.VacPayCuBp = 0;
                                line.VacPayNeBp = 0;
                                line.VacPayBothBp = 0;
                                line.VacationDrop = 0;
                               
                               
                                var blk = Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp);
                                var blkvac = Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs);
                                if (blk > blkvac)
                                {
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(blk - blkvac);
                                } else
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(0);
                                line.DaysOff += line.VacationStateLine.VacationDaysOff;
                                line.FlyPay = line.VacationStateLine.FlyPay;
                                _flyPayInLine = line.VacationStateLine.FlyPayInLine;
                                line.Legs -= line.VacationStateLine.VacationLegs;
                                tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb);
                                if (tafbInBpInt < 0) tafbInBpInt = 0;
                                line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                                line.VacPayCuBp = line.VacationStateLine.VacationTfp;
                                line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine;
                                line.VacPayNeBp = line.VacPayBothBp - line.VacPayCuBp;
                                line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationFront)), 2);
                                line.VacationOverlapBack = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationBack)), 2); ;
                                line.TfpInLine = line.VacPayCuBp + line.TfpInLine;
                                line.FlyPay = line.FlyPay + line.LineRig;


                                //FV modification
                                //===
                                if (line.FVvacationData != null)
                                {
                                    //For the FV vacation for reserve lines, we can consider each day of the trip as single day trip and we do not need to shift the FV vacation.Also we need to count number of days removed due to the FV vacation and we need to subtract those days tfp values from the flight pay for the reserve line.
                                    var Fvflypay = line.ReserveLine ? line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs) * 6 : line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripTfpInLine);
                                    if (line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Any(y => y.TripStartDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && y.TripEndDate >= GlobalSettings.CurrentBidDetails.BidPeriodEndDate))
                                        line.FlyPay = line.FlyPay - (Fvflypay - (line.TempTfpInLine - line.TempTfp));
                                    else
                                        line.FlyPay = line.FlyPay - Fvflypay;
                                    var fvtrips = line.FVvacationData.Where(x => x.FVEndDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
                                    line.VacPayCuBp = line.VacPayCuBp + fvtrips.Sum(x => x.FVVacTfp);
                                    _flyPayInLine = _flyPayInLine - Fvflypay;
                                    line.DaysOff = line.DaysOff + GeWorkDaysInSideFV(line); ;
                                    line.Legs = line.ReserveLine ? 0 : line.Legs - line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs);
                                    line.VacPayBothBp = line.VacPayBothBp + line.FVvacationData.Sum(x => x.FVVacTfp);

                                    int droppedVacationDayDuetoFV = line.FVvacationData.Sum(x => x.VaVacationDropDays);
                                    var fvtripdata = line.FVvacationData.SelectMany(x => x.FVVacationTripDatas);
                                    var Vodropvactiontfp = line.FVvacationData.Sum(x => x.VOTfpDrop);
                                    decimal droppedVacationvaluesDuetoFV = line.FVvacationData.Sum(x => x.VDTfpDrop);
                                    line.VacPayCuBp = line.VacPayCuBp - Vodropvactiontfp;
                                    line.VacPay = line.VacPayCuBp;
                                    line.VacPayBothBp = line.VacPayBothBp - Vodropvactiontfp;
                                    line.DaysOff += droppedVacationDayDuetoFV;
                                    line.FlyPay = line.FlyPay - droppedVacationvaluesDuetoFV;
                                    line.TfpInLine -= droppedVacationvaluesDuetoFV;

                                }
                                _flyPayInLine = _flyPayInLine + line.LineRig;

                                line.Tfp = line.VacPayCuBp + line.FlyPay;
                                line.CarryOverTfp = line.TfpInLine - line.Tfp;
                                if (line.CarryOverTfp < 0 && line.CarryOverTfp > -1.0m)
                                {
                                    line.CarryOverTfp = 0;
                                }



                                line.VacPayCuBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacPayCuBp)), 2);
                                line.FlyPay = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.FlyPay)), 2);

                                line.Tfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.Tfp)), 2);
                                line.CarryOverTfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.CarryOverTfp)), 2);

                                //======================

                                line.AMPM = CalcAmPmPropVacation(line);
                                line.AMPMSortOrder = CalcAmPmSortOrder(line);
                                 blk = Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine); 
                                var vacblkdrp = Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs);
                                if (blk > vacblkdrp)
                                {
                                    line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));
                                }
                                else
                                {
                                    line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(0);
                                }
                                
                                line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                               
                                line.DutyHrsInBp = CalcDutyHrsVacation(line, false);
                                line.DutyHrsInLine = CalcDutyHrsVacation(line, true);
                                
                                line.LegsPerDay = CalcLegsPerDay(line);
                                line.LegsPerPair = CalcLegsPerPairVacation(line);
                                line.Weekend = CalcWkEndProp(line);
                                line.TotDutyPds = CalcTotDutPdsVacation(line, true);
                                line.TotDutyPdsInBp = CalcTotDutPdsVacation(line, false);
                                line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffVacation(line);
                                line.LongestGrndTime = CalcLongGrndTimeVacation(line);
                                line.MostLegs = CalcMostlegsVacation(line);
                                //Trips1Day,Trips2Day,Trips3Day,Trips4Day
                                CalcTripLengthVacation(line);
                                //LegsIn800 ,LegsIn700 ,LegsIn500,LegsIn300 
                                CalcNumLegsOfEachTypeforVacation(line);
                                line.Equip8753 = line.LegsIn700.ToString() + "-" + line.LegsIn800.ToString() + "-" + line.LegsIn200.ToString() + "-" + line.LegsIn600.ToString();
                                line.AcftChanges = CalcNumAcftChangesVacation(line);
                                line.DaysWorkInLine = CalcDaysWorkInLineVacation(line);
                                //line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0.00m : line.AcftChanges / (decimal)line.DaysWorkInLine, 2, MidpointRounding.AwayFromZero);
                                line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0.00m : line.AcftChanges / (decimal)line.DaysWorkInLine, 2);
                                line.AcftChgDay = decimal.Parse(String.Format("{0:0.00}", line.AcftChgDay));
                                line.LastArrTime = CalcLastArrTimeVacation(line);
                                line.LastDomArrTime = CalcLastDomArrTimeVacation(line);
                                line.StartDow = CalcStartDowVacation(line);
                                line.StartDaysList = CalcStartDowListVacation(line);
                                line.StartDaysListPerTrip = CalcStartDowListPerTripVacation(line);
                                line.T234 = CalcT234(line);
                                //  line.VacationDrop = 0.0m;
                                //line.VacationStateLine.VacationDropTfp;
                                line.BlkOfDaysOff = CalcBlkOfDaysOffVacation(line);
                                line.LegsPerDutyPeriod = CalcLegsPerDutyPeriodVacation(line);
                                line.DaysOfWeekWork = CalcWeekDaysWorkVacation(line);
                                line.DaysOfMonthWorks = CalcDaysOfMonthOffVacation(line);
                                CalcOvernightCitiesVacation(line);
                                line.RestPeriods = CalcRestPeriodsVacation(line);
                                CalculateWorkBlockLengthVacation(line);
                                line.EPush = CalcEPushVacation(line).ToString(@"hh\:mm");
                                line.EDomPush = CalcEDomPushVacation(line).ToString(@"hh\:mm");
                                //TotPairings-- calculated value in CalcLegsPerPairVacation() method

                                CalculateWorkBlockVacation(line);
                                line.NightsInMid = line.WorkBlockList.Sum(x => x.nightINDomicile);
                                line.TotalCommutes = line.WorkBlockList.Count;
                                CalculateAverageLatestDomicileArrivalAndPushVacation(line);
                                line.ShowOverLap = false;
                                CalculateHolidayRigVacationProperties(line);
                                line.Tfp += line.HolRig;
                                line.Tfp += line.CFVPay;
                                SetInternationAndNoConusLineVacation(line);
                                line.VacPlusRig = line.VacPayCuBp + line.LineRig;
                                CalcVaPayForaLine(line);
                                if (line.ETOPS)
                                {
                                    line.ETOPSTripsCount = CalculateETOPSTripsCountVacation(line);
                                }
                                CalculateDeadHeadPropertiesforVacation(line);
                                line.TfpPerFltHr = CalcTfpPerFltHrVacation(line);
                                line.TfpPerDay = CalculateTfpPerDay(line);
                                line.TfpPerTafb = CalcTafbTime(line);
                                line.TfpPerDhr = CalcTfpPerDhr(line);
                            }



                        }
                        //if vacation and vacation drop and 
                        else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            if (GlobalSettings.OrderedVacationDays == null || GlobalSettings.OrderedVacationDays.Count == 0)
                            {
                                HandleOnlyFvVacation(line);
                                line.Tfp += line.HolRig;
                                line.Tfp += line.CFVPay;
                            }

                            if (line.VacationStateLine != null)
                            {
                                //=========================================
                                int tafbInBpInt = 0;
                                line.BlkHrsInBp = line.TempBlkHrsInBp;
                                line.DaysOff = line.TempDaysOff;
                                line.FlyPay = line.TempTfp;
                                _flyPayInLine = line.TempTfpInLine;
                                line.Legs = line.TempLegs;
                                line.TafbInBp = line.TempTafbInBp;
                                line.VacPayCuBp = 0;
                                line.VacationDrop = 0;
                                
                                var blk = Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp);
                                var vacblkblkhrs = Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs);
                                if (blk > vacblkblkhrs)
                                {
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));

                                }

                                line.DaysOff += line.VacationStateLine.VacationDaysOff;
                                line.FlyPay = line.VacationStateLine.FlyPay;
                                _flyPayInLine = line.VacationStateLine.FlyPayInLine;
                                line.Legs -= line.VacationStateLine.VacationLegs;
                                tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb);
                                if (tafbInBpInt < 0) tafbInBpInt = 0;
                                line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                                line.VacPayCuBp = line.VacationStateLine.VacationTfp;
                                line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine;
                                line.VacPayNeBp = line.VacPayBothBp - line.VacPayCuBp;

                                line.FlyPay = line.FlyPay + line.LineRig;
                                _flyPayInLine = _flyPayInLine + line.LineRig;

                                line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs));
                                line.DaysOff += line.VacationStateLine.VacationDropDaysOff;
                                line.FlyPay -= line.VacationStateLine.VacationDropTfp;
                                _flyPayInLine -= line.VacationStateLine.VacationDropTfp;
                                line.Legs -= line.VacationStateLine.VacationDropLegs;

                                tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropTafb);
                                if (tafbInBpInt < 0) tafbInBpInt = 0;
                                line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                                //line.Tfp -= line.VacationStateLine.VacationDropTfp;
                                line.TfpInLine -= line.VacationStateLine.VacationDropTfp;
                                line.VacationDrop = line.VacationStateLine.VacationDropTfp;

                                //FV modification
                                //===
                                if (line.FVvacationData != null)
                                {
                                    //For the FV vacation for reserve lines, we can consider each day of the trip as single day trip and we do not need to shift the FV vacation.Also we need to count number of days removed due to the FV vacation and we need to subtract those days tfp values from the flight pay for the reserve line.
                                    var Fvflypay = line.ReserveLine ? line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs) * 6 : line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripTfpInLine);
                                    if (line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Any(y => y.TripStartDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && y.TripEndDate >= GlobalSettings.CurrentBidDetails.BidPeriodEndDate))
                                        line.FlyPay = line.FlyPay - (Fvflypay - (line.TempTfpInLine - line.TempTfp));
                                    else
                                        line.FlyPay = line.FlyPay - Fvflypay;
                                    var fvtrips = line.FVvacationData.Where(x => x.FVEndDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
                                    line.VacPayCuBp = line.VacPayCuBp + fvtrips.Sum(x => x.FVVacTfp);
                                    _flyPayInLine = _flyPayInLine - Fvflypay;
                                    line.DaysOff = line.DaysOff + GeWorkDaysInSideFV(line); ;
                                    line.Legs = line.ReserveLine ? 0 : line.Legs - line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs);
                                    line.VacPayBothBp = line.VacPayBothBp + line.FVvacationData.Sum(x => x.FVVacTfp);

                                    var fvtripdata = line.FVvacationData.SelectMany(x => x.FVVacationTripDatas);
                                    var Vodropvactiontfp = line.FVvacationData.Sum(x => x.VOTfpDrop);
                                    decimal droppedVacationvaluesDuetoFV = line.FVvacationData.Sum(x => x.VDTfpDrop);
                                    int droppedVacationDayDuetoFV = line.FVvacationData.Sum(x => x.VaVacationDropDays);
                                    line.VacPay = line.VacPay - Vodropvactiontfp;
                                    line.VacPayCuBp = line.VacPayCuBp - Vodropvactiontfp;
                                    line.VacPayBothBp = line.VacPayBothBp - Vodropvactiontfp;

                                }
                                line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationFront)), 2);
                                line.VacationOverlapBack = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationStateLine.VacationBack)), 2); ;
                                line.TfpInLine = line.VacPayCuBp + _flyPayInLine;
                                line.Tfp = line.VacPayCuBp + line.FlyPay;
                                line.CarryOverTfp = line.TfpInLine - line.Tfp;
                                if (line.CarryOverTfp < 0 && line.CarryOverTfp > -1.0m)
                                {
                                    line.CarryOverTfp = 0;
                                }

                                line.VacPayCuBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacPayCuBp)), 2);
                                line.FlyPay = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.FlyPay)), 2);

                                line.Tfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.Tfp)), 2);
                                line.CarryOverTfp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.CarryOverTfp)), 2);

                                //=================================

                                line.AMPM = CalcAmPmPropDrop(line);
                                line.AMPMSortOrder = CalcAmPmSortOrder(line);
                                blk = Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine);
                                var vacdrpblkhours=(Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs) + Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs));
                                    if (blk > vacdrpblkhours)
                                {
                                    line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine) - (Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs) + Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs)));
                                }
                                else
                                {
                                    line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(0);

                                }

                                line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                                
                                line.TotDutyPds = CalcTotDutPdsDrop(line, true);
                                line.TotDutyPdsInBp = CalcTotDutPdsDrop(line, false);
                                
                                line.LegsPerDay = CalcLegsPerDay(line);
                                line.LegsPerPair = CalcLegsPerPairDrop(line);
                                line.Weekend = CalcWkEndPropDrop(line);
                                line.DutyHrsInBp = CalcDutyHrsDrop(line, false);
                                line.DutyHrsInLine = CalcDutyHrsDrop(line, true);
                                line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffDrop(line);
                                line.LongestGrndTime = CalcLongGrndTimeVacation(line);
                                line.MostLegs = CalcMostlegsDrop(line);
                                //Trips1Day,Trips2Day,Trips3Day,Trips4Day
                                CalcTripLengthDrop(line);
                                //LegsIn800 ,LegsIn700 ,LegsIn500,LegsIn300 
                                CalcNumLegsOfEachTypeforDrop(line);
                                line.Equip8753 = line.LegsIn700.ToString() + "-" + line.LegsIn800.ToString() + "-" + line.LegsIn200.ToString() + "-" + line.LegsIn600.ToString();
                                line.AcftChanges = CalcNumAcftChangesDrop(line);
                                line.DaysWorkInLine = CalcDaysWorkInLineDrop(line);
                                //line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2, MidpointRounding.AwayFromZero);
                                line.AcftChgDay = Math.Round(line.ReserveLine || line.BlankLine ? 0.0m : (line.DaysWorkInLine == 0) ? 0 : line.AcftChanges / (decimal)line.DaysWorkInLine, 2);
                                line.AcftChgDay = decimal.Parse(String.Format("{0:0.00}", line.AcftChgDay));
                                line.LastArrTime = CalcLastArrTimeDrop(line);
                                line.LastDomArrTime = CalcLastDomArrTimeDrop(line);
                                line.StartDow = CalcStartDowDrop(line);
                                line.StartDaysList = CalcStartDowListDrop(line);
                                line.StartDaysListPerTrip = CalcStartDowListPerTripDrop(line);
                                line.T234 = CalcT234(line);
                                // line.VacationDrop = line.VacationStateLine.VacationDropTfp;
                                line.BlkOfDaysOff = CalcBlkOfDaysOffDrop(line);
                                line.LegsPerDutyPeriod = CalcLegsPerDutyPeriodDrop(line);
                                line.DaysOfWeekWork = CalcWeekDaysWorkDrop(line);
                                line.DaysOfMonthWorks = CalcDaysOfMonthOffDrop(line);
                                CalcOvernightCitiesDrop(line);
                                line.RestPeriods = CalcRestPeriodsDrop(line);
                                CalculateWorkBlockLengthDrop(line);
                                line.EPush = CalcEPushDrop(line).ToString(@"hh\:mm");
                                line.EDomPush = CalcEDomPushDrop(line).ToString(@"hh\:mm");
                                CalculateWorkBlockDrop(line);
                                line.NightsInMid = line.WorkBlockList.Sum(x => x.nightINDomicile);
                                line.TotalCommutes = line.WorkBlockList.Count;
                                CalculateAverageLatestDomicileArrivalAndPushVacationDrop(line);
                                line.ShowOverLap = false;

                                CalculateHolidayRigDropProperties(line);
                                line.Tfp += line.HolRig;
                                line.Tfp += line.CFVPay;
                                SetInternationAndNoConusLineVacationDrop(line);
                                line.VacPlusRig = line.VacPayCuBp + line.LineRig;
                                CalcVaPayForaLine(line);
                                if (line.ETOPS)
                                {
                                    line.ETOPSTripsCount = CalculateETOPSTripsCountVacationDrop(line);
                                }
                                CalculateDeadHeadPropertiesforVacationDrop(line);
                                line.TfpPerFltHr = CalcTfpPerFltHrVacation(line);
                                line.TfpPerDay = CalculateTfpPerDay(line);
                                line.TfpPerTafb = CalcTafbTime(line);
                                line.TfpPerDhr = CalcTfpPerDhr(line);
                            }


                        }
                        //--------------
                        else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            if (line.LineNum == 47)
                            {
                            }
                            if (GlobalSettings.OrderedVacationDays == null || GlobalSettings.OrderedVacationDays.Count == 0)
                            {
                                HandleOnlyFvVacation(line);
                            }
                            if (line.VacationStateLine != null)
                            {
                                line.BlkHrsInBp = line.TempBlkHrsInBp;
                                var blk = Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp);
                                var vacblk = Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs);
                                if (blk > vacblk)
                                {
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));
                                }
                                else
                                {
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(0);

                                }


                                line.FlyPay = line.TempTfp;
                                line.FlyPay = line.VacationStateLine.FlyPay;
                                decimal eomPay = CalcFlyPayEOM(line);
                                line.FlyPay -= eomPay;

                                int tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb);
                                if (tafbInBpInt < 0) tafbInBpInt = 0;
                                line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                                //line.Legs = line.TempLegs;


                                line.VacPayCuBp = line.VacationStateLine.VacationTfp;
                                //line.VacPayBothBp=line.VacationStateLine.VacationTfpInLine;
                                //line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine + CalcvACPayEOMInLine(line);
                                if (isAdajacentEOMandVacation)
                                {
                                    line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine + CalcvACPayEOMInLineForAdjacentEOM(line);
                                }
                                else
                                {
                                    line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine + CalcvACPayEOMInLine(line);
                                }
                                line.VacPayCuBp += eomPay;

                                line.DaysOff = line.TempDaysOff;
                                line.DaysOff += line.VacationStateLine.VacationDaysOff;
                                line.DaysOff += CalDaysOffEOM(line);
                                line.FlyPay = line.FlyPay + line.LineRig;
                                //FV modification
                                //===
                                if (line.FVvacationData != null)
                                {
                                    //For the FV vacation for reserve lines, we can consider each day of the trip as single day trip and we do not need to shift the FV vacation.Also we need to count number of days removed due to the FV vacation and we need to subtract those days tfp values from the flight pay for the reserve line.
                                    var Fvflypay = line.ReserveLine ? line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs) * 6 : line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripTfpInLine);
                                    if (line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Any(y => y.TripStartDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && y.TripEndDate >= GlobalSettings.CurrentBidDetails.BidPeriodEndDate))
                                        line.FlyPay = line.FlyPay - (Fvflypay - (line.TempTfpInLine - line.TempTfp));
                                    else
                                        line.FlyPay = line.FlyPay - Fvflypay;
                                    var fvtrips = line.FVvacationData.Where(x => x.FVEndDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
                                    line.VacPayCuBp = line.VacPayCuBp + fvtrips.Sum(x => x.FVVacTfp);
                                    _flyPayInLine = _flyPayInLine - Fvflypay;
                                    line.DaysOff = line.DaysOff + GeWorkDaysInSideFV(line); ;
                                    line.Legs = line.ReserveLine ? 0 : line.Legs - line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs);
                                    line.VacPayBothBp = line.VacPayBothBp + line.FVvacationData.Sum(x => x.FVVacTfp);

                                }
                                line.VacPayNeBp = line.VacPayBothBp - line.VacPayCuBp;

                                //_flyPayInLine=_flyPayInLine+line.LineRig;
                                line.Tfp = line.VacPayCuBp + line.FlyPay;
                                line.VacationDrop = 0;


                                line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffVacationEOM(line);
                                line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                                
                                CalculateHolidayRigVacationProperties(line);
                               
                                line.ShowOverLap = false;
                                line.VacPlusRig = line.VacPayCuBp + line.LineRig;
                            }
                            line.Tfp += line.HolRig;
                            line.Tfp += line.CFVPay;
                            CalcVaPayForaLine(line);
                            line.TfpPerDay = Math.Round(decimal.Parse(String.Format("{0:0.00}", (line.DaysWork == 0) ? 0.0m : line.Tfp / line.DaysWork)), 2);
                            //line.DaysOff += CalDaysOffEOMDrop(line);

                        }
                        else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            if (line.LineNum == 190)
                            {
                            }
                            if (GlobalSettings.OrderedVacationDays == null || GlobalSettings.OrderedVacationDays.Count == 0)
                            {
                                HandleOnlyFvVacation(line);
                            }
                            if (line.VacationStateLine != null)
                            {
                                line.BlkHrsInBp = line.TempBlkHrsInBp;
                                var blk = Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp);
                                var drpblkhrs = Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs) + Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs);
                                if (blk > drpblkhrs)
                                {
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationBlkHrs));
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropBlkHrs));
                                }
                                else
                                {
                                    line.BlkHrsInBp = Helper.ConvertMinuteToHHMM(0);

                                }

                                line.FlyPay = line.TempTfp;
                                line.FlyPay = line.VacationStateLine.FlyPay;
                                line.FlyPay -= line.VacationStateLine.VacationDropTfp;
                                decimal eomPay = CalcFlyPayEOM(line);
                                line.FlyPay -= eomPay;
                                line.FlyPay = line.FlyPay + line.LineRig;
                                //_flyPayInLine=_flyPayInLine+line.LineRig;
                                CalcFlyPayEOMDrop(line);
                                int tafbInBpInt = Helper.ConvertformattedHhhmmToMinutes(line.TempTafbInBp) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationTafb) - Helper.ConvertformattedHhhmmToMinutes(line.VacationStateLine.VacationDropTafb);
                                if (tafbInBpInt < 0) tafbInBpInt = 0;
                                line.TafbInBp = Helper.ConvertMinuteToHHMM(tafbInBpInt);
                                //line.Legs = line.TempLegs;


                                line.VacPayCuBp = line.VacationStateLine.VacationTfp;
                                //line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine;
                                //line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine + CalcvACPayEOMInLine(line);
                                if (isAdajacentEOMandVacation)
                                {
                                    line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine + CalcvACPayEOMInLineForAdjacentEOM(line);
                                }
                                else
                                {
                                    line.VacPayBothBp = line.VacationStateLine.VacationTfpInLine + CalcvACPayEOMInLine(line);
                                }
                                line.VacPayCuBp += eomPay;
                                line.DaysOff = line.TempDaysOff;
                                line.DaysOff += line.VacationStateLine.VacationDaysOff;
                                line.DaysOff += line.VacationStateLine.VacationDropDaysOff;
                                line.DaysOff += CalDaysOffEOM(line);
                                line.DaysOff += CalDaysOffEOMDrop(line);
                                //FV modification
                                //===
                                if (line.FVvacationData != null)
                                {
                                    //For the FV vacation for reserve lines, we can consider each day of the trip as single day trip and we do not need to shift the FV vacation.Also we need to count number of days removed due to the FV vacation and we need to subtract those days tfp values from the flight pay for the reserve line.
                                    var Fvflypay = line.ReserveLine ? line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs) * 6 : line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripTfpInLine);
                                    if (line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Any(y => y.TripStartDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && y.TripEndDate >= GlobalSettings.CurrentBidDetails.BidPeriodEndDate))
                                        line.FlyPay = line.FlyPay - (Fvflypay - (line.TempTfpInLine - line.TempTfp));
                                    else
                                        line.FlyPay = line.FlyPay - Fvflypay;
                                    var fvtrips = line.FVvacationData.Where(x => x.FVEndDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
                                    line.VacPayCuBp = line.VacPayCuBp + fvtrips.Sum(x => x.FVVacTfp);
                                    _flyPayInLine = _flyPayInLine - Fvflypay;
                                    line.DaysOff = line.DaysOff + GeWorkDaysInSideFV(line); ;
                                    line.Legs = line.ReserveLine ? 0 : line.Legs - line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs);
                                    line.VacPayBothBp = line.VacPayBothBp + line.FVvacationData.Sum(x => x.FVVacTfp);

                                }
                                line.VacPayNeBp = line.VacPayBothBp - line.VacPayCuBp;
                                line.VacationDrop = 0;

                                line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffVacationEOMDrop(line);



                                line.Tfp = line.VacPayCuBp + line.FlyPay;
                                line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                               
                                CalculateHolidayRigVacationEOMDropProperties(line);
                                
                                line.ShowOverLap = false;
                                line.VacPlusRig = line.VacPayCuBp + line.LineRig;

                            }
                            line.Tfp += line.HolRig;
                            line.Tfp += line.CFVPay;
                            CalcVaPayForaLine(line);
                            line.TfpPerDay = Math.Round(decimal.Parse(String.Format("{0:0.00}", (line.DaysWork == 0) ? 0.0m : line.Tfp / line.DaysWork)), 2);
                        }
                        else if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            //=========================================
                            decimal eomPay = CalcFlyPayEOM(line);
                            decimal eomvacationtfp = CalcvACPayEOM(line);
                            line.BlkHrsInBp = line.TempBlkHrsInBp;
                            line.FlyPay = line.TempTfp - eomPay;
                            line.Legs = line.TempLegs;
                            line.TafbInBp = line.TempTafbInBp;
                            line.VacPayCuBp = eomvacationtfp;
                            line.VacPayBothBp = CalcvACPayEOMInLine(line);
                            line.VacPayNeBp = line.VacPayBothBp - line.VacPayCuBp;
                            //line.VacPayNeBp =CalcEOMPayoutsideBidperiod(line);
                            //line.VacPayBothBp = line.VacPayCuBp + line.VacPayNeBp;

                            line.VacationDrop = 0;

                            line.Tfp = line.VacPayCuBp + line.FlyPay;
                            line.DaysOff = line.TempDaysOff;
                            line.LargestBlkOfDaysOff = CalcLargestBlkDaysOff(line);

                            line.VacationOverlapFront = eomvacationtfp;
                            line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationOverlapFront)), 2);
                            line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                           
                            CalculateHolidayRigEOMProperties(line);
                            line.Tfp += line.HolRig;
                            line.ShowOverLap = false;
                            line.VacPlusRig = line.VacPayCuBp + line.LineRig;

                            CalcVaPayForaLine(line);
                            line.TfpPerDay = Math.Round(decimal.Parse(String.Format("{0:0.00}", (line.DaysWork == 0) ? 0.0m : line.Tfp / line.DaysWork)), 2);


                        }
                        else if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            decimal eomvacationtfp = CalcvACPayEOM(line);
                            decimal eomPay = CalcFlyPayEOM(line);

                            line.BlkHrsInBp = line.TempBlkHrsInBp;
                            line.FlyPay = line.TempTfp - eomPay;
                            line.Legs = line.TempLegs;
                            line.TafbInBp = line.TempTafbInBp;
                            line.VacPayCuBp = eomvacationtfp;
                            line.VacationDrop = CalcvACDropTfpEOM(line);
                            CalcFlyPayEOMDrop(line);
                            //line.FlyPay = line.FlyPay+line.LineRig;
                            //_flyPayInLine=_flyPayInLine+line.LineRig;
                            line.VacPayBothBp = CalcvACPayEOMInLine(line);
                            line.VacPayNeBp = line.VacPayBothBp - line.VacPayCuBp;
                            //line.VacPayBothBp = line.VacPayCuBp + line.VacPayNeBp;

                            line.Tfp = line.VacPayCuBp + line.FlyPay;
                            line.DaysOff = line.TempDaysOff;
                            line.DaysOff += CalDaysOffEOM(line);
                            line.DaysOff += CalDaysOffEOMDrop(line);

                            line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffEOMDrop(line);

                            line.VacationOverlapFront = eomvacationtfp;
                            line.VacationOverlapFront = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationOverlapFront)), 2);
                            line.VacationDrop = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.VacationDrop)), 2);
                            line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;
                            

                            CalculateHolidayRigEOMDropProperties(line);
                            line.Tfp += line.HolRig;

                            line.ShowOverLap = false;
                            line.VacPlusRig = line.VacPayCuBp + line.LineRig;
                            CalcVaPayForaLine(line);
                            line.TfpPerDay = Math.Round(decimal.Parse(String.Format("{0:0.00}", (line.DaysWork == 0) ? 0.0m : line.Tfp / line.DaysWork)), 2);
                        }



                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void HandleOnlyFvVacation(Line line)
        {

            line.VacPayCuBp = 0;
            line.VacPayNeBp = 0;
            line.VacPayBothBp = 0;
            line.VacationDrop = 0;
            line.VacationOverlapBack = 0;
            line.VacationOverlapFront = 0;
            line.AMPM = CalcAmPmProp(line);
            line.AMPMSortOrder = CalcAmPmSortOrder(line);
            line.BlkHrsInLine = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.TempBlkHrsInLine));
            line.DaysWork = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1 - line.DaysOff;

            line.BlkHrsInBp = line.TempBlkHrsInBp;
            line.Tfp = line.TempTfp;
            line.DaysOff = line.TempDaysOff;
            line.TafbInBp = line.TempTafbInBp;
            line.Legs = line.TempLegs;
            line.DaysWorkInLine = line.TempDaysWorkInLine;
            line.TfpInLine = line.TempTfpInLine;
            line.BlkHrsInLine = line.TempBlkHrsInLine;
            line.TafbInLine = line.BlankLine || line.ReserveLine ? "0:00" : CalcTafb(line, true);
            line.FlyPay = line.TempTfp;

            if (line.FVvacationData != null)
            {
                var Fvflypay = line.ReserveLine ? line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs) * 6 : line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripTfpInLine);

                //line.FlyPay = line.FlyPay - Fvflypay;

                if (line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Any(y => y.TripStartDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && y.TripEndDate >= GlobalSettings.CurrentBidDetails.BidPeriodEndDate))
                    line.FlyPay = line.FlyPay - (Fvflypay - (line.TempTfpInLine - line.TempTfp));
                else
                    line.FlyPay = line.FlyPay - Fvflypay;
                //line.VacPay = line.VacPay + (FVVacationCount * GlobalSettings.DailyVacPay);
                var fvtrips = line.FVvacationData.Where(x => x.FVEndDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
                line.VacPayCuBp = line.VacPayCuBp + fvtrips.Sum(x => x.FVVacTfp);
                decimal flyPayInLine = line.TempTfp - Fvflypay;
                line.DaysOff = line.DaysOff + GeWorkDaysInSideFV(line);
                line.Legs = line.ReserveLine ? 0 : line.Legs - line.FVvacationData.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripLegs);
                line.TfpInLine = line.VacPayCuBp + _flyPayInLine;
                line.Tfp = line.VacPayCuBp + line.FlyPay;
                line.CarryOverTfp = line.TfpInLine - line.Tfp;
                line.VacPayBothBp = line.FVvacationData.Sum(x => x.FVVacTfp);
            }
        }

        private int GeWorkDaysInSideFV(Line line)
        {
            int workDys = 0;
            foreach (var item in line.FVvacationData.SelectMany(x => x.FVVacationTripDatas))
            {
                List<DateTime> lstwork = getDaysInside(item.TripStartDate, item.TripEndDate);
                workDys += lstwork.Where(x => x <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate).Count();

            }
            return workDys;
        }
        private List<DateTime> getDaysInside(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dates = Enumerable.Range(0, (int)((endDate - startDate).TotalDays) + 1).Select(n => startDate.AddDays(n)).ToList();
            return dates;
        }
        public Line RecalculateAMPMPropertiesAfterAMPMDefenitionChanges(Line line)
        {
            if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                line.AMPM = CalcAmPmProp(line);

            }
            //if vacation  only
            else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                line.AMPM = CalcAmPmPropVacation(line);
            }
            //if vacation and vacation drop and 
            else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsVacationDrop && !GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                line.AMPM = CalcAmPmPropDrop(line);
            }
            line.AMPMSortOrder = CalcAmPmSortOrder(line);
            line.BidLineTemplates = GenerateBidLineViewTemplate(line);
            return line;
        }
        #region MIL
        private decimal CalcTfpPerFltHrVacationforMIL(Line line)
        {
            if (line.ReserveLine || line.BlankLine) return 0.00m;


            string blockHoursInBp = line.BlkHrsInBp.Replace(":", "");
            decimal blkHours = 0.0m;
            decimal tfp;
            // line.block is an int stored as hhmm, except FA could be hhhmm
            try
            {
                if (blockHoursInBp.Length == 5)

                    blkHours = (Convert.ToDecimal(blockHoursInBp.Substring(0, 3)) + Convert.ToDecimal(blockHoursInBp.Substring(3, 2)) / 60m);
                else
                    blkHours = (Convert.ToDecimal(blockHoursInBp.Substring(0, 2)) + Convert.ToDecimal(blockHoursInBp.PadLeft(4, '0').Substring(2, 2)) / 60m);

                tfp = (blkHours == 0) ? 0.00m : line.Tfp / blkHours;

                return Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", tfp)), 2);
            }
            catch (Exception ex)
            {

                throw;
            }
            // return Math.Round(1200m / 100, 2);


        }
        private string CalcDutyHrsForMIL(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            DateTime tripDate = DateTime.MinValue;
            int dpIndex = 0;
            int flightIndex = 0;
            bool isLastTrip = false;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {
                    dpIndex = 0;
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    MILTrip milTrip = null;

                    milTrip = GetMILTrip(pairing);

                    foreach (var dp in trip.DutyPeriods)
                    {

                        if (tripDate <= _bpEndDate)
                        {
                            if (milTrip.DutyPeriodsDetails[dpIndex].Type == "MO")
                            {
                                dutyHrs += dp.DutyTime;
                            }
                            else if (milTrip.DutyPeriodsDetails[dpIndex].Type == "Split")
                            {
                                flightIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (milTrip.DutyPeriodsDetails[dpIndex].FlightDetails[flightIndex].Type == "MD")
                                    {
                                        dutyHrs += flt.Block;

                                    }

                                    flightIndex++;
                                }

                            }

                        }
                        dpIndex++;
                        tripDate = tripDate.AddDays(1);
                    }
                }

                else
                {
                    if (inLine)
                    {
                        dutyHrs += trip.DutyPeriods.Sum(x => x.DutyTime);
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                        dpIndex = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (tripDate <= _bpEndDate)
                            {
                                dutyHrs += dp.DutyTime;
                            }

                            dpIndex++;
                            tripDate = tripDate.AddDays(1);
                        }


                    }
                }
            }

            return (dutyHrs / 60) + ":" + (dutyHrs % 60).ToString().PadLeft(2, '0');
        }

        private int CalcNumAcftChangesMIL(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;
            int dpIndex = 0;
            int flightIndex = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip

                trip = GetTrip(pairing);
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {
                    MILTrip milTrip = null;

                    milTrip = GetMILTrip(pairing);
                    dpIndex = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (milTrip.DutyPeriodsDetails[dpIndex].Type == "MA" || milTrip.DutyPeriodsDetails[dpIndex].Type == "MD")
                        {
                            dpIndex++;
                            continue;
                        }

                        flightIndex = 0;
                        foreach (var flt in dp.Flights)
                        {
                            if (milTrip.DutyPeriodsDetails[dpIndex].FlightDetails[flightIndex].Type == "MA" || milTrip.DutyPeriodsDetails[dpIndex].FlightDetails[flightIndex].Type == "MD")
                            {
                                flightIndex++;
                                continue;
                            }
                            if (flt.AcftChange)
                                numAcftChanges++;
                            flightIndex++;
                        }
                        dpIndex++;
                    }


                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            if (flt.AcftChange == true)
                                numAcftChanges++;
                        }
                    }
                }
            }

            return numAcftChanges;
        }

        private int CalcTotDutPdsForMIL(Line line, bool inLine)
        {
            int totDutPds = 0;
            foreach (var item in line.BidLineTemplates)
            {
                if (inLine)
                {
                    if (item.workStatus == "Work" || item.workStatus == "BackSplitWithoutStrike" || item.workStatus == "FrontSplitWithoutStrike")
                    {
                        totDutPds++;
                    }
                }
                else
                {

                    if ((item.Date) <= _bpEndDate)
                    {
                        if (item.workStatus == "Work" || item.workStatus == "BackSplitWithoutStrike" || item.workStatus == "FrontSplitWithoutStrike")
                        {
                            totDutPds++;
                        }
                    }
                }

            }
            return totDutPds;
        }
        private int CalcDaysWorkInLineForMIL(Line line)
        {
            Trip trip = null;
            int daysWorkInLine = 0;

            foreach (var item in line.BidLineTemplates)
            {

                if (item.workStatus == "Work" || item.workStatus == "BackSplitWithoutStrike" || item.workStatus == "FrontSplitWithoutStrike")
                {
                    daysWorkInLine++;
                }
            }

            return daysWorkInLine;
        }
        private List<int> CalcWeekDaysWorkforMIL(Line line)
        {
            //List<int> weekWorkingDays = new int[Enum.GetNames(typeof(Dow)).Length];
            List<int> weekWorkingDays = new List<int>(new int[Enum.GetNames(typeof(Dow)).Length]);

            int dayOfWeek;

            foreach (var item in line.BidLineTemplates)
            {

                if (item.workStatus == "Work" || item.workStatus == "BackSplitWithoutStrike" || item.workStatus == "FrontSplitWithoutStrike")
                {
                    dayOfWeek = (int)(item.Date).DayOfWeek - 1;
                    dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                    weekWorkingDays[dayOfWeek]++;
                }

            }
            return weekWorkingDays;
        }
        private List<WorkDaysOfBidLine> CalcDaysOfMonthOffForMIL(Line line)
        {
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;

            foreach (var item in line.BidLineTemplates)
            {
                if (item.workStatus == "Work" || item.workStatus == "BackSplitWithoutStrike" || item.workStatus == "FrontSplitWithoutStrike")
                {
                    workDay = new WorkDaysOfBidLine() { DayOfBidline = item.Date, Working = true };
                    daysWork.Add(workDay);
                }
            }

            return daysWork;
        }

        private TimeSpan CalcLastArrTimeForMIL(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            int fltIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {
                    MILTrip milTrip = null;

                    milTrip = GetMILTrip(pairing);
                    dutPd = 0;
                    int tempArrTime = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (milTrip.DutyPeriodsDetails[dutPd].Type == "MO" || milTrip.DutyPeriodsDetails[dutPd].Type == "Split")
                        {
                            fltIndex = 0;
                            foreach (var flt in dp.Flights)
                            {
                                if (milTrip.DutyPeriodsDetails[dutPd].Type == "MO")
                                {
                                    tempArrTime = dp.Flights[fltIndex].ArrTime - 1440 * dutPd;
                                }
                                fltIndex++;
                            }
                            arrTime = (tempArrTime > arrTime) ? tempArrTime : arrTime;

                        }

                        dutPd++;
                    }
                }
                else
                {
                    dutPd = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastFlt = dp.Flights.Count - 1;
                        arrTime = (dp.Flights[lastFlt].ArrTime - 1440 * dutPd > arrTime) ? dp.Flights[lastFlt].ArrTime - dutPd * 1440 : arrTime;
                        dutPd++;
                    }
                }
            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);


        }
        private TimeSpan CalcLastDomArrTimeForMIL(Line line)
        {
            Trip trip = null;

            int lastDomArr = 0;
            int tempDomArr = 0;
            int dpIndex = 0;
            int flightIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {
                    MILTrip milTrip = null;

                    milTrip = GetMILTrip(pairing);


                    dpIndex = 0;

                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (milTrip.DutyPeriodsDetails[dpIndex].Type == "MO" || milTrip.DutyPeriodsDetails[dpIndex].Type == "Split")
                        {
                            flightIndex = 0;
                            foreach (var flt in dp.Flights)
                            {
                                if (milTrip.DutyPeriodsDetails[dpIndex].FlightDetails[flightIndex].Type == "MO" && flt.ArrSta == GlobalSettings.CurrentBidDetails.Domicile)
                                {
                                    tempDomArr = trip.DutyPeriods[dpIndex].Flights[flightIndex].ArrTime - dpIndex * 1440;
                                }
                                flightIndex++;
                            }
                        }


                        dpIndex++;
                    }

                    lastDomArr = (tempDomArr > lastDomArr) ? tempDomArr : lastDomArr;
                }
                else
                {

                    int lastDp = trip.DutyPeriods.Count - 1;
                    int lastFlt = trip.DutyPeriods[lastDp].Flights.Count - 1;
                    lastDomArr = (trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 > lastDomArr) ?
                        trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 : lastDomArr;
                }
            }

            int hours = lastDomArr / 60;
            int minutes = lastDomArr % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);
        }
        private string CalcStartDowMIL(Line line)
        {

            DateTime previousdpdate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            string sdow = string.Empty;
            int startDowInt = 0;
            int oldDowInt = 9;

            var work = line.BidLineTemplates.Where(x => x.workStatus == "Work" || x.workStatus == "BackSplitWithoutStrike" || x.workStatus == "FrontSplitWithoutStrike" || x.workStatus == "BackSplitWork" || x.workStatus == "FrontSplitWork");
            var firsttrip = work.FirstOrDefault();
            if (firsttrip != null)
            {
                tripStartDate = firsttrip.Date;
                previousdpdate = tripStartDate.AddDays(-2);
                foreach (BidLineTemplate template in work)
                {
                    var tempdate = template.Date;
                    if (previousdpdate.AddDays(1) != tempdate)
                    {
                        tripStartDate = tempdate;
                        startDowInt = Convert.ToInt32(tripStartDate.DayOfWeek);
                        oldDowInt = (oldDowInt == 9) ? startDowInt : oldDowInt;
                    }

                    if (startDowInt != oldDowInt)
                    {
                        return "mix";
                    }
                    previousdpdate = tempdate;
                }
                switch (startDowInt)
                {
                    case 0:
                        sdow = Dow.Sun.ToString();
                        break;
                    case 1:
                        sdow = Dow.Mon.ToString();
                        break;
                    case 2:
                        sdow = Dow.Tue.ToString();
                        break;
                    case 3:
                        sdow = Dow.Wed.ToString();
                        break;
                    case 4:
                        sdow = Dow.Thu.ToString();
                        break;
                    case 5:
                        sdow = Dow.Fri.ToString();
                        break;
                    case 6:
                        sdow = Dow.Sat.ToString();
                        break;
                    default:
                        break;
                }
            }
            return sdow;
        }
        private List<int> CalcStartDowListForMIL(Line line)
        {
            var startdays = new List<int>(new int[7]);
            DateTime previousdpdate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            string sdow = string.Empty;
            int startDowInt = 0;
            int oldDowInt = 9;

            var work = line.BidLineTemplates.Where(x => x.workStatus == "Work" || x.workStatus == "BackSplitWithoutStrike" || x.workStatus == "FrontSplitWithoutStrike" || x.workStatus == "BackSplitWork" || x.workStatus == "FrontSplitWork");
            var firstTrip = work.FirstOrDefault();
            if (firstTrip != null)
            {
                tripStartDate = firstTrip.Date;
                previousdpdate = tripStartDate.AddDays(-2);
                foreach (BidLineTemplate template in work)
                {
                    var tempdate = template.Date;
                    if (previousdpdate.AddDays(1) != tempdate)
                    {
                        tripStartDate = tempdate;
                        startDowInt = Convert.ToInt32(tripStartDate.DayOfWeek);
                        oldDowInt = (oldDowInt == 9) ? startDowInt : oldDowInt;

                        switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                        {
                            case 0:
                                startdays[6]++; //sun
                                break;
                            case 1:
                                startdays[0]++; //Mon
                                break;
                            case 2:
                                startdays[1]++;
                                break;
                            case 3:
                                startdays[2]++;
                                break;
                            case 4:
                                startdays[3]++;
                                break;
                            case 5:
                                startdays[4]++;
                                break;
                            case 6:
                                startdays[5]++;
                                break;
                        }
                    }
                    //if (tripStartDate.AddDays(1) != tempdate)
                    // {


                    // }
                    previousdpdate = tempdate;
                }
            }
            return startdays;
        }
        private List<int> CalcStartDowListPerTripForMIL(Line line)
        {
            var startdays = new List<int>(new int[7]);
            //DateTime previousdpdate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            string sdow = string.Empty;
            int startDowInt = 0;
            int oldDowInt = 9;

            var work = line.BidLineTemplates.Where(x => x.workStatus == "Work" || x.workStatus == "BackSplitWithoutStrike" || x.workStatus == "FrontSplitWithoutStrike" || x.workStatus == "BackSplitWork" || x.workStatus == "FrontSplitWork");
            var firstTrip = work.FirstOrDefault();
            if (firstTrip != null)
            {
                tripStartDate = firstTrip.Date;

                foreach (BidLineTemplate template in work)
                {
                    var tempdate = template.Date;

                    tripStartDate = tempdate;
                    startDowInt = Convert.ToInt32(tripStartDate.DayOfWeek);
                    oldDowInt = (oldDowInt == 9) ? startDowInt : oldDowInt;

                    switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                    {
                        case 0:
                            startdays[6]++; //sun
                            break;
                        case 1:
                            startdays[0]++; //Mon
                            break;
                        case 2:
                            startdays[1]++;
                            break;
                        case 3:
                            startdays[2]++;
                            break;
                        case 4:
                            startdays[3]++;
                            break;
                        case 5:
                            startdays[4]++;
                            break;
                        case 6:
                            startdays[5]++;
                            break;
                    }
                }


            }
            return startdays;
        }
        private List<int> CalcLegsPerDutyPeriodForMIL(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>(new int[10]);
            int dpIndex = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {

                    MILTrip milTrip = null;

                    milTrip = GetMILTrip(pairing);

                    dpIndex = 0;

                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (milTrip.DutyPeriodsDetails[dpIndex].Type == "MO" || milTrip.DutyPeriodsDetails[dpIndex].Type == "Split")
                        {
                            arrayOfDeadheads[milTrip.DutyPeriodsDetails[dpIndex].FlightDetails.Where(x => x.Type == "MO").Count()]++;
                        }

                        dpIndex++;
                    }
                }

                else
                {
                    foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                    {
                        arrayOfDeadheads[dutyPeriod.TotFlights]++;
                    }
                }



            }

            return arrayOfDeadheads;
        }

        private int CalcTotPairingsForMIL(Line line)
        {

            Trip trip = null;
            int pairingcount = 0;
            int dpIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {
                    MILTrip milTrip = null;
                    milTrip = GetMILTrip(pairing);
                    dpIndex = 0;
                    bool prevdpdroped = false;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (dp.DutPerSeqNum == 1 && (milTrip.DutyPeriodsDetails[dpIndex].Type == "MO" || milTrip.DutyPeriodsDetails[dpIndex].Type == "Split") || (prevdpdroped && (milTrip.DutyPeriodsDetails[dpIndex].Type == "MO" || milTrip.DutyPeriodsDetails[dpIndex].Type == "Split")))
                        {
                            pairingcount++;
                        }
                        else if (milTrip.DutyPeriodsDetails[dpIndex].Type == "MA")
                        {
                            prevdpdroped = true;
                        }
                        dpIndex++;
                    }
                }
                else
                {
                    pairingcount++;
                }
            }
            return pairingcount;
        }
        private decimal CalcLegsPerPairForMIL(Line line)
        {

            decimal legsPerPair = 0.0m;
            if (line.TotPairings > 0)
            {
                legsPerPair = Math.Round((Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.TotPairings)), 2);
            }
            return legsPerPair;


        }
        private void CalcNumLegsOfEachTypeforMIL(Line line)
        {

            line.LegsIn800 = 0;
            line.LegsIn700 = 0;
            //line.LegsIn500 = 0;
            //line.LegsIn300 = 0;
            line.LegsIn600 = 0;
            line.LegsIn200 = 0;
            bool isLastTrip = false;
            int paringCount = 0;
            Trip trip = null;


            foreach (string pairing in line.Pairings)
            {


                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                if (trip == null) return;
                if (!trip.ReserveTrip)
                {
                    //Checking if the trip is MIL effected trip
                    if (_mILTripsOfSingleLine.ContainsKey(pairing))
                    {

                        MILTrip milTrip = GetMILTrip(pairing);
                        foreach (var dp in trip.DutyPeriods)
                        {
                            MILDutyPeriod mDp = milTrip.DutyPeriodsDetails.FirstOrDefault(x => x.DpSeqNum == dp.DutPerSeqNum);

                            if (mDp.Type == "MO" || mDp.Type == "Split")
                            {
                                foreach (var flt in dp.Flights)
                                {
                                    MILFlights mFlt = mDp.FlightDetails.FirstOrDefault(x => x.FltSeqNum == flt.FlightSeqNum);
                                    if (flt.Equip != null)
                                    {
                                        if (flt.Equip.Length > 0)
                                        {
                                            if (mFlt.Type == "MO")
                                            {
                                                if (flt.Equip.Length > 0)
                                                {
                                                    switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                                    {
                                                        case "8":
                                                            line.LegsIn800++;
                                                            break;
                                                        case "7":
                                                            line.LegsIn700++;
                                                            break;
                                                        //case "5":
                                                        //  line.LegsIn500++;
                                                        //  break;
                                                        //case "3":
                                                        //  line.LegsIn300++;
                                                        //  break;
                                                        case "6":
                                                            line.LegsIn600++;
                                                            break;
                                                        case "2":
                                                            line.LegsIn200++;
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

                    //normal Trip
                    //-------------------------------------------------------------------------------------------------------------------
                    else
                    {
                        foreach (var dp in trip.DutyPeriods)
                        {
                            foreach (var flt in dp.Flights)
                            {
                                if (flt.Equip != null)
                                {
                                    if (flt.Equip.Length > 0)
                                    {
                                        switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                        {
                                            case "8":
                                                line.LegsIn800++;
                                                break;
                                            case "7":
                                                line.LegsIn700++;
                                                break;
                                            //case "5":
                                            //  line.LegsIn500++;
                                            //  break;
                                            //case "3":
                                            //  line.LegsIn300++;
                                            //  break;
                                            case "6":
                                                line.LegsIn600++;
                                                break;
                                            case "2":
                                                line.LegsIn200++;
                                                break;
                                        }
                                    }

                                }
                            }

                        }

                    }
                    //-------------------------------------------------------------------------------------------------------------------

                }
            }




        }
        private void CalcTripLengthforMIL(Line line)
        {
            Trip trip = null;
            int tripLength = 0;

            line.Trips1Day = 0;
            line.Trips2Day = 0;
            line.Trips3Day = 0;
            line.Trips4Day = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {

                    MILTrip milTrip = GetMILTrip(pairing);
                    tripLength = milTrip.DutyPeriodsDetails.Where(x => x.Type == "Split" || x.Type == "MO").Count();
                }
                else
                {
                    tripLength = trip.PairLength;
                }

                switch (tripLength)
                {
                    case 1:
                        line.Trips1Day++;
                        break;
                    case 2:
                        line.Trips2Day++;
                        break;
                    case 3:
                        line.Trips3Day++;
                        break;
                    case 4:
                        line.Trips4Day++;
                        break;



                }
            }


        }
        private int CalcMostlegsForMIL(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;
            int legsintrip = 0;
            int mostlegs = 0;

            Trip trip = null;

            int dpIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {
                    MILTrip milTrip = null;
                    milTrip = GetMILTrip(pairing);
                    dpIndex = 0;
                    bool prevdpdroped = false;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (dp.DutPerSeqNum == 1 && (milTrip.DutyPeriodsDetails[dpIndex].Type == "MO" || milTrip.DutyPeriodsDetails[dpIndex].Type == "Split") || (prevdpdroped && (milTrip.DutyPeriodsDetails[dpIndex].Type == "MO" || milTrip.DutyPeriodsDetails[dpIndex].Type == "Split")))
                        {
                            legsintrip = 0;
                            legsintrip = milTrip.DutyPeriodsDetails.SelectMany(x => x.FlightDetails).Where(x => x.Type == "MO").Count();
                        }
                        else if (milTrip.DutyPeriodsDetails[dpIndex].Type == "MA")
                        {
                            if (mostlegs < legsintrip)
                            {
                                mostlegs = legsintrip;
                            }
                            prevdpdroped = true;
                        }
                        else
                        {
                            legsintrip += milTrip.DutyPeriodsDetails.SelectMany(x => x.FlightDetails).Where(x => x.Type == "MO").Count();
                        }
                        dpIndex++;
                    }
                    if (mostlegs < legsintrip)
                    {
                        mostlegs = legsintrip;
                    }
                }
                else
                {
                    legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                    if (mostlegs < legsintrip)
                    {
                        mostlegs = legsintrip;
                    }
                }
            }
            return mostlegs;


        }
        #endregion

        #region Private Methods

        #region ETOPS
        private bool CalculateETOPSProperties(Line line)
        {
            Trip trip = null;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                if (trip.DutyPeriods.SelectMany(x => x.Flights).Any(y => y.ETOPS))
                {
                    if (line.LineDisplay.EndsWith("e") == false)
                    {
                        line.LineDisplay += "e";
                    }
                    return true;
                }

            }

            return false;
        }
        private int CalculateETOPSTripsCount(Line line)
        {

            Trip trip = null;
            int etopsCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (trip.DutyPeriods.SelectMany(x => x.Flights).Any(y => y.ETOPS))
                {
                    etopsCount++;
                }
            }
            return etopsCount;

        }
        private int CalculateETOPSTripsCountVacation(Line line)
        {

            Trip trip = null;
            int etopsCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    int dpIndex = 0;
                    int flightIndex = 0;

                    bool isetops = false;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            flightIndex = 0;
                            if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType != "VD" && flt.ETOPS == true)
                            {
                                flightIndex++;
                                isetops = true;
                                break;
                            }

                        }
                        if (isetops)
                            break;

                        dpIndex++;
                    }
                    if (isetops)
                        etopsCount++;
                }

                else
                {
                    if (trip.DutyPeriods.SelectMany(x => x.Flights).Any(y => y.ETOPS))
                    {
                        etopsCount++;
                    }
                }

            }
            return etopsCount;

        }
        private int CalculateETOPSTripsCountVacationDrop(Line line)
        {

            Trip trip = null;
            int etopsCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                }
                else
                {
                    if (trip.DutyPeriods.SelectMany(x => x.Flights).Any(y => y.ETOPS))
                    {
                        etopsCount++;
                    }
                }
            }
            return etopsCount;

        }
        #endregion
        #region Deadheads
        private void CalculateDeadHeadProperties(Line line)
        {
            Trip trip = null;
            int dhfirstcount = 0;
            int dhlastCount = 0;
            int dhtotal = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                dhfirstcount = dhfirstcount + ((trip.DutyPeriods[0].Flights[0].DeadHead) ? 1 : 0);
                DutyPeriod lastdp = trip.DutyPeriods[trip.DutyPeriods.Count - 1];
                dhlastCount = dhlastCount + (lastdp.Flights[lastdp.Flights.Count - 1].DeadHead ? 1 : 0);
                dhtotal = dhtotal + trip.DutyPeriods.SelectMany(x => x.Flights).Where(y => y.DeadHead).Count();
            }
            line.DhFirstTotal = dhfirstcount;
            line.DhLastTotal = dhlastCount;
            line.DhTotal = dhtotal;
        }
        private void CalculateDeadHeadPropertiesforVacation(Line line)
        {
            try
            {
                Trip trip = null;
                int dhfirstcount = 0;
                int dhlastCount = 0;
                int dhtotal = 0;
                foreach (var pairing in line.Pairings)
                {

                    //Get trip
                    trip = GetTrip(pairing);
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }

                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            int dpIndex = 0;
                            int flightIndex = 0;

                            int firstDPCount = 0;
                            int lastdpCount = 0;
                            int firstFlightCount = 0;
                            int lastFlightCount = 0;
                            //logc: We need to consider only VD and Split dutyperiods becuase "VD" flight will comes only on this dutyperiods.
                            //Then we need to identify the Start and end of dutyperiod and flight details for the VD flights

                            var SplitandVDDutyperiods = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split");

                            firstDPCount = SplitandVDDutyperiods.FirstOrDefault().DutyperidSeqNo - 1;
                            if (SplitandVDDutyperiods.FirstOrDefault().DutyPeriodType == "Split")
                                firstFlightCount = SplitandVDDutyperiods.FirstOrDefault().VacationFlights.Where(x => x.FlightType == "VD").FirstOrDefault().FlightSeqNo - 1;
                            else
                                firstFlightCount = 0;

                            lastdpCount = SplitandVDDutyperiods.LastOrDefault().DutyperidSeqNo - 1;
                            if (SplitandVDDutyperiods.LastOrDefault().DutyPeriodType == "Split")
                                lastFlightCount = SplitandVDDutyperiods.LastOrDefault().VacationFlights.Where(x => x.FlightType == "VD").LastOrDefault().FlightSeqNo - 1;
                            else
                                lastFlightCount = SplitandVDDutyperiods.LastOrDefault().VacationFlights.Count - 1;


                            if (trip.DutyPeriods[firstDPCount].Flights[firstFlightCount].DeadHead)
                            {
                                dhfirstcount++;
                            }
                            if (trip.DutyPeriods[lastdpCount].Flights[lastFlightCount].DeadHead)
                            {
                                dhlastCount++;
                            }
                            foreach (var dp in vacTrip.VacationDutyPeriods)
                            {
                                flightIndex = 0;
                                foreach (var flt in dp.VacationFlights)
                                {
                                    if (flt.FlightType == "VD")
                                    {
                                        if (trip.DutyPeriods[dpIndex].Flights[flightIndex].DeadHead)
                                        {
                                            dhtotal++;
                                        }
                                    }
                                    flightIndex++;
                                }
                                dpIndex++;
                            }
                        }

                    }
                    else
                    {
                        dhfirstcount = dhfirstcount + ((trip.DutyPeriods[0].Flights[0].DeadHead) ? 1 : 0);
                        DutyPeriod lastdp = trip.DutyPeriods[trip.DutyPeriods.Count - 1];
                        dhlastCount = dhlastCount + (lastdp.Flights[lastdp.Flights.Count - 1].DeadHead ? 1 : 0);
                        dhtotal = dhtotal + trip.DutyPeriods.SelectMany(x => x.Flights).Where(y => y.DeadHead).Count();
                    }

                }
                line.DhFirstTotal = dhfirstcount;
                line.DhLastTotal = dhlastCount;
                line.DhTotal = dhtotal;
            }
            catch (Exception ex)
            {

            }
        }
        private void CalculateDeadHeadPropertiesforVacationDrop(Line line)
        {
            Trip trip = null;
            int dhfirstcount = 0;
            int dhlastCount = 0;
            int dhtotal = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }

                if (vacTrip != null)
                {

                }
                else
                {
                    dhfirstcount = dhfirstcount + ((trip.DutyPeriods[0].Flights[0].DeadHead) ? 1 : 0);
                    DutyPeriod lastdp = trip.DutyPeriods[trip.DutyPeriods.Count - 1];
                    dhlastCount = dhlastCount + (lastdp.Flights[lastdp.Flights.Count - 1].DeadHead ? 1 : 0);
                    dhtotal = dhtotal + trip.DutyPeriods.SelectMany(x => x.Flights).Where(y => y.DeadHead).Count();
                }

            }
            line.DhFirstTotal = dhfirstcount;
            line.DhLastTotal = dhlastCount;
            line.DhTotal = dhtotal;
        }
        #endregion

        #region Weekends
        private string CalcWkEndProp(Line line)
        {
            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;
            int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                tripLength = trip.PairLength;
                tripDate = DateTime.MinValue;
                dayOfWeek = 0;
                tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                for (int index = 0; index < tripLength; index++)
                {

                    dayOfWeek = (int)(tripDate.AddDays(index).DayOfWeek);
                    // 0 = Sunday, 6 = Saturday
                    if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        wkEndCount++;
                    }
                    totDays++;
                }
            }


            if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend)
            {
                int maxNumber = int.Parse(GlobalSettings.WBidINIContent.Week.MaxNumber);
                wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

            }
            else
            {
                int maxPercentage = int.Parse(GlobalSettings.WBidINIContent.Week.MaxPercentage);
                wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
            }


            return wkDayWkEnd;
        }

        public string CalcWkEndPropVacation(Line line)
        {
            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;
            int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                    tripDate = vacTrip.TripVacationStartDate;
                    tripLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                }
                else
                {
                    tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                    tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                    tripLength = trip.PairLength;
                }

                dayOfWeek = 0;
                for (int index = 0; index < tripLength; index++)
                {

                    dayOfWeek = (int)(tripDate.AddDays(index).DayOfWeek);
                    if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        wkEndCount++;
                    }
                    totDays++;
                }
            }


            if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend)
            {
                int maxNumber = int.Parse(GlobalSettings.WBidINIContent.Week.MaxNumber);
                wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

            }
            else
            {
                int maxPercentage = int.Parse(GlobalSettings.WBidINIContent.Week.MaxPercentage);
                wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
            }


            return wkDayWkEnd;
        }

        public string CalcWkEndPropDrop(Line line)
        {
            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;
            int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;

                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                    tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                    tripLength = trip.PairLength;
                }

                dayOfWeek = 0;
                for (int index = 0; index < tripLength; index++)
                {

                    dayOfWeek = (int)(tripDate.AddDays(index).DayOfWeek);
                    if (dayOfWeek == 0 || dayOfWeek == 6)
                    {
                        wkEndCount++;
                    }
                    totDays++;
                }
            }


            if (GlobalSettings.WBidINIContent.Week.IsMaxWeekend)
            {
                int maxNumber = int.Parse(GlobalSettings.WBidINIContent.Week.MaxNumber);
                wkDayWkEnd = (wkEndCount > maxNumber) ? "WKEND" : "WKDAY";

            }
            else
            {
                int maxPercentage = int.Parse(GlobalSettings.WBidINIContent.Week.MaxPercentage);
                wkDayWkEnd = (totDays == 0) ? "WKDAY" : (((((decimal)wkEndCount) / totDays) * 100) > maxPercentage) ? "WKEND" : "WKDAY";
            }


            return wkDayWkEnd;
        }
        #endregion

        #region AMPM
        private string CalcAmPmProp(Line line)
        {
            if (line.BlankLine) return "---";

            // initialize
            string ampm = "AM";
            Trip trip = null;
            _amPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
            int howCalc = _amPmConfigure.HowCalcAmPm;
            int amPush = Convert.ToInt32(_amPmConfigure.AmPush.TotalMinutes);
            int amLand = Convert.ToInt32(_amPmConfigure.AmLand.TotalMinutes);
            int pmPush = Convert.ToInt32(_amPmConfigure.PmPush.TotalMinutes);
            int pmLand = Convert.ToInt32(_amPmConfigure.PmLand.TotalMinutes);
            pmLand = pmLand < amLand ? pmLand + 1440 : pmLand;
            int ntePush = Convert.ToInt32(_amPmConfigure.NitePush.TotalMinutes);
            ntePush = ntePush < pmPush ? ntePush + 1440 : ntePush;
            int nteLand = Convert.ToInt32(_amPmConfigure.NiteLand.TotalMinutes);
            nteLand = nteLand + 1440;
            int amCentroid = (amPush + amLand) / 2;
            int pmCentroid = (pmPush + pmLand) / 2;
            int nteCentroid = (ntePush + nteLand) / 2;
            int numOrPct = _amPmConfigure.NumberOrPercentageCalc;
            int numDiff = _amPmConfigure.NumOpposites;
            decimal pctDiff = _amPmConfigure.PctOpposities / 100m;
            int amTermCnt, amPushCnt, pmTermCnt, pmPushCnt, nteTermCnt, ntePushCnt, unknownTerm, unknownPush, amCentCnt, pmCentCnt, nteCentCnt, case2AmCnt, case2PmCnt, case2NteCnt, case2MixCnt;
            amTermCnt = amPushCnt = pmTermCnt = pmPushCnt = nteTermCnt = ntePushCnt = unknownTerm = unknownPush = amCentCnt = pmCentCnt = nteCentCnt = case2AmCnt = case2PmCnt = case2NteCnt = case2MixCnt = 0;


            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);


                int dpCount = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    if (line.ReserveLine == true)
                    {
                        int reservePush = dp.ReserveOut % 1440;
                        if (reservePush < GlobalSettings.ReserveAmPmClassification)
                        {
                            amTermCnt++;
                            case2AmCnt++;
                            amCentCnt++;
                            trip.AmPm = "1";
                        }
                        else
                        {
                            pmTermCnt++;
                            case2PmCnt++;
                            pmCentCnt++;
                            trip.AmPm = "2";
                        }
                    }
                    else
                    {
                        int landMinutes = dp.Flights[dp.Flights.Count - 1].ArrTime - dpCount * 1440;

                        if (landMinutes < amLand)
                        {
                            if (howCalc == 1) trip.AmPm = "1";
                            amTermCnt++;
                        }
                        else if (landMinutes < pmLand)
                        {
                            if (howCalc == 1) trip.AmPm = "2";
                            pmTermCnt++;
                        }
                        else
                        {
                            if (howCalc == 1)
                                trip.AmPm = "3";
                            nteTermCnt++;
                        }

                        int pushMinutes = dp.Flights[0].DepTime - dpCount * 1440;

                        if (pushMinutes > ntePush) ntePushCnt++;
                        else if (pushMinutes > pmPush) pmPushCnt++;
                        else amPushCnt++;

                        if (pushMinutes > amPush && landMinutes < amLand)
                        {
                            if (howCalc == 2) trip.AmPm = "1";
                            case2AmCnt++;

                        }
                        else if (pushMinutes > pmPush && landMinutes < pmLand)
                        {
                            if (howCalc == 2) trip.AmPm = "2";
                            case2PmCnt++;
                        }
                        else if (pushMinutes > ntePush && landMinutes < nteLand)
                        {
                            if (howCalc == 2)
                                trip.AmPm = "3";
                            case2NteCnt++;
                        }
                        else
                        {
                            if (howCalc == 2)
                                trip.AmPm = "3";
                            case2MixCnt++;
                        }

                        int centroid = (pushMinutes + landMinutes) / 2;

                        if (centroid < amCentroid)
                        {
                            if (howCalc == 3)
                                trip.AmPm = "1";
                            amCentCnt++;
                        }

                        else if (centroid < pmCentroid)
                        {
                            if (centroid - amCentroid < pmCentroid - centroid)
                            {
                                if (howCalc == 3)
                                    trip.AmPm = "1";
                                amCentCnt++;
                            }
                            else
                            {
                                if (howCalc == 3)
                                    trip.AmPm = "2";
                                pmCentCnt++;
                            }
                        }
                        else if (centroid - pmCentroid < nteCentroid - centroid)
                        {
                            if (howCalc == 3)
                                trip.AmPm = "2";
                            pmCentCnt++;
                        }
                        else
                        {
                            if (howCalc == 3)
                                trip.AmPm = "3";
                            nteCentCnt++;
                        }


                        dpCount++;
                    }
                }

            }

            int totalTerm = amTermCnt + pmTermCnt + nteTermCnt;
            int totalCentCnt = amCentCnt + pmCentCnt + nteCentCnt;
            int totalCase2Cnt = case2AmCnt + case2PmCnt + case2NteCnt + case2MixCnt;



            switch (howCalc)
            {
                case 1:     // AM-Terminate/PM-Arrival
                    // AM terminates before amLand and pushes before pmPush
                    // PM terminates before pmLand and pushes before ntePush
                    // NTE terminates before nteLand and pushes after ntePush
                    // Mix is none of the above

                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalTerm - numDiff < amTermCnt) return "AM";
                            else if (totalTerm - numDiff < pmTermCnt) return " PM";
                            else if (totalTerm - numDiff < nteTermCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (1 - amTermCnt / (decimal)totalTerm < pctDiff) return "AM";
                            else if (1 - pmTermCnt / (decimal)totalTerm < pctDiff) return " PM";
                            else if (1 - nteTermCnt / (decimal)totalTerm < pctDiff) return "NTE";
                            else return "Mix";
                    }


                    break;
                case 2:     // AM/PM Terminate/Push  -- handled in Case 1 -- good enough for now

                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCase2Cnt - numDiff < case2AmCnt) return "AM";
                            else if (totalCase2Cnt - numDiff < case2PmCnt) return " PM";
                            else if (totalCase2Cnt - numDiff < case2NteCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (1 - case2AmCnt / (decimal)totalCase2Cnt < pctDiff) return "AM";
                            else if (1 - case2PmCnt / (decimal)totalCase2Cnt < pctDiff) return " PM";
                            else if (1 - case2NteCnt / (decimal)totalCase2Cnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;

                case 3:     // Banded Centroid
                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCentCnt - numDiff < amCentCnt) return "AM";
                            else if (totalCentCnt - numDiff < pmCentCnt) return " PM";
                            else if (totalCentCnt - numDiff < nteCentCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (1 - amCentCnt / (decimal)totalCentCnt < pctDiff) return "AM";
                            else if (1 - pmCentCnt / (decimal)totalCentCnt < pctDiff) return " PM";
                            else if (1 - nteCentCnt / (decimal)totalCentCnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;
            }

            return ampm;
        }

        private string CalcAmPmPropVacation(Line line)
        {


            try
            {


                if (line.BlankLine) return "---";

                // initialize
                string ampm = "AM";
                Trip trip = null;
                _amPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
                int howCalc = _amPmConfigure.HowCalcAmPm;
                int amPush = Convert.ToInt32(_amPmConfigure.AmPush.TotalMinutes);
                int amLand = Convert.ToInt32(_amPmConfigure.AmLand.TotalMinutes);
                int pmPush = Convert.ToInt32(_amPmConfigure.PmPush.TotalMinutes);
                int pmLand = Convert.ToInt32(_amPmConfigure.PmLand.TotalMinutes);
                pmLand = pmLand < amLand ? pmLand + 1440 : pmLand;
                int ntePush = Convert.ToInt32(_amPmConfigure.NitePush.TotalMinutes);
                ntePush = ntePush < pmPush ? ntePush + 1440 : ntePush;
                int nteLand = Convert.ToInt32(_amPmConfigure.NiteLand.TotalMinutes);
                nteLand = nteLand + 1440;
                int amCentroid = (amPush + amLand) / 2;
                int pmCentroid = (pmPush + pmLand) / 2;
                int nteCentroid = (ntePush + nteLand) / 2;
                int numOrPct = _amPmConfigure.NumberOrPercentageCalc;
                int numDiff = _amPmConfigure.NumOpposites;
                decimal pctDiff = _amPmConfigure.PctOpposities / 100m;
                int amTermCnt, amPushCnt, pmTermCnt, pmPushCnt, nteTermCnt, ntePushCnt, unknownTerm, unknownPush, amCentCnt, pmCentCnt, nteCentCnt, case2AmCnt, case2PmCnt, case2NteCnt, case2MixCnt;
                amTermCnt = amPushCnt = pmTermCnt = pmPushCnt = nteTermCnt = ntePushCnt = unknownTerm = unknownPush = amCentCnt = pmCentCnt = nteCentCnt = case2AmCnt = case2PmCnt = case2NteCnt = case2MixCnt = 0;
                int tripDay, tripLength;
                DateTime tripDate = DateTime.MinValue;

                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {

                    //Get trip
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }

                        tripDate = vacTrip.TripVacationStartDate;
                        tripLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                    }
                    else
                    {
                        tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                        //tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), tripDay);
                        tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
                        tripLength = trip.PairLength;
                    }


                    int dpCount = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (vacTrip != null)
                        {
                            if (vacTrip.VacationDutyPeriods[dpCount].DutyPeriodType == "VO" || vacTrip.VacationDutyPeriods[dpCount].DutyPeriodType == "VA")
                            {
                                dpCount++;
                                continue;
                            }
                        }

                        if (line.ReserveLine == true)
                        {
                            int reservePush = dp.ReserveOut % 1440;
                            if (reservePush < GlobalSettings.ReserveAmPmClassification)
                            {
                                amTermCnt++;
                                case2AmCnt++;
                                amCentCnt++;
                                trip.AmPm = "1";
                            }
                            else
                            {
                                pmTermCnt++;
                                case2PmCnt++;
                                pmCentCnt++;
                                trip.AmPm = "2";
                            }
                        }
                        else
                        {
                            int landMinutes = dp.Flights[dp.Flights.Count - 1].ArrTime - dpCount * 1440;

                            if (landMinutes < amLand)
                            {
                                if (howCalc == 1) trip.AmPm = "1";
                                amTermCnt++;
                            }
                            else if (landMinutes < pmLand)
                            {
                                if (howCalc == 1) trip.AmPm = "2";
                                pmTermCnt++;
                            }
                            else
                            {
                                if (howCalc == 1)
                                    trip.AmPm = "3";
                                nteTermCnt++;
                            }

                            int pushMinutes = dp.Flights[0].DepTime - dpCount * 1440;

                            if (pushMinutes > ntePush) ntePushCnt++;
                            else if (pushMinutes > pmPush) pmPushCnt++;
                            else amPushCnt++;

                            if (pushMinutes > amPush && landMinutes < amLand)
                            {
                                if (howCalc == 2) trip.AmPm = "1";
                                case2AmCnt++;
                            }
                            else if (pushMinutes > pmPush && landMinutes < pmLand)
                            {
                                if (howCalc == 2) trip.AmPm = "2";
                                case2PmCnt++;
                            }
                            else if (pushMinutes > ntePush && landMinutes < nteLand)
                            {
                                if (howCalc == 2)
                                    trip.AmPm = "3";
                                case2NteCnt++;
                            }
                            else
                            {
                                if (howCalc == 2)
                                    trip.AmPm = "3";
                                case2MixCnt++;
                            }

                            int centroid = (pushMinutes + landMinutes) / 2;

                            if (centroid < amCentroid)
                            {
                                if (howCalc == 3)
                                    trip.AmPm = "1";
                                amCentCnt++;
                            }
                            else if (centroid < pmCentroid)
                            {
                                if (centroid - amCentroid < pmCentroid - centroid)
                                {
                                    if (howCalc == 3)
                                        trip.AmPm = "1";
                                    amCentCnt++;
                                }
                                else
                                {
                                    if (howCalc == 3)
                                        trip.AmPm = "2";
                                    pmCentCnt++;
                                }
                            }
                            else if (centroid - pmCentroid < nteCentroid - centroid)
                            {
                                if (howCalc == 3)
                                    trip.AmPm = "2";
                                pmCentCnt++;
                            }
                            else
                            {
                                if (howCalc == 3)
                                    trip.AmPm = "3";
                                nteCentCnt++;
                            }

                            dpCount++;
                        }
                    }
                }

                int totalTerm = amTermCnt + pmTermCnt + nteTermCnt;
                int totalCentCnt = amCentCnt + pmCentCnt + nteCentCnt;
                int totalCase2Cnt = case2AmCnt + case2PmCnt + case2NteCnt + case2MixCnt;



                switch (howCalc)
                {
                    case 1:     // AM-Terminate/PM-Arrival
                        // AM terminates before amLand and pushes before pmPush
                        // PM terminates before pmLand and pushes before ntePush
                        // NTE terminates before nteLand and pushes after ntePush
                        // Mix is none of the above

                        switch (numOrPct)
                        {
                            case 1:         // number of differences
                                if (totalTerm - numDiff < amTermCnt) return "AM";
                                else if (totalTerm - numDiff < pmTermCnt) return " PM";
                                else if (totalTerm - numDiff < nteTermCnt) return "NTE";
                                else return "Mix";
                            case 2:         // percent of differences
                                if (totalTerm == 0) return "AM"; ;
                                if (1 - amTermCnt / (decimal)totalTerm < pctDiff) return "AM";
                                else if (1 - pmTermCnt / (decimal)totalTerm < pctDiff) return " PM";
                                else if (1 - nteTermCnt / (decimal)totalTerm < pctDiff) return "NTE";
                                else return "Mix";
                        }


                        break;
                    case 2:     // AM/PM Terminate/Push  -- handled in Case 1 -- good enough for now

                        switch (numOrPct)
                        {
                            case 1:         // number of differences
                                if (totalCase2Cnt - numDiff < case2AmCnt) return "AM";
                                else if (totalCase2Cnt - numDiff < case2PmCnt) return " PM";
                                else if (totalCase2Cnt - numDiff < case2NteCnt) return "NTE";
                                else return "Mix";
                            case 2:         // percent of differences
                                if (totalCase2Cnt == 0) return "AM"; ;
                                if (1 - case2AmCnt / (decimal)totalCase2Cnt < pctDiff) return "AM";
                                else if (1 - case2PmCnt / (decimal)totalCase2Cnt < pctDiff) return " PM";
                                else if (1 - case2NteCnt / (decimal)totalCase2Cnt < pctDiff) return "NTE";
                                else return "Mix";
                        }
                        break;

                    case 3:     // Banded Centroid
                        switch (numOrPct)
                        {
                            case 1:         // number of differences
                                if (totalCentCnt - numDiff < amCentCnt) return "AM";
                                else if (totalCentCnt - numDiff < pmCentCnt) return " PM";
                                else if (totalCentCnt - numDiff < nteCentCnt) return "NTE";
                                else return "Mix";
                            case 2:         // percent of differences
                                if (totalCentCnt == 0) return "AM"; ;
                                if (1 - amCentCnt / (decimal)totalCentCnt < pctDiff) return "AM";
                                else if (1 - pmCentCnt / (decimal)totalCentCnt < pctDiff) return " PM";
                                else if (1 - nteCentCnt / (decimal)totalCentCnt < pctDiff) return "NTE";
                                else return "Mix";
                        }
                        break;
                }

                return ampm;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private string CalcAmPmPropDrop(Line line)
        {
            if (line.BlankLine) return "---";

            // initialize
            string ampm = "AM";
            Trip trip = null;
            _amPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
            int howCalc = _amPmConfigure.HowCalcAmPm;
            int amPush = Convert.ToInt32(_amPmConfigure.AmPush.TotalMinutes);
            int amLand = Convert.ToInt32(_amPmConfigure.AmLand.TotalMinutes);
            int pmPush = Convert.ToInt32(_amPmConfigure.PmPush.TotalMinutes);
            int pmLand = Convert.ToInt32(_amPmConfigure.PmLand.TotalMinutes);
            pmLand = pmLand < amLand ? pmLand + 1440 : pmLand;
            int ntePush = Convert.ToInt32(_amPmConfigure.NitePush.TotalMinutes);
            ntePush = ntePush < pmPush ? ntePush + 1440 : ntePush;
            int nteLand = Convert.ToInt32(_amPmConfigure.NiteLand.TotalMinutes);
            nteLand = nteLand + 1440;
            int amCentroid = (amPush + amLand) / 2;
            int pmCentroid = (pmPush + pmLand) / 2;
            int nteCentroid = (ntePush + nteLand) / 2;
            int numOrPct = _amPmConfigure.NumberOrPercentageCalc;
            int numDiff = _amPmConfigure.NumOpposites;
            decimal pctDiff = _amPmConfigure.PctOpposities / 100m;
            int amTermCnt, amPushCnt, pmTermCnt, pmPushCnt, nteTermCnt, ntePushCnt, unknownTerm, unknownPush, amCentCnt, pmCentCnt, nteCentCnt, case2AmCnt, case2PmCnt, case2NteCnt, case2MixCnt;
            amTermCnt = amPushCnt = pmTermCnt = pmPushCnt = nteTermCnt = ntePushCnt = unknownTerm = unknownPush = amCentCnt = pmCentCnt = nteCentCnt = case2AmCnt = case2PmCnt = case2NteCnt = case2MixCnt = 0;


            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;

                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }

                int dpCount = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    if (line.ReserveLine == true)
                    {
                        int reservePush = dp.ReserveOut % 1440;
                        if (reservePush < GlobalSettings.ReserveAmPmClassification)
                        {
                            amTermCnt++;
                            case2AmCnt++;
                            amCentCnt++;
                            trip.AmPm = "1";
                        }
                        else
                        {
                            pmTermCnt++;
                            case2PmCnt++;
                            pmCentCnt++;
                            trip.AmPm = "2";
                        }
                    }
                    else
                    {
                        int landMinutes = dp.Flights[dp.Flights.Count - 1].ArrTime - dpCount * 1440;

                        if (landMinutes < amLand)
                        {
                            if (howCalc == 1) trip.AmPm = "1";
                            amTermCnt++;
                        }
                        else if (landMinutes < pmLand)
                        {
                            if (howCalc == 1) trip.AmPm = "2";
                            pmTermCnt++;
                        }
                        else
                        {
                            if (howCalc == 1) trip.AmPm = "2";
                            nteTermCnt++;
                        }

                        int pushMinutes = dp.Flights[0].DepTime - dpCount * 1440;

                        if (pushMinutes > ntePush) ntePushCnt++;
                        else if (pushMinutes > pmPush) pmPushCnt++;
                        else amPushCnt++;

                        if (pushMinutes > amPush && landMinutes < amLand)
                        {
                            if (howCalc == 2) trip.AmPm = "1";
                            case2AmCnt++;
                        }
                        else if (pushMinutes > pmPush && landMinutes < pmLand)
                        {
                            if (howCalc == 2)
                                trip.AmPm = "2";
                            case2PmCnt++;
                        }
                        else if (pushMinutes > ntePush && landMinutes < nteLand)
                        {
                            if (howCalc == 2)
                                trip.AmPm = "3";
                            case2NteCnt++;
                        }
                        else
                        {
                            if (howCalc == 2)
                                trip.AmPm = "3";
                            case2MixCnt++;
                        }

                        int centroid = (pushMinutes + landMinutes) / 2;

                        if (centroid < amCentroid)
                        {
                            if (howCalc == 3)
                                trip.AmPm = "1";
                            amCentCnt++;
                        }
                        else if (centroid < pmCentroid)
                        {
                            if (centroid - amCentroid < pmCentroid - centroid)
                            {
                                if (howCalc == 3)
                                    trip.AmPm = "1";
                                amCentCnt++;
                            }
                            else
                            {
                                if (howCalc == 3)
                                    trip.AmPm = "2";
                                pmCentCnt++;
                            }
                        }
                        else if (centroid - pmCentroid < nteCentroid - centroid)
                        {
                            if (howCalc == 3)
                                trip.AmPm = "2";
                            pmCentCnt++;
                        }
                        else
                        {
                            if (howCalc == 3)
                                trip.AmPm = "3";
                            nteCentCnt++;
                        }

                        dpCount++;
                    }
                }
            }

            int totalTerm = amTermCnt + pmTermCnt + nteTermCnt;
            int totalCentCnt = amCentCnt + pmCentCnt + nteCentCnt;
            int totalCase2Cnt = case2AmCnt + case2PmCnt + case2NteCnt + case2MixCnt;



            switch (howCalc)
            {
                case 1:     // AM-Terminate/PM-Arrival
                    // AM terminates before amLand and pushes before pmPush
                    // PM terminates before pmLand and pushes before ntePush
                    // NTE terminates before nteLand and pushes after ntePush
                    // Mix is none of the above

                    switch (numOrPct)
                    {
                        case 1:         // number of differences

                            if (totalTerm - numDiff < amTermCnt) return "AM";
                            else if (totalTerm - numDiff < pmTermCnt) return " PM";
                            else if (totalTerm - numDiff < nteTermCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (totalTerm == 0) return "AM"; ;
                            if (1 - amTermCnt / (decimal)totalTerm < pctDiff) return "AM";
                            else if (1 - pmTermCnt / (decimal)totalTerm < pctDiff) return " PM";
                            else if (1 - nteTermCnt / (decimal)totalTerm < pctDiff) return "NTE";
                            else return "Mix";
                    }


                    break;
                case 2:     // AM/PM Terminate/Push  -- handled in Case 1 -- good enough for now

                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCase2Cnt - numDiff < case2AmCnt) return "AM";
                            else if (totalCase2Cnt - numDiff < case2PmCnt) return " PM";
                            else if (totalCase2Cnt - numDiff < case2NteCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (totalCase2Cnt == 0) return "AM"; ;
                            if (1 - case2AmCnt / (decimal)totalCase2Cnt < pctDiff) return "AM";
                            else if (1 - case2PmCnt / (decimal)totalCase2Cnt < pctDiff) return " PM";
                            else if (1 - case2NteCnt / (decimal)totalCase2Cnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;

                case 3:     // Banded Centroid
                    switch (numOrPct)
                    {
                        case 1:         // number of differences
                            if (totalCentCnt - numDiff < amCentCnt) return "AM";
                            else if (totalCentCnt - numDiff < pmCentCnt) return " PM";
                            else if (totalCentCnt - numDiff < nteCentCnt) return "NTE";
                            else return "Mix";
                        case 2:         // percent of differences
                            if (totalCentCnt == 0) return "AM"; ;
                            if (1 - amCentCnt / (decimal)totalCentCnt < pctDiff) return "AM";
                            else if (1 - pmCentCnt / (decimal)totalCentCnt < pctDiff) return " PM";
                            else if (1 - nteCentCnt / (decimal)totalCentCnt < pctDiff) return "NTE";
                            else return "Mix";
                    }
                    break;
            }

            return ampm;
        }
        #endregion

        private string CalcTafb(Line line, bool inLine)
        {
            Trip trip = null;
            int tafb = 0;

            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                //trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                DateTime tripstartdate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                if (!inLine)        // calculates Tafb in bid period
                {

                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (tripstartdate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate) // overlaps into next month
                        {
                            tafb += CalculateTafbInBpOfDutyPeriod(dp, trip.PairLength);
                        }
                        tripstartdate = tripstartdate.AddDays(1);

                    }
                }
                else
                {
                    tafb += trip.Tafb;
                }
            }
            return (tafb / 60).ToString() + ":" + (tafb % 60).ToString("d2");
        }

        #region FltHr
        private decimal CalcTfpPerFltHr(Line line)
        {
            if (line.ReserveLine || line.BlankLine) return 0.00m;

            decimal blkHours = 0.0m;
            decimal tfp;
            // line.block is an int stored as hhmm, except FA could be hhhmm
            if (line.Block.ToString().Length == 5)
                blkHours = (Convert.ToDecimal(line.Block.ToString().Substring(0, 3)) +
                  Convert.ToDecimal(line.Block.ToString().Substring(3, 2)) / 60m);
            else
                blkHours = (Convert.ToDecimal(line.Block.ToString().Substring(0, 2)) +
                    Convert.ToDecimal(line.Block.ToString().PadLeft(4, '0').Substring(2, 2)) / 60m);
            //Convert.ToDecimal(line.Block.ToString().Substring(2, 2)) / 60m);

            tfp = (blkHours == 0) ? 0.00m : line.Tfp / blkHours;

            return Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", tfp)), 2);
            // return Math.Round(1200m / 100, 2);


        }

        private decimal CalcTfpPerFltHrVacation(Line line)
        {
            if (line.ReserveLine || line.BlankLine) return 0.00m;


            string blockHoursInBp = line.BlkHrsInBp.ToString().Replace(":", "");
            decimal blkHours = 0.0m;
            decimal tfp;
            // line.block is an int stored as hhmm, except FA could be hhhmm
            try
            {
                if (blockHoursInBp.Length == 5)

                    blkHours = (Convert.ToDecimal(blockHoursInBp.Substring(0, 3)) + Convert.ToDecimal(blockHoursInBp.Substring(3, 2)) / 60m);
                else
                    blkHours = (Convert.ToDecimal(blockHoursInBp.Substring(0, 2)) + Convert.ToDecimal(blockHoursInBp.PadLeft(4, '0').Substring(2, 2)) / 60m);

                tfp = (blkHours == 0) ? 0.00m : line.Tfp / blkHours;

                return Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", tfp)), 2);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            // return Math.Round(1200m / 100, 2);


        }
        #endregion

        #region TfpPerDay
        private decimal CalculateTfpPerDay(Line line)
        {

            decimal tfpPerDay = 0.00m;

            if (line.ReserveLine)
            {

                tfpPerDay = (GlobalSettings.CurrentBidDetails.Postion == "FA") ? GlobalSettings.FAReserveDayPay : GlobalSettings.ReserveDailyGuarantee;
            }
            else if (line.BlankLine)
            {
                tfpPerDay = 0.00m;
            }
            else
            {
                tfpPerDay = Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", line.DaysWork == 0 ? 0.00m : (line.Tfp / line.DaysWork))), 2);
            }

            return tfpPerDay;
            //<0?0:tfpPerDay;
        }
        #endregion

        #region DutPdsCount
        private int CalcTotDutPds(Line line, bool inLine)
        {
            Trip trip = null;
            int totDutPds = 0;
            //int daysInOverlap = 0;

            DateTime tripDate = DateTime.MinValue;

            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                if (inLine)
                {
                    totDutPds += trip.DutyPeriods.Count();
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (tripDate <= _bpEndDate)
                        {
                            totDutPds += 1;

                        }
                        tripDate = tripDate.AddDays(1); ;

                    }
                }
            }



            return totDutPds;
        }

        private int CalcTotDutPdsVacation(Line line, bool inLine)
        {
            Trip trip = null;
            int totDutPds = 0;
            //int daysInOverlap = 0;

            DateTime tripDate = DateTime.MinValue;
            int dutyperiodCount = 0;

            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                    tripDate = vacTrip.TripVacationStartDate;
                    dutyperiodCount = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                    if (inLine)
                    {
                        totDutPds += dutyperiodCount;
                    }
                    else
                    {
                        if (tripDate.AddDays(dutyperiodCount - 1) > _bpEndDate)
                        {
                            totDutPds += _bpEndDate.Subtract(tripDate).Days + 1;

                        }
                        else
                        {
                            totDutPds += dutyperiodCount;
                        }

                    }
                }

                else
                {
                    if (inLine)
                    {
                        totDutPds += trip.DutyPeriods.Count();
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                        if (tripDate.AddDays(trip.PairLength - 1) > _bpEndDate)
                        {
                            totDutPds += _bpEndDate.Subtract(tripDate).Days + 1;
                        }
                        else
                        {
                            totDutPds += trip.DutyPeriods.Count();
                        }

                    }
                }
            }



            return totDutPds;
        }

        private int CalcTotDutPdsDrop(Line line, bool inLine)
        {
            Trip trip = null;
            int totDutPds = 0;
            //int daysInOverlap = 0;

            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;

            foreach (string pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;

                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    if (inLine)
                    {
                        totDutPds += trip.DutyPeriods.Count();
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        if (tripDate.AddDays(trip.PairLength - 1) > _bpEndDate)
                        {
                            totDutPds += _bpEndDate.Subtract(tripDate).Days + 1;

                        }
                        else
                        {
                            totDutPds += trip.DutyPeriods.Count();
                        }

                    }
                }
            }

            return totDutPds;
        }
        #endregion

        #region TafbTime
        private decimal CalcTafbTime(Line line)
        {

            decimal tfpPerTafb = 0.0m;
            //TfpPerTafb
            if (!line.ReserveLine && !line.BlankLine)
            {
                string[] tafbTime = line.TafbInBp.Split(':');
                decimal tafbInMin = Convert.ToDecimal(tafbTime[0]) * 60 + Convert.ToDecimal(tafbTime[1]);
                tfpPerTafb = line.ReserveLine || line.BlankLine ? 0m : (tafbInMin == 0) ? 0 : Math.Round(line.Tfp / (tafbInMin / 60m), 2);
            }

            return tfpPerTafb;

        }
        #endregion

        #region  TFP in Bp
        /// <summary>
        /// PURPOSE : Calculate Overnight Cities
        /// </summary>
        /// <param name="line"></param>
        private decimal CalcTfpInBP(Line line)
        {
            // frank add
            decimal tfp = line.Tfp;

            foreach (var s in line.LineSips)
            {
                if (s.Dropped)
                {
                    tfp -= s.SipTfp;
                }
            }
            return Math.Round(decimal.Parse(String.Format("{0:0.00}", tfp)), 2); ;
        }
        #endregion

        #region DutyHrs
        private string CalcDutyHrs(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                foreach (var dp in trip.DutyPeriods)
                {
                    if (inLine)
                    {
                        dutyHrs += dp.DutyTime;

                    }
                    else if (tripDate <= _bpEndDate)
                    {
                        dutyHrs += dp.DutyTime;

                    }
                    tripDate = tripDate.AddDays(1); ;

                }
            }

            return (dutyHrs / 60).ToString() + ":" + (dutyHrs % 60).ToString().PadLeft(2, '0');

        }

        private string CalcDutyHrsVacation(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            DateTime tripDate = DateTime.MinValue;
            int dpIndex = 0;
            int flightIndex = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);


                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    dpIndex = 0;
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (tripDate <= _bpEndDate)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                dutyHrs += dp.DutyTime;
                            }
                            else if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                flightIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        dutyHrs += flt.Block;

                                    }

                                    flightIndex++;
                                }

                            }

                        }
                        dpIndex++;
                        tripDate = tripDate.AddDays(1);
                    }
                }

                else
                {
                    if (inLine)
                    {
                        dutyHrs += trip.DutyPeriods.Sum(x => x.DutyTime);
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        dpIndex = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (tripDate <= _bpEndDate)
                            {
                                dutyHrs += dp.DutyTime;
                            }

                            dpIndex++;
                            tripDate = tripDate.AddDays(1);
                        }


                    }
                }
            }

            return (dutyHrs / 60).ToString() + ":" + (dutyHrs % 60).ToString().PadLeft(2, '0');
        }

        private string CalcDutyHrsDrop(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            DateTime tripDate = DateTime.MinValue;
            int dpIndex = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    dpIndex = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (inLine)
                        {
                            dutyHrs += dp.DutyTime;

                        }
                        else if (tripDate <= _bpEndDate)
                        {
                            dutyHrs += dp.DutyTime;

                        }
                        tripDate = tripDate.AddDays(1); ;

                    }
                    dpIndex++;


                }
            }


            return (dutyHrs / 60).ToString() + ":" + (dutyHrs % 60).ToString().PadLeft(2, '0');
        }

        #endregion

        #region TfpPerDhr
        private decimal CalcTfpPerDhr(Line line)
        {

            decimal tfpPerDhr = 0.0m;
            //TfpPerDhr
            string[] dhrTime = line.DutyHrsInBp.Split(':');
            decimal dhrInMin = Convert.ToDecimal(dhrTime[0]) * 60 + Convert.ToDecimal(dhrTime[1]);
            tfpPerDhr = line.ReserveLine || line.BlankLine ? 0m : dhrInMin == 0 ? 0 : Math.Round(line.Tfp / (dhrInMin / 60m), 2);
            return tfpPerDhr;
        }

        #endregion

        #region LegsPerDay
        private decimal CalcLegsPerDay(Line line)
        {
            decimal legsPerDay = 0.0m;
            // line.LegsPerDay
            //legsPerDay = line.ReserveLine || line.BlankLine ? 0.00m : line.DaysWork == 0 ? 0.00m : Math.Round(decimal.Parse(String.Format("{0:0.00}", Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.DaysWork))), 2, MidpointRounding.AwayFromZero); ;
            legsPerDay = line.ReserveLine || line.BlankLine ? 0.00m : line.DaysWork == 0 ? 0.00m : Math.Round(decimal.Parse(String.Format("{0:0.00}", Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.DaysWork))), 2); ;

            return legsPerDay;
        }

        #endregion

        #region LargestBlkDaysOff
        private int CalcLargestBlkDaysOff(Line line)
        {
            int largestDaysOff = 0;
            int tripOff = 0;


            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            Trip trip = null;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                if (tripDate > oldTripdate)
                {
                    tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                    if (tripOff > largestDaysOff)
                    {
                        largestDaysOff = tripOff;

                    }

                }

                oldTripdate = tripDate.AddDays(trip.PairLength - 1);

            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.Subtract(oldTripdate).Days < 0) ? 0 : _bpEndDate.Subtract(oldTripdate).Days;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }
            return largestDaysOff;
        }

        private int CalcLargestBlkDaysOffVacation(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;

            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    if (vacTrip.TripType == "VOB")
                    {
                        tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
                    }
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);
                }

                else
                {
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }
                    }

                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);
                }

            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }

            return largestDaysOff;
        }

        private int CalcLargestBlkDaysOffDrop(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;
            DateTime tripDate = DateTime.MinValue;
            DateTime oldTripdate = _bpStartDate.AddDays(-1);

            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }
                    }
                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);
                }
            }

            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }
            }

            return largestDaysOff;


        }


        private int CalcLargestBlkDaysOffEOMDrop(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;
            DateTime tripDate = DateTime.MinValue;
            DateTime oldTripdate = _bpStartDate.AddDays(-1);

            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                if (tripEndDate >= _eomDate)
                {
                    VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                    DateTime date = tripStartDate;

                    if (vacationTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }

                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }
                    }
                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);
                }
            }

            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }
            }

            return largestDaysOff;


        }


        private int CalcLargestBlkDaysOffVacationEOM(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;

            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            VacationTrip vacationTrip = null;
            foreach (var pairing in line.Pairings)
            {
                vacationTrip = null;
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    if (vacTrip.TripType == "VOB")
                    {
                        tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
                    }


                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                }

                else
                {
                    //Checking the trip is EOM
                    if (tripDate.AddDays(trip.PairLength - 1) >= _eomDate)
                    {

                        if (GlobalSettings.VacationData.Keys.Contains(pairing))
                        {
                            vacationTrip = GlobalSettings.VacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
                            if (vacationTrip != null)
                            {
                                tripDate = _bpEndDate.AddDays(1);
                            }
                        }
                    }



                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);

                }


            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }

            return largestDaysOff;
        }


        //private int CalcLargestBlkDaysOffVacationEOM(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0;
        //    int tripOff = 0;

        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    DateTime tripDate = DateTime.MinValue;
        //    bool isLastTrip = false; int paringCount = 0;
        //    VacationTrip vacationTrip = null;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        vacationTrip = null;
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


        //        VacationStateTrip vacTrip = null;
        //        if (line.VacationStateLine.VacationTrips != null)
        //        {
        //            vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
        //        }
        //        // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


        //        if (vacTrip != null)
        //        {
        //            if (vacTrip.TripVacationStartDate == DateTime.MinValue)
        //            {
        //                continue;
        //            }
        //            if (vacTrip.TripType == "VOB")
        //            {
        //                tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
        //            }


        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }


        //            //oldTripdate = tripDate.AddDays(trip.PairLength - 1);
        //            oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

        //        }

        //        else
        //        {
        //            //Checking the trip is EOM
        //            if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //            {

        //                if (_vacationData.Keys.Contains(pairing))
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                    if (vacationTrip != null)
        //                    {
        //                        tripDate = tripDate;
        //                        //_bpEndDate.AddDays(1);
        //                    }
        //                }
        //            }



        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(trip.PairLength - 1);

        //        }


        //    }
        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;

        //        }

        //    }

        //    return largestDaysOff;
        //}

        private int CalcLargestBlkDaysOffVacationEOMDrop(Line line)
        {
            Trip trip = null;
            int largestDaysOff = 0;
            int tripOff = 0;

            DateTime oldTripdate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            VacationTrip vacationTrip = null;
            foreach (var pairing in line.Pairings)
            {
                vacationTrip = null;
                //Get trip
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                }

                else
                {
                    //Checking the trip is EOM
                    if (tripDate.AddDays(trip.PairLength - 1) >= _eomDate)
                    {

                        if (GlobalSettings.VacationData.Keys.Contains(pairing))
                        {
                            vacationTrip = GlobalSettings.VacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
                            if (vacationTrip != null)
                            {
                                tripDate = _bpEndDate.AddDays(1);
                            }
                        }
                    }



                    if (tripDate > oldTripdate)
                    {
                        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
                        if (tripOff > largestDaysOff)
                        {
                            largestDaysOff = tripOff;

                        }

                    }

                    oldTripdate = tripDate.AddDays(trip.PairLength - 1);

                }


            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }

            return largestDaysOff;
        }

        private int CalcLargestBlkDaysOffEOM(Line line)
        {

            int largestDaysOff = 0;
            int tripOff = 0;


            DateTime oldTripdate = _bpStartDate.AddDays(-1);

            Trip trip = null;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);

                if (tripEndDate >= _eomDate)
                {
                    VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                    DateTime date = tripStartDate;
                    if (vacationTrip != null)
                    {
                        continue;
                    }
                }




                if (tripStartDate > oldTripdate)
                {
                    tripOff = (tripStartDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripStartDate.Subtract(oldTripdate).Days - 1;
                    if (tripOff > largestDaysOff)
                    {
                        largestDaysOff = tripOff;

                    }

                }

                oldTripdate = tripStartDate.AddDays(trip.PairLength - 1);

            }
            if (oldTripdate < _bpEndDate)
            {
                tripOff = (_bpEndDate.Subtract(oldTripdate).Days < 0) ? 0 : _bpEndDate.Subtract(oldTripdate).Days;
                if (tripOff > largestDaysOff)
                {
                    largestDaysOff = tripOff;

                }

            }
            return largestDaysOff;









        }


        #endregion
        #region International constraints
        /// <summary>
        /// If any line contains a trip that has a true International property, then that line will have it’s International property set to true
        /// Similarly for the NonConus line
        /// </summary>
        /// <param name="line"></param>
        private void SetInternationAndNoConusLine(Line line)
        {
            line.International = false;
            line.NonConus = false;
            Trip trip = null;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                if (trip.International)
                    line.International = true;
                if (trip.NonConus)
                    line.NonConus = true;
            }
        }
        private void SetInternationAndNoConusLineVacation(Line line)
        {
            Trip trip = null;
            line.International = false;
            line.NonConus = false;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }

                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        int dpIndex = 0;
                        int flightIndex = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            foreach (var flt in dp.Flights)
                            {
                                flightIndex = 0;
                                if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VD")
                                {
                                    if (GlobalSettings.WBidINIContent.Cities.Where(z => z.International).Any(x => x.Name == flt.ArrSta) || GlobalSettings.WBidINIContent.Cities.Where(z => z.International).Any(x => x.Name == flt.DepSta))
                                    {
                                        line.International = true;
                                    }
                                    if (GlobalSettings.WBidINIContent.Cities.Where(z => z.NonConus).Any(x => x.Name == flt.ArrSta) || GlobalSettings.WBidINIContent.Cities.Where(z => z.NonConus).Any(x => x.Name == flt.DepSta))
                                    {
                                        line.NonConus = true;
                                    }
                                    flightIndex++;
                                }

                            }

                            dpIndex++;
                        }

                    }
                }
                else
                {
                    if (trip.International)
                        line.International = true;
                    if (trip.NonConus)
                        line.NonConus = true;
                }
            }
        }
        private void SetInternationAndNoConusLineVacationDrop(Line line)
        {
            line.International = false;
            line.NonConus = false;
            Trip trip = null;
            foreach (var pairing in line.Pairings)
            {

                //Get trip
                trip = GetTrip(pairing);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }

                if (vacTrip != null)
                {

                }
                else
                {
                    if (trip.International)
                        line.International = true;
                    if (trip.NonConus)
                        line.NonConus = true;
                }
            }
        }

        #endregion

        #region LongGrndTime
        private TimeSpan CalcLongGrndTime(Line line)
        {
            Trip trip = null;
            int maxGrndTime = 0;
            int turnTime = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                // string tripName = pairing.Substring(0, 4);
                foreach (var dp in trip.DutyPeriods)
                {
                    int lastLandTime = 0;
                    foreach (var flt in dp.Flights)
                    {
                        if (lastLandTime != 0)
                        {
                            turnTime = flt.DepTime - lastLandTime;
                            maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                        }
                        lastLandTime = flt.ArrTime;
                    }
                }
            }
            return new TimeSpan(maxGrndTime / 60, maxGrndTime % 60, 0);
        }

        private TimeSpan CalcLongGrndTimeVacation(Line line)
        {
            Trip trip = null;
            int maxGrndTime = 0;
            int turnTime = 0;
            int dpId = 0;
            int flightId = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);




                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var dp in trip.DutyPeriods)
                        {
                            int lastLandTime = 0;
                            dpId = 0;
                            if (vacTrip.VacationDutyPeriods[dpId].DutyPeriodType == "VO" || vacTrip.VacationDutyPeriods[dpId].DutyPeriodType == "VA")
                            {
                                dpId++;
                                continue;
                            }
                            flightId = 0;
                            foreach (var flt in dp.Flights)
                            {
                                if (vacTrip.VacationDutyPeriods[dpId].VacationFlights[flightId].FlightType == "VO" || vacTrip.VacationDutyPeriods[dpId].VacationFlights[flightId].FlightType == "VA")
                                {
                                    flightId++;
                                    continue;
                                }

                                if (lastLandTime != 0)
                                {
                                    turnTime = flt.DepTime - lastLandTime;
                                    maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                                }
                                lastLandTime = flt.ArrTime;
                            }
                            dpId++;
                        }

                    }

                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastLandTime = 0;
                        foreach (var flt in dp.Flights)
                        {
                            if (lastLandTime != 0)
                            {
                                turnTime = flt.DepTime - lastLandTime;
                                maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                            }
                            lastLandTime = flt.ArrTime;
                        }
                    }
                }
            }
            return new TimeSpan(maxGrndTime / 60, maxGrndTime % 60, 0);
        }

        private TimeSpan CalcLongGrndTimeDrop(Line line)
        {
            Trip trip = null;
            int maxGrndTime = 0;
            int turnTime = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastLandTime = 0;
                        foreach (var flt in dp.Flights)
                        {
                            if (lastLandTime != 0)
                            {
                                turnTime = flt.DepTime - lastLandTime;
                                maxGrndTime = turnTime > maxGrndTime ? turnTime : maxGrndTime;
                            }
                            lastLandTime = flt.ArrTime;
                        }
                    }
                }
            }
            return new TimeSpan(maxGrndTime / 60, maxGrndTime % 60, 0);
        }
        #endregion

        #region Mostlegs
        private int CalcMostlegs(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;


            Trip trip = null;
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                int legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                if (mostlegs < legsintrip)
                {
                    mostlegs = legsintrip;
                }
            }
            return mostlegs;
        }

        private int CalcMostlegsVacation(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;
            int legsintrip = 0;

            Trip trip = null;
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        legsintrip = vacTrip.VacationDutyPeriods.SelectMany(x => x.VacationFlights.Where(y => y.FlightType == "VD")).Count();
                        if (mostlegs < legsintrip)
                        {
                            mostlegs = legsintrip;
                        }
                    }
                }
                else
                {


                    legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                    if (mostlegs < legsintrip)
                    {
                        mostlegs = legsintrip;
                    }
                }
            }
            return mostlegs;
        }

        private int CalcMostlegsDrop(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;


            Trip trip = null;
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);



                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {

                    int legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                    if (mostlegs < legsintrip)
                    {
                        mostlegs = legsintrip;
                    }
                }
            }
            return mostlegs;
        }
        #endregion

        #region TripLength
        private void CalcTripLength(Line line)
        {
            Trip trip = null;
            int tripLength = 0;

            line.Trips1Day = 0;
            line.Trips2Day = 0;
            line.Trips3Day = 0;
            line.Trips4Day = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                tripLength = trip.PairLength;

                switch (tripLength)
                {
                    case 1:
                        line.Trips1Day++;
                        break;
                    case 2:
                        line.Trips2Day++;
                        break;
                    case 3:
                        line.Trips3Day++;
                        break;
                    case 4:
                        line.Trips4Day++;
                        break;



                }
            }


        }

        private void CalcTripLengthVacation(Line line)
        {
            Trip trip = null;
            int tripLength = 0;

            line.Trips1Day = 0;
            line.Trips2Day = 0;
            line.Trips3Day = 0;
            line.Trips4Day = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);



                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }

                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        tripLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();

                    }
                }
                else
                {
                    tripLength = trip.PairLength;
                }

                tripLength = trip.PairLength;
                switch (tripLength)
                {
                    case 1:
                        line.Trips1Day++;
                        break;
                    case 2:
                        line.Trips2Day++;
                        break;
                    case 3:
                        line.Trips3Day++;
                        break;
                    case 4:
                        line.Trips4Day++;
                        break;

                }
            }


        }


        private void CalcTripLengthDrop(Line line)
        {
            Trip trip = null;
            int tripLength = 0;

            line.Trips1Day = 0;
            line.Trips2Day = 0;
            line.Trips3Day = 0;
            line.Trips4Day = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripLength = trip.PairLength;
                }

                switch (tripLength)
                {
                    case 1:
                        line.Trips1Day++;
                        break;
                    case 2:
                        line.Trips2Day++;
                        break;
                    case 3:
                        line.Trips3Day++;
                        break;
                    case 4:
                        line.Trips4Day++;
                        break;



                }
            }


        }
        #endregion

        #region NumLegs
        private void CalcNumLegsOfEachType(Line line)
        {
            Trip trip = null;

            line.LegsIn800 = 0;
            line.LegsIn700 = 0;
            //line.LegsIn500 = 0;
            //line.LegsIn300 = 0;
            line.LegsIn600 = 0;
            line.LegsIn200 = 0;
            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                string tripName = pairing.Substring(0, 4);
                // if (tripName.Substring(1, 1) != "W" && tripName.Substring(1, 1) != "Y")     // does not look inside reserver trip
                if (!trip.ReserveTrip)
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            if (flt.Equip != null)
                            {
                                if (flt.Equip.Length > 0)
                                {

                                    switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                    {
                                        case "8":
                                            line.LegsIn800++;
                                            break;
                                        case "7":
                                            line.LegsIn700++;
                                            break;
                                        //case "5":
                                        //  line.LegsIn500++;
                                        //  break;
                                        //case "3":
                                        //  line.LegsIn300++;
                                        //  break;
                                        case "6":
                                            line.LegsIn600++;
                                            break;
                                        case "2":
                                            line.LegsIn200++;
                                            break;
                                    }
                                }

                            }
                        }

                    }
                }
            }


        }

        private void CalcNumLegsOfEachTypeforVacation(Line line)
        {
            Trip trip = null;

            line.LegsIn800 = 0;
            line.LegsIn700 = 0;
            //line.LegsIn500 = 0;
            //line.LegsIn300 = 0;
            line.LegsIn600 = 0;
            line.LegsIn200 = 0;
            int dpIndex = 0;
            int flightIndex = 0;


            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                string tripName = pairing.Substring(0, 4);
                if (!trip.ReserveTrip)
                // does not look inside reserver trip
                {

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }

                    //Vacation trip
                    if (vacTrip != null)
                    {
                        //vacation trip with out VD
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            dpIndex = 0;
                            foreach (var dp in trip.DutyPeriods)
                            {
                                if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VO")
                                {
                                    dpIndex++;
                                    continue;
                                }

                                flightIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VO")
                                    {
                                        flightIndex++;
                                        continue;
                                    }

                                    if (flt.Equip != null)
                                    {
                                        if (flt.Equip.Length > 0)
                                        {
                                            switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                            {
                                                case "8":
                                                    line.LegsIn800++;
                                                    break;
                                                case "7":
                                                    line.LegsIn700++;
                                                    break;
                                                //case "5":
                                                //  line.LegsIn500++;
                                                //  break;
                                                //case "3":
                                                //  line.LegsIn300++;
                                                //  break;
                                                case "6":
                                                    line.LegsIn600++;
                                                    break;
                                                case "2":
                                                    line.LegsIn200++;
                                                    break;
                                            }
                                        }

                                    }
                                    flightIndex++;
                                }
                                dpIndex++;
                            }


                        }
                    }
                    else
                    {  //Normal trip
                        foreach (var dp in trip.DutyPeriods)
                        {
                            foreach (var flt in dp.Flights)
                            {
                                if (flt.Equip != null)
                                {
                                    if (flt.Equip.Length > 0)
                                    {
                                        switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                        {
                                            case "8":
                                                line.LegsIn800++;
                                                break;
                                            case "7":
                                                line.LegsIn700++;
                                                break;
                                            //case "5":
                                            //  line.LegsIn500++;
                                            //  break;
                                            //case "3":
                                            //  line.LegsIn300++;
                                            //  break;
                                            case "6":
                                                line.LegsIn600++;
                                                break;
                                            case "2":
                                                line.LegsIn200++;
                                                break;
                                        }
                                    }

                                }
                            }

                        }

                    }

                }
            }


        }

        private void CalcNumLegsOfEachTypeforDrop(Line line)
        {
            Trip trip = null;

            line.LegsIn800 = 0;
            line.LegsIn700 = 0;
            line.LegsIn500 = 0;
            line.LegsIn300 = 0;
            line.LegsIn600 = 0;
            line.LegsIn200 = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);

                string tripName = pairing.Substring(0, 4);
                if (!trip.ReserveTrip)
                // if (tripName.Substring(1, 1) != "W" && tripName.Substring(1, 1) != "Y")     // does not look inside reserver trip
                {

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {


                        foreach (var dp in trip.DutyPeriods)
                        {
                            foreach (var flt in dp.Flights)
                            {
                                if (flt.Equip != null && flt.Equip.Length>0)
                                {

                                    switch (GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1))
                                    {
                                        case "8":
                                            line.LegsIn800++;
                                            break;
                                        case "7":
                                            line.LegsIn700++;
                                            break;
                                        //case "5":
                                        //  line.LegsIn500++;
                                        //  break;
                                        //case "3":
                                        //  line.LegsIn300++;
                                        //  break;
                                        case "6":
                                            line.LegsIn600++;
                                            break;
                                        case "2":
                                            line.LegsIn200++;
                                            break;

                                    }

                                }
                            }

                        }
                    }
                }
            }


        }


        #endregion

        #region AcftChanges
        private int CalcNumAcftChanges(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                foreach (var dp in trip.DutyPeriods)
                {
                    foreach (var flt in dp.Flights)
                    {
                        if (flt.AcftChange == true)
                            numAcftChanges++;
                    }
                }
            }

            return numAcftChanges;
        }

        private int CalcNumAcftChangesVacation(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;
            int dpIndex = 0;
            int flightIndex = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                //Get trip
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {

                        dpIndex = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VO")
                            {
                                dpIndex++;
                                continue;
                            }

                            flightIndex = 0;
                            foreach (var flt in dp.Flights)
                            {
                                if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VA" || vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VO")
                                {
                                    flightIndex++;
                                    continue;
                                }
                                if (flt.AcftChange)
                                    numAcftChanges++;
                                flightIndex++;
                            }
                            dpIndex++;
                        }

                    }
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            if (flt.AcftChange == true)
                                numAcftChanges++;
                        }
                    }
                }
            }

            return numAcftChanges;
        }

        private int CalcNumAcftChangesDrop(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;

            foreach (var pairing in line.Pairings)
            {
                ///Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        foreach (var flt in dp.Flights)
                        {
                            if (flt.AcftChange == true)
                                numAcftChanges++;
                        }
                    }
                }
            }

            return numAcftChanges;
        }


        #endregion

        #region DaysWorkInLine
        private int CalcDaysWorkInLine(Line line)
        {
            Trip trip = null;
            int daysWorkInLine = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);
                daysWorkInLine += trip.PairLength;
            }

            return daysWorkInLine;
        }

        private int CalcDaysWorkInLineVacation(Line line)
        {

            Trip trip = null;
            int daysWorkInLine = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    daysWorkInLine += vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();

                }
                else
                {
                    daysWorkInLine += trip.PairLength;
                }




            }

            return daysWorkInLine;
        }

        private int CalcDaysWorkInLineDrop(Line line)
        {
            Trip trip = null;
            int daysWorkInLine = 0;

            foreach (var pairing in line.Pairings)
            {
                //Get trip
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    daysWorkInLine += trip.PairLength;
                }
            }

            return daysWorkInLine;
        }

        #endregion

        #region LastArrTime
        private TimeSpan CalcLastArrTime(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                dutPd = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    int lastFlt = dp.Flights.Count - 1;
                    arrTime = (dp.Flights[lastFlt].ArrTime - 1440 * dutPd > arrTime) ? dp.Flights[lastFlt].ArrTime - dutPd * 1440 : arrTime;
                    dutPd++;
                }
            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);
        }

        private TimeSpan CalcLastArrTimeVacation(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            int fltIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);



                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        dutPd = 0;
                        int tempArrTime = 0;
                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dutPd].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dutPd].DutyPeriodType == "Split")
                            {
                                fltIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dutPd].DutyPeriodType == "VD")
                                    {
                                        tempArrTime = dp.Flights[fltIndex].ArrTime - 1440 * dutPd;
                                    }
                                    fltIndex++;
                                }
                                arrTime = (tempArrTime > arrTime) ? tempArrTime : arrTime;

                            }

                            dutPd++;
                        }
                    }

                }
                else
                {
                    dutPd = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastFlt = dp.Flights.Count - 1;
                        arrTime = (dp.Flights[lastFlt].ArrTime - 1440 * dutPd > arrTime) ? dp.Flights[lastFlt].ArrTime - dutPd * 1440 : arrTime;
                        dutPd++;
                    }
                }




            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);
        }


        private TimeSpan CalcLastArrTimeDrop(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    dutPd = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        int lastFlt = dp.Flights.Count - 1;
                        arrTime = (dp.Flights[lastFlt].ArrTime - 1440 * dutPd > arrTime) ? dp.Flights[lastFlt].ArrTime - dutPd * 1440 : arrTime;
                        dutPd++;
                    }
                }
            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);
        }
        #endregion

        #region LastDomArrTime
        private TimeSpan CalcLastDomArrTime(Line line)
        {
            Trip trip = null;

            int lastDomArr = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                int lastDp = trip.DutyPeriods.Count - 1;
                int lastFlt = trip.DutyPeriods[lastDp].Flights.Count - 1;
                lastDomArr = (trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 > lastDomArr) ?
                                trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 : lastDomArr;
            }

            int hours = lastDomArr / 60;
            int minutes = lastDomArr % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);
        }

        private TimeSpan CalcLastDomArrTimeVacation(Line line)
        {
            Trip trip = null;

            int lastDomArr = 0;
            int tempDomArr = 0;
            int dpIndex = 0;
            int flightIndex = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {

                        dpIndex = 0;

                        foreach (var dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                flightIndex = 0;
                                foreach (var flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        tempDomArr = trip.DutyPeriods[dpIndex].Flights[flightIndex].ArrTime - dpIndex * 1440;
                                    }
                                    flightIndex++;
                                }
                            }


                            dpIndex++;
                        }

                        lastDomArr = (tempDomArr > lastDomArr) ? tempDomArr : lastDomArr;


                    }
                }
                else
                {


                    int lastDp = trip.DutyPeriods.Count - 1;
                    int lastFlt = trip.DutyPeriods[lastDp].Flights.Count - 1;
                    lastDomArr = (trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 > lastDomArr) ?
                                    trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 : lastDomArr;
                }
            }

            int hours = lastDomArr / 60;
            int minutes = lastDomArr % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);
        }

        private TimeSpan CalcLastDomArrTimeDrop(Line line)
        {
            Trip trip = null;

            int lastDomArr = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    int lastDp = trip.DutyPeriods.Count - 1;
                    int lastFlt = trip.DutyPeriods[lastDp].Flights.Count - 1;
                    lastDomArr = (trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 > lastDomArr) ?
                                    trip.DutyPeriods[lastDp].Flights[lastFlt].ArrTime - lastDp * 1440 : lastDomArr;
                }
            }

            int hours = lastDomArr / 60;
            int minutes = lastDomArr % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            return new TimeSpan(actualhours, minutes, 0);
        }


        #endregion

        #region LegsPerPair

        private decimal CalcLegsPerPairVacation(Line line)
        {

            decimal legsPerPair = 0.0m;
            Trip trip = null;

            int pairingCount = line.Pairings.Count;
            if (!line.ReserveLine && !line.BlankLine)
            {

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }


                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            pairingCount--;
                        }

                    }
                }


                // legsPerPair = pairingCount == 0 ? 0.00m : Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2, MidpointRounding.AwayFromZero);
                legsPerPair = pairingCount == 0 ? 0.00m : Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2);
                line.TotPairings = pairingCount;


            }

            return legsPerPair;


        }


        private decimal CalcLegsPerPairDrop(Line line)
        {

            decimal legsPerPair = 0.0m;
            Trip trip = null;

            int pairingCount = line.Pairings.Count;
            if (!line.ReserveLine && !line.BlankLine)
            {

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        pairingCount--;
                    }
                }


                //                legsPerPair = Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2, MidpointRounding.AwayFromZero);
                legsPerPair = Math.Round(pairingCount == 0 ? 0 : (Convert.ToDecimal(line.Legs) / Convert.ToDecimal(pairingCount)), 2);
                line.TotPairings = pairingCount;


            }

            return legsPerPair;


        }

        #endregion

        #region StartDow

        private string CalcStartDow(Line line)
        {
            Trip trip = null;
            string sdow = " "; ;
            int startDowInt = 0;
            int oldDowInt = 9;
            int nextDate = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                int date = Convert.ToInt16(pairing.Substring(4, 2));
                int lenghtOfTrip = trip.PairLength;
                if (date != nextDate)
                {
                    nextDate = date + lenghtOfTrip;
                    startDowInt = Convert.ToInt32(WBidCollection.SetDate(date, isLastTrip).DayOfWeek);

                    //startDowInt = Convert.ToInt32(new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), date).DayOfWeek);
                    oldDowInt = oldDowInt == 9 ? startDowInt : oldDowInt;
                }
                else nextDate = date + lenghtOfTrip;

                if (startDowInt != oldDowInt)
                {
                    return "mix";
                }
            }
            switch (startDowInt)
            {
                case 0:
                    sdow = Dow.Sun.ToString();
                    break;
                case 1:
                    sdow = Dow.Mon.ToString();
                    break;
                case 2:
                    sdow = Dow.Tue.ToString();
                    break;
                case 3:
                    sdow = Dow.Wed.ToString();
                    break;
                case 4:
                    sdow = Dow.Thu.ToString();
                    break;
                case 5:
                    sdow = Dow.Fri.ToString();
                    break;
                case 6:
                    sdow = Dow.Sat.ToString();
                    break;
                default:
                    break;
            }
            return sdow;
        }

        private string CalcStartDowVacation(Line line)
        {
            Trip trip = null;
            VacationStateTrip vacTrip = null;
            DateTime oldTripEndDate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            string sdow = string.Empty;
            int lenghtOfTrip = 0;
            int dpIndex = 0;
            int paringCount = 0;
            int startDowInt = 0;
            int oldDowInt = 9;
            bool isLastTrip = false;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                vacTrip = null;

                //Check lines having vacation trips
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    // VA trip
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        dpIndex = 0;
                        tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                        foreach (var dp in trip.DutyPeriods)
                        {

                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                tripStartDate = tripStartDate.AddDays(dpIndex);
                                lenghtOfTrip = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                                break;
                            }

                            dpIndex++;

                        }
                    }
                }
                else
                {
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    // tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                    lenghtOfTrip = trip.PairLength;
                }


                if (tripStartDate != oldTripEndDate.AddDays(1))
                {

                    startDowInt = Convert.ToInt32(tripStartDate.DayOfWeek);
                    oldDowInt = (oldDowInt == 9) ? startDowInt : oldDowInt;
                }

                oldTripEndDate = tripStartDate.AddDays(lenghtOfTrip - 1);

                if (startDowInt != oldDowInt)
                {
                    return "mix";
                }
            }
            switch (startDowInt)
            {
                case 0:
                    sdow = Dow.Sun.ToString();
                    break;
                case 1:
                    sdow = Dow.Mon.ToString();
                    break;
                case 2:
                    sdow = Dow.Tue.ToString();
                    break;
                case 3:
                    sdow = Dow.Wed.ToString();
                    break;
                case 4:
                    sdow = Dow.Thu.ToString();
                    break;
                case 5:
                    sdow = Dow.Fri.ToString();
                    break;
                case 6:
                    sdow = Dow.Sat.ToString();
                    break;
                default:
                    break;
            }
            return sdow;
        }

        private string CalcStartDowDrop(Line line)
        {
            Trip trip = null;

            int startDowInt = 0;
            int oldDowInt = 9;

            DateTime oldTripEndDate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            string sdow = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {


                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    // tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                    int lenghtOfTrip = trip.PairLength;
                    if (tripStartDate != oldTripEndDate.AddDays(1))
                    {

                        startDowInt = Convert.ToInt32(tripStartDate.DayOfWeek);
                        oldDowInt = (oldDowInt == 9) ? startDowInt : oldDowInt;
                    }

                    oldTripEndDate = tripStartDate.AddDays(lenghtOfTrip - 1);

                    if (startDowInt != oldDowInt)
                    {
                        return "mix";
                    }
                }
            }
            switch (startDowInt)
            {
                case 0:
                    sdow = Dow.Sun.ToString();
                    break;
                case 1:
                    sdow = Dow.Mon.ToString();
                    break;
                case 2:
                    sdow = Dow.Tue.ToString();
                    break;
                case 3:
                    sdow = Dow.Wed.ToString();
                    break;
                case 4:
                    sdow = Dow.Thu.ToString();
                    break;
                case 5:
                    sdow = Dow.Fri.ToString();
                    break;
                case 6:
                    sdow = Dow.Sat.ToString();
                    break;
                default:
                    break;
            }
            return sdow;
        }

        #endregion
        private List<int> CalcStartDowList(Line line)
        {
            var startdays = new List<int>(new int[7]);

            int paringCount = 0;

            DateTime oldTripEndDate = DateTime.MinValue;
            foreach (var pairing in line.Pairings)
            {
                Trip trip = GetTrip(pairing);
                bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                if (oldTripEndDate.AddDays(1) != tripStartDate)
                {

                    switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                    {
                        case 0:
                            startdays[6]++; //sun
                            break;
                        case 1:
                            startdays[0]++; //Mon
                            break;
                        case 2:
                            startdays[1]++;
                            break;
                        case 3:
                            startdays[2]++;
                            break;
                        case 4:
                            startdays[3]++;
                            break;
                        case 5:
                            startdays[4]++;
                            break;
                        case 6:
                            startdays[5]++;
                            break;
                    }
                }

                oldTripEndDate = tripStartDate.AddDays(trip.PairLength - 1);
            }






            return startdays;
        }

        private List<int> CalcStartDowListVacation(Line line)
        {
            var startdays = new List<int>(new int[7]);
            int paringCount = 0;
            var oldTripEndDate = DateTime.MinValue;
            VacationStateTrip vacTrip = null;
            foreach (var pairing in line.Pairings)
            {
                Trip trip = GetTrip(pairing);
                bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                int tripLength = trip.PairLength - 1;


                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == pairing);
                }


                if (vacTrip != null)
                {

                    if (vacTrip.TripType == "VA")
                    {
                        continue;
                    }
                    else if (vacTrip.TripType == "VOF")
                    {
                        tripLength = tripLength - vacTrip.VacationDutyPeriods.Count(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split");
                    }
                    else if (vacTrip.TripType == "VOB")
                    {
                        int splitOrVdCount = vacTrip.VacationDutyPeriods.Count(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split");
                        tripLength = tripLength - splitOrVdCount;
                        tripStartDate = tripStartDate.AddDays(splitOrVdCount);
                    }

                }

                if (oldTripEndDate.AddDays(1) != tripStartDate)
                {

                    switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                    {
                        case 0:
                            startdays[6]++; //sun
                            break;
                        case 1:
                            startdays[0]++; //Mon
                            break;
                        case 2:
                            startdays[1]++;
                            break;
                        case 3:
                            startdays[2]++;
                            break;
                        case 4:
                            startdays[3]++;
                            break;
                        case 5:
                            startdays[4]++;
                            break;
                        case 6:
                            startdays[5]++;
                            break;
                    }
                }

                oldTripEndDate = tripStartDate.AddDays(tripLength);
            }

            return startdays;
        }

        private List<int> CalcStartDowListDrop(Line line)
        {
            var startdays = new List<int>(new int[7]);
            int paringCount = 0;
            var oldTripEndDate = DateTime.MinValue;
            VacationStateTrip vacTrip = null;
            foreach (var pairing in line.Pairings)
            {
                Trip trip = GetTrip(pairing);
                bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                int tripLength = trip.PairLength - 1;


                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == pairing);
                }


                if (vacTrip != null)
                {
                    continue;

                }

                if (oldTripEndDate.AddDays(1) != tripStartDate)
                {

                    switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                    {
                        case 0:
                            startdays[6]++; //sun
                            break;
                        case 1:
                            startdays[0]++; //Mon
                            break;
                        case 2:
                            startdays[1]++;
                            break;
                        case 3:
                            startdays[2]++;
                            break;
                        case 4:
                            startdays[3]++;
                            break;
                        case 5:
                            startdays[4]++;
                            break;
                        case 6:
                            startdays[5]++;
                            break;
                    }
                }

                oldTripEndDate = tripStartDate.AddDays(tripLength);
            }

            return startdays;
        }

        #region StratDayOfweekListPerTrip
        //private List<int> (Line line)
        private List<int> CalcStartDowListPerTrip(Line line)
        {
            var startdays = new List<int>(new int[7]);

            int paringCount = 0;

            DateTime oldTripEndDate = DateTime.MinValue;
            foreach (var pairing in line.Pairings)
            {
                Trip trip = GetTrip(pairing);
                bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                {
                    case 0:
                        startdays[6]++; //sun
                        break;
                    case 1:
                        startdays[0]++; //Mon
                        break;
                    case 2:
                        startdays[1]++;
                        break;
                    case 3:
                        startdays[2]++;
                        break;
                    case 4:
                        startdays[3]++;
                        break;
                    case 5:
                        startdays[4]++;
                        break;
                    case 6:
                        startdays[5]++;
                        break;
                }
            }

            return startdays;
        }

        private List<int> CalcStartDowListPerTripVacation(Line line)
        {
            var startdays = new List<int>(new int[7]);
            int paringCount = 0;

            VacationStateTrip vacTrip = null;
            foreach (var pairing in line.Pairings)
            {
                Trip trip = GetTrip(pairing);
                bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                int tripLength = trip.PairLength - 1;


                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == pairing);
                }


                if (vacTrip != null)
                {

                    if (vacTrip.TripType == "VA")
                    {
                        continue;
                    }
                    else if (vacTrip.TripType == "VOF")
                    {
                        tripLength = tripLength - vacTrip.VacationDutyPeriods.Count(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split");
                    }
                    else if (vacTrip.TripType == "VOB")
                    {
                        int splitOrVdCount = vacTrip.VacationDutyPeriods.Count(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split");
                        tripLength = tripLength - splitOrVdCount;
                        tripStartDate = tripStartDate.AddDays(splitOrVdCount);
                    }

                }


                switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                {
                    case 0:
                        startdays[6]++; //sun
                        break;
                    case 1:
                        startdays[0]++; //Mon
                        break;
                    case 2:
                        startdays[1]++;
                        break;
                    case 3:
                        startdays[2]++;
                        break;
                    case 4:
                        startdays[3]++;
                        break;
                    case 5:
                        startdays[4]++;
                        break;
                    case 6:
                        startdays[5]++;
                        break;
                }
            }



            return startdays;
        }

        private List<int> CalcStartDowListPerTripDrop(Line line)
        {
            var startdays = new List<int>(new int[7]);
            int paringCount = 0;

            VacationStateTrip vacTrip = null;
            foreach (var pairing in line.Pairings)
            {
                Trip trip = GetTrip(pairing);
                bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                int tripLength = trip.PairLength - 1;


                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == pairing);
                }


                if (vacTrip != null)
                {
                    continue;

                }



                switch (Convert.ToInt32(tripStartDate.DayOfWeek))
                {
                    case 0:
                        startdays[6]++; //sun
                        break;
                    case 1:
                        startdays[0]++; //Mon
                        break;
                    case 2:
                        startdays[1]++;
                        break;
                    case 3:
                        startdays[2]++;
                        break;
                    case 4:
                        startdays[3]++;
                        break;
                    case 5:
                        startdays[4]++;
                        break;
                    case 6:
                        startdays[5]++;
                        break;
                }
            }



            return startdays;
        }

        #endregion
        #region T234


        private string CalcT234(Line line)
        {

            string T234 = string.Empty;

            T234 = line.Trips1Day > 9 ? "*" : line.Trips1Day.ToString();
            T234 += line.Trips2Day > 9 ? "*" : line.Trips2Day.ToString();
            T234 += line.Trips3Day > 9 ? "*" : line.Trips3Day.ToString();
            T234 += line.Trips4Day > 9 ? "*" : line.Trips4Day.ToString();
            return T234;
        }

        #endregion

        #region BlkOfDaysOff
        private List<int> CalcBlkOfDaysOff(Line line)
        {
            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            Trip trip = null;
            if (line.BlankLine) return blkOff;

            DateTime oldPairingEndDate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            int daysOff = 0;

            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //Get trip
                trip = GetTrip(pairing);
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                // tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                daysOff = tripDate.Subtract(oldPairingEndDate).Days - 1;
                if (daysOff > 0)
                {
                    blkOff[daysOff]++;
                }
                oldPairingEndDate = tripDate.AddDays(trip.PairLength - 1);

            }

            if (oldPairingEndDate < _bpEndDate)
            {
                daysOff = _bpEndDate.Subtract(oldPairingEndDate).Days - 1;
                blkOff[daysOff]++;
            }

            return blkOff;


        }

        private List<int> CalcBlkOfDaysOffVacation(Line line)
        {


            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            try
            {
                Trip trip = null;
                if (line.BlankLine) return blkOff;
                bool isLastTrip = false; int paringCount = 0;
                DateTime oldPairingEndDate = _bpStartDate.AddDays(-1);
                DateTime tripDate = DateTime.MinValue;
                int daysOff = 0;
                int pairLength = 0;
                foreach (string pairing in line.Pairings)
                {
                    //Get trip
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }


                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            tripDate = vacTrip.TripVacationStartDate;
                            pairLength = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count(); ;
                        }
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        // tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                        pairLength = trip.PairLength;
                    }



                    daysOff = tripDate.Subtract(oldPairingEndDate).Days - 1;
                    if (daysOff > 0)
                    {
                        blkOff[daysOff]++;
                    }
                    oldPairingEndDate = tripDate.AddDays(pairLength - 1);

                }

                if (oldPairingEndDate < _bpEndDate)
                {
                    daysOff = _bpEndDate.Subtract(oldPairingEndDate).Days - 1;
                    blkOff[daysOff]++;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return blkOff;


        }

        private List<int> CalcBlkOfDaysOffDrop(Line line)
        {
            //int[] blkOff = new int[20];
            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            Trip trip = null;
            if (line.BlankLine) return blkOff;

            DateTime oldPairingEndDate = _bpStartDate.AddDays(-1);
            DateTime tripDate = DateTime.MinValue;
            int daysOff = 0;
            try
            {
                bool isLastTrip = false; int paringCount = 0;

                foreach (string pairing in line.Pairings)
                {
                    //Get trip
                    trip = GetTrip(pairing);

                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {
                        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        //tripDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                        daysOff = tripDate.Subtract(oldPairingEndDate).Days - 1;
                        if (daysOff > 0)
                        {
                            blkOff[daysOff]++;
                        }
                        oldPairingEndDate = tripDate.AddDays(trip.PairLength - 1);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (oldPairingEndDate < _bpEndDate)
            {
                daysOff = _bpEndDate.Subtract(oldPairingEndDate).Days - 1;
                blkOff[daysOff]++;
            }

            return blkOff;


        }

        #endregion

        #region LegsPerDutyPeriod
        private List<int> CalcLegsPerDutyPeriod(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>();
            for (int count = 0; count < 10; count++)
                arrayOfDeadheads.Add(0);
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                {
                    arrayOfDeadheads[dutyPeriod.TotFlights]++;
                }

            }

            return arrayOfDeadheads;
        }

        private List<int> CalcLegsPerDutyPeriodVacation(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>();
            for (int count = 0; count < 10; count++)
                arrayOfDeadheads.Add(0);
            int dpIndex = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);


                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        dpIndex = 0;
                        foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                arrayOfDeadheads[dutyPeriod.TotFlights]++;
                            }
                            else if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                arrayOfDeadheads[vacTrip.VacationDutyPeriods[dpIndex].VacationFlights.Where(x => x.FlightType == "VD").Count()]++;
                            }
                            dpIndex++;
                        }
                    }
                }
                else
                {
                    foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                    {
                        arrayOfDeadheads[dutyPeriod.TotFlights]++;
                    }
                }



            }

            return arrayOfDeadheads;
        }

        private List<int> CalcLegsPerDutyPeriodDrop(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>();
            for (int count = 0; count < 10; count++)
                arrayOfDeadheads.Add(0);
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                    {
                        arrayOfDeadheads[dutyPeriod.TotFlights]++;
                    }
                }

            }

            return arrayOfDeadheads;
        }


        #endregion

        #region WeekDaysWork

        private List<int> CalcWeekDaysWork(Line line)
        {
            Trip trip = null;
            List<int> weekWorkingDays = new List<int>();
            for (int count = 0; count < 7; count++)
                weekWorkingDays.Add(0);
            DateTime wkDay;
            int dayOfWeek;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                int lengthTrip = trip.PairLength;
                wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip);
                dayOfWeek = (int)wkDay.DayOfWeek - 1;
                dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                weekWorkingDays[dayOfWeek]++;

                for (int count = 1; count < lengthTrip; count++)
                {
                    wkDay = wkDay.AddDays(1);
                    dayOfWeek = (int)wkDay.DayOfWeek - 1;
                    dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                    weekWorkingDays[dayOfWeek]++;
                }
            }
            return weekWorkingDays;
        }

        private List<int> CalcWeekDaysWorkVacation(Line line)
        {
            Trip trip = null;

            List<int> weekWorkingDays = new List<int>();
            for (int count = 0; count < 7; count++)
                weekWorkingDays.Add(0);
            DateTime tripDate;
            int dayOfWeek;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                dayOfWeek = (int)tripDate.DayOfWeek - 1;
                                dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                                weekWorkingDays[dayOfWeek]++;
                            }

                            tripDate = tripDate.AddDays(1);
                        }

                    }
                }
                else
                {

                    for (int count = 0; count < trip.PairLength; count++)
                    {
                        dayOfWeek = (int)tripDate.DayOfWeek - 1;
                        dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                        weekWorkingDays[dayOfWeek]++;
                        tripDate = tripDate.AddDays(1);
                    }

                }



            }

            return weekWorkingDays;
        }

        private List<int> CalcWeekDaysWorkDrop(Line line)
        {
            Trip trip = null;

            List<int> weekWorkingDays = new List<int>();
            for (int count = 0; count < 7; count++)
                weekWorkingDays.Add(0);
            DateTime tripDate;
            int dayOfWeek;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                    for (int count = 0; count < trip.PairLength; count++)
                    {
                        dayOfWeek = (int)tripDate.DayOfWeek - 1;
                        dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                        weekWorkingDays[dayOfWeek]++;
                        tripDate = tripDate.AddDays(1);
                    }
                }


            }

            return weekWorkingDays;
        }


        #endregion

        #region DaysOfMonthOff
        private List<WorkDaysOfBidLine> CalcDaysOfMonthOff(Line line)
        {
            Trip trip = null;
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;
            DateTime wkDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //  wkDay = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                wkDay = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                for (int i = 1; i <= trip.PairLength; i++)
                {
                    workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                    daysWork.Add(workDay);
                    wkDay = wkDay.AddDays(1);
                }
            }

            return daysWork;
        }

        private List<WorkDaysOfBidLine> CalcDaysOfMonthOffVacation(Line line)
        {
            Trip trip = null;
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;
            DateTime wkDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //wkDay = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                wkDay = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                                daysWork.Add(workDay);
                            }
                            wkDay = wkDay.AddDays(1);
                        }

                    }
                }
                else
                {

                    for (int i = 1; i <= trip.PairLength; i++)
                    {
                        workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                        daysWork.Add(workDay);
                        wkDay = wkDay.AddDays(1);
                    }
                }
            }

            return daysWork;
        }

        private List<WorkDaysOfBidLine> CalcDaysOfMonthOffDrop(Line line)
        {
            Trip trip = null;
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;
            DateTime wkDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {

                    wkDay = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //wkDay = new DateTime(int.Parse(_year), int.Parse(_month), int.Parse(pairing.Substring(4, 2)));
                    for (int i = 1; i <= trip.PairLength; i++)
                    {
                        workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                        daysWork.Add(workDay);
                        wkDay = wkDay.AddDays(1);
                    }
                }
            }

            return daysWork;
        }

        private int CalDaysOffEOMDrop(Line line)
        {
            int daysOff = 0;

            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            //daysOff += vacationTrip.DutyPeriodsDetails.Where(x => (x.VacationType == "VD" || x.VacationType == "Split") && x.isInBp).Count(); ;
                            //commented by Roshil on 7-6-2021 as we have already added split in CalDaysOffEOM
                            daysOff += vacationTrip.DutyPeriodsDetails.Where(x => (x.VacationType == "VD") && x.isInBp).Count();
                        }




                    }



                }

            }
            return daysOff;

        }
        private int CalDaysOffEOM(Line line)
        {
            int daysOff = 0;
            if (line.LineNum == 15)
            {
            }
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            daysOff += vacationTrip.DutyPeriodsDetails.Where(x => (x.VacationType == "VO" || x.VacationType == "Split") && x.isInBp).Count(); ;
                        }




                    }



                }

            }
            return daysOff;

        }

        #endregion

        #region OvernightCities
        private void CalcOvernightCities(Line line)
        {
            Trip trip = null;
            line.OvernightCities = new List<string>();
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                foreach (var dp in trip.DutyPeriods)
                {
                    if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                    {   // only adds overnights and not the last day of trip
                        line.OvernightCities.Add(dp.ArrStaLastLeg);
                    }
                }
            }
        }

        private void CalcOvernightCitiesVacation(Line line)
        {
            Trip trip = null;
            line.OvernightCities = new List<string>();
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }


                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                                {   // only adds overnights and not the last day of trip
                                    line.OvernightCities.Add(dp.ArrStaLastLeg);
                                }

                            }

                        }

                    }
                }
                else
                {

                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                        {   // only adds overnights and not the last day of trip
                            line.OvernightCities.Add(dp.ArrStaLastLeg);
                        }
                    }
                }
            }
        }

        private void CalcOvernightCitiesDrop(Line line)
        {
            Trip trip = null;
            line.OvernightCities = new List<string>();
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                        {   // only adds overnights and not the last day of trip
                            line.OvernightCities.Add(dp.ArrStaLastLeg);
                        }
                    }
                }
            }
        }
        #endregion

        #region RestPeriods

        private List<RestPeriod> CalcRestPeriods(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();
            Trip trip = null;
            int periodId = 1;
            RestPeriod restPeriod = null;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //DateTime tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));

                // int rpt = 0;
                int rls = GlobalSettings.debrief;
                foreach (var dp in trip.DutyPeriods)
                {
                    //if not the first dutyperiod then we need to add one  to the tripstartdate
                    if (dp.DutPerSeqNum != 1)
                    {
                        tripStartDate = tripStartDate.AddDays(1);

                    }
                    // rpt = (dp.DutPerSeqNum == 1) ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;

                    restPeriod = new RestPeriod();
                    restPeriod.PeriodId = periodId++;
                    restPeriod.IsInTrip = IsInTrip;
                    // restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.DepTimeFirstLeg - ((dp.DutPerSeqNum - 1) * 1440) - rpt).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                    restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440)).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                    lstRestPeriod.Add(restPeriod);

                    //Finding the status ,is in trip or 'between trip'
                    IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                    lastDutyEndDate = tripStartDate.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);


                }
            }
            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }
            return lstRestPeriod;
        }

        private List<RestPeriod> CalcRestPeriodsVacation(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();
            Trip trip = null;
            int periodId = 1;
            RestPeriod restPeriod = null;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            int rpt = 0;
            int rls = GlobalSettings.debrief;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }

                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }
                    else
                    {

                        int depTimeFirstLeg = 0;
                        int landTimeLastLeg = 0;
                        int flightSequenceNumber = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            depTimeFirstLeg = 0;
                            flightSequenceNumber = 0;
                            landTimeLastLeg = 0;
                            //rpt = (dp.DutPerSeqNum == 1) ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;

                            if (vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].DutyPeriodType == "Split")
                            {
                                try
                                {
                                    flightSequenceNumber = vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].VacationFlights.FirstOrDefault(x => x.FlightType == "VD").FlightSeqNo;
                                }
                                catch (Exception ex)
                                {

                                    throw ex;
                                }
                                restPeriod = new RestPeriod();
                                restPeriod.PeriodId = periodId++;
                                restPeriod.IsInTrip = IsInTrip;
                                depTimeFirstLeg = dp.Flights[flightSequenceNumber - 1].DepTime;
                                restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(depTimeFirstLeg - ((dp.DutPerSeqNum - 1) * 1440) - rpt).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                                lstRestPeriod.Add(restPeriod);
                                //Finding the status ,is in trip or 'between trip'
                                IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                                flightSequenceNumber = vacTrip.VacationDutyPeriods[dp.DutPerSeqNum - 1].VacationFlights.LastOrDefault(x => x.FlightType == "VD").FlightSeqNo;
                                landTimeLastLeg = dp.Flights[flightSequenceNumber - 1].ArrTime;
                                lastDutyEndDate = tripStartDate.AddMinutes(landTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);

                            }

                            if (dp.DutPerSeqNum != 1)
                            {
                                tripStartDate = tripStartDate.AddDays(1);

                            }

                        }

                    }
                }
                else
                {
                    foreach (var dp in trip.DutyPeriods)
                    {
                        //if not the first dutyperiod then we need to add one  to the tripstartdate
                        if (dp.DutPerSeqNum != 1)
                        {
                            tripStartDate = tripStartDate.AddDays(1);

                        }
                        restPeriod = new RestPeriod();
                        restPeriod.PeriodId = periodId++;
                        restPeriod.IsInTrip = IsInTrip;
                        restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440)).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                        lstRestPeriod.Add(restPeriod);

                        //Finding the status ,is in trip or 'between trip'
                        IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                        lastDutyEndDate = tripStartDate.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);


                    }
                }
            }


            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }
            return lstRestPeriod;
        }

        private List<RestPeriod> CalcRestPeriodsDrop(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();
            Trip trip = null;
            int periodId = 1;
            RestPeriod restPeriod = null;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                trip = GetTrip(pairing);
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    int rls = GlobalSettings.debrief;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        //if not the first dutyperiod then we need to add one  to the tripstartdate
                        if (dp.DutPerSeqNum != 1)
                        {
                            tripStartDate = tripStartDate.AddDays(1);

                        }

                        restPeriod = new RestPeriod();
                        restPeriod.PeriodId = periodId++;
                        restPeriod.IsInTrip = IsInTrip;
                        restPeriod.RestMinutes = int.Parse(tripStartDate.AddMinutes(dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440)).Subtract(lastDutyEndDate).TotalMinutes.ToString());
                        lstRestPeriod.Add(restPeriod);

                        //Finding the status ,is in trip or 'between trip'
                        IsInTrip = !(dp.DutPerSeqNum == trip.DutyPeriods.Count && dp.ArrStaLastLeg == GlobalSettings.CurrentBidDetails.Domicile);

                        lastDutyEndDate = tripStartDate.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440) + rls);


                    }
                }
            }


            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }

            return lstRestPeriod;




        }

        #endregion

        #region WorkBlockLength

        private void CalculateWorkBlockLength(Line line)
        {

            Trip trip = null;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            DateTime tripStartDate, tripPreviousEndDate;
            int workBlockLength = 0;
            tripPreviousEndDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));

                if (workBlockLength != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }
                tripPreviousEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                workBlockLength += trip.DutyPeriods.Count;
            }

            line.WorkBlockLengths[workBlockLength]++;
        }


        private void CalculateWorkBlockLengthVacation(Line line)
        {
            Trip trip = null;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            DateTime tripStartDate, tripPreviousEndDate;
            int workBlockLength = 0;
            tripPreviousEndDate = DateTime.MinValue;
            int tripDays = 0;
            int dpIndex = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //  tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));
                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                    {
                        continue;
                    }

                    dpIndex = 0;
                    foreach (var dp in trip.DutyPeriods)
                    {
                        if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                        {
                            tripStartDate = tripStartDate.AddDays(dpIndex);
                            tripDays = vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count();
                            break;
                        }
                        dpIndex++;
                    }

                }
                else
                {
                    tripDays = trip.DutyPeriods.Count;
                }

                if (workBlockLength != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }
                tripPreviousEndDate = tripStartDate.AddDays(tripDays - 1);
                workBlockLength += tripDays;
            }

            if (workBlockLength != 0)
            {
                line.WorkBlockLengths[workBlockLength]++;
            }
        }
        private void CalculateWorkBlockLengthMIL(Line line)
        {
            DateTime previousdpdate = DateTime.MinValue;
            DateTime tripStartDate = DateTime.MinValue;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            var work = line.BidLineTemplates.Where(x => x.workStatus == "Work" || x.workStatus == "BackSplitWithoutStrike" || x.workStatus == "FrontSplitWithoutStrike" || x.workStatus == "BackSplitWork" || x.workStatus == "FrontSplitWork");
            var firstTrip = work.FirstOrDefault();
            if (firstTrip != null)
            {
                tripStartDate = firstTrip.Date;
                previousdpdate = tripStartDate.AddDays(-1);
                int workBlockLength = 0;
                int index = 1;
                foreach (BidLineTemplate template in work)
                {
                    var tempdate = template.Date;

                    //check if is the end of workdays in the bid line template.
                    if (work.Count() == index)
                    {
                        workBlockLength++;
                        line.WorkBlockLengths[workBlockLength]++;
                    }
                    //check if is adjacent work days 
                    else if (previousdpdate.AddDays(1) != tempdate)
                    {
                        line.WorkBlockLengths[workBlockLength]++;
                        workBlockLength = 1;
                    }
                    else
                    {
                        workBlockLength++;
                    }
                    //tripStartDate = tempdate;
                    previousdpdate = tempdate;
                    index++;

                }
            }

        }

        private void CalculateWorkBlockLengthDrop(Line line)
        {

            Trip trip = null;
            DateTime tripStartDate, tripPreviousEndDate;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            int workBlockLength = 0;
            tripPreviousEndDate = DateTime.MinValue;
            int tripDays = 0;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));

                VacationStateTrip vacTrip = null;
                if (line.VacationStateLine.VacationTrips != null)
                {
                    vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                }
                if (vacTrip != null)
                {
                    //we dont need to consider vacation trip
                    continue;
                }
                else
                {
                    tripDays = trip.DutyPeriods.Count;
                }

                if (workBlockLength != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }
                tripPreviousEndDate = tripStartDate.AddDays(tripDays - 1);
                workBlockLength += tripDays;
            }

            if (workBlockLength != 0)
            {
                line.WorkBlockLengths[workBlockLength]++;
            }
        }

        #endregion

        #region EPush
        private TimeSpan CalcEPush(Line line)
        {
            Trip trip = null;
            int ePush = 99999999;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                dutPd = 0;
                foreach (var dp in trip.DutyPeriods)
                {
                    ePush = (dp.Flights[0].DepTime - 1440 * dutPd < ePush) ? dp.Flights[0].DepTime - dutPd * 1440 : ePush;
                    dutPd++;
                }
            }

            int hours = ePush / 60;
            int minutes = ePush % 60;
            return new TimeSpan(hours, minutes, 0);
        }

        private TimeSpan CalcEPushVacation(Line line)
        {
            try
            {


                Trip trip = null;
                int ePush = 99999999;
                int dutPdIndex = 0;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    dutPdIndex = 0;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }

                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "VD")
                            {
                                ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                            }
                            else if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "Split")
                            {
                                for (int flightIndex = 0; flightIndex < dp.Flights.Count; flightIndex++)
                                {
                                    if (vacTrip.VacationDutyPeriods[dutPdIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        ePush = (dp.Flights[flightIndex].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                                        break;
                                    }

                                }
                            }
                            dutPdIndex++;
                        }

                    }

                    else
                    {
                        foreach (var dp in trip.DutyPeriods)
                        {
                            ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                            dutPdIndex++;
                        }
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);

            }
            catch (Exception)
            {
                return CalcEPush(line);

                // throw;
            }
        }

        private TimeSpan CalcEPushDrop(Line line)
        {
            try
            {


                Trip trip = null;
                int ePush = 99999999;
                int dutPdIndex = 0;
                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    dutPdIndex = 0;


                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {
                        foreach (var dp in trip.DutyPeriods)
                        {
                            ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                            dutPdIndex++;
                        }
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);
            }
            catch (Exception)
            {
                return CalcEPush(line);
            }
        }

        #endregion

        #region EDomPush
        private TimeSpan CalcEDomPush(Line line)
        {
            Trip trip = null;
            int ePush = 99999999;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
            }

            int hours = ePush / 60;
            int minutes = ePush % 60;
            return new TimeSpan(hours, minutes, 0);

        }


        private TimeSpan CalcEDomPushVacation(Line line)
        {
            try
            {

                Trip trip = null;
                int ePush = 99999999;
                int dutPdIndex = 0;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        dutPdIndex = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "VD")
                            {
                                ePush = (dp.Flights[0].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                                break;
                            }
                            else if (vacTrip.VacationDutyPeriods[dutPdIndex].DutyPeriodType == "Split")
                            {
                                for (int flightIndex = 0; flightIndex < dp.Flights.Count; flightIndex++)
                                {
                                    if (vacTrip.VacationDutyPeriods[dutPdIndex].VacationFlights[flightIndex].FlightType == "VD")
                                    {
                                        ePush = (dp.Flights[flightIndex].DepTime - 1440 * dutPdIndex < ePush) ? dp.Flights[0].DepTime - dutPdIndex * 1440 : ePush;
                                        break;
                                    }

                                }
                                break;
                            }
                            dutPdIndex++;
                        }
                    }
                    else
                    {

                        ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);

            }
            catch (Exception)
            {

                return CalcEDomPush(line);
            }

        }


        private TimeSpan CalcEDomPushDrop(Line line)
        {
            try
            {


                Trip trip = null;
                int ePush = 99999999;
                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {
                        ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
                    }
                }

                int hours = ePush / 60;
                int minutes = ePush % 60;
                return new TimeSpan(hours, minutes, 0);

            }
            catch (Exception)
            {

                return CalcEDomPush(line);
            }

        }

        #endregion

        #region WorkBlockDetails

        /// <summary>
        /// PURPOSE : Calculate work block details, count backToback (btb) events for each work block
        /// </summary>
        /// <param name="line"></param>
        //private void CalculateWorkBlockDetails(Line line)
        //{

        //    if (line.BlankLine) return;
        //    List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
        //    WorkBlockDetails workBlockDetails = new WorkBlockDetails();
        //    Trip trip = null;
        //    DateTime tripStartDate, tripEndDate, tripPreviousEndDate;

        //    tripPreviousEndDate = DateTime.Now;
        //    int tripPreviousLandTime = 0;
        //    int count = 0;

        //    int backToBack = 0;
        //    bool isLastTrip = false; int paringCount = 0;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

        //        trip = GetTrip(pairing);
        //        tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
        //        //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2))); ;
        //        if (count == 0)
        //        {
        //            workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.show1stDay;
        //            workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
        //        }
        //        else if (count != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
        //        {
        //            workBlockDetails.EndTime = tripPreviousLandTime;
        //            workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
        //            workBlockDetails.BackToBackCount = backToBack - 1;
        //            lstWorkBlockDetails.Add(workBlockDetails);

        //            workBlockDetails = new WorkBlockDetails();
        //            workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.show1stDay;
        //            workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;

        //            backToBack = 0;

        //        }

        //        tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
        //        tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
        //        tripPreviousEndDate = tripEndDate;
        //        count++;
        //        backToBack++;
        //    }

        //    workBlockDetails.EndTime = tripPreviousLandTime;
        //    workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
        //    workBlockDetails.BackToBackCount = backToBack - 1;
        //    lstWorkBlockDetails.Add(workBlockDetails);

        //    line.WorkBlockList = lstWorkBlockDetails;


        //}


        private void CalculateWorkBlockDetails(Line line)
        {

            if (line.BlankLine) return;
            List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
            WorkBlockDetails workBlockDetails = new WorkBlockDetails();
            Trip trip = null;
            DateTime tripStartDate, tripEndDate, tripPreviousEndDate;

            tripPreviousEndDate = DateTime.Now;
            int tripPreviousLandTime = 0;
            int tripPreviousStartTime = 0;
            int count = 0;

            int backToBack = 0;
            bool isLastTrip = false; int paringCount = 0;
            int nightinMid = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                trip = GetTrip(pairing);
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2))); ;
                if (count == 0)
                {
                    if (trip.ReserveTrip)
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut - GlobalSettings.ReserveBriefTime;
                        workBlockDetails.BriefTime = GlobalSettings.ReserveBriefTime;

                    }
                    else
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime;
                        workBlockDetails.BriefTime = trip.BriefTime;
                    }
                    workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime;
                    workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                    //Modified as part of BA Filter
                    workBlockDetails.StartDateTime = tripStartDate.Date.AddMinutes(workBlockDetails.StartTime);
                }
                else if (count != 0 && tripPreviousEndDate.AddDays(1) == tripStartDate)
                {
                    nightinMid++;
                }
                else if (count != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    workBlockDetails.EndTime = tripPreviousLandTime;
                    workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;

                    workBlockDetails.EndDate = tripPreviousEndDate.Date;
                    if (tripPreviousStartTime > tripPreviousLandTime)
                    {
                        // if the trip ends on the next day, we have to check the full days to check the trip is commutable.
                        workBlockDetails.EndDateTime = tripPreviousEndDate.AddMinutes(1439);
                    }
                    else
                    {
                        //Modified as part of BA Filter
                        workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
                    }

                    workBlockDetails.BackToBackCount = backToBack - 1;
                    workBlockDetails.nightINDomicile = nightinMid;
                    nightinMid = 0;
                    lstWorkBlockDetails.Add(workBlockDetails);

                    workBlockDetails = new WorkBlockDetails();
                    if (trip.ReserveTrip)
                    {

                        //modified on sep 21 2018, bause the start time coming as previous value.
                        workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut - GlobalSettings.ReserveBriefTime;
                        workBlockDetails.BriefTime = GlobalSettings.ReserveBriefTime;
                    }
                    else
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime;
                        workBlockDetails.BriefTime = trip.BriefTime;
                    }
                    //workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime;
                    //workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.ReserveBriefTime;
                    workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                    //Modified as part of BA Filter
                    workBlockDetails.StartDateTime = tripStartDate.Date.AddMinutes(workBlockDetails.StartTime);

                    backToBack = 0;

                }

                int dpcount = 1;
                foreach (DutyPeriod dp in trip.DutyPeriods)
                {
                    if (trip.DutyPeriods.Count != dpcount && dp.Flights[dp.Flights.Count - 1].ArrSta == GlobalSettings.CurrentBidDetails.Domicile)
                        nightinMid++;
                    dpcount++;
                }
                tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                if (trip.ReserveTrip)
                {
                    tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveOut - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.ReserveBriefTime;
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveIn - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.debrief;
                }
                else
                {
                    tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[0].DepTime - trip.BriefTime - (1440 * (trip.DutyPeriods.Count - 1));
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                }
                tripPreviousEndDate = tripEndDate;
                count++;
                backToBack++;
            }

            workBlockDetails.EndTime = tripPreviousLandTime;
            workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
            //Modified as part of BA Filter
            workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
            workBlockDetails.EndDate = tripPreviousEndDate.Date;
            workBlockDetails.nightINDomicile = nightinMid;
            workBlockDetails.BackToBackCount = backToBack - 1;
            lstWorkBlockDetails.Add(workBlockDetails);

            line.WorkBlockList = lstWorkBlockDetails;


        }

        /// <summary>
        /// PURPOSe : Calculate CalculateAverageLatestDomicileArrival
        /// </summary>
        /// <param name="line"></param>
        /// <param name="inLine"></param>
        private void CalculateAverageLatestDomicileArrivalAndPush(Line line)
        {

            if (line.BlankLine) return;
            List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
            WorkBlockDetails workBlockDetails = new WorkBlockDetails();
            Trip trip = null;
            DateTime tripStartDate, tripPreviousEndDate;

            tripPreviousEndDate = DateTime.MinValue;
            int tripPreviousLandTime = 0;
            int count = 0;

            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                //trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                if (count == 0)
                {
                    if (trip.ReserveTrip)
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut; ;
                    }
                    else
                    {

                        workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg;
                    }
                    workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                }
                else if (count != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    workBlockDetails.EndTime = tripPreviousLandTime;
                    lstWorkBlockDetails.Add(workBlockDetails);

                    workBlockDetails = new WorkBlockDetails();
                    if (trip.ReserveTrip)
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut; ;
                    }
                    else
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg;
                    }
                }
                if (trip.ReserveTrip)
                {
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveIn - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.debrief;
                }
                else
                {
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                }
                tripPreviousEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                count++;
            }

            workBlockDetails.EndTime = tripPreviousLandTime;
            lstWorkBlockDetails.Add(workBlockDetails);

            int avglatestDomArrivalTime = (int)Math.Round(Convert.ToDouble(lstWorkBlockDetails.Sum(x => x.EndTime) / lstWorkBlockDetails.Count), 2);
            int hours = avglatestDomArrivalTime / 60;
            int minutes = avglatestDomArrivalTime % 60;
            int actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            line.AvgLatestDomArrivalTime = new TimeSpan(actualhours, minutes, 0);

            int avgEarliestDomPush = (int)Math.Round(Convert.ToDouble(lstWorkBlockDetails.Sum(x => x.StartTime) / lstWorkBlockDetails.Count), 2);
            hours = avgEarliestDomPush / 60;
            minutes = avgEarliestDomPush % 60;
            actualhours = hours % 24;
            actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
            line.AvgEarliestDomPush = new TimeSpan(actualhours, minutes, 0);

        }

        private void CalculateWorkBlockVacation(Line line)
        {

            try
            {

                if (line.BlankLine) return;
                List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
                WorkBlockDetails workBlockDetails = new WorkBlockDetails();
                Trip trip = null;
                DateTime tripStartDate, tripPreviousEndDate, tripEndDate;
                int tripPreviousLandTime = 0, tripStartTime = 0, tripEndTime = 0, tripBriefTime = 0;
                int backToBack = 0;
                bool isFirstTrip = true;
                tripPreviousEndDate = tripEndDate = DateTime.Now;
                tripPreviousLandTime = tripStartTime = 0;
                int tripPreviousStartTime = 0;
                int dpIndex = 0;
                int fltIndex = 0;
                bool isLastTrip = false; int paringCount = 0;
                int nightinMid = 0;

                foreach (var pairing in line.Pairings)
                {

                    trip = GetTrip(pairing);

                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2))); ;

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        tripStartTime = 0;
                        tripEndTime = 0;
                        dpIndex = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                if (tripStartTime == 0)
                                {
                                    if (trip.ReserveTrip)
                                    {

                                        tripStartTime = dp.ReserveOut - GlobalSettings.ReserveBriefTime;
                                        tripBriefTime = GlobalSettings.ReserveBriefTime;


                                    }
                                    else
                                    {
                                        tripStartTime = dp.DepTimeFirstLeg - trip.BriefTime;
                                        tripBriefTime = trip.BriefTime;
                                    }
                                    tripEndDate = tripStartDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                                }
                                if (trip.ReserveTrip)
                                {
                                    tripEndTime = dp.ReserveIn - (1440 * (dp.DutPerSeqNum - 1)) + GlobalSettings.debrief;
                                }
                                else
                                {
                                    tripEndTime = dp.LandTimeLastLeg - (1440 * (dp.DutPerSeqNum - 1));
                                }




                            }
                            else if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                fltIndex = 0;
                                foreach (Flight flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[fltIndex].FlightType == "VD")
                                    {
                                        if (tripStartTime == 0)
                                        {
                                            tripStartTime = dp.Flights[fltIndex].DepTime - trip.BriefTime;
                                            tripBriefTime = trip.BriefTime;
                                            tripEndDate = tripStartDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                                        }
                                        tripEndTime = dp.Flights[fltIndex].ArrTime - (1440 * (dp.DutPerSeqNum - 1));
                                    }
                                    fltIndex++;

                                }

                            }

                            //VacationStateFlight lastflt=vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[vacTrip.VacationDutyPeriods[dpIndex].VacationFlights.Count-1];
                            //if (dp.Flights[dp.Flights.Count - 1].ArrSta == GlobalSettings.CurrentBidDetails.Domicile && lastflt.FlightType=="VD")
                            //    nightinMid++;

                            if (tripStartTime == 0)
                            {
                                tripStartDate = tripStartDate.AddDays(1);
                            }

                            dpIndex++;
                        }
                    }
                    else
                    {
                        if (trip.ReserveTrip)
                        {
                            tripStartTime = trip.DutyPeriods[0].ReserveOut - GlobalSettings.ReserveBriefTime;
                            tripBriefTime = GlobalSettings.ReserveBriefTime;
                            tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveOut - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.ReserveBriefTime;
                            tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveIn - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.ReserveDebrief;
                        }
                        else
                        {
                            tripStartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime;
                            tripBriefTime = trip.BriefTime;
                            tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[0].DepTime - trip.BriefTime - (1440 * (trip.DutyPeriods.Count - 1));
                            tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));

                        }

                        tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    }
                    if (isFirstTrip)
                    {
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                        workBlockDetails.BriefTime = tripBriefTime;
                        //Modified as part of BA Filter
                        workBlockDetails.StartDateTime = tripStartDate.Date.AddMinutes(workBlockDetails.StartTime);
                    }
                    else if (!isFirstTrip && vacTrip == null && tripPreviousEndDate.AddDays(1) == tripStartDate)
                    {
                        nightinMid++;
                    }
                    else if (!isFirstTrip && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        workBlockDetails.EndTime = tripPreviousLandTime;
                        workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                        workBlockDetails.EndDate = tripPreviousEndDate.Date;
                        //Modified as part of BA Filter
                        if (tripPreviousStartTime > tripPreviousLandTime)
                        {
                            // if the trip ends on the next day, we have to check the full days to check the trip is commutable.
                            workBlockDetails.EndDateTime = tripPreviousEndDate.AddMinutes(1439);
                        }
                        else
                        {
                            workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
                        }

                        workBlockDetails.nightINDomicile = nightinMid;
                        nightinMid = 0;
                        workBlockDetails.BackToBackCount = backToBack - 1;
                        lstWorkBlockDetails.Add(workBlockDetails);

                        workBlockDetails = new WorkBlockDetails();
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.BriefTime = tripBriefTime;

                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                        //Modified as part of BA Filter
                        workBlockDetails.StartDateTime = tripStartDate.Date.AddMinutes(workBlockDetails.StartTime);
                        backToBack = 0;

                    }
                    if (vacTrip == null)
                    {
                        int dpcount = 1;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (trip.DutyPeriods.Count != dpcount && dp.Flights[dp.Flights.Count - 1].ArrSta == GlobalSettings.CurrentBidDetails.Domicile)
                                nightinMid++;
                            dpcount++;
                        }
                    }
                    else
                    {
                        dpIndex = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            VacationStateFlight lastflt = vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[vacTrip.VacationDutyPeriods[dpIndex].VacationFlights.Count - 1];
                            if (dp.Flights[dp.Flights.Count - 1].ArrSta == GlobalSettings.CurrentBidDetails.Domicile && lastflt.FlightType == "VD")
                                nightinMid++;
                        }
                    }
                    tripPreviousLandTime = tripEndTime;
                    tripPreviousEndDate = tripEndDate;
                    isFirstTrip = false;
                    backToBack++;
                }

                workBlockDetails.EndTime = tripPreviousLandTime;
                workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                workBlockDetails.EndDate = tripPreviousEndDate.Date;
                workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
                workBlockDetails.BackToBackCount = backToBack - 1;
                workBlockDetails.nightINDomicile = nightinMid;

                lstWorkBlockDetails.Add(workBlockDetails);
                line.WorkBlockList = lstWorkBlockDetails;
            }
            catch (Exception)
            {

                // throw;
            }

        }
        private void CalculateAverageLatestDomicileArrivalAndPushVacation(Line line)
        {
            try
            {



                if (line.BlankLine) return;
                List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
                WorkBlockDetails workBlockDetails = new WorkBlockDetails();
                Trip trip = null;
                DateTime tripStartDate, tripPreviousEndDate, tripEndDate;
                int tripPreviousLandTime = 0, tripStartTime = 0, tripEndTime = 0;

                bool isFirstTrip = true;
                tripPreviousEndDate = tripEndDate = DateTime.Now;
                tripPreviousLandTime = tripStartTime = 0;
                int dpIndex = 0;
                int fltIndex = 0;
                bool isLastTrip = false; int paringCount = 0;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);

                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        tripStartTime = 0;
                        tripEndTime = 0;
                        dpIndex = 0;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                            {
                                if (tripStartTime == 0)
                                {
                                    tripStartTime = dp.DepTimeFirstLeg - trip.BriefTime;
                                    tripEndDate = tripStartDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                                }
                                tripEndTime = dp.LandTimeLastLeg - (1440 * (dp.DutPerSeqNum - 1));

                            }
                            else if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split")
                            {
                                fltIndex = 0;
                                foreach (Flight flt in dp.Flights)
                                {
                                    if (vacTrip.VacationDutyPeriods[dpIndex].VacationFlights[fltIndex].FlightType == "VD")
                                    {
                                        if (tripStartTime == 0)
                                        {
                                            tripStartTime = dp.Flights[fltIndex].DepTime - trip.BriefTime;
                                            tripEndDate = tripStartDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

                                        }
                                        tripEndTime = dp.Flights[fltIndex].ArrTime - (1440 * (dp.DutPerSeqNum - 1));
                                    }
                                    fltIndex++;

                                }

                            }

                            if (tripStartTime == 0)
                            {
                                tripStartDate = tripStartDate.AddDays(1);
                            }

                            dpIndex++;
                        }
                    }
                    else
                    {
                        tripStartTime = trip.DutyPeriods[0].DepTimeFirstLeg;
                        tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));

                    }

                    if (isFirstTrip)
                    {
                        workBlockDetails.StartTime = tripStartTime;

                    }
                    else if (!isFirstTrip && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        workBlockDetails.EndTime = tripPreviousLandTime;


                        lstWorkBlockDetails.Add(workBlockDetails);

                        workBlockDetails = new WorkBlockDetails();
                        workBlockDetails.StartTime = tripStartTime;
                    }


                    tripPreviousLandTime = tripEndTime;
                    tripPreviousEndDate = tripEndDate;
                    isFirstTrip = false;

                }

                workBlockDetails.EndTime = tripPreviousLandTime;


                lstWorkBlockDetails.Add(workBlockDetails);
                int avglatestDomArrivalTime = (int)Math.Round(Convert.ToDouble(lstWorkBlockDetails.Sum(x => x.EndTime) / lstWorkBlockDetails.Count), 2);
                int hours = avglatestDomArrivalTime / 60;
                int minutes = avglatestDomArrivalTime % 60;
                int actualhours = hours % 24;
                actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
                line.AvgLatestDomArrivalTime = new TimeSpan(actualhours, minutes, 0);

                int avgEarliestDomPush = (int)Math.Round(Convert.ToDouble(lstWorkBlockDetails.Sum(x => x.StartTime) / lstWorkBlockDetails.Count), 2);
                hours = avgEarliestDomPush / 60;
                minutes = avgEarliestDomPush % 60;
                actualhours = hours % 24;
                actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
                line.AvgEarliestDomPush = new TimeSpan(actualhours, minutes, 0);
            }
            catch (Exception)
            {

                // throw;
            }

        }

        private void CalculateWorkBlockDrop(Line line)
        {
            try
            {


                if (line.BlankLine) return;
                List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
                WorkBlockDetails workBlockDetails = new WorkBlockDetails();
                Trip trip = null;
                DateTime tripStartDate, tripPreviousEndDate, tripEndDate;
                int tripPreviousLandTime = 0, tripStartTime = 0, tripEndTime = 0, tripBriefTime = 0;
                int backToBack = 0;
                bool isFirstTrip = true;
                tripPreviousEndDate = tripEndDate = DateTime.Now;
                tripPreviousLandTime = tripStartTime = 0;
                int tripPreviousStartTime = 0;
                int nightinMid = 0;
                //int dpIndex = 0;
                //int fltIndex = 0;

                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    //  tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2))); ;

                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }

                    else
                    {
                        if (trip.ReserveTrip)
                        {
                            tripStartTime = trip.DutyPeriods[0].ReserveOut - GlobalSettings.ReserveBriefTime;
                            tripBriefTime = GlobalSettings.ReserveBriefTime;
                            //tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveOut - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.ReserveBriefTime;
                            tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveIn - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.ReserveDebrief;
                        }
                        else
                        {
                            tripStartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime;
                            tripBriefTime = trip.BriefTime;
                            //tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[0].DepTime - trip.BriefTime - (1440 * (trip.DutyPeriods.Count - 1));
                            tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                        }


                        tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    }




                    if (isFirstTrip)
                    {
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.BriefTime = tripBriefTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                        //Modified as part of BA Filter
                        workBlockDetails.StartDateTime = tripStartDate.Date.AddMinutes(workBlockDetails.StartTime);
                    }
                    else if (!isFirstTrip && vacTrip == null && tripPreviousEndDate.AddDays(1) == tripStartDate)
                    {
                        nightinMid++;
                    }
                    else if (!isFirstTrip && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        workBlockDetails.EndTime = tripPreviousLandTime;
                        workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                        workBlockDetails.EndDate = tripPreviousEndDate.Date;
                        //Modified as part of BA Filter
                        if (tripPreviousStartTime > tripPreviousLandTime)
                        {
                            // if the trip ends on the next day, we have to check the full days to check the trip is commutable.
                            workBlockDetails.EndDateTime = tripPreviousEndDate.AddMinutes(1439);
                        }
                        else
                        {
                            workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
                        }

                        workBlockDetails.BackToBackCount = backToBack - 1;
                        workBlockDetails.nightINDomicile = nightinMid;
                        nightinMid = 0;
                        lstWorkBlockDetails.Add(workBlockDetails);

                        workBlockDetails = new WorkBlockDetails();
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.BriefTime = tripBriefTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                        //Modified as part of BA Filter
                        workBlockDetails.StartDateTime = tripStartDate.Date.AddMinutes(workBlockDetails.StartTime);
                        backToBack = 0;

                    }
                    if (vacTrip == null)
                    {
                        int dpcount = 1;
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            if (trip.DutyPeriods.Count != dpcount && dp.Flights[dp.Flights.Count - 1].ArrSta == GlobalSettings.CurrentBidDetails.Domicile)
                                nightinMid++;
                            dpcount++;
                        }
                    }

                    tripPreviousLandTime = tripEndTime;
                    tripPreviousEndDate = tripEndDate;
                    if (trip.ReserveTrip)
                    {
                        tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveOut - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.ReserveBriefTime;
                    }
                    else
                    {
                        tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[0].DepTime - trip.BriefTime - (1440 * (trip.DutyPeriods.Count - 1));
                    }
                    isFirstTrip = false;
                    isFirstTrip = false;
                    backToBack++;
                }

                workBlockDetails.EndTime = tripPreviousLandTime;
                workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
                workBlockDetails.EndDate = tripPreviousEndDate.Date;
                workBlockDetails.BackToBackCount = backToBack - 1;
                workBlockDetails.nightINDomicile = nightinMid;
                if (workBlockDetails.StartDateTime != DateTime.MinValue)
                    lstWorkBlockDetails.Add(workBlockDetails);
                line.WorkBlockList = lstWorkBlockDetails;

            }
            catch (Exception)
            {

                //throw;
            }

        }
        private void CalculateAverageLatestDomicileArrivalAndPushVacationDrop(Line line)
        {
            try
            {


                if (line.BlankLine) return;
                List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
                WorkBlockDetails workBlockDetails = new WorkBlockDetails();
                Trip trip = null;
                DateTime tripStartDate, tripPreviousEndDate, tripEndDate;
                int tripPreviousLandTime = 0, tripStartTime = 0, tripEndTime = 0;
                int backToBack = 0;
                bool isFirstTrip = true;
                tripPreviousEndDate = tripEndDate = DateTime.Now;
                tripPreviousLandTime = tripStartTime = 0;

                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    VacationStateTrip vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }

                    else
                    {
                        tripStartTime = trip.DutyPeriods[0].DepTimeFirstLeg;
                        tripEndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                        tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    }




                    if (isFirstTrip)
                    {
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                    }
                    else if (!isFirstTrip && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        workBlockDetails.EndTime = tripPreviousLandTime;
                        workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                        workBlockDetails.BackToBackCount = backToBack - 1;
                        lstWorkBlockDetails.Add(workBlockDetails);

                        workBlockDetails = new WorkBlockDetails();
                        workBlockDetails.StartTime = tripStartTime;
                        workBlockDetails.StartDay = (int)tripStartDate.DayOfWeek;
                        backToBack = 0;

                    }


                    tripPreviousLandTime = tripEndTime;
                    tripPreviousEndDate = tripEndDate;
                    isFirstTrip = false;
                    backToBack++;
                }

                workBlockDetails.EndTime = tripPreviousLandTime;
                workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
                workBlockDetails.BackToBackCount = backToBack - 1;
                lstWorkBlockDetails.Add(workBlockDetails);


                int avglatestDomArrivalTime = (int)Math.Round(Convert.ToDouble(lstWorkBlockDetails.Sum(x => x.EndTime) / lstWorkBlockDetails.Count), 2);
                int hours = avglatestDomArrivalTime / 60;
                int minutes = avglatestDomArrivalTime % 60;
                int actualhours = hours % 24;
                actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
                line.AvgLatestDomArrivalTime = new TimeSpan(actualhours, minutes, 0);

                int avgEarliestDomPush = (int)Math.Round(Convert.ToDouble(lstWorkBlockDetails.Sum(x => x.StartTime) / lstWorkBlockDetails.Count), 2);
                hours = avgEarliestDomPush / 60;
                minutes = avgEarliestDomPush % 60;
                actualhours = hours % 24;
                actualhours = (actualhours < 3) ? actualhours + 24 : actualhours;
                line.AvgEarliestDomPush = new TimeSpan(actualhours, minutes, 0);


            }
            catch (Exception)
            {

                //throw;
            }


        }
        #endregion


        #region FlyPay


        private decimal CalcFlyPayEOM(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        //if the trip type is VA due to the absence of the vacation, we dont need to consider the value. This is considering when a user applying adjacent vacation and EOM 
                        string vactrype = CheckTripType(tripStartDate, tripEndDate);
                        if (vactrype != "VA")
                        {
                            VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                            DateTime date = tripStartDate;
                            if (vacationTrip != null)
                            {
                                result += vacationTrip.VacationGrandTotal.VO_TfpInBpTot;
                            }
                        }




                    }



                }

            }
            return result;
        }

        private string CheckTripType(DateTime tripStartDate, DateTime tripEndDate)
        {
            string type = string.Empty;
            if (GlobalSettings.TempOrderedVacationDays.Any(x => x.StartAbsenceDate <= tripStartDate && x.EndAbsenceDate >= tripEndDate))
            {
                type = "VA";
            }
            return type;
        }
        private void CalcFlyPayEOMDrop(Line line)
        {

            if (line.LineNum == 5)
            {
            }
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        string vactrype = CheckTripType(tripStartDate, tripEndDate);
                        if (vactrype != "VA")
                        {
                            VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                            DateTime date = tripStartDate;
                            if (vacationTrip != null)
                            {
                                line.FlyPay -= vacationTrip.VacationGrandTotal.VD_TfpInBpTot;
                            }
                        }



                    }



                }

            }


        }

        private decimal CalcvACPayEOM(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            result += vacationTrip.VacationGrandTotal.VO_TfpInBpTot;
                        }




                    }



                }

            }
            return result;
        }
        private decimal CalcvACPayEOMInLine(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            result += vacationTrip.VacationGrandTotal.VO_TfpInLineTot + vacationTrip.VacationGrandTotal.VA_TfpInLineTot;
                        }




                    }



                }

            }
            return result;
        }
        private decimal CalcvACPayEOMInLineForAdjacentEOM(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            result += vacationTrip.VacationGrandTotal.VA_TfpInLineTot;
                        }
                        //if adajacent EOM and vacation, we have to remove the VO data on the inbp bid period(VOB trip)
                        vacationTrip = GlobalSettings.VacationData[pairing].VobData;
                        if (vacationTrip != null)
                            result -= vacationTrip.VacationGrandTotal.VO_TfpInLineTot;

                    }
                }
            }
            return result;
        }
        private decimal CalcEOMPayoutsideBidperiod(Line line)
        {

            decimal carryoutVacationPay = 0;
            var outsidebidperiodBidlineTemplates = line.BidLineTemplates.Where(x => x.Date > GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
            decimal prevvalue = 0;
            int prevtype = 0;
            //int count=0;
            foreach (var item in outsidebidperiodBidlineTemplates)
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    if (prevtype != (int)item.BidLineType && prevvalue != item.Value)
                    {
                        if (item.BidLineType == (int)BidLineType.VA || item.BidLineType == (int)BidLineType.VAP)
                        {
                            carryoutVacationPay += Convert.ToDecimal(item.Value);
                        }
                    }
                }
                else
                {
                    //if (prevtype != (int)item.BidLineType && prevvalue != item.Value && count != 0)
                    if (prevtype != (int)item.BidLineType && prevvalue != item.Value)
                    {
                        if (item.BidLineType == (int)BidLineType.VA || item.BidLineType == (int)BidLineType.VAP)
                        {
                            carryoutVacationPay += Convert.ToDecimal(item.Value);
                        }
                    }
                }
                prevvalue = item.Value;
                prevtype = item.BidLineType;
            }
            return carryoutVacationPay;
        }

        private decimal CalcvACDropTfpEOM(Line line)
        {

            decimal result = 0.0m;
            if (_eomDate != DateTime.MinValue)
            {
                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;


                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                    if (tripEndDate >= _eomDate)
                    {
                        VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;

                        DateTime date = tripStartDate;
                        if (vacationTrip != null)
                        {
                            result += vacationTrip.VacationGrandTotal.VD_TfpInBpTot;
                        }




                    }



                }

            }
            return result;
        }
        #endregion





        //private int CalcLargestBlkDaysOffEOMDrop(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0, tripOff = 0, paringCount = 0;
        //    bool isLastTrip = false;
        //    DateTime tripDate = DateTime.MinValue;
        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    VacationTrip vacationTrip = null;

        //    foreach (var pairing in line.Pairings)
        //    {
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        if (trip == null)
        //        {
        //            continue;
        //        }
        //        vacationTrip = null;

        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

        //        if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //        {

        //            if (_vacationData.Keys.Contains(pairing))
        //            {
        //                if (_vacationData.FirstOrDefault(x => x.Key == pairing).Value != null)
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                }
        //                if (vacationTrip != null)
        //                {
        //                    tripDate = _bpEndDate.AddDays(1);
        //                }
        //            }
        //        }

        //        tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;
        //        }

        //        oldTripdate = tripDate.AddDays(trip.PairLength - 1);
        //    }

        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;
        //        }

        //    }

        //    return largestDaysOff;

        //}


        //private int CalcLargestBlkDaysOffVacationEOM(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0;
        //    int tripOff = 0;

        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    DateTime tripDate = DateTime.MinValue;
        //    bool isLastTrip = false; int paringCount = 0;
        //    VacationTrip vacationTrip = null;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        vacationTrip = null;
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


        //        VacationStateTrip vacTrip = null;
        //        if (line.VacationStateLine.VacationTrips != null)
        //        {
        //            vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
        //        }
        //        // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


        //        if (vacTrip != null)
        //        {
        //            if (vacTrip.TripVacationStartDate == DateTime.MinValue)
        //            {
        //                continue;
        //            }
        //            if (vacTrip.TripType == "VOB")
        //            {
        //                tripDate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VA" || x.DutyPeriodType == "VO").Count());
        //            }


        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(vacTrip.VacationDutyPeriods.Where(x => x.DutyPeriodType == "VD" || x.DutyPeriodType == "Split").Count() - 1);

        //        }

        //        else
        //        {
        //            //Checking the trip is EOM
        //            if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //            {

        //                if (_vacationData.Keys.Contains(pairing))
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                    if (vacationTrip != null)
        //                    {
        //                        tripDate = _bpEndDate.AddDays(1);
        //                    }
        //                }
        //            }



        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(trip.PairLength - 1);

        //        }


        //    }
        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;

        //        }

        //    }

        //    return largestDaysOff;
        //}

        //private int CalcLargestBlkDaysOffVacationEOMDrop(Line line)
        //{
        //    Trip trip = null;
        //    int largestDaysOff = 0;
        //    int tripOff = 0;

        //    DateTime oldTripdate = _bpStartDate.AddDays(-1);
        //    DateTime tripDate = DateTime.MinValue;
        //    bool isLastTrip = false; int paringCount = 0;
        //    VacationTrip vacationTrip = null;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        vacationTrip = null;
        //        //Get trip
        //        trip = GetTrip(pairing);
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


        //        VacationStateTrip vacTrip = null;
        //        if (line.VacationStateLine.VacationTrips != null)
        //        {
        //            vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
        //        }
        //        // tripDate = new DateTime(int.Parse(_year), int.Parse(_month), Convert.ToInt16(pairing.Substring(4, 2)));
        //        tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);


        //        if (vacTrip != null)
        //        {
        //            if (vacTrip.TripVacationStartDate == DateTime.MinValue)
        //            {
        //                continue;
        //            }

        //        }

        //        else
        //        {
        //            //Checking the trip is EOM
        //            if (tripDate.AddDays(trip.PairLength - 1) >= _nextBidPeriodVacationStartDate)
        //            {

        //                if (_vacationData.Keys.Contains(pairing))
        //                {
        //                    vacationTrip = _vacationData.FirstOrDefault(x => x.Key == pairing).Value.VofData;
        //                    if (vacationTrip != null)
        //                    {
        //                        tripDate = _bpEndDate.AddDays(1);
        //                    }
        //                }
        //            }



        //            if (tripDate > oldTripdate)
        //            {
        //                tripOff = (tripDate.Subtract(oldTripdate).Days - 1 < 0) ? 0 : tripDate.Subtract(oldTripdate).Days - 1;
        //                if (tripOff > largestDaysOff)
        //                {
        //                    largestDaysOff = tripOff;

        //                }

        //            }

        //            oldTripdate = tripDate.AddDays(trip.PairLength - 1);

        //        }


        //    }
        //    if (oldTripdate < _bpEndDate)
        //    {
        //        tripOff = (_bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1 < 0) ? 0 : _bpEndDate.AddDays(1).Subtract(oldTripdate).Days - 1;
        //        if (tripOff > largestDaysOff)
        //        {
        //            largestDaysOff = tripOff;

        //        }

        //    }

        //    return largestDaysOff;
        //}

        /// <summary>
        /// Get Trip using trip name.
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="pairing"></param>
        private Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
            if (trip == null)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
            }

            return trip;

        }

        public int CalcAmPmSortOrder(Line line)
        {



            if (line.AMPM == "AM")
                return 1;
            else if (line.AMPM == "Mix")
                return 2;
            else if (line.AMPM == "---")
                return 3;
            else if (line.AMPM == " PM")
                return 4;
            else return 5;
        }

        //private void GetNextBidPeriodVacationStartDate()
        //{

        //    if (GlobalSettings.CurrentBidDetails.Postion == "FA")
        //    {
        //        _nextBidPeriodVacationStartDate = _eOMStartdate;
        //    }
        //    else
        //    {
        //        int daysToADD = 0;
        //        int dayOfWeek = (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.DayOfWeek;
        //        daysToADD = 7 - dayOfWeek;
        //        _nextBidPeriodVacationStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(daysToADD);
        //    }

        //}
        #endregion

        #region Private Methods

        #region Calculate BidlineTemplate

        private void SetMILAffetedTrips(Line line)
        {


            int paringCount = 0;


            foreach (string pairing in line.Pairings)
            {

                if (_mILData.ContainsKey(pairing))
                {
                    bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                    paringCount++;
                    DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //string splitName = string.Empty;
                    //if (GlobalSettings.CurrentBidDetails.Round == "M")
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //}
                    //else
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                    //}
                    Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4));
                    if (trip == null)
                        GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
                    if (trip == null) return;
                    DateTime tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);
                    //TimeSpan time = new TimeSpan(23, 0, 0);
                    //tripEndDate = tripEndDate.Date + time;

                    int time = (trip.ReserveTrip) ? trip.DutyPeriods[trip.PairLength - 1].ReserveIn : trip.DutyPeriods[trip.PairLength - 1].ReleaseTime;
                    if (time > 1440)
                    {
                        time = time - (trip.PairLength - 1) * 1440;
                    }
                    tripEndDate = tripEndDate.AddMinutes(time);

                    //trip start time
                    time = (trip.ReserveTrip) ? trip.DutyPeriods[0].ReserveOut : trip.DutyPeriods[0].ShowTime;
                    tripStartDate = tripStartDate.AddMinutes(time);


                    MILTrip milTrip = null;

                    if (GlobalSettings.MILDates.Any(x => (x.StartAbsenceDate <= tripStartDate && x.EndAbsenceDate >= tripEndDate)))
                    {
                        _mILTripsOfSingleLine.Add(pairing, "MA");
                    }

                    // 2.Check if trip starts before the MIL period and finishes inside the MIL period. (MOF Vacation)
                    //-------------------------------------------------------------------------------------------------
                    else if (GlobalSettings.MILDates.Any(x => ((x.StartAbsenceDate > tripStartDate) && (x.StartAbsenceDate < tripEndDate) && x.EndAbsenceDate >= tripEndDate)))
                    {
                        _mILTripsOfSingleLine.Add(pairing, "MOF");
                    }
                    // 3.check if trip starts inside the vacation period and finished outside the vacation period.. (MOB Vacation)
                    //-------------------------------------------------------------------------------------------------
                    else if (GlobalSettings.MILDates.Any(x => (x.EndAbsenceDate > tripStartDate && x.EndAbsenceDate < tripEndDate && tripStartDate >= x.StartAbsenceDate)))
                    {
                        _mILTripsOfSingleLine.Add(pairing, "MOB");
                    }
                    else if (GlobalSettings.MILDates.Any(x => (x.StartAbsenceDate > tripStartDate && x.EndAbsenceDate < tripEndDate)))
                    {

                        _mILTripsOfSingleLine.Add(pairing, "MOFB");
                    }



                }

            }
        }
        private List<BidLineTemplate> GenerateBidLineViewTemplate(Line line)
        {

            try
            {



                Trip trip = null;

                DateTime wkDay;
                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {

                    //trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                    trip = GetTrip(pairing);
                    int lengthTrip = trip.PairLength;
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    // wkDay = new DateTime(int.Parse(year), int.Parse(month), int.Parse(pairing.Substring(4, 2)));
                    wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip, GlobalSettings.CurrentBidDetails);

                    for (int tripDay = 0; tripDay < lengthTrip; tripDay++)
                    {
                        BidLineTemplate bidLineTemplate = line.BidLineTemplates.Where(x => x.Date.Day == wkDay.AddDays(tripDay).Day
                                                                                      && x.Date.Month == wkDay.AddDays(tripDay).Month).FirstOrDefault();

                        bidLineTemplate.AMPMType = trip.AmPm;
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return line.BidLineTemplates;
        }
        private List<BidLineTemplate> GenerateBidLineTemplateCollection()
        {
            List<BidLineTemplate> lstBidLineTemplate = new List<BidLineTemplate>();
            try
            {
                DateTime start = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
                DateTime end = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;

                //Generate a DateTime list collection within the range BidPeriodStartDate and BidPeriodEndDate
                List<DateTime> lstDateTime = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                       .Select(day => start.AddDays(day))
                                                       .ToList();
                //Add 5 more days to the collection
                lstDateTime.AddRange(Enumerable.Range(0, 5)
                                               .Select(x => end.AddDays(1).AddDays(x))
                                               .ToList());


                foreach (var day in lstDateTime)
                {
                    char dayType;
                    //For testing we will remove the code once we parse the senioritylist file
                    string dayString = day.DayOfWeek.ToString().Substring(0, 2).ToLower();

                    bool isInCurrentMonth = (day > end) ? false : true;

                    if (dayString == "sa" || dayString == "su")
                    {
                        dayType = 'W';
                    }
                    else
                    { dayType = 'N'; }
                    //  lstBidLineTemplate.Add(new BidLineTemplate { DayOfWeek = day.DayOfWeek.ToString().Substring(0, 2).ToUpper(), Day = day.Day, Month = day.Month, Year = day.Year, DayType = dayType, IsInCurrentMonth = isInCurrentMonth });
                    lstBidLineTemplate.Add(new BidLineTemplate { Date = new DateTime(day.Year, day.Month, day.Day), DayType = dayType, IsInCurrentMonth = isInCurrentMonth });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lstBidLineTemplate;
        }
        private void GenerateMILBidLineTemplate(Line line)
        {
            SetBidLineTemplateToNormal(line.BidLineTemplates, line.ReserveLine);
            int paringCount = 0;
            List<DateTime> milDpDates = new List<DateTime>();

            foreach (string pairing in line.Pairings)
            {

                try
                {
                    //Checking if the trip is MIL effected trip
                    if (_mILTripsOfSingleLine.ContainsKey(pairing))
                    {

                        MILTrip milTrip = GetMILTrip(pairing);
                        //if (milTrip.DutyPeriodsDetails == null) continue;

                        bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
                        paringCount++;
                        DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        //string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                        //string splitName = string.Empty;
                        //if (GlobalSettings.CurrentBidDetails.Round == "M")
                        //{
                        //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                        //}
                        //else
                        //{
                        //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                        //}
                        Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4));
                        if (trip == null)
                            GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
                        if (trip == null) return;
                        DateTime tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);


                        //  MA Trip
                        //-----------------------------------------------------------------------------------------------------------
                        if (milTrip.MILType == "MA")
                        {
                            DateTime date = tripStartDate;
                            for (int day = 0; day < trip.PairLength; day++)
                            {
                                var blTemplate = line.BidLineTemplates.FirstOrDefault(x => date.Date == x.Date);
                                if (blTemplate != null)
                                {
                                    blTemplate.TemplateName = "VO";
                                    blTemplate.workStatus = "VO_No_Work";
                                    //blTemplate.ArrStaLastLeg = string.Empty;
                                    blTemplate.BorderType = (blTemplate.DutyPeriodNo == milTrip.DutyPeriodsDetails.Count - 1) ? 2 : 0;
                                    //  int mid = (trip.PairLength % 2 == 0) ? trip.PairLength - 1 : trip.PairLength;
                                    //  blTemplate.TripNumDisplay = (day == mid / 2) ? "MIL" : string.Empty;
                                    blTemplate.TripNumDisplay = string.Empty;
                                    blTemplate.BidLineType = (int)BidLineType.VA;
                                    blTemplate.ArrStaLastLegDisplay = string.Empty;
                                    //blTemplate.TripBackColor = GlobalSettings.MIlBackColr;
                                    //blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.Vacation.ToString();
                                }

                                milDpDates.Add(date);
                                date = date.AddDays(1);

                            }
                        }
                        //  MOF Trip
                        //-----------------------------------------------------------------------------------------------------------
                        else if (milTrip.MILType == "MOF")
                        {
                            DateTime date = tripStartDate;
                            int index = 0;
                            foreach (MILDutyPeriod milDP in milTrip.DutyPeriodsDetails)
                            {
                                if (milDP.Type == "MA" || milDP.Type == "MD" || milDP.Type == "Split")
                                {
                                    int cnt = milTrip.DutyPeriodsDetails.Count(x => x.Type == "MA" || x.Type == "MD" || x.Type == "Split");
                                    var blTemplate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Date == date.Date);
                                    if (blTemplate != null)
                                    {

                                        if (milDP.Type == "Split")
                                        {
                                            //blTemplate.workStatus = "Work";
                                            blTemplate.workStatus = "FrontSplitWork";
                                            blTemplate.TemplateName = "VDVOF";
                                            //blTemplate.ArrStaLastLeg = string.Empty;

                                            blTemplate.BidLineType = (int)BidLineType.VOFSplit;
                                            //blTemplate.TripBackColor1 = GlobalSettings.MIlBackColr; 
                                        }
                                        else
                                        {
                                            blTemplate.BidLineType = (int)BidLineType.VA;
                                            blTemplate.TemplateName = "VO";
                                            //blTemplate.ArrStaLastLeg = string.Empty;
                                            //blTemplate.TripBackColor = GlobalSettings.MIlBackColr; 
                                            blTemplate.workStatus = "VO_No_Work";

                                        }


                                        blTemplate.BorderType = (blTemplate.DutyPeriodNo == milTrip.DutyPeriodsDetails.Count - 1) ? 2 : 0;
                                        // int mid = (cnt % 2 == 0) ? cnt - 1 : cnt;
                                        //  blTemplate.TripNumDisplay = (index == mid / 2) ? "MIL" : string.Empty;
                                        blTemplate.TripNumDisplay = string.Empty;
                                        blTemplate.ArrStaLastLegDisplay = string.Empty;
                                        //blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.Vacation.ToString();
                                        milDpDates.Add(date);
                                    }

                                }
                                date = date.AddDays(1);
                            }
                        }
                        //  MOB Trip
                        //-----------------------------------------------------------------------------------------------------------
                        else if (milTrip.MILType == "MOB")
                        {
                            DateTime date = tripStartDate;
                            int index = 0;
                            foreach (MILDutyPeriod milDP in milTrip.DutyPeriodsDetails)
                            {
                                if (milDP.Type == "MA" || milDP.Type == "MD" || milDP.Type == "Split")
                                {
                                    int cnt = milTrip.DutyPeriodsDetails.Count(x => x.Type == "MA" || x.Type == "MD" || x.Type == "Split");
                                    var blTemplate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Date == date.Date);
                                    if (blTemplate != null)
                                    {


                                        if (milDP.Type == "Split")
                                        {
                                            blTemplate.TemplateName = "VOVDB";
                                            //blTemplate.ArrStaLastLeg = string.Empty;
                                            //blTemplate.TripBackColor1 = GlobalSettings.MIlBackColr;
                                            // blTemplate.workStatus = "Work";
                                            blTemplate.workStatus = "BackSplitWork";
                                            blTemplate.BidLineType = (int)BidLineType.VOBSplit;
                                        }
                                        else
                                        {
                                            blTemplate.BidLineType = (int)BidLineType.VA;
                                            //blTemplate.ArrStaLastLeg = string.Empty;

                                            blTemplate.TemplateName = "VO";
                                            //blTemplate.TripBackColor = GlobalSettings.MIlBackColr;
                                            blTemplate.workStatus = "VO_No_Work";

                                        }

                                        blTemplate.BorderType = (blTemplate.DutyPeriodNo == milTrip.DutyPeriodsDetails.Count - 1) ? 2 : 0;
                                        //  int mid = (cnt % 2 == 0) ? cnt - 1 : cnt;
                                        // blTemplate.TripNumDisplay = (index == mid / 2) ? "MIL" : string.Empty;
                                        blTemplate.TripNumDisplay = string.Empty;
                                        blTemplate.ArrStaLastLegDisplay = string.Empty;
                                        //blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.Vacation.ToString();
                                        milDpDates.Add(date);
                                    }
                                }
                                date = date.AddDays(1);
                            }
                        }
                        else if (milTrip.MILType == "MOFB")
                        {
                            DateTime date = tripStartDate;
                            string templatename = "VDVOF";
                            string wokstatus = "FrontSplitWork";
                            foreach (MILDutyPeriod milDP in milTrip.DutyPeriodsDetails)
                            {
                                if (milDP.Type == "MA" || milDP.Type == "MD" || milDP.Type == "Split")
                                {
                                    int cnt = milTrip.DutyPeriodsDetails.Count(x => x.Type == "MA" || x.Type == "MD" || x.Type == "Split");
                                    var blTemplate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Date == date.Date);
                                    if (blTemplate != null)
                                    {
                                        if (milDP.Type == "Split")
                                        {

                                            if (templatename != "VOVDB" && GlobalSettings.MILDates.Any(x => x.EndAbsenceDate.Date == blTemplate.Date))
                                            {
                                                wokstatus = "BackSplitWork";
                                                templatename = "VOVDB";
                                            }


                                            blTemplate.TemplateName = templatename;
                                            //blTemplate.TripBackColor1 = GlobalSettings.MIlBackColr;
                                            blTemplate.workStatus = wokstatus;
                                            if (templatename == "VDVOF")
                                            {
                                                blTemplate.BidLineType = (int)BidLineType.VOFSplit;
                                            }
                                            else
                                            {
                                                blTemplate.BidLineType = (int)BidLineType.VOBSplit;
                                            }
                                        }
                                        else
                                        {
                                            wokstatus = "BackSplitWork";
                                            templatename = "VOVDB";
                                            blTemplate.TemplateName = "VO";
                                            //blTemplate.TripBackColor = GlobalSettings.MIlBackColr;
                                            blTemplate.workStatus = "VO_No_Work";
                                            blTemplate.BidLineType = (int)BidLineType.VA;

                                        }

                                        blTemplate.BorderType = (blTemplate.DutyPeriodNo == milTrip.DutyPeriodsDetails.Count - 1) ? 2 : 0;
                                        blTemplate.TripNumDisplay = string.Empty;
                                        blTemplate.ArrStaLastLegDisplay = string.Empty;
                                        //blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.Vacation.ToString();
                                        milDpDates.Add(date);
                                    }
                                }
                                date = date.AddDays(1);
                            }
                        }

                        //-----------------------------------------------------------------------------------------------------------
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            List<MILVacPeriod> lstMILVac = new List<MILVacPeriod>();
            foreach (var abs in GlobalSettings.MILDates)
            {

                //set the back color to orange
                //-----------------------------------------------------------------------------------------------------------

                DateTime date = abs.StartAbsenceDate.Date;
                while (date <= abs.EndAbsenceDate.Date)
                {
                    var blTemplate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Date == date.Date);
                    if (blTemplate != null)
                    {
                        //not having any trip
                        if (blTemplate.TripName == null)
                        {
                            blTemplate.BidLineType = (int)BidLineType.VA;
                            blTemplate.TemplateName = "VO";
                            blTemplate.TripBackColor = "#ffa500";
                            blTemplate.ArrStaLastLegDisplay = string.Empty;
                        }
                        //if the MIL trip starts after the tripends on the same day. clearly it will not effect any current trip,but we need to indicate the MIL start day in the calender view and modern bid line view.
                        else if (blTemplate.TemplateName == "Default" && abs.StartAbsenceDate.Date == date)
                        {
                            blTemplate.BidLineType = (int)BidLineType.VOFSplit;
                            blTemplate.TemplateName = "VDVOF";
                            blTemplate.workStatus = "FrontSplitWithoutStrike";
                            blTemplate.ArrStaLastLegDisplay = string.Empty;
                            //blTemplate.TripBackColor1 = GlobalSettings.MIlBackColr; 
                        }
                        else if (blTemplate.TemplateName == "Default" && abs.EndAbsenceDate.Date == date)
                        {
                            blTemplate.BidLineType = (int)BidLineType.VOBSplit;
                            blTemplate.TemplateName = "VOVDB";
                            blTemplate.workStatus = "BackSplitWithoutStrike";
                            blTemplate.ArrStaLastLegDisplay = string.Empty;
                            //blTemplate.TripBackColor1 = GlobalSettings.MIlBackColr; 
                        }
                        blTemplate.BorderType = 1;
                    }


                    date = date.AddDays(1);
                }

                //-----------------------------------------------------------------------------------------------------------

                MILVacPeriod mILVacPeriod = new MILVacPeriod { StartDate = abs.StartAbsenceDate.Date, EndDate = abs.EndAbsenceDate.Date };
                date = mILVacPeriod.StartDate;
                bool status = true;
                //checking back word if any duty period droped before MIL
                //-----------------------------------------------------------------------------------------------------------
                while (status)
                {
                    date = date.AddDays(-1);
                    var sasi = milDpDates.FirstOrDefault(x => x == date);
                    if (milDpDates.Count() != 0 && milDpDates.FirstOrDefault(x => x == date) != DateTime.MinValue)
                    {
                        mILVacPeriod.StartDate = date;
                    }
                    else
                    {
                        status = false;
                    }

                }


                status = true;
                date = mILVacPeriod.EndDate;



                //checking forword if any duty period droped before MIL
                //-----------------------------------------------------------------------------------------------------------
                while (status)
                {
                    date = date.AddDays(1);
                    if (milDpDates.Count() != 0 && milDpDates.FirstOrDefault(x => x == date) != DateTime.MinValue)
                    {
                        mILVacPeriod.EndDate = date;
                    }
                    else
                    {
                        status = false;
                    }

                }


                var item = lstMILVac.FirstOrDefault(x => x.EndDate > mILVacPeriod.StartDate);
                if (item != null)
                {
                    item.EndDate = mILVacPeriod.EndDate;
                }
                else
                {
                    lstMILVac.Add(mILVacPeriod);
                }



                foreach (var itemDate in lstMILVac)
                {

                    int totDays = (int)(itemDate.EndDate - itemDate.StartDate).TotalDays;
                    date = itemDate.StartDate.AddDays(totDays / 2);
                    var blTemplate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Date == date.Date.Date);
                    if (blTemplate != null)
                    {
                        blTemplate.ArrStaLastLegDisplay = "MIL";
                    }

                }

            }

        }



        private void SetBidLineTemplateToNormal(List<BidLineTemplate> bidLineTemplates, bool isReserve)
        {
            foreach (BidLineTemplate blTemplate in bidLineTemplates)
            {
                blTemplate.BorderType = 2;
                blTemplate.TemplateName = "Default";
                blTemplate.ArrStaLastLegDisplay = blTemplate.ArrStaLastLeg;
                blTemplate.TripNumDisplay = blTemplate.TripNum;
                blTemplate.TripForeColor = "#000000";

                if (!string.IsNullOrEmpty(blTemplate.ArrStaLastLeg))
                {
                    blTemplate.workStatus = "Work";
                }
                else
                    blTemplate.workStatus = "No Work";
                if (blTemplate.DayType == 'N')
                {
                    if (string.IsNullOrEmpty(blTemplate.ArrStaLastLeg))
                    {
                        blTemplate.TripBackColor = (blTemplate.IsInCurrentMonth) ? "#FFFFFAF0" : "#FFD69690";
                    }
                }
                //Week ends
                //-------------------------------------------------------
                else if (blTemplate.DayType == 'W')
                {
                    if (string.IsNullOrEmpty(blTemplate.ArrStaLastLeg))
                    {
                        blTemplate.TripBackColor = "#FFFFEAD4";
                    }
                }
                if (!string.IsNullOrEmpty(blTemplate.ArrStaLastLeg))
                {
                    //SetTripColor(blTemplate, isReserve);
                }
            }
        }

        //      private void SetTripColor(BidLineTemplate blTemplate, bool isReserve)
        //      {
        //          // blTemplate.TemplateName = "NoWork";
        //          //Not a Reserve line
        //          if (GlobalSettings.CurrentTheme != null && GlobalSettings.CurrentTheme.CurrentBackColor != null)
        //          {
        //              if (!isReserve)
        //              {
        //                  if (blTemplate.AMPMType == "1")
        //                  {
        //                      blTemplate.TripBackColor = GlobalSettings.CurrentTheme.CurrentBackColor.AMTrips.ToString();
        //                      blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.AMTrips.ToString();
        //                  }
        //                  else if (blTemplate.AMPMType == "2")
        //                  {
        //                      blTemplate.TripBackColor = GlobalSettings.CurrentTheme.CurrentBackColor.PMTrips.ToString();
        //                      blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.PMTrips.ToString();
        //                  }
        //
        //                  else if (blTemplate.AMPMType == "3")
        //                  {
        //                      blTemplate.TripBackColor = GlobalSettings.CurrentTheme.CurrentBackColor.MixedTrips.ToString();
        //                      blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.MixedTrips.ToString();
        //                  }
        //
        //                  else if (blTemplate.AMPMType == null)
        //                  {
        //                      blTemplate.TripBackColor = "#FFD2B48C";
        //
        //                  }
        //              }
        //              //Reserve line
        //              else
        //              {
        //                  if (blTemplate.AMPMType == "1")
        //                  {
        //                      blTemplate.TripBackColor = GlobalSettings.CurrentTheme.CurrentBackColor.AMReserve.ToString();
        //                      blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.AMReserve.ToString();
        //                  }
        //                  else if (blTemplate.AMPMType == "2")
        //                  {
        //                      blTemplate.TripBackColor = GlobalSettings.CurrentTheme.CurrentBackColor.PMReserve.ToString();
        //                      blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.PMReserve.ToString();
        //                  }
        //
        //
        //                  else if (blTemplate.AMPMType == "3")
        //                  {
        //                      blTemplate.TripBackColor = GlobalSettings.CurrentTheme.CurrentBackColor.MixedTrips.ToString();
        //                      blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.MixedTrips.ToString();
        //                  }
        //
        //                  else if (blTemplate.AMPMType == null)
        //                  {
        //                      blTemplate.TripBackColor = "#FFD2B48C";
        //
        //                  }
        //              }
        //          }
        //      }

        #endregion

        private MILTrip GetMILTrip(string pairing)
        {
            MILTrip milTrip = null;

            switch (_mILTripsOfSingleLine[pairing])
            {
                case "MA":
                    milTrip = _mILData[pairing].MaData;
                    break;

                case "MOF":
                    milTrip = _mILData[pairing].MofData;
                    break;

                case "MOB":
                    milTrip = _mILData[pairing].MobData;
                    break;

                case "MOFB":
                    milTrip = _mILData[pairing].MofbData;
                    break;




            }

            return milTrip;

        }
        #endregion
        private int CalculateMILDaysOff(Line line)
        {


            int daysOff = line.TempDaysOff;

            foreach (string pairing in line.Pairings)
            {
                //Checking if the trip is MIL effected trip
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {

                    MILTrip milTrip = GetMILTrip(pairing);

                    foreach (var item in milTrip.DutyPeriodsDetails)
                    {
                        if (item.Type != "Split" && item.Type != "MO" && item.IsDropped && item.IsinBidPeriod)
                        {
                            daysOff++;
                        }
                    }


                }
            }



            return daysOff;
        }
        private decimal CalculateMILFlightPay(Line line)
        {


            decimal flyPay = line.TempTfp;

            foreach (string pairing in line.Pairings)
            {
                //Checking if the trip is MIL effected trip
                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {

                    MILTrip milTrip = GetMILTrip(pairing);


                    if (milTrip.DutyPeriodsDetails.Count(x => x.IsinBidPeriod) == milTrip.DutyPeriodsDetails.Count())
                    {

                        flyPay -= milTrip.Tfp;
                    }
                    else
                    {
                        foreach (var dp in milTrip.DutyPeriodsDetails)
                        {
                            if (dp.IsinBidPeriod)
                            {
                                flyPay -= dp.Tfp;
                            }

                        }
                    }

                }
            }


            flyPay = decimal.Parse(String.Format("{0:0.00}", flyPay));
            return flyPay;
        }


        private string CalculateMILBlockHours(Line line)
        {

            string blkhours = line.TempBlkHrsInBp;
            bool isLastTrip;
            Trip trip = null;
            DateTime tripStartDate;
            int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                //Checking if the trip is MIL effected trip


                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    trip = GetTrip(pairing);
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    MILTrip milTrip = GetMILTrip(pairing);

                    // if all the  dutyperiod are inside the bid period and all are dropped 
                    if ((milTrip.DutyPeriodsDetails.Count(x => x.IsinBidPeriod) == milTrip.DutyPeriodsDetails.Count()) &&
                        (milTrip.DutyPeriodsDetails.Count(x => x.Type == "MD" || x.Type == "MA") == milTrip.DutyPeriodsDetails.Count()))
                    {

                        //Trip Dropped
                        blkhours = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(blkhours) - trip.Block);

                    }
                    else
                    {
                        int index = 0;
                        foreach (var dp in milTrip.DutyPeriodsDetails)
                        {
                            if ((dp.Type == "MA" || dp.Type == "MD") && dp.IsinBidPeriod)
                            {

                                blkhours = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(blkhours) - trip.DutyPeriods[index].Block);
                            }
                            else if (dp.Type == "Split" && dp.IsinBidPeriod)
                            {
                                int flightIndex = 0;
                                foreach (var flight in milTrip.DutyPeriodsDetails[index].FlightDetails)
                                {
                                    if (flight.Type == "MD")
                                    {
                                        blkhours = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(blkhours) - trip.DutyPeriods[index].Flights[flightIndex].Block);
                                    }


                                    flightIndex++;
                                }

                            }
                            index++;
                        }
                    }



                }


            }


            //flyPay = decimal.Parse(String.Format("{0:0.00}", flyPay));
            return blkhours;
        }
        private int CalcLargestBlkDaysOffMIL(Line line)
        {
            int largestDaysOff = 0;
            int temp = 0;
            foreach (var item in line.BidLineTemplates)
            {
                if (item.workStatus == "No Work" || item.workStatus == "VO_No_Work")
                {
                    temp++;
                }
                else if (item.workStatus == "Work" || item.workStatus == "FrontSplitWork" || item.workStatus == "BackSplitWork")
                {
                    if (largestDaysOff < temp)
                    {
                        largestDaysOff = temp;
                    }
                    temp = 0;
                }
                if (!item.IsInCurrentMonth)
                {
                    if (largestDaysOff < temp)
                    {
                        largestDaysOff = temp;
                    }
                    break;
                }
            }
            return largestDaysOff;
        }

        private string CalculateMILTAFB(Line line)
        {

            string tafbInBp = line.TempTafbInBp;
            if (tafbInBp == "-104-3")
            {
            }
            bool isLastTrip;
            Trip trip = null;
            DateTime tripStartDate;
            int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                //Checking if the trip is MIL effected trip



                if (_mILTripsOfSingleLine.ContainsKey(pairing))
                {


                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    trip = GetTrip(pairing);
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    MILTrip milTrip = GetMILTrip(pairing);

                    // if all the  dutyperiod are inside the bid period and all are dropped 
                    if ((milTrip.DutyPeriodsDetails.Count(x => x.IsinBidPeriod) == milTrip.DutyPeriodsDetails.Count()) &&
                        (milTrip.DutyPeriodsDetails.Count(x => x.Type == "MD" || x.Type == "MA") == milTrip.DutyPeriodsDetails.Count()))
                    {

                        //Trip Dropped
                        tafbInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(tafbInBp) - trip.Tafb);

                    }
                    else
                    {
                        int index = 0;
                        foreach (var dp in milTrip.DutyPeriodsDetails)
                        {
                            if (dp.Type == "MA" || dp.Type == "MD" && dp.IsinBidPeriod)
                            {

                                tafbInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(tafbInBp) - CalculateTafbInBpOfDutyPeriod(trip.DutyPeriods[index], trip.PairLength));
                            }
                            else if (dp.Type == "Split" && dp.IsinBidPeriod)
                            {
                                int flightIndex = 0;



                                bool isFirstType = (milTrip.MILType == "MOF");

                                int tafb = 0; ;
                                int time = 0;
                                if (isFirstType)
                                {
                                    flightIndex = 0;
                                    foreach (var flight in milTrip.DutyPeriodsDetails[index].FlightDetails)
                                    {
                                        if (flight.Type == "MO")
                                        {

                                            time = trip.DutyPeriods[index].Flights[flightIndex].ArrTime;
                                        }


                                        flightIndex++;
                                    }
                                }

                                else
                                {
                                    flightIndex = 0;
                                    foreach (var flight in milTrip.DutyPeriodsDetails[index].FlightDetails)
                                    {
                                        if (flight.Type == "MO")
                                        {

                                            time = trip.DutyPeriods[index].Flights[flightIndex].DepTime;
                                            break;
                                        }


                                        flightIndex++;
                                    }
                                }


                                int tafbVal = CalculateTafbInBpOfSplitDutyPeriod(trip.DutyPeriods[index], trip.PairLength, isFirstType, time);

                                tafbInBp = Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(tafbInBp) - tafbVal);
                            }
                            index++;
                        }
                    }



                }



            }


            //flyPay = decimal.Parse(String.Format("{0:0.00}", flyPay));
            return tafbInBp;
        }

        private int CalculateTafbInBpOfSplitDutyPeriod(DutyPeriod dutyPeriod, int lengthOfTrip, bool isFirstType, int time)
        {
            time = time % 1440;

            int dutyPeriodTafb = 0;
            if (isFirstType)
            {

                if (dutyPeriod.DutPerSeqNum == 1)
                    // first day of trip and trip is more than 1 day
                    //  dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg % 1440 + GlobalSettings.show1stDay;
                    //  dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.show1stDay;
                    dutyPeriodTafb += time - dutyPeriod.ShowTime - (1440 * (dutyPeriod.DutPerSeqNum - 1));
                else
                    dutyPeriodTafb += time;

            }
            else
            {

                if (lengthOfTrip > 1)
                    // last day of multi day trip
                    dutyPeriodTafb += 1440 - time;
                else
                    // trip is a 1 day trip
                    //dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg % 1440 + GlobalSettings.show1stDay + GlobalSettings.debrief;
                    // dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.show1stDay + GlobalSettings.debrief;
                    dutyPeriodTafb += time - dutyPeriod.ShowTime - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.debrief;
            }


            return dutyPeriodTafb;
        }

        private int CalculateTafbInBpOfDutyPeriod(DutyPeriod dutyPeriod, int lengthOfTrip)
        {

            int dutyPeriodTafb = 0;
            if (dutyPeriod.DutPerSeqNum < lengthOfTrip)
            {
                if (dutyPeriod.DutPerSeqNum == 1)
                    // first day of trip and trip is more than 1 day
                    // dutyPeriodTafb += 24 * 60 - (dutyPeriod.DepTimeFirstLeg - GlobalSettings.show1stDay);
                    dutyPeriodTafb += 24 * 60 - (dutyPeriod.ShowTime);
                else
                    // not first or last day of trip, so there are 24 hours of Tafb
                    dutyPeriodTafb += 24 * 60;
            }
            else if (lengthOfTrip > 1)
                // last day of multi day trip
                // dutyPeriodTafb += dutyPeriod.LandTimeLastLeg % 1440 + GlobalSettings.debrief;
                dutyPeriodTafb += dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.debrief;
            else
                // trip is a 1 day trip
                //dutyPeriodTafb += dutyPeriod.LandTimeLastLeg % 1440 - (dutyPeriod.DepTimeFirstLeg % 1440 - GlobalSettings.show1stDay) + GlobalSettings.debrief;
                // dutyPeriodTafb += (dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1))) - (dutyPeriod.DepTimeFirstLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) - GlobalSettings.show1stDay) + GlobalSettings.debrief;
                dutyPeriodTafb += (dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1))) - (dutyPeriod.ShowTime - (1440 * (dutyPeriod.DutPerSeqNum - 1))) + GlobalSettings.debrief;

            //dutyPeriodTafb = dutyPeriod.DutyBreak;

            return dutyPeriodTafb;
        }

        private void CalculateHolidayRigProperties(Line line)
        {

            line.HolRig = 0;

            if (GlobalSettings.CurrentBidDetails.Month == 1 || GlobalSettings.CurrentBidDetails.Month == 11 || GlobalSettings.CurrentBidDetails.Month == 12)
            {
                Trip trip = null;
                DateTime tripStartDate;


                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    trip = GetTrip(pairing);
                    //trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                    tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                    foreach (DutyPeriod dp in trip.DutyPeriods)
                    {
                        DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);
                        ////holid day pay on jan1, dec 25 (0.5 *tfp of that day --minimum 2.5)
                        //if (dt.Month == 1 && dt.Day == 1 || dt.Month == 12 && dt.Day == 25)
                        //{
                        //    if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                        //    {
                        //        line.HolRig = dp.Tfp * 0.5m;
                        //        if (line.HolRig < 2.5m)
                        //            line.HolRig = 2.5m;
                        //    }
                        //    else
                        //    {
                        //        line.HolRig = dp.Tfp * 1m;
                        //    }

                        //    break;
                        //}
                        if (GlobalSettings.CurrentBidDetails.Month == 11)
                        {
                            DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                            while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                            {
                                fourthThursday = fourthThursday.AddDays(1);
                            }
                            if (dt.Day == fourthThursday.Day)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                {
                                    line.HolRig = dp.Tfp * 0.5m;
                                    if (line.HolRig < 2.5m)
                                        line.HolRig = 2.5m;
                                }
                                else
                                {
                                    line.HolRig = dp.Tfp * 1m;
                                    if (line.HolRig < 4m)
                                        line.HolRig = 4m;
                                }

                                break;
                            }


                        }
                        else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                        {

                            if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                            {
                                decimal pay = dp.Tfp * 1m;
                                if (pay < 4m)
                                    pay = 4m;
                                line.HolRig += pay;
                            }
                        }
                        else
                        {
                            if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                            {
                                line.HolRig = dp.Tfp * 0.5m;
                                if (line.HolRig < 2.5m)
                                    line.HolRig = 2.5m;
                            }
                        }

                    }
                }
            }
            line.HolRig = Math.Round(line.HolRig, 2);
        }
        private decimal CalcVaPayForaLine(Line line)
        {
            line.VAne = 0;
            line.VAbo = 0;
            line.VAbp = 0;
          
            decimal result = 0.0m;
            try
            {

                Trip trip = null;
                decimal tfp = 0m;
                bool isLastTrip = false; int paringCount = 0;
                if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
                {
                    foreach (var pairing in line.Pairings)
                    {
                        trip = GetTrip(pairing);
                        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                        DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);

                        if (line.VacationStateLine != null && line.VacationStateLine.VacationTrips != null)
                        {
                            var vacationtrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == pairing);
                            if (vacationtrip != null)
                            {
                                VacationTrip vacationdata = null;
                                if (vacationtrip.TripType == "VA")
                                {
                                    vacationdata = GlobalSettings.VacationData.Where(x => x.Key == pairing).Select(y => y.Value.VaData).FirstOrDefault();
                                }
                                else if (vacationtrip.TripType == "VOF")
                                {
                                    vacationdata = GlobalSettings.VacationData.Where(x => x.Key == pairing).Select(y => y.Value.VofData).FirstOrDefault();
                                }
                                else if (vacationtrip.TripType == "VOB")
                                {
                                    vacationdata = GlobalSettings.VacationData.Where(x => x.Key == pairing).Select(y => y.Value.VobData).FirstOrDefault();
                                }
                                if (vacationdata != null)
                                {
                                    line.VAbp += vacationdata.VacationGrandTotal.VA_TfpInBpTot;
                                    line.VAbo += vacationdata.VacationGrandTotal.VA_TfpInLineTot;
                                }
                            }
                        }
                        if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                        {
                            if (tripEndDate >= _eomDate)
                            {
                                VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                                DateTime date = tripStartDate;
                                if (vacationTrip != null)
                                {

                                    line.VAbp += vacationTrip.VacationGrandTotal.VA_TfpInBpTot;
                                    line.VAbo += vacationTrip.VacationGrandTotal.VA_TfpInLineTot;
                                }


                            }
                        }
                    }

                }
                else if (GlobalSettings.MenuBarButtonStatus.IsEOM)
                {

                    foreach (var pairing in line.Pairings)
                    {
                        trip = GetTrip(pairing);
                        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                        DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);

                        if (tripEndDate >= _eomDate)
                        {
                            VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                            DateTime date = tripStartDate;
                            if (vacationTrip != null)
                            {

                                line.VAbp += vacationTrip.VacationGrandTotal.VA_TfpInBpTot;
                                line.VAbo += vacationTrip.VacationGrandTotal.VA_TfpInLineTot;
                            }


                        }
                    }
                }
                line.VAne = line.VAbo - line.VAbp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        private void CalculateHolidayRigVacationProperties(Line line)
        {


            line.HolRig = 0;

            if (GlobalSettings.CurrentBidDetails.Month == 1 || GlobalSettings.CurrentBidDetails.Month == 11 || GlobalSettings.CurrentBidDetails.Month == 12)
            {


                Trip trip = null;
                VacationStateTrip vacTrip = null;
                DateTime oldTripEndDate = DateTime.MinValue;
                DateTime tripStartDate = DateTime.MinValue;
                string sdow = string.Empty;

                int dpIndex = 0;
                int paringCount = 0;


                bool isLastTrip = false;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    vacTrip = null;
                    tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                    //Check lines having vacation trips
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }


                    if (vacTrip != null)
                    {
                        // VA trip
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            dpIndex = 0;

                            //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                            foreach (var dp in trip.DutyPeriods)
                            {

                                if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                                {
                                    DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);


                                    if (GlobalSettings.CurrentBidDetails.Month == 11)
                                    {
                                        DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                        while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            fourthThursday = fourthThursday.AddDays(1);
                                        }
                                        if (dt.Day == fourthThursday.Day)
                                        {
                                            if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                            {
                                                line.HolRig = dp.Tfp * 0.5m;
                                                if (line.HolRig < 2.5m)
                                                    line.HolRig = 2.5m;
                                            }
                                            else
                                            {
                                                line.HolRig = dp.Tfp * 1m;
                                                if (line.HolRig < 4m)
                                                    line.HolRig = 4m;
                                            }

                                            // break;
                                        }


                                    }
                                    else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                                    {

                                        if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                        {
                                            decimal pay = dp.Tfp * 1m;
                                            if (pay < 4m)
                                                pay = 4m;
                                            line.HolRig += pay;
                                        }
                                    }
                                    else
                                    {
                                        if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                        {
                                            line.HolRig = dp.Tfp * 0.5m;
                                            if (line.HolRig < 2.5m)
                                                line.HolRig = 2.5m;
                                        }
                                    }
                                }

                                dpIndex++;

                            }
                        }
                    }
                    else
                    {
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);

                            if (GlobalSettings.CurrentBidDetails.Month == 11)
                            {
                                DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    fourthThursday = fourthThursday.AddDays(1);
                                }
                                if (dt.Day == fourthThursday.Day)
                                {
                                    if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                    {
                                        line.HolRig = dp.Tfp * 0.5m;
                                        if (line.HolRig < 2.5m)
                                            line.HolRig = 2.5m;
                                    }
                                    else
                                    {
                                        line.HolRig = dp.Tfp * 1m;
                                        if (line.HolRig < 4m)
                                            line.HolRig = 4m;
                                    }

                                    break;
                                }


                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                            {

                                if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                {
                                    decimal pay = dp.Tfp * 1m;
                                    if (pay < 4m)
                                        pay = 4m;
                                    line.HolRig += pay;
                                }
                            }
                            else
                            {
                                if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                {
                                    line.HolRig = dp.Tfp * 0.5m;
                                    if (line.HolRig < 2.5m)
                                        line.HolRig = 2.5m;
                                }
                            }

                        }

                    }





                }
            }
            line.HolRig = Math.Round(line.HolRig, 2);
        }





        private void CalculateHolidayRigDropProperties(Line line)
        {
            line.HolRig = 0;

            if (GlobalSettings.CurrentBidDetails.Month == 1 || GlobalSettings.CurrentBidDetails.Month == 11 || GlobalSettings.CurrentBidDetails.Month == 12)
            {


                Trip trip = null;
                VacationStateTrip vacTrip = null;
                DateTime oldTripEndDate = DateTime.MinValue;
                DateTime tripStartDate = DateTime.MinValue;
                string sdow = string.Empty;


                int paringCount = 0;


                bool isLastTrip = false;

                foreach (var pairing in line.Pairings)
                {
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    if (vacTrip != null)
                    {
                        //we dont need to consider vacation trip
                        continue;
                    }
                    else
                    {


                        tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);

                            if (GlobalSettings.CurrentBidDetails.Month == 11)
                            {
                                DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    fourthThursday = fourthThursday.AddDays(1);
                                }
                                if (dt.Day == fourthThursday.Day)
                                {
                                    if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                    {
                                        line.HolRig = dp.Tfp * 0.5m;
                                        if (line.HolRig < 2.5m)
                                            line.HolRig = 2.5m;
                                    }
                                    else
                                    {
                                        line.HolRig = dp.Tfp * 1m;
                                        if (line.HolRig < 4m)
                                            line.HolRig = 4m;
                                    }

                                    break;
                                }


                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                            {

                                if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                {
                                    decimal pay = dp.Tfp * 1m;
                                    if (pay < 4m)
                                        pay = 4m;
                                    line.HolRig += pay;
                                }
                            }
                            else
                            {
                                if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                {
                                    line.HolRig = dp.Tfp * 0.5m;
                                    if (line.HolRig < 2.5m)
                                        line.HolRig = 2.5m;
                                }
                            }

                        }
                    }
                }
            }

            line.HolRig = Math.Round(line.HolRig, 2);
        }


        private void CalculateHolidayRigVacationEOMDropProperties(Line line)
        {
            line.HolRig = 0;

            if (GlobalSettings.CurrentBidDetails.Month == 1 || GlobalSettings.CurrentBidDetails.Month == 11 || GlobalSettings.CurrentBidDetails.Month == 12)
            {


                Trip trip = null;
                VacationStateTrip vacTrip = null;
                DateTime oldTripEndDate = DateTime.MinValue;
                DateTime tripStartDate = DateTime.MinValue;
                string sdow = string.Empty;
                int paringCount = 0;
                bool isLastTrip = false;
                int dpIndex = 0;
                VacationTrip vacationTrip = null;
                foreach (var pairing in line.Pairings)
                {
                    //Get trip
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripDate = DateTime.MinValue;

                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacTrip = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();
                    }
                    //Checking the trip is EOM
                    if (tripDate.AddDays(trip.PairLength - 1) >= _eomDate)
                    {

                        //we dont need to consider EOM trips
                    }
                    //Check lines having vacation trips

                    else if (vacTrip != null)
                    {
                        // VA trip
                        if (vacTrip.TripVacationStartDate == DateTime.MinValue)
                        {
                            continue;
                        }
                        else
                        {
                            dpIndex = 0;

                            //tripStartDate = new DateTime(Convert.ToInt32(_year), Convert.ToInt32(_month), Convert.ToInt16(pairing.Substring(4, 2)));
                            foreach (var dp in trip.DutyPeriods)
                            {

                                if (vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "Split" || vacTrip.VacationDutyPeriods[dpIndex].DutyPeriodType == "VD")
                                {
                                    DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);


                                    if (GlobalSettings.CurrentBidDetails.Month == 11)
                                    {
                                        DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                        while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            fourthThursday = fourthThursday.AddDays(1);
                                        }
                                        if (dt.Day == fourthThursday.Day)
                                        {
                                            if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                            {
                                                line.HolRig = dp.Tfp * 0.5m;
                                                if (line.HolRig < 2.5m)
                                                    line.HolRig = 2.5m;
                                            }
                                            else
                                            {
                                                line.HolRig = dp.Tfp * 1m;
                                                if (line.HolRig < 4m)
                                                    line.HolRig = 4m;
                                            }

                                            // break;
                                        }


                                    }
                                    else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                                    {

                                        if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                        {
                                            decimal pay = dp.Tfp * 1m;
                                            if (pay < 4m)
                                                pay = 4m;
                                            line.HolRig += pay;
                                        }
                                    }
                                    else
                                    {
                                        if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                        {
                                            line.HolRig = dp.Tfp * 0.5m;
                                            if (line.HolRig < 2.5m)
                                                line.HolRig = 2.5m;
                                        }
                                    }
                                }

                                dpIndex++;

                            }
                        }
                    }
                    else
                    {
                        tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);

                            if (GlobalSettings.CurrentBidDetails.Month == 11)
                            {
                                DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    fourthThursday = fourthThursday.AddDays(1);
                                }
                                if (dt.Day == fourthThursday.Day)
                                {
                                    if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                    {
                                        line.HolRig = dp.Tfp * 0.5m;
                                        if (line.HolRig < 2.5m)
                                            line.HolRig = 2.5m;
                                    }
                                    else
                                    {
                                        line.HolRig = dp.Tfp * 1m;
                                        if (line.HolRig < 4m)
                                            line.HolRig = 4m;
                                    }

                                    break;
                                }


                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                            {

                                if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                {
                                    decimal pay = dp.Tfp * 1m;
                                    if (pay < 4m)
                                        pay = 4m;
                                    line.HolRig += pay;
                                }
                            }
                            else
                            {
                                if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                {
                                    line.HolRig = dp.Tfp * 0.5m;
                                    if (line.HolRig < 2.5m)
                                        line.HolRig = 2.5m;
                                }
                            }

                        }
                    }
                }
            }

            line.HolRig = Math.Round(line.HolRig, 2);
        }

        private void CalculateHolidayRigEOMDropProperties(Line line)
        {
            line.HolRig = 0;

            if (GlobalSettings.CurrentBidDetails.Month == 1 || GlobalSettings.CurrentBidDetails.Month == 11 || GlobalSettings.CurrentBidDetails.Month == 12)
            {


                Trip trip = null;
                VacationStateTrip vacTrip = null;
                DateTime oldTripEndDate = DateTime.MinValue;
                DateTime tripStartDate = DateTime.MinValue;
                string sdow = string.Empty;
                int paringCount = 0;
                bool isLastTrip = false;

                VacationTrip vacationTrip = null;
                foreach (var pairing in line.Pairings)
                {
                    //Get trip
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripDate = DateTime.MinValue;

                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //Checking the trip is EOM
                    if (tripDate.AddDays(trip.PairLength - 1) >= _eomDate)
                    {

                        //we dont need to consider EOM trips
                    }
                    else
                    {
                        tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);

                            if (GlobalSettings.CurrentBidDetails.Month == 11)
                            {
                                DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    fourthThursday = fourthThursday.AddDays(1);
                                }
                                if (dt.Day == fourthThursday.Day)
                                {
                                    if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                    {
                                        line.HolRig = dp.Tfp * 0.5m;
                                        if (line.HolRig < 2.5m)
                                            line.HolRig = 2.5m;
                                    }
                                    else
                                    {
                                        line.HolRig = dp.Tfp * 1m;
                                        if (line.HolRig < 4m)
                                            line.HolRig = 4m;
                                    }

                                    break;
                                }


                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                            {

                                if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                {
                                    decimal pay = dp.Tfp * 1m;
                                    if (pay < 4m)
                                        pay = 4m;
                                    line.HolRig += pay;
                                }
                            }
                            else
                            {
                                if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                {
                                    line.HolRig = dp.Tfp * 0.5m;
                                    if (line.HolRig < 2.5m)
                                        line.HolRig = 2.5m;
                                }
                            }

                        }
                    }
                }
            }

            line.HolRig = Math.Round(line.HolRig, 2);
        }


        private void CalculateHolidayRigEOMProperties(Line line)
        {
            line.HolRig = 0;

            if (GlobalSettings.CurrentBidDetails.Month == 1 || GlobalSettings.CurrentBidDetails.Month == 11 || GlobalSettings.CurrentBidDetails.Month == 12)
            {


                Trip trip = null;
                VacationStateTrip vacTrip = null;
                DateTime oldTripEndDate = DateTime.MinValue;
                DateTime tripStartDate = DateTime.MinValue;
                string sdow = string.Empty;
                int paringCount = 0;
                bool isLastTrip = false;

                VacationTrip vacationTrip = null;
                foreach (var pairing in line.Pairings)
                {
                    //Get trip
                    trip = GetTrip(pairing);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    DateTime tripDate = DateTime.MinValue;

                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //Checking the trip is EOM
                    if (tripDate.AddDays(trip.PairLength - 1) >= _eomDate)
                    {
                        //VacationTrip vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                        if (GlobalSettings.VacationData.Keys.Contains(pairing))
                        {
                            vacationTrip = GlobalSettings.VacationData[pairing].VofData;
                            if (vacationTrip != null)
                            {
                                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                                foreach (DutyPeriod dp in trip.DutyPeriods)
                                {

                                    if (vacationTrip.DutyPeriodsDetails[dp.DutPerSeqNum - 1].VacationType == "VD" || vacationTrip.DutyPeriodsDetails[dp.DutPerSeqNum - 1].VacationType == "Split")
                                    {

                                        DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);

                                        if (GlobalSettings.CurrentBidDetails.Month == 11)
                                        {
                                            DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                            while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                            {
                                                fourthThursday = fourthThursday.AddDays(1);
                                            }
                                            if (dt.Day == fourthThursday.Day)
                                            {
                                                if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                                {
                                                    line.HolRig = dp.Tfp * 0.5m;
                                                    if (line.HolRig < 2.5m)
                                                        line.HolRig = 2.5m;
                                                }
                                                else
                                                {
                                                    line.HolRig = dp.Tfp * 1m;
                                                    if (line.HolRig < 4m)
                                                        line.HolRig = 4m;
                                                }

                                                break;
                                            }


                                        }
                                        else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                                        {

                                            if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                            {
                                                decimal pay = dp.Tfp * 1m;
                                                if (pay < 4m)
                                                    pay = 4m;
                                                line.HolRig += pay;
                                            }
                                        }
                                        else
                                        {
                                            if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                            {
                                                line.HolRig = dp.Tfp * 0.5m;
                                                if (line.HolRig < 2.5m)
                                                    line.HolRig = 2.5m;
                                            }
                                        }
                                    }

                                }


                            }
                        }
                    }
                    else
                    {
                        tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                        foreach (DutyPeriod dp in trip.DutyPeriods)
                        {
                            DateTime dt = tripStartDate.AddDays(dp.DutPerSeqNum - 1);

                            if (GlobalSettings.CurrentBidDetails.Month == 11)
                            {
                                DateTime fourthThursday = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 22);

                                while (fourthThursday.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    fourthThursday = fourthThursday.AddDays(1);
                                }
                                if (dt.Day == fourthThursday.Day)
                                {
                                    if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
                                    {
                                        line.HolRig = dp.Tfp * 0.5m;
                                        if (line.HolRig < 2.5m)
                                            line.HolRig = 2.5m;
                                    }
                                    else
                                    {
                                        line.HolRig = dp.Tfp * 1m;
                                        if (line.HolRig < 4m)
                                            line.HolRig = 4m;
                                    }

                                    break;
                                }


                            }
                            else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                            {

                                if (GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25 || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 31)
                                {
                                    decimal pay = dp.Tfp * 1m;
                                    if (pay < 4m)
                                        pay = 4m;
                                    line.HolRig += pay;
                                }
                            }
                            else
                            {
                                if ((GlobalSettings.CurrentBidDetails.Month == 1 && dt.Month == 1 && dt.Day == 1) || GlobalSettings.CurrentBidDetails.Month == 12 && dt.Day == 25)
                                {
                                    line.HolRig = dp.Tfp * 0.5m;
                                    if (line.HolRig < 2.5m)
                                        line.HolRig = 2.5m;
                                }
                            }

                        }
                    }
                }
            }

            line.HolRig = Math.Round(line.HolRig, 2);
        }






        public class MILVacPeriod
        {

            public DateTime StartDate { get; set; }


            public DateTime EndDate { get; set; }

        }

    }
}
