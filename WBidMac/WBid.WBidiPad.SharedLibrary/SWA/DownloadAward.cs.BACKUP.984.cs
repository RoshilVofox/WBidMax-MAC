using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Model.SWA;
namespace WBid.WBidiPad.SharedLibrary.SWA
{
    public class DownloadAward
    {


        private bool buddyBidTest ;
        /// <summary>
        /// PURPOSE : Download Award Details
        /// </summary>
        /// <param name="downloadInfo"></param>
        /// <returns></returns>
        private List<DownloadedFileInfo> DownloadAwardDetails(DownloadInfo downloadInfo)
        {

            List<DownloadedFileInfo> lstDownloadedFiles = null;
            string fileName = string.Empty;
            string fileNametoSave = string.Empty;



            //// Download files from thirdparty server  
            //if (_isCompanyServerData)
            //{

            lstDownloadedFiles = DownloadFiles(downloadInfo);
            //}
            //// Download Mock awards  
            //else
            //{
            //    lstDownloadedFiles = DownloadMockDetails(downloadInfo);

            //}




            return lstDownloadedFiles;
        }
        /// <summary>
        /// PURPOSE : Download Files
        /// </summary>
        /// <param name="downloadInfo"> Creential details UserId,Password,List of files to be downloaded etc</param>
        /// <returns></returns>
        public List<DownloadedFileInfo> DownloadFiles(DownloadInfo downloadInfo)
        {
            try
            {

                List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo>();

                string packetType = string.Empty;

                //Get precredential details
                downloadInfo.PreCredentials = GetPreCredentials();
                //Get session credentials
                downloadInfo.SessionCredentials = GetSessionCredentials(downloadInfo);


                foreach (string filename in downloadInfo.DownloadList)
                {
                    packetType = string.Empty;
                    //if length <10 or > 11 will add an error message  and continue to download next file. 
                    if (filename.Length < 10 || filename.Length > 11)
                    {
                        downloadedFileDetails.Add(new DownloadedFileInfo() { IsError = true, Message = "", FileName = filename });
                        continue;
                    }
                    //finding packet type
                    packetType = (filename.Substring(7, 3) == "737") ? "ZIPPACKET" : "TXTPACKET";


                    //Download the selected file and adding  downloaded information to downloadedFileDetails list.
                    downloadedFileDetails.Add(GetFile(downloadInfo, filename.ToUpper(), packetType));



                    //logStream.WriteLine("{0} File download length: {1}", DateTime.Now, downloadedFile.file.Length);
                    //logStream.WriteLine("");

                }


                return downloadedFileDetails;
            }
            catch (Exception)
            {
                return null;

            }
        }

        /// <summary>
        /// PURPOSE : Download File
        /// </summary>
        /// <param name="downloadInfo">It holds username, password,Precredentials etc</param>
        /// <param name="fileToGet"> file name to download UPPER CASE</param>
        /// <param name="packetType"> packet type "ZIPPACKET" / "TXTPACKET"</param>
        /// <returns></returns>
        public DownloadedFileInfo GetFile(DownloadInfo downloadInfo, string fileToGet, string packetType)
        {
            try
            {

                DownloadedFileInfo fileDetails = new DownloadedFileInfo();
                WebClient webClient1 = new WebClient();
                webClient1.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                NameValueCollection formData = new NameValueCollection();
                formData["REQUEST"] = packetType;  // can also be "TXTPACKET"
                formData["CREDENTIALS"] = downloadInfo.SessionCredentials;
                formData["NAME"] = fileToGet;

                // frank add for buddy bid testing
                string url = buddyBidTest ? "https://www27.swalifeqa.com/webbid3pty/ThirdParty" :
                                            "https://www27.swalife.com/webbid3pty/ThirdParty";

                fileDetails.byteArray = webClient1.UploadValues(url, "POST", formData);
                fileDetails.FileName = fileToGet;
                var buf = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, fileDetails.byteArray);
                string temp = Encoding.UTF8.GetString(buf, 0, buf.Length);

                if (fileDetails.byteArray.Length < 200)
                {
                    fileDetails.IsError = true;
                    if (temp.Substring(temp.Length - 2, 2) == Environment.NewLine)
                    {
                        fileDetails.Message = temp.Substring(0, temp.Length - 2);
                    }
                    else
                    {
                        fileDetails.Message = temp;
                    }
                    //logStream.WriteLine("{0} {1}", DateTime.Now, fileDetails.message);
                }
                else
                {
                    fileDetails.IsError = false;
                    fileDetails.Message = string.Empty;
                    //logStream.WriteLine("{0} File: {1} downloaded", DateTime.Now, fileToGet);
                }

                return fileDetails;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        /// <summary>
        /// PURPOSE : Get Session Credentials
        /// </summary>
        /// <param name="preCred"></param>
        /// <returns></returns>
        public string GetSessionCredentials(DownloadInfo downloadInfo)
        {
            try
            {
                // frank add for buddy bid testing
                string URL = buddyBidTest ? "https://www27.swalifeqa.com/webbid3pty/ThirdParty" :
                                            "https://www27.swalife.com/webbid3pty/ThirdParty";

                WebClient webClient = new WebClient();
                NameValueCollection formData = new NameValueCollection();
                formData["CREDENTIALS"] = downloadInfo.PreCredentials;
                formData["REQUEST"] = "LOGON";
                formData["UID"] = downloadInfo.UserId;
                formData["PWD"] = downloadInfo.Password;
                byte[] responseBytes = webClient.UploadValues(URL, "POST", formData);
                return Encoding.UTF8.GetString(responseBytes);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// PURPOSE : Get Pre credentials informations
        /// </summary>
        /// <returns></returns>
        public string GetPreCredentials()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

                // frank add for buddy bid testing
                HttpWebRequest request = buddyBidTest ? (HttpWebRequest)WebRequest.Create("https://www27.swalifeqa.com/webbid3pty/ThirdParty") :
                                                        (HttpWebRequest)WebRequest.Create("https://www27.swalife.com/webbid3pty/ThirdParty");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                return sr.ReadToEnd();


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
