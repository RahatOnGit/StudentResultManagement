using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentResultManagement.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Semesters",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "2-1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Semesters",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
