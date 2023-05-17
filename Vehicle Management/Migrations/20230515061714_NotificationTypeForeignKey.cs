using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Management.Migrations
{
    /// <inheritdoc />
    public partial class NotificationTypeForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationType_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationType_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId",
                principalTable: "NotificationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationType_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationType_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId",
                principalTable: "NotificationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
