using System;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using System.Collections.Generic;

namespace WBid.WBidiPad.PortableLibrary
{
	public class CompareState
	{
		public CompareState ()
		{
		}

		public bool CompareStateChange(WBidState statefileContent, WBidState tempContent)
		{


			if (statefileContent.BottomLockCount != tempContent.BottomLockCount)
				return false;

			if (statefileContent.TopLockCount != tempContent.TopLockCount)
				return false;

			if (!CompareCxWtState(statefileContent.CxWtState, tempContent.CxWtState))
				return false;

			if (statefileContent.FAEOMStartDate != tempContent.FAEOMStartDate)
				return false;

			if (statefileContent.ForceLine.IsBlankLinetoBottom != tempContent.ForceLine.IsBlankLinetoBottom || statefileContent.ForceLine.IsReverseLinetoBottom != tempContent.ForceLine.IsReverseLinetoBottom)
				return false;

			if (statefileContent.IsOverlapCorrection != tempContent.IsOverlapCorrection)
				return false;
			if (statefileContent.IsVacationOverlapOverlapCorrection != tempContent.IsVacationOverlapOverlapCorrection)
				return false;

			//if(statefileContent.LineOrders.Lines

			if (statefileContent.MenuBarButtonState.IsEOM != tempContent.MenuBarButtonState.IsEOM || statefileContent.MenuBarButtonState.IsOverlap != tempContent.MenuBarButtonState.IsOverlap || statefileContent.MenuBarButtonState.IsVacationCorrection != tempContent.MenuBarButtonState.IsVacationCorrection || statefileContent.MenuBarButtonState.IsVacationDrop != tempContent.MenuBarButtonState.IsVacationDrop)
				return false;

			if (!CompareSortDetails(statefileContent.SortDetails, tempContent.SortDetails))
				return false;

			if (statefileContent.StateName != tempContent.StateName)
				return false;
			//if(statefileContent.TagDetails


			if (!CompareConstraints(statefileContent.Constraints, tempContent.Constraints))
				return false;


			if (!CompareWeights(statefileContent.Weights, tempContent.Weights))
				return false;

			if (statefileContent.MenuBarButtonState.IsVacationCorrection != GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || statefileContent.MenuBarButtonState.IsVacationDrop != GlobalSettings.MenuBarButtonStatus.IsVacationDrop || statefileContent.MenuBarButtonState.IsOverlap != GlobalSettings.MenuBarButtonStatus.IsOverlap || statefileContent.MenuBarButtonState.IsEOM != GlobalSettings.MenuBarButtonStatus.IsEOM)
				return false;


			// statefileContent.Weights

			return true;

		}

