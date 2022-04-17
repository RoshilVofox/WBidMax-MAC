using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    
    public class CalculateLineProperties
    {

        #region Variables
        private Dictionary<string, Trip> trips;
        private Dictionary<string, Line> lines;
        private BidDetails currentBidDetails;
        private string domicile;
        private string position;
        private string round;
        private string month;
        private string year;

        private DateTime bpStartDate;
        private DateTime bpEndDate;

        private List<Day> _newBidPeriodDay;
        private List<Day> _oldBidPeridDays;
        private List<Day> _combinedDays;

        #endregion

        #region PublicMethods
        /// <summary>
        /// Calculate property values for the Line file.
        /// </summary>
        /// <param name="tripdata"></param>
        /// <param name="linesdata"></param>
        public void CalculateLinePropertyValues(Dictionary<string, Trip> tripdata, Dictionary<string, Line> linesdata, BidDetails currentBidDetail)
        {
            try
            {
                //intialise all values
                trips = tripdata;
                lines = linesdata;
                currentBidDetails = currentBidDetail;

                domicile = currentBidDetail.Domicile;
                position = currentBidDetail.Postion;
                round = currentBidDetail.Round;
                month = currentBidDetail.Month.ToString();
                year = currentBidDetail.Year.ToString();

                bpStartDate = currentBidDetail.BidPeriodStartDate;
                bpEndDate = currentBidDetail.BidPeriodEndDate;

                //Calculate the Sips in a Line
                CalculateLineSips();

                foreach (var line in lines)
                {
                    try
                    {
                        if (line.Value.BlankLine)
                        {
                            line.Value.BidLineTemplates = GenerateBidLineViewTemplate(line.Value);
                            line.Value.AMPM = "---";
                            line.Value.AMPMSortOrder = CalcAmPmSortOrder(line.Value);
                            line.Value.TafbInBp = "0:00";
                            line.Value.Equip8753 = string.Empty;
                            continue;
                        }

                        //TfpInLine
                        line.Value.TfpInLine = CalcTfpInLine(line.Value);
                        //TFP in BP
                        line.Value.Tfp = CalcTfpInBP(line.Value);

                        SetMonthlyLineRig(line.Value);

                        line.Value.TfpInLine = line.Value.TfpInLine + line.Value.LineRig;
                        line.Value.Tfp = line.Value.Tfp + line.Value.LineRig;
                        // Block Hours
                        line.Value.BlkHrsInBp = line.Value.Block > 0 ? Helper.ConvertRawHhmmtoHhColonMm(line.Value.Block.ToString()) : "00:00";
                        //Block hours in Line
                        line.Value.BlkHrsInLine = CalcBlkHrsInLine(line.Value);


                        // Weekend  
                        line.Value.Weekend = CalcWkEndProp(line.Value, false);
                        // AMPM  
                        line.Value.AMPM = CalcAmPmProp(line.Value, false);



                        line.Value.AMPMSortOrder = CalcAmPmSortOrder(line.Value);

                        line.Value.BidLineTemplates = GenerateBidLineViewTemplate(line.Value);
                        // line.Value.AMPMSortOrder = CalcAmPmSortOrder(line.Value);
                        // DaysOFF DaysWork
                        line.Value.DaysOff = CalcDaysOff(line.Key);
                        // line.Value.DaysOffDisplay = line.Value.DaysOff;

                        line.Value.DaysWork = (bpEndDate - bpStartDate).Days + 1 - line.Value.DaysOff;
                        line.Value.DaysWorkInLine = CalcDaysWorkInLine(line.Value);
                        // T234  
                        line.Value.T234 = CalcT234(line.Key);
                        


                        int dhrInLine = CalcDutyHrs(line.Value, true);
                        line.Value.DutyHrsInLine = (dhrInLine / 60).ToString() + ":" + (dhrInLine % 60).ToString().PadLeft(2, '0');
                        int dhrInBp = CalcDutyHrs(line.Value, false);
                        line.Value.DutyHrsInBp = (dhrInBp / 60).ToString() + ":" + (dhrInBp % 60).ToString().PadLeft(2, '0');

                        // TAFB
                        line.Value.TafbInBp = line.Value.BlankLine || line.Value.ReserveLine ? "0:00" : CalcTafb(line.Value, false);
                        line.Value.TafbInLine = line.Value.BlankLine || line.Value.ReserveLine ? "0:00" : CalcTafb(line.Value, true);
                       

                        // LegsIn800  
                        line.Value.LegsIn800 = CalcNumLegsOfGivenType(800, line.Value);
                        // LegsIn700  
                        line.Value.LegsIn700 = CalcNumLegsOfGivenType(700, line.Value);
                        // LegsIn600  
                        line.Value.LegsIn600 = CalcNumLegsOfGivenType(600, line.Value);
                        //Legsin200
                        line.Value.LegsIn200 = CalcNumLegsOfGivenType(200, line.Value);
                        // LegsIn500 
                        //line.Value.LegsIn500 = CalcNumLegsOfGivenType(500, line.Value);
                        // LegsIn300 
                        // line.Value.LegsIn300 = CalcNumLegsOfGivenType(300, line.Value);
                        // Equip8753
                        line.Value.Equip8753 = line.Value.LegsIn700.ToString() + "-" +
                                               line.Value.LegsIn800.ToString() + "-" +
                                               line.Value.LegsIn200.ToString() + "-" +
                                               line.Value.LegsIn600.ToString();
                        // LongestGrndTime 
                        line.Value.LongestGrndTime = CalcLongGrndTime(line.Value);
                        // Trips1Day 
                        line.Value.Trips1Day = CalcNumTripsOfGivenLength(1, line.Value);
                        // Trips2Day 
                        line.Value.Trips2Day = CalcNumTripsOfGivenLength(2, line.Value);
                        // Trips3Day
                        line.Value.Trips3Day = CalcNumTripsOfGivenLength(3, line.Value);
                        // Trips4Day
                        line.Value.Trips4Day = CalcNumTripsOfGivenLength(4, line.Value);
                        // AcftChanges - frank add Monday 2/11/2013
                        line.Value.AcftChanges = CalcNumAcftChanges(line.Value);
                        // AcftChgDay - frank add Monday 2/11/2013
                        line.Value.AcftChgDay = Math.Round(line.Value.ReserveLine || line.Value.BlankLine ? 0 : line.Value.AcftChanges / (decimal)line.Value.DaysWorkInLine, 2);
                        //carryover
                        CalcCarryOver(line.Value);
                        // TotDutyPds 
                        line.Value.TotDutyPds = CalcTotDutPds(line.Value, true);   // true calculates all duty periods
                        // TotDutyPdsInBidPd
                        line.Value.TotDutyPdsInBp = CalcTotDutPds(line.Value, false);   // false calculates all duty periods in bid period
                        // EDomPush  
                        line.Value.EDomPush = CalcEDomPush(line.Value).ToString(@"hh\:mm");

                        // EPush  
                        line.Value.EPush = CalcEPush(line.Value).ToString(@"hh\:mm");
                        // FAPositions  
                        // LastArrTime 
                        line.Value.LastArrTime = CalcLastArrTime(line.Value);
                        // LastDomArrTime 
                        line.Value.LastDomArrTime = CalcLastDomArrTime(line.Value);
                        // Legs  
                        if (!line.Value.ReserveLine)
                        {
                            line.Value.Legs = CalcTotLegs(line.Value);

                        }
                        // LegsPerDay 
                        line.Value.LegsPerDay = line.Value.ReserveLine || line.Value.BlankLine ? 0 :
                                                    Math.Round(Convert.ToDecimal(line.Value.Legs) / Convert.ToDecimal(line.Value.DaysWork), 2);
                        // LegsPerPair 
                        line.Value.LegsPerPair = line.Value.ReserveLine || line.Value.BlankLine ? 0 :
                                                    Math.Round(Convert.ToDecimal(line.Value.Legs) / Convert.ToDecimal(line.Value.Pairings.Count()), 2);
                        // TotTrips
                        line.Value.TotPairings = line.Value.Pairings.Count();
                        // Sips
                        line.Value.Sips = CalcSips(line.Value);
                        // StartDow
                        line.Value.StartDow = CalcStartDow(line.Value);

                        line.Value.StartDaysList = CalcStartDowList(line.Value);
                        line.Value.StartDaysListPerTrip = CalcStartDowListPerTrip(line.Value);
                       
                        // LargestBlkOfDaysOff
                        line.Value.LargestBlkOfDaysOff = CalcLargestBlkDaysOff(line.Value);
                        // BlkOfDaysOff
                        line.Value.BlkOfDaysOff = CalcBlkOfDaysOff(line.Value);
                        //LegsPerDutyPeriod
                        line.Value.LegsPerDutyPeriod = CalcLegsPerDutyPeriod(line.Value);
                        // DaysOfWeekWork
                        line.Value.DaysOfWeekWork = CalcWeekDaysWork(line.Value);

                        line.Value.DaysOfMonthWorks = CalcDaysOfMonthOff(line.Value);
                        // OvernightCities
                        CalculateOvernightCities(line.Value);

                        // RestPeriods
                        CalculateRestPeriods(line.Value);
                        // WorkBlockLengths
                        CalculateWorkBlockLength(line.Value);
                        // CmtDhds
                        line.Value.CmtDhds = CalcDeadheadCities(line.Value);

                        //Calculate workBlock details
                        CalculateWorkBlockDetails(line.Value);
                        line.Value.NightsInMid = line.Value.WorkBlockList.Sum(x => x.nightINDomicile);
                        line.Value.TotalCommutes = line.Value.WorkBlockList.Count;
                        CalculateAverageLatestDomicileArrivalAndPush(line.Value);
                        // CalculatePairingWorkDetails(line.Value);

                        CalculateDhFirstandDeadHeadDhLast(line.Value);
                        CalculateDhTotal(line.Value);
                        if (GlobalSettings.IsOverlapCorrection)
                        {
                            CalculateOverlapDays(line.Value);
                        }
                        else
                            GlobalSettings.IsOverlapCorrection = false;
                        // calculate status of each day (off, city, depTime, arrTime, etc)
                        line.Value.DaysInBidPeriod = CalculateDayStatusInBidPeriod(line.Value);

                        line.Value.MostLegs = CalculateMostlegs(line.Value);
                        line.Value.DutyPeriodHours = CalcDutyPeriodHours(line.Value);
                        line.Value.Is3on3Off = CalculateIs3on3OffDay(line.Value);
                        line.Value.GroundTimes = CalculateGroundTimes(line.Value);

                        line.Value.CarryOverTfp = line.Value.TfpInLine - line.Value.Tfp;

                        if (line.Value.CarryOverTfp < 0 && line.Value.CarryOverTfp > -1.0m)
                        {
                            line.Value.CarryOverTfp = 0;
                        }
                        if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                        {
                            line.Value.ETOPS = CalculateETOPSProperties(line.Value);
                        }
                        if (line.Value.ETOPS)
                        {
                            line.Value.ETOPSTripsCount = CalculateETOPSTripsCount(line.Value);
                        }
                        SetInternationAndNoConusLine(line.Value);
                        ///Calculate Rig related properties
                        CalculateRigProperties(line.Value);
                        CalculateHolidayRigProperties(line.Value);

                        line.Value.VacPlusRig = line.Value.VacPayCuBp + line.Value.LineRig;
                        //Temporary variable to keep original value
                        line.Value.TempBlkHrsInBp = line.Value.BlkHrsInBp;
                        line.Value.TempTfp = line.Value.Tfp;
                        line.Value.TempDaysOff = line.Value.DaysOff;
                        line.Value.TempTafbInBp = line.Value.TafbInBp;
                        line.Value.TempLegs = line.Value.Legs;
                        line.Value.TempDaysWorkInLine = line.Value.DaysWorkInLine;
                        line.Value.TempTfpInLine = line.Value.TfpInLine;
                        line.Value.TempBlkHrsInLine = line.Value.BlkHrsInLine;
                        line.Value.Tfp = line.Value.Tfp + line.Value.HolRig;

                        // TfpPerFltHr
                        line.Value.TfpPerFltHr = Math.Round(CalcTfpPerFltHr(line.Value), 2);
                        // TfpPerTafb
                        if (!line.Value.ReserveLine && !line.Value.BlankLine)
                        {
                            string[] tafbTime = line.Value.TafbInBp.Split(':');
                            decimal tafbInMin = Convert.ToDecimal(tafbTime[0]) * 60 + Convert.ToDecimal(tafbTime[1]);
                            line.Value.TfpPerTafb = line.Value.ReserveLine || line.Value.BlankLine || tafbInMin == 0 ? 0m : Math.Round(line.Value.Tfp / (tafbInMin / 60m), 2);
                        }
                        // TfpPerDhr
                        string[] dhrTime = line.Value.DutyHrsInBp.Split(':');
                        decimal dhrInMin = Convert.ToDecimal(dhrTime[0]) * 60 + Convert.ToDecimal(dhrTime[1]);
                        line.Value.TfpPerDhr = line.Value.ReserveLine || line.Value.BlankLine || dhrInMin == 0 ? 0m : Math.Round(line.Value.Tfp / (dhrInMin / 60m), 2);
                        // TfpPerDay
                        CalculateTfpPerDay(line.Value);
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



        //public void CalculateLinePropertyValues(LineProperties lineProperties)
        //{
        //    try
        //    {
        //        //Number of parellel task
        //        int taskCount = 6;

        //        if (lineProperties.LinesData.Count < 6)
        //        {
        //            taskCount = lineProperties.LinesData.Count;
        //        }

        //        int subListCount = lineProperties.LinesData.Count / taskCount;
        //        var tasks = new Task[taskCount];
        //        int starRange = 0;
        //        int endRange = 0;

        //        //intialise all values
        //        trips = lineProperties.TripData;
        //        currentBidDetails = lineProperties.CurrentBidDetail;
        //        domicile = lineProperties.CurrentBidDetail.Domicile;
        //        position = lineProperties.CurrentBidDetail.Postion;
        //        round = lineProperties.CurrentBidDetail.Round;
        //        month = lineProperties.CurrentBidDetail.Month.ToString();
        //        year = lineProperties.CurrentBidDetail.Year.ToString();
        //        bpStartDate = lineProperties.CurrentBidDetail.BidPeriodStartDate;
        //        bpEndDate = lineProperties.CurrentBidDetail.BidPeriodEndDate;

        //        //Calculate the Sips in a Line
        //        //CalculateLineSips();


        //        for (int count = 0; count < taskCount; count++)
        //        {
        //            starRange = count * subListCount + 1;

        //            if (count == taskCount - 1)
        //            {

        //                endRange = lineProperties.LinesData.Count;
        //            }
        //            else
        //            {
        //                endRange = endRange + subListCount;
        //            }
        //            tasks[count] = Task.Factory.StartNew(() => CalculateProperties(lineProperties.LinesData.Where(x => int.Parse(x.Key) >= starRange && int.Parse(x.Key) <= endRange).ToDictionary(pair => pair.Key, pair => pair.Value)));
        //        }

        //        Task.WaitAll(tasks);

        //        Dictionary<string, Line> result = new Dictionary<string, Line>();

        //        int tCount = 0;
        //        foreach (Task task in tasks)
        //        {
        //            var subLines = ((Task<Dictionary<string, Line>>)tasks[tCount]).Result;
        //            foreach (var singleItem in subLines)
        //                result.Add(singleItem.Key, singleItem.Value);
        //            tCount++;

        //        }



        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //   private Dictionary<string, Line> CalculateProperties(Dictionary<string, Line> lines)
        //   {
        //       foreach (var line in lines)
        //       {
        //           try
        //           {
        //               if (line.Value.BlankLine)
        //               {
        //                   line.Value.BidLineTemplates = GenerateBidLineViewTemplate(line.Value);
        //                   line.Value.AMPM = "---";
        //                   continue;
        //               }

        //               //TfpInLine
        //               line.Value.TfpInLine = CalcTfpInLine(line.Value);
        //               //TFP in BP
        //               line.Value.Tfp = CalcTfpInBP(line.Value);
        //               // Block Hours
        //               line.Value.BlkHrsInBp = line.Value.Block > 0 ? Helper.ConvertRawHhmmtoHhColonMm(line.Value.Block.ToString()) : "00:00";
        //               //Block hours in Line
        //               line.Value.BlkHrsInLine = CalcBlkHrsInLine(line.Value);


        //               // Weekend  
        //               // line.Value.Weekend = CalcWkEndProp(line.Value);
        //               // AMPM  
        //               //line.Value.AMPM = CalcAmPmProp(line.Value);

        //               line.Value.BidLineTemplates = GenerateBidLineViewTemplate(line.Value);
        //               // line.Value.AMPMSortOrder = CalcAmPmSortOrder(line.Value);
        //               // DaysOFF DaysWork
        //               line.Value.DaysOff = CalcDaysOff(line.Key);
        //               // line.Value.DaysOffDisplay = line.Value.DaysOff;

        //               line.Value.DaysWork = (bpEndDate - bpStartDate).Days + 1 - line.Value.DaysOff;
        //               line.Value.DaysWorkInLine = CalcDaysWorkInLine(line.Value);
        //               // T234  
        //               line.Value.T234 = CalcT234(line.Key);
        //               // TfpPerFltHr
        //               line.Value.TfpPerFltHr = Math.Round(CalcTfpPerFltHr(line.Value), 2);


        //               int dhrInLine = CalcDutyHrs(line.Value, true);
        //               line.Value.DutyHrsInLine = (dhrInLine / 60).ToString() + ":" + (dhrInLine % 60).ToString().PadLeft(2, '0');
        //               int dhrInBp = CalcDutyHrs(line.Value, false);
        //               line.Value.DutyHrsInBp = (dhrInBp / 60).ToString() + ":" + (dhrInBp % 60).ToString().PadLeft(2, '0');

        //               // TAFB
        //               line.Value.TafbInBp = line.Value.BlankLine || line.Value.ReserveLine ? "0:00" : CalcTafb(line.Value, false);
        //               line.Value.TafbInLine = line.Value.BlankLine || line.Value.ReserveLine ? "0:00" : CalcTafb(line.Value, true);
        //               // TfpPerTafb
        //               if (!line.Value.ReserveLine && !line.Value.BlankLine)
        //               {
        //                   string[] tafbTime = line.Value.TafbInBp.Split(':');
        //                   decimal tafbInMin = Convert.ToDecimal(tafbTime[0]) * 60 + Convert.ToDecimal(tafbTime[1]);
        //                   line.Value.TfpPerTafb = line.Value.ReserveLine || line.Value.BlankLine || tafbInMin == 0 ? 0m : Math.Round(line.Value.Tfp / (tafbInMin / 60m), 2);
        //               }

        //               // LegsIn800  
        //               line.Value.LegsIn800 = CalcNumLegsOfGivenType(800, line.Value);
        //               // LegsIn700  
        //               line.Value.LegsIn700 = CalcNumLegsOfGivenType(700, line.Value);
        //               // LegsIn500 
        //               line.Value.LegsIn500 = CalcNumLegsOfGivenType(500, line.Value);
        //               // LegsIn300 
        //               line.Value.LegsIn300 = CalcNumLegsOfGivenType(300, line.Value);
        //               // Equip8753
        //               line.Value.Equip8753 = line.Value.LegsIn800.ToString() + "-" +
        //                                      line.Value.LegsIn700.ToString() + "-" +
        //                                      line.Value.LegsIn300.ToString();
        //               // LongestGrndTime 
        //               line.Value.LongestGrndTime = CalcLongGrndTime(line.Value);
        //               // Trips1Day 
        //               line.Value.Trips1Day = CalcNumTripsOfGivenLength(1, line.Value);
        //               // Trips2Day 
        //               line.Value.Trips2Day = CalcNumTripsOfGivenLength(2, line.Value);
        //               // Trips3Day
        //               line.Value.Trips3Day = CalcNumTripsOfGivenLength(3, line.Value);
        //               // Trips4Day
        //               line.Value.Trips4Day = CalcNumTripsOfGivenLength(4, line.Value);
        //               // AcftChanges - frank add Monday 2/11/2013
        //               line.Value.AcftChanges = CalcNumAcftChanges(line.Value);
        //               // AcftChgDay - frank add Monday 2/11/2013
        //               line.Value.AcftChgDay = Math.Round(line.Value.ReserveLine || line.Value.BlankLine ? 0 : line.Value.AcftChanges / (decimal)line.Value.DaysWorkInLine, 2);
        //               //carryover
        //               CalcCarryOver(line.Value);
        //               // TotDutyPds 
        //               line.Value.TotDutyPds = CalcTotDutPds(line.Value, true);   // true calculates all duty periods
        //               // TotDutyPdsInBidPd
        //               line.Value.TotDutyPdsInBp = CalcTotDutPds(line.Value, false);   // false calculates all duty periods in bid period
        //               // EDomPush  
        //               line.Value.EDomPush = CalcEDomPush(line.Value).ToString(@"hh\:mm");

        //               // EPush  
        //               line.Value.EPush = CalcEPush(line.Value).ToString(@"hh\:mm");
        //               // FAPositions  
        //               // LastArrTime 
        //               line.Value.LastArrTime = CalcLastArrTime(line.Value);
        //               // LastDomArrTime 
        //               line.Value.LastDomArrTime = CalcLastDomArrTime(line.Value);
        //               // Legs  
        //               if (!line.Value.ReserveLine)
        //               {
        //                   line.Value.Legs = CalcTotLegs(line.Value);

        //               }
        //               // LegsPerDay 
        //               line.Value.LegsPerDay = line.Value.ReserveLine || line.Value.BlankLine ? 0 :
        //                                           Math.Round(Convert.ToDecimal(line.Value.Legs) / Convert.ToDecimal(line.Value.DaysWork), 2);
        //               // LegsPerPair 
        //               line.Value.LegsPerPair = line.Value.ReserveLine || line.Value.BlankLine ? 0 :
        //                                           Math.Round(Convert.ToDecimal(line.Value.Legs) / Convert.ToDecimal(line.Value.Pairings.Count()), 2);
        //               // TotTrips
        //               line.Value.TotPairings = line.Value.Pairings.Count();
        //               // Sips
        //               line.Value.Sips = CalcSips(line.Value);
        //               // StartDow
        //               line.Value.StartDow = CalcStartDow(line.Value);

        //               line.Value.StartDaysList = CalcStartDowList(line.Value);

        //line.Value.StartDaysListPerTrip = CalcStartDowListPerTrip(line.Value);
        //               // TfpPerDhr
        //               string[] dhrTime = line.Value.DutyHrsInBp.Split(':');
        //               decimal dhrInMin = Convert.ToDecimal(dhrTime[0]) * 60 + Convert.ToDecimal(dhrTime[1]);
        //               line.Value.TfpPerDhr = line.Value.ReserveLine || line.Value.BlankLine || dhrInMin == 0 ? 0m : Math.Round(line.Value.Tfp / (dhrInMin / 60m), 2);
        //               // TfpPerDay
        //               CalculateTfpPerDay(line.Value);
        //               // LargestBlkOfDaysOff
        //               line.Value.LargestBlkOfDaysOff = CalcLargestBlkDaysOff(line.Value);
        //               // BlkOfDaysOff
        //               line.Value.BlkOfDaysOff = CalcBlkOfDaysOff(line.Value);
        //               //LegsPerDutyPeriod
        //               line.Value.LegsPerDutyPeriod = CalcLegsPerDutyPeriod(line.Value);
        //               // DaysOfWeekWork
        //               line.Value.DaysOfWeekWork = CalcWeekDaysWork(line.Value);

        //               line.Value.DaysOfMonthWorks = CalcDaysOfMonthOff(line.Value);
        //               // OvernightCities
        //               CalculateOvernightCities(line.Value);

        //               // RestPeriods
        //               CalculateRestPeriods(line.Value);
        //               // WorkBlockLengths
        //               CalculateWorkBlockLength(line.Value);
        //               // CmtDhds
        //               line.Value.CmtDhds = CalcDeadheadCities(line.Value);

        //               //Calculate workBlock details
        //               CalculateWorkBlockDetails(line.Value);
        //line.Value.NightsInMid = line.Value.WorkBlockList.Sum(x => x.nightINDomicile);
        //line.Value.TotalCommutes = line.Value.WorkBlockList.Count;
        //               //CalculatePairingWorkDetails(line.Value);

        //               CalculateDhFirstandDeadHeadDhLast(line.Value);

        //               //if (GlobalSettings.IsOverlapCorrection && round == "M")
        //               //{
        //               //    CalculateOverlapDays(line.Value);
        //               //}

        //               // calculate status of each day (off, city, depTime, arrTime, etc)
        //               line.Value.DaysInBidPeriod = CalculateDayStatusInBidPeriod(line.Value);

        //               line.Value.MostLegs = CalculateMostlegs(line.Value);
        //               line.Value.DutyPeriodHours = CalcDutyPeriodHours(line.Value);
        //               line.Value.Is3on3Off = CalculateIs3on3OffDay(line.Value);
        //               line.Value.GroundTimes = CalculateGroundTimes(line.Value);

        //               line.Value.CarryOverTfp = line.Value.TfpInLine - line.Value.Tfp;

        //               if (line.Value.CarryOverTfp < 0 && line.Value.CarryOverTfp > -1.0m)
        //               {
        //                   line.Value.CarryOverTfp = 0;
        //               }


        //               ///Calculate Rig related properties
        //               CalculateRigProperties(line.Value);
        //               //Temporary variable to keep original value
        //               line.Value.TempBlkHrsInBp = line.Value.BlkHrsInBp;
        //               line.Value.TempTfp = line.Value.Tfp;
        //               line.Value.TempDaysOff = line.Value.DaysOff;
        //               line.Value.TempTafbInBp = line.Value.TafbInBp;
        //               line.Value.TempLegs = line.Value.Legs;
        //               line.Value.TempDaysWorkInLine = line.Value.DaysWorkInLine;
        //               line.Value.TempTfpInLine = line.Value.TfpInLine;
        //               line.Value.TempBlkHrsInLine = line.Value.BlkHrsInLine;
        //           }
        //           catch (Exception ex)
        //           {
        //               throw ex;

        //           }

        //       }

        //       return lines;

        //   }

        /// <summary>
        /// If any line contains a trip that has a true International property, then that line will have it’s International property set to true
        /// Similarly for the NonConus line
        /// </summary>
        /// <param name="line"></param>
        private void SetInternationAndNoConusLine(Line line)
        {
            Trip trip = null;
            foreach (var pairing in line.Pairings)
            {

                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                if (trip.International)
                    line.International = true;
                if (trip.NonConus)
                    line.NonConus = true;
            }
        }


        #endregion

        #region PrivateMethods

        /// <summary>
        /// Calculate overlap Days
        /// </summary>
        /// <param name="line"></param>
        private void CalculateOverlapDays(Line line)
        {

            if (line.LineNum == 410)
            {
            }
            _newBidPeriodDay = new List<Day>();
            _oldBidPeridDays = GlobalSettings.LeadOutDays;
            _combinedDays = new List<Day>();

            //Temporay code for testing
            // frank added day in front, should be last 6 days from old bid period and 6 lead-in days from old bid period
            // also, made data more realistic
            // to test April month
            //_oldBidPeridDays = new List<Day>()
            //{
            //    new Day{Date=new DateTime(2013,3,26),FlightTime=450,DepartutreTime=420,ArrivalTime=895},       // added another day so there are 6 days from old bid period instead of 5
            //    new Day{Date=new DateTime(2013,3,27),FlightTime=450,DepartutreTime=0,ArrivalTime=0},
            //    new Day{Date=new DateTime(2013,3,28),FlightTime=0,DepartutreTime=0,ArrivalTime=0},         // changed by frank -- in any line there is always 3 days off between pairings
            //    new Day{Date=new DateTime(2013,3,29),FlightTime=0,DepartutreTime=0,ArrivalTime=0},         // also, original lines never fly more than 4 days in a row
            //    new Day{Date=new DateTime(2013,3,30),FlightTime=0,DepartutreTime=0,ArrivalTime=0},         // however, after an overlap correction, it is possible to fly 6 days in a row
            //    new Day{Date=new DateTime(2013,3,31),FlightTime=435,DepartutreTime=380,ArrivalTime=880},
            //    new Day{Date=new DateTime(2013,4,1),FlightTime=325,DepartutreTime=300,ArrivalTime=665},
            //    new Day{Date=new DateTime(2013,4,2),FlightTime=425,DepartutreTime=300,ArrivalTime=930},
            //    new Day{Date=new DateTime(2013,4,3),FlightTime=475,DepartutreTime=300,ArrivalTime=860},
            //    new Day{Date=new DateTime(2013,4,4),FlightTime=0,DepartutreTime=0,ArrivalTime=0},
            //    new Day{Date=new DateTime(2013,4,5),FlightTime=0,DepartutreTime=0,ArrivalTime=0},
            //    new Day{Date=new DateTime(2013,4,6),FlightTime=0,DepartutreTime=0,ArrivalTime=0}
            //};

            // to test march month
            //_oldBidPeridDays = new List<Day>()
            //{
            //    new Day{Date=new DateTime(2013,2,23),FlightTime=450},       // added another day so there are 6 days from old bid period instead of 5
            //    new Day{Date=new DateTime(2013,2,24),FlightTime=450},
            //    new Day{Date=new DateTime(2013,2,25),FlightTime=0},         // changed by frank -- in any line there is always 3 days off between pairings
            //    new Day{Date=new DateTime(2013,2,26),FlightTime=0},         // also, original lines never fly more than 4 days in a row
            //    new Day{Date=new DateTime(2013,2,27),FlightTime=0},         // however, after an overlap correction, it is possible to fly 6 days in a row
            //    new Day{Date=new DateTime(2013,2,28),FlightTime=435},
            //    new Day{Date=new DateTime(2013,3,1),FlightTime=325},
            //    new Day{Date=new DateTime(2013,3,2),FlightTime=425},
            //    new Day{Date=new DateTime(2013,3,3),FlightTime=475},
            //    new Day{Date=new DateTime(2013,3,4),FlightTime=0},
            //    new Day{Date=new DateTime(2013,3,5),FlightTime=0},
            //    new Day{Date=new DateTime(2013,3,6),FlightTime=0}
            //};

            //int daysinmonth = DateTime.DaysInMonth(int.Parse(year), int.Parse(month));
            //DateTime date = new DateTime(int.Parse(year), int.Parse(month), 1);
            int daysinmonth = (bpEndDate - bpStartDate).Days + 1;
            DateTime date = bpStartDate;

            //create date for the new period day
            for (int count = 0; count < daysinmonth; count++)
            {
                Day day = new Day();
                day.Date = date;
                date = date.AddDays(1);
                _newBidPeriodDay.Add(day);
            }

            ///iterate through all the Sips in the selected line
            foreach (var sip in line.LineSips)
            {
                DateTime sipstartdate = sip.SipStartDate;
                for (int count = 0; count < sip.Sip.SipDutyPeriods.Count; count++)
                {

                    if (_newBidPeriodDay.Any(x => x.Date == sip.SipStartDate))
                    {

                        Day objday = _newBidPeriodDay.Where(x => x.Date == sipstartdate).FirstOrDefault();
                        if (objday != null)
                        {
                            objday.FlightTime += sip.Sip.SipDutyPeriods[count].FlightHours;
                            sipstartdate = sipstartdate.AddDays(1);
                        }
                    }
                }
            }




            // This Will combime _oldBidPeridDays and _newBidPeriodDay lists for overlap correction
            GenarateCombinedDays();
            //var isSucccess= GenarateCombinedDays();
            //if (isSucccess) {

            //Check line is overlapped with previous date 
            OverlapConflictCheck(line);
            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                //30/7  check. Check to see 30 hours(1800 minutes) maximum flight time for one week.
                OverlapCorrrectionWeekhours(line);
            }

            // If the amount of rest from the lead in days to the start of the lead out days is less than GlobalSetting.requiredRest, then drop the first SIP.
            MinimumRestOverlapCorrrection(line);

            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                //100 hours in a month.check the new bid period have totally 100 hours flight time.if not remove the lowest TFP SIP that will make the line legal for 100 hours and mark that LineSip as dropped.
                OverlapMonthhoursCheck(line);
            }

            //24hoursRestCheck -Check each 7 days has atleat 24 hours off
            overlap24hoursRestCheck(line);

            //set the showoverlap line property  to true if any of the sip droppped for the line
            //setShowOvelapVisibility(line);

            //} 
        }

        /// <summary>
        /// set the showoverlap line property  to true if any of the sip droppped for the line
        /// </summary>
        /// <param name="line"></param>
        private void setShowOvelapVisibility(Line line)
        {
            line.ShowOverLap = line.LineSips.Any(x => x.Dropped);
        }

        /// <summary>
        /// This Will combime _oldBidPeridDays and _newBidPeriodDay lists for overlap correction
        /// </summary>
        private void GenarateCombinedDays()
        {
            //try {
            //genarate combine dayas(Sum of _newBidPeriodDay and  _oldBidPeridDays lists)
            if (_oldBidPeridDays.Count > 0)
                _combinedDays.AddRange(_oldBidPeridDays);
            if (_newBidPeriodDay.Count > 0)
                _combinedDays.AddRange(_newBidPeriodDay);

            _combinedDays = _combinedDays.GroupBy(x => x.Date).Select(y => new Day { Date = y.Key, FlightTime = y.Sum(z => z.FlightTime) }).ToList();

            //}
            //catch (Exception ex) 
            //{
            //throw ex;
            //return false;
            //}
            //return true;

        }

        /// <summary>
        /// Check line is overlapped with previous date (Condition :lead out trip has flying on the same day as lead in trip)
        /// </summary>
        /// <param name="line"></param>
        private void OverlapConflictCheck(Line line)
        {
            //get the valid start date for new bid period

            //var validcurrentbiddate = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault().Date;
            DateTime validcurrentbiddate;
            var item = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault();
            if (item != null)
            {
                validcurrentbiddate = item.Date;

                var oldbidperiodenddate = validcurrentbiddate.AddDays(-1);

                for (int count = 0; count < 6; count++)
                {
                    var date = _newBidPeriodDay[count].Date;

                    if (_newBidPeriodDay.Where(x => x.Date == date).FirstOrDefault() != null && _oldBidPeridDays.Where(x => x.Date == date).FirstOrDefault() != null)
                    {
                        //check the flight time exist for _newBidPeriodDay and _oldBidPeridDays for the same day
                        if (_newBidPeriodDay.Where(x => x.Date == date).FirstOrDefault().FlightTime != 0 && _oldBidPeridDays.Where(x => x.Date == date).FirstOrDefault().FlightTime != 0)
                        {
                            //remove the droped sip from the siplist.
                            var linesips = line.LineSips.Where(x => !x.Dropped).ToList();

                            foreach (var sip in linesips)
                            {

                                //get the sip start date of the SIP
                                DateTime sipstartdate = sip.SipStartDate;

                                //if the date is oldbidperiodenddate then we have to check whether it satisfy the minimum Rest time.if it satisfy the minimum rest time then we can make this sip as legal(rare chance?)
                                if (date == oldbidperiodenddate)
                                {
                                    int requiredRest = GlobalSettings.requiredRest;
                                    //int LastLegArrivalTime=GlobalSettings.OverlapDetails.LastLegArrivalTime;
                                    //for the test purpose
                                    int LastLegArrivalTime = GlobalSettings.LastLegArrivalTime;

                                    //get the start leg departure time for the Sip
                                    int startLegDepartureTime = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault().DepTime;
                                    if (LastLegArrivalTime + requiredRest < startLegDepartureTime)
                                    {
                                        continue;
                                    }
                                }
                                ///check the assosiate sip  for the date in the illegalWeekhoursDays
                                if (sipstartdate <= date && date <= sipstartdate.AddDays(sip.Sip.SipDutyPeriods.Count - 1))
                                {
                                    //remove the sip (Drop the flight hours for the sip from the Sip opearating days)
                                    DropoverlappedSip(sip, sipstartdate);
                                }
                            }

                        }
                    }
                }
            }

        }

        /// <summary>
        /// 30/7  check. Check to see 30 hours(1800 minutes) maximum flight time for one week.
        /// </summary>
        /// <param name="line"></param>
        private void OverlapCorrrectionWeekhours(Line line)
        {

            List<Day> illegalWeekhoursDays = new List<Day>();
            //get the valid start date for new bid period
            DateTime date;
            var oldBidPeriodday = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault();
            if (oldBidPeriodday != null)
            {
                date = oldBidPeriodday.Date;
            }
            else
                date = _oldBidPeridDays.LastOrDefault().Date.AddDays(1);            ///back ward iteration to avoid the pupose of kepping the forward and reverse illegal days in separate list(iterate upto the 12 th day of the new bid period)
            for (DateTime iterationdate = date; iterationdate < new DateTime(int.Parse(year), int.Parse(month), 12); iterationdate = iterationdate.AddDays(1))
            {
                int flighttimesum = 0;
                flighttimesum = _combinedDays.Where(y => (iterationdate.AddDays(-6) <= y.Date) && y.Date <= iterationdate).Sum(x => x.FlightTime);
                if (flighttimesum > 1800)
                {
                    var illegalday = _combinedDays.Where(x => x.Date == iterationdate).FirstOrDefault();
                    if (illegalday.FlightTime != 0)
                    {
                        illegalWeekhoursDays.Add(illegalday);
                    }
                }

            }
            //iterate illegalWeekhoursDays
            foreach (Day day in illegalWeekhoursDays)
            {
                var excessflighttimehours = _combinedDays.Where(y => (day.Date.AddDays(-6) <= y.Date) && y.Date <= day.Date).Sum(x => x.FlightTime) - 1800;

                if (excessflighttimehours > 0)
                {
                    //remove the droped sip from the siplist.
                    var linesips = line.LineSips.Where(x => !x.Dropped).ToList();

                    //get the linesips for the 7 days period and it ordered by flight time ascending values.
                    linesips = linesips.Where(x => ((day.Date.AddDays(-6) <= x.SipStartDate || (day.Date.AddDays(-6) <= x.SipStartDate.AddDays(x.Sip.SipDutyPeriods.Count - 1))) && x.SipStartDate <= day.Date)).OrderBy(x => x.SipFltHrs).ToList();
                    foreach (var sip in linesips)
                    {
                        //get the sip start date of the SIP
                        DateTime sipstartdate = sip.SipStartDate;

                        //remove the sip (Drop the flight hours for the sip from the Sip opearating days)
                        DropoverlappedSip(sip, sipstartdate);

                        //check the legality --check for week hours after dropped the flight hours (returns true if it make legal)
                        bool legal = CheckOverlaplegalityforweekhours(day.Date.AddDays(-6), day.Date);
                        if (legal)
                        {
                            //if the droping the sip makes it legal ,then go to the next illegalWeekhoursDays
                            break;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// If the amount of rest from the lead in days to the start of the lead out days is less than GlobalSetting.requiredRest, then drop the first SIP.
        /// </summary>
        /// <param name="line"></param>
        private void MinimumRestOverlapCorrrection(Line line)
        {
            int requiredRest = GlobalSettings.requiredRest;
            int LastLegArrivalTime = GlobalSettings.LastLegArrivalTime;

            //get the valid start date for new bid period
            //var date = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault().Date;

            DateTime date;
            var item = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault();
            if (item != null)
            {
                date = item.Date;
                var oldbidperiodenddate = date.AddDays(-1);


                //remove the droped sip from the siplist.
                List<LineSip> linesips = line.LineSips.Where(x => !x.Dropped).ToList();
                foreach (var sip in linesips)
                {
                    string pairing = line.Pairings.Where(x => x.Contains(sip.Sip.SipName.Substring(0, 4))).FirstOrDefault();
                    Trip trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                    //get the start leg departure time for the Sip
                    int startLegDepartureTime = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault().DepTime;
                    //startLegDepartureTime -= (GlobalSettings.CurrentBidDetails.Postion == "FA") ? GlobalSettings.FAshow1stDutyPeriod : GlobalSettings.show1stDay;
                    startLegDepartureTime -= (GlobalSettings.CurrentBidDetails.Postion == "FA") ? GlobalSettings.FAshow1stDutyPeriod : trip.BriefTime;
                    bool legal = false;

                    //if the sipstart date is  old bid period end date we have to check the Rest conflists.If it satisfy the condition,then the sip is legal.
                    if (oldbidperiodenddate == sip.SipStartDate)
                    {
                        if (LastLegArrivalTime + GlobalSettings.debrief + requiredRest < startLegDepartureTime)
                        {
                            legal = true;
                        }
                    }
                    else if ((sip.SipStartDate - oldbidperiodenddate).Days == 1)
                    {

                        if (LastLegArrivalTime + GlobalSettings.debrief + requiredRest < 1440 + startLegDepartureTime)
                        {
                            legal = true;
                        }
                    }
                    else
                    {
                        legal = true;
                    }
                    if (legal)
                    {
                        break;
                    }
                    else
                    {
                        DropoverlappedSip(sip, sip.SipStartDate);
                    }

                }
            }

        }

        /// <summary>
        /// 100 hours in a month.check the new bid period have totally 100 hours flight time.if not remove the lowest TFP SIP that will make the line legal for 100 hours and mark that LineSip as dropped.
        /// </summary>
        /// <param name="line"></param>
        private void OverlapMonthhoursCheck(Line line)
        {

            //order the sip list 
            var orderedlinesips = line.LineSips.OrderBy(x => x.SipFltHrs);

            DateTime startdate = new DateTime(int.Parse(year), int.Parse(month), 1);
            DateTime enddate = startdate.AddMonths(1).AddDays(-1);
            if (!CheckOverlaplegalityformonthlyhours(startdate, enddate))
            {
                foreach (var sip in orderedlinesips)
                {

                    //remove the sip (Drop the flight hours for the sip from the Sip opearating days)
                    DropoverlappedSip(sip, sip.SipStartDate);

                    bool legal = CheckOverlaplegalityformonthlyhours(startdate, enddate);

                    if (legal)
                    {
                        break;
                    }
                    else
                    {
                        //if it is not legal ,we have to check for the next lowest sip.
                        //previously removed sip doesnot not make the line legal,so we need to add the flight hours for the sip beacuse we removed flight hours to check the legality.
                        DateTime sipstartdate = sip.SipStartDate;
                        for (int count = 0; count < sip.Sip.SipDutyPeriods.Count; count++)
                        {

                            if (_combinedDays.Any(x => x.Date == sip.SipStartDate))
                            {

                                Day objday = _combinedDays.Where(x => x.Date == sipstartdate).FirstOrDefault();
                                if (objday != null)
                                {
                                    objday.FlightTime += sip.Sip.SipDutyPeriods[count].FlightHours;
                                    sipstartdate = sipstartdate.AddDays(1);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 24hoursRestCheck -Check each 7 days has atleat 24 hours off
        /// </summary>
        /// <param name="line"></param>
        private void overlap24hoursRestCheck(Line line)
        {


            // var date = new DateTime(int.Parse(year), int.Parse(month), 1);
            var date = bpStartDate;
            var day = (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Month == 2) ? 11 : 12;
            ///back ward iteration  (iterate upto the 12 th day of the new bid period for captain and 11 th day of the new bid period for FA)
            for (DateTime iterationdate = date; iterationdate < new DateTime(int.Parse(year), int.Parse(month), day); iterationdate = iterationdate.AddDays(1))
            {
                int loopcount = 0;
                while (true)
                {
                    ++loopcount;
                    bool legal = false;
                    DateTime startdate = iterationdate.AddDays(-6);

                    //check any day of exist within the 7 days.If exit we can break the loop and go to next iteration of 7 days
                    bool isdayoffexist = _combinedDays.Where(y => (startdate <= y.Date) && y.Date <= iterationdate).Any(x => x.FlightTime == 0);

                    if (!isdayoffexist)
                    {
                        //we need to check any "24 hours off" exist between the 7 days.
                        legal = CheckOverlaplegalityfor24houroff(line, iterationdate);
                        if (!legal)
                        {
                            //remove the droped sip from the siplist.
                            List<LineSip> linesips = line.LineSips.Where(x => !x.Dropped).ToList();

                            //get all sips between the 7 days .(I have to include the previous days sip if the previous sip days duty period includes this 7 days.So I used Sip.SipDutyPeriods.Count-1)
                            var sevendayssips = linesips.Where(x => (startdate <= x.SipStartDate.AddDays(x.Sip.SipDutyPeriods.Count - 1)) && x.SipStartDate <= iterationdate);

                            foreach (var sip in sevendayssips)
                            {
                                //remove the first sip in the sevendayssips list
                                DropoverlappedSip(sip, sip.SipStartDate);
                            }
                            if (loopcount == 7)
                                break;

                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //if 24 hours check is legal, we have to exit the while loop and go to next iteration for the date.
                        break;
                    }
                }

            }
        }

        /// <summary>
        /// Drop the sip  to correct the overlap
        /// </summary>
        /// <param name="sip"></param>
        /// <param name="sipstartdate"></param>
        private void DropoverlappedSip(LineSip sip, DateTime sipstartdate)
        {
            //we need to drop the sip to correct the overlap
            //save the dropflight time and drop status to the  the line

            for (int count = 0; count < sip.Sip.SipDutyPeriods.Count; count++)
            {

                if (_combinedDays.Any(x => x.Date == sipstartdate))
                {

                    Day objday = _combinedDays.Where(x => x.Date == sipstartdate).FirstOrDefault();
                    if (objday != null)
                    {
                        //subtract the appropriate flight hours for the day
                        objday.FlightTime -= sip.Sip.SipDutyPeriods[count].FlightHours;
                        sipstartdate = sipstartdate.AddDays(1);
                    }
                }
            }
            //set the drop status for this Sip to true to indcate that the sip is Droppped to correct the overlap.
            sip.Dropped = true;

        }

        /// <summary>
        /// check the legality for week hours(30 hours or toal 1800 minutes in 7 days)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endatate"></param>
        /// <returns></returns>
        private bool CheckOverlaplegalityforweekhours(DateTime startDate, DateTime enddate)
        {
            int flighttimesum = _combinedDays.Where(y => (startDate <= y.Date) && y.Date <= enddate).Sum(x => x.FlightTime);
            return (flighttimesum <= 1800);
        }

        /// <summary>
        /// check the legality for Monthly hours(100 hours or toal 6000 minutes in a calender month)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        private bool CheckOverlaplegalityformonthlyhours(DateTime startDate, DateTime enddate)
        {
            int flighttimesum = _combinedDays.Where(y => (startDate <= y.Date) && y.Date <= enddate).Sum(x => x.FlightTime);
            return (flighttimesum <= 6000);
        }

        /// <summary>
        /// check the legality for 24 hours off in 7 days.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="iterationdate"></param>
        /// <returns></returns>
        private bool CheckOverlaplegalityfor24houroff(Line line, DateTime iterationdate)
        {
            //get the valid start date for new bid period

            bool legal = false;
            //var validDate = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault().Date;
            DateTime validDate;
            var item = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault();
            if (item != null)
            {
                validDate = item.Date;

                int arriavaltime = 0;
                int count = 0;
                for (DateTime innerdate = iterationdate.AddDays(-6); innerdate <= iterationdate; innerdate = innerdate.AddDays(1))
                {
                    //for the first iterationdate  we have to intialise arriavaltime and it needs not tocheck the legality.Becuase it is a starting day  of the 7 consecucutive day
                    if (count == 0)
                    {
                        var obj = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate);
                        if (obj != null)
                        {
                            arriavaltime = obj.ArrivalTime;
                        }
                        count++;

                        continue;
                    }

                    if (innerdate < validDate)
                    {
                        //get the departure time
                        //int currentdeparturetime = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate).DepartutreTime;\
                        int currentdeparturetime = 0;
                        var deptitem = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate);
                        if (deptitem != null)
                            currentdeparturetime = deptitem.DepartutreTime;

                        //get the arrival time
                        //int currentarriavaltime = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate).ArrivalTime;
                        int currentarriavaltime = 0;
                        var arriveitem = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate);
                        if (arriveitem != null)
                            currentarriavaltime = arriveitem.ArrivalTime;

                        //if  previous day arrival time is less than current departure time,then it is legaly ok.(It have minimum 1440 minutes difference)
                        if (arriavaltime < currentdeparturetime)
                        {
                            legal = true;
                            break;
                        }
                        else
                        {
                            arriavaltime = currentarriavaltime;
                        }
                    }
                    else
                    {
                        //remove the droped sip from the siplist.
                        List<LineSip> linesips = line.LineSips.Where(x => !x.Dropped).ToList();

                        DateTime sipenddate = validDate;

                        foreach (var sip in linesips)
                        {

                            DateTime sipstartdate = sip.SipStartDate;

                            if (sipstartdate <= innerdate && innerdate <= sipstartdate.AddDays(sip.Sip.SipDutyPeriods.Count - 1))
                            {
                                //get the current sip departure time
                                //int currentsipDeparturetime = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault().DepTime;
                                int currentsipDeparturetime = 0;
                                var currentsipDepartureitem = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault();
                                if (currentsipDepartureitem != null)
                                    currentsipDeparturetime = currentsipDepartureitem.DepTime;
                                //get the current sip ariival time
                                var fligts = sip.Sip.SipDutyPeriods[sip.Sip.SipDutyPeriods.Count - 1].Flights;
                                int currentsiparrivaltime = fligts[fligts.Count - 1].ArrTime;

                                DateTime currentsipenddate = sipstartdate.AddDays(sip.Sip.SipDutyPeriods.Count - 1);
                                //if the previous sip end date is same as the current sip date ,that means a day has two sip) ,we dont need to consider this.becuase it violates the condition.
                                if (sipenddate == currentsipenddate)
                                {
                                    continue;
                                }
                                if (arriavaltime < currentsipDeparturetime)
                                {
                                    legal = true;
                                    break;
                                }
                                else
                                {
                                    arriavaltime = currentsiparrivaltime;
                                    sipenddate = currentsipenddate;
                                }

                            }
                        }
                        if (legal)
                        {
                            break;
                        }


                    }

                }
            }
            return legal;
        }

        /// <summary>
        /// Calculate the Sips in a Line
        /// </summary>
        private void CalculateLineSips()
        {
            Trip trip = null;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var line in lines)
            {
                paringCount = 0;
                foreach (var pairing in line.Value.Pairings)
                {
                    try
                    {
                        LineSip lineSip = new LineSip();
                        // todo: substring(0,4) below will fail on 2nd round -- fix this when able
                        trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                        isLastTrip = ((line.Value.Pairings.Count - 1) == paringCount);
                        paringCount++;
                        foreach (var siplist in trip.SipsList)
                        {
                            int day = int.Parse(pairing.Substring(4, 2).TrimStart());
                            DateTime date = WBidCollection.SetDate(day, isLastTrip, currentBidDetails);
                            lineSip = new LineSip();
                            lineSip.Sip = siplist;
                            lineSip.SipFltHrs = siplist.SipFltHrs;
                            lineSip.SipTfp = Math.Round(siplist.SipTfp, 2);
                            lineSip.SipStartDate = date.AddDays(siplist.SipStartDay);
                            line.Value.LineSips.Add(lineSip);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }
        }


        private List<BidLineTemplate> GenerateBidLineViewTemplate(Line line)
        {

            try
            {



                Trip trip = null;
                line.BidLineTemplates = GenerateBidLineTemplateCollection();
                line.Pairingdesription = string.Empty;
                List<string> lstpairingsdesc = new List<string>();
                DateTime wkDay;
                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {

                    trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                    int lengthTrip = trip.PairLength;
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    // wkDay = new DateTime(int.Parse(year), int.Parse(month), int.Parse(pairing.Substring(4, 2)));
                    wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);

                    for (int tripDay = 0; tripDay < lengthTrip; tripDay++)
                    {
                        BidLineTemplate bidLineTemplate = line.BidLineTemplates.Where(x => x.Date.Day == wkDay.AddDays(tripDay).Day
                                                                                      && x.Date.Month == wkDay.AddDays(tripDay).Month).FirstOrDefault();
                        if (tripDay == 0)
                        {
                            bidLineTemplate.TripNum = trip.TripNum.Substring(1, 3);
                            bidLineTemplate.TripNumDisplay = bidLineTemplate.TripNum;
                        }
                        bidLineTemplate.TripName = pairing;
                        bidLineTemplate.DutyPeriodNo = tripDay;

                        bidLineTemplate.ArrStaLastLeg = (trip.ReserveTrip) ? "R" : trip.DutyPeriods[tripDay].ArrStaLastLeg;
                        bidLineTemplate.ArrStaLastLegDisplay = bidLineTemplate.ArrStaLastLeg;
                        bidLineTemplate.AMPMType = trip.AmPm;
                    }

                    string deadHead = string.Empty;
                    if (trip.DutyPeriods.SelectMany(y => y.Flights).Any(z => z.DeadHead))
                    {
                        deadHead = "D";
                    }
                    string pairingdesc = pairing.Substring(1, 3).ToString() + "=" + deadHead + trip.DepTime + "/" + trip.RetTime + " (" + trip.Tfp.ToString("#.##") + ") ";
                    if (!lstpairingsdesc.Contains(pairingdesc))
                    {
                        lstpairingsdesc.Add(pairingdesc);
                    }

                }
                line.Pairingdesription = string.Join("", lstpairingsdesc.ToArray());
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
                DateTime start = currentBidDetails.BidPeriodStartDate;
                DateTime end = currentBidDetails.BidPeriodEndDate;

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


        private decimal CalcTfpInLine(Line line)
        {
            Trip trip = null;
            decimal tfp = 0m;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                //removed midoint rounding from WbidClient
                tfp += Math.Round(trip.Tfp, 2);
            }
            foreach (var s in line.LineSips)
            {
                if (s.Dropped)
                {
                    tfp -= s.SipTfp;
                }
            }

            return tfp; ;
        }

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

        /// <summary>
        /// Caculate Block hours in a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string CalcBlkHrsInLine(Line line)
        {
            Trip trip = null;
            int blkMinutes = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                blkMinutes += trip.Block;
            }
            foreach (var s in line.LineSips)
            {
                if (s.Dropped)
                {
                    blkMinutes -= s.SipFltHrs;
                }
            }
            return Helper.ConvertMinutesToHhhmm(blkMinutes);
        }

        /// <summary>
        /// PURPOSE : Calculate weekend properties of a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public string CalcWkEndProp(Line line, bool isFromExternalPage)
        {

            if (GlobalSettings.WBidINIContent.Week == null)
                return string.Empty;
            Trip trip = null;
            DateTime tripDate = DateTime.MinValue;
            int wkEndCount = 0, totDays = 0, tripDay = 0, tripLength = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            bool isLastTrip = false; int paringCount = 0;
            foreach (string pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                if (isFromExternalPage)
                    trip = GlobalSettings.Trip.Any(x => x.TripNum == pairing.Substring(0, 4)) ? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4)) : GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
                else
                    trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                tripDay = Convert.ToInt16(pairing.Substring(4, 2));
                tripLength = trip.PairLength;
                tripDate = DateTime.MinValue;
                dayOfWeek = 0;
                for (int index = 0; index < tripLength; index++)
                {
                    // tripDate = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), tripDay);
                    tripDate = WBidCollection.SetDate(tripDay, isLastTrip, currentBidDetails);
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

        public string CalcAmPmProp(Line line, bool isFromExternalPage)
        {
            if (line.BlankLine) return "blankLine";

            AmPmConfigure amPmConfigure;

            if (GlobalSettings.WBidINIContent.AmPmConfigure == null)
                return string.Empty;
            amPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
            // initialize
            string ampm = "AM";
            Trip trip = null;
            int howCalc = amPmConfigure.HowCalcAmPm;
            int amPush = Convert.ToInt32(amPmConfigure.AmPush.TotalMinutes);
            int amLand = Convert.ToInt32(amPmConfigure.AmLand.TotalMinutes);
            int pmPush = Convert.ToInt32(amPmConfigure.PmPush.TotalMinutes);
            int pmLand = Convert.ToInt32(amPmConfigure.PmLand.TotalMinutes);
            pmLand = pmLand < amLand ? pmLand + 1440 : pmLand;
            int ntePush = Convert.ToInt32(amPmConfigure.NitePush.TotalMinutes);
            ntePush = ntePush < pmPush ? ntePush + 1440 : ntePush;
            int nteLand = Convert.ToInt32(amPmConfigure.NiteLand.TotalMinutes);
            nteLand = nteLand + 1440;
            int amCentroid = (amPush + amLand) / 2;
            int pmCentroid = (pmPush + pmLand) / 2;
            int nteCentroid = (ntePush + nteLand) / 2;
            int numOrPct = amPmConfigure.NumberOrPercentageCalc;
            int numDiff = amPmConfigure.NumOpposites;
            decimal pctDiff = amPmConfigure.PctOpposities / 100m;
            int amTermCnt, amPushCnt, pmTermCnt, pmPushCnt, nteTermCnt, ntePushCnt, unknownTerm, unknownPush, amCentCnt, pmCentCnt, nteCentCnt, case2AmCnt, case2PmCnt, case2NteCnt, case2MixCnt;
            amTermCnt = amPushCnt = pmTermCnt = pmPushCnt = nteTermCnt = ntePushCnt = unknownTerm = unknownPush = amCentCnt = pmCentCnt = nteCentCnt = case2AmCnt = case2PmCnt = case2NteCnt = case2MixCnt = 0;



            foreach (var pairing in line.Pairings)
            {
                if (isFromExternalPage)
                    trip = GlobalSettings.Trip.Any(x => x.TripNum == pairing.Substring(0, 4)) ? GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4)) : GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
                else
                    trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "1";
                        }
                        else
                        {
                            pmTermCnt++;
                            case2PmCnt++;
                            pmCentCnt++;
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA") trip.AmPm = "2";
                        }
                    }
                    else
                    {
                        int landMinutes = dp.Flights[dp.Flights.Count - 1].ArrTime - dpCount * 1440;

                        if (landMinutes < amLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "1";
                            amTermCnt++;
                        }
                        else if (landMinutes < pmLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                            pmTermCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 1) trip.AmPm = "2";
                            nteTermCnt++;
                        }

                        int pushMinutes = dp.Flights[0].DepTime - dpCount * 1440;

                        if (pushMinutes > ntePush) ntePushCnt++;
                        else if (pushMinutes > pmPush) pmPushCnt++;
                        else amPushCnt++;

                        if (pushMinutes > amPush && landMinutes < amLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "1";
                            case2AmCnt++;
                        }
                        else if (pushMinutes > pmPush && landMinutes < pmLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2PmCnt++;
                        }
                        else if (pushMinutes > ntePush && landMinutes < nteLand)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2NteCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 2) trip.AmPm = "2";
                            case2MixCnt++;
                        }

                        int centroid = (pushMinutes + landMinutes) / 2;

                        if (centroid < amCentroid)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                            amCentCnt++;
                        }
                        else if (centroid < pmCentroid)
                        {
                            if (centroid - amCentroid < pmCentroid - centroid)
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "1";
                                amCentCnt++;
                            }
                            else
                            {
                                if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                                pmCentCnt++;
                            }
                        }
                        else if (centroid - pmCentroid < nteCentroid - centroid)
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "2";
                            pmCentCnt++;
                        }
                        else
                        {
                            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && howCalc == 3) trip.AmPm = "3";
                            nteCentCnt++;
                        }

                        dpCount++;

                    }

                }
                if (trip.AmPm == null || trip.AmPm == string.Empty)
                {
                }

            }

            int totalTerm = amTermCnt + pmTermCnt + nteTermCnt;
            int totalCentCnt = amCentCnt + pmCentCnt + nteCentCnt;
            int totalCase2Cnt = case2AmCnt + case2PmCnt + case2NteCnt + case2MixCnt;

            //if (line.LineNum == 40)
            //{
            //    int temp = 0;
            //}

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
                            if (1 - ((totalTerm != 0) ? (amTermCnt / (decimal)totalTerm) : 0) < pctDiff) return "AM";
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



        /// <summary>
        /// Calculate Days Off property
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int CalcDaysOff(string line)
        {
            Trip trip = null;
            // additionally, flt attendants have a different bid period for Jan and Feb (Jan is 1-30) (Feb is Jan31 - 1 Mar) (Mar is Mar2 - Mar31)
            int daysOff = System.DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month));
            if (position == "FA")
                if (month == "01" || month == "1")
                    daysOff = 30;
                else if (month == "02" || month == "2")
                    daysOff = System.DateTime.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)) + 2;
                else if (month == "03" || month == "3")
                    daysOff = 30;

            // now iterate through pairings and subtract days of work from days off
            DateTime tripStartDate = DateTime.MinValue;
            int daysInOverlap = 0;
            bool isLastTrip = false;
            foreach (var pairing in lines[line].Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                isLastTrip = (lines[line].Pairings.LastOrDefault() == pairing);
                tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);

                if (tripStartDate.AddDays(trip.PairLength - 1) > bpEndDate)
                {
                    daysInOverlap = tripStartDate.AddDays(trip.PairLength - 1).Subtract(bpEndDate).Days;

                }
                daysOff -= trip.PairLength;
            }

            if (daysInOverlap > 0) daysOff += daysInOverlap;
            return daysOff;
        }
        /// <summary>
        /// calculate Work days in a line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int CalcDaysWorkInLine(Line line)
        {
            Trip trip = null;
            int daysWorkInLine = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                daysWorkInLine += trip.PairLength;
            }
            return daysWorkInLine;
        }

        /// <summary>
        /// calculate T234 propety
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string CalcT234(string line)
        {
            Trip trip = null;
            string T234 = "";
            int turns, day2, day3, day4;
            turns = day2 = day3 = day4 = 0;

            foreach (var pairing in lines[line].Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                switch (trip.PairLength)
                {
                    case 1:
                        turns++;
                        break;
                    case 2:
                        day2++;
                        break;
                    case 3:
                        day3++;
                        break;
                    case 4:
                        day4++;
                        break;
                    default:
                        break;
                }
            }

            T234 = turns > 9 ? "*" : turns.ToString();
            T234 += day2 > 9 ? "*" : day2.ToString();
            T234 += day3 > 9 ? "*" : day3.ToString();
            T234 += day4 > 9 ? "*" : day4.ToString();
            return T234;
        }

        private decimal CalcTfpPerFltHr(Line line)
        {
            if (line.ReserveLine || line.BlankLine) return 0.00m;

            decimal tfp;
            // line.block is an int stored as hhmm, except FA could be hhhmm
            if (line.Block.ToString().Length == 5)
                tfp = line.Tfp / (Convert.ToDecimal(line.Block.ToString().Substring(0, 3)) +
                  Convert.ToDecimal(line.Block.ToString().Substring(3, 2)) / 60m);
            else
                tfp = line.Tfp / (Convert.ToDecimal(line.Block.ToString().Substring(0, 2)) +
                    Convert.ToDecimal(line.Block.ToString().PadLeft(4, '0').Substring(2, 2)) / 60m);

            return Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", tfp)), 2);
        }

        private int CalcDutyHrs(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;

            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                int date = Convert.ToInt16(pairing.Substring(4, 2));
                DateTime tripstartdate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);
                foreach (var dp in trip.DutyPeriods)
                {
                    if (!inLine)        // if inLine is false then only dutyHrs in Bid Period will be accumulated
                    {
                        if (tripstartdate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                        {
                            dutyHrs += dp.DutyTime;
                        }
                    }
                    else
                        dutyHrs += dp.DutyTime;

                    tripstartdate = tripstartdate.AddDays(1);
                }
            }

            return dutyHrs;
        }
        private string CalcTafb(Line line, bool inLine)
        {
            Trip trip = null;
            int tafb = 0;

            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                DateTime tripstartdate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);

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

        private int CalculateTafbInBpOfDutyPeriod(DutyPeriod dutyPeriod, int lengthOfTrip)
        {

            int dutyPeriodTafb = 0;
            if (dutyPeriod.DutPerSeqNum < lengthOfTrip)
            {
                if (dutyPeriod.DutPerSeqNum == 1)
                    // first day of trip and trip is more than 1 day
                    dutyPeriodTafb += 24 * 60 - (dutyPeriod.ShowTime);
                else
                    // not first or last day of trip, so there are 24 hours of Tafb
                    dutyPeriodTafb += 24 * 60;
            }
            else if (lengthOfTrip > 1)
                // last day of multi day trip
                dutyPeriodTafb += dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.debrief;
            else
                // trip is a 1 day trip
                dutyPeriodTafb += (dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1))) - (dutyPeriod.ShowTime - (1440 * (dutyPeriod.DutPerSeqNum - 1))) + GlobalSettings.debrief;

            return dutyPeriodTafb;
        }

        private int CalcNumLegsOfGivenType(int givenType, Line line)
        {
            Trip trip = null;
            int numLegsOfGivenType = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                string tripName = pairing.Substring(0, 4);
                if (!trip.ReserveTrip)
                {
                    foreach (var dp in trip.DutyPeriods)
                        foreach (var flt in dp.Flights)
                            if (flt.Equip != null)
                            {
                                if (flt.Equip.Length > 0)
                                {
                                    if ((GlobalSettings.CurrentBidDetails.Postion == "FA" ? flt.Equip.Substring(0, 1) : flt.Equip.Substring(1, 1)) == givenType.ToString().Substring(0, 1))
                                        numLegsOfGivenType++;
                                }
                            }
                }
            }

            return numLegsOfGivenType;
        }

        private TimeSpan CalcLongGrndTime(Line line)
        {
            Trip trip = null;
            int maxGrndTime = 0;
            int turnTime = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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

        private int CalcNumTripsOfGivenLength(int givenLength, Line line)
        {
            Trip trip = null;
            int numTripsOfGivenLength = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                if (trip.PairLength == givenLength)
                    numTripsOfGivenLength++;
            }

            return numTripsOfGivenLength;
        }

        private int CalcNumAcftChanges(Line line)
        {
            Trip trip = null;
            int numAcftChanges = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                foreach (var dp in trip.DutyPeriods)
                    foreach (var flt in dp.Flights)
                        if (flt.AcftChange == true)
                            numAcftChanges++;
            }

            return numAcftChanges;
        }

        private void CalcCarryOver(Line line)
        {
            Trip trip = null;
            decimal totCarryOverTfp = 0m;
            decimal totCarryOverBlock = 0m;
            line.CarryOverTfp = 0m;
            line.CarryOverBlock = 0m;

            if (line.Pairings.Count == 0) return;  // handles blank line case

            var pairing = line.Pairings[line.Pairings.Count - 1];  // only the last pairing can have overlap
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                int dateOfTrip = Convert.ToInt16(pairing.Substring(4, 2));
                int tripLength = trip.TotDutPer;
                int daysInMonth = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Subtract(GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
                if (dateOfTrip + tripLength - 1 > daysInMonth)  // this occurs if the trip has overlap
                {
                    for (int i = 1; i <= tripLength; i++)
                    {
                        if (dateOfTrip + i > daysInMonth)
                        {
                            for (int p = i; p < tripLength; p++)
                            {
                                totCarryOverTfp += trip.DutyPeriods[p].Tfp;
                                totCarryOverBlock += trip.DutyPeriods[p].Block;
                            }
                            line.CarryOverTfp = Math.Round(totCarryOverTfp, 2);
                            line.CarryOverBlock = Math.Round(totCarryOverBlock, 2);
                        }
                    }
                }
            }
        }

        private int CalcTotDutPds(Line line, bool inLine)
        {
            Trip trip = null;
            int totDutPds = 0;
            DateTime tripDate = DateTime.MinValue;

            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];

                if (inLine)
                {
                    totDutPds += trip.DutyPeriods.Count();
                }
                else
                {
                    tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);
                    if (tripDate.AddDays(trip.PairLength - 1) > bpEndDate)
                    {
                        totDutPds += bpEndDate.Subtract(tripDate).Days + 1;
                    }
                    else
                    {
                        totDutPds += trip.DutyPeriods.Count();
                    }
                }
            }
            return totDutPds;
        }

        private TimeSpan CalcEPush(Line line)
        {
            Trip trip = null;
            int ePush = 99999999;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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

        private TimeSpan CalcEDomPush(Line line)
        {
            Trip trip = null;
            int ePush = 99999999;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
            }

            int hours = ePush / 60;
            int minutes = ePush % 60;
            return new TimeSpan(hours, minutes, 0);

        }

        private TimeSpan CalcLastArrTime(Line line)
        {
            Trip trip = null;
            int arrTime = 0;
            int dutPd = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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

        private TimeSpan CalcLastDomArrTime(Line line)
        {
            Trip trip = null;
            int lastDomArr = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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

        private int CalcTotLegs(Line line)
        {
            Trip trip = null;
            int totLegs = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
                if (trip == null)
                {
                    trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
                }
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                foreach (var dp in trip.DutyPeriods)
                {
                    foreach (var flt in dp.Flights)
                        totLegs++;
                }
            }

            return totLegs;
        }

        private int CalcSips(Line line)
        {
            Trip trip = null;
            int sips = 0;
            if (line.ReserveLine || line.BlankLine) return sips;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                foreach (var dp in trip.DutyPeriods)
                    for (int i = 0; i < dp.Flights.Count() - 1; i++)
                    {
                        if (dp.Flights[i].ArrSta == domicile) sips++;
                    }
            }
            return sips;
        }

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
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                int date = Convert.ToInt16(pairing.Substring(4, 2));
                int lenghtOfTrip = trip.PairLength;
                if (date != nextDate)
                {
                    nextDate = date + lenghtOfTrip;
                    startDowInt = Convert.ToInt32(WBidCollection.SetDate(date, isLastTrip, currentBidDetails).DayOfWeek);
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

        /// <summary>
        /// PURPOSE : Calculate Tfp Per Day
        /// </summary>
        /// <param name="line"></param>
        private void CalculateTfpPerDay(Line line)
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
                tfpPerDay = Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", (line.Tfp / line.DaysWork))), 2);
            }

            line.TfpPerDay = tfpPerDay;
        }

        private int CalcLargestBlkDaysOff(Line line)
        {
            if (line.BlankLine) return 0;
            Trip trip = null;
            int oldBlkDaysOff, newBlkDaysOff;
            oldBlkDaysOff = newBlkDaysOff = 0;
            DateTime oldPairingEndDay = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.AddDays(-1);
            DateTime tripStartDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;

            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1 == paringCount)); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);

                if (tripStartDate.Subtract(oldPairingEndDay).Days != 1)
                {
                    newBlkDaysOff = tripStartDate.Subtract(oldPairingEndDay).Days - 1;

                }
                oldPairingEndDay = tripStartDate.AddDays(trip.PairLength - 1);
                oldBlkDaysOff = newBlkDaysOff > oldBlkDaysOff ? newBlkDaysOff : oldBlkDaysOff;
            }

            if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate > oldPairingEndDay)
            {
                newBlkDaysOff = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Subtract(oldPairingEndDay).Days;
                oldBlkDaysOff = newBlkDaysOff > oldBlkDaysOff ? newBlkDaysOff : oldBlkDaysOff;
            }
            return oldBlkDaysOff;
        }

        private List<int> CalcBlkOfDaysOff(Line line)
        {
            Trip trip = null;

            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            if (line.BlankLine) return blkOff;
            DateTime oldPairingStartDay, oldPairingEndDay, newPairingStartDay, newPairingEndDay;
            oldPairingStartDay = oldPairingEndDay = newPairingStartDay = newPairingEndDay = DateTime.MinValue;
            oldPairingEndDay = bpStartDate.AddDays(-1);
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                newPairingStartDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);
                newPairingEndDay = newPairingStartDay.AddDays(trip.PairLength - 1);

                if ((newPairingStartDay - oldPairingEndDay).Days != 1)
                {
                    blkOff[(newPairingStartDay - oldPairingEndDay).Days - 1]++;
                }

                oldPairingStartDay = newPairingStartDay;
                oldPairingEndDay = newPairingEndDay;
            }

            // handle end of month case:  fa bpEnd can be 30, 1, 31
            DateTime lastPairEndDate = WBidCollection.SetDate(oldPairingStartDay.Day, true, currentBidDetails);
            lastPairEndDate = lastPairEndDate.AddDays(trip.PairLength - 1);
            TimeSpan tempSpan = bpEndDate - lastPairEndDate;
            if (tempSpan.Days > 0) blkOff[tempSpan.Days]++;

            return blkOff;
        }

        /// <summary>
        /// PURPOSe : Cclculate Legs/Flights per Duty period
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<int> CalcLegsPerDutyPeriod(Line line)
        {
            Trip trip = null;
            List<int> arrayOfDeadheads = new List<int>();
            for (int count = 0; count < 10; count++)
                arrayOfDeadheads.Add(0);
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                //string tripName = pairing.Substring(0, 4);
                foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                {
                    arrayOfDeadheads[dutyPeriod.TotFlights]++;
                }
            }
            return arrayOfDeadheads;
        }

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
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                int lengthTrip = trip.PairLength;
                wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);
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

        private List<WorkDaysOfBidLine> CalcDaysOfMonthOff(Line line)
        {
            // todo: note for frank: this has problems with FA, the months Feb has two 1 days (1 Feb and 1 Mar).  Pairing BA1D01 (Feb) is not distinct from BALT01 (Mar)
            // may need to add DateTime property to trip.cs to store actual operating date of trip
            Trip trip = null;
            List<WorkDaysOfBidLine> daysWork = new List<WorkDaysOfBidLine>();
            WorkDaysOfBidLine workDay;
            DateTime wkDay;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                int lengthTrip = trip.PairLength;

                //wkDay = new DateTime(int.Parse(year), int.Parse(month), int.Parse(pairing.Substring(4, 2)));
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);
                workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };

                daysWork.Add(workDay);

                for (int i = 1; i < lengthTrip; i++)
                {
                    wkDay = wkDay.AddDays(1);
                    workDay = new WorkDaysOfBidLine() { DayOfBidline = wkDay, Working = true };
                    daysWork.Add(workDay);
                }
            }

            return daysWork;
        }

        private void CalculateOvernightCities(Line line)
        {
            Trip trip = null;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];

                foreach (var dp in trip.DutyPeriods)
                {
                    if (dp.DutPerSeqNum < trip.DutyPeriods.Count)
                    {
                        // only adds overnights and not the last day of trip
                        line.OvernightCities.Add(dp.ArrStaLastLeg);
                    }
                }
            }
        }

        // <summary>
        /// PURPOSE : Calculate Rest Periods
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private void CalculateRestPeriods(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();
            Trip trip = null;
            int periodid = 1;
            RestPeriod restPeriod = null;
            int count = 0;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = new DateTime();
            lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {

                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                DateTime tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip, currentBidDetails);
                count = 0;
                int rls = GlobalSettings.debrief;
                foreach (var dp in trip.DutyPeriods)
                {
                    //if not the first dutyperiod then we need to add one  to the tripstartdate
                    if (count != 0)
                    {
                        tripStartDate = tripStartDate.AddDays(1);
                    }

                    restPeriod = new RestPeriod();
                    restPeriod.PeriodId = periodid++;
                    restPeriod.IsInTrip = IsInTrip;
                    restPeriod.RestMinutes = CalculateTimeDifference(lastDutyEndDate, tripStartDate.AddMinutes(dp.ShowTime - (count * 1440)));
                    lstRestPeriod.Add(restPeriod);

                    //Finding the status ,is in trip or 'between trip'
                    IsInTrip = (dp.ArrStaLastLeg != GlobalSettings.CurrentBidDetails.Domicile);
                    lastDutyEndDate = tripStartDate.AddMinutes(dp.LandTimeLastLeg - (count * 1440) + rls);
                    count++;
                }
            }

            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }

            line.RestPeriods = lstRestPeriod;
        }
        private int CalculateTimeDifference(DateTime firstDateTime, DateTime secondDateTime)
        {
            return int.Parse(secondDateTime.Subtract(firstDateTime).TotalMinutes.ToString());
        }

        /// <summary>
        /// PURPOSe : Calculate WorkBlock Length
        /// </summary>
        /// <param name="line"></param>
        /// <param name="inLine"></param>
        private void CalculateWorkBlockLength(Line line)
        {

            Trip trip = null;
            DateTime tripStartDate, tripEndDate, tripPreviousEndDate;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            int workBlockLength = 0;
            tripPreviousEndDate = DateTime.MinValue;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                //tripStartDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(pairing.Substring(4, 2).Trim(' ')));
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip, currentBidDetails);

                if (workBlockLength != 0 && tripPreviousEndDate.AddDays(1) != tripStartDate)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }

                tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                tripPreviousEndDate = tripEndDate;
                workBlockLength += trip.DutyPeriods.Count;
            }

            line.WorkBlockLengths[workBlockLength]++;
        }

        /// <summary>
        /// PURPOSe : calculate commutable Dead heads
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<DeadheadCity> CalcDeadheadCities(Line line)
        {

            //Commutable deadheads only be calculated the begining or end of Work blocks
            Trip trip = null;
            List<DeadheadCity> lstDeadhedaCity = new List<DeadheadCity>();
            DateTime tripStartDate, tripEndDate, tripPreviousEndDate;
            string begingDeadHeadCityname = string.Empty;
            string endDeadHeadCityname = string.Empty;
            tripPreviousEndDate = DateTime.MinValue;

            if (line.LineNum == 264)
            {
            }

            int tripCount = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip, currentBidDetails);

                //First trip
                if (tripCount == 0)
                {
                    tripPreviousEndDate = tripStartDate.AddDays(-2);
                    if (trip.DutyPeriods[0].Flights[0].DeadHead)
                    {
                        begingDeadHeadCityname = trip.DutyPeriods[0].Flights[0].ArrSta;
                    }
                    if (begingDeadHeadCityname != string.Empty)
                    {
                        AddDeadHeadCity(lstDeadhedaCity, begingDeadHeadCityname, true);
                        begingDeadHeadCityname = string.Empty;
                    }
                }
                else
                {
                    if (endDeadHeadCityname != string.Empty && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        AddDeadHeadCity(lstDeadhedaCity, endDeadHeadCityname, false);
                        endDeadHeadCityname = string.Empty;
                    }
                    if (trip.DutyPeriods[0].Flights[0].DeadHead && tripPreviousEndDate.AddDays(1) != tripStartDate)
                    {
                        begingDeadHeadCityname = trip.DutyPeriods[0].Flights[0].ArrSta;
                        AddDeadHeadCity(lstDeadhedaCity, begingDeadHeadCityname, true);
                        begingDeadHeadCityname = string.Empty;
                    }
                }

                if (trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1].DeadHead)
                {
                    endDeadHeadCityname = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1].DepSta;
                }
                ///last trip-- last flight dead head
                if (tripCount == line.Pairings.Count - 1 && endDeadHeadCityname != string.Empty)
                {
                    AddDeadHeadCity(lstDeadhedaCity, endDeadHeadCityname, false);
                    endDeadHeadCityname = string.Empty;
                }


                tripCount++;
                tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                tripPreviousEndDate = tripEndDate;
            }
            return lstDeadhedaCity;
        }

        private void AddDeadHeadCity(List<DeadheadCity> lstDeadhedaCity, string cityName, bool isCountFrom)
        {
            DeadheadCity deadheadCity = lstDeadhedaCity.FirstOrDefault(x => x.City == cityName);
            if (deadheadCity == null)
            {
                //Getting city details from INI file
                City city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == cityName);
                if (city != null)
                {
                    deadheadCity = new DeadheadCity() { City = city.Name, CityId = city.Id };
                    //Add to main list 
                    lstDeadhedaCity.Add(deadheadCity);
                }
            }
            if (deadheadCity != null)
            {
                if (isCountFrom)
                    deadheadCity.CountFrom++;
                else
                    deadheadCity.CountTo++;

            }

        }

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
        //    tripPreviousEndDate = DateTime.MinValue;
        //    int tripPreviousLandTime = 0;
        //    int count = 0;
        //    int backToBack = 0;
        //    bool isLastTrip = false; int paringCount = 0;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
        //        trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
        //        tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip, currentBidDetails);
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
        //                //GlobalSettings.showAfter1stDay;
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


        //private void CalculatePairingWorkDetails(Line line)
        //{

        //    if (line.BlankLine) return;
        //    List<PairingWorkDetails> lstPairingWorkDetails = new List<PairingWorkDetails>();
        //    PairingWorkDetails pairingworkDetails = null;
        //    Trip trip = null;
        //    DateTime tripStartDate;

        //    int count = 0;
        //    int backToBack = 0;
        //    bool isLastTrip = false; int paringCount = 0;
        //    foreach (var pairing in line.Pairings)
        //    {
        //        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
        //        trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
        //        tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip, currentBidDetails);

        //        pairingworkDetails = new PairingWorkDetails();
        //        pairingworkDetails.StartDay = (int)tripStartDate.DayOfWeek;
        //        pairingworkDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - GlobalSettings.showAfter1stDay;
        //        pairingworkDetails.EndTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
        //        pairingworkDetails.StartDay = (int)tripStartDate.AddDays(trip.DutyPeriods.Count - 1).DayOfWeek;

        //        lstPairingWorkDetails.Add(pairingworkDetails);
        //    }
        //    line.PairingWorkDetail = lstPairingWorkDetails;

        //}

        /// <summary>
        /// PURPOSE : Calculate work block details, count backToback (btb) events for each work block
        /// </summary>
        /// <param name="line"></param>
        private void CalculateWorkBlockDetails(Line line)
        {

            if (line.BlankLine) return;
            List<WorkBlockDetails> lstWorkBlockDetails = new List<WorkBlockDetails>();
            WorkBlockDetails workBlockDetails = new WorkBlockDetails();
            Trip trip = null;
            DateTime tripStartDate, tripEndDate, tripPreviousEndDate;

            tripPreviousEndDate = DateTime.MinValue;
            int tripPreviousLandTime = 0;
            int tripPreviousStartTime = 0;
            int count = 0;

            int backToBack = 0;
            bool isLastTrip = false; int paringCount = 0;
            int nightinMid = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                //                tripStartDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(pairing.Substring(4, 2).Trim(' ')));
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                if (count == 0)
                {
                    // int showfirstday = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).Any(y => y.Name == trip.DutyPeriods[0].Flights[0].ArrSta) ? GlobalSettings.show1stDayInternational : GlobalSettings.show1stDay;

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

                    // further modified by frank to accomodate irregular datetime structure from the company
                    workBlockDetails.EndDate = tripPreviousEndDate.Date;
                    if (tripPreviousStartTime > tripPreviousLandTime)
                    {
                        // if the trip ends on the next day, we have to check the full days to check the trip is commutable.
                        workBlockDetails.EndDateTime = tripPreviousEndDate.AddMinutes(1439);
                    }
                    else
                    {
                        workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
                    }

                    //int dpcount=1;
                    //foreach (DutyPeriod dp in trip.DutyPeriods)
                    //{

                    //    if (trip.DutyPeriods.Count!=dpcount && dp.Flights[dp.Flights.Count - 1].ArrSta == GlobalSettings.CurrentBidDetails.Domicile)
                    //        nightinMid++;
                    //    dpcount++;
                    //}
                    workBlockDetails.nightINDomicile = nightinMid;
                    nightinMid = 0;
                    workBlockDetails.BackToBackCount = backToBack - 1;
                    lstWorkBlockDetails.Add(workBlockDetails);

                    workBlockDetails = new WorkBlockDetails();
                    //int showfirstday = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).Any(y => y.Name == trip.DutyPeriods[0].Flights[0].ArrSta) ? GlobalSettings.show1stDayInternational : GlobalSettings.show1stDay;
                    if (trip.ReserveTrip)
                    {
                        //workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut - trip.BriefTime; ;
                        //modified on sep 21 2018, bause the start time coming as previous value.
                        workBlockDetails.StartTime = trip.DutyPeriods[0].ReserveOut - GlobalSettings.ReserveBriefTime;
                        workBlockDetails.BriefTime = GlobalSettings.ReserveBriefTime;
                    }
                    else
                    {
                        workBlockDetails.StartTime = trip.DutyPeriods[0].DepTimeFirstLeg - trip.BriefTime; 
                        workBlockDetails.BriefTime = trip.BriefTime;
                    }
                    //trip.BriefTime;
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
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].ReserveIn - (1440 * (trip.DutyPeriods.Count - 1)) + GlobalSettings.ReserveDebrief;
                }
                else
                {
                    tripPreviousStartTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[0].DepTime - trip.BriefTime - (1440 * (trip.DutyPeriods.Count - 1));
                    tripPreviousLandTime = trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg - (1440 * (trip.DutyPeriods.Count - 1));
                }
                tripPreviousEndDate = tripEndDate;
                count++;
                backToBack++;

                if (line.LineNum == 205)
                {
                    int i;
                    i = 1;
                }
            }

            workBlockDetails.EndTime = tripPreviousLandTime;
            workBlockDetails.EndDay = (int)tripPreviousEndDate.DayOfWeek;
            //Modified as part of BA Filter
            workBlockDetails.EndDateTime = tripPreviousEndDate.Date.AddMinutes(workBlockDetails.EndTime);
            // Further modified by Frank to account for irregular datetimes from the company
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
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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
        private void CalculateDhFirstandDeadHeadDhLast(Line line)
        {
            Trip trip = null;
            int DhfirstCount = 0;
            int DhLastCount = 0;
            foreach (string pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                if (trip.DhFirst)
                    DhfirstCount++;
                if (trip.DhLast)
                    DhLastCount++;
            }
            line.DhFirstTotal = DhfirstCount;
            line.DhLastTotal = DhLastCount;
        }

        private void CalculateDhTotal(Line line)
        {
            Trip trip = null;
            int DhTotalCount = 0;

            foreach (string pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                foreach (var dp in trip.DutyPeriods)
                {
                    foreach (var flt in dp.Flights)
                    {
                        if (flt.DeadHead)
                            DhTotalCount++;
                    }
                }
            }
            line.DhTotal = DhTotalCount;
        }
        /// <summary>
        /// PURPOSE : All Days in Bid Period -- contains off, city
        /// </summary>
        /// <param name="line"></param>
        /// <returns>List<Day></Day></returns>
        private List<Day> CalculateDayStatusInBidPeriod(Line line)
        {
            Trip trip = null;
            Day day = null;
            List<Day> days = new List<Day>();
            double totalDays = (bpEndDate - bpStartDate).TotalDays + 5;
            DateTime wkDay;
            wkDay = bpStartDate;
            // initializes list to all days off and in Domicile
            for (double count = 0; count < totalDays; count++)
            {
                days.Add(new Day() { OffDuty = true, ArrivalCity = GlobalSettings.CurrentBidDetails.Domicile, DepartutreCity = GlobalSettings.CurrentBidDetails.Domicile, Date = wkDay });
                wkDay = wkDay.AddDays(1);
            }
            bool isLastTrip = false; int paringCount = 0;
            // now sets each day that works with data for city, start work, stop work, off
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                int lengthTrip = trip.PairLength;
                wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip, currentBidDetails);
                for (int count = 0; count < lengthTrip; count++)
                {
                    day = days.FirstOrDefault(x => x.Date == wkDay);
                    day.OffDuty = false;
                    day.ArrivalCity = trip.DutyPeriods[count].ArrStaLastLeg;
                    day.DepartutreCity = trip.DutyPeriods[count].Flights[0].DepSta;
                    day.DepartutreTime = trip.DutyPeriods[count].DepTimeFirstLeg - ((trip.DutyPeriods[count].DutPerSeqNum - 1) * 1440);
                    day.ArrivalTime = trip.DutyPeriods[count].LandTimeLastLeg - ((trip.DutyPeriods[count].DutPerSeqNum - 1) * 1440); ;
                    wkDay = wkDay.AddDays(1);
                }
            }
            return days;
        }

        /// <summary>
        /// Calculate Most legs in a pairing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int CalculateMostlegs(Line line)
        {
            if (line.ReserveLine || line.BlankLine)
                return 0;
            Trip trip = null;
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                int legsintrip = trip.DutyPeriods.Sum(x => x.Flights.Count);
                if (mostlegs < legsintrip)
                {
                    mostlegs = legsintrip;
                }
            }
            return mostlegs;
        }

        /// <summary>
        /// create a list collection contains duty period hours for each duty period.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<int> CalcDutyPeriodHours(Line line)
        {
            Trip trip = null;
            List<int> dutyperiodHours = new List<int>();
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                foreach (var dp in trip.DutyPeriods)
                {
                    dutyperiodHours.Add(dp.DutyTime);
                }
            }
            return dutyperiodHours;
        }

        /// <summary>
        /// Calulcate 3on3OffDay property.Property set  if line satisfied the strict pattern of 3 days of work followed by 3 days of OFF
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool CalculateIs3on3OffDay(Line line)
        {
            bool Is3on3off = false;
            Trip trip = null;
            //if the line has 
            if (line.Trips1Day == 0 && line.Trips2Day == 0 && line.Trips4Day == 0)
            {
                int count = 0;
                DateTime previoustripendday = new DateTime();
                bool isLastTrip = false; int paringCount = 0;
                foreach (var pairing in line.Pairings)
                {
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                    //DateTime tripstartdate = new DateTime(int.Parse(year), int.Parse(month), Convert.ToInt16(pairing.Substring(4, 2)));
                    DateTime tripstartdate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    DateTime tripenddate = tripstartdate.AddDays(trip.DutyPeriods.Count - 1);
                    //exclude rest period before the first trip 
                    if (count != 0)
                    {
                        double rest = (tripstartdate - previoustripendday.AddDays(1)).TotalDays;
                        //check the rest period between the currrent trip and previous trip is  exactly 3 days.
                        if (rest != 3)
                        {
                            break;
                        }
                    }
                    previoustripendday = tripenddate;
                    count++;
                }
                if (count == line.Pairings.Count)
                {
                    Is3on3off = true;
                }
            }


            return Is3on3off;
        }

        /// <summary>
        /// Calculate All the turn time for the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<int> CalculateGroundTimes(Line line)
        {
            List<int> groundtimes = new List<int>();
            Trip trip = null;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                foreach (DutyPeriod dutyperiod in trip.DutyPeriods)
                {
                    int count = 0;
                    int previousflightarrivaltime = 0;
                    foreach (Flight flight in dutyperiod.Flights)
                    {
                        int currentflightdeparture = flight.DepTime;
                        int currentflightarrival = flight.ArrTime;
                        if (count != 0)
                        {
                            int turntime = currentflightdeparture - previousflightarrivaltime;
                            groundtimes.Add(turntime);
                        }
                        previousflightarrivaltime = currentflightarrival;
                        count++;
                    }
                }
            }
            return groundtimes;
        }
        private bool CalculateETOPSProperties(Line line)
        {
            Trip trip = null;
            foreach (var pairing in line.Pairings)
            {
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];

                if (trip.DutyPeriods.SelectMany(x => x.Flights).Any(y => y.ETOPS))
                {
                    line.LineDisplay += "e";
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

                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                if (trip.DutyPeriods.SelectMany(x => x.Flights).Any(y => y.ETOPS))
                {
                    etopsCount++;
                }
            }
            return etopsCount;
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
                    trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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
        private void CalculateRigProperties(Line line)
        {
            Trip trip = null;
            DateTime tripStartDate = DateTime.MinValue;
            line.RigAdgInBp = 0.0m;
            line.RigAdgInLine = 0.0m;
            line.RigTafbInBp = 0.0m;
            line.RigTafbInLine = 0.0m;

            line.RigDailyMinInBp = 0.0m;
            line.RigDailyMinInLine = 0.0m;
            line.RigDhrInBp = 0.0m;
            line.RigDhrInLine = 0.0m;
            line.RigFltInBP = 0.0m;
            line.RigFltInLine = 0.0m;

            line.RigTotalInBp = 0.0m;
            line.RigTotalInLine = 0.0m;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip, currentBidDetails);
                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                line.RigAdgInBp += trip.RigAdg;
                line.RigAdgInLine += trip.RigAdg;
                line.RigTafbInBp += trip.RigTafb;
                line.RigTafbInLine += trip.RigTafb;

                line.RigTotalInLine = line.RigTotalInLine + trip.RigAdg + trip.RigTafb;
                line.RigTotalInBp = line.RigTotalInBp + trip.RigAdg + trip.RigTafb;


                foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
                {
                    if (tripStartDate <= bpEndDate)
                    {
                        line.RigDailyMinInBp += dutyPeriod.RigDailyMin;
                        line.RigDhrInBp += dutyPeriod.RigDhr;
                        line.RigFltInBP += dutyPeriod.RigFlt;
                        line.RigTotalInBp = line.RigTotalInBp + dutyPeriod.RigDailyMin + dutyPeriod.RigDhr + dutyPeriod.RigFlt;
                    }
                    line.RigDailyMinInLine += dutyPeriod.RigDailyMin;
                    line.RigDhrInLine += dutyPeriod.RigDhr;
                    line.RigFltInLine += dutyPeriod.RigFlt;

                    line.RigTotalInLine = line.RigTotalInLine + dutyPeriod.RigDailyMin + dutyPeriod.RigDhr + dutyPeriod.RigFlt;
                }
            }
            //line.RigAdgInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigAdgInBp)), 2, MidpointRounding.AwayFromZero);
            //line.RigTafbInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigTafbInBp)), 2, MidpointRounding.AwayFromZero);
            //line.RigDailyMinInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigDailyMinInBp)), 2, MidpointRounding.AwayFromZero);
            //line.RigDhrInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigDhrInBp)), 2, MidpointRounding.AwayFromZero);
            //line.RigFltInBP = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigFltInBP)), 2, MidpointRounding.AwayFromZero);
            //line.RigTotalInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigTotalInBp)), 2, MidpointRounding.AwayFromZero);

            line.RigAdgInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigAdgInBp)), 2);
            line.RigTafbInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigTafbInBp)), 2);
            line.RigDailyMinInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigDailyMinInBp)), 2);
            line.RigDhrInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigDhrInBp)), 2);
            line.RigFltInBP = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigFltInBP)), 2);
            line.RigTotalInBp = Math.Round(decimal.Parse(String.Format("{0:0.00}", line.RigTotalInBp)), 2);

        }
        private void SetMonthlyLineRig(Line line)
        {
            decimal minRig = 0;
            if (GlobalSettings.CurrentBidDetails.Postion != "FA")
            {
                

                switch (GlobalSettings.CurrentBidDetails.Month)
                {

                    case 2:
                        minRig = 85;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        minRig = 87;
                        break;
                    default:
                        minRig = 89;
                        break;
                }
                //check the line hae any dp extened to next month
                Trip trip = null;
                string lastPairing = line.Pairings[line.Pairings.Count() - 1];
                bool isLastTrip = false; int paringCount = 0;
                trip = (trips.ContainsKey(lastPairing.Substring(0, 4))) ? trips[lastPairing.Substring(0, 4)] : trips[lastPairing];
                var lasttripStartDate = WBidCollection.SetDate(Convert.ToInt16(lastPairing.Substring(4, 2)), true);
                if (lasttripStartDate.AddDays(trip.DutyPeriods.Count - 1) > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                {
                    //next month extended trip exists
                    decimal tfpinBp = 0;
                    foreach (var pairing in line.Pairings)
                    {
                        isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                        trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];

                        int date = Convert.ToInt16(pairing.Substring(4, 2));
                        DateTime tripstartdate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                        if (tripstartdate.AddDays(trip.DutyPeriods.Count - 1) > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                        {
                            List<RigDist> distributedRig = DistributeRigWhenTripOVerlapToNextMonth(trip, minRig, tripstartdate);
                            var InBpDutyperiods = distributedRig.Where(x => x.isInBp);
                            var totaltripPayInBp = InBpDutyperiods.Sum(x => x.Tfp) + InBpDutyperiods.Sum(y => y.Rig);
                            tfpinBp += totaltripPayInBp;
                        }
                        else
                        {
                            tfpinBp += trip.Tfp;
                        }
                    }
                    if (line.Tfp < minRig)
                    {
                        line.LineRig = Math.Round(minRig - tfpinBp, 2);
                        line.Tfp = tfpinBp;
                    }
                }
                else
                {
                    //all DP are in BP
                    if (line.Tfp < minRig)
                        line.LineRig = minRig - line.Tfp;
                }

            }
            else
            {
                //if (line.ReserveLine)
                //    minRig = 72;
                //else
                //    minRig = 80;
                ////all DP are in BP
                //if (line.Tfp < minRig)
                //    line.LineRig = minRig - line.Tfp;
            }

        }
        private List<RigDist> DistributeRigWhenTripOVerlapToNextMonth(Trip trip, decimal minRig, DateTime tripdate)
        {
            //set sorted Dp according to the pay
            var SortedDpsForDist = new List<RigDist>();
            DateTime tripStartDate = tripdate;
            int count = 0;
            foreach (var dp in trip.DutyPeriods)
            {
                bool isInDp = tripStartDate.AddDays(count) <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
                SortedDpsForDist.Add(new RigDist { dpSeqNor = dp.DutPerSeqNum, Tfp = dp.Tfp, isInBp = isInDp, Rig = 0 });
                count++;
            }
            decimal rigRemain = trip.Tfp - (SortedDpsForDist.Sum(x => x.Tfp));
            // distribute the rig to last duty period
            SortedDpsForDist[SortedDpsForDist.Count - 1].Rig = rigRemain;
            //commented below code on 5-6-2020 as we dont need to distribute the Rig equally . We need to ditribute the rig to the last DP
            return SortedDpsForDist;
        }
        //        private List<int> CalcStartDowList(Line line)
        //        {
        //            List<int> startdays = new List<int>();
        //            for (int count = 0; count < 8; count++)
        //            {
        //                startdays.Add(0);
        //            }
        //            Trip trip = null;
        //            string sdow = " "; ;
        //            int startDowInt = 0;
        //            int oldDowInt = 9;
        //            int nextDate = 0;
        //            bool isLastTrip = false; int paringCount = 0;
        //            foreach (var pairing in line.Pairings)
        //            {
        //                trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
        //
        //                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
        //
        //                int date = Convert.ToInt16(pairing.Substring(4, 2));
        //                int lenghtOfTrip = trip.PairLength;
        //                if (date != nextDate)
        //                {
        //                    nextDate = date + lenghtOfTrip;
        //                    startDowInt = Convert.ToInt32(WBidCollection.SetDate(date, isLastTrip, currentBidDetails).DayOfWeek);
        //                    oldDowInt = oldDowInt == 9 ? startDowInt : oldDowInt;
        //                }
        //                else nextDate = date + lenghtOfTrip;
        //
        //                if (startDowInt != oldDowInt)
        //                {
        //                    startDowInt = 7;
        //                }
        //               
        //                switch (startDowInt)
        //                {
        //                    case 0:
        //                        startdays[6]++; //sun
        //                        break;
        //                    case 1:
        //                        startdays[0]++;//Mon
        //                        break;
        //                    case 2:
        //                        startdays[1]++;
        //                        break;
        //                    case 3:
        //                        startdays[2]++;
        //                        break;
        //                    case 4:
        //                        startdays[3]++;
        //                        break;
        //                    case 5:
        //                        startdays[4]++;
        //                        break;
        //                    case 6:
        //                        startdays[5]++;
        //                        break;
        //                    case 7:
        //                        startdays[7]++;
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //
        //            
        //            return startdays;
        //        }

        private List<int> CalcStartDowList(Line line)
        {
            var startdays = new List<int>(new int[7]);

            int paringCount = 0;

            DateTime oldTripEndDate = DateTime.MinValue;
            foreach (var pairing in line.Pairings)
            {
                Trip trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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
        private List<int> CalcStartDowListPerTrip(Line line)
        {
            var startdays = new List<int>(new int[7]);

            int paringCount = 0;


            foreach (var pairing in line.Pairings)
            {
                Trip trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
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

        #region OverlapCorrection
        ///// <summary>
        ///// Calculate overlap Days
        ///// </summary>
        ///// <param name="line"></param>
        //private void CalculateOverlapDays(Line line)
        //{

        //    _newBidPeriodDay = new List<Day>();
        //    _oldBidPeridDays = GlobalSettings.LeadOutDays;
        //    _combinedDays = new List<Day>();

        //    //Temporay code for testing
        //    // frank added day in front, should be last 6 days from old bid period and 6 lead-in days from old bid period
        //    // also, made data more realistic
        //    // to test April month
        //    //_oldBidPeridDays = new List<Day>()
        //    //{
        //    //    new Day{Date=new DateTime(2013,3,26),FlightTime=450,DepartutreTime=420,ArrivalTime=895},       // added another day so there are 6 days from old bid period instead of 5
        //    //    new Day{Date=new DateTime(2013,3,27),FlightTime=450,DepartutreTime=0,ArrivalTime=0},
        //    //    new Day{Date=new DateTime(2013,3,28),FlightTime=0,DepartutreTime=0,ArrivalTime=0},         // changed by frank -- in any line there is always 3 days off between pairings
        //    //    new Day{Date=new DateTime(2013,3,29),FlightTime=0,DepartutreTime=0,ArrivalTime=0},         // also, original lines never fly more than 4 days in a row
        //    //    new Day{Date=new DateTime(2013,3,30),FlightTime=0,DepartutreTime=0,ArrivalTime=0},         // however, after an overlap correction, it is possible to fly 6 days in a row
        //    //    new Day{Date=new DateTime(2013,3,31),FlightTime=435,DepartutreTime=380,ArrivalTime=880},
        //    //    new Day{Date=new DateTime(2013,4,1),FlightTime=325,DepartutreTime=300,ArrivalTime=665},
        //    //    new Day{Date=new DateTime(2013,4,2),FlightTime=425,DepartutreTime=300,ArrivalTime=930},
        //    //    new Day{Date=new DateTime(2013,4,3),FlightTime=475,DepartutreTime=300,ArrivalTime=860},
        //    //    new Day{Date=new DateTime(2013,4,4),FlightTime=0,DepartutreTime=0,ArrivalTime=0},
        //    //    new Day{Date=new DateTime(2013,4,5),FlightTime=0,DepartutreTime=0,ArrivalTime=0},
        //    //    new Day{Date=new DateTime(2013,4,6),FlightTime=0,DepartutreTime=0,ArrivalTime=0}
        //    //};

        //    // to test march month
        //    //_oldBidPeridDays = new List<Day>()
        //    //{
        //    //    new Day{Date=new DateTime(2013,2,23),FlightTime=450},       // added another day so there are 6 days from old bid period instead of 5
        //    //    new Day{Date=new DateTime(2013,2,24),FlightTime=450},
        //    //    new Day{Date=new DateTime(2013,2,25),FlightTime=0},         // changed by frank -- in any line there is always 3 days off between pairings
        //    //    new Day{Date=new DateTime(2013,2,26),FlightTime=0},         // also, original lines never fly more than 4 days in a row
        //    //    new Day{Date=new DateTime(2013,2,27),FlightTime=0},         // however, after an overlap correction, it is possible to fly 6 days in a row
        //    //    new Day{Date=new DateTime(2013,2,28),FlightTime=435},
        //    //    new Day{Date=new DateTime(2013,3,1),FlightTime=325},
        //    //    new Day{Date=new DateTime(2013,3,2),FlightTime=425},
        //    //    new Day{Date=new DateTime(2013,3,3),FlightTime=475},
        //    //    new Day{Date=new DateTime(2013,3,4),FlightTime=0},
        //    //    new Day{Date=new DateTime(2013,3,5),FlightTime=0},
        //    //    new Day{Date=new DateTime(2013,3,6),FlightTime=0}
        //    //};

        //    //int daysinmonth = DateTime.DaysInMonth(int.Parse(year), int.Parse(month));
        //    //DateTime date = new DateTime(int.Parse(year), int.Parse(month), 1);
        //    int daysinmonth = (bpEndDate - bpStartDate).Days + 1;
        //    DateTime date = bpStartDate;

        //    //create date for the new period day
        //    for (int count = 0; count < daysinmonth; count++)
        //    {
        //        Day day = new Day();
        //        day.Date = date;
        //        date = date.AddDays(1);
        //        _newBidPeriodDay.Add(day);
        //    }

        //    ///iterate through all the Sips in the selected line
        //    foreach (var sip in line.LineSips)
        //    {
        //        DateTime sipstartdate = sip.SipStartDate;
        //        for (int count = 0; count < sip.Sip.SipDutyPeriods.Count; count++)
        //        {

        //            if (_newBidPeriodDay.Any(x => x.Date == sip.SipStartDate))
        //            {

        //                Day objday = _newBidPeriodDay.Where(x => x.Date == sipstartdate).FirstOrDefault();
        //                if (objday != null)
        //                {
        //                    objday.FlightTime += sip.Sip.SipDutyPeriods[count].FlightHours;
        //                    sipstartdate = sipstartdate.AddDays(1);
        //                }
        //            }
        //        }
        //    }




        //    // This Will combime _oldBidPeridDays and _newBidPeriodDay lists for overlap correction
        //    GenarateCombinedDays();

        //    //Check line is overlapped with previous date 
        //    OverlapConflictCheck(line);
        //    if (GlobalSettings.CurrentBidDetails.Postion != "FA")
        //    {
        //        //30/7  check. Check to see 30 hours(1800 minutes) maximum flight time for one week.
        //        OverlapCorrrectionWeekhours(line);
        //    }

        //    // If the amount of rest from the lead in days to the start of the lead out days is less than GlobalSetting.requiredRest, then drop the first SIP.
        //    MinimumRestOverlapCorrrection(line);

        //    if (GlobalSettings.CurrentBidDetails.Postion != "FA")
        //    {
        //        //100 hours in a month.check the new bid period have totally 100 hours flight time.if not remove the lowest TFP SIP that will make the line legal for 100 hours and mark that LineSip as dropped.
        //        OverlapMonthhoursCheck(line);
        //    }

        //    //24hoursRestCheck -Check each 7 days has atleat 24 hours off
        //    overlap24hoursRestCheck(line);

        //    //set the showoverlap line property  to true if any of the sip droppped for the line
        //    //setShowOvelapVisibility(line);


        //}

        ///// <summary>
        ///// set the showoverlap line property  to true if any of the sip droppped for the line
        ///// </summary>
        ///// <param name="line"></param>
        //private void setShowOvelapVisibility(Line line)
        //{
        //    line.ShowOverLap = line.LineSips.Any(x => x.dropped);
        //}

        ///// <summary>
        ///// This Will combime _oldBidPeridDays and _newBidPeriodDay lists for overlap correction
        ///// </summary>
        //private void GenarateCombinedDays()
        //{
        //    //genarate combine dayas(Sum of _newBidPeriodDay and  _oldBidPeridDays lists)
        //    _combinedDays.AddRange(_oldBidPeridDays);
        //    _combinedDays.AddRange(_newBidPeriodDay);
        //    _combinedDays = _combinedDays.GroupBy(x => x.Date).Select(y => new Day { Date = y.Key, FlightTime = y.Sum(z => z.FlightTime) }).ToList();
        //}

        ///// <summary>
        ///// Check line is overlapped with previous date (Condition :lead out trip has flying on the same day as lead in trip)
        ///// </summary>
        ///// <param name="line"></param>
        //private void OverlapConflictCheck(Line line)
        //{
        //    //get the valid start date for new bid period

        //    //var validcurrentbiddate = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault().Date;
        //    DateTime validcurrentbiddate;
        //    var item = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault();
        //    if (item != null)
        //    {
        //        validcurrentbiddate = item.Date;

        //        var oldbidperiodenddate = validcurrentbiddate.AddDays(-1);

        //        for (int count = 0; count < 6; count++)
        //        {
        //            var date = _newBidPeriodDay[count].Date;

        //            if (_newBidPeriodDay.Where(x => x.Date == date).FirstOrDefault() != null && _oldBidPeridDays.Where(x => x.Date == date).FirstOrDefault() != null)
        //            {
        //                //check the flight time exist for _newBidPeriodDay and _oldBidPeridDays for the same day
        //                if (_newBidPeriodDay.Where(x => x.Date == date).FirstOrDefault().FlightTime != 0 && _oldBidPeridDays.Where(x => x.Date == date).FirstOrDefault().FlightTime != 0)
        //                {
        //                    //remove the droped sip from the siplist.
        //                    var linesips = line.LineSips.Where(x => !x.dropped).ToList();

        //                    foreach (var sip in linesips)
        //                    {

        //                        //get the sip start date of the SIP
        //                        DateTime sipstartdate = sip.SipStartDate;

        //                        //if the date is oldbidperiodenddate then we have to check whether it satisfy the minimum Rest time.if it satisfy the minimum rest time then we can make this sip as legal(rare chance?)
        //                        if (date == oldbidperiodenddate)
        //                        {
        //                            int requiredRest = GlobalSettings.requiredRest;
        //                            //int LastLegArrivalTime=GlobalSettings.OverlapDetails.LastLegArrivalTime;
        //                            //for the test purpose
        //                            int LastLegArrivalTime = GlobalSettings.LastLegArrivalTime;

        //                            //get the start leg departure time for the Sip
        //                            int startLegDepartureTime = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault().DepTime;
        //                            if (LastLegArrivalTime + requiredRest < startLegDepartureTime)
        //                            {
        //                                continue;
        //                            }
        //                        }
        //                        ///check the assosiate sip  for the date in the illegalWeekhoursDays
        //                        if (sipstartdate <= date && date <= sipstartdate.AddDays(sip.Sip.SipDutyPeriods.Count - 1))
        //                        {
        //                            //remove the sip (Drop the flight hours for the sip from the Sip opearating days)
        //                            DropoverlappedSip(sip, sipstartdate);
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //    }

        //}

        ///// <summary>
        ///// 30/7  check. Check to see 30 hours(1800 minutes) maximum flight time for one week.
        ///// </summary>
        ///// <param name="line"></param>
        //private void OverlapCorrrectionWeekhours(Line line)
        //{

        //    List<Day> illegalWeekhoursDays = new List<Day>();
        //    //get the valid start date for new bid period
        //    var date = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault().Date;

        //    ///back ward iteration to avoid the pupose of kepping the forward and reverse illegal days in separate list(iterate upto the 12 th day of the new bid period)
        //    for (DateTime iterationdate = date; iterationdate < new DateTime(int.Parse(year), int.Parse(month), 12); iterationdate = iterationdate.AddDays(1))
        //    {
        //        int flighttimesum = 0;
        //        flighttimesum = _combinedDays.Where(y => (iterationdate.AddDays(-6) <= y.Date) && y.Date <= iterationdate).Sum(x => x.FlightTime);
        //        if (flighttimesum > 1800)
        //        {
        //            var illegalday = _combinedDays.Where(x => x.Date == iterationdate).FirstOrDefault();
        //            if (illegalday.FlightTime != 0)
        //            {
        //                illegalWeekhoursDays.Add(illegalday);
        //            }
        //        }

        //    }
        //    //iterate illegalWeekhoursDays
        //    foreach (Day day in illegalWeekhoursDays)
        //    {
        //        var excessflighttimehours = _combinedDays.Where(y => (day.Date.AddDays(-6) <= y.Date) && y.Date <= day.Date).Sum(x => x.FlightTime) - 1800;

        //        if (excessflighttimehours > 0)
        //        {
        //            //remove the droped sip from the siplist.
        //            var linesips = line.LineSips.Where(x => !x.dropped).ToList();

        //            //get the linesips for the 7 days period and it ordered by flight time ascending values.
        //            linesips = linesips.Where(x => ((day.Date.AddDays(-6) <= x.SipStartDate || (day.Date.AddDays(-6) <= x.SipStartDate.AddDays(x.Sip.SipDutyPeriods.Count - 1))) && x.SipStartDate <= day.Date)).OrderBy(x => x.SipFltHrs).ToList();
        //            foreach (var sip in linesips)
        //            {
        //                //get the sip start date of the SIP
        //                DateTime sipstartdate = sip.SipStartDate;

        //                //remove the sip (Drop the flight hours for the sip from the Sip opearating days)
        //                DropoverlappedSip(sip, sipstartdate);

        //                //check the legality --check for week hours after dropped the flight hours (returns true if it make legal)
        //                bool legal = CheckOverlaplegalityforweekhours(day.Date.AddDays(-6), day.Date);
        //                if (legal)
        //                {
        //                    //if the droping the sip makes it legal ,then go to the next illegalWeekhoursDays
        //                    break;
        //                }
        //            }
        //        }

        //    }
        //}

        ///// <summary>
        ///// If the amount of rest from the lead in days to the start of the lead out days is less than GlobalSetting.requiredRest, then drop the first SIP.
        ///// </summary>
        ///// <param name="line"></param>
        //private void MinimumRestOverlapCorrrection(Line line)
        //{
        //    int requiredRest = GlobalSettings.requiredRest;
        //    int LastLegArrivalTime = GlobalSettings.LastLegArrivalTime;

        //    //get the valid start date for new bid period
        //    //var date = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault().Date;

        //    DateTime date;
        //    var item = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault();
        //    if (item != null)
        //    {
        //        date = item.Date;
        //        var oldbidperiodenddate = date.AddDays(-1);


        //        //remove the droped sip from the siplist.
        //        List<LineSip> linesips = line.LineSips.Where(x => !x.dropped).ToList();
        //        foreach (var sip in linesips)
        //        {
        //            string pairing = line.Pairings.Where(x => x.Contains(sip.Sip.SipName.Substring(0, 4))).FirstOrDefault();
        //            Trip trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
        //            //get the start leg departure time for the Sip
        //            int startLegDepartureTime = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault().DepTime;
        //            //startLegDepartureTime -= (GlobalSettings.CurrentBidDetails.Postion == "FA") ? GlobalSettings.FAshow1stDutyPeriod : GlobalSettings.show1stDay;
        //            startLegDepartureTime -= (GlobalSettings.CurrentBidDetails.Postion == "FA") ? GlobalSettings.FAshow1stDutyPeriod : trip.BriefTime;
        //            bool legal = false;

        //            //if the sipstart date is  old bid period end date we have to check the Rest conflists.If it satisfy the condition,then the sip is legal.
        //            if (oldbidperiodenddate == sip.SipStartDate)
        //            {
        //                if (LastLegArrivalTime + GlobalSettings.debrief + requiredRest < startLegDepartureTime)
        //                {
        //                    legal = true;
        //                }
        //            }
        //            else if ((sip.SipStartDate - oldbidperiodenddate).Days == 1)
        //            {

        //                if (LastLegArrivalTime + GlobalSettings.debrief + requiredRest < 1440 + startLegDepartureTime)
        //                {
        //                    legal = true;
        //                }
        //            }
        //            else
        //            {
        //                legal = true;
        //            }
        //            if (legal)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                DropoverlappedSip(sip, sip.SipStartDate);
        //            }

        //        }
        //    }

        //}

        ///// <summary>
        ///// 100 hours in a month.check the new bid period have totally 100 hours flight time.if not remove the lowest TFP SIP that will make the line legal for 100 hours and mark that LineSip as dropped.
        ///// </summary>
        ///// <param name="line"></param>
        //private void OverlapMonthhoursCheck(Line line)
        //{

        //    //order the sip list 
        //    var orderedlinesips = line.LineSips.OrderBy(x => x.SipFltHrs);

        //    DateTime startdate = new DateTime(int.Parse(year), int.Parse(month), 1);
        //    DateTime enddate = startdate.AddMonths(1).AddDays(-1);
        //    if (!CheckOverlaplegalityformonthlyhours(startdate, enddate))
        //    {
        //        foreach (var sip in orderedlinesips)
        //        {

        //            //remove the sip (Drop the flight hours for the sip from the Sip opearating days)
        //            DropoverlappedSip(sip, sip.SipStartDate);

        //            bool legal = CheckOverlaplegalityformonthlyhours(startdate, enddate);

        //            if (legal)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                //if it is not legal ,we have to check for the next lowest sip.
        //                //previously removed sip doesnot not make the line legal,so we need to add the flight hours for the sip beacuse we removed flight hours to check the legality.
        //                DateTime sipstartdate = sip.SipStartDate;
        //                for (int count = 0; count < sip.Sip.SipDutyPeriods.Count; count++)
        //                {

        //                    if (_combinedDays.Any(x => x.Date == sip.SipStartDate))
        //                    {

        //                        Day objday = _combinedDays.Where(x => x.Date == sipstartdate).FirstOrDefault();
        //                        if (objday != null)
        //                        {
        //                            objday.FlightTime += sip.Sip.SipDutyPeriods[count].FlightHours;
        //                            sipstartdate = sipstartdate.AddDays(1);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// 24hoursRestCheck -Check each 7 days has atleat 24 hours off
        ///// </summary>
        ///// <param name="line"></param>
        //private void overlap24hoursRestCheck(Line line)
        //{


        //    // var date = new DateTime(int.Parse(year), int.Parse(month), 1);
        //    var date = bpStartDate;
        //    var day = (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Month == 2) ? 11 : 12;
        //    ///back ward iteration  (iterate upto the 12 th day of the new bid period for captain and 11 th day of the new bid period for FA)
        //    for (DateTime iterationdate = date; iterationdate < new DateTime(int.Parse(year), int.Parse(month), day); iterationdate = iterationdate.AddDays(1))
        //    {
        //        int loopcount = 0;
        //        while (true)
        //        {
        //            ++loopcount;
        //            bool legal = false;
        //            DateTime startdate = iterationdate.AddDays(-6);

        //            //check any day of exist within the 7 days.If exit we can break the loop and go to next iteration of 7 days
        //            bool isdayoffexist = _combinedDays.Where(y => (startdate <= y.Date) && y.Date <= iterationdate).Any(x => x.FlightTime == 0);

        //            if (!isdayoffexist)
        //            {
        //                //we need to check any "24 hours off" exist between the 7 days.
        //                legal = CheckOverlaplegalityfor24houroff(line, iterationdate);
        //                if (!legal)
        //                {
        //                    //remove the droped sip from the siplist.
        //                    List<LineSip> linesips = line.LineSips.Where(x => !x.dropped).ToList();

        //                    //get all sips between the 7 days .(I have to include the previous days sip if the previous sip days duty period includes this 7 days.So I used Sip.SipDutyPeriods.Count-1)
        //                    var sevendayssips = linesips.Where(x => (startdate <= x.SipStartDate.AddDays(x.Sip.SipDutyPeriods.Count - 1)) && x.SipStartDate <= iterationdate);

        //                    foreach (var sip in sevendayssips)
        //                    {
        //                        //remove the first sip in the sevendayssips list
        //                        DropoverlappedSip(sip, sip.SipStartDate);
        //                    }
        //                    if (loopcount == 7)
        //                        break;

        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                //if 24 hours check is legal, we have to exit the while loop and go to next iteration for the date.
        //                break;
        //            }
        //        }

        //    }
        //}

        ///// <summary>
        ///// Drop the sip  to correct the overlap
        ///// </summary>
        ///// <param name="sip"></param>
        ///// <param name="sipstartdate"></param>
        //private void DropoverlappedSip(LineSip sip, DateTime sipstartdate)
        //{
        //    //we need to drop the sip to correct the overlap
        //    //save the dropflight time and drop status to the  the line

        //    for (int count = 0; count < sip.Sip.SipDutyPeriods.Count; count++)
        //    {

        //        if (_combinedDays.Any(x => x.Date == sipstartdate))
        //        {

        //            Day objday = _combinedDays.Where(x => x.Date == sipstartdate).FirstOrDefault();
        //            if (objday != null)
        //            {
        //                //subtract the appropriate flight hours for the day
        //                objday.FlightTime -= sip.Sip.SipDutyPeriods[count].FlightHours;
        //                sipstartdate = sipstartdate.AddDays(1);
        //            }
        //        }
        //    }
        //    //set the drop status for this Sip to true to indcate that the sip is Droppped to correct the overlap.
        //    sip.dropped = true;

        //}

        ///// <summary>
        ///// check the legality for week hours(30 hours or toal 1800 minutes in 7 days)
        ///// </summary>
        ///// <param name="startDate"></param>
        ///// <param name="endatate"></param>
        ///// <returns></returns>
        //private bool CheckOverlaplegalityforweekhours(DateTime startDate, DateTime enddate)
        //{
        //    int flighttimesum = _combinedDays.Where(y => (startDate <= y.Date) && y.Date <= enddate).Sum(x => x.FlightTime);
        //    return (flighttimesum <= 1800);
        //}

        ///// <summary>
        ///// check the legality for Monthly hours(100 hours or toal 6000 minutes in a calender month)
        ///// </summary>
        ///// <param name="startDate"></param>
        ///// <param name="enddate"></param>
        ///// <returns></returns>
        //private bool CheckOverlaplegalityformonthlyhours(DateTime startDate, DateTime enddate)
        //{
        //    int flighttimesum = _combinedDays.Where(y => (startDate <= y.Date) && y.Date <= enddate).Sum(x => x.FlightTime);
        //    return (flighttimesum <= 6000);
        //}

        ///// <summary>
        ///// check the legality for 24 hours off in 7 days.
        ///// </summary>
        ///// <param name="line"></param>
        ///// <param name="iterationdate"></param>
        ///// <returns></returns>
        //private bool CheckOverlaplegalityfor24houroff(Line line, DateTime iterationdate)
        //{
        //    //get the valid start date for new bid period

        //    bool legal = false;
        //    //var validDate = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault().Date;
        //    DateTime validDate;
        //    var item = _oldBidPeridDays.OrderBy(x => x.Date).Where(x => x.Date.Month == int.Parse(month) && x.FlightTime == 0).FirstOrDefault();
        //    if (item != null)
        //    {
        //        validDate = item.Date;

        //        int arriavaltime = 0;
        //        int count = 0;
        //        for (DateTime innerdate = iterationdate.AddDays(-6); innerdate <= iterationdate; innerdate = innerdate.AddDays(1))
        //        {
        //            //for the first iterationdate  we have to intialise arriavaltime and it needs not tocheck the legality.Becuase it is a starting day  of the 7 consecucutive day
        //            if (count == 0)
        //            {
        //                var obj = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate);
        //                if (obj != null)
        //                {
        //                    arriavaltime = obj.ArrivalTime;
        //                }
        //                count++;

        //                continue;
        //            }

        //            if (innerdate < validDate)
        //            {
        //                //get the departure time
        //                //int currentdeparturetime = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate).DepartutreTime;\
        //                int currentdeparturetime = 0;
        //                var deptitem = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate);
        //                if (deptitem != null)
        //                    currentdeparturetime = deptitem.DepartutreTime;

        //                //get the arrival time
        //                //int currentarriavaltime = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate).ArrivalTime;
        //                int currentarriavaltime = 0;
        //                var arriveitem = _oldBidPeridDays.FirstOrDefault(x => x.Date == innerdate);
        //                if (arriveitem != null)
        //                    currentarriavaltime = arriveitem.ArrivalTime;

        //                //if  previous day arrival time is less than current departure time,then it is legaly ok.(It have minimum 1440 minutes difference)
        //                if (arriavaltime < currentdeparturetime)
        //                {
        //                    legal = true;
        //                    break;
        //                }
        //                else
        //                {
        //                    arriavaltime = currentarriavaltime;
        //                }
        //            }
        //            else
        //            {
        //                //remove the droped sip from the siplist.
        //                List<LineSip> linesips = line.LineSips.Where(x => !x.dropped).ToList();

        //                DateTime sipenddate = validDate;

        //                foreach (var sip in linesips)
        //                {

        //                    DateTime sipstartdate = sip.SipStartDate;

        //                    if (sipstartdate <= innerdate && innerdate <= sipstartdate.AddDays(sip.Sip.SipDutyPeriods.Count - 1))
        //                    {
        //                        //get the current sip departure time
        //                        //int currentsipDeparturetime = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault().DepTime;
        //                        int currentsipDeparturetime = 0;
        //                        var currentsipDepartureitem = sip.Sip.SipDutyPeriods.FirstOrDefault().Flights.FirstOrDefault();
        //                        if (currentsipDepartureitem != null)
        //                            currentsipDeparturetime = currentsipDepartureitem.DepTime;
        //                        //get the current sip ariival time
        //                        var fligts = sip.Sip.SipDutyPeriods[sip.Sip.SipDutyPeriods.Count - 1].Flights;
        //                        int currentsiparrivaltime = fligts[fligts.Count - 1].ArrTime;

        //                        DateTime currentsipenddate = sipstartdate.AddDays(sip.Sip.SipDutyPeriods.Count - 1);
        //                        //if the previous sip end date is same as the current sip date ,that means a day has two sip) ,we dont need to consider this.becuase it violates the condition.
        //                        if (sipenddate == currentsipenddate)
        //                        {
        //                            continue;
        //                        }
        //                        if (arriavaltime < currentsipDeparturetime)
        //                        {
        //                            legal = true;
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            arriavaltime = currentsiparrivaltime;
        //                            sipenddate = currentsipenddate;
        //                        }

        //                    }
        //                }
        //                if (legal)
        //                {
        //                    break;
        //                }


        //            }

        //        }
        //    }
        //    return legal;
        //}

        #endregion

        #endregion

        public void CalculateCommuteLineProperties(WBidState wBIdStateContent)
        {

            if (wBIdStateContent.Constraints.DailyCommuteTimesCmmutability != null)
            {
                int checkinTime = 0;
                int BaseTime = 0;
                var lines = GlobalSettings.Lines.Where(x => x.BlankLine == false).ToList();
                Trip trip;
                foreach (var line in lines)
                {
                    line.CommutableBacks = 0;
                    line.commutableFronts = 0;
                    line.CommutabilityFront = 0;
                    line.CommutabilityBack = 0;
                    line.CommutabilityOverall = 0;
                    DateTime tripStartDate = DateTime.MinValue;

                    if (line.WorkBlockList != null)
                    {
                        bool isCommuteFrontEnd = false;
                        bool isCommuteBackEnd = false;

                        foreach (WorkBlockDetails workBlock in line.WorkBlockList)
                        {
                            //Checking the  corresponding Commute based on Workblock Start time
                            CommuteTime commutTimes = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.StartDateTime.Date);

                            // if (commutTimes != null && StateContent.ToWork)
                            if (commutTimes != null)
                            {
                                if (commutTimes.EarliestArrivel != DateTime.MinValue)
                                {

                                    double value = Convert.ToDouble(wBIdStateContent.Constraints.CLAuto.CheckInTime);

                                    isCommuteFrontEnd = ((commutTimes.EarliestArrivel.AddMinutes(value)) <= workBlock.StartDateTime.AddMinutes(workBlock.BriefTime));
                                    if (isCommuteFrontEnd)
                                    {
                                        line.commutableFronts++;
                                    }
                                }
                            }


                            //Checking the  corresponding Commute based on Workblock End time
                            //commutTimes = GlobalSettings.WBidStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDateTime.Date);
                            // using EndDate to account for irregular datetimes in company time keeping method.
                            commutTimes = wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDate.Date);

                            // if (commutTimes != null && StateContent.ToHome)
                            if (commutTimes != null)
                            {
                                if (commutTimes.LatestDeparture != DateTime.MinValue)
                                {
                                    double value = Convert.ToDouble(wBIdStateContent.Constraints.CLAuto.BaseTime);
                                    isCommuteBackEnd = commutTimes.LatestDeparture.AddMinutes(-value) >= workBlock.EndDateTime;
                                    if (isCommuteBackEnd)
                                    {
                                        line.CommutableBacks++;
                                    }
                                }
                            }

                        }

                    }
                    line.TotalCommutes = line.WorkBlockList.Count;
                    if (line.TotalCommutes > 0)
                    {
                        line.CommutabilityFront = (line.commutableFronts / line.TotalCommutes) * 100;
                        line.CommutabilityBack = (line.CommutableBacks / line.TotalCommutes) * 100;
                        line.CommutabilityOverall = ((line.commutableFronts + line.CommutableBacks) / (2 * line.TotalCommutes)) * 100;
                    }
                }
            }

        }

    }
    public class RigDist
    {
        public int dpSeqNor { get; set; }
        public decimal Tfp { get; set; }

        public bool isInBp { get; set; }

        public decimal Rig { get; set; }

    }
}
