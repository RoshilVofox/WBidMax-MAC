using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    
    public class Cx4Parameters
    {
        public Cx4Parameters()
        {
        }
        public Cx4Parameters(Cx4Parameters cx4Parameters)
        {
            SecondcellValue = cx4Parameters.SecondcellValue;
            ThirdcellValue = cx4Parameters.ThirdcellValue;
            Type = cx4Parameters.Type;
            Value = cx4Parameters.Value;

            if (cx4Parameters.LstParameter != null)
            {
                LstParameter = new List<Cx4Parameter>();
                foreach (Cx4Parameter item in cx4Parameters.LstParameter)
                {
                    LstParameter.Add(new Cx4Parameter(item));
                }
            }
        }
        [XmlAttribute("SecondCell")]
        public string SecondcellValue { get; set; }

        [XmlAttribute("ThirdCell")]
        public string ThirdcellValue { get; set; }

        [XmlAttribute("Type")]
        public int Type { get; set; }

        [XmlAttribute("Value")]
        public int Value { get; set; }

        public List<Cx4Parameter> LstParameter { get; set; }
    }
    
    public class Cx4Parameter
    {
        public Cx4Parameter()
        {
        }
        public Cx4Parameter(Cx4Parameter cx4Parameter)
        {
            SecondcellValue = cx4Parameter.SecondcellValue;
            ThirdcellValue = cx4Parameter.ThirdcellValue;
            Type = cx4Parameter.Type;
            Value = cx4Parameter.Value;
        }
        [XmlAttribute("SecondCell")]
        public string SecondcellValue { get; set; }

        [XmlAttribute("ThirdCell")]
        public string ThirdcellValue { get; set; }

        [XmlAttribute("Type")]
        public int Type { get; set; }

        [XmlAttribute("Value")]
        public int Value { get; set; }
    }
}
