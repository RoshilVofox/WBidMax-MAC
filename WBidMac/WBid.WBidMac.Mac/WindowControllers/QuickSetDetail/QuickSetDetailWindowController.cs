
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using System.Collections.ObjectModel;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;

namespace WBid.WBidMac.Mac
{
	public partial class QuickSetDetailWindowController : AppKit.NSWindowController
	{
		#region Constructors

		public string QSCSWName = null;
		public string QSColumnsName = null;
		public List <QSDetailData> detailList = new  List<QSDetailData> ();

		// Called when created from unmanaged code
		public QuickSetDetailWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public QuickSetDetailWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public QuickSetDetailWindowController () : base ("QuickSetDetailWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new QuickSetDetailWindow Window {
			get {
				return (QuickSetDetailWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.ShouldCascadeWindows = false;
			this.Window.WillClose += delegate {
				this.Window.OrderOut (this);
				NSApplication.SharedApplication.StopModal ();
			};

			detailList = GetDataList ();
			tblQSDetail.Source = new QSDetailSource (this);
		}

		private List<QSDetailData> GetDataList()
		{
			var AllAppliedStates = new ObservableCollection<AppliedStates>();
			var lstData = new List<QSDetailData>();

			try
			{
				if (!string.IsNullOrEmpty(QSColumnsName))
				{
					var QuickSetColumn = GlobalSettings.QuickSets.QuickSetColumn.FirstOrDefault(x => x.QuickSetColumnName == QSColumnsName);
					//remove the legs in 500 and legs in 300 columns
					QuickSetColumn.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
					QuickSetColumn.SummaryVacationColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

					QuickSetColumn.SummaryNormalColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);
					QuickSetColumn.SummaryNormalColumns.RemoveAll(x => x.Id == 58 || x.Id == 59);

					QuickSetColumn.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);
					QuickSetColumn.ModernNormalColumns.RemoveAll(x => x == 58 || x == 59);

					QuickSetColumn.BidLineNormalColumns.RemoveAll(x => x == 58 || x == 59);
					QuickSetColumn.BidLineVacationColumns.RemoveAll(x => x == 58 || x == 59);

					QuicksetStateDetails quicksetStateDetails = new QuicksetStateDetails();
					AllAppliedStates = quicksetStateDetails.GetSelectedColumns(QuickSetColumn);

					foreach (var states in AllAppliedStates)
					{
						lstData.Add(new QSDetailData() { Type = 0, Title = states.Key, DataValue = "", RowHeight = 20 });
						foreach (var stateTypes in states.AppliedStateTypes)
						{
							var datValue = string.Empty;
							if (stateTypes.Value != null)
							{
								foreach (var value in stateTypes.Value)
								{
									datValue += value + "\n";
								}
							}
							lstData.Add(new QSDetailData()
							{
								Type = 1,
								Title = stateTypes.Key,
								DataValue = datValue,
								RowHeight = (stateTypes.Value != null && stateTypes.Value.Count > 0) ? stateTypes.Value.Count * 20 : 20
							});
						}
					}

				}
				else
				{
					var QuickSetCSW = GlobalSettings.QuickSets.QuickSetCSW.FirstOrDefault(x => x.QuickSetCSWName == QSCSWName);

					QuickSetCSW.Constraints.EQUIP.ThirdcellValue = "700";
					QuickSetCSW.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "300" || x.ThirdcellValue == "500");
					if (QuickSetCSW.Constraints.EQUIP.lstParameters.Count < 1)
						QuickSetCSW.CxWtState.EQUIP.Cx = false;
					QuickSetCSW.Weights.EQUIP.SecondlValue = 700;
					QuickSetCSW.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 300 || x.SecondlValue == 500);
					if (QuickSetCSW.Weights.EQUIP.lstParameters.Count < 1)
						QuickSetCSW.CxWtState.EQUIP.Wt = false;
					RemoveExpiredCityFromQuickSet(QuickSetCSW);
					//Remove500Equipement(QuickSetCSW);

					QuicksetStateDetails quicksetStateDetails = new QuicksetStateDetails();
					AllAppliedStates = quicksetStateDetails.GetAppliedState(QuickSetCSW);

					foreach (var states in AllAppliedStates)
					{
						lstData.Add(new QSDetailData() { Type = 0, Title = states.Key, DataValue = "", RowHeight = 20 });
						foreach (var stateTypes in states.AppliedStateTypes)
						{
							var datValue = string.Empty;
							if (stateTypes.Value != null)
							{
								foreach (var value in stateTypes.Value)
								{
									datValue += value + "\n";
								}
							}
							lstData.Add(new QSDetailData()
							{
								Type = 1,
								Title = stateTypes.Key,
								DataValue = datValue,
								RowHeight = (stateTypes.Value != null && stateTypes.Value.Count > 0) ? stateTypes.Value.Count * 22 : 20
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			}

			return lstData;
		}
		private static void Remove500Equipement(QuickSetCSW quickSetCSW)
		{
			foreach (var item in quickSetCSW.Constraints.EQUIP.lstParameters)
			{
				if (item.ThirdcellValue == "500")
					item.ThirdcellValue = "300";

			}
			foreach (var item in quickSetCSW.Weights.EQUIP.lstParameters)
			{
				if (item.SecondlValue == 500)
					item.SecondlValue = 300;

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
	}

	public partial class QSDetailSource : NSTableViewSource
	{
		QuickSetDetailWindowController parentVC;

		public QSDetailSource (QuickSetDetailWindowController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.detailList.Count;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var vw = (QuickSetDetailCell)tableView.MakeView ("QSDetail", this);
			try {
				vw.BindData (parentVC.detailList [(int)row],(int) row);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
			return vw;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			return parentVC.detailList [(int)row].RowHeight;
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
		}
	}

	public class QSDetailData
	{
		public int Type { get; set; }

		public string Title { get; set; }

		public string DataValue { get; set; }

		public float RowHeight { get; set; }
	}
}

