
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

namespace WBid.WBidMac.Mac
{
	public partial class BidEditorPilotWindowController : AppKit.NSWindowController
	{
		#region Constructors
		public List <string> selectedLines = new List<string> ();
		//public NSObject[] availableLines;
		public List <int> rightSelection = new List<int>();
		public List<ItemClass> Available = new List<ItemClass>();

		NSObject notif;

		// Called when created from unmanaged code
		public BidEditorPilotWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidEditorPilotWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BidEditorPilotWindowController () : base ("BidEditorPilotWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new BidEditorPilotWindow Window {
			get {
				return (BidEditorPilotWindow)base.Window;
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
					NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				if (GlobalSettings.BidPrepDetails.Position == "CP") {
					btnChangeAvoidance.Enabled = false;
				} else {
					btnChangeAvoidance.Enabled = true;
				} 
				if (GlobalSettings.BidPrepDetails.Position == "FA" && GlobalSettings.CurrentBidDetails.Round=="S") {
					btnChangeAvoidance.Enabled = false;
					btnChangeAvoidance.Title = "Change Buddy Bid";
				}
				btnCancel.Activated += (object sender, EventArgs e) => {
					NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				btnChangeEmployee.Activated += (object sender, EventArgs e) => {
					var panel = new NSPanel ();
					var changeEmp = new ChangeEmployeeViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CGSize (400, 180));
					panel.ContentView = changeEmp.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
				};
				btnChangeAvoidance.Activated += (object sender, EventArgs e) => {
					var panel = new NSPanel ();
					var changeAvoid = new ChangeAvoidanceViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CGSize (400, 270));
					panel.ContentView = changeAvoid.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
				};
				GenerateLineList ();
				var lineList = GlobalSettings.Lines.OrderBy (x => x.LineNum).ToList ();
				List<string> ss = lineList.Select (x => x.LineNum.ToString ()).ToList ();
				//availableLines = ss.Select (x => new NSString (x)).ToArray();
				cwAvailableLines.ItemPrototype = new MyCollectionViewItem ();
				//cwAvailableLines.Content = availableLines;
				
				foreach (var item in ss) {
					var newItem = new ItemClass ();
					newItem.Title = item;
					if (selectedLines.Contains (item))
						newItem.Selected = true;
					else
						newItem.Selected = false;
					Available.Add (newItem);
				}
				cwAvailableLines.Content = Available.ToArray ();
				
				notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"EditorUpdate", (NSNotification n) => {
					var item = n.Object.ToString ();
					if (!selectedLines.Contains (item)) {
						selectedLines.Add (item);
						Available.Find (x => x.Title == item).Selected = true;
					} else {
						selectedLines.Remove (item);
						Available.Find (x => x.Title == item).Selected = false;
					}
					tblSelectedLines.ReloadData ();
					tblSelectedLines.ScrollRowToVisible (tblSelectedLines.RowCount - 1);
					EnableDisableButtons ();
				});
				
				tblSelectedLines.Source = new PilotSelectedLinesTableSource (this);
				
				btnClear.Activated += (object sender, EventArgs e) => {
					selectedLines.Clear ();
					tblSelectedLines.ReloadData ();
					foreach (var item in Available) {
						item.Selected = false;
					}
					cwAvailableLines.ItemPrototype = new MyCollectionViewItem ();
					EnableDisableButtons ();
				};
				btnDelete.Activated += (object sender, EventArgs e) => {
					foreach (var ind in rightSelection) {
						var item = selectedLines [ind];
						selectedLines.RemoveAt (ind);
						Available.Find (x => x.Title == item).Selected = false;
					}
					tblSelectedLines.DeselectAll (this);
					tblSelectedLines.ReloadData ();
					cwAvailableLines.ItemPrototype = new MyCollectionViewItem ();
					EnableDisableButtons ();
				};
				EnableDisableButtons ();
				btnSubmit.Activated += (object sender, EventArgs e) => {
					if (selectedLines.Count != 0) {
						//set the properties required to POST the webrequest to SWA server.
						SubmitBid submitBid = new SubmitBid ();
						submitBid.Base = GlobalSettings.BidPrepDetails.Domicile;
						submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
						submitBid.BidRound = (GlobalSettings.BidPrepDetails.BidRound == "B") ? "Round 2" : "Round 1";
						submitBid.PacketId = GenaratePacketId ();
						submitBid.Seat = GlobalSettings.BidPrepDetails.Position;
						if (GlobalSettings.BidPrepDetails.Position != "CP") {
							AvoidanceBids avoidanceBids = GlobalSettings.WBidINIContent.AvoidanceBids;
							if (GlobalSettings.BidPrepDetails.IsChkAvoidanceBid) {
								submitBid.Pilot1 = submitBid.Pilot2 = submitBid.Pilot3 = null;
				
							} else {
								submitBid.Pilot1 = (avoidanceBids.Avoidance1 == "0") ? null : avoidanceBids.Avoidance1;
								submitBid.Pilot2 = (avoidanceBids.Avoidance2 == "0") ? null : avoidanceBids.Avoidance2;
								submitBid.Pilot3 = (avoidanceBids.Avoidance3 == "0") ? null : avoidanceBids.Avoidance3;
							}
						}
						//genarate bid line to submit
						submitBid.Bid = GenarateBidLineString ();
						submitBid.TotalBidChoices = selectedLines.Count ();
						GlobalSettings.SubmitBid = submitBid;
				
						var query = new QueryWindowController ();
						this.Window.AddChildWindow (query.Window, NSWindowOrderingMode.Above);
						NSApplication.SharedApplication.RunModalForWindow (query.Window); 
				
					}
				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		private void GenerateLineList()
		{
			// BidList = new ObservableCollection<LineDetails>();

			if (GlobalSettings.BidPrepDetails.IsOnStartWithCurrentLine)
			{
				int count =GlobalSettings.BidPrepDetails.LineFrom;

				foreach (Line line in GlobalSettings.Lines)
				{
					selectedLines.Add(line.LineNum.ToString());
					count++;
				}
			}
		}

		/// <summary>
		/// PURPOSE : Generate Bid lines
		/// </summary>
		/// <returns></returns>
		private string GenarateBidLineString()
		{
			string bidLines = string.Empty;
			bidLines = string.Join(",", selectedLines.Select(x => x.ToString()));
			return bidLines;
		}

		/// <summary>
		/// PURPOSE :Genarate Packet Id for Submit Bid Format:
		// Format: BASE || Year || Month || bid-round-number eg(Value=BWI2001032)
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
			else
			{
				packetid += (GlobalSettings.BidPrepDetails.BidRound == "D") ? "4" : "5";
			}

			return packetid;
		}

