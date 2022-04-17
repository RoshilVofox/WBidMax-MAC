using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    public class WBidIntialState
    {
        public WBidIntialState()
        {
        }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        /// <summary>
        /// PURPOSE: Sort Details
        /// </summary>
        public SortDetails SortDetails { get; set; }
        /// <summary>
        /// PURPOSE : Constrain Details
        /// </summary>
        public Constraints Constraints { get; set; }
        /// <summary>
        /// PURPOSE: Weight Details
        /// </summary>
        public Weights Weights { get; set; }

        /// <summary>
        /// PURPOSE:Holds the Main Window Size
        /// </summary>
        //public MainWindowSize MainWindowSize { get; set; }

        public CxWtState CxWtState { get; set; }

        /// <summary>
        /// PURPOSe :Theme
        /// </summary>
        public Theme Theme { get; set; }

        /// <summary>
        /// This will hold the check box states for the overlap,vacation and EOM checkbox
        /// </summary>
      //  public CheckBoxState CheckBoxState { get; set; }


        /// <summary>
        /// This will hold the menu bar button states for the overlap,vacation and EOM checkbox
        /// </summary>
        public MenuBarButtonStatus MenuBarButtonStatus { get; set; }
    }
}
