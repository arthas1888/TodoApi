using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Data;
using TodoApi.Models.ViewModels;

namespace TodoApi.Managers;


public class LoginManager(ApplicationDbContext dbContext, ILogger<LoginManager> logger, IConfiguration configuration)
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ILogger<LoginManager> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    public async Task<object?> ValidateUserAsync(LoginViewModel model)
    {
        var user = await _dbContext.users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
        {
            return null; // User not found
        }
        _logger.LogInformation("User {Email} found in the database.", user.Email);
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<Models.User>();
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

        var passwordSuccess = verificationResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;

        if (!passwordSuccess)
        {
            _logger.LogWarning("Password verification failed for user {Email}.", user.Email);
            return null; // Invalid password
        }
        // claims identity for jwt creation
        var handler = new JwtSecurityTokenHandler();
        var utcNow = DateTime.UtcNow;
        var key = Encoding.ASCII.GetBytes(_configuration["Authentication:Jwt:SigningKey"]!); // Use a secure key in production
        var signinKey = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        // Create a claims identity with the user's ID
        // claims son los atributos del usuario que se van a incluir en el token
        // en este caso el sub es el id del usuario, pero se pueden incluir otros atributos como email, roles, etc.
        var subject = new ClaimsIdentity([new Claim("sub", user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.FullName?.ToString()?? "")]);

        // crea el token de acuerdo a los datos del usuario o claims
        var token = handler.CreateJwtSecurityToken(_configuration["Authentication:Jwt:Issuer"], _configuration["Authentication:Jwt:Audience"], subject,
                utcNow, utcNow.AddDays(7), utcNow, signinKey, null);
        string accessToken = handler.WriteToken(token);
        // _logger.LogInformation("Generated JWT token for user accessToken {accessToken}.", accessToken);

        return new ResAccessToken
        {
            AccessToken = accessToken,
            ExpiresAt = token.ValidTo
        };
    }
}
// This class handles user login validation by checking the provided email and password against the database.