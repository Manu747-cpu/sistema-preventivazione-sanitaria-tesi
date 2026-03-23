using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Preventivatore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategoriaNomeToPreventiviMvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubCategoriaNome",
                table: "PreventiviMvp",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubCategoriaNome",
                table: "PreventiviMvp");
        }
    }
}
