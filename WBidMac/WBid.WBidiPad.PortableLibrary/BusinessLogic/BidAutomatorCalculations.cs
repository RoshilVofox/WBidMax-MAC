using System;
using WBid.WBidiPad.Core;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using WBid.WBidiPad.Model;
using System.Linq;
using System.Linq.Expressions;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary
{
	public class BidAutomatorCalculations
	{
		#region Private Properties

		private bool _grpColor;
		private ObservableCollection<Line> _lstLines;
		private List<Line> _topLockedMainList;
		private ObservableCollection<FilterHelper> _lstFilter;
		WBidState wBidStateContent;
		#endregion

		#region Constructor
		public BidAutomatorCalculations()
		{
			//var mainView = ServiceLocator.Current.GetInstance<MainViewModel>();
			//ClearTopLock()
			//_lstLines = mainView.Lines;
			_lstLines = GlobalSettings.Lines;
			 wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		}
		#endregion

		#region Public Methods
		public void CalculateLinePropertiesForBAFilters()
		{
			if (wBidStateContent.BidAuto != null &&
				wBidStateContent.BidAuto.BAFilter != null && wBidStateContent.BidAuto.BAFilter.Any())
			{

				foreach (var item in wBidStateContent.BidAuto.BAFilter)
				{
					switch (item.Name)
					{
					case "DOWA":
						CalculateDaysOfWeekAllFilterLineProperty(item);
						break;
					case "AP":
						CalculateAMPMFilterLineProperty(item);
						break;
					case "DOWS":
						CalculateDaysOfWeekSomeFilterLineProperty(item);
						break;
					case "DHFL":
						CalculateDeadHeadFirsLastFilterLineProperty(item);
						break;

					case "ET":
						CalculateEquipmentTypeFilterLineProperty(item);
						break;
					case "RT":
						CalculateRestFilterLineProperty(item);
						break;
					case "LT":
						CalculateLineTypeFilterLineProperty(item);
						break;
					case "TBL":
						CalculateTripBlockLengthFilterLineProperty(item);
						break;
					case "SDOW":
						CalculateStartDayofWeekFilterLineProperty(item);
						break;

					case "DOM":
						CalculateDaysOfMonthFilterLineProperty(item);
						break;
					case "OC":
						CalculateOvernightFilterLineProperty(item);
						break;
					case "CL":
						CalculateCommutableLineFilterLineProperty(item);
						break;

					}


				}
			}
		}

		public void ApplyBAFilterAndSort()
		{
			_grpColor = true;
			GenerateFilterList();

			_topLockedMainList = new List<Line>();

			//Adding existing top locked items to _topLockedMainList
			List<Line> topLockedList = _lstLines.Where(x => x.TopLock).ToList();
			if (topLockedList != null && topLockedList.Count() > 0)
			{
//				topLockedList.ForEach(s =>
//					{
//						if (_lstLines.Contains(s))
//						{
//							_topLockedMainList.Add(s);
//							_lstLines.Remove(s);
//						}
//
//					});
				foreach (var item in topLockedList) 
				{
					if (_lstLines.Contains(item))
					{
						_topLockedMainList.Add(item);
						_lstLines.Remove(item);
					}

				}
			}


			if (_lstFilter != null && _lstFilter.Count() > 0)
			{
				int countOfDifferentConstraints = _lstFilter.Count();
				int totalCombinations = (int)Math.Pow(2, countOfDifferentConstraints);
				int groupnumber = 1;


				//Managing Reserve and Blank line  --Remove the Reserve and Blank lines from the line list if needed
				//------------------------------------------------------

				List<Line> lstReserveLines = null;

				List<Line> lstBalankLines = null;

				if (_lstLines != null && wBidStateContent.BidAuto != null && (wBidStateContent.BidAuto.IsReserveBottom || wBidStateContent.BidAuto.IsBlankBottom))
				{


					//Selecting  reserve lines
					if (wBidStateContent.BidAuto.IsReserveBottom)
					{

						lstReserveLines = _lstLines.Where(x => x.ReserveLine).ToList();

						if (lstReserveLines != null && lstReserveLines.Count > 0)
						{
//							lstReserveLines.ForEach(s =>
//								{
//
//									if (_lstLines.Contains(s))
//									{
//										_lstLines.Remove(s); s.TopLock = false; s.BAGroup = string.Empty; s.IsGrpColorOn = 0;
//									}
//
//								});

							foreach (var item in lstReserveLines)
							{
								if (_lstLines.Contains(item))
								{
									_lstLines.Remove(item); item.TopLock = false; item.BAGroup = string.Empty; item.IsGrpColorOn = 0;
								}
							}
						}
					}

					//Selecting  blank lines
					if (wBidStateContent.BidAuto.IsBlankBottom)
					{

						lstBalankLines = _lstLines.Where(x => x.BlankLine).ToList();

						if (lstBalankLines != null && lstBalankLines.Count() > 0)
						{
//							lstBalankLines.ForEach(s =>
//								{
//
//									if (_lstLines.Contains(s))
//									{
//										_lstLines.Remove(s); s.TopLock = false; s.BAGroup = string.Empty; s.IsGrpColorOn = 0;
//									}
//
//								});
							foreach (var item in lstBalankLines) {
								if (_lstLines.Contains(item))
								{
									_lstLines.Remove(item); item.TopLock = false; item.BAGroup = string.Empty; item.IsGrpColorOn = 0;
								}

							}
						}
					}
				}
				//------------------------------------------------------



				//Generating different combinations
				for (int val = totalCombinations - 1; val >= 0; val--)
				{
					var tempFilter = new ObservableCollection<FilterHelper>();
					string combinationString = Convert.ToString(val, 2).PadLeft(countOfDifferentConstraints, '0');
					for (int pos = 0; pos < combinationString.Length; pos++)
					{
						tempFilter.Add(new FilterHelper
							{
								FilterName = _lstFilter[pos].FilterName,
								Priority = _lstFilter[pos].Priority,
								State = combinationString[pos] == '1' ? true : false
							});
					}

					bool isLastCombination = (val == 0);
					//Generating dynamic query based on a combination
					GenerateLineListBasedOnFilterPriority(_lstLines, tempFilter, groupnumber, isLastCombination);
					groupnumber++;
				}



				//Finally insert the toplocked list to main list
				_topLockedMainList.Reverse();
				foreach (var item in _topLockedMainList) 
				{
					_lstLines.Insert (0, item);
				}
				//_topLockedMainList.ForEach(x => _lstLines.Insert(0, x));

//				//Enable toplock button in Main view
//				if (_lstLines.Count(x => x.TopLock == true) > 0)
//				{
//					ServiceLocator.Current.GetInstance<MainViewModel>().IsTopLockEnable = true;
//				}


				if (_lstLines != null && wBidStateContent.BidAuto != null && (wBidStateContent.BidAuto.IsReserveBottom || wBidStateContent.BidAuto.IsBlankBottom))
				{
					//Both selected
					if (wBidStateContent.BidAuto.IsReserveBottom && wBidStateContent.BidAuto.IsBlankBottom)
					{
						if (wBidStateContent.BidAuto.IsReserveFirst)
						{
							if (lstReserveLines != null && lstReserveLines.Count > 0)
							{
								foreach (var item in lstReserveLines) 
								{
									_lstLines.Insert (_lstLines.Count, item);
								}
								//lstReserveLines.ForEach(x => _lstLines.Insert(_lstLines.Count, x));
							}
							if (lstBalankLines != null && lstBalankLines.Count() > 0)
							{
								foreach (var item in lstBalankLines) 
								{
									_lstLines.Insert (_lstLines.Count, item);
								}
								//lstBalankLines.ForEach(x => _lstLines.Insert(_lstLines.Count, x));
							}
						}
						else
						{
							if (lstBalankLines != null && lstBalankLines.Count() > 0)
							{
								foreach (var item in lstBalankLines) 
								{
									_lstLines.Insert (_lstLines.Count, item);
								}
								//lstBalankLines.ForEach(x => _lstLines.Insert(_lstLines.Count, x));
							}

							if (lstReserveLines != null && lstReserveLines.Count > 0)
							{
								foreach (var item in lstReserveLines) 
								{
									_lstLines.Insert (_lstLines.Count, item);
								}
								//lstReserveLines.ForEach(x => _lstLines.Insert(_lstLines.Count, x));
							}

						}
					}
					else if (wBidStateContent.BidAuto.IsReserveBottom)
					{
						if (lstReserveLines != null && lstReserveLines.Count > 0)
						{
							foreach (var item in lstReserveLines) 
							{
								_lstLines.Insert (_lstLines.Count, item);
							}
							//lstReserveLines.ForEach(x => _lstLines.Insert(_lstLines.Count, x));
						}
					}
					else if (wBidStateContent.BidAuto.IsBlankBottom)
					{
						if (lstBalankLines != null && lstBalankLines.Count() > 0)
						{
							foreach (var item in lstBalankLines) 
							{
								_lstLines.Insert (_lstLines.Count, item);
							}
							//lstBalankLines.ForEach(x => _lstLines.Insert(_lstLines.Count, x));
						}
					}
				}




			}

		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Generate Line List Based On Constrain Priority using dynamic expression  
		/// </summary>
		/// <param name="_lstLines"></param>
		/// <param name="tempFilter"></param>
		private void GenerateLineListBasedOnFilterPriority(ObservableCollection<Line> _lstLines, ObservableCollection<FilterHelper> tempFilter, int groupnumber, bool isLastCombination)
		{
			var iQueryable = _lstLines.AsQueryable();
			var param = Expression.Parameter(typeof(Line), "item");
			Expression expression = null;
			BinaryExpression expressionCondition = null;

			//Generating dynamix expression based on the constraint condition
			foreach (var constraintItem in tempFilter)
			{
				// Expression prop = null;
				switch (constraintItem.FilterName)
				{
				case "AP":
				case "DOWA":
				case "DOWS":
				case "DHFL":
				case "ET":
				case "RT":
				case "LT":
				case "TBL":
				case "SDOW":
				case "DOM":
				case "OC":
				case "CL":
					MemberExpression bAConstraints = Expression.Property(param, "BAFilters");
					IndexExpression indexedBAConstraints = Expression.MakeIndex(
						bAConstraints,
						indexer: typeof(List<bool>).GetProperty("Item", returnType: typeof(bool), types: new[] { typeof(int) }),
						arguments: new[] { Expression.Constant(constraintItem.Priority) }
					);
					ConstantExpression propValue = Expression.Constant(constraintItem.State);
					expressionCondition = Expression.Equal(indexedBAConstraints, propValue);
					break;

				}

				expression = expression == null ? expressionCondition : Expression.AndAlso(expression, expressionCondition);
			}

			if (expression != null)
			{
				var lambda = Expression.Lambda<Func<Line, bool>>(expression, param);
				var test = _lstLines.Where (x => x.BAFilters.Count == 0);
					

				var filterLines = new ObservableCollection<Line>(iQueryable.Where(lambda));


				if (wBidStateContent.BidAuto.BASort != null)
				{
					filterLines = SortingCalculationLogic(filterLines);
				}


				if (wBidStateContent.BidAuto.BAGroup == null)
				{
					wBidStateContent.BidAuto.BAGroup = new List<BidAutoGroup>();
				}
				//Removing the filtered list from and line  and adding to  _topLockedMainList list
				if (filterLines != null && filterLines.Count > 0)
				{
					//Adding Group Number to state object
					//------------------------------------
					BidAutoGroup bidAutoGroup = new BidAutoGroup();
					bidAutoGroup.GroupName = "G" + groupnumber;
					bidAutoGroup.Lines = new List<int>();
					foreach (var item in filterLines) 
					{
						if (_lstLines.Contains(item))
						{
							if (!isLastCombination)
							{
								item.TopLock = true;
								item.BAGroup = "G" + groupnumber;
							}

							_topLockedMainList.Add(item);
							_lstLines.Remove(item);
							bidAutoGroup.Lines.Add(item.LineNum);
							item.IsGrpColorOn = (_grpColor && !isLastCombination) ? 1 : 0;


						}

					}


					_grpColor = !_grpColor;
					if (!isLastCombination)
					{
						wBidStateContent.BidAuto.BAGroup.Add(bidAutoGroup);
					}
					//------------------------------------------------------------

				}
			}
		}

		/// <summary>
		/// Generate Filter list
		/// </summary>
		private void GenerateFilterList()
		{
			if (wBidStateContent.BidAuto != null &&
				wBidStateContent.BidAuto.BAFilter != null &&
				wBidStateContent.BidAuto.BAFilter.Any())
			{
				_lstFilter = new ObservableCollection<FilterHelper>();
				foreach (var item in wBidStateContent.BidAuto.BAFilter)
				{

					_lstFilter.Add(new FilterHelper
						{
							FilterName = item.Name,
							Priority = item.Priority

						});
				}

			}
		}

		#region  Filter  line property  for Bid Automator Logic

		private void CalculateAMPMFilterLineProperty(BidAutoItem item)
		{
			var amPmconstraints = (AMPMConstriants)item.BidAutoObject;
			foreach (var line in _lstLines)
			{
				bool status = false;

				if (amPmconstraints.AM)
				{
					status = line.AMPM == "AM";
				}

				if (!status && amPmconstraints.PM)
				{
					status = line.AMPM == " PM";
				}

				if (!status && amPmconstraints.MIX)
				{
					status = line.AMPM == "Mix";
				}

				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}

		}
		/// <summary>
		/// Calculate Days Of Week All Constraint LineProperty Logic
		/// </summary>
		/// <param name="item"></param>
		private void CalculateDaysOfWeekAllFilterLineProperty(BidAutoItem item)
		{
			//if the user has the DOW week set with Mo-Tu-We green, then we will find the lines that have Sun,Thu,Fri,Sat off, and then top lock those lines.

			var cxdays = (CxDays)item.BidAutoObject;
			var lstDays = new List<int> { 6, 0, 1, 2, 3, 4, 5 };

			if (cxdays.IsSun)
			{
				lstDays.Remove(6);
				//lstDays.Add(6);
			}
			if (cxdays.IsMon)
			{
				lstDays.Remove(0);
				//lstDays.Add(0);
			}

			if (cxdays.IsTue)
			{
				lstDays.Remove(1);
				//lstDays.Add(1);
			}

			if (cxdays.IsWed)
			{
				lstDays.Remove(2);
				//lstDays.Add(2);
			}

			if (cxdays.IsThu)
			{
				lstDays.Remove(3);
				//lstDays.Add(3);
			}

			if (cxdays.IsFri)
			{
				lstDays.Remove(4);
				//lstDays.Add(4);
			}

			if (cxdays.IsSat)
			{
				lstDays.Remove(5);
				//lstDays.Add(5);
			}

			// lstDays keeps all the workin days index

			foreach (var line in _lstLines)
			{
				bool status = true;

				if (line.BlankLine)
					status = true;
				else {
					foreach (var lstDay in lstDays) {
						status = line.DaysOfWeekWork [lstDay] == 0;
						if (!status)
							break;
					}
				}

				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}


		}

		private void CalculateDaysOfWeekSomeFilterLineProperty(BidAutoItem item)
		{


			var cx3Parameter = (Cx3Parameter)item.BidAutoObject;

			foreach (var line in _lstLines)
			{
				
				bool status = false;

				if (line.BlankLine)
					status = false;
				else {
					if (cx3Parameter.Type == (int)ConstraintType.LessThan) {
						if (line.DaysOfWeekWork [Convert.ToInt32 (cx3Parameter.ThirdcellValue)] < cx3Parameter.Value)
							status = true;
					} else if (cx3Parameter.Type == (int)ConstraintType.MoreThan) {
						if (line.DaysOfWeekWork [Convert.ToInt32 (cx3Parameter.ThirdcellValue)] > cx3Parameter.Value)
							status = true;
					} else if (cx3Parameter.Type == (int)ConstraintType.EqualTo) {
						if (line.DaysOfWeekWork [Convert.ToInt32 (cx3Parameter.ThirdcellValue)] == cx3Parameter.Value)
							status = true;
					}
				}
				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}


		}

		private void CalculateDeadHeadFirsLastFilterLineProperty(BidAutoItem item)
		{

			var cx3Parameter = (Cx3Parameter)item.BidAutoObject;

			foreach (var line in _lstLines)
			{
				bool status = false;


				if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.First).ToString())
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhFirstTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhFirstTotal > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.EqualTo)
					{
						if (line.DhFirstTotal == cx3Parameter.Value)
							status = true;
					}
				}
				else if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.Last).ToString())
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhLastTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhLastTotal > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.EqualTo)
					{
						if (line.DhLastTotal == cx3Parameter.Value)
							status = true;
					}
				}
				else if (cx3Parameter.ThirdcellValue == ((int)DeadheadType.Both).ToString())
				{
					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						if (line.DhLastTotal < cx3Parameter.Value && line.DhFirstTotal < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						if (line.DhLastTotal > cx3Parameter.Value && line.DhFirstTotal > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.Type == (int)ConstraintType.EqualTo)
					{
						if (line.DhLastTotal == cx3Parameter.Value && line.DhFirstTotal == cx3Parameter.Value)
							status = true;
					}
				}

				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}
		}

		private void CalculateEquipmentTypeFilterLineProperty(BidAutoItem item)
		{

			var cx3Parameter = (Cx3Parameter)item.BidAutoObject;
			foreach (var line in _lstLines)
			{
				bool status = false;
				if (cx3Parameter.Type == (int)ConstraintType.LessThan)
				{
					//if (cx3Parameter.ThirdcellValue == "300")
					//{
					//	if (line.LegsIn300 < cx3Parameter.Value)
					//		status = true;
					//}
					//else if (cx3Parameter.ThirdcellValue == "500")
					//{
					//	if (line.LegsIn500 < cx3Parameter.Value)
					//		status = true;
					//}
					////300 & 500
					//else if (cx3Parameter.ThirdcellValue == "35")
					//{
					//	if (line.LegsIn300 < cx3Parameter.Value && line.LegsIn500 < cx3Parameter.Value)
					//		status = true;
					//}
				    if (cx3Parameter.ThirdcellValue == "700")
					{
						if (line.LegsIn700 < cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "800")
					{
						if (line.LegsIn800 < cx3Parameter.Value)
							status = true;

					}
					else if (cx3Parameter.ThirdcellValue == "600")
					{
						if (line.LegsIn600 < cx3Parameter.Value)
							status = true;

					}

					else if (cx3Parameter.ThirdcellValue == "200")
					{
						if (line.LegsIn200 < cx3Parameter.Value)
							status = true;

					}
				}
				else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
				{
					//if (cx3Parameter.ThirdcellValue == "300")
					//{
					//	if (line.LegsIn300 > cx3Parameter.Value)
					//		status = true;
					//}
					//else if (cx3Parameter.ThirdcellValue == "500")
					//{
					//	if (line.LegsIn500 > cx3Parameter.Value)
					//		status = true;
					//}
					////300 & 500
					//else if (cx3Parameter.ThirdcellValue == "35")
					//{
					//	if (line.LegsIn300 > cx3Parameter.Value && line.LegsIn500 > cx3Parameter.Value)
					//		status = true;
					//}
					 if (cx3Parameter.ThirdcellValue == "700")
					{
						if (line.LegsIn700 > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "800")
					{
						if (line.LegsIn800 > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "600")
					{
						if (line.LegsIn600 > cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "200")
					{
						if (line.LegsIn200 > cx3Parameter.Value)
							status = true;
					}
				}
				else if (cx3Parameter.Type == (int)ConstraintType.EqualTo)
				{
					//if (cx3Parameter.ThirdcellValue == "300")
					//{
					//	if (line.LegsIn300 == cx3Parameter.Value)
					//		status = true;
					//}
					//else if (cx3Parameter.ThirdcellValue == "500")
					//{
					//	if (line.LegsIn500 == cx3Parameter.Value)
					//		status = true;
					//}
					//////300 & 500
					//else if (cx3Parameter.ThirdcellValue == "35")
					//{
					//	if (line.LegsIn300 == cx3Parameter.Value && line.LegsIn500 == cx3Parameter.Value)
					//		status = true;
					//}
					 if (cx3Parameter.ThirdcellValue == "700")
					{
						if (line.LegsIn700 == cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "800")
					{
						if (line.LegsIn800 == cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "600")
					{
						if (line.LegsIn600 == cx3Parameter.Value)
							status = true;
					}
					else if (cx3Parameter.ThirdcellValue == "200")
					{
						if (line.LegsIn200 == cx3Parameter.Value)
							status = true;
					}
				}
				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}

		}


		private void CalculateLineTypeFilterLineProperty(BidAutoItem item)
		{
			var cxLine = (CxLine)item.BidAutoObject;
			foreach (var line in _lstLines)
			{
				bool status = false;
				if (cxLine.Hard)
				{
					status = !line.ReserveLine && !line.BlankLine && line.Pairings.All(y => y.Substring(1, 1) != "R");

				}

				if (!status && cxLine.Reserve)
				{
					status = (line.ReserveLine && !line.BlankLine && line.Pairings.All(y => y.Substring(1, 1) != "R"));
				}
				if (!status && cxLine.Ready)
				{
					status = (!line.BlankLine && line.Pairings.Any(y => y.Substring(1, 1) == "R"));
				}
				if (!status && cxLine.Blank)
				{
					status = line.BlankLine;
				}

				if (!status && cxLine.International)
				{
					status = (!line.BlankLine && line.International);
				}

				if (!status && cxLine.NonConus)
				{
					status = (!line.BlankLine && line.NonConus);
				}

				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}
		}

		private void CalculateRestFilterLineProperty(BidAutoItem item)
		{

			var cx3Parameter = (Cx3Parameter)item.BidAutoObject;

			var restTimeinMinutes = cx3Parameter.Value * 60;
			foreach (var line in _lstLines)
			{
				bool status = false;
				if (cx3Parameter.ThirdcellValue == ((int)RestType.All).ToString())
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.All(x => x.RestMinutes < restTimeinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.All(x => x.RestMinutes > restTimeinMinutes);
					}
				}

				else if (cx3Parameter.ThirdcellValue == ((int)RestType.InDomicile).ToString())
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Where(x => !x.IsInTrip).All(x => x.RestMinutes < restTimeinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Where(x => !x.IsInTrip).All(x => x.RestMinutes > restTimeinMinutes);
					}
				}

				else if (cx3Parameter.ThirdcellValue == ((int)RestType.AwayDomicile).ToString())
				{

					if (cx3Parameter.Type == (int)ConstraintType.LessThan)
					{
						status = line.RestPeriods.Where(x => x.IsInTrip).All(x => x.RestMinutes < restTimeinMinutes);
					}
					else if (cx3Parameter.Type == (int)ConstraintType.MoreThan)
					{
						status = line.RestPeriods.Where(x => x.IsInTrip).All(x => x.RestMinutes > restTimeinMinutes);
					}
				}
				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}

		}

		private void CalculateTripBlockLengthFilterLineProperty(BidAutoItem item)
		{

			//if the user has the trip/block length  set with 1d,3d  green, then we will find the lines that have 2d,4d is zero, and then top lock those lines.

			var CxTripBlockLength = (CxTripBlockLength)item.BidAutoObject;
			List<int> lstTripblocks = new List<int>() { 1, 2, 3, 4 };

			if (CxTripBlockLength.Turns)
			{
				lstTripblocks.Remove(1);

			}
			if (CxTripBlockLength.Twoday)
			{
				lstTripblocks.Remove(2);
			}

			if (CxTripBlockLength.ThreeDay)
			{
				lstTripblocks.Remove(3);
			}

			if (CxTripBlockLength.FourDay)
			{
				lstTripblocks.Remove(4);

			}

			if (CxTripBlockLength.IsBlock)
			{
				//Block length

				foreach (var line in _lstLines)
				{
					bool status = true;
					if (line.BlankLine)
						status = false;
					else {

						foreach (var lstDay in lstTripblocks) {
							status = line.WorkBlockLengths [lstDay] == 0;
							if (!status)
								break;
						}
					}
					if (line.BAFilters == null)
					{
						line.BAFilters = new List<bool>();
					}
					line.BAFilters.Add(status);
				}
			}
			else
			{
				//Trip length
				foreach (var line in _lstLines)
				{
					bool status = true;
					if (line.BlankLine)
						status = false;
					else {
						foreach (var lstDay in lstTripblocks) {
							if (lstDay == 1)
								status = line.Trips1Day == 0;
							else if (lstDay == 2)
								status = line.Trips2Day == 0;
							else if (lstDay == 3)
								status = line.Trips3Day == 0;
							else if (lstDay == 4)
								status = line.Trips4Day == 0;
							if (!status)
								break;
						}
					}
					if (line.BAFilters == null)
					{
						line.BAFilters = new List<bool>();
					}
					line.BAFilters.Add(status);
				}
			}



		}

		private void CalculateStartDayofWeekFilterLineProperty(BidAutoItem item)
		{
			var cxdays = (CxDays)item.BidAutoObject;
			var lstDays = new List<int> { 6, 0, 1, 2, 3, 4, 5 };

			if (cxdays.IsSun)
			{
				lstDays.Remove(6);

			}
			else if (cxdays.IsMon)
			{
				lstDays.Remove(0);
			}

			else if (cxdays.IsTue)
			{
				lstDays.Remove(1);
			}

			else if (cxdays.IsWed)
			{
				lstDays.Remove(2);
			}

			else if (cxdays.IsThu)
			{
				lstDays.Remove(3);
			}

			else if (cxdays.IsFri)
			{
				lstDays.Remove(4);
			}

			else if (cxdays.IsSat)
			{
				lstDays.Remove(5);
			}




			foreach (var line in _lstLines)
			{
				bool status = false;

				if (line.StartDaysList != null)
				{
					foreach (var lstDay in lstDays)
					{
						status = line.StartDaysList[lstDay] == 0;
						if (!status)
							break;
					}
				}



				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);
			}
		}

		private void CalculateDaysOfMonthFilterLineProperty(BidAutoItem item)
		{
			var daysOfMonthState = (DaysOfMonthCx)item.BidAutoObject;
			List<DaysOfMonth> lstDaysOfMonth = WBidCollection.GetDaysOfMonthList();
			List<DateTime> workDays = null;
			List<DateTime> offDays = null;
			DaysOfMonth result;

			//Work days
			if (daysOfMonthState.WorkDays != null && daysOfMonthState.WorkDays.Count > 0)
			{
				workDays = new List<DateTime>();
				foreach (int id in daysOfMonthState.WorkDays)
				{
					result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
					if (result != null)
						workDays.Add(result.Date);

				}
			}

			//Off Days
			if (daysOfMonthState.OFFDays != null && daysOfMonthState.OFFDays.Count > 0)
			{
				offDays = new List<DateTime>();
				foreach (int id in daysOfMonthState.OFFDays)
				{
					result = lstDaysOfMonth.FirstOrDefault(x => x.Id == id);
					if (result != null)
						offDays.Add(result.Date);

				}
			}





			foreach (var line in _lstLines)
			{

				bool status = true;

				//working calculation
				if (daysOfMonthState.WorkDays != null && daysOfMonthState.WorkDays.Count > 0)
				{
					status = false;
					foreach (DateTime dateTime in workDays)
					{
						var dayObject = line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == dateTime && x.Working);
						status = (dayObject != null);

						if (!status)
							break;
					}

				}

				if (status && daysOfMonthState.OFFDays != null && daysOfMonthState.OFFDays.Count > 0)
				{
					status = false;
					foreach (DateTime dateTime in offDays)
					{
						var dayObject = line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == dateTime);
						status = (dayObject == null);

						if (!status)
							break;
					}
				}

				if (line.BAFilters == null)
				{
					line.BAFilters = new List<bool>();
				}
				line.BAFilters.Add(status);


			}






		}

		private void CalculateCommutableLineFilterLineProperty(BidAutoItem item)
		{
			//Sate object for Commutable line filter
			var ftCommutableLine = (FtCommutableLine)item.BidAutoObject;

			//Here we are keeping all commute times for the bid
			if (wBidStateContent.BidAuto.DailyCommuteTimes != null)
			{

				foreach (var line in _lstLines)
				{

					bool status = true;
					bool isCommuteFrontEnd = false;
					bool isCommuteBackEnd = false;

					if (line.WorkBlockList != null)
					{
						isCommuteFrontEnd = false;
						isCommuteBackEnd = false;

						foreach (WorkBlockDetails workBlock in line.WorkBlockList)
						{
							//Checking the  corresponding Commute based on Workblock Start time
							var commutTimes = wBidStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.StartDateTime.Date);

							if (commutTimes != null && ftCommutableLine.ToWork)
							{
								if (commutTimes.EarliestArrivel != DateTime.MinValue) {
                                    isCommuteFrontEnd = (commutTimes.EarliestArrivel.AddMinutes (ftCommutableLine.CheckInTime)) <= workBlock.StartDateTime.AddMinutes(workBlock.BriefTime);
									if (!isCommuteFrontEnd) {
										status = false;
										break;
									}
								}
							}


							//Checking the  corresponding Commute based on Workblock End time
							//commutTimes = wBidStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDateTime.Date);
							// using EndDate to account for irregular datetimes in company time keeping method.
							commutTimes = wBidStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDate.Date);

							if (commutTimes != null && ftCommutableLine.ToHome)
							{
								if (commutTimes.LatestDeparture != DateTime.MinValue) {
									isCommuteBackEnd = commutTimes.LatestDeparture.AddMinutes (-ftCommutableLine.BaseTime) >= workBlock.EndDateTime;
									if (!isCommuteBackEnd) {
										status = false;
										break;
									}
								}
							}

							//“No nights in middle” is GREEN 
							//-----------------------------------------------------------------------------------------------
							if (ftCommutableLine.NoNights)
							{
								//if ((ftCommutableLine.ToWork && isCommuteFrontEnd) || (ftCommutableLine.ToHome && isCommuteBackEnd))
								if (workBlock.BackToBackCount > 0)  // if BackToBackCount is > 0 then there are nights in the middle
								{
									status = false;
									break;
								}


							}
							//-----------------------------------------------------------------------------------------------
							//“no nights in middle” is orange
							// Frank Add:  no calculations are needed if the "no nights in middle" is orange
							//else
							//{
							//    if (ftCommutableLine.ToHome && ftCommutableLine.ToWork)
							//    {
							//        if (isCommuteFrontEnd || isCommuteBackEnd || workBlock.BackToBackCount > 0)
							//        {
							//            status = true;
							//            break;
							//        }
							//    }
							//    else if (ftCommutableLine.ToHome && (isCommuteBackEnd || workBlock.BackToBackCount > 0))
							//    {
							//        status = true;
							//        break;
							//    }

							//    else if (ftCommutableLine.ToWork && (isCommuteFrontEnd || workBlock.BackToBackCount > 0))
							//    {
							//        status = true;
							//        break;
							//    }

							//}
							//-----------------------------------------------------------------------------------------------
						}

					}



					if (line.BAFilters == null)
					{
						line.BAFilters = new List<bool>();
					}
					line.BAFilters.Add(status);


				}
			}






		}

		private void CalculateOvernightFilterLineProperty(BidAutoItem item)
		{

			var bulkOvernightCityCx = (BulkOvernightCityCx)item.BidAutoObject;

			foreach (var line in _lstLines)
			{
				bool status = false;
				if (bulkOvernightCityCx.OverNightYes != null && bulkOvernightCityCx.OverNightYes.Count > 0)
				{
					List<string> lstYesCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightYes.Contains(x.Id)).Select(y => y.Name).ToList();
					//if the lines contain all the cities in the "Overnight City Yes" ,then we need to set the status to true. If it is true , we need to check the overnightCity No condition.
					status = (line.OvernightCities.Intersect(lstYesCityNames).Count() == lstYesCityNames.Count);
				}

				if (status)
				{
					if (bulkOvernightCityCx.OverNightNo != null && bulkOvernightCityCx.OverNightNo.Count > 0)
					{
						//if the line overnight cities contains any cities in "overnight No" cities, we need to set the bit to false
						List<string> lstNoCityNames = GlobalSettings.WBidINIContent.Cities.Where(x => bulkOvernightCityCx.OverNightNo.Contains(x.Id)).Select(y => y.Name).ToList();
						status = !line.OvernightCities.Intersect(lstNoCityNames).Any();
					}
				}

				line.BAFilters = line.BAFilters ?? new List<bool>();

				line.BAFilters.Add(status);
			}
		}
		#endregion

		#region Sort Logic
		private ObservableCollection<Line> SortingCalculationLogic(ObservableCollection<Line> lstFilter)
		{
			try
			{


				string sortColumn = wBidStateContent.BidAuto.BASort.SortColumn;
				switch (sortColumn)
				{

				case "LinePay":
					//lstFilter.ToList().ForEach(x => x.Points = x.Tfp == 0 ? 0 : Math.Round((x.Tfp + x.WeightPoints.Total()), 2));
					foreach(var x in lstFilter)
					{
						x.Points = x.Tfp == 0 ? 0 : Math.Round((x.Tfp + x.WeightPoints.Total()), 2);
					}
					lstFilter = new ObservableCollection<Line>(lstFilter.ToList().OrderByDescending(x => x.Points));

					break;
				case "PayPerDay":
					//lstFilter.ToList().ForEach(x => x.Points = x.DaysWorkInLine == 0 ? 0 : Math.Round((14 * x.TfpInLine / x.DaysWorkInLine) + x.WeightPoints.Total(), 2));
					foreach(var x in lstFilter)
					{
						x.Points = x.DaysWorkInLine == 0 ? 0 : Math.Round((14 * x.TfpInLine / x.DaysWorkInLine) + x.WeightPoints.Total(), 2);
					}
					lstFilter = new ObservableCollection<Line>(lstFilter.ToList().OrderByDescending(x => x.Points));

					break;
				case "PayPerDutyHour":
					//lstFilter.ToList().ForEach(x => x.Points = x.DutyHrsInLine == null || x.DutyHrsInLine == "0:0" ? 0 : Math.Round((120 * x.TfpInLine / WBidHelper.ConvertHhhColonMmToFractionalHours(x.DutyHrsInLine) + x.WeightPoints.Total()), 2));
					foreach(var x in lstFilter)
					{
						x.Points = x.DutyHrsInLine == null || x.DutyHrsInLine == "0:0" || x.DutyHrsInLine == "0:00" || x.DutyHrsInLine == "" || x.DutyHrsInLine ==null? 0 : Math.Round((120 * x.TfpInLine / Helper.ConvertHhhColonMmToFractionalHours(x.DutyHrsInLine) + x.WeightPoints.Total()), 2);
					}
					lstFilter = new ObservableCollection<Line>(lstFilter.ToList().OrderByDescending(x => x.Points));

					break;
				case "PayPerFlightHour":
					//lstFilter.ToList().ForEach(x => x.Points = x.BlkHrsInLine == null || x.BlkHrsInLine == "0:0" ? 0 : Math.Round((80 * x.TfpInLine / WBidHelper.ConvertHhhColonMmToFractionalHours(x.BlkHrsInLine) + x.WeightPoints.Total()), 2));
					foreach(var x in lstFilter)
					{
						x.Points = x.BlkHrsInLine == null || x.BlkHrsInLine == "0:0" || x.BlkHrsInLine == "00:00" || x.BlkHrsInLine == ""? 0 : Math.Round((80 * x.TfpInLine / Helper.ConvertHhhColonMmToFractionalHours(x.BlkHrsInLine) + x.WeightPoints.Total()), 2);
					}
					lstFilter = new ObservableCollection<Line>(lstFilter.ToList().OrderByDescending(x => x.Points));
					//Lines.ToList().ForEach(x => x.Points = x.BlkHrsInLine == null || x.BlkHrsInLine == "0:0" ? 0 : Math.Round((80 * x.TfpInLine / WBidHelper.ConvertHhhColonMmToFractionalHours(x.BlkHrsInLine) + x.WeightPoints.Total()), 2));
					break;
				case "PayPerTimeAway":
					//lstFilter.ToList().ForEach(x => x.Points = x.TafbInLine == null || x.TafbInLine == "0:0" ? 0 : Math.Round((300 * x.TfpInLine / WBidHelper.ConvertHhhColonMmToFractionalHours(x.TafbInLine) + x.WeightPoints.Total()), 2));
					foreach(var x in lstFilter)
					{
						x.Points =  x.TafbInLine == null || x.TafbInLine == "0:0" || x.TafbInLine == "00:00" || x.TafbInLine == "0:0" || x.TafbInLine == "0:00"? 0 : Math.Round((300 * x.TfpInLine / Helper.ConvertHhhColonMmToFractionalHours(x.TafbInLine) + x.WeightPoints.Total()), 2);
					}
					lstFilter = new ObservableCollection<Line>(lstFilter.ToList().OrderByDescending(x => x.Points));
					// Lines.ToList().ForEach(x => x.Points = x.TafbInLine == null || x.TafbInLine == "0:0" ? 0 : Math.Round((300 * x.TfpInLine / WBidHelper.ConvertHhhColonMmToFractionalHours(x.TafbInLine) + x.WeightPoints.Total()), 2));
					break;

				case "BlockSort":
					lstFilter = BlockSortPointCalulation(lstFilter);
					lstFilter = BlockSortOrder(lstFilter);
					break;


				}
				//lstFilter = new ObservableCollection<Line>(lstFilter.ToList().OrderByDescending(x => x.Points));


			}
			catch(Exception ex)
			{

			}

			return lstFilter;


		}

		/// <summary>
		/// Block Sort Logic
		/// </summary>
		/// <param name="lstFilter"></param>
		private ObservableCollection<Line> BlockSortPointCalulation(ObservableCollection<Line> lstFilter)
		{


			//lstFilter.ToList().ForEach(x => x.Points = Math.Round(x.WeightPoints.Total(), 1));
			foreach (var x in lstFilter) 
			{
				x.Points = Math.Round (x.WeightPoints.Total (), 1);
			}

			return lstFilter;

		}

		private ObservableCollection<Line> BlockSortOrder(ObservableCollection<Line> lstFilter)
		{


			if (wBidStateContent.BidAuto.BASort != null && wBidStateContent.BidAuto.BASort.BlokSort != null)
			{
				var lstBlockSortColumns = new ObservableCollection<BlockSort>();

				foreach (string sortKey in wBidStateContent.BidAuto.BASort.BlokSort)
				{
					if (sortKey != string.Empty)
					{
						lstBlockSortColumns.Add(WBidCollection.GetBlockSortListData().FirstOrDefault(y => y.Id.ToString() == sortKey));
					}
				}

				lstBlockSortColumns.ToList().RemoveAll(x => x == null);
				bool isFirstcolumn = true;

				if (lstBlockSortColumns.Count != 0)
				{
					//Building expression tree to dynamically sort a dictionary of dictionaries in LINQ
					var iQueryable = lstFilter.AsQueryable();
					IOrderedQueryable<Line> iOrderedQueryable = null;

					foreach (BlockSort blockSort in lstBlockSortColumns)
					{

                        switch (blockSort.Name)
                        {
                            case "AMs then PMs":

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

                                iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderBy(x => x.BlankLine).ThenBy(x => x.Weekend) : iOrderedQueryable.ThenBy(x => x.BlankLine).ThenBy(x => x.Weekend);
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
                            case "Line Rig":
                                iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.LineRig) : iOrderedQueryable.ThenByDescending(x => x.LineRig);
                                break;
                            case "Vac+LG":
                                iOrderedQueryable = (isFirstcolumn) ? iQueryable.OrderByDescending(x => x.VacPlusRig) : iOrderedQueryable.ThenByDescending(x => x.VacPlusRig);
                                break;

                        }

						isFirstcolumn = false;
					}
					if (iOrderedQueryable != null)
					{
						iOrderedQueryable = iOrderedQueryable.ThenBy(x => x.LineNum);

						lstFilter = new ObservableCollection<Line>(iOrderedQueryable.ToList());
					}
				}

			}

			return lstFilter;

		}
		#endregion

		#endregion
	}




	#region Utility Class
	public class FilterHelper
	{
		public string FilterName { get; set; }

		public int Priority { get; set; }

		public bool State { get; set; }

	}
	#endregion
}

