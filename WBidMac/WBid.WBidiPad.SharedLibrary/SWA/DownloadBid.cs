#region NameSpace

//using MiniZip.ZipArchive;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model.SWA;

//using System.IO.Packaging;
using System.IO.Compression;

#endregion

namespace WBid.WBidiPad.SharedLibrary.SWA
{
	public class DownloadBid
	{

		/// <summary>
		/// Used for Downloading the Bid details and save the downloaded bid files
		/// Date 17 January 2014
		/// </summary>

		#region Private Variables
        private string _serverUrl = string.Empty;

		#endregion

		#region Public Methods

		//public List<DownloadedFileInfo> DownloadFiles(DownloadInfo downloadInfo)
		//{
		//    try
		//    {
		//        _serverUrl = SWAConstants.SWAUrl;
		//        List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo>();
		//        string packetType = string.Empty;

		//        //Check the SWA authentication
		//        Authentication authentication = new Authentication();
		//        downloadInfo.SessionCredentials = authentication.CheckCredential(downloadInfo.UserId, downloadInfo.Password);

		//        foreach (string filename in downloadInfo.DownloadList)
		//        {
		//            packetType = string.Empty;
		//            //if length <10 or > 11 will add an error message  and continue to download next file.
		//            if (filename.Length < 10 || filename.Length > 11)
		//            {
		//                downloadedFileDetails.Add(new DownloadedFileInfo() { IsError = true, Message = "", FileName = filename });
		//                continue;
		//            }
		//            //finding packet type
		//            packetType = (filename.Substring(7, 3) == "737") ? "ZIPPACKET" : "TXTPACKET";


		//            //Download the selected file and adding  downloaded information to downloadedFileDetails list.
		//            downloadedFileDetails.Add(DownloadBidFile(downloadInfo, filename.ToUpper(), packetType));

		//        }


		//        return downloadedFileDetails;
		//    }
		//    catch (Exception)
		//    {
		//        return null;

		//    }
		//}


		/// <summary>
		/// Download Bid files
		/// </summary>
		/// <param name="downloadInfo"></param>
		/// <param name="fileToGet"></param>
		/// <param name="packetType"></param>
		/// <returns></returns>
		public DownloadedFileInfo DownloadBidFile (DownloadInfo downloadInfo, string fileToGet, string packetType)
		{
			try {
				//_serverUrl = SWAConstants.SWAUrl;
				_serverUrl = GlobalSettings.buddyBidTest ? SWAConstants.QASWAUrl : SWAConstants.SWAUrl;
				DownloadedFileInfo fileDetails = new DownloadedFileInfo ();
				WebClient webClient1 = new WebClient ();
				webClient1.Headers.Add ("Content-Type", "application/x-www-form-urlencoded");
				NameValueCollection formData = new NameValueCollection ();
				formData ["REQUEST"] = packetType;  // can also be "TXTPACKET"
				formData ["CREDENTIALS"] = downloadInfo.SessionCredentials;
				formData ["NAME"] = fileToGet;
				fileDetails.byteArray = webClient1.UploadValues (_serverUrl, "POST", formData);
				fileDetails.FileName = fileToGet;
				var buf = Encoding.Convert (Encoding.GetEncoding ("iso-8859-1"), Encoding.UTF8, fileDetails.byteArray);
				string temp = Encoding.UTF8.GetString (buf, 0, buf.Length);

				if (fileDetails.byteArray.Length < 200) {
					fileDetails.IsError = true;
					if (temp.Length > 2 && temp.Substring (temp.Length - 2, 2) == Environment.NewLine) {
						fileDetails.Message = temp.Substring (0, temp.Length - 2);
					} else {
						fileDetails.Message = temp;
					}
				} else {
					fileDetails.IsError = false;
					fileDetails.Message = string.Empty;
				}

				return fileDetails;
			} catch (Exception ex) {

				throw ex;
			}

		}


		/// <summary>
		/// PURPOSE :DownLoad Single Mock FIle
		/// </summary>
		/// <param name="wcClient"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public DownloadedFileInfo DownloadMockFile (WebClient wcClient, string fileName,bool isSeniorityFile)
		{
			DownloadedFileInfo downloadedFileInfo = new DownloadedFileInfo ();
			downloadedFileInfo.FileName = fileName;

			try {
				if (isSeniorityFile)
				{
					downloadedFileInfo.byteArray = wcClient.DownloadData(SWAConstants.WBidDownloadFileUrl + "SeniorityList/" + fileName);
				}
				else
				{
					downloadedFileInfo.byteArray = wcClient.DownloadData(GlobalSettings.MockUrl + fileName);
				}
			} catch (Exception) {

				downloadedFileInfo.IsError = true;
				downloadedFileInfo.Message = "File Not found";
				return downloadedFileInfo;
			}


			var buf = Encoding.Convert (Encoding.GetEncoding ("iso-8859-1"), Encoding.UTF8, downloadedFileInfo.byteArray);
			string temp = Encoding.UTF8.GetString (buf, 0, buf.Length);

			if (downloadedFileInfo.byteArray.Length < 200) {
				downloadedFileInfo.IsError = true;
				if (temp.Substring (temp.Length - 2, 2) == Environment.NewLine) {
					downloadedFileInfo.Message = temp.Substring (0, temp.Length - 2);
				} else {
					downloadedFileInfo.Message = temp;
				}

			} else {
				downloadedFileInfo.IsError = false;
				downloadedFileInfo.Message = string.Empty;

			}
			return downloadedFileInfo;
		}

