using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
   public class ReCalculateLinePropertiesForOverlapCorrection
    {

        public void ReCalculateLinePropertiesOnOverlapCorrection(List<Line> lines, bool isOverlapCorrection)
        {
            foreach (Line line in lines)
            {
                if (line.BlankLine)
                    continue;


                if (line.LineSips != null)
                {

                    List<LineSip> droppedsip = line.LineSips.Where(x => x.Dropped).ToList();
                    //get the sum of droppped flight hours
                    var droppedflighthours = droppedsip.Sum(y => y.SipFltHrs);
                    // Get the sum of dropped TFS
                    var droppedTFP = droppedsip.Sum(y => y.SipTfp);
                    //get the sum of Dropped Tafb
                    var droppedTafb = droppedsip.Sum(y => y.Sip.SipTAFB);

                    var droppedaircraftchange = droppedsip.SelectMany(x => x.Sip.SipDutyPeriods.SelectMany(y => y.Flights.Where(z => z.AcftChange))).Count();
                    var droppedlegs = droppedsip.SelectMany(x => x.Sip.SipDutyPeriods.SelectMany(y => y.Flights)).Count();

                    var dropppedlegsin800 = CalcNumLegsOfGivenType(800, droppedsip);
                    var dropppedlegsin700 = CalcNumLegsOfGivenType(700, droppedsip);
                    var dropppedlegsin500 = CalcNumLegsOfGivenType(500, droppedsip);
                    var dropppedlegsin300 = CalcNumLegsOfGivenType(300, droppedsip);
var dropppedlegsin600 = CalcNumLegsOfGivenType(600, droppedsip);
                    var dropppedlegsin200 = CalcNumLegsOfGivenType(200, droppedsip);
                    //calculate how many days off after applied  applied overlap correction
                    var listDroppedTrueDays = new List<DateTime>();
                    var listDroppedFalseDays = new List<DateTime>();
                    //line.LineSips.ForEach((linesip) =>
                    //{
                    //    for (int i = 0; i < linesip.Sip.SipDutyPeriods.Count; i++)
                    //    {
                    //        if (linesip.dropped)
                    //            listDroppedTrueDays.Add(linesip.SipStartDate.AddDays(i));
                    //        else
                    //            listDroppedFalseDays.Add(linesip.SipStartDate.AddDays(i));
                    //    }
                    //});

                    foreach (LineSip linesip in line.LineSips)
                    {
                        for (int i = 0; i < linesip.Sip.SipDutyPeriods.Count; i++)
                        {
                            if (linesip.Dropped)
                                listDroppedTrueDays.Add(linesip.SipStartDate.AddDays(i));
                            else
                                listDroppedFalseDays.Add(linesip.SipStartDate.AddDays(i));
                        }
                    }
                    //holds the fully dropped days (All flights in this dutyperiod are dropped.So users do not want to fly these days)
                    List<DateTime> droppedOverlappedDays = listDroppedTrueDays.Distinct().Where(x => !(listDroppedFalseDays.Distinct().Contains(x))).ToList();
                    int noOfDaysOFf = droppedOverlappedDays.Count;


                    // int noOfDaysOFf = listDroppedTrueDays.Distinct().Where(x => !(listDroppedFalseDays.Distinct().Contains(x))).Count();

                    var droppedOvernightCities = GetDroppedOvernightCities(line);
                    var DroppedWeekDaysWork = GetDroppedWeekDaysWork(droppedOverlappedDays);
                    ////==============

                    if (isOverlapCorrection)
                    {

                        //subtract droppedTFP.
                        line.Tfp -= droppedTFP;
                        line.TfpInLine -= droppedTFP;

                        //subtract droppedflighthours for in Bp and Inline 
                        int linetfpinminutesBp = Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInBp) - droppedflighthours;
                        int linetfpinminutesLine = Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInLine) - droppedflighthours;

                        //set the block hours for In line and In Bp
                        line.BlkHrsInBp = (linetfpinminutesBp > 0) ? Helper.ConvertMinutesToFormattedHour(linetfpinminutesBp) : "00:00";
                        line.BlkHrsInLine = (linetfpinminutesLine > 0) ? Helper.ConvertMinutesToFormattedHour(linetfpinminutesLine) : "00:00";

                      //  add days off
                        line.DaysOff += noOfDaysOFf;

                        //Subtract droppedPDiem
                        var currentpdieminBp = Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp.ToString());
                        currentpdieminBp -= droppedTafb;
                        line.TafbInBp = (currentpdieminBp / 60).ToString() + ":" + (currentpdieminBp % 60).ToString("d2");

                        var currentpdieminline = Helper.ConvertformattedHhhmmToMinutes(line.TafbInLine.ToString());
                        currentpdieminline -= droppedTafb;
                        line.TafbInLine = (currentpdieminline / 60).ToString() + ":" + (currentpdieminline % 60).ToString("d2");



                        line.AcftChanges -= droppedaircraftchange;
                        line.Legs -= droppedlegs;
                        line.DaysWork -= listDroppedTrueDays.Distinct().Count();
                        line.DaysWorkInLine -= listDroppedTrueDays.Distinct().Count();
                        line.LegsIn800 -= dropppedlegsin800;
                        line.LegsIn700 -= dropppedlegsin700;
                        line.LegsIn500 -= dropppedlegsin500;
                        line.LegsIn300 -= dropppedlegsin300;
						line.LegsIn600 -= dropppedlegsin600;
                        line.LegsIn200 -= dropppedlegsin200;
                        line.LongestGrndTime = CalcLongGrndTime(line, true);
                        line.OverlapDrop = droppedTFP;
                        line.MostLegs = CalculateMostlegs(line, true);
                        line.BlkOfDaysOff = CalcBlkOfDaysOffForOverlap(line, listDroppedFalseDays);
                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOffforOverlap(line, listDroppedFalseDays);
                        line.TotDutyPds = listDroppedFalseDays.Distinct().Count();
                        line.TotDutyPdsInBp = listDroppedFalseDays.Where(x => x >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate && x <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate).Distinct().Count();
                        line.Trips1Day = CalcNumTripsOfGivenLengthForOverLap(1, line, droppedOverlappedDays);
                        line.Trips2Day = CalcNumTripsOfGivenLengthForOverLap(2, line, droppedOverlappedDays);
                        line.Trips3Day = CalcNumTripsOfGivenLengthForOverLap(3, line, droppedOverlappedDays);
                        line.Trips4Day = CalcNumTripsOfGivenLengthForOverLap(4, line, droppedOverlappedDays);
                        line.TotPairings = CalcTotalPairingsForOverlap(line);

                        line.OvernightCities.RemoveAll(x => droppedOvernightCities.Any(y => y == x));


                        line.DaysOfWeekWork = CalcWeekDaysWork(line.DaysOfWeekWork, DroppedWeekDaysWork, true);
                        line.DaysOfMonthWorks = line.DaysOfMonthWorks.Where(x => !droppedOverlappedDays.Any(y => y == x.DayOfBidline)).ToList();
                        line.Weekend = CalcWkEndPropforOverlap(listDroppedFalseDays);
                        line.StartDow = CalcStartDowForOverlap(line, listDroppedFalseDays);
                        CalculateWorkBlockLengthForOverlap(line, listDroppedFalseDays);
                        line.Is3on3Off = CalculateIs3on3OffDayFOrOverlap(line, listDroppedFalseDays.Distinct().ToList());
                        line.GroundTimes = CalculateGroundTimesForOverlap(line);
                        line.DutyHrsInLine = CalcDutyHrsForOverlap(line);
                        line.LastArrTime = CalcLastArrTimeforOverlap(line);
                        line.LastDomArrTime = CalcLastDomicileArrTimeforOverlap(line);
                        CalculateRestPeriodsForOverlap(line);
                        line.EDomPush = CalcEDomPushForOverlap(line).ToString(@"hh\:mm");
                        line.EPush = CalcEPushForOverLap(line).ToString(@"hh\:mm"); ;
                        line.ShowOverLap = line.LineSips.Any(x => x.Dropped);

                    }
                    else
                    {
                        //add droppedTFP
                        line.Tfp += droppedTFP;
                        line.TfpInLine += droppedTFP;

                        //add droppedflighthours in Bp
                        int linetfpinminutesinBp = Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInBp) + droppedflighthours;
                        line.BlkHrsInBp = (linetfpinminutesinBp > 0) ? Helper.ConvertMinutesToFormattedHour(linetfpinminutesinBp) : "00:00";

                        //add droppedflighthours in Line
                        int linetfpinminutesinline = Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInLine) + droppedflighthours;
                        line.BlkHrsInLine = (linetfpinminutesinline > 0) ? Helper.ConvertMinutesToFormattedHour(linetfpinminutesinline) : "00:00";

                        //subtract dropped days off
                        line.DaysOff -= noOfDaysOFf;

                        //add droppedPDiem in Bp
                        var currentpdieminBp = Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp.ToString());
                        currentpdieminBp += droppedTafb;
                        line.TafbInBp = (currentpdieminBp / 60).ToString() + ":" + (currentpdieminBp % 60).ToString("d2");

                        //add droppedPDiem in line
                        var currentpdieminLine = Helper.ConvertformattedHhhmmToMinutes(line.TafbInLine.ToString());
                        currentpdieminLine += droppedTafb;
                        line.TafbInLine = (currentpdieminLine / 60).ToString() + ":" + (currentpdieminLine % 60).ToString("d2");


                        line.AcftChanges += droppedaircraftchange;
                        line.Legs += droppedlegs;
                        line.DaysWork += listDroppedTrueDays.Distinct().Count();
                        line.DaysWorkInLine += listDroppedTrueDays.Distinct().Count();
                        line.LegsIn800 += dropppedlegsin800;
                        line.LegsIn700 += dropppedlegsin700;
                        line.LegsIn500 += dropppedlegsin500;
                        line.LegsIn200 += dropppedlegsin200;
                        line.LegsIn300 += dropppedlegsin300;
                        line.LongestGrndTime = CalcLongGrndTime(line, false);
                        line.OverlapDrop = 0;
                        line.MostLegs = CalculateMostlegs(line, false);
                        line.BlkOfDaysOff = CalcBlkOfDaysOff(line);
                        line.LargestBlkOfDaysOff = CalcLargestBlkDaysOff(line);
                        listDroppedFalseDays.AddRange(listDroppedTrueDays);
                        line.TotDutyPds = listDroppedFalseDays.Distinct().Count();
                        line.TotDutyPdsInBp = listDroppedFalseDays.Where(x => x >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate && x <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate).Distinct().Count();
                        line.Trips1Day = CalcNumTripsOfGivenLength(1, line);
                        line.Trips2Day = CalcNumTripsOfGivenLength(2, line);
                        line.Trips3Day = CalcNumTripsOfGivenLength(3, line);
                        line.Trips4Day = CalcNumTripsOfGivenLength(4, line);
                        line.TotPairings = line.Pairings.Count;
                        line.OvernightCities.AddRange(droppedOvernightCities);
                        line.DaysOfWeekWork = CalcWeekDaysWork(line.DaysOfWeekWork, DroppedWeekDaysWork, false);
                        line.DaysOfMonthWorks = CalcDaysOfMonthOff(line);
                        line.Weekend = CalcWkEndProp(line);
                        line.StartDow = CalcStartDow(line);
                        CalculateWorkBlockLength(line);
                        line.Is3on3Off = CalculateIs3on3OffDay(line);
                        line.GroundTimes = CalculateGroundTimes(line);
                        line.DutyHrsInLine = CalcDutyHrs(line, true);
                        line.DutyHrsInBp = CalcDutyHrs(line, false);
                        line.LastArrTime = CalcLastArrTime(line);
                        line.LastDomArrTime = CalcLastDomArrTime(line);
                        CalculateRestPeriods(line);
                        line.EDomPush = CalcEDomPush(line).ToString(@"hh\:mm");
                        line.EPush = CalcEPush(line).ToString(@"hh\:mm"); ;
                        line.ShowOverLap = false;
                    }
                    line.AcftChgDay = line.ReserveLine || line.BlankLine ? 0 : Math.Round((line.DaysWorkInLine == 0) ? 0 : Convert.ToDecimal(line.AcftChanges) / Convert.ToDecimal(line.DaysWorkInLine), 2);
                    line.LegsPerDay = line.ReserveLine || line.BlankLine ? 0 : Math.Round((line.DaysWork == 0) ? 0 : Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.DaysWork), 2);
                    line.LegsPerPair = line.ReserveLine || line.BlankLine ? 0 : Math.Round((line.TotPairings == 0) ? 0 : Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.TotPairings), 2);
                    line.Equip8753 = CalculateEquip8753(line);
                    line.T234 = CalcT234(line);
                    CalculateAssociatedLinePropertiesfortheBig4(line);

                }


            }
			try
				{
                    SetRatioValues();
				}
				catch (Exception ex)
				{ 
				}
            //}

        }
		private void SetRatioValues()
		{
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 75) ||
				GlobalSettings.WBidINIContent.ModernNormalColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.ModernVacationColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.BidLineNormalColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.BidLineVacationColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 75) ||
				wBidStateContent.SortDetails.BlokSort.Contains("19")

				)
			{

				var numeratorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Numerator);
				var denominatorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Denominator);

				if (numeratorcolumn != null && denominatorcolumn != null)
				{
					foreach (var line in GlobalSettings.Lines)
					{
						var numerator = line.GetType().GetProperty(numeratorcolumn.DataPropertyName).GetValue(line, null);
						if (numeratorcolumn.DataPropertyName == "TafbInBp")
							numerator = (line.TafbInBp == null) ? 0 : Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
						decimal numeratorValue = Convert.ToDecimal(numerator);

						var denominator = line.GetType().GetProperty(denominatorcolumn.DataPropertyName).GetValue(line, null);
						if (denominatorcolumn.DataPropertyName == "TafbInBp")
							denominator = (line.TafbInBp == null) ? 0 : Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
						decimal denominatorValue = Convert.ToDecimal(denominator);


						line.Ratio = Math.Round(decimal.Parse(String.Format("{0:0.00}", (denominatorValue == 0) ? 0 : numeratorValue / denominatorValue)), 2);

					}
				}
			}
		}
        /// <summary>
        ///  Recalculate Big 4 properties assosiated properties
        /// </summary>
        /// <param name="line"></param>
        private void CalculateAssociatedLinePropertiesfortheBig4(Line line)
        {

            //TfpPerTafb
            if (!line.ReserveLine && !line.BlankLine)
            {
                string[] tafbTime = line.TafbInBp.Split(':');
                decimal tafbInMin = Convert.ToDecimal(tafbTime[0]) * 60 + Convert.ToDecimal(tafbTime[1]);
                line.TfpPerTafb = line.ReserveLine || line.BlankLine ? 0m : Math.Round((tafbInMin == 0) ? 0 : line.Tfp / (tafbInMin / 60m), 2);
            }

            //TfpPerDhr
            string[] dhrTime = line.DutyHrsInBp.Split(':');
            decimal dhrInMin = Convert.ToDecimal(dhrTime[0]) * 60 + Convert.ToDecimal(dhrTime[1]);
            line.TfpPerDhr = line.ReserveLine || line.BlankLine ? 0m : Math.Round((dhrInMin == 0) ? 0 : line.Tfp / (dhrInMin / 60m), 2);

            //TfpPerFltHr
            line.TfpPerFltHr = Math.Round(CalcTfpPerFltHr(line), 2);

            //TfpPerDay
            CalculateTfpPerDay(line);

            line.LegsPerDay = line.ReserveLine || line.BlankLine ? 0 :
                Math.Round((line.DaysWork == 0) ? 0 : Convert.ToDecimal(line.Legs) / Convert.ToDecimal(line.DaysWork), 2);



        }

        #region TfpPerFltHr
        private decimal CalcTfpPerFltHr(Line line)
        {
            if (line.ReserveLine || line.BlankLine) return 0.00m;
            var flighthour = (Convert.ToDecimal(Helper.ConvertformattedHhhmmToMinutes(line.BlkHrsInBp)) / 60);

            decimal tfp = (flighthour == 0) ? 0 : line.Tfp / flighthour;
            return Math.Round(Convert.ToDecimal(String.Format("{0:0.00}", tfp)), 2);
        }
        #endregion

        #region TfpPerDay
        /// <summary>
        /// PURPOSE : Calculate Tfp Per Day
        /// </summary>
        /// <param name="line"></param>
        private void CalculateTfpPerDay(Line line)
        {

            decimal tfpPerDay = 0.0m;

            if (line.ReserveLine)
            {

                tfpPerDay = (GlobalSettings.CurrentBidDetails.Postion == "FA") ? GlobalSettings.FAReserveDayPay : GlobalSettings.ReserveDailyGuarantee;
            }
            else if (line.BlankLine || line.ReserveLine)
            {
                tfpPerDay = 0.0m;
            }
            else
            {
                tfpPerDay = Math.Round((line.DaysWork == 0) ? 0 : Convert.ToDecimal(String.Format("{0:0.00}", (line.Tfp / line.DaysWork))), 2);
            }

            line.TfpPerDay = tfpPerDay;

        }
        #endregion

        #region Legs
        private int CalcNumLegsOfGivenType(int givenType, List<LineSip> droppedsip)
        {
            int numLegsOfGivenType = 0;
            try
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    numLegsOfGivenType = droppedsip.SelectMany(x => x.Sip.SipDutyPeriods.SelectMany(y => y.Flights.Where(z => z.Equip.Substring(0, 1) == givenType.ToString().Substring(0, 1)))).Count();
                }
                else
                {
                    numLegsOfGivenType = droppedsip.SelectMany(x => x.Sip.SipDutyPeriods.SelectMany(y => y.Flights.Where(z => z.Equip.Substring(1, 1) == givenType.ToString().Substring(0, 1)))).Count();
                }
            }
            catch (Exception ex)
            { }

            // GlobalSettings.CurrentBidDetails.Postion == "FA" ?flt.Equip.Substring(0, 1):flt.Equip.Substring(1, 1)
            return numLegsOfGivenType;
        }
        #endregion

        #region LongGrndTime
        private TimeSpan CalcLongGrndTime(Line line, bool isOverlapCorrection)
        {
            List<SipDutyPeriod> sipdutyperiods;
            int maxGrndTime = 0;
            int turnTime = 0;
            if (isOverlapCorrection)
            {
                sipdutyperiods = line.LineSips.Where(x => x.Dropped == false).SelectMany(y => y.Sip.SipDutyPeriods).ToList();
            }
            else
            {
                sipdutyperiods = line.LineSips.SelectMany(y => y.Sip.SipDutyPeriods).ToList();
            }


            foreach (var dp in sipdutyperiods)
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
            return new TimeSpan(maxGrndTime / 60, maxGrndTime % 60, 0);
        }
        #endregion

        #region MostLegs
        /// <summary>
        /// Calculate Most legs in a pairing.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int CalculateMostlegs(Line line, bool isOverlapCorrection)
        {
            int mostlegs = 0;
            foreach (var pairing in line.Pairings)
            {
                List<LineSip> linesip;
                if (isOverlapCorrection)
                {
                    linesip = line.LineSips.Where(x => x.Dropped == false && x.Sip.SipName.Substring(0, 4) == pairing.Substring(0, 4)).ToList();
                }
                else
                {
                    linesip = line.LineSips.Where(x => x.Sip.SipName.Substring(0, 4) == pairing.Substring(0, 4)).ToList();
                }
                int legsintrip = linesip.SelectMany(x => x.Sip.SipDutyPeriods.SelectMany(y => y.Flights)).Count();
                if (mostlegs < legsintrip)
                {
                    mostlegs = legsintrip;
                }
            }
            return mostlegs;
        }
        #endregion

        #region BlkOfDaysOff
        private List<int> CalcBlkOfDaysOffForOverlap(Line line, List<DateTime> workdays)
        {
            List<int> blkOff = new List<int>();
            for (int count = 0; count < 35; count++)
            {
                blkOff.Add(0);

            }
            DateTime startday = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            workdays = workdays.Distinct().OrderBy(x => x).ToList();
            for (int count = 0; count < workdays.Count; count++)
            {
                if (count == 0)
                {
                    blkOff[(workdays[count] - startday).Days]++;

                }
                else if (count == workdays.Count - 1)
                {
                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate > workdays[count])
                    {
                        blkOff[(GlobalSettings.CurrentBidDetails.BidPeriodEndDate - workdays[count]).Days]++;
                    }
                }
                else
                {
                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate > workdays[count + 1])
                        blkOff[(workdays[count + 1] - workdays[count]).Days - 1]++;
                }

            }
            blkOff[0] = 0;
            return blkOff;
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

            oldPairingEndDay = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.AddDays(-1);
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
				if(trip==null)
					trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();

                if (trip != null)
                {
                    // pairName = pairing.Substring(0, 4);
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                    newPairingStartDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip);
                    newPairingEndDay = newPairingStartDay.AddDays(trip.PairLength - 1);

                    if ((newPairingStartDay - oldPairingEndDay).Days != 1)
                    {
                        blkOff[(newPairingStartDay - oldPairingEndDay).Days - 1]++;
                    }

                    oldPairingStartDay = newPairingStartDay;
                    oldPairingEndDay = newPairingEndDay;
                }
            }

            // handle end of month case:  fa bpEnd can be 30, 1, 31

            //DateTime lastPairEndDate = new DateTime(int.Parse(year), int.Parse(month), oldPairingStartDay.Day);
            DateTime lastPairEndDate = WBidCollection.SetDate(oldPairingStartDay.Day, true);
            lastPairEndDate = lastPairEndDate.AddDays(trip.PairLength - 1);
            TimeSpan tempSpan = GlobalSettings.CurrentBidDetails.BidPeriodEndDate - lastPairEndDate;

            if (tempSpan.Days > 0) blkOff[tempSpan.Days]++;

            return blkOff;
        }
        #endregion

        #region LargestBlkDaysOfff
        private int CalcLargestBlkDaysOffforOverlap(Line line, List<DateTime> workdays)
        {
            int LargestBlkDaysOff = 0;
            DateTime startday = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            workdays = workdays.Distinct().OrderBy(x => x).ToList();
            for (int count = 0; count < workdays.Count; count++)
            {
                int off = 0;
                if (count == 0)
                {
                    off = (workdays[count] - startday).Days;
                }
                else if (count == workdays.Count - 1)
                {
                    if (GlobalSettings.CurrentBidDetails.BidPeriodEndDate > workdays[count])
                    {
                        off = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - workdays[count]).Days;
                    }
                }
                else
                {
                    off = (workdays[count + 1] - workdays[count]).Days - 1;
                }

                if (LargestBlkDaysOff < off)
                {
                    LargestBlkDaysOff = off;
                }
            }


            return LargestBlkDaysOff;
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

                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
                if (trip == null)
                {
                    trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
                }

                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip);

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
        #endregion

        #region NumTripsOfGivenLength
        private int CalcNumTripsOfGivenLength(int givenLength, Line line)
        {
            Trip trip = null;
            int numTripsOfGivenLength = 0;

            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);

                //  string tripName = pairing.Substring(0, 4);
                if (trip.PairLength == givenLength)
                    numTripsOfGivenLength++;
            }

            return numTripsOfGivenLength;
        }
        private int CalcNumTripsOfGivenLengthForOverLap(int givenLength, Line line, List<DateTime> droppeddays)
        {
            Trip trip = null;
            int numTripsOfGivenLength = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //get the trip start date.
                //DateTime tripstartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).TrimStart()));
                DateTime tripstartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).TrimStart()), isLastTrip);
                int tripnumber = 0;
                for (DateTime day = tripstartDate; day <= tripstartDate.AddDays(trip.PairLength - 1); day = day.AddDays(1))
                {
                    if (droppeddays.Contains(day))
                    {
                        if (tripnumber == givenLength)
                        {
                            tripnumber = 0;
                            numTripsOfGivenLength++;
                        }
                        continue;
                    }
                    else
                        tripnumber++;

                }
                if (tripnumber == givenLength)
                {
                    numTripsOfGivenLength++;
                }
            }

            return numTripsOfGivenLength;
        }
        #endregion

        #region TOtalPairings
        /// <summary>
        /// Calulate total Pairings for the Overlap
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private int CalcTotalPairingsForOverlap(Line line)
        {
            int totpairings = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                ///get the sips in a pairing
                var sipsInPairing = GetSipAssociatedForthePairing(line, pairing, isLastTrip);
                var droppedsippairingname = sipsInPairing.Where(x => x.Dropped).OrderBy(z => z.SipStartDate).Select(y => y.Sip.SipName);

                totpairings++;
                if (sipsInPairing.Count == droppedsippairingname.Count())
                {
                    totpairings--;
                }

                int lastsipnsequencenumber = 0;
                foreach (string sipname in droppedsippairingname)
                {
                    int currentsipnsequencenumber = Convert.ToInt32(sipname.Substring(5, 1));
                    //if the droppped sips are not an adjacent sip,we can increment the total pairings.or if the droppped sip is not at the first or last sip we can increment the totpairings
                    if (currentsipnsequencenumber != 1 && currentsipnsequencenumber != sipsInPairing.Count() && ((currentsipnsequencenumber - lastsipnsequencenumber) > 1))
                    {
                        totpairings++;
                    }
                    lastsipnsequencenumber = currentsipnsequencenumber;
                }

            }
            return totpairings;
        }
        #endregion

        #region Equip8753
        private string CalculateEquip8753(Line line)
        {
            return line.LegsIn700 + "-" + line.LegsIn800 + "-" +  line.LegsIn200 + "-" + line.LegsIn600;
        }
        #endregion

        #region T234
        private string CalcT234(Line line)
        {
            return line.Trips1Day.ToString() + line.Trips2Day.ToString() + line.Trips3Day.ToString() + line.Trips4Day;
        }
        #endregion

        #region DutyHours
        private string CalcDutyHrsForOverlap(Line line)
        {
            int dutyhours = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                int date = Convert.ToInt16(pairing.Substring(4, 2));
                //var sipsnPairing = GetSipAssociatedForthePairing(line, pairing);
                Trip trip = GetTrip(pairing);
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                //DateTime tripstartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).TrimStart()));
                DateTime tripstartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).TrimStart()), isLastTrip);
                var sipsnPairing = line.LineSips.Where(x => x.Sip.SipName.Substring(0, 4) == pairing.Substring(0, 4) && x.SipStartDate >= tripstartDate && x.SipStartDate <= tripstartDate.AddDays(trip.DutyPeriods.Count - 1)).ToList();

                int dpLastFltLandTime = 0;
                int dpFirstFltDepTime = 0;
                bool isprevioussipdropped = false;
                DateTime previousSipEndDate = DateTime.MinValue;
                int sipcount = 0;
                bool isfirstDutyperiod = true;
                foreach (LineSip sip in sipsnPairing)
                {
                    sipcount++;

                    if (sip.Dropped)
                    {
                        int dutytime = 0;
                        if (isfirstDutyperiod || isprevioussipdropped)
                        {
                            if (isprevioussipdropped)
                            {
                                dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (trip.BriefTime + GlobalSettings.debrief));
                            }
                            // dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.show1stDay + GlobalSettings.debrief));
                        }
                        else
                        {
                            dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.showAfter1stDay + GlobalSettings.debrief));
                        }
                        dutyhours += dutytime;
                        isprevioussipdropped = true;
                        continue;
                    }

                    DateTime sipenddate = sip.SipStartDate.AddDays(sip.Sip.SipDutyPeriods.Count - 1);

                    if (sip.Sip.SipDutyPeriods.Count == 1)
                    {
                        var dutyperiod = sip.Sip.SipDutyPeriods[0];

                        if (previousSipEndDate != sip.SipStartDate || isprevioussipdropped)
                        {
                            dpFirstFltDepTime = dutyperiod.Flights[0].DepTime;
                        }
                        dpLastFltLandTime = dutyperiod.Flights[dutyperiod.Flights.Count - 1].ArrTime;

                        int dutytime = 0;
                        if (sipsnPairing.Count == sipcount)
                        {
                            if (isfirstDutyperiod || isprevioussipdropped)
                            {
                                //dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.show1stDay + GlobalSettings.debrief));
                                dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (trip.BriefTime + GlobalSettings.debrief));
                            }
                            else
                            {
                                dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.showAfter1stDay + GlobalSettings.debrief));
                            }
                            dutyhours += dutytime;
                        }
                    }
                    else
                    {
                        int sipdpcount = 0;
                        bool isfirstflightinDP = false;
                        var sipdutyperiods = sip.Sip.SipDutyPeriods;
                        foreach (SipDutyPeriod dp in sipdutyperiods)
                        {
                            sipdpcount++;
                            if (previousSipEndDate != sip.SipStartDate)
                            {
                                dpFirstFltDepTime = dp.Flights[0].DepTime;
                            }
                            if (isfirstflightinDP)
                            {
                                dpFirstFltDepTime = dp.Flights[0].DepTime;
                            }
                            dpLastFltLandTime = dp.Flights[dp.Flights.Count - 1].ArrTime;
                            if (sipdpcount < sipdutyperiods.Count || sipsnPairing.Count == sipcount)
                            {
                                int dutytime = 0;
                                if (isfirstDutyperiod || isprevioussipdropped)
                                {
                                    // dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.show1stDay + GlobalSettings.debrief));
                                    dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (trip.BriefTime + GlobalSettings.debrief));
                                }
                                else
                                {
                                    dutytime = (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.showAfter1stDay + GlobalSettings.debrief));
                                }
                                dutyhours += dutytime;
                                isfirstflightinDP = true;
                                isfirstDutyperiod = false;
                            }
                        }
                    }
                    isprevioussipdropped = false;
                    previousSipEndDate = sipenddate;

                }
            }
            return (dutyhours / 60).ToString() + ":" + (dutyhours % 60).ToString();

        }

        private string CalcDutyHrs(Line line, bool inLine)
        {
            Trip trip = null;
            int dutyHrs;
            dutyHrs = 0;
            foreach (var pairing in line.Pairings)
            {
                trip = GetTrip(pairing);
                // string tripName = pairing.Substring(0, 4);
                int date = Convert.ToInt16(pairing.Substring(4, 2));
                foreach (var dp in trip.DutyPeriods)
                {
                    if (!inLine)        // if inLine is false then only dutyHrs in Bid Period will be accumulated
                    {
                        if (date < GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Day - 4)
                            dutyHrs += dp.DutyTime;
                        else
                            if (date + dp.DutPerSeqNum - 1 <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Day)
                                dutyHrs += dp.DutyTime;
                    }
                    else
                        dutyHrs += dp.DutyTime;
                }
            }
            return (dutyHrs / 60).ToString() + ":" + (dutyHrs % 60).ToString();
            //return dutyHrs;
        }
        #endregion

        #region DaysOfweekWork
        /// <summary>
        /// Calculate Week Days of Works.
        /// </summary>
        /// <param name="originalWorkdays"></param>
        /// <param name="droppedWeekDaysWork"></param>
        /// <param name="isOverlap"></param>
        /// <returns></returns>
        private List<int> CalcWeekDaysWork(List<int> originalWorkdays, int[] droppedWeekDaysWork, bool isOverlap)
        {
            for (int count = 0; count < originalWorkdays.Count(); count++)
            {

                if (isOverlap)
                {
                    originalWorkdays[count] = originalWorkdays[count] - droppedWeekDaysWork[count];
                }
                else
                {
                    originalWorkdays[count] = originalWorkdays[count] + droppedWeekDaysWork[count];
                }

            }

            return originalWorkdays;
        }
        #endregion

        #region DaysOfMonthOff
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
                //Get trip
                trip = GetTrip(pairing);
                
                int lengthTrip = trip.PairLength;
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                wkDay = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2)), isLastTrip);
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
        #endregion

        #region WkEnd
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
                for (int index = 0; index < tripLength; index++)
                {
                    tripDate = WBidCollection.SetDate(tripDay, isLastTrip);
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

        private string CalcWkEndPropforOverlap(List<DateTime> workingDays)
        {
            int wkEndCount = 0, totDays = 0, dayOfWeek = 0;
            string wkDayWkEnd = string.Empty;
            foreach (DateTime date in workingDays)
            {
                dayOfWeek = (int)date.DayOfWeek;
                if (dayOfWeek == 0 || dayOfWeek == 6)
                {
                    wkEndCount++;
                }
            }
            totDays = workingDays.Count;

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

        #region StartDow
        private string CalcStartDowForOverlap(Line line, List<DateTime> workingDays)
        {
            string sdow = " "; ;
            int startDowInt = 0;
            int oldDowInt = 9;
            DateTime prevDate = DateTime.MinValue;
            workingDays = workingDays.Distinct().ToList();
            foreach (DateTime date in workingDays)
            {
                if (date != prevDate)
                {
                    startDowInt = Convert.ToInt32(date.DayOfWeek);
                    oldDowInt = oldDowInt == 9 ? startDowInt : oldDowInt;

                }
                prevDate = date.AddDays(1);

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
                //trip = (trips.ContainsKey(pairing.Substring(0, 4))) ? trips[pairing.Substring(0, 4)] : trips[pairing];
                trip = GetTrip(pairing);
                // to calculate Tafb in BP I need to move day by day and demarcate the 24:00 time on last day of bid period -- not done!


                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


                // string tripName = pairing.Substring(0, 4);
                int date = Convert.ToInt16(pairing.Substring(4, 2));
                int lenghtOfTrip = trip.PairLength;
                if (date != nextDate)
                {
                    nextDate = date + lenghtOfTrip;

                    //startDowInt = Convert.ToInt32(new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), date).DayOfWeek);
                    startDowInt = Convert.ToInt32(WBidCollection.SetDate(date, isLastTrip).DayOfWeek);
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
        #endregion

        #region WorkBlockLength
        private void CalculateWorkBlockLengthForOverlap(Line line, List<DateTime> workingDays)
        {
            int workBlockLength = 0;
            line.WorkBlockLengths = new List<int>();
            for (int count = 0; count < 6; count++)
                line.WorkBlockLengths.Add(0);
            workingDays = workingDays.Distinct().ToList();
            for (int count = 0; count < workingDays.Count; count++)
            {
                workBlockLength++;
                if (workingDays.Count == count + 1)
                {
                    line.WorkBlockLengths[workBlockLength]++;
                }
                else if (workingDays[count].AddDays(1) != workingDays[count + 1])
                {
                    line.WorkBlockLengths[workBlockLength]++;
                    workBlockLength = 0;
                }
            }
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
                trip = GetTrip(pairing);
                tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);

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

       
        #endregion

        #region LastArrTime
        private TimeSpan CalcLastArrTimeforOverlap(Line line)
        {
            int arrTime = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                var sipsInPairing = GetSipAssociatedForthePairing(line, pairing, isLastTrip).Where(x => x.Dropped == false);
                foreach (LineSip lineSip in sipsInPairing)
                {
                    var sip = lineSip.Sip;
                    for (int count = 0; count < sip.SipDutyPeriods.Count; count++)
                    {
                        var sipdutyperiod = sip.SipDutyPeriods[count];
                        //check if it is last dutyperiod
                        if (sip.SipDutyPeriods.Count == count + 1)
                        {
                            int lastFlt = sipdutyperiod.Flights.Count - 1;
                            arrTime = (sipdutyperiod.Flights[lastFlt].ArrTime - 1440 * sipdutyperiod.SdpSeqNum > arrTime) ? sipdutyperiod.Flights[lastFlt].ArrTime - sipdutyperiod.SdpSeqNum * 1440 : arrTime;

                        }
                        else
                        {
                            //to check next sipdutyperiod is in the next dutyperiod(we need to identify whether the current sip duperiod is at the end of the day)
                            if (sip.SipDutyPeriods[count + 1].SdpSeqNum - sipdutyperiod.SdpSeqNum >= 1)
                            {
                                int lastFlt = sipdutyperiod.Flights.Count - 1;
                                arrTime = (sipdutyperiod.Flights[lastFlt].ArrTime - 1440 * sipdutyperiod.SdpSeqNum > arrTime) ? sipdutyperiod.Flights[lastFlt].ArrTime - sipdutyperiod.SdpSeqNum * 1440 : arrTime;
                            }
                        }
                    }

                }

            }
            line.LastArrivalTime = arrTime;
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            return new TimeSpan(hours % 24, minutes, 0);

        }
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

            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            return new TimeSpan(hours % 24, minutes, 0);

        }
        #endregion

        #region LastDomicileArrivaltime

        private TimeSpan CalcLastDomicileArrTimeforOverlap(Line line)
        {
            int arrTime = 0;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                List<LineSip> sipsInPairing = GetSipAssociatedForthePairing(line, pairing, isLastTrip).Where(x => x.Dropped == false).ToList();
                if (sipsInPairing.Count > 0)
                {
                    var lastSipDutyperiod = sipsInPairing[sipsInPairing.Count() - 1].Sip.SipDutyPeriods;
                    var lastFlight = lastSipDutyperiod[lastSipDutyperiod.Count - 1].Flights[lastSipDutyperiod[lastSipDutyperiod.Count - 1].Flights.Count - 1];
                    int lastdutyperiodseqNo = lastSipDutyperiod[lastSipDutyperiod.Count - 1].SdpSeqNum;
                    arrTime = (lastFlight.ArrTime - 1440 * lastdutyperiodseqNo > arrTime) ? lastFlight.ArrTime - lastdutyperiodseqNo * 1440 : arrTime;
                }
            }
            int hours = arrTime / 60;
            int minutes = arrTime % 60;
            return new TimeSpan(hours % 24, minutes, 0);

        }

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
            return new TimeSpan(hours % 24, minutes, 0);
        }
        #endregion

        #region RestPeriods

        /// <summary>
        /// PURPOSE : Calculate Rest Periods
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private void CalculateRestPeriodsForOverlap(Line line)
        {
            List<RestPeriod> lstRestPeriod = new List<RestPeriod>();

            Trip trip = null;
            int periodid = 1;
            RestPeriod restPeriod = null;
            int count = 0;
            bool IsInTrip = false;
            DateTime lastDutyEndDate = new DateTime();
            DateTime currentDutyStartTime = new DateTime();
            bool isLastTrip = false; int paringCount = 0;
            lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                var sipsInPairing = GetSipAssociatedForthePairing(line, pairing, isLastTrip);
                count = 0;
                int rpt = 0;
                int rls = GlobalSettings.debrief;
                bool isprevioussipdropped = false;
                bool isfirstSipInDutyperiod = true;
                int linesipcount = 0;
                DateTime sipStartDay = new DateTime();
                foreach (LineSip lineSip in sipsInPairing)
                {
                    linesipcount++;
                    if (lineSip.Dropped)
                    {
                        if (linesipcount > 1)
                        {
                            var previoussip = sipsInPairing[linesipcount - 2];
                            var lastsipdutyperiod = previoussip.Sip.SipDutyPeriods[previoussip.Sip.SipDutyPeriods.Count - 1];
                            lastDutyEndDate = sipStartDay.AddMinutes(lastsipdutyperiod.Flights[lastsipdutyperiod.Flights.Count - 1].ArrTime - (lastsipdutyperiod.SdpSeqNum * 1440) + rls);
                        }
                        isprevioussipdropped = true;
                        IsInTrip = false;
                        continue;
                    }

                    sipStartDay = lineSip.SipStartDate;
                    var sip = lineSip.Sip;


                    for (int index = 0; index < sip.SipDutyPeriods.Count; index++)
                    {
                        var sipdutyperiod = sip.SipDutyPeriods[index];
                        if (isfirstSipInDutyperiod || isprevioussipdropped)
                        {
                            //rpt = (count == 0 || isprevioussipdropped) ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;
                            rpt = (count == 0 || isprevioussipdropped) ? trip.BriefTime : GlobalSettings.showAfter1stDay;

                            currentDutyStartTime = sipStartDay.AddMinutes(sipdutyperiod.Flights[0].DepTime - (sipdutyperiod.SdpSeqNum * 1440) - rpt);
                            restPeriod = new RestPeriod();
                            restPeriod.PeriodId = periodid++;
                            restPeriod.IsInTrip = IsInTrip;
                            restPeriod.RestMinutes = CalculateTimeDifference(lastDutyEndDate, currentDutyStartTime);
                            lstRestPeriod.Add(restPeriod);
                            count++;
                            isfirstSipInDutyperiod = false;
                        }
                        //check if it is last dutyperiod
                        if (sip.SipDutyPeriods.Count == index + 1)
                        {
                            //end of the last sip 
                            if (linesipcount == sipsInPairing.Count)
                            {
                                lastDutyEndDate = sipStartDay.AddMinutes(sipdutyperiod.Flights[sipdutyperiod.Flights.Count - 1].ArrTime - (sipdutyperiod.SdpSeqNum * 1440) + rls);
                                isfirstSipInDutyperiod = true;
                            }
                        }
                        else
                        {
                            //to check next sipdutyperiod is in the next dutyperiod(we need to identify whether the current sip duperiod is at the end of the day)
                            if (sip.SipDutyPeriods[index + 1].SdpSeqNum - sipdutyperiod.SdpSeqNum >= 1)
                            {
                                lastDutyEndDate = sipStartDay.AddMinutes(sipdutyperiod.Flights[sipdutyperiod.Flights.Count - 1].ArrTime - (sipdutyperiod.SdpSeqNum * 1440) + rls);
                                isfirstSipInDutyperiod = true;
                                //Finding the status ,is in trip or 'between trip'
                                IsInTrip = (sipdutyperiod.Flights[sipdutyperiod.Flights.Count - 1].ArrSta != GlobalSettings.CurrentBidDetails.Domicile);
                                sipStartDay = sipStartDay.AddDays(1);
                            }
                            else
                                isfirstSipInDutyperiod = false;
                        }

                    }
                }
            }
            //Remove first rest period before the first trip
            if (lstRestPeriod.Count > 0)
            {
                lstRestPeriod.RemoveAt(0);
            }

            line.RestPeriods = lstRestPeriod;
        }
        /// <summary>
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
            bool isLastTrip = false; int paringCount = 0;
            lastDutyEndDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                trip = GetTrip(pairing);
                //DateTime tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).Trim(' ')));
                DateTime tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim(' ')), isLastTrip);
                count = 0;
                int rpt = 0;
                int rls = GlobalSettings.debrief;
                foreach (var dp in trip.DutyPeriods)
                {
                    //if not the first dutyperiod then we need to add one  to the tripstartdate
                    if (count != 0)
                    {
                        tripStartDate = tripStartDate.AddDays(1);

                    }
                    //rpt = (count == 0) ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;
                    rpt = (count == 0) ? trip.BriefTime : GlobalSettings.showAfter1stDay;

                    restPeriod = new RestPeriod();
                    restPeriod.PeriodId = periodid++;
                    restPeriod.IsInTrip = IsInTrip;
                    restPeriod.RestMinutes = CalculateTimeDifference(lastDutyEndDate, tripStartDate.AddMinutes(dp.DepTimeFirstLeg - (count * 1440) - rpt));

                    //Temporary variable  for testing. Should remove later
                    // restPeriod.Time = Math.Round((double.Parse(restPeriod.RestMinutes.ToString()) / 60), 2).ToString();

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
        #endregion

        #region 3on3off

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
                    trip = GetTrip(pairing);
                    //DateTime tripstartdate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, Convert.ToInt16(pairing.Substring(4, 2)));
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
                    line.Is3on3Off = true;
                }
            }


            return Is3on3off;
        }
        private bool CalculateIs3on3OffDayFOrOverlap(Line line, List<DateTime> workingdays)
        {
            bool status = true;
            if (line.Trips1Day == 0 && line.Trips2Day == 0 && line.Trips4Day == 0)
            {
                for (int count = 2; count < workingdays.Count - 1; count = count + 3)
                {
                    if (workingdays[count].AddDays(4) != workingdays[count + 1])
                    {
                        status = false;
                        break;
                    }
                }
            }
            return status;
        }
        #endregion

        #region GroundTimes
        private List<int> CalculateGroundTimesForOverlap(Line line)
        {
            bool isLastTrip = false; int paringCount = 0;
            List<int> groundtimes = new List<int>();
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                var sipsnPairing = GetSipAssociatedForthePairing(line, pairing, isLastTrip).Where(x => x.Dropped == false);
                int count = 0;
                int previousflightarrivaltime = 0;
                int previoussipnsequencenumber = 1;
                foreach (LineSip sip in sipsnPairing)
                {
                    int currentsipnsequencenumber = Convert.ToInt32(sip.Sip.SipName.Substring(5, 1));

                    bool isfirstsipdutyperiod = true;
                    foreach (SipDutyPeriod dp in sip.Sip.SipDutyPeriods)
                    {

                        if (!(isfirstsipdutyperiod && currentsipnsequencenumber - previoussipnsequencenumber == 1))
                        {
                            previousflightarrivaltime = 0;
                        }

                        foreach (Flight flight in dp.Flights)
                        {
                            int currentflightdeparture = flight.DepTime;
                            int currentflightarrival = flight.ArrTime;
                            if (previousflightarrivaltime != 0)
                            {
                                int turntime = currentflightdeparture - previousflightarrivaltime;
                                groundtimes.Add(turntime);
                            }
                            previousflightarrivaltime = currentflightarrival;
                            count++;
                        }

                        isfirstsipdutyperiod = false;
                    }
                }
            }
            return groundtimes;
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
                trip = GetTrip(pairing);

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
        #endregion

        #region EPush
        private TimeSpan CalcEPushForOverLap(Line line)
        {
            int ePush = 99999999;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                var sipsInPairing = GetSipAssociatedForthePairing(line, pairing, isLastTrip);
                bool ispreviousSipdropped = true;
                bool isStartOftheDay = true;
                foreach (LineSip lineSip in sipsInPairing)
                {
                    if (lineSip.Dropped)
                    {
                        ispreviousSipdropped = true;
                    }
                    else
                    {
                        int count = 0;
                        foreach (var dp in lineSip.Sip.SipDutyPeriods)
                        {
                            if (ispreviousSipdropped || isStartOftheDay)
                            {
                                ePush = (dp.Flights[0].DepTime - 1440 * dp.SdpSeqNum < ePush) ? dp.Flights[0].DepTime - dp.SdpSeqNum * 1440 : ePush;
                            }

                            if (lineSip.Sip.SipDutyPeriods.Count - 1 > count)
                            {
                                //check the urrent Sip duty period is end of the day.If so next dutuperiod is the start of the day.
                                if ((lineSip.Sip.SipDutyPeriods[count + 1].SdpSeqNum) == (lineSip.Sip.SipDutyPeriods[count].SdpSeqNum + 1))
                                {
                                    isStartOftheDay = true;
                                }
                                else
                                    isStartOftheDay = false;
                            }

                            count++;
                        }
                        ispreviousSipdropped = false;
                    }
                }
            }

            int hours = ePush / 60;
            int minutes = ePush % 60;
            return new TimeSpan(hours, minutes, 0);
        }
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
        #endregion

        #region EDomPush
        private TimeSpan CalcEDomPushForOverlap(Line line)
        {
            int ePush = 99999999;
            bool isLastTrip = false; int paringCount = 0;
            foreach (var pairing in line.Pairings)
            {
                isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
                var sipsInPairing = GetSipAssociatedForthePairing(line, pairing, isLastTrip);
                bool ispreviousSipdropped = true;
                int count = 0;
                foreach (LineSip lineSip in sipsInPairing)
                {
                    if (lineSip.Dropped)
                    {
                        ispreviousSipdropped = true;
                    }
                    else
                    {
                        //we need to calculate edompush if the previous sip droppped or the if it is the first dutyperiod in the pairing.
                        if (ispreviousSipdropped || count == 0)
                        {
                            ePush = (lineSip.Sip.SipDutyPeriods[0].Flights[0].DepTime < ePush) ? lineSip.Sip.SipDutyPeriods[0].Flights[0].DepTime : ePush;
                            count++;
                        }
                        ispreviousSipdropped = false;
                    }
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
                trip = GetTrip(pairing);
                ePush = (trip.DutyPeriods[0].Flights[0].DepTime < ePush) ? trip.DutyPeriods[0].Flights[0].DepTime : ePush;
            }

            int hours = ePush / 60;
            int minutes = ePush % 60;
            return new TimeSpan(hours, minutes, 0);

        }
        #endregion


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
        /// <summary>
        /// Get the sips in the Pairings.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pairing"></param>
        /// <returns></returns>
        private List<LineSip> GetSipAssociatedForthePairing(Line line, string pairing, bool islastTrip)
        {
            //Get trip
            Trip trip = GetTrip(pairing);
            //DateTime tripstartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, int.Parse(pairing.Substring(4, 2).TrimStart()));
            DateTime tripstartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).TrimStart()), islastTrip);
            return line.LineSips.Where(x => x.Sip.SipName.Substring(0, 4) == pairing.Substring(0, 4) && x.SipStartDate >= tripstartDate && x.SipStartDate <= tripstartDate.AddDays(trip.DutyPeriods.Count - 1)).ToList();
        }
        /// <summary>
        /// Get the droped overnight cities
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private List<string> GetDroppedOvernightCities(Line line)
        {
            List<string> droppedOvernightCities = new List<string>();
            var droppeddutyperiods = line.LineSips.Where(x => x.Dropped).SelectMany(y => y.Sip.SipDutyPeriods);
            foreach (SipDutyPeriod sdp in droppeddutyperiods)
            {
                string lastleg = sdp.Flights[sdp.Flights.Count - 1].ArrSta;
                if (lastleg != GlobalSettings.CurrentBidDetails.Domicile)
                {
                    droppedOvernightCities.Add(lastleg);
                }
            }
            return droppedOvernightCities;
        }
        /// <summary>
        /// Get the Droppped working days in week.
        /// </summary>
        /// <param name="droppedOverlappeddays"></param>
        /// <returns></returns>
        private int[] GetDroppedWeekDaysWork(List<DateTime> droppedOverlappeddays)
        {
            int dayOfWeek;
            int[] weekWorkingDays = new int[Enum.GetNames(typeof(Dow)).Length];
            foreach (DateTime droppedday in droppedOverlappeddays)
            {
                dayOfWeek = (int)droppedday.DayOfWeek - 1;
                dayOfWeek = (dayOfWeek == -1) ? 6 : dayOfWeek;
                weekWorkingDays[dayOfWeek]++;

            }
            return weekWorkingDays;

        }
        
    }
}
