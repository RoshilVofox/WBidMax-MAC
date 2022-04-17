#region NameSpace
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.Utility;

#endregion

namespace WBid.WBidiPad.Utility
{
    public class QuicksetStateDetails
    {
        #region Properties
        public ObservableCollection<AppliedStates> AllAppliedStates { get; set; }
        
		List<ColumnDefinition> columnDefinitions;
        private AppliedStates appliedStates;
        private AppliedStateType appliedStateType;
        private List<AppliedStateType> WeightType;
        private List<AppliedStateType> ConstraintsType;
         QuickSetColumn QuickSetColumn;
        QuickSetCSW QuickSetCSW;
        #endregion

        #region Constands
        private const string Splitter = " , "; 
        #endregion

        #region Constructor
        public QuicksetStateDetails()
        {
            AllAppliedStates = new ObservableCollection<AppliedStates>();
        } 
        #endregion

        #region Public Method

        public ObservableCollection<AppliedStates> GetAppliedState(QuickSetCSW quickSetCsw)
        {
            QuickSetCSW = quickSetCsw;
            SetSortState();
            ConstraintsStates();
            AppliedWeights(quickSetCsw);
            return AllAppliedStates;
        }

        public ObservableCollection<AppliedStates> GetSelectedColumns(QuickSetColumn quickSetColumn)
        {
            QuickSetColumn = quickSetColumn;
            SetColumnDisplay();
            return AllAppliedStates;

        }
        #endregion

        #region Column
        private void SetColumnDisplay()
        {
            ConstraintsType = new List<AppliedStateType>();
            appliedStates = new AppliedStates();

			columnDefinitions = GlobalSettings.columndefinition;
            SetNormalSummaryView();
            SetVacationSummaryView();
            SetModernBidLineViewNormal();
            SetModernBidLineViewVacation();
            SetBidLineViewNormal();
            SetBidLineViewVacation();
            appliedStates.Key = "Columns";
            appliedStates.AppliedStateTypes = ConstraintsType;
            AllAppliedStates.Add(appliedStates);
        }
        private void SetNormalSummaryView()
        {
            appliedStateType = new AppliedStateType();

            string display = string.Empty;
            appliedStateType.Value = new List<string>();
            foreach (var item in QuickSetColumn.SummaryNormalColumns)
            {
				if(display!=string.Empty)
					display+=", ";
				var col = columnDefinitions.FirstOrDefault (x => x.Id == item.Id).DisplayName;
				if (display.Length + col.Length > 33) {
					appliedStateType.Value.Add (display);
					display = string.Empty;
				}
                display += columnDefinitions.FirstOrDefault(x => x.Id == item.Id).DisplayName;
            }
            appliedStateType.Value.Add(display);
            appliedStateType.Key = "LineSummary Normal";
            ConstraintsType.Add(appliedStateType);
        }
        private void SetVacationSummaryView()
        {
            appliedStateType = new AppliedStateType();

            string display = string.Empty;
            appliedStateType.Value = new List<string>();
            foreach (var item in QuickSetColumn.SummaryVacationColumns)
            {
				if(display!=string.Empty)
					display+=", ";
				var col = columnDefinitions.FirstOrDefault (x => x.Id == item.Id).DisplayName;
				if (display.Length + col.Length > 33) {
					appliedStateType.Value.Add (display);
					display = string.Empty;
				}
				display += columnDefinitions.FirstOrDefault(x => x.Id == item.Id).DisplayName;
            }
            appliedStateType.Value.Add(display);
            appliedStateType.Key = "LineSummary Vacation";
            ConstraintsType.Add(appliedStateType);
        }

        private void SetModernBidLineViewNormal()
        {
            appliedStateType = new AppliedStateType();

            string display = string.Empty;
            appliedStateType.Value = new List<string>();
            foreach (var item in QuickSetColumn.ModernNormalColumns)
            {
                if (item == 200)
                    display = "Fly Pay";
                else
                    display = columnDefinitions.FirstOrDefault(x => x.Id == item).DisplayName;
                appliedStateType.Value.Add(display);
            }
            //appliedStateType.Value.Add(display);
            appliedStateType.Key = "ModernBidLine View Normal";
            ConstraintsType.Add(appliedStateType);
        }

        private void SetModernBidLineViewVacation()
        {
            appliedStateType = new AppliedStateType();

            string display = string.Empty;
            appliedStateType.Value = new List<string>();
            foreach (var item in QuickSetColumn.ModernVacationColumns)
            {
                if (item == 200)
                    display = "Fly Pay";
                else
                    display = columnDefinitions.FirstOrDefault(x => x.Id == item).DisplayName;
                appliedStateType.Value.Add(display);
            }
            //  appliedStateType.Value.Add(display);
            appliedStateType.Key = "ModernBidLine View Vacation";
            ConstraintsType.Add(appliedStateType);
        }


        private void SetBidLineViewNormal()
        {
            appliedStateType = new AppliedStateType();

            string display = string.Empty;
            appliedStateType.Value = new List<string>();
            foreach (var item in QuickSetColumn.BidLineNormalColumns)
            {
                if (item == 200)
                    display = "Fly Pay";
                else
                    display = columnDefinitions.FirstOrDefault(x => x.Id == item).DisplayName;
                appliedStateType.Value.Add(display);
            }
            // appliedStateType.Value.Add(display);
            appliedStateType.Key = "BidLine View Normal";
            ConstraintsType.Add(appliedStateType);
        }

        private void SetBidLineViewVacation()
        {
            appliedStateType = new AppliedStateType();

            string display = string.Empty;
            appliedStateType.Value = new List<string>();
            foreach (var item in QuickSetColumn.BidLineVacationColumns)
            {
                if (item == 200)
                    display = "Fly Pay";
                else
                    display = columnDefinitions.FirstOrDefault(x => x.Id == item).DisplayName;
                appliedStateType.Value.Add(display);
            }
            //appliedStateType.Value.Add(display);
            appliedStateType.Key = "BidLine View Vacation";
            ConstraintsType.Add(appliedStateType);
        }
        #endregion

        #region Sorts
        private void SetSortState()
        {
            WeightType = new List<AppliedStateType>();
            appliedStates = new AppliedStates();
            appliedStateType = new AppliedStateType();

            var currentsort = QuickSetCSW.SortDetails.SortColumn;
            appliedStateType.Key = (appliedStateType.Key == "Line") ? "Line Number" : appliedStateType.Key;

            //------------------
            //Set Sort Name to View 
            if (currentsort == "Line")
                appliedStateType.Key = "Line Number";
            else if (currentsort == "LinePay")
                appliedStateType.Key = "Bottom Line Pay Per Total";
            else if (currentsort == "PayPerDay")
                appliedStateType.Key = "Most Pay Per Day";
            else if (currentsort == "PayPerDutyHour")
                appliedStateType.Key = "Most Pay Duty Hour";
            else if (currentsort == "PayPerFlightHour")
                appliedStateType.Key = "Most Pay Per Flight Hour";
            else if (currentsort == "PayPerTimeAway")
                appliedStateType.Key = "Most Pay Per Time Away From Base";
            else if (currentsort == "BlockSort")
                appliedStateType.Key = "Block Sort";
            else if (currentsort == "SelectedColumn")
                appliedStateType.Key = "Selected Column";
			else if (currentsort == "Manual")
				appliedStateType.Key = "Manual";

            if (appliedStateType.Key == "Block Sort")
            {
                //get the Block sort names from the block sort code stored in the global settings value
                List<string> blockSortitems = new List<string>();
				foreach(var items in QuickSetCSW.SortDetails.BlokSort)
				{
					if (items != string.Empty)
					{
						if (items == "30" || items == "31" || items == "32")
						{
							//commutability
							string name = WBidCollection.GetBlockSortListDataCSW().FirstOrDefault(x => x.Id == 18).Name;
							blockSortitems.Add(name);
						}
						else
						{
							var data = WBidCollection.GetBlockSortListData().FirstOrDefault(y => y.Id.ToString() == items);
							if (data != null)
							{
								blockSortitems.Add(data.Name);
							}
						}
					}
				}
//                QuickSetCSW.SortDetails.BlokSort.ForEach(x =>
//                {
//                    if (x != string.Empty)
//                        blockSortitems.Add(WBidCollection.GetBlockSortListData().First(y => y.Id.ToString() == x).Name);
//                });
                appliedStateType.Value = blockSortitems;
            }
            else if (appliedStateType.Key == "Selected Column")
            {
                var sortvalue = QuickSetCSW.SortDetails.SortColumnName;
                sortvalue += (QuickSetCSW.SortDetails.SortDirection == "Asc") ? " (Ascending)" : " (Descending)";
                appliedStateType.Value = new List<string>() { sortvalue.ToString() };
            }

            WeightType.Add(appliedStateType);

            appliedStates.Key = "Sort Option";
            appliedStates.AppliedStateTypes = WeightType;
            AllAppliedStates.Add(appliedStates);

        }
        #endregion

