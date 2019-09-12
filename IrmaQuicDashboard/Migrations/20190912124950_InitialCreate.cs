using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IrmaQuicDashboard.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionUploadMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    SessionNumber = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    UsesQuic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionUploadMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrmaSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SessionUploadMetadataId = table.Column<Guid>(nullable: false),
                    AppSessionId = table.Column<int>(nullable: false),
                    SessionToken = table.Column<string>(nullable: true),
                    IrmaJsSessionToken = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrmaSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IrmaSessions_SessionUploadMetadatas_SessionUploadMetadataId",
                        column: x => x.SessionUploadMetadataId,
                        principalTable: "SessionUploadMetadatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IrmaSessionId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLogEntries_IrmaSessions_IrmaSessionId",
                        column: x => x.IrmaSessionId,
                        principalTable: "IrmaSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IrmaSessionId = table.Column<Guid>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerLogEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerLogEntries_IrmaSessions_IrmaSessionId",
                        column: x => x.IrmaSessionId,
                        principalTable: "IrmaSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TimestampedLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IrmaSessionId = table.Column<Guid>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimestampedLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimestampedLocations_IrmaSessions_IrmaSessionId",
                        column: x => x.IrmaSessionId,
                        principalTable: "IrmaSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLogEntries_IrmaSessionId",
                table: "AppLogEntries",
                column: "IrmaSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_IrmaSessions_SessionUploadMetadataId",
                table: "IrmaSessions",
                column: "SessionUploadMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerLogEntries_IrmaSessionId",
                table: "ServerLogEntries",
                column: "IrmaSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TimestampedLocations_IrmaSessionId",
                table: "TimestampedLocations",
                column: "IrmaSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLogEntries");

            migrationBuilder.DropTable(
                name: "ServerLogEntries");

            migrationBuilder.DropTable(
                name: "TimestampedLocations");

            migrationBuilder.DropTable(
                name: "IrmaSessions");

            migrationBuilder.DropTable(
                name: "SessionUploadMetadatas");
        }
    }
}
