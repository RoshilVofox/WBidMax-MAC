#region NameSpace
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
#endregion

namespace VacationCorrection
{
	public class VacationCorrectionBL
	{

		public DateTime eOMCPVacationStartDate;

		private NetworkPlanData _networkPlanData;
		private int currentLine;


		/// <summary>
		/// Calculate Vacation correction
		/// </summary>
		/// <param name="vacationParameter"></param>
		/// <returns></returns>
		public Dictionary<string, TripMultiVacData> PerformVacationCorrection(VacationCorrectionParams vacationParameter)
		{
			/*************************************************************************************************************************************
          * The VAC file object is Dictionary<string, TripMultiVacData>.  The string is a trip/date in the form BAM327, where BAM3 is the trip,
          * 27 is the start date of the trip.  The TripMultiVacData contains a VA, VOF, and VOB VacationTrip object for the trip. 
          * Every trip has a VA VacationTrip object, while those trips that span Sat-Sun have both VOF and VOB VacatonTrip objects.
          * ***********************************************************************************************************************************/

			//Checking if any flight route exists
			if (vacationParameter.FlightRouteDetails == null)
				return null;
			_networkPlanData = new NetworkPlanData();
			GlobalSettings.CurrentBidDetails = vacationParameter.CurrentBidDetails;
			GlobalSettings.FlightRouteDetails = vacationParameter.FlightRouteDetails;

			Dictionary<string, TripMultiVacData> allTripsMultiVacData = new Dictionary<string, TripMultiVacData>();
			try
			{
				foreach (var line in vacationParameter.Lines)
				{

					if (!line.Value.BlankLine)
					{
						if (!line.Value.ReserveLine)
						{
							ProcessNormalLines(vacationParameter.Trips, allTripsMultiVacData, line.Value, vacationParameter.IsEOM);
						}
						else
						{
							ProcessReserveLines(vacationParameter.Trips, allTripsMultiVacData, line.Value, vacationParameter.IsEOM);
						}

					}

				}

			}
			catch (Exception ex)
			{

				throw ex;
			}
			return allTripsMultiVacData;

		}


		public Dictionary<string, TripMultiVacData> PerformFAVacationCorrection(VacationCorrectionParams vacationParameter)
		{
			/*************************************************************************************************************************************
          * The VAC file object is Dictionary<string, TripMultiVacData>.  The string is a trip/date in the form BAM327, where BAM3 is the trip,
          * 27 is the start date of the trip.  The TripMultiVacData contains a VA, VOF, and VOB VacationTrip object for the trip. 
          * Every trip has a VA VacationTrip object, while those trips that span Sat-Sun have both VOF and VOB VacatonTrip objects.
          * ***********************************************************************************************************************************/

			//Checking if any flight route exists

			_networkPlanData = new NetworkPlanData();
			GlobalSettings.CurrentBidDetails = vacationParameter.CurrentBidDetails;
			// GlobalSettings.FlightRouteDetails = vacationParameter.FlightRouteDetails;

			Dictionary<string, TripMultiVacData> allTripsMultiVacData = new Dictionary<string, TripMultiVacData>();
			try
			{
				foreach (var line in vacationParameter.Lines)
				{

					if (!line.Value.BlankLine)
					{
						if (!line.Value.ReserveLine)
						{
							ProcessFANormalLines(vacationParameter.Trips, allTripsMultiVacData, line.Value, vacationParameter.IsEOM);
						}
						//else
						//{
						//    ProcessReserveLines(vacationParameter.Trips, allTripsMultiVacData, line.Value, vacationParameter.IsEOM);
						//}

					}

				}

			}
			catch (Exception ex)
			{

				throw ex;
			}
			return allTripsMultiVacData;

		}



		public Dictionary<string, TripMultiVacData> CreateVACfileForEOMFA(VacationCorrectionParams vacationParameter, Dictionary<string, TripMultiVacData> allTripsMultiVacData)
		{


			foreach (var line in vacationParameter.Lines)
			{
				if (!line.Value.BlankLine)
				{
					ProcessNormalLinesForEOMFA(vacationParameter.Trips, allTripsMultiVacData, line.Value, GlobalSettings.FAEOMStartDate);

				}
			}   // end foreach var line in lines

			return allTripsMultiVacData;
		}

		private void ProcessNormalLinesForEOMFA(Dictionary<string, Trip> trips, Dictionary<string, TripMultiVacData> allTripsMultiVacData, Line line, DateTime vacationStartDate)
		{
			DateTime tripStartDate, tripEndDate;
			Trip trip = null;
			bool isLastTrip = false; int paringCount = 0;
			foreach (var pairing in line.Pairings)
			{
				isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

				//tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, Convert.ToInt16(pairing.Substring(4, 2)));
				tripStartDate = VacationHelper.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
				//string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                //string splitName = string.Empty;
                //if (GlobalSettings.CurrentBidDetails.Round == "M")
                //{
                //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                //}
                //else
                //{
                //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                //}
                string splitName = pairing.Substring(0, 4);
                trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == splitName);
                if (trip == null)
                {
                    splitName = pairing;
                   trip= GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == splitName);
                }
				tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);
				// instantiate TripMultiVacData
				if (!allTripsMultiVacData.Keys.Contains(pairing))
				{
					TripMultiVacData tripMultiVacData = new TripMultiVacData();
					// pairing key is 6 characters ppppdd
					allTripsMultiVacData.Add(pairing, tripMultiVacData);
				}

