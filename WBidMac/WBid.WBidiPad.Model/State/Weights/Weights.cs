#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Model.State.Weights;
#endregion

namespace WBid.WBidiPad.Model
{
    
    public class Weights
    {


        public Weights()
        {
                
        }


        public Weights(Weights weights)
        {
            AirCraftChanges = new Wt3Parameter(weights.AirCraftChanges);
            AM_PM = new Wt2Parameters(weights.AM_PM);
            BDO = new Wt3Parameters(weights.BDO);
            LrgBlkDayOff = new Wt2Parameter(weights.LrgBlkDayOff);
            DHD = new Wt3Parameters(weights.DHD);
            CL = new WtCommutableLine(weights.CL);
			CLAuto = new WtCommutableLineAuto(weights.CLAuto);
			Commute=new Commutability(weights.Commute);
            SDO = new DaysOfMonthWt(weights.SDO);
            DOW = new WtDaysOfWeek(weights.DOW);
            DP = new Wt3Parameters(weights.DP);
            EQUIP = new Wt3Parameters(weights.EQUIP);
            ETOPS = new Wt1Parameters(weights.ETOPS);
            ETOPSRes = new Wt1Parameters(weights.ETOPSRes);
            FLTMIN = new Wt3Parameters(weights.FLTMIN);
            GRD = new Wt3Parameters(weights.GRD);
            InterConus = new Wt2Parameters(weights.InterConus);
            LEGS = new Wt3Parameters(weights.LEGS);
            WtLegsPerPairing = new Wt3Parameters(weights.WtLegsPerPairing);
            NODO = new Wt2Parameters(weights.NODO);
            RON = new Wt2Parameters(weights.RON);
			CitiesLegs = new Wt2Parameters(weights.CitiesLegs);
            //WtPDOFS = new WtPDOFS(weights.WtPDOFS);
            WtRest = new Wt4Parameters(weights.WtRest);
            SDOW = new Wt2Parameters(weights.SDOW);
            PerDiem = new Wt2Parameter(weights.PerDiem);
            TL = new Wt2Parameters(weights.TL);
            WB = new Wt2Parameters(weights.WB);
            POS = new Wt2Parameters(weights.POS);
            DHDFoL = new Wt2Parameters(weights.DHDFoL);
            WorkDays = new Wt3Parameters(weights.WorkDays);
            PDAfter = new Wt4Parameters(weights.PDAfter);
            PDBefore = new Wt4Parameters(weights.PDBefore);
            NormalizeDaysOff = new Wt2Parameter(weights.NormalizeDaysOff);
            if (weights.OvernightCitybulk != null)
            {
                OvernightCitybulk = new List<Wt2Parameter>();
                foreach (var item in weights.OvernightCitybulk)
                {
                    OvernightCitybulk.Add(new Wt2Parameter(item));
 
                }
            }


        }

        /// <summary>
        /// PURPOSE :AirCraftChanges
        /// </summary>
        [XmlElement("ACChg")]
        public Wt3Parameter AirCraftChanges { get; set; }

        /// <summary>
        /// PURPOSE : AM/PM 
        /// </summary>
        [XmlElement("AM-PM")]
        public Wt2Parameters AM_PM { get; set; }

        /// <summary>
        /// PURPOSE : Block of Days
        /// </summary>
        public Wt3Parameters BDO { get; set; }



        [XmlElement("LrgBlkDayOff")]
        public Wt2Parameter LrgBlkDayOff { get; set; }

       

        /// <summary>
        /// PURPOSE : Commutable Dead Heads
        /// </summary>
        public Wt3Parameters DHD { get; set; }



        /// <summary>
        /// PURPOSE : Commutable Dead Heads
        /// </summary>
         public WtCommutableLine CL { get; set; }

		public WtCommutableLineAuto  CLAuto { get; set; }
        /// <summary>
        /// PURPOSE : Commutable Lines
        /// </summary>

		[XmlElement("Commute")]
		public Commutability Commute { get; set; }
        /// <summary>
        /// PURPOSE : Days Of Month
        /// </summary>
         public DaysOfMonthWt SDO { get; set; }

        /// <summary>
        /// PURPOSE : Days of week
        /// </summary>
        /// 
        //public List<Wt> DOW { get; set; }
        public WtDaysOfWeek DOW { get; set; }

        /// <summary>
        /// PURPOSe : Duty Periods
        /// </summary>
        public Wt3Parameters DP { get; set; }


        /// <summary>
        /// PURPOSE : Equipment Type
        /// </summary>
        public Wt3Parameters EQUIP { get; set; }
        /// <summary>
        /// PURPOSE : ETOPS
        /// </summary>
        public Wt1Parameters ETOPS { get; set; }
        /// <summary>
        /// PURPOSE : ETOPS Reserve
        /// </summary>
        public Wt1Parameters ETOPSRes { get; set; }

        /// <summary>
        /// PURPOSE : Flight Time
        /// </summary>
        public Wt3Parameters FLTMIN { get; set; }

        /// <summary>
        /// PURPOSE : Ground Time
        /// </summary>
        public Wt3Parameters GRD { get; set; }

       /// <summary>
        /// InterConus
       /// </summary>
        public Wt2Parameters InterConus { get; set; }

        /// <summary>
        /// Legs per Duty Period
        /// </summary>
        public Wt3Parameters LEGS { get; set; }

        /// <summary>
        /// Legs per Pairing
        /// </summary>
        public Wt3Parameters WtLegsPerPairing { get; set; }  // frank changed name from WtLegsPerPays

        /// <summary>
        /// Number of Days Off
        /// </summary>
        public Wt2Parameters NODO { get; set; }

        /// <summary>
        /// Over Night Cities
        /// </summary>
        public Wt2Parameters RON { get; set; }

		/// <summary>
		/// Gets or sets the cities legs.
		/// </summary>
		/// <value>The cities legs.</value>
		public Wt2Parameters CitiesLegs { get; set; }



        /// <summary>
        /// PDOF
        /// </summary>
        public WtPDOFS WtPDOFS { get; set; }

        /// <summary>
        /// Rest
        /// </summary>
        public Wt4Parameters WtRest { get; set; }
        
        /// <summary>
        ///Start Days of the week
        /// </summary>
        public Wt2Parameters SDOW { get; set; }      

        /// <summary>
        /// PerDiem
        /// </summary>
        public Wt2Parameter PerDiem { get; set; }
        
        /// <summary>
        ///Tripk length
        /// </summary>
        public Wt2Parameters TL { get; set; }

        /// <summary>
        ///Work Block length
        /// </summary>
        public Wt2Parameters WB { get; set; }

        /// <summary>
        ///Position
        /// </summary>
        public Wt2Parameters POS { get; set; }


        public Wt2Parameters DHDFoL { get; set; }

        public Wt3Parameters WorkDays { get; set; }

        /// <summary>
        /// Partial Days After
        /// </summary>
        public Wt4Parameters PDAfter { get; set; }

        /// <summary>
        /// Partial Days Before
        /// </summary>
        public Wt4Parameters PDBefore { get; set; }

        /// <summary>
        /// Normalize days off
        /// </summary>
        public Wt2Parameter NormalizeDaysOff { get; set; }

        /// <summary>
        /// over night citybulk
        /// </summary>
        public List<Wt2Parameter> OvernightCitybulk { get; set; }

		/// <summary>
		/// DailyCommuteTimes for commutabel Line auto weight
		/// </summary>
		public List<CommuteTime> DailyCommuteTimes { get; set; }
    }
}
