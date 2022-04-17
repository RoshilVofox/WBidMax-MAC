using System;

using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Parser;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.SharedLibrary.SWA;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model.SWA;
using System.Collections.Generic;
using WBid.WBidiPad.PortableLibrary;
using System.IO;
using System.IO.Compression;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.Parser;
using System.Collections.ObjectModel;
using WBid.WBidiPad.SharedLibrary.Utility;
using System.Linq;
using CoreGraphics;

namespace WBid.WBidMac.Mac.WindowControllers
{
    public partial class SecretDataDownloadController : NSWindowController
    {
        List<Domicile> listDomicile = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).ToList();
        public string[] domicileArray = GlobalSettings.WBidINIContent.Domiciles.OrderBy(x => x.DomicileName).Select(y => y.DomicileName).ToArray();
       
        string UserId = "x21221";
        string Password = "Vofox2018-4";
        string SelectedPosition = string.Empty;
        string SelectedRound = string.Empty;
        List<string> SelectedBases = new List<string>();
        int SelectedMonth = 0;
        private string SessionCredential = String.Empty;
        private DownloadInfo _downloadFileDetails;
        List<string> LogData;
        Dictionary<string, Trip> trips;
        Dictionary<string, Line> lines;
        bool IsMissingTripFailed = false;
        /// <summary>
        /// create single instance of TripTtpParser class
        /// </summary>
        private TripTtpParser _tripTtpParser;
        public TripTtpParser TripTtpParser
        {
            get
            {
                return _tripTtpParser ?? (_tripTtpParser = new TripTtpParser());
            }
        }

        private CalculateTripProperties _calculateTripProperties;
        public CalculateTripProperties CalculateTripProperties
        {
            get
            {
                return _calculateTripProperties ?? (_calculateTripProperties = new CalculateTripProperties());
            }
        }


        private CalculateLineProperties _calculateLineProperties;
        public CalculateLineProperties CalculateLineProperties
        {
            get
            {
                return _calculateLineProperties ?? (_calculateLineProperties = new CalculateLineProperties());
            }
        }
        public SecretDataDownloadController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SecretDataDownloadController(NSCoder coder) : base(coder)
        {
        }

        public SecretDataDownloadController() : base("SecretDataDownload")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            string[] arr = WBidCollection.GetBidPeriods().Select(x => x.Period).ToArray();
            int currentMonth = DateTime.Now.AddMonths(1).Month;
            dropDownMonth.RemoveAllItems();
            dropDownMonth.AddItems(arr);
            dropDownMonth.SelectItem(arr[currentMonth-1]);
            SelectedMonth = currentMonth;
            btnPositions.SelectCellWithTag(0);
            SelectedPosition = "Both";
            btnRound.SelectCellWithTag(0);
            SelectedRound = "M";
            btnPositions.Activated += (object sender, EventArgs e) => {
                SelectedPositionsAndRound();
            };
            btnRound.Activated += (object sender, EventArgs e) => {
                SelectedPositionsAndRound();
            };
            dropDownMonth.Activated += (object sender, EventArgs e) => {
                SelectedMonth = (int)((NSPopUpButton)sender).IndexOfSelectedItem + 1;
            };
            btnDownload.Activated+=(object sender, EventArgs e) => {
                DownloadAllData();
            };
            btnAllPilots.Activated += (object sender, EventArgs e) => {

                List<string> listBases = GlobalSettings.WBidINIContent.Domiciles.Select(x => x.DomicileName).OrderBy(y => y).ToList();
                SelectedBases = new List<string>();
                List<string> listPilots = new List<string>();
                if (((NSButton)sender).State == NSCellStateValue.On)
                {



                    for (int items = 1; items <= listDomicile.Count; items++)
                    {
                        if (items == 2 || items == 6)
                        {
                            var view = this.Window.ContentView.ViewWithTag(items);
                            NSButton button = (AppKit.NSButton)((NSButton)view).ViewWithTag(items);
                            button.State = NSCellStateValue.Off;

                        }
                        else
                        {

                            listPilots.Add(listBases[items - 1]);
                            var view = this.Window.ContentView.ViewWithTag(items);
                            NSButton button = (AppKit.NSButton)((NSButton)view).ViewWithTag(items);
                            button.State = NSCellStateValue.On;
                        }
                    }

                }
                else
                {

                    for (int items = 1; items <= listDomicile.Count; items++)
                    {

                        if (items == 2 || items == 6)
                        {
                            var view = this.Window.ContentView.ViewWithTag(items);
                            NSButton button = (AppKit.NSButton)((NSButton)view).ViewWithTag(items);
                            button.State = NSCellStateValue.Off;

                        }
                        else
                        {

                            listPilots.Remove(listBases[items - 1]);
                            var view = this.Window.ContentView.ViewWithTag(items);
                            NSButton button = (AppKit.NSButton)((NSButton)view).ViewWithTag(items);
                            button.State = NSCellStateValue.Off;
                        }
                    }
                }

                SelectedBases = listPilots;


            };

