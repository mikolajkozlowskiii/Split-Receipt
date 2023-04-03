namespace Split_Receipt.Payload
{
    /// <summary>
    ///  The UserGroupResponse class is a DTO (Data Transfer Object)
    ///  used for communication between the saved User_Group object
    ///  instance in the database and the view that receives the response.
    ///  It contains properties for Id, GroupName, and Email. The class has
    ///  three constructors that accept parameters to set the values of
    ///  these properties. Additionally, the class has an overridden ToString()
    ///  method that returns a string containing the group ID, group name, and member email address.
    /// </summary>
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
