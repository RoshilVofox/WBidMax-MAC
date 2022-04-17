
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;

//using MonoTouch.CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
using WBid.WBidiPad.Model;

//using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

//using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using System.IO;

//using MonoTouch.EventKit;
//using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Net;
//using System.Windows.Forms;

namespace WBid.WBidMac.Mac
{


	public partial class SummaryTableView : AppKit.NSTableView
	{
		public override void MouseDown (NSEvent theEvent)
		{
			Console.WriteLine ("Success");
		}
	}
	public partial class SummaryViewController : AppKit.NSViewController
	{
		Dictionary<string, NSImage> ColTitle = new Dictionary<string, NSImage>();
		WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

		#region Constructors

		// Called when created from unmanaged code
		public SummaryViewController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public SummaryViewController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public SummaryViewController() : base("SummaryView", NSBundle.MainBundle)
		{
			Initialize();
		}

		// Shared initialization codeWin
		void Initialize()
		{
		}

		#endregion



		//strongly typed view accessor
		public new SummaryView View
		{
			get
			{
				return (SummaryView)base.View;
			}
		}
		//		public int VisibleRow()
		//		{
		//			
		//		}
		NSObject SummaryRatioNotification;

		private void AddRatio(NSNotification n)
		{

			var objectData = n.Object.ToString();

			if (objectData == "OK")
			{
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
				{
					if (!GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(x => x.Id == GlobalSettings.columndefinition.FirstOrDefault(y => y.DisplayName == "Ratio").Id))
					{
						GlobalSettings.WBidINIContent.SummaryVacationColumns.Add(new DataColumn()
						{
							Id = GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id,
							Width = 50,
							Order = GlobalSettings.WBidINIContent.DataColumns.Count
						});
						GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;
					}
				}
				else
				{
					if (!GlobalSettings.WBidINIContent.DataColumns.Any(x => x.Id == GlobalSettings.columndefinition.FirstOrDefault(y => y.DisplayName == "Ratio").Id))
					{

						GlobalSettings.WBidINIContent.DataColumns.Add(new DataColumn()
						{
							Id = GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id,
							Width = 50,
							Order = GlobalSettings.WBidINIContent.DataColumns.Count
						});
						GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;
					}
				}
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());

			}
			else
			{

			}
            LoadContent();
							

		}
		public void removeObserver()
		{
			if (SummaryRatioNotification != null)
				NSNotificationCenter.DefaultCenter.RemoveObserver(SummaryRatioNotification);
		}
		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			ColTitle.Add("Constraint", NSImage.ImageNamed("constraintIcon.png"));
			ColTitle.Add("Lock", NSImage.ImageNamed("lockIcon.png"));
			ColTitle.Add("Overlap", NSImage.ImageNamed("overlapIcon.png"));
			if(SummaryRatioNotification == null)
			SummaryRatioNotification = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"SummaryRatioNotification", AddRatio);
			LoadContent();


			tblsummary.Source = new SummaryTableSource(this);
			tblsummary.DoubleClick += tblsummaryDoubleClicked;

			//tblsummary.MouseUp+= new MouseEventHandler(yourMethodName);

			tblsummary.AcceptsTouchEvents = true;
		}
		//private void yourMethodName(object sender, MouseEventArgs e)
		//{
		//	Console.WriteLine("Got it");
		//	//your code here
		//}
		public override void MouseUp(NSEvent theEvent)
		{
			switch (theEvent.ClickCount)
			{
				case 1:
					if (tblsummary.SelectedRowCount > 0)
					{
						var rows = tblsummary.SelectedRows.ToList().ConvertAll(x => (int)x);
						var lines = new List<int>();
						foreach (var item in rows)
						{
							var lineNum = GlobalSettings.Lines[item].LineNum;
							lines.Add(lineNum);
						}
						CommonClass.selectedRows = lines;
						CommonClass.MainController.LockBtnEnableDisable();
						if (CommonClass.CalendarController != null && CommonClass.selectedRows.Count == 1)
						{
							CommonClass.selectedLine = GlobalSettings.Lines[rows[0]].LineNum;
							CommonClass.calData = CalendarViewBL.GenerateCalendarDetails(GlobalSettings.Lines[rows[0]]);
							CommonClass.CalendarController.LoadContent();
						}
						CommonClass.selectedLine = GlobalSettings.Lines[rows[0]].LineNum;
						CommonClass.importLine = GlobalSettings.Lines[rows[0]];

						if (CommonClass.importLine.BAGroup != null)
						{
							if (CommonClass.importLine.BAGroup.Length > 0)
							{

								BAgroupWindowController Objgroup = new BAgroupWindowController();
								this.View.Window.AddChildWindow(Objgroup.Window, NSWindowOrderingMode.Above);
								NSApplication.SharedApplication.RunModalForWindow(Objgroup.Window);
							}
						}
					}
					break;
				case 2:
					break;
			}
		}

		public void RightClicked(NSEvent ev)
		{
			try
			{

				var lineSel = 0;
				//Console.WriteLine (ev.Description);
				var point = this.View.ConvertPointToView(ev.LocationInWindow, tblsummary);
				var row = tblsummary.GetRow(point);

				var point2 = this.View.ConvertPointToView(ev.LocationInWindow, tblsummary.EnclosingScrollView);
				if (CommonClass.selectedRows.Count > 0 && row >= 0 && !tblsummary.HeaderView.Bounds.Contains(point2))
				{
					var lineNum = GlobalSettings.Lines[(int)row].LineNum;
					StateManagement stateManagement = new StateManagement();
					stateManagement.UpdateWBidStateContent();
					WBidHelper.PushToUndoStack();
					if (CommonClass.isTKeyPressed)
					{
						LineOperations.TopLockThisLineAndAbove(lineNum);
					}
					else if (CommonClass.isBKeyPressed)
					{
						LineOperations.BottomLockThisLineAndBelow(lineNum);
					}
					else
					{
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
						if (isneedToshowmessage)
						{
							var alert = new NSAlert()
							{
								MessageText = "WBidMax",
								InformativeText = "Blank Lines are no longer at the bottom, you have moved a blank line(s) out of the bottom.!"
							};
							CommonClass.CSWController.ReloadAllContent();
							alert.RunModal();
						}
					}
					CommonClass.MainController.ReloadAllContent();
					tblsummary.SelectRow(GlobalSettings.Lines.IndexOf(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineSel)), false);
				}
				else if (!tblsummary.HeaderView.Bounds.Contains(point2) && row >= 0)
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
					CommonClass.MainController.ReloadAllContent();
				}
				else
				{
					if (tblsummary.HeaderView.Bounds.Contains(point2))
					{
						ShowColumnsMenu(point2);
					}
				}
			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			}
		}

		public void GoToLine(Line line)
		{
			int row = GlobalSettings.Lines.IndexOf(line);
			tblsummary.SelectRow(row, false);
			tblsummary.ScrollRowToVisible(row);
		}

		public void PromoteLine()
		{


			var lineSel = 0;


			var index = 0;
			var line = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == CommonClass.selectedRows.Last());
			if (line != null)
			{
				index = GlobalSettings.Lines.IndexOf(line) + 1;
				if (index >= GlobalSettings.Lines.Count)
					index = GlobalSettings.Lines.Count - 2;
				lineSel = GlobalSettings.Lines[index].LineNum;
			}


			tblsummary.SelectRow(GlobalSettings.Lines.IndexOf(GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineSel)), false);
		}

		void ShowColumnsMenu(CGPoint point)
		{
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
			{
				var selectedColumns = GlobalSettings.AdditionalvacationColumns.Where(x => GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(y => y.Id == x.Id)).ToList();
				var menu = new NSMenu("Additional Vac Columns");
				var reset = new NSMenuItem("Reset");
				reset.Activated += (object sender, EventArgs e) =>
				{
					GlobalSettings.WBidINIContent.SummaryVacationColumns = WBidCollection.GenerateDefaultVacationColumns();
					XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
					GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
					LoadContent();
				};
				menu.AddItem(reset);
				menu.AddItem(NSMenuItem.SeparatorItem);
				foreach (var column in GlobalSettings.AdditionalvacationColumns)
				{
					var item = new NSMenuItem(column.DisplayName);
					if (selectedColumns.Contains(column))
					{
						column.IsSelected = true;
						item.State = NSCellStateValue.On;
					}
					if (!column.IsRequied)
					{
						item.Activated += (object sender, EventArgs e) =>
						{
							var menuCol = (NSMenuItem)sender;
							var tappedColumn = GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title);
							if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count < 20)
							{
								
								if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count(x => x.Id == tappedColumn.Id) == 0)
								{
									if (tappedColumn.DataPropertyName == "Ratio")
									{
										ShowRatioView();
										return;
									}
									else
									{
										GlobalSettings.WBidINIContent.SummaryVacationColumns.Add(new DataColumn()
										{
											Id = tappedColumn.Id,
											Width = 50,
											Order = GlobalSettings.WBidINIContent.SummaryVacationColumns.Count
										});

										GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title).IsSelected = true;

										XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
										//Save and serialize.
										GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
									}
								}
								else
								{
									GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == tappedColumn.Id);
									GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title).IsSelected = false;
									XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
									//Save and serialize.
									GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
								}
								LoadContent();
							}
							else if (GlobalSettings.WBidINIContent.SummaryVacationColumns.Count(x => x.Id == tappedColumn.Id) == 1)
							{
								GlobalSettings.WBidINIContent.SummaryVacationColumns.RemoveAll(x => x.Id == tappedColumn.Id);
								GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title).IsSelected = false;
								XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
								//Save and serialize.
								GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
								LoadContent();
							}
							else
							{
								var alert = new NSAlert()
								{
									MessageText = "Summary Columns",
									InformativeText = "Maximum 20 Columns Allowed"
								};
								alert.RunModal();
							}
						};
					}
					menu.AddItem(item);
				}
				menu.PopUpMenu(null, point, tblsummary.EnclosingScrollView);
			}
			else
			{
				var selectedColumns = GlobalSettings.AdditionalColumns.Where(x => GlobalSettings.WBidINIContent.DataColumns.Any(y => y.Id == x.Id)).ToList();
				var menu = new NSMenu("Additional Columns");
				var reset = new NSMenuItem("Reset");
				reset.Activated += (object sender, EventArgs e) =>
				{
					GlobalSettings.WBidINIContent.DataColumns = WBidCollection.GenerateDefaultColumns();
					XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
					GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
					LoadContent();
				};
				menu.AddItem(reset);
				menu.AddItem(NSMenuItem.SeparatorItem);
				foreach (var column in GlobalSettings.AdditionalColumns)
				{
					var item = new NSMenuItem(column.DisplayName);
					if (selectedColumns.Contains(column))
					{
						column.IsSelected = true;
						item.State = NSCellStateValue.On;
					}
					if (!column.IsRequied)
					{
						item.Activated += (object sender, EventArgs e) =>
						{
							var menuCol = (NSMenuItem)sender;
							var tappedColumn = GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title);

							if (GlobalSettings.WBidINIContent.DataColumns.Count < 20)
							{
								if (GlobalSettings.WBidINIContent.DataColumns.Count(x => x.Id == tappedColumn.Id) == 0)
								{
									if (tappedColumn.DataPropertyName == "Ratio")
									{
										ShowRatioView();
										return;
									}
									else
									{
										GlobalSettings.WBidINIContent.DataColumns.Add(new DataColumn()
										{
											Id = tappedColumn.Id,
											Width = 50,
											Order = GlobalSettings.WBidINIContent.DataColumns.Count
										});
										GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title).IsSelected = true;
										XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
										//Save and serialize.
										GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
									}
								}
								else
								{
									GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == tappedColumn.Id);
									GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title).IsSelected = false;
									XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
									//Save and serialize.
									GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
								}
								LoadContent();
							}
							else if (GlobalSettings.WBidINIContent.DataColumns.Count(x => x.Id == tappedColumn.Id) == 1)
							{
								GlobalSettings.WBidINIContent.DataColumns.RemoveAll(x => x.Id == tappedColumn.Id);
								GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == menuCol.Title).IsSelected = false;
								XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
								//Save and serialize.
								GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
								LoadContent();
							}
							else
							{
								var alert = new NSAlert()
								{
									MessageText = "Summary Columns",
									InformativeText = "Maximum 20 Columns Allowed"
								};
								alert.RunModal();
							}
						};
					}
					menu.AddItem(item);
				}
				menu.PopUpMenu(null, point, tblsummary.EnclosingScrollView);
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
					GlobalSettings.WBidINIContent.DataColumns.Add(new DataColumn()
					{
						Id = GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id,
						Width = 50,
						Order = GlobalSettings.WBidINIContent.DataColumns.Count
					});
					GlobalSettings.AdditionalColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;
				}
				else
				{
					GlobalSettings.WBidINIContent.SummaryVacationColumns.Add(new DataColumn()
					{
						Id = GlobalSettings.columndefinition.FirstOrDefault(x => x.DisplayName == "Ratio").Id,
						Width = 50,
						Order = GlobalSettings.WBidINIContent.DataColumns.Count
					});
					GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.DisplayName == "Ratio").IsSelected = true;

				}
				WBidHelper.SetRatioValues(wBidStateContent);


				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
                GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());
                LoadContent();
			}
			else
			{
				RatioViewControllerController login = new RatioViewControllerController();
				login.FromScreen = "Summary";
				var panel = new NSPanel();
				CommonClass.Panel = panel;
				login.panel = panel;
				panel.SetContentSize(new CoreGraphics.CGSize(293, 221));
				panel.ContentView = login.View;
				NSApplication.SharedApplication.BeginSheet(panel, this.View.Window);
			}
		}
	


		public void MoveLineUp ()
		{

			var row = tblsummary.SelectedRow;
			if (row != 0) {
				tblsummary.SelectRow (row - 1, false);
				tblsummary.ScrollRowToVisible (row - 1);
			}
		}

		public void MoveLineDown ()
		{

			var row = tblsummary.SelectedRow;
			if (row != GlobalSettings.Lines.Count - 1) {
				tblsummary.SelectRow (row + 1, false);
				tblsummary.ScrollRowToVisible (row + 1);
			}
		}

		public void LoadContent ()
		{
			SortCalculation sort = new SortCalculation ();
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty) {
				sort.SortLines (wBidStateContent.SortDetails.SortColumn);
			}
			if (wBidStateContent.TagDetails != null)
            {
                
				wBidStateContent.TagDetails.ForEach (x => GlobalSettings.Lines.FirstOrDefault (y => y.LineNum == x.Line).Tag = x.Content);
			}
			if (wBidStateContent.SortDetails.SortColumn == "SelectedColumn") {
				string colName = wBidStateContent.SortDetails.SortColumnName;
				string direction = wBidStateContent.SortDetails.SortDirection;
				if (colName == "LineNum")
					colName = "LineDisplay";
				if (colName == "LastArrivalTime")
					colName = "LastArrTime";
				if (colName == "StartDowOrder")
					colName = "StartDow";

				var datapropertyId = GlobalSettings.columndefinition.FirstOrDefault (x => x.DataPropertyName == colName).Id;
				CommonClass.columnID = datapropertyId;
				if (direction == "Asc")
					CommonClass.columnAscend = true;
				else
					CommonClass.columnAscend = false;
				Console.WriteLine (datapropertyId);
			}

