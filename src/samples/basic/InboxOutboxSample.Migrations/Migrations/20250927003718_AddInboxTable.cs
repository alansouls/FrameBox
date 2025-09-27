using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InboxOutboxSample.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddInboxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InboxMessages",
                schema: "FrameBox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboxMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    HandlerName = table.Column<string>(type: "text", nullable: false),
                    FailurePayload = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    ProcessId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboxMessages_OutboxMessages_OutboxMessageId",
                        column: x => x.OutboxMessageId,
                        principalSchema: "FrameBox",
                        principalTable: "OutboxMessages",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_OutboxMessageId",
                schema: "FrameBox",
                table: "InboxMessages",
                column: "OutboxMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_ProcessId",
                schema: "FrameBox",
                table: "InboxMessages",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_State",
                schema: "FrameBox",
                table: "InboxMessages",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InboxMessages",
                schema: "FrameBox");
        }
    }
}