		/// <summary>
		/// Comapre CxWt state 
		/// </summary>
		/// <param name="stateFileContentCxWtState"></param>
		/// <param name="tempContentCxWtState"></param>
		/// <returns></returns>
		private bool CompareCxWtState(CxWtState stateFileContentCxWtState, CxWtState tempContentCxWtState)
		{


			if (stateFileContentCxWtState.ACChg.Cx != tempContentCxWtState.ACChg.Cx || stateFileContentCxWtState.ACChg.Wt != tempContentCxWtState.ACChg.Wt)
				return false;
			if (stateFileContentCxWtState.AMPM.Cx != tempContentCxWtState.AMPM.Cx || stateFileContentCxWtState.AMPM.Wt != tempContentCxWtState.AMPM.Wt)
				return false;
			if (stateFileContentCxWtState.AMPMMIX.AM != tempContentCxWtState.AMPMMIX.AM || stateFileContentCxWtState.AMPMMIX.PM != tempContentCxWtState.AMPMMIX.PM || stateFileContentCxWtState.AMPMMIX.MIX != tempContentCxWtState.AMPMMIX.MIX)
				return false;

			if (stateFileContentCxWtState.BDO.Cx != tempContentCxWtState.BDO.Cx || stateFileContentCxWtState.BDO.Wt != tempContentCxWtState.BDO.Wt)
				return false;
			if (stateFileContentCxWtState.CL.Cx != tempContentCxWtState.CL.Cx || stateFileContentCxWtState.CL.Wt != tempContentCxWtState.CL.Wt)
				return false;
			if (stateFileContentCxWtState.DaysOfWeek.SUN != tempContentCxWtState.DaysOfWeek.SUN
				|| stateFileContentCxWtState.DaysOfWeek.MON != tempContentCxWtState.DaysOfWeek.MON
				|| stateFileContentCxWtState.DaysOfWeek.TUE != tempContentCxWtState.DaysOfWeek.TUE
				|| stateFileContentCxWtState.DaysOfWeek.WED != tempContentCxWtState.DaysOfWeek.WED
				|| stateFileContentCxWtState.DaysOfWeek.THU != tempContentCxWtState.DaysOfWeek.THU
				|| stateFileContentCxWtState.DaysOfWeek.FRI != tempContentCxWtState.DaysOfWeek.FRI
				|| stateFileContentCxWtState.DaysOfWeek.SAT != tempContentCxWtState.DaysOfWeek.SAT)
				return false;

			if (stateFileContentCxWtState.DHD.Cx != tempContentCxWtState.DHD.Cx || stateFileContentCxWtState.DHD.Wt != tempContentCxWtState.DHD.Wt)
				return false;
			if (stateFileContentCxWtState.DHDFoL.Cx != tempContentCxWtState.DHDFoL.Cx || stateFileContentCxWtState.DHDFoL.Wt != tempContentCxWtState.DHDFoL.Wt)
				return false;
			if (stateFileContentCxWtState.DOW.Cx != tempContentCxWtState.DOW.Cx || stateFileContentCxWtState.DOW.Wt != tempContentCxWtState.DOW.Wt)
				return false;

			if (stateFileContentCxWtState.DP.Cx != tempContentCxWtState.DP.Cx || stateFileContentCxWtState.DP.Wt != tempContentCxWtState.DP.Wt)
				return false;

			if (stateFileContentCxWtState.EQUIP.Cx != tempContentCxWtState.EQUIP.Cx || stateFileContentCxWtState.EQUIP.Wt != tempContentCxWtState.EQUIP.Wt)
				return false;
			if (stateFileContentCxWtState.FaPosition.A != tempContentCxWtState.FaPosition.A || stateFileContentCxWtState.FaPosition.B != tempContentCxWtState.FaPosition.B || stateFileContentCxWtState.FaPosition.C != tempContentCxWtState.FaPosition.C || stateFileContentCxWtState.FaPosition.D != tempContentCxWtState.FaPosition.D)
				return false;

			if (stateFileContentCxWtState.FLTMIN.Cx != tempContentCxWtState.FLTMIN.Cx || stateFileContentCxWtState.FLTMIN.Wt != tempContentCxWtState.FLTMIN.Wt)
				return false;

			if (stateFileContentCxWtState.GRD.Cx != tempContentCxWtState.GRD.Cx || stateFileContentCxWtState.GRD.Wt != tempContentCxWtState.GRD.Wt)
				return false;

			if (stateFileContentCxWtState.InterConus.Cx != tempContentCxWtState.InterConus.Cx || stateFileContentCxWtState.InterConus.Wt != tempContentCxWtState.InterConus.Wt)
				return false;

			if (stateFileContentCxWtState.LEGS.Cx != tempContentCxWtState.LEGS.Cx || stateFileContentCxWtState.LEGS.Wt != tempContentCxWtState.LEGS.Wt)
				return false;

			if (stateFileContentCxWtState.LegsPerPairing.Cx != tempContentCxWtState.LegsPerPairing.Cx || stateFileContentCxWtState.LegsPerPairing.Wt != tempContentCxWtState.LegsPerPairing.Wt)
				return false;

			if (stateFileContentCxWtState.LrgBlkDaysOff.Cx != tempContentCxWtState.LrgBlkDaysOff.Cx || stateFileContentCxWtState.LrgBlkDaysOff.Wt != tempContentCxWtState.LrgBlkDaysOff.Wt)
				return false;

			if (stateFileContentCxWtState.MP.Cx != tempContentCxWtState.MP.Cx || stateFileContentCxWtState.MP.Wt != tempContentCxWtState.MP.Wt)
				return false;


			if (stateFileContentCxWtState.No3on3off.Cx != tempContentCxWtState.No3on3off.Cx || stateFileContentCxWtState.No3on3off.Wt != tempContentCxWtState.No3on3off.Wt)
				return false;


			if (stateFileContentCxWtState.NODO.Cx != tempContentCxWtState.NODO.Cx || stateFileContentCxWtState.NODO.Wt != tempContentCxWtState.NODO.Wt)
				return false;


			if (stateFileContentCxWtState.NOL.Cx != tempContentCxWtState.NOL.Cx || stateFileContentCxWtState.NOL.Wt != tempContentCxWtState.NOL.Wt)
				return false;

			if (stateFileContentCxWtState.PDAfter.Cx != tempContentCxWtState.PDAfter.Cx || stateFileContentCxWtState.PDAfter.Wt != tempContentCxWtState.PDAfter.Wt)
				return false;

			if (stateFileContentCxWtState.PDBefore.Cx != tempContentCxWtState.PDBefore.Cx || stateFileContentCxWtState.PDBefore.Wt != tempContentCxWtState.PDBefore.Wt)
				return false;


			if (stateFileContentCxWtState.PerDiem.Cx != tempContentCxWtState.PerDiem.Cx || stateFileContentCxWtState.PerDiem.Wt != tempContentCxWtState.PerDiem.Wt)
				return false;

			if (stateFileContentCxWtState.Position.Cx != tempContentCxWtState.Position.Cx || stateFileContentCxWtState.Position.Wt != tempContentCxWtState.Position.Wt)
				return false;

			if (stateFileContentCxWtState.Rest.Cx != tempContentCxWtState.Rest.Cx || stateFileContentCxWtState.Rest.Wt != tempContentCxWtState.Rest.Wt)
				return false;

			if (stateFileContentCxWtState.RON.Cx != tempContentCxWtState.RON.Cx || stateFileContentCxWtState.RON.Wt != tempContentCxWtState.RON.Wt)
				return false;


			if (stateFileContentCxWtState.BulkOC.Cx != tempContentCxWtState.BulkOC.Cx || stateFileContentCxWtState.BulkOC.Wt != tempContentCxWtState.BulkOC.Wt)
				return false;

			if (stateFileContentCxWtState.SDO.Cx != tempContentCxWtState.SDO.Cx || stateFileContentCxWtState.SDO.Wt != tempContentCxWtState.SDO.Wt)
				return false;

			if (stateFileContentCxWtState.SDOW.Cx != tempContentCxWtState.SDOW.Cx || stateFileContentCxWtState.SDOW.Wt != tempContentCxWtState.SDOW.Wt)
				return false;

			if (stateFileContentCxWtState.TL.Cx != tempContentCxWtState.TL.Cx || stateFileContentCxWtState.TL.Wt != tempContentCxWtState.TL.Wt)
				return false;
			if (stateFileContentCxWtState.TripLength.Turns != tempContentCxWtState.TripLength.Turns || stateFileContentCxWtState.TripLength.Twoday != tempContentCxWtState.TripLength.Twoday
				|| stateFileContentCxWtState.TripLength.ThreeDay != tempContentCxWtState.TripLength.ThreeDay || stateFileContentCxWtState.TripLength.FourDay != tempContentCxWtState.TripLength.FourDay)
				return false;
			if (stateFileContentCxWtState.WB.Cx != tempContentCxWtState.WB.Cx || stateFileContentCxWtState.WB.Wt != tempContentCxWtState.WB.Wt)
				return false;
			if (stateFileContentCxWtState.WorkDay.Cx != tempContentCxWtState.WorkDay.Cx || stateFileContentCxWtState.WorkDay.Wt != tempContentCxWtState.WorkDay.Wt)
				return false;

			if (stateFileContentCxWtState.WtPDOFS.Cx != tempContentCxWtState.WtPDOFS.Cx || stateFileContentCxWtState.WtPDOFS.Wt != tempContentCxWtState.WtPDOFS.Wt)
				return false;


			return true;

		}


