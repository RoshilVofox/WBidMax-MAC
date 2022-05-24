using System;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;

namespace WBid.WBidMac.Mac.Utility
{
    public class CommuteCalculations
    {
        public CommuteCalculations()
        {
        }
        public FtCommutableLine FtCommutable { get; set; }

        private List<FlightRouteDetails> _flightRouteDetails;
        WBidState _wBIdStateContent;
        private int _depTime;
        private int _arrTime;
        public void CalculateDailyCommutableTimes(FtCommutableLine ftCommutable, List<FlightRouteDetails> flightRouteDetails)
        {
            try
            {
                FtCommutable = ftCommutable;
                _flightRouteDetails = flightRouteDetails;
                if (_flightRouteDetails != null)
                {
                    CalculateCommutableTimes(ftCommutable.City);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<CommuteTime> CalculateDailyCommutableTimesForVacationDifference(FtCommutableLine ftCommutable, List<FlightRouteDetails> flightRouteDetails)
        {
            List<CommuteTime> lstCommuteTime = new List<CommuteTime>();
            try
            {
                FtCommutable = ftCommutable;
                _flightRouteDetails = flightRouteDetails;
                if (_flightRouteDetails != null)
                {
                    lstCommuteTime = CalculateCommutableTimesForFlightDataDiffernce(ftCommutable.City);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstCommuteTime;
        }


        private void CalculateCommutableTimes(string commuteCity)
        {
            try
            {
                _wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                string domicile = GlobalSettings.CurrentBidDetails.Domicile;

                if (_wBIdStateContent.BidAuto == null)
                {
                    _wBIdStateContent.BidAuto = new BidAutomator();
                }
                if (_wBIdStateContent.Constraints.DailyCommuteTimesCmmutability == null)
                {
                    _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability = new List<CommuteTime>();
                }
                _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Clear();

                if (_wBIdStateContent.Weights.DailyCommuteTimes == null)
                {
                    _wBIdStateContent.Weights.DailyCommuteTimes = new List<CommuteTime>();
                }
                _wBIdStateContent.Weights.DailyCommuteTimes.Clear();


                if (_wBIdStateContent.Constraints.DailyCommuteTimes == null)
                {
                    _wBIdStateContent.Constraints.DailyCommuteTimes = new List<CommuteTime>();
                }
                _wBIdStateContent.Constraints.DailyCommuteTimes.Clear();


                DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Date;
                DateTime endDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Date;
                endDate = endDate.AddDays(3);
                int depTimeHhMm = 0301;  /* format is hhmm -- min time is 0301 (3:01AM) */
                int arrTimeHhMm = 2700; /* format is hhmm -- max time is 2700 (3:00AM)   */
                _depTime = depTimeHhMm / 100 * 60 + depTimeHhMm % 100;
                _arrTime = arrTimeHhMm / 100 * 60 + arrTimeHhMm % 100;
                //string selectedConnectTime = FtCommutable.ConnectTime == "--:--" ? "00:00" : SelectedTime;
                int connecttime = FtCommutable.ConnectTime;
                int arrivalcount = 0;
                int depaturecount = 0;
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {

                    CommuteTime commuteTime = new CommuteTime();
                    commuteTime.BidDay = date;
                    commuteTime.EarliestArrivel = DateTime.MinValue;
                    commuteTime.LatestDeparture = DateTime.MinValue;
                    //Calculating earliest arrivel time
                    //------------------------------------------------------------------------
                    var nonConnectFlights = GetNonConnectFlights(commuteCity, domicile, date);

                    var oneAndZeroConnectFlights = nonConnectFlights;
                    //if (IsNonStopOnly != true)
                    if (FtCommutable.IsNonStopOnly != true)
                    {
                        var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date, connecttime);
                        oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                    }


                    if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
                    {

                        double earliestArrivelTime = oneAndZeroConnectFlights.OrderBy(x => x.RtArr).FirstOrDefault().RtArr;
                        commuteTime.EarliestArrivel = date.Date.AddMinutes(earliestArrivelTime);
                    }
                    else
                    {
                        arrivalcount++;
                        commuteTime.EarliestArrivel = DateTime.MinValue;

                    }
                    //----------------------------------------------------------------------------

                    //Calculating latest departure
                    //------------------------------------------------------------------------
                    nonConnectFlights = GetNonConnectFlights(domicile, commuteCity, date);

                    oneAndZeroConnectFlights = nonConnectFlights;
                    // (IsNonStopOnly != true)
                    if (FtCommutable.IsNonStopOnly != true)
                    {
                        var oneConnectFlights = GetOneConnectFlights(domicile, commuteCity, date, connecttime);
                        oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                    }


                    if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
                    {

                        double latestDepartureTime = oneAndZeroConnectFlights.OrderByDescending(x => x.RtDep).FirstOrDefault().RtDep;
                        commuteTime.LatestDeparture = date.Date.AddMinutes(latestDepartureTime);


                    }
                    else
                    {
                        depaturecount++;
                        commuteTime.LatestDeparture = DateTime.MinValue;

                    }
                    _wBIdStateContent.Constraints.DailyCommuteTimesCmmutability.Add(commuteTime);
                    //----------------------------------------------------------------------------

                    _wBIdStateContent.Weights.DailyCommuteTimes.Add(commuteTime);

                    _wBIdStateContent.Constraints.DailyCommuteTimes.Add(commuteTime);


                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        private List<CommuteTime> CalculateCommutableTimesForFlightDataDiffernce(string commuteCity)
        {
            List<CommuteTime> lstCommuteTimes = new List<CommuteTime>();
            try
            {

                string domicile = GlobalSettings.CurrentBidDetails.Domicile;

                DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Date;
                DateTime endDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Date;
                endDate = endDate.AddDays(3);
                int depTimeHhMm = 0301;  /* format is hhmm -- min time is 0301 (3:01AM) */
                int arrTimeHhMm = 2700; /* format is hhmm -- max time is 2700 (3:00AM)   */
                _depTime = depTimeHhMm / 100 * 60 + depTimeHhMm % 100;
                _arrTime = arrTimeHhMm / 100 * 60 + arrTimeHhMm % 100;
                //string selectedConnectTime = FtCommutable.ConnectTime == "--:--" ? "00:00" : SelectedTime;
                int connecttime = FtCommutable.ConnectTime;
                int arrivalcount = 0;
                int depaturecount = 0;
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {

                    CommuteTime commuteTime = new CommuteTime();
                    commuteTime.BidDay = date;
                    commuteTime.EarliestArrivel = DateTime.MinValue;
                    commuteTime.LatestDeparture = DateTime.MinValue;
                    //Calculating earliest arrivel time
                    //------------------------------------------------------------------------
                    var nonConnectFlights = GetNonConnectFlights(commuteCity, domicile, date);

                    var oneAndZeroConnectFlights = nonConnectFlights;
                    //if (IsNonStopOnly != true)
                    if (FtCommutable.IsNonStopOnly != true)
                    {
                        var oneConnectFlights = GetOneConnectFlights(commuteCity, domicile, date, connecttime);
                        oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                    }


                    if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
                    {

                        double earliestArrivelTime = oneAndZeroConnectFlights.OrderBy(x => x.RtArr).FirstOrDefault().RtArr;
                        commuteTime.EarliestArrivel = date.Date.AddMinutes(earliestArrivelTime);
                    }
                    else
                    {
                        arrivalcount++;
                        commuteTime.EarliestArrivel = DateTime.MinValue;

                    }
                    //----------------------------------------------------------------------------

                    //Calculating latest departure
                    //------------------------------------------------------------------------
                    nonConnectFlights = GetNonConnectFlights(domicile, commuteCity, date);

                    oneAndZeroConnectFlights = nonConnectFlights;
                    // (IsNonStopOnly != true)
                    if (FtCommutable.IsNonStopOnly != true)
                    {
                        var oneConnectFlights = GetOneConnectFlights(domicile, commuteCity, date, connecttime);
                        oneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).ToList();
                    }


                    if (oneAndZeroConnectFlights != null && oneAndZeroConnectFlights.Count > 0)
                    {

                        double latestDepartureTime = oneAndZeroConnectFlights.OrderByDescending(x => x.RtDep).FirstOrDefault().RtDep;
                        commuteTime.LatestDeparture = date.Date.AddMinutes(latestDepartureTime);


                    }
                    else
                    {
                        depaturecount++;
                        commuteTime.LatestDeparture = DateTime.MinValue;

                    }
                    lstCommuteTimes.Add(commuteTime);
                    //----------------------------------------------------------------------------


                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstCommuteTimes;
        }

        private List<RouteDomain> GetNonConnectFlights(string depSta, string arrSta, DateTime dateTime)
        {
            try
            {
                List<RouteDomain> nonConnectFlights = null;

                nonConnectFlights = _flightRouteDetails
                                   .Where(x => x.Orig == depSta && x.Dest == arrSta && x.FlightDate == dateTime && x.Cdep >= _depTime && x.Carr <= _arrTime)

                                .Select(y =>
                                  new RouteDomain
                                  {
                                      Date = y.FlightDate,
                                      Route = y.Orig + '-' + y.Dest,
                                      RtDep = y.Cdep,
                                      RtArr = y.Carr,
                                      RtTime = y.Carr - y.Cdep,
                                      Rt1 = y.RouteNum,
                                      Rt2 = 0,
                                      Rt3 = 0,
                                      Rt1Dep = y.Cdep,
                                      Rt2Dep = 0,
                                      Rt3Dep = 0,
                                      Rt1Arr = y.Carr,
                                      Rt2Arr = 0,
                                      Rt3Arr = 0,
                                      Con1 = 0,
                                      Con2 = 0,
                                      Rt1Orig = y.Orig,
                                      Rt2Orig = "",
                                      Rt3Orig = "",
                                      Rt1Dest = y.Dest,
                                      Rt2Dest = "",
                                      Rt3Dest = "",
                                      Rt1FltNum = y.Flight,
                                      Rt2FltNum = 0,
                                      Rt3FltNum = 0


                                  }).ToList()
                                  .OrderBy(z => z.Route).ThenBy(z1 => z1.RtTime).ToList();
                //.ThenBy(z1 => z1.RtTime).ToList();
                return nonConnectFlights;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<RouteDomain> GetOneConnectFlights(string depSta, string arrSta, DateTime dateTime, int connectTime)
        {
            try
            {
                //int connectTime = 30;
                List<RouteDomain> oneConnectFlights = null;

                oneConnectFlights = _flightRouteDetails.Where(frd1 => frd1.Orig == depSta && frd1.FlightDate == dateTime).Join(_flightRouteDetails.Where(frd2 => frd2.Dest == arrSta && frd2.FlightDate == dateTime), f1 => f1.Dest, f2 => f2.Orig,
                                                          (f1, f2) => new { ff1 = f1, ff2 = f2 }).ToList()
                                                          .Where(x =>
                                                            x.ff1.Dest != arrSta
                                                            && x.ff1.Cdep >= _depTime && x.ff2.Carr <= _arrTime
                                                             && (x.ff1.Carr + connectTime <= x.ff2.Cdep || x.ff1.RouteNum == x.ff2.RouteNum) && x.ff2.Cdep > x.ff1.Cdep
                                                          )
                                                        .Select(y =>
                                                        new RouteDomain
                                                        {
                                                            Date = y.ff1.FlightDate,
                                                            Route = y.ff1.Orig + '-' + y.ff1.Dest + '-' + y.ff2.Dest,
                                                            RtDep = y.ff1.Cdep,
                                                            RtArr = y.ff2.Carr,
                                                            RtTime = y.ff2.Carr - y.ff1.Cdep,
                                                            Rt1 = y.ff1.RouteNum,
                                                            Rt2 = y.ff2.RouteNum,
                                                            Rt3 = 0,
                                                            Rt1Dep = y.ff1.Cdep,
                                                            Rt2Dep = y.ff2.Cdep,
                                                            Rt3Dep = 0,
                                                            Rt1Arr = y.ff1.Carr,
                                                            Rt2Arr = y.ff2.Carr,
                                                            Rt3Arr = 0,
                                                            Con1 = y.ff2.Cdep - y.ff1.Carr,
                                                            Con2 = 0,
                                                            Rt1Orig = y.ff1.Orig,
                                                            Rt2Orig = y.ff2.Orig,
                                                            Rt3Orig = "",
                                                            Rt1Dest = y.ff1.Dest,
                                                            Rt2Dest = y.ff2.Dest,
                                                            Rt3Dest = "",
                                                            Rt1FltNum = y.ff1.Flight,
                                                            Rt2FltNum = y.ff2.Flight,
                                                            Rt3FltNum = 0

                                                        }).ToList()
                                                        .OrderBy(z => z.Route).ThenBy(z1 => z1.RtTime).ToList();
                return oneConnectFlights;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
