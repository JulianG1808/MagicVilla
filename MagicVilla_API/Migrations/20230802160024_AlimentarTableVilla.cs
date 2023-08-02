using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTableVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalles de la Villa...", new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7169), new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7158), "", 50, "Villa Real", 5, 200.0 },
                    { 2, "", "Detalles de la Villa...", new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7173), new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7172), "", 30, "Otra Villa", 3, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
