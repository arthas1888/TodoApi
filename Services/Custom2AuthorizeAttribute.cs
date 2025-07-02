using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoApi.Services;

public class Custom2AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string? Claims { get; set; }
    public string? Roles { get; set; }
    public bool RequireAllClaims { get; set; } = false; // false = OR, true = AND

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Verificar si el usuario está autenticado
        if (!user.Identity?.IsAuthenticated == true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Verificar roles si se especificaron
        if (!string.IsNullOrEmpty(Roles))
        {
            var requiredRoles = Roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim());

            var hasRole = requiredRoles.Any(role => user.IsInRole(role));
            if (!hasRole)
            {
                context.Result = new ForbidResult();
                return;
            }
        }

        // Verificar claims si se especificaron
        if (!string.IsNullOrEmpty(Claims))
        {
            var requiredClaims = Claims.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(c => c.Trim());

            bool hasValidClaims;

            if (RequireAllClaims)
            {
                // Requiere TODOS los claims (AND)
                hasValidClaims = requiredClaims.All(claim => HasClaim(user, claim));
            }
            else
            {
                // Requiere AL MENOS UNO de los claims (OR)
                hasValidClaims = requiredClaims.Any(claim => HasClaim(user, claim));
            }

            if (!hasValidClaims)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

    private static bool HasClaim(System.Security.Claims.ClaimsPrincipal user, string claim)
    {
        // Soporte para formato "type:value" o solo "type"
        var parts = claim.Split(':', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            // Formato "permission:product.view"
            return user.HasClaim(parts[0].Trim(), parts[1].Trim());
        }
        else
        {
            // Formato "product.view" (asume que es un claim de tipo "permission")
            return user.HasClaim("permission", claim);
        }
    }
}