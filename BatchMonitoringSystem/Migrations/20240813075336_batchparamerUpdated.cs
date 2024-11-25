using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BatchMonitoringSystem.Migrations
{
    /// <inheritdoc />
    public partial class batchparamerUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualValue",
                table: "BatchParameters");

            migrationBuilder.AddColumn<string>(
                name: "ActualValuesJson",
                table: "BatchParameters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualValuesJson",
                table: "BatchParameters");

            migrationBuilder.AddColumn<double>(
                name: "ActualValue",
                table: "BatchParameters",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