		private bool CompareSortDetails(SortDetails stateFileContentSortDetails, SortDetails tempContentSortDetails)
		{

			if (stateFileContentSortDetails != null)
			{

				if ((stateFileContentSortDetails == null && tempContentSortDetails != null) || (stateFileContentSortDetails != null && tempContentSortDetails == null))
					return false;


				if (stateFileContentSortDetails.SortColumn != tempContentSortDetails.SortColumn || stateFileContentSortDetails.SortColumnName != tempContentSortDetails.SortColumnName || stateFileContentSortDetails.SortDirection != tempContentSortDetails.SortDirection)
					return false;


				if ((stateFileContentSortDetails.BlokSort == null && tempContentSortDetails.BlokSort != null) || (stateFileContentSortDetails.BlokSort != null && tempContentSortDetails.BlokSort == null))
					return false;

				if (stateFileContentSortDetails.BlokSort != null && tempContentSortDetails.BlokSort != null)
				{
					if (stateFileContentSortDetails.BlokSort.Count != tempContentSortDetails.BlokSort.Count)
						return false;

					for (int cnt = 0; cnt < stateFileContentSortDetails.BlokSort.Count; cnt++)
					{
						if (stateFileContentSortDetails.BlokSort[cnt] != tempContentSortDetails.BlokSort[cnt])
						{
							return false;
						}
					}
				}

			}






			return true;
		}


		#region  Constraints
		private bool CompareConstraints(Constraints stateFileContentConstraints, Constraints tempContentConstraints)
		{
			//AircraftChanges
			if (!ComapareCx2Parameter(stateFileContentConstraints.AircraftChanges, tempContentConstraints.AircraftChanges))
				return false;

			if (stateFileContentConstraints.Blank != tempContentConstraints.Blank)
				return false;

			//BlockOfDaysOff
			if (!ComapareCx2Parameter(stateFileContentConstraints.BlockOfDaysOff, tempContentConstraints.BlockOfDaysOff))
				return false;

			//CL
			if (!ComapareCxCommutableLine(stateFileContentConstraints.CL, tempContentConstraints.CL))
				return false;

			//        DaysOfMonth
			if (!ComapareDaysOfMonthCx(stateFileContentConstraints.DaysOfMonth, tempContentConstraints.DaysOfMonth))
				return false;



			//DaysOfWeek
			if (!ComapareCx3Parameters(stateFileContentConstraints.DaysOfWeek, tempContentConstraints.DaysOfWeek))
				return false;

			//DeadHeadFoL
			if (!ComapareCx3Parameters(stateFileContentConstraints.DeadHeadFoL, tempContentConstraints.DeadHeadFoL))
				return false;
			//DeadHeads
			if (!ComapareCx4Parameters(stateFileContentConstraints.DeadHeads, tempContentConstraints.DeadHeads))
				return false;
			//DutyPeriod
			if (!ComapareCx2Parameter(stateFileContentConstraints.DutyPeriod, tempContentConstraints.DutyPeriod))
				return false;
			//EQUIP
			if (!ComapareCx3Parameters(stateFileContentConstraints.EQUIP, tempContentConstraints.EQUIP))
				return false;

			//FlightMin
			if (!ComapareCx2Parameter(stateFileContentConstraints.FlightMin, tempContentConstraints.FlightMin))
				return false;

			if (!ComapareCx3Parameter(stateFileContentConstraints.GroundTime, tempContentConstraints.GroundTime))
				return false;

			if (stateFileContentConstraints.Hard != tempContentConstraints.Hard)
				return false;

			//InterConus
			if (!ComapareCx2Parameters(stateFileContentConstraints.InterConus, tempContentConstraints.InterConus))
				return false;

			if (stateFileContentConstraints.International != tempContentConstraints.International)
				return false;

			if (!ComapareCx2Parameter(stateFileContentConstraints.LegsPerDutyPeriod, tempContentConstraints.LegsPerDutyPeriod))
				return false;

			if (!ComapareCx2Parameter(stateFileContentConstraints.LegsPerPairing, tempContentConstraints.LegsPerPairing))
				return false;


			if (!ComapareCx2Parameter(stateFileContentConstraints.LrgBlkDayOff, tempContentConstraints.LrgBlkDayOff))
				return false;


			if (!ComapareCx2Parameter(stateFileContentConstraints.MinimumPay, tempContentConstraints.MinimumPay))
				return false;

			if (!ComapareCx2Parameter(stateFileContentConstraints.No3On3Off, tempContentConstraints.No3On3Off))
				return false;

			if (stateFileContentConstraints.NonConus != tempContentConstraints.NonConus)
				return false;
            if (stateFileContentConstraints.ETOPS != tempContentConstraints.ETOPS)
                return false;

			if (!ComapareCx2Parameter(stateFileContentConstraints.NoOverLap, tempContentConstraints.NoOverLap))
				return false;

			if (!ComapareCx2Parameter(stateFileContentConstraints.NumberOfDaysOff, tempContentConstraints.NumberOfDaysOff))
				return false;

			if (!ComapareCx3Parameters(stateFileContentConstraints.OverNightCities, tempContentConstraints.OverNightCities))
				return false;


			//        OvernightcitiesBulk
			if (!ComapareBulkOvernightCityCx(stateFileContentConstraints.BulkOvernightCity, tempContentConstraints.BulkOvernightCity))
				return false;

			if (!ComapareCx4Parameters(stateFileContentConstraints.PDOFS, tempContentConstraints.PDOFS))
				return false;

			if (!ComapareCx2Parameter(stateFileContentConstraints.PerDiem, tempContentConstraints.PerDiem))
				return false;


			if (!ComapareCx3Parameters(stateFileContentConstraints.Position, tempContentConstraints.Position))
				return false;

			if (stateFileContentConstraints.Ready != tempContentConstraints.Ready)
				return false;

			if (stateFileContentConstraints.Reserve != tempContentConstraints.Reserve)
				return false;

			if (!ComapareCx3Parameters(stateFileContentConstraints.Rest, tempContentConstraints.Rest))
				return false;

			if (!ComapareCx3Parameters(stateFileContentConstraints.StartDayOftheWeek, tempContentConstraints.StartDayOftheWeek))
				return false;


			if (!ComapareCx3Parameters(stateFileContentConstraints.TripLength, tempContentConstraints.TripLength))
				return false;

			if (!ComapareCx3Parameters(stateFileContentConstraints.WorkBlockLength, tempContentConstraints.WorkBlockLength))
				return false;

			if (!ComapareCx2Parameter(stateFileContentConstraints.WorkDay, tempContentConstraints.WorkDay))
				return false;



			return true;
		}


