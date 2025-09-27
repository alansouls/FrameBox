using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InboxOutboxSample.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOutboxForeignKeyFromInbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboxMessages_OutboxMessages_OutboxMessageId",
                schema: "FrameBox",
                table: "InboxMessages");

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                schema: "FrameBox",
                table: "OutboxMessages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetryCount",
                schema: "FrameBox",
                table: "OutboxMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_InboxMessages_OutboxMessages_OutboxMessageId",
                schema: "FrameBox",
                table: "InboxMessages",
                column: "OutboxMessageId",
                principalSchema: "FrameBox",
                principalTable: "OutboxMessages",
                principalColumn: "EventId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
