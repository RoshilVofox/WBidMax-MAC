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
    public partial class BASortCell : AppKit.NSTableCellView
	{
        public BASortCell ()
		{
		}


		// Called when created from unmanaged code
        public BASortCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
        public BASortCell (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
//			WBidState wBidStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
//			ObservableCollection<BlockSort> lstblockData = WBidCollection.GetBlockSortListData ();

			btnClose.Activated += (object sender, EventArgs e) => {
//				CommonClass.SortController.blockSort.Remove (lblTitle.StringValue);
//				wBidStateContent.SortDetails.BlokSort.Remove (lstblockData.FirstOrDefault (x => x.Name == lblTitle.StringValue).Id.ToString ());
//				CommonClass.SortController.ApplyBlockSort ();
				CommonClass.BASortController.RemoveBlockSort(lblTitle.StringValue);
			};
		}

		public void BindData (string sort, int index)
		{
			lblTitle.StringValue = sort;
		}
	}
}

