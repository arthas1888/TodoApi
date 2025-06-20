using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models.ViewModels;

public class RoleViewModel
{
    [Required]
    public required string Name { get; set; }
    public string? Description { get; set; }

    public string[]? Claims { get; set; }

    public RoleViewModel(string name, string description)
    {
        Name = name;
        Description = description;
    }
}