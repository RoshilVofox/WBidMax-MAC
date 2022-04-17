#region NameSpace
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Model;

#endregion
namespace WBid.WBidiPad.SharedLibrary.Parser
{
    public class TripTtpParser
    {
        List<CityPair> lstCity;
        #region Public Methods
        /// <summary>
        /// read trips.ttp and add the contents to List<CityPair>
        /// </summary>
        /// <param name="filePath"></param>
        public List<CityPair> ParseCity(string filePath)
        {

            lstCity = new List<CityPair>();
            ReadFile(filePath);
            return lstCity;
        }
        #endregion

        #region private Methods
        /// <summary>
        /// read trips.ttp and add the contents to List<CityPair>
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadFile(string filePath)
        {
            List<string> fileContent = new List<string>();
            if (!File.Exists(filePath))
                return;

            using (StreamReader sr = new StreamReader(filePath))
            {
                string record;
                // Read and display lines from the file until the end of 
                // the file is reached.
                string distance = string.Empty;
                while ((record = sr.ReadLine()) != null)
                {

                    if (record.Length >= 11)
                    {
                        CityPair CityPair = new CityPair();
                        CityPair.City1 = record.Substring(0, 3).ToString();
                        CityPair.City2 = record.Substring(4, 3).ToString();
                        if (string.IsNullOrEmpty(CityPair.City1) || string.IsNullOrEmpty(CityPair.City2))
                            continue;
                        distance = record.Substring(8, 3);
                        CityPair.Distance = Convert.ToDecimal((distance == string.Empty) ? "0" : distance);
                        lstCity.Add(CityPair);
                    }
                }
            }
        }
        #endregion
    }
}
