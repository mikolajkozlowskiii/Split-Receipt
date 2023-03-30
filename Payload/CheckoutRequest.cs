using System.ComponentModel.DataAnnotations;
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
        [Required]
        [MaxLength(9)]
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
