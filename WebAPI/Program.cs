using DAL;
using Microsoft.EntityFrameworkCore;
using WebAPI.Interfaces;
using WebAPI.Repositories;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Swagger for API documentation - TOUJOURS activé
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PrintSystem WebAPI", Version = "v1" });
});

// Configure Entity Framework with connection string from Azure
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"[Program] Using connection string: {connectionString?.Substring(0, 50)}...");

builder.Services.AddDbContext<PrintSystemContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(60); // Timeout pour Azure
        sqlOptions.EnableRetryOnFailure(); // Retry automatique
    });
});

// Register dependency injection services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuotaService, QuotaService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Initialize database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PrintSystemContext>();

        // Pour Azure SQL : utiliser EnsureCreated pour l'instant (plus simple)
        context.Database.EnsureCreated();
        Console.WriteLine("[Program] Database initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Program] Error with database: {ex.Message}");
        Console.WriteLine($"[Program] Stack trace: {ex.StackTrace}");
    }
}

// Configure the HTTP request pipeline - TOUJOURS activer Swagger
Console.WriteLine("[Program] Configuring middleware pipeline...");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrintSystem WebAPI v1");
    c.RoutePrefix = "swagger"; // Accessible via /swagger
});

// Redirection HTTPS
app.UseHttpsRedirection();

// Authorization
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Route par défaut pour tester
app.MapGet("/", () => new {
    message = "PrintSystem WebAPI is running!",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
});

Console.WriteLine("[Program] Print Payment System API is starting...");
Console.WriteLine($"[Program] Environment: {app.Environment.EnvironmentName}");

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"[Program] Failed to start application: {ex.Message}");
    Console.WriteLine($"[Program] Stack trace: {ex.StackTrace}");
    throw;
}