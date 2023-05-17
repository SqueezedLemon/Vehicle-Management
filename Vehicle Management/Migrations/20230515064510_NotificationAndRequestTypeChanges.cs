using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Management.Migrations
{
    /// <inheritdoc />
    public partial class NotificationAndRequestTypeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestHistory_RequestId",
                table: "RequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_RequestHistory_RequestStatusId",
                table: "RequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_Request_RequestStatusId",
                table: "Request");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistory_RequestId",
                table: "RequestHistory",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistory_RequestStatusId",
                table: "RequestHistory",
                column: "RequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestStatusId",
                table: "Request",
                column: "RequestStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestHistory_RequestId",
                table: "RequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_RequestHistory_RequestStatusId",
                table: "RequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_Request_RequestStatusId",
                table: "Request");

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistory_RequestId",
                table: "RequestHistory",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestHistory_RequestStatusId",
                table: "RequestHistory",
                column: "RequestStatusId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestStatusId",
                table: "Request",
                column: "RequestStatusId",
                unique: true);
        }
    }
}
