using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    
    public class Cx3Parameters
    {
        public Cx3Parameters()
        {
        }
        public Cx3Parameters(Cx3Parameters cx3Parameters)
        {
			if (cx3Parameters != null) {
				SecondcellValue = cx3Parameters.SecondcellValue;
				ThirdcellValue = cx3Parameters.ThirdcellValue;
				Type = cx3Parameters.Type;
				Value = cx3Parameters.Value;

				if (cx3Parameters.lstParameters != null) {
					lstParameters = new List<Cx3Parameter> ();
					foreach (Cx3Parameter item in cx3Parameters.lstParameters) {
						lstParameters.Add (new Cx3Parameter (item));
					}
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

        public List<Cx3Parameter> lstParameters { get; set; }
    }
    
    public class Cx3Parameter
    {
        public  Cx3Parameter()
        {
        }
        public Cx3Parameter(Cx3Parameter cx3Parameter)
        {
			 SecondcellValue = cx3Parameter.SecondcellValue;
            ThirdcellValue = cx3Parameter.ThirdcellValue;
            Type = cx3Parameter.Type;
            Value = cx3Parameter.Value;
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
