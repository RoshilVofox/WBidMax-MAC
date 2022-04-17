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
                StreamReader dr = ServiceUtils.GetRestData("GetVacationDifferenceData", jsonData);
                lstVacationDifferencedata = WBidCollection.ConvertJSonStringToObject<List<VacationValueDifferenceOutputDTO>>(dr.ReadToEnd());
                if (lstVacationDifferencedata.Count > 0)
                {
                    var displayData = lstVacationDifferencedata.FirstOrDefault().lstFlightDataChangeVacValues;
                }
                else
                {
                   
                    ShowMessageBox("WBidMax", "There are no differences in pay for your vacation with the new Flight Data.");
                }
            }
            else
            {
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
            var alert = new NSAlert();
            alert.MessageText = title;
            alert.InformativeText = content;
            alert.RunModal();
        }
        public new VacationDifferenceController Window
        {
            get { return (VacationDifferenceController)base.Window; }
        }
    }
}
