#region NameSPace
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core; 
#endregion

namespace WBid.WBidiPad.SharedLibrary.SWA
{
    public class Authentication
    {
        /// <summary>
        /// Used for cheking the SWA authentication
        /// Date 17 January 2014
        /// </summary>

        #region Private Variables
        private string _serverUrl = string.Empty; 
        #endregion

        #region Public Methods
        public string CheckCredential(string userId, string password)
        {
            try
            {
                _serverUrl = GlobalSettings.buddyBidTest ? SWAConstants.QASWAUrl : SWAConstants.SWAUrl;
                //_serverUrl = SWAConstants.SWAUrl;
                //Get precredential details
                string preCredentials = GetPreCredentials();

                //Get session credentials
                var sessioncredential = GetSessionCredentials(userId, password, preCredentials);

                return sessioncredential;
            }
            catch (Exception ex)
            {
                //downloadInfo.SessionCredentials =  ex.Message;
                return "Exception "+ex.Message;
                
            }

        } 
        #endregion

        #region Private Methods

        /// <summary>
        /// Get PreCredentials string
        /// </summary>
        /// <returns></returns>
        private string GetPreCredentials()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_serverUrl);
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



        /// <summary>
        /// Get session credential string
        /// </summary>
        /// <param name="preCredentials"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetSessionCredentials( string username, string password,string preCredentials)
        {
            try
            {
                WebClient webClient = new WebClient();
                NameValueCollection formData = new NameValueCollection();
                formData["CREDENTIALS"] = preCredentials;
                formData["REQUEST"] = "LOGON";
                formData["UID"] = username;
                formData["PWD"] = password;
                byte[] responseBytes = webClient.UploadValues(_serverUrl, "POST", formData);
                return Encoding.UTF8.GetString(responseBytes);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }  
        #endregion
    }
}
