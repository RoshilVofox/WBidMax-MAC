using System;

namespace WBid.WBidiPad.Model
{
	public class AppointmentDetails
	{

		public string StartDate { get; set; }


		public string StartTime { get; set; }

		public DateTime StartDateTime
		{
			get
			{
				return  DateTime.Parse( StartDate + " " + StartTime);
			}
		}

		public string EndDate { get; set; }

		public string EndTime { get; set; }

		public DateTime EndDateTime
		{
			get
			{
				return DateTime.Parse( EndDate + " " + EndTime);
			}
		}

		public string Subject { get; set; }

		public string Body { get; set; }
	}

}

