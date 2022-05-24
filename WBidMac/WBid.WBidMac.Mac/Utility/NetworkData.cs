using System;
using WBid.WBidiPad.Core;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using System.Net;
using System.IO.Compression;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using System.Linq;

namespace WBid.WBidiPad.iOS.Utility
{
	public class NetworkData
	{
		public NetworkData ()
		{
		}

		public void GetFlightRoutes()
		{
			string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
			string zipLocalFile = Path.Combine (WBidHelper.GetAppDataPath (), "FlightData.zip");
			string networkDataPath = WBidHelper.GetAppDataPath () + "/" + "FlightData.NDA";

			GlobalSettings.FlightRouteDetails  = null;
			WebClient wcClient = new WebClient ();
			if (File.Exists (networkDataPath)) {
				File.Delete (networkDataPath);
			}

			//Downloading networkdat file
			wcClient.DownloadFile (serverPath, zipLocalFile);

			//                //Extracting the zip file
			//                var zip = new ZipArchive();
			//                zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");

			string target = Path.Combine (WBidHelper.GetAppDataPath (), WBidHelper.GetAppDataPath () + "/");//+ Path.GetFileNameWithoutExtension(zipLocalFile))+ "/";


			//ZipStorer.

			// Open an existing zip file for reading
			ZipStorer zip = ZipStorer.Open (zipLocalFile, FileAccess.Read);

			// Read the central directory collection
			List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();

			// Look for the desired file
			foreach (ZipStorer.ZipFileEntry entry in dir) {
				zip.ExtractFile (entry, target + entry);
			}
			zip.Close ();

			if (File.Exists (zipLocalFile)) {
				File.Delete (zipLocalFile);
			}

			ReadFlightRoutes ();





		}

		public void ReadFlightRoutes()
		{
			//Deserializing data to FlightPlan object
			string networkDataPath = WBidHelper.GetAppDataPath () + "/" + "FlightData.NDA";
			FlightPlan fp = new FlightPlan ();
			FlightPlan flightPlan = null;
			using (FileStream networkDatatream = File.OpenRead (networkDataPath)) {

				FlightPlan objineinfo = new FlightPlan ();
				flightPlan = ProtoSerailizer.DeSerializeObject (networkDataPath, fp, networkDatatream);

			}


			//				if (File.Exists (networkDataPath)) {
			//					File.Delete (networkDataPath);
			//				}



			//VacationCorrectionParams vacationParams = new VacationCorrectionParams ();
			GlobalSettings.FlightRouteDetails = flightPlan.FlightRoutes.Join (flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
				(fr, f) =>
				new FlightRouteDetails {
					Flight = f.FlightId,
					FlightDate = fr.FlightDate,
					Orig = f.Orig,
					Dest = f.Dest,
					Cdep = f.Cdep,
					Carr = f.Carr,
					Ldep = f.Ldep,
					Larr = f.Larr,
					RouteNum = fr.RouteNum,

				}).ToList ();

		}

		/// <summary>
		///  To calculate the flight route details to calculate commute difference values. 
		/// </summary>
		public List<FlightRouteDetails> GetFlightRoutesForTempCalculation()
		{
			List<FlightRouteDetails> lstFlightRouteDetails = new List<FlightRouteDetails>();
			string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
			string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightDataTemp.zip");
			string networkDataPath = WBidHelper.GetAppDataPath() + "/FlightDataTemp/" + "FlightData.NDA";

			if (System.IO.File.Exists(networkDataPath))
				File.Delete(networkDataPath);

			GlobalSettings.FlightRouteDetails = null;
			WebClient wcClient = new WebClient();
			//Downloading networkdat file
			wcClient.DownloadFile(serverPath, zipLocalFile);

			string target = Path.Combine(WBidHelper.GetAppDataPath(), WBidHelper.GetAppDataPath() + "/FlightDataTemp/");
			if (System.IO.File.Exists(networkDataPath))
				File.Delete(networkDataPath);
			//if (!File.Exists(networkDataPath))

			{
				if (File.Exists(zipLocalFile))
					ZipFile.ExtractToDirectory(zipLocalFile, target);
			}


			if (File.Exists(zipLocalFile))
			{
				File.Delete(zipLocalFile);
			}

			lstFlightRouteDetails = ReadFlightRoutesForTemporary();
			return lstFlightRouteDetails;
		}

		public List<FlightRouteDetails> ReadFlightRoutesForTemporary()
		{
			List<FlightRouteDetails> _flightRouteDetails = new List<FlightRouteDetails>();
			//Deserializing data to FlightPlan object
			string networkDataPath = WBidHelper.GetAppDataPath() + "/FlightDataTemp/" + "FlightData.NDA";
			FlightPlan fp = new FlightPlan();
			FlightPlan flightPlan = null;
			using (FileStream networkDatatream = File.OpenRead(networkDataPath))
			{

				FlightPlan objineinfo = new FlightPlan();
				flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);

			}

			_flightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
				(fr, f) =>
				new FlightRouteDetails
				{
					Flight = f.FlightId,
					FlightDate = fr.FlightDate,
					Orig = f.Orig,
					Dest = f.Dest,
					Cdep = f.Cdep,
					Carr = f.Carr,
					Ldep = f.Ldep,
					Larr = f.Larr,
					RouteNum = fr.RouteNum,

				}).ToList();
			return _flightRouteDetails;
		}


	}
}

