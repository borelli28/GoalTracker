using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using App.Services;
using App.Data;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    WebRootPath = null
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// GoalService Registration
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IProgressService, ProgressService>();

// CORS Config
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEndApp",
        builder => builder.WithOrigins("http://localhost:5173")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowFrontEndApp");

app.UseAuthorization();

app.MapGet("/", (HttpContext context) => 
{
    var port = context.Connection.LocalPort;
    return $"This server is running at http://localhost:{port}";
});
app.MapControllers();

app.Run();
