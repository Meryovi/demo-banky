using Microsoft.EntityFrameworkCore.Design;

namespace Banky.Modules.Accounts.Infrastructure;

internal class BankyAccountsDbContextFactory : IDesignTimeDbContextFactory<BankyAccountsDbContext>
{
    public BankyAccountsDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BankyAccountsDbContext>()
            .UseNpgsql("Host=localhost;Database=banky;Username=postgres;Password=postgres")
            .Options;

        return new BankyAccountsDbContext(options);
    }
}
