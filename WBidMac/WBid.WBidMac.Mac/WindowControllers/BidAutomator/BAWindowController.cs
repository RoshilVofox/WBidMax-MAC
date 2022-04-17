
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using CoreGraphics;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Model;

namespace WBid.WBidMac.Mac
{
	public partial class BAWindowController : AppKit.NSWindowController
	{
		#region Constructors

		BAFilterViewController filterVC;
        BASortsViewController sortVC;
		WeightsViewController weightVC;
		WBidState wBIdStateContent;
		int PreviousSegment;
		// Called when created from unmanaged code
		public BAWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BAWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BAWindowController () : base ("BAWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		partial void funCalculateBid (NSObject sender)
		{
			//Clear top lock and bottom lock and line property for constraint
			bool flag1=false;
			bool flag2=false;
			wBIdStateContent=GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if(wBIdStateContent.BidAuto.BAFilter.Count>0)
			{
				flag1=true;
			}

			if ((wBIdStateContent.BidAuto.BASort != null) && ((wBIdStateContent.BidAuto.BASort.BlokSort!=null && wBIdStateContent.BidAuto.BASort.BlokSort.Count>0) || (wBIdStateContent.BidAuto.BASort.SortColumn!=null &&  wBIdStateContent.BidAuto.BASort.SortColumn.Length >0)))
			{
				flag2=true;
			}

			if(!flag1 || !flag2)
			{
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Bid Automator feature required atleast one filter and Sort";
				//alert.InformativeText = "A New Version " + version + " of WBidMax exists for Mac.";
				alert.AddButton ("Ok");
				alert.RunModal();


				return;
			}
			GlobalSettings.Lines.ToList().ForEach(x =>
				{
					x.TopLock = false;
					x.BotLock = false;
					//x.WeightPoints.Reset();
					if (x.BAFilters != null)
						x.BAFilters.Clear();
					x.BAGroup = string.Empty;
					x.IsGrpColorOn = 0;
				});
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAGroup != null)
			{
				wBidStateContent.BidAuto.BAGroup.Clear();
			}

			
			BidAutomatorCalculations bidautocalculations=new BidAutomatorCalculations();
			bidautocalculations.CalculateLinePropertiesForBAFilters();
			bidautocalculations.ApplyBAFilterAndSort();
			if (wBidStateContent.BidAuto != null && wBidStateContent.BidAuto.BAFilter != null)
				wBidStateContent.BidAuto.BAFilter.ForEach(x => x.IsApplied = true);


			//Setting Bid Automator settings to CalculatedBA state
			SetCurrentBADetailsToCalculateBAState();
		
			//CommonClass.MainController.ReloadAllContent ();
			CommonClass.FilterController.ReloadContent();
			if (CommonClass.SummaryController != null)
			{
				CommonClass.SummaryController.TableViewReload();
			}
			if (CommonClass.ModernController != null)
			{
				CommonClass.ModernController.ReloadContent();
			}
			if (CommonClass.BidLineController != null)
			{
				CommonClass.BidLineController.ReloadContent();
			}

			wBidStateContent.BidAutoOn=true;
		}

