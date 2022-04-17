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
    [Register ("DaysOfMonthController")]
    partial class DaysOfMonthController
    {
        [Outlet]
        AppKit.NSButton btnDone { get; set; }

        [Outlet]
        AppKit.NSTextFieldCell btnThirdTouch { get; set; }

        [Outlet]
        AppKit.NSTextField lblMonth { get; set; }

        [Outlet]
        AppKit.NSTextField lblWeekDays { get; set; }

        [Outlet]
        AppKit.NSView ViewCalender { get; set; }

        [Action ("funCancelAction:")]
        partial void funCancelAction (Foundation.NSObject sender);

        [Action ("funClearAction:")]
        partial void funClearAction (Foundation.NSObject sender);

        [Action ("funDoneAction:")]
        partial void funDoneAction (Foundation.NSObject sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (btnDone != null) {
                btnDone.Dispose ();
                btnDone = null;
            }

            if (lblMonth != null) {
                lblMonth.Dispose ();
                lblMonth = null;
            }

            if (lblWeekDays != null) {
                lblWeekDays.Dispose ();
                lblWeekDays = null;
            }

            if (ViewCalender != null) {
                ViewCalender.Dispose ();
                ViewCalender = null;
            }

            if (btnThirdTouch != null) {
                btnThirdTouch.Dispose ();
                btnThirdTouch = null;
            }
        }
    }
}
