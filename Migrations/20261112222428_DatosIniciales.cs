using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AcademiaApi.Migrations
{
    /// <inheritdoc />
    public partial class DatosIniciales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Estudiante",
                columns: new[] { "IdEstudiante", "Activo", "Apellidos", "Email", "FechaRegistro", "Nombres", "Telefono" },
                values: new object[,]
                {
                    { 1, true, "López Quispe", "maria.lopez@alumnos.academia.com", new DateTime(2026, 3, 2, 9, 15, 0, 0, DateTimeKind.Unspecified), "María", "987654321" },
                    { 2, true, "Pérez Huamán", "jose.perez@alumnos.academia.com", new DateTime(2026, 3, 5, 14, 30, 0, 0, DateTimeKind.Unspecified), "José", "912345678" },
                    { 3, true, "Torres Mendoza", "lucia.torres@alumnos.academia.com", new DateTime(2026, 4, 10, 10, 0, 0, 0, DateTimeKind.Unspecified), "Lucía", null }
                });

            migrationBuilder.InsertData(
                table: "Instructor",
                columns: new[] { "IdInstructor", "Activo", "Apellidos", "Email", "Especialidad", "Nombres" },
                values: new object[,]
                {
                    { 1, true, "Ramírez Torres", "carlos.ramirez@academia.com", "Desarrollo Web", "Carlos" },
                    { 2, true, "Gómez Salazar", "ana.gomez@academia.com", "Base de Datos", "Ana" },
                    { 3, false, "Fernández Castro", "luis.fernandez@academia.com", "Redes y Seguridad", "Luis" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Estudiante",
                keyColumn: "IdEstudiante",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Estudiante",
                keyColumn: "IdEstudiante",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Estudiante",
                keyColumn: "IdEstudiante",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Instructor",
                keyColumn: "IdInstructor",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Instructor",
                keyColumn: "IdInstructor",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Instructor",
                keyColumn: "IdInstructor",
                keyValue: 3);
        }
    }
}
