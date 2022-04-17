using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using WBidDataDownloadAuthorizationService.Model;
using System.ServiceModel;
using WBid.WBidMac.Mac;
using System.Text.RegularExpressions;

namespace WBid.WBidiPad.iOS.Utility
{
	public class WBidLogEvent
	{
		WBidDataDwonloadAuthServiceClient DwonloadAuthServiceClient;

		public void LogBidSubmitDetails(SubmitBid submitBid, string employeeNumber,string eventname,string message)
		{
			try
			{


				// DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttpForOneminuteTimeOut();
				DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
				SubmitBidModel submitBidModel = new SubmitBidModel();
				submitBid.Buddy1 = submitBid.Buddy1 ?? "0";
				submitBid.Buddy2 = submitBid.Buddy2 ?? "0";
				submitBid.Buddy3 = submitBid.Buddy3 ?? "0";

				if (GlobalSettings.CurrentBidDetails.Postion == "FO")
				{
					submitBid.Buddy1 = submitBid.Pilot1 ?? "0";
					submitBid.Buddy2 = submitBid.Pilot2 ?? "0";
					submitBid.Buddy3 = submitBid.Pilot3 ?? "0";
				}
				submitBidModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
				submitBidModel.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
				submitBidModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
				submitBidModel.Position = GlobalSettings.CurrentBidDetails.Postion;
				submitBidModel.OperatingSystemNum = "other";
				submitBidModel.PlatformNumber = CommonClass.Platform;
				submitBidModel.EmployeeNumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", "").Replace("E", ""));
				submitBidModel.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
				submitBidModel.Event = eventname;
				submitBidModel.Message = message;
				submitBidModel.BidForEmpNum = int.Parse(employeeNumber.Replace("e", "").Replace("E", ""));
				submitBidModel.BuddyBid1 = int.Parse(submitBid.Buddy1.Replace("e", "").Replace("E", ""));
				submitBidModel.BuddyBid2 = int.Parse(submitBid.Buddy2.Replace("e", "").Replace("E", ""));
				submitBidModel.BuddyBid3 = int.Parse(submitBid.Buddy3.Replace("e", "").Replace("E", ""));
				DwonloadAuthServiceClient.SubmitBidDetailsCompleted += DwonloadAuthServiceClient_SubmitBidDetailsCompleted;
				DwonloadAuthServiceClient.SubmitBidDetailsAsync(submitBidModel);

			}
			catch (Exception ex)
			{
				throw ex;

			}

		}

		void DwonloadAuthServiceClient_SubmitBidDetailsCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
		}
		public void LogTimeoutBidSubmitDetails(SubmitBid submitBid, string employeeNumber,string swamessage)
		{
			try
			{


				//DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttpForOneminuteTimeOut();
				DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
				SubmitBidModel submitBidModel = new SubmitBidModel();
				submitBid.Buddy1 = submitBid.Buddy1 ?? "0";
				submitBid.Buddy2 = submitBid.Buddy2 ?? "0";
				submitBidModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
				submitBidModel.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
				submitBidModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
				submitBidModel.Position = GlobalSettings.CurrentBidDetails.Postion;
				//submitBidModel.OperatingSystemNum = UIDevice.CurrentDevice.SystemVersion;
				submitBidModel.PlatformNumber = CommonClass.Platform;
				submitBidModel.EmployeeNumber = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.Replace("e", "").Replace("E", ""));
				submitBidModel.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
				submitBidModel.Event = "bidSubmitTimeOut";
				submitBidModel.Message = "bidSubmitTimeOut";
				submitBidModel.BidForEmpNum = int.Parse(employeeNumber.Replace("e", "").Replace("E", ""));
				submitBidModel.BuddyBid1 = int.Parse(submitBid.Buddy1.Replace("e", "").Replace("E", ""));
				submitBidModel.BuddyBid2 = int.Parse(submitBid.Buddy2.Replace("e", "").Replace("E", ""));
				submitBidModel.SWAMessage = swamessage;
				DwonloadAuthServiceClient.SubmitBidDetailsAsync(submitBidModel);
			}
			catch (Exception ex)
			{
				throw ex;


			}

		}
		public void LogBadPasswordUsage(string userid, bool isDownload,string swaMessage)
		{
			try
			{
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttpForOneminuteTimeOut();
				DwonloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);

				WBidDataDownloadAuthorizationService.Model.LogDetails objLog = new WBidDataDownloadAuthorizationService.Model.LogDetails();
				if (isDownload)
				{
					objLog.Base = GlobalSettings.DownloadBidDetails.Domicile;
					objLog.Round = (GlobalSettings.DownloadBidDetails.Round == "D") ? 1 : 2;
					objLog.Month = new DateTime(GlobalSettings.DownloadBidDetails.Year, GlobalSettings.DownloadBidDetails.Month, 1).ToString("MMM").ToUpper();
					objLog.Position = GlobalSettings.DownloadBidDetails.Postion;
				}
				else
				{
					objLog.Base = GlobalSettings.CurrentBidDetails.Domicile;
					objLog.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
					objLog.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
					objLog.Position = GlobalSettings.CurrentBidDetails.Postion;
				}


				objLog.OperatingSystemNum = CommonClass.OperatingSystem;
				objLog.PlatformNumber = CommonClass.Platform;
				objLog.Event = "bad password";
				objLog.Message = "bad password";
				objLog.SWAMessage = swaMessage;
				objLog.VersionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
				objLog.EmployeeNumber = Convert.ToInt32(Regex.Match(userid, @"\d+").Value);
				DwonloadAuthServiceClient.LogOperationAsync(objLog);
			}
			catch (Exception ex)
			{
				
			}
		}
	}
}