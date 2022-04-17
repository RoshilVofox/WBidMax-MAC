using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Model;
using System.IO;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model.State.Weights;
using System.Globalization;
using Newtonsoft.Json;


namespace WBid.WBidiPad.PortableLibrary
{
	public class WBidCollection
	{
		/// <summary>
		/// PUPOSE : Get Bid Period List
		/// </summary>
		/// <returns></returns>
		public static List<BidPeriod> GetBidPeriods()
		{
			return new List<BidPeriod>()
			{
				new BidPeriod(){BidPeriodId=1, Period="January",HexaValue="1"},
				new BidPeriod(){BidPeriodId=2,Period="February",HexaValue="2"},
				new BidPeriod(){BidPeriodId=3,Period="March",HexaValue="3"},
				new BidPeriod(){BidPeriodId=4,Period="April",HexaValue="4"},
				new BidPeriod(){BidPeriodId=5,Period="May",HexaValue="5"},
				new BidPeriod(){BidPeriodId=6,Period="June",HexaValue="6"},
				new BidPeriod(){BidPeriodId=7,Period="July",HexaValue="7"},
				new BidPeriod(){BidPeriodId=8,Period="August",HexaValue="8"},
				new BidPeriod(){BidPeriodId=9,Period="September",HexaValue="9"},
				new BidPeriod(){BidPeriodId=10,Period="October",HexaValue="A"},
				new BidPeriod(){BidPeriodId=11,Period="November",HexaValue="B"},
				new BidPeriod(){BidPeriodId=12,Period="December",HexaValue="C"}
			};
		}

		/// <summary>
		/// PURPOSE : Get Position List
		/// </summary>
		/// <returns></returns>
		public static List<Position> GetPositions()
		{
			return new List<Position>()
			{
			  new Position(){PositionName="Captain",ShortStr="C",LongStr="CP"},
			  new Position(){PositionName="First Officer",ShortStr="F",LongStr="FO"},
			  new Position(){PositionName="Flight Attendant",ShortStr="A",LongStr="FA"}

			};
		}

