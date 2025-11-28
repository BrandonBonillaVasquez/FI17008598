using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuixoWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estadisticas",
                columns: table => new
                {
                    EstadisticaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModoJuego = table.Column<byte>(type: "tinyint", nullable: false),
                    JugadorEquipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PartidasGanadas = table.Column<int>(type: "int", nullable: false),
                    PartidasJugadas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estadisticas", x => x.EstadisticaId);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    PartidaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModoJuego = table.Column<byte>(type: "tinyint", nullable: false),
                    FechaHoraCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TiempoTranscurrido = table.Column<TimeSpan>(type: "time", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "EnCurso"),
                    GanadorId = table.Column<int>(type: "int", nullable: true),
                    EquipoGanador = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.PartidaId);
                });

            migrationBuilder.CreateTable(
                name: "Jugadas",
                columns: table => new
                {
                    JugadaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidaId = table.Column<int>(type: "int", nullable: false),
                    NumeroJugada = table.Column<int>(type: "int", nullable: false),
                    JugadorActual = table.Column<int>(type: "int", nullable: false),
                    FilaOrigen = table.Column<int>(type: "int", nullable: false),
                    ColumnaOrigen = table.Column<int>(type: "int", nullable: false),
                    FilaDestino = table.Column<int>(type: "int", nullable: false),
                    ColumnaDestino = table.Column<int>(type: "int", nullable: false),
                    OrientacionPunto = table.Column<byte>(type: "tinyint", nullable: true),
                    EstadoTablero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TiempoTranscurrido = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugadas", x => x.JugadaId);
                    table.ForeignKey(
                        name: "FK_Jugadas_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "PartidaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Estadisticas",
                columns: new[] { "EstadisticaId", "JugadorEquipo", "ModoJuego", "PartidasGanadas", "PartidasJugadas" },
                values: new object[,]
                {
                    { 1, "Jugador1", (byte)2, 0, 0 },
                    { 2, "Jugador2", (byte)2, 0, 0 },
                    { 3, "EquipoA", (byte)4, 0, 0 },
                    { 4, "EquipoB", (byte)4, 0, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estadisticas_Modo_Jugador",
                table: "Estadisticas",
                columns: new[] { "ModoJuego", "JugadorEquipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jugadas_PartidaId",
                table: "Jugadas",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_Fecha",
                table: "Partidas",
                column: "FechaHoraCreacion",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estadisticas");

            migrationBuilder.DropTable(
                name: "Jugadas");

            migrationBuilder.DropTable(
                name: "Partidas");
        }
    }
}
