using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Text.RegularExpressions;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class StateManagement
    {
        public void UpdateWBidStateContent()
        {
            
            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            if (GlobalSettings.Lines != null && GlobalSettings.Lines.Count > 0)
            {
                wBidStateContent.TagDetails = new TagDetails();
                wBidStateContent.TagDetails.AddRange(GlobalSettings.Lines.ToList().Where(x => x.Tag != null && x.Tag.Trim() != string.Empty).Select(y => new Tag { Line = y.LineNum, Content = y.Tag }));


                int toplockcount =GlobalSettings.Lines.Where(x => x.TopLock == true).ToList().Count;
                int bottomlockcount =GlobalSettings.Lines.Where(x => x.BotLock == true).ToList().Count;
                //save the top and bottom lock
                wBidStateContent.TopLockCount = toplockcount;
                wBidStateContent.BottomLockCount = bottomlockcount;

                //Get the line oreder
                List<int> lineorderlist =GlobalSettings.Lines.Select(x => x.LineNum).ToList();
                LineOrders lineOrders = new LineOrders();
                int count = 1;
                lineOrders.Orders = lineorderlist.Select(x => new LineOrder() { LId = x, OId = count++ }).ToList();
                lineOrders.Lines = lineorderlist.Count;
                wBidStateContent.LineOrders = lineOrders;

                //save the state of the Reserve line to botttom or blank line to bottom
                //wBidStateContent.ForceLine.IsBlankLinetoBottom = IsBlankLineToBotttom;
               // wBidStateContent.ForceLine.IsReverseLinetoBottom = IsReserveLineLineToBotttom;



                //string currentsortmethod = GetCurrentSortMetod();
                //if (currentsortmethod != "SelectedColumn")
                //{
                //    wBidStateContent.SortDetails.SortColumn = currentsortmethod;

                //}
                // Set the status of the Menu bar check box to the state file.
                SetMenuBarButtonStatusToStateFile(wBidStateContent);

				if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue && GlobalSettings.FAEOMStartDate != DateTime.MinValue.ToUniversalTime ()))
					wBidStateContent.FAEOMStartDate = GlobalSettings.FAEOMStartDate;
				else
					wBidStateContent.FAEOMStartDate = DateTime.MinValue.ToUniversalTime ();

                wBidStateContent.LineForBlueLine = GlobalSettings.Lines.Where(x => x.ManualScroll == 3 || x.ManualScroll == 1).Select(x => x.LineNum).FirstOrDefault();
                wBidStateContent.LinesForBlueBorder = GlobalSettings.Lines.Where(x => x.ManualScroll == 2).Select(x => x.LineNum).ToList();
            }

            //return wBidStateContent;

        }
        /// <summary>
        /// Set the status of the Menu bar check box to the state file.
        /// </summary>
        private void SetMenuBarButtonStatusToStateFile(WBidState wBidStateContent)
        {
            if (wBidStateContent != null && wBidStateContent.MenuBarButtonState != null)
            {
                wBidStateContent.MenuBarButtonState.IsVacationCorrection = GlobalSettings.MenuBarButtonStatus.IsVacationCorrection;
                wBidStateContent.MenuBarButtonState.IsVacationDrop = GlobalSettings.MenuBarButtonStatus.IsVacationDrop;
                wBidStateContent.MenuBarButtonState.IsOverlap = GlobalSettings.MenuBarButtonStatus.IsOverlap;
                wBidStateContent.MenuBarButtonState.IsEOM = GlobalSettings.MenuBarButtonStatus.IsEOM;
				wBidStateContent.MenuBarButtonState.IsMIL = GlobalSettings.MenuBarButtonStatus.IsMIL;
            }
        }
        public void ReloadDataFromStateFile()
        {
            var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            ////set constraints
            //ConstraintCalculations.CalculateAllConstraints(Lines);
            ////Calculate weights
            //WeightCalculation.CalculateAllWieghts(Lines);

           // setForceLinefromStateFile();
            //Set  Lines order based on the line order saved in the state file
            SortLineOrderBasedOnStateFileLineOrder(wBidStateContent);

            //Set the Bottom and Top lock items in the line list from state file values
            SetTopandBottomLockBasedOnStateFile(wBidStateContent);

            //Set sort columns information state file
            SetSortColumnInformationFromStateFile(wBidStateContent);

            ConstraintCalculations constaintcalculation = new ConstraintCalculations();
            constaintcalculation.ApplyAllConstraints();

			WeightCalculation weightCalculation = new WeightCalculation();
			weightCalculation.ApplyAllWeights ();



              if ( GlobalSettings.CurrentBidDetails.Postion=="FA" && (wBidStateContent.FAEOMStartDate != null && wBidStateContent.FAEOMStartDate!= DateTime.MinValue && wBidStateContent.FAEOMStartDate!=DateTime.MinValue.ToUniversalTime()))
                  GlobalSettings.FAEOMStartDate = wBidStateContent.FAEOMStartDate;
                else
                  GlobalSettings.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();

			SetBidAutoGroupNumberFromStateFile (wBidStateContent);
            //SetMenuBarButtonStatusFromStateFile(wBidStateContent);
        }
		public void ReloadLineDetailsBasedOnPreviousState(bool isNeedToRecalculateLineProp)
		{
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent != null)
			{


				GlobalSettings.CompanyVA = GlobalSettings.WBidStateCollection.CompanyVA;
				SetMenuBarButtonStatusFromStateFile(wBidStateContent);
				//Setting  status to Global variables
				SetVacationOrOverlapExists(wBidStateContent);

				if (isNeedToRecalculateLineProp) {

					RecalculateLineProperties(wBidStateContent);
				}

				ReloadStateContent(wBidStateContent);

			}
		}

		public void RecalculateLineProperties(WBidState wBidStateContent)
		{
			WBidCollection.GenarateTempAbsenceList();
			PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView();

			if (wBidStateContent.MenuBarButtonState.IsOverlap)
			{
				ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection();
				reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection(GlobalSettings.Lines.ToList(), true);
			}
			else
			{
				RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties();
				RecalcalculateLineProperties.CalcalculateLineProperties();

			}
			if (!GlobalSettings.MenuBarButtonStatus.IsMIL) {
				prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
			}

		}
		public void ReloadLineDetailsBasedOnSynchedState(bool isRecreatedMILFile)
		{
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent != null)
			{
				wBidStateContent.SortDetails.SortColumn = "Manual";
				bool isNeedToRecalculateLineProp = false;

				isNeedToRecalculateLineProp = CheckLinePropertiesNeedToRecalculate(wBidStateContent);
				GlobalSettings.CompanyVA = GlobalSettings.WBidStateCollection.CompanyVA;
				SetMenuBarButtonStatusFromStateFile(wBidStateContent);
				//Setting  status to Global variables
				SetVacationOrOverlapExists(wBidStateContent);

				if (wBidStateContent.MenuBarButtonState.IsMIL && isRecreatedMILFile) 
				{
					isNeedToRecalculateLineProp = true;
				}
				if (isNeedToRecalculateLineProp) {

					WBidCollection.GenarateTempAbsenceList ();
					PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();

					prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();

					RecalcalculateLineProperties RecalcalculateLineProperties = new PortableLibrary.BusinessLogic.RecalcalculateLineProperties ();
					RecalcalculateLineProperties.CalcalculateLineProperties ();

					if (wBidStateContent.MenuBarButtonState.IsOverlap) {
						ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
						reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
					}


				}




				ReloadStateContent(wBidStateContent);



			}
		}

		public void ReloadStateContent(WBidState wBidStateContent)
		{


			//Setting Button status to Global variables
			//  SetMenuBarButtonStatusFromStateFile(wBidStateContent);
			//Setting  status to Global variables
			// SetVacationOrOverlapExists(wBidStateContent);
			//St the line order based on the state file.
			SortLineOrderBasedOnStateFileLineOrder(wBidStateContent);

			SetTopandBottomLockBasedOnStateFile(wBidStateContent);


			ConstraintCalculations constaintcalculation = new ConstraintCalculations();
			constaintcalculation.ApplyAllConstraints();

			WeightCalculation weightCalculation = new WeightCalculation();
			weightCalculation.ApplyAllWeights();



			SortCalculation sort = new SortCalculation();
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			{
				sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			}

			SetBidAutoGroupNumberFromStateFile (wBidStateContent);
		}

		public void ApplyCSW(WBidState wBidStateContent)
		{
			ConstraintCalculations constaintcalculation = new ConstraintCalculations();
			constaintcalculation.ApplyAllConstraints();

			WeightCalculation weightCalculation = new WeightCalculation();
			weightCalculation.ApplyAllWeights();



			SortCalculation sort = new SortCalculation();
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			{
				sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			}

		}

		/// <summary>
		/// Check whether we need to recalculate the line properties
		/// </summary>
		/// <returns></returns>
		public bool CheckLinePropertiesNeedToRecalculate(WBidState wBidStateContent)
		{
			bool status = false;
			//  if (wBidStateContent != null && wBidStateContent.MenuBarButtonState != null)
			// {
			//TRUE means they are having same Menu Button state
			status = (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection == wBidStateContent.MenuBarButtonState.IsVacationCorrection &&
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop == wBidStateContent.MenuBarButtonState.IsVacationDrop &&
				GlobalSettings.MenuBarButtonStatus.IsOverlap == wBidStateContent.MenuBarButtonState.IsOverlap &&
				GlobalSettings.MenuBarButtonStatus.IsEOM == wBidStateContent.MenuBarButtonState.IsEOM&&
				GlobalSettings.MenuBarButtonStatus.IsMIL == wBidStateContent.MenuBarButtonState.IsMIL);
			status = !status;
			// }
			return status;
		}

		public void SetMenuBarButtonStatusFromStateFile(WBidState wBidStateContent)
		{
			if (wBidStateContent.MenuBarButtonState != null)
			{
				if (GlobalSettings.MenuBarButtonStatus == null)
					GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus();
				GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = wBidStateContent.MenuBarButtonState.IsVacationCorrection;
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = wBidStateContent.MenuBarButtonState.IsVacationDrop;
				GlobalSettings.MenuBarButtonStatus.IsOverlap = wBidStateContent.MenuBarButtonState.IsOverlap;
				GlobalSettings.MenuBarButtonStatus.IsEOM = wBidStateContent.MenuBarButtonState.IsEOM;
				GlobalSettings.MenuBarButtonStatus.IsMIL = wBidStateContent.MenuBarButtonState.IsMIL;
			}
		}

		public void SetVacationOrOverlapExists(WBidState wBidStateContent)
		{
			GlobalSettings.IsOverlapCorrection = wBidStateContent.IsOverlapCorrection;
			GlobalSettings.IsVacationCorrection = wBidStateContent.IsVacationOverlapOverlapCorrection;
			if (GlobalSettings.CurrentBidDetails.Postion == "FA" && (wBidStateContent.FAEOMStartDate != null && wBidStateContent.FAEOMStartDate != DateTime.MinValue && wBidStateContent.FAEOMStartDate != DateTime.MinValue.ToUniversalTime()))
				GlobalSettings.FAEOMStartDate = wBidStateContent.FAEOMStartDate;
			else
				GlobalSettings.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
		}


