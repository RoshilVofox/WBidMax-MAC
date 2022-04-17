#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility; 
#endregion

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class CalculateTripProperties
    {
        #region Variables
        private Dictionary<string, Trip> trips;

        #endregion

        #region Public Methods
        /// <summary>
        /// Calaculate property values in the Trip files
        /// </summary>
        /// <param name="tripdata"></param>
        /// <param name="ListCityPair"></param>
        public void CalculateTripPropertyValues(Dictionary<string, Trip> tripdata, List<CityPair> ListCityPair)
        {

            if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO")
            {
                UpdatePropertyValuesforPilot(tripdata, ListCityPair);
            }
            else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
            {
                UpdatePropertyValuesforFlightAttend(tripdata, ListCityPair);
            }
            CalculateTripSips(tripdata);

        }

        public void SetDSTProperties()
        {
            int countMarSundays = 0;
            int countNovSundays = 0;
            for (int d = 1; d < 14; d++)
            {
                DateTime valueMar = new DateTime(DateTime.Now.Year, 3, d);
                if (valueMar.DayOfWeek == 0)
                {
                    countMarSundays++;
                    if (countMarSundays == 2)
                    {
                        GlobalSettings.FirstDayOfDST = new DateTime(DateTime.Now.Year, 3, d);
                        return;
                    }
                }
                DateTime valueNov = new DateTime(DateTime.Now.Year, 11, d);
                if (valueNov.DayOfWeek == 0 && countNovSundays == 0)
                {
                    countNovSundays++;
                    GlobalSettings.LastDayOfDST = new DateTime(DateTime.Now.Year, 11, d);
                }
            }
        }

        public void PreProcessFaTrips(Dictionary<string, Trip> trips, List<CityPair> ListCityPair)
        {
            // this gets the trips dictionary in shape so the CalculateTripProperties can run successfully.
            try
            {

                foreach (var t in trips)
                {

                    if (t.Value.TripNum == "XR22")
                    {

                    }

                    int totDutPds = t.Value.DutyPeriods.Count;
                    t.Value.TotDutPer = totDutPds;
                    int dutPerSeqNum = 0;
                    foreach (var dp in t.Value.DutyPeriods)
                    {
                        dutPerSeqNum++;
                        int totFlts = dp.Flights.Count;
                        decimal dpTfpByDistance = 0m;
                        decimal dpTfpByTime = 0m;
                        dp.TotFlights = totFlts;
                        dp.TripNum = t.Value.TripNum;
                        int totBlock = 0;
                        string arrLastLeg = "";
                        foreach (var flt in dp.Flights)
                        {
                            // calc block, calc tfp by time, calc tfp by dist
                            flt.Block = flt.ArrTime - flt.DepTime;
                            totBlock += flt.Block;
                            arrLastLeg = flt.ArrSta;

                            dpTfpByTime += flt.TfpByTime = Math.Round((totBlock < 55 ? 1m : 1 + (totBlock - 55) % 50 / 50m), 2);
                            CityPair citypair = null;
                            if (ListCityPair != null)
                            {
                                citypair = ListCityPair.FirstOrDefault(x => (x.City1 == flt.DepSta && x.City2 == flt.ArrSta) || (x.City1 == flt.ArrSta && x.City2 == flt.DepSta));
                            }
                            if (citypair != null)
                            {
                                dpTfpByDistance += flt.TfpByDistance = citypair.Distance;
                            }
                            else
                            {
                                // todo: message box for missing city pair
                            }
                            flt.Tfp = Math.Round(Math.Max(flt.TfpByDistance, flt.TfpByTime), 2);
                        }
                        dp.Block = Convert.ToInt16((totBlock / 60).ToString() + (totBlock % 60).ToString());
                        dp.TfpByDistance = dpTfpByDistance;
                        dp.TfpByTime = dpTfpByTime;
                        dp.Tfp = Math.Max(5.0m, Math.Max(dpTfpByDistance, dpTfpByTime));
                        dp.ArrStaLastLeg = arrLastLeg;
                        dp.DutPerSeqNum = dutPerSeqNum;
                    }
                    t.Value.Block = t.Value.DutyPeriods.Select(x => x.Block).Sum();
                    // t.Value.BriefTime = GlobalSettings.show1stDay;
                    t.Value.DebriefTime = GlobalSettings.debrief;
                    t.Value.DepFltNum = t.Value.DutyPeriods[0].Flights[0].FltNum.ToString();
                    t.Value.DepSta = t.Value.DutyPeriods[0].Flights[0].DepSta;
                    t.Value.DepTime = Helper.ConvertMinutesToHhhmm(t.Value.DutyPeriods[0].Flights[0].DepTime).Replace(":", "");
                    t.Value.DepTime1stDutPer = Convert.ToInt16(t.Value.DepTime);
                    t.Value.DepTimeLastDutper = t.Value.DutyPeriods[t.Value.DutyPeriods.Count - 1].Flights[0].DepTime;
                    t.Value.PairLength = t.Value.DutyPeriods.Count;
                    Flight flight = t.Value.DutyPeriods[t.Value.PairLength - 1].Flights[t.Value.DutyPeriods[t.Value.PairLength - 1].Flights.Count - 1];
                    t.Value.RetFltNum = flight.FltNum.ToString();
                    t.Value.RetSta = flight.ArrSta;
                    t.Value.RetTime = Helper.ConvertMinutesToHhhmm(flight.ArrTime - (1440 * (t.Value.DutyPeriods.Count - 1))).Replace(":", "");
                    t.Value.Tfp = t.Value.DutyPeriods.Select(x => x.Tfp).Sum();
                    t.Value.Tfp = Math.Max(t.Value.Tfp, t.Value.DutyPeriods.Count * 6.5m);  // calculates adg
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Calculate SipList
        /// </summary>
        private void CalculateTripSips(Dictionary<string, Trip> tripdata)
        {
            int dutyperiodcount = 0;
            int sipFltHrs = 0;
            decimal sipTfp = 0m;
            foreach (var t in tripdata)
            {
                Sip newSip = new Sip();
                SipDutyPeriod sdp = new SipDutyPeriod();
                int sipCount = 1;
                dutyperiodcount = 0;
                foreach (var dp in t.Value.DutyPeriods)
                {

                    foreach (var f in dp.Flights)
                    {
                        if (f.DepSta == GlobalSettings.CurrentBidDetails.Domicile)                      // start of Sip
                        {
                            // take care or current sip items
                            newSip = new Sip();
                            sdp = new SipDutyPeriod();
                            sipFltHrs = 0;
                            sipTfp = 0m;
                            newSip.SipName = t.Value.TripNum + "-" + sipCount.ToString();
                            sipCount++;

                            newSip.SipStartDay = dutyperiodcount;
                            sdp.FlightHours += f.Block;
                            sdp.Flights.Add(f);

                            sipFltHrs = f.Block;
                            sipTfp = f.Tfp;
                        }
                        else if (f.ArrSta == GlobalSettings.CurrentBidDetails.Domicile)                 // end of Sip
                        {
                            // take care of current sip items
                            sdp.SdpSeqNum = dutyperiodcount;
                            sdp.Flights.Add(f);
                            sipFltHrs += f.Block;
                            sipTfp += f.Tfp;
                            sdp.FlightHours += f.Block;
                            newSip.SipDutyPeriods.Add(sdp);
                            newSip.SipFltHrs = sipFltHrs;
                            newSip.SipTfp = sipTfp;

                            //Sip Tafb calculation
                            //=====================================
                            var toaldutyperiodsinsip = newSip.SipDutyPeriods.Count;
                            var toalflightsinsip = newSip.SipDutyPeriods[toaldutyperiodsinsip - 1].Flights.Count;
                            newSip.SipTAFB = newSip.SipDutyPeriods[newSip.SipDutyPeriods.Count - 1].Flights[toalflightsinsip - 1].ArrTime - newSip.SipDutyPeriods[0].Flights[0].DepTime + 90;
                            //=====================================

                            t.Value.SipsList.Add(newSip);
                        }
                        else                                                                            // middle of Sip
                        {
                            sdp.FlightHours += f.Block;
                            sdp.Flights.Add(f);
                            sipFltHrs += f.Block;
                            sipTfp += f.Tfp;

                        }
                    }
                    // end of f loop
                    sdp.SdpSeqNum = dutyperiodcount;
                    if (t.Value.DutyPeriods.Count != dp.DutPerSeqNum)         // this insures we don't add an empty SipDutyPeriod at the end
                        newSip.SipDutyPeriods.Add(sdp);
                    sdp = new SipDutyPeriod();
                    dutyperiodcount++;
                }  // end of dp loop
            }  // end of t loop
        }

        /// <summary>
        /// Update Flt,Tfp,Blk,AcftChg,RigFlt properties for Pilots
        /// </summary>
        private void UpdatePropertyValuesforPilot(Dictionary<string, Trip> tripdata, List<CityPair> ListCityPair)
        {
            decimal tripTfpByDistance, tripTfpByTime, tripTfp;
            int tripFirstFltDepTime, tripLastFltLandTime, totalLegs = 0;


            foreach (var record in tripdata)
            {
                if (record.Key.Contains("BW2B"))
                {
                }

                // if (record.Key.Substring(1, 1) != "W" && record.Key.Substring(1, 1) != "Y" && record.Key.Substring(1, 1) != "R")
                if (!record.Value.ReserveTrip)
                {

                    tripFirstFltDepTime = tripLastFltLandTime = totalLegs = 0;
                    tripTfpByDistance = tripTfpByTime = tripTfp = 0m;
                    int tripBlock = 0;
                    int firstFlight = 0;
                    int flightSeqNum = 0;
                    foreach (var dp in record.Value.DutyPeriods)
                    {
                        decimal dpTfpByDistance, dpTfpByTime, dpTfp, dpTfpByDutyHrs;
                        dpTfpByDistance = dpTfpByTime = dpTfp = dpTfpByDutyHrs = 0;
                        int dpBlock, dpDutyTime, dpFirstFltDepTime, dpLastFltLandTime;
                        dpBlock = dpDutyTime = dpFirstFltDepTime = dpLastFltLandTime = 0;
                        totalLegs += dp.Flights.Count;
                        //dp.ShowTime = dp.Flights[0].DepTime - (record.Value.BriefTime / 100) * 60;
                        if (dp.DutPerSeqNum == 1)
                        {
                            dp.ShowTime = dp.Flights[0].DepTime - record.Value.BriefTime;
                        }
                        else
                        {
                            dp.ShowTime = dp.Flights[0].DepTime - GlobalSettings.showAfter1stDay;
                        }

                        flightSeqNum = 0;
                        foreach (var flt in dp.Flights)
                        {
                            flt.FlightSeqNum = ++flightSeqNum;
                            //flt.TurnTime = flightSeqNum <= dp.Flights.Count ? // true includes all flights but last flight
                            //    dp.Flights[flightSeqNum].DepTime - dp.Flights[flightSeqNum - 1].ArrTime :
                            //    dp.DutPerSeqNum == record.Value.DutyPeriods.Count ? // handles last duty period if true
                            //        flt.TurnTime = 0 : // last flight in last duty period in trip
                            //        record.Value.DutyPeriods[dp.DutPerSeqNum + 1].Flights[0].DepTime - dp.Flights[flightSeqNum - 1].ArrTime;
                            if (flightSeqNum < dp.Flights.Count)
                            {
                                flt.TurnTime = dp.Flights[flightSeqNum].DepTime - dp.Flights[flightSeqNum - 1].ArrTime;
                            }
                            else if (dp.DutPerSeqNum == record.Value.DutyPeriods.Count)
                            {
                                flt.TurnTime = 0;
                            }
                            else
                            {
                                flt.TurnTime = record.Value.DutyPeriods[dp.DutPerSeqNum].Flights[0].DepTime - dp.Flights[flightSeqNum - 1].ArrTime;
                            }
                            string depCity = flt.DepSta;
                            string arrCity = flt.ArrSta;
                            if (dpFirstFltDepTime == 0) dpFirstFltDepTime = flt.DepTime;
                            if (tripFirstFltDepTime == 0) tripFirstFltDepTime = flt.DepTime;
                            dpLastFltLandTime = flt.ArrTime;
                            tripLastFltLandTime = flt.ArrTime;
                            var citypair = ListCityPair.FirstOrDefault(x => (x.City1 == depCity && x.City2 == arrCity) || (x.City1 == arrCity && x.City2 == depCity));
                            if (citypair != null)
                            {
                                dpTfpByDistance += flt.TfpByDistance = citypair.Distance;
                            }
                            else
                            {
                                // todo: message box for missing city pair
                            }
                            //dpTfpByDistance += flt.TfpByDistance = CityPairsDistanceData[depCity + arrCity];
                            // do not add deadheads (added by frank 2/28/2013)
                            flt.Block = flt.ArrTime - flt.DepTime;
                            dpBlock += flt.DeadHead ? 0 : flt.Block;
                            dpTfpByTime += flt.TfpByTime = Math.Round((flt.Block > 55 ? (1 + (flt.Block - 55) / 50m) : GlobalSettings.fltMinRig),2);
                            flt.Block = flt.DeadHead ? 0 : flt.Block;   // block is required for calculating tfpByTime above, but dh does not count for block
                            dpTfp += flt.Tfp = Math.Max(flt.TfpByDistance, flt.TfpByTime);
                            if (flt.Equip != null)
                            {
                                if (flt.Equip.Length > 0)
                                {

                                    flt.AcftChange = (flt.Equip.Substring(0, 1) == "*") ? true : false;
                                }
                            }
                            flt.RigFlt = (flt.Block < 55 ? (55 - flt.Block) / 50m : 0m);
                            // this will capture the last flight in the duty period
                            //dp.ReleaseTime = flt.ArrTime + (record.Value.DebriefTime / 100) * 60;
                            dp.ReleaseTime = flt.ArrTime + record.Value.DebriefTime;

                            //if a trip has any flight with a city that is international, then the International property will be set to true for the trip
                            var internationalcities = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).Select(y=>y.Name);
                            var nonconuscities = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).Select(y=>y.Name);
                            var hawaicities = GlobalSettings.WBidINIContent.Cities.Where(x => x.Hawai).Select(y => y.Name);
                            if (internationalcities.Contains(flt.ArrSta) || internationalcities.Contains(flt.DepSta))
                            {
                                record.Value.International = true;
                            }
                            if (nonconuscities.Contains(flt.ArrSta) || nonconuscities.Contains(flt.DepSta))
                            {
                                record.Value.NonConus = true;
                            }
                            if (hawaicities.Contains(flt.ArrSta) || hawaicities.Contains(flt.DepSta))
                            {
                                flt.ETOPS = true;
                            }
                        }


                        //Calculating FDP
                        int flightCount = dp.Flights.Count;
                        for (int flightIndex = flightCount - 1; flightIndex >= 0; flightIndex--)
                        {
                            if (dp.Flights[flightIndex].DeadHead)
                                continue;
                            else
                            {
                                dp.FDP = dp.Flights[flightIndex].ArrTime - dp.ShowTime;
                                break;
                            }

                        }


                        tripTfpByDistance += dp.TfpByDistance = dpTfpByDistance;
                        tripTfpByTime += dp.TfpByTime = dpTfpByTime;
                        dp.RigDailyMin = (dpTfp < GlobalSettings.PltDpMinFactor) ? GlobalSettings.PltDpMinFactor - dpTfp : 0m;

                        tripBlock += dp.Block = dpBlock;
                        //dp.DutyTime = (dp.DutPerSeqNum == 1) ? (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.show1stDay + GlobalSettings.debrief)) : (dpLastFltLandTime - dpFirstFltDepTime + (GlobalSettings.showAfter1stDay + GlobalSettings.debrief));
                        dp.DutyTime = dpLastFltLandTime - dp.ShowTime + GlobalSettings.debrief;
                        //dp.DutyTime = (dp.DutPerSeqNum == 1) ? (dpLastFltLandTime - dpFirstFltDepTime + 90) : (dpLastFltLandTime - dpFirstFltDepTime + 60);

                        //dp.TfpByDutyHrs = dpTfpByDutyHrs = (dp.DutyTime / GlobalSettings.PltDhrFactor);
                        //dp.RigDhr = dpTfpByDutyHrs > dpTfp ? dpTfpByDutyHrs - dpTfp : 0m;

                        dp.TfpByDutyHrs = dpTfpByDutyHrs = ((dp.DutyTime / 60m) * GlobalSettings.PltDhrFactor);
                        dp.RigDhr = dpTfpByDutyHrs > dpTfp ? dpTfpByDutyHrs - dpTfp : 0m;


                        // tripTfp += dp.Tfp = dpTfp + dp.RigDhr;
                        tripTfp += dp.Tfp = dpTfp;
                        // tripTfp += dp.Tfp = Math.Max(Math.Max(dpTfp, GlobalSettings.PltDpMinFactor), dpTfpByDutyHrs);
                        dp.DepTimeFirstLeg = dpFirstFltDepTime;
                        dp.LandTimeLastLeg = dpLastFltLandTime;
                        if (dp.RigDailyMin > dp.RigDhr)
                            dp.RigDhr = 0m;
                        else
                            dp.RigDailyMin = 0m;

                        dp.Tfp += dp.RigDhr + dp.RigDailyMin;

                        tripTfp += dp.RigDhr + dp.RigDailyMin;
                    }


                    //Calculate FDP
                    record.Value.FDP = record.Value.DutyPeriods.Sum(x => x.FDP);

                    record.Value.TfpByDistance = tripTfpByDistance;
                    record.Value.TfpByTime = tripTfpByTime;

                    if (!record.Value.IsFromBidFile)
                    {
                        record.Value.Tfp = tripTfp;
                    }
                    record.Value.Block = tripBlock;           // consider changing Trip property Block to int

                    // calc rigs for trip
                    record.Value.RigAdg = record.Value.PairLength * GlobalSettings.PltAdgFactor > record.Value.Tfp ? record.Value.PairLength * GlobalSettings.PltAdgFactor - record.Value.Tfp : 0m;
                    //record.Value.Tafb = tripLastFltLandTime - tripFirstFltDepTime + 90;
                    record.Value.Tafb = tripLastFltLandTime - tripFirstFltDepTime + record.Value.BriefTime + GlobalSettings.debrief;

                    record.Value.RigTafb = (record.Value.Tfp < (record.Value.Tafb / Convert.ToDecimal(60.0) * GlobalSettings.PltTafbFactor)) ? ((record.Value.Tafb / Convert.ToDecimal(60.0) * GlobalSettings.PltTafbFactor) - record.Value.Tfp) : 0m;
                    // record.Value.RigTafb = record.Value.Tfp < (record.Value.Tafb / GlobalSettings.PltTafbFactor) ? record.Value.Tafb / GlobalSettings.PltTafbFactor - record.Value.Tfp : 0m;


                    if (record.Value.RigTafb > record.Value.RigAdg)
                        record.Value.RigAdg = 0m;
                    else
                        record.Value.RigTafb = 0m;

                    if (!record.Value.IsFromBidFile)
                    {
                        record.Value.Tfp += Math.Max(record.Value.RigAdg, record.Value.RigTafb);
                    }
                    record.Value.TotalLegs = totalLegs;
                }
                else
                {
                    int flightSeqNum;


                    foreach (var dp in record.Value.DutyPeriods)
                    {
                        flightSeqNum = 0;
                        // this works because there is only 1 flight per duty period for reserve
                        //dp.ShowTime = dp.Flights[0].DepTime - (record.Value.BriefTime / 100) * 60;

                        if (dp.DutPerSeqNum == 1)
                        {
                            // dp.ShowTime = dp.Flights[0].DepTime - record.Value.BriefTime;
                            dp.ShowTime = dp.Flights[0].DepTime - GlobalSettings.ReserveBriefTime;
                        }
                        else
                        {
                            dp.ShowTime = dp.Flights[0].DepTime - GlobalSettings.showAfter1stDay;
                        }

                        dp.ReleaseTime = dp.Flights[0].ArrTime + (record.Value.DebriefTime / 100) * 60;
                        dp.DepTimeFirstLeg = dp.Flights[0].DepTime;
                        dp.LandTimeLastLeg = dp.Flights[0].ArrTime;
                        //duty period time changed from 7 hours to 6.30 hours
                        //dp.ReserveOut = dp.DepTimeFirstLeg - 390;
                        //dp.ReserveIn = dp.LandTimeLastLeg + 390;
                        //May 2017 for Jun 2017 bid :#1 record and #5 record spec changed intrip file for reserve Rap Times.

                        //dp.ReserveOut = dp.DepTimeFirstLeg - 270;
                        //.ReserveIn = dp.LandTimeLastLeg + 510;
                        dp.ReserveOut = (dp.DutPerSeqNum - 1) * 1440 + record.Value.BriefTime;
                        dp.ReserveIn = (dp.DutPerSeqNum - 1) * 1440 + record.Value.DebriefTime;


                        dp.Tfp = 6.0m;
                        dp.Block = 0;

                        //Added-- To set the strt index of the flight from 0 to 1 to solve the vacation issue(Flight vacation type seems to empty
                        dp.Flights[0].FlightSeqNum = ++flightSeqNum;
                        dp.Flights[0].Tfp = dp.Tfp;
                        dp.Flights[0].ArrTime = dp.ReserveIn;
                    }
                    record.Value.Tafb = 0;

                }
                record.Value.DhFirst = record.Value.DutyPeriods[0].Flights[0].DeadHead;
                var lastDutyperiod = record.Value.DutyPeriods[record.Value.DutyPeriods.Count - 1];
                record.Value.DhLast = lastDutyperiod.Flights[lastDutyperiod.Flights.Count - 1].DeadHead;
            }
        }

        /// <summary>
        /// Update Flt,Tfp,Blk,AcftChg,RigFlt properties for Flight Attend
        /// </summary>
        private void UpdatePropertyValuesforFlightAttend(Dictionary<string, Trip> tripdata, List<CityPair> ListCityPair)
        {
            decimal tripTfpByDistance, tripTfpByTime, tripTfp;
            int tripFirstFltDepTime, tripLastFltLandTime, totalLegs = 0;


            foreach (var record in tripdata)
            {

                // if (record.Key.Substring(1, 1) != "W" && record.Key.Substring(1, 1) != "Y" && record.Key.Substring(1, 1) != "R")
                if (!record.Value.ReserveTrip)
                {

                    tripFirstFltDepTime = tripLastFltLandTime = totalLegs = 0;
                    tripTfpByDistance = tripTfpByTime = tripTfp = 0m;
                    int tripBlock = 0;
                    int firstFlight = 0;
                    int flightSeqNum = 0;
                    foreach (var dp in record.Value.DutyPeriods)
                    {
                        decimal dpTfpByDistance, dpTfpByTime, dpTfp, dpTfpByDutyHrs;
                        dpTfpByDistance = dpTfpByTime = dpTfp = dpTfpByDutyHrs = 0;
                        int dpBlock, dpDutyTime, dpFirstFltDepTime, dpLastFltLandTime;
                        dpBlock = dpDutyTime = dpFirstFltDepTime = dpLastFltLandTime = 0;
                        totalLegs += dp.Flights.Count;
                        if (dp.DutPerSeqNum == 1)
                        {
                            dp.ShowTime = dp.Flights[0].DepTime - GlobalSettings.FAshow1stDutyPeriod;
                        }
                        else
                        {
                            dp.ShowTime = dp.Flights[0].DepTime - GlobalSettings.showAfter1stDay;
                        }

                        // dp.ShowTime = dp.Flights[0].DepTime - (record.Value.BriefTime / 100) * 60;
                        flightSeqNum = 0;
                        foreach (var flt in dp.Flights)
                        {
                            flt.FlightSeqNum = ++flightSeqNum;
                            //flt.TurnTime = flightSeqNum <= dp.Flights.Count ? // true includes all flights but last flight
                            //    dp.Flights[flightSeqNum].DepTime - dp.Flights[flightSeqNum - 1].ArrTime :
                            //    dp.DutPerSeqNum == record.Value.DutyPeriods.Count ? // handles last duty period if true
                            //        flt.TurnTime = 0 : // last flight in last duty period in trip
                            //        record.Value.DutyPeriods[dp.DutPerSeqNum + 1].Flights[0].DepTime - dp.Flights[flightSeqNum - 1].ArrTime;
                            if (flightSeqNum < dp.Flights.Count)
                            {
                                flt.TurnTime = dp.Flights[flightSeqNum].DepTime - dp.Flights[flightSeqNum - 1].ArrTime;
                            }
                            else if (dp.DutPerSeqNum == record.Value.DutyPeriods.Count)
                            {
                                flt.TurnTime = 0;
                            }
                            else
                            {
                                flt.TurnTime = record.Value.DutyPeriods[dp.DutPerSeqNum].Flights[0].DepTime - dp.Flights[flightSeqNum - 1].ArrTime;
                            }
                            string depCity = flt.DepSta;
                            string arrCity = flt.ArrSta;
                            if (dpFirstFltDepTime == 0) dpFirstFltDepTime = flt.DepTime;
                            if (tripFirstFltDepTime == 0) tripFirstFltDepTime = flt.DepTime;
                            dpLastFltLandTime = flt.ArrTime;
                            tripLastFltLandTime = flt.ArrTime;
                            var citypair = ListCityPair.FirstOrDefault(x => (x.City1 == depCity && x.City2 == arrCity) || (x.City1 == arrCity && x.City2 == depCity));
                            if (citypair != null)
                            {
                                dpTfpByDistance += flt.TfpByDistance = citypair.Distance;
                            }
                            else
                            {
                                // todo: message box for missing city pair
                            }
                            //dpTfpByDistance += flt.TfpByDistance = CityPairsDistanceData[depCity + arrCity];
                            flt.Block = flt.ArrTime - flt.DepTime;
                            dpBlock += flt.DeadHead ? 0 : flt.Block;
                            dpTfpByTime += flt.TfpByTime =Math.Round((flt.Block > 55 ? (1 + (flt.Block - 55) / 50m) : GlobalSettings.fltMinRig),2);
                            flt.Block = flt.DeadHead ? 0 : flt.Block;   // block is required for calculating tfpByTime above, but dh does not count for block
                            dpTfp += flt.Tfp = Math.Max(flt.TfpByDistance, flt.TfpByTime);
                            //if (flt.Equip != null)
                            //{
                            //    flt.AcftChange = (flt.Equip.Substring(0, 1) == "*") ? true : false;
                            //}
                            flt.RigFlt = (flt.Block < 55 ? (55 - flt.Block) / 50m : 0m);
                            // this will capture the last flight in the duty period
                            dp.ReleaseTime = flt.ArrTime + (record.Value.DebriefTime / 100) * 60;

                            //if a trip has any flight with a city that is international, then the International property will be set to true for the trip
                            var internationalcities = GlobalSettings.WBidINIContent.Cities.Where(x => x.International).Select(y => y.Name);
                            var nonconuscities = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).Select(y => y.Name);
                            if (internationalcities.Contains(flt.ArrSta) || internationalcities.Contains(flt.DepSta))
                            {
                                record.Value.International = true;
                            }
                            if (nonconuscities.Contains(flt.ArrSta) || nonconuscities.Contains(flt.DepSta))
                            {
                                record.Value.NonConus = true;
                            }
                        }

                        //Calculating FDP
                        int flightCount = dp.Flights.Count;
                        for (int flightIndex = flightCount - 1; flightIndex >= 0; flightIndex--)
                        {
                            if (dp.Flights[flightIndex].DeadHead)
                                continue;
                            else
                            {
                                dp.FDP = dp.Flights[flightIndex].ArrTime - dp.ShowTime;
                                break;
                            }

                        }

                        tripTfpByDistance += dp.TfpByDistance = dpTfpByDistance;
                        tripTfpByTime += dp.TfpByTime = dpTfpByTime;
                        dp.RigDailyMin = (dpTfp < GlobalSettings.FaDpMinFactor) ? GlobalSettings.FaDpMinFactor - dpTfp : 0m;

                        tripBlock += dp.Block = dpBlock;
                        //first duty period duty time calculation
                        if (dp.DutPerSeqNum == 1)
                        {
                            dp.DutyTime = (dpLastFltLandTime - dpFirstFltDepTime) + (GlobalSettings.FAshow1stDutyPeriod + GlobalSettings.FArelease);
                        }
                        //last duty period duty time calculation
                        else if (dp.DutPerSeqNum == record.Value.DutyPeriods.Count)
                        {
                            dp.DutyTime = (dpLastFltLandTime - dpFirstFltDepTime) + (GlobalSettings.FAshow + GlobalSettings.FAreleaseLastDutyPeriod);
                        }
                        //Inner duty periods duty time calculation
                        else
                        {
                            dp.DutyTime = (dpLastFltLandTime - dpFirstFltDepTime) + (GlobalSettings.FAshow + GlobalSettings.FArelease);
                        }

                        //dp.DutyTime = (dp.DutPerSeqNum == 1) ? (dpLastFltLandTime - dpFirstFltDepTime + GlobalSettings.FAshow1stDutyPeriod+GlobalSettings.FArelease) : (dpLastFltLandTime - dpFirstFltDepTime + GlobalSettings.FAshow+GlobalSettings.FArelease);
                        dp.TfpByDutyHrs = dpTfpByDutyHrs = ((dp.DutyTime / 60m) * GlobalSettings.FaDhrFactor);
                        dp.RigDhr = dpTfpByDutyHrs > dpTfp ? dpTfpByDutyHrs - dpTfp : 0m;

                        // frank comment out 11/23/2013 -- logic error, did not consider RigDailyMin
                        // tripTfp += dp.Tfp = dpTfp + dp.RigDhr;
                        tripTfp += dp.Tfp = dpTfp;

                        //tripTfp += dp.Tfp = Math.Max(Math.Max(dpTfp, GlobalSettings.FaDpMinFactor), dpTfpByDutyHrs);
                        dp.DepTimeFirstLeg = dpFirstFltDepTime;
                        dp.LandTimeLastLeg = dpLastFltLandTime;
                        if (dp.RigDailyMin > dp.RigDhr)
                            dp.RigDhr = 0m;
                        else
                            dp.RigDailyMin = 0m;

                        // frank  11/23/2013 add to insure Flt Att DpMin is in dutyperiod
                        dp.Tfp += dp.RigDhr + dp.RigDailyMin;

                        tripTfp += dp.RigDhr + dp.RigDailyMin;

                        //duty break calculation.(Note: DutPerSeqNum is 1 for first duty duty period) 
                        if (dp.DutPerSeqNum > 1)
                        {

                            record.Value.DutyPeriods[dp.DutPerSeqNum - 2].DutyBreak = dp.DepTimeFirstLeg - record.Value.DutyPeriods[dp.DutPerSeqNum - 2].LandTimeLastLeg - (GlobalSettings.FArelease + GlobalSettings.FAshow);
                        }


                    }
                    //FDP calculation
                    record.Value.FDP = record.Value.DutyPeriods.Sum(x => x.FDP);

                    record.Value.TfpByDistance = tripTfpByDistance;
                    record.Value.TfpByTime = tripTfpByTime;
                    record.Value.Tfp = tripTfp;
                    record.Value.Block = tripBlock;           // consider changing Trip property Block to int

                    // calc rigs for trip
                    record.Value.RigAdg = record.Value.PairLength * GlobalSettings.FaAdgFactor > record.Value.Tfp ? record.Value.PairLength * GlobalSettings.FaAdgFactor - record.Value.Tfp : 0m;
                    record.Value.Tafb = tripLastFltLandTime - tripFirstFltDepTime + 90;
					record.Value.RigTafb = record.Value.Tfp < (record.Value.Tafb /Convert.ToDecimal(60.0) * GlobalSettings.FaTafbFactor) ? record.Value.Tafb / Convert.ToDecimal(60.0) * GlobalSettings.FaTafbFactor - record.Value.Tfp : 0m;

                    // record.Value.RigTafb = record.Value.Tfp < (record.Value.Tafb / WBidConstants.tafbRig) ? record.Value.Tafb / WBidConstants.tafbRig - record.Value.Tfp : 0m;
                    if (record.Value.RigTafb > record.Value.RigAdg)
                        record.Value.RigAdg = 0m;
                    else
                        record.Value.RigTafb = 0m;


                    record.Value.Tfp += Math.Max(record.Value.RigAdg, record.Value.RigTafb);
                    record.Value.TotalLegs = totalLegs;
                }
                else
                {
                    int flightSeqNum;
                    foreach (var dp in record.Value.DutyPeriods)
                    {
                        flightSeqNum = 0;
                        // this works because there is only 1 flight per duty period for reserve
                        if (dp.DutPerSeqNum == 1)
                        {
                            dp.ShowTime = dp.Flights[0].DepTime - GlobalSettings.FAshow1stDutyPeriod;
                        }
                        else
                        {
                            dp.ShowTime = dp.Flights[0].DepTime - GlobalSettings.showAfter1stDay;
                        }

                        // dp.ShowTime = dp.Flights[0].DepTime - (record.Value.BriefTime / 100) * 60;
                        dp.ReleaseTime = dp.Flights[0].ArrTime + (record.Value.DebriefTime / 100) * 60;
                        dp.DepTimeFirstLeg = dp.Flights[0].DepTime;
                        dp.LandTimeLastLeg = dp.Flights[0].ArrTime;
                        dp.ReserveOut = dp.DepTimeFirstLeg;
                        dp.ReserveIn = dp.LandTimeLastLeg;
                        dp.Tfp = 6.0m;
                        dp.Block = 0;
                        dp.Flights[0].Tfp = dp.Tfp;
                        dp.Flights[0].FlightSeqNum = ++flightSeqNum;
                    }
                    record.Value.Tafb = 0;

                }
                record.Value.DhFirst = record.Value.DutyPeriods[0].Flights[0].DeadHead;
                var lastDutyperiod = record.Value.DutyPeriods[record.Value.DutyPeriods.Count - 1];
                record.Value.DhLast = lastDutyperiod.Flights[lastDutyperiod.Flights.Count - 1].DeadHead;
            }
        }

        #endregion

        // not needed as FA data cannot be reverse engineered to FreqCode, OpDays, and NonOpDays.

        //public void SetFaFreqCode0pDaysNonOpDays(Trip trip)
        //{
        //    int[] dow = new int[7];
        //    Char one = Convert.ToChar("1");
        //    DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate; 
        //    TimeSpan bpDays = GlobalSettings.CurrentBidDetails.BidPeriodEndDate - startDate;
        //    for (int day = 0; day <= bpDays.Days; day++)
        //    {
        //        int dayOfWeek = (int)startDate.AddDays(day).DayOfWeek;
        //        dow[dayOfWeek] = trip.OpVector[day] == one ? dow[dayOfWeek]++ : dow[dayOfWeek];
        //    }
        //    int weekDayOpDaysCount = dow.Where(x => x > 0).Count();
        //    trip.FreqCode = weekDayOpDaysCount > 3 ? "X" : "F";
        //    if (trip.FreqCode == "F")
        //    {
        //        trip.OpDays = dow[1] > 0 ? " 1" : "";
        //        trip.OpDays = dow[2] > 0 ? " 2" : "";
        //        trip.OpDays = dow[3] > 0 ? " 3" : "";
        //        trip.OpDays = dow[4] > 0 ? " 4" : "";
        //        trip.OpDays = dow[5] > 0 ? " 5" : "";
        //        trip.OpDays = dow[6] > 0 ? " 6" : "";
        //        trip.OpDays = dow[0] > 0 ? " 7" : "";
        //    }
        //    else
        //    {
        //        trip.OpDays = dow[1] == 0 ? " 1" : "";
        //        trip.OpDays = dow[2] == 0 ? " 2" : "";
        //        trip.OpDays = dow[3] == 0 ? " 3" : "";
        //        trip.OpDays = dow[4] == 0 ? " 4" : "";
        //        trip.OpDays = dow[5] == 0 ? " 5" : "";
        //        trip.OpDays = dow[6] == 0 ? " 6" : "";
        //        trip.OpDays = dow[0] == 0 ? " 7" : "";
        //    }
        //}
    }
}
