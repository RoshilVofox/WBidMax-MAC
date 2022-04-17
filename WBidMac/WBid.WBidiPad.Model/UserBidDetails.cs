using System;
using System.Runtime.Serialization;

namespace WBid.WBidiPad.Model
{
    [DataContract]
    public class UserBidDetails
    {
        public UserBidDetails()
        {
        }


        [DataMember]
        public int Year { get; set; }
        [DataMember]
        public int Month { get; set; }
        [DataMember]
        public int Round { get; set; }
        [DataMember]
        public string Domicile { get; set; }
        [DataMember]
        public string Position { get; set; }
        [DataMember]
        public int EmployeeNumber { get; set; }

    }
}
