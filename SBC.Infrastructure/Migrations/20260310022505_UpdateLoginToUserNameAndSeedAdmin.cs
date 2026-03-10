using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SBC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoginToUserNameAndSeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastLoginDate", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("01c0b3b4-f06b-4e1b-852d-94863c87f6e1"), 0, "8dce09d9-ddfe-4cda-bc89-450169fab180", "admin@sbc.com", true, "Administrador", null, "SBC", false, null, "ADMIN@SBC.COM", "ADMIN", "AQAAAAIAAYagAAAAEMbXUX7hqw4CNdCZ6tHC/tlOeuSJG3f/gKR0a/7zhorqe66hRzMzSF1iEH8LWtkWoQ==", null, false, null, null, "52705f07-a77e-41cb-aadf-1f549e91434a", false, "admin" });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("8bc2bfc1-1e12-4b5f-a1ff-679b06b1996b"), new Guid("01c0b3b4-f06b-4e1b-852d-94863c87f6e1") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("8bc2bfc1-1e12-4b5f-a1ff-679b06b1996b"), new Guid("01c0b3b4-f06b-4e1b-852d-94863c87f6e1") });

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("01c0b3b4-f06b-4e1b-852d-94863c87f6e1"));
        }
    }
}
