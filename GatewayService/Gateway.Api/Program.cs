var builder = WebApplication.CreateBuilder(args);

// Cargar configuración de YARP desde appsettings.json
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Redirect("/health"));

app.MapReverseProxy();

app.Run();
