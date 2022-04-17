
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
using CoreGraphics;

namespace WBid.WBidMac.Mac
{
	public partial class FAPositionChoiceWindowController : AppKit.NSWindowController
	{
		#region Constructors

		bool ChangeBuddy = false;

		private string _buddyposition1 { get; set; }

		private string _buddyposition2 { get; set; }

		private string _buddyposition3 { get; set; }

		string[] availablePositions = { "A", "B", "C", "D", "None" };
		public string[] availablePositionSelections = {
			"ABC",
			"ACB",
			"BAC",
			"BCA",
			"CAB",
			"CBA",
			"AB",
			"AC",
			"BA",
			"BC",
			"CA",
			"CB",
			"A",
			"B",
			"C"
		};
		public List<int> availableLines = GlobalSettings.Lines.Select (x => x.LineNum).ToList ();
		public List<int> selectedLines = new List<int> ();
		public List<int> leftSelection = new List<int> ();
		public List<int> rightSelection = new List<int> ();

		public List<string> availableLinesWithPos = new List<string> ();
		public List<string> selectedLinesWithPos = new List<string> ();
		private ObservableCollection<string> AvailalbelistOrder;

		public List<Line> Lines { get; set; }

		//holds the all items in the bid Line list
		public List<string> BidLineList = new List<string> ();

		public List<string> SelectedBidLineList = new List<string> ();

		int APositionLineCount = 0;
		int BPositionLineCount = 0;
		int CPositionLineCount = 0;
		int DPositionLineCount = 0;
		int PositionLineTotal = 0;
		// Called when created from unmanaged code
		public FAPositionChoiceWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public FAPositionChoiceWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public FAPositionChoiceWindowController () : base ("FAPositionChoiceWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new FAPositionChoiceWindow Window {
			get {
				return (FAPositionChoiceWindow)base.Window;
			}
		}

		static NSButton closeButton;

		void SubmitBidopearation ()
		{
			if (btnRepeatCombo.SelectedTag == 2) {
				this.Window.ContentView = vwCustom;
				//bid order
				if (btnOrder.SelectedTag == 0) {
					if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null) {
						Lines = GlobalSettings.Lines.Where (x => !x.FAPositions.Contains ("D")).ToList ();
					}
					else {
						Lines = GlobalSettings.Lines.ToList ();
					}
				}
				// Line order
				else {
					if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null) {
						Lines = GlobalSettings.Lines.Where (x => !x.FAPositions.Contains ("D")).OrderBy (x => x.LineNum).ToList ();
					}
					else {
						Lines = GlobalSettings.Lines.OrderBy (x => x.LineNum).ToList ();
					}
				}
				SetAvailalbeLines ();
				tblAvailableLine.Source = new AvailableTableSource (this);
				tblSelectedLine.Source = new SelectedTableSource (this);
				EnableDisableButtons ();
				lblAvailable.StringValue = availableLinesWithPos.Count + " Lines";
				lblBids.StringValue = selectedLines.Count + " Bids";
			}
			else {
				var query = new QueryWindowController ();
				this.Window.AddChildWindow (query.Window, NSWindowOrderingMode.Above);
				NSApplication.SharedApplication.RunModalForWindow (query.Window);
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				hidePositionView();

				this.ShouldCascadeWindows = false;
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				this.Window.Title = "FA Position Choice";
				this.Window.ContentView = vwMain;
				btnOrder.Enabled = false;
				btnRepeatCombo.Activated += (object sender, EventArgs e) => {

					Lines = GlobalSettings.Lines.ToList();
					if (btnRepeatCombo.SelectedTag == 0)
					{
						btnOrder.Enabled = true;
						hidePositionView();

					}
					else if (btnRepeatCombo.SelectedTag == 1)
					{
						btnOrder.Enabled = true;
						if(GlobalSettings.SubmitBid.IsSubmitAllChoices == false)
						displayPositionView();
						SetRepeatPositionUI();

					}

				   if(btnRepeatCombo.SelectedTag == 2)
					{
						btnOrder.Enabled = true;
						hidePositionView();
								
					}
					else
						btnOrder.Enabled = false;




				};
				
				BindPositions ();
				
				btnFirst.Activated += HandlePositionChange;
				btnSecond.Activated += HandlePositionChange;
				btnThird.Activated += HandlePositionChange;
				btnFourth.Activated += HandlePositionChange;

				txtFirstpositionValue.Changed += delegate
				{
					//if(txtFirstpositionValue.StringValue.Length <=4)
					txtFirstpositionValue.StringValue = txtFirstpositionValue.StringValue;
						};
				txtSecondPositionValue.Changed += delegate
				{
					txtSecondPositionValue.StringValue = txtSecondPositionValue.StringValue;
				};
				txtThirdPositionValue.Changed += delegate
				{
					txtThirdPositionValue.StringValue = txtThirdPositionValue.StringValue;
				};
				txtFourthPositionValue.Changed += delegate
				{
					txtFourthPositionValue.StringValue = txtFourthPositionValue.StringValue;
				};

				lblSubmitTotal.StringValue = "Submit" + GlobalSettings.SubmitBid.SeniorityNumber + " total";
				btnChangeBuddy.Activated += (object sender, EventArgs e) => {
					var panel = new NSPanel ();
					var changeBud = new ChangeBuddyViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (400, 190));
					panel.ContentView = changeBud.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					ChangeBuddy = true;
				};
				this.Window.DidEndSheet += (object sender, EventArgs e) => {
					if ((GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0" || GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0")&& btnAddReserve.State== NSCellStateValue.On)
					{
						btnAddReserve.State=NSCellStateValue.Off;

						WarningAlertMessage ("You cannot add Reserve to the end of your Bid when Buddy Bidding");
					}
					if (ChangeBuddy) {
						LoadBuddyDetails ();
						ChangeBuddy = false;
					}
				};
				
