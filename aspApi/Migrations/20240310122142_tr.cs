using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aspApi.Migrations
{
    /// <inheritdoc />
    public partial class tr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TodoItems",
                newName: "TodoItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TodoItemId",
                table: "TodoItems",
                newName: "Id");
        }
    }
}
