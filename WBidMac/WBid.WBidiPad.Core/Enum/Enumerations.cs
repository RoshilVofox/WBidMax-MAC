using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Core.Enum
{

    public enum ConstraintType
    {
        LessThan = 1,
        EqualTo,
        MoreThan,
        NotEqualTo,
        atafter,
        atbefore
    }
    public enum AMPMType
    {
        AM = 1,
        PM,
        MIX,
        NTE
    }
    public enum WeightType
    {
        Less,
        Equal,
        More,
        NotEqual,
    }
    public enum DeadheadType
    {
        First = 1,
        Last,
        Both,
        Either
    }
    public enum FAPositon
    {
        A = 1,
        B,
        C,
        D
    }

    public enum RestType
    {
        All = 1,
        InDomicile,
        AwayDomicile
    }

    public enum CityType
    {
        International = 1,
        NonConus
    }

    public enum RestOptions
    {
        Shorter = 1,
        Longer,
        Both
    }

    public enum DutyPeriodType
    {
        Relative = 1,
        Longer,
        Shorter
    }

    public enum LegsPerPairingType
    {
        Less = 1,
        More,
        All
    }

    public enum ThreeOnThreeOff
    {
        ThreeOnThreeOff = 1,
        NoThreeOnThreeOff,

    }
    public enum BidLineType
    {
        NormalTrip = 0,
        NoTrip,
        VA,
        VAP,
        VO,
        VD,
        VOFSplit,
        VOBSplit,
        VDDrop,
        VOFSplitDrop,
        VOBSplitDrop,
        FV,
        CFV

    }
    public enum ModernBidlineBorderType
    {
        Noborder,
        singleLineBorder
    }
//    public enum CalenderColortype
//    {
//        VA=0,
//        VO,
//        VD,
//        VOB,
//        VOF,
//        Transparaent
//    }
	public enum CalenderColortype
	{
		VA=0,
		VO,
		VD,
		VOB,
		VOF,
		Transparaent,
		MILVO_No_Work,
		MILBackSplitWork,
		MILFrontSplitWork,
		MILBackSplitWithoutStrike,
		MILFrontSplitWithoutStrike,
        FV
	}

	public enum DayofTheWeek
	{
		Monday = 0,
		Tuesday,
		Wednesday,
		Thursday,
		Friday,
		Saturday,
		Sunday
	}
	public enum CommutabilitySecondCell
	{
		NoMiddle=1,
		OKMiddle
	}
	public enum CommutabilityThirdCell
	{
		Front=1,
		Back,
		Overall
	}
	public enum CommutabilityEnum
	{
		CommutabilityConstraint=1,CommutabilityWeight,CommutabilitySort
	}
	public enum StartDayOfWeekType
	{
		Blk = 1,
		Trip
	}
    public enum CommutableAutoFrom
    {
        Constraints = 1,
        Weight, Filters, CommutabilityConstraint, CommutabilityWeight,Sort
    }
    public enum StartDayType
    {
        Block = 1,
        Trip

    }
    public enum ReportReleaseDayType
    {
        StartDay = 1,
        NormalDay,
        LastDay,
        OneDayTrip

    }
    public enum StartDay
    {
        StartOn = 1,
        DoesnotStart

    }
    public enum FromApp
    {
        Wbidmax = 1,
        Crewbid,
        Wbidvalet,
        WbidmaxIpad,
        CrewbidApp,
        WbidmaxMACApp,
        WbidmaxWebApp

    }
    public enum RequestTypes
    {
        DownnloadBid = 0,
        DownloadAward,
        SubmitBid,
        ScrapMissingTrip,
        FromReparse,
        DownnloadHostoricalBid,
        Other

    }

}
