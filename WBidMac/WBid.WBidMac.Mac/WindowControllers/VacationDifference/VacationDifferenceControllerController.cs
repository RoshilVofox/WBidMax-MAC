using System;

using Foundation;
using AppKit;
using WBid.WBidiPad.Model;
using System.Collections.Generic;
using WBid.WBidiPad.Core;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary;
using System.Linq;

namespace WBid.WBidMac.Mac.WindowControllers.VacationDifference
{
    public partial class VacationDifferenceControllerController : NSWindowController
    {
        public List<VacationValueDifferenceOutputDTO> lstVacationDifferencedata { get; set; }
        public List<FlightDataChangeVacValues> lstFlightDataChangevalues { get; set; }
        public bool IsNeedToClose { get; set; }
        VacationValueDifferenceInputDTO input;
        public VacationDifferenceControllerController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public VacationDifferenceControllerController(NSCoder coder) : base(coder)
        {
        }

        public VacationDifferenceControllerController() : base("VacationDifferenceController")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.Window.WillClose += delegate
            {
                this.Window.OrderOut(this);
                NSApplication.SharedApplication.StopModal();
            };

            
        }

        public void GetVacationDifffrenceData()
        {
            bool isConnectionAvailable = Reachability.CheckVPSAvailable();

            if (isConnectionAvailable)
            {
                input = new VacationValueDifferenceInputDTO();

                input.BidDetails = new UserBidDetails();
                input.BidDetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                input.BidDetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                input.BidDetails.Round = GlobalSettings.CurrentBidDetails.Round == "M" ? 1 : 2;
                input.BidDetails.Year = GlobalSettings.CurrentBidDetails.Year;
                input.BidDetails.Month = GlobalSettings.CurrentBidDetails.Month;


                input.IsDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
                input.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
                input.IsVAC = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
                input.FAEOMStartDate = GlobalSettings.FAEOMStartDate.Date.Day;
                input.FromApp = (int)WBid.WBidiPad.Core.Enum.FromApp.WbidmaxMACApp;
                input.lstVacation = new List<VacationInfo>();

                //input.lstVacation.Add(new VacationInfo { Type = "FV", VacDate = "05/29-06/04" });
                var vavacation = GlobalSettings.WBidStateCollection.Vacation;
                if (vavacation != null && vavacation.Count > 0)
                {
                    foreach (var item in vavacation)
                    {
                        var startdate = Convert.ToDateTime(item.StartDate);
                        var enddate = Convert.ToDateTime(item.EndDate);
                        var vacationstring = startdate.Month + "/" + startdate.Day + "-" + enddate.Month + "/" + enddate.Day;
                        input.lstVacation.Add(new VacationInfo { Type = "VA", VacDate = vacationstring });

                    }
                }
                var Fvvavacation = GlobalSettings.WBidStateCollection.FVVacation;
                if (Fvvavacation != null && Fvvavacation.Count > 0)
                {
                    foreach (var item in Fvvavacation)
                    {
                        var vacationstring = item.StartAbsenceDate.Month + "/" + item.StartAbsenceDate.Day + "-" + item.EndAbsenceDate.Month + "/" + item.EndAbsenceDate.Day;
                        input.lstVacation.Add(new VacationInfo { Type = item.AbsenceType, VacDate = vacationstring });
                    }
                }
                var jsonData = ServiceUtils.JsonSerializer(input);
                jsonData = jsonData.Replace("\"__type\":\"VacationInfo:#WBid.WBidiPad.Model\",", "");
                StreamReader dr = ServiceUtils.GetRestData("GetVacationDifferenceData", jsonData);
                lstVacationDifferencedata = WBidCollection.ConvertJSonStringToObject<List<VacationValueDifferenceOutputDTO>>(dr.ReadToEnd());
                if (lstVacationDifferencedata.Count > 0)
                {
                    lstFlightDataChangevalues = lstVacationDifferencedata.FirstOrDefault().lstFlightDataChangeVacValues;
                    tblVacationDiff.Source = new VacationDifferenceTableSource(this);
                }
                else
                {
                    IsNeedToClose = true;
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        ShowMessageBox("WBidMax", "There are no differences in pay for your vacation with the new Flight Data.");
                    }
                    else
                    {
                       
                        //InvokeOnMainThread(() =>
                        //{
                        //    this.Window.Close();
                        //    this.Window.OrderOut(this);
                        //    NSApplication.SharedApplication.StopModal();
                        //});
                        ShowMessageBox("WBidMax", "There are no differences in pay with the new Flight Data. But if you have vacation, please turn ON vacation and check the vacation difference.");
                    }

                }
            }
            else
            {
                IsNeedToClose = true;
                string alertmessage = GlobalSettings.VPSDownAlert;
                if (Reachability.IsSouthWestWifiOr2wire())
                {
                    alertmessage = GlobalSettings.SouthWestConnectionAlert;
                }
                ShowMessageBox("WBidMax", alertmessage);
            }
        }

        private void ShowMessageBox(string title, string content)
        {
            InvokeOnMainThread(() =>
            {
            var alert = new NSAlert();
            alert.MessageText = title;
            alert.InformativeText = content;
            alert.AddButton("OK");
          
           
            alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
            {
                
                    alert.Window.Close();
                    this.Window.Close();
                    this.Window.OrderOut(this);
                    NSApplication.SharedApplication.StopModal();
                
                
            };
            alert.RunModal();
            });
        }
        public new VacationDifferenceController Window
        {
            get { return (VacationDifferenceController)base.Window; }
        }


        public partial class VacationDifferenceTableSource : NSTableViewSource
        {
            VacationDifferenceControllerController vacationdifferenceVC;

            public VacationDifferenceTableSource(VacationDifferenceControllerController show)
            {
                vacationdifferenceVC = show;
            }

            public override nint GetRowCount(NSTableView tableView)
            {
                return vacationdifferenceVC.lstFlightDataChangevalues.Count;
            }

            public override NSObject GetObjectValue(NSTableView tableView, NSTableColumn tableColumn, nint row)
            {
                if (tableColumn.Identifier == "linenum")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].LineNum.ToString();

                }
                else if (tableColumn.Identifier == "oldtotpay")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].OldTotalPay.ToString();

                }
                else if (tableColumn.Identifier == "newtotpay")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].NewTotalPay.ToString();

                }
                else if (tableColumn.Identifier == "oldvacpaycu")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].OldVPCu .ToString();

                }
                else if (tableColumn.Identifier == "newvacpaycu")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].NewVPCu.ToString();

                }
                else if (tableColumn.Identifier == "oldvacpayne")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].OldVPNe.ToString();

                }
                else if (tableColumn.Identifier == "newvacpayne")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].NewVPNe.ToString();

                }
                else if (tableColumn.Identifier == "oldvacpaybo")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].OldVPBo.ToString();

                }
                else if (tableColumn.Identifier == "newvacpaybo")
                {
                    return (NSString)vacationdifferenceVC.lstFlightDataChangevalues[(int)row].NewVPBo.ToString();

                }
                
                return new NSString();
            }

            public override void SelectionDidChange(NSNotification notification)
            {

               
            }
        }

    }
}
