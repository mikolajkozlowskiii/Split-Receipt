using Split_Receipt.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Models
{
    /// <summary>
    /// The <c>User_Group</c> class is a model that represents the many-to-many relationship
    /// between the <c>Group</c> and <c>Checkout</c> models in the database.
    /// This class includes a foreign key reference to the <c>UserId</c> and <c>GroupId</c> properties,
    /// which allows us to retrieve information about which user belongs to which group.
    /// The properties are decorated with the Required attribute to ensure that they are not null or empty.
    /// The class also includes a <c>User</c> property of type <c>ApplicationUser</c>
    /// and a <c>Group</c> property of type <c>Group</c>, which represent the user and group entities, respectively.
    /// The class includes a <c>ToString()</c> method that returns a string representation of the <c>User_Group</c> object.
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