				btnBuddy1.AddItems (availablePositionSelections);
				btnBuddy2.AddItems (availablePositionSelections);
				LoadBuddyDetails ();
				btnAddReserve.Activated+= (object sender, EventArgs e) => 
				{
					if ((GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0" || GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0")&& btnAddReserve.State== NSCellStateValue.On)
					{
						btnAddReserve.State=NSCellStateValue.Off;

						WarningAlertMessage ("You cannot add Reserve to the end of your Bid when Buddy Bidding");
					}
				};
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				btnSubmit.Activated += (object sender, EventArgs e) => {
				
					SetSubmitBid ();
				

				};
				btnCancelCustom.Activated += (object sender, EventArgs e) => {
					this.Window.ContentView = vwMain;
				};
				btnOKCustom.Activated += (object sender, EventArgs e) => {
					if (selectedLinesWithPos.Count != 0) {
				
						//genarate bid line to submit
						SubmitBid submitBid = GlobalSettings.SubmitBid;
						submitBid.Bid = string.Join (",", selectedLinesWithPos);
					   submitBid.TotalBidChoices = selectedLinesWithPos.Count;
						//submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
						var query = new QueryWindowController ();
						this.Window.AddChildWindow (query.Window, NSWindowOrderingMode.Above);
						NSApplication.SharedApplication.RunModalForWindow (query.Window);
					}
				};
				btnAdd.Activated += btnAddClicked;
				btnInsert.Activated += btnInsertClicked;
				btnRemove.Activated += btnRemoveClicked;
				btnClear.Activated += btnClearClicked;

				if (GlobalSettings.CurrentBidDetails.Postion == "FA")
				{
					Lines = GlobalSettings.Lines.ToList();
					var fapositions = Lines.Select(x => x.FAPositions);
					APositionLineCount = fapositions.Count(x => x.Contains("A"));
					BPositionLineCount = fapositions.Count(x => x.Contains("B"));
					CPositionLineCount = fapositions.Count(x => x.Contains("C"));
					DPositionLineCount = fapositions.Count(x => x.Contains("D"));

					lblValueExist1.StringValue = APositionLineCount.ToString();
					lblValueExist2.StringValue = BPositionLineCount.ToString();
					lblValueExist3.StringValue = CPositionLineCount.ToString();

					lblValueExistFirstPos.StringValue = btnFirst.SelectedItem.Title;
					lblValueExistSecondPos.StringValue = btnSecond.SelectedItem.Title;
					lblValueExistSecondPos.StringValue = btnThird.SelectedItem.Title;


					//IsNeedToEnterPosition = (SubmitBidDetails.IsSubmitAllChoices == false && IsRepeatPositions);
					//SubmitTotal = "Submit " + SubmitBidDetails.SeniorityNumber + " Total";
				}

				txtFirstpositionValue.Activated += (object sender, EventArgs e) =>
				{

					if (txtFirstpositionValue.StringValue == string.Empty)
						txtFirstpositionValue.StringValue = "0";
					calculateTotalbidchoiceselected();
				};
				txtSecondPositionValue.Activated += (object sender, EventArgs e) =>
				{

					if (txtSecondPositionValue.StringValue == string.Empty)
						txtSecondPositionValue.StringValue = "0";
					calculateTotalbidchoiceselected();
				};
				txtThirdPositionValue.Activated += (object sender, EventArgs e) =>
				{
									if (txtThirdPositionValue.StringValue == string.Empty)
						txtThirdPositionValue.StringValue = "0";
					calculateTotalbidchoiceselected();
				};
				txtFourthPositionValue.Activated += (object sender, EventArgs e) =>
				{

					if (txtFourthPositionValue.StringValue == string.Empty)
						txtFourthPositionValue.StringValue = "0";
					calculateTotalbidchoiceselected();
				};




				txtFirstpositionValue.StringValue = "0";
				txtSecondPositionValue.StringValue = "0";
				txtThirdPositionValue.StringValue = "0";
				txtFourthPositionValue.StringValue = "0";
				SetRepeatPositionUI();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void hidePositionView()
		{
			vwRepeatPosition.Hidden = true;
			//vwPositionSelection.Frame = new CGRect(10, 240, 377, 85);
		}

		private void displayPositionView()
		{
			vwRepeatPosition.Hidden = false;
		}

