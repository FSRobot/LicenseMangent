using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicenseManagement.Migrations
{
    /// <inheritdoc />
    public partial class addtableOEMmodifyOpName2OpId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpName",
                table: "RsaLicense",
                newName: "OpId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OpId",
                table: "RsaLicense",
                newName: "OpName");
        }
    }
}
