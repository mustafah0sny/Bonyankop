using BonyankopAPI.Extensions;
using BonyankopAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure application services
builder.Services.AddApplicationServices();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsConfiguration();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Database seeding disabled for production - run migrations manually if needed
// To seed: dotnet ef database update

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bonyankop API v1");
    options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    options.DocumentTitle = "Bonyankop API Documentation";
});

// Only use HTTPS redirection in production with proper SSL
if (app.Environment.IsProduction() && app.Configuration["UseHttpsRedirection"] == "true")
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();
