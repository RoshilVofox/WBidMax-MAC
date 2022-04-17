
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using System.Collections.Generic;
using WBid.WBidiPad.SharedLibrary;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
//using WBid.WBidiPad.PortableLibrary.Core;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
//using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;

namespace WBid.WBidMac.Mac
{
	public partial class ModernViewController : AppKit.NSViewController
	{
		#region Constructors
		WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		public TripWindowController TripWC;

		// Called when created from unmanaged code
		public ModernViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ModernViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ModernViewController () : base ("ModernView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ModernView View {
			get {
				return (ModernView)base.View;
			}
		}
		private void AddRatio(NSNotification n)
		{

			var objectData = n.Object.ToString();

			if (objectData == "OK")
			{
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
				{
					if (!GlobalSettings.WBidINIContent.ModernVacationColumns.Any(x => x == GlobalSettings.columndefinition.FirstOrDefault(y => y.DisplayName == "Ratio").Id))
					{
						GlobalSettings.WBidINIContent.ModernVacationColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
						CommonClass.modernProperties.Add("Ratio");
					}
					                                                   
					//GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;
				}
				else
				{
					if (!GlobalSettings.WBidINIContent.ModernNormalColumns.Any(x => x == GlobalSettings.columndefinition.FirstOrDefault(y => y.DisplayName == "Ratio").Id))
					{
						GlobalSettings.WBidINIContent.ModernNormalColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
						CommonClass.modernProperties.Add("Ratio");
					}
					//GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;
				}

				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
				GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending (x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList ();
			}
			else
			{

			}
			ReloadContent();

		}
		NSObject ModernRatioNotification;
		public void removeObserver()
		{
			if (ModernRatioNotification != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(ModernRatioNotification);
		}
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
                AdddBiorderforTheAwardedLine();
				tblModern.Source = new ModernTableSource (this);
				tblModern.DoubleClick += ModernDoubleClicked;
				if(ModernRatioNotification == null)
				ModernRatioNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"ModernRatioNotification", AddRatio);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void ModernDoubleClicked (object sender, EventArgs e)
		{
			try {
				Console.WriteLine ("double click");
				CommonClass.selectedLine = GlobalSettings.Lines [(int)tblModern.SelectedRow].LineNum;
				var loc = CommonClass.MainController.Window.MouseLocationOutsideOfEventStream;//NSEvent.CurrentMouseLocation;
				var cellView = (ModernCell)tblModern.GetView (0, tblModern.SelectedRow, false);
				var loc1 = this.View.ConvertPointToView (loc, cellView);
				var loc2 = cellView.ConvertPointToView (loc1, cellView.bxCalData);
				if (cellView.bxCalData.Bounds.Contains (loc2)) {
					var temp = GlobalSettings.Lines [(int)tblModern.SelectedRow].BidLineTemplates;
					var loc3 = cellView.bxCalData.ConvertPointToView (loc2, ((NSView)cellView.bxCalData.ContentView));
					Console.WriteLine ("yes! Cal box of row: " + tblModern.SelectedRow.ToString ());
					var bxTrip = ((NSView)cellView.bxCalData.ContentView).Subviews.ToList ().ConvertAll (x => (NSBox)x);
					var box = bxTrip.FirstOrDefault (x => x.Frame.Contains (loc3));
					if (box != null) {
						Console.WriteLine (box.Identifier);
						var idx1 = int.Parse (box.Identifier.Replace ("bx", ""));
						Console.WriteLine (temp [idx1].TripName);
						var trip = temp [idx1].TripName;
						if (!string.IsNullOrEmpty (trip)) {
							if (TripWC == null)
								TripWC = new TripWindowController ();
							TripWC.tripNum = trip;
							CorrectionParams correctionParams = new WBid.WBidiPad.Model.CorrectionParams ();
							correctionParams.selectedLineNum = CommonClass.selectedLine;
							CommonClass.tripData = TripViewBL.GenerateTripDetails (trip, correctionParams, false);
							TripWC.LoadContent ();
							CommonClass.selectedTrip = trip;
							var r = tblModern.SelectedRow;
							tblModern.ReloadData ();
							tblModern.SelectRow (r, false);
							CommonClass.MainController.Window.AddChildWindow (TripWC.Window, NSWindowOrderingMode.Above);
							TripWC.Window.MakeKeyAndOrderFront (this);
							//NSApplication.SharedApplication.RunModalForWindow (TripWC.Window);
						}
					}
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void RightClicked (NSEvent ev)
		{
			try {
//				StateManagement stateManagement = new StateManagement();
//				stateManagement.UpdateWBidStateContent();
//				WBidHelper.PushToUndoStack ();
				var lineSel = 0;
				//Console.WriteLine (ev.Description);
				var point = this.View.ConvertPointToView (ev.LocationInWindow, tblModern);
				var row = tblModern.GetRow (point);

				var cellView = (ModernCell)tblModern.GetView (0, row, false);
				if (cellView != null)
				{

					var point1 = tblModern.ConvertPointToView(point, cellView);
					var point2 = cellView.ConvertPointToView(point1, cellView.bxProperty);

					if (cellView.bxProperty.Bounds.Contains(point2))
					{
						Console.WriteLine("property");
						ShowColumnsMenu(point2);
					}
					else if (CommonClass.selectedRows.Count > 0 && row >= 0)
					{
						


						StateManagement stateManagement = new StateManagement();
						stateManagement.UpdateWBidStateContent();
						WBidHelper.PushToUndoStack();

						var lineNum = GlobalSettings.Lines[(int)row].LineNum;

						if (CommonClass.isTKeyPressed)
						{
							LineOperations.TopLockThisLineAndAbove(lineNum);
						}
						else if (CommonClass.isBKeyPressed)
						{
							LineOperations.BottomLockThisLineAndBelow(lineNum);
						}
						else {
							foreach (var lineobj in GlobalSettings.Lines)
							{
								lineobj.ManualScroll = 0;
							}
							foreach (var item in CommonClass.selectedRows)
							{
								GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == item).ManualScroll = 2;
							}

							bool isNeedtoShowBlueLineonFirstLine = false;
							//show blue line for the line just above last selected line
							var lastlineindex = GlobalSettings.Lines.IndexOf(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == CommonClass.selectedRows[CommonClass.selectedRows.Count - 1]));
							if (!CommonClass.selectedRows.Contains(GlobalSettings.Lines[0].LineNum))
							{
								var availablelines = GlobalSettings.Lines.ToList();
								availablelines.RemoveAll(x => x.ManualScroll == 2);
								var bluelineborder = availablelines[lastlineindex - CommonClass.selectedRows.Count].LineNum;
								GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == bluelineborder).ManualScroll = 1;
							}
							else
							{
								isNeedtoShowBlueLineonFirstLine = true;

							}

							var index = 0;
							var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == CommonClass.selectedRows.Last());
							if (line != null)
							{
								index = GlobalSettings.Lines.IndexOf(line) + 1;
								if (index >= GlobalSettings.Lines.Count)
									index = GlobalSettings.Lines.Count - 2;
								lineSel = GlobalSettings.Lines[index].LineNum;
							}
							bool isneedToshowmessage = LineOperations.MoveSelectedLineBelow(CommonClass.selectedRows, lineNum);
							CommonClass.columnID = 0;
							SortDetails stateSortDetails = wBidStateContent.SortDetails;
							stateSortDetails.SortColumn = "Manual";
							if (CommonClass.SortController != null)
							{
								CommonClass.SortController.setValuesToFixedSorts();
								CommonClass.SortController.setViews();
							}
							if (isNeedtoShowBlueLineonFirstLine)
								GlobalSettings.Lines[0].ManualScroll = 3;
							if (isneedToshowmessage)
							{
								var alert = new NSAlert()
								{
									MessageText = "WBidMax",
									InformativeText = "Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!"
								};
								CommonClass.CSWController.ReloadAllContent();
								CommonClass.MainController.HandleBlueShadowButton();
								alert.RunModal();

							}
						}
						CommonClass.MainController.ReloadAllContent();
						tblModern.SelectRow(GlobalSettings.Lines.IndexOf(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineSel)), false);
					}
					else if (row >= 0)
					{
						var lineNum = GlobalSettings.Lines[(int)row].LineNum;
						if (CommonClass.isTKeyPressed)
						{
							StateManagement stateManagement = new StateManagement();
							stateManagement.UpdateWBidStateContent();
							WBidHelper.PushToUndoStack();

							LineOperations.TopLockThisLineAndAbove(lineNum);
						}
						else if (CommonClass.isBKeyPressed)
						{
							StateManagement stateManagement = new StateManagement();
							stateManagement.UpdateWBidStateContent();
							WBidHelper.PushToUndoStack();

							LineOperations.BottomLockThisLineAndBelow(lineNum);
						}
						CommonClass.MainController.ReloadAllContent();
					}
				}
				else
				{
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			} 
		}

