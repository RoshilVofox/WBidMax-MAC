using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class PairingExport
    {
        #region Properties
        [XmlAttribute("IsCentralTime")]
        public bool IsCentralTime { get; set; }

        [XmlAttribute("IsEntirePairing")]
        public bool IsEntirePairing { get; set; }

        [XmlAttribute("SubjectLine")]
        public bool IsSubjectLineSelected { get; set; }

      
        #endregion



    }
}
