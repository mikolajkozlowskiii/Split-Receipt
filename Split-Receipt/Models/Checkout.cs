using Microsoft.Build.Framework;
using Split_Receipt.Areas.Identity.Data;

/// <summary>
/// The <c>Split_Receipt.Models</c> namespace contains all the model classes used in the Split Receipt application.
/// These classes define the structure of the database tables and relationships between them.
/// </summary>
namespace Split_Receipt.Models
{
    /// <summary>
    /// Class <c>Checkout</c> is a model for checkouts saved in the database.
    /// The class defines properties for checkout details such as price, currency, and description.
    /// The properties are decorated with the Required attribute to ensure that they are not null or empty.
    /// The class also includes a User property of type ApplicationUser and a Group property of type Group,
    /// which represent the user who made the purchase and the group the purchase belongs to, respectively.
    /// The class includes a ToString() method that returns a string representation of the checkout object.
    /// </summary>
    public class Checkout
    {
        public Checkout()
        {
        }
        
        public Checkout(int id, decimal price, string currency, bool isSplitted,
            string description, string userId, int groupId, ApplicationUser user, Group group)
        {
            Id = id;
            Price = price;
            Currency = currency;
            IsSplitted = isSplitted;
            Description = description;
            UserId = userId;
            GroupId = groupId;
            CreatedAt = DateTime.Now;
        }

        public int Id { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public bool IsSplitted { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int GroupId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }

        public override string? ToString()
        {
            return "Id: " + Id + ", Price" + Price + ", Currency" + Currency +
                ", IsSplitted " + IsSplitted + ", Description: " + Description;
        }
    }
}
