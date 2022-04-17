using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     //[Serializable]
    public class DaysOfMonthCx
    {

         public DaysOfMonthCx()
         { }
         public DaysOfMonthCx(DaysOfMonthCx daysOfMonthCx)
         {
             if (daysOfMonthCx.OFFDays != null)
             {
                 OFFDays = new List<int>();
                 foreach (int item in daysOfMonthCx.OFFDays)
                 {
                     OFFDays.Add(item);
                 }
             }
             if (daysOfMonthCx.WorkDays != null)
             {
                 WorkDays = new List<int>();
                 foreach (int item in daysOfMonthCx.WorkDays)
                 {
                     WorkDays.Add(item);
                 }
             }
         }
        //[XmlAttribute("Work")]
        //public bool Work { get; set; }
        //[XmlAttribute("Off")]
        //public bool Off { get; set; }
        [XmlElement("OFFDays")]
        public List<int> OFFDays { get; set; }
        [XmlElement("WorkDays")]
        public List<int> WorkDays { get; set; }

    }
}
