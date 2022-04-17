using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class LineData
    {
        public int LineNum { get; set; }
        /// <summary>
        /// Hold the constraint points for all lines
        /// </summary>
        public ConstraintPoints ConstriantPoints { get; set; }

        /// <summary>
        /// Hold the weight points for all lines.
        /// </summary>
        public WeightPoints WeightPoints { get; set; }
    }
}
