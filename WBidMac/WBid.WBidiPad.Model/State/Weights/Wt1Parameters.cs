using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class Wt1Parameters
    {
        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }

        public Wt1Parameters()
        {
        }

        public Wt1Parameters(Wt1Parameters wt1Parameters)
        {
           

                Weight = wt1Parameters.Weight;

                if (wt1Parameters.lstParameters != null)
                {
                    lstParameters = new List<Wt1Parameter>();
                    foreach (var item in wt1Parameters.lstParameters)
                    {
                        lstParameters.Add(new Wt1Parameter(item));

                    }

                }
            

        }


        public List<Wt1Parameter> lstParameters { get; set; }
    }
    [Serializable]
    public class Wt1Parameter
    {
        public Wt1Parameter()
        {

        }


        public Wt1Parameter(Wt1Parameter wt1Parameter)
        {
            Weight = wt1Parameter.Weight;

        }

        [XmlAttribute("Weight")]
        public decimal Weight { get; set; }
    }
}
