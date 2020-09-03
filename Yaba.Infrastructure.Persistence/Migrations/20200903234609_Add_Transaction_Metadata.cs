using Microsoft.EntityFrameworkCore.Migrations;

namespace Yaba.Infrastructure.Persistence.Migrations
{
    public partial class Add_Transaction_Metadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TR_Metadata",
                table: "Transactions",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TR_Metadata",
                table: "Transactions");
        }
    }
}