		private bool ComapareCx2Parameter(Cx2Parameter stateFileContentCx2Parameter, Cx2Parameter tempContentCx2Parameter)
		{
			if ((stateFileContentCx2Parameter == null && tempContentCx2Parameter != null) || (stateFileContentCx2Parameter != null && tempContentCx2Parameter == null))
				return false;

			if (stateFileContentCx2Parameter != null && tempContentCx2Parameter != null)
			{
				if (stateFileContentCx2Parameter.Type != tempContentCx2Parameter.Type || stateFileContentCx2Parameter.Value != tempContentCx2Parameter.Value)
					return false;
			}
			return true;
		}

		private bool ComapareCx3Parameter(Cx3Parameter stateFileContentCx3Parameter, Cx3Parameter tempContentCx3Parameter)
		{
			if ((stateFileContentCx3Parameter == null && tempContentCx3Parameter != null) || (stateFileContentCx3Parameter != null && tempContentCx3Parameter == null))
				return false;

			if (stateFileContentCx3Parameter != null && tempContentCx3Parameter != null)
			{
				if (stateFileContentCx3Parameter.Type != tempContentCx3Parameter.Type || stateFileContentCx3Parameter.Value != tempContentCx3Parameter.Value || stateFileContentCx3Parameter.ThirdcellValue != tempContentCx3Parameter.ThirdcellValue)
					return false;
			}

			return true;
		}

		private bool ComapareCx4Parameter(Cx4Parameter stateFileContentCx4Parameter, Cx4Parameter tempContentCx4Parameter)
		{
			if ((stateFileContentCx4Parameter == null && tempContentCx4Parameter != null) || (stateFileContentCx4Parameter != null && tempContentCx4Parameter == null))
				return false;


			if (stateFileContentCx4Parameter != null && tempContentCx4Parameter != null)
			{
				if (stateFileContentCx4Parameter.Type != tempContentCx4Parameter.Type || stateFileContentCx4Parameter.Value != tempContentCx4Parameter.Value || stateFileContentCx4Parameter.ThirdcellValue != tempContentCx4Parameter.ThirdcellValue || stateFileContentCx4Parameter.SecondcellValue != tempContentCx4Parameter.SecondcellValue)
					return false;
			}
			return true;
		}

		private bool ComapareCx2Parameters(Cx2Parameters stateFileContentCx2Parameter, Cx2Parameters tempContentCx2Parameter)
		{

			if (stateFileContentCx2Parameter.Type != tempContentCx2Parameter.Type || stateFileContentCx2Parameter.Value != tempContentCx2Parameter.Value)
				return false;

			if ((stateFileContentCx2Parameter.lstParameters == null && tempContentCx2Parameter.lstParameters != null) || (stateFileContentCx2Parameter.lstParameters != null && tempContentCx2Parameter.lstParameters == null))
				return false;


			if (stateFileContentCx2Parameter.lstParameters != null && tempContentCx2Parameter.lstParameters != null)
			{
				if (stateFileContentCx2Parameter.lstParameters.Count != tempContentCx2Parameter.lstParameters.Count)
					return false;

				for (int cnt = 0; cnt < stateFileContentCx2Parameter.lstParameters.Count; cnt++)
				{
					if (!ComapareCx2Parameter(stateFileContentCx2Parameter.lstParameters[cnt], tempContentCx2Parameter.lstParameters[cnt]))
					{
						return false;
					}
				}
			}




			return true;
		}

		private bool ComapareCx3Parameters(Cx3Parameters stateFileContentCx3Parameter, Cx3Parameters tempContentCx3Parameter)
		{

			if (stateFileContentCx3Parameter.Type != tempContentCx3Parameter.Type || stateFileContentCx3Parameter.Value != tempContentCx3Parameter.Value || stateFileContentCx3Parameter.ThirdcellValue != tempContentCx3Parameter.ThirdcellValue)
				return false;

			if ((stateFileContentCx3Parameter.lstParameters == null && tempContentCx3Parameter.lstParameters != null) || (stateFileContentCx3Parameter.lstParameters != null && tempContentCx3Parameter.lstParameters == null))
				return false;


			if (stateFileContentCx3Parameter.lstParameters != null && tempContentCx3Parameter.lstParameters != null)
			{
				if (stateFileContentCx3Parameter.lstParameters.Count != tempContentCx3Parameter.lstParameters.Count)
					return false;

				for (int cnt = 0; cnt < stateFileContentCx3Parameter.lstParameters.Count; cnt++)
				{
					if (!ComapareCx3Parameter(stateFileContentCx3Parameter.lstParameters[cnt], tempContentCx3Parameter.lstParameters[cnt]))
					{
						return false;
					}
				}
			}




			return true;
		}

