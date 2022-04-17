#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Parser;
using System.IO;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Parser;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel; 
#endregion

namespace WBid.WBidiPad.iOS.Utility
{
    public class ReparseBL
    {
        public static Dictionary<string, Trip> ReparseTripAndLineFiles(ReparseParameters reparseParams)
		{

			Dictionary<string, Trip> trips = null;
			Dictionary<string, Line> lines = null;
			Dictionary<string, Trip> tempTrip = null;
			List<string> pairingwHasNoDetails = new List<string>();
			string fileToSave = string.Empty;

			//Parse trip and Line file
			trips = ParseTripFile(reparseParams.ZipFileName);


			if (reparseParams.ZipFileName.Substring(0, 1) == "A" && reparseParams.ZipFileName.Substring(1, 1) == "B")
			{
				FASecondRoundParser fASecondRound = new FASecondRoundParser();
				lines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + @"\" + reparseParams.ZipFileName.Substring(0, 6).ToString() + @"/PS", ref trips, GlobalSettings.FAReserveDayPay, reparseParams.ZipFileName.Substring(2, 3));

			}
			else
			{
				lines = ParseLineFiles(reparseParams.ZipFileName);
			}

			if (trips == null) return null;

			TripTtpParser tripTtpParser = new TripTtpParser();
			List<CityPair> listCityPair = tripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
			GlobalSettings.TtpCityPairs = listCityPair;

			//Second Round missing trip management
			//---------------------------------------------------------------------------

			if (GlobalSettings.CurrentBidDetails.Round == "S")
			{   //If  the round is second round ,some times trip list contains  missing trip. So we need  take these trip details from old .WBP file.
				//Otherwise we again need to scrap the missing details from website. The issue is if the bid data is older one, we cannot scrap it from website.
				//tempTrip = reparseParams.Trips;


				//List<string> allPair = lines.SelectMany(x => x.Value.Pairings).ToList();
				//pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => z == x.Substring(0, 4))).ToList();
				List<string> allPair = lines.SelectMany(x => x.Value.Pairings).Distinct().ToList();
				pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x && x.Substring(1, 1) == "P"))).ToList();

