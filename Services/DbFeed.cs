
using OpenIddict.Abstractions;
using TodoApi.Data;
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

        await SeedCredentials(dbContext);
        return;
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

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DbFeed service is stopping.");

    }
}