		//public void handleTextDidChange(NSNotification obj)
		//{
			
		
		//}	

		private void SetRepeatPositionUI()
		{
			if ((GlobalSettings.SubmitBid.IsSubmitAllChoices == false && btnRepeatCombo.SelectedTag == 1))
			{
				var fapositions = new List<List<string>>();
				if (Lines != null)
					fapositions = Lines.Select(x => x.FAPositions).ToList();


				lblValueExist1.StringValue = fapositions.Count(x => x.Contains(btnFirst.SelectedItem.Title)).ToString();

				lblValueExistFirstPos.StringValue = btnFirst.SelectedItem.Title;

				if (btnSecond.SelectedItem.Title == "None")
				{
					txtSecondPositionValue.Enabled = false;
					txtSecondPositionValue.StringValue = "0";
					lblValueExistSecondPos.StringValue = string.Empty;
					lblValueExist2.StringValue = string.Empty;
				}
				else
				{
					txtSecondPositionValue.Enabled = true;
					lblValueExist2.StringValue = fapositions.Count(x => x.Contains(btnSecond.SelectedItem.Title)).ToString();
					lblValueExistSecondPos.StringValue = btnSecond.SelectedItem.Title;
				}
				if (btnThird.SelectedItem.Title == "None")
				{
					txtThirdPositionValue.Enabled = false;
					txtThirdPositionValue.StringValue = "0";
					lblValueExistThirdPos.StringValue = string.Empty;
					lblValueExist3.StringValue = string.Empty;
				}
				else
				{
					txtThirdPositionValue.Enabled = true;
					lblValueExist3.StringValue = fapositions.Count(x => x.Contains(btnThird.SelectedItem.Title)).ToString();
					lblValueExistThirdPos.StringValue = btnThird.SelectedItem.Title;
				}
				if (btnFourth.SelectedItem.Title == "None")
				{
					txtFourthPositionValue.Enabled = false;
					txtFourthPositionValue.StringValue = "0";
					lblValueExistFourthPos.StringValue = string.Empty;
					lblValueExist4.StringValue = string.Empty;
				}
				else
				{
					txtFourthPositionValue.Enabled = true;
					lblValueExist4.StringValue  = fapositions.Count(x => x.Contains(btnFourth.SelectedItem.Title)).ToString();
					lblValueExistFourthPos.StringValue = btnFourth.SelectedItem.Title;
				}
				calculateTotalbidchoiceselected();
			}
		}
		private void calculateTotalbidchoiceselected()
		{
			if (txtFirstpositionValue.StringValue == string.Empty)
				txtFirstpositionValue.StringValue = "0";
			if (txtSecondPositionValue.StringValue == string.Empty)
				txtSecondPositionValue.StringValue = "0";
			if (txtThirdPositionValue.StringValue == string.Empty)
				txtThirdPositionValue.StringValue = "0";
			if (txtFourthPositionValue.StringValue == string.Empty)
				txtFourthPositionValue.StringValue = "0";
			lblPositionTotal.StringValue = (Convert.ToInt32(txtFirstpositionValue.StringValue) + Convert.ToInt32(txtSecondPositionValue.StringValue) + Convert.ToInt32(txtThirdPositionValue.StringValue) + Convert.ToInt32(txtFourthPositionValue.StringValue)).ToString();
		}


