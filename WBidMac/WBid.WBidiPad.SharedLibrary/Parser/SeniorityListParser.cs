#region NameSpace
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Model;
//using WBid.WBidiPad.SharedLibrary.Model; 
#endregion

namespace WBid.WBidiPad.SharedLibrary.Parser
{
    public class SeniorityListParser
    {

        #region Property
        public List<SeniorityListMember> SenListMembers { get; set; }
        private int _month;
        #endregion

        #region Public Methods

        /// <summary>
        /// PURPOSe :Parse Seniority List For First Round Pilot
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public List<SeniorityListMember> ParseSeniorityListForFirstRoundPilot(string filePath, string position, int year, int month)
        {
            SenListMembers = new List<SeniorityListMember>();
            _month = month;
            string line = string.Empty;
            List<string> pagewiseLeftSideData = new List<string>();
            List<string> pagewiseRightSideData = new List<string>();
            List<string> fullSeniorityData = new List<string>();

            using (StreamReader reader = new StreamReader(filePath))
            {

                bool isRead = false;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Trim() == string.Empty || line.Trim() == "\n" || (line.Length > 64 && line.Substring(60, 4) == "Page"))
                        continue;

                    if (line.Contains("S O U T H W E S T   A I R L I N E S   SEN I O R I T Y   B I D   L I S T"))
                    {
                        isRead = false;
                        count = 0;
                        fullSeniorityData.AddRange(pagewiseLeftSideData);
                        fullSeniorityData.AddRange(pagewiseRightSideData);
                        pagewiseLeftSideData = new List<string>();
                        pagewiseRightSideData = new List<string>();
                        continue;
                    }

                    if (line.Substring(0, 5) == "-----")
                    {
                        if (count > 0)
                        {
                            isRead = true;
                            continue;
                        }
                    }

                    count++;

                    if (isRead)
                    {
                        //Two Column Parsing
                        if (line.Length > 42)
                        {
                            pagewiseLeftSideData.Add(line.Substring(0, 39));
                            pagewiseRightSideData.Add(line.Substring(40, 38));
                        }
                        //Single Column Parsing --Removing extra space
                        else
                        {
                            pagewiseLeftSideData.Add(line.Substring(0, 4) + line.Substring(6, line.Length - 7));
                        }
                    }
                }

            }

            //Adding last readed strings to main list
            //----------------------------------------
            if (pagewiseLeftSideData.Count > 0)
            {
                fullSeniorityData.AddRange(pagewiseLeftSideData);
            }
            if (pagewiseRightSideData.Count > 0)
            {
                fullSeniorityData.AddRange(pagewiseRightSideData);
            }
            //----------------------------------------

            //Parsing seniority strings and map to the corresponding objects
            ParseSeniorityData(fullSeniorityData, position, year);


