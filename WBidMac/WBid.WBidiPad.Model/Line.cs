#region NameSpace
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

#endregion
namespace WBid.WBidiPad.Model
{
	[ProtoContract]
	public class Line
	{

		#region Constructor
		public Line()
		{


		}

		#endregion


		[ProtoMember(1)]
		public bool Lock { get; set; }


		[ProtoMember(2)]
		public bool Constrained { get; set; }

		[ProtoMember(3)]
		public bool ShowOverLap { get; set; }

		[ProtoMember(4)]
		public string LineDisplay { get; set; }

		[ProtoMember(5)]
		public decimal TfpPerDay { get; set; }


		[ProtoMember(6)]
		public decimal TfpPerDhr { get; set; }


		[ProtoMember(7)]
		public decimal TfpPerFltHr { get; set; }



		[ProtoMember(8)]
		public decimal TfpPerTafb { get; set; }



		[ProtoMember(9)]
		public TimeSpan LongestGrndTime { get; set; }



		[ProtoMember(10)]
		public int MostLegs { get; set; }




		[ProtoMember(11)]
		public int LargestBlkOfDaysOff { get; set; }



		[ProtoMember(12)]
		public int Trips1Day { get; set; }




		[ProtoMember(13)]
		public int Trips2Day { get; set; }



		[ProtoMember(14)]
		public int Trips3Day { get; set; }



		[ProtoMember(15)]
		public int Trips4Day { get; set; }


		[ProtoMember(16)]
		public string Equip8753 { get; set; }



		private string _aMPM = string.Empty;
		[ProtoMember(17)]
		public string AMPM
		{
			get
			{
				return _aMPM;
			}
			set
			{
				_aMPM = value;



			}
		}



		[ProtoMember(18)]
		public decimal AcftChgDay { get; set; }



		[ProtoMember(19)]
		public decimal CarryOverTfp { get; set; }



		[ProtoMember(20)]
		public int TotDutyPds { get; set; }


		private List<string> faPositions = new List<string>();
		[ProtoMember(21)]
		public List<string> FAPositions
		{
			get { return faPositions; }
			set { faPositions = value; }
		}

		[ProtoMember(22)]
		public int LineNum { get; set; }


		[ProtoMember(23)]
		public decimal Tfp { get; set; }

		[ProtoMember(24)]
		public int Block { get; set; }

		private List<string> pairings = new List<string>();
		[ProtoMember(25)]
		public List<string> Pairings
		{
			get { return pairings; }
			set { pairings = value; }
		}
		[ProtoMember(26)]
		public bool ReserveLine { get; set; }

		[ProtoMember(27)]
		public bool BlankLine { get; set; }


		private List<BidLineTemplate> _bidLineTemplates;
		[ProtoMember(29)]
		public List<BidLineTemplate> BidLineTemplates
		{
			get { return _bidLineTemplates; }
			set { _bidLineTemplates = value; }
		}

		private List<LineSip> lineSips = new List<LineSip>();
		[ProtoMember(30)]
		public List<LineSip> LineSips
		{
			get { return lineSips; }
			set { lineSips = value; }
		}

		[ProtoMember(31)]
		public string Pairingdesription { get; set; }

		[ProtoMember(32)]
		public decimal TfpInLine { get; set; }

		[ProtoMember(33)]
		public string BlkHrsInBp { get; set; }

		[ProtoMember(34)]
		public string BlkHrsInLine { get; set; }

		[ProtoMember(35)]
		public string Weekend { get; set; }

		[ProtoMember(36)]
		public int DaysWork { get; set; }


		[ProtoMember(37)]
		public int DaysOff { get; set; }


		[ProtoMember(38)]
		public int DaysWorkInLine { get; set; }

		[ProtoMember(39)]
		public string T234 { get; set; }



		[ProtoMember(40)]
		public string TafbInBp { get; set; }

		[ProtoMember(41)]
		public string TafbInLine { get; set; }
		[ProtoMember(42)]
		public int Sips { get; set; }

		//  [ProtoMember(43)]
		//  public Dictionary<string, string> FASecondRoundPositions { get; set; }


		[ProtoMember(44)]
		public string DutyHrsInBp { get; set; }

		[ProtoMember(45)]
		public int LegsIn800 { get; set; }

		[ProtoMember(46)]
		public int LegsIn700 { get; set; }

		[ProtoMember(47)]
		public int LegsIn500 { get; set; }

		[ProtoMember(48)]
		public int LegsIn300 { get; set; }

		[ProtoMember(49)]
		public int AcftChanges { get; set; }

		[ProtoMember(50)]
		public int TotDutyPdsInBp { get; set; }

