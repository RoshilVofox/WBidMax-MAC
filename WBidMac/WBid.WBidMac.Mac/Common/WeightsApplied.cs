using System;
using System.Collections.Generic;

namespace WBid.WBidMac.Mac
{
	public class WeightsApplied
	{
		public WeightsApplied ()
		{
		}

		public static Dictionary <string,string> ViewTypes = new Dictionary <string, string> () {
			{ "Aircraft Changes" , "View3" },
			{ "AM/PM" , "View2" },
			{ "Blocks of Days Off" , "View3" },
			{ "Cities-Legs" , "View2" },
			{ "Cmut DHs" , "View3" },
           // { "Commutability" , "Commutability" },
            { "Commutable Lines - Manual" , "CL" },
			{ "Commutable Lines - Auto" , "Commutable Lines - Auto" },
			{ "Days of the Month" , "DOM" },
			{ "Days of the Week" , "DOW" },
			{ "DH - first - last" , "View2" },
			{ "Duty period" , "View3" },
			{ "Equipment Type" , "View3" },
			{ "ETOPS" , "View1" },
			{ "ETOPS-Res" , "View1" },
			{ "Flight Time" , "View3" },
			{ "Ground Time" , "View3" },
			{ "Intl – NonConus" , "View2" },
			{ "Largest Block of Days Off" , "View1" },
			{ "Legs Per Duty Period" , "View3" },
			{ "Legs Per Pairing" , "View3" },
			{ "Normalize Days Off" , "View1" },
			{ "Number of Days Off" , "View2" },
			{ "Overnight Cities" , "View2" },
			{ "Overnight Cities - Bulk" , "OBLK" },
			{ "PDO-after" , "View4" },
			{ "PDO-before" , "View4" },
			{ "Position" , "View2" },
			{ "Rest" , "View4" },
			{ "Start Day of Week" , "View2" },
			{ "Time-Away-From-Base" , "View2" },
			{ "Trip Length" , "View2" },
			{ "Work Blk Length" , "View2" },
			{ "Work Days" , "View3" }
		};

		public static List <string> MainList = new List <string> () { "Aircraft Changes", "AM/PM", "Blocks of Days Off","Cities-Legs", "Cmut DHs", "Commutable Lines - Manual","Commutable Lines - Auto", "Days of the Month", "Days of the Week", 
			"DH - first - last", "Duty period", "Equipment Type","ETOPS","ETOPS-Res","Flight Time", "Ground Time", "Intl – NonConus", 
			"Largest Block of Days Off", "Legs Per Duty Period", "Legs Per Pairing", "Normalize Days Off", "Number of Days Off", "Overnight Cities", 
			"Overnight Cities - Bulk", "PDO-after", "PDO-before", "Position", "Start Day of Week", "Rest", "Time-Away-From-Base", "Trip Length", 
			"Work Blk Length", "Work Days"
		};


		public static Dictionary <string,int> HelpPageNo = new Dictionary <string, int> () {
			{ "Aircraft Changes" , 3 },
			{ "AM/PM" , 3 },
			{ "Blocks of Days Off" , 4 },
			{ "Cities-Legs" , 13 },
			{ "Cmut DHs" , 5 },
			{ "Commutable Lines - Manual" , 5 },
			{ "Days of the Month" , 7 },
			{ "Days of the Week" , 8 },
			{ "DH - first - last" , 8 },
			{ "Duty period" , 9 },
			{ "Equipment Type" , 9 },
			{ "ETOPS" , 9 },
			{ "ETOPS-Res" , 9 },
			{ "Flight Time" , 10 },
			{ "Ground Time" , 10 },
			{ "Intl – NonConus" , 10 },
			{ "Largest Block of Days Off" , 11 },
			{ "Legs Per Duty Period" , 11 },
			{ "Legs Per Pairing" , 12 },
			{ "Normalize Days Off" , 12 },
			{ "Number of Days Off" , 12 },
			{ "Overnight Cities" , 13 },
			{ "Overnight Cities - Bulk" , 13 },
			{ "PDO-after" , 14 },
			{ "PDO-before" , 14 },
			{ "Position" , 15 },
			{ "Rest" , 15 },
			{ "Start Day of Week" , 16 },
			{ "Time-Away-From-Base" , 16 },
			{ "Trip Length" , 17 },
			{ "Work Blk Length" , 17 },
			{ "Work Days" , 18 }
		};

		public static List <string> AMPMWeights = new List <string> ();
		public static List <string> BlocksOfDaysOffWeights = new List <string> ();
		public static List <string> CmutDHsWeights = new List <string> ();
		public static List <string> dhFirstLastWeights = new List <string> ();
		public static List <string> DutyPeriodWeights = new List <string> ();
		public static List <string> EQTypeWeights = new List <string> ();
		public static List<string> ETOPSWeights = new List<string>();
		public static List<string> ETOPSResWeights = new List<string>();
		public static List <string> FlightTimeWeights = new List <string> ();
		public static List <string> GroundTimeWeights = new List <string> ();
		public static List <string> IntlNonConusWeights = new List <string> ();
		public static List <string> LegsPerDutyPeriodWeights = new List <string> ();
		public static List <string> LegsPerPairingWeights = new List <string> ();
		public static List <string> NumOfDaysOffWeights = new List <string> ();
		public static List <string> OvernightCitiesWeights = new List <string> ();
		public static List <string> CitiesLegsWeights = new List <string> ();
		public static List <string> PDOAfterWeights = new List <string> ();
		public static List <string> PDOBeforeWeights = new List <string> ();
		public static List <string> PositionWeights = new List <string> ();
		public static List <string> StartDOWWeights = new List <string> ();
		public static List <string> TripLengthWeights = new List <string> ();
		public static List <string> WorkBlockLengthWeights = new List <string> ();
		public static List <string> WorkDaysWeights = new List <string> ();
		public static List <string> RestWeights = new List <string> ();

		public static void clearAll ()
		{

			AMPMWeights.Clear ();
			BlocksOfDaysOffWeights.Clear ();
			CmutDHsWeights.Clear ();
			dhFirstLastWeights.Clear ();
			DutyPeriodWeights.Clear ();
			EQTypeWeights.Clear ();
			ETOPSWeights.Clear();
			ETOPSResWeights.Clear();
			FlightTimeWeights.Clear ();
			GroundTimeWeights.Clear ();
			IntlNonConusWeights.Clear ();
			LegsPerDutyPeriodWeights.Clear ();
			LegsPerPairingWeights.Clear ();
			NumOfDaysOffWeights.Clear ();
			OvernightCitiesWeights.Clear ();
			CitiesLegsWeights.Clear ();
			PDOAfterWeights.Clear ();
			PDOBeforeWeights.Clear ();
			PositionWeights.Clear ();
			StartDOWWeights.Clear ();
			TripLengthWeights.Clear ();
			WorkBlockLengthWeights.Clear ();
			WorkDaysWeights.Clear ();
			RestWeights.Clear ();
		}

	}
}

