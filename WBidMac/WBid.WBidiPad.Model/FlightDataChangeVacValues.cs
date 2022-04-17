using System;
namespace WBid.WBidiPad.Model
{
    public class FlightDataChangeVacValues
    {
        //
        public FlightDataChangeVacValues()
        {
        }
        public int Id { get; set; }
        public int LineNum { get; set; }
        public Nullable<decimal> OldTotalPay { get; set; }
        public Nullable<decimal> NewTotalPay { get; set; }
        public Nullable<decimal> OldVPCu { get; set; }
        public Nullable<decimal> NewVPCu { get; set; }
        public Nullable<decimal> OldVPNe { get; set; }
        public Nullable<decimal> NewVPNe { get; set; }
        public Nullable<decimal> OldVPBo { get; set; }
        public Nullable<decimal> NewVPBo { get; set; }
    }
}
