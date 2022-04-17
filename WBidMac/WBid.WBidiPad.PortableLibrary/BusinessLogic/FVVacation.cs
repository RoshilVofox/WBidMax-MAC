using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class FVVacation
    {
        public FVVacation()
        {
        }
        List<FVvacationLineData> FVvacationLineDatalist;
        public Dictionary<string, TripMultiVacData> VacationData { get; set; }
        public List<Line> SetCFVValuesForAllLines(List<Line> lines)
        {
            //temporary code to test CFV vacation
            //            var CFVabsence1 = new List<Absense>() { new Absense { AbsenceType = "CFV", StartAbsenceDate = new DateTime(2020, 12, 2) },
            //new Absense { AbsenceType = "CFV", StartAbsenceDate = new DateTime(2020, 12, 28) }
            //};
            //            GlobalSettings.WBidStateCollection.FVVacation.AddRange(CFVabsence1);
            List<Absense> CFVabsence = GlobalSettings.FVVacation.Where(x => x.AbsenceType == "CFV").ToList();
            decimal onedaycfvPay = 26.25m;
            foreach (var line in lines)
            {
                line.CFVDates = new List<string>();
                line.CFVPay = onedaycfvPay * CFVabsence.Count;
                var gettripdays = line.Pairings.Select(x => x.Substring(4, 2));
                foreach (var item in CFVabsence)
                {
                    var originalCBvdate = item.StartAbsenceDate.Date;
                }

            }
            return lines;
        }

        public List<Line> SetFVVacationValuesForAllLines(List<Line> lines, Dictionary<string, TripMultiVacData> _vacationData)
        {
            try
            {
                VacationData = _vacationData;
                List<Absense> absence = GlobalSettings.FVVacation.Where(x=>x.AbsenceType=="FV").ToList();
                if (absence.Count > 0)
                {

                    //absence.Add(new Absense { StartAbsenceDate = new DateTime(2018, 8, 5), EndAbsenceDate = new DateTime(2018, 8, 11) });
                    foreach (var line in lines)
                    {
                        FVvacationLineDatalist = new List<FVvacationLineData>();
                        if (line.FVvacationData == null)
                            line.FVvacationData = new List<FVvacationLineData>();
                        //get the trips related to the FV vacation
                        List<FVVacationData> TripsinsideVacation = getTripsInsidetheFVVacation(line, absence);

                        foreach (var Fvdata in TripsinsideVacation)
                        {
                            FVvacationLineData FVvacationLineData = new FVvacationLineData();
                            // if (line.ReserveLine == false)
                            // {
                            if (Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftVacation") && Fvdata.FVVacationTripDatas.Any(x => x.Type == "RightVacation"))
                            {
                                //if normal vacation conflicts on both side of the FV vacation
                                //VA 8 - 14,FV - 15 - 21,VA 22 - 28 like this. we need to give priority to normal vacation and show FV on the remanining days. But we need to show 26.25 vac tfp
                                var leftvaction = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "LeftVacation");
                                var rightvaction = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "RightVacation");
                                if ((leftvaction.TripEndDate - rightvaction.TripStartDate).Days < 7)
                                {
                                    //FVvacationLineData.FVStartDate = leftvaction.TripEndDate.AddDays(1);
                                    //FVvacationLineData.FVEndDate = rightvaction.TripStartDate.AddDays(-1);
                                    FVvacationLineData.FVStartDate = Fvdata.StartDate;
                                    FVvacationLineData.FVEndDate = Fvdata.EndDate;
                                    FVvacationLineData.FVVacTfp = 7 * GlobalSettings.DailyVacPay;
                                    FVvacationLineData.VDTfpDrop = leftvaction.VDBackTfpDrop + rightvaction.VDFrontTfpDrop;
                                    FVvacationLineData.VOTfpDrop = leftvaction.VOBackTfpDrop + rightvaction.VOFrontTfpDrop;
                                    FVvacationLineData.VaVacationDropDays = leftvaction.VaVacationDropDays + rightvaction.VaVacationDropDays;
                                    FVvacationLineData.FVVacationTripDatas = new List<FVVacationTripData>();
                                    line.VacationStateLine.VacationFront = line.VacationStateLine.VacationFront - rightvaction.VOFrontTfpDrop;
                                    line.VacationStateLine.VacationBack = line.VacationStateLine.VacationBack - leftvaction.VOBackTfpDrop;

                                }
                            }
                            else if (Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftVacation"))
                            {
                                // if the noraml vacation conflits on the left side of the FV vacation. In this case we need to only pull the FV vaction to right and check the conflicts on the right .
                                var leftvaction = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "LeftVacation");
                                int leftdayspull = 0;

                                int rightDaysPull = (leftvaction.TripEndDate - Fvdata.StartDate).Days + 1;
                                FVvacationLineData = CalculateFVVacationValues(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                                bool isAdjacentVacationExistOnRightSide = GlobalSettings.OrderedVacationDays.Any(x => x.StartAbsenceDate == Fvdata.EndDate.AddDays(1));
                                if (isAdjacentVacationExistOnRightSide)
                                {
                                    FVvacationLineData.VaVacationDropDays = leftvaction.VaVacationDropDays;
                                    FVvacationLineData.VDTfpDrop = leftvaction.VDBackTfpDrop;
                                    FVvacationLineData.VOTfpDrop = leftvaction.VOBackTfpDrop;
                                    FVvacationLineData.FVStartDate = Fvdata.StartDate;
                                    FVvacationLineData.FVEndDate = Fvdata.EndDate;
                                    line.VacationStateLine.VacationBack = line.VacationStateLine.VacationBack - leftvaction.VOBackTfpDrop;
                                }
                            }
                            else if (Fvdata.FVVacationTripDatas.Any(x => x.Type == "RightVacation"))
                            {
                                // if the noraml vacation conflits on the Right side of the FV vacation. In this case we need to only pull the FV vaction to left and check the conflicts on the left .
                                var rightvaction = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "RightVacation");
                                int leftdayspull = (Fvdata.EndDate - rightvaction.TripStartDate).Days + 1;
                                int rightDaysPull = 0;
                                FVvacationLineData = CalculateFVVacationValues(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                                bool isAdjacentVacationExistOnLeftSide = GlobalSettings.OrderedVacationDays.Any(x => x.EndAbsenceDate == Fvdata.StartDate.AddDays(-1));
                                if (isAdjacentVacationExistOnLeftSide)
                                {
                                    FVvacationLineData.FVStartDate = Fvdata.StartDate;
                                    FVvacationLineData.FVEndDate = Fvdata.EndDate;
                                    FVvacationLineData.VaVacationDropDays = rightvaction.VaVacationDropDays;
                                    FVvacationLineData.VDTfpDrop = rightvaction.VDFrontTfpDrop;
                                    FVvacationLineData.VOTfpDrop = rightvaction.VOFrontTfpDrop;
                                    line.VacationStateLine.VacationFront = line.VacationStateLine.VacationFront - rightvaction.VOFrontTfpDrop;
                                }
                            }
                            // we need to check the if there is a right float and left there. if so we need to get the minimum pay of those trips
                            // else if ( Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftConflict") || (Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftConflict")) && (Fvdata.FVVacationTripDatas.Any(x => x.Type == "RightConflict")))
                            else if (Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftConflict") && (Fvdata.FVVacationTripDatas.Any(x => x.Type == "RightConflict")))
                            {

                                var leftConflictTrip = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "LeftConflict");
                                int leftdayspull = (Fvdata.StartDate - leftConflictTrip.TripStartDate).Days;
                                int rightDaysPull = (leftConflictTrip.TripEndDate - Fvdata.StartDate).Days + 1;
                                FVvacationLineData = CalculateFVVacationValues(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                            }
                            else if ((Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftConflict")) && (Fvdata.FVVacationTripDatas.Any(x => x.Type == "Float")))
                            {

                                var leftConflictTrip = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "LeftConflict");
                                int leftdayspull = (Fvdata.StartDate - leftConflictTrip.TripStartDate).Days;
                                int rightDaysPull = (leftConflictTrip.TripEndDate - Fvdata.StartDate).Days + 1;
                                FVvacationLineData = CalculateFVVacationValues(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                            }
                            else if ((Fvdata.FVVacationTripDatas.Any(x => x.Type == "RightConflict")) && (Fvdata.FVVacationTripDatas.Any(x => x.Type == "Float")))
                            {

                                // var leftConflictTrip = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "RightConflict");
                                var rightConflictTrip = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "RightConflict");
                                int leftdayspull = (Fvdata.EndDate - rightConflictTrip.TripStartDate).Days + 1;
                                int rightDaysPull = (rightConflictTrip.TripEndDate - Fvdata.EndDate).Days;
                                FVvacationLineData = CalculateFVVacationValues(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                            }
                            //there is no left float or right float. Means there is no conflicts. In this case we need to drop all the trips inside FV vacation periods.
                            else if (!Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftConflict" || x.Type == "RightConflict"))
                            {
                                FVvacationLineData = SetValuesForFVVacations(line, Fvdata, ref FVvacationLineData);
                            }
                            else if (Fvdata.FVVacationTripDatas.Any(x => x.Type == "LeftConflict"))
                            {
                                // found there is a conflicts on the left side. so we need to pull the left trip and check the conflicts solved or not
                                var leftConflictTrip = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "LeftConflict");
                                int leftdayspull = (Fvdata.StartDate - leftConflictTrip.TripStartDate).Days;
                                int rightDaysPull = (leftConflictTrip.TripEndDate - Fvdata.StartDate).Days + 1;

                                // CalculateFVVacationValues(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                                FVvacationLineData = SolveLeftSideConflicts(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                            }
                            else if (Fvdata.FVVacationTripDatas.Any(x => x.Type == "RightConflict"))
                            {
                                // found there is a conflicts on the Right side. so we need to pull the right trip and check the conflicts solved or not
                                var rightConflictTrip = Fvdata.FVVacationTripDatas.FirstOrDefault(x => x.Type == "RightConflict");
                                int leftdayspull = (Fvdata.EndDate - rightConflictTrip.TripStartDate).Days + 1;
                                int rightDaysPull = (rightConflictTrip.TripEndDate - Fvdata.EndDate).Days;

                                //CalculateFVVacationValues(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                                FVvacationLineData = SolveRightSideConflicts(line, Fvdata, FVvacationLineData, leftdayspull, rightDaysPull);
                            }

                            FVvacationLineDatalist.Add(FVvacationLineData);
                            //}
                            //else
                            //{
                            //    //reserve lines
                            //    FVvacationLineData = SetValuesForFVVacations(line, Fvdata, ref FVvacationLineData);
                            //    FVvacationLineDatalist.Add(FVvacationLineData);

                            //}
                        }
                        foreach (var item in absence)
                        {
                            var tripsInEachAbsence = TripsinsideVacation.FirstOrDefault(x => x.StartDate == item.StartAbsenceDate && x.EndDate == item.EndAbsenceDate);
                            if (tripsInEachAbsence == null)
                            {
                                var startdate = item.StartAbsenceDate;
                                var enddate = item.EndAbsenceDate;
                                // it means there is no trips insidet the new FV period and we need to give full VAP on these dates
                                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);
                                // line.FVVaPList.AddRange(CalulateVAPForoneFVVacationPeriod(null, startdate, enddate, ((enddate - startdate).Days + 1) * GlobalSettings.DailyVacPay));

                            }
                        }
                        //Added the below condition on 6-8-2019
                        // when a FV vacation float left due to the vacation on the right and there is no trips inside the FV vacation after moved to it.
                        var vacationeffectedMove = FVvacationLineDatalist.Where(x => x.FVVacationTripDatas == null);
                        if (vacationeffectedMove != null)
                        {
                            foreach (var item in vacationeffectedMove)
                            {
                                var startdate = item.FVStartDate;
                                var enddate = item.FVEndDate;
                                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);
                            }
                        }
                        //=================
                        if (FVvacationLineDatalist.Any(x => x.FVVacationTripDatas != null))
                        {
                            line.FVvacationData.AddRange(FVvacationLineDatalist);
                        }
                    }
                }
                SetCFVValuesForAllLines(lines);
                return lines;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void SetFVValuesForEmptyTripsWithinFVVacation(Line line, DateTime startdate, DateTime enddate)
        {
            FVvacationLineData Fvdata = new FVvacationLineData();
            if (enddate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
            {

                Fvdata.FVStartDate = startdate;
                Fvdata.FVEndDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
                Fvdata.FVVacTfp = ((GlobalSettings.CurrentBidDetails.BidPeriodEndDate - startdate).Days + 1) * GlobalSettings.DailyVacPay;
                Fvdata.FVVacationTripDatas = new List<FVVacationTripData>();
                line.FVvacationData.Add(Fvdata);
                Fvdata = new FVvacationLineData();
                Fvdata.FVStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1);
                Fvdata.FVEndDate = enddate;
                Fvdata.FVVacTfp = ((enddate - GlobalSettings.CurrentBidDetails.BidPeriodEndDate).Days) * GlobalSettings.DailyVacPay;
                Fvdata.FVVacationTripDatas = new List<FVVacationTripData>();
                line.FVvacationData.Add(Fvdata);
            }
            else
            {
                Fvdata.FVStartDate = startdate;
                Fvdata.FVEndDate = enddate;
                //Fvdata.FVVacTfp = ((enddate - startdate).Days + 1) * GlobalSettings.DailyVacPay;
                if (Fvdata.FVStartDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
                    Fvdata.FVVacTfp = ((enddate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1) * GlobalSettings.DailyVacPay;
                else
                    Fvdata.FVVacTfp = ((enddate - startdate).Days + 1) * GlobalSettings.DailyVacPay;
                Fvdata.FVVacationTripDatas = new List<FVVacationTripData>();
                line.FVvacationData.Add(Fvdata);
            }
        }

        private FVvacationLineData SolveLeftSideConflicts(Line line, FVVacationData Fvdata, FVvacationLineData FVvacationLineData, int leftdayspull, int rightDaysPull)
        {
            bool isLeftConflict = false;
            bool isLeftSideVacation = false;
            List<Absense> LeftnewFVFdate = new List<Absense>();
            LeftnewFVFdate = new List<Absense>() { (new Absense { StartAbsenceDate = Fvdata.StartDate.AddDays(-leftdayspull), EndAbsenceDate = Fvdata.EndDate.AddDays(-leftdayspull) }) };
            PairingDetails pairingdetail = InAnyConflictsExists(line, LeftnewFVFdate);
            isLeftConflict = pairingdetail.IsAnyConflicts;
            isLeftSideVacation = pairingdetail.IsLeftVacationTrip;
            while (isLeftConflict && isLeftSideVacation == false)
            {
                //LeftnewFVFdate.ForEach(x => x.StartAbsenceDate = x.StartAbsenceDate.AddDays(-1));
                // LeftnewFVFdate.ForEach(x => x.EndAbsenceDate = x.EndAbsenceDate.AddDays(-1));
                foreach (var item in LeftnewFVFdate)
                {
                    item.StartAbsenceDate = item.StartAbsenceDate.AddDays(-1);
                    item.EndAbsenceDate = item.EndAbsenceDate.AddDays(-1);
                }
                pairingdetail = InAnyConflictsExists(line, LeftnewFVFdate);
                isLeftConflict = pairingdetail.IsAnyConflicts;
                isLeftSideVacation = pairingdetail.IsLeftVacationTrip;
            }
            List<FVVacationData> lefttripsinside = getTripsInsidetheFVVacation(line, LeftnewFVFdate);
            if (lefttripsinside.Count != 0)
            {
                FVvacationLineData = SetValuesForFVVacations(line, lefttripsinside.FirstOrDefault(), ref FVvacationLineData);
            }
            else
            {
                var startdate = LeftnewFVFdate.FirstOrDefault().StartAbsenceDate;
                var enddate = LeftnewFVFdate.FirstOrDefault().EndAbsenceDate;
                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);
            }
            return FVvacationLineData;
        }
        private FVvacationLineData SolveRightSideConflicts(Line line, FVVacationData Fvdata, FVvacationLineData FVvacationLineData, int leftdayspull, int rightDaysPull)
        {
            List<Absense> RightnewFVFdate = new List<Absense>();
            bool isRightConflict = false;
            bool isRightSideVacation = false;
            RightnewFVFdate.Add(new Absense { StartAbsenceDate = Fvdata.StartDate.AddDays(rightDaysPull), EndAbsenceDate = Fvdata.EndDate.AddDays(rightDaysPull) });
            PairingDetails pairingdetail = InAnyConflictsExists(line, RightnewFVFdate);
            isRightConflict = pairingdetail.IsAnyConflicts;
            isRightSideVacation = pairingdetail.IsRightVacationTrip;

            while (isRightConflict && isRightSideVacation == false)
            {
                // RightnewFVFdate.ForEach(x => x.StartAbsenceDate = x.StartAbsenceDate.AddDays(+1));
                // RightnewFVFdate.ForEach(x => x.EndAbsenceDate = x.EndAbsenceDate.AddDays(+1));

                foreach (var item in RightnewFVFdate)
                {
                    item.StartAbsenceDate = item.StartAbsenceDate.AddDays(+1);
                    item.EndAbsenceDate = item.EndAbsenceDate.AddDays(+1);
                }

                pairingdetail = InAnyConflictsExists(line, RightnewFVFdate);
                isRightConflict = pairingdetail.IsAnyConflicts;
                isRightSideVacation = pairingdetail.IsRightVacationTrip;
            }

            List<FVVacationData> Righttripsinside = getTripsInsidetheFVVacation(line, RightnewFVFdate);
            if (Righttripsinside.Count != 0)
            {
                FVvacationLineData = SetValuesForFVVacations(line, Righttripsinside.FirstOrDefault(), ref FVvacationLineData);
            }
            else
            {
                var startdate = RightnewFVFdate.FirstOrDefault().StartAbsenceDate;
                var enddate = RightnewFVFdate.FirstOrDefault().EndAbsenceDate;
                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);
            }
            return FVvacationLineData;
        }
        private FVvacationLineData CalculateFVVacationValues(Line line, FVVacationData Fvdata, FVvacationLineData FVvacationLineData, int leftdayspull, int rightDaysPull)
        {
            try
            {
                List<Absense> RightnewFVFdate = new List<Absense>();
                List<Absense> LeftnewFVFdate = new List<Absense>();
                bool isLeftConflict = false;
                bool isRightConflict = false;
                bool isLeftSideVacation = false;
                bool isRightSideVacation = false;
                int RightDaysPulled = 0;
                int LeftDaysPulled = 0;
                decimal RightSideFVPayPulled = 0;
                decimal LeftSideFVPayPulled = 0;

                //pull to right side and check for conflicts
                RightnewFVFdate.Add(new Absense { StartAbsenceDate = Fvdata.StartDate.AddDays(rightDaysPull), EndAbsenceDate = Fvdata.EndDate.AddDays(rightDaysPull) });
                PairingDetails pairingdetail = InAnyConflictsExists(line, RightnewFVFdate);
                isRightConflict = pairingdetail.IsAnyConflicts;
                isRightSideVacation = pairingdetail.IsRightVacationTrip;
                while (isRightConflict && isRightSideVacation == false)
                {
                    //RightnewFVFdate.ForEach(x => x.StartAbsenceDate = x.StartAbsenceDate.AddDays(+1));
                    //RightnewFVFdate.ForEach(x => x.EndAbsenceDate = x.EndAbsenceDate.AddDays(+1));
                    foreach (var item in RightnewFVFdate)
                    {
                        item.StartAbsenceDate = item.StartAbsenceDate.AddDays(+1);
                        item.EndAbsenceDate = item.EndAbsenceDate.AddDays(+1);
                    }
                    pairingdetail = InAnyConflictsExists(line, RightnewFVFdate);
                    isRightConflict = pairingdetail.IsAnyConflicts;
                    isRightSideVacation = pairingdetail.IsRightVacationTrip;
                }
                RightDaysPulled = (RightnewFVFdate.FirstOrDefault().StartAbsenceDate - Fvdata.StartDate).Days;
                List<FVVacationData> Righttripsinside = getTripsInsidetheFVVacation(line, RightnewFVFdate);
                RightSideFVPayPulled = Righttripsinside.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripTfpInLine);

                //pull to left side and check for conflicts
                LeftnewFVFdate = new List<Absense>() { (new Absense { StartAbsenceDate = Fvdata.StartDate.AddDays(-leftdayspull), EndAbsenceDate = Fvdata.EndDate.AddDays(-leftdayspull) }) };
                pairingdetail = InAnyConflictsExists(line, LeftnewFVFdate);
                isLeftConflict = pairingdetail.IsAnyConflicts;
                isLeftSideVacation = pairingdetail.IsLeftVacationTrip;
                while (isLeftConflict && isLeftSideVacation == false)
                {
                    //LeftnewFVFdate.ForEach(x => x.StartAbsenceDate = x.StartAbsenceDate.AddDays(-1));
                    //LeftnewFVFdate.ForEach(x => x.EndAbsenceDate = x.EndAbsenceDate.AddDays(-1));

                    foreach (var item in LeftnewFVFdate)
                    {
                        item.StartAbsenceDate = item.StartAbsenceDate.AddDays(-1);
                        item.EndAbsenceDate = item.EndAbsenceDate.AddDays(-1);
                    }
                    pairingdetail = InAnyConflictsExists(line, LeftnewFVFdate);
                    isLeftConflict = pairingdetail.IsAnyConflicts;
                    isLeftSideVacation = pairingdetail.IsLeftVacationTrip;
                }
                LeftDaysPulled = (Fvdata.StartDate - LeftnewFVFdate.FirstOrDefault().StartAbsenceDate).Days;
                List<FVVacationData> lefttripsinside = getTripsInsidetheFVVacation(line, LeftnewFVFdate);
                LeftSideFVPayPulled = lefttripsinside.SelectMany(x => x.FVVacationTripDatas).Sum(y => y.TripTfpInLine);

                if (isLeftSideVacation)
                {
                    //if there is a vacation on the left side, we dont need to pull the FV vaction to left.
                    if (Righttripsinside.FirstOrDefault() == null)
                    {
                        Righttripsinside = new List<FVVacationData>();
                        Righttripsinside.Add(new FVVacationData { StartDate = RightnewFVFdate.FirstOrDefault().StartAbsenceDate, EndDate = RightnewFVFdate.FirstOrDefault().EndAbsenceDate,FVVacationTripDatas=new List<FVVacationTripData>() });

                    }
                    FVvacationLineData = SetValuesForFVVacations(line, Righttripsinside.FirstOrDefault(), ref FVvacationLineData);
                }
                else if (isRightSideVacation)
                {
                    if (lefttripsinside.FirstOrDefault() == null)
                    {
                        lefttripsinside = new List<FVVacationData>();
                        lefttripsinside.Add(new FVVacationData { StartDate = LeftnewFVFdate.FirstOrDefault().StartAbsenceDate, EndDate = LeftnewFVFdate.FirstOrDefault().EndAbsenceDate,FVVacationTripDatas=new List<FVVacationTripData>() });
                    }
                    FVvacationLineData = SetValuesForFVVacations(line, lefttripsinside.FirstOrDefault(), ref FVvacationLineData);
                }
                else
                {
                    if (line.ReserveLine)
                    {
                        if (RightDaysPulled < LeftDaysPulled)
                        {
                            if (Righttripsinside.Count != 0)
                            {
                                FVvacationLineData = SetValuesForFVVacations(line, Righttripsinside.FirstOrDefault(), ref FVvacationLineData);
                            }
                            else
                            {
                                var startdate = RightnewFVFdate.FirstOrDefault().StartAbsenceDate;
                                var enddate = RightnewFVFdate.FirstOrDefault().EndAbsenceDate;
                                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);
                            }

                        }
                        else
                        {
                            if (lefttripsinside.Count != 0)
                            {
                                FVvacationLineData = SetValuesForFVVacations(line, lefttripsinside.FirstOrDefault(), ref FVvacationLineData);
                            }
                            else
                            {
                                var startdate = LeftnewFVFdate.FirstOrDefault().StartAbsenceDate;
                                var enddate = LeftnewFVFdate.FirstOrDefault().EndAbsenceDate;
                                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);

                            }
                        }
                    }
                    else
                    {
                        if (RightSideFVPayPulled <= LeftSideFVPayPulled)
                        {
                            if (Righttripsinside.Count != 0)
                            {
                                FVvacationLineData = SetValuesForFVVacations(line, Righttripsinside.FirstOrDefault(), ref FVvacationLineData);
                            }
                            else
                            {
                                var startdate = RightnewFVFdate.FirstOrDefault().StartAbsenceDate;
                                var enddate = RightnewFVFdate.FirstOrDefault().EndAbsenceDate;
                                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);
                                //line.FVVaPList.AddRange(CalulateVAPForoneFVVacationPeriod(null, startdate, enddate, ((enddate-startdate).Days+1) * GlobalSettings.DailyVacPay));
                                // it means there is no trips insidet the new FV period and we need to give full VAP on these dates
                                // RightnewFVFdate.ForEach(x => line.FVVaPList.Add(new FVVAP { StartDate = x.StartAbsenceDate, EndDate = x.EndAbsenceDate, Vap = ((x.EndAbsenceDate - x.StartAbsenceDate).Days + 1) * GlobalSettings.DailyVacPay }));
                            }

                        }
                        else
                        {
                            if (lefttripsinside.Count != 0)
                            {
                                FVvacationLineData = SetValuesForFVVacations(line, lefttripsinside.FirstOrDefault(), ref FVvacationLineData);
                            }
                            else
                            {
                                var startdate = LeftnewFVFdate.FirstOrDefault().StartAbsenceDate;
                                var enddate = LeftnewFVFdate.FirstOrDefault().EndAbsenceDate;
                                SetFVValuesForEmptyTripsWithinFVVacation(line, startdate, enddate);
                                // it means there is no trips insidet the new FV period and we need to give full VAP on these dates
                                //line.FVVaPList.AddRange(CalulateVAPForoneFVVacationPeriod(null, startdate, enddate, ((enddate - startdate).Days + 1) * GlobalSettings.DailyVacPay));
                                //LeftnewFVFdate.ForEach(x => line.FVVaPList.Add(new FVVAP { StartDate = x.StartAbsenceDate, EndDate = x.EndAbsenceDate, Vap = ((x.EndAbsenceDate - x.StartAbsenceDate).Days + 1) * GlobalSettings.DailyVacPay }));
                            }
                        }
                    }
                }
                return FVvacationLineData;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        private PairingDetails InAnyConflictsExists(Line line, List<Absense> absence)
        {
            PairingDetails pairingdetail = new PairingDetails();
            List<FVVacationData> tripsinside = getTripsInsidetheFVVacation(line, absence);
            pairingdetail.IsAnyConflicts = tripsinside.SelectMany(x => x.FVVacationTripDatas).Any(x => x.Type == "LeftConflict" || x.Type == "RightConflict");
            pairingdetail.IsLeftVacationTrip = tripsinside.SelectMany(x => x.FVVacationTripDatas).Any(x => x.Type == "LeftVacation");
            pairingdetail.IsRightVacationTrip = tripsinside.SelectMany(x => x.FVVacationTripDatas).Any(x => x.Type == "RightVacation");
            return pairingdetail;
        }
        private FVvacationLineData SetValuesForFVVacations(Line line, FVVacationData Fvdata, ref FVvacationLineData FVvacationLineData)
        {


            if (Fvdata.EndDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
            {
                //need to handle the FV period overlap in to the next bid period,
                FVvacationLineData.FVStartDate = Fvdata.StartDate;
                FVvacationLineData.FVEndDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate; ;
                FVvacationLineData.FVVacationTripDatas = Fvdata.FVVacationTripDatas;
                FVvacationLineData.FVVacTfp = ((GlobalSettings.CurrentBidDetails.BidPeriodEndDate - Fvdata.StartDate).Days + 1) * GlobalSettings.DailyVacPay;
                FVvacationLineDatalist.Add(FVvacationLineData);

                FVvacationLineData = new FVvacationLineData();
                FVvacationLineData.FVStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1);
                FVvacationLineData.FVEndDate = Fvdata.EndDate;
                FVvacationLineData.FVVacationTripDatas = new List<FVVacationTripData>();
                FVvacationLineData.FVVacTfp = ((Fvdata.EndDate - GlobalSettings.CurrentBidDetails.BidPeriodEndDate).Days) * GlobalSettings.DailyVacPay;
            }
            else
            {
                FVvacationLineData.FVStartDate = Fvdata.StartDate;
                FVvacationLineData.FVEndDate = Fvdata.EndDate;
                FVvacationLineData.FVVacationTripDatas = Fvdata.FVVacationTripDatas;
                //FVvacationLineData.FVVacTfp = ((Fvdata.EndDate - Fvdata.StartDate).Days + 1) * GlobalSettings.DailyVacPay;
                if (Fvdata.StartDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
                    FVvacationLineData.FVVacTfp = ((Fvdata.EndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1) * GlobalSettings.DailyVacPay;
                else
                    FVvacationLineData.FVVacTfp = ((Fvdata.EndDate - Fvdata.StartDate).Days + 1) * GlobalSettings.DailyVacPay;
            }
            return FVvacationLineData;
            //FVvacationLineData.FVVacTfp = Fvdata.FVVacationTripDatas.Sum(x => x.TripTfpInLine);
            //FVvacationLineData.FVVap = Math.Max(GlobalSettings.DailyVacPay * 7 - FVvacationLineData.FVVacTfp, 0);
            //line.FVVaPList.AddRange(CalulateVAPForoneFVVacationPeriod(FVvacationLineData.FVVacationTripDatas, Fvdata.StartDate, Fvdata.EndDate, FVvacationLineData.FVVap));
        }
        private List<FVVacationData> getTripsInsidetheFVVacation(Line line, List<Absense> absencelist)
        {
            List<FVVacationData> fvVacationlist = new List<FVVacationData>();
            DateTime tripStartDate, tripEndDate;
            int tripEndTime = 0;
            bool isLastTrip = false; int paringCount = 0;
            Trip trip = null;
            FVVacationData FVVacationData = new FVVacationData();
            int totalLegs = 0;
            foreach (var absence in absencelist)
            {
                FVVacationData = fvVacationlist.FirstOrDefault(x => x.StartDate == absence.StartAbsenceDate && x.EndDate == absence.EndAbsenceDate);

                if (FVVacationData == null)
                {
                    //DateTime startabsensedate = (absence.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate) ? GlobalSettings.CurrentBidDetails.BidPeriodStartDate : absence.StartAbsenceDate;
                    FVVacationData = new FVVacationData
                    {
                        StartDate = absence.StartAbsenceDate,
                        EndDate = absence.EndAbsenceDate,
                        FVVacationTripDatas = new List<FVVacationTripData>()
                    };
                }

                foreach (var pairing in line.Pairings)
                {
                    isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;

                    // the following line of code works for pilots since all trips start in same calendar month as bid period
                    tripStartDate = WBidCollection.SetDate(Convert.ToInt16(pairing.Substring(4, 2)), isLastTrip);
                    //string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S") ? pairing : pairing.Substring(0, 4);
                    // string splitName = (pairing.Substring(1, 1) == "P" || pairing.Substring(1, 1) == "S" || pairing.Substring(1, 1) == "W" || pairing.Substring(1, 1) == "Y") ? pairing : pairing.Substring(0, 4);
                    //trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
                    string splitName = pairing.Substring(0, 4);
                    trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing.Substring(0, 4));
                    if (trip == null)
                    {
                        trip = GlobalSettings.Trip.FirstOrDefault(x => x.TripNum == pairing);
                        splitName = pairing;
                    }

                    tripEndDate = tripStartDate.AddDays(trip.PairLength - 1);

                    // added to capture trips that arrive back in domicile after 2400 domicile time
                    tripEndTime = (trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg) % 1440;
                    tripEndTime = AfterMidnightLandingTime(tripEndTime);
                    tripEndTime = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripEndDate, tripEndTime);



                    // we dont need to consider the vacation trip for the FV vacation. Vacation will be higher priority.
                    VacationStateTrip vacobj = null;
                    TripMultiVacData vacTrip = null;
                    if (line.VacationStateLine.VacationTrips != null)
                    {
                        vacobj = line.VacationStateLine.VacationTrips.Where(x => x.TripName == pairing).FirstOrDefault();


                    }
                    if (vacobj == null)
                    {

                        //foreach (var absence in absencelist)
                        // {

                        //FVVacationData.FVVacationTripDatas = new List<FVVacationTripData>();

                        if (tripStartDate < absence.StartAbsenceDate && tripEndDate >= absence.StartAbsenceDate)
                        {

                            totalLegs = (line.ReserveLine) ? (tripEndDate - absence.StartAbsenceDate).Days + 1 : trip.TotalLegs;
                            FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "LeftConflict", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs });
                        }
                        else if (tripStartDate >= absence.StartAbsenceDate && tripEndDate <= absence.EndAbsenceDate)
                        {
                            //completly inside
                            totalLegs = (line.ReserveLine) ? (tripEndDate - tripStartDate).Days + 1 : trip.TotalLegs;
                            FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "Float", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs });
                        }
                        else if (tripStartDate <= absence.EndAbsenceDate && tripEndDate > absence.EndAbsenceDate)
                        {

                            totalLegs = (line.ReserveLine) ? (absence.EndAbsenceDate - tripStartDate).Days + 1 : trip.TotalLegs;
                            FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "RightConflict", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs });
                        }
                        //}

                    }
                    else
                    {
                        if (tripStartDate < absence.StartAbsenceDate && tripEndDate >= absence.StartAbsenceDate)
                        {
                            vacTrip = VacationData.FirstOrDefault(x => x.Key == pairing).Value;
                            if ((vacTrip == null && vacobj != null) || (vacTrip != null && vacobj == null))
                            {
                            }
                            var vacationdata = vacTrip.VobData;

                            totalLegs = (line.ReserveLine) ? (tripEndDate - absence.StartAbsenceDate).Days + 1 : trip.TotalLegs;
                            if (vacationdata == null)
                                FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "LeftVacation", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs });
                            else
                                FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "LeftVacation", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs, VDBackTfpDrop = vacationdata.VacationGrandTotal.VD_TfpInBpTot, VOBackTfpDrop = vacationdata.VacationGrandTotal.VO_TfpInBpTot, VaVacationDropDays = vacationdata.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split" || x.VacationType == "VO").Count() });
                        }
                        else if (tripStartDate >= absence.StartAbsenceDate && tripEndDate <= absence.EndAbsenceDate)
                        {
                            //completly inside
                            totalLegs = (line.ReserveLine) ? (tripEndDate - tripStartDate).Days + 1 : trip.TotalLegs;
                            FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "Vacation", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs });
                        }
                        else if (tripStartDate <= absence.EndAbsenceDate && tripEndDate > absence.EndAbsenceDate)
                        {
                            vacTrip = VacationData.FirstOrDefault(x => x.Key == pairing).Value;
                            if ((vacTrip == null && vacobj != null) || (vacTrip != null && vacobj == null))
                            {
                            }
                            var vacationdata = vacTrip.VofData;
                            totalLegs = (line.ReserveLine) ? (absence.EndAbsenceDate - tripStartDate).Days + 1 : trip.TotalLegs;
                            if (vacationdata == null)
                                FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "RightVacation", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs });
                            else
                                FVVacationData.FVVacationTripDatas.Add(new FVVacationTripData { TripName = splitName, TripTfpInLine = trip.Tfp, Type = "RightVacation", TripStartDate = tripStartDate, TripEndDate = tripEndDate, TripLegs = totalLegs, VDFrontTfpDrop = vacationdata.VacationGrandTotal.VD_TfpInBpTot, VOFrontTfpDrop = vacationdata.VacationGrandTotal.VO_TfpInBpTot, VaVacationDropDays = vacationdata.DutyPeriodsDetails.Where(x => x.VacationType == "VD" || x.VacationType == "Split" || x.VacationType == "VO").Count() });
                        }

                    }

                }
                if (FVVacationData.FVVacationTripDatas.Count() > 0)
                    fvVacationlist.Add(FVVacationData);
            }
            return fvVacationlist;
        }
        private int AfterMidnightLandingTime(int landTime)
        {
            if (landTime < GlobalSettings.LastLandingMinus1440)
                return landTime += 1440;
            else
                return landTime % 1440;
        }


    }
    public class PairingDetails
    {
        public bool IsAnyConflicts { get; set; }
        public bool IsRightVacationTrip { get; set; }
        public bool IsLeftVacationTrip { get; set; }
    }

}
