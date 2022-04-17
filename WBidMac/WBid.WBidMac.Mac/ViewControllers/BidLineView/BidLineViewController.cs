
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
	public partial class BidLineViewController : AppKit.NSViewController
	{
		#region Constructors

		WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		public TripWindowController TripWC;

		// Called when created from unmanaged code
		public BidLineViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidLineViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BidLineViewController () : base ("BidLineView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new BidLineView View {
			get {
				return (BidLineView)base.View;
			}
		}
		NSObject BidLineRatioNotification;

		private void AddRatio(NSNotification n)
		{

			var objectData = n.Object.ToString();

			if (objectData == "OK")
			{
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
				{
					if (!GlobalSettings.WBidINIContent.BidLineVacationColumns.Any(x => x == GlobalSettings.columndefinition.FirstOrDefault(y => y.DisplayName == "Ratio").Id))
					{
						GlobalSettings.WBidINIContent.BidLineVacationColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
						CommonClass.bidLineProperties.Add("Ratio");
					}

					//GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;
				}
				else
				{
					if (!GlobalSettings.WBidINIContent.BidLineNormalColumns.Any(x => x == GlobalSettings.columndefinition.FirstOrDefault(y => y.DisplayName == "Ratio").Id))
					{
						GlobalSettings.WBidINIContent.BidLineNormalColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
						CommonClass.bidLineProperties.Add("Ratio");
						//GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;
					}
				}

				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
				GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
			}
			else
			{

			}
			ReloadContent();


		}
		public void removeObserver()
		{
			if (BidLineRatioNotification != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(BidLineRatioNotification);
		}
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				tblBidLine.Source = new BidLineTableSource (this,tblBidLine);
				tblBidLine.SelectionHighlightStyle= NSTableViewSelectionHighlightStyle.None;
				tblBidLine.DoubleClick += BidLineDoubleClicked;
				if(BidLineRatioNotification == null)
				BidLineRatioNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"BidLineRatioNotification", AddRatio);

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		

			void BidLineDoubleClicked (object sender, EventArgs e)
		{
			try {
				Console.WriteLine ("double click");
				CommonClass.selectedLine = GlobalSettings.Lines [(int)tblBidLine.SelectedRow].LineNum;
				var loc = CommonClass.MainController.Window.MouseLocationOutsideOfEventStream;//NSEvent.CurrentMouseLocation;
				var cellView = (BidLineCell)tblBidLine.GetView (0, tblBidLine.SelectedRow, false);
				var loc1 = this.View.ConvertPointToView (loc, cellView);
				var loc2 = cellView.ConvertPointToView (loc1, cellView.bxCalData);
				if (cellView.bxCalData.Bounds.Contains (loc2)) {
					var temp = GlobalSettings.Lines [(int)tblBidLine.SelectedRow].BidLineTemplates;
					var loc3 = cellView.bxCalData.ConvertPointToView (loc2, ((NSView)cellView.bxCalData.ContentView));
					Console.WriteLine ("yes! Cal box of row: " + tblBidLine.SelectedRow.ToString ());
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
							var r = tblBidLine.SelectedRow;
							tblBidLine.ReloadData ();
							tblBidLine.SelectRow (r, false);
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

		public void ShowTripWindow (string trip, int row)
		{
			try {
				tblBidLine.SelectRow (row, false);
				if (TripWC == null)
					TripWC = new TripWindowController ();
				TripWC.tripNum = trip;
				CorrectionParams correctionParams = new WBid.WBidiPad.Model.CorrectionParams ();
				correctionParams.selectedLineNum = CommonClass.selectedLine;
				CommonClass.tripData = TripViewBL.GenerateTripDetails (trip, correctionParams, false);
				TripWC.LoadContent ();
				CommonClass.selectedTrip = trip;
				var r = tblBidLine.SelectedRow;
				tblBidLine.ReloadData ();
				tblBidLine.SelectRow (r, false);
				CommonClass.MainController.Window.AddChildWindow (TripWC.Window, NSWindowOrderingMode.Above);
				TripWC.Window.MakeKeyAndOrderFront (this);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void RightClicked (NSEvent ev)
		{
			try {
//				StateManagement stateManagement = new StateManagement ();
//				stateManagement.UpdateWBidStateContent ();
//				WBidHelper.PushToUndoStack ();
				var lineSel = 0;
				//Console.WriteLine (ev.Description);
				var point = this.View.ConvertPointToView (ev.LocationInWindow, tblBidLine);
				var row = tblBidLine.GetRow (point);

				var cellView = (BidLineCell)tblBidLine.GetView (0, row, false);
				var point1 = tblBidLine.ConvertPointToView (point, cellView);
				var point2 = cellView.ConvertPointToView (point1, cellView.bxProperty);





				if (cellView.bxProperty.Bounds.Contains (point2)) {
					Console.WriteLine ("property");
					ShowColumnsMenu (point2);
				} else if (CommonClass.selectedRows.Count > 0 && row >= 0) {
					StateManagement stateManagement = new StateManagement ();
					stateManagement.UpdateWBidStateContent ();
					WBidHelper.PushToUndoStack ();

					var lineNum = GlobalSettings.Lines [(int)row].LineNum;
					if (CommonClass.isTKeyPressed) {
						LineOperations.TopLockThisLineAndAbove (lineNum);
					} else if (CommonClass.isBKeyPressed) {
						LineOperations.BottomLockThisLineAndBelow (lineNum);
					} else {
						var index = 0;
						var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedRows.Last ());
						if (line != null) {
							index = GlobalSettings.Lines.IndexOf (line) + 1;
							if (index >= GlobalSettings.Lines.Count)
								index = GlobalSettings.Lines.Count - 2;
							lineSel = GlobalSettings.Lines [index].LineNum;
						}
						bool isneedToshowmessage=LineOperations.MoveSelectedLineBelow (CommonClass.selectedRows, lineNum);
						CommonClass.columnID = 0;
						SortDetails stateSortDetails = wBidStateContent.SortDetails;
						stateSortDetails.SortColumn = "Manual";
						if (CommonClass.SortController != null) {
							CommonClass.SortController.setValuesToFixedSorts ();
							CommonClass.SortController.setViews ();
						}
						if (isneedToshowmessage) {
							var alert = new NSAlert () { 
								MessageText = "WBidMax",
								InformativeText = "Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!"
							
							};
							CommonClass.CSWController.ReloadAllContent();
							alert.RunModal();

						}
					}
					CommonClass.MainController.ReloadAllContent ();
					tblBidLine.SelectRow (GlobalSettings.Lines.IndexOf (GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == lineSel)), false);
//					NSTableRowView rowView= tblBidLine.GetRowView(GlobalSettings.Lines.IndexOf (GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == lineSel)),false);
//					if (rowView != null)
//					{
//						rowView.SelectionHighlightStyle=NSTableViewSelectionHighlightStyle.Regular;
//						rowView.Emphasized=false;
//					}
				} else if (row >= 0) {
					var lineNum = GlobalSettings.Lines [(int)row].LineNum;
					if (CommonClass.isTKeyPressed) {
						StateManagement stateManagement = new StateManagement ();
						stateManagement.UpdateWBidStateContent ();
						WBidHelper.PushToUndoStack ();

						LineOperations.TopLockThisLineAndAbove (lineNum);
					} else if (CommonClass.isBKeyPressed) {
						StateManagement stateManagement = new StateManagement ();
						stateManagement.UpdateWBidStateContent ();
						WBidHelper.PushToUndoStack ();

						LineOperations.BottomLockThisLineAndBelow (lineNum);
					}
					CommonClass.MainController.ReloadAllContent ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			} 
		}

		void ShowColumnsMenu (CGPoint point)
		{
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
				var menu = new NSMenu ("Bidline Vac Columns");
				var reset = new NSMenuItem ("Reset");
				reset.Activated += (object sender, EventArgs e) => {
					GlobalSettings.WBidINIContent.BidLineVacationColumns = new List<int> () { 36, 53, 200, 34, 12 };
					LineSummaryBL.GetBidlineViewAdditionalVacationColumns ();
					LineSummaryBL.SetSelectedBidLineVacationColumnstoGlobalList ();
					CommonClass.bidLineProperties = new List<string> ();
					foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns) {
						var col = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.bidLineProperties.Add (col.DisplayName);
						}
					}
					ReloadContent ();
				};
				menu.AddItem (reset);
				menu.AddItem (NSMenuItem.SeparatorItem);
				foreach (var column in GlobalSettings.BidlineAdditionalvacationColumns) {
					var item = new NSMenuItem (column.DisplayName);
					if (column.IsSelected)
						item.State = NSCellStateValue.On;
					menu.AddItem (item);
					item.Activated += (object sender, EventArgs e) => {
						var menuCol = (NSMenuItem)sender;
						var tappedColumn = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault (x => x.DisplayName == menuCol.Title);

						if (CommonClass.bidLineProperties.Count == 1 && tappedColumn.IsSelected) {

							var alert = new NSAlert () { 
								MessageText = "BidLine Columns",
								InformativeText = "Minimum 1 Column Required."
							};
							alert.RunModal ();

							return;

						}

						column.IsSelected = !column.IsSelected;

						if (tappedColumn.IsSelected && CommonClass.bidLineProperties.Count >= 5)
						{
							tappedColumn.IsSelected = false;

							var alert = new NSAlert()
							{
								MessageText = "BidLine Columns",
								InformativeText = "Maximum 5 Columns Allowed"
							};
							alert.RunModal();

						}
						else
						{
							if (CommonClass.bidLineProperties.Contains(tappedColumn.DisplayName))
							{
								CommonClass.bidLineProperties.Remove(tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.BidLineVacationColumns.Remove(tappedColumn.Id);
							}
							else
							{
								if (tappedColumn.DisplayName == "Ratio")
								{
                                    ShowRatioView();
									return;
								}
								CommonClass.bidLineProperties.Add(tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.BidLineVacationColumns.Add(tappedColumn.Id);
							}
						}
						GlobalSettings.BidlineAdditionalvacationColumns = GlobalSettings.BidlineAdditionalvacationColumns.OrderByDescending (x => x.IsSelected == true).ThenBy (y => y.DisplayName).ToList ();

						ReloadContent ();
						item.State = (column.IsSelected) ? NSCellStateValue.On : NSCellStateValue.Off;
					};
				}
				menu.PopUpMenu (null, point, tblBidLine.EnclosingScrollView);
			} else {
				var menu = new NSMenu ("Bidline Norm Columns");
				var reset = new NSMenuItem ("Reset");
				reset.Activated += (object sender, EventArgs e) => {
					GlobalSettings.WBidINIContent.BidLineNormalColumns = new List<int> () { 36, 37, 27, 34, 12 };
					LineSummaryBL.GetBidlineViewAdditionalColumns ();
					LineSummaryBL.SetSelectedBidLineColumnstoGlobalList ();
					CommonClass.bidLineProperties = new List<string> ();
					foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns) {
						var col = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault (x => x.Id == item);
						if (col != null) {
							CommonClass.bidLineProperties.Add (col.DisplayName);
						}
					}
					ReloadContent ();
				};
				menu.AddItem (reset);
				menu.AddItem (NSMenuItem.SeparatorItem);
				foreach (var column in GlobalSettings.BidlineAdditionalColumns) {
					var item = new NSMenuItem (column.DisplayName);
					if (column.IsSelected)
						item.State = NSCellStateValue.On;
					menu.AddItem (item);
					item.Activated += (object sender, EventArgs e) => {
						var menuCol = (NSMenuItem)sender;
						var tappedColumn = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault (x => x.DisplayName == menuCol.Title);

						if (CommonClass.bidLineProperties.Count == 1 && tappedColumn.IsSelected) {

							var alert = new NSAlert () { 
								MessageText = "BidLine Columns",
								InformativeText = "Minimum 1 Column Required."
							};
							alert.RunModal ();

							return;

						}

						column.IsSelected = !column.IsSelected;

						if (tappedColumn.IsSelected && CommonClass.bidLineProperties.Count >= 5) {
							tappedColumn.IsSelected = false;

							var alert = new NSAlert () { 
								MessageText = "BidLine Columns",
								InformativeText = "Maximum 5 Columns Allowed"
							};
							alert.RunModal ();

						} else {



							if (CommonClass.bidLineProperties.Contains (tappedColumn.DisplayName)) {
								CommonClass.bidLineProperties.Remove (tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.BidLineNormalColumns.Remove (tappedColumn.Id);
							} else {

								if (tappedColumn.DisplayName == "Ratio")
								{
									ShowRatioView();
									return;
								}

								CommonClass.bidLineProperties.Add (tappedColumn.DisplayName);
								GlobalSettings.WBidINIContent.BidLineNormalColumns.Add (tappedColumn.Id);
							}
						}
						GlobalSettings.BidlineAdditionalColumns = GlobalSettings.BidlineAdditionalColumns.OrderByDescending (x => x.IsSelected == true).ThenBy (y => y.DisplayName).ToList ();

						ReloadContent ();
						item.State = (column.IsSelected) ? NSCellStateValue.On : NSCellStateValue.Off;
					};
				}
				menu.PopUpMenu (null, point, tblBidLine.EnclosingScrollView);
			}
		}

		public void GoToLine (Line line)
		{
			int row = GlobalSettings.Lines.IndexOf (line);
			tblBidLine.SelectRow (row, false);
			tblBidLine.ScrollRowToVisible (row);
		}
		public void ShowRatioView()
		{
			if (WBidHelper.IsRatioPropertiesSetFromOtherViews(wBidStateContent))
			{
				bool isNormalMode = !(GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM);
				if
					(isNormalMode)
				{
					GlobalSettings.WBidINIContent.BidLineNormalColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
				}
				else
				{
					GlobalSettings.WBidINIContent.BidLineVacationColumns.Add(GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id);
				}
				WBidHelper.SetRatioValues(wBidStateContent);
				CommonClass.bidLineProperties.Add("Ratio");
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
				GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();

				ReloadContent();
			}
			else
			{
				RatioViewControllerController login = new RatioViewControllerController();
				login.FromScreen = "BidLine";
				var panel = new NSPanel();
				login.panel = panel;
				panel.SetContentSize(new CoreGraphics.CGSize(293, 221));
				panel.ContentView = login.View;
				NSApplication.SharedApplication.BeginSheet(panel, this.View.Window);
			}
		}   
		public void ReloadContent ()
		{
			SortCalculation sort = new SortCalculation ();
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty) {
				sort.SortLines (wBidStateContent.SortDetails.SortColumn);
			}
			nint selected = tblBidLine.SelectedRow+1;
			if (selected > GlobalSettings.Lines.Count - 1)
				selected = GlobalSettings.Lines.Count - 1;
			else if (selected < 0)
				selected = 0;
			
			tblBidLine.ReloadData ();

			if (tblBidLine.SelectedRowCount > 0) {
				var rows = tblBidLine.SelectedRows.ToArray ();
				foreach (var item in rows) 
				{
//					if ((int)item != tblBidLine.ClickedRow) {
//
//							tblBidLine.DeselectRow (Convert.ToInt32 (item));
//					}
				}
				CommonClass.selectedRows.Clear ();

				CommonClass.MainController.LockBtnEnableDisable ();
			}
			tblBidLine.SelectRow (selected, false);
		}
	}

	public partial class BidLineTableSource : NSTableViewSource
	{
		BidLineViewController parentVC;
		NSTableView tableView;
		public BidLineTableSource (BidLineViewController parent,NSTableView table)
		{
			parentVC = parent;
			tableView = table;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return GlobalSettings.Lines.Count;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var vw = (BidLineCell)tableView.MakeView ("BidLine", this);
			try {
				vw.BindData (GlobalSettings.Lines [(int)row], (int)row);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
			return vw;
		}
		public override NSTableRowView GetRowView (NSTableView tableView, nint row)
		{
			var view = (BidLineRowView)tableView.MakeView ("BidLineView", this);
			if (view == null)
				view = new BidLineRowView (new CGRect (0, 0, tableView.Frame.Width, (nfloat)80));
			return view;

		}

		public override void SelectionDidChange (NSNotification notification)
		{
			try {
				var table = (NSTableView)notification.Object;

				NSTableRowView rowView= table.GetRowView(table.SelectedRow,true);


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


					if (rowView != null)
					{
						rowView.SelectionHighlightStyle=NSTableViewSelectionHighlightStyle.Regular;
						rowView.Emphasized=false;
					}
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
	}
}

