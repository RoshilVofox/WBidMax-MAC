using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{
     [Serializable]
    public class SortDetails
    {
         public SortDetails()
         { 
         }
         public SortDetails(SortDetails sortDetails)
         {
             SortColumn = sortDetails.SortColumn;
             SortDirection = sortDetails.SortDirection;
             if (sortDetails.BlokSort != null)
             {
                 BlokSort = new List<string>();
                 foreach (var item in sortDetails.BlokSort)
                 {
					BlokSort.Add(item);
                 }   
 
             }

            // this.BlokSort = sortDetails.BlokSort;
             SortColumnName = sortDetails.SortColumnName;

         }

        /// <summary>
        /// PURPOSe : Sort Column
        /// </summary>
        [XmlAttribute("SortColumn")]
        public string SortColumn { get; set; }


        /// <summary>
        /// PURPOSe : Sort Direction
        /// </summary>
        [XmlAttribute("SortDirection")]
        public string SortDirection { get; set; }


		[XmlElement("BlockSort")]
         public List<string> BlokSort { get; set; }

        /// <summary>
        /// PURPOSe : Sort Column
        /// </summary>
        [XmlAttribute("SortColumnName")]
        public string SortColumnName { get; set; }


    }
}
