using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.Model.State.Weights
{
     [Serializable]
    public class DaysOfMonthWt
    {
        public bool isWork { get; set; }

        public List<Wt> Weights { get; set; }


        public DaysOfMonthWt()
        {
                
        }

        public DaysOfMonthWt(DaysOfMonthWt daysOfMonthWt)
        {
            isWork = daysOfMonthWt.isWork;

            if (daysOfMonthWt.Weights != null)
            {
                Weights = new List<Wt>();
                foreach (var item in daysOfMonthWt.Weights)
                {
                    Weights.Add(new Wt(item));
                }
            }

        }
    }
}
