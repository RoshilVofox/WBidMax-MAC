#region NameSPace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model.SWA
{
    public class DownloadInfo
    {

        // <summary>
        /// Keep the information for download bid details
        /// Date 17 January 2014
        /// </summary>

        #region Properties
        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// Session Credentials
        /// </summary>
        public string SessionCredentials { get; set; }

        /// <summary>
        /// PreCredentials
        /// </summary>
        public string PreCredentials { get; set; }
        /// <summary>
        /// Download list
        /// </summary>
        public List<string> DownloadList { get; set; } 
        #endregion
    }
}
