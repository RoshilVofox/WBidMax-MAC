// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac
{
    [Register ("HomeWindowController")]
    partial class HomeWindowController
    {
        [Outlet]
        AppKit.NSArrayController arrBidList { get; set; }

        [Outlet]
        AppKit.NSButton btnCollection { get; set; }

        [Outlet]
        AppKit.NSButton btnDeleteBid { get; set; }

        [Outlet]
        AppKit.NSButton btnEdit { get; set; }

        [Outlet]
        AppKit.NSButton btnNewBid { get; set; }

        [Outlet]
        AppKit.NSPopUpButton btnRetrieve { get; set; }

        [Outlet]
        AppKit.NSTextField lblBidName { get; set; }

        [Outlet]
        AppKit.NSTextField lblMonthYear { get; set; }

        [Outlet]
        AppKit.NSView vwCollectionItem { get; set; }

        [Outlet]
        AppKit.NSCollectionView vwHomeCollection { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (arrBidList != null) {
                arrBidList.Dispose ();
                arrBidList = null;
            }

            if (btnCollection != null) {
                btnCollection.Dispose ();
                btnCollection = null;
            }

            if (btnDeleteBid != null) {
                btnDeleteBid.Dispose ();
                btnDeleteBid = null;
            }

            if (btnEdit != null) {
                btnEdit.Dispose ();
                btnEdit = null;
            }

            if (btnNewBid != null) {
                btnNewBid.Dispose ();
                btnNewBid = null;
            }

            if (btnRetrieve != null) {
                btnRetrieve.Dispose ();
                btnRetrieve = null;
            }

            if (lblBidName != null) {
                lblBidName.Dispose ();
                lblBidName = null;
            }

            if (vwCollectionItem != null) {
                vwCollectionItem.Dispose ();
                vwCollectionItem = null;
            }

            if (vwHomeCollection != null) {
                vwHomeCollection.Dispose ();
                vwHomeCollection = null;
            }

            if (lblMonthYear != null) {
                lblMonthYear.Dispose ();
                lblMonthYear = null;
            }
        }
    }

    [Register ("HomeWindow")]
    partial class HomeWindow
    {
        
        void ReleaseDesignerOutlets ()
        {
        }
    }
}
