using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string> // DbContext
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
    public DbSet<Role> role { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity table names to use snake_case
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("asp_net_users");
            
            // Configure indexes with snake_case names
            b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("user_name_index").IsUnique();
            b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("email_index");
            
            // Each User can have many UserClaims
            b.HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            b.HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            b.HasMany(e => e.Tokens)
                .WithOne()
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<ApplicationRole>(b =>
        {
            b.ToTable("asp_net_roles");
            b.HasIndex(u => u.NormalizedName).HasDatabaseName("normalized_name_index");
        });

        // Configure Identity-related tables to use snake_case
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>(b =>
        {
            b.ToTable("asp_net_user_claims");
            b.HasIndex(uc => uc.UserId).HasDatabaseName("ix_application_user_claims_user_id");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(b =>
        {
            b.ToTable("asp_net_user_logins");
            b.HasIndex(ul => ul.UserId).HasDatabaseName("ix_application_user_logins_user_id");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(b =>
        {
            b.ToTable("asp_net_user_tokens");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>(b =>
        {
            b.ToTable("asp_net_user_roles");
            b.HasIndex(ur => ur.RoleId).HasDatabaseName("ix_application_user_roles_role_id");
        });

        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(b =>
        {
            b.ToTable("asp_net_role_claims");
            b.HasIndex(rc => rc.RoleId).HasDatabaseName("ix_application_role_claims_role_id");
        });

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