		[ProtoMember(51)]
		public string EDomPush { get; set; }



		[ProtoMember(52)]
		public string EPush { get; set; }



		[ProtoMember(53)]
		public TimeSpan LastArrTime { get; set; }


		[ProtoMember(54)]
		public TimeSpan LastDomArrTime { get; set; }



		[ProtoMember(55)]
		public decimal LegsPerDay { get; set; }


		[ProtoMember(56)]
		public decimal LegsPerPair { get; set; }


		[ProtoMember(57)]
		public string DutyHrsInLine { get; set; }


		[ProtoMember(58)]
		public int TotPairings { get; set; }



		[ProtoMember(59)]
		public string StartDow { get; set; }

		[ProtoMember(60)]
		public List<int> BlkOfDaysOff { get; set; }

		/// <summary>
		/// PURPOSE :Legs Per Duty Period
		/// </summary>
		/// 
		[ProtoMember(61)]
		public List<int> LegsPerDutyPeriod { get; set; }

		/// <summary>
		/// PURPOSE :Days Of Week Work
		/// </summary>
		/// 

		[ProtoMember(62)]
		public List<int> DaysOfWeekWork { get; set; }

		/// <summary>
		/// PURPOSE:holds the workings days of the month
		/// </summary>
		private List<WorkDaysOfBidLine> daysOfMonthWork = new List<WorkDaysOfBidLine>();
		[ProtoMember(63)]
		public List<WorkDaysOfBidLine> DaysOfMonthWorks
		{
			get { return daysOfMonthWork; }
			set { daysOfMonthWork = value; }
		}
		/// <summary>
		/// PURPOSE :Deadhead Cities
		/// </summary>
		private List<DeadheadCity> _cmtDhds = new List<DeadheadCity>();
		[ProtoMember(64)]
		public List<DeadheadCity> CmtDhds
		{
			get
			{
				return _cmtDhds;
			}
			set
			{
				_cmtDhds = value;

			}
		}
		private List<Day> _daysInBidPeriod;
		[ProtoMember(65)]
		public List<Day> DaysInBidPeriod            // used for Partial Days Off PDO
		{
			get { return _daysInBidPeriod; }
			set { _daysInBidPeriod = value; }
		}
		/// <summary>
		/// Hold Duty hours for each duty period.
		/// </summary>
		[ProtoMember(66)]
		public List<int> DutyPeriodHours { get; set; }
		/// <summary>
		/// Set if line satisfied the strict pattern of 3 days of work followed by 3 days of OFF
		/// </summary>
		[ProtoMember(67)]
		public bool Is3on3Off { get; set; }

		private int _legs = 0;
		[ProtoMember(68)]
		public int Legs
		{
			get
			{
				return _legs;
			}
			set
			{
				_legs = value;

			}
		}

		private List<string> _overnightCities = new List<string>();
		[ProtoMember(69)]
		public List<string> OvernightCities
		{
			get { return _overnightCities; }
			set { _overnightCities = value; }
		}


		private List<RestPeriod> _restPeriods = new List<RestPeriod>();
		[ProtoMember(70)]
		public List<RestPeriod> RestPeriods
		{
			get { return _restPeriods; }
			set { _restPeriods = value; }
		}

		[ProtoMember(71)]
		public List<int> WorkBlockLengths { get; set; }


		private List<WorkBlockDetails> _workBlockList;
		[ProtoMember(72)]
		public List<WorkBlockDetails> WorkBlockList            // used for Partial Days Off PDO
		{
			get { return _workBlockList; }
			set { _workBlockList = value; }
		}
		[ProtoMember(73)]
		public decimal CarryOverBlock { get; set; }
		[ProtoMember(74)]
		public int LastArrivalTime { get; set; }
		[ProtoMember(75)]
		public int DhFirstTotal { get; set; }
		[ProtoMember(76)]
		public int DhLastTotal { get; set; }
		/// <summary>
		/// Holds all the turn time for the line.
		/// </summary>
		[ProtoMember(77)]
		public List<int> GroundTimes { get; set; }
		[ProtoMember(78)]
		public string TempBlkHrsInBp { get; set; }
		[ProtoMember(79)]
		public string TempBlkHrsInLine { get; set; }
		[ProtoMember(80)]
		public int TempDaysOff { get; set; }
		[ProtoMember(81)]
		public int TempDaysWorkInLine { get; set; }
		[ProtoMember(82)]
		public int TempLegs { get; set; }
		[ProtoMember(83)]
		public string TempTafbInBp { get; set; }
		[ProtoMember(84)]
		public decimal TempTfp { get; set; }
		[ProtoMember(85)]
		public decimal TempTfpInLine { get; set; }

