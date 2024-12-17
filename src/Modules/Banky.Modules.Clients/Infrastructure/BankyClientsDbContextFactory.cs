using Microsoft.EntityFrameworkCore.Design;

namespace Banky.Modules.Clients.Infrastructure;

internal class BankyClientsDbContextFactory : IDesignTimeDbContextFactory<BankyClientsDbContext>
{
    public BankyClientsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BankyClientsDbContext>()
            .UseNpgsql("Host=localhost;Database=banky;Username=postgres;Password=postgres")
            .Options;

        return new BankyClientsDbContext(options);
    }
}
