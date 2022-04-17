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
    public class CheckBoxState
    {
        [DataMember]
        [XmlAttribute("IsVacationCorrectionChecked")]
        public bool IsVacationCorrectionChecked { get; set; }

        [DataMember]
        [XmlAttribute("IsVacationDropChecked")]
        public bool IsVacationDropChecked { get; set; }

        [DataMember]
        [XmlAttribute("IsOverlapChecked")]
        public bool IsOverlapChecked { get; set; }

        [DataMember]
        [XmlAttribute("IsEOMChecked")]
        public bool IsEOMChecked { get; set; }
    }
}
