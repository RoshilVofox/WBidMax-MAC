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
    [Register ("SortCell")]
    partial class SortCell
    {
        [Outlet]
        AppKit.NSButton btnClose { get; set; }

        [Outlet]
        AppKit.NSButton btnClose1 { get; set; }

        [Outlet]
        AppKit.NSButton btnCloseManual { get; set; }

        [Outlet]
        AppKit.NSButton CommutableAutoTitle { get; set; }

        [Outlet]
        AppKit.NSPopUpButton CommutableAutoValue { get; set; }

        [Outlet]
        AppKit.NSButton CommutableManualTitle { get; set; }

        [Outlet]
        AppKit.NSPopUpButton CommutableManualValue { get; set; }

        [Outlet]
        AppKit.NSTextField lblTitle { get; set; }

        [Action ("ManualTitleAction:")]
        partial void ManualTitleAction (Foundation.NSObject sender);

        [Action ("TitleActn:")]
        partial void TitleActn (Foundation.NSObject sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (btnClose != null) {
                btnClose.Dispose ();
                btnClose = null;
            }

            if (btnClose1 != null) {
                btnClose1.Dispose ();
                btnClose1 = null;
            }

            if (CommutableAutoTitle != null) {
                CommutableAutoTitle.Dispose ();
                CommutableAutoTitle = null;
            }

            if (CommutableAutoValue != null) {
                CommutableAutoValue.Dispose ();
                CommutableAutoValue = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }

            if (btnCloseManual != null) {
                btnCloseManual.Dispose ();
                btnCloseManual = null;
            }

            if (CommutableManualValue != null) {
                CommutableManualValue.Dispose ();
                CommutableManualValue = null;
            }

            if (CommutableManualTitle != null) {
                CommutableManualTitle.Dispose ();
                CommutableManualTitle = null;
            }
        }
    }
}
