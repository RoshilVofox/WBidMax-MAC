using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class ErrorHandling
    {

        public static void LogErrorDetails(Exception exception)
        {


            //string content = string.Empty;
            //if (File.Exists(path))
            //{
            //    content = File.ReadAllText(path);
            //}

            //return content;

            //string currentBid = FileOperations.ReadCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt");
            //if (exception != null)
            //{
            //    Exception InnerException = exception.InnerException;
            //    string message = exception.Message;
            //    string where = exception.StackTrace.Split(new string[] { " at " }, StringSplitOptions.None)[1];
            //    string source = exception.Source;

            //    if (InnerException != null)
            //    {
            //        if (InnerException.Message != null)
            //        {
            //            message = InnerException.Message;
            //        }

            //        if (InnerException.StackTrace != null)
            //        {
            //            where = InnerException.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
            //        }

            //        source = InnerException.Source;

            //        if (InnerException.InnerException != null)
            //        {
            //            if (InnerException.InnerException.Message != null)
            //            {
            //                message += " -> " + InnerException.InnerException.Message;
            //            }

            //            if (InnerException.InnerException.StackTrace != null)
            //            {
            //                where += "\r\n\r\n -> " + InnerException.InnerException.StackTrace.Split(new string[] { " at " },
            //                    2, StringSplitOptions.None)[1];
            //            }

            //            if (InnerException.InnerException.Source != null)
            //            {
            //                source += " -> " + InnerException.InnerException.Source;
            //            }
            //        }
            //    }

            //    if (where.Length > 1024)
            //    {
            //        where = where.Substring(0, 1024);
            //    }


            //    var submitResult = "\r\n WbidiPad Error Details.\r\n\r\n Error  :  " + message + "\r\n\r\n Where  :  " + where + "\r\n\r\n Source   :  " + source + "\r\n\r\n Date  :" + DateTime.Now;
            //    submitResult += "\r\n\r\n Data :" + currentBid + "\r\n\r\n Device :" + UIDevice.CurrentDevice.LocalizedModel;

            //    // string submitResult = "\r\n\r\n\r\n Crash Report : \r\n\r\n\r\n" + "\r\n Date: " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + "\r\n\r\n Device: " + UIDevice.CurrentDevice.LocalizedModel + "\r\n\r\n Crash Details: " + ex + "\r\n\r\n Data: " + currentBid + "\r\n\r\n" + " ******************************* \r\n";


            //    if (!Directory.Exists(WBidHelper.GetAppDataPath() + "/" + "Crash"))
            //    {
            //        Directory.CreateDirectory(WBidHelper.GetAppDataPath() + "/" + "Crash");
            //    }

            //    System.IO.File.AppendAllText(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log", submitResult);
            //}
        }
    }
}