				if (pairingwHasNoDetails.Count > 0)
				{
					Dictionary<string, Trip> missingTrips = new Dictionary<string, Trip>();
					//missingTrips = missingTrips.Concat(tempTrip.Where(x => pairingwHasNoDetails.Contains(x.Key.ToString()))).ToDictionary(pair => pair.Key, pair => pair.Value);
					missingTrips = missingTrips.Concat(GlobalSettings.Trip.Where(x => pairingwHasNoDetails.Contains(x.TripNum)).ToDictionary(s => s.TripNum, s => s)).ToDictionary(pair => pair.Key, pair => pair.Value);
					if (missingTrips.Count == 0)
					{
						string bidFileName = string.Empty;
						bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
						BidLineParser bidLineParser = new BidLineParser();
						var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;
						missingTrips = missingTrips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "\\" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
					}
					// trips = trips.Concat().ToDictionary(pair => pair.Key, pair => pair.Value);
					foreach (var trip in missingTrips)
					{
						if (!trips.Keys.Contains(trip.Key) && !string.IsNullOrEmpty(trip.Key))
							trips.Add(trip.Key, trip.Value);
					}

				}


			}

			//---------------------------------------------------------------------------



			// Additional processing needs to be done to FA trips before CalculateTripPropertyValues
			CalculateTripProperties calcProperties = new CalculateTripProperties();
			if (reparseParams.ZipFileName.Substring(0, 1) == "A")
				calcProperties.PreProcessFaTrips(trips, listCityPair);

			calcProperties.CalculateTripPropertyValues(trips, listCityPair);
			//WBidHelper.SetCurrentBidInformationfromZipFileName(reparseParams.ZipFileName);

			fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


			TripInfo tripInfo = new TripInfo()
			{
				TripVersion = GlobalSettings.TripVersion,
				Trips = trips

			};

			var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP");
			ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP", tripInfo, stream);
			stream.Dispose();
			stream.Close();

			GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			if (wBIdStateContent.IsVacationOverlapOverlapCorrection)
			{

				if (GlobalSettings.WBidStateCollection.Vacation.Count > 0)
				{

					GlobalSettings.SeniorityListMember = GlobalSettings.SeniorityListMember ?? new SeniorityListMember();
					GlobalSettings.SeniorityListMember.Absences = new List<Absense>();
					GlobalSettings.WBidStateCollection.Vacation.ForEach(x => GlobalSettings.SeniorityListMember.Absences.Add(new Absense { AbsenceType = "VA", StartAbsenceDate = Convert.ToDateTime(x.StartDate), EndAbsenceDate = Convert.ToDateTime(x.EndDate) }));
					GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
					GlobalSettings.TempOrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
				}
			}
			CalculateLineProperties calcLineProperties = new CalculateLineProperties();
			calcLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);

			string vACFileName = WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC";
			if (File.Exists(vACFileName))
			{

				using (FileStream vacstream = File.OpenRead(vACFileName))
				{

					Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
					GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);
				}

				CaculateVacationDetails caculateVacationDetails = new CaculateVacationDetails();
				caculateVacationDetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);
			}
			if (wBIdStateContent.IsOverlapCorrection)
			{
				string file = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
				if (File.Exists(WBidHelper.GetAppDataPath() + "/" + file + ".OL"))
				{
					OverlapData overlapData;
					using (FileStream filestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + file + ".OL"))
					{

						OverlapData overlapdataobj = new OverlapData();
						overlapData = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + file + ".OL", overlapdataobj, filestream);
					}

					if (overlapData != null)
					{
						GlobalSettings.LeadOutDays = overlapData.LeadOutDays;
						GlobalSettings.LastLegArrivalTime = Convert.ToInt32(overlapData.LastLegArrivalTime);
					}
				}
			}

			LineInfo lineInfo = new LineInfo()
			{
				LineVersion = GlobalSettings.LineVersion,
				Lines = lines

			};

			GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));

			var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
			ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
			linestream.Dispose();
			linestream.Close();


			return trips;


		}

		public static void ReparseLineFile(ReparseParameters reparseParams)
		{

			Dictionary<string, Trip> trips = null;
			Dictionary<string, Line> lines = null;
			string fileToSave = string.Empty;


			//  trips = reparseParams.Trips;
			trips = GlobalSettings.Trip.ToDictionary(x => x.TripNum, x => x);

			if (reparseParams.ZipFileName.Substring(0, 1) == "A" && reparseParams.ZipFileName.Substring(1, 1) == "B")
			{
				FASecondRoundParser fASecondRound = new FASecondRoundParser();
				lines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + @"\" + reparseParams.ZipFileName.Substring(0, 6).ToString() + @"/PS", ref trips, GlobalSettings.FAReserveDayPay, reparseParams.ZipFileName.Substring(2, 3));

			}
			else
			{
				lines = ParseLineFiles(reparseParams.ZipFileName);
			}

			if (trips == null) return;

			TripTtpParser tripTtpParser = new TripTtpParser();
			List<CityPair> listCityPair = tripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
			GlobalSettings.TtpCityPairs = listCityPair;

			fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			if (wBIdStateContent.IsVacationOverlapOverlapCorrection)
			{

				if (GlobalSettings.WBidStateCollection.Vacation.Count > 0)
				{

					GlobalSettings.SeniorityListMember = GlobalSettings.SeniorityListMember ?? new SeniorityListMember();
					GlobalSettings.SeniorityListMember.Absences = new List<Absense>();
					GlobalSettings.WBidStateCollection.Vacation.ForEach(x => GlobalSettings.SeniorityListMember.Absences.Add(new Absense { AbsenceType = "VA", StartAbsenceDate = Convert.ToDateTime(x.StartDate), EndAbsenceDate = Convert.ToDateTime(x.EndDate) }));
					GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
					GlobalSettings.TempOrderedVacationDays = WBidCollection.GetOrderedAbsenceDates();
				}
			}

			CalculateLineProperties calcLineProperties = new CalculateLineProperties();
			calcLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);


			string vACFileName = WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC";
			if (File.Exists(vACFileName))
			{

				using (FileStream vacstream = File.OpenRead(vACFileName))
				{

					Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
					GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);
				}

				CaculateVacationDetails caculateVacationDetails = new CaculateVacationDetails();
				caculateVacationDetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);
			}
			if (wBIdStateContent.IsOverlapCorrection)
			{
				string file = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
				if (File.Exists(WBidHelper.GetAppDataPath() + "/" + file + ".OL"))
				{
					OverlapData overlapData;
					using (FileStream filestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + file + ".OL"))
					{

						OverlapData overlapdataobj = new OverlapData();
						overlapData = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + file + ".OL", overlapdataobj, filestream);
					}

					if (overlapData != null)
					{
						GlobalSettings.LeadOutDays = overlapData.LeadOutDays;
						GlobalSettings.LastLegArrivalTime = Convert.ToInt32(overlapData.LastLegArrivalTime);
					}
				}
			}
			LineInfo lineInfo = new LineInfo()
			{
				LineVersion = GlobalSettings.LineVersion,
				Lines = lines

			};

			GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));


			var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
			ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
			linestream.Dispose();
			linestream.Close();



		}



		#region Private Methods


		/// <summary>
		/// Parse trip file
		/// </summary>
		/// <param name="zipFileName"></param>
		/// <returns></returns>
		public static Dictionary<string, Trip> ParseTripFile(string zipFileName)
		{
			Dictionary<string, Trip> trips = new Dictionary<string, Trip>();
			TripParser tripParser = new TripParser();
			string filePath = WBidHelper.GetAppDataPath() + @"/" + zipFileName.Substring(0, 6).ToString() + @"/TRIPS";
			byte[] byteArray = File.ReadAllBytes(filePath);

			DateTime[] dSTProperties = DSTProperties.SetDSTProperties();
			if (dSTProperties[0] != null && dSTProperties[0] != DateTime.MinValue)
			{
				GlobalSettings.FirstDayOfDST = dSTProperties[0];
			}
			if (dSTProperties[1] != null && dSTProperties[1] != DateTime.MinValue)
			{
				GlobalSettings.LastDayOfDST = dSTProperties[1];
			}

			trips = tripParser.ParseTrips(zipFileName, byteArray, GlobalSettings.FirstDayOfDST, GlobalSettings.LastDayOfDST);
			return trips;
		}

		/// <summary>
		/// Parse line file
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static Dictionary<string, Line> ParseLineFiles(string fileName)
		{
			Dictionary<string, Line> Lines = new Dictionary<string, Line>();
			LineParser lineParser = new LineParser();
			string filePath = WBidHelper.GetAppDataPath() + @"/" + fileName.Substring(0, 6).ToString() + @"/PS";
			byte[] byteArray = File.ReadAllBytes(filePath);
			Lines = lineParser.ParseLines(fileName, byteArray);
			return Lines;
		}

		#endregion
	}
}
