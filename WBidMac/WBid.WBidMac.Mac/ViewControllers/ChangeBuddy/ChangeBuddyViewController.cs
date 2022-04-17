
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
using WBid.WBidiPad.iOS.Utility;
using System.IO;
//using System.Collections.Generic;
//using System.Linq;
using System.ServiceModel;
using WBidDataDownloadAuthorizationService.Model;

namespace WBid.WBidMac.Mac
{
	public partial class ChangeBuddyViewController : AppKit.NSViewController
	{
		NSPanel overlayPanel;
		OverlayViewController overlay;
		public Dictionary <int, string> EmployeeList { get; set; }
		#region Constructors


		// Called when created from unmanaged code
		public ChangeBuddyViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ChangeBuddyViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ChangeBuddyViewController () : base ("ChangeBuddyView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ChangeBuddyView View {
			get {
				return (ChangeBuddyView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			try {
				base.AwakeFromNib ();
				LoadBidderDetails ();
				
				btnCancel.Activated += (object sender, EventArgs e) => {
					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);
				};
				btnOK.Activated += (object sender, EventArgs e) => {
				
					EmployeeDetails empdetails = new EmployeeDetails();
					empdetails.EmployeeNumbers=new Employee[2];
					var obj=new List<Employee>();
					if(string.IsNullOrEmpty(txtBuddy1.StringValue.Trim()))
						txtBuddy1.StringValue="0";
					if(string.IsNullOrEmpty(txtBuddy2.StringValue.Trim()))
						txtBuddy2.StringValue="0";


					GlobalSettings.WBidINIContent.BuddyBids.Buddy1 = txtBuddy1.StringValue.Trim ();
					GlobalSettings.WBidINIContent.BuddyBids.Buddy2 = txtBuddy2.StringValue.Trim ();
					//save the state of the INI File
					WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

					NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
					CommonClass.Panel.OrderOut (this);


//					if ((!string.IsNullOrEmpty(txtBuddy1.StringValue.Trim())) && txtBuddy1.StringValue.Trim() != "0")
//					{
//						obj.Add (new Employee { EmpNumber = txtBuddy1.StringValue.Trim() });
//					}
//					if ((!string.IsNullOrEmpty(txtBuddy2.StringValue.Trim())) && txtBuddy2.StringValue.Trim() != "0")
//					{
//						obj.Add(new Employee { EmpNumber = txtBuddy2.StringValue.Trim() });
//					}
//					empdetails.EmployeeNumbers=obj.ToArray();
//
//					if(empdetails.EmployeeNumbers.Count()>0)
//					{
//
//						if (Reachability.IsHostReachable("google.com"))
//						{    
//							
//							overlayPanel=new NSPanel();
//							overlayPanel.SetContentSize (new CoreGraphics.CGSize (400, 120));
//							overlay = new OverlayViewController ();
//							overlay.OverlayText = "Checking Athorization for Buddy Bidders..";
//							overlayPanel.ContentView = overlay.View;
//							NSApplication.SharedApplication.BeginSheet (overlayPanel, this.View.Window);
//
//							BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
//							WBidDataDwonloadAuthServiceClient client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
//							client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 60);
//
//							empdetails.Platform = "PC";
//							//client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
//							client.CheckValidSubscriptionForEmployeesCompleted+=CheckValidSubscriptionForEmployeesCompleted;
//							client.CheckValidSubscriptionForEmployeesAsync(empdetails);  
//
//						}
//						else
//						{
//							InvokeOnMainThread (() => {
//								var alert = new NSAlert ();
//								alert.AlertStyle = NSAlertStyle.Warning;
//								alert.MessageText = "WBidMax";
//								alert.InformativeText = "Connectivity not available";
//								alert.AddButton ("OK");
//
//								alert.RunModal ();
//							});
//						}
//					}

				};
			}
			catch(Exception ex) {

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		void CheckValidSubscriptionForEmployeesCompleted (object sender, CheckValidSubscriptionForEmployeesCompletedEventArgs e)
		{
			InvokeOnMainThread (() => {
				if (e.Result != null) {
					List<AuthStatusModel> authStatus = e.Result.ToList ();
					var authfailedmembers = authStatus.Where (x => x.IsValid == false).ToList ();

					if (authfailedmembers.Count () > 0) {
						string message = string.Empty;
						foreach (var item in authfailedmembers) {
							message += item.EmployeeNumber + " : " + item.Message + "\n\n";
						}

						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.MessageText = "WBidMax";
						alert.InformativeText = "All bidders must have a WBidMax account .See the details.\n\n" + message;
						alert.AddButton ("OK");

						alert.RunModal ();

					} else {
									

						GlobalSettings.WBidINIContent.BuddyBids.Buddy1 = txtBuddy1.StringValue.Trim ();
						GlobalSettings.WBidINIContent.BuddyBids.Buddy2 = txtBuddy2.StringValue.Trim ();
						//save the state of the INI File
						WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

						NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
						CommonClass.Panel.OrderOut (this);
					
					}
				}
				this.View.Window.EndSheet (overlayPanel);
				NSApplication.SharedApplication.EndSheet (overlayPanel);
				overlayPanel.OrderOut (this.View.Window);
			});
		}
		private void LoadBidderDetails ()
		{
			if (GlobalSettings.ClearBuddyBid) {
				txtBuddy1.StringValue = txtBuddy2.StringValue = "0";
				lblBuddy1.StringValue = lblBuddy2.StringValue = "< No Matching Element >";
			} else {
				if (File.Exists (WBidHelper.GetAppDataPath () + "/falistwb4.dat")) {
					EmployeeList = (Dictionary<int, string>)WBidHelper.DeSerializeObject (WBidHelper.GetAppDataPath () + "/falistwb4.dat");
					txtBuddy1.StringValue = GlobalSettings.WBidINIContent.BuddyBids.Buddy1;
					txtBuddy2.StringValue = GlobalSettings.WBidINIContent.BuddyBids.Buddy2;

					var buddyname1 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == GlobalSettings.WBidINIContent.BuddyBids.Buddy1).Value;
					lblBuddy1.StringValue = (buddyname1 == string.Empty || buddyname1 == null) ? "< No Matching Element >" : buddyname1;
					var buddyname2 = EmployeeList.FirstOrDefault(x => x.Key.ToString() == GlobalSettings.WBidINIContent.BuddyBids.Buddy2).Value;
					lblBuddy2.StringValue = (buddyname2 == string.Empty || buddyname2 == null) ? "< No Matching Element >" : buddyname2;

				}
			}
		}

	}
}

