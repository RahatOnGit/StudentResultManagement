using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentResultManagement.Migrations
{
    /// <inheritdoc />
    public partial class addedIndivMarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mark",
                table: "Results",
                newName: "Final");

            migrationBuilder.AddColumn<double>(
                name: "Attendence",
                table: "Results",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CT1",
                table: "Results",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CT2",
                table: "Results",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CT3",
                table: "Results",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CT4",
                table: "Results",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attendence",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "CT1",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "CT2",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "CT3",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "CT4",
                table: "Results");

            migrationBuilder.RenameColumn(
                name: "Final",
                table: "Results",
                newName: "Mark");
        }
    }
}
