using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoApi.Models;
public class Todo
{
    public long Id { get; set; }

    [StringLength(120)]
    public string? Description { get; set; }

    [Required]
    public required string Name { get; set; }

    [JsonPropertyName("is_complete")]
    public bool IsComplete { get; set; }
}
