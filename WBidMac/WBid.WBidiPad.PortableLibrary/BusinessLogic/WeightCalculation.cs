#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Model.State.Weights;
using WBid.WBidiPad.PortableLibrary.Utility;
#endregion

namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
{
	public class WeightCalculation
	{
		#region Public Methods
		public void ApplyAllWeights()
		{
			WBidState wBidStateContent =
				GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
					x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);


			int leastDaysOff = requiredLines.Where(x=>!x.ReserveLine).Min(x => x.DaysOff);

			List<DateHelper> lstDatehelper = GenarateCalender();

			//---------------------------------------------
			//Days of Month
			//---------------------------------------------
			List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();
			var weightsApplied = new List<DaysOfMonthHelper>();

			foreach (Wt wt in wBidStateContent.Weights.SDO.Weights)
			{
				DayOfMonth dayOfMonth = lstDaysOfMonth.FirstOrDefault(x => x.Id == wt.Key);
				if (dayOfMonth != null)
					weightsApplied.Add(new DaysOfMonthHelper
						{
							Date = dayOfMonth.Date,
							DateId = dayOfMonth.Id,
							Weight = wt.Value
						});
			}

			//if (wBidStateContent.CxWtState.Commute == null)
			//	wBidStateContent.CxWtState.Commute = new StateStatus { Cx = false, Wt = false };

			//if (wBidStateContent.CxWtState.Commute.Wt)
			//{
			//	CalculateLineProperties lineproprty=new CalculateLineProperties();
			//	lineproprty.CalculateCommuteLineProperties (wBidStateContent);
			//}
			foreach (Line line in requiredLines)
			{
				//---------------------------------------------------------------------------------------------
				//Aircraft Changes
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.ACChg.Wt)
				{
					AircraftChangeWeightCalcaultion(wBidStateContent.Weights.AirCraftChanges, line);
				}
				else
					line.WeightPoints.AcftChanges = 0;

				//---------------------------------------------------------------------------------------------
				//AM/PM
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.AMPM.Wt)
				{
					AMPMWeightCalculation(wBidStateContent.Weights.AM_PM.lstParameters, line);
				}
				else
					line.WeightPoints.AmPmNte = 0;
				//---------------------------------------------------------------------------------------------
				//BlockOfDaysOffConstraint DaysOff
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.BDO.Wt)
				{
					BlockOFFDaysOfWeightCalculations(wBidStateContent.Weights.BDO.lstParameters, line);
				}
				else
					line.WeightPoints.BlkDaysOff = 0;


				//---------------------------------------------------------------------------------------------
				//  Cmut DHs
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.DHD.Wt)
				{
					CommutableDeadhead(wBidStateContent.Weights.DHD.lstParameters, line);
				}
				else
					line.WeightPoints.CmtDhds = 0;
				//---------------------------------------------------------------------------------------------
				//  Commutable Lines
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.CL.Wt)
				{
					CommutableLineWeight(wBidStateContent.Weights.CL, line);
				}
				else
					line.WeightPoints.CmtLines = 0;

				// Commutable Line  weight Calculation
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.CLAuto == null)
					wBidStateContent.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
				if (wBidStateContent.CxWtState.CLAuto.Wt)
				{
					if (wBidStateContent.Weights.CLAuto != null) {
						CommutableLineAutoWeight (wBidStateContent.Weights.CLAuto, line);
					}
				}
				else
				{
					line.WeightPoints.CmtLinesAuto = 0.0m;
				}
				//---------------------------------------------------------------------------------------------

				//---------------------------------------------------------------------------------------------

				// Commuttability  weight Calculation
				//---------------------------------------------------------------------------------------------

				if (wBidStateContent.CxWtState.Commute.Wt)
				{

					CommuttabilityWeight(wBidStateContent.Weights.Commute, line);
				}
				else
				{
					line.WeightPoints.Commute = 0.0m;
				}
				//---------------------------------------------------------------------------------------------
				//  Days of the Month
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.SDO.Wt)
				{
					DayOftheMonthWeight(weightsApplied, wBidStateContent.Weights.SDO.isWork, line);
				}
				else
				{
					line.WeightPoints.DaysOfMonthOff = 0;
				}

				//---------------------------------------------------------------------------------------------
				//Days of week
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.DOW.Wt)
				{
					DaysOfWeekWeightCalculation(wBidStateContent.Weights.DOW, line);
				}
				else
					line.WeightPoints.DaysOfWeekOff = 0;


				//---------------------------------------------------------------------------------------------
				//  DH - first - last
				//---------------------------------------------------------------------------------------------

				if (wBidStateContent.CxWtState.DHDFoL.Wt)
				{
					DeadheadFisrtLastWeightCalculation(wBidStateContent.Weights.DHDFoL.lstParameters, line);
				}
				else
					line.WeightPoints.DeadHeadFoL = 0;
				//---------------------------------------------------------------------------------------------
				//Dutyperiod  DaysOff
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.DP.Wt)
				{
					DutyPeriodWeightCalculation(wBidStateContent.Weights.DP.lstParameters, line);
				}
				else
					line.WeightPoints.DutyPeriod = 0;


				//---------------------------------------------------------------------------------------------
				//  Equipment
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.EQUIP.Wt)
				{
					EquipmentTypeWeights(wBidStateContent.Weights.EQUIP.lstParameters, line);
				}
				else
					line.WeightPoints.EquipType = 0;

				//---------------------------------------------------------------------------------------------
				//  ETOPS
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.ETOPS.Wt)
				{
					ETOPSWeights(wBidStateContent.Weights.ETOPS.lstParameters, line);
				}
				else
					line.WeightPoints.ETOPS = 0;

				//---------------------------------------------------------------------------------------------
				//  ETOPS reserve
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.ETOPSRes.Wt)
				{
					ETOPSResWeights(wBidStateContent.Weights.ETOPSRes.lstParameters, line);
				}
				else
					line.WeightPoints.ETOPSRes = 0;

				//---------------------------------------------------------------------------------------------
				// Flight Time
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.FLTMIN.Wt)
				{
					FlightTimeWeightCalculation(wBidStateContent.Weights.FLTMIN.lstParameters, line);
				}
				else
					line.WeightPoints.BlockTime = 0;
				//---------------------------------------------------------------------------------------------
				// GroundTime
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.GRD.Wt && !line.ReserveLine)
				{
					GroundTimeWeightCalculations(wBidStateContent.Weights.GRD.lstParameters, line);
				}
				else
					line.WeightPoints.GrndTime = 0;

				//---------------------------------------------------------------------------------------------
				//International-Nonconus
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.InterConus.Wt && !line.ReserveLine)
				{
					InternationalNonConusWeightCalculations(wBidStateContent.Weights.InterConus.lstParameters, line);
				}
				else
					line.WeightPoints.InternationalConus = 0;



				//---------------------------------------------------------------------------------------------
				//largest Block of days off
				//---------------------------------------------------------------------------------------------

				if (wBidStateContent.CxWtState.LrgBlkDaysOff.Wt)
				{
					LargestBlockOfDaysoff(wBidStateContent.Weights.LrgBlkDayOff, line);
				}
				else
					line.WeightPoints.LegsPerDutPd = 0;


				//---------------------------------------------------------------------------------------------
				// Legs per dutyperiod
				//---------------------------------------------------------------------------------------------

				if (wBidStateContent.CxWtState.LEGS.Wt)
				{
					LegsPerDutyPeriodWeightCalculation(wBidStateContent.Weights.LEGS.lstParameters, line);
				}
				else
					line.WeightPoints.LegsPerDutPd = 0;
				//---------------------------------------------------------------------------------------------
				// Legs per pairing
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.LegsPerPairing.Wt)
				{
					LegsPerPairingWeightCalculation(wBidStateContent.Weights.WtLegsPerPairing.lstParameters, line);
				}
				else
					line.WeightPoints.LegsPerTrip = 0;

				//---------------------------------------------------------------------------------------------
				//NormalizeDaysOff
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.NormalizeDays.Wt)
				{
					NormalizeDaysOff(leastDaysOff, wBidStateContent.Weights.NormalizeDaysOff.Weight, line);
				}
				else
					line.WeightPoints.NumDaysOff = 0;

				//---------------------------------------------------------------------------------------------
				// Number of days Off
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.NODO.Wt)
				{
					NumberOfDaysOff(wBidStateContent.Weights.NODO.lstParameters, line);
				}
				else
					line.WeightPoints.NumDaysOff = 0;


				//---------------------------------------------------------------------------------------------
				// Overnight Cities
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.RON.Wt)
				{
					OvernightCitiesWeightCalculations(wBidStateContent.Weights.RON.lstParameters, line);
				}
				else
					line.WeightPoints.OvernightCities = 0;
				//---------------------------------------------------------------------------------------------
				// Cities legs
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.CitiesLegs == null)
					wBidStateContent.CxWtState.CitiesLegs = new StateStatus { Cx = false, Wt = false };
				if (wBidStateContent.CxWtState.CitiesLegs.Wt)
				{
					CitiesLegsWeightCalculations(wBidStateContent.Weights.CitiesLegs.lstParameters, line);
				}
				else
					line.WeightPoints.CitiesLegs = 0;
				//---------------------------------------------------------------------------------------------
				//---------------------------------------------------------------------------------------------
				// Overnight Cities     Bulk
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.BulkOC.Wt)
				{
					OvernightCityBulkWeight(wBidStateContent.Weights.OvernightCitybulk, line);
				}
				else
					line.WeightPoints.OvernightCityBulk = 0;

				//---------------------------------------------------------------------------------------------
				// Partial days off After
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.PDAfter.Wt)
				{
					PartialDaysAfterWeightCalculation(wBidStateContent.Weights.PDAfter.lstParameters, line,
						lstDatehelper);
				}
				else
					line.WeightPoints.PartialDaysAfter = 0;

				//---------------------------------------------------------------------------------------------
				// Partial days off Before
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.PDBefore.Wt)
				{
					PartialDaysBeforeWeightCalculation(wBidStateContent.Weights.PDBefore.lstParameters, line,
						lstDatehelper);
				}
				else
					line.WeightPoints.PartialDaysBefore = 0;

				//---------------------------------------------------------------------------------------------
				// Position
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.Position.Wt)
				{
					PositionWeightCalculation(wBidStateContent.Weights.POS.lstParameters, line);
				}
				else
					line.WeightPoints.Position = 0;
				//---------------------------------------------------------------------------------------------
				// Start day of week
				//---------------------------------------------------------------------------------------------

				if (wBidStateContent.CxWtState.SDOW.Wt)
				{
					StartDateOftheWeekWeight(wBidStateContent.Weights.SDOW.lstParameters, line);
				}
				else
					line.WeightPoints.StartDow = 0;

				//  ---------------------------------------------------------------------------------------------
				//   Rest
				//  ---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.Rest.Wt)
				{
					RestWeight(wBidStateContent.Weights.WtRest.lstParameters, line);

				}
				else
				{
					line.WeightPoints.Rest = 0.0m;
				}


				//---------------------------------------------------------------------------------------------
				// Timeaway from base
				//---------------------------------------------------------------------------------------------

				if (wBidStateContent.CxWtState.PerDiem.Wt)
				{
					TimeAwayFromBaseWeight(wBidStateContent.Weights.PerDiem, line);
				}
				else
					line.WeightPoints.TimeAwayFromBase = 0;

				//---------------------------------------------------------------------------------------------
				// Trip Length
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.TL.Wt)
				{
					TripLengthWeight(wBidStateContent.Weights.TL.lstParameters, line);
				}
				else
					line.WeightPoints.TripLength = 0;

				//---------------------------------------------------------------------------------------------
				// Work Block length
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.WB.Wt)
				{
					WorkBlockLengthWeight(wBidStateContent.Weights.WB.lstParameters, line);
				}
				else
					line.WeightPoints.WorkBlklength = 0;

				//---------------------------------------------------------------------------------------------
				// Work Days
				//---------------------------------------------------------------------------------------------
				if (wBidStateContent.CxWtState.WorkDay.Wt)
				{
					WorkDaysWeight(wBidStateContent.Weights.WorkDays.lstParameters, line);
				}
				else
					line.WeightPoints.WorkDays = 0;


				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void ClearWeights()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.Reset();
				line.TotWeight = 0.0m;
			}

			WBidState currentState =
				GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
					x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			CxWtState states = currentState.CxWtState;
			states.ACChg.Wt = false;
			states.AMPM.Wt = false;
			states.BDO.Wt = false;
			states.CL.Wt = false;
			states.CLAuto.Wt = false;
			states.DHD.Wt = false;
			states.DOW.Wt = false;
			states.DHDFoL.Wt = false;
			states.DP.Wt = false;
			states.EQUIP.Wt = false;
			states.ETOPS.Wt = false;
			states.ETOPSRes.Wt = false;
			states.FLTMIN.Wt = false;
			states.GRD.Wt = false;
			states.InterConus.Wt = false;
			states.LrgBlkDaysOff.Wt = false;
			states.LEGS.Wt = false;
			states.LegsPerPairing.Wt = false;
			states.MP.Wt = false;
			states.No3on3off.Wt = false;
			states.NODO.Wt = false;
			states.NOL.Wt = false;
			states.PerDiem.Wt = false;
			states.PDAfter.Wt = false;
			states.PDBefore.Wt = false;
			states.Position.Wt = false;
			states.Rest.Wt = false;
			states.RON.Wt = false;
			states.SDO.Wt = false;
			states.SDOW.Wt = false;
			states.TL.Wt = false;
			states.WB.Wt = false;
			states.WorkDay.Wt = false;
			states.NormalizeDays.Wt = false;
			states.BulkOC.Wt = false;
			states.CitiesLegs.Wt = false;
			states.Commute.Wt = false;
			//Weights
			//----------------------------------------

			var oldclDeafult = currentState.Weights.CL.DefaultTimes;
			//----------------------------------------
			Weights weight = new Weights()
			{
				AirCraftChanges = new Wt3Parameter { SecondlValue = 1, ThrirdCellValue = 1, Weight = 0 },
				//AMM,PM, Night
				AM_PM = new Wt2Parameters { Type = 1, Weight = 0, lstParameters = new List<Wt2Parameter>() },
				LrgBlkDayOff = new Wt2Parameter { Weight = 0 },

				BDO = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0
						,
					lstParameters = new List<Wt3Parameter>()
				},
				DHD = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				//Commutable Line
				CL = new WtCommutableLine()
				{
					TimesList = new List<Times>()
					{
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
					},
					DefaultTimes = new List<Times>()
					{
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
						new Times(){Checkin=0,BackToBase=0},
					},
					BothEnds = 0,
					InDomicile = 0,
					Type = 1
						//1.  All 2. 

				},
				CLAuto=new WtCommutableLineAuto(){ToHome=true,Weight=0},
				Commute =  new Commutability {SecondcellValue = (int)CommutabilitySecondCell.NoMiddle, ThirdcellValue = (int)CommutabilityThirdCell.Overall, Type = (int)ConstraintType.MoreThan, Value = 100 ,Weight=0},

				SDO = new DaysOfMonthWt()
				{
					isWork = false,
					Weights = new List<Wt>()
				},

				DOW = new WtDaysOfWeek()
				{
					lstWeight = new List<Wt>() { new Wt() { Key = 0, Value = 0 } },
					IsOff = true

				},
				DP = new Wt3Parameters()
				{
					SecondlValue = 1,
					ThrirdCellValue = 300,
					Weight = 0
						,
					lstParameters = new List<Wt3Parameter>()
				},

				EQUIP = new Wt3Parameters
				{
					SecondlValue = 700,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				ETOPS = new Wt1Parameters
				{
					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				},

				ETOPSRes = new Wt1Parameters
				{

					Weight = 0,
					lstParameters = new List<Wt1Parameter>()
				},
				FLTMIN = new Wt3Parameters
				{
					SecondlValue = 0,
					ThrirdCellValue = 20,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},

				GRD = new Wt3Parameters()
				{
					SecondlValue = 0,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				InterConus = new Wt2Parameters()
				{
					Type = -1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				LEGS = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},
				WtLegsPerPairing = new Wt3Parameters()
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},

				NODO = new Wt2Parameters
				{
					Type = 9,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				RON = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				CitiesLegs = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},

				SDOW = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				WtRest = new Wt4Parameters
				{
					FirstValue = 1,
					SecondlValue = 480,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt4Parameter>()
				},
				PerDiem = new Wt2Parameter
				{
					Type = 100,
					Weight = 0

				},
				TL = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				WB = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				POS = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				DHDFoL = new Wt2Parameters
				{
					Type = 1,
					Weight = 0,
					lstParameters = new List<Wt2Parameter>()
				},
				WorkDays = new Wt3Parameters
				{
					SecondlValue = 1,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt3Parameter>()
				},

				PDAfter = new Wt4Parameters
				{
					FirstValue = 1,
					SecondlValue = 180,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt4Parameter>()
				},
				PDBefore = new Wt4Parameters
				{
					FirstValue = 1,
					SecondlValue = 180,
					ThrirdCellValue = 1,
					Weight = 0,
					lstParameters = new List<Wt4Parameter>()
				},

				NormalizeDaysOff = new Wt2Parameter { Type = 1, Weight = 0 } ,

				OvernightCitybulk=new List<Wt2Parameter>()

			};

			currentState.Weights = weight;

		}

		#endregion

		#region Private Methods

		#region Apply Weights

		/// <summary>
		///     Apply Aircraft Change
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyAirCraftChangeWeight(Wt3Parameter wt3Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				AircraftChangeWeightCalcaultion(wt3Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply AM/PM Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyAMPMWeight(List<Wt2Parameter> wt2Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				AMPMWeightCalculation(wt2Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Block off Days Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyBlockOFFDaysOfWeight(List<Wt3Parameter> wt3Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				BlockOFFDaysOfWeightCalculations(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply CommutableDeadhead weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyCommutableDeadhead(List<Wt3Parameter> wt3Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				CommutableDeadhead(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Commutable Line weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyCommutableLine(WtCommutableLine wtCommutableLine)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				CommutableLineWeight(wtCommutableLine, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}
		/// <summary>
		/// Apply Commutable Line weight
		/// </summary>
		/// <param name="wtCommutableLine"></param>
		public void ApplyCommutableLineAuto(WtCommutableLineAuto wtCommutableLineAuto)
		{

			IEnumerable<Line> requiredLines = GlobalSettings.Lines .Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				CommutableLineAutoWeight(wtCommutableLineAuto, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}
		public void RemoveCommutableLineAutoWeight()
		{
			
            WBidState currentState =
                GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
                    x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.CmtLinesAuto != 0);
            foreach (var line in GlobalSettings.Lines)
            {
                if (requiredLines.Contains(line))
                {
                    line.WeightPoints.CmtLinesAuto = 0;
                    line.Points = line.WeightPoints.Total();
                    line.TotWeight = line.Points;
                }
                if (currentState.CxWtState.CLAuto.Cx == false && !(currentState.SortDetails.BlokSort.Contains("33") || currentState.SortDetails.BlokSort.Contains("34") || currentState.SortDetails.BlokSort.Contains("35")))
                {
                    line.CommutableBacks = 0;
                    line.commutableFronts = 0;
                    line.CommutabilityFront = 0;
                    line.CommutabilityBack = 0;
                    line.CommutabilityOverall = 0;

                }
            }
		}

		public void ApplyCommuttabilityWeight(Commutability commutability)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines .Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				CommuttabilityWeight(commutability, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}
		public void RemoveCommutabilityWeight()
		{
			WBidState currentState =
				GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
					x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.Commute != 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.Commute = 0;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;

				if (currentState.CxWtState.Commute.Cx == false)
				{
					line.CommutableBacks = 0;
					line.commutableFronts = 0;
					line.CommutabilityFront = 0;
					line.CommutabilityBack = 0;
					line.CommutabilityOverall = 0;
				}

			}
		}
		/// <summary>
		///     Apply DaysOfMonth Weight
		/// </summary>
		/// <param name="daysOfMonthWt"></param>
		public void ApplyDaysOfMonthWeight(DaysOfMonthWt daysOfMonthWt)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);

			List<DayOfMonth> lstDaysOfMonth = ConstraintBL.GetDaysOfMonthList();
			DayOfMonth dayOfMonth;
			var weightsApplied = new List<DaysOfMonthHelper>();

			foreach (Wt wt in daysOfMonthWt.Weights)
			{
				dayOfMonth = lstDaysOfMonth.FirstOrDefault(x => x.Id == wt.Key);
				if (dayOfMonth != null)
					weightsApplied.Add(new DaysOfMonthHelper
						{
							Date = dayOfMonth.Date,
							DateId = dayOfMonth.Id,
							Weight = wt.Value
						});
			}


			foreach (Line line in requiredLines)
			{
				DayOftheMonthWeight(weightsApplied, daysOfMonthWt.isWork, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply DaysOfWeek Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyDaysOfWeekWeight(WtDaysOfWeek daysofWeek)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				DaysOfWeekWeightCalculation(daysofWeek, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply DeadheadFisrtLast Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyDeadheadFisrtLastWeight(List<Wt2Parameter> wt2Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				DeadheadFisrtLastWeightCalculation(wt2Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Duty Period Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyDutyPeriodWeight(List<Wt3Parameter> wt3Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				DutyPeriodWeightCalculation(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply EquipmentType Weights
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyEquipmentTypeWeights(List<Wt3Parameter> wt3Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				EquipmentTypeWeights(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}
		/// <summary>
		///     Apply ETOPS Weights
		/// </summary>
		/// <param name="lstWt1Parameter"></param>
		public void ApplyETOPSWeights(List<Wt1Parameter> wt1Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				ETOPSWeights(wt1Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply ETOPS Reserve Weights
		/// </summary>
		/// <param name="lstWt1Parameter"></param>
		public void ApplyETOPSResWeights(List<Wt1Parameter> wt1Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				ETOPSResWeights(wt1Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}


		/// <summary>
		///     Apply Flight Time Weights
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyFlightTimeWeights(List<Wt3Parameter> wt3Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				FlightTimeWeightCalculation(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Ground Time Weights
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyGroundTimeWeight(List<Wt3Parameter> wt3Parameters)
		{
			List<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine && !x.ReserveLine).ToList();
			foreach (Line line in requiredLines)
			{
				GroundTimeWeightCalculations(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}


		/// <summary>
		///     International NonConus  weight
		/// </summary>
		/// <param name="lstWt2Parameters"></param>
		public void ApplyInternationalNonConusWeight(List<Wt2Parameter> lstWt2Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				InternationalNonConusWeightCalculations(lstWt2Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}


		/// <summary>
		///     Apply LargestBlockOfDaysoff Weights
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyLargestBlockOfDaysoffWeight(Wt2Parameter wt2Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				LargestBlockOfDaysoff(wt2Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply LegsPerDutyPeriod Weights
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyLegsPerDutyPeriodWeight(List<Wt3Parameter> wt3Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				LegsPerDutyPeriodWeightCalculation(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply LegsPerPairing  Weights
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyLegsPerPairingWeight(List<Wt3Parameter> wt3Parameters)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				LegsPerPairingWeightCalculation(wt3Parameters, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}



		public void ApplyNormalizeDaysOffWeight(Wt2Parameter wtParameter)
		{


			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine && !x.ReserveLine);

			int leastDaysOff = requiredLines.Min(x => x.DaysOff);
			foreach (Line line in requiredLines)
			{
				NormalizeDaysOff(leastDaysOff, wtParameter.Weight, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Number of Days Of Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyNumberOfDaysOfWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				NumberOfDaysOff(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Overnight Cities Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyOverNightCitiesWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				OvernightCitiesWeightCalculations(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Cities Legs Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyCitiesLegsWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				CitiesLegsWeightCalculations(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply position Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyPositionWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				PositionWeightCalculation(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Rest Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyRestWeight(List<Wt4Parameter> lstWt4Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				RestWeight(lstWt4Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Trip Length
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		public void ApplyTripLengthWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				TripLengthWeight(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void ApplyTimeAwayFromBaseWeight(Wt2Parameter wt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				TimeAwayFromBaseWeight(wt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void ApplyStartDayOfWeekWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				StartDateOftheWeekWeight(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply WorkDays Weight
		/// </summary>
		/// <param name="wt2Parameter"></param>
		public void ApplyWorkDaysWeight(List<Wt3Parameter> lstWt3Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				WorkDaysWeight(lstWt3Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Apply Overnight City Weight
		/// </summary>
		/// <param name="wt2Parameter"></param>
		public void ApplyOvernightCityBulkWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				OvernightCityBulkWeight(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}
		public void ApplyWorkBlockLengthWeight(List<Wt2Parameter> lstWt2Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			foreach (Line line in requiredLines)
			{
				WorkBlockLengthWeight(lstWt2Parameter, line);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void ApplyPartialDaysAfterWeight(List<Wt4Parameter> lstWt4Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			List<DateHelper> lstDatehelper = GenarateCalender();
			foreach (Line line in requiredLines)
			{
				PartialDaysAfterWeightCalculation(lstWt4Parameter, line, lstDatehelper);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void ApplyPartialDaysBeforeWeight(List<Wt4Parameter> lstWt4Parameter)
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
			List<DateHelper> lstDatehelper = GenarateCalender();
			foreach (Line line in requiredLines)
			{
				PartialDaysBeforeWeightCalculation(lstWt4Parameter, line, lstDatehelper);
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		#endregion

		#region Remove Weights

		/// <summary>
		///     Remove Aircraft changes weight
		/// </summary>
		public void RemoveAirCraftChangesWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.AcftChanges != 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.AcftChanges = 0;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Remove Aircraft changes weight
		/// </summary>
		public void RemoveRestWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.Rest != 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.Rest = 0;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Remove Time Away from base weight
		/// </summary>
		public void RemoveTimeAwayFromBaseWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.TimeAwayFromBase != 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.TimeAwayFromBase = 0;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void RemoveDaysOfMonthWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.DaysOfMonthOff != 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.DaysOfMonthOff = 0;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void RemoveCommutableLineWeight()
		{
            WBidState currentState =
               GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
                   x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.CmtLines != 0);
            foreach (var line in GlobalSettings.Lines)
            {
                if (requiredLines.Contains(line))
                {
                    line.WeightPoints.CmtLines = 0;
                    line.Points = line.WeightPoints.Total();
                    line.TotWeight = line.Points;
                }
                if (currentState.CxWtState.CL.Cx == false && !(currentState.SortDetails.BlokSort.Contains("36") || currentState.SortDetails.BlokSort.Contains("37") || currentState.SortDetails.BlokSort.Contains("38")))
                {
                    line.CommutableBacks = 0;
                    line.commutableFronts = 0;
                    line.CommutabilityFront = 0;
                    line.CommutabilityBack = 0;
                    line.CommutabilityOverall = 0;

                }
            }
		}

		public void RemoveLargestBlockDaysWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.LrgBlkdaysOff != 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.LrgBlkdaysOff = 0;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		public void RemoveNormalizeDaysOffWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.NormalizeDaysOff != 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.NormalizeDaysOff = 0;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		#endregion

		#region WeightCalculation

		/// <summary>
		///     aircraft change Weight Calculation.
		/// </summary>
		/// <param name="wt3Parameter"></param>
		/// <param name="line"></param>
		public void AircraftChangeWeightCalcaultion(Wt3Parameter wt3Parameter, Line line)
		{
			//2nd cell value less, more, equal, not eq
			//3rd cell value: 1 chg, 2 chg, …, 30 chg

			line.WeightPoints.AcftChanges = 0;
			decimal points = 0;

			if (wt3Parameter.SecondlValue == ((int)WeightType.Less))
			{
				points += Math.Max(wt3Parameter.ThrirdCellValue - line.AcftChanges, 0) * wt3Parameter.Weight;
			}
			else if (wt3Parameter.SecondlValue == ((int)WeightType.More))
			{
				points += Math.Max(line.AcftChanges - wt3Parameter.ThrirdCellValue, 0) * wt3Parameter.Weight;
			}
			else if (wt3Parameter.SecondlValue == ((int)WeightType.Equal))
			{
				if (line.AcftChanges == wt3Parameter.ThrirdCellValue)
				{
					points += line.AcftChanges * wt3Parameter.Weight;
				}
			}
			else if (wt3Parameter.SecondlValue == ((int)WeightType.NotEqual))
			{
				if (line.AcftChanges != wt3Parameter.ThrirdCellValue)
				{
					points += line.AcftChanges * wt3Parameter.Weight;
				}
			}
			line.WeightPoints.AcftChanges = points;
		}

		/// <summary>
		///     AM/PM Weight Calculation
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		public void AMPMWeightCalculation(List<Wt2Parameter> wt2Parameters, Line line)
		{
			//2nd cell value:am, pm, nte
			line.WeightPoints.AmPmNte = 0;
			decimal points = 0;
			foreach (Wt2Parameter wt2Parameter in wt2Parameters)
			{
				if (Convert.ToInt32(wt2Parameter.Type) == (int)AMPMType.AM && line.AMPM == "AM")
				{
					points += wt2Parameter.Weight;
				}
				else if (Convert.ToInt32(wt2Parameter.Type) == (int)AMPMType.PM && line.AMPM == " PM")
				{
					points += wt2Parameter.Weight;
				}
				else if (Convert.ToInt32(wt2Parameter.Type) == (int)AMPMType.NTE && line.AMPM == "NTE")
				{
					points += wt2Parameter.Weight;
				}
			}

			line.WeightPoints.AmPmNte = points;
		}

		/// <summary>
		///     Block Days Off Weight Calculation
		/// </summary>
		/// <param name="wt3Parameters"></param>
		/// <param name="line"></param>
		public void BlockOFFDaysOfWeightCalculations(List<Wt3Parameter> wt3Parameters, Line line)
		{
			//2nd cellvalue :less, more, equal, not eq
			//3rd celll value:blk 1, blk 2, …, blk 31
			line.WeightPoints.BlkDaysOff = 0;



			decimal points = 0;
			//foreach (Wt3Parameter wt3Parameter in wt3Parameters)
			//{
			//    if (wt3Parameter.SecondlValue == ((int)WeightType.Less))
			//    {
			//        var lessthanBlockdays = line.BlkOfDaysOff.TakeWhile(x => x != wt3Parameter.SecondlValue).Sum(y => y);
			//        points += lessthanBlockdays * wt3Parameter.Weight;

			//    }
			//    else if (wt3Parameter.SecondlValue == ((int)WeightType.More))
			//    {
			//        var moreThanBlockdays = line.BlkOfDaysOff.SkipWhile(x => x != wt3Parameter.SecondlValue).Sum(y => y);
			//        points += moreThanBlockdays * wt3Parameter.Weight;
			//    }
			//    else if (wt3Parameter.SecondlValue == ((int)WeightType.Equal))
			//    {
			//        points += line.BlkOfDaysOff[wt3Parameter.SecondlValue] * wt3Parameter.Weight;
			//    }
			//    else if (wt3Parameter.SecondlValue == ((int)WeightType.Equal))
			//    {
			//        var notequldays = line.BlkOfDaysOff.Sum(x => x) - line.BlkOfDaysOff[wt3Parameter.SecondlValue];
			//        points += notequldays * wt3Parameter.Weight;
			//    }
			//}



			foreach (Wt3Parameter wt3Parameter in wt3Parameters)
			{
				if (wt3Parameter.SecondlValue == ((int)WeightType.Less))
				{
					int lessthanBlockdays = line.BlkOfDaysOff.Take(wt3Parameter.ThrirdCellValue).Sum(y => y);
					points += lessthanBlockdays * wt3Parameter.Weight;
				}
				else if (wt3Parameter.SecondlValue == ((int)WeightType.More))
				{
					int moreThanBlockdays = line.BlkOfDaysOff.Skip(wt3Parameter.ThrirdCellValue + 1).Sum(y => y);
					points += moreThanBlockdays * wt3Parameter.Weight;
				}
				else if (wt3Parameter.SecondlValue == ((int)WeightType.Equal))
				{
					points += line.BlkOfDaysOff[wt3Parameter.ThrirdCellValue] * wt3Parameter.Weight;
				}
				else if (wt3Parameter.SecondlValue == ((int)WeightType.NotEqual))
				{
					//var notequldays = line.BlkOfDaysOff.Sum(x => x) - line.BlkOfDaysOff[wt3Parameter.SecondlValue];
					// points += notequldays * wt3Parameter.Weight;
					if (line.BlkOfDaysOff[wt3Parameter.ThrirdCellValue] == 0)
					{
						points += wt3Parameter.Weight;
					}
				}
			}

			line.WeightPoints.BlkDaysOff = points;
		}

		/// <summary>
		///     Commutable DeadHead Weight
		/// </summary>
		/// <param name="wt3Parameters"></param>
		/// <param name="line"></param>
		public void CommutableDeadhead(List<Wt3Parameter> wt3Parameters, Line line)
		{
			//2nd cellvalue :all cites with dh's
			//3rd celll value:begin, end, both

			//Set the CmtDhds weight point to zero

			if (line.LineNum == 264)
			{
			}

			line.WeightPoints.CmtDhds = 0;
			decimal points = 0;
			//Checks  the line has  deadheads or not
			if (line.CmtDhds.Count != 0)
			{
				foreach (Wt3Parameter wt3Parameter in wt3Parameters)
				{
					//Check the weight applied  city in the state file exist in the  deadhead list
					DeadheadCity DeadheadCity = line.CmtDhds.FirstOrDefault(x => x.CityId == wt3Parameter.SecondlValue);

					if (DeadheadCity != null)
					{
						if (wt3Parameter.ThrirdCellValue == (int)DeadheadType.Both)
						{
							line.WeightPoints.CmtDhds += (DeadheadCity.CountFrom + DeadheadCity.CountTo) *
								wt3Parameter.Weight;
						}
						else if (wt3Parameter.ThrirdCellValue == (int)DeadheadType.First)
						{
							line.WeightPoints.CmtDhds += DeadheadCity.CountFrom * wt3Parameter.Weight;
						}
						else if (wt3Parameter.ThrirdCellValue == (int)DeadheadType.Last)
						{
							line.WeightPoints.CmtDhds += DeadheadCity.CountTo * wt3Parameter.Weight;
						}
					}
				}
			}
		}
		/// <summary>
		///     Deadhead First Last calculations
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		private static void CommutableLineAutoWeight(WtCommutableLineAuto wtCommutableLine, Line line)
		{
			bool status = false;
			var wBidStateContent =
				GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(
					x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			//int count = 0;
			//Here we are keeping all commute times for the bid
            if (wBidStateContent.Constraints.DailyCommuteTimesCmmutability != null)
			{


				bool isCommuteFrontEnd = false;
				bool isCommuteBackEnd = false;

				if (line.WorkBlockList != null)
				{
					isCommuteFrontEnd = false;
					isCommuteBackEnd = false;

					foreach (WorkBlockDetails workBlock in line.WorkBlockList)
					{
						//Checking the  corresponding Commute based on Workblock Start time
                        var commutTimes = wBidStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.StartDateTime.Date);

						if (commutTimes != null && wtCommutableLine.ToWork)
						{
							if (commutTimes.EarliestArrivel != DateTime.MinValue) {
                                isCommuteFrontEnd = (commutTimes.EarliestArrivel.AddMinutes (wtCommutableLine.CheckInTime)) <= workBlock.StartDateTime.AddMinutes(workBlock.BriefTime);
								if (!isCommuteFrontEnd) {
									//count++;
									status = true;
									break;
								}
							}
						}


						//Checking the  corresponding Commute based on Workblock End time
						//commutTimes = GlobalSettings.WBidStateContent.BidAuto.DailyCommuteTimes.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDateTime.Date);
						// using EndDate to account for irregular datetimes in company time keeping method.
                        commutTimes = wBidStateContent.Constraints.DailyCommuteTimesCmmutability.FirstOrDefault(x => x.BidDay.Date == workBlock.EndDate.Date);

						if (commutTimes != null && wtCommutableLine.ToHome)
						{
							if (commutTimes.LatestDeparture != DateTime.MinValue) {
								isCommuteBackEnd = commutTimes.LatestDeparture.AddMinutes (-wtCommutableLine.BaseTime) >= workBlock.EndDateTime;
								if (!isCommuteBackEnd) {
									//count++;
									status = true;
									break;
								}
							}
						}

						//“No nights in middle” is GREEN 
						//-----------------------------------------------------------------------------------------------
						if (wtCommutableLine.NoNights)
						{
							//if ((ftCommutableLine.ToWork && isCommuteFrontEnd) || (ftCommutableLine.ToHome && isCommuteBackEnd))
							if (workBlock.BackToBackCount > 0)  // if BackToBackCount is > 0 then there are nights in the middle
							{
								//count++;
								status = true;
								break;
							}


						}

					}

				}
				//if (line.WorkBlockList.Count == count)
				//{
				//    line.WeightPoints.CmtLinesAuto = wtCommutableLine.Weight;
				//}
				if (status == false)
					line.WeightPoints.CmtLinesAuto = wtCommutableLine.Weight;
				else
					line.WeightPoints.CmtLinesAuto = 0;
			}

		}
		/// <summary>
		///     PURPOSE : Calculate Commutable  lines.
		/// </summary>
		/// <param name="line"></param>
		public void CommutableLineWeight(WtCommutableLine wtCommutableLine, Line line)
		{


            line.CommutableBacks = 0;
            line.commutableFronts = 0;
            line.CommutabilityFront = 0;
            line.CommutabilityBack = 0;
            line.CommutabilityOverall = 0;
            line.TotalCommutes = 0;

            line.WeightPoints.CmtLines = 0;

            decimal bothEndsPoints = 0;
            decimal inDomicileStartPoints = 0;
            decimal inDomicileEndPoints = 0;
            bool beginCheck = false;
            bool endCheck = false;
            bool isCommuteFrontEnd = false;
            bool isCommuteBackEnd = false;

            switch (wtCommutableLine.Type)
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

            if (line.WorkBlockList != null)
            {
                foreach (WorkBlockDetails workBlock in line.WorkBlockList)
                {
                    if (beginCheck)
                    {
                        isCommuteFrontEnd = CommuteFrontEnd(wtCommutableLine.TimesList, workBlock.StartDay,
                            workBlock.StartTime);
                        //if false
                        if (!isCommuteFrontEnd)
                            inDomicileStartPoints += wtCommutableLine.InDomicile;

                        inDomicileStartPoints += workBlock.BackToBackCount * wtCommutableLine.InDomicile;
                    }

                    if (endCheck)
                    {
                        isCommuteBackEnd = CommuteBackEnd(wtCommutableLine.TimesList, workBlock.EndDay,
                            workBlock.EndTime);
                        //if false
                        if (!isCommuteBackEnd)
                            inDomicileEndPoints += wtCommutableLine.InDomicile;

                        // inDomicileEndPoints += (workBlock.BackToBackCount > 0 ? workBlock.BackToBackCount - 1 : workBlock.BackToBackCount) * wtDeadHead.InDomicile;
                        inDomicileEndPoints += workBlock.BackToBackCount * wtCommutableLine.InDomicile;
                    }

                    if (beginCheck && endCheck)
                    {
                        //isCommuteFrontEnd=false and isCommuteBackEnd=False
                        if (!isCommuteFrontEnd && !isCommuteBackEnd)
                        {
                            bothEndsPoints += wtCommutableLine.BothEnds;
                        }

                        inDomicileEndPoints -= workBlock.BackToBackCount * wtCommutableLine.InDomicile;
                    }
                    if (isCommuteFrontEnd)
                    {
                        line.commutableFronts++;
                    }
                    if (isCommuteBackEnd)
                    {
                        line.CommutableBacks++;
                    }
                }

                line.WeightPoints.CmtLines = -(bothEndsPoints + inDomicileStartPoints + inDomicileEndPoints);

                line.TotalCommutes = line.WorkBlockList.Count;
                if (line.TotalCommutes > 0)
                {
                    line.CommutabilityFront = Math.Round((line.commutableFronts / line.TotalCommutes) * 100, 2);
                    line.CommutabilityBack = Math.Round((line.CommutableBacks / line.TotalCommutes) * 100, 2);
                    line.CommutabilityOverall = Math.Round((line.commutableFronts + line.CommutableBacks) / (2 * line.TotalCommutes) * 100, 2);
                }

            }

		}

		/// <summary>
		///     PURPOSE : Checks commute front end
		/// </summary>
		/// <param name="checkinTimeList"></param>
		/// <param name="stratDay"></param>
		/// <param name="startTime"></param>
		/// <returns></returns>
		private bool CommuteFrontEnd(List<Times> checkinTimeList, int stratDay, int startTime)
		{
			int checkinStartTime = 0;

			switch (stratDay)
			{
			//Monday--Thurs
			case 1:
			case 2:
			case 3:
			case 4:
				checkinStartTime = checkinTimeList[0].Checkin;
				break;
				//Friday
			case 5:
				checkinStartTime = checkinTimeList[1].Checkin;
				break;
				// saturday
			case 6:
				checkinStartTime = checkinTimeList[2].Checkin;
				break;
				//sunday
			case 0:
				checkinStartTime = checkinTimeList[3].Checkin;
				break;
			}

			return (checkinStartTime <= startTime);
		}

		/// <summary>
		///     Purpose :Checks commute back end
		/// </summary>
		/// <param name="checkinTimeList"></param>
		/// <param name="endDay"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
		private static bool CommuteBackEnd(List<Times> checkinTimeList, int endDay, int endTime)
		{
			int backToBaseEndTime = 0;

			switch (endDay)
			{
				case 1:
				case 2:
				case 3:
				case 4:
					backToBaseEndTime = checkinTimeList[0].BackToBase;
					backToBaseEndTime += (checkinTimeList[0].Checkin > checkinTimeList[0].BackToBase) ? 1440 : 0;
					break;
				case 5:
					backToBaseEndTime = checkinTimeList[1].BackToBase;
					backToBaseEndTime += (checkinTimeList[1].Checkin > checkinTimeList[1].BackToBase) ? 1440 : 0;
					break;
				case 6:
					backToBaseEndTime = checkinTimeList[2].BackToBase;
					backToBaseEndTime += (checkinTimeList[2].Checkin > checkinTimeList[2].BackToBase) ? 1440 : 0;
					break;
				case 0:
					backToBaseEndTime = checkinTimeList[3].BackToBase;
					backToBaseEndTime += (checkinTimeList[3].Checkin > checkinTimeList[3].BackToBase) ? 1440 : 0;
					break;
			}

			return (backToBaseEndTime >= endTime);
		}
		private void CommuttabilityWeight(Commutability commutability, Line line)
		{
			line.WeightPoints.Commute = 0;

			if (commutability.SecondcellValue == ((int)CommutabilitySecondCell.NoMiddle))
			{
				//
				if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Back))
				{
					if (commutability.Type == ((int)ConstraintType.LessThan))
					{
						if (line.NightsInMid == 0 && line.CommutabilityBack <= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
					else if (commutability.Type == ((int)ConstraintType.MoreThan))
					{
						if (line.NightsInMid == 0 && line.CommutabilityBack >= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
				}
				else if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Front))
				{
					if (commutability.Type == ((int)ConstraintType.LessThan))
					{
						if (line.NightsInMid == 0 && line.CommutabilityFront <= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
					else if (commutability.Type == ((int)ConstraintType.MoreThan))
					{
						if (line.NightsInMid == 0 && line.CommutabilityFront >= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}

				}
				if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Overall))
				{
					if (commutability.Type == ((int)ConstraintType.LessThan))
					{
						if (line.NightsInMid == 0 && line.CommutabilityOverall <= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
					else if (commutability.Type == ((int)ConstraintType.MoreThan))
					{
						if (line.NightsInMid == 0 && line.CommutabilityOverall >= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}

				}
				//if(line.NightsInMid<=0)
				//    line.WeightPoints.Commute = commutability.Weight;
			}
			else
			{
				//OK Middle
				if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Back))
				{
					if (commutability.Type == ((int)ConstraintType.LessThan))
					{
						if (line.NightsInMid > 0 && line.CommutabilityBack <= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
					else if (commutability.Type == ((int)ConstraintType.MoreThan))
					{
						if (line.NightsInMid > 0 && line.CommutabilityBack >= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
				}
				else if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Front))
				{
					if (commutability.Type == ((int)ConstraintType.LessThan))
					{
						if (line.NightsInMid > 0 && line.CommutabilityFront <= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
					else if (commutability.Type == ((int)ConstraintType.MoreThan))
					{
						if (line.NightsInMid > 0 && line.CommutabilityFront >= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}

				}
				if (commutability.ThirdcellValue == ((int)CommutabilityThirdCell.Overall))
				{
					if (commutability.Type == ((int)ConstraintType.LessThan))
					{
						if (line.NightsInMid >= 0 && line.CommutabilityOverall <= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}
					else if (commutability.Type == ((int)ConstraintType.MoreThan))
					{
						if (line.NightsInMid >= 0 && line.CommutabilityOverall >= commutability.Value)
							line.WeightPoints.Commute = commutability.Weight;
					}

				}
			}

		}
		/// <summary>
		///     PURPOSE: Calculate Day of the month  weight
		/// </summary>
		/// <param name="line"></param>
		public static void DayOftheMonthWeight(List<DaysOfMonthHelper> appliedWeights, bool isWork, Line line)
		{
			decimal points = 0.0m;

			line.WeightPoints.DaysOfMonthOff = 0.0m;

			if (isWork)
			{
				foreach (DaysOfMonthHelper daysOfMonth in appliedWeights)
				{
					WorkDaysOfBidLine dayObject =
						line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == daysOfMonth.Date);
					if (dayObject != null)
						points += daysOfMonth.Weight;
				}
			}
			else
			{
				foreach (DaysOfMonthHelper daysOfMonth in appliedWeights)
				{
					WorkDaysOfBidLine dayObject =
						line.DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == daysOfMonth.Date);
					if (dayObject == null)
						points += daysOfMonth.Weight;
				}
			}

			line.WeightPoints.DaysOfMonthOff = points;

			//get working days for the month
			//List<WorkDaysOfBidLine> DaysOfMonthWorks = line.DaysOfMonthWorks;


			////non working days Id.(Id is used as same as in the view starting from 1.) These Id is used becuase date 1,2 etc  are repeated.
			//List<int> lstnonworkingdayId = new List<int>();
			//int id = 0;
			////start date of the current bid period.
			//DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
			////new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
			//id = (int)startDate.DayOfWeek + 1;


			//for (int count = 0; count < 35; count++)
			//{
			//    if (DaysOfMonthWorks.FirstOrDefault(x => x.DayOfBidline == startDate) == null)
			//    {
			//        lstnonworkingdayId.Add(id);
			//    };
			//    startDate = startDate.AddDays(1);
			//    id++;

			//}
			//line.WeightPoints.DaysOfMonthOff = 0.0m;
			//foreach (int key in lstnonworkingdayId)
			//{
			//    var dmo = daysOfmonthoff.FirstOrDefault(x => x.Key == key);
			//    if (dmo != null)
			//    {
			//        line.WeightPoints.DaysOfMonthOff += dmo.Value;
			//    }
			//}
		}

		/// <summary>
		///     PURPOSE : Calculate DaysOfWeek Weight
		/// </summary>
		/// <param name="line"></param>
		public static void DaysOfWeekWeightCalculation(WtDaysOfWeek DaysOfWeekWeights, Line line)
		{
			line.WeightPoints.DaysOfWeekOff = 0;
			foreach (Wt wt in DaysOfWeekWeights.lstWeight)
			{
				if (DaysOfWeekWeights.IsOff)
				{
					int dayvalue = (wt.Key == 6) ? 0 : wt.Key + 1;
					int offcount = line.DaysInBidPeriod.Where(x => x.OffDuty == true && x.Date <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && (int)x.Date.DayOfWeek == dayvalue).Count();
					line.WeightPoints.DaysOfWeekOff += offcount * wt.Value;
				}
				else
					line.WeightPoints.DaysOfWeekOff += (line.DaysOfWeekWork[wt.Key] * wt.Value);
			}
		}


		/// <summary>
		///     PURPOSE: Calculate Duty Period  weight
		/// </summary>
		/// <param name="line"></param>
		public static void DutyPeriodWeightCalculation(List<Wt3Parameter> wt3Parameters, Line line)
		{
			line.WeightPoints.DutyPeriod = 0;

			decimal points = 0.0m;
			foreach (Wt3Parameter wt3Parameter in wt3Parameters)
			{
				//get the list of duty period hour for the line.
				List<int> dutyperiodHours = line.DutyPeriodHours;
				// line.WeightPoints.DutyPeriod = 0;
				if (wt3Parameter.SecondlValue == (int)DutyPeriodType.Relative)
				{
					List<int> longerdutyperiods =
						line.DutyPeriodHours.Where(x => x > wt3Parameter.ThrirdCellValue).ToList();
					decimal longersum = longerdutyperiods.Sum(x => x) -
						(longerdutyperiods.Count * wt3Parameter.ThrirdCellValue);
					decimal longerresult = Math.Round((longersum / 60) * 10, 2);
					//line.WeightPoints.DutyPeriod -= longerresult;
					points -= longerresult;

					List<int> shorterdutyperiods =
						line.DutyPeriodHours.Where(x => x < wt3Parameter.ThrirdCellValue).ToList();
					decimal shortersum = (shorterdutyperiods.Count * wt3Parameter.ThrirdCellValue) -
						shorterdutyperiods.Sum(x => x);
					decimal shorterresult = Math.Round((shortersum / 60) * wt3Parameter.Weight, 2);
					//line.WeightPoints.DutyPeriod += shorterresult;
					points -= shorterresult;
				}
				//Duty period longer than length
				else if (wt3Parameter.SecondlValue == (int)DutyPeriodType.Longer)
				{
					List<int> longerdutyperiods =
						line.DutyPeriodHours.Where(x => x > wt3Parameter.ThrirdCellValue).ToList();
					decimal sum = longerdutyperiods.Sum(x => x) - (longerdutyperiods.Count * wt3Parameter.ThrirdCellValue);
					decimal result = Math.Round((sum / 60) * wt3Parameter.Weight, 2);
					//line.WeightPoints.DutyPeriod -= result;
					points += result;
					;
				}
				//Duty Period Shorter than Length
				if (wt3Parameter.SecondlValue == (int)DutyPeriodType.Shorter)
				{
					List<int> shorterdutyperiods =
						line.DutyPeriodHours.Where(x => x < wt3Parameter.ThrirdCellValue).ToList();
					decimal sum = (shorterdutyperiods.Count * wt3Parameter.ThrirdCellValue) -
						shorterdutyperiods.Sum(x => x);
					decimal result = Math.Round((sum / 60) * wt3Parameter.Weight);
					// line.WeightPoints.DutyPeriod += result;
					points += result;
				}
			}
			line.WeightPoints.DutyPeriod = points;
		}

		/// <summary>
		///     Deadhead First Last calculations
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		public void DeadheadFisrtLastWeightCalculation(List<Wt2Parameter> wt2Parameters, Line line)
		{
			//3rd cell:first, last
			line.WeightPoints.DeadHeadFoL = 0;
			decimal points = 0;
			foreach (Wt2Parameter wt2Parameter in wt2Parameters)
			{
				if (wt2Parameter.Type == (int)DeadheadType.First)
				{
					points += line.DhFirstTotal * wt2Parameter.Weight;
				}
				else if (wt2Parameter.Type == (int)DeadheadType.Last)
				{
					points += line.DhLastTotal * wt2Parameter.Weight;
				}
			}
			line.WeightPoints.DeadHeadFoL = points;
		}

		/// <summary>
		///     equipment type weight calculations.
		/// </summary>
		/// <param name="wt3Parameters"></param>
		/// <param name="line"></param>
		public void EquipmentTypeWeights(List<Wt3Parameter> wt3Parameters, Line line)
		{
			//2nd cell value: 300, 500, 700, 800
			//3rd cell value:1 leg, 2 legs, 3 legs, … 31 legs

			decimal points = 0;
			foreach (Wt3Parameter wt3Parameter in wt3Parameters)
			{
				int legsIn = 0;
				//if (wt3Parameter.SecondlValue == 300)
				//{
				//	legsIn = line.LegsIn300;
				//}
				//else if (wt3Parameter.SecondlValue == 500)
				//{
				//	legsIn = line.LegsIn500;
				//}
				if (wt3Parameter.SecondlValue == 700)
				{
					legsIn = line.LegsIn700;
				}
				else if (wt3Parameter.SecondlValue == 800)
				{
					legsIn = line.LegsIn800;
				}
				else if (wt3Parameter.SecondlValue == 600)
				{
					legsIn = line.LegsIn600;
				}
				else if (wt3Parameter.SecondlValue == 200)
				{
					legsIn = line.LegsIn200;
				}

				// points += (legsIn - wt3Parameter.ThrirdCellValue) * wt3Parameter.Weight;
				points += legsIn * wt3Parameter.Weight;
			}
			line.WeightPoints.EquipType = points;
		}

		/// <summary>
		///     etops weight calculations.
		/// </summary>
		/// <param name="wt1Parameters"></param>
		/// <param name="line"></param>
		public void ETOPSWeights(List<Wt1Parameter> wt1Parameters, Line line)
		{
			decimal points = 0;
			if (!line.ReserveLine)
			{
				foreach (Wt1Parameter wt1Parameter in wt1Parameters)
				{

					points += line.ETOPSTripsCount * wt1Parameter.Weight;
				}
				line.WeightPoints.ETOPS = points;
			}
		}

		/// <summary>
		///     etops reserve weight calculations.
		/// </summary>
		/// <param name="wt1Parameters"></param>
		/// <param name="line"></param>
		public void ETOPSResWeights(List<Wt1Parameter> wt1Parameters, Line line)
		{
			decimal points = 0;
			if (line.ReserveLine)
			{
				foreach (Wt1Parameter wt1Parameter in wt1Parameters)
				{

					points += line.ETOPSTripsCount * wt1Parameter.Weight;
				}
				line.WeightPoints.ETOPSRes = points;
			}
		}

		/// <summary>
		///     Flight Time Weight Calculation.
		/// </summary>
		/// <param name="wt3Parameters"></param>
		/// <param name="line"></param>
		public void FlightTimeWeightCalculation(List<Wt3Parameter> wt3Parameters, Line line)
		{
			//2nd cell value : Less,more
			//Third Cell value: 20-140
			line.WeightPoints.BlockTime = 0;
			decimal points = 0;
			foreach (Wt3Parameter wt3Parameter in wt3Parameters)
			{
				// Convert flight time to fractional hours
				decimal blkHrsInBp = Helper.ConvertHhhColonMmToFractionalHours(line.BlkHrsInBp);

				if (wt3Parameter.SecondlValue == ((int)WeightType.Less))
				{
					points += Math.Round(Math.Max(wt3Parameter.ThrirdCellValue - blkHrsInBp, 0) * wt3Parameter.Weight, 1);
				}
				else if (wt3Parameter.SecondlValue == ((int)WeightType.More))
				{
					points += Math.Round(Math.Max(blkHrsInBp - wt3Parameter.ThrirdCellValue, 0) * wt3Parameter.Weight, 1);
				}
			}
			line.WeightPoints.BlockTime = points;
		}

		/// <summary>
		///     Ground Time Weight Calculation
		/// </summary>
		/// <param name="wt3Parameters"></param>
		/// <param name="line"></param>
		public void GroundTimeWeightCalculations(List<Wt3Parameter> wt3Parameters, Line line)
		{
			//2nd cell value:0:15 to 6:00 in 5 min increments

			//3 rd cell value: 1 occur, 2 occur, 3 occur, … 25 occur
			if (!line.ReserveLine)
			{
				decimal points = 0;
				line.WeightPoints.GrndTime = 0;
				foreach (Wt3Parameter wt3Parameter in wt3Parameters)
				{
					int liness = line.LineNum;
					IEnumerable<int> groundtime = line.GroundTimes.Where(x => x > wt3Parameter.SecondlValue);
					if (groundtime.Count() >= wt3Parameter.ThrirdCellValue)
					{
						points -= (groundtime.Count() - (wt3Parameter.ThrirdCellValue - 1)) * wt3Parameter.Weight;
					}
				}
				line.WeightPoints.GrndTime = points;
			}
		}


		private void InternationalNonConusWeightCalculations(List<Wt2Parameter> lstWt2Parameter, Line line)
		{
			decimal points = 0.0m;
			List<string> cities = null;
			Trip trip = null;
			int count = 0;
			var arrivalCities = new List<string>();
			foreach (string pairing in line.Pairings)
			{
				trip = GetTrip(pairing);
				if (trip != null)
				{
					arrivalCities.AddRange(trip.DutyPeriods.SelectMany(x => x.Flights.Select(y => y.ArrSta)).ToList());
				}
			}

			//-1  : All international, 
			// 0  : All NonConus
			// id
			foreach (Wt2Parameter wt2Parameter in lstWt2Parameter)
			{
				cities = new List<string>();
				//All International 
				if (wt2Parameter.Type == -1)
				{
					cities =
						GlobalSettings.WBidINIContent.Cities.Where(x => x.International).Select(y => y.Name).ToList();
				}
				//  //All Nonconus 
				else if (wt2Parameter.Type == 0)
				{
					cities = GlobalSettings.WBidINIContent.Cities.Where(x => x.NonConus).Select(y => y.Name).ToList();
				}
				else
				{
					var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(
					x => (x.International || x.NonConus) && x.Id == wt2Parameter.Type);
					if (city != null)
					{
						cities.Add(city.Name);
					}
					

				}


				count = arrivalCities.Where(x => cities.Any(y => y == x)).Count();

				points += count * wt2Parameter.Weight;
			}


			line.WeightPoints.InternationalConus = points;
		}

		public void LargestBlockOfDaysoff(Wt2Parameter wt2Parameter, Line line)
		{
			line.WeightPoints.LrgBlkdaysOff = line.LargestBlkOfDaysOff * wt2Parameter.Weight;
		}

		/// <summary>
		///     Legs Per Duty Period Weight Calculation.
		/// </summary>
		/// <param name="wt3Parameters"></param>
		/// <param name="line"></param>
		public void LegsPerDutyPeriodWeightCalculation(List<Wt3Parameter> wt3Parameters, Line line)
		{
			//Type: 0 leg, 1 leg, 2 legs, … 9 legs

			//decimal points = 0;
			//line.WeightPoints.LegsPerDutPd = 0;
			//foreach (Wt2Parameter wt2Parameter in wt2Parameters)
			//{
			//    points += (wt2Parameter.Weight * line.LegsPerDutyPeriod[wt2Parameter.Type]);
			//}
			//line.WeightPoints.LegsPerDutPd = points;


			decimal points = 0;
			line.WeightPoints.LegsPerDutPd = 0;

			if (line.ReserveLine)
				return;

			foreach (Wt3Parameter wt3Parameter in wt3Parameters)
			{
				switch (wt3Parameter.SecondlValue)
				{
				case ((int)WeightType.Less)://less
					points += wt3Parameter.Weight * line.LegsPerDutyPeriod.Take(wt3Parameter.ThrirdCellValue).Sum(y => y);
					break;

				case ((int)WeightType.Equal)://equal
					points += (wt3Parameter.Weight * line.LegsPerDutyPeriod[wt3Parameter.ThrirdCellValue]);
					break;

				case ((int)WeightType.More)://equal
					points += wt3Parameter.Weight * line.LegsPerDutyPeriod.Skip(wt3Parameter.ThrirdCellValue + 1).Sum(y => y);
					break;

				}
			}
			line.WeightPoints.LegsPerDutPd = points;
		}

		/// <summary>
		///     lLegs per pairing calculation
		/// </summary>
		/// <param name="wt3Parameters"></param>
		/// <param name="line"></param>
		public void LegsPerPairingWeightCalculation(List<Wt3Parameter> wt3Parameters, Line line)
		{
			//2nd cell value:all, more, less
			//3 rd cell value: 1 leg, 2 legs, 3 legs, …, 30 legs

			line.WeightPoints.LegsPerTrip = 0;
			foreach (Wt3Parameter wt3Parameter in wt3Parameters)
			{
				//filter the pairings for the line from trip file
				//List<Trip> linePairings = GlobalSettings.Trip.Where(x => line.Pairings.Any(w => w.StartsWith(x.TripNum))).ToList();

				var linePairings = new List<Trip>();
				foreach (string pairing in line.Pairings)
				{
					linePairings.Add(GetTrip(pairing));
				}


				foreach (Trip t in linePairings)
				{
					switch (wt3Parameter.SecondlValue)
					{
					case 1: //less
						// The Weight is only added for pairings with fewer than ‘Legs’ flown segments. 
						if (t.TotalLegs < wt3Parameter.ThrirdCellValue)
							line.WeightPoints.LegsPerTrip += (wt3Parameter.Weight *
								(wt3Parameter.ThrirdCellValue - t.TotalLegs));
						break;
					case 2: // The Weight is only subtracted for pairings with more than ‘Legs’ flown segments.
						if (t.TotalLegs > wt3Parameter.ThrirdCellValue)
							line.WeightPoints.LegsPerTrip += (wt3Parameter.Weight *
								(wt3Parameter.ThrirdCellValue - t.TotalLegs));
						break;
					case 3: //all
						// The Weight will be added for each pairing with fewer than ‘Legs’ flown segments. The Weight will be subtracted for each pairing with more than ‘Legs’ flown segments.
						line.WeightPoints.LegsPerTrip += (wt3Parameter.Weight *
							(wt3Parameter.ThrirdCellValue - t.TotalLegs));
						break;
					}
				}
			}
		}


		public void NormalizeDaysOff(int leastAmountOffDaysOff, decimal wtParameter, Line line)
		{


			line.WeightPoints.NormalizeDaysOff = (line.DaysOff - leastAmountOffDaysOff) * wtParameter;

		}

		/// <summary>
		///     Number of ff days of weihgt calaulation
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		public void NumberOfDaysOff(List<Wt2Parameter> wt2Parameters, Line line)
		{
			//TYPE :9 off, 10 off, 11 off, …, 31 off
			line.WeightPoints.NumDaysOff = 0;
			foreach (Wt2Parameter wt2Parameter in wt2Parameters)
			{
				if (line.DaysOff == wt2Parameter.Type)
				{
					line.WeightPoints.NumDaysOff += wt2Parameter.Weight;
				}
			}
		}

		/// <summary>
		///     Overnight Cities Calculations
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		public void OvernightCitiesWeightCalculations(List<Wt2Parameter> wt2Parameters, Line line)
		{
			//TYPE :All the overnight cities in the bid data
			line.WeightPoints.OvernightCities = 0;
			foreach (Wt2Parameter wt2Parameter in wt2Parameters)
			{
				var city = GlobalSettings.OverNightCitiesInBid.FirstOrDefault(x => x.Id == wt2Parameter.Type);
				if (city != null)
				{
					string cityName =
						GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == wt2Parameter.Type).Name;

					//if (line.DaysOff == wt2Parameter.Weight)
					//{
					line.WeightPoints.OvernightCities +=
						line.OvernightCities.Where(x => x.ToString() == cityName).ToList().Count * wt2Parameter.Weight;
					//}
				}
			}
		}
		/// <summary>
		/// CitiesLegs Calculations
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		private void CitiesLegsWeightCalculations(List<Wt2Parameter> wt2Parameters, Line line)
		{
			if (!line.ReserveLine)
			{
				Trip trip;
				List<string> citiesleges = new List<string>();
				foreach (var pairing in line.Pairings)
				{
					trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
					if (trip == null)
					{
						trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
					}
					trip = GlobalSettings.Trip.Where(x => x.TripNum == pairing.Substring(0, 4)).FirstOrDefault();
					trip = trip ?? GlobalSettings.Trip.Where(x => x.TripNum == pairing).FirstOrDefault();
					//  string tripName = pairing.Substring(0, 4);
					foreach (var dp in trip.DutyPeriods)
					{
						foreach (var flt in dp.Flights)
							if (flt.ArrSta != null)
							{
								citiesleges.Add(flt.ArrSta);
							}
					}
					//remove last flights because we dont need to consider this last flight of a trip
					if (citiesleges.Count >= 1)
					{
						citiesleges.RemoveAt(citiesleges.Count - 1);
					}
				}

				line.WeightPoints.CitiesLegs = 0;
				foreach (Wt2Parameter wt2Parameter in wt2Parameters)
				{
					string cityName =
						GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == wt2Parameter.Type).Name;
					var cities = citiesleges.Where(x => x.ToString() == cityName);
					if (cities != null)
					{
						try
						{
							line.WeightPoints.CitiesLegs +=
								citiesleges.Where(x => x.ToString() == cityName).ToList().Count * wt2Parameter.Weight;
						}
						catch (Exception ex)
						{

							throw;
						}
					}
					else
					{ 
					}
				}
			}
		}
		/// <summary>
		///     Partial Days Before weight Calculation
		/// </summary>
		/// <param name="Wt4Parameters"></param>
		/// <param name="line"></param>
		/// <param name="lstDateHelper"></param>
		public void PartialDaysBeforeWeightCalculation(List<Wt4Parameter> Wt4Parameters, Line line,
			List<DateHelper> lstDateHelper)
		{
			//1 st value:pops up calendar
			//2nd value: 03:00 to 27:00 in 15 minute increments
			//3rd value:All overnight cities in the bid, domicile also
			var date = new DateTime();
			Day newDay = null;
			List<City> citylist = GlobalSettings.WBidINIContent.Cities;
			line.WeightPoints.PartialDaysBefore = 0;
			foreach (Wt4Parameter wt4Parameter in Wt4Parameters)
			{
				City city = citylist.FirstOrDefault(x => x.Id == wt4Parameter.ThrirdCellValue);

				if (city != null)
				{
					//  //300 indicates the user selected Any Date from the Date selection view
					if (wt4Parameter.FirstValue == 300)
					{
						newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty  && x.DepartutreCity == city.Name) || (x.DepartutreCity == city.Name && (x.DepartutreTime) >= wt4Parameter.SecondlValue));
						if (newDay != null)
							line.WeightPoints.PartialDaysBefore = line.WeightPoints.PartialDaysBefore + wt4Parameter.Weight;
					}
					else
					{
						date = lstDateHelper.FirstOrDefault(x => x.DateId == wt4Parameter.FirstValue).Date;

						newDay =line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty && x.Date == date && x.DepartutreCity == city.Name) ||(x.Date == date && x.DepartutreCity == city.Name && (x.DepartutreTime) >= wt4Parameter.SecondlValue));
						if (newDay != null)
							line.WeightPoints.PartialDaysBefore = line.WeightPoints.PartialDaysBefore + wt4Parameter.Weight;
					}
				}//400 indicates the user selected Any city from the city selection view. In those case we dont need to consider city
				else if (wt4Parameter.ThrirdCellValue == 400)
				{
					//  //300 indicates the user selected Any Date from the Date selection view
					if (wt4Parameter.FirstValue == 300)
					{

						//newDay = line.DaysInBidPeriod.Any(x => x.DepartutreTime < wt4Parameter.SecondlValue && x.OffDuty == false) ? null : new Day();
						//if (newDay != null)
						//    line.WeightPoints.PartialDaysBefore = line.WeightPoints.PartialDaysBefore + wt4Parameter.Weight;
						bool status = line.DaysInBidPeriod.Any(x => x.DepartutreTime < wt4Parameter.SecondlValue && x.OffDuty == false);
						if (!status)
							line.WeightPoints.PartialDaysBefore = line.WeightPoints.PartialDaysBefore + wt4Parameter.Weight;
					}
					else
					{
						date = lstDateHelper.FirstOrDefault(x => x.DateId == wt4Parameter.FirstValue).Date;

						newDay = line.DaysInBidPeriod.FirstOrDefault(x => (x.OffDuty && x.Date == date ) || (x.Date == date && (x.DepartutreTime) >= wt4Parameter.SecondlValue));
						if (newDay != null)
							line.WeightPoints.PartialDaysBefore = line.WeightPoints.PartialDaysBefore + wt4Parameter.Weight;
					}

				}
			}
		}

		/// <summary>
		///     PartialDaysAfter Weight Calculations
		/// </summary>
		/// <param name="Wt4Parameters"></param>
		/// <param name="line"></param>
		/// <param name="lstDateHelper"></param>
		public void PartialDaysAfterWeightCalculation(List<Wt4Parameter> Wt4Parameters, Line line,
			List<DateHelper> lstDateHelper)
		{
			//1 st value:pops up calendar
			//2nd value: 03:00 to 27:00 in 15 minute increments
			//3rd value:All overnight cities in the bid, domicile also
			var date = new DateTime();
			Day newDay = null;
			List<City> citylist = GlobalSettings.WBidINIContent.Cities;
			line.WeightPoints.PartialDaysAfter = 0;
			foreach (Wt4Parameter wt4Parameter in Wt4Parameters)
			{
				City city = citylist.FirstOrDefault(x => x.Id == wt4Parameter.ThrirdCellValue);

				if (city != null)
				{
					//  //300 indicates the user selected Any Date from the Date selection view
					if (wt4Parameter.FirstValue == 300)
					{
						newDay =
							line.DaysInBidPeriod.FirstOrDefault(
								x => (x.OffDuty  && x.ArrivalCity == city.Name) ||
								(x.ArrivalCity == city.Name &&
									(x.ArrivalTime) <= wt4Parameter.SecondlValue));
						if (newDay != null)
							line.WeightPoints.PartialDaysAfter = line.WeightPoints.PartialDaysAfter + wt4Parameter.Weight;
					}
					else
					{
						date = lstDateHelper.FirstOrDefault(x => x.DateId == wt4Parameter.FirstValue).Date;

						newDay =
							line.DaysInBidPeriod.FirstOrDefault(
								x => (x.OffDuty && x.Date == date && x.ArrivalCity == city.Name) ||
								(x.Date == date && x.ArrivalCity == city.Name &&
									(x.ArrivalTime) <= wt4Parameter.SecondlValue));
						if (newDay != null)
							line.WeightPoints.PartialDaysAfter = line.WeightPoints.PartialDaysAfter + wt4Parameter.Weight;
					}
				}
				//400 indicates the user selected Any city from the city selection view. In those case we dont need to consider city
				else if (wt4Parameter.ThrirdCellValue == 400)
				{
					//  //300 indicates the user selected Any Date from the Date selection view
					if (wt4Parameter.FirstValue == 300)
					{
						newDay = line.DaysInBidPeriod.Any(x => x.ArrivalTime > wt4Parameter.SecondlValue && x.OffDuty == false) ? null : new Day();
						if (newDay != null)
							line.WeightPoints.PartialDaysAfter = line.WeightPoints.PartialDaysAfter + wt4Parameter.Weight;
					}
					else
					{
						date = lstDateHelper.FirstOrDefault(x => x.DateId == wt4Parameter.FirstValue).Date;

						newDay =
							line.DaysInBidPeriod.FirstOrDefault(
								x => (x.OffDuty && x.Date == date) ||
								(x.Date == date && 
									(x.ArrivalTime) <= wt4Parameter.SecondlValue));
						if (newDay != null)
							line.WeightPoints.PartialDaysAfter = line.WeightPoints.PartialDaysAfter + wt4Parameter.Weight;
					}
				}
			}
		}


		/// <summary>
		///     position Weight Calculations
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		public void PositionWeightCalculation(List<Wt2Parameter> wt2Parameters, Line line)
		{
			//Type : A,B,C,D

			line.WeightPoints.Position = 0;
			foreach (Wt2Parameter wt2Parameter in wt2Parameters)
			{
				var positions = new List<string>();
				if (GlobalSettings.CurrentBidDetails.Round == "M")
				{
					positions = line.FAPositions;
				}
				else
				{
					positions =
						line.FASecondRoundPositions.Values.ToList().Select(y => y.Substring(y.Length - 1, 1)).ToList();
				}
				string position = string.Empty;
				switch (wt2Parameter.Type)
				{
				case 1:
					position = "A";
					break;
				case 2:
					position = "B";
					break;
				case 3:
					position = "C";
					break;
				case 4:
					position = "D";
					break;
				}

				if (positions.Contains(position))
					line.WeightPoints.Position += wt2Parameter.Weight * positions.Where(x => x == position).Count();
			}
		}


		/// <summary>
		///     Remove
		/// </summary>
		public void RemovWorkBlockLengthWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.WorkBlklength > 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.WorkBlklength = 0.0m;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}


		/// <summary>
		///     Remove work Days weight
		/// </summary>
		/// <param name="wt2Parameter"></param>
		public void RemoveWorkDaysWeight()
		{
			IEnumerable<Line> requiredLines = GlobalSettings.Lines.Where(x => x.WeightPoints.WorkDays > 0);
			foreach (Line line in requiredLines)
			{
				line.WeightPoints.WorkDays = 0.0m;
				line.Points = line.WeightPoints.Total();
				line.TotWeight = line.Points;
			}
		}

		/// <summary>
		///     Start Day of the Week
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		/// <param name="line"></param>
		private void StartDateOftheWeekWeight(List<Wt2Parameter> lstWt2Parameter, Line line)
		{
			line.WeightPoints.StartDow = 0.0m;
			foreach (var item in lstWt2Parameter)
			{
				line.WeightPoints.StartDow += item.Weight * line.StartDaysList[item.Type];
			}
		}

		/// <summary>
		///     Time Away from Base
		/// </summary>
		/// <param name="wt2Parameter"></param>
		/// <param name="line"></param>
		private void TimeAwayFromBaseWeight(Wt2Parameter wt2Parameter, Line line)
		{
			//(breakpoint minutes- line.TafbInBpMinutes)   * Weight
			//          line.WeightPoints.TimeAwayFromBase = (wt2Parameter.Type - Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp)) * (wt2Parameter.Weight/60);

			line.WeightPoints.TimeAwayFromBase = ((wt2Parameter.Type * 60) -
				Helper.ConvertformattedHhhmmToMinutes(line.TafbInBp)) *
				(wt2Parameter.Weight / 60);
		}


		/// <summary>
		///     Trip Length Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		/// <param name="line"></param>
		private void TripLengthWeight(List<Wt2Parameter> lstWt2Parameter, Line line)
		{
			decimal weight = 0.0m;
			foreach (Wt2Parameter wt2Parameter in lstWt2Parameter)
			{
				switch (wt2Parameter.Type)
				{
				case 1:
					weight += line.Trips1Day * wt2Parameter.Weight;
					break;
				case 2:
					weight += line.Trips2Day * wt2Parameter.Weight;
					break;
				case 3:
					weight += line.Trips3Day * wt2Parameter.Weight;
					break;
				case 4:
					weight += line.Trips4Day * wt2Parameter.Weight;
					break;
				}
			}
			line.WeightPoints.TripLength = weight;
		}


		/// <summary>
		///     Work Block Length Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		/// <param name="line"></param>
		private void WorkBlockLengthWeight(List<Wt2Parameter> lstWt2Parameter, Line line)
		{
			decimal weight = 0.0m;
			foreach (Wt2Parameter wt2Parameter in lstWt2Parameter)
			{
				if (line.WorkBlockLengths[wt2Parameter.Type] > 0)
				{
					weight += line.WorkBlockLengths[wt2Parameter.Type] * wt2Parameter.Weight;
				}
			}
			line.WeightPoints.WorkBlklength = weight;
		}


		///// <summary>
		///// Work Days weight
		///// </summary>
		///// <param name="lstWt2Parameter"></param>
		///// <param name="line"></param>
		//private void WorkDaysWeight(List<Wt2Parameter> lstWt2Parameter, Line line)
		//{
		//    decimal weight = 0.0m;

		//    int workingDays =
		//        line.DaysOfMonthWorks.Count(
		//            x =>
		//                x.Working &&
		//                (x.DayOfBidline >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate &&
		//                 x.DayOfBidline <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate));
		//    foreach (Wt2Parameter wt2Parameter in lstWt2Parameter)
		//    {
		//        if (workingDays == wt2Parameter.Type)
		//        {
		//            weight += wt2Parameter.Weight;
		//        }
		//    }
		//    line.WeightPoints.WorkDays = weight;
		//}

		/// <summary>
		/// Work Days weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		/// <param name="line"></param>
		private void WorkDaysWeight(List<Wt3Parameter> lstWt3Parameter, Line line)
		{
			//2nd cell value:all, more, less
			//3 rd cell value: 
			//int workingDays = line.DaysOfMonthWorks.Count(x => x.Working && (x.DayOfBidline >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate && x.DayOfBidline <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate));
			//we do not need to check in bp for the work days
			int workingDays = line.DaysOfMonthWorks.Count(x => x.Working);
			line.WeightPoints.WorkDays = 0;
			foreach (Wt3Parameter wt3Parameter in lstWt3Parameter)
			{
				if (wt3Parameter.SecondlValue == ((int)WeightType.Less))
				{
					if (workingDays < wt3Parameter.ThrirdCellValue)
						line.WeightPoints.WorkDays += wt3Parameter.Weight;
				}
				else if (wt3Parameter.SecondlValue == ((int)WeightType.More))
				{
					if (workingDays > wt3Parameter.ThrirdCellValue)
						line.WeightPoints.WorkDays += wt3Parameter.Weight;
				}
				else if (wt3Parameter.SecondlValue == ((int)WeightType.Equal))
				{
					if (workingDays == wt3Parameter.ThrirdCellValue)
						line.WeightPoints.WorkDays += wt3Parameter.Weight;
				}
			}

		}

		/// <summary>
		/// Overnight city Bulk Weight calculation
		/// </summary>
		/// <param name="wt2Parameters"></param>
		/// <param name="line"></param>
		public void OvernightCityBulkWeight(List<Wt2Parameter> wt2Parameters, Line line)
		{

			List<City> citylist = GlobalSettings.WBidINIContent.Cities;

			line.WeightPoints.OvernightCities = 0.0m;
			foreach (Wt2Parameter wt in wt2Parameters)
			{
				string cityName = citylist.FirstOrDefault(x => x.Id == wt.Type).Name;
				line.WeightPoints.OvernightCities += line.OvernightCities.Where(x => x.ToString() == cityName).ToList().Count * wt.Weight;

			}
		}
		/// <summary>
		///     Rest Weight
		/// </summary>
		/// <param name="lstWt2Parameter"></param>
		/// <param name="line"></param>
		private void RestWeight(List<Wt4Parameter> lstWt4Parameter, Line line)
		{
			//First cell value :shorter, + & -, longer
			// 2nd rd cell value:Time
			// 3rd rd cell value:all, away, inDom

			line.WeightPoints.Rest = 0.0m;
			foreach (Wt4Parameter wt4Parameter in lstWt4Parameter)
			{
			

				if (wt4Parameter.FirstValue == (int)RestOptions.Longer)
				{
					//Subtract weight  where rest period shorter than length
					//-----------------------------------------------------
					IEnumerable<RestPeriod> query =
						line.RestPeriods.Where(x => x.RestMinutes > wt4Parameter.SecondlValue);
					//RestInTrip
					if (wt4Parameter.ThrirdCellValue == (int)RestType.AwayDomicile)
					{
						query = query.Where(x => x.IsInTrip);
					}
					//RestBetweenTrip
					else if (wt4Parameter.ThrirdCellValue == (int)RestType.InDomicile)
					{
						query = query.Where(x => !x.IsInTrip);
					}

					//line.WeightPoints.Rest=-1*( query.ToList().Count * wtRest.Wt);
					line.WeightPoints.Rest =
						-query.Sum(x => (wt4Parameter.SecondlValue - x.RestMinutes) / 60m * wt4Parameter.Weight);
				}
				//-----------------------------------------------------

				// if check box unchecked-- Add weight  where rest period longer than length
				//-----------------------------------------------------
				else if (wt4Parameter.FirstValue == (int)RestOptions.Shorter)
				{
					IEnumerable<RestPeriod> query =
						line.RestPeriods.Where(x => x.RestMinutes < wt4Parameter.SecondlValue);
					//RestInTrip
					if (wt4Parameter.ThrirdCellValue == (int)RestType.AwayDomicile)
					{
						query = query.Where(x => x.IsInTrip);
					}
					//RestBetweenTrip
					else if (wt4Parameter.ThrirdCellValue == (int)RestType.InDomicile)
					{
						query = query.Where(x => !x.IsInTrip);
					}
					//line.WeightPoints.Rest += (query.ToList().Count * wtRest.Wt);
					line.WeightPoints.Rest +=
						query.Sum(x => (decimal)(x.RestMinutes - wt4Parameter.SecondlValue) / 60m * wt4Parameter.Weight);
				}
				else if (wt4Parameter.FirstValue == (int)RestOptions.Both)
				{
					//Subtract weight  where rest period shorter than length
					//-----------------------------------------------------
					IEnumerable<RestPeriod> query =
						line.RestPeriods.Where(x => x.RestMinutes > wt4Parameter.SecondlValue);
					//RestInTrip
					if (wt4Parameter.ThrirdCellValue == (int)RestType.AwayDomicile)
					{
						query = query.Where(x => x.IsInTrip);
					}
					//RestBetweenTrip
					else if (wt4Parameter.ThrirdCellValue == (int)RestType.InDomicile)
					{
						query = query.Where(x => !x.IsInTrip);
					}

					//line.WeightPoints.Rest=-1*( query.ToList().Count * wtRest.Wt);
					line.WeightPoints.Rest =
						-query.Sum(x => (wt4Parameter.SecondlValue - x.RestMinutes) / 60m * wt4Parameter.Weight);

					query = line.RestPeriods.Where(x => x.RestMinutes < wt4Parameter.SecondlValue);
					//RestInTrip
					if (wt4Parameter.ThrirdCellValue == (int)RestType.AwayDomicile)
					{
						query = query.Where(x => x.IsInTrip);
					}
					//RestBetweenTrip
					else if (wt4Parameter.ThrirdCellValue == (int)RestType.InDomicile)
					{
						query = query.Where(x => !x.IsInTrip);
					}
					//line.WeightPoints.Rest += (query.ToList().Count * wtRest.Wt);
					line.WeightPoints.Rest +=
						query.Sum(x => (decimal)(x.RestMinutes - wt4Parameter.SecondlValue) / 60m * wt4Parameter.Weight);
				}
			}
			//-----------------------------------------------------
		}

		#endregion


		private List<DateHelper> GenarateCalender()
		{
			var lstDateHelper = new List<DateHelper>();
			int daycount =
				(GlobalSettings.CurrentBidDetails.BidPeriodEndDate - GlobalSettings.CurrentBidDetails.BidPeriodStartDate)
					.Days + 1;
			DateTime startDate = GlobalSettings.CurrentBidDetails.BidPeriodStartDate;
			for (int count = 1; count <= daycount; count++)
			{
				lstDateHelper.Add(new DateHelper { DateId = count, Date = startDate });
				startDate = startDate.AddDays(1);
			}
			for (int count = daycount + 1, i = 1; count <= 34; count++, i++)
			{
				lstDateHelper.Add(new DateHelper { DateId = count, Date = startDate });
				startDate = startDate.AddDays(1);
			}
			return lstDateHelper;
		}

		private static Trip GetTrip(string pairing)
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
		#endregion
	}
}