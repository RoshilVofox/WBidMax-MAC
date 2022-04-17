#region NameSpace
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
#endregion

namespace WBid.WBidiPad.SharedLibrary.Parser
{
    public class FASecondRoundParser
    {

        #region Private Variable
        private DateTime _previousDate = DateTime.MinValue;
        private DateTime _dutyPeriodDate = DateTime.MinValue;
        private int _resrveCount = 0;
        private int _lineNumber = 0;
        private string _domicileName = string.Empty;
        private decimal _fAReserveDayPay = 0.0m;
        private enum LineType { VacationRelief = 1, Reserve }
        private LineType lineType { get; set; }

        #endregion

        #region Public Method

        /// <summary>
        /// PURPOSE : Parse FA SecondRound
        /// </summary>
        /// <param name="psFileName"></param>
        /// <param name="trips"></param>
        public Dictionary<string, Line> ParseFASecondRound(string psFileName, ref Dictionary<string, Trip> trips, decimal fAReserveDayPay, string domicileName)
        {
            Dictionary<string, Line> lines = new Dictionary<string, Line>();
            Dictionary<string, Trip> reserveTrips = new Dictionary<string, Trip>();
            string lineString = string.Empty;
            lineType = LineType.VacationRelief;

            _domicileName = domicileName;
            _fAReserveDayPay = fAReserveDayPay;
            Line line = null;
            List<FAVacationReliefTrip> lstFAVacationReliefTrip = null;
            List<FAReserveTrip> lstFAReserveTrip = new List<FAReserveTrip>(); ;
            List<string> pairingList = null;
            FAReserveTrip currentFAReserveTrip = null;
            _resrveCount = 0;

            try
            {

                if (File.Exists(psFileName))
                {
                    using (StreamReader reader = new StreamReader(psFileName))
                    {
                        if ((lineString = reader.ReadLine()) != null)
                        {
                            //read first line for getting bid period
                        }
                        while ((lineString = reader.ReadLine()) != null)
                        {
                            if (lineString.Substring(0, 1) == "C")
                            {
                                //Checking the line number is changed...ie the line is a new line
                                if (int.Parse(lineString.Substring(Defines.GLC_LINENUM - 1, 3)) != _lineNumber && _lineNumber != 0)
                                {
                                    //Add trip details to Line object
                                    if (lineType == LineType.VacationRelief)
                                    {
                                        AddVacationReliefDetailsToLineObject(line, lstFAVacationReliefTrip);
                                        lines.Add(line.LineNum.ToString(), line);
                                    }
                                    else if (lineType == LineType.Reserve)
                                    {
                                        AddReserveTripToList(lstFAReserveTrip, currentFAReserveTrip, pairingList);
                                        AddReserveTripDetailsToLineObject(line, pairingList);
                                        line.ReserveLine = true;
                                        lines.Add(line.LineNum.ToString(), line);
                                    }

                                }
                                line = new Line();
                                ReadLineNumber(lineString, line);
                                lstFAVacationReliefTrip = new List<FAVacationReliefTrip>();
                                pairingList = new List<string>();
                                _previousDate = DateTime.MinValue;

                            }
                            else if (lineString.Substring(0, 1) == "T")
                            {
                                ReadVacationReliefTripDetails(lineString, lstFAVacationReliefTrip);
                            }

                            else if (lineString.Substring(0, 1) == "A")
                            {
                                lineType = LineType.Reserve;
                                currentFAReserveTrip = ReadReserveTripDetails(lineString, lstFAReserveTrip, currentFAReserveTrip, pairingList);
                            }

                        }

                        //Add last trip details
                        if (lineType == LineType.Reserve && pairingList.Count > 0)
                        {
                            AddReserveTripToList(lstFAReserveTrip, currentFAReserveTrip, pairingList);
                            AddReserveTripDetailsToLineObject(line, pairingList);
                            line.ReserveLine = true;
                            lines.Add(line.LineNum.ToString(), line);
                        }
						if (lineType == LineType.VacationRelief)
						{
							AddVacationReliefDetailsToLineObject(line, lstFAVacationReliefTrip);
							lines.Add(line.LineNum.ToString(), line);
						}

                        AddReserveTripDetailsToTripDictionary(reserveTrips, lstFAReserveTrip);


                        foreach (var trip in reserveTrips)
                        {
                            if (!trips.Keys.Contains(trip.Key))
                                trips.Add(trip.Key, trip.Value);
                        }

                        //trips = trips.Concat(reserveTrips).ToDictionary(pair => pair.Key, pair => pair.Value);

                        lines = lines.OrderBy(x => x.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return lines;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// PURPOSE : Read line number from the line started with "C"
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="line"></param>
        private void ReadLineNumber(string lineString, Line line)
        {
            _lineNumber = int.Parse(lineString.Substring(Defines.GLC_LINENUM - 1, 3));
            line.LineNum = _lineNumber;
            line.LineDisplay = line.LineNum.ToString();
            line.Tfp = Convert.ToDecimal(lineString.Substring(Defines.GLC_CREDIT - 1, 5)) / 100m;
            line.Block = Convert.ToInt32(lineString.Substring(Defines.GLC_BLK - 1, 5));

        }

        #region Vacation Releif
        /// <summary>
        /// PURPOSE :Read Vacation Relief Trip Details
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="lstFAVacationReliefTrip"></param>
        private void ReadVacationReliefTripDetails(string lineString, List<FAVacationReliefTrip> lstFAVacationReliefTrip)
        {

            string pairNum = string.Empty;
            pairNum = lineString.Substring(Defines.GLT_PAIRING_1 - 1, 4);
            if (!string.IsNullOrEmpty(pairNum.Trim()))
            {
                lstFAVacationReliefTrip.Add(new FAVacationReliefTrip()
                {
                    TripName = pairNum,
                    TripStartDate = lineString.Substring(Defines.GLT_STARTDATE_1 - 1, 2),
                    Position = lineString.Substring(Defines.GLT_POSITION_1 - 1, 1)
                });
            }

            pairNum = lineString.Substring(Defines.GLT_PAIRING_2 - 1, 4);
            if (!string.IsNullOrEmpty(pairNum.Trim()))
            {
                lstFAVacationReliefTrip.Add(new FAVacationReliefTrip()
                {
                    TripName = pairNum,
                    TripStartDate = lineString.Substring(Defines.GLT_STARTDATE_2 - 1, 2),
                    Position = lineString.Substring(Defines.GLT_POSITION_2 - 1, 1)
                });
            }


            pairNum = lineString.Substring(Defines.GLT_PAIRING_3 - 1, 4);
            if (!string.IsNullOrEmpty(pairNum.Trim()))
            {
                lstFAVacationReliefTrip.Add(new FAVacationReliefTrip()
                {
                    TripName = pairNum,
                    TripStartDate = lineString.Substring(Defines.GLT_STARTDATE_3 - 1, 2),
                    Position = lineString.Substring(Defines.GLT_POSITION_3 - 1, 1)
                });
            }

        }

        /// <summary>
        /// PURPOSE :Add Vacation Relief Details To LineObject
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lstFAVacationReliefTrip"></param>
        private void AddVacationReliefDetailsToLineObject(Line line, List<FAVacationReliefTrip> lstFAVacationReliefTrip)
        {

            if (line.Pairings == null)
                line.Pairings = new List<string>();
            string pairingName = string.Empty;


            foreach (FAVacationReliefTrip fAVacationReliefTrip in lstFAVacationReliefTrip)
            {
                pairingName = fAVacationReliefTrip.TripName + fAVacationReliefTrip.TripStartDate;
                pairingName = pairingName.Replace(" ", "0");
                if (!line.Pairings.Contains(pairingName))
                {
                    line.Pairings.Add(pairingName);
                    line.FASecondRoundPositions.Add(pairingName, fAVacationReliefTrip.Position);
                }
            }
        }

        #endregion

        #region Reserve trip
        private bool CheckDutyPeriodsStartTimeEndTime(List<DutyPeriod> listDutyPeriods, FAReserveTrip fAReserveTrip)
        {
            bool status = true;

            for (int count = 0; count < listDutyPeriods.Count; count++)
            {
                status = ((listDutyPeriods[count].ShowTime == fAReserveTrip.DutyPeriods[count].ShowTime) && (listDutyPeriods[count].ReleaseTime == fAReserveTrip.DutyPeriods[count].ReleaseTime));
                if (!status) break;

            }

            //fAReserveTrip.DutyPeriods.ForEach(x =>
            //    {   if (!status)  return;
            //        status = (x.ShowTime == listDutyPeriods[fAReserveTrip.DutyPeriods.IndexOf(x)].ShowTime);

            //    }
            //    );
            return status;
        }

        /// <summary>
        /// PURPOSE:Read Reserve Trip Details
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="lstFAReserveTrip"></param>
        private FAReserveTrip ReadReserveTripDetails(string lineString, List<FAReserveTrip> lstFAReserveTrip, FAReserveTrip currentFAReserveTrip, List<string> pairingList)
        {
            _dutyPeriodDate = DateTime.ParseExact(lineString.Substring(17, 7), "ddMMMyy", CultureInfo.InvariantCulture);

            if (_dutyPeriodDate.Subtract(_previousDate).Days != 1)
            {
                if (_previousDate != DateTime.MinValue)
                {
                    //Check the trip exist in the lstFAReserveTrip list
                    AddReserveTripToList(lstFAReserveTrip, currentFAReserveTrip, pairingList);


                }
                //Create new reserve trip object
                currentFAReserveTrip = new FAReserveTrip();
                currentFAReserveTrip.DutyPeriods = new List<DutyPeriod>();
                currentFAReserveTrip.DutyPeriods = new List<DutyPeriod>();
                currentFAReserveTrip.TripStartDates = new List<DateTime>();
                currentFAReserveTrip.TripStartDates.Add(_dutyPeriodDate);
                currentFAReserveTrip.ReserveType = lineString.Substring(13, 1);
            }

            DutyPeriod dutyPeriod = new DutyPeriod();

            DateTime releaseDate = DateTime.ParseExact(lineString.Substring(28, 7), "ddMMMyy", CultureInfo.InvariantCulture);
            dutyPeriod.ShowTime = _dutyPeriodDate.Subtract(currentFAReserveTrip.TripStartDates[0]).Days * 1440 + Convert.ToInt16(lineString.Substring(24, 2)) * 60 + Convert.ToInt16(lineString.Substring(26, 2));
            dutyPeriod.ReleaseTime = releaseDate.Subtract(currentFAReserveTrip.TripStartDates[0]).Days * 1440 + Convert.ToInt16(lineString.Substring(35, 2)) * 60 + Convert.ToInt16(lineString.Substring(37, 2));


            dutyPeriod.Tfp = _fAReserveDayPay;
            dutyPeriod.Block = 0;

            currentFAReserveTrip.DutyPeriods.Add(dutyPeriod);
            dutyPeriod.Flights = new List<Flight>();

            dutyPeriod.Flights.Add(new Flight()
            {
                DepSta = _domicileName,
                ArrSta = _domicileName,
                DepTime = dutyPeriod.ShowTime,
                ArrTime = dutyPeriod.ReleaseTime,
                Tfp = _fAReserveDayPay,
                Block = 0,
                FlightSeqNum = 1



            });
            _previousDate = _dutyPeriodDate;

            return currentFAReserveTrip;
        }

        /// <summary>
        /// PURPOSE :add reserve trip to list
        /// </summary>
        /// <param name="lstFAReserveTrip"></param>
        /// <param name="currentFAReserveTrip"></param>
        private void AddReserveTripToList(List<FAReserveTrip> lstFAReserveTrip, FAReserveTrip currentFAReserveTrip, List<string> pairingList)
        {
            FAReserveTrip reserveTrip = lstFAReserveTrip.FirstOrDefault(x => (x.DutyPeriods.Count == currentFAReserveTrip.DutyPeriods.Count) &&
                                                CheckDutyPeriodsStartTimeEndTime(x.DutyPeriods, currentFAReserveTrip));
            if (reserveTrip == null)
            {
                string day = string.Empty;
                if (currentFAReserveTrip.ReserveType == "P")
                    day = currentFAReserveTrip.TripStartDates[0].Day.ToString().PadLeft(2, '0');

                currentFAReserveTrip.TripName = "X" + currentFAReserveTrip.ReserveType + _resrveCount.ToString("d2") + day;
                _resrveCount++;
                lstFAReserveTrip.Add(currentFAReserveTrip);
                reserveTrip = currentFAReserveTrip;


            }
            else
            {

                //if (!reserveTrip.TripStartDates.Contains(currentFAReserveTrip.TripStartDates[0]))
                //    reserveTrip.TripStartDates.Add(currentFAReserveTrip.TripStartDates[0]);


                if (!reserveTrip.TripStartDates.Contains(currentFAReserveTrip.TripStartDates[0]))
                {
                    reserveTrip.TripStartDates.Add(currentFAReserveTrip.TripStartDates[0]);
                    // lstFAReserveTrip.Add(

                    if (reserveTrip.TripName != null && reserveTrip.TripName.Substring(1, 1) == "P")
                    {
                        FAReserveTrip faReserve = new FAReserveTrip();
                        faReserve.TripName = reserveTrip.TripName.Substring(0, 4) + currentFAReserveTrip.TripStartDates[0].Day.ToString().PadLeft(2, '0');
                        faReserve.TripStartDates = new List<DateTime>();
                        faReserve.TripStartDates.Add(currentFAReserveTrip.TripStartDates[0]);
                        faReserve.ReserveType = reserveTrip.ReserveType;
                        faReserve.DutyPeriods = reserveTrip.DutyPeriods;
                        if (!lstFAReserveTrip.Any(x => x.TripName == faReserve.TripName))
                        {
                            lstFAReserveTrip.Add(faReserve);
                            // _resrveCount++;
                        }
                    }
                }

            }
            if (!pairingList.Contains(reserveTrip.TripName.Substring(0, 4) + currentFAReserveTrip.TripStartDates[0].Day.ToString().PadLeft(2, '0')))
                pairingList.Add(reserveTrip.TripName.Substring(0, 4) + currentFAReserveTrip.TripStartDates[0].Day.ToString().PadLeft(2, '0'));
            // pairingList.Add(reserveTrip.TripName + currentFAReserveTrip.TripStartDates[0].Day.ToString("d2"));
        }

        private void AddReserveTripDetailsToLineObject(Line line, List<string> pairingList)
        {
            if (line.Pairings == null)
                line.Pairings = new List<string>();



            foreach (string pairingName in pairingList)
            {
                if (!line.Pairings.Contains(pairingName))
                {
                    line.Pairings.Add(pairingName);

                    //line.FASecondRoundPositions.Add(pairingName, fAReserveTrip.Position);
                }
            }

            if (line.Pairings.Any(x => x.Substring(1, 1) == "R"))
            {
                line.LineDisplay = line.LineDisplay + "RR";
            }
            else
            {
                line.LineDisplay = line.LineDisplay + "R";

            }
            line.BlkHrsInLine = string.Empty;
            line.BlkHrsInBp = string.Empty;
            line.TfpPerFltHr = 0;
            line.TafbInBp = string.Empty;
            line.TafbInLine = string.Empty;


        }

        #endregion


        private void AddReserveTripDetailsToTripDictionary(Dictionary<string, Trip> reserveTrips, List<FAReserveTrip> lstFAReserveTrip)
        {
            Trip trip = null;
            foreach (FAReserveTrip faReserveTrip in lstFAReserveTrip)
            {
                trip = new Trip();
                trip.TripNum = faReserveTrip.TripName;
                trip.FAresTripOpDay = faReserveTrip.TripStartDates.OrderBy(x => x).ToList();
                trip.DutyPeriods = faReserveTrip.DutyPeriods;
                trip.Block = 0;
                trip.Tafb = 0;
                trip.ReserveTrip = true;
                reserveTrips.Add(trip.TripNum, trip);
            }



        }

        #endregion

    }

    #region Utility Class
    public class FAVacationReliefTrip
    {
        public string TripName { get; set; }

        public string TripStartDate { get; set; }

        public string Position { get; set; }

    }

    public class FAReserveTrip
    {
        public string TripName { get; set; }

        public List<DateTime> TripStartDates { get; set; }

        public string ReserveType { get; set; }

        public List<DutyPeriod> DutyPeriods { get; set; }


    }
    #endregion
}
