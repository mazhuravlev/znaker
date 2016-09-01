using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OlxLib;

namespace OlxMigrationService.Migrations
{
    [DbContext(typeof(ParserContext))]
    [Migration("20160901181631_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("OlxLib.Entities.DownloadJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdvId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("HttpResponse");

                    b.Property<int>("OlxResponse");

                    b.Property<int>("OlxType");

                    b.Property<DateTime?>("ProcessedAt");

                    b.HasKey("Id");

                    b.HasIndex("OlxType", "AdvId")
                        .IsUnique();

                    b.ToTable("DownloadJobs");
                });

            modelBuilder.Entity("OlxLib.Entities.ExportJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateAt");

                    b.Property<string>("DataSerialized")
                        .HasColumnName("Data");

                    b.Property<int>("DownloadJobId");

                    b.HasKey("Id");

                    b.HasIndex("DownloadJobId");

                    b.ToTable("ExportJobs");
                });

            modelBuilder.Entity("OlxLib.Entities.ParserMeta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key");

                    b.Property<int>("OlxType");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("OlxType", "Key")
                        .IsUnique();

                    b.ToTable("ParserMeta");
                });

            modelBuilder.Entity("OlxLib.Entities.ExportJob", b =>
                {
                    b.HasOne("OlxLib.Entities.DownloadJob", "DownloadJob")
                        .WithMany()
                        .HasForeignKey("DownloadJobId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
