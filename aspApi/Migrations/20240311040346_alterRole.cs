using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspApi.Migrations
{
    /// <inheritdoc />
    public partial class alterRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "TeamUsers");

            migrationBuilder.DropColumn(
                name: "IsLeader",
                table: "TeamUsers");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "TeamUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "TeamUsers");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "TeamUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLeader",
                table: "TeamUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
