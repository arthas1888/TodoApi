using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Models;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationRole : IdentityRole
{

    [JsonIgnore]
    public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; } = [];

    [JsonIgnore]
    public virtual ICollection<IdentityRoleClaim<string>>? RoleClaims { get; set; } = [];
}