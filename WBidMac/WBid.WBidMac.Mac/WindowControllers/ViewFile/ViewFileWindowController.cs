
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Model.SWA;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBidDataDownloadAuthorizationService.Model;
using System.Text.RegularExpressions;
using System.ServiceModel;
//using System.Collections.Generic;
using WBid.WBidiPad.Model;
//using System.Linq;
using System.IO;
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;

namespace WBid.WBidMac.Mac
{
	public partial class ViewFileWindowController : AppKit.NSWindowController
	{
		#region Constructors
		string[] domicileArray = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();

		//0--Cover Letter, 1--seniority List, 2-- View Awards
		public int ViewType {
			get;
			set;
		}
		// Called when created from unmanaged code
		public ViewFileWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ViewFileWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ViewFileWindowController () : base ("ViewFileWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new ViewFileWindow Window {
			get {
				return (ViewFileWindow)base.Window;
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
				
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				btnViewFile.Activated += btnViewFileClicked;
				
				btnDomicile.Activated += ReloadView;
				btnPosition.Activated += ReloadView;
				btnMonthly.AllowsEmptySelection = true;
				btnBlankReserve.AllowsEmptySelection = true;
				btnMonthly.Activated += HandleMonthyButton;
				;
				btnBlankReserve.Activated += HandleBlankreserveButton;
				;
				SetupViews ();
				SetAvailabilityofFileStatus ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		void HandleBlankreserveButton (object sender, EventArgs e)
		{
			try {
				btnMonthly.DeselectAllCells ();
				SetAvailabilityofFileStatus ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleMonthyButton (object sender, EventArgs e)
		{
			try {
				btnBlankReserve.DeselectAllCells ();
				SetAvailabilityofFileStatus ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void ReloadView (object sender, EventArgs e)
		{
			SetAvailabilityofFileStatus ();
		}

		void SetupViews ()
		{
			try {
				btnDomicile.AddItems (domicileArray);
				//btnDomicile.SelectItem (GlobalSettings.WbidUserContent.UserInformation.Domicile);
				if (GlobalSettings.CurrentBidDetails != null) {
					btnDomicile.SelectItem (GlobalSettings.CurrentBidDetails.Domicile);
					if (GlobalSettings.CurrentBidDetails.Postion == "CP")
						btnPosition.SelectCellWithTag (0);
					else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
						btnPosition.SelectCellWithTag (1);
					else
						btnPosition.SelectCellWithTag (2);
				
				
					if (GlobalSettings.CurrentBidDetails.Round == "M") {
						btnMonthly.SelectCellWithTag (ViewType);
						btnBlankReserve.DeselectAllCells ();
					} else {
						btnBlankReserve.SelectCellWithTag (ViewType);
						btnMonthly.DeselectAllCells ();
					}
				
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		void btnViewFileClicked (object sender, EventArgs e)
		{
			try {
				var file = GetFileNametoOpen ();
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + file)) {
					var fileViewer = new FileWindowController ();
					if ((btnMonthly.SelectedTag == 0) || (btnBlankReserve.SelectedTag == 0)) {
						fileViewer.Window.Title = "Cover Letter";
					} else if ((btnMonthly.SelectedTag == 1) || (btnBlankReserve.SelectedTag == 1)) {
						fileViewer.Window.Title = "Seniority List";
						//fileViewer.isCourier = true;
					} else if ((btnMonthly.SelectedTag == 2) || (btnBlankReserve.SelectedTag == 2)) {
						fileViewer.Window.Title = "Bid Awards";
					}
					fileViewer.LoadTXT (file);
					//this.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
					fileViewer.Window.MakeKeyAndOrderFront (this);
					NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		/// <summary>
		/// Create the filename to open 
		/// </summary>
		/// <returns></returns>
		private string GetFileNametoOpen()
		{
			string filename = string.Empty;
//			if (domicileName != "Select Domicile")
//			{
			if ((btnMonthly.SelectedTag == 0) || (btnBlankReserve.SelectedTag == 0)) {
				filename = CreateCoverletterFileName ();
			} else if ((btnMonthly.SelectedTag == 1) || (btnBlankReserve.SelectedTag == 1)) {
				filename = CreateSeniorityLetterFileName ();
			} else if ((btnMonthly.SelectedTag == 2) || (btnBlankReserve.SelectedTag == 2)) {
				filename = CreateBidAwardFileName ();
			}
//			}
			return filename;
		}

		/// <summary>
		/// Create the Cover letter name to open the file
		/// </summary>
		/// <returns></returns>
		private string CreateCoverletterFileName()
		{
			string coverletterfileName = string.Empty;
			if (btnMonthly.SelectedTag == 0) {
				//first round cover letter
				coverletterfileName = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "C" + ".TXT";
			} else if (btnBlankReserve.SelectedTag == 0) {
				if (btnPosition.SelectedTag == 2) {
					// get flight attendant 2nd round cover letter and seniority list
					coverletterfileName = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "CR" + ".TXT";

				} else {
					// get pilot 2nd round cover letter and seniority list
					coverletterfileName = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "R" + ".TXT";
				}
			}
			return coverletterfileName;
		}

		/// <summary>
		/// Create the seniority filename to open the file
		/// </summary>
		/// <returns></returns>
		private string CreateSeniorityLetterFileName()
		{
			string senioritylistfilename = string.Empty;
			if (btnMonthly.SelectedTag == 1) {
				//first round seniority list
				senioritylistfilename = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "S" + ".TXT";
			} else if (btnBlankReserve.SelectedTag == 1) {
				if (btnPosition.SelectedTag == 2) {
					// get flight attendant 2nd round cover letter and seniority list
					senioritylistfilename = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "SR" + ".TXT";

				} else {
					// get pilot 2nd round cover letter and seniority list
					senioritylistfilename = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "R" + ".TXT";
				}
			}
			return senioritylistfilename;
		}
		/// <summary>
		/// Create Bid Award filename
		/// </summary>
		/// <returns></returns>
		private string CreateBidAwardFileName()
		{
			string bidawardfileName = string.Empty;
			if (btnMonthly.SelectedTag == 2) {
				// get pilot and flight attend first round  award filename.
				bidawardfileName = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "M" + ".TXT";
			} else if (btnBlankReserve.SelectedTag == 2) {
				// get pilot and  flight attendant 2nd round  award filename
				bidawardfileName = btnDomicile.SelectedItem.Title + getLongPositionString ((int)btnPosition.SelectedTag) + "W" + ".TXT";
			}
			return bidawardfileName;
		}
		private string getLongPositionString(int n)
		{
			if (n == 0)
				return "CP";
			else if (n == 1)
				return "FO";
			else if (n == 2)
				return "FA";
			else
				return "";
		}
		/// <summary>
		/// If file exists set the IsFileAvaialable property to true
		/// </summary>
		/// <param name="fileName"></param>
		private void SetAvailabilityofFileStatus()
		{
			//get the file name to open
			string fileName = GetFileNametoOpen ();
			if (fileName != string.Empty) {
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName)) {
					// enable  the view file button 
					btnViewFile.Enabled = true;
				} else {
					// Disable  the view file button 
					btnViewFile.Enabled = false;

				}
			} else
				// Disable  the view file button 
				btnViewFile.Enabled = false;

		}
	}
}

