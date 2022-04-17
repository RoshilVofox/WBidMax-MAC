#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
#endregion

namespace WBid.WBidiPad.PortableLibrary.Utility
{
    public static class LineOperations
    {
        private static string _CurrentStartDow = string.Empty;

        #region Public Methods
		
        /// <summary>
        /// Move Blank lines to bottom
        /// </summary>
        public static void ForceBlankLinestoBottom()
        {
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            var topLockList = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
            var botLockList = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
            var blanklineList = GlobalSettings.Lines.Where(x => x.BlankLine && !x.TopLock && !x.BotLock).ToList();

            //Remove Top lock 
            foreach (var item in topLockList)
            {
                if (GlobalSettings.Lines.Contains(item))
                {
                    GlobalSettings.Lines.Remove(item);
                }
            }
            //Remove Bottom lock 
            foreach (var item in botLockList)
            {
                if (GlobalSettings.Lines.Contains(item))
                {
                    GlobalSettings.Lines.Remove(item);
                }
            }

            //Remove Blanklist  
            foreach (var item in blanklineList)
            {
                if (GlobalSettings.Lines.Contains(item))
                {
                    GlobalSettings.Lines.Remove(item);
                }
            }

            topLockList.Reverse();

            //Add top lock 
            foreach (var item in topLockList)
            {
                GlobalSettings.Lines.Insert(0, item);
            }

			if (wBidStateContent.SortDetails.SortColumn == "SelectedColumn" && wBidStateContent.SortDetails.SortColumnName == "LineNum" && wBidStateContent.SortDetails.SortDirection == "Des") {
				blanklineList = blanklineList.OrderByDescending (x => x.LineNum).ToList ();
			} else 
			{
				blanklineList = blanklineList.OrderBy(x => x.LineNum).ToList();
			}
            //Add Blank line
            foreach (var item in blanklineList)
            {
                GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, item);
            }

