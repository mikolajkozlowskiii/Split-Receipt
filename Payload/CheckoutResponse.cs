namespace Split_Receipt.Payload
{
    public class CheckoutResponse
    {
        public CheckoutResponse()
        {
        }

        public CheckoutResponse(string userEmail, decimal price, string currency, bool isSplitted, string description, string userId, int groupId)
        {
            UserEmail = userEmail;
            Price = price;
            Currency = currency;
            IsSplitted = isSplitted;
            Description = description;
            UserId = userId;
            GroupId = groupId;
        }
        public int CheckoutId { get; set; }
        public string UserEmail { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public bool IsSplitted { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public int GroupId { get; set; }

        public override string? ToString()
        {
            return "User: " + UserEmail + ", price " + Price + ", currency " + Currency + ", isSpliited " + IsSplitted + ", desc: " + Description;
        }
    }
}
