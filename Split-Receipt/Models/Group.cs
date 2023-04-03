using System.ComponentModel.DataAnnotations;

namespace Split_Receipt.Models
{
    /// <summary>
    /// Class <c>Group</c> is a model for groups saved in DB. 
    /// The group class is used mainly because of the possibility of creating
    /// different names for the checkouts of the members.
    /// The <see cref="Name"/> property is decorated with [Required], [MinLength(2)], [MaxLength(20)] attributes 
    /// to ensure that the group name is not null or empty, and has a minimum and maximum length limit.
    /// </summary>
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