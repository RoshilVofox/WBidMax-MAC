using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     
    public class Cx2Parameters
    {
    
         public Cx2Parameters()
         {
         }
         public Cx2Parameters(Cx2Parameters cx2Parameters)
         {
             Type = cx2Parameters.Type;
             Value = cx2Parameters.Value;

             if (cx2Parameters.lstParameters != null)
             {
                 lstParameters = new List<Cx2Parameter>();
                 foreach (Cx2Parameter item in cx2Parameters.lstParameters)
                 {
                     lstParameters.Add(new Cx2Parameter(item));
                 }
             }
         }
        [XmlAttribute("Type")]
        public int Type { get; set; }

        [XmlAttribute("Value")]
        public int Value { get; set; }

        public List<Cx2Parameter> lstParameters { get; set; }


    }
    
    public class Cx2Parameter
    {
        public  Cx2Parameter()
        {
        }
        public Cx2Parameter(Cx2Parameter cx2Parameter)
        {
            Type = cx2Parameter.Type;
            Value = cx2Parameter.Value;
        }

        [XmlAttribute("Type")]
        public int Type { get; set; }

        [XmlAttribute("Value")]
        public int Value { get; set; }
    }
}
