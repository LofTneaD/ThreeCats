using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThreeCats.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Clients_ClientId1",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Doctors_DoctorId1",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Pets_PetId1",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Services_ServiceId1",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_Pets_PetId1",
                table: "Vaccinations");

            migrationBuilder.DropForeignKey(
                name: "FK_Vaccinations_Visits_VisitId1",
                table: "Vaccinations");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitResults_Visits_VisitId1",
                table: "VisitResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Appointments_AppointmentId1",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Doctors_DoctorId1",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_AppointmentId1",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_DoctorId1",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_VisitResults_VisitId1",
                table: "VisitResults");

            migrationBuilder.DropIndex(
                name: "IX_Vaccinations_PetId1",
                table: "Vaccinations");

            migrationBuilder.DropIndex(
                name: "IX_Vaccinations_VisitId1",
                table: "Vaccinations");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ClientId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PetId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ServiceId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointmentId1",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "VisitId1",
                table: "VisitResults");

            migrationBuilder.DropColumn(
                name: "PetId1",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "VisitId1",
                table: "Vaccinations");

            migrationBuilder.DropColumn(
                name: "ClientId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "PetId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ServiceId1",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AppointmentId1",
                table: "Visits",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId1",
                table: "Visits",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VisitId1",
                table: "VisitResults",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PetId1",
                table: "Vaccinations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VisitId1",
                table: "Vaccinations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId1",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId1",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PetId1",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId1",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Visits_AppointmentId1",
                table: "Visits",
                column: "AppointmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorId1",
                table: "Visits",
                column: "DoctorId1");

            migrationBuilder.CreateIndex(
                name: "IX_VisitResults_VisitId1",
                table: "VisitResults",
                column: "VisitId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_PetId1",
                table: "Vaccinations",
                column: "PetId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccinations_VisitId1",
                table: "Vaccinations",
                column: "VisitId1");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientId1",
                table: "Appointments",
                column: "ClientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorId1",
                table: "Appointments",
                column: "DoctorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PetId1",
                table: "Appointments",
                column: "PetId1");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceId1",
                table: "Appointments",
                column: "ServiceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Clients_ClientId1",
                table: "Appointments",
                column: "ClientId1",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Doctors_DoctorId1",
                table: "Appointments",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Pets_PetId1",
                table: "Appointments",
                column: "PetId1",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Services_ServiceId1",
                table: "Appointments",
                column: "ServiceId1",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_Pets_PetId1",
                table: "Vaccinations",
                column: "PetId1",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vaccinations_Visits_VisitId1",
                table: "Vaccinations",
                column: "VisitId1",
                principalTable: "Visits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitResults_Visits_VisitId1",
                table: "VisitResults",
                column: "VisitId1",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Appointments_AppointmentId1",
                table: "Visits",
                column: "AppointmentId1",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Doctors_DoctorId1",
                table: "Visits",
                column: "DoctorId1",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
