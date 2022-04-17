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
	[Register ("ConstraintsCell")]
	partial class ConstraintsCell
	{
		[Outlet]
		AppKit.NSButton btnBothEnds { get; set; }

		[Outlet]
		AppKit.NSButton btnCalculate { get; set; }

		[Outlet]
		AppKit.NSButton btnClose1 { get; set; }

		[Outlet]
		AppKit.NSButton btnClose2 { get; set; }

		[Outlet]
		AppKit.NSButton btnClose3 { get; set; }

		[Outlet]
		AppKit.NSButton btnClose4 { get; set; }

		[Outlet]
		AppKit.NSButton btnClose5 { get; set; }

		[Outlet]
		AppKit.NSButton btnCloseBLK { get; set; }

		[Outlet]
		AppKit.NSButton btnCloseCL { get; set; }

		[Outlet]
		AppKit.NSButton btnCloseCmtblty { get; set; }

		[Outlet]
		AppKit.NSButton btnCloseDOM { get; set; }

		[Outlet]
		AppKit.NSButton btnCloseReportRelease { get; set; }

		[Outlet]
		AppKit.NSButton btnCommuteHome { get; set; }

		[Outlet]
		AppKit.NSButton btnCommuteWork { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnComp2 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnComp3 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnComp4 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnComp5 { get; set; }

		[Outlet]
		AppKit.NSButton btnFirstCheck { get; set; }

		[Outlet]
		AppKit.NSButton btnHelp1 { get; set; }

		[Outlet]
		AppKit.NSButton btnHelp2 { get; set; }

		[Outlet]
		AppKit.NSButton btnHelp3 { get; set; }

		[Outlet]
		AppKit.NSButton btnHelp4 { get; set; }

		[Outlet]
		AppKit.NSButton btnHelp5 { get; set; }

		[Outlet]
		AppKit.NSButton btnHelpBLK { get; set; }

		[Outlet]
		AppKit.NSButton btnHelpCL { get; set; }

		[Outlet]
		AppKit.NSButton btnHelpCmtblty { get; set; }

		[Outlet]
		AppKit.NSButton btnHelpDOM { get; set; }

		[Outlet]
		AppKit.NSButton btnHelpreportRelease { get; set; }

		[Outlet]
		AppKit.NSButton btnInDom { get; set; }

		[Outlet]
		AppKit.NSButton btnLastCheck { get; set; }

		[Outlet]
		AppKit.NSButton btnLoadDefaults { get; set; }

		[Outlet]
		AppKit.NSButton btnNoMidCheck { get; set; }

		[Outlet]
		AppKit.NSButton btnSaveDefaults { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnSecond3 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnSecond4 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnSecond5 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnThird4 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnThird5 { get; set; }

		[Outlet]
		AppKit.NSMatrix btnType { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnValue1 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnValue2 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnValue3 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnValue4 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnValue5 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cmtbltybtnValue1 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cmtbltybtnValue2 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cmtbltybtnValue3 { get; set; }

		[Outlet]
		AppKit.NSPopUpButton cmtbltybtnValue4 { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpFriCheckIn { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpFriToBase { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpMonThuCheckIn { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpMonThuToBase { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpRelease { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpReport { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpSatCheckIn { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpSatToBase { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpSunCheckIn { get; set; }

		[Outlet]
		AppKit.NSDatePicker dpSunToBase { get; set; }

		[Outlet]
		AppKit.NSTextField lblDOM { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle1 { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle2 { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle3 { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle4 { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitle5 { get; set; }

		[Outlet]
		AppKit.NSTextField lblTitleCmtblty { get; set; }

		[Outlet]
		AppKit.NSTextField lblTripWorkBlockBorder { get; set; }

		[Outlet]
		AppKit.NSCollectionView vwBLK { get; set; }

		[Outlet]
		AppKit.NSCollectionView vwDOM { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnBothEnds != null) {
				btnBothEnds.Dispose ();
				btnBothEnds = null;
			}

			if (btnCalculate != null) {
				btnCalculate.Dispose ();
				btnCalculate = null;
			}

			if (btnClose1 != null) {
				btnClose1.Dispose ();
				btnClose1 = null;
			}

			if (btnClose2 != null) {
				btnClose2.Dispose ();
				btnClose2 = null;
			}

			if (btnClose3 != null) {
				btnClose3.Dispose ();
				btnClose3 = null;
			}

			if (btnClose4 != null) {
				btnClose4.Dispose ();
				btnClose4 = null;
			}

			if (btnClose5 != null) {
				btnClose5.Dispose ();
				btnClose5 = null;
			}

			if (btnCloseBLK != null) {
				btnCloseBLK.Dispose ();
				btnCloseBLK = null;
			}

			if (btnCloseCL != null) {
				btnCloseCL.Dispose ();
				btnCloseCL = null;
			}

			if (btnCloseCmtblty != null) {
				btnCloseCmtblty.Dispose ();
				btnCloseCmtblty = null;
			}

			if (btnCloseDOM != null) {
				btnCloseDOM.Dispose ();
				btnCloseDOM = null;
			}

			if (btnCloseReportRelease != null) {
				btnCloseReportRelease.Dispose ();
				btnCloseReportRelease = null;
			}

			if (btnCommuteHome != null) {
				btnCommuteHome.Dispose ();
				btnCommuteHome = null;
			}

			if (btnCommuteWork != null) {
				btnCommuteWork.Dispose ();
				btnCommuteWork = null;
			}

			if (btnComp2 != null) {
				btnComp2.Dispose ();
				btnComp2 = null;
			}

			if (btnComp3 != null) {
				btnComp3.Dispose ();
				btnComp3 = null;
			}

			if (btnComp4 != null) {
				btnComp4.Dispose ();
				btnComp4 = null;
			}

			if (btnComp5 != null) {
				btnComp5.Dispose ();
				btnComp5 = null;
			}

			if (btnHelp1 != null) {
				btnHelp1.Dispose ();
				btnHelp1 = null;
			}

			if (btnHelp2 != null) {
				btnHelp2.Dispose ();
				btnHelp2 = null;
			}

			if (btnHelp3 != null) {
				btnHelp3.Dispose ();
				btnHelp3 = null;
			}

			if (btnHelp4 != null) {
				btnHelp4.Dispose ();
				btnHelp4 = null;
			}

			if (btnHelp5 != null) {
				btnHelp5.Dispose ();
				btnHelp5 = null;
			}

			if (btnHelpBLK != null) {
				btnHelpBLK.Dispose ();
				btnHelpBLK = null;
			}

			if (btnHelpCL != null) {
				btnHelpCL.Dispose ();
				btnHelpCL = null;
			}

			if (btnHelpCmtblty != null) {
				btnHelpCmtblty.Dispose ();
				btnHelpCmtblty = null;
			}

			if (btnHelpDOM != null) {
				btnHelpDOM.Dispose ();
				btnHelpDOM = null;
			}

			if (btnHelpreportRelease != null) {
				btnHelpreportRelease.Dispose ();
				btnHelpreportRelease = null;
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

			if (btnSecond3 != null) {
				btnSecond3.Dispose ();
				btnSecond3 = null;
			}

			if (btnSecond4 != null) {
				btnSecond4.Dispose ();
				btnSecond4 = null;
			}

			if (btnSecond5 != null) {
				btnSecond5.Dispose ();
				btnSecond5 = null;
			}

			if (btnThird4 != null) {
				btnThird4.Dispose ();
				btnThird4 = null;
			}

			if (btnThird5 != null) {
				btnThird5.Dispose ();
				btnThird5 = null;
			}

			if (btnValue1 != null) {
				btnValue1.Dispose ();
				btnValue1 = null;
			}

			if (btnValue2 != null) {
				btnValue2.Dispose ();
				btnValue2 = null;
			}

			if (btnValue3 != null) {
				btnValue3.Dispose ();
				btnValue3 = null;
			}

			if (btnValue4 != null) {
				btnValue4.Dispose ();
				btnValue4 = null;
			}

			if (btnValue5 != null) {
				btnValue5.Dispose ();
				btnValue5 = null;
			}

			if (cmtbltybtnValue1 != null) {
				cmtbltybtnValue1.Dispose ();
				cmtbltybtnValue1 = null;
			}

			if (cmtbltybtnValue2 != null) {
				cmtbltybtnValue2.Dispose ();
				cmtbltybtnValue2 = null;
			}

			if (cmtbltybtnValue3 != null) {
				cmtbltybtnValue3.Dispose ();
				cmtbltybtnValue3 = null;
			}

			if (cmtbltybtnValue4 != null) {
				cmtbltybtnValue4.Dispose ();
				cmtbltybtnValue4 = null;
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

			if (dpReport != null) {
				dpReport.Dispose ();
				dpReport = null;
			}

			if (dpRelease != null) {
				dpRelease.Dispose ();
				dpRelease = null;
			}

			if (lblDOM != null) {
				lblDOM.Dispose ();
				lblDOM = null;
			}

			if (lblTitle1 != null) {
				lblTitle1.Dispose ();
				lblTitle1 = null;
			}

			if (lblTitle2 != null) {
				lblTitle2.Dispose ();
				lblTitle2 = null;
			}

			if (lblTitle3 != null) {
				lblTitle3.Dispose ();
				lblTitle3 = null;
			}

			if (lblTitle4 != null) {
				lblTitle4.Dispose ();
				lblTitle4 = null;
			}

			if (lblTitle5 != null) {
				lblTitle5.Dispose ();
				lblTitle5 = null;
			}

			if (lblTitleCmtblty != null) {
				lblTitleCmtblty.Dispose ();
				lblTitleCmtblty = null;
			}

			if (lblTripWorkBlockBorder != null) {
				lblTripWorkBlockBorder.Dispose ();
				lblTripWorkBlockBorder = null;
			}

			if (vwBLK != null) {
				vwBLK.Dispose ();
				vwBLK = null;
			}

			if (vwDOM != null) {
				vwDOM.Dispose ();
				vwDOM = null;
			}

			if (btnFirstCheck != null) {
				btnFirstCheck.Dispose ();
				btnFirstCheck = null;
			}

			if (btnLastCheck != null) {
				btnLastCheck.Dispose ();
				btnLastCheck = null;
			}

			if (btnNoMidCheck != null) {
				btnNoMidCheck.Dispose ();
				btnNoMidCheck = null;
			}

			if (btnType != null) {
				btnType.Dispose ();
				btnType = null;
			}
		}
	}
}
