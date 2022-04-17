#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model
{
   public  class DayOfMonth
    {

        public int Id { get; set; }

        public int? Day { get; set; }

        public DateTime  Date { get; set; }

        public bool IsEnabled { get; set; }
        
        //  0-Normal Day, 1- Work day 2-NonWorking
        public int Status { get; set; }
    }
}
