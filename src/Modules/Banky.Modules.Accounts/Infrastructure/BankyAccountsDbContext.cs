namespace Banky.Modules.Accounts.Infrastructure;

internal class BankyAccountsDbContext(DbContextOptions<BankyAccountsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("account");
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClientId).IsRequired();
            entity.Property(e => e.Type).IsRequired().HasConversion<int>();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Balance).IsRequired();
            entity.Property(e => e.IsClosed).IsRequired();
            entity.Property(e => e.CreatedOnUtc).IsRequired().HasDefaultValueSql("NOW()");
            entity.Property(e => e.ClosedOnUtc).IsRequired(false);
            entity.Property<uint>("Version").IsRowVersion(); // Concurrency protection.
            entity.HasIndex(e => e.ClientId);
            entity.HasData(GetSeedAccounts());
        });
    }

    private static List<Account> GetSeedAccounts() =>
        [
            Account.Create(
                Guid.Parse("0193bb95-d3ad-7959-9cdf-c2baacb7ebf6"),
                Guid.Parse("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                AccountType.Savings,
                "Savings Account",
                16500m,
                new(2024, 12, 12, 12, 20, 3, TimeSpan.Zero)
            ),
            Account.Create(
                Guid.Parse("0193bb95-d3ae-7ecb-9d61-07386e642dfe"),
                Guid.Parse("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                AccountType.Checking,
                "Checking Account",
                7520m,
                new(2024, 12, 14, 10, 23, 8, TimeSpan.Zero)
            ),
        ];
}