		void ShowColumnsMenu (CGPoint point)
		{
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
				var menu = new NSMenu ("Modern Vac Columns");
				var reset = new NSMenuItem ("Reset");
				reset.Activated += (object sender, EventArgs e) => {
					GlobalSettings.WBidINIContent.ModernVacationColumns = new List<int> () { 36, 53, 200, 34, 12 };
					LineSummaryBL.GetModernViewAdditionalVacationalColumns ();
					LineSummaryBL.SetSelectedModernBidLineVacationColumnstoGlobalList ();
					CommonClass.modernProperties = new List<string> ();
					foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns) {
						var col = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.modernProperties.Add (col.DisplayName);
						}
					}
					ReloadContent ();
				};
				menu.AddItem (reset);
				menu.AddItem (NSMenuItem.SeparatorItem);
				foreach (var column in GlobalSettings.ModernAdditionalvacationColumns) {
					var item = new NSMenuItem (column.DisplayName);
					if (column.IsSelected)
						item.State = NSCellStateValue.On;
					menu.AddItem (item);
					item.Activated += (object sender, EventArgs e) => {
						var menuCol = (NSMenuItem)sender;
						var tappedColumn = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault (x => x.DisplayName == menuCol.Title);
						if(CommonClass.modernProperties.Count==1 && tappedColumn.IsSelected)
						{

							var alert = new NSAlert () { 
								MessageText = "Modern Columns",
								InformativeText = "Minimum 1 Column Required."
							};
							alert.RunModal ();

							return;

						}
						column.IsSelected = !column.IsSelected;

						if (tappedColumn.IsSelected && CommonClass.modernProperties.Count >= 5)
						{
							tappedColumn.IsSelected = false;

							var alert = new NSAlert()
							{
								MessageText = "Modern Columns",
								InformativeText = "Maximum 5 Columns Allowed"
							};
							alert.RunModal();

						}
						else
						{
							if (CommonClass.modernProperties.Contains(tappedColumn.DisplayName))
							{
								CommonClass.modernProperties.Remove(tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.ModernVacationColumns.Remove(tappedColumn.Id);
							}
							else
							{
								if (tappedColumn.DisplayName == "Ratio")
								{
                                    ShowRatioView();
									return;
								}
								CommonClass.modernProperties.Add(tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.ModernVacationColumns.Add(tappedColumn.Id);
							}
						}
						GlobalSettings.ModernAdditionalvacationColumns = GlobalSettings.ModernAdditionalvacationColumns.OrderByDescending (x => x.IsSelected == true).ThenBy (y => y.DisplayName).ToList ();

						ReloadContent ();
						item.State = (column.IsSelected) ? NSCellStateValue.On : NSCellStateValue.Off;
					};
				}
				menu.PopUpMenu (null, point, tblModern.EnclosingScrollView);
			} else {
				var menu = new NSMenu ("Modern Norm Columns");
				var reset = new NSMenuItem ("Reset");
				reset.Activated += (object sender, EventArgs e) => {
					GlobalSettings.WBidINIContent.ModernNormalColumns = new List<int> () { 36, 37, 27, 34, 12 };
					LineSummaryBL.GetModernViewAdditionalColumns ();
					LineSummaryBL.SetSelectedModernBidLineColumnstoGlobalList ();
					CommonClass.modernProperties = new List<string> ();
					foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns) {
						var col = GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.modernProperties.Add (col.DisplayName);
						}
					}
					ReloadContent ();
				};
				menu.AddItem (reset);
				menu.AddItem (NSMenuItem.SeparatorItem);
				foreach (var column in GlobalSettings.ModernAdditionalColumns) {
					var item = new NSMenuItem (column.DisplayName);
					if (column.IsSelected)
						item.State = NSCellStateValue.On;
					menu.AddItem (item);
					item.Activated += (object sender, EventArgs e) => {
						var menuCol = (NSMenuItem)sender;
						var tappedColumn = GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.DisplayName == menuCol.Title);
						if(CommonClass.modernProperties.Count==1 && tappedColumn.IsSelected)
						{

							var alert = new NSAlert () { 
								MessageText = "Modern Columns",
								InformativeText = "Minimum 1 Column Required."
							};
							alert.RunModal ();

							return;

						}
						column.IsSelected = !column.IsSelected;

						if (tappedColumn.IsSelected && CommonClass.modernProperties.Count >= 5) {
							tappedColumn.IsSelected = false;

							var alert = new NSAlert () { 
								MessageText = "Modern Columns",
								InformativeText = "Maximum 5 Columns Allowed"
							};
							alert.RunModal ();

						} else {
							if (CommonClass.modernProperties.Contains (tappedColumn.DisplayName)) {
								CommonClass.modernProperties.Remove (tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.ModernNormalColumns.Remove (tappedColumn.Id);
							} else {
								if (tappedColumn.DisplayName == "Ratio")
								{
									ShowRatioView();
									return;
								}

								CommonClass.modernProperties.Add (tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.ModernNormalColumns.Add (tappedColumn.Id);
							}
						}
						GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending (x => x.IsSelected == true).ThenBy (y => y.DisplayName).ToList ();

						ReloadContent ();
						item.State = (column.IsSelected) ? NSCellStateValue.On : NSCellStateValue.Off;
					};
				}
				menu.PopUpMenu (null, point, tblModern.EnclosingScrollView);
			}
		}

		public void ShowRatioView()
		{
			if (WBidHelper.IsRatioPropertiesSetFromOtherViews(wBidStateContent))
			{
				bool isNormalMode = !(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM);
				if
					(isNormalMode)
				{
					GlobalSettings.WBidINIContent.ModernNormalColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
				}
				else
				{
					GlobalSettings.WBidINIContent.ModernVacationColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
				}
				WBidHelper.SetRatioValues(wBidStateContent);
				CommonClass.modernProperties.Add("Ratio");
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
				GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending (x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList ();
                ReloadContent();
			}
			else
			{
				RatioViewControllerController login = new RatioViewControllerController();
				login.FromScreen = "Modern";
				var panel = new NSPanel();
				login.panel = panel;
				panel.SetContentSize(new CoreGraphics.CGSize(293, 221));
				panel.ContentView = login.View;
				NSApplication.SharedApplication.BeginSheet(panel, this.View.Window);
			}
		}



		public void GoToLine (Line line)
		{
			int row = GlobalSettings.Lines.IndexOf (line);
			tblModern.SelectRow(row,false);
			tblModern.ScrollRowToVisible (row);
		}
        private void AdddBiorderforTheAwardedLine()
        {
            if (GlobalSettings.WBidStateCollection.BidAwards != null && GlobalSettings.WBidStateCollection.BidAwards.Count != 0 && GlobalSettings.WbidUserContent.UserInformation.EmpNo != null)
            {

                //need to show the red line border for the awarded lime.
                var awardedline = GlobalSettings.WBidStateCollection.BidAwards.FirstOrDefault(x => x.EmpNum == Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo));
                if (awardedline != null)
                {
                    var linedata = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == awardedline.LineNum);
                    if (linedata != null)
                    {
                        linedata.ManualScroll = 4;

                    }
                }
            }

        }
		public void ReloadContent ()
		{
            AdddBiorderforTheAwardedLine();
			SortCalculation sort = new SortCalculation();
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			{
				sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			}


			nint selected = tblModern.SelectedRow+1;
			if (selected > GlobalSettings.Lines.Count - 1)
				selected = GlobalSettings.Lines.Count - 1;
			else if (selected < 0)
				selected = 0;
			
			

			tblModern.ReloadData ();

			if (tblModern.SelectedRowCount > 0) {
				var rows = tblModern.SelectedRows.ToArray ();
				foreach (var item in rows) 
				{
					if ((int)item != tblModern.ClickedRow)
						tblModern.DeselectRow (Convert.ToInt32 (item));
				}
				CommonClass.selectedRows.Clear ();
				CommonClass.MainController.LockBtnEnableDisable ();
			}

			tblModern.SelectRow (selected, false);
		}

	}

	public partial class ModernTableSource : NSTableViewSource 
	{
		ModernViewController parentVC;
		public ModernTableSource (ModernViewController parent)
		{
			parentVC = parent;
		}
		public override nint GetRowCount (NSTableView tableView)
		{
			return GlobalSettings.Lines.Count;
		}
public override NSTableRowView GetRowView(NSTableView tableView, nint row)
{
			var view = (ModernLineRowView)tableView.MakeView("ModernLineRowView", this);
	if (view == null)
		view = new ModernLineRowView(new CGRect(0, 0, tableView.Frame.Width, (nfloat)80));
			return view;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var vw = (ModernCell)tableView.MakeView ("Modern", this);
			try {
				vw.BindData (GlobalSettings.Lines [(int)row], (int)row);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
			return vw;
		}
		public override nfloat GetRowHeight(NSTableView tableView, nint row)
		{
			if (GlobalSettings.WBidINIContent.User.IsModernViewShade == false)
			{
				return 80;
			}

			else
			{
				if (GlobalSettings.Lines[(int)row].ManualScroll == 1)
				{
					return 100;
				}
				else
				{
					return 80;
				}
			}
		//	return base.GetRowHeight(tableView, row);
		}
		public override void SelectionDidChange (NSNotification notification)
		{
			try {
				var table = (NSTableView)notification.Object;
				if (table.SelectedRowCount > 0) {
					var rows = table.SelectedRows.ToList ().ConvertAll (x => (int)x);
					var lines = new List<int> ();
					foreach (var item in rows) {
						var lineNum = GlobalSettings.Lines [item].LineNum;
						lines.Add (lineNum);
					}
					CommonClass.selectedRows = lines;
					CommonClass.MainController.LockBtnEnableDisable ();
					CommonClass.selectedLine = GlobalSettings.Lines [rows [0]].LineNum;
					CommonClass.importLine = GlobalSettings.Lines [rows [0]];
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

	}
}