		/// <summary>
		/// Save Downloaded Bid files
		/// </summary>
		/// <param name="lstDownloadedFiles"></param>
		/// <param name="appDataFilePath"></param>
		/// <returns></returns>
		public bool SaveDownloadedBidFiles (List<DownloadedFileInfo> lstDownloadedFiles, string appDataFilePath)
		{
			bool status = false;
			try {

				string filePath = string.Empty;
				if (lstDownloadedFiles.Count > 0) {
					foreach (DownloadedFileInfo sWAFileInfo in lstDownloadedFiles) {

						filePath = Path.Combine (appDataFilePath, sWAFileInfo.FileName);
						//If the file is error status, we dont need to save the file
						if (sWAFileInfo.IsError) {
							continue;
						}

						FileStream fStream = new FileStream (filePath, FileMode.Create);
						fStream.Write (sWAFileInfo.byteArray, 0, sWAFileInfo.byteArray.Length);
						fStream.Dispose ();

						//Extract tIhe zip file
						if (Path.GetExtension (sWAFileInfo.FileName) == ".737") {
							Extract737File (sWAFileInfo.FileName, appDataFilePath);

							//Delete the .737 file
							if (File.Exists (filePath))
								File.Delete (filePath);
						}

						status = true;
					}

				}
			} catch (Exception ex) {

				throw ex;
			}
			return status;
		}

		public static bool DownloadWBidFile (String appDataFilePath, string fileName)
		{
			bool status = true;
           
			WebClient wcClient = new WebClient ();
			try {
				wcClient.DownloadFile (SWAConstants.WBidDownloadFileUrl + fileName, appDataFilePath + "/" + fileName);
			} catch (Exception) {
				status = false;

			}
           
			return status;
		}

		public static bool DownloadWBidSeniorityFile(string appDataFilePath, string fileName)
		{
			bool status = true;

			WebClient wcClient = new WebClient();
			try
			{

				wcClient.DownloadFile(SWAConstants.WBidDownloadFileUrl + "SeniorityList/" + fileName, appDataFilePath + "/" + fileName);
			}
			catch (Exception)
			{
				status = false;

			}

			return status;
		}

		#endregion

		#region Private Methds

		/// <summary>
		/// Extract the 737 file to trip and PS file
		/// </summary>
		/// <param name="fileName"></param>
		public void Extract737File (string fileName, string appDataPath)
		{
			// = ZipFile.Read(fileName)
			try {
				//string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/WBidMax";
				if (Directory.Exists (appDataPath + "/" + Path.GetFileNameWithoutExtension (fileName)))
					Directory.Delete (appDataPath + "/" + Path.GetFileNameWithoutExtension (fileName), true);
				else
					Directory.CreateDirectory (appDataPath + "/" + Path.GetFileNameWithoutExtension (fileName));

				// string personal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				string target = Path.Combine (appDataPath, appDataPath + "/" + Path.GetFileNameWithoutExtension (fileName)) + "/";
				string zipFile = Path.Combine (appDataPath, fileName);
//				var folder = appDataPath + "/" + Path.GetFileNameWithoutExtension(fileName);
//				Directory.CreateDirectory(folder);

//				var zip = new ZipArchive();
//              zip.EasyUnzip(zipFile, target, true, "");

				ZipFile.ExtractToDirectory(zipFile,target);

				//ZipStorer.
//				var zipFile2 = zipFile.Replace (".737", ".zip");
//				if (File.Exists (zipFile2))
//					File.Delete (zipFile2);
//
//				File.Copy (zipFile, zipFile2);
//
//				// Open an existing zip file for reading
//				ZipStorer zip = ZipStorer.Open (zipFile2, FileAccess.Read);
//
//				// Read the central directory collection
//				List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir ();
//
//				// Look for the desired file
//				foreach (ZipStorer.ZipFileEntry entry in dir) {
//					zip.ExtractFile (entry, target + entry);
//				}
//				zip.Close ();
//
//				if (File.Exists (zipFile2))
//					File.Delete (zipFile2);

			} catch (Exception ex) {
				//throw ex;
			}
		}

		#endregion


	}
}
