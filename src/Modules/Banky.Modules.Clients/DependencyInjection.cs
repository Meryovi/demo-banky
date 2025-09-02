using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banky.Modules.Clients;

public static class DependencyInjection
{
    public static void AddClientsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("Clients");

        if (!string.IsNullOrEmpty(connection)) // Don't do this in production :)
        {
            services.AddDbContext<BankyClientsDbContext>(options => options.UseNpgsql(connection));
            services.AddTransient<IClientsRepository, ClientsRepository>();
        }
        else
        {
            services.AddSingleton<IClientsRepository, MemoryClientsRepository>();
        }

        services.AddSingleton(TimeProvider.System);
    }

    public static async Task ConfigureClientsDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetService<BankyClientsDbContext>();

        if (context is null)
            return;

        await context.Database.MigrateAsync();
    }
}