		#region Rig in Bp

		private decimal? _rigAdgInBp { get; set; }
		[ProtoMember(86)]
		public decimal? RigAdgInBp
		{
			get
			{
				return _rigAdgInBp;

			}
			set
			{
				_rigAdgInBp = value;

			}
		}

		private decimal? _rigTafbInBp { get; set; }
		[ProtoMember(87)]
		public decimal? RigTafbInBp
		{
			get
			{
				return _rigTafbInBp;

			}

			set
			{
				_rigTafbInBp = value;

			}
		}

		private decimal? _rigDailyMinInBp { get; set; }
		[ProtoMember(88)]
		public decimal? RigDailyMinInBp
		{
			get
			{
				return _rigDailyMinInBp;

			}
			set
			{
				_rigDailyMinInBp = value;

			}
		}

		private decimal? _rigDhrInBp { get; set; }
		[ProtoMember(89)]
		public decimal? RigDhrInBp
		{
			get
			{
				return _rigDhrInBp;

			}
			set
			{
				_rigDhrInBp = value;

			}
		}

		private decimal? _rigFltInBP { get; set; }
		[ProtoMember(90)]
		public decimal? RigFltInBP
		{
			get
			{
				return _rigFltInBP;

			}
			set
			{
				_rigFltInBP = value;

			}
		}

		private decimal? _rigTotalInBp { get; set; }
		[ProtoMember(91)]
		public decimal? RigTotalInBp
		{
			get
			{
				return _rigTotalInBp;

			}
			set
			{
				_rigTotalInBp = value;

			}
		}




		#endregion

		#region Rig in Line

		private decimal? _rigAdgInLine { get; set; }
		[ProtoMember(92)]
		public decimal? RigAdgInLine
		{
			get
			{
				return _rigAdgInLine;

			}
			set
			{
				_rigAdgInLine = value;

			}
		}

		private decimal? _rigTafbInLine { get; set; }
		[ProtoMember(93)]
		public decimal? RigTafbInLine
		{
			get
			{
				return _rigTafbInLine;

			}
			set
			{
				_rigTafbInLine = value;

			}
		}

		private decimal? _rigDailyMinInLine { get; set; }
		[ProtoMember(94)]
		public decimal? RigDailyMinInLine
		{
			get
			{
				return _rigDailyMinInLine;

			}
			set
			{
				_rigDailyMinInLine = value;

			}
		}

		private decimal? _rigDhrInLine { get; set; }
		[ProtoMember(95)]
		public decimal? RigDhrInLine
		{
			get
			{
				return _rigDhrInLine;

			}
			set
			{
				_rigDhrInLine = value;

			}
		}

		private decimal? _rigFltInLine { get; set; }
		[ProtoMember(96)]
		public decimal? RigFltInLine
		{
			get
			{
				return _rigFltInLine;

			}
			set
			{
				_rigFltInLine = value;

			}
		}

		private decimal? _rigTotalInLine { get; set; }
		[ProtoMember(97)]
		public decimal? RigTotalInLine
		{
			get
			{
				return _rigTotalInLine;

			}
			set
			{
				_rigTotalInLine = value;

			}
		}

		[ProtoMember(98)]
		private decimal _points { get; set; }
		public decimal Points
		{
			get
			{
				return _points;
				;
			}
			set
			{
				_points = value;
			}
		}

		#endregion

		[ProtoMember(99)]
		public bool TopLock { get; set; }
		[ProtoMember(100)]
		public bool BotLock { get; set; }
		[ProtoMember(101)]
		public bool International { get; set; }
		[ProtoMember(102)]
		public bool NonConus { get; set; }

		[ProtoMember(103)]
		public ConstraintPoints ConstraintPoints { get; set; }

		[ProtoMember(104)]
		public int StartDowOrder { get; set; }

		[ProtoMember(105)]
		public decimal TotWeight { get; set; }

		[ProtoMember(106)]
		public List<int> StartDaysList { get; set; }

		[ProtoMember(107)]
		public WeightPoints WeightPoints { get; set; }

		[ProtoMember(108)]
		public Dictionary<string, string> FASecondRoundPositions = new Dictionary<string, string>();

		[ProtoMember(109)]
		public int AMPMSortOrder { get; set; }

		[ProtoMember(110)]
		public VacationStateLine VacationStateLine { get; set; }

		[ProtoMember(111)]
		public string Tag { get; set; }

		[ProtoMember(112)]
		public decimal FlyPay { get; set; }

