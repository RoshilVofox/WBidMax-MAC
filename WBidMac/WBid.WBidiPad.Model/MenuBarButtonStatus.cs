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
    public class MenuBarButtonStatus
    {
        public MenuBarButtonStatus()
        {
        }
        public MenuBarButtonStatus(MenuBarButtonStatus menuBarButtonStatus)
        {
            IsVacationCorrection = menuBarButtonStatus.IsVacationCorrection;
            IsVacationDrop = menuBarButtonStatus.IsVacationDrop;
            IsOverlap = menuBarButtonStatus.IsOverlap;
            IsEOM = menuBarButtonStatus.IsEOM;
			IsMIL = menuBarButtonStatus.IsMIL;
        }
        [DataMember]
        [XmlAttribute("IsVacationCorrection")]
        public bool IsVacationCorrection { get; set; }

        [DataMember]
        [XmlAttribute("IsVacationDrop")]
        public bool IsVacationDrop { get; set; }

        [DataMember]
        [XmlAttribute("IsOverlap")]
        public bool IsOverlap { get; set; }

        [DataMember]
        [XmlAttribute("IsEOM")]
        public bool IsEOM { get; set; }

		[DataMember]
		[XmlAttribute("IsMIL")]
		public bool IsMIL { get; set; }
    }
}
