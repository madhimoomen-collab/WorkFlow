using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Username column (unique, not null — temporarily allow null for existing rows)
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);   // nullable first so existing rows don't fail

            // Add PasswordHash column
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            // ── Seed a default admin account ─────────────────────────────
            // Password: Admin@123  (BCrypt hash, work-factor 12)
            migrationBuilder.Sql(@"
                UPDATE Users
                SET Username     = 'admin',
                    PasswordHash = '$2a$12$KIx8RGy6kKkFqC1xEJRTi.vUxMO.G1LqLGHZi8.C9D0e3Q9VCFCO.'
                WHERE Username IS NULL
                  AND Id = (SELECT TOP 1 Id FROM Users ORDER BY CreatedAt ASC);
            ");

            // Fill any remaining NULL usernames with a generated value so we can add NOT NULL constraint
            migrationBuilder.Sql(@"
                UPDATE Users
                SET Username     = CONCAT('user_', LOWER(LEFT(CAST(Id AS nvarchar(36)), 8))),
                    PasswordHash = '$2a$12$KIx8RGy6kKkFqC1xEJRTi.vUxMO.G1LqLGHZi8.C9D0e3Q9VCFCO.'
                WHERE Username IS NULL OR PasswordHash IS NULL;
            ");

            // Now enforce NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            // Unique index on Username
            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_Users_Username", table: "Users");
            migrationBuilder.DropColumn(name: "Username", table: "Users");
            migrationBuilder.DropColumn(name: "PasswordHash", table: "Users");
        }
    }
}