namespace Split_Receipt.Payload
{
    public class UserGroupResponse
    {
        public UserGroupResponse()
        {
        }

        public UserGroupResponse(string groupName, string email)
        {
            Id = 0;
            GroupName = groupName;
            Email = email;
        }

        public UserGroupResponse(int id, string groupName, string email)
        {
            Id = id;
            GroupName = groupName;
            Email = email;
        }

        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Email { get; set; }

        public override string? ToString()
        {
            return "Id of group: "+ Id +"Name of group: " + GroupName +  ", member: " + Email;
        }
    }
}
