using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoApi.Models;

public class User : BaseGuidModel
{

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
    public required string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? LastName { get; set; }

    [StringLength(255)]
    public string? FullName { get; set; }

    [StringLength(255)]
    public required string Email { get; set; }

    [StringLength(255)]
    public string? Password { get; set; }

    public Guid? RoleId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }
}