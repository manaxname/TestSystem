using Microsoft.EntityFrameworkCore.Migrations;

namespace TestSystem.Data.Migrations
{
    public partial class topicid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopicType",
                table: "Topics",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopicType",
                table: "Topics");
        }
    }
}
