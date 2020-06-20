using Microsoft.EntityFrameworkCore.Migrations;

namespace YabaAPI.Migrations
{
    public partial class Transaction_Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "TR_Category",
                table: "Transactions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TR_Category",
                table: "Transactions");
        }
    }
}
