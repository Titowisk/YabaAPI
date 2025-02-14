using Microsoft.EntityFrameworkCore.Migrations;

namespace Yaba.Infrastructure.Persistence.Migrations
{
    public partial class RefactorModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankAccounts_BankAccountId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "BankAccountId",
                table: "Transactions",
                newName: "TR_BankAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_BankAccountId",
                table: "Transactions",
                newName: "IX_Transactions_TR_BankAccountId");

            migrationBuilder.AlterColumn<int>(
                name: "TR_BankAccountId",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BK_UserId",
                table: "BankAccounts",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankAccounts_TR_BankAccountId",
                table: "Transactions",
                column: "TR_BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "BK_Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BankAccounts_TR_BankAccountId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TR_BankAccountId",
                table: "Transactions",
                newName: "BankAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TR_BankAccountId",
                table: "Transactions",
                newName: "IX_Transactions_BankAccountId");

            migrationBuilder.AlterColumn<int>(
                name: "BankAccountId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BK_UserId",
                table: "BankAccounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BankAccounts_BankAccountId",
                table: "Transactions",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "BK_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
