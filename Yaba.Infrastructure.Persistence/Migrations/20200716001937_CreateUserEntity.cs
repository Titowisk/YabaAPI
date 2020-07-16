using Microsoft.EntityFrameworkCore.Migrations;

namespace Yaba.Infrastructure.Persistence.Migrations
{
    public partial class CreateUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Transactions_BankAccount_BankAccountId",
            //    table: "Transactions");

            //migrationBuilder.DropTable(
            //    name: "BankAccount");

            migrationBuilder.AddColumn<int>(
                name: "BK_UserId",
                table: "BankAccounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    US_Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    US_Name = table.Column<string>(maxLength: 256, nullable: false),
                    US_Email = table.Column<string>(maxLength: 256, nullable: false),
                    US_Password = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.US_Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_BK_UserId",
                table: "BankAccounts",
                column: "BK_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Users_BK_UserId",
                table: "BankAccounts",
                column: "BK_UserId",
                principalTable: "Users",
                principalColumn: "US_Id",
                onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Transactions_BankAccounts_BankAccountId",
            //    table: "Transactions",
            //    column: "BankAccountId",
            //    principalTable: "BankAccounts",
            //    principalColumn: "BK_Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Users_BK_UserId",
                table: "BankAccounts");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Transactions_BankAccounts_BankAccountId",
            //    table: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_BK_UserId",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "BK_UserId",
                table: "BankAccounts");

            //migrationBuilder.CreateTable(
            //    name: "BankAccount",
            //    columns: table => new
            //    {
            //        TempId = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.UniqueConstraint("AK_BankAccount_TempId", x => x.TempId);
            //    });

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Transactions_BankAccount_BankAccountId",
            //    table: "Transactions",
            //    column: "BankAccountId",
            //    principalTable: "BankAccount",
            //    principalColumn: "TempId",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
