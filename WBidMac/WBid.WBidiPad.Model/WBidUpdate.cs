using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model
{
    public class WBidUpdate
    {

        #region Properties


        /// <summary>
        /// PURPOSE : Updates
        /// </summary>
        public Updates Updates { get; set; }

        /// <summary>
        /// PURPOSE : Cities
        /// </summary>
        public List<City> Cities { get; set; }

        /// <summary>
        /// PURPOSE : Hotels
        /// </summary>
        public Hotels Hotels { get; set; }


        // public Internationals InternationalCities { get; set; }


        // public NonConusCities NonConusCities { get; set; }

        /// <summary>
        /// PURPOSE :Domicile List
        /// </summary>
        public List<Domicile> Domiciles { get; set; }

        /// <summary>
        /// PURPOSE : Equipments
        /// </summary>
        public List<Equipment> Equipments { get; set; }

        /// <summary>
        /// PURPOSE : EquipmentTypes
        /// </summary>
        public List<EquipType> EquipTypes { get; set; }

        public SplitPointCities SplitPointCities { get; set; }

        #endregion
    }
}
