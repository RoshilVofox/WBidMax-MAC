using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    public class AmPmConfigure
    {


        /// <summary>
        /// PURPOSE : HowCalcAmPm
        /// </summary>
        [XmlAttribute("HowCalcAmPm")]
        public int HowCalcAmPm { get; set; }

        [XmlIgnore]
        public TimeSpan AmPush { get; set; }

        [XmlAttribute("AmPush")]
        public long AmPushTicks
        {
            get { return AmPush.Ticks; }
            set { AmPush = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan AmLand { get; set; }

        [XmlAttribute("AmLand")]
        public long AmLandTicks
        {
            get { return AmLand.Ticks; }
            set { AmLand = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan PmPush { get; set; }

        [XmlAttribute("PmPush")]
        public long PmPushTicks
        {
            get { return PmPush.Ticks; }
            set { PmPush = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan PmLand { get; set; }

        [XmlAttribute("PmLand")]
        public long PmLandTicks
        {
            get { return PmLand.Ticks; }
            set { PmLand = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan NitePush { get; set; }

        [XmlAttribute("NitePush")]
        public long NitePushTicks
        {
            get { return NitePush.Ticks; }
            set { NitePush = new TimeSpan(value); }
        }


        [XmlIgnore]
        public TimeSpan NiteLand { get; set; }

        [XmlAttribute("NiteLand")]
        public long NiteLandTicks
        {
            get { return NiteLand.Ticks; }
            set { NiteLand = new TimeSpan(value); }
        }


        /// <summary>
        /// PURPOSE : NumOpposites
        /// </summary>
        [XmlAttribute("NumOrPercentage")]
        public int NumberOrPercentageCalc { get; set; }


        /// <summary>
        /// PURPOSE : NumOpposites
        /// </summary>
        [XmlAttribute("NumOpposites")]
        public int NumOpposites { get; set; }

        /// <summary>
        /// PURPOSE : PctOpposities
        /// </summary>
        [XmlAttribute("PctOpposities")]
        public decimal PctOpposities { get; set; }


    }
}