		/// <summary>
		/// PURPOSE : Get Bid Rounds List 
		/// </summary>
		/// <returns></returns>
		public static List<BidRound> GetBidRounds()
		{
			return new List<BidRound>()
			{
			  new BidRound(){Round="Monthly",RoundDescription="Monthly Bid (First Round)",ShortStr="D"},
			  new BidRound(){Round="2nd Round",RoundDescription="Blank/Reserve Bid (Second Round)",ShortStr="B"}

			};
		}
		public static ColumnDefinitions GetRatioFeatureColumn()
		{
			return new ColumnDefinitions()
			{
				new ColumnDefinition{Id=6,DisplayName="$/Day",DataPropertyName="TfpPerDay"},
				new ColumnDefinition{Id=7,DisplayName="$/DHr",DataPropertyName="TfpPerDhr"},
				new ColumnDefinition{Id=8,DisplayName="$/Hr",DataPropertyName="TfpPerFltHr"},
				new ColumnDefinition{Id=9,DisplayName="$/TAFB",DataPropertyName="TfpPerTafb"},
				new ColumnDefinition{Id=11,DisplayName="+Legs",DataPropertyName="MostLegs"},
				new ColumnDefinition{Id=12,DisplayName="+Off",DataPropertyName="LargestBlkOfDaysOff"},
				new ColumnDefinition{Id=13,DisplayName="1Dy",DataPropertyName="Trips1Day"},
				new ColumnDefinition{Id=14,DisplayName="2Dy",DataPropertyName="Trips2Day"},
				new ColumnDefinition{Id=15,DisplayName="3Dy",DataPropertyName="Trips3Day"},
				new ColumnDefinition{Id=16,DisplayName="4Dy",DataPropertyName="Trips4Day"},
				new ColumnDefinition{Id=19,DisplayName="ACChg",DataPropertyName="AcftChanges"},
				new ColumnDefinition{Id=20,DisplayName="ACDay",DataPropertyName="AcftChgDay"},
				new ColumnDefinition{Id=21,DisplayName="CO",DataPropertyName="CarryOverTfp"},
				new ColumnDefinition{Id=22,DisplayName="DP",DataPropertyName="TotDutyPds"},
				new ColumnDefinition{Id=23,DisplayName="DPinBP",DataPropertyName="TotDutyPdsInBp"},
				new ColumnDefinition{Id=30,DisplayName="Legs",DataPropertyName="Legs"},
				new ColumnDefinition{Id=31,DisplayName="LgDay",DataPropertyName="LegsPerDay"},
				new ColumnDefinition{Id=32,DisplayName="LgPair",DataPropertyName="LegsPerPair"},
				new ColumnDefinition{Id=33,DisplayName="ODrop",DataPropertyName="OverlapDrop"},
				new ColumnDefinition{Id=34,DisplayName="Off",DataPropertyName="DaysOff"},
				new ColumnDefinition{Id=35,DisplayName="Pairs",DataPropertyName="TotPairings"},
				new ColumnDefinition{Id=36,DisplayName="Pay",DataPropertyName="Tfp"},
				new ColumnDefinition{Id=37,DisplayName="PDiem",DataPropertyName="TafbInBp"},
				new ColumnDefinition{Id=38,DisplayName="MyValue",DataPropertyName="Points"},
				new ColumnDefinition{Id=39,DisplayName="SIPs",DataPropertyName="Sips"},
				new ColumnDefinition{Id=43,DisplayName="VDrop",DataPropertyName="VacationDrop"},
				new ColumnDefinition{Id=45,DisplayName="FltRig",DataPropertyName="RigFltInBP"},
				new ColumnDefinition{Id=46,DisplayName="MinPayRig",DataPropertyName="RigDailyMinInBp"},
				new ColumnDefinition{Id=47,DisplayName="DhrRig",DataPropertyName="RigDhrInBp"},
				new ColumnDefinition{Id=48,DisplayName="AdgRig",DataPropertyName="RigAdgInBp"},
				new ColumnDefinition{Id=49,DisplayName="TafbRig",DataPropertyName="RigTafbInBp"},
				new ColumnDefinition{Id=50,DisplayName="TotRig",DataPropertyName="RigTotalInBp"},
				new ColumnDefinition{Id=53,DisplayName="VacPay",DataPropertyName="VacPay"},
				new ColumnDefinition{Id=54,DisplayName="Vofrnt",DataPropertyName="VacationOverlapFront"},
				new ColumnDefinition{Id=55,DisplayName="Vobk",DataPropertyName="VacationOverlapBack"},
				new ColumnDefinition{Id=56,DisplayName="800legs",DataPropertyName="LegsIn800"},
				new ColumnDefinition{Id=57,DisplayName="700legs",DataPropertyName="LegsIn700"},
				new ColumnDefinition{Id=76,DisplayName="8Max",DataPropertyName="LegsIn600"},
				new ColumnDefinition{Id=62,DisplayName="Wts",DataPropertyName="TotWeight"},
				new ColumnDefinition{Id=63,DisplayName="LineRig",DataPropertyName="LineRig"},
				new ColumnDefinition{Id=66,DisplayName="HolRig",DataPropertyName="HolRig"},
				new ColumnDefinition{Id=68,DisplayName="nMid",DataPropertyName="NightsInMid"},
				new ColumnDefinition{Id=69,DisplayName="cmts",DataPropertyName="TotalCommutes"},
				new ColumnDefinition{Id=70,DisplayName="cmtFr",DataPropertyName="commutableFronts"},
				new ColumnDefinition{Id=71,DisplayName="cmtBa",DataPropertyName="CommutableBacks"},
				new ColumnDefinition{Id=72,DisplayName="cmt%Fr",DataPropertyName="CommutabilityFront"},
				new ColumnDefinition{Id=73,DisplayName="cmt%Ba",DataPropertyName="CommutabilityBack"},
				new ColumnDefinition{Id=74,DisplayName="cmt%Ov",DataPropertyName="CommutabilityOverall"},
				new ColumnDefinition{Id=75,DisplayName="7Max", DataPropertyName = "LegsIn200"},
				new ColumnDefinition{Id = 87, DisplayName = "Etrips", DataPropertyName = "ETOPSTripsCount" },
				new ColumnDefinition{Id = 88,  DisplayName = "DHFirst", DataPropertyName = "DhFirstTotal"},
				new ColumnDefinition{Id = 89, DisplayName = "DHLast", DataPropertyName = "DhLastTotal"},
				new ColumnDefinition{Id = 90, DisplayName = "DHTotal", DataPropertyName = "DhTotal"}
		};
		}
		/// <summary>
		/// PURPOSE : Create INI file
		/// </summary>
		/// <param name="fileName"></param>
		public static WBidINI CreateINIFile()
		{
			WBidINI config = new WBidINI();

			config.Version = GlobalSettings.IniFileVersion;

			config.Updates = new INIUpdates() { Trips = 0, News = 0, Cities = 0, Hotels = 0, Domiciles = 0, Equipment = 0, EquipTypes = 0 };

			//Domiciles
			config.DataColumns = GenerateDefaultColumns();
			config.Domiciles = new List<Domicile>
			{

				new Domicile{DomicileName="BWI",DomicileId=1,Code="B",Number=6},
				new Domicile{DomicileName="DAL",DomicileId=2,Code="D",Number=2},
				new Domicile{DomicileName="DEN",DomicileId=3,Code="C",Number=4},
				new Domicile{DomicileName="HOU",DomicileId=4,Code="H",Number=5},
				new Domicile{DomicileName="LAS",DomicileId=5,Code="L",Number=9},
				new Domicile{DomicileName="MCO",DomicileId=6,Code="F",Number=4},
				new Domicile{DomicileName="MDW",DomicileId=7,Code="M",Number=7},
				new Domicile{DomicileName="OAK",DomicileId=8,Code="O",Number=3},
				new Domicile{DomicileName="PHX",DomicileId=9,Code="P",Number=8},
				new Domicile{DomicileName="ATL",DomicileId=10,Code="A",Number=1},
				new Domicile{DomicileName="AUS",DomicileId=11,Code="T",Number=11},
				new Domicile{DomicileName="FLL",DomicileId=12,Code="Y",Number=12},
                new Domicile{DomicileName="LAX",DomicileId=13,Code="X",Number=13}
			};

			config.AmPmConfigure = new AmPmConfigure()
			{
				HowCalcAmPm = 1,
				AmPush = TimeSpan.FromHours(4),
				AmLand = TimeSpan.FromHours(19),
				PmPush = TimeSpan.FromHours(11),
				PmLand = TimeSpan.FromHours(2),
				NitePush = TimeSpan.FromHours(22),
				NiteLand = TimeSpan.FromHours(7),
				NumberOrPercentageCalc = 1,
				NumOpposites = 3,
				PctOpposities = 20

			};

			//Cities
			config.Cities = new List<City>(){
			   new City{Id=1,Name="ABQ",Code=7},
				new City{Id=2,Name="ALB",Code=5},
				new City{Id=3,Name="AMA",Code=6},
				new City{Id=4,Name="ATL",Code=5},
				new City{Id=5,Name="AUA",Code=4},
				new City{Id=6,Name="AUS",Code=6},
				new City{Id=7,Name="BDA",Code=4},
				new City{Id=8,Name="BDL",Code=5},
				new City{Id=9,Name="BHM",Code=5},
				new City{Id=10,Name="BKG",Code=6},
				new City{Id=11,Name="BNA",Code=6},
				new City{Id=12,Name="BOI",Code=7},
				new City{Id=13,Name="BOS",Code=5},
				new City{Id=14,Name="BUF",Code=5},
				new City{Id=15,Name="BUR",Code=8},
				new City{Id=16,Name="BWI",Code=5},
				new City{Id=17,Name="CAK",Code=5},
				new City{Id=18,Name="CHS",Code=5},
				new City{Id=19,Name="CLE",Code=5},
				new City{Id=20,Name="CLT",Code=5},
				new City{Id=21,Name="CMH",Code=5},
				new City{Id=22,Name="CRP",Code=6},
				new City{Id=23,Name="CUN",Code=6},
				new City{Id=24,Name="DAL",Code=6},
				new City{Id=25,Name="DAY",Code=6},
				new City{Id=26,Name="DCA",Code=5},
				new City{Id=27,Name="DEN",Code=7},
				new City{Id=28,Name="DSM",Code=6},
				new City{Id=29,Name="DTW",Code=5},
				new City{Id=30,Name="ECP",Code=5},
				new City{Id=31,Name="ELP",Code=7},
				new City{Id=32,Name="EWR",Code=5},
				new City{Id=33,Name="EYW",Code=5},
				new City{Id=34,Name="FLL",Code=5},
				new City{Id=35,Name="FNT",Code=5},
				new City{Id=36,Name="GEG",Code=8},
				new City{Id=37,Name="GRR",Code=6},
				new City{Id=38,Name="GSP",Code=5},
				new City{Id=39,Name="HOU",Code=6},
				new City{Id=40,Name="HRL",Code=6},
				new City{Id=41,Name="IAD",Code=5},
				new City{Id=42,Name="ICT",Code=6},
				new City{Id=43,Name="ISP",Code=5},
				new City{Id=44,Name="JAN",Code=6},
				new City{Id=45,Name="JAX",Code=5},
				new City{Id=46,Name="IND",Code=6},
				new City{Id=47,Name="LAS",Code=8},
				new City{Id=48,Name="LAX",Code=8},
				new City{Id=49,Name="LBB",Code=6},
				new City{Id=50,Name="LGA",Code=5},
				new City{Id=51,Name="LIT",Code=6},
				new City{Id=52,Name="MAF",Code=6},
				new City{Id=53,Name="MBJ",Code=5},
				new City{Id=54,Name="MCI",Code=6},
				new City{Id=55,Name="MCO",Code=5},
				new City{Id=56,Name="MDW",Code=6},
				new City{Id=57,Name="MEM",Code=5},
				new City{Id=58,Name="MEX",Code=6},
				new City{Id=59,Name="MHT",Code=5},
				new City{Id=60,Name="MKE",Code=6},
				new City{Id=61,Name="MSP",Code=6},
				new City{Id=62,Name="MSY",Code=6},
				new City{Id=63,Name="NAS",Code=5},
				new City{Id=64,Name="OAK",Code=8},
				new City{Id=65,Name="OKC",Code=6},
				new City{Id=66,Name="OMA",Code=6},
				new City{Id=67,Name="ONT",Code=8},
				new City{Id=68,Name="ORF",Code=5},
				new City{Id=69,Name="NAS",Code=5},
				new City{Id=70,Name="PBI",Code=5},
				new City{Id=71,Name="PDX",Code=8},
				new City{Id=72,Name="PHL",Code=5},
				new City{Id=73,Name="PHX",Code=7,NonConus=true},
				new City{Id=74,Name="PIT",Code=5},
				new City{Id=75,Name="PNS",Code=6},
				new City{Id=76,Name="PUJ",Code=4},
				new City{Id=77,Name="PVD",Code=5},
				new City{Id=78,Name="PWM",Code=5},
				new City{Id=79,Name="RIC",Code=5},
				new City{Id=80,Name="RDU",Code=5},
				new City{Id=81,Name="RNO",Code=8},
				new City{Id=82,Name="ROC",Code=5},
				new City{Id=83,Name="RSW",Code=5},
				new City{Id=84,Name="SAN",Code=8},
				new City{Id=85,Name="SAT",Code=6},
				new City{Id=86,Name="SDF",Code=6},
				new City{Id=87,Name="SEA",Code=8},
				new City{Id=88,Name="SFO",Code=8},
				new City{Id=89,Name="SJC",Code=8},
				new City{Id=90,Name="SJD",Code=7},
				new City{Id=91,Name="SJU",Code=4},
				new City{Id=92,Name="SLC",Code=7},
				new City{Id=93,Name="SMF",Code=8},
				new City{Id=94,Name="SNA",Code=8},
				new City{Id=95,Name="STL",Code=6},
				new City{Id=96,Name="TPA",Code=5},
				new City{Id=97,Name="TUL",Code=6},
				new City{Id=98,Name="TUS",Code=7,NonConus=true},
                new City{Id=98,Name="TUS",Code=7,NonConus=true},
                new City{Id=99,Name="LIR",Code=6},
                new City{Id=100,Name="LGB",Code=6},
                new City{Id=101,Name="SJO",Code=6},
                new City{Id=102,Name="CVG",Code=6}
			};


			config.Hotels = new Hotels();
			config.Hotels.Effective = string.Empty;
			config.Hotels.HotelList = new List<Hotel>()
			{
				new Hotel(){City="ABQ",Hotels="AM:Sheraton Airport 843-7000, PM:Hyatt   (505)842-1234"},
				new Hotel(){City="ALB",Hotels="Crowne Plaza                             (518)462-6611"},
				new Hotel(){City="AMA",Hotels="Ambassador                               (806)358-6161"},
				new Hotel(){City="ATL",Hotels="Mariott 766-7900, Renaissance Concourse  (404)209-9999"},
				new Hotel(){City="AUS",Hotels="Holiday Inn Town Lake 472-8211, Doubletree (512)454-3737"},
				new Hotel(){City="BDA",Hotels="Sheraton (413)781-1010, Doubletree Bradley (860)627-5171"},
				new Hotel(){City="BDL",Hotels="Sheraton (413)781-1010, Doubletree Bradley (860)627-5171"},
				new Hotel(){City="BHM",Hotels="Sheraton                                 (205)324-5000"},
				new Hotel(){City="BNA",Hotels="Renaisance 255-8400, Sheraton Music City (615)885-2200"},
				new Hotel(){City="BOI",Hotels="Red Lion Downtown                        (208)344-7691 "},
				new Hotel(){City="BOS",Hotels="Hilton 568-6700, Courtyard by Marriott   (617)569-5250 "},
				new Hotel(){City="BUF",Hotels="Adam's Mark Buffalo                      (716)845-5100"},
				new Hotel(){City="BUR",Hotels="Holiday Inn Downtown                     (818)841-4770"},
				new Hotel(){City="BWI",Hotels="Doubletree                               (410)859-8400"},
				new Hotel(){City="CHS",Hotels="Crowne Plaza                             (843)744-4422"},
				new Hotel(){City="CLE",Hotels="Holiday Inn Strongsville                 (440)238-8800"},
				new Hotel(){City="CMH",Hotels="Crowne Plaza                             (614)461-4100"},
				new Hotel(){City="CRP",Hotels="Holiday Inn                              (361)882-1700"},
				new Hotel(){City="DAL",Hotels="Wyndham 357-8500, Embassy Suites Mkt Ctr (214)630-5332"},
				new Hotel(){City="DEN",Hotels="Courtyard 371-0300, Holiday Inn 574-1300, Doubletree (303)321-3333"},
				new Hotel(){City="DTW",Hotels="Four Points by Sheraton                  (734)729-9000"},
				new Hotel(){City="ECP",Hotels="Majestic Beach Resort                    (850)563-1000"},
				new Hotel(){City="ELP",Hotels="Radisson 772-3333, Marriott              (915)779-3300"},
				new Hotel(){City="EWR",Hotels="Renaissance(908)436-4600, Newark Aiprort (973)623-0006"},
				new Hotel(){City="FLL",Hotels="Renaissance 626-1700, Hilton Marina      (954)463-4000"},
				new Hotel(){City="GEG",Hotels="Red Lion at the Park                     (509)326-8000"},
				new Hotel(){City="GSP",Hotels="Hyatt Regency Greenville                 (864)235-1234"},
				new Hotel(){City="HOU",Hotels="Doubletree Hobby Airport                 (713)645-3000"},
				new Hotel(){City="HRL",Hotels="Courtyard by Marriott                    (956)412-7800"},
				new Hotel(){City="IAD",Hotels="Dulles Hilton                            (703)478-2900"},
				new Hotel(){City="IND",Hotels="Embassy Suites 236-1800, Wyndham West    (317)248-2481"},
				new Hotel(){City="ISP",Hotels="Hyatt 784-1234, Mariott Islandia         (631)232-3000"},
				new Hotel(){City="JAN",Hotels="Marriott                                 (601)969-5100"},
				new Hotel(){City="JAX",Hotels="Hyatt                                    (904)588-1234"},
				new Hotel(){City="LAS",Hotels="Hyatt Place                              (702)369-3366"},
				new Hotel(){City="LAX",Hotels="Crowne Plaza 642-1111, Sheraton Gateway  (310)642-1111"},
				new Hotel(){City="LBB",Hotels="Holiday Inn                              (806)763-1200"},
				new Hotel(){City="LGA",Hotels="Marriott 565-8900, Courtyard by Marriott (718)446-4800"},
				new Hotel(){City="LIT",Hotels="Holiday Inn Presidential                 (501)375-2100"},
				new Hotel(){City="MAF",Hotels="Odessa MCM Elegante                      (432)368-5885"},
				new Hotel(){City="MCI",Hotels="Marriott 464-2200, Hilton 891-8900, Courtyard (816)891-7500"},
				new Hotel(){City="MCO",Hotels="Hyatt Orlando Airport                    (407)825-1234"},
				new Hotel(){City="MDW",Hotels="Holiday Inn                              (708)563-6490"},
				new Hotel(){City="MHT",Hotels="Four Points Sheraton 688-6110, Crowne Plaza (603)886-1200"},
				new Hotel(){City="MKE",Hotels="Crowne Plaza 764-5300, Hilton City Center (414)271-7250"},
				new Hotel(){City="MSP",Hotels="Minneapolis Airport Marriott             (952)851-7441"},
				new Hotel(){City="MSY",Hotels="Doubletree 467-3111, Sheraton Metairie   (504)837-6707"},
				new Hotel(){City="OAK",Hotels="Hilton                                   (510)635-5000"},
				new Hotel(){City="OKC",Hotels="Shearton Downtown                        (405)235-2780"},
				new Hotel(){City="OMA",Hotels="Doubletree Downtown                      (402)346-7600"},
				new Hotel(){City="ONT",Hotels="Ayres/CountrySide 390-7778, Doubletree   (909)937-0900"},
				new Hotel(){City="ORF",Hotels="Sheraton Norfolk Waterside               (757)622-6664"},
				new Hotel(){City="PBI",Hotels="Hilton Palm Beach                        (561)684-9400"},
				new Hotel(){City="PDX",Hotels="Hyatt Place 288-2808, Monarch            (503)652-1515"},
				new Hotel(){City="PHL",Hotels="Renaissance (610)521-5900, Sonesta Downtown (215)561-7500"},
				new Hotel(){City="PHX",Hotels="Crowne Plaza PHX 273-7778, Radisson South (602)437-8400"},
				new Hotel(){City="PIT",Hotels="Marriott City Center(412)471-4000, Hyatt(724)899-1234, Marriott(412)471-4000"},
				new Hotel(){City="PVD",Hotels="Sheraton 738-4000, Crowne Plaza          (401)738-4000"},
				new Hotel(){City="RDU",Hotels="Doubletree 361-4660, Four Points Sheraton (919)380-1221"},
				new Hotel(){City="RNO",Hotels="Nuggett                                  (775)356-3300"},
				new Hotel(){City="RSW",Hotels="Crowne Plaza                             (239)482-2900"},
				new Hotel(){City="SAN",Hotels="Wyndham Bayside 232-3861, Crowne Plaza   (619)297-1101"},
				new Hotel(){City="SAT",Hotels="Holiday Inn Select                       (210)349-9900"},
				new Hotel(){City="SDF",Hotels="Crowne Plaza 367-2251, The Brown         (502)583-1234"},
				new Hotel(){City="SEA",Hotels="Doubletree                               (206)246-8600"},
				new Hotel(){City="SFO",Hotels="Marriott 692-9100, Doubletree 344-5500, Crowne Plaza Foster City (650)570-5700"},
				new Hotel(){City="SJC",Hotels="Airport Radisson 452-0200, Doubletree 453-4000, Holiday Inn (408)453-6200"},
				new Hotel(){City="SLC",Hotels="Red Lion                                 (801)521-7373"},
				new Hotel(){City="SMF",Hotels="Holiday Inn Capitol Plaza                (916)446-0100"},
				new Hotel(){City="SNA",Hotels="Wyndham (714)751-5100, Hyatt             (949)975-1234"},
				new Hotel(){City="STL",Hotels="Doubletree 434-0100, Hilton Airport      (314)426-5500"},
				new Hotel(){City="TPA",Hotels="Marriott 878-6503, Doubletree 879-4800, Wyndham (813)289-8200"},
				new Hotel(){City="TUL",Hotels="Doubletree Downtown                      (918)587-8000"},
				new Hotel(){City="TUS",Hotels="Radisson Suites 721-7100, Hyatt Place    (520)295-0405"},
				new Hotel(){City="CAK",Hotels="McKinley Grand Hotels                     (330)454-5000"},
				new Hotel(){City="DAY",Hotels="Crowne Plaza                             (937)224-0800"},
				new Hotel(){City="DCA",Hotels="Doubletree                               (703)416-4100"},
				new Hotel(){City="EYW",Hotels="Marriott Beachside                       (305)296-8100"},
				new Hotel(){City="BKG",Hotels="Radisson                                 (417)335-5767"},
				new Hotel(){City="CLT",Hotels="Hilton Charlotte Exec. Park              (704)527-8000"},
				new Hotel(){City="CUN",Hotels="Westin                                   52 998-848-74"},
				new Hotel(){City="DSM",Hotels="Des Moines Marriott                      (515)245-5500"},
				new Hotel(){City="FNT",Hotels="Holiday Inn Gateway Center               (810)232-5300"},
				new Hotel(){City="GRR",Hotels="Holiday Inn                              (616)285-7600"},
				new Hotel(){City="ICT",Hotels="Drury Plaza Broadview                    (316)262-5000"},
				new Hotel(){City="MBJ",Hotels="Ritz Carlton                             (876)953-2800"},
				new Hotel(){City="MEM",Hotels="Doubletree                               (901)767-6490"},
				new Hotel(){City="PNS",Hotels="Hampton Pensacola Beach                  (850)932-6800"},
				new Hotel(){City="PUJ",Hotels="Now Larimar & Secrets Royal Beach        (809)221-4646"},
				new Hotel(){City="PWM",Hotels="Holiday Inn by the Bay                   (207)775-2311"},
				new Hotel(){City="RIC",Hotels="Crowne Plaza Richmond                    (804)788-0900"},
				new Hotel(){City="ROC",Hotels="Radisson Rochester Riverside             (585)546-6400"},
				new Hotel(){City="SJU",Hotels="Intercontinental                         (787)791-6100m"},
			};


			config.AvoidanceBids = new AvoidanceBids() { Avoidance1 = "0", Avoidance2 = "0", Avoidance3 = "0" };
			config.BuddyBids = new BuddyBids() { Buddy1 = "0", Buddy2 = "0" };


			config.Data = new Data { IsCompanyData = true };
			config.Week = new Week() { IsMaxWeekend = false, IsMaxWeekendDefault = false, MaxNumber = "3", MaxNumberDefault = "3", MaxPercentage = "20", MaxPercentageDefault = "20", StartDOW = "0" };

			config.MiscellaneousTab = new MiscellaneousTab() { Coverletter = true, DataUpdate = true, IsRetrieveMissingData = true };

			config.PairingExport = new PairingExport() { IsCentralTime = true, IsEntirePairing = true, IsSubjectLineSelected = false };

			config.SourceHotel = new SourceHotel() { SourceType = 1 };

			config.User = new User() { IsNeedCrashMail = true, SmartSynch = false, AutoSave = false, AutoSavevalue = 10, IsNeedBidReceipt = true,IsModernViewShade=true };

			config.BidLineNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
			config.BidLineVacationColumns = new List<int>() { 36, 53, 200, 34, 12 };

			config.ModernNormalColumns = new List<int>() { 36, 37, 27, 34, 12 };
			config.ModernVacationColumns = new List<int>() { 36, 53, 200, 34, 12 };

			config.SummaryVacationColumns = GenerateDefaultVacationColumns();
            config.SenioritylistFormat = getDefaultSenlistFormatValue();

			config.SplitPointCities = new SplitPointCities()
			{
				new SplitPointCity {Domicile = "ATL", Cities = new List<string> {"ATL","BNA","CMH","ECP","GSP","IND","JAX","LIT","MCO","MEM", "MSY","PNS","RDU","RIC","SDF", "SRQ","STL","TPA"} },
				new SplitPointCity {Domicile = "AUS", Cities = new List<string>{"AMA", "CRP", "DAL", "HOU", "HRL", "JAN", "LBB", "LIT", "MAF", "MSY", "OKC", "SAT", "TUL" }},
				new SplitPointCity {Domicile = "BWI", Cities = new List<string>{"ALB","BDL","BOS","BUF","BWI","CHS","CLE","CLT","CMH","CVG","DTW","GSP","ISP","MHT","MYR","ORF","PIT","PVD","PWM","RDU","ROC","SDF"}},
				new SplitPointCity {Domicile = "DAL", Cities = new List<string>{"AMA","AUS","DAL","HOU","IAH","LBB","LIT","MAF","MCI","MEM","MSY","SAT","TUL"}},
				new SplitPointCity {Domicile = "DEN", Cities = new List<string>{"ABQ","COS","DEN","HDN","ICT","MTJ","OKC","OMA","SLC"}},
				new SplitPointCity {Domicile = "FLL", Cities = new List<string>{"ECP","JAX","MCO","RSW","TPA"}},
				new SplitPointCity {Domicile = "HOU", Cities = new List<string>{"AUS","CRP","DAL","HOU","HRL","MAF","MEM","MSY","OKC","PNS","SAT","TUL"}},
				new SplitPointCity {Domicile = "LAS", Cities = new List<string>{"ABQ","BUR","FAT","LAS","LAX","LGB","OAK","ONT","PHX","PSP","RNO","SAN","SBA","SFO","SJC","SLC","SMF","SNA","TUS"}},
				new SplitPointCity {Domicile = "LAX", Cities = new List<string>{"LAS","LAX", "OAK", "PHX", "RNO", "SFO", "SJC", "SMF"}},
				new SplitPointCity {Domicile = "MCO", Cities = new List<string>{"ATL","BHM","FLL","MCO","RSW"}},
				new SplitPointCity {Domicile = "MDW", Cities = new List<string>{"BNA","BUF","CLE","CMH","CVG","DTW","GRR","MCI","MDW","MEM","MSP","OMA","PIT","SDF","STL"}},
				new SplitPointCity {Domicile = "OAK", Cities = new List<string>{"BUR","LAS","LAX","LGB","OAK","ONT","PSP","RNO","SAN","SBA","SNA"}},
				new SplitPointCity {Domicile = "PHX", Cities = new List<string>{"ABQ","BUR","ELP","LAS","LAX","LGB","ONT","PHX","PSP","SAN","SNA"}}

			};
			return config;
		}



