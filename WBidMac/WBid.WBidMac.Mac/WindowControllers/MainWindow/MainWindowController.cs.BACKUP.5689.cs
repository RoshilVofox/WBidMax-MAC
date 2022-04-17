
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

//using System;
//using System.Drawing;
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
using System.IO;
//using MonoTouch.EventKit;
//using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary;
using VacationCorrection;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.IO.Compression;
using System.Net;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Model.State.Weights;
using WBid.WBidiPad.SharedLibrary.Parser;
using System.Globalization;
using System.Runtime.Serialization.Json;
using  System.Text;

namespace WBid.WBidMac.Mac
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		SummaryViewController summaryVC;
		BidLineViewController bidlineVC;
		ModernViewController modernVC;
		CalendarWindowController calendarWC;
		WBidState wBIdStateContent;
		CSWWindowController cswWC;

		NSPanel overlayPanel;
		OverlayViewController overlay;
		NSObject notif;
		Dictionary<string, Trip> trips = null;
		Dictionary<string, Line> lines = null;
		NSObject confNotif;
		bool FirstTime;
		bool SynchBtn;
		int isNeedToClose;
		public System.Timers.Timer timer;
		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();

		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
		static NSButton closeButton;

		public override  void KeyUp(NSEvent theEvent)
		{

		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			this.Window.IsZoomed = true;
			this.ShouldCascadeWindows = false;
			isNeedToClose = 0;
			//this.Window.KeyUp +=KeyUp ;
			GlobalSettings.RedoStack = new List<WBidState> ();
			GlobalSettings.UndoStack = new List<WBidState> ();
			try {
				btnRedo.Activated += (sender, e) => {
					RedoOperation ();
				};
				btnUndo.Activated += (sender, e) => {
					UndoOperation ();
				};
				closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
				closeButton.Activated += (sender, e) => {
					if (GlobalSettings.isModified) {
						var alert = new NSAlert ();
						alert.Window.Title = "WBidMax";
						alert.MessageText = "Save your Changes?";
						//alert.InformativeText = "There are no Latest News available..!";
						alert.AddButton ("Save & Exit");
						alert.AddButton ("Exit");
						alert.AddButton ("Cancel");
						alert.Buttons [0].Activated += delegate {
							// save and exit
							StateManagement stateManagement = new StateManagement ();
							stateManagement.UpdateWBidStateContent ();
							if (GlobalSettings.isModified) {
								GlobalSettings.WBidStateCollection.IsModified = true;
								WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
							}
							//ExitApp ();
							alert.Window.Close ();
							NSApplication.SharedApplication.StopModal ();
							isNeedToClose = 2;
							CheckSmartSync();
						};
						alert.Buttons [1].Activated += delegate {
							ExitApp ();	
							alert.Window.Close ();
							NSApplication.SharedApplication.StopModal ();
						};
						alert.RunModal ();
					} else {
						//ExitApp ();
						isNeedToClose = 2;
						CheckSmartSync();
					}

				};
				this.Window.Title = WBidCollection.SetTitile ();
				SetPropertyNames ();
				setViews ();
				CommonClass.AppDelegate.ReloadMenu ();
				txtGoToLine.Activated += HandleGoToLine;
				SetupAdminView ();
				wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				UpdateSaveButton (false);

				SetVacButtonStates ();

				if (btnEOM.State == NSCellStateValue.On || btnVacation.State == NSCellStateValue.On || btnDrop.State == NSCellStateValue.On) {
					BeginInvokeOnMainThread (() => {
						applyVacation ();
					});
				}
				UpdateUndoRedoButtons();
				if (GlobalSettings.WBidINIContent.User.AutoSave)
				{
					AutoSave();
				}
				BeginInvokeOnMainThread(() =>
					{
						FirstTime = true;
						Synch();
					});

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void ExitApp ()
		{
			CloseAllChildWindows ();
			this.Window.Close ();
			this.Window.OrderOut (this);
		}
		public void UpdateAutoSave ()
		{
			if (GlobalSettings.WBidINIContent.User.AutoSave)
			{
				if (timer != null)
					timer.Stop();
				AutoSave();
			}
			else
			{
				if (timer != null)
					timer.Stop();
			}
		}

		/// <summary>
		/// This will save the current bid state automatically dependes on the Settings in the Configuration=>user tab
		/// </summary>
		public void AutoSave()
		{
			if (GlobalSettings.WBidINIContent.User.AutoSave)
			{
				timer = new System.Timers.Timer(GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000)
				{
					Interval = GlobalSettings.WBidINIContent.User.AutoSavevalue * 60000,
					Enabled = true
				};
				timer.Elapsed += timer_Elapsed;
			}
		}
		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			StateManagement stateManagement = new StateManagement();
			stateManagement.UpdateWBidStateContent();
			GlobalSettings.WBidStateCollection.IsModified = true;
			WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);
			//save the state of the INI File
			WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
			GlobalSettings.isModified = false;
			InvokeOnMainThread (() => {

<<<<<<< HEAD
				UpdateSaveButton();

=======
				UpdateSaveButton(false);
				UpdateUndoRedoButtons();
				//if (CommonClass.lineVC != null)
				//	CommonClass.lineVC.UpdateSaveButton ();
				//if (CommonClass.cswVC != null)
				//	CommonClass.cswVC.UpdateSaveButton ();
>>>>>>> 1bab1c4730a0b419cde7f753f9346aa5ef0d8329
			});
		}
		private void ShowMessageBox(string title,string content)
		{
			var alert = new NSAlert ();

			alert.MessageText = title;
			alert.InformativeText = content;
			alert.RunModal ();
		}

		public bool IsSynchStart;
		public int SynchStateVersion { get; set; }
		public DateTime ServerSynchTime { get; set; }
		private void CheckSmartSync()
		{
			if (GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch) {

				var alert = new NSAlert ();
				alert.Window.Title = "Smart Sync";
				alert.MessageText = "Do you want to sync local changes with Server?";
				//alert.InformativeText = "There are no Latest News available..!";
				alert.AddButton ("Yes");
				alert.AddButton ("No");
				//alert.AddButton ("Cancel");
				alert.Buttons [1].Activated += delegate {
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
					if (isNeedToClose == 1)
						GoToHomeWindow ();
					else if (isNeedToClose == 2)
						ExitApp ();				
				};
				alert.Buttons [0].Activated += delegate {
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
					//isNeedToClose=true;
					SynchState ();
				};
				alert.RunModal ();
//				UIAlertView syAlert = new UIAlertView("Smart Sync", "Do you want to sync local changes with Server?", null, "NO", new string[] { "YES" });
//				syAlert.Show();
//				syAlert.Dismissed += (object sender, UIButtonEventArgs e) =>
//				{
//					if (e.ButtonIndex == 0)
//					{
				//						GoToHomeWindow ();
//					}
//					else
//					{
//						SynchState();
//					}
//				};
			} else {
				if (isNeedToClose == 1)
					GoToHomeWindow ();
				else if (isNeedToClose == 2)
					ExitApp ();				
			}
		}
		private void Synch()
		{
			if ( GlobalSettings.WBidStateCollection != null && GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.User.SmartSynch)
			{
				//syncOverlay = new LoadingOverlay(View.Bounds, "Smart Synchronisation checking server version..\n Please wait..");
				//View.Add(syncOverlay);

				overlayPanel = new NSPanel ();
				overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
				overlay = new OverlayViewController ();
				overlay.OverlayText = "Smart Synchronisation checking server version..\n Please wait..";
				overlayPanel.ContentView = overlay.View;
				NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

				BeginInvokeOnMainThread(() =>
					{
						SynchStateForApplicationLoad();
					});
			}
		}

		private void SynchStateForApplicationLoad()
		{

			try {
				// MessageBoxResult msgResult;
				bool isConnectionAvailable = Reachability.IsHostReachable (GlobalSettings.ServerUrl);

				if (isConnectionAvailable) {

					IsSynchStart = true;
					string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
					SynchStateVersion = int.Parse (GlobalSettings.WBidStateCollection.SyncVersion);


					//Get server State Version
					VersionInfo versionInfo = GetServerVersion (stateFileName);
					//syncOverlay.Hide();
					NSApplication.SharedApplication.EndSheet (overlayPanel);
					overlayPanel.OrderOut (this);

					if (versionInfo != null) {
						ServerSynchTime = DateTime.Parse (versionInfo.LastUpdatedDate, CultureInfo.InvariantCulture);

						if (versionInfo.Version != string.Empty) {
							int serverVersion = Convert.ToInt32 (versionInfo.Version);

							if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified) {
								//conflict
								confNotif = NSNotificationCenter.DefaultCenter.AddObserver ("SyncConflict", (NSNotification notification) => {
									string str = notification.Object.ToString ();
									NSNotificationCenter.DefaultCenter.RemoveObserver (confNotif);
									BeginInvokeOnMainThread (() => {
										if (str == "server") {
											FirstTime = true;
//											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State FROM server \n Please wait..");
//											View.Add (syncOverlay);
											overlayPanel = new NSPanel ();
											overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
											overlay = new OverlayViewController ();
											overlay.OverlayText = "Synching current State FROM server \n Please wait..";
											overlayPanel.ContentView = overlay.View;
											NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

											WBidHelper.PushToUndoStack ();
											UpdateUndoRedoButtons ();
											BeginInvokeOnMainThread (() => {
												GetStateFromServer (stateFileName);
											});
										} else if (str == "local") {
											FirstTime = true;
//											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State TO server \n Please wait..");
//											View.Add (syncOverlay);
											overlayPanel = new NSPanel ();
											overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
											overlay = new OverlayViewController ();
											overlay.OverlayText = "Synching current State TO server \n Please wait..";
											overlayPanel.ContentView = overlay.View;
											NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

											BeginInvokeOnMainThread (() => {
												UploadLocalVersionToServer (stateFileName);
											});
										} else {
											IsSynchStart = false;
										}							
//										CloseAllChildWindows ();
//										this.Window.Close ();
//										this.Window.OrderOut (this);	
										
									});
								});
//								SynchConflictViewController synchConf = new SynchConflictViewController ();
//								synchConf.serverSynchTime = ServerSynchTime;
//								if (serverVersion == 0)
//									synchConf.noServer = true;
//								synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
//								this.PresentViewController (synchConf, true, null);
								InvokeOnMainThread(()=>{
								var panel = new NSPanel ();
								var synchConf = new SynchConflictViewController ();
								synchConf.serverSynchTime = ServerSynchTime;
								if (serverVersion == 0)
									synchConf.noServer = true;
								CommonClass.Panel = panel;
								panel.SetContentSize (new System.Drawing.SizeF (450, 285));
								panel.ContentView = synchConf.View;
								NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
								NSApplication.SharedApplication.RunModalForWindow(panel);
								});
							
							} else if (SynchBtn) {
								SynchBtn = false;
								//UIAlertView syAlert = new UIAlertView ("Smart Sync", "Your App is already synchronized with the server..", null, "OK", null);
								//syAlert.Show ();
								ShowMessageBox("Smart Sync", "Your App is already synchronized with the server..");
							}

						}
					} else {
						InvokeOnMainThread (() => {
//							UIAlertView syAlert = new UIAlertView ("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.", null, "OK", null);
//							syAlert.Show ();
							ShowMessageBox("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.");
						});
					}

				} else {
					InvokeOnMainThread (() => {
//						UIAlertView syAlert = new UIAlertView ("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
//						syAlert.Show ();
//						syncOverlay.Hide ();
						ShowMessageBox("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.");
						NSApplication.SharedApplication.EndSheet (overlayPanel);
						overlayPanel.OrderOut (this);

					});

					// base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);


				}
			} catch (Exception ex) {
				throw ex;
			}
		}

		private void GetStateFromServer(string stateFileName)
		{
			bool failed = false;
			try {
				string url = GlobalSettings.synchServiceUrl + "GetWBidStateFromServer/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;


				HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
				request.Timeout = 30000;
				HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
				var stream = response.GetResponseStream ();
				var reader = new StreamReader (stream);
				StateSync stateSync = SmartSyncLogic.ConvertJsonToObject<StateSync> (reader.ReadToEnd ());
				WBidStateCollection wBidStateCollection = null;
				if (stateSync != null) {
					// byte[] byteArr = Convert.FromBase64String(stateSync.StateContent);

					wBidStateCollection = SmartSyncLogic.ConvertJsonToObject<WBidStateCollection> (stateSync.StateContent);
					//DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WBidStateCollection));
					//using (var ms = new MemoryStream(byteArr))
					//{
					//    wBidStateCollection = (WBidStateCollection)serializer.ReadObject(ms);
					//}

//					foreach(var vac in wBidStateCollection.Vacation) {
//						vac.StartDate = DateTime.Parse(vac.StartDate).ToShortDateString();
//						vac.EndDate = DateTime.Parse(vac.EndDate).ToShortDateString();
//					}

					GlobalSettings.WBidStateCollection = wBidStateCollection;
					//GlobalSettings.WBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName); ;
					GlobalSettings.WBidStateCollection.SyncVersion = stateSync.VersionNumber.ToString ();
					GlobalSettings.WBidStateCollection.StateUpdatedTime = stateSync.LastUpdatedTime;
					GlobalSettings.WBidStateCollection.IsModified = false;
					//string stateFilePath = Path.Combine (WBidHelper.GetAppDataPath (), stateFileName + ".WBS");
					//WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);

					//ConvertVacationDateFormat();

					WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);

					//System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
					//{


					//    //RebindStateFromStatefile();
					//    //Lines = new ObservableCollection<Line>(Lines);
					//    IsSynchStart = false;
					//    UIAlertView syAlert = new UIAlertView("Smart Sync", "Successfully synchronized your computer with the server...", null, "NO", null);
					//    syAlert.Show();


					//}));

				}
				InvokeOnMainThread (() => {
					//UIAlertView syAlert = new UIAlertView ("Smart Sync", "Successfully Synchronized  your computer with the server.", null, "OK", null);
					//syAlert.Show ();
//						syncOverlay.Hide();
					NSApplication.SharedApplication.EndSheet (overlayPanel);
					overlayPanel.OrderOut (this);


					if (isNeedToClose==0) {
						GlobalSettings.Lines.ToList ().ForEach (x => {
							x.ConstraintPoints.Reset ();
							x.Constrained = false;
							x.WeightPoints.Reset ();
							x.TotWeight = 0.0m;
						});
						StateManagement statemanagement = new StateManagement ();
						statemanagement.ReloadDataFromStateFile ();
						//loadSummaryListAndHeader();
						//ReloadLineView ();
						//NSNotificationCenter.DefaultCenter.PostNotificationName ("DataReload", null);
						CommonClass.ViewChanged = true;
						ReloadAllContent();
						CommonClass.ViewChanged = false;
						if (CommonClass.CSWController != null) {
							CommonClass.CSWController.ReloadAllContent ();
						}
						ShowMessageBox("Smart Sync", "Successfully Synchronized  your computer with the server.");

					} else {
//						syAlert.Dismissed += (object sender, UIButtonEventArgs e) => {
//							if (e.ButtonIndex == 0)
//								GoToHomeWindow(); //GoToHome ();
						//};
						var alert = new NSAlert ();
						alert.Window.Title = "Smart Sync";
						alert.MessageText = "Successfully Synchronized  your computer with the server.";
						//alert.InformativeText = "There are no Latest News available..!";
						alert.AddButton ("OK");
						//alert.AddButton ("Cancel");
						alert.Buttons [0].Activated += delegate {
							alert.Window.Close();
							NSApplication.SharedApplication.StopModal();
							if(isNeedToClose==1)
								GoToHomeWindow();
							else if(isNeedToClose==2)
								ExitApp();
						};
						alert.RunModal ();
					}
					FirstTime = false;
				});
			} catch (Exception ex) {
				//System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
				//{
				//    SmartSynchMessage = "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.";
				//    base.SendNotificationMessage(WBidMessages.MainVM_Notofication_ShowSmartSyncConfirmationWindow);
				//    IsSynchStart = false;
				//}));

				FirstTime = false;
				failed = true;
				//throw  ex;
			}

			if (failed) {
				InvokeOnMainThread (() => {
//					syncOverlay.Hide ();
					NSApplication.SharedApplication.EndSheet (overlayPanel);
					overlayPanel.OrderOut (this);
					ShowMessageBox("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");
//					UIAlertView syAlert = new UIAlertView ("Smart Sync", "The server to get the previous state file is not responding.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
//					syAlert.Show ();
				});

			}

		}

		private void ConvertVacationDateFormat()
		{
			if (GlobalSettings.WBidStateCollection.Vacation.Count () > 0) {
				for (int count = 0; count < GlobalSettings.WBidStateCollection.Vacation.Count (); count++) {
					//  GlobalSettings.WBidStateCollection.Vacation[count].StartDate = DateTime.Parse(GlobalSettings.WBidStateCollection.Vacation[count].StartDate).ToShortDateString();
					// GlobalSettings.WBidStateCollection.Vacation[count].EndDate = DateTime.Parse(GlobalSettings.WBidStateCollection.Vacation[count].EndDate).ToShortDateString();
					if (GlobalSettings.WBidStateCollection.Vacation [count].StartDate.Contains ("/")) {
						string[] split = GlobalSettings.WBidStateCollection.Vacation [count].StartDate.Split ('/');
						GlobalSettings.WBidStateCollection.Vacation [count].StartDate = new DateTime (int.Parse (split [2]), int.Parse (split [0]), int.Parse (split [1])).ToShortDateString ();
						split = GlobalSettings.WBidStateCollection.Vacation [count].EndDate.Split ('/');
						GlobalSettings.WBidStateCollection.Vacation [count].EndDate = new DateTime (int.Parse (split [2]), int.Parse (split [0]), int.Parse (split [1])).ToShortDateString ();

					}
				}
			}
		}

		public static T ConvertJSonToObject<T>(string jsonString)
		{
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
			T obj = (T)serializer.ReadObject(ms);
			return obj;
		}

		private void SynchState()
		{
			try {
				bool isConnectionAvailable = Reachability.IsHostReachable (GlobalSettings.ServerUrl);
				if (isConnectionAvailable) {
					//new thread?
					IsSynchStart = true;

					string stateFileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
					SynchStateVersion = int.Parse (GlobalSettings.WBidStateCollection.SyncVersion);
					//Get server State Version
					VersionInfo versionInfo = GetServerVersion (stateFileName);
					if (versionInfo != null) {
						ServerSynchTime = DateTime.Parse (versionInfo.LastUpdatedDate, CultureInfo.InvariantCulture);

						if (versionInfo.Version != string.Empty) {
							int serverVersion = Convert.ToInt32 (versionInfo.Version);

							if (SynchStateVersion != serverVersion || GlobalSettings.WBidStateCollection.IsModified) {
								//conflict
								confNotif = NSNotificationCenter.DefaultCenter.AddObserver ("SyncConflict", (NSNotification notification) => {
									string str = notification.Object.ToString ();
									NSNotificationCenter.DefaultCenter.RemoveObserver (confNotif);
									BeginInvokeOnMainThread (() => {
										if (str == "server") {
//											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State FROM server \n Please wait..");
//											View.Add (syncOverlay);
											overlayPanel = new NSPanel ();
											overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
											overlay = new OverlayViewController ();
											overlay.OverlayText = "Synching current State FROM server \n Please wait..";
											overlayPanel.ContentView = overlay.View;
											NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

											BeginInvokeOnMainThread (() => {
												GetStateFromServer (stateFileName);
											}
											);
										} else if (str == "local") {
//											syncOverlay = new LoadingOverlay (View.Bounds, "Synching current State TO server \n Please wait..");
//											View.Add (syncOverlay);
											overlayPanel = new NSPanel ();
											overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
											overlay = new OverlayViewController ();
											overlay.OverlayText = "Synching current State TO server \n Please wait..";
											overlayPanel.ContentView = overlay.View;
											NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

											BeginInvokeOnMainThread (() => {
												UploadLocalVersionToServer (stateFileName);
											});
										} else {
											IsSynchStart = false;
											GoToHomeWindow(); //GoToHome ();
										}
										//GoToHome ();
									});
								});
//								SynchConflictViewController synchConf = new SynchConflictViewController ();
//								synchConf.serverSynchTime = ServerSynchTime;
//								if (serverVersion == 0)
//									synchConf.noServer = true;
//								synchConf.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
//								this.PresentViewController (synchConf, true, null);
							
								var panel = new NSPanel ();
								var synchConf = new SynchConflictViewController ();
								synchConf.serverSynchTime = ServerSynchTime;
								if (serverVersion == 0)
									synchConf.noServer = true;
								CommonClass.Panel = panel;
								panel.SetContentSize (new System.Drawing.SizeF (450, 285));
								panel.ContentView = synchConf.View;
								NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
								NSApplication.SharedApplication.RunModalForWindow(panel);
							
							}


						}
					} else {
						InvokeOnMainThread (() => {
							//UIAlertView syAlert = new UIAlertView ("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.", null, "OK", null);
							//syAlert.Show ();
							ShowMessageBox("Smart Sync", "The WBid Synch server is not responding.  You can work on this bid and attempt to synch at a later time.");
						});
					}
				} else {
					InvokeOnMainThread (() => {
						//UIAlertView syAlert = new UIAlertView ("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
						//syAlert.Show ();
						ShowMessageBox("Smart Sync", "You do not have an internet connection.  You can work on this bid offline, but your state file for this bid may become unsynchronized from a previous state.");
					});
				}
			} catch (Exception ex) {
				throw ex;
			}

		}

		private void UploadLocalVersionToServer(string stateFileName)
		{
			int version = int.Parse(SaveStateToServer(stateFileName));
			if (version != -1)
			{
				GlobalSettings.WBidStateCollection.SyncVersion = version.ToString();
				GlobalSettings.WBidStateCollection.StateUpdatedTime = DateTime.Now.ToUniversalTime();
				GlobalSettings.WBidStateCollection.IsModified = false;
				string stateFilePath = Path.Combine(WBidHelper.GetAppDataPath(), stateFileName + ".WBS");
				//WBidCollection.SaveStateFile(GlobalSettings.WBidStateCollection, stateFilePath);
				WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

				IsSynchStart = false;
				InvokeOnMainThread(() =>
					{
						//UIAlertView syAlert = new UIAlertView("Smart Sync", "Successfully Synchronized  your computer with the server.", null, "OK", null);
						//syAlert.Show();
						//syncOverlay.Hide();
						NSApplication.SharedApplication.EndSheet (overlayPanel);
						overlayPanel.OrderOut (this);

//						if (FirstTime)
//							ReloadAllContent();
//							//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);//loadSummaryListAndHeader();
//						else
//						{
////							syAlert.Dismissed += (object sender, UIButtonEventArgs e) =>
////							{
////								if (e.ButtonIndex == 0)
////									GoToHomeWindow();//GoToHome();
////							};
//						}
						if(isNeedToClose==0)
							ShowMessageBox("Smart Sync", "Successfully Synchronized  your computer with the server.");
						else{
							var alert = new NSAlert ();
							alert.Window.Title = "Smart Sync";
							alert.MessageText = "Successfully Synchronized  your computer with the server.";
							//alert.InformativeText = "There are no Latest News available..!";
							alert.AddButton ("OK");
							//alert.AddButton ("Cancel");
							alert.Buttons [0].Activated += delegate {
								alert.Window.Close();
								NSApplication.SharedApplication.StopModal();
								if(isNeedToClose==1)
									GoToHomeWindow();
								else if(isNeedToClose==2)
									ExitApp();
							};
							alert.RunModal ();

						}

						FirstTime = false;
					});
			}
			else
			{
				InvokeOnMainThread(() =>
					{
//						UIAlertView syAlert = new UIAlertView("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.", null, "OK", null);
//						syAlert.Show();
						//syncOverlay.Hide();
						NSApplication.SharedApplication.EndSheet (overlayPanel);
						overlayPanel.OrderOut (this);
						ShowMessageBox("Smart Sync", "An error occured while synchronizing your state to the server.  You can work on this bid, but your state file for this bid may become unsynchronized from a previous state.");

					});
			}
		}
