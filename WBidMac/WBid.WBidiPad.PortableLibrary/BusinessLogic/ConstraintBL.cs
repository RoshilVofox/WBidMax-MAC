#region NameSpace
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
#endregion

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public static class ConstraintBL
    {



        /// <summary>
        /// Get days of Month List
        /// </summary>
        /// <returns></returns>
        public static List<DayOfMonth> GetDaysOfMonthList()
        {
            List<DayOfMonth> lstDaysOfMonth = new List<DayOfMonth>();
            DateTime calendarDate;
            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
          
            if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (GlobalSettings.CurrentBidDetails.Month == 3 || GlobalSettings.CurrentBidDetails.Month == 2))
            {
                startDate = startDate.AddDays(-1);
            }

            int iterationCount = (int)startDate.DayOfWeek;

            int id = 1;
            calendarDate = startDate;
            for (int count = 0; count < iterationCount; count++)
            {
                lstDaysOfMonth.Add(new DayOfMonth() { Id = id, IsEnabled = false, Day =null, Date = calendarDate,Status=0 });
                calendarDate = calendarDate.AddDays(1);
                
                id++;
            }

            iterationCount = GlobalSettings.CurrentBidDetails.BidPeriodStartDate.Subtract(startDate).Days + 1;
            calendarDate = startDate;
            for (int count = 1; count <= iterationCount; count++)
            {
                lstDaysOfMonth.Add(new DayOfMonth() { Id = id, IsEnabled = true, Day = calendarDate.Day, Date = calendarDate, Status = 0 });
                calendarDate = calendarDate.AddDays(1);
                id++;
            }


            for (int count = 1; count <= 35 - iterationCount; count++)
            {
                lstDaysOfMonth.Add(new DayOfMonth() { Id = id, IsEnabled = true, Day = calendarDate.Day, Date = calendarDate, Status = 0 });
                calendarDate = calendarDate.AddDays(1);
                id++;
            }

            for (int count = id; count <= 42; count++)
            {
                lstDaysOfMonth.Add(new DayOfMonth() { Id = id, IsEnabled = false, Day = calendarDate.Day, Date = calendarDate, Status = 0 });
                calendarDate = calendarDate.AddDays(1);
               id++;

            }

            return lstDaysOfMonth;
        }



        public static List<DateHelper> GetPartialDayList()
        {
             List<DateHelper> lstDateHelper = new List<DateHelper>();
            int dayCount = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            for (int count = 1; count <= dayCount; count++)
            {
                lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
                startDate = startDate.AddDays(1);
            }
            for (int count = dayCount + 1, i = 1; count <= 34; count++, i++)
            {
                lstDateHelper.Add(new DateHelper() { DateId = count, Date = startDate });
                startDate = startDate.AddDays(1);
            }
            return lstDateHelper;
        }

        public static List<string> GetPartialTimeList()
        {
            List<string> lstTime = new List<string>();
            for (int count = 3; count < 27; count++)
            {
                lstTime.Add(count.ToString().PadLeft(2, '0') + ":00");
                lstTime.Add(count.ToString().PadLeft(2, '0') + ":15");
                lstTime.Add(count.ToString().PadLeft(2, '0') + ":30");
                lstTime.Add(count.ToString().PadLeft(2, '0') + ":45");
 
            }
            lstTime.Add("27:00");

            return lstTime;
        }
    }
}