        public static List<SenListFormat> getDefaultSenlistFormatValue()
        {
            return new List<SenListFormat>() {
                new SenListFormat { SeqNumSt=1,SeqNumEnd=4,EbgSt=15,EbgEnd=15,BidTypeSt=24,BidTypeEnd=24,LcSt=28,LcEnd=28,EmpIdSt=30,EmpIdEnd=35,NameSt=37,NameEnd=69,ChkPltSt=71,ChkPltEnd=71,AbscenceTypeSt=74,AbscenceTypeEnd=75,AbsenceDatesSt=77,AbsenceDatesEnd=87,Round=1,Position="CP"},
                new SenListFormat { SeqNumSt=1,SeqNumEnd=4,EbgSt=15,EbgEnd=15,BidTypeSt=24,BidTypeEnd=24,LcSt=28,LcEnd=28,EmpIdSt=30,EmpIdEnd=35,NameSt=37,NameEnd=69,ChkPltSt=71,ChkPltEnd=71,AbscenceTypeSt=74,AbscenceTypeEnd=75,AbsenceDatesSt=77,AbsenceDatesEnd=87,Round=1,Position="FO"},
                new SenListFormat { SeqNumSt=1,SeqNumEnd=4,EbgSt=15,EbgEnd=15,BidTypeSt=14,BidTypeEnd=14,LcSt=19,LcEnd=19,EmpIdSt=24,EmpIdEnd=29,NameSt=31,NameEnd=61,ChkPltSt=63,ChkPltEnd=63,AbscenceTypeSt=66,AbscenceTypeEnd=67,AbsenceDatesSt=69,AbsenceDatesEnd=79,Round=2,Position="CP"},
                new SenListFormat { SeqNumSt=1,SeqNumEnd=4,EbgSt=15,EbgEnd=15,BidTypeSt=14,BidTypeEnd=14,LcSt=19,LcEnd=19,EmpIdSt=24,EmpIdEnd=29,NameSt=31,NameEnd=61,ChkPltSt=63,ChkPltEnd=63,AbscenceTypeSt=66,AbscenceTypeEnd=67,AbsenceDatesSt=69,AbsenceDatesEnd=79,Round=2,Position="FO"}

            };
        }



