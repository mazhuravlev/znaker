using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using PostgreSqlProvider;

namespace MigrationService.Migrations
{
    [DbContext(typeof(PostgreSqlContext))]
    partial class PostgreSqlContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("PostgreSqlProvider.Entities.Contact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContactType");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Identity");

                    b.HasKey("Id");

                    b.HasIndex("Identity");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("PostgreSqlProvider.Entities.Entry", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("SourceId");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.HasIndex("SourceId");

                    b.ToTable("Entries");
                });

            modelBuilder.Entity("PostgreSqlProvider.Entities.EntryContact", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ContactId");

                    b.Property<long>("EntryId");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("EntryId");

                    b.ToTable("EntryContacts");
                });

            modelBuilder.Entity("PostgreSqlProvider.Entities.Source", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Sources");
                });

            modelBuilder.Entity("PostgreSqlProvider.Entities.Entry", b =>
                {
                    b.HasOne("PostgreSqlProvider.Entities.Source", "Source")
                        .WithMany()
                        .HasForeignKey("SourceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PostgreSqlProvider.Entities.EntryContact", b =>
                {
                    b.HasOne("PostgreSqlProvider.Entities.Contact", "Contact")
                        .WithMany("EntryContacts")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PostgreSqlProvider.Entities.Entry", "Entry")
                        .WithMany("EntryContacts")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
