using Microsoft.Build.Framework;

namespace Split_Receipt.Models
{
    public class Group
    {
        public Group()
        {
        }

        public Group(int id, string name, ICollection<User_Group> user_Groups)
        {
            Id = id;
            Name = name;
            //User_Groups = user_Groups;
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        //relacja z User_Group
        public virtual ICollection<User_Group> User_Groups { get; set; }

        public override string? ToString()
        {
            return Id + ", " + Name; 
        }
    }
}