//			var objects = arrColumn.ArrangedObjects ();
//			arrColumn.Remove (objects);
			var selected = tblsummary.SelectedRow;
			while (tblsummary.ColumnCount > 0) {
				tblsummary.RemoveColumn (tblsummary.TableColumns ().Last ());
			}

			int index = 0;
			List<DataColumn> columns;
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
				columns = GlobalSettings.WBidINIContent.SummaryVacationColumns;
			} else {
				columns = GlobalSettings.WBidINIContent.DataColumns;
			}
			foreach (var item in columns) {
				var columnDef = GlobalSettings.columndefinition.Where (x => x.Id == item.Id).FirstOrDefault ();
				var newColumn = new NSTableColumn ();
				newColumn.Editable = false;
				newColumn.HeaderCell.Alignment = NSTextAlignment.Center;
				newColumn.DataCell.Alignment = NSTextAlignment.Center;
				newColumn.Identifier = columnDef.DisplayName;
				if (index == 0)
				{
					newColumn.HeaderCell.Title = columnDef.DisplayName;
					newColumn.Width = 40;
					newColumn.DataCell = new NSTextFieldCell();
				}
				else if (index == 1 | index == 2 | index == 3)
				{
					var img = ColTitle[columnDef.DisplayName];
					img.Size = new CGSize(15, 15);
					newColumn.HeaderCell.Image = img;
					newColumn.Width = 30;
					newColumn.DataCell = new NSImageCell();

				}
				//else if (index == 4) 
				else if (columnDef.DisplayName == "Grp")
				{
					newColumn.HeaderCell.Title = columnDef.DisplayName;
					newColumn.Width = 60;
					NSButtonCell btn = new NSButtonCell("s");
					btn.BezelStyle = NSBezelStyle.TexturedSquare;

					newColumn.DataCell = btn;

					SetAllignment(newColumn.HeaderCell.Title.ToLower(), newColumn.DataCell);
				}
				else
				{
					newColumn.HeaderCell.Title = columnDef.DisplayName;
					newColumn.Width = 60;
					newColumn.DataCell = new NSTextFieldCell();
					SetAllignment(newColumn.HeaderCell.Title.ToLower(), newColumn.DataCell);
					//newColumn.DataCell.Alignment = NSTextAlignment.Center;
				}
				if (CommonClass.columnID > 4 && columnDef.Id == CommonClass.columnID) {
					newColumn.HeaderCell.Highlighted = true;
					if (!CommonClass.columnAscend) {
						newColumn.HeaderCell.Tag = 2;
						newColumn.HeaderCell.Title = newColumn.HeaderCell.Title + " ▼";
					} else {
						newColumn.HeaderCell.Tag = 1;
						newColumn.HeaderCell.Title = newColumn.HeaderCell.Title + " ▲";
					}
				}
				if (newColumn.Identifier == "MyValue")
					newColumn.Width = 70;
				if (newColumn.Identifier == "Tag") {
					newColumn.Editable = true;
					((NSTextFieldCell)newColumn.DataCell).Editable = true;


				}
				tblsummary.AddColumn (newColumn);
				index++;

			}
			CommonClass.MainController.RemoveBtnEnableDisable ();

			if (tblsummary.SelectedRowCount > 0) {
				var rows = tblsummary.SelectedRows.ToArray ();
				foreach (var item in rows) {
					if ((int)item != tblsummary.ClickedRow)
						tblsummary.DeselectRow (Convert.ToInt32 (item));
				}
				CommonClass.selectedRows.Clear ();
				CommonClass.MainController.LockBtnEnableDisable ();
			}
			tblsummary.SelectRow (selected, false);
			//
			//tblsummary.SelectRow (-1, true);;
		}

		void tblsummaryDoubleClicked (object sender, EventArgs e)
		{
			if (((NSTableView)sender).ClickedRow != -1 && ((NSTableView)sender).SelectedRowCount > 0) {
				var rows = ((NSTableView)sender).SelectedRows.ToArray ();
				foreach (var item in rows) {
					var intItem = Convert.ToInt32 (item);
					if ((int)item != ((NSTableView)sender).ClickedRow)
						tblsummary.DeselectRow (intItem);
					CommonClass.selectedLine = GlobalSettings.Lines [intItem].LineNum;
					CommonClass.calData = CalendarViewBL.GenerateCalendarDetails (GlobalSettings.Lines [intItem]);
				}
				CommonClass.MainController.ShowClendarView ();
			}
		}

		public void ReloadContent ()
		{
			SortCalculation sort = new SortCalculation ();
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty) {
				sort.SortLines (wBidStateContent.SortDetails.SortColumn);
			}
			if (wBidStateContent.TagDetails != null) {
				wBidStateContent.TagDetails.ForEach (x => GlobalSettings.Lines.FirstOrDefault (y => y.LineNum == x.Line).Tag = x.Content);
			}
			tblsummary.ReloadData ();
			var row = tblsummary.SelectedRow;
			tblsummary.DeselectRow (row);
			tblsummary.SelectRow (row, false);
			CommonClass.MainController.RemoveBtnEnableDisable ();

		}


		private void SetAllignment (string displayName, NSCell column)
		{
			switch (displayName) {
			case "line":
			case "myvalue":
			case "pay":
			case "flt":
			case "pdiem":
			case "$/hr":
			case "$/day":
			case "vacpaycu":
			case "vofrnt":
			case "vobk":
			case "vdrop":
			case "co":
			case "+off":
			case "off":
			case "dhrinline":
			case "t234":
			case "$/dhr":
			case "$/tafb":
			case "1dy":
			case "2dy":
			case "3dy":
			case "4dy":
			case "larr":
			case "+legs":
			case "legs":
			case "a/p":
			case "+grd":
			case "8753":
			case "acchg":
			case "acday":
			case "dp":
			case "dpinbp":
			case "edompush":
			case "epush":
			case "ldomarr":
			case "lgpair":
			case "lgday":
			case "odrop":
			case "pairs":
			case "sips":
			case "wkend":
			case "tafbrig":
			case "totrig":
			case "adgrig":
			case "dhrinbp":
			case "dhrrig":
			case "fltrig":
			case "minpayrig":
			case "800legs":
			case "700legs":
			case "500legs":
			case "300legs":
					case "maxlegs":
			case "wts":
			case "linerig":
			case "nmid":
			case "cmts":
			case "cmtfr":
			case "cmtba":
			case "cmt%fr":
			case "cmt%ba":
			case "cmt%ov":
			case "ratio":
			case "etrips":
				case "vacpayne":
                case "vacpaybo":
                    
				column.Alignment = NSTextAlignment.Right;
				break;
			case "startdow":
				column.Alignment = NSTextAlignment.Center;
				break;
			default:
				column.Alignment = NSTextAlignment.Left;
				break;
			}

		}
		public void TableViewReload()
		{
			tblsummary.ReloadData ();

		}
	}

	public partial class SummaryTableSource : NSTableViewSource
	{
		SummaryViewController parentVC;

		public SummaryTableSource (SummaryViewController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return GlobalSettings.Lines.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			if (tableColumn.Identifier == "Lock") {
				if (GlobalSettings.Lines [(int)row].TopLock)
					return NSImage.ImageNamed ("lockIconGreen.png");
				else if (GlobalSettings.Lines [(int)row].BotLock)
					return NSImage.ImageNamed ("lockIconRed.png");
				else
					return null;
			} 

			else if (tableColumn.Identifier == "Grp") {
				NSString value = new NSString (GetLineProperty (tableColumn.Identifier, GlobalSettings.Lines [(int)row], (int)row));
				tableColumn.DataCell = new NSButtonCell ("d");

				NSButtonCell btn = new NSButtonCell (value);
				//btn.DrawWithFrame(new CGRect(-4,0,70,30),tableColumn.DataCell); 
				btn.BezelStyle = NSBezelStyle.TexturedSquare;
				if (!(value.Length > 0))
					btn.BackgroundColor = NSColor.Clear;
				else {
					btn.Activated+= Btn_Activated;
				}
				btn.BackgroundColor = NSColor.Green;
				tableColumn.DataCell =btn;



				return  tableColumn.DataCell;

			}
			else if (tableColumn.Identifier == "Constraint") {
				if (GlobalSettings.Lines [(int)row].Constrained)
					return NSImage.ImageNamed ("constraintIcon.png");
				else
					return null;
			} else if (tableColumn.Identifier == "Overlap") {
				if (GlobalSettings.Lines [(int)row].ShowOverLap)
					return NSImage.ImageNamed ("overlapIcon.png");
				else
					return null;
			} else {

				return new NSString (GetLineProperty (tableColumn.Identifier, GlobalSettings.Lines [(int)row], (int)row));
			}
		}

		void Btn_Activated (object sender, EventArgs e)
		{
			NSTableView table = (NSTableView)sender;
			if (table.SelectedRowCount > 0) {
				//Console.WriteLine (table.SelectedColumn);
				//var column = table.SelectedColumns.ToList ().ConvertAll (x => (int)x);

				var rows = table.SelectedRows.ToList ().ConvertAll (x => (int)x);
				var lines = new List<int> ();
				foreach (var item in rows) {
					var lineNum = GlobalSettings.Lines [item].LineNum;
					lines.Add (lineNum);
				}
				CommonClass.selectedRows = lines;
				CommonClass.MainController.LockBtnEnableDisable ();
				if (CommonClass.CalendarController != null && CommonClass.selectedRows.Count == 1) {
					CommonClass.selectedLine = GlobalSettings.Lines [rows [0]].LineNum;
					CommonClass.calData = CalendarViewBL.GenerateCalendarDetails (GlobalSettings.Lines [rows [0]]);
					CommonClass.CalendarController.LoadContent ();
				}
				CommonClass.selectedLine = GlobalSettings.Lines [rows [0]].LineNum;
				CommonClass.importLine = GlobalSettings.Lines [rows [0]];

			}

			BAgroupWindowController Objgroup = new BAgroupWindowController ();
			
			
			parentVC.View.Window.AddChildWindow (Objgroup.Window, NSWindowOrderingMode.Above);

			NSApplication.SharedApplication.RunModalForWindow (Objgroup.Window);
		}



		public override void WillDisplayCell(NSTableView tableView, NSObject cell, NSTableColumn tableColumn, nint row)
		{
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
            if (tableColumn.Identifier != "Lock" && tableColumn.Identifier != "Constraint" && tableColumn.Identifier != "Overlap" && tableColumn.Identifier != "Grp")
			{
				var txt = (NSTextFieldCell)cell;
				if (txt != null)
				{

					Line line = GlobalSettings.Lines[(int)row];
					if (GlobalSettings.Lines[(int)row].BlankLine || GlobalSettings.Lines[(int)row].LineDisplay.Contains("RR"))
					{
						txt.DrawsBackground = true;
                        txt.BackgroundColor = ColorClass.BlankLineColor;
                        if (interfaceStyle == "Dark")
                        {
                            txt.TextColor = NSColor.Black;
                        }
						else
						{
							txt.TextColor = NSColor.Black;
						}
					}
					else if (GlobalSettings.Lines[(int)row].ReserveLine)
					{
						txt.DrawsBackground = true;
                        txt.BackgroundColor = ColorClass.ReserveLineColor;
                        if (interfaceStyle == "Dark")
                        {
                            txt.TextColor = NSColor.White;
                        }
						else
						{
							txt.TextColor = NSColor.Black;
						}
					}
					else if (GlobalSettings.WBidINIContent.User.IsSummaryViewShade)
					{
						if (line.AMPM == "AM")
						{
							txt.DrawsBackground = true;
							txt.BackgroundColor = ColorClass.AmLineSummaryColor;
						}
						else if (line.AMPM == " PM")
						{
							txt.DrawsBackground = true;
							txt.BackgroundColor = ColorClass.PmLineSummaryColor;
						}
						else
						{
							txt.DrawsBackground = false;
							txt.BackgroundColor = NSColor.ControlBackground;
						}

                        if (interfaceStyle == "Dark")
                        {
                            txt.TextColor = NSColor.White;
                        }
						else
						{
							txt.TextColor = NSColor.Black;
						}
					}

					else
					{
						txt.DrawsBackground = false;
						txt.BackgroundColor = NSColor.ControlBackground;
                        if (interfaceStyle == "Dark")
                        {
                            txt.TextColor = NSColor.White;
                        }
						else
                        {
							txt.TextColor = NSColor.Black;
						}
                    }

				}
			}
			if (tableColumn.Identifier == "Grp")
			{
				var btnGroup = (NSButtonCell)cell;
				btnGroup.Bordered = false;
				if (GlobalSettings.Lines[(int)row].BAGroup == null)
				{
					btnGroup.Transparent = true;
					return;
				}
				if (!(GlobalSettings.Lines[(int)row].BAGroup.Length > 0))
				{
					btnGroup.Transparent = true;
					return;
				}

                var interfaceMode = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");
                if (GlobalSettings.Lines[(int)row].IsGrpColorOn == 1)
                {

                    if (interfaceMode == "Dark") {
                        btnGroup.BackgroundColor = NSColor.Gray;
                         }
                    else {
                        btnGroup.BackgroundColor = NSColor.FromCalibratedRgba(185f / 255f, 185f / 255f, 185f / 255f, 1);
                    }



                }
                else
                {
                    if (interfaceMode == "Dark")
                    {
                        btnGroup.BackgroundColor = NSColor.LightGray;
                    }
                    else
                    {
                        btnGroup.BackgroundColor = NSColor.FromCalibratedRgba(230f / 255f, 230f / 255f, 230f / 255f, 1);
                    }
                   
                }




			}
		}

		public override bool ShouldReorder (NSTableView tableView, nint columnIndex, nint newColumnIndex)
		{
			if (columnIndex < 4)
				return false;
			else if (newColumnIndex > -1 && newColumnIndex < 4)
				return false;
			else
				return true;
		}

		public override void ColumnDidMove(NSNotification notification)
		{
			var oldColumn = int.Parse(notification.UserInfo["NSOldColumn"].ToString());
			var newColumn = int.Parse(notification.UserInfo["NSNewColumn"].ToString());
			if (oldColumn != newColumn)
			{
				List<DataColumn> columns;
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
				{
					columns = GlobalSettings.WBidINIContent.SummaryVacationColumns;
				}
				else {
					columns = GlobalSettings.WBidINIContent.DataColumns;
				}
				//DataColumn backupColumn = GlobalSettings.WBidINIContent.DataColumns [oldColumn];
				//GlobalSettings.WBidINIContent.DataColumns.RemoveAt(oldColumn);
				DataColumn backupColumn = columns[oldColumn];
				columns.RemoveAt(oldColumn);

				List<DataColumn> newDataColumns = new List<DataColumn>();
				bool addedBackUpColumn = false;
				for (int i = 0; i < columns.Count; i++)
				{
					if (i == newColumn)
					{
						newDataColumns.Add(backupColumn);
						addedBackUpColumn = true;
					}
					//newDataColumns.Add (GlobalSettings.WBidINIContent.DataColumns [i]);
					newDataColumns.Add(columns[i]);
				}
				if (!addedBackUpColumn)
					newDataColumns.Add(backupColumn);

				int index = 0;
				foreach (DataColumn aDataColmn in newDataColumns)
				{
					aDataColmn.Order = index;
					index++;
				}
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
				{
					GlobalSettings.WBidINIContent.SummaryVacationColumns = new List<DataColumn>();
					GlobalSettings.WBidINIContent.SummaryVacationColumns = newDataColumns;
				}
				else
				{
					GlobalSettings.WBidINIContent.DataColumns = new List<DataColumn>();
					GlobalSettings.WBidINIContent.DataColumns = newDataColumns;
				}
				//columns = newDataColumns;
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());//Save and serialize.
				GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());

			}
		}

	
		public override void DidClickTableColumn (NSTableView tableView, NSTableColumn tableColumn)
		{
			var columnDef = GlobalSettings.columndefinition.Where (x => x.DisplayName == tableColumn.Identifier).FirstOrDefault ();
			if (columnDef.Id > 4) {
				WBidHelper.PushToUndoStack ();
				CommonClass.columnID = columnDef.Id;
				if (tableColumn.HeaderCell.Tag == 0 || tableColumn.HeaderCell.Tag == 1)
					CommonClass.columnAscend = true;
				else
					CommonClass.columnAscend = false;
				LineOperations.SortColumns (columnDef.DataPropertyName, !CommonClass.columnAscend);
				//parentVC.LoadContent ();
				CommonClass.MainController.ReloadAllContent ();
				if (CommonClass.SortController != null) {
					CommonClass.SortController.setValuesToFixedSorts ();

					CommonClass.SortController.setViews ();
				}
			}
		}


		public override void SelectionDidChange (NSNotification notification)
		{			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				//Console.WriteLine (table.SelectedColumn);
				//var column = table.SelectedColumns.ToList ().ConvertAll (x => (int)x);

				var rows = table.SelectedRows.ToList ().ConvertAll (x => (int)x);
				var lines = new List<int> ();
				foreach (var item in rows) {
					var lineNum = GlobalSettings.Lines [item].LineNum;
					lines.Add (lineNum);
				}
				CommonClass.selectedRows = lines;
				CommonClass.MainController.LockBtnEnableDisable ();
				if (CommonClass.CalendarController != null && CommonClass.selectedRows.Count == 1) {
					CommonClass.selectedLine = GlobalSettings.Lines [rows [0]].LineNum;
					CommonClass.calData = CalendarViewBL.GenerateCalendarDetails (GlobalSettings.Lines [rows [0]]);
					CommonClass.CalendarController.LoadContent ();
				}
				CommonClass.selectedLine = GlobalSettings.Lines [rows [0]].LineNum;
				CommonClass.importLine = GlobalSettings.Lines [rows [0]];

				if (CommonClass.importLine.BAGroup != null  ) {
					if (CommonClass.importLine.BAGroup.Length > 0) {
						

					}
				}
//				if (CommonClass.isTKeyPressed) {
//					LineOperations.TopLockThisLineAndAbove (GlobalSettings.Lines [rows [0]].LineNum);
//					parentVC.LoadContent ();
//				} else if (CommonClass.isBKeyPressed) {
//					LineOperations.BottomLockThisLineAndBelow (GlobalSettings.Lines [rows [0]].LineNum);
//					parentVC.LoadContent ();
//				}
			}
		}

		private string GetLineProperty (string displayName, Line line, int row)
		{
			if (displayName == "Ord") {
				return (row + 1).ToString ();
			} else {
				return CommonClass.GetLineProperty (displayName, line);
			}
		}

		public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, nint row)
		{
			if (tableColumn.Identifier == "Tag") {

				if (GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine).Tag != theObject.ToString ()) 
				{
					GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == CommonClass.selectedLine).Tag = theObject.ToString ();
					CommonClass.MainController.UpdateSaveButton (true);
				}

				tableView.ReloadData ();
			}
		}

	}

}

