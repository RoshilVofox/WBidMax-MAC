using System;
using System.Collections.Generic;

namespace WBid.WBidMac.Mac
{
	public class ConstraintsApplied
	{
		public ConstraintsApplied ()
		{
		}

		public static Dictionary <string,string> ViewTypes = new Dictionary <string, string> () {
			{ "Aircraft Changes" , "View2" },
			{ "Blocks of Days Off" , "View2" },
			{ "Cities-Legs" , "View3" },
			{ "Cmut DHs" , "View4" },
          //  { "Commutability" , "Commutability" },
            { "Commutable Lines - Manual" , "CL" },
			{ "Commutable Lines - Auto" , "Commutable Lines - Auto" },
			{ "Days of the Month" , "DOM" },
			{ "Days of the Week" , "View3" },
			{ "DH - first - last" , "View3" },
			{ "Duty period" , "View2" },
			{ "Equipment Type" , "View3" },
			{ "Flight Time" , "View2" },
			{ "Ground Time" , "View3" },
			{ "Intl – NonConus" , "View2" },
			{ "Legs Per Duty Period" , "View2" },
			{ "Legs Per Pairing" , "View2" },
			{ "Min Pay" , "View2" },
			{ "Number of Days Off" , "View2" },
			{ "Overnight Cities" , "View3" },
			{ "Overnight Cities - Bulk" , "OBLK" },
			{ "PDO" , "View4" },
			{ "Rest" , "View3" },
            { "Start Day" , "View3" },
			{ "Start Day of Week" , "View5" },
            { "Report-Release" , "View6" },
            { "Time-Away-From-Base" , "View2" },
			{ "Trip Length" , "View3" },
			{ "Work Blk Length" , "View3" },
			{ "Work Days" , "View2" },
			{ "3-on-3-off" , "View1" },
			{"Mixed Hard/Reserve","View0" },
		};

		public static List <string> MainList = new List <string> () { "Aircraft Changes", "Blocks of Days Off","Cities-Legs", "Cmut DHs","Commutable Lines - Manual","Commutable Lines - Auto", "Days of the Week", "Days of the Month",
			"DH - first - last", "Duty period", "Equipment Type", "Flight Time", "Ground Time", "Intl – NonConus", "Legs Per Duty Period", "Legs Per Pairing", 
			"Number of Days Off", "Overnight Cities", "Overnight Cities - Bulk", "PDO", "Start Day of Week", "Rest","Start Day","Report-Release","Time-Away-From-Base", "Trip Length", "Work Blk Length", "Work Days",
			"Min Pay", "3-on-3-off","Mixed Hard/Reserve"
		};
		//, "Overlap Days"

		public static Dictionary <string,int> HelpPageNo = new Dictionary <string, int> () {
			{ "Aircraft Changes" , 7 },
			{ "Blocks of Days Off" , 7 },
			{ "Cities-Legs" , 18 },
			{ "Cmut DHs" , 8 },
			{ "Commutable Lines - Manual" , 9 },
			{ "Days of the Month" , 12 },
			{ "Days of the Week" , 12 },
			{ "DH - first - last" , 13 },
			{ "Duty period" , 13 },
			{ "Equipment Type" , 14 },
			{ "Flight Time" , 15 },
			{ "Ground Time" , 15 },
			{ "Intl – NonConus" , 16 },
			{ "Legs Per Duty Period" , 16 },
			{ "Legs Per Pairing" , 17 },
			{ "Min Pay" , 17 },
			{ "Number of Days Off" , 18 },
			{ "Overnight Cities" , 18 },
			{ "Overnight Cities - Bulk" , 19 },
			{ "PDO" , 19 },
			{ "Rest" , 20 },
            { "Start Day" , 20 },
			{ "Start Day of Week" , 20 },
			{ "Time-Away-From-Base" , 21 },
			{ "Trip Length" , 22 },
			{ "Work Blk Length" , 22 },
			{ "Work Days" , 23 },
			{ "3-on-3-off" , 23 },
			{ "Mixed Hard/Reserve" , 23 }
		};

		public static List <string> daysOfWeekConstraints = new List <string> ();
		public static List <string> DhFirstLastConstraints = new List <string> ();
		public static List <string> EQTypeConstraints = new List <string> ();
		public static List <string> OvernightCitiesConstraints = new List <string> ();
		public static List <string> CitiesLegsConstraints = new List <string> ();
		public static List <string> StartDayofWeekConstraints = new List <string> ();
		public static List <string> RestConstraints = new List <string> ();
		public static List <string> TripLengthConstraints = new List <string> ();
		public static List <string> WorkBlockLengthConstraints = new List <string> ();
		public static List <string> CmutDHsConstraints = new List <string> ();
		public static List <string> PDOConstraints = new List <string> ();
		public static List <string> IntlNonConusConstraints = new List <string> ();
        public static List<string> StartDayConstraints = new List<string>();
        public static List<string> ReportReleaseConstraints = new List<string>();

		public static void clearAll ()
		{
			daysOfWeekConstraints.Clear ();
			DhFirstLastConstraints.Clear ();
			EQTypeConstraints.Clear ();
			OvernightCitiesConstraints.Clear ();
			CitiesLegsConstraints.Clear ();
			StartDayofWeekConstraints.Clear ();
			RestConstraints.Clear ();
			TripLengthConstraints.Clear ();
			WorkBlockLengthConstraints.Clear ();
			CmutDHsConstraints.Clear ();
			PDOConstraints.Clear ();
			IntlNonConusConstraints.Clear ();
            StartDayConstraints.Clear();
            ReportReleaseConstraints.Clear();
		}

	}
}

