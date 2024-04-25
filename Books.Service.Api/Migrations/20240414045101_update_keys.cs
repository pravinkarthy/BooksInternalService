using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Books.Service.Internal.Api.Migrations
{
    public partial class update_keys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TblUsers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TblRefreshtokens",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TblPermissions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblUsers",
                table: "TblUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblRefreshtokens",
                table: "TblRefreshtokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TblPermissions",
                table: "TblPermissions",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TblUsers",
                table: "TblUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblRefreshtokens",
                table: "TblRefreshtokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TblPermissions",
                table: "TblPermissions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TblUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TblRefreshtokens");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TblPermissions");
        }
    }
}
