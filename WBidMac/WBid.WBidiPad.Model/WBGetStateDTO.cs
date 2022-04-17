using System;
namespace WBid.WBidiPad.Model
{
    public class WBGetStateDTO
    {
        public WBGetStateDTO()
        {
        }
        public int Employeeumber { get; set; }
        public string StateName { get; set; }

        public string QuickSetFileName { get; set; }
        public int Year { get; set; }
        //If the fileType is zero, we need to send Wbid file only ,
        //If the fileType is One , we need to send Quickset Only
        //If the fileType is Two, we need to send both wbidstate and quick sets
        public int FileType { get; set; }
    }
}
