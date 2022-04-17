using System;

using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using System.Text.RegularExpressions;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using System.ServiceModel;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidMac.Mac.ViewControllers.CustomAlertView;
using CoreGraphics;
using WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.IO;
//using WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo;

namespace WBid.WBidMac.Mac.WindowControllers.LoginNew
{
    public partial class NewLoginWindowController : NSWindowController
    {
        WBidDataDwonloadAuthServiceClient client;
        NSPanel overlayPanel;
        OverlayViewController overlay;
        public bool isUserXmlFileFound=true;
        public NewLoginWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public NewLoginWindowController(NSCoder coder) : base(coder)
        {
        }

        public NewLoginWindowController() : base("NewLoginWindow")
        {
            
        }

        public override void AwakeFromNib()
        {
            try
            {
                base.AwakeFromNib();

                userIdText.StringValue = (CommonClass.UserName != null) ? CommonClass.UserName : string.Empty;
                passswordText.StringValue = (CommonClass.Password != null) ? CommonClass.Password : string.Empty;
                cancelButton.Activated += HandleBtnCancel;
                loginButton.Activated += HandleBtnLogin;
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        public new NewLoginWindow Window
        {
            get { return (NewLoginWindow)base.Window; }
        }
        void HandleBtnCancel(object sender, EventArgs e)
        {
            try
            {
                DismissLogin();
                if (!isUserXmlFileFound)
                {
                    CommonClass.HomeController.Window.Close();
                    CommonClass.HomeController.Window.OrderOut(this);
                }
                //NSNotificationCenter.DefaultCenter.PostNotificationName("InitialLoginSuccess", new NSString(cancelButton.Title));
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        void HandleBtnLogin(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateUI())
                    return;
                string uName = userIdText.StringValue.ToLower();
                if (uName[0] != 'e' && uName[0] != 'x')
                    uName = "e" + uName;

                CommonClass.UserName = uName;
                CommonClass.Password = passswordText.StringValue;
                DismissLogin();
                overlayPanel = new NSPanel();
                overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                overlay = new OverlayViewController();
                overlay.OverlayText = "Authenticating..";
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);

                new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                    this.AuthenticationCheck();
                })).Start();
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        [Export("AuthenticationCheck")]
        private void AuthenticationCheck()
        {
            string userName = CommonClass.UserName;//KeychainHelpers.GetPasswordForUsername("user", "WBid.WBidiPad.cwa", false);
            string password = CommonClass.Password;//KeychainHelpers.GetPasswordForUsername("pass", "WBid.WBidiPad.cwa", false);

            //checking  the internet connection available
            //==================================================================================================================
            if (Reachability.CheckVPSAvailable())
            {
                //  NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);
                //checking CWA credential
                //==================================================================================================================


                Authentication authentication = new Authentication();
                string authResult = authentication.CheckCredential(userName, password);
                if (authResult.Contains("ERROR: "))
                {
                    WBidLogEvent objlogs = new WBidLogEvent();
                    objlogs.LogBadPasswordUsage(userName, false, authResult);

                    InvokeOnMainThread(() =>
                    {
                        //var alert = new NSAlert ();
                        //alert.AlertStyle = NSAlertStyle.Warning;
                        //alert.MessageText = "WBidMax";
                        //alert.InformativeText = "Invalid Username or Password";
                        //alert.RunModal();
                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                        overlayPanel.OrderOut(this);
                        var panel = new NSPanel();
                        var customAlert = new CustomAlertViewController();
                        customAlert.AlertType = "InvalidCredential";

                        CommonClass.Panel = panel;
                        panel.SetContentSize(new CGSize(430, 350));
                        panel.ContentView = customAlert.View;
                        NSApplication.SharedApplication.BeginSheet(panel, this.Window);



                    });
                }
                else if (authResult.Contains("Exception"))
                {
                    InvokeOnMainThread(() =>
                    {
                        WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                        obgWBidLogEvent.LogTimeoutBidSubmitDetails(GlobalSettings.SubmitBid, GlobalSettings.TemporaryEmployeeNumber, authResult);

                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Warning;
                        alert.MessageText = "Warning";
                        //alert.InformativeText = "The company server is down.  They have been notified.  We don?t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.";
                        alert.InformativeText = "Your attempt to submit a bid or download bid data has failed. Specifically, the Southwest Airlines server did not respond with a certain time, and as a result, you received a Server Timeout.\n\nThis can happen for many reasons.  Our suggestion is to keep trying over the next 10 minutes or so, and if the app still fails to submit a bid or download bid data, we suggest the following:\n\nChange your internet connection.You can also try to use your cell phone as a hotspot for your internet connection \n\nFinally, send us an email if you are continuously having trouble.";

                        alert.RunModal();
                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();

                    });
                }
                else
                {
                    try
                    {
                        string empNo = Regex.Replace(userName, "[^0-9]+", string.Empty);
                        string url = "GetEmployeeDetails/" + empNo+"/4";
                        StreamReader dr = ServiceUtils.GetRestData(url);
                        WBidDataDownloadAuthorizationService.Model.UserInformation responseData = ServiceUtils.ConvertJSonToObject<WBidDataDownloadAuthorizationService.Model.UserInformation>(dr.ReadToEnd());
                        checkUserDetails(responseData, empNo);

                    }
                    catch(Exception ex)
                    {
                        CommonClass.AppDelegate.ErrorLog(ex);
                        CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                    }

                  
                    //BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
                    //client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
                    //client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
                    //client.GetEmployeeDetailsCompleted += client_GetEmployeeDetailsAsyncCompleted;
                    //client.GetEmployeeDetailsAsync(empNo, "4");

                }
            }
        }
        //private void client_GetEmployeeDetailsAsyncCompleted(object sender, GetEmployeeDetailsCompletedEventArgs e)
        //{
           private void checkUserDetails(WBidDataDownloadAuthorizationService.Model.UserInformation userData,string empNum)
        { 
            try
            {
                //if (e.Result != null && e.Result.EmpNum != 0 && e.Result.EmpNum.ToString() != string.Empty)
                if (userData!=null && userData.EmpNum != 0 && userData.EmpNum.ToString()!=string.Empty)
                {
                    var userinfo = userData;
                    WbidUser wbidUser = new WbidUser();
                    wbidUser.UserInformation = new UserInformation();
                    wbidUser.UserInformation.FirstName = userinfo.FirstName;
                    wbidUser.UserInformation.LastName = userinfo.LastName;
                    wbidUser.UserInformation.Email = userinfo.Email;

                    wbidUser.UserInformation.EmpNo = userinfo.EmpNum.ToString();
                    wbidUser.UserInformation.CellNumber = userinfo.CellPhone;
                    wbidUser.UserInformation.AcceptMail = userinfo.AcceptEmail;
                    // wbidUser.UserInformation.Position =userinfo.Position;
                    if (userinfo.Position == 4)
                    {
                        wbidUser.UserInformation.Position = "Pilot";
                    }
                    else if (userinfo.Position == 3)
                    {
                        wbidUser.UserInformation.Position = "FA";
                    }
                    if (GlobalSettings.WbidUserContent != null)
                    {
                        wbidUser.RecentFiles = GlobalSettings.WbidUserContent.RecentFiles;
                    }

                    wbidUser.isUserFromDB = true;
                    wbidUser.UserInformation.IsFree = userinfo.IsFree;
                    wbidUser.UserInformation.PaidUntilDate = userinfo.WBExpirationDate ?? DateTime.MinValue; //userinfo.WBExpirationDate ?? DateTime.MinValue;
                    wbidUser.UserInformation.IsMonthlySubscribed = userinfo.IsMonthlySubscribed;
                    wbidUser.UserInformation.IsYearlySubscribed = userinfo.IsYearlySubscribed;
                    wbidUser.UserInformation.SecondSubscriptionLine = userinfo.SecondSubscriptionLine;
                    wbidUser.UserInformation.TopSubscriptionLine = userinfo.TopSubscriptionLine;
                    wbidUser.UserInformation.ThirdSubscriptionLine = userinfo.ThirdSubscriptionLine;
                    wbidUser.UserAccountDateTimeField = userinfo.UserAccountDateTime;
                    if (GlobalSettings.isVFXServer)
                        wbidUser.IsVFXServer = true;
                    GlobalSettings.WbidUserContent = wbidUser;

                    WBidHelper.SaveUserFile(wbidUser, WBidHelper.WBidUserFilePath);

                    InvokeOnMainThread(() =>
                    {
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();
                        

                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                        overlayPanel.OrderOut(this);
                        var alert = new NSAlert();
                        alert.AlertStyle = NSAlertStyle.Warning;
                        alert.MessageText = "Great";
                        alert.InformativeText = "We found a previous account from WBidMax.\nWe have imported those settings. Please verify the settings and change as needed!!";

                        alert.RunModal();
                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                       
                        
                        var userUpdation = new UserUpdateInfoController();
                        userUpdation.authenticatedUserId = empNum;
                        userUpdation.title = "User Update Info";
                        userUpdation.buttonName = "Update";
                        userUpdation.isRegister = false;
                        CommonClass.HomeController.Window.AddChildWindow(userUpdation.Window, NSWindowOrderingMode.Above);
                        userUpdation.Window.MakeKeyAndOrderFront(this);
                        NSApplication.SharedApplication.RunModalForWindow(userUpdation.Window);
                        
                    });
                    
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();
                        var userReg = new UserUpdateInfoController();
                        userReg.authenticatedUserId = empNum;
                        userReg.title = "User Registration";
                        userReg.buttonName = "Register";
                        userReg.isRegister = true;
                        userReg.Window.StandardWindowButton(NSWindowButton.CloseButton).Enabled = false;
                        CommonClass.HomeController.Window.AddChildWindow(userReg.Window, NSWindowOrderingMode.Above);
                        userReg.Window.MakeKeyAndOrderFront(this);
                        NSApplication.SharedApplication.RunModalForWindow(userReg.Window);

                    });
                }
                
            }
            catch(Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }

        }
        private bool ValidateUI()
        {

            bool status = true;
            string message = string.Empty;


            if (string.IsNullOrEmpty(userIdText.StringValue.Trim()))
            {

                userIdText.BecomeFirstResponder();
                message = "Employee number required";
                status = false;
            }
            else if (!RegXHandler.EmployeeNumberValidation(userIdText.StringValue.Trim()))
            {

                userIdText.BecomeFirstResponder();
                message = "Invalid Employee number ";
                status = false;
            }
            else if (string.IsNullOrEmpty(passswordText.StringValue.Trim()))
            {
                passswordText.BecomeFirstResponder();
                message = "Password required";
                status = false;

            }





            if (!status)
            {

                var alert = new NSAlert();
                alert.AlertStyle = NSAlertStyle.Warning;
                alert.MessageText = "WBidMax";
                alert.InformativeText = message;
                alert.AddButton("OK");
                alert.RunModal();
            }

            return status;


        }
        void DismissLogin()
        {
            userIdText.StringValue = string.Empty;
            passswordText.StringValue = string.Empty;
            this.Window.Close();
            this.Window.OrderOut(this);
            NSApplication.SharedApplication.StopModal();
        }

        public bool EmailValidation(string value)
        {
            string matchpattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

            if (!Regex.IsMatch(value, matchpattern))
            {
                return false;
            }
            return true;
        }

        private bool EmployeeNumberValidation(string value)
        {
            if (!Regex.Match(value, "^[e,E,x,X,0-9][0-9]*$").Success)
            {
                return false;
            }
            return true;
        }
    }
}
