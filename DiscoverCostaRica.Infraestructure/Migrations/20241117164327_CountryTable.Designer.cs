﻿// <auto-generated />
using DiscoverCostaRica.Infraestructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DiscoverCostaRica.Infraestructure.Migrations
{
    [DbContext(typeof(DiscoverCostaRicaContext))]
    [Migration("20241117164327_CountryTable")]
    partial class CountryTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Attraction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("ProviceId")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("ProviceId");

                    b.ToTable("Attraction", (string)null);
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Beach", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.ToTable("Beach", (string)null);
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Canton", b =>
                {
                    b.Property<short>("Id")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("ProvinceId")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("ProvinceId");

                    b.ToTable("Canton", (string)null);
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Country", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CountryCode")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Population")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Dish", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Dish", (string)null);
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.District", b =>
                {
                    b.Property<short>("Id")
                        .HasColumnType("smallint");

                    b.Property<short>("CantonId")
                        .HasColumnType("smallint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CantonId");

                    b.ToTable("District", (string)null);
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Province", b =>
                {
                    b.Property<short>("Id")
                        .HasColumnType("smallint");

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId")
                        .IsUnique();

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Province", (string)null);
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Attraction", b =>
                {
                    b.HasOne("DiscoverCostaRica.Domain.Entities.Province", "Province")
                        .WithMany("Attractions")
                        .HasForeignKey("ProviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Province");
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Canton", b =>
                {
                    b.HasOne("DiscoverCostaRica.Domain.Entities.Province", "Province")
                        .WithMany("Cantons")
                        .HasForeignKey("ProvinceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Province");
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.District", b =>
                {
                    b.HasOne("DiscoverCostaRica.Domain.Entities.Canton", "Canton")
                        .WithMany("Districts")
                        .HasForeignKey("CantonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Canton");
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Province", b =>
                {
                    b.HasOne("DiscoverCostaRica.Domain.Entities.Country", "Country")
                        .WithOne("Capital")
                        .HasForeignKey("DiscoverCostaRica.Domain.Entities.Province", "CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Canton", b =>
                {
                    b.Navigation("Districts");
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Country", b =>
                {
                    b.Navigation("Capital")
                        .IsRequired();
                });

            modelBuilder.Entity("DiscoverCostaRica.Domain.Entities.Province", b =>
                {
                    b.Navigation("Attractions");

                    b.Navigation("Cantons");
                });
#pragma warning restore 612, 618
        }
    }
}