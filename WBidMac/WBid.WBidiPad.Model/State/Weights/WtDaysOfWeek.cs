using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     [Serializable]
    public class WtDaysOfWeek
    {


         public WtDaysOfWeek()
         {

         }

         public WtDaysOfWeek(WtDaysOfWeek wtDaysOfWeek)
         {
             IsWork = wtDaysOfWeek.IsWork;
             IsOff = wtDaysOfWeek.IsOff;
             if (wtDaysOfWeek.lstWeight != null)
             {
                 lstWeight = new List<Wt>();
                 foreach (var item in wtDaysOfWeek.lstWeight)
                 {
                     lstWeight.Add(new Wt(item));
                     
                 }
             }

         }

        public List<Wt> lstWeight { get; set; }

        [XmlAttribute("IsWork")]
        public bool IsWork { get; set; }

        [XmlAttribute("IsOff")]
        public bool IsOff { get; set; }
    }
}
