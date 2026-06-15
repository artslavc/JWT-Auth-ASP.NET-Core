using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using WebApi.Configurations;
using WebApi.Data;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddCustomRateLimiting();
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Host.UseSerilog((context, config) =>
{
    LoggerConfiguration loggerConfiguration = config.ReadFrom.Configuration(context.Configuration)
          .WriteTo.Console()
          .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
          .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
          .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning) 
          .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
});

var app = builder.Build();

app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseRateLimiter();
app.MapControllers();

app.Run();