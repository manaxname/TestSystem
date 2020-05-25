using Microsoft.EntityFrameworkCore.Migrations;

namespace TestSystem.Data.Migrations
{
    public partial class thir : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isLocked",
                table: "Topics",
                newName: "IsLocked");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLocked",
                table: "Topics",
                newName: "isLocked");
        }
    }
}
