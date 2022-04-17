using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [DataContract]
    [Serializable]
    public class Theme
    {
        public  Theme()
        { 
        }
        public Theme(Theme theme)
		{
			if (theme == null) {
				FontType = "Arial";
				FontSize = "10";
				Background = new BackgroundColor () {
					DialogColor = "#FFDCDCDC",
					TextColor = "#FFE6E6FA",
					AMTripsColor = "#FFDB7093",
					PMTripsColor = "#FF6495ED",
					MixedTripsColor = "#FFFFFF00",
					AMReserveColor = "#FFFFC0CB",
					PMReserveColor = "#FFB0C4DE",
					ReadyReserveColor = "#FFA0522D",
					VacationOverlapColor = "#FF98FB98",
					VacationDropColor = "#FFFF0000",
					VacationColor = "#FF00FF00"
				};

				TextColor = new TextColor () {
					DialogText = "#FF000000",
					TextBox = "#FF000000",
					AMTripsText = "#FF000000",
					PMTripsText = "#FF000000",
					MixedTripsText = "#FF000000",
					AMReserveText = "#FF000000",
					PMReserveText = "#FF000000",
					ReadyReserveText = "#FF000000",
					VacationOverlapText = "#FF000000",
					VacationDropText = "#FF000000",
					VacationText = "#FF000000"
				};
			} else {
				FontType = theme.FontType;
				FontSize = theme.FontSize;
				Background = theme.Background;
				TextColor = theme.TextColor;
			}
		}

        [DataMember]
        [XmlAttribute("FontType")]
        public string FontType { get; set; }

        [DataMember]
        [XmlAttribute("FontSize")]
        public string FontSize { get; set; }

        [DataMember]
        [XmlElement("Background")]
        public BackgroundColor Background { get; set; }

        [DataMember]
        [XmlElement("Text")]
        public TextColor TextColor { get; set; }



    }
}
