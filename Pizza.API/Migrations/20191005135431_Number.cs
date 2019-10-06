using Microsoft.EntityFrameworkCore.Migrations;

namespace Pizza.API.Migrations
{
    public partial class Number : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "MenuItemModel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "MenuItemModel");
        }
    }
}
