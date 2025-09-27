using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InboxOutboxSample.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProcessId",
                schema: "FrameBox",
                table: "OutboxMessages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_ProcessId",
                schema: "FrameBox",
                table: "OutboxMessages",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_State",
                schema: "FrameBox",
                table: "OutboxMessages",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_ProcessId",
                schema: "FrameBox",
                table: "OutboxMessages");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessages_State",
                schema: "FrameBox",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                schema: "FrameBox",
                table: "OutboxMessages");
        }
    }
}
