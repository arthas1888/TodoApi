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
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext applicationDbContext, IMapper mapper) : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
    private readonly IMapper _mapper = mapper;
    private static bool _databaseChecked;


    [HttpGet]
    public IActionResult GetUserClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        // await Response.WriteAsJsonAsync(claims);

        return Ok(claims);
    }

    // POST: /api/Account/userinfo
    [HttpGet("[action]")]
    public async Task<IActionResult> UserInfo()
    {
        // HttpContext.User.Identity.IsAuthenticated;
        var user = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);
        if (user == null)
        {
            return NotFound();
        }

        var userInfo = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.LastName,
            user.Address
        };

        return Ok(_mapper.Map<UserDto>(user));
        // return Ok(userInfo);
    }

    //
    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
    {
        EnsureDatabaseCreated(_applicationDbContext);
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return Ok();
            }
            AddErrors(result);
        }

        // If we got this far, something failed.
        return BadRequest(ModelState);
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