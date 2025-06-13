using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApi;
using TodoApi.Data;
using TodoApi.Managers;
using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDbContextPool<ApplicationDbContext>(opt =>
{
    opt
        .UseNpgsql(builder.Configuration.GetConnectionString("DbContext"))
        .UseSnakeCaseNamingConvention();
    if (env.IsDevelopment())
    {
        opt.EnableSensitiveDataLogging();
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "TodoAPI";
    config.Title = "TodoAPI v1";
    config.Version = "v1";
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();

#region Managers
builder.Services.AddScoped<LoginManager>();
#endregion

// Add services to the container.
#region Services
// Singleton -> un servicio que se crea una sola vez y se comparte en toda la aplicación.
// Transient -> un servicio que se crea cada vez que se solicita.
// Scoped -> un servicio que se crea una vez por solicitud HTTP.
builder.Services.AddScoped<IGenericCrud<Category>, CategoryCrudService>();
builder.Services.AddScoped<IGenericCrud<Product>, ProductCrudService>();
builder.Services.AddSingleton<TodoService>();
builder.Services.AddSingleton<PostgresService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();
#endregion

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        // jwtOptions.Authority = "https://www.tiendana.com";
        jwtOptions.Audience = builder.Configuration["Authentication:Jwt:Audience"]!;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:Jwt:SigningKey"]!)),
            ValidIssuer = builder.Configuration["Authentication:Jwt:Issuer"]!,
        };
    });

#region background Services
// builder.Services.AddHostedService<DbFeed>();
#endregion

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


