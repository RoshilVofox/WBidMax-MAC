using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBid.WBidiPad.Model
{
	public class SplitPointCities:List<SplitPointCity>
	{

	}
	public class SplitPointCity
	{

		public string Domicile { get; set; }

		public List<string> Cities { get; set; }
	}
}

