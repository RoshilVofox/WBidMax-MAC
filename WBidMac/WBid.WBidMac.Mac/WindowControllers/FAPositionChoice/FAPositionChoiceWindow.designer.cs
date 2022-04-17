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
	[Register ("FAPositionChoiceWindowController")]
	partial class FAPositionChoiceWindowController
	{
		[Outlet]
		AppKit.NSButton btnAdd { get; set; }

		[Outlet]
		AppKit.NSButton btnAddReserve { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnBuddy1 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnBuddy2 { get; set; }

		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSButton btnCancelCustom { get; set; }

		[Outlet]
		AppKit.NSButton btnChangeBuddy { get; set; }

		[Outlet]
		AppKit.NSButton btnClear { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnFirst { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnFourth { get; set; }

		[Outlet]
		AppKit.NSButton btnInsert { get; set; }

		[Outlet]
		AppKit.NSButton btnOKCustom { get; set; }

		[Outlet]
		AppKit.NSMatrix btnOrder { get; set; }

		[Outlet]
		AppKit.NSButton btnRemove { get; set; }

		[Outlet]
		AppKit.NSMatrix btnRepeatCombo { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnSecond { get; set; }

		[Outlet]
		AppKit.NSButton btnSubmit { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnThird { get; set; }

		[Outlet]
		AppKit.NSTextField lblAvailable { get; set; }

		[Outlet]
		AppKit.NSTextField lblBids { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddy1 { get; set; }

		[Outlet]
		AppKit.NSTextField lblBuddy2 { get; set; }

		[Outlet]
		AppKit.NSTextField lblPositionTotal { get; set; }

		[Outlet]
		AppKit.NSTextField lblSubmitTotal { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExist1 { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExist2 { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExist3 { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExist4 { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExistFirstPos { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExistFourthPos { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExistSecondPos { get; set; }

		[Outlet]
		AppKit.NSTextField lblValueExistThirdPos { get; set; }

		[Outlet]
		AppKit.NSTableView tblAvailableLine { get; set; }

		[Outlet]
		AppKit.NSTableView tblSelectedLine { get; set; }

		[Outlet]
		AppKit.NSTextField txtFirstpositionValue { get; set; }

		[Outlet]
		AppKit.NSTextField txtFourthPositionValue { get; set; }

		[Outlet]
		AppKit.NSTextField txtSecondPositionValue { get; set; }

		[Outlet]
		AppKit.NSTextField txtThirdPositionValue { get; set; }

		[Outlet]
		AppKit.NSView vwCustom { get; set; }

		[Outlet]
		AppKit.NSView vwMain { get; set; }

		[Outlet]
		AppKit.NSView vwPositionSelection { get; set; }

		[Outlet]
		AppKit.NSView vwRepeatPosition { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblValueExist4 != null) {
				lblValueExist4.Dispose ();
				lblValueExist4 = null;
			}

			if (lblValueExist3 != null) {
				lblValueExist3.Dispose ();
				lblValueExist3 = null;
			}

			if (lblValueExist2 != null) {
				lblValueExist2.Dispose ();
				lblValueExist2 = null;
			}

			if (lblValueExist1 != null) {
				lblValueExist1.Dispose ();
				lblValueExist1 = null;
			}

			if (lblValueExistFourthPos != null) {
				lblValueExistFourthPos.Dispose ();
				lblValueExistFourthPos = null;
			}

			if (lblValueExistThirdPos != null) {
				lblValueExistThirdPos.Dispose ();
				lblValueExistThirdPos = null;
			}

			if (lblValueExistSecondPos != null) {
				lblValueExistSecondPos.Dispose ();
				lblValueExistSecondPos = null;
			}

			if (lblValueExistFirstPos != null) {
				lblValueExistFirstPos.Dispose ();
				lblValueExistFirstPos = null;
			}

			if (txtFourthPositionValue != null) {
				txtFourthPositionValue.Dispose ();
				txtFourthPositionValue = null;
			}

			if (txtThirdPositionValue != null) {
				txtThirdPositionValue.Dispose ();
				txtThirdPositionValue = null;
			}

			if (txtSecondPositionValue != null) {
				txtSecondPositionValue.Dispose ();
				txtSecondPositionValue = null;
			}

			if (txtFirstpositionValue != null) {
				txtFirstpositionValue.Dispose ();
				txtFirstpositionValue = null;
			}

			if (lblPositionTotal != null) {
				lblPositionTotal.Dispose ();
				lblPositionTotal = null;
			}

			if (lblSubmitTotal != null) {
				lblSubmitTotal.Dispose ();
				lblSubmitTotal = null;
			}

			if (vwPositionSelection != null) {
				vwPositionSelection.Dispose ();
				vwPositionSelection = null;
			}

			if (vwRepeatPosition != null) {
				vwRepeatPosition.Dispose ();
				vwRepeatPosition = null;
			}

			if (btnAdd != null) {
				btnAdd.Dispose ();
				btnAdd = null;
			}

			if (btnAddReserve != null) {
				btnAddReserve.Dispose ();
				btnAddReserve = null;
			}

			if (btnBuddy1 != null) {
				btnBuddy1.Dispose ();
				btnBuddy1 = null;
			}

			if (btnBuddy2 != null) {
				btnBuddy2.Dispose ();
				btnBuddy2 = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnCancelCustom != null) {
				btnCancelCustom.Dispose ();
				btnCancelCustom = null;
			}

			if (btnChangeBuddy != null) {
				btnChangeBuddy.Dispose ();
				btnChangeBuddy = null;
			}

			if (btnClear != null) {
				btnClear.Dispose ();
				btnClear = null;
			}

			if (btnFirst != null) {
				btnFirst.Dispose ();
				btnFirst = null;
			}

			if (btnFourth != null) {
				btnFourth.Dispose ();
				btnFourth = null;
			}

			if (btnInsert != null) {
				btnInsert.Dispose ();
				btnInsert = null;
			}

			if (btnOKCustom != null) {
				btnOKCustom.Dispose ();
				btnOKCustom = null;
			}

			if (btnOrder != null) {
				btnOrder.Dispose ();
				btnOrder = null;
			}

			if (btnRemove != null) {
				btnRemove.Dispose ();
				btnRemove = null;
			}

			if (btnRepeatCombo != null) {
				btnRepeatCombo.Dispose ();
				btnRepeatCombo = null;
			}

			if (btnSecond != null) {
				btnSecond.Dispose ();
				btnSecond = null;
			}

			if (btnSubmit != null) {
				btnSubmit.Dispose ();
				btnSubmit = null;
			}

			if (btnThird != null) {
				btnThird.Dispose ();
				btnThird = null;
			}

			if (lblAvailable != null) {
				lblAvailable.Dispose ();
				lblAvailable = null;
			}

			if (lblBids != null) {
				lblBids.Dispose ();
				lblBids = null;
			}

			if (lblBuddy1 != null) {
				lblBuddy1.Dispose ();
				lblBuddy1 = null;
			}

			if (lblBuddy2 != null) {
				lblBuddy2.Dispose ();
				lblBuddy2 = null;
			}

			if (tblAvailableLine != null) {
				tblAvailableLine.Dispose ();
				tblAvailableLine = null;
			}

			if (tblSelectedLine != null) {
				tblSelectedLine.Dispose ();
				tblSelectedLine = null;
			}

			if (vwCustom != null) {
				vwCustom.Dispose ();
				vwCustom = null;
			}

			if (vwMain != null) {
				vwMain.Dispose ();
				vwMain = null;
			}
		}
	}

	[Register ("FAPositionChoiceWindow")]
	partial class FAPositionChoiceWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
