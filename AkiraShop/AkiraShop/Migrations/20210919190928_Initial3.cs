using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AkiraShop.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    categoryName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    desc = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    items = table.Column<List<Guid>>(type: "uuid[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    itemName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Desc = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    img = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    price = table.Column<long>(type: "bigint", nullable: false),
                    categoryID = table.Column<Guid>(type: "uuid", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
