using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.SharedLibrary.Parser
{
    public class WBidUpdateParser
    {

        #region Private variables

        private List<string> _updates = new List<string>();

        private List<string> _cities = new List<string>();

        private List<string> _hotels = new List<string>();

        private List<string> _domiciles = new List<string>();

        private List<string> _equipment = new List<string>();

        private List<string> _equipmentType = new List<string>();

        private List<string> _internationals = new List<string>();

        private List<string> _nonConus = new List<string>();

        private List<string> _splitpointcities = new List<string>();

        private WBidUpdate _wBidUpdate;
        #endregion

        #region Methods


        public WBidUpdate ParseWBidUpdateFile(string filePath)
        {
            //Read the WBid update  files and added to assosiated list
            ReadFile(filePath);
            //Map readed content to WBidUpdate domain

            if (_wBidUpdate != null)
            {
                MapListContentToDomain();


            }

            return _wBidUpdate;
        }


        /// <summary>
        /// PURPOSE : Read the WBid update  files and added to assosiated list
        /// </summary>
        private void ReadFile(string filePath)
        {
            //  List<string> fileContent = new List<string>();
            if (!File.Exists(filePath))
            {
                return;
            }
            using (StreamReader sr = new StreamReader(filePath))
            {


                _wBidUpdate = new WBidUpdate();
                string line, oldLine, currentParse;
                currentParse = string.Empty;
                oldLine = "a";

                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {

                    //if oldline ==string.empty, we are assigning the  line valu to "Currentparse"
                    //Current parse value decides the values to added to which list. 
                    if (oldLine == string.Empty)
                    {
                        currentParse = line;
                        oldLine = line;
                        continue;
                    }

                    oldLine = line;

                    if (line == string.Empty)
                        continue;

                    switch (currentParse)
                    {
                        case "[Updates]":
                            _updates.Add(line);
                            break;

                        case "[Cities]":
                            _cities.Add(line);
                            break;

                        case "[Hotels]":
                            _hotels.Add(line);
                            break;

                        case "[Domiciles]":
                            _domiciles.Add(line);
                            break;

                        case "[Equipment]":
                            _equipment.Add(line);
                            break;

                        case "[EquipTypes]":
                            _equipmentType.Add(line);
                            break;

                        case "[SplitCities]":
                            _splitpointcities.Add(line);
                            break;


                    }
                }


            }

        }

        /// <summary>
        /// PURPOSE :Map readed content to WBidUpdate domain
        /// </summary>
        private void MapListContentToDomain()
        {



            ParseUpdatesList();

            ///Parse cities
            ParseCitiesList();

            //Parse Hotels
            ParseHotelsList();

            ///Parse domiciles
            ParseDomicileList();


            ///Parse equipment 
            ParseEquipment();

            ///Parse equipment types
            ParseEquipmentTypes();


            //parse split point cities
            ParseSplitPointCities();

        }

        /// <summary>
        /// PURPOSE : Parse Updates List
        /// </summary>
        private void ParseUpdatesList()
        {
            string[] strArr;
            _wBidUpdate.Updates = new Updates();

            foreach (string str in _updates)
            {
                strArr = str.Split('=');


                switch (strArr[0])
                {
                    case "Hotels":
                        _wBidUpdate.Updates.Hotels = int.Parse(strArr[1]);
                        break;

                    case "FAList":
                        _wBidUpdate.Updates.FAList = strArr[1];
                        break;

                    case "Trips":
                        _wBidUpdate.Updates.Trips = int.Parse(strArr[1]);
                        break;

                    case "Version":
                        _wBidUpdate.Updates.Version = strArr[1];
                        break;

                    case "Cities":
                        _wBidUpdate.Updates.Cities = int.Parse(strArr[1]);
                        break;

                    case "Domiciles":
                        _wBidUpdate.Updates.Domiciles = int.Parse(strArr[1]);
                        break;

                    case "Equipment":
                        _wBidUpdate.Updates.Equipment = int.Parse(strArr[1]);
                        break;

                    case "EquipTypes":
                        _wBidUpdate.Updates.EquipTypes = int.Parse(strArr[1]);
                        break;

                    case "News":
                        _wBidUpdate.Updates.News = int.Parse(strArr[1]);
                        break;
                    case "SplitCities":
                        _wBidUpdate.Updates.SplitPointCities = int.Parse(strArr[1]);
                        break;
                }


            }

        }

        /// <summary>
        /// PURPOSE : Parse Cities List
        /// </summary>
        private void ParseCitiesList()
        {
            string[] strArr;
            _wBidUpdate.Cities = new List<City>();
            bool dLST = false;
            bool international = false;
            bool nonConus = false;

            foreach (string str in _cities)
            {
                strArr = str.Split(',');
                dLST = false;
                international = false;
                nonConus = false;
                bool hawai = false;

                if (strArr.Length > 2)
                {
                    dLST = (strArr[2].Trim() != "F");
                }
                if (strArr.Length > 3)
                {


                    if (strArr[3].Trim() == "N")
                    {
                        nonConus = true;
                    }
                    else if (strArr[3].Trim() == "I")
                    {
                        international = true;
                    }
                }
                if (strArr.Length > 4)
                {
                    if (strArr[4].Trim() == "H")
                    {
                        hawai = true;
                    }
                }
                _wBidUpdate.Cities.Add(new City()
                {
                    Id = int.Parse(strArr[0].Split('=')[0]),
                    Name = strArr[0].Split('=')[1],
                    Code = int.Parse(strArr[1]),
                    International = international,
                    NonConus = nonConus,
                    Hawai=hawai
                   // DLST = dLST
                });

            }

        }

        /// <summary>
        /// PURPOSE : Parse Hotels List
        /// </summary>
        private void ParseHotelsList()
        {

            bool isFirsrstLine = true;
            string[] strArr;
            _wBidUpdate.Hotels = new Hotels();
            _wBidUpdate.Hotels.HotelList = new List<Hotel>();
            foreach (string str in _hotels)
            {

                strArr = str.Split('=');
                //Read effective date
                if (isFirsrstLine)
                {
                    isFirsrstLine = false;
                    _wBidUpdate.Hotels.Effective = strArr[1];
                    continue;
                }

                _wBidUpdate.Hotels.HotelList.Add(new Hotel()
                {
                    City = strArr[0],
                    Hotels = strArr[1]
                });

            }

        }

        /// <summary>
        /// PURPOSE : Parse Domicile List
        /// </summary>
        private void ParseDomicileList()
        {
            string[] strArr;

            _wBidUpdate.Domiciles = new List<Domicile>();
            foreach (string str in _domiciles)
            {
                strArr = str.Split(',');

                _wBidUpdate.Domiciles.Add(new Domicile()

                {
                    DomicileId = int.Parse(strArr[0].Split('=')[0]),
                    DomicileName = strArr[0].Split('=')[1],
                    Number = int.Parse(strArr[1]),
                    Code = strArr[2]
                });

            }

        }

        /// <summary>
        /// PURPOSE : Parse Equipment
        /// </summary>
        private void ParseEquipment()
        {

            string[] strArr;
            _wBidUpdate.Equipments = new List<Equipment>();

            foreach (string str in _equipment)
            {
                strArr = str.Split(',');
                _wBidUpdate.Equipments.Add(new Equipment()

                {
                    EquipmentId = int.Parse(strArr[0].Split('=')[0]),
                    EquipmentNumber = int.Parse(strArr[0].Split('=')[1]),
                    EquipmentName = strArr[1]
                });

            }

        }

        /// <summary>
        /// PURPOSE : Parse Equipment
        /// </summary>
        private void ParseEquipmentTypes()
        {

            string[] strArr;
            _wBidUpdate.EquipTypes = new List<EquipType>();

            foreach (string str in _equipmentType)
            {
                strArr = str.Split('=');
                _wBidUpdate.EquipTypes.Add(new EquipType()

                {
                    Id = int.Parse(strArr[0]),
                    Name = strArr[1]

                });

            }

        }


        /// <summary>
        /// PURPOSE : Parse Split point cities
        /// </summary>
        private void ParseSplitPointCities()
        {
            string[] strArr;
            _wBidUpdate.SplitPointCities = new SplitPointCities();

            foreach (string str in _splitpointcities)
            {
                strArr = str.Split('=');
                _wBidUpdate.SplitPointCities.Add(new SplitPointCity()

                {
                    Domicile = strArr[0],
                    Cities = strArr[1].Split(',').ToList()

                });

            }
        }


        #endregion
    }
}
