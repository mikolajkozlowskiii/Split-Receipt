using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Namespace <c>Split_Receipt.Areas.Identity.Data</c> contains classes and other types related to user authentication
/// and authorization in the Split Receipt application.
/// </summary>
namespace Split_Receipt.Areas.Identity.Data;

/// <summary>
/// Class <c>ApplicationUsert</c>  is a model for representing users in a database.
/// It extends the IdentityUser class from the Microsoft.AspNetCore.Identity namespace,
/// which provides built-in support for managing user authentication and authorization in ASP.NET applications.
/// The class includes two additional properties, FirstName and LastName,
/// which are decorated with PersonalData and Column attributes to indicate that they contain personal data
/// and to specify the database column data type. The PersonalData attribute is used to mark properties that contain
/// sensitive user data.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(30)")]
    public string FirstName { get; set; }
    [PersonalData]
    [Column(TypeName = "nvarchar(30)")]
    public string LastName { get; set; }
}