		/// <summary>
		/// Setting Bid Automator settings to CalculatedBA state
		/// </summary>
		private void SetCurrentBADetailsToCalculateBAState()
		{
			try
			{
				WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				if (wBIdStateContent.BidAuto != null)
				{
					wBIdStateContent.CalculatedBA = new BidAutomator
					{
						IsBlankBottom = wBIdStateContent.BidAuto.IsBlankBottom,
						IsReserveBottom = wBIdStateContent.BidAuto.IsReserveBottom,
						IsReserveFirst = wBIdStateContent.BidAuto.IsReserveFirst
					};


					//Ba filter
					//---------------------------------------------------------------------------
					if (wBIdStateContent.BidAuto.BAFilter != null)
					{
						wBIdStateContent.CalculatedBA.BAFilter = new List<BidAutoItem>();
						SetCurrentBAFilterDetailsToCalculateBAFilterState();
					}
					//---------------------------------------------------------------------------
					//Ba Group object
					//---------------------------------------------------------------------------
					if (wBIdStateContent.BidAuto.BAGroup != null)
					{
						wBIdStateContent.CalculatedBA.BAGroup = new List<BidAutoGroup>();
						SetCurrentBAGroupDetailsToCalculateBAGroupState();

						// GlobalSettings.WBidStateContent.BidAuto.BAGroup = null;
					}
					//---------------------------------------------------------------------------

					//Sort object
					//---------------------------------------------------------------------------
					if (wBIdStateContent.BidAuto.BASort != null)
					{
						wBIdStateContent.CalculatedBA.BASort = new SortDetails
						{
							SortColumn = wBIdStateContent.BidAuto.BASort.SortColumn,
							SortColumnName = wBIdStateContent.BidAuto.BASort.SortColumnName,
							SortDirection = wBIdStateContent.BidAuto.BASort.SortDirection
						};
						//Block sort list
						// if (GlobalSettings.WBidStateContent.CalculatedBA.BASort.BlokSort != null)
						if (wBIdStateContent.BidAuto.BASort.BlokSort != null)
						{
							wBIdStateContent.CalculatedBA.BASort.BlokSort = new List<string>();
							foreach (var item in wBIdStateContent.BidAuto.BASort.BlokSort)
							{
								wBIdStateContent.CalculatedBA.BASort.BlokSort.Add(item);
							}
						}

					}
					//---------------------------------------------------------------------------




				}
			}
			catch (Exception ex)
			{

			}
		}
		private void SetCurrentBAFilterDetailsToCalculateBAFilterState()
		{
			try
			{
				foreach (var item in wBIdStateContent.BidAuto.BAFilter)
				{
					var calculatedItem = new BidAutoItem();
					calculatedItem.Name = item.Name;
					calculatedItem.Priority = item.Priority;
					calculatedItem.IsApplied = item.IsApplied;

					SetAutoObjectValueToCalculateBAFilter(item, calculatedItem);
					wBIdStateContent.CalculatedBA.BAFilter.Add(calculatedItem);
				}

			}
			catch (Exception ex)
			{

			}

		}

