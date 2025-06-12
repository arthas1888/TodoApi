using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoApi.Models;



/// <summary>
/// Represents a blog post with an ID, title, content, and a flag indicating if it is published.
/// </summary>
public class ProductViewModel
{
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public string? Category { get; set; }


}