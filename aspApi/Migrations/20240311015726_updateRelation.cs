using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspApi.Migrations
{
    /// <inheritdoc />
    public partial class updateRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Teams_TeamId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "TodoItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Teams_TeamId",
                table: "TodoItems",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_Teams_TeamId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "TodoItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems",
                column: "TeamId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_Teams_TeamId",
                table: "TodoItems",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "TeamId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
