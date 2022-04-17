
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using MonoTouch.CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;

namespace WBid.WBidMac.Mac
{
	public partial class BASortsViewController : AppKit.NSViewController
	{
		#region Constructors

		public List <string> blockSort = new List <string> ();
		WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListData ();

		// Called when created from unmanaged code
        public BASortsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
        public BASortsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
        public BASortsViewController () : base ("BASortsView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
        public new BASortsView View {
			get {
                return (BASortsView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				btnPayPerFDP.Enabled = false;
				btnPayPerFDP.BezelStyle = NSBezelStyle.SmallSquare;
                if(wBidStateContent.BidAuto.BASort == null)
                    wBidStateContent.BidAuto.BASort = new SortDetails ();

                if (wBidStateContent.BidAuto.BASort.BlokSort == null)
                    wBidStateContent.BidAuto.BASort.BlokSort = new List<string> ();
				tblBlockSort.Source = new BABlockSortSource (this);
				
                btnBlankToBottom.Enabled=false;
                btnBlankToBottom.AlphaValue=(nfloat)0.7;
                btnReserveToBottom.Enabled=false;
                btnReserveToBottom.AlphaValue=(nfloat)0.7;
                segLinePriority.Enabled=false;
				btnBlankToBottom.Activated += btnBlankToBottomClicked;
				btnReserveToBottom.Activated += btnReserveToBottomClicked;
//				btnLineNumber.Activated += HandleSortButtons;
				btnLinePay.Activated += HandleSortButtons;
				btnPayPerDay.Activated += HandleSortButtons;
				btnPayPerDH.Activated += HandleSortButtons;
				btnPayPerFH.Activated += HandleSortButtons;
				btnPayPerFDP.Activated += ShowMessage;
				btnPayPerTimeAway.Activated += HandleSortButtons;
				btnBlockSort.Activated += HandleSortButtons;

				btnBlockDrop.Activated += btnBlockDropClicked;
				btnClear.Activated += ClearTapped;
				setValuesToFixedSorts ();
				setViews ();
				LoadBlockSort ();
				btnHelpSort.Activated += (object sender, EventArgs e) => {
					if (CommonClass.HelpController == null) {
						var help = new HelpWindowController ();
						CommonClass.HelpController = help;
					}
					CommonClass.HelpController.Window.MakeKeyAndOrderFront (this);
					CommonClass.HelpController.tblDocument.SelectRow (3, false);
					CommonClass.HelpController.pdfDocView.GoToPage (CommonClass.HelpController.pdfDocView.Document.GetPage (0));

				};

				Initial_ButtonLayOutSetUp();

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		void Initial_ButtonLayOutSetUp()
		{
			if (btnBlankToBottom.State == NSCellStateValue.On)ButtonActiveLayout (btnBlankToBottom);
			else ButtonInActiveLayout (btnBlankToBottom);


			if (btnReserveToBottom.State == NSCellStateValue.On)ButtonActiveLayout (btnReserveToBottom);
			else ButtonInActiveLayout (btnReserveToBottom);

//			if (btnLineNumber.State == NSCellStateValue.On)ButtonActiveLayout (btnLineNumber);
//			 else ButtonInActiveLayout (btnLineNumber);


			if (btnLinePay.State == NSCellStateValue.On)ButtonActiveLayout (btnLinePay);
			 else ButtonInActiveLayout (btnLinePay);


			if (btnPayPerDay.State == NSCellStateValue.On)ButtonActiveLayout (btnPayPerDay);
			else ButtonInActiveLayout (btnPayPerDay);

			if (btnPayPerDH.State == NSCellStateValue.On)ButtonActiveLayout (btnPayPerDH);
			else ButtonInActiveLayout (btnPayPerDH);

			if (btnPayPerFH.State == NSCellStateValue.On)ButtonActiveLayout (btnPayPerFH);
			else ButtonInActiveLayout (btnPayPerFH);

			if (btnPayPerFDP.State == NSCellStateValue.On)ButtonActiveLayout (btnPayPerFDP);
			else ButtonInActiveLayout (btnPayPerFDP);

			if (btnPayPerTimeAway.State == NSCellStateValue.On)ButtonActiveLayout (btnPayPerTimeAway);
			else ButtonInActiveLayout (btnPayPerTimeAway);

			if (btnBlockSort.State == NSCellStateValue.On)ButtonActiveLayout (btnBlockSort);
			else ButtonInActiveLayout (btnBlockSort);
         

		}
		void LoadBlockSort ()
		{
            if (wBidStateContent.BidAuto.BASort.BlokSort.Count != 0) {
                foreach (var id in wBidStateContent.BidAuto.BASort.BlokSort) {
					var blk = lstblockData.FirstOrDefault (x => x.Id == int.Parse (id)).Name;
					blockSort.Add (blk);
					btnBlockDrop.Items ().FirstOrDefault (x => x.Title == blk).State = NSCellStateValue.On;
				}
				tblBlockSort.ReloadData ();
			}
		}

		void ClearTapped (object sender, EventArgs e)
		{
			try {
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.MessageText = "WBidMax";
				alert.InformativeText = "Are you sure you want to clear Sorts?";
				alert.AddButton ("YES");
				alert.AddButton ("No");
				alert.Buttons [0].Activated += (object sender1, EventArgs ex) => {
					//ClearButtons ();
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
					wBidStateContent.BidAuto.IsBlankBottom=false;
					wBidStateContent.BidAuto.IsReserveBottom=false;
					wBidStateContent.BidAuto.IsReserveFirst=false;
					wBidStateContent.BidAuto.BASort= new SortDetails();
					//ApplySort ("Line Pay");
					CommonClass.MainController.ReloadAllContent ();

				

					CommonClass.BAController.ReloadAllContent ();
					//tblBlockSort.ReloadData();
				};
				alert.RunModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void setValuesToFixedSorts ()
		{
			ClearButtons ();

			if (wBidStateContent.BidAuto.IsBlankBottom) {
				btnBlankToBottom.State = NSCellStateValue.On;
				ButtonActiveLayout (btnBlankToBottom);
			} else
				ButtonInActiveLayout (btnBlankToBottom);

            if (wBidStateContent.BidAuto.IsReserveBottom) {
				btnReserveToBottom.State = NSCellStateValue.On;
				ButtonActiveLayout (btnReserveToBottom);
			} else
				ButtonInActiveLayout (btnReserveToBottom);

			btnBlankToBottom.BezelStyle = NSBezelStyle.TexturedSquare;
			btnReserveToBottom.BezelStyle = NSBezelStyle.TexturedSquare;

            SortDetails stateSortDetails = wBidStateContent.BidAuto.BASort;
//			if (stateSortDetails.SortColumn == "Line" || stateSortDetails.SortColumn == string.Empty) {
//				this.btnLineNumber.State = NSCellStateValue.On;
//				btnBlankToBottom.Enabled = false;
//				btnReserveToBottom.Enabled = false;
//				btnBlankToBottom.BezelStyle = NSBezelStyle.SmallSquare;
//				btnReserveToBottom.BezelStyle = NSBezelStyle.SmallSquare;
//			} else
            if (stateSortDetails.SortColumn == "LinePay") {
				this.btnLinePay.State = NSCellStateValue.On;
			} else if (stateSortDetails.SortColumn == "PayPerDay") {
				this.btnPayPerDay.State = NSCellStateValue.On;
			} else if (stateSortDetails.SortColumn == "PayPerDutyHour") {
				this.btnPayPerDH.State = NSCellStateValue.On;
			} else if (stateSortDetails.SortColumn == "PayPerFlightHour") {
				this.btnPayPerFH.State = NSCellStateValue.On;
			} else if (stateSortDetails.SortColumn == "PayPerTimeAway") {
				this.btnPayPerTimeAway.State = NSCellStateValue.On;
			} else if (stateSortDetails.SortColumn == "BlockSort") {
				this.btnBlockSort.State = NSCellStateValue.On;
			}

//			if (stateSortDetails.SortColumn == "SelectedColumn") {
//				this.lblSelected.BackgroundColor = NSColor.LightGray;

				//this.lblSelected.TextColor = UIColor.White;
//			}

//			if (stateSortDetails.SortColumn == "Manual") {
//				this.lblManual.BackgroundColor = NSColor.LightGray;
				//this.lblManual.TextColor = UIColor.White;
//			}


		}
		void ButtonActiveLayout(NSButton button)
		{

			button.WantsLayer = true;
            button.Layer.BackgroundColor = NSColor.FromCalibratedRgba(.48f, .80f, .14f, 1).CGColor;//NSColor.FromCalibratedRgba(.91f, .51f, .21f, 1).CGColor;
			button.Layer.CornerRadius = (nfloat)2;
			button.Layer.BorderColor = NSColor.DarkGray.CGColor;
			button.Layer.BorderWidth = (nfloat)0.5;
			button.NeedsLayout = true;

		}

		void ButtonInActiveLayout(NSButton button)
		{
			button.WantsLayer = true;//NSColor.FromRgba(124/256,206/256,38/256,1).CGColor;
            button.Layer.BackgroundColor = NSColor.Orange.CGColor;
			button.Layer.CornerRadius = (nfloat)2;
			button.Layer.BorderColor = NSColor.DarkGray.CGColor;
			button.Layer.BorderWidth = (nfloat)0.5;
			button.NeedsLayout = true;
		}

		public void RightClicked (NSEvent ev)
		{
			try {
				//Console.WriteLine (ev.Description);
				var point = this.View.ConvertPointToView (ev.LocationInWindow, tblBlockSort);
				var toRow = tblBlockSort.GetRow (point);
				if (tblBlockSort.SelectedRowCount > 0 && toRow >= 0) {
					WBidHelper.PushToUndoStack ();
					var frmRow = tblBlockSort.SelectedRow;
					var block = blockSort [(int)frmRow];
					blockSort.RemoveAt ((int)frmRow);
					blockSort.Insert ((int)toRow, block);
				
                    var blockID = wBidStateContent.BidAuto.BASort.BlokSort [(int)frmRow];
                    wBidStateContent.BidAuto.BASort.BlokSort.RemoveAt ((int)frmRow);
                    wBidStateContent.BidAuto.BASort.BlokSort.Insert ((int)toRow, blockID);
				
					ApplyBlockSort ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


		void btnBlockDropClicked (object sender, EventArgs e)
		{
			try {
				if (!blockSort.Contains (btnBlockDrop.SelectedItem.Title)) {
					WBidHelper.PushToUndoStack ();
					btnBlockDrop.SelectedItem.State = NSCellStateValue.On;
					blockSort.Add (btnBlockDrop.SelectedItem.Title);
                    wBidStateContent.BidAuto.BASort.BlokSort.Add (lstblockData.FirstOrDefault (x => x.Name == btnBlockDrop.SelectedItem.Title).Id.ToString ());
					ApplyBlockSort ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void ApplyBlockSort ()
		{
			try {

                if (blockSort.Count != 0) {
                    wBidStateContent.BidAuto.BASort.SortColumn = "BlockSort";
					ApplySort ("BlockSort");
					setValuesToFixedSorts ();
					setViews ();
				} 
            else {
					wBidStateContent.BidAuto.BASort.SortColumn = "";
                    wBidStateContent.BidAuto.IsBlankBottom=false;
                    wBidStateContent.BidAuto.IsReserveBottom=false;
                    btnBlankToBottom.Enabled=false;
                    btnReserveToBottom.Enabled=false;
                    segLinePriority.Enabled=false;
					ApplySort ("");
					setValuesToFixedSorts ();
					setViews ();
				}
				CommonClass.columnID = 0;
//				CommonClass.SummaryController.LoadContent ();
				CommonClass.MainController.ReloadAllContent ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
            
		public void RemoveBlockSort (string sort)
		{
			blockSort.Remove (sort);
            wBidStateContent.BidAuto.BASort.BlokSort.Remove (lstblockData.FirstOrDefault (x => x.Name == sort).Id.ToString ());
			btnBlockDrop.Items ().FirstOrDefault (x => x.Title == sort).State = NSCellStateValue.Off;
			ApplyBlockSort ();
		}

		void btnBlankToBottomClicked (object sender, EventArgs e)
		{
			try {
				WBidHelper.PushToUndoStack ();
				if (btnBlankToBottom.State == NSCellStateValue.On) {
//					wBidStateContent.ForceLine.IsBlankLinetoBottom = true;
//					LineOperations.ForceBlankLinestoBottom ();
					ButtonActiveLayout(btnBlankToBottom);
					wBidStateContent.BidAuto.IsBlankBottom=true;
				} else {
					ButtonInActiveLayout(btnBlankToBottom);
//					wBidStateContent.ForceLine.IsBlankLinetoBottom = false;
					wBidStateContent.BidAuto.IsBlankBottom=false;
				}
				CommonClass.MainController.ReloadAllContent ();
                segmentControlEnability ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


		void btnReserveToBottomClicked (object sender, EventArgs e)
		{
			try {
				WBidHelper.PushToUndoStack ();
				if (btnReserveToBottom.State == NSCellStateValue.On) {
//					wBidStateContent.BidAuto.IsReserveBottom= true;
//
//					LineOperations.ForceReserveLinestoBottom ();
					ButtonActiveLayout(btnReserveToBottom);
                    wBidStateContent.BidAuto.IsReserveBottom=true;
				} else {
					
					ButtonInActiveLayout(btnReserveToBottom);
//					wBidStateContent.ForceLine.IsReverseLinetoBottom = false;
					wBidStateContent.BidAuto.IsReserveBottom=false;
				}
				CommonClass.MainController.ReloadAllContent ();
                segmentControlEnability ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

        void segmentControlEnability ()
        {
            if (btnBlankToBottom.State == NSCellStateValue.On && btnReserveToBottom.State == NSCellStateValue.On)
            {
                segLinePriority.Enabled = true;
            }
            else {
                    segLinePriority.Enabled=false;
                 }
        }
		partial void segLinePriorityAction (NSObject sender)
		{
			NSSegmentedControl segButton= (NSSegmentedControl)sender;
			switch(segButton.SelectedSegment)
			{
			case 0: wBidStateContent.BidAuto.IsReserveFirst=true;  break;
			case 1: wBidStateContent.BidAuto.IsReserveFirst=false; break;
			}
		}
		void ClearButtons ()
		{
			try {
				
//				if(btnLineNumber != null)	
//				{
//					btnLineNumber.State = NSCellStateValue.Off;
//					ButtonInActiveLayout (btnLineNumber);
//				}
				if(btnLinePay != null)   
				{
					btnLinePay.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnLinePay);
				}
				if(btnPayPerDay != null)   
				{
					btnPayPerDay.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnPayPerDay);
				}
				if(btnPayPerDH != null)   
				{
					btnPayPerDH.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnPayPerDH);
				}
				if(btnPayPerFH != null)   
				{
					btnPayPerFH.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnPayPerFH);
				}
				if(btnPayPerFDP != null)   
				{
					btnPayPerFDP.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnPayPerFDP);
				}
				if(btnPayPerTimeAway != null)   
				{
					btnPayPerTimeAway.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnPayPerTimeAway);
				}
				if(btnBlockSort != null)  
				{
					btnBlockSort.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnBlockSort);
				}
				if(btnBlankToBottom != null)  
				{
					btnBlankToBottom.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnBlankToBottom);
				}
				if(btnReserveToBottom != null)  
				{
					btnReserveToBottom.State = NSCellStateValue.Off;
					ButtonInActiveLayout (btnReserveToBottom);
				}
//				lblSelected.BackgroundColor = NSColor.White;
//				lblManual.BackgroundColor = NSColor.White;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void ShowMessage (object sender, EventArgs e)
		{
			//
			var alert = new NSAlert ();
			alert.AlertStyle = NSAlertStyle.Warning;
			alert.MessageText = "Information";
			alert.InformativeText = "Not yet functional";
			alert.RunModal ();
		}

		void HandleSortButtons (object sender, EventArgs e)
		{

			try {

				var btn = (NSButton)sender;
				ClearButtons ();
				btn.State = NSCellStateValue.On;
				setViews ();
				ApplySort (btn.Title);

				CommonClass.columnID = 0;
//				CommonClass.SummaryController.LoadContent ();
				CommonClass.MainController.ReloadAllContent ();
				Initial_ButtonLayOutSetUp();
//				if (btn == btnLineNumber)
//				{
//				ButtonActiveLayout (btnLineNumber);
//					ButtonInActiveLayout (btnLinePay);
//					ButtonInActiveLayout (btnPayPerDay);
//					ButtonInActiveLayout (btnPayPerDH);
//				}else 
                if(btn == btnLinePay)
				{
					ButtonActiveLayout (btnLinePay);
					FunEnableReserveBlankLineButton();
                }
             
				else if(btn == btnPayPerDay)
				{
					ButtonActiveLayout (btnPayPerDay);
					FunEnableReserveBlankLineButton();
				}

				else if(btn == btnPayPerDH)
				{
				ButtonActiveLayout (btnPayPerDH);
					FunEnableReserveBlankLineButton();
				}

				else if(btn == btnPayPerFH)
				{
					ButtonActiveLayout (btnPayPerFH);
					FunEnableReserveBlankLineButton();
				}
                else if(btn == btnPayPerTimeAway)
                {
                    ButtonActiveLayout (btnPayPerTimeAway);
					FunEnableReserveBlankLineButton();
                }

              

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void FunEnableReserveBlankLineButton()
		{
			btnBlankToBottom.Enabled=true;
			btnReserveToBottom.Enabled=true;
			btnBlankToBottom.AlphaValue=(nfloat)1.0;
			btnReserveToBottom.AlphaValue=(nfloat)1.0;
            segLinePriority.Enabled = false;
            if( blockSort.Count != 0)
            {
                btnBlockSort.Enabled=true;

            }
            else btnBlockSort.Enabled = false;
		}
		public void setViews ()
		{
			btnBlockDrop.RemoveAllItems ();
			List <string> lstBlockSort = new List <string> ();
			foreach (var item in lstblockData) {
				lstBlockSort.Add (item.Name);
			}
			btnBlockDrop.AddItem ("");
			btnBlockDrop.AddItems (lstBlockSort.ToArray ());
			if (blockSort.Count == 0) {
				btnBlockSort.Enabled = false;
				btnBlockSort.BezelStyle = NSBezelStyle.SmallSquare;
				ButtonInActiveLayout (btnBlockSort);
			}
            else {
				btnBlockSort.Enabled = true;
				btnBlockSort.BezelStyle = NSBezelStyle.TexturedSquare;
				ButtonActiveLayout (btnBlockSort);
			}

           
			foreach (var item in blockSort) {
				btnBlockDrop.Items ().FirstOrDefault (x => x.Title == item).State = NSCellStateValue.On;
			}

			btnBlankToBottom.BezelStyle =  NSBezelStyle.TexturedSquare;
			btnReserveToBottom.BezelStyle =  NSBezelStyle.TexturedSquare;

			if (GlobalSettings.CurrentBidDetails != null && GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M") {
			
				btnBlankToBottom.Enabled = false;
				btnBlankToBottom.State = NSCellStateValue.Off;
				btnReserveToBottom.Enabled = false;
				btnReserveToBottom.State = NSCellStateValue.Off;
				btnBlankToBottom.BezelStyle =  NSBezelStyle.SmallSquare;
				btnReserveToBottom.BezelStyle =  NSBezelStyle.SmallSquare;

			} 
//                else {
//				if (btnLineNumber.State == NSCellStateValue.On) {
//					btnBlankToBottom.Enabled = false;
//					btnBlankToBottom.State = NSCellStateValue.Off;
//					btnReserveToBottom.Enabled = false;
//					btnReserveToBottom.State = NSCellStateValue.Off;
//					btnBlankToBottom.BezelStyle =  NSBezelStyle.SmallSquare;
//					btnReserveToBottom.BezelStyle =  NSBezelStyle.SmallSquare;
//
//				} else {
//					btnBlankToBottom.Enabled = true;
//					btnReserveToBottom.Enabled = true;
//				}
//			}
			tblBlockSort.ReloadData ();
		}

		public void ApplySort (string sortParameter)
		{
			string sortKey = string.Empty;
			SortCalculation sort = new SortCalculation ();
			switch (sortParameter) {
			case "Line Number":
				sortKey = "Line";
				break;
			case "Line Pay":
				sortKey = "LinePay";
				break;
			case "Pay Per Day":
				sortKey = "PayPerDay";
				break;
			case "Pay Per Duty Hour":
				sortKey = "PayPerDutyHour";
				break;

			case "Pay Per Flight Hour":
				sortKey = "PayPerFlightHour";
				break;
			case "Pay Per Time Away From Base":
				sortKey = "PayPerTimeAway";
				break;
			case "Selected Column":
				sortKey = "SelectedColumn";
				break;
			case "Block Sort":
				sortKey = "BlockSort";
				break;

			}			
			if (sortKey != "" && sortKey != "BlockSort")
				WBidHelper.PushToUndoStack ();

			if (sortKey != string.Empty) {
				sort.SortLines (sortKey);
                wBidStateContent.BidAuto.BASort.SortColumn = sortKey;
			}
		}

	}

	public partial class BABlockSortSource : NSTableViewSource
	{
		BASortsViewController parentVC;

		public BABlockSortSource (BASortsViewController parent)
		{
			parentVC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentVC.blockSort.Count;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
            var vw = (BASortCell)tableView.MakeView ("Sort", this);
			vw.BindData (parentVC.blockSort [(int)row], (int)row);
			return vw;
		}

	}

}

