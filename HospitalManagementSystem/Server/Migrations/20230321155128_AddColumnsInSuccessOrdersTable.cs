using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagementSystem.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsInSuccessOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalBill",
                table: "SuccessOrders",
                newName: "TotalPrice");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "SuccessOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "SuccessOrders");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "SuccessOrders",
                newName: "TotalBill");
        }
    }
}
