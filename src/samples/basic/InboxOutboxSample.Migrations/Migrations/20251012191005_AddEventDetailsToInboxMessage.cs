using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InboxOutboxSample.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddEventDetailsToInboxMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OutboxMessageId",
                schema: "FrameBox",
                table: "InboxMessages",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_InboxMessages_OutboxMessageId",
                schema: "FrameBox",
                table: "InboxMessages",
                newName: "IX_InboxMessages_EventId");

            migrationBuilder.AddColumn<string>(
                name: "EventName",
                schema: "FrameBox",
                table: "InboxMessages",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EventPayload",
                schema: "FrameBox",
                table: "InboxMessages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventName",
                schema: "FrameBox",
                table: "InboxMessages");

            migrationBuilder.DropColumn(
                name: "EventPayload",
                schema: "FrameBox",
                table: "InboxMessages");

            migrationBuilder.RenameColumn(
                name: "EventId",
                schema: "FrameBox",
                table: "InboxMessages",
                newName: "OutboxMessageId");

            migrationBuilder.RenameIndex(
                name: "IX_InboxMessages_EventId",
                schema: "FrameBox",
                table: "InboxMessages",
                newName: "IX_InboxMessages_OutboxMessageId");
        }
    }
}
