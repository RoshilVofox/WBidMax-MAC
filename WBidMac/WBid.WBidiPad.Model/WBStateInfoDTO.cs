using System;
namespace WBid.WBidiPad.Model
{
    public class WBStateInfoDTO
    {
        public WBStateInfoDTO()
        {
        }
		public string StateVersionNumber { get; set; }
public string QuickSetVersionNumber { get; set; }
public string StateLastUpdatedDate { get; set; }
public DateTime StateLastUpdate { get; set; }
public string QuickSetLastUpdatedDate { get; set; }
public DateTime QuickSetLastUpdated { get; set; }
	}
}
