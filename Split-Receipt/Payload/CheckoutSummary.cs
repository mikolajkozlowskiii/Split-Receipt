namespace Split_Receipt.Payload
{
    public class CheckoutSummary
    {
        /// <summary>
        /// Class <c>CheckoutSummary</c> is a class that represent summary of all checkouts
        /// for specific Group.
        /// </summary>
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
        public string Currency { get; set; }
        public string GroupName { get; set; }
        public decimal Total { get; set; }
        public List<CheckoutResponse> Checkouts { get; set; }
        public List<String> Members { get; set; }

    }
}