            btnAllFlts.Activated += (object sender, EventArgs e) => {

                List<string> listBases = GlobalSettings.WBidINIContent.Domiciles.Select(x => x.DomicileName).OrderBy(y => y).ToList();
                SelectedBases = new List<string>();
                List<string> listFlts = new List<string>();
                if (((NSButton)sender).State == NSCellStateValue.On)
                {

                    for (int items = 1; items < listDomicile.Count; items++)
                    {
                        listFlts.Add(listBases[items - 1]);
                        var view = this.Window.ContentView.ViewWithTag(items);
                        NSButton button = (AppKit.NSButton)((NSButton)view).ViewWithTag(items);
                        button.State = NSCellStateValue.On;
                    }
                  
                }
                else
                {

                    for (int items = 1; items < listDomicile.Count; items++)
                    {
                        listFlts.Remove(listBases[items - 1]);
                        var view = this.Window.ContentView.ViewWithTag(items);
                        NSButton button = (AppKit.NSButton)((NSButton)view).ViewWithTag(items);
                        button.State = NSCellStateValue.Off;
                    }
                }

                SelectedBases = listFlts;

            };

            tblDomiciles.Source = new DomicileTableSource(this);


        }
        private void DownloadAllData()
        {
            UserId = txtUserID.StringValue;
            Password = txtPassword.StringValue;
           // UserId = "x21221";
           //Password = "Vofox2018-4";
            bool isAutneticated = checkAuthentication();
            if (isAutneticated)
            {

                bool ISFA, ISPilot = false;
                ISFA = btnAllFlts.State == NSCellStateValue.On;
                ISPilot = btnAllPilots.State == NSCellStateValue.On;
                var allpositions = WBidCollection.GetPositions().ToList();
                if (ISFA && !ISPilot)
                {
                    allpositions = allpositions.Where(x => x.LongStr == "FA").ToList();
                }
                else if (ISFA && ISPilot)
                {
                    if (SelectedPosition == "Capt")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "FO");
                    }
                    else if (SelectedPosition == "FO")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "CP");
                    }
                }
                else if (ISPilot)
                {
                    if (SelectedPosition == "Capt")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "FO" || x.LongStr == "FA");
                    }
                    else if (SelectedPosition == "FO")
                    {
                        allpositions.RemoveAll(x => x.LongStr == "CP" || x.LongStr == "FA");
                    }
                    else
                    {
                        allpositions.RemoveAll(x => x.LongStr == "FA");
                    }
                }
                foreach (var position in allpositions)
                {
                    foreach (var domicile in SelectedBases)
                    {
                        AddlogDisplay("======================================");
                        //AddlogDisplay("Download started for " + domicile + " - " + position.LongStr + " - " + SelectedBidMonth.Period + " - " + SelectedBidRound.Round);
                        _downloadFileDetails = new DownloadInfo();

                        _downloadFileDetails.SessionCredentials = SessionCredential;
                        GlobalSettings.DownloadBidDetails = new BidDetails();
                        GlobalSettings.DownloadBidDetails.Month = SelectedMonth;
                        GlobalSettings.DownloadBidDetails.Domicile = domicile;
                        GlobalSettings.DownloadBidDetails.Postion = position.LongStr;
                        GlobalSettings.DownloadBidDetails.Round = (SelectedRound == "M") ? "D" : "B";
                        GlobalSettings.DownloadBidDetails.Year = DateTime.Now.AddMonths(1).Year;
                        _downloadFileDetails.DownloadList = WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails);
                        DownloadDataFromSWA(_downloadFileDetails.DownloadList);
                    }
                }
            }

            CommonClass.HomeController.LoadContent();
        }
        void allPilotAction (int tag) {

          

        }

        void allFltAttnds (int tag) {

            SelectedBases = new List<string>();             List<string> listFlts = new List<string>();

            //for (int items = 0; items < domicileArray.Count; items++)
            //{

            //}
        }

        void SelectedPositionsAndRound()
        {


            //Captain
            if (btnPositions.SelectedTag == 0)
            {
                SelectedPosition = "Both";
            }
            //First Officer
            else if (btnPositions.SelectedTag == 1)
            {
                SelectedPosition = "CP";

            }
            //Flight Attendant
            else
            {
                SelectedPosition = "FO";
            }

            SelectedRound = (btnRound.SelectedTag == 0) ? "M" : "S";

        }

        public void setSelectedBases(int tag) {

            SelectedBases.Add(domicileArray[tag - 1]);

        }

        public void removeSelectedBases(int tag)
        {
            SelectedBases.Remove(domicileArray[tag - 1]);
        }

        public new SecretDataDownload Window
        {
            get { return (SecretDataDownload)base.Window; }
        }

        private bool checkAuthentication()
        {
            if (Reachability.CheckVPSAvailable())
            {

                Authentication authentication = new Authentication();
                string authResult = authentication.CheckCredential(UserId, Password);
                if (authResult.Contains("ERROR: ") || authResult.Contains("Exception"))
                {
                    return false;
                }
                else
                {
                    SessionCredential = authResult;
                    return true;
                }
            }
            else
                return false;

        }
        private void StartDataDownload()
        {
            _downloadFileDetails.SessionCredentials = SessionCredential;
            _downloadFileDetails.DownloadList = WBidCollection.GenarateDownloadFileslist(GlobalSettings.DownloadBidDetails);
        }
        private void DownloadDataFromSWA(List<string> downloadFiles)
        {
            DownloadInfo downloadinfo = new DownloadInfo();
            downloadinfo.SessionCredentials = SessionCredential;
            downloadinfo.UserId = UserId;
            downloadinfo.Password = Password;
            string zipFileName = downloadFiles.Where(x => x.Contains(".737")).FirstOrDefault();

            DownloadedFileInfo sWAFileInfo;
            List<DownloadedFileInfo> downloadedFileDetails = new List<DownloadedFileInfo>();
            DownloadBid _downloadBidObject = new DownloadBid();
            string packetType = string.Empty;

            foreach (string filename in downloadFiles)
            {
                sWAFileInfo = new DownloadedFileInfo();

                AddlogDisplay("Downloading  " + filename + "...");
                packetType = string.Empty;
                //if length <10 or > 11 will add an error message  and continue to download next file. 
                if (filename.Length < 10 || filename.Length > 11)
                {
                    downloadedFileDetails.Add(new DownloadedFileInfo() { IsError = true, Message = "", FileName = filename });
                    continue;
                }
                //finding packet type
                packetType = (filename.Substring(7, 3) == "737") ? "ZIPPACKET" : "TXTPACKET";

                sWAFileInfo = _downloadBidObject.DownloadBidFile(downloadinfo, filename.ToUpper(), packetType);

                if (sWAFileInfo.IsError && sWAFileInfo.FileName.Contains("737"))
                {
                    AddlogDisplay("Data Transfer Failed for Domcile");
                    List<string> lstMessages = new List<string>();
                    lstMessages.Add("Bid Package Data");
                    if (sWAFileInfo.Message.Contains("BIDINFO DATA NOT AVAILABLE"))
                    {
                        AddlogDisplay("Data Transfer Failed for Domcile");
                        //MessageBox.Show("The Requested data doesnot exist on  the SWA servers  for Domcile " + bidDetails.Domicile + ". make sure proper month is selected and you are within the normal timeframe for the request");
                    }
                    else
                    {
                        AddlogDisplay("Data Transfer Failed for Domcile");
                        // MessageBox.Show("Data Transfer Failed for Domcile  " + bidDetails.Domicile);
                    }
                    return;
                }
                else
                {
                    downloadedFileDetails.Add(sWAFileInfo);

                }
            }

            foreach (DownloadedFileInfo sWAFile in downloadedFileDetails)
            {
                if (sWAFile.FileName != null)
                {
                    //If the file is error status, we dont need to save the file
                    if (sWAFile.IsError)
                    {
                        AddlogDisplay("Error File download " + sWAFile.FileName);
                        // Message("Error   " + sWAFile.FileName + " Download...");
                        continue;
                    }

                    FileStream fStream = new FileStream(Path.Combine(WBidHelper.GetAppDataPath(), sWAFile.FileName), FileMode.Create);
                    fStream.Write(sWAFile.byteArray, 0, sWAFile.byteArray.Length);
                    fStream.Dispose();
                    AddlogDisplay("Saved   " + sWAFile.FileName + "...");



                    //Extract tIhe zip file
                    if (Path.GetExtension(sWAFile.FileName) == ".737")
                    {
                        Extract737File(sWAFile.FileName);

                        //Delete the .737 file
                        string path = Path.Combine(WBidHelper.GetAppDataPath(), sWAFile.FileName);
                        //if (File.Exists(path))
                        // File.Delete(path);
                    }

                }
            }


            DownloadedFileInfo zipFile = downloadedFileDetails.FirstOrDefault(x => x.FileName == zipFileName);
            if (zipFile.IsError)
            {
                AddlogDisplay("The Requested data doesnot exist on  the SWA servers ");
            }
            else
            {
                string path = WBidHelper.GetAppDataPath() + "/" + Path.GetFileNameWithoutExtension(zipFileName);

                if (!(File.Exists(path + "/" + "TRIPS") && File.Exists(path + "/" + "PS")))
                {
                    AddlogDisplay("There is an error while downloading the data. Please check your internet connection and try again ");
                }
                else
                {
                    WBidHelper.SetCurrentBidInformationfromZipFileName(zipFileName, false);
                    AddlogDisplay("Started parsing of line and trip file ");
                    ParseData(zipFile.FileName);
                }

            }
        }
        /// <summary>
        /// Extract the 737 file to trip and PS file
        /// </summary>
        /// <param name="fileName"></param>
        public void Extract737File(string fileName)
        {
            try
            {
                string appDataPath = WBidHelper.GetAppDataPath();
                //string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/WBidMax";
                string folderPath = appDataPath + "/" + Path.GetFileNameWithoutExtension(fileName);
                if (Directory.Exists(folderPath))
                    Directory.Delete(folderPath, true);

                Directory.CreateDirectory(folderPath);

                string target = Path.Combine(appDataPath, appDataPath + "/" + Path.GetFileNameWithoutExtension(fileName)) + "/";
                string zipFile = Path.Combine(appDataPath, fileName);

                if (File.Exists(zipFile))
                {

                    ZipFile.ExtractToDirectory(zipFile, target);
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void AddlogDisplay(string message)
        {
            LogData = LogData ?? new List<string>();
            LogData.Insert(0, message);

        }
        /// <summary>
        /// Parse the Downloaded Data.This will Parse trip file,line file etc.
        /// </summary>
        /// <param name="employeeNumber"></param>
        /// <param name="password"></param>
        /// <param name="zipFilename"></param>
        private void ParseData(string zipFilename)
        {

           
            try
            {
                //Prase Trip files

                trips = ParseTripFile(zipFilename);

                if (trips != null)
                {

                    if (zipFilename.Substring(0, 1) == "A" && zipFilename.Substring(1, 1) == "B")
                    {
                        FASecondRoundParser fASecondRound = new FASecondRoundParser();
                        lines = fASecondRound.ParseFASecondRound(WBidHelper.GetAppDataPath() + "/" + zipFilename.Substring(0, 6).ToString() + "/PS", ref trips, GlobalSettings.FAReserveDayPay, zipFilename.Substring(2, 3));
                    }
                    else
                    {
                        lines = ParseLineFiles(zipFilename);
                    }
                    ParseDataExecution(zipFilename);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        void ParseDataExecution(string zipFilename)
        {
            //InvokeInBackground(() =>
            //{
                try
                {

                    //bid round is second round
                    if (GlobalSettings.CurrentBidDetails.Round == "S")
                    {
                        //Finding if any missed trip exists
                        List<string> allPair = lines.SelectMany(x => x.Value.Pairings).Distinct().ToList();
                        var pairingwHasNoDetails = allPair.Where(x => !trips.Select(y => y.Key).ToList().Any(z => (z == x.Substring(0, 4)) || (z == x && x.Substring(1, 1) == "P"))).ToList();

                        //Checking any missed trip  exist
                        if (pairingwHasNoDetails.Count > 0)
                        {

                            try
                            {
                                if (GlobalSettings.WBidINIContent.MiscellaneousTab.IsRetrieveMissingData && (GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(-1).Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.Month || GlobalSettings.CurrentBidDetails.Month == DateTime.Now.AddMonths(1).Month))
                                {

                                    GlobalSettings.parsedDict = new Dictionary<string, Trip>();


                                    ///string password = (GlobalSettings.buddyBidTest) ? GlobalSettings.QAScrapPassword : _downloadFileDetails.Password;
                                    string password =  _downloadFileDetails.Password;
                                    scrap(_downloadFileDetails.UserId, password, pairingwHasNoDetails, GlobalSettings.DownloadBidDetails.Month, GlobalSettings.DownloadBidDetails.Year, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay);

                                    if (GlobalSettings.parsedDict == null || GlobalSettings.parsedDict.Count == 0)
                                    {

                                        IsMissingTripFailed = true;
                                        string bidFileName = string.Empty;
                                        bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                        BidLineParser bidLineParser = new BidLineParser();
                                        var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                        trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                                        TripExecution(zipFilename);

                                        return;
                                    }
                                    else
                                    {
                                        //IsMissingTripFailed = true;
                                        trips = trips.Concat(GlobalSettings.parsedDict).ToDictionary(pair => pair.Key, pair => pair.Value);
                                    }

                                }
                                else
                                {

                                    string bidFileName = string.Empty;
                                    IsMissingTripFailed = true;
                                    bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                    BidLineParser bidLineParser = new BidLineParser();
                                    var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                    trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);

                                }
                            }
                            catch (Exception ex)
                            {

                                IsMissingTripFailed = true;
                                string bidFileName = string.Empty;
                                bidFileName = GlobalSettings.CurrentBidDetails.Domicile + GlobalSettings.CurrentBidDetails.Postion + "N.TXT";
                                BidLineParser bidLineParser = new BidLineParser();
                                var domcilecode = GlobalSettings.WBidINIContent.Domiciles.FirstOrDefault(x => x.DomicileName == GlobalSettings.CurrentBidDetails.Domicile).Code;

                                trips = trips.Concat(bidLineParser.ParseBidlineFile(WBidHelper.GetAppDataPath() + "/" + bidFileName, GlobalSettings.CurrentBidDetails.Domicile, domcilecode, GlobalSettings.show1stDay, GlobalSettings.showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion).Where(x => pairingwHasNoDetails.Contains(x.Key))).ToDictionary(pair => pair.Key, pair => pair.Value);
                                TripExecution(zipFilename);

                                return;
                            }
                        }
                    }
                    TripExecution(zipFilename);
                }

                catch (Exception ex)
                {
                    throw ex;
                }
            //});
        }
        void TripExecution(string zipFilename)
        {

            //InvokeInBackground(() =>
            //{
                try
                {

                    List<CityPair> ListCityPair = TripTtpParser.ParseCity(WBidHelper.GetAppDataPath() + "/trips.ttp");
                    GlobalSettings.TtpCityPairs = ListCityPair;

                    // Additional processing needs to be done to FA trips before CalculateTripPropertyValues
                    if (zipFilename.Substring(0, 1) == "A")
                        CalculateTripProperties.PreProcessFaTrips(trips, ListCityPair);

                    CalculateTripProperties.CalculateTripPropertyValues(trips, ListCityPair);

                    try
                    {
                        CalculateLineProperties.CalculateLinePropertyValues(trips, lines, GlobalSettings.CurrentBidDetails);
                    }
                    catch (Exception ex)
                    {

                    }
                    SaveParsedFiles(trips, lines);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            //});

        }
        /// <summary>
        /// Parse Trip Files
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Dictionary<string, Trip> ParseTripFile(string fileName)
        {

            Dictionary<string, Trip> Trips = new Dictionary<string, Trip>();
            try
            {
                TripParser tripParser = new TripParser();
                string filePath = WBidHelper.GetAppDataPath() + "/" + fileName.Substring(0, 6).ToString() + "/TRIPS";
                byte[] byteArray = File.ReadAllBytes(filePath);

                DateTime[] dSTProperties = DSTProperties.SetDSTProperties();
                if (dSTProperties[0] != null && dSTProperties[0] != DateTime.MinValue)
                {
                    GlobalSettings.FirstDayOfDST = dSTProperties[0];
                }
                if (dSTProperties[1] != null && dSTProperties[1] != DateTime.MinValue)
                {
                    GlobalSettings.LastDayOfDST = dSTProperties[1];
                }
                //WBidHelper.SetDSTProperties();
                Trips = tripParser.ParseTrips(fileName, byteArray, GlobalSettings.FirstDayOfDST, GlobalSettings.LastDayOfDST);
            }
            catch (Exception ex)
            {

                throw;
            }
            return Trips;
        }

        private Dictionary<string, Line> ParseLineFiles(string fileName)
        {
            Dictionary<string, Line> Lines = new Dictionary<string, Line>();
            LineParser lineParser = new LineParser();
            string filePath = WBidHelper.GetAppDataPath() + "/" + fileName.Substring(0, 6).ToString() + "/PS";
            byte[] byteArray = File.ReadAllBytes(filePath);
            Lines = lineParser.ParseLines(fileName, byteArray);
            return Lines;
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

                    WBidIntialState wbidintialState = null;
                    try
                    {
                        wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    }
                    catch (Exception ex)
                    {
                        WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                        try
                        {
                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                        }
                        catch (Exception exx)
                        {

                            wbidintialState = WBidCollection.CreateDWCFile(GlobalSettings.DwcVersion);
                            XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());

                            //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0");

                        }
                        XmlHelper.SerializeToXml(wbidintialState, WBidHelper.GetWBidDWCFilePath());

                        //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0");
                        //WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"dwcRecreate","0","0");

                    }
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
                    if (GlobalSettings.isHistorical)
                    {
                        GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    }
                    else
                        GlobalSettings.WBidStateCollection.DataSource = "Original";

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            else
            {
                try
                {
                    //Read the state file object and store it to global settings.
                    GlobalSettings.WBidStateCollection = XmlHelper.ReadStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS");
                }
                catch (Exception ex)
                {

                    //Recreate state file
                    //--------------------------------------------------------------------------------
                    WBidIntialState wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());
                    GlobalSettings.WBidStateCollection = WBidCollection.CreateStateFile(WBidHelper.GetAppDataPath() + "/" + fileToSave + ".WBS", lines.Count, lines.First().Value.LineNum, wbidintialState);
                    //WBidHelper.LogDetails(GlobalSettings.WbidUserContent.UserInformation.EmpNo,"wbsRecreate","0","0");
                    WBidLogEvent obgWBidLogEvent = new WBidLogEvent();
                    //obgWBidLogEvent.LogAllEvents(GlobalSettings.WbidUserContent.UserInformation.EmpNo, "dwcRecreate", "0", "0");
                    if (GlobalSettings.isHistorical)
                    {
                        GlobalSettings.WBidStateCollection.DataSource = "HistoricalData";
                    }
                    else
                        GlobalSettings.WBidStateCollection.DataSource = "Original";
                }

                GlobalSettings.WBidStateCollection.DataSource = "Original";
            }

            //save the vacation to state file
            GlobalSettings.WBidStateCollection.Vacation = new List<Vacation>();
            WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            wBIdStateContent.MenuBarButtonState.IsMIL = false;


            wBIdStateContent.IsOverlapCorrection = GlobalSettings.IsOverlapCorrection;


            GlobalSettings.WBidStateCollection.CompanyVA = GlobalSettings.CompanyVA;

            if (IsMissingTripFailed)
            {
                wBIdStateContent.IsMissingTripFailed = true;
            }
            else
            {
                wBIdStateContent.IsMissingTripFailed = false;
            }

            //GlobalSettings.WBidStateCollection.IsModified = true;
            WBidHelper.SaveStateFile(WBidHelper.WBidStateFilePath);

            WBidHelper.GenerateDynamicOverNightCitiesList();
            //  GlobalSettings.OverNightCitiesInBid = GlobalSettings.Lines.SelectMany(x => x.OvernightCities).Distinct().OrderBy(x => x).ToList();
            GlobalSettings.AllCitiesInBid = GlobalSettings.WBidINIContent.Cities.Select(y => y.Name).ToList(); var linePairing = GlobalSettings.Lines.SelectMany(y => y.Pairings);

            if (wBIdStateContent.CxWtState.AMPMMIX == null)
                wBIdStateContent.CxWtState.AMPMMIX = new AMPMConstriants();
            if (wBIdStateContent.CxWtState.FaPosition == null)
                wBIdStateContent.CxWtState.FaPosition = new PostionConstraint();
            if (wBIdStateContent.CxWtState.TripLength == null)
                wBIdStateContent.CxWtState.TripLength = new TripLengthConstraints();
            if (wBIdStateContent.CxWtState.DaysOfWeek == null)
                wBIdStateContent.CxWtState.DaysOfWeek = new DaysOfWeekConstraints();
            if (wBIdStateContent.Constraints.DaysOfMonth == null)
                wBIdStateContent.Constraints.DaysOfMonth = new DaysOfMonthCx() { };


            if (wBIdStateContent.Weights.NormalizeDaysOff == null)
            {
                wBIdStateContent.Weights.NormalizeDaysOff = new Wt2Parameter() { Type = 1, Weight = 0 };

            }
            if (wBIdStateContent.CxWtState.NormalizeDays == null)
            {
                wBIdStateContent.CxWtState.NormalizeDays = new StateStatus() { Cx = false, Wt = false };

            }

            StateManagement statemanagement = new StateManagement();
            //statemanagement.ReloadDataFromStateFile();
            WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            statemanagement.SetMenuBarButtonStatusFromStateFile(wBidStateContent);
            //Setting  status to Global variables
            statemanagement.SetVacationOrOverlapExists(wBidStateContent);
            //St the line order based on the state file.
            statemanagement.ReloadStateContent(wBidStateContent);


            SortCalculation sort = new SortCalculation();


            if (wBidStateContent.SortDetails != null && wBidStateContent.SortDetails.SortColumn != null && wBidStateContent.SortDetails.SortColumn != string.Empty)
            {
                sort.SortLines(wBidStateContent.SortDetails.SortColumn);
            }

        }
        private void scrap(string userName, string password, List<string> pairingwHasNoDetails, int month, int year, int show1stDay, int showAfter1stDay)
        {
            //GlobalSettings.IsScrapStart = true;

            //InvokeOnMainThread(() =>
            //{
            //    if (userName == "x21221")
            //    {
            //        ContractorEmpScrap scrap = new ContractorEmpScrap(userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion);
            //        //this.AddChildViewController(scrap);
            //        //scrap.View.Hidden = true;

            //        //this.View.AddSubview(scrap.View);
            //    }
            //    else
            //    {
            //        webView scrapper = new webView(userName, password, pairingwHasNoDetails, month, year, show1stDay, showAfter1stDay, GlobalSettings.CurrentBidDetails.Postion);
            //        this.AddChildViewController(scrapper);
            //        scrapper.View.Hidden = true;

            //        this.View.AddSubview(scrapper.View);
            //    }
            //});

            //while (GlobalSettings.IsScrapStart)
            //{
            //};
        }
    }


    public partial class DomicileTableSource : NSTableViewSource
    {
        SecretDataDownloadController parentWC;
        public DomicileTableSource(SecretDataDownloadController parent)
        {
            parentWC = parent;
        }
        public override nint GetRowCount(NSTableView tableView)
        {
            return parentWC.domicileArray.Count();
        }
        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {

            var view = (SecretDataCell)tableView.MakeView("SecretCell", this);
            view.BindData(parentWC.domicileArray[(int)row], (int)row,parentWC);
            return view;
          
        }
    }
}
