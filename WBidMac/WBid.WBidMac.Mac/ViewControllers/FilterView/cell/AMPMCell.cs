using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

//using System;
using CoreGraphics;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.PortableLibrary.Utility;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core;

//using System.Linq;
using WBid.WBidiPad.SharedLibrary.Utility;
using WBid.WBidiPad.iOS.Utility;
using WBid.WBidiPad.PortableLibrary.BusinessLogic;

//using System.Collections.Generic;
using WBid.WBidiPad.Core.Enum;

namespace WBid.WBidMac.Mac
{
	public partial class AMPMCell1 : AppKit.NSTableCellView
	{
		public AMPMCell1 ()
		{

		}



		#region Constructors

		public static WBidState wBIdStateContent;
		// = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		static WBidIntialState wbidintialState;
		// = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

	
		// Called when created from unmanaged code
		public AMPMCell1 (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AMPMCell1 (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			wBIdStateContent = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault (x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
			wbidintialState = XmlHelper.DeserializeFromXml<WBidIntialState> (WBidHelper.GetWBidDWCFilePath ());

		}

	


		public void BindData (string constraint, int index)
		{


		}
	}
}

