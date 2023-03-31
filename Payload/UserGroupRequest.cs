using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Payload
{
    /// <summary>
    ///  Class <c>UserGroupRequest</c> is a DTO's object
    ///  beetween view's request of creating group with assigned users to this group
    ///  and saved User_Group object instance in DB.
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
        //[ExistingEmails]
        public List<String> Emails { get; set; }

    }
}
