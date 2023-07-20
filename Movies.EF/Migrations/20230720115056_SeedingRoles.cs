using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FirstWebApi.Migrations
{
    public partial class SeedingRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4752df33-a9ea-487f-90b8-b454b11d4b9d", "10b1e9e4-10e5-4290-b164-06436465c484", "user", "USER" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7245849b-7b69-463a-a067-6662f9297f02", "c97840e9-1265-4b0a-9f1e-bc9185183932", "admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "4752df33-a9ea-487f-90b8-b454b11d4b9d");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "7245849b-7b69-463a-a067-6662f9297f02");
        }
    }
}
