using Microsoft.EntityFrameworkCore;
using TodoApi;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

builder.Services.AddControllers();


builder.Services.AddSingleton<TodoService>();

var app = builder.Build();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.DocumentTitle = "TodoAPI";
    config.Path = "/swagger";
    config.DocumentPath = "/swagger/{documentName}/swagger.json";
    config.DocExpansion = "list";
});

// minimal api
app.MapGet("/", () => "Hello World!");
// app.MapGet("/api/Todo", () => new List<Todo>
// {
//     new Todo { Id = 1, Name = "Learn ASP.NET Core", IsComplete = false },
//     // new Todo { Id = 2, Name = "Build a web API", IsComplete = true }
// });
app.MapCustomApis();

app.Run();


// Singleton -> un servicio que se crea una sola vez y se comparte en toda la aplicación.
// Transient -> un servicio que se crea cada vez que se solicita.
// Scoped -> un servicio que se crea una vez por solicitud HTTP.