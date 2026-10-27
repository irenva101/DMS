using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMS.Library.Migrations
{
    /// <inheritdoc />
    public partial class remove_processes_sent_to_sef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sent_to_sef",
                schema: "dms",
                table: "processes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "sent_to_sef",
                schema: "dms",
                table: "processes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
