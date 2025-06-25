using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TodoApi.Midleware;

public class UserTokenMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<UserTokenMiddleware> _logger, ApplicationDbContext dbContext)
    {
        // capturar el role del usuario autenticado
        var user = context.User;
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var role = user.Claims.FirstOrDefault(c => c.Type == Claims.Role)?.Value;
            var roleClaims = await dbContext.Roles
                .Include(x => x.RoleClaims)
                .Where(x => x.Name == role)
                .SelectMany(x => x.RoleClaims)
                .AsNoTracking()
                .ToListAsync();
            _logger.LogInformation("User ID: {UserId}, Role: {Role}", userId, role);

            roleClaims.ForEach(rc =>
            {
                _logger.LogInformation("Role Claim: {ClaimType} - {ClaimValue}", rc.ClaimType, rc.ClaimValue);

                // Add role claims to the user claims
                if (!context.User.HasClaim(c => c.Type == rc.ClaimType && c.Value == rc.ClaimValue))
                {
                    var identity = (ClaimsIdentity)context.User.Identity;
                    identity.AddClaim(new Claim(rc.ClaimType, rc.ClaimValue));
                    _logger.LogInformation("Added claim: {ClaimType} - {ClaimValue}", rc.ClaimType, rc.ClaimValue);
                    context.User.AddIdentity((ClaimsIdentity)identity);
                }
            });
        }


        // Call the next middleware in the pipeline
        await _next(context);
    }
}