// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
	[Register ("ArrivalAndDepartViewController")]
	partial class ArrivalAndDepartViewController
	{
		[Outlet]
		AppKit.NSCollectionView ArrivalAndDepartedCollectionView { get; set; }

		[Outlet]
		AppKit.NSCollectionViewItem CollectionViewItem { get; set; }

		[Outlet]
		AppKit.NSTextField lblBase { get; set; }

		[Outlet]
		AppKit.NSTextField lblCommuteCity { get; set; }

		[Outlet]
		AppKit.NSTextField lblDate { get; set; }

		[Action ("btnOk:")]
		partial void btnOk (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ArrivalAndDepartedCollectionView != null) {
				ArrivalAndDepartedCollectionView.Dispose ();
				ArrivalAndDepartedCollectionView = null;
			}

			if (CollectionViewItem != null) {
				CollectionViewItem.Dispose ();
				CollectionViewItem = null;
			}

			if (lblBase != null) {
				lblBase.Dispose ();
				lblBase = null;
			}

			if (lblCommuteCity != null) {
				lblCommuteCity.Dispose ();
				lblCommuteCity = null;
			}

			if (lblDate != null) {
				lblDate.Dispose ();
				lblDate = null;
			}
		}
	}
}
