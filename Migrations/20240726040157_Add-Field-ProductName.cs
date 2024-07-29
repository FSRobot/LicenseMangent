using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenseManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldProductName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "RsaLicense",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "RsaLicense");
        }
    }
}