		[ProtoMember(113)]
		public decimal VacPay { get; set; }

		[ProtoMember(114)]
		public decimal VacationDrop { get; set; }

		[ProtoMember(115)]
		public decimal VacationOverlapFront { get; set; }

		[ProtoMember(116)]
		public decimal VacationOverlapBack { get; set; }

		[ProtoMember(117)]
		public decimal OverlapDrop { get; set; }

		[ProtoMember(118)]
		public decimal LineRig { get; set; }

		[ProtoMember(119)]
		public TimeSpan AvgLatestDomArrivalTime { get; set; }

		[ProtoMember(120)]
		public TimeSpan AvgEarliestDomPush { get; set; }

		private decimal _holRig;
		[ProtoMember(121)]
		public decimal HolRig
		{
			get
			{
				return _holRig;
			}
			set
			{
				_holRig = value;

			}
		}

		private string _bAGroup;
		[ProtoMember(122)]
		public string BAGroup
		{
			get
			{
				return _bAGroup;
			}
			set
			{
				_bAGroup = value;

			}
		}
		//0- Normal,1-Gray
		private int _isGrpColorOn;
		[ProtoMember(123)]
		public int IsGrpColorOn
		{
			get
			{
				return _isGrpColorOn;
			}
			set
			{
				_isGrpColorOn = value;

			}
		}
		private List<bool> _bAFilters = new List<bool>();
		[ProtoMember(124)]
		public List<bool> BAFilters
		{
			get
			{
				return _bAFilters;
			}
			set
			{
				_bAFilters = value;
			}
		}

		private decimal _nightsInMid;
		[ProtoMember(125)]
		public decimal NightsInMid
		{
			get
			{
				return _nightsInMid;
			}
			set
			{
				_nightsInMid = value;

			}
		}
		private decimal _totalCommutes;
		[ProtoMember(126)]
		public decimal TotalCommutes
		{
			get
			{
				return _totalCommutes;
			}
			set
			{
				_totalCommutes = value;

			}
		}
		private decimal _commutableFronts;
		public decimal commutableFronts
		{
			get
			{
				return _commutableFronts;
			}
			set
			{
				_commutableFronts = value;

			}
		}
		private decimal _commutableBacks;
		public decimal CommutableBacks
		{
			get
			{
				return _commutableBacks;
			}
			set
			{
				_commutableBacks = value;

			}
		}
		private decimal _commutabilityFront;
		public decimal CommutabilityFront
		{
			get
			{
				return _commutabilityFront;
			}
			set
			{
				_commutabilityFront = value;

			}
		}
		private decimal _commutabilityBack;
		public decimal CommutabilityBack
		{
			get
			{
				return _commutabilityBack;
			}
			set
			{
				_commutabilityBack = value;

			}
		}
		private decimal _commutabilityOverall;
		public decimal CommutabilityOverall
		{
			get
			{
				return _commutabilityOverall;
			}
			set
			{
				_commutabilityOverall = value;

			}
		}
		[ProtoMember(127)]
		public List<int> StartDaysListPerTrip { get; set; }

		[ProtoMember(128)]
		public decimal Ratio { get; set; }

		[ProtoMember(129)]
		public int LegsIn600 { get; set; }

		[ProtoMember(130)]
		public int ManualScroll { get; set; }

        [ProtoMember(131)]
        public decimal VacPayCuBp { get; set; }
        [ProtoMember(132)]
        public decimal VacPayNeBp { get; set; }
        [ProtoMember(133)]
        public decimal VacPayBothBp { get; set; }
        [ProtoMember(134)]
        public bool ETOPS { get; set; }

        [ProtoMember(135)]
        public List<FVvacationLineData> FVvacationData { get; set; }

        [ProtoMember(136)]
        public decimal VacPlusRig { get; set; }

		[ProtoMember(137)]
		public decimal CFVPay { get; set; }


		[ProtoMember(138)]
		public List<string> CFVDates { get; set; }


		[ProtoMember(139)]
		public int LegsIn200 { get; set; }
		[ProtoMember(140)]
		public decimal VAbp { get; set; }
		[ProtoMember(141)]
		public decimal VAne { get; set; }
		[ProtoMember(142)]
		public decimal VAbo { get; set; }
		[ProtoMember(143)]
		public decimal VAPbp { get; set; }
		[ProtoMember(144)]
		public decimal VAPne { get; set; }
		[ProtoMember(145)]
		public decimal VAPbo { get; set; }
		[ProtoMember(146)]
		public int ETOPSTripsCount { get; set; }
		[ProtoMember(147)]
		public int DhTotal { get; set; }

	}
}