		public void EnableDisableButtons ()
		{
			lblBids.StringValue = selectedLines.Count.ToString() + " Bids";
			if (rightSelection.Count > 0)
				btnDelete.Enabled = true;
			else
				btnDelete.Enabled = false;

			if (selectedLines.Count > 0)
				btnClear.Enabled = true;
			else
				btnClear.Enabled = false;
		}
	}

	public partial class PilotSelectedLinesTableSource : NSTableViewSource 
	{
		BidEditorPilotWindowController parentPilot;
		public PilotSelectedLinesTableSource (BidEditorPilotWindowController parent)
		{
			parentPilot = parent;
		}
		public override nint GetRowCount (NSTableView tableView)
		{
			return parentPilot.selectedLines.Count;
		}
		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			//var lineDisp = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == parentWC.AvailableLineList [row]).LineDisplay;
			return new NSString (parentPilot.selectedLines[(int)row]);		
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var table = (NSTableView)notification.Object;
			if (table.SelectedRowCount > 0) {
				parentPilot.rightSelection = table.SelectedRows.ToList ().ConvertAll (x => (int)x);
			} else
				parentPilot.rightSelection.Clear ();
			parentPilot.EnableDisableButtons ();
		}
	}


	public class MyCollectionViewItem : NSCollectionViewItem
	{
		private static readonly NSString EMPTY_NSSTRING = new NSString(string.Empty);
		private MyView view;
		public MyCollectionViewItem() : base()
		{

		}

		public MyCollectionViewItem(IntPtr ptr) : base(ptr)
		{

		}

		public override void LoadView ()
		{
			view = new MyView();
			View = view;
		}

		public override NSObject RepresentedObject 
		{
			get { return base.RepresentedObject; }

			set 
			{
				if (value == null) {
					// Need to do this because setting RepresentedObject in base to null 
					// throws an exception because of the MonoMac generated wrappers,
					// and even though we don't have any null values, this will get 
					// called during initialization of the content with a null value.
					base.RepresentedObject = EMPTY_NSSTRING;
					view.Button.Title = string.Empty;
				} else {
					base.RepresentedObject = value;
					var item = (ItemClass)value;
					view.Button.Title = item.Title;
					if (item.Selected) {
//						view.WantsLayer = true;
						this.view.BackView.BackgroundColor = NSColor.Gray;
//						view.NeedsLayout = true;
					}
					view.Button.Activated += delegate {
						//view.WantsLayer = true;
                        this.view.BackView.BackgroundColor = this.view.BackView.BackgroundColor == NSColor.Gray ? NSColor.ControlBackground : NSColor.Gray;
                      
                        //view.NeedsLayout = true;
                        NSNotificationCenter.DefaultCenter.PostNotificationName ("EditorUpdate", new NSString (view.Button.Title));
					};
				}
			}
		}
	}

	public class MyView : NSView
	{
		private NSButton button;
		private NSTextField backView;

		public MyView() : base(new CGRect(0,0,50,50))
		{
			backView = new NSTextField (new CGRect(5,5,40,40));
			backView.Bordered = false;
			backView.Editable = false;
			AddSubview(backView);
			button = new NSButton(new CGRect(5,5,40,40));
			button.Bordered = false;
			AddSubview(button);
		}

		public NSButton Button
		{
			get { return button; }	
		}	

		public NSTextField BackView
		{
			get { return backView; }	
		}		

	}

	public class ItemClass : NSObject
	{
		public bool Selected { get; set; }
		public string Title { get; set; }
	}
}

