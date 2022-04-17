using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.SharedLibrary;

//using System.Linq;
using WBid.WBidiPad.Core;

//using System.Collections.Generic;
using CoreAnimation;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;
using WBid.WBidiPad.Core.Enum;
using CoreGraphics;

namespace WBid.WBidMac.Mac
{


	[Register ("BidLineRowView")]
	public partial class BidLineRowView : AppKit.NSTableRowView
	{
		public BidLineRowView (CGRect frame)
		{
			this.Frame = frame;
		}

		// Called when created from unmanaged code
		public BidLineRowView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidLineRowView (NSCoder coder) : base (coder)
		{
			Initialize ();

		}

		// Shared initialization code
		void Initialize ()
		{

		}

		public override void DrawSelection (CGRect dirtyRect)
		{
			base.DrawSelection (dirtyRect);
			if (this.SelectionHighlightStyle != NSTableViewSelectionHighlightStyle.None) {

				dirtyRect = new CGRect (0, 0, this.Frame.Width, this.Frame.Height);
				NSColor.FromRgba((nfloat)(153.0/255.0),(nfloat)(204.0/255.0),(nfloat)(230/255.0),(nfloat)1.0).SetFill();
				//NSColor.Green.SetFill ();
				var text6Path = NSBezierPath.FromRect(dirtyRect);
				text6Path.Fill ();
			} else {
                //dirtyRect = new CGRect(0, 0, this.Frame.Width, this.Frame.Height);
                ////NSColor.Green.SetFill();
                //NSColor.FromRgba((nfloat)(102 / 255), (nfloat)(178 / 255), (nfloat)(255 / 255), (nfloat)1.0).SetFill();
                //var text6Path = NSBezierPath.FromRect(dirtyRect);
                //text6Path.Fill();

            }
			//base.DrawSelection (dirtyRect);
		}


	}



	public partial class BidLineCell : AppKit.NSTableCellView
	{
		public BidLineCell ()
		{
		}

		// Called when created from unmanaged code
		public BidLineCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BidLineCell (NSCoder coder) : base (coder)
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
		}

