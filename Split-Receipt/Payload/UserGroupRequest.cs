using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Payload
{
    /// <summary>
    ///  The <c>UserGroupRequest</c> class is a DTO (Data Transfer Object)
    ///  that represents the information sent between the client view and
    ///  the database when creating a new group with assigned users.
    ///  It contains properties for the group name and a list of emails
    ///  representing the group members. The <c>Required</c> attribute
    ///  ensures that these properties are not null or empty, while the
    ///  <c>MinLength</c> and <c>MaxLength</c> attributes provide
    ///  constraints on the length of the group name.
    /// </summary>
    public class UserGroupRequest
    {
        public UserGroupRequest(){ }
        public UserGroupRequest(string name, List<String> emails)
        {
            GroupName = name;
            this.Emails = emails;
        }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string GroupName { get; set; }

        [Required]
        public List<String> Emails { get; set; }
    }
}
