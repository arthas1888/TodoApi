
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using TodoApi.Data;
using TodoApi.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TodoApi.Services;

public class DbFeed(IServiceProvider services, ILogger<DbFeed> logger) : IHostedService
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DbFeed service is starting.");
        using var serviceScope = _services.CreateScope();
        var services = serviceScope.ServiceProvider;

        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
        _logger.LogInformation("Database created or already exists.");

        // await SeedCredentials(dbContext);

        // await CreateUsers();
        await SeedCategories(dbContext);
        await SeedProducts(dbContext);
        return;
        // return;
        var adminRole = dbContext.role.FirstOrDefault(r => r.Name == "Admin");

        adminRole ??= dbContext.role.Add(new Models.Role
        {
            Name = "Admin"
        }).Entity;

        var passwordHasher = services.GetRequiredService<Microsoft.AspNetCore.Identity.IPasswordHasher<Models.User>>();
        var user = new Models.User
        {
            Name = "John",
            LastName = "Doe",
            FullName = "John Doe",
            Email = "admin2@mail.com",
            RoleId = adminRole?.Id
        };
        user.Password = passwordHasher.HashPassword(user, "Abc123");
        _logger.LogInformation("Encrypted password {Pass}.", user.Password);

        // Verify the hashed password matches "12345678"
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, "Abc123");
        if (verificationResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
        {
            _logger.LogInformation("Password verification succeeded for {Email}.", user.Email);
        }
        else
        {
            _logger.LogWarning("Password verification failed for {Email}.", user.Email);
        }

        _logger.LogInformation("Adding user {Email} to the database.", user.Email);

        dbContext.users.Add(user);
        await dbContext.SaveChangesAsync();

    }

    private async Task CreateUsers()
    {
        using var serviceScope = _services.CreateScope();
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var role = new ApplicationRole { Name = "Admin" };
        var res = await roleManager.CreateAsync(role);

        var user = new ApplicationUser { UserName = "admin4@mail.com", Email = "admin3@mail.com", LastName = "John", Address = "123 Main St" };
        var result = await userManager.CreateAsync(user, "Abc123*");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role.Name);
            _logger.LogInformation("User {Email} created successfully.", user.Email);
        }
        else
        {
            _logger.LogError("Failed to create user {Email}: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        role = new ApplicationRole { Name = "SuperUser" };
        res = await roleManager.CreateAsync(role);

        user = new ApplicationUser { UserName = "superuser@mail.com", Email = "superuser@mail.com", LastName = "SuperUser", Address = "123 Main St" };
        result = await userManager.CreateAsync(user, "Abc123*");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, role.Name);
            _logger.LogInformation("User {Email} created successfully.", user.Email);
        }
        else
        {
            _logger.LogError("Failed to create user {Email}: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }


    public async Task SeedCredentials(ApplicationDbContext dbContext)
    {
        using var serviceScope = _services.CreateScope();
        var manager = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync("nutresa") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "nutresa",
                ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                DisplayName = "nutresa client application",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials
                }
            });
        }
        if (await manager.FindByClientIdAsync("alpina") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "alpina",
                ClientSecret = "27b7eccf-72fa-4575-9b1e-51cd176d2437",
                DisplayName = "alpina client application",
                Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials
                }
            });
        }
    }

    public async Task SeedCategories(ApplicationDbContext dbContext)
    {
        dbContext.category.AddRange(
            new Category { Name = "Beverages" },
            new Category { Name = "Condiments" },
            new Category { Name = "Confections" },
            new Category { Name = "Dairy Products" },
            new Category { Name = "Grains/Cereals" }
        );
        await dbContext.SaveChangesAsync();

    }


    public async Task SeedProducts(ApplicationDbContext dbContext)
    {
        var categories = await dbContext.category.ToListAsync();
        var random = new Random();
        var products = new List<Product>();
        for (int i = 1; i <= 100; i++)
        {
            var category = categories[random.Next(categories.Count)];
            products.Add(new Product
            {
                Name = $"Product {i}",
                CategoryId = category.Id
            });
        }
        dbContext.product.AddRange(products);
        dbContext.product.AddRange(
            new Product { Name = "Chai", CategoryId = categories.FirstOrDefault(c => c.Name == "Beverages")?.Id },
            new Product { Name = "Chang", CategoryId = categories.FirstOrDefault(c => c.Name == "Beverages")?.Id },
            new Product { Name = "Aniseed Syrup", CategoryId = categories.FirstOrDefault(c => c.Name == "Condiments")?.Id },
            new Product { Name = "Chef Anton's Cajun Seasoning", CategoryId = categories.FirstOrDefault(c => c.Name == "Condiments")?.Id }
        );
        await dbContext.SaveChangesAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DbFeed service is stopping.");

    }
}