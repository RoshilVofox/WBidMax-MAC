﻿// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace WBid.WBidMac.Mac.WindowControllers
{
    [Register ("SecretDataCell")]
    partial class SecretDataCell
    {
        [Outlet]
        AppKit.NSButton btnDomicileCheck { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (btnDomicileCheck != null) {
                btnDomicileCheck.Dispose ();
                btnDomicileCheck = null;
            }
        }
    }
}