		public void BindData (Line line, int index)
		{
            var interfaceStyle = NSUserDefaults.StandardUserDefaults.StringForKey("AppleInterfaceStyle");

            lblOrder.StringValue = (index + 1).ToString ();
			lblLine.StringValue = line.LineDisplay;
			lblPosition.StringValue =  string.Join ("", line.FAPositions.ToArray ());

            if (interfaceStyle == "Dark" && line.BlankLine)
            {
                lblOrder.TextColor = NSColor.Black;
                lblLine.TextColor = NSColor.Black;
                lblPosition.TextColor = NSColor.Black;
                lblPairingDesc.TextColor = NSColor.Black;
            }

            if (line.BlankLine) {
				lblBack.DrawsBackground = true;
				lblBack.BackgroundColor = ColorClass.BlankLineColor;

			} else if (line.ReserveLine) {
				lblBack.DrawsBackground = true;
				lblBack.BackgroundColor = ColorClass.ReserveLineColor;
			} else {
				lblBack.DrawsBackground = false;
				lblBack.BackgroundColor = NSColor.ControlBackground;
			}

			if (line.TopLock)
				imgLock.Image = NSImage.ImageNamed ("lockIconGreen.png");
			else if (line.BotLock)
				imgLock.Image = NSImage.ImageNamed ("lockIconRed.png");
			else
				imgLock.Image = null;

			if (line.Constrained)
				imgConstraint.Image = NSImage.ImageNamed ("constraintIcon.png");
			else
				imgConstraint.Image = null;

			if (line.ShowOverLap)
				imgOverlap.Image = NSImage.ImageNamed ("overlapIcon.png");
			else
				imgOverlap.Image = null;

			foreach (var vw in ((NSView)bxProperty.ContentView).Subviews) {
				var lblName = (vw.Identifier == "lbPropName") ? (NSTextField)vw : null;
				var lblValue = (vw.Identifier == "lbPropValue") ? (NSTextField)vw : null;
                //Color change for Darkmode Blank line
                if (interfaceStyle == "Dark" && line.BlankLine)
                {
                    if (lblName != null && lblName.Tag < CommonClass.bidLineProperties.Count) {
                        lblName.TextColor = NSColor.Black;
                    }
                    if (lblValue != null && lblValue.Tag < CommonClass.bidLineProperties.Count) {
                        lblValue.TextColor = NSColor.Black;
                    }

                }

                if (lblName != null && lblName.Tag < CommonClass.bidLineProperties.Count)
					lblName.StringValue = CommonClass.bidLineProperties [(int)lblName.Tag];
				else if (lblName != null)
					lblName.StringValue = string.Empty;
				if (lblValue != null && lblValue.Tag < CommonClass.bidLineProperties.Count)
					lblValue.StringValue = (CommonClass.GetLineProperty (CommonClass.bidLineProperties [(int)lblValue.Tag], line) != null) ? CommonClass.GetLineProperty (CommonClass.bidLineProperties [(int)lblValue.Tag], line) : "0";
				else if (lblValue != null)
					lblValue.StringValue = string.Empty;


               
            }
            

			lblPairingDesc.StringValue = line.Pairingdesription;

			foreach (var vw in ((NSView)bxCalData.ContentView).Subviews) {
				var bxCal = (NSBox)vw;
				var idx = int.Parse (bxCal.Identifier.Replace ("bx", ""));
				if (idx < line.BidLineTemplates.Count) {
					var temp = line.BidLineTemplates [idx];
					var lstVw = ((NSView)bxCal.ContentView).Subviews.ToList ();
					var lblDate = (NSTextField)lstVw.FirstOrDefault (x => x.Identifier == "lbDate");
					var lblDay = (NSTextField)lstVw.FirstOrDefault (x => x.Identifier == "lbDay");
					var lblTrip = (NSTextField)lstVw.FirstOrDefault (x => x.Identifier == "lbTrip");
					var lblArrival = (NSTextField)lstVw.FirstOrDefault (x => x.Identifier == "lbArrival");

					lblDate.StringValue = temp.Date.Day.ToString ();
					lblDay.StringValue = temp.Date.DayOfWeek.ToString ().Substring (0, 2).ToUpper ();
					lblTrip.StringValue = (temp.TripNum != null) ? temp.TripNum : "";
					lblArrival.StringValue = (temp.ArrStaLastLeg != null) ? temp.ArrStaLastLeg : "";

					if (lblDay.StringValue == "SA" || lblDay.StringValue == "SU") { // Weekend Days
						lblDate.TextColor = NSColor.Red;
						lblDay.TextColor = NSColor.Red;
						lblTrip.TextColor = NSColor.Red;
						lblArrival.TextColor = NSColor.Red;
					} else {

                       

                        if (interfaceStyle == "Dark") {
                            if (line.BlankLine)
                            {
                                lblDate.TextColor = NSColor.Black;
                                lblDay.TextColor = NSColor.Black;
                                lblTrip.TextColor = NSColor.Black;
                                lblArrival.TextColor = NSColor.Black;
                            }
                            else {
                                lblDate.TextColor = NSColor.White;
                                lblDay.TextColor = NSColor.White;
                                lblTrip.TextColor = NSColor.White;
                                lblArrival.TextColor = NSColor.White;
                            }
                        }
                        else {
                            lblDate.TextColor = NSColor.Black;
                            lblDay.TextColor = NSColor.Black;
                            lblTrip.TextColor = NSColor.Black;
                            lblArrival.TextColor = NSColor.Black;
                        }
                         
					}

					if (CommonClass.selectedTrip != null && CommonClass.selectedTrip == temp.TripName) {
						lblTrip.DrawsBackground = true;
						lblArrival.DrawsBackground = true;
						lblTrip.BackgroundColor = NSColor.Yellow;
						lblArrival.BackgroundColor = NSColor.Yellow;
                        if (interfaceStyle == "Dark") {
                            if (lblDay.StringValue == "SA" || lblDay.StringValue == "SU") {

                                lblDate.TextColor = NSColor.Red;
                                lblDay.TextColor = NSColor.Red;
                                lblTrip.TextColor = NSColor.Red;
                                lblArrival.TextColor = NSColor.Red;
                            }
                            else
                            {

                                lblDate.TextColor = NSColor.Black;
                                lblDay.TextColor = NSColor.Black;
                                lblTrip.TextColor = NSColor.Black;
                                lblArrival.TextColor = NSColor.Black;

                            }

                        }

                    }
                    else {
						lblTrip.DrawsBackground = false;
						lblArrival.DrawsBackground = false;
						lblTrip.BackgroundColor = NSColor.White;
						lblArrival.BackgroundColor = NSColor.White;
					}

//					if (lstVw.Any (x => x.Identifier == "TripBtn")) {
//						var btn = (NSButton)lstVw.FirstOrDefault (x => x.Identifier == "TripBtn");
//						btn.RemoveFromSuperview ();
//					}
//					
//					if (!string.IsNullOrEmpty (temp.TripName)) {
//						var btn = new NSButton (new RectangleF (-2, 10, 34, 35));
//						btn.Identifier = "TripBtn";
//						btn.Title = "";
//	//					btn.Transparent = true;
//	//					btn.Bordered = false;
//						btn.IgnoresMultiClick = true;
//						bxCal.AddSubview (btn);
//						btn.Activated += delegate {
//							CommonClass.BidLineController.ShowTripWindow (temp.TripName, index);
//						};
//
//					}

				} else {
					bxCal.Hidden = true;
					bxCalData.Frame = new CGRect (bxCalData.Frame.Location, new CGSize (1000, bxCalData.Frame.Size.Height));
				}
			}
		}

