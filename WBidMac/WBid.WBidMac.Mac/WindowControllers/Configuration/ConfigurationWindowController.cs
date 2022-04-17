
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
using WBid.WBidiPad.Model;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using System.IO;
using WBid.WBidiPad.PortableLibrary;

//using System.Collections.Generic;
//using System.Linq;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.SharedLibrary.Utility;

namespace WBid.WBidMac.Mac
{
	public partial class ConfigurationWindowController : AppKit.NSWindowController
	{
		#region Constructors


		private List<string> _startDowList = null;
		private List<string> _autoSaveList = null;

		private bool _isChangeInWeekTab = false;
		private bool _isChangeInAmPmTab = false;



		// Called when created from unmanaged code
		public ConfigurationWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ConfigurationWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ConfigurationWindowController () : base ("ConfigurationWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new ConfigurationWindow Window {
			get {
				return (ConfigurationWindow)base.Window;
			}
		}

		static NSButton closeButton;

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();




				this.ShouldCascadeWindows = false;
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				CommonClass.isShowDataTab = false;
				SetupButtons ();
				TimePickerSetup ();
				SetupViews ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		void TimePickerSetup ()
		{
			txtAMAfter.Locale = new NSLocale ("NL");
			txtPMAfter.Locale = new NSLocale ("NL");
			txtNTEAfter.Locale = new NSLocale ("NL");
			txtAMBefore.Locale = new NSLocale ("NL");
			txtPMBefore.Locale = new NSLocale ("NL");
			txtNTEBefore.Locale = new NSLocale ("NL");

			txtAMAfter.TimeZone = NSTimeZone.LocalTimeZone;
			txtPMAfter.TimeZone = NSTimeZone.LocalTimeZone;
			txtNTEAfter.TimeZone = NSTimeZone.LocalTimeZone;
			txtAMBefore.TimeZone = NSTimeZone.LocalTimeZone;
			txtPMBefore.TimeZone = NSTimeZone.LocalTimeZone;
			txtNTEBefore.TimeZone = NSTimeZone.LocalTimeZone;

			txtAMAfter.Calendar = NSCalendar.CurrentCalendar;
			txtPMAfter.Calendar = NSCalendar.CurrentCalendar;
			txtNTEAfter.Calendar = NSCalendar.CurrentCalendar;
			txtAMBefore.Calendar = NSCalendar.CurrentCalendar;
			txtPMBefore.Calendar = NSCalendar.CurrentCalendar;
			txtNTEBefore.Calendar = NSCalendar.CurrentCalendar;
		}

