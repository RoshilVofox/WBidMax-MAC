using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
	public class AppliedStates
	{
		public AppliedStates ()
		{

		}
		//ww
		public string Key { get; set; }
		public List<AppliedStateType> AppliedStateTypes { get; set; }
	}
}

