using System.ComponentModel.DataAnnotations;
namespace Split_Receipt.Payload
{
    public class CheckoutRequest
    {
        /// <summary>
        ///  Class <c>CheckoutRequest</c> is a DTO's object
        ///  beetween view's request of creating checkout and saved checkout in DB.
        /// </summary>
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
        [Required]
        [Range(0.01,1000000)]
        public decimal Price { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public bool IsSplitted { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Description { get; set; }
    }
}
