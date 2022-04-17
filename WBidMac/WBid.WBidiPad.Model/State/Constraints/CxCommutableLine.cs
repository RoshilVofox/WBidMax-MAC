using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [DataContract]
   
    public class CxCommutableLine
    {
        public  CxCommutableLine()
        {
        }
        public CxCommutableLine(CxCommutableLine cxCommutableLine)
        {
            CommuteToHome = cxCommutableLine.CommuteToHome;
            CommuteToWork = cxCommutableLine.CommuteToWork;
            AnyNight = cxCommutableLine.AnyNight;
            RunBoth = cxCommutableLine.RunBoth;
            MondayThu = new Times(cxCommutableLine.MondayThu);
            Friday = new Times(cxCommutableLine.Friday);
            Saturday = new Times(cxCommutableLine.Saturday);
            Sunday = new Times(cxCommutableLine.Sunday);
            MondayThuDefault = new Times(cxCommutableLine.MondayThuDefault);
            FridayDefault = new Times(cxCommutableLine.FridayDefault);
            SaturdayDefault = new Times(cxCommutableLine.SaturdayDefault);
            SundayDefault = new Times(cxCommutableLine.SundayDefault);

            if (cxCommutableLine.TimesList != null)
            {
                TimesList = new List<Times>();
                foreach (Times item in cxCommutableLine.TimesList)
                {
                    TimesList.Add(new Times(item));
                }
            }
        }

        [DataMember]
        [XmlElement("TimesList")]
        public List<Times> TimesList { get; set; }

        [DataMember]
        [XmlAttribute("CommuteToHome")]
        public bool CommuteToHome { get; set; }


        [DataMember]
        [XmlAttribute("CommuteToWork")]
        public bool CommuteToWork { get; set; }

        [DataMember]
        [XmlAttribute("AnyNight")]
        public bool AnyNight { get; set; }

        [DataMember]
        [XmlAttribute("RunBoth")]
        public bool RunBoth { get; set; }

        [DataMember]
        public Times  MondayThu { get; set; }


        [DataMember]
        public Times Friday { get; set; }

        [DataMember]
        public Times Saturday { get; set; }

        [DataMember]
        public Times Sunday { get; set; }


        [DataMember]
        public Times MondayThuDefault { get; set; }


        [DataMember]
        public Times FridayDefault { get; set; }

        [DataMember]
        public Times SaturdayDefault { get; set; }

        [DataMember]
        public Times SundayDefault { get; set; }





    }
}