		private bool ComapareCx4Parameters(Cx4Parameters stateFileContentCx4Parameter, Cx4Parameters tempContentCx4Parameter)
		{

			if (stateFileContentCx4Parameter.Type != tempContentCx4Parameter.Type || stateFileContentCx4Parameter.Value != tempContentCx4Parameter.Value || stateFileContentCx4Parameter.ThirdcellValue != tempContentCx4Parameter.ThirdcellValue || stateFileContentCx4Parameter.SecondcellValue != tempContentCx4Parameter.SecondcellValue)
				return false;

			if ((stateFileContentCx4Parameter.LstParameter == null && tempContentCx4Parameter.LstParameter != null) || (stateFileContentCx4Parameter.LstParameter != null && tempContentCx4Parameter.LstParameter == null))
				return false;


			if (stateFileContentCx4Parameter.LstParameter != null && tempContentCx4Parameter.LstParameter != null)
			{
				if (stateFileContentCx4Parameter.LstParameter.Count != tempContentCx4Parameter.LstParameter.Count)
					return false;

				for (int cnt = 0; cnt < stateFileContentCx4Parameter.LstParameter.Count; cnt++)
				{
					if (!ComapareCx4Parameter(stateFileContentCx4Parameter.LstParameter[cnt], tempContentCx4Parameter.LstParameter[cnt]))
					{
						return false;
					}
				}
			}




			return true;
		}

		private bool ComapareCxCommutableLine(CxCommutableLine stateFileContentCxCommutableLine, CxCommutableLine tempContentCxCommutableLine)
		{

			if (stateFileContentCxCommutableLine.AnyNight != tempContentCxCommutableLine.AnyNight || stateFileContentCxCommutableLine.CommuteToHome != tempContentCxCommutableLine.CommuteToHome || stateFileContentCxCommutableLine.CommuteToWork != tempContentCxCommutableLine.CommuteToWork)
				return false;


			if (stateFileContentCxCommutableLine.Sunday.Checkin != tempContentCxCommutableLine.Sunday.Checkin || stateFileContentCxCommutableLine.Sunday.BackToBase != tempContentCxCommutableLine.Sunday.BackToBase)
				return false;
			
			if (stateFileContentCxCommutableLine.MondayThu.Checkin != tempContentCxCommutableLine.MondayThu.Checkin || stateFileContentCxCommutableLine.MondayThu.BackToBase != tempContentCxCommutableLine.MondayThu.BackToBase)
				return false;



			if (stateFileContentCxCommutableLine.Friday.Checkin != tempContentCxCommutableLine.Friday.Checkin || stateFileContentCxCommutableLine.Friday.BackToBase != tempContentCxCommutableLine.Friday.BackToBase)
				return false;

			if (stateFileContentCxCommutableLine.Saturday.Checkin != tempContentCxCommutableLine.Saturday.Checkin || stateFileContentCxCommutableLine.Saturday.BackToBase != tempContentCxCommutableLine.Saturday.BackToBase)
				return false;


			if ((stateFileContentCxCommutableLine.TimesList == null && tempContentCxCommutableLine.TimesList != null) || (stateFileContentCxCommutableLine.TimesList != null && tempContentCxCommutableLine.TimesList == null))
				return false;



			if (stateFileContentCxCommutableLine.TimesList != null && tempContentCxCommutableLine.TimesList!=null)
			{
				if (stateFileContentCxCommutableLine.TimesList.Count != tempContentCxCommutableLine.TimesList.Count)
					return false;

				for (int count = 0; count < stateFileContentCxCommutableLine.TimesList.Count; count++)
				{

					if ((stateFileContentCxCommutableLine.TimesList[count].BackToBase != tempContentCxCommutableLine.TimesList[count].BackToBase) || (stateFileContentCxCommutableLine.TimesList[count].Checkin != tempContentCxCommutableLine.TimesList[count].Checkin))
					{
						return false;
					}

				}
			}


			return true;
		}

		private bool ComapareDaysOfMonthCx(DaysOfMonthCx stateFileContentDaysOfMonthCx, DaysOfMonthCx tempContentDaysOfMonthCx)
		{
			if ((stateFileContentDaysOfMonthCx.OFFDays != null && tempContentDaysOfMonthCx.OFFDays == null) || (stateFileContentDaysOfMonthCx.OFFDays == null && tempContentDaysOfMonthCx.OFFDays != null))
				return false;
			if (stateFileContentDaysOfMonthCx.OFFDays != null && tempContentDaysOfMonthCx.OFFDays != null)
			{
				if (stateFileContentDaysOfMonthCx.OFFDays.Count != tempContentDaysOfMonthCx.OFFDays.Count)
					return false;

				for (int count = 0; count < stateFileContentDaysOfMonthCx.OFFDays.Count; count++)
				{
					if (stateFileContentDaysOfMonthCx.OFFDays[count] != tempContentDaysOfMonthCx.OFFDays[count])
						return false;
				}


			}


			if ((stateFileContentDaysOfMonthCx.WorkDays != null && tempContentDaysOfMonthCx.WorkDays == null) || (stateFileContentDaysOfMonthCx.WorkDays == null && tempContentDaysOfMonthCx.WorkDays != null))
				return false;
			if (stateFileContentDaysOfMonthCx.WorkDays != null && tempContentDaysOfMonthCx.WorkDays != null)
			{
				if (stateFileContentDaysOfMonthCx.WorkDays.Count != tempContentDaysOfMonthCx.WorkDays.Count)
					return false;

				for (int count = 0; count < stateFileContentDaysOfMonthCx.WorkDays.Count; count++)
				{
					if (stateFileContentDaysOfMonthCx.WorkDays[count] != tempContentDaysOfMonthCx.WorkDays[count])
						return false;
				}


			}


			return true;
		}



