
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.iOS.Utility;
using System.Text.RegularExpressions;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.Model;
using System.Windows;
using WBid.WBidiPad.Core;
using System.IO;

namespace WBid.WBidMac.Mac
{
	public partial class UserRegistrationWindowController : AppKit.NSWindowController
	{
		WbidUser wbidUser;
		NSPanel panel = new NSPanel ();
		#region Constructors


		public bool IsEditMode {
			get;
			set;
		}


		// Called when created from unmanaged code
		public UserRegistrationWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public UserRegistrationWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public UserRegistrationWindowController () : base ("UserRegistrationWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new UserRegistrationWindow Window {
			get {
				return (UserRegistrationWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				//ckAutoSave.Enabled = false;
				this.ShouldCascadeWindows = false;
				
				this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				btnSubmit.Activated += HandleBtnSubmit;
				btnAccept.Activated += HandleBtnAccept;
				var lstDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy (x => x.DomicileName).ToList ();
				foreach (var item in lstDomicile) {
					btnDomicle.AddItem (item.DomicileName);
				}
				var lstPosition = WBidCollection.GetPositions ();
				foreach (var item in lstPosition) {
					btnPosition.AddItem (item.LongStr);
				}
				var arrMin = new string[]{ "5", "10", "15" };
				foreach (var item in arrMin) {
					popSaveTime.AddItem (item);
				}
				ckAutoSave.Activated += (sender, e) => {
					if (ckAutoSave.State == NSCellStateValue.On)
						popSaveTime.Enabled = true;
					else
						popSaveTime.Enabled = false;
				};
				//txtEmail.StringValue!=GlobalSettings.WbidUserContent.UserInformation.Email&&
				txtEmail.EditingEnded += (object sender, EventArgs e) => {
					if(GlobalSettings.WbidUserContent!=null&&txtEmail.StringValue!=GlobalSettings.WbidUserContent.UserInformation.Email&&txtEmail.StringValue!=txtRePass.StringValue) {
						panel.SetContentSize (new CoreGraphics.CGSize (400,250));
						panel.ContentView = vwRePass;
						txtRePass.BecomeFirstResponder ();
						txtRePass.StringValue = string.Empty;
						NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					}

//					NSTextField txtField =  new NSTextField();
//					txtField.Frame = new System.Drawing.RectangleF(0,0,200,20);
//					NSAlert alert = new NSAlert();
//					alert.MessageText = "WBidMax";
//					alert.InformativeText = "Please re-enter your email";
//					alert.AccessoryView = txtField;
//					alert.AddButton("OK");
//					alert.Buttons[0].Activated += (object senderr, EventArgs ee) => {
//						alert.Window.Close();
//						NSApplication.SharedApplication.StopModal ();
//						if(txtField.StringValue!=txtEmail.StringValue) {
//							txtEmail.StringValue=string.Empty;
//							txtEmail.BecomeFirstResponder();
//						}
//					};
//					alert.RunModal();
				};

				btnRePass.Activated += (object sender, EventArgs e) => {
					if(txtRePass.StringValue!=txtEmail.StringValue)
						txtEmail.StringValue = string.Empty;
					NSApplication.SharedApplication.EndSheet (panel);
					panel.OrderOut (this);
				};
				
				if (IsEditMode) {
					if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {
						txtFirstName.StringValue = GlobalSettings.WbidUserContent.UserInformation.FirstName;
						txtLastName.StringValue = GlobalSettings.WbidUserContent.UserInformation.LastName;
						txtEmail.StringValue = GlobalSettings.WbidUserContent.UserInformation.Email;
						txtEmployee.StringValue = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
						btnDomicle.SelectItem (GlobalSettings.WbidUserContent.UserInformation.Domicile);
						btnPosition.SelectItem (GlobalSettings.WbidUserContent.UserInformation.Position);
						btnGender.SelectCellWithTag ((GlobalSettings.WbidUserContent.UserInformation.IsFemale) ? 1 : 0);
						 
				
					}
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleBtnSubmit (object sender, EventArgs e)
	{
//			if (string.IsNullOrEmpty (txtFirstName.StringValue.Trim ()))
//				return;
//			if (string.IsNullOrEmpty (txtLastName.StringValue.Trim ()))
//				return;
//			if (string.IsNullOrEmpty (txtEmail.StringValue.Trim ()) || !EmailValidation (txtEmail.StringValue))
//				return;
//			if (string.IsNullOrEmpty (txtEmployee.StringValue.Trim ()) || !EmployeeNumberValidation (txtEmployee.StringValue))
//				return;

			try {
				if (!ValidateUI ())
					return;
				
				if (!IsEditMode) {
					wbidUser = new WbidUser ();
					wbidUser.UserInformation = new UserInformation ();
				} else {
					wbidUser = GlobalSettings.WbidUserContent;
				}
				
				//to handle null values
				//----------------------------------------------------
				if (wbidUser == null) {
					wbidUser = new WbidUser ();
					wbidUser.UserInformation = new UserInformation ();
				} else if (wbidUser.UserInformation == null) {
					wbidUser.UserInformation = new UserInformation ();
				}
				//----------------------------------------------------
				
				
				
				wbidUser.UserInformation.FirstName = txtFirstName.StringValue;
				wbidUser.UserInformation.LastName = txtLastName.StringValue;
				wbidUser.UserInformation.Email = txtEmail.StringValue;
				wbidUser.UserInformation.EmpNo = Convert.ToInt32 (System.Text.RegularExpressions.Regex.Match (txtEmployee.StringValue, @"\d+").Value).ToString ();
				wbidUser.UserInformation.Domicile = btnDomicle.SelectedItem.Title;
				wbidUser.UserInformation.Position = btnPosition.SelectedItem.Title;
				wbidUser.UserInformation.IsFemale = (btnGender.SelectedTag == 1);

				wbidUser.UserInformation.AcceptMail = btnAccept.State == NSCellStateValue.On ? true : false;
				
				GlobalSettings.WbidUserContent = wbidUser;
				
				if (!File.Exists (WBidHelper.WBidUserFilePath)) {
					this.Window.ContentView = vwUserManagement;
				} else {
					WBidHelper.SaveUserFile (wbidUser, WBidHelper.WBidUserFilePath);
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
					InvokeOnMainThread(() =>
					{
						CommonClass.HomeController.LoadContent();
					});
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


		private bool ValidateUI()
		{

			bool status = true;
			string message = string.Empty;

			if (string.IsNullOrEmpty (txtFirstName.StringValue.Trim ())) {
				txtFirstName.BecomeFirstResponder ();
				message = "First Name required";
				status = false;

			} else if (string.IsNullOrEmpty (txtLastName.StringValue.Trim ())) {

				txtLastName.BecomeFirstResponder ();
				message = "Last Name required";
				status = false;
			}

			else if (string.IsNullOrEmpty (txtEmail.StringValue.Trim ())) {

				txtEmail.BecomeFirstResponder ();
				message = "Email required";
				status = false;
			}
			else if (!RegXHandler.EmailValidation(txtEmail.StringValue.Trim ())) {

				txtEmail.BecomeFirstResponder ();
				message = "Invalid email";
				status = false;
			}

			else if (string.IsNullOrEmpty (txtEmployee.StringValue.Trim ())) {

				txtEmployee.BecomeFirstResponder ();
				message = "Employee number required";
				status = false;
			}
			else if (!RegXHandler.EmployeeNumberValidation(txtEmployee.StringValue.Trim ())) {

				txtEmployee.BecomeFirstResponder ();
				message = "Invalid Employee number ";
				status = false;
			}





				if(!status)
				{
				
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.MessageText = "WBidMax";
				alert.InformativeText = message;
				alert.AddButton("OK");
				alert.RunModal ();
				}

				return status;


		}

		void HandleBtnAccept (object sender, EventArgs e)
		{
			try {
				if (ckAutoSave.State == NSCellStateValue.On)
					GlobalSettings.WBidINIContent.User.AutoSave = true;
				else
					GlobalSettings.WBidINIContent.User.AutoSave = false;
				
				if (ckBidReceipt.State == NSCellStateValue.On)
					GlobalSettings.WBidINIContent.User.IsNeedBidReceipt = true;
				else
					GlobalSettings.WBidINIContent.User.IsNeedBidReceipt = false;
				
				if (ckCrashReports.State == NSCellStateValue.On)
					GlobalSettings.WBidINIContent.User.IsNeedCrashMail = true;
				else
					GlobalSettings.WBidINIContent.User.IsNeedCrashMail = false;
				
				if (ckSmartSync.State == NSCellStateValue.On)
					GlobalSettings.WBidINIContent.User.SmartSynch = true;
				else
					GlobalSettings.WBidINIContent.User.SmartSynch = false;
				
				if (ckAutoSave.State == NSCellStateValue.On)
					GlobalSettings.WBidINIContent.User.AutoSavevalue = int.Parse (popSaveTime.SelectedItem.Title);
				
				WBidHelper.SaveUserFile (wbidUser, WBidHelper.WBidUserFilePath);
				this.Window.Close ();
				this.Window.OrderOut (this);
				NSApplication.SharedApplication.StopModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public bool EmailValidation (string value)
		{
			string matchpattern = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

			if (!Regex.IsMatch (value, matchpattern)) {
				return false;
			}
			return true;
		}

		private bool EmployeeNumberValidation (string value)
		{
			if (!Regex.Match (value, "^[e,E,x,X,0-9][0-9]*$").Success) {
				return false;
			}
			return true;
		}
	}
}

