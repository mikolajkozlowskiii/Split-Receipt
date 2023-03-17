using Split_Receipt.Models;

namespace Split_Receipt.Payload
{
    public class UserGroupRequest
    {
        public UserGroupRequest(){ }
        public UserGroupRequest(string name, ICollection<string> emails)
        {
            GroupName = name;
            this.emails = emails;
        }

        public string GroupName { get; set; }
        public ICollection<String> emails { get; set; }

    }
}
