using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class SmartSyncLogic
    {

        public static string ObjectToBase64(WBidStateCollection wBidStateCollection)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(WBidStateCollection));
            string base64String = "";
            using (MemoryStream memStream = new MemoryStream())
            {
                if (wBidStateCollection.StateUpdatedTime == DateTime.MinValue) ;
				wBidStateCollection.StateUpdatedTime = wBidStateCollection.StateUpdatedTime.ToUniversalTime ();
                ser.WriteObject(memStream, wBidStateCollection);

                byte[] byteArray = memStream.ToArray();

                base64String = Convert.ToBase64String(byteArray);


            }
            return base64String;
        }


        public static string JsonSerializer<T>(T t)
        {
            string jsonString = string.Empty;
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            jsonString = Encoding.UTF8.GetString(ms.ToArray(), 0, ms.ToArray().Length);
            return jsonString;
        }

        public static string JsonObjectToStringSerializer<T>(T t)
        {
           
            return  JsonConvert.SerializeObject(t);
          


        }

        public static string ConvertToJsonString(WBidStateCollection wBidStateCollection)
        {
            return JsonConvert.SerializeObject(wBidStateCollection);
        }
        public static T ConvertJsonToObject<T>(string json)
        {
           return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
