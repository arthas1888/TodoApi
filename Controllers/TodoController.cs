using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="TodoController"/> class.
    /// This constructor takes a <see cref="ApplicationDbContext"/> context to manage todos.
    /// </summary>
    /// <param name="dbContext">The service context for managing todos.</param>
    public TodoController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET: api/Todo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
    {
        return await _dbContext.Todos.ToListAsync();
    }

    // GET: api/Todo/complete
    [HttpGet("[action]")]
    public async Task<ActionResult<IEnumerable<Todo>>> Complete()
    {
        return await _dbContext.Todos.Where(x => x.IsComplete).ToListAsync();
    }

    // GET: api/Todo/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetTodo(long id)
    {
        var todo = await _dbContext.Todos.FirstOrDefaultAsync(x => x.Id == id);

        if (todo == null)
        {
            return NotFound();
        }

        return todo;
    }

    // POST: api/Todo
    [HttpPost]
    public async Task<ActionResult<Todo>> PostTodo(Todo todo)
    {
        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync(); // commit the changes to the database
        return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
    }


    // PUT: api/Todo/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodo(long id, Todo todo)
    {
        if (id != todo.Id)
        {
            return BadRequest();
        }
        _dbContext.Entry(todo).State = EntityState.Modified;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // PUT: api/Todo/Update/5
    [HttpPut("[action]/{id}")]
    public async Task<IActionResult> Update(long id, Todo todo)
    {
        var entity = await _dbContext.Todos.FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }
        entity.Name = todo.Name;
        entity.IsComplete = todo.IsComplete;
        await _dbContext.SaveChangesAsync();
        return Ok(entity);
    }


    // DELETE: api/Todo/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(long id)
    {
        var todo = await _dbContext.Todos.FindAsync(id);
        if (todo == null)
        {
            return NotFound();
        }

        _dbContext.Todos.Remove(todo);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    private bool TodoExists(long id)
    {
        return _dbContext.Todos.Any(e => e.Id == id);
    }
}