//
		private VersionInfo GetServerVersion(string stateFileName)
		{
			VersionInfo versionInfo = null;
			try
			{
				if (!GlobalSettings.WBidINIContent.User.SmartSynch)
					return versionInfo;
				string url = GlobalSettings.synchServiceUrl + "GetServerStateVersionNumber/" + GlobalSettings.WbidUserContent.UserInformation.EmpNo + "/" + stateFileName + "/" + GlobalSettings.CurrentBidDetails.Year;
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				request.Timeout = 30000;
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				var stream = response.GetResponseStream();
				var reader = new StreamReader(stream);
				versionInfo = ConvertJSonToObject<VersionInfo>(reader.ReadToEnd());
				versionInfo.Version = versionInfo.Version.Trim('"');
				return versionInfo;
			}
			catch (Exception ex)
			{
				versionInfo = null;
				IsSynchStart = false;
				return versionInfo;
				//throw ex;
			}
		}
//
		private string SaveStateToServer(string stateFileName)
		{
			try
			{
				string url = GlobalSettings.synchServiceUrl + "SaveWBidStateToServer/";
				WBidStateCollection wBidStateCollection = GlobalSettings.WBidStateCollection;

				foreach (var item in wBidStateCollection.StateList)
				{
					if (item.FAEOMStartDate == DateTime.MinValue)
					{
						item.FAEOMStartDate = DateTime.MinValue.ToUniversalTime();
					}

				}

				string data = string.Empty;
				StateSync stateSync = new StateSync();
				stateSync.EmployeeNumber = GlobalSettings.WbidUserContent.UserInformation.EmpNo;
				stateSync.StateFileName = stateFileName;
				stateSync.VersionNumber = 0;
				stateSync.Year = GlobalSettings.CurrentBidDetails.Year;
				stateSync.StateContent = SmartSyncLogic.JsonObjectToStringSerializer<WBidStateCollection>(wBidStateCollection);
				stateSync.LastUpdatedTime = DateTime.MinValue.ToUniversalTime();

				var request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				//data = SmartSyncLogic.JsonSerializer(stateSync);

				data = SmartSyncLogic.JsonObjectToStringSerializer<StateSync>(stateSync);
				var bytes = Encoding.UTF8.GetBytes(data);
				request.ContentLength = bytes.Length;
				request.GetRequestStream().Write(bytes, 0, bytes.Length);
				request.Timeout = 30000;
				//Response
				var response = (HttpWebResponse)request.GetResponse();
				var stream = response.GetResponseStream();
				if (stream == null) return string.Empty;

				var reader = new StreamReader(stream);
				string result = reader.ReadToEnd();

				return result.Trim('"');
			}
			catch (Exception ex)
			{
				IsSynchStart = false;
				return "-1";
			}
		}


		static void SetPropertyNames ()
		{
			if (GlobalSettings.MenuBarButtonStatus == null)
				GlobalSettings.MenuBarButtonStatus = new MenuBarButtonStatus ();
			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) {
				CommonClass.modernProperties = new List<string> () {
					"TotPay",
					"VacPay",
					"FlyPay",
					"Off",
					"+Off"
				};
//				if (GlobalSettings.ModernAdditionalColumns.Any (x => x.DisplayName == "Pay"))
//					GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.DisplayName == "Pay").DisplayName = "TotPay";
			}
			else {
				CommonClass.modernProperties = new List<string> () {
					"Pay",
					"PDiem",
					"Flt",
					"Off",
					"+Off"
				};
//				if (GlobalSettings.ModernAdditionalColumns.Any (x => x.DisplayName == "TotPay"))
//					GlobalSettings.ModernAdditionalColumns.FirstOrDefault (x => x.DisplayName == "TotPay").DisplayName = "Pay";
			}
		}
		void RedoOperation()
		{
			if (GlobalSettings.RedoStack.Count > 0)
			{
				var state = GlobalSettings.RedoStack[0];
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();

				var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == state.StateName);

				if (stateContent != null)
				{
					GlobalSettings.UndoStack.Insert(0, new WBidState(stateContent));
					GlobalSettings.WBidStateCollection.StateList.Remove(stateContent);
					GlobalSettings.WBidStateCollection.StateList.Insert(0, new WBidState(state));

				}

				GlobalSettings.RedoStack.RemoveAt(0);
				//   StateManagement stateManagement = new StateManagement();
				stateManagement.ReloadDataFromStateFile();
				//ReloadLineView ();
				ReloadAllContent ();
				if (CommonClass.CSWController != null) {
					CommonClass.CSWController.ReloadAllContent ();
				}
			}

			GlobalSettings.isUndo = false;
			GlobalSettings.isRedo = true;
			UpdateUndoRedoButtons();
			GlobalSettings.isModified = true;
			btnSave.Enabled = GlobalSettings.isModified;

		}

		void UndoOperation()
		{
			if (GlobalSettings.UndoStack.Count > 0)
			{
				WBidState state = GlobalSettings.UndoStack[0];
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();

				var stateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == state.StateName);
			

				if (stateContent != null)
				{
					GlobalSettings.RedoStack.Insert(0, new WBidState(stateContent));
					GlobalSettings.WBidStateCollection.StateList.Remove(stateContent);
					GlobalSettings.WBidStateCollection.StateList.Insert(0, new WBidState(state));

				}


				GlobalSettings.UndoStack.RemoveAt(0);
					stateManagement.ReloadDataFromStateFile();
				//ReloadLineView ();
				ReloadAllContent ();
				if (CommonClass.CSWController != null) {
					CommonClass.CSWController.ReloadAllContent ();
				}

			}

			GlobalSettings.isUndo = true;
			GlobalSettings.isRedo = false;
			UpdateUndoRedoButtons();
			GlobalSettings.isModified = true;
			btnSave.Enabled = GlobalSettings.isModified;
		}

		void UpdateUndoRedoButtons()
		{
			btnUndo.Title = GlobalSettings.UndoStack.Count.ToString ();
			btnRedo.Title = GlobalSettings.RedoStack.Count.ToString ();

//			if (GlobalSettings.isUndo)
//				btnUndo.Image = NSImage.ImageNamed ("undoOrange.png");
//			else
//				btnUndo.Image = NSImage.ImageNamed ("undoGreen.png");
//
//			if (GlobalSettings.isRedo)
//				btnRedo.Image = NSImage.ImageNamed ("redoOrange.png");
//			else
//				btnRedo.Image = NSImage.ImageNamed ("redoGreen.png");

			if (GlobalSettings.UndoStack.Count == 0)
			{
//				btnUndo.Image = NSImage.ImageNamed ("undoGreen.png");
				btnUndo.Title = string.Empty;
				btnUndo.Enabled = false;
			}
			else
			{
				btnUndo.Title = GlobalSettings.UndoStack.Count.ToString ();
				btnUndo.Enabled = true;
			}

			if (GlobalSettings.RedoStack.Count == 0)
			{
//				btnRedo.Image = NSImage.ImageNamed ("redoGreen.png");
				btnRedo.Title = string.Empty;
				btnRedo.Enabled = false;
			}
			else
			{
				btnRedo.Title = GlobalSettings.RedoStack.Count.ToString ();
				btnRedo.Enabled = true;
			}

			GlobalSettings.isUndo = false;
			GlobalSettings.isRedo = false;
		}
		public void UpdateSaveButton (bool value)
		{
			try {
				GlobalSettings.isModified = value;
				btnSave.Enabled = GlobalSettings.isModified;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		void SetupAdminView ()
		{
			//txtUser.Enabled = false;
			btnReparse.Activated += HandleReparse;
			txtUser.StringValue = GlobalSettings.ModifiedEmployeeNumber??string.Empty;
			swUser.State = GlobalSettings.IsDifferentUser ? NSCellStateValue.On : NSCellStateValue.Off;
			swUser.Activated += (object sender, EventArgs e) => {
				//txtUser.Enabled = (swUser.State == NSCellStateValue.On);
				GlobalSettings.IsDifferentUser = (swUser.State == NSCellStateValue.On);
				//GlobalSettings.ModifiedEmployeeNumber =txtUser.StringValue;

			};
			txtUser.Changed+= (object sender, EventArgs e) => {
				GlobalSettings.ModifiedEmployeeNumber =txtUser.StringValue;
			};

		}
		public void CloseAllChildWindows ()
		{
			if (this.Window.ChildWindows != null) {
				foreach (var item in this.Window.ChildWindows) {
					item.Close ();
				}
			}
		}

		void HandleGoToLine (object sender, EventArgs e)
		{
			try {
				if (txtGoToLine.StringValue != string.Empty) {
					var num = int.Parse (txtGoToLine.StringValue);
					if (GlobalSettings.Lines.Any (x => x.LineNum == num)) {
						var line = GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == num);
						if (sgViewSelect.SelectedSegment == 0)
							CommonClass.SummaryController.GoToLine (line);
						else if (sgViewSelect.SelectedSegment == 1)
							CommonClass.BidLineController.GoToLine (line);
						else if (sgViewSelect.SelectedSegment == 2)
							CommonClass.ModernController.GoToLine (line);
					}
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		private void setViews ()
		{
			btnTopLock.Image = NSImage.ImageNamed ("lockIconGreen.png");
			btnBottomLock.Image = NSImage.ImageNamed ("lockIconRed.png");
			btnRemTopLock.Image = NSImage.ImageNamed ("removeLockGreen.png");
			btnRemBottomLock.Image = NSImage.ImageNamed ("removeLockRed.png");
			btnHome.Image = NSImage.ImageNamed ("homeMac.png");
			btnSave.Image = NSImage.ImageNamed ("saveMac.png");

			//MonoMac.CoreGraphics.CGColor aa = new MonoMac.CoreGraphics.CGColor ("Red");
			//okbtnVacation.Layer.BackgroundColor=new MonoMac.CoreGraphics.CGColor(254/255f,200/255f,200/255f);
			//btnVacation.WantsLayer = true;
			//btnVacation.Layer.BackgroundColor=NSColor.Red.CGColor;
			//btnVacation.SetUpGState ();;
			//btnEOM.Cell.ControlTint = NSControlTint.Blue;

			//btnCSW.Cell.BackgroundColor = NSColor.Red;
			//btnCSW.Bordered = false;
			//btnEOM.Cell.BackgroundColor = NSColor.Red;
			//btnEOM.WantsLayer = true;
			//btnEOM.Bordered = false;

			sgViewSelect.Activated += HandleViewSelect;
			btnTopLock.Activated += btnTopLockClicked;
			btnBottomLock.Activated += btnBottomLockClicked;
			btnRemTopLock.Activated += btnRemTopLockClicked;
			btnRemBottomLock.Activated += btnRemBottomLockClicked;
			btnHome.Activated += btnHomeClicked;
			btnSave.Activated += btnSaveClicked;
			btnCSW.Activated += btnCSWClicked;
			btnOverlap.Activated += btnOverLapClicked;
			btnVacation.Activated += btnVacationClicked;
			btnDrop.Activated += btnVacationDropClicked;
			btnEOM.Activated += btnEOMClicked;
			btnReset.Activated += btnResetClicked;
			btnSynch.Activated += btnSynchClicked;

			btnSynch.Enabled = GlobalSettings.WBidINIContent.User.SmartSynch;
			ChangeView ();
		}

		public void ToggleView (int index)
		{
			CommonClass.ViewChanged = true;
			sgViewSelect.SetSelected (true, index);
			ChangeView ();
			ReloadAllContent ();
			CommonClass.ViewChanged = false;
		}

		void btnSynchClicked (object sender, EventArgs e)
		{
			if (GlobalSettings.isModified) {
				//UIAlertView syAlert = new UIAlertView("Smart Sync", "Please save the current state before performing synch.", null, "OK", null);
				//syAlert.Show();
				ShowMessageBox ("Smart Sync", "Please save the current state before performing synch.");
			} else {
				BeginInvokeOnMainThread (() => {
					SynchBtn = true;
					Synch ();
				});
			}

		}

		void btnResetClicked (object sender, EventArgs e)
		{
			ResetAll ();
		}
		private void HandleReparse(object sender, EventArgs e)
		{   if (swUser.State== NSCellStateValue.On)
			{
				GlobalSettings.IsDifferentUser = true;
				GlobalSettings.ModifiedEmployeeNumber =txtUser.StringValue;
			}

			var alert = new NSAlert ();
			alert.MessageText = "WBidMax";
			alert.InformativeText = "Do you want to test Vacation Correction?";
			alert.AddButton ("YES");
			alert.AddButton ("No");
			alert.Buttons [0].Activated += (object senderr, EventArgs ee) => 
			{
				alert.Window.Close();
				NSApplication.SharedApplication.StopModal();
				// show test vacation view
				var panel = new NSPanel ();
				var testVac = new TestVacationViewController ();
				CommonClass.Panel = panel;
				panel.SetContentSize (new System.Drawing.SizeF (350, 300));
				panel.ContentView = testVac.View;
				NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);

				btnDrop.State =  NSCellStateValue.Off;
				btnVacation.State=NSCellStateValue.Off;

			};
			alert.Buttons [1].Activated += (object senderr, EventArgs ee) => 
			{
				alert.Window.Close();
				NSApplication.SharedApplication.StopModal();
				// reparse
				PerformReparse();
			};
			alert.RunModal();

//			//if (btnReparseCheck.Selected)
//			//{
//			//	GlobalSettings.IsDifferentUser = true;
//			//	GlobalSettings.ModifiedEmployeeNumber = txtReparse.Text;
//			//}
//			//if (e.ButtonIndex == 0)
//			//{
//			//	var loadingOverlay = new LoadingOverlay(View.Bounds, "Reparsing..Please Wait..");
//			//	View.Add(loadingOverlay);
//			//	InvokeInBackground(() =>
//			//		{
//						string zipFilename = WBidHelper.GenarateZipFileName();
//						ReparseParameters reparseParams = new ReparseParameters() { ZipFileName = zipFilename };
//						ReparseBL.ReparseTripAndLineFiles(reparseParams);
//						
//			//		});
//			//}
//			//else if (//vacation corection)
//			//{
		}

		public void PerformReparse ()
		{
			GlobalSettings.MenuBarButtonStatus.IsEOM = false;
			GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
			GlobalSettings.MenuBarButtonStatus.IsVacationCorrection  = false;
			GlobalSettings.MenuBarButtonStatus.IsVacationDrop= false;
			overlayPanel = new NSPanel ();
			overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
			overlay = new OverlayViewController ();
			overlay.OverlayText = "Reparsing.. Please wait.";
			overlayPanel.ContentView = overlay.View;
			NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

			string zipFilename = WBidHelper.GenarateZipFileName ();
			ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			string fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
			if (wBIdStateContent.IsOverlapCorrection)
			{

				if (File.Exists(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL"))
				{
					// OverlapData overlapData = XmlHelper.DeserializeFromXml<OverlapData>(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL");
					OverlapData overlapData;
					using (FileStream filestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL"))
					{

						OverlapData overlapdataobj = new OverlapData();
						overlapData = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".OL", overlapdataobj, filestream);
					}

					if (overlapData != null)
					{
						GlobalSettings.LeadOutDays = overlapData.LeadOutDays;
						GlobalSettings.LastLegArrivalTime = Convert.ToInt32(overlapData.LastLegArrivalTime);
					}
				}
			}


			ReparseBL.ReparseTripAndLineFiles (reparseParams);
			string currentBidName =WBidHelper.GenerateFileNameUsingCurrentBidDetails();
			string vACFileName = WBidHelper.GetAppDataPath() + "/" + currentBidName + ".VAC";
			Dictionary<string, TripMultiVacData> VacationData;
			if (GlobalSettings.IsVacationCorrection) 
			{
				bool vacFileExists = File.Exists (vACFileName);
				//if the vac file  for EOM already exists and user doesnot have any vacation , we need to overwrite the vac file 
				if (!wBIdStateContent.IsVacationOverlapOverlapCorrection) {
					vacFileExists = false;

				}
			
				wBIdStateContent.IsVacationOverlapOverlapCorrection = GlobalSettings.IsVacationCorrection;
				if (!vacFileExists) 
				{
					VacationData = new Dictionary<string, TripMultiVacData> ();
				}
				else 
				{

					using (FileStream vacstream = File.OpenRead (vACFileName)) 
					{

						Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData> ();
						VacationData = ProtoSerailizer.DeSerializeObject (vACFileName, objineinfo, vacstream);

					}
				}

				if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null && GlobalSettings.SeniorityListMember.Absences.Where (y => y.AbsenceType == "VA").Count () > 0 && File.Exists (vACFileName)) {

					CaculateVacationDetails caculateVacationDetails = new CaculateVacationDetails ();
					//caculateVacationDetails.CalculateVacationdetailsFromVACfile (lines, vACFileName);
					lines = GlobalSettings.Lines.ToDictionary (x => x.LineNum.ToString (), x => x);
					
					caculateVacationDetails.CalculateVacationdetailsFromVACfile (lines, VacationData);
					// SerializeObject(WBidHelper.GetAppDataPath() + "\\" + filenametosave + ".WBL", lines);

					GlobalSettings.WBidStateCollection.Vacation = new List<Vacation> ();

					var vacation = GlobalSettings.SeniorityListMember.Absences.Where (x => x.AbsenceType == "VA").Select (y => new Vacation {
						StartDate = y.StartAbsenceDate.ToShortDateString (),
						EndDate = y.EndAbsenceDate.ToShortDateString ()
					});
					if (vacation != null) {
						GlobalSettings.WBidStateCollection.Vacation.AddRange (vacation.ToList ());
					}
				}
				LineInfo lineInfo = new LineInfo () {
					LineVersion = GlobalSettings.LineVersion,
					Lines = lines

				};
				GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));
				var linestream = File.Create (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL");
				ProtoSerailizer.SerializeObject (WBidHelper.GetAppDataPath () + "/" + fileToSave + ".WBL", lineInfo, linestream);
				linestream.Dispose ();
				linestream.Close ();
			}



			NSApplication.SharedApplication.EndSheet(overlayPanel);
			overlayPanel.OrderOut(this);
			SetVacButtonStates ();
			ReloadAllContent ();
		}
		public void PerformVacationReparse ()
		{
			overlayPanel = new NSPanel ();
			overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
			overlay = new OverlayViewController ();
			overlay.OverlayText = "Reparsing.. Please wait.";
			overlayPanel.ContentView = overlay.View;
			NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

			string zipFilename = WBidHelper.GenarateZipFileName();
			ReparseParameters reparseParams = new ReparseParameters() { ZipFileName = zipFilename };
			ReparseLineAndTripFileForvacation (reparseParams);

			NSApplication.SharedApplication.EndSheet(overlayPanel);
			overlayPanel.OrderOut(this);
			SetVacButtonStates ();
			ReloadAllContent ();
		}
		private void ReparseLineAndTripFileForvacation(ReparseParameters reparseParams)
		{


			GlobalSettings.MenuBarButtonStatus.IsEOM = false;
			GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
			GlobalSettings.MenuBarButtonStatus.IsVacationCorrection  = false;
			GlobalSettings.MenuBarButtonStatus.IsVacationDrop= false;
			List<string> pairingwHasNoDetails = new List<string>();
			string fileToSave = string.Empty;

			//Parse trip and Line file
			trips = ReparseBL.ParseTripFile(reparseParams.ZipFileName);


			if (reparseParams.ZipFileName.Substring(0, 1) == "A" && reparseParams.ZipFileName.Substring(1, 1) == "B")
			{
				FASecondRoundParser fASecondRound = new FASecondRoundParser();
				lines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + "/" + reparseParams.ZipFileName.Substring(0, 6).ToString() + "/PS", ref trips, GlobalSettings.FAReserveDayPay, reparseParams.ZipFileName.Substring(2, 3));

			}
			else
			{
				lines = ReparseBL.ParseLineFiles(reparseParams.ZipFileName);
			}

			// if (trips == null) return null;

			TripTtpParser tripTtpParser = new TripTtpParser();
			List<CityPair> listCityPair = tripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");

			//Second Round missing trip management
			//---------------------------------------------------------------------------

			if (GlobalSettings.CurrentBidDetails.Round == "S")
			{   //If  the round is second round ,some times trip list contains  missing trip. So we need  take these trip details from old .WBP file.
				//Otherwise we again need to scrap the missing details from website. The issue is if the bid data is older one, we cannot scrap it from website.
				//tempTrip = reparseParams.Trips;


				//List<string> allPair = lines.SelectMany(x => x.Value.Pairings).ToList();
				//pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => z == x.Substring(0, 4))).ToList();
				List<string> allPair = lines.SelectMany(x => x.Value.Pairings).Distinct().ToList();
				pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x && x.Substring(1, 1) == "P"))).ToList();

				if (pairingwHasNoDetails.Count > 0)
				{
					Dictionary<string, Trip> missingTrips = new Dictionary<string, Trip>();
					//missingTrips = missingTrips.Concat(tempTrip.Where(x => pairingwHasNoDetails.Contains(x.Key.ToString()))).ToDictionary(pair => pair.Key, pair => pair.Value);
					missingTrips = missingTrips.Concat(GlobalSettings.Trip.Where(x => pairingwHasNoDetails.Contains(x.TripNum)).ToDictionary(s => s.TripNum, s => s)).ToDictionary(pair => pair.Key, pair => pair.Value);
					if (missingTrips.Count == 0)
					{
						string bidFileName = string.Empty;
						bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
						BidLineParser bidLineParser = new BidLineParser();
						var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;
						missingTrips = missingTrips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "\\" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
					}
					// trips = trips.Concat().ToDictionary(pair => pair.Key, pair => pair.Value);
					foreach (var trip in missingTrips)
					{
						if (!trips.Keys.Contains(trip.Key) && !string.IsNullOrEmpty(trip.Key))
							trips.Add(trip.Key, trip.Value);
					}

				}


			}

			//---------------------------------------------------------------------------



			// Additional processing needs to be done to FA trips before CalculateTripPropertyValues
			CalculateTripProperties calcProperties = new CalculateTripProperties();
			if (reparseParams.ZipFileName.Substring(0, 1) == "A")
				calcProperties.PreProcessFaTrips(trips, listCityPair);

			calcProperties.CalculateTripPropertyValues(trips, listCityPair);


			GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));

			CalculateLineProperties calcLineProperties = new CalculateLineProperties();
			calcLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);

			GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));

			if (GlobalSettings.IsVacationCorrection)
			{
				PerformVacation();
				SaveParsedFiles(trips,lines);


			}




		}

		private void PerformVacation()
		{
			try
			{
				VacationCorrectionParams vacationParams = new VacationCorrectionParams();


				if (GlobalSettings.CurrentBidDetails.Postion != "FA")
				{
					string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
					string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
					string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";

					FlightPlan flightPlan = null;
					WebClient wcClient = new WebClient();
					//Downloading networkdat file
					wcClient.DownloadFile(serverPath, zipLocalFile);


					// Open an existing zip file for reading
					ZipStorer zip = ZipStorer.Open(zipLocalFile, FileAccess.Read);

					// Read the central directory collection
					List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

					// Look for the desired file
					foreach (ZipStorer.ZipFileEntry entry in dir)
					{
						zip.ExtractFile(entry, networkDataPath);
					}
					zip.Close();

					//Deserializing data to FlightPlan object
					FlightPlan fp = new FlightPlan();
					using (FileStream networkDatatream = File.OpenRead(networkDataPath))
					{

						FlightPlan objineinfo = new FlightPlan();
						flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);

					}

					if (File.Exists(zipLocalFile))
					{
						File.Delete(zipLocalFile);
					}
					if (File.Exists(networkDataPath))
					{
						File.Delete(networkDataPath);
					}




					vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
						(fr, f) =>
						new FlightRouteDetails
						{
							Flight = f.FlightId,
							FlightDate = fr.FlightDate,
							Orig = f.Orig,
							Dest = f.Dest,
							Cdep = f.Cdep,
							Carr = f.Carr,
							Ldep = f.Ldep,
							Larr = f.Larr,
							RouteNum = fr.RouteNum,

						}).ToList();

				}


				vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
				vacationParams.Trips = trips;
				vacationParams.Lines = lines;
				//  VacationData = new Dictionary<string, TripMultiVacData>();


				//Performing vacation correction algoritham
				VacationCorrectionBL vacationBL = new VacationCorrectionBL();

				if (GlobalSettings.CurrentBidDetails.Postion != "FA")
				{
					GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);
				}
				else
				{
					GlobalSettings.VacationData = vacationBL.PerformFAVacationCorrection(vacationParams);
				}



				if (GlobalSettings.VacationData != null)
				{

					string fileToSave = string.Empty;
					fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


					// save the VAC file to app data folder

					var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
					ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
					stream.Dispose();
					stream.Close();
				}
				else
				{
					GlobalSettings.IsVacationCorrection = false;
				}



			}
			catch (Exception ex)
			{
				GlobalSettings.IsVacationCorrection = false;
				throw ex;
			}
		}

		private void SaveParsedFiles(Dictionary<string, Trip> trips, Dictionary<string, Line> lines)
		{

			string fileToSave = string.Empty;

			fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


			TripInfo tripInfo = new TripInfo()
			{
				TripVersion = GlobalSettings.TripVersion,
				Trips = trips

			};

			var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP");
			ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBP", tripInfo, stream);
			stream.Dispose();
			stream.Close();

			GlobalSettings.Trip = new ObservableCollection<Trip>(trips.Select(x => x.Value));


			if (GlobalSettings.IsVacationCorrection && GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0)
			{//set  vacation details  to line object. 

				CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
				calVacationdetails.CalculateVacationdetailsFromVACfile(lines, GlobalSettings.VacationData);
			}

			LineInfo lineInfo = new LineInfo()
			{
				LineVersion = GlobalSettings.LineVersion,
				Lines = lines

			};

			GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(lines.Select(x => x.Value));

			try
			{
				var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
				ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
				linestream.Dispose();
				linestream.Close();
			}
			catch (Exception ex)
			{
				throw ex;
			}


			foreach (Line line in GlobalSettings.Lines)
			{
				line.ConstraintPoints = new ConstraintPoints();
				line.WeightPoints = new WeightPoints();
			}

			//Read the intial state file value from DWC file and create state file
			if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS"))
			{
				try
				{

					WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
					GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			else
			{
				//Read the state file object and store it to global settings.
				GlobalSettings.WBidStateCollection = XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
			}
			//save the vacation to state file
			GlobalSettings.WBidStateCollection.Vacation = new List<Vacation>();
			WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);

			if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null && GlobalSettings.IsVacationCorrection)
			{
				var vacation = GlobalSettings.SeniorityListMember.Absences.Where(x => x.AbsenceType == "VA").Select(y => new Vacation { StartDate = y.StartAbsenceDate.ToShortDateString(), EndDate = y.EndAbsenceDate.ToShortDateString() });

				GlobalSettings.WBidStateCollection.Vacation.AddRange(vacation.ToList());

				wBIdStateContent.IsVacationOverlapOverlapCorrection = true;
			}
			else
				wBIdStateContent.IsVacationOverlapOverlapCorrection = false;
			WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);



		}
		public void ShowCSW () 
		{	
			try {
				if (cswWC == null)
					cswWC = new CSWWindowController ();
				CommonClass.CSWController = cswWC;
				//				this.Window.SetFrameOrigin (NSScreen.MainScreen.VisibleFrame.Location);
				//				this.Window.SetContentSize (new System.Drawing.SizeF (NSScreen.MainScreen.VisibleFrame.Width - cswWC.Window.Frame.Size.Width, this.Window.Frame.Size.Height));
				//				cswWC.Window.SetFrameOrigin(new System.Drawing.PointF(NSScreen.MainScreen.VisibleFrame.Width - cswWC.Window.Frame.Size.Width,cswWC.Window.Frame.Location.Y));
				this.Window.AddChildWindow (cswWC.Window, NSWindowOrderingMode.Above);
				cswWC.Window.MakeKeyWindow ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}
		private void applyVacation()
		{
			try
			{
				var str = string.Empty;
				if (btnEOM.State==NSCellStateValue.On)
					str = "Applying EOM";
				else
					str = "Applying Vacation Correction";

				overlayPanel = new NSPanel ();
				overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
				overlay = new OverlayViewController ();
				overlay.OverlayText = str;
				overlayPanel.ContentView = overlay.View;
				NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
				InvokeOnMainThread(() =>
					{
						try
						{
							WBidCollection.GenarateTempAbsenceList();
							PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView();
							RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties();
							prepareModernBidLineView.CalculatebidLinePropertiesforVacation();
							RecalcalculateLineProperties.CalcalculateLineProperties();
						}
						catch (Exception ex)
						{
							InvokeOnMainThread(() =>
								{
									throw ex;
								});
						}

						InvokeOnMainThread(() =>
							{
								NSApplication.SharedApplication.EndSheet(overlayPanel);
								overlayPanel.OrderOut(this);

								ReloadAllContent();
							});
					});
			}
			catch (Exception ex)
			{

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);

			}
		}
		void btnEOMClicked (object sender, EventArgs e)
		{



			 try
			{

				if (btnEOM.State == NSCellStateValue.On)
				{
					GlobalSettings.MenuBarButtonStatus.IsEOM = true;
					SetVacButtonStates();
					if (GlobalSettings.CurrentBidDetails.Postion == "FA")
					{
						//DateTime defDate = new DateTime(GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);
						//defDate.AddMonths(1);
						//string[] strParams = { String.Format("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1)), String.Format("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2)), String.Format("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3)) };
						//UIActionSheet sheet = new UIActionSheet("Where does your vacation start on next month?", null, null, null, strParams);
						//sheet.ShowFrom(sender.Frame, tbBottomBar, true);
						//GlobalSettings.FAEOMStartDate = DateTime.MinValue;
						//sheet.Clicked += handleEOMOptions;

						EOMFAViewController eomFA = new EOMFAViewController ();
						string[] strParams = { String.Format("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1)), String.Format("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2)), String.Format("{0:m}", GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3)) };
						eomFA.options = strParams;
						var panel = new NSPanel ();
						CommonClass.Panel = panel;
						panel.SetContentSize (new System.Drawing.SizeF (400, 220));
						panel.ContentView = eomFA.View;
						NSApplication.SharedApplication.BeginSheet (panel, this.Window);
						notif = NSNotificationCenter.DefaultCenter.AddObserver ("EOMFAVacation", (NSNotification obj) => {
							NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
							NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
							CommonClass.Panel.OrderOut (this);
							if(obj.Object!=null)
							{
								//var option = obj.Object.ToString ();
								handleEOMOptions(Convert.ToInt32(obj.Object.ToString ()));

							}
						});

					}
					else
					{
						SetPropertyNames ();
						//sender.Selected = true;
						btnEOM.State=NSCellStateValue.On;


						string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();

						//string zipFileName = GenarateZipFileName();
						string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
						//Cheks the VAC file exists
						bool vacFileExists = File.Exists(vACFileName);

						if (!vacFileExists)
						{

							CreateEOMVacFileForCP(currentBidName);
						}
						else
						{



							string overlayTxt = string.Empty;
							if (GlobalSettings.MenuBarButtonStatus.IsEOM)
								overlayTxt = "Applying EOM";
							else
								overlayTxt = "Removing EOM";

							overlayPanel = new NSPanel ();
							overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
							overlay = new OverlayViewController ();
							overlay.OverlayText = overlayTxt;
							overlayPanel.ContentView = overlay.View;
							NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

							//LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
							//this.View.Add(overlay);
							InvokeOnMainThread(() =>
								{

									try
									{

										if (GlobalSettings.VacationData == null)
										{
											using (FileStream vacstream = File.OpenRead(vACFileName))
											{

												Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
												GlobalSettings.VacationData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);
											}
										}


										GenerateVacationDataView();

										InvokeOnMainThread(() =>
											{
												//loadSummaryListAndHeader();
												// NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
												NSApplication.SharedApplication.EndSheet(overlayPanel);
												overlayPanel.OrderOut(this);

												GlobalSettings.isModified = true;
												ReloadAllContent();
												//CommonClass.lineVC.UpdateSaveButton();
											});
									}
									catch (Exception ex)
									{
										InvokeOnMainThread(() =>
											{
												CommonClass.AppDelegate.ErrorLog (ex);
												CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
											});
									}
								});

						}




					}
				}

				else
				{


					GlobalSettings.MenuBarButtonStatus.IsEOM = false;
					//sender.Selected = false;
					btnEOM.State=NSCellStateValue.Off;
					SetPropertyNames ();
					if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection)
					{
						//btnVacDrop.Selected = false;
						btnDrop.State=NSCellStateValue.Off;
						GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
					}

					SetVacButtonStates();

					string overlayTxt = string.Empty;
					if (GlobalSettings.MenuBarButtonStatus.IsEOM)
						overlayTxt = "Applying EOM";
					else
						overlayTxt = "Removing EOM";

					overlayPanel = new NSPanel ();
					overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
					overlay = new OverlayViewController ();
					overlay.OverlayText = overlayTxt;
					overlayPanel.ContentView = overlay.View;
					NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

					InvokeOnMainThread(() =>
						{
							try
							{
								GenerateVacationDataView();
							}
							catch (Exception ex)
							{
								InvokeOnMainThread(() =>
									{
										CommonClass.AppDelegate.ErrorLog (ex);
										CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
									});
							}

							InvokeOnMainThread(() =>
								{
									//loadSummaryListAndHeader();
									//NSNotificationCenter.DefaultCenter.PostNotificationName("DataReload", null);
									NSApplication.SharedApplication.EndSheet(overlayPanel);
									overlayPanel.OrderOut(this);

									GlobalSettings.isModified = true;
									ReloadAllContent();
									//CommonClass.lineVC.UpdateSaveButton();
								});
						});

				}

			}
			catch (Exception ex)
			{

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		private void CreateEOMVacFileForCP(string currentBidName)
		{

			var alert = new NSAlert ();
			alert.AlertStyle = NSAlertStyle.Critical;
			alert.MessageText = "WBidMax";
			alert.InformativeText = "WBidMax needs to download vacation data to make the predictions for your end of month trips (EOM VAC).   This could take up to a minute.  Do you want to continue?";
			alert.AddButton ("YES");
			alert.AddButton ("Cancel");
			alert.Buttons [0].Activated += (object sender, EventArgs e) => 
			{
				alert.Window.Close();
				NSApplication.SharedApplication.StopModal();
				DownloadEOMData();


			};
			alert.Buttons [1].Activated += (object sender, EventArgs e) => 
			{
				if (!wBIdStateContent.IsVacationOverlapOverlapCorrection)
					btnDrop.Enabled = false;
				btnEOM.State = NSCellStateValue.Off;
				GlobalSettings.MenuBarButtonStatus.IsEOM = false;
				NSApplication.SharedApplication.StopModal();
			};
			alert.RunModal();

		}

		private static void GenerateVacationDataView()
		{

			WBidCollection.GenarateTempAbsenceList();
			PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView();
			RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties();
			prepareModernBidLineView.CalculatebidLinePropertiesforVacation();
			RecalcalculateLineProperties.CalcalculateLineProperties();
			SortLineList();
		}
		void DownloadEOMData()
		{

			try
			{
				btnDrop.Enabled = true;
					string overlayTxt = string.Empty;
					if (GlobalSettings.MenuBarButtonStatus.IsEOM)
						overlayTxt = "Applying EOM";
					else
						overlayTxt = "Removing EOM";

				overlayPanel = new NSPanel ();
				overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
				overlay = new OverlayViewController ();
				overlay.OverlayText = overlayTxt;
				overlayPanel.ContentView = overlay.View;
				NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);

					InvokeOnMainThread(() =>
						{
							CreateEOMVacationforCP();

							GenerateVacationDataView();

							InvokeOnMainThread(() =>
								{
									//loadSummaryListAndHeader();

								NSApplication.SharedApplication.EndSheet(overlayPanel);
								overlayPanel.OrderOut(this);
								ReloadAllContent();
								});
						});

			}
			catch (Exception ex)
			{

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		
		}
		private static void CreateEOMVacationforCP()
		{


			try
			{

				DateTime nextSunday = GetnextSunday();


				GlobalSettings.OrderedVacationDays = new List<Absense>() { new Absense { StartAbsenceDate = nextSunday, EndAbsenceDate = nextSunday.AddDays(6), AbsenceType = "VA" } };

				string serverPath = GlobalSettings.WBidDownloadFileUrl + "FlightData.zip";
				string zipLocalFile = Path.Combine(WBidHelper.GetAppDataPath(), "FlightData.zip");
				string networkDataPath = WBidHelper.GetAppDataPath() + "/" + "FlightData.NDA";

				FlightPlan flightPlan = null;
				WebClient wcClient = new WebClient();
				//Downloading networkdat file
				wcClient.DownloadFile(serverPath, zipLocalFile);

				//string appdataPath=WBidHelper.GetAppDataPath();
				//string target = Path.Combine(appdataPath, WBidHelper.appdataPath + "/" + Path.GetFileNameWithoutExtension(fileName)) + "/";
				//string zipFile = Path.Combine(appdataPath, fileName);
				//Extracting the zip file
				//var zip = new ZipArchive();
				//zip.EasyUnzip(zipLocalFile, WBidHelper.GetAppDataPath(), true, "");


				// Open an existing zip file for reading
				ZipStorer zip = ZipStorer.Open(zipLocalFile, FileAccess.Read);

				// Read the central directory collection
				List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

				// Look for the desired file
				foreach (ZipStorer.ZipFileEntry entry in dir)
				{
					zip.ExtractFile(entry, networkDataPath);
				}
				zip.Close();
				//Deserializing data to FlightPlan object
				FlightPlan fp = new FlightPlan();
				using (FileStream networkDatatream = File.OpenRead(networkDataPath))
				{

					FlightPlan objineinfo = new FlightPlan();
					flightPlan = ProtoSerailizer.DeSerializeObject(networkDataPath, fp, networkDatatream);

				}

				if (File.Exists(zipLocalFile))
				{
					File.Delete(zipLocalFile);
				}
				if (File.Exists(networkDataPath))
				{
					File.Delete(networkDataPath);
				}



				VacationCorrectionParams vacationParams = new VacationCorrectionParams();
				vacationParams.FlightRouteDetails = flightPlan.FlightRoutes.Join(flightPlan.FlightDetails, fr => fr.FlightId, f => f.FlightId,
					(fr, f) =>
					new FlightRouteDetails
					{
						Flight = f.FlightId,
						FlightDate = fr.FlightDate,
						Orig = f.Orig,
						Dest = f.Dest,
						Cdep = f.Cdep,
						Carr = f.Carr,
						Ldep = f.Ldep,
						Larr = f.Larr,
						RouteNum = fr.RouteNum,

					}).ToList();






				vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
				vacationParams.Trips = GlobalSettings.Trip.ToDictionary(x => x.TripNum, x => x);
				vacationParams.Lines = GlobalSettings.Lines.ToDictionary(x => x.LineNum.ToString(), x => x);
				vacationParams.IsEOM = true;
				//  VacationData = new Dictionary<string, TripMultiVacData>();


				//Performing vacation correction algoritham
				VacationCorrectionBL vacationBL = new VacationCorrectionBL();
				GlobalSettings.VacationData = vacationBL.PerformVacationCorrection(vacationParams);


				if (GlobalSettings.VacationData != null)
				{

					string fileToSave = string.Empty;
					fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


					// save the VAC file to app data folder

					var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
					ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
					stream.Dispose();
					stream.Close();
				}
				else
				{
					GlobalSettings.IsVacationCorrection = false;
				}
				GlobalSettings.OrderedVacationDays = null;

			}
			catch (Exception ex)
			{
				GlobalSettings.OrderedVacationDays = null;

				CommonClass.AppDelegate.ErrorLog (ex);
				//CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public static DateTime GetnextSunday()
		{
			DateTime date = GlobalSettings.CurrentBidDetails.BidPeriodEndDate;
			for (int count = 1; count <= 3; count++)
			{
				date = date.AddDays(1);
				if (date.DayOfWeek.ToString() == "Sunday")
					break;
			}


			return date;
		}

		void handleEOMOptions(int option)
		{
			if (option == 0)
			{
				//btnEOM.Selected = true;
				btnEOM.State = NSCellStateValue.On;
				GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1);
			}
			else if (option == 1)
			{
				//btnEOM.Selected = true;
				btnEOM.State = NSCellStateValue.On;
				GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2);

			}
			else if (option == 2)
			{
				//btnEOM.Selected = true;
				btnEOM.State = NSCellStateValue.On;
				GlobalSettings.FAEOMStartDate = GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3);

			}
			else
			{
				//btnEOM.Selected = false;
				btnEOM.State = NSCellStateValue.Off;
				GlobalSettings.MenuBarButtonStatus.IsEOM = false;
				if (!(btnEOM.State==NSCellStateValue.On)&& !(btnVacation.State==NSCellStateValue.On))
					btnDrop.Enabled = false;

			}

			if (GlobalSettings.FAEOMStartDate != null && GlobalSettings.FAEOMStartDate != DateTime.MinValue)
			{
				btnDrop.Enabled = true;

				string overlayTxt = string.Empty;
				if (GlobalSettings.MenuBarButtonStatus.IsEOM)
					overlayTxt = "Applying EOM";
				else
					overlayTxt = "Removing EOM";

				overlayPanel = new NSPanel ();
				overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
				overlay = new OverlayViewController ();
				overlay.OverlayText = overlayTxt;
				overlayPanel.ContentView = overlay.View;
				NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
				InvokeOnMainThread(() =>
					{
						CreateEOMVacforFA();

						GenerateVacationDataView();

						InvokeOnMainThread(() =>
							{
								//loadSummaryListAndHeader();

								NSApplication.SharedApplication.EndSheet(overlayPanel);
								overlayPanel.OrderOut(this);

								GlobalSettings.isModified = true;
								ReloadAllContent();
								//CommonClass.lineVC.UpdateSaveButton();
							});
					});



			}
		}

		private void CreateEOMVacforFA()
		{
			VacationCorrectionParams vacationParams = new VacationCorrectionParams();
			vacationParams.CurrentBidDetails = GlobalSettings.CurrentBidDetails;
			vacationParams.Trips = GlobalSettings.Trip.ToDictionary(x => x.TripNum, x => x);
			vacationParams.Lines = GlobalSettings.Lines.ToDictionary(x => x.LineNum.ToString(), x => x);
			Dictionary<string, TripMultiVacData> allTripsMultiVacData = null;

			string currentBidName = WBidHelper.GenerateFileNameUsingCurrentBidDetails();


			string vACFileName = WBidHelper.GetAppDataPath() + "//" + currentBidName + ".VAC";
			//Cheks the VAC file exists
			bool vacFileExists = File.Exists(vACFileName);

			if (!vacFileExists)
			{
				allTripsMultiVacData = new Dictionary<string, TripMultiVacData>();
			}
			else
			{

				using (FileStream vacstream = File.OpenRead(vACFileName))
				{

					Dictionary<string, TripMultiVacData> objineinfo = new Dictionary<string, TripMultiVacData>();
					allTripsMultiVacData = ProtoSerailizer.DeSerializeObject(vACFileName, objineinfo, vacstream);

				}
			}



			//Performing vacation correction algoritham
			VacationCorrectionBL vacationBL = new VacationCorrectionBL();
			GlobalSettings.VacationData = vacationBL.CreateVACfileForEOMFA(vacationParams, allTripsMultiVacData);



			string fileToSave = string.Empty;
			fileToSave = WBidHelper.GenerateFileNameUsingCurrentBidDetails();
			if (GlobalSettings.VacationData != null && GlobalSettings.VacationData.Count > 0)
			{




				// save the VAC file to app data folder

				var stream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC");
				ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".VAC", GlobalSettings.VacationData, stream);
				stream.Dispose();
				stream.Close();


				CaculateVacationDetails calVacationdetails = new CaculateVacationDetails();
				calVacationdetails.CalculateVacationdetailsFromVACfile(vacationParams.Lines, GlobalSettings.VacationData);

				//set the Vacpay,Vdrop,Vofont and VoBack columns in the line summary view 
				ManageVacationColumns managevacationcolumns = new ManageVacationColumns();
				managevacationcolumns.SetVacationColumns();

				LineInfo lineInfo = new LineInfo()
				{
					LineVersion = GlobalSettings.LineVersion,
					Lines = vacationParams.Lines

				};




				GlobalSettings.Lines = new System.Collections.ObjectModel.ObservableCollection<Line>(vacationParams.Lines.Select(x => x.Value));


				try
				{
					var linestream = File.Create(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL");
					ProtoSerailizer.SerializeObject(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBL", lineInfo, linestream);
					linestream.Dispose();
					linestream.Close();
				}
				catch (Exception ex)
				{

					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}


				foreach (Line line in GlobalSettings.Lines)
				{
					if (line.ConstraintPoints == null)
						line.ConstraintPoints = new ConstraintPoints();
					if (line.WeightPoints == null)
						line.WeightPoints = new WeightPoints();
				}

			}






		}
		void btnVacationDropClicked (object sender, EventArgs e)
		{

			GlobalSettings.MenuBarButtonStatus.IsVacationDrop = (btnDrop.State == NSCellStateValue.On);
			SetVacButtonStates();
			SetPropertyNames ();
			WBidCollection.GenarateTempAbsenceList();
			string overlayTxt = string.Empty;
			if (GlobalSettings.MenuBarButtonStatus.IsVacationDrop)
				overlayTxt = "Applying Vacation Drop";
			else
				overlayTxt = "Removing Vacation Drop";
			overlayPanel = new NSPanel ();
			overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
			overlay = new OverlayViewController ();
			overlay.OverlayText = overlayTxt;
			overlayPanel.ContentView = overlay.View;
			NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
			BeginInvokeOnMainThread(() =>
				{
					try
					{
						PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView();
						RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties();
						prepareModernBidLineView.CalculatebidLinePropertiesforVacation();
						RecalcalculateLineProperties.CalcalculateLineProperties();
						SortLineList();

					}
					catch (Exception ex)
					{
						InvokeOnMainThread(() =>
							{
								CommonClass.AppDelegate.ErrorLog (ex);
								CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
							});
					}

					InvokeOnMainThread(() =>
						{
						
							GlobalSettings.isModified = true;
							//CommonClass.lineVC.UpdateSaveButton();
							NSApplication.SharedApplication.EndSheet(overlayPanel);
							overlayPanel.OrderOut(this);
							ReloadAllContent();
						});
				});
		}
		void btnVacationClicked (object sender, EventArgs e)
		{

			try {
				if (btnVacation.State == NSCellStateValue.On) {
					//vacation button selected.
					GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
					GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
				} else {


					//vacation button un selected.
					GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
					if (GlobalSettings.MenuBarButtonStatus.IsEOM == false) {

						btnDrop.State = NSCellStateValue.Off;
						GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
					}

				}


				SetVacButtonStates();
				SetPropertyNames ();
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;


				WBidCollection.GenarateTempAbsenceList ();
				string overlayTxt = string.Empty;
				if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) {
					overlayTxt = "Applying Vacation Correction";
					//btnVacDrop.Enabled = true;

				} else {
					overlayTxt = "Removing Vacation Correction";
					//btnVacDrop.Enabled = false;

				}
				//foreach (var column in GlobalSettings.AdditionalColumns) {
				//	column.IsSelected = false;
				//}
				//var selectedColumns = GlobalSettings.AdditionalColumns.Where (x => GlobalSettings.WBidINIContent.DataColumns.Any (y => y.Id == x.Id)).ToList ();
				//foreach (var selectedColumn in selectedColumns) {
				//	selectedColumn.IsSelected = true;
				//}

				overlayPanel = new NSPanel ();
				overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
				overlay = new OverlayViewController ();
				overlay.OverlayText = overlayTxt;
				overlayPanel.ContentView = overlay.View;
				NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);


				BeginInvokeOnMainThread (() => {
					try {

						PrepareModernBidLineView prepareModernBidLineView = new PrepareModernBidLineView ();
						RecalcalculateLineProperties RecalcalculateLineProperties = new RecalcalculateLineProperties ();
						prepareModernBidLineView.CalculatebidLinePropertiesforVacation ();
						RecalcalculateLineProperties.CalcalculateLineProperties ();
						SortLineList ();
					} catch (Exception ex) {
						InvokeOnMainThread (() => {

							throw ex;
						});
					}

					InvokeOnMainThread (() => {

						//loadSummaryListAndHeader ();

						GlobalSettings.isModified = true;
						//CommonClass.lineVC.UpdateSaveButton();
						NSApplication.SharedApplication.EndSheet(overlayPanel);
						overlayPanel.OrderOut(this);
						ReloadAllContent();
					});
				});



			} catch (Exception ex) {

				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void SetVacButtonStates()
		{
			// Configuring Modern view property lists.

			if (wBIdStateContent.IsOverlapCorrection)
			{
				if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM)
				{
					if (!btnOverlap.Enabled)
						btnOverlap.Enabled = true;

				}
				else
				{
					if (btnOverlap.Enabled)
						btnOverlap.Enabled = false;

				}

			}
			else
			{
				if (btnOverlap.Enabled)
					btnOverlap.Enabled = false;
			}


			if (GlobalSettings.IsVacationCorrection)

				wBIdStateContent.IsVacationOverlapOverlapCorrection = true;

			if (wBIdStateContent.IsVacationOverlapOverlapCorrection)
			{

				if (!GlobalSettings.MenuBarButtonStatus.IsOverlap)
				{
					if (!btnVacation.Enabled)
					{
						btnVacation.Enabled = true;
					}

				}
				else
				{
					if (btnVacation.Enabled)
					{
						btnVacation.Enabled = false;
					}

				}

			}
			else
			{

				if (btnVacation.Enabled)
					btnVacation.Enabled = false;

			}




			if (!GlobalSettings.MenuBarButtonStatus.IsOverlap && (GlobalSettings.CurrentBidDetails.Postion == "FA" || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(1).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(2).DayOfWeek == 0 || (int)GlobalSettings.CurrentBidDetails.BidPeriodEndDate.AddDays(3).DayOfWeek == 0))
			{
				if (!btnEOM.Enabled)
					btnEOM.Enabled = true;
				// IsEOMChecked = false;
			}
			else
			{
				if (btnEOM.Enabled)
					btnEOM.Enabled = false;

			}

			if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM)
			{
				if (!btnDrop.Enabled)
					btnDrop.Enabled = true;
			}
			else
			{

				if (btnDrop.Enabled)
					btnDrop.Enabled = false;

			}

			if (!GlobalSettings.MenuBarButtonStatus.IsVacationCorrection && !GlobalSettings.MenuBarButtonStatus.IsEOM)
			{
				GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
			}


			btnEOM.State = (GlobalSettings.MenuBarButtonStatus.IsEOM)?NSCellStateValue.On:NSCellStateValue.Off;
			btnVacation.State = (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection) ? NSCellStateValue.On : NSCellStateValue.Off;
			btnDrop.State = (GlobalSettings.MenuBarButtonStatus.IsVacationDrop) ? NSCellStateValue.On : NSCellStateValue.Off;

		}

		void btnOverLapClicked (object sender, EventArgs e)
		{
			try {
				ReCalculateLinePropertiesForOverlapCorrection reCalculateLinePropertiesForOverlapCorrection = new ReCalculateLinePropertiesForOverlapCorrection ();
				string overlayTxt = string.Empty;
				
				if (btnOverlap.State == NSCellStateValue.On) {
				
					GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
					GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
					GlobalSettings.MenuBarButtonStatus.IsEOM = false;
					GlobalSettings.MenuBarButtonStatus.IsOverlap = true;
					overlayTxt = "Applying Overlap Correction";
				
					SetVacButtonStates ();
				
				
					overlayPanel = new NSPanel ();
					overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
					overlay = new OverlayViewController ();
					overlay.OverlayText = overlayTxt;
					overlayPanel.ContentView = overlay.View;
					NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
				
					//LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, overlayTxt);
					//this.View.Add(overlay);
					BeginInvokeOnMainThread (() => {
						try {
							reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), true);
							SortLineList ();
						} catch (Exception ex) {
							InvokeOnMainThread (() => {
								throw ex;
							});
						}
				
						InvokeOnMainThread (() => {
							NSApplication.SharedApplication.EndSheet (overlayPanel);
							overlayPanel.OrderOut (this);
							ReloadAllContent ();
							//								CommonClass.SummaryController.ReloadContent();
				
							GlobalSettings.isModified = true;
							//CommonClass.lineVC.UpdateSaveButton();
						});
					});
				} else {
					GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = false;
					GlobalSettings.MenuBarButtonStatus.IsVacationDrop = false;
					GlobalSettings.MenuBarButtonStatus.IsEOM = false;
					GlobalSettings.MenuBarButtonStatus.IsOverlap = false;
					overlayTxt = "Removing Overlap Correction";
				
					//SetVacButtonStates();
				
					overlayPanel = new NSPanel ();
					overlayPanel.SetContentSize (new System.Drawing.SizeF (400, 120));
					overlay = new OverlayViewController ();
					overlay.OverlayText = overlayTxt;
					overlayPanel.ContentView = overlay.View;
					NSApplication.SharedApplication.BeginSheet (overlayPanel, this.Window);
				
				
				
					BeginInvokeOnMainThread (() => {
						reCalculateLinePropertiesForOverlapCorrection.ReCalculateLinePropertiesOnOverlapCorrection (GlobalSettings.Lines.ToList (), false);
				
						InvokeOnMainThread (() => {
				
							NSApplication.SharedApplication.EndSheet (overlayPanel);
							overlayPanel.OrderOut (this);
							ReloadAllContent ();
							//								CommonClass.SummaryController.ReloadContent();
				
							GlobalSettings.isModified = true;
							//CommonClass.lineVC.UpdateSaveButton();
						});
					});
				
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
//		void btnCSWClicked (object sender, EventArgs e)
//		{
//			if (cswWC == null)
//				cswWC = new CSWWindowController ();
//			CommonClass.CSWController = cswWC;
////				this.Window.SetFrameOrigin (NSScreen.MainScreen.VisibleFrame.Location);
////				this.Window.SetContentSize (new System.Drawing.SizeF (NSScreen.MainScreen.VisibleFrame.Width - cswWC.Window.Frame.Size.Width, this.Window.Frame.Size.Height));
////				cswWC.Window.SetFrameOrigin(new System.Drawing.PointF(NSScreen.MainScreen.VisibleFrame.Width - cswWC.Window.Frame.Size.Width,cswWC.Window.Frame.Location.Y));
//			this.Window.AddChildWindow (cswWC.Window, NSWindowOrderingMode.Above);
//			cswWC.Window.MakeKeyWindow ();
//		}

		void btnCSWClicked (object sender, EventArgs e)
		{
			ShowCSW ();
		}

		void btnSaveClicked (object sender, EventArgs e)
		{
			try {
				StateManagement stateManagement = new StateManagement ();
				stateManagement.UpdateWBidStateContent ();
				//CompareState compareState = new CompareState ();
				//string fileName = WBidHelper.GenerateFileNameUsingCurrentBidDetails ();
				//var WbidCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileName + ".WBS");
				//var wBIdStateContent = WbidCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				//bool isNochange = compareState.CompareStateChange(wBIdStateContent, GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName));
				if (GlobalSettings.isModified) {
				
					GlobalSettings.WBidStateCollection.IsModified = true;
//					 StateManagement stateManagement = new StateManagement();
//					 stateManagement.UpdateWBidStateContent();
					WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
					if (timer != null)
					{
					timer.Stop();
						timer.Start();
					}
				//	save the state of the INI File
					WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
					UpdateSaveButton (false);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		void btnHomeClicked (object sender, EventArgs e)
		{
			try {
				if(GlobalSettings.isModified){
					var alert = new NSAlert ();
					alert.Window.Title = "WBidMax";
					alert.MessageText = "Save your Changes?";
					//alert.InformativeText = "There are no Latest News available..!";
					alert.AddButton ("Save & Exit");
					alert.AddButton ("Exit");
					alert.AddButton ("Cancel");
					alert.Buttons[0].Activated += delegate {
						// save and exit
						StateManagement stateManagement = new StateManagement ();
						stateManagement.UpdateWBidStateContent ();
						if (GlobalSettings.isModified) {
							GlobalSettings.WBidStateCollection.IsModified = true;
							WBidHelper.SaveStateFile (WBidHelper.WBidStateFilePath);
						}
						//GoToHomeWindow ();
						alert.Window.Close();
						NSApplication.SharedApplication.StopModal();
						isNeedToClose = 1;
						CheckSmartSync();
					};
					alert.Buttons[1].Activated += delegate {
						GoToHomeWindow ();
						alert.Window.Close();
						NSApplication.SharedApplication.StopModal();
					};
					alert.RunModal ();
				} else {
					if (GlobalSettings.WBidStateCollection.IsModified) {
						isNeedToClose = 1;
						CheckSmartSync();
					} else
						GoToHomeWindow ();
				}
				if (timer != null)
				{
					timer.Stop();
					timer.Start();
				}
				//save the state of the INI File
				WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void GoToHomeWindow ()
		{
			CommonClass.columnID = 0;
			CommonClass.isHomeWindow = true;
			CommonClass.HomeController.Window.MakeKeyAndOrderFront (this);
			this.Window.Close ();
			this.Window.OrderOut (this);
			CommonClass.AppDelegate.ReloadMenu ();
			if (this.Window.ChildWindows != null) {
				foreach (var item in this.Window.ChildWindows) {
					item.Close ();
				}
			}
			if (CommonClass.Panel != null) {
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
			}
		}

		public void ShowClendarView ()
		{
			if(calendarWC==null)
				calendarWC = new CalendarWindowController ();
			CommonClass.CalendarController = calendarWC;
			this.Window.AddChildWindow (calendarWC.Window, NSWindowOrderingMode.Above);
			calendarWC.Window.MakeKeyAndOrderFront (this);
			calendarWC.LoadContent ();
		}

		public void MoveLineUp ()
		{
			summaryVC.MoveLineUp ();
		}

		public void MoveLineDown ()
		{ 
			summaryVC.MoveLineDown ();
		}

		void btnRemTopLockClicked (object sender, EventArgs e)
		{
			StateManagement stateManagement = new StateManagement();
			stateManagement.UpdateWBidStateContent();
			WBidHelper.PushToUndoStack();

			RemoveTopLock ();
		}

		void btnRemBottomLockClicked (object sender, EventArgs e)
		{
			try {
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();
				WBidHelper.PushToUndoStack();

				btnBottomLock.Enabled = true;
				LineOperations.RemoveAllBottomLock ();
				ReloadAllContent ();
				if (calendarWC != null)
					calendarWC.Window.Close ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}


		public void RemoveTopLock()
		{


			btnTopLock.Enabled = true;
			LineOperations.RemoveAllTopLock ();
			ReloadAllContent ();
			if (calendarWC != null)
				calendarWC.Window.Close ();


		}

		public void RemoveBottomLock()
		{
			btnBottomLock.Enabled = true;
			LineOperations.RemoveAllBottomLock ();
			summaryVC.LoadContent ();
			if (calendarWC != null)
				calendarWC.Window.Close ();
		}

		public void LockBtnEnableDisable () 
		{
			int none = 0;
			int top = 0;
			int bot = 0;
			foreach (var item in CommonClass.selectedRows) {
				if (!GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).TopLock && !GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).BotLock)
					none++;
				if (GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).TopLock)
					top++;
				if (GlobalSettings.Lines.FirstOrDefault (x => x.LineNum == item).BotLock)
					bot++;
			}
			if (none > 0) {
				btnTopLock.Enabled = true;
				btnBottomLock.Enabled = true;
			} else if (top > 0) {
				btnTopLock.Enabled = false;
				btnBottomLock.Enabled = true;
			} else if (bot > 0) {
				btnTopLock.Enabled = true;
				btnBottomLock.Enabled = false;
			} else {
				btnTopLock.Enabled = false;
				btnBottomLock.Enabled = false;
			}
		}
		public void RemoveBtnEnableDisable () 
		{
			btnRemTopLock.Enabled = (GlobalSettings.Lines.Count (x => x.TopLock) > 0);
			btnRemBottomLock.Enabled = (GlobalSettings.Lines.Count (x => x.BotLock) > 0);
			CommonClass.AppDelegate.menuRemoveToplock.Enabled = btnRemTopLock.Enabled ;
			CommonClass.AppDelegate.menuRemoveBottomLock.Enabled = btnRemBottomLock.Enabled ;


		}

		public void ResetAll()
		{

			StateManagement stateManagement = new StateManagement();
			stateManagement.UpdateWBidStateContent();
			WBidHelper.PushToUndoStack();

			var alert = new NSAlert ();
			alert.AlertStyle = NSAlertStyle.Critical;
			alert.MessageText = "Reset All";
			alert.InformativeText = "Do you want to Reset All?";
			alert.AddButton ("YES");
			alert.AddButton ("NO");
			alert.Buttons [0].Activated += (object sender, EventArgs e) => {
				ClearTopAndBottomLocks ();
				ClearConstraints ();
				ClearWeights ();
				ClearSort ();
				CommonClass.columnID = 0;
				CommonClass.MainController.ReloadAllContent ();
				if (CommonClass.CSWController != null) {
					CommonClass.CSWController.ReloadAllContent ();
				}
				alert.Window.Close();
				NSApplication.SharedApplication.StopModal();
			};
			alert.RunModal();
		}

		public void ClearConstraintsAndweights()
		{
			var alert = new NSAlert ();
			alert.AlertStyle = NSAlertStyle.Critical;
			alert.MessageText = "Reset Constraints and Weights";
			alert.InformativeText = "Do you want to Clear All Constraints and Weights";
			alert.AddButton ("YES");
			alert.AddButton ("NO");
			alert.Buttons [0].Activated += (object sender, EventArgs e) => {
				ClearConstraints ();
				ClearWeights ();
				CommonClass.MainController.ReloadAllContent ();
				if (CommonClass.CSWController != null) {
					CommonClass.CSWController.ReloadAllContent ();
				}
				alert.Window.Close();
				NSApplication.SharedApplication.StopModal();
			};
			alert.RunModal();

		}
		private void ClearTopAndBottomLocks()
		{
			GlobalSettings.Lines.ToList().ForEach(x => { x.TopLock = false; x.BotLock = false; });
		}

		public void ClearConstraints()
		{
			try {
				List<int> lstOff = new List<int> () { };
				
				List<int> lstWork = new List<int> () { };
				//var requiredLines = GlobalSettings.Lines.Where(x => !x.BlankLine);
				foreach (Line line in GlobalSettings.Lines) {
					line.ConstraintPoints.Reset ();
					line.Constrained = false;
				
				}
				
				WBidState currentState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				
				CxWtState states = currentState.CxWtState;
				
				currentState.Constraints.Hard = false;
				currentState.Constraints.Ready = false;
				currentState.Constraints.Reserve = false;
				currentState.Constraints.Blank = false;
				currentState.Constraints.International = false;
				currentState.Constraints.NonConus = false;
				
				currentState.CxWtState.AMPMMIX.AM = false;
				currentState.CxWtState.AMPMMIX.PM = false;
				currentState.CxWtState.AMPMMIX.MIX = false;
				
				currentState.CxWtState.FaPosition.A = false;
				currentState.CxWtState.FaPosition.B = false;
				currentState.CxWtState.FaPosition.C = false;
				currentState.CxWtState.FaPosition.D = false;
				
				currentState.CxWtState.TripLength.Turns = false;
				currentState.CxWtState.TripLength.Twoday = false;
				currentState.CxWtState.TripLength.ThreeDay = false;
				currentState.CxWtState.TripLength.FourDay = false;
				
				currentState.CxWtState.DaysOfWeek.MON = false;
				currentState.CxWtState.DaysOfWeek.TUE = false;
				currentState.CxWtState.DaysOfWeek.WED = false;
				currentState.CxWtState.DaysOfWeek.THU = false;
				currentState.CxWtState.DaysOfWeek.FRI = false;
				currentState.CxWtState.DaysOfWeek.SAT = false;
				currentState.CxWtState.DaysOfWeek.SUN = false;
				
				
				
				states.ACChg.Cx = false;
				//states.AMPM.Cx = false;
				states.BDO.Cx = false;
				states.CL.Cx = false;
				states.DHD.Cx = false;
				states.DHDFoL.Cx = false;
				states.DOW.Cx = false;
				states.DP.Cx = false;
				states.EQUIP.Cx = false;
				states.FLTMIN.Cx = false;
				states.GRD.Cx = false;
				states.InterConus.Cx = false;
				states.LEGS.Cx = false;
				states.LegsPerPairing.Cx = false;
				states.MP.Cx = false;
				states.No3on3off.Cx = false;
				states.NODO.Cx = false;
				states.NOL.Cx = false;
				states.PerDiem.Cx = false;
				states.Rest.Cx = false;
				states.RON.Cx = false;
				states.SDO.Cx = false;
				states.SDOW.Cx = false;
				states.TL.Cx = false;
				states.WB.Cx = false;
				states.WtPDOFS.Cx = false;
				states.LrgBlkDaysOff.Cx = false;
				states.Position.Cx = false;
				states.WorkDay.Cx = false;
				states.BulkOC.Cx = false;
				// states.InterCon.Cx = false;
				//Update the state object
				
				
				Constraints constraint = new Constraints () {
					Hard = false,
					Ready = false,
					Reserve = false,
					International = false,
					NonConus = false,
					// AM_PM = new AMPMConstriants{AM=false,PM=false,MIX=false},
					LrgBlkDayOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 10 },
					AircraftChanges = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
					BlockOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },
					DeadHeads = new Cx4Parameters {
						SecondcellValue = "1",
						ThirdcellValue = ((int)DeadheadType.First).ToString (),
						Type = (int)ConstraintType.LessThan,
						Value = 1,
						LstParameter = new List<Cx4Parameter> ()
					},
					CL = new CxCommutableLine () {
						AnyNight = true,
						RunBoth = false,
						CommuteToHome = true,
						CommuteToWork = true,
						MondayThu = new Times () { Checkin = 0, BackToBase = 0 },
						MondayThuDefault = new Times () { Checkin = 0, BackToBase = 0 },
						Friday = new Times () { Checkin = 0, BackToBase = 0 },
						FridayDefault = new Times () { Checkin = 0, BackToBase = 0 },
						Saturday = new Times () { Checkin = 0, BackToBase = 0 },
						SaturdayDefault = new Times () { Checkin = 0, BackToBase = 0 },
						Sunday = new Times () { Checkin = 0, BackToBase = 0 },
						SundayDefault = new Times () { Checkin = 0, BackToBase = 0 },
						TimesList = new List<Times> ()
				
					},
				
					DaysOfMonth = new DaysOfMonthCx () { OFFDays = lstOff, WorkDays = lstWork },
					DaysOfWeek = new Cx3Parameters () {
						ThirdcellValue = ((int)Dow.Tue).ToString (),
						Type = (int)ConstraintType.LessThan,
						Value = 1,
						lstParameters = new List<Cx3Parameter> ()
					},
					DeadHeadFoL = new Cx3Parameters {
						ThirdcellValue = ((int)DeadheadType.First).ToString (),
						Type = (int)ConstraintType.LessThan,
						Value = 1,
						lstParameters = new List<Cx3Parameter> ()
					},
					DutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 600 },
				
					EQUIP = new Cx3Parameters {
						ThirdcellValue = "500",
						Type = (int)ConstraintType.MoreThan,
						Value = 0,
						lstParameters = new List<Cx3Parameter> ()
					},
					FlightMin = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 7200 },
					GroundTime = new Cx3Parameter { Type = (int)ConstraintType.MoreThan, Value = 1, ThirdcellValue = "30" },
					InterConus = new Cx2Parameters () {
						Type = (int)CityType.International,
						Value = 1,
						lstParameters = new List<Cx2Parameter> ()
					},
					LegsPerDutyPeriod = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 4 },
					LegsPerPairing = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18 },
					NumberOfDaysOff = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 18 },
				
					OverNightCities = new Cx3Parameters {
						ThirdcellValue = "6",
						Type = (int)ConstraintType.LessThan,
						Value = 1,
						lstParameters = new List<Cx3Parameter> ()
					},
					BulkOvernightCity = new BulkOvernightCityCx { OverNightNo = new List<int> (), OverNightYes = new List<int> () },
					PDOFS = new Cx4Parameters {
						SecondcellValue = "1",
						ThirdcellValue = "1",
						Type = (int)ConstraintType.atafter,
						Value = 915,
						LstParameter = new List<Cx4Parameter> ()
					},
					Position = new Cx3Parameters {
						Type = (int)ConstraintType.LessThan,
						Value = 1,
						lstParameters = new List<Cx3Parameter> ()
					},
					StartDayOftheWeek = new Cx3Parameters {
						ThirdcellValue = "6",
						Type = (int)ConstraintType.MoreThan,
						Value = 3 ,
						lstParameters = new List<Cx3Parameter> ()
					},
					Rest = new Cx3Parameters {
						ThirdcellValue = "1",
						Type = (int)ConstraintType.LessThan,
						Value = 8,
						lstParameters = new List<Cx3Parameter> ()
					},
					PerDiem = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 18000 },
					TripLength = new Cx3Parameters {
						ThirdcellValue = "4",
						Type = (int)ConstraintType.MoreThan,
						Value = 1,
						lstParameters = new List<Cx3Parameter> ()
					},
					WorkBlockLength = new Cx3Parameters {
						ThirdcellValue = "4",
						Type = (int)ConstraintType.LessThan,
						Value = 2,
						lstParameters = new List<Cx3Parameter> ()
					},
					MinimumPay = new Cx2Parameter { Type = (int)ConstraintType.MoreThan, Value = 90 },
					No3On3Off = new Cx2Parameter { Type = (int)ThreeOnThreeOff.ThreeOnThreeOff, Value = 10 },
					WorkDay = new Cx2Parameter { Type = (int)ConstraintType.LessThan, Value = 11 },
				
				
				
				
				
				};
				
				currentState.Constraints = constraint;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void ClearWeights()
		{
			try {
				///Reset all weight values to zero.
				GlobalSettings.Lines.ToList ().ForEach (x => {
					x.WeightPoints.Reset ();
					x.TotWeight = 0.0m;
				});
				//ApplyLineOrderBasedOnWeight();
				
				WBidState currentState = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
				
				CxWtState states = currentState.CxWtState;
				
				
				states.ACChg.Wt = false;
				states.AMPM.Wt = false;
				states.BDO.Wt = false;
				states.CL.Wt = false;
				states.DHD.Wt = false;
				states.DOW.Wt = false;
				states.DHDFoL.Wt = false;
				states.DP.Wt = false;
				states.EQUIP.Wt = false;
				states.FLTMIN.Wt = false;
				states.GRD.Wt = false;
				states.InterConus.Wt = false;
				states.LrgBlkDaysOff.Wt = false;
				states.LEGS.Wt = false;
				states.LegsPerPairing.Wt = false;
				states.MP.Wt = false;
				states.No3on3off.Wt = false;
				states.NODO.Wt = false;
				states.NOL.Wt = false;
				states.PerDiem.Wt = false;
				states.PDAfter.Wt = false;
				states.PDBefore.Wt = false;
				states.Position.Wt = false;
				states.Rest.Wt = false;
				states.RON.Wt = false;
				states.SDO.Wt = false;
				states.SDOW.Wt = false;
				states.TL.Wt = false;
				states.WB.Wt = false;
				states.WorkDay.Wt = false;
				states.NormalizeDays.Wt = false;
				states.BulkOC.Wt = false;
				//Weights
				//----------------------------------------
				
				var oldclDeafult = currentState.Weights.CL.DefaultTimes;
				var weight = new Weights {
					AirCraftChanges = new Wt3Parameter { SecondlValue = 1, ThrirdCellValue = 1, Weight = 0 },
					//AMM,PM, Night
					AM_PM = new Wt2Parameters { Type = 1, Weight = 0, lstParameters = new List<Wt2Parameter> () },
					LrgBlkDayOff = new Wt2Parameter { Weight = 0 },
				
					BDO = new Wt3Parameters {
						SecondlValue = 1,
						ThrirdCellValue = 1,
						Weight = 0
							,
						lstParameters = new List<Wt3Parameter> ()
					},
					DHD = new Wt3Parameters {
						SecondlValue = 1,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt3Parameter> ()
					},
					//Commutable Line
					CL = new WtCommutableLine () {
						TimesList = new List<Times> () {
							new Times (){ Checkin = 0, BackToBase = 0 },
							new Times (){ Checkin = 0, BackToBase = 0 },
							new Times (){ Checkin = 0, BackToBase = 0 },
							new Times (){ Checkin = 0, BackToBase = 0 },
						},
						DefaultTimes = new List<Times> () {
							new Times (){ Checkin = 0, BackToBase = 0 },
							new Times (){ Checkin = 0, BackToBase = 0 },
							new Times (){ Checkin = 0, BackToBase = 0 },
							new Times (){ Checkin = 0, BackToBase = 0 },
						},
						BothEnds = 0,
						InDomicile = 0,
						Type = 1
						//1.  All 2. 
				
					},
				
				
					SDO = new DaysOfMonthWt () {
						isWork = false,
						Weights = new List<Wt> ()
					},
				
					DOW = new WtDaysOfWeek () {
						lstWeight = new List<Wt> () { new Wt () { Key = 0, Value = 0 } },
						IsOff = true
				
					},
					DP = new Wt3Parameters () {
						SecondlValue = 1,
						ThrirdCellValue = 300,
						Weight = 0
							,
						lstParameters = new List<Wt3Parameter> ()
					},
				
					EQUIP = new Wt3Parameters {
						SecondlValue = 300,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt3Parameter> ()
					},
				
					FLTMIN = new Wt3Parameters {
						SecondlValue = 0,
						ThrirdCellValue = 20,
						Weight = 0,
						lstParameters = new List<Wt3Parameter> ()
					},
				
					GRD = new Wt3Parameters () {
						SecondlValue = 0,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt3Parameter> ()
					},
					InterConus = new Wt2Parameters () {
						Type = -1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
					LEGS = new Wt3Parameters {
						SecondlValue = 1,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt3Parameter> ()
					},
					WtLegsPerPairing = new Wt3Parameters () {
						SecondlValue = 1,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt3Parameter> ()
					},
				
					NODO = new Wt2Parameters {
						Type = 9,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
					RON = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
				
					SDOW = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
					WtRest = new Wt4Parameters {
						FirstValue = 1,
						SecondlValue = 480,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt4Parameter> ()
					},
					PerDiem = new Wt2Parameter {
						Type = 100,
						Weight = 0
				
					},
					TL = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
					WB = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
					POS = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
					DHDFoL = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					},
					WorkDays = new Wt3Parameters {
						SecondlValue = 1,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt3Parameter> ()
					},
				
					PDAfter = new Wt4Parameters {
						FirstValue = 1,
						SecondlValue = 180,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt4Parameter> ()
					},
					PDBefore = new Wt4Parameters {
						FirstValue = 1,
						SecondlValue = 180,
						ThrirdCellValue = 1,
						Weight = 0,
						lstParameters = new List<Wt4Parameter> ()
					},
				
					NormalizeDaysOff = new Wt2Parameter { Type = 1, Weight = 0 },
				
				
					OvernightCitybulk = new List<Wt2Parameter> ()
				};
				
				currentState.Weights = weight;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private void ClearSort()
		{
			//SortCalculation sort = new SortCalculation();
			//sort.SortLines("Line");
			var state=GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			state.ForceLine.IsBlankLinetoBottom = false;
			state.ForceLine.IsReverseLinetoBottom = false;
			state.SortDetails.SortColumn = "Line";
			state.SortDetails.SortDirection = string.Empty;
		}


		void HandleViewSelect (object sender, EventArgs e)
		{
			try {
				ChangeView ();
				CommonClass.ViewChanged = true;
				ReloadAllContent ();
				CommonClass.ViewChanged = false;
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void btnBottomLockClicked (object sender, EventArgs e)
		{
			try {
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();
				WBidHelper.PushToUndoStack();

				LineOperations.TrashLines (CommonClass.selectedRows);
				ReloadAllContent ();
				CommonClass.selectedRows.Clear ();
				CommonClass.columnID = 0;
				if (calendarWC != null)
					calendarWC.Window.Close ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		void btnTopLockClicked (object sender, EventArgs e)
		{
			try {
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();
				WBidHelper.PushToUndoStack();

				LineOperations.PromoteLines (CommonClass.selectedRows);
				ReloadAllContent ();
				CommonClass.selectedRows.Clear ();
				CommonClass.columnID = 0;
				if (calendarWC != null)
					calendarWC.Window.Close ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		public void TopLock ()
		{
			try {
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();
				WBidHelper.PushToUndoStack();

				LineOperations.PromoteLines (CommonClass.selectedRows);
				//summaryVC.ReloadContent ();
				ReloadAllContent ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		public void BottomLock ()
		{
			try {
				StateManagement stateManagement = new StateManagement();
				stateManagement.UpdateWBidStateContent();
				WBidHelper.PushToUndoStack();

				LineOperations.TrashLines (CommonClass.selectedRows);
				//summaryVC.ReloadContent ();
				ReloadAllContent ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void ChangeView ()
		{
			try {
				foreach (var vw in vwMain.Subviews) {
					vw.RemoveFromSuperview ();
				}
				if (sgViewSelect.SelectedSegment == 0) {
					if (summaryVC == null) {
						summaryVC = new SummaryViewController ();
					}
					//this.Window.ContentView = summaryVC.View;
					summaryVC.View.Frame = vwMain.Bounds;
					vwMain.AddSubview (summaryVC.View);
					CommonClass.SummaryController = summaryVC;
				} else if (sgViewSelect.SelectedSegment == 1) {
					if (bidlineVC == null) {
						bidlineVC = new BidLineViewController ();
					}
					//this.Window.ContentView = bidlineVC.View;
					bidlineVC.View.Frame = vwMain.Bounds;
					vwMain.AddSubview (bidlineVC.View);
					CommonClass.BidLineController = bidlineVC;
				} else if (sgViewSelect.SelectedSegment == 2) {
					if (modernVC == null) {
						modernVC = new ModernViewController ();
					}
					//this.Window.ContentView = modernVC.View;
					modernVC.View.Frame = vwMain.Bounds;
					vwMain.AddSubview (modernVC.View);
					CommonClass.ModernController = modernVC;
				}
				if (calendarWC != null)
					calendarWC.Window.Close ();
				if (bidlineVC!=null&&bidlineVC.TripWC != null)
					bidlineVC.TripWC.Window.Close ();
				if (modernVC!=null&&modernVC.TripWC != null)
					modernVC.TripWC.Window.Close ();

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

		}

		public void ReloadAllContent ()
		{
			try {
				if (CommonClass.SummaryController != null)
					CommonClass.SummaryController.LoadContent ();
				if (CommonClass.BidLineController != null)
					CommonClass.BidLineController.ReloadContent ();
				if (CommonClass.ModernController != null)
					CommonClass.ModernController.ReloadContent ();
				LockBtnEnableDisable ();
				RemoveBtnEnableDisable ();
				if (!CommonClass.ViewChanged)
					UpdateSaveButton (true);
				UpdateUndoRedoButtons ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private static void SortLineList()
		{
			SortCalculation sort = new SortCalculation();
			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
			{
				sort.SortLines(wBidStateContent.SortDetails.SortColumn);
			}
		}

	}
}

