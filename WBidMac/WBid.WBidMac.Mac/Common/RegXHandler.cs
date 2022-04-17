using System;
using System.Text.RegularExpressions;

namespace WBid.WBidiPad.iOS.Utility
{
	public class RegXHandler
	{
		public RegXHandler ()
		{
		}
		public static bool EmailValidation(string value)
		{
			string matchpattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
           // string matchpattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
			if (!Regex.IsMatch(value, matchpattern))
			{
				return false;
			}
			return true;
		}
		public static bool EmployeeNumberValidation(string value)
		{
			if (!Regex.Match(value, "^[e,E,x,X,0-9][0-9]*$").Success)
			{
				return false;
			}
			return true;
		}
		public static bool NumberValidation(string value)
		{
			if (!Regex.Match(value, "^[0-9][0-9]*$").Success)
			{
				return false;
			}
			return true;
		}

		public static bool PhoneNumberValidation(string value)
		{
			if ((Regex.Match(value,@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$")).Success)
				return true;

			if ((!Regex.Match(value, "^[0-9][0-9]*$").Success) || (!Regex.Match(value, "^[0-9]{3}-[0-9]{3}-[0-9]{4}$").Success))
			{
				return false;
			}
			return true;
		}
		public static bool NameValidation(string value)
		{
			if (!Regex.Match(value, "^([ \\u00c0-\\u01ffa-zA-Z'\\-])+$").Success)
			{
				return false;
			}
			return true;
		}
		public static bool AnythingValidation(string value)
		{
            //??
			return false;
		}
	}
}

