using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InboxOutboxSample.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddEventContextTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventContexts",
                schema: "FrameBox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventContexts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventContexts_Links",
                schema: "FrameBox",
                columns: table => new
                {
                    EventContextId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventContexts_Links", x => new { x.EventContextId, x.EventId });
                    table.ForeignKey(
                        name: "FK_EventContexts_Links_EventContexts_EventContextId",
                        column: x => x.EventContextId,
                        principalSchema: "FrameBox",
                        principalTable: "EventContexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventContexts_Links",
                schema: "FrameBox");

            migrationBuilder.DropTable(
                name: "EventContexts",
                schema: "FrameBox");
        }
    }
}
