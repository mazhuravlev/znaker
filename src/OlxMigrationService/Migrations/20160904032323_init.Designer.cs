﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OlxLib;

namespace OlxMigrationService.Migrations
{
    [DbContext(typeof(ParserContext))]
    [Migration("20160904032323_init")]
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

                    b.Property<int?>("AdHttpStatusCode");

                    b.Property<int>("AdvId");

                    b.Property<int?>("ContactsHttpStatusCode");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int?>("ExportJobId");

                    b.Property<int>("OlxType");

                    b.Property<DateTime?>("ProcessedAt");

                    b.Property<DateTime?>("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("ExportJobId")
                        .IsUnique();

                    b.HasIndex("OlxType", "AdvId")
                        .IsUnique();

                    b.ToTable("DownloadJobs");
                });

            modelBuilder.Entity("OlxLib.Entities.ExportJob", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("DataSerialized")
                        .HasColumnName("Data");

                    b.Property<int>("DownloadJobId");

                    b.Property<DateTime?>("ExportedAt");

                    b.HasKey("Id");

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

            modelBuilder.Entity("OlxLib.Entities.DownloadJob", b =>
                {
                    b.HasOne("OlxLib.Entities.ExportJob", "ExportJob")
                        .WithOne("DownloadJob")
                        .HasForeignKey("OlxLib.Entities.DownloadJob", "ExportJobId");
                });
        }
    }
}