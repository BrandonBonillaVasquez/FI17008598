using Microsoft.EntityFrameworkCore;
using QuixoWeb.Models;

namespace QuixoWeb.Data
{
    public class QuixoDbContext : DbContext
    {
        public QuixoDbContext(DbContextOptions<QuixoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Jugada> Jugadas { get; set; }
        public DbSet<Estadistica> Estadisticas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Partida
            modelBuilder.Entity<Partida>(entity =>
            {
                entity.HasKey(e => e.PartidaId);
                entity.Property(e => e.Estado).HasDefaultValue("EnCurso");
                entity.Property(e => e.FechaHoraCreacion).HasDefaultValueSql("GETDATE()");

                entity.HasIndex(e => e.FechaHoraCreacion)
                    .HasDatabaseName("IX_Partidas_Fecha")
                    .IsDescending();
            });

            // Configuración de Jugada
            modelBuilder.Entity<Jugada>(entity =>
            {
                entity.HasKey(e => e.JugadaId);

                entity.HasOne(e => e.Partida)
                    .WithMany(p => p.Jugadas)
                    .HasForeignKey(e => e.PartidaId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.PartidaId)
                    .HasDatabaseName("IX_Jugadas_PartidaId");
            });

            // Configuración de Estadistica
            modelBuilder.Entity<Estadistica>(entity =>
            {
                entity.HasKey(e => e.EstadisticaId);

                entity.HasIndex(e => new { e.ModoJuego, e.JugadorEquipo })
                    .IsUnique()
                    .HasDatabaseName("IX_Estadisticas_Modo_Jugador");
            });

            // Seed de datos iniciales para Estadísticas
            SeedEstadisticas(modelBuilder);
        }

        private void SeedEstadisticas(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estadistica>().HasData(
                // Modo 2 jugadores
                new Estadistica { EstadisticaId = 1, ModoJuego = 2, JugadorEquipo = "Jugador1", PartidasGanadas = 0, PartidasJugadas = 0 },
                new Estadistica { EstadisticaId = 2, ModoJuego = 2, JugadorEquipo = "Jugador2", PartidasGanadas = 0, PartidasJugadas = 0 },

                // Modo 4 jugadores
                new Estadistica { EstadisticaId = 3, ModoJuego = 4, JugadorEquipo = "EquipoA", PartidasGanadas = 0, PartidasJugadas = 0 },
                new Estadistica { EstadisticaId = 4, ModoJuego = 4, JugadorEquipo = "EquipoB", PartidasGanadas = 0, PartidasJugadas = 0 }
            );
        }
    }
}