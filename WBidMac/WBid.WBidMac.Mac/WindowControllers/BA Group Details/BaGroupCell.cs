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
using WBid.WBidiPad.PortableLibrary;
using System.Collections.ObjectModel;

namespace WBid.WBidMac.Mac
{
	public partial class BaGroupCell : AppKit.NSTableCellView
	{
		public BaGroupCell ()
		{

		}
		public BAgroupWindowController _parent;
		int order;

		#region Constructors

		public static WBidState wBIdStateContent;
		// = GlobalSettings.WBidStateCollection.StateList.FirstOrDefault(x => x.StateName == GlobalSettings.WBidStateCollection.DefaultName);
		static WBidIntialState wbidintialState;
		// = XmlHelper.DeserializeFromXml<WBidIntialState>(WBidHelper.GetWBidDWCFilePath());

	
		// Called when created from unmanaged code
		public BaGroupCell (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BaGroupCell (NSCoder coder) : base (coder)
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

		/// <summary>
		/// retuurn true if any value chnages from the dafault state.
		/// </summary>
		/// <returns></returns>
	
		public void BindData (string constraint, string head,string SubString)
		{
			//_parent = parent;
			//			var lst = CommonClass.ConstraintsController.appliedConstraints;
			//			order = index - lst.IndexOf (lst.FirstOrDefault (x => x == constraint));
			//order = index;
			if (constraint == "Cell") {
				NSTextField txthead = (NSTextField)ViewWithTag ((nint)2);
				txthead.StringValue =head;

				NSTextField txtDetails = (NSTextField)ViewWithTag ((nint)3);

				txtDetails.StringValue =SubString;

			} else if (constraint == "Header") {
				NSTextField txttitle = (NSTextField)ViewWithTag ((nint)1);
				txttitle.StringValue = SubString;
			}

		}

//		public void BindData (string constraint, int index,BAgroupWindowController parent,List<AppliedStateType> AppliedStateTypes,string title)
//		{
//			_parent = parent;
////			var lst = CommonClass.ConstraintsController.appliedConstraints;
////			order = index - lst.IndexOf (lst.FirstOrDefault (x => x == constraint));
//			order = index;
//			if (constraint == "Cell") {
//				NSTextField txthead = (NSTextField)ViewWithTag ((nint)2);
//				txthead.StringValue = AppliedStateTypes [index].Key;
//
//				NSTextField txtDetails = (NSTextField)ViewWithTag ((nint)3);
//				string concat="\n\n\n";
//				if(AppliedStateTypes[index].Value !=null)
//					concat = String.Join("\n", AppliedStateTypes[index].Value.ToArray());
//				txthead.StringValue =concat;
//
//			} else if (constraint == "Header") {
//				NSTextField txttitle = (NSTextField)ViewWithTag ((nint)1);
//				txttitle.StringValue = title;
//			}
//
//		}

	}









}

