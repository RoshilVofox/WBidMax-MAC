#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
// System.IO.Compression;
using WBid.WBidiPad.Model;
#endregion

namespace WBid.WBidiPad.SharedLibrary.Utility
{
    public class NetworkPlanData
    {
        #region Public Methods

        /// <summary>
        /// PURPOSe : Get Flight Routes
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        //public List<FlightRouteDetails> GetFlightRoutes(DateTime fromDate, DateTime toDate)
        //{
        //    try
        //    {
        //        WBid.WBidiPad.SharedLibrary.NetworkPlanServiceReference.NetworkPlanServiceClient client = new WBid.WBidiPad.SharedLibrary.NetworkPlanServiceReference.NetworkPlanServiceClient();
        //        WBid.WBidiPad.SharedLibrary.NetworkPlanServiceReference.FlightPlan flightPlan = client.GetFlights(fromDate, toDate);
        //        GlobalSettings.FlightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
        //                    (fr, f) =>
        //                   new FlightRouteDetails
        //                   {
        //                       Flight = f.FlightId,
        //                       FlightDate = fr.FlightDate,
        //                       Orig = f.Orig,
        //                       Dest = f.Dest,
        //                       Cdep = f.Cdep,
        //                       Carr = f.Carr,
        //                       Ldep = f.Ldep,
        //                       Larr = f.Larr,
        //                       RouteNum = fr.RouteNum,

        //                   }).ToList();
        //     //   WBidHelper.SerializeObject("File\\ss.txt",GlobalSettings.FlightRouteDetails);
        //      //  ZipFile.CreateFromDirectory("File", "File.zip");
                
        //        //GlobalSettings.FlightRouteDetails = GlobalSettings.FlightRouteDetails.Take(50000).ToList();
        //        //GlobalSettings.FlightRouteDetails = GlobalSettings.FlightRouteDetails.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        GlobalSettings.FlightRouteDetails = null;
                
        //    }
        //    return GlobalSettings.FlightRouteDetails;

        //}

        /// <summary>
        /// PURPOSE : Check pilot Deadhead
        /// </summary>
        /// <param name="deadHeadParameter"></param>
        /// <returns></returns>
        public List<DeadHeadResult> CheckPilotDeadhead(DeadHeadParameter deadHeadParameter, Flight originalFlight, bool deadHeadToDom, ref List<DeadHeadResult> pltDh )
        {
            int routeNumber = 0;
            List<RouteDomain> nonConnectFlights = new List<RouteDomain>();
            List<RouteDomain> oneConnectFlights = new List<RouteDomain>();
            List<RouteDomain> twoConnectFlights = new List<RouteDomain>();
            List<RouteDomain> twoAndOneAndZeroConnectFlights = new List<RouteDomain>();
            int flightdepttime;
            // frank add 6/25/2019
            if (originalFlight.DepTime % 1440 < GlobalSettings.LastLandingMinus1440)
            {
                //originalFlight.DepTime = (originalFlight.DepTime % 1440) + 1440;
                flightdepttime = (originalFlight.DepTime % 1440) + 1440;
            }
            else
            {
                //originalFlight.DepTime = originalFlight.DepTime % 1440;
                flightdepttime = originalFlight.DepTime % 1440;
            }
            // end frank add 6/25/2019

            FlightRouteDetails originalFlightToJoin = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == deadHeadParameter.Date &&
                                                                                              // frank add 6/25/2019
                                                                                              //x.Cdep == (originalFlight.DepTime % 1440) &&
                                                                                              x.Cdep == flightdepttime &&
                                                                           // end frank add 6/25/2019
                                                                           x.Orig == originalFlight.DepSta &&
                                                                           x.Dest == originalFlight.ArrSta).FirstOrDefault();

            if (originalFlightToJoin != null)
            {
                routeNumber = originalFlightToJoin.RouteNum;
            }


            deadHeadParameter.ArrSta = deadHeadParameter.ArrSta.ToUpper();
            deadHeadParameter.DepSta = deadHeadParameter.DepSta.ToUpper();

            //convert HHMM time to minutes
            int depTime = deadHeadParameter.DepTimeHhMm / 100 * 60 + deadHeadParameter.DepTimeHhMm % 100;
            int arrTime = deadHeadParameter.ArrTimeHhMm / 100 * 60 + deadHeadParameter.ArrTimeHhMm % 100;

            // List<FlightRouteDetails> flightRouteDetailsTemp = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == deadHeadParameter.Date).ToList();

            List<FlightRouteDetails> flightRouteDetails = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == deadHeadParameter.Date && x.Cdep >= depTime && x.Carr <= arrTime).ToList();

