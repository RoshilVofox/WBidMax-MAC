
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using WBid.WBidiPad.Core;

namespace WBid.WBidMac.Mac
{
	public partial class ConfigurationWindow : AppKit.NSWindow
	{
		#region Constructors
		private  string _lastEnter=string.Empty;

		// Called when created from unmanaged code
		public ConfigurationWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ConfigurationWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
			if ((theEvent.ModifierFlags.HasFlag (NSEventModifierMask.ControlKeyMask))) {

				//NSKey.
				_lastEnter += theEvent.CharactersIgnoringModifiers.ToUpper ();

				if (_lastEnter == "CD" || _lastEnter == "DC") {
					CommonClass.isShowDataTab = !CommonClass.isShowDataTab;
					if (CommonClass.ConfigureController != null)
						CommonClass.ConfigureController.SetupViews ();
				} else if (_lastEnter != "D" && _lastEnter != "C") {
					_lastEnter = string.Empty;
				}

			} else {
				_lastEnter = string.Empty;

			}
		}
		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
			_lastEnter = string.Empty;
		}

		#endregion
	}
}

