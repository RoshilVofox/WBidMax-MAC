using System;

namespace WBid.WBidiPad.Model
{
	public class CAPOutputParameter
	{
		public CAPOutputParameter ()
		{
		}
		public string Domicile { get; set; }
		public string Position { get; set; }
		public decimal? PreviousMonthCap { get; set; }
		public decimal? CurrentMonthCap { get; set; }
	}
}

