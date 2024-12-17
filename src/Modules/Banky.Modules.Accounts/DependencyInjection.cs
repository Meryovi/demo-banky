using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banky.Modules.Accounts;

public static class DependencyInjection
{
    public static void AddAccountsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("Accounts");

        if (!string.IsNullOrEmpty(connection)) // Don't do this in production :)
        {
            services.AddDbContext<BankyAccountsDbContext>(options => options.UseNpgsql(connection));
            services.AddTransient<IAccountsRepository, AccountsRepository>();
        }
        else
        {
            services.AddSingleton<IAccountsRepository, MemoryAccountsRepository>();
        }

        services.AddSingleton(TimeProvider.System);
    }

    public static async Task ConfigureAccountsDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetService<BankyAccountsDbContext>();

        if (context is null)
            return;

        await context.Database.MigrateAsync();
    }
}
