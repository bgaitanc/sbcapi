using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LogDate",
                schema: "logging",
                table: "TransactionLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Action",
                schema: "logging",
                table: "TransactionLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_EntityName",
                schema: "logging",
                table: "TransactionLogs",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_LogDate",
                schema: "logging",
                table: "TransactionLogs",
                column: "LogDate");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_Status",
                schema: "logging",
                table: "TransactionLogs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_Action",
                schema: "logging",
                table: "TransactionLogs");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_EntityName",
                schema: "logging",
                table: "TransactionLogs");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_LogDate",
                schema: "logging",
                table: "TransactionLogs");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLogs_Status",
                schema: "logging",
                table: "TransactionLogs");

            migrationBuilder.DropColumn(
                name: "LogDate",
                schema: "logging",
                table: "TransactionLogs");
        }
    }
}