            return SenListMembers;
        }

        /// <summary>
        /// PURPOSE :Parse Seniority List ForF irstRound FA
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <param name="position">Position CP/FA/FO</param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<SeniorityListMember> ParseSeniorityListForFirstRoundFA(string filePath, string position, int year, int month)
        {

            string line = string.Empty;
            _month = month;
            SenListMembers = new List<SeniorityListMember>();
            List<string> userDetails = new List<string>();
            string previousLine = string.Empty;
            //int lineNumber = 1;
            bool isReadyToRead = false;
            //Reading the text file.
            using (StreamReader reader = new StreamReader(filePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    //we dont need to consider first four lines
                    //if (lineNumber > 4)
                    // {    //checking first 6 letter having any sequence number

                    if (!isReadyToRead && line.Replace("\t", " ").TrimStart() != string.Empty)
                    {
                        isReadyToRead = (line.Replace("\t", " ").TrimStart().Substring(0, 1) == "1");
                    }


                    if (isReadyToRead)
                    {
                        if (line.Substring(0, 6).Trim() != string.Empty)
                        {
                            if (previousLine != string.Empty)
                            {
                                userDetails.Add(previousLine);
                                previousLine = string.Empty;

                            }
                        }
                        if (previousLine != string.Empty) previousLine += " ";

                      //  previousLine += line.Substring(9, line.Length - 9).Trim();
                        previousLine += line.Trim();
                    }

                    // }

                    //lineNumber++;
                }
            }

            if (!userDetails.Contains(previousLine))
            {
                if (previousLine != string.Empty)
                {
                    userDetails.Add(previousLine);
                    previousLine = string.Empty;

                }
            }
            if (userDetails.Count != 0)
            {
                foreach (string singleUser in userDetails)
                {
                    SeniorityListMember seniorityitem = AddSingleUser(singleUser, year);
                    if (seniorityitem.EmpNum != null)
                    {
                        SenListMembers.Add(seniorityitem);
                    }
                }
            }

            return SenListMembers;
           

        }



        public List<SeniorityListMember> ParseSeniorityListForSecondRoundPilot(string filePath, string position, int year, int month)
        {
            string line = string.Empty;
            _month = month;
            SenListMembers = new List<SeniorityListMember>();
            List<string> userDetails = new List<string>();
            string previousLine = string.Empty;
            //int lineNumber = 1;
            bool isReadyToRead = false;
            //Reading the text file.
            using (StreamReader reader = new StreamReader(filePath))
            {
                try
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Replace("\t", " ").TrimStart() != string.Empty)
                        {

                            if (!isReadyToRead)
                            {
                                isReadyToRead = (line.Substring(0, 1) == "1");
                            }


                            if (isReadyToRead)
                            {
                                int selectedLine = 0;
                                //checking first 6 letter having any sequence number
                                //if (line.Substring(0, 2).Trim() != string.Empty)
                                if (int.TryParse(line.Substring(0, 2).Trim(), out selectedLine) && line.Trim().Substring(0, 2) != "VA")
                                {
                                    if (previousLine != string.Empty)
                                    {
                                        userDetails.Add(previousLine);
                                        previousLine = string.Empty;

                                    }
                                }

                                if (line.Substring(0, 1) == "*")
                                    break;
                                if (previousLine != string.Empty) previousLine += "\t";

                                //previousLine += line.Substring(2, line.Length - 2).Trim();
                                previousLine += line.Trim();
                            }


                        }

                    }
                    if (previousLine != string.Empty)
                    {
                        userDetails.Add(previousLine);
                        previousLine = string.Empty;
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }


            if (userDetails.Count != 0)
            {
                foreach (string singleUser in userDetails)
                {
                    SenListMembers.Add(AddSingleUserforCPsecondround(singleUser, year));
                }
            }

            return SenListMembers;
        }
		public int ParseSeniorityListForSecondRoundFAForTotalFACount(string filePath)
		{ 
			string line = string.Empty;
			SenListMembers = new List<SeniorityListMember>();

			int totalFAcount = 0;
			bool isReadyToRead = false;
			//Reading the text file.
			using (StreamReader reader = new StreamReader(filePath))
			{
				try
				{
					while ((line = reader.ReadLine()) != null)
					{
						if (line.StartsWith("1-"))
						{
							isReadyToRead = true;

						}
						if (isReadyToRead && line!=string.Empty && line.Contains('['))
						{
							totalFAcount++;
						}
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			return totalFAcount;

		}
        #endregion

        #region Private Methods
        private void ParseSeniorityData(List<string> fullSeniorityData, string position, int year)
        {
            SeniorityListMember seniorityListMember = null;
            foreach (string seniorityData in fullSeniorityData)
            {

                if (seniorityData.Substring(0, 3).Trim() != string.Empty)
                {
                    //Adding previously parsed member details to list
                    if (seniorityListMember != null && !string.IsNullOrEmpty(seniorityListMember.EmpNum))
                    {
                        SenListMembers.Add(seniorityListMember);
                    }

                    seniorityListMember = new SeniorityListMember();
                    seniorityListMember.DomicileSeniority = int.Parse(seniorityData.Substring(0, 3).Trim());
                    seniorityListMember.EmpNum = seniorityData.Substring(32, 6).PadLeft(6, '0');
                    seniorityListMember.Position = position;

                    if (seniorityData.Substring(17, 13).Trim() != string.Empty)
                    {
                        string abscenceDetail = seniorityData.Substring(17, 13);
                        seniorityListMember.Absences = new List<Absense>();
                       
                        string firstPart = abscenceDetail.Split('-')[0].Trim();
                        //Remove the first two Characters
                        firstPart = firstPart.Substring(2, firstPart.Length - 2).Trim();
                        string secondPart = abscenceDetail.Split('-')[1].Trim();

                        Absense absense = new Absense()
                        {
                            AbsenceString = abscenceDetail,
                            AbsenceType = abscenceDetail.Substring(0, 2),
                            StartAbsenceDate = new DateTime(year, int.Parse(firstPart.Split('/')[0]), int.Parse(firstPart.Split('/')[1])),
                            EndAbsenceDate = new DateTime(year, int.Parse(secondPart.Split('/')[0]), int.Parse(secondPart.Split('/')[1])),
                        };

                        if (absense.StartAbsenceDate > absense.EndAbsenceDate)
                        {
                            if (_month == 12)
                            {
                                absense.EndAbsenceDate = absense.EndAbsenceDate.AddYears(1);
                            }
                            else if (_month == 1)
                            {
                                absense.StartAbsenceDate = absense.StartAbsenceDate.AddYears(-1);
                            }
                        }

                        seniorityListMember.Absences.Add(absense);
                    }
                }
                else
                {

                    if (seniorityData.Substring(17, 13).Trim() != string.Empty)
                    {
                        string abscenceDetail = seniorityData.Substring(17, 13);
                       
                        string firstPart = abscenceDetail.Split('-')[0].Trim();
                        //Remove the first two Characters
                        firstPart = firstPart.Substring(2, firstPart.Length - 2).Trim();
                        string secondPart = abscenceDetail.Split('-')[1].Trim();
                        Absense absense = new Absense()
                        {
                            AbsenceString = abscenceDetail,
                            AbsenceType = abscenceDetail.Substring(0, 2),
                            StartAbsenceDate = new DateTime(year, int.Parse(firstPart.Split('/')[0]), int.Parse(firstPart.Split('/')[1])),
                            EndAbsenceDate = new DateTime(year, int.Parse(secondPart.Split('/')[0]), int.Parse(secondPart.Split('/')[1])),
                        };

                        if (absense.StartAbsenceDate > absense.EndAbsenceDate)
                        {
                            if (_month == 12)
                            {
                                absense.EndAbsenceDate = absense.EndAbsenceDate.AddYears(1);
                            }
                            else if (_month == 1)
                            {
                                absense.StartAbsenceDate = absense.StartAbsenceDate.AddYears(-1);
                            }
                        }

                        seniorityListMember.Absences.Add(absense);
                    }
                }
            }
            SenListMembers.Add(seniorityListMember);
        }


        private SeniorityListMember AddSingleUser(string singleUser, int year)
        {
            SeniorityListMember seniorityListMember = new SeniorityListMember();

            //Some employee number doesn't having space.
            singleUser = singleUser.Replace("(", " (");
            string[] split = singleUser.Split(' ');
            string[] splitVacation;
            string[] splitDates;
            split = split.Where(x => x != string.Empty).ToArray();
            try
            {
                seniorityListMember.DomicileSeniority = Convert.ToInt32(split[0]);
            }
            catch (Exception ex)
            {
               // throw ex;
            }
            foreach (var item in split)
            {

                if (item.Trim() == string.Empty) continue;

                else if (item.StartsWith("("))
                {
                    seniorityListMember.EmpNum = item.Replace("(", "").Replace(")", "").PadLeft(6, '0');
                }
                else if (item.StartsWith("VAC") && item.Contains('-'))
                {
                    splitVacation = item.Replace("VAC", "").Split(';');
                    seniorityListMember.Absences = new List<Absense>();

                    foreach (var vacation in splitVacation)
                    {
                        splitDates = vacation.Split('-');
                        if (splitDates.Count() == 2)
                        {
                            Absense absense = new Absense()
                            {
                                AbsenceType = "VA",
                                StartAbsenceDate = new DateTime(year, int.Parse(splitDates[0].Split('/')[0]), int.Parse(splitDates[0].Split('/')[1])),
                                EndAbsenceDate = new DateTime(year, int.Parse(splitDates[1].Split('/')[0]), int.Parse(splitDates[1].Split('/')[1]))

                            };
                            if (absense.StartAbsenceDate > absense.EndAbsenceDate)
                            {
                                if (_month == 12)
                                {
                                    absense.EndAbsenceDate = absense.EndAbsenceDate.AddYears(1);
                                }
                                else if (_month == 1)
                                {
                                    absense.StartAbsenceDate = absense.StartAbsenceDate.AddYears(-1);
                                }
                            }
                            seniorityListMember.Absences.Add(absense);
                        }


                    }

                }

            }

            return seniorityListMember;
        }


        private SeniorityListMember AddSingleUserforCPsecondround(string singleUser, int year)
        {
            try
            {

                SeniorityListMember seniorityListMember = new SeniorityListMember();
                seniorityListMember.Absences = new List<Absense>();
                List<string> splitstring = singleUser.Replace("\t", "  ").Split(new string[] { "  " }, StringSplitOptions.None).ToList();
                splitstring = splitstring.Where(x => x != string.Empty).ToList();
                seniorityListMember.DomicileSeniority = Convert.ToInt32(splitstring[0]);
                seniorityListMember.EmpNum = splitstring[1].PadLeft(6, '0');
                splitstring.RemoveAt(0);
                splitstring.RemoveAt(0);
                List<string> absences = splitstring.Where(x => x.Trim().StartsWith("VA ")).ToList();
                foreach (string item in absences)
                {
                    var absenceitem = item.TrimStart();
                    if (absenceitem.Length == 14)
                    {

                        Absense absence = new Absense();
                        absence.AbsenceType = "VA";
                        absence.StartAbsenceDate = new DateTime(year, GetMonthinDigit(absenceitem.Substring(5, 3)), Convert.ToInt32(absenceitem.Substring(3, 2)));
                        absence.EndAbsenceDate = new DateTime(year, GetMonthinDigit(absenceitem.Substring(11, 3)), Convert.ToInt32(absenceitem.Substring(9, 2)));

                        if (absence.StartAbsenceDate > absence.EndAbsenceDate)
                        {
                            if (_month == 12)
                            {
                                absence.EndAbsenceDate = absence.EndAbsenceDate.AddYears(1);
                            }
                            else if (_month == 1)
                            {
                                absence.StartAbsenceDate = absence.StartAbsenceDate.AddYears(-1);
                            }
                        }
                        seniorityListMember.Absences.Add(absence);

                    }


                }
                return seniorityListMember;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private int GetMonthinDigit(string month)
        {
            return DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month;
        }
        #endregion
    }
}
