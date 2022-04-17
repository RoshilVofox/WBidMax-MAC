using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Model;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class PrepareModernBidLineView
    {
        private List<VAHelper> VAHelperList { get; set; }
        #region Old
        //public void SetModernBidLineView()
        //{
        //    try
        //    {
        //        foreach (Line line in GlobalSettings.Lines)
        //        {
        //            CalcaulatePermanantbidLinevalues(line);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //private void CalcaulatePermanantbidLinevalues(Line line)
        //{
        //    foreach (BidLineTemplate template in line.BidLineTemplates)
        //    {
        //        SetBidLineViewType(line, template);

        //    }
        //}

        //public void SetBidLineViewType(Line line, BidLineTemplate template)
        //{
        //    VacationStateTrip vacationStateTrip = new VacationStateTrip();
        //    if (line.VacationStateLine.VacationTrips != null)
        //        vacationStateTrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == template.TripName);

        //    //trip is not a vacation trip
        //    if (vacationStateTrip == null || line.VacationStateLine.VacationTrips == null)
        //    {
        //        bool absenseExists = GlobalSettings.OrderedVacationDays.Any(x => x.StartAbsenceDate <= template.Date && x.EndAbsenceDate >= template.Date);
        //        if (absenseExists)
        //        {
        //            //if the  bid line template is within the vacation period and vacationstate trip is null,then the duty period will be VAP
        //            if (template.ArrStaLastLeg == null)
        //                template.BidLineType = (int)BidLineType.VAP;
        //        }
        //        else
        //        {
        //            if (template.ArrStaLastLeg == null)
        //                template.BidLineType = (int)BidLineType.NoTrip;
        //            else
        //                template.BidLineType = (int)BidLineType.NormalTrip;
        //        }
        //    }
        //    //vacation trip
        //    else
        //    {
        //        int dutyperiodSeqNumber = (template.Date - vacationStateTrip.TripActualStartDate).Days;
        //        VacationStateDutyPeriod dutyperiod = vacationStateTrip.VacationDutyPeriods.FirstOrDefault(x => x.DutyperidSeqNo == dutyperiodSeqNumber + 1);
        //        if (dutyperiod != null)
        //        {
        //            if (dutyperiod.DutyPeriodType == "VA")
        //            {
        //                template.BidLineType = (int)BidLineType.VA;
        //            }
        //            else if (dutyperiod.DutyPeriodType == "VD")
        //            {
        //                template.BidLineType = (int)BidLineType.VD;
        //            }
        //            else if (dutyperiod.DutyPeriodType == "VO")
        //            {
        //                template.BidLineType = (int)BidLineType.VO;
        //            }
        //            else if (dutyperiod.DutyPeriodType == "Split")
        //            {
        //                if (vacationStateTrip.TripType == "VOF")
        //                {
        //                    template.BidLineType = (int)BidLineType.VOFSplit;
        //                }
        //                else
        //                {
        //                    template.BidLineType = (int)BidLineType.VOBSplit;
        //                }

        //            }
        //        }
        //    }
        //}

        //private void CalcaulteVAPValue(Line line, BidLineTemplate template)
        //{
        //    VacationStateTrip vacationStateTrip = new VacationStateTrip();
        //    if (line.VacationStateLine.VacationTrips != null)
        //        vacationStateTrip = line.VacationStateLine.VacationTrips.FirstOrDefault(x => x.TripName == template.TripName);
        //    decimal tfp;
        //    if (vacationStateTrip.TripType == "VA")
        //    {
        //        VacationTrip vTrip = GlobalSettings.VacationData.Where(x => x.Key == template.TripName).Select(y => y.Value.VaData).FirstOrDefault();
        //        if (vTrip == null) return;
        //        tfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
        //        //tfp = GlobalSettings.VacationData.Where(x => x.Key == template.TripName).Select(y => y.Value.VaData.VacationGrandTotal.VA_TfpInBpTot).FirstOrDefault();
        //        //VacationObject.VAPList.Add(new VapHelper() { StartDate = tripStart, EndDate = tripStart.AddDays(vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1), VAP = tfp, NumberOfDays = tripStart.AddDays(vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1).Subtract(tripStart).Days + 1 });

        //    }
        //    else if(vacationStateTrip.TripType == "VOF")
        //    {
        //        VacationTrip vTrip = GlobalSettings.VacationData.Where(x => x.Key == template.TripName).Select(y => y.Value.VofData).FirstOrDefault();
        //        if (vTrip == null) return;
        //        tfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
        //        //VacationObject.VAPList.Add(new VapHelper() { StartDate = tripEnd.AddDays((vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1)), EndDate = tripEnd, VAP = tfp, NumberOfDays = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() });


        //    }
        //    else if (vacationStateTrip.TripType == "VOB")
        //    {
        //        VacationTrip vTrip = GlobalSettings.VacationData.Where(x => x.Key == template.TripName).Select(y => y.Value.VobData).FirstOrDefault();
        //        if (vTrip == null) return;
        //        tfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
        //        //VacationObject.VAPList.Add(new VapHelper() { StartDate = tripStart, EndDate = tripStart.AddDays((vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1)), VAP = tfp, NumberOfDays = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() });

        //    }

        //}

        //private void CalculatebidlInePropertiesforNormal(Line line)
        //{ 
        //    foreach(BidLineTemplate bidlinetemplate in line.BidLineTemplates)
        //    {
        //        bidlinetemplate.BidLineType=(int)BidLineType.NormalTrip;
        //        //clear the  remaining propoerties for the bidline
        //    }
        //}
        #endregion
        public void CalculatebidLinePropertiesforFloatingPointVacation(Line line)
        {

            foreach (var blTemplate in line.BidLineTemplates)
            {
                bool FVabsenseExists = false;

                //if (line.BlankLine || line.ReserveLine)
                if (line.BlankLine)
                    continue;
                if (GlobalSettings.IsFVVacation)
                {
                    FVabsenseExists = line.FVvacationData.Any(x => x.FVStartDate <= blTemplate.Date && x.FVEndDate >= blTemplate.Date);
                }
                if (FVabsenseExists)
                {
                    blTemplate.BidLineType = (int)BidLineType.FV;
                }

                SetFvTripname(line, blTemplate);
            }


        }
        private void SetFvTripname(Line line, BidLineTemplate blTemplate)
        {
            try
            {
                blTemplate.TemplateName = "VO";
                DateTime bidDate = blTemplate.Date;
                var tripdata = line.FVvacationData.FirstOrDefault(x => x.FVStartDate <= bidDate && x.FVEndDate >= bidDate);
                if (tripdata != null)
                {
                    if (tripdata.FVEndDate >= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                    {
                        blTemplate.ToolTip = "FV = " + Math.Round(GlobalSettings.DailyVacPay * ((tripdata.FVEndDate - tripdata.FVStartDate).Days + 1), 2);
                    }
                    else
                    {
                        // there is a chance of the less than 7 days for the FV vaction. in those case we need toi give 26.25 FV pay. THe chance will come when users have nromal vacation conflitcts on both side of the FV vacation
                        blTemplate.ToolTip = "FV = " + Math.Round(GlobalSettings.DailyVacPay * 7, 2);
                    }
                    int dpCount = (tripdata.FVEndDate - tripdata.FVStartDate).Days + 1;
                    int middleDutyperiod = (dpCount % 2 == 0) ? dpCount : dpCount + 1;
                    middleDutyperiod = (middleDutyperiod / 2) - 1;
                    blTemplate.ArrStaLastLegDisplay = (bidDate == tripdata.FVStartDate.AddDays(middleDutyperiod)) ? "FV" : string.Empty;
                    // }

                    //blTemplate.TripBackColor = color;
                    //blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.Vacation.ToString();
                    blTemplate.BorderType = (tripdata.FVEndDate == bidDate) ? 2 : 0;
                }
                else
                {
                    ////it means there is no trip inside the FV vacation. so we need to show the FV vacation color on the FV vacation periods.
                    var fvvacation = line.FVvacationData.FirstOrDefault(x => x.FVStartDate <= bidDate && x.FVEndDate >= bidDate);
                    if (fvvacation != null)
                    {
                        blTemplate.ToolTip = "FV = " + Math.Round(GlobalSettings.DailyVacPay * ((fvvacation.FVEndDate - fvvacation.FVStartDate).Days + 1), 2);
                        int dpCount = (fvvacation.FVEndDate - fvvacation.FVStartDate).Days + 1;
                        int middleDutyperiod = (dpCount % 2 == 0) ? dpCount : dpCount + 1;
                        middleDutyperiod = (middleDutyperiod / 2) - 1;
                        blTemplate.ArrStaLastLegDisplay = (bidDate == fvvacation.FVStartDate.AddDays(middleDutyperiod)) ? "FV" : string.Empty;
                        //blTemplate.TripBackColor = GlobalSettings.FVBackColr;
                        //blTemplate.TripForeColor = GlobalSettings.CurrentTheme.CurrentTextColor.Vacation.ToString();
                        blTemplate.BorderType = (fvvacation.FVEndDate == bidDate) ? 2 : 0;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void CalculatebidLinePropertiesforVacation()
        {

            try
            {

                foreach (Line line in GlobalSettings.Lines)
                {
                    //if (line.BlankLine || line.ReserveLine)
                    if (line.BlankLine)
                        continue;
                    if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM)
                    {
                        foreach (BidLineTemplate template in line.BidLineTemplates)
                        {
                            template.BidLineType = (int)BidLineType.NormalTrip;
                        }
                    }
                    else
                    {
                        foreach (BidLineTemplate template in line.BidLineTemplates)
                        {
                            template.BidLineType = (int)BidLineType.NormalTrip;
                        }
                        if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && line.FVvacationData != null)
                            CalculatebidLinePropertiesforFloatingPointVacation(line);
                        else
                        {

                        }
                        //if(line.VacationStateLine!=null )
                        if (GlobalSettings.TempOrderedVacationDays != null && GlobalSettings.TempOrderedVacationDays.Count > 0)
                        {
                            CalculatebidLinePropertiesforVacation(line);
                            //CalulateVacPayoutsideBidPeriodForEOM(line);
                        }
                        CalculateBidLinePropertiesForCFVVacation(line);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void CalculateBidLinePropertiesForCFVVacation(Line line)
        {
            List<Absense> CFVabsence = GlobalSettings.WBidStateCollection.FVVacation.Where(x => x.AbsenceType == "CFV").ToList();

            if (CFVabsence.Count > 0)
            {

                foreach (var item in CFVabsence)
                {
                    bool isneedtoIterateNextLine = false;
                    foreach (var blTemplate in line.BidLineTemplates)
                    {
                        var originalFVdate = item.StartAbsenceDate.Date;
                        DateTime currentdate = blTemplate.Date;
                        if (currentdate >= originalFVdate)
                        {
                            if (currentdate == originalFVdate && blTemplate.TripName == null && (blTemplate.BidLineType == (int)BidLineType.NoTrip || blTemplate.BidLineType == (int)BidLineType.NormalTrip))
                            {
                                line.CFVDates.Add(blTemplate.Date.Day.ToString());
                                blTemplate.TemplateName = "VO";
                                //blTemplate.TripBackColor = color
                                blTemplate.ArrStaLastLegDisplay = "$FV";
                                blTemplate.ToolTip = "CFV=26.25";
                                blTemplate.BidLineType = (int)BidLineType.CFV;
                                break;
                            }
                            else
                            {
                                int currentday = blTemplate.Date.Day;

                                int count = 0;
                                for (int day = currentday; day >= 1; day--)
                                {
                                    //need to iterate back ward. Set CFV date on the left open day
                                    var previousdaytempate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Day == day);
                                    if (previousdaytempate.TripName == null && (previousdaytempate.BidLineType == (int)BidLineType.NoTrip || previousdaytempate.BidLineType == (int)BidLineType.NormalTrip))
                                    {
                                        line.CFVDates.Add(previousdaytempate.Date.Day.ToString());
                                        //previousdaytempate.TripBackColor = GlobalSettings.CFVBackColr;
                                        previousdaytempate.TemplateName = "VO";
                                        previousdaytempate.ArrStaLastLegDisplay = "$FV";
                                        previousdaytempate.ToolTip = "CFV=26.25";
                                        previousdaytempate.BidLineType = (int)BidLineType.CFV;
                                        isneedtoIterateNextLine = true;

                                        break;
                                    }
                                    count++;
                                }
                                if (count == currentday)
                                {
                                    //it means , there is no place found during the backward iteration. if so we need to make the CFV day on the next open day after original CFV date
                                    for (int day = currentday; day <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Day; day++)
                                    {
                                        var nexttempate = line.BidLineTemplates.FirstOrDefault(x => x.Date.Day == day);
                                        if (nexttempate.TripName == null && (nexttempate.BidLineType == (int)BidLineType.NoTrip || nexttempate.BidLineType == (int)BidLineType.NormalTrip))
                                        {
                                            line.CFVDates.Add(nexttempate.Date.Day.ToString());
                                            //nexttempate.TripBackColor = GlobalSettings.CFVBackColr;
                                            nexttempate.TemplateName = "VO";
                                            nexttempate.ArrStaLastLegDisplay = "$FV";
                                            nexttempate.ToolTip = "CFV = 26.25";
                                            nexttempate.BidLineType = (int)BidLineType.CFV;
                                            isneedtoIterateNextLine = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (isneedtoIterateNextLine)
                                break;
                        }
                    }
                }
            }
        }

        private void CalulateVacPayoutsideBidPeriodForEOM(Line line)
        {
            if (GlobalSettings.MenuBarButtonStatus.IsEOM)
            {
                line.VacPayNeBp = CalcEOMPayoutsideBidperiod(line);
                line.VacPayBothBp = line.VacPayCuBp + line.VacPayNeBp;
            }

        }
        private decimal CalcEOMPayoutsideBidperiod(Line line)
        {

            decimal carryoutVacationPay = 0;
            var outsidebidperiodBidlineTemplates = line.BidLineTemplates.Where(x => x.Date > GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
            decimal prevvalue = 0;
            int prevtype = 0;
            //int count=0;
            foreach (var item in outsidebidperiodBidlineTemplates)
            {
                if (GlobalSettings.CurrentBidDetails.Postion == "FA")
                {
                    if (prevtype != (int)item.BidLineType && prevvalue != item.Value)
                    {
                        if (item.BidLineType == (int)BidLineType.VA || item.BidLineType == (int)BidLineType.VAP)
                        {
                            carryoutVacationPay += Convert.ToDecimal(item.Value);
                        }
                    }
                }
                else
                {
                    //if (prevtype != (int)item.BidLineType && prevvalue != item.Value && count != 0)
                    if (prevtype != (int)item.BidLineType && prevvalue != item.Value)
                    {
                        if (item.BidLineType == (int)BidLineType.VA || item.BidLineType == (int)BidLineType.VAP)
                        {
                            carryoutVacationPay += Convert.ToDecimal(item.Value);
                        }
                    }
                }
                prevvalue = item.Value;
                prevtype = item.BidLineType;
            }
            return carryoutVacationPay;
        }
        /// <summary>
        /// Calculate the bid line properties to show the vacattion details to the modern bid line view.
        /// </summary>
        /// <param name="line"></param>
        private void CalculatebidLinePropertiesforVacation(Line line)
		{

			try
			{
				VAHelperList = new List<VAHelper>();
				if (line.LineNum == 304)
				{
				}
				decimal vATfp = 0;
				decimal vATfpInBp = 0;
				//  if (line.VacationStateLine.VacationTrips != null)
				//{
				//foreach (VacationStateTrip vacationStateTrip in line.VacationStateLine.VacationTrips)
				//{
				//    var tripEndDate = vacationStateTrip.TripActualStartDate.AddDays(vacationStateTrip.VacationDutyPeriods.Count - 1);
				//    var BidLines = line.BidLineTemplates.Where(x => x.Date >= vacationStateTrip.TripActualStartDate && x.Date <= tripEndDate);
				//    //remove all the arriavl start leg display
				//    foreach (BidLineTemplate template in BidLines)
				//    {
				//        template.ArrStaLastLegDisplay = "";
				//    }

				//    VacationTrip vTrip = new VacationTrip();
				//    if (vacationStateTrip.TripType == "VA")
				//    {

				//        vTrip = GlobalSettings.VacationData.Where(x => x.Key == vacationStateTrip.TripName).Select(y => y.Value.VaData).FirstOrDefault();
				//        vATfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
				//        if (vTrip == null) return;
				//        VAHelperList.Add(new VAHelper { StartDate = vacationStateTrip.TripActualStartDate, EndDate = tripEndDate, VA = vATfp });
				//        foreach (BidLineTemplate bidlinetemplate in BidLines)
				//        {
				//            bidlinetemplate.BidLineType = (int)BidLineType.VA;
				//            bidlinetemplate.ToolTip = "VA = " + vATfp.ToString();
				//            bidlinetemplate.ArrStaLastLegDisplay = "";
				//        }
				//        //set the bid line name into the center of the VA duty periods
				//        int dpCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count();
				//        int middleDutyperiod = (dpCount % 2 == 0) ? dpCount : dpCount + 1;
				//        middleDutyperiod = (middleDutyperiod / 2) - 1;
				//        BidLines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VA";
				//    }
				//    else if (vacationStateTrip.TripType == "VOF")
				//    {
				//        vTrip = GlobalSettings.VacationData.Where(x => x.Key == vacationStateTrip.TripName).Select(y => y.Value.VofData).FirstOrDefault();
				//        if (vTrip == null) return;
				//        vATfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
				//        int vADays = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count();
				//        VAHelperList.Add(new VAHelper() { StartDate = tripEndDate.AddDays(-(vADays - 1)), EndDate = tripEndDate, VA = vATfp, });

				//        foreach (VacationStateDutyPeriod vacationStateDutyPeriod in vacationStateTrip.VacationDutyPeriods)
				//        {
				//            SetBidLineValues(vacationStateTrip, BidLines, vacationStateDutyPeriod, vTrip, true);

				//        }
				//        //set the bid line names into the center of the duty periods
				//        SetBidLineName(BidLines, vTrip, true);

				//    }
				//    else if (vacationStateTrip.TripType == "VOB")
				//    {
				//        vTrip = GlobalSettings.VacationData.Where(x => x.Key == vacationStateTrip.TripName).Select(y => y.Value.VobData).FirstOrDefault();
				//        if (vTrip == null) return;
				//        vATfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
				//        VAHelperList.Add(new VAHelper() { StartDate = vacationStateTrip.TripActualStartDate, EndDate = vacationStateTrip.TripActualStartDate.AddDays((vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1)), VA = vATfp, });

				//        foreach (VacationStateDutyPeriod vacationStateDutyPeriod in vacationStateTrip.VacationDutyPeriods)
				//        {
				//            SetBidLineValues(vacationStateTrip, BidLines, vacationStateDutyPeriod, vTrip, false);
				//        }
				//        //set the bid line names into the center of the duty periods
				//        SetBidLineName(BidLines, vTrip, false);
				//    }



				//}


				//--------------------------------------------------------------------------------------------------------



				Trip trip = null;
				bool isLastTrip = false;
				int paringCount = 0;

				foreach (string pairing in line.Pairings)
				{

					try
					{

						trip = GetTrip(pairing);
						isLastTrip = ((line.Pairings.Count - 1) == paringCount); paringCount++;


						DateTime tripStartDate = WBidCollection.SetDate(int.Parse(pairing.Substring(4, 2).TrimStart()), isLastTrip);
						int tripEndTime = 0;
						DateTime tripEndDate = tripStartDate.AddDays(trip.DutyPeriods.Count - 1);
						String tripType = string.Empty;
						// added to capture trips that arrive back in domicile after 2400 domicile time
						tripEndTime = (trip.DutyPeriods[trip.DutyPeriods.Count - 1].LandTimeLastLeg) % 1440;
						tripEndTime = WBidCollection.AfterMidnightLandingTime(tripEndTime);
						tripEndTime = WBidCollection.DomicileTimeFromHerb(GlobalSettings.CurrentBidDetails.Domicile, tripEndDate, tripEndTime);


						//  1.Check if the trip is completely inside the vacation (VA Vacation)
						//-------------------------------------------------------------------------------------------------
						if (GlobalSettings.TempOrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.StartAbsenceDate <= tripStartDate && x.EndAbsenceDate >= tripEndDate)))
						{
							tripType = "VA";
						}
						// 2.Check if trip starts before the vacation period and finishes inside the vacation period. (VDF Vacation)
						//-------------------------------------------------------------------------------------------------
						else if (GlobalSettings.TempOrderedVacationDays.Any(x => x.AbsenceType == "VA" && ((x.StartAbsenceDate > tripStartDate) && (x.StartAbsenceDate <= tripEndDate)) ||
							((x.StartAbsenceDate.AddDays(-1) == tripEndDate) && (tripEndTime > 1440))))
						{
							tripType = "VOF";
						}

						// 3.check if trip starts inside the vacation period and finished outside the vacation period.. (VDB Vacation)
						//-------------------------------------------------------------------------------------------------

						else if (GlobalSettings.TempOrderedVacationDays.Any(x => x.AbsenceType == "VA" && (x.EndAbsenceDate >= tripStartDate && x.EndAbsenceDate <= tripEndDate)))
						{
							tripType = "VOB";
						}


						if (tripType != string.Empty)
						{
							var BidLines = line.BidLineTemplates.Where(x => x.Date >= tripStartDate && x.Date <= tripEndDate);
							//remove all the arriavl start leg display
							foreach (BidLineTemplate template in BidLines)
							{
								template.ArrStaLastLegDisplay = "";
							}

							VacationTrip vTrip = new VacationTrip();
							if (tripType == "VA")
							{

								var s = GlobalSettings.VacationData.Where(x => x.Key == pairing);
								vTrip = GlobalSettings.VacationData.Where(x => x.Key == pairing).Select(y => y.Value.VaData).FirstOrDefault();
                                if (vTrip != null)
                                {
                                    //							
                                    if (vTrip.VacationGrandTotal.VA_TfpInBpTot == null)
                                    {
                                    }

                                    vATfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
                                    vATfpInBp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
                                    if (vTrip == null) return;



                                    //VAHelperList.Add(new VAHelper { StartDate = tripStartDate, EndDate = tripEndDate, VA = vATfp });



                                    VAHelperList.Add(new VAHelper() { StartDate = tripStartDate, EndDate = tripStartDate.AddDays(vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1), VA = vATfp, IsInBp = false });

                                    //VAHelperList.Add(new VAHelper() { StartDate = tripStartDate, EndDate = tripStartDate.AddDays(vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1), VA = vATfpInBp,IsInBp=true });

                                    if (tripEndDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                                    {
                                        int dpinoutsidebidperiod = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && !x.isInBp).Count();
                                        if (vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && !x.isInBp).Count() > 0)
                                        {
                                            VAHelperList.Add(new VAHelper() { StartDate = tripEndDate.AddDays(-dpinoutsidebidperiod + 1), EndDate = tripEndDate, VA = vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot });
                                        }
                                    }
                                    foreach (BidLineTemplate bidlinetemplate in BidLines)
                                    {

                                        bidlinetemplate.BidLineType = (int)BidLineType.VA;

                                        bidlinetemplate.ArrStaLastLegDisplay = "";
                                        if (bidlinetemplate.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                                        {
                                            bidlinetemplate.ToolTip = "VA = " + Math.Round(vATfp, 2).ToString();
                                            bidlinetemplate.Value = Math.Round(vATfp, 2);

                                        }
                                        else
                                        {
                                            bidlinetemplate.ToolTip = "VA = " + Math.Round(vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot, 2).ToString();
                                            bidlinetemplate.Value = Math.Round(vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot, 2);
                                        }


                                    }
                                    //set the bid line name into the center of the VA duty periods
                                    //int dpCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count();
                                    int dpCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count();
                                    int middleDutyperiod = (dpCount % 2 == 0) ? dpCount : dpCount + 1;
                                    middleDutyperiod = (middleDutyperiod / 2) - 1;
                                    BidLines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VA";
                                }


							}
							else if (tripType == "VOF")
							{
								vTrip = GlobalSettings.VacationData.Where(x => x.Key == pairing).Select(y => y.Value.VofData).FirstOrDefault();
								if (vTrip == null) return;
								vATfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
								vATfpInBp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
								int vADays = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count();
								//





								if (tripEndDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
								{

									VAHelperList.Add(new VAHelper() { StartDate = tripEndDate.AddDays(-(vADays - 1)), EndDate = tripStartDate.AddDays(vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1), VA = vATfp - (vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot), IsInBp = false });
									//VAHelperList.Add(new VAHelper() { StartDate = tripEndDate.AddDays(-(vADays - 1)), EndDate = tripStartDate.AddDays(vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count() - 1), VA = vATfp - (vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot),IsInBp=true });
									int dpinoutsidebidperiod = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && !x.isInBp).Count();
									if (vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && !x.isInBp).Count() > 0)
									{
										VAHelperList.Add(new VAHelper() { StartDate = tripEndDate.AddDays(-dpinoutsidebidperiod + 1), EndDate = tripEndDate, VA = vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot });
									}

								}
								else
								{
									VAHelperList.Add(new VAHelper() { StartDate = tripEndDate.AddDays(-(vADays - 1)), EndDate = tripEndDate, VA = vATfp, });
								}
								DateTime tDate = tripStartDate;
								foreach (VacationDutyPeriod vacationDutyPeriod in vTrip.DutyPeriodsDetails)
								{
									var BidLinesBsedonCurrentDutyPeriod = line.BidLineTemplates.FirstOrDefault(x => x.Date == tDate);
									SetBidLineValues(vTrip, vacationDutyPeriod, BidLinesBsedonCurrentDutyPeriod);
									tDate = tDate.AddDays(1);

								}
                                //set the bid line names into the center of the duty periods
                                //set the bid line names into the center of the duty periods /// Modified by Roshil on 21-6-2021 due to the reason that we need to hide Vacation if the FV assigned to that day alredy( case: Two adjacent vacation comes for an FV)
                                var fvremovednidlines = BidLines.Where(x => x.BidLineType != (int)BidLineType.FV);
                                SetBidLineName(fvremovednidlines, vTrip, true);

							}

							else if (tripType == "VOB")
							{
								vTrip = GlobalSettings.VacationData.Where(x => x.Key == pairing).Select(y => y.Value.VobData).FirstOrDefault();
								if (vTrip == null) return;
								vATfp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
								vATfpInBp = vTrip.VacationGrandTotal.VA_TfpInBpTot;
								int vADays = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" & !x.isInBp).Count();
								if (tripEndDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
								{

									VAHelperList.Add(new VAHelper() { StartDate = tripStartDate, EndDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate, VA = vTrip.VacationGrandTotal.VA_TfpInBpTot, IsInBp = true });
									VAHelperList.Add(new VAHelper() { StartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1), EndDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(vADays), VA = vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot, IsInBp = false });

								}
								else
								{
									VAHelperList.Add(new VAHelper() { StartDate = tripStartDate, EndDate = tripStartDate.AddDays((vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count() - 1)), VA = vATfp, IsInBp = true });

									//VAHelperList.Add(new VAHelper() { StartDate = tripEndDate.AddDays(-(vADays - 1)), EndDate = tripEndDate, VA = vATfp, });
								}

								//VAHelperList.Add(new VAHelper() { StartDate = tripStartDate, EndDate = tripStartDate.AddDays((vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" ).Count() - 1)), VA = vATfp,IsInBp=false });

								DateTime tDate = tripStartDate;
								foreach (VacationDutyPeriod vacationDutyPeriod in vTrip.DutyPeriodsDetails)
								{

									var BidLinesBsedonCurrentDutyPeriod = line.BidLineTemplates.FirstOrDefault(x => x.Date == tDate);
									SetBidLineValues(vTrip, vacationDutyPeriod, BidLinesBsedonCurrentDutyPeriod);
									tDate = tDate.AddDays(1);

								}
                                //set the bid line names into the center of the duty periods /// Modified by Roshil on 21-6-2021 due to the reason that we need to hide Vacation if the FV assigned to that day alredy( case: Two adjacent vacation comes for an FV)
                                var fvremovednidlines = BidLines.Where(x => x.BidLineType != (int)BidLineType.FV);
                                //set the bid line names into the center of the duty periods
                                SetBidLineName(fvremovednidlines, vTrip, false);
							}



						}
					}
					catch (Exception ex)
					{
						throw;
					}


				}




				//}
				if (line.LineNum == 304)
				{

				}
				//Set VAP Values
				List<VAPHelper> VAPHelper = GenerateVAPValues(VAHelperList);
				foreach (VAPHelper vaphelper in VAPHelper)
				{
					var bidlines = line.BidLineTemplates.Where(x => x.Date >= vaphelper.StartDate && x.Date <= vaphelper.EndDate);
					if (bidlines.Count() > 0)
					{

						foreach (BidLineTemplate temmplate in bidlines)
						{
							temmplate.BidLineType = (int)BidLineType.VAP;
							temmplate.ToolTip = "VAP = " + Decimal.Round(vaphelper.VAPValue, 2).ToString();
                            temmplate.Value = Math.Round(vaphelper.VAPValue, 2);

						}
						int bidlinescount = bidlines.Count();
						int middleDutyperiod = (bidlinescount % 2 == 0) ? bidlinescount : bidlinescount + 1;
						middleDutyperiod = (middleDutyperiod / 2) - 1;
						bidlines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VAP";
					}
				}

                var inBpVAplist= VAPHelper.Where(x => x.StartDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && x.EndDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
                line.VAPbp = inBpVAplist.Sum(x => x.VAPValue);
                line.VAPbo = VAPHelper.Sum(x => x.VAPValue);
                line.VAPne = line.VAPbo - line.VAPbp;

            }


			catch (Exception ex)
			{
				throw ex;
			}
		}
        /// <summary>
        /// Set the bid line name like VA,VOF,VDF etc into the center of the bid lines.
        /// </summary>
        /// <param name="BidLines"></param>
        /// <param name="vTrip"></param>
        /// <param name="isVOF"></param>
        private void SetBidLineName(IEnumerable<BidLineTemplate> BidLines, VacationTrip vTrip, bool isVOF)
        {
            try
            {
               
                //int vAdpCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA" && x.isInBp).Count();
                int vAdpCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VA").Count();
                int middleDutyperiod = (vAdpCount % 2 == 0) ? vAdpCount : vAdpCount + 1;
                middleDutyperiod = (middleDutyperiod / 2) - 1;
                var   VaBiLines = BidLines.Where(x => x.BidLineType == (int)BidLineType.VA);
                if (middleDutyperiod >= 0)
                {
                    VaBiLines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VA";
                }
                //VOF trip
                if (isVOF)
                {
                   // int vdfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" && x.isInBp).Count();
                    int vdfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD").Count();
                    if (vdfCount > 0)
                    {
                        middleDutyperiod = (vdfCount % 2 == 0) ? vdfCount : vdfCount + 1;
                        middleDutyperiod = (middleDutyperiod / 2) - 1;
                        
                        if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
                            VaBiLines = BidLines.Where(x => x.BidLineType == (int)BidLineType.VDDrop || x.BidLineType == (int)BidLineType.VOFSplitDrop);
                        else 
                            VaBiLines = BidLines.Where(x => x.BidLineType == (int)BidLineType.VD);
                        if(VaBiLines.Count()>0)
                        VaBiLines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VDF";
                    }

                   // int vOfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VO" && x.isInBp).Count();
                    int vOfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VO").Count();
                    if (vOfCount > 0)
                    {
                        middleDutyperiod = (vOfCount % 2 == 0) ? vOfCount : vOfCount + 1;
                        middleDutyperiod = (middleDutyperiod / 2) - 1;
                        VaBiLines = BidLines.Where(x => x.BidLineType == (int)BidLineType.VO);
                        if (VaBiLines.Count() > 0)
                            VaBiLines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VOF";
                    }
                }
                //VOB trip
                else
                {
                   // int vdfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD" && x.isInBp).Count();
                    int vdfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VD").Count();
                    if (vdfCount > 0)
                    {
                        middleDutyperiod = (vdfCount % 2 == 0) ? vdfCount : vdfCount + 1;
                        middleDutyperiod = (middleDutyperiod / 2) - 1;

                        if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
                            VaBiLines = BidLines.Where(x => x.BidLineType == (int)BidLineType.VDDrop || x.BidLineType == (int)BidLineType.VOBSplitDrop);
                        else
                            VaBiLines = BidLines.Where(x => x.BidLineType == (int)BidLineType.VD);
                        if (VaBiLines.Count() > 0)
                            VaBiLines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VDB";
                    }

                   // int vOfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VO" && x.isInBp).Count();
                    int vOfCount = vTrip.DutyPeriodsDetails.Where(x => x.VacationType == "VO").Count();
                    if (vOfCount > 0)
                    {
                        middleDutyperiod = (vOfCount % 2 == 0) ? vOfCount : vOfCount + 1;
                        middleDutyperiod = (middleDutyperiod / 2) - 1;
                        VaBiLines = BidLines.Where(x => x.BidLineType == (int)BidLineType.VO);
                        if (VaBiLines.Count() > 0)
                            VaBiLines.ToList()[middleDutyperiod].ArrStaLastLegDisplay = "VOB";
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }
        /// <summary>
        /// Set the bid lien values for each bid line template 
        /// <param name="vacationStateTrip"></param>        /// </summary>

        /// <param name="BidLines"></param>
        /// <param name="isVOF"></param>
        //private void SetBidLineValues(VacationStateTrip vacationStateTrip, IEnumerable<BidLineTemplate> BidLines, VacationStateDutyPeriod vacationStateDutyPeriod, VacationTrip vTrip, bool isVOF)
        //{
        //    var bidline = BidLines.FirstOrDefault(x => x.Date == vacationStateTrip.TripActualStartDate.AddDays(vacationStateDutyPeriod.DutyperidSeqNo - 1));
        //    //VA Duty period
        //    if (bidline == null)
        //    {
        //    }
        //    if (vacationStateDutyPeriod.DutyPeriodType == "VA")
        //    {
        //        bidline.BidLineType = (int)BidLineType.VA;
        //        bidline.ToolTip = "VA = " + vTrip.VacationGrandTotal.VA_TfpInBpTot.ToString();
        //    }
        //    //VD Duty period
        //    else if (vacationStateDutyPeriod.DutyPeriodType == "VD")
        //    {
        //        if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
        //            bidline.BidLineType = (int)BidLineType.VDDrop;
        //        else
        //            bidline.BidLineType = (int)BidLineType.VD;
        //        bidline.ToolTip = "VD = " + vTrip.VacationGrandTotal.VD_TfpInBpTot.ToString();
        //    }
        //    //VO Duty period
        //    else if (vacationStateDutyPeriod.DutyPeriodType == "VO")
        //    {
        //        bidline.BidLineType = (int)BidLineType.VO;
        //        bidline.ToolTip = "VO = " + vTrip.VacationGrandTotal.VO_TfpInBpTot.ToString();
        //    }
        //    //Split Duty period
        //    else if (vacationStateDutyPeriod.DutyPeriodType == "Split")
        //    {
        //        if (isVOF)
        //        {
        //            if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
        //                bidline.BidLineType = (int)BidLineType.VOFSplitDrop;
        //            else
        //                bidline.BidLineType = (int)BidLineType.VOFSplit;
        //            bidline.ToolTip = "VD = " + vTrip.VacationGrandTotal.VD_TfpInBpTot.ToString();
        //            bidline.ToolTipBottom = "VO =" + vTrip.VacationGrandTotal.VO_TfpInBpTot.ToString();
        //        }
        //        else
        //        {
        //            if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
        //                bidline.BidLineType = (int)BidLineType.VOBSplitDrop;
        //            else
        //                bidline.BidLineType = (int)BidLineType.VOBSplit;
        //            bidline.ToolTip = "VD = " + vTrip.VacationGrandTotal.VD_TfpInBpTot.ToString();
        //            bidline.ToolTipBottom = "VO = " + vTrip.VacationGrandTotal.VO_TfpInBpTot.ToString();
        //        }
        //    }
        //}

   //     private void SetBidLineValues(VacationTrip vTrip, VacationDutyPeriod vacationDutyPeriod, BidLineTemplate bidline)
   //     {
   //        // var bidline = BidLines.FirstOrDefault(x => x.Date == vacationStateTrip.TripActualStartDate.AddDays(vacationStateDutyPeriod.DutyperidSeqNo - 1));
   //         //VA Duty period
   //         //if (bidline == null)
   //         //{
   //         //}
			//if (vacationDutyPeriod.VacationType == "VA") 
			//{
			//	bidline.BidLineType = (int)BidLineType.VA;
			//	bidline.ToolTip = "VA = " + Math.Round (vTrip.VacationGrandTotal.VA_TfpInBpTot, 2).ToString ();
   //             bidline.Value =  Math.Round(vTrip.VacationGrandTotal.VA_TfpInBpTot, 2);
			//}
   //         //VD Duty period
   //         else if (vacationDutyPeriod.VacationType == "VD") {
			//	if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
			//		bidline.BidLineType = (int)BidLineType.VDDrop;
			//	else
			//		bidline.BidLineType = (int)BidLineType.VD;
			//	bidline.ToolTip = "VD = " + Math.Round (vTrip.VacationGrandTotal.VD_TfpInBpTot, 2).ToString ();
   //             bidline.Value = Math.Round(vTrip.VacationGrandTotal.VD_TfpInBpTot, 2);
			//}
   //         //VO Duty period
   //         else if (vacationDutyPeriod.VacationType == "VO") {
			//	bidline.BidLineType = (int)BidLineType.VO;
			//	bidline.ToolTip = "VO = " + Math.Round (vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString ();
   //             bidline.Value = Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2);
			//	//Split Duty period
			//} else if (vacationDutyPeriod.VacationType == "Split")
     //       {
     //           if (vTrip.VacationType=="VOF")
     //           {
     //               if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
     //                   bidline.BidLineType = (int)BidLineType.VOFSplitDrop;
     //               else
     //                   bidline.BidLineType = (int)BidLineType.VOFSplit;
					//bidline.ToolTip = "VD = " + Math.Round(vTrip.VacationGrandTotal.VD_TfpInBpTot,2).ToString();
					//bidline.ToolTipBottom = "VO =" + Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString();
     //               bidline.Value =  Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2);
     //           }
     //           else
     //           {
     //               if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
     //                   bidline.BidLineType = (int)BidLineType.VOBSplitDrop;
     //               else
     //                   bidline.BidLineType = (int)BidLineType.VOBSplit;
					//bidline.ToolTip = "VD = " + Math.Round(vTrip.VacationGrandTotal.VD_TfpInBpTot,2).ToString();
					//bidline.ToolTipBottom = "VO = " + Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString();
        //            bidline.Value =  Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2);
        //        }
        //    }
        //}
        private void SetBidLineValues(VacationTrip vTrip, VacationDutyPeriod vacationDutyPeriod, BidLineTemplate bidline)
        {
            // var bidline = BidLines.FirstOrDefault(x => x.Date == vacationStateTrip.TripActualStartDate.AddDays(vacationStateDutyPeriod.DutyperidSeqNo - 1));
            //VA Duty period
            //if (bidline == null)
            //{
            //}
            if (bidline.BidLineType != (int)BidLineType.FV)
            {
                if (vacationDutyPeriod.VacationType == "VA")
                {
                    bidline.BidLineType = (int)BidLineType.VA;
                    if (bidline.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                        bidline.ToolTip = "VA = " + Math.Round(vTrip.VacationGrandTotal.VA_TfpInBpTot, 2).ToString();
                    else
                        bidline.ToolTip = "VA = " + Math.Round(vTrip.VacationGrandTotal.VA_TfpInLineTot - vTrip.VacationGrandTotal.VA_TfpInBpTot, 2).ToString();
                    bidline.Value = Math.Round(vTrip.VacationGrandTotal.VA_TfpInBpTot, 2);

                }
                //VD Duty period
                else if (vacationDutyPeriod.VacationType == "VD")
                {
                    if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
                        bidline.BidLineType = (int)BidLineType.VDDrop;
                    else
                        bidline.BidLineType = (int)BidLineType.VD;
                    if (bidline.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                        bidline.ToolTip = "VD = " + Math.Round(vTrip.VacationGrandTotal.VD_TfpInBpTot, 2).ToString();
                    else
                        bidline.ToolTip = "VD = " + Math.Round(vTrip.VacationGrandTotal.VD_TfpInLineTot - vTrip.VacationGrandTotal.VD_TfpInBpTot, 2).ToString();
                    bidline.Value = Math.Round(vTrip.VacationGrandTotal.VD_TfpInBpTot, 2);
                }
                //VO Duty period
                else if (vacationDutyPeriod.VacationType == "VO")
                {
                    bidline.BidLineType = (int)BidLineType.VO;
                    if (bidline.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                        bidline.ToolTip = "VO = " + Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString();
                    else
                        bidline.ToolTip = "VO = " + Math.Round(vTrip.VacationGrandTotal.VO_TfpInLineTot - vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString();
                    bidline.Value = Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2);
                    //Split Duty period
                }
                else if (vacationDutyPeriod.VacationType == "Split")
                {
                    if (vTrip.VacationType == "VOF")
                    {
                        if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
                            bidline.BidLineType = (int)BidLineType.VOFSplitDrop;
                        else
                            bidline.BidLineType = (int)BidLineType.VOFSplit;
                        if (bidline.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                            bidline.ToolTip = "VD = " + Math.Round(vTrip.VacationGrandTotal.VD_TfpInBpTot, 2).ToString();
                        else
                            bidline.ToolTip = "VD = " + Math.Round(vTrip.VacationGrandTotal.VD_TfpInLineTot - vTrip.VacationGrandTotal.VD_TfpInBpTot, 2).ToString();

                        if (bidline.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
                            bidline.ToolTipBottom = "VO = " + Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString();
                        else
                            bidline.ToolTipBottom = "VO = " + Math.Round(vTrip.VacationGrandTotal.VO_TfpInLineTot - vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString();
                        bidline.Value = Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2);
                    }
                    else
                    {
                        if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
                            bidline.BidLineType = (int)BidLineType.VOBSplitDrop;
                        else
                            bidline.BidLineType = (int)BidLineType.VOBSplit;
                        bidline.ToolTip = "VD = " + Math.Round(vTrip.VacationGrandTotal.VD_TfpInBpTot, 2).ToString();
                        bidline.ToolTipBottom = "VO = " + Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2).ToString();
                        bidline.Value = Math.Round(vTrip.VacationGrandTotal.VO_TfpInBpTot, 2);
                    }
                }
            }
        }
		private List<VAPHelper> GenerateVAPValues(List<VAHelper> lstVAHelper)
		{
List<VAPHelper> lstVAPHelper = new List<VAPHelper>();

			DateTime startDate = DateTime.MinValue;
			List<DateTime> lstVAPDays = null;
			int totalVacationDays;
			decimal totTfp = 0;
			decimal totTfpinBp = 0;
			decimal vAPPerday = 0;




			//foreach (Absense vacationPeriod in GlobalSettings.OrderedVacationDays)
			foreach (Absense vacationPeriod in GlobalSettings.TempOrderedVacationDays)
			{

				startDate = vacationPeriod.StartAbsenceDate;
				lstVAPDays = new List<DateTime>();
				//Generate list of VAP days
				//-------------------------------------------------
				while (startDate <= vacationPeriod.EndAbsenceDate)
				{

					if (!lstVAHelper.Any(x => x.StartDate <= startDate && x.EndDate >= startDate))
					{
						//if a VAP day then we will add it into VApDays list
						lstVAPDays.Add(startDate);
					}

					startDate = startDate.AddDays(1);
				}
				//-------------------------------------------------
				totalVacationDays = vacationPeriod.EndAbsenceDate.Subtract(vacationPeriod.StartAbsenceDate).Days + 1;
				int totalVacationDaysInBP = 0;
				decimal vAPPerdayInBP = 0;
				if (vacationPeriod.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
				{
					totalVacationDaysInBP = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Subtract(vacationPeriod.StartAbsenceDate).Days + 1;
				}
				else if (vacationPeriod.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
				{
					totalVacationDaysInBP = vacationPeriod.EndAbsenceDate.Subtract(GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
				}
				else
				{
					totalVacationDaysInBP = vacationPeriod.EndAbsenceDate.Subtract(vacationPeriod.StartAbsenceDate).Days + 1;
				}
				vAPPerday = 0;
				totTfp = lstVAHelper.Where(x => x.StartDate >= vacationPeriod.StartAbsenceDate && x.EndDate <= vacationPeriod.EndAbsenceDate && x.StartDate<=x.EndDate).Sum(y => y.VA);
               // var item1 = lstVAHelper.Where(x => x.StartDate >= vacationPeriod.StartAbsenceDate);
               // var item2 = lstVAHelper.Where(x => x.EndDate <= vacationPeriod.EndAbsenceDate);
               // var item3 = lstVAHelper.Where(x => x.StartDate >= vacationPeriod.StartAbsenceDate && x.EndDate <= vacationPeriod.EndAbsenceDate);


                totTfpinBp = lstVAHelper.Where(x => x.StartDate >= vacationPeriod.StartAbsenceDate && x.EndDate <= vacationPeriod.EndAbsenceDate && x.EndDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate).Sum(y => y.VA);

				int totalVAPDays = lstVAPDays.Count();
				int vapdaysInBP = (lstVAPDays == null) ? 0 : lstVAPDays.Count(x => x <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
                //// frank added 5-21-2017
                //decimal actualVap = Math.Max(totalVacationDays * GlobalSettings.DailyVacPay - totTfp, 0);
                //// distribute VAP in bid period and next bid period
                //decimal actualVapInBp = Math.Min(Math.Max(totalVacationDaysInBP * GlobalSettings.DailyVacPay - totTfpinBp, 0), actualVap);
                //decimal actualVapInNextBp = actualVap - actualVapInBp;
                //            // end frank add 5-21-2017

                // Roshil added 2-23-2021
                decimal actualVapInBp = 0m;
                decimal actualVap = Math.Max(totalVacationDays * GlobalSettings.DailyVacPay - totTfp, 0);
                // distribute VAP in bid period and next bid period
                //Special case :when the total tfp with the mtm week exceeds 26.25.  The special case only applies when the tfp exceeds 26.25.  Otherwise, the Mar Vac Pay is the greater of 3.75 x days of vacation (4) = 15.0 or tfp pulled, whichever is greater.
                //Then in April if the special case does not apply, the user is paid 26.25 minus the MAR vac pay.If the Apr VA is greater that that pay, the VAP for April will be negative.
                if (totTfp > totalVacationDays * GlobalSettings.DailyVacPay)
                    actualVapInBp = Math.Min(Math.Max(totalVacationDaysInBP * GlobalSettings.DailyVacPay - totTfpinBp, 0), actualVap);
                else
                    actualVapInBp = Math.Max(totalVacationDaysInBP * GlobalSettings.DailyVacPay - totTfpinBp, 0);
                decimal actualVapInNextBp = actualVap - actualVapInBp;
                // end Roshil add 2-23-2021


                // Roshil added 5-23-2017
                decimal VAPPerDayInBp = 0;
				if (vapdaysInBP > 0)
					VAPPerDayInBp = actualVapInBp / vapdaysInBP;
				int totalVAPDaysInNextBp = totalVAPDays - vapdaysInBP;
				decimal VAPPerDayInNextBp = 0;
				if (totalVAPDaysInNextBp > 0)
					VAPPerDayInNextBp = actualVapInNextBp / totalVAPDaysInNextBp;


				if (lstVAPDays.Count > 0)
				{     //Modified the code for Company VA implementaltion
					  //----------------------------------------------------------------
					if (!string.IsNullOrEmpty(GlobalSettings.CompanyVA) &&
						vacationPeriod.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
					{


						decimal maxPossibleVap = decimal.Parse(GlobalSettings.CompanyVA);
						if (vacationPeriod.EndAbsenceDate > GlobalSettings.SeniorityListMember.Absences[0].EndAbsenceDate)
						{
							maxPossibleVap +=
								vacationPeriod.EndAbsenceDate.Subtract(
									GlobalSettings.SeniorityListMember.Absences[0].EndAbsenceDate).Days *
								GlobalSettings.DailyVacPay;
						}


						if (maxPossibleVap > totTfp)
						{

							vAPPerday = (maxPossibleVap - totTfp) /
								lstVAPDays.Count;

						}

					}
					//----------------------------------------------------------------
					else if (GlobalSettings.DailyVacPay * totalVacationDays > totTfp)
					{

						//vAPPerday = ((GlobalSettings.DailyVacPay * totalVacationDays) - totTfp) / lstVAPDays.Count;


						//we only need to consider in the case of if the vacation end date overlap the bidperiod enddate
						if (vacationPeriod.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
						{

							vapdaysInBP = (lstVAPDays == null) ? 0 : lstVAPDays.Count(x => x <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);

							if (vapdaysInBP != 0)
							{
								if (GlobalSettings.DailyVacPay * totalVacationDaysInBP > totTfpinBp)
								{
									vAPPerdayInBP = ((GlobalSettings.DailyVacPay * totalVacationDaysInBP) - totTfpinBp) / vapdaysInBP;
								}
							}
							if (lstVAPDays.Count - vapdaysInBP != 0)
							{
								if (GlobalSettings.DailyVacPay * totalVacationDays > (totTfp + (vAPPerdayInBP * vapdaysInBP)))
								{
									vAPPerday = ((GlobalSettings.DailyVacPay * totalVacationDays) - (totTfp) - (vAPPerdayInBP * vapdaysInBP)) / (lstVAPDays.Count - vapdaysInBP);
								}
							}
						}
						else
						{
							vAPPerdayInBP = ((GlobalSettings.DailyVacPay * totalVacationDays) - totTfp) / lstVAPDays.Count;
							//vAPPerdayInBP = ((GlobalSettings.DailyVacPay * totalVacationDaysInBP) - totTfpinBp) / vapdaysInBP;
						}



					}
				}


				startDate = DateTime.MinValue;
				VAPHelper vapHelper = null;

				try
				{
					DateTime previousVapDay = DateTime.MinValue;
					foreach (var vAPDay in lstVAPDays)
					{

						if (startDate == DateTime.MinValue && vAPDay == GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
						{
							startDate = vAPDay;
							vapHelper = new VAPHelper();
							vapHelper.StartDate = startDate;
							vapHelper.EndDate = startDate;
                            vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * ((vAPDay <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate) ? VAPPerDayInBp : VAPPerDayInNextBp);
							//vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) *  ((vAPDay<=GlobalSettings.CurrentBidDetails.BidPeriodEndDate)?vAPPerdayInBP: vAPPerday);
							lstVAPHelper.Add(vapHelper);
							previousVapDay = startDate;
							vapHelper = new VAPHelper();
							startDate = DateTime.MinValue;


						}

						else if (startDate == DateTime.MinValue)
						{
							startDate = vAPDay;
							vapHelper = new VAPHelper();
							vapHelper.StartDate = startDate;
							previousVapDay = startDate;

						}
						else
						{
							if (vAPDay == GlobalSettings.CurrentBidDetails.BidPeriodEndDate && startDate.AddDays(1) != vAPDay)
							{
								//vapHelper.StartDate = startDate;
								vapHelper.EndDate = startDate;
								vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * ((startDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate) ? VAPPerDayInBp : VAPPerDayInNextBp);
								lstVAPHelper.Add(vapHelper);
								vapHelper = new VAPHelper();
								vapHelper.StartDate = vAPDay;
							}
							if (startDate.AddDays(1) != vAPDay || vAPDay == GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
							{
								if (vAPDay == GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
								{
									vapHelper.EndDate = vAPDay;

								}
								else {
									vapHelper.EndDate = startDate;

								}


								vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * ((startDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate) ? VAPPerDayInBp : VAPPerDayInNextBp);
								lstVAPHelper.Add(vapHelper);
								vapHelper = new VAPHelper();
								vapHelper.StartDate = vAPDay;

								if (vAPDay == GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
								{
									startDate = DateTime.MinValue;
									continue;

								}

							}
							startDate = vAPDay;
							previousVapDay = vAPDay;

						}

					}
					vapHelper.EndDate = startDate;
					vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * ((startDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate) ? VAPPerDayInBp : VAPPerDayInNextBp);
					lstVAPHelper.Add(vapHelper);
				}
				catch (Exception Exception)
				{

				}

                
			}



			return lstVAPHelper;

		}

//        private List<VAPHelper> GenerateVAPValues(List<VAHelper> lstVAHelper)
//        {
//            List<VAPHelper> lstVAPHelper = new List<VAPHelper>();

//            DateTime startDate = DateTime.MinValue;
//            List<DateTime> lstVAPDays = null;
//            int totalVacationDays;
//            decimal totTfp = 0;
//			decimal totTfpinBp = 0;
//            decimal vAPPerday = 0;




//            //foreach (Absense vacationPeriod in GlobalSettings.OrderedVacationDays)
//            foreach (Absense vacationPeriod in GlobalSettings.TempOrderedVacationDays)
//            {

//                startDate = vacationPeriod.StartAbsenceDate;
//                lstVAPDays = new List<DateTime>();
//                //Generate list of VAP days
//                //-------------------------------------------------
//                while (startDate <= vacationPeriod.EndAbsenceDate)
//                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             {

//                    if (!lstVAHelper.Any(x => x.StartDate <= startDate && x.EndDate >= startDate))
//                    {
//                        //if a VAP day then we will add it into VApDays list
//                        lstVAPDays.Add(startDate);
//                    }

//                    startDate = startDate.AddDays(1);
//                }
//                //-------------------------------------------------
//                totalVacationDays = vacationPeriod.EndAbsenceDate.Subtract(vacationPeriod.StartAbsenceDate).Days + 1;
//				int totalVacationDaysInBP = 0;
//				decimal vAPPerdayInBP = 0;
//				if (vacationPeriod.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate) {
//					totalVacationDaysInBP = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.Subtract (vacationPeriod.StartAbsenceDate).Days + 1;
//				}
//				vAPPerday = 0;
//                totTfp = lstVAHelper.Where(x => x.StartDate >= vacationPeriod.StartAbsenceDate && x.EndDate <= vacationPeriod.EndAbsenceDate).Sum(y => y.VA);
//				totTfpinBp= lstVAHelper.Where(x => x.StartDate >= vacationPeriod.StartAbsenceDate && x.EndDate <= vacationPeriod.EndAbsenceDate && x.EndDate<=GlobalSettings.CurrentBidDetails.BidPeriodEndDate).Sum(y => y.VA);
//                if (lstVAPDays.Count > 0)
//				{     //Modified the code for Company VA implementaltion
//					//----------------------------------------------------------------
//					if (!string.IsNullOrEmpty(GlobalSettings.CompanyVA) &&
//						vacationPeriod.StartAbsenceDate < GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
//					{


//						decimal maxPossibleVap = decimal.Parse(GlobalSettings.CompanyVA);
//						if (vacationPeriod.EndAbsenceDate > GlobalSettings.SeniorityListMember.Absences[0].EndAbsenceDate)
//						{
//							maxPossibleVap +=
//								vacationPeriod.EndAbsenceDate.Subtract(
//									GlobalSettings.SeniorityListMember.Absences[0].EndAbsenceDate).Days*
//								GlobalSettings.DailyVacPay;
//						}


//						if (maxPossibleVap > totTfp)
//						{

//							vAPPerday = (maxPossibleVap - totTfp) /
//								lstVAPDays.Count;

//						}

//					}
//					//----------------------------------------------------------------
//                    else if (GlobalSettings.DailyVacPay * totalVacationDays > totTfp)
//                    {
						
//                        vAPPerday = ((GlobalSettings.DailyVacPay * totalVacationDays) - totTfp) / lstVAPDays.Count;

////						int vapdaysInBP = 0;
////						//we only need to consider in the case of if the vacation end date overlap the bidperiod enddate
////						if (vacationPeriod.EndAbsenceDate > GlobalSettings.CurrentBidDetails.BidPeriodEndDate) 
////						{
////							
////							vapdaysInBP = (lstVAPDays == null) ? 0 : lstVAPDays.Count (x => x <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate);
////
////							if (vapdaysInBP != 0) 
////							{
////								if (GlobalSettings.DailyVacPay * totalVacationDaysInBP > totTfpinBp) 
////								{
////									vAPPerdayInBP = ((GlobalSettings.DailyVacPay * totalVacationDaysInBP) - totTfpinBp) / vapdaysInBP;
////								}
////							}
////							if (lstVAPDays.Count - vapdaysInBP != 0) 
////							{
////								if (GlobalSettings.DailyVacPay * totalVacationDays > (totTfp + (vAPPerdayInBP * vapdaysInBP))) 
////								{
////									vAPPerday = ((GlobalSettings.DailyVacPay * totalVacationDays) - (totTfp) - (vAPPerdayInBP * vapdaysInBP)) / (lstVAPDays.Count - vapdaysInBP);
////								}
////							}
////						} 
////						else
////						{
////							vAPPerday = ((GlobalSettings.DailyVacPay * totalVacationDays) - totTfp) / lstVAPDays.Count;
////						}



//                    }
//                }


//                startDate = DateTime.MinValue;
//                VAPHelper vapHelper=null;
             
//				try{
//                foreach (var vAPDay in lstVAPDays)
//                {

//					if (startDate == DateTime.MinValue && vAPDay==GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
//					{
//						startDate = vAPDay;
//						vapHelper = new VAPHelper();
//						vapHelper.StartDate = startDate;
//							vapHelper.EndDate=startDate;
//						//vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) *   vAPPerday;
//						vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) *  ((vAPDay<=GlobalSettings.CurrentBidDetails.BidPeriodEndDate)?vAPPerdayInBP: vAPPerday);
//						lstVAPHelper.Add(vapHelper);
//						vapHelper = new VAPHelper();
//						startDate = DateTime.MinValue;

//					}

//					 else if (startDate == DateTime.MinValue)
//                    {
//                        startDate = vAPDay;
//                        vapHelper = new VAPHelper();
//                        vapHelper.StartDate = startDate;

//                    }
//                    else
//                    {
//							if (startDate.AddDays(1) != vAPDay ||vAPDay==GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
//                        {
//								if(vAPDay==GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
//								{vapHelper.EndDate = vAPDay;
									
//								}
//								else{
//                            vapHelper.EndDate = startDate;

//								}
//                           // vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * vAPPerday;
//								//vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * ((startDate<=GlobalSettings.CurrentBidDetails.BidPeriodEndDate)?vAPPerdayInBP: vAPPerday);
//								vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) *  vAPPerday;
//                            lstVAPHelper.Add(vapHelper);
//                            vapHelper = new VAPHelper();
//                            vapHelper.StartDate = vAPDay;

//								if(vAPDay==GlobalSettings.CurrentBidDetails.BidPeriodEndDate)
//								{
//									startDate = DateTime.MinValue;
//									continue;

//								}
 
//                        }
//							startDate = vAPDay;

//                    }
                    
//                }
//                vapHelper.EndDate = startDate;
//				//vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * ((startDate<=GlobalSettings.CurrentBidDetails.BidPeriodEndDate)?vAPPerdayInBP: vAPPerday);;
//					vapHelper.VAPValue = (vapHelper.EndDate.Subtract(vapHelper.StartDate).Days + 1) * vAPPerday;
//                lstVAPHelper.Add(vapHelper);
//				}
//				catch(Exception Exception)
//				{

//				}

//            }
			


//            return lstVAPHelper;

//        }


        //public int AfterMidnightLandingTime(int landTime)
        //{
        //    if (landTime <= GlobalSettings.LastLandingMinus1440)
        //        return landTime += 1440;
        //    else
        //        return landTime % 1440;
        //}

        //public int DomicileTimeFromHerb(string domicile, DateTime date, int herb)
        //{
        //    bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(date);

        //    switch (domicile)
        //    {
        //        case "ATL":
        //            return herb + 60;       // EST = herb + 60
        //        case "BWI":
        //            return herb + 60;       // EST = herb + 60
        //        case "DAL":
        //            return herb;            // CST = herb 
        //        case "DEN":
        //            return herb - 60;       // MST = herb - 60
        //        case "HOU":
        //            return herb;            // CST = herb 
        //        case "LAS":
        //            return herb - 120;      // PST = herb - 120
        //        case "MCO":
        //            return herb + 60;       // EST = herb + 60
        //        case "MDW":
        //            return herb;            // CST = herb 
        //        case "OAK":
        //            return herb - 120;      // PST = herb - 120
        //        case "PHX":
        //            if (isDst)
        //                return herb - 120;  // PST = herb - 120
        //            else
        //                return herb - 60;   // MST = herb - 60
        //        default:
        //            return 1440;
        //    }
        //}



        private Trip GetTrip(string pairing)
        {
            Trip trip = null;
            trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
            if (trip == null)
            {
                trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
            }
			if (trip == null && pairing.Length > 6)
			{
				trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 6)).FirstOrDefault();
			}
            return trip;

        }

        private class VAHelper
        {
            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public decimal VA { get; set; }
			public bool IsInBp { get; set; }


        }

    }
}




