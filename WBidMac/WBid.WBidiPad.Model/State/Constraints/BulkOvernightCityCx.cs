using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WBid.WBidiPad.Model
{

   
    public class BulkOvernightCityCx
    {
        #region Constructor
        public BulkOvernightCityCx()
        {
        }
        public BulkOvernightCityCx(BulkOvernightCityCx bulkOvernightCityCx)
        {
            if(bulkOvernightCityCx.OverNightYes!=null)
            {
                OverNightYes = new List<int>();
                foreach (int item in bulkOvernightCityCx.OverNightYes)
                {
                    OverNightYes.Add(item);
                }
            }
            if (bulkOvernightCityCx.OverNightNo != null)
            {
                OverNightNo = new List<int>();
                foreach (int item in bulkOvernightCityCx.OverNightNo)
                {
                    OverNightNo.Add(item);
                }
            }
        }
        #endregion

        [XmlElement("OverNightYes")]
        public List<int> OverNightYes { get; set; }
        [XmlElement("OverNightNo")]
        public List<int> OverNightNo { get; set; }

    }
}
