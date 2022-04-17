using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.PortableLibrary.Parser;
using WBid.WBidiPad.Model;
using WebKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class ContractorEmpScrapController : AppKit.NSViewController
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
		PraseScrapedTripDetails scraper = new PraseScrapedTripDetails();

		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public ContractorEmpScrapController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public ContractorEmpScrapController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		//		// Call to load from the XIB/NIB file
		//		public ContractorEmpScrapController () : base ("ContractorEmpScrap", NSBundle.MainBundle)
		//		{
		//			Initialize ();
		//		}

		// Shared initialization code
		void Initialize()
		{
		}

		#endregion

		//strongly typed view accessor
		public new ContractorEmpScrap View
		{
			get
			{
				return (ContractorEmpScrap)base.View;
			}
		}

		public ContractorEmpScrapController(string uname, string pass, List<string> strArr, int mn, int yr, int s1D, int sA1D, string position) : base("ContractorEmpScrap", null)
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
		public override void AwakeFromNib()
		{

			base.AwakeFromNib();
			try
			{

				WebBrowser1.FinishedLoad += FirstWebLoaded;
				WebBrowser2.FinishedLoad += SecondWebLoaded;
				WebBrowser3.FinishedLoad += ThirdWebLoaded;
				//string url = "https://login.swalife.com/myswa_lifelogin.htm";
				string url = "https://www.swacrew.com";
				WebBrowser1.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(url)));
			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
			}

		}

		void FirstWebLoaded(object sender, WebFrameEventArgs e)
		{

			WebBrowser1.WindowScriptObject.EvaluateWebScript("window.alert = function() {};");
			finishCount++;
			if (finishCount == 2)
			{

				string setUser = "window.document.getElementById(\"ContentPlaceHolder1_MFALoginControl1_UserIDView_txtUserid\").value =" + "\"" + userName + "\"" + ";";
				string setPass = "document.querySelectorAll('[type=password]')[0].value =" + "\"" + password + "\"" + ";";


				WebBrowser1.WindowScriptObject.EvaluateWebScript(setUser);//Set value to it

				WebBrowser1.WindowScriptObject.EvaluateWebScript(setPass);
				WebBrowser1.WindowScriptObject.EvaluateWebScript("var field = window.document.getElementById(\"ContentPlaceHolder1_MFALoginControl1_UserIDView_btnSubmit\"); field.click();");
				//string setUser = "window.document.getElementById(\"useridField\").value =" + "\"" + userName + "\"" + ";";
				//string setPass = "document.querySelectorAll('[type=password]')[0].value =" + "\"" + password + "\"" + ";";


				//WebBrowser1.WindowScriptObject.EvaluateWebScript (setUser);//Set value to it

				//WebBrowser1.WindowScriptObject.EvaluateWebScript (setPass);
				//WebBrowser1.WindowScriptObject.EvaluateWebScript ("goSubmit()");

			}
			else if (finishCount == 3)
			{
				//WebBrowser1.WindowScriptObject.EvaluateWebScript ("var field = document.getElementsByClassName('eipTabBar')[5].children[0]; field.click();");

				//WebBrowser1.WindowScriptObject.EvaluateWebScript  ("var field = document.getElementsByClassName(\"navLink newPageLink level3Link\")[34]; field.click();");
				if (_position == "FA")
				{
					string url = "https://www.prod1.swacrew.com/prod1-if/CSSWebClient/eipSessionJsp/EipFrameset.jsp?eipFrameUrl=../loginAsNonScheduler.do?isPortal=truehome&role=webhdq&uid=x21221";
					WebBrowser2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(url)));
					///WebBrowser1.WindowScriptObject.EvaluateWebScript("var field = Array.prototype.filter.call(document.getElementsByClassName(\"navLink newPageLink level3Link\"),function(element){ return element.text=='Launch CSS Web IF HDQ';})[0];field.click();");
				}
				else
				{
					string url = "https://www.prod1.swacrew.com/prod1-fo/CSSWebClient/eipSessionJsp/EipFrameset.jsp?eipFrameUrl=../loginAsNonScheduler.do?isPortal=truehome&role=webhdq&uid=x21221";
					WebBrowser2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(url)));

					//WebBrowser1.WindowScriptObject.EvaluateWebScript("var field = Array.prototype.filter.call(document.getElementsByClassName(\"navLink newPageLink level3Link\"),function(element){ return element.text=='Launch CSS Web FO HDQ';})[0];field.click();");
				}
				currentIndex = 0;


			}
			//else if (finishCount == 7) {

			//	string ss=WebBrowser1.WindowScriptObject.EvaluateWebScript ("popUpUrl").ToString();
			//	WebBrowser2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(ss)));
			//	//WebBrowser1.WindowScriptObject.EvaluateWebScript ("window.alert = function() {}; var field = document.getElementById(\"Link5\"); field.click();");

			//}
			//			else if (finishCount == 12) {
			//
			//				WebBrowser1.WindowScriptObject.EvaluateWebScript ("window.alert = function() {}; var field = document.getElementById(\"Menu5\").children[0].getElementsByTagName('a')[0]; field.click();");
			//			}
			//			else if (finishCount == 16) 
			//			{
			//
			//				WebBrowser1.WindowScriptObject.EvaluateWebScript ("window.alert = function() {};var field = document.getElementsByClassName('eipNavItemMenu eipFlatItem')[1].children[0]; field.click();");
			//			}
			//
			//			else if (finishCount == 18) 
			//			{
			//				string ss=WebBrowser1.WindowScriptObject.EvaluateWebScript ("popUpUrl").ToString();
			//				WebBrowser2.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(ss)));
			//			}







		}
		void SecondWebLoaded(object sender, WebFrameEventArgs e)
		{
			finishCount2++;
			if (finishCount2 == 2)
			{
				//get html string here and pass to method

				//pairingwHasNoDetails.Add("BA1101");
				//pairingwHasNoDetails.Add("BA1201");
				//pairingwHasNoDetails.Add("BA1101");
				//finishCount2 = 0;
				string day = pairingwHasNoDetails[currentIndex].Substring(4, 2).TrimStart(' ');
				string url = string.Empty;
				if (_position == "FA")
				{
					//url = "https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-if/CSSWebClient/getPairingDetails.do?crewMemberId=";
				}
				else
				{
					//url = "https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-fo/CSSWebClient/getPairingDetails.do?crewMemberId=";
				}
				tripDate = month.ToString("d2") + "%2F" + int.Parse(day).ToString("d2") + "%2F" + year.ToString();
				WebBrowser3.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(url + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
				return;
			}
		}
		void ThirdWebLoaded(object sender, WebFrameEventArgs e)
		{
			try
			{
				Trip aTrip;
				if (_position == "FA")
				{
					aTrip = scraper.ParseTripDetailsForFA(pairingwHasNoDetails[currentIndex], WebBrowser3.WindowScriptObject.EvaluateWebScript("document.body.innerHTML").ToString(), show1stDay, showAfter1stDay);
				}
				else
				{
					aTrip = scraper.ParseTripDetails(pairingwHasNoDetails[currentIndex], WebBrowser3.WindowScriptObject.EvaluateWebScript("document.body.innerHTML").ToString(), show1stDay, showAfter1stDay);
				}

				if (aTrip != null)
				{
					if (parsedDict == null)
						parsedDict = new Dictionary<string, Trip>();

					parsedDict.Add(pairingwHasNoDetails[currentIndex], aTrip);
				}
				else
				{
					parsedDict = null;
				}
			}
			catch (Exception ex)
			{
				GlobalSettings.parsedDict = null;
				GlobalSettings.IsScrapStart = false;
				NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingFailed", null);
				Logout();

				return;
			}

			if (parsedDict == null)
			{
				GlobalSettings.parsedDict = null;
				GlobalSettings.IsScrapStart = false;
				NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingFailed", null);
				Logout();

				return;
			}

			if (currentIndex == pairingwHasNoDetails.Count - 1)
			{
				GlobalSettings.parsedDict = parsedDict;
				GlobalSettings.IsScrapStart = false;
				NSNotificationCenter.DefaultCenter.PostNotificationName("ScrapingSuccess", null);

				Logout();

				return;
			}
			else
			{
				finishCount2 = 0;
				currentIndex++;
				Console.WriteLine(pairingwHasNoDetails[currentIndex].Substring(0, 4));
				string day = pairingwHasNoDetails[currentIndex].Substring(4, 2).TrimStart(' ');
				string url = string.Empty;
				if (_position == "FA")
				{
					//url = "https://www15.swalife.com/csswa/ea/fa/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-if/CSSWebClient/getPairingDetails.do?crewMemberId=";
				}
				else
				{
					//url = "https://www15.swalife.com/csswa/ea/plt/getPairingDetails.do?crewMemberId=";
					url = "https://www.prod1.swacrew.com/prod1-fo/CSSWebClient/getPairingDetails.do?crewMemberId=";
				}
				tripDate = month.ToString("d2") + "%2F" + int.Parse(day).ToString("d2") + "%2F" + year.ToString();
				WebBrowser3.MainFrame.LoadRequest(new NSUrlRequest(new NSUrl(url + "&tripDate=" + tripDate + "&tripNumber=" + pairingwHasNoDetails[currentIndex].Substring(0, 4) + "&tripDateInput=" + tripDate)));
			}
		}

		private void Logout()
		{
			//WebBrowser1.WindowScriptObject.EvaluateWebScript ("window.alert = function() {};var field = document.getElementsByClassName('eipToolBarLink_LogOut')[0]; field.click();");

		}

	}
}
