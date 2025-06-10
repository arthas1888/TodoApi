using System.Collections.Concurrent;
using TodoApi.Models;

namespace TodoApi.Services;

public class TodoService
{
    private readonly ConcurrentBag<Todo> todos = new();

    public IEnumerable<Todo> GetTodos()
    {
        return todos;
    }

    public Todo? GetTodo(long id)
    {
        return todos.FirstOrDefault(t => t.Id == id);
    }

    public void AddTodo(Todo todo)
    {
        todos.Add(todo);
    }

    public Todo? UpdateTodo(long id, Todo todo)
    {
        var existingTodo = GetTodo(id);
        if (existingTodo != null)
        {
            existingTodo.Name = todo.Name;
            existingTodo.IsComplete = todo.IsComplete;
        }
        return existingTodo;
    }

    public void DeleteTodo(long id)
    {
        var todo = GetTodo(id);
        if (todo != null)
        {
            // todos.Reverse(todo);
        }
    }
}