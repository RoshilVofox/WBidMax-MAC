
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public partial class HomeWindow : AppKit.NSWindow
	{

		private  string _lastEnter=string.Empty;
		#region Constructors

		// Called when created from unmanaged code
		public HomeWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public HomeWindow (NSCoder coder) : base (coder)
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



			 if (_lastEnter == "TE" || _lastEnter == "ET") {
					//QA view
					var panel = new NSPanel ();
					var qaView = new QAEnvironmentViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (300, 300));
					panel.ContentView = qaView.View;
					NSApplication.SharedApplication.BeginSheet (panel, CommonClass.HomeController. Window);
				} else if (_lastEnter != "T" && _lastEnter != "E") {
					_lastEnter = string.Empty;
				}





				//var a=10;

			} else {
				_lastEnter = string.Empty;
			}
			//&& (theEvent.KeyCode == 18)
			//var aa=theEvent.KeyCode;
			//Console.WriteLine (theEvent.ToString ());
		}

		public override void KeyUp (NSEvent theEvent)
		{
			base.KeyUp (theEvent);
			if (theEvent.Characters.ToUpper () == "T")
				CommonClass.isTKeyPressed = false;
			if (theEvent.Characters.ToUpper () == "B")
				CommonClass.isBKeyPressed = false;

			_lastEnter = string.Empty;}
		#endregion
	}
}

