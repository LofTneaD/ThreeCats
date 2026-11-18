using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreeCats.Web.Migrations
{
    /// <inheritdoc />
    public partial class AlignServiceConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Service_Price_NonNegative",
                table: "Services");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Service_Price_Positive",
                table: "Services",
                sql: "\"Price\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Service_Price_Positive",
                table: "Services");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Services",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Service_Price_NonNegative",
                table: "Services",
                sql: "\"Price\" >= 0");
        }
    }
}
