using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.data.migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ArabicName",
                table: "Accounts",
                column: "ArabicName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Code",
                table: "Accounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_EnglishName",
                table: "Accounts",
                column: "EnglishName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Accounts_ArabicName",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Code",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_EnglishName",
                table: "Accounts");
        }
    }
}
