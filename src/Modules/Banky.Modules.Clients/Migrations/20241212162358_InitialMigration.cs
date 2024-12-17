using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banky.Modules.Clients.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "client");

        migrationBuilder.CreateTable(
            name: "Clients",
            schema: "client",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                LastName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                Email = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                CreatedOnUtc = table.Column<DateTimeOffset>(
                    type: "timestamp with time zone",
                    nullable: false,
                    defaultValueSql: "NOW()"
                ),
                UpdatedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Clients", x => x.Id);
            }
        );

        migrationBuilder.InsertData(
            schema: "client",
            table: "Clients",
            columns: ["Id", "BirthDate", "CreatedOnUtc", "Email", "LastName", "Name", "Type", "UpdatedOnUtc"],
            values:
            [
                new Guid("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                new DateTime(1988, 5, 8, 0, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTimeOffset(
                    new DateTime(2024, 12, 10, 12, 20, 3, 0, DateTimeKind.Unspecified),
                    new TimeSpan(0, 0, 0, 0, 0)
                ),
                "johndoe@test.net",
                "Doe",
                "John",
                100,
                null
            ]
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Clients", schema: "client");
    }
}
