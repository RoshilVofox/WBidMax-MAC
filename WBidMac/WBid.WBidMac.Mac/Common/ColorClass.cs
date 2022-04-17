using System;
using CoreGraphics;
using Foundation;
using AppKit;

namespace WBid.WBidMac.Mac
{
	public class ColorClass
	{
		public ColorClass ()
		{
		}
		// Summary View Colors.
		public static NSColor TopHeaderColor = NSColor.FromSrgb (133f/255f, 192f/255f, 57f/255f, 1);	//Main bar color.
		public static NSColor BottomHeaderColor = NSColor.FromSrgb (238f/255f, 255f/255f, 217f/255f, 1);	//View bar color.
		public static NSColor SummaryHeaderColor = NSColor.FromSrgb (207f/255f, 226f/255f, 183f/255f, 1);	//Summary header background.
		public static NSColor SummaryHeaderBorderColor = NSColor.FromSrgb (158f/255f, 179f/255f, 131f/255f, 1); //Summary header Border.

		public static NSColor AlternatingLineColor1 = NSColor.FromSrgb (255f/255f, 255f/255f, 255f/255f, 1); 
		public static NSColor AlternatingLineColor2 = NSColor.FromSrgb (249f/255f, 249f/255f, 249f/255f, 1); 
		public static NSColor ListSeparatorColor = NSColor.FromSrgb (220f/255f, 220f/255f, 220f/255f, 1);

		public static NSColor BlankLineColor = NSColor.FromSrgb (254f/255f, 255f/255f, 0f/255f, 1);
		public static NSColor ReserveLineColor = NSColor.FromSrgb (253f/255f, 181f/255f, 193f/255f, 1);

//public static NSColor AmLineSummaryColor = NSColor.FromSrgb(175f / 255f, 203f / 255f, 247f / 255f, 1);
//public static NSColor PmLineSummaryColor = NSColor.FromSrgb(255f / 255f, 222f / 255f, 164f / 255f, 1);

		public static NSColor AmLineSummaryColor = NSColor.FromSrgb(216f / 255f, 109f / 255f, 148f / 255f, 1);
		public static NSColor PmLineSummaryColor = NSColor.FromSrgb(105f / 255f, 143f / 255f, 239f / 255f, 1);

		public static NSColor activeGreen = NSColor.FromSrgb (133f/255f, 192f/255f, 57f/255f, 1);
		public static NSColor activeOrange = NSColor.FromSrgb (248f/255f, 128f/255f, 0f/255f, 1);

		public static NSColor OffDayColor = NSColor.FromSrgb (226f/255f, 0f/255f, 9f/255f, 1);
		public static NSColor WorkDayColor = NSColor.FromSrgb (135f/255f,201f/255f,39f/255f, 1);


		// Modern View Colors.
		public static NSColor normDayColor = NSColor.FromSrgb (254f/255f, 247f/255f, 170f/255f, 1); 
		public static NSColor weekendDayColor = NSColor.FromSrgb (253f/255f, 209f/255f, 163f/255f, 1);
		public static NSColor nextMonthDayColor = NSColor.FromSrgb (211f/255f, 124f/255f, 97f/255f, 1);

		public static NSColor normTripColor = NSColor.FromSrgb (255f/255f, 250f/255f, 240f/255f, 1);
		public static NSColor weekendTripColor = NSColor.FromSrgb (254f/255f, 235f/255f, 211f/255f, 1);
		public static NSColor nextMonthTripColor = NSColor.FromSrgb (212f/255f, 150f/255f, 144f/255f, 1);

		public static NSColor AMTripColor = NSColor.FromSrgb (216f/255f, 109f/255f, 148f/255f, 1);
		public static NSColor PMTripColor = NSColor.FromSrgb (105f/255f, 143f/255f, 239f/255f, 1);
		public static NSColor MixedTripColor = NSColor.FromSrgb (254f/255f, 255f/255f, 0f/255f, 1);

		public static NSColor AMReserveTripColor = NSColor.FromSrgb (253f/255f, 191f/255f, 203f/255f, 1);
		public static NSColor PMReserveTripColor = NSColor.FromSrgb (177f/255f, 195f/255f, 223f/255f, 1);

		public static NSColor VacationTripColor = NSColor.FromSrgb(0f/255f, 255f/255f, 0f/255f, 1);
		public static NSColor VAPColor = NSColor.FromSrgb(64f/255f, 179f/255f, 26f/255f, 1);
		public static NSColor VacationDropTripColor = NSColor.FromSrgb(222f/255f, 54f/255f, 37f/255f, 1);
		public static NSColor VacationOverlapTripColor = NSColor.FromSrgb(152f/255f, 251f/255f, 152f/255f, 1);
		public static NSColor OverlapColor = NSColor.FromSrgb(243f/255f, 108f/255f, 108f/255f, 1);

        public static NSColor FVVacationColor = NSColor.FromSrgb(102f/255f, 179f/ 255f, 255f/255f,1);


		public static NSColor CFVVacationColor = NSColor.FromSrgb(135f / 255f, 131f / 255f, 120f / 255f, 1);

		public static NSColor ReadyReserveTripColor = NSColor.FromSrgb(255 / 255f, 165f / 255f, 0f / 255f, 1);


	}
}