		private bool ComapareBulkOvernightCityCx(BulkOvernightCityCx stateFileContentBulkOvernightCityCx, BulkOvernightCityCx tempContentBulkOvernightCityCx)
		{
			if ((stateFileContentBulkOvernightCityCx.OverNightYes != null && tempContentBulkOvernightCityCx.OverNightYes == null) || (stateFileContentBulkOvernightCityCx.OverNightYes == null && tempContentBulkOvernightCityCx.OverNightYes != null))
				return false;
			if (stateFileContentBulkOvernightCityCx.OverNightYes != null && tempContentBulkOvernightCityCx.OverNightYes != null)
			{
				if (stateFileContentBulkOvernightCityCx.OverNightYes.Count != tempContentBulkOvernightCityCx.OverNightYes.Count)
					return false;

				for (int count = 0; count < stateFileContentBulkOvernightCityCx.OverNightYes.Count; count++)
				{
					if (stateFileContentBulkOvernightCityCx.OverNightYes[count] != tempContentBulkOvernightCityCx.OverNightYes[count])
						return false;
				}


			}


			if ((stateFileContentBulkOvernightCityCx.OverNightNo != null && tempContentBulkOvernightCityCx.OverNightNo == null) || (stateFileContentBulkOvernightCityCx.OverNightNo == null && tempContentBulkOvernightCityCx.OverNightNo != null))
				return false;
			if (stateFileContentBulkOvernightCityCx.OverNightNo != null && tempContentBulkOvernightCityCx.OverNightNo != null)
			{
				if (stateFileContentBulkOvernightCityCx.OverNightNo.Count != tempContentBulkOvernightCityCx.OverNightNo.Count)
					return false;

				for (int count = 0; count < stateFileContentBulkOvernightCityCx.OverNightNo.Count; count++)
				{
					if (stateFileContentBulkOvernightCityCx.OverNightNo[count] != tempContentBulkOvernightCityCx.OverNightNo[count])
						return false;
				}


			}


			return true;
		}

		#endregion


		#region Weights
		private bool CompareWeights(Weights stateFileContentWeights, Weights tempContentWeights)
		{
			if (!ComapareWt3Parameter(stateFileContentWeights.AirCraftChanges, tempContentWeights.AirCraftChanges))
				return false;

			if (!ComapareWt2Parameters(stateFileContentWeights.AM_PM, tempContentWeights.AM_PM))
				return false;

			if (!ComapareWt3Parameters(stateFileContentWeights.BDO, tempContentWeights.BDO))
				return false;
			if (!ComaparCommutableLineWt(stateFileContentWeights.CL, tempContentWeights.CL))
				return false;
			if (!ComapareWt3Parameters(stateFileContentWeights.DHD, tempContentWeights.DHD))
				return false;

			if (!ComapareWt2Parameters(stateFileContentWeights.DHDFoL, tempContentWeights.DHDFoL))
				return false;

			if (!ComaparDOWWeigh(stateFileContentWeights.DOW, tempContentWeights.DOW))
				return false;

			if (!ComapareWt3Parameters(stateFileContentWeights.DP, tempContentWeights.DP))
				return false;

			if (!ComapareWt3Parameters(stateFileContentWeights.EQUIP, tempContentWeights.EQUIP))
				return false;

			if (!ComapareWt3Parameters(stateFileContentWeights.FLTMIN, tempContentWeights.FLTMIN))
				return false;


			if (!ComapareWt3Parameters(stateFileContentWeights.GRD, tempContentWeights.GRD))
				return false;

			if (!ComapareWt2Parameters(stateFileContentWeights.InterConus, tempContentWeights.InterConus))
				return false;


			if (!ComapareWt3Parameters(stateFileContentWeights.LEGS, tempContentWeights.LEGS))
				return false;


			if (!ComapareWt2Parameter(stateFileContentWeights.LrgBlkDayOff, tempContentWeights.LrgBlkDayOff))
				return false;


			if (!ComapareWt2Parameters(stateFileContentWeights.NODO, tempContentWeights.NODO))
				return false;

			if (!ComapareWt4Parameters(stateFileContentWeights.PDAfter, tempContentWeights.PDAfter))
				return false;


			if (!ComapareWt4Parameters(stateFileContentWeights.PDBefore, tempContentWeights.PDBefore))
				return false;

			if (!ComapareWt2Parameter(stateFileContentWeights.PerDiem, tempContentWeights.PerDiem))
				return false;


			if (!ComapareWt2Parameters(stateFileContentWeights.POS, tempContentWeights.POS))
				return false;

			if (!ComapareWt2Parameters(stateFileContentWeights.POS, tempContentWeights.POS))
				return false;

			if (!ComapareWt2Parameters(stateFileContentWeights.RON, tempContentWeights.RON))
				return false;

			if (!ComapareBulkOverNightCities(stateFileContentWeights.OvernightCitybulk, tempContentWeights.OvernightCitybulk))
				return false;



			if (!ComaparDaysOfMonthWt(stateFileContentWeights.SDO, tempContentWeights.SDO))
				return false;

			if (!ComapareWt2Parameters(stateFileContentWeights.SDOW, tempContentWeights.SDOW))
				return false;

			if (!ComapareWt2Parameters(stateFileContentWeights.TL, tempContentWeights.TL))
				return false;


			if (!ComapareWt2Parameters(stateFileContentWeights.WB, tempContentWeights.WB))
				return false;



			if (!ComapareWt3Parameters(stateFileContentWeights.WorkDays, tempContentWeights.WorkDays))
				return false;


			if (!ComapareWt3Parameters(stateFileContentWeights.WtLegsPerPairing, tempContentWeights.WtLegsPerPairing))
				return false;

			if (!ComaparDaysOfMonthWt(stateFileContentWeights.WtPDOFS, tempContentWeights.WtPDOFS))
				return false;


			if (!ComapareWt4Parameters(stateFileContentWeights.WtRest, tempContentWeights.WtRest))
				return false;







			return true;
		}

		private bool ComapareBulkOverNightCities(List<Wt2Parameter> stateFileContenList, List<Wt2Parameter> tempContentList)
		{
			if ((stateFileContenList == null && tempContentList != null) || (stateFileContenList != null && tempContentList == null))
				return false;


			if (stateFileContenList != null && tempContentList != null)
			{
				if (stateFileContenList.Count != tempContentList.Count)
					return false;

				for (int cnt = 0; cnt < stateFileContenList.Count; cnt++)
				{
					if (!ComapareWt2Parameter(stateFileContenList[cnt], tempContentList[cnt]))
					{
						return false;
					}
				}
			}


			return true;
		}


