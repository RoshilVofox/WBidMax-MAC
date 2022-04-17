#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
#endregion

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public static class WeightBL
    {
        public static List<string> GetTimeList(int start, int end, int intrvl)
        {
            List<string> lstTime = new List<string>();
            for (int count = start; count < end; count++)
            {
                for (int intervel = 0; intervel < 60; intervel = intervel + intrvl)
                {
                    lstTime.Add(count.ToString().PadLeft(2, '0') + ":" + intervel.ToString().PadLeft(2, '0'));

                }

            }
            lstTime.Add(end.ToString() + ":00");

            return lstTime;
        }


        /// <summary>
        /// Validate Decimal
        /// </summary>
        /// <param name="fullString"></param>
        /// <param name="lastEntered"></param>
        /// <returns></returns>
        public static string ValidateDecimal(string fullString, string lastEntered)
        {

            string result = string.Empty;
            if (lastEntered == "-" && fullString != "0")
            {
                result = Convert.ToString(decimal.Parse(fullString) * -1);

            }
            else if (lastEntered == ".")
            {
                result = (!fullString.Contains(".")) ? ((fullString == string.Empty) ? "0" + lastEntered : fullString + lastEntered) : fullString;

            }
            else
            {
                if (fullString.Contains(".") && (fullString.Split('.')[1].Length == 2))
                {

                    result = fullString;
                }
                else
                {
                    result = fullString + lastEntered;
                }


                //decimal res;
                //if (decimal.TryParse(fullString + lastEntered, out res))
                //{
                //    result = res.ToString();
                //}
                //else
                //{
                //    result = fullString;
                //}

            }

            return result;

        }


        public static List<IntlNonConusCity> GetIntlNonConusCities()
        {
            List<IntlNonConusCity> lstCities = new List<IntlNonConusCity>();
          
         lstCities= GlobalSettings.WBidINIContent.Cities
                                                .Where(x => (x.International || x.NonConus))
                                                .Select(y=>new IntlNonConusCity
                                                                {
                                                                    City= y.Name,CityId=y.Id
                                                                }
                                                        ).ToList();


         lstCities.Insert(0,new IntlNonConusCity() { CityId = 0, City = "All NonConus" });
         lstCities.Insert(0,new IntlNonConusCity() { CityId = -1, City = "All Intl" });
        
         return lstCities;


        }
    }



}
