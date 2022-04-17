
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

#region NameSpace
//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Utility;
//using iOSPasswordStorage;
//using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Linq;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.iOS.Utility;
using System.ServiceModel;
using WBidDataDownloadAuthorizationService.Model;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Text.RegularExpressions;
#endregion

namespace WBid.WBidMac.Mac
{
	public partial class SubmitWindowController : AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubmitWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubmitWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public SubmitWindowController () : base ("SubmitWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new SubmitWindow Window {
			get {
				return (SubmitWindow)base.Window;
			}
		}

		static NSButton closeButton;
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
				this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					this.Window.Close ();
					//				this.Window.OrderOut (this);
					//				NSApplication.SharedApplication.StopModal ();
				};
				txtSeniorityNo.Enabled = false;
				btnSubmitType.Activated += (object sender, EventArgs e) => {
					if (btnSubmitType.SelectedTag == 0)
						txtSeniorityNo.Enabled = false;
					else
						txtSeniorityNo.Enabled = true;
				};
				btnChangeEmp.Activated += (object sender, EventArgs e) => {
					try {
						var panel = new NSPanel ();
						var changeEmp = new ChangeEmployeeViewController ();
						CommonClass.Panel = panel;
						panel.SetContentSize (new CoreGraphics.CGSize (400, 180));
						panel.ContentView = changeEmp.View;
						NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					} catch (Exception ex) {
						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					}
				};
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					//				this.Window.OrderOut (this);
					//				NSApplication.SharedApplication.StopModal ();
				};
				btnChangeAvoidance.Activated += (object sender, EventArgs e) => {
					try {
						if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
							var panel = new NSPanel ();
							var changeAvoid = new ChangeAvoidanceViewController ();
							CommonClass.Panel = panel;
							panel.SetContentSize (new CoreGraphics.CGSize (400, 270));
							panel.ContentView = changeAvoid.View;
							NSApplication.SharedApplication.BeginSheet (panel, this.Window);
						} else if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
							var panel = new NSPanel ();
							var changeBud = new ChangeBuddyViewController ();
							CommonClass.Panel = panel;
							panel.SetContentSize (new CoreGraphics.CGSize (400, 190));
							panel.ContentView = changeBud.View;
							NSApplication.SharedApplication.BeginSheet (panel, this.Window);
						}
					} catch (Exception ex) {
						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					}
				};
				btnSubmit.Activated += (object sender, EventArgs e) => {
				
					try {

						if (!ValidateUI())
						{
							return;
						}

						GlobalSettings.SubmitBid = SetSubmitDetails ();
						
						if (GlobalSettings.CurrentBidDetails.Postion == "FA" && GlobalSettings.CurrentBidDetails.Round == "M") {
							var faChoice = new FAPositionChoiceWindowController ();
							this.Window.AddChildWindow (faChoice.Window, NSWindowOrderingMode.Above);
							NSApplication.SharedApplication.RunModalForWindow (faChoice.Window);
						} else {
							var query = new QueryWindowController ();
							this.Window.AddChildWindow (query.Window, NSWindowOrderingMode.Above);
							NSApplication.SharedApplication.RunModalForWindow (query.Window);
							//this.Window.AddChildWindow(query.Window,NSWindowOrderingMode.Above); 
                          //  query.Window.MakeKeyAndOrderFront(this);
						}
					} catch (Exception ex) {
						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					}
				};
				
				this.Window.DidEndSheet += (object sender, EventArgs e) => {
					this.Window.Title = "Submit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
					if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
						ReloadControls ();
					} else if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
						ReloadControls ();
					}
				};
				
				SetUpView ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		private bool ValidateUI()
		{

			bool status = true;
			string message = string.Empty;


			if (string.IsNullOrEmpty(txtSeniorityNo.StringValue.Trim()))
			{

				txtSeniorityNo.BecomeFirstResponder();
				message = "Seniority number required";
				status = false;
			}
			else if (!RegXHandler.NumberValidation(txtSeniorityNo.StringValue.Trim()))
			{

				txtSeniorityNo.BecomeFirstResponder();
				message = "Invalid Seniority number ";
				status = false;
			}

			else if (txtSeniorityNo.StringValue.Trim().Length > 7)
			{

				txtSeniorityNo.BecomeFirstResponder();
				message = "Invalid Seniority number ";
				status = false;
			}
			//else if (string.IsNullOrEmpty(txtSeniorityNo.StringValue.Trim()))
			//{
			//	txtSeniorityNo.BecomeFirstResponder();
			//	message = "Password required";
			//	status = false;

			//}





			if (!status)
			{

				var alert = new NSAlert();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.MessageText = "WBidMax";
				alert.InformativeText = message;
				alert.AddButton("OK");
				alert.RunModal();
			}

			return status;


		}
		public void SetUpView ()
		{
			btnSubmitType.SelectCellWithTag (0);
			txtSeniorityNo.Enabled = false;

			GlobalSettings.TemporaryEmployeeNumber = (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) ? GlobalSettings.WbidUserContent.UserInformation.EmpNo : string.Empty;
			txtSeniorityNo.StringValue = GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber.ToString ();

			this.Window.Title = "Submit Bid For " + GlobalSettings.TemporaryEmployeeNumber;
			ReloadControls ();

		}

		void ReloadControls ()
		{
			if (GlobalSettings.CurrentBidDetails.Postion == "CP") {
				lblAvoidanceBid.StringValue = "No Avoidance Bids";
				btnAvoidance.Enabled = false;
				btnAvoidance.SelectCellWithTag (1);
				btnChangeAvoidance.Title = "Change Avoidance Bids";
				btnChangeAvoidance.Enabled = false;
			} else if (GlobalSettings.CurrentBidDetails.Postion == "FO") {
				string str = GetAvoidanceBid ();
				lblAvoidanceBid.StringValue = "Avoidance Bids: " + str;
				btnAvoidance.Enabled = true;
				if (str == string.Empty)
					btnAvoidance.SelectCellWithTag (1);
				else
					btnAvoidance.SelectCellWithTag (0);
				btnChangeAvoidance.Title = "Change Avoidance Bids";
				btnChangeAvoidance.Enabled = true;
			} else if (GlobalSettings.CurrentBidDetails.Postion == "FA") 
			{
				string str = string.Empty;
				if (GlobalSettings.CurrentBidDetails.Round != "S") {
					 str = GetBuddyBid ();
				}
				lblAvoidanceBid.StringValue = "Buddy Bids: " + str;
				btnAvoidance.Enabled = true;
				if (str == string.Empty)
					btnAvoidance.SelectCellWithTag (1);
				else
					btnAvoidance.SelectCellWithTag (0);
				btnAvoidance.CellWithTag (0).Title = "Use These Buddy Bids";
				btnChangeAvoidance.Title = "Change Buddy Bids";
				btnChangeAvoidance.Enabled = true;

				if (GlobalSettings.CurrentBidDetails.Round == "S") {
					btnAvoidance.SelectCellWithTag (1);
					btnAvoidance.Enabled = false;
					btnChangeAvoidance.Enabled = false;
				}
			}
		}

		/// <summary>
		/// Get Avoidance Bid string
		/// </summary>
		/// <returns></returns>
		private string GetAvoidanceBid()
		{
			string avoidanceBidsStr = string.Empty;
			AvoidanceBids avoidancebids = GlobalSettings.WBidINIContent.AvoidanceBids;
			avoidanceBidsStr += (avoidancebids.Avoidance1 != "0") ? avoidancebids.Avoidance1.ToString() + "," : "";
			avoidanceBidsStr += (avoidancebids.Avoidance2 != "0") ? avoidancebids.Avoidance2.ToString() + "," : "";
			avoidanceBidsStr += (avoidancebids.Avoidance3 != "0") ? avoidancebids.Avoidance3.ToString() : "";
			avoidanceBidsStr = avoidanceBidsStr.TrimEnd(',');
			return avoidanceBidsStr;
		}

		/// <summary>
		/// Get Buddy Bid string
		/// </summary>
		/// <returns></returns>
		public string GetBuddyBid()
		{
			string buddyBidStr = string.Empty;
			BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
			//disable buddy bid
			buddyBidStr += (buddyBids.Buddy1 != "0") ? buddyBids.Buddy1.ToString() + "," : "";
			buddyBidStr += (buddyBids.Buddy2 != "0") ? buddyBids.Buddy2.ToString() + "," : "";
			buddyBidStr = buddyBidStr.TrimEnd(',');
			return buddyBidStr;
		}

		/// <summary>
		/// Set submit Details
		/// </summary>
		/// <returns></returns>
		private SubmitBid SetSubmitDetails()
		{
			WBid.WBidiPad.Model.BidDetails bidDetails = GlobalSettings.CurrentBidDetails;
			SubmitBid submitBid = new SubmitBid();
			//set the properties required to POST the webrequest to SWA server.
			submitBid.Base = bidDetails.Domicile;
			submitBid.Bidder = GlobalSettings.TemporaryEmployeeNumber;
			submitBid.BidRound = (bidDetails.Round == "S") ? "Round 2" : "Round 1";
			submitBid.PacketId = GenaratePacketId(bidDetails);
			submitBid.Seat = bidDetails.Postion;
			submitBid.IsSubmitAllChoices = (btnSubmitType.SelectedTag == 0);
			//int aa = sgNoAvoidanceSubmissionType.SelectedSegment;
			if (bidDetails.Postion == "FO" && (btnAvoidance.SelectedTag == 0))
			{
				AvoidanceBids avoidanceBids = GlobalSettings.WBidINIContent.AvoidanceBids;
				submitBid.Pilot1 = (avoidanceBids.Avoidance1 == "0") ? null : avoidanceBids.Avoidance1;
				submitBid.Pilot2 = (avoidanceBids.Avoidance2 == "0") ? null : avoidanceBids.Avoidance2;
				submitBid.Pilot3 = (avoidanceBids.Avoidance3 == "0") ? null : avoidanceBids.Avoidance3;
			}

			if (bidDetails.Postion == "FA" && (btnAvoidance.SelectedTag == 0))
			{
				BuddyBids buddyBids = GlobalSettings.WBidINIContent.BuddyBids;
				//comment out this to disable buddy bid
				submitBid.Buddy1 = (buddyBids.Buddy1 == "0") ? null : buddyBids.Buddy1;
				submitBid.Buddy2 = (buddyBids.Buddy2 == "0") ? null : buddyBids.Buddy2;

			}


			int seniorityNumber = int.Parse((txtSeniorityNo.StringValue.Trim() == string.Empty) ? "0" : txtSeniorityNo.StringValue.Trim());
			if (submitBid.IsSubmitAllChoices)
			{
				submitBid.SeniorityNumber = GlobalSettings.Lines.Count();
				submitBid.TotalBidChoices = GlobalSettings.Lines.Count();
				//submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Select(x => x.LineNum));
			}
			else
			{
				submitBid.SeniorityNumber = seniorityNumber;
				submitBid.TotalBidChoices = seniorityNumber;
				//submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Take(seniorityNumber).Select(x => x.LineNum));

			}

			if (bidDetails.Postion == "FO" || bidDetails.Postion == "CP" ||(bidDetails.Postion=="FA" && bidDetails.Round=="S" ))
			{
				submitBid.Bid = string.Join(",", GlobalSettings.Lines.ToList().Take(submitBid.TotalBidChoices).Select(x => x.LineNum));
			}

			return submitBid;
		}

		/// <summary>
		/// Genarate Packet Id for Submit Bid Format:
		// Format: BASE || Year || Month || bid-round-number eg(Value=BWI2001032)
		/// </summary>
		/// <param name="bidDetails"></param>
		/// <returns></returns>
		private string GenaratePacketId(WBid.WBidiPad.Model.BidDetails bidDetails)
		{
			string packetid = string.Empty;
			packetid = bidDetails.Domicile + bidDetails.Year + bidDetails.Month.ToString("d2");

			//Set-round-numbers:
			//1 - F/A monthly bids
			//2 - F/A supplemental bids
			//3 - reserved
			//4 - Pilot monthly bids
			//5 - Pilot supplemental bids

			if (bidDetails.Round == "M" && bidDetails.Postion == "FA")
			{
				packetid += "1";
			}
			else if (bidDetails.Round == "S" && bidDetails.Postion == "FA")
			{
				packetid += "2";
			}
			else if (bidDetails.Round == "M" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
			{
				packetid += "4";
			}
			else if (bidDetails.Round == "S" && (bidDetails.Postion == "FO" || bidDetails.Postion == "CP"))
			{
				packetid += "5";
			}
			return packetid;
		}

	}
}

