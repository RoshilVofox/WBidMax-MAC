
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Foundation;
	using AppKit;
	using WBid.WBidiPad.Core;
	using CoreGraphics;
	using WBid.WBidiPad.iOS.Utility;

	namespace WBid.WBidMac.Mac
	{
		public partial class CSWWindowController : AppKit.NSWindowController
		{
			#region Constructors

			ConstraintsViewController constVC;
			SortsViewController sortVC;
			WeightsViewController weightVC;

			// Called when created from unmanaged code
			public CSWWindowController (IntPtr handle) : base (handle)
			{
				Initialize ();
			}
			
			// Called when created directly from a XIB file
			[Export ("initWithCoder:")]
			public CSWWindowController (NSCoder coder) : base (coder)
			{
				Initialize ();
			}
			
			// Call to load from the XIB/NIB file
			public CSWWindowController () : base ("CSWWindow")
			{
				Initialize ();
			}
			
			// Shared initialization code
			void Initialize ()
			{
			}

			#endregion

			//strongly typed window accessor
			public new CSWWindow Window {
				get {
					return (CSWWindow)base.Window;
				}
			}
			static NSButton closeButton;
			public override void AwakeFromNib ()
			{
				try {
					base.AwakeFromNib ();
					ScreenSizeManagement();
					this.ShouldCascadeWindows = false;
					sgViewSelect.Activated += (object sender, EventArgs e) => {
						ChangeView ();
					};

					closeButton = this.Window.StandardWindowButton (NSWindowButton.CloseButton);
					closeButton.Activated += (sender, e) => {
						SetScreenSize();
						if(constVC != null)
						constVC.RemoveNotifications();

						if(sortVC !=null)
						sortVC.RemoveNotifications();

						if(weightVC != null)
						weightVC.RemoveNotifications();
					CommonClass.CSWController = null;
						sortVC = null;
						CommonClass.MainController.cswWC = null;
						this.Window.Close ();
					};


					if (constVC == null)
					{
						constVC = new ConstraintsViewController();
						CommonClass.ConstraintsController = constVC;
						
					} 
					if (sortVC == null){
						sortVC = new SortsViewController();
						CommonClass.SortController = sortVC;
					} 

						if (weightVC == null)
				{
							weightVC = new WeightsViewController();
							CommonClass.WeightsController = weightVC;
						
					}


					ChangeView ();
				} catch (Exception ex) {
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}
			}

		public void CloseCSW()
		{
            SetScreenSize();
						if(constVC != null)
						constVC.RemoveNotifications();

						if(sortVC !=null)
						sortVC.RemoveNotifications();

						if(weightVC != null)
						weightVC.RemoveNotifications();
			

						this.Window.Close ();
		}

			public void ResetSortSet()
			{
				
			}

			void ScreenSizeManagement()
			{

				if (GlobalSettings.WBidINIContent.MainWindowSize.IsMaximised == true) {
					this.Window.IsZoomed = true;
				} else {
					if (GlobalSettings.WBidINIContent.CSWViewSize.Height > 0) {
						CGRect ScreenFrame = new CGRect (GlobalSettings.WBidINIContent.CSWViewSize.Left, GlobalSettings.WBidINIContent.CSWViewSize.Top, GlobalSettings.WBidINIContent.CSWViewSize.Width, GlobalSettings.WBidINIContent.CSWViewSize.Height);
						this.Window.SetFrame (ScreenFrame, true);

					} else {

						SetScreenSize ();

					}
				}
			}
			void SetScreenSize()
			{
				GlobalSettings.WBidINIContent.CSWViewSize.Left =(int) this.Window.Frame.X;
				GlobalSettings.WBidINIContent.CSWViewSize.Top = (int)this.Window.Frame.Y;
				GlobalSettings.WBidINIContent.CSWViewSize.Width = (int)this.Window.Frame.Width;
				GlobalSettings.WBidINIContent.CSWViewSize.Height = (int)this.Window.Frame.Height;
				GlobalSettings.WBidINIContent.CSWViewSize.IsMaximised = this.Window.IsZoomed;	
				//save the state of the INI File
				WBidHelper.SaveINIFile (GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath ());

			}
			public void ChangeView ()
			{
				try {
					if (sgViewSelect.SelectedSegment == 0) {
					if (constVC == null)
							constVC = new ConstraintsViewController();
						CommonClass.ConstraintsController = constVC;

						this.Window.ContentView = constVC.View;
						constVC.tblConstraints.ReloadData ();
					} else if (sgViewSelect.SelectedSegment == 1) {
					if (sortVC == null)
							sortVC = new SortsViewController ();
						CommonClass.SortController = sortVC;
						this.Window.ContentView = sortVC.View;
						sortVC.tblBlockSort.ReloadData();
					} else if (sgViewSelect.SelectedSegment == 2) {
						if (weightVC == null)
							weightVC = new WeightsViewController ();
						CommonClass.WeightsController = weightVC;
						this.Window.ContentView = weightVC.View;
						weightVC.tblWeights.ReloadData ();
					}
				} catch (Exception ex) 
				{
					CommonClass.AppDelegate.ErrorLog (ex);
					CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
				}

			}



		public void ReloadAllContent()		{


		CommonClass.CSWController.CloseCSW ();
					CommonClass.CSWController = null;
                    this.PerformSelector (new ObjCRuntime.Selector ("ShowCSWView"), null, 0.05);

		}


		[Export("ShowCSWView")]
void ShowCSWView()
{
	CommonClass.MainController.ShowCSWFromReset();
		}





			//public void ReloadAllContent ()
			//{







			//	try {
			//		constVC = null;
			//		sortVC = null;
			//		weightVC = null;
					
			//		if (sgViewSelect.SelectedSegment == 0) {
			//			constVC = new ConstraintsViewController ();
			//			CommonClass.ConstraintsController = constVC;

			//			this.Window.ContentView = constVC.View;
			//			constVC.tblConstraints.ReloadData ();
			//		} else if (sgViewSelect.SelectedSegment == 1) {
			//			sortVC = new SortsViewController ();
			//			CommonClass.SortController = sortVC;
			//			this.Window.ContentView = sortVC.View;
			//		} else if (sgViewSelect.SelectedSegment == 2) {
			//			weightVC = new WeightsViewController ();
			//			CommonClass.WeightsController = weightVC;
			//			this.Window.ContentView = weightVC.View;
			//		}
			//	} catch (Exception ex) {
			//		CommonClass.AppDelegate.ErrorLog (ex);
			//		CommonClass.AppDelegate.ShowErrorMessage (WBidErrorMessages.CommonError);
			//	}
			//}


		}

		//	public static class Extensions {
		//		public static string LocalValue (this NSDate date) {
		//			var d = DateTime.Parse (date.ToString());
		//			var convertedTime = TimeZoneInfo.ConvertTime (d, TimeZoneInfo.Local);
		//			return convertedTime.ToString ("HH:mm");
		//		}
		//	}
	}

