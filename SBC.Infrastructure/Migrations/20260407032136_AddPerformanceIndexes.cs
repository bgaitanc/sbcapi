using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_Credit",
                schema: "accounting",
                table: "JournalEntryLines",
                column: "Credit");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_Debit",
                schema: "accounting",
                table: "JournalEntryLines",
                column: "Debit");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_Date",
                schema: "accounting",
                table: "JournalEntries",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_Date_Code",
                schema: "accounting",
                table: "JournalEntries",
                columns: new[] { "Date", "Code" },
                descending: new[] { true, true });

            migrationBuilder.CreateIndex(
                name: "IX_BulkImports_CreatedAt",
                schema: "accounting",
                table: "BulkImports",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_BulkImports_FileName",
                schema: "accounting",
                table: "BulkImports",
                column: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_Credit",
                schema: "accounting",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_Debit",
                schema: "accounting",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_Date",
                schema: "accounting",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_Date_Code",
                schema: "accounting",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_BulkImports_CreatedAt",
                schema: "accounting",
                table: "BulkImports");

            migrationBuilder.DropIndex(
                name: "IX_BulkImports_FileName",
                schema: "accounting",
                table: "BulkImports");
        }
    }
}
