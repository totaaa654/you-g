using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceDaypartWithTimeSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvailabilityInstances_UserId_Date_Daypart",
                table: "AvailabilityInstances");

            migrationBuilder.DropColumn(
                name: "Daypart",
                table: "AvailabilityRules");

            migrationBuilder.DropColumn(
                name: "Daypart",
                table: "AvailabilityInstances");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "AvailabilityRules",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "AvailabilityInstances",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityInstances_UserId_Date_StartTime",
                table: "AvailabilityInstances",
                columns: new[] { "UserId", "Date", "StartTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvailabilityInstances_UserId_Date_StartTime",
                table: "AvailabilityInstances");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "AvailabilityRules");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "AvailabilityInstances");

            migrationBuilder.AddColumn<short>(
                name: "Daypart",
                table: "AvailabilityRules",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Daypart",
                table: "AvailabilityInstances",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilityInstances_UserId_Date_Daypart",
                table: "AvailabilityInstances",
                columns: new[] { "UserId", "Date", "Daypart" },
                unique: true);
        }
    }
}
