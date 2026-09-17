
using Provider.Api.Middlewares;
using Provider.Application.DependencyInjection;
using Provider.Infrastructure.DependencyInjection;
using Scalar.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Provider.Infrastructure.Configuration;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.File(
        Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Registrar servicios de la capa Application (servicios de negocio)
builder.Services.AddApplicationServices();

// Register infrastructure services (repositories, etc.) via Scrutor
builder.Services.AddInfrastructureServices();

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? new JwtSettings();

string ResolveConfig(string? current, params string[] keys)
{
    if (!string.IsNullOrWhiteSpace(current))
        return current!;

    foreach (var key in keys)
    {
        // First try configuration (supports appsettings and other providers)
        var cfg = builder.Configuration[key];
        if (!string.IsNullOrWhiteSpace(cfg)) return cfg;

        // Then try environment variables (both hierarchical and flat conventions)
        var env = Environment.GetEnvironmentVariable(key)
                  ?? Environment.GetEnvironmentVariable(key.Replace(':', '_'))
                  ?? Environment.GetEnvironmentVariable(key.ToUpperInvariant().Replace(':', '_'));

        if (!string.IsNullOrWhiteSpace(env)) return env;
    }

    return string.Empty;
}

// Validate connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

jwtSettings.Key = ResolveConfig(jwtSettings.Key, "Jwt:Key", "Jwt__Key", "JWT_KEY");
jwtSettings.Issuer = ResolveConfig(jwtSettings.Issuer, "Jwt:Issuer", "Jwt__Issuer", "JWT_ISSUER");
jwtSettings.Audience = ResolveConfig(jwtSettings.Audience, "Jwt:Audience", "Jwt__Audience", "JWT_AUDIENCE");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    throw new InvalidOperationException("JWT Key is not configured.");

if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
    throw new InvalidOperationException("JWT Issuer is not configured.");

if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
    throw new InvalidOperationException("JWT Audience is not configured.");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)
            ),

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(pattern: "/scalar/v1/openapi.json");
    app.MapScalarApiReference(endpointPrefix: "/docs",
        configureOptions: options =>
        {
            options.Title = "Provider Service API";
            options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
            options.OpenApiRoutePattern = "/scalar/v1/openapi.json";
        }
    );
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseCors("AllowAngularDev");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
