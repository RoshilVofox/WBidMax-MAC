using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class UserInformation
    {
        ///// <summary>
        ///// First Name
        ///// </summary>
        ///// 
        //[XmlAttribute("FirstName")]
        //public string FirstName { get; set; }
        ///// <summary>
        ///// Last Name
        ///// </summary>
        //[XmlAttribute("LastName")]
        //public string LastName { get; set; }
        ///// <summary>
        ///// Employee Number
        ///// </summary>
        //[XmlAttribute("EmpNo")]
        //public string EmpNo { get; set; }

        ///// <summary>
        ///// Domicile
        ///// </summary>
        //[XmlAttribute("Domicile")]
        //public string Domicile { get; set; }

        ///// <summary>
        ///// Position
        ///// </summary>
        //[XmlAttribute("Position")]
        //public string Position { get; set; }

        ///// <summary>
        ///// Seniority Number
        ///// </summary>
        //[XmlAttribute("SeniorityNumber")]
        //public int SeniorityNumber { get; set; }

        ///// <summary>
        ///// Email
        ///// </summary>
        //[XmlAttribute("Email")]
        //public string Email { get; set; }

        ///// <summary>
        ///// To check male or femaie.true if it is female.
        ///// </summary>
        //[XmlAttribute("isFemale")]
        //public bool IsFemale { get; set; }

        //[XmlAttribute("CellNumber")]
        //public string CellNumber { get; set; }

        ///// <summary>
        ///// AcceptMail
        ///// </summary>
        //[XmlAttribute("AcceptMail")]
        //public bool AcceptMail { get; set; }

        /// <summary>
        /// First Name
        /// </summary>
        ///
        [XmlAttribute("FirstName")]
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        [XmlAttribute("LastName")]
        public string LastName { get; set; }
        /// <summary>
        /// Employee Number
        /// </summary>
        [XmlAttribute("EmpNo")]
        public string EmpNo { get; set; }

        /// <summary>
        /// Domicile
        /// </summary>
        [XmlAttribute("Domicile")]
        public string Domicile { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        [XmlAttribute("Position")]
        public string Position { get; set; }

        /// <summary>
        /// LastBidPosition
        /// </summary>
        [XmlAttribute("LastBidPosition")]
        public string LastBidPosition { get; set; }

        /// <summary>
        /// To check male or femaie.true if it is female.
        /// </summary>
        [XmlAttribute("isFemale")]
        public bool IsFemale { get; set; }

        /// <summary>
        /// CellNumber
        /// </summary>
        [XmlAttribute("CellNumber")]
        public string CellNumber { get; set; }

        /// <summary>
        /// Seniority Number
        /// </summary>
        [XmlAttribute("SeniorityNumber")]
        public int SeniorityNumber { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [XmlAttribute("Email")]
        public string Email { get; set; }

        /// <summary>
        /// AcceptMail
        /// </summary>
        [XmlAttribute("AcceptMail")]
        public bool AcceptMail { get; set; }

        /// <summary>
        /// PaidUntil Date
        /// </summary>
        [XmlAttribute("PaidUntilDate")]
        public DateTime PaidUntilDate { get; set; }
        /// <summary>
        /// IsFree
        /// </summary>
        [XmlAttribute("IsFree")]
        public bool IsFree { get; set; }

        /// <summary>
        /// IsYearlySubscribed
        /// </summary>
        [XmlAttribute("IsYearlySubscribed")]
        public bool IsYearlySubscribed { get; set; }

        /// <summary>
        /// IsMonthlySubscribed
        /// </summary>
        [XmlAttribute("IsMonthlySubscribed")]
        public bool IsMonthlySubscribed { get; set; }

        /// <summary>
        /// TopSubscriptionLine
        /// </summary>
        [XmlAttribute("TopSubscriptionLine")]
        public string TopSubscriptionLine { get; set; }

        /// <summary>
        /// SecondSubscriptionLine
        /// </summary>
        [XmlAttribute("SecondSubscriptionLine")]
        public string SecondSubscriptionLine { get; set; }

        /// <summary>
        /// ThirdSubscriptionLine
        /// </summary>
        [XmlAttribute("ThirdSubscriptionLine")]
        public string ThirdSubscriptionLine { get; set; }

    }
}