		private void SetCurrentBAGroupDetailsToCalculateBAGroupState()
		{

			foreach (var item in wBIdStateContent.BidAuto.BAGroup)
			{
				wBIdStateContent.CalculatedBA.BAGroup.Add(new BidAutoGroup { GroupName = item.GroupName, Lines = item.Lines });
			}

		}
		private void SetAutoObjectValueToCalculateBAFilter(BidAutoItem item, BidAutoItem calculatedItem)
		{
			try
			{
				Cx3Parameter cx3Parameter;
				Cx3Parameter calculateCx3Parameter;
				CxDays cxDay;
				CxDays calculateCxDays;
				switch (calculatedItem.Name)
				{

				//-----------------------------------------------------------------------
				case "AP":
					var amPmConstriants = (AMPMConstriants)item.BidAutoObject;
					var calculateAmPmConstriants = new AMPMConstriants
					{
						AM = amPmConstriants.AM,
						MIX = amPmConstriants.MIX,
						PM = amPmConstriants.PM
					};
					calculatedItem.BidAutoObject = calculateAmPmConstriants;
					break;
					//-----------------------------------------------------------------------
				case "CL":
					var ftCommutableLine = (FtCommutableLine)item.BidAutoObject;
					var calculateFtCommutableLine = new FtCommutableLine
					{
						BaseTime = ftCommutableLine.BaseTime,
						CheckInTime = ftCommutableLine.CheckInTime,
						City = ftCommutableLine.City,
						CommuteCity = ftCommutableLine.CommuteCity,
						ConnectTime = ftCommutableLine.ConnectTime,
						NoNights = ftCommutableLine.NoNights,
						ToHome = ftCommutableLine.NoNights,
						ToWork = ftCommutableLine.ToWork,
						IsNonStopOnly=ftCommutableLine.IsNonStopOnly
					};
					calculatedItem.BidAutoObject = calculateFtCommutableLine;
					break;
					//-----------------------------------------------------------------------
				case "DOM":
					var daysOfMonthCx = (DaysOfMonthCx)item.BidAutoObject;
					var calculateDaysOfMonthCx = new DaysOfMonthCx();
					if (daysOfMonthCx.OFFDays != null)
					{
						calculateDaysOfMonthCx.OFFDays = new List<int>();
						foreach (var offDay in daysOfMonthCx.OFFDays)
						{
							calculateDaysOfMonthCx.OFFDays.Add(offDay);
						}
					}
					if (daysOfMonthCx.WorkDays != null)
					{
						calculateDaysOfMonthCx.WorkDays = new List<int>();
						foreach (var workDay in daysOfMonthCx.WorkDays)
						{
							calculateDaysOfMonthCx.WorkDays.Add(workDay);
						}
					}
					calculatedItem.BidAutoObject = calculateDaysOfMonthCx;
					break;
					//-----------------------------------------------------------------------
				case "DOWA":
					cxDay = (CxDays)item.BidAutoObject;
					calculateCxDays = new CxDays
					{
						IsFri = cxDay.IsFri,
						IsMon = cxDay.IsMon,
						IsSat = cxDay.IsSat,
						IsSun = cxDay.IsSun,
						IsThu = cxDay.IsThu,
						IsTue = cxDay.IsTue,
						IsWed = cxDay.IsWed
					};
					calculatedItem.BidAutoObject = calculateCxDays;
					break;
					//-----------------------------------------------------------------------
				case "DOWS":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "DHFL":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "ET":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "LT":
					var lineTypeItem = (CxLine)item.BidAutoObject;
					var calculateCxLine = new CxLine
					{
						Blank = lineTypeItem.Blank,
						Hard = lineTypeItem.Hard,
						International = lineTypeItem.International,
						NonConus = lineTypeItem.NonConus,
						Ready = lineTypeItem.Ready,
						Reserve = lineTypeItem.Reserve
					};
					calculatedItem.BidAutoObject = calculateCxLine;
					break;
					//-----------------------------------------------------------------------
				case "OC":

					var bulkOvernightCityCx = (BulkOvernightCityCx)item.BidAutoObject;
					var calculateBulkOvernightCityCx = new BulkOvernightCityCx();
					if (bulkOvernightCityCx.OverNightNo != null)
					{
						calculateBulkOvernightCityCx.OverNightNo = new List<int>();
						foreach (var overNightNo in bulkOvernightCityCx.OverNightNo)
						{
							calculateBulkOvernightCityCx.OverNightNo.Add(overNightNo);
						}
					}
					if (bulkOvernightCityCx.OverNightYes != null)
					{
						calculateBulkOvernightCityCx.OverNightYes = new List<int>();
						foreach (var overNightYes in bulkOvernightCityCx.OverNightYes)
						{
							calculateBulkOvernightCityCx.OverNightYes.Add(overNightYes);
						}
					}
					calculatedItem.BidAutoObject = calculateBulkOvernightCityCx;
					break;
					//-----------------------------------------------------------------------
				case "RT":
					cx3Parameter = (Cx3Parameter)item.BidAutoObject;
					calculateCx3Parameter = new Cx3Parameter
					{
						ThirdcellValue = cx3Parameter.ThirdcellValue,
						Type = cx3Parameter.Type,
						Value = cx3Parameter.Value
					};
					calculatedItem.BidAutoObject = calculateCx3Parameter;
					break;
					//-----------------------------------------------------------------------
				case "SDOW":
					cxDay = (CxDays)item.BidAutoObject;
					calculateCxDays = new CxDays
					{
						IsFri = cxDay.IsFri,
						IsMon = cxDay.IsMon,
						IsSat = cxDay.IsSat,
						IsSun = cxDay.IsSun,
						IsThu = cxDay.IsThu,
						IsTue = cxDay.IsTue,
						IsWed = cxDay.IsWed
					};
					calculatedItem.BidAutoObject = calculateCxDays;
					break;
					//-----------------------------------------------------------------------
				case "TBL":
					var tripBlockLengthItem = (CxTripBlockLength)item.BidAutoObject;
					var calculateCxTripBlockLength = new CxTripBlockLength
					{
						FourDay = tripBlockLengthItem.FourDay,
						IsBlock = tripBlockLengthItem.IsBlock,
						ThreeDay = tripBlockLengthItem.ThreeDay,
						Turns = tripBlockLengthItem.Turns,
						Twoday = tripBlockLengthItem.Twoday
					};
					calculatedItem.BidAutoObject = calculateCxTripBlockLength;
					break;
					//-----------------------------------------------------------------------


				}
			}
			catch (Exception ex)
			{

			}


		}
		#endregion

