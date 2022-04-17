using System;
using WBid.WBidiPad.Model;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace WBid.WBidiPad.SharedLibrary
{
	public class ModifiedSeniorityListParser
	{
        SenListFormat senlistformat;
		public ModifiedSeniorityListParser()
		{

		}
		#region Property
		public List<SeniorityListMember> SenListMembers { get; set; }
		private int _month;
		#endregion

		/// <summary>
		/// PURPOSe :Parse Seniority List For First Round Pilot
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="position"></param>
		/// <returns></returns>
  //      public List<SeniorityListMember> ParseSeniorityListForPilot(string filePath, string position, int year, int month, string round,SenListFormat senlist)
		//{
		//	SenListMembers = new List<SeniorityListMember>();
  //          senlistformat = senlist;
		//	if (round == "M")
		//	{
  //              //senlistformat = senlist;
		//		SenListMembers = ParseSeniorityListForPilotForFirstRound(filePath, position, year, month);
		//	}
		//	else
		//	{
		//		SenListMembers = ParseSeniorityListForPilotForSecondRound(filePath, position, year, month);
		//	}
		//	return SenListMembers;

		//}
        public List<SeniorityListMember> ParseSeniorityListForPilot(string filePath, string position, int year, int month, string round, SenListFormat senlist)
        {
            SenListMembers = new List<SeniorityListMember>();
            senlistformat = senlist;
            SenListMembers= ParseSeniorityListForPilotusingDBValues(filePath, position, year, month);
            return SenListMembers;
        }

        public List<SeniorityListMember> ParseSeniorityListForPilotusingDBValues(string filePath, string position, int year, int month)
        {
            try
            {
                SenListMembers = new List<SeniorityListMember>();
                _month = month;
                bool isReadyToRead = false;
                List<string> fullSeniorityData = new List<string>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Trim() == string.Empty || line.Trim() == "\n" || (line.Length > 64 && line.Substring(60, 4) == "Page"))
                            continue;

                        if (line.Contains("SQ#"))
                        {
                            isReadyToRead = true;
                            continue;
                        }
                        if (isReadyToRead && line.Substring(0, 1) == "*")
                            break;
                        if (isReadyToRead)
                        {
                            fullSeniorityData.Add(line);
                        }
                    }
                }
                SeniorityListMember senlistmember = new SeniorityListMember();
                SenListMembers = new List<SeniorityListMember>();
                int count = 1;
                int employeenumberlength = (senlistformat.EmpIdEnd - senlistformat.EmpIdSt + 1);
                int ebglength = senlistformat.EbgEnd - senlistformat.EbgSt + 1;
                int bidtypelength = senlistformat.BidTypeEnd - senlistformat.BidTypeEnd + 1;

                foreach (var line in fullSeniorityData)
                {

                    if (line.Substring(0, 6).Trim() != string.Empty)
                    {
                        if (senlistmember.EmpNum != null)
                        {
                            SenListMembers.Add(senlistmember);
                        }
                        senlistmember = new SeniorityListMember();
                        senlistmember.Absences = new List<Absense>();
                        senlistmember.DomicileSeniority = Convert.ToInt32(line.Substring(senlistformat.SeqNumSt - 1, senlistformat.SeqNumEnd - 1).TrimEnd());
                        senlistmember.EmpNum = line.Substring(senlistformat.EmpIdSt-1, employeenumberlength).Trim().PadLeft(employeenumberlength, '0');

                        senlistmember.EBG = line.Substring(senlistformat.EbgSt - 1, ebglength).Trim();
                        senlistmember.BidType = line.Substring(senlistformat.BidTypeSt - 1, bidtypelength).Trim();
                        ParseVacationData(year, fullSeniorityData, senlistmember, count, line,month);
                    }
                    else
                    {
                        ParseVacationData(year, fullSeniorityData, senlistmember, count, line,month);
                    }
                    count++;
                }
                if (SenListMembers.Count == 0)
                    throw new Exception();
                return SenListMembers;

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }
        private void ParseVacationData(int year, List<string> fullSeniorityData, SeniorityListMember senlistmember, int count, string line,int month)
        {
            if(line.Length > senlistformat.AbsenceDatesSt)
            {
                string vacationType = line.Substring(senlistformat.AbscenceTypeSt - 1, (senlistformat.AbscenceTypeEnd - senlistformat.AbscenceTypeSt + 1));
				var cfvChar = line.Substring(senlistformat.AbscenceTypeSt - 2, 1);
				var cfvnextChar = line.Substring(senlistformat.AbscenceTypeSt + 1, 1);
				vacationType = ((vacationType == "FV" && cfvChar == "C")) ? "CFV" : vacationType;
				if ((vacationType == "CF" && cfvnextChar == "V"))
				{
					//This condition included becuase the seniority file contains space issue for  multiple CFV
					Absense absence = new Absense();
					absence.AbsenceType = "CFV";
					absence.StartAbsenceDate = new DateTime(year, GetMonthinDigit(line.Substring(senlistformat.AbsenceDatesSt + 2, 3)), Convert.ToInt32(line.Substring(senlistformat.AbsenceDatesSt - 0, 2)));
					absence.EndAbsenceDate = new DateTime(year, GetMonthinDigit(line.Substring(senlistformat.AbsenceDatesSt + 8, 3)), Convert.ToInt32(line.Substring(senlistformat.AbsenceDatesSt + 6, 2)));
					if (month == 12)
					{
						absence.EndAbsenceDate = (absence.StartAbsenceDate > absence.EndAbsenceDate) ? absence.EndAbsenceDate.AddMonths(12) : absence.EndAbsenceDate;
					}
					else if (month == 1)
					{
						absence.StartAbsenceDate = (absence.StartAbsenceDate > absence.EndAbsenceDate) ? absence.StartAbsenceDate.AddMonths(-12) : absence.StartAbsenceDate;
					}
					senlistmember.Absences.Add(absence);
				}
				else
				{
					if (vacationType == "VA" || vacationType == "FV" || vacationType == "CFV")
					{
						Absense absence = new Absense();
						absence.AbsenceType = vacationType;
						absence.StartAbsenceDate = new DateTime(year, GetMonthinDigit(line.Substring(senlistformat.AbsenceDatesSt + 1, 3)), Convert.ToInt32(line.Substring(senlistformat.AbsenceDatesSt - 1, 2)));
						absence.EndAbsenceDate = new DateTime(year, GetMonthinDigit(line.Substring(senlistformat.AbsenceDatesSt + 7, 3)), Convert.ToInt32(line.Substring(senlistformat.AbsenceDatesSt + 5, 2)));
						//absence.EndAbsenceDate = (absence.StartAbsenceDate > absence.EndAbsenceDate) ? absence.EndAbsenceDate.AddMonths(12) : absence.EndAbsenceDate;
						if (month == 12)
						{
							absence.EndAbsenceDate = (absence.StartAbsenceDate > absence.EndAbsenceDate) ? absence.EndAbsenceDate.AddMonths(12) : absence.EndAbsenceDate;
						}
						else if (month == 1)
						{
							absence.StartAbsenceDate = (absence.StartAbsenceDate > absence.EndAbsenceDate) ? absence.StartAbsenceDate.AddMonths(-12) : absence.StartAbsenceDate;
						}

						senlistmember.Absences.Add(absence);
					}
				}
                //check the line reached at the end of the document.
                if (count == fullSeniorityData.Count)
                {
                    SenListMembers.Add(senlistmember);
                }
            }
            else
            {
                if (count == fullSeniorityData.Count)
                {
                    SenListMembers.Add(senlistmember);
                }
            }
        }


		public List<SeniorityListMember> ParseSeniorityListForPilotForFirstRound(string filePath, string position, int year, int month)
		{


			try
			{
				SenListMembers = new List<SeniorityListMember>();
				_month = month;
				string line = string.Empty;
				List<string> userDetails = new List<string>();
				string previousLine = string.Empty;
				bool isReadyToRead = false;
				List<string> fullSeniorityData = new List<string>();

				using (StreamReader reader = new StreamReader(filePath))
				{

					while ((line = reader.ReadLine()) != null)
					{
						if (line.Trim() == string.Empty || line.Trim() == "\n" || (line.Length > 64 && line.Substring(60, 4) == "Page"))
							continue;

						if (line.Contains("SQ#"))
						{
							isReadyToRead = true;
							continue;
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
							if (line.Substring(0, 1) == "*")
								break;
							if (previousLine != string.Empty) previousLine += " ";


							previousLine += line;
						}
					}
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
						SeniorityListMember seniorityitem = AddSingleUserForFirstRound(singleUser, year);
						if (seniorityitem.EmpNum != null)
						{
							SenListMembers.Add(seniorityitem);
						}
					}
				}
				if (SenListMembers.Count == 0)
					throw new Exception();
				return SenListMembers;

			}
			catch (Exception ex)
			{
				throw ex;

			}

		}

		public List<SeniorityListMember> ParseSeniorityListForPilotForSecondRound(string filePath, string position, int year, int month)
		{


			try
			{
				SenListMembers = new List<SeniorityListMember>();
				_month = month;
				string line = string.Empty;
				List<string> userDetails = new List<string>();
				string previousLine = string.Empty;
				bool isReadyToRead = false;
				List<string> fullSeniorityData = new List<string>();

				using (StreamReader reader = new StreamReader(filePath))
				{

					while ((line = reader.ReadLine()) != null)
					{
						if (line.Trim() == string.Empty || line.Trim() == "\n" || (line.Length > 64 && line.Substring(60, 4) == "Page"))
							continue;

						if (line.Contains("SQ#"))
						{
							isReadyToRead = true;
							continue;
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
							if (line.Substring(0, 1) == "*")
								break;
							if (previousLine != string.Empty) previousLine += " ";


							previousLine += line;
						}
					}
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
						SeniorityListMember seniorityitem = AddSingleUserForSecondRound(singleUser, year);
						if (seniorityitem.EmpNum != null)
						{
							SenListMembers.Add(seniorityitem);
						}
					}
				}
				if (SenListMembers.Count == 0)
					throw new Exception();
				return SenListMembers;

			}
			catch (Exception ex)
			{
				throw ex;

			}

		}

		#region Private Methods

		private SeniorityListMember AddSingleUserForFirstRound(string singleUser, int year)
		{
			try
			{


				SeniorityListMember seniorityListMember = new SeniorityListMember();
				seniorityListMember.Absences = new List<Absense>();

				seniorityListMember.DomicileSeniority = Convert.ToInt32(singleUser.Substring(0, 3).TrimEnd());
				//seniorityListMember.EmpNum = singleUser.Substring(20, 6).Trim().PadLeft(6, '0');
				//format changes on 3/3/2018
				seniorityListMember.EmpNum = singleUser.Substring(27, 6).Trim().PadLeft(6, '0');
				List<string> absences = new List<string>();
				//if (singleUser.Length > 60)
				if (singleUser.Length > 67)
				{
					//List<string> splitstring = singleUser.Substring(61, singleUser.Length - 61).Split(new string[] { "  " }, StringSplitOptions.None).ToList();
					List<string> splitstring = singleUser.Substring(68, singleUser.Length - 68).Split(new string[] { "  " }, StringSplitOptions.None).ToList();
					absences = splitstring.Where(x => x.Trim().StartsWith("VA ")).ToList();
				}
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

		private SeniorityListMember AddSingleUserForSecondRound(string singleUser, int year)
		{
			try
			{


				SeniorityListMember seniorityListMember = new SeniorityListMember();
				seniorityListMember.Absences = new List<Absense>();

				seniorityListMember.DomicileSeniority = Convert.ToInt32(singleUser.Substring(0, 3).TrimEnd());
				seniorityListMember.EmpNum = singleUser.Substring(20, 6).Trim().PadLeft(6, '0');

				List<string> absences = new List<string>();
				if (singleUser.Length > 60)
				{
					List<string> splitstring = singleUser.Substring(61, singleUser.Length - 61).Split(new string[] { "  " }, StringSplitOptions.None).ToList();
					absences = splitstring.Where(x => x.Trim().StartsWith("VA ")).ToList();
				}
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