		//		private string GetLineProperty (string displayName, Line line)
		//		{
		//			if (displayName == "$/Day") {
		//				return line.TfpPerDay.ToString ();
		//			} else if (displayName == "$/DHr") {
		//				return line.TfpPerDhr.ToString ();
		//			} else if (displayName == "$/Hr") {
		//				return line.TfpPerFltHr.ToString ();
		//			} else if (displayName == "$/TAFB") {
		//				return line.TfpPerTafb.ToString ();
		//			} else if (displayName == "+Grd") {
		//				return line.LongestGrndTime.ToString ();
		//			} else if (displayName == "+Legs") {
		//				return line.MostLegs.ToString ();
		//			} else if (displayName == "+Off") {
		//				return line.LargestBlkOfDaysOff.ToString ();
		//			} else if (displayName == "1Dy") {
		//				return line.Trips1Day.ToString ();
		//			} else if (displayName == "2Dy") {
		//				return line.Trips2Day.ToString ();
		//			} else if (displayName == "3Dy") {
		//				return line.Trips3Day.ToString ();
		//			} else if (displayName == "4Dy") {
		//				return line.Trips4Day.ToString ();
		//			} else if (displayName == "8753") {
		//				return line.Equip8753.ToString ();
		//			} else if (displayName == "A/P") {
		//				return line.AMPM.ToString ();
		//			} else if (displayName == "ACChg") {
		//				return line.AcftChanges.ToString ();
		//			} else if (displayName == "ACDay") {
		//				return line.AcftChgDay.ToString ();
		//			} else if (displayName == "CO") {
		//				return line.CarryOverTfp.ToString ();
		//			} else if (displayName == "DP") {
		//				return line.TotDutyPds.ToString ();
		//			} else if (displayName == "DPinBP") {
		//				return line.TotDutyPdsInBp.ToString ();
		//			} else if (displayName == "EDomPush") {
		//				return line.EDomPush;
		//			} else if (displayName == "EPush") {
		//				return line.EPush;
		//			} else if (displayName == "FA Posn") {
		//				return string.Join ("", line.FAPositions.ToArray ());
		//			} else if (displayName == "Flt") {
		//				return line.BlkHrsInBp;
		//			} else if (displayName == "LArr") {
		//				return line.LastArrTime.ToString ();
		//			} else if (displayName == "LDomArr") {
		//				return line.LastDomArrTime.ToString ();
		//			} else if (displayName == "Legs") {
		//				return line.Legs.ToString ();
		//			} else if (displayName == "LgDay") {
		//				return line.LegsPerDay.ToString ();
		//			} else if (displayName == "LgPair") {
		//				return line.LegsPerPair.ToString ();
		//			} else if (displayName == "ODrop") {
		//				return line.OverlapDrop.ToString ();
		//			} else if (displayName == "Off") {
		//				return line.DaysOff.ToString ();
		//			} else if (displayName == "Pairs") {
		//				return line.TotPairings.ToString ();
		//			} else if (displayName == "Pay" || displayName == "TotPay") {
		//				return string.Format ("{0:0.00}",Decimal.Round (line.Tfp, 2));
		//			} else if (displayName == "PDiem") {
		//				return line.TafbInBp;
		//			} else if (displayName == "MyValue") {
		//				return string.Format ("{0:0.00}",Decimal.Round (line.Points, 2));
		//
		//			} else if (displayName == "SIPs") {
		//				return line.Sips.ToString ();
		//			} else if (displayName == "StartDOW") {
		//				return line.StartDow;
		//			} else if (displayName == "T234") {
		//				return line.T234;
		//			} else if (displayName == "VDrop") {
		//				return line.VacationDrop.ToString ();
		//			} else if (displayName == "WkEnd") {
		//				if (line.Weekend != null)
		//					return line.Weekend.ToLower ();
		//				else
		//					return "";
		//			} else if (displayName == "FltRig") {
		//				return line.RigFltInBP.ToString ();
		//			} else if (displayName == "MinPayRig") {
		//				return line.RigDailyMinInBp.ToString ();
		//			} else if (displayName == "DhrRig") {
		//				return line.RigDhrInBp.ToString ();
		//			} else if (displayName == "AdgRig") {
		//				return line.RigAdgInBp.ToString ();
		//			} else if (displayName == "TafbRig") {
		//				return line.RigTafbInBp.ToString ();
		//			} else if (displayName == "TotRig") {
		//				return line.RigTotalInBp.ToString ();
		//			} else if (displayName == "VacPay") {
		//				return Decimal.Round (line.VacPay, 2).ToString ();
		//			} else if (displayName == "Vofrnt") {
		//				return Decimal.Round (line.VacationOverlapFront, 2).ToString ();
		//			} else if (displayName == "Vobk") {
		//				return Decimal.Round (line.VacationOverlapBack, 2).ToString ();
		//			} else if (displayName == "800legs") {
		//				return line.LegsIn800.ToString ();
		//			} else if (displayName == "700legs") {
		//				return line.LegsIn700.ToString ();
		//			} else if (displayName == "500legs") {
		//				return line.LegsIn500.ToString ();
		//			} else if (displayName == "300legs") {
		//				return line.LegsIn300.ToString ();
		//			} else if (displayName == "DhrInBp") {
		//				return line.DutyHrsInBp;
		//			} else if (displayName == "DhrInLine") {
		//				return line.DutyHrsInLine;
		//			} else if (displayName == "Wts") {
		//				return Decimal.Round (line.TotWeight, 2).ToString ();
		//			} else if (displayName == "LineRig") {
		//				return Decimal.Round (line.LineRig, 2).ToString ();
		//			} else if (displayName == "FlyPay") {
		//				return Decimal.Round (line.FlyPay, 2).ToString ();
		//			} else {
		//				return "";
		//			}
		//		}

	}
}

