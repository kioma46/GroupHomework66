using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupHomework66.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsRespondedByUserField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRespondedByTheCurrentUser",
                table: "Vacancies",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRespondedByTheCurrentUser",
                table: "Vacancies");
        }
    }
}
