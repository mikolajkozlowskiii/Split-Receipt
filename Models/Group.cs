using System.ComponentModel.DataAnnotations;

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
        }

        public int Id { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(20)]
        public string Name { get; set; }

        public virtual ICollection<User_Group> User_Groups { get; set; }

        public override string? ToString()
        {
            return Id + ", " + Name;
        }
    }
}