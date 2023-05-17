using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Management.Migrations
{
    /// <inheritdoc />
    public partial class RequestStatusChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatus_AspNetUsers_CreatedById",
                table: "RequestStatus");

            migrationBuilder.DropIndex(
                name: "IX_RequestStatus_CreatedById",
                table: "RequestStatus");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "RequestStatus");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "RequestStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "RequestStatus",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "RequestStatus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatus_CreatedById",
                table: "RequestStatus",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatus_AspNetUsers_CreatedById",
                table: "RequestStatus",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
