
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.Model;

//using System.Collections.Generic;
using System.Collections.ObjectModel;

//using System.Linq;
using WBid.WBidiPad.PortableLibrary;
using VacationCorrection;
using System.IO;
using System.Net;

//using MiniZip.ZipArchive;
using WBid.WBidiPad.SharedLibrary.Parser;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

namespace WBid.WBidMac.Mac
{
	public partial class TestVacationViewController : AppKit.NSViewController
	{
		#region Constructors

		NSPopover pop;
		NSDatePicker curPopDate;

		// Called when created from unmanaged code
		public TestVacationViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public TestVacationViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public TestVacationViewController () : base ("TestVacationView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new TestVacationView View {
			get {
				return (TestVacationView)base.View;
			}
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			pop = new NSPopover ();
			pop.ContentViewController = new NSViewController ();
			pop.ContentViewController.View = vwDatePopOver;
			pop.ContentSize = new CGSize (139, 190);

			btnCancel.Activated += delegate {
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
			};
			btnOK.Activated += delegate {
				NSApplication.SharedApplication.EndSheet (CommonClass.Panel);
				CommonClass.Panel.OrderOut (this);
				GlobalSettings.SeniorityListMember = new SeniorityListMember ();
				GlobalSettings.SeniorityListMember.Absences = new List<Absense> ();
				if (sw1.State == NSCellStateValue.On) {
					GlobalSettings.SeniorityListMember.Absences.Add (
						new Absense () {
							StartAbsenceDate = dpStart1.DateValue.NSDateToDateTime (),
							EndAbsenceDate = dpEnd1.DateValue.NSDateToDateTime (),
							AbsenceType = "VA"
						});
				}
				if (sw2.State == NSCellStateValue.On) {
					GlobalSettings.SeniorityListMember.Absences.Add (
						new Absense () {
							StartAbsenceDate = dpStart2.DateValue.NSDateToDateTime (),
							EndAbsenceDate = dpEnd2.DateValue.NSDateToDateTime (),
							AbsenceType = "VA"
						});
				}

				if (sw3.State == NSCellStateValue.On) {
					GlobalSettings.SeniorityListMember.Absences.Add (
						new Absense () {
							StartAbsenceDate = dpStart3.DateValue.NSDateToDateTime (),
							EndAbsenceDate = dpEnd3.DateValue.NSDateToDateTime (),
							AbsenceType = "VA"
						});
				}
				if (sw4.State == NSCellStateValue.On) {
					GlobalSettings.SeniorityListMember.Absences.Add (
						new Absense () {
							StartAbsenceDate = dpStart4.DateValue.NSDateToDateTime (),
							EndAbsenceDate = dpEnd4.DateValue.NSDateToDateTime (),
							AbsenceType = "VA"
						});
				}

                if (sw5.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                        StartAbsenceDate = dpStart5.DateValue.NSDateToDateTime(),
                        EndAbsenceDate = dpEnd5.DateValue.NSDateToDateTime(),
                            AbsenceType = "FV"
                        });
                }
                if (sw6.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                        StartAbsenceDate = dpStart6.DateValue.NSDateToDateTime(),
                        EndAbsenceDate = dpEnd6.DateValue.NSDateToDateTime(),
                            AbsenceType = "FV"
                        });
                }

                if (sw7.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                        StartAbsenceDate = dpStart7.DateValue.NSDateToDateTime(),
                        EndAbsenceDate = dpEnd7.DateValue.NSDateToDateTime(),
                            AbsenceType = "FV"
                        });
                }
                if (sw8.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                        StartAbsenceDate = dpStart8.DateValue.NSDateToDateTime(),
                        EndAbsenceDate = dpEnd8.DateValue.NSDateToDateTime(),
                            AbsenceType = "FV"
                        });
                }
                if (sw9.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                            StartAbsenceDate = dpStart9.DateValue.NSDateToDateTime(),
                            EndAbsenceDate = dpEnd8.DateValue.NSDateToDateTime(),
                            AbsenceType = "CFV"
                        });
                }
                if (sw10.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                            StartAbsenceDate = dpStart10.DateValue.NSDateToDateTime(),
                            EndAbsenceDate = dpEnd8.DateValue.NSDateToDateTime(),
                            AbsenceType = "CFV"
                        });
                }
                if (sw11.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                            StartAbsenceDate = dpStart11.DateValue.NSDateToDateTime(),
                            EndAbsenceDate = dpEnd8.DateValue.NSDateToDateTime(),
                            AbsenceType = "CFV"
                        });
                }
                if (sw12.State == NSCellStateValue.On)
                {
                    GlobalSettings.SeniorityListMember.Absences.Add(
                        new Absense()
                        {
                            StartAbsenceDate = dpStart12.DateValue.NSDateToDateTime(),
                            EndAbsenceDate = dpEnd8.DateValue.NSDateToDateTime(),
                            AbsenceType = "CFV"
                        });
                }
                var outofBidPeriod = GlobalSettings.SeniorityListMember.Absences.Where (x => !(x.StartAbsenceDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && x.EndAbsenceDate >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate));
				// Get the ordered absence date.If the multiple vacation exists in the seniority list with consequitive dates ,then will combine the adjacent vacation dates.
				GlobalSettings.OrderedVacationDays = WBidCollection.GetOrderedAbsenceDates ();
				GlobalSettings.TempOrderedVacationDays = WBidCollection.GetOrderedAbsenceDates ();
                GlobalSettings.FVVacation=WBidCollection.GetFVOrderedAbsenceDates();
                var cfvabsence = WBidCollection.GetCFVOrderedAbsenceDates();
                GlobalSettings.FVVacation.AddRange(cfvabsence);

                if (outofBidPeriod.Count () > 0) {
					// MessageBox.Show(outofBidPeriod.Count() + " Test Vacation Week is not in the Bid Period");
					GlobalSettings.SeniorityListMember.Absences.RemoveAll (x => !(x.StartAbsenceDate <= GlobalSettings.CurrentBidDetails.BidPeriodEndDate && x.EndAbsenceDate >= GlobalSettings.CurrentBidDetails.BidPeriodStartDate));

				}
				GlobalSettings.IsVacationCorrection = true;
				GlobalSettings.MenuBarButtonStatus.IsVacationCorrection = true;
                GlobalSettings.IsFVVacation = GlobalSettings.FVVacation.Count > 0;
               
                //GlobalSettings.TempOrderedVacationDays = new List<Absense> { new Absense { StartAbsenceDate = DateTime.Now.AddDays(24), EndAbsenceDate = DateTime.Now.AddDays(31) } };
                //GlobalSettings.OrderedVacationDays=new List<Absense> { new Absense { StartAbsenceDate = DateTime.Now.AddDays(24), EndAbsenceDate = DateTime.Now.AddDays(31) } };
                CommonClass.MainController.PerformVacationReparse ();
			};

			btnStartPop1.Activated += HandleDatePopOver;
			btnStartPop2.Activated += HandleDatePopOver;
			btnStartPop3.Activated += HandleDatePopOver;
			btnStartPop4.Activated += HandleDatePopOver;
			btnEndPop1.Activated += HandleDatePopOver;
			btnEndPop2.Activated += HandleDatePopOver;
			btnEndPop3.Activated += HandleDatePopOver;
			btnEndPop4.Activated += HandleDatePopOver;
            // for FV data.
            btnStartPop5.Activated += HandleDatePopOver;
            btnStartPop6.Activated += HandleDatePopOver;
            btnStartPop7.Activated += HandleDatePopOver;
            btnStartPop8.Activated += HandleDatePopOver;
            btnEndPop5.Activated += HandleDatePopOver;
            btnEndPop6.Activated += HandleDatePopOver;
            btnEndPop7.Activated += HandleDatePopOver;
            btnEndPop8.Activated += HandleDatePopOver;

            btnStartPop9.Activated += HandleDatePopOver;
            btnStartPop10.Activated += HandleDatePopOver;
            btnStartPop11.Activated += HandleDatePopOver;
            btnStartPop12.Activated += HandleDatePopOver;

            btnPopCancel.Activated += delegate {
				pop.Close ();
			};
			btnPopOK.Activated += delegate {
				curPopDate.DateValue = dpPopOver.DateValue;
				pop.Close ();
			};

			TimePickerSetup ();

			setValues ();
		}

		void HandleDatePopOver(object sender, EventArgs e)
        {
            var btn = (NSButton)sender;
            switch (btn.Tag)
            {
                case 11:
                    curPopDate = dpStart1;
                    break;
                case 12:
                    curPopDate = dpStart2;
                    break;
                case 13:
                    curPopDate = dpStart3;
                    break;
                case 14:
                    curPopDate = dpStart4;
                    break;

                case 15:
                    curPopDate = dpStart5;
                    break;
                case 16:
                    curPopDate = dpStart6;
                    break;
                case 17:
                    curPopDate = dpStart7;
                    break;
                case 18:
                    curPopDate = dpStart8;
                    break;

                case 21:
                    curPopDate = dpEnd1;
                    break;
                case 22:
                    curPopDate = dpEnd2;
                    break;
                case 23:
                    curPopDate = dpEnd3;
                    break;
                case 24:
                    curPopDate = dpEnd4;
                    break;

                case 25:
                    curPopDate = dpEnd5;
                    break;
                case 26:
                    curPopDate = dpEnd6;
                    break;
                case 27:
                    curPopDate = dpEnd7;
                    break;
                case 28:
                    curPopDate = dpEnd8;
                    break;

                case 19:
                    curPopDate = dpStart9;
                    break;
                case 110:
                    curPopDate = dpStart10;
                    break;
                case 111:
                    curPopDate = dpStart11;
                    break;
                case 112:
                    curPopDate = dpStart12;
                    break;

            }
            dpPopOver.DateValue = curPopDate.DateValue;
            pop.Show(btn.Frame, this.View, NSRectEdge.MaxXEdge);
        }

		void TimePickerSetup ()
		{
//			dpStart1.Locale = new NSLocale ("NL");
//			dpStart2.Locale = new NSLocale ("NL");
//			dpStart3.Locale = new NSLocale ("NL");
//			dpStart4.Locale = new NSLocale ("NL");
//			dpEnd1.Locale = new NSLocale ("NL");
//			dpEnd2.Locale = new NSLocale ("NL");
//			dpEnd3.Locale = new NSLocale ("NL");
//			dpEnd4.Locale = new NSLocale ("NL");

            dpStart1.TimeZone = NSTimeZone.FromName("GMT");
            dpStart2.TimeZone = NSTimeZone.FromName("GMT");
            dpStart3.TimeZone = NSTimeZone.FromName("GMT");
            dpStart4.TimeZone =NSTimeZone.FromName("GMT");
            dpEnd1.TimeZone = NSTimeZone.FromName("GMT");
            dpEnd2.TimeZone = NSTimeZone.FromName("GMT");
            dpEnd3.TimeZone = NSTimeZone.FromName("GMT");
            dpEnd4.TimeZone = NSTimeZone.FromName("GMT");

            dpStart5.TimeZone = NSTimeZone.FromName("GMT");
            dpStart6.TimeZone = NSTimeZone.FromName("GMT");
            dpStart7.TimeZone = NSTimeZone.FromName("GMT");
            dpStart8.TimeZone = NSTimeZone.FromName("GMT");
            dpEnd5.TimeZone = NSTimeZone.FromName("GMT");
            dpEnd6.TimeZone = NSTimeZone.FromName("GMT");
            dpEnd7.TimeZone = NSTimeZone.FromName("GMT");
            dpEnd8.TimeZone = NSTimeZone.FromName("GMT");

            dpStart9.TimeZone = NSTimeZone.FromName("GMT");
            dpStart10.TimeZone = NSTimeZone.FromName("GMT");
            dpStart11.TimeZone = NSTimeZone.FromName("GMT");
            dpStart12.TimeZone = NSTimeZone.FromName("GMT");

            dpStart1.Calendar = NSCalendar.CurrentCalendar;
            dpStart2.Calendar = NSCalendar.CurrentCalendar;
            dpStart3.Calendar = NSCalendar.CurrentCalendar;
            dpStart4.Calendar = NSCalendar.CurrentCalendar;
            dpEnd1.Calendar = NSCalendar.CurrentCalendar;
            dpEnd2.Calendar = NSCalendar.CurrentCalendar;
            dpEnd3.Calendar = NSCalendar.CurrentCalendar;
            dpEnd4.Calendar = NSCalendar.CurrentCalendar;

            dpStart5.Calendar = NSCalendar.CurrentCalendar;
            dpStart6.Calendar = NSCalendar.CurrentCalendar;
            dpStart7.Calendar = NSCalendar.CurrentCalendar;
            dpStart8.Calendar = NSCalendar.CurrentCalendar;
            dpEnd5.Calendar = NSCalendar.CurrentCalendar;
            dpEnd6.Calendar = NSCalendar.CurrentCalendar;
            dpEnd7.Calendar = NSCalendar.CurrentCalendar;
            dpEnd8.Calendar = NSCalendar.CurrentCalendar;

            dpStart9.Calendar = NSCalendar.CurrentCalendar;
            dpStart10.Calendar = NSCalendar.CurrentCalendar;
            dpStart11.Calendar = NSCalendar.CurrentCalendar;
            dpStart12.Calendar = NSCalendar.CurrentCalendar;

            dpPopOver.TimeZone = NSTimeZone.FromName("GMT");
			dpPopOver.Calendar = NSCalendar.CurrentCalendar;
		}

		private void setValues ()
		{
			DateTime defDate = new DateTime (GlobalSettings.CurrentBidDetails.Year, GlobalSettings.CurrentBidDetails.Month, 1);

			dpStart1.DateValue = defDate.AddDays (5).DateTimeToNSDate();
			dpStart2.DateValue = defDate.AddDays (10).DateTimeToNSDate();
			dpStart3.DateValue = defDate.AddDays (15).DateTimeToNSDate();
			dpStart4.DateValue = defDate.AddDays (20).DateTimeToNSDate();

			dpEnd1.DateValue = defDate.AddDays (10).DateTimeToNSDate();
			dpEnd2.DateValue = defDate.AddDays (15).DateTimeToNSDate();
			dpEnd3.DateValue = defDate.AddDays (20).DateTimeToNSDate();
			dpEnd4.DateValue = defDate.AddDays (25).DateTimeToNSDate();

            dpStart5.DateValue = defDate.AddDays(5).DateTimeToNSDate();
            dpStart6.DateValue = defDate.AddDays(10).DateTimeToNSDate(); 
            dpStart7.DateValue = defDate.AddDays(15).DateTimeToNSDate(); 
            dpStart8.DateValue = defDate.AddDays(20).DateTimeToNSDate(); 

            dpEnd5.DateValue = defDate.AddDays(10).DateTimeToNSDate(); 
            dpEnd6.DateValue = defDate.AddDays(15).DateTimeToNSDate(); 
            dpEnd7.DateValue = defDate.AddDays(20).DateTimeToNSDate(); 
            dpEnd8.DateValue = defDate.AddDays(25).DateTimeToNSDate();

            dpStart9.DateValue = defDate.AddDays(5).DateTimeToNSDate();
            dpStart10.DateValue = defDate.AddDays(10).DateTimeToNSDate();
            dpStart11.DateValue = defDate.AddDays(15).DateTimeToNSDate();
            dpStart12.DateValue = defDate.AddDays(20).DateTimeToNSDate();

            //			if (GlobalSettings.SeniorityListMember != null && GlobalSettings.SeniorityListMember.Absences != null) {
            //				int index = 0;
            //				foreach (Absense abs in GlobalSettings.SeniorityListMember.Absences) {
            //					switch (index) {
            //					case 0:
            //						sw1.State = NSCellStateValue.On;
            //						dpStart1.DateValue = abs.StartAbsenceDate;
            //						dpEnd1.DateValue = abs.EndAbsenceDate;
            //						break;
            //					case 1:
            //						sw2.State = NSCellStateValue.On;
            //						dpStart2.DateValue = abs.StartAbsenceDate;
            //						dpEnd2.DateValue = abs.EndAbsenceDate;
            //						break;
            //					case 2:
            //						sw3.State = NSCellStateValue.On;
            //						dpStart3.DateValue = abs.StartAbsenceDate;
            //						dpEnd3.DateValue = abs.EndAbsenceDate;
            //						break;
            //					case 3:
            //						sw4.State = NSCellStateValue.On;
            //						dpStart4.DateValue = abs.StartAbsenceDate;
            //						dpEnd4.DateValue = abs.EndAbsenceDate;
            //						break;
            //					}
            //					index++;
            //				}
            //			} else {
            //				GlobalSettings.SeniorityListMember = new SeniorityListMember ();
            //				GlobalSettings.SeniorityListMember.Absences = new List<Absense> ();
            //			}

        }

	}
}

