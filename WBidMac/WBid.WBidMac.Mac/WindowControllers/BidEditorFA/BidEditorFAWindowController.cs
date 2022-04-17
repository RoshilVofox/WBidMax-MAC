
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;

//using System;
using CoreGraphics;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using WBid.WBidiPad.Core;
//using MonoTouch.CoreGraphics;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.iOS;
//using WBid.WBidiPad.Model;
//using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
//using System.Linq;
using WBid.WBidiPad.iOS.Utility;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using WBid.WBidiPad.SharedLibrary.SWA;
//using iOSPasswordStorage;
using WBidDataDownloadAuthorizationService.Model;
using System.ServiceModel;
using System.Threading.Tasks;
using System.IO;

namespace WBid.WBidMac.Mac
{
	public partial class BidEditorFAWindowController : AppKit.NSWindowController
	{
		#region Constructors
		bool ChangeBuddy = false;
		string[] availablePositions = {"A","B","C","D","None"};
		public string[] availablePositionSelections = { "ABC", "ACB", "BAC", "BCA", "CAB", "CBA", "AB", "AC", "BA", "BC", "CA", "CB", "A", "B", "C" };
		public List<int>availableLines = new List<int>();//GlobalSettings.Lines.Select(x=>x.LineNum).ToList();
		public List<int>selectedLines = new List<int>();
		public List<int>leftSelection = new List<int>();
		public List<int>rightSelection = new List<int>();

		//holds the selected availalble line items
		public List<int> selectedFromAvailableList;
		//holds all the items in the avaialble line list
		public List<int> AvailableLineList;

		//holds the all items in the bid Line list
		public List<string> BidLineList;
		//holds the selected bid line list.
		public List<string> SelectedBidLineList;



		// Called when created from unmanaged code
		public BidEditorFAWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidEditorFAWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BidEditorFAWindowController () : base ("BidEditorFAWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new BidEditorFAWindow Window {
			get {
				return (BidEditorFAWindow)base.Window;
			}
		}

		static NSButton closeButton;
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				
				this.Window.Title = "Bid Editor for FA";
				
				if (GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine) {
					AvailableLineList = GlobalSettings.Lines.Select (x => x.LineNum).ToList ();
				} else {
					AvailableLineList = GlobalSettings.Lines.Select (x => x.LineNum).OrderBy (x => x).ToList ();
				}
				
				string befFilename = WBidHelper.GetAppDataPath () + "/" + GetBefFileName () + ".BEF";
				selectedFromAvailableList = new List<int> ();
				SelectedBidLineList = new List<string> ();
				if (File.Exists (befFilename)) {
					List<string> bidlinelist = (List<string>)WBidHelper.DeSerializeObject (befFilename);
				
					var listPositions = bidlinelist [0].Split (',').ToList ();
					bidlinelist.RemoveAt (0);
					BidLineList = new List<string> (bidlinelist);
					listPositions = listPositions.Where (x => !(x == "[None]")).ToList ();
					AvailableLineList = new List<int> (AvailableLineList.Where (x => WheatherPositionShouldBeRemoved (x, BidLineList.ToList (), listPositions)));
					//				setupViews();
					//				btnFirstChoice.SetTitle(listPositions[0], UIControlState.Normal);
					//				btnSecondChoice.SetTitle(listPositions[1], UIControlState.Normal);
					//				btnThirdChoice.SetTitle(listPositions[2], UIControlState.Normal);
					//				btnFourthChoice.SetTitle(listPositions[3], UIControlState.Normal);
				} else {
				
					BidLineList = new List<string> ();
					//				setupViews();
				}
				
				BindPositions ();
				
				btnFirst.Activated += HandlePositionChange;
				btnSecond.Activated += HandlePositionChange;
				btnThird.Activated += HandlePositionChange;
				btnFourth.Activated += HandlePositionChange;
				
				btnChangeEmployee.Activated += (object sender, EventArgs e) => {
					var panel = new NSPanel ();
					var changeEmp = new ChangeEmployeeViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CGSize (400, 180));
					panel.ContentView = changeEmp.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
				};
				
