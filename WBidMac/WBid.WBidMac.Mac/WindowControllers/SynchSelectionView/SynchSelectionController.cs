using System;

using Foundation;
using AppKit;
using CoreGraphics;


namespace WBid.WBidMac.Mac.WindowControllers.SynchSelectionView
{
    public partial class SynchSelectionController : NSWindowController
    {

        // selection index

       public int selectionOption;

        // Synch date and time varaibles

        public string localStateDate;
        public string localStatetime;

        public string localQuickSetDate;
        public string localQuickSettime;

        public string sereverStateDate;
        public string serevertatetime;

        public string serverQuickSetDate;
        public string serverQuickSettime;


        public DateTime ServerStateSynchTime;
        public DateTime ServerQSSynchTime;
        public DateTime LocalStateSynchTime;
        public DateTime LocalQSSynchTime;

        // selection stirng
        string selectionString = "000";

        public SynchSelectionController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SynchSelectionController(NSCoder coder) : base(coder)
        {
        }

        public SynchSelectionController() : base("SynchSelection")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.StateView.WantsLayer = true;
            this.QuickView.WantsLayer = true;

            

            if (selectionOption == 1)
            {
               
                this.StateHieghtConstraint.Constant = 0;
                this.QuickHieghtConstraint.Constant = 0;
                this.QuickView.Hidden = true;
                this.StateView.Hidden = false;
            }
            if(selectionOption == 2)
            {
                this.StateHieghtConstraint.Constant = 0;
                this.QuickHieghtConstraint.Constant = 0;
                this.QuickView.Hidden = false;
                this.StateView.Hidden = true;
            }

            var localStateTimeCST = DateTime.MinValue;
            var serverStateTimeCST = DateTime.MinValue;
            var localQSTimeCST = DateTime.MinValue;
            var serverQSTimeCST = DateTime.MinValue;
            //State

            if (LocalStateSynchTime.Year == 0001 && selectionOption!=2)
            {
                lblStateLocalDate.StringValue = "-- /--/--";
                lblStateLocalTime.StringValue = "-- /--/--";
                this.StateSegment.SelectedSegment = 1;


            }
            else
            {

                try
                {
                    localStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalStateSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    localStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalStateSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblStateLocalDate.StringValue = localStateTimeCST.ToShortDateString();
                lblStateLocalTime.StringValue = localStateTimeCST.ToString("hh:mm:ss tt");
            }

            if (ServerStateSynchTime.Year == 0001)
            {
                lblStateServerDate.StringValue = "-- /--/--";
                lblStateServerTime.StringValue = "-- /--/--";
                this.StateSegment.SelectedSegment = 0;

            }
            else
            {

                try
                {
                    serverStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerStateSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    serverStateTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerStateSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblStateServerDate.StringValue = serverStateTimeCST.ToShortDateString();
                lblStateServerTime.StringValue = serverStateTimeCST.ToString("hh:mm:ss tt");
            }


            //quickset

            try
            {
                serverQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
            }
            catch
            {
                serverQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
            }

            if (LocalQSSynchTime.Year == 0001 && selectionOption != 1)
            {
                lblQuickLocalDate.StringValue = "-- /--/--";
                lblQuickLocalTime.StringValue = "-- /--/--";
                this.QuickSegment.SelectedSegment = 1;

            }
            else
            {

                try
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalQSSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(LocalQSSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblQuickLocalDate.StringValue = localQSTimeCST.ToShortDateString();
                lblQuickLocalTime.StringValue = localQSTimeCST.ToString("hh:mm:ss tt");
            }

            if (ServerQSSynchTime.Year == 0001)
            {
                lblQuickServerDate.StringValue = "-- /--/--";
                lblQuickServerTime.StringValue = "-- /--/--";
                this.QuickSegment.SelectedSegment = 0;
            }
            else
            {
                try
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime, TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                catch
                {
                    localQSTimeCST = TimeZoneInfo.ConvertTimeFromUtc(ServerQSSynchTime.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("America/Chicago"));
                }
                lblQuickServerDate.StringValue = serverQSTimeCST.ToShortDateString();
                lblQuickServerTime.StringValue = serverQSTimeCST.ToString("hh:mm:ss tt");
            }
            if (ServerQSSynchTime.Year == 0001 || LocalQSSynchTime.Year == 0001)
            {
                //this.QuickSegment.UserInteractionEnabled = false;
            }
            else
               // this.QuickSegment.UserInteractionEnabled = true;
            if (ServerStateSynchTime.Year == 0001 || LocalStateSynchTime.Year == 0001)
            {
               // this.StateSegment.UserInteractionEnabled = false;
            }
            else
                //this.StateSegment.UserInteractionEnabled = true;

            // Show view with respect to selectedIndex

            if (selectionOption == 1) // View State
            {

                this.QuickView.Hidden = true;
                this.StateHieghtConstraint.Constant = 0;
                this.QuickHieghtConstraint.Constant = 0;
                //this.mainViewHieghtConstraint.Constant = 422;


            }

            else if (selectionOption == 2)//view Quick set
            {

                this.StateView.Hidden = true;
                this.StateHieghtConstraint.Constant = 0;
                this.QuickHieghtConstraint.Constant = 0;
            
                //this.mainViewHieghtConstraint.Constant = 422;

            }

            else if (selectionOption == 3) // View Both
            {

            }

        }

        public new SynchSelection Window
        {
            get { return (SynchSelection)base.Window; }
        }



        partial void SynchAction(NSObject sender)
        {

            
            int segState = (int)this.StateSegment.SelectedSegment;
            int segQuick = (int)this.QuickSegment.SelectedSegment;
            if (segState < 0)
                segState = 0;
            if (segQuick < 0)
                segQuick = 0;

             selectionString = selectionOption.ToString() + segState.ToString() + segQuick.ToString();

            CloseSynch();

            // Notiifcation Obeserver call SynchSelectionNotif

            NSNotificationCenter.DefaultCenter.PostNotificationName("synchProcessing", new NSString(selectionString));

           
        }



        public void CloseSynch()
        {
            
            this.Window.Close();


        }
    }


    

}