		private bool ComapareWt2Parameter(Wt2Parameter stateFileContentWt2Parameter, Wt2Parameter tempContentWt2Parameter)
		{
			if ((stateFileContentWt2Parameter == null && tempContentWt2Parameter != null) || (stateFileContentWt2Parameter != null && tempContentWt2Parameter == null))
				return false;

			if (stateFileContentWt2Parameter != null && tempContentWt2Parameter != null)
			{
				if (stateFileContentWt2Parameter.Type != tempContentWt2Parameter.Type || stateFileContentWt2Parameter.Weight != tempContentWt2Parameter.Weight)
					return false;
			}
			return true;
		}


		private bool ComapareWt3Parameter(Wt3Parameter stateFileContentWt3Parameter, Wt3Parameter tempContentWt3Parameter)
		{
			if ((stateFileContentWt3Parameter == null && tempContentWt3Parameter != null) || (stateFileContentWt3Parameter != null && tempContentWt3Parameter == null))
				return false;

			if (stateFileContentWt3Parameter != null && tempContentWt3Parameter != null)
			{
				if (stateFileContentWt3Parameter.SecondlValue != tempContentWt3Parameter.SecondlValue || stateFileContentWt3Parameter.ThrirdCellValue != tempContentWt3Parameter.ThrirdCellValue || stateFileContentWt3Parameter.Weight != tempContentWt3Parameter.Weight)
					return false;
			}

			return true;
		}

		private bool ComapareWt4Parameter(Wt4Parameter stateFileContentWt4Parameter, Wt4Parameter tempContentWt4Parameter)
		{
			if ((stateFileContentWt4Parameter == null && tempContentWt4Parameter != null) || (stateFileContentWt4Parameter != null && tempContentWt4Parameter == null))
				return false;


			if (stateFileContentWt4Parameter != null && tempContentWt4Parameter != null)
			{
				if (stateFileContentWt4Parameter.FirstValue != tempContentWt4Parameter.FirstValue || stateFileContentWt4Parameter.SecondlValue != tempContentWt4Parameter.SecondlValue || stateFileContentWt4Parameter.ThrirdCellValue != tempContentWt4Parameter.ThrirdCellValue || stateFileContentWt4Parameter.Weight != tempContentWt4Parameter.Weight)
					return false;
			}
			return true;
		}


		private bool ComapareWt2Parameters(Wt2Parameters stateFileContentWt2Parameters, Wt2Parameters tempContentWt2Parameters)
		{

			if (stateFileContentWt2Parameters.Type != tempContentWt2Parameters.Type || stateFileContentWt2Parameters.Weight != tempContentWt2Parameters.Weight)
				return false;

			if ((stateFileContentWt2Parameters.lstParameters == null && tempContentWt2Parameters.lstParameters != null) || (stateFileContentWt2Parameters.lstParameters != null && tempContentWt2Parameters.lstParameters == null))
				return false;


			if (stateFileContentWt2Parameters.lstParameters != null && tempContentWt2Parameters.lstParameters != null)
			{
				if (stateFileContentWt2Parameters.lstParameters.Count != tempContentWt2Parameters.lstParameters.Count)
					return false;

				for (int cnt = 0; cnt < stateFileContentWt2Parameters.lstParameters.Count; cnt++)
				{
					if (!ComapareWt2Parameter(stateFileContentWt2Parameters.lstParameters[cnt], tempContentWt2Parameters.lstParameters[cnt]))
					{
						return false;
					}
				}
			}


			return true;
		}


		private bool ComapareWt3Parameters(Wt3Parameters stateFileContentWt3Parameters, Wt3Parameters tempContentWt3Parameters)
		{

			if (stateFileContentWt3Parameters.SecondlValue != tempContentWt3Parameters.SecondlValue || stateFileContentWt3Parameters.ThrirdCellValue != tempContentWt3Parameters.ThrirdCellValue || stateFileContentWt3Parameters.Weight != tempContentWt3Parameters.Weight)
				return false;

			if ((stateFileContentWt3Parameters.lstParameters == null && tempContentWt3Parameters.lstParameters != null) || (stateFileContentWt3Parameters.lstParameters != null && tempContentWt3Parameters.lstParameters == null))
				return false;


			if (stateFileContentWt3Parameters.lstParameters != null && tempContentWt3Parameters.lstParameters != null)
			{
				if (stateFileContentWt3Parameters.lstParameters.Count != tempContentWt3Parameters.lstParameters.Count)
					return false;

				for (int cnt = 0; cnt < stateFileContentWt3Parameters.lstParameters.Count; cnt++)
				{
					if (!ComapareWt3Parameter(stateFileContentWt3Parameters.lstParameters[cnt], tempContentWt3Parameters.lstParameters[cnt]))
					{
						return false;
					}
				}
			}




			return true;
		}


		private bool ComapareWt4Parameters(Wt4Parameters stateFileContentWt4Parameters, Wt4Parameters tempContentWt4Parameters)
		{

			if (stateFileContentWt4Parameters.FirstValue != tempContentWt4Parameters.FirstValue || stateFileContentWt4Parameters.SecondlValue != tempContentWt4Parameters.SecondlValue || stateFileContentWt4Parameters.ThrirdCellValue != tempContentWt4Parameters.ThrirdCellValue || stateFileContentWt4Parameters.Weight != tempContentWt4Parameters.Weight)
				return false;

			if ((stateFileContentWt4Parameters.lstParameters == null && tempContentWt4Parameters.lstParameters != null) || (stateFileContentWt4Parameters.lstParameters != null && tempContentWt4Parameters.lstParameters == null))
				return false;


			if (stateFileContentWt4Parameters.lstParameters != null && tempContentWt4Parameters.lstParameters != null)
			{
				if (stateFileContentWt4Parameters.lstParameters.Count != tempContentWt4Parameters.lstParameters.Count)
					return false;

				for (int cnt = 0; cnt < stateFileContentWt4Parameters.lstParameters.Count; cnt++)
				{
					if (!ComapareWt4Parameter(stateFileContentWt4Parameters.lstParameters[cnt], tempContentWt4Parameters.lstParameters[cnt]))
					{
						return false;
					}
				}
			}




			return true;
		}



