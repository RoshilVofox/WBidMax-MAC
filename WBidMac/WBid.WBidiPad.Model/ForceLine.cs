using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [DataContract]
    [Serializable]
    public class ForceLine
    {
        public  ForceLine()
        {
    }
        public ForceLine(ForceLine forceLine)
        {
            IsBlankLinetoBottom = forceLine.IsBlankLinetoBottom;
            IsReverseLinetoBottom = forceLine.IsReverseLinetoBottom;
        }
        /// <summary>
        /// IsBlankLinetoBottom
        /// </summary>
        [DataMember]
        [XmlAttribute("IsBlankLinetoBottom")]
        public bool IsBlankLinetoBottom { get; set; }
        /// <summary>
        /// IsReverseLinetoBottom
        /// </summary>
        [DataMember]
        [XmlAttribute("IsReverseLinetoBottom")]
        public bool IsReverseLinetoBottom { get; set; }
    }
}
