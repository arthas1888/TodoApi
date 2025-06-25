using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TodoApi.Services;

public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    public CustomAuthorizeAttribute(string claimValues, string Roles = "")
        : base(typeof(ClaimRequirementFilter))
    {
        Arguments = [claimValues, Roles];
    }
}

public class ClaimRequirementFilter(string claimValues, string Roles, ApplicationDbContext context, RoleClaimService roleClaimService) : IAuthorizationFilter
{
    private string[] Roles = [.. Roles.Split(',')
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrEmpty(x))];
        
    private string[] ClaimValues = [.. claimValues.Split(',')
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrEmpty(x))];

    private readonly ApplicationDbContext dbContext = context;
    private readonly RoleClaimService _roleClaimService = roleClaimService;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (ClaimValues.Length == 0)
        {
            return;
        }

        // esto se debe reemplazar por imemorycache https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-9.0
        var role = user.Claims.FirstOrDefault(c => c.Type == Claims.Role)?.Value;
        if (!_roleClaimService.RoleExists(role))
        {
            var claims = dbContext.Roles
                .Include(x => x.RoleClaims)
                .Where(x => x.Name == role)
                .SelectMany(x => x.RoleClaims)
                .AsNoTracking()
                .ToList();
            _roleClaimService.AddRoleClaims(role, claims.Select(rc => rc.ClaimValue).ToArray() ?? []);
        }

        var roleClaims = _roleClaimService.GetRoleClaims(role) ?? [];
        var hasRequiredClaim = roleClaims.Any(rc => ClaimValues.Contains(rc));

        if (!hasRequiredClaim)
        {
            // context.Result = new ForbidResult();
            context.Result = new StatusCodeResult(403); // Forbidden
            return;
        }

    }
}