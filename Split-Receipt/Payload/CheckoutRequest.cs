using System.ComponentModel.DataAnnotations;

/// <summary>
/// The namespace <c>Split_Receipt.Payload</c> contains several classes that serve
/// as Data Transfer Objects (DTOs) used to transfer data between different layers of the application. 
/// </summary>
namespace Split_Receipt.Payload
{
    public class CheckoutRequest
    {
        /// <summary>
        ///  The <c>CheckoutRequest</c> class is a Data Transfer Object (DTO)
        ///  used to transfer data between the view's request of creating checkout
        ///  and the saved checkout in the database. This class includes properties for the price,
        ///  currency, whether the checkout is split, and a description.
        ///  The properties are decorated with the Required attribute to ensure that they are not null or empty.
        ///  The <c>Price</c> property is decorated with the Range attribute
        ///  to ensure that it falls within a specified range of values.
        ///  The <c>Description</c> property is decorated with the MinLength
        ///  and MaxLength attributes to ensure that it is between 3 and 30 characters long.
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
