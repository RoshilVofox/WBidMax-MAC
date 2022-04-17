using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{

    public class Wt3Parameters
    {
        [XmlAttribute("ThirdCell")]
        public int SecondlValue { get; set; }

        [XmlAttribute("Type")]
        public int ThrirdCellValue { get; set; }

        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }


        public Wt3Parameters()
        {

        }

        public Wt3Parameters(Wt3Parameters wt3Parameters)
        {
            SecondlValue = wt3Parameters.SecondlValue;
            ThrirdCellValue = wt3Parameters.ThrirdCellValue;
            Weight = wt3Parameters.Weight;


               if( wt3Parameters.lstParameters!=null)
               {
                   lstParameters=new List<Wt3Parameter>();
                   foreach(var item in      wt3Parameters.lstParameters)
                   {
                      lstParameters.Add(new Wt3Parameter(item));
                   }
               }
        }


        public List<Wt3Parameter> lstParameters { get; set; }
    }

    public class Wt3Parameter
    {

        public Wt3Parameter()
        {

        }

        public Wt3Parameter(Wt3Parameter wt3Parameter)
        {
            SecondlValue = wt3Parameter.SecondlValue;
            ThrirdCellValue = wt3Parameter.ThrirdCellValue;
            Weight = wt3Parameter.Weight;
        }


        [XmlAttribute("ThirdCell")]
        public int SecondlValue { get; set; }

        [XmlAttribute("Type")]
        public int ThrirdCellValue { get; set; }

        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }
    }
}
