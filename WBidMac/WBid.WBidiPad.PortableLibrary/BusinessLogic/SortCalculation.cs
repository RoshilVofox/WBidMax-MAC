using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
    public class SortCalculation
    {


        public void SortLines(string sortParameter)
        {
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            var topLockList = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
            var botLockList = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
            List<Line> forceLineList = null; ;
            //Generate  BlankLine and force Line list
            forceLineList = GenerateForceLineList();
            SortingPointCalculationLogic(sortParameter);

            if (sortParameter != "Manual")
            {

                RemoveConstantItems(topLockList, botLockList, forceLineList);

                switch (sortParameter)
                {
                    case "Line":

                        GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderBy(x => x.LineNum));
                        break;

                    case "LinePay":
                    case "PayPerDay":
                    case "PayPerDutyHour":
                    case "PayPerFlightHour":
                    case "PayPerTimeAway":

                        GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderByDescending(x => x.Points));
                        break;
                    case "Award":
                        if (GlobalSettings.WBidStateCollection.BidAwards != null)
                        {
                            var awardOrder = GlobalSettings.WBidStateCollection.BidAwards.OrderBy(x => x.SeqNumber);
                            var Sortlines = GlobalSettings.Lines.Where(x => awardOrder.Any(y => y.LineNum == x.LineNum));
                            var linesnotinAwards = GlobalSettings.Lines.Except(Sortlines);
                            var sortedLines = Sortlines.OrderBy(x => awardOrder.Select(y => y.LineNum).ToList().IndexOf(x.LineNum)).ToList();
                            sortedLines.AddRange(linesnotinAwards);
                            GlobalSettings.Lines = new ObservableCollection<Line>(sortedLines);
                        }
                        break;
                    case "SubmittedBid":
                        if (GlobalSettings.WBidStateCollection.SubmittedResult != null)
                        {
                            List<int> submittedlist = GlobalSettings.WBidStateCollection.SubmittedResult.Split(',').Select(int.Parse).ToList();
                            var submittedlines = GlobalSettings.Lines.Where(x => submittedlist.Any(y => y == x.LineNum));
                            var linesnotinsubmitttedResult = GlobalSettings.Lines.Except(submittedlines);
                            var submittedLines = submittedlines.OrderBy(x => submittedlist.IndexOf(x.LineNum)).ToList();
                            submittedLines.AddRange(linesnotinsubmitttedResult);
                            GlobalSettings.Lines = new ObservableCollection<Line>(submittedLines);
                        }
                        break;



                    case "BlockSort":

                        BlockSortLogic();

                        break;
                    case "SelectedColumn":
                        if (!string.IsNullOrEmpty(wBidStateContent.SortDetails.SortColumnName))
                        {
                            System.Reflection.PropertyInfo prop = typeof(Line).GetProperty(wBidStateContent.SortDetails.SortColumnName);
                            if (wBidStateContent != null && wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortDirection != null)
                            {
                                if (wBidStateContent.SortDetails.SortDirection == "Asc")
                                {
                                    if (wBidStateContent.SortDetails.SortColumnName == "FAPositions")
                                    {
                                        GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderBy(x => string.Join("", x.FAPositions.ToArray())));
                                    }
                                    else
                                    {
                                        GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderBy(x => prop.GetValue(x, null)));
                                        if (forceLineList != null)
                                        {
                                            var blanklines = forceLineList.Where(x => x.BlankLine);
                                            if (blanklines != null)
                                            {

                                                blanklines = blanklines.OrderBy(x => x.LineNum).ToList();
                                                forceLineList.RemoveAll(x => x.BlankLine);
                                                forceLineList.InsertRange(forceLineList.Count, blanklines);
                                            }
                                        }
                                    }


                                }
                                else
                                {
                                    if (wBidStateContent.SortDetails.SortColumnName == "FAPositions")
                                    {
                                        GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderByDescending(x => string.Join("", x.FAPositions.ToArray())));
                                    }
                                    else
                                    {
                                        GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderByDescending(x => prop.GetValue(x, null)));
                                        if (forceLineList != null)
                                        {
                                            var blanklines = forceLineList.Where(x => x.BlankLine);
                                            if (blanklines != null && wBidStateContent.SortDetails.SortColumnName == "LineNum")
                                            {

                                                blanklines = blanklines.OrderByDescending(x => x.LineNum).ToList();
                                                forceLineList.RemoveAll(x => x.BlankLine);
                                                forceLineList.InsertRange(forceLineList.Count, blanklines);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        break;

                }

                if (sortParameter != "Line")
                {
                    OrderNonConstraintLinesToTheTop();
                }
                if (forceLineList != null && sortParameter != "SelectedColumn")
                {
                    var blanklines = forceLineList.Where(x => x.BlankLine);
                    if (blanklines != null)
                    {

                        blanklines = blanklines.OrderBy(x => x.LineNum).ToList();
                        forceLineList.RemoveAll(x => x.BlankLine);
                        forceLineList.InsertRange(forceLineList.Count, blanklines);
                    }
                }
                //Add TopLock and BottomLock list back to Line list
                AddConstantItems(topLockList, botLockList, forceLineList);
            }


        }

        private List<Line> GenerateForceLineList()
        {
            List<Line> forceLineList = new List<Line>();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (wBidStateContent.ForceLine.IsBlankLinetoBottom && wBidStateContent.ForceLine.IsReverseLinetoBottom)
            {
                forceLineList = GlobalSettings.Lines.Where(x => (x.BlankLine || x.ReserveLine) && !x.TopLock & !x.BotLock).ToList();
            }
            else if (wBidStateContent.ForceLine.IsBlankLinetoBottom)
            {
                forceLineList = GlobalSettings.Lines.Where(x => x.BlankLine && !x.TopLock & !x.BotLock).ToList();
            }
            else if (wBidStateContent.ForceLine.IsReverseLinetoBottom)
            {
                forceLineList = GlobalSettings.Lines.Where(x => x.ReserveLine && !x.TopLock & !x.BotLock).ToList();
            }
            return forceLineList;
        }

        /// <summary>
        /// Sorting point claculation
        /// </summary>
        /// <param name="sortColumn"></param>
        private void SortingPointCalculationLogic(string sortColumn)
        {
            try
            {
                switch (sortColumn)
                {
                    case "Line":
				case  "Manual"   :
				case   "SelectedColumn":
                        foreach (var x in GlobalSettings.Lines)
                        {
                            x.Points = x.Tfp == 0 ? 0 : Math.Round((x.Tfp + x.WeightPoints.Total()), 2);
                        }
                        break;
                    case "LinePay":
                        foreach (var x in GlobalSettings.Lines)
                        {
                            x.Points = x.Tfp == 0 ? 0 : Math.Round((x.Tfp + x.WeightPoints.Total()), 2);
                        }
                        break;
                    case "PayPerDay":
                        foreach (var x in GlobalSettings.Lines)
                        {
                            x.Points = x.DaysWorkInLine == 0 ? 0 : Math.Round((14 * x.TfpInLine / x.DaysWorkInLine) + x.WeightPoints.Total(), 2);
                        }
                        break;
                    case "PayPerDutyHour":
                        foreach (var x in GlobalSettings.Lines)
                        {
                            x.Points = x.DutyHrsInLine == null || x.DutyHrsInLine == "0:0" ? 0 : Math.Round((120 * x.TfpInLine / Helper.ConvertHhhColonMmToFractionalHours(x.DutyHrsInLine) + x.WeightPoints.Total()), 2);
                        }
                        break;
                    case "PayPerFlightHour":
                        foreach (var x in GlobalSettings.Lines)
                        {
                            x.Points = x.BlkHrsInLine == null || x.BlkHrsInLine == "0:0" ? 0 : Math.Round((80 * x.TfpInLine / Helper.ConvertHhhColonMmToFractionalHours(x.BlkHrsInLine) + x.WeightPoints.Total()), 2);
                        }
                        break;
                    case "PayPerTimeAway":
                        foreach (var x in GlobalSettings.Lines)
                        {
                            x.Points = x.TafbInLine == null || x.TafbInLine == "0:0" ? 0 : Math.Round((300 * x.TfpInLine / Helper.ConvertHhhColonMmToFractionalHours(x.TafbInLine) + x.WeightPoints.Total()), 2);
                        }

                        break;

                    case "BlockSort":
                        foreach (var x in GlobalSettings.Lines)
                        {
                            x.Points = x.Points = Math.Round(x.WeightPoints.Total(), 1);
                        }

                        break;


                }
            }
            catch (Exception)
            {
                //PayPerTimeAway SORT causing exception.Need to recheck it.TafbInLine property is null.
                //throw;
            }




        }

        private void RemoveConstantItems(List<Line> topLockList, List<Line> botLockList, List<Line> forceLineList)
        {


            //Remove itemes having Tiplock=true from the Line list
            if (topLockList != null)
            {

                foreach (var item in topLockList)
                {
                    if (GlobalSettings.Lines.Contains(item))
                    {
                        GlobalSettings.Lines.Remove(item);
                    }

                }
            }

            //Remove itemes having BotLock=true from the Line list

            if (botLockList != null)
            {

                foreach (var item in botLockList)
                {
                    if (GlobalSettings.Lines.Contains(item))
                    {
                        GlobalSettings.Lines.Remove(item);
                    }

                }


            }

            if (forceLineList != null)
            {

                foreach (var item in forceLineList)
                {
                    if (GlobalSettings.Lines.Contains(item))
                    {
                        GlobalSettings.Lines.Remove(item);
                    }

                }
               
            }
        }

        private void AddConstantItems(List<Line> topLockList, List<Line> botLockList, List<Line> forceLineList)
        {
            if (topLockList != null)
            {
                topLockList.Reverse();

                foreach (var item in topLockList)
                {
                    GlobalSettings.Lines.Insert(0, item);
                }
                
            }
            if (forceLineList != null)
            {
                foreach (var item in forceLineList)
                {
                    GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, item);
                }
                
            }
            if (botLockList != null)
            {
                foreach (var item in botLockList)
                {
                    GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, item);
                }
          
            }
        }

        private void BlockSortLogic()
        {

            ObservableCollection<BlockSort> lstBlockSortColumns = new ObservableCollection<BlockSort>();
            WBidState WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

            foreach (string sortKey in WBidStateContent.SortDetails.BlokSort)
            {
                if (sortKey != string.Empty)
                {
                    lstBlockSortColumns.Add(WBidCollection.GetBlockSortListDataForInternalCalculation().FirstOrDefault(y => y.Id.ToString() == sortKey));
                }
            }

            //read the block sort target list


            lstBlockSortColumns.ToList().RemoveAll(x => x == null);
            bool isFirstcolumn = true;

            if (lstBlockSortColumns.Count != 0)
            {
                //Building expression tree to dynamically sort a dictionary of dictionaries in LINQ
                var iQueryable = GlobalSettings.Lines.AsQueryable();
                IOrderedQueryable<Line> iOrderedQueryable = null;

                foreach (BlockSort blockSort in lstBlockSortColumns)
                {

                    switch (blockSort.Name)
                    {
                        case "AMs then PMs":

                            //iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderBy(x => x.AMPMSortOrder).ThenBy(x => x.ReserveLine) : iOrderedQueryable.ThenBy(x => x.AMPMSortOrder).ThenBy(x => x.ReserveLine);
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderBy(x => x.AMPMSortOrder) : iOrderedQueryable.ThenBy(x => x.AMPMSortOrder);
                            if (lstBlockSortColumns.Count == 1)
                            {
                                iOrderedQueryable = iOrderedQueryable.ThenBy(x => x.ReserveLine);
                            }
                            break;

                        case "Blocks of Day Off":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.LargestBlkOfDaysOff) : iOrderedQueryable.ThenByDescending(x => x.LargestBlkOfDaysOff);
                            break;

                        case "Days Off":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.DaysOff) : iOrderedQueryable.ThenByDescending(x => x.DaysOff);
                            break;

                        case "Flight Time":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderBy(x => x.BlkHrsInBp) : iOrderedQueryable.ThenBy(x => x.BlkHrsInBp);
                            break;

                        case "Pay Credit":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.Tfp) : iOrderedQueryable.ThenByDescending(x => x.Tfp);
                            break;

                        case "Pay per Day":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.TfpPerDay) : iOrderedQueryable.ThenByDescending(x => x.TfpPerDay);
                            break;

                        case "Pay Per Duty Hour":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.TfpPerDhr) : iOrderedQueryable.ThenByDescending(x => x.TfpPerDhr);
                            break;

                        case "Pay per Flight Hour":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.TfpPerFltHr) : iOrderedQueryable.ThenByDescending(x => x.TfpPerFltHr);
                            break;

                        case "Pay per TAFB":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.TfpPerTafb) : iOrderedQueryable.ThenByDescending(x => x.TfpPerTafb);
                            break;

                        case "Per Diem":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.TafbInBp) : iOrderedQueryable.ThenByDescending(x => x.TafbInBp);
                            break;

                        case "PMs then AMs":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.AMPMSortOrder).ThenBy(x => x.ReserveLine) : iOrderedQueryable.ThenByDescending(x => x.AMPMSortOrder).ThenBy(x => x.ReserveLine);

                            break;

                        case "Start Day of Week":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.StartDow) : iOrderedQueryable.ThenByDescending(x => x.StartDow);
                            break;

                        case "Weekday Pairings":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderBy(x => x.BlankLine).OrderBy(x => x.Weekend) : iOrderedQueryable.ThenBy(x => x.BlankLine).ThenBy(x => x.Weekend);
                            break;


                        case "Largest Blk of Days Off":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.LargestBlkOfDaysOff) : iOrderedQueryable.ThenByDescending(x => x.LargestBlkOfDaysOff);
                            break;

                        case "Weights":
                            if (lstBlockSortColumns.Count == 1)
                            {
                                iOrderedQueryable = iQueryable.OrderByDescending(x => x.WeightPoints.Total());
                                iOrderedQueryable.ThenBy(x => x.LineNum);
                            }
                            else
                            {

                                iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.WeightPoints.Total()) : iOrderedQueryable.ThenByDescending(x => x.WeightPoints.Total());
                            }
                            //iOrderedQueryable = iOrderedQueryable.ThenBy(x => x.LineNum);
                            break;


                        case "Duty Periods (asc)":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderBy(x => x.TotDutyPdsInBp) : iOrderedQueryable.ThenBy(x => x.TotDutyPdsInBp);
                            break;
                        case "VacPay Current Bid Period":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.VacPayCuBp) : iOrderedQueryable.ThenByDescending(x => x.VacPayCuBp);
                            break;
                        case "VacPay Next Bid Period":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.VacPayNeBp) : iOrderedQueryable.ThenByDescending(x => x.VacPayNeBp);
                            break;
                        case "VacPay Both Current and Next":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.VacPayBothBp) : iOrderedQueryable.ThenByDescending(x => x.VacPayBothBp);
                            break;
                        case "CommutableLine-Auto Overall":
                        case "CommutableLine-Manual Overall":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.CommutabilityOverall) : iOrderedQueryable.ThenByDescending(x => x.CommutabilityOverall);
                            break;
                        case "CommutableLine-Auto Front":
                        case "CommutableLine-Manual Front":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.CommutabilityFront) : iOrderedQueryable.ThenByDescending(x => x.CommutabilityFront);
                            break;
                        case "CommutableLine-Auto Back":
                        case "CommutableLine-Manual Back":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.CommutabilityBack) : iOrderedQueryable.ThenByDescending(x => x.CommutabilityBack);
                            break;
                        case "Ratio":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.Ratio) : iOrderedQueryable.ThenByDescending(x => x.Ratio);
                            break;
                        case "ETOPS":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.ETOPS) : iOrderedQueryable.ThenByDescending(x => x.ETOPS);
                            break;
                        case "Line Rig":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.LineRig) : iOrderedQueryable.ThenByDescending(x => x.LineRig);
                            break;
                        case "Vac+LG":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.VacPlusRig) : iOrderedQueryable.ThenByDescending(x => x.VacPlusRig);
                            break;
                        case "ETOPS Trips":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.ETOPSTripsCount) : iOrderedQueryable.ThenByDescending(x => x.ETOPSTripsCount);
                            break;
                        case "DH First":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.DhFirstTotal) : iOrderedQueryable.ThenByDescending(x => x.DhFirstTotal);
                            break;
                        case "DH Last":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.DhLastTotal) : iOrderedQueryable.ThenByDescending(x => x.DhLastTotal);
                            break;
                        case "DH Total":
                            iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.DhTotal) : iOrderedQueryable.ThenByDescending(x => x.DhTotal);
                            break;
                    }

                    isFirstcolumn = false;
                }
                iOrderedQueryable = iOrderedQueryable.ThenBy(x => x.LineNum);

                GlobalSettings.Lines = new ObservableCollection<Line>(iOrderedQueryable.ToList());




            }






        }

        private void OrderNonConstraintLinesToTheTop()
        {


            var nonConstraintList = GlobalSettings.Lines.Where(x => !x.Constrained).ToList();
            //Remove itemes having Constrained=false from the Line list
            if (nonConstraintList != null)
            {

                foreach (var item in nonConstraintList)
                {
                    if (GlobalSettings.Lines.Contains(item))
                    {
                        GlobalSettings.Lines.Remove(item);
                    }
                }

               
                nonConstraintList.Reverse();

                foreach (var item in nonConstraintList)
                {
                   GlobalSettings.Lines.Insert(0, item);
                }
               
            }


        }

    }
}
