using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [DataContract]
    [Serializable]
    public class WtPDOFS : List<WtPDOF>
    {
        public WtPDOFS()
        {

        }


        
    }
    [DataContract]
    [Serializable]
    public class WtPDOF
    {
        [DataMember]
        [XmlAttribute("Date")]
        public int Date { get; set; }
        [DataMember]
        [XmlAttribute("City")]
        public int City { get; set; }
        [DataMember]
        [XmlAttribute("Time")]
        public int Time { get; set; }
        [DataMember]
        [XmlAttribute("BA")]
        public bool IsBefore { get; set; }
        [DataMember]
        [XmlAttribute("Wt")]
        public decimal Weight { get; set; }


        public WtPDOF()
        {

        }

        public WtPDOF(WtPDOF wtPDOF)
        {
            Date = wtPDOF.Date; 
            City = wtPDOF.City;
            Time = wtPDOF.Time;
            IsBefore = wtPDOF.IsBefore;
            Weight = wtPDOF.Weight; 

        }   
    }
}
