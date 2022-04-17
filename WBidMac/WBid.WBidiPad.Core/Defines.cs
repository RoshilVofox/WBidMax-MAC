using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Core
{
    public class Defines
    {
        //////////////////////////////////////////////////////
        //-------- SBS/CmdLine Format DEFINES ----------------
        //
        public const int MAX23RECORDS = 2;
        public const int MAX5RECORDS = 8;

        // Cmd-line format TRIP File column locations
        public const int TC_PAIRING = 1;           // Pairing Number
        public const int TC_RECORD = 5;           // Record Number
        // "1" card
        public const int TC1_CAPACITY = 6;           // Capacity
        public const int TC1_CREWPOS1 = 7;           // Crew Position 1
        public const int TC1_CREWPOS2 = 8;           // Crew Position 2
        public const int TC1_CREWPOS3 = 9;           // Crew Position 3
        public const int TC1_FREQUENCY = 11;          // Frequency Code (D, X)
        public const int TC1_DAYSOP1 = 12;          // Days of Operation
        public const int TC1_DAYSOP2 = 14;          // Days of Operation
        public const int TC1_DAYSOP3 = 16;          // Days of Operation
        public const int TC1_TRIPLENGTH = 18;          // Trip Length
        public const int TC1_DEPSTATION = 20;          // Departure Station 
        public const int TC1_DEPTIME = 23;          // Departure Time (clock) 
        public const int TC1_ARRSTATION = 27;          // Arrival Station 
        public const int TC1_ARRTIME = 30;          // Arrival Time (clock) 
        public const int TC1_FLIGHTNUM = 34;          // Flight Number
        public const int TC1_BLKHRMN = 38;          // Pairing Flight Hours (block)
        public const int TC1_DUTYPERIODS = 42;          // Number of Duty Periods
        public const int TC1_FLTNUMDEPART = 44;          // Departure Flight Number
        public const int TC1_FLTNUMRETURN = 50;          // Return Flight Number
        public const int TC1_BLANK = 56;          // Blank Field
        public const int TC1_PAIRINGCLASS = 62;          // Pairing Class Code
        public const int TC1_ATTRIBCLASS = 63;          // Attribute Class
        public const int TC1_OPSTART = 69;          // First Day of Operation (e.g. "1" = 1st bid period day)
        public const int TC1_OPEND = 71;          // Last Day of Operation
        public const int TC1_NOOPDAY1 = 73;          // No Operation Days 1
        public const int TC1_NOOPDAY2 = 75;          // No Operation Days 2 
        public const int TC1_NOOPDAY3 = 77;          // No Operation Days 3
        public const int TC1_NOOPDAY4 = 79;          // No Operation Days 4
        // "2" card
        public const int TC2_STATION = 6;           // Overnight City (3 chars)
        public const int TC2_CREDITHRMN = 9;           // Daily (NOT duty) Pay Hours (HRMN format, 4 chars)
        public const int TC2_NUMBLOCKS = 10;          // Number of Station & Pay Blocks (how many 7 char blocks)
        public const int TC2_SPECRESTHRMN = 76;          // Specific Rest 
        public const int TC2_5CARDS = 80;          // Number of "5" Cards
        // "3" card
        public const int TC3_FIRSTDUTY = 6;           // First Duty "Push" time (clock) 
        public const int TC3_LASTDUTY = 10;          // Last Duty "Push" time (clock)
        public const int TC3_BLKHRMN = 14;          // Daily Flight Hours (HRMN format, 4 chars)
        public const int TC3_NUMBLOCKS = 10;          // Number of Daily Blocks
        public const int TC3_BEGRESTHRMN = 54;          // Beginning Rest Req.
        public const int TC3_ENDRESTHRMN = 54;          // Ending Rest Req.
        public const int TC3_EQUIP = 54;          // Equipment Qualification
        public const int TC3_LANGUAGE = 58;          // Language Qualification
        public const int TC3_NIGHTHRMN = 62;          // Night Hours 
        public const int TC3_TIMEOFFHRMN = 66;          // Time Off Required After Rest End 
        public const int TC3_BRIEFHRMN = 70;          // Brief HRMN 
        public const int TC3_DEBRIEFHRMN = 74;          // Debrief HRMN 
        // "4" card (not normally used)
        public const int TC4_FIRSTDAY = 6;           // First Day of Bidmonth
        public const int TC4_NUMBLOCKS = 36;          // Number of Codes 
        // "5" card
        public const int TC5_DEADHEAD = 6;           // Deadhead Column (9 = dhd)
        public const int TC5_DEPTIME = 7;           // Departure Time (minutes after midnite 1st day of pairing) 
        public const int TC5_ENDDUTY = 12;          // End of Duty Period 
        public const int TC5_ARRTIME = 13;          // Arrival Time (minutes)
        public const int TC5_NUMBLOCKS = 6;           // Number of Blocks
        public const int TC5_6CARDS = 80;          // Number of "6" Cards
        // "6" card
        public const int TC6_BLANK = 6;           // Blank Column
        public const int TC6_FLTNUM = 8;           // Flight Number
        public const int TC6_DEP = 12;          // Departure City
        public const int TC6_ARR = 15;          // Arrival City
        public const int TC6_ACCHG = 18;          // Equipment Change
        public const int TC6_EQUIP = 19;          // Equipment Type
        public const int TC6_NUMBLOCKS = 4;           // Number of Blocks

        // Cmd-line format PS File column locations
        public const int PS_BLANK = 1;           // Blank 
        public const int PS_LINENUM = 4;           // Line Number
        public const int PS_ALPHALINE = 7;           // Alpha-line specification.
        public const int PS_CREW = 9;           // Crew Position
        public const int PS_PAIRING = 11;          // Pairing Number 
        public const int PS_DATE = 15;          // Calendar Date
        public const int PS_NUMBLOCKS = 10;          // Number of Pairing Blocks 
        public const int PS_CONTINUATION = 71;          // Continuation Card 
        public const int PS_CREDITHRMN = 72;          // Line Credit HRMN 
        public const int PS_BLKHRMN = 77;          // Line Flight Time HRMN


        // TRIPS file contains all the pairings for a specific 
        // bid period, seat, and domicile.
        // The TRIP cmd-line format has no header records.
        // Each pairing consists of (nominally):
        // - one "1" card with summary info for the pairing
        // - one "2" card with daily pay and the number of 5 cards later in the pairing
        // - one "3" card
        // - a series of "5" cards with leg push/arrival times (in minutes after midnight of pairing day 1)
        //     the "5" card also has deadhead info.
        // - a series of "6" cards with flight number, departure cities, arrival cities, and
        //     aircraft type info (for FltOps).
        //
        // The file is terminated by a *E record (*E in the 1st column).

        //////////////////////////////////////////////////
        //-------- SBS/GUI-format DEFINES ----------------

        // GUI Trip File column locations
        // HDR record
        public const int GTHDR_FIELD = 1;           // "GTRIP " Card identifier
        public const int GTHDR_STARTDATE = 25;          // Bid Period Start Date (e.g. 01AUG96)
        public const int GTHDR_ENDDATE = 33;          // Bid Period End Date (e.g. 31AUG96)
        public const int GTHDR_CREATEDATE = 41;          // Bid Period End Date (e.g. 31AUG96)
        // T1 record
        public const int GTT1_FIELD = 1;           // "T1" Card identifier
        public const int GTT1_PAIRING = 3;           // Pairing Number
        public const int GTT1_DOMICILE = 7;           // Domicile
        // T2 record
        public const int GTT2_FIELD = 1;           // "T2" Card Ident
        // public const int GTT2_TAFB       = 3;           // TAFB   ... removed with "D" position change
        public const int GTT2_POSN = 11;          // Position Codes
        // F record - frequency
        public const int GTF_FIELD = 1;           // "F " card identifier
        public const int GTF_OPVECTOR = 3;           // Operation vector start date
        // Z record - frequency (ignored here. F and Z records are typically the same)
        // D record - Duty summary
        public const int GTD_FIELD = 1;           // "D " card identifier (Duty Period)
        public const int GTD_BLK = 3;           // DP Block Flt time (min.)
        public const int GTD_CREDIT = 8;           // DP Credit
        public const int GTD_DUTYTIME = 13;          // DP length (minutes)
        public const int GTD_REPORTTIME = 18;          // DP Report Time
        // L record - Leg
        public const int GTL_FIELD = 1;           // "L " card identifier (Duty Period)
        public const int GTL_FLTNUM = 5;           // Leg Flight Number
        public const int GTL_EQUIP = 10;          // Leg Equipment
        public const int GTL_DEPCITY = 14;          // Leg Depature City
        public const int GTL_DEPTIME_L = 18;          // Departure Time (Local)
        public const int GTL_DEPTIME_Z = 23;          // Departure Time (UTC)
        public const int GTL_DEPTIME_B = 28;          // Departure Time (Base)
        public const int GTL_ARRCITY = 33;          // Leg Depature City
        public const int GTL_ARRTIME_L = 37;          // Arrival Time (Local)
        public const int GTL_ARRTIME_Z = 42;          // Arrival Time (UTC)
        public const int GTL_ARRTIME_B = 47;          // Arrival Time (Base)
        public const int GTL_BEGINDUTY = 52;          // Begin Duty Period flag
        public const int GTL_BRIEFTIME = 53;          // Brief prior to D.P.
        public const int GTL_ENDDUTY = 56;          // End Duty Period flag
        public const int GTL_DEBRIEF = 57;          // Debrief time
        public const int GTL_DEADHEAD = 61;          // Deadhead indicator
        public const int GTL_ACCHANGE = 67;          // AC Change
        public const int GTL_CONUSFLAG = 66;          // CONUS flag for this leg
        public const int GTL_ETOPS = 74;              //ETOPS flag for this leg

        // GUI Line File column locations
        // HDR record
        public const int GLHDR_FIELD = 1;           // "GBIDL " Card identifier
        public const int GLHDR_STARTDATE = 16;          // Bid Period Start Date (e.g. 01AUG96)
        public const int GLHDR_ENDDATE = 24;          // Bid Period End Date (e.g. 31AUG96)
        public const int GLHDR_CREATEDATE = 32;          // Solution Creation Date
        // C record
        public const int GLC_FIELD = 1;           // "C " Card identifier
        public const int GLC_CREWCODE = 3;           // Crew Code (0 = 'A', 1='B', 2='C')
        public const int GLC_LINENUM = 5;           // Line Number
        public const int GLC_CREDIT = 13;          // Line Credit (hhhmm)
        public const int GLC_BLK = 18;          // Line Block Hours (hhhmm)
        // T record
        public const int GLT_FIELD = 1;           // "T " Card identifier
        public const int GLT_CREWCODE = 3;           // Crew Code (0 = 'A', 1='B', 2='C')
        public const int GLT_LINENUM = 5;           // Line Number
        public const int GLT_PAIRING_1 = 13;          // Pairing #1 Number
        public const int GLT_STARTDATE_1 = 17;          // Pairing #1 Start Date
        public const int GLT_POSITION_1 = 31;          // Pairing #1 Position
        public const int GLT_PAIRING_2 = 32;          // Pairing #2 Number
        public const int GLT_STARTDATE_2 = 36;          // Pairing #2 Start Date
        public const int GLT_POSITION_2 = 50;          // Pairing #3 Position
        public const int GLT_PAIRING_3 = 51;          // Pairing #3 Number
        public const int GLT_STARTDATE_3 = 55;          // Pairing #3 Start Date
        public const int GLT_POSITION_3 = 69;          // Pairing #3 Position
        // A record - text, Normally only used for F/A 2nd round reserve pairings
        public const int GLA_LINENUM = 3;           // Line Number
        public const int GLA_CODE = 13;          // code
        public const int GLA_STARTDATE = 18;          // Pairing Start Date
        public const int GLA_STARTTIME = 25;          // Pairing Start Time
        public const int GLA_ENDDATE = 29;          // Pairing End Date
        public const int GLA_ENDTIME = 36;          // Pairing End Time


        // GTrip (TRIPS file) contains all the pairings for a specific 
        // bid period, seat, and domicile.
        // The GTrip format has one header record with dates, 
        // followed by all the pairings (normally in sequential order),
        // follwed by a terminating record (*E in the 1st column).

        // Each GTrip pairing constis of:
        // one T1 record (pairing number, domicile)
        // one T2 record (Time-Away-From-Base info)
        // one Z or F record (frequency of operation)
        // multiple D & L records.
        //
        // Each Duty Period is described by a "D" record, then each Leg
        // of the pairing has an "L" record. This sequences repeasts for
        // each duty period of the pairing (one D record followed by 
        // multiple L records for each Leg of that duty period).

        // a "D" record contains block, duty, pay, and report time for the
        // associated duty period.

        // an "L" record contains leg specific information.


    }
}
