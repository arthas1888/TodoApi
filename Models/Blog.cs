using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TodoApi.Models;

public enum PostStatus
{
    Draft,
    Published,
    Archived
}

/// <summary>
/// Represents a blog post with an ID, title, content, and a flag indicating if it is published.
/// </summary>
public class Blog : BaseModel
{
    public string? Url { get; set; }

    public List<Post> Posts { get; set; } = [];
}

public class Post : BaseModel
{
    public PostStatus Status { get; set; }

    [Required]
    public required string Title { get; set; }

    public string? Content { get; set; }


    public int BlogId { get; set; }
    [JsonIgnore]
    [ForeignKey(nameof(BlogId))]
    public Blog? Blog { get; set; }
}

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder
            .Property(e => e.CreateDate)
            .HasDefaultValueSql("now()");

        builder
            .HasOne(pt => pt.Blog)
            .WithMany(p => p.Posts)
            .HasForeignKey(pt => pt.BlogId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}