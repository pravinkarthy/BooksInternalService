using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Books.Service.Internal.Api.Migrations
{
    public partial class seeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TblRoles",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblRoles",
                table: "TblRoles",
                column: "Id");

            migrationBuilder.InsertData(
                table: "TblRoles",
                columns: new[] { "Id", "Name", "Roleid" },
                values: new object[] { 1, "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "TblRoles",
                columns: new[] { "Id", "Name", "Roleid" },
                values: new object[] { 2, "User", "user" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TblRoles",
                table: "TblRoles");

            migrationBuilder.DeleteData(
                table: "TblRoles",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TblRoles",
                keyColumn: "Id",
                keyColumnType: "int",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TblRoles");
        }
    }
}
