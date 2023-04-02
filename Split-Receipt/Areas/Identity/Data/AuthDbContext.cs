using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Split_Receipt.Areas.Identity.Data;
using Split_Receipt.Models;

namespace Split_Receipt.Data;

/// <summary>
///  Class <c>AuthDbContext</c> is used to manage authentication-related data using Entity Framework.
///  It inherits from IdentityDbContext<ApplicationUser>, indicating that it provides additional
///  functionality to the base Identity framework. The class includes DbSets for Group, User_Group,
///  and Checkout entities, which are used to define the relationships between users and groups,
///  as well as checkouts made by users.
/// </summary>
public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    public DbSet<Group> Groups { get; set; }
    public DbSet<User_Group> User_Groups { get; set; }
    public DbSet<Checkout> Checkouts { get; set; }
}
