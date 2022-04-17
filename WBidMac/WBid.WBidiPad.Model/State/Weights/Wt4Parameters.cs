using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
   
    public class Wt4Parameters
    {
        [XmlAttribute("FirstCell")]
        public int FirstValue { get; set; }

        [XmlAttribute("ThirdCell")]
        public int SecondlValue { get; set; }

        [XmlAttribute("Type")]
        public int ThrirdCellValue { get; set; }

        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }

        public Wt4Parameters()
        {
           
        }
        public Wt4Parameters(Wt4Parameters wt4Parameters)
        {
            FirstValue = wt4Parameters.FirstValue;
            SecondlValue = wt4Parameters.SecondlValue;
            ThrirdCellValue = wt4Parameters.ThrirdCellValue;
            Weight = wt4Parameters.Weight;

            if (wt4Parameters.lstParameters != null)
            {
                lstParameters = new List<Wt4Parameter>();
                foreach (var item in wt4Parameters.lstParameters )
                {
                    lstParameters.Add(new Wt4Parameter(item));
                    
                }
            }
        }


        public List<Wt4Parameter> lstParameters { get; set; }
    }
    
    public class Wt4Parameter
    {

        public Wt4Parameter()
        {
                
        }

        public Wt4Parameter(Wt4Parameter wt4Parameter)
        {
            FirstValue = wt4Parameter.FirstValue;
            SecondlValue = wt4Parameter.SecondlValue;
            ThrirdCellValue = wt4Parameter.ThrirdCellValue;
            Weight = wt4Parameter.Weight;
        }

        [XmlAttribute("FirstCell")]
        public int FirstValue { get; set; }

        [XmlAttribute("ThirdCell")]
        public int SecondlValue { get; set; }

        [XmlAttribute("Type")]
        public int ThrirdCellValue { get; set; }

        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }
    }

}
