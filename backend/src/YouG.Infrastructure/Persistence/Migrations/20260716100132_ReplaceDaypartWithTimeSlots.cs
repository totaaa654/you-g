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
            // Existing rows are keyed by Daypart, which has no valid mapping onto a single
            // 30-minute StartTime (least of all "WholeDay") - and multiple old rows for the same
            // user/date would otherwise collide on whatever default StartTime they're backfilled
            // to, violating the new unique index below. Pre-launch dev data only, safe to clear.
            migrationBuilder.Sql("DELETE FROM \"AvailabilityInstances\";");
            migrationBuilder.Sql("DELETE FROM \"AvailabilityRules\";");

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
