#region NameSpace
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Model; 
#endregion

namespace WBid.WBidiPad.SharedLibrary.Parser
{
    public class BidLineParser
    {
        public Dictionary<string, Trip> tripData { get; set; }
        private int _show1stDay;
        private int _showAfter1stDay;

        /// <summary>
        /// PURPOSE :
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="domicile"></param>
        public Dictionary<string, Trip> ParseBidlineFile(string filePath, string domicile, string domcileCode, int show1stDay, int showAfter1stDay,string position)
        {


            List<string> lstLines = new List<string>();
            tripData = new Dictionary<string, Trip>();

            _show1stDay = show1stDay;
            _showAfter1stDay = showAfter1stDay;
            string line = string.Empty;
            int isReadStartCount = 0;
			if (File.Exists (filePath))
			{
				try {

					using (StreamReader reader = new StreamReader (filePath)) {
						while ((line = reader.ReadLine ()) != null) {
							try {
								if (line != "\f" && ((isReadStartCount == 0 && line.Substring (0, 4) == "Line") || (isReadStartCount > 0 && line.Substring (0, 4) != "----"))) {
									isReadStartCount++;
									lstLines.Add (line);

								} else if (isReadStartCount > 0) {
									isReadStartCount = 0;
									if (position == "FA")
									{
										ParseSingleBidlineForFA(lstLines, domicile, domcileCode);
									}
									else
									{
										ParseSingleBidline(lstLines, domicile, domcileCode);
									}
									lstLines.Clear ();

								}
							} catch (Exception) {

								throw;
							}

						}


					}
				} catch (Exception) {

					throw;
				}

			}


            return tripData;
       }
		//Parsing single line details
		private void ParseSingleBidlineForFA(List<string> lstLines, string domicile, string domcileCode)
		{
			//Get Total dutyperiods Count
			// int totalDutyPeriod = int.Parse(lstLines[3].Substring(18, 4).TrimStart(' '));

			try
			{
				Trip trip = null;

				int dpcount = 0;
				int colPosition = 24;
				DutyPeriod dutyPeriod = null;
				Flight flight = null;

				List<TripTime> tripTimeList = GetTripTimeDetailsForFA(lstLines);

				//if dpcount ==totalDutyPeriod,then we can stop the parsing
				while (colPosition < lstLines[2].Length)
				{
					//Trip Line" Checking each 3 character from the 24th position, if the trip name !=string.empty
					if (lstLines[2].Substring(colPosition, 3).Replace(':', ' ').Trim() != string.Empty)
					{
						trip = new Trip();
						string lastArrvStation = domicile;
						string arrvStation = string.Empty;

						trip.TripNum = domcileCode + lstLines[2].Substring(colPosition, 3).Replace(':', ' ').Trim() + int.Parse(lstLines[0].Substring(colPosition, 3).Replace(':', ' ').Trim()).ToString().PadLeft(2, '0');


						while (true)
						{
							//Loop 

							arrvStation = lstLines[3].Substring(colPosition, 3).Replace("*", " ").Replace(":", " ").Trim();
							if (trip.DutyPeriods.Count != 0)
							{

								if (arrvStation != string.Empty)
								{
									if ((lstLines[2].Substring(colPosition, 3).Replace(":", " ").Trim() != string.Empty))
									{

										break;
									}
								}
							}

							if (arrvStation != string.Empty)
							{




								dutyPeriod = new DutyPeriod();
								dutyPeriod.TripNum = trip.TripNum;
								dutyPeriod.ArrStaLastLeg = arrvStation;
								flight = new Flight();
								flight.ArrSta = arrvStation;
								flight.DepSta = lastArrvStation;
								flight.DepTime = (trip.DutyPeriods.Count) * 1440;
								flight.ArrTime = (trip.DutyPeriods.Count) * 1440;
								dutyPeriod.Flights.Add(flight);

								if (trip.DutyPeriods.Count == 1)
								{
									dutyPeriod.ShowTime = dutyPeriod.Flights[0].DepTime - _show1stDay;
									;
								}
								else
								{
									dutyPeriod.ShowTime = dutyPeriod.Flights[0].DepTime - _showAfter1stDay;
								}

								lastArrvStation = arrvStation;


								//Calculating FDP
								int flightCount = dutyPeriod.Flights.Count;
								for (int flightIndex = flightCount - 1; flightIndex >= 0; flightIndex--)
								{
									if (dutyPeriod.Flights[flightIndex].DeadHead)
										continue;
									else
									{
										dutyPeriod.FDP = dutyPeriod.Flights[flightIndex].ArrTime - dutyPeriod.ShowTime;
										break;
									}

								}

								dutyPeriod.DutPerSeqNum = trip.DutyPeriods.Count() + 1;
								trip.DutyPeriods.Add(dutyPeriod);
								dpcount++;
								// if (totalDutyPeriod == dpcount)
								//{
								//break;
								//}
								colPosition += 4;
							}
							else
							{
								break;
							}


						}

						TripTime tripTime = tripTimeList.FirstOrDefault(x => x.TripName == trip.TripNum.Substring(1, 3));
						if (tripTime != null)
						{
							trip.DutyPeriods[0].DepTimeFirstLeg = tripTime.DepTime;
							trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg = ((trip.DutyPeriods.Count - 1) * 1440) + tripTime.ArrTime;
							trip.DutyPeriods[0].Flights[0].DepTime = tripTime.DepTime;
							//trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[0].ArrTime = ((trip.DutyPeriods.Count - 1)*1440)+ tripTime.ArrTime;
							trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1].ArrTime = ((trip.DutyPeriods.Count - 1) * 1440) + tripTime.ArrTime;
							trip.Tfp = tripTime.Tfp;
							// trip.DepTime = trip.DutyPeriods[0].Flights[0].DepTime.ToString();
							trip.DepTime = ConvertMinuteToHHMM(trip.DutyPeriods[0].Flights[0].DepTime).Replace(":", "");
							Flight lastFlight = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1];
							trip.RetTime = ConvertMinuteToHHMM(lastFlight.ArrTime - (1440 * (trip.DutyPeriods.Count - 1))).Replace(":", "");

						}

						trip.FDP = trip.DutyPeriods.Sum(x => x.FDP);
						trip.PairLength = trip.DutyPeriods.Count;
						trip.IsFromBidFile = true;
						if (!tripData.ContainsKey(trip.TripNum))
						{
							tripData.Add(trip.TripNum, trip);
						}
					}
					else
					{
						colPosition += 4;
					}

				}

			}
			catch (Exception ex)
			{

			}
		}
		private List<TripTime> GetTripTimeDetailsForFA(List<string> lstLines)
		{
			string[] arrayTrip;
			List<TripTime> tripTimeList = new List<TripTime>();
			//Some times TFP,take off and land time details resides in multiple lines.
			for (int count = 4; count < lstLines.Count; count++)
			{
				arrayTrip = lstLines[count].Substring(23, lstLines[count].Length - 23).Replace(" ", "").Split(',');
				if (arrayTrip.Length > 0)
				{
					foreach (string tripDetails in arrayTrip)
					{
						if (tripDetails == string.Empty) continue;

						tripTimeList.Add(new TripTime()
						{
							TripName = tripDetails.Substring(0, 3),
							DepTime = ConvertHHMMToMinutes(tripDetails.Substring(4, 4)),
							ArrTime = ConvertHHMMToMinutes(tripDetails.Substring(9, 4)),
							Tfp = decimal.Parse(tripDetails.Split('(')[1].Replace(")", ""))

						});

					}


				}

			}
			return tripTimeList;
		}
        //Parsing  single line details
        private void ParseSingleBidline(List<string> lstLines, string domicile, string domcileCode)
        {
            //Get Total dutyperiods Count
            int totalDutyPeriod = int.Parse(lstLines[3].Substring(18, 4).TrimStart(' '));
            Trip trip = null;

            int dpcount = 0;
            int colPosition = 24;
            DutyPeriod dutyPeriod = null;
            Flight flight = null;

            List<TripTime> tripTimeList = GetTripTimeDetails(lstLines);



            //if  dpcount ==totalDutyPeriod,then  we can stop the parsing
            while (totalDutyPeriod > dpcount && colPosition < lstLines[2].Length)
            {
                //Trip Line"  Checking each 3 character from the 24th position, if the trip name !=string.empty
                if (lstLines[2].Substring(colPosition, 3).Replace(':', ' ').Trim() != string.Empty)
                {
                    trip = new Trip();
                    string lastArrvStation = domicile;
                    string arrvStation = string.Empty;

                    trip.TripNum = domcileCode + lstLines[2].Substring(colPosition, 3).Replace(':', ' ').Trim() + int.Parse(lstLines[0].Substring(colPosition, 3).Replace(':', ' ').Trim()).ToString().PadLeft(2, '0');


                    while (true)
                    {
                        //Loop 

                        arrvStation = lstLines[3].Substring(colPosition, 3).Replace("*", " ").Replace(":", " ").Trim();
                        if (trip.DutyPeriods.Count != 0)
                        {

                            if (arrvStation != string.Empty)
                            {
                                if ((lstLines[2].Substring(colPosition, 3).Replace(":", " ").Trim() != string.Empty))
                                {

                                    break;
                                }
                            }
                        }

                        if (arrvStation != string.Empty)
                        {




                            dutyPeriod = new DutyPeriod();
                            dutyPeriod.TripNum = trip.TripNum;
                            dutyPeriod.ArrStaLastLeg = arrvStation;
                            flight = new Flight();
                            flight.ArrSta = arrvStation;
                            flight.DepSta = lastArrvStation;
                            flight.DepTime = (trip.DutyPeriods.Count) * 1440;
                            flight.ArrTime = (trip.DutyPeriods.Count) * 1440;
                            dutyPeriod.Flights.Add(flight);

                            if (trip.DutyPeriods.Count == 1)
                            {
                                dutyPeriod.ShowTime = dutyPeriod.Flights[0].DepTime - _show1stDay;
                                ;
                            }
                            else
                            {
                                dutyPeriod.ShowTime = dutyPeriod.Flights[0].DepTime - _showAfter1stDay;
                            }

                            lastArrvStation = arrvStation;


                            //Calculating FDP
                            int flightCount = dutyPeriod.Flights.Count;
                            for (int flightIndex = flightCount - 1; flightIndex >= 0; flightIndex--)
                            {
                                if (dutyPeriod.Flights[flightIndex].DeadHead)
                                    continue;
                                else
                                {
                                    dutyPeriod.FDP = dutyPeriod.Flights[flightIndex].ArrTime - dutyPeriod.ShowTime;
                                    break;
                                }

                            }

                            dutyPeriod.DutPerSeqNum = trip.DutyPeriods.Count() + 1;
                            trip.DutyPeriods.Add(dutyPeriod);
                            dpcount++;
                            if (totalDutyPeriod == dpcount)
                            {
                                break;
                            }
                            colPosition += 3;
                        }
                        else
                        {
                            break;
                        }


                    }

                    TripTime tripTime = tripTimeList.FirstOrDefault(x => x.TripName == trip.TripNum.Substring(1, 3));
                    if (tripTime != null)
                    {
                        trip.DutyPeriods[0].DepTimeFirstLeg = tripTime.DepTime;
                        trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg = ((trip.DutyPeriods.Count - 1) * 1440) + tripTime.ArrTime;
                        trip.DutyPeriods[0].Flights[0].DepTime = tripTime.DepTime;
                        //trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[0].ArrTime = ((trip.DutyPeriods.Count - 1)*1440)+ tripTime.ArrTime;
                        trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1].ArrTime = ((trip.DutyPeriods.Count - 1) * 1440) + tripTime.ArrTime;
                        trip.Tfp = tripTime.Tfp;
                        // trip.DepTime = trip.DutyPeriods[0].Flights[0].DepTime.ToString();
                        trip.DepTime = ConvertMinuteToHHMM(trip.DutyPeriods[0].Flights[0].DepTime).Replace(":", "");
                        Flight lastFlight = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1];
                        trip.RetTime = ConvertMinuteToHHMM(lastFlight.ArrTime - (1440 * (trip.DutyPeriods.Count - 1))).Replace(":", "");

                    }

                    trip.FDP = trip.DutyPeriods.Sum(x => x.FDP);
                    trip.PairLength = trip.DutyPeriods.Count;
                    trip.IsFromBidFile = true;
                    tripData.Add(trip.TripNum, trip);
                }
                else
                {
                    colPosition += 3;
                }

            }

        }

        public string ConvertMinuteToHHMM(int minute)
        {
            string result = string.Empty;
            result = Convert.ToString(minute / 60).PadLeft(2, '0');
            result += ":";
            result += Convert.ToString(minute % 60).PadLeft(2, '0'); ;
            return result;

        }

        /// <summary>
        /// PURPOSE : Parse All trip take off ,land time and Tfp details
        /// </summary>
        /// <param name="lstLines"></param>
        /// <returns></returns>
        private List<TripTime> GetTripTimeDetails(List<string> lstLines)
        {
            string[] arrayTrip;
            List<TripTime> tripTimeList = new List<TripTime>();
            //Some times TFP,take off and land time details resides in multiple lines.
            for (int count = 4; count < lstLines.Count; count++)
            {
                arrayTrip = lstLines[count].Substring(24, lstLines[count].Length - 24).Replace(" ", "").Split(',');
                if (arrayTrip.Length > 0)
                {
                    foreach (string tripDetails in arrayTrip)
                    {
                        if (tripDetails == string.Empty) continue;

                        tripTimeList.Add(new TripTime()
                        {
                            TripName = tripDetails.Substring(0, 3),
                            DepTime = ConvertHHMMToMinutes(tripDetails.Substring(4, 4)),
                            ArrTime = ConvertHHMMToMinutes(tripDetails.Substring(9, 4)),
                            Tfp = decimal.Parse(tripDetails.Split('(')[1].Replace(")", ""))

                        });

                    }


                }

            }
            return tripTimeList;
        }

        /// <summary>
        /// PURPOSe : Convert HHMM to Minute
        /// </summary>
        /// <param name="hhmm"></param>
        /// <returns></returns>
        private static int ConvertHHMMToMinutes(string hhmm)
        {
            int result = 0;
            result = int.Parse(hhmm.Substring(0, 2)) * 60;
            result += int.Parse(hhmm.Substring(2, 2));
            return result;

        }
    }




    public class TripTime
    {
        public string TripName { get; set; }

        public int DepTime { get; set; }

        public int ArrTime { get; set; }

        public decimal Tfp { get; set; }
    }
}
