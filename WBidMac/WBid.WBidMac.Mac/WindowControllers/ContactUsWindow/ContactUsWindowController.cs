
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Core;
using System.Text;
using System.ServiceModel;

namespace WBid.WBidMac.Mac
{
	public partial class ContactUsWindowController : AppKit.NSWindowController
	{
        


		#region Constructors

		// Called when created from unmanaged code
		public ContactUsWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ContactUsWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ContactUsWindowController () : base ("ContactUsWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
        void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new ContactUsWindow Window {
			get {
				return (ContactUsWindow)base.Window;
			}
		}
		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				this.ShouldCascadeWindows = false;
				this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				
				};
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
				};
				
				btnOK.Activated += HandleOk;
				lblVersion.StringValue = "Version : " + System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
				
				if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {
					txtEmail.StringValue = GlobalSettings.WbidUserContent.UserInformation.Email;
					txtName.StringValue = GlobalSettings.WbidUserContent.UserInformation.FirstName + " " + GlobalSettings.WbidUserContent.UserInformation.LastName;
                    GetEmployeeDetailsFromDB();

				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}


		}
        private void GetEmployeeDetailsFromDB()
        {
            BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
            WBidDataDwonloadAuthServiceClient client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
            client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
            client.GetUserDetailsCompleted+=Client_GetUserDetailsCompleted;
            client.GetUserDetailsAsync(GlobalSettings.WbidUserContent.UserInformation.EmpNo);

        }

        void Client_GetUserDetailsCompleted(object sender, GetUserDetailsCompletedEventArgs e)
        {
            try
            {
                InvokeOnMainThread(() =>
                {
                    if (e.Result != null)
                    {
                        txtPhoneNo.StringValue = e.Result.CellPhone;
                        txtEmpNum.StringValue = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
                    }
                });

            }
            catch(Exception ex)
            {}
        }


		void HandleOk (object sender, EventArgs e)
		{

			try {
				if (!ValidateUI ())
					return;
				
                if (Reachability.CheckVPSAvailable()) {
				
					SendMailToAdmin ();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}


		}

		public void SendMailToAdmin()
		{

			string employeeNumber = string.Empty;
			if (GlobalSettings.WbidUserContent != null && GlobalSettings.WbidUserContent.UserInformation != null) {
				employeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
			}
			var sb = new StringBuilder();
			sb.Append("<table width=\"500\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Hi Admin ,</td></tr>");
			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">&nbsp;</td></tr>");
			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px; padding: 0 0 10px 0;\">");
			sb.Append(txtDescription.Value);
			sb.Append("</td></tr>");
			sb.Append("</table>");
			sb.Append("<table width=\"250\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\">");
			sb.Append("<tr><td></td><td></td><td></td></tr><tr><td colspan=\"3\">&nbsp;</td></tr><tr><td colspan=\"3\">&nbsp;</td></tr>");
			sb.Append("<tr><td align=\"left\" width=\"127px\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Name</td>");
			sb.Append("<td>:</td><td width=\"373px\" align=\"left\" valign=\"top\">");
			sb.Append(txtName.StringValue);
			sb.Append("</td></tr><tr>");
			sb.Append("<td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Emp no </td><td width=\"10\">:</td><td align=\"left\" valign=\"top\">");
			sb.Append(employeeNumber);
			sb.Append("</td></tr>");
			sb.Append("<tr><td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Email </td><td width=\"10\">:</td><td align=\"left\" valign=\"top\">");
			sb.Append(txtEmail.StringValue);

			sb.Append("</td></tr><tr><td align=\"left\" valign=\"middle\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;\">Phone No </td><td>:</td>");
			sb.Append("<td align=\"left\" valign=\"top\">");
			sb.Append(txtPhoneNo.StringValue);
			sb.Append("</td></tr>");
			sb.Append("<tr><td align=\"left\" valign=\"middle\"  style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Version</td><td>:</td>");
			sb.Append("<td align=\"left\" valign=\"top\">");
			sb.Append(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
			//sb.Append("</td></tr><tr><td align=\"left\" valign=\"middle\"  style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: justify; font-size:13px;\">Installation Date </td><td>:</td>");
			//sb.Append("<td align=\"left\" valign=\"top\">");
			//sb.Append(installeddate);
			//sb.Append("</td></tr>");

			//sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" >");
			//sb.Append("<br/>");
			//sb.Append(SortState());
			//sb.Append(WeightStates().ToString());
			//sb.Append(ConstraintsStates().ToString());
			//sb.Append(GetOSInformation());
			sb.Append("</td></tr>");

			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\"><br/><br/>Sincerely ,</td></tr>");
			sb.Append("<tr><td align=\"left\" valign=\"top\" colspan=\"3\" style=\"color: #000000;font-family: Verdana, Helvetica, sans-serif;text-align: Left; font-size:13px;padding:15px 0 0 0;\">");
			sb.Append(txtName.StringValue);
			sb.Append("</td></tr>");
			sb.Append("</table>");
			string email = txtEmail.StringValue;
			WBidMail objMailAgent = new WBidMail();

			BeginInvokeOnMainThread(() =>
				{

                objMailAgent.SendSupportMail(sb.ToString(), email, "WBidMax Support");


					var alert = new NSAlert ();
					alert.AlertStyle = NSAlertStyle.Warning;
					alert.MessageText = "WBidMax";
					alert.InformativeText = "Mail sent Successfully.";
					alert.AddButton("OK");
					alert.RunModal ();


					this.Window.Close();
//					InvokeOnMainThread(() =>
//						{
//							//loadingOverlay.Hide();
//							//this.DismissViewController(true, null);
//						});
				});
		}


		private bool ValidateUI()
		{
			bool status = true;
			string message = string.Empty;

			if (string.IsNullOrEmpty (txtName.StringValue.Trim ())) {
				txtName.BecomeFirstResponder ();
				message = "Name required";
				status = false;

			} else if (string.IsNullOrEmpty (txtPhoneNo.StringValue.Trim ())) {

				txtPhoneNo.BecomeFirstResponder ();
				message = "Phone Number required";
				status = false;
			}
			if(!RegXHandler.PhoneNumberValidation(txtPhoneNo.StringValue))
			{
				txtPhoneNo.BecomeFirstResponder ();
				message = "Invalid Phone Number";
				status = false;
			}

            if (!RegXHandler.EmployeeNumberValidation(txtEmpNum.StringValue))
            {
                txtEmpNum.BecomeFirstResponder();
                message = "Invalid Employee Number";
                status = false;
            }

			else if (string.IsNullOrEmpty (txtEmail.StringValue.Trim ())) {

				txtEmail.BecomeFirstResponder ();
				message = "Email Address required";
				status = false;
			}
			else if (!RegXHandler.EmailValidation(txtEmail.StringValue.Trim ())) {

				txtEmail.BecomeFirstResponder ();
				message = "Invalid email";
				status = false;
			}


			else if (string.IsNullOrEmpty (txtDescription.Value.Trim ())) {

				//txtDescription.BecomeFirstResponder ();
				message = "Description required";
				status = false;
			}

			if(!status)
			{

				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.MessageText = "WBidMax";
				alert.InformativeText = message;
				alert.AddButton("OK");
				//alert.Window.SetFrameTopLeftPoint(new System.Drawing.PointF(0,0));
				alert.RunModal ();

			}

			return status;
		}
	}
}

