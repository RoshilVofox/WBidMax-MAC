
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
//using MonoTouch.CoreGraphics;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidMac.Mac.Utility;
using WBid.WBidMac.Mac.WindowControllers.initialLogin;
using WBid.WBidMac.Mac.WindowControllers.LoginNew;
using WBid.WBidMac.Mac.WindowControllers.UserAccoutDifference;
using WBid.WBidMac.Mac.WindowControllers.UserLogin;
using WBid.WBidMac.Mac.WindowControllers.UserUpdateInfo;

namespace WBid.WBidMac.Mac
{
    public partial class HomeWindowController : AppKit.NSWindowController
	{
		NewBidWindowController newBidWindowController;
		MainWindowController mainController;
		NSObject notif;
        NSObject ReloadHomeNotif;
        WbidUser User;
		List<RecentFile> recentFiles;
		WBidDataDwonloadAuthServiceClient client;
		bool isEdit;

		#region Constructors

		// Called when created from unmanaged code
		public HomeWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		//	GlobalSettings.WBidINIContent.
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public HomeWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public HomeWindowController () : base ("HomeWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new HomeWindow Window {
			get {
				return (HomeWindow)base.Window;
			}
		}

		public override void AwakeFromNib ()
		{
			
			try {
				Console.WriteLine("Position" + this.Window.Frame.X);
				base.AwakeFromNib ();

				bool IsInternetAvailable = Reachability.CheckVPSAvailable();

				var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");

                if (interfaceStyle == "Dark") {

                    lblBidName.TextColor = NSColor.Black;
                    lblMonthYear.TextColor = NSColor.Black;

                }

                //CommonClass.FormatBidReceipt2();
                //				btnRetrieve.RemoveAllItems();
                //				btnRetrieve.AddItems(new string[]{"Retrieve","New Bid Period","Historical Bid Period"});
                //				var servermode=string.Empty;
                //				if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.IsConnectedVofox)
                //					servermode = " (Vofox server)";
                //				else
                //					servermode = string.Empty;

                //this.Window.Title="WBidMax - Home ("+NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString ()+")";

                this.Window.Title = "WBidMax - Home (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
				this.ShouldCascadeWindows = false;
				
				btnNewBid.Activated += HandleBtnNewBidTapped;
				btnCollection.Activated += HandleBtnCollection;
				btnCollection.IgnoresMultiClick = true;
				btnRetrieve.Activated += HandleBtnRetrieve;

				LineSummaryBL.GetAdditionalColumns ();

				LineSummaryBL.GetAdditionalVacationColumns ();

				// Saving additional bidline colunmns.
				LineSummaryBL.GetBidlineViewAdditionalColumns ();

				// Saving additional modern colunmns.
				LineSummaryBL.GetModernViewAdditionalColumns ();

				LineSummaryBL.GetBidlineViewAdditionalVacationColumns ();

				// Saving additional modern colunmns.
				LineSummaryBL.GetModernViewAdditionalVacationalColumns ();

				LineSummaryBL.SetSelectedBidLineColumnstoGlobalList ();

				LineSummaryBL.SetSelectedModernBidLineColumnstoGlobalList ();

				LineSummaryBL.SetSelectedBidLineVacationColumnstoGlobalList ();

				LineSummaryBL.SetSelectedModernBidLineVacationColumnstoGlobalList ();
				
				if (!File.Exists (WBidHelper.WBidUserFilePath)) {
					//this.PerformSelector (new ObjCRuntime.Selector ("ShowUserRegistration"), null, 0.5);
					

					this.PerformSelector (new ObjCRuntime.Selector ("ShowInitialLogin"), null, 0.5);
					btnEdit.Hidden = true;
				}
				else {

					try
					{
						//Read user file content
						User = (WbidUser)XmlHelper.DeserializeFromXml<WbidUser>(WBidHelper.WBidUserFilePath);
						GlobalSettings.WbidUserContent = User;
						
						
						
						//recentFiles = GlobalSettings.WbidUserContent.RecentFiles;

						if (GlobalSettings.WbidUserContent != null)
						{
							if (GlobalSettings.WbidUserContent.IsVFXServer)
								GlobalSettings.isVFXServer = true;
							if (!GlobalSettings.WbidUserContent.isUserFromDB)
							{
								//Existing user.( User have local user account but it is not connected with server Db) We need to ask them to enter theier SWA credential only first time
								this.PerformSelector(new ObjCRuntime.Selector("ShowInitialLogin"), null, 0.5);
							}
							else
							{
								bool isConnectionAvailable = Reachability.CheckVPSAvailable();
								if (isConnectionAvailable)
								{
									//Normal process. User have local user account and is already connected with DB.

									//string empNo = Regex.Replace(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "[^0-9]+", string.Empty);
									//BasicHttpBinding binding = ServiceUtils.CreateBasicHttp();
									//client = new WBidDataDwonloadAuthServiceClient(binding, ServiceUtils.EndPoint);
									//client.InnerChannel.OperationTimeout = new TimeSpan(0, 0, 30);
									//client.GetEmployeeDetailsCompleted += client_GetEmployeeDetailsAsyncCompleted;
									//client.GetEmployeeDetailsAsync(empNo, "4");

									string empNo = Regex.Replace(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "[^0-9]+", string.Empty);
									string url = "GetEmployeeDetails/" + empNo + "/4";
									StreamReader dr = ServiceUtils.GetRestData(url);
									WBidDataDownloadAuthorizationService.Model.UserInformation responseData = ServiceUtils.ConvertJSonToObject<WBidDataDownloadAuthorizationService.Model.UserInformation>(dr.ReadToEnd());
									checkUserDetails(responseData);

								}
								else 
								{
									LoadContent();
								}
							}
						}
					}
					catch (Exception ex)
					{
						CommonClass.AppDelegate.ErrorLog(ex);
						CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
					}



					
				}



                btnEdit.Activated += (object sender, EventArgs e) => {
					isEdit = (btnEdit.State == NSCellStateValue.On);
					SetEditButtonTitle ();
					LoadContent ();
				};
				btnDeleteBid.Activated += HandleDeleteBid;
				//+= (object sender, EventArgs e) => {
				//int ind = int.Parse (((NSButton)sender).AlternateTitle);
				//};

//				if (System.IO.File.Exists (WBidHelper.HistoricalFilesInfoPath))
//				{
//					File.Delete (WBidHelper.HistoricalFilesInfoPath);
//				}
				if (!System.IO.File.Exists (WBidHelper.HistoricalFilesInfoPath))
				{
                    if (IsInternetAvailable) 
						{
						BasicHttpBinding binding = ServiceUtils.CreateBasicHttp ();
				client = new WBidDataDwonloadAuthServiceClient (binding, ServiceUtils.EndPoint);
				client.InnerChannel.OperationTimeout = new TimeSpan (0, 0, 30);
				client.GetAvailableHistoricalListCompleted+= Client_GetAvailableHistoricalListCompleted;
					client.GetAvailableHistoricalListAsync();
						}
				}
				if (IsInternetAvailable)
				{
					GetApplicationLoadDataFromServer();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}

//			BWI - CP - 2nd - Jan
//			scrap(e22028, Vofox2013-9, pairingwHasNoDetails, 1, 2015, 60, 30);

//			var trp = new List<string> () {
//				"BP1230",
//				"BP1601",
//				"BP1201",
//				"BP1220",
//				"BP1101",
//				"BP1L01",
//				"BP1701",
//				"BP1402",
//				"BP1501",
//				"BP1227",
//				"BP1702"
//			};
//			var scrap = new ScrapViewController ("e22028", "Vofox2013-9", trp, 1, 2015, 60, 30);
//			//scrap.View.Hidden = true;
//			this.Window.ContentView.AddSubview (scrap.View);
		}
		/// <summary>
        /// get button enable/disable or get settings from the database
        /// </summary>
		private void GetApplicationLoadDataFromServer()
		{

			ApplicationData info = new ApplicationData();
			info.FromApp = (int)FromApp.WbidmaxMACApp;
			var jsonData = ServiceUtils.JsonSerializer(info);
			StreamReader dr = ServiceUtils.GetRestData("GetApplicationLoadDatas", jsonData);
			ApplicationLoadData appLoadData = WBidCollection.ConvertJSonStringToObject<ApplicationLoadData>(dr.ReadToEnd());
			GlobalSettings.IsNeedToEnableVacDiffButton = appLoadData.IsNeedtoEnableVacationDifference;
			GlobalSettings.ServerFlightDataVersion = appLoadData.FlightDataVersion;
		}

		private void checkUserDetails(WBidDataDownloadAuthorizationService.Model.UserInformation userData)
		{
			try
			{
				//if (e.Result != null && e.Result.EmpNum != 0 && e.Result.EmpNum.ToString() != string.Empty)
				if (userData != null &&  userData.EmpNum != 0 && userData.EmpNum.ToString() != string.Empty)
				{
					GlobalSettings.WbidUserContent.UserInformation.IsFree = userData.IsFree;
					GlobalSettings.WbidUserContent.UserInformation.PaidUntilDate = userData.WBExpirationDate ?? DateTime.MinValue;
					GlobalSettings.WbidUserContent.UserInformation.IsMonthlySubscribed = userData.IsMonthlySubscribed;
					GlobalSettings.WbidUserContent.UserInformation.IsYearlySubscribed = userData.IsYearlySubscribed;
					GlobalSettings.WbidUserContent.UserInformation.SecondSubscriptionLine = userData.SecondSubscriptionLine;
					GlobalSettings.WbidUserContent.UserInformation.TopSubscriptionLine = userData.TopSubscriptionLine;
					GlobalSettings.WbidUserContent.UserInformation.ThirdSubscriptionLine = userData.ThirdSubscriptionLine;
					GlobalSettings.WbidUserContent.UserAccountDateTimeField = userData.UserAccountDateTime;
					var diffList = WBidHelper.CheckUserInformations(userData);
					
					if (diffList.Count > 0)
					{
						InvokeOnMainThread(() =>
						{
							var userDiff = new UserAccountDifferenceController();
							userDiff.userDiffList = diffList;
							this.Window.AddChildWindow(userDiff.Window, NSWindowOrderingMode.Above);
							userDiff.Window.MakeKeyAndOrderFront(this);
							NSApplication.SharedApplication.RunModalForWindow(userDiff.Window);
						});

					}
					else
					{
						InvokeOnMainThread(() =>
						{

							LoadContent();
						});

					}
				}
				else
				{
					this.PerformSelector(new ObjCRuntime.Selector("ShowUserRegistration"), null, 0.5);
				}
			}
			catch(Exception ex)
            {

            }
		}
		void setTitle()
		{
			var servermode = string.Empty;
			if (GlobalSettings.WBidINIContent != null && GlobalSettings.WBidINIContent.IsConnectedVofox)
				servermode = " (Vofox server)";
			else
				servermode = string.Empty;

			//this.Window.Title="WBidMax - Home ("+NSBundle.MainBundle.InfoDictionary [new NSString ("CFBundleShortVersionString")].ToString ()+")"+servermode;
            this.Window.Title = "WBidMax - Home (" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")"+ servermode;;
		}
		void Client_GetAvailableHistoricalListCompleted (object sender, GetAvailableHistoricalListCompletedEventArgs e)
		{try{
			if (e.Result != null) {

				List<WBidDataDownloadAuthorizationService.Model.BidData> lstBid = e.Result.ToList ();
				List<HistoricalBidData> lstHistoricalData = new List<HistoricalBidData> ();
				foreach (var item in lstBid) {
					lstHistoricalData.Add (new HistoricalBidData {Domicile = item.Domicile, Month = item.Month, Position = item.Position, Round = item.Round
							, Year = item.Year
					});
				}

					var previousfile = Directory.EnumerateFiles(WBidHelper.GetAppDataPath(), "*.HST", SearchOption.AllDirectories).Select(Path.GetFileName);
					if (previousfile != null && previousfile.Count()>0)
					{
						foreach (var item in previousfile)
						{
							File.Delete(WBidHelper.GetAppDataPath() + "/" + item);
						}
					}

				var stream = File.Create (WBidHelper.HistoricalFilesInfoPath);
				ProtoSerailizer.SerializeObject (WBidHelper.HistoricalFilesInfoPath, lstHistoricalData, stream);
				stream.Dispose ();
				stream.Close ();
			}
			}catch(Exception ex){
			}
		}


		void SetEditButtonTitle ()
		{
			if (isEdit) {
				btnEdit.SetTitleWithMnemonic ("Done");

			} else {
				btnEdit.SetTitleWithMnemonic ("Edit");

			}
		}

		void HandleDeleteBid (object sender, EventArgs e)
		{
			try {
				int ind = int.Parse (((NSButton)sender).AlternateTitle);
				RecentFile fileTodelete = recentFiles [ind];
				string message = fileTodelete.MonthDisplay + " " + fileTodelete.Year + "-" + fileTodelete.Domcile + "-" + fileTodelete.Position + "-" + fileTodelete.Round + "\nDo you want to delete this Bid?";
				var alert = new NSAlert ();
				alert.AlertStyle = NSAlertStyle.Warning;
				alert.MessageText = "Delete";
				alert.InformativeText = message;
				alert.AddButton ("YES");
				alert.AddButton ("NO");
				alert.Buttons [0].Activated += (object sender1, EventArgs e1) => {
					DeleteBidPeriod (fileTodelete.Domcile, fileTodelete.Position, fileTodelete.Round, fileTodelete.Month, Convert.ToInt32 (fileTodelete.Year));
					recentFiles.Remove (fileTodelete);
					LoadContent ();
					alert.Window.Close ();
					NSApplication.SharedApplication.StopModal ();
				};
				alert.RunModal ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
			
		}

		public void LoadContent ()
		{
			try {
				recentFiles = GetExistingDataInAppData ();
				var objects = arrBidList.ArrangedObjects ();
				arrBidList.Remove (objects);
				int index = 0;
				foreach (var item in recentFiles) {
					var bid = new BidListClass ();
					bid.Date = item.MonthDisplay + " " + item.Year;
					bid.Name = item.Domcile + "-" + item.Position + "-" + item.Round;
					bid.Index = index.ToString ();
					bid.Show = !isEdit;
					if (GlobalSettings.WbidUserContent.UserInformation.IsFemale) {
						switch (recentFiles [index].Position) {
						case "CP":
							bid.Image = NSImage.ImageNamed ("cpBgWomen.png");
							break;
						case "FO":
							bid.Image = NSImage.ImageNamed ("foBgWomen.png");
							break;
						case "FA":
							bid.Image = NSImage.ImageNamed ("faBgWomen.png");
							break;
						}
					} else {
						switch (recentFiles [index].Position) {
						case "CP":
							bid.Image = NSImage.ImageNamed ("cpBgMen.png");
							break;
						case "FO":
							bid.Image = NSImage.ImageNamed ("foBgMen.png");
							break;
						case "FA":
							bid.Image = NSImage.ImageNamed ("faBgMen.png");
							break;
						}
					}
				
					arrBidList.AddObject (bid);
					index++;
				}
				
				
				btnEdit.Hidden = (recentFiles.Count == 0);
				vwHomeCollection.ScrollRectToVisible (CoreGraphics.CGRect.Empty);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		private static List<RecentFile> GetExistingDataInAppData ()
		{
			string path = WBidHelper.GetAppDataPath ();
			List<RecentFile> lstRecentFiles = new RecentFiles ();
			//get all the  files in the root folder(look for wbl)
			List<string> linefilenames = Directory.EnumerateFiles (path, "*.*", SearchOption.AllDirectories).Select (Path.GetFileName)
				.Where (s => s.ToLower ().EndsWith (".wbl")).ToList ();
			foreach (string filenames in linefilenames) {
				string filename = filenames.Substring (0, filenames.Length - 3);
				if (filenames.Length < 17)
					continue;
				// if (File.Exists(path + "/" + filenames + ".WBP") && File.Exists(path + "/" + filenames + ".WBS"))
				//temporary code.In future we need to check the WBS file also
				if (File.Exists (path + "/" + filename + "WBP")) {
					System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo ();
					RecentFile recentfile = new RecentFile ();
					recentfile.Domcile = filename.Substring (0, 3);
					recentfile.Position = filename.Substring (3, 2);
					recentfile.Month = Convert.ToInt32 (filename.Substring (5, 2));
					recentfile.MonthDisplay = mfi.GetMonthName (Convert.ToInt32 (filename.Substring (5, 2))).Substring (0, 3).ToUpper ();
					recentfile.Year = (Convert.ToInt16 (filename.Substring (7, 2)) + 2000).ToString ();
					recentfile.Round = (filename.Substring (9, 1) == "M") ? "1st Round" : "2nd Round";
					lstRecentFiles.Add (recentfile);
				}
			}
			lstRecentFiles = lstRecentFiles.OrderByDescending (x => x.Year).ThenByDescending (y => y.Month).ThenByDescending (z => z.Round).ThenBy (a => a.Domcile).ToList ();
			return lstRecentFiles;

		}

		private void DeleteBidPeriod (string domcile, string position, string round, int bidperiod, int year)
		{
			try {
				//domicle==bwi
				//position==Cp
				//round=D
				//bidperiod=1

				//string message = "Are you sure you want to delete the" + currentOpenBid + " WBid data files \nfor " + SelectedDomicile.DomicileName + " " + SelectedPosition.LongStr + " " + SelectedEquipment.EquipmentNumber.ToString() + " ";
				// message += SelectedBidRound.RoundDescription + " for " + SelectedBidPeriod.Period + " ?";

				//if (MessageBox.Show(message, "WBidMax", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Information) == System.Windows.MessageBoxResult.Yes)
				// {
				string fileName = domcile + position + bidperiod.ToString ("d2") + (year - 2000) + (round == "1st Round" ? "M" : "S") + "737";
				string CommutefileName = domcile + position + bidperiod.ToString("d2") + (year - 2000) + (round == "1st Round" ? "M" : "S") + "Cmt.COM";
				
				string folderName = WBidCollection.GetPositions ().FirstOrDefault (x => x.LongStr == fileName.Substring (3, 2)).ShortStr + (round == "1st Round" ? "D" : "B") + fileName.Substring (0, 3) + WBidCollection.GetBidPeriods ().FirstOrDefault (x => x.BidPeriodId == bidperiod).HexaValue;
				//Delete WBL file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBL")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBL");

				}

				//Delete WBP file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBP")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBP");

				}

				//Delete WBS file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBS")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".WBS");

				}

				//Delete VAC file
				if (File.Exists (WBidHelper.GetAppDataPath () + "/" + fileName + ".VAC")) {
					File.Delete (WBidHelper.GetAppDataPath () + "/" + fileName + ".VAC");

				}
				//delete the folder.
				if (Directory.Exists (WBidHelper.GetAppDataPath () + "/" + folderName)) {
					Directory.Delete (WBidHelper.GetAppDataPath () + "/" + folderName, true);
				}
				if (Directory.Exists(WBidHelper.GetAppDataPath() + "/" + folderName))
				{
					Directory.Delete(WBidHelper.GetAppDataPath() + "/" + folderName, true);
				}
				//Delete Commute file
				if (File.Exists(WBidHelper.GetAppDataPath() + "/" + CommutefileName))
				{
					File.Delete(WBidHelper.GetAppDataPath() + "/" + CommutefileName);

				}

			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		[Export ("ShowUserRegistration")]
		void ShowUserRegistration ()
		{
			try {
				//var userReg = new UserRegistrationWindowController ();
				//userReg.Window.StandardWindowButton (NSWindowButton.CloseButton).Enabled = false;
				//this.Window.AddChildWindow (userReg.Window, NSWindowOrderingMode.Above);
				//userReg.Window.MakeKeyAndOrderFront (this);
				//NSApplication.SharedApplication.RunModalForWindow (userReg.Window);
				var userUpdate = new UserUpdateInfoController();
				userUpdate.title = "User Registration";
				userUpdate.buttonName = "Register";
				userUpdate.isRegister = true;
				userUpdate.Window.StandardWindowButton(NSWindowButton.CloseButton).Enabled = false;
				this.Window.AddChildWindow(userUpdate.Window, NSWindowOrderingMode.Above);
				userUpdate.Window.MakeKeyAndOrderFront(this);
				NSApplication.SharedApplication.RunModalForWindow(userUpdate.Window);
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
		[Export("ShowInitialLogin")]
		void ShowInitialLogin()
		{
			try
			{

				
				var userInitialLogin = new NewLoginWindowController();
				userInitialLogin.isUserXmlFileFound = false;
               //userInitialLogin.Window.StandardWindowButton(NSWindowButton.CloseButton).Enabled = false;
                this.Window.AddChildWindow(userInitialLogin.Window, NSWindowOrderingMode.Above);
                userInitialLogin.Window.MakeKeyAndOrderFront(this);
                NSApplication.SharedApplication.RunModalForWindow(userInitialLogin.Window);


            }
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			}
		}
		void HandleBtnRetrieve (object sender, EventArgs e)
		{
			try {
				if (btnRetrieve.SelectedTag == 0) {
					GlobalSettings.isHistorical = false;
					ShowBidOptionsWindow ();
				} else {
					var alert = new NSAlert();
					alert.Window.Title = "WBidMax";
					alert.MessageText = "Historical Bid Period";
					alert.InformativeText = "When viewing Historical Bid Data, Vacation Correction, EOM and MIL Corrections will not be available.\n\nNor will you be able to accidentally submit a bid using the Historical Bid Data.";
					alert.AddButton("OK");
					alert.Buttons[0].Activated += delegate {
						alert.Window.Close ();
						NSApplication.SharedApplication.StopModal ();
						GlobalSettings.isHistorical = true;
						ShowBidOptionsWindow ();
					};
					alert.RunModal();
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void ShowBidOptionsWindow ()
		{
			if (isEdit) {
				btnEdit.State = NSCellStateValue.Off;
				isEdit = false;
				SetEditButtonTitle ();
				LoadContent ();
			}
			if (notif == null)
				notif = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"StartBidDownload", HandleBidDownload);
			newBidWindowController = new NewBidWindowController ();
			this.Window.AddChildWindow (newBidWindowController.Window, NSWindowOrderingMode.Above);
			newBidWindowController.Window.MakeKeyAndOrderFront (this);
			NSApplication.SharedApplication.RunModalForWindow (newBidWindowController.Window);
		}

		void HandleBtnNewBidTapped (object sender, EventArgs e)
		{
			try {
				ShowBidOptionsWindow ();
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}

		void HandleBtnCollection (object sender, EventArgs e)
		{
			if (isEdit) {
				btnEdit.State = NSCellStateValue.Off;
				isEdit = false;
				SetEditButtonTitle ();
				LoadContent ();
			}

			int ind = int.Parse (((NSButton)sender).Title);
			var file = recentFiles [ind];
			string text = "Loading " + file.Domcile + "-" + file.Position + "-" + file.Round + " "	+ file.MonthDisplay + " " + file.Year + "\n Please Wait..";
			var panel = new NSPanel ();
			panel.SetContentSize (new CoreGraphics.CGSize (400, 120));
			var overlay = new OverlayViewController ();
			overlay.OverlayText = text;
			panel.ContentView = overlay.View;
			NSApplication.SharedApplication.BeginSheet (panel, this.Window);
			Console.WriteLine (((NSButton)sender).Title);

			try {
				var index = int.Parse (((NSButton)sender).Title);
				RecentFile aFile = recentFiles [index];
				string round = (aFile.Round == "1st Round") ? "M" : "S";
				//genarate the filename
				string filename = aFile.Domcile + aFile.Position + aFile.Month.ToString ().PadLeft (2, '0') + (Convert.ToInt16 (aFile.Year) - 2000) + round + "737";
				WBidHelper.SetCurrentBidInformationfromStateFileName (filename);

				//Write to currentBidDetailsfile for Error log
				FileOperations.WriteCurrentBidDetails (WBidHelper.GetAppDataPath () + "/CurrentDetails.txt", WBidHelper.GetApplicationBidData ());

				string zipFilename = WBidHelper.GenarateZipFileName ();


//			UICollectionViewCell selectedCell = collectionView.CellForItem(indexPath);
//			loadingOverlay = new LoadingOverlay(selectedCell.Frame, "");
//			View.Add(loadingOverlay);

				//load the line file data
				Task task = Task.Run (() => {

					if (!File.Exists(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS"))
					{

						WBidIntialState wbidintialState = null;
						try
						{
							wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
						}
						catch (Exception ex)
						{
							wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
							XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
							WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0");
						}
						GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS", GlobalSettings.Lines.Count, GlobalSettings.Lines.First().LineNum, wbidintialState);
						WBidHelper.SaveStateFile(filename);

					}
					else {
						//Read State file content and Stored it into an object
						GlobalSettings.WBidStateCollection = null;
						try
						{
							GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");

						}
						catch (Exception exx)

						{
							WBidIntialState wbidintialState = null;
							try
							{
								wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

							}
							catch (Exception ex)
							{
								wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
								XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());
								WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0");
							}
							GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS", GlobalSettings.Lines.Count, GlobalSettings.Lines.First().LineNum, wbidintialState);
							WBidHelper.SaveStateFile(filename);
							WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "wbsRecreate", "0", "0");
						}
						//XmlHelper.DeserializeFromXml<WBidStateCollection>(WBidHelper.GetAppDataPath() + "/" + filename + ".WBS");

					}
                    try
                    {
                        DownloadSubmittedData();
                    }
                    catch(Exception ex)
                    {
                        
                    }
					try {
						TripInfo tripInfo = null;
						LineInfo lineInfo = null;
						using (FileStream tripStream = File.OpenRead (WBidHelper.GetAppDataPath () + "/" + filename + ".WBP")) {
							//int a = 1;
							TripInfo objTripInfo = new TripInfo ();
							tripInfo = ProtoSerailizer.DeSerializeObject (WBidHelper.GetAppDataPath () + "/" + filename + ".WBP", objTripInfo, tripStream);
						}
						
						GlobalSettings.Trip = new ObservableCollection<Trip> (tripInfo.Trips.Values);
						if (tripInfo.TripVersion == GlobalSettings.TripVersion) {
						
							using (FileStream linestream = File.OpenRead (WBidHelper.GetAppDataPath () + "/" + filename + ".WBL")) {
								//int a = 1;
						
						
								LineInfo objineinfo = new LineInfo ();
								lineInfo = ProtoSerailizer.DeSerializeObject (WBidHelper.GetAppDataPath () + "/" + filename + ".WBL", objineinfo, linestream);
						
							}
						
							if (lineInfo.LineVersion == GlobalSettings.LineVersion) {
								GlobalSettings.Lines = new ObservableCollection<Line> (lineInfo.Lines.Values);
							} else {
								ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
								ReparseBL.ReparseLineFile (reparseParams);
							}
						} else {
							ReparseParameters reparseParams = new ReparseParameters () { ZipFileName = zipFilename };
							tripInfo.Trips = ReparseBL.ReparseTripAndLineFiles (reparseParams);
						}
						
						
						// NetworkPlanData networkPlanData = new NetworkPlanData();
						//networkPlanData.GetFlightRoutes(new DateTime(2015, 06, 1), new DateTime(2015, 06, 30));
						
						//if (tripInfo.TripVersion == GlobalSettings.TripVersion)
						//{
						
						//    using (FileStream linestream = File.OpenRead(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL"))
						//    {
						//        //int a = 1;
						//        LineInfo objineinfo = new LineInfo();
						//         lineInfo = ProtoSerailizer.DeSerializeObject(WBidHelper.GetAppDataPath() + "/" + filename + ".WBL", objineinfo, linestream);
						
						//    }
						//    if (lineInfo.LineVersion == GlobalSettings.LineVersion)
						//    {
						//        GlobalSettings.Lines = new ObservableCollection<Line>(lineInfo.Lines.Values);
						//    }
						//    else
						//    {
						//        ReparseParameters reparseParams = new ReparseParameters() { Trips = tripInfo.Trips, ZipFileName = zipFilename };
						//        ReparseBL.ReparseLineFile(reparseParams);
						//    }
						
						//    foreach (Line line in GlobalSettings.Lines)
						//    {
						//        line.ConstraintPoints = new ConstraintPoints();
						//        line.WeightPoints = new WeightPoints();
						//    }
						//}
						
						foreach (Line line in GlobalSettings.Lines) {
							line.ConstraintPoints = new ConstraintPoints ();
							line.WeightPoints = new WeightPoints ();
						}
						
						
					

						
					
						if (GlobalSettings.WBidStateCollection.SeniorityListItem != null) {
							if (GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber == 0)
								GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.TotalCount;
							else
								GlobalSettings.WbidUserContent.UserInformation.SeniorityNumber = GlobalSettings.WBidStateCollection.SeniorityListItem.SeniorityNumber;
						}
						GlobalSettings.CompanyVA = GlobalSettings.WBidStateCollection.CompanyVA;
						WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
						
						if (wBIdStateContent.Weights.NormalizeDaysOff == null) {
							wBIdStateContent.Weights.NormalizeDaysOff = new Wt2Parameter () { Type = 1, Weight = 0 };
						
						}
						if (wBIdStateContent.CxWtState.NormalizeDays == null) {
							wBIdStateContent.CxWtState.NormalizeDays = new StateStatus () { Cx = false, Wt = false };
						
						}
						if (wBIdStateContent.CxWtState.ETOPS == null)
						{
							wBIdStateContent.CxWtState.ETOPS = new StateStatus() { Cx = false, Wt = false };

						}
						if (wBIdStateContent.CxWtState.ETOPSRes == null)
						{
							wBIdStateContent.CxWtState.ETOPSRes = new StateStatus() { Cx = false, Wt = false };

						}
						if (wBIdStateContent.Weights.ETOPS == null)
						{
							wBIdStateContent.Weights.ETOPS = new Wt1Parameters
							{
								Weight = 0,
								lstParameters = new List<Wt1Parameter>()
							};
						}
						if (wBIdStateContent.Weights.ETOPSRes == null)
						{
							wBIdStateContent.Weights.ETOPSRes = new Wt1Parameters
							{
								Weight = 0,
								lstParameters = new List<Wt1Parameter>()
							};
						}
						if (wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue == null)
						{
							wBIdStateContent.Constraints.StartDayOftheWeek.SecondcellValue = "1";
							foreach (var item in wBIdStateContent.Constraints.StartDayOftheWeek.lstParameters)
							{
								if (item.SecondcellValue == null)
								{
									item.SecondcellValue = "1";
								}
							}
						}
						if ((double.Parse(GlobalSettings.WBidStateCollection.Version) < 2.2))
						{
							//setting the default value for the PDO to any city and any date
							wBIdStateContent.Constraints.PDOFS.SecondcellValue = "300";
							wBIdStateContent.Constraints.PDOFS.ThirdcellValue = "400";
							wBIdStateContent.Weights.PDAfter.FirstValue = 300;
							wBIdStateContent.Weights.PDAfter.ThrirdCellValue = 400;

							wBIdStateContent.Weights.PDBefore.FirstValue = 300;
							wBIdStateContent.Weights.PDBefore.ThrirdCellValue = 400;
							GlobalSettings.WBidStateCollection.Version=GlobalSettings.StateFileVersion;
						}
						if ((double.Parse(GlobalSettings.WBidStateCollection.Version) < 2.3))
						{
							if (wBIdStateContent.Constraints.EQUIP.ThirdcellValue == "500")
								wBIdStateContent.Constraints.EQUIP.ThirdcellValue = "300";
						if (wBIdStateContent.Weights.EQUIP.SecondlValue == 500)
							wBIdStateContent.Weights.EQUIP.SecondlValue = 300;
							foreach (var item in wBIdStateContent.Constraints.EQUIP.lstParameters)
							{
								if (item.ThirdcellValue == "500")
									item.ThirdcellValue = "300";
								
							}
							foreach (var item in wBIdStateContent.Weights.EQUIP.lstParameters)
							{
							if (item.SecondlValue == 500)
								item.SecondlValue = 300;
								
							}
							GlobalSettings.WBidStateCollection.Version=GlobalSettings.StateFileVersion;

						}
						if ((double.Parse(GlobalSettings.WBidStateCollection.Version) < 2.4))
						{
							foreach (WBidState state in GlobalSettings.WBidStateCollection.StateList)
							{
								state.Constraints.EQUIP.ThirdcellValue = "700";
								state.Constraints.EQUIP.lstParameters.RemoveAll(x => x.ThirdcellValue == "300" || x.ThirdcellValue == "500");
								if (state.Constraints.EQUIP.lstParameters.Count < 1)

									state.CxWtState.EQUIP.Cx = false;
								state.Weights.EQUIP.SecondlValue = 700;
								state.Weights.EQUIP.lstParameters.RemoveAll(x => x.SecondlValue == 300 || x.SecondlValue == 500);
								if (state.Weights.EQUIP.lstParameters.Count < 1)

									state.CxWtState.EQUIP.Wt = false;

								//remove 300 and 500 equipments
								if (state.BidAuto != null && state.BidAuto.BAFilter != null && state.BidAuto.BAFilter.Count > 0)
								{
									state.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
									state.BidAuto.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
									int count = 0;
									foreach (var item in state.BidAuto.BAFilter)
									{
										item.Priority = count;
										count++;
									}
								}
								if (state.CalculatedBA != null && state.CalculatedBA.BAFilter != null && state.CalculatedBA.BAFilter.Count > 0)
								{
									state.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "500");
									state.CalculatedBA.BAFilter.RemoveAll(x => x.Name == "ET" && ((Cx3Parameter)x.BidAutoObject).ThirdcellValue == "300");
								}
							}
						}

                        foreach (WBidState state in GlobalSettings.WBidStateCollection.StateList)
                        {

                            //remove all previous commutability sorts
                            state.SortDetails.BlokSort.RemoveAll((x) => x.Contains("30") || x.Contains("31") || x.Contains("32"));

                        }
                       
											
						//read the VAC file contents if the Vacation exits.
						// if (wBIdStateContent.IsVacationOverlapOverlapCorrection || wBIdStateContent.MenuBarButtonState.IsEOM)
						// {
						if (File.Exists (WBidHelper.GetAppDataPath () + "/" + filename + ".VAC")) {
						
							using (
								FileStream vacstream =
									File.OpenRead (WBidHelper.GetAppDataPath () + "/" + filename + ".VAC")) {
						
								Dictionary<string, TripMultiVacData> objineinfo =
									new Dictionary<string, TripMultiVacData> ();
								GlobalSettings.VacationData =
									ProtoSerailizer.DeSerializeObject (
									WBidHelper.GetAppDataPath () + "/" + filename + ".VAC", objineinfo, vacstream);
						
							}
						}

						if (wBIdStateContent.MenuBarButtonState.IsMIL && GlobalSettings.WBidINIContent.User.MIL)
						{
							MILData milData;
							if (File.Exists (WBidHelper.MILFilePath)) 
							{
                                   using (FileStream milStream = File.OpenRead (WBidHelper.MILFilePath)) 
								{
									MILData milDataobject = new MILData ();
									milData = ProtoSerailizer.DeSerializeObject (WBidHelper.MILFilePath, milDataobject, milStream);
								}
								if(milData.MILValue!=null)
								{
								GlobalSettings.MILData=milData.MILValue;
								GlobalSettings.MILDates=WBidCollection.GenarateOrderedMILDates(wBIdStateContent.MILDateList);
								}
								else
								{
									//mil value is null when the user selects the Mil start and end date same.
									wBIdStateContent.MenuBarButtonState.IsMIL=false;
								}
							}

						}
						//}
						GlobalSettings.MenuBarButtonStatus = wBIdStateContent.MenuBarButtonState;
						GlobalSettings.IsVacationCorrection = wBIdStateContent.IsVacationOverlapOverlapCorrection;
						GlobalSettings.IsOverlapCorrection = wBIdStateContent.IsOverlapCorrection;
						//GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
						
						WBidHelper.GenerateDynamicOverNightCitiesList ();
						GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select (y => y.Name).ToList ();

						if (wBIdStateContent.CxWtState.CLAuto == null)
							wBIdStateContent.CxWtState.CLAuto = new StateStatus { Cx = false, Wt = false };
						if (GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(1).Month && (wBIdStateContent.CxWtState.CLAuto.Cx || wBIdStateContent.CxWtState.CLAuto.Wt || (wBIdStateContent.SortDetails.BlokSort.Contains("33") || wBIdStateContent.SortDetails.BlokSort.Contains("34") || wBIdStateContent.SortDetails.BlokSort.Contains("35"))))
						{
							if (GlobalSettings.FlightRouteDetails == null)
							{
								NetworkData networkplandata = new NetworkData();
								networkplandata.ReadFlightRoutes();
							}
							CommuteCalculations objCommuteCalculations = new CommuteCalculations();
							objCommuteCalculations.CalculateDailyCommutableTimes(wBIdStateContent.Constraints.CLAuto, GlobalSettings.FlightRouteDetails);
						}
						if (GlobalSettings.WBidStateCollection.Vacation!=null && GlobalSettings.WBidStateCollection.Vacation.Count > 0) {
							GlobalSettings.SeniorityListMember = GlobalSettings.SeniorityListMember ?? new SeniorityListMember ();
							GlobalSettings.SeniorityListMember.Absences = new List<Absense> ();
							try {
							GlobalSettings.WBidStateCollection.Vacation.ForEach (x => GlobalSettings.SeniorityListMember.Absences.Add (new Absense {
								AbsenceType = "VA",
								StartAbsenceDate = Convert.ToDateTime (x.StartDate),
								EndAbsenceDate = Convert.ToDateTime (x.EndDate)
							}));
							} catch (Exception ex) {
								GlobalSettings.WBidStateCollection.Vacation.ForEach (x => GlobalSettings.SeniorityListMember.Absences.Add (new Absense {
									AbsenceType = "VA",
									StartAbsenceDate = DateTime.ParseExact (x.StartDate,"M/dd/yyyy",null),
									EndAbsenceDate = DateTime.ParseExact (x.EndDate,"M/dd/yyyy",null)
								}));
							}
							GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates ();
							GlobalSettings.TempOrderedVacationDays = WBidCollection.GetOrderedAbsenceDates ();
						}

						
						
						
						// var sabu = GlobalSettings.Trip.Where(x => linePairing.Contains(x.TripNum)).SelectMany(z => z.DutyPeriods.SelectMany(r => r.Flights.Select(t => new { arrival = t.ArrSta, depart = t.DepSta }))).ToList();
						
						List<int> lstint = new List<int> () { 1, 2, 3 };
						
						StateManagement statemanagement = new StateManagement ();
						WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
                        //FV Vacation
                        GlobalSettings.WBidStateCollection.FVVacation = GlobalSettings.WBidStateCollection.FVVacation ?? new List<Absense>();
                        GlobalSettings.IsFVVacation = (GlobalSettings.WBidStateCollection.FVVacation.Count > 0 && (GlobalSettings.CurrentBidDetails.Postion == "CP" || GlobalSettings.CurrentBidDetails.Postion == "FO"));

                        GlobalSettings.FVVacation = GlobalSettings.WBidStateCollection.FVVacation;
                        if (GlobalSettings.IsFVVacation)
                        {
                            FVVacation objvac = new FVVacation();
                            GlobalSettings.Lines = new ObservableCollection<Line>(objvac.SetFVVacationValuesForAllLines(GlobalSettings.Lines.ToList(), GlobalSettings.VacationData));
                            
                        }
						statemanagement.RecalculateLineProperties(wBidStateContent);
						statemanagement.ReloadDataFromStateFile ();

						if (GlobalSettings.WBidINIContent.RatioValues != null)
						{
							WBidHelper.SetRatioValues(wBidStateContent);
						}
						SortCalculation sort = new SortCalculation ();
						if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty) {
							sort.SortLines (wBidStateContent.SortDetails.SortColumn);
						}
                        SetManualMovementShadowForLines(wBidStateContent);
						InvokeOnMainThread (() => {
						
							NSApplication.SharedApplication.EndSheet (panel);
							panel.OrderOut (this);
							CommonClass.isHomeWindow = false;
							mainController = new MainWindowController ();
							CommonClass.MainController = mainController;
							mainController.Window.MakeKeyAndOrderFront (this);
							this.Window.Close ();
							this.Window.OrderOut (this);
							//								this.Window.ResignKeyWindow ();
						});
					} catch (Exception ex) {
						InvokeOnMainThread (() => {
							NSApplication.SharedApplication.EndSheet (panel);
							panel.OrderOut (this);
							CommonClass.AppDelegate.ErrorLog (ex);
							CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
						});
					}

				});
			} catch (Exception ex) {
				InvokeOnMainThread (() => {
					NSApplication.SharedApplication.EndSheet (panel);
					panel.OrderOut (this);
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				});

			}

		}
        private void SetManualMovementShadowForLines(WBidState wBidStateContent)
        {
            try
            {
                if (GlobalSettings.WBidINIContent.User.IsModernViewShade)
                {
                    int blueLine = wBidStateContent.LineForBlueLine;
                    List<int> shadowLines = wBidStateContent.LinesForBlueBorder;

                    var objBlueLine = GlobalSettings.Lines.FirstOrDefault(x => x.LineNum == blueLine);
                    if (objBlueLine != null)
                    {
                        if (GlobalSettings.Lines[0].LineNum == blueLine)
                            objBlueLine.ManualScroll = 3;
                        else
                            objBlueLine.ManualScroll = 1;
                    }
                    if (shadowLines.Count > 0)
                    {
                        var objBorderLines = GlobalSettings.Lines.Where(x => shadowLines.Any(y => y == x.LineNum));
                        foreach (var item in objBorderLines)
                        {
                            item.ManualScroll = 2;
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void DownloadSubmittedData()
        {
            if (GlobalSettings.WBidStateCollection.SubmittedResult != null && GlobalSettings.WBidStateCollection.SubmittedResult != string.Empty)
            {
                //update submit result
                bool isConnectionAvailable = Reachability.CheckVPSAvailable();
                if (isConnectionAvailable)
                {
                   
                    BidSubmittedData objbiddetails = new BidSubmittedData();
                    objbiddetails.Domicile = GlobalSettings.CurrentBidDetails.Domicile;
                    objbiddetails.Month = GlobalSettings.CurrentBidDetails.Month;
                    objbiddetails.Year = GlobalSettings.CurrentBidDetails.Year;
                    objbiddetails.Position = GlobalSettings.CurrentBidDetails.Postion;
                    objbiddetails.Round = (GlobalSettings.CurrentBidDetails.Round == "M") ? 1 : 2;
                    objbiddetails.EmpNum = Convert.ToInt32(GlobalSettings.WbidUserContent.UserInformation.EmpNo);
                    var jsonData = ServiceUtils.JsonSerializer(objbiddetails);
                    StreamReader dr = ServiceUtils.GetRestData("GetBidSubmittedData", jsonData);
                    GlobalSettings.WBidStateCollection.SubmittedResult = (WBidCollection.ConvertJSonStringToObject<BidSubmittedData>(dr.ReadToEnd())).SubmittedResult;
                }

            }

        }
		void HandleBidDownload (NSNotification obj)
		{
			try {
				if (notif!=null) {
					NSNotificationCenter.DefaultCenter.RemoveObserver (notif);
					notif = null;
				}
				if (obj.Object.ToString () == "Start") {
					var download = new DownloadBidWindowController ();
					download.Window.IgnoresMouseEvents = true;
					CommonClass.DownloadController = download;
					this.Window.AddChildWindow (download.Window, NSWindowOrderingMode.Above);
					download.Window.MakeKeyAndOrderFront (this);
					//NSApplication.SharedApplication.RunModalForWindow (download.Window);

					IntPtr modalSession;
					long response;
					modalSession = NSApplication.SharedApplication.BeginModalSession (download.Window);
					response = (long)NSRunResponse.Continues;
					while (download.Running && response == (long)NSRunResponse.Continues) 
					{
						response = (long)NSApplication.SharedApplication.RunModalSession (modalSession);
						NSRunLoop.Current.RunUntil (NSRunLoopMode.Default, NSDate.DistantFuture);
					}
					NSApplication.SharedApplication.EndModalSession (modalSession);
				}
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
				CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			}
		}
	}

	public static class Extensions
	{
		public static string LocalValue (this NSDate date)
		{
			var d = DateTime.Parse (date.ToString ());
			var convertedTime = TimeZoneInfo.ConvertTime (d, TimeZoneInfo.Local);
			return convertedTime.ToString ("HH:mm");
		}

//		public static DateTime NSDateToDateTime (this NSDate date)
//		{
//			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime (
//				                     new DateTime (2001, 1, 1, 0, 0, 0));
//			return reference.AddSeconds (date.SecondsSinceReferenceDate);
//		}
//
		//public static NSDate DateTimeToNSDate (this DateTime date)
		//{
		//	DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime (
  //              new DateTime (date.Year, date.Month, date.Day, 0, 0, 0));
		//	return NSDate.FromTimeIntervalSinceReferenceDate (
		//		(date - reference).TotalSeconds);
		//}

		public static DateTime NSDateToDateTime(this NSDate date)
		{
			// NSDate has a wider range than DateTime, so clip
			// the converted date to DateTime.Min|MaxValue.
			double secs = date.SecondsSinceReferenceDate;


			if (secs < -63113904000)
				return DateTime.MinValue;
			if (secs > 252423993599)
				return DateTime.MaxValue;
            DateTime s = (DateTime)date; //some Datetime that you want to change time for 8:36:44 ;
s = new DateTime(s.Year, s.Month, s.Day, 0, 0, 0);

			return s;
		}

		public static NSDate DateTimeToNSDate(this DateTime date)
		{
			if (date.Kind == DateTimeKind.Unspecified)
				date = DateTime.SpecifyKind (date, DateTimeKind.Local);
			return (NSDate) date;
		}

	}

	public class BidListClass : NSObject
	{
		[Export ("Date")]
		public string Date { get; set; }

		[Export ("Name")]
		public string Name { get; set; }

		[Export ("Index")]
		public string Index { get; set; }

		[Export ("Show")]
		public bool Show { get; set; }

		[Export ("Image")]
		public NSImage Image { get; set; }

	}

}

