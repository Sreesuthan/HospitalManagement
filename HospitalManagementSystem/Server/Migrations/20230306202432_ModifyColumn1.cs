using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class ModifyColumn1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Appointments",
                newName: "PatientName");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Appointments",
                newName: "ConsultingTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientName",
                table: "Appointments",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "ConsultingTime",
                table: "Appointments",
                newName: "EndTime");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
