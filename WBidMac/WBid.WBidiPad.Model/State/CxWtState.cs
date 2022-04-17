using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class CxWtState
    {
        public  CxWtState()
        {
        }
        public CxWtState(CxWtState cxWtState)
        {
            ACChg = new StateStatus(cxWtState.ACChg);
            AMPM = new StateStatus(cxWtState.AMPM);
            BDO = new StateStatus(cxWtState.BDO);
            DHD = new StateStatus(cxWtState.DHD);
            CL = new StateStatus(cxWtState.CL);
            if(cxWtState.CLAuto==null)
                cxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
			CLAuto = new StateStatus (cxWtState.CLAuto);
            SDO = new StateStatus(cxWtState.SDO);
            DOW = new StateStatus(cxWtState.DOW);
            DP = new StateStatus(cxWtState.DP);
            EQUIP = new StateStatus(cxWtState.EQUIP);
            if (cxWtState.ETOPS == null)
                cxWtState.ETOPS = new StateStatus { Wt = false, Cx = false };
            if (cxWtState.ETOPSRes == null)
                cxWtState.ETOPSRes = new StateStatus { Wt = false, Cx = false };
            FLTMIN = new StateStatus(cxWtState.FLTMIN);
            GRD = new StateStatus(cxWtState.GRD);
            InterConus = new StateStatus(cxWtState.InterConus);
            LEGS = new StateStatus(cxWtState.LEGS);
            LegsPerPairing = new StateStatus(cxWtState.LegsPerPairing);
            NODO = new StateStatus(cxWtState.NODO);
            RON = new StateStatus(cxWtState.RON);
            WtPDOFS = new StateStatus(cxWtState.WtPDOFS);
            SDOW = new StateStatus(cxWtState.SDOW);
            Rest = new StateStatus(cxWtState.Rest);
            PerDiem = new StateStatus(cxWtState.PerDiem);
            TL = new StateStatus(cxWtState.TL);
            WB = new StateStatus(cxWtState.WB);
            MP = new StateStatus(cxWtState.MP);
            NOL = new StateStatus(cxWtState.NOL);
            No3on3off = new StateStatus(cxWtState.No3on3off);
            LrgBlkDaysOff = new StateStatus(cxWtState.LrgBlkDaysOff);
            Position = new StateStatus(cxWtState.Position);
            WorkDay = new StateStatus(cxWtState.WorkDay);
            PDAfter = new StateStatus(cxWtState.PDAfter);
            PDBefore = new StateStatus(cxWtState.PDBefore);
            NormalizeDays = new StateStatus(cxWtState.NormalizeDays);
            DHDFoL = new StateStatus(cxWtState.DHDFoL);
            AMPMMIX = new AMPMConstriants(cxWtState.AMPMMIX);
            FaPosition = new PostionConstraint(cxWtState.FaPosition);
            TripLength = new TripLengthConstraints(cxWtState.TripLength);
            DaysOfWeek = new DaysOfWeekConstraints(cxWtState.DaysOfWeek);
            BulkOC = new StateStatus(cxWtState.BulkOC);
			CitiesLegs = new StateStatus (cxWtState.CitiesLegs);
			if (cxWtState.Commute == null)
				cxWtState.Commute = new StateStatus{ Wt = false, Cx = false };
			Commute=new StateStatus(cxWtState.Commute);
            StartDay = new StateStatus(cxWtState.StartDay);
            ReportRelease = new StateStatus(cxWtState.ReportRelease);
            if (cxWtState.MixedHardReserveTrip == null)
                cxWtState.MixedHardReserveTrip = new StateStatus { Cx = false, Wt = false };
            MixedHardReserveTrip= new StateStatus(cxWtState.MixedHardReserveTrip);

            ETOPS = new StateStatus(cxWtState.ETOPS);
            ETOPSRes = new StateStatus(cxWtState.ETOPSRes);


        }
        [XmlElement("ACChg")]
        public StateStatus ACChg;

        [XmlElement("AM-PM")]
        public StateStatus AMPM;

        [XmlElement("BDO")]
        public StateStatus BDO;

        [XmlElement("DHD")]
        public StateStatus DHD;

        [XmlElement("CL")]
        public StateStatus CL;

		[XmlElement("CLAuto")]
		public StateStatus CLAuto;

		[XmlElement("Commute")]
		public StateStatus Commute;

        [XmlElement("SDO")]
        public StateStatus SDO;


        [XmlElement("DOW")]
        public StateStatus DOW;

        [XmlElement("DP")]
        public StateStatus DP;

        [XmlElement("EQUIP")]
        public StateStatus EQUIP;

        [XmlElement("ETOPS")]
        public StateStatus ETOPS;

        [XmlElement("ETOPSRes")]
        public StateStatus ETOPSRes;

        [XmlElement("FLTMIN")]
        public StateStatus FLTMIN;


        [XmlElement("GRD")]
        public StateStatus GRD;

        [XmlElement("InterConus")]
        public StateStatus InterConus;

        [XmlElement("LEGS")]
        public StateStatus LEGS;

        [XmlElement("LegsPerPairing")]
        public StateStatus LegsPerPairing;

        [XmlElement("NODO")]
        public StateStatus NODO;

        [XmlElement("RON")]  // used for overnight cities  RON=remain overnight
        public StateStatus RON;

        [XmlElement("WtPDOFS")]
        public StateStatus WtPDOFS;

        [XmlElement("SDOW")]
        public StateStatus SDOW;

        [XmlElement("Rest")]
        public StateStatus Rest;

        [XmlElement("PerDiem")]
        public StateStatus PerDiem;

        [XmlElement("TL")]
        public StateStatus TL;

        [XmlElement("WB")]
        public StateStatus WB;

        /// <summary>
        /// Minimum pay
        /// </summary>
        [XmlElement("MP")]
        public StateStatus MP;

        /// <summary>
        /// NO Overlap
        /// </summary>
        [XmlElement("NOL")]
        public StateStatus NOL;
        /// <summary>
        /// No 3 on 3 off
        /// </summary>
        [XmlElement("No3on3off")]
        public StateStatus No3on3off;

        /// <summary>
        /// Largest block days Off
        /// </summary>
        [XmlElement("LrgBlkDaysOff")]
        public StateStatus LrgBlkDaysOff;


        /// <summary>
        /// Position
        /// </summary>
        [XmlElement("Position")]
        public StateStatus Position;

        [XmlElement("WorkDay")]
        public StateStatus WorkDay;

       
        [XmlElement("PDAfter")]
        public StateStatus PDAfter;

        [XmlElement("PDBefore")]
        public StateStatus PDBefore;


        [XmlElement("NormalizeDays")]
        public StateStatus NormalizeDays;



        [XmlElement("DHDFoL")]
        public StateStatus DHDFoL;

        [XmlElement("AMPM")]
        public AMPMConstriants AMPMMIX;

        [XmlElement("POS")]
        public PostionConstraint FaPosition;

        [XmlElement("TrpLen")]
        public TripLengthConstraints TripLength;

        [XmlElement("WeekDays")]
        public DaysOfWeekConstraints  DaysOfWeek;

        [XmlElement("BulkOC")]
        public StateStatus BulkOC;

		[XmlElement("CitiesLegs")]
		public StateStatus CitiesLegs;


        [XmlElement("StartDay")]
        public StateStatus StartDay;

        [XmlElement("ReportRelease")]
        public StateStatus ReportRelease;

        [XmlElement("MixedHardReserveTrip")]
        public StateStatus MixedHardReserveTrip;


    }
     [Serializable]
    public class StateStatus
    {
         public StateStatus()
         {
         }
         public StateStatus(StateStatus stateStatus)
         {
            if (stateStatus == null)
            {
                Wt = false;
                Cx = false;
            }
            else
            {
                Wt = stateStatus.Wt;
                Cx = stateStatus.Cx;
            }
         }
        [XmlAttribute("Wt")]
        public bool Wt { get; set; }

        [XmlAttribute("Cx")]
        public bool Cx { get; set; }
 
    }
}