		/// <summary>
		/// Create DWC file--Default weight and Constraints
		/// </summary>
		/// <param name="dwcVersion"></param>
		/// <returns></returns>
		public static WBidIntialState CreateDWCFile(string dwcVersion)
		{

			List<int> lstOff = new List<int>() { };
			List<int> lstWork = new List<int>() { };
			WBidIntialState WbidIntialState = new WBidIntialState();

			WbidIntialState.Version = dwcVersion;

			//Sort section
			WbidIntialState.SortDetails = new SortDetails
			{
				SortColumn = "Line",
				SortDirection = "",
				BlokSort = new List<string>(),
				SortColumnName = ""
			};



			//Constraints
			//----------------------------------------
			Constraints constraint = new Constraints()
			{
				Hard = false,
				Ready = false,
				Reserve = false,
				International = false,
				NonConus = false,
				ETOPS = false,
				// AM_PM = new AMPMConstriants{AM=false,PM=false,MIX=false},
				LrgBlkDayOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 10 },
				AircraftChanges = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
				BlockOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },
				DeadHeads = new Cx4Parameters { SecondcellValue = "1", ThirdcellValue = ((int)DeadheadType.First).ToString(), Type = (int)ConstraintType.LessThan, Value = 1 },
				CL = new CxCommutableLine()
				{
					AnyNight = true,
					RunBoth = false,
					CommuteToHome = true,
					CommuteToWork = true,
					MondayThu = new Times() { Checkin = 0, BackToBase = 0 },
					MondayThuDefault = new Times() { Checkin = 0, BackToBase = 0 },
					Friday = new Times() { Checkin = 0, BackToBase = 0 },
					FridayDefault = new Times() { Checkin = 0, BackToBase = 0 },
					Saturday = new Times() { Checkin = 0, BackToBase = 0 },
					SaturdayDefault = new Times() { Checkin = 0, BackToBase = 0 },
					Sunday = new Times() { Checkin = 0, BackToBase = 0 },
					SundayDefault = new Times() { Checkin = 0, BackToBase = 0 },
					TimesList = new List<Times>()

				},
				Commute = new Commutability { BaseTime = 10, ConnectTime = 30, CheckInTime = 60, SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 },

