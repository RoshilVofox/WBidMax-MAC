using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using AppKit;
using System.IO;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Runtime.Serialization.Formatters.Binary;
//using WBid.WBidiPad.iOS.Common;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Parser;
using System.ServiceModel;
using WBid.WBidMac.Mac;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.PortableLibrary;
using System.Globalization;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidiPad.iOS.Utility
{
	public class WBidHelper
	{
		/// <summary>
		/// PURPOSE : Get App Data path
		/// </summary>
		/// <returns></returns>
		public static string GetAppDataPath()
		{
			return NSSearchPath.GetDirectories(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User, true).First() + "/WBidMax";
		}

		/// <summary>
		/// PURPOSE : Get the path of the INI file
		/// </summary>
		/// <returns></returns>
		public static string GetWBidINIFilePath()
		{
			//Console.WriteLine (GetAppDataPath ());
			return GetAppDataPath() + "/WBidINI.XML";
		}

		public static string GetWBidDWCFilePath()
		{
			return GetAppDataPath() + "/WBidDWC.XML";
		}

		public static string GetQuickSetFilePath()
		{
			return GetAppDataPath() + "/WBidQuicksets.qs";
		}

		public static string WBidUserFilePath
		{
			get
			{
				return GetAppDataPath() + "/User.xml";
			}
		}
		public static string WBidStateFilePath
		{
			get
			{
				return WBidHelper.GetAppDataPath() + "/" + GenerateFileNameUsingCurrentBidDetails() + ".WBS";
			}
		}
		public static string WBidUpdateFilePath
		{
			get
			{
				return WBidHelper.GetAppDataPath() + "/WBUPDATE.DAT";
			}
		}
		public static string MILFilePath
		{
			get
			{
				return GetAppDataPath() + "/" + GenerateFileNameUsingCurrentBidDetails() + ".MIL";
			}
		}

		public static string HistoricalFilesInfoPath
		{
			get
			{
				return GetAppDataPath() + "/History" + DateTime.Now.Month.ToString().PadLeft(2, '0') + ".HST";
			}
		}
		public static string WBidCommuteFilePath
		{
			get
			{
				return WBidHelper.GetAppDataPath() + "\\" + GetCommuteDifferenceFileName();
			}
		}
		public static string GetCommuteDifferenceFileName()
		{

			//BWICP0122M
			var fileName = (GlobalSettings.CurrentBidDetails == null) ? string.Empty : GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + GlobalSettings.CurrentBidDetails.Month.ToString("d2") + (GlobalSettings.CurrentBidDetails.Year - 2000).ToString() + GlobalSettings.CurrentBidDetails.Round + "Cmt.COM";
			return fileName;
		}


		/// <summary>
		/// Get the "Column defenition" path 
		/// </summary>
		/// <returns></returns>
		public static string GetWBidColumnDefinitionFilePath()
		{
			//return GetAppDataPath() + "/ColumnDefinitions.xml";
			return NSBundle.MainBundle.ResourcePath + "/ColumnDefinitions.xml";
		}
		/// <summary>
		/// create the directory to store the data
		/// </summary>
		public static void CreateAppDataDirectory()
		{
			var documents = NSSearchPath.GetDirectories(NSSearchPathDirectory.ApplicationSupportDirectory, NSSearchPathDomain.User, true).First();
			var directoryname = Path.Combine(documents, "WBidMax");
			Directory.CreateDirectory(directoryname);
		}

		/// <summary>
		/// Serialize the parsed trip file and saved it into the root folder.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="objectToSerialize"></param>
		public static void SerializeObject(string filename, Object objectToSerialize)
		{
			MemoryStream memStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memStream, objectToSerialize);
			byte[] byteArray = memStream.ToArray();
			FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write);
			fileStream.Write(byteArray.ToArray(), 0, byteArray.Length);
			fileStream.Close();
			memStream.Close();
			memStream.Dispose();
		}
		/// <summary>
		/// Derialize the file
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static Object DeSerializeObject(string filename)
		{
			object obj = new object();
			Stream stream = File.Open(filename, FileMode.Open);
			BinaryFormatter bFormatter = new BinaryFormatter();
			obj = (object)bFormatter.Deserialize(stream);
			stream.Close();
			return obj;
		}




		/// <summary>
		/// PURPOSE : Set current Bid details
		/// </summary>
		/// <param name="fileName"></param>
		public static void SetCurrentBidInformationfromZipFileName(string fileName, bool isHistorical)
		{
			string domicile = fileName.Substring(2, 3);
			string position = fileName.Substring(0, 1);
			position = (position == "C" ? "CP" : (position == "F" ? "FO" : "FA"));
			int month = int.Parse(fileName.Substring(5, 1), System.Globalization.NumberStyles.HexNumber);
			string round = fileName.Substring(1, 1) == "D" ? "M" : "S";
			int equipment = Convert.ToInt32(fileName.Substring(7, 3));
			int year = 0;
			if (isHistorical)
				year = GlobalSettings.DownloadBidDetails.Year;
			else
				year = GetZipFolderCreationTime(fileName.Replace(".737", ""), month);
			DateTime bpStartDay = CalculateBpStartDayWithYear(position, month, year);
			GlobalSettings.CurrentBidDetails = new BidDetails
			{
				Domicile = domicile,
				Postion = position,
				Round = round,
				Month = month,
				Equipment = equipment,
				BidPeriodStartDate = bpStartDay,
				BidPeriodEndDate = CalculateBPEndDateWithYear(position, month, year),
				Year = bpStartDay.Year,
			};
		}

		public static string GetApplicationBidData()
		{
			try
			{
				string currentbuild = string.Empty;
				string domicile = GlobalSettings.CurrentBidDetails.Domicile ?? string.Empty;
				string position = GlobalSettings.CurrentBidDetails.Postion ?? string.Empty;
				System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
				string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
				string round = GlobalSettings.CurrentBidDetails.Round == "M" ? "Monthly" : "2nd Round";
				currentbuild = domicile + "/" + position + "/" + " " + round + "  Line for " + strMonthName + " " + GlobalSettings.CurrentBidDetails.Year;

				var sb = new StringBuilder();
				if (GlobalSettings.WbidUserContent.UserInformation != null)
				{
					sb.Append("<br/>" + "Base            :" + GlobalSettings.WbidUserContent.UserInformation.Domicile);
					sb.Append("<br/>" + "Seat            :" + GlobalSettings.WbidUserContent.UserInformation.Position);
					sb.Append("<br/>" + "Employee Number :" + GlobalSettings.WbidUserContent.UserInformation.EmpNo);
					sb.Append("<br/>" + "App Email  :" + GlobalSettings.WbidUserContent.UserInformation.Email);
				}
				currentbuild = currentbuild + sb.ToString();
				return currentbuild;


			}
			catch (Exception ex)
			{
				return string.Empty;
			}

		}

		/// <summary>
		/// PURPOSE : Set current Bid details from State fileName,   
		/// </summary>
		/// <param name="fileName"></param>
		public static void SetCurrentBidInformationfromStateFileName(string fileName)
		{

			string domicile = fileName.Substring(0, 3);
			string position = fileName.Substring(3, 2);
			int month = Convert.ToInt32(fileName.Substring(5, 2));
			//string round = fileName.Substring(7, 1);
			//modified the file structure
			string round = fileName.Substring(9, 1);
			string linefilename = domicile + position + month.ToString("d2") + round + "737";
			int year = Convert.ToInt16(fileName.Substring(7, 2)) + 2000;
			DateTime bpStartDay = CalculateBpStartDayWithYear(position, month, year);

			GlobalSettings.CurrentBidDetails = new BidDetails
			{
				Domicile = domicile,
				Postion = position,
				Round = round,
				Month = month,
				BidPeriodStartDate = bpStartDay,
				BidPeriodEndDate = CalculateBPEndDateWithYear(position, month, year),
				Year = bpStartDay.Year,
			};
		}
		public static DateTime CalculateBpStartDayWithYear(string position, int month, int year)
		{
			DateTime startDay;
			if (position == "FA")
			{
				if (month == 2) startDay = new DateTime(year, month - 1, 31);                  // Jan 31
				else if (month == 3) startDay = new DateTime(year, month, 2);                  // Mar 2
				else startDay = new DateTime(year, month, 1);                                  // all other months, start day is the 1st
			}
			else
			{
				startDay = new DateTime(year, month, 1);
			}
			return startDay;
		}

		public static DateTime CalculateBPEndDateWithYear(string position, int month, int year)
		{
			DateTime endDay;
			int numberOfDays = DateTime.DaysInMonth(year, month);

			if (position == "FA")
			{
				if (month == 1) endDay = new DateTime(year, month, 30);
				else if (month == 2) endDay = new DateTime(year, month + 1, 1);                  // Mar 1
				else endDay = new DateTime(year, month, numberOfDays);
			}
			else
			{
				if (month == 1) endDay = new DateTime(year, month, 31);
				else endDay = new DateTime(year, month, numberOfDays);
			}
			return endDay;

		}

		private static int GetlineFileCreationTime(string linefilename, int month)
		{

			string lineFilename = Path.Combine(WBidHelper.GetAppDataPath(), linefilename + ".WBL");
			//get the file created time for the line file 
			DateTime filecreationTime = File.GetCreationTime(lineFilename);

			int year = 2013;
			//if the user donwloads the decemeber month data from the decemeber month ,we have to use the  file created year for the bid year.
			if (filecreationTime.Month == 12 && filecreationTime.Month == month)
				year = filecreationTime.Year;
			else
				//(Since the January month data is downloaded from the December month we need to add one month to the createddatetime of the line file to get the exact bid period year.)
				year = filecreationTime.AddMonths(1).Year;
			return year;
		}
		private static int GetZipFolderCreationTime(string zipfoldername, int month)
		{

			string zipFilename = Path.Combine(WBidHelper.GetAppDataPath(), zipfoldername);
			//get the folder created time for the zip file 
			DateTime foldercreationTime = Directory.GetCreationTime(zipFilename);

			int year = 2013;
			//if the user donwloads the decemeber month data from the decemeber month ,we have to use the  file created year for the bid year.
			if (foldercreationTime.Month == 12 && foldercreationTime.Month == month)
				year = foldercreationTime.Year;
			else
				//(Since the January month data is downloaded from the December month we need to add one month to the createddatetime of the line file to get the exact bid period year.)
				year = foldercreationTime.AddMonths(1).Year;
			return year;
		}

		/// <summary>
		/// PURPOSE : Generate Filename using current bid details
		/// </summary>
		/// <returns></returns>
		public static string GenerateFileNameUsingCurrentBidDetails()
		{
			return (GlobalSettings.CurrentBidDetails == null) ? string.Empty : GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + GlobalSettings.CurrentBidDetails.Month.ToString("d2") + (GlobalSettings.CurrentBidDetails.Year - 2000).ToString() + GlobalSettings.CurrentBidDetails.Round + "737";
		}

		/// <summary>
		/// save the state file to app data folder.
		/// </summary>
		/// <param name="stateFileName"></param>
		public static void SaveStateFile(string stateFileName)
		{
			GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime();
			XmlHelper.SerializeToXml(GlobalSettings.WBidStateCollection, stateFileName);
		}
		/// <summary>
		/// PURPOSE : Save INI File
		/// </summary>
		/// <param name="wBidINI"></param>
		/// <param name="fileName"></param>
		public static bool SaveINIFile(WBidINI wBidINI, string fileName)
		{
			try
			{
				return XmlHelper.SerializeToXml<WBidINI>(wBidINI, fileName);
			}
			catch (Exception)
			{

				throw;
			}

		}

		/// <summary>
		///  Save the user information(recent file) to the user.xml file
		/// </summary>
		/// <param name="wbidUser"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static bool SaveUserFile(WbidUser wbidUser, string fileName)
		{
			try
			{
				return XmlHelper.SerializeToXml<WbidUser>(wbidUser, fileName);
			}
			catch (Exception)
			{

				throw;
			}
		}
		/// <summary>
		/// PURPOSE : Read  file content ferom WBid Updated (WBUPDATE.DAT ) file
		/// </summary>
		/// <param name="Filename"></param>
		/// <returns></returns>
		public static WBidUpdate ReadValuesfromWBUpdateFile(string fileName)
		{

			WBidUpdateParser parser = new WBidUpdateParser();
			return parser.ParseWBidUpdateFile(fileName);


		}
		/// <summary>
		/// Read Coulmn Defenition Data from XML file
		/// </summary>
		/// <param name="Filename"></param>
		public static ColumnDefinitions ReadCoulumnDefenitionData(string Filename)
		{
			//Read Coulmn Defenition Data from XML file
			return XmlHelper.DeserializeFromXml<ColumnDefinitions>(Filename);
		}

		public static string GenarateZipFileName()
		{
			string filename = (GlobalSettings.CurrentBidDetails.Postion == "CP") ? "C" : (GlobalSettings.CurrentBidDetails.Postion == "FO") ? "F" : "A";
			filename += (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B";
			filename += GlobalSettings.CurrentBidDetails.Domicile;
			filename += GlobalSettings.CurrentBidDetails.Month.ToString("X");
			return filename;
		}


		public static void GenerateDynamicOverNightCitiesList()
		{
			GlobalSettings.OverNightCitiesInBid = new List<City>();
			foreach (Line line in GlobalSettings.Lines)
			{
				// bool isLastTrip = false; int paringCount = 0;
				Trip trip = null;
				DateTime tripDate = DateTime.MinValue;
				foreach (var pairing in line.Pairings)
				{                 //Get trip
					trip = GetTrip(pairing);

					// isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;
					// tripDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);

					List<string> overNightCities = trip.DutyPeriods.Select(x => x.ArrStaLastLeg).Where(y => y.ToString() != GlobalSettings.CurrentBidDetails.Domicile).ToList();

					foreach (string city in overNightCities)
					{
						if (!GlobalSettings.OverNightCitiesInBid.Any(x => x.Name == city))
						{
							var newcity = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == city);

							if (newcity != null)
							{
								GlobalSettings.OverNightCitiesInBid.Add(new City()
								{
									Name = city,
									Id = newcity.Id
								});
							}
						}

					}

				}
			}

			GlobalSettings.OverNightCitiesInBid = GlobalSettings.OverNightCitiesInBid.OrderBy(x => x.Name).ToList();
		}

		private static Trip GetTrip(string pairing)
		{
			Trip trip = null;
			trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
			if (trip == null)
			{
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
			}
			if (trip == null && pairing.Length>6)
			{
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0,6)).FirstOrDefault();
			}

			return trip;

		}
		/// <summary>
		/// push the Current state file to the Undo  stack and clear the Redo statck
		/// </summary>
		public static void PushToUndoStack()
		{
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (GlobalSettings.UndoStack.Count == 99)
			{
				GlobalSettings.UndoStack.RemoveAt(98);
			}
			GlobalSettings.UndoStack.Insert(0, new WBidState(wBIdStateContent));
			GlobalSettings.RedoStack.Clear();
		}


		public static void LogDetails(string employeeNumber, string eventName, string buddy1, string buddy2)
		{
			try
			{

				WBidDataDwonloadAuthServiceClient client;
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
				client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
				client.LogOperationCompleted += Client_LogOperationCompleted;


				string baseStr = GlobalSettings.WbidUserContent.UserInformation.Domicile;
				string roundStr = "M";
				string monthStr = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("MMM").ToUpper();
				string positionStr = GlobalSettings.WbidUserContent.UserInformation.Position;

				if (GlobalSettings.CurrentBidDetails != null)
				{
					baseStr = GlobalSettings.CurrentBidDetails.Domicile;
					roundStr = GlobalSettings.CurrentBidDetails.Round;
					monthStr = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
					positionStr = GlobalSettings.CurrentBidDetails.Postion;

				}


				//DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
				WBidDataDownloadAuthorizationService.Model.LogDetails logDetails = new WBidDataDownloadAuthorizationService.Model.LogDetails();
				buddy1 = buddy1 ?? "0";
				buddy2 = buddy2 ?? "0";

				logDetails.Base = baseStr;
				logDetails.Round = (roundStr == "M") ? 1 : 2;
				logDetails.Month = monthStr;
				logDetails.Position = positionStr;
				logDetails.OperatingSystemNum = CommonClass.OperatingSystem;
				logDetails.PlatformNumber = CommonClass.Platform;
				logDetails.EmployeeNumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", "").Replace("E", ""));
				logDetails.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
				logDetails.Event = eventName;
				logDetails.Message = eventName;
				logDetails.BidForEmpNum = int.Parse(employeeNumber.Replace("e", "").Replace("E", ""));
				logDetails.BuddyBid1 = int.Parse(buddy1.Replace("e", "").Replace("E", ""));
				logDetails.BuddyBid2 = int.Parse(buddy2.Replace("e", "").Replace("E", ""));
				client.LogOperationAsync(logDetails);





				//client.LogOperationAsync(
			}
			catch (Exception ex)
			{


			}
		}
		public  static bool IsRatioPropertiesSetFromOtherViews(WBidState wbidStateContent)
		{
			bool isNormalMode = !(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM);

			if (isNormalMode)
			{
				if (GlobalSettings.WBidINIContent.BidLineNormalColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.ModernNormalColumns.Any(x => x == 75) ||
				 GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 75) ||
wbidStateContent.SortDetails.BlokSort.Contains("19")
				//wbidSta
				)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (
				GlobalSettings.WBidINIContent.BidLineVacationColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.ModernVacationColumns.Any(x => x == 75) ||
				 GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 75) ||
wbidStateContent.SortDetails.BlokSort.Contains("19")

				)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		public static void SetRatioValues(WBidState wbidStateContent)
		{
			if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == 75) ||
				GlobalSettings.WBidINIContent.ModernNormalColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.ModernVacationColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.BidLineNormalColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.BidLineVacationColumns.Any(x => x == 75) ||
				GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == 75) ||
				wbidStateContent.SortDetails.BlokSort.Contains("19")

				)
			{

				int numeratorvalue = 0;

				int denominatorvalue = 0;
				if (GlobalSettings.WBidINIContent.RatioValues.Numerator == 58 || GlobalSettings.WBidINIContent.RatioValues.Numerator == 59)
				{
					GlobalSettings.WBidINIContent.RatioValues.Numerator = 0;
					numeratorvalue = 0;
				}
				if (GlobalSettings.WBidINIContent.RatioValues.Denominator == 58 || GlobalSettings.WBidINIContent.RatioValues.Denominator == 59)
				{
					GlobalSettings.WBidINIContent.RatioValues.Denominator = 0;
					denominatorvalue = 0;
				}


				var numeratorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Numerator);
				var denominatorcolumn = GlobalSettings.columndefinition.FirstOrDefault(X => X.Id == GlobalSettings.WBidINIContent.RatioValues.Denominator);

				if (numeratorcolumn != null && denominatorcolumn != null && numeratorvalue != 0 && denominatorvalue != 0)
				{
					foreach (var line in GlobalSettings.Lines)
					{

						var numerator = line.GetType().GetProperty(numeratorcolumn.DataPropertyName).GetValue(line, null);
						if (numeratorcolumn.DataPropertyName == "TafbInBp")
							numerator = (line.TafbInBp == null) ? 0 : Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
						decimal numeratorValue = Convert.ToDecimal(numerator);

						var denominator = line.GetType().GetProperty(denominatorcolumn.DataPropertyName).GetValue(line, null);
						if (denominatorcolumn.DataPropertyName == "TafbInBp")
							denominator = (line.TafbInBp == null) ? 0 : Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
						decimal denominatorValue = Convert.ToDecimal(denominator);


						line.Ratio = Math.Round(decimal.Parse(String.Format("{0:0.00}", (denominatorValue == 0) ? 0 : numeratorValue / denominatorValue)), 2, MidpointRounding.AwayFromZero);

					}
				}
			}
		}
		static void Client_LogOperationCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{

		}
        public static Dictionary<string, Trip> GetMissingtripFromVPS(MonthlyBidDetails bidDetails)
        {

            var jsonData = ServiceUtils.JsonSerializer(bidDetails);
            StreamReader dr = ServiceUtils.GetRestData("GetScrappedMissedTrips", jsonData);
            MissedTripResponseModel tripdata = WBidCollection.ConvertJSonStringToObject<MissedTripResponseModel>(dr.ReadToEnd());
            if (tripdata.JsonTripData == null || tripdata.Message == "No such missed trips available")
            {
                return new Dictionary<string, Trip>();
            }
            Dictionary<string, Trip> tripdatas = tripdata.JsonTripData.ToDictionary(x => x.TripNum, x => x);
            return tripdatas;


        }


		public static string GetAwardAlert(UserBidDetails userbiddetails)
		{
			string alert = string.Empty;

			AwardDetails awarddata = null;
			string url = GlobalSettings.DataDownloadAuthenticationUrl + "GetCurrentMonthAwardData";

			string data = SmartSyncLogic.JsonObjectToStringSerializer<UserBidDetails>(userbiddetails);
			string response = ServiceUtils.PostData(url, data);
			response.Trim('"');

			awarddata = ServiceUtils.ConvertJSonToObject<AwardDetails>(response);
			
			string monthName = new DateTime(2010, userbiddetails.Month, 1).ToString("MMM", CultureInfo.InvariantCulture);
			if (awarddata.AwardedLine != 0)
			{

				if (awarddata.IsPaperbid)
				{
					alert = "You are a paper bid for the month and you were awarded line " + awarddata.AwardedLine;
				}
				else if (userbiddetails.Position == "FA")
				{
					//You were awarded line 214 B for Jan 2020.  You will be flying with Wonder Woman (22222) postion A and SuperMan (11111) position C
					//You are a paper bid for the month and you were awarded line 176
					//You were awarded line 213 for Jan 2020.  You will be flying with Capt Sky King (22028)".  Then a simple OK button.  When the OK button is pushed, the Awards list will display

					alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .";
					if (awarddata.BuddyAwards.Count > 0)
					{
						alert += "\n\nYou will be flying with ";
					}
					foreach (var item in awarddata.BuddyAwards)
					{
						alert += item.BuddyName.TrimEnd() + " ( " + item.BuddyEmpNum + " ) position " + item.BuddyPosition + " and ";
					}
					if (alert.Length > 3 && alert.Substring(alert.Length - 4, 4) == "and ")
					{
						alert = alert.Substring(0, alert.Length - 4);
					}
				}
				else if (userbiddetails.Position == "CP")
				{
					alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
					if (awarddata.BuddyAwards.Count > 0)
					{
						alert += "You will be flying with " + awarddata.BuddyAwards[0].BuddyName + " ( " + awarddata.BuddyAwards[0].BuddyEmpNum + " )";
					}
				}
				else if (userbiddetails.Position == "FO")
				{
					alert = "You were awarded line " + awarddata.AwardedLine + " " + awarddata.Position + " for " + monthName + " " + userbiddetails.Year + " .\n\n";
					if (awarddata.BuddyAwards.Count > 0)
					{
						alert += "You will be flying with Capt " + awarddata.BuddyAwards[0].BuddyName + " ( " + awarddata.BuddyAwards[0].BuddyEmpNum + " )";
					}
				}
			}
			return alert;
		}



		public static List<KeyValuePair<string, string>> CheckUserInformations(WBidDataDownloadAuthorizationService.Model.UserInformation serverUserData)
		{
			var DifferenceList = new List<KeyValuePair<string, string>>();
			UserInformation localUserData = new UserInformation();
			localUserData = GlobalSettings.WbidUserContent.UserInformation;
			if (serverUserData.FirstName != localUserData.FirstName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("FirstName", localUserData.FirstName + "," + serverUserData.FirstName));
			}

			if (serverUserData.LastName != localUserData.LastName)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("LastName", localUserData.LastName + "," + serverUserData.LastName));
			}


			if (serverUserData.Email != localUserData.Email)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Email", localUserData.Email + "," + serverUserData.Email));
			}

			if (serverUserData.CellPhone != localUserData.CellNumber)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("CellPhone", localUserData.CellNumber + "," + serverUserData.CellPhone));
			}

			string LocalPosition = "";
			string RemotePosition = "";
			LocalPosition = "Flight Attendant";


			if (localUserData.Position == "CP" || localUserData.Position == "FO" || localUserData.Position == "Pilot")
			{
				LocalPosition = "Pilot";
			}
			else if (localUserData.Position == "FA")
			{
				LocalPosition = "Flight Attendant";
			}

			if (serverUserData.Position == 3)
			{
				RemotePosition = "Flight Attendant";
			}

			else if (serverUserData.Position == 4)
			{
				RemotePosition = "Pilot";
			}

			if (LocalPosition != RemotePosition)
			{
				DifferenceList.Add(new KeyValuePair<string, string>("Position", LocalPosition + "," + RemotePosition));
			}
			return DifferenceList;


		}
	}
}
