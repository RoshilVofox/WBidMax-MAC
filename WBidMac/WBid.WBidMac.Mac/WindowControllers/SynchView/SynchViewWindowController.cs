using System;

using Foundation;
using AppKit;
using WBid.WBidMac.Mac.WindowControllers.SynchSelectionView;
using CoreGraphics;

namespace WBid.WBidMac.Mac.WindowControllers.SynchView
{
    public partial class SynchViewWindowController : NSWindowController
    {
		// close delgate   

		NSObject notif;

		// Synch date and time varaibles

		


        public DateTime ServerStateSynchTime;
        public DateTime ServerQSSynchTime;
        public DateTime LocalStateSynchTime;
        public DateTime LocalQSSynchTime;

		// synch selection view object

		SynchSelectionController synchSelectionView;

		// selected option (state = 1,quickset = 2, both =3)

		int selectedOption;

		// notification

		NSObject synchSelectionNotif;


		// Called when created from unmanaged code
		public SynchViewWindowController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public SynchViewWindowController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public SynchViewWindowController() : base("SynchViewWindow")
		{
			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{

			



		}



		
		static NSButton closeButton;
		public override void AwakeFromNib()
		{

			// set initial value as both

			this.quickOption.State = NSCellStateValue.Off;
			this.bothOption.State = NSCellStateValue.On;
			this.stateOption.State = NSCellStateValue.Off;
			selectedOption = 3;
		}

		public void CloseSynch()
		{
			
			this.Window.Close();
            // Notiifcation Obeserver call SynchSelectionNotif

            NSNotificationCenter.DefaultCenter.PostNotificationName("synchProcessing", new NSString("000"));
		}




		// option button actions


		partial void StateSelectionAction(NSObject sender)
		{
			this.quickOption.State = NSCellStateValue.Off;
			this.bothOption.State = NSCellStateValue.Off;
			this.stateOption.State = NSCellStateValue.On;
			selectedOption = 1;
		}

		partial void quicksetSelectAction(NSObject sender)
		{
			this.quickOption.State = NSCellStateValue.On;
			this.bothOption.State = NSCellStateValue.Off;
			this.stateOption.State = NSCellStateValue.Off;
			selectedOption = 2;
		}

		partial void bothSelectionAction(NSObject sender)
		{
			this.quickOption.State = NSCellStateValue.Off;
			this.bothOption.State = NSCellStateValue.On;
			this.stateOption.State = NSCellStateValue.Off;
			selectedOption = 3;
		}


		// Ok Action

		partial void OkAction(NSObject sender)
		{


			if (synchSelectionView == null) {

				synchSelectionView = new SynchSelectionController();

				synchSelectionView.selectionOption = selectedOption;

				//synchSelectionView.localStateDate = " ";
				//synchSelectionView.localQuickSetDate = " ";
               
                synchSelectionView.ServerStateSynchTime=ServerStateSynchTime;
                synchSelectionView.ServerQSSynchTime=ServerQSSynchTime;
                synchSelectionView.LocalStateSynchTime=LocalStateSynchTime;
                synchSelectionView.LocalQSSynchTime=LocalQSSynchTime;

				if(selectedOption == 1)
				{
					var frame = synchSelectionView.Window.Frame;
					frame = new CGRect(synchSelectionView.Window.Frame.Location, new CGSize(422, 250));
					synchSelectionView.Window.SetFrame(frame, true);
				}
				else if(selectedOption == 2)
				{
					var frame = synchSelectionView.Window.Frame;
					frame = new CGRect(synchSelectionView.Window.Frame.Location, new CGSize(422, 250));
					synchSelectionView.Window.SetFrame(frame, true);
				}
                else
                {
					var frame = synchSelectionView.Window.Frame;
					frame = new CGRect(synchSelectionView.Window.Frame.Location, new CGSize(422, 600));
					synchSelectionView.Window.SetFrame(frame, true);
				}
				
					


				synchSelectionView.ShowWindow(this);
				
				// notification for synch processing
				synchSelectionNotif = NSNotificationCenter.DefaultCenter.AddObserver((NSString)"closeSynchSelection", (NSNotification n) =>
				{
					NSNotificationCenter.DefaultCenter.RemoveObserver(synchSelectionNotif);
					synchSelectionView.CloseSynch();
					synchSelectionView = null;
                
				});
			}
				




		}

		partial void CancelAction(NSObject sender)
		{
			this.Window.Close();
            // Notiifcation Obeserver call SynchSelectionNotif

            NSNotificationCenter.DefaultCenter.PostNotificationName("synchProcessing", new NSString("000"));
		}



	}
}
