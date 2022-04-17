using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidMac.Mac
{
	public partial class BAFilterViewController : AppKit.NSViewController
	{
		#region Constructors
		NSObject CLONotification;
		NSObject daysOfMonthNotification;
		NSObject OverNightCitiesNotification;
		NSObject CLOLoadNotification;
		public List<string> appliedFilters = new List<string> ();
		WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		// Called when created from unmanaged code
		//BidAutomatorCalculations constCalc = new BidAutomatorCalculations ();
		public BAFilterViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BAFilterViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public BAFilterViewController () : base ("BAFilterView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}




		public void ShowCommuteDetails(NSNotification n)
		{
			ShowCommutableLine ();
		}
		public void ShowCommutableLine()
		{
			//			ArrivalAndDepartViewController ObjArrival= new ArrivalAndDepartViewController();
			//			this.PresentViewControllerAsSheet(ObjArrival);
			//			return;
			CLONotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"CLONotification", AddCLOToContraints);
			CommutableViewController ObjCommute = new CommutableViewController ();
			ObjCommute.Objtype = CommutableAutoFrom.Filters;
			this.PresentViewControllerAsSheet (ObjCommute);


		}

		public void ShowDaysOfMonths(NSNotification n)
		{
			ShowDaysOfMonth ();
		}
		public void ShowDaysOfMonth()
		{
			//			ArrivalAndDepartViewController ObjArrival= new ArrivalAndDepartViewController();
			//			this.PresentViewControllerAsSheet(ObjArrival);
			//			return;
			daysOfMonthNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"daysOfMonthNotification", AddMonthsToFilter);
			DaysOfMonthController ObjdaysOfMonth = new DaysOfMonthController ();

			this.PresentViewControllerAsSheet (ObjdaysOfMonth);


		}

		public void ShowDaysOfMonthCellClick(DaysOfMonthCx daysOfMonth)
		{
			//			ArrivalAndDepartViewController ObjArrival= new ArrivalAndDepartViewController();
			//			this.PresentViewControllerAsSheet(ObjArrival);
			//			return;
			daysOfMonthNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"ClickdaysOfMonthNotification", UpdateMonthsToFilter);
			DaysOfMonthController ObjdaysOfMonth = new DaysOfMonthController ();
			ObjdaysOfMonth.objData = daysOfMonth;
			this.PresentViewControllerAsSheet (ObjdaysOfMonth);


		}
		private void UpdateMonthsToFilter(NSNotification n)
		{

		
			LoadAdditionalConstraints();
			tblFilters.ReloadData ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (daysOfMonthNotification);


		}

		public void ShowOvernightCities()
		{
			//			ArrivalAndDepartViewController ObjArrival= new ArrivalAndDepartViewController();
			//			this.PresentViewControllerAsSheet(ObjArrival);
			//			return;
			OverNightCitiesNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"OverNightCitiesNotification", AddOverNightcities);
			OvernightCitiesDetailsController ObjOverNightCities = new OvernightCitiesDetailsController ();
			this.PresentViewControllerAsSheet (ObjOverNightCities);


		}
		public void ShowOvernightCitiesCellClick(BulkOvernightCityCx objOverNightCity)
		{
			//			ArrivalAndDepartViewController ObjArrival= new ArrivalAndDepartViewController();
			//			this.PresentViewControllerAsSheet(ObjArrival);
			//			return;
			OverNightCitiesNotification = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"UpdateOverNightCitiesNotification", UpdateOverNightcities);
			OvernightCitiesDetailsController ObjOverNightCities = new OvernightCitiesDetailsController ();
			ObjOverNightCities._BulkOverNightCity = objOverNightCity;
			this.PresentViewControllerAsSheet (ObjOverNightCities);


		}
		private void UpdateOverNightcities(NSNotification n)
		{
			//			if (!appliedFilters.Contains ("OC")) 
			//			{
		//	appliedFilters.Add ("OC");
			//			}
			LoadAdditionalConstraints ();

			tblFilters.ReloadData ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (OverNightCitiesNotification);


		}

		private void AddOverNightcities(NSNotification n)
		{
//			if (!appliedFilters.Contains ("OC")) 
//			{
				appliedFilters.Add ("OC");
//			}
			LoadAdditionalConstraints ();

			tblFilters.ReloadData ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (OverNightCitiesNotification);


		}
		private void AddMonthsToFilter(NSNotification n)
		{

//			if (!appliedFilters.Contains ("DOM")) 
//			{
				appliedFilters.Add ("DOM");
//			}
			LoadAdditionalConstraints();
			tblFilters.ReloadData ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (daysOfMonthNotification);


		}


		private void AddCLOToContraints(NSNotification n)
		{
			//FtCommutableLine ftCom=	(FtCommutableLine)n.Object as FtCommutableLine;
			if(!appliedFilters.Contains("CL"))
				appliedFilters.Add ("CL");
			//ApplyAndReloadConstraints ("Commutable Lines - Auto");
		
//			if (!wBIdStateContent.BidAuto.BAFilter.Contains ("Commutable Lines - Auto")) {
//				BidAutoItem bidAutoItem = new BidAutoItem ();
//				appliedFilters.Add (CommonClass.DicFilters["Commutable Lines - Auto"]);
//				bidAutoItem.Name = "CLO";
//				bidAutoItem.BidAutoObject = new FtCommutableLine()
//				{
//					
//				};
//				wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
//				tblFilters.ReloadData ();
//
//				//wBIdStateContent.BidAuto.BAFilter.Add ("Commutable Lines - Auto");
//			}
//			
			LoadAdditionalConstraints();
			tblFilters.ReloadData ();
			NSNotificationCenter.DefaultCenter.RemoveObserver (CLONotification);


		}

		public override void ViewDidDisappear ()
		{
			base.ViewDidDisappear ();

			//NSNotificationCenter.DefaultCenter.RemoveObserver (CLOLoadNotification);

		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.View.WantsLayer = true;
				btnPopUp.RemoveAllItems();
			btnPopUp.Title= "Add Filters";
				btnPopUp.AddItems (CommonClass.DicFilters.Keys.ToArray ());
				btnPopUp.Activated += btnPopUpClicked;

				if(wBIdStateContent.BidAuto == null) wBIdStateContent.BidAuto = new BidAutomator();

				if(wBIdStateContent.BidAuto.BAFilter == null)wBIdStateContent.BidAuto.BAFilter = new List<BidAutoItem>();

				//wBIdStateContent.BidAuto.BAFilter = new List<BidAutoItem>();
				appliedFilters= new List<string>();
				appliedFilters=wBIdStateContent.BidAuto.BAFilter.Select(x=>x.Name).ToList();


				tblFilters.Source = new FilterTableSource (this);

				btnClear.Activated += ClearTapped;
				LoadAdditionalConstraints ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		void ClearTapped (object sender, EventArgs e)
		{
			try {
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Are you sure you want to clear filters";
				alert.AddButton ("YES");
				alert.AddButton ("No");
				alert.Buttons [0].Activated += (object sender1, EventArgs ex) => {
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
//					//clear group number
//					GlobalSettings.Lines.ToList().ForEach(x => { x.BAGroup = string.Empty; x.IsGrpColorOn = 0; });

				//	WBidHelper.PushToUndoStack ();
					//wBIdStateContent.BidAuto.BAFilter= new List<BidAutoItem>();

					if (wBIdStateContent.BidAuto != null)
					{
						wBIdStateContent.BidAuto.BAFilter.Clear();

					}
					//wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
					CommonClass.MainController.ReloadAllContent ();
					CommonClass.BAController.ReloadAllContent ();
				};
				alert.RunModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		void LoadAdditionalConstraints()
		{
			for (int item = 0; item < appliedFilters.Count; item++) {
				btnPopUp.Items ().FirstOrDefault (x => x.Title == (CommonClass.DicFilters.FirstOrDefault (y => y.Value == appliedFilters[item]).Key)).State = NSCellStateValue.On;
			}

		}
		#endregion

		//strongly typed view accessor
		public new BAFilterView View {
			get {
				return (BAFilterView)base.View;
			}
		}
		bool isMultiple (string str)
		{
			if (str == "CL" || str == "RT")
				return false;
			else
				return true;
}



		void btnPopUpClicked (object sender, EventArgs e)
		{
		//	LoadAdditionalConstraints ();
			var btn = (NSPopUpButton)sender;
			Console.WriteLine (btn.SelectedItem.Title);
			//return;
			try {
				
				if(wBIdStateContent.BidAuto == null) wBIdStateContent.BidAuto = new BidAutomator();

				if(wBIdStateContent.BidAuto.BAFilter == null)wBIdStateContent.BidAuto.BAFilter = new List<BidAutoItem>();
				btnPopUp.SelectedItem.State = NSCellStateValue.On;
				if(CommonClass.DicFilters[btn.SelectedItem.Title] == "CL" || CommonClass.DicFilters[btn.SelectedItem.Title] == "DOM" || CommonClass.DicFilters[btn.SelectedItem.Title] == "OC") btnPopUp.SelectedItem.State = NSCellStateValue.Off;

				BidAutoItem bidAutoItem = new BidAutoItem();
				//bidAutoItem.Priority =  appliedFilters.Count +1 ;
				bidAutoItem.IsApplied = false; 
					switch (CommonClass.DicFilters[btn.SelectedItem.Title])
					{


					case "DOWA":
					
					appliedFilters.Add (CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "DOWA";
					bidAutoItem.BidAutoObject = new CxDays()
					{
						IsMon=true,
						IsTue=true,
						IsWed=true,
						IsThu=true,
						IsFri=true,
						IsSat=true,
						IsSun=true
					};
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
					break;
					case "AP":
					appliedFilters.Add (CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "AP";
					bidAutoItem.BidAutoObject = new AMPMConstriants()
					{
						AM = true,
						PM = true,
						MIX = true
					};
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
					break;


					case "DOWS":
					appliedFilters.Add(CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "DOWS";
					bidAutoItem.BidAutoObject = new Cx3Parameter();
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
						break;
					case "DHFL":
					appliedFilters.Add(CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "DHFL";
					bidAutoItem.BidAutoObject = new Cx3Parameter();
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
						break;

					case "ET":
					appliedFilters.Add(CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "ET";
					bidAutoItem.BidAutoObject = new Cx3Parameter();
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
						break;
					case "RT":
					if (appliedFilters.Contains("RT")) break;
					else
					{
						appliedFilters.Add(CommonClass.DicFilters[btn.SelectedItem.Title]);
						bidAutoItem.Name = "RT";
						bidAutoItem.BidAutoObject = new Cx3Parameter();
						bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
						wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
					}
						break;
					case "LT":
					appliedFilters.Add (CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "LT";
					CxLine objCxLine=new CxLine

					{
						Hard = true,
						Reserve = true,
						International = true,
						NonConus=true
					};
					if (GlobalSettings.CurrentBidDetails.Postion=="FA")
					{
						objCxLine.Ready=true;
						
					}
					else{
						objCxLine.Blank=true;
					}
					bidAutoItem.BidAutoObject = objCxLine;
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
						break;
					case "TBL":
					appliedFilters.Add(CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "TBL";
					bidAutoItem.BidAutoObject = new CxTripBlockLength();
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
						break;
					case "SDOW":
					appliedFilters.Add (CommonClass.DicFilters[btn.SelectedItem.Title]);
					bidAutoItem.Name = "SDOW";
					bidAutoItem.BidAutoObject = new CxDays()
					{
						IsMon=false,
						IsTue=false,
						IsWed=false,
						IsThu=false,
						IsFri=false,
						IsSat=false,
						IsSun=false
					};
					bidAutoItem.Priority=wBIdStateContent.BidAuto.BAFilter.Count;
					wBIdStateContent.BidAuto.BAFilter.Add(bidAutoItem);
						break;

					case "DOM":
//					if (appliedFilters.Contains("DOM")) 
//						break;
//					else
//					{
						ShowDaysOfMonth();
						return;
//					}
//					break;

					case "OC":
					
//					if (appliedFilters.Contains("OC")) 
//						break;
//					else
//					{
						ShowOvernightCities();
						return;
//					}


				case "CL":
					if (appliedFilters.Contains("CL")) break;
					else
					{
						ShowCommutableLine();
						return;
					}
						//break;

					}


				tblFilters.ReloadData();
				CommonClass.MainController.UpdateSaveButton (true);
//				} else
//					return;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


		public void RemoveAndReloadConstraints (string filters, int order)
		{  
			

		
			if (filters == "CL" || filters == "RT")
				btnPopUp.Items ().FirstOrDefault (x => x.Title == (CommonClass.DicFilters.FirstOrDefault (y => y.Value == filters).Key)).State = NSCellStateValue.Off;
			else {
				if (appliedFilters.Count (x => x == filters) == 1) {
					btnPopUp.Items ().FirstOrDefault (x => x.Title == (CommonClass.DicFilters.FirstOrDefault (y => y.Value == filters).Key)).State = NSCellStateValue.Off;
				}
			}
				
			wBIdStateContent.BidAuto.BAFilter.RemoveAt (order);
			appliedFilters.RemoveAt (order);
            for (int item=0;item<wBIdStateContent.BidAuto.BAFilter.Count;item++  )
            {
                          
                BidAutoItem Biditem= wBIdStateContent.BidAuto.BAFilter [item];
                Biditem.Priority=item;


            }
			tblFilters.ReloadData ();

			return;

				switch (filters) {
				case "DOWA":
				
					break;
				case "AP":
				
					break;
				case "DOWS":
				
					break;
				case "DHFL":
				
					break;

				case "ET":
				
					break;
				case "RT":
				
					break;
				case "LT":
				
					break;
				case "TBL":
				
					break;
				case "SDOW":
				
					break;

				case "DOM":
				
					break;
				case "OC":
				
					break;
			case "CL":
				
					break;

				}



//				} else if (constraint == "Commutable Lines") {
//					btnPopUp.Items ().FirstOrDefault (x => x.Title == constraint).State = NSCellStateValue.Off;
//					wBIdStateContent.CxWtState.CL.Cx = false;
//					constCalc.RemoveCommutableLinesConstraint ();
//					appliedFilters.Remove (constraint);
//					tblFilters.ReloadData ();
//				}
//				CommonClass.MainController.ReloadAllContent ();
//				//lblLineCount.StringValue = constCalc.LinesNotConstrained ();
//			} catch (Exception ex) {
//				CommonClass.AppDelegate.ErrorLog (ex);
//				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
//			}
		}
		public void ReloadContent()
		{
			tblFilters.ReloadData ();
		}
		public void RightClicked (NSEvent ev)
		{
			try {
				//Console.WriteLine (ev.Description);
				var point = this.View.ConvertPointToView (ev.LocationInWindow, tblFilters);
				var toRow = tblFilters.GetRow (point);
				if (tblFilters.SelectedRowCount > 0 && toRow >= 0) {
					WBidHelper.PushToUndoStack ();
					var frmRow = tblFilters.SelectedRow;

					var block = appliedFilters [(int)frmRow];
					appliedFilters.RemoveAt ((int)frmRow);
					appliedFilters.Insert ((int)toRow, block);

					var blockID = wBIdStateContent.BidAuto.BAFilter[(int)frmRow];
					wBIdStateContent.BidAuto.BAFilter.RemoveAt ((int)frmRow);
					wBIdStateContent.BidAuto.BAFilter.Insert ((int)toRow, blockID);


				//	nslog
				
					for (int item=0;item<wBIdStateContent.BidAuto.BAFilter.Count;item++	 )
					{

//						((BidAutoItem)wBIdStateContent.BidAuto.BAFilter [(nint)row]).Priority=(int)row+1;
						BidAutoItem Biditem= wBIdStateContent.BidAuto.BAFilter [item];
						Biditem.Priority=item;
						if(item >=toRow) Biditem.IsApplied=false;

					}

					tblFilters.ReloadData();

				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public partial class FilterTableSource : NSTableViewSource
		{
			BAFilterViewController parentVC;

			public FilterTableSource (BAFilterViewController parent)
			{
				parentVC = parent;
				//this.RegisterForDraggedTypes(new string[]{ "MyDragDropPasteType" });
			}

			public override nint GetRowCount (NSTableView tableView)
			{
				return parentVC.appliedFilters.Count;
			}


			public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
			{


				var vw = (FiltersCell)tableView.MakeView (parentVC.appliedFilters.ElementAt((int)row), this);
				vw.WantsLayer = true;
				vw.BindData (parentVC.appliedFilters [(int)row].ToString(), (int)row,parentVC);

				return vw;
			}

			public override nfloat GetRowHeight (NSTableView tableView, nint row)
			{
				
					return 30;
			}

			public override bool SelectionShouldChange (NSTableView tableView)
			{
				return true;
			}

			public override bool AcceptDrop (NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
			{
				return true;
			}
			public override NSDragOperation ValidateDrop (NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
			{
				return NSDragOperation.All;
			}
			public override void DidClickTableColumn (NSTableView tableView, NSTableColumn tableColumn)
			{
				Console.WriteLine ("Clicked");

//				var columnDef = GlobalSettings.columndefinition.Where (x => x.DisplayName == tableColumn.Identifier).FirstOrDefault ();
//				if (columnDef.Id > 4) {
//					WBidHelper.PushToUndoStack ();
//					CommonClass.columnID = columnDef.Id;
//					if (tableColumn.HeaderCell.Tag == 0 || tableColumn.HeaderCell.Tag == 1)
//						CommonClass.columnAscend = true;
//					else
//						CommonClass.columnAscend = false;
//					LineOperations.SortColumns (columnDef.DataPropertyName, !CommonClass.columnAscend);
//					//parentVC.LoadContent ();
//					CommonClass.MainController.ReloadAllContent ();
//					if (CommonClass.SortController != null) {
//						CommonClass.SortController.setValuesToFixedSorts ();
//						CommonClass.SortController.setViews ();
//					}
//				}
			}
			public override void WillDisplayCell (NSTableView tableView, NSObject cell, NSTableColumn tableColumn, nint row)
			{
			
//				var txt = (FiltersCell)cell;
//				if (txt != null) {
//					BidAutoItem Biditem= parentVC.wBIdStateContent.BidAuto.BAFilter [(int)row];
//					txt.DrawsBackground = true;
//					if (!Biditem.IsApplied) {
//						
//						txt.BackgroundColor = NSColor.Red;
//					}
//					else 
//					{
//						txt.BackgroundColor = NSColor.White;
//					} 
//
//			}
			}

			public override void SelectionDidChange (NSNotification notification)
			{
				Console.WriteLine ("Clicked");

//				var table = (NSTableView)notification.Object;
//				NSTableRowView rowView = table.GetRowView (table.SelectedRow, false);
//				rowView.BackgroundColor = NSColor.Red;

//				if (table.SelectedRowCount > 0) {
//					var rows = table.SelectedRows.ToList ().ConvertAll (x => (int)x);
//					var lines = new List<int> ();
//					foreach (var item in rows) {
//						var lineNum = GlobalSettings.Lines [item].LineNum;
//						lines.Add (lineNum);
//					}
//					CommonClass.selectedRows = lines;
//					CommonClass.MainController.LockBtnEnableDisable ();
//					if (CommonClass.CalendarController != null && CommonClass.selectedRows.Count == 1) {
//						CommonClass.selectedLine = GlobalSettings.Lines [rows [0]].LineNum;
//						CommonClass.calData = CalendarViewBL.GenerateCalendarDetails (GlobalSettings.Lines [rows [0]]);
//						CommonClass.CalendarController.LoadContent ();
//					}
//					CommonClass.selectedLine = GlobalSettings.Lines [rows [0]].LineNum;
//					CommonClass.importLine = GlobalSettings.Lines [rows [0]];
//					//				if (CommonClass.isTKeyPressed) {
//					//					LineOperations.TopLockThisLineAndAbove (GlobalSettings.Lines [rows [0]].LineNum);
//					//					parentVC.LoadContent ();
//					//				}  else if (CommonClass.isBKeyPressed) {
//					//					LineOperations.BottomLockThisLineAndBelow (GlobalSettings.Lines [rows [0]].LineNum);
//					//					parentVC.LoadContent ();
//					//				}
//				}
			}
//			public override bool WriteRows (NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard)
//			{
////				NSData *data = [NSKeyedArchiver archivedDataWithRootObject:rowIndexes];
////
////				[pboard declareTypes:[NSArray arrayWithObject:MyPrivateTableViewDataType] owner:controller];
////
////				[pboard setData:data forType:MyPrivateTableViewDataType];
////
////				NSData dataaa= new NSKeyedArchiver.archive
////
////				return YES;
//
//			}

		}
	}
}
