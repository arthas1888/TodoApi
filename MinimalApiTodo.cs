using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi;

public static class MinimalApiTodo
{
    /// <summary>
    /// Adds endpoints for controller actions to the <see cref="IEndpointRouteBuilder"/> without specifying any routes.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/>.</param>
    /// <returns>An <see cref="ControllerActionEndpointConventionBuilder"/> for endpoints associated with controller actions.</returns>
    public static void MapCustomApis(this WebApplication app)
    {

        app.MapGet("/todoitems", async (ApplicationDbContext db) =>
            await db.todos.ToListAsync());

        app.MapGet("/todoitems/complete", async (ApplicationDbContext db) =>
            await db.todos.Where(t => t.IsComplete).ToListAsync());

        app.MapGet("/todoitems/{id}", async (int id, ApplicationDbContext db) =>
            await db.todos.FindAsync(id)
                is Todo todo
                    ? Results.Ok(todo)
                    : Results.NotFound());

        app.MapPost("/todoitems", async (Todo todo, ApplicationDbContext db) =>
        {
            db.todos.Add(todo);
            await db.SaveChangesAsync();

            return Results.Created($"/todoitems/{todo.Id}", todo);
        });

        app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, ApplicationDbContext db) =>
        {
            var todo = await db.todos.FindAsync(id);

            if (todo is null) return Results.NotFound();

            todo.Name = inputTodo.Name;
            todo.IsComplete = inputTodo.IsComplete;

            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        app.MapDelete("/todoitems/{id}", async (int id, ApplicationDbContext db) =>
        {
            if (await db.todos.FindAsync(id) is Todo todo)
            {
                db.todos.Remove(todo);
                await db.SaveChangesAsync();
                return Results.NoContent();
            }

            return Results.NotFound();
        });
    }
}