        #region Constraints
        /// <summary>
        /// Get all constriaint states
        /// </summary>
        private void ConstraintsStates()
        {
            ConstraintsType = new List<AppliedStateType>();
            appliedStates = new AppliedStates();
            CxWtState states = QuickSetCSW.CxWtState;
            states.Commute.Cx = false;

            QyuickConstraitns();
            if (states.ACChg.Cx)
            {
                AircraftChangeConstraints();
            }
           
            if (states.BDO.Cx)
            {
                BlockOfDaysOffConstraints();
            }
            if (states.DHD.Cx)
            {
                CommutableDeadHeadConstraints();
            }
            if (states.CL.Cx)
            {
                CommutableLineConstraints();
            }
			if (states.CLAuto == null)
				states.CLAuto = new StateStatus { Cx = false, Wt = false };
			if (states.CLAuto.Cx)
			{
				CommutableLineAutoConstraints();
			}
            //if (states.SDO.Cx)
            //{
            //    DaysOfMonthConstraint();
            //}
            if (states.DOW.Cx)
            {
                DaysOfWeekConstraint();
            }
            if (states.BulkOC.Cx)
            {
                OvernightCityBulkConstraint();
            }
            if (states.DP.Cx)
            {
                DutyPeriodConstraint();
            }
            if (states.EQUIP.Cx)
            {
                EquipemntTypeConstraint();
            }
            if (states.FLTMIN.Cx)
            {
                FlightTimeConstraint();
            }
            if (states.GRD.Cx)
            {
                GroundTimeConstraint();
            }
            if (states.LEGS.Cx)
            {
                LegsPerDutyPeriodConstraint();
            }
            if (states.LegsPerPairing.Cx)
            {
                LegsPerPairingConstraint();
            }
            if (states.NODO.Cx)
            {
                NumberOfDaysOffConstraint();
            }
            if (states.RON.Cx)
            {
                OverNightCitiesConstrain();
            }
			if (states.CitiesLegs == null)
				states.CitiesLegs = new StateStatus { Cx = false, Wt = false };
			if (states.CitiesLegs.Cx)
			{
				CityLegsConstrain();
			}
            //if (states.WtPDOFS.Cx)
            //{
            //    PartialDayOffConstraint();
            //}
            if (states.SDOW.Cx)
            {
                StartDayOfTheWeekConstraint();
            }
            if (states.Rest.Cx)
            {
                RestConstraint();
            }
            if (states.StartDay.Cx)
            {
                StartDayConstraint();
            }
            if (states.ReportRelease.Cx)
            {
                ReportReleaseConstraint();
            }
            if (states.PerDiem.Cx)
            {
                TimeAwayfromBaseConstraint();
            }
            if (states.TL.Cx)
            {
                TripLengthConstraint();
            }
            if (states.WB.Cx)
            {
                WorkBlockLengthLengthConstraint();
            }
            if (states.MP.Cx)
            {
                MinimumPayConstraint();
            }
            if (states.No3on3off.Cx)
            {
                No3On3offConstraint();
            }
            if (states.NOL.Cx)
            {
                // NoOverlapConstraint();
            }
            if (states.DHDFoL.Cx)
            {
                DeadHeadFirstOrLastConstraints();
            }
            if (states.Position.Cx)
            {
                // PositionConstriant();
            }
            if (states.InterConus.Cx)
            {
                InternationalNonConusContraint();
            }
            if (states.WorkDay.Cx)
            {
                WorkDaysConstraint();
            }
            appliedStates.Key = "Constraints";
            appliedStates.AppliedStateTypes = ConstraintsType;
            AllAppliedStates.Add(appliedStates);
        }

        private string GetConstraintType(int value)
        {
            string result = string.Empty;
            switch (value)
            {
                case 1:
                    result = "Less Than";
                    break;
                case 2:
                    result = "More Than";
                    break;
                case 3:
                    result = "More Than";
                    break;
                case 4:
                    result = "Not Equal To";
                    break;
                case 5:
                    result = "At After";
                    break;
                case 6:
                    result = "At Before";
                    break;
            }
            return result;
        }

        private string GetDeadheadConstraintValue(int value)
        {
            string result = string.Empty;
            switch (value)
            {
                case 1:
                    result = "Begin";
                    break;
                case 2:
                    result = "End";
                    break;
                case 3:
                    result = "Both";
                    break;
                case 4:
                    result = "Either";
                    break;

            }
            return result;
        }

        private void AircraftChangeConstraints()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.AircraftChanges.Type) + "  " + QuickSetCSW.Constraints.AircraftChanges.Value.ToString();

