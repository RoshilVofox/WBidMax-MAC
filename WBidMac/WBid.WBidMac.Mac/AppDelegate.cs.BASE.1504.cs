using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.SharedLibrary.Parser;
using WBid.WBidiPad.SharedLibrary;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Serialization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WBid.WBidiPad.SharedLibrary.SWA;
using System.Net;
//using MiniZip.ZipArchive;
using System.ServiceModel;
//using WBidPushService.Model;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidMac.Mac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		//MainWindowController mainWindowController;
		HomeWindowController homeWindowController;
		SubmitWindowController submitController;
		bool isManualUpdate;

		WBidDataDwonloadAuthServiceClient client;
		public AppDelegate ()
		{
			BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
			client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
			client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
			client.GetLatestVersionByUserRoleForMultyPlatformCompleted += client_GetLatestVersionByUserRoleForMultyPlatformCompleted;
		}

		void client_GetLatestVersionByUserRoleForMultyPlatformCompleted(object sender, GetLatestVersionByUserRoleForMultyPlatformCompletedEventArgs e)
		{
			try {
				var version = e.Result.VersionNumber;
				var file = e.Result.FileName;
				if (!string.IsNullOrEmpty (version) && version != CommonClass.AppVersion) {
					InvokeOnMainThread (() => {
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "App Update Available!";
						alert.InformativeText = "A New Version " + version + " of WBidMax exists for Mac.";
						alert.AddButton ("Download Now");
						alert.AddButton ("Later");
						alert.Buttons [0].Activated += (object senderr, EventArgs ee) => {
							alert.Window.Close ();
							NSApplication.SharedApplication.StopModal ();
							DownloadNewINstallerFromServer();
						};
						alert.RunModal ();
					});
				} else if (isManualUpdate) {
					isManualUpdate = false;
					InvokeOnMainThread (() => {
						var alert = new NSAlert ();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "No Update Available!";
						alert.InformativeText = "You're using the Latest Verion of WBidMax";
						alert.RunModal ();
					});
				}
			} catch (Exception ex) {

			}
		}
		private void DownloadNewINstallerFromServer()
		{
		
			string url = GlobalSettings.installerDownloadUrl;
		
			WebClient client = new WebClient();

			//client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
			//string ss = tempPath;
			//string path=NSSearchPath.GetDirectories (NSSearchPathDirectory.DownloadsDirectory, NSSearchPathDomain., true).First ();
			client.DownloadFileAsync(new Uri(url),WBidHelper.GetAppDataPath() +"/wbidexe.zip");
			client.DownloadFileCompleted+= (object sender, System.ComponentModel.AsyncCompletedEventArgs e) => {
				if(e.Error==null)
				{
					//DownloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
					//
					//				ClientRequestModel clientRequestModel = new ClientRequestModel();
					//				clientRequestModel.OperatingSystem = GetOSFriendlyName();
					//				clientRequestModel.Platform = "PC";
					//				clientRequestModel.Version = MaintananceData.VersionNumber;
					//				clientRequestModel.EmployeeNumber = Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
					//				var result = DownloadAuthServiceClient.LogDownloadUpdates(clientRequestModel);
				}
			};
		}
