using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using System.ServiceModel;
using WBid.WBidiPad.Core;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.IO;

namespace WBid.WBidiPad.iOS.Utility
{
    public static class ServiceUtils
    {
		public static  EndpointAddress EndPoint = (GlobalSettings.WBidINIContent!=null && GlobalSettings.WBidINIContent.IsConnectedVofox)?new EndpointAddress("http://122.166.23.155/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/soap"):new EndpointAddress("http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc/soap");

       public static readonly EndpointAddress NetworkdataEndPoint = new EndpointAddress("http://108.60.201.50:8001/NetworkPlanService.svc");

       //public static readonly EndpointAddress PushEndPoint = new EndpointAddress("http://108.60.201.50:8001/NetworkPlanService.svc");
       public static readonly EndpointAddress PushEndPoint = new EndpointAddress("http://108.60.201.50:8007/WBidPushSerivce.svc");

        public static BasicHttpBinding CreateBasicHttp()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };
            TimeSpan timeout = new TimeSpan(0, 0, 30);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
			EndPoint = (GlobalSettings.WBidINIContent!=null && GlobalSettings.WBidINIContent.IsConnectedVofox)?new EndpointAddress("http://122.166.23.155/WBidDataDownloadAuthorizationService/WBidDataDwonloadAuthService.svc/soap"):new EndpointAddress("http://108.60.201.50:8000/WBidDataDwonloadAuthService.svc/soap");
            return binding;
        }
        public static BasicHttpBinding CreateBasicHttpForOneminuteTimeOut()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };
            TimeSpan timeout = new TimeSpan(0, 0, 59);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            
            return binding;
        }


        public static BasicHttpBinding CreateBasicHttp(int minute,int second)
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };

            binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
            {
                MaxArrayLength = 2147483646,
                MaxStringContentLength = 5242880,
            };
            TimeSpan timeout = new TimeSpan(0, minute, second);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            binding.CloseTimeout = timeout;
            return binding;
        }
        public static StreamReader GetRestData(string serviceNameandParameter)
        {
            string url = GlobalSettings.DataDownloadAuthenticationUrl + serviceNameandParameter;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 30000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            var reader = new StreamReader(stream);
            return reader;

        }
		public static StreamReader GetRestData(string serviceName,string jsonString)
		{
			string url = GlobalSettings.DataDownloadAuthenticationUrl + serviceName;
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";

			var bytes = Encoding.UTF8.GetBytes(jsonString);
			request.ContentLength = bytes.Length;
			request.GetRequestStream().Write(bytes, 0, bytes.Length);
			//Response
			var response = (HttpWebResponse)request.GetResponse();
			var streamoutput = response.GetResponseStream();
			var readeroutput = new StreamReader(streamoutput);
			return readeroutput;

			// streamoutput.Dispose();
			// readeroutput.Dispose();
		}
        public static string PostData(string url, string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            var bytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;
            request.GetRequestStream().Write(bytes, 0, bytes.Length);
            request.Timeout = 30000;
            //Response
            var response = (HttpWebResponse)request.GetResponse();
            var stream = response.GetResponseStream();
            if (stream == null)
                return string.Empty;

            var reader = new StreamReader(stream);
            string result = reader.ReadToEnd();

            return result.Trim('"');
        }
		public static string JsonSerializer<T>(T t)
		{
			string jsonString = string.Empty;
			DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
			MemoryStream ms = new MemoryStream();

            ser.WriteObject(ms, t);

			jsonString = Encoding.UTF8.GetString(ms.ToArray());
			return jsonString;
		}
        public static T ConvertJSonToObject<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)serializer.ReadObject(ms);
            return obj;
        }
    }
}