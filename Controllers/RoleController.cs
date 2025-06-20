using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Models.ViewModels;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "SuperUser", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class RoleController(
    RoleManager<ApplicationRole> roleManager) : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private static bool _databaseChecked;


    //
    // POST: /api/Role
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] RoleViewModel model)
    {
        var role = new ApplicationRole
        {
            Name = model.Name,
        };
        var res = await _roleManager.CreateAsync(role);
        if (res.Succeeded)
        {
            // Add claims if provided
            if (model.Claims != null)
            {
                foreach (var claim in model.Claims)
                {
                    await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("permission", claim));
                }
            }
        }

        // If we got this far, something failed.
        return Ok(new
        {
            Success = res.Succeeded,
            Errors = res.Errors,
            Role = role
        });
    }

    #region Helpers

    // The following code creates the database and schema if they don't exist.
    // This is a temporary workaround since deploying database through EF migrations is
    // not yet supported in this release.
    // Please see this http://go.microsoft.com/fwlink/?LinkID=615859 for more information on how to do deploy the database
    // when publishing your application.
    private static void EnsureDatabaseCreated(ApplicationDbContext context)
    {
        if (!_databaseChecked)
        {
            _databaseChecked = true;
            context.Database.EnsureCreated();
        }
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    #endregion
}