            //Zero Connect (non-stop)
            nonConnectFlights = GetNonConnectFlights(flightRouteDetails, deadHeadParameter, depTime, arrTime);

            //one connect 
            oneConnectFlights = GetOneConnectFlights(flightRouteDetails, deadHeadParameter, depTime, arrTime);

            //Two connect
            twoConnectFlights = GetTwoConnectFlights(flightRouteDetails, deadHeadParameter, depTime, arrTime);

            //Union all
            twoAndOneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).Union(twoConnectFlights).ToList();

            //List<DeadHeadResult> resultList = twoAndOneAndZeroConnectFlights.GroupBy(x => x.Route).Select(i => i.OrderByDescending(y => y.RtDep).First()).ToList()
            //            .Select(y =>
            //                          new DeadHeadResult
            //                          {
            //                              Route = y.Route,
            //                              RtArr = Convert.ToString(y.RtArr / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtArr % 60).PadLeft(2, '0'),
            //                              RtDep = Convert.ToString(y.RtDep / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtDep % 60).PadLeft(2, '0'),
            //                              RtTime = Convert.ToString(y.RtTime / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtTime % 60).PadLeft(2, '0')
            //                          }).ToList();

            // filter for same plane (routeNum)

            List<DeadHeadResult> resultList = new List<DeadHeadResult>();
            //if (resultList.Count == 0) return resultList;

            if (deadHeadToDom)  // VOF
            {
                //resultList = twoAndOneAndZeroConnectFlights.GroupBy(x => x.Route).Select(i => i.OrderByDescending(y => y.RtDep).First()).ToList()
                     resultList = twoAndOneAndZeroConnectFlights.ToList()
                        // filter for dh flight not being same route Number => dh back to domicile needs to comply with connect times in contract
                        .Where(y => ((y.Rt1 != 0) && (y.Rt1 == routeNumber)) ||
                              ((y.Rt1 != 0) && (y.Rt1Dep >= (originalFlight.ArrTime % 1440 + deadHeadParameter.ConnectTime))))
                        .Select(y =>
                                      new DeadHeadResult
                                      {
                                          Route = y.Route,
                                          RtArr = Convert.ToString(y.RtArr / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtArr % 60).PadLeft(2, '0'),
                                          RtDep = Convert.ToString(y.RtDep / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtDep % 60).PadLeft(2, '0'),
                                          RtTime = Convert.ToString(y.RtTime / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtTime % 60).PadLeft(2, '0')
                                      }).ToList();
            }
            else                // VOB
            {
                //resultList = twoAndOneAndZeroConnectFlights.GroupBy(x => x.Route).Select(i => i.OrderByDescending(y => y.RtDep).First()).ToList()
                //        // filter for dh flight not being same rounte Number => dh from domicile needs to comply with connect times in contract
                //        .Where(y => ((y.Rt3 != 0) && (y.Rt3 == routeNumber)) || 
                //              ((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2 == routeNumber)) ||
                //              ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1 != 0) && (y.Rt1 == routeNumber)) ||
                //              ((y.Rt3 != 0) && (y.Rt3Arr <= (originalFlight.ArrTime % 1440 - deadHeadParameter.ConnectTime))) ||
                //              ((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2Arr <= (originalFlight.DepTime % 1440 - deadHeadParameter.ConnectTime))) ||
                //              ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1Arr <= (originalFlight.DepTime % 1440 - deadHeadParameter.ConnectTime))))
                //        .Select(y =>
                //                      new DeadHeadResult
                //                      {
                //                          Route = y.Route,
                //                          RtArr = Convert.ToString(y.RtArr / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtArr % 60).PadLeft(2, '0'),
                //                          RtDep = Convert.ToString(y.RtDep / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtDep % 60).PadLeft(2, '0'),
                //                          RtTime = Convert.ToString(y.RtTime / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtTime % 60).PadLeft(2, '0')
                //                      }).ToList();

                // the following query is the same as above but I removed the grouping.  The Grouping would not allow legal dh routes to flow through.  See CACI, Den, 1st Round Mar

                resultList = twoAndOneAndZeroConnectFlights.Where(y => ((y.Rt3 != 0) && (y.Rt3 == routeNumber)) ||
                              ((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2 == routeNumber)) ||
                              ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1 != 0) && (y.Rt1 == routeNumber)) ||
                              ((y.Rt3 != 0) && (y.Rt3Arr <= (originalFlight.ArrTime % 1440 - deadHeadParameter.ConnectTime))) ||
                                                                  // frank add 6/25/2019
                                                                  //((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2Arr <= (originalFlight.DepTime % 1440 - deadHeadParameter.ConnectTime))) ||
                                                                  //((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1Arr <= (originalFlight.DepTime % 1440 - deadHeadParameter.ConnectTime))))
                                                                  // removed % from originalFlight.DepTime since it was handled in lines 347 to 356 above
                                                                  ((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2Arr <= (flightdepttime - deadHeadParameter.ConnectTime))) ||
                                                                  ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1Arr <= (flightdepttime - deadHeadParameter.ConnectTime))))
                        // end frank add 6/25/2019
                        .Select(y =>
                                      new DeadHeadResult
                                      {
                                          Route = y.Route,
                                          RtArr = Convert.ToString(y.RtArr / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtArr % 60).PadLeft(2, '0'),
                                          RtDep = Convert.ToString(y.RtDep / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtDep % 60).PadLeft(2, '0'),
                                          RtTime = Convert.ToString(y.RtTime / 60).PadLeft(2, '0') + ":" + Convert.ToString(y.RtTime % 60).PadLeft(2, '0')
                                      }).ToList();

            }

            pltDh = resultList;
            return resultList;

        }


        /// <summary>
        /// PURPOSE : Check Cost of ReserveDH
        /// </summary>
        /// <param name="deadHeadParameter"></param>
        /// <returns></returns>
        public decimal CheckCostOfReserveDH(DeadHeadParameter deadHeadParameter, Flight flight, VacationTrip vacTrip, Trip trip, DateTime tripDate, bool rcWasZero, ref List<RouteDomain> resDh)
        {
            decimal tfp = 0.0m;

            List<RouteDomain> nonConnectFlights = new List<RouteDomain>();
            List<RouteDomain> oneConnectFlights = new List<RouteDomain>();
            List<RouteDomain> twoConnectFlights = new List<RouteDomain>();
            List<RouteDomain> twoAndOneAndZeroConnectFlights = new List<RouteDomain>();

            List<RouteDomain> test = new List<RouteDomain>();

           

            int routeNumber = 0;
            bool isReserverDayBeforeOrAfter = vacTrip.VDvsRCdata.RCisDayPriorOrAfter;
            // the dutPdSeq is the day of split
            int dutPdSeq = vacTrip.VDvsRCdata.RCcalcDutyPeriod;
            // if fltSeq = 0 beginning of day is split, if fltSeq = 9 end of day is split
            int fltSeq = vacTrip.VDvsRCdata.RCcalcFlightSeqNum;
            bool resDhToDom = vacTrip.VacationType == "VOB"?true:false;
            int numOfFlightsInDp = trip.DutyPeriods[dutPdSeq-1].Flights.Count;
            //Flight originalFlight = flight;
            //Flight originalFlight = deadHeadParameter.RefFlt;
            Flight originalFlight = deadHeadParameter.RefFlt==null?new Flight(): deadHeadParameter.RefFlt;
            //if (resDhToDom)
            //    originalFlight=trip.DutyPeriods[dutPdSeq-1];

            // need to pass in rcWasZero to get correct date for originalFlight -- NO not necessary for day before or after
            // VOF and fltSeq == 0 should be referencing seq 1 flight for  dep time as that is res arr time, also ref seq 1 for routeNum


            var originalFlightDetails = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == deadHeadParameter.Date &&
                                                                           x.Cdep == (originalFlight.DepTime % 1440) &&
                                                                           x.Orig == originalFlight.DepSta &&
                                                                           x.Dest == originalFlight.ArrSta).FirstOrDefault();

            if (originalFlightDetails != null)
            {
                routeNumber = originalFlightDetails.RouteNum;
            }

            int connect = deadHeadParameter.ConnectTime;
            int origArr = originalFlight.ArrTime % 1440;
            
            deadHeadParameter.ArrSta = deadHeadParameter.ArrSta.ToUpper();
            deadHeadParameter.DepSta = deadHeadParameter.DepSta.ToUpper();

            //convert HHMM time to minutes
            int depTime = deadHeadParameter.DepTimeHhMm / 100 * 60 + deadHeadParameter.DepTimeHhMm % 100;
            int arrTime = deadHeadParameter.ArrTimeHhMm / 100 * 60 + deadHeadParameter.ArrTimeHhMm % 100;



            List<FlightRouteDetails> flightRouteDetails = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == deadHeadParameter.Date && x.Cdep >= depTime && x.Carr <= arrTime).ToList();


            //Zero Connect (non-stop)
            nonConnectFlights = GetNonConnectFlights(flightRouteDetails, deadHeadParameter, depTime, arrTime);

            //one connect 
            oneConnectFlights = GetOneConnectFlights(flightRouteDetails, deadHeadParameter, depTime, arrTime);

            //Two connect
            twoConnectFlights = GetTwoConnectFlights(flightRouteDetails, deadHeadParameter, depTime, arrTime);

            //Union all
            twoAndOneAndZeroConnectFlights = nonConnectFlights.Union(oneConnectFlights).Union(twoConnectFlights).ToList();

            test = twoAndOneAndZeroConnectFlights;

            // no filter needed if rcWasZero is true

            if (!rcWasZero && vacTrip.VacationType == "VOF")
                // filter out for connect time if not same plane.  RC deadhead is from domicile to city, so check flight depTime - connect
                //twoAndOneAndZeroConnectFlights = twoAndOneAndZeroConnectFlights.GroupBy(x => x.Route).Select(i => i.OrderByDescending(y => y.RtDep).First()).ToList()
                      twoAndOneAndZeroConnectFlights = twoAndOneAndZeroConnectFlights.ToList()
                        .Where(y => ((y.Rt3 != 0) && (y.Rt3 == routeNumber)) ||
                              ((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2 == routeNumber)) ||
                              ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1 != 0) && (y.Rt1 == routeNumber)) ||
                              ((y.Rt3 != 0) && (y.Rt3Arr <= (originalFlight.DepTime % 1440 - deadHeadParameter.ConnectTime))) ||
                              //((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2Arr <= (originalFlight.ArrTime % 1440 - deadHeadParameter.ConnectTime))) ||
                             // ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1Arr <= (originalFlight.ArrTime % 1440 - deadHeadParameter.ConnectTime))) 
						((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2Arr <= (originalFlight.DepTime % 1440 - deadHeadParameter.ConnectTime))) ||
						((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1Arr <= (originalFlight.DepTime % 1440 - deadHeadParameter.ConnectTime))) 
                              )
                            .Select(y => y).ToList();
            else if (!rcWasZero && vacTrip.VacationType == "VOB")
                // filter out for connect time if not same plane.  RC deadhead is from city to domicile, so check flight arrTime + connect  
                //twoAndOneAndZeroConnectFlights = twoAndOneAndZeroConnectFlights.GroupBy(x => x.Route).Select(i => i.OrderByDescending(y => y.RtDep).First()).ToList()
                //            .Where(y => ((y.Rt3 != 0) && (y.Rt3 == y.Rt2)) ||
                //                  ((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2 == y.Rt1)) ||
                //                  ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1 != 0) && (y.Rt1 == routeNumber)) ||
                //                  ((y.Rt3 != 0) && (y.Rt3Dep <= (originalFlight.ArrTime % 1440 - deadHeadParameter.ConnectTime))) ||
                //                  ((y.Rt3 == 0) && (y.Rt2 != 0) && (y.Rt2Arr <= (originalFlight.ArrTime % 1440 + deadHeadParameter.ConnectTime))) ||
                //                  ((y.Rt3 == 0) && (y.Rt2 == 0) && (y.Rt1Arr <= (originalFlight.ArrTime % 1440 + deadHeadParameter.ConnectTime))))
                //            .Select(y => y).ToList();

				//twoAndOneAndZeroConnectFlights = twoAndOneAndZeroConnectFlights.GroupBy(x => x.Route).Select(i => i.OrderByDescending(y => y.RtDep).First()).ToList()
                    twoAndOneAndZeroConnectFlights = twoAndOneAndZeroConnectFlights.ToList()
                    .Where(y => /****   Three Flights   ****/
						// t-t-t all 3 flights are same plane
						((routeNumber == y.Rt1) && (y.Rt1 == y.Rt2) && (y.Rt2 == y.Rt3)) ||
						// t-t-f first 2 flights are same plane, and 3rd flight uses connect (40) time
						((routeNumber == y.Rt1) && (y.Rt1 == y.Rt2) && ((y.Rt2Arr + connect <= y.Rt3Dep) && (y.Rt3 !=0))) ||
						// t-f-t first flight is same and flight 2 uses connect time and flight 3 is same as flight 2 routeNum
						((routeNumber == y.Rt1) && ((y.Rt1Arr + connect <= y.Rt2Dep) && (y.Rt2 != 0)) && (y.Rt2 == y.Rt3)) ||
						// t-f-f first flight is same and both flight 2 and 3 use connect time.
						((routeNumber == y.Rt1) && ((y.Rt1Arr + connect <= y.Rt2Dep) && (y.Rt2 != 0)) && ((y.Rt2Arr + connect <= y.Rt3Dep) && (y.Rt3 !=0))) ||
						// f-t-t first flight uses connect time and flight 2 and 3 are same routeNum
						((origArr + connect <= y.Rt1Dep) && (y.Rt1 == y.Rt2) && (y.Rt2 == y.Rt3)) ||
						// f-t-f first flight uses connect time and flight 2 is same as flight 1 and flight 3 uses connect time
						((origArr + connect <= y.Rt1Dep) && (y.Rt1 == y.Rt2) && ((y.Rt2Arr + connect <= y.Rt3Dep) && (y.Rt3 !=0))) ||
						// f-f-t first flight uses connect time and flight 2 uses connect time and flight 2 and 3 are the same routeNum
						((origArr + connect <= y.Rt1Dep) && ((y.Rt1Arr + connect <= y.Rt2Dep) && (y.Rt2 != 0)) && (y.Rt2 == y.Rt3)) ||
						// f-f-f all 3 flights use connect time 
						((origArr + connect <= y.Rt1Dep) && ((y.Rt1Arr + connect <= y.Rt2Dep) && (y.Rt2 != 0)) && ((y.Rt2Arr + connect <= y.Rt3Dep) && (y.Rt3 !=0))) ||
						/****    Two Flights   ****/
						// f-t only 2 flights, first flight uses connect and flight 2 is same routeNum as flight 1
						((origArr + connect <= y.Rt1Dep) && ((y.Rt1 == y.Rt2) && (y.Rt3 == 0))) ||
						// f-f only two flights and both flights use connect time
						((origArr + connect <= y.Rt1Dep) && ((y.Rt1Arr + connect <= y.Rt2Dep) && (y.Rt3 == 0))) ||
						// t-t only 2 flights and both flights have same routeNum
						((routeNumber == y.Rt1) && (y.Rt1 == y.Rt2) && (y.Rt3 == 0)) ||
						// t-f only 2 flights and 1st flight has same routeNum and flight 2 uses connect time
						((routeNumber == y.Rt1) && (y.Rt1Arr + connect <= y.Rt2Dep) && (y.Rt3 == 0)) ||
						/****     One Flight   ****/
						// t only 1 flight and flight 1 is routeNum
						((routeNumber == y.Rt1) && (y.Rt2 == 0)) ||
						// f only 1 fight and it uses connect time
						((origArr + connect <= y.Rt1Dep) && ((y.Rt2 == 0)))

					)
					.Select(y => y).ToList();

                //twoAndOneAndZeroConnectFlights = twoAndOneAndZeroConnectFlights.GroupBy(x => x.Route).Select(i => i.OrderByDescending(y => y.RtDep).First()).ToList()
                //            .Where(y => ((y.Rt1 != 0) && (y.Rt1 == routeNumber)) ||
                //                  (y.Rt1Dep >= (originalFlight.ArrTime % 1440 + deadHeadParameter.ConnectTime)))
                //            .Select(y => y).ToList();
            
           // List<TfpDomain> sss = twoAndOneAndZeroConnectFlights.Select(y => CalculateTfp(y)).ToList().OrderBy(z => z.TotTfp).ToList();
          TfpDomain tfpDomain=  twoAndOneAndZeroConnectFlights.Select(y =>CalculateTfp(y)).ToList().OrderBy(z=>z.TotTfp).FirstOrDefault();

          if (tfpDomain != null)
          {
              tfp = tfpDomain.TotTfp;
              var myResDh = twoAndOneAndZeroConnectFlights.First();
              resDh.Add(myResDh);
          }
          //twoAndOneAndZeroConnectFlights = null;
          //oneConnectFlights = null;
          //resDh = twoAndOneAndZeroConnectFlights;
          
          return tfp;

        }

       

        #endregion


        #region Private Methods
        /// <summary>
        /// PURPOSE :Get Non Connect Flights
        /// </summary>
        /// <param name="deadHeadParameter"></param>
        /// <param name="depTime"></param>
        /// <param name="arrTime"></param>
        /// <returns></returns>
        private List<RouteDomain> GetNonConnectFlights(List<FlightRouteDetails> flightRouteDetails, DeadHeadParameter deadHeadParameter, int depTime, int arrTime)
        {

            List<RouteDomain> nonConnectFlights = null;

            nonConnectFlights = flightRouteDetails
                               .Where(x => x.Orig == deadHeadParameter.DepSta && x.Dest == deadHeadParameter.ArrSta
                              && x.Cdep >= depTime && x.Carr <= arrTime && x.FlightDate == deadHeadParameter.Date)

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
            return nonConnectFlights;
        }

        /// <summary>
        /// PURPOSE :Get One Connect Flights
        /// </summary>
        /// <param name="deadHeadParameter"></param>
        /// <param name="depTime"></param>
        /// <param name="arrTime"></param>
        /// <returns></returns>
        private List<RouteDomain> GetOneConnectFlights(List<FlightRouteDetails> flightRouteDetails, DeadHeadParameter deadHeadParameter, int depTime, int arrTime)
        {

            List<RouteDomain> oneConnectFlights = null;


            ///Old code --------------Should remove later 
            //-------------------
             //oneConnectFlights = flightRouteDetails.Join(flightRouteDetails, f1 => f1.Dest, f2 => f2.Orig,
             //                                         (f1, f2) => new { ff1 = f1, ff2 = f2 }).ToList()
             //                                         .Where(x => x.ff1.Orig == deadHeadParameter.DepSta && x.ff2.Dest == deadHeadParameter.ArrSta
             //                                           && x.ff1.Cdep >= depTime && x.ff2.Carr <= arrTime
             //                                           && x.ff1.FlightDate == deadHeadParameter.Date && x.ff2.FlightDate == deadHeadParameter.Date
             //                                           && x.ff1.Dest != deadHeadParameter.ArrSta
             //                                           && (x.ff1.Carr + deadHeadParameter.ConnectTime <= x.ff2.Cdep || x.ff1.RouteNum == x.ff2.RouteNum)
             //                                           && x.ff2.Cdep > x.ff1.Cdep)
             //                                       .Select(y =>
            //----------------------------------------

            //Included  filter to first list and second list
            //Included filter .Orig == deadHeadParameter.DepSta to the first flight list
            //Included filter .Dest == deadHeadParameter.ArrSta to the second flight list

            oneConnectFlights = flightRouteDetails.Where(frd1 => frd1.Orig == deadHeadParameter.DepSta).Join(flightRouteDetails.Where(frd2 => frd2.Dest == deadHeadParameter.ArrSta), f1 => f1.Dest, f2 => f2.Orig,
                                                      (f1, f2) => new { ff1 = f1, ff2 = f2 }).ToList()
                                                      .Where(x =>
                                                        //ranish--Reason for commenting the following statement is we have already included "DepSta" filter for the first flight list  and "ArrSta" ilter for the second flight list 
                                                       //x.ff1.Orig == deadHeadParameter.DepSta && x.ff2.Dest == deadHeadParameter.ArrSta
                                                       
                                                       //ranish--we are alredy  filtered  "deptime" , "arrTime"  and "flightdate"just before calling "GetOneConnectFlights" method.
                                                       // && x.ff1.Cdep >= depTime && x.ff2.Carr <= arrTime
                                                       // && x.ff1.FlightDate == deadHeadParameter.Date && x.ff2.FlightDate == deadHeadParameter.Date
                                                        //&&
                                                        x.ff1.Dest != deadHeadParameter.ArrSta
                                                        && (x.ff1.Carr + deadHeadParameter.ConnectTime <= x.ff2.Cdep || x.ff1.RouteNum == x.ff2.RouteNum)
                                                        && x.ff2.Cdep > x.ff1.Cdep)
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

        /// <summary>
        /// PURPOSE : Get Two Connect Flights
        /// </summary>
        /// <param name="deadHeadParameter"></param>
        /// <param name="depTime"></param>
        /// <param name="arrTime"></param>
        /// <returns></returns>
        private List<RouteDomain> GetTwoConnectFlights(List<FlightRouteDetails> flightRouteDetails, DeadHeadParameter deadHeadParameter, int depTime, int arrTime)
        {
            
            List<RouteDomain> twoConnectFlights = null;
            
            
            ///Old code --------------Should remove later 
            //-------------------
              //twoConnectFlights = flightRouteDetails.Join(flightRouteDetails, f1 => f1.Dest, f2 => f2.Orig,
              //                                     (f1, f2) => new { ff1 = f1, ff2 = f2 })
              //                                     .Join(flightRouteDetails, tf1 => tf1.ff2.Dest, f3 => f3.Orig,
              //                                     (tf1, f3) => new { tf1, f3 }).ToList()
              //                                      .Where(x => x.tf1.ff1.Orig == deadHeadParameter.DepSta && x.f3.Dest == deadHeadParameter.ArrSta
              //                                       && x.tf1.ff1.Cdep >= depTime && x.f3.Carr <= arrTime
              //                                       && x.tf1.ff1.FlightDate == deadHeadParameter.Date && x.tf1.ff2.FlightDate == deadHeadParameter.Date && x.f3.FlightDate == deadHeadParameter.Date
              //                                       && x.tf1.ff1.Dest != deadHeadParameter.ArrSta && x.tf1.ff2.Dest != deadHeadParameter.ArrSta
              //                                       && (x.tf1.ff1.Carr + deadHeadParameter.ConnectTime <= x.tf1.ff2.Cdep || x.tf1.ff1.RouteNum == x.tf1.ff2.RouteNum)
              //                                       && (x.tf1.ff2.Carr + deadHeadParameter.ConnectTime <= x.f3.Cdep || x.tf1.ff2.RouteNum == x.f3.RouteNum)
              //                                       && x.f3.Cdep > x.tf1.ff2.Cdep && x.tf1.ff2.Cdep > x.tf1.ff1.Cdep
              //                                       && x.tf1.ff1.Orig != x.f3.Orig
              //                                      )
              //                                      .Select(y =>

            //----------------------------------------
            var count1 = flightRouteDetails.Where(x => x.Orig == deadHeadParameter.DepSta).Count();
            var count2 = flightRouteDetails.Where(y => y.Dest == deadHeadParameter.ArrSta).Count();

            //Included  filter to first flight list and last(third) flight list
            //Included filter .Orig == deadHeadParameter.DepSta to the first flight list
            //Included filter .Dest == deadHeadParameter.ArrSta to the third flight list

            twoConnectFlights = flightRouteDetails.Where(frd1 => frd1.Orig == deadHeadParameter.DepSta).Join(flightRouteDetails, f1 => f1.Dest, f2 => f2.Orig,
                                                   (f1, f2) => new { ff1 = f1, ff2 = f2 })
                                                   .Join(flightRouteDetails.Where(frd3 => frd3.Dest == deadHeadParameter.ArrSta), tf1 => tf1.ff2.Dest, f3 => f3.Orig,
                                                   (tf1, f3) => new { tf1, f3 }).ToList()
                                                    .Where(x =>
                                                        //ranish--Reason for commenting the following statement is we have already included "DepSta" filter for the first flight list  and "ArrSta" filter for the third flight list 
                                                      //x.tf1.ff1.Orig == deadHeadParameter.DepSta && x.f3.Dest == deadHeadParameter.ArrSta

                                                      //ranish--we are alredy  filtered  "deptime" , "arrTime"  and "flightdate"just before calling "GetOneConnectFlights" method.
                                                      //&& x.tf1.ff1.Cdep >= depTime && x.f3.Carr <= arrTime
                                                     //&& x.tf1.ff1.FlightDate == deadHeadParameter.Date && x.tf1.ff2.FlightDate == deadHeadParameter.Date && x.f3.FlightDate == deadHeadParameter.Date
                                                     //&&
                                                     x.tf1.ff1.Dest != deadHeadParameter.ArrSta && x.tf1.ff2.Dest != deadHeadParameter.ArrSta
                                                     && (x.tf1.ff1.Carr + deadHeadParameter.ConnectTime <= x.tf1.ff2.Cdep || x.tf1.ff1.RouteNum == x.tf1.ff2.RouteNum)
                                                     && (x.tf1.ff2.Carr + deadHeadParameter.ConnectTime <= x.f3.Cdep || x.tf1.ff2.RouteNum == x.f3.RouteNum)
                                                     && x.f3.Cdep > x.tf1.ff2.Cdep && x.tf1.ff2.Cdep > x.tf1.ff1.Cdep
                                                     && x.tf1.ff1.Orig != x.f3.Orig
                                                    )
                                                    .Select(y =>
                                                     new RouteDomain
                                                     {
                                                         Date = y.tf1.ff1.FlightDate,
                                                         Route = y.tf1.ff1.Orig + '-' + y.tf1.ff1.Dest + '-' + y.tf1.ff2.Dest + '-' + y.f3.Dest,
                                                         RtDep = y.tf1.ff1.Cdep,
                                                         RtArr = y.f3.Carr,
                                                         RtTime = y.f3.Carr - y.tf1.ff1.Cdep,
                                                         Rt1 = y.tf1.ff1.RouteNum,
                                                         Rt2 = y.tf1.ff2.RouteNum,
                                                         Rt3 = y.f3.RouteNum,
                                                         Rt1Dep = y.tf1.ff1.Cdep,
                                                         Rt2Dep = y.tf1.ff2.Cdep,
                                                         Rt3Dep = y.f3.Cdep,
                                                         Rt1Arr = y.tf1.ff1.Carr,
                                                         Rt2Arr = y.tf1.ff2.Carr,
                                                         Rt3Arr = y.f3.Carr,
                                                         Con1 = y.tf1.ff2.Cdep - y.tf1.ff1.Carr,
                                                         Con2 = y.f3.Cdep - y.tf1.ff2.Carr,
                                                         Rt1Orig = y.tf1.ff1.Orig,
                                                         Rt2Orig = y.tf1.ff2.Orig,
                                                         Rt3Orig = y.f3.Orig,
                                                         Rt1Dest = y.tf1.ff1.Dest,
                                                         Rt2Dest = y.tf1.ff2.Dest,
                                                         Rt3Dest = y.f3.Dest,
                                                         Rt1FltNum = y.tf1.ff1.Flight,
                                                         Rt2FltNum = y.tf1.ff2.Flight,
                                                         Rt3FltNum = y.f3.Flight

                                                     }).ToList()
                                                     .OrderBy(z => z.Route).ThenBy(z1 => z1.RtTime).ToList();


            return twoConnectFlights;
        }


      
        /// <summary>
        /// PURPOSe : Calculate TFP
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        private TfpDomain CalculateTfp(RouteDomain y)
        {
            // frank todo -- add orig and dest to all rtes so tfp by distance can be queried from TtpCityPairs
            TfpDomain tfpDomain = new TfpDomain();
            //tfpDomain.Flight1Tfp = CalcTfp(y.Rt1Dep, y.Rt1Arr);
            //tfpDomain.Flight2Tfp = CalcTfp(y.Rt2Dep, y.Rt2Arr);
            //tfpDomain.Flight3Tfp = CalcTfp(y.Rt3Dep, y.Rt3Arr);
            tfpDomain.Flight1Tfp = Math.Max(TfpByDist(y.Rt1Orig, y.Rt1Dest), CalcTfp(y.Rt1Dep, y.Rt1Arr));
            tfpDomain.Flight2Tfp = Math.Max(TfpByDist(y.Rt2Orig, y.Rt2Dest), CalcTfp(y.Rt2Dep, y.Rt2Arr));
            tfpDomain.Flight3Tfp = Math.Max(TfpByDist(y.Rt3Orig, y.Rt3Dest), CalcTfp(y.Rt3Dep, y.Rt3Arr));
            tfpDomain.TotTfp = tfpDomain.Flight1Tfp + tfpDomain.Flight2Tfp + tfpDomain.Flight3Tfp;
            return tfpDomain;
        }

        private decimal CalcTfp(Int64 cdep, Int64 carr)
        {
            decimal tripTime;
            decimal retVal;
            tripTime = carr - cdep;
            retVal = tripTime == 0 ? 0.0m : (tripTime >= 55.0m) ? (1.0m + (tripTime - 55.0m) / 50.0m) : 1.0m;
            return retVal;

        }

        private decimal TfpByDist(string orig, string dest)
        {
            decimal distTfp = GlobalSettings.TtpCityPairs.Where(x => (x.City1 == orig && x.City2 == dest) || (x.City2 == orig && x.City1 == dest))
                                                         .OrderByDescending(x => x.Distance)
                                                         .Select(x => x.Distance)
                                                         .FirstOrDefault();
            // handle null City1 and null City2
            return Math.Max(0.0m, distTfp);
        }



        #endregion

    }

    // The following classes were moved to WBid.WBidClient.Models

    //public class RouteDomain
    //{
    //    public DateTime Date { get; set; }

    //    public string Route { get; set; }

    //    public Int64 RtDep { get; set; }

    //    public Int64 RtArr { get; set; }

    //    public Int64 RtTime { get; set; }

    //    public Int64 Rt1 { get; set; }
    //    public Int64 Rt2 { get; set; }
    //    public Int64 Rt3 { get; set; }

    //    public Int64 Rt1Dep { get; set; }
    //    public Int64 Rt2Dep { get; set; }
    //    public Int64 Rt3Dep { get; set; }

    //    public Int64 Rt1Arr { get; set; }
    //    public Int64 Rt2Arr { get; set; }
    //    public Int64 Rt3Arr { get; set; }

    //    // added by frank to get tfp by dist
    //    public string Rt1Orig { get; set; }
    //    public string Rt2Orig { get; set; }
    //    public string Rt3Orig { get; set; }

    //    // added by frank to get tfp by dist
    //    public string Rt1Dest { get; set; }
    //    public string Rt2Dest { get; set; }
    //    public string Rt3Dest { get; set; }

    //    // added by frank for display purposes
    //    public int Rt1FltNum { get; set; }
    //    public int Rt2FltNum { get; set; }
    //    public int Rt3FltNum { get; set; }

    //    public Int64 Con1 { get; set; }
    //    public Int64 Con2 { get; set; }

    //}


    //public class TfpDomain
    //{
    //    public decimal Flight1Tfp { get; set; }

    //    public decimal Flight2Tfp { get; set; }

    //    public decimal Flight3Tfp { get; set; }

    //    public decimal TotTfp { get; set; }

    //}
}
