using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Banky.Modules.Accounts.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "account");

        migrationBuilder.CreateTable(
            name: "Accounts",
            schema: "account",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Balance = table.Column<decimal>(type: "numeric", nullable: false),
                IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                CreatedOnUtc = table.Column<DateTimeOffset>(
                    type: "timestamp with time zone",
                    nullable: false,
                    defaultValueSql: "NOW()"
                ),
                ClosedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Accounts", x => x.Id);
            }
        );

        migrationBuilder.InsertData(
            schema: "account",
            table: "Accounts",
            columns: ["Id", "Balance", "ClientId", "ClosedOnUtc", "CreatedOnUtc", "IsClosed", "Name", "Type"],
            values: new object[,]
            {
                {
                    new Guid("0193bb95-d3ad-7959-9cdf-c2baacb7ebf6"),
                    16500m,
                    new Guid("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                    null,
                    new DateTimeOffset(
                        new DateTime(2024, 12, 12, 12, 20, 3, 0, DateTimeKind.Unspecified),
                        new TimeSpan(0, 0, 0, 0, 0)
                    ),
                    false,
                    "Savings Account",
                    200
                },
                {
                    new Guid("0193bb95-d3ae-7ecb-9d61-07386e642dfe"),
                    7520m,
                    new Guid("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                    null,
                    new DateTimeOffset(
                        new DateTime(2024, 12, 14, 10, 23, 8, 0, DateTimeKind.Unspecified),
                        new TimeSpan(0, 0, 0, 0, 0)
                    ),
                    false,
                    "Checking Account",
                    100
                }
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_Accounts_ClientId",
            schema: "account",
            table: "Accounts",
            column: "ClientId"
        );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Accounts", schema: "account");
    }
}
