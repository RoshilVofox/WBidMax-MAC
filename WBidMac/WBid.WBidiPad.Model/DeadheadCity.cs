using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    [ProtoContract]
    public class DeadheadCity
    {

        /// <summary>
        /// PURPOSe :from INI file for cities
        /// </summary>
        [ProtoMember(1)]
        public int CityId { get; set; }

        /// <summary>
        /// PURPOSE : CIty Name
        /// </summary>
        [ProtoMember(2)]
        public string City { get; set; }

        /// <summary>
        /// PURPOSE : Total count of deadheads to the city
        /// </summary>
        [ProtoMember(3)]
        public int CountTo { get; set; }

        /// <summary>
        /// PURPOSE: Total count of deadheads from the city
        /// </summary>
        [ProtoMember(4)]
        public int CountFrom { get; set; }

    }
}