				btnChangeBuddy.Activated += (object sender, EventArgs e) => {
					var panel = new NSPanel ();
					var changeBud = new ChangeBuddyViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CGSize (400, 190));
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
				btnAddReserve.Activated+= (object sender, EventArgs e) => 
				{
					if ((GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0" || GlobalSettings.WBidINIContent.BuddyBids.Buddy1 != "0")&& btnAddReserve.State== NSCellStateValue.On)
					{
						btnAddReserve.State=NSCellStateValue.Off;

						WarningAlertMessage ("You cannot add Reserve to the end of your Bid when Buddy Bidding");
					}
				};
				btnBuddy1.AddItems (availablePositionSelections);
				btnBuddy2.AddItems (availablePositionSelections);
				LoadBuddyDetails ();
				
				btnCancelClear.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				btnSaveClose.Activated += (object sender, EventArgs e) => {
					SaveBidLineAndPositions();
				};
				btnSubmit.Activated += (object sender, EventArgs e) => {
				
				
					submitbid ();
				
				};
				
				tblAvailableLines.Source = new AvailableLinesTableSource (this);
				tblSelectedLines.Source = new SelectedLinesTableSource (this);
				
				btnAdd.Activated += btnAddClicked;
				btnInsert.Activated += btnInsertClicked;
				btnRemove.Activated += btnRemoveClicked;
				btnClear.Activated += btnClearClicked;
				btnRepeatA.Activated += btnRepeatAClicked;
				btnRepeatB.Activated += btnRepeatBClicked;
				btnRepeatC.Activated += btnRepeatCClicked;
				
				EnableDisableButtons ();
				lblLines.StringValue = AvailableLineList.Count + " Lines";
				lblBids.StringValue = BidLineList.Count + " Bids";
				
				txtManual.Changed += (object sender, EventArgs e) => {
					if (txtManual.StringValue.Length != 0) {
						leftSelection.Clear ();
						tblAvailableLines.DeselectAll (this);
						EnableDisableButtons2 ();
					}
				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		private void submitbid()
		{
			SubmitBid submitBid = new SubmitBid();
			try
			{

				//set the properties required to POST the webrequest to SWA server.
				submitBid.Base = GlobalSettings.BidPrepDetails.Domicile;
				submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
				submitBid.BidRound = (GlobalSettings.BidPrepDetails.BidRound == "B") ? "Round 2" : "Round 1";
				submitBid.PacketId = GenaratePacketId();
				submitBid.Seat = GlobalSettings.BidPrepDetails.Position;
				BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
				if (GlobalSettings.BidPrepDetails.IsChkAvoidanceBid || btnAddReserve.State== NSCellStateValue.On)
				{
					submitBid.Buddy1 = submitBid.Buddy2 = null;

				}
				else
				{
					submitBid.Buddy1 = (buddyBids.Buddy1 == "0") ? null : buddyBids.Buddy1;
					submitBid.Buddy2 = (buddyBids.Buddy2 == "0") ? null : buddyBids.Buddy2;
				}

				int dPositionCount = BidLineList.Where(x => x.Substring(x.Length - 1, 1) == "D").Count();
				bool isNeedtoshownextscreen = true;
				if ((submitBid.Buddy1 != null || submitBid.Buddy2 != null) && dPositionCount > 0)
				{
					isNeedtoshownextscreen = false;
					var alert = new NSAlert();
					alert.AlertStyle = NSAlertStyle.Informational;
					alert.MessageText = "WBidMax";
					alert.InformativeText = dPositionCount + " D Lines  were deleted from the bid. \nBuddy Bid Lines can only have A, B or C positions. \nPress OK to continue or CANCEL to return the position Choices. ";

					alert.AddButton("OK");
					alert.AddButton("Cancel");
					alert.Buttons[0].Activated += (object sender1, EventArgs ex) =>
					{
						alert.Window.Close();
						NSApplication.SharedApplication.StopModal();
						BidLineList = new List<string>(BidLineList.Where(x => !(x.Substring(x.Length - 1, 1) == "D")));
						tblSelectedLines.ReloadData();
						SubmitBidopearation(submitBid);

						return;
					};
					alert.Buttons[1].Activated += (object sender1, EventArgs ex) =>
					{

						alert.Window.Close();
						NSApplication.SharedApplication.StopModal();
						return;
					};
					alert.RunModal();

					//					UIAlertView alert = new UIAlertView();
					//					alert.Title = "Lines  were deleted from the bid. Buddy Bid Lines must have positions (A,B,etc) for each bidder. nPress OK to continue or CANCEL to return the position Choices. ";
					//					alert.Message = GlobalSettings.TemporaryEmployeeNumber ?? string.Empty;
					//					alert.AddButton("Cancel");
					//					alert.AddButton("Ok");
					//					alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
					//					alert.Clicked += submitBidAlertHandle;
					//					alert.Show();
				}
				if (isNeedtoshownextscreen)
					SubmitBidopearation(submitBid);





			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		private void SubmitBidopearation(SubmitBid submitBid)
		{
			//genarate bid line to submit
			
			submitBid.Bid = GenarateBidLineString();
			submitBid.TotalBidChoices = BidLineList.Count();
			if (btnAddReserve.State == NSCellStateValue.On)
			{
				submitBid.Bid += string.IsNullOrEmpty(submitBid.Bid) ? string.Empty : ",";
				submitBid.Bid += "R";
				submitBid.TotalBidChoices += 1;
			}



			submitBid.BuddyBidderBids = new List<BuddyBidderBid>();
			if (submitBid.Buddy1 != null)
			{
				submitBid.BuddyBidderBids.Add(new BuddyBidderBid() { BuddyBidder = submitBid.Buddy1, BidLines = GenarateBuddyBidBidLineString(btnBuddy1.SelectedItem.Title) });
			}

			if (submitBid.Buddy2 != null)
			{
				submitBid.BuddyBidderBids.Add(new BuddyBidderBid() { BuddyBidder = submitBid.Buddy2, BidLines = GenarateBuddyBidBidLineString(btnBuddy2.SelectedItem.Title) });
			}
			GlobalSettings.SubmitBid = submitBid;


			var query = new QueryWindowController();
			this.Window.AddChildWindow(query.Window, NSWindowOrderingMode.Above);
			NSApplication.SharedApplication.RunModalForWindow(query.Window);
		}
		/// </summary>
		/// <param name="bidDetails"></param>
		/// <returns></returns>
		private string GenaratePacketId()
		{
			string packetid = string.Empty;
			packetid = GlobalSettings.BidPrepDetails.Domicile + GlobalSettings.BidPrepDetails.BidYear + GlobalSettings.BidPrepDetails.BidPeriod.ToString("d2");

			//Set-round-numbers:
			//1 - F/A monthly bids
			//2 - F/A supplemental bids
			//3 - reserved
			//4 - Pilot monthly bids
			//5 - Pilot supplemental bids

			//D first Round  B second Round
			if (GlobalSettings.BidPrepDetails.Position == "FA")
			{
				packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "1" : "2";
			}
			else if (GlobalSettings.BidPrepDetails.Position == "CP" || GlobalSettings.BidPrepDetails.Position == "FO")
			{
				packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "4" : "5";
			}
			else
			{
				packetid = "3";
			}


			return packetid;
		}
		/// <summary>
		/// PURPOSE : Generate Bid lines
		/// </summary>
		/// <returns></returns>
		private string GenarateBidLineString()
		{
			string bidLines = string.Empty;
			bidLines = string.Join(",", BidLineList.Select(x => x.ToString()));
			return bidLines;
		}

		private string GenarateBuddyBidBidLineString(string SelectedBuddyBidder1Postion)
		{
			string bidOrder = string.Empty;

			List<string> tempList = BidLineList.Select(x => x.Substring(0, x.Length - 1)).Distinct().ToList();


			foreach (var line in tempList)
			{
				foreach (char item in SelectedBuddyBidder1Postion)
				{
					if (GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == Convert.ToInt32(line)).FAPositions.Contains(item.ToString()))
					{
						if (bidOrder != string.Empty)
						{
							bidOrder += ",";
						}

						bidOrder += line + item;
					}
				}

			}
			return bidOrder;
		}

		void btnRepeatCClicked (object sender, EventArgs e)
		{
			try {
				SelectedBidLineList = new List<string> ();
				foreach (var index in rightSelection) {
					SelectedBidLineList.Add (BidLineList [index]);
				}
				RepeatBidLine (SelectedBidLineList, "C");
				ReloadLists ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void btnRepeatBClicked (object sender, EventArgs e)
		{
			try {
				SelectedBidLineList = new List<string> ();
				foreach (var index in rightSelection) {
					SelectedBidLineList.Add (BidLineList [index]);
				}
				RepeatBidLine (SelectedBidLineList, "B");
				ReloadLists ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void btnRepeatAClicked (object sender, EventArgs e)
		{
			try {
				SelectedBidLineList = new List<string> ();
				foreach (var index in rightSelection) {
					SelectedBidLineList.Add (BidLineList [index]);
				}
				RepeatBidLine (SelectedBidLineList, "A");
				ReloadLists ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		/// <summary>
		/// save the selected bid line and current positions to the BeF file
		/// </summary>
		private void SaveBidLineAndPositions()
		{
			string filename = WBidHelper.GetAppDataPath() + "/" + GetBefFileName() + ".BEF";
			BidLineList.Insert(0, btnFirst.SelectedItem.Title + "," + btnSecond.SelectedItem.Title + "," + btnThird.SelectedItem.Title + "," + btnFourth.SelectedItem.Title);
			WBidHelper.SerializeObject(filename, BidLineList);
		}
		/// <summary>
		/// Get the filename for the ".BEF" file.
		/// </summary>
		/// <returns></returns>
		private string GetBefFileName()
		{
			string filename = "F";
			filename += (GlobalSettings.CurrentBidDetails.Round == "M") ? "D" : "B";
			filename += GlobalSettings.CurrentBidDetails.Domicile;
			filename += GlobalSettings.CurrentBidDetails.Month.ToString("X");
			return filename;
		}

		private static bool WheatherPositionShouldBeRemoved(int position, List<string> listString, List<string> listPositions)
		{
			var listPositionsIn = listString.Where(y => y.StartsWith(position.ToString()));

			var positionedListPositions = listPositions.Select(x => position.ToString() + x);

			return !positionedListPositions.All(x => listPositionsIn.Contains(x));
		}

		void btnClearClicked (object sender, EventArgs e)
		{
			try {
				if (GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine) {
					AvailableLineList = GlobalSettings.Lines.Select (x => x.LineNum).ToList ();
				} else {
					AvailableLineList = GlobalSettings.Lines.Select (x => x.LineNum).OrderBy (x => x).ToList ();
				}
				BidLineList.Clear ();
				ReloadLists ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void btnRemoveClicked (object sender, EventArgs e)
		{
//			foreach (var index in rightSelection) {
//				availableLines.Add (selectedLines [index]);
//			}
//			foreach (var index in rightSelection) {
//				selectedLines.RemoveAt (rightSelection.First ());
//			}
			try {
				SelectedBidLineList = new List<string> ();
				foreach (var index in rightSelection) {
					SelectedBidLineList.Add (BidLineList [index]);
				}
				
				List<string> collection = SelectedBidLineList;
				collection.Reverse ();
				if (collection.Count > 0) {
					foreach (string bidline in collection) {
						int index = BidLineList.IndexOf (bidline);
						BidLineList.Remove (bidline);
				
						AddBidLineToAvaialableLines (bidline);
						// IsEnableSubmitButton = (BidLineList.Count > 0) ? true : false;
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

		private void AddBidLineToAvaialableLines(string column)
		{
			//get the line number
			var linenumber = int.Parse(string.Join(null, Regex.Split(column, "[^\\d]")));
			if (!AvailableLineList.Contains(linenumber))
			{
				//insert into the avaialbe line list
				AvailableLineList.Add(linenumber);
				if (!GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine)
				{
					AvailableLineList = new List<int>(AvailableLineList.OrderBy(x => x));
				}
				else
				{
					var orderedlines = AvailableLineList.ToList().OrderBy(x => GlobalSettings.Lines.Select(y => y.LineNum).ToList().IndexOf(x));
					AvailableLineList = new List<int>(orderedlines);
				}
			}
		}

		void btnAddClicked (object sender, EventArgs e)
		{
//			foreach (var index in leftSelection) {
//				selectedLines.Add (availableLines [index]);
//			}
//			foreach (var index in leftSelection) {
//				availableLines.RemoveAt (leftSelection.First ());
//			}
			try {
				if (txtManual.StringValue.Length != 0) {
					var bidlinelist = GetBidLinelistforbidChoice (txtManual.StringValue);
					bidlinelist.ForEach (BidLineList.Add);
					// SelectedBidLines = bidlinelist.LastOrDefault();
					var objline = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == txtManual.IntValue);
					if (objline != null) {
						List<string> fapositions = objline.FAPositions;
						if (fapositions.All (x => BidLineList.Contains (string.Format ("{0}{1}", txtManual.IntValue, x))))
							AvailableLineList.Remove (txtManual.IntValue);
					}
					txtManual.StringValue = string.Empty;
				} else {
					selectedFromAvailableList = new List<int> ();
					foreach (var index in leftSelection) {
						selectedFromAvailableList.Add (AvailableLineList [index]);
					}
					List<string> itemstoadd = null;
					foreach (int line in selectedFromAvailableList) {
						itemstoadd = new List<string> ();
						var lineitems = GetBidLinelistforPositions (line);
						if (lineitems.Count > 0) {
							itemstoadd.AddRange (GetBidLinelistforPositions (line));
							itemstoadd.ForEach (BidLineList.Add);
							List<string> fapositions = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == line).FAPositions;
				
							if (fapositions.All (x => BidLineList.Contains (string.Format ("{0}{1}", line, x))))
								AvailableLineList.Remove (line);
						}
					}
				}
				ReloadLists ();
				tblSelectedLines.ScrollRowToVisible (tblSelectedLines.RowCount - 1);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		/// <summary>
		/// Get the bid lines if the user enters the data in to the Bid line text box(Appended the positions)
		/// </summary>
		/// <param name="bidChoice"></param>
		/// <returns></returns>
		private List<string> GetBidLinelistforbidChoice(string bidChoice)
		{
			List<string> bidline = new List<string>();

			string pattern = "^\\d{1,3}[A-Da-d]{0,4}$";
			if (Regex.Match(bidChoice, pattern).Success)
			{
				string numberInTheFileName = string.Join(null, Regex.Split(bidChoice, "[^\\d]"));
				int linenumber = Int32.Parse(numberInTheFileName);
				if (linenumber >= AvailableLineList[0] && linenumber <= AvailableLineList[AvailableLineList.Count - 1])
				{
					List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber).FAPositions;
					string[] positions = bidChoice.Split(new string[] { linenumber.ToString() }, StringSplitOptions.RemoveEmptyEntries);
					//check any positions entered in the Bid choice
					if (positions.Count() > 0)
					{
						List<string> manualpositonedlines = new List<string>();
						foreach (var position in positions[0])
						{
							string item = linenumber.ToString() + position.ToString().ToUpper();
							if (!BidLineList.Contains(item) && fapositions.Contains(position.ToString().ToUpper()))
							{
								bidline.Add(item);
							}
						}
					}
					else
					{
						bidline = GetBidLinelistforPositions(linenumber);
					}
				}


			}
			else
			{
				//  MessageBox.Show("Invalid Bid Choice", "Wrong Bid Choice", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
			}
			return bidline;
		}

		private List<string> GetBidLinelistforPositions(int line)
		{
			List<string> fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == line).FAPositions;

			List<string> itemstoadd = new List<string>();
			if (fapositions != null)
			{
				if (btnFirst.SelectedItem.Title != "[None]" && !BidLineList.Contains(line + btnFirst.SelectedItem.Title) && fapositions.Contains(btnFirst.SelectedItem.Title))
					itemstoadd.Add(line + btnFirst.SelectedItem.Title);
				if (btnSecond.SelectedItem.Title != "[None]" && !BidLineList.Contains(line + btnSecond.SelectedItem.Title) && fapositions.Contains(btnSecond.SelectedItem.Title))
					itemstoadd.Add(line + btnSecond.SelectedItem.Title);
				if (btnThird.SelectedItem.Title != "[None]" && !BidLineList.Contains(line + btnThird.SelectedItem.Title) && fapositions.Contains(btnThird.SelectedItem.Title))
					itemstoadd.Add(line + btnThird.SelectedItem.Title);
				if (btnFourth.SelectedItem.Title != "[None]" && !BidLineList.Contains(line + btnFourth.SelectedItem.Title) && fapositions.Contains(btnFourth.SelectedItem.Title))
					itemstoadd.Add(line + btnFourth.SelectedItem.Title);
			}
			return itemstoadd;

		}

		void btnInsertClicked (object sender, EventArgs e)
		{
			try {
				int rightIndex = rightSelection.First ();
				//			foreach (var index in leftSelection) {
				//				selectedLines.Insert (rightIndex, availableLines [index]);
				//				rightIndex++;
				//			}
				//			foreach (var index in leftSelection) {
				//				availableLines.RemoveAt (leftSelection.First ());
				//			}
				
				int index = rightSelection.First ();//BidLineList.IndexOf(SelectedBidLines);
				index = (index < 0) ? 0 : index;
				
				List<string> bidlines = new List<string> ();
				if (txtManual.StringValue.Length != 0) {
					bidlines = GetBidLinelistforbidChoice (txtManual.StringValue);
					txtManual.StringValue = string.Empty;
				
				} else {
					selectedFromAvailableList = new List<int> ();
					foreach (var inde in leftSelection) {
						selectedFromAvailableList.Add (AvailableLineList [inde]);
					}
					foreach (int line in selectedFromAvailableList) {
						var lineitems = GetBidLinelistforPositions (line);
						if (lineitems.Count > 0) {
							bidlines.AddRange (lineitems);
				
						}
					}
				}
				BidLineList.InsertRange (index, bidlines);
				//BidLineList.InsertRange(bidlines, index);
				//SelectedBidLines = bidlines.LastOrDefault();
				
				foreach (var line in selectedFromAvailableList) {
					List<string> fapositions = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == line).FAPositions;
					if (fapositions.All (x => BidLineList.Contains (string.Format ("{0}{1}", line, x))))
						AvailableLineList.Remove (line);
				
				}
				
				ReloadLists ();
				tblSelectedLines.ScrollRowToVisible (rightIndex);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		private void RepeatBidLine(List<string> choices, string position)
		{
			if (choices != null)
			{
				foreach (var selectedBid in choices)
				{
					int linenumber = Convert.ToInt32(string.Join(null, Regex.Split(selectedBid, "[^\\d]")));
					var fapositions = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == linenumber).FAPositions;
					if (!BidLineList.Contains(linenumber + position) && fapositions.Contains(position))
					{
						BidLineList.Add(linenumber + position);
						if (fapositions.All(x => BidLineList.Contains(string.Format("{0}{1}", linenumber, x))))
							AvailableLineList.Remove(linenumber);
					}
				}
			}
		}

		void ReloadLists ()
		{
			tblAvailableLines.ReloadData ();
			tblSelectedLines.ReloadData ();
			tblAvailableLines.DeselectAll (this);
			tblSelectedLines.DeselectAll (this);
			leftSelection.Clear ();
			rightSelection.Clear ();
			EnableDisableButtons ();
			lblLines.StringValue = AvailableLineList.Count + " Lines";
			lblBids.StringValue = BidLineList.Count + " Bids";
		}

		void EnableDisableButtons2 ()
		{
			if (AvailableLineList.Contains (txtManual.IntValue))
				btnAdd.Enabled = true;
			else
				btnAdd.Enabled = false;
			if (AvailableLineList.Contains (txtManual.IntValue) && rightSelection.Count == 1)
				btnInsert.Enabled = true;
			else
				btnInsert.Enabled = false;

			if (rightSelection.Count != 0)
				btnRemove.Enabled = true;
			else
				btnRemove.Enabled = false;

			if (BidLineList.Count != 0)
				btnClear.Enabled = true;
			else
				btnClear.Enabled = false;

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

			if (BidLineList.Count != 0)
				btnClear.Enabled = true;
			else
				btnClear.Enabled = false;

			if (leftSelection.Count != 0)
				txtManual.StringValue = string.Empty;

			if (txtManual.StringValue.Length != 0)
				EnableDisableButtons2 ();

		}

		void LoadBuddyDetails ()
		{
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
			try {
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
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
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

	public partial class AvailableLinesTableSource : NSTableViewSource 
	{
		BidEditorFAWindowController parentWC;
		public AvailableLinesTableSource (BidEditorFAWindowController parent)
		{
			parentWC = parent;
		}
		public override nint GetRowCount (NSTableView tableView)
		{
			return parentWC.AvailableLineList.Count;
		}
		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var lineDisp = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == parentWC.AvailableLineList [(int)row]).LineDisplay;
			return new NSString (lineDisp);		
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

	public partial class SelectedLinesTableSource : NSTableViewSource 
	{
		BidEditorFAWindowController parentWC;
		public SelectedLinesTableSource (BidEditorFAWindowController parent)
		{
			parentWC = parent;
		}
		public override nint GetRowCount (NSTableView tableView)
		{
			return parentWC.BidLineList.Count;
		}
		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			//var lineDisp = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == parentWC.selectedLines [row]).LineDisplay;
//			if(row>=parentWC.BidLineList.Count)
//				return new NSString (parentWC.BidLineList [(int)row-1]);
			return new NSString (parentWC.BidLineList [(int)row]);		
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

