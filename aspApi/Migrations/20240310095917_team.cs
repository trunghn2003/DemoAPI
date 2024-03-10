using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspApi.Migrations
{
    /// <inheritdoc />
    public partial class team : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "User",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    TeamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.TeamId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_TeamId",
                table: "User",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Teams_TeamId",
                table: "User",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Teams_TeamId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_User_TeamId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User",
                newName: "Id");
        }
    }
}
