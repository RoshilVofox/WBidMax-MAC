#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class DSTProperties
    {

        public static DateTime[] SetDSTProperties()
        {
            DateTime[] dateofDST = new DateTime[2];
            int countMarSundays = 0;
            int countNovSundays = 0;
            for (int d = 1; d < 14; d++)
            {
                DateTime valueMar = new DateTime(DateTime.Now.Year, 3, d);
                if (valueMar.DayOfWeek == 0)
                {
                    countMarSundays++;
                    if (countMarSundays == 2)
                    {
                        //GlobalSettings.FirstDayOfDST = new DateTime(DateTime.Now.Year, 3, d);
                        dateofDST[0] = new DateTime(DateTime.Now.Year, 3, d); ;
                        return dateofDST;
                    }
                }
                DateTime valueNov = new DateTime(DateTime.Now.Year, 11, d);
                if (valueNov.DayOfWeek == 0 && countNovSundays == 0)
                {
                    countNovSundays++;
                    //GlobalSettings.LastDayOfDST = new DateTime(DateTime.Now.Year, 11, d);
                    dateofDST[1] = new DateTime(DateTime.Now.Year, 11, d);
                }
            }

            return dateofDST;
        }
    }
}
