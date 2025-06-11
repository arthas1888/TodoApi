using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

/// <summary>
/// Initializes a new instance of the <see cref="ProductController"/> class.
/// This constructor takes a <see cref="ApplicationDbContext"/> context to manage Products.
/// </summary>
/// <param name="dbContext">The service context for managing Products.</param>
[Route("api/[controller]")]
[ApiController]
public class ProductController(IGenericCrud<Product> service) : ControllerBase
{
    private readonly IGenericCrud<Product> _service = service;

    // GET: api/Product
    [HttpGet]
    public async Task<IEnumerable<Product>> GetProducts() => await _service.GetAllAsync();

    // GET: api/Product/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(long id)
    {
        var entity = await _service.GetByIdAsync((int)id);
        if (entity == null) return NotFound(); // 404 Not Found
        return entity; // 200 OK
    }

    // POST: api/Product
    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product model)
    {
        var entity = await _service.CreateAsync(model);
        return StatusCode(201, entity); // 201 Created
        // return CreatedAtAction("GetProduct", new { id = model.Id }, model);
    }


    // PUT: api/Product/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Product model)
    {
        var entity = await _service.UpdateAsync(id, model);
        if (entity == null) return NotFound();
        return Ok(entity);
    }


    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(long id)
    {
        var entity = await _service.DeleteAsync((int)id);
        if (entity == null) return NotFound();
        return Ok();
    }
}

