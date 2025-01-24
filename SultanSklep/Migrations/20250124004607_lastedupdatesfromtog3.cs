using Microsoft.EntityFrameworkCore.Migrations;

namespace SultanSklep.Migrations
{
    public partial class lastedupdatesfromtog3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "ProductOperations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "ProductOperations");
        }
    }
}
