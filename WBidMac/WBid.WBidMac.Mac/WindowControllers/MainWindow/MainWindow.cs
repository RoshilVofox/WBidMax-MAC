
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{


	// mwindows delegate

	public class MainWindowDelegate : AppKit.NSWindowDelegate
	{
		public override bool WindowShouldClose(NSObject sender)
		{
			Console.WriteLine("Should");

			return true;
		}

		public override void WillClose(NSNotification notification)
		{
			NSNotificationCenter.DefaultCenter.PostNotificationName("closeSynch", null);
		}
	}




	public partial class MainWindow : AppKit.NSWindow
	{
		#region Constructors

		private  string _lastEnter=string.Empty;

		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{

			this.Delegate = new MainWindowDelegate();

		}

		
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
			if (theEvent.Characters.ToUpper () == "T")
				CommonClass.isTKeyPressed = true;
			if (theEvent.Characters.ToUpper () == "B")
				CommonClass.isBKeyPressed = true;


			if ((theEvent.ModifierFlags.HasFlag (NSEventModifierMask.ControlKeyMask))) {

				//NSKey.
				_lastEnter += theEvent.CharactersIgnoringModifiers.ToUpper ();



				if (_lastEnter == "FB" || _lastEnter == "BF") {
					CommonClass.MainController.vwAdmin.Hidden = !CommonClass.MainController.vwAdmin.Hidden;
					//Console.WriteLine ("Success");
					_lastEnter = string.Empty;
				} else if (_lastEnter == "TE" || _lastEnter == "ET") {
					//QA view
					var panel = new NSPanel ();
					var qaView = new QAEnvironmentViewController ();
					CommonClass.Panel = panel;
					panel.SetContentSize (new CoreGraphics.CGSize (300, 300));
					panel.ContentView = qaView.View;
					NSApplication.SharedApplication.BeginSheet (panel, CommonClass.MainController.Window);
				} else if (_lastEnter != "F" && _lastEnter != "B"&&_lastEnter != "T" && _lastEnter != "E") {
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
		public override void FlagsChanged (NSEvent theEvent)
		{
			base.FlagsChanged (theEvent);
			//Console.WriteLine (theEvent.ToString ());
			////if ((theEvent.ModifierFlags & NSEventModifierMask.ControlKeyMask) ) 
			//{
				//&& (theEvent.KeyCode ==  "1")

			//}

//			if (theEvent.ModifierFlags >= NSEventModifierMask.AlternateKeyMask)
//				Console.WriteLine ("Alt Pressed");
//			if (theEvent.ModifierFlags >= NSEventModifierMask.ControlKeyMask)
//				Console.WriteLine ("Ctrl Pressed");
//			if (theEvent.ModifierFlags >= NSEventModifierMask.CommandKeyMask)
//				Console.WriteLine ("Cmd Pressed");
//			if (theEvent.ModifierFlags >= NSEventModifierMask.ShiftKeyMask)
//				Console.WriteLine ("Shift Pressed");

		}

		#endregion


	}
}

