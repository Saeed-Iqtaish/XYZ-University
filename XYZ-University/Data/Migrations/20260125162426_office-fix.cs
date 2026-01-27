using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XYZ_University.Data.Migrations
{
    /// <inheritdoc />
    public partial class officefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offices_AspNetUsers_InstructorId",
                table: "Offices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Offices",
                table: "Offices");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Offices",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "OfficeId",
                table: "Offices",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Offices",
                table: "Offices",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Offices_InstructorId",
                table: "Offices",
                column: "InstructorId",
                unique: true,
                filter: "[InstructorId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_AspNetUsers_InstructorId",
                table: "Offices",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offices_AspNetUsers_InstructorId",
                table: "Offices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Offices",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Offices_InstructorId",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "Offices");

            migrationBuilder.AlterColumn<string>(
                name: "InstructorId",
                table: "Offices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Offices",
                table: "Offices",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offices_AspNetUsers_InstructorId",
                table: "Offices",
                column: "InstructorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
