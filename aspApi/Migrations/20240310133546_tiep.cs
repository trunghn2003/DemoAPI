using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspApi.Migrations
{
    /// <inheritdoc />
    public partial class tiep : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems",
                column: "TeamId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TeamId",
                table: "TodoItems",
                column: "TeamId");
        }
    }
}
