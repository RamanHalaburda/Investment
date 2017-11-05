using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Investments.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Optimazation",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dividentsA = table.Column<int>(type: "int", nullable: false),
                    dividentsB = table.Column<int>(type: "int", nullable: false),
                    investmentA = table.Column<int>(type: "int", nullable: false),
                    investmentB = table.Column<int>(type: "int", nullable: false),
                    limitA = table.Column<int>(type: "int", nullable: false),
                    limitB = table.Column<int>(type: "int", nullable: false),
                    result = table.Column<int>(type: "int", nullable: false),
                    sum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Optimazation", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Optimazation");
        }
    }
}
