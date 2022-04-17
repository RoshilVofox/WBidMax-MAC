using System;
namespace WBid.WBidiPad.Model
{
    public class CustomServiceResponse
    {
        public CustomServiceResponse()
        {
        }
        public bool Status { get; set; }
        public string Message { get; set; }

        public DateTime? CBExpirationDate { get; set; }

        public DateTime? WBExpirationDate { get; set; }

        public string TopSubscriptionLine { get; set; }

        public string SecondSubscriptionLine { get; set; }

        public string ThirdSubscriptionLine { get; set; }
    }
}
