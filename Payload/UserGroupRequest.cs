using Microsoft.Build.Framework;
using Split_Receipt.Models;

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
        public string GroupName { get; set; }
        // [ExistingEmails(ErrorMessage = "All emails must be existing users in the system")]
        [Required]
        public List<String> Emails { get; set; }

    }
}
