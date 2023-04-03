namespace Split_Receipt.Payload
{
    /// <summary>
    ///  The <c>CheckoutResponse</c> class in the <c>Split_Receipt.Payload</c>
    ///  namespace is a Data Transfer Object (DTO) that represents a checkout
    ///  response object between a saved checkout in the database and the response
    ///  sent back to the view. It contains various properties such as checkout id,
    ///  user email, price, currency, etc. that are used to build the response object.
    ///  Additionally, it overrides the Equals(), GetHashCode(), and ToString() methods
    ///  to provide functionality for comparing and displaying the object.
    /// </summary>
    public class CheckoutResponse
    {
        public CheckoutResponse()
        {
        }

        public CheckoutResponse(int checkoutId, string userEmail, decimal price, string currency,
            bool isSplitted, string description, string userId, int groupId, DateTime createdAt)
        {
            CheckoutId = checkoutId;
            UserEmail = userEmail;
            Price = price;
            Currency = currency;
            IsSplitted = isSplitted;
            Description = description;
            UserId = userId;
            GroupId = groupId;
            CreatedAt = createdAt;
        }

        public int CheckoutId { get; set; }
        public string UserEmail { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public bool IsSplitted { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public int GroupId { get; set; }
        public DateTime CreatedAt { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is CheckoutResponse))
            {
                return false;
            }

            CheckoutResponse other = (CheckoutResponse)obj;

            return this.CheckoutId == other.CheckoutId &&
                   this.UserEmail == other.UserEmail &&
                   this.Price == other.Price &&
                   this.Currency == other.Currency &&
                   this.IsSplitted == other.IsSplitted &&
                   this.Description == other.Description &&
                   this.UserId == other.UserId &&
                   this.GroupId == other.GroupId &&
                   this.CreatedAt == other.CreatedAt;
        }


        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + this.CheckoutId.GetHashCode();
            hash = hash * 23 + (this.UserEmail ?? "").GetHashCode();
            hash = hash * 23 + this.Price.GetHashCode();
            hash = hash * 23 + (this.Currency ?? "").GetHashCode();
            hash = hash * 23 + this.IsSplitted.GetHashCode();
            hash = hash * 23 + (this.Description ?? "").GetHashCode();
            hash = hash * 23 + (this.UserId ?? "").GetHashCode();
            hash = hash * 23 + this.GroupId.GetHashCode();
            hash = hash * 23 + this.CreatedAt.GetHashCode();

            return hash;
        }

        public override string? ToString()
        {
            return "User: " + UserEmail + ", price " + Price + ", currency " + Currency + ", isSpliited " + IsSplitted + ", desc: " + Description + ", date:" + CreatedAt;
        }
    }
}
