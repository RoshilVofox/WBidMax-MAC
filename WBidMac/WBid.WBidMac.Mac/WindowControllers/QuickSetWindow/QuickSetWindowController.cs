
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidMac.Mac
{
	public partial class QuickSetWindowController : AppKit.NSWindowController
	{
		#region Constructors

		NSPanel newPanel;
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		public string selectedCSWQset;
		public string selectedColumnsQset;

		// Called when created from unmanaged code
		public QuickSetWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QuickSetWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public QuickSetWindowController () : base ("QuickSetWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new QuickSetWindow Window {
			get {
				return (QuickSetWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.ShouldCascadeWindows = false;
			try {
				if (GlobalSettings.QuickSets == null) {
					LoadQuickSetFile ();
				}
				
				newPanel = new NSPanel ();
				newPanel.SetContentSize (new CoreGraphics.CGSize (300, 130));
				newPanel.ContentView = vwNewSet;
				
				this.Window.WillClose += delegate {
					this.Window.OrderOut (this);
					//NSApplication.SharedApplication.StopModal ();
				};
				
				SetupButtons ();
				EnableDisableUseDelete ();
				tblCSWQuickSets.Source = new CSWQuickSetTableSource (this);
				tblColumnsQuickSets.Source = new ColumnsQuickSetTableSource (this);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		public void EnableDisableUseDelete ()
		{
			btnCSWUse.Enabled = !string.IsNullOrEmpty (selectedCSWQset);
			btnCSWDelete.Enabled = !string.IsNullOrEmpty (selectedCSWQset);
			btnColumnUse.Enabled = !string.IsNullOrEmpty (selectedColumnsQset);
			btnColumnDelete.Enabled = !string.IsNullOrEmpty (selectedColumnsQset);
		}

		private void LoadQuickSetFile ()
		{
			if (File.Exists (WBidHelper.GetQuickSetFilePath ())) {
				GlobalSettings.QuickSets = XmlHelper.DeserializeFromXml<QuickSets> (WBidHelper.GetQuickSetFilePath ());
			} else {
				GlobalSettings.QuickSets = new QuickSets ();
				GlobalSettings.QuickSets.QuickSetColumn = new List<QuickSetColumn> ();
				GlobalSettings.QuickSets.QuickSetCSW = new List<QuickSetCSW> ();
			}
		}

		private void AddCswDatatoQuickSets (string cSwName)
		{
			try {
				var quickSetCsw = new QuickSetCSW {
					QuickSetCSWName = cSwName,
					Constraints = new Constraints (wBIdStateContent.Constraints),
					Weights = new Weights (wBIdStateContent.Weights),
					SortDetails = new SortDetails (wBIdStateContent.SortDetails),
					CxWtState = new CxWtState (wBIdStateContent.CxWtState)
				};
				
				if (quickSetCsw.CxWtState.SDO.Cx || quickSetCsw.CxWtState.SDO.Wt) {
					var message = "The Following constraints and weights are not included";
				
					if (quickSetCsw.CxWtState.SDO.Cx)
						message += "\r\n Days of month Constraints ";
					if (quickSetCsw.CxWtState.WtPDOFS.Cx)
						message += "\r\n Partial Days Off Constraints ";
					if (quickSetCsw.CxWtState.SDO.Wt)
						message += "\r\n  Days of month Weights ";
					if (quickSetCsw.CxWtState.PDAfter.Wt)
						message += "\r\n  PDO After ";
					if (quickSetCsw.CxWtState.PDBefore.Wt)
						message += "\r\n  PDO Before ";
				
					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = message;
					alert.RunModal ();
				
					quickSetCsw.CxWtState.PDBefore.Wt = false;
					quickSetCsw.CxWtState.PDAfter.Wt = false;
					quickSetCsw.CxWtState.WtPDOFS.Cx = false;
					quickSetCsw.CxWtState.SDO.Cx = false;
					quickSetCsw.CxWtState.SDO.Wt = false;
					quickSetCsw.Constraints.DaysOfMonth.OFFDays = new List<int> ();
					quickSetCsw.Constraints.DaysOfMonth.WorkDays = new List<int> ();
					quickSetCsw.Weights.SDO.isWork = false;
					quickSetCsw.Weights.SDO.Weights = new List<Wt> ();
					quickSetCsw.Weights.PDAfter = new Wt4Parameters {
						FirstValue = 1,
						SecondlValue = 180,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt4Parameter> ()
					};
					quickSetCsw.Weights.PDBefore = new Wt4Parameters {
						FirstValue = 1,
						SecondlValue = 180,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt4Parameter> ()
					};
				}
				
				GlobalSettings.QuickSets.QuickSetCSW = GlobalSettings.QuickSets.QuickSetCSW ?? new List<QuickSetCSW> ();
				GlobalSettings.QuickSets.QuickSetCSW.Add (quickSetCsw);
				XmlHelper.SerializeToXml (GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath ());
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void ApplyCSWDataFromQuickSets (string cSWName)
		{
			try {
				QuickSetCSW quickSetCSW = GlobalSettings.QuickSets.QuickSetCSW.FirstOrDefault (X => X.QuickSetCSWName == cSWName);
                quickSetCSW.CxWtState.Commute.Cx = false;
                quickSetCSW.CxWtState.Commute.Wt = false;

                       quickSetCSW.Constraints.EQUIP.ThirdcellValue = "700";
                        quickSetCSW.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "300" || x.ThirdcellValue == "500");
                        if (quickSetCSW.Constraints.EQUIP.lstParameters.Count < 1)
                            quickSetCSW.CxWtState.EQUIP.Cx = false;
                        quickSetCSW.Weights.EQUIP.SecondlValue = 700;
                        quickSetCSW.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 300 || x.SecondlValue == 500);
                        if (quickSetCSW.Weights.EQUIP.lstParameters.Count < 1)
                            quickSetCSW.CxWtState.EQUIP.Wt = false;
				if (quickSetCSW.CxWtState.ETOPS == null)
					quickSetCSW.CxWtState.ETOPS = new StateStatus { Cx = false, Wt = false };
				if (quickSetCSW.CxWtState.ETOPSRes == null)
					quickSetCSW.CxWtState.ETOPSRes = new StateStatus { Cx = false, Wt = false };
				if (quickSetCSW.Weights.ETOPS == null)
					quickSetCSW.Weights.ETOPS = new Wt1Parameters { lstParameters = new List<Wt1Parameter>() };
				if (quickSetCSW.Weights.ETOPSRes == null)
					quickSetCSW.Weights.ETOPSRes = new Wt1Parameters { lstParameters = new List<Wt1Parameter>() };
				RemoveExpiredCityFromQuickSet(quickSetCSW);
				wBIdStateContent.Constraints = new Constraints (quickSetCSW.Constraints);
				wBIdStateContent.Weights = new Weights (quickSetCSW.Weights);
				wBIdStateContent.CxWtState = new CxWtState (quickSetCSW.CxWtState);
				wBIdStateContent.SortDetails = new SortDetails (quickSetCSW.SortDetails);

				if (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO") {
					wBIdStateContent.CxWtState.FaPosition.A = false;
					wBIdStateContent.CxWtState.FaPosition.B = false;
					wBIdStateContent.CxWtState.FaPosition.C = false;
					wBIdStateContent.CxWtState.FaPosition.D = false;
					wBIdStateContent.CxWtState.Position.Wt = false;
					wBIdStateContent.Weights.POS.lstParameters = null;
				}
				if (wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue == null)
				{
					wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue = "1";
					foreach (var item in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters)
					{
						if (item.SecondcellValue == null)
						{
							item.SecondcellValue = "1";
						}
					}
				}
				StateManagement stateManagement = new StateManagement ();
				stateManagement.ApplyCSW (wBIdStateContent);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void AddColumnDatatoQuickSets (string colName)
		{
			try {
				var quickSetCol = new QuickSetColumn {
					QuickSetColumnName = colName,
					SummaryNormalColumns = new List<DataColumn> (GlobalSettings.WBidINIContent.DataColumns),
					SummaryVacationColumns = new List<DataColumn> (GlobalSettings.WBidINIContent.SummaryVacationColumns),
					BidLineNormalColumns = new List<int> (GlobalSettings.WBidINIContent.BidLineNormalColumns),
					BidLineVacationColumns = new List<int> (GlobalSettings.WBidINIContent.BidLineVacationColumns),
					ModernNormalColumns = new List<int> (GlobalSettings.WBidINIContent.ModernNormalColumns),
					ModernVacationColumns = new List<int> (GlobalSettings.WBidINIContent.ModernVacationColumns)
				};
				GlobalSettings.QuickSets.QuickSetColumn = GlobalSettings.QuickSets.QuickSetColumn ?? new List<QuickSetColumn> ();
				GlobalSettings.QuickSets.QuickSetColumn.Add (quickSetCol);
				XmlHelper.SerializeToXml (GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath ());
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void ApplyColumnsDataFromQuickSets(string colName)
		{
			try
			{
				QuickSetColumn quickSetCol = GlobalSettings.QuickSets.QuickSetColumn.FirstOrDefault(X => X.QuickSetColumnName == colName);

				//remove the legs in 500 and legs in 300 columns
				quickSetCol.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
				quickSetCol.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

				quickSetCol.SummaryNormalColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
				quickSetCol.SummaryNormalColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

				quickSetCol.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);
				quickSetCol.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);

				quickSetCol.BidLineNormalColumns.RemoveAll(x => x == 58 || x == 59);
				quickSetCol.BidLineVacationColumns.RemoveAll(x => x == 58 || x == 59);

				GlobalSettings.WBidINIContent.DataColumns = new List<DataColumn>(quickSetCol.SummaryNormalColumns);
				GlobalSettings.WBidINIContent.SummaryVacationColumns = new List<DataColumn>(quickSetCol.SummaryVacationColumns);
				GlobalSettings.WBidINIContent.BidLineNormalColumns = new List<int>(quickSetCol.BidLineNormalColumns);
				GlobalSettings.WBidINIContent.BidLineVacationColumns = new List<int>(quickSetCol.BidLineVacationColumns);
				GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int>(quickSetCol.ModernNormalColumns);
				GlobalSettings.WBidINIContent.ModernVacationColumns = new List<int>(quickSetCol.ModernVacationColumns);

				WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			}

		}
		/// <summary>
		/// If the City in the Quick set is not available in the inI file,we have to delete those item from the quick set
		/// </summary>
		/// <param name="quickSetCSW"></param>
		private static void RemoveExpiredCityFromQuickSet(QuickSetCSW quickSetCSW)
		{
			try{
			//International Non Conus
			bool isNeedToSaVeQuicKset = false;
			List<Wt2Parameter> WtItemsToRemove = new List<Wt2Parameter>();
			foreach (var item in quickSetCSW.Weights.InterConus.lstParameters)
			{
				if (item.Type != 0 && item.Type != -1)
				{
					var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(
						x => (x.International || x.NonConus) && x.Id == item.Type);
					if (city == null)
					{
						
						var alert = new NSAlert ();
						alert.MessageText = "WBidMax";
						alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those International and Non conus Weight item from the Quick set. ";
						alert.RunModal ();

						WtItemsToRemove.Add(item);
					}
				}
			}
			foreach (var item in WtItemsToRemove)
			{
				quickSetCSW.Weights.InterConus.lstParameters.Remove(item);
				isNeedToSaVeQuicKset = true;
			}
			//=====================================
			List<Cx2Parameter> CxItemsToRemove = new List<Cx2Parameter>();
			foreach (var item in quickSetCSW.Constraints.InterConus.lstParameters)
			{
				if (item.Type != 0 && item.Type != -1)
				{
					var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(
						x => (x.International || x.NonConus) && x.Id == item.Value);
					if (city == null)
					{
						
						var alert = new NSAlert ();
						alert.MessageText = "WBidMax";
						alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those International and Non conus Constrain item from the Quick set. ";
						alert.RunModal ();
						CxItemsToRemove.Add(item);
					}
				}
			}
			foreach (var item in CxItemsToRemove)
			{
				quickSetCSW.Constraints.InterConus.lstParameters.Remove(item);
				isNeedToSaVeQuicKset = true;
			}

			//==========================================
			//Overnight City
			List<Cx3Parameter> CxOVCityItemsToRemove = new List<Cx3Parameter>();
			foreach (var item in quickSetCSW.Constraints.OverNightCities.lstParameters)
			{
				if (item.Type != 0 && item.Type != -1)
				{
					var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == Convert.ToInt32(item.ThirdcellValue));
					if (city == null)
					{
						
						var alert = new NSAlert ();
						alert.MessageText = "WBidMax";
						alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those Overnight City Constrain item from the Quick set. ";
						alert.RunModal ();
						CxOVCityItemsToRemove.Add(item);
					}
				}
			}
			foreach (var item in CxOVCityItemsToRemove)
			{
				quickSetCSW.Constraints.OverNightCities.lstParameters.Remove(item);
				isNeedToSaVeQuicKset = true;
			}

			List<Wt2Parameter> WtOVCityItemsToRemove = new List<Wt2Parameter>();
			foreach (var item in quickSetCSW.Weights.RON.lstParameters)
			{
				if (item.Type != 0 && item.Type != -1)
				{
					var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == item.Type);
					if (city == null)
					{

						var alert = new NSAlert ();
						alert.MessageText = "WBidMax";
						alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those Overnight City Weight item from the Quick set.";
						alert.RunModal ();
						WtOVCityItemsToRemove.Add(item);
					}
				}
			}
			foreach (var item in WtOVCityItemsToRemove)
			{
				quickSetCSW.Weights.RON.lstParameters.Remove(item);
				isNeedToSaVeQuicKset = true;
			}
			//==========================================

			//Overnight City BulK CONSTRAINTS

			foreach (var item in quickSetCSW.Constraints.BulkOvernightCity.OverNightNo)
			{

				var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == item);
				if (city == null)
				{

					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those Overnight City bulk Constrain item from the Quick set. ";
					alert.RunModal ();
					quickSetCSW.Constraints.BulkOvernightCity.OverNightNo.Remove(city.Id);
					isNeedToSaVeQuicKset = true;
				}
			}
			foreach (var item in quickSetCSW.Constraints.BulkOvernightCity.OverNightYes)
			{

				var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == item);
				if (city == null)
				{

					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those Overnight City bulk Constrain item from the Quick set. ";
					alert.RunModal ();
					quickSetCSW.Constraints.BulkOvernightCity.OverNightYes.Remove(city.Id);
					isNeedToSaVeQuicKset = true;
				}
			}
			//Overnight City wEIGHTS.
			List<Wt2Parameter> WtOVCityBulkItemsToRemove = new List<Wt2Parameter>();
			foreach (var item in quickSetCSW.Weights.OvernightCitybulk)
			{

				var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == item.Type);
				if (city == null)
				{
					
					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those Overnight City bulk Weight item from the Quick set. ";
					alert.RunModal ();
					WtOVCityBulkItemsToRemove.Add(item);
				}
			}

			foreach (var item in WtOVCityBulkItemsToRemove)
			{
				quickSetCSW.Weights.OvernightCitybulk.Remove(item);
				isNeedToSaVeQuicKset = true;
			}

			//Partial Days OfF
			List<Cx4Parameter> cxPDOCityBulkItemsToRemove = new List<Cx4Parameter>();
			foreach (var item in quickSetCSW.Constraints.PDOFS.LstParameter)
			{

				var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id ==Convert.ToInt32(item.ThirdcellValue));
				if (city == null)
				{
					
					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those Partial Days OFF constraints item from the Quick set. ";
					alert.RunModal ();
					cxPDOCityBulkItemsToRemove.Add(item);
				}
			}

			foreach (var item in cxPDOCityBulkItemsToRemove)
			{
				quickSetCSW.Constraints.PDOFS.LstParameter.Remove(item);
				isNeedToSaVeQuicKset = true;
			}
			//PDO after
			List<Wt4Parameter> WtPDOCityBulkItemsToRemove = new List<Wt4Parameter>();
			foreach (var item in quickSetCSW.Weights.PDAfter.lstParameters)
			{

				var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == Convert.ToInt32(item.ThrirdCellValue));
				if (city == null)
				{
					
					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those PDO after weight item from the Quick set. ";
					alert.RunModal ();
					WtPDOCityBulkItemsToRemove.Add(item);
				}
			}

			foreach (var item in WtPDOCityBulkItemsToRemove)
			{
				quickSetCSW.Weights.PDAfter.lstParameters.Remove(item);
				isNeedToSaVeQuicKset = true;
			}

			//PDO Before
			List<Wt4Parameter> WtPDOBeforeCityBulkItemsToRemove = new List<Wt4Parameter>();
			foreach (var item in quickSetCSW.Weights.PDBefore.lstParameters)
			{

				var city = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Id == Convert.ToInt32(item.ThrirdCellValue));
				if (city == null)
				{
					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those PDO after weight item from the Quick set. ";
					alert.RunModal ();
					WtPDOBeforeCityBulkItemsToRemove.Add(item);
				}
			}

			foreach (var item in WtPDOBeforeCityBulkItemsToRemove)
			{
				quickSetCSW.Weights.PDBefore.lstParameters.Remove(item);
				isNeedToSaVeQuicKset = true;
			}

			// Commutable Line Auto
			if (quickSetCSW.Constraints.CLAuto != null && quickSetCSW.Constraints.CLAuto.City!=null)
			{
				var city1 = GlobalSettings.WBidINIContent.Cities.FirstOrDefault(x => x.Name == quickSetCSW.Constraints.CLAuto.City);
				if (city1 == null)
				{
					
					var alert = new NSAlert ();
					alert.MessageText = "WBidMax";
					alert.InformativeText = "You have cities in your Quicksets that Southwest Airlines no longer services and this will delete those Commutable line auto Constrain item from the Quick set. ";
					alert.RunModal ();
					quickSetCSW.CxWtState.CLAuto.Cx = false;
					quickSetCSW.Constraints.CLAuto = new FtCommutableLine () {ToHome = true,
						ToWork = false,
						NoNights = false,
						BaseTime = 10,
						ConnectTime = 30,
						CheckInTime = 60
					};
				}
			}

			if (isNeedToSaVeQuicKset)
			{
				XmlHelper.SerializeToXml (GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath ());
				//WBidCollection.CreateQuickSets(GlobalSettings.QuickSets);
			}
			}catch(Exception ex) {
			}
		}
		void SetupButtons ()
		{
			// CSW Tab
			btnCSWNew.Activated += delegate {
				lblNewName.StringValue = "Enter a Name for CSW QuickSet";
				txtNewName.StringValue = string.Empty;
				NSApplication.SharedApplication.BeginSheet (newPanel, this.Window);
			};
			btnCSWUse.Activated += delegate {
				try {
					WBidHelper.PushToUndoStack ();
					ApplyCSWDataFromQuickSets (selectedCSWQset);
					CommonClass.MainController.ReloadAllContent ();
					if (CommonClass.CSWController != null)
						CommonClass.CSWController.ReloadAllContent ();
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			btnCSWDelete.Activated += delegate {
				try {
					var alert = new NSAlert ();
					alert.MessageText = "Are you sure you want to delete \"" + selectedCSWQset + "\" from QuickSet?";
					alert.AddButton ("NO");
					alert.AddButton ("YES");
					alert.Buttons [1].Activated += delegate {
						NSApplication.SharedApplication.EndSheet (alert.Window);
						alert.Window.OrderOut (this);
						GlobalSettings.QuickSets.QuickSetCSW.RemoveAll (x => x.QuickSetCSWName == selectedCSWQset);
                        GlobalSettings.QuickSets.IsModified = true; //added by roshil
                        GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
						XmlHelper.SerializeToXml (GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath ());
						tblCSWQuickSets.ReloadData ();
						selectedCSWQset = string.Empty;
						EnableDisableUseDelete ();
					};
					alert.RunSheetModal (this.Window);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			// Columns Tab
			btnColumnNew.Activated += delegate {
				lblNewName.StringValue = "Enter a Name for Columns QuickSet";
				txtNewName.StringValue = string.Empty;
				NSApplication.SharedApplication.BeginSheet (newPanel, this.Window);
			};
			btnColumnUse.Activated += delegate {
				try {
					ApplyColumnsDataFromQuickSets (selectedColumnsQset);
					CommonClass.bidLineProperties.Clear ();
					CommonClass.modernProperties.Clear ();
					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
						foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns) {
							var col = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
							if (col != null) {
								CommonClass.bidLineProperties.Add (col.DisplayName);
							}
						}
						foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns) {
							var col = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
							if (col != null) {
								CommonClass.modernProperties.Add (col.DisplayName);
							}
						}
						LineSummaryBL.GetBidlineViewAdditionalVacationColumns ();
						LineSummaryBL.SetSelectedBidLineVacationColumnstoGlobalList ();
						LineSummaryBL.GetModernViewAdditionalVacationalColumns ();
						LineSummaryBL.SetSelectedModernBidLineVacationColumnstoGlobalList ();
					} else {
						foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns) {
							var col = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault (x => x.Id == item);
							if (col != null) {
								CommonClass.bidLineProperties.Add (col.DisplayName);
							}
						}
						foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns) {
							var col = GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.Id == item);
							if (col != null) {
								CommonClass.modernProperties.Add (col.DisplayName);
							}
						}
						LineSummaryBL.GetBidlineViewAdditionalColumns ();
						LineSummaryBL.SetSelectedBidLineColumnstoGlobalList ();
						LineSummaryBL.GetModernViewAdditionalColumns ();
						LineSummaryBL.SetSelectedModernBidLineColumnstoGlobalList ();
					}
					CommonClass.MainController.ReloadAllContent ();
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			btnColumnDelete.Activated += delegate {
				try {
					var alert = new NSAlert ();
					alert.MessageText = "Are you sure you want to delete \"" + selectedColumnsQset + "\" from QuickSet?";
					alert.AddButton ("NO");
					alert.AddButton ("YES");
					alert.Buttons [1].Activated += delegate {
						NSApplication.SharedApplication.EndSheet (alert.Window);
						alert.Window.OrderOut (this);
						GlobalSettings.QuickSets.QuickSetColumn.RemoveAll (x => x.QuickSetColumnName == selectedColumnsQset);
						XmlHelper.SerializeToXml (GlobalSettings.QuickSets, WBidHelper.GetQuickSetFilePath ());
                        GlobalSettings.QuickSets.IsModified = true;
                        GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
						tblColumnsQuickSets.ReloadData ();
						selectedColumnsQset = string.Empty;
						EnableDisableUseDelete ();
					};
					alert.RunSheetModal (this.Window);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}

			};

			// New View
			btnNewOK.Activated += delegate {
				try {
					var name = txtNewName.StringValue;
					if (!string.IsNullOrEmpty (name.Trim ())) {
						if (vwTabQuickSets.Selected.Label == "CSW") {
							// CSW Add
							if (GlobalSettings.QuickSets.QuickSetCSW.Count (x => x.QuickSetCSWName == name) == 0) {
								AddCswDatatoQuickSets (name);
								tblCSWQuickSets.ReloadData ();
								NSApplication.SharedApplication.EndSheet (newPanel);
                                GlobalSettings.QuickSets.IsModified = true;
                                GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
								newPanel.OrderOut (this);
							} else {
								var alert = new NSAlert ();
								alert.MessageText = "WBidMax";
								alert.InformativeText = name + " already exists. Please choose another name!";
								alert.RunModal ();
							}
						} else {
							// Columns Add
							if (GlobalSettings.QuickSets.QuickSetColumn.Count (x => x.QuickSetColumnName == name) == 0) {
								AddColumnDatatoQuickSets (name);
								tblColumnsQuickSets.ReloadData ();
                                GlobalSettings.QuickSets.IsModified = true;
                                GlobalSettings.QuickSets.QuickSetUpdatedTime = DateTime.Now.ToUniversalTime();
								NSApplication.SharedApplication.EndSheet (newPanel);
								newPanel.OrderOut (this);
							} else {
								var alert = new NSAlert ();
								alert.MessageText = "WBidMax";
								alert.InformativeText = name + " already exists. Please choose another name!";
								alert.RunModal ();
							}
						}
					}
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			btnNewCancel.Activated += delegate {
				NSApplication.SharedApplication.EndSheet (newPanel);
				newPanel.OrderOut (this);
			};

		}

		public partial class CSWQuickSetTableSource : NSTableViewSource
		{
			QuickSetWindowController parentVC;

			public CSWQuickSetTableSource (QuickSetWindowController parent)
			{
				parentVC = parent;
			}

			public override nint GetRowCount (NSTableView tableView)
			{
				return GlobalSettings.QuickSets.QuickSetCSW.Count;
			}

			public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var vw = (QuickSetCell)tableView.MakeView ("CSWQs", this);
				try {
					vw.BindData (0, GlobalSettings.QuickSets.QuickSetCSW [(int)row].QuickSetCSWName, (int)row);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
				return vw;
			}

			public override void SelectionDidChange (NSNotification notification)
			{
				var table = (NSTableView)notification.Object;
				parentVC.selectedCSWQset = (table.SelectedRowCount > 0) ? GlobalSettings.QuickSets.QuickSetCSW [(int)table.SelectedRow].QuickSetCSWName : string.Empty;
				parentVC.EnableDisableUseDelete ();
			}
		}

		public partial class ColumnsQuickSetTableSource : NSTableViewSource
		{
			QuickSetWindowController parentVC;

			public ColumnsQuickSetTableSource (QuickSetWindowController parent)
			{
				parentVC = parent;
			}

			public override nint GetRowCount (NSTableView tableView)
			{
				return GlobalSettings.QuickSets.QuickSetColumn.Count;
			}

			public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
			{
				var vw = (QuickSetCell)tableView.MakeView ("ColumnsQs", this);
				try {
					vw.BindData (1, GlobalSettings.QuickSets.QuickSetColumn [(int)row].QuickSetColumnName, (int)row);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
				return vw;
			}

			public override void SelectionDidChange (NSNotification notification)
			{
				var table = (NSTableView)notification.Object;
				parentVC.selectedColumnsQset = (table.SelectedRowCount > 0) ? GlobalSettings.QuickSets.QuickSetColumn [(int)table.SelectedRow].QuickSetColumnName : string.Empty;
				parentVC.EnableDisableUseDelete ();
			}
		}

	}
}

