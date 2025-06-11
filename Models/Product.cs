using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoApi.Models;



/// <summary>
/// Represents a blog post with an ID, title, content, and a flag indicating if it is published.
/// </summary>
public class Product : BaseModel
{
    [Required]
    public required string Name { get; set; }

    [NotMapped] // This property is not mapped to the database, but can be used for display purposes.
    [JsonPropertyName("category")]
    public string? CategoryName { get => Category?.Name; }

    [JsonIgnore]
    public int? CategoryId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

}

public class Category : BaseModel
{

    [Required]
    public required string Name { get; set; }
    public List<Product> Products { get; set; } = [];
}

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasOne(pt => pt.Category)
            .WithMany(p => p.Products)
            .HasForeignKey(pt => pt.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(e => e.CreateDate)
            .HasDefaultValueSql("now()");

    }
}