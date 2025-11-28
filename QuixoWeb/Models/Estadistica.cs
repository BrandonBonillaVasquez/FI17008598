using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuixoWeb.Models
{
    public class Estadistica
    {
        [Key]
        public int EstadisticaId { get; set; }

        [Required]
        public byte ModoJuego { get; set; } // 2 o 4

        [Required]
        [MaxLength(20)]
        public string JugadorEquipo { get; set; } = string.Empty; // "Jugador1", "Jugador2", "EquipoA", "EquipoB"

        public int PartidasGanadas { get; set; } = 0;

        public int PartidasJugadas { get; set; } = 0;

        // Propiedad calculada
        [NotMapped]
        public decimal Efectividad => PartidasJugadas > 0
            ? Math.Round((decimal)PartidasGanadas * 100 / PartidasJugadas, 2)
            : 0;
    }
}