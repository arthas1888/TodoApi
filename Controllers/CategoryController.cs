using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

/// <summary>
/// Initializes a new instance of the <see cref="CategoryController"/> class.
/// This constructor takes a <see cref="ApplicationDbContext"/> context to manage Categorys.
/// </summary>
/// <param name="dbContext">The service context for managing Categorys.</param>
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class CategoryController(IGenericCrud<Category> service) : ControllerBase
{
    private readonly IGenericCrud<Category> _service = service;

    // GET: api/Category
    [HttpGet]
    [CustomAuthorize("category.view", Roles: "Admin")]
    public async Task<IEnumerable<Category>> GetCategories() => await _service.GetAllAsync();

    // GET: api/Category/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategory(long id)
    {
        var entity = await _service.GetByIdAsync((int)id);
        if (entity == null) return NotFound(); // 404 Not Found
        return entity; // 200 OK
    }

    // POST: api/Category
    [HttpPost]
    [Authorize(Policy = "categoryAdd")] // Only Admin and Manager can create
    public async Task<ActionResult<Category>> PostCategory(Category model)
    {
        var entity = await _service.CreateAsync(model);
        return StatusCode(201, entity); // 201 Created
        // return CreatedAtAction("GetCategory", new { id = model.Id }, model);
    }


    // PUT: api/Category/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category model)
    {
        var entity = await _service.UpdateAsync(id, model);
        if (entity == null) return NotFound();
        return Ok(entity);
    }


    // DELETE: api/Category/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        var entity = await _service.DeleteAsync((int)id);
        if (entity == null) return NotFound();
        return Ok();
    }
}