		public void SetupViews ()
		{
			if (CommonClass.isHomeWindow) {
				if (vwTab.Items.Contains (vwTab.Items.FirstOrDefault (x => x.Label == "AM/PM"))) {
					vwTab.Remove (vwTab.Items.FirstOrDefault (x => x.Label == "AM/PM"));
				}
			}

			_isChangeInAmPmTab = false;
			_isChangeInWeekTab = false;


			//Misc
			swShowCoverLetter.State = GlobalSettings.WBidINIContent.MiscellaneousTab.Coverletter ? NSCellStateValue.On : NSCellStateValue.Off;
			swCheckDataUpdates.State = GlobalSettings.WBidINIContent.MiscellaneousTab.DataUpdate ? NSCellStateValue.On : NSCellStateValue.Off;
			swGatherTrips.State = GlobalSettings.WBidINIContent.MiscellaneousTab.GatherData ? NSCellStateValue.On : NSCellStateValue.Off;
			swGettingMissingTrips.State = GlobalSettings.WBidINIContent.MiscellaneousTab.IsRetrieveMissingData ? NSCellStateValue.On : NSCellStateValue.Off;

			//Pairing Export
			btnCalTime.SelectCellWithTag (GlobalSettings.WBidINIContent.PairingExport.IsCentralTime ? 0 : 1);
			btnCalPlan.SelectCellWithTag (GlobalSettings.WBidINIContent.PairingExport.IsEntirePairing ? 0 : 1);
			swPairingOnSubject.State = GlobalSettings.WBidINIContent.PairingExport.IsSubjectLineSelected ? NSCellStateValue.On : NSCellStateValue.Off;

			//Am/Pm
			btnAMPMOptions.SelectCellWithTag (GlobalSettings.WBidINIContent.AmPmConfigure.HowCalcAmPm);
			btnAMPMLineOptions.SelectCellWithTag (GlobalSettings.WBidINIContent.AmPmConfigure.NumberOrPercentageCalc);
			txtAMAfter.DateValue = DateTime.Parse (GlobalSettings.WBidINIContent.AmPmConfigure.AmPush.ToString (@"hh\:mm")).DateTimeToNSDate();
			txtPMAfter.DateValue = DateTime.Parse (GlobalSettings.WBidINIContent.AmPmConfigure.PmPush.ToString (@"hh\:mm")).DateTimeToNSDate();
			txtNTEAfter.DateValue = DateTime.Parse (GlobalSettings.WBidINIContent.AmPmConfigure.NitePush.ToString (@"hh\:mm")).DateTimeToNSDate();
			txtAMBefore.DateValue = DateTime.Parse (GlobalSettings.WBidINIContent.AmPmConfigure.AmLand.ToString (@"hh\:mm")).DateTimeToNSDate();
			txtPMBefore.DateValue = DateTime.Parse (GlobalSettings.WBidINIContent.AmPmConfigure.PmLand.ToString (@"hh\:mm")).DateTimeToNSDate();
			txtNTEBefore.DateValue = DateTime.Parse (GlobalSettings.WBidINIContent.AmPmConfigure.NiteLand.ToString (@"hh\:mm")).DateTimeToNSDate();
			txtAMPMMaxNum.Enabled = (btnAMPMLineOptions.SelectedTag == 1);
			txtAMPMMaxPercent.Enabled = (btnAMPMLineOptions.SelectedTag == 2);
			txtAMPMMaxNum.StringValue = GlobalSettings.WBidINIContent.AmPmConfigure.NumOpposites.ToString ();
			txtAMPMMaxPercent.StringValue = GlobalSettings.WBidINIContent.AmPmConfigure.PctOpposities.ToString ();
			//Week
			_startDowList = new List<string> () { "0", "1", "2", "3", "4" };
			btnStartDay.RemoveAllItems ();
			btnStartDay.AddItems (_startDowList.ToArray ());
			if (GlobalSettings.WBidINIContent.Week != null) {
				btnWeekOptions.SelectCellWithTag (GlobalSettings.WBidINIContent.Week.IsMaxWeekend ? 0 : 1);
				txtWeekMaxNum.StringValue = GlobalSettings.WBidINIContent.Week.MaxNumber;
				txtWeekMaxPercent.StringValue = GlobalSettings.WBidINIContent.Week.MaxPercentage;
				txtWeekMaxNum.Enabled = (btnWeekOptions.SelectedTag == 0);
				txtWeekMaxPercent.Enabled = (btnWeekOptions.SelectedTag == 1);
				btnStartDay.SelectedItem.Title = GlobalSettings.WBidINIContent.Week.StartDOW;




			}


			//Hotels

			if (GlobalSettings.WBidINIContent.SourceHotel != null) {
				btnHotelOptions.SelectCellWithTag (GlobalSettings.WBidINIContent.SourceHotel.SourceType);
			}

			//User

			//swAutoSave.Enabled = false;
			_autoSaveList = new List<string>{ "1", "2", "3", "4", "5", "10", "15", "20" };
			btnSaveTime.RemoveAllItems ();
			btnSaveTime.AddItems (_autoSaveList.ToArray ());
			if (GlobalSettings.WBidINIContent.User != null) {
				swBidReceipt.State = GlobalSettings.WBidINIContent.User.IsNeedBidReceipt ? NSCellStateValue.On : NSCellStateValue.Off;
				swSmartSynch.State = GlobalSettings.WBidINIContent.User.SmartSynch ? NSCellStateValue.On : NSCellStateValue.Off;
				//IsMIL = GlobalSettings.WBidINIContent.User.MIL;
				swAutoSave.State = GlobalSettings.WBidINIContent.User.AutoSave ? NSCellStateValue.On : NSCellStateValue.Off;
				;
				btnSaveTime.Enabled = (swAutoSave.State == NSCellStateValue.On);
				btnSaveTime.SelectedItem.Title = GlobalSettings.WBidINIContent.User.AutoSavevalue.ToString ();
				if (btnSaveTime.SelectedItem.Title == "0") {
					btnSaveTime.SelectedItem.Title = "10";
				}
				swSendCrash.State = GlobalSettings.WBidINIContent.User.IsNeedCrashMail ? NSCellStateValue.On : NSCellStateValue.Off;
				chSummaryShade.State=GlobalSettings.WBidINIContent.User.IsSummaryViewShade ? NSCellStateValue.On : NSCellStateValue.Off;

				btnModernManuaMoveBorder.State = GlobalSettings.WBidINIContent.User.IsModernViewShade? NSCellStateValue.On : NSCellStateValue.Off;
			}
			btnCSWPos.SelectCellWithTag ((GlobalSettings.WBidINIContent.User.IsCSWViewFloat) ? 1 : 0);
			SetApplyButtonState (false);

			if (CommonClass.isShowDataTab) {
				var dataItem = new NSTabViewItem () { 
					Label = "Data",
					View = vwData
				};
				btnData.SelectCellWithTag ((GlobalSettings.WBidINIContent.Data.IsCompanyData) ? 0 : 1);
				vwTab.Add (dataItem);
			} else {
				if (vwTab.Items.Contains (vwTab.Items.FirstOrDefault (x => x.Label == "Data"))) {
					vwTab.Remove (vwTab.Items.FirstOrDefault (x => x.Label == "Data"));
				}
			}
//			swMILDisplay.Enabled = false;
			swMILDisplay.State = (GlobalSettings.WBidINIContent.User.MIL) ? NSCellStateValue.On : NSCellStateValue.Off;
		}

