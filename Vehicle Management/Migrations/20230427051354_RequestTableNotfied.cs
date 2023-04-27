using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vehicle_Management.Migrations
{
    /// <inheritdoc />
    public partial class RequestTableNotfied : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotified",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnapproved",
                table: "Request",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotified",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "IsUnapproved",
                table: "Request");
        }
    }
}
