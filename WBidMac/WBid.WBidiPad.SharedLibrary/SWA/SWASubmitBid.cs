using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.SharedLibrary.SWA
{
    public class SWASubmitBid
    {
        // frank add buddy bid test
        // ***** only frank can use this feature for now  *****

        private bool buddyBidTest = GlobalSettings.buddyBidTest;

        #region Public Methods
        /// <summary>
        /// Submit the Bid Details to the SWA Server
        /// </summary>
        /// <param name="submitBid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public string SubmitBid(SubmitBid submitBid, string sessioncredential)
        {
            string status = string.Empty;

            try
            {
               

                CustomWebClient webClient1 = new CustomWebClient();
                webClient1.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                //set the formdata values
                NameValueCollection formData = new NameValueCollection();
                formData["REQUEST"] = "UPLOAD_BID";
                formData["BASE"] = submitBid.Base;
                formData["BID"] = submitBid.Bid;
                formData["BIDDER"] = submitBid.Bidder;
                formData["BIDROUND"] = submitBid.BidRound;
                //formData["CLOSEDBIDSIM"] = "N";
                formData["CREDENTIALS"] = sessioncredential;
                formData["PACKETID"] = submitBid.PacketId;
                formData["SEAT"] = submitBid.Seat;
                formData["VENDOR"] = "WBidMax";
                // should always be null for CP and FA
                if (submitBid.Pilot1 != null) formData["PILOT1"] = submitBid.Pilot1;
                if (submitBid.Pilot2 != null) formData["PILOT2"] = submitBid.Pilot2;
                if (submitBid.Pilot3 != null) formData["PILOT3"] = submitBid.Pilot3;
                // should always be null for CP and FO
                if (submitBid.Buddy1 != null) formData["BUDDY1"] = submitBid.Buddy1;
                if (submitBid.Buddy2 != null) formData["BUDDY2"] = submitBid.Buddy2;

                byte[] responsebyte = null;

                // frank add for buddy bid testing
                string url = buddyBidTest ? "https://12.191.17.47/webbid3pty/ThirdParty" :
                                            "https://www27.swalife.com/webbid3pty/ThirdParty";
                try
                {
                    responsebyte = webClient1.UploadValues(url, "POST", formData);
                }
                catch (Exception ex)
                {
                    return "server failure";
                }
                var response = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, responsebyte);
                status = Encoding.UTF8.GetString(response, 0, response.Length);
                

            }
            catch (Exception ex)
            {
                
                throw ex;
            }
            return status;

        }
        private class CustomWebClient : WebClient
        {
            public CustomWebClient()
            {

            }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest w = base.GetWebRequest(uri);
                w.Timeout = 60000;
                return w;
            }
        }
        #endregion
    }
}
