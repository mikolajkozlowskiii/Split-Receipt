using Split_Receipt.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Models
{
    /// <summary>
    /// Class <c>User_Group</c> is a model saved in DB, for relation on Group and Checkout tables.
    /// Referencing to specific <f>UserId</f> or/and <f>GroupId</f> we can achievie full information about
    /// which user belongs to which group.
    /// </summary>
    public class User_Group
    {
        public User_Group()
        {
        }

        public User_Group(int groupId, string userId) 
        {
            GroupId = groupId;
            UserId = userId; 
        }
    
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int GroupId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }

        public override string? ToString()
        {
            string userEmail = User != null ? User.Email : "null";
            return "Id: " + Id + ", UserId: " + UserId + ", GroupId: " + GroupId + ", User: " + userEmail;
        }
    }
}
