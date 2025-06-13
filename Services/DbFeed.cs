
using TodoApi.Data;

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

        var passwordHasher = services.GetRequiredService<Microsoft.AspNetCore.Identity.IPasswordHasher<Models.User>>();
        var user = new Models.User
        {
            Name = "John",
            LastName = "Doe",
            FullName = "John Doe",
            Email = "superuser@mail.com"
        };
        user.Password = passwordHasher.HashPassword(user, "12345678");
        _logger.LogInformation("Encrypted password {Pass}.", user.Password);

        // Verify the hashed password matches "12345678"
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, "12345678");
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

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("DbFeed service is stopping.");

    }
}