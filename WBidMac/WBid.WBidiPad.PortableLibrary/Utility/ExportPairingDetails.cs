using System;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Linq;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary
{
	public class ExportPairingDetails
	{
		public ExportPairingDetails ()
		{
		}
		public AppointmentDetails ExportToOutlook(string tripName, DateTime selectedTripStartDate, string body)
		{
			Trip trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripName);
			if(trip==null)
				trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == tripName+selectedTripStartDate.Day.ToString().PadLeft(2,' '));


			DateTime endDate = selectedTripStartDate.AddDays(trip.DutyPeriods.Count - 1);
			AppointmentDetails appointmentDetails = new AppointmentDetails();
			appointmentDetails.Subject = GenerateSubject(trip);
			//appointmentDetails.StartDate = selectedTripStartDate.Date.ToString ("MM/dd/yyyy");
            appointmentDetails.StartDate = selectedTripStartDate.Date.ToString("d");
            appointmentDetails.StartTime = CalculateStartTimeBasedOnTime(trip,selectedTripStartDate);
			//if the endtime >=24:00 we need to add a day to the endadate. That is why we are passing day as an out pramaeter to the "CalculateEndTimeBasedOnTime" method.
			int day = 0;
			appointmentDetails.EndTime = CalculateEndTimeBasedOnTime(trip,endDate,out day);
			endDate = endDate.AddDays(day);
            //appointmentDetails.EndDate = endDate.Date.ToString("MM/dd/yyyy");
            appointmentDetails.EndDate = endDate.Date.ToString("d");

            appointmentDetails.Body = "REPORT " + CalculateStartTime(trip) + " CST/CDT";
			if (appointmentDetails.StartDateTime > appointmentDetails.EndDateTime)
				appointmentDetails.EndDate = endDate.AddDays(1).Date.ToString ("MM/dd/yyyy");
            appointmentDetails.EndDate = endDate.Date.ToString("d");
            body += Environment.NewLine + Environment.NewLine + "(Note: All times are CST/CDT unless otherwise noted.)";
			if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
			{
				appointmentDetails.Body += " (" + appointmentDetails.StartTime + " Domicile Time)";
				body +=   Environment.NewLine + "(Note: AppointmentDetails times  are in Domicile time).";
			}
			appointmentDetails.Body += Environment.NewLine + Environment.NewLine + body;
			return appointmentDetails;

		}
		/// <summary>
		/// PURPOSE : Generate Trip Appointment Subject
		/// </summary>
		/// <param name="trip"></param>
		/// <returns></returns>
		private string GenerateSubject(Trip trip)
		{



			string subject = string.Empty;
			foreach (DutyPeriod dutyPeriod in trip.DutyPeriods)
			{

				if (subject != string.Empty)
				{
					subject += "/";
				}
				subject += dutyPeriod.ArrStaLastLeg;
			}

			if (GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected)
			{
				subject = trip.TripNum.Substring(0, 4) + " " + subject;
			}

			return subject;
		}
		/// <summary>
		/// PURPOSE : Calculate StartTime
		/// </summary>
		/// <param name="trip"></param>
		/// <returns></returns>
		private string CalculateStartTimeBasedOnTime(Trip trip, DateTime tripStartDate)
		{

			string startTime = string.Empty;
			int hour = 0;
			int minutes = 0;
			int startTimeMinute = 0;
			//int depTimeMinutes =  int.Parse(trip.DepTime);
			int depTimeMinutes =  Helper.ConvertHhmmToMinutes(trip.DepTime);

			//Int16.Parse(trip.DepTime.Substring(0, 2)) * 60 + Int16.Parse(trip.DepTime.Substring(2, 2));
			//startTimeMinute = depTimeMinutes - trip.BriefTime;
			startTimeMinute = trip.DutyPeriods[0].ShowTime;
			if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
			{
				startTimeMinute = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, startTimeMinute);
			}

			hour = startTimeMinute / 60;
			minutes = startTimeMinute % 60;

			if (hour == 24)
			{
				hour = 0;
				startTime = "00";
			}
			else
			{
				startTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
			}
			startTime += ":" + minutes.ToString("d2");

			startTime += ((hour >= 12) ? " PM" : " AM");

			return startTime;

		}


		private string CalculateStartTime(Trip trip)
		{

			string startTime = string.Empty;
			int hour = 0;
			int minutes = 0;
			int startTimeMinute = 0;
			//int depTimeMinutes =  int.Parse(trip.DepTime);
			int depTimeMinutes = ConvertHhmmToMinutes(trip.DepTime);

			//Int16.Parse(trip.DepTime.Substring(0, 2)) * 60 + Int16.Parse(trip.DepTime.Substring(2, 2));
			//  startTimeMinute = depTimeMinutes - trip.BriefTime;
			startTimeMinute = trip.DutyPeriods[0].ShowTime;
			hour = startTimeMinute / 60;
			minutes = startTimeMinute % 60;

			if (hour == 24)
			{
				hour = 0;
				startTime = "00";
			}
			else
			{
				startTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
			}
			startTime += ":" + minutes.ToString("d2");

			startTime += ((hour >= 12) ? " PM" : " AM");

			return startTime;

		}

		/// <summary>
		/// PURPOSE : Calculate EndTime
		/// </summary>
		/// <param name="trip"></param>
		/// <returns></returns>
		private string CalculateEndTimeBasedOnTime(Trip trip, DateTime tripStartDate, out int day)
		{

			string endtTime = string.Empty;
			int hour = 0;
			int minutes = 0;
			int endTimeMinute = 0;
			day = 0;
			//int repTimeMinutes = int.Parse( trip.RetTime)%1440;
			int repTimeMinutes = ConvertHhmmToMinutes(trip.RetTime);
			//Int16.Parse(trip.RetTime.Substring(0, 2)) * 60 + Int16.Parse(trip.RetTime.Substring(2, 2));
			endTimeMinute = repTimeMinutes + trip.DebriefTime;
			if (!GlobalSettings.WBidINIContent.PairingExport.IsCentralTime)
			{
				endTimeMinute = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripStartDate, endTimeMinute);
			}

			if (endTimeMinute >= 1440)
			{
				day = 1;
				endTimeMinute = endTimeMinute % 1440;
			}


			if (endTimeMinute == 0)
			{
				hour = 0;
				minutes = 0;

			}
			else
			{

				hour = endTimeMinute / 60;
				minutes = endTimeMinute % 60;
			}


			//if (hour == 24 || hour==0)
			//{

			//    hour = 0;
			//    endtTime = "00";
			//}
			//else
			//{
			//    endtTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");
			//}

			endtTime = (hour > 12) ? (hour - 12).ToString("d2") : hour.ToString("d2");

			endtTime += ":" + minutes.ToString("d2");

			endtTime += ((hour >= 12) ? " PM" : " AM");

			return endtTime;

		}
		private int ConvertHhmmToMinutes(string hhmm)
		{
			hhmm = hhmm.PadLeft(4, '0');
			int hours = Convert.ToInt32(hhmm.Substring(0, 2));
			int minutes = Convert.ToInt32(hhmm.Substring(2, 2));
			return hours * 60 + minutes;
		}
	}
}

