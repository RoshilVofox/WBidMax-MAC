using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using System.Collections.ObjectModel;
using WBid.WBidiPad.PortableLibrary;
using System;
using CoreGraphics;

namespace WBid.WBidMac.Mac.WindowControllers
{
   
    public partial class SecretDataCell : AppKit.NSTableCellView
    {
        SecretDataDownloadController SuperparentWC;
        public SecretDataCell(CGRect frame)
        {
            this.Frame = frame;
        }


        // Called when created from unmanaged code
        public SecretDataCell(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public SecretDataCell(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }
      //  NSCellStateValue.On
        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            btnDomicileCheck.Activated += (object sender, EventArgs e) => {
                //SelectedBases(((NSButton)sender).State);
                if (((NSButton)sender).State == NSCellStateValue.On) {

                    SuperparentWC.setSelectedBases((int)((NSButton)sender).Tag);
                }
                else {
                    SuperparentWC.removeSelectedBases((int)((NSButton)sender).Tag);

                }


            };

        }

        public void CheckDomiciles(int index) {

        }

        public void BindData(string name, int index,SecretDataDownloadController parent)
        {
           btnDomicileCheck.Title = name;
            btnDomicileCheck.Tag = index + 1;
            SuperparentWC = parent;
        }
    }
}
