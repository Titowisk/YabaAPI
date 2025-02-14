using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Yaba.Infrastructure.Persistence.Migrations
{
    public partial class CreateTransactionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TR_Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TR_Origin = table.Column<string>(maxLength: 100, nullable: true),
                    TR_Date = table.Column<DateTime>(type: "date", nullable: false),
                    TR_Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TR_Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
