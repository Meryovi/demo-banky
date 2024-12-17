﻿// <auto-generated />
using System;
using Banky.Modules.Accounts.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Banky.Modules.Accounts.Migrations
{
    [DbContext(typeof(BankyAccountsDbContext))]
    partial class BankyAccountsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("account")
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Banky.Modules.Accounts.Domain.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Balance")
                        .HasColumnType("numeric");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("ClosedOnUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("CreatedOnUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.Property<uint>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("xid")
                        .HasColumnName("xmin");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("Accounts", "account");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0193bb95-d3ad-7959-9cdf-c2baacb7ebf6"),
                            Balance = 16500m,
                            ClientId = new Guid("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                            CreatedOnUtc = new DateTimeOffset(new DateTime(2024, 12, 12, 12, 20, 3, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsClosed = false,
                            Name = "Savings Account",
                            Type = 200
                        },
                        new
                        {
                            Id = new Guid("0193bb95-d3ae-7ecb-9d61-07386e642dfe"),
                            Balance = 7520m,
                            ClientId = new Guid("322a7823-c1e7-4fd7-b027-936c7a8fcb8a"),
                            CreatedOnUtc = new DateTimeOffset(new DateTime(2024, 12, 14, 10, 23, 8, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                            IsClosed = false,
                            Name = "Checking Account",
                            Type = 100
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
