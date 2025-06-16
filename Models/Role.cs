using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models;

public class Role : BaseGuidModel
{

    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
    public required string Name { get; set; } = string.Empty;

}