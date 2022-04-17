using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBid.WBidiPad.Model
{
	public class MILParams
	{

		public DateTime MilStartDate { get; set; }

		public DateTime MilEndDate { get; set; }

		public List<Line> Lines { get; set; }
	}         
}