		private void SetSubmitBid ()
		{

			SubmitBid submitBid = GlobalSettings.SubmitBid;
			try {

				if (lblBuddy1.StringValue == string.Empty && lblBuddy2.StringValue == string.Empty) {
					submitBid.Buddy1 = null;
					submitBid.Buddy2 = null;
				} else {

					BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
					if (btnAddReserve.State == NSCellStateValue.On) {
						submitBid.Buddy1 = submitBid.Buddy2 = null;

					} else {
						submitBid.Buddy1 = (buddyBids.Buddy1 == "0") ? null : buddyBids.Buddy1;
						submitBid.Buddy2 = (buddyBids.Buddy2 == "0") ? null : buddyBids.Buddy2;
					}
					//Genarate and store buddy bid line string to the BuddyBidderBids list
					submitBid.BuddyBidderBids = new List<BuddyBidderBid> ();
					//buddy bidder1
					if (submitBid.Buddy1 != null) {
						if (btnBuddy1.SelectedItem.Title != string.Empty) {
							var buddybidline = GenarateBidLineStringForBuddyBidder (btnBuddy1.SelectedItem.Title);
							submitBid.BuddyBidderBids.Add (new BuddyBidderBid { BidLines = buddybidline, BuddyBidder = submitBid.Buddy1 });
						}
					}
					//buddy bidder 2
					if (submitBid.Buddy2 != null) {
						if (btnBuddy2.SelectedItem.Title != string.Empty) {
							var buddybidline = GenarateBidLineStringForBuddyBidder (btnBuddy2.SelectedItem.Title);
							submitBid.BuddyBidderBids.Add (new BuddyBidderBid { BidLines = buddybidline, BuddyBidder = submitBid.Buddy2 });
						}
					}
				}

				//genarate bid line to submit
				submitBid.Bid = GenarateBidLineString (false);
				submitBid.TotalBidChoices = BidLineList.Count;
				submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;

				if (btnAddReserve.State == NSCellStateValue.On) {
					submitBid.Bid += string.IsNullOrEmpty (submitBid.Bid) ? string.Empty : ",";
					submitBid.Bid += "R";
					submitBid.TotalBidChoices++;
				}


			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);

			}
			if (GlobalSettings.SubmitBid.IsSubmitAllChoices == false && btnRepeatCombo.SelectedTag == 1)
			{
				calculateTotalbidchoiceselected();
				APositionLineCount = Convert.ToInt32(txtFirstpositionValue.StringValue);
				BPositionLineCount = Convert.ToInt32(txtSecondPositionValue.StringValue);
				CPositionLineCount = Convert.ToInt32(txtThirdPositionValue.StringValue);
				DPositionLineCount = Convert.ToInt32(txtFourthPositionValue.StringValue);
				if (GlobalSettings.SubmitBid.SeniorityNumber != Convert.ToInt32(lblPositionTotal.StringValue))
				{
					//Xceed.Wpf.Toolkit.MessageBox.Show("You have requested to submit  " + GlobalSettings.SubmitBid.SeniorityNumber + " bid choices, but your individual bid choices for each position do not equal " + GlobalSettings.SubmitBid.SeniorityNumber + " - go back and adjust your individual position numbers", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

					var msg = "You have requested to submit  " + GlobalSettings.SubmitBid.SeniorityNumber + " bid choices, but your individual bid choices for each position do not equal " + GlobalSettings.SubmitBid.SeniorityNumber + " - go back and adjust your individual position numbers";
					WarningAlertMessage(msg);
					return;
				}

				if (APositionLineCount > Convert.ToInt32(lblValueExist1.StringValue == string.Empty ? "0" : lblValueExist1.StringValue))
				{

					var msg = "You cannot submit " + APositionLineCount + " positions. since only " + lblValueExist1.StringValue + " " + lblValueExistFirstPos.StringValue + " exist. \n\nChange your number of " + lblValueExistFirstPos.StringValue + " positions to " + lblValueExist1.StringValue + " or less";
					WarningAlertMessage(msg);
					return;
				}
				if (BPositionLineCount > Convert.ToInt32(lblValueExist2.StringValue == string.Empty ? "0" : lblValueExist2.StringValue))
				{
					var msg = "You cannot submit " + BPositionLineCount + " positions. since only " + lblValueExist2.StringValue + " " + lblValueExistSecondPos.StringValue + " exist. \n\nChange your number of " + lblValueExistSecondPos.StringValue + " positions to " + lblValueExist2.StringValue + " or less";
					WarningAlertMessage(msg);
					return;
				}
				if (CPositionLineCount > Convert.ToInt32(lblValueExist3.StringValue == string.Empty ? "0" : lblValueExist3.StringValue))
				{

					var msg = "You cannot submit " + CPositionLineCount + " positions. since only " + lblValueExist3.StringValue + " " + lblValueExistThirdPos.StringValue + " exist. \n\nChange your number of " + lblValueExistThirdPos.StringValue + " positions to " + lblValueExist3.StringValue + " or less";
					WarningAlertMessage(msg);
					return;
				}
				if (DPositionLineCount > Convert.ToInt32(lblValueExist4.StringValue == string.Empty ? "0" : lblValueExist4.StringValue))
				{


					var msg = "You cannot submit " + DPositionLineCount + " positions. since only " + lblValueExist4.StringValue + " " + lblValueExistFourthPos.StringValue + " exist. \n\nChange your number of " + lblValueExistFourthPos.StringValue + " positions to " + lblValueExist4.StringValue + " or less";
					WarningAlertMessage(msg);
					return;
				}
			}
			bool isNeedtoshownextscreen = true;
			//int dPositionCount = Lines.SelectMany(x => x.FAPositions).Where(y => y == "D").Count();
			int dPositionCount = GlobalSettings.Lines.Where (z => z.FAPositions.Count == 1).SelectMany (x => x.FAPositions).Where (y => y == "D").Count ();

			if ((submitBid.Buddy1 != null || submitBid.Buddy2 != null) && dPositionCount > 0) 
			{
				isNeedtoshownextscreen = false;
				//if (MessageBox.Show(System.Windows.Application.Current.MainWindow, +-- ", "WBidMax", System.Windows.MessageBoxButton.OKCancel) == System.Windows.MessageBoxResult.Cancel)
				//{
				//    return;
				//}
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.MessageText = "WBidMax";
				alert.InformativeText = dPositionCount + " D Lines  were deleted from the bid. \nBuddy Bid Lines can only have A, B or C positions. \nPress OK to continue or CANCEL to return the position Choices. ";

				alert.AddButton ("OK");
				alert.AddButton ("Cancel");
				alert.Buttons [0].Activated += (object sender1, EventArgs ex) => 
				{
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
					SubmitBidopearation ();

					return;
				};
				alert.Buttons [1].Activated += (object sender1, EventArgs ex) => 
				{
					
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
					return;
				};
				alert.RunModal ();
//				alert.Dismissed += (object senderr, UIButtonEventArgs ee) => {
//					if (ee.ButtonIndex == 1) {
//						SubmitBidopearation ();
//						return;
//					} else {
//						return;
//					}
//				};


			}


