using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.Core;
using System.ServiceModel;

namespace WBid.WBidiPad.iOS.Utility
{
    public class WBidMail
    {
        public WBidMail()
        {
 
        }
        WBidDataDwonloadAuthServiceClient WBidDataDwonloadAuthServiceClientClient;
        public void SendCrashMail(string bidData)
        {
            MailInfo mailInfo = new MailInfo();
            mailInfo.Alias = GetmailAlias();
            mailInfo.ToAddress = "maccrash@wbidmax.com";
            mailInfo.FromAddress = "admin@wbidmax.com";
            mailInfo.MessageBody = bidData.Replace("\r\n", "<br/>");
			if (GlobalSettings.WbidUserContent.UserInformation != null)
			{
				mailInfo.EmployeeNumber = Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
				mailInfo.UserAppEmail = GlobalSettings.WbidUserContent.UserInformation.Email;
			}
            //mailInfo.MessageBody = getFormattedErrorMailContent(bidData);
            mailInfo.Subject = "WBidMax Error Log" + " (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " )";
            SendMail(mailInfo);
        }

        public void SendMailtoAdmin(string mailcontent, string fromId,string subject)
        {
            MailInfo mailInfo = new MailInfo();
            mailInfo.Alias = GetmailAlias();
            mailInfo.ToAddress = "admin@wbidmax.com";
            mailInfo.FromAddress = fromId;
            mailInfo.MessageBody = mailcontent;
            mailInfo.Subject = subject;
            SendMail(mailInfo);
        }
        public void SendSupportMail(string mailcontent, string fromId, string subject)
        {
            MailInfo mailInfo = new MailInfo();
            mailInfo.Alias = GetmailAlias();
            mailInfo.ToAddress = "support@wbidmax.app";
            mailInfo.FromAddress = fromId;
            mailInfo.MessageBody = mailcontent;
            mailInfo.Subject = subject;
            SendMail(mailInfo);
        }

        public void SendMailtoUser(string mailcontent, string toEmailId, string subject, byte[] attachement, string attachmentName)
        {
            MailInfo mailInfo = new MailInfo();
            mailInfo.Alias = "WBidMax Admin";
            mailInfo.ToAddress = toEmailId;
            mailInfo.FromAddress = "admin@wbidmax.com";
            mailInfo.MessageBody = mailcontent;
            mailInfo.Subject = subject;
            mailInfo.Attachment1 = attachement;
            mailInfo.Attachment1Name = attachmentName;
            SendMail(mailInfo);
        }
        private void SendMail(MailInfo mailInfo)
        {
            try
            {
                BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
                WBidDataDwonloadAuthServiceClientClient = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
                WBidDataDwonloadAuthServiceClientClient.SendMailCompleted += dwonloadAuthServiceClient_SendMailCompleted;
                WBidDataDwonloadAuthServiceClientClient.SendMailAsync(mailInfo);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        private string getFormattedErrorMailContent(string body)
        {
            var sb = new StringBuilder();
            sb.Append("<table  border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Hi Admin ,</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">&nbsp;</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px; padding: 0 0 10px 0;\">");
            sb.Append(body);
            sb.Append("<br /><br /> Data  :" + GetApplicationbiddata());
            sb.Append("<br /><br /> OS  :" + "Ipad OS");
			if (GlobalSettings.WbidUserContent.UserInformation != null)
			{
				sb.Append("<br/>"+"Base            :" + GlobalSettings.WbidUserContent.UserInformation.Domicile );
				sb.Append("<br/>"+"Seat            :" + GlobalSettings.WbidUserContent.UserInformation.Position);
				sb.Append("<br/>"+"Employee Number :" + GlobalSettings.WbidUserContent.UserInformation.EmpNo);
				sb.Append("<br/>" + "Email  :" + GlobalSettings.WbidUserContent.UserInformation.Email);
			}

            sb.Append("</td></tr>");

            sb.Append("</table>");
            sb.Append("<table width=\"250\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
            sb.Append("<tr><td></td><td></td><td></td></tr><tr><td colspan=\"3\">&nbsp;</td></tr><tr><td colspan=\"3\">&nbsp;</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\"><br/><br/>Sincerely ,</td></tr>");
            sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\">");
            sb.Append(GetmailAlias());
            sb.Append("</td></tr>");
            sb.Append("</table>");
            return sb.ToString();
        }
        /// <summary>
        /// PURPOSE : Set Application Title 
        /// </summary>
        private string GetApplicationbiddata()
        {
            try
            {
                string domicile = GlobalSettings.CurrentBidDetails.Domicile ?? string.Empty;
                string position = GlobalSettings.CurrentBidDetails.Postion ?? string.Empty;
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                string strMonthName = mfi.GetMonthName(GlobalSettings.CurrentBidDetails.Month).ToString();
                string round = GlobalSettings.CurrentBidDetails.Round == "M" ? "Monthly" : "2nd Round";
                return domicile + "/" + position + "/" + " " + round + "  Line for " + strMonthName + " " + GlobalSettings.CurrentBidDetails.Year;

            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        private string GetmailAlias()
        {
            try
            {
                if (GlobalSettings.WbidUserContent.UserInformation != null)
                {
                    return GlobalSettings.WbidUserContent.UserInformation.FirstName + " " + GlobalSettings.WbidUserContent.UserInformation.LastName + "-" + GlobalSettings.WbidUserContent.UserInformation.EmpNo+"WBid-Mac";
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }
        void dwonloadAuthServiceClient_SendMailCompleted(object sender, SendMailCompletedEventArgs e)
        {
            if (e.Result != null)
            {
 
            }
        }
    }
}