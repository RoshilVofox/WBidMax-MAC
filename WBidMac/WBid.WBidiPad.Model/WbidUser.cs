using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class WbidUser
    {
        /// <summary>
        /// Store all the user information like Emp no,Domicile,postion,and Seniority number
        /// </summary>
        public UserInformation UserInformation { get; set; }

        /// <summary>
        /// Holds the recent file
        /// </summary>
        public RecentFiles RecentFiles { get; set; }
        /// <summary>
        /// checks whether user info updated from server DB
        /// </summary
        public bool isUserFromDB { get; set; }

        /// <summary>
        /// UserAccountDateTimeField
        /// </summary

        public DateTime? UserAccountDateTimeField { get; set; }
        /// <summary>
        /// checks whether test db or live db
        /// </summary
        public bool IsVFXServer { get; set; }

    }
}
