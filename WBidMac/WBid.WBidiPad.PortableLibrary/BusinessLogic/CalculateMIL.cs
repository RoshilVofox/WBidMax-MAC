#region NameSpace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VacationCorrection;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;

#endregion
namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
	public class CalculateMIL
	{
		#region Private Variables
		private List<Trip> _trips = null;
		private List<Line> _lines = null;
		private Dictionary<string, TripMultiMILData> _allTripMultiMILData;

		#endregion



		public Dictionary<string, TripMultiMILData> CalculateMILValues(MILParams milParams)
		{
			try
			{
				_lines = milParams.Lines;
				_trips = GlobalSettings.Trip.ToList();
				_allTripMultiMILData = new Dictionary<string, TripMultiMILData>();
				//  _mILStartDate = milParams.MilStartDate;
				// _mILEndDate = milParams.MilEndDate;



				GenerateMILData();



			}
			catch (Exception)
			{

				throw;
			}

			return _allTripMultiMILData;
		}


		#region MIL Generation

		private void GenerateMILData()
		{


			int counter = 0;
			foreach (var line in _lines)
			{


				counter++;

				if (!line.BlankLine)
				{


					ProcessLines(line);
				}
			}

		}

		private string ConvertSecondsToMinutes(double remainingTime)
		{
			TimeSpan t = TimeSpan.FromSeconds(remainingTime);

			return string.Format("Time to finished: {0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
		}


		/// <summary>
		/// Process a line for MIL calculation
		/// </summary>
		/// <param name="line"></param>
		private void ProcessLines(Line line)
		{
			int paringCount = 0;
			foreach (var pairing in line.Pairings)
			{
				try
				{
					bool isLastTrip = ((line.Pairings.Count - 1) == paringCount);
					paringCount++;
					DateTime tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
					////string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //string splitName = string.Empty;
                    //if (GlobalSettings.CurrentBidDetails.Round == "M")
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //}
                    //else
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                    //}
                    Trip trip = _trips.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4));
                    if (trip == null)
                        trip = _trips.FirstOrDefault(x => x.TripNum == pairing);
					if (trip == null) return;
					DateTime tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);



					//trip end time
					int time = (trip.ReserveTrip) ? trip.DutyPeriods[trip.PairLength - 1].ReserveIn : trip.DutyPeriods[trip.PairLength - 1].ReleaseTime;
					if (time > 1440)
					{
						time = time - (trip.PairLength - 1) * 1440;
					}
					tripEndDate = tripEndDate.AddMinutes(time);

					//trip start time
					time = (trip.ReserveTrip) ? trip.DutyPeriods[0].ReserveOut : trip.DutyPeriods[0].ShowTime;
					tripStartDate = tripStartDate.AddMinutes(time);



					//TimeSpan time = new TimeSpan(23, 0, 0);
					//tripEndDate = tripEndDate.Date + time;

					//int tripEndTime = (trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg) % 1440;
					//tripEndTime = WBidHelper.AfterMidnightLandingTime(tripEndTime);
					//tripEndTime = WBidHelper.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripEndDate, tripEndTime);


					//if ( (_milStartDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440))
					//{
					//    trip.DutyPeriods[trip.DutyPeriods.Count - 1].ArrivesAfterMidnightDayBeforeVac = true;
					//}


					var tripMultiMilData = new TripMultiMILData();


					TripDetails tripDetails = new TripDetails
					{
						Trip = trip,
						TripDate = tripStartDate,
						TripEndDate = tripEndDate,
						IsReserveLine = line.ReserveLine

					};
				
					//  1.Check if the trip is completely inside the MIL (MA)
					//-------------------------------------------------------------------------------------------------

					if (GlobalSettings.MILDates.Any(x => (x.StartAbsenceDate <= tripStartDate && x.EndAbsenceDate >= tripEndDate)))
					{
						tripDetails.TripType = "MA";
						tripMultiMilData.MaData = CreateMilTrip_MA(tripDetails); ;
					}

					// 2.Check if trip starts before the MIL period and finishes inside the MIL period. (MOF Vacation)
					//-------------------------------------------------------------------------------------------------
					else if (GlobalSettings.MILDates.Any(x => ((x.StartAbsenceDate > tripStartDate) && (x.StartAbsenceDate < tripEndDate) && x.EndAbsenceDate >= tripEndDate)))
					{
						tripDetails.TripType = "MOF";
						tripMultiMilData.MofData = CreateMilTrip_MOF_MOB(tripDetails);
					}

					// 3.check if trip starts inside the MIL period and finished outside the MIL period.. (MOB Vacation)
					//-------------------------------------------------------------------------------------------------
					else if (GlobalSettings.MILDates.Any(x => (x.EndAbsenceDate > tripStartDate && x.EndAbsenceDate < tripEndDate && tripStartDate >= x.StartAbsenceDate)))
					{
						tripDetails.TripType = "MOB";
						tripMultiMilData.MobData = CreateMilTrip_MOF_MOB(tripDetails);
					}

					else if (GlobalSettings.MILDates.Any(x => (x.StartAbsenceDate > tripStartDate && x.EndAbsenceDate < tripEndDate)))
					{
						tripDetails.TripType = "MOFB";
						tripMultiMilData.MofbData = CreateMilTrip_MOFB(tripDetails);
					}

					if (tripMultiMilData.MofData != null || tripMultiMilData.MaData != null || tripMultiMilData.MobData != null || tripMultiMilData.MofbData != null)
					{
						_allTripMultiMILData.Add(pairing, tripMultiMilData);
					}

				}
				catch (Exception ex)
				{

					throw;
				}

			}
		}



		/// <summary>
		/// Make the trip "MA".means all the dutyprtiods in this trip will be dropped becuse this trip is inside the MIL period
		/// </summary>
		/// <param name="tripDetails"></param>
		/// <returns></returns>
		private MILTrip CreateMilTrip_MA(TripDetails tripDetails)
		{
			MILTrip milTrip = new MILTrip();
			milTrip.DutyPeriodsDetails = new List<MILDutyPeriod>();
			milTrip.TripName = tripDetails.Trip.TripNum;
			milTrip.MILType = tripDetails.TripType;
			milTrip.IsDropped = true;
			milTrip.Tfp = tripDetails.Trip.Tfp;
			//set all the Dutyperiods in the trip to MA and drop all the dutyperiods
			foreach (var dutyperids in tripDetails.Trip.DutyPeriods)
			{
				MILDutyPeriod mILDutyPeriod = new MILDutyPeriod();
				mILDutyPeriod.IsDropped = true;
				mILDutyPeriod.Type = "MA";
				mILDutyPeriod.DpSeqNum = dutyperids.DutPerSeqNum;
				mILDutyPeriod.FlightDetails = new List<MILFlights>();
				mILDutyPeriod.IsinBidPeriod = CheckIsInBidPeriod(tripDetails.TripDate, dutyperids.DutPerSeqNum);
				mILDutyPeriod.Tfp = dutyperids.Tfp;
				//set all the flights in the MA duty period to MA and drop all the flights
				foreach (var flight in dutyperids.Flights)
				{
					MILFlights milflight = new MILFlights();
					milflight.IsDropped = true;
					milflight.Type = "MA";
					milflight.FlightNumber = flight.FltNum;
					milflight.FltSeqNum = flight.FlightSeqNum;
					milflight.Tfp = flight.Tfp;
					mILDutyPeriod.FlightDetails.Add(milflight);
				}
				milTrip.DutyPeriodsDetails.Add(mILDutyPeriod);
			}

			return milTrip;

		}



		private MILTrip CreateMilTrip_MOFB(TripDetails tripDetails)
		{
			MILTrip milTrip = new MILTrip();
			milTrip.DutyPeriodsDetails = new List<MILDutyPeriod>();
			milTrip.TripName = tripDetails.Trip.TripNum;
			milTrip.MILType = tripDetails.TripType;
			Absense milVacationDates = null;
			milVacationDates = GlobalSettings.MILDates.FirstOrDefault(x => x.StartAbsenceDate > tripDetails.TripDate && x.EndAbsenceDate < tripDetails.TripEndDate);
			if (milVacationDates == null)
				return milTrip;

			tripDetails.MILStartDate = milVacationDates.StartAbsenceDate;
			tripDetails.MILEndDate = milVacationDates.EndAbsenceDate;

			Reverse reverse;
			DeadHead directionOfDh;


			//Create MIL duty periods and flights
			//---------------------------------------------------
			MILDutyPeriod milDutyPeriod = null;
			MILFlights milFlights = null;
			foreach (DutyPeriod dp in tripDetails.Trip.DutyPeriods)
			{
				milDutyPeriod = new MILDutyPeriod();
				milDutyPeriod.FlightDetails = new List<MILFlights>();
				milDutyPeriod.DpSeqNum = dp.DutPerSeqNum;
				foreach (Flight flt in dp.Flights)
				{
					milFlights = new MILFlights();
					milFlights.FltSeqNum = flt.FlightSeqNum;
					milDutyPeriod.FlightDetails.Add(milFlights);

				}

				milTrip.DutyPeriodsDetails.Add(milDutyPeriod);
			}
			//---------------------------------------------------



			List<DutyPeriod> dutPds = null;
			List<MILDutyPeriod> milDutPds = null;
			bool isMOF;


			if (tripDetails.IsReserveLine)
			{
				dutPds = tripDetails.Trip.DutyPeriods;
				DateTime dpDate = tripDetails.TripDate;
				DateTime dutyperiodStart;
				DateTime dutyperiodEnd;
				foreach (var dp in dutPds)
				{
					dutyperiodStart = dpDate.Date.AddMinutes(dp.ReserveOut - ((dp.DutPerSeqNum - 1) * 1440));
					dutyperiodEnd = dpDate.Date.AddMinutes(dp.ReserveIn - ((dp.DutPerSeqNum - 1) * 1440));
					milDutyPeriod = milTrip.DutyPeriodsDetails.FirstOrDefault(x => x.DpSeqNum == dp.DutPerSeqNum);
					if (IsDutyPeriodOverlapInMILPeriod(dutyperiodStart, dutyperiodEnd))
					{
						CreateFltsAndMakeDp(milDutyPeriod, dp, "MA", 0, tripDetails);
					}
					else
					{
						CreateFltsAndMakeDp(milDutyPeriod, dp, "MO", 0, tripDetails);
					}
					dpDate = dpDate.AddDays(1);
				}

			}
			else
			{

				string tripType = string.Empty;
				//MOF part
				//---------------------------------------------------------------------

				tripDetails.Trip.DutyPeriods.Reverse();
				dutPds = tripDetails.Trip.DutyPeriods;
				directionOfDh = DeadHead.toDomicile;
				reverse = Reverse.Yes;
				isMOF = true;

				tripType = tripDetails.TripType;
				tripDetails.TripType = "MOF";
				CreateMOFBDetails(tripDetails, milTrip, reverse, directionOfDh, dutPds, isMOF);
				tripDetails.TripType = tripType;
				// tripDetails.Trip.DutyPeriods.Reverse<DutyPeriod>().ToList();
				tripDetails.Trip.DutyPeriods.Reverse();
				dutPds = tripDetails.Trip.DutyPeriods;
				//---------------------------------------------------------------------



				//MOB part
				//---------------------------------------------------------------------


				directionOfDh = DeadHead.toRejoinTrip;
				reverse = Reverse.No;
				isMOF = false;

				tripType = tripDetails.TripType;
				tripDetails.TripType = "MOB";
				CreateMOFBDetails(tripDetails, milTrip, reverse, directionOfDh, dutPds, isMOF);
				tripDetails.TripType = tripType;

				//---------------------------------------------------------------------

				DateTime dpDate = tripDetails.TripDate;
				DateTime dutyperiodStart;
				DateTime dutyperiodEnd;
				foreach (var dp in dutPds)
				{
					//dutyperiodStart = dpDate;// commented by Roshil on 25-11-2021 due to solve an issue comes when second dutyperiod doesnot taken for comparision on the next lines. only first dutyperiod time was correct.
					dutyperiodStart = dpDate.Date.AddMinutes(dp.ShowTime - ((dp.DutPerSeqNum - 1) * 1440));

					dutyperiodEnd = dpDate.Date.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440));

					milDutyPeriod = milTrip.DutyPeriodsDetails.FirstOrDefault(x => x.DpSeqNum == dp.DutPerSeqNum);
					if (tripDetails.MILStartDate < dutyperiodStart && tripDetails.MILEndDate > dutyperiodEnd)
					{
						CreateFltsAndMakeDp(milDutyPeriod, dp, "MA", 0, tripDetails);
					}

					dpDate = dpDate.Date.AddDays(1);


				}
			}



			if (milTrip.DutyPeriodsDetails.Count(x => x.Type == "MD") == milTrip.DutyPeriodsDetails.Count)
			{
				milTrip.Tfp = tripDetails.Trip.Tfp;
				milTrip.IsDropped = true;
			}
			else
			{
				milTrip.Tfp = milTrip.DutyPeriodsDetails.Sum(y => y.Tfp);
			}


			return milTrip;


		}

		private void CreateMOFBDetails(TripDetails tripDetails, MILTrip milTrip, Reverse reverse, DeadHead directionOfDh, List<DutyPeriod> dutPds, bool isMOF)
		{
			bool isSuccess = false;
			int splitLeg = 0;
			DateTime dpDate = reverse == Reverse.No ? tripDetails.TripDate : tripDetails.TripDate.AddDays(tripDetails.Trip.DutyPeriods.Count() - 1);
			DateTime dutyperiodStart;
			DateTime dutyperiodEnd;
			foreach (var dp in dutPds)
			{
				dutyperiodStart = dpDate;
				dutyperiodEnd = dpDate.Date.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440));

				//for MOF part
				if (isMOF && dutyperiodStart > tripDetails.MILStartDate)
				{
					dpDate = (reverse == Reverse.Yes) ? dpDate.AddDays(-1) : dpDate.AddDays(1);
					continue;
				}

				//for MOB part
				else if (!isMOF && dutyperiodEnd < tripDetails.MILEndDate)
				{
					dpDate = (reverse == Reverse.Yes) ? dpDate.AddDays(-1) : dpDate.AddDays(1);
					continue;
				}
				else
				{

					MILDutyPeriod milDutyPeriod = milTrip.DutyPeriodsDetails.FirstOrDefault(x => x.DpSeqNum == dp.DutPerSeqNum);
					if (isSuccess)
					{

						CreateFltsAndMakeDp(milDutyPeriod, dp, "MO", splitLeg, tripDetails);
					}

					//check the dutyperiod is having any sip.if the dutyperiod has a sip ,remove the flight upto that flight.(iterate backward for the MOF trip)
					else if (CheckSip(dp, ref splitLeg, GlobalSettings.CurrentBidDetails.Domicile, reverse, tripDetails, dpDate))
					{
						CreateFltsAndMakeDp(milDutyPeriod, dp, "Split", splitLeg, tripDetails);
						isSuccess = true;
					}
					//if the dutyperiod is not having any sip,we need to check whether the dutyperiod contains valid Split point city
					else if (CheckValidSPC(dp, ref splitLeg, GlobalSettings.CurrentBidDetails.Domicile, reverse, directionOfDh, tripDetails, dpDate))
					{
						CreateFltsAndMakeDp(milDutyPeriod, dp, "Split", splitLeg, tripDetails);
						isSuccess = true;
					}
					//if the dutyperiod is not having sip and Split point city,remove the entire dutyperiod
					else
					{
						CreateFltsAndMakeDp(milDutyPeriod, dp, "MD", splitLeg, tripDetails);
					}

					milDutyPeriod.Tfp = milDutyPeriod.FlightDetails.Where(x => x.Type == "MD" || x.Type == "MA").Sum(y => y.Tfp);
					dpDate = (reverse == Reverse.Yes) ? dpDate.Date.AddDays(-1) : dpDate.Date.AddDays(1);
				}




			}

		}

		private MILTrip CreateMilTrip_MOF_MOB(TripDetails tripDetails)
		{
			MILTrip milTrip = new MILTrip();
			milTrip.TripName = tripDetails.Trip.TripNum;
			milTrip.MILType = tripDetails.TripType;




			Reverse reverse;
			DeadHead directionOfDh;


			// for MOF trip ,we need to iterate the flight from the flight/dutyperiod prior to the start of MIL start date (backwards iteration )
			Absense milVacationDates = null;
			if (tripDetails.TripType == "MOF")
			{


				milVacationDates = GlobalSettings.MILDates.FirstOrDefault(x => x.StartAbsenceDate >= tripDetails.TripDate && x.StartAbsenceDate <= tripDetails.TripEndDate);
				directionOfDh = DeadHead.toDomicile;
				reverse = Reverse.Yes;
			}
			else
			{
				milVacationDates = GlobalSettings.MILDates.FirstOrDefault(x => x.EndAbsenceDate >= tripDetails.TripDate && x.EndAbsenceDate <= tripDetails.TripEndDate);
				directionOfDh = DeadHead.toRejoinTrip;
				reverse = Reverse.No;
			}

			if (milVacationDates == null)
				return milTrip;
			tripDetails.MILStartDate = milVacationDates.StartAbsenceDate;
			tripDetails.MILEndDate = milVacationDates.EndAbsenceDate;


			List<DutyPeriod> dutPds = null;
			if (reverse == Reverse.Yes)
				dutPds = tripDetails.Trip.DutyPeriods.Reverse<DutyPeriod>().ToList();
			else
				dutPds = tripDetails.Trip.DutyPeriods;

			DateTime dpDate = reverse == Reverse.No ? tripDetails.TripDate : tripDetails.TripDate.AddDays(tripDetails.Trip.DutyPeriods.Count() - 1);
			DateTime dutyperiodStart;
			DateTime dutyperiodEnd;
			int splitLeg = 0;

			milTrip.DutyPeriodsDetails = new List<MILDutyPeriod>();
			//iterate through the duty period. for the MOF trip, we need to ierate the dutperiod prior to the start of the MIL in the backword
			//for the MOB trip,we need to iterate through the dutyperiod after the MIL end 
			bool isSuccess = false;


			if (tripDetails.IsReserveLine)
			{
				foreach (var dp in dutPds)
				{
					dutyperiodStart = dpDate.Date.AddMinutes(dp.ReserveOut - ((dp.DutPerSeqNum - 1) * 1440));
					dutyperiodEnd = dpDate.Date.AddMinutes(dp.ReserveIn - ((dp.DutPerSeqNum - 1) * 1440));
					if (IsDutyPeriodOverlapInMILPeriod(dutyperiodStart, dutyperiodEnd))
					{
						milTrip.DutyPeriodsDetails.Add(CreateFltsAndMakeDp(dp, "MA", splitLeg, tripDetails));
					}
					else
					{
						milTrip.DutyPeriodsDetails.Add(CreateFltsAndMakeDp(dp, "MO", splitLeg, tripDetails));
					}
					dpDate = (reverse == Reverse.Yes) ? dpDate.AddDays(-1) : dpDate.AddDays(1);
				}
			}
			else
			{

				foreach (var dp in dutPds)
				{
					dutyperiodStart = dpDate;
					dutyperiodEnd = dpDate.Date.AddMinutes(dp.LandTimeLastLeg - ((dp.DutPerSeqNum - 1) * 1440));
					if (isSuccess)
					{
						milTrip.DutyPeriodsDetails.Add(CreateFltsAndMakeDp(dp, "MO", splitLeg, tripDetails));
					}

					//check the dutperiod is in MIL period.if the dutyperid is in the MIL ,we need to drop the entire dutyperiod.
					else if (IsDutyPeriodInMILPeriod(dutyperiodStart, dutyperiodEnd))
					{
						milTrip.DutyPeriodsDetails.Add(CreateFltsAndMakeDp(dp, "MA", splitLeg, tripDetails));

					}
					//check the dutyperiod is having any sip.if the dutyperiod has a sip ,remove the flight upto that flight.(iterate backward for the MOF trip)
					else if (CheckSip(dp, ref splitLeg, GlobalSettings.CurrentBidDetails.Domicile, reverse, tripDetails, dpDate))
					{
						milTrip.DutyPeriodsDetails.Add(CreateFltsAndMakeDp(dp, "Split", splitLeg, tripDetails));
						isSuccess = true;
					}
					//if the dutyperiod is not having any sip,we need to check whether the dutyperiod contains valid Split point city
					else if (CheckValidSPC(dp, ref splitLeg, GlobalSettings.CurrentBidDetails.Domicile, reverse, directionOfDh, tripDetails, dpDate))
					{
						milTrip.DutyPeriodsDetails.Add(CreateFltsAndMakeDp(dp, "Split", splitLeg, tripDetails));
						isSuccess = true;
					}
					//if the dutyperiod is not having sip and Split point city,remove the entire dutyperiod
					else
					{
						milTrip.DutyPeriodsDetails.Add(CreateFltsAndMakeDp(dp, "MD", splitLeg, tripDetails));
					}


					dpDate = (reverse == Reverse.Yes) ? dpDate.AddDays(-1) : dpDate.AddDays(1);

				}
			}

			if (reverse == Reverse.Yes)
				milTrip.DutyPeriodsDetails.Reverse();

			if (milTrip.DutyPeriodsDetails.Count(x => x.Type == "MD") == milTrip.DutyPeriodsDetails.Count)
			{
				milTrip.Tfp = tripDetails.Trip.Tfp;
				milTrip.IsDropped = true;
			}
			else
			{
				milTrip.Tfp = milTrip.DutyPeriodsDetails.Sum(y => y.Tfp);
			}

			return milTrip;

		}
		//private bool isNeedtoRemovetheFlightsAtSplitPoint(DutyPeriod dp, ref int sipLeg, string domicile, Reverse reverse, DeadHead directionOfDh, TripDetails tripDetails, DateTime dpDate)
		//{
		//    int splitLeg = 0;
		//   bool isSplitPointExist=CheckValidSPC(dp, ref splitLeg, GlobalSettings.CurrentBidDetails.Domicile, reverse,directionOfDh, tripDetails, dpDate);
		//   if (isSplitPointExist)
		//   {
		//       if (tripDetails.TripType == "MOF")
		//       {
		//           //The company would never split the trip with 1 flight being flown.The entire pairing would be dropped.
		//           if (splitLeg == 1 && dp.DutPerSeqNum == 1)
		//               return false;
		//           else
		//               return true;
		//       }
		//       else
		//       {
		//           //However, the company will never DH a pilot to fly just one flight back to domicile.  In this case, the last flight would be pulled also.
		//           if (dp.Flights.Count == splitLeg - 2 && tripDetails.Trip.PairLength == dp.DutPerSeqNum)
		//               return false;
		//           else
		//               return true;
		//       }
		//   }
		//   else return false;

		//}
		/// <summary>
		/// /Check whether the dutyperiod in the MIL period.
		/// </summary>
		/// <param name="dpDate"></param>
		/// <returns></returns>
		private bool IsDutyPeriodInMILPeriod(DateTime startDate, DateTime endTime)
		{

			// return GlobalSettings.MILDates.Any(x => x.StartAbsenceDate <= dpDate && x.EndAbsenceDate >= dpDate);
			return GlobalSettings.MILDates.Any(x => x.StartAbsenceDate <= startDate && x.EndAbsenceDate >= endTime);
		}


		private bool IsDutyPeriodOverlapInMILPeriod(DateTime startDate, DateTime endTime)
		{

			// return GlobalSettings.MILDates.Any(x => x.StartAbsenceDate <= dpDate && x.EndAbsenceDate >= dpDate);
			return GlobalSettings.MILDates.Any(x => (x.StartAbsenceDate > startDate && x.StartAbsenceDate < endTime && x.EndAbsenceDate > startDate && x.EndAbsenceDate > endTime) ||

				(x.StartAbsenceDate < startDate && x.StartAbsenceDate < endTime && x.EndAbsenceDate > startDate && x.EndAbsenceDate < endTime)

				|| (x.StartAbsenceDate <= startDate && x.EndAbsenceDate >= endTime));


		}


		/// <summary>
		/// Drop all the flights in a dutyperiod.(In the case of "MA dutyperiod and MD dutyperiod)
		/// </summary>
		/// <param name="dp"></param>
		/// <param name="dpType"></param>
		/// <param name="splitLeg"></param>
		/// <param name="tripDetails"></param>
		/// <returns></returns>
		private MILDutyPeriod CreateFltsAndMakeDp(DutyPeriod dp, string dpType, int splitLeg, TripDetails tripDetails)
		{
			MILDutyPeriod mILDutyPeriod = new MILDutyPeriod();

			mILDutyPeriod.Type = dpType;
			mILDutyPeriod.DpSeqNum = dp.DutPerSeqNum;
			mILDutyPeriod.FlightDetails = new List<MILFlights>();
			mILDutyPeriod.IsinBidPeriod = CheckIsInBidPeriod(tripDetails.TripDate, dp.DutPerSeqNum);
			string firstType = string.Empty;
			string secondType = string.Empty;



			// if the duty period type is "MA" or "MD" 
			if (dpType == "MA" || dpType == "MD")
			{
				mILDutyPeriod.IsDropped = true;
				mILDutyPeriod.Tfp = dp.Tfp;
				//set all the flights in the MA duty period to MA , MD duty period to MD  and drop all the flights
				foreach (var flight in dp.Flights)
				{
					MILFlights milflight = new MILFlights();
					milflight.IsDropped = true;
					milflight.Type = dpType;
					milflight.FlightNumber = flight.FltNum;
					milflight.FltSeqNum = flight.FlightSeqNum;
					milflight.Tfp = flight.Tfp;
					mILDutyPeriod.FlightDetails.Add(milflight);
				}
			}
			else if (dpType == "MO")
			{
				mILDutyPeriod.IsDropped = false;
				// mILDutyPeriod.Tfp = dp.Tfp;
				foreach (var flight in dp.Flights)
				{
					MILFlights milflight = new MILFlights();
					milflight.IsDropped = false;
					milflight.Type = dpType;
					milflight.FlightNumber = flight.FltNum;
					milflight.FltSeqNum = flight.FlightSeqNum;
					milflight.Tfp = flight.Tfp;
					mILDutyPeriod.FlightDetails.Add(milflight);
				}
			}
			else
			{
				// if the duty period type is "Split" and  trip type is MOF
				if (tripDetails.TripType == "MOF")
				{
					string flightType = "MO";
					bool isDroped = false;
					foreach (var flight in dp.Flights)
					{
						MILFlights milflight = new MILFlights();
						milflight.FlightNumber = flight.FltNum;
						milflight.FltSeqNum = flight.FlightSeqNum;
						milflight.Tfp = flight.Tfp;
						if (milflight.FltSeqNum == splitLeg)
						{
							flightType = "MD";
							isDroped = true;
						}

						milflight.IsDropped = isDroped;
						milflight.Type = flightType;
						mILDutyPeriod.FlightDetails.Add(milflight);
					}
					mILDutyPeriod.Tfp = mILDutyPeriod.FlightDetails.Where(x => x.Type == "MD" || x.Type == "MA").Sum(y => y.Tfp);
				}
				// if the duty period type is "Split" and  trip type is MOB
				else if (tripDetails.TripType == "MOB")
				{
					string flightType = "MD";
					bool isDroped = true;
					foreach (var flight in dp.Flights)
					{
						MILFlights milflight = new MILFlights();
						milflight.FlightNumber = flight.FltNum;
						milflight.FltSeqNum = flight.FlightSeqNum;
						milflight.Tfp = flight.Tfp;

						if (milflight.FltSeqNum == splitLeg)
						{
							flightType = "MO";
							isDroped = false;
						}

						milflight.IsDropped = isDroped;
						milflight.Type = flightType;

						mILDutyPeriod.FlightDetails.Add(milflight);


					}

					mILDutyPeriod.Tfp = mILDutyPeriod.FlightDetails.Where(x => x.Type == "MD" || x.Type == "MA").Sum(y => y.Tfp);





				}

			}
			if (mILDutyPeriod.FlightDetails.Count(x => x.Type == "MD") == mILDutyPeriod.FlightDetails.Count())
			{
				mILDutyPeriod.Type = "MD";

			}
			else if (mILDutyPeriod.FlightDetails.Count(x => x.Type == "MO") == mILDutyPeriod.FlightDetails.Count())
			{
				mILDutyPeriod.Type = "MO";

			}
			return mILDutyPeriod;
		}


		private void CreateFltsAndMakeDp(MILDutyPeriod mILDutyPeriod, DutyPeriod dp, string dpType, int splitLeg, TripDetails tripDetails)
		{
			//MILDutyPeriod mILDutyPeriod = new MILDutyPeriod();

			mILDutyPeriod.Type = dpType;
			// mILDutyPeriod.DpSeqNum = dp.DutPerSeqNum;
			// mILDutyPeriod.FlightDetails = new List<MILFlights>();
			mILDutyPeriod.IsinBidPeriod = CheckIsInBidPeriod(tripDetails.TripDate, dp.DutPerSeqNum);
			string firstType = string.Empty;
			string secondType = string.Empty;



			// if the duty period type is "MA" or "MD" 
			if (dpType == "MA" || dpType == "MD")
			{
				mILDutyPeriod.IsDropped = true;
				mILDutyPeriod.Tfp = dp.Tfp;
				//set all the flights in the MA duty period to MA , MD duty period to MD  and drop all the flights
				foreach (var flight in dp.Flights)
				{

					MILFlights milflight = mILDutyPeriod.FlightDetails.FirstOrDefault(x => x.FltSeqNum == flight.FlightSeqNum);
					if (milflight != null)
					{
						milflight.IsDropped = true;
						milflight.Type = dpType;
						milflight.FlightNumber = flight.FltNum;
						milflight.FltSeqNum = flight.FlightSeqNum;
						milflight.Tfp = flight.Tfp;
					}
					else
					{ 
					}
					// mILDutyPeriod.FlightDetails.Add(milflight);
				}
			}
			else if (dpType == "MO")
			{
				mILDutyPeriod.IsDropped = false;
				// mILDutyPeriod.Tfp = dp.Tfp;
				foreach (var flight in dp.Flights)
				{
					MILFlights milflight = mILDutyPeriod.FlightDetails.FirstOrDefault(x => x.FltSeqNum == flight.FlightSeqNum);
					milflight.IsDropped = false;
					milflight.Type = dpType;
					milflight.FlightNumber = flight.FltNum;
					milflight.FltSeqNum = flight.FlightSeqNum;
					milflight.Tfp = flight.Tfp;
					//  mILDutyPeriod.FlightDetails.Add(milflight);
				}
			}
			else
			{
				// if the duty period type is "Split" and  trip type is MOF
				if (tripDetails.TripType == "MOF")
				{
					string flightType = "MO";
					bool isDroped = false;
					foreach (var flight in dp.Flights)
					{
						MILFlights milflight = mILDutyPeriod.FlightDetails.FirstOrDefault(x => x.FltSeqNum == flight.FlightSeqNum);
						milflight.FlightNumber = flight.FltNum;
						milflight.FltSeqNum = flight.FlightSeqNum;
						milflight.Tfp = flight.Tfp;
						if (milflight.FltSeqNum == splitLeg)
						{
							flightType = "MD";
							isDroped = true;
						}

						milflight.IsDropped = isDroped;
						milflight.Type = flightType;
						//mILDutyPeriod.FlightDetails.Add(milflight);
					}
					mILDutyPeriod.Tfp = mILDutyPeriod.FlightDetails.Where(x => x.Type == "MD" || x.Type == "MA").Sum(y => y.Tfp);
				}
				// if the duty period type is "Split" and  trip type is MOB
				else if (tripDetails.TripType == "MOB")
				{
					string flightType = "MD";
					bool isDroped = true;
					foreach (var flight in dp.Flights)
					{
						MILFlights milflight = mILDutyPeriod.FlightDetails.FirstOrDefault(x => x.FltSeqNum == flight.FlightSeqNum);
						milflight.FlightNumber = flight.FltNum;
						milflight.FltSeqNum = flight.FlightSeqNum;
						milflight.Tfp = flight.Tfp;

						if (milflight.FltSeqNum == splitLeg)
						{
							flightType = "MO";
							isDroped = false;
						}
						DateTime dpdate = tripDetails.TripDate.AddDays(dp.DutPerSeqNum - 1);
						if (tripDetails.MILStartDate > dpdate.Date.AddMinutes(flight.ArrTime - (1440 * (dp.DutPerSeqNum - 1))))
						{

							// MILDutyPeriod milDutyPeriod = milTrip.DutyPeriodsDetails.FirstOrDefault(x => x.DpSeqNum == dp.DutPerSeqNum);
							// var miflight = milDutyPeriod.FlightDetails.FirstOrDefault(x => x.FlightNumber == flight.FlightSeqNum);
							//if (miflight != null)
							//{
							//    milflight.IsDropped = miflight.IsDropped;
							//    milflight.Type = miflight.Type;
							//}
							//else
							//{
							//   // milflight.IsDropped = isDroped;
							//  //  milflight.Type = flightType;
							//}
							//we dont need to set  new isdrop and fligt type for this becuse these flights are flying before MIL strart date in the case of MOB trip.

						}
						else
						{
							milflight.IsDropped = isDroped;
							milflight.Type = flightType;
						}


						//mILDutyPeriod.FlightDetails.Add(milflight);


					}

					mILDutyPeriod.Tfp = mILDutyPeriod.FlightDetails.Where(x => x.Type == "MD" || x.Type == "MA").Sum(y => y.Tfp);





				}

			}
			if (mILDutyPeriod.FlightDetails.Count(x => x.Type == "MD") == mILDutyPeriod.FlightDetails.Count())
			{
				mILDutyPeriod.Type = "MD";

			}
			else if (mILDutyPeriod.FlightDetails.Count(x => x.Type == "MO") == mILDutyPeriod.FlightDetails.Count())
			{
				mILDutyPeriod.Type = "MO";

			}
			//return mILDutyPeriod;
		}

		/// <summary>
		/// check whether the duty period has sip or not. if sip exists return true.
		/// </summary>
		/// <param name="dp"></param>
		/// <param name="sipLeg"></param>
		/// <param name="domicile"></param>
		/// <param name="reverse"></param>
		/// <param name="tripDetails"></param>
		/// <returns></returns>
		private bool CheckSip(DutyPeriod dp, ref int sipLeg, string domicile, Reverse reverse, TripDetails tripDetails, DateTime dpDate)
		{
			int i = 0;
			if (tripDetails.Trip.TripNum == "DACB")
				i = 0;

			List<Flight> flights = null;
			// for MOF trip ,we need to iterate the flight prior to the start of MIL start date (backwards iteration )
			if (reverse == Reverse.Yes)
				flights = dp.Flights.Reverse<Flight>().ToList();
			else
				flights = dp.Flights;



			if (tripDetails.TripType == "MOF")
			{
				int prevFlightSeq = 0;
				foreach (var flt in flights)
				{


					if (flt.FlightSeqNum == flights.Count || flt.ArrSta != domicile)
					{
						prevFlightSeq = flt.FlightSeqNum;
						continue;
					}

					// start frank add 2/17/2015 -- have to check if SIP departure is 1 hour prior after MILOA start

					bool status = true; //false means it is not a possible sip 

					if (dpDate.Date == tripDetails.MILStartDate.Date)
					{  //Checking Departure time is equal to or earlier than 1 hour after the start  of MILOA,
						var miloaStartTime = tripDetails.MILStartDate.TimeOfDay.Hours * 60 + tripDetails.MILStartDate.TimeOfDay.Minutes;
						//var possibleSipDepTime = flt.DepTime - (dp.DutPerSeqNum - 1) * 1440;
						var possibleSipDepTime = dp.Flights[flt.FlightSeqNum].DepTime - (dp.DutPerSeqNum - 1) * 1440;
						if (possibleSipDepTime >= miloaStartTime + GlobalSettings.show1stDay)
						{
							status = false;  // it is a possible SIP
						}
					}

					// end frank add

					// frank change 2/17/2015
					//if (flt.ArrSta == domicile)
					if (flt.ArrSta == domicile && status)
					{
						//sip exists
						sipLeg = prevFlightSeq;
						return true;
					}
					//if (dpDate.Date == tripDetails.MILStartDate.Date)
					//{
					//    if (flt.ArrSta == domicile && status)
					//    {
					//        //sip exists
					//        sipLeg = prevFlightSeq;
					//        return true;
					//    }
					//}
					//else
					//{
					//    if (flt.ArrSta == domicile)
					//    {
					//        //sip exists
					//        sipLeg = prevFlightSeq;
					//        return true;
					//    }
					//}


					prevFlightSeq = flt.FlightSeqNum; ;
				}

			}
			if (tripDetails.TripType == "MOB")
			{

				foreach (var flt in flights)
				{

					//if (flt.FlightSeqNum == 1)
					//    continue;
					bool status = true;

					//Modified By Vofox team on 11 th March 2015 to solve the MIL  issue
					//--------------------------------------------------------------------------------------------
					// if (dpDate.Date == tripDetails.MILEndDate.Date)
					if (dpDate.Date == tripDetails.MILEndDate.Date || dpDate.Date.AddDays(1) == tripDetails.MILEndDate.Date)
					{
						//Modified By Vofox team on 11 th March 2015 to solve the MIL  issue
						//--------------------------------------------------------------------------------------------
						//Checking Departure time is equal to or later than 1 hour after the end of MILOA,
						//if ((tripDetails.MILEndDate.TimeOfDay.Hours * 60 + tripDetails.MILEndDate.TimeOfDay.Minutes) + 60 > (flt.DepTime - (dp.DutPerSeqNum-1) * 1440))
						int dateDidd = tripDetails.MILEndDate.Date.Subtract(dpDate.Date).Days;
						if (dateDidd * 1440 + (tripDetails.MILEndDate.TimeOfDay.Hours * 60 + tripDetails.MILEndDate.TimeOfDay.Minutes) + 60 > (flt.DepTime - (dp.DutPerSeqNum - 1) * 1440))
						{

							status = false;
						}
					}
					//else if (dpDate.Date.AddDays(1)== tripDetails.MILEndDate.Date)
					//{
					//    if((flt.DepTime - (dp.DutPerSeqNum-1) * 1440) < 1440)
					//    {
					//         status = false;
					//    }
					//    else if((tripDetails.MILEndDate.TimeOfDay.Hours * 60 + tripDetails.MILEndDate.TimeOfDay.Minutes) + 60 <((flt.DepTime - (dp.DutPerSeqNum-1) * 1440)-1440))
					//        {
					//        status = false;
					//    }
					//}

					if (flt.DepSta == domicile && status)
					{
						//sip exists
						sipLeg = flt.FlightSeqNum; ;
						return true;
					}



				}
			}
			return false;
		}

		/// <summary>
		/// Check whether the dutyperiod is having the Split point city.return true if the dutyepriod is having Split point city
		/// </summary>
		/// <param name="dp"></param>
		/// <param name="sipLeg"></param>
		/// <param name="domicile"></param>
		/// <param name="reverse"></param>
		/// <param name="directionOfDh"></param>
		/// <param name="tripDetails"></param>
		/// <returns></returns>
		private bool CheckValidSPC(DutyPeriod dp, ref int sipLeg, string domicile, Reverse reverse, DeadHead directionOfDh, TripDetails tripDetails, DateTime dpDate)
		{

			bool status = false;
			List<Flight> flights = null;

			if (reverse == Reverse.Yes)
				flights = dp.Flights.Reverse<Flight>().ToList();
			else
				flights = dp.Flights;

			List<string> sPCCities = GlobalSettings.SplitPointCities.FirstOrDefault(x => x.Domicile == domicile).Cities;

			if (sPCCities.Count() == 0)
				return status;

			if (directionOfDh == DeadHead.toDomicile)
			{
				int prevFlightSeq = 0;
				foreach (var flt in flights)
				{
					// if Split Point City
					if (flt.ArrSta != null && sPCCities.Contains(flt.ArrSta.ToUpper()))
					{
						// The company would never split the trip with 1 flight being flown.The entire pairing would be dropped.
						if (flt.FlightSeqNum == 1 && dp.DutPerSeqNum == 1)
						{
							status = false;
							break;
						}

						if (CheckPltDeadHead(dp, flt, domicile, tripDetails, directionOfDh, dpDate))
						{
							status = true;
							sipLeg = prevFlightSeq;
							break;
						}
					}

					prevFlightSeq = flt.FlightSeqNum;
				}
			}
			else
			{

				foreach (var flt in flights)
				{
					bool isValidtoCheckSipcity = true;
					if (dpDate.Date == tripDetails.MILEndDate.Date)
					{  //Checking Departure time is equal to or later than 1 hour after the end of MILOA,
						if ((tripDetails.MILEndDate.TimeOfDay.Hours * 60 + tripDetails.MILEndDate.TimeOfDay.Minutes) + 60 > (flt.DepTime - (dp.DutPerSeqNum - 1) * 1440))
						{
							isValidtoCheckSipcity = false;
						}
					}
					// if Split Point City
					if (flt.ArrSta != null && sPCCities.Contains(flt.DepSta.ToUpper()) && isValidtoCheckSipcity)
					{
						//However, the company will never DH a pilot to fly just one flight back to domicile.  In this case, the last flight would be pulled also.
						if (dp.DutPerSeqNum == tripDetails.Trip.PairLength && flt.FlightSeqNum == dp.Flights.Count)
						{
							status = false;
							break;
						}

						if (CheckPltDeadHead(dp, flt, domicile, tripDetails, directionOfDh, dpDate))
						{
							status = true;
							sipLeg = flt.FlightSeqNum;
							break;
						}
					}


				}
			}
			return status;

		}
		/// <summary>
		/// Check whether the dutyperiod is in the Bid period. retutn true if the dutyperiod is in the bid period
		/// </summary>
		/// <param name="tripStartDate"></param>
		/// <param name="dutyperiodSequenseNumber"></param>
		/// <returns></returns>
		private bool CheckIsInBidPeriod(DateTime tripStartDate, int dutyperiodSequenseNumber)
		{
			DateTime dutyperiodDate = tripStartDate.AddDays(dutyperiodSequenseNumber-1);

			if (GlobalSettings.CurrentBidDetails.BidPeriodStartDate <= dutyperiodDate.Date && dutyperiodDate.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
				return true;
			else
				return false;
		}

		private bool CheckPltDeadHead(DutyPeriod dp, Flight flt, string domicile, TripDetails tripDetails, DeadHead directionOfDh, DateTime dpDate)
		{

			bool status = false;

			if (directionOfDh == DeadHead.toDomicile)
			{
				status = CkeckPltDhToDom(dp, flt, domicile, tripDetails, dpDate);

			}
			else
			{
				status = CheckPltDhToRejoinTrip(dp, flt, domicile, tripDetails, dpDate);
			}
			return status;


		}

		private bool CkeckPltDhToDom(DutyPeriod dp, Flight flt, string domicile, TripDetails tripDetails, DateTime dpDate)
		{
			//the arrival time in domicile is equal to or prior to the start of MILOA
			//------------------------------------------------------------------------------------------------



			//Convertion mil start time to int
			int milStartTime = tripDetails.MILStartDate.TimeOfDay.Hours * 60 + tripDetails.MILStartDate.TimeOfDay.Minutes;
			FlightRouteDetails flightRouteDetails = null;

			// if the SPC city dut period date and MIL start date are on same day we need to fetch all flights between the arrival of SPC city and  Mil start time

			// frank change 2/17/2015 -- changed dpDate to dpDate.Date
			// if (dpDate == tripDetails.MILStartDate.Date)


			if (dpDate.Date == tripDetails.MILStartDate.Date)
			{

				//flightRouteDetails = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == dpDate &&
				//                                                                   x.Cdep >= (flt.ArrTime % 1440) && x.Carr <= milStartTime &&
				//                                                                   x.Orig == flt.ArrSta && x.Dest == domicile)
				//                                                               .FirstOrDefault();
				flightRouteDetails = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == dpDate.Date &&
					x.Cdep >= (flt.ArrTime % 1440) + GlobalSettings.connectTime && x.Carr <= milStartTime &&
					x.Orig == flt.ArrSta && x.Dest == domicile)
					.FirstOrDefault();
			}

			// if the duty period date and Mil start date are different we need to  fetch all the flights between these two days
			else
			{

				//suppose       SPC date is 12 and  MIL start date is 14
				// Also SPC flight arrival time is 16:00  and  Mil start time is  10:00
				// so we need to fetch following flights
				// 1. All flights on 12th  ,departure time is  greater than 16:00
				// 2. All flights on 13 th
				// 3. All flights on 14 th , arrival time less than 10:00

				flightRouteDetails = GlobalSettings.FlightRouteDetails.FirstOrDefault(x =>

					#region First day
					(x.FlightDate == dpDate && x.Cdep >= (flt.ArrTime % 1440) && x.Orig == flt.ArrSta && x.Dest == domicile)
					#endregion

					||
					#region In Between
					(x.FlightDate > dpDate && x.FlightDate < tripDetails.MILStartDate.Date && x.Orig == flt.ArrSta && x.Dest == domicile)
					#endregion
					||

					#region Last Day's comarison
					(x.FlightDate == tripDetails.MILStartDate.Date && x.Carr <= milStartTime && x.Orig == flt.ArrSta && x.Dest == domicile)
					#endregion

				);
			}


			return (flightRouteDetails != null);



		}

		private bool CheckPltDhToRejoinTrip(DutyPeriod dp, Flight flt, string domicile, TripDetails tripDetails, DateTime dpDate)
		{


			//Convertion mil End time to int
			int milEndTime = tripDetails.MILEndDate.TimeOfDay.Hours * 60 + tripDetails.MILStartDate.TimeOfDay.Minutes;
			FlightRouteDetails flightRouteDetails = null;



			// if the MIL start date and SPC city duty period date   are on same day we need to fetch all flights between the a  Mil end time and arrival of SPC city 

			// frank change 2/17/2015
			// if (tripDetails.MILEndDate.Date == dpDate)
			var dutyPdSeq = tripDetails.Trip.DutyPeriods.Count;
			if (tripDetails.MILEndDate.Date == dpDate.Date && flt.FlightSeqNum != tripDetails.Trip.DutyPeriods[tripDetails.Trip.DutyPeriods.Count - 1].Flights.Count)
			{
				//The departure time in domicile is equal to or later than 1 hour after the end of MILOA, 
				//and the arrival in the SPLIT POINT city is equal to or earlier than 40 minutes prior to the departure time in the SPLIT POINT city. 

				// frank change 2/17/2015 -- we need to check for flights from Domicile to the Departure city

				//flightRouteDetails = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == tripDetails.MILEndDate.Date &&
				//                                                               x.Cdep >= milEndTime + 60 && x.Carr <= (flt.DepTime % 1440) - 40 &&
				//                                                               x.Orig == flt.ArrSta && x.Dest == domicile)
				//                                                               .FirstOrDefault();

				flightRouteDetails = GlobalSettings.FlightRouteDetails.Where(x => x.FlightDate == tripDetails.MILEndDate.Date &&
					x.Cdep >= milEndTime + 60 && x.Carr <= (flt.DepTime % 1440) - 40 &&
					x.Orig == domicile && x.Dest == flt.DepSta)
					.FirstOrDefault();
			}
			// if the  Mil end date and duty period date are different we need to  fetch all the flights between these two days
			else
			{

				//suppose      MIL end date  and SPC date is 12 and  is 14  Also Mil end  time is  10:00 and   SPC flight arrival time is 16:00  and  
				// so we need to fetch following flights
				// 1. All flights on 12th  ,departure time is  greater than 10:00 + 1 Hour
				// 2. All flights on 13 th
				// 3. All flights on 14 th , arrival time less than 16:00 - 40 minute

				flightRouteDetails = GlobalSettings.FlightRouteDetails.FirstOrDefault(x =>

					#region First day
					(x.FlightDate == tripDetails.MILEndDate.Date &&
						x.Cdep >= milEndTime + 60 &&
						x.Orig == domicile && x.Dest == flt.DepSta)
					#endregion
					||
					#region In Between
					(x.FlightDate > tripDetails.MILEndDate && x.FlightDate < dpDate &&
						x.Orig == domicile && x.Dest == flt.DepSta)
					#endregion
					||
					#region Last Day
					(x.FlightDate == dpDate &&
						x.Carr <= (flt.DepTime % 1440) - 40 &&
						x.Orig == domicile && x.Dest == flt.DepSta
					)
					#endregion

				);
			}

			return (flightRouteDetails != null);


		}

		//private bool isDutyperiodInBp(Trip trip,DutyPeriod dp)
		//{

		//}

		public class TripDetails
		{

			public Trip Trip { get; set; }

			public DateTime TripDate { get; set; }

			public DateTime TripEndDate { get; set; }

			public string TripType { get; set; }

			public DateTime MILStartDate { get; set; }

			public DateTime MILEndDate { get; set; }

			public bool IsReserveLine { get; set; }

			// public bool IsReserve { get; set; }


		}
		public enum Reverse
		{
			No = 0,
			Yes
		};

		public enum DeadHead
		{
			toDomicile = 0,
			toRejoinTrip
		};

		#endregion
	}
}
