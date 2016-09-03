using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OlxMigrationService.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    DownloadJobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParserMeta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Key = table.Column<string>(nullable: true),
                    OlxType = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParserMeta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DownloadJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    AdHttpStatusCode = table.Column<int>(nullable: true),
                    AdvId = table.Column<int>(nullable: false),
                    ContactsHttpStatusCode = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ExportJobId = table.Column<int>(nullable: true),
                    OlxType = table.Column<int>(nullable: false),
                    ProcessedAt = table.Column<DateTime>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DownloadJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DownloadJobs_ExportJobs_ExportJobId",
                        column: x => x.ExportJobId,
                        principalTable: "ExportJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DownloadJobs_ExportJobId",
                table: "DownloadJobs",
                column: "ExportJobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DownloadJobs_OlxType_AdvId",
                table: "DownloadJobs",
                columns: new[] { "OlxType", "AdvId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParserMeta_OlxType_Key",
                table: "ParserMeta",
                columns: new[] { "OlxType", "Key" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DownloadJobs");

            migrationBuilder.DropTable(
                name: "ParserMeta");

            migrationBuilder.DropTable(
                name: "ExportJobs");
        }
    }
}
