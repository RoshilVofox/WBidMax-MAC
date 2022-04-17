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
	[Register ("ConfigurationWindowController")]
	partial class ConfigurationWindowController
	{
		[Outlet]
		AppKit.NSMatrix btnAMPMLineOptions { get; set; }

		[Outlet]
		AppKit.NSMatrix btnAMPMOptions { get; set; }

		[Outlet]
		AppKit.NSButton btnAMPMReset { get; set; }

		[Outlet]
		AppKit.NSButton btnApply { get; set; }

		[Outlet]
		AppKit.NSMatrix btnCalPlan { get; set; }

		[Outlet]
		AppKit.NSMatrix btnCalTime { get; set; }

		[Outlet]
		AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		AppKit.NSMatrix btnCSWPos { get; set; }

		[Outlet]
		AppKit.NSMatrix btnData { get; set; }

		[Outlet]
		AppKit.NSMatrix btnHotelOptions { get; set; }

		[Outlet]
		AppKit.NSButton btnModernManuaMoveBorder { get; set; }

		[Outlet]
		AppKit.NSButton btnOK { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnSaveTime { get; set; }

		[Outlet]
		AppKit.NSPopUpButton btnStartDay { get; set; }

		[Outlet]
		AppKit.NSMatrix btnWeekOptions { get; set; }

		[Outlet]
		AppKit.NSButton btnWeekReset { get; set; }

		[Outlet]
		AppKit.NSButton chSummaryShade { get; set; }

		[Outlet]
		AppKit.NSButton swAutoSave { get; set; }

		[Outlet]
		AppKit.NSButton swBidReceipt { get; set; }

		[Outlet]
		AppKit.NSButton swCheckDataUpdates { get; set; }

		[Outlet]
		AppKit.NSButton swGatherTrips { get; set; }

		[Outlet]
		AppKit.NSButton swGettingMissingTrips { get; set; }

		[Outlet]
		AppKit.NSButton swMILDisplay { get; set; }

		[Outlet]
		AppKit.NSButton swPairingOnSubject { get; set; }

		[Outlet]
		AppKit.NSButton swSendCrash { get; set; }

		[Outlet]
		AppKit.NSButton swShowCoverLetter { get; set; }

		[Outlet]
		AppKit.NSButton swSmartSynch { get; set; }

		[Outlet]
		AppKit.NSDatePicker txtAMAfter { get; set; }

		[Outlet]
		AppKit.NSDatePicker txtAMBefore { get; set; }

		[Outlet]
		AppKit.NSTextField txtAMPMMaxNum { get; set; }

		[Outlet]
		AppKit.NSTextField txtAMPMMaxPercent { get; set; }

		[Outlet]
		AppKit.NSDatePicker txtNTEAfter { get; set; }

		[Outlet]
		AppKit.NSDatePicker txtNTEBefore { get; set; }

		[Outlet]
		AppKit.NSDatePicker txtPMAfter { get; set; }

		[Outlet]
		AppKit.NSDatePicker txtPMBefore { get; set; }

		[Outlet]
		AppKit.NSTextField txtWeekMaxNum { get; set; }

		[Outlet]
		AppKit.NSTextField txtWeekMaxPercent { get; set; }

		[Outlet]
		AppKit.NSView vwData { get; set; }

		[Outlet]
		AppKit.NSTabView vwTab { get; set; }

		[Action ("funChangeBlueBorderFeature:")]
		partial void funChangeBlueBorderFeature (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnModernManuaMoveBorder != null) {
				btnModernManuaMoveBorder.Dispose ();
				btnModernManuaMoveBorder = null;
			}

			if (btnAMPMLineOptions != null) {
				btnAMPMLineOptions.Dispose ();
				btnAMPMLineOptions = null;
			}

			if (btnAMPMOptions != null) {
				btnAMPMOptions.Dispose ();
				btnAMPMOptions = null;
			}

			if (btnAMPMReset != null) {
				btnAMPMReset.Dispose ();
				btnAMPMReset = null;
			}

			if (btnApply != null) {
				btnApply.Dispose ();
				btnApply = null;
			}

			if (btnCalPlan != null) {
				btnCalPlan.Dispose ();
				btnCalPlan = null;
			}

			if (btnCalTime != null) {
				btnCalTime.Dispose ();
				btnCalTime = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnCSWPos != null) {
				btnCSWPos.Dispose ();
				btnCSWPos = null;
			}

			if (btnData != null) {
				btnData.Dispose ();
				btnData = null;
			}

			if (btnHotelOptions != null) {
				btnHotelOptions.Dispose ();
				btnHotelOptions = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (btnSaveTime != null) {
				btnSaveTime.Dispose ();
				btnSaveTime = null;
			}

			if (btnStartDay != null) {
				btnStartDay.Dispose ();
				btnStartDay = null;
			}

			if (btnWeekOptions != null) {
				btnWeekOptions.Dispose ();
				btnWeekOptions = null;
			}

			if (btnWeekReset != null) {
				btnWeekReset.Dispose ();
				btnWeekReset = null;
			}

			if (chSummaryShade != null) {
				chSummaryShade.Dispose ();
				chSummaryShade = null;
			}

			if (swAutoSave != null) {
				swAutoSave.Dispose ();
				swAutoSave = null;
			}

			if (swBidReceipt != null) {
				swBidReceipt.Dispose ();
				swBidReceipt = null;
			}

			if (swCheckDataUpdates != null) {
				swCheckDataUpdates.Dispose ();
				swCheckDataUpdates = null;
			}

			if (swGatherTrips != null) {
				swGatherTrips.Dispose ();
				swGatherTrips = null;
			}

			if (swGettingMissingTrips != null) {
				swGettingMissingTrips.Dispose ();
				swGettingMissingTrips = null;
			}

			if (swMILDisplay != null) {
				swMILDisplay.Dispose ();
				swMILDisplay = null;
			}

			if (swPairingOnSubject != null) {
				swPairingOnSubject.Dispose ();
				swPairingOnSubject = null;
			}

			if (swSendCrash != null) {
				swSendCrash.Dispose ();
				swSendCrash = null;
			}

			if (swShowCoverLetter != null) {
				swShowCoverLetter.Dispose ();
				swShowCoverLetter = null;
			}

			if (swSmartSynch != null) {
				swSmartSynch.Dispose ();
				swSmartSynch = null;
			}

			if (txtAMAfter != null) {
				txtAMAfter.Dispose ();
				txtAMAfter = null;
			}

			if (txtAMBefore != null) {
				txtAMBefore.Dispose ();
				txtAMBefore = null;
			}

			if (txtAMPMMaxNum != null) {
				txtAMPMMaxNum.Dispose ();
				txtAMPMMaxNum = null;
			}

			if (txtAMPMMaxPercent != null) {
				txtAMPMMaxPercent.Dispose ();
				txtAMPMMaxPercent = null;
			}

			if (txtNTEAfter != null) {
				txtNTEAfter.Dispose ();
				txtNTEAfter = null;
			}

			if (txtNTEBefore != null) {
				txtNTEBefore.Dispose ();
				txtNTEBefore = null;
			}

			if (txtPMAfter != null) {
				txtPMAfter.Dispose ();
				txtPMAfter = null;
			}

			if (txtPMBefore != null) {
				txtPMBefore.Dispose ();
				txtPMBefore = null;
			}

			if (txtWeekMaxNum != null) {
				txtWeekMaxNum.Dispose ();
				txtWeekMaxNum = null;
			}

			if (txtWeekMaxPercent != null) {
				txtWeekMaxPercent.Dispose ();
				txtWeekMaxPercent = null;
			}

			if (vwData != null) {
				vwData.Dispose ();
				vwData = null;
			}

			if (vwTab != null) {
				vwTab.Dispose ();
				vwTab = null;
			}
		}
	}

	[Register ("ConfigurationWindow")]
	partial class ConfigurationWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
