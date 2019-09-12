using Microsoft.EntityFrameworkCore.Migrations;

namespace IrmaQuicDashboard.Migrations
{
    public partial class AddUsesQuic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsesQuic",
                table: "SessionUploadMetadatas",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsesQuic",
                table: "SessionUploadMetadatas");
        }
    }
}
