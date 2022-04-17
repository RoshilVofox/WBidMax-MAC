#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model.SWA
{
    public class DownloadedFileInfo
    {

        // used in return of file from 3rd party app
        // return IsError=true , if there is an error
        // returns error message or
        // returns file as byte[] array

        /// <summary>
        /// Is Error bit== Returns "TRUE" if error exists
        /// </summary>
        public bool IsError;


        /// <summary>
        /// File Name
        /// </summary>
        public string FileName;

        /// <summary>
        /// Message-- Returns error message if error occured
        /// </summary>
        public string Message;

        /// <summary>
        /// File content
        /// </summary>
        public byte[] byteArray;
    }
}
