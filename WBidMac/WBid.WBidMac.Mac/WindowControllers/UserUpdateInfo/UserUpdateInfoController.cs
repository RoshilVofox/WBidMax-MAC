

using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.iOS.Utility;
using System.Text.RegularExpressions;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Model;
using System.Windows;
using WBid.WBidiPad.Core;
using System.IO;
using WBid.WBidMac.Mac.WindowControllers.UserAccoutDifference;
using WBid.WBidMac.Mac.WindowControllers.LoginNew;
using System.ServiceModel;

namespace WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo
{
    public partial class UserUpdateInfoController : AppKit.NSWindowController
    {
        NSObject notif;
        WBidDataDwonloadAuthServiceClient client;
        NSPanel overlayPanel;
        OverlayViewController overlay;
        public string title = "";
        public string buttonName = "";
        public bool isRegister;
        public string authenticatedUserId;
        public UserUpdateInfoController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public UserUpdateInfoController(NSCoder coder) : base(coder)
        {
        }

        public UserUpdateInfoController() : base("UserUpdateInfo")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            UpdateButton.Activated += HandleBtnRegisterOrUpdate;
            CancelButton.Activated += HandleBtnCancel;
            ChangeEmplyeeNumButton.Activated += HandleChangeEmployee;
            this.Window.Title = title;
            UpdateButton.Title = buttonName;
            BindUserAccount();

        }
        void BindUserAccount()
        {
            try
            {
                PositionSegmentControl.SelectedSegment = 0;
                AcceptCheckButton.State = NSCellStateValue.On;
                EmployeeNumberTextField.StringValue = authenticatedUserId;
                if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null)
                {
                    UserInformation userInformation = GlobalSettings.WbidUserContent.UserInformation;
                    if (userInformation != null)
                    {
                        FirstNameTextField.StringValue = userInformation.FirstName;
                        LastNameTextField.StringValue = userInformation.LastName;
                        EmailTextField.StringValue = userInformation.Email;

                        CellPhoneTextField.StringValue = userInformation.CellNumber;

                        AcceptCheckButton.State = userInformation.AcceptMail ? NSCellStateValue.On : NSCellStateValue.Off;
                        if (userInformation.Position == "CP" || userInformation.Position == "Pilot")
                        {
                            PositionSegmentControl.SelectedSegment = 0;
                        }
                        else if (userInformation.Position == "FA")
                        {
                            PositionSegmentControl.SelectedSegment = 1;
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        void HandleBtnRegisterOrUpdate(object sender, EventArgs e)
        {
            try
            {
                overlayPanel = new NSPanel();
                overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                overlay = new OverlayViewController();
                string overlayText = "Updating";
                if (isRegister)
                    overlayText = "Creating";
                overlay.OverlayText = overlayText + " User Data...Please Wait.";
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);
                if (ValidateUI())
                {

                    WbidUser wbidUser = new WbidUser();
                    wbidUser.UserInformation = new UserInformation();
                    wbidUser.UserInformation.FirstName = FirstNameTextField.StringValue;
                    wbidUser.UserInformation.LastName = LastNameTextField.StringValue;
                    wbidUser.UserInformation.Email = EmailTextField.StringValue;
                    wbidUser.UserInformation.EmpNo = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(EmployeeNumberTextField.StringValue, @"\d+").Value).ToString();
                    if (PositionSegmentControl.SelectedSegment == 0)
                    {
                        wbidUser.UserInformation.Position = "CP";
                    }
                    else
                    {
                        wbidUser.UserInformation.Position = "FA";
                    }


                    //  wbidUser.UserInformation.IsFemale = (btnGender.SelectedTag == 1);

                    wbidUser.UserInformation.CellNumber = CellPhoneTextField.StringValue;

                    wbidUser.UserInformation.AcceptMail = AcceptCheckButton.State == NSCellStateValue.On ? true : false;
                    if (GlobalSettings.WbidUserContent != null)
                    {
                        wbidUser.RecentFiles = GlobalSettings.WbidUserContent.RecentFiles;
                    }


                    if (GlobalSettings.isVFXServer)
                        wbidUser.IsVFXServer = true;
                    GlobalSettings.WbidUserContent = wbidUser;
                    GlobalSettings.WbidUserContent.isUserFromDB = true;

                    RegistrationDetails userinfo = new RegistrationDetails();
                    userinfo.FirstName = wbidUser.UserInformation.FirstName;
                    userinfo.LastName = wbidUser.UserInformation.LastName;
                    userinfo.EmpNum = Convert.ToInt32(Regex.Match(wbidUser.UserInformation.EmpNo, @"\d+").Value);
                    userinfo.Email = wbidUser.UserInformation.Email;

                    userinfo.CellPhone = wbidUser.UserInformation.CellNumber;
                    userinfo.UserAccountDateTime = DateTime.Now;

                    if (PositionSegmentControl.SelectedSegment == 0)
                    {
                        userinfo.Position = 4;
                    }
                    else
                    {
                        userinfo.Position = 3;
                    }

                    userinfo.AcceptEmail = wbidUser.UserInformation.AcceptMail;

                    userinfo.AppNum = 4;
                    bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                    if (isConnectionAvailable)
                    {
                        if (isRegister)
                        {
                            var jsonData = ServiceUtils.JsonSerializer(userinfo);

                            string url = "CreateWbidMaxUser/";
                            StreamReader dr = ServiceUtils.GetRestData(url, jsonData);
                            WBidDataDownloadAuthorizationService.Model.CustomServiceResponse serviceResponse = ServiceUtils.ConvertJSonToObject<WBidDataDownloadAuthorizationService.Model.CustomServiceResponse>(dr.ReadToEnd());
                            if (serviceResponse.Status == true)
                            {
                                GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = serviceResponse.WBExpirationDate ?? DateTime.MinValue;
                                WBidHelper.SaveUserFile(wbidUser, WBidHelper.WBidUserFilePath);
                                InvokeOnMainThread(() =>
                                {
                                    var alert = new NSAlert();
                                    alert.Window.Title = "WBidMax";
                                    alert.MessageText = "Congratulation!! You have created an account with Wbid";
                                    alert.RunModal();

                                    this.Window.Close();
                                    this.Window.OrderOut(this);
                                    NSApplication.SharedApplication.StopModal();
                                    InvokeOnMainThread(() =>
                                    {
                                        CommonClass.HomeController.LoadContent();
                                    });
                                });
                            }
                        }
                        else
                        {
                            string empNo = Regex.Replace(EmployeeNumberTextField.StringValue, "[^0-9]+", string.Empty);

                            string url = "GetEmployeeDetails/" + empNo + "/4";

                            StreamReader dr = ServiceUtils.GetRestData(url);
                            WBidDataDownloadAuthorizationService.Model.UserInformation responseData = ServiceUtils.ConvertJSonToObject<WBidDataDownloadAuthorizationService.Model.UserInformation>(dr.ReadToEnd());
                            checkUserDetails(responseData);

                        }
                    }
                    //BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
                    //client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
                    //client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
                    //client.GetEmployeeDetailsCompleted += client_GetEmployeeDetailsAsyncCompleted;
                    //client.GetEmployeeDetailsAsync(empNo, "4");
                }
                else
                {
                    NSApplication.SharedApplication.EndSheet(overlayPanel);
                    overlayPanel.OrderOut(this);
                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        // private void client_GetEmployeeDetailsAsyncCompleted(object sender, GetEmployeeDetailsCompletedEventArgs e)
        private void checkUserDetails(WBidDataDownloadAuthorizationService.Model.UserInformation userData)
        {
            try
            {
                //if (e.Result != null && e.Result.EmpNum != 0 && e.Result.EmpNum.ToString() != string.Empty)
                if (userData != null && userData.EmpNum != 0 && userData.EmpNum.ToString() != string.Empty)
                {

                    GlobalSettings.WbidUserContent.UserInformation.IsFree = userData.IsFree;
                    GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = userData.WBExpirationDate ?? DateTime.MinValue;
                    GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed = userData.IsMonthlySubscribed;
                    GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed = userData.IsYearlySubscribed;
                    GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine = userData.SecondSubscriptionLine;
                    GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine = userData.TopSubscriptionLine;
                    GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine = userData.ThirdSubscriptionLine;
                    GlobalSettings.WbidUserContent.UserAccountDateTimeField = userData.UserAccountDateTime;
                    var diffList = WBidHelper.CheckUserInformations(userData);
                   
                    if (diffList.Count > 0)
                    {

                        NSApplication.SharedApplication.EndSheet(overlayPanel);
                        overlayPanel.OrderOut(this);
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();
                        InvokeOnMainThread(() =>
                        {


                            var userDiff = new UserAccountDifferenceController();
                            userDiff.userDiffList = diffList;
                            CommonClass.HomeController.Window.AddChildWindow(userDiff.Window, NSWindowOrderingMode.Above);
                            userDiff.Window.MakeKeyAndOrderFront(this);
                            NSApplication.SharedApplication.RunModalForWindow(userDiff.Window);

                        });
                    }
                    else
                    {

                        InvokeOnMainThread(() =>
                        {

                            CommonClass.HomeController.LoadContent();

                            NSApplication.SharedApplication.EndSheet(overlayPanel);
                            overlayPanel.OrderOut(this);
                            this.Window.Close();
                            this.Window.OrderOut(this);
                            NSApplication.SharedApplication.StopModal();

                            var alert = new NSAlert();
                            alert.Window.Title = "WBidMax";
                            alert.MessageText = "Your user account details updated successfully !!";
                            alert.RunModal();
                        });
                    }
                }


            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        void HandleBtnCancel(object sender, EventArgs e)
        {
            try
            {
                if (isRegister)
                {
                    InvokeOnMainThread(() =>
                    {
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();
                        CommonClass.HomeController.Window.Close();
                        CommonClass.HomeController.Window.OrderOut(this);

                        var userInitialLogin = new NewLoginWindowController();
                        userInitialLogin.isUserXmlFileFound = true;
                        CommonClass.UserName = "";
                        CommonClass.Password = "";
                        CommonClass.HomeController.Window.AddChildWindow(userInitialLogin.Window, NSWindowOrderingMode.Above);
                        userInitialLogin.Window.MakeKeyAndOrderFront(this);
                        NSApplication.SharedApplication.RunModalForWindow(userInitialLogin.Window);
                    });
                }
                else
                {
                    InvokeOnMainThread(() =>
                    {
                        this.Window.Close();
                        this.Window.OrderOut(this);
                        NSApplication.SharedApplication.StopModal();

                        CommonClass.HomeController.LoadContent();
                    });
                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }

        }
        void HandleChangeEmployee(object sender, EventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    this.Window.Close();
                    this.Window.OrderOut(this);
                    NSApplication.SharedApplication.StopModal();

                    var userInitialLogin = new NewLoginWindowController();
                    userInitialLogin.isUserXmlFileFound = true;
                    CommonClass.UserName = "";
                    CommonClass.Password = "";
                    CommonClass.HomeController.Window.AddChildWindow(userInitialLogin.Window, NSWindowOrderingMode.Above);
                    userInitialLogin.Window.MakeKeyAndOrderFront(this);
                    NSApplication.SharedApplication.RunModalForWindow(userInitialLogin.Window);

                });
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);

            }

        }
        public new UserUpdateInfo Window
        {
            get { return (UserUpdateInfo)base.Window; }
        }


        private bool ValidateUI()
        {
            try
            {
                bool status = true;
                string message = string.Empty;

                if (string.IsNullOrEmpty(FirstNameTextField.StringValue.Trim()))
                {
                    FirstNameTextField.BecomeFirstResponder();
                    message = "First Name required";
                    status = false;

                }
                else if (string.IsNullOrEmpty(LastNameTextField.StringValue.Trim()))
                {

                    LastNameTextField.BecomeFirstResponder();
                    message = "Last Name required";
                    status = false;
                }

                else if (string.IsNullOrEmpty(EmailTextField.StringValue.Trim()))
                {

                    EmailTextField.BecomeFirstResponder();
                    message = "Email required";
                    status = false;
                }
                else if (!RegXHandler.EmailValidation(EmailTextField.StringValue.Trim()))
                {

                    EmailTextField.BecomeFirstResponder();
                    message = "Invalid email";
                    status = false;
                }

                else if (string.IsNullOrEmpty(EmployeeNumberTextField.StringValue.Trim()))
                {

                    EmployeeNumberTextField.BecomeFirstResponder();
                    message = "Employee number required";
                    status = false;
                }
                else if (!RegXHandler.EmployeeNumberValidation(EmployeeNumberTextField.StringValue.Trim()))
                {

                    EmployeeNumberTextField.BecomeFirstResponder();
                    message = "Invalid Employee number ";
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
            catch (Exception ex)
            {
                throw ex;
            }

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
