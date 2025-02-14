using Microsoft.EntityFrameworkCore.Migrations;

namespace Yaba.Infrastructure.Persistence.Migrations
{
    public partial class BankAccount_Code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "BK_Code",
                table: "BankAccounts",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BK_Code",
                table: "BankAccounts");
        }
    }
}
