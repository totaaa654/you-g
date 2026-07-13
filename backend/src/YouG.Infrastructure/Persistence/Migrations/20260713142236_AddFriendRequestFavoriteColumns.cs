using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YouG.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendRequestFavoriteColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddresseeFavorited",
                table: "FriendRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequesterFavorited",
                table: "FriendRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddresseeFavorited",
                table: "FriendRequests");

            migrationBuilder.DropColumn(
                name: "RequesterFavorited",
                table: "FriendRequests");
        }
    }
}
