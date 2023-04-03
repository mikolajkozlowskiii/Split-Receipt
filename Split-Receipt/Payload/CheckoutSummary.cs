namespace Split_Receipt.Payload
{
    public class CheckoutSummary
    {
        /// <summary>
        /// The <c>CheckoutSummary</c> class is a representation of a summary of all the checkouts
        /// made for a specific group. This class contains properties such as <c>Email</c>,
        /// <c>Currency</c>, <c>GroupName</c>, <c>Total</c>, <c>Checkouts</c>, and <c>Members</c>
        /// which are used to store email, currency, group name, total amount, a list of checkouts,
        /// and a list of members respectively.
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
