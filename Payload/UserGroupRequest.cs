using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Payload
{
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