            appliedStateType.Key = "Aircrcraft Changes";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }
       

        private void BlockOfDaysOffConstraints()
        {
            appliedStateType = new AppliedStateType();
            var display = GetConstraintType(QuickSetCSW.Constraints.BlockOfDaysOff.Type) + "  " + QuickSetCSW.Constraints.BlockOfDaysOff.Value.ToString();
            appliedStateType.Key = "Block Of Days Off";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }
        private void CommutableDeadHeadConstraints()
        {
            appliedStateType = new AppliedStateType();

            Cx4Parameters cxDeadHead = QuickSetCSW.Constraints.DeadHeads;
            appliedStateType.Value = new List<string>();
            foreach (Cx4Parameter Cx4Parameter in cxDeadHead.LstParameter)
            {

                var citylist = GlobalSettings.WBidINIContent.Cities;
                var display = citylist.FirstOrDefault(x => x.Id == Convert.ToInt32(Cx4Parameter.SecondcellValue)).Name + "  " + GetDeadheadConstraintValue(Convert.ToInt32(Cx4Parameter.ThirdcellValue)) + "  " + GetConstraintType(Cx4Parameter.Type) + "  " + Cx4Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Commutable DeadHead";
            ConstraintsType.Add(appliedStateType);
        }

        private void DaysOfWeekConstraint()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxDaysOfWeek = QuickSetCSW.Constraints.DaysOfWeek;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxDaysOfWeek.lstParameters)
            {

				var display = Enum.Parse(typeof(DayOfWeek), Cx3Parameter.ThirdcellValue,true) + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value;
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Days of Week";
            ConstraintsType.Add(appliedStateType);
        }

        private void DeadHeadFirstOrLastConstraints()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxDeadHead = QuickSetCSW.Constraints.DeadHeadFoL;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxDeadHead.lstParameters)
            {

                var display = GetDeadheadConstraintValue(Convert.ToInt32(Cx3Parameter.ThirdcellValue)) + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "DH-First-last";
            ConstraintsType.Add(appliedStateType);
        }

        private void DutyPeriodConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.DutyPeriod.Type) + "  " + Helper.ConvertMinutesToFormattedHour(QuickSetCSW.Constraints.DutyPeriod.Value);

            appliedStateType.Key = "Duty Period";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }

        private void EquipemntTypeConstraint()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxequip = QuickSetCSW.Constraints.EQUIP;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxequip.lstParameters)
            {
                int equip = 0;

                var display = Cx3Parameter.ThirdcellValue + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value;
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Equipment Type";
            ConstraintsType.Add(appliedStateType);
        }
        private void FlightTimeConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.FlightMin.Type) + "  " + Helper.ConvertMinutesToFormattedHour(QuickSetCSW.Constraints.FlightMin.Value);

            appliedStateType.Key = "Flight Time";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }

        private void GroundTimeConstraint()
        {
            appliedStateType = new AppliedStateType();
            var display = Helper.ConvertMinutesToFormattedHour(Convert.ToInt32(QuickSetCSW.Constraints.GroundTime.ThirdcellValue)) + "  " + GetConstraintType(QuickSetCSW.Constraints.GroundTime.Type) + "  " + QuickSetCSW.Constraints.GroundTime.Value;
            appliedStateType.Key = "Ground Time";
            appliedStateType.Value = new List<string>() { display.ToString() };
            ConstraintsType.Add(appliedStateType);
        }

        private void InternationalNonConusContraint()
        {
            appliedStateType = new AppliedStateType();

            Cx2Parameters cxinterConus = QuickSetCSW.Constraints.InterConus;
            appliedStateType.Value = new List<string>();
            foreach (Cx2Parameter Cx2Parameter in cxinterConus.lstParameters)
            {
                var citylist = GlobalSettings.WBidINIContent.Cities;

                string type = (Cx2Parameter.Type == 1) ? "Intl" : "NonConus";
                string city = string.Empty;
                if (Cx2Parameter.Value == 0)
                    city = "All";
                else
                    city = citylist.FirstOrDefault(x => x.Id == Convert.ToInt32(Cx2Parameter.Value)).Name;
                var display = type + "  " + city;
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Intl - NonConus";
            ConstraintsType.Add(appliedStateType);
        }

        private void LegsPerDutyPeriodConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.LegsPerDutyPeriod.Type) + "  " + QuickSetCSW.Constraints.LegsPerDutyPeriod.Value.ToString();

            appliedStateType.Key = "Legs Per Duty Period";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }


        private void LegsPerPairingConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.LegsPerPairing.Type) + "  " + QuickSetCSW.Constraints.LegsPerPairing.Value.ToString();

            appliedStateType.Key = "Legs Per Pairing";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }

        private void MinimumPayConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.MinimumPay.Type) + "  " + QuickSetCSW.Constraints.MinimumPay.Value.ToString();

            appliedStateType.Key = "Min-Pay";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }

        private void NumberOfDaysOffConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.NumberOfDaysOff.Type) + "  " + QuickSetCSW.Constraints.NumberOfDaysOff.Value.ToString();

            appliedStateType.Key = "Number of Days Off";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }

        private void OverNightCitiesConstrain()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxOvernight = QuickSetCSW.Constraints.OverNightCities;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxOvernight.lstParameters)
            {

                var citylist = GlobalSettings.WBidINIContent.Cities;
                var display = citylist.FirstOrDefault(x => x.Id == Convert.ToInt32(Cx3Parameter.ThirdcellValue)).Name + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Overnight Cities";
            ConstraintsType.Add(appliedStateType);
        }
		private void CityLegsConstrain()
		{
			appliedStateType = new AppliedStateType();

			Cx3Parameters cxcityLegs = QuickSetCSW.Constraints.CitiesLegs;
			appliedStateType.Value = new List<string>();
			foreach (Cx3Parameter Cx3Parameter in cxcityLegs.lstParameters)
			{

				var citylist = GlobalSettings.WBidINIContent.Cities;
				var display = citylist.FirstOrDefault(x => x.Id == Convert.ToInt32(Cx3Parameter.ThirdcellValue)).Name + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value.ToString();
				appliedStateType.Value.Add(display);
			}
			appliedStateType.Key = "Cities-Legs";
			ConstraintsType.Add(appliedStateType);
		}

        private void PartialDayOffConstraint()
        {
            appliedStateType = new AppliedStateType();

            List<BindingManager> lstDateHelper = new List<BindingManager>();
            int dayCount = (GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate).Days + 1;
            DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
            for (int count = 1; count <= dayCount; count++)
            {
                lstDateHelper.Add(new BindingManager() { Id = count, Name = startDate.Day.ToString() + "-" + startDate.ToString("MMM") });
                startDate = startDate.AddDays(1);
            }
            for (int count = dayCount + 1, i = 1; count <= 34; count++, i++)
            {
                lstDateHelper.Add(new BindingManager() { Id = count, Name = startDate.Day.ToString() + "-" + startDate.ToString("MMM") });
                startDate = startDate.AddDays(1);
            }

            Cx4Parameters cxDeadHead = QuickSetCSW.Constraints.PDOFS;
            appliedStateType.Value = new List<string>();
            foreach (Cx4Parameter Cx4Parameter in cxDeadHead.LstParameter)
            {

                var citylist = GlobalSettings.WBidINIContent.Cities;
                string type = (Cx4Parameter.Type == 1) ? "at+after" : "at+before";
                var display = lstDateHelper.FirstOrDefault(x => x.Id == Convert.ToInt32(Cx4Parameter.SecondcellValue)).Name + "  " + citylist.FirstOrDefault(x => x.Id == Convert.ToInt32(Cx4Parameter.ThirdcellValue)).Name + "  " + type + "  " + Helper.ConvertMinutesToFormattedHour(Cx4Parameter.Value);
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "PDO";
            ConstraintsType.Add(appliedStateType);
        }
        private void RestConstraint()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxRest = QuickSetCSW.Constraints.Rest;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxRest.lstParameters)
            {
                string secondvalue = string.Empty;
                if (Cx3Parameter.ThirdcellValue == "1")
                    secondvalue = "All";
                else if (Cx3Parameter.ThirdcellValue == "2")
                    secondvalue = "InDom";
                else if (Cx3Parameter.ThirdcellValue == "3")
                    secondvalue = "AwayDom";

                var display = secondvalue + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Rest";
            ConstraintsType.Add(appliedStateType);
        }
        private void ReportReleaseConstraint()
        {
            appliedStateType = new AppliedStateType();

            ReportReleases cxReportReleases = QuickSetCSW.Constraints.ReportRelease;
            appliedStateType.Value = new List<string>();
            foreach (ReportRelease report in cxReportReleases.lstParameters)
            {
                string secondvalue = string.Empty;
                if (report.AllDays)
                    appliedStateType.Value.Add("All Days");
                else
                {
                    appliedStateType.Value.Add("Trip/Work Block");
                    if (report.First)
                        appliedStateType.Value.Add("First");
                    if (report.Last)
                        appliedStateType.Value.Add("Last");
                    if (report.NoMid)
                        appliedStateType.Value.Add("NoMid");
                }
                var display = "Report NET : " + Helper.ConvertMinuteToHHMM(report.Report);
                display += "   Release NLT : " + Helper.ConvertMinuteToHHMM(report.Release);

                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Report-Release";
            ConstraintsType.Add(appliedStateType);
        }
        private void StartDayConstraint()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxRest = QuickSetCSW.Constraints.StartDay;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxRest.lstParameters)
            {
                string secondvalue = string.Empty;
                if (Cx3Parameter.ThirdcellValue == "1")
                    secondvalue = "Block";
                else if (Cx3Parameter.ThirdcellValue == "2")
                    secondvalue = "Trip";
                string type = string.Empty;
                if (Cx3Parameter.Type == 1)
                    type = "Start On";
                else
                    type = "Does not Start";
                var display = secondvalue + "  " + type + "  " + Cx3Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Start Day";
            ConstraintsType.Add(appliedStateType);
        }
        private void StartDayOfTheWeekConstraint()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxRest = QuickSetCSW.Constraints.Rest;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxRest.lstParameters)
            {
                var display = Enum.Parse(typeof(DayOfWeek), Cx3Parameter.ThirdcellValue,true) + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Start Day of Week";
            ConstraintsType.Add(appliedStateType);
        }

        private void TimeAwayfromBaseConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.PerDiem.Type) + "  " + Helper.ConvertMinutesToFormattedHour(QuickSetCSW.Constraints.PerDiem.Value);

            appliedStateType.Key = "Time Away From Base";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }
        private void TripLengthConstraint()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxtripLength = QuickSetCSW.Constraints.TripLength;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxtripLength.lstParameters)
            {
                var display = Cx3Parameter.ThirdcellValue.ToString() + "Day" + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Trip Length";
            ConstraintsType.Add(appliedStateType);
        }
        private void WorkBlockLengthLengthConstraint()
        {
            appliedStateType = new AppliedStateType();

            Cx3Parameters cxworkLength = QuickSetCSW.Constraints.WorkBlockLength;
            appliedStateType.Value = new List<string>();
            foreach (Cx3Parameter Cx3Parameter in cxworkLength.lstParameters)
            {
                var display = Cx3Parameter.ThirdcellValue.ToString() + "Day" + "  " + GetConstraintType(Cx3Parameter.Type) + "  " + Cx3Parameter.Value.ToString();
                appliedStateType.Value.Add(display);
            }
            appliedStateType.Key = "Work Blk Length";
            ConstraintsType.Add(appliedStateType);
        }
        private void WorkDaysConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = GetConstraintType(QuickSetCSW.Constraints.WorkDay.Type) + "  " + QuickSetCSW.Constraints.PerDiem.Value;

            appliedStateType.Key = "Work Days";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }
        private void No3On3offConstraint()
        {
            appliedStateType = new AppliedStateType();

            var display = (QuickSetCSW.Constraints.No3On3Off.Value == 1) ? "No 3-on-3-off" : "Only 3-on-3-off";

            appliedStateType.Key = "3-on-3-off";
            appliedStateType.Value = new List<string>() { display.ToString() };

            ConstraintsType.Add(appliedStateType);
        }
        private void CommutableLineConstraints()
        {
            appliedStateType = new AppliedStateType();
            CxCommutableLine cxCommutableLine = QuickSetCSW.Constraints.CL;

            string commutableline = (cxCommutableLine.AnyNight) ? "Constrain If ANY night is spent in Domicile" : "Constrain if BOTH ends of any pairing is spent in Domicile";
            appliedStateType.Value = new List<string>() { commutableline };
            if (cxCommutableLine.CommuteToHome && cxCommutableLine.CommuteToWork)
            {
                appliedStateType.Value.Add("Week Day : " + " Check In = " + cxCommutableLine.MondayThu.Checkin.ToString("d4") + " ,  Return = " + cxCommutableLine.MondayThu.BackToBase.ToString("d4"));
                appliedStateType.Value.Add("Friday       : " + " Check In = " + cxCommutableLine.Friday.Checkin.ToString("d4") + " ,  Return = " + cxCommutableLine.Friday.BackToBase.ToString("d4"));
                appliedStateType.Value.Add("Saturday   : " + " Check In = " + cxCommutableLine.Saturday.Checkin.ToString("d4") + " ,  Return = " + cxCommutableLine.Saturday.BackToBase.ToString("d4"));
                appliedStateType.Value.Add("Sunday     : " + " Check In = " + cxCommutableLine.Sunday.Checkin.ToString("d4") + " ,  Return = " + cxCommutableLine.Sunday.BackToBase.ToString("d4"));

            }
            else if (cxCommutableLine.CommuteToHome)
            {
                appliedStateType.Value.Add("Ignore Commute to Work");

                appliedStateType.Value.Add("Week Day : " + " Return = " + cxCommutableLine.MondayThu.BackToBase.ToString("d4"));
                appliedStateType.Value.Add("Friday       : " + " Return = " + cxCommutableLine.Friday.BackToBase.ToString("d4"));
                appliedStateType.Value.Add("Saturday   : " + "  Return = " + cxCommutableLine.Saturday.BackToBase.ToString("d4"));
                appliedStateType.Value.Add("Sunday     : " + "  Return = " + cxCommutableLine.Sunday.BackToBase.ToString("d4"));


            }
            else if (cxCommutableLine.CommuteToWork)
            {
                appliedStateType.Value.Add("Ignore Commute To Home");

                appliedStateType.Value.Add("Week Day : " + " Check In = " + cxCommutableLine.MondayThu.Checkin.ToString("d4"));
                appliedStateType.Value.Add("Friday       : " + " Check In = " + cxCommutableLine.Friday.Checkin.ToString("d4"));
                appliedStateType.Value.Add("Saturday   : " + " Check In = " + cxCommutableLine.Saturday.Checkin.ToString("d4"));
                appliedStateType.Value.Add("Sunday     : " + " Check In = " + cxCommutableLine.Sunday.Checkin.ToString("d4"));

            }


            appliedStateType.Key = "Commutable line";
            ConstraintsType.Add(appliedStateType);
        }
		private void CommutableLineAutoConstraints()
		{
			appliedStateType = new AppliedStateType();
			FtCommutableLine clAuto = QuickSetCSW.Constraints.CLAuto;
			appliedStateType.Value = new List<string>();

			if (clAuto != null)
			{
				appliedStateType.Value.Add(((clAuto.NoNights) ? "No Nights in Middle" : "Nights in Middle") + Splitter + ((clAuto.ToWork) ? "To Work "+Splitter : "")+ ((clAuto.ToHome) ? "To Home" : "To Home Off"));
			}
			appliedStateType.Key = "Commutable Line-Auto";
			ConstraintsType.Add(appliedStateType);

		}
        private string GetTypeforCommutableLine(int count)
        {
            string type = string.Empty;
            switch (count)
            {
                case 0:
                    type = "Week Day : ";
                    break;
                case 1:
                    type = "Friday       : ";
                    break;
                case 2:
                    type = "Saturday   : ";
                    break;
                case 3:
                    type = "Sunday     : ";
                    break;
            }
            return type;
        }

       
        /// <summary>
        /// Days of month Constrain
        /// </summary>
        private void OvernightCityBulkConstraint()
        {
            appliedStateType = new AppliedStateType();
            appliedStateType.Value = new List<string>();
            List<int> overNightNo = QuickSetCSW.Constraints.BulkOvernightCity.OverNightNo;
            List<int> overNightYes = QuickSetCSW.Constraints.BulkOvernightCity.OverNightYes;

            string overnightYesstring = string.Empty;
            var citylist = GlobalSettings.WBidINIContent.Cities;
			foreach (var item in overNightYes) 
			{
				overnightYesstring += citylist.FirstOrDefault(y => y.Id == item).Name + " , ";
			}
           // overNightYes.ForEach(x => { overnightYesstring += citylist.FirstOrDefault(y => y.Id == x).Name + " , "; });
            overnightYesstring = overnightYesstring.TrimEnd(',');
            appliedStateType.Value.Add("OverNight Yes : " + overnightYesstring);

            string overnightNostring = string.Empty;
			foreach (var item in overNightNo) {
				overnightNostring += citylist.FirstOrDefault(y => y.Id == item).Name + " , "; 
			}
           // overNightNo.ForEach(x => { overnightNostring += citylist.FirstOrDefault(y => y.Id == x).Name + " , "; });
            overnightNostring = overnightNostring.TrimEnd(',');
            appliedStateType.Value.Add("OverNight No : " + overnightNostring);

            appliedStateType.Key = "Overnight Cities-Bulk";
            ConstraintsType.Add(appliedStateType);
        }
        private void QyuickConstraitns()
        {
			appliedStateType = new AppliedStateType();
			appliedStateType.Value = new List<string>();

			string line1 = string.Empty;
			if (QuickSetCSW.Constraints.Hard)
				line1 += " Hard  ,";
			if (QuickSetCSW.Constraints.Blank)
				line1 += " Blank  ,";
			if (QuickSetCSW.Constraints.Reserve)
				line1 += " Reserve ,";
			if (QuickSetCSW.Constraints.Ready)
				line1 += " Ready ,";
			if (QuickSetCSW.Constraints.International)
				line1 += " International ,";
			if (QuickSetCSW.Constraints.NonConus)
				line1 += " Non-Conus ,";
            if (QuickSetCSW.Constraints.ETOPS)
                line1 += " ETOPS ,";
            if (QuickSetCSW.Constraints.ReserveETOPS)
                line1 += " ETOPS - Res ,";

            line1 = line1.TrimEnd(',');
			if (line1 != string.Empty)
				appliedStateType.Value.Add(line1);
			string line2 = string.Empty;
			if (QuickSetCSW.CxWtState.AMPMMIX.AM)
				line2 += " AM ,";
			if (QuickSetCSW.CxWtState.AMPMMIX.PM)
				line2 += " PM ,";
			if (QuickSetCSW.CxWtState.AMPMMIX.MIX)
				line2 += " MIX ,";


			line2 = line2.TrimEnd(',');
			if (line2 != string.Empty)
				appliedStateType.Value.Add(line2);


			string line3 = string.Empty;
			if (QuickSetCSW.CxWtState.TripLength.Turns)
				line3 += " Turns ,";
			if (QuickSetCSW.CxWtState.TripLength.Twoday)
				line3 += " 2-Days ,";
			if (QuickSetCSW.CxWtState.TripLength.ThreeDay)
				line3 += " 3-Days ,";
			if (QuickSetCSW.CxWtState.TripLength.FourDay)
				line3 += " 4-Days ,";
			line3 = line3.TrimEnd(',');
			if (line3 != string.Empty)
				appliedStateType.Value.Add(line3);

			string line4 = string.Empty;
			if (QuickSetCSW.CxWtState.DaysOfWeek.MON)
				line4 += " Mon ,";
			if (QuickSetCSW.CxWtState.DaysOfWeek.TUE)
				line4 += " Tue ,";
			if (QuickSetCSW.CxWtState.DaysOfWeek.WED)
				line4 += " Wed ,";
			if (QuickSetCSW.CxWtState.DaysOfWeek.THU)
				line4 += " Thu ,";
			if (QuickSetCSW.CxWtState.DaysOfWeek.FRI)
				line4 += " Fri ,";
			if (QuickSetCSW.CxWtState.DaysOfWeek.SAT)
				line4 += " Sat ,";
			if (QuickSetCSW.CxWtState.DaysOfWeek.SUN)
				line4 += " Sun ,";
			line4 = line4.TrimEnd(',');
			if (line4 != string.Empty)
				appliedStateType.Value.Add(line4);

			appliedStateType.Key = "QuickConstraints";
			ConstraintsType.Add(appliedStateType);
        }
        #endregion

        #region Weights

        #region Main Weights

        /// <summary>
        /// Get Applied Weight Details
        /// </summary>
        /// <param name="quickSetCsw"></param>
        private void AppliedWeights(QuickSetCSW quickSetCsw)
        {
            var appliedStates = new AppliedStates {Key = "Weights", AppliedStateTypes = new List<AppliedStateType>()};
            CxWtState cxWtState = quickSetCsw.CxWtState;

            cxWtState.Commute.Wt = false;

            //Aircraft changes
            //--------------------------------------------------------------------------------------
            if (cxWtState.ACChg.Wt)
            {
                appliedStates.AppliedStateTypes.Add(AircraftChangeWeight(quickSetCsw.Weights.AirCraftChanges));
            }
            //--------------------------------------------------------------------------------------

            //Am/PM
            //--------------------------------------------------------------------------------------
            if (cxWtState.AMPM.Wt)
            {
                appliedStates.AppliedStateTypes.Add(AmPmWeights(quickSetCsw.Weights.AM_PM.lstParameters));
            }
            //--------------------------------------------------------------------------------------

            //Block of Days Off
            //--------------------------------------------------------------------------------------
            if (cxWtState.BDO.Wt)
            {
                appliedStates.AppliedStateTypes.Add(BlockOffDaysOffWeight(quickSetCsw.Weights.BDO.lstParameters));
            }
            //--------------------------------------------------------------------------------------
            //Comut DeadHead  
            //--------------------------------------------------------------------------------------
            if (cxWtState.DHD.Wt)
            {
                appliedStates.AppliedStateTypes.Add(ComutDHsWeight(quickSetCsw.Weights.DHD.lstParameters));
            }
            //--------------------------------------------------------------------------------------

            //Commutable Line  
            //--------------------------------------------------------------------------------------
            if (cxWtState.CL.Wt)
            {
                appliedStates.AppliedStateTypes.Add(CommutableLineWeight(quickSetCsw.Weights.CL));
            }



            //Days of week   
            //--------------------------------------------------------------------------------------
            if (cxWtState.DOW.Wt)
            {
                appliedStates.AppliedStateTypes.Add(DaysOfWeekWeight(quickSetCsw.Weights.DOW));
            }
            //--------------------------------------------------------------------------------------


            //Deadhead First or last
            //--------------------------------------------------------------------------------------
            if (cxWtState.DHDFoL.Wt)
            {
                appliedStates.AppliedStateTypes.Add(DeadheadFisrtLastWeigh(quickSetCsw.Weights.DHDFoL.lstParameters));

            }
            //--------------------------------------------------------------------------------------

            //Duty Period
            //--------------------------------------------------------------------------------------
            if (cxWtState.DP.Wt)
            {
                appliedStates.AppliedStateTypes.Add(DutyPeriodWeight(quickSetCsw.Weights.DP.lstParameters));

            }

            //--------------------------------------------------------------------------------------

            // Equipment Type  weight 
            //--------------------------------------------------------------------------------------
            if (cxWtState.EQUIP.Wt)
            {
                appliedStates.AppliedStateTypes.Add(EquipmentTypeWeight(quickSetCsw.Weights.EQUIP.lstParameters));

            }
            //--------------------------------------------------------------------------------------
            // ETOPS weight 
            //--------------------------------------------------------------------------------------
            if (cxWtState.ETOPS.Wt)
            {
                appliedStates.AppliedStateTypes.Add(ETOPSWeight(quickSetCsw.Weights.ETOPS.lstParameters));

            }
            //--------------------------------------------------------------------------------------

            // ETOPSRes  weight 
            //--------------------------------------------------------------------------------------
            if (cxWtState.ETOPSRes.Wt)
            {
                appliedStates.AppliedStateTypes.Add(ETOPSResWeight(quickSetCsw.Weights.ETOPSRes.lstParameters));

            }
            //--------------------------------------------------------------------------------------


            // FlightTime  weight Calculation
            //--------------------------------------------------------------------------------------
            if (cxWtState.FLTMIN.Wt)
            {
                appliedStates.AppliedStateTypes.Add(FlightTimeWeight(quickSetCsw.Weights.FLTMIN.lstParameters));

            }
            //--------------------------------------------------------------------------------------
            // Ground  weight Calculation
            ////--------------------------------------------------------------------------------------
            if (cxWtState.GRD.Wt)
            {
                appliedStates.AppliedStateTypes.Add(GroundTimeWeight(quickSetCsw.Weights.GRD.lstParameters));

            }
            //--------------------------------------------------------------------------------------

            // International non-conus 
            //--------------------------------------------------------------------------------------
            if (cxWtState.InterConus.Wt)
            {
                appliedStates.AppliedStateTypes.Add(InternationalNonConusWeight(quickSetCsw.Weights.InterConus.lstParameters));

            }
            //--------------------------------------------------------------------------------------

            // Largest Block  days Off 
            //---------------------------------------------------------------------------------------------
            if (cxWtState.LrgBlkDaysOff.Wt)
            {
                appliedStates.AppliedStateTypes.Add(LargestBlockOfDaysoffWeight(quickSetCsw.Weights.LrgBlkDayOff));

            }
            // LegsPerDutyPeriod 
            //---------------------------------------------------------------------------------------------
            if (cxWtState.LEGS.Wt)
            {
                appliedStates.AppliedStateTypes.Add(LegsPerDutyPeriodWeight(quickSetCsw.Weights.LEGS.lstParameters));

            }


            // LegsPerPairing 
            //--------------------------------------------------------------------------------------
            if (cxWtState.LegsPerPairing.Wt)
            {
                appliedStates.AppliedStateTypes.Add(LegsPerPairingWeight(quickSetCsw.Weights.WtLegsPerPairing.lstParameters));

            }
            //--------------------------------------------------------------------------------------
            // NormalizeDaysOff  
            //--------------------------------------------------------------------------------------
            if (cxWtState.NormalizeDays.Wt)
            {
                appliedStates.AppliedStateTypes.Add(NormalizeDaysOffWeight(quickSetCsw.Weights.NormalizeDaysOff.Weight));

            }
            //--------------------------------------------------------------------------------------

            // NumberofDays Off  
            //--------------------------------------------------------------------------------------

            if (cxWtState.NODO.Wt)
            {
                appliedStates.AppliedStateTypes.Add(NumberOfDaysOffWeight(quickSetCsw.Weights.NODO.lstParameters));


            }
            //--------------------------------------------------------------------------------------

            // Overnight cities  weight Calculation
            //--------------------------------------------------------------------------------------

            if (cxWtState.RON.Wt)
            {
                appliedStates.AppliedStateTypes.Add(OvernightCitiesWeight(quickSetCsw.Weights.RON.lstParameters));

            }
            //--------------------------------------------------------------------------------------

			if (cxWtState.CitiesLegs.Wt)
			{
				appliedStates.AppliedStateTypes.Add(CitiesLegsWeight(quickSetCsw.Weights.CitiesLegs.lstParameters));

			}
			//---

            // OvernightCities Bulk  
            //--------------------------------------------------------------------------------------
            if (cxWtState.BulkOC.Wt)
            {
                appliedStates.AppliedStateTypes.Add(OvernightCitiesBulkWeight(quickSetCsw.Weights.OvernightCitybulk));

            }
            //--------------------------------------------------------------------------------------

            // Position 
            //--------------------------------------------------------------------------------------
            if (cxWtState.Position != null && cxWtState.Position.Wt)
            {
                appliedStates.AppliedStateTypes.Add(PositionWeight(quickSetCsw.Weights.POS.lstParameters));

            }
            //--------------------------------------------------------------------------------------
            // Rest 
            //--------------------------------------------------------------------------------------
            if (cxWtState.Rest.Wt)
            {
                appliedStates.AppliedStateTypes.Add(RestWeight(quickSetCsw.Weights.WtRest.lstParameters));

            }
            // Start Days  Off  
            //--------------------------------------------------------------------------------------
            if (cxWtState.SDOW.Wt)
            {
                appliedStates.AppliedStateTypes.Add(StartDateOftheWeekWeight(quickSetCsw.Weights.SDOW.lstParameters));
            }
            //--------------------------------------------------------------------------------------
            // Time away from base  
            //--------------------------------------------------------------------------------------
            if (cxWtState.PerDiem.Wt)
            {

                appliedStates.AppliedStateTypes.Add(TimeAwayFromBaseWeight(quickSetCsw.Weights.PerDiem));
            }
            //--------------------------------------------------------------------------------------

            // Trip Length weight Calculation
            //--------------------------------------------------------------------------------------
            if (cxWtState.TL.Wt)
            {
                appliedStates.AppliedStateTypes.Add(TripLengthWeight(quickSetCsw.Weights.TL.lstParameters));

            }
            //--------------------------------------------------------------------------------------
            // Work Block Length 
            //--------------------------------------------------------------------------------------
            if (cxWtState.WB.Wt)
            {
                appliedStates.AppliedStateTypes.Add(WorkBlockLengthWeight(quickSetCsw.Weights.WB.lstParameters));


            }
            // Work Days 
            //---------------------------------------------------------------------------------------------
            if (cxWtState.WorkDay.Wt)
            {
                appliedStates.AppliedStateTypes.Add(WorkDaysWeight(quickSetCsw.Weights.WorkDays.lstParameters));

            }

            //--------------------------------------------------------------------------------------

            if (appliedStates.AppliedStateTypes.Count > 0)
            {
                AllAppliedStates.Add(appliedStates);
            }
            //--------------------------------------------------------------------------------------


        }
        #endregion

        #region Induvidual Weights

        /// <summary>
        /// Aircraft CHanges Weight Details
        /// </summary>
        /// <param name="airCraftChanges"></param>
        /// <returns></returns>
        private AppliedStateType AircraftChangeWeight(Wt3Parameter airCraftChanges)
        {
            var appliedStateType = new AppliedStateType();
            string aircraftchange = Convert.ToString(((WeightType)airCraftChanges.SecondlValue));
            aircraftchange += Splitter + airCraftChanges.ThrirdCellValue + " chg";
            aircraftchange += Splitter + airCraftChanges.Weight;
            appliedStateType.Key = "Aircraft Changes";
            appliedStateType.Value = new List<string> { aircraftchange };
            return appliedStateType;

        }

        /// <summary>
        /// AM/PM weight Details
        /// </summary>
        /// <param name="aMpm"></param>
        /// <returns></returns>
        private AppliedStateType AmPmWeights(IEnumerable<Wt2Parameter> aMpm)
        {
            var appliedStateType = new AppliedStateType {Key = "Am/Pm", Value = new List<string>()};

            foreach (Wt2Parameter item in aMpm)
            {
                appliedStateType.Value.Add(Convert.ToString((AMPMType)Convert.ToInt32(item.Type)) + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        /// <summary>
        /// Block of Dyas off weight Details
        /// </summary>
        /// <param name="blockOfDaysOff"></param>
        /// <returns></returns>
        private AppliedStateType BlockOffDaysOffWeight(IEnumerable<Wt3Parameter> blockOfDaysOff)
        {
            var appliedStateType = new AppliedStateType {Key = "Block off Days Off", Value = new List<string>()};
            foreach (Wt3Parameter item in blockOfDaysOff)
            {
                appliedStateType.Value.Add(Convert.ToString(((WeightType)Convert.ToInt32(item.SecondlValue))) + Splitter + "blk " + item.ThrirdCellValue + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        private AppliedStateType ComutDHsWeight(IEnumerable<Wt3Parameter> comutDHs)
        {
            var appliedStateType = new AppliedStateType {Key = "Comut DHs", Value = new List<string>()};

            foreach (Wt3Parameter item in comutDHs)
            {
                var cityName = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == Convert.ToInt32(item.SecondlValue));
                string city = string.Empty;
                if (cityName != null)
                {
                    city = cityName.Name;
                }
                appliedStateType.Value.Add(city + Splitter + GetBeginEndType(item.ThrirdCellValue) + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        private AppliedStateType CommutableLineWeight(WtCommutableLine commutLine)
        {
            var appliedStateType = new AppliedStateType {Key = "Commutable Lines", Value = new List<string>()};

            bool beginCheck = false;
            bool endCheck = false;

            switch (commutLine.Type)
            {
                //All
                case 1:
                    beginCheck = true;
                    endCheck = true;
                    break;
                //Backend
                case 2:
                    endCheck = true;
                    break;
                //Frontend
                case 3:
                    beginCheck = true;
                    break;
            }


            if (!beginCheck)
                appliedStateType.Value.Add("Ignore Commute to Work ");
            if (!endCheck)
                appliedStateType.Value.Add("Ignore Commute to Home ");


            if (beginCheck && endCheck)
            {

                appliedStateType.Value.Add("Week Day\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[0].Checkin) + " - " + Helper.ConvertMinuteToHHMM(commutLine.TimesList[0].BackToBase));
                appliedStateType.Value.Add("Friday  \t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[1].Checkin) + " - " + Helper.ConvertMinuteToHHMM(commutLine.TimesList[1].BackToBase));
                appliedStateType.Value.Add("Saturday\t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[2].Checkin) + " - " + Helper.ConvertMinuteToHHMM(commutLine.TimesList[2].BackToBase));
                appliedStateType.Value.Add("Sunday  \t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[3].Checkin) + " - " + Helper.ConvertMinuteToHHMM(commutLine.TimesList[3].BackToBase));
            }
            else if (beginCheck)
            {

                appliedStateType.Value.Add("Week Day\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[0].Checkin));
                appliedStateType.Value.Add("Friday  \t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[1].Checkin));
                appliedStateType.Value.Add("Saturday\t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[2].Checkin));
                appliedStateType.Value.Add("Sunday  \t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[3].Checkin));
            }
            else if (endCheck)
            {
                appliedStateType.Value.Add("Week Day\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[0].BackToBase));
                appliedStateType.Value.Add("Friday  \t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[1].BackToBase));
                appliedStateType.Value.Add("Saturday\t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[2].BackToBase));
                appliedStateType.Value.Add("Sunday  \t\t" + Helper.ConvertMinuteToHHMM(commutLine.TimesList[3].BackToBase));
            }

            appliedStateType.Value.Add("In Domicile = " + commutLine.InDomicile + " - " + "Both Ends = " + commutLine.BothEnds);



            return appliedStateType;
        }

        private AppliedStateType DaysOfWeekWeight(WtDaysOfWeek wtDaysOfWeek)
        {
            var appliedStateType = new AppliedStateType {Key = "Days of Week", Value = new List<string>()};

            string buttonState = wtDaysOfWeek.IsOff ? "Off" : "On";
            appliedStateType.Value.Add(buttonState);
            foreach (var item in wtDaysOfWeek.lstWeight)
            {
                appliedStateType.Value.Add(Convert.ToString((Dow)item.Key) + Splitter + item.Value);
            }
            return appliedStateType;
        }

        private AppliedStateType DeadheadFisrtLastWeigh(IEnumerable<Wt2Parameter> deadHead)
        {
            var appliedStateType = new AppliedStateType {Key = "DH - first - last", Value = new List<string>()};


            foreach (Wt2Parameter item in deadHead)
            {
                appliedStateType.Value.Add(Convert.ToString((DeadheadType)Convert.ToInt32(item.Type)) + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        private AppliedStateType DutyPeriodWeight(IEnumerable<Wt3Parameter> dutyPeriod)
        {
            var appliedStateType = new AppliedStateType {Key = "Duty Period", Value = new List<string>()};

			var timeList = WBidCollection.GenerateTimeList();
            foreach (Wt3Parameter item in dutyPeriod)
            {
                var time = timeList.FirstOrDefault(x => x.Id == item.ThrirdCellValue);
                string timeStr = string.Empty;
                if (time != null)
                {
                    timeStr = time.Name;
                }
                appliedStateType.Value.Add(Convert.ToString((DutyPeriodType)Convert.ToInt32(item.SecondlValue)) + Splitter + timeStr + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        private AppliedStateType EquipmentTypeWeight(IEnumerable<Wt3Parameter> equipmentType)
        {
            var appliedStateType = new AppliedStateType {Key = "Equipment Type", Value = new List<string>()};


            foreach (Wt3Parameter item in equipmentType)
            {

                appliedStateType.Value.Add(item.SecondlValue + Splitter + item.ThrirdCellValue + " leg" + Splitter + item.Weight);
            }
            return appliedStateType;
        }
        private AppliedStateType ETOPSWeight(IEnumerable<Wt1Parameter> list)
        {
            var appliedStateType = new AppliedStateType { Key = "ETOPS", Value = new List<string>() };


            foreach (Wt1Parameter item in list)
            {

                appliedStateType.Value.Add(item.Weight+Splitter );
            }
            return appliedStateType;
        }
        private AppliedStateType ETOPSResWeight(IEnumerable<Wt1Parameter> list)
        {
            var appliedStateType = new AppliedStateType { Key = "ETOPS-Res", Value = new List<string>() };


            foreach (Wt1Parameter item in list)
            {

                appliedStateType.Value.Add(item.Weight+Splitter);
            }
            return appliedStateType;
        }
        private AppliedStateType FlightTimeWeight(IEnumerable<Wt3Parameter> flightTime)
        {
            var appliedStateType = new AppliedStateType {Key = "Flight Time", Value = new List<string>()};


            foreach (Wt3Parameter item in flightTime)
            {

                appliedStateType.Value.Add(Convert.ToString((WeightType)Convert.ToInt32(item.SecondlValue)) + Splitter + item.ThrirdCellValue + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        private AppliedStateType GroundTimeWeight(IEnumerable<Wt3Parameter> groundTime)
        {
            var appliedStateType = new AppliedStateType {Key = "Ground Time", Value = new List<string>()};

			var timeList = WBidCollection.GenerateGroundTimeList();
            foreach (Wt3Parameter item in groundTime)
            {
                var time = timeList.FirstOrDefault(x => x.Id == item.SecondlValue);
                string timeStr = string.Empty;
                if (time != null)
                {
                    timeStr = time.Name;
                }

                appliedStateType.Value.Add(timeStr + Splitter + item.ThrirdCellValue + " Occur" + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        private AppliedStateType InternationalNonConusWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Intl - NonConus", Value = new List<string>()};

            var cityList = new ObservableCollection<BindingManager>(GlobalSettings.WBidINIContent.Cities
                                                  .Where(x => (x.International || x.NonConus))
                                                  .Select(y => new BindingManager
                                                  {
                                                      Name = y.Name,
                                                      Id = y.Id
                                                  }
                                                          ));


            cityList.Insert(0, new BindingManager { Id = 0, Name = "All NonConus" });
            cityList.Insert(0, new BindingManager { Id = -1, Name = "All Intl" });

            foreach (Wt2Parameter item in list)
            {
                var city = cityList.FirstOrDefault(x => x.Id == Convert.ToInt32(item.Type));
                var cityName = string.Empty;
                if (city != null)
                {
                    cityName = city.Name;
                }
                appliedStateType.Value.Add(cityName + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        private AppliedStateType LargestBlockOfDaysoffWeight(Wt2Parameter wt2Parameter)
        {
            var appliedStateType = new AppliedStateType
            {
                Key = "Lagest Block of Days Off",
                Value = new List<string> {wt2Parameter.Weight.ToString(CultureInfo.InvariantCulture)}
            };
            return appliedStateType;

        }

        private AppliedStateType LegsPerDutyPeriodWeight(IEnumerable<Wt3Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Legs Per Duty Period", Value = new List<string>()};
            foreach (Wt3Parameter item in list)
            {
                appliedStateType.Value.Add(Convert.ToString(((WeightType)Convert.ToInt32(item.SecondlValue))) + Splitter + item.ThrirdCellValue + " leg" + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        private AppliedStateType LegsPerPairingWeight(IEnumerable<Wt3Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Legs Per Pairing", Value = new List<string>()};
            foreach (Wt3Parameter item in list)
            {
                appliedStateType.Value.Add(Convert.ToString(((LegsPerPairingType)Convert.ToInt32(item.SecondlValue))) + Splitter + item.ThrirdCellValue + " leg" + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        private AppliedStateType NormalizeDaysOffWeight(decimal weight)
        {
            var appliedStateType = new AppliedStateType
            {
                Key = "Normalise Days Off",
                Value = new List<string> {weight.ToString(CultureInfo.InvariantCulture)}
            };
            return appliedStateType;
        }

        private AppliedStateType NumberOfDaysOffWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Number of Days Off", Value = new List<string>()};

            foreach (Wt2Parameter item in list)
            {
                appliedStateType.Value.Add(item.Type + " Off" + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        private AppliedStateType OvernightCitiesWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Overnight Cities", Value = new List<string>()};

            foreach (Wt2Parameter item in list)
            {
                var cityName = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == item.Type);
                string city = string.Empty;
                if (cityName != null)
                {
                    city = cityName.Name;
                }
                appliedStateType.Value.Add(city + Splitter + item.Weight);
            }

            return appliedStateType;
        }
		private AppliedStateType CitiesLegsWeight(IEnumerable<Wt2Parameter> list)
		{
			var appliedStateType = new AppliedStateType {Key = "Cities-Legs", Value = new List<string>()};

			foreach (Wt2Parameter item in list)
			{
				var cityName = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == item.Type);
				string city = string.Empty;
				if (cityName != null)
				{
					city = cityName.Name;
				}
				appliedStateType.Value.Add(city + Splitter + item.Weight);
			}

			return appliedStateType;
		}

        private AppliedStateType OvernightCitiesBulkWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Overnight Cities -Bulk", Value = new List<string>()};

            foreach (Wt2Parameter item in list)
            {
                var cityName = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == item.Type);
                string city = string.Empty;
                if (cityName != null)
                {
                    city = cityName.Name;
                }
                appliedStateType.Value.Add(city + Splitter + item.Weight);
            }

            return appliedStateType;

        }

        private AppliedStateType PositionWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Position", Value = new List<string>()};

            foreach (Wt2Parameter item in list)
            {



                appliedStateType.Value.Add(PositionName(item.Type) + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        private AppliedStateType RestWeight(IEnumerable<Wt4Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Rest", Value = new List<string>()};

            foreach (Wt4Parameter item in list)
            {
                string values = Convert.ToString(((RestOptions)Convert.ToInt32(item.FirstValue))) + Splitter;
                values += Helper.ConvertMinuteToHHMM(item.SecondlValue) + Splitter;
                values += Convert.ToString(((RestType)Convert.ToInt32(item.ThrirdCellValue))) + Splitter;
                values += item.Weight;
                appliedStateType.Value.Add(values);
            }

            return appliedStateType;
        }

        private AppliedStateType StartDateOftheWeekWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Start Day of Week", Value = new List<string>()};

            foreach (Wt2Parameter item in list)
            {
                appliedStateType.Value.Add(Convert.ToString(((Dow)item.Type)) + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        private AppliedStateType TimeAwayFromBaseWeight(Wt2Parameter wt2Parameter)
        {
            var appliedStateType = new AppliedStateType
            {
                Key = "Time-Away-From-Base",
                Value = new List<string> {wt2Parameter.Type + Splitter + wt2Parameter.Weight}
            };


            return appliedStateType;
        }

        private AppliedStateType TripLengthWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Trip Length", Value = new List<string>()};

            foreach (Wt2Parameter item in list)
            {
                appliedStateType.Value.Add(TripType(item.Type) + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        private AppliedStateType WorkBlockLengthWeight(IEnumerable<Wt2Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Work Block Length", Value = new List<string>()};

            foreach (Wt2Parameter item in list)
            {
                appliedStateType.Value.Add(TripType(item.Type) + Splitter + item.Weight);
            }

            return appliedStateType;
        }

        private AppliedStateType WorkDaysWeight(IEnumerable<Wt3Parameter> list)
        {
            var appliedStateType = new AppliedStateType {Key = "Work Days", Value = new List<string>()};


            foreach (Wt3Parameter item in list)
            {

                appliedStateType.Value.Add(Convert.ToString(((WeightType)item.SecondlValue)) + Splitter + item.ThrirdCellValue + " wk days" + Splitter + item.Weight);
            }
            return appliedStateType;
        }

        #endregion

        #region Utility Methods
        private string GetBeginEndType(int type)
        {
            string typeStr = string.Empty;
            switch (type)
            {
                case 1:
                    typeStr = "begin";
                    break;
                case 2:
                    typeStr = "end";
                    break;
                case 3:
                    typeStr = "both";
                    break;

            }
            return typeStr;
        }

        private string PositionName(int pos)
        {
            string positionName = string.Empty;
            switch (pos)
            {
                case 1:
                    positionName = "A";
                    break;
                case 2:
                    positionName = "B";
                    break;
                case 3:
                    positionName = "C";
                    break;
                case 4:
                    positionName = "D";
                    break;
            }
            return positionName;
        }

        private string TripType(int type)
        {
            string tripTypestr = string.Empty;
            switch (type)
            {
                case 1:
                    tripTypestr = "Turn";
                    break;
                case 2:
                    tripTypestr = "2day";
                    break;
                case 3:
                    tripTypestr = "3day";
                    break;
                case 4:
                    tripTypestr = "4day";
                    break;
            }
            return tripTypestr;
        }
        #endregion

        
        #endregion

    }
}
