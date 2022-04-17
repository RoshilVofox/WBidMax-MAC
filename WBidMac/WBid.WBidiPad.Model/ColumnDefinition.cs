using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
    [XmlRoot("ColumnDefinitions")]
    public class ColumnDefinitions : List<ColumnDefinition>
    {

    }
    public class ColumnDefinition
    { 
        /// <summary>
        /// Column Id
        /// </summary>
        [XmlAttribute("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Column Code
        /// </summary>
        [XmlAttribute("Order")]
        public int Order { get; set; }

        /// <summary>
        /// Column Wdth
        /// </summary>
        [XmlAttribute("Width")]
        public int Width { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Data Property Name
        /// </summary>
        [XmlAttribute("DataPropertyName")]
        public string DataPropertyName { get; set; }

        /// <summary>
        /// Is required
        /// </summary>
        [XmlAttribute("IsRequied")]
        public bool IsRequied { get; set; }

    }
}
