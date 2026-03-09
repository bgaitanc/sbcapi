using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureAccountingPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingPeriods_JournalEntries_ClosingJournalEntryId",
                table: "AccountingPeriods");

            migrationBuilder.RenameTable(
                name: "AccountingPeriods",
                newName: "AccountingPeriods",
                newSchema: "accounting");

            migrationBuilder.AlterColumn<short>(
                name: "Year",
                schema: "accounting",
                table: "AccountingPeriods",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                schema: "accounting",
                table: "AccountingPeriods",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "Month",
                schema: "accounting",
                table: "AccountingPeriods",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsClosed",
                schema: "accounting",
                table: "AccountingPeriods",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "accounting",
                table: "AccountingPeriods",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClosedBy",
                schema: "accounting",
                table: "AccountingPeriods",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingPeriods_Year_Month",
                schema: "accounting",
                table: "AccountingPeriods",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_AccountingPeriods_Month_Valid",
                schema: "accounting",
                table: "AccountingPeriods",
                sql: "[Month] >= 1 AND [Month] <= 12");

            migrationBuilder.AddCheckConstraint(
                name: "CK_AccountingPeriods_Year_Valid",
                schema: "accounting",
                table: "AccountingPeriods",
                sql: "[Year] >= 1900");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingPeriods_JournalEntries_ClosingJournalEntryId",
                schema: "accounting",
                table: "AccountingPeriods",
                column: "ClosingJournalEntryId",
                principalSchema: "accounting",
                principalTable: "JournalEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingPeriods_JournalEntries_ClosingJournalEntryId",
                schema: "accounting",
                table: "AccountingPeriods");

            migrationBuilder.DropIndex(
                name: "IX_AccountingPeriods_Year_Month",
                schema: "accounting",
                table: "AccountingPeriods");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AccountingPeriods_Month_Valid",
                schema: "accounting",
                table: "AccountingPeriods");

            migrationBuilder.DropCheckConstraint(
                name: "CK_AccountingPeriods_Year_Valid",
                schema: "accounting",
                table: "AccountingPeriods");

            migrationBuilder.RenameTable(
                name: "AccountingPeriods",
                schema: "accounting",
                newName: "AccountingPeriods");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "AccountingPeriods",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "AccountingPeriods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Month",
                table: "AccountingPeriods",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AlterColumn<bool>(
                name: "IsClosed",
                table: "AccountingPeriods",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "AccountingPeriods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClosedBy",
                table: "AccountingPeriods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingPeriods_JournalEntries_ClosingJournalEntryId",
                table: "AccountingPeriods",
                column: "ClosingJournalEntryId",
                principalSchema: "accounting",
                principalTable: "JournalEntries",
                principalColumn: "Id");
        }
    }
}
