#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
#endregion

namespace WBid.WBidiPad.Model
{
    [DataContract]
    [Serializable]
    public class WtCommutableLine
    {
        [DataMember]
        [XmlElement("TimesList")]
        public List<Times> TimesList { get; set; }

        [DataMember]
        [XmlElement("DefaultTimes")]
        public List<Times> DefaultTimes{ get; set; }


        [DataMember]
        [XmlAttribute("Domicile")]
        public decimal InDomicile { get; set; }


        [DataMember]
        [XmlAttribute("BothEnds")]
        public decimal BothEnds { get; set; }

        [DataMember]
        [XmlAttribute("Type")]
        public int Type { get; set; }


        public WtCommutableLine()
        {

        }

        public WtCommutableLine(WtCommutableLine wtCommutableLine)
        {
            InDomicile = wtCommutableLine.InDomicile;
            BothEnds = wtCommutableLine.BothEnds;
            Type = wtCommutableLine.Type;

            if (wtCommutableLine.TimesList != null)
            {
                TimesList = new List<Times>();
                foreach (var item in wtCommutableLine.TimesList)
                {
                    TimesList.Add(new Times(item));
                    
                }
            }


            if (wtCommutableLine.DefaultTimes != null)
            {
                DefaultTimes = new List<Times>();
                foreach (var item in wtCommutableLine.DefaultTimes)
                {
                    DefaultTimes.Add(new Times(item));

                }
            }

        }


    }

   
}