		//strongly typed window accessor
		public new BAWindow Window {
			get {
				return (BAWindow)base.Window;
			}
		}
		static NSButton closeButton;
		public override void AwakeFromNib ()
		{
			try {
				 wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				base.AwakeFromNib ();
				ScreenSizeManagement();
				this.ShouldCascadeWindows = false;
				sgViewSelect.Activated += (object sender, EventArgs e) => {
					ChangeView ();
				};

				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					SetScreenSize();
					this.Window.Close ();
				};
				ChangeView ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		public void ResetSortSet()
		{
			
		}
		void ScreenSizeManagement()
		{

			if (GlobalSettings.WBidINIContent.MainWindowSize.IsMaximised == true) {
				this.Window.IsZoomed = true;
			} else {
				if (GlobalSettings.WBidINIContent.CSWViewSize.Height > 0) {
					CGRect ScreenFrame = new CGRect (GlobalSettings.WBidINIContent.CSWViewSize.Left, GlobalSettings.WBidINIContent.CSWViewSize.Top, GlobalSettings.WBidINIContent.CSWViewSize.Width, GlobalSettings.WBidINIContent.CSWViewSize.Height);
					this.Window.SetFrame (ScreenFrame, true);

				} else {

					SetScreenSize ();

				}
			}
		}
		void SetScreenSize()
		{
			GlobalSettings.WBidINIContent.CSWViewSize.Left =(int) this.Window.Frame.X;
			GlobalSettings.WBidINIContent.CSWViewSize.Top = (int)this.Window.Frame.Y;
			GlobalSettings.WBidINIContent.CSWViewSize.Width = (int)this.Window.Frame.Width;
			GlobalSettings.WBidINIContent.CSWViewSize.Height = (int)this.Window.Frame.Height;
			GlobalSettings.WBidINIContent.CSWViewSize.IsMaximised = this.Window.IsZoomed;	
			//save the state of the INI File
			WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

		}
		public void ChangeView ()
		{
			
			try {
				
				if (sgViewSelect.SelectedSegment == 0) {
					if (filterVC == null)
						filterVC = new BAFilterViewController ();
					CommonClass.FilterController = filterVC;
					this.Window.ContentView = filterVC.View;
					PreviousSegment = (int)sgViewSelect.SelectedSegment;
				//	filterVC.tblConstraints.ReloadData ();
				} else if (sgViewSelect.SelectedSegment == 1) {
					if (sortVC == null)
                        sortVC = new BASortsViewController ();
                    CommonClass.BASortController = sortVC;
					this.Window.ContentView = sortVC.View;
					PreviousSegment =  (int)sgViewSelect.SelectedSegment;
				} else if (sgViewSelect.SelectedSegment == 2) {
					
					NSAlert alert= new NSAlert();
					alert.AlertStyle = NSAlertStyle.Warning;
					alert.MessageText = "WBidMax";
					alert.InformativeText = "the feature is coming soon";
					alert.AddButton("OK");
					alert.RunModal ();
					sgViewSelect.SelectSegment((nint)PreviousSegment);
//					return;
//
//					if (weightVC == null)
//						weightVC = new WeightsViewController ();
//					CommonClass.WeightsController = weightVC;
//					this.Window.ContentView = weightVC.View;
//					weightVC.tblWeights.ReloadData ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		public void ReloadAllContent ()
		{
			
			try {
				filterVC = null;
				sortVC = null;
				weightVC = null;

				if (sgViewSelect.SelectedSegment == 0) {
					PreviousSegment = (int)sgViewSelect.SelectedSegment;
					filterVC = new BAFilterViewController ();
					CommonClass.FilterController = filterVC;
					this.Window.ContentView = filterVC.View;
				} else if (sgViewSelect.SelectedSegment == 1) {
					PreviousSegment = (int)sgViewSelect.SelectedSegment;
                    sortVC = new BASortsViewController ();
                    CommonClass.BASortController = sortVC;
					this.Window.ContentView = sortVC.View;
				} else if (sgViewSelect.SelectedSegment == 2) {
					weightVC = new WeightsViewController ();
					CommonClass.WeightsController = weightVC;
					this.Window.ContentView = weightVC.View;
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


	}

	//	public static class Extensions {
	//		public static string LocalValue (this NSDate date) {
	//			var d = DateTime.Parse (date.ToString());
	//			var convertedTime = TimeZoneInfo.ConvertTime (d, TimeZoneInfo.Local);
	//			return convertedTime.ToString ("HH:mm");
	//		}
	//	}
}

