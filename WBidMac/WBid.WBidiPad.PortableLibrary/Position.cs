using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.PortableLibrary
{
    public class Position
    {
        /// <summary>
        /// Position
        /// </summary>
        public string PositionName { get; set; }

        /// <summary>
        /// Short string-1 Character (C,F,A)
        /// </summary>
        public string ShortStr { get; set; }

        /// <summary>
        /// Long string --2 Character (CP,FO,FA)
        /// </summary>
        public string LongStr { get; set; }
    }
}
