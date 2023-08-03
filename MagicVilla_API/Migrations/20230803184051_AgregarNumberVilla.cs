using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNumberVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumberVillas",
                columns: table => new
                {
                    VillaNo = table.Column<int>(type: "int", nullable: false),
                    VillaId = table.Column<int>(type: "int", nullable: false),
                    DetalleEspecial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberVillas", x => x.VillaNo);
                    table.ForeignKey(
                        name: "FK_NumberVillas_Villas_VillaId",
                        column: x => x.VillaId,
                        principalTable: "Villas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2023, 8, 3, 15, 40, 51, 230, DateTimeKind.Local).AddTicks(6988), new DateTime(2023, 8, 3, 15, 40, 51, 230, DateTimeKind.Local).AddTicks(6975) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2023, 8, 3, 15, 40, 51, 230, DateTimeKind.Local).AddTicks(6991), new DateTime(2023, 8, 3, 15, 40, 51, 230, DateTimeKind.Local).AddTicks(6991) });

            migrationBuilder.CreateIndex(
                name: "IX_NumberVillas_VillaId",
                table: "NumberVillas",
                column: "VillaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumberVillas");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7169), new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7158) });

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaActualizacion", "FechaCreacion" },
                values: new object[] { new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7173), new DateTime(2023, 8, 2, 13, 0, 24, 150, DateTimeKind.Local).AddTicks(7172) });
        }
    }
}
