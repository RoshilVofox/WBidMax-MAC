using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace WBid.WBidiPad.Model
{
	public class AppliedStateType
	{
		public AppliedStateType ()
		{
			  
		}
		public string Key { get; set; }
		public List<string> Value { get; set; }     
	}
}

