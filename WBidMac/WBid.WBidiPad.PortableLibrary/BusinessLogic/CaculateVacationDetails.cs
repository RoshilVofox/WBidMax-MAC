using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class CaculateVacationDetails
    {

        #region Private Variables
        private Dictionary<string, TripMultiVacData> _vacationData;
        private Dictionary<string, Line> _lines;
        #endregion

        #region Public Methods
        public void CalculateVacationdetailsFromVACfile(Dictionary<string, Line> linesDictionary, Dictionary<string, TripMultiVacData> vacationData)
        {
            try
            {
                //Read the content of vacation file.
                //  _vacationData = (Dictionary<string, TripMultiVacData>)DeSerializeObject(vacFileName);
                _vacationData = vacationData;
                if (_vacationData == null)
                    return;

                _lines = linesDictionary;

                bool isVacationTrip = false;
                string vacType = string.Empty;
                DateTime tripStartDate = DateTime.MinValue;
                DateTime tripEndDate = DateTime.MinValue;
                VacationTrip vacTrip = null;
                Trip trip = null;
                List<TempVap> tempVapList = null;

                foreach (var line in _lines)
                {

                    //Set Initial value to Line vacation state properties
                    SetInitialValues(line);

                    if (line.Value.BlankLine)
                        continue;


                    if (line.Value.LineNum == 308)
                    {
                    }

                    tempVapList = new List<TempVap>();
                    isVacationTrip = false;
                    int tripEndTime = 0;
                    if (GlobalSettings.OrderedVacationDays != null)
                    {
                        bool isLastTrip = false; int paringCount = 0;

                        foreach (string pairing in line.Value.Pairings)
                        {


                            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
                            trip = trip ?? GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
                            isLastTrip = ((line.Value.Pairings.Count - 1) == paringCount); paringCount++;
                            tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).Trim()), isLastTrip, GlobalSettings.CurrentBidDetails);
                            tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
                            isVacationTrip = false;


                            tripEndTime = (trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg) % 1440;
                            tripEndTime = AfterMidnightLandingTime(tripEndTime);
                            tripEndTime = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripEndDate, tripEndTime);
                            //VA type
                            foreach (Absense vacPerid in GlobalSettings.OrderedVacationDays)
                            {
                                vacType = string.Empty;
                                if (vacPerid.AbsenceType == "VA")
                                {
                                    DateTime eDate = (vacPerid.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate) ? GlobalSettings.CurrentBidDetails.BidPeriodEndDate : vacPerid.EndAbsenceDate;
                                    DateTime sDate = (vacPerid.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate) ? GlobalSettings.CurrentBidDetails.BidPeriodStartDate : vacPerid.StartAbsenceDate;

                                    //VA type trip
                                    //-------------------------------------------------------------------------------------
                                    if (tripStartDate >= vacPerid.StartAbsenceDate && tripEndDate <= vacPerid.EndAbsenceDate)
                                    {
                                        isVacationTrip = true;
                                        vacType = "VA";
                                        vacTrip = _vacationData.Where(x => x.Key == pairing).Select(y => y.Value.VaData).FirstOrDefault();
                                        if (vacTrip == null)
                                            continue;
                                        //tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count(), StartDate = sDate, EndDate = eDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInBpTot });

                                       tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count(), StartDate = sDate, EndDate = vacPerid.EndAbsenceDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInLineTot, IsInBp = false });
										tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count(), StartDate = sDate, EndDate = eDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInBpTot, IsInBp = true });
                                        line.Value.VacationStateLine.VacationTfp += vacTrip.VacationGrandTotal.VA_TfpInBpTot;
                                        line.Value.VacationStateLine.VacationTfpInLine += vacTrip.VacationGrandTotal.VA_TfpInLineTot;
                                        line.Value.VacationStateLine.VacationDaysOff += vacTrip.DutyPeriodsDetails.Where(x => x.isInBp && x.VacationType == "VA").Count();
                                        line.Value.VacationStateLine.VacationLegs += (line.Value.ReserveLine) ? 0 : vacTrip.DutyPeriodsDetails.SelectMany(x => x.FlightDetails).Count();
                                        line.Value.VacationStateLine.VacationTafb = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationTafb) + (int)vacTrip.VacationGrandTotal.VA_TafbInBpTot);
                                        line.Value.VacationStateLine.VacationBlkHrs = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationBlkHrs) + (int)vacTrip.VacationGrandTotal.VA_BlkInBpTot);
                                        VATrip(line, pairing, tripStartDate, vacTrip);
                                        break;
                                    }
                                    //-------------------------------------------------------------------------------------


                                     //VOF type trip
                                    //-------------------------------------------------------------------------------------
                                    else if (tripStartDate < vacPerid.StartAbsenceDate && tripEndDate >= vacPerid.StartAbsenceDate ||
                                                                                                  ((vacPerid.StartAbsenceDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440)))
                                    {
                                        isVacationTrip = true;
                                        vacType = "VOF";
                                        vacTrip = _vacationData.Where(x => x.Key == pairing).Select(y => y.Value.VofData).FirstOrDefault();
                                        if (vacTrip == null)
                                            continue;

                                        //tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count(), StartDate = sDate, EndDate = eDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInBpTot });
										tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count(), StartDate = sDate, EndDate = vacPerid.EndAbsenceDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInLineTot, IsInBp = false });
										tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count(), StartDate = sDate, EndDate = eDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInBpTot, IsInBp = true });
                                        line.Value.VacationStateLine.VacationTfp = line.Value.VacationStateLine.VacationTfp + vacTrip.VacationGrandTotal.VA_TfpInBpTot + vacTrip.VacationGrandTotal.VO_TfpInBpTot;
                                        line.Value.VacationStateLine.VacationTfpInLine = line.Value.VacationStateLine.VacationTfpInLine + vacTrip.VacationGrandTotal.VA_TfpInLineTot + vacTrip.VacationGrandTotal.VO_TfpInLineTot;
                                        line.Value.VacationStateLine.VacationDropTfp += vacTrip.VacationGrandTotal.VD_TfpInBpTot;
                                        line.Value.VacationStateLine.VacationFront += vacTrip.VacationGrandTotal.VO_TfpInBpTot;
                                        line.Value.VacationStateLine.VacationDaysOff += vacTrip.DutyPeriodsDetails.Where(x => x.isInBp && (x.VacationType == "VO" || x.VacationType == "VA")).Count();
                                        line.Value.VacationStateLine.VacationDropDaysOff += vacTrip.DutyPeriodsDetails.Where(x => x.isInBp && (x.VacationType == "VD" || x.VacationType == "Split")).Count();
                                        if (!line.Value.ReserveLine)
                                        {
                                            line.Value.VacationStateLine.VacationLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VO" || x.VacationType == "VA").SelectMany(x => x.FlightDetails).Count();
                                            line.Value.VacationStateLine.VacationLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "Split").SelectMany(x => x.FlightDetails.Where(y => y.VacationType == "VO")).Count();
                                            line.Value.VacationStateLine.VacationDropLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD").SelectMany(x => x.FlightDetails).Count();
                                            line.Value.VacationStateLine.VacationDropLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "Split").SelectMany(x => x.FlightDetails.Where(y => y.VacationType == "VD")).Count();
                                        }
                                        else
                                        {
                                            line.Value.VacationStateLine.VacationLegs = 0;
                                            line.Value.VacationStateLine.VacationDropLegs = 0;
                                        }
                                        line.Value.VacationStateLine.VacationTafb = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationTafb) + (int)vacTrip.VacationGrandTotal.VA_TafbInBpTot + (int)vacTrip.VacationGrandTotal.VO_TafbInBpTot);
                                        line.Value.VacationStateLine.VacationDropTafb = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationDropTafb) + (int)vacTrip.VacationGrandTotal.VD_TafbInBpTot);
                                        line.Value.VacationStateLine.VacationBlkHrs = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationBlkHrs) + (int)vacTrip.VacationGrandTotal.VA_BlkInBpTot + (int)vacTrip.VacationGrandTotal.VO_BlkInBpTot);
                                        line.Value.VacationStateLine.VacationDropBlkHrs = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationDropBlkHrs) + (int)vacTrip.VacationGrandTotal.VD_BlkInBpTot);
                                        VOFTrip(line, pairing, tripStartDate, vacTrip);
                                        break;
                                    }

                                    //VOB type trip
                                    //-------------------------------------------------------------------------------------
                                    else if (tripStartDate <= vacPerid.EndAbsenceDate && tripEndDate > vacPerid.EndAbsenceDate)
                                    {
                                        isVacationTrip = true;
                                        vacType = "VOB";
                                        vacTrip = _vacationData.Where(x => x.Key == pairing).Select(y => y.Value.VobData).FirstOrDefault();
                                        if (vacTrip == null)
                                            continue;

                                        //tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count(), StartDate = sDate, EndDate = eDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInBpTot });
										 tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count(), StartDate = sDate, EndDate = vacPerid.EndAbsenceDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInLineTot, IsInBp = false });
										tempVapList.Add(new TempVap() { Days = vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count(), StartDate = sDate, EndDate = eDate, Tfp = vacTrip.VacationGrandTotal.VA_TfpInBpTot, IsInBp = true });                                        
                                        line.Value.VacationStateLine.VacationTfp = line.Value.VacationStateLine.VacationTfp + vacTrip.VacationGrandTotal.VA_TfpInBpTot + vacTrip.VacationGrandTotal.VO_TfpInBpTot;
                                       line.Value.VacationStateLine.VacationTfpInLine = line.Value.VacationStateLine.VacationTfpInLine + vacTrip.VacationGrandTotal.VA_TfpInLineTot + vacTrip.VacationGrandTotal.VO_TfpInLineTot;

                                        line.Value.VacationStateLine.VacationDropTfp += vacTrip.VacationGrandTotal.VD_TfpInBpTot;
                                        line.Value.VacationStateLine.VacationBack += vacTrip.VacationGrandTotal.VO_TfpInBpTot;
                                        line.Value.VacationStateLine.VacationDaysOff += vacTrip.DutyPeriodsDetails.Where(x => x.isInBp && (x.VacationType == "VO" || x.VacationType == "VA")).Count();
                                        line.Value.VacationStateLine.VacationDropDaysOff += vacTrip.DutyPeriodsDetails.Where(x => x.isInBp && (x.VacationType == "VD" || x.VacationType == "Split")).Count();
                                        if (!line.Value.ReserveLine)
                                        {
                                            line.Value.VacationStateLine.VacationLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VO" || x.VacationType == "VA").SelectMany(x => x.FlightDetails).Count();
                                            line.Value.VacationStateLine.VacationLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "Split").SelectMany(x => x.FlightDetails.Where(y => y.VacationType == "VO")).Count();
                                            line.Value.VacationStateLine.VacationDropLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD").SelectMany(x => x.FlightDetails).Count();
                                            line.Value.VacationStateLine.VacationDropLegs += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "Split").SelectMany(x => x.FlightDetails.Where(y => y.VacationType == "VD")).Count();
                                        }
                                        else
                                        {
                                            line.Value.VacationStateLine.VacationLegs = 0;
                                            line.Value.VacationStateLine.VacationDropLegs = 0;
                                        }
                                        line.Value.VacationStateLine.VacationTafb = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationTafb) + (int)vacTrip.VacationGrandTotal.VA_TafbInBpTot + (int)vacTrip.VacationGrandTotal.VO_TafbInBpTot);
                                        line.Value.VacationStateLine.VacationDropTafb = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationDropTafb) + (int)vacTrip.VacationGrandTotal.VD_TafbInBpTot);
                                        line.Value.VacationStateLine.VacationBlkHrs = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationBlkHrs) + (int)vacTrip.VacationGrandTotal.VA_BlkInBpTot + (int)vacTrip.VacationGrandTotal.VO_BlkInBpTot);
                                        line.Value.VacationStateLine.VacationDropBlkHrs = (line.Value.ReserveLine) ? "00:00" : Helper.ConvertMinuteToHHMM(Helper.ConvertformattedHhhmmToMinutes(line.Value.VacationStateLine.VacationDropBlkHrs) + (int)vacTrip.VacationGrandTotal.VD_BlkInBpTot);

                                        VOBTrip(line, pairing, tripStartDate, vacTrip);
                                        break;
                                    }
                                    //-------------------------------------------------------------------------------------

                                }

                            }

                            if (!isVacationTrip)
                            {
                                decimal flyPay = CalculateFlyPay(tripStartDate, trip, line);
                                line.Value.VacationStateLine.FlyPay += flyPay;
                                line.Value.VacationStateLine.FlyPayInLine += trip.Tfp;
                            }

                        }
                        CalculateVAP(tempVapList, line);
                    }



                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }


        #endregion

        #region Private Methods

        private int AfterMidnightLandingTime(int landTime)
        {
            if (landTime < GlobalSettings.LastLandingMinus1440)
                return landTime += 1440;
            else
                return landTime % 1440;
        }

        public  int DomicileTimeFromHerb(string domicile, DateTime dutPdDate, int herb)
        {
           // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
          //  TimeSpan ts = new TimeSpan(12, 0, 0);

            // Insures that time dateTime is not 12:00AM but 12:00pm
            // Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.

          //  dutPdDate = dutPdDate.Date + ts;
           // bool isDst = timeZoneInfo.IsDaylightSavingTime(dutPdDate);
            bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(dutPdDate);



            switch (domicile)
            {
                case "ATL":
                    return herb + 60;       // EST = herb + 60
					case "AUS":
                    return herb;            // CST = herb 
                case "BWI":
                    return herb + 60;       // EST = herb + 60
                case "DAL":
                    return herb;            // CST = herb 
                case "DEN":
                    return herb - 60;       // MST = herb - 60
					case "FLL":
                    return herb + 60;       // EST = herb + 60
                case "HOU":
                    return herb;            // CST = herb 
                case "LAS":
                    return herb - 120;      // PST = herb - 120
                case "MCO":
                    return herb + 60;       // EST = herb + 60
                case "MDW":
                    return herb;            // CST = herb 
                case "LAX":
                    return herb - 120;      // PST = herb - 120
                case "OAK":
                    return herb - 120;      // PST = herb - 120
                case "PHX":
                    if (isDst)
                        return herb - 120;  // PST = herb - 120
                    else
                        return herb - 60;   // MST = herb - 60
                default:
                    return 1440;
            }
        }

        private void VATrip(KeyValuePair<string, Line> line, string pairing, DateTime tripStartDate, VacationTrip vacTrip)
        {
            line.Value.VacationStateLine.VacationTrips = line.Value.VacationStateLine.VacationTrips ?? new List<VacationStateTrip>();
            VacationStateTrip vacationStateTrip = new VacationStateTrip();
            VacationStateDutyPeriod vacationStateDutyPeriod = null;
            VacationStateFlight vacationStateFlight = null; ;
            vacationStateTrip.TripType = "VA";
            vacationStateTrip.TripName = pairing;
            vacationStateTrip.TripActualStartDate = tripStartDate;
            vacationStateTrip.TripVacationStartDate = DateTime.MinValue;
            vacationStateTrip.VacationDutyPeriods = new List<VacationStateDutyPeriod>();

            int dpSeqNo = 1;
            int fltSeqNo = 1;
            foreach (VacationDutyPeriod dp in vacTrip.DutyPeriodsDetails)
            {
                vacationStateDutyPeriod = new VacationStateDutyPeriod();
                vacationStateDutyPeriod.VacationFlights = new List<VacationStateFlight>();
                vacationStateDutyPeriod.DutyperidSeqNo = dpSeqNo++;
                vacationStateDutyPeriod.DutyPeriodType = dp.VacationType;
                fltSeqNo = 1;
                foreach (VacationFlight flt in dp.FlightDetails)
                {
                    vacationStateFlight = new VacationStateFlight();
                    vacationStateFlight.FlightSeqNo = fltSeqNo++;
                    vacationStateFlight.FlightType = flt.VacationType;
                    vacationStateDutyPeriod.VacationFlights.Add(vacationStateFlight);
                }

                vacationStateTrip.VacationDutyPeriods.Add(vacationStateDutyPeriod);

            }

            line.Value.VacationStateLine.VacationTrips.Add(vacationStateTrip);
        }

        private void VOFTrip(KeyValuePair<string, Line> line, string pairing, DateTime tripStartDate, VacationTrip vacTrip)
        {
            try
            {
                line.Value.VacationStateLine.VacationTrips = line.Value.VacationStateLine.VacationTrips ?? new List<VacationStateTrip>();
                VacationStateTrip vacationStateTrip = new VacationStateTrip();
                vacationStateTrip.TripType = "VOF";
                vacationStateTrip.TripName = pairing;
                vacationStateTrip.TripActualStartDate = tripStartDate;
                if (vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split").Count() > 0)
                {
                    vacationStateTrip.TripVacationStartDate = tripStartDate;


                }
                else
                {
                    vacationStateTrip.TripVacationStartDate = DateTime.MinValue;
                }
                vacationStateTrip.VacationDutyPeriods = new List<VacationStateDutyPeriod>();
                VacationStateDutyPeriod vacationStateDutyPeriod = null;


                VacationStateFlight vacationStateFlight = null; ;

                int dpSeqNo = 1;
                int fltSeqNo = 1;
                foreach (VacationDutyPeriod dp in vacTrip.DutyPeriodsDetails)
                {
                    vacationStateDutyPeriod = new VacationStateDutyPeriod();
                    vacationStateDutyPeriod.VacationFlights = new List<VacationStateFlight>();
                    vacationStateDutyPeriod.DutyperidSeqNo = dpSeqNo++;
                    vacationStateDutyPeriod.DutyPeriodType = dp.VacationType;
                    fltSeqNo = 1;
                    foreach (VacationFlight flt in dp.FlightDetails)
                    {
                        vacationStateFlight = new VacationStateFlight();
                        vacationStateFlight.FlightSeqNo = fltSeqNo++;
                        vacationStateFlight.FlightType = flt.VacationType;
                        vacationStateDutyPeriod.VacationFlights.Add(vacationStateFlight);
                    }

                    vacationStateTrip.VacationDutyPeriods.Add(vacationStateDutyPeriod);

                }

                line.Value.VacationStateLine.VacationTrips.Add(vacationStateTrip);
                line.Value.VacationStateLine.FlyPay += vacTrip.VacationGrandTotal.VD_TfpInBpTot;
                line.Value.VacationStateLine.FlyPayInLine += vacTrip.VacationGrandTotal.VD_TfpInLineTot;

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private void VOBTrip(KeyValuePair<string, Line> line, string pairing, DateTime tripStartDate, VacationTrip vacTrip)
        {
            try
            {
                line.Value.VacationStateLine.VacationTrips = line.Value.VacationStateLine.VacationTrips ?? new List<VacationStateTrip>();
                VacationStateTrip vacationStateTrip = new VacationStateTrip();
                vacationStateTrip.TripType = "VOB";
                vacationStateTrip.TripName = pairing;
                vacationStateTrip.TripActualStartDate = tripStartDate;
                if (vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split").Count() > 0)
                {
                    vacationStateTrip.TripVacationStartDate = tripStartDate.AddDays(vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" || x.VacationType == "VO").Count());
                }
                else
                {
                    vacationStateTrip.TripVacationStartDate = DateTime.MinValue;
                }
                vacationStateTrip.VacationDutyPeriods = new List<VacationStateDutyPeriod>();
                VacationStateDutyPeriod vacationStateDutyPeriod = null;


                VacationStateFlight vacationStateFlight = null; ;

                int dpSeqNo = 1;
                int fltSeqNo = 1;
                foreach (VacationDutyPeriod dp in vacTrip.DutyPeriodsDetails)
                {
                    vacationStateDutyPeriod = new VacationStateDutyPeriod();
                    vacationStateDutyPeriod.VacationFlights = new List<VacationStateFlight>();
                    vacationStateDutyPeriod.DutyperidSeqNo = dpSeqNo++;
                    vacationStateDutyPeriod.DutyPeriodType = dp.VacationType;
                    fltSeqNo = 1;
                    foreach (VacationFlight flt in dp.FlightDetails)
                    {
                        vacationStateFlight = new VacationStateFlight();
                        vacationStateFlight.FlightSeqNo = fltSeqNo++;
                        vacationStateFlight.FlightType = flt.VacationType;
                        vacationStateDutyPeriod.VacationFlights.Add(vacationStateFlight);
                    }

                    vacationStateTrip.VacationDutyPeriods.Add(vacationStateDutyPeriod);

                }

                line.Value.VacationStateLine.VacationTrips.Add(vacationStateTrip);
                line.Value.VacationStateLine.FlyPay += vacTrip.VacationGrandTotal.VD_TfpInBpTot;
                line.Value.VacationStateLine.FlyPayInLine += vacTrip.VacationGrandTotal.VD_TfpInLineTot;

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        /// <summary>
        /// PURPOSE : Calculate VAP values
        /// </summary>
        /// <param name="tempVapList"></param>
        /// <param name="line"></param>
        private void CalculateVAP(List<TempVap> tempVapList, KeyValuePair<string, Line> line)
        {


			try
			{

				foreach (Absense vacPerid in GlobalSettings.OrderedVacationDays)
				{
					//DateTime eDate = (vacPerid.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate) ? GlobalSettings.CurrentBidDetails.BidPeriodEndDate : vacPerid.EndAbsenceDate;
					//DateTime sDate = (vacPerid.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate) ? GlobalSettings.CurrentBidDetails.BidPeriodStartDate : vacPerid.StartAbsenceDate;

					DateTime eDate = (vacPerid.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate) ? GlobalSettings.CurrentBidDetails.BidPeriodEndDate : vacPerid.EndAbsenceDate;
					DateTime sDate = (vacPerid.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate) ? GlobalSettings.CurrentBidDetails.BidPeriodStartDate : vacPerid.StartAbsenceDate;

					//var vacDetails = tempVapList.Where(x => x.StartDate == sDate && x.EndDate == eDate);

					if ((vacPerid.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate && vacPerid.EndAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
					|| (vacPerid.StartAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate && vacPerid.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate))
						continue;
					int totalVacationDays = 0;
					totalVacationDays = vacPerid.EndAbsenceDate.Subtract(sDate).Days + 1;
					int totalVacationDaysinBp = eDate.Subtract(sDate).Days + 1;
					int totalNoTripDaysInsideVacation = totalVacationDays;
					decimal totTfp = 0.0m;
					decimal vacTfpPerday = 0.0m;

					totTfp = tempVapList.Where(x => x.IsInBp == false && x.StartDate == sDate && x.EndDate == vacPerid.EndAbsenceDate).Select(x => x.Tfp).Sum();
					decimal totTfpinBp = tempVapList.Where(x => x.IsInBp == true && x.StartDate == sDate && x.EndDate == eDate).Select(x => x.Tfp).Sum();
					totalNoTripDaysInsideVacation = totalVacationDays - tempVapList.Where(x => x.IsInBp == false && x.StartDate == sDate && x.EndDate == vacPerid.EndAbsenceDate).Sum(x => x.Days);
					int totalNoTripDaysInsideVacationinBp = totalVacationDaysinBp - tempVapList.Where(x => x.IsInBp == true && x.StartDate == sDate && x.EndDate == eDate).Sum(x => x.Days);


                    //// frank added 5-21-2017
                    //decimal actualVap = Math.Max(totalVacationDays * GlobalSettings.DailyVacPay - totTfp, 0);
                    //// distribute VAP in bid period and next bid period
                    //decimal actualVapInBp = Math.Min(Math.Max(totalVacationDaysinBp * GlobalSettings.DailyVacPay - totTfpinBp, 0), actualVap);
                    //decimal actualVapInNextBp = actualVap - actualVapInBp;
                    //// end frank add 5-21-2017
                    ///
                    // Roshil added 2-23-2021
                    decimal actualVapInBp = 0m;
                    decimal actualVap = Math.Max(totalVacationDays * GlobalSettings.DailyVacPay - totTfp, 0);
                    // distribute VAP in bid period and next bid period
                    //Special case :when the total tfp with the mtm week exceeds 26.25.  The special case only applies when the tfp exceeds 26.25.  Otherwise, the Mar Vac Pay is the greater of 3.75 x days of vacation (4) = 15.0 or tfp pulled, whichever is greater.
                    //Then in April if the special case does not apply, the user is paid 26.25 minus the MAR vac pay.If the Apr VA is greater that that pay, the VAP for April will be negative.
                    if (totTfp > totalVacationDays * GlobalSettings.DailyVacPay)
                        actualVapInBp = Math.Min(Math.Max(totalVacationDaysinBp * GlobalSettings.DailyVacPay - totTfpinBp, 0), actualVap);
                    else
                        actualVapInBp = Math.Max(totalVacationDaysinBp * GlobalSettings.DailyVacPay - totTfpinBp, 0);
                    decimal actualVapInNextBp = actualVap - actualVapInBp;
                    // end Roshil add 2-23-2021


                    // Roshil added 5-23-2017
                    decimal VAPPerDayInBp = 0;
					if (totalNoTripDaysInsideVacationinBp > 0)
						VAPPerDayInBp = actualVapInBp / totalNoTripDaysInsideVacationinBp;
					int totalVAPDaysInNextBp = totalNoTripDaysInsideVacation - totalNoTripDaysInsideVacationinBp;
					decimal VAPPerDayInNextBp = 0;
					if (totalVAPDaysInNextBp > 0)
						VAPPerDayInNextBp = actualVapInNextBp / totalVAPDaysInNextBp;

					//Modified the code for Company VA implementaltion
					//----------------------------------------------------------------
					if (!string.IsNullOrEmpty(GlobalSettings.CompanyVA) &&
vacPerid.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
					{

						decimal maxPossibleVap = decimal.Parse(GlobalSettings.CompanyVA);
						if (vacPerid.EndAbsenceDate > GlobalSettings.SeniorityListMember.Absences[0].EndAbsenceDate)
						{
							maxPossibleVap +=
								vacPerid.EndAbsenceDate.Subtract(
									GlobalSettings.SeniorityListMember.Absences[0].EndAbsenceDate).Days *
								GlobalSettings.DailyVacPay;
						}


						if (maxPossibleVap > totTfp)
						{
							if (totalNoTripDaysInsideVacation != 0)
							{
								vacTfpPerday = (maxPossibleVap - totTfp) / totalNoTripDaysInsideVacation;
							}
						}

					}
					//----------------------------------------------------------------
					else
					{
						if (GlobalSettings.DailyVacPay * totalVacationDaysinBp > totTfpinBp)

						{
							vacTfpPerday = ((GlobalSettings.DailyVacPay * totalVacationDaysinBp) - totTfpinBp) /
										   totalNoTripDaysInsideVacationinBp;
						}
					}
					if ((GlobalSettings.DailyVacPay * totalVacationDaysinBp) > totTfpinBp)
					{
						line.Value.VacationStateLine.VacationTfp += (totalNoTripDaysInsideVacationinBp * VAPPerDayInBp);
						//line.Value.VacationStateLine.VacationTfp += (totalNoTripDaysInsideVacationinBp * vacTfpPerday);
					}
                    if ((GlobalSettings.DailyVacPay * totalVacationDays) > totTfp)
                    {

                        line.Value.VacationStateLine.VacationTfpInLine += (((totalNoTripDaysInsideVacation - totalNoTripDaysInsideVacationinBp) * VAPPerDayInNextBp) +(totalNoTripDaysInsideVacationinBp * VAPPerDayInBp));
                    }
                    //line.Value.VAPbo += actualVap;
                   // line.Value.VAPbp += actualVapInBp;
				}
               // line.Value.VAPne = line.Value.VAPbo - line.Value.VAPbp;
			}
			catch (Exception ex)
			{

				throw ex;
			}


		}

        private decimal CalculateFlyPay(DateTime tripStartDate, Trip trip, KeyValuePair<string, Line> line)
        {
            decimal flyPay = 0.0m;
            //Trip is In Bid Period
            try
            {
                if (IsTripInBidPeriod(tripStartDate, trip.PairLength))
                {
                    flyPay += trip.Tfp;
                }
                // Trip overlaps  Bid Period
                else
                {
                    decimal tfp = 0.0m;
                    int tafb = 0;
                    int pairLength = 0;
                    foreach (DutyPeriod dp in trip.DutyPeriods)
                    {
                        if (tripStartDate.AddDays(dp.DutPerSeqNum - 1) <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                        {
                            tfp += dp.Tfp;
                            pairLength++;
                            tafb += CalculateTafbInBpOfDutyPeriod(dp, trip.PairLength);
                        }
                    }


                    flyPay += tfp;

                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return flyPay;
        }

        /// <summary>
        /// PURPOSE : Set Initial values for Big 4 properties
        /// </summary>
        /// <param name="line"></param>
        private void SetInitialValues(KeyValuePair<string, Line> line)
        {
            line.Value.VacationStateLine = new VacationStateLine();
            line.Value.VacationStateLine.VacationTfp = 0.0m;
            line.Value.VacationStateLine.VacationDropTfp = 0.0m;

            line.Value.VacationStateLine.VacationDaysOff = 0;
            line.Value.VacationStateLine.VacationDropDaysOff = 0;

            line.Value.VacationStateLine.VacationLegs = 0;
            line.Value.VacationStateLine.VacationDropLegs = 0;

            line.Value.VacationStateLine.VacationTafb = "00:00";
            line.Value.VacationStateLine.VacationDropTafb = "00:00";

            line.Value.VacationStateLine.VacationBlkHrs = "00:00";
            line.Value.VacationStateLine.VacationDropBlkHrs = "00:00";

            line.Value.VacationStateLine.FlyPay = 0.0m;
            line.Value.VacationStateLine.FlyPayInLine = 0.0m;
            line.Value.VacationStateLine.VacationFront = 0.0m;
            line.Value.VacationStateLine.VacationBack= 0.0m;

        }

        private bool IsTripInBidPeriod(DateTime tripStartDate, int tripLength)
        {
            DateTime dpDate = tripStartDate.AddDays(tripLength - 1);
            if (dpDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                return true;
            else
                return false;
        }

        private int CalculateTafbInBpOfDutyPeriod(DutyPeriod dutyPeriod, int lengthOfTrip)
        {

            int dutyPeriodTafb = 0;
            try
            {
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

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return dutyPeriodTafb;
        }


        #endregion

        #region TempClass
        public class TempVap
        {
            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public int Days { get; set; }

            public decimal Tfp { get; set; }

			public bool IsInBp { get; set; }

        }
        #endregion
    }
}
