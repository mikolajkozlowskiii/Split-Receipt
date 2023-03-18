namespace Split_Receipt.Payload
{
    public class UserGroupResponse
    {
        public UserGroupResponse()
        {
        }

        public UserGroupResponse(string groupName, string email)
        {
            GroupName = groupName;
            Email = email;
        }

        public string GroupName { get; set; }
        public string Email { get; set; }

        public override string? ToString()
        {
            return "Name of group: " + GroupName +  ", member: " + Email;
        }
    }
}
