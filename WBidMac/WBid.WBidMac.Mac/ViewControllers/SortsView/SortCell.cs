using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidMac.Mac
{
	public partial class SortCell : AppKit.NSTableCellView
	{
		public SortCell ()
		{
		}
		public static WBidState wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		string[] arrCommutabilityCellValue = { "Front", "Back", "Overall" };
		// Called when created from unmanaged code
		public SortCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SortCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
            //			ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListData ();
            switch (this.Identifier)
            {

                case "Sort":
                    btnClose.Activated += (object sender, EventArgs e) =>
                    {
                        CommonClass.SortController.RemoveBlockSort(lblTitle.StringValue);
                    };
                    break;
                case "CommutabilitySort":

                    btnClose1.Activated += (object sender, EventArgs e) =>
            {
                CommonClass.SortController.RemoveBlockSort(("Commutability"));
            };
                    break;
                case "Commutable Line-Auto":

                    btnClose1.Activated += (object sender, EventArgs e) =>
                    {
                        CommonClass.SortController.RemoveBlockSort(("Commutable Line-Auto"));
                    };
                    break;

                case "CommutableManual":

                    btnCloseManual.Activated += (object sender, EventArgs e) =>
                    {
                        CommonClass.SortController.RemoveBlockSort(("Commutable Line- Manual"));
                    };
                    break;
            }
        }

		partial void TitleActn(NSObject sender)
		{
            NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommuteInfoView", null);
		}


        partial void ManualTitleAction(NSObject sender)
        {
            NSNotificationCenter.DefaultCenter.PostNotificationName("ShowCommutableManualView", null);
        }

        public void BindData(string sort, int index)
		{
			//wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			switch (this.Identifier)
			{
				case "Sort":
					lblTitle.StringValue = sort;
					break;
				case "CommutabilitySort":
                case "Commutable Line-Auto":
                    CommutableAutoTitle.Title = "Comut%(" + wBIdStateContent.Constraints.CLAuto.City + ")";
					
					string NoFronOrBack;

					if (wBIdStateContent.SortDetails.BlokSort.Contains("34"))
						NoFronOrBack = "Front";
					else if (wBIdStateContent.SortDetails.BlokSort.Contains("35"))
						NoFronOrBack = "Back";
					else
						NoFronOrBack = "Overall";

					CommutableAutoValue.RemoveAllItems();
					CommutableAutoValue.AddItems(arrCommutabilityCellValue);
					CommutableAutoValue.SelectItem(NoFronOrBack);
					CommutableAutoValue.Activated += (object sender, EventArgs e) =>
					{
						//remove previous commtaiblity sort item 
						wBIdStateContent.SortDetails.BlokSort.RemoveAll(x => x == "33" || x == "34" || x == "35");
						if (CommutableAutoValue.IndexOfSelectedItem == 0)
						{
							wBIdStateContent.SortDetails.BlokSort.Add("34");
						}
						else if (CommutableAutoValue.IndexOfSelectedItem == 1)
						{
							wBIdStateContent.SortDetails.BlokSort.Add("35");
						}
						else if (CommutableAutoValue.IndexOfSelectedItem == 2)
						{
							wBIdStateContent.SortDetails.BlokSort.Add("33");
						}
						NSNotificationCenter.DefaultCenter.PostNotificationName("ApplyBlockSort", null);

					};

					break;
                case "CommutableManual":

                    CommutableManualTitle.Title = "Commutable Line- Manual";

                    string NotFronOrBack;

                    if (wBIdStateContent.SortDetails.BlokSort.Contains("37"))
                        NotFronOrBack = "Front";
                    else if (wBIdStateContent.SortDetails.BlokSort.Contains("38"))
                        NotFronOrBack = "Back";
                    else
                        NotFronOrBack = "Overall";

                    CommutableManualValue.RemoveAllItems();
                    CommutableManualValue.AddItems(arrCommutabilityCellValue);
                    CommutableManualValue.SelectItem(NotFronOrBack);
                    CommutableManualValue.Activated += (object sender, EventArgs e) =>
                    {
                        //remove previous commtaiblity sort item 
                        wBIdStateContent.SortDetails.BlokSort.RemoveAll(x => x == "36" || x == "37" || x == "38");
                        if (CommutableManualValue.IndexOfSelectedItem == 0)
                        {
                            wBIdStateContent.SortDetails.BlokSort.Add("37");
                        }
                        else if (CommutableManualValue.IndexOfSelectedItem == 1)
                        {
                            wBIdStateContent.SortDetails.BlokSort.Add("38");
                        }
                        else if (CommutableManualValue.IndexOfSelectedItem == 2)
                        {
                            wBIdStateContent.SortDetails.BlokSort.Add("36");
                        }
                        NSNotificationCenter.DefaultCenter.PostNotificationName("ApplyBlockSort", null);

                    };
                    break;
            }
		}
	}
}


