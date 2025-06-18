using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
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
    // Register the entity sets needed by OpenIddict.
    opt.UseOpenIddict();
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

#region  identity
// Register the Identity services.
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
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

builder.Services.Configure<IdentityOptions>(options =>
{
    //     options.ClaimsIdentity.UserNameClaimType = Claims.Name;
    //     options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});

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

#region openIddict

// OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
// (like pruning orphaned authorizations/tokens from the database) at regular intervals.
builder.Services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddOpenIddict()
        // Register the OpenIddict core components.
        .AddCore(options =>
        {
            // Configure OpenIddict to use the Entity Framework Core stores and models.
            // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
            options.UseEntityFrameworkCore()
                    .UseDbContext<ApplicationDbContext>();
            // Enable Quartz.NET integration.
            options.UseQuartz();

        })

        // Register the OpenIddict server components.
        .AddServer(options =>
        {
            // Enable the token endpoint.
            options.SetTokenEndpointUris("connect/token");

            // Enable the client credentials flow.
            options
                // .AllowClientCredentialsFlow()
                .AllowPasswordFlow()
                .AllowRefreshTokenFlow();

            options.AcceptAnonymousClients();

            // Register the signing and encryption credentials.
            options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

            options.DisableAccessTokenEncryption();

            // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
            options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough();

        })

        // Register the OpenIddict validation components.
        .AddValidation(options =>
        {
            // Import the configuration from the local OpenIddict server instance.
            options.UseLocalServer();

            // Register the ASP.NET Core host.
            options.UseAspNetCore();
        });

#endregion

#region background Services
// builder.Services.AddHostedService<DbFeed>();
#endregion

var app = builder.Build();
app.MapControllers();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

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


