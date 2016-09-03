using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MigrationService.Migrations
{
    public partial class insert_source_data_1_to_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            Avito = 1,
            OlxUa = 2,
            OlxBy = 3,
            OlxKz = 4,
            OlxUz = 5
             */


            migrationBuilder
                .Sql("INSERT INTO public.\"Sources\" (\"Id\", \"SiteUrl\", \"Title\") VALUES " +

                    "('1', 'http://www.avito.ru', 'Avito.ru')," +
                    "('2', 'http://olx.ua',       'Olx.ua')," +
                    "('3', 'http://olx.by',       'Olx.by')," +
                    "('4', 'http://olx.kz',       'Olx.kz')," +
                    "('5', 'http://olx.uz',       'Olx.uz');"

                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