		private bool ComaparDOWWeigh(WtDaysOfWeek stateFileContentDOWWt, WtDaysOfWeek tempContentDOWWt)
		{

			if ((stateFileContentDOWWt == null && tempContentDOWWt != null) || (stateFileContentDOWWt != null && tempContentDOWWt == null))
				return false;



            if ((stateFileContentDOWWt.IsOff != tempContentDOWWt.IsOff) )
                return false;
			if (stateFileContentDOWWt != null && tempContentDOWWt != null)
			{
				if (stateFileContentDOWWt.lstWeight.Count != tempContentDOWWt.lstWeight.Count)
					return false;
				for (int count = 0; count < stateFileContentDOWWt.lstWeight.Count; count++)
				{
					if (stateFileContentDOWWt.lstWeight[count].Key != tempContentDOWWt.lstWeight[count].Key || stateFileContentDOWWt.lstWeight[count].Value != tempContentDOWWt.lstWeight[count].Value)
						return false;
				}

			}



			return true;
		}

		private bool ComaparDaysOfMonthWt(WBid.WBidiPad.Model.State.Weights.DaysOfMonthWt stateFileContentDaysOfMonthWt, WBid.WBidiPad.Model.State.Weights.DaysOfMonthWt tempContentDaysOfMonthWt)
		{

			if ((stateFileContentDaysOfMonthWt == null && tempContentDaysOfMonthWt != null) || (stateFileContentDaysOfMonthWt != null && tempContentDaysOfMonthWt == null))
				return false;


			if (stateFileContentDaysOfMonthWt != null && tempContentDaysOfMonthWt != null)
			{
				if (stateFileContentDaysOfMonthWt.isWork != tempContentDaysOfMonthWt.isWork)
					return false;


				if ((stateFileContentDaysOfMonthWt.Weights == null && tempContentDaysOfMonthWt.Weights != null) || (stateFileContentDaysOfMonthWt.Weights != null && tempContentDaysOfMonthWt.Weights == null))
					return false;


				if (stateFileContentDaysOfMonthWt.Weights != null && tempContentDaysOfMonthWt.Weights != null)
				{
					if (stateFileContentDaysOfMonthWt.Weights.Count != tempContentDaysOfMonthWt.Weights.Count)
						return false;

					for (int count = 0; count < stateFileContentDaysOfMonthWt.Weights.Count; count++)
					{
						if (stateFileContentDaysOfMonthWt.Weights[count].Key != tempContentDaysOfMonthWt.Weights[count].Key || stateFileContentDaysOfMonthWt.Weights[count].Value != tempContentDaysOfMonthWt.Weights[count].Value)
							return false;
					}
				}
			}



			return true;
		}


		private bool ComaparDaysOfMonthWt(WtPDOFS stateFileContentWtPDOFS, WtPDOFS tempContentWtPDOFS)
		{
			if ((stateFileContentWtPDOFS == null && tempContentWtPDOFS != null) || (stateFileContentWtPDOFS != null && tempContentWtPDOFS == null))
				return false;
			if (stateFileContentWtPDOFS != null && tempContentWtPDOFS != null)
			{
				if (stateFileContentWtPDOFS.Count != tempContentWtPDOFS.Count)
					return false;


				for (int count = 0; count < stateFileContentWtPDOFS.Count; count++)
				{
					if (stateFileContentWtPDOFS[count].City != tempContentWtPDOFS[count].City || stateFileContentWtPDOFS[count].Date != tempContentWtPDOFS[count].Date ||
						stateFileContentWtPDOFS[count].IsBefore != tempContentWtPDOFS[count].IsBefore || stateFileContentWtPDOFS[count].Time != tempContentWtPDOFS[count].Time ||
						stateFileContentWtPDOFS[count].Weight != tempContentWtPDOFS[count].Weight
					)
						return false;
				}
			}


			return true;
		}

		private bool ComaparCommutableLineWt(WtCommutableLine stateFileContentWtCommutableLine, WtCommutableLine tempContentWtCommutableLine)
		{

			if ((stateFileContentWtCommutableLine == null && tempContentWtCommutableLine != null) || (stateFileContentWtCommutableLine != null && tempContentWtCommutableLine == null))
				return false;

			if (stateFileContentWtCommutableLine != null && tempContentWtCommutableLine != null)
			{
				if (stateFileContentWtCommutableLine.BothEnds != tempContentWtCommutableLine.BothEnds || stateFileContentWtCommutableLine.InDomicile != tempContentWtCommutableLine.InDomicile || stateFileContentWtCommutableLine.Type != tempContentWtCommutableLine.Type)
					return false;



				if ((stateFileContentWtCommutableLine.TimesList == null && tempContentWtCommutableLine.TimesList != null) || (stateFileContentWtCommutableLine.TimesList != null && tempContentWtCommutableLine.TimesList == null))
					return false;


				if (stateFileContentWtCommutableLine.TimesList != null && tempContentWtCommutableLine.TimesList != null)
				{
					if (stateFileContentWtCommutableLine.TimesList.Count != tempContentWtCommutableLine.TimesList.Count)
						return false;

					for (int count = 0; count < stateFileContentWtCommutableLine.TimesList.Count; count++)
					{
						if (stateFileContentWtCommutableLine.TimesList[count].BackToBase != tempContentWtCommutableLine.TimesList[count].BackToBase || stateFileContentWtCommutableLine.TimesList[count].Checkin != tempContentWtCommutableLine.TimesList[count].Checkin)
							return false;
					}
				}
			}


			return true;
		}


		#endregion

	}
}

