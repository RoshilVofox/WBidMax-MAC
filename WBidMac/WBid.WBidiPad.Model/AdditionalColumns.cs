#region NameSpace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization; 
#endregion

namespace WBid.WBidiPad.Model
{
    public class AdditionalColumns
    {

        public int Id { get; set; }

       
        public string DisplayName { get; set; }

      
        public bool IsRequied { get; set; }

       
        public bool IsSelected { get; set; }


        public string  DataPropertyName { get; set; }


    }
}
