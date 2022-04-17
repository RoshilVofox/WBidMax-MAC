using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
//using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary.Parser
{
    public class TripParser
    {
        #region Constructor
        public TripParser()
        {
            Trips = new Dictionary<string, Trip>();
        }

        public TripParser(string domicile, string yrMonth)
        {
            Trips = new Dictionary<string, Trip>();
            YrMonth = yrMonth;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Store all the trip details
        /// </summary>
        public Dictionary<string, Trip> Trips { get; set; }
        public string YrMonth { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Parse the trip file and return it as ictionary<string, Trip>
        /// </summary>
        /// <param name="fielname"></param>
        /// <param name="filesToParses"></param>
        /// <returns></returns>
        public Dictionary<string, Trip> ParseTrips(string filename, Byte[] filesToParses, DateTime firstDateOfDST, DateTime lastDateOfDST)
        {
            Dictionary<string, List<string>> tripRecords = new Dictionary<string, List<string>>();
            tripRecords.Add(filename, getTripRecords(filesToParses));
            return Trips = ParseTripsFromTripRecords(tripRecords, firstDateOfDST, lastDateOfDST, filename);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Parse trip file and returns all the parsed trip record as Dictionary<string, Trip>
        /// </summary>
        /// <param name="tripRecords"></param>
        /// <returns></returns>
        private Dictionary<string, Trip> ParseTripsFromTripRecords(Dictionary<string, List<string>> tripRecords, DateTime firstDateOfDST, DateTime lastDateOfDST, string filename)
        {
            Dictionary<string, Trip> trips = new Dictionary<string, Trip>();

            foreach (var tripRecordSet in tripRecords)
            {
                if (tripRecordSet.Key.Substring(0, 1) != "A")  // used != so both CP and FO files would be parsed
                    ParsePlilotTripsData(trips, tripRecordSet);
                else
                    ParseFlightAttendTripsData(trips, tripRecordSet, firstDateOfDST, lastDateOfDST, filename);
            }
            return trips;
        }
        /// <summary>
        /// convert the byte data for parsed data to list of records( each individual line from Trips file is added to TripRecords)
        /// </summary>
        /// <param name="fileToParse"></param>
        /// <returns></returns>
        private List<string> getTripRecords(Byte[] fileToParse)
        {
            string record;
            List<string> domicileTripRecords = new List<string>();
            Stream stream = new MemoryStream(fileToParse);
            using (StreamReader sr = new StreamReader(stream))
            {
                //while ((record = sr.ReadLine()).Substring(0, 2) != "*E" || !sr.EndOfStream)
                while (!sr.EndOfStream)
                {
                    record = sr.ReadLine();
                    if (record.Substring(0, 2) != "*E")
                        domicileTripRecords.Add(record);  // each individual line from Trips file is added to TripRecords (rec1,2,3,5,6 etc or GTrip etc)
                }
            }
            return domicileTripRecords;
        }


        // **********************
        // * Pilot Trip Parsing *
        // **********************

        /// <summary>
        /// Parse the pilot trip datas
        /// </summary>
        /// <param name="trips"></param>
        /// <param name="tripRecordSet"></param>
        private void ParsePlilotTripsData(Dictionary<string, Trip> trips, KeyValuePair<string, List<string>> tripRecordSet)
        {
            string tripNum = null;
            int rec6count = 0, numRec6 = 0;
            Trip trip = null;

            List<string> recFives = new List<string>();
            List<string> recSixes = new List<string>();

            // enumerates through each record in a specific (domicile, seat) trip file

            foreach (var record in tripRecordSet.Value)
            {
                if (record.Substring(0, 2) != "*E" && !trips.ContainsKey(record.Substring(0, 4)))  // checks eof for Pilot records and skips over duplicate Trip pairings
                {
                    switch (record.Substring(4, 1))             // processes each record type (1,2,3,5,6 -- no 4) 5th spot is rec type
                    {
                        case "1":
                            recFives.Clear();
                            recSixes.Clear();
                            rec6count = 0;
                            tripNum = record.Substring(0, 4);
                            trip = Parse1Rec(trip = new Trip(), record, tripRecordSet.Key);
                            break;
                        case "2":
                            // if (record.Substring(0, 4) != tripNum)
                            //     MessageBox.Show("Bad Parse Case 2 -- pairNum");
                            trip = Parse2Rec(trip, record);
                            break;
                        case "3":
                            // if (record.Substring(0, 4) != tripNum)
                            /// MessageBox.Show("Bad Parse Case 3 -- pairNum");
                            trip = Parse3Rec(trip, record);
                            break;
                        case "4":
                            // MessageBox.Show("Looking for type 4 record -- there shouldn't be any!");
                            break;
                        case "5":
                            // if (record.Substring(0, 4) != tripNum)
                            //     MessageBox.Show("Bad Parse Case 5 -- pairNum");
                            recFives.Add(record);
                            numRec6 = Convert.ToInt32(record.Substring(79, 1));
                            break;
                        case "6":
                            // if (record.Substring(0, 4) != tripNum)
                            //    MessageBox.Show("Bad Parse Case 6 -- pairNum");
                            if (rec6count == 0) trip = Parse5Rec(trip, recFives);  // have not parsed any 6 records yet
                            recSixes.Add(record);
                            rec6count++;
                            if (rec6count == numRec6)
                            {
                                trip = Parse6Rec(trip, recSixes);
                                if (!trips.ContainsKey(trip.TripNum))
                                    trips.Add(trip.TripNum, trip);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// parse record type 1 for pilots records
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="record"></param>
        /// <param name="tripRecordSetKey"></param>
        /// <returns></returns>
        private Trip Parse1Rec(Trip trip, string record, string tripRecordSetKey)
        {
            trip.TripNum = record.Substring(0, 4);
            trip.FreqCode = record.Substring(10, 1);
            trip.OpDays = record.Substring(11, 6);
            trip.PairLength = Convert.ToInt32(record.Substring(18, 1));
            trip.DepSta = record.Substring(19, 3);
            trip.DepTime = record.Substring(22, 4);
            trip.RetSta = record.Substring(26, 3);
            trip.RetTime = record.Substring(29, 4);
            trip.AmPm = record.Substring(36, 1);
            trip.Block = Convert.ToInt32(record.Substring(37, 4));
            trip.TotDutPer = Convert.ToInt32(record.Substring(42, 1));
            trip.DepFltNum = record.Substring(43, 6);
            trip.RetFltNum = record.Substring(49, 6);
            trip.PairClassCode = record.Substring(61, 1);
            trip.StartOp = Convert.ToInt32(record.Substring(68, 2));
            trip.EndOp = Convert.ToInt32(record.Substring(70, 2));
            trip.NonOpDays = record.Substring(72, 8);
            return trip;
        }
        /// <summary>
        /// parse record type 2 for pilots records
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private Trip Parse2Rec(Trip trip, string record)
        {
            int dp = 1;
            decimal totTfp = 0;
            decimal tfp = 0;
            while (record.Substring(5 + (dp - 1) * 7, 2) != " -")
            {
                DutyPeriod dutyPeriod = new DutyPeriod();
                dutyPeriod.TripNum = trip.TripNum;
                dutyPeriod.DutPerSeqNum = dp;
                dutyPeriod.ArrStaLastLeg = record.Substring(5 + (dp - 1) * 7, 3);
                tfp = Convert.ToDecimal(record.Substring(8 + (dp - 1) * 7, 4)) / 100m;
                dutyPeriod.Tfp = tfp;
                totTfp = totTfp + tfp;
                trip.DutyPeriods.Add(dutyPeriod);
                dp++;
            }
            trip.TotDutPer = dp - 1;
            trip.Tfp = totTfp;
            return trip;
        }
        /// <summary>
        /// parse record type 3 for pilots records
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        private Trip Parse3Rec(Trip trip, string record)
        {
            trip.DepTime1stDutPer = Convert.ToInt32(record.Substring(5, 4));
            trip.DepTimeLastDutper = Convert.ToInt32(record.Substring(9, 4));
            for (int i = 1; i <= trip.TotDutPer; i++)
            {
                trip.DutyPeriods[i - 1].Block = Convert.ToInt32(record.Substring(13 + (i - 1) * 4, 4));
            }
            trip.BriefTime = Convert.ToInt16(record.Substring(69, 2)) * 60 + Convert.ToInt16(record.Substring(71, 2));
            trip.DebriefTime = Convert.ToInt16(record.Substring(73, 2)) * 60 + Convert.ToInt16(record.Substring(75, 2));
			//if (trip.TripNum.Substring(1, 1) == "W" || trip.TripNum.Substring(1, 1) == "Y")
			//{

			//	trip.BriefTime = 420;// Cpmpany changed their trip file format on nov 2017. The data in character postions 70-73 will be the actual RAP start for reserve and the data in the character positions 74-77 will be the actual RAP end time for reserve.
			//	trip.DebriefTime = 420;
			//}
			//else
			//{
			//	trip.BriefTime = Convert.ToInt16(record.Substring(69, 2)) * 60 + Convert.ToInt16(record.Substring(71, 2));
			//	trip.DebriefTime = Convert.ToInt16(record.Substring(73, 2)) * 60 + Convert.ToInt16(record.Substring(75, 2));
			//}
            return trip;
        }

        /// <summary>
        /// parse record type 5 for pilots records
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="recFives"></param>
        /// <returns></returns>
        private Trip Parse5Rec(Trip trip, List<string> recFives)
        {
            int dp = 0;
            int dpFlts = 0;
            int depTime = 0;
            int recCount = 0;
            if (trip.TripNum.Contains("BW2B"))
            {
            }
            for (int i = 0; i < recFives.Count; i++)
            {
                recCount = 0;
                while (recCount < 6 && (depTime = Convert.ToInt32(recFives[i].Substring(6 + (recCount) * 12, 5))) != 0)
                {
                    if (recFives[i].Substring(11 + (recCount) * 12, 1) == "9")  // 9 = duty break
                    {
                        trip.DutyPeriods[dp].TotFlights = dpFlts;
                        dp++;
                        dpFlts = 0;
                    }
                    Flight flight = new Flight();
                    flight.DepType = recFives[i].Substring(5 + (recCount) * 12, 1);
                    flight.DepTime = depTime;
                    flight.ArrTime = Convert.ToInt32(recFives[i].Substring(12 + (recCount) * 12, 5));
                    trip.DutyPeriods[dp].Flights.Add(flight);
                    if (dp != 0 && dpFlts == 0)
                    {
                        trip.DutyPeriods[dp - 1].DutyBreak = depTime -
                            trip.DutyPeriods[dp - 1].Flights[trip.DutyPeriods[dp - 1].TotFlights - 1].ArrTime - 60;
                    }
                    dpFlts++;
                    recCount++;
                }
                if (i + 1 == recFives.Count)
                {
                    trip.DutyPeriods[dp].TotFlights = dpFlts;
                }
            }
            return trip;
        }

        /// <summary>
        /// parse record type 6 for pilots records
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="recSixes"></param>
        /// <returns></returns>
        private Trip Parse6Rec(Trip trip, List<string> recSixes)
        {
            int dpFlts = 0;
            int recCount = 0;
            int totDutPer = trip.TotDutPer;
            int dutPerFlts = 0;
            string oneSix = ConCat6s(recSixes);
            bool reserveTrip = false;
            if (trip.TripNum.Substring(1, 1) == "W" || trip.TripNum.Substring(1, 1) == "Y")
            {
                trip = ParseReserve(trip, recSixes);

            }
            else
            {
                for (int dp = 0; dp < totDutPer; dp++)
                {
                    dutPerFlts = trip.DutyPeriods[dp].TotFlights;
                    dpFlts = 0;
                    while (dpFlts < dutPerFlts)
                    {


                        if (oneSix.Substring(5 + recCount * 15, 2) == "DH" || oneSix.Substring(5 + recCount * 15, 2) == "DM")
                            trip.DutyPeriods[dp].Flights[dpFlts].DeadHead = true;
                        else
                            trip.DutyPeriods[dp].Flights[dpFlts].DeadHead = false;
                        if (!reserveTrip)
                        {
                            trip.DutyPeriods[dp].Flights[dpFlts].FltNum = Convert.ToInt32(oneSix.Substring(7 + recCount * 15, 4));
                        }
                        else
                        {

                        }
                        trip.DutyPeriods[dp].Flights[dpFlts].DepSta = oneSix.Substring(11 + recCount * 15, 3);
                        trip.DutyPeriods[dp].Flights[dpFlts].ArrSta = oneSix.Substring(14 + recCount * 15, 3);
                        trip.DutyPeriods[dp].Flights[dpFlts].Equip = oneSix.Substring(17 + recCount * 15, 3);
                        //if (!ckSkipTfpDistCalc.Checked)
                        //    trip.DutyPeriods[dp].TfpByDistance = calcDistTfp(trip.DutyPeriods[dp].Flights[dpFlts]);
                        dpFlts++;
                        recCount++;
                    }
                }
            }
            return trip;
        }



        private Trip ParseReserve(Trip trip, List<string> recSixes)
        {
            trip.ReserveTrip = true;
            return trip;    // not done!
        }

        private string ConCat6s(List<string> recSixes)
        {
            //******************************************************
            //** Creates one big 6 record -- easier to parse :-)  **
            //******************************************************

            string oneSix = null;
            oneSix = recSixes[0].Substring(0, 77);
            for (int i = 1; i < recSixes.Count; i++)
            {
                oneSix = oneSix + recSixes[i].Substring(5, 72);
            }
            return oneSix;
        }



        // ******************************
        // * Flt Attendant Trip Parsing *
        // ******************************
        /// <summary>
        /// Parse the Flight Attended Trips data
        /// </summary>
        /// <param name="trips"></param>
        /// <param name="tripRecordSet"></param>
        private void ParseFlightAttendTripsData(Dictionary<string, Trip> trips, KeyValuePair<string, List<string>> tripRecordSet, DateTime firstDateOfDST, DateTime lastDateOfDST, string filename)
        {
            Trip trip = null;
            int dutyPeriodNum = 0;
            int flightSequence = 0;
            int reportTime = 0;
            int releaseTime = 0;
            foreach (var record in tripRecordSet.Value)
            {
                switch (record.Substring(0, 2))
                {
                    case "GT":
                        // fix this when you have internet connections
                        // trip=parseGTrecord(trip=new Trip(), record, tripRecordSet.Key);
                        break;
                    case "T1":
                        dutyPeriodNum = 1;
                        flightSequence = 0;
                        if (trip != null)
                            if (!trips.ContainsKey(trip.TripNum))
                            {
                                trip.PairLength = trip.DutyPeriods.Count;
                            trip.BriefTime = (trip.DutyPeriods[0].Flights[0].DepTime) - (reportTime);
                                trip.DebriefTime = releaseTime - (trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1].ArrTime);
       
                                trips.Add(trip.TripNum, trip);
                            }
                        trip = parseT1record(trip = new Trip(), record);
                        reportTime = GetTripReportTime(record);
                        releaseTime = GetTripReleaseTime(record);
                        break;
                    case "T2":
                        trip = parseT2record(trip, record);
                        break;
                    case "F ":
                        trip = parseFrecord(trip, record);
                        break;
                    case "Z ":
                        trip = parseZrecord(trip, record);
                        break;
                    case "L ":
                        flightSequence += 1;
                        trip = parseLrecord(trip, record, dutyPeriodNum, flightSequence, firstDateOfDST, lastDateOfDST, filename);
                        break;
                    case "D ":
                        dutyPeriodNum += 1;
                        flightSequence = 0;
                        break;
                    default:
                        break;
                }
            }
            if (!trips.ContainsKey(trip.TripNum))
            {
                trip.PairLength = trip.DutyPeriods.Count;
                trip.BriefTime = (trip.DutyPeriods[0].Flights[0].DepTime) - (reportTime);
                trip.DebriefTime = releaseTime - (trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1].ArrTime);

                trips.Add(trip.TripNum, trip);
            }
        }
        private int GetTripReportTime(string record)
        {
            return Convert.ToInt32(record.Substring(23, 4)) - 1440;
        }
        private int GetTripReleaseTime(string record)
        {
            return Convert.ToInt32(record.Substring(33, 4)) - 1440;
        }
        private Trip parseGTrecord(Trip trip, string record, string tripRecordSetKey)
        {
            trip.GtripStartDate = record.Substring(Defines.GTHDR_STARTDATE - 1, 7);
            trip.GtripEndDate = record.Substring(Defines.GTHDR_ENDDATE - 1, 7);
            trip.GtripCreateDate = record.Substring(Defines.GTHDR_CREATEDATE - 1, 7);

            return trip;
        }

        private Trip parseT1record(Trip trip, string record)
        {
            trip.TripNum = record.Substring(Defines.GTT1_PAIRING - 1, 4);
            trip.GtripDomicile = record.Substring(Defines.GTT1_DOMICILE - 1, 3);

            return trip;
        }

        private Trip parseT2record(Trip trip, string record)
        {
            trip.GtripPositions = record;
            return trip;
        }

        private Trip parseFrecord(Trip trip, string record)
        {
            trip.GtripOpVector = record.Substring(2, 60);
            return trip;
        }

        private Trip parseZrecord(Trip trip, string record)
        {
            //------commented
            // if (record.Substring(2, 60) != trip.GtripOpVector)
            //   MessageBox.Show("Oops - F record and Z record are not equal while parsing FA trips");

            return trip;
        }

        private Trip parseLrecord(Trip trip, string record, int dutyPeriodNum, int flightNum, DateTime firstDateOfDST, DateTime lastDateOfDST, string filename)
        {
            //if (WBid.WBidClient
            DutyPeriod dutyPeriod = null;
            Flight flight = new Flight();
            if (record.Substring(Defines.GTL_BEGINDUTY - 1, 1) == "1")
                dutyPeriod = new DutyPeriod();
            else
                dutyPeriod = trip.DutyPeriods[trip.DutyPeriods.Count() - 1];
            flight.FltNum = Convert.ToInt16(record.Substring(Defines.GTL_FLTNUM - 1, 5));
            flight.Equip = record.Substring(Defines.GTL_EQUIP - 1, 4);
            flight.DepSta = record.Substring(Defines.GTL_DEPCITY - 1, 3);
            //flight.DepTimeL = record.Substring(Defines.GTL_DEPTIME_L - 1, 5);
            flight.DepTime = Convert.ToInt16(record.Substring(Defines.GTL_DEPTIME_Z - 1, 5)) - 1440;
            //flight.DepTimeB = record.Substring(Defines.GTL_DEPTIME_B - 1, 5);
            flight.ArrSta = record.Substring(Defines.GTL_ARRCITY - 1, 3);
            //flight.ArrTimeL = record.Substring(Defines.GTL_ARRTIME_L - 1, 5);
            flight.ArrTime = Convert.ToInt16(record.Substring(Defines.GTL_ARRTIME_Z - 1, 5)) - 1440;
            //flight.ArrTimeB = record.Substring(Defines.GTL_ARRTIME_B - 1, 5);
            /*
            flight.BeginDuty = record.Substring(Defines.GTL_BEGINDUTY - 1, 1);
            flight.BriefTime = record.Substring(Defines.GTL_BRIEFTIME - 1, 3);
            flight.EndDuty = record.Substring(Defines.GTL_ENDDUTY - 1, 1);
            flight.Debrief = record.Substring(Defines.GTL_DEBRIEF - 1, 4);
             */
            flight.DeadHead = record.Substring(Defines.GTL_DEADHEAD - 1, 1) == "1" ? true : false;

            flight.AcftChange = record.Substring(Defines.GTL_ACCHANGE - 1, 1) == "1" ? true : false;

            if (record.Length > 73)
                flight.ETOPS = record.Substring(Defines.GTL_ETOPS - 1, 1) == "H" ? true : false;
            //flight.ConusFlag = record.Substring(Defines.GTL_CONUSFLAG - 1, 1);

            if (trip.DutyPeriods.Count() != dutyPeriodNum)
                trip.DutyPeriods.Add(dutyPeriod);
            trip.DutyPeriods[dutyPeriodNum - 1].Flights.Add(flight);
            return trip;
        }

        private int GetFADepArrTimesInHerbTime(string time, DateTime firstDateOfDST, DateTime lastDateOfDST, string filename, Trip trip)
        {
            // i also need the date of this flight
            // need to set dates in duty period for FA

            return 0;
        }




        #endregion

    }
}
