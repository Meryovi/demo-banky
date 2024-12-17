using Banky.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBankyOpenApi();
builder.Services.AddBankyModules(builder.Configuration);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
    app.UseBankyOpenApi();

app.MapBankyApiRoutes();
app.MapFallbackToFile("/index.html");

await app.ConfigureDatabase();

app.Run();
