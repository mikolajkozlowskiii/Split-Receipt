using Split_Receipt.Areas.Identity.Data;
using System.Text.RegularExpressions;

namespace Split_Receipt.Models
{
    public class Checkout
    {
        public Checkout()
        {
        }

        public Checkout(int id, decimal price, string currency, bool isSplitted, string description, string userId, int groupId, ApplicationUser user, Group group)
        {
            Id = id;
            Price = price;
            Currency = currency;
            IsSplitted = isSplitted;
            Description = description;
            UserId = userId;
            GroupId = groupId;
            //User = user;
            //Group = group;
        }

        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public bool IsSplitted { get; set; }
        public string Description { get; set; }

        //klucze obce
        public string UserId { get; set; }
        public int GroupId { get; set; }

        //nawigacja
        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }

        public override string? ToString()
        {
            return "Id: " + Id + ", Price" + Price + ", Currency" + Currency + ", IsSplitted " + IsSplitted + ", Description: " + Description;
        }
    }
}
