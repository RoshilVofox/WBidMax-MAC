
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WebKit;
using WBid.WBidiPad.PortableLibrary.Parser;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;
using HtmlAgilityPack;

namespace WBid.WBidMac.Mac
{
	public partial class ScrapViewController : AppKit.NSViewController
	{
		#region Constructors
		int finishCount = 0;
		int finishCount2 = 0;
		List<string> listOfLines;
		string userName;
		string password;
		List<string> pairingwHasNoDetails;
		int month;
		int year;
		int show1stDay;
		int showAfter1stDay;
		string tripDate;
		int currentIndex;
		string _position;
		Dictionary<string, Trip> parsedDict;
		PraseScrapedTripDetails scraper = new PraseScrapedTripDetails ();

		public ScrapViewController ( string uname, string pass, List<string> strArr, int mn, int yr, int s1D, int sA1D,string position) : base ("ScrapView", null)
		{
			userName = uname;
			password = pass;
			pairingwHasNoDetails = strArr;
			month = mn;
			year = yr;
			show1stDay = s1D;
			showAfter1stDay = sA1D;
			_position = position;
		}

		// Called when created from unmanaged code
		public ScrapViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ScrapViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ScrapViewController () : base ("ScrapView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ScrapView View {
			get {
				return (ScrapView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			try {
				scrapWeb1.FinishedLoad += FirstWebLoaded;
				scrapWeb2.FinishedLoad += SecondWebLoaded;
				string url = "https://www15.swalife.com/PortalWeb/cwa.jsp?test=test";
				scrapWeb1.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (url)));
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
			}

		}

		void FirstWebLoaded (object sender, WebFrameEventArgs e)
		{
			finishCount++;
			if (finishCount == 3) {
				string setUser = "window.frames[1].document.getElementById(\"useridField\").value =" + "\"" + userName + "\"" + ";";
				string setPass = "window.frames[1].document.forms[0].elements[2].value =" + "\"" + password + "\"" + ";";
				scrapWeb1.WindowScriptObject.EvaluateWebScript (setUser);//Set value to it
				scrapWeb1.WindowScriptObject.EvaluateWebScript (setPass);
				scrapWeb1.WindowScriptObject.EvaluateWebScript ("var field = window.frames[1].document.getElementsByTagName('a')[1]; field.click();");
				//browser.EvaluateJavascript ("window.alert('test');");
			} else if (finishCount == 7) {
				//finishCount = 0;
				//browser.Tag = 3;
				currentIndex = 0;

				string day = pairingwHasNoDetails [currentIndex].Substring (4, 2).TrimStart (' ');
				tripDate = month.ToString ("d2") + "%2F" + int.Parse (day).ToString ("d2") + "%2F" + year.ToString ();
				Console.WriteLine (pairingwHasNoDetails [currentIndex].Substring (0, 4));
				if (_position == "FA")
				{

					scrapWeb2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
				}
				else
				{

					scrapWeb2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
				}


			
			
			}
		}

		void SecondWebLoaded (object sender, WebFrameEventArgs e)
		{
			finishCount2++;
			if (finishCount2 == 1) {
				//get html string here and pass to method

				string html = scrapWeb2.WindowScriptObject.EvaluateWebScript ("document.body.innerHTML").ToString();
				if (html == string.Empty) {
					finishCount2 = 0;
					if (_position == "FA")
					{
						scrapWeb2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}
					else
					{
						scrapWeb2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}

					return;
				}
				try {
					
					Trip aTrip = scraper.ParseTripDetails (pairingwHasNoDetails [currentIndex], scrapWeb2.WindowScriptObject.EvaluateWebScript ("document.body.innerHTML").ToString(), show1stDay, showAfter1stDay);



					if (aTrip != null) {
						if (parsedDict == null)
							parsedDict = new Dictionary<string, Trip> ();

						parsedDict.Add (pairingwHasNoDetails [currentIndex], aTrip);
					} else {
						parsedDict = null;
					}
				} catch (Exception ex) {
					GlobalSettings.parsedDict = null;
					GlobalSettings.IsScrapStart = false;
					//NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingFailed", null);
//					scrapWeb1.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));
//					scrapWeb2.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));

					//Log Out
					LogOut ();

					return;
				}

				if (parsedDict == null) {
					GlobalSettings.parsedDict = null;
					GlobalSettings.IsScrapStart = false;
					//NSNotificationCenter.DefaultCenter.PostNotificationName ("ScrapingFailed", null);
//					scrapWeb1.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));
//					scrapWeb2.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));

					//Log Out
					LogOut ();

					return;
				}


				if (currentIndex == pairingwHasNoDetails.Count - 1) {
					//call method and return
					GlobalSettings.parsedDict = parsedDict;
					GlobalSettings.IsScrapStart = false;
					//NSNotificationCenter.DefaultCenter.PostNotificationName ("ScrapingSuccess", null);

					//Log Out
					LogOut ();

					return;
				} else {
					finishCount2 = 0;
					currentIndex++;
					Console.WriteLine (pairingwHasNoDetails [currentIndex].Substring (0, 4));

					string day = pairingwHasNoDetails [currentIndex].Substring (4, 2).TrimStart (' ');
					tripDate = month.ToString ("d2") + "%2F" + int.Parse (day).ToString ("d2") + "%2F" + year.ToString ();
					if (_position == "FA")
					{
						scrapWeb2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}
					else
					{
						scrapWeb2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl("https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=" + userName + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
					}
				}	
			}
		}

		private void LogOut()
		{
			try {
				var first = scrapWeb1.MainFrame.ChildFrames.FirstOrDefault (x => x.Name == "portalMainFrame");
				var second = first.ChildFrames [1].ChildFrames.FirstOrDefault (x => x.Name == "appMainFrame");
				var links = second.DomDocument.links;

				foreach (var link in links) {
					if (link.TextContent == "Exit CWA") {
						link.EvaluateWebScript ("closeApp();");
					}
				}
				scrapWeb1.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));
				scrapWeb2.MainFrame.LoadRequest (new NSUrlRequest (new NSUrl (string.Empty)));
			} catch (Exception ex) {
				CommonClass.AppDelegate.ErrorLog (ex);
			}
		}
	}
}

