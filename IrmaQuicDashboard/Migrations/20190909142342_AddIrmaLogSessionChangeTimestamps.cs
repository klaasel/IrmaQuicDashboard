using Microsoft.EntityFrameworkCore.Migrations;

namespace IrmaQuicDashboard.Migrations
{
    public partial class AddIrmaLogSessionChangeTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IrmaLogSessionId",
                table: "LogEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RawInfo",
                table: "LogEntries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RawJson",
                table: "LogEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IrmaLogSessionId",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "RawInfo",
                table: "LogEntries");

            migrationBuilder.DropColumn(
                name: "RawJson",
                table: "LogEntries");
        }
    }
}
