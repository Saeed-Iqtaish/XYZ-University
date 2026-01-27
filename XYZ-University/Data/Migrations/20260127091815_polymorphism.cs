using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XYZ_University.Data.Migrations
{
    /// <inheritdoc />
    public partial class polymorphism : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Courses",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LabSoftware",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Platform",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "LabSoftware",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Platform",
                table: "Courses");
        }
    }
}
