using Split_Receipt.Areas.Identity.Data;

namespace Split_Receipt.Models
{
    public class User_Group
    {
        public User_Group()
        {
        }

        public User_Group(int id, string userId, int groupId, string userEmail)
        {
            Id = id;
            UserId = userId;
            GroupId = groupId;
            UserEmail = userEmail;
            //User = user;
            //Group = group;
        }
        public User_Group(int groupId, string userEmail)
        {
            GroupId = groupId;
            UserEmail = userEmail;
            //User = user;
            //Group = group;
        }

        public int Id { get; set; }

        //klucze obce
        public string UserId { get ; set; }
        public string UserEmail { get; set; }
        public int GroupId { get; set; }

        //nawigacja
        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }

        public override string? ToString()
        {
            return Id + ", " + UserId + ", " + GroupId;
        }
    }
}
