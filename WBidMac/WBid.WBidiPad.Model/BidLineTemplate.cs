using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class BidLineTemplate 
    {
        public BidLineTemplate()
        {
            TemplateName = "Default";
            BorderType = 2;
        }


        /// <summary>
        /// Check whether the item holds the default template or not in the case modern bid line view
        /// </summary>
     
        [ProtoMember(1)]
        public string TemplateName { get; set; }
     
        //[ProtoMember(2)]
        //public int Day { get; set; }
        //[ProtoMember(3)]
        //public int Month { get; set; }
       // [ProtoMember(4)]
       // public string DayOfWeek { get; set; }
        [ProtoMember(5)]
        public string TripNum { get; set; }
        [ProtoMember(6)]
        public string ArrStaLastLeg { get; set; }

        //-------------------------------------------------------------
        //Using to bind the values to UI

   
        [ProtoMember(7)]
        public string TripNumDisplay { get; set; }
        

      
        [ProtoMember(8)]
        public string ArrStaLastLegDisplay { get; set; }
    


        //-------------------------------------------------------------

        [ProtoMember(9)]
        public string AMPMType { get; set; }

        //N-normal days,W-week ends,V-vacation
        //public char DayType { get; set; }

        private char _dayType = ' ';
        [ProtoMember(10)]
        public char DayType
        {
            get
            {
                return _dayType;
            }
            set
            {
                _dayType = value;
              
            }
        }

       
        [ProtoMember(11)]
        public string DayBackColor { get; set; }
     
      
        [ProtoMember(12)]
        public string DayStringBackColor { get; set; }
       

       
        [ProtoMember(13)]
        public string TripBackColor { get; set; }
        

        //  If we have multiple color in single dutyperiod we will use this color ie VO/VD
        //---------------------------------------------------------------------------------
      
        [ProtoMember(14)]
        public string TripBackColor1 { get; set; }
        
        //---------------------------------------------------------------------------------


      
        [ProtoMember(15)]
        public string DayForeColor { get; set; }
      

      
        [ProtoMember(16)]
        public string DayStringForeColor { get; set; }
      

     
        [ProtoMember(17)]
        public string TripForeColor { get; set; }
       

        [ProtoMember(18)]
        public string TripName { get; set; }

       

        [ProtoMember(19)]
        public bool IsInCurrentMonth { get; set; }

        /// <summary>
        /// Border Type 1 - BorderThickness - 0000
        /// Border Type 2 - BorderThickness - 0010
        /// </summary>
        [ProtoMember(20)]
        public int BorderType { get; set; }

        [ProtoMember(21)]
        public string ToolTip { get; set; }

        //public string ToolTipTop { get; set; }
        [ProtoMember(22)]
        public string ToolTipBottom { get; set; }

        //[ProtoMember(23)]
        //public int Year { get; set; }

        //public DateTime Date { get; set; }
        private DateTime _date;
        
        [ProtoMember(23)]
        public DateTime Date
        {
            get { return _date; }
            set { _date = new DateTime(value.Ticks, DateTimeKind.Utc); }
        }
        [ProtoMember(24)]
        public int DutyPeriodNo { get; set; }
         [ProtoMember(25)]
        public int BidLineType { get; set; }

		[ProtoMember(26)]
		public string workStatus { get; set; }

        [ProtoMember(27)]
        public decimal Value { get; set; }





    }
}
