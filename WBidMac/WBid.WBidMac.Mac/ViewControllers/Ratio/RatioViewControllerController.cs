using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.PortableLibrary;
using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidMac.Mac
{
	public partial class RatioViewControllerController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public RatioViewControllerController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public RatioViewControllerController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public RatioViewControllerController() : base("RatioViewController", NSBundle.MainBundle)
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}

		#endregion
		public NSPanel panel;
		public string FromScreen;
		public override void AwakeFromNib()
		{
			try
			{
				base.AwakeFromNib();
                List<ColumnDefinition> Numerator  = WBidCollection.GetRatioFeatureColumn();
                Numerator.Insert(0, new ColumnDefinition { Id = 0, DisplayName = "Select" });

                btnDenominator.RemoveAllItems();
				btnNumerator.RemoveAllItems();
				  btnNumerator.AddItems(Numerator.Select(x => x.DisplayName).ToArray());
				btnDenominator.AddItems(Numerator.Select(x => x.DisplayName).ToArray()); 				btnNumerator.Activated += btnNumeratorClicked; 
				//btnDenominator.AddItems();
				btnDenominator.Activated += btnDenominatorClicked;

				btnOkAction.Activated += btnOkClicked;

				btnCancelAction.Activated += btnCancelClicked;

			}
			catch (Exception ex)
			{
				CommonClass.AppDelegate.ErrorLog(ex);
				CommonClass.AppDelegate.ShowErrorMessage(WBidErrorMessages.CommonError);
			} 		}

		void btnNumeratorClicked(object sender, EventArgs e)
		{
			Console.WriteLine(btnNumerator.TitleOfSelectedItem);
 		}

		void btnDenominatorClicked(object sender, EventArgs e)
		{
			Console.WriteLine(btnDenominator.TitleOfSelectedItem);
		}

        void btnOkClicked(object sender, EventArgs e)
        {
			if (btnNumerator.TitleOfSelectedItem != "Select" && btnDenominator.TitleOfSelectedItem != "Select")
            {
                
               if (GlobalSettings.WBidINIContent.RatioValues == null)
                   GlobalSettings.WBidINIContent.RatioValues = new Ratio();
                CalculateRatioValuesForAllLines();
				if (panel != null)
				{
					NSApplication.SharedApplication.EndSheet(panel);
					panel.OrderOut(this);
				}

				SendNotification("OK");
		    }
            else
            {


						var alert = new NSAlert();
						alert.AlertStyle = NSAlertStyle.Informational;
						alert.Window.Title = "WBidMax";
						alert.MessageText = "Either select numerator or denominator values - or click Cancel.";
						alert.AddButton ("Ok");
						alert.RunModal ();
				return;
              
            }



         }
		private void CalculateRatioValuesForAllLines()
		{
			var numeratorcolumn = WBidCollection.GetRatioFeatureColumn().FirstOrDefault(x => x.DisplayName == btnNumerator.TitleOfSelectedItem);
			var denominatorcolumn = WBidCollection.GetRatioFeatureColumn().FirstOrDefault(x => x.DisplayName == btnDenominator.TitleOfSelectedItem);
			if (numeratorcolumn != null && denominatorcolumn != null)
			{
				GlobalSettings.WBidINIContent.RatioValues.Numerator = numeratorcolumn.Id;
				GlobalSettings.WBidINIContent.RatioValues.Denominator = denominatorcolumn.Id;
				foreach (var line in GlobalSettings.Lines)
				{
					var numerator = line.GetType().GetProperty(numeratorcolumn.DataPropertyName).GetValue(line, null);
					if (numeratorcolumn.DataPropertyName == "TafbInBp")
						numerator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
					decimal numeratorValue = Convert.ToDecimal(numerator);

					var denominator = line.GetType().GetProperty(denominatorcolumn.DataPropertyName).GetValue(line, null);
					if (denominatorcolumn.DataPropertyName == "TafbInBp")
						denominator = Helper.ConvertHhhColonMmToFractionalHours(line.TafbInBp);
					decimal denominatorValue = Convert.ToDecimal(denominator);

			        line.Ratio = Math.Round(decimal.Parse(String.Format("{0:0.00}", (denominatorValue == 0) ? 0 : numeratorValue / denominatorValue)), 2, MidpointRounding.AwayFromZero);


				}
			}
		}
		void SendNotification(string StateData)
		{
			string notificaitonName = "";
			if (FromScreen == "BidLine")
			{
				notificaitonName = "BidLineRatioNotification"; 
			}
			else if (FromScreen == "Modern")
			{
				notificaitonName = "ModernRatioNotification";
			}
			else if (FromScreen == "Sort")
			{
				notificaitonName = "RatioNotification";
			}
			else if (FromScreen == "Summary")
			{
				notificaitonName = "SummaryRatioNotification";
			}

			NSNotificationCenter.DefaultCenter.PostNotificationName(notificaitonName, new Foundation.NSString(StateData));
		}
		//private void CalculateRatioValuesForAllLines()
		//{
		//	MainViewModel mainviewmodel = ServiceLocator.Current.GetInstance<MainViewModel>();
		//	ObservableCollection<Line> lines = mainviewmodel.Lines;
		//	foreach (var line in lines)
		//	{
		//		decimal numeratorValue = Convert.ToDecimal(line.GetType().GetProperty(SelectedNumerator.DataPropertyName).GetValue(line, null));
		//		decimal denominatorValue = Convert.ToDecimal(line.GetType().GetProperty(SelectedDenominator.DataPropertyName).GetValue(line, null));
		//		line.Ratio = Math.Round(decimal.Parse(String.Format("{0:0.00}", (denominatorValue == 0) ? 0 : numeratorValue / denominatorValue)), 2, MidpointRounding.AwayFromZero);

		//	}
		//}
         void btnCancelClicked(object sender, EventArgs e)
		{
			NSApplication.SharedApplication.EndSheet (panel);
			panel.OrderOut (this);
			SendNotification("CANCEL");
		}


		//strongly typed view accessor
		public new RatioViewController View
		{
			get
			{
				return (RatioViewController)base.View;
			}
		}
	}
}