		partial void funChangeBlueBorderFeature(NSObject sender)
		{
            SetApplyButtonState(true);
		}
		void SetupButtons ()
		{
			btnApply.Activated += (object sender, EventArgs e) => {

				UpdateINIContent ();
			};
			btnOK.Activated += (object sender, EventArgs e) => {

				if (btnApply.Enabled) {
					UpdateINIContent ();
				}
				this.Window.Close ();
			};

			btnAMPMReset.Activated += (object sender, EventArgs e) => {
				try {
					ResetAmPmClick ();
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			btnWeekReset.Activated += (object sender, EventArgs e) => {
				try {
					ResetWeekClick ();
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			btnAMPMOptions.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			btnAMPMLineOptions.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					txtAMPMMaxNum.Enabled = (btnAMPMLineOptions.SelectedTag == 1);
					txtAMPMMaxPercent.Enabled = (btnAMPMLineOptions.SelectedTag == 2);
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};


			txtAMAfter.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			txtPMAfter.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			txtNTEAfter.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			txtAMBefore.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			txtPMBefore.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}	
			};
			txtNTEBefore.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}	
			};

			txtAMPMMaxNum.Changed += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			txtAMPMMaxPercent.Changed += (object sender, EventArgs e) => {
				try {
					_isChangeInAmPmTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};




			swShowCoverLetter.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			swCheckDataUpdates.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}

			};
			swGatherTrips.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			swGettingMissingTrips.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			btnCalTime.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			btnCalPlan.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			swPairingOnSubject.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			btnWeekOptions.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInWeekTab = true;
					SetApplyButtonState (true);
					txtWeekMaxNum.Enabled = (btnWeekOptions.SelectedTag == 0);
					txtWeekMaxPercent.Enabled = (btnWeekOptions.SelectedTag == 1);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			txtWeekMaxNum.Changed += (object sender, EventArgs e) => {
				try {
					_isChangeInWeekTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			txtWeekMaxPercent.Changed += (object sender, EventArgs e) => {
				try {
					_isChangeInWeekTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
		
			btnStartDay.Activated += (object sender, EventArgs e) => {
				try {
					_isChangeInWeekTab = true;
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}

			};
			btnHotelOptions.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};

			swBidReceipt.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			swSmartSynch.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			swAutoSave.Activated += (object sender, EventArgs e) => {
				try {
					btnSaveTime.Enabled = (swAutoSave.State == NSCellStateValue.On);
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			swSendCrash.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			btnSaveTime.Activated += (object sender, EventArgs e) => {
				try {
					SetApplyButtonState (true);
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			};
			txtAMPMMaxNum.Changed += (object sender, EventArgs e) => {
				txtAMPMMaxNum.StringValue = txtAMPMMaxNum.StringValue;
			};
			txtAMPMMaxPercent.Changed += (object sender, EventArgs e) => {
				txtAMPMMaxPercent.StringValue = txtAMPMMaxPercent.StringValue;
			};
			txtWeekMaxNum.Changed += (object sender, EventArgs e) => {
				txtWeekMaxNum.StringValue = txtWeekMaxNum.StringValue;
			};
			txtWeekMaxPercent.Changed += (object sender, EventArgs e) => {
				txtWeekMaxPercent.StringValue = txtWeekMaxPercent.StringValue;
			};

			btnData.Activated += delegate {
				SetApplyButtonState (true);
			};
			btnCSWPos.Activated += (object sender, EventArgs e) => {
				SetApplyButtonState (true);
			};
			swMILDisplay.Activated += (object sender, EventArgs e) => {
				SetApplyButtonState (true);
			};
			chSummaryShade.Activated+=(object sender, EventArgs e) => {
                SetApplyButtonState (true);
			};
		}

		private void SetApplyButtonState (bool status)
		{
			btnApply.Enabled = status;
		}


		private void ResetAmPmClick ()
		{

			_isChangeInAmPmTab = true;
			btnAMPMOptions.SelectCellWithTag (3);
			btnAMPMLineOptions.SelectCellWithTag (2);
			txtAMAfter.DateValue = DateTime.Parse (TimeSpan.FromHours (4).ToString (@"hh\:mm")).DateTimeToNSDate();
			txtPMAfter.DateValue = DateTime.Parse (TimeSpan.FromHours (11).ToString (@"hh\:mm")).DateTimeToNSDate();
			txtNTEAfter.DateValue = DateTime.Parse (TimeSpan.FromHours (22).ToString (@"hh\:mm")).DateTimeToNSDate();
			txtAMBefore.DateValue = DateTime.Parse (TimeSpan.FromHours (19).ToString (@"hh\:mm")).DateTimeToNSDate();
			txtPMBefore.DateValue = DateTime.Parse (TimeSpan.FromHours (2).ToString (@"hh\:mm")).DateTimeToNSDate();
			txtNTEBefore.DateValue = DateTime.Parse (TimeSpan.FromHours (7).ToString (@"hh\:mm")).DateTimeToNSDate();
			txtAMPMMaxNum.Enabled = (btnAMPMLineOptions.SelectedTag == 1);
			txtAMPMMaxPercent.Enabled = (btnAMPMLineOptions.SelectedTag == 2);
			txtAMPMMaxNum.StringValue = "3";
			txtAMPMMaxPercent.StringValue = "20";
			SetApplyButtonState (true);	


	
		}

		private void ResetWeekClick ()
		{
			_isChangeInWeekTab = true;
			btnWeekOptions.SelectCellWithTag (GlobalSettings.WBidINIContent.Week.IsMaxWeekendDefault ? 0 : 1);
			txtWeekMaxNum.StringValue = GlobalSettings.WBidINIContent.Week.MaxNumberDefault;
			txtWeekMaxPercent.StringValue = GlobalSettings.WBidINIContent.Week.MaxPercentageDefault;
			txtWeekMaxNum.Enabled = (btnWeekOptions.SelectedTag == 0);
			txtWeekMaxPercent.Enabled = (btnWeekOptions.SelectedTag == 1);
			SetApplyButtonState (true);
		}


		private void UpdateINIContent ()
		{
			try {
				bool isSummaryShade = (chSummaryShade.State == NSCellStateValue.On);
				bool isModernShade = (btnModernManuaMoveBorder.State == NSCellStateValue.On);
				bool isNeedtoSummaryViewReload = false;
				bool isNeedtoModernViewReload = false;
				if (GlobalSettings.WBidINIContent.User.IsSummaryViewShade != isSummaryShade)
				{
					isNeedtoSummaryViewReload = true;
				}
				if (GlobalSettings.WBidINIContent.User.IsModernViewShade != isModernShade)
				{
					isNeedtoModernViewReload = true;
				}
				SetMiscellaneousConfigureValues ();
				
				SetParingExportConfigureValues ();
				
				SetAmPmConfigureValues ();
				
				SetWeekConfigureValues ();
				
				SetHotelConfigureValues ();
				
				SetUserConfigureValues ();
				
				XmlHelper.SerializeToXml (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());
				
				if (_isChangeInAmPmTab || _isChangeInWeekTab) {
				
					//Need to recalculate the properties of line file
				
					RecalculateAndSaveLineFile ();
				
				
				
				}
				
				
				
				_isChangeInAmPmTab = false;
				_isChangeInWeekTab = false;
				btnApply.Enabled = false;
				GlobalSettings.WBidINIContent.Data.IsCompanyData = (btnData.SelectedTag == 0);
				if (isNeedtoSummaryViewReload && CommonClass.SummaryController!=null)
				{
					CommonClass.SummaryController.TableViewReload();
				}
				if (isNeedtoModernViewReload && CommonClass.ModernController!=null)
				{
					//CommonClass.SummaryController.reloa
					//modern view reload
					NSNotificationCenter.DefaultCenter.PostNotificationName ("reloadModernViewFromMainClass", null);

				}


			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void RecalculateAndSaveLineFile ()
		{
			try {
				if (_isChangeInAmPmTab) {
					RecalculateAMPMProperties ();
					if (CommonClass.ModernController != null)
						CommonClass.ModernController.ReloadContent();
				}
				if (_isChangeInWeekTab) {
					RecalculateWeekProperties ();
				}
				
				Dictionary<string, Line> lines = new Dictionary<string, Line> ();
				foreach (Line line in GlobalSettings.Lines) {
				
					lines.Add (line.LineNum.ToString (), line);
				}
				LineInfo lineInfo = new LineInfo () {
					LineVersion = GlobalSettings.LineVersion,
					Lines = lines
				
				};
				var fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
				var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
				ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
				linestream.Dispose ();
				linestream.Close ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void RecalculateAMPMProperties ()
		{

			//Dictionary<string, Line> lines = new Dictionary<string, Line>();
			RecalcalculateLineProperties calculateLineProperties = new RecalcalculateLineProperties ();
			foreach (Line line in GlobalSettings.Lines) {
				calculateLineProperties.RecalculateAMPMPropertiesAfterAMPMDefenitionChanges(line);
				//lines.Add(line.LineNum.ToString(), line);
			}
			//return lines;
		}

		private void RecalculateWeekProperties ()
		{

			CalculateLineProperties calculateLineProperties = new CalculateLineProperties ();

			foreach (Line line in GlobalSettings.Lines) {
				line.Weekend = calculateLineProperties.CalcWkEndProp (line, true);

			}

		}


		/// <summary>
		/// Assign  Miscellaneous configuartion valus to INI file object
		/// </summary>
		private void SetMiscellaneousConfigureValues ()
		{
			var miscellaneousTab = GlobalSettings.WBidINIContent.MiscellaneousTab;
			if (miscellaneousTab != null) {

				miscellaneousTab.Coverletter = (swShowCoverLetter.State == NSCellStateValue.On);
				miscellaneousTab.DataUpdate = (swCheckDataUpdates.State == NSCellStateValue.On);
				miscellaneousTab.GatherData = (swGatherTrips.State == NSCellStateValue.On);
				miscellaneousTab.IsRetrieveMissingData = (swGettingMissingTrips.State == NSCellStateValue.On);

			}
		}


		/// <summary>
		/// Assign  Pairing Export configuartion valus to INI file object
		/// </summary>
		private void SetParingExportConfigureValues ()
		{

			var pairingExport = GlobalSettings.WBidINIContent.PairingExport;
			if (pairingExport != null) {
				pairingExport.IsCentralTime = (btnCalTime.SelectedTag == 0);
				pairingExport.IsEntirePairing = (btnCalPlan.SelectedTag == 0);
				pairingExport.IsSubjectLineSelected = (swPairingOnSubject.State == NSCellStateValue.On);
			}
		}


		/// <summary>
		/// Assign  AmPm configuartion valus to INI file object
		/// </summary>
		private void SetAmPmConfigureValues ()
		{

			var amPmConfigure = GlobalSettings.WBidINIContent.AmPmConfigure;
			if (amPmConfigure != null) {
				amPmConfigure.HowCalcAmPm = (int)btnAMPMOptions.SelectedTag;
				amPmConfigure.NumberOrPercentageCalc = (int)btnAMPMLineOptions.SelectedTag;

				TimeSpan timeSpan;
				if (TimeSpan.TryParse (txtAMAfter.DateValue.LocalValue (), out timeSpan)) {
					amPmConfigure.AmPush = timeSpan;
				}

				if (TimeSpan.TryParse (txtPMAfter.DateValue.LocalValue (), out timeSpan)) {
					amPmConfigure.PmPush = timeSpan;
				}
				if (TimeSpan.TryParse (txtNTEAfter.DateValue.LocalValue (), out timeSpan)) {
					amPmConfigure.NitePush = timeSpan;
				}

				if (TimeSpan.TryParse (txtAMBefore.DateValue.LocalValue (), out timeSpan)) {
					amPmConfigure.AmLand = timeSpan;
				}

				if (TimeSpan.TryParse (txtPMBefore.DateValue.LocalValue (), out timeSpan)) {
					amPmConfigure.PmLand = timeSpan;
				}

				if (TimeSpan.TryParse (txtNTEBefore.DateValue.LocalValue (), out timeSpan)) {
					amPmConfigure.NiteLand = timeSpan;
				}

				amPmConfigure.NumOpposites = int.Parse (txtAMPMMaxNum.StringValue);
				amPmConfigure.PctOpposities = Convert.ToDecimal (txtAMPMMaxPercent.StringValue);

			}
		}

		/// <summary>
		/// Assign  Week configuartion valus to INI file object
		/// </summary>
		private void SetWeekConfigureValues ()
		{


			var week = GlobalSettings.WBidINIContent.Week;
			if (week != null) {
				week.IsMaxWeekend = (btnWeekOptions.SelectedTag == 0);
				week.MaxNumber = txtWeekMaxNum.StringValue;
				week.MaxPercentage = txtWeekMaxPercent.StringValue;
				week.StartDOW = btnStartDay.SelectedItem.Title;
			}
		}


		/// <summary>
		/// Assign  Hotel configuartion valus to INI file object
		/// </summary>
		private void SetHotelConfigureValues ()
		{

			var sourceHotel = GlobalSettings.WBidINIContent.SourceHotel;
			if (sourceHotel != null) {
				sourceHotel.SourceType = (int)btnHotelOptions.SelectedTag;
			}
		}


		/// <summary>
		///  Assign  User configuartion valus to INI file object
		/// </summary>
		private void SetUserConfigureValues ()
		{

			var user = GlobalSettings.WBidINIContent.User;
			if (user != null) {
				user.IsNeedBidReceipt = (swBidReceipt.State == NSCellStateValue.On);
				var isSmart = (swSmartSynch.State == NSCellStateValue.On);
				if (user.SmartSynch != isSmart) {			
					if (CommonClass.MainController != null)
						CommonClass.MainController.btnSynch.Enabled = isSmart;
				}
				user.SmartSynch = isSmart;
				user.AutoSave = (swAutoSave.State == NSCellStateValue.On);
				user.AutoSavevalue = int.Parse (btnSaveTime.SelectedItem.Title);
				user.IsNeedCrashMail = (swSendCrash.State == NSCellStateValue.On);
				user.IsModernViewShade = (btnModernManuaMoveBorder.State == NSCellStateValue.On);
				user.IsSummaryViewShade=(chSummaryShade.State==NSCellStateValue.On);
				if (CommonClass.MainController != null)
					CommonClass.MainController.UpdateAutoSave ();
				user.IsCSWViewFloat = (btnCSWPos.SelectedTag == 0) ? false : true;
				if (CommonClass.CSWController != null && (CommonClass.CSWController.Window.IsVisible || CommonClass.CSWController.Window.IsMiniaturized))
					CommonClass.MainController.ShowCSW ();
				GlobalSettings.WBidINIContent.User.MIL = (swMILDisplay.State == NSCellStateValue.On) ? true : false;
				if (CommonClass.MainController != null)
					CommonClass.MainController.btnMIL.Hidden = !GlobalSettings.WBidINIContent.User.MIL;

			}
		}

	}
}

