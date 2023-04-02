using Microsoft.Build.Framework;
using Split_Receipt.Areas.Identity.Data;

namespace Split_Receipt.Models
{
    /// <summary>
    /// Class <c>Checkout</c> is model for checkouts saved in DB.
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
