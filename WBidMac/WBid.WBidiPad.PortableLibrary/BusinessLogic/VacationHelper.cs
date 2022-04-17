#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;

#endregion

namespace VacationCorrection
{
    public class VacationHelper
    {
        public static DateTime SetDate(int day, bool isLastTrip)
        {

            //  Jan:	Jan 1 to Jan 30
            //   Feb:	Jan 31 to Mar 1
            //    Mar:	Mar 2 to Mar 31

            var tempMonth = GlobalSettings.CurrentBidDetails.Month;
            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Month == 2)
            {
                if (day == 31)
                {
                    tempMonth--;
                }
                //Eg BAQW01---  in this case  trip start date may either February 1 or  march 1
                else if (day == 1 && isLastTrip)
                {
                    tempMonth++;
                }
            }
            return new DateTime(GlobalSettings.CurrentBidDetails.Year, tempMonth, day);

        }



        public static int ConvertMinutesToHhhmmInt(int minutes)
        {
            int hhh = (minutes / 60) * 100;
            int mm = minutes % 60;
            return hhh + mm;
        }
    }
}
