
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreeCats.Web.Migrations
{
    public partial class MoveDoctorNameToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Сначала добавляем новые столбцы в AspNetUsers.
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            // Переносим ФИО существующих врачей в их учётные записи.
            migrationBuilder.Sql("""
                UPDATE "AspNetUsers" AS u
                SET
                    "LastName" = d."LastName",
                    "FirstName" = d."FirstName",
                    "MiddleName" = d."MiddleName"
                FROM "Doctors" AS d
                WHERE u."Id" = d."UserId";
                """);

            // Только после переноса удаляем старые столбцы.
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Doctors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Возвращаем столбцы ФИО в Doctors.
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Doctors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Doctors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Doctors",
                type: "text",
                nullable: true);

            // Восстанавливаем ФИО из AspNetUsers.
            migrationBuilder.Sql("""
                UPDATE "Doctors" AS d
                SET
                    "LastName" = u."LastName",
                    "FirstName" = u."FirstName",
                    "MiddleName" = u."MiddleName"
                FROM "AspNetUsers" AS u
                WHERE d."UserId" = u."Id";
                """);

            // Удаляем новые столбцы из AspNetUsers.
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "AspNetUsers");
        }
    }
}
