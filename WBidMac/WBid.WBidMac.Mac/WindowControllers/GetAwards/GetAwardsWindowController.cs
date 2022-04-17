
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
using WBid.WBidiPad.Core.Enum;
using WBid.WBidMac.Mac.WindowControllers.CustomAlert;
using WBid.WBidMac.Mac.ViewControllers.CustomAlertView;
using CoreGraphics;

namespace WBid.WBidMac.Mac
{
	public partial class GetAwardsWindowController : AppKit.NSWindowController
	{ 
		WBidDataDwonloadAuthServiceClient client;
		private string _sessionCredentials = string.Empty;
		NSObject notif;
		private DownloadInfo _downloadFileDetails;
		#region Constructors
		string[] domicileArray = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();
		NSPanel overlayPanel;
		OverlayViewController overlay;

		// Called when created from unmanaged code
		public GetAwardsWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public GetAwardsWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public GetAwardsWindowController () : base ("GetAwardsWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new GetAwardsWindow Window {
			get {
				return (GetAwardsWindow)base.Window;
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
					this.Window.OrderOut(this);
				};
				this.Window.WillClose += (object sender, EventArgs e) => {
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				
				btnCancel.Activated += (object sender, EventArgs e) => {
					this.Window.Close ();
					this.Window.OrderOut (this);
					NSApplication.SharedApplication.StopModal ();
				};
				
				SetupViews ();
				SetupButtons ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void SetupViews ()
		{
			try {
				btnDomicile.AddItems (domicileArray);
				//btnDomicile.SelectItem (GlobalSettings.WbidUserContent.UserInformation.Domicile);
				
				BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
				client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
				    
				client.GetAuthorizationforMultiPlatformCompleted += client_GetAuthorizationforMultiPlatformCompleted;
				// Perform any additional setup after loading the view, typically from a nib.
				
				//int indexToSelect = GlobalSettings.WBidINIContent.Domiciles.IndexOf(GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile));
				btnDomicile.SelectItem (GlobalSettings.CurrentBidDetails.Domicile);
				//pckrDomicilePick.Select(indexToSelect, 0, true);
				//domicileName = GlobalSettings.CurrentBidDetails.Domicile;
				if (DateTime.Now.Day <= 19)
					btnAwards.SelectCellWithTag (0);
				else
					btnAwards.SelectCellWithTag (1);
				
				if (GlobalSettings.CurrentBidDetails.Postion == "CP")
					btnPosition.SelectCellWithTag (0);
				else if (GlobalSettings.CurrentBidDetails.Postion == "FO")
					btnPosition.SelectCellWithTag (1);
				else if (GlobalSettings.CurrentBidDetails.Postion == "FA")
					btnPosition.SelectCellWithTag (2);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		void SetupButtons ()
		{
			try {
				btnPosition.Activated += (object sender, EventArgs e) => {
				
				};
				btnAwards.Activated += (object sender, EventArgs e) => {
				
				};
				btnRetrieve.Activated += (object sender, EventArgs e) => {
					LoginViewController login = new LoginViewController ();
					var panel = new NSPanel ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (450, 250));
					panel.ContentView = login.View;
					NSApplication.SharedApplication.BeginSheet (panel, this.Window);
					notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"LoginSuccess", HandleLoginSuccess);
				
				};
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleLoginSuccess (NSNotification obj)
		{
			try {
				if (obj.Object.ToString () == "Login") {
					NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
					overlayPanel = new NSPanel ();
					overlayPanel.SetContentSize (new CoreGraphics.CGSize (400, 120));
					overlay = new OverlayViewController ();
					overlay.OverlayText = "Retreiving Bid Award..";
					overlayPanel.ContentView = overlay.View;
					NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
				
					InitiateDownloadProcess (overlayPanel);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		/// <summary>
		/// start the download process ..
		/// </summary>
		public void InitiateDownloadProcess( NSPanel overlay)
		{
			try
			{
				
				_downloadFileDetails = new DownloadInfo();
				_downloadFileDetails.UserId = CommonClass.UserName;
				_downloadFileDetails.Password = CommonClass.Password;

				//checking  the internet connection available
				//==================================================================================================================
                if (Reachability.CheckVPSAvailable())
				{
					NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckSuccess", null);
					//checking CWA credential
					//==================================================================================================================

					//this.startProgress();
					Authentication authentication = new Authentication();
					string authResult = authentication.CheckCredential(_downloadFileDetails.UserId, _downloadFileDetails.Password);
					if (authResult.Contains("ERROR: "))
					{
						WBidLogEvent objlogs = new WBidLogEvent();
						objlogs.LogBadPasswordUsage(_downloadFileDetails.UserId,false, authResult);
						InvokeOnMainThread(() =>
							{
								//KeychainHelpers.SetPasswordForUsername ("pass", "", "WBid.WBidiPad.cwa", SecAccessible.Always, false);
								//UIAlertView alert = new UIAlertView("WBidMax", "Invalid Username or Password", null, "OK", null);
								//alert.Show();
								//overlay.RemoveFromSuperview();

								var panel = new NSPanel();
								var customAlert = new CustomAlertViewController();
								customAlert.AlertType = "InvalidCredential";
								customAlert.objAwardWindow = this;
								CommonClass.Panel = panel;
								panel.SetContentSize(new CGSize(430, 350));
								panel.ContentView = customAlert.View;
								NSApplication.SharedApplication.BeginSheet(panel, this.Window);

								//	var alert = new NSAlert ();
								//	alert.AlertStyle = NSAlertStyle.Warning;
								//	alert.Window.Title = "WBidMax";
								//	alert.MessageText = "Bid Award Download";
								//	alert.InformativeText = "Invalid Username or Password";
								//	alert.AddButton ("OK");
								//alert.Buttons[0].Activated += (object senderr, EventArgs ee) =>
								//{
								//	alert.Window.Close();
								//	//							menuCheckUpdate.Enabled = false;
								//	NSApplication.SharedApplication.StopModal();
								//	NSApplication.SharedApplication.EndSheet(overlayPanel);
								//	overlayPanel.OrderOut(this);
								//};
								//	alert.RunModal ();

							});
					}
					else if (authResult.Contains("Exception"))
					{
						InvokeOnMainThread(() =>
							{
								NSApplication.SharedApplication.EndSheet(overlayPanel);
								overlayPanel.OrderOut(this);
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Informational;
								alert.Window.Title = "WBidMax";
								alert.MessageText = "Bid Award Download";
								//alert.InformativeText = "The company server is down.  They have been notified.  We don’t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also";
								alert.InformativeText = "Your attempt to submit a bid or download bid data has failed. Specifically, the Southwest Airlines server did not respond with a certain time, and as a result, you received a Server Timeout.\n\nThis can happen for many reasons.  Our suggestion is to keep trying over the next 10 minutes or so, and if the app still fails to submit a bid or download bid data, we suggest the following:\n\nChange your internet connection.You can also try to use your cell phone as a hotspot for your internet connection \n\nFinally, send us an email if you are continuously having trouble.";

								alert.AddButton ("OK");
								alert.RunModal ();
								//UIAlertView alert = new UIAlertView("Warning", "The company server is down.  They have been notified.  We don’t know how long it could take to bring the server back on line.  Most of the time it is within10-20 minutes, but we have seen this server down for 6-7 hours also.", null, "OK", null);
								//alert.Show();
								//overlay.RemoveFromSuperview();
								//this.DismissViewController(true, null);
							});
					}
					else
					{
						NSNotificationCenter.DefaultCenter.PostNotificationName("cwaCheckSuccess", null);
						// this.startProgress();

						_sessionCredentials = authResult;

						ClientRequestModel clientRequestModel = new ClientRequestModel();
						clientRequestModel.Base = GlobalSettings.CurrentBidDetails.Domicile;
						clientRequestModel.BidRound = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
						clientRequestModel.Month = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1).ToString("MMM").ToUpper();
						clientRequestModel.Postion = GlobalSettings.CurrentBidDetails.Postion;
						clientRequestModel.OperatingSystem = CommonClass.OperatingSystem;
						clientRequestModel.Platform = CommonClass.Platform;
						clientRequestModel.RequestType = (int)RequestTypes.DownloadAward;
						clientRequestModel.Token = new Guid();
						clientRequestModel.Version = CommonClass.AppVersion;
						clientRequestModel.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
						client.GetAuthorizationforMultiPlatformAsync(clientRequestModel);
					}
				}
				else
				{
					InvokeOnMainThread(() =>
						{
                        string alertmessage = GlobalSettings.VPSDownAlert;
                            if (Reachability.IsSouthWestWifiOr2wire())
                            {
                                alertmessage = GlobalSettings.SouthWestConnectionAlert;
                            }
							NSApplication.SharedApplication.EndSheet(overlayPanel);
							overlayPanel.OrderOut(this);
							var alert = new NSAlert ();
							alert.AlertStyle = NSAlertStyle.Warning;
							alert.Window.Title = "WBidMax";
							alert.MessageText = "Bid Award Download";
                        alert.InformativeText = alertmessage;
							alert.AddButton ("OK");
							alert.RunModal ();

							//UIAlertView alert = new UIAlertView("WBidMax", "Connectivity not available", null, "OK", null);
							//alert.Show();
							//NSNotificationCenter.DefaultCenter.PostNotificationName("reachabilityCheckFailed", null);
							//this.DismissViewController(true, null);
						});
				}
			}
			catch (Exception ex)
			{



				InvokeOnMainThread(() =>
					{

						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					});
			}
			//}
		}
		/// <summary>
		/// Create the Filename for the Award file based on teh UI selection
		/// </summary>
		private void GenarateAwardFileName()
		{
			List<string> downLoadList = new List<string>();
			try
			{

				if (btnPosition.SelectedCell.Tag == 0)
				{
					string fileName = btnDomicile.SelectedItem.Title + "CP" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
					fileName = btnDomicile.SelectedItem.Title  + "FO" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
				}
				else if (btnPosition.SelectedCell.Tag  == 1)
				{
					string fileName = btnDomicile.SelectedItem.Title + "FO" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
					fileName = btnDomicile.SelectedItem.Title + "CP" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
				}
				else if (btnPosition.SelectedCell.Tag  == 2)
				{
					string fileName = btnDomicile.SelectedItem.Title + "FA" + ((btnAwards.SelectedCell.Tag == 0) ? "M" : "W") + ".TXT";
					downLoadList.Add(fileName);
				}
				else
				{
					return;
				}
				_downloadFileDetails.DownloadList = downLoadList;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		/// <summary>
		/// Donwload the awards and show it ot the file viewer.
		/// </summary>
		private void AwardDownlaod()
		{
			try {
				DownloadAward downloadAward = new DownloadAward ();
				List<DownloadedFileInfo> lstDownloadedFiles = downloadAward.DownloadAwardDetails (_downloadFileDetails);

				if (lstDownloadedFiles [0].IsError) {


					InvokeOnMainThread (() => {
						NSApplication.SharedApplication.EndSheet(overlayPanel);
						overlayPanel.OrderOut(this);
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Warning;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "Bid Award Download";
						alert.InformativeText = "The request data does not exist on the SWA  Servers. Make sure the proper month is  selected and  you are within the  normal timeframe for the request.";
						alert.AddButton ("OK");
						alert.RunModal ();

						//UIAlertView alert = new UIAlertView ("WBidMax", "The request data does not exist on the SWA  Servers. Make sure the proper month is  selected and  you are within the  normal timeframe for the request.", null, "OK", null);
						//alert.Show ();
						//overlay.RemoveFromSuperview ();

					});
				} else
				{


					foreach (DownloadedFileInfo fileinfo in lstDownloadedFiles) {
						FileStream fStream = new FileStream (Path.Combine (WBidHelper.GetAppDataPath (), fileinfo.FileName), FileMode.Create);
						fStream.Write (fileinfo.byteArray, 0, fileinfo.byteArray.Length);
						fStream.Dispose ();
					}
					bool isNeedtoShowAwardData = true;
                    var filename = lstDownloadedFiles[0].FileName;
                    if (filename.Substring(5, 1) == "M")
                    {
                        UserBidDetails biddetails = new UserBidDetails();
                        biddetails.Domicile = filename.Substring(0, 3);
                        biddetails.Position = filename.Substring(3, 2);
                        biddetails.Round = filename.Substring(5, 1) == "M" ? 1 : 2;
                        biddetails.Year = DateTime.Now.AddMonths(1).Year;
                        biddetails.Month = DateTime.Now.AddMonths(1).Month;
                        if (biddetails.Round == 1)
                        {

                            if (GlobalSettings.IsDifferentUser)
                            {
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(GlobalSettings.ModifiedEmployeeNumber.ToString().PadLeft(6, '0'), @"\d+").Value);
                            }
                            else
                            {
                                biddetails.EmployeeNumber = Convert.ToInt32(Regex.Match(_downloadFileDetails.UserId, @"\d+").Value);
                            }
                            string alertmessage = WBidHelper.GetAwardAlert(biddetails);
                            if (alertmessage != string.Empty)
                            {
                                alertmessage = alertmessage.Insert(0, "\n\n");
                                alertmessage += "\n\n";
                                InvokeOnMainThread(() =>
                                {

									var alert = new NSAlert();
									alert.Window.Title = "WBidMax";
									alert.MessageText = alertmessage;
									alert.AddButton("Ok");
									
									alert.Buttons[0].Activated += delegate
									{
										isNeedtoShowAwardData = false;
										alert.Window.Close();
										NSApplication.SharedApplication.StopModal();

										NSApplication.SharedApplication.EndSheet(overlayPanel);
										overlayPanel.OrderOut(this);
										var fileViewer = new FileWindowController();
										fileViewer.Window.Title = "Bid Awards";
										fileViewer.LoadTXT(lstDownloadedFiles[0].FileName);
										fileViewer.Window.MakeKeyAndOrderFront(this);
										NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
										this.Window.Close();
									};
									
									alert.RunModal();


									

                                });
                            }

                        }
                    }
					if (isNeedtoShowAwardData)
					{
						InvokeOnMainThread(() =>
						{
							NSApplication.SharedApplication.EndSheet(overlayPanel);
							overlayPanel.OrderOut(this);
							var fileViewer = new FileWindowController();
							fileViewer.Window.Title = "Bid Awards";
							fileViewer.LoadTXT(lstDownloadedFiles[0].FileName);
							fileViewer.Window.MakeKeyAndOrderFront(this);
							NSApplication.SharedApplication.RunModalForWindow(fileViewer.Window);
							//CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
							//fileViewer.Window.MakeKeyAndOrderFront (this);
							this.Window.Close();

							//webPrint fileViewer = new webPrint ();
							//this.PresentViewController (fileViewer, true, () => {
							//fileViewer.loadFileFromUrl (lstDownloadedFiles [0].FileName);
							//});
						});
					}
				}
				InvokeOnMainThread (() => {
					DismissCurrentView();
					//Goooverlay.RemoveFromSuperview ();

				});
			} catch (Exception ex) {

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		#region WCF CompletedEvent
		private void client_GetAuthorizationforMultiPlatformCompleted(object sender, GetAuthorizationforMultiPlatformCompletedEventArgs e)
		{
			try
			{

				if (e.Result != null)
				{
					ServiceResponseModel serviceResponseModel = e.Result;

					if (serviceResponseModel.IsAuthorized)
					{
						NSNotificationCenter.DefaultCenter.PostNotificationName("authCheckSuccess", null);
						//this.startProgress();
						_downloadFileDetails.SessionCredentials = _sessionCredentials;
						InvokeOnMainThread(() =>
							{
								GenarateAwardFileName();
							});
						AwardDownlaod();
					}
					else
					{
						InvokeOnMainThread(() =>
							{
								NSApplication.SharedApplication.EndSheet(overlayPanel);
								overlayPanel.OrderOut(this);
								var alert = new NSAlert ();
								alert.AlertStyle = NSAlertStyle.Warning;
								alert.Window.Title = "WBidMax";
								alert.MessageText = "Bid Award Download";
								alert.InformativeText = serviceResponseModel.Message;
								alert.AddButton ("OK");
								alert.RunModal ();
								
								//UIAlertView alert = new UIAlertView("Error", serviceResponseModel.Message, null, "OK", null);
								//alert.Show();
								//overlay.RemoveFromSuperview();

							});
						DismissCurrentView();
					}
				}
			}
			catch (Exception ex)
			{

				InvokeOnMainThread(() =>
					{

						CommonClass.AppDelegate.ErrorLog (ex);
						CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
					});
			}
		}
		#endregion

		public void DismissCurrentView()
		{
			this.Window.Close();
			this.Window.OrderOut(this);
			//			CommonClass.DownloadController.Window.ResignKeyWindow ();
			NSApplication.SharedApplication.StopModal();
		}
	}
}