				DaysOfMonth = new DaysOfMonthCx() { OFFDays = lstOff, WorkDays = lstWork },
				DaysOfWeek = new Cx3Parameters() { ThirdcellValue = ((int)Dow.Tue).ToString(), Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				DeadHeadFoL = new Cx3Parameters { ThirdcellValue = ((int)DeadheadType.First).ToString(), Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				DutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 600 },

				EQUIP = new Cx3Parameters { ThirdcellValue = "700", Type = (int)ConstraintType.MoreThan, Value = 0, lstParameters = new List<Cx3Parameter>() },
				FlightMin = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 7200 },
				GroundTime = new Cx3Parameter { Type = (int)ConstraintType.MoreThan, Value = 1, ThirdcellValue = "30" },
				InterConus = new Cx2Parameters() { Type = (int)CityType.International, Value = 1, lstParameters = new List<Cx2Parameter>() },
				LegsPerDutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
				LegsPerPairing = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18 },
				NumberOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },

				OverNightCities = new Cx3Parameters { ThirdcellValue = "6", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				CitiesLegs = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				BulkOvernightCity = new BulkOvernightCityCx { OverNightNo = new List<int>(), OverNightYes = new List<int>() },
				PDOFS = new Cx4Parameters { SecondcellValue = "300", ThirdcellValue = "400", Type = (int)ConstraintType.atafter, Value = 915, LstParameter = new List<Cx4Parameter>() },
				Position = new Cx3Parameters { Type = (int)ConstraintType.LessThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				StartDayOftheWeek = new Cx3Parameters { SecondcellValue = "1", ThirdcellValue = "6", Type = (int)ConstraintType.MoreThan, Value = 3 },
				Rest = new Cx3Parameters { ThirdcellValue = "1", Type = (int)ConstraintType.LessThan, Value = 8, lstParameters = new List<Cx3Parameter>() },
				PerDiem = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18000 },
				TripLength = new Cx3Parameters { ThirdcellValue = "4", Type = (int)ConstraintType.MoreThan, Value = 1, lstParameters = new List<Cx3Parameter>() },
				WorkBlockLength = new Cx3Parameters { ThirdcellValue = "4", Type = (int)ConstraintType.LessThan, Value = 2, lstParameters = new List<Cx3Parameter>() },
				MinimumPay = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 90 },
				No3On3Off = new Cx2Parameter { Type = (int)ThreeOnThreeOff.NoThreeOnThreeOff, Value = 10 },
				WorkDay = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 11 },
				StartDay = new Cx3Parameters { ThirdcellValue = "1", Type = 1, Value = 1 },
				ReportRelease = new ReportReleases { AllDays = true, First = false, Last = false, NoMid = false, Report = 0, Release = 0 },
				MixedHardReserveTrip = false



			};


			WbidIntialState.Constraints = constraint;


			//Weights
			//----------------------------------------
			Weights weight = new Weights()
			{
				AirCraftChanges = new Wt3Parameter { SecondlValue = 1, ThrirdCellValue = 1, Weight = 0 },
				//AMM,PM, Night
				AM_PM = new Wt2Parameters { Type = 1, Weight = 0, lstParameters = new List<Wt2Parameter>() },
				LrgBlkDayOff = new Wt2Parameter { Weight = 0 },

				BDO = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0
					,
					lstParameters = new List<Wt3Parameter>()
				},
				DHD = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				//Commutable Line
				CL = new WtCommutableLine()
				{
					TimesList = new List<Times>()
					{
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
					},
					DefaultTimes = new List<Times>()
					{
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
					},
					BothEnds = 0,
					InDomicile = 0,
					Type = 1
					//1.  All 2. 

				},

				Commute = new Commutability
				{
					BaseTime = 10,
					ConnectTime = 30,
					CheckInTime = 60,
					SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
					ThirdcellValue = (int)CommutabilityThirdCell.Overall,
					Type = (int)ConstraintType.MoreThan,
					Value = 100,
					Weight = 0
				},
				SDO = new DaysOfMonthWt()
				{
					isWork = false,
					Weights = new List<Wt>()
				},

				DOW = new WtDaysOfWeek()
				{
					lstWeight = new List<Wt>() { new Wt() { Key = 0, Value = 0 } },
					IsOff = true

				},
				DP = new Wt3Parameters()
				{
					SecondlValue = 1,
					ThrirdCellValue = 300,
					Weight = 0
					,
					lstParameters = new List<Wt3Parameter>()
				},

				EQUIP = new Wt3Parameters
				{
					SecondlValue = 700,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				ETOPS = new Wt1Parameters
				{

					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				},
				ETOPSRes = new Wt1Parameters
				{

					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				},
				FLTMIN = new Wt3Parameters
				{
					SecondlValue = 0,
					ThrirdCellValue = 20,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},

				GRD = new Wt3Parameters()
				{
					SecondlValue = 0,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				InterConus = new Wt2Parameters()
				{
					Type = -1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				LEGS = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				WtLegsPerPairing = new Wt3Parameters()
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},

				NODO = new Wt2Parameters
				{
					Type = 9,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				RON = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				CitiesLegs = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				SDOW = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				WtRest = new Wt4Parameters
				{
					FirstValue = 1,
					SecondlValue = 480,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt4Parameter>()
				},
				PerDiem = new Wt2Parameter
				{
					Type = 100,
					Weight = 0,

				},
				TL = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				WB = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				POS = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				DHDFoL = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				WorkDays = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},

				PDAfter = new Wt4Parameters
				{
					FirstValue = 300,
					SecondlValue = 180,
					ThrirdCellValue = 400,
					Weight = 0,
					lstParameters = new List<Wt4Parameter>()
				},
				PDBefore = new Wt4Parameters
				{
					FirstValue = 300,
					SecondlValue = 180,
					ThrirdCellValue = 400,
					Weight = 0,
					lstParameters = new List<Wt4Parameter>()
				},
				NormalizeDaysOff = new Wt2Parameter { Type = 1, Weight = 0 }

			};

			WbidIntialState.Weights = weight;



			WbidIntialState.Theme = new Theme()
			{
				FontType = "Arial",
				FontSize = "10",
				Background = new BackgroundColor()
				{
					DialogColor = "#FFFFFFFF",
					TextColor = "#FFE6E6FA",
					AMTripsColor = "#FFDB7093",
					PMTripsColor = "#FF6495ED",
					MixedTripsColor = "#FFFFFF00",
					AMReserveColor = "#FFFFC0CB",
					PMReserveColor = "#FFB0C4DE",
					ReadyReserveColor = "#FFA0522D",
					VacationOverlapColor = "#FF98FB98",
					VacationDropColor = "#FFFF0000",
					VacationColor = "#FF00FF00"
				},

				TextColor = new TextColor()
				{
					DialogText = "#FF000000",
					TextBox = "#FF000000",
					AMTripsText = "#FF000000",
					PMTripsText = "#FF000000",
					MixedTripsText = "#FF000000",
					AMReserveText = "#FF000000",
					PMReserveText = "#FF000000",
					ReadyReserveText = "#FF000000",
					VacationOverlapText = "#FF000000",
					VacationDropText = "#FF000000",
					VacationText = "#FF000000"
				}

			};
			// WbidIntialState.CheckBoxState = new CheckBoxState() { IsVacationCorrectionChecked = false, IsVacationDropChecked = false, IsOverlapChecked = false, IsEOMChecked = false };
			WbidIntialState.MenuBarButtonStatus = new MenuBarButtonStatus() { IsVacationCorrection = false, IsVacationDrop = false, IsOverlap = false, IsEOM = false };
			WbidIntialState.CxWtState = new CxWtState()
			{
				ACChg = new StateStatus() { Cx = false, Wt = false },
				AMPM = new StateStatus() { Cx = false, Wt = false },
				BDO = new StateStatus() { Cx = false, Wt = false },
				DHD = new StateStatus() { Cx = false, Wt = false },
				CL = new StateStatus() { Cx = false, Wt = false },
				Commute = new StateStatus() { Cx = false, Wt = false },
				DOW = new StateStatus() { Cx = false, Wt = false },
				DP = new StateStatus() { Cx = false, Wt = false },
				EQUIP = new StateStatus() { Cx = false, Wt = false },
				ETOPS = new StateStatus() { Cx = false, Wt = false },
				ETOPSRes = new StateStatus() { Cx = false, Wt = false },
				FLTMIN = new StateStatus() { Cx = false, Wt = false },
				GRD = new StateStatus() { Cx = false, Wt = false },
				InterConus = new StateStatus { Cx = false, Wt = false },
				LEGS = new StateStatus() { Cx = false, Wt = false },
				NODO = new StateStatus() { Cx = false, Wt = false },
				RON = new StateStatus() { Cx = false, Wt = false },
				SDO = new StateStatus() { Cx = false, Wt = false },
				SDOW = new StateStatus() { Cx = false, Wt = false },
				TL = new StateStatus() { Cx = false, Wt = false },
				WB = new StateStatus() { Cx = false, Wt = false },
				LegsPerPairing = new StateStatus() { Cx = false, Wt = false },
				WtPDOFS = new StateStatus() { Cx = false, Wt = false },
				Rest = new StateStatus() { Cx = false, Wt = false },
				PerDiem = new StateStatus() { Cx = false, Wt = false },
				MP = new StateStatus() { Cx = false, Wt = false },
				NOL = new StateStatus() { Cx = false, Wt = false },
				No3on3off = new StateStatus() { Cx = false, Wt = false },
				LrgBlkDaysOff = new StateStatus() { Cx = false, Wt = false },
				Position = new StateStatus() { Cx = false, Wt = false },
				DHDFoL = new StateStatus() { Cx = false, Wt = false },
				AMPMMIX = new AMPMConstriants() { AM = false, PM = false, MIX = false },
				DaysOfWeek = new DaysOfWeekConstraints() { SUN = false, MON = false, TUE = false, WED = false, THU = false, FRI = false, SAT = false },
				FaPosition = new PostionConstraint() { A = false, B = false, C = false, D = false },
				TripLength = new TripLengthConstraints() { Turns = false, Twoday = false, ThreeDay = false, FourDay = false },
				WorkDay = new StateStatus() { Cx = false, Wt = false },
				PDAfter = new StateStatus() { Cx = false, Wt = false },
				PDBefore = new StateStatus() { Cx = false, Wt = false },
				NormalizeDays = new StateStatus() { Cx = false, Wt = false },
				BulkOC = new StateStatus() { Cx = false, Wt = false },
                StartDay=new StateStatus(){Cx=false,Wt=false},
                ReportRelease=new StateStatus() { Cx = false, Wt = false },
				MixedHardReserveTrip= new StateStatus() { Cx = false, Wt = false }

			};
			return WbidIntialState;

		}
		public static WBidStateCollection CreateStateFile(string fileName, int lineCount, int startValue, WBidIntialState WBidIntialState)
		{
			WBidStateCollection stateCollection = CreateStateFileObject(fileName, lineCount, startValue, WBidIntialState);

			return stateCollection;


		}
		public static WBidStateCollection CreateStateFileObject(string fileName, int lineCount, int startValue, WBidIntialState WBidIntialState)
		{

			WBidState state = new WBidState();
			state.CxWtState = WBidIntialState.CxWtState;

			state.SortDetails = WBidIntialState.SortDetails;

			state.Theme = WBidIntialState.Theme;


			LineOrder lineOrder;

			state.LineOrders = new LineOrders();
			state.LineOrders.Lines = lineCount;

			///Generate Line sections for state file.
			for (int count = 1; count <= lineCount; count++)
			{
				lineOrder = new LineOrder() { OId = count, LId = startValue++ };
				state.LineOrders.Orders.Add(lineOrder);
			}

			state.Constraints = WBidIntialState.Constraints;

			state.Weights = WBidIntialState.Weights;


			// state.CheckBoxState = new CheckBoxState() { IsVacationCorrectionChecked = false, IsVacationDropChecked = false, IsOverlapChecked = false, IsEOMChecked = false };

			state.MenuBarButtonState = new MenuBarButtonStatus() { IsVacationCorrection = false, IsVacationDrop = false, IsOverlap = false, IsEOM = false };

			state.ForceLine = new ForceLine();
			state.StateName = "Default";

			state.TagDetails = new TagDetails();


			if (decimal.Parse(WBidIntialState.Version) < 2.0m)
			{
				if (state.Constraints.BulkOvernightCity == null)
				{
					state.Constraints.BulkOvernightCity = new BulkOvernightCityCx() { OverNightYes = new List<int>(), OverNightNo = new List<int>() };

				}
				if (state.Weights.OvernightCitybulk == null)
				{
					state.Weights.OvernightCitybulk = new List<Wt2Parameter>();

				}
				if (state.CxWtState.BulkOC == null)
				{
					state.CxWtState.BulkOC = new StateStatus() { Cx = false, Wt = false };
				}
			}
			if (decimal.Parse(WBidIntialState.Version) < 2.1m)
			{
				state.Constraints.No3On3Off = new Cx2Parameter { Type = (int)ThreeOnThreeOff.NoThreeOnThreeOff, Value = 10 };
			}
			if ((double.Parse(WBidIntialState.Version) < 2.3))
			{
				WBidIntialState.CxWtState.Commute = new StateStatus { Cx = false, Wt = false };
				WBidIntialState.Constraints.Commute = new Commutability { SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };
				WBidIntialState.Weights.Commute = new Commutability { SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 };

			}
			if ((double.Parse(WBidIntialState.Version) < 2.4))
			{
				//setting the default value for the PDO to any city and any date
				state.Constraints.PDOFS.SecondcellValue = "300";
				state.Constraints.PDOFS.ThirdcellValue = "400";
				state.Weights.PDAfter.FirstValue = 300;
				state.Weights.PDAfter.ThrirdCellValue = 400;

				state.Weights.PDBefore.FirstValue = 300;
				state.Weights.PDBefore.ThrirdCellValue = 400;
			}
			if ((double.Parse(WBidIntialState.Version) < 2.8))
			{
				//setting the default value for the PDO to any city and any date
				state.Constraints.EQUIP.ThirdcellValue = "700";
				state.Weights.EQUIP.SecondlValue = 700;

			}
            if ((double.Parse(WBidIntialState.Version) < 2.8))
            {
                if (state.CxWtState.StartDay == null)
                    state.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
                if (state.CxWtState.ReportRelease == null)
                    state.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };
                if (state.Constraints.StartDay == null)
                    state.Constraints.StartDay = new Cx3Parameters { ThirdcellValue = "1", Type = 1, Value = 1 };
                if (state.Constraints.ReportRelease == null)
                    state.Constraints.ReportRelease = new ReportReleases { AllDays = true, First = false, Last = false, NoMid = false, Report = 0, Release = 0 };

            }
			if (state.CxWtState.ETOPS == null)
			{
				state.CxWtState.ETOPS = new StateStatus { Cx = false, Wt = false };
			}
			if (state.CxWtState.ETOPSRes == null)
			{
				state.CxWtState.ETOPSRes = new StateStatus { Cx = false, Wt = false };
			}
			if (state.Weights.ETOPS == null)
			{
				state.Weights.ETOPS = new Wt1Parameters
				{
					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				};
			}
			if (state.Weights.ETOPSRes == null)
			{
				state.Weights.ETOPSRes = new Wt1Parameters
				{
					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				};
			}
			WBidStateCollection stateCollection = new WBidStateCollection();
			stateCollection.Version = GlobalSettings.StateFileVersion;
			stateCollection.DefaultName = "Default";
			stateCollection.DataSource = "Original";
			stateCollection.IsModified = true;
			stateCollection.SyncVersion = "0";
			stateCollection.StateUpdatedTime = DateTime.MinValue.ToUniversalTime();

			stateCollection.StateList = new List<WBidState>();


			stateCollection.StateList.Add(state);

			return stateCollection;
		}
		public static List<DataColumn> GenerateDefaultColumns()
		{
			return new List<DataColumn>
			{

				new DataColumn{ Order=1,Id=0,Width=50},
				new DataColumn{Order=2,Id=1,Width=50},
				new DataColumn{Order=3,Id=2,Width=50},
				new DataColumn{Order=4,Id=3,Width=50},
				new DataColumn{Order=5,Id=67,Width=50},
                //new DataColumn{Order=5,Id=4,Width=50},
                new DataColumn{Order=6,Id=5,Width=50},
				new DataColumn{Order=7,Id=38,Width=50},
				new DataColumn{Order=8,Id=62,Width=50},
				new DataColumn{Order=9,Id=36,Width=50},
				new DataColumn{Order=10,Id=44,Width=50},
				new DataColumn{Order=11,Id=18,Width=50},
				new DataColumn{Order=12,Id=34,Width=50},
				new DataColumn{Order=13,Id=41,Width=50},
				new DataColumn{Order=14,Id=27,Width=50},
				new DataColumn{Order=15,Id=8,Width=50},
				new DataColumn{Order=16,Id=6,Width=50},
				new DataColumn{Order=17,Id=37,Width=50}
			};

		}

		public static List<DataColumn> GenerateDefaultVacationColumns()
		{
			return new List<DataColumn>
			{

				new DataColumn{ Order=1,Id=0,Width=50},
				new DataColumn{Order=2,Id=1,Width=50},
				new DataColumn{Order=3,Id=2,Width=50},
				new DataColumn{Order=4,Id=3,Width=50},
				new DataColumn{Order=5,Id=67,Width=50},

				//new DataColumn{Order=5,Id=4,Width=50},
				new DataColumn{Order=6,Id=5,Width=50},
				new DataColumn{Order=7,Id=38,Width=50},
				new DataColumn{Order=8,Id=62,Width=50},
				new DataColumn{Order=9,Id=36,Width=50},
				new DataColumn{Order=10,Id=44,Width=50},
				new DataColumn{Order=11,Id=18,Width=50},
				new DataColumn{Order=12,Id=34,Width=50},
				new DataColumn{Order=13,Id=41,Width=50},
				new DataColumn{Order=14,Id=27,Width=50},
				//new DataColumn{Order=15,Id=8,Width=50},
				//new DataColumn{Order=16,Id=6,Width=50},
				//new DataColumn{Order=17,Id=37,Width=50}
                new DataColumn{Order=15,Id=53,Width=50},
                new DataColumn{Order=16,Id=54,Width=50},
                new DataColumn{Order=17,Id=55,Width=50}
			};

		}




		/// <summary>
		/// Genarate the required files to downlaod from the server
		/// </summary>
		/// <param name="SelectedBidRound"></param>
		/// <param name="SelectedPosition"></param>
		/// <param name="SelectedDomicile"></param>
		/// <param name="SelectedBidPeriod"></param>
		/// <returns></returns>
		public static List<string> GenarateDownloadFileslist(BidDetails bidDetails)
		{
			BidRound SelectedBidRound = GetBidRounds().FirstOrDefault(x => x.ShortStr == bidDetails.Round);
			Position SelectedPosition = GetPositions().FirstOrDefault(x => x.LongStr == bidDetails.Postion);
			BidPeriod SelectedBidPeriod = GetBidPeriods().FirstOrDefault(x => x.BidPeriodId == bidDetails.Month);
			List<string> _downloadFiles = new List<string>();
			//First Round
			if (SelectedBidRound.ShortStr == "D")
			{
				//.737 file
				_downloadFiles.Add(SelectedPosition.ShortStr + "D" + bidDetails.Domicile + SelectedBidPeriod.HexaValue + ".737");

				// C cover letter
				_downloadFiles.Add(bidDetails.Domicile + SelectedPosition.LongStr + "C" + ".TXT");

				//S seniority list
				_downloadFiles.Add(bidDetails.Domicile + SelectedPosition.LongStr + "S" + ".TXT");

				// hotel list


				_downloadFiles.Add(((SelectedPosition.ShortStr == "A") ? "F" : "P") + bidDetails.Domicile + SelectedBidPeriod.BidPeriodId.ToString("d2") + ".DAT");
				//DateTime.Now.AddMonths(1).Month.ToString("d2") 

			}
			//Second round
			else
			{
				//.737 file
				_downloadFiles.Add(SelectedPosition.ShortStr + "B" + bidDetails.Domicile + SelectedBidPeriod.HexaValue + ".737");
				// N line view
				_downloadFiles.Add(bidDetails.Domicile + SelectedPosition.LongStr + "N" + ".TXT");

				//hotel list
				_downloadFiles.Add(((SelectedPosition.ShortStr == "A") ? "F" : "P") + bidDetails.Domicile + SelectedBidPeriod.BidPeriodId.ToString("d2") + ".DAT");

				//DateTime.Now.AddMonths(1).Month.ToString("d2")
				if (SelectedPosition.ShortStr == "A")
				{
					// get flight attendant 2nd round cover letter and seniority list
					_downloadFiles.Add(bidDetails.Domicile + SelectedPosition.LongStr + "CR" + ".TXT");
					_downloadFiles.Add(bidDetails.Domicile + SelectedPosition.LongStr + "SR" + ".TXT");
				}
				else
				{
					// get pilot 2nd round cover letter and seniority list
					_downloadFiles.Add(bidDetails.Domicile + SelectedPosition.LongStr + "R" + ".TXT");
				}
			}
			return _downloadFiles;
		}


		/// <summary>
		/// this will returns Full Date  in datettime format ,When pass the date of the trip.
		/// </summary>
		/// <param name="day"></param>
		/// <returns></returns>
		public static DateTime SetDate(int day, bool isLastTrip, BidDetails currentBidDetails)
		{

			//  Jan:	Jan 1 to Jan 30
			//   Feb:	Jan 31 to Mar 1
			//    Mar:	Mar 2 to Mar 31

			var tempMonth = currentBidDetails.Month;
			if (currentBidDetails.Postion == "FA" && currentBidDetails.Month == 2)
			{
				if (day == 31)
				{
					tempMonth--;
				}
				//Eg BAQW01---  in this case  trip start date may either February 1 or  march 1
				else if (day == 1 && isLastTrip)
				{
					tempMonth++;
				}
			}
			return new DateTime(currentBidDetails.Year, tempMonth, day);

		}
		/// <summary>
		/// this will returns Full Date  in datettime format ,When pass the date of the trip.
		/// </summary>
		/// <param name="day"></param>
		/// <returns></returns>
		public static DateTime SetDate(int day, bool isLastTrip)
		{

			//  Jan:	Jan 1 to Jan 30
			//   Feb:	Jan 31 to Mar 1
			//    Mar:	Mar 2 to Mar 31

			var tempMonth = GlobalSettings.CurrentBidDetails.Month;
			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Month == 2)
			{
				if (day == 31)
				{
					tempMonth--;
				}
				//Eg BAQW01---  in this case  trip start date may either February 1 or  march 1
				else if (day == 1 && isLastTrip)
				{
					tempMonth++;
				}
			}
			return new DateTime(GlobalSettings.CurrentBidDetails.Year, tempMonth, day);

		}

		/// <summary>
		/// PURPOSE : Compare INI file  and WBID update file, overwrite the .ini file if any updates found in WBid Update file
		/// </summary>
		public static bool UpdateINIFile(WBidUpdate WBidUpdate)
		{
			bool isUpdateFound = false;


			//Check cities section
			if (GlobalSettings.WBidINIContent.Updates.Cities != WBidUpdate.Updates.Cities && WBidUpdate.Updates.Cities != 0)
			{
				GlobalSettings.WBidINIContent.Updates.Cities = WBidUpdate.Updates.Cities;

				//Update the cities list
				GlobalSettings.WBidINIContent.Cities = WBidUpdate.Cities.OrderBy(x=>x.Name).ToList();

				isUpdateFound = true;
			}

			//Check domiciles section
			if (GlobalSettings.WBidINIContent.Updates.Domiciles != WBidUpdate.Updates.Domiciles && WBidUpdate.Updates.Domiciles != 0)
			{
				GlobalSettings.WBidINIContent.Updates.Domiciles = WBidUpdate.Updates.Domiciles;

				//Update the Domiciles list
				GlobalSettings.WBidINIContent.Domiciles = WBidUpdate.Domiciles;

				isUpdateFound = true;
			}

			//Check Equipment section
			if (GlobalSettings.WBidINIContent.Updates.Equipment != WBidUpdate.Updates.Equipment && WBidUpdate.Updates.Equipment != 0)
			{
				GlobalSettings.WBidINIContent.Updates.Equipment = WBidUpdate.Updates.Equipment;

				//Update the Equipment list
				GlobalSettings.WBidINIContent.Equipments = WBidUpdate.Equipments;

				isUpdateFound = true;
			}


			//Check Equipment section
			if (GlobalSettings.WBidINIContent.Updates.EquipTypes != WBidUpdate.Updates.EquipTypes && WBidUpdate.Updates.EquipTypes != 0)
			{
				GlobalSettings.WBidINIContent.Updates.EquipTypes = WBidUpdate.Updates.EquipTypes;

				//Update the EquipTypes list
				GlobalSettings.WBidINIContent.EquipTypes = WBidUpdate.EquipTypes;

				isUpdateFound = true;
			}


			//Check Equipment section
			if (GlobalSettings.WBidINIContent.Updates.Hotels != WBidUpdate.Updates.Hotels && WBidUpdate.Updates.Hotels != 0)
			{
				GlobalSettings.WBidINIContent.Updates.Hotels = WBidUpdate.Updates.Hotels;


				if (GlobalSettings.WBidINIContent.Hotels == null)
					GlobalSettings.WBidINIContent.Hotels = new Hotels();

				GlobalSettings.WBidINIContent.Hotels.HotelList = WBidUpdate.Hotels.HotelList;
				GlobalSettings.WBidINIContent.Hotels.Effective = WBidUpdate.Hotels.Effective;

				//Update the Hotels list


				isUpdateFound = true;
			}
			//Check News section
			if (GlobalSettings.WBidINIContent.Updates.News != WBidUpdate.Updates.News && WBidUpdate.Updates.News != 0)
			{
				GlobalSettings.WBidINIContent.Updates.News = WBidUpdate.Updates.News;

				isUpdateFound = true;
			}
			if (GlobalSettings.WBidINIContent.Updates.SplitPointCities != WBidUpdate.Updates.News && WBidUpdate.Updates.SplitPointCities != 0)
			{
				GlobalSettings.WBidINIContent.Updates.SplitPointCities = WBidUpdate.Updates.SplitPointCities;
				GlobalSettings.WBidINIContent.SplitPointCities = WBidUpdate.SplitPointCities;
				isUpdateFound = true;
				
			}

			return isUpdateFound;


		}

        public static ObservableCollection<BlockSort> GetBlockSortListData()
        {
            // if the block sort is commutability ,we have to assign manula ids 
            //30 commutability overall
            //31 commutability Front
            //32 commutability Back3

            ObservableCollection<BlockSort> blk = new ObservableCollection<BlockSort>()
            {     new BlockSort{Id=1,Name="AMs then PMs"},
                new BlockSort{Id=2,Name="Blocks of Day Off"},
               new BlockSort{Id=3,Name="Days Off"},
                new BlockSort{Id=16,Name="Duty Periods (asc)"},
                new BlockSort{Id=4,Name="Flight Time"},
                 new BlockSort{Id=5,Name="Largest Blk of Days Off"},
                new BlockSort{Id=6,Name="Pay Credit"},
                new BlockSort{Id=7,Name="Pay per Day"},
                new BlockSort{Id=8,Name="Pay Per Duty Hour"},
                new BlockSort{Id=9,Name="Pay per Flight Hour"},
                new BlockSort{Id=10,Name="Pay per TAFB"},
                new BlockSort{Id=11,Name="Per Diem"},
                new BlockSort{Id=12,Name="PMs then AMs"},
                 new BlockSort{Id=19,Name="Ratio"},
                new BlockSort{Id=13,Name="Start Day of Week"},
                new BlockSort{Id=17,Name="VacPay Current Bid Period"},
                new BlockSort{Id=20,Name="VacPay Next Bid Period"},
                new BlockSort{Id=21,Name="VacPay Both Current and Next"},
                 new BlockSort{Id=14,Name="Weekday Pairings"},
                new BlockSort{Id=15,Name="Weights"},
                new BlockSort { Id = 22, Name = "ETOPS" },
                new BlockSort { Id = 23, Name = "Commutable Line-Auto"},
                new BlockSort { Id = 24, Name = "Commutable Line- Manual"},
                new BlockSort{Id=26,Name="Line Rig"},
                new BlockSort{Id=27,Name="Vac+LG"},
				new BlockSort{Id=28,Name="ETOPS Trips"},
				 

			};

            blk = new ObservableCollection<BlockSort>(blk.OrderBy(x => x.Name));
            return blk;

        }
        public static ObservableCollection<BlockSort> GetBlockSortListDataCSW()
        {
            // if the block sort is commutability ,we have to assign manula ids 
            //30 commutability overall
            //31 commutability Front
            //32 commutability Back3

            ObservableCollection<BlockSort> blk = new ObservableCollection<BlockSort>()
            {
                new BlockSort{Id=1,Name="AMs then PMs"},
                new BlockSort{Id=2,Name="Blocks of Day Off"},
                //new BlockSort{Id=18, Name="Commutability",IsVisibleOptions=true,SelectedBlockSort=1},
                new BlockSort{Id=3,Name="Days Off"},
                new BlockSort{Id=16,Name="Duty Periods (asc)"},
                new BlockSort{Id=4,Name="Flight Time"},
                new BlockSort{Id=5,Name="Largest Blk of Days Off"},
                new BlockSort{Id=6,Name="Pay Credit"},
                new BlockSort{Id=7,Name="Pay per Day"},
                new BlockSort{Id=8,Name="Pay Per Duty Hour"},
                new BlockSort{Id=9,Name="Pay per Flight Hour"},
                new BlockSort{Id=10,Name="Pay per TAFB"},
                new BlockSort{Id=11,Name="Per Diem"},
                new BlockSort{Id=12,Name="PMs then AMs"},
                new BlockSort{Id=19,Name="Ratio"},
                new BlockSort{Id=13,Name="Start Day of Week"},
                new BlockSort{Id=17,Name="VacPay Current Bid Period"},
                new BlockSort{Id=20,Name="VacPay Next Bid Period"},
                new BlockSort{Id=21,Name="VacPay Both Current and Next"},
                new BlockSort{Id=14,Name="Weekday Pairings"},
                new BlockSort{Id=15,Name="Weights"},
                new BlockSort { Id = 22, Name = "ETOPS" },
                new BlockSort { Id = 23, Name = "Commutable Line-Auto"},
                new BlockSort { Id = 24, Name = "Commutable Line- Manual"},
                new BlockSort{Id=26,Name="Line Rig"},
                new BlockSort{Id=27,Name="Vac+LG"},
				new BlockSort{Id=28,Name="ETOPS Trips"},
				 new BlockSort{Id=39,Name="DH First"},
				   new BlockSort{Id=40,Name="DH Last"},
					new BlockSort{Id=41,Name="DH Total"}

			};

            blk = new ObservableCollection<BlockSort>(blk.OrderBy(x => x.Name));
            return blk;
        }
        public static ObservableCollection<BlockSort> GetBlockSortListDataForInternalCalculation()
        {

            // if the block sort is commutability ,we have to assign manula ids 
            //30 commutability overall
            //31 commutability Front
            //32 commutability Back
            return new ObservableCollection<BlockSort>()
            {
                new BlockSort{Id=1,Name="AMs then PMs"},
                new BlockSort{Id=2,Name="Blocks of Day Off"},
                new BlockSort{Id=3,Name="Days Off"},
                new BlockSort{Id=16,Name="Duty Periods (asc)"},
                new BlockSort{Id=4,Name="Flight Time"},
                new BlockSort{Id=5,Name="Largest Blk of Days Off"},
                new BlockSort{Id=6,Name="Pay Credit"},
                new BlockSort{Id=7,Name="Pay per Day"},
                new BlockSort{Id=8,Name="Pay Per Duty Hour"},
                new BlockSort{Id=9,Name="Pay per Flight Hour"},
                new BlockSort{Id=10,Name="Pay per TAFB"},
                new BlockSort{Id=11,Name="Per Diem"},
                new BlockSort{Id=12,Name="PMs then AMs"},
                new BlockSort{Id=19,Name="Ratio"},
                new BlockSort{Id=13,Name="Start Day of Week"},
                new BlockSort{Id=17,Name="VacPay Current Bid Period"},
                new BlockSort{Id=20,Name="VacPay Next Bid Period"},
                new BlockSort{Id=21,Name="VacPay Both Current and Next"},
                new BlockSort{Id=14,Name="Weekday Pairings"},
                new BlockSort{Id=15,Name="Weights"},
                new BlockSort{Id=30,Name="Commutability Overall"},
                new BlockSort{Id=31,Name="Commutability Front"},
                new BlockSort{Id=32,Name="Commutability Back"},
                new BlockSort{Id=22,Name="ETOPS"},
                new BlockSort{Id=33,Name="CommutableLine-Auto Overall"},
                new BlockSort{Id=34,Name="CommutableLine-Auto Front"},
                new BlockSort{Id=35,Name="CommutableLine-Auto Back"},
                new BlockSort{Id=36,Name="CommutableLine-Manual Overall"},
                new BlockSort{Id=37,Name="CommutableLine-Manual Front"},
                new BlockSort{Id=38,Name="CommutableLine-Manual Back"},
                new BlockSort{Id=26,Name="Line Rig"},
                new BlockSort{Id=27,Name="Vac+LG"},
				new BlockSort{Id=28,Name="ETOPS Trips"},
				 new BlockSort{Id=39,Name="DH First"},
				   new BlockSort{Id=40,Name="DH Last"},
					new BlockSort{Id=41,Name="DH Total"}



			};
        }
		public static List<Absense> GetOrderedAbsenceDates()
		{
			List<Absense> absence = new List<Absense>();
			if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null)
			{
				List<Absense> sortedAbsencelist = GlobalSettings.SeniorityListMember.Absences.OrderBy(x => x.StartAbsenceDate).Where(y => y.AbsenceType == "VA").ToList();
				if (sortedAbsencelist.Count > 0)
				{
					absence.Add(new Absense { StartAbsenceDate = sortedAbsencelist.FirstOrDefault().StartAbsenceDate, EndAbsenceDate = sortedAbsencelist.FirstOrDefault().EndAbsenceDate, AbsenceType = "VA" });

					for (int count = 0; count < sortedAbsencelist.Count - 1; count++)
					{
						if ((sortedAbsencelist[count + 1].StartAbsenceDate - sortedAbsencelist[count].EndAbsenceDate).Days == 1)
						{
							absence[absence.Count - 1].EndAbsenceDate = sortedAbsencelist[count + 1].EndAbsenceDate;
						}
						else
						{
							absence.Add(new Absense { StartAbsenceDate = sortedAbsencelist[count + 1].StartAbsenceDate, EndAbsenceDate = sortedAbsencelist[count + 1].EndAbsenceDate, AbsenceType = "VA" });
						}
					}
				}
			}
			//absence.Add(new Absense() { AbsenceType = "VA", StartAbsenceDate = new DateTime(2013, 12, 1), EndAbsenceDate = new DateTime(2013, 12, 7) });
			return absence;
		}

        public static List<Absense> GetFVOrderedAbsenceDates()
        {
            List<Absense> absence = new List<Absense>();
            if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null)
            {
                List<Absense> sortedAbsencelist = GlobalSettings.SeniorityListMember.Absences.OrderBy(x => x.StartAbsenceDate).Where(y => y.AbsenceType == "FV").ToList();
                if (sortedAbsencelist.Count > 0)
                {
                    absence.Add(new Absense { StartAbsenceDate = sortedAbsencelist.FirstOrDefault().StartAbsenceDate, EndAbsenceDate = sortedAbsencelist.FirstOrDefault().EndAbsenceDate, AbsenceType = "FV" });

                    for (int count = 0; count < sortedAbsencelist.Count - 1; count++)
                    {
                        if ((sortedAbsencelist[count + 1].StartAbsenceDate - sortedAbsencelist[count].EndAbsenceDate).Days == 1)
                        {
                            absence[absence.Count - 1].EndAbsenceDate = sortedAbsencelist[count + 1].EndAbsenceDate;
                        }
                        else
                        {
                            absence.Add(new Absense { StartAbsenceDate = sortedAbsencelist[count + 1].StartAbsenceDate, EndAbsenceDate = sortedAbsencelist[count + 1].EndAbsenceDate, AbsenceType = "FV" });
                        }
                    }
                }
            }
            //absence.Add(new Absense() { AbsenceType = "VA", StartAbsenceDate = new DateTime(2013, 12, 1), EndAbsenceDate = new DateTime(2013, 12, 7) });
            return absence;
        }
		public static List<Absense> GetCFVOrderedAbsenceDates()
		{
			List<Absense> absence = new List<Absense>();
			if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null)
			{
				List<Absense> sortedAbsencelist = GlobalSettings.SeniorityListMember.Absences.OrderBy(x => x.StartAbsenceDate).Where(y => y.AbsenceType == "CFV").ToList();
				if (sortedAbsencelist.Count > 0)
				{
					absence.Add(new Absense { StartAbsenceDate = sortedAbsencelist.FirstOrDefault().StartAbsenceDate, EndAbsenceDate = sortedAbsencelist.FirstOrDefault().EndAbsenceDate, AbsenceType = "CFV" });

					for (int count = 0; count < sortedAbsencelist.Count - 1; count++)
					{
						if ((sortedAbsencelist[count + 1].StartAbsenceDate - sortedAbsencelist[count].EndAbsenceDate).Days == 1)
						{
							absence[absence.Count - 1].EndAbsenceDate = sortedAbsencelist[count + 1].EndAbsenceDate;
						}
						else
						{
							absence.Add(new Absense { StartAbsenceDate = sortedAbsencelist[count + 1].StartAbsenceDate, EndAbsenceDate = sortedAbsencelist[count + 1].EndAbsenceDate, AbsenceType = "CFV" });
						}
					}
				}
			}
			//absence.Add(new Absense() { AbsenceType = "VA", StartAbsenceDate = new DateTime(2013, 12, 1), EndAbsenceDate = new DateTime(2013, 12, 7) });
			return absence;
		}
		/// <summary>
		/// PURPOSE : Set Application Title 
		/// </summary>
		public static string SetTitile()
		{

			string domicile = GlobalSettings.CurrentBidDetails.Domicile;
			string position = GlobalSettings.CurrentBidDetails.Postion;
			System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
			string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).Substring(0, 3);
			string round = GlobalSettings.CurrentBidDetails.Round == "M" ? "1st Rnd" : "2nd Rnd";
			string dataSource = string.Empty;
			string stateName = string.Empty;
			string seniority = string.Empty;
			string wBidiTitle;
			//will need to enable after the state file implemntation
			if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource == "HistoricalData")
			{
				dataSource = " (Historical Data)";
			}
			else if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidStateCollection.DataSource != "Original")
			{
				dataSource = " (Mock - Data)";

			}

			//if (GlobalSettings.WBidStateContent != null && GlobalSettings.WBidStateContent.StateName != "Default")
			//{
			//    stateName = " State : " + GlobalSettings.WBidStateContent.StateName;

			//}
			string equipment = string.Empty;
			if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.Equipments != null && GlobalSettings.WBidINIContent.Equipments.Count > 0)
				equipment = GlobalSettings.WBidINIContent.Equipments[0].EquipmentNumber.ToString();
			//if (IsOverlapCorrection)
			//{
			//    WBidiTitle = domicile + "/" + position + "/" + equipment + " " + round + "  Line for " + strMonthName + " " + GlobalSettings.CurrentBidDetails.Year + "( Corrected for Overlap )";
			//}
			//else
			//{
			wBidiTitle = domicile + " - " + position + " - " + round + " - " + strMonthName + " - " + GlobalSettings.CurrentBidDetails.Year;
			//}
			var qamode = string.Empty;
			if (GlobalSettings.buddyBidTest)
			{
				qamode = "  ( QA Mode )";
			}
			if (GlobalSettings.WBidStateCollection.SeniorityListItem != null)
			{
				if (GlobalSettings.CurrentBidDetails.Postion != "FA" || GlobalSettings.CurrentBidDetails.Round != "S")
				{
					if (GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0)
						seniority = " (Not in Domicile) ";
					else
						seniority = " ( " + GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber + " of " + GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount + " )";
				}
			}
			var servermode = string.Empty;
			if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.IsConnectedVofox)
				servermode = "(Vofox server)";
			else
				servermode = string.Empty;
			var title = wBidiTitle += dataSource + stateName + seniority + qamode;

			return title;
		}

		/// <summary>
		/// for prepare bid line view to handle EOM vacation
		/// </summary>
		public static void GenarateTempAbsenceList()
		{
			DateTime nextBidPeriodVacationStartDate = DateTime.MinValue;
			if (GlobalSettings.CurrentBidDetails.Postion != "FA")
			{
				nextBidPeriodVacationStartDate = GetnextSunday();
			}
			else
			{
				nextBidPeriodVacationStartDate = GlobalSettings.FAEOMStartDate;

			}
			//           if (IsVacCorrection && IsEOMChecked)
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && GlobalSettings.MenuBarButtonStatus.IsEOM)
			{
				GlobalSettings.TempOrderedVacationDays = new List<Absense>();
				if (GlobalSettings.OrderedVacationDays != null)
				{
					foreach (var item in GlobalSettings.OrderedVacationDays)
					{
						GlobalSettings.TempOrderedVacationDays.Add(new Absense { StartAbsenceDate = item.StartAbsenceDate, EndAbsenceDate = item.EndAbsenceDate, AbsenceType = item.AbsenceType });
					}
				}


				Absense absence = GlobalSettings.TempOrderedVacationDays.FirstOrDefault(x => x.EndAbsenceDate == nextBidPeriodVacationStartDate.AddDays(-1));
				if (absence != null)
				{
					absence.EndAbsenceDate = nextBidPeriodVacationStartDate.AddDays(6);
				}
				else
					GlobalSettings.TempOrderedVacationDays.Add(new Absense { StartAbsenceDate = nextBidPeriodVacationStartDate, EndAbsenceDate = nextBidPeriodVacationStartDate.AddDays(6), AbsenceType = "VA" });
			}
			else if (GlobalSettings.MenuBarButtonStatus.IsEOM)
			{

				GlobalSettings.TempOrderedVacationDays = new List<Absense>() { new Absense {
							StartAbsenceDate = nextBidPeriodVacationStartDate,
							EndAbsenceDate = nextBidPeriodVacationStartDate.AddDays (6),
							AbsenceType = "VA"
						} };


			}
			else if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
			{
				GlobalSettings.TempOrderedVacationDays = new List<Absense>();
				if (GlobalSettings.OrderedVacationDays != null)
				{
					foreach (var item in GlobalSettings.OrderedVacationDays)
					{
						GlobalSettings.TempOrderedVacationDays.Add(new Absense { StartAbsenceDate = item.StartAbsenceDate, EndAbsenceDate = item.EndAbsenceDate, AbsenceType = item.AbsenceType });
					}
				}
			}
			//else if (!IsVacCorrection && !IsEOMChecked && !IsVacDrop)
			else if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM && !GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
			{
				GlobalSettings.TempOrderedVacationDays = null;
			}
		}

		/// <summary>
		/// get the Next sunday after the current bid period end date
		/// </summary>
		/// <returns></returns>
		public static DateTime GetnextSunday()
		{
			DateTime date = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
			for (int count = 1; count <= 3; count++)
			{
				date = date.AddDays(1);
				if (date.DayOfWeek.ToString() == "Sunday")
					break;
			}


			return date;
		}


		public static int AfterMidnightLandingTime(int landTime)
		{
			if (landTime < GlobalSettings.LastLandingMinus1440)
				return landTime += 1440;
			else
				return landTime % 1440;
		}

		public static int DomicileTimeFromHerb(string domicile, DateTime date, int herb)
		{
			bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(date);

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
					return herb + 60;       // EST = herb + 60
				case "HOU":
					return herb;            // CST = herb 
				case "LAS":
					return herb - 120;      // PST = herb - 120
				case "MCO":
					return herb + 60;       // EST = herb + 60
				case "MDW":
					return herb;            // CST = herb 
                case "LAX":
                    return herb - 120;      // PST = herb - 120
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
		public static ObservableCollection<BindingManager> GenerateTimeList()
		{
			var timeList = new ObservableCollection<BindingManager>();
			int start = 5;
			int end = 16;
			int intrvl = 5;

			for (int count = start; count < end; count++)
			{
				for (int intervel = 0; intervel < 60; intervel = intervel + intrvl)
				{
					timeList.Add(new BindingManager() { Name = count.ToString().PadLeft(2, '0') + ":" + intervel.ToString().PadLeft(2, '0'), Id = count * 60 + intervel });


				}

			}
			timeList.Add(new BindingManager() { Name = end.ToString() + ":00", Id = end * 60 });

			return timeList;

		}


		public static ObservableCollection<BindingManager> GenerateGroundTimeList()
		{
			var grndTimeList = new ObservableCollection<BindingManager>();
			const int start = 0;
			const int end = 6;
			const int intrvl = 5;

			for (int count = start; count < end; count++)
			{
				for (int intervel = 0; intervel < 60; intervel = intervel + intrvl)
				{
					grndTimeList.Add(new BindingManager { Name = count.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0') + ":" + intervel.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), Id = count * 60 + intervel });

				}

			}
			grndTimeList.Add(new BindingManager { Name = end.ToString().PadLeft(2, '0') + ":00", Id = end * 60 });
			return grndTimeList;

		}

		//
		public static void GenerateSplitPointCities()
		{
			//"Look in SwaLife=>MyWork=>Flt Ops => Our Business => Contract"
			GlobalSettings.SplitPointCities = GlobalSettings.WBidINIContent.SplitPointCities;
			if (GlobalSettings.SplitPointCities.Count() <= 0)
			{
				GlobalSettings.SplitPointCities = new SplitPointCities()
			{
 new SplitPointCity {Domicile = "ATL", Cities = new List<string> {"ATL","BNA","CMH","ECP","GSP","IND","JAX","LIT","MCO","MEM", "MSY","PNS","RDU","RIC","SDF", "SRQ","STL","TPA"} },
				new SplitPointCity {Domicile = "AUS", Cities = new List<string>{"AMA", "CRP", "DAL", "HOU", "HRL", "JAN", "LBB", "LIT", "MAF", "MSY", "OKC", "SAT", "TUL" }},
				new SplitPointCity {Domicile = "BWI", Cities = new List<string>{"ALB","BDL","BOS","BUF","BWI","CHS","CLE","CLT","CMH","CVG","DTW","GSP","ISP","MHT","MYR","ORF","PIT","PVD","PWM","RDU","ROC","SDF"}},
				new SplitPointCity {Domicile = "DAL", Cities = new List<string>{"AMA","AUS","DAL","HOU","IAH","LBB","LIT","MAF","MCI","MEM","MSY","SAT","TUL"}},
				new SplitPointCity {Domicile = "DEN", Cities = new List<string>{"ABQ","COS","DEN","HDN","ICT","MTJ","OKC","OMA","SLC"}},
				new SplitPointCity {Domicile = "FLL", Cities = new List<string>{"ECP","JAX","MCO","RSW","TPA"}},
				new SplitPointCity {Domicile = "HOU", Cities = new List<string>{"AUS","CRP","DAL","HOU","HRL","MAF","MEM","MSY","OKC","PNS","SAT","TUL"}},
				new SplitPointCity {Domicile = "LAS", Cities = new List<string>{"ABQ","BUR","FAT","LAS","LAX","LGB","OAK","ONT","PHX","PSP","RNO","SAN","SBA","SFO","SJC","SLC","SMF","SNA","TUS"}},
				new SplitPointCity {Domicile = "LAX", Cities = new List<string>{"LAS","LAX", "OAK", "PHX", "RNO", "SFO", "SJC", "SMF"}},
				new SplitPointCity {Domicile = "MCO", Cities = new List<string>{"ATL","BHM","FLL","MCO","RSW"}},
				new SplitPointCity {Domicile = "MDW", Cities = new List<string>{"BNA","BUF","CLE","CMH","CVG","DTW","GRR","MCI","MDW","MEM","MSP","OMA","PIT","SDF","STL"}},
				new SplitPointCity {Domicile = "OAK", Cities = new List<string>{"BUR","LAS","LAX","LGB","OAK","ONT","PSP","RNO","SAN","SBA","SNA"}},
				new SplitPointCity {Domicile = "PHX", Cities = new List<string>{"ABQ","BUR","ELP","LAS","LAX","LGB","ONT","PHX","PSP","SAN","SNA"}}

			};
				GlobalSettings.WBidINIContent.SplitPointCities = GlobalSettings.SplitPointCities;
			}
		}

		public static List<Absense> GenarateOrderedMILDates(List<Absense> milList)
		{
			List<Absense> absence = new List<Absense>();
			if (milList.Count > 0)
			{
				absence.Add(new Absense
				{
					StartAbsenceDate = milList.FirstOrDefault().StartAbsenceDate,
					EndAbsenceDate = milList.FirstOrDefault().EndAbsenceDate,
					AbsenceType = "VA"
				});

				for (int count = 0; count < milList.Count - 1; count++)
				{
					if ((milList[count + 1].StartAbsenceDate - milList[count].EndAbsenceDate).Days == 1)
					{
						absence[absence.Count - 1].EndAbsenceDate = milList[count + 1].EndAbsenceDate;
					}
					else
					{
						absence.Add(new Absense
						{
							StartAbsenceDate = milList[count + 1].StartAbsenceDate,
							EndAbsenceDate = milList[count + 1].EndAbsenceDate,
							AbsenceType = "VA"
						});
					}
				}
			}
			return absence;
		}
		public static List<DaysOfMonth> GetDaysOfMonthList()
		{
			List<DaysOfMonth> lstDaysOfMonth = new List<DaysOfMonth>();
			DateTime calendarDate;
			DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;

			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Month == 3)
			{
				startDate = startDate.AddDays(-1);
			}

			int iterationCount = (int)startDate.DayOfWeek;

			int id = 1;
			calendarDate = startDate;
			for (int count = 0; count < iterationCount; count++)
			{
				lstDaysOfMonth.Add(new DaysOfMonth() { Id = id, IsEnabled = false, Day = null, Date = calendarDate, Status = null });
				calendarDate = calendarDate.AddDays(1);
				id++;
			}

			calendarDate = startDate;
			bool status = false;
			while (calendarDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
			{

				status = calendarDate >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
				lstDaysOfMonth.Add(new DaysOfMonth() { Id = id, IsEnabled = status, Day = calendarDate.Day.ToString(), Date = calendarDate, Status = null });
				calendarDate = calendarDate.AddDays(1);
				id++;

			}

			while (calendarDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3))
			{

				lstDaysOfMonth.Add(new DaysOfMonth() { Id = id, IsEnabled = true, Day = calendarDate.Day.ToString(), Date = calendarDate, Status = null });
				calendarDate = calendarDate.AddDays(1);
				id++;

			}


			for (int count = id; count <= 42; count++)
			{
				lstDaysOfMonth.Add(new DaysOfMonth() { Id = id, IsEnabled = false, Day = calendarDate.Day.ToString(), Date = calendarDate, Status = null });
				calendarDate = calendarDate.AddDays(1);
				id++;

			}

			return lstDaysOfMonth;
		}

		public static T ConvertJSonStringToObject<T>(string jsonString)
		{
			var jsonSerializerSettings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.Auto,

			};

			T obj = JsonConvert.DeserializeObject<T>(jsonString, jsonSerializerSettings);
			return obj;
		}
		/// <summary>
		///  get the name of the flt equipment. flt string contains the value parsed from the trip records (parse 6 record)
		/// </summary>
		/// <param name="fltstring"></param>
		/// <returns></returns>
		public static string GetEquipmentName(string fltstring)
		{
			string equip = string.Empty;

			if (GlobalSettings.CurrentBidDetails.Postion == "FA")
			{
				equip = fltstring.Substring(0, 3);
			}
			else
			{
				if (fltstring.Substring(1, 1) == "6")
					equip = "8Max";
				else if (fltstring.Substring(1, 1) == "2")
					equip = "7Max";
				else
					equip = fltstring.Substring(1, 1) + "00";
				//equip = (fltstring.Substring(1, 1) == "6") ? "MAX" : fltstring.Substring(1, 1) + "00";
			}
			return equip;
		}

		
	}
}
