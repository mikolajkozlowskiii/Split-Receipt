using Split_Receipt.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Split_Receipt.Models
{
    public class User_Group
    {
        public User_Group()
        {
        }

        public User_Group(int groupId, string userId) // zmiana parametru z UserEmail na UserId
        {
            GroupId = groupId;
            UserId = userId; // zmiana pola z UserEmail na UserId
        }
    

        public int Id { get; set; }

        public string UserId { get; set; }
        public int GroupId { get; set; }

        //nawigacja
        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }

        public override string? ToString()
        {
            string userEmail = User != null ? User.Email : "null";
            return "Id: " + Id + ", UserId: " + UserId + ", GroupId: " + GroupId + ", User: " + userEmail;
        }
    }
}
