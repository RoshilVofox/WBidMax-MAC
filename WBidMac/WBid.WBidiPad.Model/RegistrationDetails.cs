using System;
namespace WBid.WBidiPad.Model
{
    public class RegistrationDetails
    {

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int EmpNum { get; set; }
            public string Email { get; set; }
            public string CellPhone { get; set; }
            public DateTime? UserAccountDateTime { get; set; }
            public string CellCarrier { get; set; }
            public int Position { get; set; }
            public int CarrierNum { get; set; }
            public bool AcceptEmail { get; set; }
            public string Password { get; set; }

            public DateTime? CBExpirationDate { get; set; }

            public DateTime? WBExpirationDate { get; set; }

            public string BidBase { get; set; }
            public string BidSeat { get; set; }

            public int AppNum { get; set; }


            public bool IsFree { get; set; }


            public bool IsYearlySubscribed { get; set; }


            public bool IsMonthlySubscribed { get; set; }

            public bool IsCBYearlySubscribed { get; set; }


            public bool IsCBMonthlySubscribed { get; set; }


            public string TopSubscriptionLine { get; set; }


            public string SecondSubscriptionLine { get; set; }


            public string ThirdSubscriptionLine { get; set; }

            public string SubscriptionMessage { get; set; }
        
    }
}