            //Add bottom lock
            foreach (var item in botLockList)
            {
                GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, item);
            }

        }

        /// <summary>
        /// Move reserve lines to bottom
        /// </summary>
         public static void ForceReserveLinestoBottom()
        {
            var topLockList = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
            var botLockList = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
            var reverselineList = GlobalSettings.Lines.Where(x => x.ReserveLine && !x.TopLock && !x.BotLock).ToList();

            foreach (var item in topLockList)
            {
                if (GlobalSettings.Lines.Contains(item))
                {
                    GlobalSettings.Lines.Remove(item);
                }
            }
            //Remove Bottom lock 
            foreach (var item in botLockList)
            {
                if (GlobalSettings.Lines.Contains(item))
                {
                    GlobalSettings.Lines.Remove(item);
                }
            }

            //Remove Reserve line  
            foreach (var item in reverselineList)
            {
                if (GlobalSettings.Lines.Contains(item))
                {
                    GlobalSettings.Lines.Remove(item);
                }
            }
            
         
            topLockList.Reverse();

            //Add top lock 
            foreach (var item in topLockList)
            {
                GlobalSettings.Lines.Insert(0, item);
            }
            //Add Blank line
            foreach (var item in reverselineList)
            {
                GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, item);
            }

            //Add bottom lock
            foreach (var item in botLockList)
            {
                GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, item);
            }

           
        }
      
        /// <summary>
        /// Promote/Top Lock the Lines
        /// </summary>
        /// <param name="SelectedRows"></param>
        public static void PromoteLines(List<int> SelectedRows)
        {
            try
            {
                List<Line> PromotedLines = new List<Line>();
                foreach (int linenum in SelectedRows)
                {
                    PromotedLines.Add(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum));
                }
                if (PromotedLines != null && PromotedLines.Count>0)
                {
                    PromotedLines = PromotedLines.OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x.LineNum)).ToList();
                    int currentIndex = GlobalSettings.Lines.IndexOf(PromotedLines[0]);
                    currentIndex += PromotedLines.Count;
                    if (currentIndex >= GlobalSettings.Lines.Count)
                        currentIndex = GlobalSettings.Lines.Count - 1;

                    if (PromotedLines != null && PromotedLines.Count > 0)
                    {
                        for (int i = 0; i < PromotedLines.Count; i++)
                        {
                            GlobalSettings.Lines.Remove(PromotedLines[i]);
                        }

                        int count = GlobalSettings.Lines.Where(x => x.TopLock).Count();

                        foreach (Line promoteline in PromotedLines)
                        {
                            promoteline.TopLock = true;
                            promoteline.BotLock = false;
                            GlobalSettings.Lines.Insert(count, promoteline);
                            count++;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Move the selected line above the selected line number
        /// </summary>
        /// <param name="SelectedRows"></param>
        /// <param name="lineNumberToMove"></param>
		public static bool MoveSelectedLineAbove(List<int> SelectedRows, int lineNumberToMove)
        {
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			List<Line> PromotedLines = new List<Line>();
			foreach (int linenum in SelectedRows)
			{
				if (linenum != lineNumberToMove)
				{

					PromotedLines.Add(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum));
				}
			}
			if (PromotedLines != null && PromotedLines.Count>0)
			{
				PromotedLines = PromotedLines.OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x.LineNum)).ToList();
				int currentIndex = GlobalSettings.Lines.IndexOf(PromotedLines[0]);
				currentIndex += PromotedLines.Count;
				if (currentIndex >= GlobalSettings.Lines.Count)
					currentIndex = GlobalSettings.Lines.Count - 1;

				if (PromotedLines != null && PromotedLines.Count > 0)
				{

					//  bool isMovelineSelected = false;

					for (int i = 0; i < PromotedLines.Count; i++)
					{
						//if (PromotedLines[i].LineNum != lineNumberToMove)
						//{
						GlobalSettings.Lines.Remove(PromotedLines[i]);
						//}
					}

					Line lineToMove = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineNumberToMove);


					int lineindex = GlobalSettings.Lines.IndexOf(lineToMove);
					bool toplock = false;
					bool bottomlock = false;

					if (lineToMove.TopLock)
						toplock = true;
					else if (lineToMove.BotLock)
						bottomlock = true;

					foreach (Line promoteline in PromotedLines)
					{
						promoteline.TopLock = toplock;
						promoteline.BotLock = bottomlock;
						GlobalSettings.Lines.Insert(lineindex, promoteline);
						lineindex++;
					}
				}
			}
			if (wBidStateContent.ForceLine.IsBlankLinetoBottom==true && PromotedLines.Any(x=>x.BlankLine))
			{
				wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
				return true;

				//wBidStateContent.ForceLine.IsBlankLinetoBottom
				//IsBlankLineToBotttom = false;
				//if(CSWViewModelInstance!=null)
				//	CSWViewModelInstance.IsBlankLineToBotttom = false;
				//Xceed.Wpf.Toolkit.MessageBox.Show("Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
			}
			return false;
        }

        /// <summary>
        /// Trash/Bottom Lock Lines
        /// </summary>
        /// <param name="selectedRows"></param>
        public static void TrashLines(List<int> selectedRows)
        {
            try
            {
                List<Line> TrashedLines = new List<Line>();
                foreach (int linenum in selectedRows)
                {
                    TrashedLines.Add(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum));
                }
                if (TrashedLines != null && TrashedLines.Count > 0)
                {
                    TrashedLines = TrashedLines.OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x.LineNum)).ToList();

                    int currentIndex = GlobalSettings.Lines.IndexOf(TrashedLines[0]);
                    if (currentIndex >= GlobalSettings.Lines.Count)
                        currentIndex = GlobalSettings.Lines.Count - 1;
                    if (TrashedLines != null && TrashedLines.Count > 0)
                    {
                        for (int i = 0; i < TrashedLines.Count; i++)
                        {
                            GlobalSettings.Lines.Remove(TrashedLines[i]);
                        }
                        foreach (Line trashedline in TrashedLines)
                        {
                            trashedline.TopLock = false;
                            trashedline.BotLock = true;
                            GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, trashedline);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

		public static bool MoveSelectedLineBelow(List<int> selectedRows, int lineNumberToMove)
        {
			try
			{
				var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				List<Line> TrashedLines = new List<Line>();
				foreach (int linenum in selectedRows)
				{
					if (linenum != lineNumberToMove)
					{
						TrashedLines.Add(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum));
					}
				}
				if (TrashedLines != null && TrashedLines.Count>0)
				{
					TrashedLines = TrashedLines.OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x.LineNum)).ToList();

					int currentIndex = GlobalSettings.Lines.IndexOf(TrashedLines[0]);
					if (currentIndex >= GlobalSettings.Lines.Count)
						currentIndex = GlobalSettings.Lines.Count - 1;
					if (TrashedLines != null && TrashedLines.Count > 0)
					{
						for (int i = 0; i < TrashedLines.Count; i++)
						{
							GlobalSettings.Lines.Remove(TrashedLines[i]);
						}

						Line lineToMove = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineNumberToMove);
						int lineindex = GlobalSettings.Lines.IndexOf(lineToMove);
						bool toplock = false;
						bool bottomlock = false;

						if (lineToMove.TopLock)
							toplock = true;
						else if (lineToMove.BotLock)
							bottomlock = true;
						foreach (Line trashedline in TrashedLines)
						{
							trashedline.TopLock = toplock;
							trashedline.BotLock = bottomlock;
							GlobalSettings.Lines.Insert(lineindex + 1, trashedline);
							lineindex++;
						}
					}
				}
				if (wBidStateContent.ForceLine.IsBlankLinetoBottom==true && TrashedLines.Any(x=>x.BlankLine))
				{
					wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
					return true;


				}
				return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}
        }

        /// <summary>
        /// Remove from Top lock
        /// </summary>
        /// <param name="selectedRows"></param>
        public static void RemoveFromTopLock(List<int> selectedRows)
        {
            if (selectedRows != null)
            {
                List<Line> toplockLines = new List<Line>();
                foreach (int linenum in selectedRows)
                {
                    var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum);
                    line.TopLock = false;
                    toplockLines.Add(line);
                    GlobalSettings.Lines.Remove(line);
                }
                int toplockcount = GlobalSettings.Lines.Where(x => x.TopLock).Count();
                if (toplockLines.Count != 0)
                {
                    toplockLines.Reverse();
                    foreach (Line line in toplockLines)
                    {
                        GlobalSettings.Lines.Insert(toplockcount, line);
                    }
                }
            }
        }

        /// <summary>
        /// Remove all top locks
        /// </summary>
        public static void RemoveAllTopLock()
        {
            foreach (Line line in GlobalSettings.Lines.Where(x => x.TopLock))
            {
                line.TopLock = false;
            }
        }

        /// <summary>
        /// Remove all bottom locks
        /// </summary>
        public static void RemoveAllBottomLock()
        {
            foreach (Line line in GlobalSettings.Lines.Where(x => x.BotLock))
            {
                line.BotLock = false;
            }
        }

        /// <summary>
        /// Bottom lock this line and Below
        /// </summary>
        /// <param name="linenumber"></param>
        public static void BottomLockThisLineAndBelow(int linenumber)
        {
            var bottomlockeditems = GlobalSettings.Lines.Where(x => x.BotLock);
            foreach (Line line in bottomlockeditems)
            {
                line.BotLock = false;
            }
            var bottomlockitems = GlobalSettings.Lines.SkipWhile(x => x.LineNum != linenumber);
            foreach (Line line in bottomlockitems)
            {
                line.BotLock = true;
                line.TopLock = false;
            }
            // GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber).BotLock = true;
          Line selectedLine=  GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber);
          selectedLine.BotLock = true;
          selectedLine.TopLock = false;
           
        }

        /// <summary>
        /// Remove from Bottom lock
        /// </summary>
        /// <param name="selectedRows"></param>
        public static void RemoveFromBottomLock(List<int> selectedRows)
        {
            if (selectedRows != null)
            {
                List<Line> bottomlockLines = new List<Line>();
                foreach (int linenum in selectedRows)
                {
                    var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenum);
                    line.BotLock = false;
                    bottomlockLines.Add(line);
                    GlobalSettings.Lines.Remove(line);
                }
                int bottomlockcount = GlobalSettings.Lines.Where(x => !x.BotLock).Count();
                var lineee = GlobalSettings.Lines.Where(x => x.BotLock);
                if (bottomlockLines.Count != 0)
                {
                    bottomlockLines.Reverse();
                    foreach (Line line in bottomlockLines)
                    {
                        GlobalSettings.Lines.Insert(bottomlockcount, line);
                    }
                }
            }
        }

        /// <summary>
        /// Top lock this line and above
        /// </summary>
        public static void TopLockThisLineAndAbove(int linenumber)
        {
            var toplockeditems = GlobalSettings.Lines.Where(x => x.TopLock);
            foreach (Line line in toplockeditems)
            {
                line.TopLock = false;
            }
            var toplockitems = GlobalSettings.Lines.TakeWhile(x => x.LineNum != linenumber);
            foreach (Line line in toplockitems)
            {
                line.TopLock = true;
                line.BotLock = false;
            }
           // GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber).TopLock = true;
            Line selectedLine = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber);
            selectedLine.BotLock = false;
            selectedLine.TopLock = true;
        }
      
        /// <summary>
        /// sort the coumns in ascending or decsending.
        /// </summary>
        /// <param name="sortcolumnName"></param>
        /// <param name="IsAscending"></param>
        public static void SortColumns(string sortcolumnName, bool IsAscending)
        {
			var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            if (sortcolumnName == "LineDisplay")
                sortcolumnName = "LineNum";
            if (sortcolumnName == "LastArrTime")
                sortcolumnName = "LastArrivalTime";
            if (sortcolumnName != "StartDow")
            {
                _CurrentStartDow = string.Empty;
            }
            else
            {
                sortcolumnName = "StartDowOrder";
                SortStartDay();

            }

            var topLockList = GlobalSettings.Lines.Where(x => x.TopLock).ToList();
            var botLockList = GlobalSettings.Lines.Where(x => x.BotLock).ToList();
            List<Line> forceLineList = null; ;
            //Generate  BlankLine and force Line list
            forceLineList = GenerateForceLineList(forceLineList);

            //Remove constant(Top Locks,Bottom Lock etc) from the main list
            //This records not needed sorting
            RemoveConstantItems(topLockList, botLockList, forceLineList);


            System.Reflection.PropertyInfo prop = typeof(Line).GetProperty(sortcolumnName);
            if (IsAscending)
            {
                if (sortcolumnName == "FAPositions")
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
                if (sortcolumnName == "FAPositions")
                {
                    GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderByDescending(x => string.Join("", x.FAPositions.ToArray())));
                }
                else
                {
                    GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.ToList().OrderByDescending(x => prop.GetValue(x, null)));
					if (forceLineList != null)
					{
						var blanklines = forceLineList.Where(x => x.BlankLine);
						if (blanklines != null &&  wBidStateContent.SortDetails.SortColumnName=="LineNum")
						{

							blanklines = blanklines.OrderByDescending(x => x.LineNum).ToList();
							forceLineList.RemoveAll(x => x.BlankLine);
							forceLineList.InsertRange(forceLineList.Count, blanklines);
						}

					}
                }
            }
            //var wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            SortDetails stateSortDetails = wBidStateContent.SortDetails;
            if (stateSortDetails == null)
                stateSortDetails = new SortDetails();
            stateSortDetails.SortColumn = "SelectedColumn";
            stateSortDetails.SortColumnName = sortcolumnName;
            stateSortDetails.SortDirection = (IsAscending) ? "Asc" : "Dsc";

            wBidStateContent.SortDetails = stateSortDetails;

            OrderNonConstraintLinesToTheTop();
            //Add TopLock and BottomLock list back to Line list
            AddConstantItems(topLockList, botLockList, forceLineList);


        }
        
	#endregion

        #region Private Methods
		
        /// <summary>
        /// PURPOSE :Remove constant(Top Locks,Bottom Lock etc) from the main list
        ///This records not needed for sorting
        /// </summary>
        /// <param name="topLockList"></param>
        /// <param name="botLockList"></param>
        private static void RemoveConstantItems(List<Line> topLockList, List<Line> botLockList, List<Line> forceLineList)
        {


            //Remove itemes having Tiplock=true from the Line list
            if (topLockList != null)
            {
                GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.Except(topLockList));
            }

            //Remove items having BotLock=true from the Line list

            if (botLockList != null)
            {
                GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.Except(botLockList));
            }

            if (forceLineList != null)
            {
                GlobalSettings.Lines = new ObservableCollection<Line>(GlobalSettings.Lines.Except(forceLineList));
            }
        }
      
        private static void SortStartDay()
        {

            int sunday, monday, tuesday, wednesday, thursday, friday, saturday;
            sunday = monday = tuesday = wednesday = thursday = friday = saturday = 0;
            switch (_CurrentStartDow)
            {
                case "":
                case "Sat":
                    _CurrentStartDow = "Sun";
                    sunday = 0;
                    break;
                case "Sun":
                    _CurrentStartDow = "Mon";
                    sunday = 6;
                    break;
                case "Mon":
                    _CurrentStartDow = "Tue";
                    sunday = 5;
                    break;
                case "Tue":
                    _CurrentStartDow = "Wed";
                    sunday = 4;
                    break;
                case "Wed":
                    _CurrentStartDow = "Thu";
                    sunday = 3;
                    break;
                case "Thu":
                    _CurrentStartDow = "Fri";
                    sunday = 2;
                    break;

                case "Fri":
                    _CurrentStartDow = "Sat";
                    sunday = 1;
                    break;

            }


            monday = (sunday + 1) % 7;
            tuesday = (monday + 1) % 7;
            wednesday = (tuesday + 1) % 7;
            thursday = (wednesday + 1) % 7;
            friday = (thursday + 1) % 7;
            saturday = (friday + 1) % 7;

            foreach (Line line in GlobalSettings.Lines)
            {

                switch (line.StartDow)
                {
                    case "Sun":
                        line.StartDowOrder = sunday;
                        break;
                    case "Mon":
                        line.StartDowOrder = monday;
                        break;
                    case "Tue":
                        line.StartDowOrder = tuesday;
                        break;
                    case "Wed":
                        line.StartDowOrder = wednesday;
                        break;
                    case "Thu":
                        line.StartDowOrder = thursday;
                        break;
                    case "Fri":
                        line.StartDowOrder = friday;
                        break;
                    case "Sat":
                        line.StartDowOrder = saturday;
                        break;
                    case "mix":
                        line.StartDowOrder = 7;
                        break;
                    default:
                        line.StartDowOrder = 8;
                        break;
                }
            }
        }
      
        /// <summary>
        /// PURPOSE : Add constant(Top Locks,Bottom Lock etc) to the main list
        /// </summary>
        /// <param name="topLockList"></param>
        /// <param name="botLockList"></param>
        private static void AddConstantItems(List<Line> topLockList, List<Line> botLockList, List<Line> forceLineList)
        {
            if (topLockList != null)
            {
                topLockList.Reverse();
                foreach (Line line in topLockList)
                {
                    GlobalSettings.Lines.Insert(0, line);
                }
            }
            if (forceLineList != null)
            {
                foreach (Line line in forceLineList)
                {
                    GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, line);
                }
            }
            if (botLockList != null)
            {
                foreach (Line line in botLockList)
                {
                    GlobalSettings.Lines.Insert(GlobalSettings.Lines.Count, line);
                }
            }
        }

        /// <summary>
        /// PURPOSE : 	Order NonConstraintLines To the Top of the list
        ///             Display all non-constrained lines – sort method orders lines
        ///             Display all constrained lines – sort method orders lines
        /// </summary>
        private static void OrderNonConstraintLinesToTheTop()
        {
            var nonConstraintList = GlobalSettings.Lines.Where(x => !x.Constrained).ToList();
            //Remove itemes having Constrained=false from the Line list
            if (nonConstraintList != null)
            {
                foreach (Line line in nonConstraintList)
                {
                    if (GlobalSettings.Lines.Contains(line))
                    {
                        GlobalSettings.Lines.Remove(line);
                    }
                }
                nonConstraintList.Reverse();

                foreach (Line line in nonConstraintList)
                {
                    GlobalSettings.Lines.Insert(0, line);
                }
            }
        }
   
        /// <summary>
        /// PURPOSE : Generate  BlankLine and force Line list
        /// </summary>
        /// <param name="forceLineList"></param>
        /// <returns></returns>
        private static List<Line> GenerateForceLineList(List<Line> forceLineList)
        {
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
	#endregion
    }
}
