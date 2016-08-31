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
                name: "DownloadJobs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    AdvId = table.Column<int>(nullable: false),
                    CreateAt = table.Column<DateTime>(nullable: false),
                    HttpResponse = table.Column<int>(nullable: true),
                    OlxResponse = table.Column<int>(nullable: false),
                    OlxType = table.Column<int>(nullable: false),
                    ProcessedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_DownloadJobs", x => x.Id); });

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
                constraints: table => { table.PrimaryKey("PK_ParserMeta", x => x.Id); });

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
                    table.ForeignKey(
                        name: "FK_ExportJobs_DownloadJobs_DownloadJobId",
                        column: x => x.DownloadJobId,
                        principalTable: "DownloadJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportJobs_DownloadJobId",
                table: "ExportJobs",
                column: "DownloadJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ParserMeta_OlxType_Key",
                table: "ParserMeta",
                columns: new[] {"OlxType", "Key"},
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportJobs");

            migrationBuilder.DropTable(
                name: "ParserMeta");

            migrationBuilder.DropTable(
                name: "DownloadJobs");
        }
    }
}