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
    [Register ("MILConfigViewController")]
    partial class MILConfigViewController
    {
        [Outlet]
        AppKit.NSButton btnAdd { get; set; }

        [Outlet]
        AppKit.NSButton btnApply { get; set; }

        [Outlet]
        AppKit.NSButton btnCalculate2 { get; set; }

        [Outlet]
        AppKit.NSButton btnCalulateNew { get; set; }

        [Outlet]
        AppKit.NSButton btnCancel { get; set; }

        [Outlet]
        AppKit.NSButton btnCancel2 { get; set; }

        [Outlet]
        AppKit.NSButton btnPopCancel { get; set; }

        [Outlet]
        AppKit.NSButton btnPopOK { get; set; }

        [Outlet]
        AppKit.NSDatePicker dpPop { get; set; }

        [Outlet]
        AppKit.NSTextField lblEndDates { get; set; }

        [Outlet]
        AppKit.NSTextField lblStartDates { get; set; }

        [Outlet]
        AppKit.NSTextField objLabelEnd { get; set; }

        [Outlet]
        AppKit.NSTextField objLabelStart { get; set; }

        [Outlet]
        AppKit.NSTextField objLabelTitle { get; set; }

        [Outlet]
        AppKit.NSTableView tblMILDates { get; set; }

        [Outlet]
        AppKit.NSView vwDatePopover { get; set; }

        [Outlet]
        AppKit.NSView vwDateSelect { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (btnAdd != null) {
                btnAdd.Dispose ();
                btnAdd = null;
            }

            if (btnApply != null) {
                btnApply.Dispose ();
                btnApply = null;
            }

            if (btnCalculate2 != null) {
                btnCalculate2.Dispose ();
                btnCalculate2 = null;
            }

            if (btnCalulateNew != null) {
                btnCalulateNew.Dispose ();
                btnCalulateNew = null;
            }

            if (btnCancel != null) {
                btnCancel.Dispose ();
                btnCancel = null;
            }

            if (btnCancel2 != null) {
                btnCancel2.Dispose ();
                btnCancel2 = null;
            }

            if (btnPopCancel != null) {
                btnPopCancel.Dispose ();
                btnPopCancel = null;
            }

            if (btnPopOK != null) {
                btnPopOK.Dispose ();
                btnPopOK = null;
            }

            if (dpPop != null) {
                dpPop.Dispose ();
                dpPop = null;
            }

            if (lblEndDates != null) {
                lblEndDates.Dispose ();
                lblEndDates = null;
            }

            if (lblStartDates != null) {
                lblStartDates.Dispose ();
                lblStartDates = null;
            }

            if (tblMILDates != null) {
                tblMILDates.Dispose ();
                tblMILDates = null;
            }

            if (vwDatePopover != null) {
                vwDatePopover.Dispose ();
                vwDatePopover = null;
            }

            if (vwDateSelect != null) {
                vwDateSelect.Dispose ();
                vwDateSelect = null;
            }

            if (objLabelStart != null) {
                objLabelStart.Dispose ();
                objLabelStart = null;
            }

            if (objLabelEnd != null) {
                objLabelEnd.Dispose ();
                objLabelEnd = null;
            }

            if (objLabelTitle != null) {
                objLabelTitle.Dispose ();
                objLabelTitle = null;
            }
        }
    }

    [Register ("MILConfigView")]
    partial class MILConfigView
    {
        
        void ReleaseDesignerOutlets ()
        {
        }
    }
}
