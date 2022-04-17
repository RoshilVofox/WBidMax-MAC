using System;

using Foundation;
using AppKit;
using System.Collections.Generic;
using WBid.WBidiPad.Core;
using System.Reflection;
using System.ServiceModel;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidMac.Mac.WindowControllers.UserAccoutDifference
{
    public partial class UserAccountDifferenceController : NSWindowController
    {
        public List<KeyValuePair<string, string>> userDiffList = new List<KeyValuePair<string, string>>();
        WBidDataDwonloadAuthServiceClient client;
        WBidDataDownloadAuthorizationService.Model.UserInformation userInformation;
        NSPanel overlayPanel;
        OverlayViewController overlay;
        public UserAccountDifferenceController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public UserAccountDifferenceController(NSCoder coder) : base(coder)
        {
        }

        public UserAccountDifferenceController() : base("UserAccountDifference")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            CancelButton.Activated += HandleCancelButton;
            UpdateButton.Activated += HandleUpdateButton;
            updateInfoTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;
            LoadUserData();
            BindDiffData();
        }

        public new UserAccountDifference Window
        {
            get { return (UserAccountDifference)base.Window; }
        }


        void HandleCancelButton(object sender, EventArgs e)
        {
            try
            {

                this.Window.Close();
                this.Window.OrderOut(this);
                NSApplication.SharedApplication.StopModal();
                if (!CommonClass.HomeController.IsWindowLoaded)
                    InvokeOnMainThread(() =>
                    {
                        CommonClass.HomeController.LoadContent();
                    });
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        void HandleUpdateButton(object sender, EventArgs e)
        {
            try
            {
                overlayPanel = new NSPanel();
                overlayPanel.SetContentSize(new CoreGraphics.CGSize(400, 120));
                overlay = new OverlayViewController();
                overlay.OverlayText = "Updating User Data...Please Wait..";
                overlayPanel.ContentView = overlay.View;
                NSApplication.SharedApplication.BeginSheet(overlayPanel, this.Window);
                getStatusOfCell();
                InvokeOnMainThread(() =>
                {

                    if (userInformation != null)
                        UpdateUserAccount();
                });


            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        public void BindDiffData()
        {
            try
            {

                updateInfoTableView.Source = new UserAccountDiffTableSource(this);


            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        void LoadUserData()
        {
            try
            {
                userInformation = new WBidDataDownloadAuthorizationService.Model.UserInformation();

                userInformation.AcceptEmail = GlobalSettings.WbidUserContent.UserInformation.AcceptMail;
                userInformation.FirstName = GlobalSettings.WbidUserContent.UserInformation.FirstName;
                userInformation.LastName = GlobalSettings.WbidUserContent.UserInformation.LastName;
                userInformation.Email = GlobalSettings.WbidUserContent.UserInformation.Email;
                userInformation.CellPhone = GlobalSettings.WbidUserContent.UserInformation.CellNumber;
                userInformation.EmpNum = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToString());

                if (GlobalSettings.WbidUserContent.UserInformation.Position == "CP" || GlobalSettings.WbidUserContent.UserInformation.Position == "FO" || GlobalSettings.WbidUserContent.UserInformation.Position == "Pilot")
                {
                    userInformation.Position = 4;
                }

                else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
                {
                    userInformation.Position = 3;
                }
                userInformation.BidBase = GlobalSettings.WbidUserContent.UserInformation.Domicile;


            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }
        public void getStatusOfCell()
        {
            try
            {
                for (int i = 0; i < userDiffList.Count; i++)
                {
                    var vw = (AccountCell)updateInfoTableView.GetView(0, i, true);
                    var selecetedSegment = vw.parameterSegment.GetLabel(vw.parameterSegment.SelectedSegment);
                    var selectedProperty = vw.parameter.StringValue;
                    PropertyInfo propertyInfo = userInformation.GetType().GetProperty(selectedProperty);
                    if (selectedProperty == "Position")
                    {
                        int selectedSegmentValue = 0;
                        if (selecetedSegment == "CP" || selecetedSegment == "FO" || selecetedSegment == "Pilot")
                        {
                            selectedSegmentValue = 4;
                        }

                        else if (GlobalSettings.WbidUserContent.UserInformation.Position == "FA")
                        {
                            selectedSegmentValue = 3;
                        }
                        propertyInfo.SetValue(userInformation, Convert.ChangeType(selectedSegmentValue, propertyInfo.PropertyType), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(userInformation, Convert.ChangeType(selecetedSegment, propertyInfo.PropertyType), null);
                    }

                }
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }

        void UpdateUserAccount()
        {
            try
            {
                userInformation.UserAccountDateTime = DateTime.Now;
                createUserData();

                var data = ServiceUtils.JsonSerializer(userInformation);


                StreamReader dr = ServiceUtils.GetRestData("UpdateWBidUserDetails", data);
                CustomServiceResponse responsedata = ServiceUtils.ConvertJSonToObject<CustomServiceResponse>(dr.ReadToEnd());
                var alert = new NSAlert();
                alert.Window.Title = "WBidMax";

                
                if (responsedata.Status)
                {
                    
                    InvokeOnMainThread(() =>
                    {
                        CommonClass.HomeController.LoadContent();
                    });
                    this.Window.Close();
                    this.Window.OrderOut(this);
                    overlayPanel.OrderOut(this);
                    NSApplication.SharedApplication.StopModal();
                    NSApplication.SharedApplication.EndSheet(overlayPanel);

                    alert.MessageText = "Success";
                    alert.InformativeText = "Your user account updated successfully!!";
                    alert.RunModal();
                }
                else
                {
                    this.Window.Close();
                    this.Window.OrderOut(this);
                    overlayPanel.OrderOut(this);
                    NSApplication.SharedApplication.StopModal();
                    NSApplication.SharedApplication.EndSheet(overlayPanel);

                    alert.MessageText = "Failed";
                    alert.InformativeText = "Something went wrong.Please try again";
                    alert.RunModal();
                }
                
            }
            catch (Exception ex)
            {
                NSApplication.SharedApplication.EndSheet(overlayPanel);
                overlayPanel.OrderOut(this);
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);

            }

        }

        void createUserData()
        {
            try
            {
                WbidUser wbidUser = new WbidUser();
                wbidUser.UserInformation = new UserInformation();
                wbidUser.UserInformation.FirstName = userInformation.FirstName;
                wbidUser.UserInformation.LastName = userInformation.LastName;
                wbidUser.UserInformation.Email = userInformation.Email;
                wbidUser.UserInformation.EmpNo = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(userInformation.EmpNum.ToString(), @"\d+").Value).ToString();
                if (userInformation.Position == 4)

                    wbidUser.UserInformation.Position = "Pilot";
                else
                    wbidUser.UserInformation.Position = "FA";
                wbidUser.UserInformation.CellNumber = userInformation.CellPhone;

                wbidUser.UserInformation.AcceptMail = userInformation.AcceptEmail;
                if (GlobalSettings.WbidUserContent != null)
                {
                    wbidUser.RecentFiles = GlobalSettings.WbidUserContent.RecentFiles;
                }


                if (GlobalSettings.isVFXServer)
                    wbidUser.IsVFXServer = true;

                wbidUser.UserAccountDateTimeField = DateTime.Now;
                GlobalSettings.WbidUserContent = wbidUser;
                GlobalSettings.WbidUserContent.isUserFromDB = true;

                WBidHelper.SaveUserFile(wbidUser, WBidHelper.WBidUserFilePath);
            }
            catch (Exception ex)
            {
                CommonClass.AppDelegate.ErrorLog(ex);
                CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
            }
        }


        public static byte[] ObjectToByteArray(WBidDataDownloadAuthorizationService.Model.UserInformation obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public partial class UserAccountDiffTableSource : NSTableViewSource
        {
            UserAccountDifferenceController parentWC;
            public UserAccountDiffTableSource(UserAccountDifferenceController parent)
            {
                parentWC = parent;
            }

            public override nint GetRowCount(NSTableView tableView)
            {
                return parentWC.userDiffList.Count;
            }
            public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
            {

                var vw = (AccountCell)tableView.MakeView("acntDif", this);

                try
                {
                    string[] values = parentWC.userDiffList[(int)row].Value.Split(',');

                    if (parentWC.userDiffList[(int)row].Key == "CellPhone")
                    {
                        vw.BindData(parentWC.userDiffList[(int)row].Key, values[0], values[1]);
                    }
                    else if (parentWC.userDiffList[(int)row].Key == "Position")
                    {
                        vw.BindData(parentWC.userDiffList[(int)row].Key, values[0], values[1]);
                    }
                    else if (parentWC.userDiffList[(int)row].Key == "FirstName")
                    {
                        vw.BindData(parentWC.userDiffList[(int)row].Key, values[0], values[1]);
                    }
                    else if (parentWC.userDiffList[(int)row].Key == "LastName")
                    {
                        vw.BindData(parentWC.userDiffList[(int)row].Key, values[0], values[1]);
                    }
                    else if (parentWC.userDiffList[(int)row].Key == "Email")
                    {
                        vw.BindData(parentWC.userDiffList[(int)row].Key, values[0], values[1]);
                    }

                }
                catch (Exception ex)
                {
                    CommonClass.AppDelegate.ErrorLog(ex);
                    CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
                }
                return vw;

            }

            public override void SelectionDidChange(NSNotification notification)
            {
                var table = (NSTableView)notification.Object;

            }
            public override nfloat GetRowHeight(NSTableView tableView, nint row)
            {
                return 30;
            }

        }
    }
}