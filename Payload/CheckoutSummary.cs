namespace Split_Receipt.Payload
{
    public class CheckoutSummary
    {
        public CheckoutSummary()
        {
        }

        public CheckoutSummary(string email, decimal total, List<CheckoutResponse> checkouts)
        {
            Email = email;
            Total = total;
            Checkouts = checkouts;
        }

        public string Email { get; set; }
        public decimal Total { get; set; }
        public List<CheckoutResponse> Checkouts { get; set; }

    }
}
