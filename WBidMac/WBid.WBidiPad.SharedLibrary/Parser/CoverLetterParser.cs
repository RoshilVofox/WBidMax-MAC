using System;
using System.IO;
using System.Globalization;
using System.Linq;

namespace WBid.WBidiPad.SharedLibrary
{
	public class CoverLetterParser
	{
		public CoverLetterData ParseCoverLetteForPilots(string file,string domcile,string position)
		{
			position = (position == "CP") ? "CA" : position;
			string line = string.Empty;
			CoverLetterData coverletterData = new CoverLetterData();
			bool isReadytoRead = false;

			using (StreamReader reader = new StreamReader(file))
			{
				try
				{
					while ((line = reader.ReadLine()) != null)
					{
						if (line.StartsWith("RE:") && isReadytoRead==false)
						{
							string[] dateinfo= line.Split(' ');
							coverletterData.Month=  DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(dateinfo[4]) + 1;
							coverletterData.Year = Convert.ToInt32(dateinfo[5]);

						}
						if (isReadytoRead)
						{
							string[] linedata= line.Split('\t');
                            string[] linedatasplt = line.Split('-');
							if (linedata.Count() > 3)
							{
								if (linedata[0] == domcile && linedata[1] == position)
								{
									coverletterData.Base = domcile;
									string pos = (linedata[1] == "CA") ? "CP" : linedata[1];
									coverletterData.Position = pos;
                                    if (linedata[2] != string.Empty)
                                        coverletterData.TotalLine = Convert.ToInt32(linedata[2]);
                                    else
                                        coverletterData.TotalLine = Convert.ToInt32(linedatasplt[linedatasplt.Count() - 1].Substring(0, 4));
									return coverletterData;
								}
							}
						}
						//if(line=="BASE\t*\tTOTAL\tHARD\tRESERVE\t*         BLANK")
						string lineswithoutspace = line.Replace("\t", "").Replace(" ", "").Replace("*", "");
						if(lineswithoutspace.Contains("BASETOTALHARDRESERVE"))
						{
							isReadytoRead=true;
						}

					}
				}
				catch (Exception ex)
				{
					throw;
				}
			}
			return coverletterData;
		}
		public CoverLetterData ParseCoverLetteForFlightAttendants(string file, string domcile)
		{

			string line = string.Empty;
			CoverLetterData coverletterData = new CoverLetterData();
			int count = 0;
			int totallines = 0;
			using (StreamReader reader = new StreamReader(file))
			{
				try
				{
					while ((line = reader.ReadLine()) != null)
					{
						if (count == 0)
						{
							string[] dateinfo = line.Split(' ');

							coverletterData.Month = DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().ConvertAll(x => x.ToLower()).IndexOf(dateinfo[dateinfo.Count() - 3].ToLower()) + 1;
							coverletterData.Year = Convert.ToInt32(dateinfo[dateinfo.Count() - 2]);
							count++;
						}
						else
						{
							if (line.Replace(" ", "").ToLower().Contains("numberofa-b-cposition"))
							{
								//totallines = Convert.ToInt32(line.Split('\t')[3].Trim());
								totallines = Convert.ToInt32(line.Split('(')[0].Split(':')[1]);
							}
							if (line.Replace(" ", "").ToLower().Contains("numberofb-cposition"))
							{
								//totallines = totallines + Convert.ToInt32(line.Split('\t')[4].Trim());
								totallines = totallines + Convert.ToInt32(line.Split('(')[0].Split(':')[1]);
							}
							if (line.Replace(" ", "").ToLower().Contains("numberofa-b-c-dposition"))
							{
								//totallines = totallines + Convert.ToInt32(line.Split('\t')[3].Trim());
								totallines = totallines +Convert.ToInt32(line.Split('(')[0].Split(':')[1]);
							}
							if (line.Replace(" ", "").ToLower().Contains("numberofd-onlyposition"))
							{
								//totallines=totallines +  Convert.ToInt32(line.Split('\t')[3].Trim());
								totallines = totallines +Convert.ToInt32(line.Split('(')[0].Split(':')[1]);
								coverletterData.Base = domcile;
								coverletterData.Position = "FA";
								coverletterData.TotalLine = totallines;
								return coverletterData;
							}
						}

					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			return coverletterData;
		}

	}
	public class CoverLetterData
	{
		public int Month { get; set; }
		public int Year { get; set; }
		public string Base { get; set; }
		public string Position { get; set; }
		public int TotalLine { get; set; }
	}
}