			if (isNeedtoshownextscreen)
				SubmitBidopearation ();

		}

		private string GenarateBidLineString (bool isBuddyBid)
		{
			string bidlines = string.Empty;
			//var linesToSubmit = GlobalSettings.Lines.Select (x => x.LineNum);
			List<int> linesToSubmit;
			if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null)
			{
				linesToSubmit = GlobalSettings.Lines.Where(x => !x.FAPositions.Contains("D")).Select(x => x.LineNum).ToList();
			}
			else
			{
				linesToSubmit = GlobalSettings.Lines.Select(x => x.LineNum).ToList();
			}
			BidLineList = new List<string> ();
			int seniorityCount = GlobalSettings.SubmitBid.TotalBidChoices;
			//Repeat line
			if (btnRepeatCombo.SelectedTag == 0) {
				List<string> itemstoadd = null;
				foreach (int lineNumber in linesToSubmit) {
					itemstoadd = new List<string> ();
					itemstoadd.AddRange (GetBidLinelistforPositions (lineNumber, isBuddyBid));

					foreach (var item in itemstoadd) {
						BidLineList.Add (item);
						seniorityCount--;
						if (!GlobalSettings.SubmitBid.IsSubmitAllChoices && seniorityCount == 0) {
							break;
						}
					}
					if (!GlobalSettings.SubmitBid.IsSubmitAllChoices && seniorityCount == 0) {
						break;
					}
				}
				// itemstoadd.ForEach(BidLineList.Add);
			}         // IsRepeatPositions
			else if (btnRepeatCombo.SelectedTag == 1) 
			{
				if (GlobalSettings.SubmitBid.IsSubmitAllChoices)
				{

					var positions = GetpositionList(isBuddyBid);
					foreach (string position in positions)
					{
						foreach (int lineNumber in linesToSubmit)
						{
							List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == lineNumber).FAPositions;
							if (fapositions.Contains(position))
							{
								if (!GlobalSettings.SubmitBid.IsSubmitAllChoices && seniorityCount == 0)
								{
									break;
								}
								seniorityCount--;
								BidLineList.Add(lineNumber + position);
							}

						}

						if (!GlobalSettings.SubmitBid.IsSubmitAllChoices && seniorityCount == 0)
						{
							break;
						}
					}
				}
				else
				{
					if (txtFirstpositionValue.StringValue == string.Empty)
						txtFirstpositionValue.StringValue = "0";
					if (txtSecondPositionValue.StringValue == string.Empty)
						txtSecondPositionValue.StringValue = "0";
					if (txtThirdPositionValue.StringValue == string.Empty)
						txtThirdPositionValue.StringValue = "0";
					if (txtFourthPositionValue.StringValue == string.Empty)
						txtFourthPositionValue.StringValue = "0";
					AddPositionLines(btnFirst.SelectedItem.Title, Convert.ToInt32(txtFirstpositionValue.StringValue));
					AddPositionLines(btnSecond.SelectedItem.Title, Convert.ToInt32(txtSecondPositionValue.StringValue));
					AddPositionLines(btnThird.SelectedItem.Title, Convert.ToInt32(txtThirdPositionValue.StringValue));
					AddPositionLines(btnFourth.SelectedItem.Title, Convert.ToInt32(txtFourthPositionValue.StringValue));
				}

			}
			//Custom
			else if (btnRepeatCombo.SelectedTag == 2) {

			}
			bidlines = string.Join (",", BidLineList.Select (x => x.ToString ()));
			return bidlines;
		}
		private void AddPositionLines(string position, int positionCount)
		{
			List<Line> lines = new List<Line>();
			if (GlobalSettings.SubmitBid.Buddy1 != null || GlobalSettings.SubmitBid.Buddy2 != null)
			{
				lines = GlobalSettings.Lines.Where(x => !x.FAPositions.Contains("D")).ToList();
			}
			else
			{
				lines = GlobalSettings.Lines.ToList();
			}
			foreach (var item in lines)
			{
				if (item.FAPositions.Contains(position))
				{
					positionCount--;
					if (positionCount < 0)
						break;
					BidLineList.Add(item.LineNum + position);

				}



			}
		}

		private List<string> GetBidLinelistforPositions (int line, bool isBuddyBid)
		{

			List<string> fapositions = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == line).FAPositions;

			List<string> itemstoadd = new List<string> ();
			if (fapositions != null) {
				if (isBuddyBid) {
					if (_buddyposition1 != string.Empty && fapositions.Contains (btnFirst.SelectedItem.Title))
						itemstoadd.Add (line + _buddyposition1);
					if (_buddyposition2 != string.Empty && fapositions.Contains (btnSecond.SelectedItem.Title))
						itemstoadd.Add (line + _buddyposition2);
					if (_buddyposition3 != string.Empty && fapositions.Contains (btnThird.SelectedItem.Title))
						itemstoadd.Add (line + _buddyposition3);

				} else {
					if (btnFirst.SelectedItem.Title != "[None]" && fapositions.Contains (btnFirst.SelectedItem.Title))
						itemstoadd.Add (line + btnFirst.SelectedItem.Title);
					if (btnSecond.SelectedItem.Title != "[None]" && fapositions.Contains (btnSecond.SelectedItem.Title))
						itemstoadd.Add (line + btnSecond.SelectedItem.Title);
					if (btnThird.SelectedItem.Title != "[None]" && fapositions.Contains (btnThird.SelectedItem.Title))
						itemstoadd.Add (line + btnThird.SelectedItem.Title);
					if (btnFourth.SelectedItem.Title != "[None]" && fapositions.Contains (btnFourth.SelectedItem.Title))
						itemstoadd.Add (line + btnFourth.SelectedItem.Title);
				}
			} else {
				//needs to remove
				//MessageBox.Show("FA Position is Null.Please Reparse :Temporary Message");
			}
			return itemstoadd;

		}

		private string GenarateBidLineStringForBuddyBidder (string postion)
		{
			_buddyposition1 = string.Empty;
			_buddyposition2 = string.Empty;
			_buddyposition3 = string.Empty;
			char[] positionchar = postion.ToCharArray ();
			if (positionchar.Length == 3) {
				_buddyposition1 = positionchar [0].ToString ();
				_buddyposition2 = positionchar [1].ToString ();
				_buddyposition3 = positionchar [2].ToString ();
			} else if (positionchar.Length == 2) {
				_buddyposition1 = positionchar [0].ToString ();
				_buddyposition2 = positionchar [1].ToString ();
			} else {
				_buddyposition1 = positionchar [0].ToString ();
			}
			string bidline = GenarateBidLineString (true);
			return bidline;
		}

		private void SetAvailalbeLines ()
		{
			List<string> itemstoadd = new List<string> ();
			availableLinesWithPos = new List<string> ();
			selectedLinesWithPos = new List<string> ();
			List<string> postionlist = GetpositionList (false);
			foreach (var line in Lines) {
				foreach (string position in postionlist) {
					if (line.FAPositions.Contains (position)) {
						itemstoadd.Add (line.LineNum + position);
					}
				}
			}
			itemstoadd.ForEach (availableLinesWithPos.Add);
			AvailalbelistOrder = new ObservableCollection<string> ();
			itemstoadd.ForEach (AvailalbelistOrder.Add);

		}

		private List<string> GetpositionList (bool isBuddyBid)
		{
			List<string> positions = new List<string> ();
			if (isBuddyBid) {
				if (_buddyposition1 != string.Empty)
					positions.Add (_buddyposition1);
				if (_buddyposition2 != string.Empty)
					positions.Add (_buddyposition2);
				if (_buddyposition3 != string.Empty)
					positions.Add (_buddyposition3);
			} else {
				if (btnFirst.SelectedItem.Title != "[None]")
					positions.Add (btnFirst.SelectedItem.Title);
				if (btnSecond.SelectedItem.Title != "[None]")
					positions.Add (btnSecond.SelectedItem.Title);
				if (btnThird.SelectedItem.Title != "[None]")
					positions.Add (btnThird.SelectedItem.Title);
				if (btnFourth.SelectedItem.Title != "[None]")
					positions.Add (btnFourth.SelectedItem.Title);
			}
			return positions;

		}

		void LoadBuddyDetails ()
		{
			//lblBuddy1.StringValue = (GlobalSettings.SubmitBid.Buddy1 == null || GlobalSettings.SubmitBid.Buddy1 == "0") ? string.Empty : GlobalSettings.SubmitBid.Buddy1;
			//lblBuddy2.StringValue = (GlobalSettings.SubmitBid.Buddy2 == null || GlobalSettings.SubmitBid.Buddy2 == "0") ? string.Empty : GlobalSettings.SubmitBid.Buddy2;
			lblBuddy1.StringValue = (GlobalSettings.WBidINIContent.BuddyBids.Buddy1 == "0") ? string.Empty : GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
			lblBuddy2.StringValue = (GlobalSettings.WBidINIContent.BuddyBids.Buddy2 == "0") ? string.Empty : GlobalSettings.WBidINIContent.BuddyBids.Buddy2;

			if (lblBuddy1.StringValue == string.Empty)
				btnBuddy1.Enabled = false;
			else
				btnBuddy1.Enabled = true;
			if (lblBuddy2.StringValue == string.Empty)
				btnBuddy2.Enabled = false;
			else
				btnBuddy2.Enabled = true;
		}

		void btnClearClicked (object sender, EventArgs e)
		{
			SetAvailalbeLines ();
			//selectedLinesWithPos.Clear ();
			ReloadLists ();
		}

		void btnRemoveClicked (object sender, EventArgs e)
		{
//			foreach (var index in rightSelection) {
//				availableLinesWithPos.Add (selectedLinesWithPos [index]);
//			}
//			foreach (var index in rightSelection) {
//				selectedLinesWithPos.RemoveAt (rightSelection.First ());
//			}

			try {
				SelectedBidLineList = new List<string> ();
				foreach (var index in rightSelection) {
					SelectedBidLineList.Add (selectedLinesWithPos [index]);
				}
				
				List<string> collection = SelectedBidLineList;
				collection.Reverse ();
				if (collection.Count > 0) {
					foreach (string bidline in collection) {
						int index = selectedLinesWithPos.IndexOf (bidline);
						selectedLinesWithPos.Remove (bidline);
				
						AddBidLineToAvaialableLines (bidline);
						//set the list box  selection
						if (index > 0)
							index = index - 1;
						else
						{
							ReloadLists ();
							return;
						}
						//SelectedBidLines = BidLineList[index];
				
					}
				}
				
				ReloadLists ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void AddBidLineToAvaialableLines (string column)
		{
			//get the line number
			if (!availableLinesWithPos.Contains (column)) {
				//insert into the avaialbe line list
				availableLinesWithPos.Add (column);
				availableLinesWithPos = new List<string> (availableLinesWithPos.OrderBy (x => AvailalbelistOrder.IndexOf (x)).ToList ());
				availableLinesWithPos = new List<string> (availableLinesWithPos);
			}
		}

		void btnAddClicked (object sender, EventArgs e)
		{
			try {
				foreach (var index in leftSelection) {
					selectedLinesWithPos.Add (availableLinesWithPos [index]);
				}
				foreach (var index in leftSelection) {
					availableLinesWithPos.RemoveAt (leftSelection.First ());
				}
				ReloadLists ();
				tblSelectedLine.ScrollRowToVisible (tblSelectedLine.RowCount - 1);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void btnInsertClicked (object sender, EventArgs e)
		{
			try {
				int rightIndex = rightSelection.First ();
				foreach (var index in leftSelection) {
					selectedLinesWithPos.Insert (rightIndex, availableLinesWithPos [index]);
					rightIndex++;
				}
				foreach (var index in leftSelection) {
					availableLinesWithPos.RemoveAt (leftSelection.First ());
				}
				ReloadLists ();
				tblSelectedLine.ScrollRowToVisible (rightIndex);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void ReloadLists ()
		{
			tblAvailableLine.ReloadData ();
			tblSelectedLine.ReloadData ();
			tblAvailableLine.DeselectAll (this);
			tblSelectedLine.DeselectAll (this);
			leftSelection.Clear ();
			rightSelection.Clear ();
			EnableDisableButtons ();
			lblAvailable.StringValue = availableLinesWithPos.Count + " Lines";
			lblBids.StringValue = selectedLinesWithPos.Count + " Bids";
		}

		public void EnableDisableButtons ()
		{
			if (leftSelection.Count != 0)
				btnAdd.Enabled = true;
			else
				btnAdd.Enabled = false;

			if (rightSelection.Count == 1 && leftSelection.Count != 0)
				btnInsert.Enabled = true;
			else
				btnInsert.Enabled = false;

			if (rightSelection.Count != 0)
				btnRemove.Enabled = true;
			else
				btnRemove.Enabled = false;

			if (selectedLinesWithPos.Count != 0)
				btnClear.Enabled = true;
			else
				btnClear.Enabled = false;
		}

		void BindPositions ()
		{
//			btnFirst.RemoveAllItems ();
//			btnFirst.AddItems (availablePositions);
			var secList = availablePositions.ToList ();
			secList.Remove (btnFirst.SelectedItem.Title);
			btnSecond.RemoveAllItems ();
			btnSecond.AddItems (secList.ToArray ());
			var thrdList = secList;
			thrdList.Remove (btnSecond.SelectedItem.Title);
			btnThird.RemoveAllItems ();
			btnThird.AddItems (thrdList.ToArray ());
			var frthList = thrdList;
			frthList.Remove (btnThird.SelectedItem.Title);
			btnFourth.RemoveAllItems ();
			btnFourth.AddItems (frthList.ToArray ());
			btnFourth.SelectItem ("None");
		}

		private string first, second, third, fourth;
		void HandlePositionChange (object sender, EventArgs e)
		{
			first = btnFirst.SelectedItem.Title;
			second = btnSecond.SelectedItem.Title;
			third = btnThird.SelectedItem.Title;
			fourth = btnFourth.SelectedItem.Title;

			NSPopUpButton btn = (NSPopUpButton)sender;

			if (btn == btnThird)
			{
				if (second != @"None") 
				{
					UpdatePositions (btn);
				} 
				else 
				{
					btnThird.SelectItem (@"None");
					WarningAlertMessage ("You cannot set the 3rd or 4th position while the 2nd position is “NONE”.  Change the 2nd position first");
				}

			}
			else if (btn == btnFourth)
			{
				if (third != @"None") 
				{
					UpdatePositions (btn);
				} 
				else 
					{
					btnFourth.SelectItem (@"None");
					WarningAlertMessage ("You cannot set the 4th position while the 3rd position is “NONE”.  Change the 3rd position first");
					}
			}
			else
				UpdatePositions (btn);

			SetRepeatPositionUI();
		}
		void WarningAlertMessage(string msg)
			{
				var alert = new NSAlert();
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Warning";
				alert.InformativeText = msg;

				alert.RunModal();
		}
		void UpdatePositions (NSPopUpButton btn)
		{
			try {
				var BtnTitle = btn.SelectedItem.Title;

				if (btn == btnFirst) {
					if (BtnTitle != "None") {
						BindPositions ();
					} else {
						btnSecond.RemoveAllItems ();
						btnSecond.AddItems (availablePositions);
						var thrdList = availablePositions.ToList ();
						thrdList.Remove (btnSecond.SelectedItem.Title);
						btnThird.RemoveAllItems ();
						btnThird.AddItems (thrdList.ToArray ());
						var frthList = thrdList;
						frthList.Remove (btnThird.SelectedItem.Title);
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (frthList.ToArray ());
					}

					if (btnSecond.ItemTitles ().Contains (second))
						btnSecond.SelectItem (second);
					else
						btnSecond.SelectItem ("None");
					
					UpdatePositions (btnSecond);
				
				} else if (btn == btnSecond) {
					if (BtnTitle != "None") {
						var thrdList = btnSecond.ItemTitles ().ToList ();
						thrdList.Remove (btnSecond.SelectedItem.Title);
						btnThird.RemoveAllItems ();
						btnThird.AddItems (thrdList.ToArray ());
						var frthList = thrdList;
						frthList.Remove (btnThird.SelectedItem.Title);
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (frthList.ToArray ());
					} else if (btnFirst.SelectedItem.Title != "None") {
						var thrdList = btnFirst.ItemTitles ().ToList ();
						thrdList.Add("None");
						thrdList.Remove (btnFirst.SelectedItem.Title);
						btnThird.RemoveAllItems ();
						btnThird.AddItems (thrdList.ToArray ());
						var frthList = thrdList;
						frthList.Remove (btnThird.SelectedItem.Title);
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (frthList.ToArray ());
					} else {
						btnThird.RemoveAllItems ();
						btnThird.AddItems (availablePositions);
						var frthList = btnThird.ItemTitles ().ToList ();
						frthList.Remove (btnThird.SelectedItem.Title);
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (frthList.ToArray ());
					}

					if(btnSecond.SelectedItem.Title == "None")
						btnThird.SelectItem ("None");
					else if (btnThird.ItemTitles ().Contains (third))
						btnThird.SelectItem (third);
					else
						btnThird.SelectItem ("None");
					
					UpdatePositions (btnThird);
				
				} else if (btn == btnThird) {
					if (BtnTitle != "None") {
						var frthList = btnThird.ItemTitles ().ToList ();
						frthList.Remove (btnThird.SelectedItem.Title);
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (frthList.ToArray ());
					} else if (btnSecond.SelectedItem.Title != "None") {
						var frthList = btnSecond.ItemTitles ().ToList ();
						frthList.Remove (btnSecond.SelectedItem.Title);
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (frthList.ToArray ());
					} else if (btnFirst.SelectedItem.Title != "None") {
						var frthList = btnFirst.ItemTitles ().ToList ();
						frthList.Add("None");
						frthList.Remove (btnFirst.SelectedItem.Title);
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (frthList.ToArray ());
					} else {
						btnFourth.RemoveAllItems ();
						btnFourth.AddItems (availablePositions);
					}

					if(btnThird.SelectedItem.Title == "None")
						btnFourth.SelectItem ("None");
					else if (btnFourth.ItemTitles ().Contains (fourth))
						btnFourth.SelectItem (fourth);
					else
						btnFourth.SelectItem ("None");
				
				} else if (btn == btnFourth) {
				
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
	}

	public partial class AvailableTableSource : NSTableViewSource
	{
		FAPositionChoiceWindowController parentWC;

		public AvailableTableSource (FAPositionChoiceWindowController parent)
		{
			parentWC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentWC.availableLinesWithPos.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			//var lineDisp = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == parentWC.availableLines [row]).LineDisplay;
			return new NSString (parentWC.availableLinesWithPos [(int)row]);		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				parentWC.leftSelection = table.SelectedRows.ToList ().ConvertAll (x => (int)x);
			} else
				parentWC.leftSelection.Clear ();
			parentWC.EnableDisableButtons ();
		}
	}

	public partial class SelectedTableSource : NSTableViewSource
	{
		FAPositionChoiceWindowController parentWC;

		public SelectedTableSource (FAPositionChoiceWindowController parent)
		{
			parentWC = parent;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return parentWC.selectedLinesWithPos.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			//var lineDisp = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == parentWC.selectedLines [row]).LineDisplay;
			return new NSString (parentWC.selectedLinesWithPos [(int)row]);		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				parentWC.rightSelection = table.SelectedRows.ToList ().ConvertAll (x => (int)x);
			} else
				parentWC.rightSelection.Clear ();
			parentWC.EnableDisableButtons ();
		}

	}

}