//        private void SetMenuBarButtonStatusFromStateFile(WBidState wBidStateContent)
//        {
//            if (wBidStateContent != null && wBidStateContent.MenuBarButtonState != null)
//            {
//                GlobalSettings.MenuBarButtonStatus.IsVacationCorrection=wBidStateContent.MenuBarButtonState.IsVacationCorrection ;
//                GlobalSettings.MenuBarButtonStatus.IsVacationDrop=wBidStateContent.MenuBarButtonState.IsVacationDrop;
//                  GlobalSettings.MenuBarButtonStatus.IsOverlap=wBidStateContent.MenuBarButtonState.IsOverlap;
//                  GlobalSettings.MenuBarButtonStatus.IsEOM=wBidStateContent.MenuBarButtonState.IsEOM ;
//            }
//        }
        /// <summary>
        /// Set  Lines order based on the line order saved in the state file
        /// </summary>
        private void SortLineOrderBasedOnStateFileLineOrder(WBidState wBidStateContent)
        {
            LineOrders lineOrders = wBidStateContent.LineOrders;
            var orderedlines =GlobalSettings.Lines.ToList().OrderBy(x => lineOrders.Orders.Select(y => y.LId).ToList().IndexOf(x.LineNum));
            GlobalSettings.Lines = new ObservableCollection<Line>(orderedlines);
        }
        /// <summary>
        /// Set the Bottom and Top lock items in the line list from state file values
        /// </summary>
        private void SetTopandBottomLockBasedOnStateFile(WBidState wBidStateContent)
        {
            int toplockcount = wBidStateContent.TopLockCount;
            int bottomlockcount = wBidStateContent.BottomLockCount;

            foreach (Line line in GlobalSettings.Lines)
            {
                line.TopLock = false;
            }
            foreach (Line line in GlobalSettings.Lines)
            {
                line.BotLock = false;
            }
            
            var toplockedlines =GlobalSettings.Lines.ToList().Take(toplockcount);
            foreach (Line line in toplockedlines)
            {
                line.TopLock = true;
            }
            var bottomlockedlines =GlobalSettings. Lines.ToList().Skip(GlobalSettings.Lines.Count - bottomlockcount).ToList();
            foreach (Line line in bottomlockedlines)
            {
                line.BotLock = true;
            }
        }

        private void SetSortColumnInformationFromStateFile(WBidState wBidStateContent)
        {

        }
		private void SetBidAutoGroupNumberFromStateFile(WBidState wBidStateContent)
		{
			if (wBidStateContent.BidAutoOn) {
				if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAGroup != null && wBidStateContent.BidAuto.BAGroup.Count > 0) {

					var items = wBidStateContent.BidAuto.BAGroup.Select (x => x.GroupName).Distinct ().ToList ();

					items.Sort ((s1, s2) => {
						string pattern = "([A-Za-z])([0-9]+)";
						string h1 = Regex.Match (s1, pattern).Groups [1].Value;
						string h2 = Regex.Match (s2, pattern).Groups [1].Value;
						if (h1 != h2)
							return h1.CompareTo (h2);
						string t1 = Regex.Match (s1, pattern).Groups [2].Value;
						string t2 = Regex.Match (s2, pattern).Groups [2].Value;
						return int.Parse (t1).CompareTo (int.Parse (t2));
					});

					foreach (Line line in GlobalSettings.Lines) {
						var groupObject = wBidStateContent.BidAuto.BAGroup.FirstOrDefault (x => x.Lines.Any (y => y == line.LineNum));
						if (groupObject != null) {
							line.BAGroup = groupObject.GroupName;
							if (items != null) {
								//								int index = items.FindIndex(x => x == groupObject.GroupName);
								//								if (index != -1)
								//								{
								//									line.IsGrpColorOn = (index % 2 == 0)?1:0;
								//								}
							}
						} else
							line.BAGroup = string.Empty;
					}
				}
			}
			else
			{
				foreach (Line line in GlobalSettings.Lines) {
					line.BAGroup = string.Empty;
					line.IsGrpColorOn = 0;
				}
			}

		}
    }
}
