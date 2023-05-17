using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Management.Migrations
{
    /// <inheritdoc />
    public partial class RequestStatusAndNotificationStstus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetedRole",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "NotificationTypeId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RoleId",
                table: "Notifications",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetRoles_RoleId",
                table: "Notifications",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotificationType_NotificationTypeId",
                table: "Notifications",
                column: "NotificationTypeId",
                principalTable: "NotificationType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetRoles_RoleId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotificationType_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RoleId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationTypeId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "NotificationType",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetedRole",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