//		private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
//		{
//			if (e.Error == null)
//			{
//				/*IsBusy = false;
//				DownloadAuthServiceClient = new WBidDataDwonloadAuthServiceClient("BasicHttpBinding_IWBidDataDwonloadAuthServiceForNormalTimout");
//
//				ClientRequestModel clientRequestModel = new ClientRequestModel();
//				clientRequestModel.OperatingSystem = GetOSFriendlyName();
//				clientRequestModel.Platform = "PC";
//				clientRequestModel.Version = MaintananceData.VersionNumber;
//				clientRequestModel.EmployeeNumber = Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
//				var result = DownloadAuthServiceClient.LogDownloadUpdates(clientRequestModel);
//
//
//
//				//((MainViewModel)ServiceLocator.Current.GetInstance<MainViewModel>()).IsStateModified = false;
//				InstallDownloadedSetup();
//				*/
//			}
//			else
//			{
//				//IsBusy = false;
//				//Xceed.Wpf.Toolkit.MessageBox.Show("Update failed..Please try again later..!", "WBidMax", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
//			}
//		}
		public override void FinishedLaunching (NSObject notification)
		{
			NSApplication.SharedApplication.Delegate = this;
			CommonClass.AppDelegate = this;
			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

			homeWindowController = new HomeWindowController ();
			CommonClass.HomeController = homeWindowController;
			homeWindowController.Window.MakeKeyAndOrderFront (this);
			CommonClass.isHomeWindow = true;

			setupMenu ();
			ReloadMenu ();

			LoadINIFileData();
			if (File.Exists (WBidHelper.WBidUserFilePath)) {
				GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
			}
			if (File.Exists (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log") && GlobalSettings.WBidINIContent.User.IsNeedCrashMail && GlobalSettings.WbidUserContent!=null && GlobalSettings.WbidUserContent.UserInformation!=null) {
				//Check internet wavailable
				if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {
					string content = System.IO.File.ReadAllText (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log");
					WBidMail wbidMail = new WBidMail ();
					wbidMail.SendCrashMail (content);
					File.Delete (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log");
				}
			}

			copyBundledFileToAppdata ("ColumnDefinitions.xml");

			if (GlobalSettings.WbidUserContent != null&&GlobalSettings.WbidUserContent.UserInformation != null) {
				if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {
					client.GetLatestVersionByUserRoleForMultyPlatformAsync (new WBidDataDownloadAuthorizationService.Model.EmployeeVersionDetails {
						EmpNum = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToLower().Replace("e","").Replace("x","")),
						Platform = CommonClass.Platform,
						Version = CommonClass.AppVersion
					});
				}
			}

		}

		void HandleUnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine (e.ExceptionObject.ToString ());
			var exception = (Exception)e.ExceptionObject;

			var submitResult=	GenerateErrorMessageFromException (exception);


			if (exception != null)
			{

				if (!Directory.Exists(WBidHelper.GetAppDataPath() + "/" + "Crash"))
				{
					Directory.CreateDirectory(WBidHelper.GetAppDataPath() + "/" + "Crash");
				}

				System.IO.File.AppendAllText(WBidHelper.GetAppDataPath() + "/Crash/" + "Crash.log", submitResult);
			}

		}


		public void ShowErrorMessage(string message)
		{
			var alert = new NSAlert ();
			alert.AlertStyle = NSAlertStyle.Critical;
			alert.Window.Title = "WBidMax";
			alert.MessageText = "Error";
			alert.InformativeText = message;
			alert.AddButton ("OK");
			alert.RunModal ();

		}


		public void ErrorLog(Exception exception)
		{

				// string submitResult = "\r\n\r\n\r\n Crash Report : \r\n\r\n\r\n" + "\r\n Date: " + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + "\r\n\r\n Device: " + UIDevice.CurrentDevice.LocalizedModel + "\r\n\r\n Crash Details: " + ex + "\r\n\r\n Data: " + currentBid + "\r\n\r\n" + " ******************************* \r\n";

				var submitResult=	GenerateErrorMessageFromException (exception);
				if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) 
				{
					WBidMail wbidMail = new WBidMail ();
					wbidMail.SendCrashMail (submitResult);
				}
			    else
				{

					if (!Directory.Exists (WBidHelper.GetAppDataPath () + "/" + "Crash")) {
						Directory.CreateDirectory (WBidHelper.GetAppDataPath () + "/" + "Crash");
					}

					System.IO.File.AppendAllText (WBidHelper.GetAppDataPath () + "/Crash/" + "Crash.log", submitResult);
				}

		}


		private string GenerateErrorMessageFromException(Exception exception)
	{
			string error = string.Empty;
		Console.WriteLine (exception.ToString ());

		string currentBid = FileOperations.ReadCurrentBidDetails(WBidHelper.GetAppDataPath() + "/CurrentDetails.txt");

		if (exception != null)
		{
			Exception InnerException = exception.InnerException;
			string message = exception.Message;
			string where = exception.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
			string source = exception.Source;

			if (InnerException != null)
			{
				if (InnerException.Message != null)
				{
					message = InnerException.Message;
				}

				if (InnerException.StackTrace != null)
				{
						where = InnerException.StackTrace.Split(new string[] { " at " }, 2, StringSplitOptions.None)[1];
				}

				source = InnerException.Source;

				if (InnerException.InnerException != null)
				{
					if (InnerException.InnerException.Message != null)
					{
						message += " -> " + InnerException.InnerException.Message;
					}

					if (InnerException.InnerException.StackTrace != null)
					{
							where += "\r\n\r\n -> " + InnerException.InnerException.StackTrace.Split(new string[] { " at " },
								2, StringSplitOptions.None)[1];
					}

					if (InnerException.InnerException.Source != null)
					{
						source += " -> " + InnerException.InnerException.Source;
					}
				}
			}

			if (where.Length > 1024)
			{
					where = where.Substring(0, 1024);
			}


			var submitResult = "\r\n WBidMax Error Details.\r\n\r\n Error  :  " + message + "\r\n\r\n Where  :  " + where + "\r\n\r\n Source   :  " + source + "\r\n\r\n Version : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\r\n\r\n Date  :" + DateTime.Now;
			submitResult += "\r\n\r\n Data :" + currentBid + "\r\n\r\n Device :" + "Mac" + "\r\n\r\n";
				error = submitResult;
			}

			return error;

	}


		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}


		public void ReloadMenu ()
		{
			if (!CommonClass.isHomeWindow) {
				menuBidStuff.Hidden = false;
				menuView.Hidden = false;
				menuCSW.Hidden = false;
				menuResetAll.Enabled = true;
				menuClearConstraints.Enabled = true;
				menuBuddyBid.Hidden = false;
				if(GlobalSettings.CurrentBidDetails.Postion=="FA")
					menuBuddyBid.Title = "Buddy Bid";
				else
					menuBuddyBid.Title = "Avoidance Bid";
			} else {
				menuBidStuff.Hidden = true;
				menuView.Hidden = true;
				menuCSW.Hidden = true;
				menuResetAll.Enabled = false;
				menuClearConstraints.Enabled = false;
				menuRemoveToplock.Enabled = false;
				menuRemoveBottomLock.Enabled = false;
				menuBuddyBid.Hidden = true;
			}
		}

		/// <summary>
		/// Copies the bundled XMLs to appdata.
		/// </summary>
		private static void copyBundledFileToAppdata (string fileName)
		{
			var sourcePath = Path.Combine(NSBundle.MainBundle.ResourcePath,fileName);
			var destinationPath = WBidHelper.GetAppDataPath () + "/" + fileName;
			try {
				//---copy only if file does not exist---
				if(File.Exists(destinationPath))
				{
					File.Delete(destinationPath);
				}
				if (!File.Exists(destinationPath))
				{
					File.Copy(sourcePath, destinationPath);

					if(File.Exists(destinationPath))
					{
						LoadColumnDefenitionData();
					}

				}  else {
					LoadColumnDefenitionData();
				}
			}  catch (Exception e) {
				Console.WriteLine(e.Message);
				throw e;
			}

		}

		private static void LoadColumnDefenitionData()
		{
			GlobalSettings.columndefinition = (List<ColumnDefinition>)XmlHelper.DeserializeFromXml<ColumnDefinitions>(WBidHelper.GetWBidColumnDefinitionFilePath());
		}

		/// <summary>
		/// Load/Read the INI file data from the app data folder.or Create the INI file if the INI file is not present in the app data folder.
		/// </summary>
		private static void LoadINIFileData()
		{
			if (!Directory.Exists(WBidHelper.GetAppDataPath()))
			{
				//create app data folder
				WBidHelper.CreateAppDataDirectory();
			}
			//cheCk the INI file is ceated or not.If not,create it.

			if (!File.Exists(WBidHelper.GetWBidINIFilePath()))
			{
				WBidINI wbidINI = WBidCollection.CreateINIFile();
				XmlHelper.SerializeToXml(wbidINI, WBidHelper.GetWBidINIFilePath());
			}

			if (!File.Exists(WBidHelper.GetWBidDWCFilePath()))
			{
				WBidIntialState WBidIntialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
				XmlHelper.SerializeToXml(WBidIntialState, WBidHelper.GetWBidDWCFilePath());
			}

			//WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
			//read the values of the INI file.
			GlobalSettings.WBidINIContent = XmlHelper.DeserializeFromXml<WBidINI>(WBidHelper.GetWBidINIFilePath());

			if (GlobalSettings.WBidINIContent.AmPmConfigure == null)
			{

				GlobalSettings.WBidINIContent.AmPmConfigure = new AmPmConfigure()
				{
					HowCalcAmPm = 1,
					AmPush = TimeSpan.FromHours(4),
					AmLand = TimeSpan.FromHours(19),
					PmPush = TimeSpan.FromHours(11),
					PmLand = TimeSpan.FromHours(2),
					NitePush = TimeSpan.FromHours(22),
					NiteLand = TimeSpan.FromHours(7),
					NumberOrPercentageCalc = 1,
					NumOpposites = 3,
					PctOpposities = 20

				};
			}
			if (GlobalSettings.WBidINIContent.Version == null)
			{
				GlobalSettings.WBidINIContent.Version = "1.0";
				GlobalSettings.WBidINIContent.Updates = new INIUpdates { Trips = 0, News = 0, Cities = 0, Hotels = 0, Domiciles = 0, Equipment = 0, EquipTypes = 0 };
				XmlHelper.SerializeToXml(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());
			}
			if (GlobalSettings.WBidINIContent.Data == null)
			{
				GlobalSettings.WBidINIContent.Data = new Data();
				GlobalSettings.WBidINIContent.Data.IsCompanyData = true;
			}
			if (GlobalSettings.WBidINIContent.User == null)
			{
				GlobalSettings.WBidINIContent.User = new User() { IsNeedBidReceipt= true, SmartSynch = false, AutoSave = false,IsNeedCrashMail=true };
			}

			if (File.Exists(WBidHelper.WBidUserFilePath))
			{
				GlobalSettings.WbidUserContent = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser> (WBidHelper.WBidUserFilePath);
			}

		}

		#region MenuItems

		private void setupMenu ()
		{
			menuNewBid.Activated += HandleNewBid;
			menuSubmit.Activated += HandleSubmit;
			menuBidEditor.Activated += HandleBidEditor;
			menuGetAwards.Activated += HandleGetAwards;
			menuBidReceipt.Activated += HandleBidReceipt;
			menuConfigure.Activated += HandleConfigure;
			menuSummaryView.Activated += HandleViewChange;
			menuBidLineView.Activated += HandleViewChange;
			menuModernView.Activated += HandleViewChange;
			menuCSW.Activated += HandleCSW;
			menuLatestNews.Activated += HandleLatestNews;
			menuChangeUserInfo.Activated+= HandleChangeUserInfo;
			menuResetAll.Activated+= HandleResetAll;
			menuClearConstraints.Activated+= HandleClearConstrints; 
			menuRemoveToplock.Activated+= HandleRemoveTopLock;
			menuRemoveBottomLock.Activated+= HandleRemoveBottomLock;
			menuClearTags.Enabled = false;
			menuExportToCalendar.Enabled = false;
			menuCoverLetter.Activated+= HandleViewFile;
			menuSeniorityList.Activated+= HandleViewFile;
			menuViewAwards.Activated+= HandleViewFile;
			menuBuddyBid.Activated += HandleBuddyBid;
			menuCheckUpdate.Activated += HandleCheckUpdate;
		}

		void HandleCheckUpdate (object sender, EventArgs e)
		{
			isManualUpdate = true;
			if (Reachability.IsHostReachable (GlobalSettings.ServerUrl)) {
				client.GetLatestVersionByUserRoleForMultyPlatformAsync (new WBidDataDownloadAuthorizationService.Model.EmployeeVersionDetails {
					EmpNum = int.Parse(GlobalSettings.WbidUserContent.UserInformation.EmpNo.ToLower().Replace("e","").Replace("x","")),
					Platform = CommonClass.Platform,
					Version = CommonClass.AppVersion
				});
			}
		}

		void HandleBuddyBid (object sender, EventArgs e)
		{
			if (GlobalSettings.CurrentBidDetails.Postion == "FA") {
				var panel = new NSPanel ();
				var changeBud = new ChangeBuddyViewController ();
				CommonClass.Panel = panel;
				panel.SetContentSize (new System.Drawing.SizeF (400, 190));
				panel.ContentView = changeBud.View;
				NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
			} else {
				var panel = new NSPanel ();
				var changeAvoid = new ChangeAvoidanceViewController ();
				CommonClass.Panel = panel;
				panel.SetContentSize (new System.Drawing.SizeF (400, 270));
				panel.ContentView = changeAvoid.View;
				NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
			}
		}



		void HandleClearConstrints (object sender, EventArgs e)
		{
			CommonClass.MainController.ClearConstraintsAndweights ();
		}

		void HandleChangeUserInfo (object sender, EventArgs e)
		{
			var userReg = new UserRegistrationWindowController ();
			userReg.IsEditMode = true;
//			CommonClass.MainController.Window.AddChildWindow (userReg.Window,NSWindowOrderingMode.Above);
			userReg.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (userReg.Window);
			
		}

		void HandleResetAll (object sender, EventArgs e)
		{
			CommonClass.MainController.ResetAll ();
		}

		void HandleRemoveTopLock (object sender, EventArgs e)
		{
			CommonClass.MainController.RemoveTopLock ();
		}

		void HandleRemoveBottomLock (object sender, EventArgs e)
		{
			CommonClass.MainController.RemoveBottomLock ();

		}

		void HandleLatestNews (object sender, EventArgs e)
		{
			var newsPath = WBidHelper.GetAppDataPath () + "/news.pdf";
			if (File.Exists (newsPath)) {
				var fileViewer = new FileWindowController ();
				fileViewer.Window.Title = "Latest News";
				fileViewer.LoadPDF ("news.pdf");
				CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
				fileViewer.Window.MakeKeyAndOrderFront (this);
			} else {
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Latest News";
				alert.InformativeText = "There are no Latest News available..!";
				alert.AddButton ("OK");
				alert.RunModal ();
			}
		}		

		void HandleCSW (object sender, EventArgs e)
		{
			CommonClass.MainController.ShowCSW ();
		}

		void HandleViewChange (object sender, EventArgs e)
		{
			var m = (NSMenuItem)sender;
			CommonClass.MainController.ToogleView (m.Tag);
		}

		void HandleConfigure (object sender, EventArgs e)
		{
			var config = new ConfigurationWindowController ();
			//CommonClass.HomeController.Window.AddChildWindow (config.Window,NSWindowOrderingMode.Above);
			config.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (config.Window);
		}

		void HandleBidReceipt (object sender, EventArgs e)
		{
			string path = WBidHelper.GetAppDataPath();
			List<string> filenames = Directory.EnumerateFiles(path, "*.RCT", SearchOption.AllDirectories).Select(Path.GetFileName).ToList();
			if (filenames.Count > 1) 
			{
				//show separate controller
				var panel = new NSPanel ();
				var receiptView = new BidReceiptViewController ();
				receiptView.fileNames = filenames;
				CommonClass.Panel = panel;
				panel.SetContentSize (new System.Drawing.SizeF (300, 350));
				panel.ContentView = receiptView.View;
				NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);

			} else if (filenames.Count == 1) {
				InvokeOnMainThread (() => {
					var fileViewer = new FileWindowController ();
					fileViewer.Window.Title = "Bid Receipt";
					fileViewer.LoadTXT (filenames[0]);
					CommonClass.MainController.Window.AddChildWindow (fileViewer.Window, NSWindowOrderingMode.Above);
					fileViewer.Window.MakeKeyAndOrderFront (this);
				});
			} else 
			{
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Informational;
				alert.Window.Title = "WBidMax";
				alert.MessageText = "Bid Receipt";
				alert.InformativeText = "There is no bid reciept available..!";
				alert.AddButton ("OK");
				alert.RunModal ();

				//UIAlertView alert = new UIAlertView("WBidMax", "There is no bid reciept avaialbe..!", null, "OK", null);
				//alert.Show();
			}

		}

		void HandleGetAwards (object sender, EventArgs e)
		{
			var getAwards = new GetAwardsWindowController ();
			CommonClass.MainController.Window.AddChildWindow (getAwards.Window,NSWindowOrderingMode.Above);
			getAwards.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (getAwards.Window);
		}

		void HandleViewFile (object sender, EventArgs e)
		{


			var viewFile = new ViewFileWindowController ();
			viewFile.ViewType = ((NSMenuItem)sender).Tag;
			CommonClass.MainController.Window.AddChildWindow (viewFile.Window,NSWindowOrderingMode.Above);
			viewFile.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (viewFile.Window);
		}

		void HandleBidEditor (object sender, EventArgs e)
		{
			var bidEdit = new BidEditorPrepWindowController ();
			CommonClass.MainController.Window.AddChildWindow (bidEdit.Window,NSWindowOrderingMode.Above);
			bidEdit.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (bidEdit.Window);
		}

		void HandleSubmit (object sender, EventArgs e)
		{
			if (submitController == null)
				submitController = new SubmitWindowController ();
			else
				submitController.SetUpView ();
			CommonClass.MainController.Window.AddChildWindow (submitController.Window,NSWindowOrderingMode.Above);
			submitController.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (submitController.Window);
		}

		void HandleNewBid (object sender, EventArgs e)
		{

		}

		#endregion
	}
}

