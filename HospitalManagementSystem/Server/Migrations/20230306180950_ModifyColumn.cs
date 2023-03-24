using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class ModifyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ApplicationUsers");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "ApplicationUsers",
                newName: "ConsultingTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConsultingTime",
                table: "ApplicationUsers",
                newName: "StartTime");

            migrationBuilder.AddColumn<string>(
                name: "EndTime",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