				if (tripEndDate >= vacationStartDate)
				{


					allTripsMultiVacData[pairing].VofData = CreateVacationTripForEOMFA(splitName, trip, tripStartDate, "VOF", line.ReserveLine, vacationStartDate);
				}
			}

		}

		private VacationTrip CreateVacationTripForEOMFA(string tripName, Trip trip, DateTime tripStartDate, string tripType, bool isReserve, DateTime vacationStartDate)
		{

			VacationTrip vacTrip = new VacationTrip();
			DateTime dutyPeriodDate = tripStartDate;
			string dutyPeriodType = string.Empty;

			vacTrip.VacationType = tripType;

			CreateVacTripObjects(vacTrip);
			foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
			{
				VacationDutyPeriod vacDutyPeriod = new VacationDutyPeriod();
				CreateVacDutyPeriodObjects(vacDutyPeriod);
                vacDutyPeriod.RefDutPdSeqNum = dutyPeriod.DutPerSeqNum;
				vacTrip.DutyPeriodsDetails.Add(vacDutyPeriod);
				if (vacationStartDate > dutyPeriodDate)
				{
					dutyPeriodType = "VD";
				}
				else
				{
					dutyPeriodType = "VA";
				}
				vacDutyPeriod.isInBp = IsDutyPeriodInBidPeriod(tripStartDate, dutyPeriod.DutPerSeqNum);

				CreateFltsAndMakeDpForFA(vacDutyPeriod, dutyPeriod, dutyPeriodType, trip.PairLength);
				dutyPeriodDate = dutyPeriodDate.AddDays(1);

			}

			vacTrip.RigAdg = trip.RigAdg;
			vacTrip.RigTafb = trip.RigTafb;
			DistributeRig(vacTrip);
			distributeAdgTafbRig(vacTrip);
			vacTrip.VacationGrandTotal = GetGrandTotal(vacTrip);
			vacTrip.TripName = tripName;



			return vacTrip;
		}

		private void CreateFltsAndMakeDpForFA(VacationDutyPeriod vacDutyPeriod, DutyPeriod dp, string dutPdType, int tripLength)
		{

			foreach (var flt in dp.Flights)
			{
				VacationFlight vacFlt = new VacationFlight();
				vacFlt.VacationDetail = new VacationDetails();

				vacFlt.FlightNumber = flt.FltNum.ToString();
				vacFlt.RefFltSeqNum = flt.FlightSeqNum;
				vacFlt.VacationType = dutPdType;
				vacFlt.Tfp = flt.Tfp;
				vacFlt.Block = flt.Block;
				vacDutyPeriod.FlightDetails.Add(vacFlt);
			}

			vacDutyPeriod.VacationType = dutPdType;


			vacDutyPeriod.RigDpMin = dp.RigDailyMin;
			vacDutyPeriod.RigDhr = dp.RigDhr;

			if (dutPdType == "VA")
			{
				vacDutyPeriod.RigDpMin_VA = dp.RigDailyMin;
				vacDutyPeriod.RigDhr_VA = dp.RigDhr;
			}
			else
			{
				vacDutyPeriod.RigDpMin_VD = dp.RigDailyMin;
				vacDutyPeriod.RigDhr_VD = dp.RigDhr;
			}

			//Calculate Tafb
			//-----------------------------------------------------
			if (vacDutyPeriod.isInBp)
			{
				if (dutPdType == "VA")
				{
					vacDutyPeriod.Tafb_VA = CalculateTafbInBpOfDutyPeriod(dp, tripLength);
				}
				else
				{
					vacDutyPeriod.Tafb_VD = CalculateTafbInBpOfDutyPeriod(dp, tripLength);
				}
			}

			//-----------------------------------------------------------------------------------


		}


		private void ProcessNormalLines(Dictionary<string, Trip> trips, Dictionary<string, TripMultiVacData> allTripsMultiVacData, Line line,bool isEOM)
		{
			try
			{
				DateTime tripStartDate, tripEndDate;
				int tripEndTime = 0;
				bool isLastTrip = false; int paringCount = 0;
				Trip trip = null;

				if (line.LineNum == 189)
				{ 
					var a = 10;
				}

				foreach (var pairing in line.Pairings)
				{
					// frank add 4/13/2015 to see what line is being processed later in VDvsRC
					currentLine = line.LineNum;

					isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
					tripStartDate = VacationHelper.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //string splitName = string.Empty;
                    //if (GlobalSettings.CurrentBidDetails.Round == "M")
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //}
                    //else
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                    //}
                    string splitName = (trips.ContainsKey(pairing.Substring(0, 4))) ? pairing.Substring(0, 4) : pairing;
                    trip = trips[splitName];
					tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);

					// added to capture trips that arrive back in domicile after 2400 domicile time
					tripEndTime = (trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg) % 1440;
					tripEndTime = AfterMidnightLandingTime(tripEndTime);
					tripEndTime = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripEndDate, tripEndTime);

					if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && ((x.StartAbsenceDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440))))
					{
						trip.DutyPeriods[trip.DutyPeriods.Count - 1].ArrivesAfterMidnightDayBeforeVac = true;
					}

					// instantiate TripMultiVacData
					TripMultiVacData tripMultiVacData = new TripMultiVacData();

					// pairing key is 6 characters ppppdd
					if (allTripsMultiVacData.ContainsKey(pairing))
					{
						continue;
					}



					if (!isEOM)
					{
						//  1.Check if the trip is completely inside the vacation (VA Vacation)
						//-------------------------------------------------------------------------------------------------
						if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.StartAbsenceDate <= tripStartDate && x.EndAbsenceDate >= tripEndDate)))
						{
							tripMultiVacData.VaData = CreateVacationTrip_VA(pairing, trips[splitName], tripStartDate, line.ReserveLine);
						}
						// 2.Check if trip starts before the vacation period and finishes inside the vacation period. (VDF Vacation)
						//-------------------------------------------------------------------------------------------------
						else if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && ((x.StartAbsenceDate > tripStartDate) && (x.StartAbsenceDate <= tripEndDate)) ||
							((x.StartAbsenceDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440))))
						{
							tripMultiVacData.VofData = CreateVacationTrip_VOF_VOB(trips[splitName], tripStartDate, "VOF", line.ReserveLine);
						}

						// 3.check if trip starts inside the vacation period and finished outside the vacation period.. (VDB Vacation)
						//-------------------------------------------------------------------------------------------------

						else if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.EndAbsenceDate >= tripStartDate && x.EndAbsenceDate <= tripEndDate)))
						{
							tripMultiVacData.VobData = CreateVacationTrip_VOF_VOB(trips[splitName], tripStartDate, "VOB", line.ReserveLine);
						}

					}

					// The next two if clauses handle trips that overlap into next month.  There are NO VOB trips that overlap.

					if (tripStartDate.AddDays(trips[splitName].PairLength) > GlobalSettings.CurrentBidDetails.BidPeriodEndDate && tripMultiVacData.VaData == null)
					{
						tripMultiVacData.VaData = CreateVacationTrip_VA(pairing, trips[splitName], tripStartDate, line.ReserveLine);
					}
					if (tripStartDate.AddDays(trips[splitName].PairLength) > GlobalSettings.CurrentBidDetails.BidPeriodEndDate && tripMultiVacData.VofData == null)
					{
						tripMultiVacData.VofData = CreateVacationTrip_VOF_VOB(trips[splitName], tripStartDate, "VOF", line.ReserveLine);
					}

					if (tripMultiVacData.VofData != null || tripMultiVacData.VaData != null || tripMultiVacData.VobData != null)
					{
						allTripsMultiVacData.Add(pairing, tripMultiVacData);
					}

				}
			}
			catch (Exception ex)
			{
				//string ss = ex.Message;
				throw ex;
			}

			// end foreach var trip in line.value.pairings

		}

		private void ProcessFANormalLines(Dictionary<string, Trip> trips, Dictionary<string, TripMultiVacData> allTripsMultiVacData, Line line, bool isEOM)
		{
			try
			{
				DateTime tripStartDate, tripEndDate;
				int tripEndTime = 0;
				bool isLastTrip = false; int paringCount = 0;
				Trip trip = null;




				foreach (var pairing in line.Pairings)
				{
					isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
					tripStartDate = VacationHelper.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
					//string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //string splitName = string.Empty;
                    //if (GlobalSettings.CurrentBidDetails.Round == "M")
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //}
                    //else
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                    //}
                    string splitName = (trips.ContainsKey(pairing.Substring(0, 4))) ? pairing.Substring(0, 4) : pairing;
					trip = trips[splitName];
					tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);

					// added to capture trips that arrive back in domicile after 2400 domicile time
					tripEndTime = (trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg) % 1440;
					tripEndTime = AfterMidnightLandingTime(tripEndTime);
					tripEndTime = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripEndDate, tripEndTime);

					if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && ((x.StartAbsenceDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440))))
					{
						trip.DutyPeriods[trip.DutyPeriods.Count - 1].ArrivesAfterMidnightDayBeforeVac = true;
					}

					// instantiate TripMultiVacData
					TripMultiVacData tripMultiVacData = new TripMultiVacData();

					// pairing key is 6 characters ppppdd
					if (allTripsMultiVacData.ContainsKey(pairing))
					{
						continue;
					}



					if (!isEOM)
					{
						//  1.Check if the trip is completely inside the vacation (VA Vacation)
						//-------------------------------------------------------------------------------------------------
						if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.StartAbsenceDate <= tripStartDate && x.EndAbsenceDate >= tripEndDate)))
						{
							tripMultiVacData.VaData = CreateVacationTripForFA(splitName, trips[splitName], tripStartDate, "VA", line.ReserveLine);
						}
						// 2.Check if trip starts before the vacation period and finishes inside the vacation period. (VDF Vacation)
						//-------------------------------------------------------------------------------------------------
						else if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && ((x.StartAbsenceDate > tripStartDate) && (x.StartAbsenceDate <= tripEndDate)) ||
							((x.StartAbsenceDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440))))
						{
							tripMultiVacData.VofData = CreateVacationTripForFA(splitName, trips[splitName], tripStartDate, "VOF", line.ReserveLine);
						}

						// 3.check if trip starts inside the vacation period and finished outside the vacation period.. (VDB Vacation)
						//-------------------------------------------------------------------------------------------------

						else if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.EndAbsenceDate >= tripStartDate && x.EndAbsenceDate <= tripEndDate)))
						{
							tripMultiVacData.VobData = CreateVacationTripForFA(splitName, trips[splitName], tripStartDate, "VOB", line.ReserveLine);
						}

					}


					if (tripStartDate >= GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(-2) && tripMultiVacData.VaData == null)
					{
						tripMultiVacData.VaData = CreateVacationTripForFA(splitName, trip, tripStartDate, "VA", line.ReserveLine);
					}

					if (tripMultiVacData.VofData != null || tripMultiVacData.VaData != null || tripMultiVacData.VobData != null)
					{
						allTripsMultiVacData.Add(pairing, tripMultiVacData);
					}

				}
			}
			catch (Exception ex)
			{
				//string ss = ex.Message;
				throw ex;
			}

			// end foreach var trip in line.value.pairings

		}

		private void ProcessReserveLines(Dictionary<string, Trip> trips, Dictionary<string, TripMultiVacData> allTripsMultiVacData, Line line,bool isEOM)
		{
			try
			{
				DateTime tripStartDate, tripEndDate; ;
				bool isLastTrip = false; int paringCount = 0;
				Trip trip = null;
				foreach (var pairing in line.Pairings)
				{
					isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
					//tripStartDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, Convert.ToInt16(trip.Substring(4, 2)));
					tripStartDate = VacationHelper.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
					//string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //string splitName = string.Empty;
                    //if (GlobalSettings.CurrentBidDetails.Round == "M")
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    //}
                    //else
                    //{
                    //    splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                    //}
                    string splitName = (trips.ContainsKey(pairing.Substring(0, 4))) ? pairing.Substring(0, 4) : pairing;
					trip = trips[splitName];
                    
					tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);
					// instantiate TripMultiVacData
					TripMultiVacData tripMultiVacData = new TripMultiVacData();
					// pairing key is 6 characters ppppdd
					if (allTripsMultiVacData.ContainsKey(pairing))
					{
						continue;
					}


					if (!isEOM)
					{

						//  1.Check if the trip is completely inside the vacation (VA Vacation)
						//-------------------------------------------------------------------------------------------------
						if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.StartAbsenceDate <= tripStartDate && x.EndAbsenceDate >= tripEndDate)))
						{
							tripMultiVacData.VaData = CreateVacationTrip_VA(pairing, trips[splitName], tripStartDate, line.ReserveLine); ;
						}
						// 2.Check if trip starts before the vacation period and finishes inside the vacation period. (VDF Vacation)
						//-------------------------------------------------------------------------------------------------
						else if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && ((x.StartAbsenceDate > tripStartDate) && (x.StartAbsenceDate <= tripEndDate))))
						{
							tripMultiVacData.VofData = CreateVacationTrip_VOF_VOBReserveLine(trips[splitName], tripStartDate, "VOF");
						}

						// 3.check if trip starts inside the vacation period and finished outside the vacation period.. (VDB Vacation)
						//-------------------------------------------------------------------------------------------------

						else if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.EndAbsenceDate >= tripStartDate && x.EndAbsenceDate <= tripEndDate)))
						{
							tripMultiVacData.VobData = CreateVacationTrip_VOF_VOBReserveLine(trips[splitName], tripStartDate, "VOB");
						}
					}


					if (tripStartDate.AddDays(trips[splitName].PairLength) > GlobalSettings.CurrentBidDetails.BidPeriodEndDate && tripMultiVacData.VaData == null)
					{
						tripMultiVacData.VaData = CreateVacationTrip_VA(pairing, trips[splitName], tripStartDate, line.ReserveLine); ;
					}
					if (tripStartDate.AddDays(trips[splitName].PairLength) > GlobalSettings.CurrentBidDetails.BidPeriodEndDate && tripMultiVacData.VofData == null)
					{
						tripMultiVacData.VofData = CreateVacationTrip_VOF_VOBReserveLine(trips[splitName], tripStartDate, "VOF");
					}

					if (tripMultiVacData.VofData != null || tripMultiVacData.VaData != null || tripMultiVacData.VobData != null)
					{
						allTripsMultiVacData.Add(pairing, tripMultiVacData);
					}


				}

			}
			catch (Exception ex)
			{

				throw ex;
			}

		}

		#region Reserveline Vacation

		//private VacationTrip CreateVacationTrip_VAforReserveLine(string tripAndDate, Trip trip, DateTime tripStartDate)
		//{

		//    VacationTrip vaTrip = null;
		//    vaTrip = CreateVacationTripObject(vaTrip);
		//    int tripTafbInBp = 0;

		//    foreach (var dutPd in trip.DutyPeriods)
		//    {

		//        VacationDutyPeriod vacDutyPeriod = new VacationDutyPeriod();
		//        CreateVacDutyPeriodObjects(vacDutyPeriod);
		//        VacationFlight vacFlt = null;

		//        vacDutyPeriod.VacationType = "VA";
		//        vacDutyPeriod.RefDutPdSeqNum = dutPd.DutPerSeqNum;

		//        // bool inBidPeriod 
		//        vacDutyPeriod.isInBp = IsDutyPeriodInBidPeriod(tripStartDate, dutPd.DutPerSeqNum);


		//        foreach (var flt in dutPd.Flights)
		//        {
		//            vacFlt = new VacationFlight();
		//            vacFlt.VacationDetail = new VacationDetails();
		//            vacFlt.VacationType = "VA";
		//            vacFlt.RefFltSeqNum = flt.FlightSeqNum;
		//            //since the reverse line  has only flight
		//            vacFlt.Tfp = dutPd.Tfp;
		//            vacFlt.Block = flt.Block;
		//            vacDutyPeriod.FlightDetails.Add(vacFlt);
		//        }



		//        //Tafb Calculation
		//        //-----------------------------------
		//        if (vacDutyPeriod.isInBp)
		//        {
		//            tripTafbInBp = CalculateTafbInBpOfDutyPeriod(dutPd, trip.PairLength);

		//        }
		//        vacDutyPeriod.Tafb_VA = tripTafbInBp;
		//        //-----------------------------------
		//        vacDutyPeriod.RigDpMin = dutPd.RigDailyMin;
		//        vacDutyPeriod.RigDhr = dutPd.RigDhr;
		//        vacDutyPeriod.RigDpMin_VA = dutPd.RigDailyMin;
		//        vacDutyPeriod.RigDhr_VA = dutPd.RigDhr;
		//        //======

		//        vacDutyPeriod.VacationDetails.VATafbInLine = trip.Tafb;
		//        vaTrip.DutyPeriodsDetails.Add(vacDutyPeriod);
		//    }
		//    vaTrip.VacationDetails.VacationType = "VA";
		//    vaTrip.TripName = tripAndDate;
		//    //assignTripTfpBlockAndTafb_VA(vaTrip, trip);
		//    vaTrip.RigAdg = trip.RigAdg;
		//    vaTrip.RigTafb = trip.RigTafb;

		//    vaTrip.RigVAVO_Adg = vaTrip.RigAdg;
		//    vaTrip.RigVAVO_Tafb = vaTrip.RigTafb;

		//    vaTrip.VacationGrandTotal = GetGrandTotal(vaTrip);

		//    return vaTrip;
		//}

		private VacationTrip CreateVacationTrip_VOF_VOBReserveLine(Trip trip, DateTime tripDate, string tripType)
		{


			VacationTrip vacTrip = new VacationTrip();
			CreateVacTripObjects(vacTrip);
			vacTrip.VacationType = tripType;

			List<DutyPeriod> dutPds = null;
			dutPds = trip.DutyPeriods;


			DateTime dpDate = tripDate;

			foreach (var dp in dutPds)
			{
				bool isDpInBp = IsDutyPeriodInBidPeriod(tripDate, dp.DutPerSeqNum);
				VacationDutyPeriod vacDutyPeriod = new VacationDutyPeriod();
				CreateVacDutyPeriodObjects(vacDutyPeriod);
				vacDutyPeriod.isInBp = isDpInBp;
				vacDutyPeriod.hasSip = false;
                vacDutyPeriod.RefDutPdSeqNum = dp.DutPerSeqNum;

				if (IsDutyPeriodInVacationPeriod(dp, tripDate, tripType, trip.PairLength))
				{

					CreateFltsAndMakeDpReserveLine(vacDutyPeriod, dp, "VA", tripType, trip.PairLength);
				}
				else
				{
					CreateFltsAndMakeDpReserveLine(vacDutyPeriod, dp, "VD", tripType, trip.PairLength);
				}

				vacTrip.DutyPeriodsDetails.Add(vacDutyPeriod);
				dpDate = dpDate.AddDays(1);
			}


			vacTrip.RigAdg = trip.RigAdg;
			vacTrip.TripName = trip.TripNum;
			vacTrip.RigTafb = trip.RigTafb;


			//DistributeRig(vacTrip);
			//int hasSipCount = vacTrip.DutyPeriodsDetails.Where(x => x.hasSip == true).Count();
			//// we don't check VDvsRC with a SIP
			//if (hasSipCount == 0)
			//    CalcVDvsRC(vacTrip, trip, tripDate);
			//Calculating Grand toal 
			vacTrip.VacationGrandTotal = GetGrandTotal(vacTrip);



			return vacTrip;
		}

		private void CreateFltsAndMakeDpReserveLine(VacationDutyPeriod vacDutyPeriod, DutyPeriod dp, string dutPdType, string tripType, int tripLength)
		{

			List<Flight> flights = dp.Flights.ToList();

			foreach (var flt in flights)
			{
				VacationFlight vacFlt = new VacationFlight();
				vacFlt.VacationDetail = new VacationDetails();

				vacFlt.FlightNumber = flt.FltNum.ToString();
				vacFlt.RefFltSeqNum = flt.FlightSeqNum;
				vacFlt.VacationType = dutPdType;
				//Since the reseve line will have only one flight
				vacFlt.Tfp = dp.Tfp;//flt.Tfp;
				vacFlt.Block = flt.Block;
				vacDutyPeriod.FlightDetails.Add(vacFlt);
			}


			vacDutyPeriod.VacationType = dutPdType;

			vacDutyPeriod.RigDpMin = dp.RigDailyMin;
			vacDutyPeriod.RigDhr = dp.RigDhr;

			//Calculate Tafb
			//-----------------------------------------------------

			if (dutPdType == "VA")
				vacDutyPeriod.Tafb_VA = CalculateTafbInBpOfDutyPeriod(dp, tripLength);

			else if (dutPdType == "VD")
				vacDutyPeriod.Tafb_VD = CalculateTafbInBpOfDutyPeriod(dp, tripLength);

			//-----------------------------------------------------------------------------------

		}





		#endregion

		private VacationTrip CreateVacationTrip_VA(string tripAndDate, Trip trip, DateTime tripStartDate, bool isReserve)
		{


			VacationTrip vaTrip = null;
			try
			{
				vaTrip = CreateVacationTripObject(vaTrip);
				int tripTafbInBp = 0;

				foreach (var dutPd in trip.DutyPeriods)
				{

					VacationDutyPeriod vacDutyPeriod = new VacationDutyPeriod();
					CreateVacDutyPeriodObjects(vacDutyPeriod);
					VacationFlight vacFlt = null;
					vacDutyPeriod.VacationType = "VA";
					vacDutyPeriod.RefDutPdSeqNum = dutPd.DutPerSeqNum;

					// bool inBidPeriod 
					vacDutyPeriod.isInBp = IsDutyPeriodInBidPeriod(tripStartDate, dutPd.DutPerSeqNum);

					foreach (var flt in dutPd.Flights)
					{
						vacFlt = new VacationFlight();
						vacFlt.VacationDetail = new VacationDetails();
						vacFlt.VacationType = "VA";
						vacFlt.RefFltSeqNum = flt.FlightSeqNum;
						vacFlt.Tfp = flt.Tfp;
						vacFlt.Block = flt.Block;
						vacDutyPeriod.FlightDetails.Add(vacFlt);
					}


					//Tafb Calculation
					//-----------------------------------
					if (vacDutyPeriod.isInBp)
					{
						tripTafbInBp = CalculateTafbInBpOfDutyPeriod(dutPd, trip.PairLength);

					}
					vacDutyPeriod.Tafb_VA = tripTafbInBp;
					//-----------------------------------
					vacDutyPeriod.RigDpMin = dutPd.RigDailyMin;
					vacDutyPeriod.RigDhr = dutPd.RigDhr;
					vacDutyPeriod.RigDpMin_VA = dutPd.RigDailyMin;
					vacDutyPeriod.RigDhr_VA = dutPd.RigDhr;
					//======

					vacDutyPeriod.VacationDetails.VATafbInLine = trip.Tafb;
					vaTrip.DutyPeriodsDetails.Add(vacDutyPeriod);
				}
				vaTrip.VacationDetails.VacationType = "VA";
				vaTrip.TripName = tripAndDate;
				//assignTripTfpBlockAndTafb_VA(vaTrip, trip);
				vaTrip.RigAdg = trip.RigAdg;
				vaTrip.RigTafb = trip.RigTafb;

				vaTrip.RigVAVO_Adg = vaTrip.RigAdg;
				vaTrip.RigVAVO_Tafb = vaTrip.RigTafb;
			}
			catch (Exception ex)
			{

				throw ex;
			}
			vaTrip.VacationGrandTotal = GetGrandTotal(vaTrip);

			return vaTrip;
		}

		private VacationTrip CreateVacationTripForFA(string tripName, Trip trip, DateTime tripStartDate, string tripType, bool isReserve)
		{

			VacationTrip vacTrip = new VacationTrip();
			DateTime dutyPeriodDate = tripStartDate;
			string dutyPeriodType = string.Empty;

			vacTrip.VacationType = tripType;

			if (tripType == "VA")
			{
				vacTrip = CreateVacationTrip_VA(tripName, trip, tripStartDate, isReserve);
			}
			else
			{
				CreateVacTripObjects(vacTrip);
				foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
				{
					VacationDutyPeriod vacDutyPeriod = new VacationDutyPeriod();
					CreateVacDutyPeriodObjects(vacDutyPeriod);
					vacTrip.DutyPeriodsDetails.Add(vacDutyPeriod);
					if (GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && x.StartAbsenceDate <= dutyPeriodDate && x.EndAbsenceDate >= dutyPeriodDate))
					{
						dutyPeriodType = "VA";
					}
					else
					{
						dutyPeriodType = "VD";
					}
                    vacDutyPeriod.RefDutPdSeqNum = dutyPeriod.DutPerSeqNum;
					vacDutyPeriod.isInBp = IsDutyPeriodInBidPeriod(tripStartDate, dutyPeriod.DutPerSeqNum);
					CreateFltsAndMakeDpForFA(vacDutyPeriod, dutyPeriod, dutyPeriodType, trip.PairLength);
					dutyPeriodDate = dutyPeriodDate.AddDays(1);

				}

				vacTrip.RigAdg = trip.RigAdg;
				vacTrip.RigTafb = trip.RigTafb;
				DistributeRig(vacTrip);
				distributeAdgTafbRig(vacTrip);
				vacTrip.VacationGrandTotal = GetGrandTotal(vacTrip);
				vacTrip.TripName = tripName;
			}


			return vacTrip;
		}


		private VacationTrip CreateVacationTrip_VOF_VOB(Trip trip, DateTime tripDate, string tripType, bool isReserve)
		{
			/************************************************************************
             * if a trip spansSatSun then it can be a VA, VOF, or VOB trip
             * if a trip does not spansSatSun, then it can only be a VA trip
             * 
             * I am creating a VAC file only for purposes of validating the Vacation Correction
             * algorithm.  The newly filtered query is fast enough for in-place Vacation Correction 
             * in the client.
             * 
             * This VAC file creation is NOT going to be accessable for USERS in the client.
             * *********************************************************************/

			// frank debug
			//if (trip.TripNum.Contains("BACW") && tripType=="VOF")
			if (trip.TripNum.Contains("DAIL"))
			{
				string ss = string.Empty;
			}

			VacationTrip vacTrip = new VacationTrip();
			try
			{
				CreateVacTripObjects(vacTrip);
				vacTrip.VacationType = tripType;

				// Reverse reverse = tripType == "VOF" ? Reverse.Yes : Reverse.No;
				//Modified by Vofox team
				Reverse reverse;

				string frontOrBack = tripType.Substring(2, 1);      // gets the F or B from trip type
				string flts1st = string.Empty;
				string flts2nd = string.Empty;

				DeadHead directionOfDh;

				if (frontOrBack == "F")
				{
					flts1st = "VD";
					flts2nd = "VO";
					directionOfDh = DeadHead.toDomicile;
					reverse = Reverse.Yes;
				}
				else
				{
					//tripMixedType = "VOVDB";
					//flts1st = "VOB";
					//flts2nd = "VDB";
					flts1st = "VO";
					flts2nd = "VD";
					directionOfDh = DeadHead.toRejoinTrip;
					reverse = Reverse.No;
				}

				List<DutyPeriod> dutPds = null;
				if (reverse == Reverse.Yes)
					dutPds = trip.DutyPeriods.Reverse<DutyPeriod>().ToList();
				else
					dutPds = trip.DutyPeriods;

				int dhLeg = 0;
				int sipLeg = 0;
				int legZero = 0;
				bool isSplit = false;
				bool isPrevVD = false;
				bool is2ndDp = false;
				bool isPrevVO = false;


				DateTime dpDate = reverse == Reverse.No ? tripDate : tripDate.AddDays(trip.DutyPeriods.Count() - 1);
				foreach (var dp in dutPds)
				{
					bool isDpInBp = IsDutyPeriodInBidPeriod(tripDate, dp.DutPerSeqNum);
					VacationDutyPeriod vacDutyPeriod = new VacationDutyPeriod();
					CreateVacDutyPeriodObjects(vacDutyPeriod);
					vacDutyPeriod.isInBp = isDpInBp;
					vacDutyPeriod.hasSip = false;
                    vacDutyPeriod.RefDutPdSeqNum = dp.DutPerSeqNum;

					if (IsDutyPeriodInVacationPeriod(dp, tripDate, tripType, trip.PairLength))
					{
						// inputs are: legZero = no split dutPd, first type for flights, second type for flights, type for Trip
						CreateFltsAndMakeDp(vacDutyPeriod, dp, legZero, string.Empty, "VA", "VA", tripType, trip.PairLength);
					}
					else
					{
						if (isSplit || isPrevVD || trip.ReserveTrip)
						{
							// can only be VDF or VDB
							CreateFltsAndMakeDp(vacDutyPeriod, dp, legZero, string.Empty, "VD", "VD", tripType, trip.PairLength);
							isPrevVD = true;
						}
						// added !is2ndDp check to stop using SIP in 2nd DP before vac or after vac
						//else if (GetSip(dp, ref sipLeg, GlobalSettings.CurrentBidDetails.Domicile, reverse, trip.PairLength, tripType))
						else if (GetSip(dp, ref sipLeg, GlobalSettings.CurrentBidDetails.Domicile, reverse, trip.PairLength, tripType, dpDate) && !is2ndDp)
						{
							// can only be VDVOF or VOVDB
							vacDutyPeriod.hasSip = true;
							vacTrip.HasSip = true;
							CreateFltsAndMakeDp(vacDutyPeriod, dp, sipLeg, flts1st, flts2nd, "Split", tripType, trip.PairLength);

							//If the last flight in the trip is a sip ,currently duty period is showing as "Split" and all its flight type showing as "VO"... for solving
							int vOCount = vacDutyPeriod.FlightDetails.Where(x => x.VacationType == "VO").Count();
							if (vOCount == vacDutyPeriod.FlightDetails.Count)
							{
								vacDutyPeriod.VacationType = "VO";
							}

							vacTrip.VDvsRCdata.RCcalcDutyPeriod = dp.DutPerSeqNum;
							// for VOF this means we arrive domicile on this flight
							// for VOB this means we depart domicile on this flight
							vacTrip.VDvsRCdata.RCcalcFlightSeqNum = sipLeg;
							vacTrip.VDvsRCdata.isSplit = true;
							isSplit = true;
						}
						else if (is2ndDp)
						{
							// can only be VDF or VDB  (Frank add 4/12/2015, this is not true, can be split if cost comparison is in company favor)
							CreateFltsAndMakeDp(vacDutyPeriod, dp, legZero, string.Empty, "VD", "VD", tripType, trip.PairLength);

							if (isPrevVO)
							{
								// this only occurs if Trip is split between dutyPeriods
								if (tripType == "VOF")
								{
									vacTrip.VDvsRCdata.RCcalcDutyPeriod = dp.DutPerSeqNum;
									vacTrip.VDvsRCdata.RCcalcFlightSeqNum = 9;  // means end of day is split
								}
								if (tripType == "VOB")
								{
									vacTrip.VDvsRCdata.RCcalcDutyPeriod = dp.DutPerSeqNum;
									vacTrip.VDvsRCdata.RCcalcFlightSeqNum = 0;  // means beginning of day is split
								}
							}
							isPrevVD = true;
						}
						//else if (pltDh(directionOfDh, ref dhLeg, GlobalSettings.CurrentBidDetails.Domicile, dp, reverse, tripDate))
						else if (pltDh(directionOfDh, ref dhLeg, GlobalSettings.CurrentBidDetails.Domicile, dp, reverse, dpDate, trip.PairLength, trip, vacTrip))
						{
							// can only be VDVOF or VOVDB
							CreateFltsAndMakeDp(vacDutyPeriod, dp, dhLeg, flts1st, flts2nd, "Split", tripType, trip.PairLength);

							vacTrip.VDvsRCdata.RCcalcDutyPeriod = dp.DutPerSeqNum;
							vacTrip.VDvsRCdata.RCcalcFlightSeqNum = dhLeg;
							vacTrip.VDvsRCdata.isSplit = true;
							isSplit = true;
						}

						else
						{
							// can only be VOF or VOB
							CreateFltsAndMakeDp(vacDutyPeriod, dp, legZero, string.Empty, "VO", "VO", tripType, trip.PairLength);
							is2ndDp = true;
							// this flag helps when setting split parameters between VO and VD dutyPeriods
							isPrevVO = true;
						}
					}

					vacTrip.DutyPeriodsDetails.Add(vacDutyPeriod);
					dpDate = reverse == Reverse.No ? dpDate.AddDays(1) : dpDate.AddDays(-1);
				}
				if (reverse == Reverse.Yes)
					vacTrip.DutyPeriodsDetails.Reverse();

				vacTrip.RigAdg = trip.RigAdg;
				vacTrip.TripName = trip.TripNum;
				vacTrip.RigTafb = trip.RigTafb;


				DistributeRig(vacTrip);
				int hasSipCount = vacTrip.DutyPeriodsDetails.Where(x => x.hasSip == true).Count();
				// If SIP before 1200 and 1800, I need to look at best financial case.

				// we don't check VDvsRC with a SIP
				// if (hasSipCount == 0)
				if (hasSipCount == 0 && !trip.ReserveTrip)
					CalcVDvsRC(vacTrip, trip, tripDate);
				else if (hasSipCount > 0)
				{
					// gather other cost data on the sip duty period.
				}
				distributeAdgTafbRig(vacTrip);
				//Calculating Grand toal 
				vacTrip.VacationGrandTotal = GetGrandTotal(vacTrip);


			}
			catch (Exception ex)
			{

				throw ex;
			}

			return vacTrip;
		}
		private void distributeAdgTafbRig(VacationTrip vacTrip)
		{
			if (vacTrip.DutyPeriodsDetails.SelectMany(x => x.FlightDetails).Any(y => y.VacationType == "VD"))
			{
				DistributeAdgTafbRigToLowestDutyperiod(vacTrip);
			}
			else
			{
				DistributeAdgTafbRigToLastDuperiod(vacTrip);
			}
		}
			private void CreateFltsAndMakeDp_Va(DutyPeriod dp, VacationDutyPeriod vacDutyPeriod)
		{
			try
			{
				vacDutyPeriod.VacationDetails = new VacationDetails();
				vacDutyPeriod.FlightDetails = new List<VacationFlight>();
				vacDutyPeriod.VacationDetails.VacationType = "VA";
				vacDutyPeriod.RefDutPdSeqNum = dp.DutPerSeqNum;

				foreach (var flt in dp.Flights)
				{
					VacationFlight vacFlt = new VacationFlight();
					vacFlt.VacationDetail = new VacationDetails();
					vacFlt.VacationDetail.VacationType = "VA";
					vacFlt.RefFltSeqNum = flt.FlightSeqNum;

					vacDutyPeriod.FlightDetails.Add(vacFlt);
				}
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}

		private void CreateFltsAndMakeDp(VacationDutyPeriod vacDutyPeriod, DutyPeriod dp, int legNum, string firstType,
			string secondType, string dutPdType, string tripType, int tripLength)
		{
			try
			{
				// only VOF flights are processed in reverse
				int legNumber = 0;

				switch (tripType)
				{
				case "VOB":
					//legNumber =legNum;
					legNumber = (dutPdType == "Split") ? (legNum - 1) : legNum;
					break;
				case "VOF":
					legNumber = legNum;
					//Need to confirm with frank before changing
					//legNumber = (dutPdType == "Split") ? (legNum - 1) : legNum;
					break;
				case "VA":
					legNumber = legNum;
					break;
				}


				Reverse reverse = tripType == "VOF" ? Reverse.Yes : Reverse.No;
				List<Flight> flights = null;
				if (reverse == Reverse.Yes)
					flights = dp.Flights.Reverse<Flight>().ToList();
				else
				{
					flights = dp.Flights.ToList();
					// legNum= legNum - 1;
				}


				int timeVD = 0;
				int timeVO = 0;
				foreach (var flt in flights)
				{
					VacationFlight vacFlt = new VacationFlight();
					vacFlt.VacationDetail = new VacationDetails();

					vacFlt.FlightNumber = flt.FltNum.ToString();
					vacFlt.RefFltSeqNum = flt.FlightSeqNum;

					if (flt.FlightSeqNum <= legNumber)
					{ //vacFlt.VacationDetail.VacationType = firstType;
						vacFlt.VacationType = firstType;
						// time = flt.ArrTime;
					}
					else
						//vacFlt.VacationDetail.VacationType = secondType;
						vacFlt.VacationType = secondType;

					if (dutPdType == "Split" && tripType == "VOF")
					{
						if (timeVD == 0 && vacFlt.VacationType == "VD")
						{
							timeVD = flt.ArrTime;
						}
						if (vacFlt.VacationType == "VO")
						{
							timeVO = flt.DepTime;
						}
					}
					else if (dutPdType == "Split" && tripType == "VOB")
					{
						if (timeVD == 0 && vacFlt.VacationType == "VD")
						{
							timeVD = flt.DepTime;
						}
						if (vacFlt.VacationType == "VO")
						{
							timeVO = flt.ArrTime;
						}
					}

					//assignFltTfpAndBlock(vacFlt, flt, inBidPeriod);
					vacFlt.Tfp = flt.Tfp;
					vacFlt.Block = flt.Block;

					vacDutyPeriod.FlightDetails.Add(vacFlt);
				}
				if (vacDutyPeriod.FlightDetails.Where(x => x.VacationType == "VO").Count() == vacDutyPeriod.FlightDetails.Count)
				{
					dutPdType = "VO";
				}
				if (vacDutyPeriod.FlightDetails.Where(x => x.VacationType == "VD").Count() == vacDutyPeriod.FlightDetails.Count)
				{
					dutPdType = "VD";
				}

				//vacDutyPeriod.VacationDetails.VacationType = dutPdType;
				vacDutyPeriod.VacationType = dutPdType;

				vacDutyPeriod.RigDpMin = dp.RigDailyMin;
				vacDutyPeriod.RigDhr = dp.RigDhr;

				//Calculate Tafb
				//-----------------------------------------------------
				if (dutPdType == "Split")
				{
					if (tripType == "VOF")
					{
						//time= vacDutyPeriod.FlightDetails.FirstOrDefault(x=>x.VacationType=="VD").fli
						vacDutyPeriod.Tafb_VD = CalculateTafbInBpOfSplitDutyPeriod(dp, tripLength, true, timeVD + GlobalSettings.debrief);
						vacDutyPeriod.Tafb_VO = CalculateTafbInBpOfSplitDutyPeriod(dp, tripLength, false, timeVO);
					}
					else if (tripType == "VOB")
					{
						vacDutyPeriod.Tafb_VO = CalculateTafbInBpOfSplitDutyPeriod(dp, tripLength, true, timeVO + GlobalSettings.debrief);
						vacDutyPeriod.Tafb_VD = CalculateTafbInBpOfSplitDutyPeriod(dp, tripLength, false, timeVD);
					}

				}
				else
				{
					if (dutPdType == "VA")
						vacDutyPeriod.Tafb_VA = CalculateTafbInBpOfDutyPeriod(dp, tripLength);
					else if (dutPdType == "VO")
						vacDutyPeriod.Tafb_VO = CalculateTafbInBpOfDutyPeriod(dp, tripLength);
					else if (dutPdType == "VD")
						vacDutyPeriod.Tafb_VD = CalculateTafbInBpOfDutyPeriod(dp, tripLength);
				}
				//-----------------------------------------------------------------------------------


				if (reverse == Reverse.Yes)
				{
					vacDutyPeriod.FlightDetails.Reverse();

				}
			}
			catch (Exception ex)
			{

				throw ex;
			}
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
						// dutyPeriodTafb += 24 * 60 - (dutyPeriod.DepTimeFirstLeg - GlobalSettings.show1stDay);
						dutyPeriodTafb += 24 * 60 - (dutyPeriod.ShowTime);
					else
						// not first or last day of trip, so there are 24 hours of Tafb
						dutyPeriodTafb += 24 * 60;
				}
				else if (lengthOfTrip > 1)
					// last day of multi day trip
					// dutyPeriodTafb += dutyPeriod.LandTimeLastLeg % 1440 + GlobalSettings.debrief;
					dutyPeriodTafb += dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.debrief;
				else
					// trip is a 1 day trip
					//dutyPeriodTafb += dutyPeriod.LandTimeLastLeg % 1440 - (dutyPeriod.DepTimeFirstLeg % 1440 - GlobalSettings.show1stDay) + GlobalSettings.debrief;
					// dutyPeriodTafb += (dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1))) - (dutyPeriod.DepTimeFirstLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) - GlobalSettings.show1stDay) + GlobalSettings.debrief;
					dutyPeriodTafb += (dutyPeriod.LandTimeLastLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1))) - (dutyPeriod.ShowTime - (1440 * (dutyPeriod.DutPerSeqNum - 1))) + GlobalSettings.debrief;

				//dutyPeriodTafb = dutyPeriod.DutyBreak;

			}
			catch (Exception ex)
			{

				throw ex;
			}
			return dutyPeriodTafb;
		}

		/// <summary>
		/// Calculate TafbInBp Of Split DutyPeriod
		/// </summary>
		/// <param name="dutyPeriod"></param>
		/// <param name="lengthOfTrip"></param>
		/// <param name="isFirstType"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		private int CalculateTafbInBpOfSplitDutyPeriod(DutyPeriod dutyPeriod, int lengthOfTrip, bool isFirstType, int time)
		{
			time = time % 1440;

			int dutyPeriodTafb = 0;
			try
			{
				if (isFirstType)
				{

					if (dutyPeriod.DutPerSeqNum == 1)
						// first day of trip and trip is more than 1 day
						//  dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg % 1440 + GlobalSettings.show1stDay;
						//  dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.show1stDay;
						dutyPeriodTafb += time - dutyPeriod.ShowTime - (1440 * (dutyPeriod.DutPerSeqNum - 1));
					else
						dutyPeriodTafb += time;

				}
				else
				{

					if (lengthOfTrip > 1)
						// last day of multi day trip
						dutyPeriodTafb += 1440 - time;
					else
						// trip is a 1 day trip
						//dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg % 1440 + GlobalSettings.show1stDay + GlobalSettings.debrief;
						// dutyPeriodTafb += time - dutyPeriod.DepTimeFirstLeg - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.show1stDay + GlobalSettings.debrief;
						dutyPeriodTafb += time - dutyPeriod.ShowTime - (1440 * (dutyPeriod.DutPerSeqNum - 1)) + GlobalSettings.debrief;
				}


			}
			catch (Exception ex)
			{

				throw ex;
			}
			return dutyPeriodTafb;
		}

		private void CreateVacTripObjects(VacationTrip vacTrip)
		{
			vacTrip.VacationDetails = new VacationDetails();
			vacTrip.DutyPeriodsDetails = new List<VacationDutyPeriod>();
			vacTrip.VacationRig = new VacationRig();
			vacTrip.VacationGrandTotal = new VacationGrandTotal();
			vacTrip.VDvsRCdata = new VDvsRCdata();
			vacTrip.RCtimes = new RCtimes();
		}

		private VacationTrip CreateVacationTripObject(VacationTrip vacationTrip)
		{
			vacationTrip = new VacationTrip();
			vacationTrip.DutyPeriodsDetails = new List<VacationDutyPeriod>();
			vacationTrip.VacationDetails = new VacationDetails();
			vacationTrip.VacationGrandTotal = new VacationGrandTotal();
			vacationTrip.VacationRig = new VacationRig();
			return vacationTrip;
		}

		private void CreateVacDutyPeriodObjects(VacationDutyPeriod vacDutyPeriod)
		{
			vacDutyPeriod.VacationDetails = new VacationDetails();
			vacDutyPeriod.FlightDetails = new List<VacationFlight>();
			vacDutyPeriod.VacationRig = new VacationRig();
		}

		private VacationGrandTotal GetGrandTotal(VacationTrip vacTrip)
		{

			VacationGrandTotal vacGrandTot = new VacationGrandTotal();
			try
			{


				// Tfp inLine
				vacGrandTot.VA_TfpInLineTot = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VA").Sum(y => y.Tfp));
				vacGrandTot.VO_TfpInLineTot = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VO").Sum(y => y.Tfp));
				vacGrandTot.VD_TfpInLineTot = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VD").Sum(y => y.Tfp));

				// Block inLine
				vacGrandTot.VA_BlkInLineTot = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VA").Sum(y => y.Block));
				vacGrandTot.VO_BlkInLineTot = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VO").Sum(y => y.Block));
				vacGrandTot.VD_BlkInLineTot = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VD").Sum(y => y.Block));


				// Tafb inLine
				vacGrandTot.VA_TafbInLineTot = vacTrip.DutyPeriodsDetails.Sum(y => y.Tafb_VA);
				vacGrandTot.VO_TafbInLineTot = vacTrip.DutyPeriodsDetails.Sum(y => y.Tafb_VO);
				vacGrandTot.VD_TafbInLineTot = vacTrip.DutyPeriodsDetails.Sum(y => y.Tafb_VD);

				// Tfp inBp
				vacGrandTot.VA_TfpInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(x => x.FlightDetails.Where(y => y.VacationType == "VA").Sum(y => y.Tfp));
				vacGrandTot.VO_TfpInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(x => x.FlightDetails.Where(y => y.VacationType == "VO").Sum(y => y.Tfp));
				vacGrandTot.VD_TfpInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(x => x.FlightDetails.Where(y => y.VacationType == "VD").Sum(y => y.Tfp));

				// Block inBp
				vacGrandTot.VA_BlkInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(x => x.FlightDetails.Where(y => y.VacationType == "VA").Sum(y => y.Block));
				vacGrandTot.VO_BlkInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(x => x.FlightDetails.Where(y => y.VacationType == "VO").Sum(y => y.Block));
				vacGrandTot.VD_BlkInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(x => x.FlightDetails.Where(y => y.VacationType == "VD").Sum(y => y.Block));

				// Tafb inBp
				vacGrandTot.VA_TafbInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(y => y.Tafb_VA);
				vacGrandTot.VO_TafbInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(y => y.Tafb_VO);
				vacGrandTot.VD_TafbInBpTot = vacTrip.DutyPeriodsDetails.Where(x => x.isInBp).Sum(y => y.Tafb_VD);

				// Rig


				//In Bp
				vacGrandTot.VA_TfpInBpTot += vacTrip.RigVAVO_Adg + vacTrip.RigVAVO_Tafb;
				vacGrandTot.VA_TfpInBpTot += vacTrip.DutyPeriodsDetails.Where(y => y.isInBp).Sum(x => x.RigDpMin_VA);
				vacGrandTot.VA_TfpInBpTot += vacTrip.DutyPeriodsDetails.Where(y => y.isInBp).Sum(x => x.RigDhr_VA);

				vacGrandTot.VD_TfpInBpTot += vacTrip.RigVD_Adg + vacTrip.RigVD_Tafb;
				vacGrandTot.VD_TfpInBpTot += vacTrip.DutyPeriodsDetails.Where(y => y.isInBp).Sum(x => x.RigDpMin_VD);
				vacGrandTot.VD_TfpInBpTot += vacTrip.DutyPeriodsDetails.Where(y => y.isInBp).Sum(x => x.RigDhr_VD);

				//vacGrandTot.VO_TfpInLineTot += vaTrip.RigVAVO_Tafb + vaTrip.RigVAVO_Tafb;    this rig is already added to VacTfpInLineTot
				vacGrandTot.VO_TfpInBpTot += vacTrip.DutyPeriodsDetails.Where(y => y.isInBp).Sum(x => x.RigDpMin_VO);
				vacGrandTot.VO_TfpInBpTot += vacTrip.DutyPeriodsDetails.Where(y => y.isInBp).Sum(x => x.RigDhr_VO);


				// inLine
				vacGrandTot.VA_TfpInLineTot += vacTrip.RigVAVO_Adg + vacTrip.RigVAVO_Tafb;
				vacGrandTot.VA_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigDpMin_VA);
				vacGrandTot.VA_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigDhr_VA);

				vacGrandTot.VD_TfpInLineTot += vacTrip.RigVD_Adg + vacTrip.RigVD_Tafb;
				vacGrandTot.VD_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigDpMin_VD);
				vacGrandTot.VD_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigDhr_VD);

                //Added by Roshil on 4-12-2019 (Need to check the rig values or total vacation values are correct after adding this)
                vacGrandTot.VA_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigAdg_VA);
                vacGrandTot.VA_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigTafb_VA);

                vacGrandTot.VD_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigAdg_VD);
                vacGrandTot.VD_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigTafb_VD);

                vacGrandTot.VO_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigAdg_VO);
                vacGrandTot.VO_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigTafb_VO);
                //-- In BP
                vacGrandTot.VA_TfpInBpTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigAdg_VA);
                vacGrandTot.VA_TfpInBpTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigTafb_VA);

                vacGrandTot.VD_TfpInBpTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigAdg_VD);
                vacGrandTot.VD_TfpInBpTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigTafb_VD);

                vacGrandTot.VO_TfpInBpTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigAdg_VO);
                vacGrandTot.VO_TfpInBpTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigTafb_VO);
                //End- Added by Roshil on 4-12-2019

				//vacGrandTot.VO_TfpInLineTot += vaTrip.RigVAVO_Tafb + vaTrip.RigVAVO_Tafb;    this rig is already added to VacTfpInLineTot
				vacGrandTot.VO_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigDpMin_VO);
				vacGrandTot.VO_TfpInLineTot += vacTrip.DutyPeriodsDetails.Sum(x => x.RigDhr_VO);





				// Rig
				// add inBp

			}
			catch (Exception ex)
			{

				throw ex;
			}

			return vacGrandTot;
		}


		private bool GetSip(DutyPeriod dp, ref int sipLeg, string domicile, Reverse reverse, int tripLength, string tripType,DateTime dpDate)
		{
			List<Flight> flights = null;

			if (reverse == Reverse.Yes)
				flights = dp.Flights.Reverse<Flight>().ToList();
			else
				flights = dp.Flights;

			foreach (var flt in flights)
			{
				// for VOF we look to see if we arrive at a domicile in the dutyPeriod 
				if (tripType == "VOF")
				{
					// special case: duty period arriving after midnight on day before vacation
					if (dp.ArrivesAfterMidnightDayBeforeVac && flt.FlightSeqNum == flights.Count)
					{
						continue;
					}

					if (flt.ArrSta == domicile)
					{
						// check for contractual requirments of SIP 
						// for domicile report equal to or before 1100 Dom time then SIP must pass through DOM after 1200

						// add case statement for all domiciles with a default

						int domicileMinutesDifference = 0;

						switch (domicile)
						{
							case "ATL":
							case "BWI":
							case "MCO":
								domicileMinutesDifference = 60;
								break;
							case "MDW":
							case "DAL":
							case "HOU":
								domicileMinutesDifference = 0;
								break;
							case "DEN":
								domicileMinutesDifference = -60;
								break;
							case "LAS":
							case "LAX":
							case "OAK":
								domicileMinutesDifference = -120;
								break;
							case "PHX":
								TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Denver");
								TimeSpan ts = new TimeSpan(12, 0, 0);
								// Insures that time dateTime is not 12:00AM but 12:00pm
								// Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.
								dpDate = dpDate.Date + ts;
								bool isDst = timeZoneInfo.IsDaylightSavingTime(dpDate);
								if (isDst)
									domicileMinutesDifference = -120;
								else
									domicileMinutesDifference = -60;
								// the date for the duty period needs to be passed in to this method
								// I need to check the date for for Daylight Savings Time for PHX
								// during DST the PHX time difference is -120, when normal time the PHX time difference is -60
								break;

						}

                        //if ((dp.ShowTime % 1440 + domicileMinutesDifference) <= 660 && (flt.ArrTime % 1440 + domicileMinutesDifference) > 720)
                        //{
                        //	sipLeg = flt.FlightSeqNum;
                        //	return true;
                        //}
                        //else if ((dp.ShowTime % 1440 + domicileMinutesDifference) > 660 && (AfterMidnightLandingTime(flt.ArrTime % 1440) + domicileMinutesDifference) > 1080)
                        //{
                        //	sipLeg = flt.FlightSeqNum;
                        //	return true;
                        //}

                        // original code  **************************

                        sipLeg = flt.FlightSeqNum;
                        return true;

                        // orignal code above **********************
                    }
				}
				// for VOB we look to see if we depart a domicile in the dutyPeriod
				if (tripType == "VOB")
				{
					if (flt.DepSta == domicile)
					{
						sipLeg = flt.FlightSeqNum;
						return true;
					}
				}
			}
			return false;
		}


		private bool IsDutyPeriodInVacationPeriod(DutyPeriod dp, DateTime tripDate, string tripType, int tripLength)
		{
			//  This method is entered only if Trip is VOF or VOB


			//DayOfWeek dow = tripDate.AddDays(dp.DutPerSeqNum - 1).DayOfWeek;
			//if (dow == DayOfWeek.Sunday || dow == DayOfWeek.Monday || dow == DayOfWeek.Tuesday)
			//{
			//    if (tripType == "VOF")
			//        return true;
			//    else                        // VOB
			//        return false;
			//}
			//else                            // dow == Thu, Fri, or Sat
			//{
			//    if (tripType == "VOF")
			//        return false;
			//    else                        // VOB
			//        return true;
			//}
			bool status = false;
			DateTime dutyperiodDate = tripDate.AddDays(dp.DutPerSeqNum - 1);


			if (tripType == "VOF" && tripDate.AddDays(tripLength - 1) >= WBidCollection.GetnextSunday())
			{

				status = (dutyperiodDate >= WBidCollection.GetnextSunday());
			}
			else
				if (GlobalSettings.OrderedVacationDays != null && GlobalSettings.OrderedVacationDays.Any(x => x.AbsenceType == "VA" && (dutyperiodDate >= x.StartAbsenceDate && dutyperiodDate <= x.EndAbsenceDate)))
				{
					status = true;

				}

			return status;
		}

		private bool IsDutyPeriodInBidPeriod(DateTime tripStartDate, int dutyPdSeqNum)
		{
			DateTime dpDate = tripStartDate.AddDays(dutyPdSeqNum - 1);
			if (dpDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
				return true;
			else
				return false;
		}


		private bool pltDh(DeadHead dh, ref int dhLeg, string domicile, DutyPeriod dp, Reverse reverse, DateTime dutPdDate, int tripLength, Trip trip, VacationTrip vacTrip)
		{
			bool test = false;
			if (trip.TripNum.Contains("LALD"))
			{
				test = true;
			}
			List<Flight> flights = null;
			if (reverse == Reverse.Yes)
				flights = dp.Flights.Reverse<Flight>().ToList();
			else
				flights = dp.Flights;

			if (DeadHead.toDomicile == dh)  // VOF trip
			{
				foreach (var flt in flights)
				{
					if (pltDhToDomVOF(flt, dutPdDate, dp.ShowTime, dp, vacTrip))
					{
						dhLeg = flt.FlightSeqNum;
						return true;
					}
				}
				dhLeg = 0;
				return false;
			}
			else                            // VOB trip
			{
				foreach (var flt in flights)
				{
					if (!(tripLength == dp.DutPerSeqNum && flt.FlightSeqNum == flights.Count) && pltDhToRejoinTripVOB(flt, dutPdDate, dp.ReleaseTime, dp, vacTrip))
					{
						dhLeg = flt.FlightSeqNum;
						return true;
					}
				}
				dhLeg = 0;
				return false;
			}
		}

		private bool pltDhToDomVOF(Flight flt, DateTime dutPdDate, int showTime, DutyPeriod dp, VacationTrip vacTrip)
		{
            //Frank Add 7/21/2019 DH back after first flight of first dutyperiod never happen
            if (flt.FlightSeqNum == 1 && dp.DutPerSeqNum == 1) return false;
			// VOF Trip
			string domicile = GlobalSettings.CurrentBidDetails.Domicile;
			int returnTime = GetReturnTimeVof(showTime % 1440, dutPdDate);
			//int depTimeMinutes = flt.ArrTime % 1440 + GlobalSettings.connectTime;
			// expanded flight range by subtracting connect time.  We will further filter in CheckPilotDeadhead with routeNum
			int depTimeMinutes = flt.ArrTime % 1440;
			if (depTimeMinutes < GlobalSettings.EarliestTakeOffMinutes)
				return false;

			int lastLandTimeInHhMM = GetLatestLandTime(flt, dp, dutPdDate, domicile, showTime, vacTrip);

			//int tempToMinutes = lastLandTimeInHhMM / 100 * 60;
			//tempToMinutes = tempToMinutes + lastLandTimeInHhMM % 100;

			// convert to domicile time
			//lastLandTimeInHhMM = WBidHelper.ConvertMinutesToHhhmmInt(WBidHelper.ConvertToHerbFromDomTime(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate, tempToMinutes));
			bool deadHeadToDom = true;      //VOF
			List<DeadHeadResult> pltDh = new List<DeadHeadResult>();

			DeadHeadParameter dhParam = new DeadHeadParameter();
			dhParam.DepSta = flt.ArrSta;
			dhParam.ArrSta = domicile;
			//frank added 2/5/2018 //this accomadates dutyperiods that are get back after the contract time of t1700 0r 2400 and no dutyperiod exists next day 
			if (dhParam.DepSta == dhParam.ArrSta)
			{
				return true; 
			}
			dhParam.Date = dutPdDate;
			dhParam.ConnectTime = GlobalSettings.connectTime;
			dhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(depTimeMinutes);
			dhParam.ArrTimeHhMm = lastLandTimeInHhMM;

			List<DeadHeadResult> lstDeadHead = _networkPlanData.CheckPilotDeadhead(dhParam, flt, deadHeadToDom, ref pltDh);

			vacTrip.VDvsRCdata.pltDh = pltDh;

			if (lstDeadHead.Count > 0)
				return true;
			else
				return false;
		}

		private bool pltDhToRejoinTripVOB(Flight flt, DateTime dutPdDate, int releaseTime, DutyPeriod dp, VacationTrip vacTrip)
		{
			// VOB Trip
			string domicile = GlobalSettings.CurrentBidDetails.Domicile;
			//if (releaseTime % 1440 >= 0 && releaseTime % 1440 <= 180)
			//    releaseTime = releaseTime % 1440 + 1440;
			//else
			//    releaseTime = releaseTime % 1440;
			//int earliestTakeOff = WBidHelper.ConvertMinutesToHhhmmInt(getReportTimeVob(releaseTime, dp, dutPdDate));
			int earliestTakeOffInHhMm = GetEarliestTakeOffTimeInHhMm(flt, dp, dutPdDate, domicile);
			bool deadHeadToDom = false;
			List<DeadHeadResult> pltDh = new List<DeadHeadResult>();

			DeadHeadParameter dhParam = new DeadHeadParameter();
			dhParam.DepSta = domicile;
			dhParam.ArrSta = flt.DepSta;
			dhParam.Date = dutPdDate;
			dhParam.ConnectTime = GlobalSettings.connectTime;
			dhParam.DepTimeHhMm = earliestTakeOffInHhMm;
            dhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(flt.DepTime % 1440));

			List<DeadHeadResult> lstDeadHead = _networkPlanData.CheckPilotDeadhead(dhParam, flt, deadHeadToDom, ref pltDh);

			vacTrip.VDvsRCdata.pltDh = pltDh;

			if (lstDeadHead.Count > 0)
				return true;
			else
				return false;
		}


        private void CalcVDvsRC(VacationTrip vacTrip, Trip trip, DateTime tripDate)
        {
            // frank debug
            int i = 1;
            if (trip.TripNum == "CALR")
                i = 0;

            bool furtherEval = false;
            bool noVD = true;
            int vdCount = vacTrip.DutyPeriodsDetails.SelectMany(x => x.FlightDetails).Where(y => y.VacationType == "VD").Count();
            noVD = vdCount == 0 ? true : false;

            if (noVD)
                return;

            decimal totVD = 0m;
            decimal reserveCost = 0m;

            string depSta = string.Empty;
            string arrSta = string.Empty;
            int arrTimeHhMm = 0;
            int depTimeHhMm = 0;
            Flight flight = new Flight();

            DateTime rcDpDate = tripDate.AddDays(vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1);
            bool rcWasZero = false;
            // This should be simple have SetStationAndTimeDataForRC return a Deadhead Parameter
            // The input should be: Vof/Vob, 0/9, fltSeq, DpSeq, cnt=fltSeq, RCwasZero, trip, vacTrip
            DeadHeadParameter resDh = getResDhParam(trip, vacTrip, tripDate, rcWasZero, 0, false);  // added two params furtherEval DH leg, and furtherEval boolean

            // SetStationAndTimeDataForRC(vacTrip, ref depSta, ref arrSta, ref arrTimeHhMm, ref depTimeHhMm, ref rcDpDate, trip, tripDate, rcWasZero, ref flight);

            List<RouteDomain> resRteDomain = new List<RouteDomain>();

            reserveCost = _networkPlanData.CheckCostOfReserveDH(resDh, flight, vacTrip, trip, tripDate, rcWasZero, ref resRteDomain);

            vacTrip.VDvsRCdata.resDh = resRteDomain;

            if (reserveCost == 0m)  // means it was not possible to dh the resere that day, thus now the reserve dh's a day early (VOF) or a day late (VOB)
            {
                rcWasZero = true;
                // SetStationAndTimeDataForRC(vacTrip, ref depSta, ref arrSta, ref arrTimeHhMm, ref depTimeHhMm, ref rcDpDate, trip, tripDate, rcWasZero, ref flight);

                resDh = getResDhParam(trip, vacTrip, tripDate, rcWasZero, 0, false);  // added two params furtherEval DH leg, and furtherEval boolean

                reserveCost = _networkPlanData.CheckCostOfReserveDH(resDh, flight, vacTrip, trip, tripDate, rcWasZero, ref resRteDomain);

                // frank change 12/7/2014:  reserve cost is min of 5.0 on day before or day after
                // reserveCost = Math.Max(5.0m, reserveCost); 
                // causing too many problems with the above dpm inclusion


                vacTrip.VDvsRCdata.resDh = resRteDomain;

                if (GlobalSettings.CurrentBidDetails.Postion == "CP")
                    reserveCost += GlobalSettings.HotelCostCP;
                else
                    reserveCost += GlobalSettings.HotelCostFO;

            }

            totVD = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VD").Sum(y => y.Tfp));
            // need to add rig here
            totVD += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split").Sum(y => y.RigDpMin_VD);
            totVD += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split").Sum(y => y.RigTafb_VD);
            totVD += vacTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split").Sum(y => y.RigDhr_VD);

            vacTrip.VDvsRCdata.TotVDCost = totVD;
            vacTrip.VDvsRCdata.TotRCCost = reserveCost;
            // if doing test for all combinations, then add field for additional VO after 1st split
            if (trip.TripNum == "CALR")
            {
                i = 1;
            }
            //if (totVD - reserveCost <= GlobalSettings.VDvsRCtfpFactor)
            if (totVD - reserveCost < GlobalSettings.VDvsRCtfpFactor)
            {
                var totVODutPds = vacTrip.DutyPeriodsDetails.Where(dp => dp.VacationType == "VO").Count();
                int legForVD = 0;
                furtherEval = true;
                if (furtherEvaluateCost(vacTrip, trip, tripDate, totVODutPds, currentLine, ref legForVD, totVD, reserveCost, furtherEval))
                {
                    // legForVD indicates the last VD leg in the duty period
                }
                else
                {
                    // since there is NO cost savings in the first or last duty period, the ConverVD to VO
                    ConvertVD_DpsAndFltsTo_VO(vacTrip);

                }


                // frank added 7/14/2018

                // if the trip has two VO-VO duty periods before VA or two VO-VO duty periods after VA, the check for deadheads
                // -- for sure if the first flight of the second duty period before VA is DH then change VO-VO to VD-VO
                // -- for sure if the last flight of the of the first VO period after VA is DH then change VO-VO to VO-VD
            }
            // This code works for both 3 and 4 day trips with VO-VO before VA. One and two day trips are not considered
            if (vacTrip.DutyPeriodsDetails.Count >= 3)
            {
                // 4 Day trips cannot have VO-VO-VO-VA, so no need to check
                if (vacTrip.DutyPeriodsDetails[0].VacationType == "VO" && vacTrip.DutyPeriodsDetails[1].VacationType == "VO")
                {
                    // looking for DH on 1st flight of second VO period before VA
                    if (trip.DutyPeriods[1].Flights[0].DeadHead == true)
                    {
                        // change 1st duty period and all flights to VD
                        changeDutyPeriodFromVOtoVD(vacTrip.DutyPeriodsDetails[0]);
                    }

                }
            }

            // This code works for both 3 and 4 day trips with VO-VO after VA
            int dutPds = vacTrip.DutyPeriodsDetails.Count;
            if (dutPds >= 3)
            {
                if (vacTrip.DutyPeriodsDetails[dutPds - 2].VacationType == "VO" && vacTrip.DutyPeriodsDetails[dutPds - 1].VacationType == "VO")
                {

                    // looking for DH on last flight of first VO period after VA
                    int lastFlight = trip.DutyPeriods[dutPds - 2].Flights.Count;
                    if (trip.DutyPeriods[dutPds - 2].Flights[lastFlight - 1].DeadHead == true)
                    {
                        // change last duty period and all flights to VD after VA
                        changeDutyPeriodFromVOtoVD(vacTrip.DutyPeriodsDetails[dutPds - 1]);
                    }
                }
            } // Frank comment 7-16-2018
              // Adg and Tafb rig is distributed to all duty periods if VacTrip is NOT all VA
              // if a duty period is split, the Adg and Tafb assigned the the split duty period is further distributed.
        }
        private void changeDutyPeriodFromVOtoVD(VacationDutyPeriod vacationDutyPeriod)
        {
            vacationDutyPeriod.VacationType = "VD";
            foreach (VacationFlight vacFlt in vacationDutyPeriod.FlightDetails)
            {
                vacFlt.VacationType = "VD";
                vacFlt.VacationDetail.VacationType = "VD";
                vacFlt.VacationDetail.VDfBlock = vacFlt.VacationDetail.VOfBlock;
                vacFlt.VacationDetail.VOfBlock = 0;
                vacFlt.VacationDetail.VDfTafb = vacFlt.VacationDetail.VOfTafb;
                vacFlt.VacationDetail.VOfTafb = 0;
                vacFlt.VacationDetail.VDfTfp = vacFlt.VacationDetail.VOfTfp;
                vacFlt.VacationDetail.VOfTfp = 0;
            }
            vacationDutyPeriod.RigDpMin_VD = vacationDutyPeriod.RigDpMin_VD + vacationDutyPeriod.RigDpMin_VO;
            vacationDutyPeriod.RigDpMin_VO = 0;
            vacationDutyPeriod.RigDhr_VD = vacationDutyPeriod.RigDhr_VD + vacationDutyPeriod.RigDhr_VO;
            vacationDutyPeriod.RigDhr_VO = 0;
        }
		private bool furtherEvaluateCost(VacationTrip vacTrip, Trip trip, DateTime tripDate, int totVODutPds, int currentLine, ref int legForVD, decimal totVD, decimal reserveCost, bool furtherEval)
		{
			// iterate through all legs in the first or last duty period and create a costSavings<LegPltSavingsVsResCost>
			// if there is a savings by splitting the duty period, return true and the leg in legForVd

			// frank add 4/12/2015
			// before assuming all of the is2ndDp is VD, we need to do a cost comparison on all flights
			// in reverse order look to see if plt can DH back from depSta and cost of reserve to get to depSta
			//     savings to company is total of all prev flts - cost to company is res cost to get to depSta
			//     if savings - rest cost > 0 then make all prev flts VD
			//     set "skipVDvsRC" = true flag.

			return false;

			bool rcWasZero = false; 
			LegPltSavingsVsResCost legPltsavingVsResCost = new LegPltSavingsVsResCost();
			List<LegPltSavingsVsResCost> savingsPerLeg = new List<LegPltSavingsVsResCost>();
			legPltsavingVsResCost.PltCost = totVD;
			legPltsavingVsResCost.ResCost = reserveCost;
			legPltsavingVsResCost.legSplit = 0;  // the 0 leg is the VDvsRC condition between duty periods
			savingsPerLeg.Add(legPltsavingVsResCost);

			if (vacTrip.VacationType == "VOF")
			{
				// the first duty period needs to be checked
				foreach (Flight flt in trip.DutyPeriods[0].Flights)
				{
					legPltsavingVsResCost = new LegPltSavingsVsResCost();
					legPltsavingVsResCost.legSplit = flt.FlightSeqNum;
					// check rescost and plt cost for flt
					legPltsavingVsResCost.PltCost = trip.DutyPeriods[0].Flights.Where(f => f.FlightSeqNum > flt.FlightSeqNum).Sum(f => f.Tfp);
					legPltsavingVsResCost.ResCost = getResCost(flt, trip, tripDate, vacTrip.VacationType, vacTrip, rcWasZero, furtherEval);
					if (legPltsavingVsResCost.ResCost == 0)
					{
						rcWasZero = true;
						legPltsavingVsResCost.ResCost = getResCost(flt, trip, tripDate, vacTrip.VacationType, vacTrip, rcWasZero, furtherEval);
					}
					savingsPerLeg.Add(legPltsavingVsResCost);

				}
			}
			else
			{
				// the last duty period needs to be checked
				foreach (Flight flt in trip.DutyPeriods[trip.DutyPeriods.Count() - 1].Flights)
				{
					legPltsavingVsResCost = new LegPltSavingsVsResCost();
					legPltsavingVsResCost.legSplit = flt.FlightSeqNum;
					// check rescost and plt cost for flt
					legPltsavingVsResCost.PltCost = trip.DutyPeriods[trip.DutyPeriods.Count() - 1].Flights.Where(f => f.FlightSeqNum > flt.FlightSeqNum).Sum(f => f.Tfp);
					if (legPltsavingVsResCost.ResCost == 0)
					{
						rcWasZero = true;
						legPltsavingVsResCost.ResCost = getResCost(flt, trip, tripDate, vacTrip.VacationType, vacTrip, rcWasZero, furtherEval);
					}
					savingsPerLeg.Add(legPltsavingVsResCost);
				}
			}

			//if (pltDh(directionOfDh, ref dhLeg, GlobalSettings.CurrentBidDetails.Domicile, dp, reverse, dpDate, trip.PairLength, trip, vacTrip))
			//{
			//    var compSavings = dp.Flights.Where(f => f.FlightSeqNum <= dhLeg).Sum(f => f.Tfp);

			//    // now get res cost

			//    bool rcWasZero = false;
			//    int releaseTimeForDutyPeriodinMinutes = dp.Flights[(dp.Flights.Count() - 1)].ArrTime + GlobalSettings.debrief;
			//    DeadHeadParameter resDh = new DeadHeadParameter();

			//    resDh.ArrSta = dp.Flights[dhLeg - 1].ArrSta;
			//    // this is not perfect, as I'm not checking same airplane and using generic connect time below
			//    resDh.ArrTimeHhMm = WBidHelper.ConvertMinutesToHhhmmInt(dp.Flights[dhLeg].DepTime - GlobalSettings.connectTime);
			//    resDh.ConnectTime = GlobalSettings.connectTime;
			//    resDh.Date = dpDate;
			//    resDh.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
			//    resDh.DepTimeHhMm = WBidHelper.ConvertMinutesToHhhmmInt(getReportOrReleaseInMinutesFromOther(releaseTimeForDutyPeriodinMinutes, dhLeg, dpDate, false));
			//    Flight flight = new Flight();

			//    VacationTrip newVacTrip = new VacationTrip();
			//    newVacTrip.VDvsRCdata = new VDvsRCdata();
			//    newVacTrip.VDvsRCdata.RCcalcDutyPeriod = dp.DutPerSeqNum;
			//    newVacTrip.VDvsRCdata.RCcalcFlightSeqNum = dhLeg;

			//    List<RouteDomain> resRteDomain = new List<RouteDomain>();
			//    var reserveCost = _networkPlanData.CheckCostOfReserveDH(resDh, flight, newVacTrip, trip, tripDate, rcWasZero, ref resRteDomain);

			//    var pilotSavings = dp.Flights.Where(f => f.FlightSeqNum < dhLeg).Sum(f => f.Tfp);

			//    if (pilotSavings > reserveCost)
			//    {
			//        bool makeVDF = true;
			//    }
			//}

			return false;
		}

		private decimal getResCost(Flight flt, Trip trip, DateTime tripDate, string VofVob, VacationTrip vacTrip, bool rcWasZero, bool furtherEval)
		{
			DeadHeadParameter resDh = new DeadHeadParameter();
			resDh = getResDhParam(trip, vacTrip, tripDate, rcWasZero, flt.FlightSeqNum, furtherEval);
			List<RouteDomain> resRteDomain = new List<RouteDomain>();

			return _networkPlanData.CheckCostOfReserveDH(resDh, flt, vacTrip, trip, tripDate, rcWasZero, ref resRteDomain);
		}
		private DeadHeadParameter getResDhParam(Trip trip, VacationTrip vacTrip, DateTime tripDate, bool rcWasZero, int furtherEvaluateDhLeg, bool furtherEval)
		{
			int ii = 0;
			if (trip.TripNum == "PAMX")
				ii = 1;

			DeadHeadParameter resDhParam = new DeadHeadParameter();

			bool isVof = vacTrip.VacationType == "VOF" ? true : false;
			bool vofSplit9 = (vacTrip.VDvsRCdata.RCcalcFlightSeqNum==9);
			bool vobSplit0 = (vacTrip.VDvsRCdata.RCcalcFlightSeqNum==0);
			int resDhDp = Math.Max(0, vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1);
			int resDhFltSeq=0;
			if (furtherEval)
			{
				resDhFltSeq = Math.Max(0, furtherEvaluateDhLeg-1);
			}
			else if (vacTrip.VDvsRCdata.RCcalcFlightSeqNum != 9)
			{
				resDhFltSeq = Math.Max(0, vacTrip.VDvsRCdata.RCcalcFlightSeqNum - 1);
			}
			else
			{
				//resDhFltSeq = trip.DutyPeriods[Math.Max(0, vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1)].Flights.Count - 1;
				// first leg next day
				resDhFltSeq = 0;
				resDhDp += 1;
			}
			DateTime today = tripDate.AddDays(resDhDp);
			DateTime tomorrow = today.AddDays(1);
			DateTime yesterday = today.AddDays(-1);
			string domicile = GlobalSettings.CurrentBidDetails.Domicile;
			DutyPeriod dp=trip.DutyPeriods[resDhDp];
			Flight flt=trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
			bool getReleaseTime = true;
			int legs = Math.Max(1, isVof ? resDhFltSeq : dp.Flights.Count);
			int depTime = 0;
			int arrTime = 0;
			int rptTime=0;

			resDhParam.DepSta = isVof ? domicile : trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].DepSta;

			if (!rcWasZero)
			{
				// or statement added to if to accomodate 1 leg days that are split, but Split is not set due to just 1 VO leg
				//if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split" || (isVof && vacTrip.VDvsRCdata.RCcalcFlightSeqNum !=9))
				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					// line 13 - OA89

					rptTime = getReportOrReleaseInMinutesFromOther(dp.ReleaseTime % 1440, 3, today, !getReleaseTime);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(rptTime % 1440 + GlobalSettings.show1stDay);
					resDhParam.ArrSta = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].ArrSta;
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					//arrTime = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].ArrTime; 
					arrTime = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq+1].DepTime;
					arrTime = AfterMidnightLandingTime(arrTime % 1440);
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(arrTime);
					//resDhParam.RefFlt = resDhFltSeq == 0 ? dp.Flights[0] : flt;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[vacTrip.VDvsRCdata.RCcalcFlightSeqNum];
					resDhParam.Date = today;
				}

				// this next case occurs when the duty periods are VD and VA, NOT split and NO VO
				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "VD")
				{
					// line 54 - OA87

					resDhParam.Date = tomorrow;
					resDhParam.RefFlt = trip.DutyPeriods[vacTrip.VDvsRCdata.RCcalcDutyPeriod-1].Flights[0];
					var refDutPd = trip.DutyPeriods[vacTrip.VDvsRCdata.RCcalcDutyPeriod-1];
					rptTime = getReportOrReleaseInMinutesFromOther(refDutPd.ReleaseTime % 1440, 3, today, !getReleaseTime);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(rptTime % 1440 + GlobalSettings.show1stDay);
					resDhParam.ArrSta = resDhParam.RefFlt.DepSta;
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					//arrTime = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].ArrTime; 
					arrTime = resDhParam.RefFlt.DepTime % 1440;
					arrTime = AfterMidnightLandingTime(arrTime % 1440);
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(arrTime);
					resDhParam.ConnectTime = GlobalSettings.connectTime;
				}


				if (isVof && vofSplit9)     // this means the resDh is first thing on next day 
				{
					// line 12 - OA85

					rptTime = getReportOrReleaseInMinutesFromOther(dp.ReleaseTime % 1440, 3, today, !getReleaseTime);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(rptTime % 1440 + GlobalSettings.show1stDay);
					resDhParam.ArrSta = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].DepSta;
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					arrTime = trip.DutyPeriods[resDhDp].Flights[0].DepTime;
					arrTime = AfterMidnightLandingTime(arrTime % 1440);
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(arrTime);
					resDhParam.RefFlt = resDhFltSeq == 0 ? dp.Flights[0] : flt;
					resDhParam.Date = today;
					resDhParam.ConnectTime = GlobalSettings.connectTime;

				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					// line 30 - OAEJ

					depTime = dp.Flights[Math.Max(0, resDhFltSeq - 1)].ArrTime;
					arrTime = getReportOrReleaseInMinutesFromOther(dp.DepTimeFirstLeg - (resDhDp == 0 ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay), 1, today, getReleaseTime);
					resDhParam.DepSta = flt.DepSta;
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(depTime % 1440));
					resDhParam.ConnectTime=GlobalSettings.connectTime;
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(arrTime % 1440) - GlobalSettings.debrief);
					resDhParam.RefFlt = dp.Flights[Math.Max(0, resDhFltSeq - 1)];
					resDhParam.Date=today;

				}

				//if ((!isVof && vobSplit0) || (!isVof && vacTrip.VDvsRCdata.RCcalcFlightSeqNum<=1))   
				if (!isVof && vobSplit0)   // should check for deadhead the day prior and use last flight as ref flt
				{
					// line 49 - OAEW

					resDhParam.RefFlt = dp.Flights[Math.Max(0, resDhFltSeq - 1)];
					var refFlt = trip.DutyPeriods[resDhDp - 1].Flights[trip.DutyPeriods[resDhDp - 1].Flights.Count - 1];
					var reportTime = trip.DutyPeriods[resDhDp - 1].DepTimeFirstLeg - (resDhDp == 1 ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay);
					resDhParam.RefFlt = refFlt;
					resDhParam.Date = yesterday;
					resDhParam.DepSta = refFlt.ArrSta;
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					//resDhParam.DepTimeHhMm = WBidHelper.ConvertMinutesToHhhmmInt(refFlt.ArrTime % 1440);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(refFlt.ArrTime % 1440));
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(getReportOrReleaseInMinutesFromOther(reportTime % 1440, 1, resDhParam.Date, getReleaseTime) - GlobalSettings.debrief);
					resDhParam.ConnectTime = GlobalSettings.connectTime;
					//resDhParam.Date = today;
				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "VD" && !vobSplit0)
				{
					// line 191 - OACY

					resDhParam.RefFlt = dp.Flights[Math.Max(0, resDhFltSeq - 1)];
					var refFlt = trip.DutyPeriods[resDhDp - 1].Flights[trip.DutyPeriods[resDhDp - 1].Flights.Count - 1];
					var reportTime = trip.DutyPeriods[resDhDp - 1].DepTimeFirstLeg - (resDhDp == 1 ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay);
					resDhParam.RefFlt = refFlt;
					resDhParam.Date = yesterday;
					resDhParam.DepSta = refFlt.ArrSta;
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					//resDhParam.DepTimeHhMm = WBidHelper.ConvertMinutesToHhhmmInt(refFlt.ArrTime % 1440);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(refFlt.ArrTime % 1440));
					resDhParam.ArrTimeHhMm = Math.Min(2700, VacationHelper.ConvertMinutesToHhhmmInt(getReportOrReleaseInMinutesFromOther(reportTime % 1440, 1, resDhParam.Date, getReleaseTime) - GlobalSettings.debrief));
					resDhParam.ConnectTime = GlobalSettings.connectTime;
					//resDhParam.Date = today;
				}
			}
			else       // since rcWasZero, Vof deadheads day before (yesterday) and Vob deadheads next day (tomorrow)
			{
				// should limit for AM or PM for performance issues
				DutyPeriod tempDp = trip.DutyPeriods[Math.Max(0, vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1)];

				var refFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq]; 
				var arrSta = isVof ? refFlt.DepSta : GlobalSettings.CurrentBidDetails.Domicile;
				var depSta = isVof ? GlobalSettings.CurrentBidDetails.Domicile : refFlt.DepSta;

				var midDay = tempDp.DepTimeFirstLeg % 1440 + (tempDp.LandTimeLastLeg - tempDp.DepTimeFirstLeg) / 2;
				int amCentroid = 690;       // 690 = 11:30 CST
				int pmCentroid = 1110;      // 1110 = 18:30 CST
				string dutyPdAmPm = (midDay - amCentroid) < (pmCentroid - midDay) ? "AM" : "PM";

				//int earliestPlusMinus12 = getEarliestPlusMinus12(isVof);
				int earliestTakeOffForCityInMinutes=GetEarliestTakeOffForCityInMinutes(depSta, resDhParam.Date);
				int latestArrivalInMinutes = GetLatestArrivalForCityInMinutes(arrSta, resDhParam.Date);

				// shorten the range of search by 8 hours for performance
				//earliestTakeOffForCityInMinutes = dutyPdAmPm == "AM" ? earliestTakeOffForCityInMinutes : earliestTakeOffForCityInMinutes + 480;
				//latestArrivalInMinutes = dutyPdAmPm == "AM" ? latestArrivalInMinutes - 480 : latestArrivalInMinutes;

				// frank add 1/8/2015  --  window is not wide enough to see potential deadheads
				earliestTakeOffForCityInMinutes = dutyPdAmPm == "AM" ? earliestTakeOffForCityInMinutes : earliestTakeOffForCityInMinutes + 300;
				latestArrivalInMinutes = dutyPdAmPm == "AM" ? latestArrivalInMinutes - 240 : latestArrivalInMinutes;

				// get yesterday or tomorrow for the duty period
				int rcZeroRefDutPd = (vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1);
				rcZeroRefDutPd = isVof ? rcZeroRefDutPd - 1 : rcZeroRefDutPd + 1;

				// flesh out -- may need a vacation type for "VO"
				if (isVof && vofSplit9)
				{
					// line 12 - OA85

					resDhParam.Date = yesterday;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					//resDhParam.ArrSta = resDhParam.RefFlt.ArrSta;
					resDhParam.ArrSta = resDhParam.RefFlt.DepSta;
				}

				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType =="VD")
				{
					// line 54 - OA87

					resDhParam.Date = today;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.ArrSta = resDhParam.RefFlt.ArrSta;                    
				}

				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					// line 25 - OA7J

					resDhParam.Date = yesterday;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.ArrSta = resDhParam.RefFlt.ArrSta;
				}

				if (!isVof && vobSplit0)
				{
					// line 49 - OAEW

					resDhParam.Date = today;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepSta = resDhParam.RefFlt.DepSta;
				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "VD" && !vobSplit0)
				{
					// line 191 - OACY

					resDhParam.Date = today;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepSta = resDhParam.RefFlt.DepSta;
				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					// line 30 - OAEJ

					resDhParam.Date = tomorrow;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq-1];
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepSta = resDhParam.RefFlt.ArrSta;
				}

				resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(earliestTakeOffForCityInMinutes);
				resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(latestArrivalInMinutes);
				resDhParam.ConnectTime = GlobalSettings.connectTime;
			}

			return resDhParam;
		}

		private int getReportOrReleaseInMinutesFromOther(int minutesInHerb, int legs, DateTime date, bool getReleaseHhMm)
		{
			// do FDP calculation and Duty calculation
			string legsFlown = Math.Max(1, legs).ToString(); 

			// insure minutes are based on day
			minutesInHerb = minutesInHerb % 1440;
			minutesInHerb = AfterMidnightLandingTime(minutesInHerb);

			// adjust minutes for domicile
			//var domRlsMinutes = WBidHelper.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, minutesInHerb % 1440);
			var domRlsMinutes = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, minutesInHerb);
			var domRptMinutes = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, minutesInHerb);

			if (getReleaseHhMm)
			{
				// use domicile time for FAR-117 legalities
				// this may be wrong
				//int reportInHhMm = WBidHelper.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, WBidHelper.ConvertMinutesToHhhmmInt(minutes % 1440));
				//int reportInHhMm = WBidHelper.ConvertMinutesToHhhmmInt(minutesInHerb % 1440);
				int reportInHhMm = VacationHelper.ConvertMinutesToHhhmmInt(domRptMinutes);



				var fdpHours = MaxFdpTablePilots(legs)
					.Where(x => x.AccStartMin <= reportInHhMm && x.AccStartMax >= reportInHhMm)
					.Select(y => y.MaxFdp).FirstOrDefault();


				//				var fdpHours = GlobalSettings.MaxFdpTablePilots(legs).AsEnumerable()
				//					.Where(x => x.Field<int>("AccStartMin") <= reportInHhMm && x.Field<int>("AccStartMax") >= reportInHhMm)
				//					.Select(x => x.Field<decimal>("MaxFdp")).FirstOrDefault();


				var dutyHours = MaxDutyDayTablePilots()
					.Where(x => x.ReportMin <= reportInHhMm && x.ReportMax >= reportInHhMm)
					.Select(y => y.MaxDutyDay).FirstOrDefault();

				//				var dutyHours = GlobalSettings.MaxDutyDayTablePilots().AsEnumerable()
				//					.Where(x => x.Field<int>("ReportMin") <= reportInHhMm && x.Field<int>("ReportMax") >= reportInHhMm)
				//					.Select(x => x.Field<decimal>("MaxDutyDay")).FirstOrDefault();

				// Frank Add 4/10/2015 if I am getting release time, the limit should just be duty hours, since the DH does not count towards fdp time

				//var releaseMinutes = ((int)((decimal)fdpHours  60) + GlobalSettings.debrief) < (dutyHours  60) ?
				//                (int)((decimal)fdpHours * 60) + GlobalSettings.debrief + minutesInHerb :
				//                (int)((decimal)dutyHours * 60 + minutesInHerb);

				var releaseMinutes = (int)((decimal)dutyHours * 60 + minutesInHerb);

				//return WBidHelper.ConvertMinutesToHhhmmInt(releaseMinutes);

				// frank change 2/17/2015 to insure last landing is no later the SWA Fly day (275)

				// return releaseMinutes;
				return Math.Min(releaseMinutes, GlobalSettings.LastLandingMinus1440 + 1440);
			}
			else
			{
				// Algorithm for calculating the earliest report time is tricky
				// with a report time of 0450 you would have an fdp = 10 hours  (0400-2359 = 10 hours)
				// however, a report 10 minutes later at 0500 you would have an fdp = 12 hours  (0500-1659 = 12 hours)
				// *****
				// example with Duty Period
				// lets say dutyHours = 12 and reportMinHhMm = 0400 and release time is 1650
				//      in this case, the earliest report would be Math.Max(reportMinHhMm, (releaseTime - dutyHours)
				//      which would be Math.Max(0400, (1650 - 1200 = 0450)) = 0450
				// lets say dutyHours = 12 and reportMinHhMm = 0400 and release time is 1550
				//      in this case, the earliest report would be Math.Max(reportMinHhMm, (releaseTime - dutyHours)
				//      which would be Math.Max(0400, (1550 - 1200 = 0350)) = 0400

				string legColumn = FdpLegColumn(legs);

				var domLandMinutes = domRlsMinutes - GlobalSettings.debrief;

				PropertyInfo pinfo = typeof(MasterFdpTable).GetProperty(legColumn);

				var fdpReportMinutesDom = MasterFdpTable().FirstOrDefault(x => x.AccStartMax / 100 * 60 +
					(int)(x.AccStartMax % 100) +
					(int)(((decimal)pinfo.GetValue(x,null)) * 60) >= domLandMinutes).AccStartMin / 100 * 60;


				//				var fdpReportMinutesDom = GlobalSettings.MasterFdpTable().AsEnumerable().Where(x => x.Field<int>("AccStartMax") / 100 * 60 +
				//					(int)(x.Field<int>("AccStartMax") % 100) +
				//					(int)(x.Field<decimal>(legColumn) * 60) >= domLandMinutes)
				//					.Select(x => x.Field<int>("AccStartMin") / 100 * 60).FirstOrDefault();



				var fdpMinutes = (decimal)pinfo.GetValue(MasterFdpTable().FirstOrDefault(x => x.AccStartMax / 100 * 60 +
					(int)(x.AccStartMax % 100) +
					(int)(((decimal)pinfo.GetValue(x,null)) * 60) >= domLandMinutes),null) * 60;
				//.Select(x => x.Field<decimal>(legColumn) * 60).FirstOrDefault();

				//
				//				var fdpMinutes =          GlobalSettings.MasterFdpTable().AsEnumerable().Where(x => x.Field<int>("AccStartMax") / 100 * 60 +
				//					(int)(x.Field<int>("AccStartMax") % 100) +
				//					(int)(x.Field<decimal>(legColumn) * 60) >= domLandMinutes)
				//					.Select(x => x.Field<decimal>(legColumn) * 60).FirstOrDefault();

				var reportFdpMinutesDom = Math.Max(fdpReportMinutesDom, domLandMinutes - fdpMinutes);

				/***************************************************
                 *  Duty
                   **********************************************/

				//var test = GlobalSettings.MaxDutyDayTablePilots().AsEnumerable().Skip(3).Select(x => x.Field<decimal>("MaxDutyDay")).FirstOrDefault();

				// walk through the math and I will see.



				var dutyReportMinutesDom = MaxDutyDayTablePilots().Where(x => x.ReportMax / 100 * 60 +
					(int)(x.ReportMax % 100) +
					(int)(x.MaxDutyDay * 60) >= domRlsMinutes)
					.Select(y => y.ReportMin / 100 * 60).FirstOrDefault();


				//				var dutyReportMinutesDom = GlobalSettings.MaxDutyDayTablePilots().AsEnumerable().Where(x => x.Field<int>("ReportMax") / 100 * 60 +
				//					(int)(x.Field<int>("ReportMax") % 100) +
				//					(int)(x.Field<decimal>("MaxDutyDay") * 60) >= domRlsMinutes)
				//					.Select(x => x.Field<int>("ReportMin") / 100 * 60).FirstOrDefault();




				var dutyMinutes = MaxDutyDayTablePilots().Where(x => x.ReportMax / 100 * 60 +
					(int)(x.ReportMax % 100) +
					(int)(x.MaxDutyDay * 60) >= domRlsMinutes)
					.Select(y => y.MaxDutyDay * 60).FirstOrDefault();



				//				var dutyMinutes =          GlobalSettings.MaxDutyDayTablePilots().AsEnumerable().Where(x => x.Field<int>("ReportMax") / 100 * 60 +
				//					(int)(x.Field<int>("ReportMax") % 100) +
				//					(int)(x.Field<decimal>("MaxDutyDay") * 60) >= domRlsMinutes)
				//					.Select(x => x.Field<decimal>("MaxDutyDay") * 60).FirstOrDefault();

				var reportDutyMinutesDom = Math.Max(dutyReportMinutesDom, domRlsMinutes - dutyMinutes);

				var reportHerbMinutes = ConvertToHerbFromDomTime(GlobalSettings.CurrentBidDetails.Domicile, date, (int)Math.Max(reportFdpMinutesDom, reportDutyMinutesDom));

				return (int)reportHerbMinutes;
			}
		}
		private void ConvertVD_DpsAndFltsTo_VO(VacationTrip vacTrip)
		{
			foreach (var dp in vacTrip.DutyPeriodsDetails)
			{
				foreach (var flt in dp.FlightDetails)
				{
					if (flt.VacationType == "VD")
						flt.VacationType = "VO";
				}
				dp.RigDpMin_VO = dp.RigDpMin_VO + dp.RigDpMin_VD;
				dp.RigDpMin_VD = 0;
				dp.RigDhr_VO = dp.RigDhr_VO + dp.RigDhr_VD;
				dp.RigDhr_VD = 0;
				//Comment by ranish to fix the issue in VOF trips
				// if (dp.VacationType == "VA" || dp.VacationType == "Split" || dp.VacationType == "VD")
				if (dp.VacationType == "Split" || dp.VacationType == "VD")
					dp.VacationType = "VO";
			}
			vacTrip.RigVAVO_Adg = vacTrip.RigVAVO_Adg + vacTrip.RigVD_Adg;
			vacTrip.RigVD_Adg = 0;
			vacTrip.RigVAVO_Tafb = vacTrip.RigVAVO_Tafb + vacTrip.RigVD_Tafb;
			vacTrip.RigVD_Tafb = 0;
		}

        private void DistributeRig(VacationTrip vacTrip)
        {
            /*******************************************************************************************
             * 1.   Only distribute RigDpMin and RigDhr if the dutyPeriod is split (VO and VD flts).  
             *      Half of each rig goes to VO and VD in the dutyPeriod.  Use Rig1st and Rig2nd to hold the 
             *      tfp values if rig is distributed.  
             * 2.   Distribute the RigAdg and RigTafb to the lesser total: (VA+VO) or VD.  Use Rig1st 
             *      and Rig2nd to hold the tfp values if the rig is distributed.
             *      
             * above distribution is NOT correct
             * 
             * 1.   THR and ADG is distributed before a duty period is split
             * 2.   THR and ADG to to the smallest dutyPeriod until all duty periods are equal and once equal
             *      THR and ADG are split equally between all the duty periods.
             * 3.   DPM and DHR stay with dutyPeriod unless split
             * 4.   if dutyPeriod is split, DPM and DHR go to lesser of the two split duty periods (VO or VD) and once equal
             *      DPM and DHR is split equally between the two split duty periods (VO & VD)
             * 5.   If a split duty period has THR or ADG, then THR and ADG to to lesser of split duty periods (VO or VD) and once
             *      equal THR and ADG is split equally to the two split duty periods (VO & VD)
             * 6.   Note that the total value for a duty period does not increase when distributing rig to split duty periods.
             
             ********************************************************************************************/
            int i = 0;
            if (vacTrip.TripName == "OA7J")
                i = 9;

            int totDutPds = vacTrip.DutyPeriodsDetails.Count();
            int splitDutPd = 0;
            decimal dutPdTfp = 0.0m;
            int dutPdCount = 0;
            //public class dpTfpPlusRig
            //{
            //    public int dpNum { get; set; }
            //    public decimal tfpPlusRig { get; set; }
            //}
            ////List<decimal> dpTfpPlusRig = new List<decimal>();
            //List<decimal> sortedTfpPlusRig = new List<decimal>();

            //foreach (var dp in vacTrip.DutyPeriodsDetails)
            //{
            //    dutPdCount++;
            //    if (dp.VacationType == "Split")
            //    {
            //        splitDutPd = dutPdCount;
            //    }

            //    foreach (var flt in dp.FlightDetails)
            //    {
            //        dp.Tfp += flt.Tfp;
            //    }
            //    dp.RigDhr = trips[vacTrip.TripName].DutyPeriods[dutPdCount - 1].RigDhr;
            //    dp.RigDpMin = trips[vacTrip.TripName].DutyPeriods[dutPdCount - 1].RigDailyMin;
            //    dpTfpPlusRig.Add(dp.Tfp + dp.RigDhr + dp.RigDpMin);
            //    // create class dpTfpTotals:  dpNum, tfp
            //}

            // frank add 6/28/2019: now distribute ADG rig to duty period
            //sortedTfpPlusRig = dpTfpPlusRig;
            //sortedTfpPlusRig.Sort();

            //for (int dutPd = 0; dutPd < dutPdCount; dutPd++)
            //{
            //    if (dutPdCount == 1)
            //    {
            //        break;
            //    }
            //    else
            //    {

            //    }
            //}

            //switch (dutPdCount)
            //{
            //    case 4:
            //        if (dpTfpPlusRig[0] < 
            //}

            foreach (var dp in vacTrip.DutyPeriodsDetails)
            {
                if (dp.VacationType == "Split")
                {
                    if (false)   // Frank change 3/29/2014: changed to false for new distribution method
                    {
                        dp.RigDpMin_VO = dp.RigDpMin / 2;
                        dp.RigDpMin_VD = dp.RigDpMin / 2;
                        dp.RigDhr_VO = dp.RigDhr / 2;
                        dp.RigDhr_VD = dp.RigDhr / 2;
                    }
                    else
                    {

                        // * Old Rig calculation before 4-12-2019. commented by Roshil to modify the rig calculation
                        //decimal dpVdTotTfp = dp.VacationDetails.VDfTfp + dp.VacationDetails.VDbTfpInLine;
                        //decimal dpVoTotTfp = dp.VacationDetails.VOfTfp + dp.VacationDetails.VObTfpInLine;

                        //Added by Roshil 4-12-2019
                        decimal dpVdTotTfp = dp.FlightDetails.Where(x => x.VacationType == "VD").Sum(y => y.Tfp);
                        decimal dpVoTotTfp = dp.FlightDetails.Where(x => x.VacationType == "VO").Sum(y => y.Tfp);
                        //end addition on 4-12-2019
                        // diffVoVdTfp > 0 means the VO duty period is bigger, also the diffVoVdTfp is the amount to make duty periods equal

                        decimal diffVoVdTfp = dpVoTotTfp - dpVdTotTfp;
                        decimal rigTotToDistribute = dp.RigDpMin + dp.RigDhr;
                        decimal pctToLowestPairTotal = rigTotToDistribute > Math.Abs(diffVoVdTfp) ? Math.Abs(diffVoVdTfp)
							/ rigTotToDistribute : 1m;

                        // distribute enough rig to make both VO and VD equal

                        if (diffVoVdTfp > 0)
                        {
                            dp.RigDpMin_VD = dp.RigDpMin * pctToLowestPairTotal;
                            dp.RigDhr_VD = dp.RigDhr * pctToLowestPairTotal;
                        }
                        else
                        {
                            dp.RigDpMin_VO = dp.RigDpMin * pctToLowestPairTotal;
                            dp.RigDhr_VO = dp.RigDhr * pctToLowestPairTotal;
                        }

                        decimal remainingRigToDistribute = (rigTotToDistribute - Math.Abs(diffVoVdTfp)) > 0 ? rigTotToDistribute - Math.Abs(diffVoVdTfp) : 0;

                        // if any rig is remainig it is distributed 50/50 to both dutyperiods in the split duty period

                        if (remainingRigToDistribute > 0)
                        {
                            dp.RigDpMin_VD += dp.RigDpMin * (1m - pctToLowestPairTotal) / 2m;
                            dp.RigDhr_VD += dp.RigDhr * (1m - pctToLowestPairTotal) / 2m;
                            dp.RigDpMin_VO += dp.RigDpMin * (1m - pctToLowestPairTotal) / 2m;
                            dp.RigDhr_VO += dp.RigDhr * (1m - pctToLowestPairTotal) / 2m;
                        }

                    }

                }
                else
                {
                    switch (dp.VacationType)
                    {
                        case "VA":
                            dp.RigDpMin_VA = dp.RigDpMin;
                            dp.RigDhr_VA = dp.RigDhr;
                            break;
                        case "VO":
                            dp.RigDpMin_VO = dp.RigDpMin;
                            dp.RigDhr_VO = dp.RigDhr;
                            break;
                        case "VD":
                            dp.RigDpMin_VD = dp.RigDpMin;
                            dp.RigDhr_VD = dp.RigDhr;
                            break;
                    }
                }
            }

			// add new distribution of ADG and Tafb rig
			//DistributeAdgTafbRig(vacTrip);
			//DistributeAdgTafbRigToLastDuperiod(vacTrip);

			//if (vacTrip.DutyPeriodsDetails.SelectMany(x => x.FlightDetails).Any(y => y.VacationType == "VD"))
			//{
			//	DistributeAdgTafbRigToLowestDutyperiod(vacTrip);
			//}
			//else
			//{
			//	DistributeAdgTafbRigToLastDuperiod(vacTrip);
			//}
			decimal totVAVO = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => (y.VacationType == "VA" || y.VacationType == "VO") && x.isInBp).Sum(y => y.Tfp));
            decimal totVD = vacTrip.DutyPeriodsDetails.Sum(x => x.FlightDetails.Where(y => y.VacationType == "VD" && x.isInBp).Sum(y => y.Tfp));

            /*      THR (Tafb rig) and ADG to to the smallest dutyPeriod until all duty periods are equal and once equal
             *      THR (Tafb rig) and ADG are split equally between all the duty periods.
             */

            //if (vacTrip.VacationType != "VA")
            //{

            //    if (totVD == 0)
            //    {
            //        vacTrip.RigVAVO_Adg = vacTrip.RigAdg;
            //        vacTrip.RigVAVO_Tafb = vacTrip.RigTafb;
            //    }
            //    else
            //    {
            //        vacTrip.RigVAVO_Adg = totVAVO < totVD ? vacTrip.RigAdg : 0m;
            //        vacTrip.RigVD_Adg = totVD < totVAVO ? vacTrip.RigAdg : 0m;
            //        vacTrip.RigVAVO_Tafb = totVAVO < totVD ? vacTrip.RigTafb : 0m;
            //        vacTrip.RigVD_Tafb = totVD < totVAVO ? vacTrip.RigTafb : 0m;
            //    }
            //}
        }
        private void DistributeAdgTafbRigToLastDuperiod(VacationTrip vacTrip)
        {
            if (vacTrip.VacationType == "VA") return;

            decimal RigTafb = vacTrip.RigTafb;
            decimal RigAdg = vacTrip.RigAdg;
            int dpcount = vacTrip.DutyPeriodsDetails.Count;
            var lastDP = vacTrip.DutyPeriodsDetails[dpcount - 1];
            if (lastDP.VacationType == "VD")
            {
                lastDP.RigAdg_VD += RigAdg;
                lastDP.RigTafb_VD += RigTafb;
            }
            else if (lastDP.VacationType == "VO")
            {
                lastDP.RigAdg_VO += RigAdg;
                lastDP.RigTafb_VO += RigTafb;
            }
            else if (lastDP.VacationType == "VA")
            {
                lastDP.RigAdg_VA += RigAdg;
                lastDP.RigTafb_VA += RigTafb;
            }
            else if (lastDP.VacationType == "Split")
            {
                //need to check this logic when the last day dutyperiod is split.
                int flightcount = lastDP.FlightDetails.Count;
                var lastflight = lastDP.FlightDetails[flightcount - 1];
                if (lastflight.VacationType == "VA")
                {
                    lastDP.RigAdg_VA += RigAdg;
                    lastDP.RigTafb_VA += RigTafb;
                }
                if (lastflight.VacationType == "VO")
                {
                    lastDP.RigAdg_VO += RigAdg;
                    lastDP.RigTafb_VO += RigTafb;
                }
                if (lastflight.VacationType == "VD")
                {
                    lastDP.RigAdg_VD += RigAdg;
                    lastDP.RigTafb_VD += RigTafb;
                }
            }
        }

        decimal SplitDpADGRig = 0;
        decimal SplitDpTAFBRig = 0;
        private void DistributeAdgTafbRigToLowestDutyperiod(VacationTrip vacTrip)
        {
            /*
             * If the rig is generated in accordance with the THR and ADG provision of this agreement, that
portion of the rig is credited to the lowest paying duty period in the original pairing until all
duty periods are equal and then equally to all duty periods until the rig is exhausted. THR and
ADG rig credited to a duty period to be split will be prorated as above.
Keep in mind a single duty period can be split.  If that is done, then a 1 day trip would have two duty periods, a 2 day trips would have 3 duty periods, a 3 day trip would have 4 duty periods and a 5 day trip would have 6 duty periods.
             */

            SplitDpADGRig = 0;
            SplitDpTAFBRig = 0;
            if (vacTrip.VacationType == "VA") return;

            decimal RigTafb = vacTrip.RigTafb;
            decimal RigAdg = vacTrip.RigAdg;
            List<VacDpForRigDist> AllDpsForDist = new List<VacDpForRigDist>();
            // get all duty periods an order from lowest tfp to highest tfp for distribution. Distribute the Rig to the original Dutyperiod first
            foreach (var dp in vacTrip.DutyPeriodsDetails)
            {
                VacDpForRigDist vacDpForDist = new VacDpForRigDist();
                vacDpForDist.Tfp = dp.FlightDetails.Sum(x => x.Tfp) + dp.RigDpMin + dp.RigDhr;
                vacDpForDist.VacType = dp.VacationType;
                vacDpForDist.DutPdSeqNum = dp.RefDutPdSeqNum;
                AllDpsForDist.Add(vacDpForDist);
            }
            List<VacDpForRigDist> SortedDpsForDist = new List<VacDpForRigDist>();
            SortedDpsForDist = AllDpsForDist.OrderBy(x => x.Tfp).ToList();
            DistributeRigToSortedDutPds(SortedDpsForDist, RigAdg, RigTafb, vacTrip);

            // distribut the Rig to the split dutyperiod
            AllDpsForDist = new List<VacDpForRigDist>();
            foreach (var dp in vacTrip.DutyPeriodsDetails)
            {
                VacDpForRigDist vacDpForDist = new VacDpForRigDist();
                if (dp.VacationType == "Split")
                {
                    vacDpForDist.Tfp = dp.FlightDetails.Where(x => x.VacationType == "VO").Sum(x => x.Tfp) + dp.RigDpMin_VO + dp.RigDhr_VO;
                    vacDpForDist.VacType = "VO";
                    vacDpForDist.DutPdSeqNum = dp.RefDutPdSeqNum;
                    AllDpsForDist.Add(vacDpForDist);

                    vacDpForDist = new VacDpForRigDist();
                    vacDpForDist.Tfp = dp.FlightDetails.Where(x => x.VacationType == "VD").Sum(x => x.Tfp) + dp.RigDpMin_VD + dp.RigDhr_VD;
                    vacDpForDist.VacType = "VD";
                    vacDpForDist.DutPdSeqNum = dp.RefDutPdSeqNum;
                    AllDpsForDist.Add(vacDpForDist);


                }

            }
            SortedDpsForDist = new List<VacDpForRigDist>();
            SortedDpsForDist = AllDpsForDist.OrderBy(x => x.Tfp).ToList();
            DistributeRigToSortedDutPds(SortedDpsForDist, SplitDpADGRig, SplitDpTAFBRig, vacTrip);


        }

        private void DistributeRigToSortedDutPds(List<VacDpForRigDist> SortedDpsForDist, decimal RigAdg, decimal RigTafb, VacationTrip vacTrip)
        {
            //decimal rigAvail = RigTafb + RigAdg;
            decimal rigAvail = (RigTafb > RigAdg) ? RigTafb : RigAdg;
            if (rigAvail == 0) return;

            int numDutPds = SortedDpsForDist.Count();
            decimal rigRemain = rigAvail;
            decimal adgPct = RigAdg / rigAvail;

            // distributes Tafb and Adg rig until rig is gone or all duty periods are equal
            for (int i = 0; i < (numDutPds - 1) && rigRemain > 0; i++)
            {
                //commented by Roshil 4-12-2019
                // decimal rigToDist = Math.Min(rigRemain, SortedDpsForDist[i + 1].Tfp - SortedDpsForDist[i].Tfp);
                var diffrenceInPay = SortedDpsForDist[i + 1].Tfp - SortedDpsForDist[i].Tfp;
				decimal rigToDist = (diffrenceInPay < (rigRemain / (i + 1))) ? (diffrenceInPay * (i + 1)) : rigRemain;
				for (int p = 0; p <= i; p++)
                {
                    AssignRigToVacDutPd(SortedDpsForDist[p].DutPdSeqNum, SortedDpsForDist[p].VacType, rigToDist / (i + 1), adgPct, vacTrip);
                }
                rigRemain -= rigToDist;
            }

            // distributes all remaining rig to duty periods equally
            if (rigRemain > 0)
            {
                decimal rigToDist = rigRemain;
                for (int i = 0; i < numDutPds; i++)
                {
                    AssignRigToVacDutPd(SortedDpsForDist[i].DutPdSeqNum, SortedDpsForDist[i].VacType, rigToDist / numDutPds, adgPct, vacTrip);
                }
            }

            // the Tafb and Adg rig has been distributed to the duty periods, so this rig does NOT exist at the trip level - I think :-)
            vacTrip.RigAdg = 0;
            vacTrip.RigTafb = 0;
        }

        private void AssignRigToVacDutPd(int dutyPd, string dpVacType, decimal rigToDist, decimal adgPct, VacationTrip vacTrip)
        {
            if (dpVacType == "VD")
            {
                vacTrip.DutyPeriodsDetails[dutyPd - 1].RigAdg_VD += rigToDist * adgPct;
                vacTrip.DutyPeriodsDetails[dutyPd - 1].RigTafb_VD += rigToDist * (1 - adgPct);
            }
            else if (dpVacType == "VO")
            {
                vacTrip.DutyPeriodsDetails[dutyPd - 1].RigAdg_VO += rigToDist * adgPct;
                vacTrip.DutyPeriodsDetails[dutyPd - 1].RigTafb_VO += rigToDist * (1 - adgPct);
            }
            else if (dpVacType == "VA")
            {
                vacTrip.DutyPeriodsDetails[dutyPd - 1].RigAdg_VA += rigToDist * adgPct;
                vacTrip.DutyPeriodsDetails[dutyPd - 1].RigTafb_VA += rigToDist * (1 - adgPct);
            }
            else if (dpVacType == "Split")
            {
                SplitDpADGRig += rigToDist * adgPct;
                SplitDpTAFBRig += rigToDist * (1 - adgPct);
            }

        }


		private int ConvertToHerbFromDomTime(string domicile, DateTime date, int domTime)
		{
			bool isDst ;
			TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Denver");
			TimeSpan ts = new TimeSpan(12, 0, 0);

			// Insures that time dateTime is not 12:00AM but 12:00pm
			// Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.

			date = date.Date + ts;
			isDst = timeZoneInfo.IsDaylightSavingTime(date);

			switch (domicile)
			{
			case "ATL":
				return domTime - 60;       // EST = herb - 60
					case "AUS":
				return domTime;            // CST = herb 
			case "BWI":
				return domTime - 60;       // EST = herb - 60
			case "DAL":
				return domTime;            // CST = herb 
			case "DEN":
				return domTime + 60;       // MST = herb + 60
					case "FLL":
				return domTime - 60;       // EST = herb - 60
			case "HOU":
				return domTime;            // CST = herb 
			case "LAS":
				return domTime + 120;      // PST = herb + 120
            case "LAX":
                    return domTime + 120;      // PST = herb + 120
			case "MCO":
				return domTime - 60;       // EST = herb - 60
			case "MDW":
				return domTime;            // CST = herb 
			case "OAK":
				return domTime + 120;      // PST = herb + 120
			case "PHX":
				if (isDst)
					return domTime + 120;  // PST = herb + 120
				else
					return domTime + 60;   // MST = herb + 60
			default:
				return 1440;
			}
		}

		public int AfterMidnightLandingTime(int landTime)
		{
			if (landTime < GlobalSettings.LastLandingMinus1440)
				return landTime += 1440;
			else
				return landTime % 1440;
		}

		public int DomicileTimeFromHerb(string domicile, DateTime date, int herb)
		{
			bool isDst ;
			TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Denver");
			TimeSpan ts = new TimeSpan(12, 0, 0);

			// Insures that time dateTime is not 12:00AM but 12:00pm
			// Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.

			date = date.Date + ts;
			isDst = timeZoneInfo.IsDaylightSavingTime(date);

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
				return herb + 60;       // MST = herb - 60
			case "HOU":
				return herb;            // CST = herb 
			case "LAS":
				return herb - 120;      // PST = herb - 120
            case "LAX":
                return herb - 120;      // PST = herb - 120
			case "MCO":
				return herb + 60;       // EST = herb + 60
			case "MDW":
				return herb;            // CST = herb 
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


		/// <summary>
		/// calculates the domicile showtime to determine AM or PM shows for vacation overlap correction
		/// </summary>
		/// <param name="domicile"></param>
		private int DomicileShowTime(string domicile, DateTime dutPdDate)
		{
			TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Denver");
			TimeSpan ts = new TimeSpan(12, 0, 0);

			// Insures that time dateTime is not 12:00AM but 12:00pm
			// Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.

			dutPdDate = dutPdDate.Date + ts;
			bool isDst = timeZoneInfo.IsDaylightSavingTime(dutPdDate);

			if (domicile == "BWI" || domicile == "MCO" || domicile == "ATL" || domicile == "FLL")
				return 600;  // returns 10:00 CST in herb, which is 11:00 domicle time
			if (domicile == "AUS" || domicile == "DAL" || domicile == "HOU" || domicile == "MDW")
				return 660;  // retruns 11:00 CST in herb, which is 11:00 domicle time
			if (domicile == "DEN")
				return 720;  // retruns 12:00 CST in herb, which is 11:00 domicle time
            if (domicile == "LAS" || domicile == "OAK" || domicile=="LAX")
				return 780;  // retruns 13:00 CST in herb, which is 11:00 domicle time
			if (domicile == "PHX" && isDst)
				return 780;  // returns 13:00 CST in herb, which is 11:00 domicile time
			else
				return 720;  // returns 12:00 CST in herb, which is 11:00 domicile time
		}

		/// <summary>
		/// calculates the return time for each domicile reference an AM or PM show for Vacation Overlap
		/// </summary>
		/// <param name="domicile"></param>
		/// <param name="AmPm"></param>
		private int DomicileReturnTime(string domicile, string AmPm, DateTime dutPdDate)
		{

			TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/Denver");

			TimeSpan ts = new TimeSpan(12, 0, 0);

			// Insures that time dateTime is not 12:00AM but 12:00pm
			// Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.

			dutPdDate = dutPdDate.Date + ts;
			bool isDst = timeZoneInfo.IsDaylightSavingTime(dutPdDate);


			if (AmPm == "AM")
			{
				if (domicile == "BWI" || domicile == "MCO" || domicile == "ATL" || domicile == "FLL")
					return 1110;  // returns 18:30 CST in herb, which is 19:30 domicle time
				if (domicile == "AUS" || domicile == "DAL" || domicile == "HOU" || domicile == "MDW")
					return 1170;  // returns 19:30 CST in herb, which is 19:30 domicle time
				if (domicile == "DEN")
					return 1230;  // returns 20:30 CST in herb, which is 19:30 domicle time
                if (domicile == "LAS" || domicile == "OAK"|| domicile == "LAX")
					return 1290;  // returns 21:30 CST in herb, which is 19:30 domicle time
				if (domicile == "PHX" && isDst)
					return 1290;  // returns 21:30 CST in herb, which is 19:30 domicle time
				else
					return 1230;  // returns 20:30 CST in herb, which is 19:30 domicle time
			}
			else  // is PM
			{
				if (domicile == "BWI" || domicile == "MCO" || domicile == "ATL" || domicile == "FLL")
					return 1380;  // returns 23:00 CST in herb, which is 24:00 domicle time
				if (domicile == "AUS" || domicile == "DAL" || domicile == "HOU" || domicile == "MDW")
					return 1440;  // retruns 24:00 CST in herb, which is 24:00 domicle time
				if (domicile == "DEN")
					return 1500;  // retruns 25:00 CST in herb, which is 24:00 domicle time
                if (domicile == "LAS" || domicile == "OAK" || domicile == "LAX")
					return 1560;  // retruns 26:00 CST in herb, which is 24:00 domicle time
				if (domicile == "PHX" && isDst)
					return 1560;  // retruns 26:00 CST in herb, which is 24:00 domicle time
				else
					return 1500;  // retruns 25:00 CST in herb, which is 24:00 domicle time
			}
		}

		private int GetReturnTimeVof(int showTime, DateTime dutPdDate)
		{
			//if (tripShowTime < 660)
			if (showTime < DomicileShowTime(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate))
			{
				//returnTime = 1170;
				return DomicileReturnTime(GlobalSettings.CurrentBidDetails.Domicile, "AM", dutPdDate);
			}
			else
			{
				//returnTime = 1440;
				return DomicileReturnTime(GlobalSettings.CurrentBidDetails.Domicile, "PM", dutPdDate);
			}
		}

		private DeadHeadParameter GetResDhParam(Trip trip, VacationTrip vacTrip, DateTime tripDate, bool rcWasZero)
		{
			DeadHeadParameter resDhParam = new DeadHeadParameter();

			bool isVof = vacTrip.VacationType == "VOF" ? true : false;
			bool vofSplit9 = (vacTrip.VDvsRCdata.RCcalcFlightSeqNum == 9);
			bool vobSplit0 = (vacTrip.VDvsRCdata.RCcalcFlightSeqNum == 0);
			int resDhDp = Math.Max(0, vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1);
			int resDhFltSeq = 0;
			if (vacTrip.VDvsRCdata.RCcalcFlightSeqNum != 9)
			{
				resDhFltSeq = Math.Max(0, vacTrip.VDvsRCdata.RCcalcFlightSeqNum - 1);
			}
			else
			{
				//resDhFltSeq = trip.DutyPeriods[Math.Max(0, vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1)].Flights.Count - 1;
				// first leg next day
				resDhFltSeq = 0;
				resDhDp += 1;
			}
			DateTime today = tripDate.AddDays(resDhDp);
			DateTime tomorrow = today.AddDays(1);
			DateTime yesterday = today.AddDays(-1);
			string domicile = GlobalSettings.CurrentBidDetails.Domicile;
			DutyPeriod dp = trip.DutyPeriods[resDhDp];
			Flight flt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
			bool getReleaseTime = true;
			int legs = Math.Max(1, isVof ? resDhFltSeq : dp.Flights.Count);
			int depTime = 0;
			int arrTime = 0;
			int rptTime = 0;

			resDhParam.DepSta = isVof ? domicile : trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].DepSta;

			if (!rcWasZero)
			{
				// or statement added to if to accomodate 1 leg days that are split, but Split is not set due to just 1 VO leg
				//if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split" || (isVof && vacTrip.VDvsRCdata.RCcalcFlightSeqNum !=9))
				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					// all is OK, CA8X, CAA3

					rptTime = GetReportOrReleaseInMinutesFromOther(dp.ReleaseTime % 1440, 3, today, !getReleaseTime);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(rptTime % 1440 + GlobalSettings.show1stDay);
					resDhParam.ArrSta = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].ArrSta;
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					//arrTime = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].ArrTime; 
					arrTime = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq + 1].DepTime;
					arrTime = AfterMidnightLandingTime(arrTime % 1440);
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(arrTime);
					//resDhParam.RefFlt = resDhFltSeq == 0 ? dp.Flights[0] : flt;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[vacTrip.VDvsRCdata.RCcalcFlightSeqNum];
					resDhParam.Date = today;
				}

				// this next case occurs when the duty periods are VD and VA, NOT split and NO VO
				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "VD")
				{
					// was here from CA9D, all is OK
					// change aftermidnight constant, all is OK, all is OK CA9I

					resDhParam.Date = tomorrow;
					resDhParam.RefFlt = trip.DutyPeriods[vacTrip.VDvsRCdata.RCcalcDutyPeriod-1].Flights[0];
					var refDutPd = trip.DutyPeriods[vacTrip.VDvsRCdata.RCcalcDutyPeriod-1];
					rptTime = GetReportOrReleaseInMinutesFromOther(refDutPd.ReleaseTime % 1440, 3, today, !getReleaseTime);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(rptTime % 1440 + GlobalSettings.show1stDay);
					resDhParam.ArrSta = resDhParam.RefFlt.DepSta;
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					//arrTime = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].ArrTime; 
					arrTime = resDhParam.RefFlt.DepTime % 1440;
					arrTime = AfterMidnightLandingTime(arrTime % 1440);
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(arrTime);
					resDhParam.ConnectTime = GlobalSettings.connectTime;
				}


				if (isVof && vofSplit9)     // this means the resDh is first thing on next day 
				{
					// all is OK, CA9F, CA9T, CA9P

					rptTime = GetReportOrReleaseInMinutesFromOther(dp.ReleaseTime % 1440, 3, today, !getReleaseTime);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(rptTime % 1440 + GlobalSettings.show1stDay);
					resDhParam.ArrSta = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq].DepSta;
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					arrTime = trip.DutyPeriods[resDhDp].Flights[0].DepTime;
					arrTime = AfterMidnightLandingTime(arrTime % 1440);
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(arrTime);
					resDhParam.RefFlt = resDhFltSeq == 0 ? dp.Flights[0] : flt;
					resDhParam.Date = today;
					resDhParam.ConnectTime = GlobalSettings.connectTime;

				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					// been here, all is OK, CABM, CACT

					depTime = dp.Flights[Math.Max(0, resDhFltSeq - 1)].ArrTime;
					arrTime = GetReportOrReleaseInMinutesFromOther(dp.DepTimeFirstLeg - (resDhDp == 0 ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay), 1, today, getReleaseTime);
					resDhParam.DepSta = flt.DepSta;
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(depTime % 1440));
					resDhParam.ConnectTime = GlobalSettings.connectTime;
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(arrTime % 1440) - GlobalSettings.debrief);
					resDhParam.RefFlt = dp.Flights[Math.Max(0, resDhFltSeq - 1)];
					resDhParam.Date = today;

				}

				//if ((!isVof && vobSplit0) || (!isVof && vacTrip.VDvsRCdata.RCcalcFlightSeqNum<=1))   
				if (!isVof && vobSplit0)   // should check for deadhead the day prior and use last flight as ref flt
				{
					// all is OK, CACE, CACG

					resDhParam.RefFlt = dp.Flights[Math.Max(0, resDhFltSeq - 1)];
					var refFlt = trip.DutyPeriods[resDhDp - 1].Flights[trip.DutyPeriods[resDhDp - 1].Flights.Count - 1];
					var reportTime = trip.DutyPeriods[resDhDp - 1].DepTimeFirstLeg - (resDhDp == 1 ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay);
					resDhParam.RefFlt = refFlt;
					resDhParam.Date = yesterday;
					resDhParam.DepSta = refFlt.ArrSta;
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					//resDhParam.DepTimeHhMm = WBidHelper.ConvertMinutesToHhhmmInt(refFlt.ArrTime % 1440);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(refFlt.ArrTime % 1440));
					resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(GetReportOrReleaseInMinutesFromOther(reportTime % 1440, 1, resDhParam.Date, getReleaseTime) - GlobalSettings.debrief);
					resDhParam.ConnectTime = GlobalSettings.connectTime;
					//resDhParam.Date = today;
				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "VD" && !vobSplit0)
				{
					// all is OK, CAC1, CAC3, CACL

					resDhParam.RefFlt = dp.Flights[Math.Max(0, resDhFltSeq - 1)];
					var refFlt = trip.DutyPeriods[resDhDp - 1].Flights[trip.DutyPeriods[resDhDp - 1].Flights.Count - 1];
					var reportTime = trip.DutyPeriods[resDhDp - 1].DepTimeFirstLeg - (resDhDp == 1 ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay);
					resDhParam.RefFlt = refFlt;
					resDhParam.Date = yesterday;
					resDhParam.DepSta = refFlt.ArrSta;
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					//resDhParam.DepTimeHhMm = WBidHelper.ConvertMinutesToHhhmmInt(refFlt.ArrTime % 1440);
					resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(AfterMidnightLandingTime(refFlt.ArrTime % 1440));
					resDhParam.ArrTimeHhMm = Math.Min(2700, VacationHelper.ConvertMinutesToHhhmmInt(GetReportOrReleaseInMinutesFromOther(reportTime % 1440, 1, resDhParam.Date, getReleaseTime) - GlobalSettings.debrief));
					resDhParam.ConnectTime = GlobalSettings.connectTime;
					//resDhParam.Date = today;
				}
			}
			else       // since rcWasZero, Vof deadheads day before (yesterday) and Vob deadheads next day (tomorrow)
			{
				// should limit for AM or PM for performance issues
				DutyPeriod tempDp = trip.DutyPeriods[Math.Max(0, vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1)];

				var refFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
				var arrSta = isVof ? refFlt.DepSta : GlobalSettings.CurrentBidDetails.Domicile;
				var depSta = isVof ? GlobalSettings.CurrentBidDetails.Domicile : refFlt.DepSta;

				var midDay = tempDp.DepTimeFirstLeg % 1440 + (tempDp.LandTimeLastLeg - tempDp.DepTimeFirstLeg) / 2;
				int amCentroid = 690;
				int pmCentroid = 1110;
				string dutyPdAmPm = (midDay - amCentroid) < (pmCentroid - midDay) ? "AM" : "PM";

				//int earliestPlusMinus12 = getEarliestPlusMinus12(isVof);
				int earliestTakeOffForCityInMinutes = GetEarliestTakeOffForCityInMinutes(depSta, resDhParam.Date);
				int latestArrivalInMinutes = GetLatestArrivalForCityInMinutes(arrSta, resDhParam.Date);

				// shorten the range of search by 8 hours for performance
				//earliestTakeOffForCityInMinutes = dutyPdAmPm == "AM" ? earliestTakeOffForCityInMinutes : earliestTakeOffForCityInMinutes + 480;
				//latestArrivalInMinutes = dutyPdAmPm == "AM" ? latestArrivalInMinutes - 480 : latestArrivalInMinutes;

				// frank add 1/8/2015  --  window is not wide enough to see potential deadheads
				earliestTakeOffForCityInMinutes = dutyPdAmPm == "AM" ? earliestTakeOffForCityInMinutes : earliestTakeOffForCityInMinutes + 300;
				latestArrivalInMinutes = dutyPdAmPm == "AM" ? latestArrivalInMinutes - 240 : latestArrivalInMinutes;
				// get yesterday or tomorrow for the duty period
				int rcZeroRefDutPd = (vacTrip.VDvsRCdata.RCcalcDutyPeriod - 1);
				rcZeroRefDutPd = isVof ? rcZeroRefDutPd - 1 : rcZeroRefDutPd + 1;

				// flesh out -- may need a vacation type for "VO"
				if (isVof && vofSplit9)
				{
					// all is OK, CA9F, CA9P

					resDhParam.Date = yesterday;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					//resDhParam.ArrSta = resDhParam.RefFlt.ArrSta;
					resDhParam.ArrSta = resDhParam.RefFlt.DepSta;
				}

				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "VD")
				{
					// all is OK, CA9D, CA9I

					resDhParam.Date = today;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.ArrSta = resDhParam.RefFlt.ArrSta;
				}

				if (isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					//resDhParam.Date = today;
					// all is OK CA9H, CA9A

					resDhParam.Date = yesterday;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.DepSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.ArrSta = resDhParam.RefFlt.ArrSta;
				}

				if (!isVof && vobSplit0)
				{
					// All is OK, CACE, CACG

					resDhParam.Date = today;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepSta = resDhParam.RefFlt.DepSta;
				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "VD" && !vobSplit0)
				{
					// all is OK CAC1, not OK - changed to DepSta for CAC3, OK for CACL, OK for CAC2

					resDhParam.Date = today;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq];
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepSta = resDhParam.RefFlt.DepSta;
				}

				if (!isVof && vacTrip.DutyPeriodsDetails[resDhDp].VacationType == "Split")
				{
					// been here once CA9P is split 3-2 - 1st flt is VO, 2nd flt is VD, Plt rejoins trip and flies 2nd flight, all is OK, CA9Q
					//resDhParam.Date = today;
					resDhParam.Date = tomorrow;
					resDhParam.RefFlt = trip.DutyPeriods[resDhDp].Flights[resDhFltSeq - 1];
					resDhParam.ArrSta = GlobalSettings.CurrentBidDetails.Domicile;
					resDhParam.DepSta = resDhParam.RefFlt.ArrSta;
				}

				resDhParam.DepTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(earliestTakeOffForCityInMinutes);
				resDhParam.ArrTimeHhMm = VacationHelper.ConvertMinutesToHhhmmInt(latestArrivalInMinutes);
				resDhParam.ConnectTime = GlobalSettings.connectTime;
			}

			return resDhParam;
		}
		private int GetLastLandFromLastOpFlightNoDH(List<Flight> flights)
		{
			int totFlts = flights.Count();
			while (totFlts > 0)
			{
				if (flights[totFlts - 1].DeadHead == false)
					return (flights[totFlts - 1].ArrTime % 1440);
				else
					totFlts = totFlts - 1;
			}
			return (flights[0].ArrTime & 14440);
		}
		private int GetEarliestTakeOffTimeInHhMm(Flight flt, DutyPeriod dp, DateTime dutPdDate, string domicile)  // VOB: this is the earliest takeoff time from domicile to be legal to deadhead to rejoin trip
		{
			// only used in VOB pilot dh calculation

			//int earliestTakeOffFdpInHhMm = 0;
			//int earliestTakeOffDutyInHhMm = 0;
			//int calculatedTakeOffinHhMm = 0;
			// since this is deadheading to rejoin, it is always a 1st day, thus showtime in 60 minutes
			// int showTimeMinutes = dp.DutPerSeqNum == 1 ? GlobalSettings.show1stDay : GlobalSettings.showAfter1stDay;

			// get remaining non deadhead flights in duty period for FAR 117 FDP calculation
			int? remainingFlts = dp.Flights.Count - flt.FlightSeqNum - dp.Flights.Where(x => x.FlightSeqNum >= flt.FlightSeqNum).Count(x => x.DeadHead == true) + 1;
			remainingFlts = Math.Max(1, remainingFlts ?? 1); // even if the remaining flight is a dh, the min is 1 for the FAR 117 table

			var temp = MaxFdpTablePilots((int)remainingFlts);

			//string remFlts = remainingFlts >= 7 ? "7+" : remainingFlts.ToString();
			//DataView dv = new DataView(temp);
			//dv.Sort = remFlts + " desc";
			//temp = dv.ToTable();

			// The following foreach calculates the earliest takoff based upon the landTimeLast leg, remaining flts, and max fdp and returns the result in hhmm

			//foreach (DataRow row in temp.Rows) // looks for the maximun fdp by subtracting the fdp from landTimeLastLeg and checking that the takeoff time is valid for the fdp
			//{
			//    decimal fdp = (decimal)row["MaxFdp"];
			//    // test calculated takeoff time converted to domicile time based on AccStartMin and AccStartMax - FAR 117
			//    calculatedTakeOffinHhMm = WBidHelper.ConvertMinutesToHhhmmInt(WBidHelper.DomicileTimeFromHerb(domicile, dutPdDate, ((dp.LandTimeLastLeg - (int)(fdp * 60)) % 1440) + GlobalSettings.show1stDay));
			//    if ((int)row["AccStartMin"] <= calculatedTakeOffinHhMm &
			//        (int)row["AccStartMax"] >= calculatedTakeOffinHhMm)
			//    {
			//        // the following time is not domicile time as the nextwork planning data is all herb time
			//        earliestTakeOffFdpInHhMm = WBidHelper.ConvertMinutesToHhhmmInt(((dp.LandTimeLastLeg - (int)(fdp * 60)) % 1440) + GlobalSettings.show1stDay); ;
			//        break;
			//    }
			//}

			//temp = GlobalSettings.MaxDutyDayTablePilots();
			//dv = new DataView(temp);
			//dv.Sort = "MaxDutyDay desc";

			//foreach (DataRow row in temp.Rows) // looks for the maximun fdp by subtracting the fdp from landTimeLastLeg and checking that the takeoff time is valid for the fdp
			//{
			//    decimal duty = (decimal)row["MaxDutyDay"];
			//    // does not accomodate international yet, but for duty we include showtime and release time
			//    calculatedTakeOffinHhMm = WBidHelper.ConvertMinutesToHhhmmInt(WBidHelper.DomicileTimeFromHerb(domicile, dutPdDate, (dp.LandTimeLastLeg - (int)(duty * 60)) % 1440 + 
			//                                GlobalSettings.show1stDay + GlobalSettings.debrief));
			//    if ((int)row["ReportMin"] <= calculatedTakeOffinHhMm &
			//        (int)row["ReportMax"] >= calculatedTakeOffinHhMm)
			//    {
			//        // the following time is not domicile time as the nextwork planning data is all herb time
			//        //earliestTakeOffDutyInHhMm = WBidHelper.ConvertMinutesToHhhmmInt(dp.LandTimeLastLeg - (int)duty * 60 + showTimeMinutes + dp.ReleaseTime);
			//        earliestTakeOffDutyInHhMm = WBidHelper.ConvertMinutesToHhhmmInt((dp.ReleaseTime - ((int)duty * 60)) % 1440 + GlobalSettings.show1stDay);
			//        break;
			//    }
			//}

			string legColumn = FdpLegColumn((int)remainingFlts);

			//frank change 6-21-2021 to accomdate dh not being counted in FDP time

			// var domLandMinutes = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate, (dp.ReleaseTime % 1440) - GlobalSettings.debrief);

			//Frank VAc--need method to get lat landing for operating flights --no dh
			//iterate through dp.flights to see if a dh exists
			var domLandMinutes = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate, GetLastLandFromLastOpFlightNoDH(dp.Flights));
			//end Frank change 6/21/2021

			PropertyInfo pinfo = typeof(MasterFdpTable).GetProperty(legColumn);

			// var getByReflection = listEmps.Where(x => ((int)pinfo.GetValue(x)) > 3).ToList();

			
			if (domLandMinutes < 300)
				domLandMinutes = AfterMidnightLandingTime(domLandMinutes);

			//var fdpReportMinutesDom = MasterFdpTable().AsEnumerable().Where(x => x.Field<int>("AccStartMax") / 100 * 60 +
			//                                                                              (int)(x.Field<int>("AccStartMax") % 100) +
			//                                                                              (int)(x.Field<decimal>(legColumn) * 60) >= domLandMinutes)
			//                                                                        .Select(x => x.Field<int>("AccStartMin") / 100 * 60).FirstOrDefault();

			var fdpReportMinutesDom = MasterFdpTable().FirstOrDefault(x => x.AccStartMax / 100 * 60 +
				(int)(x.AccStartMax % 100) +
				(int)(((decimal)pinfo.GetValue(x,null)) * 60) >= domLandMinutes).AccStartMin / 100 * 60;


			//var fdpMinutes = MasterFdpTable().AsEnumerable().Where(x => x.Field<int>("AccStartMax") / 100 * 60 +
			//                                                                              (int)(x.Field<int>("AccStartMax") % 100) +
			//                                                                              (int)(x.Field<decimal>(legColumn) * 60) >= domLandMinutes)
			//                                                                        .Select(x => x.Field<decimal>(legColumn) * 60).FirstOrDefault();

			var fdpMinutes = (decimal)pinfo.GetValue(MasterFdpTable().FirstOrDefault(x => x.AccStartMax / 100 * 60 +
				(int)(x.AccStartMax % 100) +
				(int)(((decimal)pinfo.GetValue(x,null)) * 60) >= domLandMinutes),null) * 60;
			//.Select(x => x.Field<decimal>(legColumn) * 60).FirstOrDefault();

			var reportFdpMinutesDom = Math.Max(fdpReportMinutesDom, domLandMinutes - fdpMinutes);

			// Duty

			var domRlsMinutes = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate, (dp.ReleaseTime % 1440));
			if (domRlsMinutes < 300)
				domRlsMinutes = AfterMidnightLandingTime(domRlsMinutes);

			//var dutyReportMinutesDom = MaxDutyDayTablePilots().AsEnumerable().Where(x => x.Field<int>("ReportMax") / 100 * 60 +
			//                                                                                      (int)(x.Field<int>("ReportMax") % 100) +
			//                                                                                      (int)(x.Field<decimal>("MaxDutyDay") * 60) >= domRlsMinutes)
			//                                                                                .Select(x => x.Field<int>("ReportMin") / 100 * 60).FirstOrDefault();
			var dutyReportMinutesDom = MaxDutyDayTablePilots().Where(x => x.ReportMax / 100 * 60 +
				(int)(x.ReportMax % 100) +
				(int)(x.MaxDutyDay * 60) >= domRlsMinutes)
				.Select(y => y.ReportMin / 100 * 60).FirstOrDefault();

			//var dutyMinutes = GlobalSettings.MaxDutyDayTablePilots().AsEnumerable().Where(x => x.Field<int>("ReportMax") / 100 * 60 + (int)(x.Field<decimal>("MaxDutyDay") * 60) >= domRlsMinutes)
			//                    .Select(x => x.Field<decimal>("MaxDutyDay") * 60).FirstOrDefault();
			//var dutyMinutes = MaxDutyDayTablePilots().AsEnumerable().Where(x => x.Field<int>("ReportMax") / 100 * 60 +
			//                                                                             (int)(x.Field<int>("ReportMax") % 100) +
			//                                                                             (int)(x.Field<decimal>("MaxDutyDay") * 60) >= domRlsMinutes)
			//                                                                       .Select(x => x.Field<decimal>("MaxDutyDay") * 60).FirstOrDefault();

			var dutyMinutes = MaxDutyDayTablePilots().Where(x => x.ReportMax / 100 * 60 +
				(int)(x.ReportMax % 100) +
				(int)(x.MaxDutyDay * 60) >= domRlsMinutes)
				.Select(y => y.MaxDutyDay * 60).FirstOrDefault();


			var reportDutyMinutesDom = Math.Max(dutyReportMinutesDom, domRlsMinutes - dutyMinutes);

			var reportHerbMinutes = ConvertToHerbFromDomTime(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate, (int)Math.Max(reportFdpMinutesDom, reportDutyMinutesDom));

			var earliestTakeOfHhMm = VacationHelper.ConvertMinutesToHhhmmInt(reportHerbMinutes + GlobalSettings.show1stDay);

			return earliestTakeOfHhMm;
		}

		private int GetLatestLandTime(Flight flt, DutyPeriod dp, DateTime dutPdDate, string domicile, int showTime, VacationTrip vacTrip)  // VOF: this is the latest time back at domicile to be legal to deadhead from trip to domicile
		{
			// this method is only used in VOF for pilots deadheading to domicile

			int lastLandTimeFdpInHhMm = 0;
			int lastLandTimeDutyInHhMm = 0;
			int calculatedTakeOffTimeInHhMm = 0;
			int maxLandTimeInHhMm = DomicileTimeFromHerb(domicile, dutPdDate, (dp.ShowTime % 1440)) <= 660 ? 1930 : 2400;
			// now convert maxLandTimeInHhMm to herb time
			var maxLandTimeMinutesHerb = ConvertToHerbFromDomTime(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate,
				(maxLandTimeInHhMm / 100 * 60 + maxLandTimeInHhMm % 100));
			var maxLandHebInHhMm = VacationHelper.ConvertMinutesToHhhmmInt(maxLandTimeMinutesHerb);

			int totalFlts = flt.FlightSeqNum;

			var reportInHhMm = VacationHelper.ConvertMinutesToHhhmmInt(DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, dutPdDate, dp.ShowTime % 1440));
			//var fdpHours = MaxFdpTablePilots(totalFlts).AsEnumerable()
			//                .Where(x => x.Field<int>("AccStartMin") <= reportInHhMm && x.Field<int>("AccStartMax") >= reportInHhMm)
			//                .Select(x => x.Field<decimal>("MaxFdp")).FirstOrDefault();

			var fdpHours = MaxFdpTablePilots(totalFlts)
				.Where(x => x.AccStartMin <= reportInHhMm && x.AccStartMax >= reportInHhMm)
				.Select(y => y.MaxFdp).FirstOrDefault();


			//var dutyHours = MaxDutyDayTablePilots().AsEnumerable()
			//                .Where(x => x.Field<int>("ReportMin") <= reportInHhMm && x.Field<int>("ReportMax") >= reportInHhMm)
			//                .Select(x => x.Field<decimal>("MaxDutyDay")).FirstOrDefault();

			var dutyHours = MaxDutyDayTablePilots()
				.Where(x => x.ReportMin <= reportInHhMm && x.ReportMax >= reportInHhMm)
				.Select(y => y.MaxDutyDay).FirstOrDefault();

			// I need to accomodate landings after midnight 2520 etc.
			int lastLandTimeFdpInMinutes = ((dp.ShowTime + (int)(fdpHours * 60)) % 1440);
			lastLandTimeFdpInMinutes += (showTime % 1440) > lastLandTimeFdpInMinutes ? 1440 : 0;

			lastLandTimeFdpInHhMm = VacationHelper.ConvertMinutesToHhhmmInt(lastLandTimeFdpInMinutes);

			int lastLandTimeDutyInMinutes = (((dp.ShowTime + (int)(dutyHours * 60)) % 1440) - GlobalSettings.debrief);  // duty includes show and release time in calculation
			// if after midnight, we add 1440 minutes
			lastLandTimeDutyInMinutes += (showTime % 1440) > lastLandTimeDutyInMinutes ? 1440 : 0;

			lastLandTimeDutyInHhMm = VacationHelper.ConvertMinutesToHhhmmInt(lastLandTimeDutyInMinutes);

			// FDP does not come into play for pilot deadhead back to domicile

			return Math.Min(lastLandTimeDutyInHhMm, maxLandHebInHhMm);

		}

		private int GetEarliestPlusMinus12(bool isVof)
		{
			// this method returns the ealiest takeoff time either a day before or after from the dh flight an hard coding 12 hours rest
			int refTime = 0;

			return refTime;
		}

		private int GetEarliestTakeOffForCityInMinutes(string depSta, DateTime date)
		{
			var flts = GlobalSettings.FlightRouteDetails;

			return flts.Where(x => x.Orig == depSta).OrderBy(x => x.Cdep).Select(x => x.Cdep).FirstOrDefault();

		}

		private int GetLatestArrivalForCityInMinutes(string arrSta, DateTime date)
		{
			var flts = GlobalSettings.FlightRouteDetails;

			return flts.Where(x => x.Dest == arrSta).OrderByDescending(x => x.Carr).Select(x => x.Carr).FirstOrDefault();
		}

		private int GetReportOrReleaseInMinutesFromOther(int minutesInHerb, int legs, DateTime date, bool getReleaseHhMm)
		{
			// do FDP calculation and Duty calculation
			string legsFlown = Math.Max(1, legs).ToString();

			// insure minutes are based on day
			minutesInHerb = minutesInHerb % 1440;
			minutesInHerb = AfterMidnightLandingTime(minutesInHerb);

			// adjust minutes for domicile
			//var domRlsMinutes = WBidHelper.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, minutesInHerb % 1440);
			var domRlsMinutes = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, minutesInHerb);
			var domRptMinutes = DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, minutesInHerb);

			if (getReleaseHhMm)
			{
				// use domicile time for FAR-117 legalities
				// this may be wrong
				//int reportInHhMm = WBidHelper.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, date, WBidHelper.ConvertMinutesToHhhmmInt(minutes % 1440));
				//int reportInHhMm = WBidHelper.ConvertMinutesToHhhmmInt(minutesInHerb % 1440);
				int reportInHhMm = VacationHelper.ConvertMinutesToHhhmmInt(domRptMinutes);

				//var fdpHours = MaxFdpTablePilots(legs).AsEnumerable()
				//            .Where(x => x.Field<int>("AccStartMin") <= reportInHhMm && x.Field<int>("AccStartMax") >= reportInHhMm)
				//            .Select(x => x.Field<decimal>("MaxFdp")).FirstOrDefault();

				var fdpHours = MaxFdpTablePilots(legs)
					.Where(x => x.AccStartMin <= reportInHhMm && x.AccStartMax >= reportInHhMm)
					.Select(y => y.MaxFdp).FirstOrDefault();

				//var dutyHours = MaxDutyDayTablePilots().AsEnumerable()
				//            .Where(x => x.Field<int>("ReportMin") <= reportInHhMm && x.Field<int>("ReportMax") >= reportInHhMm)
				//            .Select(x => x.Field<decimal>("MaxDutyDay")).FirstOrDefault();
				var dutyHours = MaxDutyDayTablePilots()
					.Where(x => x.ReportMin <= reportInHhMm && x.ReportMax >= reportInHhMm)
					.Select(y => y.MaxDutyDay).FirstOrDefault();

				//                var releaseMinutes = ((int)((decimal)fdpHours * 60) + GlobalSettings.debrief) < (dutyHours * 60) ?
				//                                (int)((decimal)fdpHours * 60) + GlobalSettings.debrief + minutesInHerb :
				//                                (int)((decimal)dutyHours * 60 + minutesInHerb);
				var releaseMinutes = (int)((decimal)dutyHours * 60 + minutesInHerb);
				//return WBidHelper.ConvertMinutesToHhhmmInt(releaseMinutes);
				//return releaseMinutes;
				return Math.Min(releaseMinutes, GlobalSettings.LastLandingMinus1440 + 1440);
			}
			else
			{
				// Algorithm for calculating the earliest report time is tricky
				// with a report time of 0450 you would have an fdp = 10 hours  (0400-2359 = 10 hours)
				// however, a report 10 minutes later at 0500 you would have an fdp = 12 hours  (0500-1659 = 12 hours)
				// *****
				// example with Duty Period
				// lets say dutyHours = 12 and reportMinHhMm = 0400 and release time is 1650
				//      in this case, the earliest report would be Math.Max(reportMinHhMm, (releaseTime - dutyHours)
				//      which would be Math.Max(0400, (1650 - 1200 = 0450)) = 0450
				// lets say dutyHours = 12 and reportMinHhMm = 0400 and release time is 1550
				//      in this case, the earliest report would be Math.Max(reportMinHhMm, (releaseTime - dutyHours)
				//      which would be Math.Max(0400, (1550 - 1200 = 0350)) = 0400

				string legColumn = FdpLegColumn(legs);


				PropertyInfo pinfo = typeof(MasterFdpTable).GetProperty(legColumn);

				var domLandMinutes = domRlsMinutes - GlobalSettings.debrief;

				//var fdpReportMinutesDom = MasterFdpTable().AsEnumerable().Where(x => x.Field<int>("AccStartMax") / 100 * 60 +
				//                                                                              (int)(x.Field<int>("AccStartMax") % 100) +
				//                                                                              (int)(x.Field<decimal>(legColumn) * 60) >= domLandMinutes)
				//                                                                        .Select(x => x.Field<int>("AccStartMin") / 100 * 60).FirstOrDefault();


				var fdpReportMinutesDom = MasterFdpTable().FirstOrDefault(x => x.AccStartMax / 100 * 60 +
					(int)(x.AccStartMax % 100) +
					(int)(((decimal)pinfo.GetValue(x,null)) * 60) >= domLandMinutes).AccStartMin / 100 * 60;


				//var fdpMinutes = MasterFdpTable().AsEnumerable().Where(x => x.Field<int>("AccStartMax") / 100 * 60 +
				//                                                                              (int)(x.Field<int>("AccStartMax") % 100) +
				//                                                                              (int)(x.Field<decimal>(legColumn) * 60) >= domLandMinutes)
				//                                                                        .Select(x => x.Field<decimal>(legColumn) * 60).FirstOrDefault();


				var fdpMinutes = (decimal)pinfo.GetValue(MasterFdpTable().FirstOrDefault(x => x.AccStartMax / 100 * 60 +
					(int)(x.AccStartMax % 100) +
					(int)(((decimal)pinfo.GetValue(x,null)) * 60) >= domLandMinutes),null) * 60;

				var reportFdpMinutesDom = Math.Max(fdpReportMinutesDom, domLandMinutes - fdpMinutes);

				/***************************************************
                 *  Duty
                 *  ***********************************************/

				// var test = MaxDutyDayTablePilots().AsEnumerable().Skip(3).Select(x => x.Field<decimal>("MaxDutyDay")).FirstOrDefault();
				var test = MaxDutyDayTablePilots().Skip(3).Select(x => x.MaxDutyDay).FirstOrDefault();

				// walk through the math and I will see.

				//var dutyReportMinutesDom = MaxDutyDayTablePilots().AsEnumerable().Where(x => x.Field<int>("ReportMax") / 100 * 60 +
				//                                                                                      (int)(x.Field<int>("ReportMax") % 100) +
				//                                                                                      (int)(x.Field<decimal>("MaxDutyDay") * 60) >= domRlsMinutes)
				//                                                                                .Select(x => x.Field<int>("ReportMin") / 100 * 60).FirstOrDefault();

				var dutyReportMinutesDom = MaxDutyDayTablePilots().Where(x => x.ReportMax / 100 * 60 +
					(int)(x.ReportMax % 100) +
					(int)(x.MaxDutyDay * 60) >= domRlsMinutes)
					.Select(y => y.ReportMin / 100 * 60).FirstOrDefault();

				//var dutyMinutes = MaxDutyDayTablePilots().AsEnumerable().Where(x => x.Field<int>("ReportMax") / 100 * 60 +
				//                                                                                      (int)(x.Field<int>("ReportMax") % 100) +
				//                                                                                      (int)(x.Field<decimal>("MaxDutyDay") * 60) >= domRlsMinutes)
				//                                                                                .Select(x => x.Field<decimal>("MaxDutyDay") * 60).FirstOrDefault();

				var dutyMinutes = MaxDutyDayTablePilots().Where(x => x.ReportMax / 100 * 60 +
					(int)(x.ReportMax % 100) +
					(int)(x.MaxDutyDay * 60) >= domRlsMinutes)
					.Select(y => y.MaxDutyDay * 60).FirstOrDefault();

				var reportDutyMinutesDom = Math.Max(dutyReportMinutesDom, domRlsMinutes - dutyMinutes);

				var reportHerbMinutes = ConvertToHerbFromDomTime(GlobalSettings.CurrentBidDetails.Domicile, date, (int)Math.Max(reportFdpMinutesDom, reportDutyMinutesDom));

				return (int)reportHerbMinutes;
			}
		}

		private int GetEarliestReportFromRelease(int releaseTime, int legs, DateTime date)
		{
			int reportTime = 0;



			return reportTime;
		}




		private List<FdpTable> MaxFdpTablePilots(int legs)
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			List<FdpTable> maxFdpTable = new List<FdpTable>();

			switch (legs)
			{
			case 0:
			case 1:
				maxFdpTable = MaxFdpTable1Leg();
				break;
			case 2:
				maxFdpTable = MaxFdpTable2Leg();
				break;
			case 3:
				maxFdpTable = MaxFdpTable3Leg();
				break;
			case 4:
				maxFdpTable = MaxFdpTable4Leg();
				break;
			case 5:
				maxFdpTable = MaxFdpTable5Leg();
				break;
			case 6:
				maxFdpTable = MaxFdpTable6Leg();
				break;
			case 7:
				maxFdpTable = MaxFdpTable7Leg();
				break;
			}

			return maxFdpTable;
		}

		private List<FdpTable> MaxFdpTable1Leg()
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("MaxFdp", typeof(decimal));

			//maxFdpTable.Rows.Add(0700, 1159, 14);
			//maxFdpTable.Rows.Add(0600, 1259, 13);
			//maxFdpTable.Rows.Add(0500, 2159, 12);
			//maxFdpTable.Rows.Add(2200, 2259, 11);
			//maxFdpTable.Rows.Add(0400, 2359, 10);
			//maxFdpTable.Rows.Add(0000, 2759, 9);

			//return maxFdpTable;

			List<FdpTable> maxFdpTable = new List<FdpTable>()
			{
				new FdpTable(){AccStartMin=0700,AccStartMax=1159,MaxFdp=14},
				new FdpTable(){AccStartMin=0600,AccStartMax=1259,MaxFdp=13},
				new FdpTable(){AccStartMin=0500,AccStartMax=2159,MaxFdp=12},
				new FdpTable(){AccStartMin=2200,AccStartMax=2259,MaxFdp=11},
				new FdpTable(){AccStartMin=0400,AccStartMax=2359,MaxFdp=10},
				new FdpTable(){AccStartMin=0000,AccStartMax=2759,MaxFdp=9}

			};


			return maxFdpTable;
		}

		private List<FdpTable> MaxFdpTable2Leg()
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("MaxFdp", typeof(decimal));

			//maxFdpTable.Rows.Add(0700, 1159, 14);
			//maxFdpTable.Rows.Add(0600, 1259, 13);
			//maxFdpTable.Rows.Add(0500, 2159, 12);
			//maxFdpTable.Rows.Add(2200, 2259, 11);
			//maxFdpTable.Rows.Add(0400, 2359, 10);
			//maxFdpTable.Rows.Add(0000, 2759, 9);

			List<FdpTable> maxFdpTable = new List<FdpTable>()
			{
				new FdpTable(){AccStartMin=0700,AccStartMax=1159,MaxFdp=14},
				new FdpTable(){AccStartMin=0600,AccStartMax=1259,MaxFdp=13},
				new FdpTable(){AccStartMin=0500,AccStartMax=2159,MaxFdp=12},
				new FdpTable(){AccStartMin=2200,AccStartMax=2259,MaxFdp=11},
				new FdpTable(){AccStartMin=0400,AccStartMax=2359,MaxFdp=10},
				new FdpTable(){AccStartMin=0000,AccStartMax=2759,MaxFdp=9}

			};

			return maxFdpTable;
		}

		private List<FdpTable> MaxFdpTable3Leg()
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("MaxFdp", typeof(decimal));

			//maxFdpTable.Rows.Add(0700, 1259, 13);
			//maxFdpTable.Rows.Add(0500, 1659, 12);
			//maxFdpTable.Rows.Add(1700, 2159, 11);
			//maxFdpTable.Rows.Add(0400, 2359, 10);
			//maxFdpTable.Rows.Add(0000, 2759, 9);
			List<FdpTable> maxFdpTable = new List<FdpTable>()
			{
				new FdpTable(){AccStartMin=0700,AccStartMax=1259,MaxFdp=13},
				new FdpTable(){AccStartMin=0500,AccStartMax=1659,MaxFdp=12},
				new FdpTable(){AccStartMin=1700,AccStartMax=2159,MaxFdp=11},
				new FdpTable(){AccStartMin=0400,AccStartMax=2359,MaxFdp=10},
				new FdpTable(){AccStartMin=0000,AccStartMax=2759,MaxFdp=9}

			};

			return maxFdpTable;
		}

		private List<FdpTable> MaxFdpTable4Leg()
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("MaxFdp", typeof(decimal));

			//maxFdpTable.Rows.Add(0700, 1259, 13);
			//maxFdpTable.Rows.Add(0500, 1659, 12);
			//maxFdpTable.Rows.Add(1700, 2159, 11);
			//maxFdpTable.Rows.Add(0400, 2259, 10);
			//maxFdpTable.Rows.Add(0000, 2759, 9);
			List<FdpTable> maxFdpTable = new List<FdpTable>()
			{
				new FdpTable(){AccStartMin=0700,AccStartMax=1259,MaxFdp=13},
				new FdpTable(){AccStartMin=0500,AccStartMax=1659,MaxFdp=12},
				new FdpTable(){AccStartMin=1700,AccStartMax=2159,MaxFdp=11},
				new FdpTable(){AccStartMin=0400,AccStartMax=2259,MaxFdp=10},
				new FdpTable(){AccStartMin=0000,AccStartMax=2759,MaxFdp=9}

			};
			return maxFdpTable;
		}

		private List<FdpTable> MaxFdpTable5Leg()
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("MaxFdp", typeof(decimal));

			//maxFdpTable.Rows.Add(0700, 1259, 12.5);
			//maxFdpTable.Rows.Add(0500, 1659, 11.5);
			//maxFdpTable.Rows.Add(1700, 2159, 10);
			//maxFdpTable.Rows.Add(0000, 2759, 9);
			List<FdpTable> maxFdpTable = new List<FdpTable>()
			{
				new FdpTable(){AccStartMin=0700,AccStartMax=1259,MaxFdp=12.5m},
				new FdpTable(){AccStartMin=0500,AccStartMax=1659,MaxFdp=11.5m},
				new FdpTable(){AccStartMin=1700,AccStartMax=2159,MaxFdp=10},
				new FdpTable(){AccStartMin=0000,AccStartMax=2759,MaxFdp=9}

			};
			return maxFdpTable;
		}

		private List<FdpTable> MaxFdpTable6Leg()
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("MaxFdp", typeof(decimal));

			//maxFdpTable.Rows.Add(0700, 1259, 12);
			//maxFdpTable.Rows.Add(0500, 1659, 11);
			//maxFdpTable.Rows.Add(0000, 2759, 9);
			List<FdpTable> maxFdpTable = new List<FdpTable>()
			{
				new FdpTable(){AccStartMin=0700,AccStartMax=1259,MaxFdp=12},
				new FdpTable(){AccStartMin=0500,AccStartMax=1659,MaxFdp=11},
				new FdpTable(){AccStartMin=0000,AccStartMax=2759,MaxFdp=9}

			};
			return maxFdpTable;
		}

		private List<FdpTable> MaxFdpTable7Leg()
		{
			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("MaxFdp", typeof(decimal));

			//maxFdpTable.Rows.Add(0700, 1259, 11.5);
			//maxFdpTable.Rows.Add(0500, 1659, 10.5);
			//maxFdpTable.Rows.Add(0000, 2759, 9);

			List<FdpTable> maxFdpTable = new List<FdpTable>()
			{
				new FdpTable(){AccStartMin=0700,AccStartMax=1259,MaxFdp=11.5m},
				new FdpTable(){AccStartMin=0500,AccStartMax=1659,MaxFdp=10.5m},
				new FdpTable(){AccStartMin=0000,AccStartMax=2759,MaxFdp=9}

			};

			return maxFdpTable;
		}
		private List<DutyDayTable> MaxDutyDayTablePilots()
		{
			// returns the maximum duty day table.  ReportStart and ReportFinish are in hhmm format.  The MaxDutyDay is in decimal hours.

			//DataTable maxDutyDayTable = new DataTable();

			//maxDutyDayTable.Columns.Add("ReportMin", typeof(int));
			//maxDutyDayTable.Columns.Add("ReportMax", typeof(int));
			//maxDutyDayTable.Columns.Add("MaxDutyDay", typeof(decimal));

			////maxDutyDayTable.Rows.Add(0600, 1059, 13);
			////maxDutyDayTable.Rows.Add(0400, 1459, 12);
			////maxDutyDayTable.Rows.Add(1500, 1959, 11);
			////maxDutyDayTable.Rows.Add(0200, 1959, 10);
			////maxDutyDayTable.Rows.Add(0000, 2559, 9);

			//maxDutyDayTable.Rows.Add(0000, 0159, 9);
			//maxDutyDayTable.Rows.Add(0200, 0359, 10);
			//maxDutyDayTable.Rows.Add(0400, 0559, 12);
			//maxDutyDayTable.Rows.Add(0600, 1059, 13);
			//maxDutyDayTable.Rows.Add(1100, 1459, 12);
			//maxDutyDayTable.Rows.Add(1500, 1959, 11);
			//maxDutyDayTable.Rows.Add(2000, 2559, 9);


			List<DutyDayTable> maxDutyDayTable = new List<DutyDayTable>()
			{
				new DutyDayTable(){ReportMin=0000,ReportMax=0159,MaxDutyDay=9},
				new DutyDayTable(){ReportMin=0200,ReportMax=0359,MaxDutyDay=10},
				new DutyDayTable(){ReportMin=0400,ReportMax=0559,MaxDutyDay=12},
				new DutyDayTable(){ReportMin=0600,ReportMax=1059,MaxDutyDay=13},
				new DutyDayTable(){ReportMin=1100,ReportMax=1459,MaxDutyDay=12},
				new DutyDayTable(){ReportMin=1500,ReportMax=1959,MaxDutyDay=11},
				new DutyDayTable(){ReportMin=2000,ReportMax=2559,MaxDutyDay=9}
			};
			return maxDutyDayTable;
		}

		private List<MasterFdpTable> MasterFdpTable()
		{

			// returns the maximum flight duty period table.  AccStart and AccFinish are in hhmm format.  The fdp based upon legs is in decimal hours

			//DataTable maxFdpTable = new DataTable();

			//maxFdpTable.Columns.Add("AccStartMin", typeof(int));
			//maxFdpTable.Columns.Add("AccStartMax", typeof(int));
			//maxFdpTable.Columns.Add("1leg", typeof(decimal));
			//maxFdpTable.Columns.Add("2leg", typeof(decimal));
			//maxFdpTable.Columns.Add("3leg", typeof(decimal));
			//maxFdpTable.Columns.Add("4leg", typeof(decimal));
			//maxFdpTable.Columns.Add("5leg", typeof(decimal));
			//maxFdpTable.Columns.Add("6leg", typeof(decimal));
			//maxFdpTable.Columns.Add("7leg", typeof(decimal));

			//maxFdpTable.Rows.Add(0000, 0359, 9, 9, 9, 9, 9, 9, 9);
			//maxFdpTable.Rows.Add(0400, 0459, 10, 10, 10, 10, 9, 9, 9);
			//maxFdpTable.Rows.Add(0500, 0559, 12, 12, 12, 12, 11.5, 11, 10.5);
			//maxFdpTable.Rows.Add(0600, 0659, 13, 13, 12, 12, 11.5, 11, 10.5);
			//maxFdpTable.Rows.Add(0700, 1159, 14, 14, 13, 13, 12.5, 12, 11.5);
			//maxFdpTable.Rows.Add(1200, 1259, 13, 13, 13, 13, 12.5, 12, 11.5);
			//maxFdpTable.Rows.Add(1300, 1659, 12, 12, 12, 12, 11.5, 11, 10.5);
			//maxFdpTable.Rows.Add(1700, 2159, 12, 12, 11, 11, 10, 9, 9);
			//maxFdpTable.Rows.Add(2200, 2259, 11, 11, 10, 10, 9, 9, 9);
			//maxFdpTable.Rows.Add(2300, 2359, 10, 10, 10, 9, 9, 9, 9);
			//maxFdpTable.Rows.Add(2400, 2759, 9, 9, 9, 9, 9, 9, 9);


			List<MasterFdpTable> maxFdpTable = new List<MasterFdpTable>() 
			{
				new MasterFdpTable(){AccStartMin=0000,AccStartMax=0359,_1leg=9,_2leg=9,_3leg=9,_4leg=9,_5leg=9,_6leg=9,_7leg=9}, 
				new MasterFdpTable(){AccStartMin=0400,AccStartMax=0459,_1leg=10,_2leg=10,_3leg=10,_4leg=10,_5leg=9,_6leg=9,_7leg=9}, 
				new MasterFdpTable(){AccStartMin=0500,AccStartMax=0559,_1leg=12,_2leg=12,_3leg=12,_4leg=12,_5leg=11.5m,_6leg=11,_7leg=10.5m}, 
				new MasterFdpTable(){AccStartMin=0600,AccStartMax=0659,_1leg=13,_2leg=13,_3leg=12,_4leg=12,_5leg=11.5m,_6leg=11,_7leg=10.5m}, 
				new MasterFdpTable(){AccStartMin=0700,AccStartMax=1159,_1leg=14,_2leg=14,_3leg=13,_4leg=13,_5leg=12.5m,_6leg=12,_7leg=11.5m}, 
				new MasterFdpTable(){AccStartMin=1200,AccStartMax=1259,_1leg=13,_2leg=13,_3leg=13,_4leg=13,_5leg=12.5m,_6leg=12,_7leg=11.5m}, 
				new MasterFdpTable(){AccStartMin=1300,AccStartMax=1659,_1leg=12,_2leg=12,_3leg=12,_4leg=12,_5leg=11.5m,_6leg=11,_7leg=10.5m}, 
				new MasterFdpTable(){AccStartMin=2200,AccStartMax=2259,_1leg=11,_2leg=11,_3leg=10,_4leg=10,_5leg=9,_6leg=9,_7leg=9}, 
				new MasterFdpTable(){AccStartMin=2300,AccStartMax=2359,_1leg=10,_2leg=10,_3leg=10,_4leg=9,_5leg=9,_6leg=9,_7leg=9}, 
				new MasterFdpTable(){AccStartMin=2400,AccStartMax=2759,_1leg=9,_2leg=9,_3leg=9,_4leg=9,_5leg=9,_6leg=9,_7leg=9}, 

			};


			return maxFdpTable;
		}


		public  string FdpLegColumn(int legs)
		{
			switch (legs)
			{
			case 0:
			case 1: return "_1leg";
			case 2: return "_2leg";
			case 3: return "_3leg";
			case 4: return "_4leg";
			case 5: return "_5leg";
			case 6: return "_6leg";
			case 7: return "_7leg";
			default: return "_7leg";
			}
		}

		public class LegPltSavingsVsResCost
		{
			public decimal PltCost { get; set; }
			public decimal ResCost { get; set; }
			// legSpilt is where the VO starts in front for VOF dutyPeriod and where the VD starts in back for VOB duty period
			public int legSplit { get; set; }
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

	}


}
