namespace Banky.Modules.Clients.Infrastructure;

internal class BankyClientsDbContext(DbContextOptions<BankyClientsDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("client");
        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Type).IsRequired().HasConversion<int>();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.LastName).HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(80);
            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.Property(e => e.CreatedOnUtc).IsRequired().HasDefaultValueSql("NOW()");
            entity.Property(e => e.UpdatedOnUtc);
            entity.HasData(GetSeedClients());
        });
    }

    private static List<Client> GetSeedClients() =>
        [
            Client.Create(
                Guid.Parse("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                ClientType.Person,
                "John",
                "Doe",
                "johndoe@test.net",
                new(1988, 5, 8),
                new(2024, 12, 10, 12, 20, 3, TimeSpan.Zero)
            )
        ];
}
