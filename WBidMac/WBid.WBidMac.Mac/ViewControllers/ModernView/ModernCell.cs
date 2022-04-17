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



[Register("ModernLineRowView")]
public partial class ModernLineRowView : AppKit.NSTableRowView
{
	public ModernLineRowView(CGRect frame)
	{
		this.Frame = frame;
	}

	// Called when created from unmanaged code
	public ModernLineRowView(IntPtr handle) : base(handle)
	{
		Initialize();
	}

	// Called when created directly from a XIB file
	[Export("initWithCoder:")]
	public ModernLineRowView(NSCoder coder) : base(coder)
	{
		Initialize();

	}

	// Shared initialization code
	void Initialize()
	{

	}

	public override void DrawSelection(CGRect dirtyRect)
	{
		base.DrawSelection(dirtyRect);
		if (this.SelectionHighlightStyle != NSTableViewSelectionHighlightStyle.None)
		{

			dirtyRect = new CGRect(0, 0, this.Frame.Width, this.Frame.Height);
			NSColor.FromRgba((nfloat)(153.0 / 255.0), (nfloat)(204.0 / 255.0), (nfloat)(230 / 255.0), (nfloat)1.0).SetFill();
			//NSColor.Green.SetFill ();
			var text6Path = NSBezierPath.FromRect(dirtyRect);
			text6Path.Fill();


		}
		else
		{
			//                dirtyRect = new CGRect (0, 0, this.Frame.Width, this.Frame.Height);
			//                //NSColor.Green.SetFill();
			//                NSColor.FromRgba((nfloat)(102/255),(nfloat)(178/255),(nfloat)(255/255),(nfloat)1.0).SetFill();
			//                var text6Path = NSBezierPath.FromRect(dirtyRect);
			//                text6Path.Fill ();

		}
		//base.DrawSelection (dirtyRect);
	}

	}



	public partial class ModernCell : AppKit.NSTableCellView
	{

		NSBezierPath BezierPath;
		public ModernCell ()
		{
		}

		// Called when created from unmanaged code
		public ModernCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ModernCell (NSCoder coder) : base (coder)
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
			this.NeedsDisplay = true;
		}

		private void DrawBorder( Line line)
{


			borderView.Hidden = true;
		      borderView.Frame = new CGRect(0, 0, 1195, 100);
            if(line.ManualScroll==4)
            {
                borderView.Hidden = false;
                borderView.Frame = new CGRect(0, 0, 1195, 100);
                borderView.Color = NSColor.Red;
            }
			if (!GlobalSettings.WBidINIContent.User.IsModernViewShade)
				return;
			
			if (line.ManualScroll == 1)

		{
				borderView.Hidden = false;  
				borderView.Frame = new CGRect(3, 0, 1187, 50);
                borderView.Color = NSColor.Blue;

		}
		else if (line.ManualScroll == 2)
		{
               borderView.Hidden = false;
				  borderView.Frame = new CGRect(0, 0, 1195, 100);
                borderView.Color = NSColor.Blue;
		}
		else if (line.ManualScroll == 3)
		{
				 borderView.Hidden = false;
				borderView.Frame = new CGRect(0, 73, 1197, 50);
                borderView.Color = NSColor.Blue;
		}
		else
		{
				
			

		}
	
}




		public void BindData (Line line, int index)
		{
			
			DrawBorder(line);
			
			lblLineBack.BackgroundColor = ColorClass.normDayColor;
			lblPropBack.BackgroundColor = ColorClass.normDayColor;

			lblOrder.StringValue = (index + 1).ToString ();
			lblLine.StringValue = line.LineDisplay;
			lblPosition.StringValue =  string.Join ("", line.FAPositions.ToArray ());
            lblOrder.TextColor = NSColor.Black;
            lblLine.TextColor = NSColor.Black;
            lblPosition.TextColor = NSColor.Black;
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

				if (lblName != null && lblName.Tag < CommonClass.modernProperties.Count)
					lblName.StringValue = CommonClass.modernProperties [(int)lblName.Tag];
				else if (lblName != null)
					lblName.StringValue = string.Empty;
				if (lblValue != null && lblValue.Tag < CommonClass.modernProperties.Count)
					lblValue.StringValue = (CommonClass.GetLineProperty (CommonClass.modernProperties [(int)lblValue.Tag], line) != null) ? CommonClass.GetLineProperty (CommonClass.modernProperties [(int)lblValue.Tag], line) : "0";
				else if (lblValue != null)
					lblValue.StringValue = string.Empty;

                if (lblValue != null)
                {
                    lblValue.TextColor = NSColor.Black;
                }
                if (lblName != null)
                {
                    lblName.TextColor = NSColor.Black;
                }
                //

			}
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

					if (lstVw.Any (x => x.Identifier == "split")) {
						var splt = (NSImageView)lstVw.FirstOrDefault (x => x.Identifier == "split");
						splt.RemoveFromSuperview ();
					}

					lblDate.StringValue = temp.Date.Day.ToString ();
					lblDay.StringValue = temp.Date.DayOfWeek.ToString ().Substring (0, 2).ToUpper ();

					if (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM || GlobalSettings.MenuBarButtonStatus.IsMIL) {
						if (temp.BidLineType != (int)BidLineType.NormalTrip && temp.BidLineType != (int)BidLineType.VOBSplit && temp.BidLineType != (int)BidLineType.VOBSplitDrop && temp.BidLineType != (int)BidLineType.VOFSplit && temp.BidLineType != (int)BidLineType.VOFSplitDrop) {
							lblTrip.StringValue = "";
							lblArrival.StringValue = (temp.ArrStaLastLegDisplay != null) ? temp.ArrStaLastLegDisplay : "";
							lblArrival.Frame = new CGRect (lblArrival.Frame.X, lblArrival.Frame.Y, lblArrival.Frame.Width, 27);
							lblTrip.ToolTip = (temp.ToolTip != null) ? temp.ToolTip : line.Pairingdesription;
							lblArrival.ToolTip = (temp.ToolTip != null) ? temp.ToolTip : line.Pairingdesription;
						} else {
							lblTrip.StringValue = (temp.TripNum != null) ? temp.TripNum : "";
							lblArrival.StringValue = (temp.ArrStaLastLeg != null) ? temp.ArrStaLastLeg : "";
							lblArrival.Frame = new CGRect (lblArrival.Frame.X, lblArrival.Frame.Y, lblArrival.Frame.Width, 18);
							lblTrip.ToolTip = (temp.ToolTip != null) ? temp.ToolTip : line.Pairingdesription;
							lblArrival.ToolTip = (temp.ToolTipBottom != null) ? temp.ToolTipBottom : line.Pairingdesription;
						}
					} else {
						lblTrip.StringValue = (temp.TripNum != null) ? temp.TripNum : "";
						lblArrival.StringValue = (temp.ArrStaLastLeg != null) ? temp.ArrStaLastLeg : "";
						lblArrival.Frame = new CGRect (lblArrival.Frame.X, lblArrival.Frame.Y, lblArrival.Frame.Width, 18);
						lblTrip.ToolTip = line.Pairingdesription;
						lblArrival.ToolTip = line.Pairingdesription;
					}

					lblDate.DrawsBackground = true;
					lblDay.DrawsBackground = true;
					lblTrip.DrawsBackground = true;
					lblArrival.DrawsBackground = true;

					if (lblDay.StringValue == "SA" || lblDay.StringValue == "SU") { // Weekend Days
						lblDate.BackgroundColor = ColorClass.weekendDayColor;
						lblDay.BackgroundColor = ColorClass.weekendDayColor;
						lblTrip.BackgroundColor = ColorClass.weekendTripColor;
						lblArrival.BackgroundColor = ColorClass.weekendTripColor;
					} else {	// Normal Days
						lblDate.BackgroundColor = ColorClass.normDayColor;
						lblDay.BackgroundColor = ColorClass.normDayColor;
						lblTrip.BackgroundColor = ColorClass.normTripColor;
						lblArrival.BackgroundColor = ColorClass.normTripColor;
					}

					if (!temp.IsInCurrentMonth) { // Not Current Month
						lblDay.BackgroundColor = ColorClass.nextMonthDayColor;
						lblTrip.BackgroundColor = ColorClass.nextMonthTripColor;
						lblArrival.BackgroundColor = ColorClass.nextMonthTripColor;
					}

					if (GlobalSettings.TempOrderedVacationDays != null && (GlobalSettings.MenuBarButtonStatus.IsVacationCorrection || GlobalSettings.MenuBarButtonStatus.IsEOM) && GlobalSettings.TempOrderedVacationDays.Any (x => x.AbsenceType == "VA" && (x.StartAbsenceDate <= temp.Date) && (x.EndAbsenceDate >= temp.Date))) {
						lblDate.BackgroundColor = ColorClass.VacationTripColor;
					} else if (lblDay.StringValue == "SA" || lblDay.StringValue == "SU") {
						lblDate.BackgroundColor = ColorClass.weekendDayColor;
					} else {
						lblDate.BackgroundColor = ColorClass.normDayColor;
					}

					if (!line.ReserveLine)
					{ // Not Reserve Line
						if (temp.AMPMType == "1")
						{ // AM
							lblTrip.BackgroundColor = ColorClass.AMTripColor;
							lblArrival.BackgroundColor = ColorClass.AMTripColor;
						}
						else if (temp.AMPMType == "2")
						{ //PM
							lblTrip.BackgroundColor = ColorClass.PMTripColor;
							lblArrival.BackgroundColor = ColorClass.PMTripColor;
						}
						else if (temp.AMPMType == "3")
						{ //Mix
							lblTrip.BackgroundColor = ColorClass.MixedTripColor;
							lblArrival.BackgroundColor = ColorClass.MixedTripColor;
						}
					}
					else
					{
                        if (line.LineDisplay.Contains("RR"))
                        {
							if (temp.AMPMType == "1" || (temp.AMPMType == "2") || (temp.AMPMType == "3"))
							{
								lblTrip.BackgroundColor = ColorClass.ReadyReserveTripColor;
								lblArrival.BackgroundColor = ColorClass.ReadyReserveTripColor;
							}
						}
                        else
                        {
                            // Reserve Line
                            if (temp.AMPMType == "1")
							{ // AM
								lblTrip.BackgroundColor = ColorClass.AMReserveTripColor;
								lblArrival.BackgroundColor = ColorClass.AMReserveTripColor;
							}
							else if (temp.AMPMType == "2")
							{ //PM
								lblTrip.BackgroundColor = ColorClass.PMReserveTripColor;
								lblArrival.BackgroundColor = ColorClass.PMReserveTripColor;
							}
							else if (temp.AMPMType == "3")
							{ //Mix
								lblTrip.BackgroundColor = ColorClass.MixedTripColor;
								lblArrival.BackgroundColor = ColorClass.MixedTripColor;
							}
						}
					}

					// prepareModernBidLineView.SetBidLineViewType(line, temp);
					if (temp.BidLineType == (int)BidLineType.CFV)
					{

						lblTrip.BackgroundColor = ColorClass.CFVVacationColor;
						lblArrival.BackgroundColor = ColorClass.CFVVacationColor;
					}
					if (temp.BidLineType == (int)BidLineType.FV)
                    {
                        
                        lblTrip.BackgroundColor = ColorClass.FVVacationColor;
                        lblArrival.BackgroundColor = ColorClass.FVVacationColor;
                    }
					if (temp.BidLineType == (int)BidLineType.VA) {
						lblTrip.BackgroundColor = ColorClass.VacationTripColor;
						lblArrival.BackgroundColor = ColorClass.VacationTripColor;
					} else if (temp.BidLineType == (int)BidLineType.VAP) {
						lblTrip.BackgroundColor = ColorClass.VAPColor;
						lblArrival.BackgroundColor = ColorClass.VAPColor;
					} else if (temp.BidLineType == (int)BidLineType.VO) {
						lblTrip.BackgroundColor = ColorClass.VacationOverlapTripColor;
						lblArrival.BackgroundColor = ColorClass.VacationOverlapTripColor;
					} else if (temp.BidLineType == (int)BidLineType.VD) {
						lblTrip.BackgroundColor = ColorClass.VacationDropTripColor;
						lblArrival.BackgroundColor = ColorClass.VacationDropTripColor;
					} else if (temp.BidLineType == (int)BidLineType.VDDrop) {
						lblTrip.BackgroundColor = NSColor.White;
                        lblArrival.BackgroundColor = NSColor.White;
					} else if (temp.BidLineType == (int)BidLineType.VOFSplit) {
//					drawSplitShapes(lblTripName[index].Frame, shapeOffset, 1, ColorClass.VacationDropTripColor, ColorClass.VacationOverlapTripColor);
						var imgV = new NSImageView (new CGRect (-2, 0, 33, 35));
						imgV.Identifier = "split";
						if (GlobalSettings.MenuBarButtonStatus.IsMIL)
							imgV.Image = NSImage.ImageNamed ("split1m.png");
						else
							imgV.Image = NSImage.ImageNamed ("split1.png");
						bxCal.AddSubview (imgV);
					} else if (temp.BidLineType == (int)BidLineType.VOFSplitDrop) {
//					drawSplitShapes(lblTripName[index].Frame, shapeOffset, 1, UIColor.White, ColorClass.VacationOverlapTripColor);
						var imgV = new NSImageView (new CGRect (-2, 0, 33, 35));
						imgV.Identifier = "split";
						imgV.Image = NSImage.ImageNamed ("split1d.png");
						bxCal.AddSubview (imgV);
					} else if (temp.BidLineType == (int)BidLineType.VOBSplit) {
//					drawSplitShapes(lblTripName[index].Frame, shapeOffset, 2, ColorClass.VacationOverlapTripColor, ColorClass.VacationDropTripColor);
						var imgV = new NSImageView (new CGRect (-2, 0, 33, 35));
						imgV.Identifier = "split";
						if (GlobalSettings.MenuBarButtonStatus.IsMIL)
							imgV.Image = NSImage.ImageNamed ("split2m.png");
						else
							imgV.Image = NSImage.ImageNamed ("split2.png");
						bxCal.AddSubview (imgV);
					} else if (temp.BidLineType == (int)BidLineType.VOBSplitDrop) {
//					drawSplitShapes(lblTripName[index].Frame, shapeOffset, 2, ColorClass.VacationOverlapTripColor, UIColor.White);
						var imgV = new NSImageView (new CGRect (-2, 0, 33, 35));
						imgV.Identifier = "split";
						imgV.Image = NSImage.ImageNamed ("split2d.png");
						bxCal.AddSubview (imgV);
					}

					if (GlobalSettings.MenuBarButtonStatus.IsMIL) {
						if (temp.BidLineType == (int)BidLineType.VA) {
							lblTrip.BackgroundColor = NSColor.Orange;
							lblArrival.BackgroundColor = NSColor.Orange;
						}
//						if (temp.BidLineType == (int)BidLineType.VOFSplit) {
//							var imgV = new NSImageView (new RectangleF (-2, 0, 33, 35));
//							imgV.Identifier = "split";
//							imgV.Image = NSImage.ImageNamed ("split1m.png");
//							bxCal.AddSubview (imgV);
//						}
//						if (temp.BidLineType == (int)BidLineType.VOBSplit) {
//							var imgV = new NSImageView (new RectangleF (-2, 0, 33, 35));
//							imgV.Identifier = "split";
//							imgV.Image = NSImage.ImageNamed ("split2m.png");
//							bxCal.AddSubview (imgV);
//						}

					}


					if (CommonClass.selectedTrip != null && CommonClass.selectedTrip == temp.TripName) {
						lblTrip.BackgroundColor = NSColor.Yellow;
						lblArrival.BackgroundColor = NSColor.Yellow;
					} 

				} else {
					bxCal.Hidden = true;
					bxCalData.Frame = new CGRect (bxCalData.Frame.Location, new CGSize (1000, bxCalData.Frame.Size.Height));
				}
			}
		}

		//		private void drawSplitShapes(RectangleF rect, int offset, int type, NSColor color1, NSColor color2)
		//		{
		//			if (type == 1)
		//			{
		//				//imgVLayer.Tag = 1;
		//
		//				PointF point1 = rect.Location;
		//				NSBezierPath path = new NSBezierPath();
		//				path.MoveTo(new PointF(offset + point1.X, point1.Y));
		//				path.LineTo(new PointF(offset + point1.X + 22, point1.Y));
		//				path.LineTo(new PointF(offset + point1.X, point1.Y + 40));
		//				path.LineTo(new PointF(offset + point1.X, point1.Y));
		//				path.ClosePath();
		//				CAShapeLayer shLayer = new CAShapeLayer();
		//				shLayer.Path = path;
		//				shLayer.FillColor = color1.CGColor;
		//				//imgVLayer.Layer.AddSublayer(shLayer);
		//
		//				NSBezierPath path1 = new NSBezierPath();
		//				path1.MoveTo(new PointF(offset + point1.X + 22, point1.Y));
		//				path1.LineTo(new PointF(offset + point1.X + 22, point1.Y + 40));
		//				path1.LineTo(new PointF(offset + point1.X, point1.Y + 40));
		//				path1.LineTo(new PointF(offset + point1.X + 22, point1.Y));
		//				path1.ClosePath();
		//				CAShapeLayer shLayer1 = new CAShapeLayer();
		//				shLayer1.Path = path1;
		//				shLayer1.FillColor = color2.CGColor;
		//				//imgVLayer.Layer.AddSublayer(shLayer1);
		//
		//			}
		//			else if (type == 2)
		//			{
		//				//imgVLayer.Tag = 1;
		//
		//				PointF point1 = rect.Location;
		//				NSBezierPath path = new NSBezierPath();
		//				path.MoveTo(new PointF(offset + point1.X, point1.Y));
		//				path.LineTo(new PointF(offset + point1.X, point1.Y + 40));
		//				path.LineTo(new PointF(offset + point1.X + 22, point1.Y + 40));
		//				path.LineTo(new PointF(offset + point1.X, point1.Y));
		//				path.ClosePath();
		//				CAShapeLayer shLayer = new CAShapeLayer();
		//				shLayer.Path = path;
		//				shLayer.FillColor = color1.CGColor;
		//				//imgVLayer.Layer.AddSublayer(shLayer);
		//
		//				NSBezierPath path1 = new NSBezierPath();
		//				path1.MoveTo(new PointF(offset + point1.X, point1.Y));
		//				path1.LineTo(new PointF(offset + point1.X + 22, point1.Y));
		//				path1.LineTo(new PointF(offset + point1.X + 22, point1.Y + 40));
		//				path1.LineTo(new PointF(offset + point1.X, point1.Y));
		//				path1.ClosePath();
		//				CAShapeLayer shLayer1 = new CAShapeLayer();
		//				shLayer1.Path = path1;
		//				shLayer1.FillColor = color2.CGColor;
		//				//imgVLayer.Layer.AddSublayer(shLayer1);
		//			}
		//			//imgVLayer.Layer.ShouldRasterize = true;
		//			//imgVLayer.Layer.DrawsAsynchronously = true;
		//
		//		}

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

