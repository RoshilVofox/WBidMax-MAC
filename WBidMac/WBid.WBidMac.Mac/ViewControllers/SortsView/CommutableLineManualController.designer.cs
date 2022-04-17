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
    [Register ("CommutableLineManualController")]
    partial class CommutableLineManualController
    {
        [Outlet]
        AppKit.NSButton btnBothEnds { get; set; }

        [Outlet]
        AppKit.NSButton btnCloseCL { get; set; }

        [Outlet]
        AppKit.NSButton btnCommuteHome { get; set; }

        [Outlet]
        AppKit.NSButton btnCommuteWork { get; set; }

        [Outlet]
        AppKit.NSButton btnDoneButton { get; set; }

        [Outlet]
        AppKit.NSButton btnHelp { get; set; }

        [Outlet]
        AppKit.NSButton btnInDom { get; set; }

        [Outlet]
        AppKit.NSButton btnLoadDefaults { get; set; }

        [Outlet]
        AppKit.NSButton btnSaveDefaults { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpFriCheckIn { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpFriToBase { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpMonThuCheckIn { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpMonThuToBase { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpSatCheckIn { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpSatToBase { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpSunCheckIn { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpSunToBase { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (btnBothEnds != null) {
                btnBothEnds.Dispose ();
                btnBothEnds = null;
            }

            if (btnCommuteHome != null) {
                btnCommuteHome.Dispose ();
                btnCommuteHome = null;
            }

            if (btnCommuteWork != null) {
                btnCommuteWork.Dispose ();
                btnCommuteWork = null;
            }

            if (btnInDom != null) {
                btnInDom.Dispose ();
                btnInDom = null;
            }

            if (btnLoadDefaults != null) {
                btnLoadDefaults.Dispose ();
                btnLoadDefaults = null;
            }

            if (btnSaveDefaults != null) {
                btnSaveDefaults.Dispose ();
                btnSaveDefaults = null;
            }

            if (dpFriCheckIn != null) {
                dpFriCheckIn.Dispose ();
                dpFriCheckIn = null;
            }

            if (dpFriToBase != null) {
                dpFriToBase.Dispose ();
                dpFriToBase = null;
            }

            if (dpMonThuCheckIn != null) {
                dpMonThuCheckIn.Dispose ();
                dpMonThuCheckIn = null;
            }

            if (dpMonThuToBase != null) {
                dpMonThuToBase.Dispose ();
                dpMonThuToBase = null;
            }

            if (dpSatCheckIn != null) {
                dpSatCheckIn.Dispose ();
                dpSatCheckIn = null;
            }

            if (dpSatToBase != null) {
                dpSatToBase.Dispose ();
                dpSatToBase = null;
            }

            if (dpSunCheckIn != null) {
                dpSunCheckIn.Dispose ();
                dpSunCheckIn = null;
            }

            if (dpSunToBase != null) {
                dpSunToBase.Dispose ();
                dpSunToBase = null;
            }

            if (btnHelp != null) {
                btnHelp.Dispose ();
                btnHelp = null;
            }

            if (btnCloseCL != null) {
                btnCloseCL.Dispose ();
                btnCloseCL = null;
            }

            if (btnDoneButton != null) {
                btnDoneButton.Dispose ();
                btnDoneButton = null;
            }
        }
    }
}
