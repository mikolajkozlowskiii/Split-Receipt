namespace Split_Receipt.Payload
{
    public class CheckoutRequest
    {
        public CheckoutRequest()
        {
        }

        public CheckoutRequest(decimal price, string currency, bool isSplitted, string description)
        {
            Price = price;
            Currency = currency;
            IsSplitted = isSplitted;
            Description = description;
        }

        public decimal Price { get; set; }
        public string Currency { get; set; }
        public bool IsSplitted { get; set; }
        public string Description { get; set; }
    }
}
