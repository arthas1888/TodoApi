using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Todo> todos { get; set; } = null!;
    public DbSet<Product> product { get; set; } = null!;
    public DbSet<Category> category { get; set; } = null!;
    public DbSet<Blog> blog { get; set; } = null!;
    public DbSet<Post> post { get; set; } = null!;
    public DbSet<User> users { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Blog>()
            .Property(e => e.CreateDate)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<Category>()
            .Property(e => e.CreateDate)
            .HasDefaultValueSql("now()");

        modelBuilder
            .Entity<User>()
            .Property(e => e.CreateDate)
            .HasDefaultValueSql("now()");

        modelBuilder.Entity<User>()
            .Property(p => p.FullName)
            .HasComputedColumnSql(@"name || ' ' || coalesce(last_name, '')", stored: true);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new PostConfiguration());
    }
}
