using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     [Serializable]
   public class Wt2Parameters
    {

        [XmlAttribute("Type")]
        public int Type { get; set; }

        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }

        public Wt2Parameters()
        {
                
        }

        public Wt2Parameters(Wt2Parameters wt2Parameters)
        {
			if (wt2Parameters != null) {
				Type = wt2Parameters.Type;
				Weight = wt2Parameters.Weight;

				if (wt2Parameters.lstParameters != null) {
					lstParameters = new List<Wt2Parameter> ();
					foreach (var item in wt2Parameters.lstParameters) {
						lstParameters.Add (new Wt2Parameter (item));
                    
					}
 
				}
			}

        }


        public List<Wt2Parameter> lstParameters { get; set; }
    }
     [Serializable]
   public class Wt2Parameter
   {
         public Wt2Parameter()
         {
                
         }


         public Wt2Parameter(Wt2Parameter wt2Parameter)
         {
             Type = wt2Parameter.Type;
             Weight = wt2Parameter.Weight;

         }



       [XmlAttribute("Type")]
       public int Type { get; set; }

       [XmlAttribute("Weight")]
       public decimal Weight { get; set; }
   }
}
