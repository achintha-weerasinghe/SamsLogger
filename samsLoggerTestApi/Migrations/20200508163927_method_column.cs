using Microsoft.EntityFrameworkCore.Migrations;

namespace samsLoggerTestApi.Migrations
{
    public partial class method_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestMethod",
                table: "MyLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestMethod",
                table: "MyLogs");
        }
    }
}
