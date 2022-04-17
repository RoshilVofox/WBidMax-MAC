#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
#endregion

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class WtDutyPeriods
    {

        /// <summary>
        /// PURPOSe :Apply
        /// </summary>
        [XmlAttribute("Apply")]
        public int Apply { get; set; }


        /// <summary>
        /// PURPOSe :Time
        /// </summary>
        [XmlAttribute("Length")]
        public int Length { get; set; }


        /// <summary>
        /// PURPOSe :Weight
        /// </summary>
        [XmlAttribute("WT")]
        public decimal Weight { get; set; }

        public List<WtDutyPeriod> LstProperties { get; set; }


        public WtDutyPeriods()
        {

        }


        public WtDutyPeriods(WtDutyPeriods wtDutyPeriods)
        {
            Apply = wtDutyPeriods.Apply;
            Length = wtDutyPeriods.Length;
            Weight = wtDutyPeriods.Weight;

            if (wtDutyPeriods.LstProperties != null)
            {
                LstProperties = new List<WtDutyPeriod>();
                foreach (var item in wtDutyPeriods.LstProperties)
                {
                    LstProperties.Add(new WtDutyPeriod(item));
                }
            }

        }

    }
    public class WtDutyPeriod
    {

        /// <summary>
        /// PURPOSe :Apply
        /// </summary>
        [XmlAttribute("Apply")]
        public int Apply { get; set; }


        /// <summary>
        /// PURPOSe :Time
        /// </summary>
        [XmlAttribute("Length")]
        public int Length { get; set; }


        /// <summary>
        /// PURPOSe :Weight
        /// </summary>
        [XmlAttribute("WT")]
        public decimal Weight { get; set; }


        public WtDutyPeriod()
        {

        }

        public WtDutyPeriod(WtDutyPeriod wtDutyPeriod)
        {
            Apply = wtDutyPeriod.Apply;
            Length = wtDutyPeriod.Length;
            Weight = wtDutyPeriod.Weight; 

        }



    }
}
